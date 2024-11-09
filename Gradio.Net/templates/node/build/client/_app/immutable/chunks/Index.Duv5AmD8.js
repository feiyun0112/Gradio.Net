import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, element, space, claim_element, claim_space, toggle_class, set_style, listen, transition_in, group_outros, transition_out, check_outros, createEventDispatcher, onMount, afterUpdate, text, claim_text, get_svelte_dataset, set_data, ensure_array_like, destroy_each, create_component, claim_component, mount_component, destroy_component, tick, bubble, binding_callbacks, empty, onDestroy, assign, add_render_callback, add_iframe_resize_listener, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { I as IconButton, q as Check, r as Copy, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
function create_fragment$3(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        "xmlns:xlink": true,
        "aria-hidden": true,
        role: true,
        class: true,
        width: true,
        height: true,
        preserveAspectRatio: true,
        viewBox: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "fill", "currentColor");
      attr(path, "d", "M5 3h2v2H5v5a2 2 0 0 1-2 2a2 2 0 0 1 2 2v5h2v2H5c-1.07-.27-2-.9-2-2v-4a2 2 0 0 0-2-2H0v-2h1a2 2 0 0 0 2-2V5a2 2 0 0 1 2-2m14 0a2 2 0 0 1 2 2v4a2 2 0 0 0 2 2h1v2h-1a2 2 0 0 0-2 2v4a2 2 0 0 1-2 2h-2v-2h2v-5a2 2 0 0 1 2-2a2 2 0 0 1-2-2V5h-2V3h2m-7 12a1 1 0 0 1 1 1a1 1 0 0 1-1 1a1 1 0 0 1-1-1a1 1 0 0 1 1-1m-4 0a1 1 0 0 1 1 1a1 1 0 0 1-1 1a1 1 0 0 1-1-1a1 1 0 0 1 1-1m8 0a1 1 0 0 1 1 1a1 1 0 0 1-1 1a1 1 0 0 1-1-1a1 1 0 0 1 1-1Z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "xmlns:xlink", "http://www.w3.org/1999/xlink");
      attr(svg, "aria-hidden", "true");
      attr(svg, "role", "img");
      attr(svg, "class", "iconify iconify--mdi");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "preserveAspectRatio", "xMidYMid meet");
      attr(svg, "viewBox", "0 0 24 24");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
let JSON$2 = class JSON2 extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$3, safe_not_equal, {});
  }
};
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[17] = list[i][0];
  child_ctx[18] = list[i][1];
  child_ctx[20] = i;
  return child_ctx;
}
function create_if_block_10(ctx) {
  let button;
  let button_data_pseudo_content_value;
  let button_aria_label_value;
  let mounted;
  let dispose;
  return {
    c() {
      button = element("button");
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", {
        "data-pseudo-content": true,
        "aria-label": true,
        class: true
      });
      children(button).forEach(detach);
      this.h();
    },
    h() {
      attr(button, "data-pseudo-content", button_data_pseudo_content_value = /*collapsed*/
      ctx[9] ? "▶" : "▼");
      attr(button, "aria-label", button_aria_label_value = /*collapsed*/
      ctx[9] ? "Expand" : "Collapse");
      attr(button, "class", "toggle svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*toggle_collapse*/
          ctx[11]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*collapsed*/
      512 && button_data_pseudo_content_value !== (button_data_pseudo_content_value = /*collapsed*/
      ctx2[9] ? "▶" : "▼")) {
        attr(button, "data-pseudo-content", button_data_pseudo_content_value);
      }
      if (dirty & /*collapsed*/
      512 && button_aria_label_value !== (button_aria_label_value = /*collapsed*/
      ctx2[9] ? "Expand" : "Collapse")) {
        attr(button, "aria-label", button_aria_label_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_9(ctx) {
  let span0;
  let t0;
  let t1;
  let t2;
  let span1;
  let textContent = ":";
  return {
    c() {
      span0 = element("span");
      t0 = text('"');
      t1 = text(
        /*key*/
        ctx[4]
      );
      t2 = text('"');
      span1 = element("span");
      span1.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span0 = claim_element(nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, '"');
      t1 = claim_text(
        span0_nodes,
        /*key*/
        ctx[4]
      );
      t2 = claim_text(span0_nodes, '"');
      span0_nodes.forEach(detach);
      span1 = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span1) !== "svelte-1cahzs5")
        span1.textContent = textContent;
      this.h();
    },
    h() {
      attr(span0, "class", "key svelte-19ir0ev");
      attr(span1, "class", "punctuation colon svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span0, anchor);
      append_hydration(span0, t0);
      append_hydration(span0, t1);
      append_hydration(span0, t2);
      insert_hydration(target, span1, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*key*/
      16)
        set_data(
          t1,
          /*key*/
          ctx2[4]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(span0);
        detach(span1);
      }
    }
  };
}
function create_else_block$1(ctx) {
  let span;
  let t;
  return {
    c() {
      span = element("span");
      t = text(
        /*value*/
        ctx[0]
      );
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", {});
      var span_nodes = children(span);
      t = claim_text(
        span_nodes,
        /*value*/
        ctx[0]
      );
      span_nodes.forEach(detach);
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1)
        set_data(
          t,
          /*value*/
          ctx2[0]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_8(ctx) {
  let span;
  let textContent = "null";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-xcjkvs")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "value null svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_7(ctx) {
  let span;
  let t_value = (
    /*value*/
    ctx[0].toString() + ""
  );
  let t;
  return {
    c() {
      span = element("span");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t = claim_text(span_nodes, t_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "value bool svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*value*/
      ctx2[0].toString() + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_6(ctx) {
  let span;
  let t;
  return {
    c() {
      span = element("span");
      t = text(
        /*value*/
        ctx[0]
      );
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t = claim_text(
        span_nodes,
        /*value*/
        ctx[0]
      );
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "value number svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1)
        set_data(
          t,
          /*value*/
          ctx2[0]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_5(ctx) {
  let span;
  let t0;
  let t1;
  let t2;
  return {
    c() {
      span = element("span");
      t0 = text('"');
      t1 = text(
        /*value*/
        ctx[0]
      );
      t2 = text('"');
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, '"');
      t1 = claim_text(
        span_nodes,
        /*value*/
        ctx[0]
      );
      t2 = claim_text(span_nodes, '"');
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "value string svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t0);
      append_hydration(span, t1);
      append_hydration(span, t2);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1)
        set_data(
          t1,
          /*value*/
          ctx2[0]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_3(ctx) {
  let span;
  let t0_value = Array.isArray(
    /*value*/
    ctx[0]
  ) ? "[" : "{";
  let t0;
  let t1;
  let if_block_anchor;
  let if_block = (
    /*collapsed*/
    ctx[9] && create_if_block_4(ctx)
  );
  return {
    c() {
      span = element("span");
      t0 = text(t0_value);
      t1 = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, t0_value);
      span_nodes.forEach(detach);
      t1 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(span, "class", "punctuation bracket svelte-19ir0ev");
      toggle_class(span, "square-bracket", Array.isArray(
        /*value*/
        ctx[0]
      ));
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t0);
      insert_hydration(target, t1, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t0_value !== (t0_value = Array.isArray(
        /*value*/
        ctx2[0]
      ) ? "[" : "{"))
        set_data(t0, t0_value);
      if (dirty & /*Array, value*/
      1) {
        toggle_class(span, "square-bracket", Array.isArray(
          /*value*/
          ctx2[0]
        ));
      }
      if (
        /*collapsed*/
        ctx2[9]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_4(ctx2);
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(span);
        detach(t1);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block_4(ctx) {
  let button;
  let t0_value = get_collapsed_preview(
    /*value*/
    ctx[0]
  ) + "";
  let t0;
  let t1;
  let span;
  let t2_value = Array.isArray(
    /*value*/
    ctx[0]
  ) ? "]" : "}";
  let t2;
  let mounted;
  let dispose;
  return {
    c() {
      button = element("button");
      t0 = text(t0_value);
      t1 = space();
      span = element("span");
      t2 = text(t2_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      t0 = claim_text(button_nodes, t0_value);
      button_nodes.forEach(detach);
      t1 = claim_space(nodes);
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t2 = claim_text(span_nodes, t2_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "preview svelte-19ir0ev");
      attr(span, "class", "punctuation bracket svelte-19ir0ev");
      toggle_class(span, "square-bracket", Array.isArray(
        /*value*/
        ctx[0]
      ));
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t0);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, span, anchor);
      append_hydration(span, t2);
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*toggle_collapse*/
          ctx[11]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t0_value !== (t0_value = get_collapsed_preview(
        /*value*/
        ctx2[0]
      ) + ""))
        set_data(t0, t0_value);
      if (dirty & /*value*/
      1 && t2_value !== (t2_value = Array.isArray(
        /*value*/
        ctx2[0]
      ) ? "]" : "}"))
        set_data(t2, t2_value);
      if (dirty & /*Array, value*/
      1) {
        toggle_class(span, "square-bracket", Array.isArray(
          /*value*/
          ctx2[0]
        ));
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
        detach(t1);
        detach(span);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_2(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-19nlgjl")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "punctuation svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block$2(ctx) {
  let div1;
  let t0;
  let div0;
  let span0;
  let t1;
  let span2;
  let span1;
  let t2_value = Array.isArray(
    /*value*/
    ctx[0]
  ) ? "]" : "}";
  let t2;
  let t3;
  let current;
  let each_value = ensure_array_like(
    /*child_nodes*/
    ctx[10]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  let if_block = !/*is_last_item*/
  ctx[3] && create_if_block_1();
  return {
    c() {
      div1 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t0 = space();
      div0 = element("div");
      span0 = element("span");
      t1 = space();
      span2 = element("span");
      span1 = element("span");
      t2 = text(t2_value);
      t3 = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div1_nodes);
      }
      t0 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      span0 = claim_element(div0_nodes, "SPAN", { class: true });
      children(span0).forEach(detach);
      t1 = claim_space(div0_nodes);
      span2 = claim_element(div0_nodes, "SPAN", { class: true });
      var span2_nodes = children(span2);
      span1 = claim_element(span2_nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      t2 = claim_text(span1_nodes, t2_value);
      span1_nodes.forEach(detach);
      t3 = claim_space(span2_nodes);
      if (if_block)
        if_block.l(span2_nodes);
      span2_nodes.forEach(detach);
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "line-number svelte-19ir0ev");
      attr(span1, "class", "punctuation bracket svelte-19ir0ev");
      toggle_class(span1, "square-bracket", Array.isArray(
        /*value*/
        ctx[0]
      ));
      attr(span2, "class", "content svelte-19ir0ev");
      attr(div0, "class", "line svelte-19ir0ev");
      attr(div1, "class", "children svelte-19ir0ev");
      toggle_class(
        div1,
        "hidden",
        /*collapsed*/
        ctx[9]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div1, null);
        }
      }
      append_hydration(div1, t0);
      append_hydration(div1, div0);
      append_hydration(div0, span0);
      append_hydration(div0, t1);
      append_hydration(div0, span2);
      append_hydration(span2, span1);
      append_hydration(span1, t2);
      append_hydration(span2, t3);
      if (if_block)
        if_block.m(span2, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*child_nodes, depth, open, theme_mode, show_indices*/
      1250) {
        each_value = ensure_array_like(
          /*child_nodes*/
          ctx2[10]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div1, t0);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if ((!current || dirty & /*value*/
      1) && t2_value !== (t2_value = Array.isArray(
        /*value*/
        ctx2[0]
      ) ? "]" : "}"))
        set_data(t2, t2_value);
      if (!current || dirty & /*Array, value*/
      1) {
        toggle_class(span1, "square-bracket", Array.isArray(
          /*value*/
          ctx2[0]
        ));
      }
      if (!/*is_last_item*/
      ctx2[3]) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_1();
          if_block.c();
          if_block.m(span2, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (!current || dirty & /*collapsed*/
      512) {
        toggle_class(
          div1,
          "hidden",
          /*collapsed*/
          ctx2[9]
        );
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d();
    }
  };
}
function create_each_block(ctx) {
  let jsonnode;
  let current;
  jsonnode = new JSONNode({
    props: {
      value: (
        /*subVal*/
        ctx[18]
      ),
      depth: (
        /*depth*/
        ctx[1] + 1
      ),
      is_last_item: (
        /*i*/
        ctx[20] === /*child_nodes*/
        ctx[10].length - 1
      ),
      key: (
        /*subKey*/
        ctx[17]
      ),
      open: (
        /*open*/
        ctx[5]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[6]
      ),
      show_indices: (
        /*show_indices*/
        ctx[7]
      )
    }
  });
  jsonnode.$on(
    "toggle",
    /*toggle_handler_1*/
    ctx[13]
  );
  return {
    c() {
      create_component(jsonnode.$$.fragment);
    },
    l(nodes) {
      claim_component(jsonnode.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(jsonnode, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const jsonnode_changes = {};
      if (dirty & /*child_nodes*/
      1024)
        jsonnode_changes.value = /*subVal*/
        ctx2[18];
      if (dirty & /*depth*/
      2)
        jsonnode_changes.depth = /*depth*/
        ctx2[1] + 1;
      if (dirty & /*child_nodes*/
      1024)
        jsonnode_changes.is_last_item = /*i*/
        ctx2[20] === /*child_nodes*/
        ctx2[10].length - 1;
      if (dirty & /*child_nodes*/
      1024)
        jsonnode_changes.key = /*subKey*/
        ctx2[17];
      if (dirty & /*open*/
      32)
        jsonnode_changes.open = /*open*/
        ctx2[5];
      if (dirty & /*theme_mode*/
      64)
        jsonnode_changes.theme_mode = /*theme_mode*/
        ctx2[6];
      if (dirty & /*show_indices*/
      128)
        jsonnode_changes.show_indices = /*show_indices*/
        ctx2[7];
      jsonnode.$set(jsonnode_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(jsonnode.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(jsonnode.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(jsonnode, detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-19nlgjl")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "punctuation svelte-19ir0ev");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_fragment$2(ctx) {
  let div1;
  let div0;
  let span0;
  let t0;
  let span1;
  let show_if_3 = is_collapsible(
    /*value*/
    ctx[0]
  );
  let t1;
  let t2;
  let show_if_2;
  let t3;
  let show_if_1 = !/*is_last_item*/
  ctx[3] && (!is_collapsible(
    /*value*/
    ctx[0]
  ) || /*collapsed*/
  ctx[9]);
  let t4;
  let show_if = is_collapsible(
    /*value*/
    ctx[0]
  );
  let current;
  let mounted;
  let dispose;
  let if_block0 = show_if_3 && create_if_block_10(ctx);
  let if_block1 = (
    /*key*/
    ctx[4] !== null && create_if_block_9(ctx)
  );
  function select_block_type(ctx2, dirty) {
    if (dirty & /*value*/
    1)
      show_if_2 = null;
    if (show_if_2 == null)
      show_if_2 = !!is_collapsible(
        /*value*/
        ctx2[0]
      );
    if (show_if_2)
      return create_if_block_3;
    if (typeof /*value*/
    ctx2[0] === "string")
      return create_if_block_5;
    if (typeof /*value*/
    ctx2[0] === "number")
      return create_if_block_6;
    if (typeof /*value*/
    ctx2[0] === "boolean")
      return create_if_block_7;
    if (
      /*value*/
      ctx2[0] === null
    )
      return create_if_block_8;
    return create_else_block$1;
  }
  let current_block_type = select_block_type(ctx, -1);
  let if_block2 = current_block_type(ctx);
  let if_block3 = show_if_1 && create_if_block_2();
  let if_block4 = show_if && create_if_block$2(ctx);
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      span0 = element("span");
      t0 = space();
      span1 = element("span");
      if (if_block0)
        if_block0.c();
      t1 = space();
      if (if_block1)
        if_block1.c();
      t2 = space();
      if_block2.c();
      t3 = space();
      if (if_block3)
        if_block3.c();
      t4 = space();
      if (if_block4)
        if_block4.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true, style: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      span0 = claim_element(div0_nodes, "SPAN", { class: true });
      children(span0).forEach(detach);
      t0 = claim_space(div0_nodes);
      span1 = claim_element(div0_nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      if (if_block0)
        if_block0.l(span1_nodes);
      t1 = claim_space(span1_nodes);
      if (if_block1)
        if_block1.l(span1_nodes);
      t2 = claim_space(span1_nodes);
      if_block2.l(span1_nodes);
      t3 = claim_space(span1_nodes);
      if (if_block3)
        if_block3.l(span1_nodes);
      span1_nodes.forEach(detach);
      div0_nodes.forEach(detach);
      t4 = claim_space(div1_nodes);
      if (if_block4)
        if_block4.l(div1_nodes);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "line-number svelte-19ir0ev");
      attr(span1, "class", "content svelte-19ir0ev");
      attr(div0, "class", "line svelte-19ir0ev");
      toggle_class(
        div0,
        "collapsed",
        /*collapsed*/
        ctx[9]
      );
      attr(div1, "class", "json-node svelte-19ir0ev");
      set_style(
        div1,
        "--depth",
        /*depth*/
        ctx[1]
      );
      toggle_class(
        div1,
        "root",
        /*is_root*/
        ctx[2]
      );
      toggle_class(
        div1,
        "dark-mode",
        /*theme_mode*/
        ctx[6] === "dark"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      append_hydration(div0, span0);
      append_hydration(div0, t0);
      append_hydration(div0, span1);
      if (if_block0)
        if_block0.m(span1, null);
      append_hydration(span1, t1);
      if (if_block1)
        if_block1.m(span1, null);
      append_hydration(span1, t2);
      if_block2.m(span1, null);
      append_hydration(span1, t3);
      if (if_block3)
        if_block3.m(span1, null);
      append_hydration(div1, t4);
      if (if_block4)
        if_block4.m(div1, null);
      ctx[14](div1);
      current = true;
      if (!mounted) {
        dispose = listen(
          div1,
          "toggle",
          /*toggle_handler*/
          ctx[12]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*value*/
      1)
        show_if_3 = is_collapsible(
          /*value*/
          ctx2[0]
        );
      if (show_if_3) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_10(ctx2);
          if_block0.c();
          if_block0.m(span1, t1);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (
        /*key*/
        ctx2[4] !== null
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
        } else {
          if_block1 = create_if_block_9(ctx2);
          if_block1.c();
          if_block1.m(span1, t2);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
      if (current_block_type === (current_block_type = select_block_type(ctx2, dirty)) && if_block2) {
        if_block2.p(ctx2, dirty);
      } else {
        if_block2.d(1);
        if_block2 = current_block_type(ctx2);
        if (if_block2) {
          if_block2.c();
          if_block2.m(span1, t3);
        }
      }
      if (dirty & /*is_last_item, value, collapsed*/
      521)
        show_if_1 = !/*is_last_item*/
        ctx2[3] && (!is_collapsible(
          /*value*/
          ctx2[0]
        ) || /*collapsed*/
        ctx2[9]);
      if (show_if_1) {
        if (if_block3)
          ;
        else {
          if_block3 = create_if_block_2();
          if_block3.c();
          if_block3.m(span1, null);
        }
      } else if (if_block3) {
        if_block3.d(1);
        if_block3 = null;
      }
      if (!current || dirty & /*collapsed*/
      512) {
        toggle_class(
          div0,
          "collapsed",
          /*collapsed*/
          ctx2[9]
        );
      }
      if (dirty & /*value*/
      1)
        show_if = is_collapsible(
          /*value*/
          ctx2[0]
        );
      if (show_if) {
        if (if_block4) {
          if_block4.p(ctx2, dirty);
          if (dirty & /*value*/
          1) {
            transition_in(if_block4, 1);
          }
        } else {
          if_block4 = create_if_block$2(ctx2);
          if_block4.c();
          transition_in(if_block4, 1);
          if_block4.m(div1, null);
        }
      } else if (if_block4) {
        group_outros();
        transition_out(if_block4, 1, 1, () => {
          if_block4 = null;
        });
        check_outros();
      }
      if (!current || dirty & /*depth*/
      2) {
        set_style(
          div1,
          "--depth",
          /*depth*/
          ctx2[1]
        );
      }
      if (!current || dirty & /*is_root*/
      4) {
        toggle_class(
          div1,
          "root",
          /*is_root*/
          ctx2[2]
        );
      }
      if (!current || dirty & /*theme_mode*/
      64) {
        toggle_class(
          div1,
          "dark-mode",
          /*theme_mode*/
          ctx2[6] === "dark"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block4);
      current = true;
    },
    o(local) {
      transition_out(if_block4);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      if_block2.d();
      if (if_block3)
        if_block3.d();
      if (if_block4)
        if_block4.d();
      ctx[14](null);
      mounted = false;
      dispose();
    }
  };
}
function is_collapsible(val) {
  return val !== null && (typeof val === "object" || Array.isArray(val));
}
function get_collapsed_preview(val) {
  if (Array.isArray(val))
    return `Array(${val.length})`;
  if (typeof val === "object" && val !== null)
    return `Object(${Object.keys(val).length})`;
  return String(val);
}
function instance$2($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { depth = 0 } = $$props;
  let { is_root = false } = $$props;
  let { is_last_item = true } = $$props;
  let { key = null } = $$props;
  let { open = false } = $$props;
  let { theme_mode = "system" } = $$props;
  let { show_indices = false } = $$props;
  const dispatch = createEventDispatcher();
  let root_element;
  let collapsed = open ? false : depth >= 3;
  let child_nodes = [];
  async function toggle_collapse() {
    $$invalidate(9, collapsed = !collapsed);
    await tick();
    dispatch("toggle", { collapsed, depth });
  }
  function updateLineNumbers() {
    const lines = root_element.querySelectorAll(".line");
    lines.forEach((line, index) => {
      const line_number = line.querySelector(".line-number");
      if (line_number) {
        line_number.setAttribute("data-pseudo-content", (index + 1).toString());
        line_number == null ? void 0 : line_number.setAttribute("aria-roledescription", `Line number ${index + 1}`);
        line_number == null ? void 0 : line_number.setAttribute("title", `Line number ${index + 1}`);
      }
    });
  }
  onMount(() => {
    if (is_root) {
      updateLineNumbers();
    }
  });
  afterUpdate(() => {
    if (is_root) {
      updateLineNumbers();
    }
  });
  function toggle_handler(event) {
    bubble.call(this, $$self, event);
  }
  function toggle_handler_1(event) {
    bubble.call(this, $$self, event);
  }
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      root_element = $$value;
      $$invalidate(8, root_element);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("depth" in $$props2)
      $$invalidate(1, depth = $$props2.depth);
    if ("is_root" in $$props2)
      $$invalidate(2, is_root = $$props2.is_root);
    if ("is_last_item" in $$props2)
      $$invalidate(3, is_last_item = $$props2.is_last_item);
    if ("key" in $$props2)
      $$invalidate(4, key = $$props2.key);
    if ("open" in $$props2)
      $$invalidate(5, open = $$props2.open);
    if ("theme_mode" in $$props2)
      $$invalidate(6, theme_mode = $$props2.theme_mode);
    if ("show_indices" in $$props2)
      $$invalidate(7, show_indices = $$props2.show_indices);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    1) {
      if (is_collapsible(value)) {
        $$invalidate(10, child_nodes = Object.entries(value));
      } else {
        $$invalidate(10, child_nodes = []);
      }
    }
    if ($$self.$$.dirty & /*is_root, root_element*/
    260) {
      if (is_root && root_element) {
        updateLineNumbers();
      }
    }
  };
  return [
    value,
    depth,
    is_root,
    is_last_item,
    key,
    open,
    theme_mode,
    show_indices,
    root_element,
    collapsed,
    child_nodes,
    toggle_collapse,
    toggle_handler,
    toggle_handler_1,
    div1_binding
  ];
}
class JSONNode extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      value: 0,
      depth: 1,
      is_root: 2,
      is_last_item: 3,
      key: 4,
      open: 5,
      theme_mode: 6,
      show_indices: 7
    });
  }
}
function create_else_block(ctx) {
  let div;
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      div = element("div");
      create_component(empty_1.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(empty_1.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "empty-wrapper svelte-ryarus");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(empty_1, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const empty_1_changes = {};
      if (dirty & /*$$scope*/
      2048) {
        empty_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      empty_1.$set(empty_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(empty_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(empty_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(empty_1);
    }
  };
}
function create_if_block$1(ctx) {
  let iconbuttonwrapper;
  let t;
  let div;
  let jsonnode;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  jsonnode = new JSONNode({
    props: {
      value: (
        /*value*/
        ctx[0]
      ),
      depth: 0,
      is_root: true,
      open: (
        /*open*/
        ctx[1]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[2]
      ),
      show_indices: (
        /*show_indices*/
        ctx[3]
      )
    }
  });
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
      t = space();
      div = element("div");
      create_component(jsonnode.$$.fragment);
      this.h();
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(jsonnode.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "json-holder svelte-ryarus");
      set_style(
        div,
        "max-height",
        /*json_max_height*/
        ctx[5]
      );
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      mount_component(jsonnode, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, copied*/
      2064) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      const jsonnode_changes = {};
      if (dirty & /*value*/
      1)
        jsonnode_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*open*/
      2)
        jsonnode_changes.open = /*open*/
        ctx2[1];
      if (dirty & /*theme_mode*/
      4)
        jsonnode_changes.theme_mode = /*theme_mode*/
        ctx2[2];
      if (dirty & /*show_indices*/
      8)
        jsonnode_changes.show_indices = /*show_indices*/
        ctx2[3];
      jsonnode.$set(jsonnode_changes);
      if (dirty & /*json_max_height*/
      32) {
        set_style(
          div,
          "max-height",
          /*json_max_height*/
          ctx2[5]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(jsonnode.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(jsonnode.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(div);
      }
      destroy_component(iconbuttonwrapper, detaching);
      destroy_component(jsonnode);
    }
  };
}
function create_default_slot_1(ctx) {
  let jsonicon;
  let current;
  jsonicon = new JSON$2({});
  return {
    c() {
      create_component(jsonicon.$$.fragment);
    },
    l(nodes) {
      claim_component(jsonicon.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(jsonicon, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(jsonicon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(jsonicon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(jsonicon, detaching);
    }
  };
}
function create_default_slot$1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      show_label: false,
      label: (
        /*copied*/
        ctx[4] ? "Copied" : "Copy"
      ),
      Icon: (
        /*copied*/
        ctx[4] ? Check : Copy
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[8]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*copied*/
      16)
        iconbutton_changes.label = /*copied*/
        ctx2[4] ? "Copied" : "Copy";
      if (dirty & /*copied*/
      16)
        iconbutton_changes.Icon = /*copied*/
        ctx2[4] ? Check : Copy;
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let show_if;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block$1, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (dirty & /*value*/
    1)
      show_if = null;
    if (show_if == null)
      show_if = !!/*value*/
      (ctx2[0] && /*value*/
      ctx2[0] !== '""' && !is_empty(
        /*value*/
        ctx2[0]
      ));
    if (show_if)
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx, -1);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2, dirty);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block.c();
        } else {
          if_block.p(ctx2, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(if_block_anchor.parentNode, if_block_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function is_empty(obj) {
  return obj && Object.keys(obj).length === 0 && Object.getPrototypeOf(obj) === Object.prototype && JSON.stringify(obj) === JSON.stringify({});
}
function instance$1($$self, $$props, $$invalidate) {
  let json_max_height;
  let { value = {} } = $$props;
  let { open = false } = $$props;
  let { theme_mode = "system" } = $$props;
  let { show_indices = false } = $$props;
  let { label_height } = $$props;
  let copied = false;
  let timer;
  function copy_feedback() {
    $$invalidate(4, copied = true);
    if (timer)
      clearTimeout(timer);
    timer = setTimeout(
      () => {
        $$invalidate(4, copied = false);
      },
      1e3
    );
  }
  async function handle_copy() {
    if ("clipboard" in navigator) {
      await navigator.clipboard.writeText(JSON.stringify(value, null, 2));
      copy_feedback();
    }
  }
  onDestroy(() => {
    if (timer)
      clearTimeout(timer);
  });
  const click_handler = () => handle_copy();
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("open" in $$props2)
      $$invalidate(1, open = $$props2.open);
    if ("theme_mode" in $$props2)
      $$invalidate(2, theme_mode = $$props2.theme_mode);
    if ("show_indices" in $$props2)
      $$invalidate(3, show_indices = $$props2.show_indices);
    if ("label_height" in $$props2)
      $$invalidate(7, label_height = $$props2.label_height);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*label_height*/
    128) {
      $$invalidate(5, json_max_height = `calc(100% - ${label_height}px)`);
    }
  };
  return [
    value,
    open,
    theme_mode,
    show_indices,
    copied,
    json_max_height,
    handle_copy,
    label_height,
    click_handler
  ];
}
class JSON_1 extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      value: 0,
      open: 1,
      theme_mode: 2,
      show_indices: 3,
      label_height: 7
    });
  }
}
const JSON$1 = JSON_1;
function create_if_block(ctx) {
  let blocklabel;
  let current;
  blocklabel = new BlockLabel({
    props: {
      Icon: JSON$2,
      show_label: (
        /*show_label*/
        ctx[6]
      ),
      label: (
        /*label*/
        ctx[5]
      ),
      float: false,
      disable: (
        /*container*/
        ctx[7] === false
      )
    }
  });
  return {
    c() {
      create_component(blocklabel.$$.fragment);
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      64)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[6];
      if (dirty & /*label*/
      32)
        blocklabel_changes.label = /*label*/
        ctx2[5];
      if (dirty & /*container*/
      128)
        blocklabel_changes.disable = /*container*/
        ctx2[7] === false;
      blocklabel.$set(blocklabel_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(blocklabel, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let div;
  let div_resize_listener;
  let t0;
  let statustracker;
  let t1;
  let json;
  let current;
  let if_block = (
    /*label*/
    ctx[5] && create_if_block(ctx)
  );
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[10].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[10].i18n
    ) },
    /*loading_status*/
    ctx[4]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[20]
  );
  json = new JSON$1({
    props: {
      value: (
        /*value*/
        ctx[3]
      ),
      open: (
        /*open*/
        ctx[11]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[12]
      ),
      show_indices: (
        /*show_indices*/
        ctx[13]
      ),
      label_height: (
        /*label_height*/
        ctx[17]
      )
    }
  });
  return {
    c() {
      div = element("div");
      if (if_block)
        if_block.c();
      t0 = space();
      create_component(statustracker.$$.fragment);
      t1 = space();
      create_component(json.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {});
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      div_nodes.forEach(detach);
      t0 = claim_space(nodes);
      claim_component(statustracker.$$.fragment, nodes);
      t1 = claim_space(nodes);
      claim_component(json.$$.fragment, nodes);
      this.h();
    },
    h() {
      add_render_callback(() => (
        /*div_elementresize_handler*/
        ctx[19].call(div)
      ));
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (if_block)
        if_block.m(div, null);
      div_resize_listener = add_iframe_resize_listener(
        div,
        /*div_elementresize_handler*/
        ctx[19].bind(div)
      );
      insert_hydration(target, t0, anchor);
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t1, anchor);
      mount_component(json, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*label*/
        ctx2[5]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*label*/
          32) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(div, null);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const statustracker_changes = dirty & /*gradio, loading_status*/
      1040 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        1024 && {
          autoscroll: (
            /*gradio*/
            ctx2[10].autoscroll
          )
        },
        dirty & /*gradio*/
        1024 && { i18n: (
          /*gradio*/
          ctx2[10].i18n
        ) },
        dirty & /*loading_status*/
        16 && get_spread_object(
          /*loading_status*/
          ctx2[4]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const json_changes = {};
      if (dirty & /*value*/
      8)
        json_changes.value = /*value*/
        ctx2[3];
      if (dirty & /*open*/
      2048)
        json_changes.open = /*open*/
        ctx2[11];
      if (dirty & /*theme_mode*/
      4096)
        json_changes.theme_mode = /*theme_mode*/
        ctx2[12];
      if (dirty & /*show_indices*/
      8192)
        json_changes.show_indices = /*show_indices*/
        ctx2[13];
      if (dirty & /*label_height*/
      131072)
        json_changes.label_height = /*label_height*/
        ctx2[17];
      json.$set(json_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(statustracker.$$.fragment, local);
      transition_in(json.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(statustracker.$$.fragment, local);
      transition_out(json.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t0);
        detach(t1);
      }
      if (if_block)
        if_block.d();
      div_resize_listener();
      destroy_component(statustracker, detaching);
      destroy_component(json, detaching);
    }
  };
}
function create_fragment(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[2]
      ),
      test_id: "json",
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      container: (
        /*container*/
        ctx[7]
      ),
      scale: (
        /*scale*/
        ctx[8]
      ),
      min_width: (
        /*min_width*/
        ctx[9]
      ),
      padding: false,
      allow_overflow: true,
      overflow_behavior: "auto",
      height: (
        /*height*/
        ctx[14]
      ),
      min_height: (
        /*min_height*/
        ctx[15]
      ),
      max_height: (
        /*max_height*/
        ctx[16]
      ),
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(block.$$.fragment);
    },
    l(nodes) {
      claim_component(block.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(block, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const block_changes = {};
      if (dirty & /*visible*/
      4)
        block_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*elem_id*/
      1)
        block_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*container*/
      128)
        block_changes.container = /*container*/
        ctx2[7];
      if (dirty & /*scale*/
      256)
        block_changes.scale = /*scale*/
        ctx2[8];
      if (dirty & /*min_width*/
      512)
        block_changes.min_width = /*min_width*/
        ctx2[9];
      if (dirty & /*height*/
      16384)
        block_changes.height = /*height*/
        ctx2[14];
      if (dirty & /*min_height*/
      32768)
        block_changes.min_height = /*min_height*/
        ctx2[15];
      if (dirty & /*max_height*/
      65536)
        block_changes.max_height = /*max_height*/
        ctx2[16];
      if (dirty & /*$$scope, value, open, theme_mode, show_indices, label_height, gradio, loading_status, show_label, label, container*/
      2243832) {
        block_changes.$$scope = { dirty, ctx: ctx2 };
      }
      block.$set(block_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(block.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(block.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(block, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { loading_status } = $$props;
  let { label } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { open = false } = $$props;
  let { theme_mode } = $$props;
  let { show_indices } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let label_height = 0;
  function div_elementresize_handler() {
    label_height = this.clientHeight;
    $$invalidate(17, label_height);
  }
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("loading_status" in $$props2)
      $$invalidate(4, loading_status = $$props2.loading_status);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(6, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(7, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(8, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(9, min_width = $$props2.min_width);
    if ("gradio" in $$props2)
      $$invalidate(10, gradio = $$props2.gradio);
    if ("open" in $$props2)
      $$invalidate(11, open = $$props2.open);
    if ("theme_mode" in $$props2)
      $$invalidate(12, theme_mode = $$props2.theme_mode);
    if ("show_indices" in $$props2)
      $$invalidate(13, show_indices = $$props2.show_indices);
    if ("height" in $$props2)
      $$invalidate(14, height = $$props2.height);
    if ("min_height" in $$props2)
      $$invalidate(15, min_height = $$props2.min_height);
    if ("max_height" in $$props2)
      $$invalidate(16, max_height = $$props2.max_height);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value, old_value, gradio*/
    263176) {
      {
        if (value !== old_value) {
          $$invalidate(18, old_value = value);
          gradio.dispatch("change");
        }
      }
    }
  };
  return [
    elem_id,
    elem_classes,
    visible,
    value,
    loading_status,
    label,
    show_label,
    container,
    scale,
    min_width,
    gradio,
    open,
    theme_mode,
    show_indices,
    height,
    min_height,
    max_height,
    label_height,
    old_value,
    div_elementresize_handler,
    clear_status_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      visible: 2,
      value: 3,
      loading_status: 4,
      label: 5,
      show_label: 6,
      container: 7,
      scale: 8,
      min_width: 9,
      gradio: 10,
      open: 11,
      theme_mode: 12,
      show_indices: 13,
      height: 14,
      min_height: 15,
      max_height: 16
    });
  }
}
export {
  JSON$1 as BaseJSON,
  Index as default
};
