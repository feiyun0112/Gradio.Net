import { c as create_ssr_component, v as validate_component, e as escape, f as each, m as missing_component, n as null_to_empty } from './ssr-RaXq3SJh.js';
import { B as Block, Q as Example } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const css = {
  code: ".wrap.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{display:inline-block;width:var(--size-full);max-width:var(--size-full);color:var(--body-text-color)}.hide.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{display:none}.label.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{display:flex;align-items:center;margin-bottom:var(--size-2);color:var(--block-label-text-color);font-weight:var(--block-label-text-weight);font-size:var(--block-label-text-size);line-height:var(--line-sm)}svg.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{margin-right:var(--size-1)}.gallery.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{display:flex;flex-wrap:wrap;gap:var(--spacing-lg)}.gallery-item.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{border:1px solid var(--border-color-primary);border-radius:var(--button-large-radius);overflow:hidden}.gallery-item.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i:hover{border-color:var(--border-color-accent);background:var(--table-row-focus)}.table-wrap.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{border:1px solid var(--border-color-primary);border-radius:var(--table-radius);width:var(--size-full);table-layout:auto;overflow-x:auto;line-height:var(--line-sm);color:var(--table-text-color)}table.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{width:var(--size-full)}.tr-head.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{box-shadow:var(--shadow-drop-lg);border-bottom:1px solid var(--border-color-primary)}.tr-head.svelte-p5q82i>.svelte-p5q82i+.svelte-p5q82i{border-right-width:0px;border-left-width:1px;border-color:var(--border-color-primary)}th.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{padding:var(--size-2);white-space:nowrap}.tr-body.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{cursor:pointer;border-bottom:1px solid var(--border-color-primary);background:var(--table-even-background-fill)}.tr-body.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i:last-child{border:none}.tr-body.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i:nth-child(odd){background:var(--table-odd-background-fill)}.tr-body.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i:hover{background:var(--table-row-focus)}.tr-body.svelte-p5q82i>.svelte-p5q82i+.svelte-p5q82i{border-right-width:0px;border-left-width:1px;border-color:var(--border-color-primary)}.tr-body.svelte-p5q82i:hover>.svelte-p5q82i+.svelte-p5q82i{border-color:var(--border-color-accent)}td.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{padding:var(--size-2);text-align:center}.paginate.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{display:flex;justify-content:center;align-items:center;gap:var(--spacing-sm);margin-top:var(--size-2);color:var(--block-label-text-color);font-size:var(--text-sm)}button.current-page.svelte-p5q82i.svelte-p5q82i.svelte-p5q82i{font-weight:var(--weight-bold)}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { Block } from \\"@gradio/atoms\\";\\nimport { BaseExample } from \\"@gradio/textbox\\";\\nexport let components;\\nexport let component_props;\\nexport let component_map;\\nexport let label = \\"Examples\\";\\nexport let headers;\\nexport let samples = null;\\nlet old_samples = null;\\nexport let sample_labels = null;\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = null;\\nexport let root;\\nexport let proxy_url;\\nexport let samples_per_page = 10;\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let gradio;\\nlet samples_dir = proxy_url ? `/proxy=${proxy_url}file=` : `${root}/file=`;\\nlet page = 0;\\n$: gallery = components.length < 2 || sample_labels !== null;\\nlet paginate = samples ? samples.length > samples_per_page : false;\\nlet selected_samples;\\nlet page_count;\\nlet visible_pages = [];\\nlet current_hover = -1;\\nfunction handle_mouseenter(i) {\\n    current_hover = i;\\n}\\nfunction handle_mouseleave() {\\n    current_hover = -1;\\n}\\n$: {\\n    if (sample_labels) {\\n        samples = sample_labels.map((e) => [e]);\\n    }\\n    else if (!samples) {\\n        samples = [];\\n    }\\n    if (samples !== old_samples) {\\n        page = 0;\\n        old_samples = samples;\\n    }\\n    paginate = samples.length > samples_per_page;\\n    if (paginate) {\\n        visible_pages = [];\\n        selected_samples = samples.slice(page * samples_per_page, (page + 1) * samples_per_page);\\n        page_count = Math.ceil(samples.length / samples_per_page);\\n        [0, page, page_count - 1].forEach((anchor) => {\\n            for (let i = anchor - 2; i <= anchor + 2; i++) {\\n                if (i >= 0 && i < page_count && !visible_pages.includes(i)) {\\n                    if (visible_pages.length > 0 && i - visible_pages[visible_pages.length - 1] > 1) {\\n                        visible_pages.push(-1);\\n                    }\\n                    visible_pages.push(i);\\n                }\\n            }\\n        });\\n    }\\n    else {\\n        selected_samples = samples.slice();\\n    }\\n}\\nlet component_meta = [];\\nasync function get_component_meta(selected_samples2) {\\n    component_meta = await Promise.all(selected_samples2 && selected_samples2.map(async (sample_row) => await Promise.all(sample_row.map(async (sample_cell, j) => {\\n        return {\\n            value: sample_cell,\\n            component: (await component_map.get(components[j]))?.default\\n        };\\n    }))));\\n}\\n$: component_map, get_component_meta(selected_samples);\\n<\/script>\\n\\n<Block\\n\\t{visible}\\n\\tpadding={false}\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{scale}\\n\\t{min_width}\\n\\tallow_overflow={false}\\n\\tcontainer={false}\\n>\\n\\t<div class=\\"label\\">\\n\\t\\t<svg\\n\\t\\t\\txmlns=\\"http://www.w3.org/2000/svg\\"\\n\\t\\t\\txmlns:xlink=\\"http://www.w3.org/1999/xlink\\"\\n\\t\\t\\taria-hidden=\\"true\\"\\n\\t\\t\\trole=\\"img\\"\\n\\t\\t\\twidth=\\"1em\\"\\n\\t\\t\\theight=\\"1em\\"\\n\\t\\t\\tpreserveAspectRatio=\\"xMidYMid meet\\"\\n\\t\\t\\tviewBox=\\"0 0 32 32\\"\\n\\t\\t>\\n\\t\\t\\t<path\\n\\t\\t\\t\\tfill=\\"currentColor\\"\\n\\t\\t\\t\\td=\\"M10 6h18v2H10zm0 18h18v2H10zm0-9h18v2H10zm-6 0h2v2H4zm0-9h2v2H4zm0 18h2v2H4z\\"\\n\\t\\t\\t/>\\n\\t\\t</svg>\\n\\t\\t{label}\\n\\t</div>\\n\\t{#if gallery}\\n\\t\\t<div class=\\"gallery\\">\\n\\t\\t\\t{#each selected_samples as sample_row, i}\\n\\t\\t\\t\\t{#if sample_row[0]}\\n\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\tclass=\\"gallery-item\\"\\n\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\tvalue = i + page * samples_per_page;\\n\\t\\t\\t\\t\\t\\t\\tgradio.dispatch(\\"click\\", value);\\n\\t\\t\\t\\t\\t\\t\\tgradio.dispatch(\\"select\\", { index: value, value: sample_row });\\n\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\ton:mouseenter={() => handle_mouseenter(i)}\\n\\t\\t\\t\\t\\t\\ton:mouseleave={() => handle_mouseleave()}\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t{#if sample_labels}\\n\\t\\t\\t\\t\\t\\t\\t<BaseExample\\n\\t\\t\\t\\t\\t\\t\\t\\tvalue={sample_row[0]}\\n\\t\\t\\t\\t\\t\\t\\t\\tselected={current_hover === i}\\n\\t\\t\\t\\t\\t\\t\\t\\ttype=\\"gallery\\"\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{:else if component_meta.length && component_map.get(components[0])}\\n\\t\\t\\t\\t\\t\\t\\t<svelte:component\\n\\t\\t\\t\\t\\t\\t\\t\\tthis={component_meta[0][0].component}\\n\\t\\t\\t\\t\\t\\t\\t\\t{...component_props[0]}\\n\\t\\t\\t\\t\\t\\t\\t\\tvalue={sample_row[0]}\\n\\t\\t\\t\\t\\t\\t\\t\\t{samples_dir}\\n\\t\\t\\t\\t\\t\\t\\t\\ttype=\\"gallery\\"\\n\\t\\t\\t\\t\\t\\t\\t\\tselected={current_hover === i}\\n\\t\\t\\t\\t\\t\\t\\t\\tindex={i}\\n\\t\\t\\t\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t<div class=\\"table-wrap\\">\\n\\t\\t\\t<table tabindex=\\"0\\" role=\\"grid\\">\\n\\t\\t\\t\\t<thead>\\n\\t\\t\\t\\t\\t<tr class=\\"tr-head\\">\\n\\t\\t\\t\\t\\t\\t{#each headers as header}\\n\\t\\t\\t\\t\\t\\t\\t<th>\\n\\t\\t\\t\\t\\t\\t\\t\\t{header}\\n\\t\\t\\t\\t\\t\\t\\t</th>\\n\\t\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t\\t</tr>\\n\\t\\t\\t\\t</thead>\\n\\t\\t\\t\\t<tbody>\\n\\t\\t\\t\\t\\t{#each component_meta as sample_row, i}\\n\\t\\t\\t\\t\\t\\t<tr\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"tr-body\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\t\\t\\tvalue = i + page * samples_per_page;\\n\\t\\t\\t\\t\\t\\t\\t\\tgradio.dispatch(\\"click\\", value);\\n\\t\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t\\t\\ton:mouseenter={() => handle_mouseenter(i)}\\n\\t\\t\\t\\t\\t\\t\\ton:mouseleave={() => handle_mouseleave()}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{#each sample_row as { value, component }, j}\\n\\t\\t\\t\\t\\t\\t\\t\\t{@const component_name = components[j]}\\n\\t\\t\\t\\t\\t\\t\\t\\t{#if component_name !== undefined && component_map.get(component_name) !== undefined}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t<td\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tstyle=\\"max-width: {component_name === \'textbox\'\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t? \'35ch\'\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t: \'auto\'}\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tclass={component_name}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t<svelte:component\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tthis={component}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{...component_props[j]}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{samples_dir}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\ttype=\\"table\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tselected={current_hover === i}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tindex={i}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t</td>\\n\\t\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t\\t\\t</tr>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</tbody>\\n\\t\\t\\t</table>\\n\\t\\t</div>\\n\\t{/if}\\n\\t{#if paginate}\\n\\t\\t<div class=\\"paginate\\">\\n\\t\\t\\tPages:\\n\\t\\t\\t{#each visible_pages as visible_page}\\n\\t\\t\\t\\t{#if visible_page === -1}\\n\\t\\t\\t\\t\\t<div>...</div>\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\tclass:current-page={page === visible_page}\\n\\t\\t\\t\\t\\t\\ton:click={() => (page = visible_page)}\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t{visible_page + 1}\\n\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t{/each}\\n\\t\\t</div>\\n\\t{/if}\\n</Block>\\n\\n<style>\\n\\t.wrap {\\n\\t\\tdisplay: inline-block;\\n\\t\\twidth: var(--size-full);\\n\\t\\tmax-width: var(--size-full);\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\n\\t.hide {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.label {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tmargin-bottom: var(--size-2);\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t\\tfont-weight: var(--block-label-text-weight);\\n\\t\\tfont-size: var(--block-label-text-size);\\n\\t\\tline-height: var(--line-sm);\\n\\t}\\n\\n\\tsvg {\\n\\t\\tmargin-right: var(--size-1);\\n\\t}\\n\\n\\t.gallery {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tgap: var(--spacing-lg);\\n\\t}\\n\\n\\t.gallery-item {\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--button-large-radius);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.gallery-item:hover {\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t\\tbackground: var(--table-row-focus);\\n\\t}\\n\\n\\t.table-wrap {\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--table-radius);\\n\\t\\twidth: var(--size-full);\\n\\t\\ttable-layout: auto;\\n\\t\\toverflow-x: auto;\\n\\t\\tline-height: var(--line-sm);\\n\\t\\tcolor: var(--table-text-color);\\n\\t}\\n\\ttable {\\n\\t\\twidth: var(--size-full);\\n\\t}\\n\\n\\t.tr-head {\\n\\t\\tbox-shadow: var(--shadow-drop-lg);\\n\\t\\tborder-bottom: 1px solid var(--border-color-primary);\\n\\t}\\n\\n\\t.tr-head > * + * {\\n\\t\\tborder-right-width: 0px;\\n\\t\\tborder-left-width: 1px;\\n\\t\\tborder-color: var(--border-color-primary);\\n\\t}\\n\\n\\tth {\\n\\t\\tpadding: var(--size-2);\\n\\t\\twhite-space: nowrap;\\n\\t}\\n\\n\\t.tr-body {\\n\\t\\tcursor: pointer;\\n\\t\\tborder-bottom: 1px solid var(--border-color-primary);\\n\\t\\tbackground: var(--table-even-background-fill);\\n\\t}\\n\\n\\t.tr-body:last-child {\\n\\t\\tborder: none;\\n\\t}\\n\\n\\t.tr-body:nth-child(odd) {\\n\\t\\tbackground: var(--table-odd-background-fill);\\n\\t}\\n\\n\\t.tr-body:hover {\\n\\t\\tbackground: var(--table-row-focus);\\n\\t}\\n\\n\\t.tr-body > * + * {\\n\\t\\tborder-right-width: 0px;\\n\\t\\tborder-left-width: 1px;\\n\\t\\tborder-color: var(--border-color-primary);\\n\\t}\\n\\n\\t.tr-body:hover > * + * {\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t}\\n\\n\\ttd {\\n\\t\\tpadding: var(--size-2);\\n\\t\\ttext-align: center;\\n\\t}\\n\\n\\t.paginate {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tgap: var(--spacing-sm);\\n\\t\\tmargin-top: var(--size-2);\\n\\t\\tcolor: var(--block-label-text-color);\\n\\t\\tfont-size: var(--text-sm);\\n\\t}\\n\\n\\tbutton.current-page {\\n\\t\\tfont-weight: var(--weight-bold);\\n\\t}</style>\\n"],"names":[],"mappings":"AAoNC,+CAAM,CACL,OAAO,CAAE,YAAY,CACrB,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,SAAS,CAAE,IAAI,WAAW,CAAC,CAC3B,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CAEA,+CAAM,CACL,OAAO,CAAE,IACV,CAEA,gDAAO,CACN,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,aAAa,CAAE,IAAI,QAAQ,CAAC,CAC5B,KAAK,CAAE,IAAI,wBAAwB,CAAC,CACpC,WAAW,CAAE,IAAI,yBAAyB,CAAC,CAC3C,SAAS,CAAE,IAAI,uBAAuB,CAAC,CACvC,WAAW,CAAE,IAAI,SAAS,CAC3B,CAEA,6CAAI,CACH,YAAY,CAAE,IAAI,QAAQ,CAC3B,CAEA,kDAAS,CACR,OAAO,CAAE,IAAI,CACb,SAAS,CAAE,IAAI,CACf,GAAG,CAAE,IAAI,YAAY,CACtB,CAEA,uDAAc,CACb,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAC7C,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,QAAQ,CAAE,MACX,CAEA,uDAAa,MAAO,CACnB,YAAY,CAAE,IAAI,qBAAqB,CAAC,CACxC,UAAU,CAAE,IAAI,iBAAiB,CAClC,CAEA,qDAAY,CACX,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAC7C,aAAa,CAAE,IAAI,cAAc,CAAC,CAClC,KAAK,CAAE,IAAI,WAAW,CAAC,CACvB,YAAY,CAAE,IAAI,CAClB,UAAU,CAAE,IAAI,CAChB,WAAW,CAAE,IAAI,SAAS,CAAC,CAC3B,KAAK,CAAE,IAAI,kBAAkB,CAC9B,CACA,+CAAM,CACL,KAAK,CAAE,IAAI,WAAW,CACvB,CAEA,kDAAS,CACR,UAAU,CAAE,IAAI,gBAAgB,CAAC,CACjC,aAAa,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CACpD,CAEA,sBAAQ,CAAG,cAAC,CAAG,cAAE,CAChB,kBAAkB,CAAE,GAAG,CACvB,iBAAiB,CAAE,GAAG,CACtB,YAAY,CAAE,IAAI,sBAAsB,CACzC,CAEA,4CAAG,CACF,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,WAAW,CAAE,MACd,CAEA,kDAAS,CACR,MAAM,CAAE,OAAO,CACf,aAAa,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CACpD,UAAU,CAAE,IAAI,4BAA4B,CAC7C,CAEA,kDAAQ,WAAY,CACnB,MAAM,CAAE,IACT,CAEA,kDAAQ,WAAW,GAAG,CAAE,CACvB,UAAU,CAAE,IAAI,2BAA2B,CAC5C,CAEA,kDAAQ,MAAO,CACd,UAAU,CAAE,IAAI,iBAAiB,CAClC,CAEA,sBAAQ,CAAG,cAAC,CAAG,cAAE,CAChB,kBAAkB,CAAE,GAAG,CACvB,iBAAiB,CAAE,GAAG,CACtB,YAAY,CAAE,IAAI,sBAAsB,CACzC,CAEA,sBAAQ,MAAM,CAAG,cAAC,CAAG,cAAE,CACtB,YAAY,CAAE,IAAI,qBAAqB,CACxC,CAEA,4CAAG,CACF,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,UAAU,CAAE,MACb,CAEA,mDAAU,CACT,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,GAAG,CAAE,IAAI,YAAY,CAAC,CACtB,UAAU,CAAE,IAAI,QAAQ,CAAC,CACzB,KAAK,CAAE,IAAI,wBAAwB,CAAC,CACpC,SAAS,CAAE,IAAI,SAAS,CACzB,CAEA,MAAM,uDAAc,CACnB,WAAW,CAAE,IAAI,aAAa,CAC/B"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let gallery;
  let { components } = $$props;
  let { component_props } = $$props;
  let { component_map } = $$props;
  let { label = "Examples" } = $$props;
  let { headers } = $$props;
  let { samples = null } = $$props;
  let old_samples = null;
  let { sample_labels = null } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = null } = $$props;
  let { root } = $$props;
  let { proxy_url } = $$props;
  let { samples_per_page = 10 } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let samples_dir = proxy_url ? `/proxy=${proxy_url}file=` : `${root}/file=`;
  let page = 0;
  let paginate = samples ? samples.length > samples_per_page : false;
  let selected_samples;
  let page_count;
  let visible_pages = [];
  let current_hover = -1;
  let component_meta = [];
  async function get_component_meta(selected_samples2) {
    component_meta = await Promise.all(selected_samples2 && selected_samples2.map(async (sample_row) => await Promise.all(sample_row.map(async (sample_cell, j) => {
      return {
        value: sample_cell,
        component: (await component_map.get(components[j]))?.default
      };
    }))));
  }
  if ($$props.components === void 0 && $$bindings.components && components !== void 0)
    $$bindings.components(components);
  if ($$props.component_props === void 0 && $$bindings.component_props && component_props !== void 0)
    $$bindings.component_props(component_props);
  if ($$props.component_map === void 0 && $$bindings.component_map && component_map !== void 0)
    $$bindings.component_map(component_map);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.headers === void 0 && $$bindings.headers && headers !== void 0)
    $$bindings.headers(headers);
  if ($$props.samples === void 0 && $$bindings.samples && samples !== void 0)
    $$bindings.samples(samples);
  if ($$props.sample_labels === void 0 && $$bindings.sample_labels && sample_labels !== void 0)
    $$bindings.sample_labels(sample_labels);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.proxy_url === void 0 && $$bindings.proxy_url && proxy_url !== void 0)
    $$bindings.proxy_url(proxy_url);
  if ($$props.samples_per_page === void 0 && $$bindings.samples_per_page && samples_per_page !== void 0)
    $$bindings.samples_per_page(samples_per_page);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  $$result.css.add(css);
  gallery = components.length < 2 || sample_labels !== null;
  {
    {
      if (sample_labels) {
        samples = sample_labels.map((e) => [e]);
      } else if (!samples) {
        samples = [];
      }
      if (samples !== old_samples) {
        page = 0;
        old_samples = samples;
      }
      paginate = samples.length > samples_per_page;
      if (paginate) {
        visible_pages = [];
        selected_samples = samples.slice(page * samples_per_page, (page + 1) * samples_per_page);
        page_count = Math.ceil(samples.length / samples_per_page);
        [0, page, page_count - 1].forEach((anchor) => {
          for (let i = anchor - 2; i <= anchor + 2; i++) {
            if (i >= 0 && i < page_count && !visible_pages.includes(i)) {
              if (visible_pages.length > 0 && i - visible_pages[visible_pages.length - 1] > 1) {
                visible_pages.push(-1);
              }
              visible_pages.push(i);
            }
          }
        });
      } else {
        selected_samples = samples.slice();
      }
    }
  }
  {
    get_component_meta(selected_samples);
  }
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      visible,
      padding: false,
      elem_id,
      elem_classes,
      scale,
      min_width,
      allow_overflow: false,
      container: false
    },
    {},
    {
      default: () => {
        return `<div class="label svelte-p5q82i"><svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true" role="img" width="1em" height="1em" preserveAspectRatio="xMidYMid meet" viewBox="0 0 32 32" class="svelte-p5q82i"><path fill="currentColor" d="M10 6h18v2H10zm0 18h18v2H10zm0-9h18v2H10zm-6 0h2v2H4zm0-9h2v2H4zm0 18h2v2H4z"></path></svg> ${escape(label)}</div> ${gallery ? `<div class="gallery svelte-p5q82i">${each(selected_samples, (sample_row, i) => {
          return `${sample_row[0] ? `<button class="gallery-item svelte-p5q82i">${sample_labels ? `${validate_component(Example, "BaseExample").$$render(
            $$result,
            {
              value: sample_row[0],
              selected: current_hover === i,
              type: "gallery"
            },
            {},
            {}
          )}` : `${component_meta.length && component_map.get(components[0]) ? `${validate_component(component_meta[0][0].component || missing_component, "svelte:component").$$render($$result, Object.assign({}, component_props[0], { value: sample_row[0] }, { samples_dir }, { type: "gallery" }, { selected: current_hover === i }, { index: i }, { root }), {}, {})}` : ``}`} </button>` : ``}`;
        })}</div>` : `<div class="table-wrap svelte-p5q82i"><table tabindex="0" role="grid" class="svelte-p5q82i"><thead><tr class="tr-head svelte-p5q82i">${each(headers, (header) => {
          return `<th class="svelte-p5q82i">${escape(header)} </th>`;
        })}</tr></thead> <tbody>${each(component_meta, (sample_row, i) => {
          return `<tr class="tr-body svelte-p5q82i">${each(sample_row, ({ value: value2, component }, j) => {
            let component_name = components[j];
            return ` ${component_name !== void 0 && component_map.get(component_name) !== void 0 ? `<td style="${"max-width: " + escape(component_name === "textbox" ? "35ch" : "auto", true)}" class="${escape(null_to_empty(component_name), true) + " svelte-p5q82i"}">${validate_component(component || missing_component, "svelte:component").$$render($$result, Object.assign({}, component_props[j], { value: value2 }, { samples_dir }, { type: "table" }, { selected: current_hover === i }, { index: i }, { root }), {}, {})} </td>` : ``}`;
          })} </tr>`;
        })}</tbody></table></div>`} ${paginate ? `<div class="paginate svelte-p5q82i">Pages:
			${each(visible_pages, (visible_page) => {
          return `${visible_page === -1 ? `<div data-svelte-h="svelte-12rhcfw">...</div>` : `<button class="${["svelte-p5q82i", page === visible_page ? "current-page" : ""].join(" ").trim()}">${escape(visible_page + 1)} </button>`}`;
        })}</div>` : ``}`;
      }
    }
  )}`;
});

export { Index as default };
//# sourceMappingURL=Index16-Bbb_5hf_.js.map
