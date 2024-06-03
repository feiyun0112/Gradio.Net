
using Gradio.Net.Enums;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace Gradio.Net;

public abstract class Block
{
    private bool _selectable;

    internal async Task SetSelectable(bool selectable)
    {
        _selectable = selectable;
    }
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
        Dictionary<string, object> result = new()
        {
            ["id"] = Id
        };

        return result;
    }

    internal virtual Dictionary<string, object> GetConfig()
    {
        Dictionary<string, object> blockConfig = new()
        {
            ["id"] = Id,
            ["type"] = GetTypeName(),
            ["props"] = GetProps(),
            ["skip_api"] = true,
            ["key"] = Key,

            ["component_class_id"] = GetComponentClassId()
        };

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
    internal bool? Streaming { get;  set; }
    internal bool? Likeable { get;  set; }

    protected virtual Dictionary<string, object> GetProps() {
        Dictionary<string, object> result = [];

        Type type = this.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance |
                        BindingFlags.NonPublic);
        foreach (PropertyInfo p in properties)
        {
            if (p.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
            {
                continue;
            }
            string name = p.Name.ToSnakeCase();
            object? value = p.GetValue(this, null);
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
                    IEnumerable? tmpList = value as IEnumerable;
                    List<string>? listValue = tmpList?.Cast<object>().Select(item => item.ToString().ToSnakeCase().ToLowerInvariant()).ToList();
                    result[name] = listValue;
                }
                else
                {
                    result[name] = value;
                } 
            }
          }

        result["_selectable"] = _selectable;
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
