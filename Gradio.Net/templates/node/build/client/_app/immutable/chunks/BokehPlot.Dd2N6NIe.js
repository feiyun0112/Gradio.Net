import { SvelteComponent, init, safe_not_equal, element, claim_element, children, detach, attr, insert_hydration, noop, createEventDispatcher, onDestroy } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
function create_fragment(ctx) {
  let div;
  return {
    c() {
      div = element("div");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        "data-testid": true,
        id: true,
        class: true
      });
      children(div).forEach(detach);
      this.h();
    },
    h() {
      attr(div, "data-testid", "bokeh");
      attr(
        div,
        "id",
        /*div_id*/
        ctx[0]
      );
      attr(div, "class", "gradio-bokeh svelte-1rhu6ax");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let plot;
  let { value } = $$props;
  let { bokeh_version } = $$props;
  const div_id = `bokehDiv-${Math.random().toString(5).substring(2)}`;
  const dispatch = createEventDispatcher();
  async function embed_bokeh(_plot) {
    if (document) {
      if (document.getElementById(div_id)) {
        document.getElementById(div_id).innerHTML = "";
      }
    }
    if (window.Bokeh) {
      load_bokeh();
      let plotObj = JSON.parse(_plot);
      const y = await window.Bokeh.embed.embed_item(plotObj, div_id);
      y._roots.forEach(async (p) => {
        await p.ready;
        dispatch("load");
      });
    }
  }
  const main_src = `https://cdn.bokeh.org/bokeh/release/bokeh-${bokeh_version}.min.js`;
  const plugins_src = [
    `https://cdn.pydata.org/bokeh/release/bokeh-widgets-${bokeh_version}.min.js`,
    `https://cdn.pydata.org/bokeh/release/bokeh-tables-${bokeh_version}.min.js`,
    `https://cdn.pydata.org/bokeh/release/bokeh-gl-${bokeh_version}.min.js`,
    `https://cdn.pydata.org/bokeh/release/bokeh-api-${bokeh_version}.min.js`
  ];
  let loaded = false;
  async function load_plugins() {
    await Promise.all(plugins_src.map((src, i) => {
      return new Promise((resolve) => {
        const script = document.createElement("script");
        script.onload = resolve;
        script.src = src;
        document.head.appendChild(script);
        return script;
      });
    }));
    $$invalidate(3, loaded = true);
  }
  let plugin_scripts = [];
  function handle_bokeh_loaded() {
    plugin_scripts = load_plugins();
  }
  function load_bokeh() {
    const script = document.createElement("script");
    script.onload = handle_bokeh_loaded;
    script.src = main_src;
    const is_bokeh_script_present = document.head.querySelector(`script[src="${main_src}"]`);
    if (!is_bokeh_script_present) {
      document.head.appendChild(script);
    } else {
      handle_bokeh_loaded();
    }
    return script;
  }
  const main_script = bokeh_version ? load_bokeh() : null;
  onDestroy(() => {
    if (main_script in document.children) {
      document.removeChild(main_script);
      plugin_scripts.forEach((child) => document.removeChild(child));
    }
  });
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(1, value = $$props2.value);
    if ("bokeh_version" in $$props2)
      $$invalidate(2, bokeh_version = $$props2.bokeh_version);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    2) {
      $$invalidate(4, plot = value == null ? void 0 : value.plot);
    }
    if ($$self.$$.dirty & /*loaded, plot*/
    24) {
      loaded && embed_bokeh(plot);
    }
  };
  return [div_id, value, bokeh_version, loaded, plot];
}
class BokehPlot extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, { value: 1, bokeh_version: 2 });
  }
}
export {
  BokehPlot as default
};
