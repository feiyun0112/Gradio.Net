import { SvelteComponent, init, safe_not_equal, create_slot, element, text, space, claim_element, children, claim_text, detach, claim_space, get_svelte_dataset, attr, set_style, toggle_class, insert_hydration, append_hydration, listen, set_data, update_slot_base, get_all_dirty_from_scope, get_slot_changes, transition_in, transition_out, createEventDispatcher, create_component, claim_component, mount_component, destroy_component, assign, binding_callbacks, bind, get_spread_update, get_spread_object, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static, i as Index$1 } from "./2.BqWhUxOo.js";
function create_fragment$1(ctx) {
  let button;
  let span0;
  let t0;
  let t1;
  let span1;
  let textContent = "▼";
  let t3;
  let div;
  let current;
  let mounted;
  let dispose;
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
      button = element("button");
      span0 = element("span");
      t0 = text(
        /*label*/
        ctx[1]
      );
      t1 = space();
      span1 = element("span");
      span1.textContent = textContent;
      t3 = space();
      div = element("div");
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      span0 = claim_element(button_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(
        span0_nodes,
        /*label*/
        ctx[1]
      );
      span0_nodes.forEach(detach);
      t1 = claim_space(button_nodes);
      span1 = claim_element(button_nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span1) !== "svelte-1mqwc8d")
        span1.textContent = textContent;
      button_nodes.forEach(detach);
      t3 = claim_space(nodes);
      div = claim_element(nodes, "DIV", {});
      var div_nodes = children(div);
      if (default_slot)
        default_slot.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "svelte-1w6vloh");
      attr(span1, "class", "icon svelte-1w6vloh");
      set_style(
        span1,
        "transform",
        /*open*/
        ctx[0] ? "rotate(0)" : "rotate(90deg)"
      );
      attr(button, "class", "label-wrap svelte-1w6vloh");
      toggle_class(
        button,
        "open",
        /*open*/
        ctx[0]
      );
      set_style(
        div,
        "display",
        /*open*/
        ctx[0] ? "block" : "none"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, span0);
      append_hydration(span0, t0);
      append_hydration(button, t1);
      append_hydration(button, span1);
      insert_hydration(target, t3, anchor);
      insert_hydration(target, div, anchor);
      if (default_slot) {
        default_slot.m(div, null);
      }
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler*/
          ctx[5]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (!current || dirty & /*label*/
      2)
        set_data(
          t0,
          /*label*/
          ctx2[1]
        );
      if (dirty & /*open*/
      1) {
        set_style(
          span1,
          "transform",
          /*open*/
          ctx2[0] ? "rotate(0)" : "rotate(90deg)"
        );
      }
      if (!current || dirty & /*open*/
      1) {
        toggle_class(
          button,
          "open",
          /*open*/
          ctx2[0]
        );
      }
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
      if (dirty & /*open*/
      1) {
        set_style(
          div,
          "display",
          /*open*/
          ctx2[0] ? "block" : "none"
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
        detach(button);
        detach(t3);
        detach(div);
      }
      if (default_slot)
        default_slot.d(detaching);
      mounted = false;
      dispose();
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  const dispatch = createEventDispatcher();
  let { open = true } = $$props;
  let { label = "" } = $$props;
  const click_handler = () => {
    $$invalidate(0, open = !open);
    if (open) {
      dispatch("expand");
    } else {
      dispatch("collapse");
    }
  };
  $$self.$$set = ($$props2) => {
    if ("open" in $$props2)
      $$invalidate(0, open = $$props2.open);
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("$$scope" in $$props2)
      $$invalidate(3, $$scope = $$props2.$$scope);
  };
  return [open, label, dispatch, $$scope, slots, click_handler];
}
class Accordion extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, { open: 0, label: 1 });
  }
}
function create_default_slot_2(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[7].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[11],
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
        2048)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[11],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[11]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[11],
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
function create_default_slot_1(ctx) {
  let column;
  let current;
  column = new Index$1({
    props: {
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(column.$$.fragment);
    },
    l(nodes) {
      claim_component(column.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(column, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const column_changes = {};
      if (dirty & /*$$scope*/
      2048) {
        column_changes.$$scope = { dirty, ctx: ctx2 };
      }
      column.$set(column_changes);
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
      destroy_component(column, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t;
  let accordion;
  let updating_open;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[6].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[6].i18n
    ) },
    /*loading_status*/
    ctx[5]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  function accordion_open_binding(value) {
    ctx[8](value);
  }
  let accordion_props = {
    label: (
      /*label*/
      ctx[1]
    ),
    $$slots: { default: [create_default_slot_1] },
    $$scope: { ctx }
  };
  if (
    /*open*/
    ctx[0] !== void 0
  ) {
    accordion_props.open = /*open*/
    ctx[0];
  }
  accordion = new Accordion({ props: accordion_props });
  binding_callbacks.push(() => bind(accordion, "open", accordion_open_binding));
  accordion.$on(
    "expand",
    /*expand_handler*/
    ctx[9]
  );
  accordion.$on(
    "collapse",
    /*collapse_handler*/
    ctx[10]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(accordion.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(accordion.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(accordion, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      96 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        64 && { autoscroll: (
          /*gradio*/
          ctx2[6].autoscroll
        ) },
        dirty & /*gradio*/
        64 && { i18n: (
          /*gradio*/
          ctx2[6].i18n
        ) },
        dirty & /*loading_status*/
        32 && get_spread_object(
          /*loading_status*/
          ctx2[5]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const accordion_changes = {};
      if (dirty & /*label*/
      2)
        accordion_changes.label = /*label*/
        ctx2[1];
      if (dirty & /*$$scope*/
      2048) {
        accordion_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_open && dirty & /*open*/
      1) {
        updating_open = true;
        accordion_changes.open = /*open*/
        ctx2[0];
        add_flush_callback(() => updating_open = false);
      }
      accordion.$set(accordion_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(accordion.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(accordion.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(accordion, detaching);
    }
  };
}
function create_fragment(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      elem_id: (
        /*elem_id*/
        ctx[2]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[3]
      ),
      visible: (
        /*visible*/
        ctx[4]
      ),
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(block.$$.fragment);
    },
    l(nodes) {
      claim_component(block.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(block, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const block_changes = {};
      if (dirty & /*elem_id*/
      4)
        block_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty & /*elem_classes*/
      8)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[3];
      if (dirty & /*visible*/
      16)
        block_changes.visible = /*visible*/
        ctx2[4];
      if (dirty & /*$$scope, label, open, gradio, loading_status*/
      2147) {
        block_changes.$$scope = { dirty, ctx: ctx2 };
      }
      block.$set(block_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(block.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(block.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(block, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { label } = $$props;
  let { elem_id } = $$props;
  let { elem_classes } = $$props;
  let { visible = true } = $$props;
  let { open = true } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  function accordion_open_binding(value) {
    open = value;
    $$invalidate(0, open);
  }
  const expand_handler = () => gradio.dispatch("expand");
  const collapse_handler = () => gradio.dispatch("collapse");
  $$self.$$set = ($$props2) => {
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("elem_id" in $$props2)
      $$invalidate(2, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(3, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(4, visible = $$props2.visible);
    if ("open" in $$props2)
      $$invalidate(0, open = $$props2.open);
    if ("loading_status" in $$props2)
      $$invalidate(5, loading_status = $$props2.loading_status);
    if ("gradio" in $$props2)
      $$invalidate(6, gradio = $$props2.gradio);
    if ("$$scope" in $$props2)
      $$invalidate(11, $$scope = $$props2.$$scope);
  };
  return [
    open,
    label,
    elem_id,
    elem_classes,
    visible,
    loading_status,
    gradio,
    slots,
    accordion_open_binding,
    expand_handler,
    collapse_handler,
    $$scope
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      label: 1,
      elem_id: 2,
      elem_classes: 3,
      visible: 4,
      open: 0,
      loading_status: 5,
      gradio: 6
    });
  }
}
export {
  Index as default
};
