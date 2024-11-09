import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, s as subscribe, p as setContext, q as set_store_value, e as escape, b as add_attribute, f as each } from './ssr-RaXq3SJh.js';
import { w as writable } from './index-hSrgoQUm.js';

const OverflowIcon = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  return `<svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg"><circle cx="2.5" cy="8" r="1.5" fill="currentColor"></circle><circle cx="8" cy="8" r="1.5" fill="currentColor"></circle><circle cx="13.5" cy="8" r="1.5" fill="currentColor"></circle></svg>`;
});
const css = {
  code: '.tabs.svelte-1tcem6n.svelte-1tcem6n{position:relative;display:flex;flex-direction:column;gap:var(--layout-gap)}.hide.svelte-1tcem6n.svelte-1tcem6n{display:none}.tab-wrapper.svelte-1tcem6n.svelte-1tcem6n{display:flex;align-items:center;justify-content:space-between;position:relative;height:var(--size-8);padding-bottom:var(--size-2)}.tab-container.svelte-1tcem6n.svelte-1tcem6n{display:flex;align-items:center;width:100%;position:relative;overflow:hidden;height:var(--size-8)}.tab-container.svelte-1tcem6n.svelte-1tcem6n::after{content:"";position:absolute;bottom:0;left:0;right:0;height:1px;background-color:var(--border-color-primary)}.overflow-menu.svelte-1tcem6n.svelte-1tcem6n{flex-shrink:0;margin-left:var(--size-2)}button.svelte-1tcem6n.svelte-1tcem6n{margin-bottom:0;border:none;border-radius:0;padding:0 var(--size-4);color:var(--body-text-color);font-weight:var(--section-header-text-weight);font-size:var(--section-header-text-size);transition:background-color color 0.2s ease-out;background-color:transparent;height:100%;display:flex;align-items:center;white-space:nowrap;position:relative}button.svelte-1tcem6n.svelte-1tcem6n:disabled{opacity:0.5;cursor:not-allowed}button.svelte-1tcem6n.svelte-1tcem6n:hover:not(:disabled):not(.selected){background-color:var(--background-fill-secondary);color:var(--body-text-color)}.selected.svelte-1tcem6n.svelte-1tcem6n{background-color:transparent;color:var(--color-accent);position:relative}.selected.svelte-1tcem6n.svelte-1tcem6n::after{content:"";position:absolute;bottom:0;left:0;width:100%;height:2px;background-color:var(--color-accent);animation:svelte-1tcem6n-fade-grow 0.2s ease-out forwards;transform-origin:center;z-index:1}@keyframes svelte-1tcem6n-fade-grow{from{opacity:0;transform:scaleX(0.8)}to{opacity:1;transform:scaleX(1)}}.overflow-dropdown.svelte-1tcem6n.svelte-1tcem6n{position:absolute;top:calc(100% + var(--size-2));right:0;background-color:var(--background-fill-primary);border:1px solid var(--border-color-primary);border-radius:var(--radius-sm);z-index:var(--layer-5);box-shadow:0 2px 5px rgba(0, 0, 0, 0.1);padding:var(--size-2);min-width:150px;width:max-content}.overflow-dropdown.svelte-1tcem6n button.svelte-1tcem6n{display:block;width:100%;text-align:left;padding:var(--size-2);white-space:nowrap;overflow:hidden;text-overflow:ellipsis}.overflow-menu.svelte-1tcem6n>button.svelte-1tcem6n{padding:var(--size-1) var(--size-2);min-width:auto;border:1px solid var(--border-color-primary);border-radius:var(--radius-sm);display:flex;align-items:center;justify-content:center}.overflow-menu.svelte-1tcem6n>button.svelte-1tcem6n:hover{background-color:var(--background-fill-secondary)}.overflow-menu.svelte-1tcem6n svg{width:16px;height:16px}.overflow-item-selected.svelte-1tcem6n svg{color:var(--color-accent)}.visually-hidden.svelte-1tcem6n.svelte-1tcem6n{position:absolute;width:1px;height:1px;padding:0;margin:-1px;overflow:hidden;clip:rect(0, 0, 0, 0);white-space:nowrap;border:0}',
  map: `{"version":3,"file":"Tabs.svelte","sources":["Tabs.svelte"],"sourcesContent":["<script context=\\"module\\" lang=\\"ts\\">export const TABS = {};\\n<\/script>\\n\\n<script lang=\\"ts\\">import { setContext, createEventDispatcher, tick, onMount } from \\"svelte\\";\\nimport OverflowIcon from \\"./OverflowIcon.svelte\\";\\nimport { writable } from \\"svelte/store\\";\\nexport let visible = true;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let selected;\\nexport let initial_tabs;\\nlet tabs = [...initial_tabs];\\nlet visible_tabs = [...initial_tabs];\\nlet overflow_tabs = [];\\nlet overflow_menu_open = false;\\nlet overflow_menu;\\n$: has_tabs = tabs.length > 0;\\nlet tab_nav_el;\\nconst selected_tab = writable(selected || tabs[0]?.id || false);\\nconst selected_tab_index = writable(tabs.findIndex((t) => t.id === selected) || 0);\\nconst dispatch = createEventDispatcher();\\nlet is_overflowing = false;\\nlet overflow_has_selected_tab = false;\\nlet tab_els = {};\\nonMount(() => {\\n    const observer = new IntersectionObserver((entries) => {\\n        handle_menu_overflow();\\n    });\\n    observer.observe(tab_nav_el);\\n});\\nsetContext(TABS, {\\n    register_tab: (tab) => {\\n        let index = tabs.findIndex((t) => t.id === tab.id);\\n        if (index !== -1) {\\n            tabs[index] = { ...tabs[index], ...tab };\\n        }\\n        else {\\n            tabs = [...tabs, tab];\\n            index = tabs.length - 1;\\n        }\\n        if ($selected_tab === false && tab.visible && tab.interactive) {\\n            $selected_tab = tab.id;\\n        }\\n        return index;\\n    },\\n    unregister_tab: (tab) => {\\n        const index = tabs.findIndex((t) => t.id === tab.id);\\n        if (index !== -1) {\\n            tabs = tabs.filter((t) => t.id !== tab.id);\\n            if ($selected_tab === tab.id) {\\n                $selected_tab = tabs[0]?.id || false;\\n            }\\n        }\\n    },\\n    selected_tab,\\n    selected_tab_index\\n});\\nfunction change_tab(id) {\\n    const tab_to_activate = tabs.find((t) => t.id === id);\\n    if (tab_to_activate && tab_to_activate.interactive && tab_to_activate.visible && $selected_tab !== tab_to_activate.id) {\\n        selected = id;\\n        $selected_tab = id;\\n        $selected_tab_index = tabs.findIndex((t) => t.id === id);\\n        dispatch(\\"change\\");\\n        overflow_menu_open = false;\\n    }\\n}\\n$: tabs, selected !== null && change_tab(selected);\\n$: tabs, tab_nav_el, tab_els, handle_menu_overflow();\\nfunction handle_outside_click(event) {\\n    if (overflow_menu_open && overflow_menu && !overflow_menu.contains(event.target)) {\\n        overflow_menu_open = false;\\n    }\\n}\\nasync function handle_menu_overflow() {\\n    if (!tab_nav_el)\\n        return;\\n    await tick();\\n    const tab_nav_size = tab_nav_el.getBoundingClientRect();\\n    let max_width = tab_nav_size.width;\\n    const tab_sizes = get_tab_sizes(tabs, tab_els);\\n    let last_visible_index = 0;\\n    const offset = tab_nav_size.left;\\n    for (let i = tabs.length - 1; i >= 0; i--) {\\n        const tab = tabs[i];\\n        const tab_rect = tab_sizes[tab.id];\\n        if (!tab_rect)\\n            continue;\\n        if (tab_rect.right - offset < max_width) {\\n            last_visible_index = i;\\n            break;\\n        }\\n    }\\n    overflow_tabs = tabs.slice(last_visible_index + 1);\\n    visible_tabs = tabs.slice(0, last_visible_index + 1);\\n    overflow_has_selected_tab = handle_overflow_has_selected_tab($selected_tab);\\n    is_overflowing = overflow_tabs.length > 0;\\n}\\n$: overflow_has_selected_tab = handle_overflow_has_selected_tab($selected_tab);\\nfunction handle_overflow_has_selected_tab(selected_tab2) {\\n    if (selected_tab2 === false)\\n        return false;\\n    return overflow_tabs.some((t) => t.id === selected_tab2);\\n}\\nfunction get_tab_sizes(tabs2, tab_els2) {\\n    const tab_sizes = {};\\n    tabs2.forEach((tab) => {\\n        tab_sizes[tab.id] = tab_els2[tab.id]?.getBoundingClientRect();\\n    });\\n    return tab_sizes;\\n}\\n<\/script>\\n\\n<svelte:window\\n\\ton:resize={handle_menu_overflow}\\n\\ton:click={handle_outside_click}\\n/>\\n\\n<div class=\\"tabs {elem_classes.join(' ')}\\" class:hide={!visible} id={elem_id}>\\n\\t{#if has_tabs}\\n\\t\\t<div class=\\"tab-wrapper\\">\\n\\t\\t\\t<div class=\\"tab-container visually-hidden\\" aria-hidden=\\"true\\">\\n\\t\\t\\t\\t{#each tabs as t, i (t.id)}\\n\\t\\t\\t\\t\\t{#if t.visible}\\n\\t\\t\\t\\t\\t\\t<button bind:this={tab_els[t.id]}>\\n\\t\\t\\t\\t\\t\\t\\t{t.label}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t</div>\\n\\t\\t\\t<div class=\\"tab-container\\" bind:this={tab_nav_el} role=\\"tablist\\">\\n\\t\\t\\t\\t{#each visible_tabs as t, i (t.id)}\\n\\t\\t\\t\\t\\t{#if t.visible}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\trole=\\"tab\\"\\n\\t\\t\\t\\t\\t\\t\\tclass:selected={t.id === $selected_tab}\\n\\t\\t\\t\\t\\t\\t\\taria-selected={t.id === $selected_tab}\\n\\t\\t\\t\\t\\t\\t\\taria-controls={t.elem_id}\\n\\t\\t\\t\\t\\t\\t\\tdisabled={!t.interactive}\\n\\t\\t\\t\\t\\t\\t\\taria-disabled={!t.interactive}\\n\\t\\t\\t\\t\\t\\t\\tid={t.elem_id ? t.elem_id + \\"-button\\" : null}\\n\\t\\t\\t\\t\\t\\t\\tdata-tab-id={t.id}\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\t\\tif (t.id !== $selected_tab) {\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tchange_tab(t.id);\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tdispatch(\\"select\\", { value: t.label, index: i });\\n\\t\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{t.label}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{/each}\\n\\t\\t\\t</div>\\n\\t\\t\\t<span\\n\\t\\t\\t\\tclass=\\"overflow-menu\\"\\n\\t\\t\\t\\tclass:hide={!is_overflowing}\\n\\t\\t\\t\\tbind:this={overflow_menu}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\ton:click|stopPropagation={() =>\\n\\t\\t\\t\\t\\t\\t(overflow_menu_open = !overflow_menu_open)}\\n\\t\\t\\t\\t\\tclass:overflow-item-selected={overflow_has_selected_tab}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<OverflowIcon />\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t<div class=\\"overflow-dropdown\\" class:hide={!overflow_menu_open}>\\n\\t\\t\\t\\t\\t{#each overflow_tabs as t}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => change_tab(t.id)}\\n\\t\\t\\t\\t\\t\\t\\tclass:selected={t.id === $selected_tab}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{t.label}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t</span>\\n\\t\\t</div>\\n\\t{/if}\\n\\t<slot />\\n</div>\\n\\n<style>\\n\\t.tabs {\\n\\t\\tposition: relative;\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tgap: var(--layout-gap);\\n\\t}\\n\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.tab-wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: space-between;\\n\\t\\tposition: relative;\\n\\t\\theight: var(--size-8);\\n\\t\\tpadding-bottom: var(--size-2);\\n\\t}\\n\\n\\t.tab-container {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\twidth: 100%;\\n\\t\\tposition: relative;\\n\\t\\toverflow: hidden;\\n\\t\\theight: var(--size-8);\\n\\t}\\n\\n\\t.tab-container::after {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 0;\\n\\t\\tleft: 0;\\n\\t\\tright: 0;\\n\\t\\theight: 1px;\\n\\t\\tbackground-color: var(--border-color-primary);\\n\\t}\\n\\n\\t.overflow-menu {\\n\\t\\tflex-shrink: 0;\\n\\t\\tmargin-left: var(--size-2);\\n\\t}\\n\\n\\tbutton {\\n\\t\\tmargin-bottom: 0;\\n\\t\\tborder: none;\\n\\t\\tborder-radius: 0;\\n\\t\\tpadding: 0 var(--size-4);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-weight: var(--section-header-text-weight);\\n\\t\\tfont-size: var(--section-header-text-size);\\n\\t\\ttransition: background-color color 0.2s ease-out;\\n\\t\\tbackground-color: transparent;\\n\\t\\theight: 100%;\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\twhite-space: nowrap;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\tbutton:disabled {\\n\\t\\topacity: 0.5;\\n\\t\\tcursor: not-allowed;\\n\\t}\\n\\n\\tbutton:hover:not(:disabled):not(.selected) {\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.selected {\\n\\t\\tbackground-color: transparent;\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t.selected::after {\\n\\t\\tcontent: \\"\\";\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 0;\\n\\t\\tleft: 0;\\n\\t\\twidth: 100%;\\n\\t\\theight: 2px;\\n\\t\\tbackground-color: var(--color-accent);\\n\\t\\tanimation: fade-grow 0.2s ease-out forwards;\\n\\t\\ttransform-origin: center;\\n\\t\\tz-index: 1;\\n\\t}\\n\\n\\t@keyframes fade-grow {\\n\\t\\tfrom {\\n\\t\\t\\topacity: 0;\\n\\t\\t\\ttransform: scaleX(0.8);\\n\\t\\t}\\n\\t\\tto {\\n\\t\\t\\topacity: 1;\\n\\t\\t\\ttransform: scaleX(1);\\n\\t\\t}\\n\\t}\\n\\n\\t.overflow-dropdown {\\n\\t\\tposition: absolute;\\n\\t\\ttop: calc(100% + var(--size-2));\\n\\t\\tright: 0;\\n\\t\\tbackground-color: var(--background-fill-primary);\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tz-index: var(--layer-5);\\n\\t\\tbox-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);\\n\\t\\tpadding: var(--size-2);\\n\\t\\tmin-width: 150px;\\n\\t\\twidth: max-content;\\n\\t}\\n\\n\\t.overflow-dropdown button {\\n\\t\\tdisplay: block;\\n\\t\\twidth: 100%;\\n\\t\\ttext-align: left;\\n\\t\\tpadding: var(--size-2);\\n\\t\\twhite-space: nowrap;\\n\\t\\toverflow: hidden;\\n\\t\\ttext-overflow: ellipsis;\\n\\t}\\n\\n\\t.overflow-menu > button {\\n\\t\\tpadding: var(--size-1) var(--size-2);\\n\\t\\tmin-width: auto;\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t}\\n\\n\\t.overflow-menu > button:hover {\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t}\\n\\n\\t.overflow-menu :global(svg) {\\n\\t\\twidth: 16px;\\n\\t\\theight: 16px;\\n\\t}\\n\\n\\t.overflow-item-selected :global(svg) {\\n\\t\\tcolor: var(--color-accent);\\n\\t}\\n\\n\\t.visually-hidden {\\n\\t\\tposition: absolute;\\n\\t\\twidth: 1px;\\n\\t\\theight: 1px;\\n\\t\\tpadding: 0;\\n\\t\\tmargin: -1px;\\n\\t\\toverflow: hidden;\\n\\t\\tclip: rect(0, 0, 0, 0);\\n\\t\\twhite-space: nowrap;\\n\\t\\tborder: 0;\\n\\t}</style>\\n"],"names":[],"mappings":"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"}`
};
const TABS = {};
const Tabs = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let has_tabs;
  let $selected_tab, $$unsubscribe_selected_tab;
  let $selected_tab_index, $$unsubscribe_selected_tab_index;
  let { visible = true } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { selected } = $$props;
  let { initial_tabs } = $$props;
  let tabs = [...initial_tabs];
  let visible_tabs = [...initial_tabs];
  let overflow_tabs = [];
  let overflow_menu_open = false;
  let overflow_menu;
  let tab_nav_el;
  const selected_tab = writable(selected || tabs[0]?.id || false);
  $$unsubscribe_selected_tab = subscribe(selected_tab, (value) => $selected_tab = value);
  const selected_tab_index = writable(tabs.findIndex((t) => t.id === selected) || 0);
  $$unsubscribe_selected_tab_index = subscribe(selected_tab_index, (value) => $selected_tab_index = value);
  const dispatch = createEventDispatcher();
  let overflow_has_selected_tab = false;
  let tab_els = {};
  setContext(TABS, {
    register_tab: (tab) => {
      let index = tabs.findIndex((t) => t.id === tab.id);
      if (index !== -1) {
        tabs[index] = { ...tabs[index], ...tab };
      } else {
        tabs = [...tabs, tab];
        index = tabs.length - 1;
      }
      if ($selected_tab === false && tab.visible && tab.interactive) {
        set_store_value(selected_tab, $selected_tab = tab.id, $selected_tab);
      }
      return index;
    },
    unregister_tab: (tab) => {
      const index = tabs.findIndex((t) => t.id === tab.id);
      if (index !== -1) {
        tabs = tabs.filter((t) => t.id !== tab.id);
        if ($selected_tab === tab.id) {
          set_store_value(selected_tab, $selected_tab = tabs[0]?.id || false, $selected_tab);
        }
      }
    },
    selected_tab,
    selected_tab_index
  });
  function change_tab(id) {
    const tab_to_activate = tabs.find((t) => t.id === id);
    if (tab_to_activate && tab_to_activate.interactive && tab_to_activate.visible && $selected_tab !== tab_to_activate.id) {
      selected = id;
      set_store_value(selected_tab, $selected_tab = id, $selected_tab);
      set_store_value(selected_tab_index, $selected_tab_index = tabs.findIndex((t) => t.id === id), $selected_tab_index);
      dispatch("change");
      overflow_menu_open = false;
    }
  }
  async function handle_menu_overflow() {
    return;
  }
  function handle_overflow_has_selected_tab(selected_tab2) {
    if (selected_tab2 === false)
      return false;
    return overflow_tabs.some((t) => t.id === selected_tab2);
  }
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  if ($$props.initial_tabs === void 0 && $$bindings.initial_tabs && initial_tabs !== void 0)
    $$bindings.initial_tabs(initial_tabs);
  $$result.css.add(css);
  has_tabs = tabs.length > 0;
  {
    selected !== null && change_tab(selected);
  }
  {
    handle_menu_overflow();
  }
  overflow_has_selected_tab = handle_overflow_has_selected_tab($selected_tab);
  $$unsubscribe_selected_tab();
  $$unsubscribe_selected_tab_index();
  return ` <div class="${[
    "tabs " + escape(elem_classes.join(" "), true) + " svelte-1tcem6n",
    !visible ? "hide" : ""
  ].join(" ").trim()}"${add_attribute("id", elem_id, 0)}>${has_tabs ? `<div class="tab-wrapper svelte-1tcem6n"><div class="tab-container visually-hidden svelte-1tcem6n" aria-hidden="true">${each(tabs, (t, i) => {
    return `${t.visible ? `<button class="svelte-1tcem6n"${add_attribute("this", tab_els[t.id], 0)}>${escape(t.label)} </button>` : ``}`;
  })}</div> <div class="tab-container svelte-1tcem6n" role="tablist"${add_attribute("this", tab_nav_el, 0)}>${each(visible_tabs, (t, i) => {
    return `${t.visible ? `<button role="tab"${add_attribute("aria-selected", t.id === $selected_tab, 0)}${add_attribute("aria-controls", t.elem_id, 0)} ${!t.interactive ? "disabled" : ""}${add_attribute("aria-disabled", !t.interactive, 0)}${add_attribute("id", t.elem_id ? t.elem_id + "-button" : null, 0)}${add_attribute("data-tab-id", t.id, 0)} class="${["svelte-1tcem6n", t.id === $selected_tab ? "selected" : ""].join(" ").trim()}">${escape(t.label)} </button>` : ``}`;
  })}</div> <span class="${["overflow-menu svelte-1tcem6n", "hide"].join(" ").trim()}"${add_attribute("this", overflow_menu, 0)}><button class="${[
    "svelte-1tcem6n",
    overflow_has_selected_tab ? "overflow-item-selected" : ""
  ].join(" ").trim()}">${validate_component(OverflowIcon, "OverflowIcon").$$render($$result, {}, {}, {})}</button> <div class="${["overflow-dropdown svelte-1tcem6n", !overflow_menu_open ? "hide" : ""].join(" ").trim()}">${each(overflow_tabs, (t) => {
    return `<button class="${["svelte-1tcem6n", t.id === $selected_tab ? "selected" : ""].join(" ").trim()}">${escape(t.label)} </button>`;
  })}</div></span></div>` : ``} ${slots.default ? slots.default({}) : ``} </div>`;
});
const Tabs$1 = Tabs;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  const dispatch = createEventDispatcher();
  let { visible = true } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { selected } = $$props;
  let { initial_tabs = [] } = $$props;
  let { gradio } = $$props;
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.selected === void 0 && $$bindings.selected && selected !== void 0)
    $$bindings.selected(selected);
  if ($$props.initial_tabs === void 0 && $$bindings.initial_tabs && initial_tabs !== void 0)
    $$bindings.initial_tabs(initial_tabs);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      dispatch("prop_change", { selected });
    }
    $$rendered = `${validate_component(Tabs$1, "Tabs").$$render(
      $$result,
      {
        visible,
        elem_id,
        elem_classes,
        initial_tabs,
        selected
      },
      {
        selected: ($$value) => {
          selected = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `${slots.default ? slots.default({}) : ``}`;
        }
      }
    )}`;
  } while (!$$settled);
  return $$rendered;
});

export { Tabs$1 as BaseTabs, TABS, Index as default };
//# sourceMappingURL=Index40-Wah-RNjO.js.map
