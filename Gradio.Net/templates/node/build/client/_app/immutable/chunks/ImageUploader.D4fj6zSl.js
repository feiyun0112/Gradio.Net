import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, element, create_component, space, text, claim_element, claim_component, claim_space, claim_text, set_style, mount_component, listen, transition_in, transition_out, destroy_component, createEventDispatcher, toggle_class, src_url_equal, group_outros, check_outros, onMount, binding_callbacks, empty, add_render_callback, create_in_transition, stop_propagation, action_destroyer, run_all, ensure_array_like, destroy_each, set_input_value, set_data, bind, add_flush_callback, tick, bubble, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes } from "../../../svelte/svelte.js";
import { fade } from "../../../svelte/svelte-submodules.js";
import { h as prepare_files, w as Square, I as IconButton, C as Clear } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { I as Image } from "./Image.CMPoCWop.js";
import { W as Webcam$2, a as Spinner, S as SelectSource } from "./SelectSource.j1x9BYNI.js";
import { g as get_coordinates_of_clicked_image } from "./utils.avOzX0x9.js";
import { D as DropdownArrow } from "./DropdownArrow.Shy1tB9t.js";
import { S as StreamingBar } from "./StreamingBar.CkIfdne5.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
/* empty css                                                    */
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { I as Image$1 } from "./Image.eJ_qOnkr.js";
function create_fragment$5(ctx) {
  let svg;
  let path;
  let circle;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      circle = svg_element("circle");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true,
        class: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true });
      children(path).forEach(detach);
      circle = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true });
      children(circle).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z");
      attr(circle, "cx", "12");
      attr(circle, "cy", "13");
      attr(circle, "r", "4");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-camera");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
      append_hydration(svg, circle);
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
class Camera extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$5, safe_not_equal, {});
  }
}
function create_fragment$4(ctx) {
  let svg;
  let circle;
  return {
    c() {
      svg = svg_element("svg");
      circle = svg_element("circle");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        viewBox: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true,
        class: true
      });
      var svg_nodes = children(svg);
      circle = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true });
      children(circle).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(circle, "cx", "12");
      attr(circle, "cy", "12");
      attr(circle, "r", "10");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-circle");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, circle);
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
class Circle extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$4, safe_not_equal, {});
  }
}
function create_fragment$3(ctx) {
  let button;
  let div;
  let span;
  let webcam;
  let t0;
  let t1_value = "Click to Access Webcam";
  let t1;
  let current;
  let mounted;
  let dispose;
  webcam = new Webcam$2({});
  return {
    c() {
      button = element("button");
      div = element("div");
      span = element("span");
      create_component(webcam.$$.fragment);
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      div = claim_element(button_nodes, "DIV", { class: true });
      var div_nodes = children(div);
      span = claim_element(div_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      claim_component(webcam.$$.fragment, span_nodes);
      span_nodes.forEach(detach);
      t0 = claim_space(div_nodes);
      t1 = claim_text(div_nodes, t1_value);
      div_nodes.forEach(detach);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "icon-wrap svelte-qbrfs");
      attr(div, "class", "wrap svelte-qbrfs");
      attr(button, "class", "svelte-qbrfs");
      set_style(button, "height", `100%`);
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, div);
      append_hydration(div, span);
      mount_component(webcam, span, null);
      append_hydration(div, t0);
      append_hydration(div, t1);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler*/
          ctx[1]
        );
        mounted = true;
      }
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(webcam.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(webcam.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(webcam);
      mounted = false;
      dispose();
    }
  };
}
function instance$3($$self) {
  const dispatch = createEventDispatcher();
  const click_handler = () => dispatch("click");
  return [dispatch, click_handler];
}
class WebcamPermissions extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, {});
  }
}
function get_devices() {
  return navigator.mediaDevices.enumerateDevices();
}
function set_local_stream(local_stream, video_source) {
  video_source.srcObject = local_stream;
  video_source.muted = true;
  video_source.play();
}
async function get_video_stream(include_audio, video_source, device_id) {
  const size = {
    width: { ideal: 1920 },
    height: { ideal: 1440 }
  };
  const constraints = {
    video: device_id ? { deviceId: { exact: device_id }, ...size } : size,
    audio: include_audio
  };
  return navigator.mediaDevices.getUserMedia(constraints).then((local_stream) => {
    set_local_stream(local_stream, video_source);
    return local_stream;
  });
}
function set_available_devices(devices) {
  const cameras = devices.filter(
    (device) => device.kind === "videoinput"
  );
  return cameras;
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[37] = list[i];
  return child_ctx;
}
function create_else_block(ctx) {
  let div;
  let button;
  let current_block_type_index;
  let if_block0;
  let button_aria_label_value;
  let t0;
  let t1;
  let if_block2_anchor;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_4$1, create_else_block_3];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*mode*/
      ctx2[2] === "video" || /*streaming*/
      ctx2[1]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block1 = !/*recording*/
  ctx[11] && create_if_block_3$1(ctx);
  let if_block2 = (
    /*options_open*/
    ctx[13] && /*selected_device*/
    ctx[8] && create_if_block_1$1(ctx)
  );
  return {
    c() {
      div = element("div");
      button = element("button");
      if_block0.c();
      t0 = space();
      if (if_block1)
        if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      if_block2_anchor = empty();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      button = claim_element(div_nodes, "BUTTON", { "aria-label": true, class: true });
      var button_nodes = children(button);
      if_block0.l(button_nodes);
      button_nodes.forEach(detach);
      t0 = claim_space(div_nodes);
      if (if_block1)
        if_block1.l(div_nodes);
      div_nodes.forEach(detach);
      t1 = claim_space(nodes);
      if (if_block2)
        if_block2.l(nodes);
      if_block2_anchor = empty();
      this.h();
    },
    h() {
      attr(button, "aria-label", button_aria_label_value = /*mode*/
      ctx[2] === "image" ? "capture photo" : "start recording");
      attr(button, "class", "svelte-s8feoe");
      attr(div, "class", "button-wrap svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, button);
      if_blocks[current_block_type_index].m(button, null);
      append_hydration(div, t0);
      if (if_block1)
        if_block1.m(div, null);
      insert_hydration(target, t1, anchor);
      if (if_block2)
        if_block2.m(target, anchor);
      insert_hydration(target, if_block2_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*record_video_or_photo*/
          ctx[16]
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
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block0.c();
        } else {
          if_block0.p(ctx2, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(button, null);
      }
      if (!current || dirty[0] & /*mode*/
      4 && button_aria_label_value !== (button_aria_label_value = /*mode*/
      ctx2[2] === "image" ? "capture photo" : "start recording")) {
        attr(button, "aria-label", button_aria_label_value);
      }
      if (!/*recording*/
      ctx2[11]) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*recording*/
          2048) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_3$1(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div, null);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (
        /*options_open*/
        ctx2[13] && /*selected_device*/
        ctx2[8]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*options_open, selected_device*/
          8448) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block_1$1(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(if_block2_anchor.parentNode, if_block2_anchor);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t1);
        detach(if_block2_anchor);
      }
      if_blocks[current_block_type_index].d();
      if (if_block1)
        if_block1.d();
      if (if_block2)
        if_block2.d(detaching);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block$1(ctx) {
  let div;
  let webcampermissions;
  let div_intro;
  let current;
  webcampermissions = new WebcamPermissions({});
  webcampermissions.$on(
    "click",
    /*click_handler*/
    ctx[26]
  );
  return {
    c() {
      div = element("div");
      create_component(webcampermissions.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { title: true, style: true });
      var div_nodes = children(div);
      claim_component(webcampermissions.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "title", "grant webcam access");
      set_style(div, "height", "100%");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(webcampermissions, div, null);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(webcampermissions.$$.fragment, local);
      if (local) {
        if (!div_intro) {
          add_render_callback(() => {
            div_intro = create_in_transition(div, fade, { delay: 100, duration: 200 });
            div_intro.start();
          });
        }
      }
      current = true;
    },
    o(local) {
      transition_out(webcampermissions.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(webcampermissions);
    }
  };
}
function create_else_block_3(ctx) {
  let div;
  let camera;
  let current;
  camera = new Camera({});
  return {
    c() {
      div = element("div");
      create_component(camera.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, title: true });
      var div_nodes = children(div);
      claim_component(camera.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "icon svelte-s8feoe");
      attr(div, "title", "capture photo");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(camera, div, null);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(camera.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(camera.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(camera);
    }
  };
}
function create_if_block_4$1(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block_5, create_if_block_6, create_else_block_2];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    if (
      /*streaming*/
      ctx2[1] && /*stream_state*/
      ctx2[10] === "waiting"
    )
      return 0;
    if (
      /*streaming*/
      ctx2[1] && /*stream_state*/
      ctx2[10] === "open" || !/*streaming*/
      ctx2[1] && /*recording*/
      ctx2[11]
    )
      return 1;
    return 2;
  }
  current_block_type_index = select_block_type_2(ctx);
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
function create_else_block_2(ctx) {
  let div1;
  let div0;
  let circle;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[4]("audio.record") + ""
  );
  let t1;
  let current;
  circle = new Circle({});
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      create_component(circle.$$.fragment);
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true, title: true });
      var div0_nodes = children(div0);
      claim_component(circle.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      t0 = claim_space(div1_nodes);
      t1 = claim_text(div1_nodes, t1_value);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "icon color-primary svelte-s8feoe");
      attr(div0, "title", "start recording");
      attr(div1, "class", "icon-with-text svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      mount_component(circle, div0, null);
      append_hydration(div1, t0);
      append_hydration(div1, t1);
      current = true;
    },
    p(ctx2, dirty) {
      if ((!current || dirty[0] & /*i18n*/
      16) && t1_value !== (t1_value = /*i18n*/
      ctx2[4]("audio.record") + ""))
        set_data(t1, t1_value);
    },
    i(local) {
      if (current)
        return;
      transition_in(circle.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(circle.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      destroy_component(circle);
    }
  };
}
function create_if_block_6(ctx) {
  let div1;
  let div0;
  let square;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[4]("audio.stop") + ""
  );
  let t1;
  let current;
  square = new Square({});
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      create_component(square.$$.fragment);
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true, title: true });
      var div0_nodes = children(div0);
      claim_component(square.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      t0 = claim_space(div1_nodes);
      t1 = claim_text(div1_nodes, t1_value);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "icon color-primary svelte-s8feoe");
      attr(div0, "title", "stop recording");
      attr(div1, "class", "icon-with-text svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      mount_component(square, div0, null);
      append_hydration(div1, t0);
      append_hydration(div1, t1);
      current = true;
    },
    p(ctx2, dirty) {
      if ((!current || dirty[0] & /*i18n*/
      16) && t1_value !== (t1_value = /*i18n*/
      ctx2[4]("audio.stop") + ""))
        set_data(t1, t1_value);
    },
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
      if (detaching) {
        detach(div1);
      }
      destroy_component(square);
    }
  };
}
function create_if_block_5(ctx) {
  let div1;
  let div0;
  let spinner;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[4]("audio.waiting") + ""
  );
  let t1;
  let current;
  spinner = new Spinner({});
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      create_component(spinner.$$.fragment);
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true, style: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true, title: true });
      var div0_nodes = children(div0);
      claim_component(spinner.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      t0 = claim_space(div1_nodes);
      t1 = claim_text(div1_nodes, t1_value);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "icon color-primary svelte-s8feoe");
      attr(div0, "title", "spinner");
      attr(div1, "class", "icon-with-text svelte-s8feoe");
      set_style(div1, "width", "var(--size-24)");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      mount_component(spinner, div0, null);
      append_hydration(div1, t0);
      append_hydration(div1, t1);
      current = true;
    },
    p(ctx2, dirty) {
      if ((!current || dirty[0] & /*i18n*/
      16) && t1_value !== (t1_value = /*i18n*/
      ctx2[4]("audio.waiting") + ""))
        set_data(t1, t1_value);
    },
    i(local) {
      if (current)
        return;
      transition_in(spinner.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(spinner.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      destroy_component(spinner);
    }
  };
}
function create_if_block_3$1(ctx) {
  let button;
  let dropdownarrow;
  let current;
  let mounted;
  let dispose;
  dropdownarrow = new DropdownArrow({});
  return {
    c() {
      button = element("button");
      create_component(dropdownarrow.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(dropdownarrow.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "icon svelte-s8feoe");
      attr(button, "aria-label", "select input source");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(dropdownarrow, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_1*/
          ctx[27]
        );
        mounted = true;
      }
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(dropdownarrow.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(dropdownarrow.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(dropdownarrow);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_1$1(ctx) {
  let select;
  let button;
  let dropdownarrow;
  let t;
  let current;
  let mounted;
  let dispose;
  dropdownarrow = new DropdownArrow({});
  function select_block_type_3(ctx2, dirty) {
    if (
      /*available_video_devices*/
      ctx2[7].length === 0
    )
      return create_if_block_2$1;
    return create_else_block_1;
  }
  let current_block_type = select_block_type_3(ctx);
  let if_block = current_block_type(ctx);
  return {
    c() {
      select = element("select");
      button = element("button");
      create_component(dropdownarrow.$$.fragment);
      t = space();
      if_block.c();
      this.h();
    },
    l(nodes) {
      select = claim_element(nodes, "SELECT", { class: true, "aria-label": true });
      var select_nodes = children(select);
      button = claim_element(select_nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      claim_component(dropdownarrow.$$.fragment, button_nodes);
      t = claim_space(button_nodes);
      button_nodes.forEach(detach);
      if_block.l(select_nodes);
      select_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "inset-icon svelte-s8feoe");
      attr(select, "class", "select-wrap svelte-s8feoe");
      attr(select, "aria-label", "select source");
    },
    m(target, anchor) {
      insert_hydration(target, select, anchor);
      append_hydration(select, button);
      mount_component(dropdownarrow, button, null);
      append_hydration(button, t);
      if_block.m(select, null);
      current = true;
      if (!mounted) {
        dispose = [
          listen(button, "click", stop_propagation(
            /*click_handler_2*/
            ctx[28]
          )),
          action_destroyer(click_outside.call(
            null,
            select,
            /*handle_click_outside*/
            ctx[17]
          )),
          listen(
            select,
            "change",
            /*handle_device_change*/
            ctx[14]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (current_block_type === (current_block_type = select_block_type_3(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(select, null);
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(dropdownarrow.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(dropdownarrow.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(select);
      }
      destroy_component(dropdownarrow);
      if_block.d();
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_else_block_1(ctx) {
  let each_1_anchor;
  let each_value = ensure_array_like(
    /*available_video_devices*/
    ctx[7]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
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
      if (dirty[0] & /*available_video_devices, selected_device*/
      384) {
        each_value = ensure_array_like(
          /*available_video_devices*/
          ctx2[7]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(each_1_anchor.parentNode, each_1_anchor);
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
        detach(each_1_anchor);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block_2$1(ctx) {
  let option;
  let t_value = (
    /*i18n*/
    ctx[4]("common.no_devices") + ""
  );
  let t;
  return {
    c() {
      option = element("option");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      option = claim_element(nodes, "OPTION", { class: true });
      var option_nodes = children(option);
      t = claim_text(option_nodes, t_value);
      option_nodes.forEach(detach);
      this.h();
    },
    h() {
      option.__value = "";
      set_input_value(option, option.__value);
      attr(option, "class", "svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, option, anchor);
      append_hydration(option, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*i18n*/
      16 && t_value !== (t_value = /*i18n*/
      ctx2[4]("common.no_devices") + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(option);
      }
    }
  };
}
function create_each_block(ctx) {
  let option;
  let t0_value = (
    /*device*/
    ctx[37].label + ""
  );
  let t0;
  let t1;
  let option_value_value;
  let option_selected_value;
  return {
    c() {
      option = element("option");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      option = claim_element(nodes, "OPTION", { class: true });
      var option_nodes = children(option);
      t0 = claim_text(option_nodes, t0_value);
      t1 = claim_space(option_nodes);
      option_nodes.forEach(detach);
      this.h();
    },
    h() {
      option.__value = option_value_value = /*device*/
      ctx[37].deviceId;
      set_input_value(option, option.__value);
      option.selected = option_selected_value = /*selected_device*/
      ctx[8].deviceId === /*device*/
      ctx[37].deviceId;
      attr(option, "class", "svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, option, anchor);
      append_hydration(option, t0);
      append_hydration(option, t1);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*available_video_devices*/
      128 && t0_value !== (t0_value = /*device*/
      ctx2[37].label + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*available_video_devices*/
      128 && option_value_value !== (option_value_value = /*device*/
      ctx2[37].deviceId)) {
        option.__value = option_value_value;
        set_input_value(option, option.__value);
      }
      if (dirty[0] & /*selected_device, available_video_devices*/
      384 && option_selected_value !== (option_selected_value = /*selected_device*/
      ctx2[8].deviceId === /*device*/
      ctx2[37].deviceId)) {
        option.selected = option_selected_value;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(option);
      }
    }
  };
}
function create_fragment$2(ctx) {
  let div;
  let streamingbar;
  let t0;
  let video;
  let t1;
  let img;
  let img_src_value;
  let t2;
  let current_block_type_index;
  let if_block;
  let current;
  streamingbar = new StreamingBar({
    props: { time_limit: (
      /*time_limit*/
      ctx[9]
    ) }
  });
  const if_block_creators = [create_if_block$1, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*webcam_accessed*/
    ctx2[12])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      div = element("div");
      create_component(streamingbar.$$.fragment);
      t0 = space();
      video = element("video");
      t1 = space();
      img = element("img");
      t2 = space();
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(streamingbar.$$.fragment, div_nodes);
      t0 = claim_space(div_nodes);
      video = claim_element(div_nodes, "VIDEO", { class: true });
      children(video).forEach(detach);
      t1 = claim_space(div_nodes);
      img = claim_element(div_nodes, "IMG", { src: true, class: true });
      t2 = claim_space(div_nodes);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      var _a;
      attr(video, "class", "svelte-s8feoe");
      toggle_class(
        video,
        "flip",
        /*mirror_webcam*/
        ctx[3]
      );
      toggle_class(video, "hide", !/*webcam_accessed*/
      ctx[12] || /*webcam_accessed*/
      ctx[12] && !!/*value*/
      ctx[0]);
      if (!src_url_equal(img.src, img_src_value = /*value*/
      (_a = ctx[0]) == null ? void 0 : _a.url))
        attr(img, "src", img_src_value);
      attr(img, "class", "svelte-s8feoe");
      toggle_class(img, "hide", !/*webcam_accessed*/
      ctx[12] || /*webcam_accessed*/
      ctx[12] && !/*value*/
      ctx[0]);
      attr(div, "class", "wrap svelte-s8feoe");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(streamingbar, div, null);
      append_hydration(div, t0);
      append_hydration(div, video);
      ctx[25](video);
      append_hydration(div, t1);
      append_hydration(div, img);
      append_hydration(div, t2);
      if_blocks[current_block_type_index].m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      var _a;
      const streamingbar_changes = {};
      if (dirty[0] & /*time_limit*/
      512)
        streamingbar_changes.time_limit = /*time_limit*/
        ctx2[9];
      streamingbar.$set(streamingbar_changes);
      if (!current || dirty[0] & /*mirror_webcam*/
      8) {
        toggle_class(
          video,
          "flip",
          /*mirror_webcam*/
          ctx2[3]
        );
      }
      if (!current || dirty[0] & /*webcam_accessed, value*/
      4097) {
        toggle_class(video, "hide", !/*webcam_accessed*/
        ctx2[12] || /*webcam_accessed*/
        ctx2[12] && !!/*value*/
        ctx2[0]);
      }
      if (!current || dirty[0] & /*value*/
      1 && !src_url_equal(img.src, img_src_value = /*value*/
      (_a = ctx2[0]) == null ? void 0 : _a.url)) {
        attr(img, "src", img_src_value);
      }
      if (!current || dirty[0] & /*webcam_accessed, value*/
      4097) {
        toggle_class(img, "hide", !/*webcam_accessed*/
        ctx2[12] || /*webcam_accessed*/
        ctx2[12] && !/*value*/
        ctx2[0]);
      }
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
      transition_in(streamingbar.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(streamingbar.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(streamingbar);
      ctx[25](null);
      if_blocks[current_block_type_index].d();
    }
  };
}
function click_outside(node, cb) {
  const handle_click = (event) => {
    if (node && !node.contains(event.target) && !event.defaultPrevented) {
      cb(event);
    }
  };
  document.addEventListener("click", handle_click, true);
  return {
    destroy() {
      document.removeEventListener("click", handle_click, true);
    }
  };
}
function instance$2($$self, $$props, $$invalidate) {
  let video_source;
  let available_video_devices = [];
  let selected_device = null;
  let time_limit = null;
  let stream_state = "closed";
  const modify_stream = (state) => {
    if (state === "closed") {
      $$invalidate(9, time_limit = null);
      $$invalidate(10, stream_state = "closed");
      $$invalidate(0, value = null);
    } else if (state === "waiting") {
      $$invalidate(10, stream_state = "waiting");
    } else {
      $$invalidate(10, stream_state = "open");
    }
  };
  const set_time_limit = (time) => {
    if (recording)
      $$invalidate(9, time_limit = time);
  };
  let canvas;
  let { streaming = false } = $$props;
  let { pending = false } = $$props;
  let { root = "" } = $$props;
  let { stream_every = 1 } = $$props;
  let { mode = "image" } = $$props;
  let { mirror_webcam } = $$props;
  let { include_audio } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  let { value = null } = $$props;
  const dispatch = createEventDispatcher();
  onMount(() => {
    canvas = document.createElement("canvas");
    if (streaming && mode === "image") {
      window.setInterval(
        () => {
          if (video_source && !pending) {
            take_picture();
          }
        },
        stream_every * 1e3
      );
    }
  });
  const handle_device_change = async (event) => {
    const target = event.target;
    const device_id = target.value;
    await get_video_stream(include_audio, video_source, device_id).then(async (local_stream) => {
      stream = local_stream;
      $$invalidate(8, selected_device = available_video_devices.find((device) => device.deviceId === device_id) || null);
      $$invalidate(13, options_open = false);
    });
  };
  async function access_webcam() {
    try {
      get_video_stream(include_audio, video_source).then(async (local_stream) => {
        $$invalidate(12, webcam_accessed = true);
        $$invalidate(7, available_video_devices = await get_devices());
        stream = local_stream;
      }).then(() => set_available_devices(available_video_devices)).then((devices) => {
        $$invalidate(7, available_video_devices = devices);
        const used_devices = stream.getTracks().map((track) => {
          var _a;
          return (_a = track.getSettings()) == null ? void 0 : _a.deviceId;
        })[0];
        $$invalidate(8, selected_device = used_devices ? devices.find((device) => device.deviceId === used_devices) || available_video_devices[0] : available_video_devices[0]);
      });
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        dispatch("error", i18n("image.no_webcam_support"));
      }
    } catch (err) {
      if (err instanceof DOMException && err.name == "NotAllowedError") {
        dispatch("error", i18n("image.allow_webcam_access"));
      } else {
        throw err;
      }
    }
  }
  function take_picture() {
    var context = canvas.getContext("2d");
    if ((!streaming || streaming && recording) && video_source.videoWidth && video_source.videoHeight) {
      canvas.width = video_source.videoWidth;
      canvas.height = video_source.videoHeight;
      context.drawImage(video_source, 0, 0, video_source.videoWidth, video_source.videoHeight);
      if (mirror_webcam) {
        context.scale(-1, 1);
        context.drawImage(video_source, -video_source.videoWidth, 0);
      }
      if (streaming && (!recording || stream_state === "waiting")) {
        return;
      }
      if (streaming) {
        const image_data = canvas.toDataURL("image/jpeg");
        dispatch("stream", image_data);
        return;
      }
      canvas.toBlob(
        (blob) => {
          dispatch(streaming ? "stream" : "capture", blob);
        },
        `image/${streaming ? "jpeg" : "png"}`,
        0.8
      );
    }
  }
  let recording = false;
  let recorded_blobs = [];
  let stream;
  let mimeType;
  let media_recorder;
  function take_recording() {
    if (recording) {
      media_recorder.stop();
      let video_blob = new Blob(recorded_blobs, { type: mimeType });
      let ReaderObj = new FileReader();
      ReaderObj.onload = async function(e) {
        var _a;
        if (e.target) {
          let _video_blob = new File([video_blob], "sample." + mimeType.substring(6));
          const val = await prepare_files([_video_blob]);
          let val_ = (_a = await upload(val, root)) == null ? void 0 : _a.filter(Boolean)[0];
          dispatch("capture", val_);
          dispatch("stop_recording");
        }
      };
      ReaderObj.readAsDataURL(video_blob);
    } else {
      dispatch("start_recording");
      recorded_blobs = [];
      let validMimeTypes = ["video/webm", "video/mp4"];
      for (let validMimeType of validMimeTypes) {
        if (MediaRecorder.isTypeSupported(validMimeType)) {
          mimeType = validMimeType;
          break;
        }
      }
      if (mimeType === null) {
        console.error("No supported MediaRecorder mimeType");
        return;
      }
      media_recorder = new MediaRecorder(stream, { mimeType });
      media_recorder.addEventListener("dataavailable", function(e) {
        recorded_blobs.push(e.data);
      });
      media_recorder.start(200);
    }
    $$invalidate(11, recording = !recording);
  }
  let webcam_accessed = false;
  function record_video_or_photo() {
    if (mode === "image" && streaming) {
      $$invalidate(11, recording = !recording);
    }
    if (mode === "image") {
      take_picture();
    } else {
      take_recording();
    }
    if (!recording && stream) {
      dispatch("close_stream");
      stream.getTracks().forEach((track) => track.stop());
      $$invalidate(6, video_source.srcObject = null, video_source);
      $$invalidate(12, webcam_accessed = false);
      window.setTimeout(
        () => {
          $$invalidate(0, value = null);
        },
        500
      );
      $$invalidate(0, value = null);
    }
  }
  let options_open = false;
  function handle_click_outside(event) {
    event.preventDefault();
    event.stopPropagation();
    $$invalidate(13, options_open = false);
  }
  function video_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      video_source = $$value;
      $$invalidate(6, video_source);
    });
  }
  const click_handler = async () => access_webcam();
  const click_handler_1 = () => $$invalidate(13, options_open = true);
  const click_handler_2 = () => $$invalidate(13, options_open = false);
  $$self.$$set = ($$props2) => {
    if ("streaming" in $$props2)
      $$invalidate(1, streaming = $$props2.streaming);
    if ("pending" in $$props2)
      $$invalidate(20, pending = $$props2.pending);
    if ("root" in $$props2)
      $$invalidate(21, root = $$props2.root);
    if ("stream_every" in $$props2)
      $$invalidate(22, stream_every = $$props2.stream_every);
    if ("mode" in $$props2)
      $$invalidate(2, mode = $$props2.mode);
    if ("mirror_webcam" in $$props2)
      $$invalidate(3, mirror_webcam = $$props2.mirror_webcam);
    if ("include_audio" in $$props2)
      $$invalidate(23, include_audio = $$props2.include_audio);
    if ("i18n" in $$props2)
      $$invalidate(4, i18n = $$props2.i18n);
    if ("upload" in $$props2)
      $$invalidate(24, upload = $$props2.upload);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
  };
  return [
    value,
    streaming,
    mode,
    mirror_webcam,
    i18n,
    click_outside,
    video_source,
    available_video_devices,
    selected_device,
    time_limit,
    stream_state,
    recording,
    webcam_accessed,
    options_open,
    handle_device_change,
    access_webcam,
    record_video_or_photo,
    handle_click_outside,
    modify_stream,
    set_time_limit,
    pending,
    root,
    stream_every,
    include_audio,
    upload,
    video_binding,
    click_handler,
    click_handler_1,
    click_handler_2
  ];
}
class Webcam extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$2,
      create_fragment$2,
      safe_not_equal,
      {
        modify_stream: 18,
        set_time_limit: 19,
        streaming: 1,
        pending: 20,
        root: 21,
        stream_every: 22,
        mode: 2,
        mirror_webcam: 3,
        include_audio: 23,
        i18n: 4,
        upload: 24,
        value: 0,
        click_outside: 5
      },
      null,
      [-1, -1]
    );
  }
  get modify_stream() {
    return this.$$.ctx[18];
  }
  get set_time_limit() {
    return this.$$.ctx[19];
  }
  get click_outside() {
    return click_outside;
  }
}
const Webcam$1 = Webcam;
function create_default_slot$1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: { Icon: Clear, label: "Remove Image" }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[1]
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
    p: noop,
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
  let iconbuttonwrapper;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope*/
      4) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbuttonwrapper, detaching);
    }
  };
}
function instance$1($$self) {
  const dispatch = createEventDispatcher();
  const click_handler = (event) => {
    dispatch("remove_image");
    event.stopPropagation();
  };
  return [dispatch, click_handler];
}
class ClearImage extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {});
  }
}
function create_if_block_4(ctx) {
  let clearimage;
  let current;
  clearimage = new ClearImage({});
  clearimage.$on(
    "remove_image",
    /*remove_image_handler*/
    ctx[28]
  );
  return {
    c() {
      create_component(clearimage.$$.fragment);
    },
    l(nodes) {
      claim_component(clearimage.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(clearimage, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(clearimage.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(clearimage.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(clearimage, detaching);
    }
  };
}
function create_if_block_3(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[27].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[42],
    null
  );
  return {
    c() {
      if (default_slot)
        default_slot.c();
    },
    l(nodes) {
      if (default_slot)
        default_slot.l(nodes);
    },
    m(target, anchor) {
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty[1] & /*$$scope*/
        2048)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[42],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[42]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[42],
              dirty,
              null
            ),
            null
          );
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(default_slot, local);
      current = true;
    },
    o(local) {
      transition_out(default_slot, local);
      current = false;
    },
    d(detaching) {
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_default_slot(ctx) {
  let if_block_anchor;
  let current;
  let if_block = (
    /*value*/
    ctx[3] === null && create_if_block_3(ctx)
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*value*/
        ctx2[3] === null
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*value*/
          8) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_3(ctx2);
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
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let div;
  let image;
  let current;
  let mounted;
  let dispose;
  image = new Image$1({
    props: {
      src: (
        /*value*/
        ctx[3].url
      ),
      alt: (
        /*value*/
        ctx[3].alt_text
      )
    }
  });
  return {
    c() {
      div = element("div");
      create_component(image.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(image.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "image-frame svelte-1ti4ehe");
      toggle_class(
        div,
        "selectable",
        /*selectable*/
        ctx[11]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(image, div, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          div,
          "click",
          /*handle_click*/
          ctx[24]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const image_changes = {};
      if (dirty[0] & /*value*/
      8)
        image_changes.src = /*value*/
        ctx2[3].url;
      if (dirty[0] & /*value*/
      8)
        image_changes.alt = /*value*/
        ctx2[3].alt_text;
      image.$set(image_changes);
      if (!current || dirty[0] & /*selectable*/
      2048) {
        toggle_class(
          div,
          "selectable",
          /*selectable*/
          ctx2[11]
        );
      }
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
      if (detaching) {
        detach(div);
      }
      destroy_component(image);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_1(ctx) {
  let webcam;
  let updating_modify_stream;
  let updating_set_time_limit;
  let current;
  function webcam_modify_stream_binding(value) {
    ctx[33](value);
  }
  function webcam_set_time_limit_binding(value) {
    ctx[34](value);
  }
  let webcam_props = {
    root: (
      /*root*/
      ctx[12]
    ),
    value: (
      /*value*/
      ctx[3]
    ),
    mirror_webcam: (
      /*mirror_webcam*/
      ctx[10]
    ),
    stream_every: (
      /*stream_every*/
      ctx[17]
    ),
    streaming: (
      /*streaming*/
      ctx[9]
    ),
    mode: "image",
    include_audio: false,
    i18n: (
      /*i18n*/
      ctx[13]
    ),
    upload: (
      /*upload*/
      ctx[15]
    )
  };
  if (
    /*modify_stream*/
    ctx[4] !== void 0
  ) {
    webcam_props.modify_stream = /*modify_stream*/
    ctx[4];
  }
  if (
    /*set_time_limit*/
    ctx[5] !== void 0
  ) {
    webcam_props.set_time_limit = /*set_time_limit*/
    ctx[5];
  }
  webcam = new Webcam$1({ props: webcam_props });
  binding_callbacks.push(() => bind(webcam, "modify_stream", webcam_modify_stream_binding));
  binding_callbacks.push(() => bind(webcam, "set_time_limit", webcam_set_time_limit_binding));
  webcam.$on(
    "capture",
    /*capture_handler*/
    ctx[35]
  );
  webcam.$on(
    "stream",
    /*stream_handler_1*/
    ctx[36]
  );
  webcam.$on(
    "error",
    /*error_handler_1*/
    ctx[37]
  );
  webcam.$on(
    "drag",
    /*drag_handler*/
    ctx[38]
  );
  webcam.$on(
    "upload",
    /*upload_handler*/
    ctx[39]
  );
  webcam.$on(
    "close_stream",
    /*close_stream_handler*/
    ctx[40]
  );
  return {
    c() {
      create_component(webcam.$$.fragment);
    },
    l(nodes) {
      claim_component(webcam.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(webcam, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const webcam_changes = {};
      if (dirty[0] & /*root*/
      4096)
        webcam_changes.root = /*root*/
        ctx2[12];
      if (dirty[0] & /*value*/
      8)
        webcam_changes.value = /*value*/
        ctx2[3];
      if (dirty[0] & /*mirror_webcam*/
      1024)
        webcam_changes.mirror_webcam = /*mirror_webcam*/
        ctx2[10];
      if (dirty[0] & /*stream_every*/
      131072)
        webcam_changes.stream_every = /*stream_every*/
        ctx2[17];
      if (dirty[0] & /*streaming*/
      512)
        webcam_changes.streaming = /*streaming*/
        ctx2[9];
      if (dirty[0] & /*i18n*/
      8192)
        webcam_changes.i18n = /*i18n*/
        ctx2[13];
      if (dirty[0] & /*upload*/
      32768)
        webcam_changes.upload = /*upload*/
        ctx2[15];
      if (!updating_modify_stream && dirty[0] & /*modify_stream*/
      16) {
        updating_modify_stream = true;
        webcam_changes.modify_stream = /*modify_stream*/
        ctx2[4];
        add_flush_callback(() => updating_modify_stream = false);
      }
      if (!updating_set_time_limit && dirty[0] & /*set_time_limit*/
      32) {
        updating_set_time_limit = true;
        webcam_changes.set_time_limit = /*set_time_limit*/
        ctx2[5];
        add_flush_callback(() => updating_set_time_limit = false);
      }
      webcam.$set(webcam_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(webcam.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(webcam.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(webcam, detaching);
    }
  };
}
function create_if_block(ctx) {
  let selectsource;
  let updating_active_source;
  let current;
  function selectsource_active_source_binding(value) {
    ctx[41](value);
  }
  let selectsource_props = {
    sources: (
      /*sources*/
      ctx[8]
    ),
    handle_clear: (
      /*handle_clear*/
      ctx[21]
    ),
    handle_select: (
      /*handle_select_source*/
      ctx[25]
    )
  };
  if (
    /*active_source*/
    ctx[1] !== void 0
  ) {
    selectsource_props.active_source = /*active_source*/
    ctx[1];
  }
  selectsource = new SelectSource({ props: selectsource_props });
  binding_callbacks.push(() => bind(selectsource, "active_source", selectsource_active_source_binding));
  return {
    c() {
      create_component(selectsource.$$.fragment);
    },
    l(nodes) {
      claim_component(selectsource.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(selectsource, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const selectsource_changes = {};
      if (dirty[0] & /*sources*/
      256)
        selectsource_changes.sources = /*sources*/
        ctx2[8];
      if (!updating_active_source && dirty[0] & /*active_source*/
      2) {
        updating_active_source = true;
        selectsource_changes.active_source = /*active_source*/
        ctx2[1];
        add_flush_callback(() => updating_active_source = false);
      }
      selectsource.$set(selectsource_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(selectsource.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(selectsource.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(selectsource, detaching);
    }
  };
}
function create_fragment(ctx) {
  var _a;
  let blocklabel;
  let t0;
  let div1;
  let t1;
  let div0;
  let upload_1;
  let updating_uploading;
  let updating_dragging;
  let t2;
  let current_block_type_index;
  let if_block1;
  let t3;
  let show_if = (
    /*sources*/
    ctx[8].length > 1 || /*sources*/
    ctx[8].includes("clipboard")
  );
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[7]
      ),
      Icon: Image,
      label: (
        /*label*/
        ctx[6] || "Image"
      )
    }
  });
  let if_block0 = (
    /*value*/
    ((_a = ctx[3]) == null ? void 0 : _a.url) && !/*active_streaming*/
    ctx[18] && create_if_block_4(ctx)
  );
  function upload_1_uploading_binding(value) {
    ctx[30](value);
  }
  function upload_1_dragging_binding(value) {
    ctx[31](value);
  }
  let upload_1_props = {
    hidden: (
      /*value*/
      ctx[3] !== null || /*active_source*/
      ctx[1] === "webcam"
    ),
    filetype: (
      /*active_source*/
      ctx[1] === "clipboard" ? "clipboard" : "image/*"
    ),
    root: (
      /*root*/
      ctx[12]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[14]
    ),
    disable_click: !/*sources*/
    ctx[8].includes("upload") || /*value*/
    ctx[3] !== null,
    upload: (
      /*upload*/
      ctx[15]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[16]
    ),
    $$slots: { default: [create_default_slot] },
    $$scope: { ctx }
  };
  if (
    /*uploading*/
    ctx[0] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[0];
  }
  if (
    /*dragging*/
    ctx[2] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[2];
  }
  upload_1 = new Upload({ props: upload_1_props });
  ctx[29](upload_1);
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  upload_1.$on(
    "load",
    /*handle_upload*/
    ctx[20]
  );
  upload_1.$on(
    "error",
    /*error_handler*/
    ctx[32]
  );
  const if_block_creators = [create_if_block_1, create_if_block_2];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*active_source*/
      ctx2[1] === "webcam" && /*streaming*/
      (ctx2[9] || !/*streaming*/
      ctx2[9] && !/*value*/
      ctx2[3])
    )
      return 0;
    if (
      /*value*/
      ctx2[3] !== null && !/*streaming*/
      ctx2[9]
    )
      return 1;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
    if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  let if_block2 = show_if && create_if_block(ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t0 = space();
      div1 = element("div");
      if (if_block0)
        if_block0.c();
      t1 = space();
      div0 = element("div");
      create_component(upload_1.$$.fragment);
      t2 = space();
      if (if_block1)
        if_block1.c();
      t3 = space();
      if (if_block2)
        if_block2.c();
      this.h();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t0 = claim_space(nodes);
      div1 = claim_element(nodes, "DIV", { "data-testid": true, class: true });
      var div1_nodes = children(div1);
      if (if_block0)
        if_block0.l(div1_nodes);
      t1 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      claim_component(upload_1.$$.fragment, div0_nodes);
      t2 = claim_space(div0_nodes);
      if (if_block1)
        if_block1.l(div0_nodes);
      div0_nodes.forEach(detach);
      t3 = claim_space(div1_nodes);
      if (if_block2)
        if_block2.l(div1_nodes);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "upload-container svelte-1ti4ehe");
      toggle_class(
        div0,
        "reduced-height",
        /*sources*/
        ctx[8].length > 1
      );
      set_style(
        div0,
        "width",
        /*value*/
        ctx[3] ? "auto" : "100%"
      );
      attr(div1, "data-testid", "image");
      attr(div1, "class", "image-container svelte-1ti4ehe");
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, div1, anchor);
      if (if_block0)
        if_block0.m(div1, null);
      append_hydration(div1, t1);
      append_hydration(div1, div0);
      mount_component(upload_1, div0, null);
      append_hydration(div0, t2);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(div0, null);
      }
      append_hydration(div1, t3);
      if (if_block2)
        if_block2.m(div1, null);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      128)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*label*/
      64)
        blocklabel_changes.label = /*label*/
        ctx2[6] || "Image";
      blocklabel.$set(blocklabel_changes);
      if (
        /*value*/
        ((_a2 = ctx2[3]) == null ? void 0 : _a2.url) && !/*active_streaming*/
        ctx2[18]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*value, active_streaming*/
          262152) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div1, t1);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      const upload_1_changes = {};
      if (dirty[0] & /*value, active_source*/
      10)
        upload_1_changes.hidden = /*value*/
        ctx2[3] !== null || /*active_source*/
        ctx2[1] === "webcam";
      if (dirty[0] & /*active_source*/
      2)
        upload_1_changes.filetype = /*active_source*/
        ctx2[1] === "clipboard" ? "clipboard" : "image/*";
      if (dirty[0] & /*root*/
      4096)
        upload_1_changes.root = /*root*/
        ctx2[12];
      if (dirty[0] & /*max_file_size*/
      16384)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[14];
      if (dirty[0] & /*sources, value*/
      264)
        upload_1_changes.disable_click = !/*sources*/
        ctx2[8].includes("upload") || /*value*/
        ctx2[3] !== null;
      if (dirty[0] & /*upload*/
      32768)
        upload_1_changes.upload = /*upload*/
        ctx2[15];
      if (dirty[0] & /*stream_handler*/
      65536)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[16];
      if (dirty[0] & /*value*/
      8 | dirty[1] & /*$$scope*/
      2048) {
        upload_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      1) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[0];
        add_flush_callback(() => updating_uploading = false);
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      4) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[2];
        add_flush_callback(() => updating_dragging = false);
      }
      upload_1.$set(upload_1_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx2, dirty);
        }
      } else {
        if (if_block1) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block1 = if_blocks[current_block_type_index];
          if (!if_block1) {
            if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
            if_block1.c();
          } else {
            if_block1.p(ctx2, dirty);
          }
          transition_in(if_block1, 1);
          if_block1.m(div0, null);
        } else {
          if_block1 = null;
        }
      }
      if (!current || dirty[0] & /*sources*/
      256) {
        toggle_class(
          div0,
          "reduced-height",
          /*sources*/
          ctx2[8].length > 1
        );
      }
      if (dirty[0] & /*value*/
      8) {
        set_style(
          div0,
          "width",
          /*value*/
          ctx2[3] ? "auto" : "100%"
        );
      }
      if (dirty[0] & /*sources*/
      256)
        show_if = /*sources*/
        ctx2[8].length > 1 || /*sources*/
        ctx2[8].includes("clipboard");
      if (show_if) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*sources*/
          256) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(div1, null);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block0);
      transition_in(upload_1.$$.fragment, local);
      transition_in(if_block1);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block0);
      transition_out(upload_1.$$.fragment, local);
      transition_out(if_block1);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(div1);
      }
      destroy_component(blocklabel, detaching);
      if (if_block0)
        if_block0.d();
      ctx[29](null);
      destroy_component(upload_1);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
      if (if_block2)
        if_block2.d();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let active_streaming;
  let { $$slots: slots = {}, $$scope } = $$props;
  let { value = null } = $$props;
  let { label = void 0 } = $$props;
  let { show_label } = $$props;
  let { sources = ["upload", "clipboard", "webcam"] } = $$props;
  let { streaming = false } = $$props;
  let { pending = false } = $$props;
  let { mirror_webcam } = $$props;
  let { selectable = false } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { stream_every } = $$props;
  let { modify_stream } = $$props;
  let { set_time_limit } = $$props;
  let upload_input;
  let { uploading = false } = $$props;
  let { active_source = null } = $$props;
  function handle_upload({ detail }) {
    if (!streaming) {
      $$invalidate(3, value = detail);
      dispatch("upload");
    }
  }
  function handle_clear() {
    $$invalidate(3, value = null);
    dispatch("clear");
    dispatch("change", null);
  }
  async function handle_save(img_blob, event) {
    if (event === "stream") {
      dispatch("stream", {
        value: { url: img_blob },
        is_value_data: true
      });
      return;
    }
    $$invalidate(26, pending = true);
    const f = await upload_input.load_files([new File([img_blob], `image/${streaming ? "jpeg" : "png"}`)]);
    if (event === "change" || event === "upload") {
      $$invalidate(3, value = (f == null ? void 0 : f[0]) || null);
      await tick();
      dispatch("change");
    }
    $$invalidate(26, pending = false);
  }
  const dispatch = createEventDispatcher();
  let { dragging = false } = $$props;
  function handle_click(evt) {
    let coordinates = get_coordinates_of_clicked_image(evt);
    if (coordinates) {
      dispatch("select", { index: coordinates, value: null });
    }
  }
  async function handle_select_source(source) {
    switch (source) {
      case "clipboard":
        upload_input.paste_clipboard();
        break;
    }
  }
  const remove_image_handler = () => {
    $$invalidate(3, value = null);
    dispatch("clear");
  };
  function upload_1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      upload_input = $$value;
      $$invalidate(19, upload_input);
    });
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(0, uploading);
  }
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(2, dragging);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function webcam_modify_stream_binding(value2) {
    modify_stream = value2;
    $$invalidate(4, modify_stream);
  }
  function webcam_set_time_limit_binding(value2) {
    set_time_limit = value2;
    $$invalidate(5, set_time_limit);
  }
  const capture_handler = (e) => handle_save(e.detail, "change");
  const stream_handler_1 = (e) => handle_save(e.detail, "stream");
  function error_handler_1(event) {
    bubble.call(this, $$self, event);
  }
  function drag_handler(event) {
    bubble.call(this, $$self, event);
  }
  const upload_handler = (e) => handle_save(e.detail, "upload");
  function close_stream_handler(event) {
    bubble.call(this, $$self, event);
  }
  function selectsource_active_source_binding(value2) {
    active_source = value2;
    $$invalidate(1, active_source), $$invalidate(8, sources);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(6, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("sources" in $$props2)
      $$invalidate(8, sources = $$props2.sources);
    if ("streaming" in $$props2)
      $$invalidate(9, streaming = $$props2.streaming);
    if ("pending" in $$props2)
      $$invalidate(26, pending = $$props2.pending);
    if ("mirror_webcam" in $$props2)
      $$invalidate(10, mirror_webcam = $$props2.mirror_webcam);
    if ("selectable" in $$props2)
      $$invalidate(11, selectable = $$props2.selectable);
    if ("root" in $$props2)
      $$invalidate(12, root = $$props2.root);
    if ("i18n" in $$props2)
      $$invalidate(13, i18n = $$props2.i18n);
    if ("max_file_size" in $$props2)
      $$invalidate(14, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(15, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(16, stream_handler = $$props2.stream_handler);
    if ("stream_every" in $$props2)
      $$invalidate(17, stream_every = $$props2.stream_every);
    if ("modify_stream" in $$props2)
      $$invalidate(4, modify_stream = $$props2.modify_stream);
    if ("set_time_limit" in $$props2)
      $$invalidate(5, set_time_limit = $$props2.set_time_limit);
    if ("uploading" in $$props2)
      $$invalidate(0, uploading = $$props2.uploading);
    if ("active_source" in $$props2)
      $$invalidate(1, active_source = $$props2.active_source);
    if ("dragging" in $$props2)
      $$invalidate(2, dragging = $$props2.dragging);
    if ("$$scope" in $$props2)
      $$invalidate(42, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*active_source, sources*/
    258) {
      if (!active_source && sources) {
        $$invalidate(1, active_source = sources[0]);
      }
    }
    if ($$self.$$.dirty[0] & /*streaming, active_source*/
    514) {
      $$invalidate(18, active_streaming = streaming && active_source === "webcam");
    }
    if ($$self.$$.dirty[0] & /*uploading, active_streaming*/
    262145) {
      if (uploading && !active_streaming)
        $$invalidate(3, value = null);
    }
    if ($$self.$$.dirty[0] & /*dragging*/
    4) {
      dispatch("drag", dragging);
    }
  };
  return [
    uploading,
    active_source,
    dragging,
    value,
    modify_stream,
    set_time_limit,
    label,
    show_label,
    sources,
    streaming,
    mirror_webcam,
    selectable,
    root,
    i18n,
    max_file_size,
    upload,
    stream_handler,
    stream_every,
    active_streaming,
    upload_input,
    handle_upload,
    handle_clear,
    handle_save,
    dispatch,
    handle_click,
    handle_select_source,
    pending,
    slots,
    remove_image_handler,
    upload_1_binding,
    upload_1_uploading_binding,
    upload_1_dragging_binding,
    error_handler,
    webcam_modify_stream_binding,
    webcam_set_time_limit_binding,
    capture_handler,
    stream_handler_1,
    error_handler_1,
    drag_handler,
    upload_handler,
    close_stream_handler,
    selectsource_active_source_binding,
    $$scope
  ];
}
class ImageUploader extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        value: 3,
        label: 6,
        show_label: 7,
        sources: 8,
        streaming: 9,
        pending: 26,
        mirror_webcam: 10,
        selectable: 11,
        root: 12,
        i18n: 13,
        max_file_size: 14,
        upload: 15,
        stream_handler: 16,
        stream_every: 17,
        modify_stream: 4,
        set_time_limit: 5,
        uploading: 0,
        active_source: 1,
        dragging: 2
      },
      null,
      [-1, -1]
    );
  }
}
const ImageUploader$1 = ImageUploader;
export {
  ImageUploader$1 as I,
  Webcam$1 as W
};
