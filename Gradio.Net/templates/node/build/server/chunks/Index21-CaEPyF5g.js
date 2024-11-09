import { c as create_ssr_component, b as add_attribute, e as escape, d as add_styles } from './ssr-RaXq3SJh.js';

const css = {
  code: "div.svelte-1nguped{border:var(--block-border-width) solid var(--border-color-primary);background:var(--block-border-color);border-radius:var(--block-radius);display:flex;flex-direction:column;gap:var(--form-gap-width);overflow:hidden}div.svelte-1nguped>*:not(.absolute){border:none;border-radius:0}.hide.svelte-1nguped{display:none}",
  map: `{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\n<\/script>\\n\\n<div\\n\\tid={elem_id}\\n\\tclass=\\"gr-group {elem_classes.join(' ')}\\"\\n\\tclass:hide={!visible}\\n>\\n\\t<div\\n\\t\\tclass=\\"styler\\"\\n\\t\\tstyle:--block-radius=\\"0px\\"\\n\\t\\tstyle:--block-border-width=\\"0px\\"\\n\\t\\tstyle:--layout-gap=\\"1px\\"\\n\\t\\tstyle:--form-gap-width=\\"1px\\"\\n\\t\\tstyle:--button-border-width=\\"0px\\"\\n\\t\\tstyle:--button-large-radius=\\"0px\\"\\n\\t\\tstyle:--button-small-radius=\\"0px\\"\\n\\t>\\n\\t\\t<slot />\\n\\t</div>\\n</div>\\n\\n<style>\\n\\tdiv {\\n\\t\\tborder: var(--block-border-width) solid var(--border-color-primary);\\n\\t\\tbackground: var(--block-border-color);\\n\\t\\tborder-radius: var(--block-radius);\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tgap: var(--form-gap-width);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\tdiv > :global(*:not(.absolute)) {\\n\\t\\tborder: none;\\n\\t\\tborder-radius: 0;\\n\\t}\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AAyBC,kBAAI,CACH,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CACnE,UAAU,CAAE,IAAI,oBAAoB,CAAC,CACrC,aAAa,CAAE,IAAI,cAAc,CAAC,CAClC,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,GAAG,CAAE,IAAI,gBAAgB,CAAC,CAC1B,QAAQ,CAAE,MACX,CACA,kBAAG,CAAW,gBAAkB,CAC/B,MAAM,CAAE,IAAI,CACZ,aAAa,CAAE,CAChB,CACA,oBAAM,CACL,OAAO,CAAE,IACV"}`
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  $$result.css.add(css);
  return `<div${add_attribute("id", elem_id, 0)} class="${[
    "gr-group " + escape(elem_classes.join(" "), true) + " svelte-1nguped",
    !visible ? "hide" : ""
  ].join(" ").trim()}"><div class="styler svelte-1nguped"${add_styles({
    "--block-radius": `0px`,
    "--block-border-width": `0px`,
    "--layout-gap": `1px`,
    "--form-gap-width": `1px`,
    "--button-border-width": `0px`,
    "--button-large-radius": `0px`,
    "--button-small-radius": `0px`
  })}>${slots.default ? slots.default({}) : ``}</div> </div>`;
});

export { Index as default };
//# sourceMappingURL=Index21-CaEPyF5g.js.map
