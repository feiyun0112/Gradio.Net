import { c as create_ssr_component, a as createEventDispatcher, b as add_attribute } from './ssr-RaXq3SJh.js';
import { r as resolve_wasm_src } from './DownloadLink--4obEanq.js';
import { H as Hls } from './hls-CrxM9YLy.js';

const css = {
  code: ".overlay.svelte-1y0s5gv{position:absolute;background-color:rgba(0, 0, 0, 0.4);width:100%;height:100%}.hidden.svelte-1y0s5gv{display:none}.load-wrap.svelte-1y0s5gv{display:flex;justify-content:center;align-items:center;height:100%}.loader.svelte-1y0s5gv{display:flex;position:relative;background-color:var(--border-color-accent-subdued);animation:svelte-1y0s5gv-shadowPulse 2s linear infinite;box-shadow:-24px 0 var(--border-color-accent-subdued),\n			24px 0 var(--border-color-accent-subdued);margin:var(--spacing-md);border-radius:50%;width:10px;height:10px;scale:0.5}@keyframes svelte-1y0s5gv-shadowPulse{33%{box-shadow:-24px 0 var(--border-color-accent-subdued),\n				24px 0 #fff;background:#fff}66%{box-shadow:-24px 0 #fff,\n				24px 0 #fff;background:var(--border-color-accent-subdued)}100%{box-shadow:-24px 0 #fff,\n				24px 0 var(--border-color-accent-subdued);background:#fff}}",
  map: '{"version":3,"file":"Video.svelte","sources":["Video.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport { loaded } from \\"./utils\\";\\nimport { resolve_wasm_src } from \\"@gradio/wasm/svelte\\";\\nimport Hls from \\"hls.js\\";\\nexport let src = void 0;\\nexport let muted = void 0;\\nexport let playsinline = void 0;\\nexport let preload = void 0;\\nexport let autoplay = void 0;\\nexport let controls = void 0;\\nexport let currentTime = void 0;\\nexport let duration = void 0;\\nexport let paused = void 0;\\nexport let node = void 0;\\nexport let loop;\\nexport let is_stream;\\nexport let processingVideo = false;\\nlet resolved_src;\\nlet stream_active = false;\\nlet latest_src;\\n$: {\\n    resolved_src = src;\\n    latest_src = src;\\n    const resolving_src = src;\\n    resolve_wasm_src(resolving_src).then((s) => {\\n        if (latest_src === resolving_src) {\\n            resolved_src = s;\\n        }\\n    });\\n}\\nconst dispatch = createEventDispatcher();\\nfunction load_stream(src2, is_stream2, node2) {\\n    if (!src2 || !is_stream2)\\n        return;\\n    if (!node2)\\n        return;\\n    if (Hls.isSupported() && !stream_active) {\\n        const hls = new Hls({\\n            maxBufferLength: 1,\\n            // 0.5 seconds (500 ms)\\n            maxMaxBufferLength: 1,\\n            // Maximum max buffer length in seconds\\n            lowLatencyMode: true\\n            // Enable low latency mode\\n        });\\n        hls.loadSource(src2);\\n        hls.attachMedia(node2);\\n        hls.on(Hls.Events.MANIFEST_PARSED, function () {\\n            node2.play();\\n        });\\n        hls.on(Hls.Events.ERROR, function (event, data) {\\n            console.error(\\"HLS error:\\", event, data);\\n            if (data.fatal) {\\n                switch (data.type) {\\n                    case Hls.ErrorTypes.NETWORK_ERROR:\\n                        console.error(\\"Fatal network error encountered, trying to recover\\");\\n                        hls.startLoad();\\n                        break;\\n                    case Hls.ErrorTypes.MEDIA_ERROR:\\n                        console.error(\\"Fatal media error encountered, trying to recover\\");\\n                        hls.recoverMediaError();\\n                        break;\\n                    default:\\n                        console.error(\\"Fatal error, cannot recover\\");\\n                        hls.destroy();\\n                        break;\\n                }\\n            }\\n        });\\n        stream_active = true;\\n    }\\n}\\n$: src, stream_active = false;\\n$: load_stream(src, is_stream, node);\\n<\/script>\\n\\n<!--\\nThe spread operator with `$$props` or `$$restProps` can\'t be used here\\nto pass props from the parent component to the <video> element\\nbecause of its unexpected behavior: https://github.com/sveltejs/svelte/issues/7404\\nFor example, if we add {...$$props} or {...$$restProps}, the boolean props aside it like `controls` will be compiled as string \\"true\\" or \\"false\\" on the actual DOM.\\nThen, even when `controls` is false, the compiled DOM would be `<video controls=\\"false\\">` which is equivalent to `<video controls>` since the string \\"false\\" is even truthy.\\n-->\\n<div class:hidden={!processingVideo} class=\\"overlay\\">\\n\\t<span class=\\"load-wrap\\">\\n\\t\\t<span class=\\"loader\\" />\\n\\t</span>\\n</div>\\n<video\\n\\tsrc={resolved_src}\\n\\t{muted}\\n\\t{playsinline}\\n\\t{preload}\\n\\t{autoplay}\\n\\t{controls}\\n\\t{loop}\\n\\ton:loadeddata={dispatch.bind(null, \\"loadeddata\\")}\\n\\ton:click={dispatch.bind(null, \\"click\\")}\\n\\ton:play={dispatch.bind(null, \\"play\\")}\\n\\ton:pause={dispatch.bind(null, \\"pause\\")}\\n\\ton:ended={dispatch.bind(null, \\"ended\\")}\\n\\ton:mouseover={dispatch.bind(null, \\"mouseover\\")}\\n\\ton:mouseout={dispatch.bind(null, \\"mouseout\\")}\\n\\ton:focus={dispatch.bind(null, \\"focus\\")}\\n\\ton:blur={dispatch.bind(null, \\"blur\\")}\\n\\ton:loadstart\\n\\ton:loadeddata\\n\\ton:loadedmetadata\\n\\tbind:currentTime\\n\\tbind:duration\\n\\tbind:paused\\n\\tbind:this={node}\\n\\tuse:loaded={{ autoplay: autoplay ?? false }}\\n\\tdata-testid={$$props[\\"data-testid\\"]}\\n\\tcrossorigin=\\"anonymous\\"\\n>\\n\\t<slot />\\n</video>\\n\\n<style>\\n\\t.overlay {\\n\\t\\tposition: absolute;\\n\\t\\tbackground-color: rgba(0, 0, 0, 0.4);\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.hidden {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.load-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.loader {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tbackground-color: var(--border-color-accent-subdued);\\n\\t\\tanimation: shadowPulse 2s linear infinite;\\n\\t\\tbox-shadow:\\n\\t\\t\\t-24px 0 var(--border-color-accent-subdued),\\n\\t\\t\\t24px 0 var(--border-color-accent-subdued);\\n\\t\\tmargin: var(--spacing-md);\\n\\t\\tborder-radius: 50%;\\n\\t\\twidth: 10px;\\n\\t\\theight: 10px;\\n\\t\\tscale: 0.5;\\n\\t}\\n\\n\\t@keyframes shadowPulse {\\n\\t\\t33% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 var(--border-color-accent-subdued),\\n\\t\\t\\t\\t24px 0 #fff;\\n\\t\\t\\tbackground: #fff;\\n\\t\\t}\\n\\t\\t66% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 #fff,\\n\\t\\t\\t\\t24px 0 #fff;\\n\\t\\t\\tbackground: var(--border-color-accent-subdued);\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\tbox-shadow:\\n\\t\\t\\t\\t-24px 0 #fff,\\n\\t\\t\\t\\t24px 0 var(--border-color-accent-subdued);\\n\\t\\t\\tbackground: #fff;\\n\\t\\t}\\n\\t}</style>\\n"],"names":[],"mappings":"AAwHC,uBAAS,CACR,QAAQ,CAAE,QAAQ,CAClB,gBAAgB,CAAE,KAAK,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,GAAG,CAAC,CACpC,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IACT,CAEA,sBAAQ,CACP,OAAO,CAAE,IACV,CAEA,yBAAW,CACV,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,IACT,CAEA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,gBAAgB,CAAE,IAAI,6BAA6B,CAAC,CACpD,SAAS,CAAE,0BAAW,CAAC,EAAE,CAAC,MAAM,CAAC,QAAQ,CACzC,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAAC;AAC9C,GAAG,IAAI,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAC1C,MAAM,CAAE,IAAI,YAAY,CAAC,CACzB,aAAa,CAAE,GAAG,CAClB,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,GACR,CAEA,WAAW,0BAAY,CACtB,GAAI,CACH,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAAC;AAC/C,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CACZ,UAAU,CAAE,IACb,CACA,GAAI,CACH,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,CAAC;AACjB,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,CACZ,UAAU,CAAE,IAAI,6BAA6B,CAC9C,CACA,IAAK,CACJ,UAAU,CACT,KAAK,CAAC,CAAC,CAAC,IAAI,CAAC;AACjB,IAAI,IAAI,CAAC,CAAC,CAAC,IAAI,6BAA6B,CAAC,CAC1C,UAAU,CAAE,IACb,CACD"}'
};
const Video = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { src = void 0 } = $$props;
  let { muted = void 0 } = $$props;
  let { playsinline = void 0 } = $$props;
  let { preload = void 0 } = $$props;
  let { autoplay = void 0 } = $$props;
  let { controls = void 0 } = $$props;
  let { currentTime = void 0 } = $$props;
  let { duration = void 0 } = $$props;
  let { paused = void 0 } = $$props;
  let { node = void 0 } = $$props;
  let { loop } = $$props;
  let { is_stream } = $$props;
  let { processingVideo = false } = $$props;
  let resolved_src;
  let stream_active = false;
  let latest_src;
  createEventDispatcher();
  function load_stream(src2, is_stream2, node2) {
    if (!src2 || !is_stream2)
      return;
    if (!node2)
      return;
    if (Hls.isSupported() && !stream_active) {
      const hls = new Hls({
        maxBufferLength: 1,
        // 0.5 seconds (500 ms)
        maxMaxBufferLength: 1,
        // Maximum max buffer length in seconds
        lowLatencyMode: true
      });
      hls.loadSource(src2);
      hls.attachMedia(node2);
      hls.on(Hls.Events.MANIFEST_PARSED, function() {
        node2.play();
      });
      hls.on(Hls.Events.ERROR, function(event, data) {
        console.error("HLS error:", event, data);
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              console.error("Fatal network error encountered, trying to recover");
              hls.startLoad();
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              console.error("Fatal media error encountered, trying to recover");
              hls.recoverMediaError();
              break;
            default:
              console.error("Fatal error, cannot recover");
              hls.destroy();
              break;
          }
        }
      });
      stream_active = true;
    }
  }
  if ($$props.src === void 0 && $$bindings.src && src !== void 0)
    $$bindings.src(src);
  if ($$props.muted === void 0 && $$bindings.muted && muted !== void 0)
    $$bindings.muted(muted);
  if ($$props.playsinline === void 0 && $$bindings.playsinline && playsinline !== void 0)
    $$bindings.playsinline(playsinline);
  if ($$props.preload === void 0 && $$bindings.preload && preload !== void 0)
    $$bindings.preload(preload);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.controls === void 0 && $$bindings.controls && controls !== void 0)
    $$bindings.controls(controls);
  if ($$props.currentTime === void 0 && $$bindings.currentTime && currentTime !== void 0)
    $$bindings.currentTime(currentTime);
  if ($$props.duration === void 0 && $$bindings.duration && duration !== void 0)
    $$bindings.duration(duration);
  if ($$props.paused === void 0 && $$bindings.paused && paused !== void 0)
    $$bindings.paused(paused);
  if ($$props.node === void 0 && $$bindings.node && node !== void 0)
    $$bindings.node(node);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.is_stream === void 0 && $$bindings.is_stream && is_stream !== void 0)
    $$bindings.is_stream(is_stream);
  if ($$props.processingVideo === void 0 && $$bindings.processingVideo && processingVideo !== void 0)
    $$bindings.processingVideo(processingVideo);
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
  {
    stream_active = false;
  }
  {
    load_stream(src, is_stream, node);
  }
  return ` <div class="${["overlay svelte-1y0s5gv", !processingVideo ? "hidden" : ""].join(" ").trim()}" data-svelte-h="svelte-mez4j5"><span class="load-wrap svelte-1y0s5gv"><span class="loader svelte-1y0s5gv"></span></span></div> <video${add_attribute("src", resolved_src, 0)} ${muted ? "muted" : ""} ${playsinline ? "playsinline" : ""}${add_attribute("preload", preload, 0)} ${autoplay ? "autoplay" : ""} ${controls ? "controls" : ""} ${loop ? "loop" : ""}${add_attribute("data-testid", $$props["data-testid"], 0)} crossorigin="anonymous"${add_attribute("currentTime", currentTime, 0)}${add_attribute("paused", paused, 0)}${add_attribute("this", node, 0)}>${slots.default ? slots.default({}) : ``}</video>`;
});

export { Video as V };
//# sourceMappingURL=Video-_epjqq1V.js.map
