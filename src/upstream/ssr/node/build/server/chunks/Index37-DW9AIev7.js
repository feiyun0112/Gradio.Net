import { K as h, L as g, O as z } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { n, V as Ve, h as h$1, K as j } from './src3-C1zEyqUA.js';
export { default as BaseExample } from './Example28-DIIslOY3.js';
import { n as n$1 } from './HTML-DJ7Neqps.js';
import './async-Cv1-GZGV.js';
import { s as spread_props, a as attr_class, b as attr_style, d as derived } from './renderer-RGQlaTg4.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';
import 'fs';

function d(c,d){c.component(c=>{let{$$slots:f,$$events:p$1,...m}=d,h$2=m.children,g$1=new h(m),_=derived(()=>({value:g$1.props.value??``,label:g$1.shared.label,visible:g$1.shared.visible,...g$1.props.props}));g$1.props.value;async function v(e){try{let t=await z([e]),r=await g$1.shared.client.upload(t,g$1.shared.root,void 0,g$1.shared.max_file_size??void 0);if(r&&r[0])return {path:r[0].path,url:r[0].url};throw Error(`Upload failed`)}catch(e){throw g$1.dispatch(`error`,e instanceof Error?e.message:String(e)),e}}n(c,{visible:g$1.shared.visible,elem_id:g$1.shared.elem_id,elem_classes:g$1.shared.elem_classes,container:g$1.shared.container,padding:g$1.shared.padding!==false,overflow_behavior:`visible`,children:t=>{g$1.shared.show_label&&g$1.props.buttons&&g$1.props.buttons.length>0?(t.push(`<!--[0-->`),Ve(t,{buttons:g$1.props.buttons,on_custom_button_click:e=>{g$1.dispatch(`custom_button_click`,{id:e});}})):t.push(`<!--[-1-->`),t.push(`<!--]--> `),g$1.shared.show_label?(t.push(`<!--[0-->`),h$1(t,{Icon:j,show_label:g$1.shared.show_label,label:g$1.shared.label,float:true})):t.push(`<!--[-1-->`),t.push(`<!--]--> `),p(t,spread_props([{autoscroll:g$1.shared.autoscroll,i18n:g$1.i18n},g$1.shared.loading_status,{variant:`center`,on_clear_status:()=>g$1.dispatch(`clear_status`,g$1.shared.loading_status)}])),t.push(`<!----> <div${attr_class(`html-container svelte-1jts93g`,void 0,{pending:g$1.shared.loading_status?.status===`pending`&&g$1.shared.loading_status?.show_progress!==`hidden`,"label-padding":g$1.shared.show_label??void 0})}${attr_style(``,{"min-height":g$1.props.min_height&&g$1.shared.loading_status?.status!==`pending`?g(g$1.props.min_height):void 0,"max-height":g$1.props.max_height?g(g$1.props.max_height):void 0,"overflow-y":g$1.props.max_height?`auto`:void 0})}><!---->`),n$1(t,{props:_(),html_template:g$1.props.html_template,css_template:g$1.props.css_template,js_on_load:g$1.props.js_on_load,elem_classes:g$1.shared.elem_classes,visible:g$1.shared.visible===`hidden`?false:g$1.shared.visible,autoscroll:g$1.shared.autoscroll,apply_default_css:g$1.props.apply_default_css,head:g$1.props.head,component_class_name:g$1.props.component_class_name,upload:v,server:g$1.shared.server,onevent:e=>{g$1.dispatch(e.type,e.data);},onupdate_value:e=>{e.property===`value`?g$1.props.value=e.data:e.property===`label`?g$1.shared.label=e.data:e.property===`visible`&&(g$1.shared.visible=e.data);},children:e=>{h$2?.(e);}}),t.push(`<!----></div>`);},$$slots:{default:true}});});}

export { n$1 as BaseHTML, d as default };
//# sourceMappingURL=Index37-DW9AIev7.js.map
