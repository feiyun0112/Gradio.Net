
using Gradio.Net.Enums;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Gradio.Net;

public abstract class Block
{
    private bool _selectable;
    private Dictionary<string, object> _updateProps = new();

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
    internal bool? Render { get; set; }
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
            ["type"] = this.GetTypeName(),
            ["props"] = this.GetProps(true),
            ["skip_api"] = true,
            ["key"] = Key,

            ["component_class_id"] = this.GetComponentClassId()
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
    internal bool? Visible { get; set; } = true;
    internal bool? Streaming { get; set; }
    internal bool? Likeable { get; set; }

    protected virtual object? GetDefaultProp(string name)
    {
        return null;
    }

    internal T GetPropertyValue<T>(string propertyName)
    {
        PropertyInfo? p = this.GetType().GetProperty(propertyName, BindingFlags.Instance |
                         BindingFlags.NonPublic);
        if (p == null)
        {
            throw new Exception($"{this.GetType()} no property {propertyName}");
        }

        object? value = this.GetPropValue(p);
        if (value != null && value is T retValue)
        {
            return retValue;
        }

        return default(T);
    }

    private object? GetPropValue(PropertyInfo? p, bool useDefaultValue = true)
    {
        object? value = p.GetValue(this, null);
        if (value == null && useDefaultValue)
        {
            value = this.GetDefaultProp(p.Name);
        }

        return value;
    }

    protected Dictionary<string, object> GetUpdatedProps()
    {
        return _updateProps;
    }

    protected virtual Dictionary<string, object> GetProps(bool useDefaultValue)
    {
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
            object? value = this.GetPropValue(p);
            if (value != null)
            {
                Type underlyingType = Nullable.GetUnderlyingType(p.PropertyType);
                if (underlyingType == null)
                {
                    underlyingType = p.PropertyType;
                }
                if (underlyingType.Namespace == typeof(ShowProgress).Namespace)
                {
                    result[name] = value.ToString().ToSnakeCase().ToLowerInvariant();
                }
                else if (underlyingType.IsGenericType
                    && typeof(IEnumerable).IsAssignableFrom(underlyingType)
                    && underlyingType.GenericTypeArguments.Count() == 1
                    && underlyingType.GenericTypeArguments[0].Namespace == typeof(ShowProgress).Namespace
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

    internal static object PostProcess(Block blockUpdate, Block block)
    {
        Dictionary<string, object> blockPropsUpdate = blockUpdate.GetProps(false);
        Dictionary<string, object> blockProps = block.GetProps(false);
        Dictionary<string, object> blockUpdatedProps = block.GetUpdatedProps();
        Dictionary<string, object> result = new();
        foreach (KeyValuePair<string, object> prop in blockPropsUpdate)
        {
            if (prop.Key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }
            object valueUpdate = prop.Value;
            if (blockProps.ContainsKey(prop.Key))
            {
                object value = blockProps[prop.Key];

                if (value == null && valueUpdate != null)
                {
                    result[prop.Key] = valueUpdate;
                    blockUpdatedProps[prop.Key] = valueUpdate;
                }
                else if (value != null && valueUpdate != null)
                {
                    if (blockUpdatedProps.ContainsKey(prop.Key))
                    {
                        value = blockUpdatedProps[prop.Key];
                    }

                    if (!JsonUtils.Serialize(value).Equals(JsonUtils.Serialize(valueUpdate)))
                    {
                        result[prop.Key] = valueUpdate;
                        blockUpdatedProps[prop.Key] = valueUpdate;
                    }
                }
            }
            else
            {
                result[prop.Key] = valueUpdate;
                blockUpdatedProps[prop.Key] = valueUpdate;
            }
        }
        result["__type__"] = "update";

        return result;
    }

    internal void Init()
    {
        _updateProps.Clear();
    }
}
