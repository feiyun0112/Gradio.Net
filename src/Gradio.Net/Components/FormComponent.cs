
namespace Gradio.Net.Components
{
    public class FormComponent : Component
    {
        protected FormComponent() { }

        protected FormComponent(bool _proxyMode) : base(_proxyMode) { }

        public override Type GetExpectedParent()
        {
            if (Container == false) return null;
            return typeof(Core.Layouts.Form);
        }

        public override bool BreaksGrouping() => false;

        public override object Preprocess(object payload) => payload;

        public override object Postprocess(object value) => value;
    }
}
