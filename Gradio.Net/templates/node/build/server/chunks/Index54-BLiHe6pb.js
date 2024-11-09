import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, f as each, b as add_attribute, e as escape } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, e as BlockLabel, a1 as File$1 } from './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const ArrowIcon = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  return `<svg width="100%" height="100%" viewBox="0 0 14 17" version="1.1" style="fill-rule:evenodd;clip-rule:evenodd;stroke-linejoin:round;stroke-miterlimit:2;"><g transform="matrix(1,0,0,1,-10.6667,-7.73588)"><path d="M12.7,24.033C12.256,24.322 11.806,24.339 11.351,24.084C10.896,23.829 10.668,23.434 10.667,22.9L10.667,9.1C10.667,8.567 10.895,8.172 11.351,7.916C11.807,7.66 12.256,7.677 12.7,7.967L23.567,14.867C23.967,15.133 24.167,15.511 24.167,16C24.167,16.489 23.967,16.867 23.567,17.133L12.7,24.033Z" style="fill:currentColor;fill-rule:nonzero;"></path></g></svg>`;
});
const css$2 = {
  code: "input.svelte-1j130g3{--ring-color:transparent;position:relative;box-shadow:var(--input-shadow);border:1px solid var(--checkbox-border-color);border-radius:var(--radius-xs);background-color:var(--checkbox-background-color);line-height:var(--line-sm);width:18px !important;height:18px !important}input.svelte-1j130g3:checked,input.svelte-1j130g3:checked:hover,input.svelte-1j130g3:checked:focus{border-color:var(--checkbox-border-color-selected);background-image:var(--checkbox-check);background-color:var(--checkbox-background-color-selected)}input.svelte-1j130g3:hover{border-color:var(--checkbox-border-color-hover);background-color:var(--checkbox-background-color-hover)}input.svelte-1j130g3:focus{border-color:var(--checkbox-border-color-focus);background-color:var(--checkbox-background-color-focus)}.disabled.svelte-1j130g3{opacity:0.8 !important;cursor:not-allowed}",
  map: '{"version":3,"file":"Checkbox.svelte","sources":["Checkbox.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nexport let value;\\nexport let disabled;\\nconst dispatch = createEventDispatcher();\\n<\/script>\\n\\n<input\\n\\tbind:checked={value}\\n\\ton:input={() => dispatch(\\"change\\", !value)}\\n\\ttype=\\"checkbox\\"\\n\\t{disabled}\\n\\tclass:disabled={disabled && !value}\\n/>\\n\\n<style>\\n\\tinput {\\n\\t\\t--ring-color: transparent;\\n\\t\\tposition: relative;\\n\\t\\tbox-shadow: var(--input-shadow);\\n\\t\\tborder: 1px solid var(--checkbox-border-color);\\n\\t\\tborder-radius: var(--radius-xs);\\n\\t\\tbackground-color: var(--checkbox-background-color);\\n\\t\\tline-height: var(--line-sm);\\n\\t\\twidth: 18px !important;\\n\\t\\theight: 18px !important;\\n\\t}\\n\\n\\tinput:checked,\\n\\tinput:checked:hover,\\n\\tinput:checked:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-selected);\\n\\t\\tbackground-image: var(--checkbox-check);\\n\\t\\tbackground-color: var(--checkbox-background-color-selected);\\n\\t}\\n\\n\\tinput:hover {\\n\\t\\tborder-color: var(--checkbox-border-color-hover);\\n\\t\\tbackground-color: var(--checkbox-background-color-hover);\\n\\t}\\n\\n\\tinput:focus {\\n\\t\\tborder-color: var(--checkbox-border-color-focus);\\n\\t\\tbackground-color: var(--checkbox-background-color-focus);\\n\\t}\\n\\n\\t.disabled {\\n\\t\\topacity: 0.8 !important;\\n\\t\\tcursor: not-allowed;\\n\\t}</style>\\n"],"names":[],"mappings":"AAeC,oBAAM,CACL,YAAY,CAAE,WAAW,CACzB,QAAQ,CAAE,QAAQ,CAClB,UAAU,CAAE,IAAI,cAAc,CAAC,CAC/B,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,WAAW,CAAE,IAAI,SAAS,CAAC,CAC3B,KAAK,CAAE,IAAI,CAAC,UAAU,CACtB,MAAM,CAAE,IAAI,CAAC,UACd,CAEA,oBAAK,QAAQ,CACb,oBAAK,QAAQ,MAAM,CACnB,oBAAK,QAAQ,MAAO,CACnB,YAAY,CAAE,IAAI,gCAAgC,CAAC,CACnD,gBAAgB,CAAE,IAAI,gBAAgB,CAAC,CACvC,gBAAgB,CAAE,IAAI,oCAAoC,CAC3D,CAEA,oBAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,oBAAK,MAAO,CACX,YAAY,CAAE,IAAI,6BAA6B,CAAC,CAChD,gBAAgB,CAAE,IAAI,iCAAiC,CACxD,CAEA,wBAAU,CACT,OAAO,CAAE,GAAG,CAAC,UAAU,CACvB,MAAM,CAAE,WACT"}'
};
const Checkbox = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { disabled } = $$props;
  createEventDispatcher();
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.disabled === void 0 && $$bindings.disabled && disabled !== void 0)
    $$bindings.disabled(disabled);
  $$result.css.add(css$2);
  return `<input type="checkbox" ${disabled ? "disabled" : ""} class="${["svelte-1j130g3", disabled && !value ? "disabled" : ""].join(" ").trim()}"${add_attribute("checked", value, 1)}>`;
});
const FileIcon = "data:image/svg+xml,%3csvg%20xmlns='http://www.w3.org/2000/svg'%20width='32'%20height='32'%20viewBox='0%200%2024%2024'%3e%3cpath%20fill='%23888888'%20d='M6%202c-1.1%200-1.99.9-1.99%202L4%2020c0%201.1.89%202%201.99%202H18c1.1%200%202-.9%202-2V8l-6-6H6zm7%207V3.5L18.5%209H13z'/%3e%3c/svg%3e";
const FolderIcon = "data:image/svg+xml,%3c?xml%20version='1.0'%20encoding='UTF-8'%20standalone='no'?%3e%3csvg%20viewBox='0%200%2032%2032'%20version='1.1'%20id='svg7'%20sodipodi:docname='light-folder-new.svg'%20inkscape:version='1.3.2%20(091e20e,%202023-11-25)'%20xmlns:inkscape='http://www.inkscape.org/namespaces/inkscape'%20xmlns:sodipodi='http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd'%20xmlns='http://www.w3.org/2000/svg'%20xmlns:svg='http://www.w3.org/2000/svg'%3e%3csodipodi:namedview%20id='namedview7'%20pagecolor='%23ffffff'%20bordercolor='%23000000'%20borderopacity='0.25'%20inkscape:showpageshadow='2'%20inkscape:pageopacity='0.0'%20inkscape:pagecheckerboard='0'%20inkscape:deskcolor='%23d1d1d1'%20inkscape:zoom='7.375'%20inkscape:cx='15.932203'%20inkscape:cy='16'%20inkscape:window-width='1312'%20inkscape:window-height='529'%20inkscape:window-x='0'%20inkscape:window-y='38'%20inkscape:window-maximized='0'%20inkscape:current-layer='svg7'%20/%3e%3cdefs%20id='defs6'%3e%3cclipPath%20id='clipPath1'%3e%3cpath%20d='m69.63%2012.145h-.052c-22.727-.292-46.47%204.077-46.709%204.122-2.424.451-4.946%202.974-5.397%205.397-.044.237-4.414%2023.983-4.122%2046.71-.292%2022.777%204.078%2046.523%204.122%2046.761.451%202.423%202.974%204.945%205.398%205.398.237.044%2023.982%204.413%2046.709%204.121%2022.779.292%2046.524-4.077%2046.761-4.121%202.423-.452%204.946-2.976%205.398-5.399.044-.236%204.413-23.981%204.121-46.709.292-22.777-4.077-46.523-4.121-46.761-.453-2.423-2.976-4.946-5.398-5.397-.238-.045-23.984-4.414-46.71-4.122'%20id='path1'%20/%3e%3c/clipPath%3e%3clinearGradient%20gradientUnits='userSpaceOnUse'%20y2='352.98'%20x2='-601.15'%20y1='663.95'%20x1='-591.02'%20id='2'%3e%3cstop%20stop-color='%23a0a0a0'%20id='stop1'%20/%3e%3cstop%20offset='1'%20stop-color='%23aaa'%20id='stop2'%20/%3e%3c/linearGradient%3e%3clinearGradient%20gradientUnits='userSpaceOnUse'%20y2='354.29'%20x2='-704.05'%20y1='647.77'%20x1='-701.19'%20id='1'%3e%3cstop%20stop-color='%23acabab'%20id='stop3'%20/%3e%3cstop%20offset='1'%20stop-color='%23d4d4d4'%20id='stop4'%20/%3e%3c/linearGradient%3e%3clinearGradient%20id='0'%20x1='59.12'%20y1='-19.888'%20x2='59.15'%20y2='-37.783'%20gradientUnits='userSpaceOnUse'%20gradientTransform='matrix(4.17478%200%200%204.16765-1069.7%20447.73)'%3e%3cstop%20stop-color='%23a0a0a0'%20id='stop5'%20/%3e%3cstop%20offset='1'%20stop-color='%23bdbdbd'%20id='stop6'%20/%3e%3c/linearGradient%3e%3c/defs%3e%3cg%20transform='matrix(.07089%200%200%20.07017%2023.295-40.67)'%20fill='%2360aae5'%20id='g7'%20style='fill:%23888888;fill-opacity:1'%3e%3cpath%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20d='m-884.1%20294.78c-4.626%200-8.349%203.718-8.349%208.335v161.41l468.19%201v-121.2c0-4.618-3.724-8.335-8.35-8.335h-272.65c-8.51.751-9.607-.377-13.812-5.981-5.964-7.968-14.969-21.443-20.84-29.21-4.712-6.805-5.477-6.02-13.292-6.02z'%20fill='url(%230)'%20color='%23000'%20id='path6'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3crect%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20y='356.85'%20x='-890.28'%20height='295.13'%20width='463.85'%20fill='url(%231)'%20stroke='url(%231)'%20stroke-width='2.378'%20rx='9.63'%20id='rect6'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3crect%20width='463.85'%20height='295.13'%20x='-890.28'%20y='356.85'%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20fill='none'%20stroke='url(%232)'%20stroke-linejoin='round'%20stroke-linecap='round'%20stroke-width='5.376'%20rx='9.63'%20id='rect7'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3c/g%3e%3c/svg%3e";
const css$1 = {
  code: ".icon.svelte-p1d4ff.svelte-p1d4ff{display:inline-block;width:18px;height:18px;padding:3px 2px 3px 3px;margin:0;flex-grow:0;display:inline-flex;justify-content:center;align-items:center;border-radius:2px;cursor:pointer;transition:0.1s;flex-shrink:0}.file-icon.svelte-p1d4ff.svelte-p1d4ff{display:inline-block;height:20px;margin-left:-1px;margin:0;flex-grow:0;display:inline-flex;justify-content:center;align-items:center;transition:0.1s}.file-icon.svelte-p1d4ff img.svelte-p1d4ff{width:100%;height:100%}.icon.svelte-p1d4ff.svelte-p1d4ff:hover{background:#eee}.icon.svelte-p1d4ff:hover > *{color:var(--block-info-text-color)}.icon.svelte-p1d4ff > *{transform:rotate(90deg);transform-origin:40% 50%;transition:0.2s;color:var(--color-accent)}.no-checkbox.svelte-p1d4ff.svelte-p1d4ff{width:18px;height:18px}.hidden.svelte-p1d4ff > *{transform:rotate(0);color:var(--body-text-color-subdued)}ul.svelte-p1d4ff.svelte-p1d4ff{margin-left:26px;padding-left:0;list-style:none}li.svelte-p1d4ff.svelte-p1d4ff{margin-left:0;padding-left:0;align-items:center;margin:8px 0;font-family:var(--font-mono);font-size:var(--scale-00);overflow-wrap:anywhere;word-break:break-word}.wrap.svelte-p1d4ff.svelte-p1d4ff{display:flex;gap:8px;align-items:center}",
  map: '{"version":3,"file":"FileTree.svelte","sources":["FileTree.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { createEventDispatcher } from \\"svelte\\";\\nimport Arrow from \\"./ArrowIcon.svelte\\";\\nimport Checkbox from \\"./Checkbox.svelte\\";\\nimport FileIcon from \\"../icons/light-file.svg\\";\\nimport FolderIcon from \\"../icons/light-folder.svg\\";\\nexport let path = [];\\nexport let selected_files = [];\\nexport let selected_folders = [];\\nexport let is_selected_entirely = false;\\nexport let interactive;\\nexport let ls_fn;\\nexport let file_count = \\"multiple\\";\\nexport let valid_for_selection;\\nlet content = [];\\nlet opened_folders = [];\\nconst toggle_open_folder = (i) => {\\n    if (opened_folders.includes(i)) {\\n        opened_folders = opened_folders.filter((x) => x !== i);\\n    }\\n    else {\\n        opened_folders = [...opened_folders, i];\\n    }\\n};\\nconst open_folder = (i) => {\\n    if (!opened_folders.includes(i)) {\\n        opened_folders = [...opened_folders, i];\\n    }\\n};\\n(async () => {\\n    content = await ls_fn(path);\\n    if (valid_for_selection) {\\n        content = [{ name: \\".\\", type: \\"file\\" }, ...content];\\n    }\\n    opened_folders = content.map((x, i) => x.type === \\"folder\\" && (is_selected_entirely || selected_files.some((y) => y[0] === x.name)) ? i : null).filter((x) => x !== null);\\n})();\\n$: if (is_selected_entirely) {\\n    content.forEach((x) => {\\n        dispatch(\\"check\\", {\\n            path: [...path, x.name],\\n            checked: true,\\n            type: x.type\\n        });\\n    });\\n}\\nconst dispatch = createEventDispatcher();\\n<\/script>\\n\\n<ul>\\n\\t{#each content as { type, name, valid }, i}\\n\\t\\t<li>\\n\\t\\t\\t<span class=\\"wrap\\">\\n\\t\\t\\t\\t{#if type === \\"folder\\" && file_count === \\"single\\"}\\n\\t\\t\\t\\t\\t<span class=\\"no-checkbox\\" aria-hidden=\\"true\\"></span>\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<Checkbox\\n\\t\\t\\t\\t\\t\\tdisabled={!interactive}\\n\\t\\t\\t\\t\\t\\tvalue={(type === \\"file\\" ? selected_files : selected_folders).some(\\n\\t\\t\\t\\t\\t\\t\\t(x) => x[0] === name && x.length === 1\\n\\t\\t\\t\\t\\t\\t)}\\n\\t\\t\\t\\t\\t\\ton:change={(e) => {\\n\\t\\t\\t\\t\\t\\t\\tlet checked = e.detail;\\n\\t\\t\\t\\t\\t\\t\\tdispatch(\\"check\\", {\\n\\t\\t\\t\\t\\t\\t\\t\\tpath: [...path, name],\\n\\t\\t\\t\\t\\t\\t\\t\\tchecked,\\n\\t\\t\\t\\t\\t\\t\\t\\ttype\\n\\t\\t\\t\\t\\t\\t\\t});\\n\\t\\t\\t\\t\\t\\t\\tif (type === \\"folder\\" && checked) {\\n\\t\\t\\t\\t\\t\\t\\t\\topen_folder(i);\\n\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t{/if}\\n\\n\\t\\t\\t\\t{#if type === \\"folder\\"}\\n\\t\\t\\t\\t\\t<span\\n\\t\\t\\t\\t\\t\\tclass=\\"icon\\"\\n\\t\\t\\t\\t\\t\\tclass:hidden={!opened_folders.includes(i)}\\n\\t\\t\\t\\t\\t\\ton:click|stopPropagation={() => toggle_open_folder(i)}\\n\\t\\t\\t\\t\\t\\trole=\\"button\\"\\n\\t\\t\\t\\t\\t\\taria-label=\\"expand directory\\"\\n\\t\\t\\t\\t\\t\\ttabindex=\\"0\\"\\n\\t\\t\\t\\t\\t\\ton:keydown={({ key }) => {\\n\\t\\t\\t\\t\\t\\t\\tif (key === \\" \\" || key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\t\\ttoggle_open_folder(i);\\n\\t\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t\\t}}><Arrow /></span\\n\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<span class=\\"file-icon\\">\\n\\t\\t\\t\\t\\t\\t<img src={name === \\".\\" ? FolderIcon : FileIcon} alt=\\"file icon\\" />\\n\\t\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{name}\\n\\t\\t\\t</span>\\n\\t\\t\\t{#if type === \\"folder\\" && opened_folders.includes(i)}\\n\\t\\t\\t\\t<svelte:self\\n\\t\\t\\t\\t\\tpath={[...path, name]}\\n\\t\\t\\t\\t\\tselected_files={selected_files\\n\\t\\t\\t\\t\\t\\t.filter((x) => x[0] === name)\\n\\t\\t\\t\\t\\t\\t.map((x) => x.slice(1))}\\n\\t\\t\\t\\t\\tselected_folders={selected_folders\\n\\t\\t\\t\\t\\t\\t.filter((x) => x[0] === name)\\n\\t\\t\\t\\t\\t\\t.map((x) => x.slice(1))}\\n\\t\\t\\t\\t\\tis_selected_entirely={selected_folders.some(\\n\\t\\t\\t\\t\\t\\t(x) => x[0] === name && x.length === 1\\n\\t\\t\\t\\t\\t)}\\n\\t\\t\\t\\t\\t{interactive}\\n\\t\\t\\t\\t\\t{ls_fn}\\n\\t\\t\\t\\t\\t{file_count}\\n\\t\\t\\t\\t\\tvalid_for_selection={valid}\\n\\t\\t\\t\\t\\ton:check\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t</li>\\n\\t{/each}\\n</ul>\\n\\n<style>\\n\\t.icon {\\n\\t\\tdisplay: inline-block;\\n\\t\\twidth: 18px;\\n\\t\\theight: 18px;\\n\\t\\tpadding: 3px 2px 3px 3px;\\n\\t\\tmargin: 0;\\n\\t\\tflex-grow: 0;\\n\\t\\tdisplay: inline-flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tborder-radius: 2px;\\n\\t\\tcursor: pointer;\\n\\t\\ttransition: 0.1s;\\n\\t\\tflex-shrink: 0;\\n\\t}\\n\\n\\t.file-icon {\\n\\t\\tdisplay: inline-block;\\n\\t\\theight: 20px;\\n\\t\\tmargin-left: -1px;\\n\\t\\tmargin: 0;\\n\\t\\tflex-grow: 0;\\n\\t\\tdisplay: inline-flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\n\\t\\ttransition: 0.1s;\\n\\t}\\n\\n\\t.file-icon img {\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.icon:hover {\\n\\t\\tbackground: #eee;\\n\\t}\\n\\n\\t.icon:hover :global(> *) {\\n\\t\\tcolor: var(--block-info-text-color);\\n\\t}\\n\\n\\t.icon :global(> *) {\\n\\t\\ttransform: rotate(90deg);\\n\\t\\ttransform-origin: 40% 50%;\\n\\t\\ttransition: 0.2s;\\n\\t\\tcolor: var(--color-accent);\\n\\t}\\n\\n\\t.no-checkbox {\\n\\t\\twidth: 18px;\\n\\t\\theight: 18px;\\n\\t}\\n\\n\\t.hidden :global(> *) {\\n\\t\\ttransform: rotate(0);\\n\\t\\tcolor: var(--body-text-color-subdued);\\n\\t}\\n\\n\\tul {\\n\\t\\tmargin-left: 26px;\\n\\t\\tpadding-left: 0;\\n\\t\\tlist-style: none;\\n\\t}\\n\\n\\tli {\\n\\t\\tmargin-left: 0;\\n\\t\\tpadding-left: 0;\\n\\t\\talign-items: center;\\n\\t\\tmargin: 8px 0;\\n\\t\\tfont-family: var(--font-mono);\\n\\t\\tfont-size: var(--scale-00);\\n\\t\\toverflow-wrap: anywhere;\\n\\t\\tword-break: break-word;\\n\\t}\\n\\n\\t.wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tgap: 8px;\\n\\t\\talign-items: center;\\n\\t}</style>\\n"],"names":[],"mappings":"AAsHC,iCAAM,CACL,OAAO,CAAE,YAAY,CACrB,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,OAAO,CAAE,GAAG,CAAC,GAAG,CAAC,GAAG,CAAC,GAAG,CACxB,MAAM,CAAE,CAAC,CACT,SAAS,CAAE,CAAC,CACZ,OAAO,CAAE,WAAW,CACpB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,aAAa,CAAE,GAAG,CAClB,MAAM,CAAE,OAAO,CACf,UAAU,CAAE,IAAI,CAChB,WAAW,CAAE,CACd,CAEA,sCAAW,CACV,OAAO,CAAE,YAAY,CACrB,MAAM,CAAE,IAAI,CACZ,WAAW,CAAE,IAAI,CACjB,MAAM,CAAE,CAAC,CACT,SAAS,CAAE,CAAC,CACZ,OAAO,CAAE,WAAW,CACpB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CAEnB,UAAU,CAAE,IACb,CAEA,wBAAU,CAAC,iBAAI,CACd,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IACT,CAEA,iCAAK,MAAO,CACX,UAAU,CAAE,IACb,CAEA,mBAAK,MAAM,CAAS,GAAK,CACxB,KAAK,CAAE,IAAI,uBAAuB,CACnC,CAEA,mBAAK,CAAS,GAAK,CAClB,SAAS,CAAE,OAAO,KAAK,CAAC,CACxB,gBAAgB,CAAE,GAAG,CAAC,GAAG,CACzB,UAAU,CAAE,IAAI,CAChB,KAAK,CAAE,IAAI,cAAc,CAC1B,CAEA,wCAAa,CACZ,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IACT,CAEA,qBAAO,CAAS,GAAK,CACpB,SAAS,CAAE,OAAO,CAAC,CAAC,CACpB,KAAK,CAAE,IAAI,yBAAyB,CACrC,CAEA,8BAAG,CACF,WAAW,CAAE,IAAI,CACjB,YAAY,CAAE,CAAC,CACf,UAAU,CAAE,IACb,CAEA,8BAAG,CACF,WAAW,CAAE,CAAC,CACd,YAAY,CAAE,CAAC,CACf,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,GAAG,CAAC,CAAC,CACb,WAAW,CAAE,IAAI,WAAW,CAAC,CAC7B,SAAS,CAAE,IAAI,UAAU,CAAC,CAC1B,aAAa,CAAE,QAAQ,CACvB,UAAU,CAAE,UACb,CAEA,iCAAM,CACL,OAAO,CAAE,IAAI,CACb,GAAG,CAAE,GAAG,CACR,WAAW,CAAE,MACd"}'
};
const FileTree = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { path = [] } = $$props;
  let { selected_files = [] } = $$props;
  let { selected_folders = [] } = $$props;
  let { is_selected_entirely = false } = $$props;
  let { interactive } = $$props;
  let { ls_fn } = $$props;
  let { file_count = "multiple" } = $$props;
  let { valid_for_selection } = $$props;
  let content = [];
  let opened_folders = [];
  (async () => {
    content = await ls_fn(path);
    if (valid_for_selection) {
      content = [{ name: ".", type: "file" }, ...content];
    }
    opened_folders = content.map((x, i) => x.type === "folder" && (is_selected_entirely || selected_files.some((y) => y[0] === x.name)) ? i : null).filter((x) => x !== null);
  })();
  const dispatch = createEventDispatcher();
  if ($$props.path === void 0 && $$bindings.path && path !== void 0)
    $$bindings.path(path);
  if ($$props.selected_files === void 0 && $$bindings.selected_files && selected_files !== void 0)
    $$bindings.selected_files(selected_files);
  if ($$props.selected_folders === void 0 && $$bindings.selected_folders && selected_folders !== void 0)
    $$bindings.selected_folders(selected_folders);
  if ($$props.is_selected_entirely === void 0 && $$bindings.is_selected_entirely && is_selected_entirely !== void 0)
    $$bindings.is_selected_entirely(is_selected_entirely);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.ls_fn === void 0 && $$bindings.ls_fn && ls_fn !== void 0)
    $$bindings.ls_fn(ls_fn);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.valid_for_selection === void 0 && $$bindings.valid_for_selection && valid_for_selection !== void 0)
    $$bindings.valid_for_selection(valid_for_selection);
  $$result.css.add(css$1);
  {
    if (is_selected_entirely) {
      content.forEach((x) => {
        dispatch("check", {
          path: [...path, x.name],
          checked: true,
          type: x.type
        });
      });
    }
  }
  return `<ul class="svelte-p1d4ff">${each(content, ({ type, name, valid }, i) => {
    return `<li class="svelte-p1d4ff"><span class="wrap svelte-p1d4ff">${type === "folder" && file_count === "single" ? `<span class="no-checkbox svelte-p1d4ff" aria-hidden="true"></span>` : `${validate_component(Checkbox, "Checkbox").$$render(
      $$result,
      {
        disabled: !interactive,
        value: (type === "file" ? selected_files : selected_folders).some((x) => x[0] === name && x.length === 1)
      },
      {},
      {}
    )}`} ${type === "folder" ? `<span class="${["icon svelte-p1d4ff", !opened_folders.includes(i) ? "hidden" : ""].join(" ").trim()}" role="button" aria-label="expand directory" tabindex="0">${validate_component(ArrowIcon, "Arrow").$$render($$result, {}, {}, {})}</span>` : `<span class="file-icon svelte-p1d4ff"><img${add_attribute("src", name === "." ? FolderIcon : FileIcon, 0)} alt="file icon" class="svelte-p1d4ff"> </span>`} ${escape(name)}</span> ${type === "folder" && opened_folders.includes(i) ? `${validate_component(FileTree, "svelte:self").$$render(
      $$result,
      {
        path: [...path, name],
        selected_files: selected_files.filter((x) => x[0] === name).map((x) => x.slice(1)),
        selected_folders: selected_folders.filter((x) => x[0] === name).map((x) => x.slice(1)),
        is_selected_entirely: selected_folders.some((x) => x[0] === name && x.length === 1),
        interactive,
        ls_fn,
        file_count,
        valid_for_selection: valid
      },
      {},
      {}
    )}` : ``} </li>`;
  })} </ul>`;
});
const css = {
  code: ".file-wrap.svelte-dicskc{height:calc(100% - 25px);overflow-y:scroll}",
  map: '{"version":3,"file":"DirectoryExplorer.svelte","sources":["DirectoryExplorer.svelte"],"sourcesContent":["<script lang=\\"ts\\">import FileTree from \\"./FileTree.svelte\\";\\nexport let interactive;\\nexport let file_count = \\"multiple\\";\\nexport let value = [];\\nexport let ls_fn;\\nlet selected_folders = [];\\nconst paths_equal = (path, path_2) => {\\n    return path.join(\\"/\\") === path_2.join(\\"/\\");\\n};\\nconst path_in_set = (path, set) => {\\n    return set.some((x) => paths_equal(x, path));\\n};\\nconst path_inside = (path, path_2) => {\\n    return path.join(\\"/\\").startsWith(path_2.join(\\"/\\"));\\n};\\n<\/script>\\n\\n<div class=\\"file-wrap\\">\\n\\t<FileTree\\n\\t\\tpath={[]}\\n\\t\\tselected_files={value}\\n\\t\\t{selected_folders}\\n\\t\\t{interactive}\\n\\t\\t{ls_fn}\\n\\t\\t{file_count}\\n\\t\\tvalid_for_selection={false}\\n\\t\\ton:check={(e) => {\\n\\t\\t\\tconst { path, checked, type } = e.detail;\\n\\t\\t\\tif (checked) {\\n\\t\\t\\t\\tif (file_count === \\"single\\") {\\n\\t\\t\\t\\t\\tvalue = [path];\\n\\t\\t\\t\\t} else if (type === \\"folder\\") {\\n\\t\\t\\t\\t\\tif (!path_in_set(path, selected_folders)) {\\n\\t\\t\\t\\t\\t\\tselected_folders = [...selected_folders, path];\\n\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t} else {\\n\\t\\t\\t\\t\\tif (!path_in_set(path, value)) {\\n\\t\\t\\t\\t\\t\\tvalue = [...value, path];\\n\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t}\\n\\t\\t\\t} else {\\n\\t\\t\\t\\tselected_folders = selected_folders.filter(\\n\\t\\t\\t\\t\\t(folder) => !path_inside(path, folder)\\n\\t\\t\\t\\t); // deselect all parent folders\\n\\t\\t\\t\\tif (type === \\"folder\\") {\\n\\t\\t\\t\\t\\tselected_folders = selected_folders.filter(\\n\\t\\t\\t\\t\\t\\t(folder) => !path_inside(folder, path)\\n\\t\\t\\t\\t\\t); // deselect all children folders\\n\\t\\t\\t\\t\\tvalue = value.filter((file) => !path_inside(file, path)); // deselect all children files\\n\\t\\t\\t\\t} else {\\n\\t\\t\\t\\t\\tvalue = value.filter((x) => !paths_equal(x, path));\\n\\t\\t\\t\\t}\\n\\t\\t\\t}\\n\\t\\t}}\\n\\t/>\\n</div>\\n\\n<style>\\n\\t.file-wrap {\\n\\t\\theight: calc(100% - 25px);\\n\\t\\toverflow-y: scroll;\\n\\t}</style>\\n"],"names":[],"mappings":"AA0DC,wBAAW,CACV,MAAM,CAAE,KAAK,IAAI,CAAC,CAAC,CAAC,IAAI,CAAC,CACzB,UAAU,CAAE,MACb"}'
};
const DirectoryExplorer = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { interactive } = $$props;
  let { file_count = "multiple" } = $$props;
  let { value = [] } = $$props;
  let { ls_fn } = $$props;
  let selected_folders = [];
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.ls_fn === void 0 && $$bindings.ls_fn && ls_fn !== void 0)
    $$bindings.ls_fn(ls_fn);
  $$result.css.add(css);
  return `<div class="file-wrap svelte-dicskc">${validate_component(FileTree, "FileTree").$$render(
    $$result,
    {
      path: [],
      selected_files: value,
      selected_folders,
      interactive,
      ls_fn,
      file_count,
      valid_for_selection: false
    },
    {},
    {}
  )} </div>`;
});
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { label } = $$props;
  let { show_label } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let { file_count = "multiple" } = $$props;
  let { root_dir } = $$props;
  let { glob } = $$props;
  let { ignore_glob } = $$props;
  let { loading_status } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { server } = $$props;
  let { interactive } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.max_height === void 0 && $$bindings.max_height && max_height !== void 0)
    $$bindings.max_height(max_height);
  if ($$props.file_count === void 0 && $$bindings.file_count && file_count !== void 0)
    $$bindings.file_count(file_count);
  if ($$props.root_dir === void 0 && $$bindings.root_dir && root_dir !== void 0)
    $$bindings.root_dir(root_dir);
  if ($$props.glob === void 0 && $$bindings.glob && glob !== void 0)
    $$bindings.glob(glob);
  if ($$props.ignore_glob === void 0 && $$bindings.ignore_glob && ignore_glob !== void 0)
    $$bindings.ignore_glob(ignore_glob);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.server === void 0 && $$bindings.server && server !== void 0)
    $$bindings.server(server);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      if (JSON.stringify(value) !== JSON.stringify(old_value)) {
        old_value = value;
        gradio.dispatch("change");
      }
    }
    $$rendered = `  ${validate_component(Block, "Block").$$render(
      $$result,
      {
        visible,
        variant: value === null ? "dashed" : "solid",
        border_mode: "base",
        padding: false,
        elem_id,
        elem_classes,
        container,
        scale,
        min_width,
        allow_overflow: true,
        overflow_behavior: "auto",
        height,
        max_height,
        min_height
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, loading_status, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }), {}, {})} ${validate_component(BlockLabel, "BlockLabel").$$render(
            $$result,
            {
              show_label,
              Icon: File$1,
              label: label || "FileExplorer",
              float: false
            },
            {},
            {}
          )} ${validate_component(DirectoryExplorer, "DirectoryExplorer").$$render(
            $$result,
            {
              file_count,
              interactive,
              ls_fn: server.ls,
              value
            },
            {
              value: ($$value) => {
                value = $$value;
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

export { Index as default };
//# sourceMappingURL=Index54-BLiHe6pb.js.map
