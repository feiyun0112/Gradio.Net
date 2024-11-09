import { c as create_ssr_component, a as createEventDispatcher, o as onDestroy, b as add_attribute } from './ssr-RaXq3SJh.js';

const css = {
  code: ".gradio-bokeh.svelte-1rhu6ax{display:flex;justify-content:center}",
  map: '{"version":3,"file":"BokehPlot.svelte","sources":["BokehPlot.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onDestroy, createEventDispatcher } from \\"svelte\\";\\nexport let value;\\nexport let bokeh_version;\\nconst div_id = `bokehDiv-${Math.random().toString(5).substring(2)}`;\\nconst dispatch = createEventDispatcher();\\n$: plot = value?.plot;\\nasync function embed_bokeh(_plot) {\\n    if (document) {\\n        if (document.getElementById(div_id)) {\\n            document.getElementById(div_id).innerHTML = \\"\\";\\n        }\\n    }\\n    if (window.Bokeh) {\\n        load_bokeh();\\n        let plotObj = JSON.parse(_plot);\\n        const y = await window.Bokeh.embed.embed_item(plotObj, div_id);\\n        y._roots.forEach(async (p) => {\\n            await p.ready;\\n            dispatch(\\"load\\");\\n        });\\n    }\\n}\\n$: loaded && embed_bokeh(plot);\\nconst main_src = `https://cdn.bokeh.org/bokeh/release/bokeh-${bokeh_version}.min.js`;\\nconst plugins_src = [\\n    `https://cdn.pydata.org/bokeh/release/bokeh-widgets-${bokeh_version}.min.js`,\\n    `https://cdn.pydata.org/bokeh/release/bokeh-tables-${bokeh_version}.min.js`,\\n    `https://cdn.pydata.org/bokeh/release/bokeh-gl-${bokeh_version}.min.js`,\\n    `https://cdn.pydata.org/bokeh/release/bokeh-api-${bokeh_version}.min.js`\\n];\\nlet loaded = false;\\nasync function load_plugins() {\\n    await Promise.all(plugins_src.map((src, i) => {\\n        return new Promise((resolve) => {\\n            const script = document.createElement(\\"script\\");\\n            script.onload = resolve;\\n            script.src = src;\\n            document.head.appendChild(script);\\n            return script;\\n        });\\n    }));\\n    loaded = true;\\n}\\nlet plugin_scripts = [];\\nfunction handle_bokeh_loaded() {\\n    plugin_scripts = load_plugins();\\n}\\nfunction load_bokeh() {\\n    const script = document.createElement(\\"script\\");\\n    script.onload = handle_bokeh_loaded;\\n    script.src = main_src;\\n    const is_bokeh_script_present = document.head.querySelector(`script[src=\\"${main_src}\\"]`);\\n    if (!is_bokeh_script_present) {\\n        document.head.appendChild(script);\\n    }\\n    else {\\n        handle_bokeh_loaded();\\n    }\\n    return script;\\n}\\nconst main_script = bokeh_version ? load_bokeh() : null;\\nonDestroy(() => {\\n    if (main_script in document.children) {\\n        document.removeChild(main_script);\\n        plugin_scripts.forEach((child) => document.removeChild(child));\\n    }\\n});\\n<\/script>\\n\\n<div data-testid={\\"bokeh\\"} id={div_id} class=\\"gradio-bokeh\\" />\\n\\n<style>\\n\\t.gradio-bokeh {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t}</style>\\n"],"names":[],"mappings":"AAwEC,4BAAc,CACb,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAClB"}'
};
const BokehPlot = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
    loaded = true;
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
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.bokeh_version === void 0 && $$bindings.bokeh_version && bokeh_version !== void 0)
    $$bindings.bokeh_version(bokeh_version);
  $$result.css.add(css);
  plot = value?.plot;
  loaded && embed_bokeh(plot);
  return `<div${add_attribute("data-testid", "bokeh", 0)}${add_attribute("id", div_id, 0)} class="gradio-bokeh svelte-1rhu6ax"></div>`;
});

export { BokehPlot as default };
//# sourceMappingURL=BokehPlot-Bb6OhJ4s.js.map
