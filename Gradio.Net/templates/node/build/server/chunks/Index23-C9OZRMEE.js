import { B as Block, S as Static, N as BlockTitle, g as Empty, ao as LineChart, a5 as globals } from './2-B6LMYTAg.js';
import { c as create_ssr_component, v as validate_component, e as escape, b as add_attribute } from './ssr-RaXq3SJh.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const { Object: Object_1 } = globals;
const css = {
  code: "div.svelte-10k9m4v{width:100%}#vg-tooltip-element{font-family:var(--font) !important;font-size:var(--text-xs) !important;box-shadow:none !important;background-color:var(--block-background-fill) !important;border:1px solid var(--border-color-primary) !important;color:var(--body-text-color) !important}#vg-tooltip-element .key{color:var(--body-text-color-subdued) !important}.caption.svelte-10k9m4v{padding:0 4px;margin:0;text-align:center}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { BlockTitle } from \\"@gradio/atoms\\";\\nimport { Block } from \\"@gradio/atoms\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport { onMount } from \\"svelte\\";\\nimport { LineChart as LabelIcon } from \\"@gradio/icons\\";\\nimport { Empty } from \\"@gradio/atoms\\";\\nexport let value;\\nexport let x;\\nexport let y;\\nexport let color = null;\\nexport let root;\\n$: unique_colors = color && value && value.datatypes[color] === \\"nominal\\" ? Array.from(new Set(_data.map((d) => d[color]))) : [];\\nexport let title = null;\\nexport let x_title = null;\\nexport let y_title = null;\\nexport let color_title = null;\\nexport let x_bin = null;\\nexport let y_aggregate = void 0;\\nexport let color_map = null;\\nexport let x_lim = null;\\nexport let y_lim = null;\\nexport let x_label_angle = null;\\nexport let y_label_angle = null;\\nexport let x_axis_labels_visible = true;\\nexport let caption = null;\\nexport let sort = null;\\nexport let tooltip = \\"axis\\";\\nfunction reformat_sort(_sort2) {\\n    if (_sort2 === \\"x\\") {\\n        return \\"ascending\\";\\n    }\\n    else if (_sort2 === \\"-x\\") {\\n        return \\"descending\\";\\n    }\\n    else if (_sort2 === \\"y\\") {\\n        return { field: y, order: \\"ascending\\" };\\n    }\\n    else if (_sort2 === \\"-y\\") {\\n        return { field: y, order: \\"descending\\" };\\n    }\\n    else if (_sort2 === null) {\\n        return void 0;\\n    }\\n    else if (Array.isArray(_sort2)) {\\n        return _sort2;\\n    }\\n}\\n$: _sort = reformat_sort(sort);\\nexport let _selectable = false;\\nlet _data;\\nexport let gradio;\\n$: x_temporal = value && value.datatypes[x] === \\"temporal\\";\\n$: _x_lim = x_lim && x_temporal ? [x_lim[0] * 1e3, x_lim[1] * 1e3] : x_lim;\\nlet _x_bin;\\nlet mouse_down_on_chart = false;\\nconst SUFFIX_DURATION = {\\n    s: 1,\\n    m: 60,\\n    h: 60 * 60,\\n    d: 24 * 60 * 60\\n};\\n$: _x_bin = x_bin ? typeof x_bin === \\"string\\" ? 1e3 * parseInt(x_bin.substring(0, x_bin.length - 1)) * SUFFIX_DURATION[x_bin[x_bin.length - 1]] : x_bin : void 0;\\nlet _y_aggregate;\\nlet aggregating;\\n$: {\\n    if (value) {\\n        if (value.mark === \\"point\\") {\\n            aggregating = _x_bin !== void 0;\\n            _y_aggregate = y_aggregate || aggregating ? \\"sum\\" : void 0;\\n        }\\n        else {\\n            aggregating = _x_bin !== void 0 || value.datatypes[x] === \\"nominal\\";\\n            _y_aggregate = y_aggregate ? y_aggregate : \\"sum\\";\\n        }\\n    }\\n}\\nfunction reformat_data(data) {\\n    if (tooltip == \\"all\\" || Array.isArray(tooltip)) {\\n        return data.data.map((row) => {\\n            const obj = {};\\n            data.columns.forEach((col, i) => {\\n                obj[col] = row[i];\\n            });\\n            return obj;\\n        });\\n    }\\n    let x_index = data.columns.indexOf(x);\\n    let y_index = data.columns.indexOf(y);\\n    let color_index = color ? data.columns.indexOf(color) : null;\\n    return data.data.map((row) => {\\n        const obj = {\\n            [x]: row[x_index],\\n            [y]: row[y_index]\\n        };\\n        if (color && color_index !== null) {\\n            obj[color] = row[color_index];\\n        }\\n        return obj;\\n    });\\n}\\n$: _data = value ? reformat_data(value) : [];\\nconst is_browser = typeof window !== \\"undefined\\";\\nlet chart_element;\\n$: computed_style = chart_element ? window.getComputedStyle(chart_element) : null;\\nlet view;\\nlet mounted = false;\\nlet old_width;\\nlet resizeObserver;\\nlet vegaEmbed;\\nasync function load_chart() {\\n    if (view) {\\n        view.finalize();\\n    }\\n    if (!value || !chart_element)\\n        return;\\n    old_width = chart_element.offsetWidth;\\n    const spec = create_vega_lite_spec();\\n    if (!spec)\\n        return;\\n    resizeObserver = new ResizeObserver((el) => {\\n        if (!el[0].target || !(el[0].target instanceof HTMLElement))\\n            return;\\n        if (old_width === 0 && chart_element.offsetWidth !== 0 && value.datatypes[x] === \\"nominal\\") {\\n            load_chart();\\n        }\\n        else {\\n            view.signal(\\"width\\", el[0].target.offsetWidth).run();\\n        }\\n    });\\n    if (!vegaEmbed) {\\n        vegaEmbed = (await import(\\"vega-embed\\")).default;\\n    }\\n    vegaEmbed(chart_element, spec, { actions: false }).then(function (result) {\\n        view = result.view;\\n        resizeObserver.observe(chart_element);\\n        var debounceTimeout;\\n        view.addEventListener(\\"dblclick\\", () => {\\n            gradio.dispatch(\\"double_click\\");\\n        });\\n        chart_element.addEventListener(\\"mousedown\\", function (e) {\\n            if (e.detail > 1) {\\n                e.preventDefault();\\n            }\\n        }, false);\\n        if (_selectable) {\\n            view.addSignalListener(\\"brush\\", function (_, value2) {\\n                if (Object.keys(value2).length === 0)\\n                    return;\\n                clearTimeout(debounceTimeout);\\n                let range = value2[Object.keys(value2)[0]];\\n                if (x_temporal) {\\n                    range = [range[0] / 1e3, range[1] / 1e3];\\n                }\\n                let callback = () => {\\n                    gradio.dispatch(\\"select\\", {\\n                        value: range,\\n                        index: range,\\n                        selected: true\\n                    });\\n                };\\n                if (mouse_down_on_chart) {\\n                    release_callback = callback;\\n                }\\n                else {\\n                    debounceTimeout = setTimeout(function () {\\n                        gradio.dispatch(\\"select\\", {\\n                            value: range,\\n                            index: range,\\n                            selected: true\\n                        });\\n                    }, 250);\\n                }\\n            });\\n        }\\n    });\\n}\\nlet release_callback = null;\\nonMount(() => {\\n    mounted = true;\\n    chart_element.addEventListener(\\"mousedown\\", () => {\\n        mouse_down_on_chart = true;\\n    });\\n    chart_element.addEventListener(\\"mouseup\\", () => {\\n        mouse_down_on_chart = false;\\n        if (release_callback) {\\n            release_callback();\\n            release_callback = null;\\n        }\\n    });\\n    return () => {\\n        mounted = false;\\n        if (view) {\\n            view.finalize();\\n        }\\n        if (resizeObserver) {\\n            resizeObserver.disconnect();\\n        }\\n    };\\n});\\n$: title, x_title, y_title, color_title, x, y, color, x_bin, _y_aggregate, color_map, x_lim, y_lim, caption, sort, value, mounted, chart_element, computed_style && requestAnimationFrame(load_chart);\\nfunction create_vega_lite_spec() {\\n    if (!value || !computed_style)\\n        return null;\\n    let accent_color = computed_style.getPropertyValue(\\"--color-accent\\");\\n    let body_text_color = computed_style.getPropertyValue(\\"--body-text-color\\");\\n    let borderColorPrimary = computed_style.getPropertyValue(\\"--border-color-primary\\");\\n    let font_family = computed_style.fontFamily;\\n    let title_weight = computed_style.getPropertyValue(\\"--block-title-text-weight\\");\\n    const font_to_px_val = (font) => {\\n        return font.endsWith(\\"px\\") ? parseFloat(font.slice(0, -2)) : 12;\\n    };\\n    let text_size_md = font_to_px_val(computed_style.getPropertyValue(\\"--text-md\\"));\\n    let text_size_sm = font_to_px_val(computed_style.getPropertyValue(\\"--text-sm\\"));\\n    return {\\n        $schema: \\"https://vega.github.io/schema/vega-lite/v5.17.0.json\\",\\n        background: \\"transparent\\",\\n        config: {\\n            autosize: { type: \\"fit\\", contains: \\"padding\\" },\\n            axis: {\\n                labelFont: font_family,\\n                labelColor: body_text_color,\\n                titleFont: font_family,\\n                titleColor: body_text_color,\\n                titlePadding: 8,\\n                tickColor: borderColorPrimary,\\n                labelFontSize: text_size_sm,\\n                gridColor: borderColorPrimary,\\n                titleFontWeight: \\"normal\\",\\n                titleFontSize: text_size_sm,\\n                labelFontWeight: \\"normal\\",\\n                domain: false,\\n                labelAngle: 0\\n            },\\n            legend: {\\n                labelColor: body_text_color,\\n                labelFont: font_family,\\n                titleColor: body_text_color,\\n                titleFont: font_family,\\n                titleFontWeight: \\"normal\\",\\n                titleFontSize: text_size_sm,\\n                labelFontWeight: \\"normal\\",\\n                offset: 2\\n            },\\n            title: {\\n                color: body_text_color,\\n                font: font_family,\\n                fontSize: text_size_md,\\n                fontWeight: title_weight,\\n                anchor: \\"middle\\"\\n            },\\n            view: { stroke: borderColorPrimary },\\n            mark: {\\n                stroke: value.mark !== \\"bar\\" ? accent_color : void 0,\\n                fill: value.mark === \\"bar\\" ? accent_color : void 0,\\n                cursor: \\"crosshair\\"\\n            }\\n        },\\n        data: { name: \\"data\\" },\\n        datasets: {\\n            data: _data\\n        },\\n        layer: [\\"plot\\", ...value.mark === \\"line\\" ? [\\"hover\\"] : []].map((mode) => {\\n            return {\\n                encoding: {\\n                    size: value.mark === \\"line\\" ? mode == \\"plot\\" ? {\\n                        condition: {\\n                            empty: false,\\n                            param: \\"hoverPlot\\",\\n                            value: 3\\n                        },\\n                        value: 2\\n                    } : {\\n                        condition: { empty: false, param: \\"hover\\", value: 100 },\\n                        value: 0\\n                    } : void 0,\\n                    opacity: mode === \\"plot\\" ? void 0 : {\\n                        condition: { empty: false, param: \\"hover\\", value: 1 },\\n                        value: 0\\n                    },\\n                    x: {\\n                        axis: {\\n                            ...x_label_angle !== null && { labelAngle: x_label_angle },\\n                            labels: x_axis_labels_visible,\\n                            ticks: x_axis_labels_visible\\n                        },\\n                        field: x,\\n                        title: x_title || x,\\n                        type: value.datatypes[x],\\n                        scale: _x_lim ? { domain: _x_lim } : void 0,\\n                        bin: _x_bin ? { step: _x_bin } : void 0,\\n                        sort: _sort\\n                    },\\n                    y: {\\n                        axis: y_label_angle ? { labelAngle: y_label_angle } : {},\\n                        field: y,\\n                        title: y_title || y,\\n                        type: value.datatypes[y],\\n                        scale: y_lim ? { domain: y_lim } : void 0,\\n                        aggregate: aggregating ? _y_aggregate : void 0\\n                    },\\n                    color: color ? {\\n                        field: color,\\n                        legend: { orient: \\"bottom\\", title: color_title },\\n                        scale: value.datatypes[color] === \\"nominal\\" ? {\\n                            domain: unique_colors,\\n                            range: color_map ? unique_colors.map((c) => color_map[c]) : void 0\\n                        } : {\\n                            range: [\\n                                100,\\n                                200,\\n                                300,\\n                                400,\\n                                500,\\n                                600,\\n                                700,\\n                                800,\\n                                900\\n                            ].map((n) => computed_style.getPropertyValue(\\"--primary-\\" + n)),\\n                            interpolate: \\"hsl\\"\\n                        },\\n                        type: value.datatypes[color]\\n                    } : void 0,\\n                    tooltip: tooltip == \\"none\\" ? void 0 : [\\n                        {\\n                            field: y,\\n                            type: value.datatypes[y],\\n                            aggregate: aggregating ? _y_aggregate : void 0,\\n                            title: y_title || y\\n                        },\\n                        {\\n                            field: x,\\n                            type: value.datatypes[x],\\n                            title: x_title || x,\\n                            format: x_temporal ? \\"%Y-%m-%d %H:%M:%S\\" : void 0,\\n                            bin: _x_bin ? { step: _x_bin } : void 0\\n                        },\\n                        ...color ? [\\n                            {\\n                                field: color,\\n                                type: value.datatypes[color]\\n                            }\\n                        ] : [],\\n                        ...tooltip === \\"axis\\" ? [] : value?.columns.filter((col) => col !== x && col !== y && col !== color && (tooltip === \\"all\\" || tooltip.includes(col))).map((column) => ({\\n                            field: column,\\n                            type: value.datatypes[column]\\n                        }))\\n                    ]\\n                },\\n                strokeDash: {},\\n                mark: { clip: true, type: mode === \\"hover\\" ? \\"point\\" : value.mark },\\n                name: mode\\n            };\\n        }),\\n        // @ts-ignore\\n        params: [\\n            ...value.mark === \\"line\\" ? [\\n                {\\n                    name: \\"hoverPlot\\",\\n                    select: {\\n                        clear: \\"mouseout\\",\\n                        fields: color ? [color] : [],\\n                        nearest: true,\\n                        on: \\"mouseover\\",\\n                        type: \\"point\\"\\n                    },\\n                    views: [\\"hover\\"]\\n                },\\n                {\\n                    name: \\"hover\\",\\n                    select: {\\n                        clear: \\"mouseout\\",\\n                        nearest: true,\\n                        on: \\"mouseover\\",\\n                        type: \\"point\\"\\n                    },\\n                    views: [\\"hover\\"]\\n                }\\n            ] : [],\\n            ..._selectable ? [\\n                {\\n                    name: \\"brush\\",\\n                    select: {\\n                        encodings: [\\"x\\"],\\n                        mark: { fill: \\"gray\\", fillOpacity: 0.3, stroke: \\"none\\" },\\n                        type: \\"interval\\"\\n                    },\\n                    views: [\\"plot\\"]\\n                }\\n            ] : []\\n        ],\\n        width: chart_element.offsetWidth,\\n        title: title || void 0\\n    };\\n}\\nexport let label = \\"Textbox\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let show_label;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let loading_status = void 0;\\nexport let height = void 0;\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{scale}\\n\\t{min_width}\\n\\tallow_overflow={false}\\n\\tpadding={true}\\n\\t{height}\\n>\\n\\t{#if loading_status}\\n\\t\\t<StatusTracker\\n\\t\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\t\\ti18n={gradio.i18n}\\n\\t\\t\\t{...loading_status}\\n\\t\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t\\t/>\\n\\t{/if}\\n\\t<BlockTitle {root} {show_label} info={undefined}>{label}</BlockTitle>\\n\\t{#if value && is_browser}\\n\\t\\t<div bind:this={chart_element}></div>\\n\\n\\t\\t{#if caption}\\n\\t\\t\\t<p class=\\"caption\\">{caption}</p>\\n\\t\\t{/if}\\n\\t{:else}\\n\\t\\t<Empty unpadded_box={true}><LabelIcon /></Empty>\\n\\t{/if}\\n</Block>\\n\\n<style>\\n\\tdiv {\\n\\t\\twidth: 100%;\\n\\t}\\n\\t:global(#vg-tooltip-element) {\\n\\t\\tfont-family: var(--font) !important;\\n\\t\\tfont-size: var(--text-xs) !important;\\n\\t\\tbox-shadow: none !important;\\n\\t\\tbackground-color: var(--block-background-fill) !important;\\n\\t\\tborder: 1px solid var(--border-color-primary) !important;\\n\\t\\tcolor: var(--body-text-color) !important;\\n\\t}\\n\\t:global(#vg-tooltip-element .key) {\\n\\t\\tcolor: var(--body-text-color-subdued) !important;\\n\\t}\\n\\t.caption {\\n\\t\\tpadding: 0 4px;\\n\\t\\tmargin: 0;\\n\\t\\ttext-align: center;\\n\\t}</style>\\n"],"names":[],"mappings":"AAobC,kBAAI,CACH,KAAK,CAAE,IACR,CACQ,mBAAqB,CAC5B,WAAW,CAAE,IAAI,MAAM,CAAC,CAAC,UAAU,CACnC,SAAS,CAAE,IAAI,SAAS,CAAC,CAAC,UAAU,CACpC,UAAU,CAAE,IAAI,CAAC,UAAU,CAC3B,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAAC,UAAU,CACzD,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAAC,UAAU,CACxD,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAAC,UAC/B,CACQ,wBAA0B,CACjC,KAAK,CAAE,IAAI,yBAAyB,CAAC,CAAC,UACvC,CACA,uBAAS,CACR,OAAO,CAAE,CAAC,CAAC,GAAG,CACd,MAAM,CAAE,CAAC,CACT,UAAU,CAAE,MACb"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let unique_colors;
  let _sort;
  let x_temporal;
  let _x_lim;
  let computed_style;
  let { value } = $$props;
  let { x } = $$props;
  let { y } = $$props;
  let { color = null } = $$props;
  let { root } = $$props;
  let { title = null } = $$props;
  let { x_title = null } = $$props;
  let { y_title = null } = $$props;
  let { color_title = null } = $$props;
  let { x_bin = null } = $$props;
  let { y_aggregate = void 0 } = $$props;
  let { color_map = null } = $$props;
  let { x_lim = null } = $$props;
  let { y_lim = null } = $$props;
  let { x_label_angle = null } = $$props;
  let { y_label_angle = null } = $$props;
  let { x_axis_labels_visible = true } = $$props;
  let { caption = null } = $$props;
  let { sort = null } = $$props;
  let { tooltip = "axis" } = $$props;
  function reformat_sort(_sort2) {
    if (_sort2 === "x") {
      return "ascending";
    } else if (_sort2 === "-x") {
      return "descending";
    } else if (_sort2 === "y") {
      return { field: y, order: "ascending" };
    } else if (_sort2 === "-y") {
      return { field: y, order: "descending" };
    } else if (_sort2 === null) {
      return void 0;
    } else if (Array.isArray(_sort2)) {
      return _sort2;
    }
  }
  let { _selectable = false } = $$props;
  let _data;
  let { gradio } = $$props;
  let _x_bin;
  const SUFFIX_DURATION = { s: 1, m: 60, h: 60 * 60, d: 24 * 60 * 60 };
  let _y_aggregate;
  let aggregating;
  function reformat_data(data) {
    if (tooltip == "all" || Array.isArray(tooltip)) {
      return data.data.map((row) => {
        const obj = {};
        data.columns.forEach((col, i) => {
          obj[col] = row[i];
        });
        return obj;
      });
    }
    let x_index = data.columns.indexOf(x);
    let y_index = data.columns.indexOf(y);
    let color_index = color ? data.columns.indexOf(color) : null;
    return data.data.map((row) => {
      const obj = { [x]: row[x_index], [y]: row[y_index] };
      if (color && color_index !== null) {
        obj[color] = row[color_index];
      }
      return obj;
    });
  }
  const is_browser = typeof window !== "undefined";
  let chart_element;
  let view;
  let old_width;
  let resizeObserver;
  let vegaEmbed;
  async function load_chart() {
    if (view) {
      view.finalize();
    }
    if (!value || !chart_element)
      return;
    old_width = chart_element.offsetWidth;
    const spec = create_vega_lite_spec();
    if (!spec)
      return;
    resizeObserver = new ResizeObserver((el) => {
      if (!el[0].target || !(el[0].target instanceof HTMLElement))
        return;
      if (old_width === 0 && chart_element.offsetWidth !== 0 && value.datatypes[x] === "nominal") {
        load_chart();
      } else {
        view.signal("width", el[0].target.offsetWidth).run();
      }
    });
    if (!vegaEmbed) {
      vegaEmbed = (await import('./vega-embed.module-BxvS8bt3.js')).default;
    }
    vegaEmbed(chart_element, spec, { actions: false }).then(function(result) {
      view = result.view;
      resizeObserver.observe(chart_element);
      var debounceTimeout;
      view.addEventListener("dblclick", () => {
        gradio.dispatch("double_click");
      });
      chart_element.addEventListener(
        "mousedown",
        function(e) {
          if (e.detail > 1) {
            e.preventDefault();
          }
        },
        false
      );
      if (_selectable) {
        view.addSignalListener("brush", function(_, value2) {
          if (Object.keys(value2).length === 0)
            return;
          clearTimeout(debounceTimeout);
          let range = value2[Object.keys(value2)[0]];
          if (x_temporal) {
            range = [range[0] / 1e3, range[1] / 1e3];
          }
          {
            debounceTimeout = setTimeout(
              function() {
                gradio.dispatch("select", {
                  value: range,
                  index: range,
                  selected: true
                });
              },
              250
            );
          }
        });
      }
    });
  }
  function create_vega_lite_spec() {
    if (!value || !computed_style)
      return null;
    let accent_color = computed_style.getPropertyValue("--color-accent");
    let body_text_color = computed_style.getPropertyValue("--body-text-color");
    let borderColorPrimary = computed_style.getPropertyValue("--border-color-primary");
    let font_family = computed_style.fontFamily;
    let title_weight = computed_style.getPropertyValue("--block-title-text-weight");
    const font_to_px_val = (font) => {
      return font.endsWith("px") ? parseFloat(font.slice(0, -2)) : 12;
    };
    let text_size_md = font_to_px_val(computed_style.getPropertyValue("--text-md"));
    let text_size_sm = font_to_px_val(computed_style.getPropertyValue("--text-sm"));
    return {
      $schema: "https://vega.github.io/schema/vega-lite/v5.17.0.json",
      background: "transparent",
      config: {
        autosize: { type: "fit", contains: "padding" },
        axis: {
          labelFont: font_family,
          labelColor: body_text_color,
          titleFont: font_family,
          titleColor: body_text_color,
          titlePadding: 8,
          tickColor: borderColorPrimary,
          labelFontSize: text_size_sm,
          gridColor: borderColorPrimary,
          titleFontWeight: "normal",
          titleFontSize: text_size_sm,
          labelFontWeight: "normal",
          domain: false,
          labelAngle: 0
        },
        legend: {
          labelColor: body_text_color,
          labelFont: font_family,
          titleColor: body_text_color,
          titleFont: font_family,
          titleFontWeight: "normal",
          titleFontSize: text_size_sm,
          labelFontWeight: "normal",
          offset: 2
        },
        title: {
          color: body_text_color,
          font: font_family,
          fontSize: text_size_md,
          fontWeight: title_weight,
          anchor: "middle"
        },
        view: { stroke: borderColorPrimary },
        mark: {
          stroke: value.mark !== "bar" ? accent_color : void 0,
          fill: value.mark === "bar" ? accent_color : void 0,
          cursor: "crosshair"
        }
      },
      data: { name: "data" },
      datasets: { data: _data },
      layer: ["plot", ...value.mark === "line" ? ["hover"] : []].map((mode) => {
        return {
          encoding: {
            size: value.mark === "line" ? mode == "plot" ? {
              condition: {
                empty: false,
                param: "hoverPlot",
                value: 3
              },
              value: 2
            } : {
              condition: { empty: false, param: "hover", value: 100 },
              value: 0
            } : void 0,
            opacity: mode === "plot" ? void 0 : {
              condition: { empty: false, param: "hover", value: 1 },
              value: 0
            },
            x: {
              axis: {
                ...x_label_angle !== null && { labelAngle: x_label_angle },
                labels: x_axis_labels_visible,
                ticks: x_axis_labels_visible
              },
              field: x,
              title: x_title || x,
              type: value.datatypes[x],
              scale: _x_lim ? { domain: _x_lim } : void 0,
              bin: _x_bin ? { step: _x_bin } : void 0,
              sort: _sort
            },
            y: {
              axis: y_label_angle ? { labelAngle: y_label_angle } : {},
              field: y,
              title: y_title || y,
              type: value.datatypes[y],
              scale: y_lim ? { domain: y_lim } : void 0,
              aggregate: aggregating ? _y_aggregate : void 0
            },
            color: color ? {
              field: color,
              legend: { orient: "bottom", title: color_title },
              scale: value.datatypes[color] === "nominal" ? {
                domain: unique_colors,
                range: color_map ? unique_colors.map((c) => color_map[c]) : void 0
              } : {
                range: [100, 200, 300, 400, 500, 600, 700, 800, 900].map((n) => computed_style.getPropertyValue("--primary-" + n)),
                interpolate: "hsl"
              },
              type: value.datatypes[color]
            } : void 0,
            tooltip: tooltip == "none" ? void 0 : [
              {
                field: y,
                type: value.datatypes[y],
                aggregate: aggregating ? _y_aggregate : void 0,
                title: y_title || y
              },
              {
                field: x,
                type: value.datatypes[x],
                title: x_title || x,
                format: x_temporal ? "%Y-%m-%d %H:%M:%S" : void 0,
                bin: _x_bin ? { step: _x_bin } : void 0
              },
              ...color ? [
                {
                  field: color,
                  type: value.datatypes[color]
                }
              ] : [],
              ...tooltip === "axis" ? [] : value?.columns.filter((col) => col !== x && col !== y && col !== color && (tooltip === "all" || tooltip.includes(col))).map((column) => ({
                field: column,
                type: value.datatypes[column]
              }))
            ]
          },
          strokeDash: {},
          mark: {
            clip: true,
            type: mode === "hover" ? "point" : value.mark
          },
          name: mode
        };
      }),
      // @ts-ignore
      params: [
        ...value.mark === "line" ? [
          {
            name: "hoverPlot",
            select: {
              clear: "mouseout",
              fields: color ? [color] : [],
              nearest: true,
              on: "mouseover",
              type: "point"
            },
            views: ["hover"]
          },
          {
            name: "hover",
            select: {
              clear: "mouseout",
              nearest: true,
              on: "mouseover",
              type: "point"
            },
            views: ["hover"]
          }
        ] : [],
        ..._selectable ? [
          {
            name: "brush",
            select: {
              encodings: ["x"],
              mark: {
                fill: "gray",
                fillOpacity: 0.3,
                stroke: "none"
              },
              type: "interval"
            },
            views: ["plot"]
          }
        ] : []
      ],
      width: chart_element.offsetWidth,
      title: title || void 0
    };
  }
  let { label = "Textbox" } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { show_label } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status = void 0 } = $$props;
  let { height = void 0 } = $$props;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.x === void 0 && $$bindings.x && x !== void 0)
    $$bindings.x(x);
  if ($$props.y === void 0 && $$bindings.y && y !== void 0)
    $$bindings.y(y);
  if ($$props.color === void 0 && $$bindings.color && color !== void 0)
    $$bindings.color(color);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.title === void 0 && $$bindings.title && title !== void 0)
    $$bindings.title(title);
  if ($$props.x_title === void 0 && $$bindings.x_title && x_title !== void 0)
    $$bindings.x_title(x_title);
  if ($$props.y_title === void 0 && $$bindings.y_title && y_title !== void 0)
    $$bindings.y_title(y_title);
  if ($$props.color_title === void 0 && $$bindings.color_title && color_title !== void 0)
    $$bindings.color_title(color_title);
  if ($$props.x_bin === void 0 && $$bindings.x_bin && x_bin !== void 0)
    $$bindings.x_bin(x_bin);
  if ($$props.y_aggregate === void 0 && $$bindings.y_aggregate && y_aggregate !== void 0)
    $$bindings.y_aggregate(y_aggregate);
  if ($$props.color_map === void 0 && $$bindings.color_map && color_map !== void 0)
    $$bindings.color_map(color_map);
  if ($$props.x_lim === void 0 && $$bindings.x_lim && x_lim !== void 0)
    $$bindings.x_lim(x_lim);
  if ($$props.y_lim === void 0 && $$bindings.y_lim && y_lim !== void 0)
    $$bindings.y_lim(y_lim);
  if ($$props.x_label_angle === void 0 && $$bindings.x_label_angle && x_label_angle !== void 0)
    $$bindings.x_label_angle(x_label_angle);
  if ($$props.y_label_angle === void 0 && $$bindings.y_label_angle && y_label_angle !== void 0)
    $$bindings.y_label_angle(y_label_angle);
  if ($$props.x_axis_labels_visible === void 0 && $$bindings.x_axis_labels_visible && x_axis_labels_visible !== void 0)
    $$bindings.x_axis_labels_visible(x_axis_labels_visible);
  if ($$props.caption === void 0 && $$bindings.caption && caption !== void 0)
    $$bindings.caption(caption);
  if ($$props.sort === void 0 && $$bindings.sort && sort !== void 0)
    $$bindings.sort(sort);
  if ($$props.tooltip === void 0 && $$bindings.tooltip && tooltip !== void 0)
    $$bindings.tooltip(tooltip);
  if ($$props._selectable === void 0 && $$bindings._selectable && _selectable !== void 0)
    $$bindings._selectable(_selectable);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  $$result.css.add(css);
  _data = value ? reformat_data(value) : [];
  unique_colors = color && value && value.datatypes[color] === "nominal" ? Array.from(new Set(_data.map((d) => d[color]))) : [];
  _sort = reformat_sort(sort);
  x_temporal = value && value.datatypes[x] === "temporal";
  _x_lim = x_lim && x_temporal ? [x_lim[0] * 1e3, x_lim[1] * 1e3] : x_lim;
  _x_bin = x_bin ? typeof x_bin === "string" ? 1e3 * parseInt(x_bin.substring(0, x_bin.length - 1)) * SUFFIX_DURATION[x_bin[x_bin.length - 1]] : x_bin : void 0;
  {
    {
      if (value) {
        if (value.mark === "point") {
          aggregating = _x_bin !== void 0;
          _y_aggregate = y_aggregate || aggregating ? "sum" : void 0;
        } else {
          aggregating = _x_bin !== void 0 || value.datatypes[x] === "nominal";
          _y_aggregate = y_aggregate ? y_aggregate : "sum";
        }
      }
    }
  }
  computed_style = null;
  {
    computed_style && requestAnimationFrame(load_chart);
  }
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      scale,
      min_width,
      allow_overflow: false,
      padding: true,
      height
    },
    {},
    {
      default: () => {
        return `${loading_status ? `${validate_component(Static, "StatusTracker").$$render($$result, Object_1.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})}` : ``} ${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info: void 0 }, {}, {
          default: () => {
            return `${escape(label)}`;
          }
        })} ${value && is_browser ? `<div class="svelte-10k9m4v"${add_attribute("this", chart_element, 0)}></div> ${caption ? `<p class="caption svelte-10k9m4v">${escape(caption)}</p>` : ``}` : `${validate_component(Empty, "Empty").$$render($$result, { unpadded_box: true }, {}, {
          default: () => {
            return `${validate_component(LineChart, "LabelIcon").$$render($$result, {}, {}, {})}`;
          }
        })}`}`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index23-C9OZRMEE.js.map
