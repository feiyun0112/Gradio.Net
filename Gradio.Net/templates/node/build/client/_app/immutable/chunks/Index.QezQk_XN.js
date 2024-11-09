import { SvelteComponent, init, safe_not_equal, create_slot, element, claim_element, children, detach, attr, set_style, toggle_class, insert_hydration, append_hydration, update_slot_base, get_all_dirty_from_scope, get_slot_changes, transition_in, transition_out } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div1;
  let div0;
  let div1_class_value;
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[4].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[3],
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
      div1 = claim_element(nodes, "DIV", { id: true, class: true });
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
      attr(div0, "class", "styler svelte-1nguped");
      set_style(div0, "--block-radius", `0px`);
      set_style(div0, "--block-border-width", `0px`);
      set_style(div0, "--layout-gap", `1px`);
      set_style(div0, "--form-gap-width", `1px`);
      set_style(div0, "--button-border-width", `0px`);
      set_style(div0, "--button-large-radius", `0px`);
      set_style(div0, "--button-small-radius", `0px`);
      attr(
        div1,
        "id",
        /*elem_id*/
        ctx[0]
      );
      attr(div1, "class", div1_class_value = "gr-group " + /*elem_classes*/
      ctx[1].join(" ") + " svelte-1nguped");
      toggle_class(div1, "hide", !/*visible*/
      ctx[2]);
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      if (default_slot) {
        default_slot.m(div0, null);
      }
      current = true;
    },
    p(ctx2, [dirty]) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        8)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[3],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[3]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[3],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty & /*elem_id*/
      1) {
        attr(
          div1,
          "id",
          /*elem_id*/
          ctx2[0]
        );
      }
      if (!current || dirty & /*elem_classes*/
      2 && div1_class_value !== (div1_class_value = "gr-group " + /*elem_classes*/
      ctx2[1].join(" ") + " svelte-1nguped")) {
        attr(div1, "class", div1_class_value);
      }
      if (!current || dirty & /*elem_classes, visible*/
      6) {
        toggle_class(div1, "hide", !/*visible*/
        ctx2[2]);
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
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("$$scope" in $$props2)
      $$invalidate(3, $$scope = $$props2.$$scope);
  };
  return [elem_id, elem_classes, visible, $$scope, slots];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { elem_id: 0, elem_classes: 1, visible: 2 });
  }
}
export {
  Index as default
};
