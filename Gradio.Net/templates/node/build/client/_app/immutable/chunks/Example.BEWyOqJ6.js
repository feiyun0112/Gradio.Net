import { SvelteComponent, init, safe_not_equal, element, text, claim_element, children, claim_text, detach, attr, toggle_class, insert_hydration, append_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div;
  let t;
  return {
    c() {
      div = element("div");
      t = text(
        /*names_string*/
        ctx[2]
      );
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      t = claim_text(
        div_nodes,
        /*names_string*/
        ctx[2]
      );
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "svelte-1ayixqk");
      toggle_class(
        div,
        "table",
        /*type*/
        ctx[0] === "table"
      );
      toggle_class(
        div,
        "gallery",
        /*type*/
        ctx[0] === "gallery"
      );
      toggle_class(
        div,
        "selected",
        /*selected*/
        ctx[1]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*type*/
      1) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[0] === "table"
        );
      }
      if (dirty & /*type*/
      1) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[0] === "gallery"
        );
      }
      if (dirty & /*selected*/
      2) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx2[1]
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
  let { choices } = $$props;
  let value_array = value ? Array.isArray(value) ? value : [value] : [];
  let names = value_array.map((val) => {
    var _a;
    return (_a = choices.find((pair) => pair[1] === val)) == null ? void 0 : _a[0];
  }).filter((name) => name !== void 0);
  let names_string = names.join(", ");
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(0, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(1, selected = $$props2.selected);
    if ("choices" in $$props2)
      $$invalidate(4, choices = $$props2.choices);
  };
  return [type, selected, names_string, value, choices];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 3,
      type: 0,
      selected: 1,
      choices: 4
    });
  }
}
export {
  Example as default
};
