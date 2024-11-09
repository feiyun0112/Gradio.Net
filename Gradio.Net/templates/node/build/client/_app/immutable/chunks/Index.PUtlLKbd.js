import { SvelteComponent, init, safe_not_equal, flush, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, assign, space, empty, claim_space, insert_hydration, get_spread_update, get_spread_object, group_outros, check_outros, detach, binding_callbacks, bind, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as BaseFileUpload, F as File } from "./FileUpload.CLCgRYp8.js";
import { a } from "./FileUpload.CLCgRYp8.js";
import { B as Block, S as Static } from "./2.BqWhUxOo.js";
import { U as UploadText } from "./UploadText.BG6Hywul.js";
import { default as default2 } from "./Example.DBkrk8px.js";
function create_else_block(ctx) {
  let fileupload;
  let updating_uploading;
  let current;
  function fileupload_uploading_binding(value) {
    ctx[26](value);
  }
  let fileupload_props = {
    upload: (
      /*func*/
      ctx[24]
    ),
    stream_handler: (
      /*func_1*/
      ctx[25]
    ),
    label: (
      /*label*/
      ctx[7]
    ),
    show_label: (
      /*show_label*/
      ctx[8]
    ),
    value: (
      /*value*/
      ctx[0]
    ),
    file_count: (
      /*file_count*/
      ctx[15]
    ),
    file_types: (
      /*file_types*/
      ctx[16]
    ),
    selectable: (
      /*_selectable*/
      ctx[10]
    ),
    root: (
      /*root*/
      ctx[6]
    ),
    height: (
      /*height*/
      ctx[9]
    ),
    max_file_size: (
      /*gradio*/
      ctx[14].max_file_size
    ),
    i18n: (
      /*gradio*/
      ctx[14].i18n
    ),
    $$slots: { default: [create_default_slot_1] },
    $$scope: { ctx }
  };
  if (
    /*uploading*/
    ctx[17] !== void 0
  ) {
    fileupload_props.uploading = /*uploading*/
    ctx[17];
  }
  fileupload = new BaseFileUpload({ props: fileupload_props });
  binding_callbacks.push(() => bind(fileupload, "uploading", fileupload_uploading_binding));
  fileupload.$on(
    "change",
    /*change_handler*/
    ctx[27]
  );
  fileupload.$on(
    "drag",
    /*drag_handler*/
    ctx[28]
  );
  fileupload.$on(
    "clear",
    /*clear_handler*/
    ctx[29]
  );
  fileupload.$on(
    "select",
    /*select_handler_1*/
    ctx[30]
  );
  fileupload.$on(
    "upload",
    /*upload_handler*/
    ctx[31]
  );
  fileupload.$on(
    "error",
    /*error_handler*/
    ctx[32]
  );
  fileupload.$on(
    "delete",
    /*delete_handler*/
    ctx[33]
  );
  return {
    c() {
      create_component(fileupload.$$.fragment);
    },
    l(nodes) {
      claim_component(fileupload.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(fileupload, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const fileupload_changes = {};
      if (dirty[0] & /*gradio*/
      16384)
        fileupload_changes.upload = /*func*/
        ctx2[24];
      if (dirty[0] & /*gradio*/
      16384)
        fileupload_changes.stream_handler = /*func_1*/
        ctx2[25];
      if (dirty[0] & /*label*/
      128)
        fileupload_changes.label = /*label*/
        ctx2[7];
      if (dirty[0] & /*show_label*/
      256)
        fileupload_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*value*/
      1)
        fileupload_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*file_count*/
      32768)
        fileupload_changes.file_count = /*file_count*/
        ctx2[15];
      if (dirty[0] & /*file_types*/
      65536)
        fileupload_changes.file_types = /*file_types*/
        ctx2[16];
      if (dirty[0] & /*_selectable*/
      1024)
        fileupload_changes.selectable = /*_selectable*/
        ctx2[10];
      if (dirty[0] & /*root*/
      64)
        fileupload_changes.root = /*root*/
        ctx2[6];
      if (dirty[0] & /*height*/
      512)
        fileupload_changes.height = /*height*/
        ctx2[9];
      if (dirty[0] & /*gradio*/
      16384)
        fileupload_changes.max_file_size = /*gradio*/
        ctx2[14].max_file_size;
      if (dirty[0] & /*gradio*/
      16384)
        fileupload_changes.i18n = /*gradio*/
        ctx2[14].i18n;
      if (dirty[0] & /*gradio*/
      16384 | dirty[1] & /*$$scope*/
      8) {
        fileupload_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      131072) {
        updating_uploading = true;
        fileupload_changes.uploading = /*uploading*/
        ctx2[17];
        add_flush_callback(() => updating_uploading = false);
      }
      fileupload.$set(fileupload_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(fileupload.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(fileupload.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(fileupload, detaching);
    }
  };
}
function create_if_block(ctx) {
  let file;
  let current;
  file = new File({
    props: {
      selectable: (
        /*_selectable*/
        ctx[10]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      label: (
        /*label*/
        ctx[7]
      ),
      show_label: (
        /*show_label*/
        ctx[8]
      ),
      height: (
        /*height*/
        ctx[9]
      ),
      i18n: (
        /*gradio*/
        ctx[14].i18n
      )
    }
  });
  file.$on(
    "select",
    /*select_handler*/
    ctx[22]
  );
  file.$on(
    "download",
    /*download_handler*/
    ctx[23]
  );
  return {
    c() {
      create_component(file.$$.fragment);
    },
    l(nodes) {
      claim_component(file.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(file, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const file_changes = {};
      if (dirty[0] & /*_selectable*/
      1024)
        file_changes.selectable = /*_selectable*/
        ctx2[10];
      if (dirty[0] & /*value*/
      1)
        file_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*label*/
      128)
        file_changes.label = /*label*/
        ctx2[7];
      if (dirty[0] & /*show_label*/
      256)
        file_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*height*/
      512)
        file_changes.height = /*height*/
        ctx2[9];
      if (dirty[0] & /*gradio*/
      16384)
        file_changes.i18n = /*gradio*/
        ctx2[14].i18n;
      file.$set(file_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(file.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(file.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(file, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let uploadtext;
  let current;
  uploadtext = new UploadText({
    props: {
      i18n: (
        /*gradio*/
        ctx[14].i18n
      ),
      type: "file"
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
      16384)
        uploadtext_changes.i18n = /*gradio*/
        ctx2[14].i18n;
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
function create_default_slot(ctx) {
  var _a;
  let statustracker;
  let t;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[14].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[14].i18n
    ) },
    /*loading_status*/
    ctx[1],
    {
      status: (
        /*loading_status*/
        ((_a = ctx[1]) == null ? void 0 : _a.status) || "complete"
      )
    }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[21]
  );
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (!/*interactive*/
    ctx2[5])
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      16386 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        16384 && {
          autoscroll: (
            /*gradio*/
            ctx2[14].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        16384 && { i18n: (
          /*gradio*/
          ctx2[14].i18n
        ) },
        dirty[0] & /*loading_status*/
        2 && get_spread_object(
          /*loading_status*/
          ctx2[1]
        ),
        dirty[0] & /*loading_status*/
        2 && {
          status: (
            /*loading_status*/
            ((_a2 = ctx2[1]) == null ? void 0 : _a2.status) || "complete"
          )
        }
      ]) : {};
      statustracker.$set(statustracker_changes);
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
      transition_in(statustracker.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_component(statustracker, detaching);
      if_blocks[current_block_type_index].d(detaching);
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
        ctx[4]
      ),
      variant: (
        /*value*/
        ctx[0] ? "solid" : "dashed"
      ),
      border_mode: (
        /*dragging*/
        ctx[18] ? "focus" : "base"
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
      if (dirty[0] & /*value*/
      1)
        block_changes.variant = /*value*/
        ctx2[0] ? "solid" : "dashed";
      if (dirty[0] & /*dragging*/
      262144)
        block_changes.border_mode = /*dragging*/
        ctx2[18] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      4)
        block_changes.elem_id = /*elem_id*/
        ctx2[2];
      if (dirty[0] & /*elem_classes*/
      8)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[3];
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
      if (dirty[0] & /*_selectable, value, label, show_label, height, gradio, interactive, file_count, file_types, root, uploading, dragging, loading_status*/
      509923 | dirty[1] & /*$$scope*/
      8) {
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
  let { value } = $$props;
  let { interactive } = $$props;
  let { root } = $$props;
  let { label } = $$props;
  let { show_label } = $$props;
  let { height = void 0 } = $$props;
  let { _selectable = false } = $$props;
  let { loading_status } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { file_count } = $$props;
  let { file_types = ["file"] } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let old_value = value;
  let dragging = false;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const select_handler = ({ detail }) => gradio.dispatch("select", detail);
  const download_handler = ({ detail }) => gradio.dispatch("download", detail);
  const func = (...args) => gradio.client.upload(...args);
  const func_1 = (...args) => gradio.client.stream(...args);
  function fileupload_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(17, uploading);
  }
  const change_handler = ({ detail }) => {
    $$invalidate(0, value = detail);
  };
  const drag_handler = ({ detail }) => $$invalidate(18, dragging = detail);
  const clear_handler = () => gradio.dispatch("clear");
  const select_handler_1 = ({ detail }) => gradio.dispatch("select", detail);
  const upload_handler = () => gradio.dispatch("upload");
  const error_handler = ({ detail }) => {
    $$invalidate(1, loading_status = loading_status || {});
    $$invalidate(1, loading_status.status = "error", loading_status);
    gradio.dispatch("error", detail);
  };
  const delete_handler = ({ detail }) => {
    gradio.dispatch("delete", detail);
  };
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(2, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(3, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(4, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("interactive" in $$props2)
      $$invalidate(5, interactive = $$props2.interactive);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("label" in $$props2)
      $$invalidate(7, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(8, show_label = $$props2.show_label);
    if ("height" in $$props2)
      $$invalidate(9, height = $$props2.height);
    if ("_selectable" in $$props2)
      $$invalidate(10, _selectable = $$props2._selectable);
    if ("loading_status" in $$props2)
      $$invalidate(1, loading_status = $$props2.loading_status);
    if ("container" in $$props2)
      $$invalidate(11, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(12, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(13, min_width = $$props2.min_width);
    if ("gradio" in $$props2)
      $$invalidate(14, gradio = $$props2.gradio);
    if ("file_count" in $$props2)
      $$invalidate(15, file_count = $$props2.file_count);
    if ("file_types" in $$props2)
      $$invalidate(16, file_types = $$props2.file_types);
    if ("input_ready" in $$props2)
      $$invalidate(19, input_ready = $$props2.input_ready);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*uploading*/
    131072) {
      $$invalidate(19, input_ready = !uploading);
    }
    if ($$self.$$.dirty[0] & /*old_value, value, gradio*/
    1064961) {
      if (JSON.stringify(old_value) !== JSON.stringify(value)) {
        gradio.dispatch("change");
        $$invalidate(20, old_value = value);
      }
    }
  };
  return [
    value,
    loading_status,
    elem_id,
    elem_classes,
    visible,
    interactive,
    root,
    label,
    show_label,
    height,
    _selectable,
    container,
    scale,
    min_width,
    gradio,
    file_count,
    file_types,
    uploading,
    dragging,
    input_ready,
    old_value,
    clear_status_handler,
    select_handler,
    download_handler,
    func,
    func_1,
    fileupload_uploading_binding,
    change_handler,
    drag_handler,
    clear_handler,
    select_handler_1,
    upload_handler,
    error_handler,
    delete_handler
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
        interactive: 5,
        root: 6,
        label: 7,
        show_label: 8,
        height: 9,
        _selectable: 10,
        loading_status: 1,
        container: 11,
        scale: 12,
        min_width: 13,
        gradio: 14,
        file_count: 15,
        file_types: 16,
        input_ready: 19
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
  get interactive() {
    return this.$$.ctx[5];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get root() {
    return this.$$.ctx[6];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get label() {
    return this.$$.ctx[7];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get show_label() {
    return this.$$.ctx[8];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get height() {
    return this.$$.ctx[9];
  }
  set height(height) {
    this.$$set({ height });
    flush();
  }
  get _selectable() {
    return this.$$.ctx[10];
  }
  set _selectable(_selectable) {
    this.$$set({ _selectable });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[1];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
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
  get gradio() {
    return this.$$.ctx[14];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get file_count() {
    return this.$$.ctx[15];
  }
  set file_count(file_count) {
    this.$$set({ file_count });
    flush();
  }
  get file_types() {
    return this.$$.ctx[16];
  }
  set file_types(file_types) {
    this.$$set({ file_types });
    flush();
  }
  get input_ready() {
    return this.$$.ctx[19];
  }
  set input_ready(input_ready) {
    this.$$set({ input_ready });
    flush();
  }
}
export {
  default2 as BaseExample,
  File as BaseFile,
  BaseFileUpload,
  a as FilePreview,
  Index as default
};
