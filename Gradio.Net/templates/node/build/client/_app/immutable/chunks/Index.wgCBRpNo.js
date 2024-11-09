import { SvelteComponent, init, safe_not_equal, init_binding_group, element, space, text, claim_element, children, claim_space, claim_text, detach, attr, set_input_value, toggle_class, insert_hydration, append_hydration, listen, set_data, noop, createEventDispatcher, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, assign, ensure_array_like, get_spread_update, get_spread_object, group_outros, update_keyed_each, outro_and_destroy_block, check_outros, binding_callbacks, bind, empty, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static, f as BlockTitle } from "./2.BqWhUxOo.js";
import { default as default2 } from "./Example.DH7CNEn-.js";
function create_fragment$1(ctx) {
  let label;
  let input;
  let value_has_changed = false;
  let t0;
  let span;
  let t1;
  let label_data_testid_value;
  let binding_group;
  let mounted;
  let dispose;
  binding_group = init_binding_group(
    /*$$binding_groups*/
    ctx[6][0]
  );
  return {
    c() {
      label = element("label");
      input = element("input");
      t0 = space();
      span = element("span");
      t1 = text(
        /*display_value*/
        ctx[1]
      );
      this.h();
    },
    l(nodes) {
      label = claim_element(nodes, "LABEL", { "data-testid": true, class: true });
      var label_nodes = children(label);
      input = claim_element(label_nodes, "INPUT", {
        type: true,
        name: true,
        "aria-checked": true,
        class: true
      });
      t0 = claim_space(label_nodes);
      span = claim_element(label_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t1 = claim_text(
        span_nodes,
        /*display_value*/
        ctx[1]
      );
      span_nodes.forEach(detach);
      label_nodes.forEach(detach);
      this.h();
    },
    h() {
      input.disabled = /*disabled*/
      ctx[3];
      attr(input, "type", "radio");
      attr(input, "name", "radio-" + ++id);
      input.__value = /*internal_value*/
      ctx[2];
      set_input_value(input, input.__value);
      attr(
        input,
        "aria-checked",
        /*is_selected*/
        ctx[4]
      );
      attr(input, "class", "svelte-167tx3z");
      attr(span, "class", "ml-2 svelte-167tx3z");
      attr(label, "data-testid", label_data_testid_value = /*display_value*/
      ctx[1] + "-radio-label");
      attr(label, "class", "svelte-167tx3z");
      toggle_class(
        label,
        "disabled",
        /*disabled*/
        ctx[3]
      );
      toggle_class(
        label,
        "selected",
        /*is_selected*/
        ctx[4]
      );
      binding_group.p(input);
    },
    m(target, anchor) {
      insert_hydration(target, label, anchor);
      append_hydration(label, input);
      input.checked = input.__value === /*selected*/
      ctx[0];
      append_hydration(label, t0);
      append_hydration(label, span);
      append_hydration(span, t1);
      if (!mounted) {
        dispose = listen(
          input,
          "change",
          /*input_change_handler*/
          ctx[5]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*disabled*/
      8) {
        input.disabled = /*disabled*/
        ctx2[3];
      }
      if (dirty & /*internal_value*/
      4) {
        input.__value = /*internal_value*/
        ctx2[2];
        set_input_value(input, input.__value);
        value_has_changed = true;
      }
      if (dirty & /*is_selected*/
      16) {
        attr(
          input,
          "aria-checked",
          /*is_selected*/
          ctx2[4]
        );
      }
      if (value_has_changed || dirty & /*selected*/
      1) {
        input.checked = input.__value === /*selected*/
        ctx2[0];
      }
      if (dirty & /*display_value*/
      2)
        set_data(
          t1,
          /*display_value*/
          ctx2[1]
        );
      if (dirty & /*display_value*/
      2 && label_data_testid_value !== (label_data_testid_value = /*display_value*/
      ctx2[1] + "-radio-label")) {
        attr(label, "data-testid", label_data_testid_value);
      }
      if (dirty & /*disabled*/
      8) {
        toggle_class(
          label,
          "disabled",
          /*disabled*/
          ctx2[3]
        );
      }
      if (dirty & /*is_selected*/
      16) {
        toggle_class(
          label,
          "selected",
          /*is_selected*/
          ctx2[4]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(label);
      }
      binding_group.r();
      mounted = false;
      dispose();
    }
  };
}
let id = 0;
function instance$1($$self, $$props, $$invalidate) {
  let { display_value } = $$props;
  let { internal_value } = $$props;
  let { disabled = false } = $$props;
  let { selected = null } = $$props;
  const dispatch = createEventDispatcher();
  let is_selected = false;
  async function handle_input(selected2, internal_value2) {
    $$invalidate(4, is_selected = selected2 === internal_value2);
    if (is_selected) {
      dispatch("input", internal_value2);
    }
  }
  const $$binding_groups = [[]];
  function input_change_handler() {
    selected = this.__value;
    $$invalidate(0, selected);
  }
  $$self.$$set = ($$props2) => {
    if ("display_value" in $$props2)
      $$invalidate(1, display_value = $$props2.display_value);
    if ("internal_value" in $$props2)
      $$invalidate(2, internal_value = $$props2.internal_value);
    if ("disabled" in $$props2)
      $$invalidate(3, disabled = $$props2.disabled);
    if ("selected" in $$props2)
      $$invalidate(0, selected = $$props2.selected);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*selected, internal_value*/
    5) {
      handle_input(selected, internal_value);
    }
  };
  return [
    selected,
    display_value,
    internal_value,
    disabled,
    is_selected,
    input_change_handler,
    $$binding_groups
  ];
}
class Radio extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      display_value: 1,
      internal_value: 2,
      disabled: 3,
      selected: 0
    });
  }
}
const BaseRadio = Radio;
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[21] = list[i][0];
  child_ctx[22] = list[i][1];
  child_ctx[24] = i;
  return child_ctx;
}
function create_default_slot_1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*label*/
        ctx[2]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*label*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*label*/
      4)
        set_data(
          t,
          /*label*/
          ctx2[2]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_each_block(key_1, ctx) {
  let first;
  let baseradio;
  let updating_selected;
  let current;
  function baseradio_selected_binding(value) {
    ctx[18](value);
  }
  function input_handler() {
    return (
      /*input_handler*/
      ctx[19](
        /*internal_value*/
        ctx[22],
        /*i*/
        ctx[24]
      )
    );
  }
  let baseradio_props = {
    display_value: (
      /*display_value*/
      ctx[21]
    ),
    internal_value: (
      /*internal_value*/
      ctx[22]
    ),
    disabled: (
      /*disabled*/
      ctx[14]
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    baseradio_props.selected = /*value*/
    ctx[0];
  }
  baseradio = new BaseRadio({ props: baseradio_props });
  binding_callbacks.push(() => bind(baseradio, "selected", baseradio_selected_binding));
  baseradio.$on("input", input_handler);
  return {
    key: key_1,
    first: null,
    c() {
      first = empty();
      create_component(baseradio.$$.fragment);
      this.h();
    },
    l(nodes) {
      first = empty();
      claim_component(baseradio.$$.fragment, nodes);
      this.h();
    },
    h() {
      this.first = first;
    },
    m(target, anchor) {
      insert_hydration(target, first, anchor);
      mount_component(baseradio, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const baseradio_changes = {};
      if (dirty & /*choices*/
      128)
        baseradio_changes.display_value = /*display_value*/
        ctx[21];
      if (dirty & /*choices*/
      128)
        baseradio_changes.internal_value = /*internal_value*/
        ctx[22];
      if (dirty & /*disabled*/
      16384)
        baseradio_changes.disabled = /*disabled*/
        ctx[14];
      if (!updating_selected && dirty & /*value*/
      1) {
        updating_selected = true;
        baseradio_changes.selected = /*value*/
        ctx[0];
        add_flush_callback(() => updating_selected = false);
      }
      baseradio.$set(baseradio_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(baseradio.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(baseradio.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(first);
      }
      destroy_component(baseradio, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let blocktitle;
  let t1;
  let div;
  let each_blocks = [];
  let each_1_lookup = /* @__PURE__ */ new Map();
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[1].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[1].i18n
    ) },
    /*loading_status*/
    ctx[12]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[17]
  );
  blocktitle = new BlockTitle({
    props: {
      root: (
        /*root*/
        ctx[13]
      ),
      show_label: (
        /*show_label*/
        ctx[8]
      ),
      info: (
        /*info*/
        ctx[3]
      ),
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  let each_value = ensure_array_like(
    /*choices*/
    ctx[7]
  );
  const get_key = (ctx2) => (
    /*i*/
    ctx2[24]
  );
  for (let i = 0; i < each_value.length; i += 1) {
    let child_ctx = get_each_context(ctx, each_value, i);
    let key = get_key(child_ctx);
    each_1_lookup.set(key, each_blocks[i] = create_each_block(key, child_ctx));
  }
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      create_component(blocktitle.$$.fragment);
      t1 = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      claim_component(blocktitle.$$.fragment, nodes);
      t1 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "wrap svelte-1kzox3m");
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      mount_component(blocktitle, target, anchor);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      4098 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        2 && { autoscroll: (
          /*gradio*/
          ctx2[1].autoscroll
        ) },
        dirty & /*gradio*/
        2 && { i18n: (
          /*gradio*/
          ctx2[1].i18n
        ) },
        dirty & /*loading_status*/
        4096 && get_spread_object(
          /*loading_status*/
          ctx2[12]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const blocktitle_changes = {};
      if (dirty & /*root*/
      8192)
        blocktitle_changes.root = /*root*/
        ctx2[13];
      if (dirty & /*show_label*/
      256)
        blocktitle_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty & /*info*/
      8)
        blocktitle_changes.info = /*info*/
        ctx2[3];
      if (dirty & /*$$scope, label*/
      33554436) {
        blocktitle_changes.$$scope = { dirty, ctx: ctx2 };
      }
      blocktitle.$set(blocktitle_changes);
      if (dirty & /*choices, disabled, value, gradio*/
      16515) {
        each_value = ensure_array_like(
          /*choices*/
          ctx2[7]
        );
        group_outros();
        each_blocks = update_keyed_each(each_blocks, dirty, get_key, 1, ctx2, each_value, each_1_lookup, div, outro_and_destroy_block, create_each_block, null, get_each_context);
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(blocktitle.$$.fragment, local);
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(blocktitle.$$.fragment, local);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(div);
      }
      destroy_component(statustracker, detaching);
      destroy_component(blocktitle, detaching);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].d();
      }
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
        ctx[6]
      ),
      type: "fieldset",
      elem_id: (
        /*elem_id*/
        ctx[4]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[5]
      ),
      container: (
        /*container*/
        ctx[9]
      ),
      scale: (
        /*scale*/
        ctx[10]
      ),
      min_width: (
        /*min_width*/
        ctx[11]
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
      64)
        block_changes.visible = /*visible*/
        ctx2[6];
      if (dirty & /*elem_id*/
      16)
        block_changes.elem_id = /*elem_id*/
        ctx2[4];
      if (dirty & /*elem_classes*/
      32)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[5];
      if (dirty & /*container*/
      512)
        block_changes.container = /*container*/
        ctx2[9];
      if (dirty & /*scale*/
      1024)
        block_changes.scale = /*scale*/
        ctx2[10];
      if (dirty & /*min_width*/
      2048)
        block_changes.min_width = /*min_width*/
        ctx2[11];
      if (dirty & /*$$scope, choices, disabled, value, gradio, root, show_label, info, label, loading_status*/
      33583503) {
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
  let disabled;
  let { gradio } = $$props;
  let { label = gradio.i18n("radio.radio") } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { choices = [] } = $$props;
  let { show_label = true } = $$props;
  let { container = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { interactive = true } = $$props;
  let { root } = $$props;
  function handle_change() {
    gradio.dispatch("change");
  }
  let old_value = value;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function baseradio_selected_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  const input_handler = (internal_value, i) => {
    gradio.dispatch("select", { value: internal_value, index: i });
    gradio.dispatch("input");
  };
  $$self.$$set = ($$props2) => {
    if ("gradio" in $$props2)
      $$invalidate(1, gradio = $$props2.gradio);
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(3, info = $$props2.info);
    if ("elem_id" in $$props2)
      $$invalidate(4, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(5, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(6, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("choices" in $$props2)
      $$invalidate(7, choices = $$props2.choices);
    if ("show_label" in $$props2)
      $$invalidate(8, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(9, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(10, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(11, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(12, loading_status = $$props2.loading_status);
    if ("interactive" in $$props2)
      $$invalidate(15, interactive = $$props2.interactive);
    if ("root" in $$props2)
      $$invalidate(13, root = $$props2.root);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value, old_value*/
    65537) {
      {
        if (value !== old_value) {
          $$invalidate(16, old_value = value);
          handle_change();
        }
      }
    }
    if ($$self.$$.dirty & /*interactive*/
    32768) {
      $$invalidate(14, disabled = !interactive);
    }
  };
  return [
    value,
    gradio,
    label,
    info,
    elem_id,
    elem_classes,
    visible,
    choices,
    show_label,
    container,
    scale,
    min_width,
    loading_status,
    root,
    disabled,
    interactive,
    old_value,
    clear_status_handler,
    baseradio_selected_binding,
    input_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      gradio: 1,
      label: 2,
      info: 3,
      elem_id: 4,
      elem_classes: 5,
      visible: 6,
      value: 0,
      choices: 7,
      show_label: 8,
      container: 9,
      scale: 10,
      min_width: 11,
      loading_status: 12,
      interactive: 15,
      root: 13
    });
  }
}
export {
  default2 as BaseExample,
  BaseRadio,
  Index as default
};
