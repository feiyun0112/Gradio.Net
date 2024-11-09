import { SvelteComponent, init, safe_not_equal, element, text, claim_element, children, claim_text, detach, attr, toggle_class, insert_hydration, append_hydration, set_data, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let pre;
  let t_value = (
    /*value*/
    (ctx[0] ? (
      /*value*/
      ctx[0]
    ) : "") + ""
  );
  let t;
  return {
    c() {
      pre = element("pre");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      pre = claim_element(nodes, "PRE", { class: true });
      var pre_nodes = children(pre);
      t = claim_text(pre_nodes, t_value);
      pre_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(pre, "class", "svelte-agpzo2");
      toggle_class(
        pre,
        "table",
        /*type*/
        ctx[1] === "table"
      );
      toggle_class(
        pre,
        "gallery",
        /*type*/
        ctx[1] === "gallery"
      );
      toggle_class(
        pre,
        "selected",
        /*selected*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, pre, anchor);
      append_hydration(pre, t);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*value*/
      (ctx2[0] ? (
        /*value*/
        ctx2[0]
      ) : "") + ""))
        set_data(t, t_value);
      if (dirty & /*type*/
      2) {
        toggle_class(
          pre,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          pre,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (dirty & /*selected*/
      4) {
        toggle_class(
          pre,
          "selected",
          /*selected*/
          ctx2[2]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(pre);
      }
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
  };
  return [value, type, selected];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0, type: 1, selected: 2 });
  }
}
export {
  Example as default
};
