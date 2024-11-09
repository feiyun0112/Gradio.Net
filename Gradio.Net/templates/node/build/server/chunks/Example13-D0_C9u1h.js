import { c as create_ssr_component } from './ssr-RaXq3SJh.js';

const css = {
  code: ".gallery.svelte-zvfedn{padding:var(--size-2)}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\n<\/script>\\n\\n<div\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n\\tclass=\\"prose\\"\\n>\\n\\t{@html value}\\n</div>\\n\\n<style>\\n\\t.gallery {\\n\\t\\tpadding: var(--size-2);\\n\\t}</style>\\n"],"names":[],"mappings":"AAeC,sBAAS,CACR,OAAO,CAAE,IAAI,QAAQ,CACtB"}'
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
    "prose svelte-zvfedn",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}"><!-- HTML_TAG_START -->${value}<!-- HTML_TAG_END --> </div>`;
});

export { Example as default };
//# sourceMappingURL=Example13-D0_C9u1h.js.map
