import { c as create_ssr_component, e as escape, f as each } from './ssr-RaXq3SJh.js';

const css = {
  code: "table.svelte-1cib1xd.svelte-1cib1xd{position:relative}td.svelte-1cib1xd.svelte-1cib1xd{border:1px solid var(--table-border-color);padding:var(--size-2);font-size:var(--text-sm);font-family:var(--font-mono)}.selected.svelte-1cib1xd td.svelte-1cib1xd{border-color:var(--border-color-accent)}.table.svelte-1cib1xd.svelte-1cib1xd{display:inline-block;margin:0 auto}.gallery.svelte-1cib1xd td.svelte-1cib1xd:first-child{border-left:none}.gallery.svelte-1cib1xd tr:first-child td.svelte-1cib1xd{border-top:none}.gallery.svelte-1cib1xd td.svelte-1cib1xd:last-child{border-right:none}.gallery.svelte-1cib1xd tr:last-child td.svelte-1cib1xd{border-bottom:none}.overlay.svelte-1cib1xd.svelte-1cib1xd{--gradient-to:transparent;position:absolute;bottom:0;background:linear-gradient(to bottom, transparent, var(--gradient-to));width:var(--size-full);height:50%}.odd.svelte-1cib1xd.svelte-1cib1xd{--gradient-to:var(--table-even-background-fill)}.even.svelte-1cib1xd.svelte-1cib1xd{--gradient-to:var(--table-odd-background-fill)}.button.svelte-1cib1xd.svelte-1cib1xd{--gradient-to:var(--background-fill-primary)}",
  map: `{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let type;\\nexport let selected = false;\\nexport let index;\\nlet hovered = false;\\nlet loaded = Array.isArray(value);\\n<\/script>\\n\\n{#if loaded}\\n\\t<!-- TODO: fix-->\\n\\t<!-- svelte-ignore a11y-no-static-element-interactions-->\\n\\t<div\\n\\t\\tclass:table={type === \\"table\\"}\\n\\t\\tclass:gallery={type === \\"gallery\\"}\\n\\t\\tclass:selected\\n\\t\\ton:mouseenter={() => (hovered = true)}\\n\\t\\ton:mouseleave={() => (hovered = false)}\\n\\t>\\n\\t\\t{#if typeof value === \\"string\\"}\\n\\t\\t\\t{value}\\n\\t\\t{:else}\\n\\t\\t\\t<table class=\\"\\">\\n\\t\\t\\t\\t{#each value.slice(0, 3) as row, i}\\n\\t\\t\\t\\t\\t<tr>\\n\\t\\t\\t\\t\\t\\t{#each row.slice(0, 3) as cell, j}\\n\\t\\t\\t\\t\\t\\t\\t<td>{cell}</td>\\n\\t\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t\\t\\t{#if row.length > 3}\\n\\t\\t\\t\\t\\t\\t\\t<td>…</td>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</tr>\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t{#if value.length > 3}\\n\\t\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\t\\tclass=\\"overlay\\"\\n\\t\\t\\t\\t\\t\\tclass:odd={index % 2 != 0}\\n\\t\\t\\t\\t\\t\\tclass:even={index % 2 == 0}\\n\\t\\t\\t\\t\\t\\tclass:button={type === \\"gallery\\"}\\n\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</table>\\n\\t\\t{/if}\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\ttable {\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\ttd {\\n\\t\\tborder: 1px solid var(--table-border-color);\\n\\t\\tpadding: var(--size-2);\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\tfont-family: var(--font-mono);\\n\\t}\\n\\n\\t.selected td {\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t}\\n\\n\\t.table {\\n\\t\\tdisplay: inline-block;\\n\\t\\tmargin: 0 auto;\\n\\t}\\n\\n\\t.gallery td:first-child {\\n\\t\\tborder-left: none;\\n\\t}\\n\\n\\t.gallery tr:first-child td {\\n\\t\\tborder-top: none;\\n\\t}\\n\\n\\t.gallery td:last-child {\\n\\t\\tborder-right: none;\\n\\t}\\n\\n\\t.gallery tr:last-child td {\\n\\t\\tborder-bottom: none;\\n\\t}\\n\\n\\t.overlay {\\n\\t\\t--gradient-to: transparent;\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 0;\\n\\t\\tbackground: linear-gradient(to bottom, transparent, var(--gradient-to));\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: 50%;\\n\\t}\\n\\n\\t/* i dont know what i've done here but it is what it is */\\n\\t.odd {\\n\\t\\t--gradient-to: var(--table-even-background-fill);\\n\\t}\\n\\n\\t.even {\\n\\t\\t--gradient-to: var(--table-odd-background-fill);\\n\\t}\\n\\n\\t.button {\\n\\t\\t--gradient-to: var(--background-fill-primary);\\n\\t}</style>\\n"],"names":[],"mappings":"AA8CC,mCAAM,CACL,QAAQ,CAAE,QACX,CAEA,gCAAG,CACF,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CAC3C,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,WAAW,CAAE,IAAI,WAAW,CAC7B,CAEA,wBAAS,CAAC,iBAAG,CACZ,YAAY,CAAE,IAAI,qBAAqB,CACxC,CAEA,oCAAO,CACN,OAAO,CAAE,YAAY,CACrB,MAAM,CAAE,CAAC,CAAC,IACX,CAEA,uBAAQ,CAAC,iBAAE,YAAa,CACvB,WAAW,CAAE,IACd,CAEA,uBAAQ,CAAC,EAAE,YAAY,CAAC,iBAAG,CAC1B,UAAU,CAAE,IACb,CAEA,uBAAQ,CAAC,iBAAE,WAAY,CACtB,YAAY,CAAE,IACf,CAEA,uBAAQ,CAAC,EAAE,WAAW,CAAC,iBAAG,CACzB,aAAa,CAAE,IAChB,CAEA,sCAAS,CACR,aAAa,CAAE,WAAW,CAC1B,QAAQ,CAAE,QAAQ,CAClB,MAAM,CAAE,CAAC,CACT,UAAU,CAAE,gBAAgB,EAAE,CAAC,MAAM,CAAC,CAAC,WAAW,CAAC,CAAC,IAAI,aAAa,CAAC,CAAC,CACvE,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,GACT,CAGA,kCAAK,CACJ,aAAa,CAAE,iCAChB,CAEA,mCAAM,CACL,aAAa,CAAE,gCAChB,CAEA,qCAAQ,CACP,aAAa,CAAE,8BAChB"}`
};
const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let { index } = $$props;
  let loaded = Array.isArray(value);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  if ($$props.index === void 0 && $$bindings.index && index !== void 0)
    $$bindings.index(index);
  $$result.css.add(css);
  return `${loaded ? `  <div class="${[
    "svelte-1cib1xd",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
  ].join(" ").trim()}">${typeof value === "string" ? `${escape(value)}` : `<table class=" svelte-1cib1xd">${each(value.slice(0, 3), (row, i) => {
    return `<tr>${each(row.slice(0, 3), (cell, j) => {
      return `<td class="svelte-1cib1xd">${escape(cell)}</td>`;
    })} ${row.length > 3 ? `<td class="svelte-1cib1xd" data-svelte-h="svelte-1o35md4">…</td>` : ``} </tr>`;
  })} ${value.length > 3 ? `<div class="${[
    "overlay svelte-1cib1xd",
    (index % 2 != 0 ? "odd" : "") + " " + (index % 2 == 0 ? "even" : "") + " " + (type === "gallery" ? "button" : "")
  ].join(" ").trim()}"></div>` : ``}</table>`}</div>` : ``}`;
});

export { Example as default };
//# sourceMappingURL=Example8-CJzifLVt.js.map
