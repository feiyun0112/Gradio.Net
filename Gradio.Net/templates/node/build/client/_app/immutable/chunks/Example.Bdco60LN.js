import { SvelteComponent, init, safe_not_equal, ensure_array_like, element, text, space, claim_element, children, claim_text, detach, claim_space, attr, add_render_callback, toggle_class, insert_hydration, append_hydration, add_iframe_resize_listener, set_data, transition_in, group_outros, check_outros, transition_out, destroy_each, onMount, empty, binding_callbacks, noop, src_url_equal, create_component, claim_component, mount_component, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { I as Image } from "./Image.eJ_qOnkr.js";
import "./2.BqWhUxOo.js";
/* empty css                                                    */
/* empty css                                                    */
import { V as Video } from "./Video.ML_kOajE.js";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[7] = list[i];
  return child_ctx;
}
function create_else_block(ctx) {
  let t_value = (
    /*file*/
    ctx[7].orig_name + ""
  );
  let t;
  return {
    c() {
      t = text(t_value);
    },
    l(nodes) {
      t = claim_text(nodes, t_value);
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && t_value !== (t_value = /*file*/
      ctx2[7].orig_name + ""))
        set_data(t, t_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_if_block_2(ctx) {
  let audio;
  let audio_src_value;
  return {
    c() {
      audio = element("audio");
      this.h();
    },
    l(nodes) {
      audio = claim_element(nodes, "AUDIO", { src: true });
      children(audio).forEach(detach);
      this.h();
    },
    h() {
      if (!src_url_equal(audio.src, audio_src_value = /*file*/
      ctx[7].url))
        attr(audio, "src", audio_src_value);
      audio.controls = true;
    },
    m(target, anchor) {
      insert_hydration(target, audio, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*value*/
      1 && !src_url_equal(audio.src, audio_src_value = /*file*/
      ctx2[7].url)) {
        attr(audio, "src", audio_src_value);
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(audio);
      }
    }
  };
}
function create_if_block_1(ctx) {
  let video;
  let current;
  video = new Video({
    props: {
      src: (
        /*file*/
        ctx[7].url
      ),
      alt: "",
      loop: true,
      is_stream: false
    }
  });
  return {
    c() {
      create_component(video.$$.fragment);
    },
    l(nodes) {
      claim_component(video.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(video, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const video_changes = {};
      if (dirty & /*value*/
      1)
        video_changes.src = /*file*/
        ctx2[7].url;
      video.$set(video_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(video.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(video.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(video, detaching);
    }
  };
}
function create_if_block(ctx) {
  let image;
  let current;
  image = new Image({
    props: { src: (
      /*file*/
      ctx[7].url
    ), alt: "" }
  });
  return {
    c() {
      create_component(image.$$.fragment);
    },
    l(nodes) {
      claim_component(image.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(image, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const image_changes = {};
      if (dirty & /*value*/
      1)
        image_changes.src = /*file*/
        ctx2[7].url;
      image.$set(image_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(image, detaching);
    }
  };
}
function create_each_block(ctx) {
  let show_if;
  let show_if_1;
  let show_if_2;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_if_block_1, create_if_block_2, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (dirty & /*value*/
    1)
      show_if = null;
    if (dirty & /*value*/
    1)
      show_if_1 = null;
    if (dirty & /*value*/
    1)
      show_if_2 = null;
    if (show_if == null)
      show_if = !!/*file*/
      (ctx2[7].mime_type && /*file*/
      ctx2[7].mime_type.includes("image"));
    if (show_if)
      return 0;
    if (show_if_1 == null)
      show_if_1 = !!/*file*/
      (ctx2[7].mime_type && /*file*/
      ctx2[7].mime_type.includes("video"));
    if (show_if_1)
      return 1;
    if (show_if_2 == null)
      show_if_2 = !!/*file*/
      (ctx2[7].mime_type && /*file*/
      ctx2[7].mime_type.includes("audio"));
    if (show_if_2)
      return 2;
    return 3;
  }
  current_block_type_index = select_block_type(ctx, -1);
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
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2, dirty);
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
function create_fragment(ctx) {
  let div;
  let p;
  let t0_value = (
    /*value*/
    (ctx[0].text ? (
      /*value*/
      ctx[0].text
    ) : "") + ""
  );
  let t0;
  let t1;
  let div_resize_listener;
  let current;
  let each_value = ensure_array_like(
    /*value*/
    ctx[0].files
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      div = element("div");
      p = element("p");
      t0 = text(t0_value);
      t1 = space();
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      p = claim_element(div_nodes, "P", {});
      var p_nodes = children(p);
      t0 = claim_text(p_nodes, t0_value);
      p_nodes.forEach(detach);
      t1 = claim_space(div_nodes);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "container svelte-1cl8bqt");
      add_render_callback(() => (
        /*div_elementresize_handler*/
        ctx[5].call(div)
      ));
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
      toggle_class(
        div,
        "border",
        /*value*/
        ctx[0]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, p);
      append_hydration(p, t0);
      append_hydration(div, t1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      div_resize_listener = add_iframe_resize_listener(
        div,
        /*div_elementresize_handler*/
        ctx[5].bind(div)
      );
      ctx[6](div);
      current = true;
    },
    p(ctx2, [dirty]) {
      if ((!current || dirty & /*value*/
      1) && t0_value !== (t0_value = /*value*/
      (ctx2[0].text ? (
        /*value*/
        ctx2[0].text
      ) : "") + ""))
        set_data(t0, t0_value);
      if (dirty & /*value*/
      1) {
        each_value = ensure_array_like(
          /*value*/
          ctx2[0].files
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
            each_blocks[i].m(div, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "table",
          /*type*/
          ctx2[1] === "table"
        );
      }
      if (!current || dirty & /*type*/
      2) {
        toggle_class(
          div,
          "gallery",
          /*type*/
          ctx2[1] === "gallery"
        );
      }
      if (!current || dirty & /*selected*/
      4) {
        toggle_class(
          div,
          "selected",
          /*selected*/
          ctx2[2]
        );
      }
      if (!current || dirty & /*value*/
      1) {
        toggle_class(
          div,
          "border",
          /*value*/
          ctx2[0]
        );
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
        detach(div);
      }
      destroy_each(each_blocks, detaching);
      div_resize_listener();
      ctx[6](null);
    }
  };
}
function set_styles(element2, el_width) {
  element2.style.setProperty("--local-text-width", `${el_width && el_width < 150 ? el_width : 200}px`);
  element2.style.whiteSpace = "unset";
}
function instance($$self, $$props, $$invalidate) {
  let { value = { text: "", files: [] } } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let size;
  let el;
  onMount(() => {
    set_styles(el, size);
  });
  function div_elementresize_handler() {
    size = this.clientWidth;
    $$invalidate(3, size);
  }
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      el = $$value;
      $$invalidate(4, el);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("type" in $$props2)
      $$invalidate(1, type = $$props2.type);
    if ("selected" in $$props2)
      $$invalidate(2, selected = $$props2.selected);
  };
  return [value, type, selected, size, el, div_elementresize_handler, div_binding];
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
