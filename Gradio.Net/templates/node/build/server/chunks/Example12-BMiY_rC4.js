import { c as create_ssr_component, f as each, e as escape } from './ssr-RaXq3SJh.js';

const css = {
  code: "ul.svelte-4tf8f{white-space:nowrap;max-height:100px;list-style:none;padding:0;margin:0}.extra.svelte-4tf8f{text-align:center}.gallery.svelte-4tf8f{align-items:center;cursor:pointer;padding:var(--size-1) var(--size-2);text-align:left}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\n<\/script>\\n\\n<ul\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n>\\n\\t{#if value}\\n\\t\\t{#each Array.isArray(value) ? value.slice(0, 3) : [value] as path}\\n\\t\\t\\t<li><code>./{path}</code></li>\\n\\t\\t{/each}\\n\\t\\t{#if Array.isArray(value) && value.length > 3}\\n\\t\\t\\t<li class=\\"extra\\">...</li>\\n\\t\\t{/if}\\n\\t{/if}\\n</ul>\\n\\n<style>\\n\\tul {\\n\\t\\twhite-space: nowrap;\\n\\t\\tmax-height: 100px;\\n\\t\\tlist-style: none;\\n\\t\\tpadding: 0;\\n\\t\\tmargin: 0;\\n\\t}\\n\\n\\t.extra {\\n\\t\\ttext-align: center;\\n\\t}\\n\\n\\t.gallery {\\n\\t\\talign-items: center;\\n\\t\\tcursor: pointer;\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\ttext-align: left;\\n\\t}</style>\\n"],"names":[],"mappings":"AAqBC,eAAG,CACF,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,KAAK,CACjB,UAAU,CAAE,IAAI,CAChB,OAAO,CAAE,CAAC,CACV,MAAM,CAAE,CACT,CAEA,mBAAO,CACN,UAAU,CAAE,MACb,CAEA,qBAAS,CACR,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,OAAO,CACf,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,UAAU,CAAE,IACb"}'
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
  return `<ul class="${[
    "svelte-4tf8f",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}">${value ? `${each(Array.isArray(value) ? value.slice(0, 3) : [value], (path) => {
    return `<li><code>./${escape(path)}</code></li>`;
  })} ${Array.isArray(value) && value.length > 3 ? `<li class="extra svelte-4tf8f" data-svelte-h="svelte-17d9ayl">...</li>` : ``}` : ``} </ul>`;
});

export { Example as default };
//# sourceMappingURL=Example12-BMiY_rC4.js.map
