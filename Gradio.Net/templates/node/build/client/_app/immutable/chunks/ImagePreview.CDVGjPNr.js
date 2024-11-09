import { SvelteComponent, init, safe_not_equal, create_component, space, empty, claim_component, claim_space, mount_component, insert_hydration, group_outros, transition_out, check_outros, transition_in, detach, destroy_component, createEventDispatcher, onMount, bubble, binding_callbacks, element, claim_element, children, attr, toggle_class, append_hydration, listen } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { u as uploadToHuggingFace, I as IconButton } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { S as ShareButton } from "./ShareButton.B3gcBIAK.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { M as Maximize, a as Minimize } from "./Minimize.BNzzPy3I.js";
import { I as Image } from "./Image.CMPoCWop.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { g as get_coordinates_of_clicked_image } from "./utils.avOzX0x9.js";
import { I as Image$1 } from "./Image.eJ_qOnkr.js";
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
/* empty css                                                    */
function create_else_block(ctx) {
  let div1;
  let iconbuttonwrapper;
  let t;
  let button;
  let div0;
  let image;
  let current;
  let mounted;
  let dispose;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  image = new Image$1({
    props: {
      src: (
        /*value*/
        ctx[0].url
      ),
      alt: "",
      loading: "lazy"
    }
  });
  image.$on(
    "load",
    /*load_handler*/
    ctx[15]
  );
  return {
    c() {
      div1 = element("div");
      create_component(iconbuttonwrapper.$$.fragment);
      t = space();
      button = element("button");
      div0 = element("div");
      create_component(image.$$.fragment);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      claim_component(iconbuttonwrapper.$$.fragment, div1_nodes);
      t = claim_space(div1_nodes);
      button = claim_element(div1_nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      div0 = claim_element(button_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      claim_component(image.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      button_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "image-frame svelte-1kpcxni");
      toggle_class(
        div0,
        "selectable",
        /*selectable*/
        ctx[4]
      );
      attr(button, "class", "svelte-1kpcxni");
      attr(div1, "class", "image-container svelte-1kpcxni");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      mount_component(iconbuttonwrapper, div1, null);
      append_hydration(div1, t);
      append_hydration(div1, button);
      append_hydration(button, div0);
      mount_component(image, div0, null);
      ctx[16](div1);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*handle_click*/
          ctx[10]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, i18n, value, show_share_button, show_download_button, is_full_screen, show_fullscreen_button*/
      262633) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      const image_changes = {};
      if (dirty & /*value*/
      1)
        image_changes.src = /*value*/
        ctx2[0].url;
      image.$set(image_changes);
      if (!current || dirty & /*selectable*/
      16) {
        toggle_class(
          div0,
          "selectable",
          /*selectable*/
          ctx2[4]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      destroy_component(iconbuttonwrapper);
      destroy_component(image);
      ctx[16](null);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot] },
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
      if (dirty & /*$$scope*/
      262144) {
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
function create_if_block_4(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Maximize,
      label: (
        /*is_full_screen*/
        ctx[8] ? "Exit full screen" : "View in full screen"
      )
    }
  });
  iconbutton.$on(
    "click",
    /*toggle_full_screen*/
    ctx[11]
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
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*is_full_screen*/
      256)
        iconbutton_changes.label = /*is_full_screen*/
        ctx2[8] ? "Exit full screen" : "View in full screen";
      iconbutton.$set(iconbutton_changes);
    },
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
function create_if_block_3(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Minimize,
      label: (
        /*is_full_screen*/
        ctx[8] ? "Exit full screen" : "View in full screen"
      )
    }
  });
  iconbutton.$on(
    "click",
    /*toggle_full_screen*/
    ctx[11]
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
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*is_full_screen*/
      256)
        iconbutton_changes.label = /*is_full_screen*/
        ctx2[8] ? "Exit full screen" : "View in full screen";
      iconbutton.$set(iconbutton_changes);
    },
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
  let downloadlink;
  let current;
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*value*/
        ctx[0].url
      ),
      download: (
        /*value*/
        ctx[0].orig_name || "image"
      ),
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(downloadlink.$$.fragment);
    },
    l(nodes) {
      claim_component(downloadlink.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(downloadlink, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const downloadlink_changes = {};
      if (dirty & /*value*/
      1)
        downloadlink_changes.href = /*value*/
        ctx2[0].url;
      if (dirty & /*value*/
      1)
        downloadlink_changes.download = /*value*/
        ctx2[0].orig_name || "image";
      if (dirty & /*$$scope, i18n*/
      262208) {
        downloadlink_changes.$$scope = { dirty, ctx: ctx2 };
      }
      downloadlink.$set(downloadlink_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(downloadlink.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(downloadlink.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(downloadlink, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Download,
      label: (
        /*i18n*/
        ctx[6]("common.download")
      )
    }
  });
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
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*i18n*/
      64)
        iconbutton_changes.label = /*i18n*/
        ctx2[6]("common.download");
      iconbutton.$set(iconbutton_changes);
    },
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
function create_if_block_1(ctx) {
  let sharebutton;
  let current;
  sharebutton = new ShareButton({
    props: {
      i18n: (
        /*i18n*/
        ctx[6]
      ),
      formatter: (
        /*func*/
        ctx[12]
      ),
      value: (
        /*value*/
        ctx[0]
      )
    }
  });
  sharebutton.$on(
    "share",
    /*share_handler*/
    ctx[13]
  );
  sharebutton.$on(
    "error",
    /*error_handler*/
    ctx[14]
  );
  return {
    c() {
      create_component(sharebutton.$$.fragment);
    },
    l(nodes) {
      claim_component(sharebutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(sharebutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const sharebutton_changes = {};
      if (dirty & /*i18n*/
      64)
        sharebutton_changes.i18n = /*i18n*/
        ctx2[6];
      if (dirty & /*value*/
      1)
        sharebutton_changes.value = /*value*/
        ctx2[0];
      sharebutton.$set(sharebutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(sharebutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(sharebutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(sharebutton, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let t0;
  let t1;
  let t2;
  let if_block3_anchor;
  let current;
  let if_block0 = !/*is_full_screen*/
  ctx[8] && /*show_fullscreen_button*/
  ctx[7] && create_if_block_4(ctx);
  let if_block1 = (
    /*is_full_screen*/
    ctx[8] && /*show_fullscreen_button*/
    ctx[7] && create_if_block_3(ctx)
  );
  let if_block2 = (
    /*show_download_button*/
    ctx[3] && create_if_block_2(ctx)
  );
  let if_block3 = (
    /*show_share_button*/
    ctx[5] && create_if_block_1(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      if (if_block1)
        if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      t2 = space();
      if (if_block3)
        if_block3.c();
      if_block3_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      t1 = claim_space(nodes);
      if (if_block2)
        if_block2.l(nodes);
      t2 = claim_space(nodes);
      if (if_block3)
        if_block3.l(nodes);
      if_block3_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, t1, anchor);
      if (if_block2)
        if_block2.m(target, anchor);
      insert_hydration(target, t2, anchor);
      if (if_block3)
        if_block3.m(target, anchor);
      insert_hydration(target, if_block3_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (!/*is_full_screen*/
      ctx2[8] && /*show_fullscreen_button*/
      ctx2[7]) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*is_full_screen, show_fullscreen_button*/
          384) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t0.parentNode, t0);
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
        ctx2[8] && /*show_fullscreen_button*/
        ctx2[7]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*is_full_screen, show_fullscreen_button*/
          384) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_3(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(t1.parentNode, t1);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (
        /*show_download_button*/
        ctx2[3]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty & /*show_download_button*/
          8) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block_2(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(t2.parentNode, t2);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
      if (
        /*show_share_button*/
        ctx2[5]
      ) {
        if (if_block3) {
          if_block3.p(ctx2, dirty);
          if (dirty & /*show_share_button*/
          32) {
            transition_in(if_block3, 1);
          }
        } else {
          if_block3 = create_if_block_1(ctx2);
          if_block3.c();
          transition_in(if_block3, 1);
          if_block3.m(if_block3_anchor.parentNode, if_block3_anchor);
        }
      } else if (if_block3) {
        group_outros();
        transition_out(if_block3, 1, 1, () => {
          if_block3 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(if_block2);
      transition_in(if_block3);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      transition_out(if_block3);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(t2);
        detach(if_block3_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
      if (if_block2)
        if_block2.d(detaching);
      if (if_block3)
        if_block3.d(detaching);
    }
  };
}
function create_default_slot(ctx) {
  let imageicon;
  let current;
  imageicon = new Image({});
  return {
    c() {
      create_component(imageicon.$$.fragment);
    },
    l(nodes) {
      claim_component(imageicon.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(imageicon, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(imageicon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(imageicon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(imageicon, detaching);
    }
  };
}
function create_fragment(ctx) {
  let blocklabel;
  let t;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[2]
      ),
      Icon: Image,
      label: !/*show_label*/
      ctx[2] ? "" : (
        /*label*/
        ctx[1] || /*i18n*/
        ctx[6]("image.image")
      )
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] === null || !/*value*/
      ctx2[0].url
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      4)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[2];
      if (dirty & /*show_label, label, i18n*/
      70)
        blocklabel_changes.label = !/*show_label*/
        ctx2[2] ? "" : (
          /*label*/
          ctx2[1] || /*i18n*/
          ctx2[6]("image.image")
        );
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
        if_block.m(if_block_anchor.parentNode, if_block_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { label = void 0 } = $$props;
  let { show_label } = $$props;
  let { show_download_button = true } = $$props;
  let { selectable = false } = $$props;
  let { show_share_button = false } = $$props;
  let { i18n } = $$props;
  let { show_fullscreen_button = true } = $$props;
  const dispatch = createEventDispatcher();
  const handle_click = (evt) => {
    let coordinates = get_coordinates_of_clicked_image(evt);
    if (coordinates) {
      dispatch("select", { index: coordinates, value: null });
    }
  };
  let is_full_screen = false;
  let image_container;
  onMount(() => {
    document.addEventListener("fullscreenchange", () => {
      $$invalidate(8, is_full_screen = !!document.fullscreenElement);
    });
  });
  const toggle_full_screen = async () => {
    if (!is_full_screen) {
      await image_container.requestFullscreen();
    } else {
      await document.exitFullscreen();
      $$invalidate(8, is_full_screen = !is_full_screen);
    }
  };
  const func = async (value2) => {
    if (!value2)
      return "";
    let url = await uploadToHuggingFace(value2);
    return `<img src="${url}" />`;
  };
  function share_handler(event) {
    bubble.call(this, $$self, event);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      image_container = $$value;
      $$invalidate(9, image_container);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("show_download_button" in $$props2)
      $$invalidate(3, show_download_button = $$props2.show_download_button);
    if ("selectable" in $$props2)
      $$invalidate(4, selectable = $$props2.selectable);
    if ("show_share_button" in $$props2)
      $$invalidate(5, show_share_button = $$props2.show_share_button);
    if ("i18n" in $$props2)
      $$invalidate(6, i18n = $$props2.i18n);
    if ("show_fullscreen_button" in $$props2)
      $$invalidate(7, show_fullscreen_button = $$props2.show_fullscreen_button);
  };
  return [
    value,
    label,
    show_label,
    show_download_button,
    selectable,
    show_share_button,
    i18n,
    show_fullscreen_button,
    is_full_screen,
    image_container,
    handle_click,
    toggle_full_screen,
    func,
    share_handler,
    error_handler,
    load_handler,
    div1_binding
  ];
}
class ImagePreview extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      label: 1,
      show_label: 2,
      show_download_button: 3,
      selectable: 4,
      show_share_button: 5,
      i18n: 6,
      show_fullscreen_button: 7
    });
  }
}
export {
  ImagePreview as default
};
