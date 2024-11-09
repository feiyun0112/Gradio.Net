import * as svelte from "../../../svelte/svelte.js";
import { SvelteComponent, init, safe_not_equal, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, transition_in, transition_out } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
const is_browser = typeof window !== "undefined";
if (is_browser) {
  const o = {
    SvelteComponent: svelte.SvelteComponent
  };
  for (const key in svelte) {
    if (key === "SvelteComponent")
      continue;
    if (key === "SvelteComponentDev") {
      o[key] = o["SvelteComponent"];
    } else {
      o[key] = svelte[key];
    }
  }
  window.__gradio__svelte__internal = o;
  window.__gradio__svelte__internal["globals"] = {};
  window.globals = window;
}
function create_fragment(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[1].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[0],
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
    p(ctx2, [dirty]) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        1)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[0],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[0]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[0],
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
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  $$self.$$set = ($$props2) => {
    if ("$$scope" in $$props2)
      $$invalidate(0, $$scope = $$props2.$$scope);
  };
  return [$$scope, slots];
}
class Layout extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {});
  }
}
export {
  Layout as component
};
