import { SvelteComponent, init, safe_not_equal, element, claim_element, children, detach, attr, toggle_class, insert_hydration, noop, ensure_array_like, space, empty, claim_space, destroy_each, text, claim_text, append_hydration, set_data, get_svelte_dataset } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[3] = list[i];
  return child_ctx;
}
function create_if_block(ctx) {
  let t;
  let show_if = Array.isArray(
    /*value*/
    ctx[0]
  ) && /*value*/
  ctx[0].length > 3;
  let if_block_anchor;
  let each_value = ensure_array_like(Array.isArray(
    /*value*/
    ctx[0]
  ) ? (
    /*value*/
    ctx[0].slice(0, 3)
  ) : [
    /*value*/
    ctx[0]
  ]);
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  let if_block = show_if && create_if_block_1();
  return {
    c() {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(nodes);
      }
      t = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(target, anchor);
        }
      }
      insert_hydration(target, t, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*Array, value*/
      1) {
        each_value = ensure_array_like(Array.isArray(
          /*value*/
          ctx2[0]
        ) ? (
          /*value*/
          ctx2[0].slice(0, 3)
        ) : [
          /*value*/
          ctx2[0]
        ]);
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(t.parentNode, t);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value.length;
      }
      if (dirty & /*value*/
      1)
        show_if = Array.isArray(
          /*value*/
          ctx2[0]
        ) && /*value*/
        ctx2[0].length > 3;
      if (show_if) {
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
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_each_block(ctx) {
  let li;
  let code;
  let t0;
  let t1_value = (
    /*path*/
    ctx[3] + ""
  );
  let t1;
  return {
    c() {
      li = element("li");
      code = element("code");
      t0 = text("./");
      t1 = text(t1_value);
    },
    l(nodes) {
      li = claim_element(nodes, "LI", {});
      var li_nodes = children(li);
      code = claim_element(li_nodes, "CODE", {});
      var code_nodes = children(code);
      t0 = claim_text(code_nodes, "./");
      t1 = claim_text(code_nodes, t1_value);
      code_nodes.forEach(detach);
      li_nodes.forEach(detach);
    },
    m(target, anchor) {
      insert_hydration(target, li, anchor);
      append_hydration(li, code);
      append_hydration(code, t0);
      append_hydration(code, t1);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t1_value !== (t1_value = /*path*/
      ctx2[3] + ""))
        set_data(t1, t1_value);
    },
    d(detaching) {
      if (detaching) {
        detach(li);
      }
    }
  };
}
function create_if_block_1(ctx) {
  let li;
  let textContent = "...";
  return {
    c() {
      li = element("li");
      li.textContent = textContent;
      this.h();
    },
    l(nodes) {
      li = claim_element(nodes, "LI", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(li) !== "svelte-17d9ayl")
        li.textContent = textContent;
      this.h();
    },
    h() {
      attr(li, "class", "extra svelte-4tf8f");
    },
    m(target, anchor) {
      insert_hydration(target, li, anchor);
    },
    d(detaching) {
      if (detaching) {
        detach(li);
      }
    }
  };
}
function create_fragment(ctx) {
  let ul;
  let if_block = (
    /*value*/
    ctx[0] && create_if_block(ctx)
  );
  return {
    c() {
      ul = element("ul");
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      ul = claim_element(nodes, "UL", { class: true });
      var ul_nodes = children(ul);
      if (if_block)
        if_block.l(ul_nodes);
      ul_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(ul, "class", "svelte-4tf8f");
      toggle_class(
        ul,
        "table",
        /*type*/
        ctx[1] === "table"
      );
      toggle_class(
        ul,
        "gallery",
        /*type*/
        ctx[1] === "gallery"
      );
      toggle_class(
        ul,
        "selected",
        /*selected*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, ul, anchor);
      if (if_block)
        if_block.m(ul, null);
    },
    p(ctx2, [dirty]) {
      if (
        /*value*/
        ctx2[0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          if_block.m(ul, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          ul,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (dirty & /*type*/
      2) {
        toggle_class(
          ul,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (dirty & /*selected*/
      4) {
        toggle_class(
          ul,
          "selected",
          /*selected*/
          ctx2[2]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(ul);
      }
      if (if_block)
        if_block.d();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
  };
  return [value, type, selected];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 0, type: 1, selected: 2 });
  }
}
export {
  Example as default
};
