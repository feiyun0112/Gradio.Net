import { SvelteComponent, init, safe_not_equal, empty, insert_hydration, group_outros, transition_out, check_outros, transition_in, detach, ensure_array_like, element, text, space, claim_element, children, claim_text, claim_space, attr, toggle_class, append_hydration, listen, set_data, destroy_each, run_all, noop, get_svelte_dataset, create_component, claim_component, mount_component, destroy_component, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static } from "./2.BqWhUxOo.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[10] = list[i];
  child_ctx[12] = i;
  return child_ctx;
}
function create_else_block(ctx) {
  let span0;
  let t0_value = (
    /*brackets*/
    ctx[6][0] + ""
  );
  let t0;
  let t1;
  let ul;
  let t2;
  let span1;
  let t3_value = (
    /*brackets*/
    ctx[6][1] + ""
  );
  let t3;
  let if_block_anchor;
  let current;
  let mounted;
  let dispose;
  let each_value = ensure_array_like(
    /*items*/
    ctx[5]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  let if_block = !/*_last*/
  ctx[3] && create_if_block_4();
  return {
    c() {
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      ul = element("ul");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t2 = space();
      span1 = element("span");
      t3 = text(t3_value);
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      span0 = claim_element(nodes, "SPAN", { class: true, role: true, tabindex: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(nodes);
      ul = claim_element(nodes, "UL", { class: true });
      var ul_nodes = children(ul);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(ul_nodes);
      }
      ul_nodes.forEach(detach);
      t2 = claim_space(nodes);
      span1 = claim_element(nodes, "SPAN", { class: true, role: true, tabindex: true });
      var span1_nodes = children(span1);
      t3 = claim_text(span1_nodes, t3_value);
      span1_nodes.forEach(detach);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(span0, "class", "_jsonBkt svelte-ei2xnu");
      attr(span0, "role", "button");
      attr(span0, "tabindex", "0");
      toggle_class(
        span0,
        "isArray",
        /*isArray*/
        ctx[4]
      );
      attr(ul, "class", "_jsonList svelte-ei2xnu");
      attr(span1, "class", "_jsonBkt svelte-ei2xnu");
      attr(span1, "role", "button");
      attr(span1, "tabindex", "0");
      toggle_class(
        span1,
        "isArray",
        /*isArray*/
        ctx[4]
      );
    },
    m(target, anchor) {
      insert_hydration(target, span0, anchor);
      append_hydration(span0, t0);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, ul, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(ul, null);
        }
      }
      insert_hydration(target, t2, anchor);
      insert_hydration(target, span1, anchor);
      append_hydration(span1, t3);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            span0,
            "click",
            /*clicked*/
            ctx[8]
          ),
          listen(
            span0,
            "keydown",
            /*pressed*/
            ctx[9]
          ),
          listen(
            span1,
            "click",
            /*clicked*/
            ctx[8]
          ),
          listen(
            span1,
            "keydown",
            /*pressed*/
            ctx[9]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if ((!current || dirty & /*brackets*/
      64) && t0_value !== (t0_value = /*brackets*/
      ctx2[6][0] + ""))
        set_data(t0, t0_value);
      if (!current || dirty & /*isArray*/
      16) {
        toggle_class(
          span0,
          "isArray",
          /*isArray*/
          ctx2[4]
        );
      }
      if (dirty & /*json, items, depth, _cur, getType, format, isArray*/
      55) {
        each_value = ensure_array_like(
          /*items*/
          ctx2[5]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(ul, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if ((!current || dirty & /*brackets*/
      64) && t3_value !== (t3_value = /*brackets*/
      ctx2[6][1] + ""))
        set_data(t3, t3_value);
      if (!current || dirty & /*isArray*/
      16) {
        toggle_class(
          span1,
          "isArray",
          /*isArray*/
          ctx2[4]
        );
      }
      if (!/*_last*/
      ctx2[3]) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_4();
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value.length; i += 1) {
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
        detach(span0);
        detach(t1);
        detach(ul);
        detach(t2);
        detach(span1);
        detach(if_block_anchor);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_2(ctx) {
  let span;
  let t0_value = (
    /*brackets*/
    ctx[6][0] + ""
  );
  let t0;
  let t1;
  let t2_value = (
    /*brackets*/
    ctx[6][1] + ""
  );
  let t2;
  let if_block_anchor;
  let mounted;
  let dispose;
  let if_block = !/*_last*/
  ctx[3] && /*collapsed*/
  ctx[7] && create_if_block_3();
  return {
    c() {
      span = element("span");
      t0 = text(t0_value);
      t1 = text("...");
      t2 = text(t2_value);
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, role: true, tabindex: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, t0_value);
      t1 = claim_text(span_nodes, "...");
      t2 = claim_text(span_nodes, t2_value);
      span_nodes.forEach(detach);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(span, "class", "_jsonBkt svelte-ei2xnu");
      attr(span, "role", "button");
      attr(span, "tabindex", "0");
      toggle_class(
        span,
        "isArray",
        /*isArray*/
        ctx[4]
      );
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t0);
      append_hydration(span, t1);
      append_hydration(span, t2);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      if (!mounted) {
        dispose = [
          listen(
            span,
            "click",
            /*clicked*/
            ctx[8]
          ),
          listen(
            span,
            "keydown",
            /*pressed*/
            ctx[9]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*brackets*/
      64 && t0_value !== (t0_value = /*brackets*/
      ctx2[6][0] + ""))
        set_data(t0, t0_value);
      if (dirty & /*brackets*/
      64 && t2_value !== (t2_value = /*brackets*/
      ctx2[6][1] + ""))
        set_data(t2, t2_value);
      if (dirty & /*isArray*/
      16) {
        toggle_class(
          span,
          "isArray",
          /*isArray*/
          ctx2[4]
        );
      }
      if (!/*_last*/
      ctx2[3] && /*collapsed*/
      ctx2[7]) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_3();
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block$1(ctx) {
  let span;
  let t0_value = (
    /*brackets*/
    ctx[6][0] + ""
  );
  let t0;
  let t1_value = (
    /*brackets*/
    ctx[6][1] + ""
  );
  let t1;
  let if_block_anchor;
  let if_block = !/*_last*/
  ctx[3] && create_if_block_1();
  return {
    c() {
      span = element("span");
      t0 = text(t0_value);
      t1 = text(t1_value);
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, t0_value);
      t1 = claim_text(span_nodes, t1_value);
      span_nodes.forEach(detach);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(span, "class", "_jsonBkt empty svelte-ei2xnu");
      toggle_class(
        span,
        "isArray",
        /*isArray*/
        ctx[4]
      );
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t0);
      append_hydration(span, t1);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*brackets*/
      64 && t0_value !== (t0_value = /*brackets*/
      ctx2[6][0] + ""))
        set_data(t0, t0_value);
      if (dirty & /*brackets*/
      64 && t1_value !== (t1_value = /*brackets*/
      ctx2[6][1] + ""))
        set_data(t1, t1_value);
      if (dirty & /*isArray*/
      16) {
        toggle_class(
          span,
          "isArray",
          /*isArray*/
          ctx2[4]
        );
      }
      if (!/*_last*/
      ctx2[3]) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_1();
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block_7(ctx) {
  let span0;
  let t0;
  let t1_value = (
    /*i*/
    ctx[10] + ""
  );
  let t1;
  let t2;
  let span1;
  let textContent = ":";
  return {
    c() {
      span0 = element("span");
      t0 = text('"');
      t1 = text(t1_value);
      t2 = text('"');
      span1 = element("span");
      span1.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span0 = claim_element(nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, '"');
      t1 = claim_text(span0_nodes, t1_value);
      t2 = claim_text(span0_nodes, '"');
      span0_nodes.forEach(detach);
      span1 = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span1) !== "svelte-168684w")
        span1.textContent = textContent;
      this.h();
    },
    h() {
      attr(span0, "class", "_jsonKey svelte-ei2xnu");
      attr(span1, "class", "_jsonSep svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span0, anchor);
      append_hydration(span0, t0);
      append_hydration(span0, t1);
      append_hydration(span0, t2);
      insert_hydration(target, span1, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*items*/
      32 && t1_value !== (t1_value = /*i*/
      ctx2[10] + ""))
        set_data(t1, t1_value);
    },
    d(detaching) {
      if (detaching) {
        detach(span0);
        detach(span1);
      }
    }
  };
}
function create_else_block_1(ctx) {
  let span;
  let t_value = format(
    /*json*/
    ctx[0][
      /*i*/
      ctx[10]
    ]
  ) + "";
  let t;
  let span_class_value;
  let if_block_anchor;
  let if_block = (
    /*idx*/
    ctx[12] < /*items*/
    ctx[5].length - 1 && create_if_block_6()
  );
  return {
    c() {
      span = element("span");
      t = text(t_value);
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t = claim_text(span_nodes, t_value);
      span_nodes.forEach(detach);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(span, "class", span_class_value = "_jsonVal " + getType(
        /*json*/
        ctx[0][
          /*i*/
          ctx[10]
        ]
      ) + " svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*json, items*/
      33 && t_value !== (t_value = format(
        /*json*/
        ctx2[0][
          /*i*/
          ctx2[10]
        ]
      ) + ""))
        set_data(t, t_value);
      if (dirty & /*json, items*/
      33 && span_class_value !== (span_class_value = "_jsonVal " + getType(
        /*json*/
        ctx2[0][
          /*i*/
          ctx2[10]
        ]
      ) + " svelte-ei2xnu")) {
        attr(span, "class", span_class_value);
      }
      if (
        /*idx*/
        ctx2[12] < /*items*/
        ctx2[5].length - 1
      ) {
        if (if_block)
          ;
        else {
          if_block = create_if_block_6();
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block_5(ctx) {
  let jsonview;
  let current;
  jsonview = new JsonView({
    props: {
      json: (
        /*json*/
        ctx[0][
          /*i*/
          ctx[10]
        ]
      ),
      depth: (
        /*depth*/
        ctx[1]
      ),
      _cur: (
        /*_cur*/
        ctx[2] + 1
      ),
      _last: (
        /*idx*/
        ctx[12] === /*items*/
        ctx[5].length - 1
      )
    }
  });
  return {
    c() {
      create_component(jsonview.$$.fragment);
    },
    l(nodes) {
      claim_component(jsonview.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(jsonview, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const jsonview_changes = {};
      if (dirty & /*json, items*/
      33)
        jsonview_changes.json = /*json*/
        ctx2[0][
          /*i*/
          ctx2[10]
        ];
      if (dirty & /*depth*/
      2)
        jsonview_changes.depth = /*depth*/
        ctx2[1];
      if (dirty & /*_cur*/
      4)
        jsonview_changes._cur = /*_cur*/
        ctx2[2] + 1;
      if (dirty & /*items*/
      32)
        jsonview_changes._last = /*idx*/
        ctx2[12] === /*items*/
        ctx2[5].length - 1;
      jsonview.$set(jsonview_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(jsonview.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(jsonview.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(jsonview, detaching);
    }
  };
}
function create_if_block_6(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-1inngla")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "_jsonSep svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_each_block(ctx) {
  let li;
  let t0;
  let show_if;
  let current_block_type_index;
  let if_block1;
  let t1;
  let current;
  let if_block0 = !/*isArray*/
  ctx[4] && create_if_block_7(ctx);
  const if_block_creators = [create_if_block_5, create_else_block_1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (dirty & /*json, items*/
    33)
      show_if = null;
    if (show_if == null)
      show_if = !!(getType(
        /*json*/
        ctx2[0][
          /*i*/
          ctx2[10]
        ]
      ) === "object");
    if (show_if)
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx, -1);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      li = element("li");
      if (if_block0)
        if_block0.c();
      t0 = space();
      if_block1.c();
      t1 = space();
      this.h();
    },
    l(nodes) {
      li = claim_element(nodes, "LI", { class: true });
      var li_nodes = children(li);
      if (if_block0)
        if_block0.l(li_nodes);
      t0 = claim_space(li_nodes);
      if_block1.l(li_nodes);
      t1 = claim_space(li_nodes);
      li_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(li, "class", "svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, li, anchor);
      if (if_block0)
        if_block0.m(li, null);
      append_hydration(li, t0);
      if_blocks[current_block_type_index].m(li, null);
      append_hydration(li, t1);
      current = true;
    },
    p(ctx2, dirty) {
      if (!/*isArray*/
      ctx2[4]) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_7(ctx2);
          if_block0.c();
          if_block0.m(li, t0);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2, dirty);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(li, t1);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(li);
      }
      if (if_block0)
        if_block0.d();
      if_blocks[current_block_type_index].d();
    }
  };
}
function create_if_block_4(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-1inngla")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "_jsonSep svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_3(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-1inngla")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "_jsonSep svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_1(ctx) {
  let span;
  let textContent = ",";
  return {
    c() {
      span = element("span");
      span.textContent = textContent;
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span) !== "svelte-1f29ohw")
        span.textContent = textContent;
      this.h();
    },
    h() {
      attr(span, "class", "_jsonSep svelte-ei2xnu");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_fragment$1(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block$1, create_if_block_2, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*items*/
    ctx2[5].length)
      return 0;
    if (
      /*collapsed*/
      ctx2[7]
    )
      return 1;
    return 2;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
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
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
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
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block.c();
        } else {
          if_block.p(ctx2, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(if_block_anchor.parentNode, if_block_anchor);
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
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function getType(i) {
  if (i === null)
    return "null";
  return typeof i;
}
function format(i) {
  const t = getType(i);
  if (t === "string")
    return `"${i}"`;
  if (t === "function")
    return "f () {...}";
  if (t === "symbol")
    return i.toString();
  return i;
}
function instance$1($$self, $$props, $$invalidate) {
  let { json } = $$props;
  let { depth = Infinity } = $$props;
  let { _cur = 0 } = $$props;
  let { _last = true } = $$props;
  let items;
  let isArray = false;
  let brackets = ["", ""];
  let collapsed = false;
  function clicked() {
    $$invalidate(7, collapsed = !collapsed);
  }
  function pressed(e) {
    if (e instanceof KeyboardEvent && ["Enter", " "].includes(e.key))
      clicked();
  }
  $$self.$$set = ($$props2) => {
    if ("json" in $$props2)
      $$invalidate(0, json = $$props2.json);
    if ("depth" in $$props2)
      $$invalidate(1, depth = $$props2.depth);
    if ("_cur" in $$props2)
      $$invalidate(2, _cur = $$props2._cur);
    if ("_last" in $$props2)
      $$invalidate(3, _last = $$props2._last);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*json, isArray*/
    17) {
      {
        $$invalidate(5, items = getType(json) === "object" ? Object.keys(json) : []);
        $$invalidate(4, isArray = Array.isArray(json));
        $$invalidate(6, brackets = isArray ? ["[", "]"] : ["{", "}"]);
      }
    }
    if ($$self.$$.dirty & /*depth, _cur*/
    6) {
      $$invalidate(7, collapsed = depth < _cur);
    }
  };
  return [
    json,
    depth,
    _cur,
    _last,
    isArray,
    items,
    brackets,
    collapsed,
    clicked,
    pressed
  ];
}
class JsonView extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, { json: 0, depth: 1, _cur: 2, _last: 3 });
  }
}
function create_if_block(ctx) {
  let statustracker;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[8].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[8].i18n
    ) },
    /*loading_status*/
    ctx[7]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[9]
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
      const statustracker_changes = dirty & /*gradio, loading_status*/
      384 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        256 && { autoscroll: (
          /*gradio*/
          ctx2[8].autoscroll
        ) },
        dirty & /*gradio*/
        256 && { i18n: (
          /*gradio*/
          ctx2[8].i18n
        ) },
        dirty & /*loading_status*/
        128 && get_spread_object(
          /*loading_status*/
          ctx2[7]
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
  let jsonview;
  let current;
  let if_block = (
    /*loading_status*/
    ctx[7] && create_if_block(ctx)
  );
  jsonview = new JsonView({ props: { json: (
    /*value*/
    ctx[3]
  ) } });
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      create_component(jsonview.$$.fragment);
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      claim_component(jsonview.$$.fragment, nodes);
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(jsonview, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*loading_status*/
        ctx2[7]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*loading_status*/
          128) {
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
      const jsonview_changes = {};
      if (dirty & /*value*/
      8)
        jsonview_changes.json = /*value*/
        ctx2[3];
      jsonview.$set(jsonview_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(jsonview.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(jsonview.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_component(jsonview, detaching);
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
        ctx[2]
      ),
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      container: (
        /*container*/
        ctx[4]
      ),
      scale: (
        /*scale*/
        ctx[5]
      ),
      min_width: (
        /*min_width*/
        ctx[6]
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
      4)
        block_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*elem_id*/
      1)
        block_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*container*/
      16)
        block_changes.container = /*container*/
        ctx2[4];
      if (dirty & /*scale*/
      32)
        block_changes.scale = /*scale*/
        ctx2[5];
      if (dirty & /*min_width*/
      64)
        block_changes.min_width = /*min_width*/
        ctx2[6];
      if (dirty & /*$$scope, value, gradio, loading_status*/
      1416) {
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
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("container" in $$props2)
      $$invalidate(4, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(5, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(6, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(7, loading_status = $$props2.loading_status);
    if ("gradio" in $$props2)
      $$invalidate(8, gradio = $$props2.gradio);
  };
  return [
    elem_id,
    elem_classes,
    visible,
    value,
    container,
    scale,
    min_width,
    loading_status,
    gradio,
    clear_status_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      visible: 2,
      value: 3,
      container: 4,
      scale: 5,
      min_width: 6,
      loading_status: 7,
      gradio: 8
    });
  }
}
export {
  Index as default
};
