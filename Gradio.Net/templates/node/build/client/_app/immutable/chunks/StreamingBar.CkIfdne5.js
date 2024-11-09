import { SvelteComponent, init, safe_not_equal, empty, insert_hydration, noop, detach, element, claim_element, children, attr, set_style } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
function create_if_block(ctx) {
  let div;
  let style_animation_duration = `${/*time_limit*/
  ctx[0]}s`;
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
      attr(div, "class", "streaming-bar svelte-roz8lq");
      set_style(div, "animation-duration", style_animation_duration);
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*time_limit*/
      1 && style_animation_duration !== (style_animation_duration = `${/*time_limit*/
      ctx2[0]}s`)) {
        set_style(div, "animation-duration", style_animation_duration);
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
    /*time_limit*/
    ctx[0] && create_if_block(ctx)
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
        /*time_limit*/
        ctx2[0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block(ctx2);
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
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { time_limit } = $$props;
  $$self.$$set = ($$props2) => {
    if ("time_limit" in $$props2)
      $$invalidate(0, time_limit = $$props2.time_limit);
  };
  return [time_limit];
}
class StreamingBar extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { time_limit: 0 });
  }
}
export {
  StreamingBar as S
};
