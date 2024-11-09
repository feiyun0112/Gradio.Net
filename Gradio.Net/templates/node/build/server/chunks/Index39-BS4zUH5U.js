import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, g as getContext, s as subscribe, b as add_attribute, e as escape, d as add_styles } from './ssr-RaXq3SJh.js';
import { t as tick } from './Component-Dv7eSVA_.js';
import { TABS } from './Index40-Wah-RNjO.js';
import { I as Index$1 } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';

const css = {
  code: "div.svelte-tcemt9{display:flex;position:relative;border:none;border-radius:var(--radius-sm);padding:var(--block-padding);width:100%;box-sizing:border-box}",
  map: `{"version":3,"file":"TabItem.svelte","sources":["TabItem.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { getContext, onMount, createEventDispatcher, tick } from \\"svelte\\";\\nimport { TABS } from \\"@gradio/tabs\\";\\nimport Column from \\"@gradio/column\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let label;\\nexport let id = {};\\nexport let visible;\\nexport let interactive;\\nconst dispatch = createEventDispatcher();\\nconst { register_tab, unregister_tab, selected_tab, selected_tab_index } = getContext(TABS);\\nlet tab_index;\\n$: tab_index = register_tab({ label, id, elem_id, visible, interactive });\\nonMount(() => {\\n    return () => unregister_tab({ label, id, elem_id });\\n});\\n$: $selected_tab_index === tab_index && tick().then(() => dispatch(\\"select\\", { value: label, index: tab_index }));\\n<\/script>\\n\\n<div\\n\\tid={elem_id}\\n\\tclass=\\"tabitem {elem_classes.join(' ')}\\"\\n\\tstyle:display={$selected_tab === id && visible ? \\"block\\" : \\"none\\"}\\n\\trole=\\"tabpanel\\"\\n>\\n\\t<Column>\\n\\t\\t<slot />\\n\\t</Column>\\n</div>\\n\\n<style>\\n\\tdiv {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tborder: none;\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tpadding: var(--block-padding);\\n\\t\\twidth: 100%;\\n\\t\\tbox-sizing: border-box;\\n\\t}</style>\\n"],"names":[],"mappings":"AA+BC,iBAAI,CACH,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,MAAM,CAAE,IAAI,CACZ,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,OAAO,CAAE,IAAI,eAAe,CAAC,CAC7B,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,UACb"}`
};
const TabItem = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let $selected_tab_index, $$unsubscribe_selected_tab_index;
  let $selected_tab, $$unsubscribe_selected_tab;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { label } = $$props;
  let { id = {} } = $$props;
  let { visible } = $$props;
  let { interactive } = $$props;
  const dispatch = createEventDispatcher();
  const { register_tab, unregister_tab, selected_tab, selected_tab_index } = getContext(TABS);
  $$unsubscribe_selected_tab = subscribe(selected_tab, (value) => $selected_tab = value);
  $$unsubscribe_selected_tab_index = subscribe(selected_tab_index, (value) => $selected_tab_index = value);
  let tab_index;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.id === void 0 && $$bindings.id && id !== void 0)
    $$bindings.id(id);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  $$result.css.add(css);
  tab_index = register_tab({ label, id, elem_id, visible, interactive });
  $selected_tab_index === tab_index && tick().then(() => dispatch("select", { value: label, index: tab_index }));
  $$unsubscribe_selected_tab_index();
  $$unsubscribe_selected_tab();
  return `<div${add_attribute("id", elem_id, 0)} class="${"tabitem " + escape(elem_classes.join(" "), true) + " svelte-tcemt9"}" role="tabpanel"${add_styles({
    "display": $selected_tab === id && visible ? "block" : "none"
  })}>${validate_component(Index$1, "Column").$$render($$result, {}, {}, {
    default: () => {
      return `${slots.default ? slots.default({}) : ``}`;
    }
  })} </div>`;
});
const TabItem$1 = TabItem;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { label } = $$props;
  let { id } = $$props;
  let { gradio } = $$props;
  let { visible = true } = $$props;
  let { interactive = true } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.id === void 0 && $$bindings.id && id !== void 0)
    $$bindings.id(id);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  return `${validate_component(TabItem$1, "TabItem").$$render(
    $$result,
    {
      elem_id,
      elem_classes,
      label,
      visible,
      interactive,
      id
    },
    {},
    {
      default: () => {
        return `${slots.default ? slots.default({}) : ``}`;
      }
    }
  )}`;
});

export { TabItem$1 as BaseTabItem, Index as default };
//# sourceMappingURL=Index39-BS4zUH5U.js.map
