import { B as Block, S as Static, e as BlockLabel, a4 as TextHighlight, g as Empty, a5 as globals, a6 as colors } from './2-B6LMYTAg.js';
import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, f as each, b as add_attribute, e as escape, d as add_styles, h as merge_ssr_styles } from './ssr-RaXq3SJh.js';
import { g as get_next_color } from './color-8KH1gdSw.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

function name_to_rgba(name, a, ctx) {
  if (!ctx) {
    var canvas = document.createElement("canvas");
    ctx = canvas.getContext("2d");
  }
  ctx.fillStyle = name;
  ctx.fillRect(0, 0, 1, 1);
  const [r, g, b] = ctx.getImageData(0, 0, 1, 1).data;
  ctx.clearRect(0, 0, 1, 1);
  return `rgba(${r}, ${g}, ${b}, ${255 / a})`;
}
function correct_color_map(color_map, _color_map, browser, ctx) {
  for (const col in color_map) {
    const _c = color_map[col].trim();
    if (_c in colors) {
      _color_map[col] = colors[_c];
    } else {
      _color_map[col] = {
        primary: browser ? name_to_rgba(color_map[col], 1, ctx) : color_map[col],
        secondary: browser ? name_to_rgba(color_map[col], 0.5, ctx) : color_map[col]
      };
    }
  }
}
function merge_elements(value, mergeMode) {
  let result = [];
  let tempStr = null;
  let tempVal = null;
  for (const val of value) {
    if (tempVal === val.class_or_confidence) {
      tempStr = tempStr ? tempStr + val.token : val.token;
    } else {
      if (tempStr !== null) {
        result.push({
          token: tempStr,
          class_or_confidence: tempVal
        });
      }
      tempStr = val.token;
      tempVal = val.class_or_confidence;
    }
  }
  if (tempStr !== null) {
    result.push({
      token: tempStr,
      class_or_confidence: tempVal
    });
  }
  return result;
}
const css$2 = {
  code: ".container.svelte-ju12zg.svelte-ju12zg{display:flex;flex-direction:column;gap:var(--spacing-sm);padding:var(--block-padding)}.hl.svelte-ju12zg+.hl.svelte-ju12zg{margin-left:var(--size-1)}.textspan.svelte-ju12zg:last-child>.label.svelte-ju12zg{margin-right:0}.category-legend.svelte-ju12zg.svelte-ju12zg{display:flex;flex-wrap:wrap;gap:var(--spacing-sm);color:black}.category-label.svelte-ju12zg.svelte-ju12zg{cursor:pointer;border-radius:var(--radius-xs);padding-right:var(--size-2);padding-left:var(--size-2);font-weight:var(--weight-semibold)}.color-legend.svelte-ju12zg.svelte-ju12zg{display:flex;justify-content:space-between;border-radius:var(--radius-xs);background:linear-gradient(\n			to right,\n			var(--color-purple),\n			rgba(255, 255, 255, 0),\n			var(--color-red)\n		);padding:var(--size-1) var(--size-2);font-weight:var(--weight-semibold)}.textfield.svelte-ju12zg.svelte-ju12zg{box-sizing:border-box;border-radius:var(--radius-xs);background:var(--background-fill-primary);background-color:transparent;max-width:var(--size-full);line-height:var(--scale-4);word-break:break-all}.textspan.svelte-ju12zg.svelte-ju12zg{transition:150ms;border-radius:var(--radius-xs);padding-top:2.5px;padding-right:var(--size-1);padding-bottom:3.5px;padding-left:var(--size-1);color:black}.label.svelte-ju12zg.svelte-ju12zg{transition:150ms;margin-top:1px;border-radius:var(--radius-xs);padding:1px 5px;color:var(--body-text-color);color:white;font-weight:var(--weight-bold);font-size:var(--text-sm);text-transform:uppercase}.text.svelte-ju12zg.svelte-ju12zg{color:black;white-space:pre-wrap}.score-text.svelte-ju12zg .text.svelte-ju12zg{color:var(--body-text-color)}.score-text.svelte-ju12zg.svelte-ju12zg{margin-right:var(--size-1);padding:var(--size-1)}.no-cat.svelte-ju12zg.svelte-ju12zg{color:var(--body-text-color)}.no-label.svelte-ju12zg.svelte-ju12zg{color:var(--body-text-color)}.selectable.svelte-ju12zg.svelte-ju12zg{cursor:pointer}",
  map: '{"version":3,"file":"StaticHighlightedtext.svelte","sources":["StaticHighlightedtext.svelte"],"sourcesContent":["<script lang=\\"ts\\">const browser = typeof document !== \\"undefined\\";\\nimport { get_next_color } from \\"@gradio/utils\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nimport { correct_color_map } from \\"./utils\\";\\nexport let value = [];\\nexport let show_legend = false;\\nexport let show_inline_category = true;\\nexport let color_map = {};\\nexport let selectable = false;\\nlet ctx;\\nlet _color_map = {};\\nlet active = \\"\\";\\nfunction splitTextByNewline(text) {\\n    return text.split(\\"\\\\n\\");\\n}\\nconst dispatch = createEventDispatcher();\\nlet mode;\\n$: {\\n    if (!color_map) {\\n        color_map = {};\\n    }\\n    if (value.length > 0) {\\n        for (let entry of value) {\\n            if (entry.class_or_confidence !== null) {\\n                if (typeof entry.class_or_confidence === \\"string\\") {\\n                    mode = \\"categories\\";\\n                    if (!(entry.class_or_confidence in color_map)) {\\n                        let color = get_next_color(Object.keys(color_map).length);\\n                        color_map[entry.class_or_confidence] = color;\\n                    }\\n                }\\n                else {\\n                    mode = \\"scores\\";\\n                }\\n            }\\n        }\\n    }\\n    correct_color_map(color_map, _color_map, browser, ctx);\\n}\\nfunction handle_mouseover(label) {\\n    active = label;\\n}\\nfunction handle_mouseout() {\\n    active = \\"\\";\\n}\\n<\/script>\\n\\n<!-- \\n\\t@todo victor: try reimplementing without flex (negative margins on container to avoid left margin on linebreak). \\n\\tIf not possible hijack the copy execution like this:\\n\\n<svelte:window\\n\\ton:copy|preventDefault={() => {\\n\\t\\tconst selection =.getSelection()?.toString();\\n\\t\\tconsole.log(selection?.replaceAll(\\"\\\\n\\", \\" \\"));\\n\\t}}\\n/>\\n-->\\n\\n<div class=\\"container\\">\\n\\t{#if mode === \\"categories\\"}\\n\\t\\t{#if show_legend}\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"category-legend\\"\\n\\t\\t\\t\\tdata-testid=\\"highlighted-text:category-legend\\"\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#each Object.entries(_color_map) as [category, color], i}\\n\\t\\t\\t\\t\\t<!-- TODO: fix -->\\n\\t\\t\\t\\t\\t<!-- svelte-ignore a11y-no-static-element-interactions -->\\n\\t\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\t\\ton:mouseover={() => handle_mouseover(category)}\\n\\t\\t\\t\\t\\t\\ton:focus={() => handle_mouseover(category)}\\n\\t\\t\\t\\t\\t\\ton:mouseout={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\ton:blur={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\tclass=\\"category-label\\"\\n\\t\\t\\t\\t\\t\\tstyle={\\"background-color:\\" + color.secondary}\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t{category}\\n\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\t\\t<div class=\\"textfield\\">\\n\\t\\t\\t{#each value as v, i}\\n\\t\\t\\t\\t{#each splitTextByNewline(v.token) as line, j}\\n\\t\\t\\t\\t\\t{#if line.trim() !== \\"\\"}\\n\\t\\t\\t\\t\\t\\t<!-- TODO: fix -->\\n\\t\\t\\t\\t\\t\\t<!-- svelte-ignore a11y-no-static-element-interactions -->\\n\\t\\t\\t\\t\\t\\t<!-- svelte-ignore a11y-click-events-have-key-events-->\\n\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"textspan\\"\\n\\t\\t\\t\\t\\t\\t\\tstyle:background-color={v.class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t(active && active !== v.class_or_confidence)\\n\\t\\t\\t\\t\\t\\t\\t\\t? \\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t: _color_map[v.class_or_confidence].secondary}\\n\\t\\t\\t\\t\\t\\t\\tclass:no-cat={v.class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t(active && active !== v.class_or_confidence)}\\n\\t\\t\\t\\t\\t\\t\\tclass:hl={v.class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\t\\tclass:selectable\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\t\\tdispatch(\\"select\\", {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tindex: i,\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tvalue: [v.token, v.class_or_confidence]\\n\\t\\t\\t\\t\\t\\t\\t\\t});\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\tclass:no-label={v.class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t!_color_map[v.class_or_confidence]}\\n\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"text\\">{line}</span\\n\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{#if !show_legend && show_inline_category && v.class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\t\\t\\t&nbsp;\\n\\t\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"label\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tstyle:background-color={v.class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t(active && active !== v.class_or_confidence)\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t? \\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t: _color_map[v.class_or_confidence].primary}\\n\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{v.class_or_confidence}\\n\\t\\t\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t{#if j < splitTextByNewline(v.token).length - 1}\\n\\t\\t\\t\\t\\t\\t<br />\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t{#if show_legend}\\n\\t\\t\\t<div class=\\"color-legend\\" data-testid=\\"highlighted-text:color-legend\\">\\n\\t\\t\\t\\t<span>-1</span>\\n\\t\\t\\t\\t<span>0</span>\\n\\t\\t\\t\\t<span>+1</span>\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\t\\t<div class=\\"textfield\\" data-testid=\\"highlighted-text:textfield\\">\\n\\t\\t\\t{#each value as v}\\n\\t\\t\\t\\t{@const score =\\n\\t\\t\\t\\t\\ttypeof v.class_or_confidence === \\"string\\"\\n\\t\\t\\t\\t\\t\\t? parseInt(v.class_or_confidence)\\n\\t\\t\\t\\t\\t\\t: v.class_or_confidence}\\n\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\tclass=\\"textspan score-text\\"\\n\\t\\t\\t\\t\\tstyle={\\"background-color: rgba(\\" +\\n\\t\\t\\t\\t\\t\\t(score && score < 0\\n\\t\\t\\t\\t\\t\\t\\t? \\"128, 90, 213,\\" + -score\\n\\t\\t\\t\\t\\t\\t\\t: \\"239, 68, 60,\\" + score) +\\n\\t\\t\\t\\t\\t\\t\\")\\"}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<span class=\\"text\\">{v.token}</span>\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.container {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tpadding: var(--block-padding);\\n\\t}\\n\\t.hl + .hl {\\n\\t\\tmargin-left: var(--size-1);\\n\\t}\\n\\n\\t.textspan:last-child > .label {\\n\\t\\tmargin-right: 0;\\n\\t}\\n\\n\\t.category-legend {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tcolor: black;\\n\\t}\\n\\n\\t.category-label {\\n\\t\\tcursor: pointer;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding-right: var(--size-2);\\n\\t\\tpadding-left: var(--size-2);\\n\\t\\tfont-weight: var(--weight-semibold);\\n\\t}\\n\\n\\t.color-legend {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tbackground: linear-gradient(\\n\\t\\t\\tto right,\\n\\t\\t\\tvar(--color-purple),\\n\\t\\t\\trgba(255, 255, 255, 0),\\n\\t\\t\\tvar(--color-red)\\n\\t\\t);\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\tfont-weight: var(--weight-semibold);\\n\\t}\\n\\n\\t.textfield {\\n\\t\\tbox-sizing: border-box;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tbackground: var(--background-fill-primary);\\n\\t\\tbackground-color: transparent;\\n\\t\\tmax-width: var(--size-full);\\n\\t\\tline-height: var(--scale-4);\\n\\t\\tword-break: break-all;\\n\\t}\\n\\n\\t.textspan {\\n\\t\\ttransition: 150ms;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding-top: 2.5px;\\n\\t\\tpadding-right: var(--size-1);\\n\\t\\tpadding-bottom: 3.5px;\\n\\t\\tpadding-left: var(--size-1);\\n\\t\\tcolor: black;\\n\\t}\\n\\n\\t.label {\\n\\t\\ttransition: 150ms;\\n\\t\\tmargin-top: 1px;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding: 1px 5px;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tcolor: white;\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\ttext-transform: uppercase;\\n\\t}\\n\\n\\t.text {\\n\\t\\tcolor: black;\\n\\t\\twhite-space: pre-wrap;\\n\\t}\\n\\n\\t.score-text .text {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.score-text {\\n\\t\\tmargin-right: var(--size-1);\\n\\t\\tpadding: var(--size-1);\\n\\t}\\n\\n\\t.no-cat {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.no-label {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"AAiKC,sCAAW,CACV,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,OAAO,CAAE,IAAI,eAAe,CAC7B,CACA,iBAAG,CAAG,iBAAI,CACT,WAAW,CAAE,IAAI,QAAQ,CAC1B,CAEA,uBAAS,WAAW,CAAG,oBAAO,CAC7B,YAAY,CAAE,CACf,CAEA,4CAAiB,CAChB,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,IAAI,CACf,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,KAAK,CAAE,KACR,CAEA,2CAAgB,CACf,MAAM,CAAE,OAAO,CACf,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,WAAW,CAAE,IAAI,iBAAiB,CACnC,CAEA,yCAAc,CACb,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE;AACd,GAAG,EAAE,CAAC,KAAK,CAAC;AACZ,GAAG,IAAI,cAAc,CAAC,CAAC;AACvB,GAAG,KAAK,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,CAAC;AAC1B,GAAG,IAAI,WAAW,CAAC;AACnB,GAAG,CACD,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,WAAW,CAAE,IAAI,iBAAiB,CACnC,CAEA,sCAAW,CACV,UAAU,CAAE,UAAU,CACtB,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE,IAAI,yBAAyB,CAAC,CAC1C,gBAAgB,CAAE,WAAW,CAC7B,SAAS,CAAE,IAAI,WAAW,CAAC,CAC3B,WAAW,CAAE,IAAI,SAAS,CAAC,CAC3B,UAAU,CAAE,SACb,CAEA,qCAAU,CACT,UAAU,CAAE,KAAK,CACjB,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,WAAW,CAAE,KAAK,CAClB,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,cAAc,CAAE,KAAK,CACrB,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,KAAK,CAAE,KACR,CAEA,kCAAO,CACN,UAAU,CAAE,KAAK,CACjB,UAAU,CAAE,GAAG,CACf,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,OAAO,CAAE,GAAG,CAAC,GAAG,CAChB,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,IAAI,aAAa,CAAC,CAC/B,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,cAAc,CAAE,SACjB,CAEA,iCAAM,CACL,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,QACd,CAEA,yBAAW,CAAC,mBAAM,CACjB,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,uCAAY,CACX,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,OAAO,CAAE,IAAI,QAAQ,CACtB,CAEA,mCAAQ,CACP,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,qCAAU,CACT,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,uCAAY,CACX,MAAM,CAAE,OACT"}'
};
function splitTextByNewline$1(text) {
  return text.split("\n");
}
const StaticHighlightedtext = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  const browser = typeof document !== "undefined";
  let { value = [] } = $$props;
  let { show_legend = false } = $$props;
  let { show_inline_category = true } = $$props;
  let { color_map = {} } = $$props;
  let { selectable = false } = $$props;
  let ctx;
  let _color_map = {};
  let active = "";
  createEventDispatcher();
  let mode;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_legend === void 0 && $$bindings.show_legend && show_legend !== void 0)
    $$bindings.show_legend(show_legend);
  if ($$props.show_inline_category === void 0 && $$bindings.show_inline_category && show_inline_category !== void 0)
    $$bindings.show_inline_category(show_inline_category);
  if ($$props.color_map === void 0 && $$bindings.color_map && color_map !== void 0)
    $$bindings.color_map(color_map);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  $$result.css.add(css$2);
  {
    {
      if (!color_map) {
        color_map = {};
      }
      if (value.length > 0) {
        for (let entry of value) {
          if (entry.class_or_confidence !== null) {
            if (typeof entry.class_or_confidence === "string") {
              mode = "categories";
              if (!(entry.class_or_confidence in color_map)) {
                let color = get_next_color(Object.keys(color_map).length);
                color_map[entry.class_or_confidence] = color;
              }
            } else {
              mode = "scores";
            }
          }
        }
      }
      correct_color_map(color_map, _color_map, browser, ctx);
    }
  }
  return ` <div class="container svelte-ju12zg">${mode === "categories" ? `${show_legend ? `<div class="category-legend svelte-ju12zg" data-testid="highlighted-text:category-legend">${each(Object.entries(_color_map), ([category, color], i) => {
    return `  <div class="category-label svelte-ju12zg"${add_attribute("style", "background-color:" + color.secondary, 0)}>${escape(category)} </div>`;
  })}</div>` : ``} <div class="textfield svelte-ju12zg">${each(value, (v, i) => {
    return `${each(splitTextByNewline$1(v.token), (line, j) => {
      return `${line.trim() !== "" ? `   <span class="${[
        "textspan svelte-ju12zg",
        (v.class_or_confidence === null || active ? "no-cat" : "") + " " + (v.class_or_confidence !== null ? "hl" : "") + " " + (selectable ? "selectable" : "")
      ].join(" ").trim()}"${add_styles({
        "background-color": v.class_or_confidence === null || active ? "" : _color_map[v.class_or_confidence].secondary
      })}><span class="${[
        "text svelte-ju12zg",
        v.class_or_confidence === null || !_color_map[v.class_or_confidence] ? "no-label" : ""
      ].join(" ").trim()}">${escape(line)}</span> ${!show_legend && show_inline_category && v.class_or_confidence !== null ? ` 
								<span class="label svelte-ju12zg"${add_styles({
        "background-color": v.class_or_confidence === null || active ? "" : _color_map[v.class_or_confidence].primary
      })}>${escape(v.class_or_confidence)} </span>` : ``} </span>` : ``} ${j < splitTextByNewline$1(v.token).length - 1 ? `<br>` : ``}`;
    })}`;
  })}</div>` : `${show_legend ? `<div class="color-legend svelte-ju12zg" data-testid="highlighted-text:color-legend" data-svelte-h="svelte-mv3vmx"><span>-1</span> <span>0</span> <span>+1</span></div>` : ``} <div class="textfield svelte-ju12zg" data-testid="highlighted-text:textfield">${each(value, (v) => {
    let score = typeof v.class_or_confidence === "string" ? parseInt(v.class_or_confidence) : v.class_or_confidence;
    return ` <span class="textspan score-text svelte-ju12zg"${add_attribute(
      "style",
      "background-color: rgba(" + (score && score < 0 ? "128, 90, 213," + -score : "239, 68, 60," + score) + ")",
      0
    )}><span class="text svelte-ju12zg">${escape(v.token)}</span> </span>`;
  })}</div>`} </div>`;
});
const StaticHighlightedText = StaticHighlightedtext;
const css$1 = {
  code: ".label-input.svelte-1cag2po{transition:150ms;margin-top:1px;margin-right:calc(var(--size-1));border-radius:var(--radius-xs);padding:1px 5px;color:black;font-weight:var(--weight-bold);font-size:var(--text-sm);text-transform:uppercase;line-height:1;color:white}.label-input.svelte-1cag2po::placeholder{color:rgba(1, 1, 1, 0.5)}",
  map: '{"version":3,"file":"LabelInput.svelte","sources":["LabelInput.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let value;\\nexport let category;\\nexport let active;\\nexport let labelToEdit;\\nexport let indexOfLabel;\\nexport let text;\\nexport let handleValueChange;\\nexport let isScoresMode = false;\\nexport let _color_map;\\nlet _input_value = category;\\nfunction handleInput(e) {\\n    let target = e.target;\\n    if (target) {\\n        _input_value = target.value;\\n    }\\n}\\nfunction updateLabelValue(e, elementIndex, text2) {\\n    let target = e.target;\\n    value = [\\n        ...value.slice(0, elementIndex),\\n        {\\n            token: text2,\\n            class_or_confidence: target.value === \\"\\" ? null : isScoresMode ? Number(target.value) : target.value\\n        },\\n        ...value.slice(elementIndex + 1)\\n    ];\\n    handleValueChange();\\n}\\nfunction clearPlaceHolderOnFocus(e) {\\n    let target = e.target;\\n    if (target && target.placeholder)\\n        target.placeholder = \\"\\";\\n}\\n<\/script>\\n\\n<!-- svelte-ignore a11y-autofocus -->\\n<!-- autofocus should not be disorienting for a screen reader users\\nas input is only rendered once a new label is created -->\\n{#if !isScoresMode}\\n\\t<input\\n\\t\\tclass=\\"label-input\\"\\n\\t\\tautofocus\\n\\t\\tid={`label-input-${indexOfLabel}`}\\n\\t\\ttype=\\"text\\"\\n\\t\\tplaceholder=\\"label\\"\\n\\t\\tvalue={category}\\n\\t\\tstyle:background-color={category === null || (active && active !== category)\\n\\t\\t\\t? \\"\\"\\n\\t\\t\\t: _color_map[category].primary}\\n\\t\\tstyle:width={_input_value\\n\\t\\t\\t? _input_value.toString()?.length + 4 + \\"ch\\"\\n\\t\\t\\t: \\"8ch\\"}\\n\\t\\ton:input={handleInput}\\n\\t\\ton:blur={(e) => updateLabelValue(e, indexOfLabel, text)}\\n\\t\\ton:keydown={(e) => {\\n\\t\\t\\tif (e.key === \\"Enter\\") {\\n\\t\\t\\t\\tupdateLabelValue(e, indexOfLabel, text);\\n\\t\\t\\t\\tlabelToEdit = -1;\\n\\t\\t\\t}\\n\\t\\t}}\\n\\t\\ton:focus={clearPlaceHolderOnFocus}\\n\\t/>\\n{:else}\\n\\t<input\\n\\t\\tclass=\\"label-input\\"\\n\\t\\tautofocus\\n\\t\\ttype=\\"number\\"\\n\\t\\tstep=\\"0.1\\"\\n\\t\\tstyle={\\"background-color: rgba(\\" +\\n\\t\\t\\t(typeof category === \\"number\\" && category < 0\\n\\t\\t\\t\\t? \\"128, 90, 213,\\" + -category\\n\\t\\t\\t\\t: \\"239, 68, 60,\\" + category) +\\n\\t\\t\\t\\")\\"}\\n\\t\\tvalue={category}\\n\\t\\tstyle:width=\\"7ch\\"\\n\\t\\ton:input={handleInput}\\n\\t\\ton:blur={(e) => updateLabelValue(e, indexOfLabel, text)}\\n\\t\\ton:keydown={(e) => {\\n\\t\\t\\tif (e.key === \\"Enter\\") {\\n\\t\\t\\t\\tupdateLabelValue(e, indexOfLabel, text);\\n\\t\\t\\t\\tlabelToEdit = -1;\\n\\t\\t\\t}\\n\\t\\t}}\\n\\t/>\\n{/if}\\n\\n<style>\\n\\t.label-input {\\n\\t\\ttransition: 150ms;\\n\\t\\tmargin-top: 1px;\\n\\t\\tmargin-right: calc(var(--size-1));\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding: 1px 5px;\\n\\t\\tcolor: black;\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\ttext-transform: uppercase;\\n\\t\\tline-height: 1;\\n\\t\\tcolor: white;\\n\\t}\\n\\n\\t.label-input::placeholder {\\n\\t\\tcolor: rgba(1, 1, 1, 0.5);\\n\\t}</style>\\n"],"names":[],"mappings":"AAuFC,2BAAa,CACZ,UAAU,CAAE,KAAK,CACjB,UAAU,CAAE,GAAG,CACf,YAAY,CAAE,KAAK,IAAI,QAAQ,CAAC,CAAC,CACjC,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,OAAO,CAAE,GAAG,CAAC,GAAG,CAChB,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,IAAI,aAAa,CAAC,CAC/B,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,cAAc,CAAE,SAAS,CACzB,WAAW,CAAE,CAAC,CACd,KAAK,CAAE,KACR,CAEA,2BAAY,aAAc,CACzB,KAAK,CAAE,KAAK,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,GAAG,CACzB"}'
};
const LabelInput = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { category } = $$props;
  let { active } = $$props;
  let { labelToEdit } = $$props;
  let { indexOfLabel } = $$props;
  let { text } = $$props;
  let { handleValueChange } = $$props;
  let { isScoresMode = false } = $$props;
  let { _color_map } = $$props;
  let _input_value = category;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.category === void 0 && $$bindings.category && category !== void 0)
    $$bindings.category(category);
  if ($$props.active === void 0 && $$bindings.active && active !== void 0)
    $$bindings.active(active);
  if ($$props.labelToEdit === void 0 && $$bindings.labelToEdit && labelToEdit !== void 0)
    $$bindings.labelToEdit(labelToEdit);
  if ($$props.indexOfLabel === void 0 && $$bindings.indexOfLabel && indexOfLabel !== void 0)
    $$bindings.indexOfLabel(indexOfLabel);
  if ($$props.text === void 0 && $$bindings.text && text !== void 0)
    $$bindings.text(text);
  if ($$props.handleValueChange === void 0 && $$bindings.handleValueChange && handleValueChange !== void 0)
    $$bindings.handleValueChange(handleValueChange);
  if ($$props.isScoresMode === void 0 && $$bindings.isScoresMode && isScoresMode !== void 0)
    $$bindings.isScoresMode(isScoresMode);
  if ($$props._color_map === void 0 && $$bindings._color_map && _color_map !== void 0)
    $$bindings._color_map(_color_map);
  $$result.css.add(css$1);
  return `  ${!isScoresMode ? `<input class="label-input svelte-1cag2po" autofocus${add_attribute("id", `label-input-${indexOfLabel}`, 0)} type="text" placeholder="label"${add_attribute("value", category, 0)}${add_styles({
    "background-color": category === null || active && active !== category ? "" : _color_map[category].primary,
    "width": _input_value ? _input_value.toString()?.length + 4 + "ch" : "8ch"
  })}>` : `<input class="label-input svelte-1cag2po" autofocus type="number" step="0.1"${add_styles(merge_ssr_styles(
    escape(
      "background-color: rgba(" + (typeof category === "number" && category < 0 ? "128, 90, 213," + -category : "239, 68, 60," + category) + ")",
      true
    ),
    { "width": `7ch` }
  ))}${add_attribute("value", category, 0)}>`}`;
});
const css = {
  code: ".label-clear-button.svelte-1ozsnjl.svelte-1ozsnjl{display:none;border-radius:var(--radius-xs);padding-top:2.5px;padding-right:var(--size-1);padding-bottom:3.5px;padding-left:var(--size-1);color:black;background-color:var(--background-fill-secondary);user-select:none;position:relative;left:-3px;border-radius:0 var(--radius-xs) var(--radius-xs) 0;color:var(--block-label-text-color)}.text-class_or_confidence-container.svelte-1ozsnjl:hover .label-clear-button.svelte-1ozsnjl,.text-class_or_confidence-container.svelte-1ozsnjl:focus-within .label-clear-button.svelte-1ozsnjl,.score-text-container.svelte-1ozsnjl:hover .label-clear-button.svelte-1ozsnjl,.score-text-container.svelte-1ozsnjl:focus-within .label-clear-button.svelte-1ozsnjl{display:inline}.text-class_or_confidence-container.svelte-1ozsnjl:hover .textspan.hl.svelte-1ozsnjl,.text-class_or_confidence-container.svelte-1ozsnjl:focus-within .textspan.hl.svelte-1ozsnjl,.score-text.svelte-1ozsnjl.svelte-1ozsnjl:hover{border-radius:var(--radius-xs) 0 0 var(--radius-xs)}.container.svelte-1ozsnjl.svelte-1ozsnjl{display:flex;flex-direction:column;gap:var(--spacing-sm);padding:var(--block-padding)}.hl.svelte-1ozsnjl.svelte-1ozsnjl{margin-left:var(--size-1);transition:background-color 0.3s;user-select:none}.textspan.svelte-1ozsnjl:last-child>.label.svelte-1ozsnjl{margin-right:0}.class_or_confidence-legend.svelte-1ozsnjl.svelte-1ozsnjl{display:flex;flex-wrap:wrap;gap:var(--spacing-sm);color:black}.class_or_confidence-label.svelte-1ozsnjl.svelte-1ozsnjl{cursor:pointer;border-radius:var(--radius-xs);padding-right:var(--size-2);padding-left:var(--size-2);font-weight:var(--weight-semibold)}.color-legend.svelte-1ozsnjl.svelte-1ozsnjl{display:flex;justify-content:space-between;border-radius:var(--radius-xs);background:linear-gradient(\n			to right,\n			var(--color-purple),\n			rgba(255, 255, 255, 0),\n			var(--color-red)\n		);padding:var(--size-1) var(--size-2);font-weight:var(--weight-semibold)}.textfield.svelte-1ozsnjl.svelte-1ozsnjl{box-sizing:border-box;border-radius:var(--radius-xs);background:var(--background-fill-primary);background-color:transparent;max-width:var(--size-full);line-height:var(--scale-4);word-break:break-all}.textspan.svelte-1ozsnjl.svelte-1ozsnjl{transition:150ms;border-radius:var(--radius-xs);padding-top:2.5px;padding-right:var(--size-1);padding-bottom:3.5px;padding-left:var(--size-1);color:black;cursor:text}.label.svelte-1ozsnjl.svelte-1ozsnjl{transition:150ms;margin-top:1px;border-radius:var(--radius-xs);padding:1px 5px;color:var(--body-text-color);color:white;font-weight:var(--weight-bold);font-size:var(--text-sm);text-transform:uppercase;user-select:none}.text.svelte-1ozsnjl.svelte-1ozsnjl{color:black;white-space:pre-wrap}.textspan.hl.svelte-1ozsnjl.svelte-1ozsnjl{user-select:none}.score-text-container.svelte-1ozsnjl.svelte-1ozsnjl{margin-right:var(--size-1)}.score-text.svelte-1ozsnjl .text.svelte-1ozsnjl{color:var(--body-text-color)}.no-cat.svelte-1ozsnjl.svelte-1ozsnjl{color:var(--body-text-color)}.no-label.svelte-1ozsnjl.svelte-1ozsnjl{color:var(--body-text-color);user-select:text}.selectable.svelte-1ozsnjl.svelte-1ozsnjl{cursor:text;user-select:text}",
  map: '{"version":3,"file":"InteractiveHighlightedtext.svelte","sources":["InteractiveHighlightedtext.svelte"],"sourcesContent":["<script lang=\\"ts\\">const browser = typeof document !== \\"undefined\\";\\nimport { get_next_color } from \\"@gradio/utils\\";\\nimport { createEventDispatcher, onMount } from \\"svelte\\";\\nimport { correct_color_map, merge_elements } from \\"./utils\\";\\nimport LabelInput from \\"./LabelInput.svelte\\";\\nexport let value = [];\\nexport let show_legend = false;\\nexport let color_map = {};\\nexport let selectable = false;\\nlet activeElementIndex = -1;\\nlet ctx;\\nlet _color_map = {};\\nlet active = \\"\\";\\nlet selection;\\nlet labelToEdit = -1;\\nonMount(() => {\\n    const mouseUpHandler = () => {\\n        selection = window.getSelection();\\n        handleSelectionComplete();\\n        window.removeEventListener(\\"mouseup\\", mouseUpHandler);\\n    };\\n    window.addEventListener(\\"mousedown\\", () => {\\n        window.addEventListener(\\"mouseup\\", mouseUpHandler);\\n    });\\n});\\nasync function handleTextSelected(startIndex, endIndex) {\\n    if (selection?.toString() && activeElementIndex !== -1 && value[activeElementIndex].token.toString().includes(selection.toString())) {\\n        const tempFlag = Symbol();\\n        const str = value[activeElementIndex].token;\\n        const [before, selected, after] = [\\n            str.substring(0, startIndex),\\n            str.substring(startIndex, endIndex),\\n            str.substring(endIndex)\\n        ];\\n        let tempValue = [\\n            ...value.slice(0, activeElementIndex),\\n            { token: before, class_or_confidence: null },\\n            {\\n                token: selected,\\n                class_or_confidence: mode === \\"scores\\" ? 1 : \\"label\\",\\n                flag: tempFlag\\n            },\\n            // add a temp flag to the new highlighted text element\\n            { token: after, class_or_confidence: null },\\n            ...value.slice(activeElementIndex + 1)\\n        ];\\n        labelToEdit = tempValue.findIndex(({ flag }) => flag === tempFlag);\\n        tempValue = tempValue.filter((item) => item.token.trim() !== \\"\\");\\n        value = tempValue.map(({ flag, ...rest }) => rest);\\n        handleValueChange();\\n        document.getElementById(`label-input-${labelToEdit}`)?.focus();\\n    }\\n}\\nconst dispatch = createEventDispatcher();\\nfunction splitTextByNewline(text) {\\n    return text.split(\\"\\\\n\\");\\n}\\nfunction removeHighlightedText(index) {\\n    if (!value || index < 0 || index >= value.length)\\n        return;\\n    value[index].class_or_confidence = null;\\n    value = merge_elements(value, \\"equal\\");\\n    handleValueChange();\\n    window.getSelection()?.empty();\\n}\\nfunction handleValueChange() {\\n    dispatch(\\"change\\", value);\\n    labelToEdit = -1;\\n    if (show_legend) {\\n        color_map = {};\\n        _color_map = {};\\n    }\\n}\\nlet mode;\\n$: {\\n    if (!color_map) {\\n        color_map = {};\\n    }\\n    if (value.length > 0) {\\n        for (let entry of value) {\\n            if (entry.class_or_confidence !== null) {\\n                if (typeof entry.class_or_confidence === \\"string\\") {\\n                    mode = \\"categories\\";\\n                    if (!(entry.class_or_confidence in color_map)) {\\n                        let color = get_next_color(Object.keys(color_map).length);\\n                        color_map[entry.class_or_confidence] = color;\\n                    }\\n                }\\n                else {\\n                    mode = \\"scores\\";\\n                }\\n            }\\n        }\\n    }\\n    correct_color_map(color_map, _color_map, browser, ctx);\\n}\\nfunction handle_mouseover(label) {\\n    active = label;\\n}\\nfunction handle_mouseout() {\\n    active = \\"\\";\\n}\\nasync function handleKeydownSelection(event) {\\n    selection = window.getSelection();\\n    if (event.key === \\"Enter\\") {\\n        handleSelectionComplete();\\n    }\\n}\\nfunction handleSelectionComplete() {\\n    if (selection && selection?.toString().trim() !== \\"\\") {\\n        const textBeginningIndex = selection.getRangeAt(0).startOffset;\\n        const textEndIndex = selection.getRangeAt(0).endOffset;\\n        handleTextSelected(textBeginningIndex, textEndIndex);\\n    }\\n}\\nfunction handleSelect(i, text, class_or_confidence) {\\n    dispatch(\\"select\\", {\\n        index: i,\\n        value: [text, class_or_confidence]\\n    });\\n}\\n<\/script>\\n\\n<div class=\\"container\\">\\n\\t{#if mode === \\"categories\\"}\\n\\t\\t{#if show_legend}\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"class_or_confidence-legend\\"\\n\\t\\t\\t\\tdata-testid=\\"highlighted-text:class_or_confidence-legend\\"\\n\\t\\t\\t>\\n\\t\\t\\t\\t{#if _color_map}\\n\\t\\t\\t\\t\\t{#each Object.entries(_color_map) as [class_or_confidence, color], i}\\n\\t\\t\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\taria-roledescription=\\"Categories of highlighted text. Hover to see text with this class_or_confidence highlighted.\\"\\n\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\ton:mouseover={() => handle_mouseover(class_or_confidence)}\\n\\t\\t\\t\\t\\t\\t\\ton:focus={() => handle_mouseover(class_or_confidence)}\\n\\t\\t\\t\\t\\t\\t\\ton:mouseout={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\t\\ton:blur={() => handle_mouseout()}\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"class_or_confidence-label\\"\\n\\t\\t\\t\\t\\t\\t\\tstyle={\\"background-color:\\" + color.secondary}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{class_or_confidence}\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\n\\t\\t<div class=\\"textfield\\">\\n\\t\\t\\t{#each value as { token, class_or_confidence }, i}\\n\\t\\t\\t\\t{#each splitTextByNewline(token) as line, j}\\n\\t\\t\\t\\t\\t{#if line.trim() !== \\"\\"}\\n\\t\\t\\t\\t\\t\\t<span class=\\"text-class_or_confidence-container\\">\\n\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"textspan\\"\\n\\t\\t\\t\\t\\t\\t\\t\\tstyle:background-color={class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t(active && active !== class_or_confidence)\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t? \\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t: class_or_confidence && _color_map[class_or_confidence]\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t? _color_map[class_or_confidence].secondary\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t: \\"\\"}\\n\\t\\t\\t\\t\\t\\t\\t\\tclass:no-cat={class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t(active && active !== class_or_confidence)}\\n\\t\\t\\t\\t\\t\\t\\t\\tclass:hl={class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\t\\t\\tclass:selectable\\n\\t\\t\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tif (class_or_confidence !== null) {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\thandleSelect(i, token, class_or_confidence);\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:keydown={(e) => {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tif (class_or_confidence !== null) {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tlabelToEdit = i;\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\thandleSelect(i, token, class_or_confidence);\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t} else {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\thandleKeydownSelection(e);\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:focus={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:mouseover={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tclass:no-label={class_or_confidence === null}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"text\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:keydown={(e) => handleKeydownSelection(e)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:focus={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:mouseover={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:click={() => (labelToEdit = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\">{line}</span\\n\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t{#if !show_legend && class_or_confidence !== null && labelToEdit !== i}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tid={`label-tag-${i}`}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"label\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tstyle:background-color={class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t(active && active !== class_or_confidence)\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t? \\"\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t: _color_map[class_or_confidence].primary}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\ton:click={() => (labelToEdit = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\ton:keydown={() => (labelToEdit = i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{class_or_confidence}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t\\t\\t{#if labelToEdit === i && class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t&nbsp;\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t<LabelInput\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tbind:value\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{labelToEdit}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tcategory={class_or_confidence}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{active}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{_color_map}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tindexOfLabel={i}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\ttext={token}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{handleValueChange}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t\\t\\t{#if class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"label-clear-button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\taria-roledescription=\\"Remove label from text\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:click={() => removeHighlightedText(i)}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:keydown={(event) => {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tif (event.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tremoveHighlightedText(i);\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t>×\\n\\t\\t\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t{#if j < splitTextByNewline(token).length - 1}\\n\\t\\t\\t\\t\\t\\t<br />\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t{#if show_legend}\\n\\t\\t\\t<div class=\\"color-legend\\" data-testid=\\"highlighted-text:color-legend\\">\\n\\t\\t\\t\\t<span>-1</span>\\n\\t\\t\\t\\t<span>0</span>\\n\\t\\t\\t\\t<span>+1</span>\\n\\t\\t\\t</div>\\n\\t\\t{/if}\\n\\n\\t\\t<div class=\\"textfield\\" data-testid=\\"highlighted-text:textfield\\">\\n\\t\\t\\t{#each value as { token, class_or_confidence }, i}\\n\\t\\t\\t\\t{@const score =\\n\\t\\t\\t\\t\\ttypeof class_or_confidence === \\"string\\"\\n\\t\\t\\t\\t\\t\\t? parseInt(class_or_confidence)\\n\\t\\t\\t\\t\\t\\t: class_or_confidence}\\n\\t\\t\\t\\t<span class=\\"score-text-container\\">\\n\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\tclass=\\"textspan score-text\\"\\n\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\tclass:no-cat={class_or_confidence === null ||\\n\\t\\t\\t\\t\\t\\t\\t(active && active !== class_or_confidence)}\\n\\t\\t\\t\\t\\t\\tclass:hl={class_or_confidence !== null}\\n\\t\\t\\t\\t\\t\\ton:mouseover={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\ton:focus={() => (activeElementIndex = i)}\\n\\t\\t\\t\\t\\t\\ton:click={() => (labelToEdit = i)}\\n\\t\\t\\t\\t\\t\\ton:keydown={(e) => {\\n\\t\\t\\t\\t\\t\\t\\tif (e.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\tlabelToEdit = i;\\n\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\tstyle={\\"background-color: rgba(\\" +\\n\\t\\t\\t\\t\\t\\t\\t(score && score < 0\\n\\t\\t\\t\\t\\t\\t\\t\\t? \\"128, 90, 213,\\" + -score\\n\\t\\t\\t\\t\\t\\t\\t\\t: \\"239, 68, 60,\\" + score) +\\n\\t\\t\\t\\t\\t\\t\\t\\")\\"}\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t<span class=\\"text\\">{token}</span>\\n\\t\\t\\t\\t\\t\\t{#if class_or_confidence && labelToEdit === i}\\n\\t\\t\\t\\t\\t\\t\\t<LabelInput\\n\\t\\t\\t\\t\\t\\t\\t\\tbind:value\\n\\t\\t\\t\\t\\t\\t\\t\\t{labelToEdit}\\n\\t\\t\\t\\t\\t\\t\\t\\t{_color_map}\\n\\t\\t\\t\\t\\t\\t\\t\\tcategory={class_or_confidence}\\n\\t\\t\\t\\t\\t\\t\\t\\t{active}\\n\\t\\t\\t\\t\\t\\t\\t\\tindexOfLabel={i}\\n\\t\\t\\t\\t\\t\\t\\t\\ttext={token}\\n\\t\\t\\t\\t\\t\\t\\t\\t{handleValueChange}\\n\\t\\t\\t\\t\\t\\t\\t\\tisScoresMode\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t{#if class_or_confidence && activeElementIndex === i}\\n\\t\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"label-clear-button\\"\\n\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\taria-roledescription=\\"Remove label from text\\"\\n\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => removeHighlightedText(i)}\\n\\t\\t\\t\\t\\t\\t\\ton:keydown={(event) => {\\n\\t\\t\\t\\t\\t\\t\\t\\tif (event.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tremoveHighlightedText(i);\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\t>×\\n\\t\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.label-clear-button {\\n\\t\\tdisplay: none;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding-top: 2.5px;\\n\\t\\tpadding-right: var(--size-1);\\n\\t\\tpadding-bottom: 3.5px;\\n\\t\\tpadding-left: var(--size-1);\\n\\t\\tcolor: black;\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tuser-select: none;\\n\\t\\tposition: relative;\\n\\t\\tleft: -3px;\\n\\t\\tborder-radius: 0 var(--radius-xs) var(--radius-xs) 0;\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t}\\n\\n\\t.text-class_or_confidence-container:hover .label-clear-button,\\n\\t.text-class_or_confidence-container:focus-within .label-clear-button,\\n\\t.score-text-container:hover .label-clear-button,\\n\\t.score-text-container:focus-within .label-clear-button {\\n\\t\\tdisplay: inline;\\n\\t}\\n\\n\\t.text-class_or_confidence-container:hover .textspan.hl,\\n\\t.text-class_or_confidence-container:focus-within .textspan.hl,\\n\\t.score-text:hover {\\n\\t\\tborder-radius: var(--radius-xs) 0 0 var(--radius-xs);\\n\\t}\\n\\n\\t.container {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tpadding: var(--block-padding);\\n\\t}\\n\\n\\t.hl {\\n\\t\\tmargin-left: var(--size-1);\\n\\t\\ttransition: background-color 0.3s;\\n\\t\\tuser-select: none;\\n\\t}\\n\\n\\t.textspan:last-child > .label {\\n\\t\\tmargin-right: 0;\\n\\t}\\n\\n\\t.class_or_confidence-legend {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tcolor: black;\\n\\t}\\n\\n\\t.class_or_confidence-label {\\n\\t\\tcursor: pointer;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding-right: var(--size-2);\\n\\t\\tpadding-left: var(--size-2);\\n\\t\\tfont-weight: var(--weight-semibold);\\n\\t}\\n\\n\\t.color-legend {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tbackground: linear-gradient(\\n\\t\\t\\tto right,\\n\\t\\t\\tvar(--color-purple),\\n\\t\\t\\trgba(255, 255, 255, 0),\\n\\t\\t\\tvar(--color-red)\\n\\t\\t);\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\tfont-weight: var(--weight-semibold);\\n\\t}\\n\\n\\t.textfield {\\n\\t\\tbox-sizing: border-box;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tbackground: var(--background-fill-primary);\\n\\t\\tbackground-color: transparent;\\n\\t\\tmax-width: var(--size-full);\\n\\t\\tline-height: var(--scale-4);\\n\\t\\tword-break: break-all;\\n\\t}\\n\\n\\t.textspan {\\n\\t\\ttransition: 150ms;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding-top: 2.5px;\\n\\t\\tpadding-right: var(--size-1);\\n\\t\\tpadding-bottom: 3.5px;\\n\\t\\tpadding-left: var(--size-1);\\n\\t\\tcolor: black;\\n\\t\\tcursor: text;\\n\\t}\\n\\n\\t.label {\\n\\t\\ttransition: 150ms;\\n\\t\\tmargin-top: 1px;\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tpadding: 1px 5px;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tcolor: white;\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t\\tfont-size: var(--text-sm);\\n\\t\\ttext-transform: uppercase;\\n\\t\\tuser-select: none;\\n\\t}\\n\\n\\t.text {\\n\\t\\tcolor: black;\\n\\t\\twhite-space: pre-wrap;\\n\\t}\\n\\n\\t.textspan.hl {\\n\\t\\tuser-select: none;\\n\\t}\\n\\n\\t.score-text-container {\\n\\t\\tmargin-right: var(--size-1);\\n\\t}\\n\\n\\t.score-text .text {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.no-cat {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.no-label {\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tuser-select: text;\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: text;\\n\\t\\tuser-select: text;\\n\\t}</style>\\n"],"names":[],"mappings":"AAkUC,iDAAoB,CACnB,OAAO,CAAE,IAAI,CACb,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,WAAW,CAAE,KAAK,CAClB,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,cAAc,CAAE,KAAK,CACrB,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,KAAK,CAAE,KAAK,CACZ,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,WAAW,CAAE,IAAI,CACjB,QAAQ,CAAE,QAAQ,CAClB,IAAI,CAAE,IAAI,CACV,aAAa,CAAE,CAAC,CAAC,IAAI,WAAW,CAAC,CAAC,IAAI,WAAW,CAAC,CAAC,CAAC,CACpD,KAAK,CAAE,IAAI,wBAAwB,CACpC,CAEA,kDAAmC,MAAM,CAAC,kCAAmB,CAC7D,kDAAmC,aAAa,CAAC,kCAAmB,CACpE,oCAAqB,MAAM,CAAC,kCAAmB,CAC/C,oCAAqB,aAAa,CAAC,kCAAoB,CACtD,OAAO,CAAE,MACV,CAEA,kDAAmC,MAAM,CAAC,SAAS,kBAAG,CACtD,kDAAmC,aAAa,CAAC,SAAS,kBAAG,CAC7D,yCAAW,MAAO,CACjB,aAAa,CAAE,IAAI,WAAW,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,IAAI,WAAW,CACpD,CAEA,wCAAW,CACV,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,OAAO,CAAE,IAAI,eAAe,CAC7B,CAEA,iCAAI,CACH,WAAW,CAAE,IAAI,QAAQ,CAAC,CAC1B,UAAU,CAAE,gBAAgB,CAAC,IAAI,CACjC,WAAW,CAAE,IACd,CAEA,wBAAS,WAAW,CAAG,qBAAO,CAC7B,YAAY,CAAE,CACf,CAEA,yDAA4B,CAC3B,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,IAAI,CACf,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,KAAK,CAAE,KACR,CAEA,wDAA2B,CAC1B,MAAM,CAAE,OAAO,CACf,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,WAAW,CAAE,IAAI,iBAAiB,CACnC,CAEA,2CAAc,CACb,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE;AACd,GAAG,EAAE,CAAC,KAAK,CAAC;AACZ,GAAG,IAAI,cAAc,CAAC,CAAC;AACvB,GAAG,KAAK,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,GAAG,CAAC,CAAC,CAAC,CAAC,CAAC;AAC1B,GAAG,IAAI,WAAW,CAAC;AACnB,GAAG,CACD,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,IAAI,QAAQ,CAAC,CACpC,WAAW,CAAE,IAAI,iBAAiB,CACnC,CAEA,wCAAW,CACV,UAAU,CAAE,UAAU,CACtB,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,UAAU,CAAE,IAAI,yBAAyB,CAAC,CAC1C,gBAAgB,CAAE,WAAW,CAC7B,SAAS,CAAE,IAAI,WAAW,CAAC,CAC3B,WAAW,CAAE,IAAI,SAAS,CAAC,CAC3B,UAAU,CAAE,SACb,CAEA,uCAAU,CACT,UAAU,CAAE,KAAK,CACjB,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,WAAW,CAAE,KAAK,CAClB,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,cAAc,CAAE,KAAK,CACrB,YAAY,CAAE,IAAI,QAAQ,CAAC,CAC3B,KAAK,CAAE,KAAK,CACZ,MAAM,CAAE,IACT,CAEA,oCAAO,CACN,UAAU,CAAE,KAAK,CACjB,UAAU,CAAE,GAAG,CACf,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,OAAO,CAAE,GAAG,CAAC,GAAG,CAChB,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,IAAI,aAAa,CAAC,CAC/B,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,cAAc,CAAE,SAAS,CACzB,WAAW,CAAE,IACd,CAEA,mCAAM,CACL,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,QACd,CAEA,SAAS,iCAAI,CACZ,WAAW,CAAE,IACd,CAEA,mDAAsB,CACrB,YAAY,CAAE,IAAI,QAAQ,CAC3B,CAEA,0BAAW,CAAC,oBAAM,CACjB,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,qCAAQ,CACP,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,uCAAU,CACT,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,WAAW,CAAE,IACd,CAEA,yCAAY,CACX,MAAM,CAAE,IAAI,CACZ,WAAW,CAAE,IACd"}'
};
function splitTextByNewline(text) {
  return text.split("\n");
}
const InteractiveHighlightedtext = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  const browser = typeof document !== "undefined";
  let { value = [] } = $$props;
  let { show_legend = false } = $$props;
  let { color_map = {} } = $$props;
  let { selectable = false } = $$props;
  let activeElementIndex = -1;
  let ctx;
  let _color_map = {};
  let active = "";
  let labelToEdit = -1;
  const dispatch = createEventDispatcher();
  function handleValueChange() {
    dispatch("change", value);
    labelToEdit = -1;
    if (show_legend) {
      color_map = {};
      _color_map = {};
    }
  }
  let mode;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_legend === void 0 && $$bindings.show_legend && show_legend !== void 0)
    $$bindings.show_legend(show_legend);
  if ($$props.color_map === void 0 && $$bindings.color_map && color_map !== void 0)
    $$bindings.color_map(color_map);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      {
        if (!color_map) {
          color_map = {};
        }
        if (value.length > 0) {
          for (let entry of value) {
            if (entry.class_or_confidence !== null) {
              if (typeof entry.class_or_confidence === "string") {
                mode = "categories";
                if (!(entry.class_or_confidence in color_map)) {
                  let color = get_next_color(Object.keys(color_map).length);
                  color_map[entry.class_or_confidence] = color;
                }
              } else {
                mode = "scores";
              }
            }
          }
        }
        correct_color_map(color_map, _color_map, browser, ctx);
      }
    }
    $$rendered = `<div class="container svelte-1ozsnjl">${mode === "categories" ? `${show_legend ? `<div class="class_or_confidence-legend svelte-1ozsnjl" data-testid="highlighted-text:class_or_confidence-legend">${_color_map ? `${each(Object.entries(_color_map), ([class_or_confidence, color], i) => {
      return `<div role="button" aria-roledescription="Categories of highlighted text. Hover to see text with this class_or_confidence highlighted." tabindex="0" class="class_or_confidence-label svelte-1ozsnjl"${add_attribute("style", "background-color:" + color.secondary, 0)}>${escape(class_or_confidence)} </div>`;
    })}` : ``}</div>` : ``} <div class="textfield svelte-1ozsnjl">${each(value, ({ token, class_or_confidence }, i) => {
      return `${each(splitTextByNewline(token), (line, j) => {
        return `${line.trim() !== "" ? `<span class="text-class_or_confidence-container svelte-1ozsnjl"><span role="button" tabindex="0" class="${[
          "textspan svelte-1ozsnjl",
          (class_or_confidence === null || active ? "no-cat" : "") + " " + (class_or_confidence !== null ? "hl" : "") + " " + (selectable ? "selectable" : "")
        ].join(" ").trim()}"${add_styles({
          "background-color": class_or_confidence === null || active ? "" : class_or_confidence && _color_map[class_or_confidence] ? _color_map[class_or_confidence].secondary : ""
        })}><span class="${[
          "text svelte-1ozsnjl",
          class_or_confidence === null ? "no-label" : ""
        ].join(" ").trim()}" role="button" tabindex="0">${escape(line)}</span> ${!show_legend && class_or_confidence !== null && labelToEdit !== i ? `<span${add_attribute("id", `label-tag-${i}`, 0)} class="label svelte-1ozsnjl" role="button" tabindex="0"${add_styles({
          "background-color": class_or_confidence === null || active ? "" : _color_map[class_or_confidence].primary
        })}>${escape(class_or_confidence)} </span>` : ``} ${labelToEdit === i && class_or_confidence !== null ? ` 
									${validate_component(LabelInput, "LabelInput").$$render(
          $$result,
          {
            labelToEdit,
            category: class_or_confidence,
            active,
            _color_map,
            indexOfLabel: i,
            text: token,
            handleValueChange,
            value
          },
          {
            value: ($$value) => {
              value = $$value;
              $$settled = false;
            }
          },
          {}
        )}` : ``}</span> ${class_or_confidence !== null ? `<span class="label-clear-button svelte-1ozsnjl" role="button" aria-roledescription="Remove label from text" tabindex="0" data-svelte-h="svelte-1fuy4vv">×
								</span>` : ``} </span>` : ``} ${j < splitTextByNewline(token).length - 1 ? `<br>` : ``}`;
      })}`;
    })}</div>` : `${show_legend ? `<div class="color-legend svelte-1ozsnjl" data-testid="highlighted-text:color-legend" data-svelte-h="svelte-mv3vmx"><span>-1</span> <span>0</span> <span>+1</span></div>` : ``} <div class="textfield svelte-1ozsnjl" data-testid="highlighted-text:textfield">${each(value, ({ token, class_or_confidence }, i) => {
      let score = typeof class_or_confidence === "string" ? parseInt(class_or_confidence) : class_or_confidence;
      return ` <span class="score-text-container svelte-1ozsnjl"><span class="${[
        "textspan score-text svelte-1ozsnjl",
        (class_or_confidence === null || active ? "no-cat" : "") + " " + (class_or_confidence !== null ? "hl" : "")
      ].join(" ").trim()}" role="button" tabindex="0"${add_attribute(
        "style",
        "background-color: rgba(" + (score && score < 0 ? "128, 90, 213," + -score : "239, 68, 60," + score) + ")",
        0
      )}><span class="text svelte-1ozsnjl">${escape(token)}</span> ${class_or_confidence && labelToEdit === i ? `${validate_component(LabelInput, "LabelInput").$$render(
        $$result,
        {
          labelToEdit,
          _color_map,
          category: class_or_confidence,
          active,
          indexOfLabel: i,
          text: token,
          handleValueChange,
          isScoresMode: true,
          value
        },
        {
          value: ($$value) => {
            value = $$value;
            $$settled = false;
          }
        },
        {}
      )}` : ``}</span> ${class_or_confidence && activeElementIndex === i ? `<span class="label-clear-button svelte-1ozsnjl" role="button" aria-roledescription="Remove label from text" tabindex="0" data-svelte-h="svelte-hxhs1z">×
						</span>` : ``} </span>`;
    })}</div>`} </div>`;
  } while (!$$settled);
  return $$rendered;
});
const InteractiveHighlightedText = InteractiveHighlightedtext;
const { Object: Object_1 } = globals;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { gradio } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { show_legend } = $$props;
  let { show_inline_category } = $$props;
  let { color_map = {} } = $$props;
  let { label = gradio.i18n("highlighted_text.highlighted_text") } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { _selectable = false } = $$props;
  let { combine_adjacent = false } = $$props;
  let { interactive } = $$props;
  let { loading_status } = $$props;
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_legend === void 0 && $$bindings.show_legend && show_legend !== void 0)
    $$bindings.show_legend(show_legend);
  if ($$props.show_inline_category === void 0 && $$bindings.show_inline_category && show_inline_category !== void 0)
    $$bindings.show_inline_category(show_inline_category);
  if ($$props.color_map === void 0 && $$bindings.color_map && color_map !== void 0)
    $$bindings.color_map(color_map);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props._selectable === void 0 && $$bindings._selectable && _selectable !== void 0)
    $$bindings._selectable(_selectable);
  if ($$props.combine_adjacent === void 0 && $$bindings.combine_adjacent && combine_adjacent !== void 0)
    $$bindings.combine_adjacent(combine_adjacent);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      if (!color_map && Object.keys(color_map).length) {
        color_map = color_map;
      }
    }
    {
      if (value && combine_adjacent) {
        value = merge_elements(value);
      }
    }
    {
      {
        if (value !== old_value) {
          old_value = value;
          gradio.dispatch("change");
        }
      }
    }
    $$rendered = `${!interactive ? `${validate_component(Block, "Block").$$render(
      $$result,
      {
        variant: "solid",
        test_id: "highlighted-text",
        visible,
        elem_id,
        elem_classes,
        padding: false,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object_1.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${label ? `${validate_component(BlockLabel, "BlockLabel").$$render(
            $$result,
            {
              Icon: TextHighlight,
              label,
              float: false,
              disable: container === false
            },
            {},
            {}
          )}` : ``} ${value ? `${validate_component(StaticHighlightedText, "StaticHighlightedText").$$render(
            $$result,
            {
              selectable: _selectable,
              value,
              show_legend,
              show_inline_category,
              color_map
            },
            {},
            {}
          )}` : `${validate_component(Empty, "Empty").$$render($$result, {}, {}, {
            default: () => {
              return `${validate_component(TextHighlight, "TextHighlight").$$render($$result, {}, {}, {})}`;
            }
          })}`}`;
        }
      }
    )}` : `${validate_component(Block, "Block").$$render(
      $$result,
      {
        variant: interactive ? "dashed" : "solid",
        test_id: "highlighted-text",
        visible,
        elem_id,
        elem_classes,
        padding: false,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object_1.assign({}, { autoscroll: gradio.autoscroll }, loading_status, { i18n: gradio.i18n }), {}, {})} ${label ? `${validate_component(BlockLabel, "BlockLabel").$$render(
            $$result,
            {
              Icon: TextHighlight,
              label,
              float: false,
              disable: container === false
            },
            {},
            {}
          )}` : ``} ${value ? `${validate_component(InteractiveHighlightedText, "InteractiveHighlightedText").$$render(
            $$result,
            {
              selectable: _selectable,
              show_legend,
              color_map,
              value
            },
            {
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              }
            },
            {}
          )}` : `${validate_component(Empty, "Empty").$$render($$result, {}, {}, {
            default: () => {
              return `${validate_component(TextHighlight, "TextHighlight").$$render($$result, {}, {}, {})}`;
            }
          })}`}`;
        }
      }
    )}`}`;
  } while (!$$settled);
  return $$rendered;
});

export { InteractiveHighlightedText as BaseInteractiveHighlightedText, StaticHighlightedText as BaseStaticHighlightedText, Index as default };
//# sourceMappingURL=Index52-BZBO4I9J.js.map
