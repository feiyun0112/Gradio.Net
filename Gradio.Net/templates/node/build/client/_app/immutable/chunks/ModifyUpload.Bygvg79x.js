import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, createEventDispatcher, create_slot, space, claim_space, group_outros, check_outros, update_slot_base, get_all_dirty_from_scope, get_slot_changes } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { I as IconButton, C as Clear } from "./2.BqWhUxOo.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { U as Undo } from "./Undo.BEjgqHJW.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
function create_fragment$1(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true,
        class: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "stroke-linecap", "round");
      attr(svg, "stroke-linejoin", "round");
      attr(svg, "class", "feather feather-edit-2");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class Edit extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$1, safe_not_equal, {});
  }
}
function create_if_block_2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Edit,
      label: (
        /*i18n*/
        ctx[3]("common.edit")
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[6]
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
      if (dirty & /*i18n*/
      8)
        iconbutton_changes.label = /*i18n*/
        ctx2[3]("common.edit");
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
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Undo,
      label: (
        /*i18n*/
        ctx[3]("common.undo")
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler_1*/
    ctx[7]
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
      if (dirty & /*i18n*/
      8)
        iconbutton_changes.label = /*i18n*/
        ctx2[3]("common.undo");
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
function create_if_block(ctx) {
  let downloadlink;
  let current;
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*download*/
        ctx[2]
      ),
      download: true,
      $$slots: { default: [create_default_slot_1] },
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
      if (dirty & /*download*/
      4)
        downloadlink_changes.href = /*download*/
        ctx2[2];
      if (dirty & /*$$scope, i18n*/
      520) {
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
function create_default_slot_1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Download,
      label: (
        /*i18n*/
        ctx[3]("common.download")
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
      8)
        iconbutton_changes.label = /*i18n*/
        ctx2[3]("common.download");
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
function create_default_slot(ctx) {
  let t0;
  let t1;
  let t2;
  let t3;
  let iconbutton;
  let current;
  let if_block0 = (
    /*editable*/
    ctx[0] && create_if_block_2(ctx)
  );
  let if_block1 = (
    /*undoable*/
    ctx[1] && create_if_block_1(ctx)
  );
  let if_block2 = (
    /*download*/
    ctx[2] && create_if_block(ctx)
  );
  const default_slot_template = (
    /*#slots*/
    ctx[5].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[9],
    null
  );
  iconbutton = new IconButton({
    props: {
      Icon: Clear,
      label: (
        /*i18n*/
        ctx[3]("common.clear")
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler_2*/
    ctx[8]
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
      if (default_slot)
        default_slot.c();
      t3 = space();
      create_component(iconbutton.$$.fragment);
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
      if (default_slot)
        default_slot.l(nodes);
      t3 = claim_space(nodes);
      claim_component(iconbutton.$$.fragment, nodes);
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
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      insert_hydration(target, t3, anchor);
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*editable*/
        ctx2[0]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*editable*/
          1) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2(ctx2);
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
        /*undoable*/
        ctx2[1]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*undoable*/
          2) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_1(ctx2);
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
        /*download*/
        ctx2[2]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty & /*download*/
          4) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block(ctx2);
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
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        512)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[9],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[9]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[9],
              dirty,
              null
            ),
            null
          );
        }
      }
      const iconbutton_changes = {};
      if (dirty & /*i18n*/
      8)
        iconbutton_changes.label = /*i18n*/
        ctx2[3]("common.clear");
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(if_block2);
      transition_in(default_slot, local);
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      transition_out(default_slot, local);
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(t2);
        detach(t3);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
      if (if_block2)
        if_block2.d(detaching);
      if (default_slot)
        default_slot.d(detaching);
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_fragment(ctx) {
  let iconbuttonwrapper;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, i18n, download, undoable, editable*/
      527) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
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
      destroy_component(iconbuttonwrapper, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { editable = false } = $$props;
  let { undoable = false } = $$props;
  let { download = null } = $$props;
  let { i18n } = $$props;
  const dispatch = createEventDispatcher();
  const click_handler = () => dispatch("edit");
  const click_handler_1 = () => dispatch("undo");
  const click_handler_2 = (event) => {
    dispatch("clear");
    event.stopPropagation();
  };
  $$self.$$set = ($$props2) => {
    if ("editable" in $$props2)
      $$invalidate(0, editable = $$props2.editable);
    if ("undoable" in $$props2)
      $$invalidate(1, undoable = $$props2.undoable);
    if ("download" in $$props2)
      $$invalidate(2, download = $$props2.download);
    if ("i18n" in $$props2)
      $$invalidate(3, i18n = $$props2.i18n);
    if ("$$scope" in $$props2)
      $$invalidate(9, $$scope = $$props2.$$scope);
  };
  return [
    editable,
    undoable,
    download,
    i18n,
    dispatch,
    slots,
    click_handler,
    click_handler_1,
    click_handler_2,
    $$scope
  ];
}
class ModifyUpload extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      editable: 0,
      undoable: 1,
      download: 2,
      i18n: 3
    });
  }
}
export {
  ModifyUpload as M
};
