const __vite__fileDeps=["./Canvas3D.BKqbL-gL.js","./2.BqWhUxOo.js","./preload-helper.DpQnamwV.js","./stores.DcWgXC6T.js","./client.DB6ownDU.js","../assets/2.DoS7Rli5.css","./file-url.Bs-FMz4v.js","../assets/DownloadLink.wTm37iHI.css","./Canvas3DGS.o8AhzH63.js"],__vite__mapDeps=i=>i.map(i=>__vite__fileDeps[i]);
import { SvelteComponent, init, safe_not_equal, create_component, space, empty, claim_component, claim_space, mount_component, insert_hydration, transition_in, group_outros, transition_out, check_outros, detach, destroy_component, element, claim_element, children, attr, append_hydration, binding_callbacks, construct_svelte_component, bind, add_flush_callback, createEventDispatcher, tick, bubble, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { _ as __vitePreload } from "./preload-helper.DpQnamwV.js";
import { I as IconButton, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { F as File } from "./File.DqOJDDoa.js";
import { U as Undo } from "./Undo.BEjgqHJW.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
import { M as ModifyUpload } from "./ModifyUpload.Bygvg79x.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { U as UploadText } from "./UploadText.BG6Hywul.js";
import { default as default2 } from "./Example.Ydb40JSe.js";
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
function create_if_block$2(ctx) {
  let div;
  let iconbuttonwrapper;
  let t;
  let current_block_type_index;
  let if_block;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot$2] },
      $$scope: { ctx }
    }
  });
  const if_block_creators = [create_if_block_1$2, create_else_block$2];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*use_3dgs*/
      ctx2[10]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      div = element("div");
      create_component(iconbuttonwrapper.$$.fragment);
      t = space();
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(iconbuttonwrapper.$$.fragment, div_nodes);
      t = claim_space(div_nodes);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "model3D svelte-1mxwah3");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(iconbuttonwrapper, div, null);
      append_hydration(div, t);
      if_blocks[current_block_type_index].m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, resolved_url, value, i18n, has_change_history, use_3dgs*/
      2115105) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
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
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(iconbuttonwrapper);
      if_blocks[current_block_type_index].d();
    }
  };
}
function create_if_block_2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Undo,
      label: "Undo",
      disabled: !/*has_change_history*/
      ctx[9]
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[17]
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
      if (dirty & /*has_change_history*/
      512)
        iconbutton_changes.disabled = !/*has_change_history*/
        ctx2[9];
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
function create_default_slot$2(ctx) {
  let t;
  let a;
  let iconbutton;
  let a_download_value;
  let current;
  let if_block = !/*use_3dgs*/
  ctx[10] && create_if_block_2(ctx);
  iconbutton = new IconButton({
    props: {
      Icon: Download,
      label: (
        /*i18n*/
        ctx[5]("common.download")
      )
    }
  });
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      a = element("a");
      create_component(iconbutton.$$.fragment);
      this.h();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      a = claim_element(nodes, "A", { href: true, target: true, download: true });
      var a_nodes = children(a);
      claim_component(iconbutton.$$.fragment, a_nodes);
      a_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(
        a,
        "href",
        /*resolved_url*/
        ctx[14]
      );
      attr(a, "target", window.__is_colab__ ? "_blank" : null);
      attr(a, "download", a_download_value = window.__is_colab__ ? null : (
        /*value*/
        ctx[0].orig_name || /*value*/
        ctx[0].path
      ));
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, a, anchor);
      mount_component(iconbutton, a, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (!/*use_3dgs*/
      ctx2[10]) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*use_3dgs*/
          1024) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_2(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const iconbutton_changes = {};
      if (dirty & /*i18n*/
      32)
        iconbutton_changes.label = /*i18n*/
        ctx2[5]("common.download");
      iconbutton.$set(iconbutton_changes);
      if (!current || dirty & /*resolved_url*/
      16384) {
        attr(
          a,
          "href",
          /*resolved_url*/
          ctx2[14]
        );
      }
      if (!current || dirty & /*value*/
      1 && a_download_value !== (a_download_value = window.__is_colab__ ? null : (
        /*value*/
        ctx2[0].orig_name || /*value*/
        ctx2[0].path
      ))) {
        attr(a, "download", a_download_value);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(a);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_component(iconbutton);
    }
  };
}
function create_else_block$2(ctx) {
  let switch_instance;
  let updating_resolved_url;
  let switch_instance_anchor;
  let current;
  function switch_instance_resolved_url_binding_1(value) {
    ctx[20](value);
  }
  var switch_value = (
    /*Canvas3DComponent*/
    ctx[13]
  );
  function switch_props(ctx2, dirty) {
    let switch_instance_props = {
      value: (
        /*value*/
        ctx2[0]
      ),
      display_mode: (
        /*display_mode*/
        ctx2[1]
      ),
      clear_color: (
        /*clear_color*/
        ctx2[2]
      ),
      camera_position: (
        /*camera_position*/
        ctx2[8]
      ),
      zoom_speed: (
        /*zoom_speed*/
        ctx2[6]
      ),
      pan_speed: (
        /*pan_speed*/
        ctx2[7]
      )
    };
    if (
      /*resolved_url*/
      ctx2[14] !== void 0
    ) {
      switch_instance_props.resolved_url = /*resolved_url*/
      ctx2[14];
    }
    return { props: switch_instance_props };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    ctx[19](switch_instance);
    binding_callbacks.push(() => bind(switch_instance, "resolved_url", switch_instance_resolved_url_binding_1));
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*Canvas3DComponent*/
      8192 && switch_value !== (switch_value = /*Canvas3DComponent*/
      ctx2[13])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          ctx2[19](switch_instance);
          binding_callbacks.push(() => bind(switch_instance, "resolved_url", switch_instance_resolved_url_binding_1));
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        1)
          switch_instance_changes.value = /*value*/
          ctx2[0];
        if (dirty & /*display_mode*/
        2)
          switch_instance_changes.display_mode = /*display_mode*/
          ctx2[1];
        if (dirty & /*clear_color*/
        4)
          switch_instance_changes.clear_color = /*clear_color*/
          ctx2[2];
        if (dirty & /*camera_position*/
        256)
          switch_instance_changes.camera_position = /*camera_position*/
          ctx2[8];
        if (dirty & /*zoom_speed*/
        64)
          switch_instance_changes.zoom_speed = /*zoom_speed*/
          ctx2[6];
        if (dirty & /*pan_speed*/
        128)
          switch_instance_changes.pan_speed = /*pan_speed*/
          ctx2[7];
        if (!updating_resolved_url && dirty & /*resolved_url*/
        16384) {
          updating_resolved_url = true;
          switch_instance_changes.resolved_url = /*resolved_url*/
          ctx2[14];
          add_flush_callback(() => updating_resolved_url = false);
        }
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      ctx[19](null);
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_1$2(ctx) {
  let switch_instance;
  let updating_resolved_url;
  let switch_instance_anchor;
  let current;
  function switch_instance_resolved_url_binding(value) {
    ctx[18](value);
  }
  var switch_value = (
    /*Canvas3DGSComponent*/
    ctx[12]
  );
  function switch_props(ctx2, dirty) {
    let switch_instance_props = {
      value: (
        /*value*/
        ctx2[0]
      ),
      zoom_speed: (
        /*zoom_speed*/
        ctx2[6]
      ),
      pan_speed: (
        /*pan_speed*/
        ctx2[7]
      )
    };
    if (
      /*resolved_url*/
      ctx2[14] !== void 0
    ) {
      switch_instance_props.resolved_url = /*resolved_url*/
      ctx2[14];
    }
    return { props: switch_instance_props };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    binding_callbacks.push(() => bind(switch_instance, "resolved_url", switch_instance_resolved_url_binding));
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*Canvas3DGSComponent*/
      4096 && switch_value !== (switch_value = /*Canvas3DGSComponent*/
      ctx2[12])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          binding_callbacks.push(() => bind(switch_instance, "resolved_url", switch_instance_resolved_url_binding));
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        1)
          switch_instance_changes.value = /*value*/
          ctx2[0];
        if (dirty & /*zoom_speed*/
        64)
          switch_instance_changes.zoom_speed = /*zoom_speed*/
          ctx2[6];
        if (dirty & /*pan_speed*/
        128)
          switch_instance_changes.pan_speed = /*pan_speed*/
          ctx2[7];
        if (!updating_resolved_url && dirty & /*resolved_url*/
        16384) {
          updating_resolved_url = true;
          switch_instance_changes.resolved_url = /*resolved_url*/
          ctx2[14];
          add_flush_callback(() => updating_resolved_url = false);
        }
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_fragment$2(ctx) {
  let blocklabel;
  let t;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[4]
      ),
      Icon: File,
      label: (
        /*label*/
        ctx[3] || /*i18n*/
        ctx[5]("3D_model.3d_model")
      )
    }
  });
  let if_block = (
    /*value*/
    ctx[0] && create_if_block$2(ctx)
  );
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      16)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[4];
      if (dirty & /*label, i18n*/
      40)
        blocklabel_changes.label = /*label*/
        ctx2[3] || /*i18n*/
        ctx2[5]("3D_model.3d_model");
      blocklabel.$set(blocklabel_changes);
      if (
        /*value*/
        ctx2[0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*value*/
          1) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block$2(ctx2);
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
      if (if_block)
        if_block.d(detaching);
    }
  };
}
async function loadCanvas3D$1() {
  const module = await __vitePreload(() => import("./Canvas3D.BKqbL-gL.js"), true ? __vite__mapDeps([0,1,2,3,4,5,6,7]) : void 0, import.meta.url);
  return module.default;
}
async function loadCanvas3DGS$1() {
  const module = await __vitePreload(() => import("./Canvas3DGS.o8AhzH63.js"), true ? __vite__mapDeps([8,6,7]) : void 0, import.meta.url);
  return module.default;
}
function instance$2($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color = [0, 0, 0, 0] } = $$props;
  let { label = "" } = $$props;
  let { show_label } = $$props;
  let { i18n } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { pan_speed = 1 } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { has_change_history = false } = $$props;
  let current_settings = { camera_position, zoom_speed, pan_speed };
  let use_3dgs = false;
  let Canvas3DGSComponent;
  let Canvas3DComponent;
  let canvas3d;
  function handle_undo() {
    canvas3d == null ? void 0 : canvas3d.reset_camera_position(camera_position, zoom_speed, pan_speed);
  }
  let resolved_url;
  const click_handler = () => handle_undo();
  function switch_instance_resolved_url_binding(value2) {
    resolved_url = value2;
    $$invalidate(14, resolved_url);
  }
  function switch_instance_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      canvas3d = $$value;
      $$invalidate(11, canvas3d);
    });
  }
  function switch_instance_resolved_url_binding_1(value2) {
    resolved_url = value2;
    $$invalidate(14, resolved_url);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("display_mode" in $$props2)
      $$invalidate(1, display_mode = $$props2.display_mode);
    if ("clear_color" in $$props2)
      $$invalidate(2, clear_color = $$props2.clear_color);
    if ("label" in $$props2)
      $$invalidate(3, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(4, show_label = $$props2.show_label);
    if ("i18n" in $$props2)
      $$invalidate(5, i18n = $$props2.i18n);
    if ("zoom_speed" in $$props2)
      $$invalidate(6, zoom_speed = $$props2.zoom_speed);
    if ("pan_speed" in $$props2)
      $$invalidate(7, pan_speed = $$props2.pan_speed);
    if ("camera_position" in $$props2)
      $$invalidate(8, camera_position = $$props2.camera_position);
    if ("has_change_history" in $$props2)
      $$invalidate(9, has_change_history = $$props2.has_change_history);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value, use_3dgs*/
    1025) {
      if (value) {
        $$invalidate(10, use_3dgs = value.path.endsWith(".splat") || value.path.endsWith(".ply"));
        if (use_3dgs) {
          loadCanvas3DGS$1().then((component) => {
            $$invalidate(12, Canvas3DGSComponent = component);
          });
        } else {
          loadCanvas3D$1().then((component) => {
            $$invalidate(13, Canvas3DComponent = component);
          });
        }
      }
    }
    if ($$self.$$.dirty & /*current_settings, camera_position, zoom_speed, pan_speed, canvas3d*/
    68032) {
      {
        if (!dequal(current_settings.camera_position, camera_position) || current_settings.zoom_speed !== zoom_speed || current_settings.pan_speed !== pan_speed) {
          canvas3d == null ? void 0 : canvas3d.reset_camera_position(camera_position, zoom_speed, pan_speed);
          $$invalidate(16, current_settings = { camera_position, zoom_speed, pan_speed });
        }
      }
    }
  };
  return [
    value,
    display_mode,
    clear_color,
    label,
    show_label,
    i18n,
    zoom_speed,
    pan_speed,
    camera_position,
    has_change_history,
    use_3dgs,
    canvas3d,
    Canvas3DGSComponent,
    Canvas3DComponent,
    resolved_url,
    handle_undo,
    current_settings,
    click_handler,
    switch_instance_resolved_url_binding,
    switch_instance_binding,
    switch_instance_resolved_url_binding_1
  ];
}
class Model3D extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      value: 0,
      display_mode: 1,
      clear_color: 2,
      label: 3,
      show_label: 4,
      i18n: 5,
      zoom_speed: 6,
      pan_speed: 7,
      camera_position: 8,
      has_change_history: 9
    });
  }
}
const Model3D$1 = Model3D;
function create_else_block$1(ctx) {
  let div;
  let modifyupload;
  let t;
  let current_block_type_index;
  let if_block;
  let current;
  modifyupload = new ModifyUpload({
    props: {
      undoable: !/*use_3dgs*/
      ctx[14],
      i18n: (
        /*i18n*/
        ctx[7]
      )
    }
  });
  modifyupload.$on(
    "clear",
    /*handle_clear*/
    ctx[20]
  );
  modifyupload.$on(
    "undo",
    /*handle_undo*/
    ctx[21]
  );
  const if_block_creators = [create_if_block_1$1, create_else_block_1$1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*use_3dgs*/
      ctx2[14]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      div = element("div");
      create_component(modifyupload.$$.fragment);
      t = space();
      if_block.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(modifyupload.$$.fragment, div_nodes);
      t = claim_space(div_nodes);
      if_block.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "input-model svelte-jub4pj");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(modifyupload, div, null);
      append_hydration(div, t);
      if_blocks[current_block_type_index].m(div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const modifyupload_changes = {};
      if (dirty & /*use_3dgs*/
      16384)
        modifyupload_changes.undoable = !/*use_3dgs*/
        ctx2[14];
      if (dirty & /*i18n*/
      128)
        modifyupload_changes.i18n = /*i18n*/
        ctx2[7];
      modifyupload.$set(modifyupload_changes);
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
        if_block.m(div, null);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(modifyupload.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(modifyupload.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(modifyupload);
      if_blocks[current_block_type_index].d();
    }
  };
}
function create_if_block$1(ctx) {
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let current;
  function upload_1_dragging_binding(value) {
    ctx[23](value);
  }
  function upload_1_uploading_binding(value) {
    ctx[24](value);
  }
  let upload_1_props = {
    upload: (
      /*upload*/
      ctx[12]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[13]
    ),
    root: (
      /*root*/
      ctx[6]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[10]
    ),
    filetype: [".stl", ".obj", ".gltf", ".glb", "model/obj", ".splat", ".ply"],
    $$slots: { default: [create_default_slot$1] },
    $$scope: { ctx }
  };
  if (
    /*dragging*/
    ctx[15] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[15];
  }
  if (
    /*uploading*/
    ctx[1] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[1];
  }
  upload_1 = new Upload({ props: upload_1_props });
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  upload_1.$on(
    "load",
    /*handle_upload*/
    ctx[19]
  );
  upload_1.$on(
    "error",
    /*error_handler*/
    ctx[25]
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
      if (dirty & /*upload*/
      4096)
        upload_1_changes.upload = /*upload*/
        ctx2[12];
      if (dirty & /*stream_handler*/
      8192)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[13];
      if (dirty & /*root*/
      64)
        upload_1_changes.root = /*root*/
        ctx2[6];
      if (dirty & /*max_file_size*/
      1024)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[10];
      if (dirty & /*$$scope*/
      134217728) {
        upload_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_dragging && dirty & /*dragging*/
      32768) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[15];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty & /*uploading*/
      2) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[1];
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
function create_else_block_1$1(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*Canvas3DComponent*/
    ctx[17]
  );
  function switch_props(ctx2, dirty) {
    let switch_instance_props = {
      value: (
        /*value*/
        ctx2[0]
      ),
      display_mode: (
        /*display_mode*/
        ctx2[2]
      ),
      clear_color: (
        /*clear_color*/
        ctx2[3]
      ),
      camera_position: (
        /*camera_position*/
        ctx2[11]
      ),
      zoom_speed: (
        /*zoom_speed*/
        ctx2[8]
      ),
      pan_speed: (
        /*pan_speed*/
        ctx2[9]
      )
    };
    return { props: switch_instance_props };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    ctx[26](switch_instance);
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*Canvas3DComponent*/
      131072 && switch_value !== (switch_value = /*Canvas3DComponent*/
      ctx2[17])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          ctx2[26](switch_instance);
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        1)
          switch_instance_changes.value = /*value*/
          ctx2[0];
        if (dirty & /*display_mode*/
        4)
          switch_instance_changes.display_mode = /*display_mode*/
          ctx2[2];
        if (dirty & /*clear_color*/
        8)
          switch_instance_changes.clear_color = /*clear_color*/
          ctx2[3];
        if (dirty & /*camera_position*/
        2048)
          switch_instance_changes.camera_position = /*camera_position*/
          ctx2[11];
        if (dirty & /*zoom_speed*/
        256)
          switch_instance_changes.zoom_speed = /*zoom_speed*/
          ctx2[8];
        if (dirty & /*pan_speed*/
        512)
          switch_instance_changes.pan_speed = /*pan_speed*/
          ctx2[9];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      ctx[26](null);
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_1$1(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*Canvas3DGSComponent*/
    ctx[16]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[0]
        ),
        zoom_speed: (
          /*zoom_speed*/
          ctx2[8]
        ),
        pan_speed: (
          /*pan_speed*/
          ctx2[9]
        )
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*Canvas3DGSComponent*/
      65536 && switch_value !== (switch_value = /*Canvas3DGSComponent*/
      ctx2[16])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        1)
          switch_instance_changes.value = /*value*/
          ctx2[0];
        if (dirty & /*zoom_speed*/
        256)
          switch_instance_changes.zoom_speed = /*zoom_speed*/
          ctx2[8];
        if (dirty & /*pan_speed*/
        512)
          switch_instance_changes.pan_speed = /*pan_speed*/
          ctx2[9];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_default_slot$1(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[22].default
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
        if (default_slot.p && (!current || dirty & /*$$scope*/
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
  let t;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[5]
      ),
      Icon: File,
      label: (
        /*label*/
        ctx[4] || "3D Model"
      )
    }
  });
  const if_block_creators = [create_if_block$1, create_else_block$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] === null
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
      32)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[5];
      if (dirty & /*label*/
      16)
        blocklabel_changes.label = /*label*/
        ctx2[4] || "3D Model";
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
async function loadCanvas3D() {
  const module = await __vitePreload(() => import("./Canvas3D.BKqbL-gL.js"), true ? __vite__mapDeps([0,1,2,3,4,5,6,7]) : void 0, import.meta.url);
  return module.default;
}
async function loadCanvas3DGS() {
  const module = await __vitePreload(() => import("./Canvas3DGS.o8AhzH63.js"), true ? __vite__mapDeps([8,6,7]) : void 0, import.meta.url);
  return module.default;
}
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { value } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color = [0, 0, 0, 0] } = $$props;
  let { label = "" } = $$props;
  let { show_label } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { pan_speed = 1 } = $$props;
  let { max_file_size = null } = $$props;
  let { uploading = false } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  async function handle_upload({ detail }) {
    $$invalidate(0, value = detail);
    await tick();
    dispatch("change", value);
    dispatch("load", value);
  }
  async function handle_clear() {
    $$invalidate(0, value = null);
    await tick();
    dispatch("clear");
    dispatch("change");
  }
  let use_3dgs = false;
  let Canvas3DGSComponent;
  let Canvas3DComponent;
  let canvas3d;
  async function handle_undo() {
    canvas3d == null ? void 0 : canvas3d.reset_camera_position(camera_position, zoom_speed, pan_speed);
  }
  const dispatch = createEventDispatcher();
  let dragging = false;
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(15, dragging);
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(1, uploading);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function switch_instance_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      canvas3d = $$value;
      $$invalidate(18, canvas3d);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("display_mode" in $$props2)
      $$invalidate(2, display_mode = $$props2.display_mode);
    if ("clear_color" in $$props2)
      $$invalidate(3, clear_color = $$props2.clear_color);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(5, show_label = $$props2.show_label);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("i18n" in $$props2)
      $$invalidate(7, i18n = $$props2.i18n);
    if ("zoom_speed" in $$props2)
      $$invalidate(8, zoom_speed = $$props2.zoom_speed);
    if ("pan_speed" in $$props2)
      $$invalidate(9, pan_speed = $$props2.pan_speed);
    if ("max_file_size" in $$props2)
      $$invalidate(10, max_file_size = $$props2.max_file_size);
    if ("uploading" in $$props2)
      $$invalidate(1, uploading = $$props2.uploading);
    if ("camera_position" in $$props2)
      $$invalidate(11, camera_position = $$props2.camera_position);
    if ("upload" in $$props2)
      $$invalidate(12, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(13, stream_handler = $$props2.stream_handler);
    if ("$$scope" in $$props2)
      $$invalidate(27, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value, use_3dgs*/
    16385) {
      if (value) {
        $$invalidate(14, use_3dgs = value.path.endsWith(".splat") || value.path.endsWith(".ply"));
        if (use_3dgs) {
          loadCanvas3DGS().then((component) => {
            $$invalidate(16, Canvas3DGSComponent = component);
          });
        } else {
          loadCanvas3D().then((component) => {
            $$invalidate(17, Canvas3DComponent = component);
          });
        }
      }
    }
    if ($$self.$$.dirty & /*dragging*/
    32768) {
      dispatch("drag", dragging);
    }
  };
  return [
    value,
    uploading,
    display_mode,
    clear_color,
    label,
    show_label,
    root,
    i18n,
    zoom_speed,
    pan_speed,
    max_file_size,
    camera_position,
    upload,
    stream_handler,
    use_3dgs,
    dragging,
    Canvas3DGSComponent,
    Canvas3DComponent,
    canvas3d,
    handle_upload,
    handle_clear,
    handle_undo,
    slots,
    upload_1_dragging_binding,
    upload_1_uploading_binding,
    error_handler,
    switch_instance_binding,
    $$scope
  ];
}
class Model3DUpload extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      value: 0,
      display_mode: 2,
      clear_color: 3,
      label: 4,
      show_label: 5,
      root: 6,
      i18n: 7,
      zoom_speed: 8,
      pan_speed: 9,
      max_file_size: 10,
      uploading: 1,
      camera_position: 11,
      upload: 12,
      stream_handler: 13
    });
  }
}
const Model3DUpload$1 = Model3DUpload;
function create_else_block_1(ctx) {
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
        ctx[20] ? "focus" : "base"
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
      height: (
        /*height*/
        ctx[15]
      ),
      $$slots: { default: [create_default_slot_2] },
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
      if (dirty[0] & /*value*/
      1)
        block_changes.variant = /*value*/
        ctx2[0] === null ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      1048576)
        block_changes.border_mode = /*dragging*/
        ctx2[20] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty[0] & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
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
      if (dirty[0] & /*height*/
      32768)
        block_changes.height = /*height*/
        ctx2[15];
      if (dirty[0] & /*label, show_label, root, display_mode, clear_color, value, camera_position, zoom_speed, gradio, uploading, dragging, has_change_history, loading_status*/
      1787847 | dirty[1] & /*$$scope*/
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
function create_if_block(ctx) {
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
        ctx[20] ? "focus" : "base"
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
      height: (
        /*height*/
        ctx[15]
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
      if (dirty[0] & /*value*/
      1)
        block_changes.variant = /*value*/
        ctx2[0] === null ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      1048576)
        block_changes.border_mode = /*dragging*/
        ctx2[20] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      8)
        block_changes.elem_id = /*elem_id*/
        ctx2[3];
      if (dirty[0] & /*elem_classes*/
      16)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[4];
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
      if (dirty[0] & /*height*/
      32768)
        block_changes.height = /*height*/
        ctx2[15];
      if (dirty[0] & /*value, gradio, display_mode, clear_color, label, show_label, camera_position, zoom_speed, has_change_history, loading_status*/
      214919 | dirty[1] & /*$$scope*/
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
function create_default_slot_3(ctx) {
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
function create_default_slot_2(ctx) {
  let statustracker;
  let t;
  let model3dupload;
  let updating_uploading;
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
    ctx[24]
  );
  function model3dupload_uploading_binding(value) {
    ctx[27](value);
  }
  let model3dupload_props = {
    label: (
      /*label*/
      ctx[9]
    ),
    show_label: (
      /*show_label*/
      ctx[10]
    ),
    root: (
      /*root*/
      ctx[6]
    ),
    display_mode: (
      /*display_mode*/
      ctx[7]
    ),
    clear_color: (
      /*clear_color*/
      ctx[8]
    ),
    value: (
      /*value*/
      ctx[0]
    ),
    camera_position: (
      /*camera_position*/
      ctx[17]
    ),
    zoom_speed: (
      /*zoom_speed*/
      ctx[16]
    ),
    i18n: (
      /*gradio*/
      ctx[14].i18n
    ),
    max_file_size: (
      /*gradio*/
      ctx[14].max_file_size
    ),
    upload: (
      /*func*/
      ctx[25]
    ),
    stream_handler: (
      /*func_1*/
      ctx[26]
    ),
    $$slots: { default: [create_default_slot_3] },
    $$scope: { ctx }
  };
  if (
    /*uploading*/
    ctx[19] !== void 0
  ) {
    model3dupload_props.uploading = /*uploading*/
    ctx[19];
  }
  model3dupload = new Model3DUpload$1({ props: model3dupload_props });
  binding_callbacks.push(() => bind(model3dupload, "uploading", model3dupload_uploading_binding));
  model3dupload.$on(
    "change",
    /*change_handler*/
    ctx[28]
  );
  model3dupload.$on(
    "drag",
    /*drag_handler*/
    ctx[29]
  );
  model3dupload.$on(
    "change",
    /*change_handler_1*/
    ctx[30]
  );
  model3dupload.$on(
    "clear",
    /*clear_handler*/
    ctx[31]
  );
  model3dupload.$on(
    "load",
    /*load_handler*/
    ctx[32]
  );
  model3dupload.$on(
    "error",
    /*error_handler*/
    ctx[33]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      create_component(model3dupload.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(model3dupload.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(model3dupload, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
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
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const model3dupload_changes = {};
      if (dirty[0] & /*label*/
      512)
        model3dupload_changes.label = /*label*/
        ctx2[9];
      if (dirty[0] & /*show_label*/
      1024)
        model3dupload_changes.show_label = /*show_label*/
        ctx2[10];
      if (dirty[0] & /*root*/
      64)
        model3dupload_changes.root = /*root*/
        ctx2[6];
      if (dirty[0] & /*display_mode*/
      128)
        model3dupload_changes.display_mode = /*display_mode*/
        ctx2[7];
      if (dirty[0] & /*clear_color*/
      256)
        model3dupload_changes.clear_color = /*clear_color*/
        ctx2[8];
      if (dirty[0] & /*value*/
      1)
        model3dupload_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*camera_position*/
      131072)
        model3dupload_changes.camera_position = /*camera_position*/
        ctx2[17];
      if (dirty[0] & /*zoom_speed*/
      65536)
        model3dupload_changes.zoom_speed = /*zoom_speed*/
        ctx2[16];
      if (dirty[0] & /*gradio*/
      16384)
        model3dupload_changes.i18n = /*gradio*/
        ctx2[14].i18n;
      if (dirty[0] & /*gradio*/
      16384)
        model3dupload_changes.max_file_size = /*gradio*/
        ctx2[14].max_file_size;
      if (dirty[0] & /*gradio*/
      16384)
        model3dupload_changes.upload = /*func*/
        ctx2[25];
      if (dirty[0] & /*gradio*/
      16384)
        model3dupload_changes.stream_handler = /*func_1*/
        ctx2[26];
      if (dirty[0] & /*gradio*/
      16384 | dirty[1] & /*$$scope*/
      8) {
        model3dupload_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      524288) {
        updating_uploading = true;
        model3dupload_changes.uploading = /*uploading*/
        ctx2[19];
        add_flush_callback(() => updating_uploading = false);
      }
      model3dupload.$set(model3dupload_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(model3dupload.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(model3dupload.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(statustracker, detaching);
      destroy_component(model3dupload, detaching);
    }
  };
}
function create_else_block(ctx) {
  let blocklabel;
  let t;
  let empty_1;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[10]
      ),
      Icon: File,
      label: (
        /*label*/
        ctx[9] || "3D Model"
      )
    }
  });
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      create_component(empty_1.$$.fragment);
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(empty_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(empty_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      1024)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[10];
      if (dirty[0] & /*label*/
      512)
        blocklabel_changes.label = /*label*/
        ctx2[9] || "3D Model";
      blocklabel.$set(blocklabel_changes);
      const empty_1_changes = {};
      if (dirty[1] & /*$$scope*/
      8) {
        empty_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      empty_1.$set(empty_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      transition_in(empty_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(empty_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(blocklabel, detaching);
      destroy_component(empty_1, detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let model3d;
  let current;
  model3d = new Model3D$1({
    props: {
      value: (
        /*value*/
        ctx[0]
      ),
      i18n: (
        /*gradio*/
        ctx[14].i18n
      ),
      display_mode: (
        /*display_mode*/
        ctx[7]
      ),
      clear_color: (
        /*clear_color*/
        ctx[8]
      ),
      label: (
        /*label*/
        ctx[9]
      ),
      show_label: (
        /*show_label*/
        ctx[10]
      ),
      camera_position: (
        /*camera_position*/
        ctx[17]
      ),
      zoom_speed: (
        /*zoom_speed*/
        ctx[16]
      ),
      has_change_history: (
        /*has_change_history*/
        ctx[2]
      )
    }
  });
  return {
    c() {
      create_component(model3d.$$.fragment);
    },
    l(nodes) {
      claim_component(model3d.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(model3d, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const model3d_changes = {};
      if (dirty[0] & /*value*/
      1)
        model3d_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*gradio*/
      16384)
        model3d_changes.i18n = /*gradio*/
        ctx2[14].i18n;
      if (dirty[0] & /*display_mode*/
      128)
        model3d_changes.display_mode = /*display_mode*/
        ctx2[7];
      if (dirty[0] & /*clear_color*/
      256)
        model3d_changes.clear_color = /*clear_color*/
        ctx2[8];
      if (dirty[0] & /*label*/
      512)
        model3d_changes.label = /*label*/
        ctx2[9];
      if (dirty[0] & /*show_label*/
      1024)
        model3d_changes.show_label = /*show_label*/
        ctx2[10];
      if (dirty[0] & /*camera_position*/
      131072)
        model3d_changes.camera_position = /*camera_position*/
        ctx2[17];
      if (dirty[0] & /*zoom_speed*/
      65536)
        model3d_changes.zoom_speed = /*zoom_speed*/
        ctx2[16];
      if (dirty[0] & /*has_change_history*/
      4)
        model3d_changes.has_change_history = /*has_change_history*/
        ctx2[2];
      model3d.$set(model3d_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(model3d.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(model3d.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(model3d, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let file;
  let current;
  file = new File({});
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
function create_default_slot(ctx) {
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
    ctx[23]
  );
  const if_block_creators = [create_if_block_1, create_else_block];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] && /*is_browser*/
      ctx2[21]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
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
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
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
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_else_block_1];
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
  let { root } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color } = $$props;
  let { loading_status } = $$props;
  let { label } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { height = void 0 } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let { has_change_history = false } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { interactive } = $$props;
  let dragging = false;
  const is_browser = typeof window !== "undefined";
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const clear_status_handler_1 = () => gradio.dispatch("clear_status", loading_status);
  const func = (...args) => gradio.client.upload(...args);
  const func_1 = (...args) => gradio.client.stream(...args);
  function model3dupload_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(19, uploading);
  }
  const change_handler = ({ detail }) => $$invalidate(0, value = detail);
  const drag_handler = ({ detail }) => $$invalidate(20, dragging = detail);
  const change_handler_1 = ({ detail }) => {
    gradio.dispatch("change", detail);
    $$invalidate(2, has_change_history = true);
  };
  const clear_handler = () => {
    $$invalidate(0, value = null);
    gradio.dispatch("clear");
  };
  const load_handler = ({ detail }) => {
    $$invalidate(0, value = detail);
    gradio.dispatch("upload");
  };
  const error_handler = ({ detail }) => {
    $$invalidate(1, loading_status = loading_status || {});
    $$invalidate(1, loading_status.status = "error", loading_status);
    gradio.dispatch("error", detail);
  };
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(3, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(4, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(5, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("display_mode" in $$props2)
      $$invalidate(7, display_mode = $$props2.display_mode);
    if ("clear_color" in $$props2)
      $$invalidate(8, clear_color = $$props2.clear_color);
    if ("loading_status" in $$props2)
      $$invalidate(1, loading_status = $$props2.loading_status);
    if ("label" in $$props2)
      $$invalidate(9, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(10, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(11, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(12, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(13, min_width = $$props2.min_width);
    if ("gradio" in $$props2)
      $$invalidate(14, gradio = $$props2.gradio);
    if ("height" in $$props2)
      $$invalidate(15, height = $$props2.height);
    if ("zoom_speed" in $$props2)
      $$invalidate(16, zoom_speed = $$props2.zoom_speed);
    if ("input_ready" in $$props2)
      $$invalidate(22, input_ready = $$props2.input_ready);
    if ("has_change_history" in $$props2)
      $$invalidate(2, has_change_history = $$props2.has_change_history);
    if ("camera_position" in $$props2)
      $$invalidate(17, camera_position = $$props2.camera_position);
    if ("interactive" in $$props2)
      $$invalidate(18, interactive = $$props2.interactive);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*uploading*/
    524288) {
      $$invalidate(22, input_ready = !uploading);
    }
  };
  return [
    value,
    loading_status,
    has_change_history,
    elem_id,
    elem_classes,
    visible,
    root,
    display_mode,
    clear_color,
    label,
    show_label,
    container,
    scale,
    min_width,
    gradio,
    height,
    zoom_speed,
    camera_position,
    interactive,
    uploading,
    dragging,
    is_browser,
    input_ready,
    clear_status_handler,
    clear_status_handler_1,
    func,
    func_1,
    model3dupload_uploading_binding,
    change_handler,
    drag_handler,
    change_handler_1,
    clear_handler,
    load_handler,
    error_handler
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
        elem_id: 3,
        elem_classes: 4,
        visible: 5,
        value: 0,
        root: 6,
        display_mode: 7,
        clear_color: 8,
        loading_status: 1,
        label: 9,
        show_label: 10,
        container: 11,
        scale: 12,
        min_width: 13,
        gradio: 14,
        height: 15,
        zoom_speed: 16,
        input_ready: 22,
        has_change_history: 2,
        camera_position: 17,
        interactive: 18
      },
      null,
      [-1, -1]
    );
  }
}
export {
  default2 as BaseExample,
  Model3D$1 as BaseModel3D,
  Model3DUpload$1 as BaseModel3DUpload,
  Index as default
};
