import { SvelteComponent, init, safe_not_equal, element, space, create_component, claim_element, claim_space, claim_component, attr, insert_hydration, mount_component, listen, transition_in, transition_out, detach, destroy_component, run_all, createEventDispatcher, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, binding_callbacks, src_url_equal, tick, text, claim_text, set_data } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { b as Button, h as prepare_files } from "./2.BqWhUxOo.js";
function create_if_block(ctx) {
  let img;
  let img_src_value;
  let img_alt_value;
  return {
    c() {
      img = element("img");
      this.h();
    },
    l(nodes) {
      img = claim_element(nodes, "IMG", { class: true, src: true, alt: true });
      this.h();
    },
    h() {
      attr(img, "class", "button-icon svelte-1rvxzzt");
      if (!src_url_equal(img.src, img_src_value = /*icon*/
      ctx[7].url))
        attr(img, "src", img_src_value);
      attr(img, "alt", img_alt_value = `${/*value*/
      ctx[0]} icon`);
    },
    m(target, anchor) {
      insert_hydration(target, img, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*icon*/
      128 && !src_url_equal(img.src, img_src_value = /*icon*/
      ctx2[7].url)) {
        attr(img, "src", img_src_value);
      }
      if (dirty & /*value*/
      1 && img_alt_value !== (img_alt_value = `${/*value*/
      ctx2[0]} icon`)) {
        attr(img, "alt", img_alt_value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(img);
      }
    }
  };
}
function create_default_slot$1(ctx) {
  let t;
  let current;
  let if_block = (
    /*icon*/
    ctx[7] && create_if_block(ctx)
  );
  const default_slot_template = (
    /*#slots*/
    ctx[20].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[22],
    null
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      if (default_slot)
        default_slot.c();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      if (default_slot)
        default_slot.l(nodes);
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*icon*/
        ctx2[7]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        4194304)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[22],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[22]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[22],
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
      if (detaching) {
        detach(t);
      }
      if (if_block)
        if_block.d(detaching);
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let input;
  let input_multiple_value;
  let input_webkitdirectory_value;
  let input_mozdirectory_value;
  let input_data_testid_value;
  let t;
  let basebutton;
  let current;
  let mounted;
  let dispose;
  basebutton = new Button({
    props: {
      size: (
        /*size*/
        ctx[6]
      ),
      variant: (
        /*variant*/
        ctx[10]
      ),
      elem_id: (
        /*elem_id*/
        ctx[1]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[2]
      ),
      visible: (
        /*visible*/
        ctx[3]
      ),
      scale: (
        /*scale*/
        ctx[8]
      ),
      min_width: (
        /*min_width*/
        ctx[9]
      ),
      disabled: (
        /*disabled*/
        ctx[11]
      ),
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  basebutton.$on(
    "click",
    /*open_file_upload*/
    ctx[14]
  );
  return {
    c() {
      input = element("input");
      t = space();
      create_component(basebutton.$$.fragment);
      this.h();
    },
    l(nodes) {
      input = claim_element(nodes, "INPUT", {
        class: true,
        accept: true,
        type: true,
        webkitdirectory: true,
        mozdirectory: true,
        "data-testid": true
      });
      t = claim_space(nodes);
      claim_component(basebutton.$$.fragment, nodes);
      this.h();
    },
    h() {
      attr(input, "class", "hide svelte-1rvxzzt");
      attr(
        input,
        "accept",
        /*accept_file_types*/
        ctx[13]
      );
      attr(input, "type", "file");
      input.multiple = input_multiple_value = /*file_count*/
      ctx[5] === "multiple" || void 0;
      attr(input, "webkitdirectory", input_webkitdirectory_value = /*file_count*/
      ctx[5] === "directory" || void 0);
      attr(input, "mozdirectory", input_mozdirectory_value = /*file_count*/
      ctx[5] === "directory" || void 0);
      attr(input, "data-testid", input_data_testid_value = /*label*/
      ctx[4] + "-upload-button");
    },
    m(target, anchor) {
      insert_hydration(target, input, anchor);
      ctx[21](input);
      insert_hydration(target, t, anchor);
      mount_component(basebutton, target, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            input,
            "change",
            /*load_files_from_upload*/
            ctx[15]
          ),
          listen(input, "click", clear_input_value)
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (!current || dirty & /*accept_file_types*/
      8192) {
        attr(
          input,
          "accept",
          /*accept_file_types*/
          ctx2[13]
        );
      }
      if (!current || dirty & /*file_count*/
      32 && input_multiple_value !== (input_multiple_value = /*file_count*/
      ctx2[5] === "multiple" || void 0)) {
        input.multiple = input_multiple_value;
      }
      if (!current || dirty & /*file_count*/
      32 && input_webkitdirectory_value !== (input_webkitdirectory_value = /*file_count*/
      ctx2[5] === "directory" || void 0)) {
        attr(input, "webkitdirectory", input_webkitdirectory_value);
      }
      if (!current || dirty & /*file_count*/
      32 && input_mozdirectory_value !== (input_mozdirectory_value = /*file_count*/
      ctx2[5] === "directory" || void 0)) {
        attr(input, "mozdirectory", input_mozdirectory_value);
      }
      if (!current || dirty & /*label*/
      16 && input_data_testid_value !== (input_data_testid_value = /*label*/
      ctx2[4] + "-upload-button")) {
        attr(input, "data-testid", input_data_testid_value);
      }
      const basebutton_changes = {};
      if (dirty & /*size*/
      64)
        basebutton_changes.size = /*size*/
        ctx2[6];
      if (dirty & /*variant*/
      1024)
        basebutton_changes.variant = /*variant*/
        ctx2[10];
      if (dirty & /*elem_id*/
      2)
        basebutton_changes.elem_id = /*elem_id*/
        ctx2[1];
      if (dirty & /*elem_classes*/
      4)
        basebutton_changes.elem_classes = /*elem_classes*/
        ctx2[2];
      if (dirty & /*visible*/
      8)
        basebutton_changes.visible = /*visible*/
        ctx2[3];
      if (dirty & /*scale*/
      256)
        basebutton_changes.scale = /*scale*/
        ctx2[8];
      if (dirty & /*min_width*/
      512)
        basebutton_changes.min_width = /*min_width*/
        ctx2[9];
      if (dirty & /*disabled*/
      2048)
        basebutton_changes.disabled = /*disabled*/
        ctx2[11];
      if (dirty & /*$$scope, icon, value*/
      4194433) {
        basebutton_changes.$$scope = { dirty, ctx: ctx2 };
      }
      basebutton.$set(basebutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(basebutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(basebutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(input);
        detach(t);
      }
      ctx[21](null);
      destroy_component(basebutton, detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function clear_input_value(e) {
  const target = e.target;
  if (target.value)
    target.value = "";
}
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { label } = $$props;
  let { value } = $$props;
  let { file_count } = $$props;
  let { file_types = [] } = $$props;
  let { root } = $$props;
  let { size = "lg" } = $$props;
  let { icon = null } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { variant = "secondary" } = $$props;
  let { disabled = false } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  const dispatch = createEventDispatcher();
  let hidden_upload;
  let accept_file_types;
  if (file_types == null) {
    accept_file_types = null;
  } else {
    file_types = file_types.map((x) => {
      if (x.startsWith(".")) {
        return x;
      }
      return x + "/*";
    });
    accept_file_types = file_types.join(", ");
  }
  function open_file_upload() {
    dispatch("click");
    hidden_upload.click();
  }
  async function load_files(files) {
    var _a;
    let _files = Array.from(files);
    if (!files.length) {
      return;
    }
    if (file_count === "single") {
      _files = [files[0]];
    }
    let all_file_data = await prepare_files(_files);
    await tick();
    try {
      all_file_data = (_a = await upload(all_file_data, root, void 0, max_file_size ?? Infinity)) == null ? void 0 : _a.filter((x) => x !== null);
    } catch (e) {
      dispatch("error", e.message);
      return;
    }
    $$invalidate(0, value = file_count === "single" ? all_file_data == null ? void 0 : all_file_data[0] : all_file_data);
    dispatch("change", value);
    dispatch("upload", value);
  }
  async function load_files_from_upload(e) {
    const target = e.target;
    if (!target.files)
      return;
    await load_files(target.files);
  }
  function input_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      hidden_upload = $$value;
      $$invalidate(12, hidden_upload);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("file_count" in $$props2)
      $$invalidate(5, file_count = $$props2.file_count);
    if ("file_types" in $$props2)
      $$invalidate(16, file_types = $$props2.file_types);
    if ("root" in $$props2)
      $$invalidate(17, root = $$props2.root);
    if ("size" in $$props2)
      $$invalidate(6, size = $$props2.size);
    if ("icon" in $$props2)
      $$invalidate(7, icon = $$props2.icon);
    if ("scale" in $$props2)
      $$invalidate(8, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(9, min_width = $$props2.min_width);
    if ("variant" in $$props2)
      $$invalidate(10, variant = $$props2.variant);
    if ("disabled" in $$props2)
      $$invalidate(11, disabled = $$props2.disabled);
    if ("max_file_size" in $$props2)
      $$invalidate(18, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(19, upload = $$props2.upload);
    if ("$$scope" in $$props2)
      $$invalidate(22, $$scope = $$props2.$$scope);
  };
  return [
    value,
    elem_id,
    elem_classes,
    visible,
    label,
    file_count,
    size,
    icon,
    scale,
    min_width,
    variant,
    disabled,
    hidden_upload,
    accept_file_types,
    open_file_upload,
    load_files_from_upload,
    file_types,
    root,
    max_file_size,
    upload,
    slots,
    input_binding,
    $$scope
  ];
}
class UploadButton extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      elem_id: 1,
      elem_classes: 2,
      visible: 3,
      label: 4,
      value: 0,
      file_count: 5,
      file_types: 16,
      root: 17,
      size: 6,
      icon: 7,
      scale: 8,
      min_width: 9,
      variant: 10,
      disabled: 11,
      max_file_size: 18,
      upload: 19
    });
  }
}
const UploadButton$1 = UploadButton;
function create_default_slot(ctx) {
  let t_value = (
    /*label*/
    (ctx[4] ? (
      /*gradio*/
      ctx[13].i18n(
        /*label*/
        ctx[4]
      )
    ) : "") + ""
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
      if (dirty & /*label, gradio*/
      8208 && t_value !== (t_value = /*label*/
      (ctx2[4] ? (
        /*gradio*/
        ctx2[13].i18n(
          /*label*/
          ctx2[4]
        )
      ) : "") + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_fragment(ctx) {
  let uploadbutton;
  let current;
  uploadbutton = new UploadButton$1({
    props: {
      elem_id: (
        /*elem_id*/
        ctx[1]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[2]
      ),
      visible: (
        /*visible*/
        ctx[3]
      ),
      file_count: (
        /*file_count*/
        ctx[5]
      ),
      file_types: (
        /*file_types*/
        ctx[6]
      ),
      size: (
        /*size*/
        ctx[8]
      ),
      scale: (
        /*scale*/
        ctx[9]
      ),
      icon: (
        /*icon*/
        ctx[10]
      ),
      min_width: (
        /*min_width*/
        ctx[11]
      ),
      root: (
        /*root*/
        ctx[7]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      disabled: (
        /*disabled*/
        ctx[14]
      ),
      variant: (
        /*variant*/
        ctx[12]
      ),
      label: (
        /*label*/
        ctx[4]
      ),
      max_file_size: (
        /*gradio*/
        ctx[13].max_file_size
      ),
      upload: (
        /*func*/
        ctx[17]
      ),
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  uploadbutton.$on(
    "click",
    /*click_handler*/
    ctx[18]
  );
  uploadbutton.$on(
    "change",
    /*change_handler*/
    ctx[19]
  );
  uploadbutton.$on(
    "upload",
    /*upload_handler*/
    ctx[20]
  );
  uploadbutton.$on(
    "error",
    /*error_handler*/
    ctx[21]
  );
  return {
    c() {
      create_component(uploadbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(uploadbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(uploadbutton, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const uploadbutton_changes = {};
      if (dirty & /*elem_id*/
      2)
        uploadbutton_changes.elem_id = /*elem_id*/
        ctx2[1];
      if (dirty & /*elem_classes*/
      4)
        uploadbutton_changes.elem_classes = /*elem_classes*/
        ctx2[2];
      if (dirty & /*visible*/
      8)
        uploadbutton_changes.visible = /*visible*/
        ctx2[3];
      if (dirty & /*file_count*/
      32)
        uploadbutton_changes.file_count = /*file_count*/
        ctx2[5];
      if (dirty & /*file_types*/
      64)
        uploadbutton_changes.file_types = /*file_types*/
        ctx2[6];
      if (dirty & /*size*/
      256)
        uploadbutton_changes.size = /*size*/
        ctx2[8];
      if (dirty & /*scale*/
      512)
        uploadbutton_changes.scale = /*scale*/
        ctx2[9];
      if (dirty & /*icon*/
      1024)
        uploadbutton_changes.icon = /*icon*/
        ctx2[10];
      if (dirty & /*min_width*/
      2048)
        uploadbutton_changes.min_width = /*min_width*/
        ctx2[11];
      if (dirty & /*root*/
      128)
        uploadbutton_changes.root = /*root*/
        ctx2[7];
      if (dirty & /*value*/
      1)
        uploadbutton_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*disabled*/
      16384)
        uploadbutton_changes.disabled = /*disabled*/
        ctx2[14];
      if (dirty & /*variant*/
      4096)
        uploadbutton_changes.variant = /*variant*/
        ctx2[12];
      if (dirty & /*label*/
      16)
        uploadbutton_changes.label = /*label*/
        ctx2[4];
      if (dirty & /*gradio*/
      8192)
        uploadbutton_changes.max_file_size = /*gradio*/
        ctx2[13].max_file_size;
      if (dirty & /*gradio*/
      8192)
        uploadbutton_changes.upload = /*func*/
        ctx2[17];
      if (dirty & /*$$scope, label, gradio*/
      4202512) {
        uploadbutton_changes.$$scope = { dirty, ctx: ctx2 };
      }
      uploadbutton.$set(uploadbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(uploadbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(uploadbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(uploadbutton, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let disabled;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { label } = $$props;
  let { value } = $$props;
  let { file_count } = $$props;
  let { file_types = [] } = $$props;
  let { root } = $$props;
  let { size = "lg" } = $$props;
  let { scale = null } = $$props;
  let { icon = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { variant = "secondary" } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  async function handle_event(detail, event) {
    $$invalidate(0, value = detail);
    gradio.dispatch(event);
  }
  const func = (...args) => gradio.client.upload(...args);
  const click_handler = () => gradio.dispatch("click");
  const change_handler = ({ detail }) => handle_event(detail, "change");
  const upload_handler = ({ detail }) => handle_event(detail, "upload");
  const error_handler = ({ detail }) => {
    gradio.dispatch("error", detail);
  };
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("file_count" in $$props2)
      $$invalidate(5, file_count = $$props2.file_count);
    if ("file_types" in $$props2)
      $$invalidate(6, file_types = $$props2.file_types);
    if ("root" in $$props2)
      $$invalidate(7, root = $$props2.root);
    if ("size" in $$props2)
      $$invalidate(8, size = $$props2.size);
    if ("scale" in $$props2)
      $$invalidate(9, scale = $$props2.scale);
    if ("icon" in $$props2)
      $$invalidate(10, icon = $$props2.icon);
    if ("min_width" in $$props2)
      $$invalidate(11, min_width = $$props2.min_width);
    if ("variant" in $$props2)
      $$invalidate(12, variant = $$props2.variant);
    if ("gradio" in $$props2)
      $$invalidate(13, gradio = $$props2.gradio);
    if ("interactive" in $$props2)
      $$invalidate(16, interactive = $$props2.interactive);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*interactive*/
    65536) {
      $$invalidate(14, disabled = !interactive);
    }
  };
  return [
    value,
    elem_id,
    elem_classes,
    visible,
    label,
    file_count,
    file_types,
    root,
    size,
    scale,
    icon,
    min_width,
    variant,
    gradio,
    disabled,
    handle_event,
    interactive,
    func,
    click_handler,
    change_handler,
    upload_handler,
    error_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 1,
      elem_classes: 2,
      visible: 3,
      label: 4,
      value: 0,
      file_count: 5,
      file_types: 6,
      root: 7,
      size: 8,
      scale: 9,
      icon: 10,
      min_width: 11,
      variant: 12,
      gradio: 13,
      interactive: 16
    });
  }
}
export {
  UploadButton$1 as BaseUploadButton,
  Index as default
};
