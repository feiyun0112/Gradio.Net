import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, b as add_attribute, e as escape, o as onDestroy, f as each } from './ssr-RaXq3SJh.js';
import { e as BlockLabel, ap as Video, g as Empty, h as IconButtonWrapper, i as IconButton, D as Download, o as ShareButton, u as uploadToHuggingFace, v as Undo, t as Play, P as Pause, q as format_time, av as Maximise, w as Trim, n as prepare_files } from './2-B6LMYTAg.js';
import { D as DownloadLink } from './DownloadLink--4obEanq.js';
import { V as Video$1 } from './Video-_epjqq1V.js';
import { M as ModifyUpload } from './ModifyUpload-CsV9IzIz.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './hls-CrxM9YLy.js';

const prettyBytes = (bytes) => {
  let units = ["B", "KB", "MB", "GB", "PB"];
  let i = 0;
  while (bytes > 1024) {
    bytes /= 1024;
    i++;
  }
  let unit = units[i];
  return bytes.toFixed(1) + " " + unit;
};
const playable = () => {
  return true;
};
function loaded(node, { autoplay }) {
  async function handle_playback() {
    if (!autoplay)
      return;
    await node.play();
  }
  node.addEventListener("loadeddata", handle_playback);
  return {
    destroy() {
      node.removeEventListener("loadeddata", handle_playback);
    }
  };
}
const css$2 = {
  code: ".load-wrap.svelte-10c4beq{display:flex;justify-content:center;align-items:center;height:100%}.loader.svelte-10c4beq{display:flex;position:relative;background-color:var(--border-color-accent-subdued);animation:svelte-10c4beq-shadowPulse 2s linear infinite;box-shadow:-24px 0 var(--border-color-accent-subdued),\n			24px 0 var(--border-color-accent-subdued);margin:var(--spacing-md);border-radius:50%;width:10px;height:10px;scale:0.5}@keyframes svelte-10c4beq-shadowPulse{33%{box-shadow:-24px 0 var(--border-color-accent-subdued),\n				24px 0 #fff;background:#fff}66%{box-shadow:-24px 0 #fff,\n				24px 0 #fff;background:var(--border-color-accent-subdued)}100%{box-shadow:-24px 0 #fff,\n				24px 0 var(--border-color-accent-subdued);background:#fff}}.container.svelte-10c4beq{display:flex;flex-direction:column;align-items:center;justify-content:center;margin:var(--spacing-lg) var(--spacing-lg) 0 var(--spacing-lg)}#timeline.svelte-10c4beq{display:flex;height:var(--size-10);flex:1;position:relative}img.svelte-10c4beq{flex:1 1 auto;min-width:0;object-fit:cover;height:var(--size-12);border:1px solid var(--block-border-color);user-select:none;z-index:1}.handle.svelte-10c4beq{width:3px;background-color:var(--color-accent);cursor:ew-resize;height:var(--size-12);z-index:3;position:absolute}.opaque-layer.svelte-10c4beq{background-color:rgba(230, 103, 40, 0.25);border:1px solid var(--color-accent);height:var(--size-12);position:absolute;z-index:2}",
  map: '{"version":3,"file":"VideoTimeline.svelte","sources":["VideoTimeline.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount, onDestroy } from \\"svelte\\";\\nexport let videoElement;\\nexport let trimmedDuration;\\nexport let dragStart;\\nexport let dragEnd;\\nexport let loadingTimeline;\\nlet thumbnails = [];\\nlet numberOfThumbnails = 10;\\nlet intervalId;\\nlet videoDuration;\\nlet leftHandlePosition = 0;\\nlet rightHandlePosition = 100;\\nlet dragging = null;\\nconst startDragging = (side) => {\\n    dragging = side;\\n};\\n$: loadingTimeline = thumbnails.length !== numberOfThumbnails;\\nconst stopDragging = () => {\\n    dragging = null;\\n};\\nconst drag = (event, distance) => {\\n    if (dragging) {\\n        const timeline = document.getElementById(\\"timeline\\");\\n        if (!timeline)\\n            return;\\n        const rect = timeline.getBoundingClientRect();\\n        let newPercentage = (event.clientX - rect.left) / rect.width * 100;\\n        if (distance) {\\n            newPercentage = dragging === \\"left\\" ? leftHandlePosition + distance : rightHandlePosition + distance;\\n        }\\n        else {\\n            newPercentage = (event.clientX - rect.left) / rect.width * 100;\\n        }\\n        newPercentage = Math.max(0, Math.min(newPercentage, 100));\\n        if (dragging === \\"left\\") {\\n            leftHandlePosition = Math.min(newPercentage, rightHandlePosition);\\n            const newTimeLeft = leftHandlePosition / 100 * videoDuration;\\n            videoElement.currentTime = newTimeLeft;\\n            dragStart = newTimeLeft;\\n        }\\n        else if (dragging === \\"right\\") {\\n            rightHandlePosition = Math.max(newPercentage, leftHandlePosition);\\n            const newTimeRight = rightHandlePosition / 100 * videoDuration;\\n            videoElement.currentTime = newTimeRight;\\n            dragEnd = newTimeRight;\\n        }\\n        const startTime = leftHandlePosition / 100 * videoDuration;\\n        const endTime = rightHandlePosition / 100 * videoDuration;\\n        trimmedDuration = endTime - startTime;\\n        leftHandlePosition = leftHandlePosition;\\n        rightHandlePosition = rightHandlePosition;\\n    }\\n};\\nconst moveHandle = (e) => {\\n    if (dragging) {\\n        const distance = 1 / videoDuration * 100;\\n        if (e.key === \\"ArrowLeft\\") {\\n            drag({ clientX: 0 }, -distance);\\n        }\\n        else if (e.key === \\"ArrowRight\\") {\\n            drag({ clientX: 0 }, distance);\\n        }\\n    }\\n};\\nconst generateThumbnail = () => {\\n    const canvas = document.createElement(\\"canvas\\");\\n    const ctx = canvas.getContext(\\"2d\\");\\n    if (!ctx)\\n        return;\\n    canvas.width = videoElement.videoWidth;\\n    canvas.height = videoElement.videoHeight;\\n    ctx.drawImage(videoElement, 0, 0, canvas.width, canvas.height);\\n    const thumbnail = canvas.toDataURL(\\"image/jpeg\\", 0.7);\\n    thumbnails = [...thumbnails, thumbnail];\\n};\\nonMount(() => {\\n    const loadMetadata = () => {\\n        videoDuration = videoElement.duration;\\n        const interval = videoDuration / numberOfThumbnails;\\n        let captures = 0;\\n        const onSeeked = () => {\\n            generateThumbnail();\\n            captures++;\\n            if (captures < numberOfThumbnails) {\\n                videoElement.currentTime += interval;\\n            }\\n            else {\\n                videoElement.removeEventListener(\\"seeked\\", onSeeked);\\n            }\\n        };\\n        videoElement.addEventListener(\\"seeked\\", onSeeked);\\n        videoElement.currentTime = 0;\\n    };\\n    if (videoElement.readyState >= 1) {\\n        loadMetadata();\\n    }\\n    else {\\n        videoElement.addEventListener(\\"loadedmetadata\\", loadMetadata);\\n    }\\n});\\nonDestroy(() => {\\n    window.removeEventListener(\\"mousemove\\", drag);\\n    window.removeEventListener(\\"mouseup\\", stopDragging);\\n    window.removeEventListener(\\"keydown\\", moveHandle);\\n    if (intervalId !== void 0) {\\n        clearInterval(intervalId);\\n    }\\n});\\nonMount(() => {\\n    window.addEventListener(\\"mousemove\\", drag);\\n    window.addEventListener(\\"mouseup\\", stopDragging);\\n    window.addEventListener(\\"keydown\\", moveHandle);\\n});\\n<\/script>\\n\\n<div class=\\"container\\">\\n\\t{#if loadingTimeline}\\n\\t\\t<div class=\\"load-wrap\\">\\n\\t\\t\\t<span aria-label=\\"loading timeline\\" class=\\"loader\\" />\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t<div id=\\"timeline\\" class=\\"thumbnail-wrapper\\">\\n\\t\\t\\t<button\\n\\t\\t\\t\\taria-label=\\"start drag handle for trimming video\\"\\n\\t\\t\\t\\tclass=\\"handle left\\"\\n\\t\\t\\t\\ton:mousedown={() => startDragging(\\"left\\")}\\n\\t\\t\\t\\ton:blur={stopDragging}\\n\\t\\t\\t\\ton:keydown={(e) => {\\n\\t\\t\\t\\t\\tif (e.key === \\"ArrowLeft\\" || e.key == \\"ArrowRight\\") {\\n\\t\\t\\t\\t\\t\\tstartDragging(\\"left\\");\\n\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t}}\\n\\t\\t\\t\\tstyle=\\"left: {leftHandlePosition}%;\\"\\n\\t\\t\\t/>\\n\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"opaque-layer\\"\\n\\t\\t\\t\\tstyle=\\"left: {leftHandlePosition}%; right: {100 - rightHandlePosition}%\\"\\n\\t\\t\\t/>\\n\\n\\t\\t\\t{#each thumbnails as thumbnail, i (i)}\\n\\t\\t\\t\\t<img src={thumbnail} alt={`frame-${i}`} draggable=\\"false\\" />\\n\\t\\t\\t{/each}\\n\\t\\t\\t<button\\n\\t\\t\\t\\taria-label=\\"end drag handle for trimming video\\"\\n\\t\\t\\t\\tclass=\\"handle right\\"\\n\\t\\t\\t\\ton:mousedown={() => startDragging(\\"right\\")}\\n\\t\\t\\t\\ton:blur={stopDragging}\\n\\t\\t\\t\\ton:keydown={(e) => {\\n\\t\\t\\t\\t\\tif (e.key === \\"ArrowLeft\\" || e.key == \\"ArrowRight\\") {\\n\\t\\t\\t\\t\\t\\tstartDragging(\\"right\\");\\n\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t}}\\n\\t\\t\\t\\tstyle=\\"left: {rightHandlePosition}%;\\"\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.load-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\theight: 100%;\\n\\t}\\n\\t.loader {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tbackground-color: var(--border-color-accent-subdued);\\n\\t\\tanimation: shadowPulse 2s linear infinite;\\n\\t\\tbox-shadow:\\n\\t\\t\\t-24px 0 var(--border-color-accent-subdued),\\n\\t\\t\\t24px 0 var(--border-color-accent-subdued);\\n\\t\\tmargin: var(--spacing-md);\\n\\t\\tborder-radius: 50%;\\n\\t\\twidth: 10px;\\n\\t\\theight: 10px;\\n\\t\\tscale: 0.5;\\n\\t}\\n\\n\\t@keyframes shadowPulse {\\n\\t\\t33% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 var(--border-color-accent-subdued),\\n\\t\\t\\t\\t24px 0 #fff;\\n\\t\\t\\tbackground: #fff;\\n\\t\\t}\\n\\t\\t66% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 #fff,\\n\\t\\t\\t\\t24px 0 #fff;\\n\\t\\t\\tbackground: var(--border-color-accent-subdued);\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 #fff,\\n\\t\\t\\t\\t24px 0 var(--border-color-accent-subdued);\\n\\t\\t\\tbackground: #fff;\\n\\t\\t}\\n\\t}\\n\\n\\t.container {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\tmargin: var(--spacing-lg) var(--spacing-lg) 0 var(--spacing-lg);\\n\\t}\\n\\n\\t#timeline {\\n\\t\\tdisplay: flex;\\n\\t\\theight: var(--size-10);\\n\\t\\tflex: 1;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\timg {\\n\\t\\tflex: 1 1 auto;\\n\\t\\tmin-width: 0;\\n\\t\\tobject-fit: cover;\\n\\t\\theight: var(--size-12);\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tuser-select: none;\\n\\t\\tz-index: 1;\\n\\t}\\n\\n\\t.handle {\\n\\t\\twidth: 3px;\\n\\t\\tbackground-color: var(--color-accent);\\n\\t\\tcursor: ew-resize;\\n\\t\\theight: var(--size-12);\\n\\t\\tz-index: 3;\\n\\t\\tposition: absolute;\\n\\t}\\n\\n\\t.opaque-layer {\\n\\t\\tbackground-color: rgba(230, 103, 40, 0.25);\\n\\t\\tborder: 1px solid var(--color-accent);\\n\\t\\theight: var(--size-12);\\n\\t\\tposition: absolute;\\n\\t\\tz-index: 2;\\n\\t}</style>\\n"],"names":[],"mappings":"AAgKC,yBAAW,CACV,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,IACT,CACA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,gBAAgB,CAAE,IAAI,6BAA6B,CAAC,CACpD,SAAS,CAAE,0BAAW,CAAC,EAAE,CAAC,MAAM,CAAC,QAAQ,CACzC,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAAC;AAC9C,GAAG,IAAI,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAC1C,MAAM,CAAE,IAAI,YAAY,CAAC,CACzB,aAAa,CAAE,GAAG,CAClB,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,GACR,CAEA,WAAW,0BAAY,CACtB,GAAI,CACH,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAAC;AAC/C,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CACZ,UAAU,CAAE,IACb,CACA,GAAI,CACH,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,CAAC;AACjB,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CACZ,UAAU,CAAE,IAAI,6BAA6B,CAC9C,CACA,IAAK,CACJ,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,CAAC;AACjB,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAC1C,UAAU,CAAE,IACb,CACD,CAEA,yBAAW,CACV,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CACvB,MAAM,CAAE,IAAI,YAAY,CAAC,CAAC,IAAI,YAAY,CAAC,CAAC,CAAC,CAAC,IAAI,YAAY,CAC/D,CAEA,wBAAU,CACT,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,IAAI,CAAE,CAAC,CACP,QAAQ,CAAE,QACX,CAEA,kBAAI,CACH,IAAI,CAAE,CAAC,CAAC,CAAC,CAAC,IAAI,CACd,SAAS,CAAE,CAAC,CACZ,UAAU,CAAE,KAAK,CACjB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CAC3C,WAAW,CAAE,IAAI,CACjB,OAAO,CAAE,CACV,CAEA,sBAAQ,CACP,KAAK,CAAE,GAAG,CACV,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,MAAM,CAAE,SAAS,CACjB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,OAAO,CAAE,CAAC,CACV,QAAQ,CAAE,QACX,CAEA,4BAAc,CACb,gBAAgB,CAAE,KAAK,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,EAAE,CAAC,CAAC,IAAI,CAAC,CAC1C,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,cAAc,CAAC,CACrC,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,QAAQ,CAAE,QAAQ,CAClB,OAAO,CAAE,CACV"}'
};
let numberOfThumbnails = 10;
const VideoTimeline = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { videoElement } = $$props;
  let { trimmedDuration } = $$props;
  let { dragStart } = $$props;
  let { dragEnd } = $$props;
  let { loadingTimeline } = $$props;
  let thumbnails = [];
  let videoDuration;
  let leftHandlePosition = 0;
  let rightHandlePosition = 100;
  let dragging = null;
  const stopDragging = () => {
    dragging = null;
  };
  const drag = (event, distance) => {
    if (dragging) {
      const timeline = document.getElementById("timeline");
      if (!timeline)
        return;
      const rect = timeline.getBoundingClientRect();
      let newPercentage = (event.clientX - rect.left) / rect.width * 100;
      if (distance) {
        newPercentage = dragging === "left" ? leftHandlePosition + distance : rightHandlePosition + distance;
      } else {
        newPercentage = (event.clientX - rect.left) / rect.width * 100;
      }
      newPercentage = Math.max(0, Math.min(newPercentage, 100));
      if (dragging === "left") {
        leftHandlePosition = Math.min(newPercentage, rightHandlePosition);
        const newTimeLeft = leftHandlePosition / 100 * videoDuration;
        videoElement.currentTime = newTimeLeft;
        dragStart = newTimeLeft;
      } else if (dragging === "right") {
        rightHandlePosition = Math.max(newPercentage, leftHandlePosition);
        const newTimeRight = rightHandlePosition / 100 * videoDuration;
        videoElement.currentTime = newTimeRight;
        dragEnd = newTimeRight;
      }
      const startTime = leftHandlePosition / 100 * videoDuration;
      const endTime = rightHandlePosition / 100 * videoDuration;
      trimmedDuration = endTime - startTime;
      leftHandlePosition = leftHandlePosition;
      rightHandlePosition = rightHandlePosition;
    }
  };
  const moveHandle = (e) => {
    if (dragging) {
      const distance = 1 / videoDuration * 100;
      if (e.key === "ArrowLeft") {
        drag({ clientX: 0 }, -distance);
      } else if (e.key === "ArrowRight") {
        drag({ clientX: 0 }, distance);
      }
    }
  };
  onDestroy(() => {
    window.removeEventListener("mousemove", drag);
    window.removeEventListener("mouseup", stopDragging);
    window.removeEventListener("keydown", moveHandle);
  });
  if ($$props.videoElement === void 0 && $$bindings.videoElement && videoElement !== void 0)
    $$bindings.videoElement(videoElement);
  if ($$props.trimmedDuration === void 0 && $$bindings.trimmedDuration && trimmedDuration !== void 0)
    $$bindings.trimmedDuration(trimmedDuration);
  if ($$props.dragStart === void 0 && $$bindings.dragStart && dragStart !== void 0)
    $$bindings.dragStart(dragStart);
  if ($$props.dragEnd === void 0 && $$bindings.dragEnd && dragEnd !== void 0)
    $$bindings.dragEnd(dragEnd);
  if ($$props.loadingTimeline === void 0 && $$bindings.loadingTimeline && loadingTimeline !== void 0)
    $$bindings.loadingTimeline(loadingTimeline);
  $$result.css.add(css$2);
  loadingTimeline = thumbnails.length !== numberOfThumbnails;
  return `<div class="container svelte-10c4beq">${loadingTimeline ? `<div class="load-wrap svelte-10c4beq" data-svelte-h="svelte-13yzice"><span aria-label="loading timeline" class="loader svelte-10c4beq"></span></div>` : `<div id="timeline" class="thumbnail-wrapper svelte-10c4beq"><button aria-label="start drag handle for trimming video" class="handle left svelte-10c4beq" style="${"left: " + escape(leftHandlePosition, true) + "%;"}"></button> <div class="opaque-layer svelte-10c4beq" style="${"left: " + escape(leftHandlePosition, true) + "%; right: " + escape(100 - rightHandlePosition, true) + "%"}"></div> ${each(thumbnails, (thumbnail, i) => {
    return `<img${add_attribute("src", thumbnail, 0)}${add_attribute("alt", `frame-${i}`, 0)} draggable="false" class="svelte-10c4beq">`;
  })} <button aria-label="end drag handle for trimming video" class="handle right svelte-10c4beq" style="${"left: " + escape(rightHandlePosition, true) + "%;"}"></button></div>`} </div>`;
});
const css$1 = {
  code: ".container.svelte-7yrr5f.svelte-7yrr5f{width:100%}time.svelte-7yrr5f.svelte-7yrr5f{color:var(--color-accent);font-weight:bold;padding-left:var(--spacing-xs)}.timeline-wrapper.svelte-7yrr5f.svelte-7yrr5f{display:flex;align-items:center;justify-content:center;width:100%}.text-button.svelte-7yrr5f.svelte-7yrr5f{border:1px solid var(--neutral-400);border-radius:var(--radius-sm);font-weight:300;font-size:var(--size-3);text-align:center;color:var(--neutral-400);height:var(--size-5);font-weight:bold;padding:0 5px;margin-left:5px}.text-button.svelte-7yrr5f.svelte-7yrr5f:hover,.text-button.svelte-7yrr5f.svelte-7yrr5f:focus{color:var(--color-accent);border-color:var(--color-accent)}.controls.svelte-7yrr5f.svelte-7yrr5f{display:flex;justify-content:space-between;align-items:center;margin:var(--spacing-lg);overflow:hidden}.edit-buttons.svelte-7yrr5f.svelte-7yrr5f{display:flex;gap:var(--spacing-sm)}@media(max-width: 320px){.controls.svelte-7yrr5f.svelte-7yrr5f{flex-direction:column;align-items:flex-start}.edit-buttons.svelte-7yrr5f.svelte-7yrr5f{margin-top:var(--spacing-sm)}.controls.svelte-7yrr5f .svelte-7yrr5f{margin:var(--spacing-sm)}.controls.svelte-7yrr5f .text-button.svelte-7yrr5f{margin-left:0}}.container.svelte-7yrr5f.svelte-7yrr5f{display:flex;flex-direction:column}.hidden.svelte-7yrr5f.svelte-7yrr5f{display:none}",
  map: '{"version":3,"file":"VideoControls.svelte","sources":["VideoControls.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { Undo, Trim, Clear } from \\"@gradio/icons\\";\\nimport VideoTimeline from \\"./VideoTimeline.svelte\\";\\nimport { trimVideo } from \\"./utils\\";\\nimport { FFmpeg } from \\"@ffmpeg/ffmpeg\\";\\nimport loadFfmpeg from \\"./utils\\";\\nimport { onMount } from \\"svelte\\";\\nimport { format_time } from \\"@gradio/utils\\";\\nimport { IconButton } from \\"@gradio/atoms\\";\\nimport { ModifyUpload } from \\"@gradio/upload\\";\\nexport let videoElement;\\nexport let showRedo = false;\\nexport let interactive = true;\\nexport let mode = \\"\\";\\nexport let handle_reset_value;\\nexport let handle_trim_video;\\nexport let processingVideo = false;\\nexport let i18n;\\nexport let value = null;\\nexport let show_download_button = false;\\nexport let handle_clear = () => {\\n};\\nexport let has_change_history = false;\\nlet ffmpeg;\\nonMount(async () => {\\n    ffmpeg = await loadFfmpeg();\\n});\\n$: if (mode === \\"edit\\" && trimmedDuration === null && videoElement)\\n    trimmedDuration = videoElement.duration;\\nlet trimmedDuration = null;\\nlet dragStart = 0;\\nlet dragEnd = 0;\\nlet loadingTimeline = false;\\nconst toggleTrimmingMode = () => {\\n    if (mode === \\"edit\\") {\\n        mode = \\"\\";\\n        trimmedDuration = videoElement.duration;\\n    }\\n    else {\\n        mode = \\"edit\\";\\n    }\\n};\\n<\/script>\\n\\n<div class=\\"container\\" class:hidden={mode !== \\"edit\\"}>\\n\\t{#if mode === \\"edit\\"}\\n\\t\\t<div class=\\"timeline-wrapper\\">\\n\\t\\t\\t<VideoTimeline\\n\\t\\t\\t\\t{videoElement}\\n\\t\\t\\t\\tbind:dragStart\\n\\t\\t\\t\\tbind:dragEnd\\n\\t\\t\\t\\tbind:trimmedDuration\\n\\t\\t\\t\\tbind:loadingTimeline\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\t{/if}\\n\\n\\t<div class=\\"controls\\" data-testid=\\"waveform-controls\\">\\n\\t\\t{#if mode === \\"edit\\" && trimmedDuration !== null}\\n\\t\\t\\t<time\\n\\t\\t\\t\\taria-label=\\"duration of selected region in seconds\\"\\n\\t\\t\\t\\tclass:hidden={loadingTimeline}>{format_time(trimmedDuration)}</time\\n\\t\\t\\t>\\n\\t\\t\\t<div class=\\"edit-buttons\\">\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass:hidden={loadingTimeline}\\n\\t\\t\\t\\t\\tclass=\\"text-button\\"\\n\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\tmode = \\"\\";\\n\\t\\t\\t\\t\\t\\tprocessingVideo = true;\\n\\t\\t\\t\\t\\t\\ttrimVideo(ffmpeg, dragStart, dragEnd, videoElement)\\n\\t\\t\\t\\t\\t\\t\\t.then((videoBlob) => {\\n\\t\\t\\t\\t\\t\\t\\t\\thandle_trim_video(videoBlob);\\n\\t\\t\\t\\t\\t\\t\\t})\\n\\t\\t\\t\\t\\t\\t\\t.then(() => {\\n\\t\\t\\t\\t\\t\\t\\t\\tprocessingVideo = false;\\n\\t\\t\\t\\t\\t\\t\\t});\\n\\t\\t\\t\\t\\t}}>Trim</button\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"text-button\\"\\n\\t\\t\\t\\t\\tclass:hidden={loadingTimeline}\\n\\t\\t\\t\\t\\ton:click={toggleTrimmingMode}>Cancel</button\\n\\t\\t\\t\\t>\\n\\t\\t\\t</div>\\n\\t\\t{:else}\\n\\t\\t\\t<div />\\n\\t\\t{/if}\\n\\t</div>\\n</div>\\n\\n<ModifyUpload\\n\\t{i18n}\\n\\ton:clear={() => handle_clear()}\\n\\tdownload={show_download_button ? value?.url : null}\\n>\\n\\t{#if showRedo && mode === \\"\\"}\\n\\t\\t<IconButton\\n\\t\\t\\tIcon={Undo}\\n\\t\\t\\tlabel=\\"Reset video to initial value\\"\\n\\t\\t\\tdisabled={processingVideo || !has_change_history}\\n\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\thandle_reset_value();\\n\\t\\t\\t\\tmode = \\"\\";\\n\\t\\t\\t}}\\n\\t\\t/>\\n\\t{/if}\\n\\n\\t{#if interactive && mode === \\"\\"}\\n\\t\\t<IconButton\\n\\t\\t\\tIcon={Trim}\\n\\t\\t\\tlabel=\\"Trim video to selection\\"\\n\\t\\t\\tdisabled={processingVideo}\\n\\t\\t\\ton:click={toggleTrimmingMode}\\n\\t\\t/>\\n\\t{/if}\\n</ModifyUpload>\\n\\n<style>\\n\\t.container {\\n\\t\\twidth: 100%;\\n\\t}\\n\\ttime {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tfont-weight: bold;\\n\\t\\tpadding-left: var(--spacing-xs);\\n\\t}\\n\\n\\t.timeline-wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t.text-button {\\n\\t\\tborder: 1px solid var(--neutral-400);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tfont-weight: 300;\\n\\t\\tfont-size: var(--size-3);\\n\\t\\ttext-align: center;\\n\\t\\tcolor: var(--neutral-400);\\n\\t\\theight: var(--size-5);\\n\\t\\tfont-weight: bold;\\n\\t\\tpadding: 0 5px;\\n\\t\\tmargin-left: 5px;\\n\\t}\\n\\n\\t.text-button:hover,\\n\\t.text-button:focus {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tborder-color: var(--color-accent);\\n\\t}\\n\\n\\t.controls {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: center;\\n\\t\\tmargin: var(--spacing-lg);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.edit-buttons {\\n\\t\\tdisplay: flex;\\n\\t\\tgap: var(--spacing-sm);\\n\\t}\\n\\n\\t@media (max-width: 320px) {\\n\\t\\t.controls {\\n\\t\\t\\tflex-direction: column;\\n\\t\\t\\talign-items: flex-start;\\n\\t\\t}\\n\\n\\t\\t.edit-buttons {\\n\\t\\t\\tmargin-top: var(--spacing-sm);\\n\\t\\t}\\n\\n\\t\\t.controls * {\\n\\t\\t\\tmargin: var(--spacing-sm);\\n\\t\\t}\\n\\n\\t\\t.controls .text-button {\\n\\t\\t\\tmargin-left: 0;\\n\\t\\t}\\n\\t}\\n\\n\\t.container {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t}\\n\\n\\t.hidden {\\n\\t\\tdisplay: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AAsHC,sCAAW,CACV,KAAK,CAAE,IACR,CACA,gCAAK,CACJ,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,WAAW,CAAE,IAAI,CACjB,YAAY,CAAE,IAAI,YAAY,CAC/B,CAEA,6CAAkB,CACjB,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CACvB,KAAK,CAAE,IACR,CAEA,wCAAa,CACZ,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,aAAa,CAAC,CACpC,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,WAAW,CAAE,GAAG,CAChB,SAAS,CAAE,IAAI,QAAQ,CAAC,CACxB,UAAU,CAAE,MAAM,CAClB,KAAK,CAAE,IAAI,aAAa,CAAC,CACzB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,WAAW,CAAE,IAAI,CACjB,OAAO,CAAE,CAAC,CAAC,GAAG,CACd,WAAW,CAAE,GACd,CAEA,wCAAY,MAAM,CAClB,wCAAY,MAAO,CAClB,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,YAAY,CAAE,IAAI,cAAc,CACjC,CAEA,qCAAU,CACT,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,IAAI,YAAY,CAAC,CACzB,QAAQ,CAAE,MACX,CAEA,yCAAc,CACb,OAAO,CAAE,IAAI,CACb,GAAG,CAAE,IAAI,YAAY,CACtB,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,qCAAU,CACT,cAAc,CAAE,MAAM,CACtB,WAAW,CAAE,UACd,CAEA,yCAAc,CACb,UAAU,CAAE,IAAI,YAAY,CAC7B,CAEA,uBAAS,CAAC,cAAE,CACX,MAAM,CAAE,IAAI,YAAY,CACzB,CAEA,uBAAS,CAAC,0BAAa,CACtB,WAAW,CAAE,CACd,CACD,CAEA,sCAAW,CACV,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MACjB,CAEA,mCAAQ,CACP,OAAO,CAAE,IACV"}'
};
const VideoControls = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { videoElement } = $$props;
  let { showRedo = false } = $$props;
  let { interactive = true } = $$props;
  let { mode = "" } = $$props;
  let { handle_reset_value } = $$props;
  let { handle_trim_video } = $$props;
  let { processingVideo = false } = $$props;
  let { i18n } = $$props;
  let { value = null } = $$props;
  let { show_download_button = false } = $$props;
  let { handle_clear = () => {
  } } = $$props;
  let { has_change_history = false } = $$props;
  let trimmedDuration = null;
  let dragStart = 0;
  let dragEnd = 0;
  let loadingTimeline = false;
  if ($$props.videoElement === void 0 && $$bindings.videoElement && videoElement !== void 0)
    $$bindings.videoElement(videoElement);
  if ($$props.showRedo === void 0 && $$bindings.showRedo && showRedo !== void 0)
    $$bindings.showRedo(showRedo);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.handle_trim_video === void 0 && $$bindings.handle_trim_video && handle_trim_video !== void 0)
    $$bindings.handle_trim_video(handle_trim_video);
  if ($$props.processingVideo === void 0 && $$bindings.processingVideo && processingVideo !== void 0)
    $$bindings.processingVideo(processingVideo);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.handle_clear === void 0 && $$bindings.handle_clear && handle_clear !== void 0)
    $$bindings.handle_clear(handle_clear);
  if ($$props.has_change_history === void 0 && $$bindings.has_change_history && has_change_history !== void 0)
    $$bindings.has_change_history(has_change_history);
  $$result.css.add(css$1);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      if (mode === "edit" && trimmedDuration === null && videoElement)
        trimmedDuration = videoElement.duration;
    }
    $$rendered = `<div class="${["container svelte-7yrr5f", mode !== "edit" ? "hidden" : ""].join(" ").trim()}">${mode === "edit" ? `<div class="timeline-wrapper svelte-7yrr5f">${validate_component(VideoTimeline, "VideoTimeline").$$render(
      $$result,
      {
        videoElement,
        dragStart,
        dragEnd,
        trimmedDuration,
        loadingTimeline
      },
      {
        dragStart: ($$value) => {
          dragStart = $$value;
          $$settled = false;
        },
        dragEnd: ($$value) => {
          dragEnd = $$value;
          $$settled = false;
        },
        trimmedDuration: ($$value) => {
          trimmedDuration = $$value;
          $$settled = false;
        },
        loadingTimeline: ($$value) => {
          loadingTimeline = $$value;
          $$settled = false;
        }
      },
      {}
    )}</div>` : ``} <div class="controls svelte-7yrr5f" data-testid="waveform-controls">${mode === "edit" && trimmedDuration !== null ? `<time aria-label="duration of selected region in seconds" class="${["svelte-7yrr5f", loadingTimeline ? "hidden" : ""].join(" ").trim()}">${escape(format_time(trimmedDuration))}</time> <div class="edit-buttons svelte-7yrr5f"><button class="${["text-button svelte-7yrr5f", loadingTimeline ? "hidden" : ""].join(" ").trim()}" data-svelte-h="svelte-18nzick">Trim</button> <button class="${["text-button svelte-7yrr5f", loadingTimeline ? "hidden" : ""].join(" ").trim()}" data-svelte-h="svelte-1mj98i4">Cancel</button></div>` : `<div class="svelte-7yrr5f"></div>`}</div></div> ${validate_component(ModifyUpload, "ModifyUpload").$$render(
      $$result,
      {
        i18n,
        download: show_download_button ? value?.url : null
      },
      {},
      {
        default: () => {
          return `${showRedo && mode === "" ? `${validate_component(IconButton, "IconButton").$$render(
            $$result,
            {
              Icon: Undo,
              label: "Reset video to initial value",
              disabled: processingVideo || !has_change_history
            },
            {},
            {}
          )}` : ``} ${interactive && mode === "" ? `${validate_component(IconButton, "IconButton").$$render(
            $$result,
            {
              Icon: Trim,
              label: "Trim video to selection",
              disabled: processingVideo
            },
            {},
            {}
          )}` : ``}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});
