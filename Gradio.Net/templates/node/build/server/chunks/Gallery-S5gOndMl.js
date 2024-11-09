import { t as tick } from './Component-Dv7eSVA_.js';
import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, b as add_attribute, e as escape, f as each } from './ssr-RaXq3SJh.js';
import { e as BlockLabel, f as Image, g as Empty, h as IconButtonWrapper, i as IconButton, D as Download, M as Maximize, a3 as Minimize, o as ShareButton, Z as Clear, u as uploadToHuggingFace, t as Play } from './2-B6LMYTAg.js';
import { M as ModifyUpload } from './ModifyUpload-CsV9IzIz.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
import { V as Video } from './Video-_epjqq1V.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './DownloadLink--4obEanq.js';
import './hls-CrxM9YLy.js';

var has = Object.prototype.hasOwnProperty;
function find(iter, tar, key) {
  for (key of iter.keys()) {
    if (dequal(key, tar))
      return key;
  }
}
function dequal(foo, bar) {
  var ctor, len, tmp;
  if (foo === bar)
    return true;
  if (foo && bar && (ctor = foo.constructor) === bar.constructor) {
    if (ctor === Date)
      return foo.getTime() === bar.getTime();
    if (ctor === RegExp)
      return foo.toString() === bar.toString();
    if (ctor === Array) {
      if ((len = foo.length) === bar.length) {
        while (len-- && dequal(foo[len], bar[len]))
          ;
      }
      return len === -1;
    }
    if (ctor === Set) {
      if (foo.size !== bar.size) {
        return false;
      }
      for (len of foo) {
        tmp = len;
        if (tmp && typeof tmp === "object") {
          tmp = find(bar, tmp);
          if (!tmp)
            return false;
        }
        if (!bar.has(tmp))
          return false;
      }
      return true;
    }
    if (ctor === Map) {
      if (foo.size !== bar.size) {
        return false;
      }
      for (len of foo) {
        tmp = len[0];
        if (tmp && typeof tmp === "object") {
          tmp = find(bar, tmp);
          if (!tmp)
            return false;
        }
        if (!dequal(len[1], bar.get(tmp))) {
          return false;
        }
      }
      return true;
    }
    if (ctor === ArrayBuffer) {
      foo = new Uint8Array(foo);
      bar = new Uint8Array(bar);
    } else if (ctor === DataView) {
      if ((len = foo.byteLength) === bar.byteLength) {
        while (len-- && foo.getInt8(len) === bar.getInt8(len))
          ;
      }
      return len === -1;
    }
    if (ArrayBuffer.isView(foo)) {
      if ((len = foo.byteLength) === bar.byteLength) {
        while (len-- && foo[len] === bar[len])
          ;
      }
      return len === -1;
    }
    if (!ctor || typeof foo === "object") {
      len = 0;
      for (ctor in foo) {
        if (has.call(foo, ctor) && ++len && !has.call(bar, ctor))
          return false;
        if (!(ctor in bar) || !dequal(foo[ctor], bar[ctor]))
          return false;
      }
      return Object.keys(bar).length === len;
    }
  }
  return foo !== foo && bar !== bar;
}
async function format_gallery_for_sharing(value) {
  if (!value)
    return "";
  let urls = await Promise.all(
    value.map(async ([image, _]) => {
      if (image === null || !image.url)
        return "";
      return await uploadToHuggingFace(image.url);
    })
  );
  return `<div style="display: flex; flex-wrap: wrap; gap: 16px">${urls.map((url) => `<img src="${url}" style="height: 400px" />`).join("")}</div>`;
}
const css = {
  code: 'button.svelte-17j4qub.svelte-17j4qub{width:var(--size-full);height:var(--size-full);object-fit:contain;display:block;border-radius:var(--radius-lg)}.preview.svelte-17j4qub.svelte-17j4qub{display:flex;position:absolute;flex-direction:column;z-index:var(--layer-2);border-radius:calc(var(--block-radius) - var(--block-border-width));-webkit-backdrop-filter:blur(8px);backdrop-filter:blur(8px);width:var(--size-full);height:var(--size-full)}.preview.minimal.svelte-17j4qub.svelte-17j4qub{width:fit-content;height:fit-content}.preview.svelte-17j4qub.svelte-17j4qub::before{content:"";position:absolute;z-index:var(--layer-below);background:var(--background-fill-primary);opacity:0.9;width:var(--size-full);height:var(--size-full)}.fixed-height.svelte-17j4qub.svelte-17j4qub{min-height:var(--size-80);max-height:55vh}@media(min-width: 1280px){.fixed-height.svelte-17j4qub.svelte-17j4qub{min-height:450px}}.media-button.svelte-17j4qub.svelte-17j4qub{height:calc(100% - 60px);width:100%;display:flex}.media-button.svelte-17j4qub img,.media-button.svelte-17j4qub video{width:var(--size-full);height:var(--size-full);object-fit:contain}.thumbnails.svelte-17j4qub img{object-fit:cover;width:var(--size-full);height:var(--size-full)}.thumbnails.svelte-17j4qub svg{position:absolute;top:var(--size-2);left:var(--size-2);width:50%;height:50%;opacity:50%}.preview.svelte-17j4qub img.with-caption{height:var(--size-full)}.preview.minimal.svelte-17j4qub img.with-caption{height:auto}.caption.svelte-17j4qub.svelte-17j4qub{padding:var(--size-2) var(--size-3);overflow:hidden;color:var(--block-label-text-color);font-weight:var(--weight-semibold);text-align:center;text-overflow:ellipsis;white-space:nowrap;align-self:center}.thumbnails.svelte-17j4qub.svelte-17j4qub{display:flex;position:absolute;bottom:0;justify-content:center;align-items:center;gap:var(--spacing-lg);width:var(--size-full);height:var(--size-14);overflow-x:scroll}.thumbnail-item.svelte-17j4qub.svelte-17j4qub{--ring-color:transparent;position:relative;box-shadow:inset 0 0 0 1px var(--ring-color),\n			var(--shadow-drop);border:1px solid var(--border-color-primary);border-radius:var(--button-small-radius);background:var(--background-fill-secondary);aspect-ratio:var(--ratio-square);width:var(--size-full);height:var(--size-full);overflow:clip}.thumbnail-item.svelte-17j4qub.svelte-17j4qub:hover{--ring-color:var(--color-accent);border-color:var(--color-accent);filter:brightness(1.1)}.thumbnail-item.selected.svelte-17j4qub.svelte-17j4qub{--ring-color:var(--color-accent);border-color:var(--color-accent)}.thumbnail-item.svelte-17j4qub svg{position:absolute;top:50%;left:50%;width:50%;height:50%;opacity:50%;transform:translate(-50%, -50%)}.thumbnail-item.svelte-17j4qub video{width:var(--size-full);height:var(--size-full);overflow:hidden;object-fit:cover}.thumbnail-small.svelte-17j4qub.svelte-17j4qub{flex:none;transform:scale(0.9);transition:0.075s;width:var(--size-9);height:var(--size-9)}.thumbnail-small.selected.svelte-17j4qub.svelte-17j4qub{--ring-color:var(--color-accent);transform:scale(1);border-color:var(--color-accent)}.grid-wrap.svelte-17j4qub.svelte-17j4qub{position:relative;padding:var(--size-2);height:var(--size-full);overflow-y:scroll}.grid-container.svelte-17j4qub.svelte-17j4qub{display:grid;position:relative;grid-template-rows:repeat(var(--grid-rows), minmax(100px, 1fr));grid-template-columns:repeat(var(--grid-cols), minmax(100px, 1fr));grid-auto-rows:minmax(100px, 1fr);gap:var(--spacing-lg)}.thumbnail-lg.svelte-17j4qub>img{width:var(--size-full);height:var(--size-full);overflow:hidden;object-fit:var(--object-fit)}.thumbnail-lg.svelte-17j4qub:hover .caption-label.svelte-17j4qub{opacity:0.5}.caption-label.svelte-17j4qub.svelte-17j4qub{position:absolute;right:var(--block-label-margin);bottom:var(--block-label-margin);z-index:var(--layer-1);border-top:1px solid var(--border-color-primary);border-left:1px solid var(--border-color-primary);border-radius:var(--block-label-radius);background:var(--background-fill-secondary);padding:var(--block-label-padding);max-width:80%;overflow:hidden;font-size:var(--block-label-text-size);text-align:left;text-overflow:ellipsis;white-space:nowrap}.icon-button.svelte-17j4qub.svelte-17j4qub{top:1px;right:1px}.grid-wrap.minimal.svelte-17j4qub.svelte-17j4qub{padding:0}',
  map: `{"version":3,"file":"Gallery.svelte","sources":["Gallery.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { BlockLabel, Empty, ShareButton } from \\"@gradio/atoms\\";\\nimport { ModifyUpload } from \\"@gradio/upload\\";\\nimport { Image } from \\"@gradio/image/shared\\";\\nimport { Video } from \\"@gradio/video/shared\\";\\nimport { dequal } from \\"dequal\\";\\nimport { createEventDispatcher, onMount } from \\"svelte\\";\\nimport { tick } from \\"svelte\\";\\nimport { Download, Image as ImageIcon, Maximize, Minimize, Clear, Play } from \\"@gradio/icons\\";\\nimport { FileData } from \\"@gradio/client\\";\\nimport { format_gallery_for_sharing } from \\"./utils\\";\\nimport { IconButton, IconButtonWrapper } from \\"@gradio/atoms\\";\\nexport let show_label = true;\\nexport let label;\\nexport let value = null;\\nexport let columns = [2];\\nexport let rows = void 0;\\nexport let height = \\"auto\\";\\nexport let preview;\\nexport let allow_preview = true;\\nexport let object_fit = \\"cover\\";\\nexport let show_share_button = false;\\nexport let show_download_button = false;\\nexport let i18n;\\nexport let selected_index = null;\\nexport let interactive;\\nexport let _fetch;\\nexport let mode = \\"normal\\";\\nexport let show_fullscreen_button = true;\\nlet is_full_screen = false;\\nlet gallery_container;\\nconst dispatch = createEventDispatcher();\\nlet was_reset = true;\\n$: was_reset = value == null || value.length === 0 ? true : was_reset;\\nlet resolved_value = null;\\n$: resolved_value = value == null ? null : value.map((data) => {\\n    if (\\"video\\" in data) {\\n        return {\\n            video: data.video,\\n            caption: data.caption\\n        };\\n    }\\n    else if (\\"image\\" in data) {\\n        return { image: data.image, caption: data.caption };\\n    }\\n    return {};\\n});\\nlet prev_value = value;\\nif (selected_index == null && preview && value?.length) {\\n    selected_index = 0;\\n}\\nlet old_selected_index = selected_index;\\n$: if (!dequal(prev_value, value)) {\\n    if (was_reset) {\\n        selected_index = preview && value?.length ? 0 : null;\\n        was_reset = false;\\n    }\\n    else {\\n        selected_index = selected_index != null && value != null && selected_index < value.length ? selected_index : null;\\n    }\\n    dispatch(\\"change\\");\\n    prev_value = value;\\n}\\n$: previous = ((selected_index ?? 0) + (resolved_value?.length ?? 0) - 1) % (resolved_value?.length ?? 0);\\n$: next = ((selected_index ?? 0) + 1) % (resolved_value?.length ?? 0);\\nfunction handle_preview_click(event) {\\n    const element = event.target;\\n    const x = event.offsetX;\\n    const width = element.offsetWidth;\\n    const centerX = width / 2;\\n    if (x < centerX) {\\n        selected_index = previous;\\n    }\\n    else {\\n        selected_index = next;\\n    }\\n}\\nfunction on_keydown(e) {\\n    switch (e.code) {\\n        case \\"Escape\\":\\n            e.preventDefault();\\n            selected_index = null;\\n            break;\\n        case \\"ArrowLeft\\":\\n            e.preventDefault();\\n            selected_index = previous;\\n            break;\\n        case \\"ArrowRight\\":\\n            e.preventDefault();\\n            selected_index = next;\\n            break;\\n        default:\\n            break;\\n    }\\n}\\n$: {\\n    if (selected_index !== old_selected_index) {\\n        old_selected_index = selected_index;\\n        if (selected_index !== null) {\\n            dispatch(\\"select\\", {\\n                index: selected_index,\\n                value: resolved_value?.[selected_index]\\n            });\\n        }\\n    }\\n}\\n$: if (allow_preview) {\\n    scroll_to_img(selected_index);\\n}\\nlet el = [];\\nlet container_element;\\nasync function scroll_to_img(index) {\\n    if (typeof index !== \\"number\\")\\n        return;\\n    await tick();\\n    if (el[index] === void 0)\\n        return;\\n    el[index]?.focus();\\n    const { left: container_left, width: container_width } = container_element.getBoundingClientRect();\\n    const { left, width } = el[index].getBoundingClientRect();\\n    const relative_left = left - container_left;\\n    const pos = relative_left + width / 2 - container_width / 2 + container_element.scrollLeft;\\n    if (container_element && typeof container_element.scrollTo === \\"function\\") {\\n        container_element.scrollTo({\\n            left: pos < 0 ? 0 : pos,\\n            behavior: \\"smooth\\"\\n        });\\n    }\\n}\\nlet window_height = 0;\\nasync function download(file_url, name) {\\n    let response;\\n    try {\\n        response = await _fetch(file_url);\\n    }\\n    catch (error) {\\n        if (error instanceof TypeError) {\\n            window.open(file_url, \\"_blank\\", \\"noreferrer\\");\\n            return;\\n        }\\n        throw error;\\n    }\\n    const blob = await response.blob();\\n    const url = URL.createObjectURL(blob);\\n    const link = document.createElement(\\"a\\");\\n    link.href = url;\\n    link.download = name;\\n    link.click();\\n    URL.revokeObjectURL(url);\\n}\\n$: selected_media = selected_index != null && resolved_value != null ? resolved_value[selected_index] : null;\\nonMount(() => {\\n    document.addEventListener(\\"fullscreenchange\\", () => {\\n        is_full_screen = !!document.fullscreenElement;\\n    });\\n});\\nconst toggle_full_screen = async () => {\\n    if (!is_full_screen) {\\n        await gallery_container.requestFullscreen();\\n    }\\n    else {\\n        await document.exitFullscreen();\\n    }\\n};\\n<\/script>\\n\\n<svelte:window bind:innerHeight={window_height} />\\n\\n{#if show_label}\\n\\t<BlockLabel {show_label} Icon={ImageIcon} label={label || \\"Gallery\\"} />\\n{/if}\\n{#if value == null || resolved_value == null || resolved_value.length === 0}\\n\\t<Empty unpadded_box={true} size=\\"large\\"><ImageIcon /></Empty>\\n{:else}\\n\\t<div class=\\"gallery-container\\" bind:this={gallery_container}>\\n\\t\\t{#if selected_media && allow_preview}\\n\\t\\t\\t<button\\n\\t\\t\\t\\ton:keydown={on_keydown}\\n\\t\\t\\t\\tclass=\\"preview\\"\\n\\t\\t\\t\\tclass:minimal={mode === \\"minimal\\"}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<IconButtonWrapper>\\n\\t\\t\\t\\t\\t{#if show_download_button}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={Download}\\n\\t\\t\\t\\t\\t\\t\\tlabel={i18n(\\"common.download\\")}\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\t\\tconst image =\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\"image\\" in selected_media\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t? selected_media?.image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t: selected_media?.video;\\n\\t\\t\\t\\t\\t\\t\\t\\tif (image == null) {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\treturn;\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t\\tconst { url, orig_name } = image;\\n\\t\\t\\t\\t\\t\\t\\t\\tif (url) {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tdownload(url, orig_name ?? \\"image\\");\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\n\\t\\t\\t\\t\\t{#if show_fullscreen_button && !is_full_screen}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={is_full_screen ? Minimize : Maximize}\\n\\t\\t\\t\\t\\t\\t\\tlabel={is_full_screen\\n\\t\\t\\t\\t\\t\\t\\t\\t? \\"Exit full screen\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t: \\"View in full screen\\"}\\n\\t\\t\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\n\\t\\t\\t\\t\\t{#if show_fullscreen_button && is_full_screen}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={Minimize}\\n\\t\\t\\t\\t\\t\\t\\tlabel=\\"Exit full screen\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t{#if show_share_button}\\n\\t\\t\\t\\t\\t\\t<div class=\\"icon-button\\">\\n\\t\\t\\t\\t\\t\\t\\t<ShareButton\\n\\t\\t\\t\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:share\\n\\t\\t\\t\\t\\t\\t\\t\\ton:error\\n\\t\\t\\t\\t\\t\\t\\t\\tvalue={resolved_value}\\n\\t\\t\\t\\t\\t\\t\\t\\tformatter={format_gallery_for_sharing}\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t{#if !is_full_screen}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={Clear}\\n\\t\\t\\t\\t\\t\\t\\tlabel=\\"Close\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => (selected_index = null)}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</IconButtonWrapper>\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"media-button\\"\\n\\t\\t\\t\\t\\ton:click={\\"image\\" in selected_media\\n\\t\\t\\t\\t\\t\\t? (event) => handle_preview_click(event)\\n\\t\\t\\t\\t\\t\\t: null}\\n\\t\\t\\t\\t\\tstyle=\\"height: calc(100% - {selected_media.caption\\n\\t\\t\\t\\t\\t\\t? '80px'\\n\\t\\t\\t\\t\\t\\t: '60px'})\\"\\n\\t\\t\\t\\t\\taria-label=\\"detailed view of selected image\\"\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#if \\"image\\" in selected_media}\\n\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\tdata-testid=\\"detailed-image\\"\\n\\t\\t\\t\\t\\t\\t\\tsrc={selected_media.image.url}\\n\\t\\t\\t\\t\\t\\t\\talt={selected_media.caption || \\"\\"}\\n\\t\\t\\t\\t\\t\\t\\ttitle={selected_media.caption || null}\\n\\t\\t\\t\\t\\t\\t\\tclass={selected_media.caption && \\"with-caption\\"}\\n\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t<Video\\n\\t\\t\\t\\t\\t\\t\\tsrc={selected_media.video.url}\\n\\t\\t\\t\\t\\t\\t\\tdata-testid={\\"detailed-video\\"}\\n\\t\\t\\t\\t\\t\\t\\talt={selected_media.caption || \\"\\"}\\n\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\tloop={false}\\n\\t\\t\\t\\t\\t\\t\\tis_stream={false}\\n\\t\\t\\t\\t\\t\\t\\tmuted={false}\\n\\t\\t\\t\\t\\t\\t\\tcontrols={true}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t{#if selected_media?.caption}\\n\\t\\t\\t\\t\\t<caption class=\\"caption\\">\\n\\t\\t\\t\\t\\t\\t{selected_media.caption}\\n\\t\\t\\t\\t\\t</caption>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\tbind:this={container_element}\\n\\t\\t\\t\\t\\tclass=\\"thumbnails scroll-hide\\"\\n\\t\\t\\t\\t\\tdata-testid=\\"container_el\\"\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#each resolved_value as media, i}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\tbind:this={el[i]}\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => (selected_index = i)}\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"thumbnail-item thumbnail-small\\"\\n\\t\\t\\t\\t\\t\\t\\tclass:selected={selected_index === i && mode !== \\"minimal\\"}\\n\\t\\t\\t\\t\\t\\t\\taria-label={\\"Thumbnail \\" +\\n\\t\\t\\t\\t\\t\\t\\t\\t(i + 1) +\\n\\t\\t\\t\\t\\t\\t\\t\\t\\" of \\" +\\n\\t\\t\\t\\t\\t\\t\\t\\tresolved_value.length}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{#if \\"image\\" in media}\\n\\t\\t\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tsrc={media.image.url}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ttitle={media.caption || null}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tdata-testid={\\"thumbnail \\" + (i + 1)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\talt=\\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t\\t<Play />\\n\\t\\t\\t\\t\\t\\t\\t\\t<Video\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tsrc={media.video.url}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ttitle={media.caption || null}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tis_stream={false}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tdata-testid={\\"thumbnail \\" + (i + 1)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\talt=\\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tloop={false}\\n\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t</button>\\n\\t\\t{/if}\\n\\n\\t\\t<div\\n\\t\\t\\tclass=\\"grid-wrap\\"\\n\\t\\t\\tclass:minimal={mode === \\"minimal\\"}\\n\\t\\t\\tclass:fixed-height={mode !== \\"minimal\\" && (!height || height == \\"auto\\")}\\n\\t\\t\\tclass:hidden={is_full_screen}\\n\\t\\t>\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"grid-container\\"\\n\\t\\t\\t\\tstyle=\\"--grid-cols:{columns}; --grid-rows:{rows}; --object-fit: {object_fit}; height: {height};\\"\\n\\t\\t\\t\\tclass:pt-6={show_label}\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#if interactive}\\n\\t\\t\\t\\t\\t<div class=\\"icon-button\\">\\n\\t\\t\\t\\t\\t\\t<ModifyUpload {i18n} on:clear={() => (value = [])} />\\n\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{#each resolved_value as entry, i}\\n\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\tclass=\\"thumbnail-item thumbnail-lg\\"\\n\\t\\t\\t\\t\\t\\tclass:selected={selected_index === i}\\n\\t\\t\\t\\t\\t\\ton:click={() => (selected_index = i)}\\n\\t\\t\\t\\t\\t\\taria-label={\\"Thumbnail \\" + (i + 1) + \\" of \\" + resolved_value.length}\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t{#if \\"image\\" in entry}\\n\\t\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\t\\talt={entry.caption || \\"\\"}\\n\\t\\t\\t\\t\\t\\t\\t\\tsrc={typeof entry.image === \\"string\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t? entry.image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t: entry.image.url}\\n\\t\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t<Play />\\n\\t\\t\\t\\t\\t\\t\\t<Video\\n\\t\\t\\t\\t\\t\\t\\t\\tsrc={entry.video.url}\\n\\t\\t\\t\\t\\t\\t\\t\\ttitle={entry.caption || null}\\n\\t\\t\\t\\t\\t\\t\\t\\tis_stream={false}\\n\\t\\t\\t\\t\\t\\t\\t\\tdata-testid={\\"thumbnail \\" + (i + 1)}\\n\\t\\t\\t\\t\\t\\t\\t\\talt=\\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\t\\tloop={false}\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t{#if entry.caption}\\n\\t\\t\\t\\t\\t\\t\\t<div class=\\"caption-label\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t{entry.caption}\\n\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t</div>\\n{/if}\\n\\n<style lang=\\"postcss\\">\\n\\t.image-container {\\n\\t\\theight: 100%;\\n\\t\\tposition: relative;\\n\\t}\\n\\t.image-container :global(img),\\n\\tbutton {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: contain;\\n\\t\\tdisplay: block;\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t}\\n\\n\\t.preview {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: absolute;\\n\\t\\tflex-direction: column;\\n\\t\\tz-index: var(--layer-2);\\n\\t\\tborder-radius: calc(var(--block-radius) - var(--block-border-width));\\n\\t\\t-webkit-backdrop-filter: blur(8px);\\n\\t\\tbackdrop-filter: blur(8px);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\n\\t.preview.minimal {\\n\\t\\twidth: fit-content;\\n\\t\\theight: fit-content;\\n\\t}\\n\\n\\t.preview::before {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\tz-index: var(--layer-below);\\n\\t\\tbackground: var(--background-fill-primary);\\n\\t\\topacity: 0.9;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\n\\t.fixed-height {\\n\\t\\tmin-height: var(--size-80);\\n\\t\\tmax-height: 55vh;\\n\\t}\\n\\n\\t@media (min-width: 1280px) {\\n\\t\\t.fixed-height {\\n\\t\\t\\tmin-height: 450px;\\n\\t\\t}\\n\\t}\\n\\n\\t.media-button {\\n\\t\\theight: calc(100% - 60px);\\n\\t\\twidth: 100%;\\n\\t\\tdisplay: flex;\\n\\t}\\n\\t.media-button :global(img),\\n\\t.media-button :global(video) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: contain;\\n\\t}\\n\\t.thumbnails :global(img) {\\n\\t\\tobject-fit: cover;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\t.thumbnails :global(svg) {\\n\\t\\tposition: absolute;\\n\\t\\ttop: var(--size-2);\\n\\t\\tleft: var(--size-2);\\n\\t\\twidth: 50%;\\n\\t\\theight: 50%;\\n\\t\\topacity: 50%;\\n\\t}\\n\\t.preview :global(img.with-caption) {\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\n\\t.preview.minimal :global(img.with-caption) {\\n\\t\\theight: auto;\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: crosshair;\\n\\t}\\n\\n\\t.caption {\\n\\t\\tpadding: var(--size-2) var(--size-3);\\n\\t\\toverflow: hidden;\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t\\tfont-weight: var(--weight-semibold);\\n\\t\\ttext-align: center;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\twhite-space: nowrap;\\n\\t\\talign-self: center;\\n\\t}\\n\\n\\t.thumbnails {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 0;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--spacing-lg);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-14);\\n\\t\\toverflow-x: scroll;\\n\\t}\\n\\n\\t.thumbnail-item {\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow:\\n\\t\\t\\tinset 0 0 0 1px var(--ring-color),\\n\\t\\t\\tvar(--shadow-drop);\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--button-small-radius);\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\taspect-ratio: var(--ratio-square);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\toverflow: clip;\\n\\t}\\n\\n\\t.thumbnail-item:hover {\\n\\t\\t--ring-color: var(--color-accent);\\n\\t\\tborder-color: var(--color-accent);\\n\\t\\tfilter: brightness(1.1);\\n\\t}\\n\\n\\t.thumbnail-item.selected {\\n\\t\\t--ring-color: var(--color-accent);\\n\\t\\tborder-color: var(--color-accent);\\n\\t}\\n\\n\\t.thumbnail-item :global(svg) {\\n\\t\\tposition: absolute;\\n\\t\\ttop: 50%;\\n\\t\\tleft: 50%;\\n\\t\\twidth: 50%;\\n\\t\\theight: 50%;\\n\\t\\topacity: 50%;\\n\\t\\ttransform: translate(-50%, -50%);\\n\\t}\\n\\n\\t.thumbnail-item :global(video) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\toverflow: hidden;\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\t.thumbnail-small {\\n\\t\\tflex: none;\\n\\t\\ttransform: scale(0.9);\\n\\t\\ttransition: 0.075s;\\n\\t\\twidth: var(--size-9);\\n\\t\\theight: var(--size-9);\\n\\t}\\n\\t.thumbnail-small.selected {\\n\\t\\t--ring-color: var(--color-accent);\\n\\t\\ttransform: scale(1);\\n\\t\\tborder-color: var(--color-accent);\\n\\t}\\n\\n\\t.thumbnail-small > img {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\toverflow: hidden;\\n\\t\\tobject-fit: var(--object-fit);\\n\\t}\\n\\n\\t.grid-wrap {\\n\\t\\tposition: relative;\\n\\t\\tpadding: var(--size-2);\\n\\t\\theight: var(--size-full);\\n\\t\\toverflow-y: scroll;\\n\\t}\\n\\n\\t.grid-container {\\n\\t\\tdisplay: grid;\\n\\t\\tposition: relative;\\n\\t\\tgrid-template-rows: repeat(var(--grid-rows), minmax(100px, 1fr));\\n\\t\\tgrid-template-columns: repeat(var(--grid-cols), minmax(100px, 1fr));\\n\\t\\tgrid-auto-rows: minmax(100px, 1fr);\\n\\t\\tgap: var(--spacing-lg);\\n\\t}\\n\\n\\t.thumbnail-lg > :global(img) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\toverflow: hidden;\\n\\t\\tobject-fit: var(--object-fit);\\n\\t}\\n\\n\\t.thumbnail-lg:hover .caption-label {\\n\\t\\topacity: 0.5;\\n\\t}\\n\\n\\t.caption-label {\\n\\t\\tposition: absolute;\\n\\t\\tright: var(--block-label-margin);\\n\\t\\tbottom: var(--block-label-margin);\\n\\t\\tz-index: var(--layer-1);\\n\\t\\tborder-top: 1px solid var(--border-color-primary);\\n\\t\\tborder-left: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--block-label-radius);\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\tpadding: var(--block-label-padding);\\n\\t\\tmax-width: 80%;\\n\\t\\toverflow: hidden;\\n\\t\\tfont-size: var(--block-label-text-size);\\n\\t\\ttext-align: left;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\twhite-space: nowrap;\\n\\t}\\n\\n\\t.icon-button {\\n\\t\\ttop: 1px;\\n\\t\\tright: 1px;\\n\\t}\\n\\n\\t.grid-wrap.minimal {\\n\\t\\tpadding: 0;\\n\\t}</style>\\n"],"names":[],"mappings":"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"}`
};
const Gallery = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let selected_media;
  let { show_label = true } = $$props;
  let { label } = $$props;
  let { value = null } = $$props;
  let { columns = [2] } = $$props;
  let { rows = void 0 } = $$props;
  let { height = "auto" } = $$props;
  let { preview } = $$props;
  let { allow_preview = true } = $$props;
  let { object_fit = "cover" } = $$props;
  let { show_share_button = false } = $$props;
  let { show_download_button = false } = $$props;
  let { i18n } = $$props;
  let { selected_index = null } = $$props;
  let { interactive } = $$props;
  let { _fetch } = $$props;
  let { mode = "normal" } = $$props;
  let { show_fullscreen_button = true } = $$props;
  let is_full_screen = false;
  let gallery_container;
  const dispatch = createEventDispatcher();
  let was_reset = true;
  let resolved_value = null;
  let prev_value = value;
  if (selected_index == null && preview && value?.length) {
    selected_index = 0;
  }
  let old_selected_index = selected_index;
  let el = [];
  let container_element;
  async function scroll_to_img(index) {
    if (typeof index !== "number")
      return;
    await tick();
    if (el[index] === void 0)
      return;
    el[index]?.focus();
    const { left: container_left, width: container_width } = container_element.getBoundingClientRect();
    const { left, width } = el[index].getBoundingClientRect();
    const relative_left = left - container_left;
    relative_left + width / 2 - container_width / 2 + container_element.scrollLeft;
  }
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
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
  if ($$props.object_fit === void 0 && $$bindings.object_fit && object_fit !== void 0)
    $$bindings.object_fit(object_fit);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.selected_index === void 0 && $$bindings.selected_index && selected_index !== void 0)
    $$bindings.selected_index(selected_index);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props._fetch === void 0 && $$bindings._fetch && _fetch !== void 0)
    $$bindings._fetch(_fetch);
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.show_fullscreen_button === void 0 && $$bindings.show_fullscreen_button && show_fullscreen_button !== void 0)
    $$bindings.show_fullscreen_button(show_fullscreen_button);
  $$result.css.add(css);
  was_reset = value == null || value.length === 0 ? true : was_reset;
  resolved_value = value == null ? null : value.map((data) => {
    if ("video" in data) {
      return { video: data.video, caption: data.caption };
    } else if ("image" in data) {
      return { image: data.image, caption: data.caption };
    }
    return {};
  });
  {
    if (!dequal(prev_value, value)) {
      if (was_reset) {
        selected_index = preview && value?.length ? 0 : null;
        was_reset = false;
      } else {
        selected_index = selected_index != null && value != null && selected_index < value.length ? selected_index : null;
      }
      dispatch("change");
      prev_value = value;
    }
  }
  ((selected_index ?? 0) + (resolved_value?.length ?? 0) - 1) % (resolved_value?.length ?? 0);
  ((selected_index ?? 0) + 1) % (resolved_value?.length ?? 0);
  {
    {
      if (selected_index !== old_selected_index) {
        old_selected_index = selected_index;
        if (selected_index !== null) {
          dispatch("select", {
            index: selected_index,
            value: resolved_value?.[selected_index]
          });
        }
      }
    }
  }
  {
    if (allow_preview) {
      scroll_to_img(selected_index);
    }
  }
  selected_media = selected_index != null && resolved_value != null ? resolved_value[selected_index] : null;
  return ` ${show_label ? `${validate_component(BlockLabel, "BlockLabel").$$render(
    $$result,
    {
      show_label,
      Icon: Image,
      label: label || "Gallery"
    },
    {},
    {}
  )}` : ``} ${value == null || resolved_value == null || resolved_value.length === 0 ? `${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true, size: "large" }, {}, {
    default: () => {
      return `${validate_component(Image, "ImageIcon").$$render($$result, {}, {}, {})}`;
    }
  })}` : `<div class="gallery-container"${add_attribute("this", gallery_container, 0)}>${selected_media && allow_preview ? `<button class="${["preview svelte-17j4qub", mode === "minimal" ? "minimal" : ""].join(" ").trim()}">${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${show_download_button ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Download,
          label: i18n("common.download")
        },
        {},
        {}
      )}` : ``} ${show_fullscreen_button && !is_full_screen ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Maximize,
          label: "View in full screen"
        },
        {},
        {}
      )}` : ``} ${show_fullscreen_button && is_full_screen ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Minimize,
          label: "Exit full screen"
        },
        {},
        {}
      )}` : ``} ${show_share_button ? `<div class="icon-button svelte-17j4qub">${validate_component(ShareButton, "ShareButton").$$render(
        $$result,
        {
          i18n,
          value: resolved_value,
          formatter: format_gallery_for_sharing
        },
        {},
        {}
      )}</div>` : ``} ${`${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Clear, label: "Close" }, {}, {})}` }`;
    }
  })} <button class="media-button svelte-17j4qub" style="${"height: calc(100% - " + escape(selected_media.caption ? "80px" : "60px", true) + ")"}" aria-label="detailed view of selected image">${"image" in selected_media ? `${validate_component(Image$1, "Image").$$render(
    $$result,
    {
      "data-testid": "detailed-image",
      src: selected_media.image.url,
      alt: selected_media.caption || "",
      title: selected_media.caption || null,
      class: selected_media.caption && "with-caption",
      loading: "lazy"
    },
    {},
    {}
  )}` : `${validate_component(Video, "Video").$$render(
    $$result,
    {
      src: selected_media.video.url,
      "data-testid": "detailed-video",
      alt: selected_media.caption || "",
      loading: "lazy",
      loop: false,
      is_stream: false,
      muted: false,
      controls: true
    },
    {},
    {}
  )}`}</button> ${selected_media?.caption ? `<caption class="caption svelte-17j4qub">${escape(selected_media.caption)}</caption>` : ``} <div class="thumbnails scroll-hide svelte-17j4qub" data-testid="container_el"${add_attribute("this", container_element, 0)}>${each(resolved_value, (media, i) => {
    return `<button class="${[
      "thumbnail-item thumbnail-small svelte-17j4qub",
      selected_index === i && mode !== "minimal" ? "selected" : ""
    ].join(" ").trim()}"${add_attribute("aria-label", "Thumbnail " + (i + 1) + " of " + resolved_value.length, 0)}${add_attribute("this", el[i], 0)}>${"image" in media ? `${validate_component(Image$1, "Image").$$render(
      $$result,
      {
        src: media.image.url,
        title: media.caption || null,
        "data-testid": "thumbnail " + (i + 1),
        alt: "",
        loading: "lazy"
      },
      {},
      {}
    )}` : `${validate_component(Play, "Play").$$render($$result, {}, {}, {})} ${validate_component(Video, "Video").$$render(
      $$result,
      {
        src: media.video.url,
        title: media.caption || null,
        is_stream: false,
        "data-testid": "thumbnail " + (i + 1),
        alt: "",
        loading: "lazy",
        loop: false
      },
      {},
      {}
    )}`} </button>`;
  })}</div></button>` : ``} <div class="${[
    "grid-wrap svelte-17j4qub",
    (mode === "minimal" ? "minimal" : "") + " " + (mode !== "minimal" && (!height || height == "auto") ? "fixed-height" : "") + " " + ("")
  ].join(" ").trim()}"><div class="${["grid-container svelte-17j4qub", show_label ? "pt-6" : ""].join(" ").trim()}" style="${"--grid-cols:" + escape(columns, true) + "; --grid-rows:" + escape(rows, true) + "; --object-fit: " + escape(object_fit, true) + "; height: " + escape(height, true) + ";"}">${interactive ? `<div class="icon-button svelte-17j4qub">${validate_component(ModifyUpload, "ModifyUpload").$$render($$result, { i18n }, {}, {})}</div>` : ``} ${each(resolved_value, (entry, i) => {
    return `<button class="${[
      "thumbnail-item thumbnail-lg svelte-17j4qub",
      selected_index === i ? "selected" : ""
    ].join(" ").trim()}"${add_attribute("aria-label", "Thumbnail " + (i + 1) + " of " + resolved_value.length, 0)}>${"image" in entry ? `${validate_component(Image$1, "Image").$$render(
      $$result,
      {
        alt: entry.caption || "",
        src: typeof entry.image === "string" ? entry.image : entry.image.url,
        loading: "lazy"
      },
      {},
      {}
    )}` : `${validate_component(Play, "Play").$$render($$result, {}, {}, {})} ${validate_component(Video, "Video").$$render(
      $$result,
      {
        src: entry.video.url,
        title: entry.caption || null,
        is_stream: false,
        "data-testid": "thumbnail " + (i + 1),
        alt: "",
        loading: "lazy",
        loop: false
      },
      {},
      {}
    )}`} ${entry.caption ? `<div class="caption-label svelte-17j4qub">${escape(entry.caption)} </div>` : ``} </button>`;
  })}</div></div></div>`}`;
});

export { Gallery as default };
//# sourceMappingURL=Gallery-S5gOndMl.js.map
