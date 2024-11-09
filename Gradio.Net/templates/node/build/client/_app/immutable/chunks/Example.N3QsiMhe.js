import { SvelteComponent, init, safe_not_equal, element, text, claim_element, children, claim_text, detach, attr, add_render_callback, toggle_class, insert_hydration, append_hydration, add_iframe_resize_listener, set_data, noop, onMount, binding_callbacks } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
function create_fragment(ctx) {
  let div;
  let t_value = (
    /*value*/
    (ctx[0] ? (
      /*value*/
      ctx[0]
    ) : "") + ""
  );
  let t;
  let div_resize_listener;
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
      attr(div, "class", "svelte-1viwdyg");
      add_render_callback(() => (
        /*div_elementresize_handler*/
        ctx[5].call(div)
      ));
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
      div_resize_listener = add_iframe_resize_listener(
        div,
        /*div_elementresize_handler*/
        ctx[5].bind(div)
      );
      ctx[6](div);
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
      div_resize_listener();
      ctx[6](null);
    }
  };
}
function set_styles(element2, el_width) {
  element2.style.setProperty("--local-text-width", `${el_width && el_width < 150 ? el_width : 200}px`);
  element2.style.whiteSpace = "unset";
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let size;
  let el;
  onMount(() => {
    set_styles(el, size);
  });
  function div_elementresize_handler() {
    size = this.clientWidth;
    $$invalidate(3, size);
  }
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      el = $$value;
      $$invalidate(4, el);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
  };
  return [value, type, selected, size, el, div_elementresize_handler, div_binding];
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
