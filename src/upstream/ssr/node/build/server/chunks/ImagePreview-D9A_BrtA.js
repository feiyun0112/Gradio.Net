import { Q as c } from './2-C9eYyMgE.js';
import { h, v, V as Ve, H, a as He, g, _, i as ie, I as Ie } from './src3-C1zEyqUA.js';
import { t } from './Image-BmvGPCEw.js';
import './async-Cv1-GZGV.js';
import { a as attr_class, c as bind_props } from './renderer-RGQlaTg4.js';

function f(f,p){f.component(f=>{let{value:m,alt_text:h$1=null,label:g$1=void 0,show_label:_$1,buttons:v$1=[],on_custom_button_click:y=null,selectable:b=false,i18n:x,display_icon_button_wrapper_top_corner:S=false,fullscreen:C=false,show_button_background:w=true,onselect:T,onfullscreen:E,onshare:D,onerror:O,onload:k}=p;h(f,{show_label:_$1,Icon:H,label:_$1?g$1||x(`image.image`):``}),f.push(`<!----> `),m==null||!m?.url?(f.push(`<!--[0-->`),v(f,{unpadded_box:true,size:`large`,children:e=>{H(e);}})):(f.push(`<!--[-1-->`),f.push(`<div class="image-container svelte-12vrxzd">`),Ve(f,{display_top_corner:S,show_background:w,buttons:v$1,on_custom_button_click:y,children:t=>{v$1.some(e=>typeof e==`string`&&e===`fullscreen`)?(t.push(`<!--[0-->`),He(t,{fullscreen:C,onclick:e=>{C=e,E?.(e);}})):t.push(`<!--[-1-->`),t.push(`<!--]--> `),v$1.some(e=>typeof e==`string`&&e===`download`)?(t.push(`<!--[0-->`),g(t,{href:m.url,download:m.orig_name||`image`,children:e=>{_(e,{Icon:ie,label:x(`common.download`)});},$$slots:{default:true}})):t.push(`<!--[-1-->`),t.push(`<!--]--> `),v$1.some(e=>typeof e==`string`&&e===`share`)?(t.push(`<!--[0-->`),Ie(t,{i18n:x,onshare:e=>D?.(e),onerror:e=>O?.(e),formatter:async t=>t?`<img src="${await c(t)}" />`:``,value:m})):t.push(`<!--[-1-->`),t.push(`<!--]-->`);}}),f.push(`<!----> <button class="svelte-12vrxzd"><div${attr_class(`image-frame svelte-12vrxzd`,void 0,{selectable:b})}>`),t(f,{src:m.url,restProps:{loading:`lazy`,alt:h$1??``},onload:k}),f.push(`<!----></div></button></div>`)),f.push(`<!--]-->`),bind_props(p,{fullscreen:C});});}

export { f };
//# sourceMappingURL=ImagePreview-D9A_BrtA.js.map
