import { c as create_ssr_component, v as validate_component, b as add_attribute, f as each, e as escape } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, e as BlockLabel, f as Image, g as Empty, h as IconButtonWrapper, i as IconButton, M as Maximize } from './2-B6LMYTAg.js';
import { r as resolve_wasm_src } from './DownloadLink--4obEanq.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".base-image.svelte-303fln.svelte-303fln{display:block;width:100%;height:auto}.container.svelte-303fln.svelte-303fln{display:flex;position:relative;flex-direction:column;justify-content:center;align-items:center;width:var(--size-full);height:var(--size-full)}.image-container.svelte-303fln.svelte-303fln{position:relative;top:0;left:0;flex-grow:1;width:100%;overflow:hidden}.fit-height.svelte-303fln.svelte-303fln{top:0;left:0;width:100%;height:100%;object-fit:contain}.mask.svelte-303fln.svelte-303fln{opacity:0.85;transition:all 0.2s ease-in-out;position:absolute}.image-container.svelte-303fln:hover .mask.svelte-303fln{opacity:0.3}.mask.active.svelte-303fln.svelte-303fln{opacity:1}.mask.inactive.svelte-303fln.svelte-303fln{opacity:0}.legend.svelte-303fln.svelte-303fln{display:flex;flex-direction:row;flex-wrap:wrap;align-content:center;justify-content:center;align-items:center;gap:var(--spacing-sm);padding:var(--spacing-sm)}.legend-item.svelte-303fln.svelte-303fln{display:flex;flex-direction:row;align-items:center;cursor:pointer;border-radius:var(--radius-sm);padding:var(--spacing-sm)}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport { Block, BlockLabel, Empty, IconButton, IconButtonWrapper } from \\"@gradio/atoms\\";\\nimport { Image, Maximize, Minimize } from \\"@gradio/icons\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport {} from \\"@gradio/client\\";\\nimport { resolve_wasm_src } from \\"@gradio/wasm/svelte\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = null;\\nlet old_value = null;\\nlet _value = null;\\nexport let gradio;\\nexport let label = gradio.i18n(\\"annotated_image.annotated_image\\");\\nexport let show_label = true;\\nexport let show_legend = true;\\nexport let height;\\nexport let width;\\nexport let color_map;\\nexport let container = true;\\nexport let scale = null;\\nexport let min_width = void 0;\\nlet active = null;\\nexport let loading_status;\\nexport let show_fullscreen_button = true;\\nlet is_full_screen = false;\\nlet image_container;\\nonMount(() => {\\n    document.addEventListener(\\"fullscreenchange\\", () => {\\n        is_full_screen = !!document.fullscreenElement;\\n    });\\n});\\nconst toggle_full_screen = async () => {\\n    if (!is_full_screen) {\\n        await image_container.requestFullscreen();\\n    }\\n    else {\\n        await document.exitFullscreen();\\n    }\\n};\\nlet latest_promise = null;\\n$: {\\n    if (value !== old_value) {\\n        old_value = value;\\n        gradio.dispatch(\\"change\\");\\n    }\\n    if (value) {\\n        const normalized_value = {\\n            image: value.image,\\n            annotations: value.annotations.map((ann) => ({\\n                image: ann.image,\\n                label: ann.label\\n            }))\\n        };\\n        _value = normalized_value;\\n        const image_url_promise = resolve_wasm_src(normalized_value.image.url);\\n        const annotation_urls_promise = Promise.all(normalized_value.annotations.map((ann) => resolve_wasm_src(ann.image.url)));\\n        const current_promise = Promise.all([\\n            image_url_promise,\\n            annotation_urls_promise\\n        ]);\\n        latest_promise = current_promise;\\n        current_promise.then(([image_url, annotation_urls]) => {\\n            if (latest_promise !== current_promise) {\\n                return;\\n            }\\n            const async_resolved_value = {\\n                image: {\\n                    ...normalized_value.image,\\n                    url: image_url ?? void 0\\n                },\\n                annotations: normalized_value.annotations.map((ann, i) => ({\\n                    ...ann,\\n                    image: {\\n                        ...ann.image,\\n                        url: annotation_urls[i] ?? void 0\\n                    }\\n                }))\\n            };\\n            _value = async_resolved_value;\\n        });\\n    }\\n    else {\\n        _value = null;\\n    }\\n}\\nfunction handle_mouseover(_label) {\\n    active = _label;\\n}\\nfunction handle_mouseout() {\\n    active = null;\\n}\\nfunction handle_click(i, value2) {\\n    gradio.dispatch(\\"select\\", {\\n        value: label,\\n        index: i\\n    });\\n}\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\tpadding={false}\\n\\t{height}\\n\\t{width}\\n\\tallow_overflow={false}\\n\\t{container}\\n\\t{scale}\\n\\t{min_width}\\n>\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t/>\\n\\t<BlockLabel\\n\\t\\t{show_label}\\n\\t\\tIcon={Image}\\n\\t\\tlabel={label || gradio.i18n(\\"image.image\\")}\\n\\t/>\\n\\n\\t<div class=\\"container\\">\\n\\t\\t{#if _value == null}\\n\\t\\t\\t<Empty size=\\"large\\" unpadded_box={true}><Image /></Empty>\\n\\t\\t{:else}\\n\\t\\t\\t<div class=\\"image-container\\" bind:this={image_container}>\\n\\t\\t\\t\\t<IconButtonWrapper>\\n\\t\\t\\t\\t\\t{#if !is_full_screen && show_fullscreen_button}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={Maximize}\\n\\t\\t\\t\\t\\t\\t\\tlabel=\\"View in full screen\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\n\\t\\t\\t\\t\\t{#if is_full_screen}\\n\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\tIcon={Minimize}\\n\\t\\t\\t\\t\\t\\t\\tlabel=\\"Exit full screen\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</IconButtonWrapper>\\n\\n\\t\\t\\t\\t<img\\n\\t\\t\\t\\t\\tclass=\\"base-image\\"\\n\\t\\t\\t\\t\\tclass:fit-height={height && !is_full_screen}\\n\\t\\t\\t\\t\\tsrc={_value ? _value.image.url : null}\\n\\t\\t\\t\\t\\talt=\\"the base file that is annotated\\"\\n\\t\\t\\t\\t/>\\n\\t\\t\\t\\t{#each _value ? _value?.annotations : [] as ann, i}\\n\\t\\t\\t\\t\\t<img\\n\\t\\t\\t\\t\\t\\talt=\\"segmentation mask identifying {label} within the uploaded file\\"\\n\\t\\t\\t\\t\\t\\tclass=\\"mask fit-height\\"\\n\\t\\t\\t\\t\\t\\tclass:fit-height={!is_full_screen}\\n\\t\\t\\t\\t\\t\\tclass:active={active == ann.label}\\n\\t\\t\\t\\t\\t\\tclass:inactive={active != ann.label && active != null}\\n\\t\\t\\t\\t\\t\\tsrc={ann.image.url}\\n\\t\\t\\t\\t\\t\\tstyle={color_map && ann.label in color_map\\n\\t\\t\\t\\t\\t\\t\\t? null\\n\\t\\t\\t\\t\\t\\t\\t: `filter: hue-rotate(${Math.round(\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t(i * 360) / _value?.annotations.length\\n\\t\\t\\t\\t\\t\\t\\t\\t)}deg);`}\\n\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t</div>\\n\\t\\t\\t{#if show_legend && _value}\\n\\t\\t\\t\\t<div class=\\"legend\\">\\n\\t\\t\\t\\t\\t{#each _value.annotations as ann, i}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"legend-item\\"\\n\\t\\t\\t\\t\\t\\t\\tstyle=\\"background-color: {color_map && ann.label in color_map\\n\\t\\t\\t\\t\\t\\t\\t\\t? color_map[ann.label] + \'88\'\\n\\t\\t\\t\\t\\t\\t\\t\\t: `hsla(${Math.round(\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t(i * 360) / _value.annotations.length\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t)}, 100%, 50%, 0.3)`}\\"\\n\\t\\t\\t\\t\\t\\t\\ton:mouseover={() => handle_mouseover(ann.label)}\\n\\t\\t\\t\\t\\t\\t\\ton:focus={() => handle_mouseover(ann.label)}\\n\\t\\t\\t\\t\\t\\t\\ton:mouseout={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\t\\ton:blur={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => handle_click(i, ann.label)}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{ann.label}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t{/if}\\n\\t\\t{/if}\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\t.base-image {\\n\\t\\tdisplay: block;\\n\\t\\twidth: 100%;\\n\\t\\theight: auto;\\n\\t}\\n\\t.container {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\t.image-container {\\n\\t\\tposition: relative;\\n\\t\\ttop: 0;\\n\\t\\tleft: 0;\\n\\t\\tflex-grow: 1;\\n\\t\\twidth: 100%;\\n\\t\\toverflow: hidden;\\n\\t}\\n\\t.fit-height {\\n\\t\\ttop: 0;\\n\\t\\tleft: 0;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\tobject-fit: contain;\\n\\t}\\n\\t.mask {\\n\\t\\topacity: 0.85;\\n\\t\\ttransition: all 0.2s ease-in-out;\\n\\t\\tposition: absolute;\\n\\t}\\n\\t.image-container:hover .mask {\\n\\t\\topacity: 0.3;\\n\\t}\\n\\t.mask.active {\\n\\t\\topacity: 1;\\n\\t}\\n\\t.mask.inactive {\\n\\t\\topacity: 0;\\n\\t}\\n\\t.legend {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: row;\\n\\t\\tflex-wrap: wrap;\\n\\t\\talign-content: center;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tpadding: var(--spacing-sm);\\n\\t}\\n\\t.legend-item {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: row;\\n\\t\\talign-items: center;\\n\\t\\tcursor: pointer;\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tpadding: var(--spacing-sm);\\n\\t}</style>\\n"],"names":[],"mappings":"AAkMC,uCAAY,CACX,OAAO,CAAE,KAAK,CACd,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IACT,CACA,sCAAW,CACV,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CACxB,CACA,4CAAiB,CAChB,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,CAAC,CACN,IAAI,CAAE,CAAC,CACP,SAAS,CAAE,CAAC,CACZ,KAAK,CAAE,IAAI,CACX,QAAQ,CAAE,MACX,CACA,uCAAY,CACX,GAAG,CAAE,CAAC,CACN,IAAI,CAAE,CAAC,CACP,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,UAAU,CAAE,OACb,CACA,iCAAM,CACL,OAAO,CAAE,IAAI,CACb,UAAU,CAAE,GAAG,CAAC,IAAI,CAAC,WAAW,CAChC,QAAQ,CAAE,QACX,CACA,8BAAgB,MAAM,CAAC,mBAAM,CAC5B,OAAO,CAAE,GACV,CACA,KAAK,mCAAQ,CACZ,OAAO,CAAE,CACV,CACA,KAAK,qCAAU,CACd,OAAO,CAAE,CACV,CACA,mCAAQ,CACP,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,GAAG,CACnB,SAAS,CAAE,IAAI,CACf,aAAa,CAAE,MAAM,CACrB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,OAAO,CAAE,IAAI,YAAY,CAC1B,CACA,wCAAa,CACZ,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,GAAG,CACnB,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,OAAO,CACf,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,OAAO,CAAE,IAAI,YAAY,CAC1B"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let old_value = null;
  let _value = null;
  let { gradio } = $$props;
  let { label = gradio.i18n("annotated_image.annotated_image") } = $$props;
  let { show_label = true } = $$props;
  let { show_legend = true } = $$props;
  let { height } = $$props;
  let { width } = $$props;
  let { color_map } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let active = null;
  let { loading_status } = $$props;
  let { show_fullscreen_button = true } = $$props;
  let is_full_screen = false;
  let image_container;
  let latest_promise = null;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.show_legend === void 0 && $$bindings.show_legend && show_legend !== void 0)
    $$bindings.show_legend(show_legend);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.width === void 0 && $$bindings.width && width !== void 0)
    $$bindings.width(width);
  if ($$props.color_map === void 0 && $$bindings.color_map && color_map !== void 0)
    $$bindings.color_map(color_map);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.show_fullscreen_button === void 0 && $$bindings.show_fullscreen_button && show_fullscreen_button !== void 0)
    $$bindings.show_fullscreen_button(show_fullscreen_button);
  $$result.css.add(css);
  {
    {
      if (value !== old_value) {
        old_value = value;
        gradio.dispatch("change");
      }
      if (value) {
        const normalized_value = {
          image: value.image,
          annotations: value.annotations.map((ann) => ({ image: ann.image, label: ann.label }))
        };
        _value = normalized_value;
        const image_url_promise = resolve_wasm_src(normalized_value.image.url);
        const annotation_urls_promise = Promise.all(normalized_value.annotations.map((ann) => resolve_wasm_src(ann.image.url)));
        const current_promise = Promise.all([image_url_promise, annotation_urls_promise]);
        latest_promise = current_promise;
        current_promise.then(([image_url, annotation_urls]) => {
          if (latest_promise !== current_promise) {
            return;
          }
          const async_resolved_value = {
            image: {
              ...normalized_value.image,
              url: image_url ?? void 0
            },
            annotations: normalized_value.annotations.map((ann, i) => ({
              ...ann,
              image: {
                ...ann.image,
                url: annotation_urls[i] ?? void 0
              }
            }))
          };
          _value = async_resolved_value;
        });
      } else {
        _value = null;
      }
    }
  }
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      padding: false,
      height,
      width,
      allow_overflow: false,
      container,
      scale,
      min_width
    },
    {},
    {
      default: () => {
        return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(BlockLabel, "BlockLabel").$$render(
          $$result,
          {
            show_label,
            Icon: Image,
            label: label || gradio.i18n("image.image")
          },
          {},
          {}
        )} <div class="container svelte-303fln">${_value == null ? `${validate_component(Empty, "Empty").$$render($$result, { size: "large", unpadded_box: true }, {}, {
          default: () => {
            return `${validate_component(Image, "Image").$$render($$result, {}, {}, {})}`;
          }
        })}` : `<div class="image-container svelte-303fln"${add_attribute("this", image_container, 0)}>${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
          default: () => {
            return `${show_fullscreen_button ? `${validate_component(IconButton, "IconButton").$$render(
              $$result,
              {
                Icon: Maximize,
                label: "View in full screen"
              },
              {},
              {}
            )}` : ``} ${``}`;
          }
        })} <img class="${[
          "base-image svelte-303fln",
          height && !is_full_screen ? "fit-height" : ""
        ].join(" ").trim()}"${add_attribute("src", _value ? _value.image.url : null, 0)} alt="the base file that is annotated"> ${each(_value ? _value?.annotations : [], (ann, i) => {
          return `<img alt="${"segmentation mask identifying " + escape(label, true) + " within the uploaded file"}" class="${[
            "mask fit-height svelte-303fln",
            ("fit-height" ) + " " + (active == ann.label ? "active" : "") + " " + (active != ann.label && active != null ? "inactive" : "")
          ].join(" ").trim()}"${add_attribute("src", ann.image.url, 0)}${add_attribute(
            "style",
            color_map && ann.label in color_map ? null : `filter: hue-rotate(${Math.round(i * 360 / _value?.annotations.length)}deg);`,
            0
          )}>`;
        })}</div> ${show_legend && _value ? `<div class="legend svelte-303fln">${each(_value.annotations, (ann, i) => {
          return `<button class="legend-item svelte-303fln" style="${"background-color: " + escape(
            color_map && ann.label in color_map ? color_map[ann.label] + "88" : `hsla(${Math.round(i * 360 / _value.annotations.length)}, 100%, 50%, 0.3)`,
            true
          )}">${escape(ann.label)} </button>`;
        })}</div>` : ``}`}</div>`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index14-BHNymPPV.js.map
