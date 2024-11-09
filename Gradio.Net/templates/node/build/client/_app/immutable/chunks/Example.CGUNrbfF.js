import { SvelteComponent, init, safe_not_equal, element, HtmlTagHydration, claim_element, children, claim_html_tag, detach, attr, toggle_class, insert_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div;
  let html_tag;
  return {
    c() {
      div = element("div");
      html_tag = new HtmlTagHydration(false);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      html_tag = claim_html_tag(div_nodes, false);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      html_tag.a = null;
      attr(div, "class", "prose svelte-zvfedn");
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
      html_tag.m(
        /*value*/
        ctx[0],
        div
      );
    },
    p(ctx2, [dirty]) {
      if (dirty & /*value*/
      1)
        html_tag.p(
          /*value*/
          ctx2[0]
        );
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
