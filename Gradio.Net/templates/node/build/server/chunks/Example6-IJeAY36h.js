import { c as create_ssr_component, e as escape } from './ssr-RaXq3SJh.js';

const css = {
  code: "pre.svelte-agpzo2{text-align:left}.gallery.svelte-agpzo2{padding:var(--size-1) var(--size-2)}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\n<\/script>\\n\\n<pre\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected>{value ? value : \\"\\"}</pre>\\n\\n<style>\\n\\tpre {\\n\\t\\ttext-align: left;\\n\\t}\\n\\t.gallery {\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t}</style>\\n"],"names":[],"mappings":"AAWC,iBAAI,CACH,UAAU,CAAE,IACb,CACA,sBAAS,CACR,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CACpC"}'
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
  return `<pre class="${[
    "svelte-agpzo2",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}">${escape(value ? value : "")}</pre>`;
});

export { Example as default };
//# sourceMappingURL=Example6-IJeAY36h.js.map
