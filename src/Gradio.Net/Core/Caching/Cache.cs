using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A cache: entries an application looks up itself, and the wrapper that files a function's answers
/// in it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.Cache</c> and <c>gr.cache</c> are two ways into one store — the
/// handle a handler calls <c>get</c>/<c>set</c> on, and the decorator that makes a function's own
/// results the entries. They are one object here because a .NET application has one thing to hold
/// onto: the decorator is a method on the cache it writes to, which is what makes "clear this
/// cache" and "clear what this function cached" the same sentence.
/// </para>
/// <para>
/// A wrapped function is a delegate of the same type it was given, so a binding takes it wherever it
/// would have taken the original. What it carries besides is the store, which is how the queue
/// recognises a cached binding and asks it whether the answer is already known instead of running it
/// to find out.
/// </para>
/// </remarks>
public sealed class Cache
{
    private readonly CacheStore _store;

    /// <summary>Creates a cache.</summary>
    /// <param name="options">How much it keeps and whether it keeps it per session, or <see langword="null"/> for the defaults.</param>
    public Cache(CacheOptions? options = null)
    {
        _store = new CacheStore(options ?? new CacheOptions());
    }

    /// <summary>How many entries the cache holds.</summary>
    public int Count => _store.Count;

    /// <summary>Whether each session gets its own namespace.</summary>
    /// <remarks>
    /// Read by the application when it says whether a client has to keep a connection open: a cache
    /// that forgets a session's entries when the session ends needs to know when it ends.
    /// </remarks>
    public bool PerSession => _store.PerSession;

    /// <summary>Wraps a function so that its answers are cached.</summary>
    /// <typeparam name="TDelegate">The function's delegate type.</typeparam>
    /// <param name="function">The function.</param>
    /// <param name="key">
    /// A function that turns the call's arguments into the value to key on, or <see langword="null"/>
    /// to key on all of them.
    /// </param>
    /// <returns>A delegate of the same type, which answers from the cache when it can.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="function"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The function returns nothing, so it has no answer to cache.</exception>
    /// <exception cref="NotSupportedException">The function's signature cannot be wrapped.</exception>
    /// <remarks>
    /// <para>
    /// Every shape a handler can have is cached: a value, a task, and a stream. A stream is cached
    /// as the chunks it produced, replayed to the next caller — which is what the Python version
    /// does with a generator, and what makes a cached streaming endpoint answer twice as fast the
    /// second time.
    /// </para>
    /// <para>
    /// The parameters the framework supplies — a <see cref="Progress"/>, a
    /// <see cref="CancellationToken"/>, an event's data — are not part of the key: they differ every
    /// run and are not inputs, so a key that included them would never be looked up twice.
    /// </para>
    /// </remarks>
    public TDelegate Wrap<TDelegate>(TDelegate function, Func<object?[], object?>? key = null)
        where TDelegate : Delegate
    {
        ArgumentNullException.ThrowIfNull(function);

        var invoke = typeof(TDelegate).GetMethod("Invoke")
            ?? throw new NotSupportedException($"{typeof(TDelegate).Name} is not a delegate type.");

        if (invoke.ReturnType == typeof(void))
        {
            throw new ArgumentException(
                "A cached function has to return something; there is no answer to cache otherwise.",
                nameof(function));
        }

        var parameters = invoke.GetParameters();

        if (parameters.Length > CachedInvoker.MaxArity)
        {
            throw new NotSupportedException(
                $"A cached function may take up to {CachedInvoker.MaxArity} parameters; "
                + $"this one takes {parameters.Length}.");
        }

        if (parameters.Any(parameter => parameter.ParameterType.IsByRef || parameter.ParameterType.IsPointer))
        {
            throw new NotSupportedException("A cached function cannot take ref, out or pointer parameters.");
        }

        var invoker = new CachedInvoker(this, function, key, parameters.Select(p => p.ParameterType).ToArray());

        return (TDelegate)Delegate.CreateDelegate(
            typeof(TDelegate),
            invoker,
            invoker.Binding(invoke.ReturnType));
    }

    /// <summary>Looks an entry up.</summary>
    /// <param name="key">The key.</param>
    /// <returns>What was stored under it, or <see langword="null"/> when nothing was.</returns>
    public IReadOnlyDictionary<string, object?>? Get(object? key)
    {
        var entry = _store.Get(CacheHash.Of(key));

        return entry is null
            ? null
            : new Dictionary<string, object?>(entry.Where(pair => pair.Key != CacheStore.KeyOfEntry));
    }

