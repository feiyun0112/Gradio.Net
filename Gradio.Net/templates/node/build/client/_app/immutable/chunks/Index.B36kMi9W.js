import { SvelteComponent, init, safe_not_equal, flush, empty, insert_hydration, group_outros, transition_out, check_outros, transition_in, detach, afterUpdate, binding_callbacks, create_component, claim_component, mount_component, destroy_component, assign, bind, space, claim_space, get_spread_update, get_spread_object, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import ImagePreview from "./ImagePreview.CDVGjPNr.js";
import { I as ImageUploader } from "./ImageUploader.D4fj6zSl.js";
import { W } from "./ImageUploader.D4fj6zSl.js";
import { B as Block, S as Static } from "./2.BqWhUxOo.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { I as Image } from "./Image.CMPoCWop.js";
import { U as UploadText } from "./UploadText.BG6Hywul.js";
import { I } from "./Image.eJ_qOnkr.js";
import { default as default2 } from "./Example.wemIBKRq.js";
function create_else_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[5]
      ),
      variant: (
        /*value*/
        ctx[0] === null ? "dashed" : "solid"
      ),
      border_mode: (
        /*dragging*/
        ctx[28] ? "focus" : "base"
      ),
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[3]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[4]
      ),
      height: (
        /*height*/
        ctx[10] || void 0
      ),
      width: (
        /*width*/
        ctx[11]
      ),
      allow_overflow: false,
      container: (
        /*container*/
        ctx[14]
      ),
      scale: (
        /*scale*/
        ctx[15]
      ),
      min_width: (
        /*min_width*/
        ctx[16]
      ),
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  block.$on(
    "dragenter",
    /*handle_drag_event*/
    ctx[31]
  );
  block.$on(
    "dragleave",
    /*handle_drag_event*/
    ctx[31]
  );
  block.$on(
    "dragover",
    /*handle_drag_event*/
    ctx[31]
  );
  block.$on(
    "drop",
    /*handle_drop*/
    ctx[32]
  );
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
      32)
        block_changes.visible = /*visible*/
        ctx2[5];
      if (dirty[0] & /*value*/
      1)
        block_changes.variant = /*value*/
        ctx2[0] === null ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      268435456)
        block_changes.border_mode = /*dragging*/
        ctx2[28] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty[0] & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
      if (dirty[0] & /*height*/
      1024)
        block_changes.height = /*height*/
        ctx2[10] || void 0;
      if (dirty[0] & /*width*/
      2048)
        block_changes.width = /*width*/
        ctx2[11];
      if (dirty[0] & /*container*/
      16384)
        block_changes.container = /*container*/
        ctx2[14];
      if (dirty[0] & /*scale*/
      32768)
        block_changes.scale = /*scale*/
        ctx2[15];
      if (dirty[0] & /*min_width*/
      65536)
        block_changes.min_width = /*min_width*/
        ctx2[16];
      if (dirty[0] & /*_selectable, root, sources, label, show_label, pending, streaming, mirror_webcam, stream_every, gradio, upload_component, uploading, active_source, value, dragging, _modify_stream, set_time_limit, loading_status, placeholder*/
      2129932999 | dirty[1] & /*$$scope*/
      536870912) {
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
        ctx[5]
      ),
      variant: "solid",
      border_mode: (
        /*dragging*/
        ctx[28] ? "focus" : "base"
      ),
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[3]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[4]
      ),
      height: (
        /*height*/
        ctx[10] || void 0
      ),
      width: (
        /*width*/
        ctx[11]
      ),
      allow_overflow: false,
      container: (
        /*container*/
        ctx[14]
      ),
      scale: (
        /*scale*/
        ctx[15]
      ),
      min_width: (
        /*min_width*/
        ctx[16]
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
      32)
        block_changes.visible = /*visible*/
        ctx2[5];
      if (dirty[0] & /*dragging*/
      268435456)
        block_changes.border_mode = /*dragging*/
        ctx2[28] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty[0] & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
      if (dirty[0] & /*height*/
      1024)
        block_changes.height = /*height*/
        ctx2[10] || void 0;
      if (dirty[0] & /*width*/
      2048)
        block_changes.width = /*width*/
        ctx2[11];
      if (dirty[0] & /*container*/
      16384)
        block_changes.container = /*container*/
        ctx2[14];
      if (dirty[0] & /*scale*/
      32768)
        block_changes.scale = /*scale*/
        ctx2[15];
      if (dirty[0] & /*min_width*/
      65536)
        block_changes.min_width = /*min_width*/
        ctx2[16];
      if (dirty[0] & /*value, label, show_label, show_download_button, _selectable, show_share_button, gradio, show_fullscreen_button, loading_status*/
      50471365 | dirty[1] & /*$$scope*/
      536870912) {
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
function create_else_block_1(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot_3] },
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
      536870912) {
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
function create_if_block_2(ctx) {
  let uploadtext;
  let current;
  uploadtext = new UploadText({
    props: {
      i18n: (
        /*gradio*/
        ctx[25].i18n
      ),
      type: "clipboard",
      mode: "short"
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
      33554432)
        uploadtext_changes.i18n = /*gradio*/
        ctx2[25].i18n;
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
function create_if_block_1(ctx) {
  let uploadtext;
  let current;
  uploadtext = new UploadText({
    props: {
      i18n: (
        /*gradio*/
        ctx[25].i18n
      ),
      type: "image",
      placeholder: (
        /*placeholder*/
        ctx[23]
      )
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
      33554432)
        uploadtext_changes.i18n = /*gradio*/
        ctx2[25].i18n;
      if (dirty[0] & /*placeholder*/
      8388608)
        uploadtext_changes.placeholder = /*placeholder*/
        ctx2[23];
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
function create_default_slot_3(ctx) {
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
function create_default_slot_2(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block_1, create_if_block_2, create_else_block_1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*active_source*/
      ctx2[29] === "upload" || !/*active_source*/
      ctx2[29]
    )
      return 0;
    if (
      /*active_source*/
      ctx2[29] === "clipboard"
    )
      return 1;
    return 2;
  }
  current_block_type_index = select_block_type_1(ctx);
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
      current_block_type_index = select_block_type_1(ctx2);
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
function create_default_slot_1(ctx) {
  var _a;
  let statustracker;
  let t;
  let imageuploader;
  let updating_uploading;
  let updating_active_source;
  let updating_value;
  let updating_dragging;
  let updating_modify_stream;
  let updating_set_time_limit;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[25].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[25].i18n
    ) },
    /*loading_status*/
    ctx[2]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[41]
  );
  function imageuploader_uploading_binding(value) {
    ctx[44](value);
  }
  function imageuploader_active_source_binding(value) {
    ctx[45](value);
  }
  function imageuploader_value_binding(value) {
    ctx[46](value);
  }
  function imageuploader_dragging_binding(value) {
    ctx[47](value);
  }
  function imageuploader_modify_stream_binding(value) {
    ctx[48](value);
  }
  function imageuploader_set_time_limit_binding(value) {
    ctx[49](value);
  }
  let imageuploader_props = {
    selectable: (
      /*_selectable*/
      ctx[13]
    ),
    root: (
      /*root*/
      ctx[9]
    ),
    sources: (
      /*sources*/
      ctx[18]
    ),
    label: (
      /*label*/
      ctx[6]
    ),
    show_label: (
      /*show_label*/
      ctx[7]
    ),
    pending: (
      /*pending*/
      ctx[21]
    ),
    streaming: (
      /*streaming*/
      ctx[20]
    ),
    mirror_webcam: (
      /*mirror_webcam*/
      ctx[22]
    ),
    stream_every: (
      /*stream_every*/
      ctx[12]
    ),
    max_file_size: (
      /*gradio*/
      ctx[25].max_file_size
    ),
    i18n: (
      /*gradio*/
      ctx[25].i18n
    ),
    upload: (
      /*func*/
      ctx[42]
    ),
    stream_handler: (
      /*gradio*/
      (_a = ctx[25].client) == null ? void 0 : _a.stream
    ),
    $$slots: { default: [create_default_slot_2] },
    $$scope: { ctx }
  };
  if (
    /*uploading*/
    ctx[26] !== void 0
  ) {
    imageuploader_props.uploading = /*uploading*/
    ctx[26];
  }
  if (
    /*active_source*/
    ctx[29] !== void 0
  ) {
    imageuploader_props.active_source = /*active_source*/
    ctx[29];
  }
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    imageuploader_props.value = /*value*/
    ctx[0];
  }
  if (
    /*dragging*/
    ctx[28] !== void 0
  ) {
    imageuploader_props.dragging = /*dragging*/
    ctx[28];
  }
  if (
    /*_modify_stream*/
    ctx[27] !== void 0
  ) {
    imageuploader_props.modify_stream = /*_modify_stream*/
    ctx[27];
  }
  if (
    /*set_time_limit*/
    ctx[1] !== void 0
  ) {
    imageuploader_props.set_time_limit = /*set_time_limit*/
    ctx[1];
  }
  imageuploader = new ImageUploader({ props: imageuploader_props });
  ctx[43](imageuploader);
  binding_callbacks.push(() => bind(imageuploader, "uploading", imageuploader_uploading_binding));
  binding_callbacks.push(() => bind(imageuploader, "active_source", imageuploader_active_source_binding));
  binding_callbacks.push(() => bind(imageuploader, "value", imageuploader_value_binding));
  binding_callbacks.push(() => bind(imageuploader, "dragging", imageuploader_dragging_binding));
  binding_callbacks.push(() => bind(imageuploader, "modify_stream", imageuploader_modify_stream_binding));
  binding_callbacks.push(() => bind(imageuploader, "set_time_limit", imageuploader_set_time_limit_binding));
  imageuploader.$on(
    "edit",
    /*edit_handler*/
    ctx[50]
  );
  imageuploader.$on(
    "clear",
    /*clear_handler*/
    ctx[51]
  );
  imageuploader.$on(
    "stream",
    /*stream_handler*/
    ctx[52]
  );
  imageuploader.$on(
    "drag",
    /*drag_handler*/
    ctx[53]
  );
  imageuploader.$on(
    "upload",
    /*upload_handler*/
    ctx[54]
  );
  imageuploader.$on(
    "select",
    /*select_handler_1*/
    ctx[55]
  );
  imageuploader.$on(
    "share",
    /*share_handler_1*/
    ctx[56]
  );
  imageuploader.$on(
    "error",
    /*error_handler_1*/
    ctx[57]
  );
  imageuploader.$on(
    "close_stream",
    /*close_stream_handler*/
    ctx[58]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(imageuploader.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(imageuploader.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(imageuploader, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      33554436 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        33554432 && {
          autoscroll: (
            /*gradio*/
            ctx2[25].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        33554432 && { i18n: (
          /*gradio*/
          ctx2[25].i18n
        ) },
        dirty[0] & /*loading_status*/
        4 && get_spread_object(
          /*loading_status*/
          ctx2[2]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const imageuploader_changes = {};
      if (dirty[0] & /*_selectable*/
      8192)
        imageuploader_changes.selectable = /*_selectable*/
        ctx2[13];
      if (dirty[0] & /*root*/
      512)
        imageuploader_changes.root = /*root*/
        ctx2[9];
      if (dirty[0] & /*sources*/
      262144)
        imageuploader_changes.sources = /*sources*/
        ctx2[18];
      if (dirty[0] & /*label*/
      64)
        imageuploader_changes.label = /*label*/
        ctx2[6];
      if (dirty[0] & /*show_label*/
      128)
        imageuploader_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*pending*/
      2097152)
        imageuploader_changes.pending = /*pending*/
        ctx2[21];
      if (dirty[0] & /*streaming*/
      1048576)
        imageuploader_changes.streaming = /*streaming*/
        ctx2[20];
      if (dirty[0] & /*mirror_webcam*/
      4194304)
        imageuploader_changes.mirror_webcam = /*mirror_webcam*/
        ctx2[22];
      if (dirty[0] & /*stream_every*/
      4096)
        imageuploader_changes.stream_every = /*stream_every*/
        ctx2[12];
      if (dirty[0] & /*gradio*/
      33554432)
        imageuploader_changes.max_file_size = /*gradio*/
        ctx2[25].max_file_size;
      if (dirty[0] & /*gradio*/
      33554432)
        imageuploader_changes.i18n = /*gradio*/
        ctx2[25].i18n;
      if (dirty[0] & /*gradio*/
      33554432)
        imageuploader_changes.upload = /*func*/
        ctx2[42];
      if (dirty[0] & /*gradio*/
      33554432)
        imageuploader_changes.stream_handler = /*gradio*/
        (_a2 = ctx2[25].client) == null ? void 0 : _a2.stream;
      if (dirty[0] & /*gradio, placeholder, active_source*/
      578813952 | dirty[1] & /*$$scope*/
      536870912) {
        imageuploader_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      67108864) {
        updating_uploading = true;
        imageuploader_changes.uploading = /*uploading*/
        ctx2[26];
        add_flush_callback(() => updating_uploading = false);
      }
      if (!updating_active_source && dirty[0] & /*active_source*/
      536870912) {
        updating_active_source = true;
        imageuploader_changes.active_source = /*active_source*/
        ctx2[29];
        add_flush_callback(() => updating_active_source = false);
      }
      if (!updating_value && dirty[0] & /*value*/
      1) {
        updating_value = true;
        imageuploader_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      268435456) {
        updating_dragging = true;
        imageuploader_changes.dragging = /*dragging*/
        ctx2[28];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_modify_stream && dirty[0] & /*_modify_stream*/
      134217728) {
        updating_modify_stream = true;
        imageuploader_changes.modify_stream = /*_modify_stream*/
        ctx2[27];
        add_flush_callback(() => updating_modify_stream = false);
      }
      if (!updating_set_time_limit && dirty[0] & /*set_time_limit*/
      2) {
        updating_set_time_limit = true;
        imageuploader_changes.set_time_limit = /*set_time_limit*/
        ctx2[1];
        add_flush_callback(() => updating_set_time_limit = false);
      }
      imageuploader.$set(imageuploader_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(imageuploader.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(imageuploader.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      ctx[43](null);
      destroy_component(imageuploader, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t;
  let staticimage;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[25].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[25].i18n
    ) },
    /*loading_status*/
    ctx[2]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  staticimage = new ImagePreview({
    props: {
      value: (
        /*value*/
        ctx[0]
      ),
      label: (
        /*label*/
        ctx[6]
      ),
      show_label: (
        /*show_label*/
        ctx[7]
      ),
      show_download_button: (
        /*show_download_button*/
        ctx[8]
      ),
      selectable: (
        /*_selectable*/
        ctx[13]
      ),
      show_share_button: (
        /*show_share_button*/
        ctx[17]
      ),
      i18n: (
        /*gradio*/
        ctx[25].i18n
      ),
      show_fullscreen_button: (
        /*show_fullscreen_button*/
        ctx[24]
      )
    }
  });
  staticimage.$on(
    "select",
    /*select_handler*/
    ctx[38]
  );
  staticimage.$on(
    "share",
    /*share_handler*/
    ctx[39]
  );
  staticimage.$on(
    "error",
    /*error_handler*/
    ctx[40]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(staticimage.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(staticimage.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(staticimage, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      33554436 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        33554432 && {
          autoscroll: (
            /*gradio*/
            ctx2[25].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        33554432 && { i18n: (
          /*gradio*/
          ctx2[25].i18n
        ) },
        dirty[0] & /*loading_status*/
        4 && get_spread_object(
          /*loading_status*/
          ctx2[2]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const staticimage_changes = {};
      if (dirty[0] & /*value*/
      1)
        staticimage_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*label*/
      64)
        staticimage_changes.label = /*label*/
        ctx2[6];
      if (dirty[0] & /*show_label*/
      128)
        staticimage_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*show_download_button*/
      256)
        staticimage_changes.show_download_button = /*show_download_button*/
        ctx2[8];
      if (dirty[0] & /*_selectable*/
      8192)
        staticimage_changes.selectable = /*_selectable*/
        ctx2[13];
      if (dirty[0] & /*show_share_button*/
      131072)
        staticimage_changes.show_share_button = /*show_share_button*/
        ctx2[17];
      if (dirty[0] & /*gradio*/
      33554432)
        staticimage_changes.i18n = /*gradio*/
        ctx2[25].i18n;
      if (dirty[0] & /*show_fullscreen_button*/
      16777216)
        staticimage_changes.show_fullscreen_button = /*show_fullscreen_button*/
        ctx2[24];
      staticimage.$set(staticimage_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(staticimage.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(staticimage.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(staticimage, detaching);
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
    ctx2[19])
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
  let stream_state = "closed";
  let _modify_stream = () => {
  };
  function modify_stream_state(state) {
    stream_state = state;
    _modify_stream(state);
  }
  const get_stream_state = () => stream_state;
  let { set_time_limit } = $$props;
  let { value_is_output = false } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let old_value = null;
  let { label } = $$props;
  let { show_label } = $$props;
  let { show_download_button } = $$props;
  let { root } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { stream_every } = $$props;
  let { _selectable = false } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { show_share_button = false } = $$props;
  let { sources = ["upload", "clipboard", "webcam"] } = $$props;
  let { interactive } = $$props;
  let { streaming } = $$props;
  let { pending } = $$props;
  let { mirror_webcam } = $$props;
  let { placeholder = void 0 } = $$props;
  let { show_fullscreen_button } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let { gradio } = $$props;
  afterUpdate(() => {
    $$invalidate(33, value_is_output = false);
  });
  let dragging;
  let active_source = null;
  let upload_component;
  const handle_drag_event = (event) => {
    const drag_event = event;
    drag_event.preventDefault();
    drag_event.stopPropagation();
    if (drag_event.type === "dragenter" || drag_event.type === "dragover") {
      $$invalidate(28, dragging = true);
    } else if (drag_event.type === "dragleave") {
      $$invalidate(28, dragging = false);
    }
  };
  const handle_drop = (event) => {
    if (interactive) {
      const drop_event = event;
      drop_event.preventDefault();
      drop_event.stopPropagation();
      $$invalidate(28, dragging = false);
      if (upload_component) {
        upload_component.loadFilesFromDrop(drop_event);
      }
    }
  };
  const select_handler = ({ detail }) => gradio.dispatch("select", detail);
  const share_handler = ({ detail }) => gradio.dispatch("share", detail);
  const error_handler = ({ detail }) => gradio.dispatch("error", detail);
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const func = (...args) => gradio.client.upload(...args);
  function imageuploader_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      upload_component = $$value;
      $$invalidate(30, upload_component);
    });
  }
  function imageuploader_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(26, uploading);
  }
  function imageuploader_active_source_binding(value2) {
    active_source = value2;
    $$invalidate(29, active_source);
  }
  function imageuploader_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  function imageuploader_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(28, dragging);
  }
  function imageuploader_modify_stream_binding(value2) {
    _modify_stream = value2;
    $$invalidate(27, _modify_stream);
  }
  function imageuploader_set_time_limit_binding(value2) {
    set_time_limit = value2;
    $$invalidate(1, set_time_limit);
  }
  const edit_handler = () => gradio.dispatch("edit");
  const clear_handler = () => {
    gradio.dispatch("clear");
  };
  const stream_handler = ({ detail }) => gradio.dispatch("stream", detail);
  const drag_handler = ({ detail }) => $$invalidate(28, dragging = detail);
  const upload_handler = () => gradio.dispatch("upload");
  const select_handler_1 = ({ detail }) => gradio.dispatch("select", detail);
  const share_handler_1 = ({ detail }) => gradio.dispatch("share", detail);
  const error_handler_1 = ({ detail }) => {
    $$invalidate(2, loading_status = loading_status || {});
    $$invalidate(2, loading_status.status = "error", loading_status);
    gradio.dispatch("error", detail);
  };
  const close_stream_handler = () => {
    gradio.dispatch("close_stream", "stream");
  };
  $$self.$$set = ($$props2) => {
    if ("set_time_limit" in $$props2)
      $$invalidate(1, set_time_limit = $$props2.set_time_limit);
    if ("value_is_output" in $$props2)
      $$invalidate(33, value_is_output = $$props2.value_is_output);
    if ("elem_id" in $$props2)
      $$invalidate(3, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(4, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(5, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(6, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("show_download_button" in $$props2)
      $$invalidate(8, show_download_button = $$props2.show_download_button);
    if ("root" in $$props2)
      $$invalidate(9, root = $$props2.root);
    if ("height" in $$props2)
      $$invalidate(10, height = $$props2.height);
    if ("width" in $$props2)
      $$invalidate(11, width = $$props2.width);
    if ("stream_every" in $$props2)
      $$invalidate(12, stream_every = $$props2.stream_every);
    if ("_selectable" in $$props2)
      $$invalidate(13, _selectable = $$props2._selectable);
    if ("container" in $$props2)
      $$invalidate(14, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(15, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(16, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(2, loading_status = $$props2.loading_status);
    if ("show_share_button" in $$props2)
      $$invalidate(17, show_share_button = $$props2.show_share_button);
    if ("sources" in $$props2)
      $$invalidate(18, sources = $$props2.sources);
    if ("interactive" in $$props2)
      $$invalidate(19, interactive = $$props2.interactive);
    if ("streaming" in $$props2)
      $$invalidate(20, streaming = $$props2.streaming);
    if ("pending" in $$props2)
      $$invalidate(21, pending = $$props2.pending);
    if ("mirror_webcam" in $$props2)
      $$invalidate(22, mirror_webcam = $$props2.mirror_webcam);
    if ("placeholder" in $$props2)
      $$invalidate(23, placeholder = $$props2.placeholder);
    if ("show_fullscreen_button" in $$props2)
      $$invalidate(24, show_fullscreen_button = $$props2.show_fullscreen_button);
    if ("input_ready" in $$props2)
      $$invalidate(34, input_ready = $$props2.input_ready);
    if ("gradio" in $$props2)
      $$invalidate(25, gradio = $$props2.gradio);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*uploading*/
    67108864) {
      $$invalidate(34, input_ready = !uploading);
    }
    if ($$self.$$.dirty[0] & /*value, gradio*/
    33554433 | $$self.$$.dirty[1] & /*old_value, value_is_output*/
    68) {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          $$invalidate(37, old_value = value);
          gradio.dispatch("change");
          if (!value_is_output) {
            gradio.dispatch("input");
          }
        }
      }
    }
  };
  return [
    value,
    set_time_limit,
    loading_status,
    elem_id,
    elem_classes,
    visible,
    label,
    show_label,
    show_download_button,
    root,
    height,
    width,
    stream_every,
    _selectable,
    container,
    scale,
    min_width,
    show_share_button,
    sources,
    interactive,
    streaming,
    pending,
    mirror_webcam,
    placeholder,
    show_fullscreen_button,
    gradio,
    uploading,
    _modify_stream,
    dragging,
    active_source,
    upload_component,
    handle_drag_event,
    handle_drop,
    value_is_output,
    input_ready,
    modify_stream_state,
    get_stream_state,
    old_value,
    select_handler,
    share_handler,
    error_handler,
    clear_status_handler,
    func,
    imageuploader_binding,
    imageuploader_uploading_binding,
    imageuploader_active_source_binding,
    imageuploader_value_binding,
    imageuploader_dragging_binding,
    imageuploader_modify_stream_binding,
    imageuploader_set_time_limit_binding,
    edit_handler,
    clear_handler,
    stream_handler,
    drag_handler,
    upload_handler,
    select_handler_1,
    share_handler_1,
    error_handler_1,
    close_stream_handler
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
        modify_stream_state: 35,
        get_stream_state: 36,
        set_time_limit: 1,
        value_is_output: 33,
        elem_id: 3,
        elem_classes: 4,
        visible: 5,
        value: 0,
        label: 6,
        show_label: 7,
        show_download_button: 8,
        root: 9,
        height: 10,
        width: 11,
        stream_every: 12,
        _selectable: 13,
        container: 14,
        scale: 15,
        min_width: 16,
        loading_status: 2,
        show_share_button: 17,
        sources: 18,
        interactive: 19,
        streaming: 20,
        pending: 21,
        mirror_webcam: 22,
        placeholder: 23,
        show_fullscreen_button: 24,
        input_ready: 34,
        gradio: 25
      },
      null,
      [-1, -1]
    );
  }
  get modify_stream_state() {
    return this.$$.ctx[35];
  }
  get get_stream_state() {
    return this.$$.ctx[36];
  }
  get set_time_limit() {
    return this.$$.ctx[1];
  }
  set set_time_limit(set_time_limit) {
    this.$$set({ set_time_limit });
    flush();
  }
  get value_is_output() {
    return this.$$.ctx[33];
  }
  set value_is_output(value_is_output) {
    this.$$set({ value_is_output });
    flush();
  }
  get elem_id() {
    return this.$$.ctx[3];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[4];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[5];
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
    return this.$$.ctx[6];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get show_label() {
    return this.$$.ctx[7];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get show_download_button() {
    return this.$$.ctx[8];
  }
  set show_download_button(show_download_button) {
    this.$$set({ show_download_button });
    flush();
  }
  get root() {
    return this.$$.ctx[9];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get height() {
    return this.$$.ctx[10];
  }
  set height(height) {
    this.$$set({ height });
    flush();
  }
  get width() {
    return this.$$.ctx[11];
  }
  set width(width) {
    this.$$set({ width });
    flush();
  }
  get stream_every() {
    return this.$$.ctx[12];
  }
  set stream_every(stream_every) {
    this.$$set({ stream_every });
    flush();
  }
  get _selectable() {
    return this.$$.ctx[13];
  }
  set _selectable(_selectable) {
    this.$$set({ _selectable });
    flush();
  }
  get container() {
    return this.$$.ctx[14];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[15];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[16];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[2];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get show_share_button() {
    return this.$$.ctx[17];
  }
  set show_share_button(show_share_button) {
    this.$$set({ show_share_button });
    flush();
  }
  get sources() {
    return this.$$.ctx[18];
  }
  set sources(sources) {
    this.$$set({ sources });
    flush();
  }
  get interactive() {
    return this.$$.ctx[19];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get streaming() {
    return this.$$.ctx[20];
  }
  set streaming(streaming) {
    this.$$set({ streaming });
    flush();
  }
  get pending() {
    return this.$$.ctx[21];
  }
  set pending(pending) {
    this.$$set({ pending });
    flush();
  }
  get mirror_webcam() {
    return this.$$.ctx[22];
  }
  set mirror_webcam(mirror_webcam) {
    this.$$set({ mirror_webcam });
    flush();
  }
  get placeholder() {
    return this.$$.ctx[23];
  }
  set placeholder(placeholder) {
    this.$$set({ placeholder });
    flush();
  }
  get show_fullscreen_button() {
    return this.$$.ctx[24];
  }
  set show_fullscreen_button(show_fullscreen_button) {
    this.$$set({ show_fullscreen_button });
    flush();
  }
  get input_ready() {
    return this.$$.ctx[34];
  }
  set input_ready(input_ready) {
    this.$$set({ input_ready });
    flush();
  }
  get gradio() {
    return this.$$.ctx[25];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
}
export {
  default2 as BaseExample,
  I as BaseImage,
  ImageUploader as BaseImageUploader,
  ImagePreview as BaseStaticImage,
  W as Webcam,
  Index as default
};
