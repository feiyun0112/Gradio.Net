import { c as create_ssr_component, v as validate_component } from './ssr-RaXq3SJh.js';
import { V as Video } from './Video-_epjqq1V.js';
import './2-B6LMYTAg.js';
import './DownloadLink--4obEanq.js';
import './hls-CrxM9YLy.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".container.svelte-1de9zxs{flex:none;max-width:none}.container.svelte-1de9zxs video{width:var(--size-full);height:var(--size-full);object-fit:cover}.container.svelte-1de9zxs:hover,.container.selected.svelte-1de9zxs{border-color:var(--border-color-accent)}.container.table.svelte-1de9zxs{margin:0 auto;border:2px solid var(--border-color-primary);border-radius:var(--radius-lg);overflow:hidden;width:var(--size-20);height:var(--size-20);object-fit:cover}.container.gallery.svelte-1de9zxs{height:var(--size-20);max-height:var(--size-20);object-fit:cover}",
  map: '{"version":3,"file":"Example.svelte","sources":["Example.svelte"],"sourcesContent":["<script lang=\\"ts\\">import Video from \\"./shared/Video.svelte\\";\\nimport { playable } from \\"./shared/utils\\";\\nimport {} from \\"@gradio/client\\";\\nexport let type;\\nexport let selected = false;\\nexport let value;\\nexport let loop;\\nlet video;\\nasync function init() {\\n    video.muted = true;\\n    video.playsInline = true;\\n    video.controls = false;\\n    video.setAttribute(\\"muted\\", \\"\\");\\n    await video.play();\\n    video.pause();\\n}\\n<\/script>\\n\\n{#if value}\\n\\t{#if playable()}\\n\\t\\t<div\\n\\t\\t\\tclass=\\"container\\"\\n\\t\\t\\tclass:table={type === \\"table\\"}\\n\\t\\t\\tclass:gallery={type === \\"gallery\\"}\\n\\t\\t\\tclass:selected\\n\\t\\t>\\n\\t\\t\\t<Video\\n\\t\\t\\t\\tmuted\\n\\t\\t\\t\\tplaysinline\\n\\t\\t\\t\\tbind:node={video}\\n\\t\\t\\t\\ton:loadeddata={init}\\n\\t\\t\\t\\ton:mouseover={video.play.bind(video)}\\n\\t\\t\\t\\ton:mouseout={video.pause.bind(video)}\\n\\t\\t\\t\\tsrc={value?.video.url}\\n\\t\\t\\t\\tis_stream={false}\\n\\t\\t\\t\\t{loop}\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t<div>{value}</div>\\n\\t{/if}\\n{/if}\\n\\n<style>\\n\\t.container {\\n\\t\\tflex: none;\\n\\t\\tmax-width: none;\\n\\t}\\n\\t.container :global(video) {\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\t.container:hover,\\n\\t.container.selected {\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t}\\n\\t.container.table {\\n\\t\\tmargin: 0 auto;\\n\\t\\tborder: 2px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t\\toverflow: hidden;\\n\\t\\twidth: var(--size-20);\\n\\t\\theight: var(--size-20);\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\t.container.gallery {\\n\\t\\theight: var(--size-20);\\n\\t\\tmax-height: var(--size-20);\\n\\t\\tobject-fit: cover;\\n\\t}</style>\\n"],"names":[],"mappings":"AA4CC,yBAAW,CACV,IAAI,CAAE,IAAI,CACV,SAAS,CAAE,IACZ,CACA,yBAAU,CAAS,KAAO,CACzB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,UAAU,CAAE,KACb,CAEA,yBAAU,MAAM,CAChB,UAAU,wBAAU,CACnB,YAAY,CAAE,IAAI,qBAAqB,CACxC,CACA,UAAU,qBAAO,CAChB,MAAM,CAAE,CAAC,CAAC,IAAI,CACd,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAC7C,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,QAAQ,CAAE,MAAM,CAChB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,UAAU,CAAE,KACb,CAEA,UAAU,uBAAS,CAClB,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,UAAU,CAAE,IAAI,SAAS,CAAC,CAC1B,UAAU,CAAE,KACb"}'
};
const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { type } = $$props;
  let { selected = false } = $$props;
  let { value } = $$props;
  let { loop } = $$props;
  let video;
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `${value ? `${`<div class="${[
      "container svelte-1de9zxs",
      (type === "table" ? "table" : "") + " " + (type === "gallery" ? "gallery" : "") + " " + (selected ? "selected" : "")
    ].join(" ").trim()}">${validate_component(Video, "Video").$$render(
      $$result,
      {
        muted: true,
        playsinline: true,
        src: value?.video.url,
        is_stream: false,
        loop,
        node: video
      },
      {
        node: ($$value) => {
          video = $$value;
          $$settled = false;
        }
      },
      {}
    )}</div>`}` : ``}`;
  } while (!$$settled);
  return $$rendered;
});

export { Example as default };
//# sourceMappingURL=Example23-M3xiVOYG.js.map