    /// <summary>Files data under a key.</summary>
    /// <param name="key">The key.</param>
    /// <param name="data">What to store.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    public void Set(object? key, IReadOnlyDictionary<string, object?> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        _store.Put(CacheHash.Of(key), new Dictionary<string, object?>(data) { [CacheStore.KeyOfEntry] = key });
    }

    /// <summary>Files named values under a key.</summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The values, by name.</param>
    public void Set(object? key, params (string Name, object? Value)[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Set(key, data.ToDictionary(pair => pair.Name, pair => pair.Value, StringComparer.Ordinal));
    }

    /// <summary>The keys this session can see.</summary>
    /// <returns>The keys, in the order they were filed.</returns>
    public IReadOnlyList<object?> Keys() => _store.Keys();

    /// <summary>Forgets everything.</summary>
    public void Clear() => _store.Clear();

    /// <summary>Forgets one session's entries.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <remarks>
    /// This is the Python version's <c>clear_session_caches</c> for one cache. A run opens its
    /// session before it calls anything, so a cache that keeps sessions apart is cleared here when
    /// the browser says the session is over.
    /// </remarks>
    public void ClearSession(string sessionHash) => _store.ClearSession(sessionHash);

    /// <summary>Runs something with a session open.</summary>
    /// <param name="sessionHash">The session whose namespace the work belongs to.</param>
    /// <returns>A scope that closes the session again.</returns>
    /// <remarks>
    /// The framework opens one for every run, which is what makes a per-session cache keep sessions
    /// apart without the handler having to say which session it is in. A host that calls a wrapped
    /// function itself — outside a run — opens one the same way.
    /// </remarks>
    public IDisposable OpenSession(string sessionHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionHash);

        return _store.OpenSession(sessionHash);
    }

    /// <summary>Runs something that may only ask whether an answer is already known.</summary>
    /// <returns>A scope that ends probe mode.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>ProbeCache</c>. Inside the scope a wrapped function answers from the
    /// cache when it can and raises <see cref="CacheMissError"/> when it cannot, without computing
    /// anything — which is what lets the queue answer a cached binding immediately instead of
    /// running it to find out whether it had to.
    /// </para>
    /// <para>
    /// The scope covers the cache rather than one function, as the Python version's does: the
    /// question being asked is about the call that is in flight, not about which wrapper it went
    /// through.
    /// </para>
    /// </remarks>
    public IDisposable Probe() => _store.Probe();

    /// <summary>The entries behind this handle.</summary>
    internal CacheStore Store => _store;
}

/// <summary>A wrapped function, as the framework sees it: something that can be asked to probe.</summary>
/// <remarks>
/// Implemented by the object the wrapped delegate's target is, which is how a binding is recognised
/// as cached without the framework keeping a registry of wrapped functions — a registry would be
/// static state, and the delegate already carries everything the queue needs.
/// </remarks>
internal interface ICachedHandler
{
    /// <summary>The cache the function writes to.</summary>
    Cache Cache { get; }
}

/// <summary>
/// The object behind a wrapped delegate: it hashes the call, answers from the cache, and files what
/// it computes.
/// </summary>
/// <remarks>
/// <para>
/// One method per parameter count rather than one method over an array, because the delegate the
/// wrapper has to be is the delegate it was given: the framework binds the handler by its own
/// signature, so a wrapper that took an array would be called with the first input as the array.
/// </para>
/// <para>
/// The return type decides how an answer is stored and how a hit is handed back: a value is stored
/// as itself, a task is stored as what it resolved to, and a stream is stored as the chunks it
/// produced.
/// </para>
/// </remarks>
internal sealed class CachedInvoker : ICachedHandler
{
    /// <summary>How many parameters a wrapped function may take.</summary>
    internal const int MaxArity = 6;

