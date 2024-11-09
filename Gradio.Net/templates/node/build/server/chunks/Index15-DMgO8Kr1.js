import { c as create_ssr_component, v as validate_component, e as escape, f as each, b as add_attribute } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, N as BlockTitle } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".wrap.svelte-kujow2.svelte-kujow2.svelte-kujow2{display:flex;flex-wrap:wrap;gap:var(--checkbox-label-gap)}label.svelte-kujow2.svelte-kujow2.svelte-kujow2{display:flex;align-items:center;transition:var(--button-transition);cursor:pointer;box-shadow:var(--checkbox-label-shadow);border:var(--checkbox-label-border-width) solid\n			var(--checkbox-label-border-color);border-radius:var(--button-small-radius);background:var(--checkbox-label-background-fill);padding:var(--checkbox-label-padding);color:var(--checkbox-label-text-color);font-weight:var(--checkbox-label-text-weight);font-size:var(--checkbox-label-text-size);line-height:var(--line-md)}label.svelte-kujow2.svelte-kujow2.svelte-kujow2:hover{background:var(--checkbox-label-background-fill-hover)}label.svelte-kujow2.svelte-kujow2.svelte-kujow2:focus{background:var(--checkbox-label-background-fill-focus)}label.selected.svelte-kujow2.svelte-kujow2.svelte-kujow2{background:var(--checkbox-label-background-fill-selected);color:var(--checkbox-label-text-color-selected);border-color:var(--checkbox-label-border-color-selected)}label.svelte-kujow2>.svelte-kujow2+.svelte-kujow2{margin-left:var(--size-2)}input.svelte-kujow2.svelte-kujow2.svelte-kujow2{--ring-color:transparent;position:relative;box-shadow:var(--checkbox-shadow);border:var(--checkbox-border-width) solid var(--checkbox-border-color);border-radius:var(--checkbox-border-radius);background-color:var(--checkbox-background-color);line-height:var(--line-sm)}input.svelte-kujow2.svelte-kujow2.svelte-kujow2:checked,input.svelte-kujow2.svelte-kujow2.svelte-kujow2:checked:hover,input.svelte-kujow2.svelte-kujow2.svelte-kujow2:checked:focus{border-color:var(--checkbox-border-color-selected);background-image:var(--checkbox-check);background-color:var(--checkbox-background-color-selected)}input.svelte-kujow2.svelte-kujow2.svelte-kujow2:checked:focus{border-color:var(--checkbox-border-color-focus);background-image:var(--checkbox-check);background-color:var(--checkbox-background-color-selected)}input.svelte-kujow2.svelte-kujow2.svelte-kujow2:hover{border-color:var(--checkbox-border-color-hover);background-color:var(--checkbox-background-color-hover)}input.svelte-kujow2.svelte-kujow2.svelte-kujow2:not(:checked):focus{border-color:var(--checkbox-border-color-focus)}input[disabled].svelte-kujow2.svelte-kujow2.svelte-kujow2,.disabled.svelte-kujow2.svelte-kujow2.svelte-kujow2{cursor:not-allowed}input.svelte-kujow2.svelte-kujow2.svelte-kujow2:hover{cursor:pointer}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<svelte:options immutable={true} />\\n\\n<script lang=\\"ts\\">import { Block, BlockTitle } from \\"@gradio/atoms\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nexport let gradio;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = [];\\nexport let choices;\\nexport let container = true;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let label = gradio.i18n(\\"checkbox.checkbox_group\\");\\nexport let info = void 0;\\nexport let show_label = true;\\nexport let root;\\nexport let loading_status;\\nexport let interactive = true;\\nexport let old_value = value.slice();\\nfunction toggle_choice(choice) {\\n    if (value.includes(choice)) {\\n        value = value.filter((v) => v !== choice);\\n    }\\n    else {\\n        value = [...value, choice];\\n    }\\n    gradio.dispatch(\\"input\\");\\n}\\n$: disabled = !interactive;\\n$: if (JSON.stringify(old_value) !== JSON.stringify(value)) {\\n    old_value = value;\\n    gradio.dispatch(\\"change\\");\\n}\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\ttype=\\"fieldset\\"\\n\\t{container}\\n\\t{scale}\\n\\t{min_width}\\n>\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t/>\\n\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\n\\t<div class=\\"wrap\\" data-testid=\\"checkbox-group\\">\\n\\t\\t{#each choices as [display_value, internal_value], i}\\n\\t\\t\\t<label class:disabled class:selected={value.includes(internal_value)}>\\n\\t\\t\\t\\t<input\\n\\t\\t\\t\\t\\t{disabled}\\n\\t\\t\\t\\t\\ton:change={() => toggle_choice(internal_value)}\\n\\t\\t\\t\\t\\ton:input={(evt) =>\\n\\t\\t\\t\\t\\t\\tgradio.dispatch(\\"select\\", {\\n\\t\\t\\t\\t\\t\\t\\tindex: i,\\n\\t\\t\\t\\t\\t\\t\\tvalue: internal_value,\\n\\t\\t\\t\\t\\t\\t\\tselected: evt.currentTarget.checked\\n\\t\\t\\t\\t\\t\\t})}\\n\\t\\t\\t\\t\\ton:keydown={(event) => {\\n\\t\\t\\t\\t\\t\\tif (event.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\ttoggle_choice(internal_value);\\n\\t\\t\\t\\t\\t\\t\\tgradio.dispatch(\\"select\\", {\\n\\t\\t\\t\\t\\t\\t\\t\\tindex: i,\\n\\t\\t\\t\\t\\t\\t\\t\\tvalue: internal_value,\\n\\t\\t\\t\\t\\t\\t\\t\\tselected: !value.includes(internal_value)\\n\\t\\t\\t\\t\\t\\t\\t});\\n\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\tchecked={value.includes(internal_value)}\\n\\t\\t\\t\\t\\ttype=\\"checkbox\\"\\n\\t\\t\\t\\t\\tname={internal_value?.toString()}\\n\\t\\t\\t\\t\\ttitle={internal_value?.toString()}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t\\t<span class=\\"ml-2\\">{display_value}</span>\\n\\t\\t\\t</label>\\n\\t\\t{/each}\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\t.wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tgap: var(--checkbox-label-gap);\\n\\t}\\n\\tlabel {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tcursor: pointer;\\n\\t\\tbox-shadow: var(--checkbox-label-shadow);\\n\\t\\tborder: var(--checkbox-label-border-width) solid\\n\\t\\t\\tvar(--checkbox-label-border-color);\\n\\t\\tborder-radius: var(--button-small-radius);\\n\\t\\tbackground: var(--checkbox-label-background-fill);\\n\\t\\tpadding: var(--checkbox-label-padding);\\n\\t\\tcolor: var(--checkbox-label-text-color);\\n\\t\\tfont-weight: var(--checkbox-label-text-weight);\\n\\t\\tfont-size: var(--checkbox-label-text-size);\\n\\t\\tline-height: var(--line-md);\\n\\t}\\n\\n\\tlabel:hover {\\n\\t\\tbackground: var(--checkbox-label-background-fill-hover);\\n\\t}\\n\\tlabel:focus {\\n\\t\\tbackground: var(--checkbox-label-background-fill-focus);\\n\\t}\\n\\tlabel.selected {\\n\\t\\tbackground: var(--checkbox-label-background-fill-selected);\\n\\t\\tcolor: var(--checkbox-label-text-color-selected);\\n\\t\\tborder-color: var(--checkbox-label-border-color-selected);\\n\\t}\\n\\n\\tlabel > * + * {\\n\\t\\tmargin-left: var(--size-2);\\n\\t}\\n\\n\\tinput {\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow: var(--checkbox-shadow);\\n\\t\\tborder: var(--checkbox-border-width) solid var(--checkbox-border-color);\\n\\t\\tborder-radius: var(--checkbox-border-radius);\\n\\t\\tbackground-color: var(--checkbox-background-color);\\n\\t\\tline-height: var(--line-sm);\\n\\t}\\n\\n\\tinput:checked,\\n\\tinput:checked:hover,\\n\\tinput:checked:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-selected);\\n\\t\\tbackground-image: var(--checkbox-check);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t}\\n\\n\\tinput:checked:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t\\tbackground-image: var(--checkbox-check);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tborder-color: var(--checkbox-border-color-hover);\\n\\t\\tbackground-color: var(--checkbox-background-color-hover);\\n\\t}\\n\\n\\tinput:not(:checked):focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t}\\n\\n\\tinput[disabled],\\n\\t.disabled {\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"AAuFC,+CAAM,CACL,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,IAAI,CACf,GAAG,CAAE,IAAI,oBAAoB,CAC9B,CACA,+CAAM,CACL,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,MAAM,CAAE,OAAO,CACf,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,MAAM,CAAE,IAAI,6BAA6B,CAAC,CAAC,KAAK;AAClD,GAAG,IAAI,6BAA6B,CAAC,CACnC,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,UAAU,CAAE,IAAI,gCAAgC,CAAC,CACjD,OAAO,CAAE,IAAI,wBAAwB,CAAC,CACtC,KAAK,CAAE,IAAI,2BAA2B,CAAC,CACvC,WAAW,CAAE,IAAI,4BAA4B,CAAC,CAC9C,SAAS,CAAE,IAAI,0BAA0B,CAAC,CAC1C,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,+CAAK,MAAO,CACX,UAAU,CAAE,IAAI,sCAAsC,CACvD,CACA,+CAAK,MAAO,CACX,UAAU,CAAE,IAAI,sCAAsC,CACvD,CACA,KAAK,mDAAU,CACd,UAAU,CAAE,IAAI,yCAAyC,CAAC,CAC1D,KAAK,CAAE,IAAI,oCAAoC,CAAC,CAChD,YAAY,CAAE,IAAI,sCAAsC,CACzD,CAEA,mBAAK,CAAG,cAAC,CAAG,cAAE,CACb,WAAW,CAAE,IAAI,QAAQ,CAC1B,CAEA,+CAAM,CACL,YAAY,CAAE,WAAW,CACzB,QAAQ,CAAE,QAAQ,CAClB,UAAU,CAAE,IAAI,iBAAiB,CAAC,CAClC,MAAM,CAAE,IAAI,uBAAuB,CAAC,CAAC,KAAK,CAAC,IAAI,uBAAuB,CAAC,CACvE,aAAa,CAAE,IAAI,wBAAwB,CAAC,CAC5C,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,+CAAK,QAAQ,CACb,+CAAK,QAAQ,MAAM,CACnB,+CAAK,QAAQ,MAAO,CACnB,YAAY,CAAE,IAAI,gCAAgC,CAAC,CACnD,gBAAgB,CAAE,IAAI,gBAAgB,CAAC,CACvC,gBAAgB,CAAE,IAAI,oCAAoC,CAC3D,CAEA,+CAAK,QAAQ,MAAO,CACnB,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,gBAAgB,CAAC,CACvC,gBAAgB,CAAE,IAAI,oCAAoC,CAC3D,CAEA,+CAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,+CAAK,KAAK,QAAQ,CAAC,MAAO,CACzB,YAAY,CAAE,IAAI,6BAA6B,CAChD,CAEA,KAAK,CAAC,QAAQ,2CAAC,CACf,mDAAU,CACT,MAAM,CAAE,WACT,CAEA,+CAAK,MAAO,CACX,MAAM,CAAE,OACT"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let disabled;
  let { gradio } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = [] } = $$props;
  let { choices } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { label = gradio.i18n("checkbox.checkbox_group") } = $$props;
  let { info = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { root } = $$props;
  let { loading_status } = $$props;
  let { interactive = true } = $$props;
  let { old_value = value.slice() } = $$props;
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.old_value === void 0 && $$bindings.old_value && old_value !== void 0)
    $$bindings.old_value(old_value);
  $$result.css.add(css);
  disabled = !interactive;
  {
    if (JSON.stringify(old_value) !== JSON.stringify(value)) {
      old_value = value;
      gradio.dispatch("change");
    }
  }
  return `  ${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      type: "fieldset",
      container,
      scale,
      min_width
    },
    {},
    {
      default: () => {
        return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
          default: () => {
            return `${escape(label)}`;
          }
        })} <div class="wrap svelte-kujow2" data-testid="checkbox-group">${each(choices, ([display_value, internal_value], i) => {
          return `<label class="${[
            "svelte-kujow2",
            (disabled ? "disabled" : "") + " " + (value.includes(internal_value) ? "selected" : "")
          ].join(" ").trim()}"><input ${disabled ? "disabled" : ""} ${value.includes(internal_value) ? "checked" : ""} type="checkbox"${add_attribute("name", internal_value?.toString(), 0)}${add_attribute("title", internal_value?.toString(), 0)} class="svelte-kujow2"> <span class="ml-2 svelte-kujow2">${escape(display_value)}</span> </label>`;
        })}</div>`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index15-DMgO8Kr1.js.map
