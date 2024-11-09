import { c as create_ssr_component, v as validate_component, e as escape, b as add_attribute } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, N as BlockTitle } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: 'label.svelte-7ha85a.svelte-7ha85a:not(.container),label.svelte-7ha85a:not(.container)>input.svelte-7ha85a{height:100%;border:none}.container.svelte-7ha85a>input.svelte-7ha85a{border:var(--input-border-width) solid var(--input-border-color);border-radius:var(--input-radius)}input[type="number"].svelte-7ha85a.svelte-7ha85a{display:block;position:relative;outline:none !important;box-shadow:var(--input-shadow);background:var(--input-background-fill);padding:var(--input-padding);width:100%;color:var(--body-text-color);font-size:var(--input-text-size);line-height:var(--line-sm)}input.svelte-7ha85a.svelte-7ha85a:disabled{-webkit-text-fill-color:var(--body-text-color);-webkit-opacity:1;opacity:1}input.svelte-7ha85a.svelte-7ha85a:focus{box-shadow:var(--input-shadow-focus);border-color:var(--input-border-color-focus);background:var(--input-background-fill-focus)}input.svelte-7ha85a.svelte-7ha85a::placeholder{color:var(--input-placeholder-color)}input.svelte-7ha85a.svelte-7ha85a:out-of-range{border:var(--input-border-width) solid var(--error-border-color)}',
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { Block, BlockTitle } from \\"@gradio/atoms\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport { afterUpdate, tick } from \\"svelte\\";\\nexport let gradio;\\nexport let label = gradio.i18n(\\"number.number\\");\\nexport let info = void 0;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let container = true;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let value = 0;\\nexport let show_label;\\nexport let minimum = void 0;\\nexport let maximum = void 0;\\nexport let loading_status;\\nexport let value_is_output = false;\\nexport let step = null;\\nexport let interactive;\\nexport let root;\\nfunction handle_change() {\\n    if (!isNaN(value) && value !== null) {\\n        gradio.dispatch(\\"change\\");\\n        if (!value_is_output) {\\n            gradio.dispatch(\\"input\\");\\n        }\\n    }\\n}\\nafterUpdate(() => {\\n    value_is_output = false;\\n});\\nasync function handle_keypress(e) {\\n    await tick();\\n    if (e.key === \\"Enter\\") {\\n        e.preventDefault();\\n        gradio.dispatch(\\"submit\\");\\n    }\\n}\\n$: value, handle_change();\\n$: disabled = !interactive;\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\tpadding={container}\\n\\tallow_overflow={false}\\n\\t{scale}\\n\\t{min_width}\\n>\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t/>\\n\\t<label class=\\"block\\" class:container>\\n\\t\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\t\\t<input\\n\\t\\t\\taria-label={label}\\n\\t\\t\\ttype=\\"number\\"\\n\\t\\t\\tbind:value\\n\\t\\t\\tmin={minimum}\\n\\t\\t\\tmax={maximum}\\n\\t\\t\\t{step}\\n\\t\\t\\ton:keypress={handle_keypress}\\n\\t\\t\\ton:blur={() => gradio.dispatch(\\"blur\\")}\\n\\t\\t\\ton:focus={() => gradio.dispatch(\\"focus\\")}\\n\\t\\t\\t{disabled}\\n\\t\\t/>\\n\\t</label>\\n</Block>\\n\\n<style>\\n\\tlabel:not(.container),\\n\\tlabel:not(.container) > input {\\n\\t\\theight: 100%;\\n\\t\\tborder: none;\\n\\t}\\n\\t.container > input {\\n\\t\\tborder: var(--input-border-width) solid var(--input-border-color);\\n\\t\\tborder-radius: var(--input-radius);\\n\\t}\\n\\tinput[type=\\"number\\"] {\\n\\t\\tdisplay: block;\\n\\t\\tposition: relative;\\n\\t\\toutline: none !important;\\n\\t\\tbox-shadow: var(--input-shadow);\\n\\t\\tbackground: var(--input-background-fill);\\n\\t\\tpadding: var(--input-padding);\\n\\t\\twidth: 100%;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-size: var(--input-text-size);\\n\\t\\tline-height: var(--line-sm);\\n\\t}\\n\\tinput:disabled {\\n\\t\\t-webkit-text-fill-color: var(--body-text-color);\\n\\t\\t-webkit-opacity: 1;\\n\\t\\topacity: 1;\\n\\t}\\n\\n\\tinput:focus {\\n\\t\\tbox-shadow: var(--input-shadow-focus);\\n\\t\\tborder-color: var(--input-border-color-focus);\\n\\t\\tbackground: var(--input-background-fill-focus);\\n\\t}\\n\\n\\tinput::placeholder {\\n\\t\\tcolor: var(--input-placeholder-color);\\n\\t}\\n\\n\\tinput:out-of-range {\\n\\t\\tborder: var(--input-border-width) solid var(--error-border-color);\\n\\t}</style>\\n"],"names":[],"mappings":"AA4EC,iCAAK,KAAK,UAAU,CAAC,CACrB,mBAAK,KAAK,UAAU,CAAC,CAAG,mBAAM,CAC7B,MAAM,CAAE,IAAI,CACZ,MAAM,CAAE,IACT,CACA,wBAAU,CAAG,mBAAM,CAClB,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CACjE,aAAa,CAAE,IAAI,cAAc,CAClC,CACA,KAAK,CAAC,IAAI,CAAC,QAAQ,6BAAE,CACpB,OAAO,CAAE,KAAK,CACd,QAAQ,CAAE,QAAQ,CAClB,OAAO,CAAE,IAAI,CAAC,UAAU,CACxB,UAAU,CAAE,IAAI,cAAc,CAAC,CAC/B,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,OAAO,CAAE,IAAI,eAAe,CAAC,CAC7B,KAAK,CAAE,IAAI,CACX,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,SAAS,CAAE,IAAI,iBAAiB,CAAC,CACjC,WAAW,CAAE,IAAI,SAAS,CAC3B,CACA,iCAAK,SAAU,CACd,uBAAuB,CAAE,IAAI,iBAAiB,CAAC,CAC/C,eAAe,CAAE,CAAC,CAClB,OAAO,CAAE,CACV,CAEA,iCAAK,MAAO,CACX,UAAU,CAAE,IAAI,oBAAoB,CAAC,CACrC,YAAY,CAAE,IAAI,0BAA0B,CAAC,CAC7C,UAAU,CAAE,IAAI,6BAA6B,CAC9C,CAEA,iCAAK,aAAc,CAClB,KAAK,CAAE,IAAI,yBAAyB,CACrC,CAEA,iCAAK,aAAc,CAClB,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,oBAAoB,CACjE"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let disabled;
  let { gradio } = $$props;
  let { label = gradio.i18n("number.number") } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { value = 0 } = $$props;
  let { show_label } = $$props;
  let { minimum = void 0 } = $$props;
  let { maximum = void 0 } = $$props;
  let { loading_status } = $$props;
  let { value_is_output = false } = $$props;
  let { step = null } = $$props;
  let { interactive } = $$props;
  let { root } = $$props;
  function handle_change() {
    if (!isNaN(value) && value !== null) {
      gradio.dispatch("change");
      if (!value_is_output) {
        gradio.dispatch("input");
      }
    }
  }
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
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.minimum === void 0 && $$bindings.minimum && minimum !== void 0)
    $$bindings.minimum(minimum);
  if ($$props.maximum === void 0 && $$bindings.maximum && maximum !== void 0)
    $$bindings.maximum(maximum);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.step === void 0 && $$bindings.step && step !== void 0)
    $$bindings.step(step);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  $$result.css.add(css);
  {
    handle_change();
  }
  disabled = !interactive;
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      padding: container,
      allow_overflow: false,
      scale,
      min_width
    },
    {},
    {
      default: () => {
        return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} <label class="${["block svelte-7ha85a", container ? "container" : ""].join(" ").trim()}">${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
          default: () => {
            return `${escape(label)}`;
          }
        })} <input${add_attribute("aria-label", label, 0)} type="number"${add_attribute("min", minimum, 0)}${add_attribute("max", maximum, 0)}${add_attribute("step", step, 0)} ${disabled ? "disabled" : ""} class="svelte-7ha85a"${add_attribute("value", value, 0)}></label>`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index24-DCXLqtZ6.js.map
