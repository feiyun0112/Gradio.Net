import{P as F,a as G}from"./Plot-MgEXJrZ-.js";import{B as H}from"./Block-rEXcgPfT.js";import"./MarkdownCode.svelte_svelte_type_style_lang-CaEo_I2i.js";import"./IconButtonWrapper.svelte_svelte_type_style_lang-DAP8_Zsr.js";import{B as J}from"./BlockLabel-CnzaitFN.js";import{S as K}from"./index-DbBaXY2J.js";import"./StreamingBar.svelte_svelte_type_style_lang-CxOfZBE-.js";import"./index-Ya43XIXp.js";import"./svelte/svelte.js";import"./Empty-CMV1fpYf.js";import"./prism-python-DMUsXzi0.js";import"./IconButton-DtUbToT-.js";import"./Clear-By3xiIwg.js";const{SvelteComponent:M,assign:N,create_component:w,destroy_component:d,detach:C,flush:h,get_spread_object:O,get_spread_update:Q,init:R,insert:L,mount_component:k,safe_not_equal:T,space:z,transition_in:v,transition_out:B}=window.__gradio__svelte__internal;function U(l){let e,n,i,_,o,u;e=new J({props:{show_label:l[6],label:l[5]||l[13].i18n("plot.plot"),Icon:F}});const c=[{autoscroll:l[13].autoscroll},{i18n:l[13].i18n},l[4]];let f={};for(let t=0;t<c.length;t+=1)f=N(f,c[t]);return i=new K({props:f}),i.$on("clear_status",l[17]),o=new G({props:{value:l[0],theme_mode:l[10],caption:l[11],bokeh_version:l[12],show_actions_button:l[14],gradio:l[13],show_label:l[6],_selectable:l[15],x_lim:l[16]}}),o.$on("change",l[18]),o.$on("select",l[19]),{c(){w(e.$$.fragment),n=z(),w(i.$$.fragment),_=z(),w(o.$$.fragment)},m(t,a){k(e,t,a),L(t,n,a),k(i,t,a),L(t,_,a),k(o,t,a),u=!0},p(t,a){const r={};a&64&&(r.show_label=t[6]),a&8224&&(r.label=t[5]||t[13].i18n("plot.plot")),e.$set(r);const g=a&8208?Q(c,[a&8192&&{autoscroll:t[13].autoscroll},a&8192&&{i18n:t[13].i18n},a&16&&O(t[4])]):{};i.$set(g);const m={};a&1&&(m.value=t[0]),a&1024&&(m.theme_mode=t[10]),a&2048&&(m.caption=t[11]),a&4096&&(m.bokeh_version=t[12]),a&16384&&(m.show_actions_button=t[14]),a&8192&&(m.gradio=t[13]),a&64&&(m.show_label=t[6]),a&32768&&(m._selectable=t[15]),a&65536&&(m.x_lim=t[16]),o.$set(m)},i(t){u||(v(e.$$.fragment,t),v(i.$$.fragment,t),v(o.$$.fragment,t),u=!0)},o(t){B(e.$$.fragment,t),B(i.$$.fragment,t),B(o.$$.fragment,t),u=!1},d(t){t&&(C(n),C(_)),d(e,t),d(i,t),d(o,t)}}}function V(l){let e,n;return e=new H({props:{padding:!1,elem_id:l[1],elem_classes:l[2],visible:l[3],container:l[7],scale:l[8],min_width:l[9],allow_overflow:!1,$$slots:{default:[U]},$$scope:{ctx:l}}}),{c(){w(e.$$.fragment)},m(i,_){k(e,i,_),n=!0},p(i,[_]){const o={};_&2&&(o.elem_id=i[1]),_&4&&(o.elem_classes=i[2]),_&8&&(o.visible=i[3]),_&128&&(o.container=i[7]),_&256&&(o.scale=i[8]),_&512&&(o.min_width=i[9]),_&1178737&&(o.$$scope={dirty:_,ctx:i}),e.$set(o)},i(i){n||(v(e.$$.fragment,i),n=!0)},o(i){B(e.$$.fragment,i),n=!1},d(i){d(e,i)}}}function W(l,e,n){let{value:i=null}=e,{elem_id:_=""}=e,{elem_classes:o=[]}=e,{visible:u=!0}=e,{loading_status:c}=e,{label:f}=e,{show_label:t}=e,{container:a=!0}=e,{scale:r=null}=e,{min_width:g=void 0}=e,{theme_mode:m}=e,{caption:P}=e,{bokeh_version:S}=e,{gradio:b}=e,{show_actions_button:I=!1}=e,{_selectable:j=!1}=e,{x_lim:q=null}=e;const A=()=>b.dispatch("clear_status",c),D=()=>b.dispatch("change"),E=s=>b.dispatch("select",s.detail);return l.$$set=s=>{"value"in s&&n(0,i=s.value),"elem_id"in s&&n(1,_=s.elem_id),"elem_classes"in s&&n(2,o=s.elem_classes),"visible"in s&&n(3,u=s.visible),"loading_status"in s&&n(4,c=s.loading_status),"label"in s&&n(5,f=s.label),"show_label"in s&&n(6,t=s.show_label),"container"in s&&n(7,a=s.container),"scale"in s&&n(8,r=s.scale),"min_width"in s&&n(9,g=s.min_width),"theme_mode"in s&&n(10,m=s.theme_mode),"caption"in s&&n(11,P=s.caption),"bokeh_version"in s&&n(12,S=s.bokeh_version),"gradio"in s&&n(13,b=s.gradio),"show_actions_button"in s&&n(14,I=s.show_actions_button),"_selectable"in s&&n(15,j=s._selectable),"x_lim"in s&&n(16,q=s.x_lim)},[i,_,o,u,c,f,t,a,r,g,m,P,S,b,I,j,q,A,D,E]}class ae extends M{constructor(e){super(),R(this,e,W,V,T,{value:0,elem_id:1,elem_classes:2,visible:3,loading_status:4,label:5,show_label:6,container:7,scale:8,min_width:9,theme_mode:10,caption:11,bokeh_version:12,gradio:13,show_actions_button:14,_selectable:15,x_lim:16})}get value(){return this.$$.ctx[0]}set value(e){this.$$set({value:e}),h()}get elem_id(){return this.$$.ctx[1]}set elem_id(e){this.$$set({elem_id:e}),h()}get elem_classes(){return this.$$.ctx[2]}set elem_classes(e){this.$$set({elem_classes:e}),h()}get visible(){return this.$$.ctx[3]}set visible(e){this.$$set({visible:e}),h()}get loading_status(){return this.$$.ctx[4]}set loading_status(e){this.$$set({loading_status:e}),h()}get label(){return this.$$.ctx[5]}set label(e){this.$$set({label:e}),h()}get show_label(){return this.$$.ctx[6]}set show_label(e){this.$$set({show_label:e}),h()}get container(){return this.$$.ctx[7]}set container(e){this.$$set({container:e}),h()}get scale(){return this.$$.ctx[8]}set scale(e){this.$$set({scale:e}),h()}get min_width(){return this.$$.ctx[9]}set min_width(e){this.$$set({min_width:e}),h()}get theme_mode(){return this.$$.ctx[10]}set theme_mode(e){this.$$set({theme_mode:e}),h()}get caption(){return this.$$.ctx[11]}set caption(e){this.$$set({caption:e}),h()}get bokeh_version(){return this.$$.ctx[12]}set bokeh_version(e){this.$$set({bokeh_version:e}),h()}get gradio(){return this.$$.ctx[13]}set gradio(e){this.$$set({gradio:e}),h()}get show_actions_button(){return this.$$.ctx[14]}set show_actions_button(e){this.$$set({show_actions_button:e}),h()}get _selectable(){return this.$$.ctx[15]}set _selectable(e){this.$$set({_selectable:e}),h()}get x_lim(){return this.$$.ctx[16]}set x_lim(e){this.$$set({x_lim:e}),h()}}export{G as BasePlot,ae as default};
//# sourceMappingURL=Index-KTewMdAj.js.map