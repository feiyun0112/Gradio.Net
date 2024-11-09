import { c as create_ssr_component, v as validate_component, e as escape, f as each, a as createEventDispatcher, b as add_attribute } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, N as BlockTitle } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example21-u0QgSC4p.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$1 = {
  code: 'label.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z{display:flex;align-items:center;transition:var(--button-transition);cursor:pointer;box-shadow:var(--checkbox-label-shadow);border:var(--checkbox-label-border-width) solid\n			var(--checkbox-label-border-color);border-radius:var(--button-small-radius);background:var(--checkbox-label-background-fill);padding:var(--checkbox-label-padding);color:var(--checkbox-label-text-color);font-weight:var(--checkbox-label-text-weight);font-size:var(--checkbox-label-text-size);line-height:var(--line-md)}label.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:hover{background:var(--checkbox-label-background-fill-hover)}label.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:focus{background:var(--checkbox-label-background-fill-focus)}label.selected.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z{background:var(--checkbox-label-background-fill-selected);color:var(--checkbox-label-text-color-selected);border-color:var(--checkbox-label-border-color-selected)}label.svelte-167tx3z>.svelte-167tx3z+.svelte-167tx3z{margin-left:var(--size-2)}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z{--ring-color:transparent;position:relative;box-shadow:var(--checkbox-shadow);border:var(--checkbox-border-width) solid var(--checkbox-border-color);border-radius:var(--radius-full);background-color:var(--checkbox-background-color);line-height:var(--line-sm)}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:checked,input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:checked:hover{border-color:var(--checkbox-border-color-selected);background-image:var(--radio-circle);background-color:var(--checkbox-background-color-selected)}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:checked::after{content:"";position:absolute;top:50%;left:50%;transform:translate(-50%, -50%);border-radius:50%;background-color:white}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:hover{border-color:var(--checkbox-border-color-hover);background-color:var(--checkbox-background-color-hover)}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:focus{border-color:var(--checkbox-border-color-focus);background-color:var(--checkbox-background-color-focus)}input.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z:checked:focus{border-color:var(--checkbox-border-color-focus);background-image:var(--radio-circle);background-color:var(--checkbox-background-color-selected)}input[disabled].svelte-167tx3z.svelte-167tx3z.svelte-167tx3z,.disabled.svelte-167tx3z.svelte-167tx3z.svelte-167tx3z{cursor:not-allowed}',
  map: '{"version":3,"file":"Radio.svelte","sources":["Radio.svelte"],"sourcesContent":["<script context=\\"module\\">\\n\\tlet id = 0;\\n<\/script>\\n\\n<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nexport let display_value;\\nexport let internal_value;\\nexport let disabled = false;\\nexport let selected = null;\\nconst dispatch = createEventDispatcher();\\nlet is_selected = false;\\nasync function handle_input(selected2, internal_value2) {\\n    is_selected = selected2 === internal_value2;\\n    if (is_selected) {\\n        dispatch(\\"input\\", internal_value2);\\n    }\\n}\\n$: handle_input(selected, internal_value);\\n<\/script>\\n\\n<label\\n\\tclass:disabled\\n\\tclass:selected={is_selected}\\n\\tdata-testid=\\"{display_value}-radio-label\\"\\n>\\n\\t<input\\n\\t\\t{disabled}\\n\\t\\ttype=\\"radio\\"\\n\\t\\tname=\\"radio-{++id}\\"\\n\\t\\tvalue={internal_value}\\n\\t\\taria-checked={is_selected}\\n\\t\\tbind:group={selected}\\n\\t/>\\n\\t<span class=\\"ml-2\\">{display_value}</span>\\n</label>\\n\\n<style>\\n\\tlabel {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tcursor: pointer;\\n\\t\\tbox-shadow: var(--checkbox-label-shadow);\\n\\t\\tborder: var(--checkbox-label-border-width) solid\\n\\t\\t\\tvar(--checkbox-label-border-color);\\n\\t\\tborder-radius: var(--button-small-radius);\\n\\t\\tbackground: var(--checkbox-label-background-fill);\\n\\t\\tpadding: var(--checkbox-label-padding);\\n\\t\\tcolor: var(--checkbox-label-text-color);\\n\\t\\tfont-weight: var(--checkbox-label-text-weight);\\n\\t\\tfont-size: var(--checkbox-label-text-size);\\n\\t\\tline-height: var(--line-md);\\n\\t}\\n\\n\\tlabel:hover {\\n\\t\\tbackground: var(--checkbox-label-background-fill-hover);\\n\\t}\\n\\tlabel:focus {\\n\\t\\tbackground: var(--checkbox-label-background-fill-focus);\\n\\t}\\n\\n\\tlabel.selected {\\n\\t\\tbackground: var(--checkbox-label-background-fill-selected);\\n\\t\\tcolor: var(--checkbox-label-text-color-selected);\\n\\t\\tborder-color: var(--checkbox-label-border-color-selected);\\n\\t}\\n\\n\\tlabel > * + * {\\n\\t\\tmargin-left: var(--size-2);\\n\\t}\\n\\n\\tinput {\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow: var(--checkbox-shadow);\\n\\t\\tborder: var(--checkbox-border-width) solid var(--checkbox-border-color);\\n\\t\\tborder-radius: var(--radius-full);\\n\\t\\tbackground-color: var(--checkbox-background-color);\\n\\t\\tline-height: var(--line-sm);\\n\\t}\\n\\n\\tinput:checked,\\n\\tinput:checked:hover {\\n\\t\\tborder-color: var(--checkbox-border-color-selected);\\n\\t\\tbackground-image: var(--radio-circle);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t}\\n\\n\\tinput:checked::after {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\ttop: 50%;\\n\\t\\tleft: 50%;\\n\\t\\ttransform: translate(-50%, -50%);\\n\\t\\tborder-radius: 50%;\\n\\t\\tbackground-color: white;\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tborder-color: var(--checkbox-border-color-hover);\\n\\t\\tbackground-color: var(--checkbox-background-color-hover);\\n\\t}\\n\\n\\tinput:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t\\tbackground-color: var(--checkbox-background-color-focus);\\n\\t}\\n\\n\\tinput:checked:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t\\tbackground-image: var(--radio-circle);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t}\\n\\n\\tinput[disabled],\\n\\t.disabled {\\n\\t\\tcursor: not-allowed;\\n\\t}</style>\\n"],"names":[],"mappings":"AAqCC,kDAAM,CACL,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,MAAM,CAAE,OAAO,CACf,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,MAAM,CAAE,IAAI,6BAA6B,CAAC,CAAC,KAAK;AAClD,GAAG,IAAI,6BAA6B,CAAC,CACnC,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,UAAU,CAAE,IAAI,gCAAgC,CAAC,CACjD,OAAO,CAAE,IAAI,wBAAwB,CAAC,CACtC,KAAK,CAAE,IAAI,2BAA2B,CAAC,CACvC,WAAW,CAAE,IAAI,4BAA4B,CAAC,CAC9C,SAAS,CAAE,IAAI,0BAA0B,CAAC,CAC1C,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,kDAAK,MAAO,CACX,UAAU,CAAE,IAAI,sCAAsC,CACvD,CACA,kDAAK,MAAO,CACX,UAAU,CAAE,IAAI,sCAAsC,CACvD,CAEA,KAAK,sDAAU,CACd,UAAU,CAAE,IAAI,yCAAyC,CAAC,CAC1D,KAAK,CAAE,IAAI,oCAAoC,CAAC,CAChD,YAAY,CAAE,IAAI,sCAAsC,CACzD,CAEA,oBAAK,CAAG,eAAC,CAAG,eAAE,CACb,WAAW,CAAE,IAAI,QAAQ,CAC1B,CAEA,kDAAM,CACL,YAAY,CAAE,WAAW,CACzB,QAAQ,CAAE,QAAQ,CAClB,UAAU,CAAE,IAAI,iBAAiB,CAAC,CAClC,MAAM,CAAE,IAAI,uBAAuB,CAAC,CAAC,KAAK,CAAC,IAAI,uBAAuB,CAAC,CACvE,aAAa,CAAE,IAAI,aAAa,CAAC,CACjC,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,kDAAK,QAAQ,CACb,kDAAK,QAAQ,MAAO,CACnB,YAAY,CAAE,IAAI,gCAAgC,CAAC,CACnD,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,gBAAgB,CAAE,IAAI,oCAAoC,CAC3D,CAEA,kDAAK,QAAQ,OAAQ,CACpB,OAAO,CAAE,EAAE,CACX,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,GAAG,CACR,IAAI,CAAE,GAAG,CACT,SAAS,CAAE,UAAU,IAAI,CAAC,CAAC,IAAI,CAAC,CAChC,aAAa,CAAE,GAAG,CAClB,gBAAgB,CAAE,KACnB,CAEA,kDAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,kDAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,kDAAK,QAAQ,MAAO,CACnB,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,gBAAgB,CAAE,IAAI,oCAAoC,CAC3D,CAEA,KAAK,CAAC,QAAQ,8CAAC,CACf,sDAAU,CACT,MAAM,CAAE,WACT"}'
};
let id = 0;
const Radio = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { display_value } = $$props;
  let { internal_value } = $$props;
  let { disabled = false } = $$props;
  let { selected = null } = $$props;
  const dispatch = createEventDispatcher();
  let is_selected = false;
  async function handle_input(selected2, internal_value2) {
    is_selected = selected2 === internal_value2;
    if (is_selected) {
      dispatch("input", internal_value2);
    }
  }
  if ($$props.display_value === void 0 && $$bindings.display_value && display_value !== void 0)
    $$bindings.display_value(display_value);
  if ($$props.internal_value === void 0 && $$bindings.internal_value && internal_value !== void 0)
    $$bindings.internal_value(internal_value);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  $$result.css.add(css$1);
  {
    handle_input(selected, internal_value);
  }
  return `<label data-testid="${escape(display_value, true) + "-radio-label"}" class="${[
    "svelte-167tx3z",
    (disabled ? "disabled" : "") + " " + (is_selected ? "selected" : "")
  ].join(" ").trim()}"><input ${disabled ? "disabled" : ""} type="radio" name="${"radio-" + escape(++id, true)}"${add_attribute("value", internal_value, 0)}${add_attribute("aria-checked", is_selected, 0)} class="svelte-167tx3z"${internal_value === selected ? add_attribute("checked", true, 1) : ""}> <span class="ml-2 svelte-167tx3z">${escape(display_value)}</span> </label>`;
});
const BaseRadio = Radio;
const css = {
  code: ".wrap.svelte-1kzox3m{display:flex;flex-wrap:wrap;gap:var(--checkbox-label-gap)}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script context=\\"module\\" lang=\\"ts\\">export { default as BaseRadio } from \\"./shared/Radio.svelte\\";\\nexport { default as BaseExample } from \\"./Example.svelte\\";\\n<\/script>\\n\\n<script lang=\\"ts\\">import { afterUpdate } from \\"svelte\\";\\nimport { Block, BlockTitle } from \\"@gradio/atoms\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport BaseRadio from \\"./shared/Radio.svelte\\";\\nexport let gradio;\\nexport let label = gradio.i18n(\\"radio.radio\\");\\nexport let info = void 0;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = null;\\nexport let choices = [];\\nexport let show_label = true;\\nexport let container = false;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let loading_status;\\nexport let interactive = true;\\nexport let root;\\nfunction handle_change() {\\n    gradio.dispatch(\\"change\\");\\n}\\nlet old_value = value;\\n$: {\\n    if (value !== old_value) {\\n        old_value = value;\\n        handle_change();\\n    }\\n}\\n$: disabled = !interactive;\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\ttype=\\"fieldset\\"\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{container}\\n\\t{scale}\\n\\t{min_width}\\n>\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t/>\\n\\n\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\n\\t<div class=\\"wrap\\">\\n\\t\\t{#each choices as [display_value, internal_value], i (i)}\\n\\t\\t\\t<BaseRadio\\n\\t\\t\\t\\t{display_value}\\n\\t\\t\\t\\t{internal_value}\\n\\t\\t\\t\\tbind:selected={value}\\n\\t\\t\\t\\t{disabled}\\n\\t\\t\\t\\ton:input={() => {\\n\\t\\t\\t\\t\\tgradio.dispatch(\\"select\\", { value: internal_value, index: i });\\n\\t\\t\\t\\t\\tgradio.dispatch(\\"input\\");\\n\\t\\t\\t\\t}}\\n\\t\\t\\t/>\\n\\t\\t{/each}\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\t.wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tgap: var(--checkbox-label-gap);\\n\\t}</style>\\n"],"names":[],"mappings":"AAuEC,oBAAM,CACL,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,IAAI,CACf,GAAG,CAAE,IAAI,oBAAoB,CAC9B"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let disabled;
  let { gradio } = $$props;
  let { label = gradio.i18n("radio.radio") } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { choices = [] } = $$props;
  let { show_label = true } = $$props;
  let { container = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { interactive = true } = $$props;
  let { root } = $$props;
  function handle_change() {
    gradio.dispatch("change");
  }
  let old_value = value;
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
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
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      {
        if (value !== old_value) {
          old_value = value;
          handle_change();
        }
      }
    }
    disabled = !interactive;
    $$rendered = `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        type: "fieldset",
        elem_id,
        elem_classes,
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
          })} <div class="wrap svelte-1kzox3m">${each(choices, ([display_value, internal_value], i) => {
            return `${validate_component(BaseRadio, "BaseRadio").$$render(
              $$result,
              {
                display_value,
                internal_value,
                disabled,
                selected: value
              },
              {
                selected: ($$value) => {
                  value = $$value;
                  $$settled = false;
                }
              },
              {}
            )}`;
          })}</div>`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { BaseRadio, Index as default };
//# sourceMappingURL=Index46-Dn4ibBtQ.js.map
