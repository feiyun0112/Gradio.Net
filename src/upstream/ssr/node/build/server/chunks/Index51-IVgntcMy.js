import { K as h } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { n, V as Ve, p as m } from './src3-C1zEyqUA.js';
export { default as BaseExample } from './Example43-BZygSv4t.js';
import { f as attr, i as stringify, a as attr_class, e as escape_html, c as bind_props, d as derived, s as spread_props, h as ensure_array_like } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

var s=0;function c(e,t){e.component(e=>{let{selected:n=void 0,display_value:r,internal_value:i,disabled:a,rtl:c,on_input:l}=t,u=derived(()=>n===i);e.push(`<label${attr(`data-testid`,`${stringify(r)}-radio-label`)}${attr_class(`svelte-19qdtil`,void 0,{disabled:a,selected:u(),rtl:c})}><input${attr(`disabled`,a,true)} type="radio"${attr(`name`,`radio-${stringify(++s)}`)}${attr(`value`,i)}${attr(`aria-checked`,u())}${attr(`checked`,n===i,true)} class="svelte-19qdtil"/> <span class="svelte-19qdtil">${escape_html(r)}</span></label>`),bind_props(t,{selected:n});});}function l(a,s){a.component(a=>{let{$$slots:l,$$events:u,...d}=s,f=new h(d),p$1=derived(()=>!f.shared.interactive);f.props.value;let m$1=true,h$1;function g(e){n(e,{visible:f.shared.visible,type:`fieldset`,elem_id:f.shared.elem_id,elem_classes:f.shared.elem_classes,container:f.shared.container,scale:f.shared.scale,min_width:f.shared.min_width,rtl:f.props.rtl,children:e=>{p(e,spread_props([{autoscroll:f.shared.autoscroll,i18n:f.i18n},f.shared.loading_status,{on_clear_status:()=>f.dispatch(`clear_status`,f.shared.loading_status)}])),e.push(`<!----> `),f.shared.show_label&&f.props.buttons&&f.props.buttons.length>0?(e.push(`<!--[0-->`),Ve(e,{buttons:f.props.buttons,on_custom_button_click:e=>{f.dispatch(`custom_button_click`,{id:e});}})):e.push(`<!--[-1-->`),e.push(`<!--]--> `),m(e,{show_label:f.shared.show_label,info:f.props.info,children:e=>{e.push(`<!---->${escape_html(f.shared.label||f.i18n(`radio.radio`))}`);}}),e.push(`<!----> <div class="wrap svelte-e4x47i"><!--[-->`);let r=ensure_array_like(f.props.choices);for(let t=0,n=r.length;t<n;t++){let[n,i]=r[t];c(e,{display_value:f.live_i18n(n),internal_value:i,disabled:p$1(),rtl:f.props.rtl,on_input:()=>{f.dispatch(`input`),f.dispatch(`select`,{value:i,index:t});},get selected(){return f.props.value},set selected(e){f.props.value=e,m$1=false;}});}e.push(`<!--]--></div>`);},$$slots:{default:true}});}do m$1=true,h$1=a.copy(),g(h$1);while(!m$1);a.subsume(h$1);});}

export { c as BaseRadio, l as default };
//# sourceMappingURL=Index51-IVgntcMy.js.map
