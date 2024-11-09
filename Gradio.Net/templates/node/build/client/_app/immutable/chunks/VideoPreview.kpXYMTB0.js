import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, element, claim_element, onMount, onDestroy, ensure_array_like, space, claim_space, set_style, listen, update_keyed_each, destroy_block, run_all, get_svelte_dataset, src_url_equal, create_component, claim_component, toggle_class, mount_component, transition_in, group_outros, transition_out, check_outros, destroy_component, binding_callbacks, bind, add_flush_callback, empty, text, claim_text, set_data, prevent_default, stop_propagation, createEventDispatcher, bubble, afterUpdate, tick } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { x as format_time, I as IconButton, h as prepare_files, u as uploadToHuggingFace } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { S as ShareButton } from "./ShareButton.B3gcBIAK.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { V as Video$1 } from "./Video.CzEOFOtQ.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
import { T as Trim, P as Pause } from "./Trim.CQ15_So8.js";
import { P as Play } from "./Play.wmWinRDD.js";
import { U as Undo } from "./Undo.BEjgqHJW.js";
import { b as loadFfmpeg, t as trimVideo, V as Video } from "./Video.ML_kOajE.js";
/* empty css                                              */
import { M as ModifyUpload } from "./ModifyUpload.Bygvg79x.js";
function create_fragment$4(ctx) {
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
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
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
class Maximise extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$4, safe_not_equal, {});
  }
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[20] = list[i];
  child_ctx[22] = i;
  return child_ctx;
}
function create_else_block$3(ctx) {
  let div1;
  let button0;
  let t0;
  let div0;
  let t1;
  let each_blocks = [];
  let each_1_lookup = /* @__PURE__ */ new Map();
  let t2;
  let button1;
  let mounted;
  let dispose;
  let each_value = ensure_array_like(
    /*thumbnails*/
    ctx[1]
  );
  const get_key = (ctx2) => (
    /*i*/
    ctx2[22]
  );
  for (let i = 0; i < each_value.length; i += 1) {
    let child_ctx = get_each_context(ctx, each_value, i);
    let key = get_key(child_ctx);
    each_1_lookup.set(key, each_blocks[i] = create_each_block(key, child_ctx));
  }
  return {
    c() {
      div1 = element("div");
      button0 = element("button");
      t0 = space();
      div0 = element("div");
      t1 = space();
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t2 = space();
      button1 = element("button");
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { id: true, class: true });
      var div1_nodes = children(div1);
      button0 = claim_element(div1_nodes, "BUTTON", {
        "aria-label": true,
        class: true,
        style: true
      });
      children(button0).forEach(detach);
      t0 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", { class: true, style: true });
      children(div0).forEach(detach);
      t1 = claim_space(div1_nodes);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div1_nodes);
      }
      t2 = claim_space(div1_nodes);
      button1 = claim_element(div1_nodes, "BUTTON", {
        "aria-label": true,
        class: true,
        style: true
      });
      children(button1).forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button0, "aria-label", "start drag handle for trimming video");
      attr(button0, "class", "handle left svelte-10c4beq");
      set_style(
        button0,
        "left",
        /*leftHandlePosition*/
        ctx[2] + "%"
      );
      attr(div0, "class", "opaque-layer svelte-10c4beq");
      set_style(
        div0,
        "left",
        /*leftHandlePosition*/
        ctx[2] + "%"
      );
      set_style(div0, "right", 100 - /*rightHandlePosition*/
      ctx[3] + "%");
      attr(button1, "aria-label", "end drag handle for trimming video");
      attr(button1, "class", "handle right svelte-10c4beq");
      set_style(
        button1,
        "left",
        /*rightHandlePosition*/
        ctx[3] + "%"
      );
      attr(div1, "id", "timeline");
      attr(div1, "class", "thumbnail-wrapper svelte-10c4beq");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, button0);
      append_hydration(div1, t0);
      append_hydration(div1, div0);
      append_hydration(div1, t1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div1, null);
        }
      }
      append_hydration(div1, t2);
      append_hydration(div1, button1);
      if (!mounted) {
        dispose = [
          listen(
            button0,
            "mousedown",
            /*mousedown_handler*/
            ctx[10]
          ),
          listen(
            button0,
            "blur",
            /*stopDragging*/
            ctx[5]
          ),
          listen(
            button0,
            "keydown",
            /*keydown_handler*/
            ctx[11]
          ),
          listen(
            button1,
            "mousedown",
            /*mousedown_handler_1*/
            ctx[12]
          ),
          listen(
            button1,
            "blur",
            /*stopDragging*/
            ctx[5]
          ),
          listen(
            button1,
            "keydown",
            /*keydown_handler_1*/
            ctx[13]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*leftHandlePosition*/
      4) {
        set_style(
          button0,
          "left",
          /*leftHandlePosition*/
          ctx2[2] + "%"
        );
      }
      if (dirty & /*leftHandlePosition*/
      4) {
        set_style(
          div0,
          "left",
          /*leftHandlePosition*/
          ctx2[2] + "%"
        );
      }
      if (dirty & /*rightHandlePosition*/
      8) {
        set_style(div0, "right", 100 - /*rightHandlePosition*/
        ctx2[3] + "%");
      }
      if (dirty & /*thumbnails*/
      2) {
        each_value = ensure_array_like(
          /*thumbnails*/
          ctx2[1]
        );
        each_blocks = update_keyed_each(each_blocks, dirty, get_key, 1, ctx2, each_value, each_1_lookup, div1, destroy_block, create_each_block, t2, get_each_context);
      }
      if (dirty & /*rightHandlePosition*/
      8) {
        set_style(
          button1,
          "left",
          /*rightHandlePosition*/
          ctx2[3] + "%"
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].d();
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block$3(ctx) {
  let div;
  let textContent = `<span aria-label="loading timeline" class="loader svelte-10c4beq"></span>`;
  return {
    c() {
      div = element("div");
      div.innerHTML = textContent;
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(div) !== "svelte-13yzice")
        div.innerHTML = textContent;
      this.h();
    },
    h() {
      attr(div, "class", "load-wrap svelte-10c4beq");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_each_block(key_1, ctx) {
  let img;
  let img_src_value;
  let img_alt_value;
  return {
    key: key_1,
    first: null,
    c() {
      img = element("img");
      this.h();
    },
    l(nodes) {
      img = claim_element(nodes, "IMG", {
        src: true,
        alt: true,
        draggable: true,
        class: true
      });
      this.h();
    },
    h() {
      if (!src_url_equal(img.src, img_src_value = /*thumbnail*/
      ctx[20]))
        attr(img, "src", img_src_value);
      attr(img, "alt", img_alt_value = `frame-${/*i*/
      ctx[22]}`);
      attr(img, "draggable", "false");
      attr(img, "class", "svelte-10c4beq");
      this.first = img;
    },
    m(target, anchor) {
      insert_hydration(target, img, anchor);
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty & /*thumbnails*/
      2 && !src_url_equal(img.src, img_src_value = /*thumbnail*/
      ctx[20])) {
        attr(img, "src", img_src_value);
      }
      if (dirty & /*thumbnails*/
      2 && img_alt_value !== (img_alt_value = `frame-${/*i*/
      ctx[22]}`)) {
        attr(img, "alt", img_alt_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(img);
      }
    }
  };
}
function create_fragment$3(ctx) {
  let div;
  function select_block_type(ctx2, dirty) {
    if (
      /*loadingTimeline*/
      ctx2[0]
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
      attr(div, "class", "container svelte-10c4beq");
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
let numberOfThumbnails = 10;
function instance$3($$self, $$props, $$invalidate) {
  let { videoElement } = $$props;
  let { trimmedDuration } = $$props;
  let { dragStart } = $$props;
  let { dragEnd } = $$props;
  let { loadingTimeline } = $$props;
  let thumbnails = [];
  let videoDuration;
  let leftHandlePosition = 0;
  let rightHandlePosition = 100;
  let dragging = null;
  const startDragging = (side) => {
    dragging = side;
  };
  const stopDragging = () => {
    dragging = null;
  };
  const drag = (event, distance) => {
    if (dragging) {
      const timeline = document.getElementById("timeline");
      if (!timeline)
        return;
      const rect = timeline.getBoundingClientRect();
      let newPercentage = (event.clientX - rect.left) / rect.width * 100;
      if (distance) {
        newPercentage = dragging === "left" ? leftHandlePosition + distance : rightHandlePosition + distance;
      } else {
        newPercentage = (event.clientX - rect.left) / rect.width * 100;
      }
      newPercentage = Math.max(0, Math.min(newPercentage, 100));
      if (dragging === "left") {
        $$invalidate(2, leftHandlePosition = Math.min(newPercentage, rightHandlePosition));
        const newTimeLeft = leftHandlePosition / 100 * videoDuration;
        $$invalidate(6, videoElement.currentTime = newTimeLeft, videoElement);
        $$invalidate(8, dragStart = newTimeLeft);
      } else if (dragging === "right") {
        $$invalidate(3, rightHandlePosition = Math.max(newPercentage, leftHandlePosition));
        const newTimeRight = rightHandlePosition / 100 * videoDuration;
        $$invalidate(6, videoElement.currentTime = newTimeRight, videoElement);
        $$invalidate(9, dragEnd = newTimeRight);
      }
      const startTime = leftHandlePosition / 100 * videoDuration;
      const endTime = rightHandlePosition / 100 * videoDuration;
      $$invalidate(7, trimmedDuration = endTime - startTime);
      $$invalidate(2, leftHandlePosition);
      $$invalidate(3, rightHandlePosition);
    }
  };
  const moveHandle = (e) => {
    if (dragging) {
      const distance = 1 / videoDuration * 100;
      if (e.key === "ArrowLeft") {
        drag({ clientX: 0 }, -distance);
      } else if (e.key === "ArrowRight") {
        drag({ clientX: 0 }, distance);
      }
    }
  };
  const generateThumbnail = () => {
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    if (!ctx)
      return;
    canvas.width = videoElement.videoWidth;
    canvas.height = videoElement.videoHeight;
    ctx.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
    const thumbnail = canvas.toDataURL("image/jpeg", 0.7);
    $$invalidate(1, thumbnails = [...thumbnails, thumbnail]);
  };
  onMount(() => {
    const loadMetadata = () => {
      videoDuration = videoElement.duration;
      const interval = videoDuration / numberOfThumbnails;
      let captures = 0;
      const onSeeked = () => {
        generateThumbnail();
        captures++;
        if (captures < numberOfThumbnails) {
          $$invalidate(6, videoElement.currentTime += interval, videoElement);
        } else {
          videoElement.removeEventListener("seeked", onSeeked);
        }
      };
      videoElement.addEventListener("seeked", onSeeked);
      $$invalidate(6, videoElement.currentTime = 0, videoElement);
    };
    if (videoElement.readyState >= 1) {
      loadMetadata();
    } else {
      videoElement.addEventListener("loadedmetadata", loadMetadata);
    }
  });
  onDestroy(() => {
    window.removeEventListener("mousemove", drag);
    window.removeEventListener("mouseup", stopDragging);
    window.removeEventListener("keydown", moveHandle);
  });
  onMount(() => {
    window.addEventListener("mousemove", drag);
    window.addEventListener("mouseup", stopDragging);
    window.addEventListener("keydown", moveHandle);
  });
  const mousedown_handler = () => startDragging("left");
  const keydown_handler = (e) => {
    if (e.key === "ArrowLeft" || e.key == "ArrowRight") {
      startDragging("left");
    }
  };
  const mousedown_handler_1 = () => startDragging("right");
  const keydown_handler_1 = (e) => {
    if (e.key === "ArrowLeft" || e.key == "ArrowRight") {
      startDragging("right");
    }
  };
  $$self.$$set = ($$props2) => {
    if ("videoElement" in $$props2)
      $$invalidate(6, videoElement = $$props2.videoElement);
    if ("trimmedDuration" in $$props2)
      $$invalidate(7, trimmedDuration = $$props2.trimmedDuration);
    if ("dragStart" in $$props2)
      $$invalidate(8, dragStart = $$props2.dragStart);
    if ("dragEnd" in $$props2)
      $$invalidate(9, dragEnd = $$props2.dragEnd);
    if ("loadingTimeline" in $$props2)
      $$invalidate(0, loadingTimeline = $$props2.loadingTimeline);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*thumbnails*/
    2) {
      $$invalidate(0, loadingTimeline = thumbnails.length !== numberOfThumbnails);
    }
  };
  return [
    loadingTimeline,
    thumbnails,
    leftHandlePosition,
    rightHandlePosition,
    startDragging,
    stopDragging,
    videoElement,
    trimmedDuration,
    dragStart,
    dragEnd,
    mousedown_handler,
    keydown_handler,
    mousedown_handler_1,
    keydown_handler_1
  ];
}
class VideoTimeline extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, {
      videoElement: 6,
      trimmedDuration: 7,
      dragStart: 8,
      dragEnd: 9,
      loadingTimeline: 0
    });
  }
}
function create_if_block_3(ctx) {
  let div;
  let videotimeline;
  let updating_dragStart;
  let updating_dragEnd;
  let updating_trimmedDuration;
  let updating_loadingTimeline;
  let current;
  function videotimeline_dragStart_binding(value) {
    ctx[18](value);
  }
  function videotimeline_dragEnd_binding(value) {
    ctx[19](value);
  }
  function videotimeline_trimmedDuration_binding(value) {
    ctx[20](value);
  }
  function videotimeline_loadingTimeline_binding(value) {
    ctx[21](value);
  }
  let videotimeline_props = { videoElement: (
    /*videoElement*/
    ctx[2]
  ) };
  if (
    /*dragStart*/
    ctx[14] !== void 0
  ) {
    videotimeline_props.dragStart = /*dragStart*/
    ctx[14];
  }
  if (
    /*dragEnd*/
    ctx[15] !== void 0
  ) {
    videotimeline_props.dragEnd = /*dragEnd*/
    ctx[15];
  }
  if (
    /*trimmedDuration*/
    ctx[12] !== void 0
  ) {
    videotimeline_props.trimmedDuration = /*trimmedDuration*/
    ctx[12];
  }
  if (
    /*loadingTimeline*/
    ctx[16] !== void 0
  ) {
    videotimeline_props.loadingTimeline = /*loadingTimeline*/
    ctx[16];
  }
  videotimeline = new VideoTimeline({ props: videotimeline_props });
  binding_callbacks.push(() => bind(videotimeline, "dragStart", videotimeline_dragStart_binding));
  binding_callbacks.push(() => bind(videotimeline, "dragEnd", videotimeline_dragEnd_binding));
  binding_callbacks.push(() => bind(videotimeline, "trimmedDuration", videotimeline_trimmedDuration_binding));
  binding_callbacks.push(() => bind(videotimeline, "loadingTimeline", videotimeline_loadingTimeline_binding));
  return {
    c() {
      div = element("div");
      create_component(videotimeline.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(videotimeline.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "timeline-wrapper svelte-7yrr5f");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(videotimeline, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const videotimeline_changes = {};
      if (dirty & /*videoElement*/
      4)
        videotimeline_changes.videoElement = /*videoElement*/
        ctx2[2];
      if (!updating_dragStart && dirty & /*dragStart*/
      16384) {
        updating_dragStart = true;
        videotimeline_changes.dragStart = /*dragStart*/
        ctx2[14];
        add_flush_callback(() => updating_dragStart = false);
      }
      if (!updating_dragEnd && dirty & /*dragEnd*/
      32768) {
        updating_dragEnd = true;
        videotimeline_changes.dragEnd = /*dragEnd*/
        ctx2[15];
        add_flush_callback(() => updating_dragEnd = false);
      }
      if (!updating_trimmedDuration && dirty & /*trimmedDuration*/
      4096) {
        updating_trimmedDuration = true;
        videotimeline_changes.trimmedDuration = /*trimmedDuration*/
        ctx2[12];
        add_flush_callback(() => updating_trimmedDuration = false);
      }
      if (!updating_loadingTimeline && dirty & /*loadingTimeline*/
      65536) {
        updating_loadingTimeline = true;
        videotimeline_changes.loadingTimeline = /*loadingTimeline*/
        ctx2[16];
        add_flush_callback(() => updating_loadingTimeline = false);
      }
      videotimeline.$set(videotimeline_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(videotimeline.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(videotimeline.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(videotimeline);
    }
  };
}
function create_else_block$2(ctx) {
  let div;
  return {
    c() {
      div = element("div");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      children(div).forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "svelte-7yrr5f");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_if_block_2$2(ctx) {
  let time;
  let t0_value = format_time(
    /*trimmedDuration*/
    ctx[12]
  ) + "";
  let t0;
  let t1;
  let div;
  let button0;
  let textContent = "Trim";
  let t3;
  let button1;
  let textContent_1 = "Cancel";
  let mounted;
  let dispose;
  return {
    c() {
      time = element("time");
      t0 = text(t0_value);
      t1 = space();
      div = element("div");
      button0 = element("button");
      button0.textContent = textContent;
      t3 = space();
      button1 = element("button");
      button1.textContent = textContent_1;
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { "aria-label": true, class: true });
      var time_nodes = children(time);
      t0 = claim_text(time_nodes, t0_value);
      time_nodes.forEach(detach);
      t1 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      button0 = claim_element(div_nodes, "BUTTON", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(button0) !== "svelte-18nzick")
        button0.textContent = textContent;
      t3 = claim_space(div_nodes);
      button1 = claim_element(div_nodes, "BUTTON", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(button1) !== "svelte-1mj98i4")
        button1.textContent = textContent_1;
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "aria-label", "duration of selected region in seconds");
      attr(time, "class", "svelte-7yrr5f");
      toggle_class(
        time,
        "hidden",
        /*loadingTimeline*/
        ctx[16]
      );
      attr(button0, "class", "text-button svelte-7yrr5f");
      toggle_class(
        button0,
        "hidden",
        /*loadingTimeline*/
        ctx[16]
      );
      attr(button1, "class", "text-button svelte-7yrr5f");
      toggle_class(
        button1,
        "hidden",
        /*loadingTimeline*/
        ctx[16]
      );
      attr(div, "class", "edit-buttons svelte-7yrr5f");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      append_hydration(time, t0);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, div, anchor);
      append_hydration(div, button0);
      append_hydration(div, t3);
      append_hydration(div, button1);
      if (!mounted) {
        dispose = [
          listen(
            button0,
            "click",
            /*click_handler*/
            ctx[22]
          ),
          listen(
            button1,
            "click",
            /*toggleTrimmingMode*/
            ctx[17]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*trimmedDuration*/
      4096 && t0_value !== (t0_value = format_time(
        /*trimmedDuration*/
        ctx2[12]
      ) + ""))
        set_data(t0, t0_value);
      if (dirty & /*loadingTimeline*/
      65536) {
        toggle_class(
          time,
          "hidden",
          /*loadingTimeline*/
          ctx2[16]
        );
      }
      if (dirty & /*loadingTimeline*/
      65536) {
        toggle_class(
          button0,
          "hidden",
          /*loadingTimeline*/
          ctx2[16]
        );
      }
      if (dirty & /*loadingTimeline*/
      65536) {
        toggle_class(
          button1,
          "hidden",
          /*loadingTimeline*/
          ctx2[16]
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(time);
        detach(t1);
        detach(div);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_1$2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Undo,
      label: "Reset video to initial value",
      disabled: (
        /*processingVideo*/
        ctx[1] || !/*has_change_history*/
        ctx[11]
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler_1*/
    ctx[23]
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
      if (dirty & /*processingVideo, has_change_history*/
      2050)
        iconbutton_changes.disabled = /*processingVideo*/
        ctx2[1] || !/*has_change_history*/
        ctx2[11];
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
function create_if_block$2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Trim,
      label: "Trim video to selection",
      disabled: (
        /*processingVideo*/
        ctx[1]
      )
    }
  });
  iconbutton.$on(
    "click",
    /*toggleTrimmingMode*/
    ctx[17]
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
      if (dirty & /*processingVideo*/
      2)
        iconbutton_changes.disabled = /*processingVideo*/
        ctx2[1];
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
function create_default_slot$2(ctx) {
  let t;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*showRedo*/
    ctx[3] && /*mode*/
    ctx[0] === "" && create_if_block_1$2(ctx)
  );
  let if_block1 = (
    /*interactive*/
    ctx[4] && /*mode*/
    ctx[0] === "" && create_if_block$2(ctx)
  );
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
      if (
        /*showRedo*/
        ctx2[3] && /*mode*/
        ctx2[0] === ""
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*showRedo, mode*/
          9) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_1$2(ctx2);
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
      if (
        /*interactive*/
        ctx2[4] && /*mode*/
        ctx2[0] === ""
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*interactive, mode*/
          17) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block$2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
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
function create_fragment$2(ctx) {
  var _a;
  let div1;
  let t0;
  let div0;
  let t1;
  let modifyupload;
  let current;
  let if_block0 = (
    /*mode*/
    ctx[0] === "edit" && create_if_block_3(ctx)
  );
  function select_block_type(ctx2, dirty) {
    if (
      /*mode*/
      ctx2[0] === "edit" && /*trimmedDuration*/
      ctx2[12] !== null
    )
      return create_if_block_2$2;
    return create_else_block$2;
  }
  let current_block_type = select_block_type(ctx);
  let if_block1 = current_block_type(ctx);
  modifyupload = new ModifyUpload({
    props: {
      i18n: (
        /*i18n*/
        ctx[7]
      ),
      download: (
        /*show_download_button*/
        ctx[9] ? (
          /*value*/
          (_a = ctx[8]) == null ? void 0 : _a.url
        ) : null
      ),
      $$slots: { default: [create_default_slot$2] },
      $$scope: { ctx }
    }
  });
  modifyupload.$on(
    "clear",
    /*clear_handler*/
    ctx[24]
  );
  return {
    c() {
      div1 = element("div");
      if (if_block0)
        if_block0.c();
      t0 = space();
      div0 = element("div");
      if_block1.c();
      t1 = space();
      create_component(modifyupload.$$.fragment);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      if (if_block0)
        if_block0.l(div1_nodes);
      t0 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", { class: true, "data-testid": true });
      var div0_nodes = children(div0);
      if_block1.l(div0_nodes);
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      t1 = claim_space(nodes);
      claim_component(modifyupload.$$.fragment, nodes);
      this.h();
    },
    h() {
      attr(div0, "class", "controls svelte-7yrr5f");
      attr(div0, "data-testid", "waveform-controls");
      attr(div1, "class", "container svelte-7yrr5f");
      toggle_class(
        div1,
        "hidden",
        /*mode*/
        ctx[0] !== "edit"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      if (if_block0)
        if_block0.m(div1, null);
      append_hydration(div1, t0);
      append_hydration(div1, div0);
      if_block1.m(div0, null);
      insert_hydration(target, t1, anchor);
      mount_component(modifyupload, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      var _a2;
      if (
        /*mode*/
        ctx2[0] === "edit"
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*mode*/
          1) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_3(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div1, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block1) {
        if_block1.p(ctx2, dirty);
      } else {
        if_block1.d(1);
        if_block1 = current_block_type(ctx2);
        if (if_block1) {
          if_block1.c();
          if_block1.m(div0, null);
        }
      }
      if (!current || dirty & /*mode*/
      1) {
        toggle_class(
          div1,
          "hidden",
          /*mode*/
          ctx2[0] !== "edit"
        );
      }
      const modifyupload_changes = {};
      if (dirty & /*i18n*/
      128)
        modifyupload_changes.i18n = /*i18n*/
        ctx2[7];
      if (dirty & /*show_download_button, value*/
      768)
        modifyupload_changes.download = /*show_download_button*/
        ctx2[9] ? (
          /*value*/
          (_a2 = ctx2[8]) == null ? void 0 : _a2.url
        ) : null;
      if (dirty & /*$$scope, processingVideo, interactive, mode, has_change_history, handle_reset_value, showRedo*/
      33556539) {
        modifyupload_changes.$$scope = { dirty, ctx: ctx2 };
      }
      modifyupload.$set(modifyupload_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(modifyupload.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(modifyupload.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
        detach(t1);
      }
      if (if_block0)
        if_block0.d();
      if_block1.d();
      destroy_component(modifyupload, detaching);
    }
  };
}
function instance$2($$self, $$props, $$invalidate) {
  let { videoElement } = $$props;
  let { showRedo = false } = $$props;
  let { interactive = true } = $$props;
  let { mode = "" } = $$props;
  let { handle_reset_value } = $$props;
  let { handle_trim_video } = $$props;
  let { processingVideo = false } = $$props;
  let { i18n } = $$props;
  let { value = null } = $$props;
  let { show_download_button = false } = $$props;
  let { handle_clear = () => {
  } } = $$props;
  let { has_change_history = false } = $$props;
  let ffmpeg;
  onMount(async () => {
    $$invalidate(13, ffmpeg = await loadFfmpeg());
  });
  let trimmedDuration = null;
  let dragStart = 0;
  let dragEnd = 0;
  let loadingTimeline = false;
  const toggleTrimmingMode = () => {
    if (mode === "edit") {
      $$invalidate(0, mode = "");
      $$invalidate(12, trimmedDuration = videoElement.duration);
    } else {
      $$invalidate(0, mode = "edit");
    }
  };
  function videotimeline_dragStart_binding(value2) {
    dragStart = value2;
    $$invalidate(14, dragStart);
  }
  function videotimeline_dragEnd_binding(value2) {
    dragEnd = value2;
    $$invalidate(15, dragEnd);
  }
  function videotimeline_trimmedDuration_binding(value2) {
    trimmedDuration = value2;
    $$invalidate(12, trimmedDuration), $$invalidate(0, mode), $$invalidate(2, videoElement);
  }
  function videotimeline_loadingTimeline_binding(value2) {
    loadingTimeline = value2;
    $$invalidate(16, loadingTimeline);
  }
  const click_handler = () => {
    $$invalidate(0, mode = "");
    $$invalidate(1, processingVideo = true);
    trimVideo(ffmpeg, dragStart, dragEnd, videoElement).then((videoBlob) => {
      handle_trim_video(videoBlob);
    }).then(() => {
      $$invalidate(1, processingVideo = false);
    });
  };
  const click_handler_1 = () => {
    handle_reset_value();
    $$invalidate(0, mode = "");
  };
  const clear_handler = () => handle_clear();
  $$self.$$set = ($$props2) => {
    if ("videoElement" in $$props2)
      $$invalidate(2, videoElement = $$props2.videoElement);
    if ("showRedo" in $$props2)
      $$invalidate(3, showRedo = $$props2.showRedo);
    if ("interactive" in $$props2)
      $$invalidate(4, interactive = $$props2.interactive);
    if ("mode" in $$props2)
      $$invalidate(0, mode = $$props2.mode);
    if ("handle_reset_value" in $$props2)
      $$invalidate(5, handle_reset_value = $$props2.handle_reset_value);
    if ("handle_trim_video" in $$props2)
      $$invalidate(6, handle_trim_video = $$props2.handle_trim_video);
    if ("processingVideo" in $$props2)
      $$invalidate(1, processingVideo = $$props2.processingVideo);
    if ("i18n" in $$props2)
      $$invalidate(7, i18n = $$props2.i18n);
    if ("value" in $$props2)
      $$invalidate(8, value = $$props2.value);
    if ("show_download_button" in $$props2)
      $$invalidate(9, show_download_button = $$props2.show_download_button);
    if ("handle_clear" in $$props2)
      $$invalidate(10, handle_clear = $$props2.handle_clear);
    if ("has_change_history" in $$props2)
      $$invalidate(11, has_change_history = $$props2.has_change_history);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*mode, trimmedDuration, videoElement*/
    4101) {
      if (mode === "edit" && trimmedDuration === null && videoElement)
        $$invalidate(12, trimmedDuration = videoElement.duration);
    }
  };
  return [
    mode,
    processingVideo,
    videoElement,
    showRedo,
    interactive,
    handle_reset_value,
    handle_trim_video,
    i18n,
    value,
    show_download_button,
    handle_clear,
    has_change_history,
    trimmedDuration,
    ffmpeg,
    dragStart,
    dragEnd,
    loadingTimeline,
    toggleTrimmingMode,
    videotimeline_dragStart_binding,
    videotimeline_dragEnd_binding,
    videotimeline_trimmedDuration_binding,
    videotimeline_loadingTimeline_binding,
    click_handler,
    click_handler_1,
    clear_handler
  ];
}
class VideoControls extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      videoElement: 2,
      showRedo: 3,
      interactive: 4,
      mode: 0,
      handle_reset_value: 5,
      handle_trim_video: 6,
      processingVideo: 1,
      i18n: 7,
      value: 8,
      show_download_button: 9,
      handle_clear: 10,
      has_change_history: 11
    });
  }
}
function create_default_slot$1(ctx) {
  let track;
  let track_src_value;
  return {
    c() {
      track = element("track");
      this.h();
    },
    l(nodes) {
      track = claim_element(nodes, "TRACK", { kind: true, src: true });
      this.h();
    },
    h() {
      attr(track, "kind", "captions");
      if (!src_url_equal(track.src, track_src_value = /*subtitle*/
      ctx[1]))
        attr(track, "src", track_src_value);
      track.default = true;
    },
    m(target, anchor) {
      insert_hydration(target, track, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*subtitle*/
      2 && !src_url_equal(track.src, track_src_value = /*subtitle*/
      ctx2[1])) {
        attr(track, "src", track_src_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(track);
      }
    }
  };
}
function create_else_block$1(ctx) {
  let pause;
  let current;
  pause = new Pause({});
  return {
    c() {
      create_component(pause.$$.fragment);
    },
    l(nodes) {
      claim_component(pause.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(pause, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(pause.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(pause.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(pause, detaching);
    }
  };
}
function create_if_block_2$1(ctx) {
  let play;
  let current;
  play = new Play({});
  return {
    c() {
      create_component(play.$$.fragment);
    },
    l(nodes) {
      claim_component(play.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(play, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(play.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(play.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(play, detaching);
    }
  };
}
function create_if_block_1$1(ctx) {
  let undo;
  let current;
  undo = new Undo({});
  return {
    c() {
      create_component(undo.$$.fragment);
    },
    l(nodes) {
      claim_component(undo.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(undo, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(undo.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(undo.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(undo, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let videocontrols;
  let updating_processingVideo;
  let current;
  function videocontrols_processingVideo_binding(value) {
    ctx[37](value);
  }
  let videocontrols_props = {
    videoElement: (
      /*video*/
      ctx[17]
    ),
    showRedo: true,
    handle_trim_video: (
      /*handle_trim_video*/
      ctx[23]
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[7]
    ),
    value: (
      /*value*/
      ctx[11]
    ),
    i18n: (
      /*i18n*/
      ctx[9]
    ),
    show_download_button: (
      /*show_download_button*/
      ctx[10]
    ),
    handle_clear: (
      /*handle_clear*/
      ctx[12]
    ),
    has_change_history: (
      /*has_change_history*/
      ctx[13]
    )
  };
  if (
    /*processingVideo*/
    ctx[18] !== void 0
  ) {
    videocontrols_props.processingVideo = /*processingVideo*/
    ctx[18];
  }
  videocontrols = new VideoControls({ props: videocontrols_props });
  binding_callbacks.push(() => bind(videocontrols, "processingVideo", videocontrols_processingVideo_binding));
  return {
    c() {
      create_component(videocontrols.$$.fragment);
    },
    l(nodes) {
      claim_component(videocontrols.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(videocontrols, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const videocontrols_changes = {};
      if (dirty[0] & /*video*/
      131072)
        videocontrols_changes.videoElement = /*video*/
        ctx2[17];
      if (dirty[0] & /*handle_reset_value*/
      128)
        videocontrols_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[7];
      if (dirty[0] & /*value*/
      2048)
        videocontrols_changes.value = /*value*/
        ctx2[11];
      if (dirty[0] & /*i18n*/
      512)
        videocontrols_changes.i18n = /*i18n*/
        ctx2[9];
      if (dirty[0] & /*show_download_button*/
      1024)
        videocontrols_changes.show_download_button = /*show_download_button*/
        ctx2[10];
      if (dirty[0] & /*handle_clear*/
      4096)
        videocontrols_changes.handle_clear = /*handle_clear*/
        ctx2[12];
      if (dirty[0] & /*has_change_history*/
      8192)
        videocontrols_changes.has_change_history = /*has_change_history*/
        ctx2[13];
      if (!updating_processingVideo && dirty[0] & /*processingVideo*/
      262144) {
        updating_processingVideo = true;
        videocontrols_changes.processingVideo = /*processingVideo*/
        ctx2[18];
        add_flush_callback(() => updating_processingVideo = false);
      }
      videocontrols.$set(videocontrols_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(videocontrols.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(videocontrols.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(videocontrols, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let div4;
  let div0;
  let video_1;
  let updating_currentTime;
  let updating_duration;
  let updating_paused;
  let updating_node;
  let t0;
  let div3;
  let div2;
  let span0;
  let current_block_type_index;
  let if_block0;
  let t1;
  let span1;
  let t2_value = format_time(
    /*time*/
    ctx[14]
  ) + "";
  let t2;
  let t3;
  let t4_value = format_time(
    /*duration*/
    ctx[15]
  ) + "";
  let t4;
  let t5;
  let progress;
  let progress_value_value;
  let t6;
  let div1;
  let maximise;
  let t7;
  let if_block1_anchor;
  let current;
  let mounted;
  let dispose;
  function video_1_currentTime_binding(value) {
    ctx[28](value);
  }
  function video_1_duration_binding(value) {
    ctx[29](value);
  }
  function video_1_paused_binding(value) {
    ctx[30](value);
  }
  function video_1_node_binding(value) {
    ctx[31](value);
  }
  let video_1_props = {
    src: (
      /*src*/
      ctx[0]
    ),
    preload: "auto",
    autoplay: (
      /*autoplay*/
      ctx[3]
    ),
    loop: (
      /*loop*/
      ctx[4]
    ),
    is_stream: (
      /*is_stream*/
      ctx[8]
    ),
    "data-testid": `${/*label*/
    ctx[5]}-player`,
    processingVideo: (
      /*processingVideo*/
      ctx[18]
    ),
    $$slots: { default: [create_default_slot$1] },
    $$scope: { ctx }
  };
  if (
    /*time*/
    ctx[14] !== void 0
  ) {
    video_1_props.currentTime = /*time*/
    ctx[14];
  }
  if (
    /*duration*/
    ctx[15] !== void 0
  ) {
    video_1_props.duration = /*duration*/
    ctx[15];
  }
  if (
    /*paused*/
    ctx[16] !== void 0
  ) {
    video_1_props.paused = /*paused*/
    ctx[16];
  }
  if (
    /*video*/
    ctx[17] !== void 0
  ) {
    video_1_props.node = /*video*/
    ctx[17];
  }
  video_1 = new Video({ props: video_1_props });
  binding_callbacks.push(() => bind(video_1, "currentTime", video_1_currentTime_binding));
  binding_callbacks.push(() => bind(video_1, "duration", video_1_duration_binding));
  binding_callbacks.push(() => bind(video_1, "paused", video_1_paused_binding));
  binding_callbacks.push(() => bind(video_1, "node", video_1_node_binding));
  video_1.$on(
    "click",
    /*play_pause*/
    ctx[20]
  );
  video_1.$on(
    "play",
    /*play_handler*/
    ctx[32]
  );
  video_1.$on(
    "pause",
    /*pause_handler*/
    ctx[33]
  );
  video_1.$on(
    "ended",
    /*handle_end*/
    ctx[22]
  );
  video_1.$on(
    "loadstart",
    /*loadstart_handler*/
    ctx[34]
  );
  video_1.$on(
    "loadeddata",
    /*loadeddata_handler*/
    ctx[35]
  );
  video_1.$on(
    "loadedmetadata",
    /*loadedmetadata_handler*/
    ctx[36]
  );
  const if_block_creators = [create_if_block_1$1, create_if_block_2$1, create_else_block$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*time*/
      ctx2[14] === /*duration*/
      ctx2[15]
    )
      return 0;
    if (
      /*paused*/
      ctx2[16]
    )
      return 1;
    return 2;
  }
  current_block_type_index = select_block_type(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  maximise = new Maximise({});
  let if_block1 = (
    /*interactive*/
    ctx[6] && create_if_block$1(ctx)
  );
  return {
    c() {
      div4 = element("div");
      div0 = element("div");
      create_component(video_1.$$.fragment);
      t0 = space();
      div3 = element("div");
      div2 = element("div");
      span0 = element("span");
      if_block0.c();
      t1 = space();
      span1 = element("span");
      t2 = text(t2_value);
      t3 = text(" / ");
      t4 = text(t4_value);
      t5 = space();
      progress = element("progress");
      t6 = space();
      div1 = element("div");
      create_component(maximise.$$.fragment);
      t7 = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
      this.h();
    },
    l(nodes) {
      div4 = claim_element(nodes, "DIV", { class: true });
      var div4_nodes = children(div4);
      div0 = claim_element(div4_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      claim_component(video_1.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      t0 = claim_space(div4_nodes);
      div3 = claim_element(div4_nodes, "DIV", { class: true });
      var div3_nodes = children(div3);
      div2 = claim_element(div3_nodes, "DIV", { class: true });
      var div2_nodes = children(div2);
      span0 = claim_element(div2_nodes, "SPAN", {
        role: true,
        tabindex: true,
        class: true,
        "aria-label": true
      });
      var span0_nodes = children(span0);
      if_block0.l(span0_nodes);
      span0_nodes.forEach(detach);
      t1 = claim_space(div2_nodes);
      span1 = claim_element(div2_nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      t2 = claim_text(span1_nodes, t2_value);
      t3 = claim_text(span1_nodes, " / ");
      t4 = claim_text(span1_nodes, t4_value);
      span1_nodes.forEach(detach);
      t5 = claim_space(div2_nodes);
      progress = claim_element(div2_nodes, "PROGRESS", { class: true });
      children(progress).forEach(detach);
      t6 = claim_space(div2_nodes);
      div1 = claim_element(div2_nodes, "DIV", {
        role: true,
        tabindex: true,
        class: true,
        "aria-label": true
      });
      var div1_nodes = children(div1);
      claim_component(maximise.$$.fragment, div1_nodes);
      div1_nodes.forEach(detach);
      div2_nodes.forEach(detach);
      div3_nodes.forEach(detach);
      div4_nodes.forEach(detach);
      t7 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
      this.h();
    },
    h() {
      attr(div0, "class", "mirror-wrap svelte-euo1cw");
      toggle_class(
        div0,
        "mirror",
        /*mirror*/
        ctx[2]
      );
      attr(span0, "role", "button");
      attr(span0, "tabindex", "0");
      attr(span0, "class", "icon svelte-euo1cw");
      attr(span0, "aria-label", "play-pause-replay-button");
      attr(span1, "class", "time svelte-euo1cw");
      progress.value = progress_value_value = /*time*/
      ctx[14] / /*duration*/
      ctx[15] || 0;
      attr(progress, "class", "svelte-euo1cw");
      attr(div1, "role", "button");
      attr(div1, "tabindex", "0");
      attr(div1, "class", "icon svelte-euo1cw");
      attr(div1, "aria-label", "full-screen");
      attr(div2, "class", "inner svelte-euo1cw");
      attr(div3, "class", "controls svelte-euo1cw");
      attr(div4, "class", "wrap svelte-euo1cw");
    },
    m(target, anchor) {
      insert_hydration(target, div4, anchor);
      append_hydration(div4, div0);
      mount_component(video_1, div0, null);
      append_hydration(div4, t0);
      append_hydration(div4, div3);
      append_hydration(div3, div2);
      append_hydration(div2, span0);
      if_blocks[current_block_type_index].m(span0, null);
      append_hydration(div2, t1);
      append_hydration(div2, span1);
      append_hydration(span1, t2);
      append_hydration(span1, t3);
      append_hydration(span1, t4);
      append_hydration(div2, t5);
      append_hydration(div2, progress);
      append_hydration(div2, t6);
      append_hydration(div2, div1);
      mount_component(maximise, div1, null);
      insert_hydration(target, t7, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            span0,
            "click",
            /*play_pause*/
            ctx[20]
          ),
          listen(
            span0,
            "keydown",
            /*play_pause*/
            ctx[20]
          ),
          listen(
            progress,
            "mousemove",
            /*handleMove*/
            ctx[19]
          ),
          listen(progress, "touchmove", prevent_default(
            /*handleMove*/
            ctx[19]
          )),
          listen(progress, "click", stop_propagation(prevent_default(
            /*handle_click*/
            ctx[21]
          ))),
          listen(
            div1,
            "click",
            /*open_full_screen*/
            ctx[24]
          ),
          listen(
            div1,
            "keypress",
            /*open_full_screen*/
            ctx[24]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const video_1_changes = {};
      if (dirty[0] & /*src*/
      1)
        video_1_changes.src = /*src*/
        ctx2[0];
      if (dirty[0] & /*autoplay*/
      8)
        video_1_changes.autoplay = /*autoplay*/
        ctx2[3];
      if (dirty[0] & /*loop*/
      16)
        video_1_changes.loop = /*loop*/
        ctx2[4];
      if (dirty[0] & /*is_stream*/
      256)
        video_1_changes.is_stream = /*is_stream*/
        ctx2[8];
      if (dirty[0] & /*label*/
      32)
        video_1_changes["data-testid"] = `${/*label*/
        ctx2[5]}-player`;
      if (dirty[0] & /*processingVideo*/
      262144)
        video_1_changes.processingVideo = /*processingVideo*/
        ctx2[18];
      if (dirty[0] & /*subtitle*/
      2 | dirty[1] & /*$$scope*/
      256) {
        video_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_currentTime && dirty[0] & /*time*/
      16384) {
        updating_currentTime = true;
        video_1_changes.currentTime = /*time*/
        ctx2[14];
        add_flush_callback(() => updating_currentTime = false);
      }
      if (!updating_duration && dirty[0] & /*duration*/
      32768) {
        updating_duration = true;
        video_1_changes.duration = /*duration*/
        ctx2[15];
        add_flush_callback(() => updating_duration = false);
      }
      if (!updating_paused && dirty[0] & /*paused*/
      65536) {
        updating_paused = true;
        video_1_changes.paused = /*paused*/
        ctx2[16];
        add_flush_callback(() => updating_paused = false);
      }
      if (!updating_node && dirty[0] & /*video*/
      131072) {
        updating_node = true;
        video_1_changes.node = /*video*/
        ctx2[17];
        add_flush_callback(() => updating_node = false);
      }
      video_1.$set(video_1_changes);
      if (!current || dirty[0] & /*mirror*/
      4) {
        toggle_class(
          div0,
          "mirror",
          /*mirror*/
          ctx2[2]
        );
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index !== previous_block_index) {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block0.c();
        }
        transition_in(if_block0, 1);
        if_block0.m(span0, null);
      }
      if ((!current || dirty[0] & /*time*/
      16384) && t2_value !== (t2_value = format_time(
        /*time*/
        ctx2[14]
      ) + ""))
        set_data(t2, t2_value);
      if ((!current || dirty[0] & /*duration*/
      32768) && t4_value !== (t4_value = format_time(
        /*duration*/
        ctx2[15]
      ) + ""))
        set_data(t4, t4_value);
      if (!current || dirty[0] & /*time, duration*/
      49152 && progress_value_value !== (progress_value_value = /*time*/
      ctx2[14] / /*duration*/
      ctx2[15] || 0)) {
        progress.value = progress_value_value;
      }
      if (
        /*interactive*/
        ctx2[6]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*interactive*/
          64) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block$1(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(video_1.$$.fragment, local);
      transition_in(if_block0);
      transition_in(maximise.$$.fragment, local);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(video_1.$$.fragment, local);
      transition_out(if_block0);
      transition_out(maximise.$$.fragment, local);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div4);
        detach(t7);
        detach(if_block1_anchor);
      }
      destroy_component(video_1);
      if_blocks[current_block_type_index].d();
      destroy_component(maximise);
      if (if_block1)
        if_block1.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { root = "" } = $$props;
  let { src } = $$props;
  let { subtitle = null } = $$props;
  let { mirror } = $$props;
  let { autoplay } = $$props;
  let { loop } = $$props;
  let { label = "test" } = $$props;
  let { interactive = false } = $$props;
  let { handle_change = () => {
  } } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let { upload } = $$props;
  let { is_stream } = $$props;
  let { i18n } = $$props;
  let { show_download_button = false } = $$props;
  let { value = null } = $$props;
  let { handle_clear = () => {
  } } = $$props;
  let { has_change_history = false } = $$props;
  const dispatch = createEventDispatcher();
  let time = 0;
  let duration;
  let paused = true;
  let video;
  let processingVideo = false;
  function handleMove(e) {
    if (!duration)
      return;
    if (e.type === "click") {
      handle_click(e);
      return;
    }
    if (e.type !== "touchmove" && !(e.buttons & 1))
      return;
    const clientX = e.type === "touchmove" ? e.touches[0].clientX : e.clientX;
    const { left, right } = e.currentTarget.getBoundingClientRect();
    $$invalidate(14, time = duration * (clientX - left) / (right - left));
  }
  async function play_pause() {
    if (document.fullscreenElement != video) {
      const isPlaying = video.currentTime > 0 && !video.paused && !video.ended && video.readyState > video.HAVE_CURRENT_DATA;
      if (!isPlaying) {
        await video.play();
      } else
        video.pause();
    }
  }
  function handle_click(e) {
    const { left, right } = e.currentTarget.getBoundingClientRect();
    $$invalidate(14, time = duration * (e.clientX - left) / (right - left));
  }
  function handle_end() {
    dispatch("stop");
    dispatch("end");
  }
  const handle_trim_video = async (videoBlob) => {
    var _a;
    let _video_blob = new File([videoBlob], "video.mp4");
    const val = await prepare_files([_video_blob]);
    let value2 = (_a = await upload(val, root)) == null ? void 0 : _a.filter(Boolean)[0];
    handle_change(value2);
  };
  function open_full_screen() {
    video.requestFullscreen();
  }
  function video_1_currentTime_binding(value2) {
    time = value2;
    $$invalidate(14, time);
  }
  function video_1_duration_binding(value2) {
    duration = value2;
    $$invalidate(15, duration);
  }
  function video_1_paused_binding(value2) {
    paused = value2;
    $$invalidate(16, paused);
  }
  function video_1_node_binding(value2) {
    video = value2;
    $$invalidate(17, video);
  }
  function play_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_handler(event) {
    bubble.call(this, $$self, event);
  }
  function loadstart_handler(event) {
    bubble.call(this, $$self, event);
  }
  function loadeddata_handler(event) {
    bubble.call(this, $$self, event);
  }
  function loadedmetadata_handler(event) {
    bubble.call(this, $$self, event);
  }
  function videocontrols_processingVideo_binding(value2) {
    processingVideo = value2;
    $$invalidate(18, processingVideo);
  }
  $$self.$$set = ($$props2) => {
    if ("root" in $$props2)
      $$invalidate(25, root = $$props2.root);
    if ("src" in $$props2)
      $$invalidate(0, src = $$props2.src);
    if ("subtitle" in $$props2)
      $$invalidate(1, subtitle = $$props2.subtitle);
    if ("mirror" in $$props2)
      $$invalidate(2, mirror = $$props2.mirror);
    if ("autoplay" in $$props2)
      $$invalidate(3, autoplay = $$props2.autoplay);
    if ("loop" in $$props2)
      $$invalidate(4, loop = $$props2.loop);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("interactive" in $$props2)
      $$invalidate(6, interactive = $$props2.interactive);
    if ("handle_change" in $$props2)
      $$invalidate(26, handle_change = $$props2.handle_change);
    if ("handle_reset_value" in $$props2)
      $$invalidate(7, handle_reset_value = $$props2.handle_reset_value);
    if ("upload" in $$props2)
      $$invalidate(27, upload = $$props2.upload);
    if ("is_stream" in $$props2)
      $$invalidate(8, is_stream = $$props2.is_stream);
    if ("i18n" in $$props2)
      $$invalidate(9, i18n = $$props2.i18n);
    if ("show_download_button" in $$props2)
      $$invalidate(10, show_download_button = $$props2.show_download_button);
    if ("value" in $$props2)
      $$invalidate(11, value = $$props2.value);
    if ("handle_clear" in $$props2)
      $$invalidate(12, handle_clear = $$props2.handle_clear);
    if ("has_change_history" in $$props2)
      $$invalidate(13, has_change_history = $$props2.has_change_history);
  };
  return [
    src,
    subtitle,
    mirror,
    autoplay,
    loop,
    label,
    interactive,
    handle_reset_value,
    is_stream,
    i18n,
    show_download_button,
    value,
    handle_clear,
    has_change_history,
    time,
    duration,
    paused,
    video,
    processingVideo,
    handleMove,
    play_pause,
    handle_click,
    handle_end,
    handle_trim_video,
    open_full_screen,
    root,
    handle_change,
    upload,
    video_1_currentTime_binding,
    video_1_duration_binding,
    video_1_paused_binding,
    video_1_node_binding,
    play_handler,
    pause_handler,
    loadstart_handler,
    loadeddata_handler,
    loadedmetadata_handler,
    videocontrols_processingVideo_binding
  ];
}
class Player extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        root: 25,
        src: 0,
        subtitle: 1,
        mirror: 2,
        autoplay: 3,
        loop: 4,
        label: 5,
        interactive: 6,
        handle_change: 26,
        handle_reset_value: 7,
        upload: 27,
        is_stream: 8,
        i18n: 9,
        show_download_button: 10,
        value: 11,
        handle_clear: 12,
        has_change_history: 13
      },
      null,
      [-1, -1]
    );
  }
}
const Player$1 = Player;
function create_else_block(ctx) {
  let previous_key = (
    /*value*/
    ctx[0].url
  );
  let t;
  let div;
  let iconbuttonwrapper;
  let current;
  let key_block = create_key_block(ctx);
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      key_block.c();
      t = space();
      div = element("div");
      create_component(iconbuttonwrapper.$$.fragment);
      this.h();
    },
    l(nodes) {
      key_block.l(nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { "data-testid": true });
      var div_nodes = children(div);
      claim_component(iconbuttonwrapper.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "data-testid", "download-div");
    },
    m(target, anchor) {
      key_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      mount_component(iconbuttonwrapper, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && safe_not_equal(previous_key, previous_key = /*value*/
      ctx2[0].url)) {
        group_outros();
        transition_out(key_block, 1, 1, noop);
        check_outros();
        key_block = create_key_block(ctx2);
        key_block.c();
        transition_in(key_block, 1);
        key_block.m(t.parentNode, t);
      } else {
        key_block.p(ctx2, dirty);
      }
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, i18n, value, show_share_button, show_download_button*/
      2097505) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(key_block);
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(key_block);
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(div);
      }
      key_block.d(detaching);
      destroy_component(iconbuttonwrapper);
    }
  };
}
function create_if_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot] },
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
      2097152) {
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
function create_key_block(ctx) {
  var _a;
  let player;
  let current;
  player = new Player$1({
    props: {
      src: (
        /*value*/
        ctx[0].url
      ),
      subtitle: (
        /*subtitle*/
        (_a = ctx[1]) == null ? void 0 : _a.url
      ),
      is_stream: (
        /*value*/
        ctx[0].is_stream
      ),
      autoplay: (
        /*autoplay*/
        ctx[4]
      ),
      mirror: false,
      label: (
        /*label*/
        ctx[2]
      ),
      loop: (
        /*loop*/
        ctx[7]
      ),
      interactive: false,
      upload: (
        /*upload*/
        ctx[9]
      ),
      i18n: (
        /*i18n*/
        ctx[8]
      )
    }
  });
  player.$on(
    "play",
    /*play_handler*/
    ctx[11]
  );
  player.$on(
    "pause",
    /*pause_handler*/
    ctx[12]
  );
  player.$on(
    "stop",
    /*stop_handler*/
    ctx[13]
  );
  player.$on(
    "end",
    /*end_handler*/
    ctx[14]
  );
  player.$on(
    "loadedmetadata",
    /*loadedmetadata_handler*/
    ctx[15]
  );
  return {
    c() {
      create_component(player.$$.fragment);
    },
    l(nodes) {
      claim_component(player.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(player, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const player_changes = {};
      if (dirty & /*value*/
      1)
        player_changes.src = /*value*/
        ctx2[0].url;
      if (dirty & /*subtitle*/
      2)
        player_changes.subtitle = /*subtitle*/
        (_a2 = ctx2[1]) == null ? void 0 : _a2.url;
      if (dirty & /*value*/
      1)
        player_changes.is_stream = /*value*/
        ctx2[0].is_stream;
      if (dirty & /*autoplay*/
      16)
        player_changes.autoplay = /*autoplay*/
        ctx2[4];
      if (dirty & /*label*/
      4)
        player_changes.label = /*label*/
        ctx2[2];
      if (dirty & /*loop*/
      128)
        player_changes.loop = /*loop*/
        ctx2[7];
      if (dirty & /*upload*/
      512)
        player_changes.upload = /*upload*/
        ctx2[9];
      if (dirty & /*i18n*/
      256)
        player_changes.i18n = /*i18n*/
        ctx2[8];
      player.$set(player_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(player.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(player.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(player, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  var _a;
  let downloadlink;
  let current;
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*value*/
        ctx[0].is_stream ? (
          /*value*/
          (_a = ctx[0].url) == null ? void 0 : _a.replace("playlist.m3u8", "playlist-file")
        ) : (
          /*value*/
          ctx[0].url
        )
      ),
      download: (
        /*value*/
        ctx[0].orig_name || /*value*/
        ctx[0].path
      ),
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(downloadlink.$$.fragment);
    },
    l(nodes) {
      claim_component(downloadlink.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(downloadlink, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const downloadlink_changes = {};
      if (dirty & /*value*/
      1)
        downloadlink_changes.href = /*value*/
        ctx2[0].is_stream ? (
          /*value*/
          (_a2 = ctx2[0].url) == null ? void 0 : _a2.replace("playlist.m3u8", "playlist-file")
        ) : (
          /*value*/
          ctx2[0].url
        );
      if (dirty & /*value*/
      1)
        downloadlink_changes.download = /*value*/
        ctx2[0].orig_name || /*value*/
        ctx2[0].path;
      if (dirty & /*$$scope*/
      2097152) {
        downloadlink_changes.$$scope = { dirty, ctx: ctx2 };
      }
      downloadlink.$set(downloadlink_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(downloadlink.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(downloadlink.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(downloadlink, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: { Icon: Download, label: "Download" }
  });
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
function create_if_block_1(ctx) {
  let sharebutton;
  let current;
  sharebutton = new ShareButton({
    props: {
      i18n: (
        /*i18n*/
        ctx[8]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      formatter: (
        /*func*/
        ctx[16]
      )
    }
  });
  sharebutton.$on(
    "error",
    /*error_handler*/
    ctx[17]
  );
  sharebutton.$on(
    "share",
    /*share_handler*/
    ctx[18]
  );
  return {
    c() {
      create_component(sharebutton.$$.fragment);
    },
    l(nodes) {
      claim_component(sharebutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(sharebutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const sharebutton_changes = {};
      if (dirty & /*i18n*/
      256)
        sharebutton_changes.i18n = /*i18n*/
        ctx2[8];
      if (dirty & /*value*/
      1)
        sharebutton_changes.value = /*value*/
        ctx2[0];
      sharebutton.$set(sharebutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(sharebutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(sharebutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(sharebutton, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let t;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*show_download_button*/
    ctx[6] && create_if_block_2(ctx)
  );
  let if_block1 = (
    /*show_share_button*/
    ctx[5] && create_if_block_1(ctx)
  );
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
      if (
        /*show_download_button*/
        ctx2[6]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*show_download_button*/
          64) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2(ctx2);
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
      if (
        /*show_share_button*/
        ctx2[5]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*show_share_button*/
          32) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_1(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
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
function create_default_slot(ctx) {
  let video;
  let current;
  video = new Video$1({});
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
function create_fragment(ctx) {
  let blocklabel;
  let t;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[3]
      ),
      Icon: Video$1,
      label: (
        /*label*/
        ctx[2] || "Video"
      )
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*value*/
    ctx2[0] || /*value*/
    ctx2[0].url === void 0)
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      8)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[3];
      if (dirty & /*label*/
      4)
        blocklabel_changes.label = /*label*/
        ctx2[2] || "Video";
      blocklabel.$set(blocklabel_changes);
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
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value = null } = $$props;
  let { subtitle = null } = $$props;
  let { label = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { autoplay } = $$props;
  let { show_share_button = true } = $$props;
  let { show_download_button = true } = $$props;
  let { loop } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  let old_value = null;
  let old_subtitle = null;
  const dispatch = createEventDispatcher();
  afterUpdate(async () => {
    if (value !== old_value && subtitle !== old_subtitle && old_subtitle !== null) {
      old_value = value;
      $$invalidate(0, value = null);
      await tick();
      $$invalidate(0, value = old_value);
    }
    old_value = value;
    old_subtitle = subtitle;
  });
  function play_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_handler(event) {
    bubble.call(this, $$self, event);
  }
  function stop_handler(event) {
    bubble.call(this, $$self, event);
  }
  function end_handler(event) {
    bubble.call(this, $$self, event);
  }
  const loadedmetadata_handler = () => {
    dispatch("load");
  };
  const func = async (value2) => {
    if (!value2)
      return "";
    let url = await uploadToHuggingFace(value2.data);
    return url;
  };
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function share_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("subtitle" in $$props2)
      $$invalidate(1, subtitle = $$props2.subtitle);
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(3, show_label = $$props2.show_label);
    if ("autoplay" in $$props2)
      $$invalidate(4, autoplay = $$props2.autoplay);
    if ("show_share_button" in $$props2)
      $$invalidate(5, show_share_button = $$props2.show_share_button);
    if ("show_download_button" in $$props2)
      $$invalidate(6, show_download_button = $$props2.show_download_button);
    if ("loop" in $$props2)
      $$invalidate(7, loop = $$props2.loop);
    if ("i18n" in $$props2)
      $$invalidate(8, i18n = $$props2.i18n);
    if ("upload" in $$props2)
      $$invalidate(9, upload = $$props2.upload);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    1) {
      value && dispatch("change", value);
    }
  };
  return [
    value,
    subtitle,
    label,
    show_label,
    autoplay,
    show_share_button,
    show_download_button,
    loop,
    i18n,
    upload,
    dispatch,
    play_handler,
    pause_handler,
    stop_handler,
    end_handler,
    loadedmetadata_handler,
    func,
    error_handler,
    share_handler
  ];
}
class VideoPreview extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      subtitle: 1,
      label: 2,
      show_label: 3,
      autoplay: 4,
      show_share_button: 5,
      show_download_button: 6,
      loop: 7,
      i18n: 8,
      upload: 9
    });
  }
}
const VideoPreview$1 = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: VideoPreview
}, Symbol.toStringTag, { value: "Module" }));
export {
  Player$1 as P,
  VideoPreview as V,
  VideoPreview$1 as a
};
