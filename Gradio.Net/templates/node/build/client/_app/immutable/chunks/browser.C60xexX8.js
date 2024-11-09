import { g as getDefaultExportFromCjs } from "./2.BqWhUxOo.js";
function _mergeNamespaces(n, m) {
  for (var i = 0; i < m.length; i++) {
    const e = m[i];
    if (typeof e !== "string" && !Array.isArray(e)) {
      for (const k in e) {
        if (k !== "default" && !(k in n)) {
          const d = Object.getOwnPropertyDescriptor(e, k);
          if (d) {
            Object.defineProperty(n, k, d.get ? d : {
              enumerable: true,
              get: () => e[k]
            });
          }
        }
      }
    }
  }
  return Object.freeze(Object.defineProperty(n, Symbol.toStringTag, { value: "Module" }));
}
var browser = function() {
  throw new Error(
    "ws does not work in the browser. Browser clients must use the native WebSocket object"
  );
};
const browser$1 = /* @__PURE__ */ getDefaultExportFromCjs(browser);
const browser$2 = /* @__PURE__ */ _mergeNamespaces({
  __proto__: null,
  default: browser$1
}, [browser]);
export {
  browser$2 as b
};
