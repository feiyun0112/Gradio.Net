import { c as create_ssr_component, e as escape } from './ssr-RaXq3SJh.js';

const css = {
  code: "div.svelte-rgtszb{overflow:hidden;text-overflow:ellipsis;white-space:nowrap}.gallery.svelte-rgtszb{display:flex;align-items:center;cursor:pointer;padding:var(--size-1) var(--size-2);text-align:left}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\n<\/script>\\n\\n<div\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n>\\n\\t{value ? (Array.isArray(value) ? value.join(\\", \\") : value) : \\"\\"}\\n</div>\\n\\n<style>\\n\\tdiv {\\n\\t\\toverflow: hidden;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\twhite-space: nowrap;\\n\\t}\\n\\t.gallery {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tcursor: pointer;\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\ttext-align: left;\\n\\t}</style>\\n"],"names":[],"mappings":"AAcC,iBAAI,CACH,QAAQ,CAAE,MAAM,CAChB,aAAa,CAAE,QAAQ,CACvB,WAAW,CAAE,MACd,CACA,sBAAS,CACR,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,OAAO,CACf,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,UAAU,CAAE,IACb"}'
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
  return `<div class="${[
    "svelte-rgtszb",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}">${escape(value ? Array.isArray(value) ? value.join(", ") : value : "")} </div>`;
});

export { Example as default };
//# sourceMappingURL=Example11-BratyhqV.js.map
