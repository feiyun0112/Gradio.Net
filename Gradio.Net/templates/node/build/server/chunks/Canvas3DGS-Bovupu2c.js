import { c as create_ssr_component, b as add_attribute } from './ssr-RaXq3SJh.js';
import { r as resolve_wasm_src } from './DownloadLink--4obEanq.js';

const Canvas3DGS = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let url;
  let { value } = $$props;
  let { zoom_speed } = $$props;
  let { pan_speed } = $$props;
  let { resolved_url = void 0 } = $$props;
  let latest_url;
  let canvas;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.zoom_speed === void 0 && $$bindings.zoom_speed && zoom_speed !== void 0)
    $$bindings.zoom_speed(zoom_speed);
  if ($$props.pan_speed === void 0 && $$bindings.pan_speed && pan_speed !== void 0)
    $$bindings.pan_speed(pan_speed);
  if ($$props.resolved_url === void 0 && $$bindings.resolved_url && resolved_url !== void 0)
    $$bindings.resolved_url(resolved_url);
  url = value.url;
  {
    {
      resolved_url = url;
      if (url) {
        latest_url = url;
        const resolving_url = url;
        resolve_wasm_src(url).then((resolved) => {
          if (latest_url === resolving_url) {
            resolved_url = resolved ?? void 0;
          } else {
            resolved && URL.revokeObjectURL(resolved);
          }
        });
      }
    }
  }
  return `<canvas${add_attribute("this", canvas, 0)}></canvas>`;
});

export { Canvas3DGS as default };
//# sourceMappingURL=Canvas3DGS-Bovupu2c.js.map
