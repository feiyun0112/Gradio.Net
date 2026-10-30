import { K as h } from './2-C9eYyMgE.js';
import { p } from './statustracker-Bh1XNgjP.js';
import './async-Cv1-GZGV.js';
import { f as attr, a as attr_class, i as stringify, b as attr_style, s as spread_props, d as derived } from './renderer-RGQlaTg4.js';

function r(e,r){e.component(e=>{let{$$slots:i,$$events:a,...o}=r,s=derived(()=>o.scale??null),c=derived(()=>o.min_width??0),l=derived(()=>o.elem_id??``),u=derived(()=>o.elem_classes??[]),d=derived(()=>o.visible??true),f=derived(()=>o.variant??`default`),p$1=derived(()=>o.loading_status);e.push(`<div${attr(`id`,l())}${attr_class(`column ${stringify(u().join(` `))}`,`svelte-siq5d6`,{compact:f()===`compact`,panel:f()===`panel`,hide:!d()})}${attr_style(``,{"flex-grow":s(),"min-width":`calc(min(${stringify(c())}px, 100%))`})}>`),p$1()&&p$1().show_progress?(e.push(`<!--[0-->`),p(e,spread_props([{autoscroll:o.autoscroll??false,i18n:o.i18n??(e=>e)},p$1(),{queue_size:p$1().queue_size??null,status:p$1()?p$1().status==`pending`?`generating`:p$1().status:null}]))):e.push(`<!--[-1-->`),e.push(`<!--]--> `),o.children?.(e),e.push(`<!----></div>`);});}function i(t,i){t.component(t=>{let{$$slots:a,$$events:o,...s}=i,c=new h(s);r(t,spread_props([c.shared,c.props,{children:e=>{s.children?.(e),e.push(`<!---->`);},$$slots:{default:true}}]));});}

export { i, r };
//# sourceMappingURL=Index7-B978gaB_.js.map
