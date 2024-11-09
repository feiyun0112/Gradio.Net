import { c as create_ssr_component, v as validate_component, e as escape, a as createEventDispatcher, b as add_attribute } from './ssr-RaXq3SJh.js';
import { X as Button } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".button-icon.svelte-yjn27e{width:var(--text-xl);height:var(--text-xl);margin-right:var(--spacing-xl)}",
  map: '{"version":3,"file":"DownloadButton.svelte","sources":["DownloadButton.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport {} from \\"@gradio/client\\";\\nimport { BaseButton } from \\"@gradio/button\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let variant = \\"secondary\\";\\nexport let size = \\"lg\\";\\nexport let value;\\nexport let icon;\\nexport let disabled = false;\\nexport let scale = null;\\nexport let min_width = void 0;\\nconst dispatch = createEventDispatcher();\\nfunction download_file() {\\n    dispatch(\\"click\\");\\n    if (!value?.url) {\\n        return;\\n    }\\n    let file_name;\\n    if (!value.orig_name && value.url) {\\n        const parts = value.url.split(\\"/\\");\\n        file_name = parts[parts.length - 1];\\n        file_name = file_name.split(\\"?\\")[0].split(\\"#\\")[0];\\n    }\\n    else {\\n        file_name = value.orig_name;\\n    }\\n    const a = document.createElement(\\"a\\");\\n    a.href = value.url;\\n    a.download = file_name || \\"file\\";\\n    document.body.appendChild(a);\\n    a.click();\\n    document.body.removeChild(a);\\n}\\n<\/script>\\n\\n<BaseButton\\n\\t{size}\\n\\t{variant}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{visible}\\n\\ton:click={download_file}\\n\\t{scale}\\n\\t{min_width}\\n\\t{disabled}\\n>\\n\\t{#if icon}\\n\\t\\t<img class=\\"button-icon\\" src={icon.url} alt={`${value} icon`} />\\n\\t{/if}\\n\\t<slot />\\n</BaseButton>\\n\\n<style>\\n\\t.button-icon {\\n\\t\\twidth: var(--text-xl);\\n\\t\\theight: var(--text-xl);\\n\\t\\tmargin-right: var(--spacing-xl);\\n\\t}</style>\\n"],"names":[],"mappings":"AAuDC,0BAAa,CACZ,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,YAAY,CAAE,IAAI,YAAY,CAC/B"}'
};
const DownloadButton = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { variant = "secondary" } = $$props;
  let { size = "lg" } = $$props;
  let { value } = $$props;
  let { icon } = $$props;
  let { disabled = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  createEventDispatcher();
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.variant === void 0 && $$bindings.variant && variant !== void 0)
    $$bindings.variant(variant);
  if ($$props.size === void 0 && $$bindings.size && size !== void 0)
    $$bindings.size(size);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.icon === void 0 && $$bindings.icon && icon !== void 0)
    $$bindings.icon(icon);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  $$result.css.add(css);
  return `${validate_component(Button, "BaseButton").$$render(
    $$result,
    {
      size,
      variant,
      elem_id,
      elem_classes,
      visible,
      scale,
      min_width,
      disabled
    },
    {},
    {
      default: () => {
        return `${icon ? `<img class="button-icon svelte-yjn27e"${add_attribute("src", icon.url, 0)}${add_attribute("alt", `${value} icon`, 0)}>` : ``} ${slots.default ? slots.default({}) : ``}`;
      }
    }
  )}`;
});
const DownloadButton$1 = DownloadButton;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { variant = "secondary" } = $$props;
  let { interactive } = $$props;
  let { size = "lg" } = $$props;
  let { scale = null } = $$props;
  let { icon = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { label = null } = $$props;
  let { gradio } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.variant === void 0 && $$bindings.variant && variant !== void 0)
    $$bindings.variant(variant);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.size === void 0 && $$bindings.size && size !== void 0)
    $$bindings.size(size);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.icon === void 0 && $$bindings.icon && icon !== void 0)
    $$bindings.icon(icon);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  return `${validate_component(DownloadButton$1, "DownloadButton").$$render(
    $$result,
    {
      value,
      variant,
      elem_id,
      elem_classes,
      size,
      scale,
      icon,
      min_width,
      visible,
      disabled: !interactive
    },
    {},
    {
      default: () => {
        return `${escape(label ? gradio.i18n(label) : "")}`;
      }
    }
  )}`;
});

export { DownloadButton$1 as BaseButton, Index as default };
//# sourceMappingURL=Index36-BZidHjPI.js.map
