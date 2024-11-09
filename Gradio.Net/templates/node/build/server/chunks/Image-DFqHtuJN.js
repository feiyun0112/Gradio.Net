import { c as create_ssr_component, i as compute_rest_props, j as spread, l as escape_attribute_value, k as escape_object } from './ssr-RaXq3SJh.js';
import { r as resolve_wasm_src } from './DownloadLink--4obEanq.js';

const css = {
  code: "img.svelte-1pijsyv{object-fit:cover}",
  map: '{"version":3,"file":"Image.svelte","sources":["Image.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { resolve_wasm_src } from \\"@gradio/wasm/svelte\\";\\nexport let src = void 0;\\nlet resolved_src;\\nlet latest_src;\\n$: {\\n    resolved_src = src;\\n    latest_src = src;\\n    const resolving_src = src;\\n    resolve_wasm_src(resolving_src).then((s) => {\\n        if (latest_src === resolving_src) {\\n            resolved_src = s;\\n        }\\n    });\\n}\\n<\/script>\\n\\n<!-- svelte-ignore a11y-missing-attribute -->\\n<img src={resolved_src} {...$$restProps} on:load />\\n\\n<style>\\n\\timg {\\n\\t\\tobject-fit: cover;\\n\\t}</style>\\n"],"names":[],"mappings":"AAoBC,kBAAI,CACH,UAAU,CAAE,KACb"}'
};
const Image = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let $$restProps = compute_rest_props($$props, ["src"]);
  let { src = void 0 } = $$props;
  let resolved_src;
  let latest_src;
  if ($$props.src === void 0 && $$bindings.src && src !== void 0)
    $$bindings.src(src);
  $$result.css.add(css);
  {
    {
      resolved_src = src;
      latest_src = src;
      const resolving_src = src;
      resolve_wasm_src(resolving_src).then((s) => {
        if (latest_src === resolving_src) {
          resolved_src = s;
        }
      });
    }
  }
  return ` <img${spread(
    [
      {
        src: escape_attribute_value(resolved_src)
      },
      escape_object($$restProps)
    ],
    { classes: "svelte-1pijsyv" }
  )}>`;
});
const Image$1 = Image;

export { Image$1 as I };
//# sourceMappingURL=Image-DFqHtuJN.js.map
