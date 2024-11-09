import { c as create_ssr_component } from './ssr-RaXq3SJh.js';
import { s as svelte, S as SvelteComponentDev } from './ssr-Cql56Qn_.js';
import './Component-Dv7eSVA_.js';

const is_browser = typeof window !== "undefined";
if (is_browser) {
  const o = {
    SvelteComponent: SvelteComponentDev
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
const css = {
  code: "body{background:var(--body-background-fill);color:var(--body-text-color)}",
  map: '{"version":3,"file":"+layout.svelte","sources":["+layout.svelte"],"sourcesContent":["<script>\\n\\timport \\"@gradio/theme/reset.css\\";\\n\\timport \\"@gradio/theme/global.css\\";\\n\\timport \\"@gradio/theme/pollen.css\\";\\n\\timport \\"@gradio/theme/typography.css\\";\\n\\timport \\"./svelte_init\\";\\n<\/script>\\n\\n<slot></slot>\\n\\n<style>\\n\\t:global(body) {\\n\\t\\tbackground: var(--body-background-fill);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}</style>\\n"],"names":[],"mappings":"AAWS,IAAM,CACb,UAAU,CAAE,IAAI,sBAAsB,CAAC,CACvC,KAAK,CAAE,IAAI,iBAAiB,CAC7B"}'
};
const Layout = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  $$result.css.add(css);
  return `${slots.default ? slots.default({}) : ``}`;
});

export { Layout as default };
//# sourceMappingURL=_layout.svelte-Ba36fIRN.js.map
