import { c as create_ssr_component, b as add_attribute } from './ssr-RaXq3SJh.js';

const css = {
  code: ".layout.svelte-j1jcu3.svelte-j1jcu3{display:flex;flex-direction:column;justify-content:center;align-items:center;width:var(--size-full);height:var(--size-full);color:var(--body-text-color)}.matplotlib.svelte-j1jcu3 img.svelte-j1jcu3{object-fit:contain}",
  map: '{"version":3,"file":"MatplotlibPlot.svelte","sources":["MatplotlibPlot.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\n$: plot = value?.plot;\\n<\/script>\\n\\n<div data-testid={\\"matplotlib\\"} class=\\"matplotlib layout\\">\\n\\t<img\\n\\t\\tsrc={plot}\\n\\t\\talt={`${value.chart} plot visualising provided data`}\\n\\t\\ton:load\\n\\t/>\\n</div>\\n\\n<style>\\n\\t.layout {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\t.matplotlib img {\\n\\t\\tobject-fit: contain;\\n\\t}</style>\\n"],"names":[],"mappings":"AAaC,mCAAQ,CACP,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CACA,yBAAW,CAAC,iBAAI,CACf,UAAU,CAAE,OACb"}'
};
const MatplotlibPlot = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let plot;
  let { value } = $$props;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  $$result.css.add(css);
  plot = value?.plot;
  return `<div${add_attribute("data-testid", "matplotlib", 0)} class="matplotlib layout svelte-j1jcu3"><img${add_attribute("src", plot, 0)}${add_attribute("alt", `${value.chart} plot visualising provided data`, 0)} class="svelte-j1jcu3"> </div>`;
});

export { MatplotlibPlot as default };
//# sourceMappingURL=MatplotlibPlot-DzQIZkcs.js.map
