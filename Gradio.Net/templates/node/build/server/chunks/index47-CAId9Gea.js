import { c as create_ssr_component, a as createEventDispatcher, v as validate_component } from './ssr-RaXq3SJh.js';
import { U as Upload } from './ModifyUpload-CsV9IzIz.js';
import { e as BlockLabel, ap as Video$1, k as SelectSource, B as Block, S as Static, U as UploadText } from './2-B6LMYTAg.js';
import { Webcam as Webcam$1 } from './Index22-OnyqHXrF.js';
import { P as Player$1, V as VideoPreview } from './VideoPreview-LMtm46ex.js';
export { l as loaded, a as playable, p as prettyBytes } from './VideoPreview-LMtm46ex.js';
export { default as BaseExample } from './Example23-M3xiVOYG.js';
import './Component-Dv7eSVA_.js';
import './DownloadLink--4obEanq.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './ImagePreview-BWlE3pis.js';
import './Image-DFqHtuJN.js';
import './Example14-DQrOYOUG.js';
import './Video-_epjqq1V.js';
import './hls-CrxM9YLy.js';

const css = {
  code: ".file-name.svelte-14jis2k{padding:var(--size-6);font-size:var(--text-xxl);word-break:break-all}.file-size.svelte-14jis2k{padding:var(--size-2);font-size:var(--text-xl)}.upload-container.svelte-14jis2k{height:100%;width:100%}.video-container.svelte-14jis2k{display:flex;height:100%;flex-direction:column;justify-content:center;align-items:center}",
  map: '{"version":3,"file":"InteractiveVideo.svelte","sources":["InteractiveVideo.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport { Upload, ModifyUpload } from \\"@gradio/upload\\";\\nimport { BlockLabel } from \\"@gradio/atoms\\";\\nimport { Webcam } from \\"@gradio/image\\";\\nimport { Video } from \\"@gradio/icons\\";\\nimport { prettyBytes, playable } from \\"./utils\\";\\nimport Player from \\"./Player.svelte\\";\\nimport { SelectSource } from \\"@gradio/atoms\\";\\nexport let value = null;\\nexport let subtitle = null;\\nexport let sources = [\\"webcam\\", \\"upload\\"];\\nexport let label = void 0;\\nexport let show_download_button = false;\\nexport let show_label = true;\\nexport let mirror_webcam = false;\\nexport let include_audio;\\nexport let autoplay;\\nexport let root;\\nexport let i18n;\\nexport let active_source = \\"webcam\\";\\nexport let handle_reset_value = () => {\\n};\\nexport let max_file_size = null;\\nexport let upload;\\nexport let stream_handler;\\nexport let loop;\\nexport let uploading = false;\\nlet has_change_history = false;\\nconst dispatch = createEventDispatcher();\\nfunction handle_load({ detail }) {\\n    value = detail;\\n    dispatch(\\"change\\", detail);\\n    dispatch(\\"upload\\", detail);\\n}\\nfunction handle_clear() {\\n    value = null;\\n    dispatch(\\"change\\", null);\\n    dispatch(\\"clear\\");\\n}\\nfunction handle_change(video) {\\n    has_change_history = true;\\n    dispatch(\\"change\\", video);\\n}\\nfunction handle_capture({ detail }) {\\n    dispatch(\\"change\\", detail);\\n}\\nlet dragging = false;\\n$: dispatch(\\"drag\\", dragging);\\n<\/script>\\n\\n<BlockLabel {show_label} Icon={Video} label={label || \\"Video\\"} />\\n<div data-testid=\\"video\\" class=\\"video-container\\">\\n\\t{#if value === null || value.url === undefined}\\n\\t\\t<div class=\\"upload-container\\">\\n\\t\\t\\t{#if active_source === \\"upload\\"}\\n\\t\\t\\t\\t<Upload\\n\\t\\t\\t\\t\\tbind:dragging\\n\\t\\t\\t\\t\\tbind:uploading\\n\\t\\t\\t\\t\\tfiletype=\\"video/x-m4v,video/*\\"\\n\\t\\t\\t\\t\\ton:load={handle_load}\\n\\t\\t\\t\\t\\t{max_file_size}\\n\\t\\t\\t\\t\\ton:error={({ detail }) => dispatch(\\"error\\", detail)}\\n\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t\\t{stream_handler}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<slot />\\n\\t\\t\\t\\t</Upload>\\n\\t\\t\\t{:else if active_source === \\"webcam\\"}\\n\\t\\t\\t\\t<Webcam\\n\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t{mirror_webcam}\\n\\t\\t\\t\\t\\t{include_audio}\\n\\t\\t\\t\\t\\tmode=\\"video\\"\\n\\t\\t\\t\\t\\ton:error\\n\\t\\t\\t\\t\\ton:capture={handle_capture}\\n\\t\\t\\t\\t\\ton:start_recording\\n\\t\\t\\t\\t\\ton:stop_recording\\n\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t\\tstream_every={1}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t</div>\\n\\t{:else if playable()}\\n\\t\\t{#key value?.url}\\n\\t\\t\\t<Player\\n\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t{root}\\n\\t\\t\\t\\tinteractive\\n\\t\\t\\t\\t{autoplay}\\n\\t\\t\\t\\tsrc={value.url}\\n\\t\\t\\t\\tsubtitle={subtitle?.url}\\n\\t\\t\\t\\tis_stream={false}\\n\\t\\t\\t\\ton:play\\n\\t\\t\\t\\ton:pause\\n\\t\\t\\t\\ton:stop\\n\\t\\t\\t\\ton:end\\n\\t\\t\\t\\tmirror={mirror_webcam && active_source === \\"webcam\\"}\\n\\t\\t\\t\\t{label}\\n\\t\\t\\t\\t{handle_change}\\n\\t\\t\\t\\t{handle_reset_value}\\n\\t\\t\\t\\t{loop}\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t{show_download_button}\\n\\t\\t\\t\\t{handle_clear}\\n\\t\\t\\t\\t{has_change_history}\\n\\t\\t\\t/>\\n\\t\\t{/key}\\n\\t{:else if value.size}\\n\\t\\t<div class=\\"file-name\\">{value.orig_name || value.url}</div>\\n\\t\\t<div class=\\"file-size\\">\\n\\t\\t\\t{prettyBytes(value.size)}\\n\\t\\t</div>\\n\\t{/if}\\n\\n\\t<SelectSource {sources} bind:active_source {handle_clear} />\\n</div>\\n\\n<style>\\n\\t.file-name {\\n\\t\\tpadding: var(--size-6);\\n\\t\\tfont-size: var(--text-xxl);\\n\\t\\tword-break: break-all;\\n\\t}\\n\\n\\t.file-size {\\n\\t\\tpadding: var(--size-2);\\n\\t\\tfont-size: var(--text-xl);\\n\\t}\\n\\n\\t.upload-container {\\n\\t\\theight: 100%;\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t.video-container {\\n\\t\\tdisplay: flex;\\n\\t\\theight: 100%;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t}</style>\\n"],"names":[],"mappings":"AAyHC,yBAAW,CACV,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,SAAS,CAAE,IAAI,UAAU,CAAC,CAC1B,UAAU,CAAE,SACb,CAEA,yBAAW,CACV,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,SAAS,CAAE,IAAI,SAAS,CACzB,CAEA,gCAAkB,CACjB,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,IACR,CAEA,+BAAiB,CAChB,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,IAAI,CACZ,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MACd"}'
};
const InteractiveVideo = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value = null } = $$props;
  let { subtitle = null } = $$props;
  let { sources = ["webcam", "upload"] } = $$props;
  let { label = void 0 } = $$props;
  let { show_download_button = false } = $$props;
  let { show_label = true } = $$props;
  let { mirror_webcam = false } = $$props;
  let { include_audio } = $$props;
  let { autoplay } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { active_source = "webcam" } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { loop } = $$props;
  let { uploading = false } = $$props;
  let has_change_history = false;
  const dispatch = createEventDispatcher();
  function handle_clear() {
    value = null;
    dispatch("change", null);
    dispatch("clear");
  }
  function handle_change(video) {
    has_change_history = true;
    dispatch("change", video);
  }
  let dragging = false;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.subtitle === void 0 && $$bindings.subtitle && subtitle !== void 0)
    $$bindings.subtitle(subtitle);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.mirror_webcam === void 0 && $$bindings.mirror_webcam && mirror_webcam !== void 0)
    $$bindings.mirror_webcam(mirror_webcam);
  if ($$props.include_audio === void 0 && $$bindings.include_audio && include_audio !== void 0)
    $$bindings.include_audio(include_audio);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.active_source === void 0 && $$bindings.active_source && active_source !== void 0)
    $$bindings.active_source(active_source);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.uploading === void 0 && $$bindings.uploading && uploading !== void 0)
    $$bindings.uploading(uploading);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      dispatch("drag", dragging);
    }
    $$rendered = `${validate_component(BlockLabel, "BlockLabel").$$render(
      $$result,
      {
        show_label,
        Icon: Video$1,
        label: label || "Video"
      },
      {},
      {}
    )} <div data-testid="video" class="video-container svelte-14jis2k">${value === null || value.url === void 0 ? `<div class="upload-container svelte-14jis2k">${active_source === "upload" ? `${validate_component(Upload, "Upload").$$render(
      $$result,
      {
        filetype: "video/x-m4v,video/*",
        max_file_size,
        root,
        upload,
        stream_handler,
        dragging,
        uploading
      },
      {
        dragging: ($$value) => {
          dragging = $$value;
          $$settled = false;
        },
        uploading: ($$value) => {
          uploading = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `${slots.default ? slots.default({}) : ``}`;
        }
      }
    )}` : `${active_source === "webcam" ? `${validate_component(Webcam$1, "Webcam").$$render(
      $$result,
      {
        root,
        mirror_webcam,
        include_audio,
        mode: "video",
        i18n,
        upload,
        stream_every: 1
      },
      {},
      {}
    )}` : ``}`}</div>` : `${`${validate_component(Player$1, "Player").$$render(
      $$result,
      {
        upload,
        root,
        interactive: true,
        autoplay,
        src: value.url,
        subtitle: subtitle?.url,
        is_stream: false,
        mirror: mirror_webcam && active_source === "webcam",
        label,
        handle_change,
        handle_reset_value,
        loop,
        value,
        i18n,
        show_download_button,
        handle_clear,
        has_change_history
      },
      {},
      {}
    )}`}`} ${validate_component(SelectSource, "SelectSource").$$render(
      $$result,
      { sources, handle_clear, active_source },
      {
        active_source: ($$value) => {
          active_source = $$value;
          $$settled = false;
        }
      },
      {}
    )} </div>`;
  } while (!$$settled);
  return $$rendered;
});
const Video = InteractiveVideo;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let old_value = null;
  let { label } = $$props;
  let { sources } = $$props;
  let { root } = $$props;
  let { show_label } = $$props;
  let { loading_status } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { container = false } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { autoplay = false } = $$props;
  let { show_share_button = true } = $$props;
  let { show_download_button } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  let { mirror_webcam } = $$props;
  let { include_audio } = $$props;
  let { loop = false } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let _video = null;
  let _subtitle = null;
  let active_source;
  let initial_value = value;
  const handle_reset_value = () => {
    if (initial_value === null || value === initial_value) {
      return;
    }
    value = initial_value;
  };
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.width === void 0 && $$bindings.width && width !== void 0)
    $$bindings.width(width);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.mirror_webcam === void 0 && $$bindings.mirror_webcam && mirror_webcam !== void 0)
    $$bindings.mirror_webcam(mirror_webcam);
  if ($$props.include_audio === void 0 && $$bindings.include_audio && include_audio !== void 0)
    $$bindings.include_audio(include_audio);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.input_ready === void 0 && $$bindings.input_ready && input_ready !== void 0)
    $$bindings.input_ready(input_ready);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    input_ready = !uploading;
    {
      if (value && initial_value === null) {
        initial_value = value;
      }
    }
    {
      if (sources && !active_source) {
        active_source = sources[0];
      }
    }
    {
      {
        if (value != null) {
          _video = value.video;
          _subtitle = value.subtitles;
        } else {
          _video = null;
          _subtitle = null;
        }
      }
    }
    {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          old_value = value;
          gradio.dispatch("change");
        }
      }
    }
    $$rendered = `  ${!interactive ? `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: value === null && active_source === "upload" ? "dashed" : "solid",
        border_mode: "base",
        padding: false,
        elem_id,
        elem_classes,
        height,
        width,
        container,
        scale,
        min_width,
        allow_overflow: false
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(VideoPreview, "StaticVideo").$$render(
            $$result,
            {
              value: _video,
              subtitle: _subtitle,
              label,
              show_label,
              autoplay,
              loop,
              show_share_button,
              show_download_button,
              i18n: gradio.i18n,
              upload: (...args) => gradio.client.upload(...args)
            },
            {},
            {}
          )}`;
        }
      }
    )}` : `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: value === null && active_source === "upload" ? "dashed" : "solid",
        border_mode: "base",
        padding: false,
        elem_id,
        elem_classes,
        height,
        width,
        container,
        scale,
        min_width,
        allow_overflow: false
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(Video, "Video").$$render(
            $$result,
            {
              value: _video,
              subtitle: _subtitle,
              label,
              show_label,
              show_download_button,
              sources,
              active_source,
              mirror_webcam,
              include_audio,
              autoplay,
              root,
              loop,
              handle_reset_value,
              i18n: gradio.i18n,
              max_file_size: gradio.max_file_size,
              upload: (...args) => gradio.client.upload(...args),
              stream_handler: (...args) => gradio.client.stream(...args),
              uploading
            },
            {
              uploading: ($$value) => {
                uploading = $$value;
                $$settled = false;
              }
            },
            {
              default: () => {
                return `${validate_component(UploadText, "UploadText").$$render($$result, { i18n: gradio.i18n, type: "video" }, {}, {})}`;
              }
            }
          )}`;
        }
      }
    )}`}`;
  } while (!$$settled);
  return $$rendered;
});
const Index$1 = Index;

export { Video as BaseInteractiveVideo, Player$1 as BasePlayer, VideoPreview as BaseStaticVideo, Index$1 as default };
//# sourceMappingURL=index47-CAId9Gea.js.map
