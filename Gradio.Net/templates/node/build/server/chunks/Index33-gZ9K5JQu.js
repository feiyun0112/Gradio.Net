import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, e as escape, d as add_styles } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, I as Index$1 } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: "span.svelte-1w6vloh{font-weight:var(--section-header-text-weight);font-size:var(--section-header-text-size)}.label-wrap.svelte-1w6vloh{display:flex;justify-content:space-between;cursor:pointer;width:var(--size-full);color:var(--accordion-text-color)}.label-wrap.open.svelte-1w6vloh{margin-bottom:var(--size-2)}.icon.svelte-1w6vloh{transition:150ms}",
  map: '{"version":3,"file":"Accordion.svelte","sources":["Accordion.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nconst dispatch = createEventDispatcher();\\nexport let open = true;\\nexport let label = \\"\\";\\n<\/script>\\n\\n<button\\n\\ton:click={() => {\\n\\t\\topen = !open;\\n\\t\\tif (open) {\\n\\t\\t\\tdispatch(\\"expand\\");\\n\\t\\t} else {\\n\\t\\t\\tdispatch(\\"collapse\\");\\n\\t\\t}\\n\\t}}\\n\\tclass=\\"label-wrap\\"\\n\\tclass:open\\n>\\n\\t<span>{label}</span>\\n\\t<span style:transform={open ? \\"rotate(0)\\" : \\"rotate(90deg)\\"} class=\\"icon\\">\\n\\t\\t▼\\n\\t</span>\\n</button>\\n<div style:display={open ? \\"block\\" : \\"none\\"}>\\n\\t<slot />\\n</div>\\n\\n<style>\\n\\tspan {\\n\\t\\tfont-weight: var(--section-header-text-weight);\\n\\t\\tfont-size: var(--section-header-text-size);\\n\\t}\\n\\t.label-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\tcursor: pointer;\\n\\t\\twidth: var(--size-full);\\n\\t\\tcolor: var(--accordion-text-color);\\n\\t}\\n\\t.label-wrap.open {\\n\\t\\tmargin-bottom: var(--size-2);\\n\\t}\\n\\n\\t.icon {\\n\\t\\ttransition: 150ms;\\n\\t}</style>\\n"],"names":[],"mappings":"AA4BC,mBAAK,CACJ,WAAW,CAAE,IAAI,4BAA4B,CAAC,CAC9C,SAAS,CAAE,IAAI,0BAA0B,CAC1C,CACA,0BAAY,CACX,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,MAAM,CAAE,OAAO,CACf,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,KAAK,CAAE,IAAI,sBAAsB,CAClC,CACA,WAAW,oBAAM,CAChB,aAAa,CAAE,IAAI,QAAQ,CAC5B,CAEA,oBAAM,CACL,UAAU,CAAE,KACb"}'
};
const Accordion = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  createEventDispatcher();
  let { open = true } = $$props;
  let { label = "" } = $$props;
  if ($$props.open === void 0 && $$bindings.open && open !== void 0)
    $$bindings.open(open);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  $$result.css.add(css);
  return `<button class="${["label-wrap svelte-1w6vloh", open ? "open" : ""].join(" ").trim()}"><span class="svelte-1w6vloh">${escape(label)}</span> <span class="icon svelte-1w6vloh"${add_styles({
    "transform": open ? "rotate(0)" : "rotate(90deg)"
  })} data-svelte-h="svelte-1mqwc8d">▼</span></button> <div${add_styles({ "display": open ? "block" : "none" })}>${slots.default ? slots.default({}) : ``} </div>`;
});
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label } = $$props;
  let { elem_id } = $$props;
  let { elem_classes } = $$props;
  let { visible = true } = $$props;
  let { open = true } = $$props;
  let { loading_status } = $$props;
  let { gradio } = $$props;
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.open === void 0 && $$bindings.open && open !== void 0)
    $$bindings.open(open);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `${validate_component(Block, "Block").$$render($$result, { elem_id, elem_classes, visible }, {}, {
      default: () => {
        return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(Accordion, "Accordion").$$render(
          $$result,
          { label, open },
          {
            open: ($$value) => {
              open = $$value;
              $$settled = false;
            }
          },
          {
            default: () => {
              return `${validate_component(Index$1, "Column").$$render($$result, {}, {}, {
                default: () => {
                  return `${slots.default ? slots.default({}) : ``}`;
                }
              })}`;
            }
          }
        )}`;
      }
    })}`;
  } while (!$$settled);
  return $$rendered;
});

export { Index as default };
//# sourceMappingURL=Index33-gZ9K5JQu.js.map
