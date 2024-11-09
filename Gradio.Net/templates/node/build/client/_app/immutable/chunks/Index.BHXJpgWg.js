import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, tick, binding_callbacks, bind, element, create_component, space, claim_element, claim_component, claim_space, toggle_class, mount_component, set_input_value, listen, action_destroyer, prevent_default, transition_in, group_outros, transition_out, check_outros, add_flush_callback, is_function, destroy_component, run_all, createEventDispatcher, beforeUpdate, afterUpdate, text, claim_text, set_data, ensure_array_like, set_style, destroy_each, bubble, flush, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { f as BlockTitle, C as Clear, v as Send, w as Square, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { F as File } from "./File.DqOJDDoa.js";
import { M as Music } from "./Music.BKn1BNLT.js";
import { V as Video } from "./Video.CzEOFOtQ.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
/* empty css                                                    */
import { I as Image } from "./Image.eJ_qOnkr.js";
/* empty css                                                    */
import { default as default2 } from "./Example.Bdco60LN.js";
function create_fragment$2(ctx) {
  let svg;
  let g0;
  let g1;
  let g2;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      g0 = svg_element("g");
      g1 = svg_element("g");
      g2 = svg_element("g");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        fill: true,
        width: true,
        height: true,
        viewBox: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      g0 = claim_svg_element(svg_nodes, "g", { id: true, "stroke-width": true });
      children(g0).forEach(detach);
      g1 = claim_svg_element(svg_nodes, "g", {
        id: true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(g1).forEach(detach);
      g2 = claim_svg_element(svg_nodes, "g", { id: true });
      var g2_nodes = children(g2);
      path = claim_svg_element(g2_nodes, "path", { d: true, "fill-rule": true });
      children(path).forEach(detach);
      g2_nodes.forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(g0, "id", "SVGRepo_bgCarrier");
      attr(g0, "stroke-width", "0");
      attr(g1, "id", "SVGRepo_tracerCarrier");
      attr(g1, "stroke-linecap", "round");
      attr(g1, "stroke-linejoin", "round");
      attr(path, "d", "M1752.768 221.109C1532.646.986 1174.283.986 954.161 221.109l-838.588 838.588c-154.052 154.165-154.052 404.894 0 558.946 149.534 149.421 409.976 149.308 559.059 0l758.738-758.626c87.982-88.094 87.982-231.417 0-319.51-88.32-88.208-231.642-87.982-319.51 0l-638.796 638.908 79.85 79.849 638.795-638.908c43.934-43.821 115.539-43.934 159.812 0 43.934 44.047 43.934 115.877 0 159.812l-758.739 758.625c-110.23 110.118-289.355 110.005-399.36 0-110.118-110.117-110.005-289.242 0-399.247l838.588-838.588c175.963-175.962 462.382-176.188 638.909 0 176.075 176.188 176.075 462.833 0 638.908l-798.607 798.72 79.849 79.85 798.607-798.72c220.01-220.123 220.01-578.485 0-798.607");
      attr(path, "fill-rule", "evenodd");
      attr(g2, "id", "SVGRepo_iconCarrier");
      attr(svg, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 1920 1920");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, g0);
      append_hydration(svg, g1);
      append_hydration(svg, g2);
      append_hydration(g2, path);
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
class Paperclip extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$2, safe_not_equal, {});
  }
}
async function resize(target, lines, max_lines) {
  await tick();
  if (lines === max_lines)
    return;
  const computed_styles = window.getComputedStyle(target);
  const padding_top = parseFloat(computed_styles.paddingTop);
  const padding_bottom = parseFloat(computed_styles.paddingBottom);
  const line_height = parseFloat(computed_styles.lineHeight);
  let max = max_lines === void 0 ? false : padding_top + padding_bottom + line_height * max_lines;
  let min = padding_top + padding_bottom + lines * line_height;
  target.style.height = "1px";
  let scroll_height;
  if (max && target.scrollHeight > max) {
    scroll_height = max;
  } else if (target.scrollHeight < min) {
    scroll_height = min;
  } else {
    scroll_height = target.scrollHeight;
  }
  target.style.height = `${scroll_height}px`;
}
function text_area_resize(_el, _value) {
  if (_value.lines === _value.max_lines)
    return;
  _el.style.overflowY = "scroll";
  function handle_input(event) {
    resize(event.target, _value.lines, _value.max_lines);
  }
  _el.addEventListener("input", handle_input);
  if (!_value.text.trim())
    return;
  resize(_el, _value.lines, _value.max_lines);
  return {
    destroy: () => _el.removeEventListener("input", handle_input)
  };
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[59] = list[i];
  child_ctx[61] = i;
  return child_ctx;
}
function create_default_slot$1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*label*/
        ctx[5]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*label*/
        ctx[5]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*label*/
      32)
        set_data(
          t,
          /*label*/
          ctx2[5]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_if_block_4(ctx) {
  let div;
  let t;
  let current;
  let each_value = ensure_array_like(
    /*value*/
    ctx[0].files
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  let if_block = (
    /*uploading*/
    ctx[24] && create_if_block_5()
  );
  return {
    c() {
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        class: true,
        "aria-label": true,
        "data-testid": true,
        style: true
      });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      t = claim_space(div_nodes);
      if (if_block)
        if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "thumbnails scroll-hide svelte-1d7elt4");
      attr(div, "aria-label", "Uploaded files");
      attr(div, "data-testid", "container_el");
      set_style(
        div,
        "display",
        /*value*/
        ctx[0].files.length > 0 || /*uploading*/
        ctx[24] ? "flex" : "none"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      append_hydration(div, t);
      if (if_block)
        if_block.m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*value, disabled, remove_thumbnail*/
      1073741841) {
        each_value = ensure_array_like(
          /*value*/
          ctx2[0].files
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
            each_blocks[i].m(div, t);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if (
        /*uploading*/
        ctx2[24]
      ) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_5();
          if_block.c();
          if_block.m(div, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (!current || dirty[0] & /*value, uploading*/
      16777217) {
        set_style(
          div,
          "display",
          /*value*/
          ctx2[0].files.length > 0 || /*uploading*/
          ctx2[24] ? "flex" : "none"
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
        detach(div);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d();
    }
  };
}
function create_else_block_2(ctx) {
  let file_1;
  let current;
  file_1 = new File({});
  return {
    c() {
      create_component(file_1.$$.fragment);
    },
    l(nodes) {
      claim_component(file_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(file_1, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(file_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(file_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(file_1, detaching);
    }
  };
}
function create_if_block_8(ctx) {
  let video;
  let current;
  video = new Video({});
  return {
    c() {
      create_component(video.$$.fragment);
    },
    l(nodes) {
      claim_component(video.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(video, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(video.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(video.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(video, detaching);
    }
  };
}
function create_if_block_7(ctx) {
  let music;
  let current;
  music = new Music({});
  return {
    c() {
      create_component(music.$$.fragment);
    },
    l(nodes) {
      claim_component(music.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(music, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(music.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(music.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(music, detaching);
    }
  };
}
function create_if_block_6(ctx) {
  let image;
  let current;
  image = new Image({
    props: {
      src: (
        /*file*/
        ctx[59].url
      ),
      title: null,
      alt: "",
      loading: "lazy",
      class: "thumbnail-image"
    }
  });
  return {
    c() {
      create_component(image.$$.fragment);
    },
    l(nodes) {
      claim_component(image.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(image, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const image_changes = {};
      if (dirty[0] & /*value*/
      1)
        image_changes.src = /*file*/
        ctx2[59].url;
      image.$set(image_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(image, detaching);
    }
  };
}
function create_each_block(ctx) {
  let span;
  let button1;
  let button0;
  let clear;
  let t;
  let show_if;
  let show_if_1;
  let show_if_2;
  let current_block_type_index;
  let if_block;
  let current;
  let mounted;
  let dispose;
  clear = new Clear({});
  function click_handler(...args) {
    return (
      /*click_handler*/
      ctx[44](
        /*index*/
        ctx[61],
        ...args
      )
    );
  }
  const if_block_creators = [create_if_block_6, create_if_block_7, create_if_block_8, create_else_block_2];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (dirty[0] & /*value*/
    1)
      show_if = null;
    if (dirty[0] & /*value*/
    1)
      show_if_1 = null;
    if (dirty[0] & /*value*/
    1)
      show_if_2 = null;
    if (show_if == null)
      show_if = !!/*file*/
      (ctx2[59].mime_type && /*file*/
      ctx2[59].mime_type.includes("image"));
    if (show_if)
      return 0;
    if (show_if_1 == null)
      show_if_1 = !!/*file*/
      (ctx2[59].mime_type && /*file*/
      ctx2[59].mime_type.includes("audio"));
    if (show_if_1)
      return 1;
    if (show_if_2 == null)
      show_if_2 = !!/*file*/
      (ctx2[59].mime_type && /*file*/
      ctx2[59].mime_type.includes("video"));
    if (show_if_2)
      return 2;
    return 3;
  }
  current_block_type_index = select_block_type(ctx, [-1, -1, -1]);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      span = element("span");
      button1 = element("button");
      button0 = element("button");
      create_component(clear.$$.fragment);
      t = space();
      if_block.c();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { role: true, "aria-label": true });
      var span_nodes = children(span);
      button1 = claim_element(span_nodes, "BUTTON", { class: true });
      var button1_nodes = children(button1);
      button0 = claim_element(button1_nodes, "BUTTON", { class: true });
      var button0_nodes = children(button0);
      claim_component(clear.$$.fragment, button0_nodes);
      button0_nodes.forEach(detach);
      t = claim_space(button1_nodes);
      if_block.l(button1_nodes);
      button1_nodes.forEach(detach);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button0, "class", "delete-button svelte-1d7elt4");
      toggle_class(
        button0,
        "disabled",
        /*disabled*/
        ctx[4]
      );
      attr(button1, "class", "thumbnail-item thumbnail-small svelte-1d7elt4");
      attr(span, "role", "listitem");
      attr(span, "aria-label", "File thumbnail");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, button1);
      append_hydration(button1, button0);
      mount_component(clear, button0, null);
      append_hydration(button1, t);
      if_blocks[current_block_type_index].m(button1, null);
      current = true;
      if (!mounted) {
        dispose = listen(button0, "click", click_handler);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (!current || dirty[0] & /*disabled*/
      16) {
        toggle_class(
          button0,
          "disabled",
          /*disabled*/
          ctx[4]
        );
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx, dirty);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
          if_block.c();
        } else {
          if_block.p(ctx, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(button1, null);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(clear.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(clear.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      destroy_component(clear);
      if_blocks[current_block_type_index].d();
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_5(ctx) {
  let div;
  return {
    c() {
      div = element("div");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        class: true,
        role: true,
        "aria-label": true
      });
      children(div).forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "loader svelte-1d7elt4");
      attr(div, "role", "status");
      attr(div, "aria-label", "Uploading");
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
function create_if_block_2(ctx) {
  let button;
  let current_block_type_index;
  let if_block;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_3, create_else_block_1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*submit_btn*/
      ctx2[10] === true
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      button = element("button");
      if_block.c();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      if_block.l(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "submit-button svelte-1d7elt4");
      toggle_class(
        button,
        "padded-button",
        /*submit_btn*/
        ctx[10] !== true
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if_blocks[current_block_type_index].m(button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*handle_submit*/
          ctx[33]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
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
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block.c();
        } else {
          if_block.p(ctx2, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(button, null);
      }
      if (!current || dirty[0] & /*submit_btn*/
      1024) {
        toggle_class(
          button,
          "padded-button",
          /*submit_btn*/
          ctx2[10] !== true
        );
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
        detach(button);
      }
      if_blocks[current_block_type_index].d();
      mounted = false;
      dispose();
    }
  };
}
function create_else_block_1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*submit_btn*/
        ctx[10]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*submit_btn*/
        ctx[10]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*submit_btn*/
      1024)
        set_data(
          t,
          /*submit_btn*/
          ctx2[10]
        );
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
function create_if_block_3(ctx) {
  let send;
  let current;
  send = new Send({});
  return {
    c() {
      create_component(send.$$.fragment);
    },
    l(nodes) {
      claim_component(send.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(send, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(send.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(send.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(send, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let button;
  let current_block_type_index;
  let if_block;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_1, create_else_block];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    if (
      /*stop_btn*/
      ctx2[11] === true
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_2(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      button = element("button");
      if_block.c();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      if_block.l(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "stop-button svelte-1d7elt4");
      toggle_class(
        button,
        "padded-button",
        /*stop_btn*/
        ctx[11] !== true
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if_blocks[current_block_type_index].m(button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*handle_stop*/
          ctx[32]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
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
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block.c();
        } else {
          if_block.p(ctx2, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(button, null);
      }
      if (!current || dirty[0] & /*stop_btn*/
      2048) {
        toggle_class(
          button,
          "padded-button",
          /*stop_btn*/
          ctx2[11] !== true
        );
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
        detach(button);
      }
      if_blocks[current_block_type_index].d();
      mounted = false;
      dispose();
    }
  };
}
function create_else_block(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*stop_btn*/
        ctx[11]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*stop_btn*/
        ctx[11]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*stop_btn*/
      2048)
        set_data(
          t,
          /*stop_btn*/
          ctx2[11]
        );
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
function create_if_block_1(ctx) {
  let square;
  let current;
  square = new Square({
    props: { fill: "none", stroke_width: 2.5 }
  });
  return {
    c() {
      create_component(square.$$.fragment);
    },
    l(nodes) {
      claim_component(square.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(square, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(square.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(square.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(square, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let div1;
  let label_1;
  let blocktitle;
  let t0;
  let t1;
  let div0;
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let updating_hidden_upload;
  let t2;
  let button;
  let paperclip;
  let t3;
  let textarea;
  let textarea_dir_value;
  let textarea_style_value;
  let text_area_resize_action;
  let t4;
  let t5;
  let current;
  let mounted;
  let dispose;
  blocktitle = new BlockTitle({
    props: {
      root: (
        /*root*/
        ctx[15]
      ),
      show_label: (
        /*show_label*/
        ctx[7]
      ),
      info: (
        /*info*/
        ctx[6]
      ),
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  let if_block0 = (
    /*value*/
    (ctx[0].files.length > 0 || /*uploading*/
    ctx[24]) && create_if_block_4(ctx)
  );
  function upload_1_dragging_binding(value) {
    ctx[46](value);
  }
  function upload_1_uploading_binding(value) {
    ctx[47](value);
  }
  function upload_1_hidden_upload_binding(value) {
    ctx[48](value);
  }
  let upload_1_props = {
    file_count: (
      /*file_count*/
      ctx[20]
    ),
    filetype: (
      /*file_types*/
      ctx[16]
    ),
    root: (
      /*root*/
      ctx[15]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[17]
    ),
    show_progress: false,
    disable_click: true,
    hidden: true,
    upload: (
      /*upload*/
      ctx[18]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[19]
    )
  };
  if (
    /*dragging*/
    ctx[1] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[1];
  }
  if (
    /*uploading*/
    ctx[24] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[24];
  }
  if (
    /*hidden_upload*/
    ctx[23] !== void 0
  ) {
    upload_1_props.hidden_upload = /*hidden_upload*/
    ctx[23];
  }
  upload_1 = new Upload({ props: upload_1_props });
  ctx[45](upload_1);
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  binding_callbacks.push(() => bind(upload_1, "hidden_upload", upload_1_hidden_upload_binding));
  upload_1.$on(
    "load",
    /*handle_upload*/
    ctx[29]
  );
  upload_1.$on(
    "error",
    /*error_handler*/
    ctx[49]
  );
  paperclip = new Paperclip({});
  let if_block1 = (
    /*submit_btn*/
    ctx[10] && create_if_block_2(ctx)
  );
  let if_block2 = (
    /*stop_btn*/
    ctx[11] && create_if_block$1(ctx)
  );
  return {
    c() {
      div1 = element("div");
      label_1 = element("label");
      create_component(blocktitle.$$.fragment);
      t0 = space();
      if (if_block0)
        if_block0.c();
      t1 = space();
      div0 = element("div");
      create_component(upload_1.$$.fragment);
      t2 = space();
      button = element("button");
      create_component(paperclip.$$.fragment);
      t3 = space();
      textarea = element("textarea");
      t4 = space();
      if (if_block1)
        if_block1.c();
      t5 = space();
      if (if_block2)
        if_block2.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", {
        class: true,
        role: true,
        "aria-label": true
      });
      var div1_nodes = children(div1);
      label_1 = claim_element(div1_nodes, "LABEL", {});
      var label_1_nodes = children(label_1);
      claim_component(blocktitle.$$.fragment, label_1_nodes);
      t0 = claim_space(label_1_nodes);
      if (if_block0)
        if_block0.l(label_1_nodes);
      t1 = claim_space(label_1_nodes);
      div0 = claim_element(label_1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      claim_component(upload_1.$$.fragment, div0_nodes);
      t2 = claim_space(div0_nodes);
      button = claim_element(div0_nodes, "BUTTON", { "data-testid": true, class: true });
      var button_nodes = children(button);
      claim_component(paperclip.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      t3 = claim_space(div0_nodes);
      textarea = claim_element(div0_nodes, "TEXTAREA", {
        "data-testid": true,
        class: true,
        dir: true,
        placeholder: true,
        rows: true,
        style: true
      });
      children(textarea).forEach(detach);
      t4 = claim_space(div0_nodes);
      if (if_block1)
        if_block1.l(div0_nodes);
      t5 = claim_space(div0_nodes);
      if (if_block2)
        if_block2.l(div0_nodes);
      div0_nodes.forEach(detach);
      label_1_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "data-testid", "upload-button");
      attr(button, "class", "upload-button svelte-1d7elt4");
      attr(textarea, "data-testid", "textbox");
      attr(textarea, "class", "scroll-hide svelte-1d7elt4");
      attr(textarea, "dir", textarea_dir_value = /*rtl*/
      ctx[12] ? "rtl" : "ltr");
      attr(
        textarea,
        "placeholder",
        /*placeholder*/
        ctx[3]
      );
      attr(
        textarea,
        "rows",
        /*lines*/
        ctx[2]
      );
      textarea.disabled = /*disabled*/
      ctx[4];
      textarea.autofocus = /*autofocus*/
      ctx[13];
      attr(textarea, "style", textarea_style_value = /*text_align*/
      ctx[14] ? "text-align: " + /*text_align*/
      ctx[14] : "");
      toggle_class(textarea, "no-label", !/*show_label*/
      ctx[7]);
      attr(div0, "class", "input-container svelte-1d7elt4");
      toggle_class(
        label_1,
        "container",
        /*container*/
        ctx[8]
      );
      attr(div1, "class", "full-container svelte-1d7elt4");
      attr(div1, "role", "group");
      attr(div1, "aria-label", "Multimedia input field");
      toggle_class(
        div1,
        "dragging",
        /*dragging*/
        ctx[1]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, label_1);
      mount_component(blocktitle, label_1, null);
      append_hydration(label_1, t0);
      if (if_block0)
        if_block0.m(label_1, null);
      append_hydration(label_1, t1);
      append_hydration(label_1, div0);
      mount_component(upload_1, div0, null);
      append_hydration(div0, t2);
      append_hydration(div0, button);
      mount_component(paperclip, button, null);
      append_hydration(div0, t3);
      append_hydration(div0, textarea);
      set_input_value(
        textarea,
        /*value*/
        ctx[0].text
      );
      ctx[51](textarea);
      append_hydration(div0, t4);
      if (if_block1)
        if_block1.m(div0, null);
      append_hydration(div0, t5);
      if (if_block2)
        if_block2.m(div0, null);
      ctx[52](div1);
      current = true;
      if (
        /*autofocus*/
        ctx[13]
      )
        textarea.focus();
      if (!mounted) {
        dispose = [
          listen(
            button,
            "click",
            /*handle_upload_click*/
            ctx[31]
          ),
          action_destroyer(text_area_resize_action = text_area_resize.call(null, textarea, {
            text: (
              /*value*/
              ctx[0].text
            ),
            lines: (
              /*lines*/
              ctx[2]
            ),
            max_lines: (
              /*max_lines*/
              ctx[9]
            )
          })),
          listen(
            textarea,
            "input",
            /*textarea_input_handler*/
            ctx[50]
          ),
          listen(
            textarea,
            "keypress",
            /*handle_keypress*/
            ctx[27]
          ),
          listen(
            textarea,
            "blur",
            /*blur_handler*/
            ctx[42]
          ),
          listen(
            textarea,
            "select",
            /*handle_select*/
            ctx[26]
          ),
          listen(
            textarea,
            "focus",
            /*focus_handler*/
            ctx[43]
          ),
          listen(
            textarea,
            "scroll",
            /*handle_scroll*/
            ctx[28]
          ),
          listen(
            textarea,
            "paste",
            /*handle_paste*/
            ctx[34]
          ),
          listen(
            div1,
            "dragenter",
            /*handle_dragenter*/
            ctx[35]
          ),
          listen(
            div1,
            "dragleave",
            /*handle_dragleave*/
            ctx[36]
          ),
          listen(div1, "dragover", prevent_default(
            /*dragover_handler*/
            ctx[41]
          )),
          listen(
            div1,
            "drop",
            /*handle_drop*/
            ctx[37]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const blocktitle_changes = {};
      if (dirty[0] & /*root*/
      32768)
        blocktitle_changes.root = /*root*/
        ctx2[15];
      if (dirty[0] & /*show_label*/
      128)
        blocktitle_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*info*/
      64)
        blocktitle_changes.info = /*info*/
        ctx2[6];
      if (dirty[0] & /*label*/
      32 | dirty[2] & /*$$scope*/
      1) {
        blocktitle_changes.$$scope = { dirty, ctx: ctx2 };
      }
      blocktitle.$set(blocktitle_changes);
      if (
        /*value*/
        ctx2[0].files.length > 0 || /*uploading*/
        ctx2[24]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*value, uploading*/
          16777217) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(label_1, t1);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      const upload_1_changes = {};
      if (dirty[0] & /*file_count*/
      1048576)
        upload_1_changes.file_count = /*file_count*/
        ctx2[20];
      if (dirty[0] & /*file_types*/
      65536)
        upload_1_changes.filetype = /*file_types*/
        ctx2[16];
      if (dirty[0] & /*root*/
      32768)
        upload_1_changes.root = /*root*/
        ctx2[15];
      if (dirty[0] & /*max_file_size*/
      131072)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[17];
      if (dirty[0] & /*upload*/
      262144)
        upload_1_changes.upload = /*upload*/
        ctx2[18];
      if (dirty[0] & /*stream_handler*/
      524288)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[19];
      if (!updating_dragging && dirty[0] & /*dragging*/
      2) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[1];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      16777216) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[24];
        add_flush_callback(() => updating_uploading = false);
      }
      if (!updating_hidden_upload && dirty[0] & /*hidden_upload*/
      8388608) {
        updating_hidden_upload = true;
        upload_1_changes.hidden_upload = /*hidden_upload*/
        ctx2[23];
        add_flush_callback(() => updating_hidden_upload = false);
      }
      upload_1.$set(upload_1_changes);
      if (!current || dirty[0] & /*rtl*/
      4096 && textarea_dir_value !== (textarea_dir_value = /*rtl*/
      ctx2[12] ? "rtl" : "ltr")) {
        attr(textarea, "dir", textarea_dir_value);
      }
      if (!current || dirty[0] & /*placeholder*/
      8) {
        attr(
          textarea,
          "placeholder",
          /*placeholder*/
          ctx2[3]
        );
      }
      if (!current || dirty[0] & /*lines*/
      4) {
        attr(
          textarea,
          "rows",
          /*lines*/
          ctx2[2]
        );
      }
      if (!current || dirty[0] & /*disabled*/
      16) {
        textarea.disabled = /*disabled*/
        ctx2[4];
      }
      if (!current || dirty[0] & /*autofocus*/
      8192) {
        textarea.autofocus = /*autofocus*/
        ctx2[13];
      }
      if (!current || dirty[0] & /*text_align*/
      16384 && textarea_style_value !== (textarea_style_value = /*text_align*/
      ctx2[14] ? "text-align: " + /*text_align*/
      ctx2[14] : "")) {
        attr(textarea, "style", textarea_style_value);
      }
      if (text_area_resize_action && is_function(text_area_resize_action.update) && dirty[0] & /*value, lines, max_lines*/
      517)
        text_area_resize_action.update.call(null, {
          text: (
            /*value*/
            ctx2[0].text
          ),
          lines: (
            /*lines*/
            ctx2[2]
          ),
          max_lines: (
            /*max_lines*/
            ctx2[9]
          )
        });
      if (dirty[0] & /*value*/
      1) {
        set_input_value(
          textarea,
          /*value*/
          ctx2[0].text
        );
      }
      if (!current || dirty[0] & /*show_label*/
      128) {
        toggle_class(textarea, "no-label", !/*show_label*/
        ctx2[7]);
      }
      if (
        /*submit_btn*/
        ctx2[10]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*submit_btn*/
          1024) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div0, t5);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (
        /*stop_btn*/
        ctx2[11]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*stop_btn*/
          2048) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block$1(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(div0, null);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
      if (!current || dirty[0] & /*container*/
      256) {
        toggle_class(
          label_1,
          "container",
          /*container*/
          ctx2[8]
        );
      }
      if (!current || dirty[0] & /*dragging*/
      2) {
        toggle_class(
          div1,
          "dragging",
          /*dragging*/
          ctx2[1]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(blocktitle.$$.fragment, local);
      transition_in(if_block0);
      transition_in(upload_1.$$.fragment, local);
      transition_in(paperclip.$$.fragment, local);
      transition_in(if_block1);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(blocktitle.$$.fragment, local);
      transition_out(if_block0);
      transition_out(upload_1.$$.fragment, local);
      transition_out(paperclip.$$.fragment, local);
      transition_out(if_block1);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      destroy_component(blocktitle);
      if (if_block0)
        if_block0.d();
      ctx[45](null);
      destroy_component(upload_1);
      destroy_component(paperclip);
      ctx[51](null);
      if (if_block1)
        if_block1.d();
      if (if_block2)
        if_block2.d();
      ctx[52](null);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { value = { text: "", files: [] } } = $$props;
  let { value_is_output = false } = $$props;
  let { lines = 1 } = $$props;
  let { placeholder = "Type here..." } = $$props;
  let { disabled = false } = $$props;
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { container = true } = $$props;
  let { max_lines } = $$props;
  let { submit_btn = null } = $$props;
  let { stop_btn = null } = $$props;
  let { rtl = false } = $$props;
  let { autofocus = false } = $$props;
  let { text_align = void 0 } = $$props;
  let { autoscroll = true } = $$props;
  let { root } = $$props;
  let { file_types = null } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { file_count = "multiple" } = $$props;
  let upload_component;
  let hidden_upload;
  let el;
  let can_scroll;
  let previous_scroll_top = 0;
  let user_has_scrolled_up = false;
  let { dragging = false } = $$props;
  let uploading = false;
  let oldValue = value.text;
  let full_container;
  const dispatch = createEventDispatcher();
  beforeUpdate(() => {
    can_scroll = el && el.offsetHeight + el.scrollTop > el.scrollHeight - 100;
  });
  const scroll = () => {
    if (can_scroll && autoscroll && !user_has_scrolled_up) {
      el.scrollTo(0, el.scrollHeight);
    }
  };
  async function handle_change() {
    dispatch("change", value);
    if (!value_is_output) {
      dispatch("input");
    }
  }
  afterUpdate(() => {
    if (autofocus && el !== null) {
      el.focus();
    }
    if (can_scroll && autoscroll) {
      scroll();
    }
    $$invalidate(38, value_is_output = false);
  });
  function handle_select(event) {
    const target = event.target;
    const text2 = target.value;
    const index = [target.selectionStart, target.selectionEnd];
    dispatch("select", { value: text2.substring(...index), index });
  }
  async function handle_keypress(e) {
    await tick();
    if (e.key === "Enter" && e.shiftKey && lines > 1) {
      e.preventDefault();
      dispatch("submit");
    } else if (e.key === "Enter" && !e.shiftKey && lines === 1 && max_lines >= 1) {
      e.preventDefault();
      dispatch("submit");
    }
  }
  function handle_scroll(event) {
    const target = event.target;
    const current_scroll_top = target.scrollTop;
    if (current_scroll_top < previous_scroll_top) {
      user_has_scrolled_up = true;
    }
    previous_scroll_top = current_scroll_top;
    const max_scroll_top = target.scrollHeight - target.clientHeight;
    const user_has_scrolled_to_bottom = current_scroll_top >= max_scroll_top;
    if (user_has_scrolled_to_bottom) {
      user_has_scrolled_up = false;
    }
  }
  async function handle_upload({ detail }) {
    handle_change();
    if (Array.isArray(detail)) {
      for (let file of detail) {
        value.files.push(file);
      }
      $$invalidate(0, value);
    } else {
      value.files.push(detail);
      $$invalidate(0, value);
    }
    await tick();
    dispatch("change", value);
    dispatch("upload", detail);
  }
  function remove_thumbnail(event, index) {
    handle_change();
    event.stopPropagation();
    value.files.splice(index, 1);
    $$invalidate(0, value);
  }
  function handle_upload_click() {
    if (hidden_upload) {
      $$invalidate(23, hidden_upload.value = "", hidden_upload);
      hidden_upload.click();
    }
  }
  function handle_stop() {
    dispatch("stop");
  }
  function handle_submit() {
    dispatch("submit");
  }
  function handle_paste(event) {
    if (!event.clipboardData)
      return;
    const items = event.clipboardData.items;
    for (let index in items) {
      const item = items[index];
      if (item.kind === "file" && item.type.includes("image")) {
        const blob = item.getAsFile();
        if (blob)
          upload_component.load_files([blob]);
      }
    }
  }
  function handle_dragenter(event) {
    event.preventDefault();
    $$invalidate(1, dragging = true);
  }
  function handle_dragleave(event) {
    event.preventDefault();
    const rect = full_container.getBoundingClientRect();
    const { clientX, clientY } = event;
    if (clientX <= rect.left || clientX >= rect.right || clientY <= rect.top || clientY >= rect.bottom) {
      $$invalidate(1, dragging = false);
    }
  }
  function handle_drop(event) {
    event.preventDefault();
    $$invalidate(1, dragging = false);
    if (event.dataTransfer && event.dataTransfer.files) {
      upload_component.load_files(Array.from(event.dataTransfer.files));
    }
  }
  function dragover_handler(event) {
    bubble.call(this, $$self, event);
  }
  function blur_handler(event) {
    bubble.call(this, $$self, event);
  }
  function focus_handler(event) {
    bubble.call(this, $$self, event);
  }
  const click_handler = (index, event) => remove_thumbnail(event, index);
  function upload_1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      upload_component = $$value;
      $$invalidate(22, upload_component);
    });
  }
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(1, dragging);
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(24, uploading);
  }
  function upload_1_hidden_upload_binding(value2) {
    hidden_upload = value2;
    $$invalidate(23, hidden_upload);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function textarea_input_handler() {
    value.text = this.value;
    $$invalidate(0, value);
  }
  function textarea_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      el = $$value;
      $$invalidate(21, el);
    });
  }
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      full_container = $$value;
      $$invalidate(25, full_container);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("value_is_output" in $$props2)
      $$invalidate(38, value_is_output = $$props2.value_is_output);
    if ("lines" in $$props2)
      $$invalidate(2, lines = $$props2.lines);
    if ("placeholder" in $$props2)
      $$invalidate(3, placeholder = $$props2.placeholder);
    if ("disabled" in $$props2)
      $$invalidate(4, disabled = $$props2.disabled);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(6, info = $$props2.info);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(8, container = $$props2.container);
    if ("max_lines" in $$props2)
      $$invalidate(9, max_lines = $$props2.max_lines);
    if ("submit_btn" in $$props2)
      $$invalidate(10, submit_btn = $$props2.submit_btn);
    if ("stop_btn" in $$props2)
      $$invalidate(11, stop_btn = $$props2.stop_btn);
    if ("rtl" in $$props2)
      $$invalidate(12, rtl = $$props2.rtl);
    if ("autofocus" in $$props2)
      $$invalidate(13, autofocus = $$props2.autofocus);
    if ("text_align" in $$props2)
      $$invalidate(14, text_align = $$props2.text_align);
    if ("autoscroll" in $$props2)
      $$invalidate(39, autoscroll = $$props2.autoscroll);
    if ("root" in $$props2)
      $$invalidate(15, root = $$props2.root);
    if ("file_types" in $$props2)
      $$invalidate(16, file_types = $$props2.file_types);
    if ("max_file_size" in $$props2)
      $$invalidate(17, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(18, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(19, stream_handler = $$props2.stream_handler);
    if ("file_count" in $$props2)
      $$invalidate(20, file_count = $$props2.file_count);
    if ("dragging" in $$props2)
      $$invalidate(1, dragging = $$props2.dragging);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*dragging*/
    2) {
      dispatch("drag", dragging);
    }
    if ($$self.$$.dirty[0] & /*value*/
    1) {
      if (value === null)
        $$invalidate(0, value = { text: "", files: [] });
    }
    if ($$self.$$.dirty[0] & /*value*/
    1 | $$self.$$.dirty[1] & /*oldValue*/
    512) {
      if (oldValue !== value.text) {
        dispatch("change", value);
        $$invalidate(40, oldValue = value.text);
      }
    }
    if ($$self.$$.dirty[0] & /*value, el, lines, max_lines*/
    2097669) {
      el && lines !== max_lines && resize(el, lines, max_lines);
    }
  };
  return [
    value,
    dragging,
    lines,
    placeholder,
    disabled,
    label,
    info,
    show_label,
    container,
    max_lines,
    submit_btn,
    stop_btn,
    rtl,
    autofocus,
    text_align,
    root,
    file_types,
    max_file_size,
    upload,
    stream_handler,
    file_count,
    el,
    upload_component,
    hidden_upload,
    uploading,
    full_container,
    handle_select,
    handle_keypress,
    handle_scroll,
    handle_upload,
    remove_thumbnail,
    handle_upload_click,
    handle_stop,
    handle_submit,
    handle_paste,
    handle_dragenter,
    handle_dragleave,
    handle_drop,
    value_is_output,
    autoscroll,
    oldValue,
    dragover_handler,
    blur_handler,
    focus_handler,
    click_handler,
    upload_1_binding,
    upload_1_dragging_binding,
    upload_1_uploading_binding,
    upload_1_hidden_upload_binding,
    error_handler,
    textarea_input_handler,
    textarea_binding,
    div1_binding
  ];
}
class MultimodalTextbox extends SvelteComponent {
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
        value_is_output: 38,
        lines: 2,
        placeholder: 3,
        disabled: 4,
        label: 5,
        info: 6,
        show_label: 7,
        container: 8,
        max_lines: 9,
        submit_btn: 10,
        stop_btn: 11,
        rtl: 12,
        autofocus: 13,
        text_align: 14,
        autoscroll: 39,
        root: 15,
        file_types: 16,
        max_file_size: 17,
        upload: 18,
        stream_handler: 19,
        file_count: 20,
        dragging: 1
      },
      null,
      [-1, -1, -1]
    );
  }
}
const MultimodalTextbox$1 = MultimodalTextbox;
function create_if_block(ctx) {
  let statustracker;
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
    ctx[18]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[27]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      262148 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        4 && { autoscroll: (
          /*gradio*/
          ctx2[2].autoscroll
        ) },
        dirty[0] & /*gradio*/
        4 && { i18n: (
          /*gradio*/
          ctx2[2].i18n
        ) },
        dirty[0] & /*loading_status*/
        262144 && get_spread_object(
          /*loading_status*/
          ctx2[18]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(statustracker, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let t;
  let multimodaltextbox;
  let updating_value;
  let updating_value_is_output;
  let updating_dragging;
  let current;
  let if_block = (
    /*loading_status*/
    ctx[18] && create_if_block(ctx)
  );
  function multimodaltextbox_value_binding(value) {
    ctx[30](value);
  }
  function multimodaltextbox_value_is_output_binding(value) {
    ctx[31](value);
  }
  function multimodaltextbox_dragging_binding(value) {
    ctx[32](value);
  }
  let multimodaltextbox_props = {
    file_types: (
      /*file_types*/
      ctx[6]
    ),
    root: (
      /*root*/
      ctx[24]
    ),
    label: (
      /*label*/
      ctx[9]
    ),
    info: (
      /*info*/
      ctx[10]
    ),
    show_label: (
      /*show_label*/
      ctx[11]
    ),
    lines: (
      /*lines*/
      ctx[7]
    ),
    rtl: (
      /*rtl*/
      ctx[19]
    ),
    text_align: (
      /*text_align*/
      ctx[20]
    ),
    max_lines: !/*max_lines*/
    ctx[12] ? (
      /*lines*/
      ctx[7] + 1
    ) : (
      /*max_lines*/
      ctx[12]
    ),
    placeholder: (
      /*placeholder*/
      ctx[8]
    ),
    submit_btn: (
      /*submit_btn*/
      ctx[16]
    ),
    stop_btn: (
      /*stop_btn*/
      ctx[17]
    ),
    autofocus: (
      /*autofocus*/
      ctx[21]
    ),
    container: (
      /*container*/
      ctx[13]
    ),
    autoscroll: (
      /*autoscroll*/
      ctx[22]
    ),
    file_count: (
      /*file_count*/
      ctx[25]
    ),
    max_file_size: (
      /*gradio*/
      ctx[2].max_file_size
    ),
    disabled: !/*interactive*/
    ctx[23],
    upload: (
      /*func*/
      ctx[28]
    ),
    stream_handler: (
      /*func_1*/
      ctx[29]
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    multimodaltextbox_props.value = /*value*/
    ctx[0];
  }
  if (
    /*value_is_output*/
    ctx[1] !== void 0
  ) {
    multimodaltextbox_props.value_is_output = /*value_is_output*/
    ctx[1];
  }
  if (
    /*dragging*/
    ctx[26] !== void 0
  ) {
    multimodaltextbox_props.dragging = /*dragging*/
    ctx[26];
  }
  multimodaltextbox = new MultimodalTextbox$1({ props: multimodaltextbox_props });
  binding_callbacks.push(() => bind(multimodaltextbox, "value", multimodaltextbox_value_binding));
  binding_callbacks.push(() => bind(multimodaltextbox, "value_is_output", multimodaltextbox_value_is_output_binding));
  binding_callbacks.push(() => bind(multimodaltextbox, "dragging", multimodaltextbox_dragging_binding));
  multimodaltextbox.$on(
    "change",
    /*change_handler*/
    ctx[33]
  );
  multimodaltextbox.$on(
    "input",
    /*input_handler*/
    ctx[34]
  );
  multimodaltextbox.$on(
    "submit",
    /*submit_handler*/
    ctx[35]
  );
  multimodaltextbox.$on(
    "stop",
    /*stop_handler*/
    ctx[36]
  );
  multimodaltextbox.$on(
    "blur",
    /*blur_handler*/
    ctx[37]
  );
  multimodaltextbox.$on(
    "select",
    /*select_handler*/
    ctx[38]
  );
  multimodaltextbox.$on(
    "focus",
    /*focus_handler*/
    ctx[39]
  );
  multimodaltextbox.$on(
    "error",
    /*error_handler*/
    ctx[40]
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      create_component(multimodaltextbox.$$.fragment);
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      claim_component(multimodaltextbox.$$.fragment, nodes);
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(multimodaltextbox, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*loading_status*/
        ctx2[18]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*loading_status*/
          262144) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const multimodaltextbox_changes = {};
      if (dirty[0] & /*file_types*/
      64)
        multimodaltextbox_changes.file_types = /*file_types*/
        ctx2[6];
      if (dirty[0] & /*root*/
      16777216)
        multimodaltextbox_changes.root = /*root*/
        ctx2[24];
      if (dirty[0] & /*label*/
      512)
        multimodaltextbox_changes.label = /*label*/
        ctx2[9];
      if (dirty[0] & /*info*/
      1024)
        multimodaltextbox_changes.info = /*info*/
        ctx2[10];
      if (dirty[0] & /*show_label*/
      2048)
        multimodaltextbox_changes.show_label = /*show_label*/
        ctx2[11];
      if (dirty[0] & /*lines*/
      128)
        multimodaltextbox_changes.lines = /*lines*/
        ctx2[7];
      if (dirty[0] & /*rtl*/
      524288)
        multimodaltextbox_changes.rtl = /*rtl*/
        ctx2[19];
      if (dirty[0] & /*text_align*/
      1048576)
        multimodaltextbox_changes.text_align = /*text_align*/
        ctx2[20];
      if (dirty[0] & /*max_lines, lines*/
      4224)
        multimodaltextbox_changes.max_lines = !/*max_lines*/
        ctx2[12] ? (
          /*lines*/
          ctx2[7] + 1
        ) : (
          /*max_lines*/
          ctx2[12]
        );
      if (dirty[0] & /*placeholder*/
      256)
        multimodaltextbox_changes.placeholder = /*placeholder*/
        ctx2[8];
      if (dirty[0] & /*submit_btn*/
      65536)
        multimodaltextbox_changes.submit_btn = /*submit_btn*/
        ctx2[16];
      if (dirty[0] & /*stop_btn*/
      131072)
        multimodaltextbox_changes.stop_btn = /*stop_btn*/
        ctx2[17];
      if (dirty[0] & /*autofocus*/
      2097152)
        multimodaltextbox_changes.autofocus = /*autofocus*/
        ctx2[21];
      if (dirty[0] & /*container*/
      8192)
        multimodaltextbox_changes.container = /*container*/
        ctx2[13];
      if (dirty[0] & /*autoscroll*/
      4194304)
        multimodaltextbox_changes.autoscroll = /*autoscroll*/
        ctx2[22];
      if (dirty[0] & /*file_count*/
      33554432)
        multimodaltextbox_changes.file_count = /*file_count*/
        ctx2[25];
      if (dirty[0] & /*gradio*/
      4)
        multimodaltextbox_changes.max_file_size = /*gradio*/
        ctx2[2].max_file_size;
      if (dirty[0] & /*interactive*/
      8388608)
        multimodaltextbox_changes.disabled = !/*interactive*/
        ctx2[23];
      if (dirty[0] & /*gradio*/
      4)
        multimodaltextbox_changes.upload = /*func*/
        ctx2[28];
      if (dirty[0] & /*gradio*/
      4)
        multimodaltextbox_changes.stream_handler = /*func_1*/
        ctx2[29];
      if (!updating_value && dirty[0] & /*value*/
      1) {
        updating_value = true;
        multimodaltextbox_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      if (!updating_value_is_output && dirty[0] & /*value_is_output*/
      2) {
        updating_value_is_output = true;
        multimodaltextbox_changes.value_is_output = /*value_is_output*/
        ctx2[1];
        add_flush_callback(() => updating_value_is_output = false);
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      67108864) {
        updating_dragging = true;
        multimodaltextbox_changes.dragging = /*dragging*/
        ctx2[26];
        add_flush_callback(() => updating_dragging = false);
      }
      multimodaltextbox.$set(multimodaltextbox_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(multimodaltextbox.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(multimodaltextbox.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_component(multimodaltextbox, detaching);
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
        ctx[5]
      ),
      elem_id: (
        /*elem_id*/
        ctx[3]
      ),
      elem_classes: [.../*elem_classes*/
      ctx[4], "multimodal-textbox"],
      scale: (
        /*scale*/
        ctx[14]
      ),
      min_width: (
        /*min_width*/
        ctx[15]
      ),
      allow_overflow: false,
      padding: (
        /*container*/
        ctx[13]
      ),
      border_mode: (
        /*dragging*/
        ctx[26] ? "focus" : "base"
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
      if (dirty[0] & /*visible*/
      32)
        block_changes.visible = /*visible*/
        ctx2[5];
      if (dirty[0] & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty[0] & /*elem_classes*/
      16)
        block_changes.elem_classes = [.../*elem_classes*/
        ctx2[4], "multimodal-textbox"];
      if (dirty[0] & /*scale*/
      16384)
        block_changes.scale = /*scale*/
        ctx2[14];
      if (dirty[0] & /*min_width*/
      32768)
        block_changes.min_width = /*min_width*/
        ctx2[15];
      if (dirty[0] & /*container*/
      8192)
        block_changes.padding = /*container*/
        ctx2[13];
      if (dirty[0] & /*dragging*/
      67108864)
        block_changes.border_mode = /*dragging*/
        ctx2[26] ? "focus" : "base";
      if (dirty[0] & /*file_types, root, label, info, show_label, lines, rtl, text_align, max_lines, placeholder, submit_btn, stop_btn, autofocus, container, autoscroll, file_count, gradio, interactive, value, value_is_output, dragging, loading_status*/
      134168519 | dirty[1] & /*$$scope*/
      1024) {
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
  let { gradio } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = { text: "", files: [] } } = $$props;
  let { file_types = null } = $$props;
  let { lines } = $$props;
  let { placeholder = "" } = $$props;
  let { label = "MultimodalTextbox" } = $$props;
  let { info = void 0 } = $$props;
  let { show_label } = $$props;
  let { max_lines } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { submit_btn = null } = $$props;
  let { stop_btn = null } = $$props;
  let { loading_status = void 0 } = $$props;
  let { value_is_output = false } = $$props;
  let { rtl = false } = $$props;
  let { text_align = void 0 } = $$props;
  let { autofocus = false } = $$props;
  let { autoscroll = true } = $$props;
  let { interactive } = $$props;
  let { root } = $$props;
  let { file_count } = $$props;
  let dragging;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const func = (...args) => gradio.client.upload(...args);
  const func_1 = (...args) => gradio.client.stream(...args);
  function multimodaltextbox_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  function multimodaltextbox_value_is_output_binding(value2) {
    value_is_output = value2;
    $$invalidate(1, value_is_output);
  }
  function multimodaltextbox_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(26, dragging);
  }
  const change_handler = () => gradio.dispatch("change", value);
  const input_handler = () => gradio.dispatch("input");
  const submit_handler = () => gradio.dispatch("submit");
  const stop_handler = () => gradio.dispatch("stop");
  const blur_handler = () => gradio.dispatch("blur");
  const select_handler = (e) => gradio.dispatch("select", e.detail);
  const focus_handler = () => gradio.dispatch("focus");
  const error_handler = ({ detail }) => {
    gradio.dispatch("error", detail);
  };
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
    if ("file_types" in $$props2)
      $$invalidate(6, file_types = $$props2.file_types);
    if ("lines" in $$props2)
      $$invalidate(7, lines = $$props2.lines);
    if ("placeholder" in $$props2)
      $$invalidate(8, placeholder = $$props2.placeholder);
    if ("label" in $$props2)
      $$invalidate(9, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(10, info = $$props2.info);
    if ("show_label" in $$props2)
      $$invalidate(11, show_label = $$props2.show_label);
    if ("max_lines" in $$props2)
      $$invalidate(12, max_lines = $$props2.max_lines);
    if ("container" in $$props2)
      $$invalidate(13, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(14, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(15, min_width = $$props2.min_width);
    if ("submit_btn" in $$props2)
      $$invalidate(16, submit_btn = $$props2.submit_btn);
    if ("stop_btn" in $$props2)
      $$invalidate(17, stop_btn = $$props2.stop_btn);
    if ("loading_status" in $$props2)
      $$invalidate(18, loading_status = $$props2.loading_status);
    if ("value_is_output" in $$props2)
      $$invalidate(1, value_is_output = $$props2.value_is_output);
    if ("rtl" in $$props2)
      $$invalidate(19, rtl = $$props2.rtl);
    if ("text_align" in $$props2)
      $$invalidate(20, text_align = $$props2.text_align);
    if ("autofocus" in $$props2)
      $$invalidate(21, autofocus = $$props2.autofocus);
    if ("autoscroll" in $$props2)
      $$invalidate(22, autoscroll = $$props2.autoscroll);
    if ("interactive" in $$props2)
      $$invalidate(23, interactive = $$props2.interactive);
    if ("root" in $$props2)
      $$invalidate(24, root = $$props2.root);
    if ("file_count" in $$props2)
      $$invalidate(25, file_count = $$props2.file_count);
  };
  return [
    value,
    value_is_output,
    gradio,
    elem_id,
    elem_classes,
    visible,
    file_types,
    lines,
    placeholder,
    label,
    info,
    show_label,
    max_lines,
    container,
    scale,
    min_width,
    submit_btn,
    stop_btn,
    loading_status,
    rtl,
    text_align,
    autofocus,
    autoscroll,
    interactive,
    root,
    file_count,
    dragging,
    clear_status_handler,
    func,
    func_1,
    multimodaltextbox_value_binding,
    multimodaltextbox_value_is_output_binding,
    multimodaltextbox_dragging_binding,
    change_handler,
    input_handler,
    submit_handler,
    stop_handler,
    blur_handler,
    select_handler,
    focus_handler,
    error_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        gradio: 2,
        elem_id: 3,
        elem_classes: 4,
        visible: 5,
        value: 0,
        file_types: 6,
        lines: 7,
        placeholder: 8,
        label: 9,
        info: 10,
        show_label: 11,
        max_lines: 12,
        container: 13,
        scale: 14,
        min_width: 15,
        submit_btn: 16,
        stop_btn: 17,
        loading_status: 18,
        value_is_output: 1,
        rtl: 19,
        text_align: 20,
        autofocus: 21,
        autoscroll: 22,
        interactive: 23,
        root: 24,
        file_count: 25
      },
      null,
      [-1, -1]
    );
  }
  get gradio() {
    return this.$$.ctx[2];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get elem_id() {
    return this.$$.ctx[3];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[4];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[5];
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
  get file_types() {
    return this.$$.ctx[6];
  }
  set file_types(file_types) {
    this.$$set({ file_types });
    flush();
  }
  get lines() {
    return this.$$.ctx[7];
  }
  set lines(lines) {
    this.$$set({ lines });
    flush();
  }
  get placeholder() {
    return this.$$.ctx[8];
  }
  set placeholder(placeholder) {
    this.$$set({ placeholder });
    flush();
  }
  get label() {
    return this.$$.ctx[9];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get info() {
    return this.$$.ctx[10];
  }
  set info(info) {
    this.$$set({ info });
    flush();
  }
  get show_label() {
    return this.$$.ctx[11];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get max_lines() {
    return this.$$.ctx[12];
  }
  set max_lines(max_lines) {
    this.$$set({ max_lines });
    flush();
  }
  get container() {
    return this.$$.ctx[13];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[14];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[15];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get submit_btn() {
    return this.$$.ctx[16];
  }
  set submit_btn(submit_btn) {
    this.$$set({ submit_btn });
    flush();
  }
  get stop_btn() {
    return this.$$.ctx[17];
  }
  set stop_btn(stop_btn) {
    this.$$set({ stop_btn });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[18];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get value_is_output() {
    return this.$$.ctx[1];
  }
  set value_is_output(value_is_output) {
    this.$$set({ value_is_output });
    flush();
  }
  get rtl() {
    return this.$$.ctx[19];
  }
  set rtl(rtl) {
    this.$$set({ rtl });
    flush();
  }
  get text_align() {
    return this.$$.ctx[20];
  }
  set text_align(text_align) {
    this.$$set({ text_align });
    flush();
  }
  get autofocus() {
    return this.$$.ctx[21];
  }
  set autofocus(autofocus) {
    this.$$set({ autofocus });
    flush();
  }
  get autoscroll() {
    return this.$$.ctx[22];
  }
  set autoscroll(autoscroll) {
    this.$$set({ autoscroll });
    flush();
  }
  get interactive() {
    return this.$$.ctx[23];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get root() {
    return this.$$.ctx[24];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get file_count() {
    return this.$$.ctx[25];
  }
  set file_count(file_count) {
    this.$$set({ file_count });
    flush();
  }
}
export {
  default2 as BaseExample,
  MultimodalTextbox$1 as BaseMultimodalTextbox,
  Index as default
};
