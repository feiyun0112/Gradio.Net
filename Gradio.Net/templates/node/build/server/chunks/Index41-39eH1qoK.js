import { c as create_ssr_component, v as validate_component, e as escape, a as createEventDispatcher, b as add_attribute } from './ssr-RaXq3SJh.js';
import { X as Button } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".hide.svelte-1rvxzzt{display:none}.button-icon.svelte-1rvxzzt{width:var(--text-xl);height:var(--text-xl);margin-right:var(--spacing-xl)}",
  map: '{"version":3,"file":"UploadButton.svelte","sources":["UploadButton.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { tick, createEventDispatcher } from \\"svelte\\";\\nimport { BaseButton } from \\"@gradio/button\\";\\nimport { prepare_files } from \\"@gradio/client\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let label;\\nexport let value;\\nexport let file_count;\\nexport let file_types = [];\\nexport let root;\\nexport let size = \\"lg\\";\\nexport let icon = null;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let variant = \\"secondary\\";\\nexport let disabled = false;\\nexport let max_file_size = null;\\nexport let upload;\\nconst dispatch = createEventDispatcher();\\nlet hidden_upload;\\nlet accept_file_types;\\nif (file_types == null) {\\n    accept_file_types = null;\\n}\\nelse {\\n    file_types = file_types.map((x) => {\\n        if (x.startsWith(\\".\\")) {\\n            return x;\\n        }\\n        return x + \\"/*\\";\\n    });\\n    accept_file_types = file_types.join(\\", \\");\\n}\\nfunction open_file_upload() {\\n    dispatch(\\"click\\");\\n    hidden_upload.click();\\n}\\nasync function load_files(files) {\\n    let _files = Array.from(files);\\n    if (!files.length) {\\n        return;\\n    }\\n    if (file_count === \\"single\\") {\\n        _files = [files[0]];\\n    }\\n    let all_file_data = await prepare_files(_files);\\n    await tick();\\n    try {\\n        all_file_data = (await upload(all_file_data, root, void 0, max_file_size ?? Infinity))?.filter((x) => x !== null);\\n    }\\n    catch (e) {\\n        dispatch(\\"error\\", e.message);\\n        return;\\n    }\\n    value = file_count === \\"single\\" ? all_file_data?.[0] : all_file_data;\\n    dispatch(\\"change\\", value);\\n    dispatch(\\"upload\\", value);\\n}\\nasync function load_files_from_upload(e) {\\n    const target = e.target;\\n    if (!target.files)\\n        return;\\n    await load_files(target.files);\\n}\\nfunction clear_input_value(e) {\\n    const target = e.target;\\n    if (target.value)\\n        target.value = \\"\\";\\n}\\n<\/script>\\n\\n<input\\n\\tclass=\\"hide\\"\\n\\taccept={accept_file_types}\\n\\ttype=\\"file\\"\\n\\tbind:this={hidden_upload}\\n\\ton:change={load_files_from_upload}\\n\\ton:click={clear_input_value}\\n\\tmultiple={file_count === \\"multiple\\" || undefined}\\n\\twebkitdirectory={file_count === \\"directory\\" || undefined}\\n\\tmozdirectory={file_count === \\"directory\\" || undefined}\\n\\tdata-testid=\\"{label}-upload-button\\"\\n/>\\n\\n<BaseButton\\n\\t{size}\\n\\t{variant}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{visible}\\n\\ton:click={open_file_upload}\\n\\t{scale}\\n\\t{min_width}\\n\\t{disabled}\\n>\\n\\t{#if icon}\\n\\t\\t<img class=\\"button-icon\\" src={icon.url} alt={`${value} icon`} />\\n\\t{/if}\\n\\t<slot />\\n</BaseButton>\\n\\n<style>\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}\\n\\t.button-icon {\\n\\t\\twidth: var(--text-xl);\\n\\t\\theight: var(--text-xl);\\n\\t\\tmargin-right: var(--spacing-xl);\\n\\t}</style>\\n"],"names":[],"mappings":"AAuGC,oBAAM,CACL,OAAO,CAAE,IACV,CACA,2BAAa,CACZ,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,YAAY,CAAE,IAAI,YAAY,CAC/B"}'
};
const UploadButton = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { label } = $$props;
  let { value } = $$props;
  let { file_count } = $$props;
  let { file_types = [] } = $$props;
  let { root } = $$props;
  let { size = "lg" } = $$props;
  let { icon = null } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { variant = "secondary" } = $$props;
  let { disabled = false } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  createEventDispatcher();
  let accept_file_types;
  if (file_types == null) {
    accept_file_types = null;
  } else {
    file_types = file_types.map((x) => {
      if (x.startsWith(".")) {
        return x;
      }
      return x + "/*";
    });
    accept_file_types = file_types.join(", ");
  }
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.file_types === void 0 && $$bindings.file_types && file_types !== void 0)
    $$bindings.file_types(file_types);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.size === void 0 && $$bindings.size && size !== void 0)
    $$bindings.size(size);
  if ($$props.icon === void 0 && $$bindings.icon && icon !== void 0)
    $$bindings.icon(icon);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.variant === void 0 && $$bindings.variant && variant !== void 0)
    $$bindings.variant(variant);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  $$result.css.add(css);
  return `<input class="hide svelte-1rvxzzt"${add_attribute("accept", accept_file_types, 0)} type="file" ${file_count === "multiple" || void 0 ? "multiple" : ""}${add_attribute("webkitdirectory", file_count === "directory" || void 0, 0)}${add_attribute("mozdirectory", file_count === "directory" || void 0, 0)} data-testid="${escape(label, true) + "-upload-button"}"> ${validate_component(Button, "BaseButton").$$render(
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
        return `${icon ? `<img class="button-icon svelte-1rvxzzt"${add_attribute("src", icon.url, 0)}${add_attribute("alt", `${value} icon`, 0)}>` : ``} ${slots.default ? slots.default({}) : ``}`;
      }
    }
  )}`;
});
const UploadButton$1 = UploadButton;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let disabled;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { label } = $$props;
  let { value } = $$props;
  let { file_count } = $$props;
  let { file_types = [] } = $$props;
  let { root } = $$props;
  let { size = "lg" } = $$props;
  let { scale = null } = $$props;
  let { icon = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { variant = "secondary" } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.file_types === void 0 && $$bindings.file_types && file_types !== void 0)
    $$bindings.file_types(file_types);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.size === void 0 && $$bindings.size && size !== void 0)
    $$bindings.size(size);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.icon === void 0 && $$bindings.icon && icon !== void 0)
    $$bindings.icon(icon);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.variant === void 0 && $$bindings.variant && variant !== void 0)
    $$bindings.variant(variant);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  disabled = !interactive;
  return `${validate_component(UploadButton$1, "UploadButton").$$render(
    $$result,
    {
      elem_id,
      elem_classes,
      visible,
      file_count,
      file_types,
      size,
      scale,
      icon,
      min_width,
      root,
      value,
      disabled,
      variant,
      label,
      max_file_size: gradio.max_file_size,
      upload: (...args) => gradio.client.upload(...args)
    },
    {},
    {
      default: () => {
        return `${escape(label ? gradio.i18n(label) : "")}`;
      }
    }
  )}`;
});

export { UploadButton$1 as BaseUploadButton, Index as default };
//# sourceMappingURL=Index41-39eH1qoK.js.map
