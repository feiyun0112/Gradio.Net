import { K as h, L as g } from './2-C9eYyMgE.js';
import { r } from './Index7-B978gaB_.js';
import { a } from './Index56-DnOKCQ08.js';
import { g as getContext, f as attr, a as attr_class, i as stringify, b as attr_style, o as store_get, u as unsubscribe_stores, d as derived } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './statustracker-Bh1XNgjP.js';
import './src3-C1zEyqUA.js';
import './html-CfyvkLET.js';
import './server-Ck07o0bn.js';

function o(e,t){e.component(e=>{var o;let{elem_id:s=``,elem_classes:c=[],label:l,id:u,visible:d,interactive:f,order:p,alignment:m=`left`,scale:h,height:g,max_height:_,component_id:v,onselect:y,children:b}=t,{register_tab:x,unregister_tab:S,selected_tab:C,selected_tab_index:w}=getContext(a),T=derived(()=>u??v);let E=derived(()=>d!==false&&d!==`hidden`);e.push(`<div${attr(`id`,s)}${attr_class(`tabitem ${stringify(c.join(` `))}`,`svelte-dmtrd3`,{"grow-children":h>=1,scrollable:g||_})} role="tabpanel"${attr_style(``,{display:store_get(o??={},`$selected_tab`,C)===T()&&E()?`flex`:`none`,"flex-grow":h,height:g,"max-height":_})}>`),r(e,{scale:h>=1?h:null,children:e=>{b?.(e),e.push(`<!---->`);},$$slots:{default:true}}),e.push(`<!----></div>`),o&&unsubscribe_stores(o);});}function s(n,r){n.component(n=>{let{$$slots:i,$$events:a,...s}=r,c=new h(s);o(n,{elem_id:c.shared.elem_id,elem_classes:c.shared.elem_classes,label:c.shared.label,visible:c.shared.visible,interactive:c.shared.interactive,id:c.props.id,order:c.props.order,alignment:c.props.alignment,scale:c.shared.scale,height:c.props.height==null?void 0:g(c.props.height),max_height:c.props.max_height==null?void 0:g(c.props.max_height),component_id:c.props.component_id,onselect:e=>c.dispatch(`select`,e),children:e=>{s.children?.(e),e.push(`<!---->`);}});});}

export { o as BaseTabItem, s as default };
//# sourceMappingURL=Index58-D-XUfbuA.js.map
