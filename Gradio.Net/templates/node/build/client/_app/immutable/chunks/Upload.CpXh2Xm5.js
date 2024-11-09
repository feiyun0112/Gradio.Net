import { SvelteComponent, init, safe_not_equal, element, text, space, claim_element, children, claim_text, claim_space, detach, attr, toggle_class, insert_hydration, append_hydration, set_data, noop, createEventDispatcher, onMount, onDestroy, set_style, empty, group_outros, transition_out, check_outros, transition_in, bubble, binding_callbacks, create_slot, listen, stop_propagation, prevent_default, update_slot_base, get_all_dirty_from_scope, get_slot_changes, run_all, create_component, claim_component, mount_component, destroy_component, tick } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { h as prepare_files } from "./2.BqWhUxOo.js";
/* empty css                                              */
function create_if_block$1(ctx) {
  let div1;
  let span0;
  let div0;
  let progress_1;
  let t0_value = getProgress(
    /*file_to_display*/
    ctx[2]
  ) + "";
  let t0;
  let progress_1_value_value;
  let t1;
  let span1;
  let t2_value = (
    /*file_to_display*/
    ctx[2].orig_name + ""
  );
  let t2;
  return {
    c() {
      div1 = element("div");
      span0 = element("span");
      div0 = element("div");
      progress_1 = element("progress");
      t0 = text(t0_value);
      t1 = space();
      span1 = element("span");
      t2 = text(t2_value);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      span0 = claim_element(div1_nodes, "SPAN", {});
      var span0_nodes = children(span0);
      div0 = claim_element(span0_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      progress_1 = claim_element(div0_nodes, "PROGRESS", { style: true, max: true, class: true });
      var progress_1_nodes = children(progress_1);
      t0 = claim_text(progress_1_nodes, t0_value);
      progress_1_nodes.forEach(detach);
      div0_nodes.forEach(detach);
      span0_nodes.forEach(detach);
      t1 = claim_space(div1_nodes);
      span1 = claim_element(div1_nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      t2 = claim_text(span1_nodes, t2_value);
      span1_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      set_style(progress_1, "visibility", "hidden");
      set_style(progress_1, "height", "0");
      set_style(progress_1, "width", "0");
      progress_1.value = progress_1_value_value = getProgress(
        /*file_to_display*/
        ctx[2]
      );
      attr(progress_1, "max", "100");
      attr(progress_1, "class", "svelte-1vsfomn");
      attr(div0, "class", "progress-bar svelte-1vsfomn");
      attr(span1, "class", "file-name svelte-1vsfomn");
      attr(div1, "class", "file svelte-1vsfomn");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, span0);
      append_hydration(span0, div0);
      append_hydration(div0, progress_1);
      append_hydration(progress_1, t0);
      append_hydration(div1, t1);
      append_hydration(div1, span1);
      append_hydration(span1, t2);
    },
    p(ctx2, dirty) {
      if (dirty & /*file_to_display*/
      4 && t0_value !== (t0_value = getProgress(
        /*file_to_display*/
        ctx2[2]
      ) + ""))
        set_data(t0, t0_value);
      if (dirty & /*file_to_display*/
      4 && progress_1_value_value !== (progress_1_value_value = getProgress(
        /*file_to_display*/
        ctx2[2]
      ))) {
        progress_1.value = progress_1_value_value;
      }
      if (dirty & /*file_to_display*/
      4 && t2_value !== (t2_value = /*file_to_display*/
      ctx2[2].orig_name + ""))
        set_data(t2, t2_value);
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
    }
  };
}
function create_fragment$1(ctx) {
  let div;
  let span;
  let t0;
  let t1_value = (
    /*files_with_progress*/
    ctx[0].length + ""
  );
  let t1;
  let t2;
  let t3_value = (
    /*files_with_progress*/
    ctx[0].length > 1 ? "files" : "file"
  );
  let t3;
  let t4;
  let t5;
  let if_block = (
    /*file_to_display*/
    ctx[2] && create_if_block$1(ctx)
  );
  return {
    c() {
      div = element("div");
      span = element("span");
      t0 = text("Uploading ");
      t1 = text(t1_value);
      t2 = space();
      t3 = text(t3_value);
      t4 = text("...");
      t5 = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      span = claim_element(div_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, "Uploading ");
      t1 = claim_text(span_nodes, t1_value);
      t2 = claim_space(span_nodes);
      t3 = claim_text(span_nodes, t3_value);
      t4 = claim_text(span_nodes, "...");
      span_nodes.forEach(detach);
      t5 = claim_space(div_nodes);
      if (if_block)
        if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "uploading svelte-1vsfomn");
      attr(div, "class", "wrap svelte-1vsfomn");
      toggle_class(
        div,
        "progress",
        /*progress*/
        ctx[1]
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, span);
      append_hydration(span, t0);
      append_hydration(span, t1);
      append_hydration(span, t2);
      append_hydration(span, t3);
      append_hydration(span, t4);
      append_hydration(div, t5);
      if (if_block)
        if_block.m(div, null);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*files_with_progress*/
      1 && t1_value !== (t1_value = /*files_with_progress*/
      ctx2[0].length + ""))
        set_data(t1, t1_value);
      if (dirty & /*files_with_progress*/
      1 && t3_value !== (t3_value = /*files_with_progress*/
      ctx2[0].length > 1 ? "files" : "file"))
        set_data(t3, t3_value);
      if (
        /*file_to_display*/
        ctx2[2]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block$1(ctx2);
          if_block.c();
          if_block.m(div, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (dirty & /*progress*/
      2) {
        toggle_class(
          div,
          "progress",
          /*progress*/
          ctx2[1]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (if_block)
        if_block.d();
    }
  };
}
function getProgress(file) {
  return file.progress * 100 / (file.size || 0) || 0;
}
function calculateTotalProgress(files2) {
  let totalProgress = 0;
  files2.forEach((file) => {
    totalProgress += getProgress(file);
  });
  document.documentElement.style.setProperty("--upload-progress-width", (totalProgress / files2.length).toFixed(2) + "%");
  return totalProgress / files2.length;
}
function instance$1($$self, $$props, $$invalidate) {
  let { upload_id } = $$props;
  let { root } = $$props;
  let { files } = $$props;
  let { stream_handler } = $$props;
  let stream;
  let progress = false;
  let current_file_upload;
  let file_to_display;
  let files_with_progress = files.map((file) => {
    return { ...file, progress: 0 };
  });
  const dispatch = createEventDispatcher();
  function handleProgress(filename, chunk_size) {
    $$invalidate(0, files_with_progress = files_with_progress.map((file) => {
      if (file.orig_name === filename) {
        file.progress += chunk_size;
      }
      return file;
    }));
  }
  onMount(async () => {
    stream = await stream_handler(new URL(`${root}/gradio_api/upload_progress?upload_id=${upload_id}`));
    if (stream == null) {
      throw new Error("Event source is not defined");
    }
    stream.onmessage = async function(event) {
      const _data = JSON.parse(event.data);
      if (!progress)
        $$invalidate(1, progress = true);
      if (_data.msg === "done") {
        stream == null ? void 0 : stream.close();
        dispatch("done");
      } else {
        $$invalidate(7, current_file_upload = _data);
        handleProgress(_data.orig_name, _data.chunk_size);
      }
    };
  });
  onDestroy(() => {
    if (stream != null || stream != void 0)
      stream.close();
  });
  $$self.$$set = ($$props2) => {
    if ("upload_id" in $$props2)
      $$invalidate(3, upload_id = $$props2.upload_id);
    if ("root" in $$props2)
      $$invalidate(4, root = $$props2.root);
    if ("files" in $$props2)
      $$invalidate(5, files = $$props2.files);
    if ("stream_handler" in $$props2)
      $$invalidate(6, stream_handler = $$props2.stream_handler);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*files_with_progress*/
    1) {
      calculateTotalProgress(files_with_progress);
    }
    if ($$self.$$.dirty & /*current_file_upload, files_with_progress*/
    129) {
      $$invalidate(2, file_to_display = current_file_upload || files_with_progress[0]);
    }
  };
  return [
    files_with_progress,
    progress,
    file_to_display,
    upload_id,
    root,
    files,
    stream_handler,
    current_file_upload
  ];
}
class UploadProgress extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      upload_id: 3,
      root: 4,
      files: 5,
      stream_handler: 6
    });
  }
}
function create_else_block(ctx) {
  let button;
  let t;
  let input;
  let input_accept_value;
  let input_multiple_value;
  let input_webkitdirectory_value;
  let input_mozdirectory_value;
  let button_tabindex_value;
  let current;
  let mounted;
  let dispose;
  const default_slot_template = (
    /*#slots*/
    ctx[28].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[27],
    null
  );
  return {
    c() {
      button = element("button");
      if (default_slot)
        default_slot.c();
      t = space();
      input = element("input");
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { tabindex: true, class: true });
      var button_nodes = children(button);
      if (default_slot)
        default_slot.l(button_nodes);
      t = claim_space(button_nodes);
      input = claim_element(button_nodes, "INPUT", {
        "aria-label": true,
        "data-testid": true,
        type: true,
        accept: true,
        webkitdirectory: true,
        mozdirectory: true,
        class: true
      });
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(input, "aria-label", "file upload");
      attr(input, "data-testid", "file-upload");
      attr(input, "type", "file");
      attr(input, "accept", input_accept_value = /*accept_file_types*/
      ctx[17] || void 0);
      input.multiple = input_multiple_value = /*file_count*/
      ctx[6] === "multiple" || void 0;
      attr(input, "webkitdirectory", input_webkitdirectory_value = /*file_count*/
      ctx[6] === "directory" || void 0);
      attr(input, "mozdirectory", input_mozdirectory_value = /*file_count*/
      ctx[6] === "directory" || void 0);
      attr(input, "class", "svelte-1b742ao");
      attr(button, "tabindex", button_tabindex_value = /*hidden*/
      ctx[9] ? -1 : 0);
      attr(button, "class", "svelte-1b742ao");
      toggle_class(
        button,
        "hidden",
        /*hidden*/
        ctx[9]
      );
      toggle_class(
        button,
        "center",
        /*center*/
        ctx[4]
      );
      toggle_class(
        button,
        "boundedheight",
        /*boundedheight*/
        ctx[3]
      );
      toggle_class(
        button,
        "flex",
        /*flex*/
        ctx[5]
      );
      toggle_class(
        button,
        "disable_click",
        /*disable_click*/
        ctx[7]
      );
      toggle_class(
        button,
        "icon-mode",
        /*icon_upload*/
        ctx[12]
      );
      set_style(
        button,
        "height",
        /*icon_upload*/
        ctx[12] ? "" : "100%"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if (default_slot) {
        default_slot.m(button, null);
      }
      append_hydration(button, t);
      append_hydration(button, input);
      ctx[36](input);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            input,
            "change",
            /*load_files_from_upload*/
            ctx[19]
          ),
          listen(button, "drag", stop_propagation(prevent_default(
            /*drag_handler*/
            ctx[29]
          ))),
          listen(button, "dragstart", stop_propagation(prevent_default(
            /*dragstart_handler*/
            ctx[30]
          ))),
          listen(button, "dragend", stop_propagation(prevent_default(
            /*dragend_handler*/
            ctx[31]
          ))),
          listen(button, "dragover", stop_propagation(prevent_default(
            /*dragover_handler*/
            ctx[32]
          ))),
          listen(button, "dragenter", stop_propagation(prevent_default(
            /*dragenter_handler*/
            ctx[33]
          ))),
          listen(button, "dragleave", stop_propagation(prevent_default(
            /*dragleave_handler*/
            ctx[34]
          ))),
          listen(button, "drop", stop_propagation(prevent_default(
            /*drop_handler*/
            ctx[35]
          ))),
          listen(
            button,
            "click",
            /*open_file_upload*/
            ctx[14]
          ),
          listen(
            button,
            "drop",
            /*loadFilesFromDrop*/
            ctx[20]
          ),
          listen(
            button,
            "dragenter",
            /*updateDragging*/
            ctx[18]
          ),
          listen(
            button,
            "dragleave",
            /*updateDragging*/
            ctx[18]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty[0] & /*$$scope*/
        134217728)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[27],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[27]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[27],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty[0] & /*accept_file_types*/
      131072 && input_accept_value !== (input_accept_value = /*accept_file_types*/
      ctx2[17] || void 0)) {
        attr(input, "accept", input_accept_value);
      }
      if (!current || dirty[0] & /*file_count*/
      64 && input_multiple_value !== (input_multiple_value = /*file_count*/
      ctx2[6] === "multiple" || void 0)) {
        input.multiple = input_multiple_value;
      }
      if (!current || dirty[0] & /*file_count*/
      64 && input_webkitdirectory_value !== (input_webkitdirectory_value = /*file_count*/
      ctx2[6] === "directory" || void 0)) {
        attr(input, "webkitdirectory", input_webkitdirectory_value);
      }
      if (!current || dirty[0] & /*file_count*/
      64 && input_mozdirectory_value !== (input_mozdirectory_value = /*file_count*/
      ctx2[6] === "directory" || void 0)) {
        attr(input, "mozdirectory", input_mozdirectory_value);
      }
      if (!current || dirty[0] & /*hidden*/
      512 && button_tabindex_value !== (button_tabindex_value = /*hidden*/
      ctx2[9] ? -1 : 0)) {
        attr(button, "tabindex", button_tabindex_value);
      }
      if (!current || dirty[0] & /*hidden*/
      512) {
        toggle_class(
          button,
          "hidden",
          /*hidden*/
          ctx2[9]
        );
      }
      if (!current || dirty[0] & /*center*/
      16) {
        toggle_class(
          button,
          "center",
          /*center*/
          ctx2[4]
        );
      }
      if (!current || dirty[0] & /*boundedheight*/
      8) {
        toggle_class(
          button,
          "boundedheight",
          /*boundedheight*/
          ctx2[3]
        );
      }
      if (!current || dirty[0] & /*flex*/
      32) {
        toggle_class(
          button,
          "flex",
          /*flex*/
          ctx2[5]
        );
      }
      if (!current || dirty[0] & /*disable_click*/
      128) {
        toggle_class(
          button,
          "disable_click",
          /*disable_click*/
          ctx2[7]
        );
      }
      if (!current || dirty[0] & /*icon_upload*/
      4096) {
        toggle_class(
          button,
          "icon-mode",
          /*icon_upload*/
          ctx2[12]
        );
      }
      if (dirty[0] & /*icon_upload*/
      4096) {
        set_style(
          button,
          "height",
          /*icon_upload*/
          ctx2[12] ? "" : "100%"
        );
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
      if (detaching) {
        detach(button);
      }
      if (default_slot)
        default_slot.d(detaching);
      ctx[36](null);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_1(ctx) {
  let if_block_anchor;
  let current;
  let if_block = !/*hidden*/
  ctx[9] && create_if_block_2(ctx);
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
    p(ctx2, dirty) {
      if (!/*hidden*/
      ctx2[9]) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*hidden*/
          512) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_2(ctx2);
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
function create_if_block(ctx) {
  let button;
  let button_tabindex_value;
  let current;
  let mounted;
  let dispose;
  const default_slot_template = (
    /*#slots*/
    ctx[28].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[27],
    null
  );
  return {
    c() {
      button = element("button");
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { tabindex: true, class: true });
      var button_nodes = children(button);
      if (default_slot)
        default_slot.l(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "tabindex", button_tabindex_value = /*hidden*/
      ctx[9] ? -1 : 0);
      attr(button, "class", "svelte-1b742ao");
      toggle_class(
        button,
        "hidden",
        /*hidden*/
        ctx[9]
      );
      toggle_class(
        button,
        "center",
        /*center*/
        ctx[4]
      );
      toggle_class(
        button,
        "boundedheight",
        /*boundedheight*/
        ctx[3]
      );
      toggle_class(
        button,
        "flex",
        /*flex*/
        ctx[5]
      );
      toggle_class(
        button,
        "icon-mode",
        /*icon_upload*/
        ctx[12]
      );
      set_style(
        button,
        "height",
        /*icon_upload*/
        ctx[12] ? "" : "100%"
      );
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if (default_slot) {
        default_slot.m(button, null);
      }
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*paste_clipboard*/
          ctx[13]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty[0] & /*$$scope*/
        134217728)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[27],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[27]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[27],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty[0] & /*hidden*/
      512 && button_tabindex_value !== (button_tabindex_value = /*hidden*/
      ctx2[9] ? -1 : 0)) {
        attr(button, "tabindex", button_tabindex_value);
      }
      if (!current || dirty[0] & /*hidden*/
      512) {
        toggle_class(
          button,
          "hidden",
          /*hidden*/
          ctx2[9]
        );
      }
      if (!current || dirty[0] & /*center*/
      16) {
        toggle_class(
          button,
          "center",
          /*center*/
          ctx2[4]
        );
      }
      if (!current || dirty[0] & /*boundedheight*/
      8) {
        toggle_class(
          button,
          "boundedheight",
          /*boundedheight*/
          ctx2[3]
        );
      }
      if (!current || dirty[0] & /*flex*/
      32) {
        toggle_class(
          button,
          "flex",
          /*flex*/
          ctx2[5]
        );
      }
      if (!current || dirty[0] & /*icon_upload*/
      4096) {
        toggle_class(
          button,
          "icon-mode",
          /*icon_upload*/
          ctx2[12]
        );
      }
      if (dirty[0] & /*icon_upload*/
      4096) {
        set_style(
          button,
          "height",
          /*icon_upload*/
          ctx2[12] ? "" : "100%"
        );
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
      if (detaching) {
        detach(button);
      }
      if (default_slot)
        default_slot.d(detaching);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_2(ctx) {
  let uploadprogress;
  let current;
  uploadprogress = new UploadProgress({
    props: {
      root: (
        /*root*/
        ctx[8]
      ),
      upload_id: (
        /*upload_id*/
        ctx[15]
      ),
      files: (
        /*file_data*/
        ctx[16]
      ),
      stream_handler: (
        /*stream_handler*/
        ctx[11]
      )
    }
  });
  return {
    c() {
      create_component(uploadprogress.$$.fragment);
    },
    l(nodes) {
      claim_component(uploadprogress.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(uploadprogress, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const uploadprogress_changes = {};
      if (dirty[0] & /*root*/
      256)
        uploadprogress_changes.root = /*root*/
        ctx2[8];
      if (dirty[0] & /*upload_id*/
      32768)
        uploadprogress_changes.upload_id = /*upload_id*/
        ctx2[15];
      if (dirty[0] & /*file_data*/
      65536)
        uploadprogress_changes.files = /*file_data*/
        ctx2[16];
      if (dirty[0] & /*stream_handler*/
      2048)
        uploadprogress_changes.stream_handler = /*stream_handler*/
        ctx2[11];
      uploadprogress.$set(uploadprogress_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(uploadprogress.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(uploadprogress.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(uploadprogress, detaching);
    }
  };
}
function create_fragment(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_if_block_1, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*filetype*/
      ctx2[0] === "clipboard"
    )
      return 0;
    if (
      /*uploading*/
      ctx2[1] && /*show_progress*/
      ctx2[10]
    )
      return 1;
    return 2;
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
function is_valid_mimetype(file_accept, uploaded_file_extension, uploaded_file_type) {
  if (!file_accept || file_accept === "*" || file_accept === "file/*" || Array.isArray(file_accept) && file_accept.some((accept) => accept === "*" || accept === "file/*")) {
    return true;
  }
  let acceptArray;
  if (typeof file_accept === "string") {
    acceptArray = file_accept.split(",").map((s) => s.trim());
  } else if (Array.isArray(file_accept)) {
    acceptArray = file_accept;
  } else {
    return false;
  }
  return acceptArray.includes(uploaded_file_extension) || acceptArray.some((type) => {
    const [category] = type.split("/").map((s) => s.trim());
    return type.endsWith("/*") && uploaded_file_type.startsWith(category + "/");
  });
}
function instance($$self, $$props, $$invalidate) {
  let ios;
  let { $$slots: slots = {}, $$scope } = $$props;
  let { filetype = null } = $$props;
  let { dragging = false } = $$props;
  let { boundedheight = true } = $$props;
  let { center = true } = $$props;
  let { flex = true } = $$props;
  let { file_count = "single" } = $$props;
  let { disable_click = false } = $$props;
  let { root } = $$props;
  let { hidden = false } = $$props;
  let { format = "file" } = $$props;
  let { uploading = false } = $$props;
  let { hidden_upload = null } = $$props;
  let { show_progress = true } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { icon_upload = false } = $$props;
  let upload_id;
  let file_data;
  let accept_file_types;
  let use_post_upload_validation = null;
  const get_ios = () => {
    if (typeof navigator !== "undefined") {
      const userAgent = navigator.userAgent.toLowerCase();
      return userAgent.indexOf("iphone") > -1 || userAgent.indexOf("ipad") > -1;
    }
    return false;
  };
  const dispatch = createEventDispatcher();
  const validFileTypes = ["image", "video", "audio", "text", "file"];
  const process_file_type = (type) => {
    if (ios && type.startsWith(".")) {
      use_post_upload_validation = true;
      return type;
    }
    if (ios && type.includes("file/*")) {
      return "*";
    }
    if (type.startsWith(".") || type.endsWith("/*")) {
      return type;
    }
    if (validFileTypes.includes(type)) {
      return type + "/*";
    }
    return "." + type;
  };
  function updateDragging() {
    $$invalidate(21, dragging = !dragging);
  }
  function paste_clipboard() {
    navigator.clipboard.read().then(async (items) => {
      for (let i = 0; i < items.length; i++) {
        const type = items[i].types.find((t) => t.startsWith("image/"));
        if (type) {
          items[i].getType(type).then(async (blob) => {
            const file = new File([blob], `clipboard.${type.replace("image/", "")}`);
            await load_files([file]);
          });
          break;
        }
      }
    });
  }
  function open_file_upload() {
    if (disable_click)
      return;
    if (hidden_upload) {
      $$invalidate(2, hidden_upload.value = "", hidden_upload);
      hidden_upload.click();
    }
  }
  async function handle_upload(file_data2) {
    await tick();
    $$invalidate(15, upload_id = Math.random().toString(36).substring(2, 15));
    $$invalidate(1, uploading = true);
    try {
      const _file_data = await upload(file_data2, root, upload_id, max_file_size ?? Infinity);
      dispatch("load", file_count === "single" ? _file_data == null ? void 0 : _file_data[0] : _file_data);
      $$invalidate(1, uploading = false);
      return _file_data || [];
    } catch (e) {
      dispatch("error", e.message);
      $$invalidate(1, uploading = false);
      return [];
    }
  }
  async function load_files(files) {
    if (!files.length) {
      return;
    }
    let _files = files.map((f) => new File([f], f instanceof File ? f.name : "file", { type: f.type }));
    if (ios && use_post_upload_validation) {
      _files = _files.filter((file) => {
        if (is_valid_file(file)) {
          return true;
        }
        dispatch("error", `Invalid file type: ${file.name}. Only ${filetype} allowed.`);
        return false;
      });
      if (_files.length === 0) {
        return [];
      }
    }
    $$invalidate(16, file_data = await prepare_files(_files));
    return await handle_upload(file_data);
  }
  function is_valid_file(file) {
    if (!filetype)
      return true;
    const allowed_types = Array.isArray(filetype) ? filetype : [filetype];
    return allowed_types.some((type) => {
      const processed_type = process_file_type(type);
      if (processed_type.startsWith(".")) {
        return file.name.toLowerCase().endsWith(processed_type.toLowerCase());
      }
      if (processed_type === "*") {
        return true;
      }
      if (processed_type.endsWith("/*")) {
        const [category] = processed_type.split("/");
        return file.type.startsWith(category + "/");
      }
      return file.type === processed_type;
    });
  }
  async function load_files_from_upload(e) {
    const target = e.target;
    if (!target.files)
      return;
    if (format != "blob") {
      await load_files(Array.from(target.files));
    } else {
      if (file_count === "single") {
        dispatch("load", target.files[0]);
        return;
      }
      dispatch("load", target.files);
    }
  }
  async function loadFilesFromDrop(e) {
    var _a;
    $$invalidate(21, dragging = false);
    if (!((_a = e.dataTransfer) == null ? void 0 : _a.files))
      return;
    const files_to_load = Array.from(e.dataTransfer.files).filter((file) => {
      const file_extension = "." + file.name.split(".").pop();
      if (file_extension && is_valid_mimetype(accept_file_types, file_extension, file.type)) {
        return true;
      }
      if (file_extension && Array.isArray(filetype) ? filetype.includes(file_extension) : file_extension === filetype) {
        return true;
      }
      dispatch("error", `Invalid file type only ${filetype} allowed.`);
      return false;
    });
    if (format != "blob") {
      await load_files(files_to_load);
    } else {
      if (file_count === "single") {
        dispatch("load", files_to_load[0]);
        return;
      }
      dispatch("load", files_to_load);
    }
  }
  function drag_handler(event) {
    bubble.call(this, $$self, event);
  }
  function dragstart_handler(event) {
    bubble.call(this, $$self, event);
  }
  function dragend_handler(event) {
    bubble.call(this, $$self, event);
  }
  function dragover_handler(event) {
    bubble.call(this, $$self, event);
  }
  function dragenter_handler(event) {
    bubble.call(this, $$self, event);
  }
  function dragleave_handler(event) {
    bubble.call(this, $$self, event);
  }
  function drop_handler(event) {
    bubble.call(this, $$self, event);
  }
  function input_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      hidden_upload = $$value;
      $$invalidate(2, hidden_upload);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("filetype" in $$props2)
      $$invalidate(0, filetype = $$props2.filetype);
    if ("dragging" in $$props2)
      $$invalidate(21, dragging = $$props2.dragging);
    if ("boundedheight" in $$props2)
      $$invalidate(3, boundedheight = $$props2.boundedheight);
    if ("center" in $$props2)
      $$invalidate(4, center = $$props2.center);
    if ("flex" in $$props2)
      $$invalidate(5, flex = $$props2.flex);
    if ("file_count" in $$props2)
      $$invalidate(6, file_count = $$props2.file_count);
    if ("disable_click" in $$props2)
      $$invalidate(7, disable_click = $$props2.disable_click);
    if ("root" in $$props2)
      $$invalidate(8, root = $$props2.root);
    if ("hidden" in $$props2)
      $$invalidate(9, hidden = $$props2.hidden);
    if ("format" in $$props2)
      $$invalidate(22, format = $$props2.format);
    if ("uploading" in $$props2)
      $$invalidate(1, uploading = $$props2.uploading);
    if ("hidden_upload" in $$props2)
      $$invalidate(2, hidden_upload = $$props2.hidden_upload);
    if ("show_progress" in $$props2)
      $$invalidate(10, show_progress = $$props2.show_progress);
    if ("max_file_size" in $$props2)
      $$invalidate(23, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(24, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(11, stream_handler = $$props2.stream_handler);
    if ("icon_upload" in $$props2)
      $$invalidate(12, icon_upload = $$props2.icon_upload);
    if ("$$scope" in $$props2)
      $$invalidate(27, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*filetype, ios*/
    67108865) {
      if (filetype == null) {
        $$invalidate(17, accept_file_types = null);
      } else if (typeof filetype === "string") {
        $$invalidate(17, accept_file_types = process_file_type(filetype));
      } else if (ios && filetype.includes("file/*")) {
        $$invalidate(17, accept_file_types = "*");
      } else {
        $$invalidate(0, filetype = filetype.map(process_file_type));
        $$invalidate(17, accept_file_types = filetype.join(", "));
      }
    }
  };
  $$invalidate(26, ios = get_ios());
  return [
    filetype,
    uploading,
    hidden_upload,
    boundedheight,
    center,
    flex,
    file_count,
    disable_click,
    root,
    hidden,
    show_progress,
    stream_handler,
    icon_upload,
    paste_clipboard,
    open_file_upload,
    upload_id,
    file_data,
    accept_file_types,
    updateDragging,
    load_files_from_upload,
    loadFilesFromDrop,
    dragging,
    format,
    max_file_size,
    upload,
    load_files,
    ios,
    $$scope,
    slots,
    drag_handler,
    dragstart_handler,
    dragend_handler,
    dragover_handler,
    dragenter_handler,
    dragleave_handler,
    drop_handler,
    input_binding
  ];
}
class Upload extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        filetype: 0,
        dragging: 21,
        boundedheight: 3,
        center: 4,
        flex: 5,
        file_count: 6,
        disable_click: 7,
        root: 8,
        hidden: 9,
        format: 22,
        uploading: 1,
        hidden_upload: 2,
        show_progress: 10,
        max_file_size: 23,
        upload: 24,
        stream_handler: 11,
        icon_upload: 12,
        paste_clipboard: 13,
        open_file_upload: 14,
        load_files: 25
      },
      null,
      [-1, -1]
    );
  }
  get paste_clipboard() {
    return this.$$.ctx[13];
  }
  get open_file_upload() {
    return this.$$.ctx[14];
  }
  get load_files() {
    return this.$$.ctx[25];
  }
}
export {
  Upload as U
};
