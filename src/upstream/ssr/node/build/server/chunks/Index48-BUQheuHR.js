import { K as h } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { n, V as Ve, p as m } from './src3-C1zEyqUA.js';
import { s as spread_props, a as attr_class, e as escape_html, f as attr, d as derived } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

function o(o,s){o.component(o=>{let{$$slots:c,$$events:l,...u}=s,d=new h(u);d.props.value??=0,d.props.value;let f=derived(()=>!d.shared.interactive);n(o,{visible:d.shared.visible,elem_id:d.shared.elem_id,elem_classes:d.shared.elem_classes,padding:d.shared.container,allow_overflow:false,scale:d.shared.scale,min_width:d.shared.min_width,children:e=>{p(e,spread_props([{autoscroll:d.shared.autoscroll,i18n:d.i18n},d.shared.loading_status,{show_validation_error:false,on_clear_status:()=>{d.dispatch(`clear_status`,d.shared.loading_status);}}])),e.push(`<!----> <label${attr_class(`block svelte-16ty2ow`,void 0,{container:d.shared.container})}>`),d.shared.show_label&&d.props.buttons&&d.props.buttons.length>0?(e.push(`<!--[0-->`),Ve(e,{buttons:d.props.buttons,on_custom_button_click:e=>{d.dispatch(`custom_button_click`,{id:e});}})):e.push(`<!--[-1-->`),e.push(`<!--]--> `),m(e,{show_label:d.shared.show_label,info:d.props.info,children:e=>{e.push(`<!---->${escape_html(d.shared.label||`Number`)} `),d.shared.loading_status?.validation_error?(e.push(`<!--[0-->`),e.push(`<div class="validation-error svelte-16ty2ow">${escape_html(d.shared.loading_status?.validation_error)}</div>`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);}}),e.push(`<!----> <input${attr(`aria-label`,d.shared.label||`Number`)} type="number"${attr(`value`,d.props.value)}${attr(`min`,d.props.minimum)}${attr(`max`,d.props.maximum)}${attr(`step`,d.props.step)}${attr(`placeholder`,d.props.placeholder)}${attr(`disabled`,f(),true)}${attr_class(`svelte-16ty2ow`,void 0,{"validation-error":d.shared.loading_status?.validation_error})}/></label>`);},$$slots:{default:true}});});}

export { o as default };
//# sourceMappingURL=Index48-BUQheuHR.js.map
