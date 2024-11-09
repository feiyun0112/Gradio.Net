import { c as create_ssr_component, o as onDestroy } from './ssr-RaXq3SJh.js';

const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { gradio } = $$props;
  let { value = 1 } = $$props;
  let { active = true } = $$props;
  let old_value;
  let old_active;
  let interval;
  onDestroy(() => {
    if (interval)
      clearInterval(interval);
  });
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.active === void 0 && $$bindings.active && active !== void 0)
    $$bindings.active(active);
  {
    if (old_value !== value || active !== old_active) {
      if (interval)
        clearInterval(interval);
      if (active) {
        interval = setInterval(
          () => {
            if (document.visibilityState === "visible")
              gradio.dispatch("tick");
          },
          value * 1e3
        );
      }
      old_value = value;
      old_active = active;
    }
  }
  return ``;
});

export { Index as default };
//# sourceMappingURL=Index11-CLPUD2u1.js.map
