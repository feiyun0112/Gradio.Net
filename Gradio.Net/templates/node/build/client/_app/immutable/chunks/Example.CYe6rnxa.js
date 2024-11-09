import { SvelteComponent, init, safe_not_equal, text, claim_text, insert_hydration, set_data, noop, detach } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let t_value = (
    /*value*/
    (ctx[0] || "") + ""
  );
  let t;
  return {
    c() {
      t = text(t_value);
    },
    l(nodes) {
      t = claim_text(nodes, t_value);
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*value*/
      (ctx2[0] || "") + ""))
        set_data(t, t_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
  };
  return [value];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0 });
  }
}
export {
  Example as default
};
