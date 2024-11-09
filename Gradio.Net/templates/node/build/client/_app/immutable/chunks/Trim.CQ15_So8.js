import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment$1(ctx) {
  let svg;
  let rect0;
  let rect1;
  return {
    c() {
      svg = svg_element("svg");
      rect0 = svg_element("rect");
      rect1 = svg_element("rect");
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
      rect0 = claim_svg_element(svg_nodes, "rect", {
        x: true,
        y: true,
        width: true,
        height: true
      });
      children(rect0).forEach(detach);
      rect1 = claim_svg_element(svg_nodes, "rect", {
        x: true,
        y: true,
        width: true,
        height: true
      });
      children(rect1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(rect0, "x", "6");
      attr(rect0, "y", "4");
      attr(rect0, "width", "4");
      attr(rect0, "height", "16");
      attr(rect1, "x", "14");
      attr(rect1, "y", "4");
      attr(rect1, "width", "4");
      attr(rect1, "height", "16");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "currentColor");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, rect0);
      append_hydration(svg, rect1);
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
class Pause extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$1, safe_not_equal, {});
  }
}
function create_fragment(ctx) {
  let svg;
  let circle0;
  let circle1;
  let line0;
  let line1;
  let line2;
  return {
    c() {
      svg = svg_element("svg");
      circle0 = svg_element("circle");
      circle1 = svg_element("circle");
      line0 = svg_element("line");
      line1 = svg_element("line");
      line2 = svg_element("line");
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
      circle0 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true });
      children(circle0).forEach(detach);
      circle1 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true });
      children(circle1).forEach(detach);
      line0 = claim_svg_element(svg_nodes, "line", { x1: true, y1: true, x2: true, y2: true });
      children(line0).forEach(detach);
      line1 = claim_svg_element(svg_nodes, "line", { x1: true, y1: true, x2: true, y2: true });
      children(line1).forEach(detach);
      line2 = claim_svg_element(svg_nodes, "line", { x1: true, y1: true, x2: true, y2: true });
      children(line2).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(circle0, "cx", "6");
      attr(circle0, "cy", "6");
      attr(circle0, "r", "3");
      attr(circle1, "cx", "6");
      attr(circle1, "cy", "18");
      attr(circle1, "r", "3");
      attr(line0, "x1", "20");
      attr(line0, "y1", "4");
      attr(line0, "x2", "8.12");
      attr(line0, "y2", "15.88");
      attr(line1, "x1", "14.47");
      attr(line1, "y1", "14.48");
      attr(line1, "x2", "20");
      attr(line1, "y2", "20");
      attr(line2, "x1", "8.12");
      attr(line2, "y1", "8.12");
      attr(line2, "x2", "12");
      attr(line2, "y2", "12");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "2");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-scissors");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, circle0);
      append_hydration(svg, circle1);
      append_hydration(svg, line0);
      append_hydration(svg, line1);
      append_hydration(svg, line2);
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
class Trim extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment, safe_not_equal, {});
  }
}
export {
  Pause as P,
  Trim as T
};
