import { K as h, L as g } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { n } from './src3-C1zEyqUA.js';
import { r } from './Index7-B978gaB_.js';
import { s as spread_props, a as attr_class, e as escape_html, b as attr_style, c as bind_props, d as derived } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

function o(e,t){e.component(e=>{let{open:n=true,label:r=``,height:i,max_height:o,onexpand:s,oncollapse:c,children:l}=t;e.push(`<button${attr_class(`label-wrap svelte-e5lyqv`,void 0,{open:n})}><span class="svelte-e5lyqv">${escape_html(r)}</span> <span class="icon svelte-e5lyqv"${attr_style(``,{transform:n?`rotate(0)`:`rotate(90deg)`})}>▼</span></button> <div data-testid="accordion-content"${attr_class(`svelte-e5lyqv`,void 0,{scrollable:i||o})}${attr_style(``,{display:n?`block`:`none`,height:i,"max-height":o})}>`),l?.(e),e.push(`<!----></div>`),bind_props(t,{open:n});});}function s(s,c){s.component(s=>{let{$$slots:l,$$events:u,...d}=c;class f extends h{set_data(e){let t=this.props.open;super.set_data(e),`open`in e&&e.open!==t&&this.dispatch(e.open?`expand`:`collapse`),this.shared.loading_status.status=`complete`;}}let p$1=new f(d),m=derived(()=>p$1.shared.label||``),h$1=derived(()=>[...p$1.shared.elem_classes||[],`gr-accordion`]),g$1=derived(()=>p$1.shared.visible===true?true:`hidden`),_=true,v;function y(t){n(t,{elem_id:p$1.shared.elem_id,elem_classes:h$1(),visible:g$1(),children:t=>{p$1.shared.loading_status?(t.push(`<!--[0-->`),p(t,spread_props([{autoscroll:p$1.shared.autoscroll,i18n:p$1.i18n},p$1.shared.loading_status]))):t.push(`<!--[-1-->`),t.push(`<!--]--> `),o(t,{label:m(),height:p$1.props.height==null?void 0:g(p$1.props.height),max_height:p$1.props.max_height==null?void 0:g(p$1.props.max_height),onexpand:()=>p$1.dispatch(`expand`),oncollapse:()=>p$1.dispatch(`collapse`),get open(){return p$1.props.open},set open(e){p$1.props.open=e,_=false;},children:e=>{r(e,{children:e=>{d.children?.(e),e.push(`<!---->`);},$$slots:{default:true}});},$$slots:{default:true}}),t.push(`<!---->`);},$$slots:{default:true}});}do _=true,v=s.copy(),y(v);while(!_);s.subsume(v);});}

export { s as default };
//# sourceMappingURL=Index9-7oZWkYud.js.map
