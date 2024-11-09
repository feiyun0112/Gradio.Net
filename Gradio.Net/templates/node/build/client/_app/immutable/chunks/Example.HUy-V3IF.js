import { SvelteComponent, init, safe_not_equal, empty, insert_hydration, transition_in, group_outros, transition_out, check_outros, detach, element, text, claim_element, children, claim_text, append_hydration, set_data, noop, binding_callbacks, bind, is_function, create_component, claim_component, attr, toggle_class, mount_component, add_flush_callback, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { V as Video } from "./Video.ML_kOajE.js";
import "./2.BqWhUxOo.js";
function create_if_block(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block_1, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    return 0;
  }
  current_block_type_index = select_block_type();
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
    p(ctx2, dirty) {
      if_block.p(ctx2, dirty);
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
function create_else_block(ctx) {
  let div;
  let t;
  return {
    c() {
      div = element("div");
      t = text(
        /*value*/
        ctx[2]
      );
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {});
      var div_nodes = children(div);
      t = claim_text(
        div_nodes,
        /*value*/
        ctx[2]
      );
      div_nodes.forEach(detach);
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      4)
        set_data(
          t,
          /*value*/
          ctx2[2]
        );
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_if_block_1(ctx) {
  var _a;
  let div;
  let video_1;
  let updating_node;
  let current;
  function video_1_node_binding(value) {
    ctx[6](value);
  }
  let video_1_props = {
    muted: true,
    playsinline: true,
    src: (
      /*value*/
      (_a = ctx[2]) == null ? void 0 : _a.video.url
    ),
    is_stream: false,
    loop: (
      /*loop*/
      ctx[3]
    )
  };
  if (
    /*video*/
    ctx[4] !== void 0
  ) {
    video_1_props.node = /*video*/
    ctx[4];
  }
  video_1 = new Video({ props: video_1_props });
  binding_callbacks.push(() => bind(video_1, "node", video_1_node_binding));
  video_1.$on(
    "loadeddata",
    /*init*/
    ctx[5]
  );
  video_1.$on("mouseover", function() {
    if (is_function(
      /*video*/
      ctx[4].play.bind(
        /*video*/
        ctx[4]
      )
    ))
      ctx[4].play.bind(
        /*video*/
        ctx[4]
      ).apply(this, arguments);
  });
  video_1.$on("mouseout", function() {
    if (is_function(
      /*video*/
      ctx[4].pause.bind(
        /*video*/
        ctx[4]
      )
    ))
      ctx[4].pause.bind(
        /*video*/
        ctx[4]
      ).apply(this, arguments);
  });
  return {
    c() {
      div = element("div");
      create_component(video_1.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(video_1.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "container svelte-1de9zxs");
      toggle_class(
        div,
        "table",
        /*type*/
        ctx[0] === "table"
      );
      toggle_class(
        div,
        "gallery",
        /*type*/
        ctx[0] === "gallery"
      );
      toggle_class(
        div,
        "selected",
        /*selected*/
        ctx[1]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(video_1, div, null);
      current = true;
    },
    p(new_ctx, dirty) {
      var _a2;
      ctx = new_ctx;
      const video_1_changes = {};
      if (dirty & /*value*/
      4)
        video_1_changes.src = /*value*/
        (_a2 = ctx[2]) == null ? void 0 : _a2.video.url;
      if (dirty & /*loop*/
      8)
        video_1_changes.loop = /*loop*/
        ctx[3];
      if (!updating_node && dirty & /*video*/
      16) {
        updating_node = true;
        video_1_changes.node = /*video*/
        ctx[4];
        add_flush_callback(() => updating_node = false);
      }
      video_1.$set(video_1_changes);
      if (!current || dirty & /*type*/
      1) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx[0] === "table"
        );
      }
      if (!current || dirty & /*type*/
      1) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx[0] === "gallery"
        );
      }
      if (!current || dirty & /*selected*/
      2) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx[1]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(video_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(video_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(video_1);
    }
  };
}
function create_fragment(ctx) {
  let if_block_anchor;
  let current;
  let if_block = (
    /*value*/
    ctx[2] && create_if_block(ctx)
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
    p(ctx2, [dirty]) {
      if (
        /*value*/
        ctx2[2]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*value*/
          4) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
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
function instance($$self, $$props, $$invalidate) {
  let { type } = $$props;
  let { selected = false } = $$props;
  let { value } = $$props;
  let { loop } = $$props;
  let video;
  async function init2() {
    $$invalidate(4, video.muted = true, video);
    $$invalidate(4, video.playsInline = true, video);
    $$invalidate(4, video.controls = false, video);
    video.setAttribute("muted", "");
    await video.play();
    video.pause();
  }
  function video_1_node_binding(value2) {
    video = value2;
    $$invalidate(4, video);
  }
  $$self.$$set = ($$props2) => {
    if ("type" in $$props2)
      $$invalidate(0, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(1, selected = $$props2.selected);
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
    if ("loop" in $$props2)
      $$invalidate(3, loop = $$props2.loop);
  };
  return [type, selected, value, loop, video, init2, video_1_node_binding];
}
class Example extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { type: 0, selected: 1, value: 2, loop: 3 });
  }
}
export {
  Example as default
};
