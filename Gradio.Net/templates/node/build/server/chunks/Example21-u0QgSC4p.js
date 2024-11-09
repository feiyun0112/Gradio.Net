import { c as create_ssr_component, e as escape } from './ssr-RaXq3SJh.js';

const css = {
  code: ".gallery.svelte-1ayixqk{padding:var(--size-1) var(--size-2)}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\nexport let choices;\\nlet name_string;\\nif (value === null) {\\n    name_string = \\"\\";\\n}\\nelse {\\n    let name = choices.find((pair) => pair[1] === value);\\n    name_string = name ? name[0] : \\"\\";\\n}\\n<\/script>\\n\\n<div\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n>\\n\\t{name_string}\\n</div>\\n\\n<style>\\n\\t.gallery {\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t}</style>\\n"],"names":[],"mappings":"AAuBC,uBAAS,CACR,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CACpC"}'
};
const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let { choices } = $$props;
  let name_string;
  if (value === null) {
    name_string = "";
  } else {
    let name = choices.find((pair) => pair[1] === value);
    name_string = name ? name[0] : "";
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  $$result.css.add(css);
  return `<div class="${[
    "svelte-1ayixqk",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}">${escape(name_string)} </div>`;
});

export { Example as default };
//# sourceMappingURL=Example21-u0QgSC4p.js.map
