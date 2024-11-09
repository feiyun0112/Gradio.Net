import { c as create_ssr_component, v as validate_component, b as add_attribute, m as missing_component, a as createEventDispatcher } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, e as BlockLabel, a1 as File$1, g as Empty, U as UploadText, h as IconButtonWrapper, i as IconButton, v as Undo, D as Download } from './2-B6LMYTAg.js';
import { U as Upload, M as ModifyUpload } from './ModifyUpload-CsV9IzIz.js';
export { default as BaseExample } from './Example17-CcyZAx20.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './DownloadLink--4obEanq.js';

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
const css$1 = {
  code: ".model3D.svelte-1mxwah3{display:flex;position:relative;width:var(--size-full);height:var(--size-full);border-radius:var(--block-radius);overflow:hidden}.model3D.svelte-1mxwah3 canvas{width:var(--size-full);height:var(--size-full);object-fit:contain;overflow:hidden}",
  map: `{"version":3,"file":"Model3D.svelte","sources":["Model3D.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { BlockLabel, IconButton, IconButtonWrapper } from \\"@gradio/atoms\\";\\nimport { File, Download, Undo } from \\"@gradio/icons\\";\\nimport { dequal } from \\"dequal\\";\\nexport let value;\\nexport let display_mode = \\"solid\\";\\nexport let clear_color = [0, 0, 0, 0];\\nexport let label = \\"\\";\\nexport let show_label;\\nexport let i18n;\\nexport let zoom_speed = 1;\\nexport let pan_speed = 1;\\nexport let camera_position = [\\n    null,\\n    null,\\n    null\\n];\\nexport let has_change_history = false;\\nlet current_settings = { camera_position, zoom_speed, pan_speed };\\nlet use_3dgs = false;\\nlet Canvas3DGSComponent;\\nlet Canvas3DComponent;\\nasync function loadCanvas3D() {\\n    const module = await import(\\"./Canvas3D.svelte\\");\\n    return module.default;\\n}\\nasync function loadCanvas3DGS() {\\n    const module = await import(\\"./Canvas3DGS.svelte\\");\\n    return module.default;\\n}\\n$: if (value) {\\n    use_3dgs = value.path.endsWith(\\".splat\\") || value.path.endsWith(\\".ply\\");\\n    if (use_3dgs) {\\n        loadCanvas3DGS().then((component) => {\\n            Canvas3DGSComponent = component;\\n        });\\n    }\\n    else {\\n        loadCanvas3D().then((component) => {\\n            Canvas3DComponent = component;\\n        });\\n    }\\n}\\nlet canvas3d;\\nfunction handle_undo() {\\n    canvas3d?.reset_camera_position(camera_position, zoom_speed, pan_speed);\\n}\\n$: {\\n    if (!dequal(current_settings.camera_position, camera_position) || current_settings.zoom_speed !== zoom_speed || current_settings.pan_speed !== pan_speed) {\\n        canvas3d?.reset_camera_position(camera_position, zoom_speed, pan_speed);\\n        current_settings = { camera_position, zoom_speed, pan_speed };\\n    }\\n}\\nlet resolved_url;\\n<\/script>\\n\\n<BlockLabel\\n\\t{show_label}\\n\\tIcon={File}\\n\\tlabel={label || i18n(\\"3D_model.3d_model\\")}\\n/>\\n{#if value}\\n\\t<div class=\\"model3D\\">\\n\\t\\t<IconButtonWrapper>\\n\\t\\t\\t{#if !use_3dgs}\\n\\t\\t\\t\\t<!-- Canvas3DGS doesn't implement the undo method (reset_camera_position) -->\\n\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\tIcon={Undo}\\n\\t\\t\\t\\t\\tlabel=\\"Undo\\"\\n\\t\\t\\t\\t\\ton:click={() => handle_undo()}\\n\\t\\t\\t\\t\\tdisabled={!has_change_history}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t\\t<a\\n\\t\\t\\t\\thref={resolved_url}\\n\\t\\t\\t\\ttarget={window.__is_colab__ ? \\"_blank\\" : null}\\n\\t\\t\\t\\tdownload={window.__is_colab__ ? null : value.orig_name || value.path}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<IconButton Icon={Download} label={i18n(\\"common.download\\")} />\\n\\t\\t\\t</a>\\n\\t\\t</IconButtonWrapper>\\n\\n\\t\\t{#if use_3dgs}\\n\\t\\t\\t<svelte:component\\n\\t\\t\\t\\tthis={Canvas3DGSComponent}\\n\\t\\t\\t\\tbind:resolved_url\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t{zoom_speed}\\n\\t\\t\\t\\t{pan_speed}\\n\\t\\t\\t/>\\n\\t\\t{:else}\\n\\t\\t\\t<svelte:component\\n\\t\\t\\t\\tthis={Canvas3DComponent}\\n\\t\\t\\t\\tbind:this={canvas3d}\\n\\t\\t\\t\\tbind:resolved_url\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t{display_mode}\\n\\t\\t\\t\\t{clear_color}\\n\\t\\t\\t\\t{camera_position}\\n\\t\\t\\t\\t{zoom_speed}\\n\\t\\t\\t\\t{pan_speed}\\n\\t\\t\\t/>\\n\\t\\t{/if}\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.model3D {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tborder-radius: var(--block-radius);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\t.model3D :global(canvas) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: contain;\\n\\t\\toverflow: hidden;\\n\\t}</style>\\n"],"names":[],"mappings":"AA0GC,uBAAS,CACR,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,aAAa,CAAE,IAAI,cAAc,CAAC,CAClC,QAAQ,CAAE,MACX,CACA,uBAAQ,CAAS,MAAQ,CACxB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,UAAU,CAAE,OAAO,CACnB,QAAQ,CAAE,MACX"}`
};
async function loadCanvas3D$1() {
  const module = await import('./Canvas3D-Cpp-H4vC.js');
  return module.default;
}
async function loadCanvas3DGS$1() {
  const module = await import('./Canvas3DGS-Bovupu2c.js');
  return module.default;
}
const Model3D = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color = [0, 0, 0, 0] } = $$props;
  let { label = "" } = $$props;
  let { show_label } = $$props;
  let { i18n } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { pan_speed = 1 } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { has_change_history = false } = $$props;
  let current_settings = { camera_position, zoom_speed, pan_speed };
  let use_3dgs = false;
  let Canvas3DGSComponent;
  let Canvas3DComponent;
  let canvas3d;
  let resolved_url;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.display_mode === void 0 && $$bindings.display_mode && display_mode !== void 0)
    $$bindings.display_mode(display_mode);
  if ($$props.clear_color === void 0 && $$bindings.clear_color && clear_color !== void 0)
    $$bindings.clear_color(clear_color);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.zoom_speed === void 0 && $$bindings.zoom_speed && zoom_speed !== void 0)
    $$bindings.zoom_speed(zoom_speed);
  if ($$props.pan_speed === void 0 && $$bindings.pan_speed && pan_speed !== void 0)
    $$bindings.pan_speed(pan_speed);
  if ($$props.camera_position === void 0 && $$bindings.camera_position && camera_position !== void 0)
    $$bindings.camera_position(camera_position);
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
      if (value) {
        use_3dgs = value.path.endsWith(".splat") || value.path.endsWith(".ply");
        if (use_3dgs) {
          loadCanvas3DGS$1().then((component) => {
            Canvas3DGSComponent = component;
          });
        } else {
          loadCanvas3D$1().then((component) => {
            Canvas3DComponent = component;
          });
        }
      }
    }
    {
      {
        if (!dequal(current_settings.camera_position, camera_position) || current_settings.zoom_speed !== zoom_speed || current_settings.pan_speed !== pan_speed) {
          canvas3d?.reset_camera_position(camera_position, zoom_speed, pan_speed);
          current_settings = { camera_position, zoom_speed, pan_speed };
        }
      }
    }
    $$rendered = `${validate_component(BlockLabel, "BlockLabel").$$render(
      $$result,
      {
        show_label,
        Icon: File$1,
        label: label || i18n("3D_model.3d_model")
      },
      {},
      {}
    )} ${value ? `<div class="model3D svelte-1mxwah3">${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
      default: () => {
        return `${!use_3dgs ? ` ${validate_component(IconButton, "IconButton").$$render(
          $$result,
          {
            Icon: Undo,
            label: "Undo",
            disabled: !has_change_history
          },
          {},
          {}
        )}` : ``} <a${add_attribute("href", resolved_url, 0)}${add_attribute("target", window.__is_colab__ ? "_blank" : null, 0)}${add_attribute(
          "download",
          window.__is_colab__ ? null : value.orig_name || value.path,
          0
        )}>${validate_component(IconButton, "IconButton").$$render(
          $$result,
          {
            Icon: Download,
            label: i18n("common.download")
          },
          {},
          {}
        )}</a>`;
      }
    })} ${use_3dgs ? `${validate_component(Canvas3DGSComponent || missing_component, "svelte:component").$$render(
      $$result,
      {
        value,
        zoom_speed,
        pan_speed,
        resolved_url
      },
      {
        resolved_url: ($$value) => {
          resolved_url = $$value;
          $$settled = false;
        }
      },
      {}
    )}` : `${validate_component(Canvas3DComponent || missing_component, "svelte:component").$$render(
      $$result,
      {
        value,
        display_mode,
        clear_color,
        camera_position,
        zoom_speed,
        pan_speed,
        this: canvas3d,
        resolved_url
      },
      {
        this: ($$value) => {
          canvas3d = $$value;
          $$settled = false;
        },
        resolved_url: ($$value) => {
          resolved_url = $$value;
          $$settled = false;
        }
      },
      {}
    )}`}</div>` : ``}`;
  } while (!$$settled);
  return $$rendered;
});
const Model3D$1 = Model3D;
const css = {
  code: ".input-model.svelte-jub4pj{display:flex;position:relative;justify-content:center;align-items:center;width:var(--size-full);height:var(--size-full);border-radius:var(--block-radius);overflow:hidden}.input-model.svelte-jub4pj canvas{width:var(--size-full);height:var(--size-full);object-fit:contain;overflow:hidden}",
  map: '{"version":3,"file":"Model3DUpload.svelte","sources":["Model3DUpload.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, tick } from \\"svelte\\";\\nimport { Upload, ModifyUpload } from \\"@gradio/upload\\";\\nimport { BlockLabel } from \\"@gradio/atoms\\";\\nimport { File } from \\"@gradio/icons\\";\\nexport let value;\\nexport let display_mode = \\"solid\\";\\nexport let clear_color = [0, 0, 0, 0];\\nexport let label = \\"\\";\\nexport let show_label;\\nexport let root;\\nexport let i18n;\\nexport let zoom_speed = 1;\\nexport let pan_speed = 1;\\nexport let max_file_size = null;\\nexport let uploading = false;\\nexport let camera_position = [\\n    null,\\n    null,\\n    null\\n];\\nexport let upload;\\nexport let stream_handler;\\nasync function handle_upload({ detail }) {\\n    value = detail;\\n    await tick();\\n    dispatch(\\"change\\", value);\\n    dispatch(\\"load\\", value);\\n}\\nasync function handle_clear() {\\n    value = null;\\n    await tick();\\n    dispatch(\\"clear\\");\\n    dispatch(\\"change\\");\\n}\\nlet use_3dgs = false;\\nlet Canvas3DGSComponent;\\nlet Canvas3DComponent;\\nasync function loadCanvas3D() {\\n    const module = await import(\\"./Canvas3D.svelte\\");\\n    return module.default;\\n}\\nasync function loadCanvas3DGS() {\\n    const module = await import(\\"./Canvas3DGS.svelte\\");\\n    return module.default;\\n}\\n$: if (value) {\\n    use_3dgs = value.path.endsWith(\\".splat\\") || value.path.endsWith(\\".ply\\");\\n    if (use_3dgs) {\\n        loadCanvas3DGS().then((component) => {\\n            Canvas3DGSComponent = component;\\n        });\\n    }\\n    else {\\n        loadCanvas3D().then((component) => {\\n            Canvas3DComponent = component;\\n        });\\n    }\\n}\\nlet canvas3d;\\nasync function handle_undo() {\\n    canvas3d?.reset_camera_position(camera_position, zoom_speed, pan_speed);\\n}\\nconst dispatch = createEventDispatcher();\\nlet dragging = false;\\n$: dispatch(\\"drag\\", dragging);\\n<\/script>\\n\\n<BlockLabel {show_label} Icon={File} label={label || \\"3D Model\\"} />\\n\\n{#if value === null}\\n\\t<Upload\\n\\t\\t{upload}\\n\\t\\t{stream_handler}\\n\\t\\ton:load={handle_upload}\\n\\t\\t{root}\\n\\t\\t{max_file_size}\\n\\t\\tfiletype={[\\".stl\\", \\".obj\\", \\".gltf\\", \\".glb\\", \\"model/obj\\", \\".splat\\", \\".ply\\"]}\\n\\t\\tbind:dragging\\n\\t\\tbind:uploading\\n\\t\\ton:error\\n\\t>\\n\\t\\t<slot />\\n\\t</Upload>\\n{:else}\\n\\t<div class=\\"input-model\\">\\n\\t\\t<ModifyUpload\\n\\t\\t\\tundoable={!use_3dgs}\\n\\t\\t\\ton:clear={handle_clear}\\n\\t\\t\\t{i18n}\\n\\t\\t\\ton:undo={handle_undo}\\n\\t\\t/>\\n\\n\\t\\t{#if use_3dgs}\\n\\t\\t\\t<svelte:component\\n\\t\\t\\t\\tthis={Canvas3DGSComponent}\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t{zoom_speed}\\n\\t\\t\\t\\t{pan_speed}\\n\\t\\t\\t/>\\n\\t\\t{:else}\\n\\t\\t\\t<svelte:component\\n\\t\\t\\t\\tthis={Canvas3DComponent}\\n\\t\\t\\t\\tbind:this={canvas3d}\\n\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t{display_mode}\\n\\t\\t\\t\\t{clear_color}\\n\\t\\t\\t\\t{camera_position}\\n\\t\\t\\t\\t{zoom_speed}\\n\\t\\t\\t\\t{pan_speed}\\n\\t\\t\\t/>\\n\\t\\t{/if}\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.input-model {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tborder-radius: var(--block-radius);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.input-model :global(canvas) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: contain;\\n\\t\\toverflow: hidden;\\n\\t}</style>\\n"],"names":[],"mappings":"AAmHC,0BAAa,CACZ,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,aAAa,CAAE,IAAI,cAAc,CAAC,CAClC,QAAQ,CAAE,MACX,CAEA,0BAAY,CAAS,MAAQ,CAC5B,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,UAAU,CAAE,OAAO,CACnB,QAAQ,CAAE,MACX"}'
};
async function loadCanvas3D() {
  const module = await import('./Canvas3D-Cpp-H4vC.js');
  return module.default;
}
async function loadCanvas3DGS() {
  const module = await import('./Canvas3DGS-Bovupu2c.js');
  return module.default;
}
const Model3DUpload = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color = [0, 0, 0, 0] } = $$props;
  let { label = "" } = $$props;
  let { show_label } = $$props;
  let { root } = $$props;
  let { i18n } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { pan_speed = 1 } = $$props;
  let { max_file_size = null } = $$props;
  let { uploading = false } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let use_3dgs = false;
  let Canvas3DGSComponent;
  let Canvas3DComponent;
  let canvas3d;
  const dispatch = createEventDispatcher();
  let dragging = false;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.display_mode === void 0 && $$bindings.display_mode && display_mode !== void 0)
    $$bindings.display_mode(display_mode);
  if ($$props.clear_color === void 0 && $$bindings.clear_color && clear_color !== void 0)
    $$bindings.clear_color(clear_color);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.zoom_speed === void 0 && $$bindings.zoom_speed && zoom_speed !== void 0)
    $$bindings.zoom_speed(zoom_speed);
  if ($$props.pan_speed === void 0 && $$bindings.pan_speed && pan_speed !== void 0)
    $$bindings.pan_speed(pan_speed);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.uploading === void 0 && $$bindings.uploading && uploading !== void 0)
    $$bindings.uploading(uploading);
  if ($$props.camera_position === void 0 && $$bindings.camera_position && camera_position !== void 0)
    $$bindings.camera_position(camera_position);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      if (value) {
        use_3dgs = value.path.endsWith(".splat") || value.path.endsWith(".ply");
        if (use_3dgs) {
          loadCanvas3DGS().then((component) => {
            Canvas3DGSComponent = component;
          });
        } else {
          loadCanvas3D().then((component) => {
            Canvas3DComponent = component;
          });
        }
      }
    }
    {
      dispatch("drag", dragging);
    }
    $$rendered = `${validate_component(BlockLabel, "BlockLabel").$$render(
      $$result,
      {
        show_label,
        Icon: File$1,
        label: label || "3D Model"
      },
      {},
      {}
    )} ${value === null ? `${validate_component(Upload, "Upload").$$render(
      $$result,
      {
        upload,
        stream_handler,
        root,
        max_file_size,
        filetype: [".stl", ".obj", ".gltf", ".glb", "model/obj", ".splat", ".ply"],
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
    )}` : `<div class="input-model svelte-jub4pj">${validate_component(ModifyUpload, "ModifyUpload").$$render($$result, { undoable: !use_3dgs, i18n }, {}, {})} ${use_3dgs ? `${validate_component(Canvas3DGSComponent || missing_component, "svelte:component").$$render($$result, { value, zoom_speed, pan_speed }, {}, {})}` : `${validate_component(Canvas3DComponent || missing_component, "svelte:component").$$render(
      $$result,
      {
        value,
        display_mode,
        clear_color,
        camera_position,
        zoom_speed,
        pan_speed,
        this: canvas3d
      },
      {
        this: ($$value) => {
          canvas3d = $$value;
          $$settled = false;
        }
      },
      {}
    )}`}</div>`}`;
  } while (!$$settled);
  return $$rendered;
});
const Model3DUpload$1 = Model3DUpload;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { root } = $$props;
  let { display_mode = "solid" } = $$props;
  let { clear_color } = $$props;
  let { loading_status } = $$props;
  let { label } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { height = void 0 } = $$props;
  let { zoom_speed = 1 } = $$props;
  let { input_ready } = $$props;
  let uploading = false;
  let { has_change_history = false } = $$props;
  let { camera_position = [null, null, null] } = $$props;
  let { interactive } = $$props;
  const is_browser = typeof window !== "undefined";
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.display_mode === void 0 && $$bindings.display_mode && display_mode !== void 0)
    $$bindings.display_mode(display_mode);
  if ($$props.clear_color === void 0 && $$bindings.clear_color && clear_color !== void 0)
    $$bindings.clear_color(clear_color);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.zoom_speed === void 0 && $$bindings.zoom_speed && zoom_speed !== void 0)
    $$bindings.zoom_speed(zoom_speed);
  if ($$props.input_ready === void 0 && $$bindings.input_ready && input_ready !== void 0)
    $$bindings.input_ready(input_ready);
  if ($$props.has_change_history === void 0 && $$bindings.has_change_history && has_change_history !== void 0)
    $$bindings.has_change_history(has_change_history);
  if ($$props.camera_position === void 0 && $$bindings.camera_position && camera_position !== void 0)
    $$bindings.camera_position(camera_position);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    input_ready = !uploading;
    $$rendered = `${!interactive ? `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: value === null ? "dashed" : "solid",
        border_mode: "base",
        padding: false,
        elem_id,
        elem_classes,
        container,
        scale,
        min_width,
        height
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${value && is_browser ? `${validate_component(Model3D$1, "Model3D").$$render(
            $$result,
            {
              value,
              i18n: gradio.i18n,
              display_mode,
              clear_color,
              label,
              show_label,
              camera_position,
              zoom_speed,
              has_change_history
            },
            {},
            {}
          )}` : ` ${validate_component(BlockLabel, "BlockLabel").$$render(
            $$result,
            {
              show_label,
              Icon: File$1,
              label: label || "3D Model"
            },
            {},
            {}
          )} ${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true, size: "large" }, {}, {
            default: () => {
              return `${validate_component(File$1, "File").$$render($$result, {}, {}, {})}`;
            }
          })}`}`;
        }
      }
    )}` : `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: value === null ? "dashed" : "solid",
        border_mode: "base",
        padding: false,
        elem_id,
        elem_classes,
        container,
        scale,
        min_width,
        height
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(Model3DUpload$1, "Model3DUpload").$$render(
            $$result,
            {
              label,
              show_label,
              root,
              display_mode,
              clear_color,
              value,
              camera_position,
              zoom_speed,
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
                return `${validate_component(UploadText, "UploadText").$$render($$result, { i18n: gradio.i18n, type: "file" }, {}, {})}`;
              }
            }
          )}`;
        }
      }
    )}`}`;
  } while (!$$settled);
  return $$rendered;
});

export { Model3D$1 as BaseModel3D, Model3DUpload$1 as BaseModel3DUpload, Index as default };
//# sourceMappingURL=Index50-Bzy2IpbJ.js.map
