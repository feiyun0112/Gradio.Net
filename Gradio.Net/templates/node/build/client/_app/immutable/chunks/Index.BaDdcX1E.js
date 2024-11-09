import { SvelteComponent, init, safe_not_equal, element, create_component, claim_element, children, claim_component, detach, attr, set_style, insert_hydration, mount_component, transition_in, transition_out, destroy_component, createEventDispatcher, getContext, component_subscribe, onMount, tick, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { a as TABS } from "./Tabs.D7V6q4qh.js";
import { i as Index$1 } from "./2.BqWhUxOo.js";
function create_default_slot$1(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[11].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[12],
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
        4096)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[12],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[12]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[12],
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
function create_fragment$1(ctx) {
  let div;
  let column;
  let div_class_value;
  let current;
  column = new Index$1({
    props: {
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      div = element("div");
      create_component(column.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { id: true, class: true, role: true });
      var div_nodes = children(div);
      claim_component(column.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(
        div,
        "id",
        /*elem_id*/
        ctx[0]
      );
      attr(div, "class", div_class_value = "tabitem " + /*elem_classes*/
      ctx[1].join(" ") + " svelte-tcemt9");
      attr(div, "role", "tabpanel");
      set_style(
        div,
        "display",
        /*$selected_tab*/
        ctx[4] === /*id*/
        ctx[2] && /*visible*/
        ctx[3] ? "block" : "none"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(column, div, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      const column_changes = {};
      if (dirty & /*$$scope*/
      4096) {
        column_changes.$$scope = { dirty, ctx: ctx2 };
      }
      column.$set(column_changes);
      if (!current || dirty & /*elem_id*/
      1) {
        attr(
          div,
          "id",
          /*elem_id*/
          ctx2[0]
        );
      }
      if (!current || dirty & /*elem_classes*/
      2 && div_class_value !== (div_class_value = "tabitem " + /*elem_classes*/
      ctx2[1].join(" ") + " svelte-tcemt9")) {
        attr(div, "class", div_class_value);
      }
      if (dirty & /*$selected_tab, id, visible*/
      28) {
        set_style(
          div,
          "display",
          /*$selected_tab*/
          ctx2[4] === /*id*/
          ctx2[2] && /*visible*/
          ctx2[3] ? "block" : "none"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(column.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(column.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(column);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let $selected_tab_index;
  let $selected_tab;
  let { $$slots: slots = {}, $$scope } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { label } = $$props;
  let { id = {} } = $$props;
  let { visible } = $$props;
  let { interactive } = $$props;
  const dispatch = createEventDispatcher();
  const { register_tab, unregister_tab, selected_tab, selected_tab_index } = getContext(TABS);
  component_subscribe($$self, selected_tab, (value) => $$invalidate(4, $selected_tab = value));
  component_subscribe($$self, selected_tab_index, (value) => $$invalidate(10, $selected_tab_index = value));
  let tab_index;
  onMount(() => {
    return () => unregister_tab({ label, id, elem_id });
  });
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("label" in $$props2)
      $$invalidate(7, label = $$props2.label);
    if ("id" in $$props2)
      $$invalidate(2, id = $$props2.id);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("interactive" in $$props2)
      $$invalidate(8, interactive = $$props2.interactive);
    if ("$$scope" in $$props2)
      $$invalidate(12, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*label, id, elem_id, visible, interactive*/
    397) {
      $$invalidate(9, tab_index = register_tab({ label, id, elem_id, visible, interactive }));
    }
    if ($$self.$$.dirty & /*$selected_tab_index, tab_index, label*/
    1664) {
      $selected_tab_index === tab_index && tick().then(() => dispatch("select", { value: label, index: tab_index }));
    }
  };
  return [
    elem_id,
    elem_classes,
    id,
    visible,
    $selected_tab,
    selected_tab,
    selected_tab_index,
    label,
    interactive,
    tab_index,
    $selected_tab_index,
    slots,
    $$scope
  ];
}
class TabItem extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      label: 7,
      id: 2,
      visible: 3,
      interactive: 8
    });
  }
}
const TabItem$1 = TabItem;
function create_default_slot(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[7].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[9],
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
        512)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[9],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[9]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[9],
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
  let tabitem;
  let current;
  tabitem = new TabItem$1({
    props: {
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      label: (
        /*label*/
        ctx[2]
      ),
      visible: (
        /*visible*/
        ctx[5]
      ),
      interactive: (
        /*interactive*/
        ctx[6]
      ),
      id: (
        /*id*/
        ctx[3]
      ),
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  tabitem.$on(
    "select",
    /*select_handler*/
    ctx[8]
  );
  return {
    c() {
      create_component(tabitem.$$.fragment);
    },
    l(nodes) {
      claim_component(tabitem.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(tabitem, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const tabitem_changes = {};
      if (dirty & /*elem_id*/
      1)
        tabitem_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        tabitem_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*label*/
      4)
        tabitem_changes.label = /*label*/
        ctx2[2];
      if (dirty & /*visible*/
      32)
        tabitem_changes.visible = /*visible*/
        ctx2[5];
      if (dirty & /*interactive*/
      64)
        tabitem_changes.interactive = /*interactive*/
        ctx2[6];
      if (dirty & /*id*/
      8)
        tabitem_changes.id = /*id*/
        ctx2[3];
      if (dirty & /*$$scope*/
      512) {
        tabitem_changes.$$scope = { dirty, ctx: ctx2 };
      }
      tabitem.$set(tabitem_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(tabitem.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(tabitem.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(tabitem, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { label } = $$props;
  let { id } = $$props;
  let { gradio } = $$props;
  let { visible = true } = $$props;
  let { interactive = true } = $$props;
  const select_handler = ({ detail }) => gradio == null ? void 0 : gradio.dispatch("select", detail);
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("id" in $$props2)
      $$invalidate(3, id = $$props2.id);
    if ("gradio" in $$props2)
      $$invalidate(4, gradio = $$props2.gradio);
    if ("visible" in $$props2)
      $$invalidate(5, visible = $$props2.visible);
    if ("interactive" in $$props2)
      $$invalidate(6, interactive = $$props2.interactive);
    if ("$$scope" in $$props2)
      $$invalidate(9, $$scope = $$props2.$$scope);
  };
  return [
    elem_id,
    elem_classes,
    label,
    id,
    gradio,
    visible,
    interactive,
    slots,
    select_handler,
    $$scope
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      label: 2,
      id: 3,
      gradio: 4,
      visible: 5,
      interactive: 6
    });
  }
}
export {
  TabItem$1 as BaseTabItem,
  Index as default
};
