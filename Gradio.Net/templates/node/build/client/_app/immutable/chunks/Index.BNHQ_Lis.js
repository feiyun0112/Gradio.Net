import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, text, claim_text, insert_hydration, set_data, detach } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { b as Button } from "./2.BqWhUxOo.js";
function create_default_slot(ctx) {
  let t_value = (
    /*value*/
    (ctx[3] ? (
      /*gradio*/
      ctx[11].i18n(
        /*value*/
        ctx[3]
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
      if (dirty & /*value, gradio*/
      2056 && t_value !== (t_value = /*value*/
      (ctx2[3] ? (
        /*gradio*/
        ctx2[11].i18n(
          /*value*/
          ctx2[3]
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
  let button;
  let current;
  button = new Button({
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
      link: (
        /*link*/
        ctx[9]
      ),
      icon: (
        /*icon*/
        ctx[8]
      ),
      min_width: (
        /*min_width*/
        ctx[10]
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
  button.$on(
    "click",
    /*click_handler*/
    ctx[12]
  );
  return {
    c() {
      create_component(button.$$.fragment);
    },
    l(nodes) {
      claim_component(button.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(button, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const button_changes = {};
      if (dirty & /*value*/
      8)
        button_changes.value = /*value*/
        ctx2[3];
      if (dirty & /*variant*/
      16)
        button_changes.variant = /*variant*/
        ctx2[4];
      if (dirty & /*elem_id*/
      1)
        button_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        button_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*size*/
      64)
        button_changes.size = /*size*/
        ctx2[6];
      if (dirty & /*scale*/
      128)
        button_changes.scale = /*scale*/
        ctx2[7];
      if (dirty & /*link*/
      512)
        button_changes.link = /*link*/
        ctx2[9];
      if (dirty & /*icon*/
      256)
        button_changes.icon = /*icon*/
        ctx2[8];
      if (dirty & /*min_width*/
      1024)
        button_changes.min_width = /*min_width*/
        ctx2[10];
      if (dirty & /*visible*/
      4)
        button_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*interactive*/
      32)
        button_changes.disabled = !/*interactive*/
        ctx2[5];
      if (dirty & /*$$scope, value, gradio*/
      10248) {
        button_changes.$$scope = { dirty, ctx: ctx2 };
      }
      button.$set(button_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(button.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(button.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(button, detaching);
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
  let { link = null } = $$props;
  let { min_width = void 0 } = $$props;
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
    if ("link" in $$props2)
      $$invalidate(9, link = $$props2.link);
    if ("min_width" in $$props2)
      $$invalidate(10, min_width = $$props2.min_width);
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
    link,
    min_width,
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
      link: 9,
      min_width: 10,
      gradio: 11
    });
  }
}
export {
  Button as BaseButton,
  Index as default
};
