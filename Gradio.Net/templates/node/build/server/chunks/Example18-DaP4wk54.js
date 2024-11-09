import { c as create_ssr_component, b as add_attribute, e as escape, f as each, v as validate_component } from './ssr-RaXq3SJh.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
import './2-B6LMYTAg.js';
import { V as Video } from './Video-_epjqq1V.js';
import './DownloadLink--4obEanq.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './hls-CrxM9YLy.js';

const css = {
  code: ".gallery.svelte-1cl8bqt{padding:var(--size-1) var(--size-2);display:flex;align-items:center;gap:20px;overflow-x:auto}div.svelte-1cl8bqt{overflow:hidden;min-width:var(--local-text-width);white-space:nowrap}.container.svelte-1cl8bqt img,.container.svelte-1cl8bqt video{object-fit:contain;width:100px;height:100px}.container.selected.svelte-1cl8bqt{border-color:var(--border-color-accent)}.border.table.svelte-1cl8bqt{border:2px solid var(--border-color-primary)}.container.table.svelte-1cl8bqt{margin:0 auto;border-radius:var(--radius-lg);overflow-x:auto;width:max-content;height:max-content;object-fit:cover;padding:var(--size-2)}.container.gallery.svelte-1cl8bqt{object-fit:cover}div.svelte-1cl8bqt>p{font-size:var(--text-lg);white-space:normal}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport { Image } from \\"@gradio/image/shared\\";\\nimport { Video } from \\"@gradio/video/shared\\";\\nexport let value = {\\n    text: \\"\\",\\n    files: []\\n};\\nexport let type;\\nexport let selected = false;\\nlet size;\\nlet el;\\nfunction set_styles(element, el_width) {\\n    element.style.setProperty(\\"--local-text-width\\", `${el_width && el_width < 150 ? el_width : 200}px`);\\n    element.style.whiteSpace = \\"unset\\";\\n}\\nonMount(() => {\\n    set_styles(el, size);\\n});\\n<\/script>\\n\\n<div\\n\\tclass=\\"container\\"\\n\\tbind:clientWidth={size}\\n\\tbind:this={el}\\n\\tclass:table={type === \\"table\\"}\\n\\tclass:gallery={type === \\"gallery\\"}\\n\\tclass:selected\\n\\tclass:border={value}\\n>\\n\\t<p>{value.text ? value.text : \\"\\"}</p>\\n\\t{#each value.files as file}\\n\\t\\t{#if file.mime_type && file.mime_type.includes(\\"image\\")}\\n\\t\\t\\t<Image src={file.url} alt=\\"\\" />\\n\\t\\t{:else if file.mime_type && file.mime_type.includes(\\"video\\")}\\n\\t\\t\\t<Video src={file.url} alt=\\"\\" loop={true} is_stream={false} />\\n\\t\\t{:else if file.mime_type && file.mime_type.includes(\\"audio\\")}\\n\\t\\t\\t<audio src={file.url} controls />\\n\\t\\t{:else}\\n\\t\\t\\t{file.orig_name}\\n\\t\\t{/if}\\n\\t{/each}\\n</div>\\n\\n<style>\\n\\t.gallery {\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tgap: 20px;\\n\\t\\toverflow-x: auto;\\n\\t}\\n\\n\\tdiv {\\n\\t\\toverflow: hidden;\\n\\t\\tmin-width: var(--local-text-width);\\n\\t\\twhite-space: nowrap;\\n\\t}\\n\\n\\t.container :global(img),\\n\\t.container :global(video) {\\n\\t\\tobject-fit: contain;\\n\\t\\twidth: 100px;\\n\\t\\theight: 100px;\\n\\t}\\n\\n\\t.container.selected {\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t}\\n\\t.border.table {\\n\\t\\tborder: 2px solid var(--border-color-primary);\\n\\t}\\n\\n\\t.container.table {\\n\\t\\tmargin: 0 auto;\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t\\toverflow-x: auto;\\n\\t\\twidth: max-content;\\n\\t\\theight: max-content;\\n\\t\\tobject-fit: cover;\\n\\t\\tpadding: var(--size-2);\\n\\t}\\n\\n\\t.container.gallery {\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\tdiv > :global(p) {\\n\\t\\tfont-size: var(--text-lg);\\n\\t\\twhite-space: normal;\\n\\t}</style>\\n"],"names":[],"mappings":"AA4CC,uBAAS,CACR,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,GAAG,CAAE,IAAI,CACT,UAAU,CAAE,IACb,CAEA,kBAAI,CACH,QAAQ,CAAE,MAAM,CAChB,SAAS,CAAE,IAAI,kBAAkB,CAAC,CAClC,WAAW,CAAE,MACd,CAEA,yBAAU,CAAS,GAAI,CACvB,yBAAU,CAAS,KAAO,CACzB,UAAU,CAAE,OAAO,CACnB,KAAK,CAAE,KAAK,CACZ,MAAM,CAAE,KACT,CAEA,UAAU,wBAAU,CACnB,YAAY,CAAE,IAAI,qBAAqB,CACxC,CACA,OAAO,qBAAO,CACb,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAC7C,CAEA,UAAU,qBAAO,CAChB,MAAM,CAAE,CAAC,CAAC,IAAI,CACd,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE,IAAI,CAChB,KAAK,CAAE,WAAW,CAClB,MAAM,CAAE,WAAW,CACnB,UAAU,CAAE,KAAK,CACjB,OAAO,CAAE,IAAI,QAAQ,CACtB,CAEA,UAAU,uBAAS,CAClB,UAAU,CAAE,KACb,CAEA,kBAAG,CAAW,CAAG,CAChB,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,WAAW,CAAE,MACd"}'
};
const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value = { text: "", files: [] } } = $$props;
  let { type } = $$props;
  let { selected = false } = $$props;
  let el;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  $$result.css.add(css);
  return `<div class="${[
    "container svelte-1cl8bqt",
    (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "") + " " + (value ? "border" : "")
  ].join(" ").trim()}"${add_attribute("this", el, 0)}><p>${escape(value.text ? value.text : "")}</p> ${each(value.files, (file) => {
    return `${file.mime_type && file.mime_type.includes("image") ? `${validate_component(Image$1, "Image").$$render($$result, { src: file.url, alt: "" }, {}, {})}` : `${file.mime_type && file.mime_type.includes("video") ? `${validate_component(Video, "Video").$$render(
      $$result,
      {
        src: file.url,
        alt: "",
        loop: true,
        is_stream: false
      },
      {},
      {}
    )}` : `${file.mime_type && file.mime_type.includes("audio") ? `<audio${add_attribute("src", file.url, 0)} controls></audio>` : `${escape(file.orig_name)}`}`}`}`;
  })} </div>`;
});

export { Example as default };
//# sourceMappingURL=Example18-DaP4wk54.js.map
