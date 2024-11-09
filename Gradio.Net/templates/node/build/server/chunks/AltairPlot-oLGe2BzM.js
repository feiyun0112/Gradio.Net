import { c as create_ssr_component, o as onDestroy, b as add_attribute, e as escape } from './ssr-RaXq3SJh.js';
import { a6 as colors } from './2-B6LMYTAg.js';
import { g as get_next_color } from './color-8KH1gdSw.js';
import './vega-embed.module-BxvS8bt3.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

function set_config(spec, computed_style, chart_type, colors2) {
  let accentColor = computed_style.getPropertyValue("--color-accent");
  let bodyTextColor = computed_style.getPropertyValue("--body-text-color");
  let borderColorPrimary = computed_style.getPropertyValue(
    "--border-color-primary"
  );
  let fontFamily = computed_style.fontFamily;
  let titleWeight = computed_style.getPropertyValue(
    "--block-title-text-weight"
  );
  const fontToPxVal = (font) => {
    return font.endsWith("px") ? parseFloat(font.slice(0, -2)) : 12;
  };
  let textSizeMd = fontToPxVal(computed_style.getPropertyValue("--text-md"));
  let textSizeSm = fontToPxVal(computed_style.getPropertyValue("--text-sm"));
  let config = {
    autosize: { type: "fit", contains: "padding" },
    axis: {
      labelFont: fontFamily,
      labelColor: bodyTextColor,
      titleFont: fontFamily,
      titleColor: bodyTextColor,
      tickColor: borderColorPrimary,
      labelFontSize: textSizeSm,
      gridColor: borderColorPrimary,
      titleFontWeight: "normal",
      titleFontSize: textSizeSm,
      labelFontWeight: "normal",
      domain: false,
      labelAngle: 0
    },
    legend: {
      labelColor: bodyTextColor,
      labelFont: fontFamily,
      titleColor: bodyTextColor,
      titleFont: fontFamily,
      titleFontWeight: "normal",
      titleFontSize: textSizeSm,
      labelFontWeight: "normal",
      offset: 2
    },
    title: {
      color: bodyTextColor,
      font: fontFamily,
      fontSize: textSizeMd,
      fontWeight: titleWeight,
      anchor: "middle"
    },
    view: {
      stroke: borderColorPrimary
    }
  };
  spec.config = config;
  let encoding = spec.encoding;
  let layer = spec.layer;
  switch (chart_type) {
    case "scatter":
      spec.config.mark = { stroke: accentColor };
      if (encoding.color && encoding.color.type == "nominal") {
        encoding.color.scale.range = encoding.color.scale.range.map(
          (_, i) => get_color(colors2, i)
        );
      } else if (encoding.color && encoding.color.type == "quantitative") {
        encoding.color.scale.range = ["#eff6ff", "#1e3a8a"];
        encoding.color.scale.range.interpolate = "hsl";
      }
      break;
    case "line":
      spec.config.mark = { stroke: accentColor, cursor: "crosshair" };
      layer.forEach((d) => {
        if (d.encoding.color) {
          d.encoding.color.scale.range = d.encoding.color.scale.range.map(
            (_, i) => get_color(colors2, i)
          );
        }
      });
      break;
    case "bar":
      spec.config.mark = { opacity: 0.8, fill: accentColor };
      if (encoding.color) {
        encoding.color.scale.range = encoding.color.scale.range.map(
          (_, i) => get_color(colors2, i)
        );
      }
      break;
  }
  return spec;
}
function get_color(colors$1, index) {
  let current_color = colors$1[index % colors$1.length];
  if (current_color && current_color in colors) {
    return colors[current_color]?.primary;
  } else if (!current_color) {
    return colors[get_next_color(index)].primary;
  }
  return current_color;
}
const css = {
  code: ".altair.svelte-1qhqpn7 canvas{padding:6px}.altair.svelte-1qhqpn7 .vega-embed{padding:0px !important}.altair.svelte-1qhqpn7 .vega-actions{right:0px !important}.layout.svelte-1qhqpn7{display:flex;flex-direction:column;justify-content:center;align-items:center;width:var(--size-full);height:var(--size-full);color:var(--body-text-color)}.altair.svelte-1qhqpn7{display:flex;flex-direction:column;justify-content:center;align-items:center;width:var(--size-full);height:var(--size-full)}.caption.svelte-1qhqpn7{font-size:var(--text-sm);margin-bottom:6px}#vg-tooltip-element{font-family:var(--font) !important;font-size:var(--text-xs) !important;box-shadow:none !important;background-color:var(--block-background-fill) !important;border:1px solid var(--border-color-primary) !important;color:var(--body-text-color) !important}#vg-tooltip-element .key{color:var(--body-text-color-subdued) !important}",
  map: '{"version":3,"file":"AltairPlot.svelte","sources":["AltairPlot.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { set_config } from \\"./altair_utils\\";\\nimport { onMount, onDestroy } from \\"svelte\\";\\nimport vegaEmbed from \\"vega-embed\\";\\nexport let value;\\nexport let colors = [];\\nexport let caption;\\nexport let show_actions_button;\\nexport let gradio;\\nlet element;\\nlet parent_element;\\nlet view;\\nexport let _selectable;\\nlet computed_style = window.getComputedStyle(document.body);\\nlet old_spec;\\nlet spec_width;\\n$: plot = value?.plot;\\n$: spec = JSON.parse(plot);\\n$: if (spec && spec.params && !_selectable) {\\n    spec.params = spec.params.filter((param) => param.name !== \\"brush\\");\\n}\\n$: if (old_spec !== spec) {\\n    old_spec = spec;\\n    spec_width = spec.width;\\n}\\n$: if (value.chart) {\\n    spec = set_config(spec, computed_style, value.chart, colors);\\n}\\n$: fit_width_to_parent = spec.encoding?.column?.field || spec.encoding?.row?.field || value.chart === void 0 ? false : true;\\nconst get_width = () => {\\n    return Math.min(parent_element.offsetWidth, spec_width || parent_element.offsetWidth);\\n};\\nlet resize_callback = () => {\\n};\\nconst renderPlot = () => {\\n    if (fit_width_to_parent) {\\n        spec.width = get_width();\\n    }\\n    vegaEmbed(element, spec, { actions: show_actions_button }).then(function (result) {\\n        view = result.view;\\n        resize_callback = () => {\\n            view.signal(\\"width\\", get_width()).run();\\n        };\\n        if (!_selectable)\\n            return;\\n        const callback = (event, item) => {\\n            const brushValue = view.signal(\\"brush\\");\\n            if (brushValue) {\\n                if (Object.keys(brushValue).length === 0) {\\n                    gradio.dispatch(\\"select\\", {\\n                        value: null,\\n                        index: null,\\n                        selected: false\\n                    });\\n                }\\n                else {\\n                    const key = Object.keys(brushValue)[0];\\n                    let range = brushValue[key].map((x) => x / 1e3);\\n                    gradio.dispatch(\\"select\\", {\\n                        value: brushValue,\\n                        index: range,\\n                        selected: true\\n                    });\\n                }\\n            }\\n        };\\n        view.addEventListener(\\"mouseup\\", callback);\\n        view.addEventListener(\\"touchup\\", callback);\\n    });\\n};\\nlet resizeObserver = new ResizeObserver(() => {\\n    if (fit_width_to_parent && spec.width !== parent_element.offsetWidth) {\\n        resize_callback();\\n    }\\n});\\nonMount(() => {\\n    renderPlot();\\n    resizeObserver.observe(parent_element);\\n});\\nonDestroy(() => {\\n    resizeObserver.disconnect();\\n});\\n<\/script>\\n\\n<div data-testid={\\"altair\\"} class=\\"altair layout\\" bind:this={parent_element}>\\n\\t<div bind:this={element}></div>\\n\\t{#if caption}\\n\\t\\t<div class=\\"caption layout\\">\\n\\t\\t\\t{caption}\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.altair :global(canvas) {\\n\\t\\tpadding: 6px;\\n\\t}\\n\\t.altair :global(.vega-embed) {\\n\\t\\tpadding: 0px !important;\\n\\t}\\n\\t.altair :global(.vega-actions) {\\n\\t\\tright: 0px !important;\\n\\t}\\n\\t.layout {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\t.altair {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\twidth: var(--size-full);\\n\\t\\theight: var(--size-full);\\n\\t}\\n\\t.caption {\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\tmargin-bottom: 6px;\\n\\t}\\n\\t:global(#vg-tooltip-element) {\\n\\t\\tfont-family: var(--font) !important;\\n\\t\\tfont-size: var(--text-xs) !important;\\n\\t\\tbox-shadow: none !important;\\n\\t\\tbackground-color: var(--block-background-fill) !important;\\n\\t\\tborder: 1px solid var(--border-color-primary) !important;\\n\\t\\tcolor: var(--body-text-color) !important;\\n\\t}\\n\\t:global(#vg-tooltip-element .key) {\\n\\t\\tcolor: var(--body-text-color-subdued) !important;\\n\\t}</style>\\n"],"names":[],"mappings":"AA6FC,sBAAO,CAAS,MAAQ,CACvB,OAAO,CAAE,GACV,CACA,sBAAO,CAAS,WAAa,CAC5B,OAAO,CAAE,GAAG,CAAC,UACd,CACA,sBAAO,CAAS,aAAe,CAC9B,KAAK,CAAE,GAAG,CAAC,UACZ,CACA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CAAC,CACxB,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CACA,sBAAQ,CACP,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,MAAM,CAAE,IAAI,WAAW,CACxB,CACA,uBAAS,CACR,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,aAAa,CAAE,GAChB,CACQ,mBAAqB,CAC5B,WAAW,CAAE,IAAI,MAAM,CAAC,CAAC,UAAU,CACnC,SAAS,CAAE,IAAI,SAAS,CAAC,CAAC,UAAU,CACpC,UAAU,CAAE,IAAI,CAAC,UAAU,CAC3B,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAAC,UAAU,CACzD,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAAC,UAAU,CACxD,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAAC,UAC/B,CACQ,wBAA0B,CACjC,KAAK,CAAE,IAAI,yBAAyB,CAAC,CAAC,UACvC"}'
};
const AltairPlot = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let plot;
  let spec;
  let fit_width_to_parent;
  let { value } = $$props;
  let { colors: colors2 = [] } = $$props;
  let { caption } = $$props;
  let { show_actions_button } = $$props;
  let { gradio } = $$props;
  let element;
  let parent_element;
  let { _selectable } = $$props;
  let computed_style = window.getComputedStyle(document.body);
  let old_spec;
  let resizeObserver = new ResizeObserver(() => {
    if (fit_width_to_parent && spec.width !== parent_element.offsetWidth) ;
  });
  onDestroy(() => {
    resizeObserver.disconnect();
  });
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.colors === void 0 && $$bindings.colors && colors2 !== void 0)
    $$bindings.colors(colors2);
  if ($$props.caption === void 0 && $$bindings.caption && caption !== void 0)
    $$bindings.caption(caption);
  if ($$props.show_actions_button === void 0 && $$bindings.show_actions_button && show_actions_button !== void 0)
    $$bindings.show_actions_button(show_actions_button);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props._selectable === void 0 && $$bindings._selectable && _selectable !== void 0)
    $$bindings._selectable(_selectable);
  $$result.css.add(css);
  plot = value?.plot;
  spec = JSON.parse(plot);
  {
    if (spec && spec.params && !_selectable) {
      spec.params = spec.params.filter((param) => param.name !== "brush");
    }
  }
  {
    if (value.chart) {
      spec = set_config(spec, computed_style, value.chart, colors2);
    }
  }
  {
    if (old_spec !== spec) {
      old_spec = spec;
      spec.width;
    }
  }
  fit_width_to_parent = spec.encoding?.column?.field || spec.encoding?.row?.field || value.chart === void 0 ? false : true;
  return `<div${add_attribute("data-testid", "altair", 0)} class="altair layout svelte-1qhqpn7"${add_attribute("this", parent_element, 0)}><div${add_attribute("this", element, 0)}></div> ${caption ? `<div class="caption layout svelte-1qhqpn7">${escape(caption)}</div>` : ``} </div>`;
});

export { AltairPlot as default };
//# sourceMappingURL=AltairPlot-oLGe2BzM.js.map
