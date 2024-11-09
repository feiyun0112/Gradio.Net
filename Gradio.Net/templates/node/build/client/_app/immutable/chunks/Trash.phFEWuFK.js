import { SvelteComponent, init, safe_not_equal, svg_element, text, claim_svg_element, children, claim_text, detach, attr, insert_hydration, append_hydration, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let svg;
  let defs;
  let style;
  let t;
  let rect0;
  let rect1;
  let path;
  let rect2;
  let rect3;
  return {
    c() {
      svg = svg_element("svg");
      defs = svg_element("defs");
      style = svg_element("style");
      t = text(".cls-1 {\n				fill: none;\n			}");
      rect0 = svg_element("rect");
      rect1 = svg_element("rect");
      path = svg_element("path");
      rect2 = svg_element("rect");
      rect3 = svg_element("rect");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        id: true,
        xmlns: true,
        viewBox: true,
        fill: true,
        width: true,
        height: true
      });
      var svg_nodes = children(svg);
      defs = claim_svg_element(svg_nodes, "defs", {});
      var defs_nodes = children(defs);
      style = claim_svg_element(defs_nodes, "style", {});
      var style_nodes = children(style);
      t = claim_text(style_nodes, ".cls-1 {\n				fill: none;\n			}");
      style_nodes.forEach(detach);
      defs_nodes.forEach(detach);
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
      path = claim_svg_element(svg_nodes, "path", { d: true });
      children(path).forEach(detach);
      rect2 = claim_svg_element(svg_nodes, "rect", {
        x: true,
        y: true,
        width: true,
        height: true
      });
      children(rect2).forEach(detach);
      rect3 = claim_svg_element(svg_nodes, "rect", {
        id: true,
        "data-name": true,
        class: true,
        width: true,
        height: true
      });
      children(rect3).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(rect0, "x", "12");
      attr(rect0, "y", "12");
      attr(rect0, "width", "2");
      attr(rect0, "height", "12");
      attr(rect1, "x", "18");
      attr(rect1, "y", "12");
      attr(rect1, "width", "2");
      attr(rect1, "height", "12");
      attr(path, "d", "M4,6V8H6V28a2,2,0,0,0,2,2H24a2,2,0,0,0,2-2V8h2V6ZM8,28V8H24V28Z");
      attr(rect2, "x", "12");
      attr(rect2, "y", "2");
      attr(rect2, "width", "8");
      attr(rect2, "height", "2");
      attr(rect3, "id", "_Transparent_Rectangle_");
      attr(rect3, "data-name", "<Transparent Rectangle>");
      attr(rect3, "class", "cls-1");
      attr(rect3, "width", "32");
      attr(rect3, "height", "32");
      attr(svg, "id", "icon");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "viewBox", "0 0 32 32");
      attr(svg, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, defs);
      append_hydration(defs, style);
      append_hydration(style, t);
      append_hydration(svg, rect0);
      append_hydration(svg, rect1);
      append_hydration(svg, path);
      append_hydration(svg, rect2);
      append_hydration(svg, rect3);
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
class Trash extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment, safe_not_equal, {});
  }
}
export {
  Trash as T
};
