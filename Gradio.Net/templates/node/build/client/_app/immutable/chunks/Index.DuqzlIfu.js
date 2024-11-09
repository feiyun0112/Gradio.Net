import { SvelteComponent, init, safe_not_equal, onDestroy } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function instance($$self, $$props, $$invalidate) {
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
  $$self.$$set = ($$props2) => {
    if ("gradio" in $$props2)
      $$invalidate(0, gradio = $$props2.gradio);
    if ("value" in $$props2)
      $$invalidate(1, value = $$props2.value);
    if ("active" in $$props2)
      $$invalidate(2, active = $$props2.active);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*old_value, value, active, old_active, interval, gradio*/
    63) {
      if (old_value !== value || active !== old_active) {
        if (interval)
          clearInterval(interval);
        if (active) {
          $$invalidate(5, interval = setInterval(
            () => {
              if (document.visibilityState === "visible")
                gradio.dispatch("tick");
            },
            value * 1e3
          ));
        }
        $$invalidate(3, old_value = value);
        $$invalidate(4, old_active = active);
      }
    }
  };
  return [gradio, value, active, old_value, old_active, interval];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, null, safe_not_equal, { gradio: 0, value: 1, active: 2 });
  }
}
export {
  Index as default
};
