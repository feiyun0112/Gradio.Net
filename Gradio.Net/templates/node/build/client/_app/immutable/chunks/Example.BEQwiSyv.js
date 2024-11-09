import { SvelteComponent, init, safe_not_equal, element, create_component, claim_element, children, claim_component, detach, attr, toggle_class, insert_hydration, mount_component, transition_in, transition_out, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
import { I as Image } from "./Image.eJ_qOnkr.js";
/* empty css                                                    */
/* empty css                                                    */
import "./ImageUploader.D4fj6zSl.js";
/* empty css                                               */
function create_fragment(ctx) {
  var _a, _b;
  let div;
  let image;
  let current;
  image = new Image({
    props: {
      src: (
        /*value*/
        ((_a = ctx[0].composite) == null ? void 0 : _a.url) || /*value*/
        ((_b = ctx[0].background) == null ? void 0 : _b.url)
      ),
      alt: ""
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
      attr(div, "class", "container svelte-jhlhb0");
      toggle_class(
        div,
        "table",
        /*type*/
        ctx[1] === "table"
      );
      toggle_class(
        div,
        "gallery",
        /*type*/
        ctx[1] === "gallery"
      );
      toggle_class(
        div,
        "selected",
        /*selected*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(image, div, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      var _a2, _b2;
      const image_changes = {};
      if (dirty & /*value*/
      1)
        image_changes.src = /*value*/
        ((_a2 = ctx2[0].composite) == null ? void 0 : _a2.url) || /*value*/
        ((_b2 = ctx2[0].background) == null ? void 0 : _b2.url);
      image.$set(image_changes);
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (!current || dirty & /*selected*/
      4) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx2[2]
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
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
  };
  return [value, type, selected];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0, type: 1, selected: 2 });
  }
}
export {
  Example as default
};
