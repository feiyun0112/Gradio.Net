import { SvelteComponent, init, safe_not_equal, create_component, space, element, empty, claim_component, claim_space, claim_element, children, detach, attr, set_style, mount_component, insert_hydration, listen, transition_in, group_outros, transition_out, check_outros, destroy_component, run_all, createEventDispatcher, onMount, afterUpdate, globals, text, claim_text, set_data, ensure_array_like, append_hydration, set_input_value, action_destroyer, destroy_each, toggle_class, tick, bubble, binding_callbacks, flush, assign, bind, get_spread_update, get_spread_object, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { t as tinycolor, E as Eyedropper } from "./tinycolor.IwkldZk1.js";
import { f as BlockTitle, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { default as default2 } from "./Example.D2Mw89Ip.js";
function click_outside(node, callback) {
  const handle_click = (event) => {
    if (node && !node.contains(event.target) && !event.defaultPrevented) {
      callback(event);
    }
  };
  document.addEventListener("mousedown", handle_click, true);
  return {
    destroy() {
      document.removeEventListener("mousedown", handle_click, true);
    }
  };
}
function hsva_to_rgba(hsva) {
  const saturation = hsva.s;
  const value = hsva.v;
  let chroma = saturation * value;
  const hue_by_60 = hsva.h / 60;
  let x = chroma * (1 - Math.abs(hue_by_60 % 2 - 1));
  const m = value - chroma;
  chroma = chroma + m;
  x = x + m;
  const index = Math.floor(hue_by_60) % 6;
  const red = [chroma, x, m, m, x, chroma][index];
  const green = [x, chroma, chroma, x, m, m][index];
  const blue = [m, m, x, chroma, chroma, x][index];
  return `rgba(${red * 255}, ${green * 255}, ${blue * 255}, ${hsva.a})`;
}
function format_color(color, mode) {
  if (mode === "hex") {
    return tinycolor(color).toHexString();
  } else if (mode === "rgb") {
    return tinycolor(color).toRgbString();
  }
  return tinycolor(color).toHslString();
}
const { window: window_1 } = globals;
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[7] = list[i][0];
  child_ctx[1] = list[i][1];
  return child_ctx;
}
function create_default_slot$1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*label*/
        ctx[7]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*label*/
        ctx[7]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*label*/
      128)
        set_data(
          t,
          /*label*/
          ctx2[7]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_if_block(ctx) {
  let div8;
  let div1;
  let div0;
  let style_transform = `translate(${/*sl_marker_pos*/
  ctx[12][0]}px,${/*sl_marker_pos*/
  ctx[12][1]}px)`;
  let t0;
  let div3;
  let div2;
  let style_transform_1 = `translateX(${/*hue_marker_pos*/
  ctx[14]}px)`;
  let t1;
  let div7;
  let button0;
  let t2;
  let div6;
  let div4;
  let input;
  let t3;
  let button1;
  let t4;
  let div5;
  let current;
  let mounted;
  let dispose;
  let if_block = (
    /*eyedropper_supported*/
    ctx[9] && create_if_block_1()
  );
  let each_value = ensure_array_like(
    /*modes*/
    ctx[21]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  return {
    c() {
      div8 = element("div");
      div1 = element("div");
      div0 = element("div");
      t0 = space();
      div3 = element("div");
      div2 = element("div");
      t1 = space();
      div7 = element("div");
      button0 = element("button");
      t2 = space();
      div6 = element("div");
      div4 = element("div");
      input = element("input");
      t3 = space();
      button1 = element("button");
      if (if_block)
        if_block.c();
      t4 = space();
      div5 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div8 = claim_element(nodes, "DIV", { class: true });
      var div8_nodes = children(div8);
      div1 = claim_element(div8_nodes, "DIV", { class: true, style: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      children(div0).forEach(detach);
      div1_nodes.forEach(detach);
      t0 = claim_space(div8_nodes);
      div3 = claim_element(div8_nodes, "DIV", { class: true });
      var div3_nodes = children(div3);
      div2 = claim_element(div3_nodes, "DIV", { class: true });
      children(div2).forEach(detach);
      div3_nodes.forEach(detach);
      t1 = claim_space(div8_nodes);
      div7 = claim_element(div8_nodes, "DIV", { class: true });
      var div7_nodes = children(div7);
      button0 = claim_element(div7_nodes, "BUTTON", { class: true });
      children(button0).forEach(detach);
      t2 = claim_space(div7_nodes);
      div6 = claim_element(div7_nodes, "DIV", {});
      var div6_nodes = children(div6);
      div4 = claim_element(div6_nodes, "DIV", { class: true });
      var div4_nodes = children(div4);
      input = claim_element(div4_nodes, "INPUT", { type: true, class: true });
      t3 = claim_space(div4_nodes);
      button1 = claim_element(div4_nodes, "BUTTON", { class: true });
      var button1_nodes = children(button1);
      if (if_block)
        if_block.l(button1_nodes);
      button1_nodes.forEach(detach);
      div4_nodes.forEach(detach);
      t4 = claim_space(div6_nodes);
      div5 = claim_element(div6_nodes, "DIV", { class: true });
      var div5_nodes = children(div5);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div5_nodes);
      }
      div5_nodes.forEach(detach);
      div6_nodes.forEach(detach);
      div7_nodes.forEach(detach);
      div8_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "marker svelte-1oxhzww");
      set_style(div0, "transform", style_transform);
      set_style(
        div0,
        "background",
        /*value*/
        ctx[1]
      );
      attr(div1, "class", "color-gradient svelte-1oxhzww");
      set_style(
        div1,
        "--hue",
        /*hue*/
        ctx[13]
      );
      attr(div2, "class", "marker svelte-1oxhzww");
      set_style(div2, "background", "hsl(" + /*hue*/
      ctx[13] + ", 100%, 50%)");
      set_style(div2, "transform", style_transform_1);
      attr(div3, "class", "hue-slider svelte-1oxhzww");
      attr(button0, "class", "swatch svelte-1oxhzww");
      set_style(
        button0,
        "background",
        /*value*/
        ctx[1]
      );
      attr(input, "type", "text");
      attr(input, "class", "svelte-1oxhzww");
      attr(button1, "class", "eyedropper svelte-1oxhzww");
      attr(div4, "class", "input-wrap svelte-1oxhzww");
      attr(div5, "class", "buttons svelte-1oxhzww");
      attr(div7, "class", "input svelte-1oxhzww");
      attr(div8, "class", "color-picker svelte-1oxhzww");
    },
    m(target, anchor) {
      insert_hydration(target, div8, anchor);
      append_hydration(div8, div1);
      append_hydration(div1, div0);
      ctx[28](div1);
      append_hydration(div8, t0);
      append_hydration(div8, div3);
      append_hydration(div3, div2);
      ctx[29](div3);
      append_hydration(div8, t1);
      append_hydration(div8, div7);
      append_hydration(div7, button0);
      append_hydration(div7, t2);
      append_hydration(div7, div6);
      append_hydration(div6, div4);
      append_hydration(div4, input);
      set_input_value(
        input,
        /*color_string*/
        ctx[8]
      );
      append_hydration(div4, t3);
      append_hydration(div4, button1);
      if (if_block)
        if_block.m(button1, null);
      append_hydration(div6, t4);
      append_hydration(div6, div5);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div5, null);
        }
      }
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            div1,
            "mousedown",
            /*handle_sl_down*/
            ctx[16]
          ),
          listen(
            div3,
            "mousedown",
            /*handle_hue_down*/
            ctx[15]
          ),
          listen(
            button0,
            "click",
            /*handle_click*/
            ctx[23]
          ),
          listen(
            input,
            "input",
            /*input_input_handler*/
            ctx[30]
          ),
          listen(
            input,
            "change",
            /*change_handler*/
            ctx[31]
          ),
          listen(
            button1,
            "click",
            /*request_eyedropper*/
            ctx[20]
          ),
          listen(
            div8,
            "focus",
            /*focus_handler*/
            ctx[25]
          ),
          listen(
            div8,
            "blur",
            /*blur_handler*/
            ctx[26]
          ),
          action_destroyer(click_outside.call(
            null,
            div8,
            /*handle_click_outside*/
            ctx[22]
          ))
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*sl_marker_pos*/
      4096 && style_transform !== (style_transform = `translate(${/*sl_marker_pos*/
      ctx2[12][0]}px,${/*sl_marker_pos*/
      ctx2[12][1]}px)`)) {
        set_style(div0, "transform", style_transform);
      }
      if (dirty[0] & /*value*/
      2) {
        set_style(
          div0,
          "background",
          /*value*/
          ctx2[1]
        );
      }
      if (!current || dirty[0] & /*hue*/
      8192) {
        set_style(
          div1,
          "--hue",
          /*hue*/
          ctx2[13]
        );
      }
      if (dirty[0] & /*hue*/
      8192) {
        set_style(div2, "background", "hsl(" + /*hue*/
        ctx2[13] + ", 100%, 50%)");
      }
      if (dirty[0] & /*hue_marker_pos*/
      16384 && style_transform_1 !== (style_transform_1 = `translateX(${/*hue_marker_pos*/
      ctx2[14]}px)`)) {
        set_style(div2, "transform", style_transform_1);
      }
      if (dirty[0] & /*value*/
      2) {
        set_style(
          button0,
          "background",
          /*value*/
          ctx2[1]
        );
      }
      if (dirty[0] & /*color_string*/
      256 && input.value !== /*color_string*/
      ctx2[8]) {
        set_input_value(
          input,
          /*color_string*/
          ctx2[8]
        );
      }
      if (
        /*eyedropper_supported*/
        ctx2[9]
      ) {
        if (if_block) {
          if (dirty[0] & /*eyedropper_supported*/
          512) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_1();
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(button1, null);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      if (dirty[0] & /*current_mode, modes*/
      2097153) {
        each_value = ensure_array_like(
          /*modes*/
          ctx2[21]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div5, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
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
        detach(div8);
      }
      ctx[28](null);
      ctx[29](null);
      if (if_block)
        if_block.d();
      destroy_each(each_blocks, detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_1(ctx) {
  let eyedropper;
  let current;
  eyedropper = new Eyedropper({});
  return {
    c() {
      create_component(eyedropper.$$.fragment);
    },
    l(nodes) {
      claim_component(eyedropper.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(eyedropper, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(eyedropper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(eyedropper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(eyedropper, detaching);
    }
  };
}
function create_each_block(ctx) {
  let button;
  let t_value = (
    /*label*/
    ctx[7] + ""
  );
  let t;
  let mounted;
  let dispose;
  function click_handler_1() {
    return (
      /*click_handler_1*/
      ctx[32](
        /*value*/
        ctx[1]
      )
    );
  }
  return {
    c() {
      button = element("button");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      t = claim_text(button_nodes, t_value);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "button svelte-1oxhzww");
      toggle_class(
        button,
        "active",
        /*current_mode*/
        ctx[0] === /*value*/
        ctx[1]
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t);
      if (!mounted) {
        dispose = listen(button, "click", click_handler_1);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*current_mode, modes*/
      2097153) {
        toggle_class(
          button,
          "active",
          /*current_mode*/
          ctx[0] === /*value*/
          ctx[1]
        );
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
function create_fragment$1(ctx) {
  let blocktitle;
  let t0;
  let button;
  let t1;
  let if_block_anchor;
  let current;
  let mounted;
  let dispose;
  blocktitle = new BlockTitle({
    props: {
      root: (
        /*root*/
        ctx[6]
      ),
      show_label: (
        /*show_label*/
        ctx[5]
      ),
      info: (
        /*info*/
        ctx[3]
      ),
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  let if_block = (
    /*dialog_open*/
    ctx[2] && create_if_block(ctx)
  );
  return {
    c() {
      create_component(blocktitle.$$.fragment);
      t0 = space();
      button = element("button");
      t1 = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      claim_component(blocktitle.$$.fragment, nodes);
      t0 = claim_space(nodes);
      button = claim_element(nodes, "BUTTON", { class: true });
      children(button).forEach(detach);
      t1 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(button, "class", "dialog-button svelte-1oxhzww");
      button.disabled = /*disabled*/
      ctx[4];
      set_style(
        button,
        "background",
        /*value*/
        ctx[1]
      );
    },
    m(target, anchor) {
      mount_component(blocktitle, target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, button, anchor);
      insert_hydration(target, t1, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            window_1,
            "mousemove",
            /*handle_move*/
            ctx[17]
          ),
          listen(
            window_1,
            "mouseup",
            /*handle_end*/
            ctx[18]
          ),
          listen(
            button,
            "click",
            /*click_handler*/
            ctx[27]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const blocktitle_changes = {};
      if (dirty[0] & /*root*/
      64)
        blocktitle_changes.root = /*root*/
        ctx2[6];
      if (dirty[0] & /*show_label*/
      32)
        blocktitle_changes.show_label = /*show_label*/
        ctx2[5];
      if (dirty[0] & /*info*/
      8)
        blocktitle_changes.info = /*info*/
        ctx2[3];
      if (dirty[0] & /*label*/
      128 | dirty[1] & /*$$scope*/
      8192) {
        blocktitle_changes.$$scope = { dirty, ctx: ctx2 };
      }
      blocktitle.$set(blocktitle_changes);
      if (!current || dirty[0] & /*disabled*/
      16) {
        button.disabled = /*disabled*/
        ctx2[4];
      }
      if (dirty[0] & /*value*/
      2) {
        set_style(
          button,
          "background",
          /*value*/
          ctx2[1]
        );
      }
      if (
        /*dialog_open*/
        ctx2[2]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*dialog_open*/
          4) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(blocktitle.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(blocktitle.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(button);
        detach(t1);
        detach(if_block_anchor);
      }
      destroy_component(blocktitle, detaching);
      if (if_block)
        if_block.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let color_string;
  let { value = "#000000" } = $$props;
  let { value_is_output = false } = $$props;
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { disabled = false } = $$props;
  let { show_label = true } = $$props;
  let { root } = $$props;
  let { current_mode = "hex" } = $$props;
  let { dialog_open = false } = $$props;
  let eyedropper_supported = false;
  let sl_wrap;
  let hue_wrap;
  const dispatch = createEventDispatcher();
  let sl_marker_pos = [0, 0];
  let sl_rect = null;
  let sl_moving = false;
  let sl = [0, 0];
  let hue = 0;
  let hue_marker_pos = 0;
  let hue_rect = null;
  let hue_moving = false;
  function handle_hue_down(event) {
    hue_rect = event.currentTarget.getBoundingClientRect();
    hue_moving = true;
    update_hue_from_mouse(event.clientX);
  }
  function update_hue_from_mouse(x) {
    if (!hue_rect)
      return;
    const _x = Math.max(0, Math.min(x - hue_rect.left, hue_rect.width));
    $$invalidate(14, hue_marker_pos = _x);
    const _hue = _x / hue_rect.width * 360;
    $$invalidate(13, hue = _hue);
    $$invalidate(1, value = hsva_to_rgba({ h: _hue, s: sl[0], v: sl[1], a: 1 }));
  }
  function update_color_from_mouse(x, y) {
    if (!sl_rect)
      return;
    const _x = Math.max(0, Math.min(x - sl_rect.left, sl_rect.width));
    const _y = Math.max(0, Math.min(y - sl_rect.top, sl_rect.height));
    $$invalidate(12, sl_marker_pos = [_x, _y]);
    const _hsva = {
      h: hue * 1,
      s: _x / sl_rect.width,
      v: 1 - _y / sl_rect.height,
      a: 1
    };
    sl = [_hsva.s, _hsva.v];
    $$invalidate(1, value = hsva_to_rgba(_hsva));
  }
  function handle_sl_down(event) {
    sl_moving = true;
    sl_rect = event.currentTarget.getBoundingClientRect();
    update_color_from_mouse(event.clientX, event.clientY);
  }
  function handle_move(event) {
    if (sl_moving)
      update_color_from_mouse(event.clientX, event.clientY);
    if (hue_moving)
      update_hue_from_mouse(event.clientX);
  }
  function handle_end() {
    sl_moving = false;
    hue_moving = false;
  }
  async function update_mouse_from_color(color) {
    if (sl_moving || hue_moving)
      return;
    await tick();
    if (!color)
      return;
    if (!sl_rect && sl_wrap) {
      sl_rect = sl_wrap.getBoundingClientRect();
    }
    if (!hue_rect && hue_wrap) {
      hue_rect = hue_wrap.getBoundingClientRect();
    }
    if (!sl_rect || !hue_rect)
      return;
    const hsva = tinycolor(color).toHsv();
    const _x = hsva.s * sl_rect.width;
    const _y = (1 - hsva.v) * sl_rect.height;
    $$invalidate(12, sl_marker_pos = [_x, _y]);
    sl = [hsva.s, hsva.v];
    $$invalidate(13, hue = hsva.h);
    $$invalidate(14, hue_marker_pos = hsva.h / 360 * hue_rect.width);
  }
  function request_eyedropper() {
    const eyeDropper = new EyeDropper();
    eyeDropper.open().then((result) => {
      $$invalidate(1, value = result.sRGBHex);
    });
  }
  const modes = [["Hex", "hex"], ["RGB", "rgb"], ["HSL", "hsl"]];
  onMount(async () => {
    $$invalidate(9, eyedropper_supported = window !== void 0 && !!window.EyeDropper);
  });
  function handle_click_outside() {
    $$invalidate(2, dialog_open = false);
  }
  function handle_change() {
    dispatch("change", value);
    if (!value_is_output) {
      dispatch("input");
    }
  }
  afterUpdate(() => {
    $$invalidate(24, value_is_output = false);
  });
  function handle_click() {
    dispatch("selected", color_string);
    dispatch("close");
  }
  function focus_handler(event) {
    bubble.call(this, $$self, event);
  }
  function blur_handler(event) {
    bubble.call(this, $$self, event);
  }
  const click_handler = () => {
    update_mouse_from_color(value);
    $$invalidate(2, dialog_open = !dialog_open);
  };
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      sl_wrap = $$value;
      $$invalidate(10, sl_wrap);
    });
  }
  function div3_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      hue_wrap = $$value;
      $$invalidate(11, hue_wrap);
    });
  }
  function input_input_handler() {
    color_string = this.value;
    $$invalidate(8, color_string), $$invalidate(1, value), $$invalidate(0, current_mode);
  }
  const change_handler = (e) => $$invalidate(1, value = e.currentTarget.value);
  const click_handler_1 = (value2) => $$invalidate(0, current_mode = value2);
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(1, value = $$props2.value);
    if ("value_is_output" in $$props2)
      $$invalidate(24, value_is_output = $$props2.value_is_output);
    if ("label" in $$props2)
      $$invalidate(7, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(3, info = $$props2.info);
    if ("disabled" in $$props2)
      $$invalidate(4, disabled = $$props2.disabled);
    if ("show_label" in $$props2)
      $$invalidate(5, show_label = $$props2.show_label);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("current_mode" in $$props2)
      $$invalidate(0, current_mode = $$props2.current_mode);
    if ("dialog_open" in $$props2)
      $$invalidate(2, dialog_open = $$props2.dialog_open);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value, current_mode*/
    3) {
      $$invalidate(8, color_string = format_color(value, current_mode));
    }
    if ($$self.$$.dirty[0] & /*color_string*/
    256) {
      color_string && dispatch("selected", color_string);
    }
    if ($$self.$$.dirty[0] & /*value*/
    2) {
      update_mouse_from_color(value);
    }
    if ($$self.$$.dirty[0] & /*value*/
    2) {
      handle_change();
    }
  };
  return [
    current_mode,
    value,
    dialog_open,
    info,
    disabled,
    show_label,
    root,
    label,
    color_string,
    eyedropper_supported,
    sl_wrap,
    hue_wrap,
    sl_marker_pos,
    hue,
    hue_marker_pos,
    handle_hue_down,
    handle_sl_down,
    handle_move,
    handle_end,
    update_mouse_from_color,
    request_eyedropper,
    modes,
    handle_click_outside,
    handle_click,
    value_is_output,
    focus_handler,
    blur_handler,
    click_handler,
    div1_binding,
    div3_binding,
    input_input_handler,
    change_handler,
    click_handler_1
  ];
}
class Colorpicker extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 1,
        value_is_output: 24,
        label: 7,
        info: 3,
        disabled: 4,
        show_label: 5,
        root: 6,
        current_mode: 0,
        dialog_open: 2
      },
      null,
      [-1, -1]
    );
  }
}
const Colorpicker$1 = Colorpicker;
function create_default_slot(ctx) {
  let statustracker;
  let t;
  let colorpicker;
  let updating_value;
  let updating_value_is_output;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[13].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[13].i18n
    ) },
    /*loading_status*/
    ctx[11]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[16]
  );
  function colorpicker_value_binding(value) {
    ctx[17](value);
  }
  function colorpicker_value_is_output_binding(value) {
    ctx[18](value);
  }
  let colorpicker_props = {
    root: (
      /*root*/
      ctx[12]
    ),
    label: (
      /*label*/
      ctx[2]
    ),
    info: (
      /*info*/
      ctx[3]
    ),
    show_label: (
      /*show_label*/
      ctx[7]
    ),
    disabled: !/*interactive*/
    ctx[14] || /*disabled*/
    ctx[15]
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    colorpicker_props.value = /*value*/
    ctx[0];
  }
  if (
    /*value_is_output*/
    ctx[1] !== void 0
  ) {
    colorpicker_props.value_is_output = /*value_is_output*/
    ctx[1];
  }
  colorpicker = new Colorpicker$1({ props: colorpicker_props });
  binding_callbacks.push(() => bind(colorpicker, "value", colorpicker_value_binding));
  binding_callbacks.push(() => bind(colorpicker, "value_is_output", colorpicker_value_is_output_binding));
  colorpicker.$on(
    "change",
    /*change_handler*/
    ctx[19]
  );
  colorpicker.$on(
    "input",
    /*input_handler*/
    ctx[20]
  );
  colorpicker.$on(
    "submit",
    /*submit_handler*/
    ctx[21]
  );
  colorpicker.$on(
    "blur",
    /*blur_handler*/
    ctx[22]
  );
  colorpicker.$on(
    "focus",
    /*focus_handler*/
    ctx[23]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(colorpicker.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(colorpicker.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(colorpicker, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      10240 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        8192 && {
          autoscroll: (
            /*gradio*/
            ctx2[13].autoscroll
          )
        },
        dirty & /*gradio*/
        8192 && { i18n: (
          /*gradio*/
          ctx2[13].i18n
        ) },
        dirty & /*loading_status*/
        2048 && get_spread_object(
          /*loading_status*/
          ctx2[11]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const colorpicker_changes = {};
      if (dirty & /*root*/
      4096)
        colorpicker_changes.root = /*root*/
        ctx2[12];
      if (dirty & /*label*/
      4)
        colorpicker_changes.label = /*label*/
        ctx2[2];
      if (dirty & /*info*/
      8)
        colorpicker_changes.info = /*info*/
        ctx2[3];
      if (dirty & /*show_label*/
      128)
        colorpicker_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty & /*interactive, disabled*/
      49152)
        colorpicker_changes.disabled = !/*interactive*/
        ctx2[14] || /*disabled*/
        ctx2[15];
      if (!updating_value && dirty & /*value*/
      1) {
        updating_value = true;
        colorpicker_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      if (!updating_value_is_output && dirty & /*value_is_output*/
      2) {
        updating_value_is_output = true;
        colorpicker_changes.value_is_output = /*value_is_output*/
        ctx2[1];
        add_flush_callback(() => updating_value_is_output = false);
      }
      colorpicker.$set(colorpicker_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(colorpicker.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(colorpicker.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(colorpicker, detaching);
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
        ctx[6]
      ),
      elem_id: (
        /*elem_id*/
        ctx[4]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[5]
      ),
      container: (
        /*container*/
        ctx[8]
      ),
      scale: (
        /*scale*/
        ctx[9]
      ),
      min_width: (
        /*min_width*/
        ctx[10]
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
      64)
        block_changes.visible = /*visible*/
        ctx2[6];
      if (dirty & /*elem_id*/
      16)
        block_changes.elem_id = /*elem_id*/
        ctx2[4];
      if (dirty & /*elem_classes*/
      32)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[5];
      if (dirty & /*container*/
      256)
        block_changes.container = /*container*/
        ctx2[8];
      if (dirty & /*scale*/
      512)
        block_changes.scale = /*scale*/
        ctx2[9];
      if (dirty & /*min_width*/
      1024)
        block_changes.min_width = /*min_width*/
        ctx2[10];
      if (dirty & /*$$scope, root, label, info, show_label, interactive, disabled, value, value_is_output, gradio, loading_status*/
      16840847) {
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
  let { label = "ColorPicker" } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { value_is_output = false } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { root } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  let { disabled = false } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function colorpicker_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  function colorpicker_value_is_output_binding(value2) {
    value_is_output = value2;
    $$invalidate(1, value_is_output);
  }
  const change_handler = () => gradio.dispatch("change");
  const input_handler = () => gradio.dispatch("input");
  const submit_handler = () => gradio.dispatch("submit");
  const blur_handler = () => gradio.dispatch("blur");
  const focus_handler = () => gradio.dispatch("focus");
  $$self.$$set = ($$props2) => {
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(3, info = $$props2.info);
    if ("elem_id" in $$props2)
      $$invalidate(4, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(5, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(6, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("value_is_output" in $$props2)
      $$invalidate(1, value_is_output = $$props2.value_is_output);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(8, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(9, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(10, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(11, loading_status = $$props2.loading_status);
    if ("root" in $$props2)
      $$invalidate(12, root = $$props2.root);
    if ("gradio" in $$props2)
      $$invalidate(13, gradio = $$props2.gradio);
    if ("interactive" in $$props2)
      $$invalidate(14, interactive = $$props2.interactive);
    if ("disabled" in $$props2)
      $$invalidate(15, disabled = $$props2.disabled);
  };
  return [
    value,
    value_is_output,
    label,
    info,
    elem_id,
    elem_classes,
    visible,
    show_label,
    container,
    scale,
    min_width,
    loading_status,
    root,
    gradio,
    interactive,
    disabled,
    clear_status_handler,
    colorpicker_value_binding,
    colorpicker_value_is_output_binding,
    change_handler,
    input_handler,
    submit_handler,
    blur_handler,
    focus_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      label: 2,
      info: 3,
      elem_id: 4,
      elem_classes: 5,
      visible: 6,
      value: 0,
      value_is_output: 1,
      show_label: 7,
      container: 8,
      scale: 9,
      min_width: 10,
      loading_status: 11,
      root: 12,
      gradio: 13,
      interactive: 14,
      disabled: 15
    });
  }
  get label() {
    return this.$$.ctx[2];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get info() {
    return this.$$.ctx[3];
  }
  set info(info) {
    this.$$set({ info });
    flush();
  }
  get elem_id() {
    return this.$$.ctx[4];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[5];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[6];
  }
  set visible(visible) {
    this.$$set({ visible });
    flush();
  }
  get value() {
    return this.$$.ctx[0];
  }
  set value(value) {
    this.$$set({ value });
    flush();
  }
  get value_is_output() {
    return this.$$.ctx[1];
  }
  set value_is_output(value_is_output) {
    this.$$set({ value_is_output });
    flush();
  }
  get show_label() {
    return this.$$.ctx[7];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get container() {
    return this.$$.ctx[8];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[9];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[10];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[11];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get root() {
    return this.$$.ctx[12];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get gradio() {
    return this.$$.ctx[13];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get interactive() {
    return this.$$.ctx[14];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get disabled() {
    return this.$$.ctx[15];
  }
  set disabled(disabled) {
    this.$$set({ disabled });
    flush();
  }
}
export {
  Colorpicker$1 as BaseColorPicker,
  default2 as BaseExample,
  Index as default
};
