import { SvelteComponent, init, safe_not_equal, empty, insert_hydration, noop, detach, element, claim_element, children, attr, toggle_class, listen, run_all, ensure_array_like, space, claim_space, append_hydration, destroy_each, text, claim_text, set_data, get_svelte_dataset } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[8] = list[i];
  child_ctx[10] = i;
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[11] = list[i];
  child_ctx[13] = i;
  return child_ctx;
}
function create_if_block(ctx) {
  let div;
  let mounted;
  let dispose;
  function select_block_type(ctx2, dirty) {
    if (typeof /*value*/
    ctx2[0] === "string")
      return create_if_block_1;
    return create_else_block;
  }
  let current_block_type = select_block_type(ctx);
  let if_block = current_block_type(ctx);
  return {
    c() {
      div = element("div");
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "svelte-1cib1xd");
      toggle_class(
        div,
        "table",
        /*type*/
        ctx[1] === "table"
      );
      toggle_class(
        div,
        "gallery",
        /*type*/
        ctx[1] === "gallery"
      );
      toggle_class(
        div,
        "selected",
        /*selected*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if_block.m(div, null);
      if (!mounted) {
        dispose = [
          listen(
            div,
            "mouseenter",
            /*mouseenter_handler*/
            ctx[6]
          ),
          listen(
            div,
            "mouseleave",
            /*mouseleave_handler*/
            ctx[7]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(div, null);
        }
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (dirty & /*selected*/
      4) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx2[2]
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if_block.d();
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_else_block(ctx) {
  let table;
  let t;
  let each_value = ensure_array_like(
    /*value*/
    ctx[0].slice(0, 3)
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  let if_block = (
    /*value*/
    ctx[0].length > 3 && create_if_block_2(ctx)
  );
  return {
    c() {
      table = element("table");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      table = claim_element(nodes, "TABLE", { class: true });
      var table_nodes = children(table);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(table_nodes);
      }
      t = claim_space(table_nodes);
      if (if_block)
        if_block.l(table_nodes);
      table_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(table, "class", " svelte-1cib1xd");
    },
    m(target, anchor) {
      insert_hydration(target, table, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(table, null);
        }
      }
      append_hydration(table, t);
      if (if_block)
        if_block.m(table, null);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1) {
        each_value = ensure_array_like(
          /*value*/
          ctx2[0].slice(0, 3)
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(table, t);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
      }
      if (
        /*value*/
        ctx2[0].length > 3
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_2(ctx2);
          if_block.c();
          if_block.m(table, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(table);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d();
    }
  };
}
function create_if_block_1(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*value*/
        ctx[0]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*value*/
        ctx[0]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1)
        set_data(
          t,
          /*value*/
          ctx2[0]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_each_block_1(ctx) {
  let td;
  let t_value = (
    /*cell*/
    ctx[11] + ""
  );
  let t;
  return {
    c() {
      td = element("td");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      td = claim_element(nodes, "TD", { class: true });
      var td_nodes = children(td);
      t = claim_text(td_nodes, t_value);
      td_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(td, "class", "svelte-1cib1xd");
    },
    m(target, anchor) {
      insert_hydration(target, td, anchor);
      append_hydration(td, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*cell*/
      ctx2[11] + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(td);
      }
    }
  };
}
function create_if_block_3(ctx) {
  let td;
  let textContent = "…";
  return {
    c() {
      td = element("td");
      td.textContent = textContent;
      this.h();
    },
    l(nodes) {
      td = claim_element(nodes, "TD", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(td) !== "svelte-1o35md4")
        td.textContent = textContent;
      this.h();
    },
    h() {
      attr(td, "class", "svelte-1cib1xd");
    },
    m(target, anchor) {
      insert_hydration(target, td, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(td);
      }
    }
  };
}
function create_each_block(ctx) {
  let tr;
  let t;
  let each_value_1 = ensure_array_like(
    /*row*/
    ctx[8].slice(0, 3)
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
  }
  let if_block = (
    /*row*/
    ctx[8].length > 3 && create_if_block_3()
  );
  return {
    c() {
      tr = element("tr");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      if (if_block)
        if_block.c();
    },
    l(nodes) {
      tr = claim_element(nodes, "TR", {});
      var tr_nodes = children(tr);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(tr_nodes);
      }
      t = claim_space(tr_nodes);
      if (if_block)
        if_block.l(tr_nodes);
      tr_nodes.forEach(detach);
    },
    m(target, anchor) {
      insert_hydration(target, tr, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(tr, null);
        }
      }
      append_hydration(tr, t);
      if (if_block)
        if_block.m(tr, null);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1) {
        each_value_1 = ensure_array_like(
          /*row*/
          ctx2[8].slice(0, 3)
        );
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(tr, t);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_1.length;
      }
      if (
        /*row*/
        ctx2[8].length > 3
      ) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_3();
          if_block.c();
          if_block.m(tr, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(tr);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d();
    }
  };
}
function create_if_block_2(ctx) {
  let div;
  return {
    c() {
      div = element("div");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      children(div).forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "overlay svelte-1cib1xd");
      toggle_class(
        div,
        "odd",
        /*index*/
        ctx[3] % 2 != 0
      );
      toggle_class(
        div,
        "even",
        /*index*/
        ctx[3] % 2 == 0
      );
      toggle_class(
        div,
        "button",
        /*type*/
        ctx[1] === "gallery"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*index*/
      8) {
        toggle_class(
          div,
          "odd",
          /*index*/
          ctx2[3] % 2 != 0
        );
      }
      if (dirty & /*index*/
      8) {
        toggle_class(
          div,
          "even",
          /*index*/
          ctx2[3] % 2 == 0
        );
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          div,
          "button",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_fragment(ctx) {
  let if_block_anchor;
  let if_block = (
    /*loaded*/
    ctx[5] && create_if_block(ctx)
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
    },
    p(ctx2, [dirty]) {
      if (
        /*loaded*/
        ctx2[5]
      )
        if_block.p(ctx2, dirty);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let { index } = $$props;
  let hovered = false;
  let loaded = Array.isArray(value);
  const mouseenter_handler = () => $$invalidate(4, hovered = true);
  const mouseleave_handler = () => $$invalidate(4, hovered = false);
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
    if ("index" in $$props2)
      $$invalidate(3, index = $$props2.index);
  };
  return [
    value,
    type,
    selected,
    index,
    hovered,
    loaded,
    mouseenter_handler,
    mouseleave_handler
  ];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0, type: 1, selected: 2, index: 3 });
  }
}
export {
  Example as default
};
