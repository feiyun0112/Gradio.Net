import { SvelteComponent, init, not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, assign, ensure_array_like, space, element, claim_space, claim_element, children, detach, attr, insert_hydration, get_spread_update, get_spread_object, destroy_each, text, claim_text, set_data, toggle_class, append_hydration, listen, run_all } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static, f as BlockTitle } from "./2.BqWhUxOo.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[22] = list[i][0];
  child_ctx[23] = list[i][1];
  child_ctx[25] = i;
  return child_ctx;
}
function create_default_slot_1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*label*/
        ctx[9]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*label*/
        ctx[9]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*label*/
      512)
        set_data(
          t,
          /*label*/
          ctx2[9]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_each_block(ctx) {
  let label_1;
  let input;
  let input_checked_value;
  let input_name_value;
  let input_title_value;
  let t0;
  let span;
  let t1_value = (
    /*display_value*/
    ctx[22] + ""
  );
  let t1;
  let t2;
  let mounted;
  let dispose;
  function change_handler() {
    return (
      /*change_handler*/
      ctx[19](
        /*internal_value*/
        ctx[23]
      )
    );
  }
  function input_handler(...args) {
    return (
      /*input_handler*/
      ctx[20](
        /*i*/
        ctx[25],
        /*internal_value*/
        ctx[23],
        ...args
      )
    );
  }
  function keydown_handler(...args) {
    return (
      /*keydown_handler*/
      ctx[21](
        /*internal_value*/
        ctx[23],
        /*i*/
        ctx[25],
        ...args
      )
    );
  }
  return {
    c() {
      label_1 = element("label");
      input = element("input");
      t0 = space();
      span = element("span");
      t1 = text(t1_value);
      t2 = space();
      this.h();
    },
    l(nodes) {
      label_1 = claim_element(nodes, "LABEL", { class: true });
      var label_1_nodes = children(label_1);
      input = claim_element(label_1_nodes, "INPUT", {
        type: true,
        name: true,
        title: true,
        class: true
      });
      t0 = claim_space(label_1_nodes);
      span = claim_element(label_1_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t1 = claim_text(span_nodes, t1_value);
      span_nodes.forEach(detach);
      t2 = claim_space(label_1_nodes);
      label_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      var _a, _b;
      input.disabled = /*disabled*/
      ctx[14];
      input.checked = input_checked_value = /*value*/
      ctx[0].includes(
        /*internal_value*/
        ctx[23]
      );
      attr(input, "type", "checkbox");
      attr(input, "name", input_name_value = /*internal_value*/
      (_a = ctx[23]) == null ? void 0 : _a.toString());
      attr(input, "title", input_title_value = /*internal_value*/
      (_b = ctx[23]) == null ? void 0 : _b.toString());
      attr(input, "class", "svelte-kujow2");
      attr(span, "class", "ml-2 svelte-kujow2");
      attr(label_1, "class", "svelte-kujow2");
      toggle_class(
        label_1,
        "disabled",
        /*disabled*/
        ctx[14]
      );
      toggle_class(
        label_1,
        "selected",
        /*value*/
        ctx[0].includes(
          /*internal_value*/
          ctx[23]
        )
      );
    },
    m(target, anchor) {
      insert_hydration(target, label_1, anchor);
      append_hydration(label_1, input);
      append_hydration(label_1, t0);
      append_hydration(label_1, span);
      append_hydration(span, t1);
      append_hydration(label_1, t2);
      if (!mounted) {
        dispose = [
          listen(input, "change", change_handler),
          listen(input, "input", input_handler),
          listen(input, "keydown", keydown_handler)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      var _a, _b;
      ctx = new_ctx;
      if (dirty & /*disabled*/
      16384) {
        input.disabled = /*disabled*/
        ctx[14];
      }
      if (dirty & /*value, choices*/
      33 && input_checked_value !== (input_checked_value = /*value*/
      ctx[0].includes(
        /*internal_value*/
        ctx[23]
      ))) {
        input.checked = input_checked_value;
      }
      if (dirty & /*choices*/
      32 && input_name_value !== (input_name_value = /*internal_value*/
      (_a = ctx[23]) == null ? void 0 : _a.toString())) {
        attr(input, "name", input_name_value);
      }
      if (dirty & /*choices*/
      32 && input_title_value !== (input_title_value = /*internal_value*/
      (_b = ctx[23]) == null ? void 0 : _b.toString())) {
        attr(input, "title", input_title_value);
      }
      if (dirty & /*choices*/
      32 && t1_value !== (t1_value = /*display_value*/
      ctx[22] + ""))
        set_data(t1, t1_value);
      if (dirty & /*disabled*/
      16384) {
        toggle_class(
          label_1,
          "disabled",
          /*disabled*/
          ctx[14]
        );
      }
      if (dirty & /*value, choices*/
      33) {
        toggle_class(
          label_1,
          "selected",
          /*value*/
          ctx[0].includes(
            /*internal_value*/
            ctx[23]
          )
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(label_1);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let blocktitle;
  let t1;
  let div;
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
    ctx[13]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[18]
  );
  blocktitle = new BlockTitle({
    props: {
      root: (
        /*root*/
        ctx[12]
      ),
      show_label: (
        /*show_label*/
        ctx[11]
      ),
      info: (
        /*info*/
        ctx[10]
      ),
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  let each_value = ensure_array_like(
    /*choices*/
    ctx[5]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
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
      div = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "wrap svelte-kujow2");
      attr(div, "data-testid", "checkbox-group");
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
      8194 ? get_spread_update(statustracker_spread_levels, [
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
        8192 && get_spread_object(
          /*loading_status*/
          ctx2[13]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const blocktitle_changes = {};
      if (dirty & /*root*/
      4096)
        blocktitle_changes.root = /*root*/
        ctx2[12];
      if (dirty & /*show_label*/
      2048)
        blocktitle_changes.show_label = /*show_label*/
        ctx2[11];
      if (dirty & /*info*/
      1024)
        blocktitle_changes.info = /*info*/
        ctx2[10];
      if (dirty & /*$$scope, label*/
      67109376) {
        blocktitle_changes.$$scope = { dirty, ctx: ctx2 };
      }
      blocktitle.$set(blocktitle_changes);
      if (dirty & /*disabled, value, choices, toggle_choice, gradio*/
      49187) {
        each_value = ensure_array_like(
          /*choices*/
          ctx2[5]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(blocktitle.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(blocktitle.$$.fragment, local);
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
      destroy_each(each_blocks, detaching);
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
        ctx[4]
      ),
      elem_id: (
        /*elem_id*/
        ctx[2]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[3]
      ),
      type: "fieldset",
      container: (
        /*container*/
        ctx[6]
      ),
      scale: (
        /*scale*/
        ctx[7]
      ),
      min_width: (
        /*min_width*/
        ctx[8]
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
      16)
        block_changes.visible = /*visible*/
        ctx2[4];
      if (dirty & /*elem_id*/
      4)
        block_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty & /*elem_classes*/
      8)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[3];
      if (dirty & /*container*/
      64)
        block_changes.container = /*container*/
        ctx2[6];
      if (dirty & /*scale*/
      128)
        block_changes.scale = /*scale*/
        ctx2[7];
      if (dirty & /*min_width*/
      256)
        block_changes.min_width = /*min_width*/
        ctx2[8];
      if (dirty & /*$$scope, choices, disabled, value, gradio, root, show_label, info, label, loading_status*/
      67141155) {
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
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = [] } = $$props;
  let { choices } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { label = gradio.i18n("checkbox.checkbox_group") } = $$props;
  let { info = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { root } = $$props;
  let { loading_status } = $$props;
  let { interactive = true } = $$props;
  let { old_value = value.slice() } = $$props;
  function toggle_choice(choice) {
    if (value.includes(choice)) {
      $$invalidate(0, value = value.filter((v) => v !== choice));
    } else {
      $$invalidate(0, value = [...value, choice]);
    }
    gradio.dispatch("input");
  }
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const change_handler = (internal_value) => toggle_choice(internal_value);
  const input_handler = (i, internal_value, evt) => gradio.dispatch("select", {
    index: i,
    value: internal_value,
    selected: evt.currentTarget.checked
  });
  const keydown_handler = (internal_value, i, event) => {
    if (event.key === "Enter") {
      toggle_choice(internal_value);
      gradio.dispatch("select", {
        index: i,
        value: internal_value,
        selected: !value.includes(internal_value)
      });
    }
  };
  $$self.$$set = ($$props2) => {
    if ("gradio" in $$props2)
      $$invalidate(1, gradio = $$props2.gradio);
    if ("elem_id" in $$props2)
      $$invalidate(2, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(3, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(4, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("choices" in $$props2)
      $$invalidate(5, choices = $$props2.choices);
    if ("container" in $$props2)
      $$invalidate(6, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(7, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(8, min_width = $$props2.min_width);
    if ("label" in $$props2)
      $$invalidate(9, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(10, info = $$props2.info);
    if ("show_label" in $$props2)
      $$invalidate(11, show_label = $$props2.show_label);
    if ("root" in $$props2)
      $$invalidate(12, root = $$props2.root);
    if ("loading_status" in $$props2)
      $$invalidate(13, loading_status = $$props2.loading_status);
    if ("interactive" in $$props2)
      $$invalidate(17, interactive = $$props2.interactive);
    if ("old_value" in $$props2)
      $$invalidate(16, old_value = $$props2.old_value);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*interactive*/
    131072) {
      $$invalidate(14, disabled = !interactive);
    }
    if ($$self.$$.dirty & /*old_value, value, gradio*/
    65539) {
      if (JSON.stringify(old_value) !== JSON.stringify(value)) {
        $$invalidate(16, old_value = value);
        gradio.dispatch("change");
      }
    }
  };
  return [
    value,
    gradio,
    elem_id,
    elem_classes,
    visible,
    choices,
    container,
    scale,
    min_width,
    label,
    info,
    show_label,
    root,
    loading_status,
    disabled,
    toggle_choice,
    old_value,
    interactive,
    clear_status_handler,
    change_handler,
    input_handler,
    keydown_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, not_equal, {
      gradio: 1,
      elem_id: 2,
      elem_classes: 3,
      visible: 4,
      value: 0,
      choices: 5,
      container: 6,
      scale: 7,
      min_width: 8,
      label: 9,
      info: 10,
      show_label: 11,
      root: 12,
      loading_status: 13,
      interactive: 17,
      old_value: 16
    });
  }
}
export {
  Index as default
};
