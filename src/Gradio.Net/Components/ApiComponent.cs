namespace Gradio.Net.Components
{
    public class ApiComponent : Component
    {
        private readonly Dictionary<string, string> _apiInfo;

        public ApiComponent(object? value = null, Dictionary<string, string>? apiInfo = null, string label = "API")
        {
            _apiInfo = apiInfo ?? new Dictionary<string, string>();
            Label = label;
            Value = value;
        }

        public override object Preprocess(object payload) => payload;

        public override object Postprocess(object value) => value;

        public override Dictionary<string, object> ApiInfo()
            => _apiInfo.ToDictionary(kv => kv.Key, kv => (object)kv.Value);

        public override object ExamplePayload() => Value ?? "...";

        public override object ExampleValue() => Value ?? "...";

        public override string GetBlockName() => "state";
    }
}
