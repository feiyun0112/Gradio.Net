import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, create_slot, element, space, claim_element, claim_space, toggle_class, listen, transition_in, group_outros, transition_out, check_outros, update_slot_base, get_all_dirty_from_scope, get_slot_changes, run_all, component_subscribe, createEventDispatcher, onMount, setContext, set_store_value, ensure_array_like, create_component, claim_component, mount_component, stop_propagation, update_keyed_each, destroy_block, destroy_component, destroy_each, empty, text, claim_text, set_data, binding_callbacks, tick } from "../../../svelte/svelte.js";
import { writable } from "../../../svelte/svelte-submodules.js";
function create_fragment$1(ctx) {
  let svg;
  let circle0;
  let circle1;
  let circle2;
  return {
    c() {
      svg = svg_element("svg");
      circle0 = svg_element("circle");
      circle1 = svg_element("circle");
      circle2 = svg_element("circle");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      circle0 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle0).forEach(detach);
      circle1 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle1).forEach(detach);
      circle2 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle2).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(circle0, "cx", "2.5");
      attr(circle0, "cy", "8");
      attr(circle0, "r", "1.5");
      attr(circle0, "fill", "currentColor");
      attr(circle1, "cx", "8");
      attr(circle1, "cy", "8");
      attr(circle1, "r", "1.5");
      attr(circle1, "fill", "currentColor");
      attr(circle2, "cx", "13.5");
      attr(circle2, "cy", "8");
      attr(circle2, "r", "1.5");
      attr(circle2, "fill", "currentColor");
      attr(svg, "width", "16");
      attr(svg, "height", "16");
      attr(svg, "viewBox", "0 0 16 16");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, circle0);
      append_hydration(svg, circle1);
      append_hydration(svg, circle2);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class OverflowIcon extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$1, safe_not_equal, {});
  }
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[32] = list[i];
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[32] = list[i];
  child_ctx[36] = i;
  return child_ctx;
}
function get_each_context_2(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[32] = list[i];
  child_ctx[37] = list;
  child_ctx[36] = i;
  return child_ctx;
}
function create_if_block(ctx) {
  let div3;
  let div0;
  let each_blocks_2 = [];
  let each0_lookup = /* @__PURE__ */ new Map();
  let t0;
  let div1;
  let each_blocks_1 = [];
  let each1_lookup = /* @__PURE__ */ new Map();
  let t1;
  let span;
  let button;
  let overflowicon;
  let t2;
  let div2;
  let current;
  let mounted;
  let dispose;
  let each_value_2 = ensure_array_like(
    /*tabs*/
    ctx[3]
  );
  const get_key = (ctx2) => (
    /*t*/
    ctx2[32].id
  );
  for (let i = 0; i < each_value_2.length; i += 1) {
    let child_ctx = get_each_context_2(ctx, each_value_2, i);
    let key = get_key(child_ctx);
    each0_lookup.set(key, each_blocks_2[i] = create_each_block_2(key, child_ctx));
  }
  let each_value_1 = ensure_array_like(
    /*visible_tabs*/
    ctx[7]
  );
  const get_key_1 = (ctx2) => (
    /*t*/
    ctx2[32].id
  );
  for (let i = 0; i < each_value_1.length; i += 1) {
    let child_ctx = get_each_context_1(ctx, each_value_1, i);
    let key = get_key_1(child_ctx);
    each1_lookup.set(key, each_blocks_1[i] = create_each_block_1(key, child_ctx));
  }
  overflowicon = new OverflowIcon({});
  let each_value = ensure_array_like(
    /*overflow_tabs*/
    ctx[8]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  return {
    c() {
      div3 = element("div");
      div0 = element("div");
      for (let i = 0; i < each_blocks_2.length; i += 1) {
        each_blocks_2[i].c();
      }
      t0 = space();
      div1 = element("div");
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        each_blocks_1[i].c();
      }
      t1 = space();
      span = element("span");
      button = element("button");
      create_component(overflowicon.$$.fragment);
      t2 = space();
      div2 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div3 = claim_element(nodes, "DIV", { class: true });
      var div3_nodes = children(div3);
      div0 = claim_element(div3_nodes, "DIV", { class: true, "aria-hidden": true });
      var div0_nodes = children(div0);
      for (let i = 0; i < each_blocks_2.length; i += 1) {
        each_blocks_2[i].l(div0_nodes);
      }
      div0_nodes.forEach(detach);
      t0 = claim_space(div3_nodes);
      div1 = claim_element(div3_nodes, "DIV", { class: true, role: true });
      var div1_nodes = children(div1);
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        each_blocks_1[i].l(div1_nodes);
      }
      div1_nodes.forEach(detach);
      t1 = claim_space(div3_nodes);
      span = claim_element(div3_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      button = claim_element(span_nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      claim_component(overflowicon.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      t2 = claim_space(span_nodes);
      div2 = claim_element(span_nodes, "DIV", { class: true });
      var div2_nodes = children(div2);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div2_nodes);
      }
      div2_nodes.forEach(detach);
      span_nodes.forEach(detach);
      div3_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "tab-container visually-hidden svelte-1tcem6n");
      attr(div0, "aria-hidden", "true");
      attr(div1, "class", "tab-container svelte-1tcem6n");
      attr(div1, "role", "tablist");
      attr(button, "class", "svelte-1tcem6n");
      toggle_class(
        button,
        "overflow-item-selected",
        /*overflow_has_selected_tab*/
        ctx[12]
      );
      attr(div2, "class", "overflow-dropdown svelte-1tcem6n");
      toggle_class(div2, "hide", !/*overflow_menu_open*/
      ctx[9]);
      attr(span, "class", "overflow-menu svelte-1tcem6n");
      toggle_class(span, "hide", !/*is_overflowing*/
      ctx[11]);
      attr(div3, "class", "tab-wrapper svelte-1tcem6n");
    },
    m(target, anchor) {
      insert_hydration(target, div3, anchor);
      append_hydration(div3, div0);
      for (let i = 0; i < each_blocks_2.length; i += 1) {
        if (each_blocks_2[i]) {
          each_blocks_2[i].m(div0, null);
        }
      }
      append_hydration(div3, t0);
      append_hydration(div3, div1);
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        if (each_blocks_1[i]) {
          each_blocks_1[i].m(div1, null);
        }
      }
      ctx[26](div1);
      append_hydration(div3, t1);
      append_hydration(div3, span);
      append_hydration(span, button);
      mount_component(overflowicon, button, null);
      append_hydration(span, t2);
      append_hydration(span, div2);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div2, null);
        }
      }
      ctx[29](span);
      current = true;
      if (!mounted) {
        dispose = listen(button, "click", stop_propagation(
          /*click_handler_1*/
          ctx[27]
        ));
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*tab_els, tabs*/
      40) {
        each_value_2 = ensure_array_like(
          /*tabs*/
          ctx2[3]
        );
        each_blocks_2 = update_keyed_each(each_blocks_2, dirty, get_key, 1, ctx2, each_value_2, each0_lookup, div0, destroy_block, create_each_block_2, null, get_each_context_2);
      }
      if (dirty[0] & /*visible_tabs, $selected_tab, change_tab, dispatch*/
      196800) {
        each_value_1 = ensure_array_like(
          /*visible_tabs*/
          ctx2[7]
        );
        each_blocks_1 = update_keyed_each(each_blocks_1, dirty, get_key_1, 1, ctx2, each_value_1, each1_lookup, div1, destroy_block, create_each_block_1, null, get_each_context_1);
      }
      if (!current || dirty[0] & /*overflow_has_selected_tab*/
      4096) {
        toggle_class(
          button,
          "overflow-item-selected",
          /*overflow_has_selected_tab*/
          ctx2[12]
        );
      }
      if (dirty[0] & /*overflow_tabs, $selected_tab, change_tab*/
      131392) {
        each_value = ensure_array_like(
          /*overflow_tabs*/
          ctx2[8]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div2, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
      }
      if (!current || dirty[0] & /*overflow_menu_open*/
      512) {
        toggle_class(div2, "hide", !/*overflow_menu_open*/
        ctx2[9]);
      }
      if (!current || dirty[0] & /*is_overflowing*/
      2048) {
        toggle_class(span, "hide", !/*is_overflowing*/
        ctx2[11]);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(overflowicon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(overflowicon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div3);
      }
      for (let i = 0; i < each_blocks_2.length; i += 1) {
        each_blocks_2[i].d();
      }
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        each_blocks_1[i].d();
      }
      ctx[26](null);
      destroy_component(overflowicon);
      destroy_each(each_blocks, detaching);
      ctx[29](null);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_2(ctx) {
  let button;
  let t0_value = (
    /*t*/
    ctx[32].label + ""
  );
  let t0;
  let t1;
  let t = (
    /*t*/
    ctx[32]
  );
  const assign_button = () => (
    /*button_binding*/
    ctx[24](button, t)
  );
  const unassign_button = () => (
    /*button_binding*/
    ctx[24](null, t)
  );
  return {
    c() {
      button = element("button");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      t0 = claim_text(button_nodes, t0_value);
      t1 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "svelte-1tcem6n");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t0);
      append_hydration(button, t1);
      assign_button();
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*tabs*/
      8 && t0_value !== (t0_value = /*t*/
      ctx[32].label + ""))
        set_data(t0, t0_value);
      if (t !== /*t*/
      ctx[32]) {
        unassign_button();
        t = /*t*/
        ctx[32];
        assign_button();
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      unassign_button();
    }
  };
}
function create_each_block_2(key_1, ctx) {
  let first;
  let if_block_anchor;
  let if_block = (
    /*t*/
    ctx[32].visible && create_if_block_2(ctx)
  );
  return {
    key: key_1,
    first: null,
    c() {
      first = empty();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      first = empty();
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      this.first = first;
    },
    m(target, anchor) {
      insert_hydration(target, first, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (
        /*t*/
        ctx[32].visible
      ) {
        if (if_block) {
          if_block.p(ctx, dirty);
        } else {
          if_block = create_if_block_2(ctx);
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(first);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let button;
  let t0_value = (
    /*t*/
    ctx[32].label + ""
  );
  let t0;
  let t1;
  let button_aria_selected_value;
  let button_aria_controls_value;
  let button_disabled_value;
  let button_aria_disabled_value;
  let button_id_value;
  let button_data_tab_id_value;
  let mounted;
  let dispose;
  function click_handler() {
    return (
      /*click_handler*/
      ctx[25](
        /*t*/
        ctx[32],
        /*i*/
        ctx[36]
      )
    );
  }
  return {
    c() {
      button = element("button");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", {
        role: true,
        "aria-selected": true,
        "aria-controls": true,
        "aria-disabled": true,
        id: true,
        "data-tab-id": true,
        class: true
      });
      var button_nodes = children(button);
      t0 = claim_text(button_nodes, t0_value);
      t1 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "role", "tab");
      attr(button, "aria-selected", button_aria_selected_value = /*t*/
      ctx[32].id === /*$selected_tab*/
      ctx[6]);
      attr(button, "aria-controls", button_aria_controls_value = /*t*/
      ctx[32].elem_id);
      button.disabled = button_disabled_value = !/*t*/
      ctx[32].interactive;
      attr(button, "aria-disabled", button_aria_disabled_value = !/*t*/
      ctx[32].interactive);
      attr(button, "id", button_id_value = /*t*/
      ctx[32].elem_id ? (
        /*t*/
        ctx[32].elem_id + "-button"
      ) : null);
      attr(button, "data-tab-id", button_data_tab_id_value = /*t*/
      ctx[32].id);
      attr(button, "class", "svelte-1tcem6n");
      toggle_class(
        button,
        "selected",
        /*t*/
        ctx[32].id === /*$selected_tab*/
        ctx[6]
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t0);
      append_hydration(button, t1);
      if (!mounted) {
        dispose = listen(button, "click", click_handler);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*visible_tabs*/
      128 && t0_value !== (t0_value = /*t*/
      ctx[32].label + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*visible_tabs, $selected_tab*/
      192 && button_aria_selected_value !== (button_aria_selected_value = /*t*/
      ctx[32].id === /*$selected_tab*/
      ctx[6])) {
        attr(button, "aria-selected", button_aria_selected_value);
      }
      if (dirty[0] & /*visible_tabs*/
      128 && button_aria_controls_value !== (button_aria_controls_value = /*t*/
      ctx[32].elem_id)) {
        attr(button, "aria-controls", button_aria_controls_value);
      }
      if (dirty[0] & /*visible_tabs*/
      128 && button_disabled_value !== (button_disabled_value = !/*t*/
      ctx[32].interactive)) {
        button.disabled = button_disabled_value;
      }
      if (dirty[0] & /*visible_tabs*/
      128 && button_aria_disabled_value !== (button_aria_disabled_value = !/*t*/
      ctx[32].interactive)) {
        attr(button, "aria-disabled", button_aria_disabled_value);
      }
      if (dirty[0] & /*visible_tabs*/
      128 && button_id_value !== (button_id_value = /*t*/
      ctx[32].elem_id ? (
        /*t*/
        ctx[32].elem_id + "-button"
      ) : null)) {
        attr(button, "id", button_id_value);
      }
      if (dirty[0] & /*visible_tabs*/
      128 && button_data_tab_id_value !== (button_data_tab_id_value = /*t*/
      ctx[32].id)) {
        attr(button, "data-tab-id", button_data_tab_id_value);
      }
      if (dirty[0] & /*visible_tabs, $selected_tab*/
      192) {
        toggle_class(
          button,
          "selected",
          /*t*/
          ctx[32].id === /*$selected_tab*/
          ctx[6]
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_each_block_1(key_1, ctx) {
  let first;
  let if_block_anchor;
  let if_block = (
    /*t*/
    ctx[32].visible && create_if_block_1(ctx)
  );
  return {
    key: key_1,
    first: null,
    c() {
      first = empty();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      first = empty();
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      this.first = first;
    },
    m(target, anchor) {
      insert_hydration(target, first, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (
        /*t*/
        ctx[32].visible
      ) {
        if (if_block) {
          if_block.p(ctx, dirty);
        } else {
          if_block = create_if_block_1(ctx);
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(first);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_each_block(ctx) {
  let button;
  let t0_value = (
    /*t*/
    ctx[32].label + ""
  );
  let t0;
  let t1;
  let mounted;
  let dispose;
  function click_handler_2() {
    return (
      /*click_handler_2*/
      ctx[28](
        /*t*/
        ctx[32]
      )
    );
  }
  return {
    c() {
      button = element("button");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      t0 = claim_text(button_nodes, t0_value);
      t1 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "svelte-1tcem6n");
      toggle_class(
        button,
        "selected",
        /*t*/
        ctx[32].id === /*$selected_tab*/
        ctx[6]
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t0);
      append_hydration(button, t1);
      if (!mounted) {
        dispose = listen(button, "click", click_handler_2);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*overflow_tabs*/
      256 && t0_value !== (t0_value = /*t*/
      ctx[32].label + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*overflow_tabs, $selected_tab*/
      320) {
        toggle_class(
          button,
          "selected",
          /*t*/
          ctx[32].id === /*$selected_tab*/
          ctx[6]
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_fragment(ctx) {
  let div;
  let t_1;
  let div_class_value;
  let current;
  let mounted;
  let dispose;
  let if_block = (
    /*has_tabs*/
    ctx[13] && create_if_block(ctx)
  );
  const default_slot_template = (
    /*#slots*/
    ctx[23].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[22],
    null
  );
  return {
    c() {
      div = element("div");
      if (if_block)
        if_block.c();
      t_1 = space();
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, id: true });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      t_1 = claim_space(div_nodes);
      if (default_slot)
        default_slot.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", div_class_value = "tabs " + /*elem_classes*/
      ctx[2].join(" ") + " svelte-1tcem6n");
      attr(
        div,
        "id",
        /*elem_id*/
        ctx[1]
      );
      toggle_class(div, "hide", !/*visible*/
      ctx[0]);
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (if_block)
        if_block.m(div, null);
      append_hydration(div, t_1);
      if (default_slot) {
        default_slot.m(div, null);
      }
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            window,
            "resize",
            /*handle_menu_overflow*/
            ctx[19]
          ),
          listen(
            window,
            "click",
            /*handle_outside_click*/
            ctx[18]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (
        /*has_tabs*/
        ctx2[13]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*has_tabs*/
          8192) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(div, t_1);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      if (default_slot) {
        if (default_slot.p && (!current || dirty[0] & /*$$scope*/
        4194304)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[22],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[22]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[22],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty[0] & /*elem_classes*/
      4 && div_class_value !== (div_class_value = "tabs " + /*elem_classes*/
      ctx2[2].join(" ") + " svelte-1tcem6n")) {
        attr(div, "class", div_class_value);
      }
      if (!current || dirty[0] & /*elem_id*/
      2) {
        attr(
          div,
          "id",
          /*elem_id*/
          ctx2[1]
        );
      }
      if (!current || dirty[0] & /*elem_classes, visible*/
      5) {
        toggle_class(div, "hide", !/*visible*/
        ctx2[0]);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(default_slot, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(default_slot, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (if_block)
        if_block.d();
      if (default_slot)
        default_slot.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
const TABS = {};
function get_tab_sizes(tabs2, tab_els2) {
  const tab_sizes = {};
  tabs2.forEach((tab) => {
    var _a;
    tab_sizes[tab.id] = (_a = tab_els2[tab.id]) == null ? void 0 : _a.getBoundingClientRect();
  });
  return tab_sizes;
}
function instance($$self, $$props, $$invalidate) {
  var _a;
  let has_tabs;
  let $selected_tab;
  let $selected_tab_index;
  let { $$slots: slots = {}, $$scope } = $$props;
  let { visible = true } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { selected } = $$props;
  let { initial_tabs } = $$props;
  let tabs = [...initial_tabs];
  let visible_tabs = [...initial_tabs];
  let overflow_tabs = [];
  let overflow_menu_open = false;
  let overflow_menu;
  let tab_nav_el;
  const selected_tab = writable(selected || ((_a = tabs[0]) == null ? void 0 : _a.id) || false);
  component_subscribe($$self, selected_tab, (value) => $$invalidate(6, $selected_tab = value));
  const selected_tab_index = writable(tabs.findIndex((t) => t.id === selected) || 0);
  component_subscribe($$self, selected_tab_index, (value) => $$invalidate(30, $selected_tab_index = value));
  const dispatch = createEventDispatcher();
  let is_overflowing = false;
  let overflow_has_selected_tab = false;
  let tab_els = {};
  onMount(() => {
    const observer = new IntersectionObserver((entries) => {
      handle_menu_overflow();
    });
    observer.observe(tab_nav_el);
  });
  setContext(TABS, {
    register_tab: (tab) => {
      let index = tabs.findIndex((t) => t.id === tab.id);
      if (index !== -1) {
        $$invalidate(3, tabs[index] = { ...tabs[index], ...tab }, tabs);
      } else {
        $$invalidate(3, tabs = [...tabs, tab]);
        index = tabs.length - 1;
      }
      if ($selected_tab === false && tab.visible && tab.interactive) {
        set_store_value(selected_tab, $selected_tab = tab.id, $selected_tab);
      }
      return index;
    },
    unregister_tab: (tab) => {
      var _a2;
      const index = tabs.findIndex((t) => t.id === tab.id);
      if (index !== -1) {
        $$invalidate(3, tabs = tabs.filter((t) => t.id !== tab.id));
        if ($selected_tab === tab.id) {
          set_store_value(selected_tab, $selected_tab = ((_a2 = tabs[0]) == null ? void 0 : _a2.id) || false, $selected_tab);
        }
      }
    },
    selected_tab,
    selected_tab_index
  });
  function change_tab(id) {
    const tab_to_activate = tabs.find((t) => t.id === id);
    if (tab_to_activate && tab_to_activate.interactive && tab_to_activate.visible && $selected_tab !== tab_to_activate.id) {
      $$invalidate(20, selected = id);
      set_store_value(selected_tab, $selected_tab = id, $selected_tab);
      set_store_value(selected_tab_index, $selected_tab_index = tabs.findIndex((t) => t.id === id), $selected_tab_index);
      dispatch("change");
      $$invalidate(9, overflow_menu_open = false);
    }
  }
  function handle_outside_click(event) {
    if (overflow_menu_open && overflow_menu && !overflow_menu.contains(event.target)) {
      $$invalidate(9, overflow_menu_open = false);
    }
  }
  async function handle_menu_overflow() {
    if (!tab_nav_el)
      return;
    await tick();
    const tab_nav_size = tab_nav_el.getBoundingClientRect();
    let max_width = tab_nav_size.width;
    const tab_sizes = get_tab_sizes(tabs, tab_els);
    let last_visible_index = 0;
    const offset = tab_nav_size.left;
    for (let i = tabs.length - 1; i >= 0; i--) {
      const tab = tabs[i];
      const tab_rect = tab_sizes[tab.id];
      if (!tab_rect)
        continue;
      if (tab_rect.right - offset < max_width) {
        last_visible_index = i;
        break;
      }
    }
    $$invalidate(8, overflow_tabs = tabs.slice(last_visible_index + 1));
    $$invalidate(7, visible_tabs = tabs.slice(0, last_visible_index + 1));
    $$invalidate(12, overflow_has_selected_tab = handle_overflow_has_selected_tab($selected_tab));
    $$invalidate(11, is_overflowing = overflow_tabs.length > 0);
  }
  function handle_overflow_has_selected_tab(selected_tab2) {
    if (selected_tab2 === false)
      return false;
    return overflow_tabs.some((t) => t.id === selected_tab2);
  }
  function button_binding($$value, t) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      tab_els[t.id] = $$value;
      $$invalidate(5, tab_els);
    });
  }
  const click_handler = (t, i) => {
    if (t.id !== $selected_tab) {
      change_tab(t.id);
      dispatch("select", { value: t.label, index: i });
    }
  };
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      tab_nav_el = $$value;
      $$invalidate(4, tab_nav_el);
    });
  }
  const click_handler_1 = () => $$invalidate(9, overflow_menu_open = !overflow_menu_open);
  const click_handler_2 = (t) => change_tab(t.id);
  function span_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      overflow_menu = $$value;
      $$invalidate(10, overflow_menu);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("visible" in $$props2)
      $$invalidate(0, visible = $$props2.visible);
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("selected" in $$props2)
      $$invalidate(20, selected = $$props2.selected);
    if ("initial_tabs" in $$props2)
      $$invalidate(21, initial_tabs = $$props2.initial_tabs);
    if ("$$scope" in $$props2)
      $$invalidate(22, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*tabs*/
    8) {
      $$invalidate(13, has_tabs = tabs.length > 0);
    }
    if ($$self.$$.dirty[0] & /*tabs, selected*/
    1048584) {
      selected !== null && change_tab(selected);
    }
    if ($$self.$$.dirty[0] & /*tabs, tab_nav_el, tab_els*/
    56) {
      handle_menu_overflow();
    }
    if ($$self.$$.dirty[0] & /*$selected_tab*/
    64) {
      $$invalidate(12, overflow_has_selected_tab = handle_overflow_has_selected_tab($selected_tab));
    }
  };
  return [
    visible,
    elem_id,
    elem_classes,
    tabs,
    tab_nav_el,
    tab_els,
    $selected_tab,
    visible_tabs,
    overflow_tabs,
    overflow_menu_open,
    overflow_menu,
    is_overflowing,
    overflow_has_selected_tab,
    has_tabs,
    selected_tab,
    selected_tab_index,
    dispatch,
    change_tab,
    handle_outside_click,
    handle_menu_overflow,
    selected,
    initial_tabs,
    $$scope,
    slots,
    button_binding,
    click_handler,
    div1_binding,
    click_handler_1,
    click_handler_2,
    span_binding
  ];
}
class Tabs extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        visible: 0,
        elem_id: 1,
        elem_classes: 2,
        selected: 20,
        initial_tabs: 21
      },
      null,
      [-1, -1]
    );
  }
}
const Tabs$1 = Tabs;
export {
  Tabs$1 as T,
  TABS as a
};
