import { c as create_ssr_component, i as compute_rest_props, a as createEventDispatcher, j as spread, k as escape_object, l as escape_attribute_value, g as getContext } from './ssr-RaXq3SJh.js';

const WORKER_PROXY_CONTEXT_KEY = "WORKER_PROXY_CONTEXT_KEY";
function getWorkerProxyContext() {
  return getContext(WORKER_PROXY_CONTEXT_KEY);
}
const FAKE_LITE_HOST = "lite.local";
function is_self_host(url) {
  return url.host === window.location.host || url.host === "localhost:7860" || url.host === "127.0.0.1:7860" || // Ref: https://github.com/gradio-app/gradio/blob/v3.32.0/js/app/src/Index.svelte#L194
  url.host === FAKE_LITE_HOST;
}
function getHeaderValue(headers, key) {
  const unifiedKey = key.toLowerCase();
  for (const [k, v] of Object.entries(headers)) {
    if (k.toLowerCase() === unifiedKey) {
      return v;
    }
  }
}
function should_proxy_wasm_src(src) {
  const is_browser = typeof window !== "undefined";
  if (src == null || !is_browser) {
    return false;
  }
  const url = new URL(src, window.location.href);
  if (!is_self_host(url)) {
    return false;
  }
  if (url.protocol !== "http:" && url.protocol !== "https:") {
    return false;
  }
  return true;
}
let maybeWorkerProxy;
async function resolve_wasm_src(src) {
  const is_browser = typeof window !== "undefined";
  if (src == null || !is_browser || !should_proxy_wasm_src(src)) {
    return src;
  }
  if (maybeWorkerProxy == null) {
    try {
      maybeWorkerProxy = getWorkerProxyContext();
    } catch (e) {
      return src;
    }
  }
  if (maybeWorkerProxy == null) {
    return src;
  }
  const url = new URL(src, window.location.href);
  const path = url.pathname;
  return maybeWorkerProxy.httpRequest({
    method: "GET",
    path,
    headers: {},
    query_string: ""
  }).then((response) => {
    if (response.status !== 200) {
      throw new Error(`Failed to get file ${path} from the Wasm worker.`);
    }
    const blob = new Blob([response.body], {
      type: getHeaderValue(response.headers, "content-type")
    });
    const blobUrl = URL.createObjectURL(blob);
    return blobUrl;
  });
}
const css = {
  code: ".unstyled-link.svelte-1s8vnbx{all:unset;cursor:pointer}",
  map: '{"version":3,"file":"DownloadLink.svelte","sources":["DownloadLink.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, onMount } from \\"svelte\\";\\nimport { getWorkerProxyContext } from \\"./context\\";\\nimport { should_proxy_wasm_src } from \\"./file-url\\";\\nimport { getHeaderValue } from \\"../src/http\\";\\nexport let href = void 0;\\nexport let download;\\nconst dispatch = createEventDispatcher();\\nlet is_downloading = false;\\nconst worker_proxy = getWorkerProxyContext();\\nasync function wasm_click_handler() {\\n    if (is_downloading) {\\n        return;\\n    }\\n    dispatch(\\"click\\");\\n    if (href == null) {\\n        throw new Error(\\"href is not defined.\\");\\n    }\\n    if (worker_proxy == null) {\\n        throw new Error(\\"Wasm worker proxy is not available.\\");\\n    }\\n    const url = new URL(href, window.location.href);\\n    const path = url.pathname;\\n    is_downloading = true;\\n    worker_proxy.httpRequest({\\n        method: \\"GET\\",\\n        path,\\n        headers: {},\\n        query_string: \\"\\"\\n    }).then((response) => {\\n        if (response.status !== 200) {\\n            throw new Error(`Failed to get file ${path} from the Wasm worker.`);\\n        }\\n        const blob = new Blob([response.body], {\\n            type: getHeaderValue(response.headers, \\"content-type\\")\\n        });\\n        const blobUrl = URL.createObjectURL(blob);\\n        const link = document.createElement(\\"a\\");\\n        link.href = blobUrl;\\n        link.download = download;\\n        link.click();\\n        URL.revokeObjectURL(blobUrl);\\n    }).finally(() => {\\n        is_downloading = false;\\n    });\\n}\\n<\/script>\\n\\n{#if worker_proxy && should_proxy_wasm_src(href)}\\n\\t{#if is_downloading}\\n\\t\\t<slot />\\n\\t{:else}\\n\\t\\t<a {...$$restProps} {href} on:click|preventDefault={wasm_click_handler}>\\n\\t\\t\\t<slot />\\n\\t\\t</a>\\n\\t{/if}\\n{:else}\\n\\t<a\\n\\t\\tstyle:position=\\"relative\\"\\n\\t\\tclass=\\"download-link\\"\\n\\t\\t{href}\\n\\t\\ttarget={typeof window !== \\"undefined\\" && window.__is_colab__\\n\\t\\t\\t? \\"_blank\\"\\n\\t\\t\\t: null}\\n\\t\\trel=\\"noopener noreferrer\\"\\n\\t\\t{download}\\n\\t\\t{...$$restProps}\\n\\t\\ton:click={dispatch.bind(null, \\"click\\")}\\n\\t>\\n\\t\\t<slot />\\n\\t</a>\\n{/if}\\n\\n<style>\\n\\t.unstyled-link {\\n\\t\\tall: unset;\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"AAyEC,6BAAe,CACd,GAAG,CAAE,KAAK,CACV,MAAM,CAAE,OACT"}'
};
const DownloadLink = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let $$restProps = compute_rest_props($$props, ["href", "download"]);
  let { href = void 0 } = $$props;
  let { download } = $$props;
  createEventDispatcher();
  const worker_proxy = getWorkerProxyContext();
  if ($$props.href === void 0 && $$bindings.href && href !== void 0)
    $$bindings.href(href);
  if ($$props.download === void 0 && $$bindings.download && download !== void 0)
    $$bindings.download(download);
  $$result.css.add(css);
  return `${worker_proxy && should_proxy_wasm_src(href) ? `${`<a${spread([escape_object($$restProps), { href: escape_attribute_value(href) }], { classes: "svelte-1s8vnbx" })}>${slots.default ? slots.default({}) : ``}</a>`}` : `<a${spread(
    [
      { class: "download-link" },
      { href: escape_attribute_value(href) },
      {
        target: escape_attribute_value(typeof window !== "undefined" && window.__is_colab__ ? "_blank" : null)
      },
      { rel: "noopener noreferrer" },
      {
        download: escape_attribute_value(download)
      },
      escape_object($$restProps)
    ],
    {
      classes: "svelte-1s8vnbx",
      styles: { "position": `relative` }
    }
  )}>${slots.default ? slots.default({}) : ``}</a>`}`;
});

export { DownloadLink as D, resolve_wasm_src as r };
//# sourceMappingURL=DownloadLink--4obEanq.js.map
