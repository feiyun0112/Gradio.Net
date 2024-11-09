import { SvelteComponent, init, safe_not_equal, create_slot, element, claim_element, children, detach, attr, toggle_class, insert_hydration, append_hydration, update_slot_base, get_all_dirty_from_scope, get_slot_changes, transition_in, transition_out, binding_callbacks } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
function create_fragment(ctx) {
  let div1;
  let div0;
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[5].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[4],
    null
  );
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true, "aria-label": true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      if (default_slot)
        default_slot.l(div0_nodes);
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "icon svelte-1oiin9d");
      attr(div1, "class", "empty svelte-1oiin9d");
      attr(div1, "aria-label", "Empty value");
      toggle_class(
        div1,
        "small",
        /*size*/
        ctx[0] === "small"
      );
      toggle_class(
        div1,
        "large",
        /*size*/
        ctx[0] === "large"
      );
      toggle_class(
        div1,
        "unpadded_box",
        /*unpadded_box*/
        ctx[1]
      );
      toggle_class(
        div1,
        "small_parent",
        /*parent_height*/
        ctx[3]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      if (default_slot) {
        default_slot.m(div0, null);
      }
      ctx[6](div1);
      current = true;
    },
    p(ctx2, [dirty]) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        16)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[4],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[4]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[4],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty & /*size*/
      1) {
        toggle_class(
          div1,
          "small",
          /*size*/
          ctx2[0] === "small"
        );
      }
      if (!current || dirty & /*size*/
      1) {
        toggle_class(
          div1,
          "large",
          /*size*/
          ctx2[0] === "large"
        );
      }
      if (!current || dirty & /*unpadded_box*/
      2) {
        toggle_class(
          div1,
          "unpadded_box",
          /*unpadded_box*/
          ctx2[1]
        );
      }
      if (!current || dirty & /*parent_height*/
      8) {
        toggle_class(
          div1,
          "small_parent",
          /*parent_height*/
          ctx2[3]
        );
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
      if (detaching) {
        detach(div1);
      }
      if (default_slot)
        default_slot.d(detaching);
      ctx[6](null);
    }
  };
}
function compare_el_to_parent(el2) {
  var _a;
  if (!el2)
    return false;
  const { height: el_height } = el2.getBoundingClientRect();
  const { height: parent_height2 } = ((_a = el2.parentElement) == null ? void 0 : _a.getBoundingClientRect()) || { height: el_height };
  return el_height > parent_height2 + 2;
}
function instance($$self, $$props, $$invalidate) {
  let parent_height;
  let { $$slots: slots = {}, $$scope } = $$props;
  let { size = "small" } = $$props;
  let { unpadded_box = false } = $$props;
  let el;
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      el = $$value;
      $$invalidate(2, el);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("size" in $$props2)
      $$invalidate(0, size = $$props2.size);
    if ("unpadded_box" in $$props2)
      $$invalidate(1, unpadded_box = $$props2.unpadded_box);
    if ("$$scope" in $$props2)
      $$invalidate(4, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*el*/
    4) {
      $$invalidate(3, parent_height = compare_el_to_parent(el));
    }
  };
  return [size, unpadded_box, el, parent_height, $$scope, slots, div1_binding];
}
class Empty extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { size: 0, unpadded_box: 1 });
  }
}
export {
  Empty as E
};
