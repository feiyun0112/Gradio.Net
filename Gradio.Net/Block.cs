
using Gradio.Net.Enums;
using System;
using System.Collections;
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
        internal bool? Interactive { get; set; }


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
            blockConfig["type"] = GetTypeName();
            blockConfig["props"] = GetProps();
            blockConfig["skip_api"] = true;
            blockConfig["key"] = Key;

            blockConfig["component_class_id"] = GetComponentClassId();

            return blockConfig;
        }
        protected virtual string GetTypeName()
        {
            return GetType().Name.ToLower();
        }


        private string GetComponentClassId()
        {
            return GetType().Name;
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
                    if (p.PropertyType.Namespace == typeof(ShowProgress).Namespace)
                    {
                        result[name] = value.ToString().ToSnakeCase().ToLowerInvariant();
                    }
                    else if (p.PropertyType.IsGenericType  
                        && typeof(IEnumerable).IsAssignableFrom(p.PropertyType) 
                        && p.PropertyType.GenericTypeArguments.Count()==1
                        && p.PropertyType.GenericTypeArguments[0].Namespace == typeof(ShowProgress).Namespace
                        )
                    {
                        var tmpList = value as IEnumerable;
                        var listValue = tmpList?.Cast<object>().Select(item => item.ToString().ToSnakeCase().ToLowerInvariant()).ToList();
                        result[name] = listValue;
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

        internal virtual object PreProcess(object data)
        {
            return data;
        }

        internal virtual object PostProcess(string rootUrl, object data)
        {
            return data;
        }
    }
}
