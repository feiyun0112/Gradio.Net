import { SvelteComponent, init, safe_not_equal, element, space, text, claim_element, children, claim_space, claim_text, detach, attr, toggle_class, insert_hydration, append_hydration, listen, set_data, noop, run_all, createEventDispatcher, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, afterUpdate, assign, binding_callbacks, bind, get_spread_update, get_spread_object, group_outros, check_outros, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static, j as Info } from "./2.BqWhUxOo.js";
function create_fragment$1(ctx) {
  let label_1;
  let input;
  let t0;
  let span;
  let t1;
  let mounted;
  let dispose;
  return {
    c() {
      label_1 = element("label");
      input = element("input");
      t0 = space();
      span = element("span");
      t1 = text(
        /*label*/
        ctx[1]
      );
      this.h();
    },
    l(nodes) {
      label_1 = claim_element(nodes, "LABEL", { class: true });
      var label_1_nodes = children(label_1);
      input = claim_element(label_1_nodes, "INPUT", {
        type: true,
        name: true,
        "data-testid": true,
        class: true
      });
      t0 = claim_space(label_1_nodes);
      span = claim_element(label_1_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t1 = claim_text(
        span_nodes,
        /*label*/
        ctx[1]
      );
      span_nodes.forEach(detach);
      label_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      input.disabled = /*disabled*/
      ctx[2];
      attr(input, "type", "checkbox");
      attr(input, "name", "test");
      attr(input, "data-testid", "checkbox");
      attr(input, "class", "svelte-5ncdh7");
      attr(span, "class", "svelte-5ncdh7");
      attr(label_1, "class", "svelte-5ncdh7");
      toggle_class(
        label_1,
        "disabled",
        /*disabled*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, label_1, anchor);
      append_hydration(label_1, input);
      input.checked = /*value*/
      ctx[0];
      append_hydration(label_1, t0);
      append_hydration(label_1, span);
      append_hydration(span, t1);
      if (!mounted) {
        dispose = [
          listen(
            input,
            "change",
            /*input_change_handler*/
            ctx[6]
          ),
          listen(
            input,
            "keydown",
            /*handle_enter*/
            ctx[3]
          ),
          listen(
            input,
            "input",
            /*handle_input*/
            ctx[4]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*disabled*/
      4) {
        input.disabled = /*disabled*/
        ctx2[2];
      }
      if (dirty & /*value*/
      1) {
        input.checked = /*value*/
        ctx2[0];
      }
      if (dirty & /*label*/
      2)
        set_data(
          t1,
          /*label*/
          ctx2[1]
        );
      if (dirty & /*disabled*/
      4) {
        toggle_class(
          label_1,
          "disabled",
          /*disabled*/
          ctx2[2]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(label_1);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let disabled;
  let { value = false } = $$props;
  let { label = "Checkbox" } = $$props;
  let { interactive } = $$props;
  const dispatch = createEventDispatcher();
  async function handle_enter(event) {
    if (event.key === "Enter") {
      $$invalidate(0, value = !value);
      dispatch("select", {
        index: 0,
        value: event.currentTarget.checked,
        selected: event.currentTarget.checked
      });
    }
  }
  async function handle_input(event) {
    $$invalidate(0, value = event.currentTarget.checked);
    dispatch("select", {
      index: 0,
      value: event.currentTarget.checked,
      selected: event.currentTarget.checked
    });
  }
  function input_change_handler() {
    value = this.checked;
    $$invalidate(0, value);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("interactive" in $$props2)
      $$invalidate(5, interactive = $$props2.interactive);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    1) {
      dispatch("change", value);
    }
    if ($$self.$$.dirty & /*interactive*/
    32) {
      $$invalidate(2, disabled = !interactive);
    }
  };
  return [
    value,
    label,
    disabled,
    handle_enter,
    handle_input,
    interactive,
    input_change_handler
  ];
}
class Checkbox extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, { value: 0, label: 1, interactive: 5 });
  }
}
const BaseCheckbox = Checkbox;
function create_if_block(ctx) {
  let info_1;
  let current;
  info_1 = new Info({
    props: {
      root: (
        /*root*/
        ctx[6]
      ),
      info: (
        /*info*/
        ctx[5]
      )
    }
  });
  return {
    c() {
      create_component(info_1.$$.fragment);
    },
    l(nodes) {
      claim_component(info_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(info_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const info_1_changes = {};
      if (dirty & /*root*/
      64)
        info_1_changes.root = /*root*/
        ctx2[6];
      if (dirty & /*info*/
      32)
        info_1_changes.info = /*info*/
        ctx2[5];
      info_1.$set(info_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(info_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(info_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(info_1, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let t1;
  let basecheckbox;
  let updating_value;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[11].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[11].i18n
    ) },
    /*loading_status*/
    ctx[10]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[15]
  );
  let if_block = (
    /*info*/
    ctx[5] && create_if_block(ctx)
  );
  function basecheckbox_value_binding(value) {
    ctx[16](value);
  }
  let basecheckbox_props = {
    label: (
      /*label*/
      ctx[4]
    ),
    interactive: (
      /*interactive*/
      ctx[12]
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    basecheckbox_props.value = /*value*/
    ctx[0];
  }
  basecheckbox = new BaseCheckbox({ props: basecheckbox_props });
  binding_callbacks.push(() => bind(basecheckbox, "value", basecheckbox_value_binding));
  basecheckbox.$on(
    "change",
    /*handle_change*/
    ctx[13]
  );
  basecheckbox.$on(
    "select",
    /*select_handler*/
    ctx[17]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      if (if_block)
        if_block.c();
      t1 = space();
      create_component(basecheckbox.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      t1 = claim_space(nodes);
      claim_component(basecheckbox.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t1, anchor);
      mount_component(basecheckbox, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      3072 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        2048 && {
          autoscroll: (
            /*gradio*/
            ctx2[11].autoscroll
          )
        },
        dirty & /*gradio*/
        2048 && { i18n: (
          /*gradio*/
          ctx2[11].i18n
        ) },
        dirty & /*loading_status*/
        1024 && get_spread_object(
          /*loading_status*/
          ctx2[10]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      if (
        /*info*/
        ctx2[5]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*info*/
          32) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(t1.parentNode, t1);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const basecheckbox_changes = {};
      if (dirty & /*label*/
      16)
        basecheckbox_changes.label = /*label*/
        ctx2[4];
      if (dirty & /*interactive*/
      4096)
        basecheckbox_changes.interactive = /*interactive*/
        ctx2[12];
      if (!updating_value && dirty & /*value*/
      1) {
        updating_value = true;
        basecheckbox_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      basecheckbox.$set(basecheckbox_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(if_block);
      transition_in(basecheckbox.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(if_block);
      transition_out(basecheckbox.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
      }
      destroy_component(statustracker, detaching);
      if (if_block)
        if_block.d(detaching);
      destroy_component(basecheckbox, detaching);
    }
  };
}
function create_fragment(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[3]
      ),
      elem_id: (
        /*elem_id*/
        ctx[1]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[2]
      ),
      container: (
        /*container*/
        ctx[7]
      ),
      scale: (
        /*scale*/
        ctx[8]
      ),
      min_width: (
        /*min_width*/
        ctx[9]
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
      if (dirty & /*visible*/
      8)
        block_changes.visible = /*visible*/
        ctx2[3];
      if (dirty & /*elem_id*/
      2)
        block_changes.elem_id = /*elem_id*/
        ctx2[1];
      if (dirty & /*elem_classes*/
      4)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[2];
      if (dirty & /*container*/
      128)
        block_changes.container = /*container*/
        ctx2[7];
      if (dirty & /*scale*/
      256)
        block_changes.scale = /*scale*/
        ctx2[8];
      if (dirty & /*min_width*/
      512)
        block_changes.min_width = /*min_width*/
        ctx2[9];
      if (dirty & /*$$scope, label, interactive, value, gradio, root, info, loading_status*/
      269425) {
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
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = false } = $$props;
  let { value_is_output = false } = $$props;
  let { label = "Checkbox" } = $$props;
  let { info = void 0 } = $$props;
  let { root } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  function handle_change() {
    gradio.dispatch("change");
    if (!value_is_output) {
      gradio.dispatch("input");
    }
  }
  afterUpdate(() => {
    $$invalidate(14, value_is_output = false);
  });
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function basecheckbox_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  const select_handler = (e) => gradio.dispatch("select", e.detail);
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("value_is_output" in $$props2)
      $$invalidate(14, value_is_output = $$props2.value_is_output);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(5, info = $$props2.info);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("container" in $$props2)
      $$invalidate(7, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(8, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(9, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(10, loading_status = $$props2.loading_status);
    if ("gradio" in $$props2)
      $$invalidate(11, gradio = $$props2.gradio);
    if ("interactive" in $$props2)
      $$invalidate(12, interactive = $$props2.interactive);
  };
  return [
    value,
    elem_id,
    elem_classes,
    visible,
    label,
    info,
    root,
    container,
    scale,
    min_width,
    loading_status,
    gradio,
    interactive,
    handle_change,
    value_is_output,
    clear_status_handler,
    basecheckbox_value_binding,
    select_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 1,
      elem_classes: 2,
      visible: 3,
      value: 0,
      value_is_output: 14,
      label: 4,
      info: 5,
      root: 6,
      container: 7,
      scale: 8,
      min_width: 9,
      loading_status: 10,
      gradio: 11,
      interactive: 12
    });
  }
}
export {
  BaseCheckbox,
  Index as default
};
