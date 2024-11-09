import { c as create_ssr_component, v as validate_component, e as escape, f as each } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".svelte-ei2xnu:where(._jsonList){list-style:none;margin:0;padding:0;padding-left:var(--jsonPaddingLeft, 1rem);border-left:var(--jsonBorderLeft, 1px dotted)}.svelte-ei2xnu:where(._jsonBkt){color:var(--jsonBracketColor, currentcolor)}.svelte-ei2xnu:where(._jsonBkt):not(.empty):hover{cursor:pointer;background:var(--jsonBracketHoverBackground, #e5e7eb)}.svelte-ei2xnu:where(._jsonSep){color:var(--jsonSeparatorColor, currentcolor)}.svelte-ei2xnu:where(._jsonKey){color:var(--jsonKeyColor, currentcolor)}.svelte-ei2xnu:where(._jsonVal){color:var(--jsonValColor, #9ca3af)}:where(._jsonVal).string.svelte-ei2xnu{color:var(--jsonValStringColor, #059669)}:where(._jsonVal).number.svelte-ei2xnu{color:var(--jsonValNumberColor, #d97706)}:where(._jsonVal).boolean.svelte-ei2xnu{color:var(--jsonValBooleanColor, #2563eb)}",
  map: `{"version":3,"file":"JsonView.svelte","sources":["JsonView.svelte"],"sourcesContent":["<script>\\n/** @type {*} - object or array to display */\\nexport let json\\n/** @type {number} - initial expansion depth */\\nexport let depth = Infinity\\nexport let _cur = 0\\nexport let _last = true\\n\\n/** @type {*[]} */\\nlet items\\nlet isArray = false\\nlet brackets = ['', '']\\nlet collapsed = false\\n\\n/**\\n * @param {*} i\\n * @returns {string}\\n */\\nfunction getType(i) {\\n  if (i === null) return 'null'\\n  return typeof i\\n}\\n\\n/**\\n * @param {*} i\\n * @returns {string}\\n */\\nfunction format(i) {\\n  const t = getType(i)\\n  if (t === 'string') return \`\\"\${i}\\"\`\\n  if (t === 'function') return 'f () {...}'\\n  if (t === 'symbol') return i.toString()\\n  return i\\n}\\n\\nfunction clicked() {\\n  collapsed = !collapsed\\n}\\n\\n/**\\n * @param {Event} e\\n */\\nfunction pressed(e) {\\n  if (e instanceof KeyboardEvent && ['Enter', ' '].includes(e.key)) clicked()\\n}\\n\\n$: {\\n  items = getType(json) === 'object' ? Object.keys(json) : []\\n  isArray = Array.isArray(json)\\n  brackets = isArray ? ['[', ']'] : ['{', '}']\\n}\\n\\n$: collapsed = depth < _cur\\n<\/script>\\n\\n{#if !items.length}\\n  <span class=\\"_jsonBkt empty\\" class:isArray>{brackets[0]}{brackets[1]}</span>{#if !_last}<span\\n      class=\\"_jsonSep\\">,</span\\n    >{/if}\\n{:else if collapsed}\\n  <span\\n    class=\\"_jsonBkt\\"\\n    class:isArray\\n    role=\\"button\\"\\n    tabindex=\\"0\\"\\n    on:click={clicked}\\n    on:keydown={pressed}>{brackets[0]}...{brackets[1]}</span\\n  >{#if !_last && collapsed}<span class=\\"_jsonSep\\">,</span>{/if}\\n{:else}\\n  <span\\n    class=\\"_jsonBkt\\"\\n    class:isArray\\n    role=\\"button\\"\\n    tabindex=\\"0\\"\\n    on:click={clicked}\\n    on:keydown={pressed}>{brackets[0]}</span\\n  >\\n  <ul class=\\"_jsonList\\">\\n    {#each items as i, idx}\\n      <li>\\n        {#if !isArray}\\n          <span class=\\"_jsonKey\\">\\"{i}\\"</span><span class=\\"_jsonSep\\">:</span>\\n        {/if}\\n        {#if getType(json[i]) === 'object'}\\n          <svelte:self json={json[i]} {depth} _cur={_cur + 1} _last={idx === items.length - 1} />\\n        {:else}\\n          <span class=\\"_jsonVal {getType(json[i])}\\">{format(json[i])}</span\\n          >{#if idx < items.length - 1}<span class=\\"_jsonSep\\">,</span>{/if}\\n        {/if}\\n      </li>\\n    {/each}\\n  </ul>\\n  <span\\n    class=\\"_jsonBkt\\"\\n    class:isArray\\n    role=\\"button\\"\\n    tabindex=\\"0\\"\\n    on:click={clicked}\\n    on:keydown={pressed}>{brackets[1]}</span\\n  >{#if !_last}<span class=\\"_jsonSep\\">,</span>{/if}\\n{/if}\\n\\n<style>\\n:where(._jsonList) {\\n  list-style: none;\\n  margin: 0;\\n  padding: 0;\\n  padding-left: var(--jsonPaddingLeft, 1rem);\\n  border-left: var(--jsonBorderLeft, 1px dotted);\\n}\\n:where(._jsonBkt) {\\n  color: var(--jsonBracketColor, currentcolor);\\n}\\n:where(._jsonBkt):not(.empty):hover {\\n  cursor: pointer;\\n  background: var(--jsonBracketHoverBackground, #e5e7eb);\\n}\\n:where(._jsonSep) {\\n  color: var(--jsonSeparatorColor, currentcolor);\\n}\\n:where(._jsonKey) {\\n  color: var(--jsonKeyColor, currentcolor);\\n}\\n:where(._jsonVal) {\\n  color: var(--jsonValColor, #9ca3af);\\n}\\n:where(._jsonVal).string {\\n  color: var(--jsonValStringColor, #059669);\\n}\\n:where(._jsonVal).number {\\n  color: var(--jsonValNumberColor, #d97706);\\n}\\n:where(._jsonVal).boolean {\\n  color: var(--jsonValBooleanColor, #2563eb);\\n}</style>\\n"],"names":[],"mappings":"cAuGA,OAAO,UAAU,CAAE,CACjB,UAAU,CAAE,IAAI,CAChB,MAAM,CAAE,CAAC,CACT,OAAO,CAAE,CAAC,CACV,YAAY,CAAE,IAAI,iBAAiB,CAAC,KAAK,CAAC,CAC1C,WAAW,CAAE,IAAI,gBAAgB,CAAC,WAAW,CAC/C,eACA,OAAO,SAAS,CAAE,CAChB,KAAK,CAAE,IAAI,kBAAkB,CAAC,aAAa,CAC7C,eACA,OAAO,SAAS,CAAC,KAAK,MAAM,CAAC,MAAO,CAClC,MAAM,CAAE,OAAO,CACf,UAAU,CAAE,IAAI,4BAA4B,CAAC,QAAQ,CACvD,eACA,OAAO,SAAS,CAAE,CAChB,KAAK,CAAE,IAAI,oBAAoB,CAAC,aAAa,CAC/C,eACA,OAAO,SAAS,CAAE,CAChB,KAAK,CAAE,IAAI,cAAc,CAAC,aAAa,CACzC,eACA,OAAO,SAAS,CAAE,CAChB,KAAK,CAAE,IAAI,cAAc,CAAC,QAAQ,CACpC,CACA,OAAO,SAAS,CAAC,qBAAQ,CACvB,KAAK,CAAE,IAAI,oBAAoB,CAAC,QAAQ,CAC1C,CACA,OAAO,SAAS,CAAC,qBAAQ,CACvB,KAAK,CAAE,IAAI,oBAAoB,CAAC,QAAQ,CAC1C,CACA,OAAO,SAAS,CAAC,sBAAS,CACxB,KAAK,CAAE,IAAI,qBAAqB,CAAC,QAAQ,CAC3C"}`
};
function getType(i) {
  if (i === null)
    return "null";
  return typeof i;
}
function format(i) {
  const t = getType(i);
  if (t === "string")
    return `"${i}"`;
  if (t === "function")
    return "f () {...}";
  if (t === "symbol")
    return i.toString();
  return i;
}
const JsonView = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { json } = $$props;
  let { depth = Infinity } = $$props;
  let { _cur = 0 } = $$props;
  let { _last = true } = $$props;
  let items;
  let isArray = false;
  let brackets = ["", ""];
  let collapsed = false;
  if ($$props.json === void 0 && $$bindings.json && json !== void 0)
    $$bindings.json(json);
  if ($$props.depth === void 0 && $$bindings.depth && depth !== void 0)
    $$bindings.depth(depth);
  if ($$props._cur === void 0 && $$bindings._cur && _cur !== void 0)
    $$bindings._cur(_cur);
  if ($$props._last === void 0 && $$bindings._last && _last !== void 0)
    $$bindings._last(_last);
  $$result.css.add(css);
  {
    {
      items = getType(json) === "object" ? Object.keys(json) : [];
      isArray = Array.isArray(json);
      brackets = isArray ? ["[", "]"] : ["{", "}"];
    }
  }
  collapsed = depth < _cur;
  return `${!items.length ? `<span class="${["_jsonBkt empty svelte-ei2xnu", isArray ? "isArray" : ""].join(" ").trim()}">${escape(brackets[0])}${escape(brackets[1])}</span>${!_last ? `<span class="_jsonSep svelte-ei2xnu" data-svelte-h="svelte-1f29ohw">,</span>` : ``}` : `${collapsed ? `<span class="${["_jsonBkt svelte-ei2xnu", isArray ? "isArray" : ""].join(" ").trim()}" role="button" tabindex="0">${escape(brackets[0])}...${escape(brackets[1])}</span>${!_last && collapsed ? `<span class="_jsonSep svelte-ei2xnu" data-svelte-h="svelte-1inngla">,</span>` : ``}` : `<span class="${["_jsonBkt svelte-ei2xnu", isArray ? "isArray" : ""].join(" ").trim()}" role="button" tabindex="0">${escape(brackets[0])}</span> <ul class="_jsonList svelte-ei2xnu">${each(items, (i, idx) => {
    return `<li class="svelte-ei2xnu">${!isArray ? `<span class="_jsonKey svelte-ei2xnu">&quot;${escape(i)}&quot;</span><span class="_jsonSep svelte-ei2xnu" data-svelte-h="svelte-168684w">:</span>` : ``} ${getType(json[i]) === "object" ? `${validate_component(JsonView, "svelte:self").$$render(
      $$result,
      {
        json: json[i],
        depth,
        _cur: _cur + 1,
        _last: idx === items.length - 1
      },
      {},
      {}
    )}` : `<span class="${"_jsonVal " + escape(getType(json[i]), true) + " svelte-ei2xnu"}">${escape(format(json[i]))}</span>${idx < items.length - 1 ? `<span class="_jsonSep svelte-ei2xnu" data-svelte-h="svelte-1inngla">,</span>` : ``}`} </li>`;
  })}</ul> <span class="${["_jsonBkt svelte-ei2xnu", isArray ? "isArray" : ""].join(" ").trim()}" role="button" tabindex="0">${escape(brackets[1])}</span>${!_last ? `<span class="_jsonSep svelte-ei2xnu" data-svelte-h="svelte-1inngla">,</span>` : ``}`}`}`;
});
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = false } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      container,
      scale,
      min_width
    },
    {},
    {
      default: () => {
        return `${loading_status ? `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})}` : ``} ${validate_component(JsonView, "JsonView").$$render($$result, { json: value }, {}, {})}`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index42-3tDdUjqW.js.map
