import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, b as add_attribute, e as escape } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, K as Info$1 } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: "label.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7{display:flex;align-items:center;transition:var(--button-transition);cursor:pointer;color:var(--checkbox-label-text-color);font-weight:var(--checkbox-label-text-weight);font-size:var(--checkbox-label-text-size);line-height:var(--line-md)}label.svelte-5ncdh7>.svelte-5ncdh7+.svelte-5ncdh7{margin-left:var(--size-2)}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7{--ring-color:transparent;position:relative;box-shadow:var(--checkbox-shadow);border:1px solid var(--checkbox-border-color);border-radius:var(--checkbox-border-radius);background-color:var(--checkbox-background-color);line-height:var(--line-sm)}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:checked,input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:checked:hover,input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:checked:focus{background-image:var(--checkbox-check);background-color:var(--checkbox-background-color-selected);border-color:var(--checkbox-border-color-focus)}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:checked:focus{background-image:var(--checkbox-check);background-color:var(--checkbox-background-color-selected);border-color:var(--checkbox-border-color-focus)}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:hover{border-color:var(--checkbox-border-color-hover);background-color:var(--checkbox-background-color-hover)}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:focus{border-color:var(--checkbox-border-color-focus);background-color:var(--checkbox-background-color-focus)}input[disabled].svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7,.disabled.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7{cursor:not-allowed}input.svelte-5ncdh7.svelte-5ncdh7.svelte-5ncdh7:hover{cursor:pointer}",
  map: '{"version":3,"file":"Checkbox.svelte","sources":["Checkbox.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nexport let value = false;\\nexport let label = \\"Checkbox\\";\\nexport let interactive;\\nconst dispatch = createEventDispatcher();\\n$: value, dispatch(\\"change\\", value);\\n$: disabled = !interactive;\\nasync function handle_enter(event) {\\n    if (event.key === \\"Enter\\") {\\n        value = !value;\\n        dispatch(\\"select\\", {\\n            index: 0,\\n            value: event.currentTarget.checked,\\n            selected: event.currentTarget.checked\\n        });\\n    }\\n}\\nasync function handle_input(event) {\\n    value = event.currentTarget.checked;\\n    dispatch(\\"select\\", {\\n        index: 0,\\n        value: event.currentTarget.checked,\\n        selected: event.currentTarget.checked\\n    });\\n}\\n<\/script>\\n\\n<label class:disabled>\\n\\t<input\\n\\t\\tbind:checked={value}\\n\\t\\ton:keydown={handle_enter}\\n\\t\\ton:input={handle_input}\\n\\t\\t{disabled}\\n\\t\\ttype=\\"checkbox\\"\\n\\t\\tname=\\"test\\"\\n\\t\\tdata-testid=\\"checkbox\\"\\n\\t/>\\n\\t<span>{label}</span>\\n</label>\\n\\n<style>\\n\\tlabel {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tcursor: pointer;\\n\\t\\tcolor: var(--checkbox-label-text-color);\\n\\t\\tfont-weight: var(--checkbox-label-text-weight);\\n\\t\\tfont-size: var(--checkbox-label-text-size);\\n\\t\\tline-height: var(--line-md);\\n\\t}\\n\\n\\tlabel > * + * {\\n\\t\\tmargin-left: var(--size-2);\\n\\t}\\n\\n\\tinput {\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow: var(--checkbox-shadow);\\n\\t\\tborder: 1px solid var(--checkbox-border-color);\\n\\t\\tborder-radius: var(--checkbox-border-radius);\\n\\t\\tbackground-color: var(--checkbox-background-color);\\n\\t\\tline-height: var(--line-sm);\\n\\t}\\n\\n\\tinput:checked,\\n\\tinput:checked:hover,\\n\\tinput:checked:focus {\\n\\t\\tbackground-image: var(--checkbox-check);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t}\\n\\n\\tinput:checked:focus {\\n\\t\\tbackground-image: var(--checkbox-check);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tborder-color: var(--checkbox-border-color-hover);\\n\\t\\tbackground-color: var(--checkbox-background-color-hover);\\n\\t}\\n\\n\\tinput:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t\\tbackground-color: var(--checkbox-background-color-focus);\\n\\t}\\n\\n\\tinput[disabled],\\n\\t.disabled {\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"AAyCC,+CAAM,CACL,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,2BAA2B,CAAC,CACvC,WAAW,CAAE,IAAI,4BAA4B,CAAC,CAC9C,SAAS,CAAE,IAAI,0BAA0B,CAAC,CAC1C,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,mBAAK,CAAG,cAAC,CAAG,cAAE,CACb,WAAW,CAAE,IAAI,QAAQ,CAC1B,CAEA,+CAAM,CACL,YAAY,CAAE,WAAW,CACzB,QAAQ,CAAE,QAAQ,CAClB,UAAU,CAAE,IAAI,iBAAiB,CAAC,CAClC,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,wBAAwB,CAAC,CAC5C,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,+CAAK,QAAQ,CACb,+CAAK,QAAQ,MAAM,CACnB,+CAAK,QAAQ,MAAO,CACnB,gBAAgB,CAAE,IAAI,gBAAgB,CAAC,CACvC,gBAAgB,CAAE,IAAI,oCAAoC,CAAC,CAC3D,YAAY,CAAE,IAAI,6BAA6B,CAChD,CAEA,+CAAK,QAAQ,MAAO,CACnB,gBAAgB,CAAE,IAAI,gBAAgB,CAAC,CACvC,gBAAgB,CAAE,IAAI,oCAAoC,CAAC,CAC3D,YAAY,CAAE,IAAI,6BAA6B,CAChD,CAEA,+CAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,+CAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,KAAK,CAAC,QAAQ,2CAAC,CACf,mDAAU,CACT,MAAM,CAAE,WACT,CAEA,+CAAK,MAAO,CACX,MAAM,CAAE,OACT"}'
};
const Checkbox = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let disabled;
  let { value = false } = $$props;
  let { label = "Checkbox" } = $$props;
  let { interactive } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  $$result.css.add(css);
  {
    dispatch("change", value);
  }
  disabled = !interactive;
  return `<label class="${["svelte-5ncdh7", disabled ? "disabled" : ""].join(" ").trim()}"><input ${disabled ? "disabled" : ""} type="checkbox" name="test" data-testid="checkbox" class="svelte-5ncdh7"${add_attribute("checked", value, 1)}> <span class="svelte-5ncdh7">${escape(label)}</span> </label>`;
});
const BaseCheckbox = Checkbox;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = false } = $$props;
  let { value_is_output = false } = $$props;
  let { label = "Checkbox" } = $$props;
  let { info = void 0 } = $$props;
  let { root } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        elem_id,
        elem_classes,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${info ? `${validate_component(Info$1, "Info").$$render($$result, { root, info }, {}, {})}` : ``} ${validate_component(BaseCheckbox, "BaseCheckbox").$$render(
            $$result,
            { label, interactive, value },
            {
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              }
            },
            {}
          )}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { BaseCheckbox, Index as default };
//# sourceMappingURL=Index34-D-J8arey.js.map
