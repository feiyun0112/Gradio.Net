import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, createEventDispatcher } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { I as IconButton, d as ShareError } from "./2.BqWhUxOo.js";
import { C as Community } from "./Community.4FBzcHeP.js";
function create_fragment(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Community,
      label: (
        /*i18n*/
        ctx[2]("common.share")
      ),
      pending: (
        /*pending*/
        ctx[3]
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[5]
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
    p(ctx2, [dirty]) {
      const iconbutton_changes = {};
      if (dirty & /*i18n*/
      4)
        iconbutton_changes.label = /*i18n*/
        ctx2[2]("common.share");
      if (dirty & /*pending*/
      8)
        iconbutton_changes.pending = /*pending*/
        ctx2[3];
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
function instance($$self, $$props, $$invalidate) {
  const dispatch = createEventDispatcher();
  let { formatter } = $$props;
  let { value } = $$props;
  let { i18n } = $$props;
  let pending = false;
  const click_handler = async () => {
    try {
      $$invalidate(3, pending = true);
      const formatted = await formatter(value);
      dispatch("share", { description: formatted });
    } catch (e) {
      console.error(e);
      let message = e instanceof ShareError ? e.message : "Share failed.";
      dispatch("error", message);
    } finally {
      $$invalidate(3, pending = false);
    }
  };
  $$self.$$set = ($$props2) => {
    if ("formatter" in $$props2)
      $$invalidate(0, formatter = $$props2.formatter);
    if ("value" in $$props2)
      $$invalidate(1, value = $$props2.value);
    if ("i18n" in $$props2)
      $$invalidate(2, i18n = $$props2.i18n);
  };
  return [formatter, value, i18n, pending, dispatch, click_handler];
}
class ShareButton extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { formatter: 0, value: 1, i18n: 2 });
  }
}
export {
  ShareButton as S
};
