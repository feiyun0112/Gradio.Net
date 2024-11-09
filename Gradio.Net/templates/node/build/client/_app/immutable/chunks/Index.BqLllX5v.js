import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, element, svg_element, space, text, empty, claim_element, children, claim_svg_element, detach, claim_space, claim_text, attr, insert_hydration, append_hydration, set_data, group_outros, check_outros, ensure_array_like, destroy_each, listen, run_all, toggle_class, get_svelte_dataset, noop, construct_svelte_component, set_style, null_to_empty, get_spread_update, get_spread_object, assign } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block } from "./2.BqWhUxOo.js";
import Example from "./Example.N3QsiMhe.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[37] = list[i];
  return child_ctx;
}
function get_each_context_2(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[40] = list[i];
  child_ctx[42] = i;
  return child_ctx;
}
function get_each_context_3(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[0] = list[i].value;
  child_ctx[44] = list[i].component;
  child_ctx[47] = i;
  const constants_0 = (
    /*components*/
    child_ctx[1][
      /*j*/
      child_ctx[47]
    ]
  );
  child_ctx[45] = constants_0;
  return child_ctx;
}
function get_each_context_4(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[48] = list[i];
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[40] = list[i];
  child_ctx[42] = i;
  return child_ctx;
}
function create_else_block_1(ctx) {
  let div;
  let table;
  let thead;
  let tr;
  let t;
  let tbody;
  let current;
  let each_value_4 = ensure_array_like(
    /*headers*/
    ctx[5]
  );
  let each_blocks_1 = [];
  for (let i = 0; i < each_value_4.length; i += 1) {
    each_blocks_1[i] = create_each_block_4(get_each_context_4(ctx, each_value_4, i));
  }
  let each_value_2 = ensure_array_like(
    /*component_meta*/
    ctx[20]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_2.length; i += 1) {
    each_blocks[i] = create_each_block_2(get_each_context_2(ctx, each_value_2, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      div = element("div");
      table = element("table");
      thead = element("thead");
      tr = element("tr");
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        each_blocks_1[i].c();
      }
      t = space();
      tbody = element("tbody");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      table = claim_element(div_nodes, "TABLE", { tabindex: true, role: true, class: true });
      var table_nodes = children(table);
      thead = claim_element(table_nodes, "THEAD", {});
      var thead_nodes = children(thead);
      tr = claim_element(thead_nodes, "TR", { class: true });
      var tr_nodes = children(tr);
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        each_blocks_1[i].l(tr_nodes);
      }
      tr_nodes.forEach(detach);
      thead_nodes.forEach(detach);
      t = claim_space(table_nodes);
      tbody = claim_element(table_nodes, "TBODY", {});
      var tbody_nodes = children(tbody);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(tbody_nodes);
      }
      tbody_nodes.forEach(detach);
      table_nodes.forEach(detach);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(tr, "class", "tr-head svelte-p5q82i");
      attr(table, "tabindex", "0");
      attr(table, "role", "grid");
      attr(table, "class", "svelte-p5q82i");
      attr(div, "class", "table-wrap svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, table);
      append_hydration(table, thead);
      append_hydration(thead, tr);
      for (let i = 0; i < each_blocks_1.length; i += 1) {
        if (each_blocks_1[i]) {
          each_blocks_1[i].m(tr, null);
        }
      }
      append_hydration(table, t);
      append_hydration(table, tbody);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(tbody, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*headers*/
      32) {
        each_value_4 = ensure_array_like(
          /*headers*/
          ctx2[5]
        );
        let i;
        for (i = 0; i < each_value_4.length; i += 1) {
          const child_ctx = get_each_context_4(ctx2, each_value_4, i);
          if (each_blocks_1[i]) {
            each_blocks_1[i].p(child_ctx, dirty);
          } else {
            each_blocks_1[i] = create_each_block_4(child_ctx);
            each_blocks_1[i].c();
            each_blocks_1[i].m(tr, null);
          }
        }
        for (; i < each_blocks_1.length; i += 1) {
          each_blocks_1[i].d(1);
        }
        each_blocks_1.length = each_value_4.length;
      }
      if (dirty[0] & /*value, page, samples_per_page, gradio, handle_mouseenter, handle_mouseleave, component_meta, components, component_props, samples_dir, current_hover, root, component_map*/
      30985231) {
        each_value_2 = ensure_array_like(
          /*component_meta*/
          ctx2[20]
        );
        let i;
        for (i = 0; i < each_value_2.length; i += 1) {
          const child_ctx = get_each_context_2(ctx2, each_value_2, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_2(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(tbody, null);
          }
        }
        group_outros();
        for (i = each_value_2.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_2.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_each(each_blocks_1, detaching);
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let div;
  let current;
  let each_value_1 = ensure_array_like(
    /*selected_samples*/
    ctx[17]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "gallery svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*value, page, samples_per_page, gradio, selected_samples, handle_mouseenter, handle_mouseleave, current_hover, sample_labels, component_meta, component_props, samples_dir, root, component_map, components*/
      31116367) {
        each_value_1 = ensure_array_like(
          /*selected_samples*/
          ctx2[17]
        );
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_1(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div, null);
          }
        }
        group_outros();
        for (i = each_value_1.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_1.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_each_block_4(ctx) {
  let th;
  let t0_value = (
    /*header*/
    ctx[48] + ""
  );
  let t0;
  let t1;
  return {
    c() {
      th = element("th");
      t0 = text(t0_value);
      t1 = space();
      this.h();
    },
    l(nodes) {
      th = claim_element(nodes, "TH", { class: true });
      var th_nodes = children(th);
      t0 = claim_text(th_nodes, t0_value);
      t1 = claim_space(th_nodes);
      th_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(th, "class", "svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, th, anchor);
      append_hydration(th, t0);
      append_hydration(th, t1);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*headers*/
      32 && t0_value !== (t0_value = /*header*/
      ctx2[48] + ""))
        set_data(t0, t0_value);
    },
    d(detaching) {
      if (detaching) {
        detach(th);
      }
    }
  };
}
function create_if_block_6(ctx) {
  let td;
  let switch_instance;
  let td_class_value;
  let current;
  const switch_instance_spread_levels = [
    /*component_props*/
    ctx[2][
      /*j*/
      ctx[47]
    ],
    { value: (
      /*value*/
      ctx[0]
    ) },
    { samples_dir: (
      /*samples_dir*/
      ctx[22]
    ) },
    { type: "table" },
    {
      selected: (
        /*current_hover*/
        ctx[19] === /*i*/
        ctx[42]
      )
    },
    { index: (
      /*i*/
      ctx[42]
    ) },
    { root: (
      /*root*/
      ctx[10]
    ) }
  ];
  var switch_value = (
    /*component*/
    ctx[44]
  );
  function switch_props(ctx2, dirty) {
    let switch_instance_props = {};
    for (let i = 0; i < switch_instance_spread_levels.length; i += 1) {
      switch_instance_props = assign(switch_instance_props, switch_instance_spread_levels[i]);
    }
    if (dirty !== void 0 && dirty[0] & /*component_props, component_meta, samples_dir, current_hover, root*/
    5768196) {
      switch_instance_props = assign(switch_instance_props, get_spread_update(switch_instance_spread_levels, [
        dirty[0] & /*component_props*/
        4 && get_spread_object(
          /*component_props*/
          ctx2[2][
            /*j*/
            ctx2[47]
          ]
        ),
        dirty[0] & /*component_meta*/
        1048576 && { value: (
          /*value*/
          ctx2[0]
        ) },
        dirty[0] & /*samples_dir*/
        4194304 && { samples_dir: (
          /*samples_dir*/
          ctx2[22]
        ) },
        switch_instance_spread_levels[3],
        dirty[0] & /*current_hover*/
        524288 && {
          selected: (
            /*current_hover*/
            ctx2[19] === /*i*/
            ctx2[42]
          )
        },
        switch_instance_spread_levels[5],
        dirty[0] & /*root*/
        1024 && { root: (
          /*root*/
          ctx2[10]
        ) }
      ]));
    }
    return { props: switch_instance_props };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
  }
  return {
    c() {
      td = element("td");
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      this.h();
    },
    l(nodes) {
      td = claim_element(nodes, "TD", { style: true, class: true });
      var td_nodes = children(td);
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, td_nodes);
      td_nodes.forEach(detach);
      this.h();
    },
    h() {
      set_style(
        td,
        "max-width",
        /*component_name*/
        ctx[45] === "textbox" ? "35ch" : "auto"
      );
      attr(td, "class", td_class_value = null_to_empty(
        /*component_name*/
        ctx[45]
      ) + " svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, td, anchor);
      if (switch_instance)
        mount_component(switch_instance, td, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*component_meta*/
      1048576 && switch_value !== (switch_value = /*component*/
      ctx2[44])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2, dirty));
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, td, null);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = dirty[0] & /*component_props, component_meta, samples_dir, current_hover, root*/
        5768196 ? get_spread_update(switch_instance_spread_levels, [
          dirty[0] & /*component_props*/
          4 && get_spread_object(
            /*component_props*/
            ctx2[2][
              /*j*/
              ctx2[47]
            ]
          ),
          dirty[0] & /*component_meta*/
          1048576 && { value: (
            /*value*/
            ctx2[0]
          ) },
          dirty[0] & /*samples_dir*/
          4194304 && { samples_dir: (
            /*samples_dir*/
            ctx2[22]
          ) },
          switch_instance_spread_levels[3],
          dirty[0] & /*current_hover*/
          524288 && {
            selected: (
              /*current_hover*/
              ctx2[19] === /*i*/
              ctx2[42]
            )
          },
          switch_instance_spread_levels[5],
          dirty[0] & /*root*/
          1024 && { root: (
            /*root*/
            ctx2[10]
          ) }
        ]) : {};
        switch_instance.$set(switch_instance_changes);
      }
      if (!current || dirty[0] & /*components*/
      2) {
        set_style(
          td,
          "max-width",
          /*component_name*/
          ctx2[45] === "textbox" ? "35ch" : "auto"
        );
      }
      if (!current || dirty[0] & /*components*/
      2 && td_class_value !== (td_class_value = null_to_empty(
        /*component_name*/
        ctx2[45]
      ) + " svelte-p5q82i")) {
        attr(td, "class", td_class_value);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(td);
      }
      if (switch_instance)
        destroy_component(switch_instance);
    }
  };
}
function create_each_block_3(ctx) {
  let show_if = (
    /*component_name*/
    ctx[45] !== void 0 && /*component_map*/
    ctx[3].get(
      /*component_name*/
      ctx[45]
    ) !== void 0
  );
  let if_block_anchor;
  let current;
  let if_block = show_if && create_if_block_6(ctx);
  return {
    c() {
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*components, component_map*/
      10)
        show_if = /*component_name*/
        ctx2[45] !== void 0 && /*component_map*/
        ctx2[3].get(
          /*component_name*/
          ctx2[45]
        ) !== void 0;
      if (show_if) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*components, component_map*/
          10) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_6(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_each_block_2(ctx) {
  let tr;
  let t;
  let current;
  let mounted;
  let dispose;
  let each_value_3 = ensure_array_like(
    /*sample_row*/
    ctx[40]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_3.length; i += 1) {
    each_blocks[i] = create_each_block_3(get_each_context_3(ctx, each_value_3, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  function click_handler_1() {
    return (
      /*click_handler_1*/
      ctx[32](
        /*i*/
        ctx[42]
      )
    );
  }
  function mouseenter_handler_1() {
    return (
      /*mouseenter_handler_1*/
      ctx[33](
        /*i*/
        ctx[42]
      )
    );
  }
  return {
    c() {
      tr = element("tr");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      this.h();
    },
    l(nodes) {
      tr = claim_element(nodes, "TR", { class: true });
      var tr_nodes = children(tr);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(tr_nodes);
      }
      t = claim_space(tr_nodes);
      tr_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(tr, "class", "tr-body svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, tr, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(tr, null);
        }
      }
      append_hydration(tr, t);
      current = true;
      if (!mounted) {
        dispose = [
          listen(tr, "click", click_handler_1),
          listen(tr, "mouseenter", mouseenter_handler_1),
          listen(
            tr,
            "mouseleave",
            /*mouseleave_handler_1*/
            ctx[34]
          )
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*components, component_meta, component_props, samples_dir, current_hover, root, component_map*/
      5768206) {
        each_value_3 = ensure_array_like(
          /*sample_row*/
          ctx[40]
        );
        let i;
        for (i = 0; i < each_value_3.length; i += 1) {
          const child_ctx = get_each_context_3(ctx, each_value_3, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_3(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(tr, t);
          }
        }
        group_outros();
        for (i = each_value_3.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_3.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(tr);
      }
      destroy_each(each_blocks, detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_3(ctx) {
  let button;
  let show_if;
  let current_block_type_index;
  let if_block;
  let t;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_4, create_if_block_5];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (dirty[0] & /*component_meta, component_map, components*/
    1048586)
      show_if = null;
    if (
      /*sample_labels*/
      ctx2[6]
    )
      return 0;
    if (show_if == null)
      show_if = !!/*component_meta*/
      (ctx2[20].length && /*component_map*/
      ctx2[3].get(
        /*components*/
        ctx2[1][0]
      ));
    if (show_if)
      return 1;
    return -1;
  }
  if (~(current_block_type_index = select_block_type_1(ctx, [-1, -1]))) {
    if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  function click_handler() {
    return (
      /*click_handler*/
      ctx[29](
        /*i*/
        ctx[42],
        /*sample_row*/
        ctx[40]
      )
    );
  }
  function mouseenter_handler() {
    return (
      /*mouseenter_handler*/
      ctx[30](
        /*i*/
        ctx[42]
      )
    );
  }
  return {
    c() {
      button = element("button");
      if (if_block)
        if_block.c();
      t = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      if (if_block)
        if_block.l(button_nodes);
      t = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "gallery-item svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(button, null);
      }
      append_hydration(button, t);
      current = true;
      if (!mounted) {
        dispose = [
          listen(button, "click", click_handler),
          listen(button, "mouseenter", mouseenter_handler),
          listen(
            button,
            "mouseleave",
            /*mouseleave_handler*/
            ctx[31]
          )
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx, dirty);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx, dirty);
        }
      } else {
        if (if_block) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block = if_blocks[current_block_type_index];
          if (!if_block) {
            if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
            if_block.c();
          } else {
            if_block.p(ctx, dirty);
          }
          transition_in(if_block, 1);
          if_block.m(button, t);
        } else {
          if_block = null;
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_5(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  const switch_instance_spread_levels = [
    /*component_props*/
    ctx[2][0],
    { value: (
      /*sample_row*/
      ctx[40][0]
    ) },
    { samples_dir: (
      /*samples_dir*/
      ctx[22]
    ) },
    { type: "gallery" },
    {
      selected: (
        /*current_hover*/
        ctx[19] === /*i*/
        ctx[42]
      )
    },
    { index: (
      /*i*/
      ctx[42]
    ) },
    { root: (
      /*root*/
      ctx[10]
    ) }
  ];
  var switch_value = (
    /*component_meta*/
    ctx[20][0][0].component
  );
  function switch_props(ctx2, dirty) {
    let switch_instance_props = {};
    for (let i = 0; i < switch_instance_spread_levels.length; i += 1) {
      switch_instance_props = assign(switch_instance_props, switch_instance_spread_levels[i]);
    }
    if (dirty !== void 0 && dirty[0] & /*component_props, selected_samples, samples_dir, current_hover, root*/
    4850692) {
      switch_instance_props = assign(switch_instance_props, get_spread_update(switch_instance_spread_levels, [
        dirty[0] & /*component_props*/
        4 && get_spread_object(
          /*component_props*/
          ctx2[2][0]
        ),
        dirty[0] & /*selected_samples*/
        131072 && { value: (
          /*sample_row*/
          ctx2[40][0]
        ) },
        dirty[0] & /*samples_dir*/
        4194304 && { samples_dir: (
          /*samples_dir*/
          ctx2[22]
        ) },
        switch_instance_spread_levels[3],
        dirty[0] & /*current_hover*/
        524288 && {
          selected: (
            /*current_hover*/
            ctx2[19] === /*i*/
            ctx2[42]
          )
        },
        switch_instance_spread_levels[5],
        dirty[0] & /*root*/
        1024 && { root: (
          /*root*/
          ctx2[10]
        ) }
      ]));
    }
    return { props: switch_instance_props };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*component_meta*/
      1048576 && switch_value !== (switch_value = /*component_meta*/
      ctx2[20][0][0].component)) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2, dirty));
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = dirty[0] & /*component_props, selected_samples, samples_dir, current_hover, root*/
        4850692 ? get_spread_update(switch_instance_spread_levels, [
          dirty[0] & /*component_props*/
          4 && get_spread_object(
            /*component_props*/
            ctx2[2][0]
          ),
          dirty[0] & /*selected_samples*/
          131072 && { value: (
            /*sample_row*/
            ctx2[40][0]
          ) },
          dirty[0] & /*samples_dir*/
          4194304 && { samples_dir: (
            /*samples_dir*/
            ctx2[22]
          ) },
          switch_instance_spread_levels[3],
          dirty[0] & /*current_hover*/
          524288 && {
            selected: (
              /*current_hover*/
              ctx2[19] === /*i*/
              ctx2[42]
            )
          },
          switch_instance_spread_levels[5],
          dirty[0] & /*root*/
          1024 && { root: (
            /*root*/
            ctx2[10]
          ) }
        ]) : {};
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_4(ctx) {
  let baseexample;
  let current;
  baseexample = new Example({
    props: {
      value: (
        /*sample_row*/
        ctx[40][0]
      ),
      selected: (
        /*current_hover*/
        ctx[19] === /*i*/
        ctx[42]
      ),
      type: "gallery"
    }
  });
  return {
    c() {
      create_component(baseexample.$$.fragment);
    },
    l(nodes) {
      claim_component(baseexample.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(baseexample, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const baseexample_changes = {};
      if (dirty[0] & /*selected_samples*/
      131072)
        baseexample_changes.value = /*sample_row*/
        ctx2[40][0];
      if (dirty[0] & /*current_hover*/
      524288)
        baseexample_changes.selected = /*current_hover*/
        ctx2[19] === /*i*/
        ctx2[42];
      baseexample.$set(baseexample_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(baseexample.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(baseexample.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(baseexample, detaching);
    }
  };
}
function create_each_block_1(ctx) {
  let if_block_anchor;
  let current;
  let if_block = (
    /*sample_row*/
    ctx[40][0] && create_if_block_3(ctx)
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*sample_row*/
        ctx2[40][0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*selected_samples*/
          131072) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_3(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block(ctx) {
  let div;
  let t;
  let each_value = ensure_array_like(
    /*visible_pages*/
    ctx[18]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  return {
    c() {
      div = element("div");
      t = text("Pages:\n			");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      t = claim_text(div_nodes, "Pages:\n			");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "paginate svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*visible_pages, page*/
      294912) {
        each_value = ensure_array_like(
          /*visible_pages*/
          ctx2[18]
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
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_else_block(ctx) {
  let button;
  let t0_value = (
    /*visible_page*/
    ctx[37] + 1 + ""
  );
  let t0;
  let t1;
  let mounted;
  let dispose;
  function click_handler_2() {
    return (
      /*click_handler_2*/
      ctx[35](
        /*visible_page*/
        ctx[37]
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
      attr(button, "class", "svelte-p5q82i");
      toggle_class(
        button,
        "current-page",
        /*page*/
        ctx[15] === /*visible_page*/
        ctx[37]
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
      if (dirty[0] & /*visible_pages*/
      262144 && t0_value !== (t0_value = /*visible_page*/
      ctx[37] + 1 + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*page, visible_pages*/
      294912) {
        toggle_class(
          button,
          "current-page",
          /*page*/
          ctx[15] === /*visible_page*/
          ctx[37]
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
function create_if_block_1(ctx) {
  let div;
  let textContent = "...";
  return {
    c() {
      div = element("div");
      div.textContent = textContent;
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { ["data-svelte-h"]: true });
      if (get_svelte_dataset(div) !== "svelte-12rhcfw")
        div.textContent = textContent;
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_each_block(ctx) {
  let if_block_anchor;
  function select_block_type_2(ctx2, dirty) {
    if (
      /*visible_page*/
      ctx2[37] === -1
    )
      return create_if_block_1;
    return create_else_block;
  }
  let current_block_type = select_block_type_2(ctx);
  let if_block = current_block_type(ctx);
  return {
    c() {
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (current_block_type === (current_block_type = select_block_type_2(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      }
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if_block.d(detaching);
    }
  };
}
function create_default_slot(ctx) {
  let div;
  let svg;
  let path;
  let t0;
  let t1;
  let t2;
  let current_block_type_index;
  let if_block0;
  let t3;
  let if_block1_anchor;
  let current;
  const if_block_creators = [create_if_block_2, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*gallery*/
      ctx2[21]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block1 = (
    /*paginate*/
    ctx[16] && create_if_block(ctx)
  );
  return {
    c() {
      div = element("div");
      svg = svg_element("svg");
      path = svg_element("path");
      t0 = space();
      t1 = text(
        /*label*/
        ctx[4]
      );
      t2 = space();
      if_block0.c();
      t3 = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      svg = claim_svg_element(div_nodes, "svg", {
        xmlns: true,
        "xmlns:xlink": true,
        "aria-hidden": true,
        role: true,
        width: true,
        height: true,
        preserveAspectRatio: true,
        viewBox: true,
        class: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      t0 = claim_space(div_nodes);
      t1 = claim_text(
        div_nodes,
        /*label*/
        ctx[4]
      );
      div_nodes.forEach(detach);
      t2 = claim_space(nodes);
      if_block0.l(nodes);
      t3 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
      this.h();
    },
    h() {
      attr(path, "fill", "currentColor");
      attr(path, "d", "M10 6h18v2H10zm0 18h18v2H10zm0-9h18v2H10zm-6 0h2v2H4zm0-9h2v2H4zm0 18h2v2H4z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "xmlns:xlink", "http://www.w3.org/1999/xlink");
      attr(svg, "aria-hidden", "true");
      attr(svg, "role", "img");
      attr(svg, "width", "1em");
      attr(svg, "height", "1em");
      attr(svg, "preserveAspectRatio", "xMidYMid meet");
      attr(svg, "viewBox", "0 0 32 32");
      attr(svg, "class", "svelte-p5q82i");
      attr(div, "class", "label svelte-p5q82i");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, svg);
      append_hydration(svg, path);
      append_hydration(div, t0);
      append_hydration(div, t1);
      insert_hydration(target, t2, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, t3, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (!current || dirty[0] & /*label*/
      16)
        set_data(
          t1,
          /*label*/
          ctx2[4]
        );
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block0.c();
        } else {
          if_block0.p(ctx2, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(t3.parentNode, t3);
      }
      if (
        /*paginate*/
        ctx2[16]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
        } else {
          if_block1 = create_if_block(ctx2);
          if_block1.c();
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t2);
        detach(t3);
        detach(if_block1_anchor);
      }
      if_blocks[current_block_type_index].d(detaching);
      if (if_block1)
        if_block1.d(detaching);
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
        ctx[9]
      ),
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[7]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[8]
      ),
      scale: (
        /*scale*/
        ctx[12]
      ),
      min_width: (
        /*min_width*/
        ctx[13]
      ),
      allow_overflow: false,
      container: false,
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
      512)
        block_changes.visible = /*visible*/
        ctx2[9];
      if (dirty[0] & /*elem_id*/
      128)
        block_changes.elem_id = /*elem_id*/
        ctx2[7];
      if (dirty[0] & /*elem_classes*/
      256)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[8];
      if (dirty[0] & /*scale*/
      4096)
        block_changes.scale = /*scale*/
        ctx2[12];
      if (dirty[0] & /*min_width*/
      8192)
        block_changes.min_width = /*min_width*/
        ctx2[13];
      if (dirty[0] & /*visible_pages, page, paginate, selected_samples, value, samples_per_page, gradio, current_hover, sample_labels, component_meta, component_props, root, component_map, components, gallery, headers, label*/
      4181119 | dirty[1] & /*$$scope*/
      1048576) {
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
  let gallery;
  let { components } = $$props;
  let { component_props } = $$props;
  let { component_map } = $$props;
  let { label = "Examples" } = $$props;
  let { headers } = $$props;
  let { samples = null } = $$props;
  let old_samples = null;
  let { sample_labels = null } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { root } = $$props;
  let { proxy_url } = $$props;
  let { samples_per_page = 10 } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let samples_dir = proxy_url ? `/proxy=${proxy_url}file=` : `${root}/file=`;
  let page = 0;
  let paginate = samples ? samples.length > samples_per_page : false;
  let selected_samples;
  let page_count;
  let visible_pages = [];
  let current_hover = -1;
  function handle_mouseenter(i) {
    $$invalidate(19, current_hover = i);
  }
  function handle_mouseleave() {
    $$invalidate(19, current_hover = -1);
  }
  let component_meta = [];
  async function get_component_meta(selected_samples2) {
    $$invalidate(20, component_meta = await Promise.all(selected_samples2 && selected_samples2.map(async (sample_row) => await Promise.all(sample_row.map(async (sample_cell, j) => {
      var _a;
      return {
        value: sample_cell,
        component: (_a = await component_map.get(components[j])) == null ? void 0 : _a.default
      };
    })))));
  }
  const click_handler = (i, sample_row) => {
    $$invalidate(0, value = i + page * samples_per_page);
    gradio.dispatch("click", value);
    gradio.dispatch("select", { index: value, value: sample_row });
  };
  const mouseenter_handler = (i) => handle_mouseenter(i);
  const mouseleave_handler = () => handle_mouseleave();
  const click_handler_1 = (i) => {
    $$invalidate(0, value = i + page * samples_per_page);
    gradio.dispatch("click", value);
  };
  const mouseenter_handler_1 = (i) => handle_mouseenter(i);
  const mouseleave_handler_1 = () => handle_mouseleave();
  const click_handler_2 = (visible_page) => $$invalidate(15, page = visible_page);
  $$self.$$set = ($$props2) => {
    if ("components" in $$props2)
      $$invalidate(1, components = $$props2.components);
    if ("component_props" in $$props2)
      $$invalidate(2, component_props = $$props2.component_props);
    if ("component_map" in $$props2)
      $$invalidate(3, component_map = $$props2.component_map);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("headers" in $$props2)
      $$invalidate(5, headers = $$props2.headers);
    if ("samples" in $$props2)
      $$invalidate(25, samples = $$props2.samples);
    if ("sample_labels" in $$props2)
      $$invalidate(6, sample_labels = $$props2.sample_labels);
    if ("elem_id" in $$props2)
      $$invalidate(7, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(8, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(9, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("root" in $$props2)
      $$invalidate(10, root = $$props2.root);
    if ("proxy_url" in $$props2)
      $$invalidate(26, proxy_url = $$props2.proxy_url);
    if ("samples_per_page" in $$props2)
      $$invalidate(11, samples_per_page = $$props2.samples_per_page);
    if ("scale" in $$props2)
      $$invalidate(12, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(13, min_width = $$props2.min_width);
    if ("gradio" in $$props2)
      $$invalidate(14, gradio = $$props2.gradio);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*components, sample_labels*/
    66) {
      $$invalidate(21, gallery = components.length < 2 || sample_labels !== null);
    }
    if ($$self.$$.dirty[0] & /*sample_labels, samples, old_samples, samples_per_page, paginate, page, page_count, visible_pages*/
    436570176) {
      {
        if (sample_labels) {
          $$invalidate(25, samples = sample_labels.map((e) => [e]));
        } else if (!samples) {
          $$invalidate(25, samples = []);
        }
        if (samples !== old_samples) {
          $$invalidate(15, page = 0);
          $$invalidate(27, old_samples = samples);
        }
        $$invalidate(16, paginate = samples.length > samples_per_page);
        if (paginate) {
          $$invalidate(18, visible_pages = []);
          $$invalidate(17, selected_samples = samples.slice(page * samples_per_page, (page + 1) * samples_per_page));
          $$invalidate(28, page_count = Math.ceil(samples.length / samples_per_page));
          [0, page, page_count - 1].forEach((anchor) => {
            for (let i = anchor - 2; i <= anchor + 2; i++) {
              if (i >= 0 && i < page_count && !visible_pages.includes(i)) {
                if (visible_pages.length > 0 && i - visible_pages[visible_pages.length - 1] > 1) {
                  visible_pages.push(-1);
                }
                visible_pages.push(i);
              }
            }
          });
        } else {
          $$invalidate(17, selected_samples = samples.slice());
        }
      }
    }
    if ($$self.$$.dirty[0] & /*component_map, selected_samples*/
    131080) {
      get_component_meta(selected_samples);
    }
  };
  return [
    value,
    components,
    component_props,
    component_map,
    label,
    headers,
    sample_labels,
    elem_id,
    elem_classes,
    visible,
    root,
    samples_per_page,
    scale,
    min_width,
    gradio,
    page,
    paginate,
    selected_samples,
    visible_pages,
    current_hover,
    component_meta,
    gallery,
    samples_dir,
    handle_mouseenter,
    handle_mouseleave,
    samples,
    proxy_url,
    old_samples,
    page_count,
    click_handler,
    mouseenter_handler,
    mouseleave_handler,
    click_handler_1,
    mouseenter_handler_1,
    mouseleave_handler_1,
    click_handler_2
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
        components: 1,
        component_props: 2,
        component_map: 3,
        label: 4,
        headers: 5,
        samples: 25,
        sample_labels: 6,
        elem_id: 7,
        elem_classes: 8,
        visible: 9,
        value: 0,
        root: 10,
        proxy_url: 26,
        samples_per_page: 11,
        scale: 12,
        min_width: 13,
        gradio: 14
      },
      null,
      [-1, -1]
    );
  }
}
export {
  Index as default
};
