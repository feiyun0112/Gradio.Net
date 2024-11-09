import { SvelteComponent, init, safe_not_equal, element, create_component, claim_element, children, claim_component, detach, attr, toggle_class, insert_hydration, mount_component, transition_in, transition_out, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { M as MarkdownCode } from "./2.BqWhUxOo.js";
function create_fragment(ctx) {
  let div;
  let markdowncode;
  let current;
  markdowncode = new MarkdownCode({
    props: {
      message: (
        /*value*/
        ctx[0] ? (
          /*value*/
          ctx[0]
        ) : ""
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[5]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[3]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[4]
      ),
      chatbot: false,
      root: (
        /*root*/
        ctx[6]
      )
    }
  });
  return {
    c() {
      div = element("div");
      create_component(markdowncode.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(markdowncode.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "prose svelte-1ayixqk");
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
      mount_component(markdowncode, div, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      const markdowncode_changes = {};
      if (dirty & /*value*/
      1)
        markdowncode_changes.message = /*value*/
        ctx2[0] ? (
          /*value*/
          ctx2[0]
        ) : "";
      if (dirty & /*latex_delimiters*/
      32)
        markdowncode_changes.latex_delimiters = /*latex_delimiters*/
        ctx2[5];
      if (dirty & /*sanitize_html*/
      8)
        markdowncode_changes.sanitize_html = /*sanitize_html*/
        ctx2[3];
      if (dirty & /*line_breaks*/
      16)
        markdowncode_changes.line_breaks = /*line_breaks*/
        ctx2[4];
      if (dirty & /*root*/
      64)
        markdowncode_changes.root = /*root*/
        ctx2[6];
      markdowncode.$set(markdowncode_changes);
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (!current || dirty & /*selected*/
      4) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx2[2]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(markdowncode.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(markdowncode.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(markdowncode);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let { sanitize_html } = $$props;
  let { line_breaks } = $$props;
  let { latex_delimiters } = $$props;
  let { root } = $$props;
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
    if ("sanitize_html" in $$props2)
      $$invalidate(3, sanitize_html = $$props2.sanitize_html);
    if ("line_breaks" in $$props2)
      $$invalidate(4, line_breaks = $$props2.line_breaks);
    if ("latex_delimiters" in $$props2)
      $$invalidate(5, latex_delimiters = $$props2.latex_delimiters);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
  };
  return [value, type, selected, sanitize_html, line_breaks, latex_delimiters, root];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      type: 1,
      selected: 2,
      sanitize_html: 3,
      line_breaks: 4,
      latex_delimiters: 5,
      root: 6
    });
  }
}
export {
  Example as default
};
