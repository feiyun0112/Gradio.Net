import { SvelteComponent, init, safe_not_equal, empty, insert_hydration, noop, detach, text, claim_text, set_data } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_else_block(ctx) {
  let t0;
  let t1;
  let t2;
  return {
    c() {
      t0 = text(
        /*x*/
        ctx[1]
      );
      t1 = text(" x ");
      t2 = text(
        /*y*/
        ctx[2]
      );
    },
    l(nodes) {
      t0 = claim_text(
        nodes,
        /*x*/
        ctx[1]
      );
      t1 = claim_text(nodes, " x ");
      t2 = claim_text(
        nodes,
        /*y*/
        ctx[2]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t0, anchor);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, t2, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*x*/
      2)
        set_data(
          t0,
          /*x*/
          ctx2[1]
        );
      if (dirty & /*y*/
      4)
        set_data(
          t2,
          /*y*/
          ctx2[2]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(t2);
      }
    }
  };
}
function create_if_block(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*title*/
        ctx[0]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*title*/
        ctx[0]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*title*/
      1)
        set_data(
          t,
          /*title*/
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
function create_fragment(ctx) {
  let if_block_anchor;
  function select_block_type(ctx2, dirty) {
    if (
      /*title*/
      ctx2[0]
    )
      return create_if_block;
    return create_else_block;
  }
  let current_block_type = select_block_type(ctx);
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
    p(ctx2, [dirty]) {
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block) {
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
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if_block.d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { title } = $$props;
  let { x } = $$props;
  let { y } = $$props;
  $$self.$$set = ($$props2) => {
    if ("title" in $$props2)
      $$invalidate(0, title = $$props2.title);
    if ("x" in $$props2)
      $$invalidate(1, x = $$props2.x);
    if ("y" in $$props2)
      $$invalidate(2, y = $$props2.y);
  };
  return [title, x, y];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { title: 0, x: 1, y: 2 });
  }
}
export {
  Example as default
};
