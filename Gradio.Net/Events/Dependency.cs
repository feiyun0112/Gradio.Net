using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal class Dependency
    {
        public Dependency(Block block, Dictionary<string, object> config, int dep_index, Func<Input,Task<Output>> fn)
        {
            Block = block;
            Config = config;
            DepIndex = dep_index;
            Fn = fn;
        }

        internal Block Block { get; }
        internal Dictionary<string, object> Config { get; }
        internal int DepIndex { get; }
        internal Func<Input,Task<Output>> Fn { get; }
    }
}
