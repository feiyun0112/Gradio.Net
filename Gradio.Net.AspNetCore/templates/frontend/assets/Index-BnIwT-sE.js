import{g as kl}from"./color-BEs7q1FJ.js";import{c as He,S as vl}from"./Index-CLipTJLN.js";import{B as wl}from"./Button-C3qvSDkM.js";import{B as yl}from"./BlockLabel-DqfBpYep.js";import{E as $l}from"./Empty-B5pQ_UYC.js";import"./index-Cr9C6grF.js";import"./svelte/svelte.js";import"./_commonjsHelpers-BosuxZz1.js";const{SvelteComponent:_t,append:Be,attr:A,detach:dt,init:ht,insert:gt,noop:Ce,safe_not_equal:mt,svg_element:Te}=window.__gradio__svelte__internal;function bt(l){let e,t,n;return{c(){e=Te("svg"),t=Te("path"),n=Te("path"),A(t,"fill","currentColor"),A(t,"d","M12 15H5a3 3 0 0 1-3-3v-2a3 3 0 0 1 3-3h5V5a1 1 0 0 0-1-1H3V2h6a3 3 0 0 1 3 3zM5 9a1 1 0 0 0-1 1v2a1 1 0 0 0 1 1h5V9zm15 14v2a1 1 0 0 0 1 1h5v-4h-5a1 1 0 0 0-1 1z"),A(n,"fill","currentColor"),A(n,"d","M2 30h28V2Zm26-2h-7a3 3 0 0 1-3-3v-2a3 3 0 0 1 3-3h5v-2a1 1 0 0 0-1-1h-6v-2h6a3 3 0 0 1 3 3Z"),A(e,"xmlns","http://www.w3.org/2000/svg"),A(e,"xmlns:xlink","http://www.w3.org/1999/xlink"),A(e,"aria-hidden","true"),A(e,"role","img"),A(e,"class","iconify iconify--carbon"),A(e,"width","100%"),A(e,"height","100%"),A(e,"preserveAspectRatio","xMidYMid meet"),A(e,"viewBox","0 0 32 32")},m(s,i){gt(s,e,i),Be(e,t),Be(e,n)},p:Ce,i:Ce,o:Ce,d(s){s&&dt(e)}}}class je extends _t{constructor(e){super(),ht(this,e,null,bt,mt,{})}}function Ie(l,e,t){if(!t){var n=document.createElement("canvas");t=n.getContext("2d")}t.fillStyle=l,t.fillRect(0,0,1,1);const[s,i,o]=t.getImageData(0,0,1,1).data;return t.clearRect(0,0,1,1),`rgba(${s}, ${i}, ${o}, ${255/e})`}function jl(l,e,t,n){for(const s in l){const i=l[s].trim();i in He?e[s]=He[i]:e[s]={primary:t?Ie(l[s],1,n):l[s],secondary:t?Ie(l[s],.5,n):l[s]}}}function Sl(l,e){let t=[],n=null,s=null;for(const i of l)s===i.class_or_confidence?n=n?n+i.token:i.token:(n!==null&&t.push({token:n,class_or_confidence:s}),n=i.token,s=i.class_or_confidence);return n!==null&&t.push({token:n,class_or_confidence:s}),t}const{SvelteComponent:pt,append:Q,attr:V,destroy_each:Se,detach:B,element:Z,empty:zl,ensure_array_like:x,flush:be,init:kt,insert:I,listen:fe,noop:qe,run_all:vt,safe_not_equal:wt,set_data:Ne,set_style:ke,space:re,text:ue,toggle_class:X}=window.__gradio__svelte__internal,{createEventDispatcher:yt}=window.__gradio__svelte__internal;function Re(l,e,t){const n=l.slice();n[17]=e[t];const s=typeof n[17].class_or_confidence=="string"?parseInt(n[17].class_or_confidence):n[17].class_or_confidence;return n[26]=s,n}function De(l,e,t){const n=l.slice();return n[17]=e[t],n[19]=t,n}function Ae(l,e,t){const n=l.slice();return n[20]=e[t],n[22]=t,n}function Fe(l,e,t){const n=l.slice();return n[23]=e[t][0],n[24]=e[t][1],n[19]=t,n}function $t(l){let e,t,n=l[1]&&Ze(),s=x(l[0]),i=[];for(let o=0;o<s.length;o+=1)i[o]=Ke(Re(l,s,o));return{c(){n&&n.c(),e=re(),t=Z("div");for(let o=0;o<i.length;o+=1)i[o].c();V(t,"class","textfield svelte-ju12zg"),V(t,"data-testid","highlighted-text:textfield")},m(o,a){n&&n.m(o,a),I(o,e,a),I(o,t,a);for(let r=0;r<i.length;r+=1)i[r]&&i[r].m(t,null)},p(o,a){if(o[1]?n||(n=Ze(),n.c(),n.m(e.parentNode,e)):n&&(n.d(1),n=null),a&1){s=x(o[0]);let r;for(r=0;r<s.length;r+=1){const f=Re(o,s,r);i[r]?i[r].p(f,a):(i[r]=Ke(f),i[r].c(),i[r].m(t,null))}for(;r<i.length;r+=1)i[r].d(1);i.length=s.length}},d(o){o&&(B(e),B(t)),n&&n.d(o),Se(i,o)}}}function jt(l){let e,t,n=l[1]&&Pe(l),s=x(l[0]),i=[];for(let o=0;o<s.length;o+=1)i[o]=We(De(l,s,o));return{c(){n&&n.c(),e=re(),t=Z("div");for(let o=0;o<i.length;o+=1)i[o].c();V(t,"class","textfield svelte-ju12zg")},m(o,a){n&&n.m(o,a),I(o,e,a),I(o,t,a);for(let r=0;r<i.length;r+=1)i[r]&&i[r].m(t,null)},p(o,a){if(o[1]?n?n.p(o,a):(n=Pe(o),n.c(),n.m(e.parentNode,e)):n&&(n.d(1),n=null),a&111){s=x(o[0]);let r;for(r=0;r<s.length;r+=1){const f=De(o,s,r);i[r]?i[r].p(f,a):(i[r]=We(f),i[r].c(),i[r].m(t,null))}for(;r<i.length;r+=1)i[r].d(1);i.length=s.length}},d(o){o&&(B(e),B(t)),n&&n.d(o),Se(i,o)}}}function Ze(l){let e;return{c(){e=Z("div"),e.innerHTML="<span>-1</span> <span>0</span> <span>+1</span>",V(e,"class","color-legend svelte-ju12zg"),V(e,"data-testid","highlighted-text:color-legend")},m(t,n){I(t,e,n)},d(t){t&&B(e)}}}function Ke(l){let e,t,n=l[17].token+"",s,i,o;return{c(){e=Z("span"),t=Z("span"),s=ue(n),i=re(),V(t,"class","text svelte-ju12zg"),V(e,"class","textspan score-text svelte-ju12zg"),V(e,"style",o="background-color: rgba("+(l[26]&&l[26]<0?"128, 90, 213,"+-l[26]:"239, 68, 60,"+l[26])+")")},m(a,r){I(a,e,r),Q(e,t),Q(t,s),Q(e,i)},p(a,r){r&1&&n!==(n=a[17].token+"")&&Ne(s,n),r&1&&o!==(o="background-color: rgba("+(a[26]&&a[26]<0?"128, 90, 213,"+-a[26]:"239, 68, 60,"+a[26])+")")&&V(e,"style",o)},d(a){a&&B(e)}}}function Pe(l){let e,t=x(Object.entries(l[5])),n=[];for(let s=0;s<t.length;s+=1)n[s]=Ue(Fe(l,t,s));return{c(){e=Z("div");for(let s=0;s<n.length;s+=1)n[s].c();V(e,"class","category-legend svelte-ju12zg"),V(e,"data-testid","highlighted-text:category-legend")},m(s,i){I(s,e,i);for(let o=0;o<n.length;o+=1)n[o]&&n[o].m(e,null)},p(s,i){if(i&416){t=x(Object.entries(s[5]));let o;for(o=0;o<t.length;o+=1){const a=Fe(s,t,o);n[o]?n[o].p(a,i):(n[o]=Ue(a),n[o].c(),n[o].m(e,null))}for(;o<n.length;o+=1)n[o].d(1);n.length=t.length}},d(s){s&&B(e),Se(n,s)}}}function Ue(l){let e,t=l[23]+"",n,s,i,o;function a(){return l[10](l[23])}function r(){return l[11](l[23])}return{c(){e=Z("div"),n=ue(t),s=re(),V(e,"class","category-label svelte-ju12zg"),V(e,"style","background-color:"+l[24].secondary)},m(f,c){I(f,e,c),Q(e,n),Q(e,s),i||(o=[fe(e,"mouseover",a),fe(e,"focus",r),fe(e,"mouseout",l[12]),fe(e,"blur",l[13])],i=!0)},p(f,c){l=f},d(f){f&&B(e),i=!1,vt(o)}}}function Ye(l){let e,t,n=l[20]+"",s,i,o,a,r=!l[1]&&l[17].class_or_confidence!==null&&Ge(l);function f(){return l[14](l[19],l[17])}return{c(){e=Z("span"),t=Z("span"),s=ue(n),i=re(),r&&r.c(),V(t,"class","text svelte-ju12zg"),X(t,"no-label",l[17].class_or_confidence===null||!l[5][l[17].class_or_confidence]),V(e,"class","textspan svelte-ju12zg"),X(e,"no-cat",l[17].class_or_confidence===null||l[3]&&l[3]!==l[17].class_or_confidence),X(e,"hl",l[17].class_or_confidence!==null),X(e,"selectable",l[2]),ke(e,"background-color",l[17].class_or_confidence===null||l[3]&&l[3]!==l[17].class_or_confidence?"":l[5][l[17].class_or_confidence].secondary)},m(c,g){I(c,e,g),Q(e,t),Q(t,s),Q(e,i),r&&r.m(e,null),o||(a=fe(e,"click",f),o=!0)},p(c,g){l=c,g&1&&n!==(n=l[20]+"")&&Ne(s,n),g&33&&X(t,"no-label",l[17].class_or_confidence===null||!l[5][l[17].class_or_confidence]),!l[1]&&l[17].class_or_confidence!==null?r?r.p(l,g):(r=Ge(l),r.c(),r.m(e,null)):r&&(r.d(1),r=null),g&9&&X(e,"no-cat",l[17].class_or_confidence===null||l[3]&&l[3]!==l[17].class_or_confidence),g&1&&X(e,"hl",l[17].class_or_confidence!==null),g&4&&X(e,"selectable",l[2]),g&9&&ke(e,"background-color",l[17].class_or_confidence===null||l[3]&&l[3]!==l[17].class_or_confidence?"":l[5][l[17].class_or_confidence].secondary)},d(c){c&&B(e),r&&r.d(),o=!1,a()}}}function Ge(l){let e,t,n=l[17].class_or_confidence+"",s;return{c(){e=ue(` 
								`),t=Z("span"),s=ue(n),V(t,"class","label svelte-ju12zg"),ke(t,"background-color",l[17].class_or_confidence===null||l[3]&&l[3]!==l[17].class_or_confidence?"":l[5][l[17].class_or_confidence].primary)},m(i,o){I(i,e,o),I(i,t,o),Q(t,s)},p(i,o){o&1&&n!==(n=i[17].class_or_confidence+"")&&Ne(s,n),o&9&&ke(t,"background-color",i[17].class_or_confidence===null||i[3]&&i[3]!==i[17].class_or_confidence?"":i[5][i[17].class_or_confidence].primary)},d(i){i&&(B(e),B(t))}}}function Je(l){let e;return{c(){e=Z("br")},m(t,n){I(t,e,n)},d(t){t&&B(e)}}}function Qe(l){let e=l[20].trim()!=="",t,n=l[22]<ve(l[17].token).length-1,s,i=e&&Ye(l),o=n&&Je();return{c(){i&&i.c(),t=re(),o&&o.c(),s=zl()},m(a,r){i&&i.m(a,r),I(a,t,r),o&&o.m(a,r),I(a,s,r)},p(a,r){r&1&&(e=a[20].trim()!==""),e?i?i.p(a,r):(i=Ye(a),i.c(),i.m(t.parentNode,t)):i&&(i.d(1),i=null),r&1&&(n=a[22]<ve(a[17].token).length-1),n?o||(o=Je(),o.c(),o.m(s.parentNode,s)):o&&(o.d(1),o=null)},d(a){a&&(B(t),B(s)),i&&i.d(a),o&&o.d(a)}}}function We(l){let e,t=x(ve(l[17].token)),n=[];for(let s=0;s<t.length;s+=1)n[s]=Qe(Ae(l,t,s));return{c(){for(let s=0;s<n.length;s+=1)n[s].c();e=zl()},m(s,i){for(let o=0;o<n.length;o+=1)n[o]&&n[o].m(s,i);I(s,e,i)},p(s,i){if(i&111){t=x(ve(s[17].token));let o;for(o=0;o<t.length;o+=1){const a=Ae(s,t,o);n[o]?n[o].p(a,i):(n[o]=Qe(a),n[o].c(),n[o].m(e.parentNode,e))}for(;o<n.length;o+=1)n[o].d(1);n.length=t.length}},d(s){s&&B(e),Se(n,s)}}}function St(l){let e;function t(i,o){return i[4]==="categories"?jt:$t}let n=t(l),s=n(l);return{c(){e=Z("div"),s.c(),V(e,"class","container svelte-ju12zg")},m(i,o){I(i,e,o),s.m(e,null)},p(i,[o]){n===(n=t(i))&&s?s.p(i,o):(s.d(1),s=n(i),s&&(s.c(),s.m(e,null)))},i:qe,o:qe,d(i){i&&B(e),s.d()}}}function ve(l){return l.split(`
`)}function zt(l,e,t){const n=typeof document<"u";let{value:s=[]}=e,{show_legend:i=!1}=e,{color_map:o={}}=e,{selectable:a=!1}=e,r,f={},c="";const g=yt();let p;function m(b){t(3,c=b)}function u(){t(3,c="")}const d=b=>m(b),v=b=>m(b),S=()=>u(),y=()=>u(),h=(b,$)=>{g("select",{index:b,value:[$.token,$.class_or_confidence]})};return l.$$set=b=>{"value"in b&&t(0,s=b.value),"show_legend"in b&&t(1,i=b.show_legend),"color_map"in b&&t(9,o=b.color_map),"selectable"in b&&t(2,a=b.selectable)},l.$$.update=()=>{if(l.$$.dirty&513){if(o||t(9,o={}),s.length>0){for(let b of s)if(b.class_or_confidence!==null)if(typeof b.class_or_confidence=="string"){if(t(4,p="categories"),!(b.class_or_confidence in o)){let $=kl(Object.keys(o).length);t(9,o[b.class_or_confidence]=$,o)}}else t(4,p="scores")}jl(o,f,n,r)}},[s,i,a,c,p,f,g,m,u,o,d,v,S,y,h]}class Et extends pt{constructor(e){super(),kt(this,e,zt,St,wt,{value:0,show_legend:1,color_map:9,selectable:2})}get value(){return this.$$.ctx[0]}set value(e){this.$$set({value:e}),be()}get show_legend(){return this.$$.ctx[1]}set show_legend(e){this.$$set({show_legend:e}),be()}get color_map(){return this.$$.ctx[9]}set color_map(e){this.$$set({color_map:e}),be()}get selectable(){return this.$$.ctx[2]}set selectable(e){this.$$set({selectable:e}),be()}}const Ct=Et,{SvelteComponent:Tt,attr:Y,detach:Oe,element:El,empty:Nt,flush:J,init:Ot,insert:Le,listen:le,noop:Xe,run_all:Cl,safe_not_equal:Lt,set_style:ie}=window.__gradio__svelte__internal;function Mt(l){let e,t,n,s;return{c(){e=El("input"),Y(e,"class","label-input svelte-1cag2po"),e.autofocus=!0,Y(e,"type","number"),Y(e,"step","0.1"),Y(e,"style",t="background-color: rgba("+(typeof l[1]=="number"&&l[1]<0?"128, 90, 213,"+-l[1]:"239, 68, 60,"+l[1])+")"),e.value=l[1],ie(e,"width","7ch")},m(i,o){Le(i,e,o),e.focus(),n||(s=[le(e,"input",l[8]),le(e,"blur",l[14]),le(e,"keydown",l[15])],n=!0)},p(i,o){o&2&&t!==(t="background-color: rgba("+(typeof i[1]=="number"&&i[1]<0?"128, 90, 213,"+-i[1]:"239, 68, 60,"+i[1])+")")&&Y(e,"style",t),o&2&&e.value!==i[1]&&(e.value=i[1]);const a=o&2;(o&2||a)&&ie(e,"width","7ch")},d(i){i&&Oe(e),n=!1,Cl(s)}}}function Vt(l){let e,t,n,s;return{c(){e=El("input"),Y(e,"class","label-input svelte-1cag2po"),e.autofocus=!0,Y(e,"id",t=`label-input-${l[3]}`),Y(e,"type","text"),Y(e,"placeholder","label"),e.value=l[1],ie(e,"background-color",l[1]===null||l[2]&&l[2]!==l[1]?"":l[6][l[1]].primary),ie(e,"width",l[7]?l[7].toString()?.length+4+"ch":"8ch")},m(i,o){Le(i,e,o),e.focus(),n||(s=[le(e,"input",l[8]),le(e,"blur",l[12]),le(e,"keydown",l[13]),le(e,"focus",Bt)],n=!0)},p(i,o){o&8&&t!==(t=`label-input-${i[3]}`)&&Y(e,"id",t),o&2&&e.value!==i[1]&&(e.value=i[1]),o&70&&ie(e,"background-color",i[1]===null||i[2]&&i[2]!==i[1]?"":i[6][i[1]].primary),o&128&&ie(e,"width",i[7]?i[7].toString()?.length+4+"ch":"8ch")},d(i){i&&Oe(e),n=!1,Cl(s)}}}function Ht(l){let e;function t(i,o){return i[5]?Mt:Vt}let n=t(l),s=n(l);return{c(){s.c(),e=Nt()},m(i,o){s.m(i,o),Le(i,e,o)},p(i,[o]){n===(n=t(i))&&s?s.p(i,o):(s.d(1),s=n(i),s&&(s.c(),s.m(e.parentNode,e)))},i:Xe,o:Xe,d(i){i&&Oe(e),s.d(i)}}}function Bt(l){let e=l.target;e&&e.placeholder&&(e.placeholder="")}function It(l,e,t){let{value:n}=e,{category:s}=e,{active:i}=e,{labelToEdit:o}=e,{indexOfLabel:a}=e,{text:r}=e,{handleValueChange:f}=e,{isScoresMode:c=!1}=e,{_color_map:g}=e,p=s;function m(h){let b=h.target;b&&t(7,p=b.value)}function u(h,b,$){let L=h.target;t(10,n=[...n.slice(0,b),{token:$,class_or_confidence:L.value===""?null:c?Number(L.value):L.value},...n.slice(b+1)]),f()}const d=h=>u(h,a,r),v=h=>{h.key==="Enter"&&(u(h,a,r),t(0,o=-1))},S=h=>u(h,a,r),y=h=>{h.key==="Enter"&&(u(h,a,r),t(0,o=-1))};return l.$$set=h=>{"value"in h&&t(10,n=h.value),"category"in h&&t(1,s=h.category),"active"in h&&t(2,i=h.active),"labelToEdit"in h&&t(0,o=h.labelToEdit),"indexOfLabel"in h&&t(3,a=h.indexOfLabel),"text"in h&&t(4,r=h.text),"handleValueChange"in h&&t(11,f=h.handleValueChange),"isScoresMode"in h&&t(5,c=h.isScoresMode),"_color_map"in h&&t(6,g=h._color_map)},[o,s,i,a,r,c,g,p,m,u,n,f,d,v,S,y]}class Tl extends Tt{constructor(e){super(),Ot(this,e,It,Ht,Lt,{value:10,category:1,active:2,labelToEdit:0,indexOfLabel:3,text:4,handleValueChange:11,isScoresMode:5,_color_map:6})}get value(){return this.$$.ctx[10]}set value(e){this.$$set({value:e}),J()}get category(){return this.$$.ctx[1]}set category(e){this.$$set({category:e}),J()}get active(){return this.$$.ctx[2]}set active(e){this.$$set({active:e}),J()}get labelToEdit(){return this.$$.ctx[0]}set labelToEdit(e){this.$$set({labelToEdit:e}),J()}get indexOfLabel(){return this.$$.ctx[3]}set indexOfLabel(e){this.$$set({indexOfLabel:e}),J()}get text(){return this.$$.ctx[4]}set text(e){this.$$set({text:e}),J()}get handleValueChange(){return this.$$.ctx[11]}set handleValueChange(e){this.$$set({handleValueChange:e}),J()}get isScoresMode(){return this.$$.ctx[5]}set isScoresMode(e){this.$$set({isScoresMode:e}),J()}get _color_map(){return this.$$.ctx[6]}set _color_map(e){this.$$set({_color_map:e}),J()}}const{SvelteComponent:qt,add_flush_callback:Nl,append:D,attr:k,bind:Ol,binding_callbacks:Ll,check_outros:oe,create_component:Ml,destroy_component:Vl,destroy_each:ze,detach:N,element:H,empty:Me,ensure_array_like:ee,flush:pe,group_outros:se,init:Rt,insert:O,listen:z,mount_component:Hl,run_all:ae,safe_not_equal:Dt,set_data:Ee,set_style:we,space:G,text:he,toggle_class:F,transition_in:C,transition_out:q}=window.__gradio__svelte__internal,{createEventDispatcher:At,onMount:Ft}=window.__gradio__svelte__internal;function xe(l,e,t){const n=l.slice();n[45]=e[t].token,n[46]=e[t].class_or_confidence,n[48]=t;const s=typeof n[46]=="string"?parseInt(n[46]):n[46];return n[54]=s,n}function el(l,e,t){const n=l.slice();return n[45]=e[t].token,n[46]=e[t].class_or_confidence,n[48]=t,n}function ll(l,e,t){const n=l.slice();return n[49]=e[t],n[51]=t,n}function tl(l,e,t){const n=l.slice();return n[46]=e[t][0],n[52]=e[t][1],n[48]=t,n}function Zt(l){let e,t,n,s=l[1]&&nl(),i=ee(l[0]),o=[];for(let r=0;r<i.length;r+=1)o[r]=il(xe(l,i,r));const a=r=>q(o[r],1,1,()=>{o[r]=null});return{c(){s&&s.c(),e=G(),t=H("div");for(let r=0;r<o.length;r+=1)o[r].c();k(t,"class","textfield svelte-1ozsnjl"),k(t,"data-testid","highlighted-text:textfield")},m(r,f){s&&s.m(r,f),O(r,e,f),O(r,t,f);for(let c=0;c<o.length;c+=1)o[c]&&o[c].m(t,null);n=!0},p(r,f){if(r[1]?s||(s=nl(),s.c(),s.m(e.parentNode,e)):s&&(s.d(1),s=null),f[0]&889){i=ee(r[0]);let c;for(c=0;c<i.length;c+=1){const g=xe(r,i,c);o[c]?(o[c].p(g,f),C(o[c],1)):(o[c]=il(g),o[c].c(),C(o[c],1),o[c].m(t,null))}for(se(),c=i.length;c<o.length;c+=1)a(c);oe()}},i(r){if(!n){for(let f=0;f<i.length;f+=1)C(o[f]);n=!0}},o(r){o=o.filter(Boolean);for(let f=0;f<o.length;f+=1)q(o[f]);n=!1},d(r){r&&(N(e),N(t)),s&&s.d(r),ze(o,r)}}}function Kt(l){let e,t,n,s=l[1]&&rl(l),i=ee(l[0]),o=[];for(let r=0;r<i.length;r+=1)o[r]=ml(el(l,i,r));const a=r=>q(o[r],1,1,()=>{o[r]=null});return{c(){s&&s.c(),e=G(),t=H("div");for(let r=0;r<o.length;r+=1)o[r].c();k(t,"class","textfield svelte-1ozsnjl")},m(r,f){s&&s.m(r,f),O(r,e,f),O(r,t,f);for(let c=0;c<o.length;c+=1)o[c]&&o[c].m(t,null);n=!0},p(r,f){if(r[1]?s?s.p(r,f):(s=rl(r),s.c(),s.m(e.parentNode,e)):s&&(s.d(1),s=null),f[0]&13183){i=ee(r[0]);let c;for(c=0;c<i.length;c+=1){const g=el(r,i,c);o[c]?(o[c].p(g,f),C(o[c],1)):(o[c]=ml(g),o[c].c(),C(o[c],1),o[c].m(t,null))}for(se(),c=i.length;c<o.length;c+=1)a(c);oe()}},i(r){if(!n){for(let f=0;f<i.length;f+=1)C(o[f]);n=!0}},o(r){o=o.filter(Boolean);for(let f=0;f<o.length;f+=1)q(o[f]);n=!1},d(r){r&&(N(e),N(t)),s&&s.d(r),ze(o,r)}}}function nl(l){let e;return{c(){e=H("div"),e.innerHTML="<span>-1</span> <span>0</span> <span>+1</span>",k(e,"class","color-legend svelte-1ozsnjl"),k(e,"data-testid","highlighted-text:color-legend")},m(t,n){O(t,e,n)},d(t){t&&N(e)}}}function ol(l){let e,t,n;function s(o){l[32](o)}let i={labelToEdit:l[6],_color_map:l[3],category:l[46],active:l[5],indexOfLabel:l[48],text:l[45],handleValueChange:l[9],isScoresMode:!0};return l[0]!==void 0&&(i.value=l[0]),e=new Tl({props:i}),Ll.push(()=>Ol(e,"value",s)),{c(){Ml(e.$$.fragment)},m(o,a){Hl(e,o,a),n=!0},p(o,a){const r={};a[0]&64&&(r.labelToEdit=o[6]),a[0]&8&&(r._color_map=o[3]),a[0]&1&&(r.category=o[46]),a[0]&32&&(r.active=o[5]),a[0]&1&&(r.text=o[45]),!t&&a[0]&1&&(t=!0,r.value=o[0],Nl(()=>t=!1)),e.$set(r)},i(o){n||(C(e.$$.fragment,o),n=!0)},o(o){q(e.$$.fragment,o),n=!1},d(o){Vl(e,o)}}}function sl(l){let e,t,n;function s(){return l[37](l[48])}function i(...o){return l[38](l[48],...o)}return{c(){e=H("span"),e.textContent="×",k(e,"class","label-clear-button svelte-1ozsnjl"),k(e,"role","button"),k(e,"aria-roledescription","Remove label from text"),k(e,"tabindex","0")},m(o,a){O(o,e,a),t||(n=[z(e,"click",s),z(e,"keydown",i)],t=!0)},p(o,a){l=o},d(o){o&&N(e),t=!1,ae(n)}}}function il(l){let e,t,n,s=l[45]+"",i,o,a,r,f,c,g,p,m=l[46]&&l[6]===l[48]&&ol(l);function u(){return l[33](l[48])}function d(){return l[34](l[48])}function v(){return l[35](l[48])}function S(...h){return l[36](l[48],...h)}let y=l[46]&&l[4]===l[48]&&sl(l);return{c(){e=H("span"),t=H("span"),n=H("span"),i=he(s),o=G(),m&&m.c(),r=G(),y&&y.c(),f=G(),k(n,"class","text svelte-1ozsnjl"),k(t,"class","textspan score-text svelte-1ozsnjl"),k(t,"role","button"),k(t,"tabindex","0"),k(t,"style",a="background-color: rgba("+(l[54]&&l[54]<0?"128, 90, 213,"+-l[54]:"239, 68, 60,"+l[54])+")"),F(t,"no-cat",l[46]===null||l[5]&&l[5]!==l[46]),F(t,"hl",l[46]!==null),k(e,"class","score-text-container svelte-1ozsnjl")},m(h,b){O(h,e,b),D(e,t),D(t,n),D(n,i),D(t,o),m&&m.m(t,null),D(e,r),y&&y.m(e,null),D(e,f),c=!0,g||(p=[z(t,"mouseover",u),z(t,"focus",d),z(t,"click",v),z(t,"keydown",S)],g=!0)},p(h,b){l=h,(!c||b[0]&1)&&s!==(s=l[45]+"")&&Ee(i,s),l[46]&&l[6]===l[48]?m?(m.p(l,b),b[0]&65&&C(m,1)):(m=ol(l),m.c(),C(m,1),m.m(t,null)):m&&(se(),q(m,1,1,()=>{m=null}),oe()),(!c||b[0]&1&&a!==(a="background-color: rgba("+(l[54]&&l[54]<0?"128, 90, 213,"+-l[54]:"239, 68, 60,"+l[54])+")"))&&k(t,"style",a),(!c||b[0]&33)&&F(t,"no-cat",l[46]===null||l[5]&&l[5]!==l[46]),(!c||b[0]&1)&&F(t,"hl",l[46]!==null),l[46]&&l[4]===l[48]?y?y.p(l,b):(y=sl(l),y.c(),y.m(e,f)):y&&(y.d(1),y=null)},i(h){c||(C(m),c=!0)},o(h){q(m),c=!1},d(h){h&&N(e),m&&m.d(),y&&y.d(),g=!1,ae(p)}}}function rl(l){let e,t=l[3]&&al(l);return{c(){e=H("div"),t&&t.c(),k(e,"class","class_or_confidence-legend svelte-1ozsnjl"),k(e,"data-testid","highlighted-text:class_or_confidence-legend")},m(n,s){O(n,e,s),t&&t.m(e,null)},p(n,s){n[3]?t?t.p(n,s):(t=al(n),t.c(),t.m(e,null)):t&&(t.d(1),t=null)},d(n){n&&N(e),t&&t.d()}}}function al(l){let e,t=ee(Object.entries(l[3])),n=[];for(let s=0;s<t.length;s+=1)n[s]=cl(tl(l,t,s));return{c(){for(let s=0;s<n.length;s+=1)n[s].c();e=Me()},m(s,i){for(let o=0;o<n.length;o+=1)n[o]&&n[o].m(s,i);O(s,e,i)},p(s,i){if(i[0]&3080){t=ee(Object.entries(s[3]));let o;for(o=0;o<t.length;o+=1){const a=tl(s,t,o);n[o]?n[o].p(a,i):(n[o]=cl(a),n[o].c(),n[o].m(e.parentNode,e))}for(;o<n.length;o+=1)n[o].d(1);n.length=t.length}},d(s){s&&N(e),ze(n,s)}}}function cl(l){let e,t=l[46]+"",n,s,i,o,a;function r(){return l[15](l[46])}function f(){return l[16](l[46])}return{c(){e=H("div"),n=he(t),s=G(),k(e,"role","button"),k(e,"aria-roledescription","Categories of highlighted text. Hover to see text with this class_or_confidence highlighted."),k(e,"tabindex","0"),k(e,"class","class_or_confidence-label svelte-1ozsnjl"),k(e,"style",i="background-color:"+l[52].secondary)},m(c,g){O(c,e,g),D(e,n),D(e,s),o||(a=[z(e,"mouseover",r),z(e,"focus",f),z(e,"mouseout",l[17]),z(e,"blur",l[18])],o=!0)},p(c,g){l=c,g[0]&8&&t!==(t=l[46]+"")&&Ee(n,t),g[0]&8&&i!==(i="background-color:"+l[52].secondary)&&k(e,"style",i)},d(c){c&&N(e),o=!1,ae(a)}}}function fl(l){let e,t,n,s=l[49]+"",i,o,a,r,f,c,g;function p(){return l[20](l[48])}function m(){return l[21](l[48])}function u(){return l[22](l[48])}let d=!l[1]&&l[46]!==null&&l[6]!==l[48]&&ul(l),v=l[6]===l[48]&&l[46]!==null&&_l(l);function S(){return l[26](l[46],l[48],l[45])}function y(...L){return l[27](l[46],l[48],l[45],...L)}function h(){return l[28](l[48])}function b(){return l[29](l[48])}let $=l[46]!==null&&dl(l);return{c(){e=H("span"),t=H("span"),n=H("span"),i=he(s),o=G(),d&&d.c(),a=G(),v&&v.c(),r=G(),$&&$.c(),k(n,"class","text svelte-1ozsnjl"),k(n,"role","button"),k(n,"tabindex","0"),F(n,"no-label",l[46]===null),k(t,"role","button"),k(t,"tabindex","0"),k(t,"class","textspan svelte-1ozsnjl"),F(t,"no-cat",l[46]===null||l[5]&&l[5]!==l[46]),F(t,"hl",l[46]!==null),F(t,"selectable",l[2]),we(t,"background-color",l[46]===null||l[5]&&l[5]!==l[46]?"":l[46]&&l[3][l[46]]?l[3][l[46]].secondary:""),k(e,"class","text-class_or_confidence-container svelte-1ozsnjl")},m(L,M){O(L,e,M),D(e,t),D(t,n),D(n,i),D(t,o),d&&d.m(t,null),D(t,a),v&&v.m(t,null),D(e,r),$&&$.m(e,null),f=!0,c||(g=[z(n,"keydown",l[19]),z(n,"focus",p),z(n,"mouseover",m),z(n,"click",u),z(t,"click",S),z(t,"keydown",y),z(t,"focus",h),z(t,"mouseover",b)],c=!0)},p(L,M){l=L,(!f||M[0]&1)&&s!==(s=l[49]+"")&&Ee(i,s),(!f||M[0]&1)&&F(n,"no-label",l[46]===null),!l[1]&&l[46]!==null&&l[6]!==l[48]?d?d.p(l,M):(d=ul(l),d.c(),d.m(t,a)):d&&(d.d(1),d=null),l[6]===l[48]&&l[46]!==null?v?(v.p(l,M),M[0]&65&&C(v,1)):(v=_l(l),v.c(),C(v,1),v.m(t,null)):v&&(se(),q(v,1,1,()=>{v=null}),oe()),(!f||M[0]&33)&&F(t,"no-cat",l[46]===null||l[5]&&l[5]!==l[46]),(!f||M[0]&1)&&F(t,"hl",l[46]!==null),(!f||M[0]&4)&&F(t,"selectable",l[2]),M[0]&41&&we(t,"background-color",l[46]===null||l[5]&&l[5]!==l[46]?"":l[46]&&l[3][l[46]]?l[3][l[46]].secondary:""),l[46]!==null?$?$.p(l,M):($=dl(l),$.c(),$.m(e,null)):$&&($.d(1),$=null)},i(L){f||(C(v),f=!0)},o(L){q(v),f=!1},d(L){L&&N(e),d&&d.d(),v&&v.d(),$&&$.d(),c=!1,ae(g)}}}function ul(l){let e,t=l[46]+"",n,s,i;function o(){return l[23](l[48])}function a(){return l[24](l[48])}return{c(){e=H("span"),n=he(t),k(e,"id",`label-tag-${l[48]}`),k(e,"class","label svelte-1ozsnjl"),k(e,"role","button"),k(e,"tabindex","0"),we(e,"background-color",l[46]===null||l[5]&&l[5]!==l[46]?"":l[3][l[46]].primary)},m(r,f){O(r,e,f),D(e,n),s||(i=[z(e,"click",o),z(e,"keydown",a)],s=!0)},p(r,f){l=r,f[0]&1&&t!==(t=l[46]+"")&&Ee(n,t),f[0]&41&&we(e,"background-color",l[46]===null||l[5]&&l[5]!==l[46]?"":l[3][l[46]].primary)},d(r){r&&N(e),s=!1,ae(i)}}}function _l(l){let e,t,n,s;function i(a){l[25](a)}let o={labelToEdit:l[6],category:l[46],active:l[5],_color_map:l[3],indexOfLabel:l[48],text:l[45],handleValueChange:l[9]};return l[0]!==void 0&&(o.value=l[0]),t=new Tl({props:o}),Ll.push(()=>Ol(t,"value",i)),{c(){e=he(` 
									`),Ml(t.$$.fragment)},m(a,r){O(a,e,r),Hl(t,a,r),s=!0},p(a,r){const f={};r[0]&64&&(f.labelToEdit=a[6]),r[0]&1&&(f.category=a[46]),r[0]&32&&(f.active=a[5]),r[0]&8&&(f._color_map=a[3]),r[0]&1&&(f.text=a[45]),!n&&r[0]&1&&(n=!0,f.value=a[0],Nl(()=>n=!1)),t.$set(f)},i(a){s||(C(t.$$.fragment,a),s=!0)},o(a){q(t.$$.fragment,a),s=!1},d(a){a&&N(e),Vl(t,a)}}}function dl(l){let e,t,n;function s(){return l[30](l[48])}function i(...o){return l[31](l[48],...o)}return{c(){e=H("span"),e.textContent="×",k(e,"class","label-clear-button svelte-1ozsnjl"),k(e,"role","button"),k(e,"aria-roledescription","Remove label from text"),k(e,"tabindex","0")},m(o,a){O(o,e,a),t||(n=[z(e,"click",s),z(e,"keydown",i)],t=!0)},p(o,a){l=o},d(o){o&&N(e),t=!1,ae(n)}}}function hl(l){let e;return{c(){e=H("br")},m(t,n){O(t,e,n)},d(t){t&&N(e)}}}function gl(l){let e=l[49].trim()!=="",t,n=l[51]<ye(l[45]).length-1,s,i,o=e&&fl(l),a=n&&hl();return{c(){o&&o.c(),t=G(),a&&a.c(),s=Me()},m(r,f){o&&o.m(r,f),O(r,t,f),a&&a.m(r,f),O(r,s,f),i=!0},p(r,f){f[0]&1&&(e=r[49].trim()!==""),e?o?(o.p(r,f),f[0]&1&&C(o,1)):(o=fl(r),o.c(),C(o,1),o.m(t.parentNode,t)):o&&(se(),q(o,1,1,()=>{o=null}),oe()),f[0]&1&&(n=r[51]<ye(r[45]).length-1),n?a||(a=hl(),a.c(),a.m(s.parentNode,s)):a&&(a.d(1),a=null)},i(r){i||(C(o),i=!0)},o(r){q(o),i=!1},d(r){r&&(N(t),N(s)),o&&o.d(r),a&&a.d(r)}}}function ml(l){let e,t,n=ee(ye(l[45])),s=[];for(let o=0;o<n.length;o+=1)s[o]=gl(ll(l,n,o));const i=o=>q(s[o],1,1,()=>{s[o]=null});return{c(){for(let o=0;o<s.length;o+=1)s[o].c();e=Me()},m(o,a){for(let r=0;r<s.length;r+=1)s[r]&&s[r].m(o,a);O(o,e,a),t=!0},p(o,a){if(a[0]&13183){n=ee(ye(o[45]));let r;for(r=0;r<n.length;r+=1){const f=ll(o,n,r);s[r]?(s[r].p(f,a),C(s[r],1)):(s[r]=gl(f),s[r].c(),C(s[r],1),s[r].m(e.parentNode,e))}for(se(),r=n.length;r<s.length;r+=1)i(r);oe()}},i(o){if(!t){for(let a=0;a<n.length;a+=1)C(s[a]);t=!0}},o(o){s=s.filter(Boolean);for(let a=0;a<s.length;a+=1)q(s[a]);t=!1},d(o){o&&N(e),ze(s,o)}}}function Pt(l){let e,t,n,s;const i=[Kt,Zt],o=[];function a(r,f){return r[7]==="categories"?0:1}return t=a(l),n=o[t]=i[t](l),{c(){e=H("div"),n.c(),k(e,"class","container svelte-1ozsnjl")},m(r,f){O(r,e,f),o[t].m(e,null),s=!0},p(r,f){let c=t;t=a(r),t===c?o[t].p(r,f):(se(),q(o[c],1,1,()=>{o[c]=null}),oe(),n=o[t],n?n.p(r,f):(n=o[t]=i[t](r),n.c()),C(n,1),n.m(e,null))},i(r){s||(C(n),s=!0)},o(r){q(n),s=!1},d(r){r&&N(e),o[t].d()}}}function ye(l){return l.split(`
`)}function Ut(l,e,t){const n=typeof document<"u";let{value:s=[]}=e,{show_legend:i=!1}=e,{color_map:o={}}=e,{selectable:a=!1}=e,r=-1,f,c={},g="",p,m=-1;Ft(()=>{const _=()=>{p=window.getSelection(),L(),window.removeEventListener("mouseup",_)};window.addEventListener("mousedown",()=>{window.addEventListener("mouseup",_)})});async function u(_,T){if(p?.toString()&&r!==-1&&s[r].token.toString().includes(p.toString())){const W=Symbol(),ce=s[r].token,[at,ct,ft]=[ce.substring(0,_),ce.substring(_,T),ce.substring(T)];let ge=[...s.slice(0,r),{token:at,class_or_confidence:null},{token:ct,class_or_confidence:y==="scores"?1:"label",flag:W},{token:ft,class_or_confidence:null},...s.slice(r+1)];t(6,m=ge.findIndex(({flag:me})=>me===W)),ge=ge.filter(me=>me.token.trim()!==""),t(0,s=ge.map(({flag:me,...ut})=>ut)),S(),document.getElementById(`label-input-${m}`)?.focus()}}const d=At();function v(_){!s||_<0||_>=s.length||(t(0,s[_].class_or_confidence=null,s),t(0,s=Sl(s)),S(),window.getSelection()?.empty())}function S(){d("change",s),t(6,m=-1),i&&(t(14,o={}),t(3,c={}))}let y;function h(_){t(5,g=_)}function b(){t(5,g="")}async function $(_){p=window.getSelection(),_.key==="Enter"&&L()}function L(){if(p&&p?.toString().trim()!==""){const _=p.getRangeAt(0).startOffset,T=p.getRangeAt(0).endOffset;u(_,T)}}function M(_,T,W){d("select",{index:_,value:[T,W]})}const w=_=>h(_),Rl=_=>h(_),Dl=()=>b(),Al=()=>b(),Fl=_=>$(_),Zl=_=>t(4,r=_),Kl=_=>t(4,r=_),Pl=_=>t(6,m=_),Ul=_=>t(6,m=_),Yl=_=>t(6,m=_);function Gl(_){s=_,t(0,s)}const Jl=(_,T,W)=>{_!==null&&M(T,W,_)},Ql=(_,T,W,ce)=>{_!==null?(t(6,m=T),M(T,W,_)):$(ce)},Wl=_=>t(4,r=_),Xl=_=>t(4,r=_),xl=_=>v(_),et=(_,T)=>{T.key==="Enter"&&v(_)};function lt(_){s=_,t(0,s)}const tt=_=>t(4,r=_),nt=_=>t(4,r=_),ot=_=>t(6,m=_),st=(_,T)=>{T.key==="Enter"&&t(6,m=_)},it=_=>v(_),rt=(_,T)=>{T.key==="Enter"&&v(_)};return l.$$set=_=>{"value"in _&&t(0,s=_.value),"show_legend"in _&&t(1,i=_.show_legend),"color_map"in _&&t(14,o=_.color_map),"selectable"in _&&t(2,a=_.selectable)},l.$$.update=()=>{if(l.$$.dirty[0]&16393){if(o||t(14,o={}),s.length>0){for(let _ of s)if(_.class_or_confidence!==null)if(typeof _.class_or_confidence=="string"){if(t(7,y="categories"),!(_.class_or_confidence in o)){let T=kl(Object.keys(o).length);t(14,o[_.class_or_confidence]=T,o)}}else t(7,y="scores")}jl(o,c,n,f)}},[s,i,a,c,r,g,m,y,v,S,h,b,$,M,o,w,Rl,Dl,Al,Fl,Zl,Kl,Pl,Ul,Yl,Gl,Jl,Ql,Wl,Xl,xl,et,lt,tt,nt,ot,st,it,rt]}class Yt extends qt{constructor(e){super(),Rt(this,e,Ut,Pt,Dt,{value:0,show_legend:1,color_map:14,selectable:2},null,[-1,-1])}get value(){return this.$$.ctx[0]}set value(e){this.$$set({value:e}),pe()}get show_legend(){return this.$$.ctx[1]}set show_legend(e){this.$$set({show_legend:e}),pe()}get color_map(){return this.$$.ctx[14]}set color_map(e){this.$$set({color_map:e}),pe()}get selectable(){return this.$$.ctx[2]}set selectable(e){this.$$set({selectable:e}),pe()}}const Gt=Yt,{SvelteComponent:Jt,add_flush_callback:Qt,assign:Bl,bind:Wt,binding_callbacks:Xt,check_outros:_e,create_component:K,destroy_component:P,detach:te,empty:Ve,flush:R,get_spread_object:Il,get_spread_update:ql,group_outros:de,init:xt,insert:ne,mount_component:U,safe_not_equal:en,space:$e,transition_in:j,transition_out:E}=window.__gradio__svelte__internal;function ln(l){let e,t;return e=new wl({props:{variant:l[12]?"dashed":"solid",test_id:"highlighted-text",visible:l[5],elemId:l[3],elemClasses:l[4],padding:!1,container:l[8],scale:l[9],minWidth:l[10],$$slots:{default:[rn]},$$scope:{ctx:l}}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&4096&&(i.variant=n[12]?"dashed":"solid"),s&32&&(i.visible=n[5]),s&8&&(i.elemId=n[3]),s&16&&(i.elemClasses=n[4]),s&256&&(i.container=n[8]),s&512&&(i.scale=n[9]),s&1024&&(i.minWidth=n[10]),s&2107847&&(i.$$scope={dirty:s,ctx:n}),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function tn(l){let e,t;return e=new wl({props:{variant:"solid",test_id:"highlighted-text",visible:l[5],elemId:l[3],elemClasses:l[4],padding:!1,container:l[8],scale:l[9],minWidth:l[10],$$slots:{default:[un]},$$scope:{ctx:l}}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&32&&(i.visible=n[5]),s&8&&(i.elemId=n[3]),s&16&&(i.elemClasses=n[4]),s&256&&(i.container=n[8]),s&512&&(i.scale=n[9]),s&1024&&(i.minWidth=n[10]),s&2107847&&(i.$$scope={dirty:s,ctx:n}),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function bl(l){let e,t;return e=new yl({props:{Icon:je,label:l[7],float:!1,disable:l[8]===!1}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&128&&(i.label=n[7]),s&256&&(i.disable=n[8]===!1),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function nn(l){let e,t;return e=new $l({props:{$$slots:{default:[sn]},$$scope:{ctx:l}}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&2097152&&(i.$$scope={dirty:s,ctx:n}),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function on(l){let e,t,n;function s(o){l[19](o)}let i={selectable:l[11],show_legend:l[6],color_map:l[1]};return l[0]!==void 0&&(i.value=l[0]),e=new Gt({props:i}),Xt.push(()=>Wt(e,"value",s)),e.$on("change",l[20]),{c(){K(e.$$.fragment)},m(o,a){U(e,o,a),n=!0},p(o,a){const r={};a&2048&&(r.selectable=o[11]),a&64&&(r.show_legend=o[6]),a&2&&(r.color_map=o[1]),!t&&a&1&&(t=!0,r.value=o[0],Qt(()=>t=!1)),e.$set(r)},i(o){n||(j(e.$$.fragment,o),n=!0)},o(o){E(e.$$.fragment,o),n=!1},d(o){P(e,o)}}}function sn(l){let e,t;return e=new je({}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function rn(l){let e,t,n,s,i,o,a;const r=[{autoscroll:l[2].autoscroll},l[13],{i18n:l[2].i18n}];let f={};for(let u=0;u<r.length;u+=1)f=Bl(f,r[u]);e=new vl({props:f}),e.$on("clear_status",l[18]);let c=l[7]&&bl(l);const g=[on,nn],p=[];function m(u,d){return u[0]?0:1}return s=m(l),i=p[s]=g[s](l),{c(){K(e.$$.fragment),t=$e(),c&&c.c(),n=$e(),i.c(),o=Ve()},m(u,d){U(e,u,d),ne(u,t,d),c&&c.m(u,d),ne(u,n,d),p[s].m(u,d),ne(u,o,d),a=!0},p(u,d){const v=d&8196?ql(r,[d&4&&{autoscroll:u[2].autoscroll},d&8192&&Il(u[13]),d&4&&{i18n:u[2].i18n}]):{};e.$set(v),u[7]?c?(c.p(u,d),d&128&&j(c,1)):(c=bl(u),c.c(),j(c,1),c.m(n.parentNode,n)):c&&(de(),E(c,1,1,()=>{c=null}),_e());let S=s;s=m(u),s===S?p[s].p(u,d):(de(),E(p[S],1,1,()=>{p[S]=null}),_e(),i=p[s],i?i.p(u,d):(i=p[s]=g[s](u),i.c()),j(i,1),i.m(o.parentNode,o))},i(u){a||(j(e.$$.fragment,u),j(c),j(i),a=!0)},o(u){E(e.$$.fragment,u),E(c),E(i),a=!1},d(u){u&&(te(t),te(n),te(o)),P(e,u),c&&c.d(u),p[s].d(u)}}}function pl(l){let e,t;return e=new yl({props:{Icon:je,label:l[7],float:!1,disable:l[8]===!1}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&128&&(i.label=n[7]),s&256&&(i.disable=n[8]===!1),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function an(l){let e,t;return e=new $l({props:{$$slots:{default:[fn]},$$scope:{ctx:l}}}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&2097152&&(i.$$scope={dirty:s,ctx:n}),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function cn(l){let e,t;return e=new Ct({props:{selectable:l[11],value:l[0],show_legend:l[6],color_map:l[1]}}),e.$on("select",l[17]),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},p(n,s){const i={};s&2048&&(i.selectable=n[11]),s&1&&(i.value=n[0]),s&64&&(i.show_legend=n[6]),s&2&&(i.color_map=n[1]),e.$set(i)},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function fn(l){let e,t;return e=new je({}),{c(){K(e.$$.fragment)},m(n,s){U(e,n,s),t=!0},i(n){t||(j(e.$$.fragment,n),t=!0)},o(n){E(e.$$.fragment,n),t=!1},d(n){P(e,n)}}}function un(l){let e,t,n,s,i,o,a;const r=[{autoscroll:l[2].autoscroll},{i18n:l[2].i18n},l[13]];let f={};for(let u=0;u<r.length;u+=1)f=Bl(f,r[u]);e=new vl({props:f}),e.$on("clear_status",l[16]);let c=l[7]&&pl(l);const g=[cn,an],p=[];function m(u,d){return u[0]?0:1}return s=m(l),i=p[s]=g[s](l),{c(){K(e.$$.fragment),t=$e(),c&&c.c(),n=$e(),i.c(),o=Ve()},m(u,d){U(e,u,d),ne(u,t,d),c&&c.m(u,d),ne(u,n,d),p[s].m(u,d),ne(u,o,d),a=!0},p(u,d){const v=d&8196?ql(r,[d&4&&{autoscroll:u[2].autoscroll},d&4&&{i18n:u[2].i18n},d&8192&&Il(u[13])]):{};e.$set(v),u[7]?c?(c.p(u,d),d&128&&j(c,1)):(c=pl(u),c.c(),j(c,1),c.m(n.parentNode,n)):c&&(de(),E(c,1,1,()=>{c=null}),_e());let S=s;s=m(u),s===S?p[s].p(u,d):(de(),E(p[S],1,1,()=>{p[S]=null}),_e(),i=p[s],i?i.p(u,d):(i=p[s]=g[s](u),i.c()),j(i,1),i.m(o.parentNode,o))},i(u){a||(j(e.$$.fragment,u),j(c),j(i),a=!0)},o(u){E(e.$$.fragment,u),E(c),E(i),a=!1},d(u){u&&(te(t),te(n),te(o)),P(e,u),c&&c.d(u),p[s].d(u)}}}function _n(l){let e,t,n,s;const i=[tn,ln],o=[];function a(r,f){return r[12]?1:0}return e=a(l),t=o[e]=i[e](l),{c(){t.c(),n=Ve()},m(r,f){o[e].m(r,f),ne(r,n,f),s=!0},p(r,[f]){let c=e;e=a(r),e===c?o[e].p(r,f):(de(),E(o[c],1,1,()=>{o[c]=null}),_e(),t=o[e],t?t.p(r,f):(t=o[e]=i[e](r),t.c()),j(t,1),t.m(n.parentNode,n))},i(r){s||(j(t),s=!0)},o(r){E(t),s=!1},d(r){r&&te(n),o[e].d(r)}}}function dn(l,e,t){let{gradio:n}=e,{elemId:s=""}=e,{elemClasses:i=[]}=e,{visible:o=!0}=e,{value:a}=e,r,{show_legend:f}=e,{color_map:c={}}=e,{label:g=n.i18n("highlighted_text.highlighted_text")}=e,{container:p=!0}=e,{scale:m=null}=e,{minWidth:u=void 0}=e,{_selectable:d=!1}=e,{combine_adjacent:v=!1}=e,{interactive:S}=e,{loading_status:y}=e;const h=()=>n.dispatch("clear_status",y),b=({detail:w})=>n.dispatch("select",w),$=()=>n.dispatch("clear_status",y);function L(w){a=w,t(0,a),t(14,v)}const M=()=>n.dispatch("change");return l.$$set=w=>{"gradio"in w&&t(2,n=w.gradio),"elemId"in w&&t(3,s=w.elemId),"elemClasses"in w&&t(4,i=w.elemClasses),"visible"in w&&t(5,o=w.visible),"value"in w&&t(0,a=w.value),"show_legend"in w&&t(6,f=w.show_legend),"color_map"in w&&t(1,c=w.color_map),"label"in w&&t(7,g=w.label),"container"in w&&t(8,p=w.container),"scale"in w&&t(9,m=w.scale),"minWidth"in w&&t(10,u=w.minWidth),"_selectable"in w&&t(11,d=w._selectable),"combine_adjacent"in w&&t(14,v=w.combine_adjacent),"interactive"in w&&t(12,S=w.interactive),"loading_status"in w&&t(13,y=w.loading_status)},l.$$.update=()=>{l.$$.dirty&2&&!c&&Object.keys(c).length&&t(1,c),l.$$.dirty&16385&&a&&v&&t(0,a=Sl(a)),l.$$.dirty&32773&&a!==r&&(t(15,r=a),n.dispatch("change"))},[a,c,n,s,i,o,f,g,p,m,u,d,S,y,v,r,h,b,$,L,M]}class yn extends Jt{constructor(e){super(),xt(this,e,dn,_n,en,{gradio:2,elemId:3,elemClasses:4,visible:5,value:0,show_legend:6,color_map:1,label:7,container:8,scale:9,minWidth:10,_selectable:11,combine_adjacent:14,interactive:12,loading_status:13})}get gradio(){return this.$$.ctx[2]}set gradio(e){this.$$set({gradio:e}),R()}get elemId(){return this.$$.ctx[3]}set elemId(e){this.$$set({elemId:e}),R()}get elemClasses(){return this.$$.ctx[4]}set elemClasses(e){this.$$set({elemClasses:e}),R()}get visible(){return this.$$.ctx[5]}set visible(e){this.$$set({visible:e}),R()}get value(){return this.$$.ctx[0]}set value(e){this.$$set({value:e}),R()}get show_legend(){return this.$$.ctx[6]}set show_legend(e){this.$$set({show_legend:e}),R()}get color_map(){return this.$$.ctx[1]}set color_map(e){this.$$set({color_map:e}),R()}get label(){return this.$$.ctx[7]}set label(e){this.$$set({label:e}),R()}get container(){return this.$$.ctx[8]}set container(e){this.$$set({container:e}),R()}get scale(){return this.$$.ctx[9]}set scale(e){this.$$set({scale:e}),R()}get minWidth(){return this.$$.ctx[10]}set minWidth(e){this.$$set({minWidth:e}),R()}get _selectable(){return this.$$.ctx[11]}set _selectable(e){this.$$set({_selectable:e}),R()}get combine_adjacent(){return this.$$.ctx[14]}set combine_adjacent(e){this.$$set({combine_adjacent:e}),R()}get interactive(){return this.$$.ctx[12]}set interactive(e){this.$$set({interactive:e}),R()}get loading_status(){return this.$$.ctx[13]}set loading_status(e){this.$$set({loading_status:e}),R()}}export{Gt as BaseInteractiveHighlightedText,Ct as BaseStaticHighlightedText,yn as default};
//# sourceMappingURL=Index-BnIwT-sE.js.map