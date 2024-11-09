import { SvelteComponent, init, safe_not_equal, create_slot, element, space, claim_element, children, claim_space, detach, attr, toggle_class, set_style, insert_hydration, append_hydration, transition_in, group_outros, transition_out, check_outros, update_slot_base, get_all_dirty_from_scope, get_slot_changes, assign, create_component, claim_component, mount_component, get_spread_update, get_spread_object, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { S as Static } from "./2.BqWhUxOo.js";
function create_if_block(ctx) {
  let statustracker;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[6].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[6].i18n
    ) },
    /*loading_status*/
    ctx[5],
    {
      status: (
        /*loading_status*/
        ctx[5] ? (
          /*loading_status*/
          ctx[5].status == "pending" ? "generating" : (
            /*loading_status*/
            ctx[5].status
          )
        ) : null
      )
    }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  return {
    c() {
      create_component(statustracker.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*gradio, loading_status*/
      96 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        64 && { autoscroll: (
          /*gradio*/
          ctx2[6].autoscroll
        ) },
        dirty & /*gradio*/
        64 && { i18n: (
          /*gradio*/
          ctx2[6].i18n
        ) },
        dirty & /*loading_status*/
        32 && get_spread_object(
          /*loading_status*/
          ctx2[5]
        ),
        dirty & /*loading_status*/
        32 && {
          status: (
            /*loading_status*/
            ctx2[5] ? (
              /*loading_status*/
              ctx2[5].status == "pending" ? "generating" : (
                /*loading_status*/
                ctx2[5].status
              )
            ) : null
          )
        }
      ]) : {};
      statustracker.$set(statustracker_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(statustracker, detaching);
    }
  };
}
function create_fragment(ctx) {
  let div;
  let t;
  let div_class_value;
  let current;
  let if_block = (
    /*loading_status*/
    ctx[5] && /*show_progress*/
    ctx[7] && /*gradio*/
    ctx[6] && create_if_block(ctx)
  );
  const default_slot_template = (
    /*#slots*/
    ctx[13].default
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
      div = element("div");
      if (if_block)
        if_block.c();
      t = space();
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { id: true, class: true });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      t = claim_space(div_nodes);
      if (default_slot)
        default_slot.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(
        div,
        "id",
        /*elem_id*/
        ctx[1]
      );
      attr(div, "class", div_class_value = "row " + /*elem_classes*/
      ctx[2].join(" ") + " svelte-hrj4a0");
      toggle_class(
        div,
        "compact",
        /*variant*/
        ctx[4] === "compact"
      );
      toggle_class(
        div,
        "panel",
        /*variant*/
        ctx[4] === "panel"
      );
      toggle_class(
        div,
        "unequal-height",
        /*equal_height*/
        ctx[0] === false
      );
      toggle_class(
        div,
        "stretch",
        /*equal_height*/
        ctx[0]
      );
      toggle_class(div, "hide", !/*visible*/
      ctx[3]);
      set_style(
        div,
        "height",
        /*get_dimension*/
        ctx[11](
          /*height*/
          ctx[8]
        )
      );
      set_style(
        div,
        "max-height",
        /*get_dimension*/
        ctx[11](
          /*max_height*/
          ctx[10]
        )
      );
      set_style(
        div,
        "min-height",
        /*get_dimension*/
        ctx[11](
          /*min_height*/
          ctx[9]
        )
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (if_block)
        if_block.m(div, null);
      append_hydration(div, t);
      if (default_slot) {
        default_slot.m(div, null);
      }
      current = true;
    },
    p(ctx2, [dirty]) {
      if (
        /*loading_status*/
        ctx2[5] && /*show_progress*/
        ctx2[7] && /*gradio*/
        ctx2[6]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*loading_status, show_progress, gradio*/
          224) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(div, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
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
      if (!current || dirty & /*elem_id*/
      2) {
        attr(
          div,
          "id",
          /*elem_id*/
          ctx2[1]
        );
      }
      if (!current || dirty & /*elem_classes*/
      4 && div_class_value !== (div_class_value = "row " + /*elem_classes*/
      ctx2[2].join(" ") + " svelte-hrj4a0")) {
        attr(div, "class", div_class_value);
      }
      if (!current || dirty & /*elem_classes, variant*/
      20) {
        toggle_class(
          div,
          "compact",
          /*variant*/
          ctx2[4] === "compact"
        );
      }
      if (!current || dirty & /*elem_classes, variant*/
      20) {
        toggle_class(
          div,
          "panel",
          /*variant*/
          ctx2[4] === "panel"
        );
      }
      if (!current || dirty & /*elem_classes, equal_height*/
      5) {
        toggle_class(
          div,
          "unequal-height",
          /*equal_height*/
          ctx2[0] === false
        );
      }
      if (!current || dirty & /*elem_classes, equal_height*/
      5) {
        toggle_class(
          div,
          "stretch",
          /*equal_height*/
          ctx2[0]
        );
      }
      if (!current || dirty & /*elem_classes, visible*/
      12) {
        toggle_class(div, "hide", !/*visible*/
        ctx2[3]);
      }
      if (dirty & /*height*/
      256) {
        set_style(
          div,
          "height",
          /*get_dimension*/
          ctx2[11](
            /*height*/
            ctx2[8]
          )
        );
      }
      if (dirty & /*max_height*/
      1024) {
        set_style(
          div,
          "max-height",
          /*get_dimension*/
          ctx2[11](
            /*max_height*/
            ctx2[10]
          )
        );
      }
      if (dirty & /*min_height*/
      512) {
        set_style(
          div,
          "min-height",
          /*get_dimension*/
          ctx2[11](
            /*min_height*/
            ctx2[9]
          )
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(default_slot, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(default_slot, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (if_block)
        if_block.d();
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { equal_height = true } = $$props;
  let { elem_id } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { variant = "default" } = $$props;
  let { loading_status = void 0 } = $$props;
  let { gradio = void 0 } = $$props;
  let { show_progress = false } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  const get_dimension = (dimension_value) => {
    if (dimension_value === void 0) {
      return void 0;
    }
    if (typeof dimension_value === "number") {
      return dimension_value + "px";
    } else if (typeof dimension_value === "string") {
      return dimension_value;
    }
  };
  $$self.$$set = ($$props2) => {
    if ("equal_height" in $$props2)
      $$invalidate(0, equal_height = $$props2.equal_height);
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("variant" in $$props2)
      $$invalidate(4, variant = $$props2.variant);
    if ("loading_status" in $$props2)
      $$invalidate(5, loading_status = $$props2.loading_status);
    if ("gradio" in $$props2)
      $$invalidate(6, gradio = $$props2.gradio);
    if ("show_progress" in $$props2)
      $$invalidate(7, show_progress = $$props2.show_progress);
    if ("height" in $$props2)
      $$invalidate(8, height = $$props2.height);
    if ("min_height" in $$props2)
      $$invalidate(9, min_height = $$props2.min_height);
    if ("max_height" in $$props2)
      $$invalidate(10, max_height = $$props2.max_height);
    if ("$$scope" in $$props2)
      $$invalidate(12, $$scope = $$props2.$$scope);
  };
  return [
    equal_height,
    elem_id,
    elem_classes,
    visible,
    variant,
    loading_status,
    gradio,
    show_progress,
    height,
    min_height,
    max_height,
    get_dimension,
    $$scope,
    slots
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      equal_height: 0,
      elem_id: 1,
      elem_classes: 2,
      visible: 3,
      variant: 4,
      loading_status: 5,
      gradio: 6,
      show_progress: 7,
      height: 8,
      min_height: 9,
      max_height: 10
    });
  }
}
export {
  Index as default
};
