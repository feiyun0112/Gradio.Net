import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, empty, transition_in, group_outros, transition_out, check_outros, element, space, claim_element, claim_space, create_component, claim_component, toggle_class, mount_component, listen, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
import { U as Upload, I as ImagePaste } from "./Upload.v2Thvwuk.js";
function create_fragment$3(ctx) {
  let svg;
  let path0;
  let path1;
  let line0;
  let line1;
  return {
    c() {
      svg = svg_element("svg");
      path0 = svg_element("path");
      path1 = svg_element("path");
      line0 = svg_element("line");
      line1 = svg_element("line");
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
      path0 = claim_svg_element(svg_nodes, "path", { d: true });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", { d: true });
      children(path1).forEach(detach);
      line0 = claim_svg_element(svg_nodes, "line", { x1: true, y1: true, x2: true, y2: true });
      children(line0).forEach(detach);
      line1 = claim_svg_element(svg_nodes, "line", { x1: true, y1: true, x2: true, y2: true });
      children(line1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M12 1a3 3 0 0 0-3 3v8a3 3 0 0 0 6 0V4a3 3 0 0 0-3-3z");
      attr(path1, "d", "M19 10v2a7 7 0 0 1-14 0v-2");
      attr(line0, "x1", "12");
      attr(line0, "y1", "19");
      attr(line0, "x2", "12");
      attr(line0, "y2", "23");
      attr(line1, "x1", "8");
      attr(line1, "y1", "23");
      attr(line1, "x2", "16");
      attr(line1, "y2", "23");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "2");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-mic");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
      append_hydration(svg, line0);
      append_hydration(svg, line1);
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
class Microphone extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$3, safe_not_equal, {});
  }
}
function create_fragment$2(ctx) {
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
        width: true,
        height: true,
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
      attr(path0, "d", "M12 2c-4.963 0-9 4.038-9 9c0 3.328 1.82 6.232 4.513 7.79l-2.067 1.378A1 1 0 0 0 6 22h12a1 1 0 0 0 .555-1.832l-2.067-1.378C19.18 17.232 21 14.328 21 11c0-4.962-4.037-9-9-9zm0 16c-3.859 0-7-3.141-7-7c0-3.86 3.141-7 7-7s7 3.14 7 7c0 3.859-3.141 7-7 7z");
      attr(path1, "fill", "currentColor");
      attr(path1, "d", "M12 6c-2.757 0-5 2.243-5 5s2.243 5 5 5s5-2.243 5-5s-2.243-5-5-5zm0 8c-1.654 0-3-1.346-3-3s1.346-3 3-3s3 1.346 3 3s-1.346 3-3 3z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
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
class Webcam extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$2, safe_not_equal, {});
  }
}
function create_fragment$1(ctx) {
  let svg;
  let circle;
  let animateTransform;
  return {
    c() {
      svg = svg_element("svg");
      circle = svg_element("circle");
      animateTransform = svg_element("animateTransform");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        viewBox: true,
        class: true
      });
      var svg_nodes = children(svg);
      circle = claim_svg_element(svg_nodes, "circle", {
        cx: true,
        cy: true,
        r: true,
        fill: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-dasharray": true,
        "stroke-dashoffset": true
      });
      var circle_nodes = children(circle);
      animateTransform = claim_svg_element(circle_nodes, "animateTransform", {
        attributeName: true,
        type: true,
        from: true,
        to: true,
        repeatCount: true
      });
      children(animateTransform).forEach(detach);
      circle_nodes.forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(animateTransform, "attributeName", "transform");
      attr(animateTransform, "type", "rotate");
      attr(animateTransform, "from", "0 25 25");
      attr(animateTransform, "to", "360 25 25");
      attr(animateTransform, "repeatCount", "indefinite");
      attr(circle, "cx", "25");
      attr(circle, "cy", "25");
      attr(circle, "r", "20");
      attr(circle, "fill", "none");
      attr(circle, "stroke-width", "3.0");
      attr(circle, "stroke-linecap", "round");
      attr(circle, "stroke-dasharray", "94.2477796076938 94.2477796076938");
      attr(circle, "stroke-dashoffset", "0");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 50 50");
      attr(svg, "class", "svelte-184ngxt");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, circle);
      append_hydration(circle, animateTransform);
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
class Spinner extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$1, safe_not_equal, {});
  }
}
function create_if_block(ctx) {
  let span;
  let show_if_3 = (
    /*sources*/
    ctx[1].includes("upload")
  );
  let t0;
  let show_if_2 = (
    /*sources*/
    ctx[1].includes("microphone")
  );
  let t1;
  let show_if_1 = (
    /*sources*/
    ctx[1].includes("webcam")
  );
  let t2;
  let show_if = (
    /*sources*/
    ctx[1].includes("clipboard")
  );
  let current;
  let if_block0 = show_if_3 && create_if_block_4(ctx);
  let if_block1 = show_if_2 && create_if_block_3(ctx);
  let if_block2 = show_if_1 && create_if_block_2(ctx);
  let if_block3 = show_if && create_if_block_1(ctx);
  return {
    c() {
      span = element("span");
      if (if_block0)
        if_block0.c();
      t0 = space();
      if (if_block1)
        if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      t2 = space();
      if (if_block3)
        if_block3.c();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, "data-testid": true });
      var span_nodes = children(span);
      if (if_block0)
        if_block0.l(span_nodes);
      t0 = claim_space(span_nodes);
      if (if_block1)
        if_block1.l(span_nodes);
      t1 = claim_space(span_nodes);
      if (if_block2)
        if_block2.l(span_nodes);
      t2 = claim_space(span_nodes);
      if (if_block3)
        if_block3.l(span_nodes);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "source-selection svelte-snayfm");
      attr(span, "data-testid", "source-select");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      if (if_block0)
        if_block0.m(span, null);
      append_hydration(span, t0);
      if (if_block1)
        if_block1.m(span, null);
      append_hydration(span, t1);
      if (if_block2)
        if_block2.m(span, null);
      append_hydration(span, t2);
      if (if_block3)
        if_block3.m(span, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*sources*/
      2)
        show_if_3 = /*sources*/
        ctx2[1].includes("upload");
      if (show_if_3) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*sources*/
          2) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(span, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (dirty & /*sources*/
      2)
        show_if_2 = /*sources*/
        ctx2[1].includes("microphone");
      if (show_if_2) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*sources*/
          2) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_3(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(span, t1);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (dirty & /*sources*/
      2)
        show_if_1 = /*sources*/
        ctx2[1].includes("webcam");
      if (show_if_1) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty & /*sources*/
          2) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block_2(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(span, t2);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
      if (dirty & /*sources*/
      2)
        show_if = /*sources*/
        ctx2[1].includes("clipboard");
      if (show_if) {
        if (if_block3) {
          if_block3.p(ctx2, dirty);
          if (dirty & /*sources*/
          2) {
            transition_in(if_block3, 1);
          }
        } else {
          if_block3 = create_if_block_1(ctx2);
          if_block3.c();
          transition_in(if_block3, 1);
          if_block3.m(span, null);
        }
      } else if (if_block3) {
        group_outros();
        transition_out(if_block3, 1, 1, () => {
          if_block3 = null;
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
      transition_in(if_block3);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      transition_out(if_block3);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      if (if_block2)
        if_block2.d();
      if (if_block3)
        if_block3.d();
    }
  };
}
function create_if_block_4(ctx) {
  let button;
  let upload;
  let current;
  let mounted;
  let dispose;
  upload = new Upload({});
  return {
    c() {
      button = element("button");
      create_component(upload.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(upload.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "icon svelte-snayfm");
      attr(button, "aria-label", "Upload file");
      toggle_class(
        button,
        "selected",
        /*active_source*/
        ctx[0] === "upload" || !/*active_source*/
        ctx[0]
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(upload, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler*/
          ctx[6]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (!current || dirty & /*active_source*/
      1) {
        toggle_class(
          button,
          "selected",
          /*active_source*/
          ctx2[0] === "upload" || !/*active_source*/
          ctx2[0]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(upload.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(upload.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(upload);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_3(ctx) {
  let button;
  let microphone;
  let current;
  let mounted;
  let dispose;
  microphone = new Microphone({});
  return {
    c() {
      button = element("button");
      create_component(microphone.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(microphone.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "icon svelte-snayfm");
      attr(button, "aria-label", "Record audio");
      toggle_class(
        button,
        "selected",
        /*active_source*/
        ctx[0] === "microphone"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(microphone, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_1*/
          ctx[7]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (!current || dirty & /*active_source*/
      1) {
        toggle_class(
          button,
          "selected",
          /*active_source*/
          ctx2[0] === "microphone"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(microphone.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(microphone.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(microphone);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_2(ctx) {
  let button;
  let webcam;
  let current;
  let mounted;
  let dispose;
  webcam = new Webcam({});
  return {
    c() {
      button = element("button");
      create_component(webcam.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(webcam.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "icon svelte-snayfm");
      attr(button, "aria-label", "Capture from camera");
      toggle_class(
        button,
        "selected",
        /*active_source*/
        ctx[0] === "webcam"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(webcam, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_2*/
          ctx[8]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (!current || dirty & /*active_source*/
      1) {
        toggle_class(
          button,
          "selected",
          /*active_source*/
          ctx2[0] === "webcam"
        );
      }
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
      if (detaching) {
        detach(button);
      }
      destroy_component(webcam);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_1(ctx) {
  let button;
  let imagepaste;
  let current;
  let mounted;
  let dispose;
  imagepaste = new ImagePaste({});
  return {
    c() {
      button = element("button");
      create_component(imagepaste.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(imagepaste.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "icon svelte-snayfm");
      attr(button, "aria-label", "Paste from clipboard");
      toggle_class(
        button,
        "selected",
        /*active_source*/
        ctx[0] === "clipboard"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(imagepaste, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_3*/
          ctx[9]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (!current || dirty & /*active_source*/
      1) {
        toggle_class(
          button,
          "selected",
          /*active_source*/
          ctx2[0] === "clipboard"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(imagepaste.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(imagepaste.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(imagepaste);
      mounted = false;
      dispose();
    }
  };
}
function create_fragment(ctx) {
  let if_block_anchor;
  let current;
  let if_block = (
    /*unique_sources*/
    ctx[2].length > 1 && create_if_block(ctx)
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
    p(ctx2, [dirty]) {
      if (
        /*unique_sources*/
        ctx2[2].length > 1
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*unique_sources*/
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
function instance($$self, $$props, $$invalidate) {
  let unique_sources;
  let { sources } = $$props;
  let { active_source } = $$props;
  let { handle_clear = () => {
  } } = $$props;
  let { handle_select = () => {
  } } = $$props;
  async function handle_select_source(source) {
    handle_clear();
    $$invalidate(0, active_source = source);
    handle_select(source);
  }
  const click_handler = () => handle_select_source("upload");
  const click_handler_1 = () => handle_select_source("microphone");
  const click_handler_2 = () => handle_select_source("webcam");
  const click_handler_3 = () => handle_select_source("clipboard");
  $$self.$$set = ($$props2) => {
    if ("sources" in $$props2)
      $$invalidate(1, sources = $$props2.sources);
    if ("active_source" in $$props2)
      $$invalidate(0, active_source = $$props2.active_source);
    if ("handle_clear" in $$props2)
      $$invalidate(4, handle_clear = $$props2.handle_clear);
    if ("handle_select" in $$props2)
      $$invalidate(5, handle_select = $$props2.handle_select);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*sources*/
    2) {
      $$invalidate(2, unique_sources = [...new Set(sources)]);
    }
  };
  return [
    active_source,
    sources,
    unique_sources,
    handle_select_source,
    handle_clear,
    handle_select,
    click_handler,
    click_handler_1,
    click_handler_2,
    click_handler_3
  ];
}
class SelectSource extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      sources: 1,
      active_source: 0,
      handle_clear: 4,
      handle_select: 5
    });
  }
}
export {
  SelectSource as S,
  Webcam as W,
  Spinner as a
};