const css = {
  code: "span.svelte-euo1cw.svelte-euo1cw{text-shadow:0 0 8px rgba(0, 0, 0, 0.5)}progress.svelte-euo1cw.svelte-euo1cw{margin-right:var(--size-3);border-radius:var(--radius-sm);width:var(--size-full);height:var(--size-2)}progress.svelte-euo1cw.svelte-euo1cw::-webkit-progress-bar{border-radius:2px;background-color:rgba(255, 255, 255, 0.2);overflow:hidden}progress.svelte-euo1cw.svelte-euo1cw::-webkit-progress-value{background-color:rgba(255, 255, 255, 0.9)}.mirror.svelte-euo1cw.svelte-euo1cw{transform:scaleX(-1)}.mirror-wrap.svelte-euo1cw.svelte-euo1cw{position:relative;height:100%;width:100%}.controls.svelte-euo1cw.svelte-euo1cw{position:absolute;bottom:0;opacity:0;transition:500ms;margin:var(--size-2);border-radius:var(--radius-md);background:var(--color-grey-800);padding:var(--size-2) var(--size-1);width:calc(100% - 0.375rem * 2);width:calc(100% - var(--size-2) * 2)}.wrap.svelte-euo1cw:hover .controls.svelte-euo1cw{opacity:1}.inner.svelte-euo1cw.svelte-euo1cw{display:flex;justify-content:space-between;align-items:center;padding-right:var(--size-2);padding-left:var(--size-2);width:var(--size-full);height:var(--size-full)}.icon.svelte-euo1cw.svelte-euo1cw{display:flex;justify-content:center;cursor:pointer;width:var(--size-6);color:white}.time.svelte-euo1cw.svelte-euo1cw{flex-shrink:0;margin-right:var(--size-3);margin-left:var(--size-3);color:white;font-size:var(--text-sm);font-family:var(--font-mono)}.wrap.svelte-euo1cw.svelte-euo1cw{position:relative;background-color:var(--background-fill-secondary);height:var(--size-full);width:var(--size-full);border-radius:var(--radius-xl)}.wrap.svelte-euo1cw video{height:var(--size-full);width:var(--size-full)}",
  map: '{"version":3,"file":"Player.svelte","sources":["Player.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport { Play, Pause, Maximise, Undo } from \\"@gradio/icons\\";\\nimport Video from \\"./Video.svelte\\";\\nimport VideoControls from \\"./VideoControls.svelte\\";\\nimport { prepare_files } from \\"@gradio/client\\";\\nimport { format_time } from \\"@gradio/utils\\";\\nexport let root = \\"\\";\\nexport let src;\\nexport let subtitle = null;\\nexport let mirror;\\nexport let autoplay;\\nexport let loop;\\nexport let label = \\"test\\";\\nexport let interactive = false;\\nexport let handle_change = () => {\\n};\\nexport let handle_reset_value = () => {\\n};\\nexport let upload;\\nexport let is_stream;\\nexport let i18n;\\nexport let show_download_button = false;\\nexport let value = null;\\nexport let handle_clear = () => {\\n};\\nexport let has_change_history = false;\\nconst dispatch = createEventDispatcher();\\nlet time = 0;\\nlet duration;\\nlet paused = true;\\nlet video;\\nlet processingVideo = false;\\nfunction handleMove(e) {\\n    if (!duration)\\n        return;\\n    if (e.type === \\"click\\") {\\n        handle_click(e);\\n        return;\\n    }\\n    if (e.type !== \\"touchmove\\" && !(e.buttons & 1))\\n        return;\\n    const clientX = e.type === \\"touchmove\\" ? e.touches[0].clientX : e.clientX;\\n    const { left, right } = e.currentTarget.getBoundingClientRect();\\n    time = duration * (clientX - left) / (right - left);\\n}\\nasync function play_pause() {\\n    if (document.fullscreenElement != video) {\\n        const isPlaying = video.currentTime > 0 && !video.paused && !video.ended && video.readyState > video.HAVE_CURRENT_DATA;\\n        if (!isPlaying) {\\n            await video.play();\\n        }\\n        else\\n            video.pause();\\n    }\\n}\\nfunction handle_click(e) {\\n    const { left, right } = e.currentTarget.getBoundingClientRect();\\n    time = duration * (e.clientX - left) / (right - left);\\n}\\nfunction handle_end() {\\n    dispatch(\\"stop\\");\\n    dispatch(\\"end\\");\\n}\\nconst handle_trim_video = async (videoBlob) => {\\n    let _video_blob = new File([videoBlob], \\"video.mp4\\");\\n    const val = await prepare_files([_video_blob]);\\n    let value2 = ((await upload(val, root))?.filter(Boolean))[0];\\n    handle_change(value2);\\n};\\nfunction open_full_screen() {\\n    video.requestFullscreen();\\n}\\n<\/script>\\n\\n<div class=\\"wrap\\">\\n\\t<div class=\\"mirror-wrap\\" class:mirror>\\n\\t\\t<Video\\n\\t\\t\\t{src}\\n\\t\\t\\tpreload=\\"auto\\"\\n\\t\\t\\t{autoplay}\\n\\t\\t\\t{loop}\\n\\t\\t\\t{is_stream}\\n\\t\\t\\ton:click={play_pause}\\n\\t\\t\\ton:play\\n\\t\\t\\ton:pause\\n\\t\\t\\ton:ended={handle_end}\\n\\t\\t\\tbind:currentTime={time}\\n\\t\\t\\tbind:duration\\n\\t\\t\\tbind:paused\\n\\t\\t\\tbind:node={video}\\n\\t\\t\\tdata-testid={`${label}-player`}\\n\\t\\t\\t{processingVideo}\\n\\t\\t\\ton:loadstart\\n\\t\\t\\ton:loadeddata\\n\\t\\t\\ton:loadedmetadata\\n\\t\\t>\\n\\t\\t\\t<track kind=\\"captions\\" src={subtitle} default />\\n\\t\\t</Video>\\n\\t</div>\\n\\n\\t<div class=\\"controls\\">\\n\\t\\t<div class=\\"inner\\">\\n\\t\\t\\t<span\\n\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\tclass=\\"icon\\"\\n\\t\\t\\t\\taria-label=\\"play-pause-replay-button\\"\\n\\t\\t\\t\\ton:click={play_pause}\\n\\t\\t\\t\\ton:keydown={play_pause}\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#if time === duration}\\n\\t\\t\\t\\t\\t<Undo />\\n\\t\\t\\t\\t{:else if paused}\\n\\t\\t\\t\\t\\t<Play />\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<Pause />\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</span>\\n\\n\\t\\t\\t<span class=\\"time\\">{format_time(time)} / {format_time(duration)}</span>\\n\\n\\t\\t\\t<!-- TODO: implement accessible video timeline for 4.0 -->\\n\\t\\t\\t<!-- svelte-ignore a11y-click-events-have-key-events -->\\n\\t\\t\\t<!-- svelte-ignore a11y-no-noninteractive-element-interactions -->\\n\\t\\t\\t<progress\\n\\t\\t\\t\\tvalue={time / duration || 0}\\n\\t\\t\\t\\ton:mousemove={handleMove}\\n\\t\\t\\t\\ton:touchmove|preventDefault={handleMove}\\n\\t\\t\\t\\ton:click|stopPropagation|preventDefault={handle_click}\\n\\t\\t\\t/>\\n\\n\\t\\t\\t<div\\n\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\tclass=\\"icon\\"\\n\\t\\t\\t\\taria-label=\\"full-screen\\"\\n\\t\\t\\t\\ton:click={open_full_screen}\\n\\t\\t\\t\\ton:keypress={open_full_screen}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<Maximise />\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t</div>\\n</div>\\n{#if interactive}\\n\\t<VideoControls\\n\\t\\tvideoElement={video}\\n\\t\\tshowRedo\\n\\t\\t{handle_trim_video}\\n\\t\\t{handle_reset_value}\\n\\t\\tbind:processingVideo\\n\\t\\t{value}\\n\\t\\t{i18n}\\n\\t\\t{show_download_button}\\n\\t\\t{handle_clear}\\n\\t\\t{has_change_history}\\n\\t/>\\n{/if}\\n\\n<style lang=\\"postcss\\">\\n\\tspan {\\n\\t\\ttext-shadow: 0 0 8px rgba(0, 0, 0, 0.5);\\n\\t}\\n\\n\\tprogress {\\n\\t\\tmargin-right: var(--size-3);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-2);\\n\\t}\\n\\n\\tprogress::-webkit-progress-bar {\\n\\t\\tborder-radius: 2px;\\n\\t\\tbackground-color: rgba(255, 255, 255, 0.2);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\tprogress::-webkit-progress-value {\\n\\t\\tbackground-color: rgba(255, 255, 255, 0.9);\\n\\t}\\n\\n\\t.mirror {\\n\\t\\ttransform: scaleX(-1);\\n\\t}\\n\\n\\t.mirror-wrap {\\n\\t\\tposition: relative;\\n\\t\\theight: 100%;\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t.controls {\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 0;\\n\\t\\topacity: 0;\\n\\t\\ttransition: 500ms;\\n\\t\\tmargin: var(--size-2);\\n\\t\\tborder-radius: var(--radius-md);\\n\\t\\tbackground: var(--color-grey-800);\\n\\t\\tpadding: var(--size-2) var(--size-1);\\n\\t\\twidth: calc(100% - 0.375rem * 2);\\n\\t\\twidth: calc(100% - var(--size-2) * 2);\\n\\t}\\n\\t.wrap:hover .controls {\\n\\t\\topacity: 1;\\n\\t}\\n\\n\\t.inner {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: center;\\n\\t\\tpadding-right: var(--size-2);\\n\\t\\tpadding-left: var(--size-2);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\n\\t.icon {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\tcursor: pointer;\\n\\t\\twidth: var(--size-6);\\n\\t\\tcolor: white;\\n\\t}\\n\\n\\t.time {\\n\\t\\tflex-shrink: 0;\\n\\t\\tmargin-right: var(--size-3);\\n\\t\\tmargin-left: var(--size-3);\\n\\t\\tcolor: white;\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\tfont-family: var(--font-mono);\\n\\t}\\n\\t.wrap {\\n\\t\\tposition: relative;\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\theight: var(--size-full);\\n\\t\\twidth: var(--size-full);\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t}\\n\\t.wrap :global(video) {\\n\\t\\theight: var(--size-full);\\n\\t\\twidth: var(--size-full);\\n\\t}</style>\\n"],"names":[],"mappings":"AAgKC,gCAAK,CACJ,WAAW,CAAE,CAAC,CAAC,CAAC,CAAC,GAAG,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,GAAG,CACvC,CAEA,oCAAS,CACR,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,QAAQ,CACrB,CAEA,oCAAQ,sBAAuB,CAC9B,aAAa,CAAE,GAAG,CAClB,gBAAgB,CAAE,KAAK,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAAC,CAC1C,QAAQ,CAAE,MACX,CAEA,oCAAQ,wBAAyB,CAChC,gBAAgB,CAAE,KAAK,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAC1C,CAEA,mCAAQ,CACP,SAAS,CAAE,OAAO,EAAE,CACrB,CAEA,wCAAa,CACZ,QAAQ,CAAE,QAAQ,CAClB,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,IACR,CAEA,qCAAU,CACT,QAAQ,CAAE,QAAQ,CAClB,MAAM,CAAE,CAAC,CACT,OAAO,CAAE,CAAC,CACV,UAAU,CAAE,KAAK,CACjB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE,IAAI,gBAAgB,CAAC,CACjC,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,KAAK,CAAE,KAAK,IAAI,CAAC,CAAC,CAAC,QAAQ,CAAC,CAAC,CAAC,CAAC,CAAC,CAChC,KAAK,CAAE,KAAK,IAAI,CAAC,CAAC,CAAC,IAAI,QAAQ,CAAC,CAAC,CAAC,CAAC,CAAC,CACrC,CACA,mBAAK,MAAM,CAAC,uBAAU,CACrB,OAAO,CAAE,CACV,CAEA,kCAAO,CACN,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,WAAW,CAAE,MAAM,CACnB,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CACxB,CAEA,iCAAM,CACL,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,KAAK,CAAE,KACR,CAEA,iCAAM,CACL,WAAW,CAAE,CAAC,CACd,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,WAAW,CAAE,IAAI,QAAQ,CAAC,CAC1B,KAAK,CAAE,KAAK,CACZ,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,WAAW,CAAE,IAAI,WAAW,CAC7B,CACA,iCAAM,CACL,QAAQ,CAAE,QAAQ,CAClB,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,aAAa,CAAE,IAAI,WAAW,CAC/B,CACA,mBAAK,CAAS,KAAO,CACpB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,KAAK,CAAE,IAAI,WAAW,CACvB"}'
};
const Player = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { root = "" } = $$props;
  let { src } = $$props;
  let { subtitle = null } = $$props;
  let { mirror } = $$props;
  let { autoplay } = $$props;
  let { loop } = $$props;
  let { label = "test" } = $$props;
  let { interactive = false } = $$props;
  let { handle_change = () => {
  } } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let { upload } = $$props;
  let { is_stream } = $$props;
  let { i18n } = $$props;
  let { show_download_button = false } = $$props;
  let { value = null } = $$props;
  let { handle_clear = () => {
  } } = $$props;
  let { has_change_history = false } = $$props;
  createEventDispatcher();
  let time = 0;
  let duration;
  let paused = true;
  let video;
  let processingVideo = false;
  const handle_trim_video = async (videoBlob) => {
    let _video_blob = new File([videoBlob], "video.mp4");
    const val = await prepare_files([_video_blob]);
    let value2 = (await upload(val, root))?.filter(Boolean)[0];
    handle_change(value2);
  };
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.src === void 0 && $$bindings.src && src !== void 0)
    $$bindings.src(src);
  if ($$props.subtitle === void 0 && $$bindings.subtitle && subtitle !== void 0)
    $$bindings.subtitle(subtitle);
  if ($$props.mirror === void 0 && $$bindings.mirror && mirror !== void 0)
    $$bindings.mirror(mirror);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.handle_change === void 0 && $$bindings.handle_change && handle_change !== void 0)
    $$bindings.handle_change(handle_change);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.is_stream === void 0 && $$bindings.is_stream && is_stream !== void 0)
    $$bindings.is_stream(is_stream);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.handle_clear === void 0 && $$bindings.handle_clear && handle_clear !== void 0)
    $$bindings.handle_clear(handle_clear);
  if ($$props.has_change_history === void 0 && $$bindings.has_change_history && has_change_history !== void 0)
    $$bindings.has_change_history(has_change_history);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `<div class="wrap svelte-euo1cw"><div class="${["mirror-wrap svelte-euo1cw", mirror ? "mirror" : ""].join(" ").trim()}">${validate_component(Video$1, "Video").$$render(
      $$result,
      {
        src,
        preload: "auto",
        autoplay,
        loop,
        is_stream,
        "data-testid": `${label}-player`,
        processingVideo,
        currentTime: time,
        duration,
        paused,
        node: video
      },
      {
        currentTime: ($$value) => {
          time = $$value;
          $$settled = false;
        },
        duration: ($$value) => {
          duration = $$value;
          $$settled = false;
        },
        paused: ($$value) => {
          paused = $$value;
          $$settled = false;
        },
        node: ($$value) => {
          video = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `<track kind="captions"${add_attribute("src", subtitle, 0)} default>`;
        }
      }
    )}</div> <div class="controls svelte-euo1cw"><div class="inner svelte-euo1cw"><span role="button" tabindex="0" class="icon svelte-euo1cw" aria-label="play-pause-replay-button">${time === duration ? `${validate_component(Undo, "Undo").$$render($$result, {}, {}, {})}` : `${paused ? `${validate_component(Play, "Play").$$render($$result, {}, {}, {})}` : `${validate_component(Pause, "Pause").$$render($$result, {}, {}, {})}`}`}</span> <span class="time svelte-euo1cw">${escape(format_time(time))} / ${escape(format_time(duration))}</span>    <progress${add_attribute("value", time / duration || 0, 0)} class="svelte-euo1cw"></progress> <div role="button" tabindex="0" class="icon svelte-euo1cw" aria-label="full-screen">${validate_component(Maximise, "Maximise").$$render($$result, {}, {}, {})}</div></div></div></div> ${interactive ? `${validate_component(VideoControls, "VideoControls").$$render(
      $$result,
      {
        videoElement: video,
        showRedo: true,
        handle_trim_video,
        handle_reset_value,
        value,
        i18n,
        show_download_button,
        handle_clear,
        has_change_history,
        processingVideo
      },
      {
        processingVideo: ($$value) => {
          processingVideo = $$value;
          $$settled = false;
        }
      },
      {}
    )}` : ``}`;
  } while (!$$settled);
  return $$rendered;
});
const Player$1 = Player;
const VideoPreview = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value = null } = $$props;
  let { subtitle = null } = $$props;
  let { label = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { autoplay } = $$props;
  let { show_share_button = true } = $$props;
  let { show_download_button = true } = $$props;
  let { loop } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.subtitle === void 0 && $$bindings.subtitle && subtitle !== void 0)
    $$bindings.subtitle(subtitle);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  value && dispatch("change", value);
  return `${validate_component(BlockLabel, "BlockLabel").$$render(
    $$result,
    {
      show_label,
      Icon: Video,
      label: label || "Video"
    },
    {},
    {}
  )} ${!value || value.url === void 0 ? `${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true, size: "large" }, {}, {
    default: () => {
      return `${validate_component(Video, "Video").$$render($$result, {}, {}, {})}`;
    }
  })}` : `${validate_component(Player$1, "Player").$$render(
    $$result,
    {
      src: value.url,
      subtitle: subtitle?.url,
      is_stream: value.is_stream,
      autoplay,
      mirror: false,
      label,
      loop,
      interactive: false,
      upload,
      i18n
    },
    {},
    {}
  )} <div data-testid="download-div">${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${show_download_button ? `${validate_component(DownloadLink, "DownloadLink").$$render(
        $$result,
        {
          href: value.is_stream ? value.url?.replace("playlist.m3u8", "playlist-file") : value.url,
          download: value.orig_name || value.path
        },
        {},
        {
          default: () => {
            return `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Download, label: "Download" }, {}, {})}`;
          }
        }
      )}` : ``} ${show_share_button ? `${validate_component(ShareButton, "ShareButton").$$render(
        $$result,
        {
          i18n,
          value,
          formatter: async (value2) => {
            if (!value2)
              return "";
            let url = await uploadToHuggingFace(value2.data);
            return url;
          }
        },
        {},
        {}
      )}` : ``}`;
    }
  })}</div>`}`;
});
const VideoPreview$1 = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: VideoPreview
}, Symbol.toStringTag, { value: "Module" }));

export { Player$1 as P, VideoPreview as V, playable as a, VideoPreview$1 as b, loaded as l, prettyBytes as p };
//# sourceMappingURL=VideoPreview-LMtm46ex.js.map
