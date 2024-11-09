import { c as create_ssr_component, v as validate_component, d as add_styles, a as createEventDispatcher, e as escape } from './ssr-RaXq3SJh.js';
import { B as Block, e as BlockLabel, O as Code, S as Static, a8 as css_units } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$1 = {
  code: ".hide.svelte-ydeks8{display:none}",
  map: `{"version":3,"file":"HTML.svelte","sources":["HTML.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nexport let elem_classes = [];\\nexport let value;\\nexport let visible = true;\\nconst dispatch = createEventDispatcher();\\n$: value, dispatch(\\"change\\");\\n<\/script>\\n\\n<div class=\\"prose {elem_classes.join(' ')}\\" class:hide={!visible}>\\n\\t{@html value}\\n</div>\\n\\n<style>\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AAaC,mBAAM,CACL,OAAO,CAAE,IACV"}`
};
const HTML = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_classes = [] } = $$props;
  let { value } = $$props;
  let { visible = true } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  $$result.css.add(css$1);
  {
    dispatch("change");
  }
  return `<div class="${[
    "prose " + escape(elem_classes.join(" "), true) + " svelte-ydeks8",
    !visible ? "hide" : ""
  ].join(" ").trim()}"><!-- HTML_TAG_START -->${value}<!-- HTML_TAG_END --> </div>`;
});
const css = {
  code: "div.svelte-uqf9ro{transition:150ms}.pending.svelte-uqf9ro{opacity:0.2}.label-container.svelte-uqf9ro label{top:-8px !important;position:relative !important;left:-8px !important;background:var(--block-background-fill) !important;border-top:var(--block-label-border-width) solid\n			var(--border-color-primary) !important;border-left:var(--block-label-border-width) solid\n			var(--border-color-primary) !important}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">import HTML from \\"./shared/HTML.svelte\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport { Block, BlockLabel } from \\"@gradio/atoms\\";\\nimport { Code as CodeIcon } from \\"@gradio/icons\\";\\nimport { css_units } from \\"@gradio/utils\\";\\nexport let label;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = \\"\\";\\nexport let loading_status;\\nexport let gradio;\\nexport let show_label = false;\\nexport let min_height = void 0;\\nexport let max_height = void 0;\\n$: label, gradio.dispatch(\\"change\\");\\n<\/script>\\n\\n<Block {visible} {elem_id} {elem_classes} container={false}>\\n\\t{#if show_label}\\n\\t\\t<span class=\\"label-container\\">\\n\\t\\t\\t<BlockLabel Icon={CodeIcon} {show_label} {label} float={true} />\\n\\t\\t</span>\\n\\t{/if}\\n\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t\\tvariant=\\"center\\"\\n\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t/>\\n\\t<div\\n\\t\\tclass:pending={loading_status?.status === \\"pending\\"}\\n\\t\\tstyle:min-height={min_height && loading_status?.status !== \\"pending\\"\\n\\t\\t\\t? css_units(min_height)\\n\\t\\t\\t: undefined}\\n\\t\\tstyle:max-height={max_height ? css_units(max_height) : undefined}\\n\\t>\\n\\t\\t<HTML\\n\\t\\t\\t{value}\\n\\t\\t\\t{elem_classes}\\n\\t\\t\\t{visible}\\n\\t\\t\\ton:change={() => gradio.dispatch(\\"change\\")}\\n\\t\\t/>\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\tdiv {\\n\\t\\ttransition: 150ms;\\n\\t}\\n\\n\\t.pending {\\n\\t\\topacity: 0.2;\\n\\t}\\n\\n\\t.label-container :global(label) {\\n\\t\\ttop: -8px !important;\\n\\t\\tposition: relative !important;\\n\\t\\tleft: -8px !important;\\n\\t\\tbackground: var(--block-background-fill) !important;\\n\\t\\tborder-top: var(--block-label-border-width) solid\\n\\t\\t\\tvar(--border-color-primary) !important;\\n\\t\\tborder-left: var(--block-label-border-width) solid\\n\\t\\t\\tvar(--border-color-primary) !important;\\n\\t}</style>\\n"],"names":[],"mappings":"AAiDC,iBAAI,CACH,UAAU,CAAE,KACb,CAEA,sBAAS,CACR,OAAO,CAAE,GACV,CAEA,8BAAgB,CAAS,KAAO,CAC/B,GAAG,CAAE,IAAI,CAAC,UAAU,CACpB,QAAQ,CAAE,QAAQ,CAAC,UAAU,CAC7B,IAAI,CAAE,IAAI,CAAC,UAAU,CACrB,UAAU,CAAE,IAAI,uBAAuB,CAAC,CAAC,UAAU,CACnD,UAAU,CAAE,IAAI,0BAA0B,CAAC,CAAC,KAAK;AACnD,GAAG,IAAI,sBAAsB,CAAC,CAAC,UAAU,CACvC,WAAW,CAAE,IAAI,0BAA0B,CAAC,CAAC,KAAK;AACpD,GAAG,IAAI,sBAAsB,CAAC,CAAC,UAC9B"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = "" } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  let { show_label = false } = $$props;
  let { min_height = void 0 } = $$props;
  let { max_height = void 0 } = $$props;
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
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
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.max_height === void 0 && $$bindings.max_height && max_height !== void 0)
    $$bindings.max_height(max_height);
  $$result.css.add(css);
  {
    gradio.dispatch("change");
  }
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      elem_id,
      elem_classes,
      container: false
    },
    {},
    {
      default: () => {
        return `${show_label ? `<span class="label-container svelte-uqf9ro">${validate_component(BlockLabel, "BlockLabel").$$render(
          $$result,
          {
            Icon: Code,
            show_label,
            label,
            float: true
          },
          {},
          {}
        )}</span>` : ``} ${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status, { variant: "center" }), {}, {})} <div class="${["svelte-uqf9ro", loading_status?.status === "pending" ? "pending" : ""].join(" ").trim()}"${add_styles({
          "min-height": min_height && loading_status?.status !== "pending" ? css_units(min_height) : void 0,
          "max-height": max_height ? css_units(max_height) : void 0
        })}>${validate_component(HTML, "HTML").$$render($$result, { value, elem_classes, visible }, {}, {})}</div>`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index44-CoAG2qqz.js.map
