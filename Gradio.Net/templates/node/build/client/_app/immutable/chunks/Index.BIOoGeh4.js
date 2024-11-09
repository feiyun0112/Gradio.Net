import { SvelteComponent, init, safe_not_equal, flush, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, binding_callbacks, bind, space, claim_space, insert_hydration, group_outros, check_outros, add_flush_callback, detach, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, e as Textbox, S as Static } from "./2.BqWhUxOo.js";
import { default as default2 } from "./Example.N3QsiMhe.js";
function create_if_block(ctx) {
  let statustracker;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[2].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[2].i18n
    ) },
    /*loading_status*/
    ctx[19]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[27]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      524292 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        4 && { autoscroll: (
          /*gradio*/
          ctx2[2].autoscroll
        ) },
        dirty[0] & /*gradio*/
        4 && { i18n: (
          /*gradio*/
          ctx2[2].i18n
        ) },
        dirty[0] & /*loading_status*/
        524288 && get_spread_object(
          /*loading_status*/
          ctx2[19]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(statustracker, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let t;
  let textbox;
  let updating_value;
  let updating_value_is_output;
  let current;
  let if_block = (
    /*loading_status*/
    ctx[19] && create_if_block(ctx)
  );
  function textbox_value_binding(value) {
    ctx[28](value);
  }
  function textbox_value_is_output_binding(value) {
    ctx[29](value);
  }
  let textbox_props = {
    label: (
      /*label*/
      ctx[3]
    ),
    info: (
      /*info*/
      ctx[4]
    ),
    root: (
      /*root*/
      ctx[25]
    ),
    show_label: (
      /*show_label*/
      ctx[10]
    ),
    lines: (
      /*lines*/
      ctx[8]
    ),
    type: (
      /*type*/
      ctx[12]
    ),
    rtl: (
      /*rtl*/
      ctx[20]
    ),
    text_align: (
      /*text_align*/
      ctx[21]
    ),
    max_lines: !/*max_lines*/
    ctx[11] ? (
      /*lines*/
      ctx[8] + 1
    ) : (
      /*max_lines*/
      ctx[11]
    ),
    placeholder: (
      /*placeholder*/
      ctx[9]
    ),
    submit_btn: (
      /*submit_btn*/
      ctx[16]
    ),
    stop_btn: (
      /*stop_btn*/
      ctx[17]
    ),
    show_copy_button: (
      /*show_copy_button*/
      ctx[18]
    ),
    autofocus: (
      /*autofocus*/
      ctx[22]
    ),
    container: (
      /*container*/
      ctx[13]
    ),
    autoscroll: (
      /*autoscroll*/
      ctx[23]
    ),
    max_length: (
      /*max_length*/
      ctx[26]
    ),
    disabled: !/*interactive*/
    ctx[24]
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    textbox_props.value = /*value*/
    ctx[0];
  }
  if (
    /*value_is_output*/
    ctx[1] !== void 0
  ) {
    textbox_props.value_is_output = /*value_is_output*/
    ctx[1];
  }
  textbox = new Textbox({ props: textbox_props });
  binding_callbacks.push(() => bind(textbox, "value", textbox_value_binding));
  binding_callbacks.push(() => bind(textbox, "value_is_output", textbox_value_is_output_binding));
  textbox.$on(
    "change",
    /*change_handler*/
    ctx[30]
  );
  textbox.$on(
    "input",
    /*input_handler*/
    ctx[31]
  );
  textbox.$on(
    "submit",
    /*submit_handler*/
    ctx[32]
  );
  textbox.$on(
    "blur",
    /*blur_handler*/
    ctx[33]
  );
  textbox.$on(
    "select",
    /*select_handler*/
    ctx[34]
  );
  textbox.$on(
    "focus",
    /*focus_handler*/
    ctx[35]
  );
  textbox.$on(
    "stop",
    /*stop_handler*/
    ctx[36]
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      create_component(textbox.$$.fragment);
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      claim_component(textbox.$$.fragment, nodes);
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(textbox, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*loading_status*/
        ctx2[19]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*loading_status*/
          524288) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const textbox_changes = {};
      if (dirty[0] & /*label*/
      8)
        textbox_changes.label = /*label*/
        ctx2[3];
      if (dirty[0] & /*info*/
      16)
        textbox_changes.info = /*info*/
        ctx2[4];
      if (dirty[0] & /*root*/
      33554432)
        textbox_changes.root = /*root*/
        ctx2[25];
      if (dirty[0] & /*show_label*/
      1024)
        textbox_changes.show_label = /*show_label*/
        ctx2[10];
      if (dirty[0] & /*lines*/
      256)
        textbox_changes.lines = /*lines*/
        ctx2[8];
      if (dirty[0] & /*type*/
      4096)
        textbox_changes.type = /*type*/
        ctx2[12];
      if (dirty[0] & /*rtl*/
      1048576)
        textbox_changes.rtl = /*rtl*/
        ctx2[20];
      if (dirty[0] & /*text_align*/
      2097152)
        textbox_changes.text_align = /*text_align*/
        ctx2[21];
      if (dirty[0] & /*max_lines, lines*/
      2304)
        textbox_changes.max_lines = !/*max_lines*/
        ctx2[11] ? (
          /*lines*/
          ctx2[8] + 1
        ) : (
          /*max_lines*/
          ctx2[11]
        );
      if (dirty[0] & /*placeholder*/
      512)
        textbox_changes.placeholder = /*placeholder*/
        ctx2[9];
      if (dirty[0] & /*submit_btn*/
      65536)
        textbox_changes.submit_btn = /*submit_btn*/
        ctx2[16];
      if (dirty[0] & /*stop_btn*/
      131072)
        textbox_changes.stop_btn = /*stop_btn*/
        ctx2[17];
      if (dirty[0] & /*show_copy_button*/
      262144)
        textbox_changes.show_copy_button = /*show_copy_button*/
        ctx2[18];
      if (dirty[0] & /*autofocus*/
      4194304)
        textbox_changes.autofocus = /*autofocus*/
        ctx2[22];
      if (dirty[0] & /*container*/
      8192)
        textbox_changes.container = /*container*/
        ctx2[13];
      if (dirty[0] & /*autoscroll*/
      8388608)
        textbox_changes.autoscroll = /*autoscroll*/
        ctx2[23];
      if (dirty[0] & /*max_length*/
      67108864)
        textbox_changes.max_length = /*max_length*/
        ctx2[26];
      if (dirty[0] & /*interactive*/
      16777216)
        textbox_changes.disabled = !/*interactive*/
        ctx2[24];
      if (!updating_value && dirty[0] & /*value*/
      1) {
        updating_value = true;
        textbox_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      if (!updating_value_is_output && dirty[0] & /*value_is_output*/
      2) {
        updating_value_is_output = true;
        textbox_changes.value_is_output = /*value_is_output*/
        ctx2[1];
        add_flush_callback(() => updating_value_is_output = false);
      }
      textbox.$set(textbox_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(textbox.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(textbox.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_component(textbox, detaching);
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
        ctx[7]
      ),
      elem_id: (
        /*elem_id*/
        ctx[5]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[6]
      ),
      scale: (
        /*scale*/
        ctx[14]
      ),
      min_width: (
        /*min_width*/
        ctx[15]
      ),
      allow_overflow: false,
      padding: (
        /*container*/
        ctx[13]
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
    p(ctx2, dirty) {
      const block_changes = {};
      if (dirty[0] & /*visible*/
      128)
        block_changes.visible = /*visible*/
        ctx2[7];
      if (dirty[0] & /*elem_id*/
      32)
        block_changes.elem_id = /*elem_id*/
        ctx2[5];
      if (dirty[0] & /*elem_classes*/
      64)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[6];
      if (dirty[0] & /*scale*/
      16384)
        block_changes.scale = /*scale*/
        ctx2[14];
      if (dirty[0] & /*min_width*/
      32768)
        block_changes.min_width = /*min_width*/
        ctx2[15];
      if (dirty[0] & /*container*/
      8192)
        block_changes.padding = /*container*/
        ctx2[13];
      if (dirty[0] & /*label, info, root, show_label, lines, type, rtl, text_align, max_lines, placeholder, submit_btn, stop_btn, show_copy_button, autofocus, container, autoscroll, max_length, interactive, value, value_is_output, gradio, loading_status*/
      134168351 | dirty[1] & /*$$scope*/
      64) {
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
  let { gradio } = $$props;
  let { label = "Textbox" } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = "" } = $$props;
  let { lines } = $$props;
  let { placeholder = "" } = $$props;
  let { show_label } = $$props;
  let { max_lines } = $$props;
  let { type = "text" } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { submit_btn = null } = $$props;
  let { stop_btn = null } = $$props;
  let { show_copy_button = false } = $$props;
  let { loading_status = void 0 } = $$props;
  let { value_is_output = false } = $$props;
  let { rtl = false } = $$props;
  let { text_align = void 0 } = $$props;
  let { autofocus = false } = $$props;
  let { autoscroll = true } = $$props;
  let { interactive } = $$props;
  let { root } = $$props;
  let { max_length = void 0 } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function textbox_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  function textbox_value_is_output_binding(value2) {
    value_is_output = value2;
    $$invalidate(1, value_is_output);
  }
  const change_handler = () => gradio.dispatch("change", value);
  const input_handler = () => gradio.dispatch("input");
  const submit_handler = () => gradio.dispatch("submit");
  const blur_handler = () => gradio.dispatch("blur");
  const select_handler = (e) => gradio.dispatch("select", e.detail);
  const focus_handler = () => gradio.dispatch("focus");
  const stop_handler = () => gradio.dispatch("stop");
  $$self.$$set = ($$props2) => {
    if ("gradio" in $$props2)
      $$invalidate(2, gradio = $$props2.gradio);
    if ("label" in $$props2)
      $$invalidate(3, label = $$props2.label);
    if ("info" in $$props2)
      $$invalidate(4, info = $$props2.info);
    if ("elem_id" in $$props2)
      $$invalidate(5, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(6, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(7, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("lines" in $$props2)
      $$invalidate(8, lines = $$props2.lines);
    if ("placeholder" in $$props2)
      $$invalidate(9, placeholder = $$props2.placeholder);
    if ("show_label" in $$props2)
      $$invalidate(10, show_label = $$props2.show_label);
    if ("max_lines" in $$props2)
      $$invalidate(11, max_lines = $$props2.max_lines);
    if ("type" in $$props2)
      $$invalidate(12, type = $$props2.type);
    if ("container" in $$props2)
      $$invalidate(13, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(14, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(15, min_width = $$props2.min_width);
    if ("submit_btn" in $$props2)
      $$invalidate(16, submit_btn = $$props2.submit_btn);
    if ("stop_btn" in $$props2)
      $$invalidate(17, stop_btn = $$props2.stop_btn);
    if ("show_copy_button" in $$props2)
      $$invalidate(18, show_copy_button = $$props2.show_copy_button);
    if ("loading_status" in $$props2)
      $$invalidate(19, loading_status = $$props2.loading_status);
    if ("value_is_output" in $$props2)
      $$invalidate(1, value_is_output = $$props2.value_is_output);
    if ("rtl" in $$props2)
      $$invalidate(20, rtl = $$props2.rtl);
    if ("text_align" in $$props2)
      $$invalidate(21, text_align = $$props2.text_align);
    if ("autofocus" in $$props2)
      $$invalidate(22, autofocus = $$props2.autofocus);
    if ("autoscroll" in $$props2)
      $$invalidate(23, autoscroll = $$props2.autoscroll);
    if ("interactive" in $$props2)
      $$invalidate(24, interactive = $$props2.interactive);
    if ("root" in $$props2)
      $$invalidate(25, root = $$props2.root);
    if ("max_length" in $$props2)
      $$invalidate(26, max_length = $$props2.max_length);
  };
  return [
    value,
    value_is_output,
    gradio,
    label,
    info,
    elem_id,
    elem_classes,
    visible,
    lines,
    placeholder,
    show_label,
    max_lines,
    type,
    container,
    scale,
    min_width,
    submit_btn,
    stop_btn,
    show_copy_button,
    loading_status,
    rtl,
    text_align,
    autofocus,
    autoscroll,
    interactive,
    root,
    max_length,
    clear_status_handler,
    textbox_value_binding,
    textbox_value_is_output_binding,
    change_handler,
    input_handler,
    submit_handler,
    blur_handler,
    select_handler,
    focus_handler,
    stop_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        gradio: 2,
        label: 3,
        info: 4,
        elem_id: 5,
        elem_classes: 6,
        visible: 7,
        value: 0,
        lines: 8,
        placeholder: 9,
        show_label: 10,
        max_lines: 11,
        type: 12,
        container: 13,
        scale: 14,
        min_width: 15,
        submit_btn: 16,
        stop_btn: 17,
        show_copy_button: 18,
        loading_status: 19,
        value_is_output: 1,
        rtl: 20,
        text_align: 21,
        autofocus: 22,
        autoscroll: 23,
        interactive: 24,
        root: 25,
        max_length: 26
      },
      null,
      [-1, -1]
    );
  }
  get gradio() {
    return this.$$.ctx[2];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get label() {
    return this.$$.ctx[3];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get info() {
    return this.$$.ctx[4];
  }
  set info(info) {
    this.$$set({ info });
    flush();
  }
  get elem_id() {
    return this.$$.ctx[5];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[6];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[7];
  }
  set visible(visible) {
    this.$$set({ visible });
    flush();
  }
  get value() {
    return this.$$.ctx[0];
  }
  set value(value) {
    this.$$set({ value });
    flush();
  }
  get lines() {
    return this.$$.ctx[8];
  }
  set lines(lines) {
    this.$$set({ lines });
    flush();
  }
  get placeholder() {
    return this.$$.ctx[9];
  }
  set placeholder(placeholder) {
    this.$$set({ placeholder });
    flush();
  }
  get show_label() {
    return this.$$.ctx[10];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get max_lines() {
    return this.$$.ctx[11];
  }
  set max_lines(max_lines) {
    this.$$set({ max_lines });
    flush();
  }
  get type() {
    return this.$$.ctx[12];
  }
  set type(type) {
    this.$$set({ type });
    flush();
  }
  get container() {
    return this.$$.ctx[13];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[14];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[15];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get submit_btn() {
    return this.$$.ctx[16];
  }
  set submit_btn(submit_btn) {
    this.$$set({ submit_btn });
    flush();
  }
  get stop_btn() {
    return this.$$.ctx[17];
  }
  set stop_btn(stop_btn) {
    this.$$set({ stop_btn });
    flush();
  }
  get show_copy_button() {
    return this.$$.ctx[18];
  }
  set show_copy_button(show_copy_button) {
    this.$$set({ show_copy_button });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[19];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get value_is_output() {
    return this.$$.ctx[1];
  }
  set value_is_output(value_is_output) {
    this.$$set({ value_is_output });
    flush();
  }
  get rtl() {
    return this.$$.ctx[20];
  }
  set rtl(rtl) {
    this.$$set({ rtl });
    flush();
  }
  get text_align() {
    return this.$$.ctx[21];
  }
  set text_align(text_align) {
    this.$$set({ text_align });
    flush();
  }
  get autofocus() {
    return this.$$.ctx[22];
  }
  set autofocus(autofocus) {
    this.$$set({ autofocus });
    flush();
  }
  get autoscroll() {
    return this.$$.ctx[23];
  }
  set autoscroll(autoscroll) {
    this.$$set({ autoscroll });
    flush();
  }
  get interactive() {
    return this.$$.ctx[24];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get root() {
    return this.$$.ctx[25];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get max_length() {
    return this.$$.ctx[26];
  }
  set max_length(max_length) {
    this.$$set({ max_length });
    flush();
  }
}
export {
  default2 as BaseExample,
  Textbox as BaseTextbox,
  Index as default
};
