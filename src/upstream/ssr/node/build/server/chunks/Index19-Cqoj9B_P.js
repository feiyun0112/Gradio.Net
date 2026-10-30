import { K as h } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { p as m, n } from './src3-C1zEyqUA.js';
export { default as BaseExample } from './Example10-Df0uZ9es.js';
import './tinycolor-D-nIUz6e.js';
import { f as attr, b as attr_style, c as bind_props, s as spread_props, d as derived, e as escape_html } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

function l(e,t){e.component(e=>{let{value:r=void 0,label:i,info:a,disabled:l,show_label:u,on_input:d=()=>{},on_release:f=()=>{},on_submit:p=()=>{},on_blur:m$1=()=>{},on_focus:h=()=>{}}=t;m(e,{show_label:u,info:a,children:e=>{e.push(`<!---->${escape_html(i)}`);}}),e.push(`<!----> <div><button class="dialog-button svelte-nbn1m9"${attr(`aria-label`,i)}${attr(`disabled`,l,true)}${attr_style(``,{background:r})}></button> `),e.push(`<!--[-1-->`),e.push(`<!--]--></div>`),bind_props(t,{value:r});});}function u(n$1,i){n$1.component(n$1=>{let{$$slots:a,$$events:o,...c}=i,u=new h(c,{value:`#000000`});u.props.value;let d=derived(()=>u.shared.label||u.i18n(`color_picker.color_picker`)),f=true,p$1;function m(e){n(e,{visible:u.shared.visible,elem_id:u.shared.elem_id,elem_classes:u.shared.elem_classes,container:u.shared.container,scale:u.shared.scale,min_width:u.shared.min_width,children:e=>{p(e,spread_props([{autoscroll:u.shared.autoscroll,i18n:u.i18n},u.shared.loading_status,{on_clear_status:()=>u.dispatch(`clear_status`,u.shared.loading_status)}])),e.push(`<!----> `),l(e,{label:d(),info:u.props.info,show_label:u.shared.show_label,disabled:!u.shared.interactive,on_input:()=>u.dispatch(`input`),on_release:()=>u.dispatch(`release`,u.props.value),on_submit:()=>u.dispatch(`submit`),on_blur:()=>u.dispatch(`blur`),on_focus:()=>u.dispatch(`focus`),get value(){return u.props.value},set value(e){u.props.value=e,f=false;}}),e.push(`<!---->`);},$$slots:{default:true}});}do f=true,p$1=n$1.copy(),m(p$1);while(!f);n$1.subsume(p$1);});}

export { l as BaseColorPicker, u as default };
//# sourceMappingURL=Index19-Cqoj9B_P.js.map
