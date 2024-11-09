import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let svg;
  let polygon;
  let rect;
  return {
    c() {
      svg = svg_element("svg");
      polygon = svg_element("polygon");
      rect = svg_element("rect");
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
      polygon = claim_svg_element(svg_nodes, "polygon", { points: true });
      children(polygon).forEach(detach);
      rect = claim_svg_element(svg_nodes, "rect", {
        x: true,
        y: true,
        width: true,
        height: true,
        rx: true,
        ry: true
      });
      children(rect).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(polygon, "points", "23 7 16 12 23 17 23 7");
      attr(rect, "x", "1");
      attr(rect, "y", "5");
      attr(rect, "width", "15");
      attr(rect, "height", "14");
      attr(rect, "rx", "2");
      attr(rect, "ry", "2");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-video");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, polygon);
      append_hydration(svg, rect);
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
class Video extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment, safe_not_equal, {});
  }
}
export {
  Video as V
};
