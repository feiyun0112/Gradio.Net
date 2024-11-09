import { SvelteComponent, init, safe_not_equal, element, text, claim_element, children, claim_text, detach, attr, toggle_class, insert_hydration, append_hydration, set_data, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div;
  let t_value = (
    /*value*/
    (ctx[0] !== null ? (
      /*value*/
      ctx[0]
    ) : "") + ""
  );
  let t;
  return {
    c() {
      div = element("div");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      t = claim_text(div_nodes, t_value);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "svelte-1ayixqk");
      toggle_class(
        div,
        "table",
        /*type*/
        ctx[1] === "table"
      );
      toggle_class(
        div,
        "gallery",
        /*type*/
        ctx[1] === "gallery"
      );
      toggle_class(
        div,
        "selected",
        /*selected*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*value*/
      (ctx2[0] !== null ? (
        /*value*/
        ctx2[0]
      ) : "") + ""))
        set_data(t, t_value);
      if (dirty & /*type*/
      2) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (dirty & /*selected*/
      4) {
        toggle_class(
          div,
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
        detach(div);
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
