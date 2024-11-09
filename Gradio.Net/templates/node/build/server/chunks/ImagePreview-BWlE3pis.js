import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, b as add_attribute } from './ssr-RaXq3SJh.js';
import { e as BlockLabel, f as Image, g as Empty, h as IconButtonWrapper, i as IconButton, M as Maximize, D as Download, o as ShareButton, u as uploadToHuggingFace } from './2-B6LMYTAg.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
import { D as DownloadLink } from './DownloadLink--4obEanq.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".image-container.svelte-1kpcxni.svelte-1kpcxni{height:100%;position:relative}.image-container.svelte-1kpcxni button.svelte-1kpcxni{width:var(--size-full);height:var(--size-full);border-radius:var(--radius-lg);display:flex;align-items:center;justify-content:center}.image-frame.svelte-1kpcxni.svelte-1kpcxni{width:auto;height:100%;display:flex;align-items:center;justify-content:center}.image-frame.svelte-1kpcxni img{width:var(--size-full);height:var(--size-full);object-fit:scale-down}.selectable.svelte-1kpcxni.svelte-1kpcxni{cursor:crosshair}.fullscreen-controls svg{position:relative;top:0px}.image-container:fullscreen{background-color:black;display:flex;justify-content:center;align-items:center}.image-container:fullscreen img{max-width:90vw;max-height:90vh;object-fit:scale-down}",
  map: '{"version":3,"file":"ImagePreview.svelte","sources":["ImagePreview.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, onMount } from \\"svelte\\";\\nimport { uploadToHuggingFace } from \\"@gradio/utils\\";\\nimport { BlockLabel, Empty, IconButton, ShareButton, IconButtonWrapper } from \\"@gradio/atoms\\";\\nimport { Download } from \\"@gradio/icons\\";\\nimport { get_coordinates_of_clicked_image } from \\"./utils\\";\\nimport Image from \\"./Image.svelte\\";\\nimport { DownloadLink } from \\"@gradio/wasm/svelte\\";\\nimport { Maximize, Minimize } from \\"@gradio/icons\\";\\nimport { Image as ImageIcon } from \\"@gradio/icons\\";\\nimport {} from \\"@gradio/client\\";\\nexport let value;\\nexport let label = void 0;\\nexport let show_label;\\nexport let show_download_button = true;\\nexport let selectable = false;\\nexport let show_share_button = false;\\nexport let i18n;\\nexport let show_fullscreen_button = true;\\nconst dispatch = createEventDispatcher();\\nconst handle_click = (evt) => {\\n    let coordinates = get_coordinates_of_clicked_image(evt);\\n    if (coordinates) {\\n        dispatch(\\"select\\", { index: coordinates, value: null });\\n    }\\n};\\nlet is_full_screen = false;\\nlet image_container;\\nonMount(() => {\\n    document.addEventListener(\\"fullscreenchange\\", () => {\\n        is_full_screen = !!document.fullscreenElement;\\n    });\\n});\\nconst toggle_full_screen = async () => {\\n    if (!is_full_screen) {\\n        await image_container.requestFullscreen();\\n    }\\n    else {\\n        await document.exitFullscreen();\\n        is_full_screen = !is_full_screen;\\n    }\\n};\\n<\/script>\\n\\n<BlockLabel\\n\\t{show_label}\\n\\tIcon={ImageIcon}\\n\\tlabel={!show_label ? \\"\\" : label || i18n(\\"image.image\\")}\\n/>\\n{#if value === null || !value.url}\\n\\t<Empty unpadded_box={true} size=\\"large\\"><ImageIcon /></Empty>\\n{:else}\\n\\t<div class=\\"image-container\\" bind:this={image_container}>\\n\\t\\t<IconButtonWrapper>\\n\\t\\t\\t{#if !is_full_screen && show_fullscreen_button}\\n\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\tIcon={Maximize}\\n\\t\\t\\t\\t\\tlabel={is_full_screen ? \\"Exit full screen\\" : \\"View in full screen\\"}\\n\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\n\\t\\t\\t{#if is_full_screen && show_fullscreen_button}\\n\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\tIcon={Minimize}\\n\\t\\t\\t\\t\\tlabel={is_full_screen ? \\"Exit full screen\\" : \\"View in full screen\\"}\\n\\t\\t\\t\\t\\ton:click={toggle_full_screen}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\n\\t\\t\\t{#if show_download_button}\\n\\t\\t\\t\\t<DownloadLink href={value.url} download={value.orig_name || \\"image\\"}>\\n\\t\\t\\t\\t\\t<IconButton Icon={Download} label={i18n(\\"common.download\\")} />\\n\\t\\t\\t\\t</DownloadLink>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if show_share_button}\\n\\t\\t\\t\\t<ShareButton\\n\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\ton:share\\n\\t\\t\\t\\t\\ton:error\\n\\t\\t\\t\\t\\tformatter={async (value) => {\\n\\t\\t\\t\\t\\t\\tif (!value) return \\"\\";\\n\\t\\t\\t\\t\\t\\tlet url = await uploadToHuggingFace(value, \\"url\\");\\n\\t\\t\\t\\t\\t\\treturn `<img src=\\"${url}\\" />`;\\n\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t</IconButtonWrapper>\\n\\t\\t<button on:click={handle_click}>\\n\\t\\t\\t<div class:selectable class=\\"image-frame\\">\\n\\t\\t\\t\\t<Image src={value.url} alt=\\"\\" loading=\\"lazy\\" on:load />\\n\\t\\t\\t</div>\\n\\t\\t</button>\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.image-container {\\n\\t\\theight: 100%;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t.image-container button {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tborder-radius: var(--radius-lg);\\n\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t}\\n\\n\\t.image-frame {\\n\\t\\twidth: auto;\\n\\t\\theight: 100%;\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t}\\n\\t.image-frame :global(img) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: scale-down;\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: crosshair;\\n\\t}\\n\\n\\t:global(.fullscreen-controls svg) {\\n\\t\\tposition: relative;\\n\\t\\ttop: 0px;\\n\\t}\\n\\n\\t:global(.image-container:fullscreen) {\\n\\t\\tbackground-color: black;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t}\\n\\n\\t:global(.image-container:fullscreen img) {\\n\\t\\tmax-width: 90vw;\\n\\t\\tmax-height: 90vh;\\n\\t\\tobject-fit: scale-down;\\n\\t}</style>\\n"],"names":[],"mappings":"AAiGC,8CAAiB,CAChB,MAAM,CAAE,IAAI,CACZ,QAAQ,CAAE,QACX,CAEA,+BAAgB,CAAC,qBAAO,CACvB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,aAAa,CAAE,IAAI,WAAW,CAAC,CAE/B,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAClB,CAEA,0CAAa,CACZ,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAClB,CACA,2BAAY,CAAS,GAAK,CACzB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,UAAU,CAAE,UACb,CAEA,yCAAY,CACX,MAAM,CAAE,SACT,CAEQ,wBAA0B,CACjC,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,GACN,CAEQ,2BAA6B,CACpC,gBAAgB,CAAE,KAAK,CACvB,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MACd,CAEQ,+BAAiC,CACxC,SAAS,CAAE,IAAI,CACf,UAAU,CAAE,IAAI,CAChB,UAAU,CAAE,UACb"}'
};
const ImagePreview = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { label = void 0 } = $$props;
  let { show_label } = $$props;
  let { show_download_button = true } = $$props;
  let { selectable = false } = $$props;
  let { show_share_button = false } = $$props;
  let { i18n } = $$props;
  let { show_fullscreen_button = true } = $$props;
  createEventDispatcher();
  let image_container;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.show_fullscreen_button === void 0 && $$bindings.show_fullscreen_button && show_fullscreen_button !== void 0)
    $$bindings.show_fullscreen_button(show_fullscreen_button);
  $$result.css.add(css);
  return `${validate_component(BlockLabel, "BlockLabel").$$render(
    $$result,
    {
      show_label,
      Icon: Image,
      label: !show_label ? "" : label || i18n("image.image")
    },
    {},
    {}
  )} ${value === null || !value.url ? `${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true, size: "large" }, {}, {
    default: () => {
      return `${validate_component(Image, "ImageIcon").$$render($$result, {}, {}, {})}`;
    }
  })}` : `<div class="image-container svelte-1kpcxni"${add_attribute("this", image_container, 0)}>${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${show_fullscreen_button ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Maximize,
          label: "View in full screen"
        },
        {},
        {}
      )}` : ``} ${``} ${show_download_button ? `${validate_component(DownloadLink, "DownloadLink").$$render(
        $$result,
        {
          href: value.url,
          download: value.orig_name || "image"
        },
        {},
        {
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
        }
      )}` : ``} ${show_share_button ? `${validate_component(ShareButton, "ShareButton").$$render(
        $$result,
        {
          i18n,
          formatter: async (value2) => {
            if (!value2)
              return "";
            let url = await uploadToHuggingFace(value2);
            return `<img src="${url}" />`;
          },
          value
        },
        {},
        {}
      )}` : ``}`;
    }
  })} <button class="svelte-1kpcxni"><div class="${["image-frame svelte-1kpcxni", selectable ? "selectable" : ""].join(" ").trim()}">${validate_component(Image$1, "Image").$$render($$result, { src: value.url, alt: "", loading: "lazy" }, {}, {})}</div></button></div>`}`;
});

export { ImagePreview as default };
//# sourceMappingURL=ImagePreview-BWlE3pis.js.map
