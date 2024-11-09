import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, element, claim_element, createEventDispatcher, ensure_array_like, space, claim_space, destroy_each, get_svelte_dataset, text, claim_text, set_data, listen, run_all, empty, toggle_class, set_style, group_outros, transition_out, check_outros, transition_in, onMount, binding_callbacks, bind, create_component, claim_component, mount_component, add_flush_callback, destroy_component, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { g as get_next_color } from "./color.DhNifA4j.js";
import { k as colors, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
function create_fragment$4(ctx) {
  let svg;
  let path0;
  let path1;
  return {
    c() {
      svg = svg_element("svg");
      path0 = svg_element("path");
      path1 = svg_element("path");
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
      path0 = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "fill", "currentColor");
      attr(path0, "d", "M12 15H5a3 3 0 0 1-3-3v-2a3 3 0 0 1 3-3h5V5a1 1 0 0 0-1-1H3V2h6a3 3 0 0 1 3 3zM5 9a1 1 0 0 0-1 1v2a1 1 0 0 0 1 1h5V9zm15 14v2a1 1 0 0 0 1 1h5v-4h-5a1 1 0 0 0-1 1z");
      attr(path1, "fill", "currentColor");
      attr(path1, "d", "M2 30h28V2Zm26-2h-7a3 3 0 0 1-3-3v-2a3 3 0 0 1 3-3h5v-2a1 1 0 0 0-1-1h-6v-2h6a3 3 0 0 1 3 3Z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "xmlns:xlink", "http://www.w3.org/1999/xlink");
      attr(svg, "aria-hidden", "true");
      attr(svg, "role", "img");
      attr(svg, "class", "iconify iconify--carbon");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "preserveAspectRatio", "xMidYMid meet");
      attr(svg, "viewBox", "0 0 32 32");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
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
class TextHighlight extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$4, safe_not_equal, {});
  }
}
function name_to_rgba(name, a, ctx) {
  if (!ctx) {
    var canvas = document.createElement("canvas");
    ctx = canvas.getContext("2d");
  }
  ctx.fillStyle = name;
  ctx.fillRect(0, 0, 1, 1);
  const [r, g, b] = ctx.getImageData(0, 0, 1, 1).data;
  ctx.clearRect(0, 0, 1, 1);
  return `rgba(${r}, ${g}, ${b}, ${255 / a})`;
}
function correct_color_map(color_map, _color_map, browser, ctx) {
  for (const col in color_map) {
    const _c = color_map[col].trim();
    if (_c in colors) {
      _color_map[col] = colors[_c];
    } else {
      _color_map[col] = {
        primary: browser ? name_to_rgba(color_map[col], 1, ctx) : color_map[col],
        secondary: browser ? name_to_rgba(color_map[col], 0.5, ctx) : color_map[col]
      };
    }
  }
}
function merge_elements(value, mergeMode) {
  let result = [];
  let tempStr = null;
  let tempVal = null;
  for (const val of value) {
    if (tempVal === val.class_or_confidence) {
      tempStr = tempStr ? tempStr + val.token : val.token;
    } else {
      if (tempStr !== null) {
        result.push({
          token: tempStr,
          class_or_confidence: tempVal
        });
      }
      tempStr = val.token;
      tempVal = val.class_or_confidence;
    }
  }
  if (tempStr !== null) {
    result.push({
      token: tempStr,
      class_or_confidence: tempVal
    });
  }
  return result;
}
function get_each_context_3$1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[18] = list[i];
  const constants_0 = typeof /*v*/
  child_ctx[18].class_or_confidence === "string" ? parseInt(
    /*v*/
    child_ctx[18].class_or_confidence
  ) : (
    /*v*/
    child_ctx[18].class_or_confidence
  );
  child_ctx[27] = constants_0;
  return child_ctx;
}
function get_each_context$1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[18] = list[i];
  child_ctx[20] = i;
  return child_ctx;
}
function get_each_context_1$1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[21] = list[i];
  child_ctx[23] = i;
  return child_ctx;
}
function get_each_context_2$1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[24] = list[i][0];
  child_ctx[25] = list[i][1];
  child_ctx[20] = i;
  return child_ctx;
}
function create_else_block$3(ctx) {
  let t;
  let div;
  let if_block = (
    /*show_legend*/
    ctx[1] && create_if_block_5$1()
  );
  let each_value_3 = ensure_array_like(
    /*value*/
    ctx[0]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_3.length; i += 1) {
    each_blocks[i] = create_each_block_3$1(get_each_context_3$1(ctx, each_value_3, i));
  }
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "textfield svelte-ju12zg");
      attr(div, "data-testid", "highlighted-text:textfield");
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
    },
    p(ctx2, dirty) {
      if (
        /*show_legend*/
        ctx2[1]
      ) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_5$1();
          if_block.c();
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty & /*value, parseInt*/
      1) {
        each_value_3 = ensure_array_like(
          /*value*/
          ctx2[0]
        );
        let i;
        for (i = 0; i < each_value_3.length; i += 1) {
          const child_ctx = get_each_context_3$1(ctx2, each_value_3, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_3$1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_3.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(div);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block$3(ctx) {
  let t;
  let div;
  let if_block = (
    /*show_legend*/
    ctx[1] && create_if_block_4$2(ctx)
  );
  let each_value = ensure_array_like(
    /*value*/
    ctx[0]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block$1(get_each_context$1(ctx, each_value, i));
  }
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "textfield svelte-ju12zg");
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
    },
    p(ctx2, dirty) {
      if (
        /*show_legend*/
        ctx2[1]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_4$2(ctx2);
          if_block.c();
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty & /*splitTextByNewline, value, active, selectable, _color_map, dispatch, show_legend, show_inline_category*/
      223) {
        each_value = ensure_array_like(
          /*value*/
          ctx2[0]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context$1(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block$1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(div);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block_5$1(ctx) {
  let div;
  let textContent = `<span>-1</span> <span>0</span> <span>+1</span>`;
  return {
    c() {
      div = element("div");
      div.innerHTML = textContent;
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        class: true,
        "data-testid": true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(div) !== "svelte-mv3vmx")
        div.innerHTML = textContent;
      this.h();
    },
    h() {
      attr(div, "class", "color-legend svelte-ju12zg");
      attr(div, "data-testid", "highlighted-text:color-legend");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_each_block_3$1(ctx) {
  let span1;
  let span0;
  let t0_value = (
    /*v*/
    ctx[18].token + ""
  );
  let t0;
  let t1;
  let span1_style_value;
  return {
    c() {
      span1 = element("span");
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      span1 = claim_element(nodes, "SPAN", { class: true, style: true });
      var span1_nodes = children(span1);
      span0 = claim_element(span1_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(span1_nodes);
      span1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "text svelte-ju12zg");
      attr(span1, "class", "textspan score-text svelte-ju12zg");
      attr(span1, "style", span1_style_value = "background-color: rgba(" + /*score*/
      (ctx[27] && /*score*/
      ctx[27] < 0 ? "128, 90, 213," + -/*score*/
      ctx[27] : "239, 68, 60," + /*score*/
      ctx[27]) + ")");
    },
    m(target, anchor) {
      insert_hydration(target, span1, anchor);
      append_hydration(span1, span0);
      append_hydration(span0, t0);
      append_hydration(span1, t1);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t0_value !== (t0_value = /*v*/
      ctx2[18].token + ""))
        set_data(t0, t0_value);
      if (dirty & /*value*/
      1 && span1_style_value !== (span1_style_value = "background-color: rgba(" + /*score*/
      (ctx2[27] && /*score*/
      ctx2[27] < 0 ? "128, 90, 213," + -/*score*/
      ctx2[27] : "239, 68, 60," + /*score*/
      ctx2[27]) + ")")) {
        attr(span1, "style", span1_style_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(span1);
      }
    }
  };
}
function create_if_block_4$2(ctx) {
  let div;
  let each_value_2 = ensure_array_like(Object.entries(
    /*_color_map*/
    ctx[6]
  ));
  let each_blocks = [];
  for (let i = 0; i < each_value_2.length; i += 1) {
    each_blocks[i] = create_each_block_2$1(get_each_context_2$1(ctx, each_value_2, i));
  }
  return {
    c() {
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "category-legend svelte-ju12zg");
      attr(div, "data-testid", "highlighted-text:category-legend");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*Object, _color_map, handle_mouseover, handle_mouseout*/
      832) {
        each_value_2 = ensure_array_like(Object.entries(
          /*_color_map*/
          ctx2[6]
        ));
        let i;
        for (i = 0; i < each_value_2.length; i += 1) {
          const child_ctx = get_each_context_2$1(ctx2, each_value_2, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_2$1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_2.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_each_block_2$1(ctx) {
  let div;
  let t0_value = (
    /*category*/
    ctx[24] + ""
  );
  let t0;
  let t1;
  let mounted;
  let dispose;
  function mouseover_handler() {
    return (
      /*mouseover_handler*/
      ctx[11](
        /*category*/
        ctx[24]
      )
    );
  }
  function focus_handler() {
    return (
      /*focus_handler*/
      ctx[12](
        /*category*/
        ctx[24]
      )
    );
  }
  return {
    c() {
      div = element("div");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, style: true });
      var div_nodes = children(div);
      t0 = claim_text(div_nodes, t0_value);
      t1 = claim_space(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "category-label svelte-ju12zg");
      attr(div, "style", "background-color:" + /*color*/
      ctx[25].secondary);
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t0);
      append_hydration(div, t1);
      if (!mounted) {
        dispose = [
          listen(div, "mouseover", mouseover_handler),
          listen(div, "focus", focus_handler),
          listen(
            div,
            "mouseout",
            /*mouseout_handler*/
            ctx[13]
          ),
          listen(
            div,
            "blur",
            /*blur_handler*/
            ctx[14]
          )
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_2$2(ctx) {
  let span1;
  let span0;
  let t0_value = (
    /*line*/
    ctx[21] + ""
  );
  let t0;
  let t1;
  let mounted;
  let dispose;
  let if_block = !/*show_legend*/
  ctx[1] && /*show_inline_category*/
  ctx[2] && /*v*/
  ctx[18].class_or_confidence !== null && create_if_block_3$2(ctx);
  function click_handler() {
    return (
      /*click_handler*/
      ctx[15](
        /*i*/
        ctx[20],
        /*v*/
        ctx[18]
      )
    );
  }
  return {
    c() {
      span1 = element("span");
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      span1 = claim_element(nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      span0 = claim_element(span1_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(span1_nodes);
      if (if_block)
        if_block.l(span1_nodes);
      span1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "text svelte-ju12zg");
      toggle_class(
        span0,
        "no-label",
        /*v*/
        ctx[18].class_or_confidence === null || !/*_color_map*/
        ctx[6][
          /*v*/
          ctx[18].class_or_confidence
        ]
      );
      attr(span1, "class", "textspan svelte-ju12zg");
      toggle_class(
        span1,
        "no-cat",
        /*v*/
        ctx[18].class_or_confidence === null || /*active*/
        ctx[4] && /*active*/
        ctx[4] !== /*v*/
        ctx[18].class_or_confidence
      );
      toggle_class(
        span1,
        "hl",
        /*v*/
        ctx[18].class_or_confidence !== null
      );
      toggle_class(
        span1,
        "selectable",
        /*selectable*/
        ctx[3]
      );
      set_style(
        span1,
        "background-color",
        /*v*/
        ctx[18].class_or_confidence === null || /*active*/
        ctx[4] && /*active*/
        ctx[4] !== /*v*/
        ctx[18].class_or_confidence ? "" : (
          /*_color_map*/
          ctx[6][
            /*v*/
            ctx[18].class_or_confidence
          ].secondary
        )
      );
    },
    m(target, anchor) {
      insert_hydration(target, span1, anchor);
      append_hydration(span1, span0);
      append_hydration(span0, t0);
      append_hydration(span1, t1);
      if (if_block)
        if_block.m(span1, null);
      if (!mounted) {
        dispose = listen(span1, "click", click_handler);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty & /*value*/
      1 && t0_value !== (t0_value = /*line*/
      ctx[21] + ""))
        set_data(t0, t0_value);
      if (dirty & /*value, _color_map*/
      65) {
        toggle_class(
          span0,
          "no-label",
          /*v*/
          ctx[18].class_or_confidence === null || !/*_color_map*/
          ctx[6][
            /*v*/
            ctx[18].class_or_confidence
          ]
        );
      }
      if (!/*show_legend*/
      ctx[1] && /*show_inline_category*/
      ctx[2] && /*v*/
      ctx[18].class_or_confidence !== null) {
        if (if_block) {
          if_block.p(ctx, dirty);
        } else {
          if_block = create_if_block_3$2(ctx);
          if_block.c();
          if_block.m(span1, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty & /*value, active*/
      17) {
        toggle_class(
          span1,
          "no-cat",
          /*v*/
          ctx[18].class_or_confidence === null || /*active*/
          ctx[4] && /*active*/
          ctx[4] !== /*v*/
          ctx[18].class_or_confidence
        );
      }
      if (dirty & /*value*/
      1) {
        toggle_class(
          span1,
          "hl",
          /*v*/
          ctx[18].class_or_confidence !== null
        );
      }
      if (dirty & /*selectable*/
      8) {
        toggle_class(
          span1,
          "selectable",
          /*selectable*/
          ctx[3]
        );
      }
      if (dirty & /*value, active*/
      17) {
        set_style(
          span1,
          "background-color",
          /*v*/
          ctx[18].class_or_confidence === null || /*active*/
          ctx[4] && /*active*/
          ctx[4] !== /*v*/
          ctx[18].class_or_confidence ? "" : (
            /*_color_map*/
            ctx[6][
              /*v*/
              ctx[18].class_or_confidence
            ].secondary
          )
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(span1);
      }
      if (if_block)
        if_block.d();
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_3$2(ctx) {
  let t0;
  let span;
  let t1_value = (
    /*v*/
    ctx[18].class_or_confidence + ""
  );
  let t1;
  return {
    c() {
      t0 = text(" \n								");
      span = element("span");
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      t0 = claim_text(nodes, " \n								");
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t1 = claim_text(span_nodes, t1_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "label svelte-ju12zg");
      set_style(
        span,
        "background-color",
        /*v*/
        ctx[18].class_or_confidence === null || /*active*/
        ctx[4] && /*active*/
        ctx[4] !== /*v*/
        ctx[18].class_or_confidence ? "" : (
          /*_color_map*/
          ctx[6][
            /*v*/
            ctx[18].class_or_confidence
          ].primary
        )
      );
    },
    m(target, anchor) {
      insert_hydration(target, t0, anchor);
      insert_hydration(target, span, anchor);
      append_hydration(span, t1);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t1_value !== (t1_value = /*v*/
      ctx2[18].class_or_confidence + ""))
        set_data(t1, t1_value);
      if (dirty & /*value, active*/
      17) {
        set_style(
          span,
          "background-color",
          /*v*/
          ctx2[18].class_or_confidence === null || /*active*/
          ctx2[4] && /*active*/
          ctx2[4] !== /*v*/
          ctx2[18].class_or_confidence ? "" : (
            /*_color_map*/
            ctx2[6][
              /*v*/
              ctx2[18].class_or_confidence
            ].primary
          )
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(span);
      }
    }
  };
}
function create_if_block_1$2(ctx) {
  let br;
  return {
    c() {
      br = element("br");
    },
    l(nodes) {
      br = claim_element(nodes, "BR", {});
    },
    m(target, anchor) {
      insert_hydration(target, br, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(br);
      }
    }
  };
}
function create_each_block_1$1(ctx) {
  let show_if_1 = (
    /*line*/
    ctx[21].trim() !== ""
  );
  let t;
  let show_if = (
    /*j*/
    ctx[23] < splitTextByNewline$1(
      /*v*/
      ctx[18].token
    ).length - 1
  );
  let if_block1_anchor;
  let if_block0 = show_if_1 && create_if_block_2$2(ctx);
  let if_block1 = show_if && create_if_block_1$2();
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1)
        show_if_1 = /*line*/
        ctx2[21].trim() !== "";
      if (show_if_1) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_2$2(ctx2);
          if_block0.c();
          if_block0.m(t.parentNode, t);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (dirty & /*value*/
      1)
        show_if = /*j*/
        ctx2[23] < splitTextByNewline$1(
          /*v*/
          ctx2[18].token
        ).length - 1;
      if (show_if) {
        if (if_block1)
          ;
        else {
          if_block1 = create_if_block_1$2();
          if_block1.c();
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_each_block$1(ctx) {
  let each_1_anchor;
  let each_value_1 = ensure_array_like(splitTextByNewline$1(
    /*v*/
    ctx[18].token
  ));
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1$1(get_each_context_1$1(ctx, each_value_1, i));
  }
  return {
    c() {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      each_1_anchor = empty();
    },
    l(nodes) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(nodes);
      }
      each_1_anchor = empty();
    },
    m(target, anchor) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(target, anchor);
        }
      }
      insert_hydration(target, each_1_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*splitTextByNewline, value, active, selectable, _color_map, dispatch, show_legend, show_inline_category*/
      223) {
        each_value_1 = ensure_array_like(splitTextByNewline$1(
          /*v*/
          ctx2[18].token
        ));
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1$1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_1$1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(each_1_anchor.parentNode, each_1_anchor);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_1.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(each_1_anchor);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_fragment$3(ctx) {
  let div;
  function select_block_type(ctx2, dirty) {
    if (
      /*mode*/
      ctx2[5] === "categories"
    )
      return create_if_block$3;
    return create_else_block$3;
  }
  let current_block_type = select_block_type(ctx);
  let if_block = current_block_type(ctx);
  return {
    c() {
      div = element("div");
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "container svelte-ju12zg");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if_block.m(div, null);
    },
    p(ctx2, [dirty]) {
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(div, null);
        }
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if_block.d();
    }
  };
}
function splitTextByNewline$1(text2) {
  return text2.split("\n");
}
function instance$3($$self, $$props, $$invalidate) {
  const browser = typeof document !== "undefined";
  let { value = [] } = $$props;
  let { show_legend = false } = $$props;
  let { show_inline_category = true } = $$props;
  let { color_map = {} } = $$props;
  let { selectable = false } = $$props;
  let ctx;
  let _color_map = {};
  let active = "";
  const dispatch = createEventDispatcher();
  let mode;
  function handle_mouseover(label) {
    $$invalidate(4, active = label);
  }
  function handle_mouseout() {
    $$invalidate(4, active = "");
  }
  const mouseover_handler = (category) => handle_mouseover(category);
  const focus_handler = (category) => handle_mouseover(category);
  const mouseout_handler = () => handle_mouseout();
  const blur_handler = () => handle_mouseout();
  const click_handler = (i, v) => {
    dispatch("select", {
      index: i,
      value: [v.token, v.class_or_confidence]
    });
  };
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("show_legend" in $$props2)
      $$invalidate(1, show_legend = $$props2.show_legend);
    if ("show_inline_category" in $$props2)
      $$invalidate(2, show_inline_category = $$props2.show_inline_category);
    if ("color_map" in $$props2)
      $$invalidate(10, color_map = $$props2.color_map);
    if ("selectable" in $$props2)
      $$invalidate(3, selectable = $$props2.selectable);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*color_map, value*/
    1025) {
      {
        if (!color_map) {
          $$invalidate(10, color_map = {});
        }
        if (value.length > 0) {
          for (let entry of value) {
            if (entry.class_or_confidence !== null) {
              if (typeof entry.class_or_confidence === "string") {
                $$invalidate(5, mode = "categories");
                if (!(entry.class_or_confidence in color_map)) {
                  let color = get_next_color(Object.keys(color_map).length);
                  $$invalidate(10, color_map[entry.class_or_confidence] = color, color_map);
                }
              } else {
                $$invalidate(5, mode = "scores");
              }
            }
          }
        }
        correct_color_map(color_map, _color_map, browser, ctx);
      }
    }
  };
  return [
    value,
    show_legend,
    show_inline_category,
    selectable,
    active,
    mode,
    _color_map,
    dispatch,
    handle_mouseover,
    handle_mouseout,
    color_map,
    mouseover_handler,
    focus_handler,
    mouseout_handler,
    blur_handler,
    click_handler
  ];
}
class StaticHighlightedtext extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, {
      value: 0,
      show_legend: 1,
      show_inline_category: 2,
      color_map: 10,
      selectable: 3
    });
  }
}
const StaticHighlightedText = StaticHighlightedtext;
function create_else_block$2(ctx) {
  let input;
  let input_style_value;
  let mounted;
  let dispose;
  return {
    c() {
      input = element("input");
      this.h();
    },
    l(nodes) {
      input = claim_element(nodes, "INPUT", {
        class: true,
        type: true,
        step: true,
        style: true
      });
      this.h();
    },
    h() {
      attr(input, "class", "label-input svelte-1cag2po");
      input.autofocus = true;
      attr(input, "type", "number");
      attr(input, "step", "0.1");
      attr(input, "style", input_style_value = "background-color: rgba(" + (typeof /*category*/
      ctx[1] === "number" && /*category*/
      ctx[1] < 0 ? "128, 90, 213," + -/*category*/
      ctx[1] : "239, 68, 60," + /*category*/
      ctx[1]) + ")");
      input.value = /*category*/
      ctx[1];
      set_style(input, "width", `7ch`);
    },
    m(target, anchor) {
      insert_hydration(target, input, anchor);
      input.focus();
      if (!mounted) {
        dispose = [
          listen(
            input,
            "input",
            /*handleInput*/
            ctx[8]
          ),
          listen(
            input,
            "blur",
            /*blur_handler_1*/
            ctx[14]
          ),
          listen(
            input,
            "keydown",
            /*keydown_handler_1*/
            ctx[15]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*category*/
      2 && input_style_value !== (input_style_value = "background-color: rgba(" + (typeof /*category*/
      ctx2[1] === "number" && /*category*/
      ctx2[1] < 0 ? "128, 90, 213," + -/*category*/
      ctx2[1] : "239, 68, 60," + /*category*/
      ctx2[1]) + ")")) {
        attr(input, "style", input_style_value);
      }
      if (dirty & /*category*/
      2 && input.value !== /*category*/
      ctx2[1]) {
        input.value = /*category*/
        ctx2[1];
      }
      const style_changed = dirty & /*category*/
      2;
      if (dirty & /*category*/
      2 || style_changed) {
        set_style(input, "width", `7ch`);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(input);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block$2(ctx) {
  let input;
  let input_id_value;
  let mounted;
  let dispose;
  return {
    c() {
      input = element("input");
      this.h();
    },
    l(nodes) {
      input = claim_element(nodes, "INPUT", {
        class: true,
        id: true,
        type: true,
        placeholder: true
      });
      this.h();
    },
    h() {
      var _a;
      attr(input, "class", "label-input svelte-1cag2po");
      input.autofocus = true;
      attr(input, "id", input_id_value = `label-input-${/*indexOfLabel*/
      ctx[3]}`);
      attr(input, "type", "text");
      attr(input, "placeholder", "label");
      input.value = /*category*/
      ctx[1];
      set_style(
        input,
        "background-color",
        /*category*/
        ctx[1] === null || /*active*/
        ctx[2] && /*active*/
        ctx[2] !== /*category*/
        ctx[1] ? "" : (
          /*_color_map*/
          ctx[6][
            /*category*/
            ctx[1]
          ].primary
        )
      );
      set_style(
        input,
        "width",
        /*_input_value*/
        ctx[7] ? (
          /*_input_value*/
          ((_a = ctx[7].toString()) == null ? void 0 : _a.length) + 4 + "ch"
        ) : "8ch"
      );
    },
    m(target, anchor) {
      insert_hydration(target, input, anchor);
      input.focus();
      if (!mounted) {
        dispose = [
          listen(
            input,
            "input",
            /*handleInput*/
            ctx[8]
          ),
          listen(
            input,
            "blur",
            /*blur_handler*/
            ctx[12]
          ),
          listen(
            input,
            "keydown",
            /*keydown_handler*/
            ctx[13]
          ),
          listen(input, "focus", clearPlaceHolderOnFocus)
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      var _a;
      if (dirty & /*indexOfLabel*/
      8 && input_id_value !== (input_id_value = `label-input-${/*indexOfLabel*/
      ctx2[3]}`)) {
        attr(input, "id", input_id_value);
      }
      if (dirty & /*category*/
      2 && input.value !== /*category*/
      ctx2[1]) {
        input.value = /*category*/
        ctx2[1];
      }
      if (dirty & /*category, active, _color_map*/
      70) {
        set_style(
          input,
          "background-color",
          /*category*/
          ctx2[1] === null || /*active*/
          ctx2[2] && /*active*/
          ctx2[2] !== /*category*/
          ctx2[1] ? "" : (
            /*_color_map*/
            ctx2[6][
              /*category*/
              ctx2[1]
            ].primary
          )
        );
      }
      if (dirty & /*_input_value*/
      128) {
        set_style(
          input,
          "width",
          /*_input_value*/
          ctx2[7] ? (
            /*_input_value*/
            ((_a = ctx2[7].toString()) == null ? void 0 : _a.length) + 4 + "ch"
          ) : "8ch"
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(input);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_fragment$2(ctx) {
  let if_block_anchor;
  function select_block_type(ctx2, dirty) {
    if (!/*isScoresMode*/
    ctx2[5])
      return create_if_block$2;
    return create_else_block$2;
  }
  let current_block_type = select_block_type(ctx);
  let if_block = current_block_type(ctx);
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
      if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, [dirty]) {
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if_block.d(detaching);
    }
  };
}
function clearPlaceHolderOnFocus(e) {
  let target = e.target;
  if (target && target.placeholder)
    target.placeholder = "";
}
function instance$2($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { category } = $$props;
  let { active } = $$props;
  let { labelToEdit } = $$props;
  let { indexOfLabel } = $$props;
  let { text: text2 } = $$props;
  let { handleValueChange } = $$props;
  let { isScoresMode = false } = $$props;
  let { _color_map } = $$props;
  let _input_value = category;
  function handleInput(e) {
    let target = e.target;
    if (target) {
      $$invalidate(7, _input_value = target.value);
    }
  }
  function updateLabelValue(e, elementIndex, text22) {
    let target = e.target;
    $$invalidate(10, value = [
      ...value.slice(0, elementIndex),
      {
        token: text22,
        class_or_confidence: target.value === "" ? null : isScoresMode ? Number(target.value) : target.value
      },
      ...value.slice(elementIndex + 1)
    ]);
    handleValueChange();
  }
  const blur_handler = (e) => updateLabelValue(e, indexOfLabel, text2);
  const keydown_handler = (e) => {
    if (e.key === "Enter") {
      updateLabelValue(e, indexOfLabel, text2);
      $$invalidate(0, labelToEdit = -1);
    }
  };
  const blur_handler_1 = (e) => updateLabelValue(e, indexOfLabel, text2);
  const keydown_handler_1 = (e) => {
    if (e.key === "Enter") {
      updateLabelValue(e, indexOfLabel, text2);
      $$invalidate(0, labelToEdit = -1);
    }
  };
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(10, value = $$props2.value);
    if ("category" in $$props2)
      $$invalidate(1, category = $$props2.category);
    if ("active" in $$props2)
      $$invalidate(2, active = $$props2.active);
    if ("labelToEdit" in $$props2)
      $$invalidate(0, labelToEdit = $$props2.labelToEdit);
    if ("indexOfLabel" in $$props2)
      $$invalidate(3, indexOfLabel = $$props2.indexOfLabel);
    if ("text" in $$props2)
      $$invalidate(4, text2 = $$props2.text);
    if ("handleValueChange" in $$props2)
      $$invalidate(11, handleValueChange = $$props2.handleValueChange);
    if ("isScoresMode" in $$props2)
      $$invalidate(5, isScoresMode = $$props2.isScoresMode);
    if ("_color_map" in $$props2)
      $$invalidate(6, _color_map = $$props2._color_map);
  };
  return [
    labelToEdit,
    category,
    active,
    indexOfLabel,
    text2,
    isScoresMode,
    _color_map,
    _input_value,
    handleInput,
    updateLabelValue,
    value,
    handleValueChange,
    blur_handler,
    keydown_handler,
    blur_handler_1,
    keydown_handler_1
  ];
}
class LabelInput extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      value: 10,
      category: 1,
      active: 2,
      labelToEdit: 0,
      indexOfLabel: 3,
      text: 4,
      handleValueChange: 11,
      isScoresMode: 5,
      _color_map: 6
    });
  }
}
function get_each_context_3(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[45] = list[i].token;
  child_ctx[46] = list[i].class_or_confidence;
  child_ctx[48] = i;
  const constants_0 = typeof /*class_or_confidence*/
  child_ctx[46] === "string" ? parseInt(
    /*class_or_confidence*/
    child_ctx[46]
  ) : (
    /*class_or_confidence*/
    child_ctx[46]
  );
  child_ctx[54] = constants_0;
  return child_ctx;
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[45] = list[i].token;
  child_ctx[46] = list[i].class_or_confidence;
  child_ctx[48] = i;
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[49] = list[i];
  child_ctx[51] = i;
  return child_ctx;
}
function get_each_context_2(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[46] = list[i][0];
  child_ctx[52] = list[i][1];
  child_ctx[48] = i;
  return child_ctx;
}
function create_else_block$1(ctx) {
  let t;
  let div;
  let current;
  let if_block = (
    /*show_legend*/
    ctx[1] && create_if_block_10()
  );
  let each_value_3 = ensure_array_like(
    /*value*/
    ctx[0]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_3.length; i += 1) {
    each_blocks[i] = create_each_block_3(get_each_context_3(ctx, each_value_3, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "textfield svelte-1ozsnjl");
      attr(div, "data-testid", "highlighted-text:textfield");
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_legend*/
        ctx2[1]
      ) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_10();
          if_block.c();
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty[0] & /*removeHighlightedText, value, activeElementIndex, active, labelToEdit, _color_map, handleValueChange*/
      889) {
        each_value_3 = ensure_array_like(
          /*value*/
          ctx2[0]
        );
        let i;
        for (i = 0; i < each_value_3.length; i += 1) {
          const child_ctx = get_each_context_3(ctx2, each_value_3, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_3(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div, null);
          }
        }
        group_outros();
        for (i = each_value_3.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_3.length; i += 1) {
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
        detach(t);
        detach(div);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let t;
  let div;
  let current;
  let if_block = (
    /*show_legend*/
    ctx[1] && create_if_block_6(ctx)
  );
  let each_value = ensure_array_like(
    /*value*/
    ctx[0]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "textfield svelte-1ozsnjl");
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_legend*/
        ctx2[1]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_6(ctx2);
          if_block.c();
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty[0] & /*value, removeHighlightedText, active, selectable, _color_map, handleSelect, labelToEdit, handleKeydownSelection, activeElementIndex, handleValueChange, show_legend*/
      13183) {
        each_value = ensure_array_like(
          /*value*/
          ctx2[0]
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
            each_blocks[i].m(div, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
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
        detach(t);
        detach(div);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block_10(ctx) {
  let div;
  let textContent = `<span>-1</span> <span>0</span> <span>+1</span>`;
  return {
    c() {
      div = element("div");
      div.innerHTML = textContent;
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        class: true,
        "data-testid": true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(div) !== "svelte-mv3vmx")
        div.innerHTML = textContent;
      this.h();
    },
    h() {
      attr(div, "class", "color-legend svelte-1ozsnjl");
      attr(div, "data-testid", "highlighted-text:color-legend");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_if_block_9(ctx) {
  let labelinput;
  let updating_value;
  let current;
  function labelinput_value_binding_1(value) {
    ctx[32](value);
  }
  let labelinput_props = {
    labelToEdit: (
      /*labelToEdit*/
      ctx[6]
    ),
    _color_map: (
      /*_color_map*/
      ctx[3]
    ),
    category: (
      /*class_or_confidence*/
      ctx[46]
    ),
    active: (
      /*active*/
      ctx[5]
    ),
    indexOfLabel: (
      /*i*/
      ctx[48]
    ),
    text: (
      /*token*/
      ctx[45]
    ),
    handleValueChange: (
      /*handleValueChange*/
      ctx[9]
    ),
    isScoresMode: true
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    labelinput_props.value = /*value*/
    ctx[0];
  }
  labelinput = new LabelInput({ props: labelinput_props });
  binding_callbacks.push(() => bind(labelinput, "value", labelinput_value_binding_1));
  return {
    c() {
      create_component(labelinput.$$.fragment);
    },
    l(nodes) {
      claim_component(labelinput.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(labelinput, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const labelinput_changes = {};
      if (dirty[0] & /*labelToEdit*/
      64)
        labelinput_changes.labelToEdit = /*labelToEdit*/
        ctx2[6];
      if (dirty[0] & /*_color_map*/
      8)
        labelinput_changes._color_map = /*_color_map*/
        ctx2[3];
      if (dirty[0] & /*value*/
      1)
        labelinput_changes.category = /*class_or_confidence*/
        ctx2[46];
      if (dirty[0] & /*active*/
      32)
        labelinput_changes.active = /*active*/
        ctx2[5];
      if (dirty[0] & /*value*/
      1)
        labelinput_changes.text = /*token*/
        ctx2[45];
      if (!updating_value && dirty[0] & /*value*/
      1) {
        updating_value = true;
        labelinput_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      labelinput.$set(labelinput_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(labelinput.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(labelinput.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(labelinput, detaching);
    }
  };
}
function create_if_block_8(ctx) {
  let span;
  let textContent = "×";
  let mounted;
  let dispose;
  function click_handler_5() {
    return (
      /*click_handler_5*/
      ctx[37](
        /*i*/
        ctx[48]
      )
    );
  }
  function keydown_handler_5(...args) {
    return (
      /*keydown_handler_5*/
      ctx[38](
        /*i*/
        ctx[48],
        ...args
      )
    );
  }
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", {
        class: true,
        role: true,
        "aria-roledescription": true,
        tabindex: true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(span) !== "svelte-hxhs1z")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "label-clear-button svelte-1ozsnjl");
      attr(span, "role", "button");
      attr(span, "aria-roledescription", "Remove label from text");
      attr(span, "tabindex", "0");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      if (!mounted) {
        dispose = [
          listen(span, "click", click_handler_5),
          listen(span, "keydown", keydown_handler_5)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_each_block_3(ctx) {
  let span2;
  let span1;
  let span0;
  let t0_value = (
    /*token*/
    ctx[45] + ""
  );
  let t0;
  let t1;
  let span1_style_value;
  let t2;
  let t3;
  let current;
  let mounted;
  let dispose;
  let if_block0 = (
    /*class_or_confidence*/
    ctx[46] && /*labelToEdit*/
    ctx[6] === /*i*/
    ctx[48] && create_if_block_9(ctx)
  );
  function mouseover_handler_3() {
    return (
      /*mouseover_handler_3*/
      ctx[33](
        /*i*/
        ctx[48]
      )
    );
  }
  function focus_handler_3() {
    return (
      /*focus_handler_3*/
      ctx[34](
        /*i*/
        ctx[48]
      )
    );
  }
  function click_handler_4() {
    return (
      /*click_handler_4*/
      ctx[35](
        /*i*/
        ctx[48]
      )
    );
  }
  function keydown_handler_4(...args) {
    return (
      /*keydown_handler_4*/
      ctx[36](
        /*i*/
        ctx[48],
        ...args
      )
    );
  }
  let if_block1 = (
    /*class_or_confidence*/
    ctx[46] && /*activeElementIndex*/
    ctx[4] === /*i*/
    ctx[48] && create_if_block_8(ctx)
  );
  return {
    c() {
      span2 = element("span");
      span1 = element("span");
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      if (if_block0)
        if_block0.c();
      t2 = space();
      if (if_block1)
        if_block1.c();
      t3 = space();
      this.h();
    },
    l(nodes) {
      span2 = claim_element(nodes, "SPAN", { class: true });
      var span2_nodes = children(span2);
      span1 = claim_element(span2_nodes, "SPAN", {
        class: true,
        role: true,
        tabindex: true,
        style: true
      });
      var span1_nodes = children(span1);
      span0 = claim_element(span1_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(span1_nodes);
      if (if_block0)
        if_block0.l(span1_nodes);
      span1_nodes.forEach(detach);
      t2 = claim_space(span2_nodes);
      if (if_block1)
        if_block1.l(span2_nodes);
      t3 = claim_space(span2_nodes);
      span2_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "text svelte-1ozsnjl");
      attr(span1, "class", "textspan score-text svelte-1ozsnjl");
      attr(span1, "role", "button");
      attr(span1, "tabindex", "0");
      attr(span1, "style", span1_style_value = "background-color: rgba(" + /*score*/
      (ctx[54] && /*score*/
      ctx[54] < 0 ? "128, 90, 213," + -/*score*/
      ctx[54] : "239, 68, 60," + /*score*/
      ctx[54]) + ")");
      toggle_class(
        span1,
        "no-cat",
        /*class_or_confidence*/
        ctx[46] === null || /*active*/
        ctx[5] && /*active*/
        ctx[5] !== /*class_or_confidence*/
        ctx[46]
      );
      toggle_class(
        span1,
        "hl",
        /*class_or_confidence*/
        ctx[46] !== null
      );
      attr(span2, "class", "score-text-container svelte-1ozsnjl");
    },
    m(target, anchor) {
      insert_hydration(target, span2, anchor);
      append_hydration(span2, span1);
      append_hydration(span1, span0);
      append_hydration(span0, t0);
      append_hydration(span1, t1);
      if (if_block0)
        if_block0.m(span1, null);
      append_hydration(span2, t2);
      if (if_block1)
        if_block1.m(span2, null);
      append_hydration(span2, t3);
      current = true;
      if (!mounted) {
        dispose = [
          listen(span1, "mouseover", mouseover_handler_3),
          listen(span1, "focus", focus_handler_3),
          listen(span1, "click", click_handler_4),
          listen(span1, "keydown", keydown_handler_4)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if ((!current || dirty[0] & /*value*/
      1) && t0_value !== (t0_value = /*token*/
      ctx[45] + ""))
        set_data(t0, t0_value);
      if (
        /*class_or_confidence*/
        ctx[46] && /*labelToEdit*/
        ctx[6] === /*i*/
        ctx[48]
      ) {
        if (if_block0) {
          if_block0.p(ctx, dirty);
          if (dirty[0] & /*value, labelToEdit*/
          65) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_9(ctx);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(span1, null);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (!current || dirty[0] & /*value*/
      1 && span1_style_value !== (span1_style_value = "background-color: rgba(" + /*score*/
      (ctx[54] && /*score*/
      ctx[54] < 0 ? "128, 90, 213," + -/*score*/
      ctx[54] : "239, 68, 60," + /*score*/
      ctx[54]) + ")")) {
        attr(span1, "style", span1_style_value);
      }
      if (!current || dirty[0] & /*value, active*/
      33) {
        toggle_class(
          span1,
          "no-cat",
          /*class_or_confidence*/
          ctx[46] === null || /*active*/
          ctx[5] && /*active*/
          ctx[5] !== /*class_or_confidence*/
          ctx[46]
        );
      }
      if (!current || dirty[0] & /*value*/
      1) {
        toggle_class(
          span1,
          "hl",
          /*class_or_confidence*/
          ctx[46] !== null
        );
      }
      if (
        /*class_or_confidence*/
        ctx[46] && /*activeElementIndex*/
        ctx[4] === /*i*/
        ctx[48]
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
        } else {
          if_block1 = create_if_block_8(ctx);
          if_block1.c();
          if_block1.m(span2, t3);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(span2);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_6(ctx) {
  let div;
  let if_block = (
    /*_color_map*/
    ctx[3] && create_if_block_7(ctx)
  );
  return {
    c() {
      div = element("div");
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "class_or_confidence-legend svelte-1ozsnjl");
      attr(div, "data-testid", "highlighted-text:class_or_confidence-legend");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (if_block)
        if_block.m(div, null);
    },
    p(ctx2, dirty) {
      if (
        /*_color_map*/
        ctx2[3]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_7(ctx2);
          if_block.c();
          if_block.m(div, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (if_block)
        if_block.d();
    }
  };
}
function create_if_block_7(ctx) {
  let each_1_anchor;
  let each_value_2 = ensure_array_like(Object.entries(
    /*_color_map*/
    ctx[3]
  ));
  let each_blocks = [];
  for (let i = 0; i < each_value_2.length; i += 1) {
    each_blocks[i] = create_each_block_2(get_each_context_2(ctx, each_value_2, i));
  }
  return {
    c() {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      each_1_anchor = empty();
    },
    l(nodes) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(nodes);
      }
      each_1_anchor = empty();
    },
    m(target, anchor) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(target, anchor);
        }
      }
      insert_hydration(target, each_1_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*_color_map, handle_mouseover, handle_mouseout*/
      3080) {
        each_value_2 = ensure_array_like(Object.entries(
          /*_color_map*/
          ctx2[3]
        ));
        let i;
        for (i = 0; i < each_value_2.length; i += 1) {
          const child_ctx = get_each_context_2(ctx2, each_value_2, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_2(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(each_1_anchor.parentNode, each_1_anchor);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_2.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(each_1_anchor);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_each_block_2(ctx) {
  let div;
  let t0_value = (
    /*class_or_confidence*/
    ctx[46] + ""
  );
  let t0;
  let t1;
  let div_style_value;
  let mounted;
  let dispose;
  function mouseover_handler() {
    return (
      /*mouseover_handler*/
      ctx[15](
        /*class_or_confidence*/
        ctx[46]
      )
    );
  }
  function focus_handler() {
    return (
      /*focus_handler*/
      ctx[16](
        /*class_or_confidence*/
        ctx[46]
      )
    );
  }
  return {
    c() {
      div = element("div");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        role: true,
        "aria-roledescription": true,
        tabindex: true,
        class: true,
        style: true
      });
      var div_nodes = children(div);
      t0 = claim_text(div_nodes, t0_value);
      t1 = claim_space(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "role", "button");
      attr(div, "aria-roledescription", "Categories of highlighted text. Hover to see text with this class_or_confidence highlighted.");
      attr(div, "tabindex", "0");
      attr(div, "class", "class_or_confidence-label svelte-1ozsnjl");
      attr(div, "style", div_style_value = "background-color:" + /*color*/
      ctx[52].secondary);
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t0);
      append_hydration(div, t1);
      if (!mounted) {
        dispose = [
          listen(div, "mouseover", mouseover_handler),
          listen(div, "focus", focus_handler),
          listen(
            div,
            "mouseout",
            /*mouseout_handler*/
            ctx[17]
          ),
          listen(
            div,
            "blur",
            /*blur_handler*/
            ctx[18]
          )
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*_color_map*/
      8 && t0_value !== (t0_value = /*class_or_confidence*/
      ctx[46] + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*_color_map*/
      8 && div_style_value !== (div_style_value = "background-color:" + /*color*/
      ctx[52].secondary)) {
        attr(div, "style", div_style_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_2$1(ctx) {
  let span2;
  let span1;
  let span0;
  let t0_value = (
    /*line*/
    ctx[49] + ""
  );
  let t0;
  let t1;
  let t2;
  let t3;
  let current;
  let mounted;
  let dispose;
  function focus_handler_1() {
    return (
      /*focus_handler_1*/
      ctx[20](
        /*i*/
        ctx[48]
      )
    );
  }
  function mouseover_handler_1() {
    return (
      /*mouseover_handler_1*/
      ctx[21](
        /*i*/
        ctx[48]
      )
    );
  }
  function click_handler() {
    return (
      /*click_handler*/
      ctx[22](
        /*i*/
        ctx[48]
      )
    );
  }
  let if_block0 = !/*show_legend*/
  ctx[1] && /*class_or_confidence*/
  ctx[46] !== null && /*labelToEdit*/
  ctx[6] !== /*i*/
  ctx[48] && create_if_block_5(ctx);
  let if_block1 = (
    /*labelToEdit*/
    ctx[6] === /*i*/
    ctx[48] && /*class_or_confidence*/
    ctx[46] !== null && create_if_block_4$1(ctx)
  );
  function click_handler_2() {
    return (
      /*click_handler_2*/
      ctx[26](
        /*class_or_confidence*/
        ctx[46],
        /*i*/
        ctx[48],
        /*token*/
        ctx[45]
      )
    );
  }
  function keydown_handler_2(...args) {
    return (
      /*keydown_handler_2*/
      ctx[27](
        /*class_or_confidence*/
        ctx[46],
        /*i*/
        ctx[48],
        /*token*/
        ctx[45],
        ...args
      )
    );
  }
  function focus_handler_2() {
    return (
      /*focus_handler_2*/
      ctx[28](
        /*i*/
        ctx[48]
      )
    );
  }
  function mouseover_handler_2() {
    return (
      /*mouseover_handler_2*/
      ctx[29](
        /*i*/
        ctx[48]
      )
    );
  }
  let if_block2 = (
    /*class_or_confidence*/
    ctx[46] !== null && create_if_block_3$1(ctx)
  );
  return {
    c() {
      span2 = element("span");
      span1 = element("span");
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      if (if_block0)
        if_block0.c();
      t2 = space();
      if (if_block1)
        if_block1.c();
      t3 = space();
      if (if_block2)
        if_block2.c();
      this.h();
    },
    l(nodes) {
      span2 = claim_element(nodes, "SPAN", { class: true });
      var span2_nodes = children(span2);
      span1 = claim_element(span2_nodes, "SPAN", { role: true, tabindex: true, class: true });
      var span1_nodes = children(span1);
      span0 = claim_element(span1_nodes, "SPAN", { class: true, role: true, tabindex: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(span1_nodes);
      if (if_block0)
        if_block0.l(span1_nodes);
      t2 = claim_space(span1_nodes);
      if (if_block1)
        if_block1.l(span1_nodes);
      span1_nodes.forEach(detach);
      t3 = claim_space(span2_nodes);
      if (if_block2)
        if_block2.l(span2_nodes);
      span2_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "text svelte-1ozsnjl");
      attr(span0, "role", "button");
      attr(span0, "tabindex", "0");
      toggle_class(
        span0,
        "no-label",
        /*class_or_confidence*/
        ctx[46] === null
      );
      attr(span1, "role", "button");
      attr(span1, "tabindex", "0");
      attr(span1, "class", "textspan svelte-1ozsnjl");
      toggle_class(
        span1,
        "no-cat",
        /*class_or_confidence*/
        ctx[46] === null || /*active*/
        ctx[5] && /*active*/
        ctx[5] !== /*class_or_confidence*/
        ctx[46]
      );
      toggle_class(
        span1,
        "hl",
        /*class_or_confidence*/
        ctx[46] !== null
      );
      toggle_class(
        span1,
        "selectable",
        /*selectable*/
        ctx[2]
      );
      set_style(
        span1,
        "background-color",
        /*class_or_confidence*/
        ctx[46] === null || /*active*/
        ctx[5] && /*active*/
        ctx[5] !== /*class_or_confidence*/
        ctx[46] ? "" : (
          /*class_or_confidence*/
          ctx[46] && /*_color_map*/
          ctx[3][
            /*class_or_confidence*/
            ctx[46]
          ] ? (
            /*_color_map*/
            ctx[3][
              /*class_or_confidence*/
              ctx[46]
            ].secondary
          ) : ""
        )
      );
      attr(span2, "class", "text-class_or_confidence-container svelte-1ozsnjl");
    },
    m(target, anchor) {
      insert_hydration(target, span2, anchor);
      append_hydration(span2, span1);
      append_hydration(span1, span0);
      append_hydration(span0, t0);
      append_hydration(span1, t1);
      if (if_block0)
        if_block0.m(span1, null);
      append_hydration(span1, t2);
      if (if_block1)
        if_block1.m(span1, null);
      append_hydration(span2, t3);
      if (if_block2)
        if_block2.m(span2, null);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            span0,
            "keydown",
            /*keydown_handler*/
            ctx[19]
          ),
          listen(span0, "focus", focus_handler_1),
          listen(span0, "mouseover", mouseover_handler_1),
          listen(span0, "click", click_handler),
          listen(span1, "click", click_handler_2),
          listen(span1, "keydown", keydown_handler_2),
          listen(span1, "focus", focus_handler_2),
          listen(span1, "mouseover", mouseover_handler_2)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if ((!current || dirty[0] & /*value*/
      1) && t0_value !== (t0_value = /*line*/
      ctx[49] + ""))
        set_data(t0, t0_value);
      if (!current || dirty[0] & /*value*/
      1) {
        toggle_class(
          span0,
          "no-label",
          /*class_or_confidence*/
          ctx[46] === null
        );
      }
      if (!/*show_legend*/
      ctx[1] && /*class_or_confidence*/
      ctx[46] !== null && /*labelToEdit*/
      ctx[6] !== /*i*/
      ctx[48]) {
        if (if_block0) {
          if_block0.p(ctx, dirty);
        } else {
          if_block0 = create_if_block_5(ctx);
          if_block0.c();
          if_block0.m(span1, t2);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (
        /*labelToEdit*/
        ctx[6] === /*i*/
        ctx[48] && /*class_or_confidence*/
        ctx[46] !== null
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
          if (dirty[0] & /*labelToEdit, value*/
          65) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_4$1(ctx);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(span1, null);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (!current || dirty[0] & /*value, active*/
      33) {
        toggle_class(
          span1,
          "no-cat",
          /*class_or_confidence*/
          ctx[46] === null || /*active*/
          ctx[5] && /*active*/
          ctx[5] !== /*class_or_confidence*/
          ctx[46]
        );
      }
      if (!current || dirty[0] & /*value*/
      1) {
        toggle_class(
          span1,
          "hl",
          /*class_or_confidence*/
          ctx[46] !== null
        );
      }
      if (!current || dirty[0] & /*selectable*/
      4) {
        toggle_class(
          span1,
          "selectable",
          /*selectable*/
          ctx[2]
        );
      }
      if (dirty[0] & /*value, active, _color_map*/
      41) {
        set_style(
          span1,
          "background-color",
          /*class_or_confidence*/
          ctx[46] === null || /*active*/
          ctx[5] && /*active*/
          ctx[5] !== /*class_or_confidence*/
          ctx[46] ? "" : (
            /*class_or_confidence*/
            ctx[46] && /*_color_map*/
            ctx[3][
              /*class_or_confidence*/
              ctx[46]
            ] ? (
              /*_color_map*/
              ctx[3][
                /*class_or_confidence*/
                ctx[46]
              ].secondary
            ) : ""
          )
        );
      }
      if (
        /*class_or_confidence*/
        ctx[46] !== null
      ) {
        if (if_block2) {
          if_block2.p(ctx, dirty);
        } else {
          if_block2 = create_if_block_3$1(ctx);
          if_block2.c();
          if_block2.m(span2, null);
        }
      } else if (if_block2) {
        if_block2.d(1);
        if_block2 = null;
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(span2);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      if (if_block2)
        if_block2.d();
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_5(ctx) {
  let span;
  let t_value = (
    /*class_or_confidence*/
    ctx[46] + ""
  );
  let t;
  let mounted;
  let dispose;
  function click_handler_1() {
    return (
      /*click_handler_1*/
      ctx[23](
        /*i*/
        ctx[48]
      )
    );
  }
  function keydown_handler_1() {
    return (
      /*keydown_handler_1*/
      ctx[24](
        /*i*/
        ctx[48]
      )
    );
  }
  return {
    c() {
      span = element("span");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", {
        id: true,
        class: true,
        role: true,
        tabindex: true
      });
      var span_nodes = children(span);
      t = claim_text(span_nodes, t_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "id", `label-tag-${/*i*/
      ctx[48]}`);
      attr(span, "class", "label svelte-1ozsnjl");
      attr(span, "role", "button");
      attr(span, "tabindex", "0");
      set_style(
        span,
        "background-color",
        /*class_or_confidence*/
        ctx[46] === null || /*active*/
        ctx[5] && /*active*/
        ctx[5] !== /*class_or_confidence*/
        ctx[46] ? "" : (
          /*_color_map*/
          ctx[3][
            /*class_or_confidence*/
            ctx[46]
          ].primary
        )
      );
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
      if (!mounted) {
        dispose = [
          listen(span, "click", click_handler_1),
          listen(span, "keydown", keydown_handler_1)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*value*/
      1 && t_value !== (t_value = /*class_or_confidence*/
      ctx[46] + ""))
        set_data(t, t_value);
      if (dirty[0] & /*value, active, _color_map*/
      41) {
        set_style(
          span,
          "background-color",
          /*class_or_confidence*/
          ctx[46] === null || /*active*/
          ctx[5] && /*active*/
          ctx[5] !== /*class_or_confidence*/
          ctx[46] ? "" : (
            /*_color_map*/
            ctx[3][
              /*class_or_confidence*/
              ctx[46]
            ].primary
          )
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_4$1(ctx) {
  let t;
  let labelinput;
  let updating_value;
  let current;
  function labelinput_value_binding(value) {
    ctx[25](value);
  }
  let labelinput_props = {
    labelToEdit: (
      /*labelToEdit*/
      ctx[6]
    ),
    category: (
      /*class_or_confidence*/
      ctx[46]
    ),
    active: (
      /*active*/
      ctx[5]
    ),
    _color_map: (
      /*_color_map*/
      ctx[3]
    ),
    indexOfLabel: (
      /*i*/
      ctx[48]
    ),
    text: (
      /*token*/
      ctx[45]
    ),
    handleValueChange: (
      /*handleValueChange*/
      ctx[9]
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    labelinput_props.value = /*value*/
    ctx[0];
  }
  labelinput = new LabelInput({ props: labelinput_props });
  binding_callbacks.push(() => bind(labelinput, "value", labelinput_value_binding));
  return {
    c() {
      t = text(" \n									");
      create_component(labelinput.$$.fragment);
    },
    l(nodes) {
      t = claim_text(nodes, " \n									");
      claim_component(labelinput.$$.fragment, nodes);
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
      mount_component(labelinput, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const labelinput_changes = {};
      if (dirty[0] & /*labelToEdit*/
      64)
        labelinput_changes.labelToEdit = /*labelToEdit*/
        ctx2[6];
      if (dirty[0] & /*value*/
      1)
        labelinput_changes.category = /*class_or_confidence*/
        ctx2[46];
      if (dirty[0] & /*active*/
      32)
        labelinput_changes.active = /*active*/
        ctx2[5];
      if (dirty[0] & /*_color_map*/
      8)
        labelinput_changes._color_map = /*_color_map*/
        ctx2[3];
      if (dirty[0] & /*value*/
      1)
        labelinput_changes.text = /*token*/
        ctx2[45];
      if (!updating_value && dirty[0] & /*value*/
      1) {
        updating_value = true;
        labelinput_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      labelinput.$set(labelinput_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(labelinput.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(labelinput.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(labelinput, detaching);
    }
  };
}
function create_if_block_3$1(ctx) {
  let span;
  let textContent = "×";
  let mounted;
  let dispose;
  function click_handler_3() {
    return (
      /*click_handler_3*/
      ctx[30](
        /*i*/
        ctx[48]
      )
    );
  }
  function keydown_handler_3(...args) {
    return (
      /*keydown_handler_3*/
      ctx[31](
        /*i*/
        ctx[48],
        ...args
      )
    );
  }
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", {
        class: true,
        role: true,
        "aria-roledescription": true,
        tabindex: true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(span) !== "svelte-1fuy4vv")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "label-clear-button svelte-1ozsnjl");
      attr(span, "role", "button");
      attr(span, "aria-roledescription", "Remove label from text");
      attr(span, "tabindex", "0");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      if (!mounted) {
        dispose = [
          listen(span, "click", click_handler_3),
          listen(span, "keydown", keydown_handler_3)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_1$1(ctx) {
  let br;
  return {
    c() {
      br = element("br");
    },
    l(nodes) {
      br = claim_element(nodes, "BR", {});
    },
    m(target, anchor) {
      insert_hydration(target, br, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(br);
      }
    }
  };
}
function create_each_block_1(ctx) {
  let show_if_1 = (
    /*line*/
    ctx[49].trim() !== ""
  );
  let t;
  let show_if = (
    /*j*/
    ctx[51] < splitTextByNewline(
      /*token*/
      ctx[45]
    ).length - 1
  );
  let if_block1_anchor;
  let current;
  let if_block0 = show_if_1 && create_if_block_2$1(ctx);
  let if_block1 = show_if && create_if_block_1$1();
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*value*/
      1)
        show_if_1 = /*line*/
        ctx2[49].trim() !== "";
      if (show_if_1) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*value*/
          1) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2$1(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t.parentNode, t);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (dirty[0] & /*value*/
      1)
        show_if = /*j*/
        ctx2[51] < splitTextByNewline(
          /*token*/
          ctx2[45]
        ).length - 1;
      if (show_if) {
        if (if_block1)
          ;
        else {
          if_block1 = create_if_block_1$1();
          if_block1.c();
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_each_block(ctx) {
  let each_1_anchor;
  let current;
  let each_value_1 = ensure_array_like(splitTextByNewline(
    /*token*/
    ctx[45]
  ));
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      each_1_anchor = empty();
    },
    l(nodes) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(nodes);
      }
      each_1_anchor = empty();
    },
    m(target, anchor) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(target, anchor);
        }
      }
      insert_hydration(target, each_1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*value, removeHighlightedText, active, selectable, _color_map, handleSelect, labelToEdit, handleKeydownSelection, activeElementIndex, handleValueChange, show_legend*/
      13183) {
        each_value_1 = ensure_array_like(splitTextByNewline(
          /*token*/
          ctx2[45]
        ));
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_1(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(each_1_anchor.parentNode, each_1_anchor);
          }
        }
        group_outros();
        for (i = each_value_1.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_1.length; i += 1) {
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
        detach(each_1_anchor);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let div;
  let current_block_type_index;
  let if_block;
  let current;
  const if_block_creators = [create_if_block$1, create_else_block$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*mode*/
      ctx2[7] === "categories"
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      div = element("div");
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "container svelte-1ozsnjl");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if_blocks[current_block_type_index].m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
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
        if_block.m(div, null);
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
        detach(div);
      }
      if_blocks[current_block_type_index].d();
    }
  };
}
function splitTextByNewline(text2) {
  return text2.split("\n");
}
function instance$1($$self, $$props, $$invalidate) {
  const browser = typeof document !== "undefined";
  let { value = [] } = $$props;
  let { show_legend = false } = $$props;
  let { color_map = {} } = $$props;
  let { selectable = false } = $$props;
  let activeElementIndex = -1;
  let ctx;
  let _color_map = {};
  let active = "";
  let selection;
  let labelToEdit = -1;
  onMount(() => {
    const mouseUpHandler = () => {
      selection = window.getSelection();
      handleSelectionComplete();
      window.removeEventListener("mouseup", mouseUpHandler);
    };
    window.addEventListener("mousedown", () => {
      window.addEventListener("mouseup", mouseUpHandler);
    });
  });
  async function handleTextSelected(startIndex, endIndex) {
    var _a;
    if ((selection == null ? void 0 : selection.toString()) && activeElementIndex !== -1 && value[activeElementIndex].token.toString().includes(selection.toString())) {
      const tempFlag = Symbol();
      const str = value[activeElementIndex].token;
      const [before, selected, after] = [
        str.substring(0, startIndex),
        str.substring(startIndex, endIndex),
        str.substring(endIndex)
      ];
      let tempValue = [
        ...value.slice(0, activeElementIndex),
        { token: before, class_or_confidence: null },
        {
          token: selected,
          class_or_confidence: mode === "scores" ? 1 : "label",
          flag: tempFlag
        },
        // add a temp flag to the new highlighted text element
        { token: after, class_or_confidence: null },
        ...value.slice(activeElementIndex + 1)
      ];
      $$invalidate(6, labelToEdit = tempValue.findIndex(({ flag }) => flag === tempFlag));
      tempValue = tempValue.filter((item) => item.token.trim() !== "");
      $$invalidate(0, value = tempValue.map(({ flag, ...rest }) => rest));
      handleValueChange();
      (_a = document.getElementById(`label-input-${labelToEdit}`)) == null ? void 0 : _a.focus();
    }
  }
  const dispatch = createEventDispatcher();
  function removeHighlightedText(index) {
    var _a;
    if (!value || index < 0 || index >= value.length)
      return;
    $$invalidate(0, value[index].class_or_confidence = null, value);
    $$invalidate(0, value = merge_elements(value));
    handleValueChange();
    (_a = window.getSelection()) == null ? void 0 : _a.empty();
  }
  function handleValueChange() {
    dispatch("change", value);
    $$invalidate(6, labelToEdit = -1);
    if (show_legend) {
      $$invalidate(14, color_map = {});
      $$invalidate(3, _color_map = {});
    }
  }
  let mode;
  function handle_mouseover(label) {
    $$invalidate(5, active = label);
  }
  function handle_mouseout() {
    $$invalidate(5, active = "");
  }
  async function handleKeydownSelection(event) {
    selection = window.getSelection();
    if (event.key === "Enter") {
      handleSelectionComplete();
    }
  }
  function handleSelectionComplete() {
    if (selection && (selection == null ? void 0 : selection.toString().trim()) !== "") {
      const textBeginningIndex = selection.getRangeAt(0).startOffset;
      const textEndIndex = selection.getRangeAt(0).endOffset;
      handleTextSelected(textBeginningIndex, textEndIndex);
    }
  }
  function handleSelect(i, text2, class_or_confidence) {
    dispatch("select", {
      index: i,
      value: [text2, class_or_confidence]
    });
  }
  const mouseover_handler = (class_or_confidence) => handle_mouseover(class_or_confidence);
  const focus_handler = (class_or_confidence) => handle_mouseover(class_or_confidence);
  const mouseout_handler = () => handle_mouseout();
  const blur_handler = () => handle_mouseout();
  const keydown_handler = (e) => handleKeydownSelection(e);
  const focus_handler_1 = (i) => $$invalidate(4, activeElementIndex = i);
  const mouseover_handler_1 = (i) => $$invalidate(4, activeElementIndex = i);
  const click_handler = (i) => $$invalidate(6, labelToEdit = i);
  const click_handler_1 = (i) => $$invalidate(6, labelToEdit = i);
  const keydown_handler_1 = (i) => $$invalidate(6, labelToEdit = i);
  function labelinput_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  const click_handler_2 = (class_or_confidence, i, token) => {
    if (class_or_confidence !== null) {
      handleSelect(i, token, class_or_confidence);
    }
  };
  const keydown_handler_2 = (class_or_confidence, i, token, e) => {
    if (class_or_confidence !== null) {
      $$invalidate(6, labelToEdit = i);
      handleSelect(i, token, class_or_confidence);
    } else {
      handleKeydownSelection(e);
    }
  };
  const focus_handler_2 = (i) => $$invalidate(4, activeElementIndex = i);
  const mouseover_handler_2 = (i) => $$invalidate(4, activeElementIndex = i);
  const click_handler_3 = (i) => removeHighlightedText(i);
  const keydown_handler_3 = (i, event) => {
    if (event.key === "Enter") {
      removeHighlightedText(i);
    }
  };
  function labelinput_value_binding_1(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  const mouseover_handler_3 = (i) => $$invalidate(4, activeElementIndex = i);
  const focus_handler_3 = (i) => $$invalidate(4, activeElementIndex = i);
  const click_handler_4 = (i) => $$invalidate(6, labelToEdit = i);
  const keydown_handler_4 = (i, e) => {
    if (e.key === "Enter") {
      $$invalidate(6, labelToEdit = i);
    }
  };
  const click_handler_5 = (i) => removeHighlightedText(i);
  const keydown_handler_5 = (i, event) => {
    if (event.key === "Enter") {
      removeHighlightedText(i);
    }
  };
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("show_legend" in $$props2)
      $$invalidate(1, show_legend = $$props2.show_legend);
    if ("color_map" in $$props2)
      $$invalidate(14, color_map = $$props2.color_map);
    if ("selectable" in $$props2)
      $$invalidate(2, selectable = $$props2.selectable);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*color_map, value, _color_map*/
    16393) {
      {
        if (!color_map) {
          $$invalidate(14, color_map = {});
        }
        if (value.length > 0) {
          for (let entry of value) {
            if (entry.class_or_confidence !== null) {
              if (typeof entry.class_or_confidence === "string") {
                $$invalidate(7, mode = "categories");
                if (!(entry.class_or_confidence in color_map)) {
                  let color = get_next_color(Object.keys(color_map).length);
                  $$invalidate(14, color_map[entry.class_or_confidence] = color, color_map);
                }
              } else {
                $$invalidate(7, mode = "scores");
              }
            }
          }
        }
        correct_color_map(color_map, _color_map, browser, ctx);
      }
    }
  };
  return [
    value,
    show_legend,
    selectable,
    _color_map,
    activeElementIndex,
    active,
    labelToEdit,
    mode,
    removeHighlightedText,
    handleValueChange,
    handle_mouseover,
    handle_mouseout,
    handleKeydownSelection,
    handleSelect,
    color_map,
    mouseover_handler,
    focus_handler,
    mouseout_handler,
    blur_handler,
    keydown_handler,
    focus_handler_1,
    mouseover_handler_1,
    click_handler,
    click_handler_1,
    keydown_handler_1,
    labelinput_value_binding,
    click_handler_2,
    keydown_handler_2,
    focus_handler_2,
    mouseover_handler_2,
    click_handler_3,
    keydown_handler_3,
    labelinput_value_binding_1,
    mouseover_handler_3,
    focus_handler_3,
    click_handler_4,
    keydown_handler_4,
    click_handler_5,
    keydown_handler_5
  ];
}
class InteractiveHighlightedtext extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 0,
        show_legend: 1,
        color_map: 14,
        selectable: 2
      },
      null,
      [-1, -1]
    );
  }
}
const InteractiveHighlightedText = InteractiveHighlightedtext;
function create_else_block_1(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      variant: (
        /*interactive*/
        ctx[13] ? "dashed" : "solid"
      ),
      test_id: "highlighted-text",
      visible: (
        /*visible*/
        ctx[5]
      ),
      elem_id: (
        /*elem_id*/
        ctx[3]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[4]
      ),
      padding: false,
      container: (
        /*container*/
        ctx[9]
      ),
      scale: (
        /*scale*/
        ctx[10]
      ),
      min_width: (
        /*min_width*/
        ctx[11]
      ),
      $$slots: { default: [create_default_slot_2] },
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
    p(ctx2, dirty) {
      const block_changes = {};
      if (dirty & /*interactive*/
      8192)
        block_changes.variant = /*interactive*/
        ctx2[13] ? "dashed" : "solid";
      if (dirty & /*visible*/
      32)
        block_changes.visible = /*visible*/
        ctx2[5];
      if (dirty & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
      if (dirty & /*container*/
      512)
        block_changes.container = /*container*/
        ctx2[9];
      if (dirty & /*scale*/
      1024)
        block_changes.scale = /*scale*/
        ctx2[10];
      if (dirty & /*min_width*/
      2048)
        block_changes.min_width = /*min_width*/
        ctx2[11];
      if (dirty & /*$$scope, _selectable, show_legend, color_map, value, gradio, label, container, loading_status*/
      4215623) {
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
function create_if_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      variant: "solid",
      test_id: "highlighted-text",
      visible: (
        /*visible*/
        ctx[5]
      ),
      elem_id: (
        /*elem_id*/
        ctx[3]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[4]
      ),
      padding: false,
      container: (
        /*container*/
        ctx[9]
      ),
      scale: (
        /*scale*/
        ctx[10]
      ),
      min_width: (
        /*min_width*/
        ctx[11]
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
    p(ctx2, dirty) {
      const block_changes = {};
      if (dirty & /*visible*/
      32)
        block_changes.visible = /*visible*/
        ctx2[5];
      if (dirty & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
      if (dirty & /*container*/
      512)
        block_changes.container = /*container*/
        ctx2[9];
      if (dirty & /*scale*/
      1024)
        block_changes.scale = /*scale*/
        ctx2[10];
      if (dirty & /*min_width*/
      2048)
        block_changes.min_width = /*min_width*/
        ctx2[11];
      if (dirty & /*$$scope, _selectable, value, show_legend, show_inline_category, color_map, gradio, label, container, loading_status*/
      4215751) {
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
function create_if_block_4(ctx) {
  let blocklabel;
  let current;
  blocklabel = new BlockLabel({
    props: {
      Icon: TextHighlight,
      label: (
        /*label*/
        ctx[8]
      ),
      float: false,
      disable: (
        /*container*/
        ctx[9] === false
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
      if (dirty & /*label*/
      256)
        blocklabel_changes.label = /*label*/
        ctx2[8];
      if (dirty & /*container*/
      512)
        blocklabel_changes.disable = /*container*/
        ctx2[9] === false;
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
function create_else_block_2(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      $$slots: { default: [create_default_slot_3] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(empty_1.$$.fragment);
    },
    l(nodes) {
      claim_component(empty_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(empty_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const empty_1_changes = {};
      if (dirty & /*$$scope*/
      4194304) {
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
      destroy_component(empty_1, detaching);
    }
  };
}
function create_if_block_3(ctx) {
  let interactivehighlightedtext;
  let updating_value;
  let current;
  function interactivehighlightedtext_value_binding(value) {
    ctx[20](value);
  }
  let interactivehighlightedtext_props = {
    selectable: (
      /*_selectable*/
      ctx[12]
    ),
    show_legend: (
      /*show_legend*/
      ctx[6]
    ),
    color_map: (
      /*color_map*/
      ctx[1]
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    interactivehighlightedtext_props.value = /*value*/
    ctx[0];
  }
  interactivehighlightedtext = new InteractiveHighlightedText({ props: interactivehighlightedtext_props });
  binding_callbacks.push(() => bind(interactivehighlightedtext, "value", interactivehighlightedtext_value_binding));
  interactivehighlightedtext.$on(
    "change",
    /*change_handler*/
    ctx[21]
  );
  return {
    c() {
      create_component(interactivehighlightedtext.$$.fragment);
    },
    l(nodes) {
      claim_component(interactivehighlightedtext.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(interactivehighlightedtext, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const interactivehighlightedtext_changes = {};
      if (dirty & /*_selectable*/
      4096)
        interactivehighlightedtext_changes.selectable = /*_selectable*/
        ctx2[12];
      if (dirty & /*show_legend*/
      64)
        interactivehighlightedtext_changes.show_legend = /*show_legend*/
        ctx2[6];
      if (dirty & /*color_map*/
      2)
        interactivehighlightedtext_changes.color_map = /*color_map*/
        ctx2[1];
      if (!updating_value && dirty & /*value*/
      1) {
        updating_value = true;
        interactivehighlightedtext_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      interactivehighlightedtext.$set(interactivehighlightedtext_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(interactivehighlightedtext.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(interactivehighlightedtext.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(interactivehighlightedtext, detaching);
    }
  };
}
function create_default_slot_3(ctx) {
  let texthighlight;
  let current;
  texthighlight = new TextHighlight({});
  return {
    c() {
      create_component(texthighlight.$$.fragment);
    },
    l(nodes) {
      claim_component(texthighlight.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(texthighlight, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(texthighlight.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(texthighlight.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(texthighlight, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let statustracker;
  let t0;
  let t1;
  let current_block_type_index;
  let if_block1;
  let if_block1_anchor;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[2].autoscroll
    ) },
    /*loading_status*/
    ctx[14],
    { i18n: (
      /*gradio*/
      ctx[2].i18n
    ) }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler_1*/
    ctx[19]
  );
  let if_block0 = (
    /*label*/
    ctx[8] && create_if_block_4(ctx)
  );
  const if_block_creators = [create_if_block_3, create_else_block_2];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_2(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      if (if_block0)
        if_block0.c();
      t1 = space();
      if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      if (if_block0)
        if_block0.l(nodes);
      t1 = claim_space(nodes);
      if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t1, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      16388 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        4 && { autoscroll: (
          /*gradio*/
          ctx2[2].autoscroll
        ) },
        dirty & /*loading_status*/
        16384 && get_spread_object(
          /*loading_status*/
          ctx2[14]
        ),
        dirty & /*gradio*/
        4 && { i18n: (
          /*gradio*/
          ctx2[2].i18n
        ) }
      ]) : {};
      statustracker.$set(statustracker_changes);
      if (
        /*label*/
        ctx2[8]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*label*/
          256) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t1.parentNode, t1);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_2(ctx2);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(if_block1_anchor);
      }
      destroy_component(statustracker, detaching);
      if (if_block0)
        if_block0.d(detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let blocklabel;
  let current;
  blocklabel = new BlockLabel({
    props: {
      Icon: TextHighlight,
      label: (
        /*label*/
        ctx[8]
      ),
      float: false,
      disable: (
        /*container*/
        ctx[9] === false
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
      if (dirty & /*label*/
      256)
        blocklabel_changes.label = /*label*/
        ctx2[8];
      if (dirty & /*container*/
      512)
        blocklabel_changes.disable = /*container*/
        ctx2[9] === false;
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
function create_else_block(ctx) {
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
      create_component(empty_1.$$.fragment);
    },
    l(nodes) {
      claim_component(empty_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(empty_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const empty_1_changes = {};
      if (dirty & /*$$scope*/
      4194304) {
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
      destroy_component(empty_1, detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let statichighlightedtext;
  let current;
  statichighlightedtext = new StaticHighlightedText({
    props: {
      selectable: (
        /*_selectable*/
        ctx[12]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      show_legend: (
        /*show_legend*/
        ctx[6]
      ),
      show_inline_category: (
        /*show_inline_category*/
        ctx[7]
      ),
      color_map: (
        /*color_map*/
        ctx[1]
      )
    }
  });
  statichighlightedtext.$on(
    "select",
    /*select_handler*/
    ctx[18]
  );
  return {
    c() {
      create_component(statichighlightedtext.$$.fragment);
    },
    l(nodes) {
      claim_component(statichighlightedtext.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statichighlightedtext, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statichighlightedtext_changes = {};
      if (dirty & /*_selectable*/
      4096)
        statichighlightedtext_changes.selectable = /*_selectable*/
        ctx2[12];
      if (dirty & /*value*/
      1)
        statichighlightedtext_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*show_legend*/
      64)
        statichighlightedtext_changes.show_legend = /*show_legend*/
        ctx2[6];
      if (dirty & /*show_inline_category*/
      128)
        statichighlightedtext_changes.show_inline_category = /*show_inline_category*/
        ctx2[7];
      if (dirty & /*color_map*/
      2)
        statichighlightedtext_changes.color_map = /*color_map*/
        ctx2[1];
      statichighlightedtext.$set(statichighlightedtext_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statichighlightedtext.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statichighlightedtext.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(statichighlightedtext, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let texthighlight;
  let current;
  texthighlight = new TextHighlight({});
  return {
    c() {
      create_component(texthighlight.$$.fragment);
    },
    l(nodes) {
      claim_component(texthighlight.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(texthighlight, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(texthighlight.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(texthighlight.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(texthighlight, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let t1;
  let current_block_type_index;
  let if_block1;
  let if_block1_anchor;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[2].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[2].i18n
    ) },
    /*loading_status*/
    ctx[14]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[17]
  );
  let if_block0 = (
    /*label*/
    ctx[8] && create_if_block_2(ctx)
  );
  const if_block_creators = [create_if_block_1, create_else_block];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      if (if_block0)
        if_block0.c();
      t1 = space();
      if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      if (if_block0)
        if_block0.l(nodes);
      t1 = claim_space(nodes);
      if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t1, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      16388 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        4 && { autoscroll: (
          /*gradio*/
          ctx2[2].autoscroll
        ) },
        dirty & /*gradio*/
        4 && { i18n: (
          /*gradio*/
          ctx2[2].i18n
        ) },
        dirty & /*loading_status*/
        16384 && get_spread_object(
          /*loading_status*/
          ctx2[14]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      if (
        /*label*/
        ctx2[8]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*label*/
          256) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t1.parentNode, t1);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(if_block1_anchor);
      }
      destroy_component(statustracker, detaching);
      if (if_block0)
        if_block0.d(detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function create_fragment(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*interactive*/
    ctx2[13])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
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
      current_block_type_index = select_block_type(ctx2);
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
function instance($$self, $$props, $$invalidate) {
  let { gradio } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { show_legend } = $$props;
  let { show_inline_category } = $$props;
  let { color_map = {} } = $$props;
  let { label = gradio.i18n("highlighted_text.highlighted_text") } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { _selectable = false } = $$props;
  let { combine_adjacent = false } = $$props;
  let { interactive } = $$props;
  let { loading_status } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const select_handler = ({ detail }) => gradio.dispatch("select", detail);
  const clear_status_handler_1 = () => gradio.dispatch("clear_status", loading_status);
  function interactivehighlightedtext_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value), $$invalidate(15, combine_adjacent);
  }
  const change_handler = () => gradio.dispatch("change");
  $$self.$$set = ($$props2) => {
    if ("gradio" in $$props2)
      $$invalidate(2, gradio = $$props2.gradio);
    if ("elem_id" in $$props2)
      $$invalidate(3, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(4, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(5, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("show_legend" in $$props2)
      $$invalidate(6, show_legend = $$props2.show_legend);
    if ("show_inline_category" in $$props2)
      $$invalidate(7, show_inline_category = $$props2.show_inline_category);
    if ("color_map" in $$props2)
      $$invalidate(1, color_map = $$props2.color_map);
    if ("label" in $$props2)
      $$invalidate(8, label = $$props2.label);
    if ("container" in $$props2)
      $$invalidate(9, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(10, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(11, min_width = $$props2.min_width);
    if ("_selectable" in $$props2)
      $$invalidate(12, _selectable = $$props2._selectable);
    if ("combine_adjacent" in $$props2)
      $$invalidate(15, combine_adjacent = $$props2.combine_adjacent);
    if ("interactive" in $$props2)
      $$invalidate(13, interactive = $$props2.interactive);
    if ("loading_status" in $$props2)
      $$invalidate(14, loading_status = $$props2.loading_status);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*color_map*/
    2) {
      if (!color_map && Object.keys(color_map).length) {
        $$invalidate(1, color_map);
      }
    }
    if ($$self.$$.dirty & /*value, combine_adjacent*/
    32769) {
      if (value && combine_adjacent) {
        $$invalidate(0, value = merge_elements(value));
      }
    }
    if ($$self.$$.dirty & /*value, old_value, gradio*/
    65541) {
      {
        if (value !== old_value) {
          $$invalidate(16, old_value = value);
          gradio.dispatch("change");
        }
      }
    }
  };
  return [
    value,
    color_map,
    gradio,
    elem_id,
    elem_classes,
    visible,
    show_legend,
    show_inline_category,
    label,
    container,
    scale,
    min_width,
    _selectable,
    interactive,
    loading_status,
    combine_adjacent,
    old_value,
    clear_status_handler,
    select_handler,
    clear_status_handler_1,
    interactivehighlightedtext_value_binding,
    change_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      gradio: 2,
      elem_id: 3,
      elem_classes: 4,
      visible: 5,
      value: 0,
      show_legend: 6,
      show_inline_category: 7,
      color_map: 1,
      label: 8,
      container: 9,
      scale: 10,
      min_width: 11,
      _selectable: 12,
      combine_adjacent: 15,
      interactive: 13,
      loading_status: 14
    });
  }
}
export {
  InteractiveHighlightedText as BaseInteractiveHighlightedText,
  StaticHighlightedText as BaseStaticHighlightedText,
  Index as default
};
