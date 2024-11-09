import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, onMount, assign, space, element, claim_space, claim_element, children, detach, attr, insert_hydration, get_spread_update, get_spread_object, group_outros, check_outros, binding_callbacks, ensure_array_like, empty, src_url_equal, toggle_class, append_hydration, destroy_each, text, claim_text, set_style, listen, set_data, run_all, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static, I as IconButton } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { M as Maximize, a as Minimize } from "./Minimize.BNzzPy3I.js";
import { I as Image } from "./Image.CMPoCWop.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { r as resolve_wasm_src } from "./file-url.Bs-FMz4v.js";
/* empty css                                                    */
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[32] = list[i];
  child_ctx[34] = i;
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[32] = list[i];
  child_ctx[34] = i;
  return child_ctx;
}
function create_else_block(ctx) {
  var _a;
  let div;
  let iconbuttonwrapper;
  let t0;
  let img;
  let img_src_value;
  let t1;
  let t2;
  let if_block_anchor;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  let each_value_1 = ensure_array_like(
    /*_value*/
    ctx[15] ? (
      /*_value*/
      (_a = ctx[15]) == null ? void 0 : _a.annotations
    ) : []
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
  }
  let if_block = (
    /*show_legend*/
    ctx[6] && /*_value*/
    ctx[15] && create_if_block_1(ctx)
  );
  return {
    c() {
      div = element("div");
      create_component(iconbuttonwrapper.$$.fragment);
      t0 = space();
      img = element("img");
      t1 = space();
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t2 = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(iconbuttonwrapper.$$.fragment, div_nodes);
      t0 = claim_space(div_nodes);
      img = claim_element(div_nodes, "IMG", { class: true, src: true, alt: true });
      t1 = claim_space(div_nodes);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      t2 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(img, "class", "base-image svelte-303fln");
      if (!src_url_equal(img.src, img_src_value = /*_value*/
      ctx[15] ? (
        /*_value*/
        ctx[15].image.url
      ) : null))
        attr(img, "src", img_src_value);
      attr(img, "alt", "the base file that is annotated");
      toggle_class(
        img,
        "fit-height",
        /*height*/
        ctx[7] && !/*is_full_screen*/
        ctx[17]
      );
      attr(div, "class", "image-container svelte-303fln");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(iconbuttonwrapper, div, null);
      append_hydration(div, t0);
      append_hydration(div, img);
      append_hydration(div, t1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      ctx[26](div);
      insert_hydration(target, t2, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const iconbuttonwrapper_changes = {};
      if (dirty[0] & /*is_full_screen, show_fullscreen_button*/
      147456 | dirty[1] & /*$$scope*/
      32) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      if (!current || dirty[0] & /*_value*/
      32768 && !src_url_equal(img.src, img_src_value = /*_value*/
      ctx2[15] ? (
        /*_value*/
        ctx2[15].image.url
      ) : null)) {
        attr(img, "src", img_src_value);
      }
      if (!current || dirty[0] & /*height, is_full_screen*/
      131200) {
        toggle_class(
          img,
          "fit-height",
          /*height*/
          ctx2[7] && !/*is_full_screen*/
          ctx2[17]
        );
      }
      if (dirty[0] & /*label, _value, color_map, is_full_screen, active*/
      229904) {
        each_value_1 = ensure_array_like(
          /*_value*/
          ctx2[15] ? (
            /*_value*/
            (_a2 = ctx2[15]) == null ? void 0 : _a2.annotations
          ) : []
        );
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
          } else {
            each_blocks[i] = create_each_block_1(child_ctx);
            each_blocks[i].c();
            each_blocks[i].m(div, null);
          }
        }
        for (; i < each_blocks.length; i += 1) {
          each_blocks[i].d(1);
        }
        each_blocks.length = each_value_1.length;
      }
      if (
        /*show_legend*/
        ctx2[6] && /*_value*/
        ctx2[15]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_1(ctx2);
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
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t2);
        detach(if_block_anchor);
      }
      destroy_component(iconbuttonwrapper);
      destroy_each(each_blocks, detaching);
      ctx[26](null);
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      size: "large",
      unpadded_box: true,
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(empty_1.$$.fragment);
    },
    l(nodes) {
      claim_component(empty_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(empty_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const empty_1_changes = {};
      if (dirty[1] & /*$$scope*/
      32) {
        empty_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      empty_1.$set(empty_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(empty_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(empty_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(empty_1, detaching);
    }
  };
}
function create_if_block_3(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Maximize,
      label: "View in full screen"
    }
  });
  iconbutton.$on(
    "click",
    /*toggle_full_screen*/
    ctx[19]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Minimize,
      label: "Exit full screen"
    }
  });
  iconbutton.$on(
    "click",
    /*toggle_full_screen*/
    ctx[19]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let t;
  let if_block1_anchor;
  let current;
  let if_block0 = !/*is_full_screen*/
  ctx[17] && /*show_fullscreen_button*/
  ctx[14] && create_if_block_3(ctx);
  let if_block1 = (
    /*is_full_screen*/
    ctx[17] && create_if_block_2(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (!/*is_full_screen*/
      ctx2[17] && /*show_fullscreen_button*/
      ctx2[14]) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*is_full_screen, show_fullscreen_button*/
          147456) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_3(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t.parentNode, t);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*is_full_screen*/
        ctx2[17]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*is_full_screen*/
          131072) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_each_block_1(ctx) {
  let img;
  let img_alt_value;
  let img_src_value;
  let img_style_value;
  return {
    c() {
      img = element("img");
      this.h();
    },
    l(nodes) {
      img = claim_element(nodes, "IMG", {
        alt: true,
        class: true,
        src: true,
        style: true
      });
      this.h();
    },
    h() {
      var _a;
      attr(img, "alt", img_alt_value = "segmentation mask identifying " + /*label*/
      ctx[4] + " within the uploaded file");
      attr(img, "class", "mask fit-height svelte-303fln");
      if (!src_url_equal(img.src, img_src_value = /*ann*/
      ctx[32].image.url))
        attr(img, "src", img_src_value);
      attr(img, "style", img_style_value = /*color_map*/
      ctx[9] && /*ann*/
      ctx[32].label in /*color_map*/
      ctx[9] ? null : `filter: hue-rotate(${Math.round(
        /*i*/
        ctx[34] * 360 / /*_value*/
        ((_a = ctx[15]) == null ? void 0 : _a.annotations.length)
      )}deg);`);
      toggle_class(img, "fit-height", !/*is_full_screen*/
      ctx[17]);
      toggle_class(
        img,
        "active",
        /*active*/
        ctx[16] == /*ann*/
        ctx[32].label
      );
      toggle_class(
        img,
        "inactive",
        /*active*/
        ctx[16] != /*ann*/
        ctx[32].label && /*active*/
        ctx[16] != null
      );
    },
    m(target, anchor) {
      insert_hydration(target, img, anchor);
    },
    p(ctx2, dirty) {
      var _a;
      if (dirty[0] & /*label*/
      16 && img_alt_value !== (img_alt_value = "segmentation mask identifying " + /*label*/
      ctx2[4] + " within the uploaded file")) {
        attr(img, "alt", img_alt_value);
      }
      if (dirty[0] & /*_value*/
      32768 && !src_url_equal(img.src, img_src_value = /*ann*/
      ctx2[32].image.url)) {
        attr(img, "src", img_src_value);
      }
      if (dirty[0] & /*color_map, _value*/
      33280 && img_style_value !== (img_style_value = /*color_map*/
      ctx2[9] && /*ann*/
      ctx2[32].label in /*color_map*/
      ctx2[9] ? null : `filter: hue-rotate(${Math.round(
        /*i*/
        ctx2[34] * 360 / /*_value*/
        ((_a = ctx2[15]) == null ? void 0 : _a.annotations.length)
      )}deg);`)) {
        attr(img, "style", img_style_value);
      }
      if (dirty[0] & /*is_full_screen*/
      131072) {
        toggle_class(img, "fit-height", !/*is_full_screen*/
        ctx2[17]);
      }
      if (dirty[0] & /*active, _value*/
      98304) {
        toggle_class(
          img,
          "active",
          /*active*/
          ctx2[16] == /*ann*/
          ctx2[32].label
        );
      }
      if (dirty[0] & /*active, _value*/
      98304) {
        toggle_class(
          img,
          "inactive",
          /*active*/
          ctx2[16] != /*ann*/
          ctx2[32].label && /*active*/
          ctx2[16] != null
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(img);
      }
    }
  };
}
function create_if_block_1(ctx) {
  let div;
  let each_value = ensure_array_like(
    /*_value*/
    ctx[15].annotations
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
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
      attr(div, "class", "legend svelte-303fln");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*color_map, _value, handle_mouseover, handle_mouseout, handle_click*/
      7373312) {
        each_value = ensure_array_like(
          /*_value*/
          ctx2[15].annotations
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
function create_each_block(ctx) {
  let button;
  let t0_value = (
    /*ann*/
    ctx[32].label + ""
  );
  let t0;
  let t1;
  let mounted;
  let dispose;
  function mouseover_handler() {
    return (
      /*mouseover_handler*/
      ctx[27](
        /*ann*/
        ctx[32]
      )
    );
  }
  function focus_handler() {
    return (
      /*focus_handler*/
      ctx[28](
        /*ann*/
        ctx[32]
      )
    );
  }
  function click_handler() {
    return (
      /*click_handler*/
      ctx[31](
        /*i*/
        ctx[34],
        /*ann*/
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
      button = claim_element(nodes, "BUTTON", { class: true, style: true });
      var button_nodes = children(button);
      t0 = claim_text(button_nodes, t0_value);
      t1 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "legend-item svelte-303fln");
      set_style(
        button,
        "background-color",
        /*color_map*/
        ctx[9] && /*ann*/
        ctx[32].label in /*color_map*/
        ctx[9] ? (
          /*color_map*/
          ctx[9][
            /*ann*/
            ctx[32].label
          ] + "88"
        ) : `hsla(${Math.round(
          /*i*/
          ctx[34] * 360 / /*_value*/
          ctx[15].annotations.length
        )}, 100%, 50%, 0.3)`
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, t0);
      append_hydration(button, t1);
      if (!mounted) {
        dispose = [
          listen(button, "mouseover", mouseover_handler),
          listen(button, "focus", focus_handler),
          listen(
            button,
            "mouseout",
            /*mouseout_handler*/
            ctx[29]
          ),
          listen(
            button,
            "blur",
            /*blur_handler*/
            ctx[30]
          ),
          listen(button, "click", click_handler)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (dirty[0] & /*_value*/
      32768 && t0_value !== (t0_value = /*ann*/
      ctx[32].label + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*color_map, _value*/
      33280) {
        set_style(
          button,
          "background-color",
          /*color_map*/
          ctx[9] && /*ann*/
          ctx[32].label in /*color_map*/
          ctx[9] ? (
            /*color_map*/
            ctx[9][
              /*ann*/
              ctx[32].label
            ] + "88"
          ) : `hsla(${Math.round(
            /*i*/
            ctx[34] * 360 / /*_value*/
            ctx[15].annotations.length
          )}, 100%, 50%, 0.3)`
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_default_slot_1(ctx) {
  let image;
  let current;
  image = new Image({});
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
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let blocklabel;
  let t1;
  let div;
  let current_block_type_index;
  let if_block;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[3].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[3].i18n
    ) },
    /*loading_status*/
    ctx[13]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[5]
      ),
      Icon: Image,
      label: (
        /*label*/
        ctx[4] || /*gradio*/
        ctx[3].i18n("image.image")
      )
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*_value*/
      ctx2[15] == null
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      create_component(blocklabel.$$.fragment);
      t1 = space();
      div = element("div");
      if_block.c();
      this.h();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      claim_component(blocklabel.$$.fragment, nodes);
      t1 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "container svelte-303fln");
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, div, anchor);
      if_blocks[current_block_type_index].m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      8200 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        8 && { autoscroll: (
          /*gradio*/
          ctx2[3].autoscroll
        ) },
        dirty[0] & /*gradio*/
        8 && { i18n: (
          /*gradio*/
          ctx2[3].i18n
        ) },
        dirty[0] & /*loading_status*/
        8192 && get_spread_object(
          /*loading_status*/
          ctx2[13]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      32)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[5];
      if (dirty[0] & /*label, gradio*/
      24)
        blocklabel_changes.label = /*label*/
        ctx2[4] || /*gradio*/
        ctx2[3].i18n("image.image");
      blocklabel.$set(blocklabel_changes);
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
        if_block.m(div, null);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(div);
      }
      destroy_component(statustracker, detaching);
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d();
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
      padding: false,
      height: (
        /*height*/
        ctx[7]
      ),
      width: (
        /*width*/
        ctx[8]
      ),
      allow_overflow: false,
      container: (
        /*container*/
        ctx[10]
      ),
      scale: (
        /*scale*/
        ctx[11]
      ),
      min_width: (
        /*min_width*/
        ctx[12]
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
      4)
        block_changes.visible = /*visible*/
        ctx2[2];
      if (dirty[0] & /*elem_id*/
      1)
        block_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty[0] & /*elem_classes*/
      2)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty[0] & /*height*/
      128)
        block_changes.height = /*height*/
        ctx2[7];
      if (dirty[0] & /*width*/
      256)
        block_changes.width = /*width*/
        ctx2[8];
      if (dirty[0] & /*container*/
      1024)
        block_changes.container = /*container*/
        ctx2[10];
      if (dirty[0] & /*scale*/
      2048)
        block_changes.scale = /*scale*/
        ctx2[11];
      if (dirty[0] & /*min_width*/
      4096)
        block_changes.min_width = /*min_width*/
        ctx2[12];
      if (dirty[0] & /*_value, color_map, show_legend, image_container, label, is_full_screen, active, height, show_fullscreen_button, show_label, gradio, loading_status*/
      516856 | dirty[1] & /*$$scope*/
      32) {
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
  let { value = null } = $$props;
  let old_value = null;
  let _value = null;
  let { gradio } = $$props;
  let { label = gradio.i18n("annotated_image.annotated_image") } = $$props;
  let { show_label = true } = $$props;
  let { show_legend = true } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { color_map } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let active = null;
  let { loading_status } = $$props;
  let { show_fullscreen_button = true } = $$props;
  let is_full_screen = false;
  let image_container;
  onMount(() => {
    document.addEventListener("fullscreenchange", () => {
      $$invalidate(17, is_full_screen = !!document.fullscreenElement);
    });
  });
  const toggle_full_screen = async () => {
    if (!is_full_screen) {
      await image_container.requestFullscreen();
    } else {
      await document.exitFullscreen();
    }
  };
  let latest_promise = null;
  function handle_mouseover(_label) {
    $$invalidate(16, active = _label);
  }
  function handle_mouseout() {
    $$invalidate(16, active = null);
  }
  function handle_click(i, value2) {
    gradio.dispatch("select", { value: label, index: i });
  }
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      image_container = $$value;
      $$invalidate(18, image_container);
    });
  }
  const mouseover_handler = (ann) => handle_mouseover(ann.label);
  const focus_handler = (ann) => handle_mouseover(ann.label);
  const mouseout_handler = () => handle_mouseout();
  const blur_handler = () => handle_mouseout();
  const click_handler = (i, ann) => handle_click(i, ann.label);
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(23, value = $$props2.value);
    if ("gradio" in $$props2)
      $$invalidate(3, gradio = $$props2.gradio);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(5, show_label = $$props2.show_label);
    if ("show_legend" in $$props2)
      $$invalidate(6, show_legend = $$props2.show_legend);
    if ("height" in $$props2)
      $$invalidate(7, height = $$props2.height);
    if ("width" in $$props2)
      $$invalidate(8, width = $$props2.width);
    if ("color_map" in $$props2)
      $$invalidate(9, color_map = $$props2.color_map);
    if ("container" in $$props2)
      $$invalidate(10, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(11, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(12, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(13, loading_status = $$props2.loading_status);
    if ("show_fullscreen_button" in $$props2)
      $$invalidate(14, show_fullscreen_button = $$props2.show_fullscreen_button);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value, old_value, gradio, latest_promise*/
    58720264) {
      {
        if (value !== old_value) {
          $$invalidate(24, old_value = value);
          gradio.dispatch("change");
        }
        if (value) {
          const normalized_value = {
            image: value.image,
            annotations: value.annotations.map((ann) => ({ image: ann.image, label: ann.label }))
          };
          $$invalidate(15, _value = normalized_value);
          const image_url_promise = resolve_wasm_src(normalized_value.image.url);
          const annotation_urls_promise = Promise.all(normalized_value.annotations.map((ann) => resolve_wasm_src(ann.image.url)));
          const current_promise = Promise.all([image_url_promise, annotation_urls_promise]);
          $$invalidate(25, latest_promise = current_promise);
          current_promise.then(([image_url, annotation_urls]) => {
            if (latest_promise !== current_promise) {
              return;
            }
            const async_resolved_value = {
              image: {
                ...normalized_value.image,
                url: image_url ?? void 0
              },
              annotations: normalized_value.annotations.map((ann, i) => ({
                ...ann,
                image: {
                  ...ann.image,
                  url: annotation_urls[i] ?? void 0
                }
              }))
            };
            $$invalidate(15, _value = async_resolved_value);
          });
        } else {
          $$invalidate(15, _value = null);
        }
      }
    }
  };
  return [
    elem_id,
    elem_classes,
    visible,
    gradio,
    label,
    show_label,
    show_legend,
    height,
    width,
    color_map,
    container,
    scale,
    min_width,
    loading_status,
    show_fullscreen_button,
    _value,
    active,
    is_full_screen,
    image_container,
    toggle_full_screen,
    handle_mouseover,
    handle_mouseout,
    handle_click,
    value,
    old_value,
    latest_promise,
    div_binding,
    mouseover_handler,
    focus_handler,
    mouseout_handler,
    blur_handler,
    click_handler
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
        elem_id: 0,
        elem_classes: 1,
        visible: 2,
        value: 23,
        gradio: 3,
        label: 4,
        show_label: 5,
        show_legend: 6,
        height: 7,
        width: 8,
        color_map: 9,
        container: 10,
        scale: 11,
        min_width: 12,
        loading_status: 13,
        show_fullscreen_button: 14
      },
      null,
      [-1, -1]
    );
  }
}
export {
  Index as default
};
