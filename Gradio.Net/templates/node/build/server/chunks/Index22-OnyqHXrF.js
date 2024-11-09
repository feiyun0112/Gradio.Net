import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, b as add_attribute, d as add_styles, e as escape } from './ssr-RaXq3SJh.js';
import ImagePreview from './ImagePreview-BWlE3pis.js';
import { B as Block, S as Static, U as UploadText, g as Empty, f as Image, b as StreamingBar, e as BlockLabel, k as SelectSource, a9 as Webcam$2, h as IconButtonWrapper, i as IconButton, Z as Clear } from './2-B6LMYTAg.js';
import { U as Upload } from './ModifyUpload-CsV9IzIz.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
export { default as BaseExample } from './Example14-DQrOYOUG.js';
import './DownloadLink--4obEanq.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$2 = {
  code: "button.svelte-qbrfs{cursor:pointer;width:var(--size-full)}.wrap.svelte-qbrfs{display:flex;flex-direction:column;justify-content:center;align-items:center;min-height:var(--size-60);color:var(--block-label-text-color);height:100%;padding-top:var(--size-3)}.icon-wrap.svelte-qbrfs{width:30px;margin-bottom:var(--spacing-lg)}@media(min-width: 768px){.wrap.svelte-qbrfs{font-size:var(--text-lg)}}",
  map: '{"version":3,"file":"WebcamPermissions.svelte","sources":["WebcamPermissions.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { Webcam } from \\"@gradio/icons\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nconst dispatch = createEventDispatcher();\\n<\/script>\\n\\n<button style:height=\\"100%\\" on:click={() => dispatch(\\"click\\")}>\\n\\t<div class=\\"wrap\\">\\n\\t\\t<span class=\\"icon-wrap\\">\\n\\t\\t\\t<Webcam />\\n\\t\\t</span>\\n\\t\\t{\\"Click to Access Webcam\\"}\\n\\t</div>\\n</button>\\n\\n<style>\\n\\tbutton {\\n\\t\\tcursor: pointer;\\n\\t\\twidth: var(--size-full);\\n\\t}\\n\\n\\t.wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tmin-height: var(--size-60);\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t\\theight: 100%;\\n\\t\\tpadding-top: var(--size-3);\\n\\t}\\n\\n\\t.icon-wrap {\\n\\t\\twidth: 30px;\\n\\t\\tmargin-bottom: var(--spacing-lg);\\n\\t}\\n\\n\\t@media (min-width: 768px) {\\n\\t\\t.wrap {\\n\\t\\t\\tfont-size: var(--text-lg);\\n\\t\\t}\\n\\t}</style>\\n"],"names":[],"mappings":"AAeC,mBAAO,CACN,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,WAAW,CACvB,CAEA,kBAAM,CACL,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,SAAS,CAAC,CAC1B,KAAK,CAAE,IAAI,wBAAwB,CAAC,CACpC,MAAM,CAAE,IAAI,CACZ,WAAW,CAAE,IAAI,QAAQ,CAC1B,CAEA,uBAAW,CACV,KAAK,CAAE,IAAI,CACX,aAAa,CAAE,IAAI,YAAY,CAChC,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,kBAAM,CACL,SAAS,CAAE,IAAI,SAAS,CACzB,CACD"}'
};
const WebcamPermissions = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  createEventDispatcher();
  $$result.css.add(css$2);
  return `<button class="svelte-qbrfs"${add_styles({ "height": `100%` })}><div class="wrap svelte-qbrfs"><span class="icon-wrap svelte-qbrfs">${validate_component(Webcam$2, "Webcam").$$render($$result, {}, {}, {})}</span> ${escape("Click to Access Webcam")}</div> </button>`;
});
const css$1 = {
  code: ".wrap.svelte-s8feoe.svelte-s8feoe{position:relative;width:var(--size-full);height:var(--size-full)}.hide.svelte-s8feoe.svelte-s8feoe{display:none}video.svelte-s8feoe.svelte-s8feoe{width:var(--size-full);height:var(--size-full);object-fit:cover}.button-wrap.svelte-s8feoe.svelte-s8feoe{position:absolute;background-color:var(--block-background-fill);border:1px solid var(--border-color-primary);border-radius:var(--radius-xl);padding:var(--size-1-5);display:flex;bottom:var(--size-2);left:50%;transform:translate(-50%, 0);box-shadow:var(--shadow-drop-lg);border-radius:var(--radius-xl);line-height:var(--size-3);color:var(--button-secondary-text-color)}.icon-with-text.svelte-s8feoe.svelte-s8feoe{width:var(--size-20);align-items:center;margin:0 var(--spacing-xl);display:flex;justify-content:space-evenly}@media(min-width: 768px){button.svelte-s8feoe.svelte-s8feoe{bottom:var(--size-4)}}@media(min-width: 1280px){button.svelte-s8feoe.svelte-s8feoe{bottom:var(--size-8)}}.icon.svelte-s8feoe.svelte-s8feoe{width:18px;height:18px;display:flex;justify-content:space-between;align-items:center}.color-primary.svelte-s8feoe.svelte-s8feoe{fill:var(--primary-600);stroke:var(--primary-600);color:var(--primary-600)}.flip.svelte-s8feoe.svelte-s8feoe{transform:scaleX(-1)}.select-wrap.svelte-s8feoe.svelte-s8feoe{-webkit-appearance:none;-moz-appearance:none;appearance:none;color:var(--button-secondary-text-color);background-color:transparent;width:95%;font-size:var(--text-md);position:absolute;bottom:var(--size-2);background-color:var(--block-background-fill);box-shadow:var(--shadow-drop-lg);border-radius:var(--radius-xl);z-index:var(--layer-top);border:1px solid var(--border-color-primary);text-align:left;line-height:var(--size-4);white-space:nowrap;text-overflow:ellipsis;left:50%;transform:translate(-50%, 0);max-width:var(--size-52)}.select-wrap.svelte-s8feoe>option.svelte-s8feoe{padding:0.25rem 0.5rem;border-bottom:1px solid var(--border-color-accent);padding-right:var(--size-8);text-overflow:ellipsis;overflow:hidden}.select-wrap.svelte-s8feoe>option.svelte-s8feoe:hover{background-color:var(--color-accent)}.select-wrap.svelte-s8feoe>option.svelte-s8feoe:last-child{border:none}.inset-icon.svelte-s8feoe.svelte-s8feoe{position:absolute;top:5px;right:-6.5px;width:var(--size-10);height:var(--size-5);opacity:0.8}@media(min-width: 768px){.wrap.svelte-s8feoe.svelte-s8feoe{font-size:var(--text-lg)}}",
  map: '{"version":3,"file":"Webcam.svelte","sources":["Webcam.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, onMount } from \\"svelte\\";\\nimport { Camera, Circle, Square, DropdownArrow, Spinner } from \\"@gradio/icons\\";\\nimport { StreamingBar } from \\"@gradio/statustracker\\";\\nimport { prepare_files } from \\"@gradio/client\\";\\nimport WebcamPermissions from \\"./WebcamPermissions.svelte\\";\\nimport { fade } from \\"svelte/transition\\";\\nimport { get_devices, get_video_stream, set_available_devices } from \\"./stream_utils\\";\\nlet video_source;\\nlet available_video_devices = [];\\nlet selected_device = null;\\nlet time_limit = null;\\nlet stream_state = \\"closed\\";\\nexport const modify_stream = (state) => {\\n    if (state === \\"closed\\") {\\n        time_limit = null;\\n        stream_state = \\"closed\\";\\n        value = null;\\n    }\\n    else if (state === \\"waiting\\") {\\n        stream_state = \\"waiting\\";\\n    }\\n    else {\\n        stream_state = \\"open\\";\\n    }\\n};\\nexport const set_time_limit = (time) => {\\n    if (recording)\\n        time_limit = time;\\n};\\nlet canvas;\\nexport let streaming = false;\\nexport let pending = false;\\nexport let root = \\"\\";\\nexport let stream_every = 1;\\nexport let mode = \\"image\\";\\nexport let mirror_webcam;\\nexport let include_audio;\\nexport let i18n;\\nexport let upload;\\nexport let value = null;\\nconst dispatch = createEventDispatcher();\\nonMount(() => {\\n    canvas = document.createElement(\\"canvas\\");\\n    if (streaming && mode === \\"image\\") {\\n        window.setInterval(() => {\\n            if (video_source && !pending) {\\n                take_picture();\\n            }\\n        }, stream_every * 1e3);\\n    }\\n});\\nconst handle_device_change = async (event) => {\\n    const target = event.target;\\n    const device_id = target.value;\\n    await get_video_stream(include_audio, video_source, device_id).then(async (local_stream) => {\\n        stream = local_stream;\\n        selected_device = available_video_devices.find((device) => device.deviceId === device_id) || null;\\n        options_open = false;\\n    });\\n};\\nasync function access_webcam() {\\n    try {\\n        get_video_stream(include_audio, video_source).then(async (local_stream) => {\\n            webcam_accessed = true;\\n            available_video_devices = await get_devices();\\n            stream = local_stream;\\n        }).then(() => set_available_devices(available_video_devices)).then((devices) => {\\n            available_video_devices = devices;\\n            const used_devices = stream.getTracks().map((track) => track.getSettings()?.deviceId)[0];\\n            selected_device = used_devices ? devices.find((device) => device.deviceId === used_devices) || available_video_devices[0] : available_video_devices[0];\\n        });\\n        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {\\n            dispatch(\\"error\\", i18n(\\"image.no_webcam_support\\"));\\n        }\\n    }\\n    catch (err) {\\n        if (err instanceof DOMException && err.name == \\"NotAllowedError\\") {\\n            dispatch(\\"error\\", i18n(\\"image.allow_webcam_access\\"));\\n        }\\n        else {\\n            throw err;\\n        }\\n    }\\n}\\nfunction take_picture() {\\n    var context = canvas.getContext(\\"2d\\");\\n    if ((!streaming || streaming && recording) && video_source.videoWidth && video_source.videoHeight) {\\n        canvas.width = video_source.videoWidth;\\n        canvas.height = video_source.videoHeight;\\n        context.drawImage(video_source, 0, 0, video_source.videoWidth, video_source.videoHeight);\\n        if (mirror_webcam) {\\n            context.scale(-1, 1);\\n            context.drawImage(video_source, -video_source.videoWidth, 0);\\n        }\\n        if (streaming && (!recording || stream_state === \\"waiting\\")) {\\n            return;\\n        }\\n        if (streaming) {\\n            const image_data = canvas.toDataURL(\\"image/jpeg\\");\\n            dispatch(\\"stream\\", image_data);\\n            return;\\n        }\\n        canvas.toBlob((blob) => {\\n            dispatch(streaming ? \\"stream\\" : \\"capture\\", blob);\\n        }, `image/${streaming ? \\"jpeg\\" : \\"png\\"}`, 0.8);\\n    }\\n}\\nlet recording = false;\\nlet recorded_blobs = [];\\nlet stream;\\nlet mimeType;\\nlet media_recorder;\\nfunction take_recording() {\\n    if (recording) {\\n        media_recorder.stop();\\n        let video_blob = new Blob(recorded_blobs, { type: mimeType });\\n        let ReaderObj = new FileReader();\\n        ReaderObj.onload = async function (e) {\\n            if (e.target) {\\n                let _video_blob = new File([video_blob], \\"sample.\\" + mimeType.substring(6));\\n                const val = await prepare_files([_video_blob]);\\n                let val_ = ((await upload(val, root))?.filter(Boolean))[0];\\n                dispatch(\\"capture\\", val_);\\n                dispatch(\\"stop_recording\\");\\n            }\\n        };\\n        ReaderObj.readAsDataURL(video_blob);\\n    }\\n    else {\\n        dispatch(\\"start_recording\\");\\n        recorded_blobs = [];\\n        let validMimeTypes = [\\"video/webm\\", \\"video/mp4\\"];\\n        for (let validMimeType of validMimeTypes) {\\n            if (MediaRecorder.isTypeSupported(validMimeType)) {\\n                mimeType = validMimeType;\\n                break;\\n            }\\n        }\\n        if (mimeType === null) {\\n            console.error(\\"No supported MediaRecorder mimeType\\");\\n            return;\\n        }\\n        media_recorder = new MediaRecorder(stream, {\\n            mimeType\\n        });\\n        media_recorder.addEventListener(\\"dataavailable\\", function (e) {\\n            recorded_blobs.push(e.data);\\n        });\\n        media_recorder.start(200);\\n    }\\n    recording = !recording;\\n}\\nlet webcam_accessed = false;\\nfunction record_video_or_photo() {\\n    if (mode === \\"image\\" && streaming) {\\n        recording = !recording;\\n    }\\n    if (mode === \\"image\\") {\\n        take_picture();\\n    }\\n    else {\\n        take_recording();\\n    }\\n    if (!recording && stream) {\\n        dispatch(\\"close_stream\\");\\n        stream.getTracks().forEach((track) => track.stop());\\n        video_source.srcObject = null;\\n        webcam_accessed = false;\\n        window.setTimeout(() => {\\n            value = null;\\n        }, 500);\\n        value = null;\\n    }\\n}\\nlet options_open = false;\\nexport function click_outside(node, cb) {\\n    const handle_click = (event) => {\\n        if (node && !node.contains(event.target) && !event.defaultPrevented) {\\n            cb(event);\\n        }\\n    };\\n    document.addEventListener(\\"click\\", handle_click, true);\\n    return {\\n        destroy() {\\n            document.removeEventListener(\\"click\\", handle_click, true);\\n        }\\n    };\\n}\\nfunction handle_click_outside(event) {\\n    event.preventDefault();\\n    event.stopPropagation();\\n    options_open = false;\\n}\\n<\/script>\\n\\n<div class=\\"wrap\\">\\n\\t<StreamingBar {time_limit} />\\n\\t<!-- svelte-ignore a11y-media-has-caption -->\\n\\t<!-- need to suppress for video streaming https://github.com/sveltejs/svelte/issues/5967 -->\\n\\t<video\\n\\t\\tbind:this={video_source}\\n\\t\\tclass:flip={mirror_webcam}\\n\\t\\tclass:hide={!webcam_accessed || (webcam_accessed && !!value)}\\n\\t/>\\n\\t<!-- svelte-ignore a11y-missing-attribute -->\\n\\t<img\\n\\t\\tsrc={value?.url}\\n\\t\\tclass:hide={!webcam_accessed || (webcam_accessed && !value)}\\n\\t/>\\n\\t{#if !webcam_accessed}\\n\\t\\t<div\\n\\t\\t\\tin:fade={{ delay: 100, duration: 200 }}\\n\\t\\t\\ttitle=\\"grant webcam access\\"\\n\\t\\t\\tstyle=\\"height: 100%\\"\\n\\t\\t>\\n\\t\\t\\t<WebcamPermissions on:click={async () => access_webcam()} />\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t<div class=\\"button-wrap\\">\\n\\t\\t\\t<button\\n\\t\\t\\t\\ton:click={record_video_or_photo}\\n\\t\\t\\t\\taria-label={mode === \\"image\\" ? \\"capture photo\\" : \\"start recording\\"}\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#if mode === \\"video\\" || streaming}\\n\\t\\t\\t\\t\\t{#if streaming && stream_state === \\"waiting\\"}\\n\\t\\t\\t\\t\\t\\t<div class=\\"icon-with-text\\" style=\\"width:var(--size-24);\\">\\n\\t\\t\\t\\t\\t\\t\\t<div class=\\"icon color-primary\\" title=\\"spinner\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t<Spinner />\\n\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t\\t{i18n(\\"audio.waiting\\")}\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{:else if (streaming && stream_state === \\"open\\") || (!streaming && recording)}\\n\\t\\t\\t\\t\\t\\t<div class=\\"icon-with-text\\">\\n\\t\\t\\t\\t\\t\\t\\t<div class=\\"icon color-primary\\" title=\\"stop recording\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t<Square />\\n\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t\\t{i18n(\\"audio.stop\\")}\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t<div class=\\"icon-with-text\\">\\n\\t\\t\\t\\t\\t\\t\\t<div class=\\"icon color-primary\\" title=\\"start recording\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t<Circle />\\n\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t\\t{i18n(\\"audio.record\\")}\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<div class=\\"icon\\" title=\\"capture photo\\">\\n\\t\\t\\t\\t\\t\\t<Camera />\\n\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</button>\\n\\t\\t\\t{#if !recording}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"icon\\"\\n\\t\\t\\t\\t\\ton:click={() => (options_open = true)}\\n\\t\\t\\t\\t\\taria-label=\\"select input source\\"\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<DropdownArrow />\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t{/if}\\n\\t\\t</div>\\n\\t\\t{#if options_open && selected_device}\\n\\t\\t\\t<select\\n\\t\\t\\t\\tclass=\\"select-wrap\\"\\n\\t\\t\\t\\taria-label=\\"select source\\"\\n\\t\\t\\t\\tuse:click_outside={handle_click_outside}\\n\\t\\t\\t\\ton:change={handle_device_change}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"inset-icon\\"\\n\\t\\t\\t\\t\\ton:click|stopPropagation={() => (options_open = false)}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<DropdownArrow />\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t{#if available_video_devices.length === 0}\\n\\t\\t\\t\\t\\t<option value=\\"\\">{i18n(\\"common.no_devices\\")}</option>\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t{#each available_video_devices as device}\\n\\t\\t\\t\\t\\t\\t<option\\n\\t\\t\\t\\t\\t\\t\\tvalue={device.deviceId}\\n\\t\\t\\t\\t\\t\\t\\tselected={selected_device.deviceId === device.deviceId}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{device.label}\\n\\t\\t\\t\\t\\t\\t</option>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</select>\\n\\t\\t{/if}\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.wrap {\\n\\t\\tposition: relative;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\tvideo {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\t.button-wrap {\\n\\t\\tposition: absolute;\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t\\tpadding: var(--size-1-5);\\n\\t\\tdisplay: flex;\\n\\t\\tbottom: var(--size-2);\\n\\t\\tleft: 50%;\\n\\t\\ttransform: translate(-50%, 0);\\n\\t\\tbox-shadow: var(--shadow-drop-lg);\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t\\tline-height: var(--size-3);\\n\\t\\tcolor: var(--button-secondary-text-color);\\n\\t}\\n\\n\\t.icon-with-text {\\n\\t\\twidth: var(--size-20);\\n\\t\\talign-items: center;\\n\\t\\tmargin: 0 var(--spacing-xl);\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-evenly;\\n\\t}\\n\\n\\t@media (min-width: 768px) {\\n\\t\\tbutton {\\n\\t\\t\\tbottom: var(--size-4);\\n\\t\\t}\\n\\t}\\n\\n\\t@media (min-width: 1280px) {\\n\\t\\tbutton {\\n\\t\\t\\tbottom: var(--size-8);\\n\\t\\t}\\n\\t}\\n\\n\\t.icon {\\n\\t\\twidth: 18px;\\n\\t\\theight: 18px;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: center;\\n\\t}\\n\\n\\t.color-primary {\\n\\t\\tfill: var(--primary-600);\\n\\t\\tstroke: var(--primary-600);\\n\\t\\tcolor: var(--primary-600);\\n\\t}\\n\\n\\t.flip {\\n\\t\\ttransform: scaleX(-1);\\n\\t}\\n\\n\\t.select-wrap {\\n\\t\\t-webkit-appearance: none;\\n\\t\\t-moz-appearance: none;\\n\\t\\tappearance: none;\\n\\t\\tcolor: var(--button-secondary-text-color);\\n\\t\\tbackground-color: transparent;\\n\\t\\twidth: 95%;\\n\\t\\tfont-size: var(--text-md);\\n\\t\\tposition: absolute;\\n\\t\\tbottom: var(--size-2);\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tbox-shadow: var(--shadow-drop-lg);\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t\\tz-index: var(--layer-top);\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\ttext-align: left;\\n\\t\\tline-height: var(--size-4);\\n\\t\\twhite-space: nowrap;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\tleft: 50%;\\n\\t\\ttransform: translate(-50%, 0);\\n\\t\\tmax-width: var(--size-52);\\n\\t}\\n\\n\\t.select-wrap > option {\\n\\t\\tpadding: 0.25rem 0.5rem;\\n\\t\\tborder-bottom: 1px solid var(--border-color-accent);\\n\\t\\tpadding-right: var(--size-8);\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.select-wrap > option:hover {\\n\\t\\tbackground-color: var(--color-accent);\\n\\t}\\n\\n\\t.select-wrap > option:last-child {\\n\\t\\tborder: none;\\n\\t}\\n\\n\\t.inset-icon {\\n\\t\\tposition: absolute;\\n\\t\\ttop: 5px;\\n\\t\\tright: -6.5px;\\n\\t\\twidth: var(--size-10);\\n\\t\\theight: var(--size-5);\\n\\t\\topacity: 0.8;\\n\\t}\\n\\n\\t@media (min-width: 768px) {\\n\\t\\t.wrap {\\n\\t\\t\\tfont-size: var(--text-lg);\\n\\t\\t}\\n\\t}</style>\\n"],"names":[],"mappings":"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"}'
};
function click_outside(node, cb) {
  const handle_click = (event) => {
    if (node && !node.contains(event.target) && !event.defaultPrevented) {
      cb(event);
    }
  };
  document.addEventListener("click", handle_click, true);
  return {
    destroy() {
      document.removeEventListener("click", handle_click, true);
    }
  };
}
const Webcam = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let video_source;
  let time_limit = null;
  const modify_stream = (state) => {
    if (state === "closed") {
      time_limit = null;
      value = null;
    }
  };
  const set_time_limit = (time) => {
  };
  let { streaming = false } = $$props;
  let { pending = false } = $$props;
  let { root = "" } = $$props;
  let { stream_every = 1 } = $$props;
  let { mode = "image" } = $$props;
  let { mirror_webcam } = $$props;
  let { include_audio } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  let { value = null } = $$props;
  createEventDispatcher();
  if ($$props.modify_stream === void 0 && $$bindings.modify_stream && modify_stream !== void 0)
    $$bindings.modify_stream(modify_stream);
  if ($$props.set_time_limit === void 0 && $$bindings.set_time_limit && set_time_limit !== void 0)
    $$bindings.set_time_limit(set_time_limit);
  if ($$props.streaming === void 0 && $$bindings.streaming && streaming !== void 0)
    $$bindings.streaming(streaming);
  if ($$props.pending === void 0 && $$bindings.pending && pending !== void 0)
    $$bindings.pending(pending);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.stream_every === void 0 && $$bindings.stream_every && stream_every !== void 0)
    $$bindings.stream_every(stream_every);
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.mirror_webcam === void 0 && $$bindings.mirror_webcam && mirror_webcam !== void 0)
    $$bindings.mirror_webcam(mirror_webcam);
  if ($$props.include_audio === void 0 && $$bindings.include_audio && include_audio !== void 0)
    $$bindings.include_audio(include_audio);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.click_outside === void 0 && $$bindings.click_outside && click_outside !== void 0)
    $$bindings.click_outside(click_outside);
  $$result.css.add(css$1);
  return `<div class="wrap svelte-s8feoe">${validate_component(StreamingBar, "StreamingBar").$$render($$result, { time_limit }, {}, {})}   <video class="${[
    "svelte-s8feoe",
    (mirror_webcam ? "flip" : "") + " hide"
  ].join(" ").trim()}"${add_attribute("this", video_source, 0)}></video>  <img${add_attribute("src", value?.url, 0)} class="${[
    "svelte-s8feoe",
    "hide"
  ].join(" ").trim()}"> ${`<div title="grant webcam access" style="height: 100%">${validate_component(WebcamPermissions, "WebcamPermissions").$$render($$result, {}, {}, {})}</div>`} </div>`;
});
const Webcam$1 = Webcam;
const ClearImage = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  createEventDispatcher();
  return `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Clear, label: "Remove Image" }, {}, {})}`;
    }
  })}`;
});
const css = {
  code: ".image-frame.svelte-1ti4ehe img{width:var(--size-full);height:var(--size-full);object-fit:scale-down}.image-frame.svelte-1ti4ehe{object-fit:cover;width:100%;height:100%}.upload-container.svelte-1ti4ehe{display:flex;align-items:center;justify-content:center;height:100%;flex-shrink:1;max-height:100%}.reduced-height.svelte-1ti4ehe{height:calc(100% - var(--size-10))}.image-container.svelte-1ti4ehe{display:flex;height:100%;flex-direction:column;justify-content:center;align-items:center;max-height:100%}.selectable.svelte-1ti4ehe{cursor:crosshair}",
  map: '{"version":3,"file":"ImageUploader.svelte","sources":["ImageUploader.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, tick } from \\"svelte\\";\\nimport { BlockLabel } from \\"@gradio/atoms\\";\\nimport { Image as ImageIcon } from \\"@gradio/icons\\";\\nimport {} from \\"@gradio/utils\\";\\nimport { get_coordinates_of_clicked_image } from \\"./utils\\";\\nimport Webcam from \\"./Webcam.svelte\\";\\nimport { Upload } from \\"@gradio/upload\\";\\nimport { FileData } from \\"@gradio/client\\";\\nimport ClearImage from \\"./ClearImage.svelte\\";\\nimport { SelectSource } from \\"@gradio/atoms\\";\\nimport Image from \\"./Image.svelte\\";\\nexport let value = null;\\nexport let label = void 0;\\nexport let show_label;\\nexport let sources = [\\"upload\\", \\"clipboard\\", \\"webcam\\"];\\nexport let streaming = false;\\nexport let pending = false;\\nexport let mirror_webcam;\\nexport let selectable = false;\\nexport let root;\\nexport let i18n;\\nexport let max_file_size = null;\\nexport let upload;\\nexport let stream_handler;\\nexport let stream_every;\\nexport let modify_stream;\\nexport let set_time_limit;\\nlet upload_input;\\nexport let uploading = false;\\nexport let active_source = null;\\nfunction handle_upload({ detail }) {\\n    if (!streaming) {\\n        value = detail;\\n        dispatch(\\"upload\\");\\n    }\\n}\\nfunction handle_clear() {\\n    value = null;\\n    dispatch(\\"clear\\");\\n    dispatch(\\"change\\", null);\\n}\\nasync function handle_save(img_blob, event) {\\n    if (event === \\"stream\\") {\\n        dispatch(\\"stream\\", {\\n            value: { url: img_blob },\\n            is_value_data: true\\n        });\\n        return;\\n    }\\n    pending = true;\\n    const f = await upload_input.load_files([\\n        new File([img_blob], `image/${streaming ? \\"jpeg\\" : \\"png\\"}`)\\n    ]);\\n    if (event === \\"change\\" || event === \\"upload\\") {\\n        value = f?.[0] || null;\\n        await tick();\\n        dispatch(\\"change\\");\\n    }\\n    pending = false;\\n}\\n$: active_streaming = streaming && active_source === \\"webcam\\";\\n$: if (uploading && !active_streaming)\\n    value = null;\\nconst dispatch = createEventDispatcher();\\nexport let dragging = false;\\n$: dispatch(\\"drag\\", dragging);\\nfunction handle_click(evt) {\\n    let coordinates = get_coordinates_of_clicked_image(evt);\\n    if (coordinates) {\\n        dispatch(\\"select\\", { index: coordinates, value: null });\\n    }\\n}\\n$: if (!active_source && sources) {\\n    active_source = sources[0];\\n}\\nasync function handle_select_source(source) {\\n    switch (source) {\\n        case \\"clipboard\\":\\n            upload_input.paste_clipboard();\\n            break;\\n        default:\\n            break;\\n    }\\n}\\n<\/script>\\n\\n<BlockLabel {show_label} Icon={ImageIcon} label={label || \\"Image\\"} />\\n\\n<div data-testid=\\"image\\" class=\\"image-container\\">\\n\\t{#if value?.url && !active_streaming}\\n\\t\\t<ClearImage\\n\\t\\t\\ton:remove_image={() => {\\n\\t\\t\\t\\tvalue = null;\\n\\t\\t\\t\\tdispatch(\\"clear\\");\\n\\t\\t\\t}}\\n\\t\\t/>\\n\\t{/if}\\n\\t<div\\n\\t\\tclass=\\"upload-container\\"\\n\\t\\tclass:reduced-height={sources.length > 1}\\n\\t\\tstyle:width={value ? \\"auto\\" : \\"100%\\"}\\n\\t>\\n\\t\\t<Upload\\n\\t\\t\\thidden={value !== null || active_source === \\"webcam\\"}\\n\\t\\t\\tbind:this={upload_input}\\n\\t\\t\\tbind:uploading\\n\\t\\t\\tbind:dragging\\n\\t\\t\\tfiletype={active_source === \\"clipboard\\" ? \\"clipboard\\" : \\"image/*\\"}\\n\\t\\t\\ton:load={handle_upload}\\n\\t\\t\\ton:error\\n\\t\\t\\t{root}\\n\\t\\t\\t{max_file_size}\\n\\t\\t\\tdisable_click={!sources.includes(\\"upload\\") || value !== null}\\n\\t\\t\\t{upload}\\n\\t\\t\\t{stream_handler}\\n\\t\\t>\\n\\t\\t\\t{#if value === null}\\n\\t\\t\\t\\t<slot />\\n\\t\\t\\t{/if}\\n\\t\\t</Upload>\\n\\t\\t{#if active_source === \\"webcam\\" && (streaming || (!streaming && !value))}\\n\\t\\t\\t<Webcam\\n\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\ton:capture={(e) => handle_save(e.detail, \\"change\\")}\\n\\t\\t\\t\\ton:stream={(e) => handle_save(e.detail, \\"stream\\")}\\n\\t\\t\\t\\ton:error\\n\\t\\t\\t\\ton:drag\\n\\t\\t\\t\\ton:upload={(e) => handle_save(e.detail, \\"upload\\")}\\n\\t\\t\\t\\ton:close_stream\\n\\t\\t\\t\\t{mirror_webcam}\\n\\t\\t\\t\\t{stream_every}\\n\\t\\t\\t\\t{streaming}\\n\\t\\t\\t\\tmode=\\"image\\"\\n\\t\\t\\t\\tinclude_audio={false}\\n\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\tbind:modify_stream\\n\\t\\t\\t\\tbind:set_time_limit\\n\\t\\t\\t/>\\n\\t\\t{:else if value !== null && !streaming}\\n\\t\\t\\t<!-- svelte-ignore a11y-click-events-have-key-events-->\\n\\t\\t\\t<!-- svelte-ignore a11y-no-static-element-interactions-->\\n\\t\\t\\t<div class:selectable class=\\"image-frame\\" on:click={handle_click}>\\n\\t\\t\\t\\t<Image src={value.url} alt={value.alt_text} />\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\t</div>\\n\\t{#if sources.length > 1 || sources.includes(\\"clipboard\\")}\\n\\t\\t<SelectSource\\n\\t\\t\\t{sources}\\n\\t\\t\\tbind:active_source\\n\\t\\t\\t{handle_clear}\\n\\t\\t\\thandle_select={handle_select_source}\\n\\t\\t/>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.image-frame :global(img) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: scale-down;\\n\\t}\\n\\n\\t.image-frame {\\n\\t\\tobject-fit: cover;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.upload-container {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\n\\t\\theight: 100%;\\n\\t\\tflex-shrink: 1;\\n\\t\\tmax-height: 100%;\\n\\t}\\n\\n\\t.reduced-height {\\n\\t\\theight: calc(100% - var(--size-10));\\n\\t}\\n\\n\\t.image-container {\\n\\t\\tdisplay: flex;\\n\\t\\theight: 100%;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tmax-height: 100%;\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: crosshair;\\n\\t}</style>\\n"],"names":[],"mappings":"AA+JC,2BAAY,CAAS,GAAK,CACzB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,UAAU,CAAE,UACb,CAEA,2BAAa,CACZ,UAAU,CAAE,KAAK,CACjB,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IACT,CAEA,gCAAkB,CACjB,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CAEvB,MAAM,CAAE,IAAI,CACZ,WAAW,CAAE,CAAC,CACd,UAAU,CAAE,IACb,CAEA,8BAAgB,CACf,MAAM,CAAE,KAAK,IAAI,CAAC,CAAC,CAAC,IAAI,SAAS,CAAC,CACnC,CAEA,+BAAiB,CAChB,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,IAAI,CACZ,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IACb,CAEA,0BAAY,CACX,MAAM,CAAE,SACT"}'
};
const ImageUploader = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let active_streaming;
  let { value = null } = $$props;
  let { label = void 0 } = $$props;
  let { show_label } = $$props;
  let { sources = ["upload", "clipboard", "webcam"] } = $$props;
  let { streaming = false } = $$props;
  let { pending = false } = $$props;
  let { mirror_webcam } = $$props;
  let { selectable = false } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { stream_every } = $$props;
  let { modify_stream } = $$props;
  let { set_time_limit } = $$props;
  let upload_input;
  let { uploading = false } = $$props;
  let { active_source = null } = $$props;
  function handle_clear() {
    value = null;
    dispatch("clear");
    dispatch("change", null);
  }
  const dispatch = createEventDispatcher();
  let { dragging = false } = $$props;
  async function handle_select_source(source) {
    switch (source) {
      case "clipboard":
        upload_input.paste_clipboard();
        break;
    }
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.streaming === void 0 && $$bindings.streaming && streaming !== void 0)
    $$bindings.streaming(streaming);
  if ($$props.pending === void 0 && $$bindings.pending && pending !== void 0)
    $$bindings.pending(pending);
  if ($$props.mirror_webcam === void 0 && $$bindings.mirror_webcam && mirror_webcam !== void 0)
    $$bindings.mirror_webcam(mirror_webcam);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  if ($$props.stream_every === void 0 && $$bindings.stream_every && stream_every !== void 0)
    $$bindings.stream_every(stream_every);
  if ($$props.modify_stream === void 0 && $$bindings.modify_stream && modify_stream !== void 0)
    $$bindings.modify_stream(modify_stream);
  if ($$props.set_time_limit === void 0 && $$bindings.set_time_limit && set_time_limit !== void 0)
    $$bindings.set_time_limit(set_time_limit);
  if ($$props.uploading === void 0 && $$bindings.uploading && uploading !== void 0)
    $$bindings.uploading(uploading);
  if ($$props.active_source === void 0 && $$bindings.active_source && active_source !== void 0)
    $$bindings.active_source(active_source);
  if ($$props.dragging === void 0 && $$bindings.dragging && dragging !== void 0)
    $$bindings.dragging(dragging);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      if (!active_source && sources) {
        active_source = sources[0];
      }
    }
    active_streaming = streaming && active_source === "webcam";
    {
      if (uploading && !active_streaming)
        value = null;
    }
    {
      dispatch("drag", dragging);
    }
    $$rendered = `${validate_component(BlockLabel, "BlockLabel").$$render(
      $$result,
      {
        show_label,
        Icon: Image,
        label: label || "Image"
      },
      {},
      {}
    )} <div data-testid="image" class="image-container svelte-1ti4ehe">${value?.url && !active_streaming ? `${validate_component(ClearImage, "ClearImage").$$render($$result, {}, {}, {})}` : ``} <div class="${[
      "upload-container svelte-1ti4ehe",
      sources.length > 1 ? "reduced-height" : ""
    ].join(" ").trim()}"${add_styles({ "width": value ? "auto" : "100%" })}>${validate_component(Upload, "Upload").$$render(
      $$result,
      {
        hidden: value !== null || active_source === "webcam",
        filetype: active_source === "clipboard" ? "clipboard" : "image/*",
        root,
        max_file_size,
        disable_click: !sources.includes("upload") || value !== null,
        upload,
        stream_handler,
        this: upload_input,
        uploading,
        dragging
      },
      {
        this: ($$value) => {
          upload_input = $$value;
          $$settled = false;
        },
        uploading: ($$value) => {
          uploading = $$value;
          $$settled = false;
        },
        dragging: ($$value) => {
          dragging = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `${value === null ? `${slots.default ? slots.default({}) : ``}` : ``}`;
        }
      }
    )} ${active_source === "webcam" && (streaming || !streaming && !value) ? `${validate_component(Webcam$1, "Webcam").$$render(
      $$result,
      {
        root,
        value,
        mirror_webcam,
        stream_every,
        streaming,
        mode: "image",
        include_audio: false,
        i18n,
        upload,
        modify_stream,
        set_time_limit
      },
      {
        modify_stream: ($$value) => {
          modify_stream = $$value;
          $$settled = false;
        },
        set_time_limit: ($$value) => {
          set_time_limit = $$value;
          $$settled = false;
        }
      },
      {}
    )}` : `${value !== null && !streaming ? `  <div class="${["image-frame svelte-1ti4ehe", selectable ? "selectable" : ""].join(" ").trim()}">${validate_component(Image$1, "Image").$$render($$result, { src: value.url, alt: value.alt_text }, {}, {})}</div>` : ``}`}</div> ${sources.length > 1 || sources.includes("clipboard") ? `${validate_component(SelectSource, "SelectSource").$$render(
      $$result,
      {
        sources,
        handle_clear,
        handle_select: handle_select_source,
        active_source
      },
      {
        active_source: ($$value) => {
          active_source = $$value;
          $$settled = false;
        }
      },
      {}
    )}` : ``} </div>`;
  } while (!$$settled);
  return $$rendered;
});
const ImageUploader$1 = ImageUploader;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let stream_state = "closed";
  let _modify_stream = () => {
  };
  function modify_stream_state(state) {
    stream_state = state;
    _modify_stream(state);
  }
  const get_stream_state = () => stream_state;
  let { set_time_limit } = $$props;
  let { value_is_output = false } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let old_value = null;
  let { label } = $$props;
  let { show_label } = $$props;
  let { show_download_button } = $$props;
  let { root } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { stream_every } = $$props;
  let { _selectable = false } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { show_share_button = false } = $$props;
  let { sources = ["upload", "clipboard", "webcam"] } = $$props;
  let { interactive } = $$props;
  let { streaming } = $$props;
  let { pending } = $$props;
  let { mirror_webcam } = $$props;
  let { placeholder = void 0 } = $$props;
  let { show_fullscreen_button } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let { gradio } = $$props;
  let dragging;
  let active_source = null;
  let upload_component;
  if ($$props.modify_stream_state === void 0 && $$bindings.modify_stream_state && modify_stream_state !== void 0)
    $$bindings.modify_stream_state(modify_stream_state);
  if ($$props.get_stream_state === void 0 && $$bindings.get_stream_state && get_stream_state !== void 0)
    $$bindings.get_stream_state(get_stream_state);
  if ($$props.set_time_limit === void 0 && $$bindings.set_time_limit && set_time_limit !== void 0)
    $$bindings.set_time_limit(set_time_limit);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
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
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.width === void 0 && $$bindings.width && width !== void 0)
    $$bindings.width(width);
  if ($$props.stream_every === void 0 && $$bindings.stream_every && stream_every !== void 0)
    $$bindings.stream_every(stream_every);
  if ($$props._selectable === void 0 && $$bindings._selectable && _selectable !== void 0)
    $$bindings._selectable(_selectable);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.streaming === void 0 && $$bindings.streaming && streaming !== void 0)
    $$bindings.streaming(streaming);
  if ($$props.pending === void 0 && $$bindings.pending && pending !== void 0)
    $$bindings.pending(pending);
  if ($$props.mirror_webcam === void 0 && $$bindings.mirror_webcam && mirror_webcam !== void 0)
    $$bindings.mirror_webcam(mirror_webcam);
  if ($$props.placeholder === void 0 && $$bindings.placeholder && placeholder !== void 0)
    $$bindings.placeholder(placeholder);
  if ($$props.show_fullscreen_button === void 0 && $$bindings.show_fullscreen_button && show_fullscreen_button !== void 0)
    $$bindings.show_fullscreen_button(show_fullscreen_button);
  if ($$props.input_ready === void 0 && $$bindings.input_ready && input_ready !== void 0)
    $$bindings.input_ready(input_ready);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    input_ready = !uploading;
    {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          old_value = value;
          gradio.dispatch("change");
          if (!value_is_output) {
            gradio.dispatch("input");
          }
        }
      }
    }
    $$rendered = `   ${!interactive ? `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: "solid",
        border_mode: dragging ? "focus" : "base",
        padding: false,
        elem_id,
        elem_classes,
        height: height || void 0,
        width,
        allow_overflow: false,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(ImagePreview, "StaticImage").$$render(
            $$result,
            {
              value,
              label,
              show_label,
              show_download_button,
              selectable: _selectable,
              show_share_button,
              i18n: gradio.i18n,
              show_fullscreen_button
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
        variant: value === null ? "dashed" : "solid",
        border_mode: dragging ? "focus" : "base",
        padding: false,
        elem_id,
        elem_classes,
        height: height || void 0,
        width,
        allow_overflow: false,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(ImageUploader$1, "ImageUploader").$$render(
            $$result,
            {
              selectable: _selectable,
              root,
              sources,
              label,
              show_label,
              pending,
              streaming,
              mirror_webcam,
              stream_every,
              max_file_size: gradio.max_file_size,
              i18n: gradio.i18n,
              upload: (...args) => gradio.client.upload(...args),
              stream_handler: gradio.client?.stream,
              this: upload_component,
              uploading,
              active_source,
              value,
              dragging,
              modify_stream: _modify_stream,
              set_time_limit
            },
            {
              this: ($$value) => {
                upload_component = $$value;
                $$settled = false;
              },
              uploading: ($$value) => {
                uploading = $$value;
                $$settled = false;
              },
              active_source: ($$value) => {
                active_source = $$value;
                $$settled = false;
              },
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              },
              dragging: ($$value) => {
                dragging = $$value;
                $$settled = false;
              },
              modify_stream: ($$value) => {
                _modify_stream = $$value;
                $$settled = false;
              },
              set_time_limit: ($$value) => {
                set_time_limit = $$value;
                $$settled = false;
              }
            },
            {
              default: () => {
                return `${active_source === "upload" || !active_source ? `${validate_component(UploadText, "UploadText").$$render(
                  $$result,
                  {
                    i18n: gradio.i18n,
                    type: "image",
                    placeholder
                  },
                  {},
                  {}
                )}` : `${active_source === "clipboard" ? `${validate_component(UploadText, "UploadText").$$render(
                  $$result,
                  {
                    i18n: gradio.i18n,
                    type: "clipboard",
                    mode: "short"
                  },
                  {},
                  {}
                )}` : `${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true, size: "large" }, {}, {
                  default: () => {
                    return `${validate_component(Image, "Image").$$render($$result, {}, {}, {})}`;
                  }
                })}`}`}`;
              }
            }
          )}`;
        }
      }
    )}`}`;
  } while (!$$settled);
  return $$rendered;
});

export { Image$1 as BaseImage, ImageUploader$1 as BaseImageUploader, ImagePreview as BaseStaticImage, Webcam$1 as Webcam, Index as default };
//# sourceMappingURL=Index22-OnyqHXrF.js.map
