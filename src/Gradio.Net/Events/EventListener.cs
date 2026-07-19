namespace Gradio.Net.Events;

public class EventListener
{
    public EventListener(EventAttribute attribute)
    {
        EventName = attribute.EventName;
        Handler = attribute.Handler;
        EventSpecificArgs = new List<EventArg>();
        HasTrigger = true;
        ShowProgress = "full";
        Connection = "sse";
    }

    public EventListener(
        string eventName,
        Delegate? handler = null,
        bool hasTrigger = true,
        Func<Dictionary<string, object>>? configData = null,
        string showProgress = "full",
        Action<object>? callback = null,
        int? triggerAfter = null,
        bool triggerOnlyOnSuccess = false,
        bool triggerOnlyOnFailure = false,
        string doc = "",
        string connection = "sse",
        List<EventArg>? eventSpecificArgs = null)
    {
        EventName = eventName;
        Handler = handler;
        HasTrigger = hasTrigger;
        ConfigData = configData ?? (() => new Dictionary<string, object>());
        ShowProgress = showProgress;
        Callback = callback;
        TriggerAfter = triggerAfter;
        TriggerOnlyOnSuccess = triggerOnlyOnSuccess;
        TriggerOnlyOnFailure = triggerOnlyOnFailure;
        Doc = doc;
        Connection = connection;
        EventSpecificArgs = eventSpecificArgs ?? new List<EventArg>();
        Listener = new EventListenerMethod();
    }

    public string EventName { get; set; }
    public Delegate? Handler { get; set; }
    public bool HasTrigger { get; set; }
    public Func<Dictionary<string, object>> ConfigData { get; set; }
    public string ShowProgress { get; set; }
    public Action<object>? Callback { get; set; }
    public int? TriggerAfter { get; set; }
    public bool TriggerOnlyOnSuccess { get; set; }
    public bool TriggerOnlyOnFailure { get; set; }
    public string Doc { get; set; }
    public string Connection { get; set; }
    public bool IsPropagationStopped { get; set; }
    public bool IsImmediatePropagationStopped { get; set; }
    public EventListenerMethod Listener { get; set; }
    public List<EventArg> EventSpecificArgs { get; set; }
    public object? Self { get; set; }

    public Dictionary<string, object> GetConfigData()
    {
        var config = ConfigData();
        config["event_name"] = EventName;
        return config;
    }

    public void Invoke(params object[] args)
    {
        Handler?.DynamicInvoke(args);
    }

    public void StopPropagation()
    {
        IsPropagationStopped = true;
    }

    public void StopImmediatePropagation()
    {
        IsImmediatePropagationStopped = true;
        IsPropagationStopped = true;
    }

    public EventListener Copy()
    {
        var copy = new EventListener(
            EventName,
            Handler,
            HasTrigger,
            ConfigData,
            ShowProgress,
            Callback,
            TriggerAfter,
            TriggerOnlyOnSuccess,
            TriggerOnlyOnFailure,
            Doc,
            Connection,
            new List<EventArg>(EventSpecificArgs))
        {
            IsPropagationStopped = IsPropagationStopped,
            IsImmediatePropagationStopped = IsImmediatePropagationStopped,
            Listener = Listener.Copy()
        };
        return copy;
    }

    public void SetDoc(string component)
    {
        if (!string.IsNullOrEmpty(Doc))
        {
            Doc = Doc.Replace("{{ component }}", component);
        }
    }
}

