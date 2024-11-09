import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, createEventDispatcher, create_slot, space, claim_space, insert_hydration, update_slot_base, get_all_dirty_from_scope, get_slot_changes, detach, element, claim_element, attr, src_url_equal, text, claim_text, set_data } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { b as Button } from "./2.BqWhUxOo.js";
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
      attr(img, "class", "button-icon svelte-yjn27e");
      if (!src_url_equal(img.src, img_src_value = /*icon*/
      ctx[6].url))
        attr(img, "src", img_src_value);
      attr(img, "alt", img_alt_value = `${/*value*/
      ctx[5]} icon`);
    },
    m(target, anchor) {
      insert_hydration(target, img, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*icon*/
      64 && !src_url_equal(img.src, img_src_value = /*icon*/
      ctx2[6].url)) {
        attr(img, "src", img_src_value);
      }
      if (dirty & /*value*/
      32 && img_alt_value !== (img_alt_value = `${/*value*/
      ctx2[5]} icon`)) {
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
    ctx[6] && create_if_block(ctx)
  );
  const default_slot_template = (
    /*#slots*/
    ctx[11].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[12],
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
        ctx2[6]
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
        4096)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[12],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[12]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[12],
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
  let basebutton;
  let current;
  basebutton = new Button({
    props: {
      size: (
        /*size*/
        ctx[4]
      ),
      variant: (
        /*variant*/
        ctx[3]
      ),
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      visible: (
        /*visible*/
        ctx[2]
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
        ctx[7]
      ),
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  basebutton.$on(
    "click",
    /*download_file*/
    ctx[10]
  );
  return {
    c() {
      create_component(basebutton.$$.fragment);
    },
    l(nodes) {
      claim_component(basebutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(basebutton, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const basebutton_changes = {};
      if (dirty & /*size*/
      16)
        basebutton_changes.size = /*size*/
        ctx2[4];
      if (dirty & /*variant*/
      8)
        basebutton_changes.variant = /*variant*/
        ctx2[3];
      if (dirty & /*elem_id*/
      1)
        basebutton_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        basebutton_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*visible*/
      4)
        basebutton_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*scale*/
      256)
        basebutton_changes.scale = /*scale*/
        ctx2[8];
      if (dirty & /*min_width*/
      512)
        basebutton_changes.min_width = /*min_width*/
        ctx2[9];
      if (dirty & /*disabled*/
      128)
        basebutton_changes.disabled = /*disabled*/
        ctx2[7];
      if (dirty & /*$$scope, icon, value*/
      4192) {
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
      destroy_component(basebutton, detaching);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { variant = "secondary" } = $$props;
  let { size = "lg" } = $$props;
  let { value } = $$props;
  let { icon } = $$props;
  let { disabled = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  const dispatch = createEventDispatcher();
  function download_file() {
    dispatch("click");
    if (!(value == null ? void 0 : value.url)) {
      return;
    }
    let file_name;
    if (!value.orig_name && value.url) {
      const parts = value.url.split("/");
      file_name = parts[parts.length - 1];
      file_name = file_name.split("?")[0].split("#")[0];
    } else {
      file_name = value.orig_name;
    }
    const a = document.createElement("a");
    a.href = value.url;
    a.download = file_name || "file";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  }
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("variant" in $$props2)
      $$invalidate(3, variant = $$props2.variant);
    if ("size" in $$props2)
      $$invalidate(4, size = $$props2.size);
    if ("value" in $$props2)
      $$invalidate(5, value = $$props2.value);
    if ("icon" in $$props2)
      $$invalidate(6, icon = $$props2.icon);
    if ("disabled" in $$props2)
      $$invalidate(7, disabled = $$props2.disabled);
    if ("scale" in $$props2)
      $$invalidate(8, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(9, min_width = $$props2.min_width);
    if ("$$scope" in $$props2)
      $$invalidate(12, $$scope = $$props2.$$scope);
  };
  return [
    elem_id,
    elem_classes,
    visible,
    variant,
    size,
    value,
    icon,
    disabled,
    scale,
    min_width,
    download_file,
    slots,
    $$scope
  ];
}
class DownloadButton extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      visible: 2,
      variant: 3,
      size: 4,
      value: 5,
      icon: 6,
      disabled: 7,
      scale: 8,
      min_width: 9
    });
  }
}
const DownloadButton$1 = DownloadButton;
function create_default_slot(ctx) {
  let t_value = (
    /*label*/
    (ctx[10] ? (
      /*gradio*/
      ctx[11].i18n(
        /*label*/
        ctx[10]
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
      3072 && t_value !== (t_value = /*label*/
      (ctx2[10] ? (
        /*gradio*/
        ctx2[11].i18n(
          /*label*/
          ctx2[10]
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
  let downloadbutton;
  let current;
  downloadbutton = new DownloadButton$1({
    props: {
      value: (
        /*value*/
        ctx[3]
      ),
      variant: (
        /*variant*/
        ctx[4]
      ),
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      size: (
        /*size*/
        ctx[6]
      ),
      scale: (
        /*scale*/
        ctx[7]
      ),
      icon: (
        /*icon*/
        ctx[8]
      ),
      min_width: (
        /*min_width*/
        ctx[9]
      ),
      visible: (
        /*visible*/
        ctx[2]
      ),
      disabled: !/*interactive*/
      ctx[5],
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  downloadbutton.$on(
    "click",
    /*click_handler*/
    ctx[12]
  );
  return {
    c() {
      create_component(downloadbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(downloadbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(downloadbutton, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const downloadbutton_changes = {};
      if (dirty & /*value*/
      8)
        downloadbutton_changes.value = /*value*/
        ctx2[3];
      if (dirty & /*variant*/
      16)
        downloadbutton_changes.variant = /*variant*/
        ctx2[4];
      if (dirty & /*elem_id*/
      1)
        downloadbutton_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        downloadbutton_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*size*/
      64)
        downloadbutton_changes.size = /*size*/
        ctx2[6];
      if (dirty & /*scale*/
      128)
        downloadbutton_changes.scale = /*scale*/
        ctx2[7];
      if (dirty & /*icon*/
      256)
        downloadbutton_changes.icon = /*icon*/
        ctx2[8];
      if (dirty & /*min_width*/
      512)
        downloadbutton_changes.min_width = /*min_width*/
        ctx2[9];
      if (dirty & /*visible*/
      4)
        downloadbutton_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*interactive*/
      32)
        downloadbutton_changes.disabled = !/*interactive*/
        ctx2[5];
      if (dirty & /*$$scope, label, gradio*/
      11264) {
        downloadbutton_changes.$$scope = { dirty, ctx: ctx2 };
      }
      downloadbutton.$set(downloadbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(downloadbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(downloadbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(downloadbutton, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { variant = "secondary" } = $$props;
  let { interactive } = $$props;
  let { size = "lg" } = $$props;
  let { scale = null } = $$props;
  let { icon = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { label = null } = $$props;
  let { gradio } = $$props;
  const click_handler = () => gradio.dispatch("click");
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("variant" in $$props2)
      $$invalidate(4, variant = $$props2.variant);
    if ("interactive" in $$props2)
      $$invalidate(5, interactive = $$props2.interactive);
    if ("size" in $$props2)
      $$invalidate(6, size = $$props2.size);
    if ("scale" in $$props2)
      $$invalidate(7, scale = $$props2.scale);
    if ("icon" in $$props2)
      $$invalidate(8, icon = $$props2.icon);
    if ("min_width" in $$props2)
      $$invalidate(9, min_width = $$props2.min_width);
    if ("label" in $$props2)
      $$invalidate(10, label = $$props2.label);
    if ("gradio" in $$props2)
      $$invalidate(11, gradio = $$props2.gradio);
  };
  return [
    elem_id,
    elem_classes,
    visible,
    value,
    variant,
    interactive,
    size,
    scale,
    icon,
    min_width,
    label,
    gradio,
    click_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 0,
      elem_classes: 1,
      visible: 2,
      value: 3,
      variant: 4,
      interactive: 5,
      size: 6,
      scale: 7,
      icon: 8,
      min_width: 9,
      label: 10,
      gradio: 11
    });
  }
}
export {
  DownloadButton$1 as BaseButton,
  Index as default
};
