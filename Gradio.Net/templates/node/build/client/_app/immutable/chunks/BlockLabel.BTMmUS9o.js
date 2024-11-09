import { SvelteComponent, init, safe_not_equal, element, create_component, space, text, claim_element, children, claim_component, detach, claim_space, claim_text, attr, toggle_class, insert_hydration, append_hydration, mount_component, set_data, transition_in, transition_out, destroy_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
function create_fragment(ctx) {
  let label_1;
  let span;
  let icon;
  let t0;
  let t1;
  let current;
  icon = new /*Icon*/
  ctx[1]({});
  return {
    c() {
      label_1 = element("label");
      span = element("span");
      create_component(icon.$$.fragment);
      t0 = space();
      t1 = text(
        /*label*/
        ctx[0]
      );
      this.h();
    },
    l(nodes) {
      label_1 = claim_element(nodes, "LABEL", {
        for: true,
        "data-testid": true,
        class: true
      });
      var label_1_nodes = children(label_1);
      span = claim_element(label_1_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      claim_component(icon.$$.fragment, span_nodes);
      span_nodes.forEach(detach);
      t0 = claim_space(label_1_nodes);
      t1 = claim_text(
        label_1_nodes,
        /*label*/
        ctx[0]
      );
      label_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "svelte-i3tvor");
      attr(label_1, "for", "");
      attr(label_1, "data-testid", "block-label");
      attr(label_1, "class", "svelte-i3tvor");
      toggle_class(label_1, "hide", !/*show_label*/
      ctx[2]);
      toggle_class(label_1, "sr-only", !/*show_label*/
      ctx[2]);
      toggle_class(
        label_1,
        "float",
        /*float*/
        ctx[4]
      );
      toggle_class(
        label_1,
        "hide-label",
        /*disable*/
        ctx[3]
      );
    },
    m(target, anchor) {
      insert_hydration(target, label_1, anchor);
      append_hydration(label_1, span);
      mount_component(icon, span, null);
      append_hydration(label_1, t0);
      append_hydration(label_1, t1);
      current = true;
    },
    p(ctx2, [dirty]) {
      if (!current || dirty & /*label*/
      1)
        set_data(
          t1,
          /*label*/
          ctx2[0]
        );
      if (!current || dirty & /*show_label*/
      4) {
        toggle_class(label_1, "hide", !/*show_label*/
        ctx2[2]);
      }
      if (!current || dirty & /*show_label*/
      4) {
        toggle_class(label_1, "sr-only", !/*show_label*/
        ctx2[2]);
      }
      if (!current || dirty & /*float*/
      16) {
        toggle_class(
          label_1,
          "float",
          /*float*/
          ctx2[4]
        );
      }
      if (!current || dirty & /*disable*/
      8) {
        toggle_class(
          label_1,
          "hide-label",
          /*disable*/
          ctx2[3]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(icon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(icon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(label_1);
      }
      destroy_component(icon);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { label = null } = $$props;
  let { Icon } = $$props;
  let { show_label = true } = $$props;
  let { disable = false } = $$props;
  let { float = true } = $$props;
  $$self.$$set = ($$props2) => {
    if ("label" in $$props2)
      $$invalidate(0, label = $$props2.label);
    if ("Icon" in $$props2)
      $$invalidate(1, Icon = $$props2.Icon);
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("disable" in $$props2)
      $$invalidate(3, disable = $$props2.disable);
    if ("float" in $$props2)
      $$invalidate(4, float = $$props2.float);
  };
  return [label, Icon, show_label, disable, float];
}
class BlockLabel extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      label: 0,
      Icon: 1,
      show_label: 2,
      disable: 3,
      float: 4
    });
  }
}
export {
  BlockLabel as B
};
