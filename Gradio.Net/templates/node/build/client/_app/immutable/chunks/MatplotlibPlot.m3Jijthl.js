import { SvelteComponent, init, safe_not_equal, element, claim_element, children, detach, src_url_equal, attr, insert_hydration, append_hydration, listen, noop, bubble } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div;
  let img;
  let img_src_value;
  let img_alt_value;
  let mounted;
  let dispose;
  return {
    c() {
      div = element("div");
      img = element("img");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { "data-testid": true, class: true });
      var div_nodes = children(div);
      img = claim_element(div_nodes, "IMG", { src: true, alt: true, class: true });
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      if (!src_url_equal(img.src, img_src_value = /*plot*/
      ctx[1]))
        attr(img, "src", img_src_value);
      attr(img, "alt", img_alt_value = `${/*value*/
      ctx[0].chart} plot visualising provided data`);
      attr(img, "class", "svelte-j1jcu3");
      attr(div, "data-testid", "matplotlib");
      attr(div, "class", "matplotlib layout svelte-j1jcu3");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, img);
      if (!mounted) {
        dispose = listen(
          img,
          "load",
          /*load_handler*/
          ctx[2]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*plot*/
      2 && !src_url_equal(img.src, img_src_value = /*plot*/
      ctx2[1])) {
        attr(img, "src", img_src_value);
      }
      if (dirty & /*value*/
      1 && img_alt_value !== (img_alt_value = `${/*value*/
      ctx2[0].chart} plot visualising provided data`)) {
        attr(img, "alt", img_alt_value);
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      mounted = false;
      dispose();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let plot;
  let { value } = $$props;
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    1) {
      $$invalidate(1, plot = value == null ? void 0 : value.plot);
    }
  };
  return [value, plot, load_handler];
}
class MatplotlibPlot extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0 });
  }
}
export {
  MatplotlibPlot as default
};
