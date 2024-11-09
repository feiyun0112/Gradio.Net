import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, b as add_attribute, r as add_classes, e as escape, f as each } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, N as BlockTitle, Z as Clear, j as Music, ap as Video, a1 as File$1, aq as Paperclip, ar as Send, as as Square } from './2-B6LMYTAg.js';
import { U as Upload } from './ModifyUpload-CsV9IzIz.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
export { default as BaseExample } from './Example18-DaP4wk54.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './DownloadLink--4obEanq.js';
import './Video-_epjqq1V.js';
import './hls-CrxM9YLy.js';

const css = {
  code: '.full-container.svelte-1d7elt4{width:100%;position:relative}.full-container.dragging.svelte-1d7elt4::after{content:"";position:absolute;top:0;left:0;right:0;bottom:0;pointer-events:none}.input-container.svelte-1d7elt4{display:flex;position:relative;align-items:flex-end}textarea.svelte-1d7elt4{flex-grow:1;outline:none !important;background:var(--block-background-fill);padding:var(--input-padding);color:var(--body-text-color);font-weight:var(--input-text-weight);font-size:var(--input-text-size);line-height:var(--line-sm);border:none;margin-top:0px;margin-bottom:0px;resize:none;position:relative;z-index:1}textarea.no-label.svelte-1d7elt4{padding-top:5px;padding-bottom:5px}textarea.svelte-1d7elt4:disabled{-webkit-opacity:1;opacity:1}textarea.svelte-1d7elt4::placeholder{color:var(--input-placeholder-color)}.upload-button.svelte-1d7elt4,.submit-button.svelte-1d7elt4,.stop-button.svelte-1d7elt4{border:none;text-align:center;text-decoration:none;font-size:14px;cursor:pointer;border-radius:15px;min-width:30px;height:30px;flex-shrink:0;display:flex;justify-content:center;align-items:center;z-index:var(--layer-1)}.padded-button.svelte-1d7elt4{padding:0 10px}.stop-button.svelte-1d7elt4,.upload-button.svelte-1d7elt4,.submit-button.svelte-1d7elt4{background:var(--button-secondary-background-fill)}.stop-button.svelte-1d7elt4:hover,.upload-button.svelte-1d7elt4:hover,.submit-button.svelte-1d7elt4:hover{background:var(--button-secondary-background-fill-hover)}.stop-button.svelte-1d7elt4:disabled,.upload-button.svelte-1d7elt4:disabled,.submit-button.svelte-1d7elt4:disabled{background:var(--button-secondary-background-fill);cursor:initial}.stop-button.svelte-1d7elt4:active,.upload-button.svelte-1d7elt4:active,.submit-button.svelte-1d7elt4:active{box-shadow:var(--button-shadow-active)}.submit-button.svelte-1d7elt4 svg{height:22px;width:22px}.upload-button.svelte-1d7elt4 svg{height:17px;width:17px}.stop-button.svelte-1d7elt4 svg{height:16px;width:16px}.loader.svelte-1d7elt4{display:flex;justify-content:center;align-items:center;--ring-color:transparent;position:relative;border:5px solid #f3f3f3;border-top:5px solid var(--color-accent);border-radius:50%;width:25px;height:25px;animation:svelte-1d7elt4-spin 2s linear infinite}@keyframes svelte-1d7elt4-spin{0%{transform:rotate(0deg)}100%{transform:rotate(360deg)}}.thumbnails.svelte-1d7elt4 img{width:var(--size-full);height:var(--size-full);object-fit:cover;border-radius:var(--radius-lg)}.thumbnails.svelte-1d7elt4{display:flex;align-items:center;gap:var(--spacing-lg);overflow-x:scroll;padding-top:var(--spacing-sm);margin-bottom:6px}.thumbnail-item.svelte-1d7elt4{display:flex;justify-content:center;align-items:center;--ring-color:transparent;position:relative;box-shadow:0 0 0 2px var(--ring-color),\n			var(--shadow-drop);border:1px solid var(--border-color-primary);border-radius:var(--radius-lg);background:var(--background-fill-secondary);aspect-ratio:var(--ratio-square);width:var(--size-full);height:var(--size-full);cursor:default}.thumbnail-small.svelte-1d7elt4{flex:none;transform:scale(0.9);transition:0.075s;width:var(--size-12);height:var(--size-12)}.thumbnail-item.svelte-1d7elt4 svg{width:30px;height:30px}.delete-button.svelte-1d7elt4{display:flex;justify-content:center;align-items:center;position:absolute;right:-7px;top:-7px;color:var(--button-secondary-text-color);background:var(--button-secondary-background-fill);border:none;text-align:center;text-decoration:none;font-size:10px;cursor:pointer;border-radius:50%;width:20px;height:20px}.disabled.svelte-1d7elt4{display:none}.delete-button.svelte-1d7elt4 svg{width:12px;height:12px}.delete-button.svelte-1d7elt4:hover{filter:brightness(1.2);border:0.8px solid var(--color-grey-500)}',
  map: `{"version":3,"file":"MultimodalTextbox.svelte","sources":["MultimodalTextbox.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { beforeUpdate, afterUpdate, createEventDispatcher, tick } from \\"svelte\\";\\nimport { text_area_resize, resize } from \\"../shared/utils\\";\\nimport { BlockTitle } from \\"@gradio/atoms\\";\\nimport { Upload } from \\"@gradio/upload\\";\\nimport { Image } from \\"@gradio/image/shared\\";\\nimport { Clear, File, Music, Paperclip, Video, Send, Square } from \\"@gradio/icons\\";\\nexport let value = {\\n    text: \\"\\",\\n    files: []\\n};\\nexport let value_is_output = false;\\nexport let lines = 1;\\nexport let placeholder = \\"Type here...\\";\\nexport let disabled = false;\\nexport let label;\\nexport let info = void 0;\\nexport let show_label = true;\\nexport let container = true;\\nexport let max_lines;\\nexport let submit_btn = null;\\nexport let stop_btn = null;\\nexport let rtl = false;\\nexport let autofocus = false;\\nexport let text_align = void 0;\\nexport let autoscroll = true;\\nexport let root;\\nexport let file_types = null;\\nexport let max_file_size = null;\\nexport let upload;\\nexport let stream_handler;\\nexport let file_count = \\"multiple\\";\\nlet upload_component;\\nlet hidden_upload;\\nlet el;\\nlet can_scroll;\\nlet previous_scroll_top = 0;\\nlet user_has_scrolled_up = false;\\nexport let dragging = false;\\nlet uploading = false;\\nlet oldValue = value.text;\\n$: dispatch(\\"drag\\", dragging);\\nlet full_container;\\n$: if (oldValue !== value.text) {\\n    dispatch(\\"change\\", value);\\n    oldValue = value.text;\\n}\\n$: if (value === null)\\n    value = { text: \\"\\", files: [] };\\n$: value, el && lines !== max_lines && resize(el, lines, max_lines);\\nconst dispatch = createEventDispatcher();\\nbeforeUpdate(() => {\\n    can_scroll = el && el.offsetHeight + el.scrollTop > el.scrollHeight - 100;\\n});\\nconst scroll = () => {\\n    if (can_scroll && autoscroll && !user_has_scrolled_up) {\\n        el.scrollTo(0, el.scrollHeight);\\n    }\\n};\\nasync function handle_change() {\\n    dispatch(\\"change\\", value);\\n    if (!value_is_output) {\\n        dispatch(\\"input\\");\\n    }\\n}\\nafterUpdate(() => {\\n    if (autofocus && el !== null) {\\n        el.focus();\\n    }\\n    if (can_scroll && autoscroll) {\\n        scroll();\\n    }\\n    value_is_output = false;\\n});\\nfunction handle_select(event) {\\n    const target = event.target;\\n    const text = target.value;\\n    const index = [\\n        target.selectionStart,\\n        target.selectionEnd\\n    ];\\n    dispatch(\\"select\\", { value: text.substring(...index), index });\\n}\\nasync function handle_keypress(e) {\\n    await tick();\\n    if (e.key === \\"Enter\\" && e.shiftKey && lines > 1) {\\n        e.preventDefault();\\n        dispatch(\\"submit\\");\\n    }\\n    else if (e.key === \\"Enter\\" && !e.shiftKey && lines === 1 && max_lines >= 1) {\\n        e.preventDefault();\\n        dispatch(\\"submit\\");\\n    }\\n}\\nfunction handle_scroll(event) {\\n    const target = event.target;\\n    const current_scroll_top = target.scrollTop;\\n    if (current_scroll_top < previous_scroll_top) {\\n        user_has_scrolled_up = true;\\n    }\\n    previous_scroll_top = current_scroll_top;\\n    const max_scroll_top = target.scrollHeight - target.clientHeight;\\n    const user_has_scrolled_to_bottom = current_scroll_top >= max_scroll_top;\\n    if (user_has_scrolled_to_bottom) {\\n        user_has_scrolled_up = false;\\n    }\\n}\\nasync function handle_upload({ detail }) {\\n    handle_change();\\n    if (Array.isArray(detail)) {\\n        for (let file of detail) {\\n            value.files.push(file);\\n        }\\n        value = value;\\n    }\\n    else {\\n        value.files.push(detail);\\n        value = value;\\n    }\\n    await tick();\\n    dispatch(\\"change\\", value);\\n    dispatch(\\"upload\\", detail);\\n}\\nfunction remove_thumbnail(event, index) {\\n    handle_change();\\n    event.stopPropagation();\\n    value.files.splice(index, 1);\\n    value = value;\\n}\\nfunction handle_upload_click() {\\n    if (hidden_upload) {\\n        hidden_upload.value = \\"\\";\\n        hidden_upload.click();\\n    }\\n}\\nfunction handle_stop() {\\n    dispatch(\\"stop\\");\\n}\\nfunction handle_submit() {\\n    dispatch(\\"submit\\");\\n}\\nfunction handle_paste(event) {\\n    if (!event.clipboardData)\\n        return;\\n    const items = event.clipboardData.items;\\n    for (let index in items) {\\n        const item = items[index];\\n        if (item.kind === \\"file\\" && item.type.includes(\\"image\\")) {\\n            const blob = item.getAsFile();\\n            if (blob)\\n                upload_component.load_files([blob]);\\n        }\\n    }\\n}\\nfunction handle_dragenter(event) {\\n    event.preventDefault();\\n    dragging = true;\\n}\\nfunction handle_dragleave(event) {\\n    event.preventDefault();\\n    const rect = full_container.getBoundingClientRect();\\n    const { clientX, clientY } = event;\\n    if (clientX <= rect.left || clientX >= rect.right || clientY <= rect.top || clientY >= rect.bottom) {\\n        dragging = false;\\n    }\\n}\\nfunction handle_drop(event) {\\n    event.preventDefault();\\n    dragging = false;\\n    if (event.dataTransfer && event.dataTransfer.files) {\\n        upload_component.load_files(Array.from(event.dataTransfer.files));\\n    }\\n}\\n<\/script>\\n\\n<div\\n\\tclass=\\"full-container\\"\\n\\tclass:dragging\\n\\tbind:this={full_container}\\n\\ton:dragenter={handle_dragenter}\\n\\ton:dragleave={handle_dragleave}\\n\\ton:dragover|preventDefault\\n\\ton:drop={handle_drop}\\n\\trole=\\"group\\"\\n\\taria-label=\\"Multimedia input field\\"\\n>\\n\\t<!-- svelte-ignore a11y-autofocus -->\\n\\t<label class:container>\\n\\t\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\t\\t{#if value.files.length > 0 || uploading}\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"thumbnails scroll-hide\\"\\n\\t\\t\\t\\taria-label=\\"Uploaded files\\"\\n\\t\\t\\t\\tdata-testid=\\"container_el\\"\\n\\t\\t\\t\\tstyle=\\"display: {value.files.length > 0 || uploading\\n\\t\\t\\t\\t\\t? 'flex'\\n\\t\\t\\t\\t\\t: 'none'};\\"\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#each value.files as file, index}\\n\\t\\t\\t\\t\\t<span role=\\"listitem\\" aria-label=\\"File thumbnail\\">\\n\\t\\t\\t\\t\\t\\t<button class=\\"thumbnail-item thumbnail-small\\">\\n\\t\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\t\\tclass:disabled\\n\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"delete-button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\ton:click={(event) => remove_thumbnail(event, index)}\\n\\t\\t\\t\\t\\t\\t\\t\\t><Clear /></button\\n\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{#if file.mime_type && file.mime_type.includes(\\"image\\")}\\n\\t\\t\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tsrc={file.url}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ttitle={null}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\talt=\\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tloading=\\"lazy\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tclass={\\"thumbnail-image\\"}\\n\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t{:else if file.mime_type && file.mime_type.includes(\\"audio\\")}\\n\\t\\t\\t\\t\\t\\t\\t\\t<Music />\\n\\t\\t\\t\\t\\t\\t\\t{:else if file.mime_type && file.mime_type.includes(\\"video\\")}\\n\\t\\t\\t\\t\\t\\t\\t\\t<Video />\\n\\t\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t\\t<File />\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t{#if uploading}\\n\\t\\t\\t\\t\\t<div class=\\"loader\\" role=\\"status\\" aria-label=\\"Uploading\\"></div>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\t\\t<div class=\\"input-container\\">\\n\\t\\t\\t<Upload\\n\\t\\t\\t\\tbind:this={upload_component}\\n\\t\\t\\t\\ton:load={handle_upload}\\n\\t\\t\\t\\t{file_count}\\n\\t\\t\\t\\tfiletype={file_types}\\n\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t{max_file_size}\\n\\t\\t\\t\\tbind:dragging\\n\\t\\t\\t\\tbind:uploading\\n\\t\\t\\t\\tshow_progress={false}\\n\\t\\t\\t\\tdisable_click={true}\\n\\t\\t\\t\\tbind:hidden_upload\\n\\t\\t\\t\\ton:error\\n\\t\\t\\t\\thidden={true}\\n\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t{stream_handler}\\n\\t\\t\\t></Upload>\\n\\t\\t\\t<button\\n\\t\\t\\t\\tdata-testid=\\"upload-button\\"\\n\\t\\t\\t\\tclass=\\"upload-button\\"\\n\\t\\t\\t\\ton:click={handle_upload_click}><Paperclip /></button\\n\\t\\t\\t>\\n\\t\\t\\t<textarea\\n\\t\\t\\t\\tdata-testid=\\"textbox\\"\\n\\t\\t\\t\\tuse:text_area_resize={{\\n\\t\\t\\t\\t\\ttext: value.text,\\n\\t\\t\\t\\t\\tlines: lines,\\n\\t\\t\\t\\t\\tmax_lines: max_lines\\n\\t\\t\\t\\t}}\\n\\t\\t\\t\\tclass=\\"scroll-hide\\"\\n\\t\\t\\t\\tclass:no-label={!show_label}\\n\\t\\t\\t\\tdir={rtl ? \\"rtl\\" : \\"ltr\\"}\\n\\t\\t\\t\\tbind:value={value.text}\\n\\t\\t\\t\\tbind:this={el}\\n\\t\\t\\t\\t{placeholder}\\n\\t\\t\\t\\trows={lines}\\n\\t\\t\\t\\t{disabled}\\n\\t\\t\\t\\t{autofocus}\\n\\t\\t\\t\\ton:keypress={handle_keypress}\\n\\t\\t\\t\\ton:blur\\n\\t\\t\\t\\ton:select={handle_select}\\n\\t\\t\\t\\ton:focus\\n\\t\\t\\t\\ton:scroll={handle_scroll}\\n\\t\\t\\t\\ton:paste={handle_paste}\\n\\t\\t\\t\\tstyle={text_align ? \\"text-align: \\" + text_align : \\"\\"}\\n\\t\\t\\t/>\\n\\t\\t\\t{#if submit_btn}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"submit-button\\"\\n\\t\\t\\t\\t\\tclass:padded-button={submit_btn !== true}\\n\\t\\t\\t\\t\\ton:click={handle_submit}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#if submit_btn === true}\\n\\t\\t\\t\\t\\t\\t<Send />\\n\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t{submit_btn}\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if stop_btn}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"stop-button\\"\\n\\t\\t\\t\\t\\tclass:padded-button={stop_btn !== true}\\n\\t\\t\\t\\t\\ton:click={handle_stop}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#if stop_btn === true}\\n\\t\\t\\t\\t\\t\\t<Square fill={\\"none\\"} stroke_width={2.5} />\\n\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t{stop_btn}\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t{/if}\\n\\t\\t</div>\\n\\t</label>\\n</div>\\n\\n<style>\\n\\t.full-container {\\n\\t\\twidth: 100%;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t.full-container.dragging::after {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\ttop: 0;\\n\\t\\tleft: 0;\\n\\t\\tright: 0;\\n\\t\\tbottom: 0;\\n\\t\\tpointer-events: none;\\n\\t}\\n\\n\\t.input-container {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\talign-items: flex-end;\\n\\t}\\n\\n\\ttextarea {\\n\\t\\tflex-grow: 1;\\n\\t\\toutline: none !important;\\n\\t\\tbackground: var(--block-background-fill);\\n\\t\\tpadding: var(--input-padding);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-weight: var(--input-text-weight);\\n\\t\\tfont-size: var(--input-text-size);\\n\\t\\tline-height: var(--line-sm);\\n\\t\\tborder: none;\\n\\t\\tmargin-top: 0px;\\n\\t\\tmargin-bottom: 0px;\\n\\t\\tresize: none;\\n\\t\\tposition: relative;\\n\\t\\tz-index: 1;\\n\\t}\\n\\ttextarea.no-label {\\n\\t\\tpadding-top: 5px;\\n\\t\\tpadding-bottom: 5px;\\n\\t}\\n\\n\\ttextarea:disabled {\\n\\t\\t-webkit-opacity: 1;\\n\\t\\topacity: 1;\\n\\t}\\n\\n\\ttextarea::placeholder {\\n\\t\\tcolor: var(--input-placeholder-color);\\n\\t}\\n\\n\\t.upload-button,\\n\\t.submit-button,\\n\\t.stop-button {\\n\\t\\tborder: none;\\n\\t\\ttext-align: center;\\n\\t\\ttext-decoration: none;\\n\\t\\tfont-size: 14px;\\n\\t\\tcursor: pointer;\\n\\t\\tborder-radius: 15px;\\n\\t\\tmin-width: 30px;\\n\\t\\theight: 30px;\\n\\t\\tflex-shrink: 0;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tz-index: var(--layer-1);\\n\\t}\\n\\t.padded-button {\\n\\t\\tpadding: 0 10px;\\n\\t}\\n\\n\\t.stop-button,\\n\\t.upload-button,\\n\\t.submit-button {\\n\\t\\tbackground: var(--button-secondary-background-fill);\\n\\t}\\n\\n\\t.stop-button:hover,\\n\\t.upload-button:hover,\\n\\t.submit-button:hover {\\n\\t\\tbackground: var(--button-secondary-background-fill-hover);\\n\\t}\\n\\n\\t.stop-button:disabled,\\n\\t.upload-button:disabled,\\n\\t.submit-button:disabled {\\n\\t\\tbackground: var(--button-secondary-background-fill);\\n\\t\\tcursor: initial;\\n\\t}\\n\\t.stop-button:active,\\n\\t.upload-button:active,\\n\\t.submit-button:active {\\n\\t\\tbox-shadow: var(--button-shadow-active);\\n\\t}\\n\\n\\t.submit-button :global(svg) {\\n\\t\\theight: 22px;\\n\\t\\twidth: 22px;\\n\\t}\\n\\t.upload-button :global(svg) {\\n\\t\\theight: 17px;\\n\\t\\twidth: 17px;\\n\\t}\\n\\n\\t.stop-button :global(svg) {\\n\\t\\theight: 16px;\\n\\t\\twidth: 16px;\\n\\t}\\n\\n\\t.loader {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tborder: 5px solid #f3f3f3;\\n\\t\\tborder-top: 5px solid var(--color-accent);\\n\\t\\tborder-radius: 50%;\\n\\t\\twidth: 25px;\\n\\t\\theight: 25px;\\n\\t\\tanimation: spin 2s linear infinite;\\n\\t}\\n\\n\\t@keyframes spin {\\n\\t\\t0% {\\n\\t\\t\\ttransform: rotate(0deg);\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\ttransform: rotate(360deg);\\n\\t\\t}\\n\\t}\\n\\n\\t.thumbnails :global(img) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: cover;\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t}\\n\\n\\t.thumbnails {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--spacing-lg);\\n\\t\\toverflow-x: scroll;\\n\\t\\tpadding-top: var(--spacing-sm);\\n\\t\\tmargin-bottom: 6px;\\n\\t}\\n\\n\\t.thumbnail-item {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow:\\n\\t\\t\\t0 0 0 2px var(--ring-color),\\n\\t\\t\\tvar(--shadow-drop);\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\taspect-ratio: var(--ratio-square);\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tcursor: default;\\n\\t}\\n\\n\\t.thumbnail-small {\\n\\t\\tflex: none;\\n\\t\\ttransform: scale(0.9);\\n\\t\\ttransition: 0.075s;\\n\\t\\twidth: var(--size-12);\\n\\t\\theight: var(--size-12);\\n\\t}\\n\\n\\t.thumbnail-item :global(svg) {\\n\\t\\twidth: 30px;\\n\\t\\theight: 30px;\\n\\t}\\n\\n\\t.delete-button {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tposition: absolute;\\n\\t\\tright: -7px;\\n\\t\\ttop: -7px;\\n\\t\\tcolor: var(--button-secondary-text-color);\\n\\t\\tbackground: var(--button-secondary-background-fill);\\n\\t\\tborder: none;\\n\\t\\ttext-align: center;\\n\\t\\ttext-decoration: none;\\n\\t\\tfont-size: 10px;\\n\\t\\tcursor: pointer;\\n\\t\\tborder-radius: 50%;\\n\\t\\twidth: 20px;\\n\\t\\theight: 20px;\\n\\t}\\n\\n\\t.disabled {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.delete-button :global(svg) {\\n\\t\\twidth: 12px;\\n\\t\\theight: 12px;\\n\\t}\\n\\n\\t.delete-button:hover {\\n\\t\\tfilter: brightness(1.2);\\n\\t\\tborder: 0.8px solid var(--color-grey-500);\\n\\t}</style>\\n"],"names":[],"mappings":"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"}`
};
const MultimodalTextbox = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value = { text: "", files: [] } } = $$props;
  let { value_is_output = false } = $$props;
  let { lines = 1 } = $$props;
  let { placeholder = "Type here..." } = $$props;
  let { disabled = false } = $$props;
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { show_label = true } = $$props;
  let { container = true } = $$props;
  let { max_lines } = $$props;
  let { submit_btn = null } = $$props;
  let { stop_btn = null } = $$props;
  let { rtl = false } = $$props;
  let { autofocus = false } = $$props;
  let { text_align = void 0 } = $$props;
  let { autoscroll = true } = $$props;
  let { root } = $$props;
  let { file_types = null } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { file_count = "multiple" } = $$props;
  let upload_component;
  let hidden_upload;
  let el;
  let { dragging = false } = $$props;
  let uploading = false;
  let oldValue = value.text;
  let full_container;
  const dispatch = createEventDispatcher();
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.lines === void 0 && $$bindings.lines && lines !== void 0)
    $$bindings.lines(lines);
  if ($$props.placeholder === void 0 && $$bindings.placeholder && placeholder !== void 0)
    $$bindings.placeholder(placeholder);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.max_lines === void 0 && $$bindings.max_lines && max_lines !== void 0)
    $$bindings.max_lines(max_lines);
  if ($$props.submit_btn === void 0 && $$bindings.submit_btn && submit_btn !== void 0)
    $$bindings.submit_btn(submit_btn);
  if ($$props.stop_btn === void 0 && $$bindings.stop_btn && stop_btn !== void 0)
    $$bindings.stop_btn(stop_btn);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.autofocus === void 0 && $$bindings.autofocus && autofocus !== void 0)
    $$bindings.autofocus(autofocus);
  if ($$props.text_align === void 0 && $$bindings.text_align && text_align !== void 0)
    $$bindings.text_align(text_align);
  if ($$props.autoscroll === void 0 && $$bindings.autoscroll && autoscroll !== void 0)
    $$bindings.autoscroll(autoscroll);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.file_types === void 0 && $$bindings.file_types && file_types !== void 0)
    $$bindings.file_types(file_types);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
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
      dispatch("drag", dragging);
    }
    {
      if (value === null)
        value = { text: "", files: [] };
    }
    {
      if (oldValue !== value.text) {
        dispatch("change", value);
        oldValue = value.text;
      }
    }
    $$rendered = `<div class="${["full-container svelte-1d7elt4", dragging ? "dragging" : ""].join(" ").trim()}" role="group" aria-label="Multimedia input field"${add_attribute("this", full_container, 0)}> <label${add_classes((container ? "container" : "").trim())}>${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
      default: () => {
        return `${escape(label)}`;
      }
    })} ${value.files.length > 0 || uploading ? `<div class="thumbnails scroll-hide svelte-1d7elt4" aria-label="Uploaded files" data-testid="container_el" style="${"display: " + escape(value.files.length > 0 || uploading ? "flex" : "none", true) + ";"}">${each(value.files, (file, index) => {
      return `<span role="listitem" aria-label="File thumbnail"><button class="thumbnail-item thumbnail-small svelte-1d7elt4"><button class="${["delete-button svelte-1d7elt4", disabled ? "disabled" : ""].join(" ").trim()}">${validate_component(Clear, "Clear").$$render($$result, {}, {}, {})}</button> ${file.mime_type && file.mime_type.includes("image") ? `${validate_component(Image$1, "Image").$$render(
        $$result,
        {
          src: file.url,
          title: null,
          alt: "",
          loading: "lazy",
          class: "thumbnail-image"
        },
        {},
        {}
      )}` : `${file.mime_type && file.mime_type.includes("audio") ? `${validate_component(Music, "Music").$$render($$result, {}, {}, {})}` : `${file.mime_type && file.mime_type.includes("video") ? `${validate_component(Video, "Video").$$render($$result, {}, {}, {})}` : `${validate_component(File$1, "File").$$render($$result, {}, {}, {})}`}`}`}</button> </span>`;
    })} ${uploading ? `<div class="loader svelte-1d7elt4" role="status" aria-label="Uploading"></div>` : ``}</div>` : ``} <div class="input-container svelte-1d7elt4">${validate_component(Upload, "Upload").$$render(
      $$result,
      {
        file_count,
        filetype: file_types,
        root,
        max_file_size,
        show_progress: false,
        disable_click: true,
        hidden: true,
        upload,
        stream_handler,
        this: upload_component,
        dragging,
        uploading,
        hidden_upload
      },
      {
        this: ($$value) => {
          upload_component = $$value;
          $$settled = false;
        },
        dragging: ($$value) => {
          dragging = $$value;
          $$settled = false;
        },
        uploading: ($$value) => {
          uploading = $$value;
          $$settled = false;
        },
        hidden_upload: ($$value) => {
          hidden_upload = $$value;
          $$settled = false;
        }
      },
      {}
    )} <button data-testid="upload-button" class="upload-button svelte-1d7elt4">${validate_component(Paperclip, "Paperclip").$$render($$result, {}, {}, {})}</button> <textarea data-testid="textbox" class="${["scroll-hide svelte-1d7elt4", !show_label ? "no-label" : ""].join(" ").trim()}"${add_attribute("dir", rtl ? "rtl" : "ltr", 0)}${add_attribute("placeholder", placeholder, 0)}${add_attribute("rows", lines, 0)} ${disabled ? "disabled" : ""} ${autofocus ? "autofocus" : ""}${add_attribute("style", text_align ? "text-align: " + text_align : "", 0)}${add_attribute("this", el, 0)}>${escape(value.text || "")}</textarea> ${submit_btn ? `<button class="${["submit-button svelte-1d7elt4", submit_btn !== true ? "padded-button" : ""].join(" ").trim()}">${submit_btn === true ? `${validate_component(Send, "Send").$$render($$result, {}, {}, {})}` : `${escape(submit_btn)}`}</button>` : ``} ${stop_btn ? `<button class="${["stop-button svelte-1d7elt4", stop_btn !== true ? "padded-button" : ""].join(" ").trim()}">${stop_btn === true ? `${validate_component(Square, "Square").$$render($$result, { fill: "none", stroke_width: 2.5 }, {}, {})}` : `${escape(stop_btn)}`}</button>` : ``}</div></label> </div>`;
  } while (!$$settled);
  return $$rendered;
});
const MultimodalTextbox$1 = MultimodalTextbox;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { gradio } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = { text: "", files: [] } } = $$props;
  let { file_types = null } = $$props;
  let { lines } = $$props;
  let { placeholder = "" } = $$props;
  let { label = "MultimodalTextbox" } = $$props;
  let { info = void 0 } = $$props;
  let { show_label } = $$props;
  let { max_lines } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { submit_btn = null } = $$props;
  let { stop_btn = null } = $$props;
  let { loading_status = void 0 } = $$props;
  let { value_is_output = false } = $$props;
  let { rtl = false } = $$props;
  let { text_align = void 0 } = $$props;
  let { autofocus = false } = $$props;
  let { autoscroll = true } = $$props;
  let { interactive } = $$props;
  let { root } = $$props;
  let { file_count } = $$props;
  let dragging;
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
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
  if ($$props.lines === void 0 && $$bindings.lines && lines !== void 0)
    $$bindings.lines(lines);
  if ($$props.placeholder === void 0 && $$bindings.placeholder && placeholder !== void 0)
    $$bindings.placeholder(placeholder);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.max_lines === void 0 && $$bindings.max_lines && max_lines !== void 0)
    $$bindings.max_lines(max_lines);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.submit_btn === void 0 && $$bindings.submit_btn && submit_btn !== void 0)
    $$bindings.submit_btn(submit_btn);
  if ($$props.stop_btn === void 0 && $$bindings.stop_btn && stop_btn !== void 0)
    $$bindings.stop_btn(stop_btn);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.text_align === void 0 && $$bindings.text_align && text_align !== void 0)
    $$bindings.text_align(text_align);
  if ($$props.autofocus === void 0 && $$bindings.autofocus && autofocus !== void 0)
    $$bindings.autofocus(autofocus);
  if ($$props.autoscroll === void 0 && $$bindings.autoscroll && autoscroll !== void 0)
    $$bindings.autoscroll(autoscroll);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `   ${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        elem_id,
        elem_classes: [...elem_classes, "multimodal-textbox"],
        scale,
        min_width,
        allow_overflow: false,
        padding: container,
        border_mode: dragging ? "focus" : "base"
      },
      {},
      {
        default: () => {
          return `${loading_status ? `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})}` : ``} ${validate_component(MultimodalTextbox$1, "MultimodalTextbox").$$render(
            $$result,
            {
              file_types,
              root,
              label,
              info,
              show_label,
              lines,
              rtl,
              text_align,
              max_lines: !max_lines ? lines + 1 : max_lines,
              placeholder,
              submit_btn,
              stop_btn,
              autofocus,
              container,
              autoscroll,
              file_count,
              max_file_size: gradio.max_file_size,
              disabled: !interactive,
              upload: (...args) => gradio.client.upload(...args),
              stream_handler: (...args) => gradio.client.stream(...args),
              value,
              value_is_output,
              dragging
            },
            {
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              },
              value_is_output: ($$value) => {
                value_is_output = $$value;
                $$settled = false;
              },
              dragging: ($$value) => {
                dragging = $$value;
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

export { MultimodalTextbox$1 as BaseMultimodalTextbox, Index as default };
//# sourceMappingURL=Index38-Bxml_IQW.js.map
