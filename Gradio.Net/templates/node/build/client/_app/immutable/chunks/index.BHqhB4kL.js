import { SvelteComponent, init, safe_not_equal, binding_callbacks, bind, create_component, space, element, claim_component, claim_space, claim_element, children, detach, attr, mount_component, insert_hydration, append_hydration, group_outros, transition_out, check_outros, transition_in, add_flush_callback, destroy_component, createEventDispatcher, bubble, text, claim_text, set_data, noop, empty, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, flush, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
import { B as Block, S as Static } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { V as Video$1 } from "./Video.CzEOFOtQ.js";
import { S as SelectSource } from "./SelectSource.j1x9BYNI.js";
/* empty css                                                    */
import "./Image.eJ_qOnkr.js";
/* empty css                                                    */
import { W as Webcam } from "./ImageUploader.D4fj6zSl.js";
/* empty css                                               */
import { p as prettyBytes, a as playable } from "./Video.ML_kOajE.js";
import { l } from "./Video.ML_kOajE.js";
import { P as Player, V as VideoPreview } from "./VideoPreview.kpXYMTB0.js";
import { default as default2 } from "./Example.HUy-V3IF.js";
import { U as UploadText } from "./UploadText.BG6Hywul.js";
function create_if_block_4(ctx) {
  let div0;
  let t0_value = (
    /*value*/
    (ctx[0].orig_name || /*value*/
    ctx[0].url) + ""
  );
  let t0;
  let t1;
  let div1;
  let t2_value = prettyBytes(
    /*value*/
    ctx[0].size
  ) + "";
  let t2;
  return {
    c() {
      div0 = element("div");
      t0 = text(t0_value);
      t1 = space();
      div1 = element("div");
      t2 = text(t2_value);
      this.h();
    },
    l(nodes) {
      div0 = claim_element(nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      t0 = claim_text(div0_nodes, t0_value);
      div0_nodes.forEach(detach);
      t1 = claim_space(nodes);
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      t2 = claim_text(div1_nodes, t2_value);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "file-name svelte-14jis2k");
      attr(div1, "class", "file-size svelte-14jis2k");
    },
    m(target, anchor) {
      insert_hydration(target, div0, anchor);
      append_hydration(div0, t0);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, div1, anchor);
      append_hydration(div1, t2);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*value*/
      1 && t0_value !== (t0_value = /*value*/
      (ctx2[0].orig_name || /*value*/
      ctx2[0].url) + ""))
        set_data(t0, t0_value);
      if (dirty[0] & /*value*/
      1 && t2_value !== (t2_value = prettyBytes(
        /*value*/
        ctx2[0].size
      ) + ""))
        set_data(t2, t2_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div0);
        detach(t1);
        detach(div1);
      }
    }
  };
}
function create_if_block_3(ctx) {
  var _a;
  let previous_key = (
    /*value*/
    (_a = ctx[0]) == null ? void 0 : _a.url
  );
  let key_block_anchor;
  let current;
  let key_block = create_key_block(ctx);
  return {
    c() {
      key_block.c();
      key_block_anchor = empty();
    },
    l(nodes) {
      key_block.l(nodes);
      key_block_anchor = empty();
    },
    m(target, anchor) {
      key_block.m(target, anchor);
      insert_hydration(target, key_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      if (dirty[0] & /*value*/
      1 && safe_not_equal(previous_key, previous_key = /*value*/
      (_a2 = ctx2[0]) == null ? void 0 : _a2.url)) {
        group_outros();
        transition_out(key_block, 1, 1, noop);
        check_outros();
        key_block = create_key_block(ctx2);
        key_block.c();
        transition_in(key_block, 1);
        key_block.m(key_block_anchor.parentNode, key_block_anchor);
      } else {
        key_block.p(ctx2, dirty);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(key_block);
      current = true;
    },
    o(local) {
      transition_out(key_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(key_block_anchor);
      }
      key_block.d(detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let div;
  let current_block_type_index;
  let if_block;
  let current;
  const if_block_creators = [create_if_block_1, create_if_block_2];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*active_source*/
      ctx2[1] === "upload"
    )
      return 0;
    if (
      /*active_source*/
      ctx2[1] === "webcam"
    )
      return 1;
    return -1;
  }
  if (~(current_block_type_index = select_block_type_1(ctx))) {
    if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  return {
    c() {
      div = element("div");
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "upload-container svelte-14jis2k");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(div, null);
      }
      current = true;
    },
    p(ctx2, dirty) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx2, dirty);
        }
      } else {
        if (if_block) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block = if_blocks[current_block_type_index];
          if (!if_block) {
            if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
            if_block.c();
          } else {
            if_block.p(ctx2, dirty);
          }
          transition_in(if_block, 1);
          if_block.m(div, null);
        } else {
          if_block = null;
        }
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
        detach(div);
      }
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
    }
  };
}
function create_key_block(ctx) {
  var _a;
  let player;
  let current;
  player = new Player({
    props: {
      upload: (
        /*upload*/
        ctx[15]
      ),
      root: (
        /*root*/
        ctx[11]
      ),
      interactive: true,
      autoplay: (
        /*autoplay*/
        ctx[10]
      ),
      src: (
        /*value*/
        ctx[0].url
      ),
      subtitle: (
        /*subtitle*/
        (_a = ctx[3]) == null ? void 0 : _a.url
      ),
      is_stream: false,
      mirror: (
        /*mirror_webcam*/
        ctx[8] && /*active_source*/
        ctx[1] === "webcam"
      ),
      label: (
        /*label*/
        ctx[5]
      ),
      handle_change: (
        /*handle_change*/
        ctx[23]
      ),
      handle_reset_value: (
        /*handle_reset_value*/
        ctx[13]
      ),
      loop: (
        /*loop*/
        ctx[17]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      i18n: (
        /*i18n*/
        ctx[12]
      ),
      show_download_button: (
        /*show_download_button*/
        ctx[6]
      ),
      handle_clear: (
        /*handle_clear*/
        ctx[22]
      ),
      has_change_history: (
        /*has_change_history*/
        ctx[19]
      )
    }
  });
  player.$on(
    "play",
    /*play_handler*/
    ctx[32]
  );
  player.$on(
    "pause",
    /*pause_handler*/
    ctx[33]
  );
  player.$on(
    "stop",
    /*stop_handler*/
    ctx[34]
  );
  player.$on(
    "end",
    /*end_handler*/
    ctx[35]
  );
  return {
    c() {
      create_component(player.$$.fragment);
    },
    l(nodes) {
      claim_component(player.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(player, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const player_changes = {};
      if (dirty[0] & /*upload*/
      32768)
        player_changes.upload = /*upload*/
        ctx2[15];
      if (dirty[0] & /*root*/
      2048)
        player_changes.root = /*root*/
        ctx2[11];
      if (dirty[0] & /*autoplay*/
      1024)
        player_changes.autoplay = /*autoplay*/
        ctx2[10];
      if (dirty[0] & /*value*/
      1)
        player_changes.src = /*value*/
        ctx2[0].url;
      if (dirty[0] & /*subtitle*/
      8)
        player_changes.subtitle = /*subtitle*/
        (_a2 = ctx2[3]) == null ? void 0 : _a2.url;
      if (dirty[0] & /*mirror_webcam, active_source*/
      258)
        player_changes.mirror = /*mirror_webcam*/
        ctx2[8] && /*active_source*/
        ctx2[1] === "webcam";
      if (dirty[0] & /*label*/
      32)
        player_changes.label = /*label*/
        ctx2[5];
      if (dirty[0] & /*handle_reset_value*/
      8192)
        player_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[13];
      if (dirty[0] & /*loop*/
      131072)
        player_changes.loop = /*loop*/
        ctx2[17];
      if (dirty[0] & /*value*/
      1)
        player_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*i18n*/
      4096)
        player_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*show_download_button*/
      64)
        player_changes.show_download_button = /*show_download_button*/
        ctx2[6];
      if (dirty[0] & /*has_change_history*/
      524288)
        player_changes.has_change_history = /*has_change_history*/
        ctx2[19];
      player.$set(player_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(player.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(player.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(player, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let webcam;
  let current;
  webcam = new Webcam({
    props: {
      root: (
        /*root*/
        ctx[11]
      ),
      mirror_webcam: (
        /*mirror_webcam*/
        ctx[8]
      ),
      include_audio: (
        /*include_audio*/
        ctx[9]
      ),
      mode: "video",
      i18n: (
        /*i18n*/
        ctx[12]
      ),
      upload: (
        /*upload*/
        ctx[15]
      ),
      stream_every: 1
    }
  });
  webcam.$on(
    "error",
    /*error_handler*/
    ctx[29]
  );
  webcam.$on(
    "capture",
    /*handle_capture*/
    ctx[24]
  );
  webcam.$on(
    "start_recording",
    /*start_recording_handler*/
    ctx[30]
  );
  webcam.$on(
    "stop_recording",
    /*stop_recording_handler*/
    ctx[31]
  );
  return {
    c() {
      create_component(webcam.$$.fragment);
    },
    l(nodes) {
      claim_component(webcam.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(webcam, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const webcam_changes = {};
      if (dirty[0] & /*root*/
      2048)
        webcam_changes.root = /*root*/
        ctx2[11];
      if (dirty[0] & /*mirror_webcam*/
      256)
        webcam_changes.mirror_webcam = /*mirror_webcam*/
        ctx2[8];
      if (dirty[0] & /*include_audio*/
      512)
        webcam_changes.include_audio = /*include_audio*/
        ctx2[9];
      if (dirty[0] & /*i18n*/
      4096)
        webcam_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*upload*/
      32768)
        webcam_changes.upload = /*upload*/
        ctx2[15];
      webcam.$set(webcam_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(webcam.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(webcam.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(webcam, detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let current;
  function upload_1_dragging_binding(value) {
    ctx[26](value);
  }
  function upload_1_uploading_binding(value) {
    ctx[27](value);
  }
  let upload_1_props = {
    filetype: "video/x-m4v,video/*",
    max_file_size: (
      /*max_file_size*/
      ctx[14]
    ),
    root: (
      /*root*/
      ctx[11]
    ),
    upload: (
      /*upload*/
      ctx[15]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[16]
    ),
    $$slots: { default: [create_default_slot$1] },
    $$scope: { ctx }
  };
  if (
    /*dragging*/
    ctx[18] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[18];
  }
  if (
    /*uploading*/
    ctx[2] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[2];
  }
  upload_1 = new Upload({ props: upload_1_props });
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  upload_1.$on(
    "load",
    /*handle_load*/
    ctx[21]
  );
  upload_1.$on(
    "error",
    /*error_handler_1*/
    ctx[28]
  );
  return {
    c() {
      create_component(upload_1.$$.fragment);
    },
    l(nodes) {
      claim_component(upload_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(upload_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const upload_1_changes = {};
      if (dirty[0] & /*max_file_size*/
      16384)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[14];
      if (dirty[0] & /*root*/
      2048)
        upload_1_changes.root = /*root*/
        ctx2[11];
      if (dirty[0] & /*upload*/
      32768)
        upload_1_changes.upload = /*upload*/
        ctx2[15];
      if (dirty[0] & /*stream_handler*/
      65536)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[16];
      if (dirty[1] & /*$$scope*/
      64) {
        upload_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      262144) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[18];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      4) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[2];
        add_flush_callback(() => updating_uploading = false);
      }
      upload_1.$set(upload_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(upload_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(upload_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(upload_1, detaching);
    }
  };
}
function create_default_slot$1(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[25].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[37],
    null
  );
  return {
    c() {
      if (default_slot)
        default_slot.c();
    },
    l(nodes) {
      if (default_slot)
        default_slot.l(nodes);
    },
    m(target, anchor) {
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty[1] & /*$$scope*/
        64)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[37],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[37]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[37],
              dirty,
              null
            ),
            null
          );
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(default_slot, local);
      current = true;
    },
    o(local) {
      transition_out(default_slot, local);
      current = false;
    },
    d(detaching) {
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let blocklabel;
  let t0;
  let div;
  let show_if;
  let current_block_type_index;
  let if_block;
  let t1;
  let selectsource;
  let updating_active_source;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[7]
      ),
      Icon: Video$1,
      label: (
        /*label*/
        ctx[5] || "Video"
      )
    }
  });
  const if_block_creators = [create_if_block$1, create_if_block_3, create_if_block_4];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] === null || /*value*/
      ctx2[0].url === void 0
    )
      return 0;
    if (show_if == null)
      show_if = !!playable();
    if (show_if)
      return 1;
    if (
      /*value*/
      ctx2[0].size
    )
      return 2;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
    if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  function selectsource_active_source_binding(value) {
    ctx[36](value);
  }
  let selectsource_props = {
    sources: (
      /*sources*/
      ctx[4]
    ),
    handle_clear: (
      /*handle_clear*/
      ctx[22]
    )
  };
  if (
    /*active_source*/
    ctx[1] !== void 0
  ) {
    selectsource_props.active_source = /*active_source*/
    ctx[1];
  }
  selectsource = new SelectSource({ props: selectsource_props });
  binding_callbacks.push(() => bind(selectsource, "active_source", selectsource_active_source_binding));
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t0 = space();
      div = element("div");
      if (if_block)
        if_block.c();
      t1 = space();
      create_component(selectsource.$$.fragment);
      this.h();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t0 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { "data-testid": true, class: true });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      t1 = claim_space(div_nodes);
      claim_component(selectsource.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "data-testid", "video");
      attr(div, "class", "video-container svelte-14jis2k");
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, div, anchor);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(div, null);
      }
      append_hydration(div, t1);
      mount_component(selectsource, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      128)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*label*/
      32)
        blocklabel_changes.label = /*label*/
        ctx2[5] || "Video";
      blocklabel.$set(blocklabel_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx2, dirty);
        }
      } else {
        if (if_block) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block = if_blocks[current_block_type_index];
          if (!if_block) {
            if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
            if_block.c();
          } else {
            if_block.p(ctx2, dirty);
          }
          transition_in(if_block, 1);
          if_block.m(div, t1);
        } else {
          if_block = null;
        }
      }
      const selectsource_changes = {};
      if (dirty[0] & /*sources*/
      16)
        selectsource_changes.sources = /*sources*/
        ctx2[4];
      if (!updating_active_source && dirty[0] & /*active_source*/
      2) {
        updating_active_source = true;
        selectsource_changes.active_source = /*active_source*/
        ctx2[1];
        add_flush_callback(() => updating_active_source = false);
      }
      selectsource.$set(selectsource_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      transition_in(selectsource.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      transition_out(selectsource.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(div);
      }
      destroy_component(blocklabel, detaching);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
      destroy_component(selectsource);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { value = null } = $$props;
  let { subtitle = null } = $$props;
  let { sources = ["webcam", "upload"] } = $$props;
  let { label = void 0 } = $$props;
  let { show_download_button = false } = $$props;
  let { show_label = true } = $$props;
  let { mirror_webcam = false } = $$props;
  let { include_audio } = $$props;
  let { autoplay } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { active_source = "webcam" } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { loop } = $$props;
  let { uploading = false } = $$props;
  let has_change_history = false;
  const dispatch = createEventDispatcher();
  function handle_load({ detail }) {
    $$invalidate(0, value = detail);
    dispatch("change", detail);
    dispatch("upload", detail);
  }
  function handle_clear() {
    $$invalidate(0, value = null);
    dispatch("change", null);
    dispatch("clear");
  }
  function handle_change(video) {
    $$invalidate(19, has_change_history = true);
    dispatch("change", video);
  }
  function handle_capture({ detail }) {
    dispatch("change", detail);
  }
  let dragging = false;
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(18, dragging);
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(2, uploading);
  }
  const error_handler_1 = ({ detail }) => dispatch("error", detail);
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function start_recording_handler(event) {
    bubble.call(this, $$self, event);
  }
  function stop_recording_handler(event) {
    bubble.call(this, $$self, event);
  }
  function play_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_handler(event) {
    bubble.call(this, $$self, event);
  }
  function stop_handler(event) {
    bubble.call(this, $$self, event);
  }
  function end_handler(event) {
    bubble.call(this, $$self, event);
  }
  function selectsource_active_source_binding(value2) {
    active_source = value2;
    $$invalidate(1, active_source);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("subtitle" in $$props2)
      $$invalidate(3, subtitle = $$props2.subtitle);
    if ("sources" in $$props2)
      $$invalidate(4, sources = $$props2.sources);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("show_download_button" in $$props2)
      $$invalidate(6, show_download_button = $$props2.show_download_button);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("mirror_webcam" in $$props2)
      $$invalidate(8, mirror_webcam = $$props2.mirror_webcam);
    if ("include_audio" in $$props2)
      $$invalidate(9, include_audio = $$props2.include_audio);
    if ("autoplay" in $$props2)
      $$invalidate(10, autoplay = $$props2.autoplay);
    if ("root" in $$props2)
      $$invalidate(11, root = $$props2.root);
    if ("i18n" in $$props2)
      $$invalidate(12, i18n = $$props2.i18n);
    if ("active_source" in $$props2)
      $$invalidate(1, active_source = $$props2.active_source);
    if ("handle_reset_value" in $$props2)
      $$invalidate(13, handle_reset_value = $$props2.handle_reset_value);
    if ("max_file_size" in $$props2)
      $$invalidate(14, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(15, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(16, stream_handler = $$props2.stream_handler);
    if ("loop" in $$props2)
      $$invalidate(17, loop = $$props2.loop);
    if ("uploading" in $$props2)
      $$invalidate(2, uploading = $$props2.uploading);
    if ("$$scope" in $$props2)
      $$invalidate(37, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*dragging*/
    262144) {
      dispatch("drag", dragging);
    }
  };
  return [
    value,
    active_source,
    uploading,
    subtitle,
    sources,
    label,
    show_download_button,
    show_label,
    mirror_webcam,
    include_audio,
    autoplay,
    root,
    i18n,
    handle_reset_value,
    max_file_size,
    upload,
    stream_handler,
    loop,
    dragging,
    has_change_history,
    dispatch,
    handle_load,
    handle_clear,
    handle_change,
    handle_capture,
    slots,
    upload_1_dragging_binding,
    upload_1_uploading_binding,
    error_handler_1,
    error_handler,
    start_recording_handler,
    stop_recording_handler,
    play_handler,
    pause_handler,
    stop_handler,
    end_handler,
    selectsource_active_source_binding,
    $$scope
  ];
}
class InteractiveVideo extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 0,
        subtitle: 3,
        sources: 4,
        label: 5,
        show_download_button: 6,
        show_label: 7,
        mirror_webcam: 8,
        include_audio: 9,
        autoplay: 10,
        root: 11,
        i18n: 12,
        active_source: 1,
        handle_reset_value: 13,
        max_file_size: 14,
        upload: 15,
        stream_handler: 16,
        loop: 17,
        uploading: 2
      },
      null,
      [-1, -1]
    );
  }
}
const Video = InteractiveVideo;
function create_else_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[4]
      ),
      variant: (
        /*value*/
        ctx[0] === null && /*active_source*/
        ctx[23] === "upload" ? "dashed" : "solid"
      ),
      border_mode: (
        /*dragging*/
        ctx[26] ? "focus" : "base"
      ),
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[2]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[3]
      ),
      height: (
        /*height*/
        ctx[9]
      ),
      width: (
        /*width*/
        ctx[10]
      ),
      container: (
        /*container*/
        ctx[11]
      ),
      scale: (
        /*scale*/
        ctx[12]
      ),
      min_width: (
        /*min_width*/
        ctx[13]
      ),
      allow_overflow: false,
      $$slots: { default: [create_default_slot_1] },
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
      16)
        block_changes.visible = /*visible*/
        ctx2[4];
      if (dirty[0] & /*value, active_source*/
      8388609)
        block_changes.variant = /*value*/
        ctx2[0] === null && /*active_source*/
        ctx2[23] === "upload" ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      67108864)
        block_changes.border_mode = /*dragging*/
        ctx2[26] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      4)
        block_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty[0] & /*elem_classes*/
      8)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[3];
      if (dirty[0] & /*height*/
      512)
        block_changes.height = /*height*/
        ctx2[9];
      if (dirty[0] & /*width*/
      1024)
        block_changes.width = /*width*/
        ctx2[10];
      if (dirty[0] & /*container*/
      2048)
        block_changes.container = /*container*/
        ctx2[11];
      if (dirty[0] & /*scale*/
      4096)
        block_changes.scale = /*scale*/
        ctx2[12];
      if (dirty[0] & /*min_width*/
      8192)
        block_changes.min_width = /*min_width*/
        ctx2[13];
      if (dirty[0] & /*_video, _subtitle, label, show_label, show_download_button, sources, active_source, mirror_webcam, include_audio, autoplay, root, loop, gradio, uploading, dragging, loading_status*/
      133906914 | dirty[1] & /*$$scope*/
      8388608) {
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
function create_if_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[4]
      ),
      variant: (
        /*value*/
        ctx[0] === null && /*active_source*/
        ctx[23] === "upload" ? "dashed" : "solid"
      ),
      border_mode: (
        /*dragging*/
        ctx[26] ? "focus" : "base"
      ),
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[2]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[3]
      ),
      height: (
        /*height*/
        ctx[9]
      ),
      width: (
        /*width*/
        ctx[10]
      ),
      container: (
        /*container*/
        ctx[11]
      ),
      scale: (
        /*scale*/
        ctx[12]
      ),
      min_width: (
        /*min_width*/
        ctx[13]
      ),
      allow_overflow: false,
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
      16)
        block_changes.visible = /*visible*/
        ctx2[4];
      if (dirty[0] & /*value, active_source*/
      8388609)
        block_changes.variant = /*value*/
        ctx2[0] === null && /*active_source*/
        ctx2[23] === "upload" ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      67108864)
        block_changes.border_mode = /*dragging*/
        ctx2[26] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      4)
        block_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty[0] & /*elem_classes*/
      8)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[3];
      if (dirty[0] & /*height*/
      512)
        block_changes.height = /*height*/
        ctx2[9];
      if (dirty[0] & /*width*/
      1024)
        block_changes.width = /*width*/
        ctx2[10];
      if (dirty[0] & /*container*/
      2048)
        block_changes.container = /*container*/
        ctx2[11];
      if (dirty[0] & /*scale*/
      4096)
        block_changes.scale = /*scale*/
        ctx2[12];
      if (dirty[0] & /*min_width*/
      8192)
        block_changes.min_width = /*min_width*/
        ctx2[13];
      if (dirty[0] & /*_video, _subtitle, label, show_label, autoplay, loop, show_share_button, show_download_button, gradio, loading_status*/
      52674850 | dirty[1] & /*$$scope*/
      8388608) {
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
function create_default_slot_2(ctx) {
  let uploadtext;
  let current;
  uploadtext = new UploadText({
    props: {
      i18n: (
        /*gradio*/
        ctx[17].i18n
      ),
      type: "video"
    }
  });
  return {
    c() {
      create_component(uploadtext.$$.fragment);
    },
    l(nodes) {
      claim_component(uploadtext.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(uploadtext, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const uploadtext_changes = {};
      if (dirty[0] & /*gradio*/
      131072)
        uploadtext_changes.i18n = /*gradio*/
        ctx2[17].i18n;
      uploadtext.$set(uploadtext_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(uploadtext.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(uploadtext.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(uploadtext, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let statustracker;
  let t;
  let video;
  let updating_uploading;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[17].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[17].i18n
    ) },
    /*loading_status*/
    ctx[1]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler_1*/
    ctx[41]
  );
  function video_uploading_binding(value) {
    ctx[44](value);
  }
  let video_props = {
    value: (
      /*_video*/
      ctx[24]
    ),
    subtitle: (
      /*_subtitle*/
      ctx[25]
    ),
    label: (
      /*label*/
      ctx[5]
    ),
    show_label: (
      /*show_label*/
      ctx[8]
    ),
    show_download_button: (
      /*show_download_button*/
      ctx[16]
    ),
    sources: (
      /*sources*/
      ctx[6]
    ),
    active_source: (
      /*active_source*/
      ctx[23]
    ),
    mirror_webcam: (
      /*mirror_webcam*/
      ctx[19]
    ),
    include_audio: (
      /*include_audio*/
      ctx[20]
    ),
    autoplay: (
      /*autoplay*/
      ctx[14]
    ),
    root: (
      /*root*/
      ctx[7]
    ),
    loop: (
      /*loop*/
      ctx[21]
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[27]
    ),
    i18n: (
      /*gradio*/
      ctx[17].i18n
    ),
    max_file_size: (
      /*gradio*/
      ctx[17].max_file_size
    ),
    upload: (
      /*func_1*/
      ctx[42]
    ),
    stream_handler: (
      /*func_2*/
      ctx[43]
    ),
    $$slots: { default: [create_default_slot_2] },
    $$scope: { ctx }
  };
  if (
    /*uploading*/
    ctx[22] !== void 0
  ) {
    video_props.uploading = /*uploading*/
    ctx[22];
  }
  video = new Video({ props: video_props });
  binding_callbacks.push(() => bind(video, "uploading", video_uploading_binding));
  video.$on(
    "change",
    /*handle_change*/
    ctx[28]
  );
  video.$on(
    "drag",
    /*drag_handler*/
    ctx[45]
  );
  video.$on(
    "error",
    /*handle_error*/
    ctx[29]
  );
  video.$on(
    "clear",
    /*clear_handler*/
    ctx[46]
  );
  video.$on(
    "play",
    /*play_handler_1*/
    ctx[47]
  );
  video.$on(
    "pause",
    /*pause_handler_1*/
    ctx[48]
  );
  video.$on(
    "upload",
    /*upload_handler*/
    ctx[49]
  );
  video.$on(
    "stop",
    /*stop_handler_1*/
    ctx[50]
  );
  video.$on(
    "end",
    /*end_handler_1*/
    ctx[51]
  );
  video.$on(
    "start_recording",
    /*start_recording_handler*/
    ctx[52]
  );
  video.$on(
    "stop_recording",
    /*stop_recording_handler*/
    ctx[53]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(video.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(video.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(video, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      131074 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        131072 && {
          autoscroll: (
            /*gradio*/
            ctx2[17].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        131072 && { i18n: (
          /*gradio*/
          ctx2[17].i18n
        ) },
        dirty[0] & /*loading_status*/
        2 && get_spread_object(
          /*loading_status*/
          ctx2[1]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const video_changes = {};
      if (dirty[0] & /*_video*/
      16777216)
        video_changes.value = /*_video*/
        ctx2[24];
      if (dirty[0] & /*_subtitle*/
      33554432)
        video_changes.subtitle = /*_subtitle*/
        ctx2[25];
      if (dirty[0] & /*label*/
      32)
        video_changes.label = /*label*/
        ctx2[5];
      if (dirty[0] & /*show_label*/
      256)
        video_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*show_download_button*/
      65536)
        video_changes.show_download_button = /*show_download_button*/
        ctx2[16];
      if (dirty[0] & /*sources*/
      64)
        video_changes.sources = /*sources*/
        ctx2[6];
      if (dirty[0] & /*active_source*/
      8388608)
        video_changes.active_source = /*active_source*/
        ctx2[23];
      if (dirty[0] & /*mirror_webcam*/
      524288)
        video_changes.mirror_webcam = /*mirror_webcam*/
        ctx2[19];
      if (dirty[0] & /*include_audio*/
      1048576)
        video_changes.include_audio = /*include_audio*/
        ctx2[20];
      if (dirty[0] & /*autoplay*/
      16384)
        video_changes.autoplay = /*autoplay*/
        ctx2[14];
      if (dirty[0] & /*root*/
      128)
        video_changes.root = /*root*/
        ctx2[7];
      if (dirty[0] & /*loop*/
      2097152)
        video_changes.loop = /*loop*/
        ctx2[21];
      if (dirty[0] & /*gradio*/
      131072)
        video_changes.i18n = /*gradio*/
        ctx2[17].i18n;
      if (dirty[0] & /*gradio*/
      131072)
        video_changes.max_file_size = /*gradio*/
        ctx2[17].max_file_size;
      if (dirty[0] & /*gradio*/
      131072)
        video_changes.upload = /*func_1*/
        ctx2[42];
      if (dirty[0] & /*gradio*/
      131072)
        video_changes.stream_handler = /*func_2*/
        ctx2[43];
      if (dirty[0] & /*gradio*/
      131072 | dirty[1] & /*$$scope*/
      8388608) {
        video_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      4194304) {
        updating_uploading = true;
        video_changes.uploading = /*uploading*/
        ctx2[22];
        add_flush_callback(() => updating_uploading = false);
      }
      video.$set(video_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(video.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(video.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(video, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t;
  let staticvideo;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[17].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[17].i18n
    ) },
    /*loading_status*/
    ctx[1]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[33]
  );
  staticvideo = new VideoPreview({
    props: {
      value: (
        /*_video*/
        ctx[24]
      ),
      subtitle: (
        /*_subtitle*/
        ctx[25]
      ),
      label: (
        /*label*/
        ctx[5]
      ),
      show_label: (
        /*show_label*/
        ctx[8]
      ),
      autoplay: (
        /*autoplay*/
        ctx[14]
      ),
      loop: (
        /*loop*/
        ctx[21]
      ),
      show_share_button: (
        /*show_share_button*/
        ctx[15]
      ),
      show_download_button: (
        /*show_download_button*/
        ctx[16]
      ),
      i18n: (
        /*gradio*/
        ctx[17].i18n
      ),
      upload: (
        /*func*/
        ctx[34]
      )
    }
  });
  staticvideo.$on(
    "play",
    /*play_handler*/
    ctx[35]
  );
  staticvideo.$on(
    "pause",
    /*pause_handler*/
    ctx[36]
  );
  staticvideo.$on(
    "stop",
    /*stop_handler*/
    ctx[37]
  );
  staticvideo.$on(
    "end",
    /*end_handler*/
    ctx[38]
  );
  staticvideo.$on(
    "share",
    /*share_handler*/
    ctx[39]
  );
  staticvideo.$on(
    "error",
    /*error_handler*/
    ctx[40]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(staticvideo.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(staticvideo.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(staticvideo, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      131074 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        131072 && {
          autoscroll: (
            /*gradio*/
            ctx2[17].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        131072 && { i18n: (
          /*gradio*/
          ctx2[17].i18n
        ) },
        dirty[0] & /*loading_status*/
        2 && get_spread_object(
          /*loading_status*/
          ctx2[1]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const staticvideo_changes = {};
      if (dirty[0] & /*_video*/
      16777216)
        staticvideo_changes.value = /*_video*/
        ctx2[24];
      if (dirty[0] & /*_subtitle*/
      33554432)
        staticvideo_changes.subtitle = /*_subtitle*/
        ctx2[25];
      if (dirty[0] & /*label*/
      32)
        staticvideo_changes.label = /*label*/
        ctx2[5];
      if (dirty[0] & /*show_label*/
      256)
        staticvideo_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*autoplay*/
      16384)
        staticvideo_changes.autoplay = /*autoplay*/
        ctx2[14];
      if (dirty[0] & /*loop*/
      2097152)
        staticvideo_changes.loop = /*loop*/
        ctx2[21];
      if (dirty[0] & /*show_share_button*/
      32768)
        staticvideo_changes.show_share_button = /*show_share_button*/
        ctx2[15];
      if (dirty[0] & /*show_download_button*/
      65536)
        staticvideo_changes.show_download_button = /*show_download_button*/
        ctx2[16];
      if (dirty[0] & /*gradio*/
      131072)
        staticvideo_changes.i18n = /*gradio*/
        ctx2[17].i18n;
      if (dirty[0] & /*gradio*/
      131072)
        staticvideo_changes.upload = /*func*/
        ctx2[34];
      staticvideo.$set(staticvideo_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(staticvideo.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(staticvideo.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(staticvideo, detaching);
    }
  };
}
function create_fragment(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*interactive*/
    ctx2[18])
      return 0;
    return 1;
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
    p(ctx2, dirty) {
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
function instance($$self, $$props, $$invalidate) {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let old_value = null;
  let { label } = $$props;
  let { sources } = $$props;
  let { root } = $$props;
  let { show_label } = $$props;
  let { loading_status } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { container = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { autoplay = false } = $$props;
  let { show_share_button = true } = $$props;
  let { show_download_button } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  let { mirror_webcam } = $$props;
  let { include_audio } = $$props;
  let { loop = false } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let _video = null;
  let _subtitle = null;
  let active_source;
  let initial_value = value;
  const handle_reset_value = () => {
    if (initial_value === null || value === initial_value) {
      return;
    }
    $$invalidate(0, value = initial_value);
  };
  let dragging = false;
  function handle_change({ detail }) {
    if (detail != null) {
      $$invalidate(0, value = { video: detail, subtitles: null });
    } else {
      $$invalidate(0, value = null);
    }
  }
  function handle_error({ detail }) {
    const [level, status] = detail.includes("Invalid file type") ? ["warning", "complete"] : ["error", "error"];
    $$invalidate(1, loading_status = loading_status || {});
    $$invalidate(1, loading_status.status = status, loading_status);
    $$invalidate(1, loading_status.message = detail, loading_status);
    gradio.dispatch(level, detail);
  }
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const func = (...args) => gradio.client.upload(...args);
  const play_handler = () => gradio.dispatch("play");
  const pause_handler = () => gradio.dispatch("pause");
  const stop_handler = () => gradio.dispatch("stop");
  const end_handler = () => gradio.dispatch("end");
  const share_handler = ({ detail }) => gradio.dispatch("share", detail);
  const error_handler = ({ detail }) => gradio.dispatch("error", detail);
  const clear_status_handler_1 = () => gradio.dispatch("clear_status", loading_status);
  const func_1 = (...args) => gradio.client.upload(...args);
  const func_2 = (...args) => gradio.client.stream(...args);
  function video_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(22, uploading);
  }
  const drag_handler = ({ detail }) => $$invalidate(26, dragging = detail);
  const clear_handler = () => gradio.dispatch("clear");
  const play_handler_1 = () => gradio.dispatch("play");
  const pause_handler_1 = () => gradio.dispatch("pause");
  const upload_handler = () => gradio.dispatch("upload");
  const stop_handler_1 = () => gradio.dispatch("stop");
  const end_handler_1 = () => gradio.dispatch("end");
  const start_recording_handler = () => gradio.dispatch("start_recording");
  const stop_recording_handler = () => gradio.dispatch("stop_recording");
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(2, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(3, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(4, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("sources" in $$props2)
      $$invalidate(6, sources = $$props2.sources);
    if ("root" in $$props2)
      $$invalidate(7, root = $$props2.root);
    if ("show_label" in $$props2)
      $$invalidate(8, show_label = $$props2.show_label);
    if ("loading_status" in $$props2)
      $$invalidate(1, loading_status = $$props2.loading_status);
    if ("height" in $$props2)
      $$invalidate(9, height = $$props2.height);
    if ("width" in $$props2)
      $$invalidate(10, width = $$props2.width);
    if ("container" in $$props2)
      $$invalidate(11, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(12, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(13, min_width = $$props2.min_width);
    if ("autoplay" in $$props2)
      $$invalidate(14, autoplay = $$props2.autoplay);
    if ("show_share_button" in $$props2)
      $$invalidate(15, show_share_button = $$props2.show_share_button);
    if ("show_download_button" in $$props2)
      $$invalidate(16, show_download_button = $$props2.show_download_button);
    if ("gradio" in $$props2)
      $$invalidate(17, gradio = $$props2.gradio);
    if ("interactive" in $$props2)
      $$invalidate(18, interactive = $$props2.interactive);
    if ("mirror_webcam" in $$props2)
      $$invalidate(19, mirror_webcam = $$props2.mirror_webcam);
    if ("include_audio" in $$props2)
      $$invalidate(20, include_audio = $$props2.include_audio);
    if ("loop" in $$props2)
      $$invalidate(21, loop = $$props2.loop);
    if ("input_ready" in $$props2)
      $$invalidate(30, input_ready = $$props2.input_ready);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*uploading*/
    4194304) {
      $$invalidate(30, input_ready = !uploading);
    }
    if ($$self.$$.dirty[0] & /*value*/
    1 | $$self.$$.dirty[1] & /*initial_value*/
    2) {
      if (value && initial_value === null) {
        $$invalidate(32, initial_value = value);
      }
    }
    if ($$self.$$.dirty[0] & /*sources, active_source*/
    8388672) {
      if (sources && !active_source) {
        $$invalidate(23, active_source = sources[0]);
      }
    }
    if ($$self.$$.dirty[0] & /*value*/
    1) {
      {
        if (value != null) {
          $$invalidate(24, _video = value.video);
          $$invalidate(25, _subtitle = value.subtitles);
        } else {
          $$invalidate(24, _video = null);
          $$invalidate(25, _subtitle = null);
        }
      }
    }
    if ($$self.$$.dirty[0] & /*value, gradio*/
    131073 | $$self.$$.dirty[1] & /*old_value*/
    1) {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          $$invalidate(31, old_value = value);
          gradio.dispatch("change");
        }
      }
    }
  };
  return [
    value,
    loading_status,
    elem_id,
    elem_classes,
    visible,
    label,
    sources,
    root,
    show_label,
    height,
    width,
    container,
    scale,
    min_width,
    autoplay,
    show_share_button,
    show_download_button,
    gradio,
    interactive,
    mirror_webcam,
    include_audio,
    loop,
    uploading,
    active_source,
    _video,
    _subtitle,
    dragging,
    handle_reset_value,
    handle_change,
    handle_error,
    input_ready,
    old_value,
    initial_value,
    clear_status_handler,
    func,
    play_handler,
    pause_handler,
    stop_handler,
    end_handler,
    share_handler,
    error_handler,
    clear_status_handler_1,
    func_1,
    func_2,
    video_uploading_binding,
    drag_handler,
    clear_handler,
    play_handler_1,
    pause_handler_1,
    upload_handler,
    stop_handler_1,
    end_handler_1,
    start_recording_handler,
    stop_recording_handler
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
        elem_id: 2,
        elem_classes: 3,
        visible: 4,
        value: 0,
        label: 5,
        sources: 6,
        root: 7,
        show_label: 8,
        loading_status: 1,
        height: 9,
        width: 10,
        container: 11,
        scale: 12,
        min_width: 13,
        autoplay: 14,
        show_share_button: 15,
        show_download_button: 16,
        gradio: 17,
        interactive: 18,
        mirror_webcam: 19,
        include_audio: 20,
        loop: 21,
        input_ready: 30
      },
      null,
      [-1, -1]
    );
  }
  get elem_id() {
    return this.$$.ctx[2];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[3];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[4];
  }
  set visible(visible) {
    this.$$set({ visible });
    flush();
  }
  get value() {
    return this.$$.ctx[0];
  }
  set value(value) {
    this.$$set({ value });
    flush();
  }
  get label() {
    return this.$$.ctx[5];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get sources() {
    return this.$$.ctx[6];
  }
  set sources(sources) {
    this.$$set({ sources });
    flush();
  }
  get root() {
    return this.$$.ctx[7];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get show_label() {
    return this.$$.ctx[8];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[1];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get height() {
    return this.$$.ctx[9];
  }
  set height(height) {
    this.$$set({ height });
    flush();
  }
  get width() {
    return this.$$.ctx[10];
  }
  set width(width) {
    this.$$set({ width });
    flush();
  }
  get container() {
    return this.$$.ctx[11];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[12];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[13];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get autoplay() {
    return this.$$.ctx[14];
  }
  set autoplay(autoplay) {
    this.$$set({ autoplay });
    flush();
  }
  get show_share_button() {
    return this.$$.ctx[15];
  }
  set show_share_button(show_share_button) {
    this.$$set({ show_share_button });
    flush();
  }
  get show_download_button() {
    return this.$$.ctx[16];
  }
  set show_download_button(show_download_button) {
    this.$$set({ show_download_button });
    flush();
  }
  get gradio() {
    return this.$$.ctx[17];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get interactive() {
    return this.$$.ctx[18];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get mirror_webcam() {
    return this.$$.ctx[19];
  }
  set mirror_webcam(mirror_webcam) {
    this.$$set({ mirror_webcam });
    flush();
  }
  get include_audio() {
    return this.$$.ctx[20];
  }
  set include_audio(include_audio) {
    this.$$set({ include_audio });
    flush();
  }
  get loop() {
    return this.$$.ctx[21];
  }
  set loop(loop) {
    this.$$set({ loop });
    flush();
  }
  get input_ready() {
    return this.$$.ctx[30];
  }
  set input_ready(input_ready) {
    this.$$set({ input_ready });
    flush();
  }
}
const Index$1 = Index;
export {
  default2 as BaseExample,
  Video as BaseInteractiveVideo,
  Player as BasePlayer,
  VideoPreview as BaseStaticVideo,
  Index$1 as default,
  l as loaded,
  playable,
  prettyBytes
};
