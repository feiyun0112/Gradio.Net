import { t as tick } from './Component-Dv7eSVA_.js';
import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, b as add_attribute, d as add_styles, o as onDestroy, e as escape } from './ssr-RaXq3SJh.js';
import { h as IconButtonWrapper, i as IconButton, Y as Edit, v as Undo, D as Download, Z as Clear, n as prepare_files } from './2-B6LMYTAg.js';
import { D as DownloadLink } from './DownloadLink--4obEanq.js';

const css$1 = {
  code: '.wrap.svelte-1vsfomn.svelte-1vsfomn{overflow-y:auto;transition:opacity 0.5s ease-in-out;background:var(--block-background-fill);position:relative;display:flex;flex-direction:column;align-items:center;justify-content:center;min-height:var(--size-40);width:var(--size-full)}.wrap.svelte-1vsfomn.svelte-1vsfomn::after{content:"";position:absolute;top:0;left:0;width:var(--upload-progress-width);height:100%;transition:all 0.5s ease-in-out;z-index:1}.uploading.svelte-1vsfomn.svelte-1vsfomn{font-size:var(--text-lg);font-family:var(--font);z-index:2}.file-name.svelte-1vsfomn.svelte-1vsfomn{margin:var(--spacing-md);font-size:var(--text-lg);color:var(--body-text-color-subdued)}.file.svelte-1vsfomn.svelte-1vsfomn{font-size:var(--text-md);z-index:2;display:flex;align-items:center}.file.svelte-1vsfomn progress.svelte-1vsfomn{display:inline;height:var(--size-1);width:100%;transition:all 0.5s ease-in-out;color:var(--color-accent);border:none}.file.svelte-1vsfomn progress[value].svelte-1vsfomn::-webkit-progress-value{background-color:var(--color-accent);border-radius:20px}.file.svelte-1vsfomn progress[value].svelte-1vsfomn::-webkit-progress-bar{background-color:var(--border-color-accent);border-radius:20px}.progress-bar.svelte-1vsfomn.svelte-1vsfomn{width:14px;height:14px;border-radius:50%;background:radial-gradient(\n				closest-side,\n				var(--block-background-fill) 64%,\n				transparent 53% 100%\n			),\n			conic-gradient(\n				var(--color-accent) var(--upload-progress-width),\n				var(--border-color-accent) 0\n			);transition:all 0.5s ease-in-out}',
  map: '{"version":3,"file":"UploadProgress.svelte","sources":["UploadProgress.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { FileData } from \\"@gradio/client\\";\\nimport { onMount, createEventDispatcher, onDestroy } from \\"svelte\\";\\nexport let upload_id;\\nexport let root;\\nexport let files;\\nexport let stream_handler;\\nlet stream;\\nlet progress = false;\\nlet current_file_upload;\\nlet file_to_display;\\nlet files_with_progress = files.map((file) => {\\n    return {\\n        ...file,\\n        progress: 0\\n    };\\n});\\nconst dispatch = createEventDispatcher();\\nfunction handleProgress(filename, chunk_size) {\\n    files_with_progress = files_with_progress.map((file) => {\\n        if (file.orig_name === filename) {\\n            file.progress += chunk_size;\\n        }\\n        return file;\\n    });\\n}\\nfunction getProgress(file) {\\n    return file.progress * 100 / (file.size || 0) || 0;\\n}\\nonMount(async () => {\\n    stream = await stream_handler(new URL(`${root}/gradio_api/upload_progress?upload_id=${upload_id}`));\\n    if (stream == null) {\\n        throw new Error(\\"Event source is not defined\\");\\n    }\\n    stream.onmessage = async function (event) {\\n        const _data = JSON.parse(event.data);\\n        if (!progress)\\n            progress = true;\\n        if (_data.msg === \\"done\\") {\\n            stream?.close();\\n            dispatch(\\"done\\");\\n        }\\n        else {\\n            current_file_upload = _data;\\n            handleProgress(_data.orig_name, _data.chunk_size);\\n        }\\n    };\\n});\\nonDestroy(() => {\\n    if (stream != null || stream != void 0)\\n        stream.close();\\n});\\nfunction calculateTotalProgress(files2) {\\n    let totalProgress = 0;\\n    files2.forEach((file) => {\\n        totalProgress += getProgress(file);\\n    });\\n    document.documentElement.style.setProperty(\\"--upload-progress-width\\", (totalProgress / files2.length).toFixed(2) + \\"%\\");\\n    return totalProgress / files2.length;\\n}\\n$: calculateTotalProgress(files_with_progress);\\n$: file_to_display = current_file_upload || files_with_progress[0];\\n<\/script>\\n\\n<div class=\\"wrap\\" class:progress>\\n\\t<span class=\\"uploading\\"\\n\\t\\t>Uploading {files_with_progress.length}\\n\\t\\t{files_with_progress.length > 1 ? \\"files\\" : \\"file\\"}...</span\\n\\t>\\n\\n\\t{#if file_to_display}\\n\\t\\t<div class=\\"file\\">\\n\\t\\t\\t<span>\\n\\t\\t\\t\\t<div class=\\"progress-bar\\">\\n\\t\\t\\t\\t\\t<progress\\n\\t\\t\\t\\t\\t\\tstyle=\\"visibility:hidden;height:0;width:0;\\"\\n\\t\\t\\t\\t\\t\\tvalue={getProgress(file_to_display)}\\n\\t\\t\\t\\t\\t\\tmax=\\"100\\">{getProgress(file_to_display)}</progress\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t</span>\\n\\t\\t\\t<span class=\\"file-name\\">\\n\\t\\t\\t\\t{file_to_display.orig_name}\\n\\t\\t\\t</span>\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.wrap {\\n\\t\\toverflow-y: auto;\\n\\t\\ttransition: opacity 0.5s ease-in-out;\\n\\t\\tbackground: var(--block-background-fill);\\n\\t\\tposition: relative;\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\tmin-height: var(--size-40);\\n\\t\\twidth: var(--size-full);\\n\\t}\\n\\n\\t.wrap::after {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\ttop: 0;\\n\\t\\tleft: 0;\\n\\t\\twidth: var(--upload-progress-width);\\n\\t\\theight: 100%;\\n\\t\\ttransition: all 0.5s ease-in-out;\\n\\t\\tz-index: 1;\\n\\t}\\n\\n\\t.uploading {\\n\\t\\tfont-size: var(--text-lg);\\n\\t\\tfont-family: var(--font);\\n\\t\\tz-index: 2;\\n\\t}\\n\\n\\t.file-name {\\n\\t\\tmargin: var(--spacing-md);\\n\\t\\tfont-size: var(--text-lg);\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\n\\t.file {\\n\\t\\tfont-size: var(--text-md);\\n\\t\\tz-index: 2;\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t}\\n\\n\\t.file progress {\\n\\t\\tdisplay: inline;\\n\\t\\theight: var(--size-1);\\n\\t\\twidth: 100%;\\n\\t\\ttransition: all 0.5s ease-in-out;\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tborder: none;\\n\\t}\\n\\n\\t.file progress[value]::-webkit-progress-value {\\n\\t\\tbackground-color: var(--color-accent);\\n\\t\\tborder-radius: 20px;\\n\\t}\\n\\n\\t.file progress[value]::-webkit-progress-bar {\\n\\t\\tbackground-color: var(--border-color-accent);\\n\\t\\tborder-radius: 20px;\\n\\t}\\n\\n\\t.progress-bar {\\n\\t\\twidth: 14px;\\n\\t\\theight: 14px;\\n\\t\\tborder-radius: 50%;\\n\\t\\tbackground: radial-gradient(\\n\\t\\t\\t\\tclosest-side,\\n\\t\\t\\t\\tvar(--block-background-fill) 64%,\\n\\t\\t\\t\\ttransparent 53% 100%\\n\\t\\t\\t),\\n\\t\\t\\tconic-gradient(\\n\\t\\t\\t\\tvar(--color-accent) var(--upload-progress-width),\\n\\t\\t\\t\\tvar(--border-color-accent) 0\\n\\t\\t\\t);\\n\\t\\ttransition: all 0.5s ease-in-out;\\n\\t}</style>\\n"],"names":[],"mappings":"AAwFC,mCAAM,CACL,UAAU,CAAE,IAAI,CAChB,UAAU,CAAE,OAAO,CAAC,IAAI,CAAC,WAAW,CACpC,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,QAAQ,CAAE,QAAQ,CAClB,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CACvB,UAAU,CAAE,IAAI,SAAS,CAAC,CAC1B,KAAK,CAAE,IAAI,WAAW,CACvB,CAEA,mCAAK,OAAQ,CACZ,OAAO,CAAE,EAAE,CACX,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,CAAC,CACN,IAAI,CAAE,CAAC,CACP,KAAK,CAAE,IAAI,uBAAuB,CAAC,CACnC,MAAM,CAAE,IAAI,CACZ,UAAU,CAAE,GAAG,CAAC,IAAI,CAAC,WAAW,CAChC,OAAO,CAAE,CACV,CAEA,wCAAW,CACV,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,WAAW,CAAE,IAAI,MAAM,CAAC,CACxB,OAAO,CAAE,CACV,CAEA,wCAAW,CACV,MAAM,CAAE,IAAI,YAAY,CAAC,CACzB,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,KAAK,CAAE,IAAI,yBAAyB,CACrC,CAEA,mCAAM,CACL,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,OAAO,CAAE,CAAC,CACV,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MACd,CAEA,oBAAK,CAAC,uBAAS,CACd,OAAO,CAAE,MAAM,CACf,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,GAAG,CAAC,IAAI,CAAC,WAAW,CAChC,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,MAAM,CAAE,IACT,CAEA,oBAAK,CAAC,QAAQ,CAAC,KAAK,gBAAC,wBAAyB,CAC7C,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,aAAa,CAAE,IAChB,CAEA,oBAAK,CAAC,QAAQ,CAAC,KAAK,gBAAC,sBAAuB,CAC3C,gBAAgB,CAAE,IAAI,qBAAqB,CAAC,CAC5C,aAAa,CAAE,IAChB,CAEA,2CAAc,CACb,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,aAAa,CAAE,GAAG,CAClB,UAAU,CAAE;AACd,IAAI,YAAY,CAAC;AACjB,IAAI,IAAI,uBAAuB,CAAC,CAAC,GAAG,CAAC;AACrC,IAAI,WAAW,CAAC,GAAG,CAAC,IAAI;AACxB,IAAI,CAAC;AACL,GAAG;AACH,IAAI,IAAI,cAAc,CAAC,CAAC,IAAI,uBAAuB,CAAC,CAAC;AACrD,IAAI,IAAI,qBAAqB,CAAC,CAAC,CAAC;AAChC,IAAI,CACF,UAAU,CAAE,GAAG,CAAC,IAAI,CAAC,WACtB"}'
};
function getProgress(file) {
  return file.progress * 100 / (file.size || 0) || 0;
}
function calculateTotalProgress(files2) {
  let totalProgress = 0;
  files2.forEach((file) => {
    totalProgress += getProgress(file);
  });
  document.documentElement.style.setProperty("--upload-progress-width", (totalProgress / files2.length).toFixed(2) + "%");
  return totalProgress / files2.length;
}
const UploadProgress = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { upload_id } = $$props;
  let { root } = $$props;
  let { files } = $$props;
  let { stream_handler } = $$props;
  let file_to_display;
  let files_with_progress = files.map((file) => {
    return { ...file, progress: 0 };
  });
  createEventDispatcher();
  onDestroy(() => {
  });
  if ($$props.upload_id === void 0 && $$bindings.upload_id && upload_id !== void 0)
    $$bindings.upload_id(upload_id);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.files === void 0 && $$bindings.files && files !== void 0)
    $$bindings.files(files);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  $$result.css.add(css$1);
  {
    calculateTotalProgress(files_with_progress);
  }
  file_to_display = files_with_progress[0];
  return `<div class="${["wrap svelte-1vsfomn", ""].join(" ").trim()}"><span class="uploading svelte-1vsfomn">Uploading ${escape(files_with_progress.length)} ${escape(files_with_progress.length > 1 ? "files" : "file")}...</span> ${file_to_display ? `<div class="file svelte-1vsfomn"><span><div class="progress-bar svelte-1vsfomn"><progress style="visibility:hidden;height:0;width:0;"${add_attribute("value", getProgress(file_to_display), 0)} max="100" class="svelte-1vsfomn">${escape(getProgress(file_to_display))}</progress></div></span> <span class="file-name svelte-1vsfomn">${escape(file_to_display.orig_name)}</span></div>` : ``} </div>`;
});
const css = {
  code: "button.svelte-1b742ao{cursor:pointer;width:var(--size-full)}.center.svelte-1b742ao{display:flex;justify-content:center}.flex.svelte-1b742ao{display:flex;flex-direction:column;justify-content:center;align-items:center}.hidden.svelte-1b742ao{display:none;position:absolute;flex-grow:0}.hidden.svelte-1b742ao svg{display:none}.disable_click.svelte-1b742ao{cursor:default}input.svelte-1b742ao{display:none}.icon-mode.svelte-1b742ao{position:absolute !important;width:var(--size-4);height:var(--size-4);padding:0;min-height:0;border-radius:var(--radius-circle)}.icon-mode.svelte-1b742ao svg{width:var(--size-4);height:var(--size-4)}",
  map: '{"version":3,"file":"Upload.svelte","sources":["Upload.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, tick, getContext } from \\"svelte\\";\\nimport { prepare_files } from \\"@gradio/client\\";\\nimport { _ } from \\"svelte-i18n\\";\\nimport UploadProgress from \\"./UploadProgress.svelte\\";\\nexport let filetype = null;\\nexport let dragging = false;\\nexport let boundedheight = true;\\nexport let center = true;\\nexport let flex = true;\\nexport let file_count = \\"single\\";\\nexport let disable_click = false;\\nexport let root;\\nexport let hidden = false;\\nexport let format = \\"file\\";\\nexport let uploading = false;\\nexport let hidden_upload = null;\\nexport let show_progress = true;\\nexport let max_file_size = null;\\nexport let upload;\\nexport let stream_handler;\\nexport let icon_upload = false;\\nlet upload_id;\\nlet file_data;\\nlet accept_file_types;\\nlet use_post_upload_validation = null;\\nconst get_ios = () => {\\n    if (typeof navigator !== \\"undefined\\") {\\n        const userAgent = navigator.userAgent.toLowerCase();\\n        return userAgent.indexOf(\\"iphone\\") > -1 || userAgent.indexOf(\\"ipad\\") > -1;\\n    }\\n    return false;\\n};\\n$: ios = get_ios();\\nconst dispatch = createEventDispatcher();\\nconst validFileTypes = [\\"image\\", \\"video\\", \\"audio\\", \\"text\\", \\"file\\"];\\nconst process_file_type = (type) => {\\n    if (ios && type.startsWith(\\".\\")) {\\n        use_post_upload_validation = true;\\n        return type;\\n    }\\n    if (ios && type.includes(\\"file/*\\")) {\\n        return \\"*\\";\\n    }\\n    if (type.startsWith(\\".\\") || type.endsWith(\\"/*\\")) {\\n        return type;\\n    }\\n    if (validFileTypes.includes(type)) {\\n        return type + \\"/*\\";\\n    }\\n    return \\".\\" + type;\\n};\\n$: if (filetype == null) {\\n    accept_file_types = null;\\n}\\nelse if (typeof filetype === \\"string\\") {\\n    accept_file_types = process_file_type(filetype);\\n}\\nelse if (ios && filetype.includes(\\"file/*\\")) {\\n    accept_file_types = \\"*\\";\\n}\\nelse {\\n    filetype = filetype.map(process_file_type);\\n    accept_file_types = filetype.join(\\", \\");\\n}\\nfunction updateDragging() {\\n    dragging = !dragging;\\n}\\nexport function paste_clipboard() {\\n    navigator.clipboard.read().then(async (items) => {\\n        for (let i = 0; i < items.length; i++) {\\n            const type = items[i].types.find((t) => t.startsWith(\\"image/\\"));\\n            if (type) {\\n                items[i].getType(type).then(async (blob) => {\\n                    const file = new File([blob], `clipboard.${type.replace(\\"image/\\", \\"\\")}`);\\n                    await load_files([file]);\\n                });\\n                break;\\n            }\\n        }\\n    });\\n}\\nexport function open_file_upload() {\\n    if (disable_click)\\n        return;\\n    if (hidden_upload) {\\n        hidden_upload.value = \\"\\";\\n        hidden_upload.click();\\n    }\\n}\\nasync function handle_upload(file_data2) {\\n    await tick();\\n    upload_id = Math.random().toString(36).substring(2, 15);\\n    uploading = true;\\n    try {\\n        const _file_data = await upload(file_data2, root, upload_id, max_file_size ?? Infinity);\\n        dispatch(\\"load\\", file_count === \\"single\\" ? _file_data?.[0] : _file_data);\\n        uploading = false;\\n        return _file_data || [];\\n    }\\n    catch (e) {\\n        dispatch(\\"error\\", e.message);\\n        uploading = false;\\n        return [];\\n    }\\n}\\nexport async function load_files(files) {\\n    if (!files.length) {\\n        return;\\n    }\\n    let _files = files.map((f) => new File([f], f instanceof File ? f.name : \\"file\\", { type: f.type }));\\n    if (ios && use_post_upload_validation) {\\n        _files = _files.filter((file) => {\\n            if (is_valid_file(file)) {\\n                return true;\\n            }\\n            dispatch(\\"error\\", `Invalid file type: ${file.name}. Only ${filetype} allowed.`);\\n            return false;\\n        });\\n        if (_files.length === 0) {\\n            return [];\\n        }\\n    }\\n    file_data = await prepare_files(_files);\\n    return await handle_upload(file_data);\\n}\\nfunction is_valid_file(file) {\\n    if (!filetype)\\n        return true;\\n    const allowed_types = Array.isArray(filetype) ? filetype : [filetype];\\n    return allowed_types.some((type) => {\\n        const processed_type = process_file_type(type);\\n        if (processed_type.startsWith(\\".\\")) {\\n            return file.name.toLowerCase().endsWith(processed_type.toLowerCase());\\n        }\\n        if (processed_type === \\"*\\") {\\n            return true;\\n        }\\n        if (processed_type.endsWith(\\"/*\\")) {\\n            const [category] = processed_type.split(\\"/\\");\\n            return file.type.startsWith(category + \\"/\\");\\n        }\\n        return file.type === processed_type;\\n    });\\n}\\nasync function load_files_from_upload(e) {\\n    const target = e.target;\\n    if (!target.files)\\n        return;\\n    if (format != \\"blob\\") {\\n        await load_files(Array.from(target.files));\\n    }\\n    else {\\n        if (file_count === \\"single\\") {\\n            dispatch(\\"load\\", target.files[0]);\\n            return;\\n        }\\n        dispatch(\\"load\\", target.files);\\n    }\\n}\\nfunction is_valid_mimetype(file_accept, uploaded_file_extension, uploaded_file_type) {\\n    if (!file_accept || file_accept === \\"*\\" || file_accept === \\"file/*\\" || Array.isArray(file_accept) && file_accept.some((accept) => accept === \\"*\\" || accept === \\"file/*\\")) {\\n        return true;\\n    }\\n    let acceptArray;\\n    if (typeof file_accept === \\"string\\") {\\n        acceptArray = file_accept.split(\\",\\").map((s) => s.trim());\\n    }\\n    else if (Array.isArray(file_accept)) {\\n        acceptArray = file_accept;\\n    }\\n    else {\\n        return false;\\n    }\\n    return acceptArray.includes(uploaded_file_extension) || acceptArray.some((type) => {\\n        const [category] = type.split(\\"/\\").map((s) => s.trim());\\n        return type.endsWith(\\"/*\\") && uploaded_file_type.startsWith(category + \\"/\\");\\n    });\\n}\\nasync function loadFilesFromDrop(e) {\\n    dragging = false;\\n    if (!e.dataTransfer?.files)\\n        return;\\n    const files_to_load = Array.from(e.dataTransfer.files).filter((file) => {\\n        const file_extension = \\".\\" + file.name.split(\\".\\").pop();\\n        if (file_extension && is_valid_mimetype(accept_file_types, file_extension, file.type)) {\\n            return true;\\n        }\\n        if (file_extension && Array.isArray(filetype) ? filetype.includes(file_extension) : file_extension === filetype) {\\n            return true;\\n        }\\n        dispatch(\\"error\\", `Invalid file type only ${filetype} allowed.`);\\n        return false;\\n    });\\n    if (format != \\"blob\\") {\\n        await load_files(files_to_load);\\n    }\\n    else {\\n        if (file_count === \\"single\\") {\\n            dispatch(\\"load\\", files_to_load[0]);\\n            return;\\n        }\\n        dispatch(\\"load\\", files_to_load);\\n    }\\n}\\n<\/script>\\n\\n{#if filetype === \\"clipboard\\"}\\n\\t<button\\n\\t\\tclass:hidden\\n\\t\\tclass:center\\n\\t\\tclass:boundedheight\\n\\t\\tclass:flex\\n\\t\\tclass:icon-mode={icon_upload}\\n\\t\\tstyle:height={icon_upload ? \\"\\" : \\"100%\\"}\\n\\t\\ttabindex={hidden ? -1 : 0}\\n\\t\\ton:click={paste_clipboard}\\n\\t>\\n\\t\\t<slot />\\n\\t</button>\\n{:else if uploading && show_progress}\\n\\t{#if !hidden}\\n\\t\\t<UploadProgress {root} {upload_id} files={file_data} {stream_handler} />\\n\\t{/if}\\n{:else}\\n\\t<button\\n\\t\\tclass:hidden\\n\\t\\tclass:center\\n\\t\\tclass:boundedheight\\n\\t\\tclass:flex\\n\\t\\tclass:disable_click\\n\\t\\tclass:icon-mode={icon_upload}\\n\\t\\tstyle:height={icon_upload ? \\"\\" : \\"100%\\"}\\n\\t\\ttabindex={hidden ? -1 : 0}\\n\\t\\ton:drag|preventDefault|stopPropagation\\n\\t\\ton:dragstart|preventDefault|stopPropagation\\n\\t\\ton:dragend|preventDefault|stopPropagation\\n\\t\\ton:dragover|preventDefault|stopPropagation\\n\\t\\ton:dragenter|preventDefault|stopPropagation\\n\\t\\ton:dragleave|preventDefault|stopPropagation\\n\\t\\ton:drop|preventDefault|stopPropagation\\n\\t\\ton:click={open_file_upload}\\n\\t\\ton:drop={loadFilesFromDrop}\\n\\t\\ton:dragenter={updateDragging}\\n\\t\\ton:dragleave={updateDragging}\\n\\t>\\n\\t\\t<slot />\\n\\t\\t<input\\n\\t\\t\\taria-label=\\"file upload\\"\\n\\t\\t\\tdata-testid=\\"file-upload\\"\\n\\t\\t\\ttype=\\"file\\"\\n\\t\\t\\tbind:this={hidden_upload}\\n\\t\\t\\ton:change={load_files_from_upload}\\n\\t\\t\\taccept={accept_file_types || undefined}\\n\\t\\t\\tmultiple={file_count === \\"multiple\\" || undefined}\\n\\t\\t\\twebkitdirectory={file_count === \\"directory\\" || undefined}\\n\\t\\t\\tmozdirectory={file_count === \\"directory\\" || undefined}\\n\\t\\t/>\\n\\t</button>\\n{/if}\\n\\n<style>\\n\\tbutton {\\n\\t\\tcursor: pointer;\\n\\t\\twidth: var(--size-full);\\n\\t}\\n\\n\\t.center {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t}\\n\\t.flex {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t}\\n\\t.hidden {\\n\\t\\tdisplay: none;\\n\\t\\tposition: absolute;\\n\\t\\tflex-grow: 0;\\n\\t}\\n\\n\\t.hidden :global(svg) {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.disable_click {\\n\\t\\tcursor: default;\\n\\t}\\n\\n\\tinput {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.icon-mode {\\n\\t\\tposition: absolute !important;\\n\\t\\twidth: var(--size-4);\\n\\t\\theight: var(--size-4);\\n\\t\\tpadding: 0;\\n\\t\\tmin-height: 0;\\n\\t\\tborder-radius: var(--radius-circle);\\n\\t}\\n\\n\\t.icon-mode :global(svg) {\\n\\t\\twidth: var(--size-4);\\n\\t\\theight: var(--size-4);\\n\\t}</style>\\n"],"names":[],"mappings":"AAqQC,qBAAO,CACN,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,WAAW,CACvB,CAEA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAClB,CACA,oBAAM,CACL,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MACd,CACA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,SAAS,CAAE,CACZ,CAEA,sBAAO,CAAS,GAAK,CACpB,OAAO,CAAE,IACV,CAEA,6BAAe,CACd,MAAM,CAAE,OACT,CAEA,oBAAM,CACL,OAAO,CAAE,IACV,CAEA,yBAAW,CACV,QAAQ,CAAE,QAAQ,CAAC,UAAU,CAC7B,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,OAAO,CAAE,CAAC,CACV,UAAU,CAAE,CAAC,CACb,aAAa,CAAE,IAAI,eAAe,CACnC,CAEA,yBAAU,CAAS,GAAK,CACvB,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,MAAM,CAAE,IAAI,QAAQ,CACrB"}'
};
const Upload = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let ios;
  let { filetype = null } = $$props;
  let { dragging = false } = $$props;
  let { boundedheight = true } = $$props;
  let { center = true } = $$props;
  let { flex = true } = $$props;
  let { file_count = "single" } = $$props;
  let { disable_click = false } = $$props;
  let { root } = $$props;
  let { hidden = false } = $$props;
  let { format = "file" } = $$props;
  let { uploading = false } = $$props;
  let { hidden_upload = null } = $$props;
  let { show_progress = true } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { icon_upload = false } = $$props;
  let upload_id;
  let file_data;
  let accept_file_types;
  let use_post_upload_validation = null;
  const get_ios = () => {
    if (typeof navigator !== "undefined") {
      const userAgent = navigator.userAgent.toLowerCase();
      return userAgent.indexOf("iphone") > -1 || userAgent.indexOf("ipad") > -1;
    }
    return false;
  };
  const dispatch = createEventDispatcher();
  const validFileTypes = ["image", "video", "audio", "text", "file"];
  const process_file_type = (type) => {
    if (ios && type.startsWith(".")) {
      use_post_upload_validation = true;
      return type;
    }
    if (ios && type.includes("file/*")) {
      return "*";
    }
    if (type.startsWith(".") || type.endsWith("/*")) {
      return type;
    }
    if (validFileTypes.includes(type)) {
      return type + "/*";
    }
    return "." + type;
  };
  function paste_clipboard() {
    navigator.clipboard.read().then(async (items) => {
      for (let i = 0; i < items.length; i++) {
        const type = items[i].types.find((t) => t.startsWith("image/"));
        if (type) {
          items[i].getType(type).then(async (blob) => {
            const file = new File([blob], `clipboard.${type.replace("image/", "")}`);
            await load_files([file]);
          });
          break;
        }
      }
    });
  }
  function open_file_upload() {
    if (disable_click)
      return;
    if (hidden_upload) {
      hidden_upload.value = "";
      hidden_upload.click();
    }
  }
  async function handle_upload(file_data2) {
    await tick();
    upload_id = Math.random().toString(36).substring(2, 15);
    uploading = true;
    try {
      const _file_data = await upload(file_data2, root, upload_id, max_file_size ?? Infinity);
      dispatch("load", file_count === "single" ? _file_data?.[0] : _file_data);
      uploading = false;
      return _file_data || [];
    } catch (e) {
      dispatch("error", e.message);
      uploading = false;
      return [];
    }
  }
  async function load_files(files) {
    if (!files.length) {
      return;
    }
    let _files = files.map((f) => new File([f], f instanceof File ? f.name : "file", { type: f.type }));
    if (ios && use_post_upload_validation) {
      _files = _files.filter((file) => {
        if (is_valid_file(file)) {
          return true;
        }
        dispatch("error", `Invalid file type: ${file.name}. Only ${filetype} allowed.`);
        return false;
      });
      if (_files.length === 0) {
        return [];
      }
    }
    file_data = await prepare_files(_files);
    return await handle_upload(file_data);
  }
  function is_valid_file(file) {
    if (!filetype)
      return true;
    const allowed_types = Array.isArray(filetype) ? filetype : [filetype];
    return allowed_types.some((type) => {
      const processed_type = process_file_type(type);
      if (processed_type.startsWith(".")) {
        return file.name.toLowerCase().endsWith(processed_type.toLowerCase());
      }
      if (processed_type === "*") {
        return true;
      }
      if (processed_type.endsWith("/*")) {
        const [category] = processed_type.split("/");
        return file.type.startsWith(category + "/");
      }
      return file.type === processed_type;
    });
  }
  if ($$props.filetype === void 0 && $$bindings.filetype && filetype !== void 0)
    $$bindings.filetype(filetype);
  if ($$props.dragging === void 0 && $$bindings.dragging && dragging !== void 0)
    $$bindings.dragging(dragging);
  if ($$props.boundedheight === void 0 && $$bindings.boundedheight && boundedheight !== void 0)
    $$bindings.boundedheight(boundedheight);
  if ($$props.center === void 0 && $$bindings.center && center !== void 0)
    $$bindings.center(center);
  if ($$props.flex === void 0 && $$bindings.flex && flex !== void 0)
    $$bindings.flex(flex);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.disable_click === void 0 && $$bindings.disable_click && disable_click !== void 0)
    $$bindings.disable_click(disable_click);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.hidden === void 0 && $$bindings.hidden && hidden !== void 0)
    $$bindings.hidden(hidden);
  if ($$props.format === void 0 && $$bindings.format && format !== void 0)
    $$bindings.format(format);
  if ($$props.uploading === void 0 && $$bindings.uploading && uploading !== void 0)
    $$bindings.uploading(uploading);
  if ($$props.hidden_upload === void 0 && $$bindings.hidden_upload && hidden_upload !== void 0)
    $$bindings.hidden_upload(hidden_upload);
  if ($$props.show_progress === void 0 && $$bindings.show_progress && show_progress !== void 0)
    $$bindings.show_progress(show_progress);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  if ($$props.icon_upload === void 0 && $$bindings.icon_upload && icon_upload !== void 0)
    $$bindings.icon_upload(icon_upload);
  if ($$props.paste_clipboard === void 0 && $$bindings.paste_clipboard && paste_clipboard !== void 0)
    $$bindings.paste_clipboard(paste_clipboard);
  if ($$props.open_file_upload === void 0 && $$bindings.open_file_upload && open_file_upload !== void 0)
    $$bindings.open_file_upload(open_file_upload);
  if ($$props.load_files === void 0 && $$bindings.load_files && load_files !== void 0)
    $$bindings.load_files(load_files);
  $$result.css.add(css);
  ios = get_ios();
  {
    if (filetype == null) {
      accept_file_types = null;
    } else if (typeof filetype === "string") {
      accept_file_types = process_file_type(filetype);
    } else if (ios && filetype.includes("file/*")) {
      accept_file_types = "*";
    } else {
      filetype = filetype.map(process_file_type);
      accept_file_types = filetype.join(", ");
    }
  }
  return `${filetype === "clipboard" ? `<button${add_attribute("tabindex", hidden ? -1 : 0, 0)} class="${[
    "svelte-1b742ao",
    (hidden ? "hidden" : "") + " " + (center ? "center" : "") + " " + (boundedheight ? "boundedheight" : "") + " " + (flex ? "flex" : "") + " " + (icon_upload ? "icon-mode" : "")
  ].join(" ").trim()}"${add_styles({ "height": icon_upload ? "" : "100%" })}>${slots.default ? slots.default({}) : ``}</button>` : `${uploading && show_progress ? `${!hidden ? `${validate_component(UploadProgress, "UploadProgress").$$render(
    $$result,
    {
      root,
      upload_id,
      files: file_data,
      stream_handler
    },
    {},
    {}
  )}` : ``}` : `<button${add_attribute("tabindex", hidden ? -1 : 0, 0)} class="${[
    "svelte-1b742ao",
    (hidden ? "hidden" : "") + " " + (center ? "center" : "") + " " + (boundedheight ? "boundedheight" : "") + " " + (flex ? "flex" : "") + " " + (disable_click ? "disable_click" : "") + " " + (icon_upload ? "icon-mode" : "")
  ].join(" ").trim()}"${add_styles({ "height": icon_upload ? "" : "100%" })}>${slots.default ? slots.default({}) : ``} <input aria-label="file upload" data-testid="file-upload" type="file"${add_attribute("accept", accept_file_types || void 0, 0)} ${file_count === "multiple" || void 0 ? "multiple" : ""}${add_attribute("webkitdirectory", file_count === "directory" || void 0, 0)}${add_attribute("mozdirectory", file_count === "directory" || void 0, 0)} class="svelte-1b742ao"></button>`}`}`;
});
const ModifyUpload = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { editable = false } = $$props;
  let { undoable = false } = $$props;
  let { download = null } = $$props;
  let { i18n } = $$props;
  createEventDispatcher();
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.undoable === void 0 && $$bindings.undoable && undoable !== void 0)
    $$bindings.undoable(undoable);
  if ($$props.download === void 0 && $$bindings.download && download !== void 0)
    $$bindings.download(download);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  return `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${editable ? `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Edit, label: i18n("common.edit") }, {}, {})}` : ``} ${undoable ? `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Undo, label: i18n("common.undo") }, {}, {})}` : ``} ${download ? `${validate_component(DownloadLink, "DownloadLink").$$render($$result, { href: download, download: true }, {}, {
        default: () => {
          return `${validate_component(IconButton, "IconButton").$$render(
            $$result,
            {
              Icon: Download,
              label: i18n("common.download")
            },
            {},
            {}
          )}`;
        }
      })}` : ``} ${slots.default ? slots.default({}) : ``} ${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Clear, label: i18n("common.clear") }, {}, {})}`;
    }
  })}`;
});

export { ModifyUpload as M, Upload as U };
//# sourceMappingURL=ModifyUpload-CsV9IzIz.js.map
