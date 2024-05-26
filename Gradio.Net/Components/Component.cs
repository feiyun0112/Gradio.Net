using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gradio.Net
{
    public abstract class Component : Block
    {
        internal Component()
        {

        }

        internal override Dictionary<string, object> GetConfig()
        {
            var blockConfig = base.GetConfig();

            blockConfig["component_class_id"] = GetComponentClassId();

            return blockConfig;
        }

        private string GetComponentClassId()
        {
            return GetType().Name;
        }
        internal bool Rtl { get; set; }
        internal string Info { get; set; }
        internal bool Container { get; set; } = true;
        internal int? Scale { get; set; }
        internal int? MinWidth { get; set; }
        internal bool? Interactive { get; set; }
        [IgnoreDataMember]
        internal Action LoadFn { get; set; }


        internal object Value { get; set; }
        internal string Label { get; set; }
        internal float? Every { get; set; }
        internal bool ShowLabel { get; set; }
        
    
        protected virtual Dictionary<string, object> GetApiInfo() { 
        return new Dictionary<string, object>();
        }

        

        protected override Dictionary<string, object> GetProps()
        {
            var result= base.GetProps();

            result["_selectable"] = false;

            return result;
        }
    }
}
