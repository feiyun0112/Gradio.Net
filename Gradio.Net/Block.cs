
using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net
{
    public abstract class Block
    {
        internal Block()
        {
            Id = Context.NextId();
        }
        internal int Id { get; }
        internal string Key { get; set; }

        [IgnoreDataMember]
        internal bool Render { get; set; }
        internal string ElemId { get; set; }
        private IEnumerable<string> elemClasses;
        internal IEnumerable<string> ElemClasses { get { return elemClasses; } set { if (value == null) { value = new List<string>(); } elemClasses = value; } }

        internal virtual Dictionary<string, object> GetLayout()
        {
            var result = new Dictionary<string, object>();
            result["id"] = Id;

            return result;
        }

        internal virtual Dictionary<string, object> GetConfig()
        {
            var blockConfig = new Dictionary<string, object>();
            blockConfig["id"] = Id;
            blockConfig["type"] = GetType().Name.ToLower();
            blockConfig["props"] = GetProps();
            blockConfig["skip_api"] = true;
            blockConfig["key"] = Key;

            return blockConfig;
        }
        internal bool Visible { get; set; } = true;
        protected virtual Dictionary<string, object> GetProps() {
            var result = new Dictionary<string, object>();

            var type= this.GetType();
            var properties = type.GetProperties(BindingFlags.Instance |
                            BindingFlags.NonPublic);
            foreach (var p in properties)
            {
                if (p.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
                {
                    continue;
                }
                string name = p.Name.ToSnakeCase();
                var value = p.GetValue(this, null);
                if (value != null)
                {
                    if (p.PropertyType.Namespace==typeof(ShowProgress).Namespace)
                    {
                        result[name] = value.ToString().ToSnakeCase().ToLowerInvariant();
                    }
                    else
                    {
                        result[name] = value;
                    } 
                }
              }

            result["name"] = this.GetName();
            

            return result;
        }

        protected virtual string GetName()
        {
            return this.GetType().Name.ToLower();
        }
    }
}
