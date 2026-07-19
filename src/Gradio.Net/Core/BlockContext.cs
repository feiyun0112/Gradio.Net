using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Gradio.Net.Utils;

namespace Gradio.Net.Core
{
    public class BlockContext : Block, IDisposable
    {
        private static readonly Stack<Blocks> BlocksContextStack = new Stack<Blocks>();
        private static readonly Stack<BlockContext> RenderContextStack = new Stack<BlockContext>();

        public List<Block> Children { get; set; }
        public bool ShowProgress { get; set; }
        public bool AllowExpectedParents { get; set; }

        public BlockContext(
            string elemId = null,
            List<string> elemClasses = null,
            bool visible = true,
            bool render = true,
            object key = null,
            List<string> preservedByKey = null,
            string proxyUrl = null)
            : base(elemId, elemClasses, visible, render, key, preservedByKey, proxyUrl)
        {
            Children = new List<Block>();
            ShowProgress = false;
            AllowExpectedParents = true;
        }

        public static string TEMPLATE_DIR = "./templates/";
        public static string FRONTEND_DIR = "../../frontend/";

        public override bool SkipApi => true;

        private static string ComputeComponentClassId(Type blockType)
        {
            // Use runtime type to avoid collisions between different layout classes
            // (e.g. Row and Form) that would otherwise share BlockContext's class id.
            var className = blockType.FullName ?? blockType.Name;
            var assemblyName = blockType.Assembly.GetName().Name ?? string.Empty;
            var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{className}_{assemblyName}"));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public static string GetComponentClassId()
        {
            // Backward-compatible static API.
            return ComputeComponentClassId(typeof(BlockContext));
        }

        public string ComponentClassId => ComputeComponentClassId(GetType());

        public virtual void AddChild(Block child)
        {
            Children.Add(child);
        }

        public void Add(Block child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void FillExpectedParents()
        {
            var rootContext = Context.GetBlocksContext();
            var newChildren = new List<Block>();
            BlockContext pseudoParent = null;

            // Iterate over a snapshot because creating pseudo parents can trigger
            // constructor-side rendering that mutates Children.
            foreach (var child in new List<Block>(Children))
            {
                var expectedParentType = child.GetExpectedParent();

                // If no expected parent or this is already the expected parent type
                if (expectedParentType == null || expectedParentType.IsInstanceOfType(this))
                {
                    if (child.BreaksGrouping())
                    {
                        pseudoParent = null;
                    }
                    newChildren.Add(child);
                }
                else
                {
                    // Check if we can reuse existing pseudo parent
                    if (pseudoParent != null && expectedParentType.IsInstanceOfType(pseudoParent))
                    {
                        pseudoParent.AddChild(child);
                    }
                    else
                    {
                        // Create new pseudo parent
                        object key = null;
                        if (child.Key != null)
                        {
                            if (child.Key is ValueTuple)
                            {
                                // Append "_parent" to tuple key
                                key = (child.Key, "_parent");
                            }
                            else
                            {
                                key = (child.Key, "_parent");
                            }
                        }

                        // Instantiate the expected parent type with Python parity:
                        // expected_parent(render=False, key=key)
                        pseudoParent = CreatePseudoParent(expectedParentType, key);
                        if (pseudoParent.Parent != null && !ReferenceEquals(pseudoParent.Parent, this))
                        {
                            pseudoParent.Parent.Children.Remove(pseudoParent);
                        }

                        pseudoParent.Parent = this;
                        pseudoParent.Key = key;
                        newChildren.Add(pseudoParent);
                        pseudoParent.AddChild(child);
                        pseudoParent.Page = child.Page;

                        // Register pseudo parent with root context
                        if (rootContext != null)
                        {
                            rootContext.Blocks[pseudoParent._id] = pseudoParent;
                            pseudoParent.RenderSequence = Block.GenerateRenderSequence();
                        }
                    }
                    child.Parent = pseudoParent;
                }
            }

            Children = newChildren;
        }

        private static BlockContext CreatePseudoParent(Type expectedParentType, object key)
        {
            // Prefer constructors where all non-overridden parameters are optional,
            // then override render=false and key=providedKey.
            foreach (var ctor in expectedParentType.GetConstructors()
                                                   .OrderByDescending(c => c.GetParameters().Any(p => p.Name == "render"))
                                                   .ThenBy(c => c.GetParameters().Length))
            {
                var parameters = ctor.GetParameters();
                bool canUse = true;

                foreach (var p in parameters)
                {
                    if (p.Name == "render" || p.Name == "key")
                    {
                        continue;
                    }

                    if (!p.IsOptional)
                    {
                        canUse = false;
                        break;
                    }
                }

                if (!canUse)
                {
                    continue;
                }

                var args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];

                    if (p.Name == "render")
                    {
                        args[i] = false;
                    }
                    else if (p.Name == "key")
                    {
                        args[i] = key;
                    }
                    else if (p.IsOptional)
                    {
                        args[i] = p.DefaultValue;
                    }
                    else if (p.ParameterType.IsValueType)
                    {
                        args[i] = Activator.CreateInstance(p.ParameterType);
                    }
                    else
                    {
                        args[i] = null;
                    }
                }

                return (BlockContext)ctor.Invoke(args);
            }

            // Fallback: best effort.
            return (BlockContext)Activator.CreateInstance(expectedParentType);
        }

        public BlockContext Enter()
        {
            var renderContext = GetRenderContext();
            Parent = renderContext;
            SetRenderContext(this);
            return this;
        }

        public void Exit()
        {
            // Pop this context from the stack.
            if (RenderContextStack.Count > 0)
                RenderContextStack.Pop();

            // Directly restore the parent context, matching Python's set_render_context(self.parent).
            // This correctly handles both normal contexts and renderable contexts (LocalContext.RenderBlock).
            Context.SetRenderContext(Parent as BlockContext);

            if (AllowExpectedParents)
            {
                FillExpectedParents();
            }
        }

        public void Dispose()
        {
            // Only exit if this context is currently active.
            // This keeps Dispose idempotent and avoids double-exit after manual Exit().
            if (ReferenceEquals(GetRenderContext(), this))
            {
                Exit();
            }

            GC.SuppressFinalize(this);
        }

        public object Postprocess(object y)
        {
            return y;
        }

        public static void EnterBlocksContext(Blocks blocks)
        {
            BlocksContextStack.Push(blocks);
        }

        public static void ExitBlocksContext()
        {
            if (BlocksContextStack.Count > 0)
            {
                BlocksContextStack.Pop();
            }
        }

        public static Blocks GetCurrentBlocks()
        {
            return BlocksContextStack.Count > 0 ? BlocksContextStack.Peek() : null;
        }

        public static void ClearBlocksContext()
        {
            BlocksContextStack.Clear();
        }

        public static BlockContext GetRenderContext()
        {
            var contextRender = Context.GetRenderContext();
            if (contextRender != null)
            {
                return contextRender;
            }

            return RenderContextStack.Count > 0 ? RenderContextStack.Peek() : null;
        }

        public static void SetRenderContext(BlockContext context)
        {
            if (context == null)
            {
                // Pop current context and restore the previous one.
                if (RenderContextStack.Count > 0)
                    RenderContextStack.Pop();
                var restored = RenderContextStack.Count > 0 ? RenderContextStack.Peek() : null;
                Context.SetRenderContext(restored);
            }
            else
            {
                RenderContextStack.Push(context);
                Context.SetRenderContext(context);
            }
        }

        public static void ClearRenderContext()
        {
            RenderContextStack.Clear();
        }
    }
}
