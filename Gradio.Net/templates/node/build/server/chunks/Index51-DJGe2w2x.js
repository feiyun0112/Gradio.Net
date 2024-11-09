import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, e as escape, f as each, b as add_attribute, d as add_styles } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, N as BlockTitle, _ as Remove, a0 as DropdownArrow } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example10-SA_UoMlr.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$2 = {
  code: ".options.svelte-y6qw75{--window-padding:var(--size-8);position:fixed;z-index:var(--layer-top);margin-left:0;box-shadow:var(--shadow-drop-lg);border-radius:var(--container-radius);background:var(--background-fill-primary);min-width:fit-content;max-width:inherit;overflow:auto;color:var(--body-text-color);list-style:none}.item.svelte-y6qw75{display:flex;cursor:pointer;padding:var(--size-2);word-break:break-word}.item.svelte-y6qw75:hover,.active.svelte-y6qw75{background:var(--background-fill-secondary)}.inner-item.svelte-y6qw75{padding-right:var(--size-1)}.hide.svelte-y6qw75{visibility:hidden}",
  map: '{"version":3,"file":"DropdownOptions.svelte","sources":["DropdownOptions.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { fly } from \\"svelte/transition\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nexport let choices;\\nexport let filtered_indices;\\nexport let show_options = false;\\nexport let disabled = false;\\nexport let selected_indices = [];\\nexport let active_index = null;\\nlet distance_from_top;\\nlet distance_from_bottom;\\nlet input_height;\\nlet input_width;\\nlet refElement;\\nlet listElement;\\nlet top, bottom, max_height;\\nlet innerHeight;\\nfunction calculate_window_distance() {\\n    const { top: ref_top, bottom: ref_bottom } = refElement.getBoundingClientRect();\\n    distance_from_top = ref_top;\\n    distance_from_bottom = innerHeight - ref_bottom;\\n}\\nlet scroll_timeout = null;\\nfunction scroll_listener() {\\n    if (!show_options)\\n        return;\\n    if (scroll_timeout !== null) {\\n        clearTimeout(scroll_timeout);\\n    }\\n    scroll_timeout = setTimeout(() => {\\n        calculate_window_distance();\\n        scroll_timeout = null;\\n    }, 10);\\n}\\n$: {\\n    if (show_options && refElement) {\\n        if (listElement && selected_indices.length > 0) {\\n            let elements = listElement.querySelectorAll(\\"li\\");\\n            for (const element of Array.from(elements)) {\\n                if (element.getAttribute(\\"data-index\\") === selected_indices[0].toString()) {\\n                    listElement?.scrollTo?.(0, element.offsetTop);\\n                    break;\\n                }\\n            }\\n        }\\n        calculate_window_distance();\\n        const rect = refElement.parentElement?.getBoundingClientRect();\\n        input_height = rect?.height || 0;\\n        input_width = rect?.width || 0;\\n    }\\n    if (distance_from_bottom > distance_from_top) {\\n        top = `${distance_from_top}px`;\\n        max_height = distance_from_bottom;\\n        bottom = null;\\n    }\\n    else {\\n        bottom = `${distance_from_bottom + input_height}px`;\\n        max_height = distance_from_top - input_height;\\n        top = null;\\n    }\\n}\\nconst dispatch = createEventDispatcher();\\n<\/script>\\n\\n<svelte:window on:scroll={scroll_listener} bind:innerHeight />\\n\\n<div class=\\"reference\\" bind:this={refElement} />\\n{#if show_options && !disabled}\\n\\t<ul\\n\\t\\tclass=\\"options\\"\\n\\t\\ttransition:fly={{ duration: 200, y: 5 }}\\n\\t\\ton:mousedown|preventDefault={(e) => dispatch(\\"change\\", e)}\\n\\t\\tstyle:top\\n\\t\\tstyle:bottom\\n\\t\\tstyle:max-height={`calc(${max_height}px - var(--window-padding))`}\\n\\t\\tstyle:width={input_width + \\"px\\"}\\n\\t\\tbind:this={listElement}\\n\\t\\trole=\\"listbox\\"\\n\\t>\\n\\t\\t{#each filtered_indices as index}\\n\\t\\t\\t<li\\n\\t\\t\\t\\tclass=\\"item\\"\\n\\t\\t\\t\\tclass:selected={selected_indices.includes(index)}\\n\\t\\t\\t\\tclass:active={index === active_index}\\n\\t\\t\\t\\tclass:bg-gray-100={index === active_index}\\n\\t\\t\\t\\tclass:dark:bg-gray-600={index === active_index}\\n\\t\\t\\t\\tstyle:width={input_width + \\"px\\"}\\n\\t\\t\\t\\tdata-index={index}\\n\\t\\t\\t\\taria-label={choices[index][0]}\\n\\t\\t\\t\\tdata-testid=\\"dropdown-option\\"\\n\\t\\t\\t\\trole=\\"option\\"\\n\\t\\t\\t\\taria-selected={selected_indices.includes(index)}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<span class:hide={!selected_indices.includes(index)} class=\\"inner-item\\">\\n\\t\\t\\t\\t\\t✓\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t{choices[index][0]}\\n\\t\\t\\t</li>\\n\\t\\t{/each}\\n\\t</ul>\\n{/if}\\n\\n<style>\\n\\t.options {\\n\\t\\t--window-padding: var(--size-8);\\n\\t\\tposition: fixed;\\n\\t\\tz-index: var(--layer-top);\\n\\t\\tmargin-left: 0;\\n\\t\\tbox-shadow: var(--shadow-drop-lg);\\n\\t\\tborder-radius: var(--container-radius);\\n\\t\\tbackground: var(--background-fill-primary);\\n\\t\\tmin-width: fit-content;\\n\\t\\tmax-width: inherit;\\n\\t\\toverflow: auto;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tlist-style: none;\\n\\t}\\n\\n\\t.item {\\n\\t\\tdisplay: flex;\\n\\t\\tcursor: pointer;\\n\\t\\tpadding: var(--size-2);\\n\\t\\tword-break: break-word;\\n\\t}\\n\\n\\t.item:hover,\\n\\t.active {\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t}\\n\\n\\t.inner-item {\\n\\t\\tpadding-right: var(--size-1);\\n\\t}\\n\\n\\t.hide {\\n\\t\\tvisibility: hidden;\\n\\t}</style>\\n"],"names":[],"mappings":"AAsGC,sBAAS,CACR,gBAAgB,CAAE,aAAa,CAC/B,QAAQ,CAAE,KAAK,CACf,OAAO,CAAE,IAAI,WAAW,CAAC,CACzB,WAAW,CAAE,CAAC,CACd,UAAU,CAAE,IAAI,gBAAgB,CAAC,CACjC,aAAa,CAAE,IAAI,kBAAkB,CAAC,CACtC,UAAU,CAAE,IAAI,yBAAyB,CAAC,CAC1C,SAAS,CAAE,WAAW,CACtB,SAAS,CAAE,OAAO,CAClB,QAAQ,CAAE,IAAI,CACd,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,UAAU,CAAE,IACb,CAEA,mBAAM,CACL,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,OAAO,CACf,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,UAAU,CAAE,UACb,CAEA,mBAAK,MAAM,CACX,qBAAQ,CACP,UAAU,CAAE,IAAI,2BAA2B,CAC5C,CAEA,yBAAY,CACX,aAAa,CAAE,IAAI,QAAQ,CAC5B,CAEA,mBAAM,CACL,UAAU,CAAE,MACb"}'
};
const DropdownOptions = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { choices } = $$props;
  let { filtered_indices } = $$props;
  let { show_options = false } = $$props;
  let { disabled = false } = $$props;
  let { selected_indices = [] } = $$props;
  let { active_index = null } = $$props;
  let distance_from_top;
  let distance_from_bottom;
  let input_height;
  let input_width;
  let refElement;
  let listElement;
  let top, bottom, max_height;
  let innerHeight;
  function calculate_window_distance() {
    const { top: ref_top, bottom: ref_bottom } = refElement.getBoundingClientRect();
    distance_from_top = ref_top;
    distance_from_bottom = innerHeight - ref_bottom;
  }
  createEventDispatcher();
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  if ($$props.filtered_indices === void 0 && $$bindings.filtered_indices && filtered_indices !== void 0)
    $$bindings.filtered_indices(filtered_indices);
  if ($$props.show_options === void 0 && $$bindings.show_options && show_options !== void 0)
    $$bindings.show_options(show_options);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.selected_indices === void 0 && $$bindings.selected_indices && selected_indices !== void 0)
    $$bindings.selected_indices(selected_indices);
  if ($$props.active_index === void 0 && $$bindings.active_index && active_index !== void 0)
    $$bindings.active_index(active_index);
  $$result.css.add(css$2);
  {
    {
      if (show_options && refElement) {
        calculate_window_distance();
        input_height = 0;
        input_width = 0;
      }
      if (distance_from_bottom > distance_from_top) {
        top = `${distance_from_top}px`;
        max_height = distance_from_bottom;
        bottom = null;
      } else {
        bottom = `${distance_from_bottom + input_height}px`;
        max_height = distance_from_top - input_height;
        top = null;
      }
    }
  }
  return ` <div class="reference"${add_attribute("this", refElement, 0)}></div> ${show_options && !disabled ? `<ul class="options svelte-y6qw75" role="listbox"${add_styles({
    top,
    bottom,
    "max-height": `calc(${max_height}px - var(--window-padding))`,
    "width": input_width + "px"
  })}${add_attribute("this", listElement, 0)}>${each(filtered_indices, (index) => {
    return `<li class="${[
      "item svelte-y6qw75",
      (selected_indices.includes(index) ? "selected" : "") + " " + (index === active_index ? "active" : "") + " " + (index === active_index ? "bg-gray-100" : "") + " " + (index === active_index ? "dark:bg-gray-600" : "")
    ].join(" ").trim()}"${add_attribute("data-index", index, 0)}${add_attribute("aria-label", choices[index][0], 0)} data-testid="dropdown-option" role="option"${add_attribute("aria-selected", selected_indices.includes(index), 0)}${add_styles({ "width": input_width + "px" })}><span class="${[
      "inner-item svelte-y6qw75",
      !selected_indices.includes(index) ? "hide" : ""
    ].join(" ").trim()}" data-svelte-h="svelte-1id9b8g">✓</span> ${escape(choices[index][0])} </li>`;
  })}</ul>` : ``}`;
});
function handle_filter(choices, input_text) {
  return choices.reduce((filtered_indices, o, index) => {
    if (input_text ? o[0].toLowerCase().includes(input_text.toLowerCase()) : true) {
      filtered_indices.push(index);
    }
    return filtered_indices;
  }, []);
}
function handle_change(dispatch, value, value_is_output) {
  dispatch("change", value);
  if (!value_is_output) {
    dispatch("input");
  }
}
const css$1 = {
  code: ".icon-wrap.svelte-1scun43.svelte-1scun43.svelte-1scun43{color:var(--body-text-color);margin-right:var(--size-2);width:var(--size-5)}label.svelte-1scun43.svelte-1scun43.svelte-1scun43:not(.container),label.svelte-1scun43:not(.container) .wrap.svelte-1scun43.svelte-1scun43,label.svelte-1scun43:not(.container) .wrap-inner.svelte-1scun43.svelte-1scun43,label.svelte-1scun43:not(.container) .secondary-wrap.svelte-1scun43.svelte-1scun43,label.svelte-1scun43:not(.container) .token.svelte-1scun43.svelte-1scun43,label.svelte-1scun43:not(.container) input.svelte-1scun43.svelte-1scun43{height:100%}.container.svelte-1scun43 .wrap.svelte-1scun43.svelte-1scun43{box-shadow:var(--input-shadow);border:var(--input-border-width) solid var(--border-color-primary)}.wrap.svelte-1scun43.svelte-1scun43.svelte-1scun43{position:relative;border-radius:var(--input-radius);background:var(--input-background-fill)}.wrap.svelte-1scun43.svelte-1scun43.svelte-1scun43:focus-within{box-shadow:var(--input-shadow-focus);border-color:var(--input-border-color-focus)}.wrap-inner.svelte-1scun43.svelte-1scun43.svelte-1scun43{display:flex;position:relative;flex-wrap:wrap;align-items:center;gap:var(--checkbox-label-gap);padding:var(--checkbox-label-padding)}.token.svelte-1scun43.svelte-1scun43.svelte-1scun43{display:flex;align-items:center;transition:var(--button-transition);cursor:pointer;box-shadow:var(--checkbox-label-shadow);border:var(--checkbox-label-border-width) solid\n			var(--checkbox-label-border-color);border-radius:var(--button-small-radius);background:var(--checkbox-label-background-fill);padding:var(--checkbox-label-padding);color:var(--checkbox-label-text-color);font-weight:var(--checkbox-label-text-weight);font-size:var(--checkbox-label-text-size);line-height:var(--line-md);word-break:break-word}.token.svelte-1scun43>.svelte-1scun43+.svelte-1scun43{margin-left:var(--size-2)}.token-remove.svelte-1scun43.svelte-1scun43.svelte-1scun43{fill:var(--body-text-color);display:flex;justify-content:center;align-items:center;cursor:pointer;border:var(--checkbox-border-width) solid var(--border-color-primary);border-radius:var(--radius-full);background:var(--background-fill-primary);padding:var(--size-0-5);width:16px;height:16px;flex-shrink:0}.secondary-wrap.svelte-1scun43.svelte-1scun43.svelte-1scun43{display:flex;flex:1 1 0%;align-items:center;border:none;min-width:min-content}input.svelte-1scun43.svelte-1scun43.svelte-1scun43{margin:var(--spacing-sm);outline:none;border:none;background:inherit;width:var(--size-full);color:var(--body-text-color);font-size:var(--input-text-size)}input.svelte-1scun43.svelte-1scun43.svelte-1scun43:disabled{-webkit-text-fill-color:var(--body-text-color);-webkit-opacity:1;opacity:1;cursor:not-allowed}.remove-all.svelte-1scun43.svelte-1scun43.svelte-1scun43{margin-left:var(--size-1);width:20px;height:20px}.subdued.svelte-1scun43.svelte-1scun43.svelte-1scun43{color:var(--body-text-color-subdued)}input[readonly].svelte-1scun43.svelte-1scun43.svelte-1scun43{cursor:pointer}",
  map: '{"version":3,"file":"Multiselect.svelte","sources":["Multiselect.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { afterUpdate, createEventDispatcher } from \\"svelte\\";\\nimport { _ } from \\"svelte-i18n\\";\\nimport { BlockTitle } from \\"@gradio/atoms\\";\\nimport { Remove, DropdownArrow } from \\"@gradio/icons\\";\\nimport DropdownOptions from \\"./DropdownOptions.svelte\\";\\nimport { handle_filter, handle_change, handle_shared_keys } from \\"./utils\\";\\nexport let label;\\nexport let info = void 0;\\nexport let value = [];\\nlet old_value = [];\\nexport let value_is_output = false;\\nexport let max_choices = null;\\nexport let choices;\\nlet old_choices;\\nexport let disabled = false;\\nexport let show_label;\\nexport let container = true;\\nexport let allow_custom_value = false;\\nexport let filterable = true;\\nexport let i18n;\\nexport let root;\\nlet filter_input;\\nlet input_text = \\"\\";\\nlet old_input_text = \\"\\";\\nlet show_options = false;\\nlet choices_names;\\nlet choices_values;\\nlet filtered_indices = [];\\nlet active_index = null;\\nlet selected_indices = [];\\nlet old_selected_index = [];\\nconst dispatch = createEventDispatcher();\\nif (Array.isArray(value)) {\\n    value.forEach((element) => {\\n        const index = choices.map((c) => c[1]).indexOf(element);\\n        if (index !== -1) {\\n            selected_indices.push(index);\\n        }\\n        else {\\n            selected_indices.push(element);\\n        }\\n    });\\n}\\n$: {\\n    choices_names = choices.map((c) => c[0]);\\n    choices_values = choices.map((c) => c[1]);\\n}\\n$: {\\n    if (choices !== old_choices || input_text !== old_input_text) {\\n        filtered_indices = handle_filter(choices, input_text);\\n        old_choices = choices;\\n        old_input_text = input_text;\\n        if (!allow_custom_value) {\\n            active_index = filtered_indices[0];\\n        }\\n    }\\n}\\n$: {\\n    if (JSON.stringify(value) != JSON.stringify(old_value)) {\\n        handle_change(dispatch, value, value_is_output);\\n        old_value = Array.isArray(value) ? value.slice() : value;\\n    }\\n}\\n$: {\\n    if (JSON.stringify(selected_indices) != JSON.stringify(old_selected_index)) {\\n        value = selected_indices.map((index) => typeof index === \\"number\\" ? choices_values[index] : index);\\n        old_selected_index = selected_indices.slice();\\n    }\\n}\\nfunction handle_blur() {\\n    if (!allow_custom_value) {\\n        input_text = \\"\\";\\n    }\\n    if (allow_custom_value && input_text !== \\"\\") {\\n        add_selected_choice(input_text);\\n        input_text = \\"\\";\\n    }\\n    show_options = false;\\n    active_index = null;\\n    dispatch(\\"blur\\");\\n}\\nfunction remove_selected_choice(option_index) {\\n    selected_indices = selected_indices.filter((v) => v !== option_index);\\n    dispatch(\\"select\\", {\\n        index: typeof option_index === \\"number\\" ? option_index : -1,\\n        value: typeof option_index === \\"number\\" ? choices_values[option_index] : option_index,\\n        selected: false\\n    });\\n}\\nfunction add_selected_choice(option_index) {\\n    if (max_choices === null || selected_indices.length < max_choices) {\\n        selected_indices = [...selected_indices, option_index];\\n        dispatch(\\"select\\", {\\n            index: typeof option_index === \\"number\\" ? option_index : -1,\\n            value: typeof option_index === \\"number\\" ? choices_values[option_index] : option_index,\\n            selected: true\\n        });\\n    }\\n    if (selected_indices.length === max_choices) {\\n        show_options = false;\\n        active_index = null;\\n        filter_input.blur();\\n    }\\n}\\nfunction handle_option_selected(e) {\\n    const option_index = parseInt(e.detail.target.dataset.index);\\n    add_or_remove_index(option_index);\\n}\\nfunction add_or_remove_index(option_index) {\\n    if (selected_indices.includes(option_index)) {\\n        remove_selected_choice(option_index);\\n    }\\n    else {\\n        add_selected_choice(option_index);\\n    }\\n    input_text = \\"\\";\\n}\\nfunction remove_all(e) {\\n    selected_indices = [];\\n    input_text = \\"\\";\\n    e.preventDefault();\\n}\\nfunction handle_focus(e) {\\n    filtered_indices = choices.map((_2, i) => i);\\n    if (max_choices === null || selected_indices.length < max_choices) {\\n        show_options = true;\\n    }\\n    dispatch(\\"focus\\");\\n}\\nfunction handle_key_down(e) {\\n    [show_options, active_index] = handle_shared_keys(e, active_index, filtered_indices);\\n    if (e.key === \\"Enter\\") {\\n        if (active_index !== null) {\\n            add_or_remove_index(active_index);\\n        }\\n        else {\\n            if (allow_custom_value) {\\n                add_selected_choice(input_text);\\n                input_text = \\"\\";\\n            }\\n        }\\n    }\\n    if (e.key === \\"Backspace\\" && input_text === \\"\\") {\\n        selected_indices = [...selected_indices.slice(0, -1)];\\n    }\\n    if (selected_indices.length === max_choices) {\\n        show_options = false;\\n        active_index = null;\\n    }\\n}\\nfunction set_selected_indices() {\\n    if (value === void 0) {\\n        selected_indices = [];\\n    }\\n    else if (Array.isArray(value)) {\\n        selected_indices = value.map((v) => {\\n            const index = choices_values.indexOf(v);\\n            if (index !== -1) {\\n                return index;\\n            }\\n            if (allow_custom_value) {\\n                return v;\\n            }\\n            return void 0;\\n        }).filter((val) => val !== void 0);\\n    }\\n}\\n$: value, set_selected_indices();\\nafterUpdate(() => {\\n    value_is_output = false;\\n});\\n<\/script>\\n\\n<label class:container>\\n\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\n\\t<div class=\\"wrap\\">\\n\\t\\t<div class=\\"wrap-inner\\" class:show_options>\\n\\t\\t\\t{#each selected_indices as s}\\n\\t\\t\\t\\t<div class=\\"token\\">\\n\\t\\t\\t\\t\\t<span>\\n\\t\\t\\t\\t\\t\\t{#if typeof s === \\"number\\"}\\n\\t\\t\\t\\t\\t\\t\\t{choices_names[s]}\\n\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t{s}\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t\\t{#if !disabled}\\n\\t\\t\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"token-remove\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click|preventDefault={() => remove_selected_choice(s)}\\n\\t\\t\\t\\t\\t\\t\\ton:keydown|preventDefault={(event) => {\\n\\t\\t\\t\\t\\t\\t\\t\\tif (event.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tremove_selected_choice(s);\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\ttitle={i18n(\\"common.remove\\") + \\" \\" + s}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t<Remove />\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t{/each}\\n\\t\\t\\t<div class=\\"secondary-wrap\\">\\n\\t\\t\\t\\t<input\\n\\t\\t\\t\\t\\tclass=\\"border-none\\"\\n\\t\\t\\t\\t\\tclass:subdued={(!choices_names.includes(input_text) &&\\n\\t\\t\\t\\t\\t\\t!allow_custom_value) ||\\n\\t\\t\\t\\t\\t\\tselected_indices.length === max_choices}\\n\\t\\t\\t\\t\\t{disabled}\\n\\t\\t\\t\\t\\tautocomplete=\\"off\\"\\n\\t\\t\\t\\t\\tbind:value={input_text}\\n\\t\\t\\t\\t\\tbind:this={filter_input}\\n\\t\\t\\t\\t\\ton:keydown={handle_key_down}\\n\\t\\t\\t\\t\\ton:keyup={(e) =>\\n\\t\\t\\t\\t\\t\\tdispatch(\\"key_up\\", {\\n\\t\\t\\t\\t\\t\\t\\tkey: e.key,\\n\\t\\t\\t\\t\\t\\t\\tinput_value: input_text\\n\\t\\t\\t\\t\\t\\t})}\\n\\t\\t\\t\\t\\ton:blur={handle_blur}\\n\\t\\t\\t\\t\\ton:focus={handle_focus}\\n\\t\\t\\t\\t\\treadonly={!filterable}\\n\\t\\t\\t\\t/>\\n\\n\\t\\t\\t\\t{#if !disabled}\\n\\t\\t\\t\\t\\t{#if selected_indices.length > 0}\\n\\t\\t\\t\\t\\t\\t<div\\n\\t\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"token-remove remove-all\\"\\n\\t\\t\\t\\t\\t\\t\\ttitle={i18n(\\"common.clear\\")}\\n\\t\\t\\t\\t\\t\\t\\ton:click={remove_all}\\n\\t\\t\\t\\t\\t\\t\\ton:keydown={(event) => {\\n\\t\\t\\t\\t\\t\\t\\t\\tif (event.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tremove_all(event);\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t<Remove />\\n\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t<span class=\\"icon-wrap\\"> <DropdownArrow /></span>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t\\t<DropdownOptions\\n\\t\\t\\t{show_options}\\n\\t\\t\\t{choices}\\n\\t\\t\\t{filtered_indices}\\n\\t\\t\\t{disabled}\\n\\t\\t\\t{selected_indices}\\n\\t\\t\\t{active_index}\\n\\t\\t\\ton:change={handle_option_selected}\\n\\t\\t/>\\n\\t</div>\\n</label>\\n\\n<style>\\n\\t.icon-wrap {\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tmargin-right: var(--size-2);\\n\\t\\twidth: var(--size-5);\\n\\t}\\n\\tlabel:not(.container),\\n\\tlabel:not(.container) .wrap,\\n\\tlabel:not(.container) .wrap-inner,\\n\\tlabel:not(.container) .secondary-wrap,\\n\\tlabel:not(.container) .token,\\n\\tlabel:not(.container) input {\\n\\t\\theight: 100%;\\n\\t}\\n\\t.container .wrap {\\n\\t\\tbox-shadow: var(--input-shadow);\\n\\t\\tborder: var(--input-border-width) solid var(--border-color-primary);\\n\\t}\\n\\n\\t.wrap {\\n\\t\\tposition: relative;\\n\\t\\tborder-radius: var(--input-radius);\\n\\t\\tbackground: var(--input-background-fill);\\n\\t}\\n\\n\\t.wrap:focus-within {\\n\\t\\tbox-shadow: var(--input-shadow-focus);\\n\\t\\tborder-color: var(--input-border-color-focus);\\n\\t}\\n\\n\\t.wrap-inner {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tflex-wrap: wrap;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--checkbox-label-gap);\\n\\t\\tpadding: var(--checkbox-label-padding);\\n\\t}\\n\\n\\t.token {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tcursor: pointer;\\n\\t\\tbox-shadow: var(--checkbox-label-shadow);\\n\\t\\tborder: var(--checkbox-label-border-width) solid\\n\\t\\t\\tvar(--checkbox-label-border-color);\\n\\t\\tborder-radius: var(--button-small-radius);\\n\\t\\tbackground: var(--checkbox-label-background-fill);\\n\\t\\tpadding: var(--checkbox-label-padding);\\n\\t\\tcolor: var(--checkbox-label-text-color);\\n\\t\\tfont-weight: var(--checkbox-label-text-weight);\\n\\t\\tfont-size: var(--checkbox-label-text-size);\\n\\t\\tline-height: var(--line-md);\\n\\t\\tword-break: break-word;\\n\\t}\\n\\n\\t.token > * + * {\\n\\t\\tmargin-left: var(--size-2);\\n\\t}\\n\\n\\t.token-remove {\\n\\t\\tfill: var(--body-text-color);\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tcursor: pointer;\\n\\t\\tborder: var(--checkbox-border-width) solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-full);\\n\\t\\tbackground: var(--background-fill-primary);\\n\\t\\tpadding: var(--size-0-5);\\n\\t\\twidth: 16px;\\n\\t\\theight: 16px;\\n\\t\\tflex-shrink: 0;\\n\\t}\\n\\n\\t.secondary-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex: 1 1 0%;\\n\\t\\talign-items: center;\\n\\t\\tborder: none;\\n\\t\\tmin-width: min-content;\\n\\t}\\n\\n\\tinput {\\n\\t\\tmargin: var(--spacing-sm);\\n\\t\\toutline: none;\\n\\t\\tborder: none;\\n\\t\\tbackground: inherit;\\n\\t\\twidth: var(--size-full);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-size: var(--input-text-size);\\n\\t}\\n\\n\\tinput:disabled {\\n\\t\\t-webkit-text-fill-color: var(--body-text-color);\\n\\t\\t-webkit-opacity: 1;\\n\\t\\topacity: 1;\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\t.remove-all {\\n\\t\\tmargin-left: var(--size-1);\\n\\t\\twidth: 20px;\\n\\t\\theight: 20px;\\n\\t}\\n\\t.subdued {\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\tinput[readonly] {\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"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"}'
};
const Multiselect = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { value = [] } = $$props;
  let old_value = [];
  let { value_is_output = false } = $$props;
  let { max_choices = null } = $$props;
  let { choices } = $$props;
  let old_choices;
  let { disabled = false } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { allow_custom_value = false } = $$props;
  let { filterable = true } = $$props;
  let { i18n } = $$props;
  let { root } = $$props;
  let filter_input;
  let input_text = "";
  let old_input_text = "";
  let show_options = false;
  let choices_names;
  let choices_values;
  let filtered_indices = [];
  let active_index = null;
  let selected_indices = [];
  let old_selected_index = [];
  const dispatch = createEventDispatcher();
  if (Array.isArray(value)) {
    value.forEach((element) => {
      const index = choices.map((c) => c[1]).indexOf(element);
      if (index !== -1) {
        selected_indices.push(index);
      } else {
        selected_indices.push(element);
      }
    });
  }
  function set_selected_indices() {
    if (value === void 0) {
      selected_indices = [];
    } else if (Array.isArray(value)) {
      selected_indices = value.map((v) => {
        const index = choices_values.indexOf(v);
        if (index !== -1) {
          return index;
        }
        if (allow_custom_value) {
          return v;
        }
        return void 0;
      }).filter((val) => val !== void 0);
    }
  }
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.max_choices === void 0 && $$bindings.max_choices && max_choices !== void 0)
    $$bindings.max_choices(max_choices);
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.allow_custom_value === void 0 && $$bindings.allow_custom_value && allow_custom_value !== void 0)
    $$bindings.allow_custom_value(allow_custom_value);
  if ($$props.filterable === void 0 && $$bindings.filterable && filterable !== void 0)
    $$bindings.filterable(filterable);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  $$result.css.add(css$1);
  {
    {
      choices_names = choices.map((c) => c[0]);
      choices_values = choices.map((c) => c[1]);
    }
  }
  {
    {
      if (choices !== old_choices || input_text !== old_input_text) {
        filtered_indices = handle_filter(choices, input_text);
        old_choices = choices;
        old_input_text = input_text;
        if (!allow_custom_value) {
          active_index = filtered_indices[0];
        }
      }
    }
  }
  {
    {
      if (JSON.stringify(selected_indices) != JSON.stringify(old_selected_index)) {
        value = selected_indices.map((index) => typeof index === "number" ? choices_values[index] : index);
        old_selected_index = selected_indices.slice();
      }
    }
  }
  {
    {
      if (JSON.stringify(value) != JSON.stringify(old_value)) {
        handle_change(dispatch, value, value_is_output);
        old_value = Array.isArray(value) ? value.slice() : value;
      }
    }
  }
  {
    set_selected_indices();
  }
  return `<label class="${["svelte-1scun43", container ? "container" : ""].join(" ").trim()}">${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
    default: () => {
      return `${escape(label)}`;
    }
  })} <div class="wrap svelte-1scun43"><div class="${["wrap-inner svelte-1scun43", ""].join(" ").trim()}">${each(selected_indices, (s) => {
    return `<div class="token svelte-1scun43"><span class="svelte-1scun43">${typeof s === "number" ? `${escape(choices_names[s])}` : `${escape(s)}`}</span> ${!disabled ? `<div class="token-remove svelte-1scun43" role="button" tabindex="0"${add_attribute("title", i18n("common.remove") + " " + s, 0)}>${validate_component(Remove, "Remove").$$render($$result, {}, {}, {})} </div>` : ``} </div>`;
  })} <div class="secondary-wrap svelte-1scun43"><input class="${[
    "border-none svelte-1scun43",
    !choices_names.includes(input_text) && !allow_custom_value || selected_indices.length === max_choices ? "subdued" : ""
  ].join(" ").trim()}" ${disabled ? "disabled" : ""} autocomplete="off" ${!filterable ? "readonly" : ""}${add_attribute("value", input_text, 0)}${add_attribute("this", filter_input, 0)}> ${!disabled ? `${selected_indices.length > 0 ? `<div role="button" tabindex="0" class="token-remove remove-all svelte-1scun43"${add_attribute("title", i18n("common.clear"), 0)}>${validate_component(Remove, "Remove").$$render($$result, {}, {}, {})}</div>` : ``} <span class="icon-wrap svelte-1scun43">${validate_component(DropdownArrow, "DropdownArrow").$$render($$result, {}, {}, {})}</span>` : ``}</div></div> ${validate_component(DropdownOptions, "DropdownOptions").$$render(
    $$result,
    {
      show_options,
      choices,
      filtered_indices,
      disabled,
      selected_indices,
      active_index
    },
    {},
    {}
  )}</div> </label>`;
});
const Multiselect$1 = Multiselect;
const css = {
  code: ".icon-wrap.svelte-1hfxrpf.svelte-1hfxrpf{position:absolute;top:50%;transform:translateY(-50%);right:var(--size-5);color:var(--body-text-color);width:var(--size-5);pointer-events:none}.container.svelte-1hfxrpf.svelte-1hfxrpf{height:100%}.container.svelte-1hfxrpf .wrap.svelte-1hfxrpf{box-shadow:var(--input-shadow);border:var(--input-border-width) solid var(--border-color-primary)}.wrap.svelte-1hfxrpf.svelte-1hfxrpf{position:relative;border-radius:var(--input-radius);background:var(--input-background-fill)}.wrap.svelte-1hfxrpf.svelte-1hfxrpf:focus-within{box-shadow:var(--input-shadow-focus);border-color:var(--input-border-color-focus);background:var(--input-background-fill-focus)}.wrap-inner.svelte-1hfxrpf.svelte-1hfxrpf{display:flex;position:relative;flex-wrap:wrap;align-items:center;gap:var(--checkbox-label-gap);padding:var(--checkbox-label-padding);height:100%}.secondary-wrap.svelte-1hfxrpf.svelte-1hfxrpf{display:flex;flex:1 1 0%;align-items:center;border:none;min-width:min-content;height:100%}input.svelte-1hfxrpf.svelte-1hfxrpf{margin:var(--spacing-sm);outline:none;border:none;background:inherit;width:var(--size-full);color:var(--body-text-color);font-size:var(--input-text-size);height:100%}input.svelte-1hfxrpf.svelte-1hfxrpf:disabled{-webkit-text-fill-color:var(--body-text-color);-webkit-opacity:1;opacity:1;cursor:not-allowed}.subdued.svelte-1hfxrpf.svelte-1hfxrpf{color:var(--body-text-color-subdued)}input[readonly].svelte-1hfxrpf.svelte-1hfxrpf{cursor:pointer}",
  map: '{"version":3,"file":"Dropdown.svelte","sources":["Dropdown.svelte"],"sourcesContent":["<script lang=\\"ts\\">import DropdownOptions from \\"./DropdownOptions.svelte\\";\\nimport { createEventDispatcher, afterUpdate } from \\"svelte\\";\\nimport { BlockTitle } from \\"@gradio/atoms\\";\\nimport { DropdownArrow } from \\"@gradio/icons\\";\\nimport { handle_filter, handle_change, handle_shared_keys } from \\"./utils\\";\\nexport let label;\\nexport let info = void 0;\\nexport let value = void 0;\\nlet old_value = void 0;\\nexport let value_is_output = false;\\nexport let choices;\\nlet old_choices;\\nexport let disabled = false;\\nexport let show_label;\\nexport let container = true;\\nexport let allow_custom_value = false;\\nexport let filterable = true;\\nexport let root;\\nlet filter_input;\\nlet show_options = false;\\nlet choices_names;\\nlet choices_values;\\nlet input_text = \\"\\";\\nlet old_input_text = \\"\\";\\nlet initialized = false;\\nlet filtered_indices = [];\\nlet active_index = null;\\nlet selected_index = null;\\nlet old_selected_index;\\nconst dispatch = createEventDispatcher();\\nif (value) {\\n    old_selected_index = choices.map((c) => c[1]).indexOf(value);\\n    selected_index = old_selected_index;\\n    if (selected_index === -1) {\\n        old_value = value;\\n        selected_index = null;\\n    }\\n    else {\\n        [input_text, old_value] = choices[selected_index];\\n        old_input_text = input_text;\\n    }\\n    set_input_text();\\n}\\n$: {\\n    if (selected_index !== old_selected_index && selected_index !== null && initialized) {\\n        [input_text, value] = choices[selected_index];\\n        old_selected_index = selected_index;\\n        dispatch(\\"select\\", {\\n            index: selected_index,\\n            value: choices_values[selected_index],\\n            selected: true\\n        });\\n    }\\n}\\n$: if (JSON.stringify(old_value) !== JSON.stringify(value)) {\\n    set_input_text();\\n    handle_change(dispatch, value, value_is_output);\\n    old_value = value;\\n}\\nfunction set_choice_names_values() {\\n    choices_names = choices.map((c) => c[0]);\\n    choices_values = choices.map((c) => c[1]);\\n}\\n$: choices, set_choice_names_values();\\nconst is_browser = typeof window !== \\"undefined\\";\\n$: {\\n    if (choices !== old_choices) {\\n        if (!allow_custom_value) {\\n            set_input_text();\\n        }\\n        old_choices = choices;\\n        filtered_indices = handle_filter(choices, input_text);\\n        if (!allow_custom_value && filtered_indices.length > 0) {\\n            active_index = filtered_indices[0];\\n        }\\n        if (is_browser && filter_input === document.activeElement) {\\n            show_options = true;\\n        }\\n    }\\n}\\n$: {\\n    if (input_text !== old_input_text) {\\n        filtered_indices = handle_filter(choices, input_text);\\n        old_input_text = input_text;\\n        if (!allow_custom_value && filtered_indices.length > 0) {\\n            active_index = filtered_indices[0];\\n        }\\n    }\\n}\\nfunction set_input_text() {\\n    set_choice_names_values();\\n    if (value === void 0 || Array.isArray(value) && value.length === 0) {\\n        input_text = \\"\\";\\n        selected_index = null;\\n    }\\n    else if (choices_values.includes(value)) {\\n        input_text = choices_names[choices_values.indexOf(value)];\\n        selected_index = choices_values.indexOf(value);\\n    }\\n    else if (allow_custom_value) {\\n        input_text = value;\\n        selected_index = null;\\n    }\\n    else {\\n        input_text = \\"\\";\\n        selected_index = null;\\n    }\\n    old_selected_index = selected_index;\\n}\\nfunction handle_option_selected(e) {\\n    selected_index = parseInt(e.detail.target.dataset.index);\\n    if (isNaN(selected_index)) {\\n        selected_index = null;\\n        return;\\n    }\\n    show_options = false;\\n    active_index = null;\\n    filter_input.blur();\\n}\\nfunction handle_focus(e) {\\n    filtered_indices = choices.map((_, i) => i);\\n    show_options = true;\\n    dispatch(\\"focus\\");\\n}\\nfunction handle_blur() {\\n    if (!allow_custom_value) {\\n        input_text = choices_names[choices_values.indexOf(value)];\\n    }\\n    else {\\n        value = input_text;\\n    }\\n    show_options = false;\\n    active_index = null;\\n    dispatch(\\"blur\\");\\n}\\nfunction handle_key_down(e) {\\n    [show_options, active_index] = handle_shared_keys(e, active_index, filtered_indices);\\n    if (e.key === \\"Enter\\") {\\n        if (active_index !== null) {\\n            selected_index = active_index;\\n            show_options = false;\\n            filter_input.blur();\\n            active_index = null;\\n        }\\n        else if (choices_names.includes(input_text)) {\\n            selected_index = choices_names.indexOf(input_text);\\n            show_options = false;\\n            active_index = null;\\n            filter_input.blur();\\n        }\\n        else if (allow_custom_value) {\\n            value = input_text;\\n            selected_index = null;\\n            show_options = false;\\n            active_index = null;\\n            filter_input.blur();\\n        }\\n    }\\n}\\nafterUpdate(() => {\\n    value_is_output = false;\\n    initialized = true;\\n});\\n<\/script>\\n\\n<div class:container>\\n\\t<BlockTitle {root} {show_label} {info}>{label}</BlockTitle>\\n\\n\\t<div class=\\"wrap\\">\\n\\t\\t<div class=\\"wrap-inner\\" class:show_options>\\n\\t\\t\\t<div class=\\"secondary-wrap\\">\\n\\t\\t\\t\\t<input\\n\\t\\t\\t\\t\\trole=\\"listbox\\"\\n\\t\\t\\t\\t\\taria-controls=\\"dropdown-options\\"\\n\\t\\t\\t\\t\\taria-expanded={show_options}\\n\\t\\t\\t\\t\\taria-label={label}\\n\\t\\t\\t\\t\\tclass=\\"border-none\\"\\n\\t\\t\\t\\t\\tclass:subdued={!choices_names.includes(input_text) &&\\n\\t\\t\\t\\t\\t\\t!allow_custom_value}\\n\\t\\t\\t\\t\\t{disabled}\\n\\t\\t\\t\\t\\tautocomplete=\\"off\\"\\n\\t\\t\\t\\t\\tbind:value={input_text}\\n\\t\\t\\t\\t\\tbind:this={filter_input}\\n\\t\\t\\t\\t\\ton:keydown={handle_key_down}\\n\\t\\t\\t\\t\\ton:keyup={(e) =>\\n\\t\\t\\t\\t\\t\\tdispatch(\\"key_up\\", {\\n\\t\\t\\t\\t\\t\\t\\tkey: e.key,\\n\\t\\t\\t\\t\\t\\t\\tinput_value: input_text\\n\\t\\t\\t\\t\\t\\t})}\\n\\t\\t\\t\\t\\ton:blur={handle_blur}\\n\\t\\t\\t\\t\\ton:focus={handle_focus}\\n\\t\\t\\t\\t\\treadonly={!filterable}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t\\t{#if !disabled}\\n\\t\\t\\t\\t\\t<div class=\\"icon-wrap\\">\\n\\t\\t\\t\\t\\t\\t<DropdownArrow />\\n\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t\\t<DropdownOptions\\n\\t\\t\\t{show_options}\\n\\t\\t\\t{choices}\\n\\t\\t\\t{filtered_indices}\\n\\t\\t\\t{disabled}\\n\\t\\t\\tselected_indices={selected_index === null ? [] : [selected_index]}\\n\\t\\t\\t{active_index}\\n\\t\\t\\ton:change={handle_option_selected}\\n\\t\\t/>\\n\\t</div>\\n</div>\\n\\n<style>\\n\\t.icon-wrap {\\n\\t\\tposition: absolute;\\n\\t\\ttop: 50%;\\n\\t\\ttransform: translateY(-50%);\\n\\t\\tright: var(--size-5);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\twidth: var(--size-5);\\n\\t\\tpointer-events: none;\\n\\t}\\n\\t.container {\\n\\t\\theight: 100%;\\n\\t}\\n\\t.container .wrap {\\n\\t\\tbox-shadow: var(--input-shadow);\\n\\t\\tborder: var(--input-border-width) solid var(--border-color-primary);\\n\\t}\\n\\n\\t.wrap {\\n\\t\\tposition: relative;\\n\\t\\tborder-radius: var(--input-radius);\\n\\t\\tbackground: var(--input-background-fill);\\n\\t}\\n\\n\\t.wrap:focus-within {\\n\\t\\tbox-shadow: var(--input-shadow-focus);\\n\\t\\tborder-color: var(--input-border-color-focus);\\n\\t\\tbackground: var(--input-background-fill-focus);\\n\\t}\\n\\n\\t.wrap-inner {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tflex-wrap: wrap;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--checkbox-label-gap);\\n\\t\\tpadding: var(--checkbox-label-padding);\\n\\t\\theight: 100%;\\n\\t}\\n\\t.secondary-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex: 1 1 0%;\\n\\t\\talign-items: center;\\n\\t\\tborder: none;\\n\\t\\tmin-width: min-content;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\tinput {\\n\\t\\tmargin: var(--spacing-sm);\\n\\t\\toutline: none;\\n\\t\\tborder: none;\\n\\t\\tbackground: inherit;\\n\\t\\twidth: var(--size-full);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-size: var(--input-text-size);\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\tinput:disabled {\\n\\t\\t-webkit-text-fill-color: var(--body-text-color);\\n\\t\\t-webkit-opacity: 1;\\n\\t\\topacity: 1;\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\t.subdued {\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\n\\tinput[readonly] {\\n\\t\\tcursor: pointer;\\n\\t}</style>\\n"],"names":[],"mappings":"AAqNC,wCAAW,CACV,QAAQ,CAAE,QAAQ,CAClB,GAAG,CAAE,GAAG,CACR,SAAS,CAAE,WAAW,IAAI,CAAC,CAC3B,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,cAAc,CAAE,IACjB,CACA,wCAAW,CACV,MAAM,CAAE,IACT,CACA,yBAAU,CAAC,oBAAM,CAChB,UAAU,CAAE,IAAI,cAAc,CAAC,CAC/B,MAAM,CAAE,IAAI,oBAAoB,CAAC,CAAC,KAAK,CAAC,IAAI,sBAAsB,CACnE,CAEA,mCAAM,CACL,QAAQ,CAAE,QAAQ,CAClB,aAAa,CAAE,IAAI,cAAc,CAAC,CAClC,UAAU,CAAE,IAAI,uBAAuB,CACxC,CAEA,mCAAK,aAAc,CAClB,UAAU,CAAE,IAAI,oBAAoB,CAAC,CACrC,YAAY,CAAE,IAAI,0BAA0B,CAAC,CAC7C,UAAU,CAAE,IAAI,6BAA6B,CAC9C,CAEA,yCAAY,CACX,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,SAAS,CAAE,IAAI,CACf,WAAW,CAAE,MAAM,CACnB,GAAG,CAAE,IAAI,oBAAoB,CAAC,CAC9B,OAAO,CAAE,IAAI,wBAAwB,CAAC,CACtC,MAAM,CAAE,IACT,CACA,6CAAgB,CACf,OAAO,CAAE,IAAI,CACb,IAAI,CAAE,CAAC,CAAC,CAAC,CAAC,EAAE,CACZ,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,IAAI,CACZ,SAAS,CAAE,WAAW,CACtB,MAAM,CAAE,IACT,CAEA,mCAAM,CACL,MAAM,CAAE,IAAI,YAAY,CAAC,CACzB,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,IAAI,CACZ,UAAU,CAAE,OAAO,CACnB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,SAAS,CAAE,IAAI,iBAAiB,CAAC,CACjC,MAAM,CAAE,IACT,CAEA,mCAAK,SAAU,CACd,uBAAuB,CAAE,IAAI,iBAAiB,CAAC,CAC/C,eAAe,CAAE,CAAC,CAClB,OAAO,CAAE,CAAC,CACV,MAAM,CAAE,WACT,CAEA,sCAAS,CACR,KAAK,CAAE,IAAI,yBAAyB,CACrC,CAEA,KAAK,CAAC,QAAQ,+BAAE,CACf,MAAM,CAAE,OACT"}'
};
const Dropdown = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label } = $$props;
  let { info = void 0 } = $$props;
  let { value = void 0 } = $$props;
  let old_value = void 0;
  let { value_is_output = false } = $$props;
  let { choices } = $$props;
  let old_choices;
  let { disabled = false } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { allow_custom_value = false } = $$props;
  let { filterable = true } = $$props;
  let { root } = $$props;
  let filter_input;
  let show_options = false;
  let choices_names;
  let choices_values;
  let input_text = "";
  let old_input_text = "";
  let initialized = false;
  let filtered_indices = [];
  let active_index = null;
  let selected_index = null;
  let old_selected_index;
  const dispatch = createEventDispatcher();
  if (value) {
    old_selected_index = choices.map((c) => c[1]).indexOf(value);
    selected_index = old_selected_index;
    if (selected_index === -1) {
      old_value = value;
      selected_index = null;
    } else {
      [input_text, old_value] = choices[selected_index];
      old_input_text = input_text;
    }
    set_input_text();
  }
  function set_choice_names_values() {
    choices_names = choices.map((c) => c[0]);
    choices_values = choices.map((c) => c[1]);
  }
  const is_browser = typeof window !== "undefined";
  function set_input_text() {
    set_choice_names_values();
    if (value === void 0 || Array.isArray(value) && value.length === 0) {
      input_text = "";
      selected_index = null;
    } else if (choices_values.includes(value)) {
      input_text = choices_names[choices_values.indexOf(value)];
      selected_index = choices_values.indexOf(value);
    } else if (allow_custom_value) {
      input_text = value;
      selected_index = null;
    } else {
      input_text = "";
      selected_index = null;
    }
    old_selected_index = selected_index;
  }
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.info === void 0 && $$bindings.info && info !== void 0)
    $$bindings.info(info);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.allow_custom_value === void 0 && $$bindings.allow_custom_value && allow_custom_value !== void 0)
    $$bindings.allow_custom_value(allow_custom_value);
  if ($$props.filterable === void 0 && $$bindings.filterable && filterable !== void 0)
    $$bindings.filterable(filterable);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  $$result.css.add(css);
  {
    {
      if (selected_index !== old_selected_index && selected_index !== null && initialized) {
        [input_text, value] = choices[selected_index];
        old_selected_index = selected_index;
        dispatch("select", {
          index: selected_index,
          value: choices_values[selected_index],
          selected: true
        });
      }
    }
  }
  {
    if (JSON.stringify(old_value) !== JSON.stringify(value)) {
      set_input_text();
      handle_change(dispatch, value, value_is_output);
      old_value = value;
    }
  }
  {
    set_choice_names_values();
  }
  {
    {
      if (choices !== old_choices) {
        if (!allow_custom_value) {
          set_input_text();
        }
        old_choices = choices;
        filtered_indices = handle_filter(choices, input_text);
        if (!allow_custom_value && filtered_indices.length > 0) {
          active_index = filtered_indices[0];
        }
        if (is_browser && filter_input === document.activeElement) {
          show_options = true;
        }
      }
    }
  }
  {
    {
      if (input_text !== old_input_text) {
        filtered_indices = handle_filter(choices, input_text);
        old_input_text = input_text;
        if (!allow_custom_value && filtered_indices.length > 0) {
          active_index = filtered_indices[0];
        }
      }
    }
  }
  return `<div class="${["svelte-1hfxrpf", container ? "container" : ""].join(" ").trim()}">${validate_component(BlockTitle, "BlockTitle").$$render($$result, { root, show_label, info }, {}, {
    default: () => {
      return `${escape(label)}`;
    }
  })} <div class="wrap svelte-1hfxrpf"><div class="${["wrap-inner svelte-1hfxrpf", show_options ? "show_options" : ""].join(" ").trim()}"><div class="secondary-wrap svelte-1hfxrpf"><input role="listbox" aria-controls="dropdown-options"${add_attribute("aria-expanded", show_options, 0)}${add_attribute("aria-label", label, 0)} class="${[
    "border-none svelte-1hfxrpf",
    !choices_names.includes(input_text) && !allow_custom_value ? "subdued" : ""
  ].join(" ").trim()}" ${disabled ? "disabled" : ""} autocomplete="off" ${!filterable ? "readonly" : ""}${add_attribute("value", input_text, 0)}${add_attribute("this", filter_input, 0)}> ${!disabled ? `<div class="icon-wrap svelte-1hfxrpf">${validate_component(DropdownArrow, "DropdownArrow").$$render($$result, {}, {}, {})}</div>` : ``}</div></div> ${validate_component(DropdownOptions, "DropdownOptions").$$render(
    $$result,
    {
      show_options,
      choices,
      filtered_indices,
      disabled,
      selected_indices: selected_index === null ? [] : [selected_index],
      active_index
    },
    {},
    {}
  )}</div> </div>`;
});
const Dropdown$1 = Dropdown;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label = "Dropdown" } = $$props;
  let { info = void 0 } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { multiselect = false } = $$props;
  let { value = multiselect ? [] : void 0 } = $$props;
  let { value_is_output = false } = $$props;
  let { max_choices = null } = $$props;
  let { choices } = $$props;
  let { show_label } = $$props;
  let { filterable } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { allow_custom_value = false } = $$props;
  let { root } = $$props;
  let { gradio } = $$props;
  let { interactive } = $$props;
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
  if ($$props.multiselect === void 0 && $$bindings.multiselect && multiselect !== void 0)
    $$bindings.multiselect(multiselect);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.max_choices === void 0 && $$bindings.max_choices && max_choices !== void 0)
    $$bindings.max_choices(max_choices);
  if ($$props.choices === void 0 && $$bindings.choices && choices !== void 0)
    $$bindings.choices(choices);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.filterable === void 0 && $$bindings.filterable && filterable !== void 0)
    $$bindings.filterable(filterable);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.allow_custom_value === void 0 && $$bindings.allow_custom_value && allow_custom_value !== void 0)
    $$bindings.allow_custom_value(allow_custom_value);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        elem_id,
        elem_classes,
        padding: container,
        allow_overflow: false,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${multiselect ? `${validate_component(Multiselect$1, "Multiselect").$$render(
            $$result,
            {
              choices,
              max_choices,
              root,
              label,
              info,
              show_label,
              allow_custom_value,
              filterable,
              container,
              i18n: gradio.i18n,
              disabled: !interactive,
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
          )}` : `${validate_component(Dropdown$1, "Dropdown").$$render(
            $$result,
            {
              choices,
              label,
              root,
              info,
              show_label,
              filterable,
              allow_custom_value,
              container,
              disabled: !interactive,
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
          )}`}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { Dropdown$1 as BaseDropdown, Multiselect$1 as BaseMultiselect, Index as default };
//# sourceMappingURL=Index51-DJGe2w2x.js.map
