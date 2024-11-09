import { SvelteComponent, init, safe_not_equal } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, null, safe_not_equal, {});
  }
}
export {
  Index as default
};
