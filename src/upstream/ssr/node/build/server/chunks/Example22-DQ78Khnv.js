import './async-Cv1-GZGV.js';
import { a as attr_class, h as ensure_array_like, e as escape_html } from './renderer-RGQlaTg4.js';

function t(t,n){t.component(t=>{let{value:r,type:i,selected:a=false}=n;if(t.push(`<ul${attr_class(`svelte-14aa7hi`,void 0,{table:i===`table`,gallery:i===`gallery`,selected:a})}>`),r){t.push(`<!--[0-->`),t.push(`<!--[-->`);let n=ensure_array_like(Array.isArray(r)?r.slice(0,3):[r]);for(let r=0,i=n.length;r<i;r++){let i=n[r];t.push(`<li><code>./${escape_html(i)}</code></li>`);}t.push(`<!--]--> `),Array.isArray(r)&&r.length>3?(t.push(`<!--[0-->`),t.push(`<li class="extra svelte-14aa7hi">...</li>`)):t.push(`<!--[-1-->`),t.push(`<!--]-->`);}else t.push(`<!--[-1-->`);t.push(`<!--]--></ul>`);});}

export { t as default };
//# sourceMappingURL=Example22-DQ78Khnv.js.map
