import { c as create_ssr_component, a as createEventDispatcher, v as validate_component } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, U as UploadText } from './2-B6LMYTAg.js';
import Gallery from './Gallery-S5gOndMl.js';
import { BaseFileUpload } from './Index8-CqUR0MO2.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './ModifyUpload-CsV9IzIz.js';
import './DownloadLink--4obEanq.js';
import './Image-DFqHtuJN.js';
import './Video-_epjqq1V.js';
import './hls-CrxM9YLy.js';
import './Example11-BratyhqV.js';

const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let no_value;
  let { loading_status } = $$props;
  let { show_label } = $$props;
  let { label } = $$props;
  let { root } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { file_types = ["image", "video"] } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { columns = [2] } = $$props;
  let { rows = void 0 } = $$props;
  let { height = "auto" } = $$props;
  let { preview } = $$props;
  let { allow_preview = true } = $$props;
  let { selected_index = null } = $$props;
  let { object_fit = "cover" } = $$props;
  let { show_share_button = false } = $$props;
  let { interactive } = $$props;
  let { show_download_button = false } = $$props;
  let { gradio } = $$props;
  let { show_fullscreen_button = true } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.file_types === void 0 && $$bindings.file_types && file_types !== void 0)
    $$bindings.file_types(file_types);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.columns === void 0 && $$bindings.columns && columns !== void 0)
    $$bindings.columns(columns);
  if ($$props.rows === void 0 && $$bindings.rows && rows !== void 0)
    $$bindings.rows(rows);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.preview === void 0 && $$bindings.preview && preview !== void 0)
    $$bindings.preview(preview);
  if ($$props.allow_preview === void 0 && $$bindings.allow_preview && allow_preview !== void 0)
    $$bindings.allow_preview(allow_preview);
  if ($$props.selected_index === void 0 && $$bindings.selected_index && selected_index !== void 0)
    $$bindings.selected_index(selected_index);
  if ($$props.object_fit === void 0 && $$bindings.object_fit && object_fit !== void 0)
    $$bindings.object_fit(object_fit);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.show_fullscreen_button === void 0 && $$bindings.show_fullscreen_button && show_fullscreen_button !== void 0)
    $$bindings.show_fullscreen_button(show_fullscreen_button);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    no_value = value === null ? true : value.length === 0;
    {
      dispatch("prop_change", { selected_index });
    }
    $$rendered = `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: "solid",
        padding: false,
        elem_id,
        elem_classes,
        container,
        scale,
        min_width,
        allow_overflow: false,
        height: typeof height === "number" ? height : void 0
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${interactive && no_value ? `${validate_component(BaseFileUpload, "BaseFileUpload").$$render(
            $$result,
            {
              value: null,
              root,
              label,
              max_file_size: gradio.max_file_size,
              file_count: "multiple",
              file_types,
              i18n: gradio.i18n,
              upload: (...args) => gradio.client.upload(...args),
              stream_handler: (...args) => gradio.client.stream(...args)
            },
            {},
            {
              default: () => {
                return `${validate_component(UploadText, "UploadText").$$render($$result, { i18n: gradio.i18n, type: "gallery" }, {}, {})}`;
              }
            }
          )}` : `${validate_component(Gallery, "Gallery").$$render(
            $$result,
            {
              label,
              show_label,
              columns,
              rows,
              height,
              preview,
              object_fit,
              interactive,
              allow_preview,
              show_share_button,
              show_download_button,
              i18n: gradio.i18n,
              _fetch: (...args) => gradio.client.fetch(...args),
              show_fullscreen_button,
              selected_index,
              value
            },
            {
              selected_index: ($$value) => {
                selected_index = $$value;
                $$settled = false;
              },
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              }
            },
            {}
          )}`}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { Gallery as BaseGallery, Index as default };
//# sourceMappingURL=Index7-B-c2Czba.js.map
