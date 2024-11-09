import { c as create_ssr_component, e as escape } from './ssr-RaXq3SJh.js';

const css = {
  code: "div.svelte-h6ogpl{width:var(--size-10);height:var(--size-10)}.table.svelte-h6ogpl{margin:0 auto}",
  map: `{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\n<\/script>\\n\\n<div\\n\\tstyle=\\"background-color: {value ? value : 'black'}\\"\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n/>\\n\\n<style>\\n\\tdiv {\\n\\t\\twidth: var(--size-10);\\n\\t\\theight: var(--size-10);\\n\\t}\\n\\t.table {\\n\\t\\tmargin: 0 auto;\\n\\t}</style>\\n"],"names":[],"mappings":"AAaC,iBAAI,CACH,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,MAAM,CAAE,IAAI,SAAS,CACtB,CACA,oBAAO,CACN,MAAM,CAAE,CAAC,CAAC,IACX"}`
};
const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  $$result.css.add(css);
  return `<div style="${"background-color: " + escape(value ? value : "black", true)}" class="${[
    "svelte-h6ogpl",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}"></div>`;
});

export { Example as default };
//# sourceMappingURL=Example7-DJ2n4yYp.js.map
