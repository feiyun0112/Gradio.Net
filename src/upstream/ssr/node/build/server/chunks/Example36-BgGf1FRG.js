import { t } from './Image-BmvGPCEw.js';
import './2-C9eYyMgE.js';
import { a as attr_class, e as escape_html, h as ensure_array_like, f as attr } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import { _ } from './Video-PPw8acfh.js';

function r(r,i){r.component(r=>{let{value:a={text:``,files:[]},type:o,selected:s=false}=i;r.push(`<div${attr_class(`container svelte-xz0m7l`,void 0,{table:o===`table`,gallery:o===`gallery`,selected:s,border:a})}><p>${escape_html(a.text?a.text:``)}</p> <!--[-->`);let c=ensure_array_like(a.files);for(let i=0,a=c.length;i<a;i++){let a=c[i];a.mime_type&&a.mime_type.includes(`image`)?(r.push(`<!--[0-->`),t(r,{src:a.url,alt:``})):a.mime_type&&a.mime_type.includes(`video`)?(r.push(`<!--[1-->`),_(r,{src:a.url,alt:``,loop:true,is_stream:false})):a.mime_type&&a.mime_type.includes(`audio`)?(r.push(`<!--[2-->`),r.push(`<audio${attr(`src`,a.url)} controls=""></audio>`)):(r.push(`<!--[-1-->`),r.push(`${escape_html(a.orig_name)}`)),r.push(`<!--]-->`);}r.push(`<!--]--></div>`);});}

export { r };
//# sourceMappingURL=Example36-BgGf1FRG.js.map
