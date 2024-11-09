import { SvelteComponent, init, safe_not_equal, add_render_callback, space, empty, claim_space, insert_hydration, listen, transition_in, group_outros, transition_out, check_outros, detach, createEventDispatcher, onMount, globals, create_component, claim_component, mount_component, destroy_component, tick, bubble, binding_callbacks, ensure_array_like, element, claim_element, children, attr, set_style, toggle_class, append_hydration, destroy_each, is_function, run_all, text, claim_text, set_data, noop } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { u as uploadToHuggingFace, I as IconButton, C as Clear } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { S as ShareButton } from "./ShareButton.B3gcBIAK.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { a as Minimize, M as Maximize } from "./Minimize.BNzzPy3I.js";
import { I as Image } from "./Image.CMPoCWop.js";
import { P as Play } from "./Play.wmWinRDD.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
/* empty css                                              */
import { M as ModifyUpload } from "./ModifyUpload.Bygvg79x.js";
import { I as Image$1 } from "./Image.eJ_qOnkr.js";
/* empty css                                                    */
/* empty css                                                    */
import { V as Video } from "./Video.ML_kOajE.js";
var has = Object.prototype.hasOwnProperty;
function find(iter, tar, key) {
  for (key of iter.keys()) {
    if (dequal(key, tar))
      return key;
  }
}
function dequal(foo, bar) {
  var ctor, len, tmp;
  if (foo === bar)
    return true;
  if (foo && bar && (ctor = foo.constructor) === bar.constructor) {
    if (ctor === Date)
      return foo.getTime() === bar.getTime();
    if (ctor === RegExp)
      return foo.toString() === bar.toString();
    if (ctor === Array) {
      if ((len = foo.length) === bar.length) {
        while (len-- && dequal(foo[len], bar[len]))
          ;
      }
      return len === -1;
    }
    if (ctor === Set) {
      if (foo.size !== bar.size) {
        return false;
      }
      for (len of foo) {
        tmp = len;
        if (tmp && typeof tmp === "object") {
          tmp = find(bar, tmp);
          if (!tmp)
            return false;
        }
        if (!bar.has(tmp))
          return false;
      }
      return true;
    }
    if (ctor === Map) {
      if (foo.size !== bar.size) {
        return false;
      }
      for (len of foo) {
        tmp = len[0];
        if (tmp && typeof tmp === "object") {
          tmp = find(bar, tmp);
          if (!tmp)
            return false;
        }
        if (!dequal(len[1], bar.get(tmp))) {
          return false;
        }
      }
      return true;
    }
    if (ctor === ArrayBuffer) {
      foo = new Uint8Array(foo);
      bar = new Uint8Array(bar);
    } else if (ctor === DataView) {
      if ((len = foo.byteLength) === bar.byteLength) {
        while (len-- && foo.getInt8(len) === bar.getInt8(len))
          ;
      }
      return len === -1;
    }
    if (ArrayBuffer.isView(foo)) {
      if ((len = foo.byteLength) === bar.byteLength) {
        while (len-- && foo[len] === bar[len])
          ;
      }
      return len === -1;
    }
    if (!ctor || typeof foo === "object") {
      len = 0;
      for (ctor in foo) {
        if (has.call(foo, ctor) && ++len && !has.call(bar, ctor))
          return false;
        if (!(ctor in bar) || !dequal(foo[ctor], bar[ctor]))
          return false;
      }
      return Object.keys(bar).length === len;
    }
  }
  return foo !== foo && bar !== bar;
}
async function format_gallery_for_sharing(value) {
  if (!value)
    return "";
  let urls = await Promise.all(
    value.map(async ([image, _]) => {
      if (image === null || !image.url)
        return "";
      return await uploadToHuggingFace(image.url);
    })
  );
  return `<div style="display: flex; flex-wrap: wrap; gap: 16px">${urls.map((url) => `<img src="${url}" style="height: 400px" />`).join("")}</div>`;
}
const { window: window_1 } = globals;
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[47] = list[i];
  child_ctx[49] = i;
  return child_ctx;
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[50] = list[i];
  child_ctx[51] = list;
  child_ctx[49] = i;
  return child_ctx;
}
function create_if_block_13(ctx) {
  let blocklabel;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[2]
      ),
      Icon: Image,
      label: (
        /*label*/
        ctx[3] || "Gallery"
      )
    }
  });
  return {
    c() {
      create_component(blocklabel.$$.fragment);
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      4)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[2];
      if (dirty[0] & /*label*/
      8)
        blocklabel_changes.label = /*label*/
        ctx2[3] || "Gallery";
      blocklabel.$set(blocklabel_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(blocklabel, detaching);
    }
  };
}
function create_else_block(ctx) {
  let div2;
  let t0;
  let div1;
  let div0;
  let t1;
  let current;
  let if_block0 = (
    /*selected_media*/
    ctx[21] && /*allow_preview*/
    ctx[7] && create_if_block_4(ctx)
  );
  let if_block1 = (
    /*interactive*/
    ctx[12] && create_if_block_3(ctx)
  );
  let each_value = ensure_array_like(
    /*resolved_value*/
    ctx[15]
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
      div2 = element("div");
      if (if_block0)
        if_block0.c();
      t0 = space();
      div1 = element("div");
      div0 = element("div");
      if (if_block1)
        if_block1.c();
      t1 = space();
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div2 = claim_element(nodes, "DIV", { class: true });
      var div2_nodes = children(div2);
      if (if_block0)
        if_block0.l(div2_nodes);
      t0 = claim_space(div2_nodes);
      div1 = claim_element(div2_nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true, style: true });
      var div0_nodes = children(div0);
      if (if_block1)
        if_block1.l(div0_nodes);
      t1 = claim_space(div0_nodes);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div0_nodes);
      }
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      div2_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "grid-container svelte-17j4qub");
      set_style(
        div0,
        "--grid-cols",
        /*columns*/
        ctx[4]
      );
      set_style(
        div0,
        "--grid-rows",
        /*rows*/
        ctx[5]
      );
      set_style(
        div0,
        "--object-fit",
        /*object_fit*/
        ctx[8]
      );
      set_style(
        div0,
        "height",
        /*height*/
        ctx[6]
      );
      toggle_class(
        div0,
        "pt-6",
        /*show_label*/
        ctx[2]
      );
      attr(div1, "class", "grid-wrap svelte-17j4qub");
      toggle_class(
        div1,
        "minimal",
        /*mode*/
        ctx[13] === "minimal"
      );
      toggle_class(
        div1,
        "fixed-height",
        /*mode*/
        ctx[13] !== "minimal" && (!/*height*/
        ctx[6] || /*height*/
        ctx[6] == "auto")
      );
      toggle_class(
        div1,
        "hidden",
        /*is_full_screen*/
        ctx[16]
      );
      attr(div2, "class", "gallery-container");
    },
    m(target, anchor) {
      insert_hydration(target, div2, anchor);
      if (if_block0)
        if_block0.m(div2, null);
      append_hydration(div2, t0);
      append_hydration(div2, div1);
      append_hydration(div1, div0);
      if (if_block1)
        if_block1.m(div0, null);
      append_hydration(div0, t1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div0, null);
        }
      }
      ctx[42](div2);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*selected_media*/
        ctx2[21] && /*allow_preview*/
        ctx2[7]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*selected_media, allow_preview*/
          2097280) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div2, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*interactive*/
        ctx2[12]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*interactive*/
          4096) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_3(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div0, t1);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (dirty[0] & /*resolved_value, selected_index*/
      32770) {
        each_value = ensure_array_like(
          /*resolved_value*/
          ctx2[15]
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
            each_blocks[i].m(div0, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if (!current || dirty[0] & /*columns*/
      16) {
        set_style(
          div0,
          "--grid-cols",
          /*columns*/
          ctx2[4]
        );
      }
      if (!current || dirty[0] & /*rows*/
      32) {
        set_style(
          div0,
          "--grid-rows",
          /*rows*/
          ctx2[5]
        );
      }
      if (!current || dirty[0] & /*object_fit*/
      256) {
        set_style(
          div0,
          "--object-fit",
          /*object_fit*/
          ctx2[8]
        );
      }
      if (!current || dirty[0] & /*height*/
      64) {
        set_style(
          div0,
          "height",
          /*height*/
          ctx2[6]
        );
      }
      if (!current || dirty[0] & /*show_label*/
      4) {
        toggle_class(
          div0,
          "pt-6",
          /*show_label*/
          ctx2[2]
        );
      }
      if (!current || dirty[0] & /*mode*/
      8192) {
        toggle_class(
          div1,
          "minimal",
          /*mode*/
          ctx2[13] === "minimal"
        );
      }
      if (!current || dirty[0] & /*mode, height*/
      8256) {
        toggle_class(
          div1,
          "fixed-height",
          /*mode*/
          ctx2[13] !== "minimal" && (!/*height*/
          ctx2[6] || /*height*/
          ctx2[6] == "auto")
        );
      }
      if (!current || dirty[0] & /*is_full_screen*/
      65536) {
        toggle_class(
          div1,
          "hidden",
          /*is_full_screen*/
          ctx2[16]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div2);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      destroy_each(each_blocks, detaching);
      ctx[42](null);
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
      if (dirty[1] & /*$$scope*/
      2097152) {
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
  var _a;
  let button1;
  let iconbuttonwrapper;
  let t0;
  let button0;
  let current_block_type_index;
  let if_block0;
  let t1;
  let t2;
  let div;
  let current;
  let mounted;
  let dispose;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  const if_block_creators = [create_if_block_7, create_else_block_3];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if ("image" in /*selected_media*/
    ctx2[21])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block1 = (
    /*selected_media*/
    ((_a = ctx[21]) == null ? void 0 : _a.caption) && create_if_block_6(ctx)
  );
  let each_value_1 = ensure_array_like(
    /*resolved_value*/
    ctx[15]
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
      button1 = element("button");
      create_component(iconbuttonwrapper.$$.fragment);
      t0 = space();
      button0 = element("button");
      if_block0.c();
      t1 = space();
      if (if_block1)
        if_block1.c();
      t2 = space();
      div = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      button1 = claim_element(nodes, "BUTTON", { class: true });
      var button1_nodes = children(button1);
      claim_component(iconbuttonwrapper.$$.fragment, button1_nodes);
      t0 = claim_space(button1_nodes);
      button0 = claim_element(button1_nodes, "BUTTON", {
        class: true,
        style: true,
        "aria-label": true
      });
      var button0_nodes = children(button0);
      if_block0.l(button0_nodes);
      button0_nodes.forEach(detach);
      t1 = claim_space(button1_nodes);
      if (if_block1)
        if_block1.l(button1_nodes);
      t2 = claim_space(button1_nodes);
      div = claim_element(button1_nodes, "DIV", { class: true, "data-testid": true });
      var div_nodes = children(div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_nodes);
      }
      div_nodes.forEach(detach);
      button1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button0, "class", "media-button svelte-17j4qub");
      set_style(button0, "height", "calc(100% - " + /*selected_media*/
      (ctx[21].caption ? "80px" : "60px") + ")");
      attr(button0, "aria-label", "detailed view of selected image");
      attr(div, "class", "thumbnails scroll-hide svelte-17j4qub");
      attr(div, "data-testid", "container_el");
      attr(button1, "class", "preview svelte-17j4qub");
      toggle_class(
        button1,
        "minimal",
        /*mode*/
        ctx[13] === "minimal"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button1, anchor);
      mount_component(iconbuttonwrapper, button1, null);
      append_hydration(button1, t0);
      append_hydration(button1, button0);
      if_blocks[current_block_type_index].m(button0, null);
      append_hydration(button1, t1);
      if (if_block1)
        if_block1.m(button1, null);
      append_hydration(button1, t2);
      append_hydration(button1, div);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div, null);
        }
      }
      ctx[39](div);
      current = true;
      if (!mounted) {
        dispose = [
          listen(button0, "click", function() {
            if (is_function("image" in /*selected_media*/
            ctx[21] ? (
              /*click_handler_2*/
              ctx[36]
            ) : null))
              ("image" in /*selected_media*/
              ctx[21] ? (
                /*click_handler_2*/
                ctx[36]
              ) : null).apply(this, arguments);
          }),
          listen(
            button1,
            "keydown",
            /*on_keydown*/
            ctx[23]
          )
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      var _a2;
      ctx = new_ctx;
      const iconbuttonwrapper_changes = {};
      if (dirty[0] & /*selected_index, is_full_screen, i18n, resolved_value, show_share_button, show_fullscreen_button, selected_media, show_download_button*/
      2215426 | dirty[1] & /*$$scope*/
      2097152) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
          if_block0.c();
        } else {
          if_block0.p(ctx, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(button0, null);
      }
      if (!current || dirty[0] & /*selected_media*/
      2097152) {
        set_style(button0, "height", "calc(100% - " + /*selected_media*/
        (ctx[21].caption ? "80px" : "60px") + ")");
      }
      if (
        /*selected_media*/
        (_a2 = ctx[21]) == null ? void 0 : _a2.caption
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
        } else {
          if_block1 = create_if_block_6(ctx);
          if_block1.c();
          if_block1.m(button1, t2);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
      if (dirty[0] & /*resolved_value, el, selected_index, mode*/
      303106) {
        each_value_1 = ensure_array_like(
          /*resolved_value*/
          ctx[15]
        );
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx, each_value_1, i);
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
      if (!current || dirty[0] & /*mode*/
      8192) {
        toggle_class(
          button1,
          "minimal",
          /*mode*/
          ctx[13] === "minimal"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(if_block0);
      for (let i = 0; i < each_value_1.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(if_block0);
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button1);
      }
      destroy_component(iconbuttonwrapper);
      if_blocks[current_block_type_index].d();
      if (if_block1)
        if_block1.d();
      destroy_each(each_blocks, detaching);
      ctx[39](null);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_12(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Download,
      label: (
        /*i18n*/
        ctx[11]("common.download")
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[32]
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
      if (dirty[0] & /*i18n*/
      2048)
        iconbutton_changes.label = /*i18n*/
        ctx2[11]("common.download");
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
function create_if_block_11(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: (
        /*is_full_screen*/
        ctx[16] ? Minimize : Maximize
      ),
      label: (
        /*is_full_screen*/
        ctx[16] ? "Exit full screen" : "View in full screen"
      )
    }
  });
  iconbutton.$on(
    "click",
    /*toggle_full_screen*/
    ctx[25]
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
      if (dirty[0] & /*is_full_screen*/
      65536)
        iconbutton_changes.Icon = /*is_full_screen*/
        ctx2[16] ? Minimize : Maximize;
      if (dirty[0] & /*is_full_screen*/
      65536)
        iconbutton_changes.label = /*is_full_screen*/
        ctx2[16] ? "Exit full screen" : "View in full screen";
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
function create_if_block_10(ctx) {
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
    ctx[25]
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
function create_if_block_9(ctx) {
  let div;
  let sharebutton;
  let current;
  sharebutton = new ShareButton({
    props: {
      i18n: (
        /*i18n*/
        ctx[11]
      ),
      value: (
        /*resolved_value*/
        ctx[15]
      ),
      formatter: format_gallery_for_sharing
    }
  });
  sharebutton.$on(
    "share",
    /*share_handler*/
    ctx[33]
  );
  sharebutton.$on(
    "error",
    /*error_handler*/
    ctx[34]
  );
  return {
    c() {
      div = element("div");
      create_component(sharebutton.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(sharebutton.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "icon-button svelte-17j4qub");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(sharebutton, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const sharebutton_changes = {};
      if (dirty[0] & /*i18n*/
      2048)
        sharebutton_changes.i18n = /*i18n*/
        ctx2[11];
      if (dirty[0] & /*resolved_value*/
      32768)
        sharebutton_changes.value = /*resolved_value*/
        ctx2[15];
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
      if (detaching) {
        detach(div);
      }
      destroy_component(sharebutton);
    }
  };
}
function create_if_block_8(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({ props: { Icon: Clear, label: "Close" } });
  iconbutton.$on(
    "click",
    /*click_handler_1*/
    ctx[35]
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
function create_default_slot_1(ctx) {
  let t0;
  let t1;
  let t2;
  let t3;
  let if_block4_anchor;
  let current;
  let if_block0 = (
    /*show_download_button*/
    ctx[10] && create_if_block_12(ctx)
  );
  let if_block1 = (
    /*show_fullscreen_button*/
    ctx[14] && !/*is_full_screen*/
    ctx[16] && create_if_block_11(ctx)
  );
  let if_block2 = (
    /*show_fullscreen_button*/
    ctx[14] && /*is_full_screen*/
    ctx[16] && create_if_block_10(ctx)
  );
  let if_block3 = (
    /*show_share_button*/
    ctx[9] && create_if_block_9(ctx)
  );
  let if_block4 = !/*is_full_screen*/
  ctx[16] && create_if_block_8(ctx);
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
      t3 = space();
      if (if_block4)
        if_block4.c();
      if_block4_anchor = empty();
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
      t3 = claim_space(nodes);
      if (if_block4)
        if_block4.l(nodes);
      if_block4_anchor = empty();
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
      insert_hydration(target, t3, anchor);
      if (if_block4)
        if_block4.m(target, anchor);
      insert_hydration(target, if_block4_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_download_button*/
        ctx2[10]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*show_download_button*/
          1024) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_12(ctx2);
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
        /*show_fullscreen_button*/
        ctx2[14] && !/*is_full_screen*/
        ctx2[16]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*show_fullscreen_button, is_full_screen*/
          81920) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_11(ctx2);
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
        /*show_fullscreen_button*/
        ctx2[14] && /*is_full_screen*/
        ctx2[16]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*show_fullscreen_button, is_full_screen*/
          81920) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block_10(ctx2);
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
        ctx2[9]
      ) {
        if (if_block3) {
          if_block3.p(ctx2, dirty);
          if (dirty[0] & /*show_share_button*/
          512) {
            transition_in(if_block3, 1);
          }
        } else {
          if_block3 = create_if_block_9(ctx2);
          if_block3.c();
          transition_in(if_block3, 1);
          if_block3.m(t3.parentNode, t3);
        }
      } else if (if_block3) {
        group_outros();
        transition_out(if_block3, 1, 1, () => {
          if_block3 = null;
        });
        check_outros();
      }
      if (!/*is_full_screen*/
      ctx2[16]) {
        if (if_block4) {
          if_block4.p(ctx2, dirty);
          if (dirty[0] & /*is_full_screen*/
          65536) {
            transition_in(if_block4, 1);
          }
        } else {
          if_block4 = create_if_block_8(ctx2);
          if_block4.c();
          transition_in(if_block4, 1);
          if_block4.m(if_block4_anchor.parentNode, if_block4_anchor);
        }
      } else if (if_block4) {
        group_outros();
        transition_out(if_block4, 1, 1, () => {
          if_block4 = null;
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
      transition_in(if_block4);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      transition_out(if_block3);
      transition_out(if_block4);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(t2);
        detach(t3);
        detach(if_block4_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
      if (if_block2)
        if_block2.d(detaching);
      if (if_block3)
        if_block3.d(detaching);
      if (if_block4)
        if_block4.d(detaching);
    }
  };
}
function create_else_block_3(ctx) {
  let video;
  let current;
  video = new Video({
    props: {
      src: (
        /*selected_media*/
        ctx[21].video.url
      ),
      "data-testid": "detailed-video",
      alt: (
        /*selected_media*/
        ctx[21].caption || ""
      ),
      loading: "lazy",
      loop: false,
      is_stream: false,
      muted: false,
      controls: true
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
      if (dirty[0] & /*selected_media*/
      2097152)
        video_changes.src = /*selected_media*/
        ctx2[21].video.url;
      if (dirty[0] & /*selected_media*/
      2097152)
        video_changes.alt = /*selected_media*/
        ctx2[21].caption || "";
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
function create_if_block_7(ctx) {
  let image;
  let current;
  image = new Image$1({
    props: {
      "data-testid": "detailed-image",
      src: (
        /*selected_media*/
        ctx[21].image.url
      ),
      alt: (
        /*selected_media*/
        ctx[21].caption || ""
      ),
      title: (
        /*selected_media*/
        ctx[21].caption || null
      ),
      class: (
        /*selected_media*/
        ctx[21].caption && "with-caption"
      ),
      loading: "lazy"
    }
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
      if (dirty[0] & /*selected_media*/
      2097152)
        image_changes.src = /*selected_media*/
        ctx2[21].image.url;
      if (dirty[0] & /*selected_media*/
      2097152)
        image_changes.alt = /*selected_media*/
        ctx2[21].caption || "";
      if (dirty[0] & /*selected_media*/
      2097152)
        image_changes.title = /*selected_media*/
        ctx2[21].caption || null;
      if (dirty[0] & /*selected_media*/
      2097152)
        image_changes.class = /*selected_media*/
        ctx2[21].caption && "with-caption";
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
function create_if_block_6(ctx) {
  let caption;
  let t_value = (
    /*selected_media*/
    ctx[21].caption + ""
  );
  let t;
  return {
    c() {
      caption = element("caption");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      caption = claim_element(nodes, "CAPTION", { class: true });
      var caption_nodes = children(caption);
      t = claim_text(caption_nodes, t_value);
      caption_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(caption, "class", "caption svelte-17j4qub");
    },
    m(target, anchor) {
      insert_hydration(target, caption, anchor);
      append_hydration(caption, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*selected_media*/
      2097152 && t_value !== (t_value = /*selected_media*/
      ctx2[21].caption + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(caption);
      }
    }
  };
}
function create_else_block_2(ctx) {
  let play;
  let t;
  let video;
  let current;
  play = new Play({});
  video = new Video({
    props: {
      src: (
        /*media*/
        ctx[50].video.url
      ),
      title: (
        /*media*/
        ctx[50].caption || null
      ),
      is_stream: false,
      "data-testid": "thumbnail " + /*i*/
      (ctx[49] + 1),
      alt: "",
      loading: "lazy",
      loop: false
    }
  });
  return {
    c() {
      create_component(play.$$.fragment);
      t = space();
      create_component(video.$$.fragment);
    },
    l(nodes) {
      claim_component(play.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(video.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(play, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(video, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const video_changes = {};
      if (dirty[0] & /*resolved_value*/
      32768)
        video_changes.src = /*media*/
        ctx2[50].video.url;
      if (dirty[0] & /*resolved_value*/
      32768)
        video_changes.title = /*media*/
        ctx2[50].caption || null;
      video.$set(video_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(play.$$.fragment, local);
      transition_in(video.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(play.$$.fragment, local);
      transition_out(video.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(play, detaching);
      destroy_component(video, detaching);
    }
  };
}
function create_if_block_5(ctx) {
  let image;
  let current;
  image = new Image$1({
    props: {
      src: (
        /*media*/
        ctx[50].image.url
      ),
      title: (
        /*media*/
        ctx[50].caption || null
      ),
      "data-testid": "thumbnail " + /*i*/
      (ctx[49] + 1),
      alt: "",
      loading: "lazy"
    }
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
      if (dirty[0] & /*resolved_value*/
      32768)
        image_changes.src = /*media*/
        ctx2[50].image.url;
      if (dirty[0] & /*resolved_value*/
      32768)
        image_changes.title = /*media*/
        ctx2[50].caption || null;
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
function create_each_block_1(ctx) {
  let button;
  let current_block_type_index;
  let if_block;
  let t;
  let button_aria_label_value;
  let i = (
    /*i*/
    ctx[49]
  );
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_5, create_else_block_2];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    if ("image" in /*media*/
    ctx2[50])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_2(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  const assign_button = () => (
    /*button_binding*/
    ctx[37](button, i)
  );
  const unassign_button = () => (
    /*button_binding*/
    ctx[37](null, i)
  );
  function click_handler_3() {
    return (
      /*click_handler_3*/
      ctx[38](
        /*i*/
        ctx[49]
      )
    );
  }
  return {
    c() {
      button = element("button");
      if_block.c();
      t = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      if_block.l(button_nodes);
      t = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "thumbnail-item thumbnail-small svelte-17j4qub");
      attr(button, "aria-label", button_aria_label_value = "Thumbnail " + /*i*/
      (ctx[49] + 1) + " of " + /*resolved_value*/
      ctx[15].length);
      toggle_class(
        button,
        "selected",
        /*selected_index*/
        ctx[1] === /*i*/
        ctx[49] && /*mode*/
        ctx[13] !== "minimal"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if_blocks[current_block_type_index].m(button, null);
      append_hydration(button, t);
      assign_button();
      current = true;
      if (!mounted) {
        dispose = listen(button, "click", click_handler_3);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_2(ctx);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
          if_block.c();
        } else {
          if_block.p(ctx, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(button, t);
      }
      if (!current || dirty[0] & /*resolved_value*/
      32768 && button_aria_label_value !== (button_aria_label_value = "Thumbnail " + /*i*/
      (ctx[49] + 1) + " of " + /*resolved_value*/
      ctx[15].length)) {
        attr(button, "aria-label", button_aria_label_value);
      }
      if (i !== /*i*/
      ctx[49]) {
        unassign_button();
        i = /*i*/
        ctx[49];
        assign_button();
      }
      if (!current || dirty[0] & /*selected_index, mode*/
      8194) {
        toggle_class(
          button,
          "selected",
          /*selected_index*/
          ctx[1] === /*i*/
          ctx[49] && /*mode*/
          ctx[13] !== "minimal"
        );
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
      if_blocks[current_block_type_index].d();
      unassign_button();
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_3(ctx) {
  let div;
  let modifyupload;
  let current;
  modifyupload = new ModifyUpload({ props: { i18n: (
    /*i18n*/
    ctx[11]
  ) } });
  modifyupload.$on(
    "clear",
    /*clear_handler*/
    ctx[40]
  );
  return {
    c() {
      div = element("div");
      create_component(modifyupload.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(modifyupload.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "icon-button svelte-17j4qub");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(modifyupload, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const modifyupload_changes = {};
      if (dirty[0] & /*i18n*/
      2048)
        modifyupload_changes.i18n = /*i18n*/
        ctx2[11];
      modifyupload.$set(modifyupload_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(modifyupload.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(modifyupload.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(modifyupload);
    }
  };
}
function create_else_block_1(ctx) {
  let play;
  let t;
  let video;
  let current;
  play = new Play({});
  video = new Video({
    props: {
      src: (
        /*entry*/
        ctx[47].video.url
      ),
      title: (
        /*entry*/
        ctx[47].caption || null
      ),
      is_stream: false,
      "data-testid": "thumbnail " + /*i*/
      (ctx[49] + 1),
      alt: "",
      loading: "lazy",
      loop: false
    }
  });
  return {
    c() {
      create_component(play.$$.fragment);
      t = space();
      create_component(video.$$.fragment);
    },
    l(nodes) {
      claim_component(play.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(video.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(play, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(video, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const video_changes = {};
      if (dirty[0] & /*resolved_value*/
      32768)
        video_changes.src = /*entry*/
        ctx2[47].video.url;
      if (dirty[0] & /*resolved_value*/
      32768)
        video_changes.title = /*entry*/
        ctx2[47].caption || null;
      video.$set(video_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(play.$$.fragment, local);
      transition_in(video.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(play.$$.fragment, local);
      transition_out(video.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(play, detaching);
      destroy_component(video, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let image;
  let current;
  image = new Image$1({
    props: {
      alt: (
        /*entry*/
        ctx[47].caption || ""
      ),
      src: typeof /*entry*/
      ctx[47].image === "string" ? (
        /*entry*/
        ctx[47].image
      ) : (
        /*entry*/
        ctx[47].image.url
      ),
      loading: "lazy"
    }
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
      if (dirty[0] & /*resolved_value*/
      32768)
        image_changes.alt = /*entry*/
        ctx2[47].caption || "";
      if (dirty[0] & /*resolved_value*/
      32768)
        image_changes.src = typeof /*entry*/
        ctx2[47].image === "string" ? (
          /*entry*/
          ctx2[47].image
        ) : (
          /*entry*/
          ctx2[47].image.url
        );
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
function create_if_block_1(ctx) {
  let div;
  let t_value = (
    /*entry*/
    ctx[47].caption + ""
  );
  let t;
  return {
    c() {
      div = element("div");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      t = claim_text(div_nodes, t_value);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "caption-label svelte-17j4qub");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*resolved_value*/
      32768 && t_value !== (t_value = /*entry*/
      ctx2[47].caption + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_each_block(ctx) {
  let button;
  let current_block_type_index;
  let if_block0;
  let t0;
  let t1;
  let button_aria_label_value;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_2, create_else_block_1];
  const if_blocks = [];
  function select_block_type_3(ctx2, dirty) {
    if ("image" in /*entry*/
    ctx2[47])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_3(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block1 = (
    /*entry*/
    ctx[47].caption && create_if_block_1(ctx)
  );
  function click_handler_4() {
    return (
      /*click_handler_4*/
      ctx[41](
        /*i*/
        ctx[49]
      )
    );
  }
  return {
    c() {
      button = element("button");
      if_block0.c();
      t0 = space();
      if (if_block1)
        if_block1.c();
      t1 = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      if_block0.l(button_nodes);
      t0 = claim_space(button_nodes);
      if (if_block1)
        if_block1.l(button_nodes);
      t1 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "thumbnail-item thumbnail-lg svelte-17j4qub");
      attr(button, "aria-label", button_aria_label_value = "Thumbnail " + /*i*/
      (ctx[49] + 1) + " of " + /*resolved_value*/
      ctx[15].length);
      toggle_class(
        button,
        "selected",
        /*selected_index*/
        ctx[1] === /*i*/
        ctx[49]
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if_blocks[current_block_type_index].m(button, null);
      append_hydration(button, t0);
      if (if_block1)
        if_block1.m(button, null);
      append_hydration(button, t1);
      current = true;
      if (!mounted) {
        dispose = listen(button, "click", click_handler_4);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_3(ctx);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
          if_block0.c();
        } else {
          if_block0.p(ctx, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(button, t0);
      }
      if (
        /*entry*/
        ctx[47].caption
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
        } else {
          if_block1 = create_if_block_1(ctx);
          if_block1.c();
          if_block1.m(button, t1);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
      if (!current || dirty[0] & /*resolved_value*/
      32768 && button_aria_label_value !== (button_aria_label_value = "Thumbnail " + /*i*/
      (ctx[49] + 1) + " of " + /*resolved_value*/
      ctx[15].length)) {
        attr(button, "aria-label", button_aria_label_value);
      }
      if (!current || dirty[0] & /*selected_index*/
      2) {
        toggle_class(
          button,
          "selected",
          /*selected_index*/
          ctx[1] === /*i*/
          ctx[49]
        );
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
        detach(button);
      }
      if_blocks[current_block_type_index].d();
      if (if_block1)
        if_block1.d();
      mounted = false;
      dispose();
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
  let t;
  let current_block_type_index;
  let if_block1;
  let if_block1_anchor;
  let current;
  let mounted;
  let dispose;
  add_render_callback(
    /*onwindowresize*/
    ctx[31]
  );
  let if_block0 = (
    /*show_label*/
    ctx[2] && create_if_block_13(ctx)
  );
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] == null || /*resolved_value*/
      ctx2[15] == null || /*resolved_value*/
      ctx2[15].length === 0
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t = space();
      if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t = claim_space(nodes);
      if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = listen(
          window_1,
          "resize",
          /*onwindowresize*/
          ctx[31]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (
        /*show_label*/
        ctx2[2]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*show_label*/
          4) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_13(ctx2);
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
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
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
      if_blocks[current_block_type_index].d(detaching);
      mounted = false;
      dispose();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let previous;
  let next;
  let selected_media;
  let { show_label = true } = $$props;
  let { label } = $$props;
  let { value = null } = $$props;
  let { columns = [2] } = $$props;
  let { rows = void 0 } = $$props;
  let { height = "auto" } = $$props;
  let { preview } = $$props;
  let { allow_preview = true } = $$props;
  let { object_fit = "cover" } = $$props;
  let { show_share_button = false } = $$props;
  let { show_download_button = false } = $$props;
  let { i18n } = $$props;
  let { selected_index = null } = $$props;
  let { interactive } = $$props;
  let { _fetch } = $$props;
  let { mode = "normal" } = $$props;
  let { show_fullscreen_button = true } = $$props;
  let is_full_screen = false;
  let gallery_container;
  const dispatch = createEventDispatcher();
  let was_reset = true;
  let resolved_value = null;
  let prev_value = value;
  if (selected_index == null && preview && (value == null ? void 0 : value.length)) {
    selected_index = 0;
  }
  let old_selected_index = selected_index;
  function handle_preview_click(event) {
    const element2 = event.target;
    const x = event.offsetX;
    const width = element2.offsetWidth;
    const centerX = width / 2;
    if (x < centerX) {
      $$invalidate(1, selected_index = previous);
    } else {
      $$invalidate(1, selected_index = next);
    }
  }
  function on_keydown(e) {
    switch (e.code) {
      case "Escape":
        e.preventDefault();
        $$invalidate(1, selected_index = null);
        break;
      case "ArrowLeft":
        e.preventDefault();
        $$invalidate(1, selected_index = previous);
        break;
      case "ArrowRight":
        e.preventDefault();
        $$invalidate(1, selected_index = next);
        break;
    }
  }
  let el = [];
  let container_element;
  async function scroll_to_img(index) {
    var _a;
    if (typeof index !== "number")
      return;
    await tick();
    if (el[index] === void 0)
      return;
    (_a = el[index]) == null ? void 0 : _a.focus();
    const { left: container_left, width: container_width } = container_element.getBoundingClientRect();
    const { left, width } = el[index].getBoundingClientRect();
    const relative_left = left - container_left;
    const pos = relative_left + width / 2 - container_width / 2 + container_element.scrollLeft;
    if (container_element && typeof container_element.scrollTo === "function") {
      container_element.scrollTo({
        left: pos < 0 ? 0 : pos,
        behavior: "smooth"
      });
    }
  }
  let window_height = 0;
  async function download(file_url, name) {
    let response;
    try {
      response = await _fetch(file_url);
    } catch (error) {
      if (error instanceof TypeError) {
        window.open(file_url, "_blank", "noreferrer");
        return;
      }
      throw error;
    }
    const blob = await response.blob();
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = name;
    link.click();
    URL.revokeObjectURL(url);
  }
  onMount(() => {
    document.addEventListener("fullscreenchange", () => {
      $$invalidate(16, is_full_screen = !!document.fullscreenElement);
    });
  });
  const toggle_full_screen = async () => {
    if (!is_full_screen) {
      await gallery_container.requestFullscreen();
    } else {
      await document.exitFullscreen();
    }
  };
  function onwindowresize() {
    $$invalidate(20, window_height = window_1.innerHeight);
  }
  const click_handler = () => {
    const image = "image" in selected_media ? selected_media == null ? void 0 : selected_media.image : selected_media == null ? void 0 : selected_media.video;
    if (image == null) {
      return;
    }
    const { url, orig_name } = image;
    if (url) {
      download(url, orig_name ?? "image");
    }
  };
  function share_handler(event) {
    bubble.call(this, $$self, event);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  const click_handler_1 = () => $$invalidate(1, selected_index = null);
  const click_handler_2 = (event) => handle_preview_click(event);
  function button_binding($$value, i) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      el[i] = $$value;
      $$invalidate(18, el);
    });
  }
  const click_handler_3 = (i) => $$invalidate(1, selected_index = i);
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      container_element = $$value;
      $$invalidate(19, container_element);
    });
  }
  const clear_handler = () => $$invalidate(0, value = []);
  const click_handler_4 = (i) => $$invalidate(1, selected_index = i);
  function div2_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      gallery_container = $$value;
      $$invalidate(17, gallery_container);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("label" in $$props2)
      $$invalidate(3, label = $$props2.label);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("columns" in $$props2)
      $$invalidate(4, columns = $$props2.columns);
    if ("rows" in $$props2)
      $$invalidate(5, rows = $$props2.rows);
    if ("height" in $$props2)
      $$invalidate(6, height = $$props2.height);
    if ("preview" in $$props2)
      $$invalidate(26, preview = $$props2.preview);
    if ("allow_preview" in $$props2)
      $$invalidate(7, allow_preview = $$props2.allow_preview);
    if ("object_fit" in $$props2)
      $$invalidate(8, object_fit = $$props2.object_fit);
    if ("show_share_button" in $$props2)
      $$invalidate(9, show_share_button = $$props2.show_share_button);
    if ("show_download_button" in $$props2)
      $$invalidate(10, show_download_button = $$props2.show_download_button);
    if ("i18n" in $$props2)
      $$invalidate(11, i18n = $$props2.i18n);
    if ("selected_index" in $$props2)
      $$invalidate(1, selected_index = $$props2.selected_index);
    if ("interactive" in $$props2)
      $$invalidate(12, interactive = $$props2.interactive);
    if ("_fetch" in $$props2)
      $$invalidate(27, _fetch = $$props2._fetch);
    if ("mode" in $$props2)
      $$invalidate(13, mode = $$props2.mode);
    if ("show_fullscreen_button" in $$props2)
      $$invalidate(14, show_fullscreen_button = $$props2.show_fullscreen_button);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value, was_reset*/
    268435457) {
      $$invalidate(28, was_reset = value == null || value.length === 0 ? true : was_reset);
    }
    if ($$self.$$.dirty[0] & /*value*/
    1) {
      $$invalidate(15, resolved_value = value == null ? null : value.map((data) => {
        if ("video" in data) {
          return { video: data.video, caption: data.caption };
        } else if ("image" in data) {
          return { image: data.image, caption: data.caption };
        }
        return {};
      }));
    }
    if ($$self.$$.dirty[0] & /*prev_value, value, was_reset, preview, selected_index*/
    872415235) {
      if (!dequal(prev_value, value)) {
        if (was_reset) {
          $$invalidate(1, selected_index = preview && (value == null ? void 0 : value.length) ? 0 : null);
          $$invalidate(28, was_reset = false);
        } else {
          $$invalidate(1, selected_index = selected_index != null && value != null && selected_index < value.length ? selected_index : null);
        }
        dispatch("change");
        $$invalidate(29, prev_value = value);
      }
    }
    if ($$self.$$.dirty[0] & /*selected_index, resolved_value*/
    32770) {
      previous = ((selected_index ?? 0) + ((resolved_value == null ? void 0 : resolved_value.length) ?? 0) - 1) % ((resolved_value == null ? void 0 : resolved_value.length) ?? 0);
    }
    if ($$self.$$.dirty[0] & /*selected_index, resolved_value*/
    32770) {
      next = ((selected_index ?? 0) + 1) % ((resolved_value == null ? void 0 : resolved_value.length) ?? 0);
    }
    if ($$self.$$.dirty[0] & /*selected_index, old_selected_index, resolved_value*/
    1073774594) {
      {
        if (selected_index !== old_selected_index) {
          $$invalidate(30, old_selected_index = selected_index);
          if (selected_index !== null) {
            dispatch("select", {
              index: selected_index,
              value: resolved_value == null ? void 0 : resolved_value[selected_index]
            });
          }
        }
      }
    }
    if ($$self.$$.dirty[0] & /*allow_preview, selected_index*/
    130) {
      if (allow_preview) {
        scroll_to_img(selected_index);
      }
    }
    if ($$self.$$.dirty[0] & /*selected_index, resolved_value*/
    32770) {
      $$invalidate(21, selected_media = selected_index != null && resolved_value != null ? resolved_value[selected_index] : null);
    }
  };
  return [
    value,
    selected_index,
    show_label,
    label,
    columns,
    rows,
    height,
    allow_preview,
    object_fit,
    show_share_button,
    show_download_button,
    i18n,
    interactive,
    mode,
    show_fullscreen_button,
    resolved_value,
    is_full_screen,
    gallery_container,
    el,
    container_element,
    window_height,
    selected_media,
    handle_preview_click,
    on_keydown,
    download,
    toggle_full_screen,
    preview,
    _fetch,
    was_reset,
    prev_value,
    old_selected_index,
    onwindowresize,
    click_handler,
    share_handler,
    error_handler,
    click_handler_1,
    click_handler_2,
    button_binding,
    click_handler_3,
    div_binding,
    clear_handler,
    click_handler_4,
    div2_binding
  ];
}
class Gallery extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        show_label: 2,
        label: 3,
        value: 0,
        columns: 4,
        rows: 5,
        height: 6,
        preview: 26,
        allow_preview: 7,
        object_fit: 8,
        show_share_button: 9,
        show_download_button: 10,
        i18n: 11,
        selected_index: 1,
        interactive: 12,
        _fetch: 27,
        mode: 13,
        show_fullscreen_button: 14
      },
      null,
      [-1, -1]
    );
  }
}
export {
  Gallery as default
};
