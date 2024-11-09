import { t as tick } from './Component-Dv7eSVA_.js';
import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, e as escape, d as add_styles, b as add_attribute, f as each } from './ssr-RaXq3SJh.js';
import { t as tinycolor } from './tinycolor-LGDJOiKq.js';
import { B as Block, S as Static, N as BlockTitle } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example7-DJ2n4yYp.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';

function format_color(color, mode) {
  if (mode === "hex") {
    return tinycolor(color).toHexString();
  } else if (mode === "rgb") {
    return tinycolor(color).toRgbString();
  }
  return tinycolor(color).toHslString();
}
const css = {
  code: ".dialog-button.svelte-1oxhzww.svelte-1oxhzww{display:block;width:var(--size-10);height:var(--size-5);border:var(--block-border-width) solid var(--block-border-color)}.dialog-button.svelte-1oxhzww.svelte-1oxhzww:disabled{cursor:not-allowed}.input.svelte-1oxhzww.svelte-1oxhzww{display:flex;align-items:center;padding:0 10px 15px}.input.svelte-1oxhzww input.svelte-1oxhzww{height:30px;width:100%;flex-shrink:1;border-bottom-left-radius:0;border:1px solid var(--block-border-color);letter-spacing:-0.05rem;border-left:none;border-right:none;font-family:var(--font-mono);font-size:var(--scale-000);padding-left:15px;padding-right:0;background-color:var(--background-fill-secondary);color:var(--block-label-text-color)}.swatch.svelte-1oxhzww.svelte-1oxhzww{width:50px;height:50px;border-top-left-radius:15px;border-bottom-left-radius:15px;flex-shrink:0;border:1px solid var(--block-border-color)}.color-picker.svelte-1oxhzww.svelte-1oxhzww{width:230px;background:var(--background-fill-secondary);border:1px solid var(--block-border-color);border-radius:var(--block-radius);margin:var(--spacing-sm) 0}.buttons.svelte-1oxhzww.svelte-1oxhzww{height:20px;display:flex;justify-content:stretch;gap:0px}.buttons.svelte-1oxhzww button.svelte-1oxhzww{display:flex;align-items:center;justify-content:center;border:1px solid var(--block-border-color);background:var(--background-fill-secondary);padding:3px 6px;font-size:var(--scale-000);cursor:pointer;border-right:none;width:100%;border-top:none}.buttons.svelte-1oxhzww button.svelte-1oxhzww:first-child{border-left:none}.buttons.svelte-1oxhzww button.svelte-1oxhzww:last-child{border-bottom-right-radius:15px;border-right:1px solid var(--block-border-color)}.buttons.svelte-1oxhzww button.svelte-1oxhzww:hover{background:var(--background-fill-secondary-hover);font-weight:var(--weight-bold)}.buttons.svelte-1oxhzww button.active.svelte-1oxhzww{background:var(--background-fill-secondary);font-weight:var(--weight-bold)}.input-wrap.svelte-1oxhzww.svelte-1oxhzww{display:flex}.color-gradient.svelte-1oxhzww.svelte-1oxhzww{position:relative;--hue:white;background:linear-gradient(rgba(0, 0, 0, 0), #000),\n			linear-gradient(90deg, #fff, hsl(var(--hue), 100%, 50%));width:100%;height:150px;border-radius:var(--radius-sm) var(--radius-sm) 0 0}.hue-slider.svelte-1oxhzww.svelte-1oxhzww{position:relative;width:90%;margin:10px auto;height:10px;border-radius:5px;background:linear-gradient(\n			to right,\n			hsl(0, 100%, 50%) 0%,\n			#ff0 17%,\n			lime 33%,\n			cyan 50%,\n			blue 67%,\n			magenta 83%,\n			red 100%\n		)}.swatch.svelte-1oxhzww.svelte-1oxhzww{width:50px;height:50px;border-top-left-radius:15px;border-bottom-left-radius:15px;flex-shrink:0;border:1px solid var(--block-border-color)}.eyedropper.svelte-1oxhzww.svelte-1oxhzww{display:flex;align-items:center;justify-content:center;width:25px;height:30px;border-top-right-radius:15px;border:1px solid var(--block-border-color);border-left:none;background:var(--background-fill-secondary);height:30px;padding:7px 7px 5px 0px;cursor:pointer}.marker.svelte-1oxhzww.svelte-1oxhzww{position:absolute;width:14px;height:14px;border-radius:50%;border:2px solid white;top:-2px;left:-7px;box-shadow:0 1px 5px rgba(0, 0, 0, 0.1);pointer-events:none}input.svelte-1oxhzww.svelte-1oxhzww{width:100%;height:30px;border:1px solid var(--block-border-color);border-radius:var(--radius-sm);padding:0 var(--size-2);font-family:var(--font-mono);font-size:var(--scale-000);color:var(--block-label-text-color);background-color:var(--background-fill-primary)}",
  map: '{"version":3,"file":"Colorpicker.svelte","sources":["Colorpicker.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher, afterUpdate, onMount, tick } from \\"svelte\\";\\nimport tinycolor from \\"tinycolor2\\";\\nimport { BlockTitle } from \\"@gradio/atoms\\";\\nimport { click_outside } from \\"./events\\";\\nimport { Eyedropper } from \\"@gradio/icons\\";\\nimport { hsva_to_rgba, format_color } from \\"./utils\\";\\nexport let value = \\"#000000\\";\\nexport let value_is_output = false;\\nexport let label;\\nexport let info = void 0;\\nexport let disabled = false;\\nexport let show_label = true;\\nexport let root;\\nexport let current_mode = \\"hex\\";\\nexport let dialog_open = false;\\nlet eyedropper_supported = false;\\nlet sl_wrap;\\nlet hue_wrap;\\nconst dispatch = createEventDispatcher();\\nlet sl_marker_pos = [0, 0];\\nlet sl_rect = null;\\nlet sl_moving = false;\\nlet sl = [0, 0];\\nlet hue = 0;\\nlet hue_marker_pos = 0;\\nlet hue_rect = null;\\nlet hue_moving = false;\\nfunction handle_hue_down(event) {\\n    hue_rect = event.currentTarget.getBoundingClientRect();\\n    hue_moving = true;\\n    update_hue_from_mouse(event.clientX);\\n}\\nfunction update_hue_from_mouse(x) {\\n    if (!hue_rect)\\n        return;\\n    const _x = Math.max(0, Math.min(x - hue_rect.left, hue_rect.width));\\n    hue_marker_pos = _x;\\n    const _hue = _x / hue_rect.width * 360;\\n    hue = _hue;\\n    value = hsva_to_rgba({ h: _hue, s: sl[0], v: sl[1], a: 1 });\\n}\\nfunction update_color_from_mouse(x, y) {\\n    if (!sl_rect)\\n        return;\\n    const _x = Math.max(0, Math.min(x - sl_rect.left, sl_rect.width));\\n    const _y = Math.max(0, Math.min(y - sl_rect.top, sl_rect.height));\\n    sl_marker_pos = [_x, _y];\\n    const _hsva = {\\n        h: hue * 1,\\n        s: _x / sl_rect.width,\\n        v: 1 - _y / sl_rect.height,\\n        a: 1\\n    };\\n    sl = [_hsva.s, _hsva.v];\\n    value = hsva_to_rgba(_hsva);\\n}\\nfunction handle_sl_down(event) {\\n    sl_moving = true;\\n    sl_rect = event.currentTarget.getBoundingClientRect();\\n    update_color_from_mouse(event.clientX, event.clientY);\\n}\\nfunction handle_move(event) {\\n    if (sl_moving)\\n        update_color_from_mouse(event.clientX, event.clientY);\\n    if (hue_moving)\\n        update_hue_from_mouse(event.clientX);\\n}\\nfunction handle_end() {\\n    sl_moving = false;\\n    hue_moving = false;\\n}\\nasync function update_mouse_from_color(color) {\\n    if (sl_moving || hue_moving)\\n        return;\\n    await tick();\\n    if (!color)\\n        return;\\n    if (!sl_rect && sl_wrap) {\\n        sl_rect = sl_wrap.getBoundingClientRect();\\n    }\\n    if (!hue_rect && hue_wrap) {\\n        hue_rect = hue_wrap.getBoundingClientRect();\\n    }\\n    if (!sl_rect || !hue_rect)\\n        return;\\n    const hsva = tinycolor(color).toHsv();\\n    const _x = hsva.s * sl_rect.width;\\n    const _y = (1 - hsva.v) * sl_rect.height;\\n    sl_marker_pos = [_x, _y];\\n    sl = [hsva.s, hsva.v];\\n    hue = hsva.h;\\n    hue_marker_pos = hsva.h / 360 * hue_rect.width;\\n}\\nfunction request_eyedropper() {\\n    const eyeDropper = new EyeDropper();\\n    eyeDropper.open().then((result) => {\\n        value = result.sRGBHex;\\n    });\\n}\\nconst modes = [\\n    [\\"Hex\\", \\"hex\\"],\\n    [\\"RGB\\", \\"rgb\\"],\\n    [\\"HSL\\", \\"hsl\\"]\\n];\\n$: color_string = format_color(value, current_mode);\\n$: color_string && dispatch(\\"selected\\", color_string);\\nonMount(async () => {\\n    eyedropper_supported = window !== void 0 && !!window.EyeDropper;\\n});\\nfunction handle_click_outside() {\\n    dialog_open = false;\\n}\\nfunction handle_change() {\\n    dispatch(\\"change\\", value);\\n    if (!value_is_output) {\\n        dispatch(\\"input\\");\\n    }\\n}\\nafterUpdate(() => {\\n    value_is_output = false;\\n});\\n$: update_mouse_from_color(value);\\n$: value, handle_change();\\nfunction handle_click() {\\n    dispatch(\\"selected\\", color_string);\\n    dispatch(\\"close\\");\\n}\\n<\/script>\\n\\n<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n<button\\n\\tclass=\\"dialog-button\\"\\n\\tstyle:background={value}\\n\\t{disabled}\\n\\ton:click={() => {\\n\\t\\tupdate_mouse_from_color(value);\\n\\t\\tdialog_open = !dialog_open;\\n\\t}}\\n/>\\n\\n<svelte:window on:mousemove={handle_move} on:mouseup={handle_end} />\\n\\n{#if dialog_open}\\n\\t<div\\n\\t\\tclass=\\"color-picker\\"\\n\\t\\ton:focus\\n\\t\\ton:blur\\n\\t\\tuse:click_outside={handle_click_outside}\\n\\t>\\n\\t\\t<!-- svelte-ignore a11y-no-static-element-interactions -->\\n\\t\\t<div\\n\\t\\t\\tclass=\\"color-gradient\\"\\n\\t\\t\\ton:mousedown={handle_sl_down}\\n\\t\\t\\tstyle=\\"--hue:{hue}\\"\\n\\t\\t\\tbind:this={sl_wrap}\\n\\t\\t>\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"marker\\"\\n\\t\\t\\t\\tstyle:transform=\\"translate({sl_marker_pos[0]}px,{sl_marker_pos[1]}px)\\"\\n\\t\\t\\t\\tstyle:background={value}\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\t\\t<!-- svelte-ignore a11y-no-static-element-interactions -->\\n\\t\\t<div class=\\"hue-slider\\" on:mousedown={handle_hue_down} bind:this={hue_wrap}>\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"marker\\"\\n\\t\\t\\t\\tstyle:background={\\"hsl(\\" + hue + \\", 100%, 50%)\\"}\\n\\t\\t\\t\\tstyle:transform=\\"translateX({hue_marker_pos}px)\\"\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\n\\t\\t<div class=\\"input\\">\\n\\t\\t\\t<button class=\\"swatch\\" style:background={value} on:click={handle_click}\\n\\t\\t\\t></button>\\n\\t\\t\\t<div>\\n\\t\\t\\t\\t<div class=\\"input-wrap\\">\\n\\t\\t\\t\\t\\t<input\\n\\t\\t\\t\\t\\t\\ttype=\\"text\\"\\n\\t\\t\\t\\t\\t\\tbind:value={color_string}\\n\\t\\t\\t\\t\\t\\ton:change={(e) => (value = e.currentTarget.value)}\\n\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t<button class=\\"eyedropper\\" on:click={request_eyedropper}>\\n\\t\\t\\t\\t\\t\\t{#if eyedropper_supported}\\n\\t\\t\\t\\t\\t\\t\\t<Eyedropper />\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t</div>\\n\\n\\t\\t\\t\\t<div class=\\"buttons\\">\\n\\t\\t\\t\\t\\t{#each modes as [label, value]}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\tclass:active={current_mode === value}\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => (current_mode = value)}>{label}</button\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.dialog-button {\\n\\t\\tdisplay: block;\\n\\t\\twidth: var(--size-10);\\n\\t\\theight: var(--size-5);\\n\\t\\tborder: var(--block-border-width) solid var(--block-border-color);\\n\\t}\\n\\n\\t.dialog-button:disabled {\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\t.input {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tpadding: 0 10px 15px;\\n\\t}\\n\\n\\t.input input {\\n\\t\\theight: 30px;\\n\\t\\twidth: 100%;\\n\\t\\tflex-shrink: 1;\\n\\t\\tborder-bottom-left-radius: 0;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tletter-spacing: -0.05rem;\\n\\t\\tborder-left: none;\\n\\t\\tborder-right: none;\\n\\t\\tfont-family: var(--font-mono);\\n\\t\\tfont-size: var(--scale-000);\\n\\t\\tpadding-left: 15px;\\n\\t\\tpadding-right: 0;\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t}\\n\\n\\t.swatch {\\n\\t\\twidth: 50px;\\n\\t\\theight: 50px;\\n\\t\\tborder-top-left-radius: 15px;\\n\\t\\tborder-bottom-left-radius: 15px;\\n\\t\\tflex-shrink: 0;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t}\\n\\n\\t.color-picker {\\n\\t\\twidth: 230px;\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tborder-radius: var(--block-radius);\\n\\t\\tmargin: var(--spacing-sm) 0;\\n\\t}\\n\\n\\t.buttons {\\n\\t\\theight: 20px;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: stretch;\\n\\t\\tgap: 0px;\\n\\t}\\n\\n\\t.buttons button {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\tpadding: 3px 6px;\\n\\t\\tfont-size: var(--scale-000);\\n\\t\\tcursor: pointer;\\n\\t\\tborder-right: none;\\n\\t\\twidth: 100%;\\n\\t\\tborder-top: none;\\n\\t}\\n\\n\\t.buttons button:first-child {\\n\\t\\tborder-left: none;\\n\\t}\\n\\n\\t.buttons button:last-child {\\n\\t\\tborder-bottom-right-radius: 15px;\\n\\t\\tborder-right: 1px solid var(--block-border-color);\\n\\t}\\n\\n\\t.buttons button:hover {\\n\\t\\tbackground: var(--background-fill-secondary-hover);\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t}\\n\\n\\t.buttons button.active {\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t}\\n\\n\\t.input-wrap {\\n\\t\\tdisplay: flex;\\n\\t}\\n\\n\\t.color-gradient {\\n\\t\\tposition: relative;\\n\\t\\t--hue: white;\\n\\t\\tbackground: linear-gradient(rgba(0, 0, 0, 0), #000),\\n\\t\\t\\tlinear-gradient(90deg, #fff, hsl(var(--hue), 100%, 50%));\\n\\t\\twidth: 100%;\\n\\t\\theight: 150px;\\n\\t\\tborder-radius: var(--radius-sm) var(--radius-sm) 0 0;\\n\\t}\\n\\n\\t.hue-slider {\\n\\t\\tposition: relative;\\n\\t\\twidth: 90%;\\n\\t\\tmargin: 10px auto;\\n\\t\\theight: 10px;\\n\\t\\tborder-radius: 5px;\\n\\t\\tbackground: linear-gradient(\\n\\t\\t\\tto right,\\n\\t\\t\\thsl(0, 100%, 50%) 0%,\\n\\t\\t\\t#ff0 17%,\\n\\t\\t\\tlime 33%,\\n\\t\\t\\tcyan 50%,\\n\\t\\t\\tblue 67%,\\n\\t\\t\\tmagenta 83%,\\n\\t\\t\\tred 100%\\n\\t\\t);\\n\\t}\\n\\n\\t.swatch {\\n\\t\\twidth: 50px;\\n\\t\\theight: 50px;\\n\\t\\tborder-top-left-radius: 15px;\\n\\t\\tborder-bottom-left-radius: 15px;\\n\\t\\tflex-shrink: 0;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t}\\n\\n\\t.eyedropper {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\twidth: 25px;\\n\\t\\theight: 30px;\\n\\t\\tborder-top-right-radius: 15px;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tborder-left: none;\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\theight: 30px;\\n\\t\\tpadding: 7px 7px 5px 0px;\\n\\t\\tcursor: pointer;\\n\\t}\\n\\n\\t.marker {\\n\\t\\tposition: absolute;\\n\\t\\twidth: 14px;\\n\\t\\theight: 14px;\\n\\t\\tborder-radius: 50%;\\n\\t\\tborder: 2px solid white;\\n\\t\\ttop: -2px;\\n\\t\\tleft: -7px;\\n\\t\\tbox-shadow: 0 1px 5px rgba(0, 0, 0, 0.1);\\n\\t\\tpointer-events: none;\\n\\t}\\n\\n\\tinput {\\n\\t\\twidth: 100%;\\n\\t\\theight: 30px;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tpadding: 0 var(--size-2);\\n\\t\\tfont-family: var(--font-mono);\\n\\t\\tfont-size: var(--scale-000);\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t\\tbackground-color: var(--background-fill-primary);\\n\\t}</style>\\n"],"names":[],"mappings":"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"}'
};
const Colorpicker = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let color_string;
  let { value = "#000000" } = $$props;
  let { value_is_output = false } = $$props;
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { disabled = false } = $$props;
  let { show_label = true } = $$props;
  let { root } = $$props;
  let { current_mode = "hex" } = $$props;
  let { dialog_open = false } = $$props;
  let sl_wrap;
  let hue_wrap;
  const dispatch = createEventDispatcher();
  let sl_marker_pos = [0, 0];
  let hue = 0;
  let hue_marker_pos = 0;
  async function update_mouse_from_color(color) {
    await tick();
    if (!color)
      return;
    return;
  }
  const modes = [["Hex", "hex"], ["RGB", "rgb"], ["HSL", "hsl"]];
  function handle_change() {
    dispatch("change", value);
    if (!value_is_output) {
      dispatch("input");
    }
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.current_mode === void 0 && $$bindings.current_mode && current_mode !== void 0)
    $$bindings.current_mode(current_mode);
  if ($$props.dialog_open === void 0 && $$bindings.dialog_open && dialog_open !== void 0)
    $$bindings.dialog_open(dialog_open);
  $$result.css.add(css);
  color_string = format_color(value, current_mode);
  color_string && dispatch("selected", color_string);
  {
    update_mouse_from_color(value);
  }
  {
    handle_change();
  }
  return `${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
    default: () => {
      return `${escape(label)}`;
    }
  })} <button class="dialog-button svelte-1oxhzww" ${disabled ? "disabled" : ""}${add_styles({ "background": value })}></button>  ${dialog_open ? `<div class="color-picker svelte-1oxhzww"> <div class="color-gradient svelte-1oxhzww" style="${"--hue:" + escape(hue, true)}"${add_attribute("this", sl_wrap, 0)}><div class="marker svelte-1oxhzww"${add_styles({
    "transform": `translate(${sl_marker_pos[0]}px,${sl_marker_pos[1]}px)`,
    "background": value
  })}></div></div>  <div class="hue-slider svelte-1oxhzww"${add_attribute("this", hue_wrap, 0)}><div class="marker svelte-1oxhzww"${add_styles({
    "background": "hsl(" + hue + ", 100%, 50%)",
    "transform": `translateX(${hue_marker_pos}px)`
  })}></div></div> <div class="input svelte-1oxhzww"><button class="swatch svelte-1oxhzww"${add_styles({ "background": value })}></button> <div><div class="input-wrap svelte-1oxhzww"><input type="text" class="svelte-1oxhzww"${add_attribute("value", color_string, 0)}> <button class="eyedropper svelte-1oxhzww">${``}</button></div> <div class="buttons svelte-1oxhzww">${each(modes, ([label2, value2]) => {
    return `<button class="${["button svelte-1oxhzww", current_mode === value2 ? "active" : ""].join(" ").trim()}">${escape(label2)}</button>`;
  })}</div></div></div></div>` : ``}`;
});
const Colorpicker$1 = Colorpicker;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label = "ColorPicker" } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { value_is_output = false } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { root } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
  let { disabled = false } = $$props;
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
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
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `   ${validate_component(Block, "Block").$$render(
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
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(Colorpicker$1, "Colorpicker").$$render(
            $$result,
            {
              root,
              label,
              info,
              show_label,
              disabled: !interactive || disabled,
              value,
              value_is_output
            },
            {
              value: ($$value) => {
                value = $$value;
                $$settled = false;
              },
              value_is_output: ($$value) => {
                value_is_output = $$value;
                $$settled = false;
              }
            },
            {}
          )}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { Colorpicker$1 as BaseColorPicker, Index as default };
//# sourceMappingURL=Index43-C6_gR61P.js.map
