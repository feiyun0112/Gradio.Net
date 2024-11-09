import { c as create_ssr_component, v as validate_component, o as onDestroy, d as add_styles, a as createEventDispatcher, e as escape, b as add_attribute, f as each } from './ssr-RaXq3SJh.js';
import { B as Block, e as BlockLabel, an as JSON$1$1, S as Static, h as IconButtonWrapper, i as IconButton, H as Copy, g as Empty } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$1 = {
  code: '.json-node.svelte-19ir0ev{font-family:var(--font-mono);--text-color:#d18770;--key-color:var(--text-color);--string-color:#ce9178;--number-color:#719fad;--bracket-color:#5d8585;--square-bracket-color:#be6069;--punctuation-color:#8fbcbb;--line-number-color:#6a737d;--separator-color:var(--line-number-color)}.json-node.dark-mode.svelte-19ir0ev{--bracket-color:#7eb4b3;--number-color:#638d9a}.json-node.root.svelte-19ir0ev{position:relative;padding-left:var(--size-14)}.json-node.root.svelte-19ir0ev::before{content:"";position:absolute;top:0;bottom:0;left:var(--size-11);width:1px;background-color:var(--separator-color)}.line.svelte-19ir0ev{display:flex;align-items:flex-start;padding:0;margin:0;line-height:var(--line-md)}.line-number.svelte-19ir0ev{position:absolute;left:0;width:calc(var(--size-7));text-align:right;color:var(--line-number-color);user-select:none;text-overflow:ellipsis;text-overflow:ellipsis;direction:rtl;overflow:hidden}.content.svelte-19ir0ev{flex:1;display:flex;align-items:center;padding-left:calc(var(--depth) * var(--size-2));flex-wrap:wrap}.children.svelte-19ir0ev{padding-left:var(--size-4)}.children.hidden.svelte-19ir0ev{display:none}.key.svelte-19ir0ev{color:var(--key-color)}.string.svelte-19ir0ev{color:var(--string-color)}.number.svelte-19ir0ev{color:var(--number-color)}.bool.svelte-19ir0ev{color:var(--text-color)}.null.svelte-19ir0ev{color:var(--text-color)}.value.svelte-19ir0ev{margin-left:var(--spacing-md)}.punctuation.svelte-19ir0ev{color:var(--punctuation-color)}.bracket.svelte-19ir0ev{margin-left:var(--spacing-sm);color:var(--bracket-color)}.square-bracket.svelte-19ir0ev{margin-left:var(--spacing-sm);color:var(--square-bracket-color)}.toggle.svelte-19ir0ev,.preview.svelte-19ir0ev{background:none;border:none;color:inherit;cursor:pointer;padding:0;margin:0}.toggle.svelte-19ir0ev{user-select:none;margin-right:var(--spacing-md)}.preview.svelte-19ir0ev{margin:0 var(--spacing-sm) 0 var(--spacing-lg)}.preview.svelte-19ir0ev:hover{text-decoration:underline}[data-pseudo-content]::before{content:attr(data-pseudo-content)}',
  map: '{"version":3,"file":"JSONNode.svelte","sources":["JSONNode.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount, createEventDispatcher, tick, afterUpdate } from \\"svelte\\";\\nexport let value;\\nexport let depth = 0;\\nexport let is_root = false;\\nexport let is_last_item = true;\\nexport let key = null;\\nexport let open = false;\\nexport let theme_mode = \\"system\\";\\nexport let show_indices = false;\\nconst dispatch = createEventDispatcher();\\nlet root_element;\\nlet collapsed = open ? false : depth >= 3;\\nlet child_nodes = [];\\nfunction is_collapsible(val) {\\n    return val !== null && (typeof val === \\"object\\" || Array.isArray(val));\\n}\\nasync function toggle_collapse() {\\n    collapsed = !collapsed;\\n    await tick();\\n    dispatch(\\"toggle\\", { collapsed, depth });\\n}\\nfunction get_collapsed_preview(val) {\\n    if (Array.isArray(val))\\n        return `Array(${val.length})`;\\n    if (typeof val === \\"object\\" && val !== null)\\n        return `Object(${Object.keys(val).length})`;\\n    return String(val);\\n}\\n$: if (is_collapsible(value)) {\\n    child_nodes = Object.entries(value);\\n}\\nelse {\\n    child_nodes = [];\\n}\\n$: if (is_root && root_element) {\\n    updateLineNumbers();\\n}\\nfunction updateLineNumbers() {\\n    const lines = root_element.querySelectorAll(\\".line\\");\\n    lines.forEach((line, index) => {\\n        const line_number = line.querySelector(\\".line-number\\");\\n        if (line_number) {\\n            line_number.setAttribute(\\"data-pseudo-content\\", (index + 1).toString());\\n            line_number?.setAttribute(\\"aria-roledescription\\", `Line number ${index + 1}`);\\n            line_number?.setAttribute(\\"title\\", `Line number ${index + 1}`);\\n        }\\n    });\\n}\\nonMount(() => {\\n    if (is_root) {\\n        updateLineNumbers();\\n    }\\n});\\nafterUpdate(() => {\\n    if (is_root) {\\n        updateLineNumbers();\\n    }\\n});\\n<\/script>\\n\\n<div\\n\\tclass=\\"json-node\\"\\n\\tclass:root={is_root}\\n\\tclass:dark-mode={theme_mode === \\"dark\\"}\\n\\tbind:this={root_element}\\n\\ton:toggle\\n\\tstyle=\\"--depth: {depth};\\"\\n>\\n\\t<div class=\\"line\\" class:collapsed>\\n\\t\\t<span class=\\"line-number\\"></span>\\n\\t\\t<span class=\\"content\\">\\n\\t\\t\\t{#if is_collapsible(value)}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tdata-pseudo-content={collapsed ? \\"▶\\" : \\"▼\\"}\\n\\t\\t\\t\\t\\taria-label={collapsed ? \\"Expand\\" : \\"Collapse\\"}\\n\\t\\t\\t\\t\\tclass=\\"toggle\\"\\n\\t\\t\\t\\t\\ton:click={toggle_collapse}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if key !== null}\\n\\t\\t\\t\\t<span class=\\"key\\">\\"{key}\\"</span><span class=\\"punctuation colon\\"\\n\\t\\t\\t\\t\\t>:\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if is_collapsible(value)}\\n\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\tclass=\\"punctuation bracket\\"\\n\\t\\t\\t\\t\\tclass:square-bracket={Array.isArray(value)}\\n\\t\\t\\t\\t\\t>{Array.isArray(value) ? \\"[\\" : \\"{\\"}</span\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t{#if collapsed}\\n\\t\\t\\t\\t\\t<button on:click={toggle_collapse} class=\\"preview\\">\\n\\t\\t\\t\\t\\t\\t{get_collapsed_preview(value)}\\n\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\tclass=\\"punctuation bracket\\"\\n\\t\\t\\t\\t\\t\\tclass:square-bracket={Array.isArray(value)}\\n\\t\\t\\t\\t\\t\\t>{Array.isArray(value) ? \\"]\\" : \\"}\\"}</span\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t{:else if typeof value === \\"string\\"}\\n\\t\\t\\t\\t<span class=\\"value string\\">\\"{value}\\"</span>\\n\\t\\t\\t{:else if typeof value === \\"number\\"}\\n\\t\\t\\t\\t<span class=\\"value number\\">{value}</span>\\n\\t\\t\\t{:else if typeof value === \\"boolean\\"}\\n\\t\\t\\t\\t<span class=\\"value bool\\">{value.toString()}</span>\\n\\t\\t\\t{:else if value === null}\\n\\t\\t\\t\\t<span class=\\"value null\\">null</span>\\n\\t\\t\\t{:else}\\n\\t\\t\\t\\t<span>{value}</span>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if !is_last_item && (!is_collapsible(value) || collapsed)}\\n\\t\\t\\t\\t<span class=\\"punctuation\\">,</span>\\n\\t\\t\\t{/if}\\n\\t\\t</span>\\n\\t</div>\\n\\n\\t{#if is_collapsible(value)}\\n\\t\\t<div class=\\"children\\" class:hidden={collapsed}>\\n\\t\\t\\t{#each child_nodes as [subKey, subVal], i}\\n\\t\\t\\t\\t<svelte:self\\n\\t\\t\\t\\t\\tvalue={subVal}\\n\\t\\t\\t\\t\\tdepth={depth + 1}\\n\\t\\t\\t\\t\\tis_last_item={i === child_nodes.length - 1}\\n\\t\\t\\t\\t\\tkey={subKey}\\n\\t\\t\\t\\t\\t{open}\\n\\t\\t\\t\\t\\t{theme_mode}\\n\\t\\t\\t\\t\\t{show_indices}\\n\\t\\t\\t\\t\\ton:toggle\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/each}\\n\\t\\t\\t<div class=\\"line\\">\\n\\t\\t\\t\\t<span class=\\"line-number\\"></span>\\n\\t\\t\\t\\t<span class=\\"content\\">\\n\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\tclass=\\"punctuation bracket\\"\\n\\t\\t\\t\\t\\t\\tclass:square-bracket={Array.isArray(value)}\\n\\t\\t\\t\\t\\t\\t>{Array.isArray(value) ? \\"]\\" : \\"}\\"}</span\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#if !is_last_item}<span class=\\"punctuation\\">,</span>{/if}\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.json-node {\\n\\t\\tfont-family: var(--font-mono);\\n\\t\\t--text-color: #d18770;\\n\\t\\t--key-color: var(--text-color);\\n\\t\\t--string-color: #ce9178;\\n\\t\\t--number-color: #719fad;\\n\\n\\t\\t--bracket-color: #5d8585;\\n\\t\\t--square-bracket-color: #be6069;\\n\\t\\t--punctuation-color: #8fbcbb;\\n\\t\\t--line-number-color: #6a737d;\\n\\t\\t--separator-color: var(--line-number-color);\\n\\t}\\n\\t.json-node.dark-mode {\\n\\t\\t--bracket-color: #7eb4b3;\\n\\t\\t--number-color: #638d9a;\\n\\t}\\n\\t.json-node.root {\\n\\t\\tposition: relative;\\n\\t\\tpadding-left: var(--size-14);\\n\\t}\\n\\t.json-node.root::before {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\ttop: 0;\\n\\t\\tbottom: 0;\\n\\t\\tleft: var(--size-11);\\n\\t\\twidth: 1px;\\n\\t\\tbackground-color: var(--separator-color);\\n\\t}\\n\\t.line {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: flex-start;\\n\\t\\tpadding: 0;\\n\\t\\tmargin: 0;\\n\\t\\tline-height: var(--line-md);\\n\\t}\\n\\t.line-number {\\n\\t\\tposition: absolute;\\n\\t\\tleft: 0;\\n\\t\\twidth: calc(var(--size-7));\\n\\t\\ttext-align: right;\\n\\t\\tcolor: var(--line-number-color);\\n\\t\\tuser-select: none;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\tdirection: rtl;\\n\\t\\toverflow: hidden;\\n\\t}\\n\\t.content {\\n\\t\\tflex: 1;\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tpadding-left: calc(var(--depth) * var(--size-2));\\n\\t\\tflex-wrap: wrap;\\n\\t}\\n\\t.children {\\n\\t\\tpadding-left: var(--size-4);\\n\\t}\\n\\t.children.hidden {\\n\\t\\tdisplay: none;\\n\\t}\\n\\t.key {\\n\\t\\tcolor: var(--key-color);\\n\\t}\\n\\t.string {\\n\\t\\tcolor: var(--string-color);\\n\\t}\\n\\t.number {\\n\\t\\tcolor: var(--number-color);\\n\\t}\\n\\t.bool {\\n\\t\\tcolor: var(--text-color);\\n\\t}\\n\\t.null {\\n\\t\\tcolor: var(--text-color);\\n\\t}\\n\\t.value {\\n\\t\\tmargin-left: var(--spacing-md);\\n\\t}\\n\\t.punctuation {\\n\\t\\tcolor: var(--punctuation-color);\\n\\t}\\n\\t.bracket {\\n\\t\\tmargin-left: var(--spacing-sm);\\n\\t\\tcolor: var(--bracket-color);\\n\\t}\\n\\t.square-bracket {\\n\\t\\tmargin-left: var(--spacing-sm);\\n\\t\\tcolor: var(--square-bracket-color);\\n\\t}\\n\\t.toggle,\\n\\t.preview {\\n\\t\\tbackground: none;\\n\\t\\tborder: none;\\n\\t\\tcolor: inherit;\\n\\t\\tcursor: pointer;\\n\\t\\tpadding: 0;\\n\\t\\tmargin: 0;\\n\\t}\\n\\t.toggle {\\n\\t\\tuser-select: none;\\n\\t\\tmargin-right: var(--spacing-md);\\n\\t}\\n\\t.preview {\\n\\t\\tmargin: 0 var(--spacing-sm) 0 var(--spacing-lg);\\n\\t}\\n\\t.preview:hover {\\n\\t\\ttext-decoration: underline;\\n\\t}\\n\\n\\t:global([data-pseudo-content])::before {\\n\\t\\tcontent: attr(data-pseudo-content);\\n\\t}</style>\\n"],"names":[],"mappings":"AAmJC,yBAAW,CACV,WAAW,CAAE,IAAI,WAAW,CAAC,CAC7B,YAAY,CAAE,OAAO,CACrB,WAAW,CAAE,iBAAiB,CAC9B,cAAc,CAAE,OAAO,CACvB,cAAc,CAAE,OAAO,CAEvB,eAAe,CAAE,OAAO,CACxB,sBAAsB,CAAE,OAAO,CAC/B,mBAAmB,CAAE,OAAO,CAC5B,mBAAmB,CAAE,OAAO,CAC5B,iBAAiB,CAAE,wBACpB,CACA,UAAU,yBAAW,CACpB,eAAe,CAAE,OAAO,CACxB,cAAc,CAAE,OACjB,CACA,UAAU,oBAAM,CACf,QAAQ,CAAE,QAAQ,CAClB,YAAY,CAAE,IAAI,SAAS,CAC5B,CACA,UAAU,oBAAK,QAAS,CACvB,OAAO,CAAE,EAAE,CACX,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,CAAC,CACN,MAAM,CAAE,CAAC,CACT,IAAI,CAAE,IAAI,SAAS,CAAC,CACpB,KAAK,CAAE,GAAG,CACV,gBAAgB,CAAE,IAAI,iBAAiB,CACxC,CACA,oBAAM,CACL,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,UAAU,CACvB,OAAO,CAAE,CAAC,CACV,MAAM,CAAE,CAAC,CACT,WAAW,CAAE,IAAI,SAAS,CAC3B,CACA,2BAAa,CACZ,QAAQ,CAAE,QAAQ,CAClB,IAAI,CAAE,CAAC,CACP,KAAK,CAAE,KAAK,IAAI,QAAQ,CAAC,CAAC,CAC1B,UAAU,CAAE,KAAK,CACjB,KAAK,CAAE,IAAI,mBAAmB,CAAC,CAC/B,WAAW,CAAE,IAAI,CACjB,aAAa,CAAE,QAAQ,CACvB,aAAa,CAAE,QAAQ,CACvB,SAAS,CAAE,GAAG,CACd,QAAQ,CAAE,MACX,CACA,uBAAS,CACR,IAAI,CAAE,CAAC,CACP,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,YAAY,CAAE,KAAK,IAAI,OAAO,CAAC,CAAC,CAAC,CAAC,IAAI,QAAQ,CAAC,CAAC,CAChD,SAAS,CAAE,IACZ,CACA,wBAAU,CACT,YAAY,CAAE,IAAI,QAAQ,CAC3B,CACA,SAAS,sBAAQ,CAChB,OAAO,CAAE,IACV,CACA,mBAAK,CACJ,KAAK,CAAE,IAAI,WAAW,CACvB,CACA,sBAAQ,CACP,KAAK,CAAE,IAAI,cAAc,CAC1B,CACA,sBAAQ,CACP,KAAK,CAAE,IAAI,cAAc,CAC1B,CACA,oBAAM,CACL,KAAK,CAAE,IAAI,YAAY,CACxB,CACA,oBAAM,CACL,KAAK,CAAE,IAAI,YAAY,CACxB,CACA,qBAAO,CACN,WAAW,CAAE,IAAI,YAAY,CAC9B,CACA,2BAAa,CACZ,KAAK,CAAE,IAAI,mBAAmB,CAC/B,CACA,uBAAS,CACR,WAAW,CAAE,IAAI,YAAY,CAAC,CAC9B,KAAK,CAAE,IAAI,eAAe,CAC3B,CACA,8BAAgB,CACf,WAAW,CAAE,IAAI,YAAY,CAAC,CAC9B,KAAK,CAAE,IAAI,sBAAsB,CAClC,CACA,sBAAO,CACP,uBAAS,CACR,UAAU,CAAE,IAAI,CAChB,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,OAAO,CACd,MAAM,CAAE,OAAO,CACf,OAAO,CAAE,CAAC,CACV,MAAM,CAAE,CACT,CACA,sBAAQ,CACP,WAAW,CAAE,IAAI,CACjB,YAAY,CAAE,IAAI,YAAY,CAC/B,CACA,uBAAS,CACR,MAAM,CAAE,CAAC,CAAC,IAAI,YAAY,CAAC,CAAC,CAAC,CAAC,IAAI,YAAY,CAC/C,CACA,uBAAQ,MAAO,CACd,eAAe,CAAE,SAClB,CAEQ,qBAAsB,QAAS,CACtC,OAAO,CAAE,KAAK,mBAAmB,CAClC"}'
};
function is_collapsible(val) {
  return val !== null && (typeof val === "object" || Array.isArray(val));
}
function get_collapsed_preview(val) {
  if (Array.isArray(val))
    return `Array(${val.length})`;
  if (typeof val === "object" && val !== null)
    return `Object(${Object.keys(val).length})`;
  return String(val);
}
const JSONNode = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { depth = 0 } = $$props;
  let { is_root = false } = $$props;
  let { is_last_item = true } = $$props;
  let { key = null } = $$props;
  let { open = false } = $$props;
  let { theme_mode = "system" } = $$props;
  let { show_indices = false } = $$props;
  createEventDispatcher();
  let root_element;
  let collapsed = open ? false : depth >= 3;
  let child_nodes = [];
  function updateLineNumbers() {
    const lines = root_element.querySelectorAll(".line");
    lines.forEach((line, index) => {
      const line_number = line.querySelector(".line-number");
      if (line_number) {
        line_number.setAttribute("data-pseudo-content", (index + 1).toString());
        line_number?.setAttribute("aria-roledescription", `Line number ${index + 1}`);
        line_number?.setAttribute("title", `Line number ${index + 1}`);
      }
    });
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.depth === void 0 && $$bindings.depth && depth !== void 0)
    $$bindings.depth(depth);
  if ($$props.is_root === void 0 && $$bindings.is_root && is_root !== void 0)
    $$bindings.is_root(is_root);
  if ($$props.is_last_item === void 0 && $$bindings.is_last_item && is_last_item !== void 0)
    $$bindings.is_last_item(is_last_item);
  if ($$props.key === void 0 && $$bindings.key && key !== void 0)
    $$bindings.key(key);
  if ($$props.open === void 0 && $$bindings.open && open !== void 0)
    $$bindings.open(open);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.show_indices === void 0 && $$bindings.show_indices && show_indices !== void 0)
    $$bindings.show_indices(show_indices);
  $$result.css.add(css$1);
  {
    if (is_collapsible(value)) {
      child_nodes = Object.entries(value);
    } else {
      child_nodes = [];
    }
  }
  {
    if (is_root && root_element) {
      updateLineNumbers();
    }
  }
  return `<div class="${[
    "json-node svelte-19ir0ev",
    (is_root ? "root" : "") + " " + (theme_mode === "dark" ? "dark-mode" : "")
  ].join(" ").trim()}" style="${"--depth: " + escape(depth, true) + ";"}"${add_attribute("this", root_element, 0)}><div class="${["line svelte-19ir0ev", collapsed ? "collapsed" : ""].join(" ").trim()}"><span class="line-number svelte-19ir0ev"></span> <span class="content svelte-19ir0ev">${is_collapsible(value) ? `<button${add_attribute("data-pseudo-content", collapsed ? "▶" : "▼", 0)}${add_attribute("aria-label", collapsed ? "Expand" : "Collapse", 0)} class="toggle svelte-19ir0ev"></button>` : ``} ${key !== null ? `<span class="key svelte-19ir0ev">&quot;${escape(key)}&quot;</span><span class="punctuation colon svelte-19ir0ev" data-svelte-h="svelte-1cahzs5">:</span>` : ``} ${is_collapsible(value) ? `<span class="${[
    "punctuation bracket svelte-19ir0ev",
    Array.isArray(value) ? "square-bracket" : ""
  ].join(" ").trim()}">${escape(Array.isArray(value) ? "[" : "{")}</span> ${collapsed ? `<button class="preview svelte-19ir0ev">${escape(get_collapsed_preview(value))}</button> <span class="${[
    "punctuation bracket svelte-19ir0ev",
    Array.isArray(value) ? "square-bracket" : ""
  ].join(" ").trim()}">${escape(Array.isArray(value) ? "]" : "}")}</span>` : ``}` : `${typeof value === "string" ? `<span class="value string svelte-19ir0ev">&quot;${escape(value)}&quot;</span>` : `${typeof value === "number" ? `<span class="value number svelte-19ir0ev">${escape(value)}</span>` : `${typeof value === "boolean" ? `<span class="value bool svelte-19ir0ev">${escape(value.toString())}</span>` : `${value === null ? `<span class="value null svelte-19ir0ev" data-svelte-h="svelte-xcjkvs">null</span>` : `<span>${escape(value)}</span>`}`}`}`}`} ${!is_last_item && (!is_collapsible(value) || collapsed) ? `<span class="punctuation svelte-19ir0ev" data-svelte-h="svelte-19nlgjl">,</span>` : ``}</span></div> ${is_collapsible(value) ? `<div class="${["children svelte-19ir0ev", collapsed ? "hidden" : ""].join(" ").trim()}">${each(child_nodes, ([subKey, subVal], i) => {
    return `${validate_component(JSONNode, "svelte:self").$$render(
      $$result,
      {
        value: subVal,
        depth: depth + 1,
        is_last_item: i === child_nodes.length - 1,
        key: subKey,
        open,
        theme_mode,
        show_indices
      },
      {},
      {}
    )}`;
  })} <div class="line svelte-19ir0ev"><span class="line-number svelte-19ir0ev"></span> <span class="content svelte-19ir0ev"><span class="${[
    "punctuation bracket svelte-19ir0ev",
    Array.isArray(value) ? "square-bracket" : ""
  ].join(" ").trim()}">${escape(Array.isArray(value) ? "]" : "}")}</span> ${!is_last_item ? `<span class="punctuation svelte-19ir0ev" data-svelte-h="svelte-19nlgjl">,</span>` : ``}</span></div></div>` : ``} </div>`;
});
const css = {
  code: ".copied svg{animation:svelte-ryarus-fade ease 300ms;animation-fill-mode:forwards}@keyframes svelte-ryarus-fade{0%{opacity:0}100%{opacity:1}}.json-holder.svelte-ryarus{padding:var(--size-2);overflow-y:auto}.empty-wrapper.svelte-ryarus{min-height:calc(var(--size-32) - 20px);height:100%}",
  map: '{"version":3,"file":"JSON.svelte","sources":["JSON.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onDestroy } from \\"svelte\\";\\nimport { JSON as JSONIcon } from \\"@gradio/icons\\";\\nimport { Empty, IconButtonWrapper, IconButton } from \\"@gradio/atoms\\";\\nimport JSONNode from \\"./JSONNode.svelte\\";\\nimport { Copy, Check } from \\"@gradio/icons\\";\\nexport let value = {};\\nexport let open = false;\\nexport let theme_mode = \\"system\\";\\nexport let show_indices = false;\\nexport let label_height;\\n$: json_max_height = `calc(100% - ${label_height}px)`;\\nlet copied = false;\\nlet timer;\\nfunction copy_feedback() {\\n    copied = true;\\n    if (timer)\\n        clearTimeout(timer);\\n    timer = setTimeout(() => {\\n        copied = false;\\n    }, 1e3);\\n}\\nasync function handle_copy() {\\n    if (\\"clipboard\\" in navigator) {\\n        await navigator.clipboard.writeText(JSON.stringify(value, null, 2));\\n        copy_feedback();\\n    }\\n}\\nfunction is_empty(obj) {\\n    return obj && Object.keys(obj).length === 0 && Object.getPrototypeOf(obj) === Object.prototype && JSON.stringify(obj) === JSON.stringify({});\\n}\\nonDestroy(() => {\\n    if (timer)\\n        clearTimeout(timer);\\n});\\n<\/script>\\n\\n{#if value && value !== \'\\"\\"\' && !is_empty(value)}\\n\\t<IconButtonWrapper>\\n\\t\\t<IconButton\\n\\t\\t\\tshow_label={false}\\n\\t\\t\\tlabel={copied ? \\"Copied\\" : \\"Copy\\"}\\n\\t\\t\\tIcon={copied ? Check : Copy}\\n\\t\\t\\ton:click={() => handle_copy()}\\n\\t\\t/>\\n\\t</IconButtonWrapper>\\n\\t<div class=\\"json-holder\\" style:max-height={json_max_height}>\\n\\t\\t<JSONNode\\n\\t\\t\\t{value}\\n\\t\\t\\tdepth={0}\\n\\t\\t\\tis_root={true}\\n\\t\\t\\t{open}\\n\\t\\t\\t{theme_mode}\\n\\t\\t\\t{show_indices}\\n\\t\\t/>\\n\\t</div>\\n{:else}\\n\\t<div class=\\"empty-wrapper\\">\\n\\t\\t<Empty>\\n\\t\\t\\t<JSONIcon />\\n\\t\\t</Empty>\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t:global(.copied svg) {\\n\\t\\tanimation: fade ease 300ms;\\n\\t\\tanimation-fill-mode: forwards;\\n\\t}\\n\\n\\t@keyframes fade {\\n\\t\\t0% {\\n\\t\\t\\topacity: 0;\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\topacity: 1;\\n\\t\\t}\\n\\t}\\n\\n\\t.json-holder {\\n\\t\\tpadding: var(--size-2);\\n\\t\\toverflow-y: auto;\\n\\t}\\n\\n\\t.empty-wrapper {\\n\\t\\tmin-height: calc(var(--size-32) - 20px);\\n\\t\\theight: 100%;\\n\\t}</style>\\n"],"names":[],"mappings":"AAgES,WAAa,CACpB,SAAS,CAAE,kBAAI,CAAC,IAAI,CAAC,KAAK,CAC1B,mBAAmB,CAAE,QACtB,CAEA,WAAW,kBAAK,CACf,EAAG,CACF,OAAO,CAAE,CACV,CACA,IAAK,CACJ,OAAO,CAAE,CACV,CACD,CAEA,0BAAa,CACZ,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,UAAU,CAAE,IACb,CAEA,4BAAe,CACd,UAAU,CAAE,KAAK,IAAI,SAAS,CAAC,CAAC,CAAC,CAAC,IAAI,CAAC,CACvC,MAAM,CAAE,IACT"}'
};
function is_empty(obj) {
  return obj && Object.keys(obj).length === 0 && Object.getPrototypeOf(obj) === Object.prototype && JSON.stringify(obj) === JSON.stringify({});
}
const JSON_1 = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let json_max_height;
  let { value = {} } = $$props;
  let { open = false } = $$props;
  let { theme_mode = "system" } = $$props;
  let { show_indices = false } = $$props;
  let { label_height } = $$props;
  onDestroy(() => {
  });
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.open === void 0 && $$bindings.open && open !== void 0)
    $$bindings.open(open);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.show_indices === void 0 && $$bindings.show_indices && show_indices !== void 0)
    $$bindings.show_indices(show_indices);
  if ($$props.label_height === void 0 && $$bindings.label_height && label_height !== void 0)
    $$bindings.label_height(label_height);
  $$result.css.add(css);
  json_max_height = `calc(100% - ${label_height}px)`;
  return `${value && value !== '""' && !is_empty(value) ? `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          show_label: false,
          label: "Copy",
          Icon: Copy
        },
        {},
        {}
      )}`;
    }
  })} <div class="json-holder svelte-ryarus"${add_styles({ "max-height": json_max_height })}>${validate_component(JSONNode, "JSONNode").$$render(
    $$result,
    {
      value,
      depth: 0,
      is_root: true,
      open,
      theme_mode,
      show_indices
    },
    {},
    {}
  )}</div>` : `<div class="empty-wrapper svelte-ryarus">${validate_component(Empty, "Empty").$$render($$result, {}, {}, {
    default: () => {
      return `${validate_component(JSON$1$1, "JSONIcon").$$render($$result, {}, {}, {})}`;
    }
  })}</div>`}`;
});
const JSON$1 = JSON_1;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { loading_status } = $$props;
  let { label } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { open = false } = $$props;
  let { theme_mode } = $$props;
  let { show_indices } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let label_height = 0;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.open === void 0 && $$bindings.open && open !== void 0)
    $$bindings.open(open);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.show_indices === void 0 && $$bindings.show_indices && show_indices !== void 0)
    $$bindings.show_indices(show_indices);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.max_height === void 0 && $$bindings.max_height && max_height !== void 0)
    $$bindings.max_height(max_height);
  {
    {
      if (value !== old_value) {
        old_value = value;
        gradio.dispatch("change");
      }
    }
  }
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      test_id: "json",
      elem_id,
      elem_classes,
      container,
      scale,
      min_width,
      padding: false,
      allow_overflow: true,
      overflow_behavior: "auto",
      height,
      min_height,
      max_height
    },
    {},
    {
      default: () => {
        return `<div>${label ? `${validate_component(BlockLabel, "BlockLabel").$$render(
          $$result,
          {
            Icon: JSON$1$1,
            show_label,
            label,
            float: false,
            disable: container === false
          },
          {},
          {}
        )}` : ``}</div> ${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(JSON$1, "JSON").$$render(
          $$result,
          {
            value,
            open,
            theme_mode,
            show_indices,
            label_height
          },
          {},
          {}
        )}`;
      }
    }
  )}`;
});

export { JSON$1 as BaseJSON, Index as default };
//# sourceMappingURL=Index48-BRRuz8G_.js.map
