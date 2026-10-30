import { K as h } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import { n, p as m } from './src3-C1zEyqUA.js';
import { s as spread_props, f as attr, e as escape_html, d as derived } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

var a=0;function o(o,s){o.component(o=>{let{$$slots:c,$$events:l,...u}=s,d=new h(u);d.props.value,d.props.value;let f=`range_id_${a++}`,p$1=derived(()=>d.props.minimum??0);let m$1=derived(()=>!d.shared.interactive);n(o,{visible:d.shared.visible,elem_id:d.shared.elem_id,elem_classes:d.shared.elem_classes,container:d.shared.container,scale:d.shared.scale,min_width:d.shared.min_width,children:e=>{p(e,spread_props([{autoscroll:d.shared.autoscroll,i18n:d.i18n},d.shared.loading_status,{on_clear_status:()=>d.dispatch(`clear_status`,d.shared.loading_status)}])),e.push(`<!----> <div class="wrap svelte-8epfm4"><div class="head svelte-8epfm4"><label${attr(`for`,f)} class="svelte-8epfm4">`),m(e,{show_label:d.shared.show_label,info:d.props.info,children:e=>{e.push(`<!---->${escape_html(d.shared.label||`Slider`)}`);}}),e.push(`<!----></label> <div class="tab-like-container svelte-8epfm4"><input${attr(`aria-label`,`number input for ${d.shared.label}`)} data-testid="number-input" type="number"${attr(`value`,d.props.value)}${attr(`min`,d.props.minimum)}${attr(`max`,d.props.maximum)}${attr(`step`,d.props.step)}${attr(`disabled`,m$1(),true)} class="svelte-8epfm4"/> `),d.props.buttons?.includes(`reset`)??true?(e.push(`<!--[0-->`),e.push(`<button class="reset-button svelte-8epfm4"${attr(`disabled`,m$1(),true)} aria-label="Reset to default value" data-testid="reset-button">↺</button>`)):e.push(`<!--[-1-->`),e.push(`<!--]--></div></div> <div class="slider_input_container svelte-8epfm4"><span class="min_value svelte-8epfm4" data-testid="min-value">${escape_html(p$1())}</span> <input type="range"${attr(`id`,f)} name="cowbell" data-testid="range-input"${attr(`value`,d.props.value)}${attr(`min`,d.props.minimum)}${attr(`max`,d.props.maximum)}${attr(`step`,d.props.step)}${attr(`disabled`,m$1(),true)}${attr(`aria-label`,`range slider for ${d.shared.label}`)} class="svelte-8epfm4"/> <span class="max_value svelte-8epfm4" data-testid="max-value">${escape_html(d.props.maximum)}</span></div></div>`);},$$slots:{default:true}});});}

export { o as default };
//# sourceMappingURL=Index54-BG87VE1D.js.map
