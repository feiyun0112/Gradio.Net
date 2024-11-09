import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let svg;
  let rect;
  let circle;
  let polyline;
  return {
    c() {
      svg = svg_element("svg");
      rect = svg_element("rect");
      circle = svg_element("circle");
      polyline = svg_element("polyline");
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
      rect = claim_svg_element(svg_nodes, "rect", {
        x: true,
        y: true,
        width: true,
        height: true,
        rx: true,
        ry: true
      });
      children(rect).forEach(detach);
      circle = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true });
      children(circle).forEach(detach);
      polyline = claim_svg_element(svg_nodes, "polyline", { points: true });
      children(polyline).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(rect, "x", "3");
      attr(rect, "y", "3");
      attr(rect, "width", "18");
      attr(rect, "height", "18");
      attr(rect, "rx", "2");
      attr(rect, "ry", "2");
      attr(circle, "cx", "8.5");
      attr(circle, "cy", "8.5");
      attr(circle, "r", "1.5");
      attr(polyline, "points", "21 15 16 10 5 21");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-image");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, rect);
      append_hydration(svg, circle);
      append_hydration(svg, polyline);
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
class Image extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment, safe_not_equal, {});
  }
}
export {
  Image as I
};
