import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, e as escape, b as add_attribute, d as add_styles, h as merge_ssr_styles } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, a8 as css_units, h as IconButtonWrapper, i as IconButton, H as Copy, G as MarkdownCode } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example16-CojSOHBb.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css$1 = {
  code: "div.svelte-lag733 .math.inline{fill:var(--body-text-color);display:inline-block;vertical-align:middle;padding:var(--size-1-5) -var(--size-1);color:var(--body-text-color)}div.svelte-lag733 .math.inline svg{display:inline;margin-bottom:0.22em}div.svelte-lag733{max-width:100%}.hide.svelte-lag733{display:none}",
  map: '{"version":3,"file":"Markdown.svelte","sources":["Markdown.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport { copy, css_units } from \\"@gradio/utils\\";\\nimport { Copy, Check } from \\"@gradio/icons\\";\\nimport { IconButton, IconButtonWrapper } from \\"@gradio/atoms\\";\\nimport { MarkdownCode } from \\"@gradio/markdown-code\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value;\\nexport let min_height = void 0;\\nexport let rtl = false;\\nexport let sanitize_html = true;\\nexport let line_breaks = false;\\nexport let latex_delimiters;\\nexport let header_links = false;\\nexport let height = void 0;\\nexport let show_copy_button = false;\\nexport let root;\\nexport let loading_status = void 0;\\nlet copied = false;\\nlet timer;\\nconst dispatch = createEventDispatcher();\\n$: value, dispatch(\\"change\\");\\nasync function handle_copy() {\\n    if (\\"clipboard\\" in navigator) {\\n        await navigator.clipboard.writeText(value);\\n        copy_feedback();\\n    }\\n}\\nfunction copy_feedback() {\\n    copied = true;\\n    if (timer)\\n        clearTimeout(timer);\\n    timer = setTimeout(() => {\\n        copied = false;\\n    }, 1e3);\\n}\\n<\/script>\\n\\n<div\\n\\tclass=\\"prose {elem_classes.join(\' \')}\\"\\n\\tclass:hide={!visible}\\n\\tdata-testid=\\"markdown\\"\\n\\tdir={rtl ? \\"rtl\\" : \\"ltr\\"}\\n\\tuse:copy\\n\\tstyle={height ? `max-height: ${css_units(height)}; overflow-y: auto;` : \\"\\"}\\n\\tstyle:min-height={min_height && loading_status?.status !== \\"pending\\"\\n\\t\\t? css_units(min_height)\\n\\t\\t: undefined}\\n>\\n\\t{#if show_copy_button}\\n\\t\\t<IconButtonWrapper>\\n\\t\\t\\t<IconButton\\n\\t\\t\\t\\tIcon={copied ? Check : Copy}\\n\\t\\t\\t\\ton:click={handle_copy}\\n\\t\\t\\t\\tlabel={copied ? \\"Copied conversation\\" : \\"Copy conversation\\"}\\n\\t\\t\\t></IconButton>\\n\\t\\t</IconButtonWrapper>\\n\\t{/if}\\n\\t<MarkdownCode\\n\\t\\tmessage={value}\\n\\t\\t{latex_delimiters}\\n\\t\\t{sanitize_html}\\n\\t\\t{line_breaks}\\n\\t\\tchatbot={false}\\n\\t\\t{header_links}\\n\\t\\t{root}\\n\\t/>\\n</div>\\n\\n<style>\\n\\tdiv :global(.math.inline) {\\n\\t\\tfill: var(--body-text-color);\\n\\t\\tdisplay: inline-block;\\n\\t\\tvertical-align: middle;\\n\\t\\tpadding: var(--size-1-5) -var(--size-1);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\tdiv :global(.math.inline svg) {\\n\\t\\tdisplay: inline;\\n\\t\\tmargin-bottom: 0.22em;\\n\\t}\\n\\n\\tdiv {\\n\\t\\tmax-width: 100%;\\n\\t}\\n\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AAsEC,iBAAG,CAAS,YAAc,CACzB,IAAI,CAAE,IAAI,iBAAiB,CAAC,CAC5B,OAAO,CAAE,YAAY,CACrB,cAAc,CAAE,MAAM,CACtB,OAAO,CAAE,IAAI,UAAU,CAAC,CAAC,KAAK,QAAQ,CAAC,CACvC,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,iBAAG,CAAS,gBAAkB,CAC7B,OAAO,CAAE,MAAM,CACf,aAAa,CAAE,MAChB,CAEA,iBAAI,CACH,SAAS,CAAE,IACZ,CAEA,mBAAM,CACL,OAAO,CAAE,IACV"}'
};
const Markdown = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { min_height = void 0 } = $$props;
  let { rtl = false } = $$props;
  let { sanitize_html = true } = $$props;
  let { line_breaks = false } = $$props;
  let { latex_delimiters } = $$props;
  let { header_links = false } = $$props;
  let { height = void 0 } = $$props;
  let { show_copy_button = false } = $$props;
  let { root } = $$props;
  let { loading_status = void 0 } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.sanitize_html === void 0 && $$bindings.sanitize_html && sanitize_html !== void 0)
    $$bindings.sanitize_html(sanitize_html);
  if ($$props.line_breaks === void 0 && $$bindings.line_breaks && line_breaks !== void 0)
    $$bindings.line_breaks(line_breaks);
  if ($$props.latex_delimiters === void 0 && $$bindings.latex_delimiters && latex_delimiters !== void 0)
    $$bindings.latex_delimiters(latex_delimiters);
  if ($$props.header_links === void 0 && $$bindings.header_links && header_links !== void 0)
    $$bindings.header_links(header_links);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  $$result.css.add(css$1);
  {
    dispatch("change");
  }
  return `<div class="${[
    "prose " + escape(elem_classes.join(" "), true) + " svelte-lag733",
    !visible ? "hide" : ""
  ].join(" ").trim()}" data-testid="markdown"${add_attribute("dir", rtl ? "rtl" : "ltr", 0)}${add_styles(merge_ssr_styles(
    escape(
      height ? `max-height: ${css_units(height)}; overflow-y: auto;` : "",
      true
    ),
    {
      "min-height": min_height && loading_status?.status !== "pending" ? css_units(min_height) : void 0
    }
  ))}>${show_copy_button ? `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Copy,
          label: "Copy conversation"
        },
        {},
        {}
      )}`;
    }
  })}` : ``} ${validate_component(MarkdownCode, "MarkdownCode").$$render(
    $$result,
    {
      message: value,
      latex_delimiters,
      sanitize_html,
      line_breaks,
      chatbot: false,
      header_links,
      root
    },
    {},
    {}
  )} </div>`;
});
const Markdown$1 = Markdown;
const css = {
  code: "div.svelte-1ed2p3z{transition:150ms}.pending.svelte-1ed2p3z{opacity:0.2}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script context=\\"module\\" lang=\\"ts\\">export { default as BaseMarkdown } from \\"./shared/Markdown.svelte\\";\\nexport { default as BaseExample } from \\"./Example.svelte\\";\\n<\/script>\\n\\n<script lang=\\"ts\\">import Markdown from \\"./shared/Markdown.svelte\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport { Block } from \\"@gradio/atoms\\";\\nexport let label;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = \\"\\";\\nexport let loading_status;\\nexport let rtl = false;\\nexport let sanitize_html = true;\\nexport let line_breaks = false;\\nexport let gradio;\\nexport let latex_delimiters;\\nexport let header_links = false;\\nexport let height;\\nexport let min_height;\\nexport let max_height;\\nexport let show_copy_button = false;\\nexport let container = false;\\n$: label, gradio.dispatch(\\"change\\");\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{container}\\n\\tallow_overflow={true}\\n\\toverflow_behavior=\\"auto\\"\\n\\t{height}\\n\\t{min_height}\\n\\t{max_height}\\n>\\n\\t<StatusTracker\\n\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\ti18n={gradio.i18n}\\n\\t\\t{...loading_status}\\n\\t\\tvariant=\\"center\\"\\n\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t/>\\n\\t<div class:pending={loading_status?.status === \\"pending\\"}>\\n\\t\\t<Markdown\\n\\t\\t\\t{value}\\n\\t\\t\\t{elem_classes}\\n\\t\\t\\t{visible}\\n\\t\\t\\t{rtl}\\n\\t\\t\\ton:change={() => gradio.dispatch(\\"change\\")}\\n\\t\\t\\t{latex_delimiters}\\n\\t\\t\\t{sanitize_html}\\n\\t\\t\\t{line_breaks}\\n\\t\\t\\t{header_links}\\n\\t\\t\\t{show_copy_button}\\n\\t\\t\\troot={gradio.root}\\n\\t\\t\\t{loading_status}\\n\\t\\t/>\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\tdiv {\\n\\t\\ttransition: 150ms;\\n\\t}\\n\\n\\t.pending {\\n\\t\\topacity: 0.2;\\n\\t}</style>\\n"],"names":[],"mappings":"AAgEC,kBAAI,CACH,UAAU,CAAE,KACb,CAEA,uBAAS,CACR,OAAO,CAAE,GACV"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { label } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = "" } = $$props;
  let { loading_status } = $$props;
  let { rtl = false } = $$props;
  let { sanitize_html = true } = $$props;
  let { line_breaks = false } = $$props;
  let { gradio } = $$props;
  let { latex_delimiters } = $$props;
  let { header_links = false } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let { show_copy_button = false } = $$props;
  let { container = false } = $$props;
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
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.sanitize_html === void 0 && $$bindings.sanitize_html && sanitize_html !== void 0)
    $$bindings.sanitize_html(sanitize_html);
  if ($$props.line_breaks === void 0 && $$bindings.line_breaks && line_breaks !== void 0)
    $$bindings.line_breaks(line_breaks);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.latex_delimiters === void 0 && $$bindings.latex_delimiters && latex_delimiters !== void 0)
    $$bindings.latex_delimiters(latex_delimiters);
  if ($$props.header_links === void 0 && $$bindings.header_links && header_links !== void 0)
    $$bindings.header_links(header_links);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.max_height === void 0 && $$bindings.max_height && max_height !== void 0)
    $$bindings.max_height(max_height);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
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
      container,
      allow_overflow: true,
      overflow_behavior: "auto",
      height,
      min_height,
      max_height
    },
    {},
    {
      default: () => {
        return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status, { variant: "center" }), {}, {})} <div class="${["svelte-1ed2p3z", loading_status?.status === "pending" ? "pending" : ""].join(" ").trim()}">${validate_component(Markdown$1, "Markdown").$$render(
          $$result,
          {
            value,
            elem_classes,
            visible,
            rtl,
            latex_delimiters,
            sanitize_html,
            line_breaks,
            header_links,
            show_copy_button,
            root: gradio.root,
            loading_status
          },
          {},
          {}
        )}</div>`;
      }
    }
  )}`;
});

export { Markdown$1 as BaseMarkdown, Index as default };
//# sourceMappingURL=Index45-B3kKVQQ5.js.map
