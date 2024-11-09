import { SvelteComponent, init, safe_not_equal, binding_callbacks, bind, create_component, claim_component, mount_component, add_flush_callback, transition_in, transition_out, destroy_component, createEventDispatcher, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { T as Tabs } from "./Tabs.D7V6q4qh.js";
import { a } from "./Tabs.D7V6q4qh.js";
function create_default_slot(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[6].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[10],
    null
  );
  return {
    c() {
      if (default_slot)
        default_slot.c();
    },
    l(nodes) {
      if (default_slot)
        default_slot.l(nodes);
    },
    m(target, anchor) {
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        1024)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[10],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[10]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[10],
              dirty,
              null
            ),
            null
          );
        }
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
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_fragment(ctx) {
  let tabs;
  let updating_selected;
  let current;
  function tabs_selected_binding(value) {
    ctx[7](value);
  }
  let tabs_props = {
    visible: (
      /*visible*/
      ctx[1]
    ),
    elem_id: (
      /*elem_id*/
      ctx[2]
    ),
    elem_classes: (
      /*elem_classes*/
      ctx[3]
    ),
    initial_tabs: (
      /*initial_tabs*/
      ctx[4]
    ),
    $$slots: { default: [create_default_slot] },
    $$scope: { ctx }
  };
  if (
    /*selected*/
    ctx[0] !== void 0
  ) {
    tabs_props.selected = /*selected*/
    ctx[0];
  }
  tabs = new Tabs({ props: tabs_props });
  binding_callbacks.push(() => bind(tabs, "selected", tabs_selected_binding));
  tabs.$on(
    "change",
    /*change_handler*/
    ctx[8]
  );
  tabs.$on(
    "select",
    /*select_handler*/
    ctx[9]
  );
  return {
    c() {
      create_component(tabs.$$.fragment);
    },
    l(nodes) {
      claim_component(tabs.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(tabs, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const tabs_changes = {};
      if (dirty & /*visible*/
      2)
        tabs_changes.visible = /*visible*/
        ctx2[1];
      if (dirty & /*elem_id*/
      4)
        tabs_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty & /*elem_classes*/
      8)
        tabs_changes.elem_classes = /*elem_classes*/
        ctx2[3];
      if (dirty & /*initial_tabs*/
      16)
        tabs_changes.initial_tabs = /*initial_tabs*/
        ctx2[4];
      if (dirty & /*$$scope*/
      1024) {
        tabs_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_selected && dirty & /*selected*/
      1) {
        updating_selected = true;
        tabs_changes.selected = /*selected*/
        ctx2[0];
        add_flush_callback(() => updating_selected = false);
      }
      tabs.$set(tabs_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(tabs.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(tabs.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(tabs, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  const dispatch = createEventDispatcher();
  let { visible = true } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { selected } = $$props;
  let { initial_tabs = [] } = $$props;
  let { gradio } = $$props;
  function tabs_selected_binding(value) {
    selected = value;
    $$invalidate(0, selected);
  }
  const change_handler = () => gradio == null ? void 0 : gradio.dispatch("change");
  const select_handler = (e) => gradio == null ? void 0 : gradio.dispatch("select", e.detail);
  $$self.$$set = ($$props2) => {
    if ("visible" in $$props2)
      $$invalidate(1, visible = $$props2.visible);
    if ("elem_id" in $$props2)
      $$invalidate(2, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(3, elem_classes = $$props2.elem_classes);
    if ("selected" in $$props2)
      $$invalidate(0, selected = $$props2.selected);
    if ("initial_tabs" in $$props2)
      $$invalidate(4, initial_tabs = $$props2.initial_tabs);
    if ("gradio" in $$props2)
      $$invalidate(5, gradio = $$props2.gradio);
    if ("$$scope" in $$props2)
      $$invalidate(10, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*selected*/
    1) {
      dispatch("prop_change", { selected });
    }
  };
  return [
    selected,
    visible,
    elem_id,
    elem_classes,
    initial_tabs,
    gradio,
    slots,
    tabs_selected_binding,
    change_handler,
    select_handler,
    $$scope
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      visible: 1,
      elem_id: 2,
      elem_classes: 3,
      selected: 0,
      initial_tabs: 4,
      gradio: 5
    });
  }
}
export {
  Tabs as BaseTabs,
  a as TABS,
  Index as default
};
