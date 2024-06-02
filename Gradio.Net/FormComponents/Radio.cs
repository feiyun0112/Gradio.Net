using Gradio.Net.Enums;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace Gradio.Net
{
    public class Radio : FormComponent, IHaveChangeEvent, IHaveInputEvent,IHaveSelectEvent
    {
        internal Radio() { }
        internal IEnumerable<string> Choices { get;  set; }
        internal RadioType Type { get;  set; }

        public static IEnumerable<string> Payload(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is IEnumerable<string> list)
            {
                return list;
            }

            throw new ArgumentException($"Payload Type expect IEnumerable<string> actual {obj.GetType()}");
        }

        protected override Dictionary<string, object> GetProps()
        {
            var result = base.GetProps();

            result["choices"]= this.Choices.Select(x => new[] { x,x}).ToArray();

            return result;
        }

        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }

        internal override object PreProcess(object data)
        {
            
            if (data == null)
            {
                return new List<string>();
            }

            if (data is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    return new[] { data.ToString() };
                }

                if (element.ValueKind == JsonValueKind.Array)
                {
                    return JsonUtils.Deserialize<string[]>(data.ToString());
                }

            }
            var str = data.ToString();

            var choices = JsonUtils.Deserialize<string[]>(str);
            if (choices != null)
            {
                return choices; 
            }
            
            return new[] { str };
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null)
            {
                return new List<string>();
            }
            if (data is IEnumerable<string>)
            {
                return data;
            }

            var str = data.ToString();

            var choices = JsonUtils.Deserialize<string[]>(str);
            if (choices != null)
            {
                return choices;
            }

            return new[] { str };
        }


    }
}
