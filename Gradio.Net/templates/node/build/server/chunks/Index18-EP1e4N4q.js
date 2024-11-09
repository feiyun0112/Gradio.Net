import { c as create_ssr_component, v as validate_component, e as escape, b as add_attribute } from './ssr-RaXq3SJh.js';
import { B as Block, N as BlockTitle, W as Calendar } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example-BOKdRpfa.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".label-content.svelte-15bft71{display:flex;justify-content:space-between;align-items:flex-start}button.svelte-15bft71{cursor:pointer;color:var(--body-text-color-subdued)}button.svelte-15bft71:hover{color:var(--body-text-color)}.svelte-15bft71::placeholder{color:var(--input-placeholder-color)}.timebox.svelte-15bft71{flex-grow:1;flex-shrink:1;display:flex;position:relative;background:var(--input-background-fill)}.timebox.svelte-15bft71 svg{height:18px}.time.svelte-15bft71{padding:var(--input-padding);color:var(--body-text-color);font-weight:var(--input-text-weight);font-size:var(--input-text-size);line-height:var(--line-sm);outline:none;flex-grow:1;background:none;border:var(--input-border-width) solid var(--input-border-color);border-right:none;border-top-left-radius:var(--input-radius);border-bottom-left-radius:var(--input-radius);box-shadow:var(--input-shadow)}.time.invalid.svelte-15bft71{color:var(--body-text-color-subdued)}.calendar.svelte-15bft71{display:inline-flex;justify-content:center;align-items:center;transition:var(--button-transition);box-shadow:var(--button-primary-shadow);text-align:center;background:var(--button-secondary-background-fill);color:var(--button-secondary-text-color);font-weight:var(--button-large-text-weight);font-size:var(--button-large-text-size);border-top-right-radius:var(--input-radius);border-bottom-right-radius:var(--input-radius);padding:var(--size-2);border:var(--input-border-width) solid var(--input-border-color)}.calendar.svelte-15bft71:hover{background:var(--button-secondary-background-fill-hover);box-shadow:var(--button-primary-shadow-hover)}.calendar.svelte-15bft71:active{box-shadow:var(--button-primary-shadow-active)}.datetime.svelte-15bft71{width:0px;padding:0;border:0;margin:0;background:none}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script context=\\"module\\" lang=\\"ts\\">export { default as BaseExample } from \\"./Example.svelte\\";\\n<\/script>\\n\\n<script lang=\\"ts\\">import { Block, BlockTitle } from \\"@gradio/atoms\\";\\nimport { Back, Calendar } from \\"@gradio/icons\\";\\nexport let gradio;\\nexport let label = \\"Time\\";\\nexport let show_label = true;\\nexport let info = void 0;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = \\"\\";\\nlet old_value = value;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let root;\\nexport let include_time = true;\\n$: if (value !== old_value) {\\n    old_value = value;\\n    entered_value = value;\\n    datevalue = value;\\n    gradio.dispatch(\\"change\\");\\n}\\nconst format_date = (date) => {\\n    if (date.toJSON() === null)\\n        return \\"\\";\\n    const pad = (num) => num.toString().padStart(2, \\"0\\");\\n    const year = date.getFullYear();\\n    const month = pad(date.getMonth() + 1);\\n    const day = pad(date.getDate());\\n    const hours = pad(date.getHours());\\n    const minutes = pad(date.getMinutes());\\n    const seconds = pad(date.getSeconds());\\n    const date_str = `${year}-${month}-${day}`;\\n    const time_str = `${hours}:${minutes}:${seconds}`;\\n    if (include_time) {\\n        return `${date_str} ${time_str}`;\\n    }\\n    return date_str;\\n};\\nlet entered_value = value;\\nlet datetime;\\nlet datevalue = value;\\nconst date_is_valid_format = (date) => {\\n    if (date === \\"\\")\\n        return false;\\n    const valid_regex = include_time ? /^\\\\d{4}-\\\\d{2}-\\\\d{2} \\\\d{2}:\\\\d{2}:\\\\d{2}$/ : /^\\\\d{4}-\\\\d{2}-\\\\d{2}$/;\\n    const is_valid_date = date.match(valid_regex) !== null;\\n    const is_valid_now = date.match(/^(?:\\\\s*now\\\\s*(?:-\\\\s*\\\\d+\\\\s*[dmhs])?)?\\\\s*$/) !== null;\\n    return is_valid_date || is_valid_now;\\n};\\n$: valid = date_is_valid_format(entered_value);\\nconst submit_values = () => {\\n    if (entered_value === value)\\n        return;\\n    if (!date_is_valid_format(entered_value))\\n        return;\\n    old_value = value = entered_value;\\n    gradio.dispatch(\\"change\\");\\n};\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{scale}\\n\\t{min_width}\\n\\tallow_overflow={false}\\n\\tpadding={true}\\n>\\n\\t<div class=\\"label-content\\">\\n\\t\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\t</div>\\n\\t<div class=\\"timebox\\">\\n\\t\\t<input\\n\\t\\t\\tclass=\\"time\\"\\n\\t\\t\\tbind:value={entered_value}\\n\\t\\t\\tclass:invalid={!valid}\\n\\t\\t\\ton:keydown={(evt) => {\\n\\t\\t\\t\\tif (evt.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\tsubmit_values();\\n\\t\\t\\t\\t\\tgradio.dispatch(\\"submit\\");\\n\\t\\t\\t\\t}\\n\\t\\t\\t}}\\n\\t\\t\\ton:blur={submit_values}\\n\\t\\t/>\\n\\t\\t{#if include_time}\\n\\t\\t\\t<input\\n\\t\\t\\t\\ttype=\\"datetime-local\\"\\n\\t\\t\\t\\tclass=\\"datetime\\"\\n\\t\\t\\t\\tstep=\\"1\\"\\n\\t\\t\\t\\tbind:this={datetime}\\n\\t\\t\\t\\tbind:value={datevalue}\\n\\t\\t\\t\\ton:input={() => {\\n\\t\\t\\t\\t\\tconst date = new Date(datevalue);\\n\\t\\t\\t\\t\\tentered_value = format_date(date);\\n\\t\\t\\t\\t\\tsubmit_values();\\n\\t\\t\\t\\t}}\\n\\t\\t\\t/>\\n\\t\\t{:else}\\n\\t\\t\\t<input\\n\\t\\t\\t\\ttype=\\"date\\"\\n\\t\\t\\t\\tclass=\\"datetime\\"\\n\\t\\t\\t\\tstep=\\"1\\"\\n\\t\\t\\t\\tbind:this={datetime}\\n\\t\\t\\t\\tbind:value={datevalue}\\n\\t\\t\\t\\ton:input={() => {\\n\\t\\t\\t\\t\\tconst date = new Date(datevalue + \\"T00:00:00\\");\\n\\t\\t\\t\\t\\tentered_value = format_date(date);\\n\\t\\t\\t\\t\\tsubmit_values();\\n\\t\\t\\t\\t}}\\n\\t\\t\\t/>\\n\\t\\t{/if}\\n\\n\\t\\t<button\\n\\t\\t\\tclass=\\"calendar\\"\\n\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\tdatetime.showPicker();\\n\\t\\t\\t}}><Calendar></Calendar></button\\n\\t\\t>\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\t.label-content {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: flex-start;\\n\\t}\\n\\tbutton {\\n\\t\\tcursor: pointer;\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\tbutton:hover {\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t::placeholder {\\n\\t\\tcolor: var(--input-placeholder-color);\\n\\t}\\n\\t.timebox {\\n\\t\\tflex-grow: 1;\\n\\t\\tflex-shrink: 1;\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tbackground: var(--input-background-fill);\\n\\t}\\n\\t.timebox :global(svg) {\\n\\t\\theight: 18px;\\n\\t}\\n\\t.time {\\n\\t\\tpadding: var(--input-padding);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-weight: var(--input-text-weight);\\n\\t\\tfont-size: var(--input-text-size);\\n\\t\\tline-height: var(--line-sm);\\n\\t\\toutline: none;\\n\\t\\tflex-grow: 1;\\n\\t\\tbackground: none;\\n\\t\\tborder: var(--input-border-width) solid var(--input-border-color);\\n\\t\\tborder-right: none;\\n\\t\\tborder-top-left-radius: var(--input-radius);\\n\\t\\tborder-bottom-left-radius: var(--input-radius);\\n\\t\\tbox-shadow: var(--input-shadow);\\n\\t}\\n\\t.time.invalid {\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\t.calendar {\\n\\t\\tdisplay: inline-flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tbox-shadow: var(--button-primary-shadow);\\n\\t\\ttext-align: center;\\n\\t\\tbackground: var(--button-secondary-background-fill);\\n\\t\\tcolor: var(--button-secondary-text-color);\\n\\t\\tfont-weight: var(--button-large-text-weight);\\n\\t\\tfont-size: var(--button-large-text-size);\\n\\t\\tborder-top-right-radius: var(--input-radius);\\n\\t\\tborder-bottom-right-radius: var(--input-radius);\\n\\t\\tpadding: var(--size-2);\\n\\t\\tborder: var(--input-border-width) solid var(--input-border-color);\\n\\t}\\n\\t.calendar:hover {\\n\\t\\tbackground: var(--button-secondary-background-fill-hover);\\n\\t\\tbox-shadow: var(--button-primary-shadow-hover);\\n\\t}\\n\\t.calendar:active {\\n\\t\\tbox-shadow: var(--button-primary-shadow-active);\\n\\t}\\n\\t.datetime {\\n\\t\\twidth: 0px;\\n\\t\\tpadding: 0;\\n\\t\\tborder: 0;\\n\\t\\tmargin: 0;\\n\\t\\tbackground: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AA8HC,6BAAe,CACd,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,WAAW,CAAE,UACd,CACA,qBAAO,CACN,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,yBAAyB,CACrC,CACA,qBAAM,MAAO,CACZ,KAAK,CAAE,IAAI,iBAAiB,CAC7B,gBAEA,aAAc,CACb,KAAK,CAAE,IAAI,yBAAyB,CACrC,CACA,uBAAS,CACR,SAAS,CAAE,CAAC,CACZ,WAAW,CAAE,CAAC,CACd,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,UAAU,CAAE,IAAI,uBAAuB,CACxC,CACA,uBAAQ,CAAS,GAAK,CACrB,MAAM,CAAE,IACT,CACA,oBAAM,CACL,OAAO,CAAE,IAAI,eAAe,CAAC,CAC7B,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,WAAW,CAAE,IAAI,mBAAmB,CAAC,CACrC,SAAS,CAAE,IAAI,iBAAiB,CAAC,CACjC,WAAW,CAAE,IAAI,SAAS,CAAC,CAC3B,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,CAAC,CACZ,UAAU,CAAE,IAAI,CAChB,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CACjE,YAAY,CAAE,IAAI,CAClB,sBAAsB,CAAE,IAAI,cAAc,CAAC,CAC3C,yBAAyB,CAAE,IAAI,cAAc,CAAC,CAC9C,UAAU,CAAE,IAAI,cAAc,CAC/B,CACA,KAAK,uBAAS,CACb,KAAK,CAAE,IAAI,yBAAyB,CACrC,CACA,wBAAU,CACT,OAAO,CAAE,WAAW,CACpB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,UAAU,CAAE,MAAM,CAClB,UAAU,CAAE,IAAI,kCAAkC,CAAC,CACnD,KAAK,CAAE,IAAI,6BAA6B,CAAC,CACzC,WAAW,CAAE,IAAI,0BAA0B,CAAC,CAC5C,SAAS,CAAE,IAAI,wBAAwB,CAAC,CACxC,uBAAuB,CAAE,IAAI,cAAc,CAAC,CAC5C,0BAA0B,CAAE,IAAI,cAAc,CAAC,CAC/C,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,oBAAoB,CACjE,CACA,wBAAS,MAAO,CACf,UAAU,CAAE,IAAI,wCAAwC,CAAC,CACzD,UAAU,CAAE,IAAI,6BAA6B,CAC9C,CACA,wBAAS,OAAQ,CAChB,UAAU,CAAE,IAAI,8BAA8B,CAC/C,CACA,wBAAU,CACT,KAAK,CAAE,GAAG,CACV,OAAO,CAAE,CAAC,CACV,MAAM,CAAE,CAAC,CACT,MAAM,CAAE,CAAC,CACT,UAAU,CAAE,IACb"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let valid;
  let { gradio } = $$props;
  let { label = "Time" } = $$props;
  let { show_label = true } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = "" } = $$props;
  let old_value = value;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { root } = $$props;
  let { include_time = true } = $$props;
  let entered_value = value;
  let datetime;
  let datevalue = value;
  const date_is_valid_format = (date) => {
    if (date === "")
      return false;
    const valid_regex = include_time ? /^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$/ : /^\d{4}-\d{2}-\d{2}$/;
    const is_valid_date = date.match(valid_regex) !== null;
    const is_valid_now = date.match(/^(?:\s*now\s*(?:-\s*\d+\s*[dmhs])?)?\s*$/) !== null;
    return is_valid_date || is_valid_now;
  };
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.include_time === void 0 && $$bindings.include_time && include_time !== void 0)
    $$bindings.include_time(include_time);
  $$result.css.add(css);
  {
    if (value !== old_value) {
      old_value = value;
      entered_value = value;
      datevalue = value;
      gradio.dispatch("change");
    }
  }
  valid = date_is_valid_format(entered_value);
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      scale,
      min_width,
      allow_overflow: false,
      padding: true
    },
    {},
    {
      default: () => {
        return `<div class="label-content svelte-15bft71">${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
          default: () => {
            return `${escape(label)}`;
          }
        })}</div> <div class="timebox svelte-15bft71"><input class="${["time svelte-15bft71", !valid ? "invalid" : ""].join(" ").trim()}"${add_attribute("value", entered_value, 0)}> ${include_time ? `<input type="datetime-local" class="datetime svelte-15bft71" step="1"${add_attribute("this", datetime, 0)}${add_attribute("value", datevalue, 0)}>` : `<input type="date" class="datetime svelte-15bft71" step="1"${add_attribute("this", datetime, 0)}${add_attribute("value", datevalue, 0)}>`} <button class="calendar svelte-15bft71">${validate_component(Calendar, "Calendar").$$render($$result, {}, {}, {})}</button></div>`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index18-EP1e4N4q.js.map
