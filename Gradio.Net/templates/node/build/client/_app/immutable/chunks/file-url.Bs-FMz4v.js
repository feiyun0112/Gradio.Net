import { getContext } from "../../../svelte/svelte.js";
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
export {
  getHeaderValue as a,
  getWorkerProxyContext as g,
  resolve_wasm_src as r,
  should_proxy_wasm_src as s
};
