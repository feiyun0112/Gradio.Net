import { SvelteComponent, init, safe_not_equal, assign, element, claim_element, set_attributes, toggle_class, insert_hydration, listen, get_spread_update, src_url_equal, noop, detach, compute_rest_props, exclude_internal_props, bubble } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { r as resolve_wasm_src } from "./file-url.Bs-FMz4v.js";
/* empty css                                                    */
function create_fragment(ctx) {
  let img;
  let img_src_value;
  let mounted;
  let dispose;
  let img_levels = [
    {
      src: img_src_value = /*resolved_src*/
      ctx[0]
    },
    /*$$restProps*/
    ctx[1]
  ];
  let img_data = {};
  for (let i = 0; i < img_levels.length; i += 1) {
    img_data = assign(img_data, img_levels[i]);
  }
  return {
    c() {
      img = element("img");
      this.h();
    },
    l(nodes) {
      img = claim_element(nodes, "IMG", { src: true });
      this.h();
    },
    h() {
      set_attributes(img, img_data);
      toggle_class(img, "svelte-1pijsyv", true);
    },
    m(target, anchor) {
      insert_hydration(target, img, anchor);
      if (!mounted) {
        dispose = listen(
          img,
          "load",
          /*load_handler*/
          ctx[4]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      set_attributes(img, img_data = get_spread_update(img_levels, [
        dirty & /*resolved_src*/
        1 && !src_url_equal(img.src, img_src_value = /*resolved_src*/
        ctx2[0]) && { src: img_src_value },
        dirty & /*$$restProps*/
        2 && /*$$restProps*/
        ctx2[1]
      ]));
      toggle_class(img, "svelte-1pijsyv", true);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(img);
      }
      mounted = false;
      dispose();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  const omit_props_names = ["src"];
  let $$restProps = compute_rest_props($$props, omit_props_names);
  let { src = void 0 } = $$props;
  let resolved_src;
  let latest_src;
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$new_props) => {
    $$props = assign(assign({}, $$props), exclude_internal_props($$new_props));
    $$invalidate(1, $$restProps = compute_rest_props($$props, omit_props_names));
    if ("src" in $$new_props)
      $$invalidate(2, src = $$new_props.src);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*src, latest_src*/
    12) {
      {
        $$invalidate(0, resolved_src = src);
        $$invalidate(3, latest_src = src);
        const resolving_src = src;
        resolve_wasm_src(resolving_src).then((s) => {
          if (latest_src === resolving_src) {
            $$invalidate(0, resolved_src = s);
          }
        });
      }
    }
  };
  return [resolved_src, $$restProps, src, latest_src, load_handler];
}
class Image extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { src: 2 });
  }
}
const Image$1 = Image;
export {
  Image$1 as I
};