    private static readonly MethodInfo AwaitAndStoreMethod = typeof(CachedInvoker)
        .GetMethod(nameof(AwaitAndStoreAsync), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo StreamedMethod = typeof(CachedInvoker)
        .GetMethod(nameof(StreamedCore), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly MethodInfo ReplayMethod = typeof(CachedInvoker)
        .GetMethod(nameof(ReplayCore), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly MethodInfo FromResultMethod = typeof(Task)
        .GetMethod(nameof(Task.FromResult))!;

    private readonly Cache _cache;
    private readonly Delegate _function;
    private readonly Func<object?[], object?>? _key;
    private readonly Type[] _parameterTypes;

    /// <summary>Which arguments are part of the key: the ones that are inputs.</summary>
    private readonly int[] _hashable;

    internal CachedInvoker(Cache cache, Delegate function, Func<object?[], object?>? key, Type[] parameterTypes)
    {
        _cache = cache;
        _function = function;
        _key = key;
        _parameterTypes = parameterTypes;
        _hashable = [.. Enumerable.Range(0, parameterTypes.Length).Where(IsInput)];
    }

    /// <inheritdoc />
    public Cache Cache => _cache;

    /// <summary>The method a delegate of this signature is bound to.</summary>
    /// <param name="returnType">The delegate's return type.</param>
    /// <returns>The closed method.</returns>
    /// <remarks>
    /// The return type travels as a type argument rather than as a branch inside one method, because
    /// the method has to <em>return</em> it: a wrapper for a task-returning function is a method that
    /// returns a task.
    /// </remarks>
    internal MethodInfo Binding(Type returnType)
    {
        var arguments = new Type[_parameterTypes.Length + 1];
        _parameterTypes.CopyTo(arguments, 0);
        arguments[^1] = returnType;

        return typeof(CachedInvoker)
            .GetMethod($"Invoke{_parameterTypes.Length}", BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(arguments);
    }

    private TResult Invoke0<TResult>() => (TResult)Call([], typeof(TResult))!;

    private TResult Invoke1<T1, TResult>(T1 a1) => (TResult)Call([a1], typeof(TResult))!;

    private TResult Invoke2<T1, T2, TResult>(T1 a1, T2 a2) => (TResult)Call([a1, a2], typeof(TResult))!;

    private TResult Invoke3<T1, T2, T3, TResult>(T1 a1, T2 a2, T3 a3) =>
        (TResult)Call([a1, a2, a3], typeof(TResult))!;

    private TResult Invoke4<T1, T2, T3, T4, TResult>(T1 a1, T2 a2, T3 a3, T4 a4) =>
        (TResult)Call([a1, a2, a3, a4], typeof(TResult))!;

    private TResult Invoke5<T1, T2, T3, T4, T5, TResult>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) =>
        (TResult)Call([a1, a2, a3, a4, a5], typeof(TResult))!;

    private TResult Invoke6<T1, T2, T3, T4, T5, T6, TResult>(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) =>
        (TResult)Call([a1, a2, a3, a4, a5, a6], typeof(TResult))!;

    /// <summary>Answers a call: from the cache when it can, by computing when it must.</summary>
    /// <param name="arguments">The call's arguments.</param>
    /// <param name="returnType">What the delegate returns.</param>
    /// <returns>The answer, in the shape the delegate returns.</returns>
    /// <exception cref="CacheMissError">The answer is not cached and the run is only probing.</exception>
    private object? Call(object?[] arguments, Type returnType)
    {
        var keyHash = KeyOf(arguments);

        if (_cache.Store.Get(keyHash) is { } hit)
        {
            return Shaped(hit, returnType);
        }

        if (_cache.Store.Probing)
        {
            throw new CacheMissError();
        }

        object? result;
        try
        {
            result = _function.DynamicInvoke(arguments);
        }
        catch (TargetInvocationException failure) when (failure.InnerException is not null)
        {
            // The handler's own failure, not the reflection's: rethrowing it wrapped would change
            // what the application's error handling sees.
            ExceptionDispatchInfo.Capture(failure.InnerException).Throw();

            throw;
        }

        if (result is Task task)
        {
            return AwaitAndStore(task, keyHash, returnType);
        }

        // A value task is not a task: it is a struct that wraps one, and what has to be waited on is
        // the task it can hand out — once, because asking twice is not allowed.
        if (IsValueTask(returnType))
        {
            var pending = (Task)returnType.GetMethod(nameof(ValueTask<int>.AsTask))!.Invoke(result, null)!;

            return AwaitAndStore(pending, keyHash, returnType);
        }

        if (IsStream(returnType))
        {
            return Streamed(result!, keyHash, returnType);
        }

        _cache.Store.Put(keyHash, new Dictionary<string, object?> { [CacheStore.ValueOfEntry] = Snapshot(result) });

        return result;
    }

    /// <summary>The digest a call is filed under.</summary>
    /// <param name="arguments">The call's arguments.</param>
    /// <returns>The digest.</returns>
    /// <remarks>
    /// A key function sees the arguments as they were given, which is the Python version's
    /// <c>key(kwargs)</c>: it exists so that an application can say "only the prompt matters"
    /// without the framework having to guess which arguments are noise.
    /// </remarks>
    private string KeyOf(object?[] arguments) =>
        _key is not null
            ? CacheHash.Of(_key(arguments))
            : CacheHash.Of(_hashable.Select(index => arguments[index]).ToArray());

    /// <summary>A stored answer, in the shape the delegate returns.</summary>
    /// <param name="entry">The stored entry.</param>
    /// <param name="returnType">What the delegate returns.</param>
    /// <returns>The answer.</returns>
    private static object? Shaped(Dictionary<string, object?> entry, Type returnType)
    {
        var value = entry.TryGetValue(CacheStore.ValueOfEntry, out var stored) ? stored : null;

        if (!returnType.IsGenericType)
        {
            return value;
        }

        var shape = returnType.GetGenericTypeDefinition();

        if (shape == typeof(Task<>))
        {
            return FromResultMethod.MakeGenericMethod(returnType.GetGenericArguments()[0]).Invoke(null, [value]);
        }

        if (shape == typeof(ValueTask<>))
        {
            return Activator.CreateInstance(
                returnType,
                FromResultMethod.MakeGenericMethod(returnType.GetGenericArguments()[0]).Invoke(null, [value]))!;
        }

        if (shape == typeof(IAsyncEnumerable<>))
        {
            return ReplayMethod.MakeGenericMethod(returnType.GetGenericArguments()[0]).Invoke(null, [value])!;
        }

        return value;
    }

    /// <summary>Waits for a task, files what it resolved to, and hands the answer back.</summary>
    /// <param name="task">The task the handler returned.</param>
    /// <param name="keyHash">The digest to file it under.</param>
    /// <param name="returnType">What the delegate returns.</param>
    /// <returns>A task of the delegate's own type.</returns>
    private object AwaitAndStore(Task task, string keyHash, Type returnType)
    {
        var resultType = returnType.IsGenericType ? returnType.GetGenericArguments()[0] : typeof(object);

        return AwaitAndStoreMethod.MakeGenericMethod(resultType).Invoke(this, [task, keyHash, returnType])!;
    }

    private object AwaitAndStoreAsync<T>(Task task, string keyHash, Type returnType)
    {
        var pending = AwaitAndStoreCoreAsync((Task<T>)task, keyHash);

        return returnType == typeof(Task<T>)
            ? pending
            : Activator.CreateInstance(returnType, pending)!;
    }

    private async Task<T> AwaitAndStoreCoreAsync<T>(Task<T> task, string keyHash)
    {
        var value = await task.ConfigureAwait(false);

        _cache.Store.Put(keyHash, new Dictionary<string, object?> { [CacheStore.ValueOfEntry] = Snapshot(value) });

        return value;
    }

    /// <summary>Hands a stream back, recording its chunks for the next caller.</summary>
    /// <param name="source">The stream the handler returned.</param>
    /// <param name="keyHash">The digest to file the chunks under.</param>
    /// <param name="returnType">What the delegate returns.</param>
    /// <returns>The stream to hand to the caller.</returns>
    private object Streamed(object source, string keyHash, Type returnType) =>
        StreamedMethod.MakeGenericMethod(returnType.GetGenericArguments()[0]).Invoke(this, [source, keyHash])!;

    private static CachedStream<T> ReplayCore<T>(object? cached) =>
        new(cached as IReadOnlyList<T> ?? []);

    private CachedStream<T> StreamedCore<T>(object source, string keyHash) =>
        new CachedStream<T>(
            (IAsyncEnumerable<T>)source,
            cached: null,
            chunks => _cache.Store.Put(keyHash, new Dictionary<string, object?> { [CacheStore.ValueOfEntry] = chunks }));

    /// <summary>Whether a return type is a stream rather than a value.</summary>
    /// <param name="returnType">The type.</param>
    /// <returns><see langword="true"/> when it is an asynchronous sequence.</returns>
    private static bool IsStream(Type returnType) =>
        returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);

    /// <summary>Whether a return type is a value task rather than a task.</summary>
    /// <param name="returnType">The type.</param>
    /// <returns><see langword="true"/> when it is a <see cref="ValueTask{TResult}"/>.</returns>
    private static bool IsValueTask(Type returnType) =>
        returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>);

    /// <summary>Whether a parameter is an input rather than something the framework supplies.</summary>
    /// <param name="index">Which parameter.</param>
    /// <returns><see langword="true"/> when its value is part of the key.</returns>
    private bool IsInput(int index)
    {
        var type = _parameterTypes[index];

        return !DelegateInvocation.IsSuppliedByRun(type) && !EventData.Is(type);
    }

    /// <summary>A value as it is stored, so that a later mutation cannot change what was cached.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The value to store.</returns>
    private static object? Snapshot(object? value) => value is JsonNode node ? node.DeepClone() : value;
}
