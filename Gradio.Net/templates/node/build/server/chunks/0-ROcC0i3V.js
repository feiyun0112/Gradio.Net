import { r as redirect } from './index3-DyoisQP2.js';

function load({ url }) {
  if (url.pathname !== "/") {
    redirect(308, "/");
  }
}

var _layout_server_ts = /*#__PURE__*/Object.freeze({
  __proto__: null,
  load: load
});

const index = 0;
let component_cache;
const component = async () => component_cache ??= (await import('./_layout.svelte-Ba36fIRN.js')).default;
const server_id = "src/routes/+layout.server.ts";
const imports = ["_app/immutable/nodes/0.DNkWqxrq.js"];
const stylesheets = ["_app/immutable/assets/0.BROieVb2.css"];
const fonts = [];

export { component, fonts, imports, index, _layout_server_ts as server, server_id, stylesheets };
//# sourceMappingURL=0-ROcC0i3V.js.map
