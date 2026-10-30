import { K as h } from './2-C9eYyMgE.js';
import { r as r$1 } from './Index5-Dyf_LLdV.js';
import { e as escape_html, f as attr } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './Image-BmvGPCEw.js';

function r(e,r){e.component(e=>{let{elem_id:i=``,elem_classes:a=[],visible:o=true,variant:s=`secondary`,size:c=`lg`,value:l,icon:u,disabled:d=false,scale:f=null,min_width:p=void 0,on_click:m,children:h}=r;function g(){if(m?.(),!l?.url)return;let e;if(!l.orig_name&&l.url){let t=l.url.split(`/`);e=t[t.length-1],e=e.split(`?`)[0].split(`#`)[0];}else e=l.orig_name;let t=document.createElement(`a`);t.href=l.url,t.download=e||`file`,document.body.appendChild(t),t.click(),document.body.removeChild(t);}r$1(e,{size:c,variant:s,elem_id:i,elem_classes:a,visible:o,onclick:g,scale:f,min_width:p,disabled:d,children:e=>{u?(e.push(`<!--[0-->`),e.push(`<img class="button-icon svelte-4ac0fl"${attr(`src`,u.url)}${attr(`alt`,`${l} icon`)}/>`)):e.push(`<!--[-1-->`),e.push(`<!--]--> `),h?(e.push(`<!--[0-->`),h(e),e.push(`<!---->`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);}});});}function i(t,i){t.component(t=>{let{$$slots:a,$$events:o,...s}=i,c=new h(s);c.watch_for_change(),r(t,{value:c.props.value,variant:c.props.variant,elem_id:c.shared.elem_id,elem_classes:c.shared.elem_classes,size:c.props.size,scale:c.shared.scale,icon:c.props.icon,min_width:c.shared.min_width,visible:c.shared.visible,disabled:!c.shared.interactive,on_click:()=>c.dispatch(`click`),children:e=>{e.push(`<!---->${escape_html(c.shared.label??``)}`);}});});}

export { r as BaseButton, i as default };
//# sourceMappingURL=Index26-BGCJIznN.js.map
