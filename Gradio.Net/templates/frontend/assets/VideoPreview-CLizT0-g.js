import"./IconButtonWrapper.svelte_svelte_type_style_lang-DAP8_Zsr.js";import"./MarkdownCode.svelte_svelte_type_style_lang-CaEo_I2i.js";import{B as Vt}from"./BlockLabel-CnzaitFN.js";import{I as xe}from"./IconButton-DtUbToT-.js";import{E as Dt}from"./Empty-CMV1fpYf.js";import{S as qt}from"./ShareButton-BhT5Qc-O.js";import{D as $t}from"./Download-DVtk-Jv3.js";import{V as kt}from"./Video-fsmLZWjA.js";import{I as Ct}from"./IconButtonWrapper-fdTarNL8.js";import{f as _e,u as Mt}from"./utils-BsGrhMNe.js";import{D as Rt}from"./DownloadLink-IzUam-rM.js";import{T as Lt,P as Pt}from"./Trim-JQYgj7Jd.js";import{P as Bt}from"./Play-B0Q0U1Qz.js";import{U as yt}from"./Undo-DCjBnnSO.js";import{b as It,t as At,V as Ht}from"./Video-DiLYgEjl.js";import{b as Nt}from"./index-Ya43XIXp.js";/* empty css                                             */import{M as Xt}from"./ModifyUpload-Yi6-Wo9R.js";const{SvelteComponent:Ut,append:jt,attr:F,detach:Ft,init:Ot,insert:zt,noop:We,safe_not_equal:Wt,svg_element:tt}=window.__gradio__svelte__internal;function Gt(l){let e,n;return{c(){e=tt("svg"),n=tt("path"),F(n,"d","M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3"),F(e,"xmlns","http://www.w3.org/2000/svg"),F(e,"width","100%"),F(e,"height","100%"),F(e,"viewBox","0 0 24 24"),F(e,"fill","none"),F(e,"stroke","currentColor"),F(e,"stroke-width","1.5"),F(e,"stroke-linecap","round"),F(e,"stroke-linejoin","round")},m(t,i){zt(t,e,i),jt(e,n)},p:We,i:We,o:We,d(t){t&&Ft(e)}}}class Jt extends Ut{constructor(e){super(),Ot(this,e,null,Gt,Wt,{})}}const{SvelteComponent:Kt,append:ae,attr:P,destroy_block:Qt,detach:Ie,element:se,ensure_array_like:nt,flush:ve,init:Yt,insert:Ae,listen:ue,noop:Qe,run_all:Zt,safe_not_equal:xt,set_style:x,space:Ge,src_url_equal:lt,update_keyed_each:en}=window.__gradio__svelte__internal,{onMount:it,onDestroy:tn}=window.__gradio__svelte__internal;function ot(l,e,n){const t=l.slice();return t[20]=e[n],t[22]=n,t}function nn(l){let e,n,t,i,o,r=[],s=new Map,c,g,d,u,_=nt(l[1]);const f=w=>w[22];for(let w=0;w<_.length;w+=1){let v=ot(l,_,w),k=f(v);s.set(k,r[w]=rt(k,v))}return{c(){e=se("div"),n=se("button"),t=Ge(),i=se("div"),o=Ge();for(let w=0;w<r.length;w+=1)r[w].c();c=Ge(),g=se("button"),P(n,"aria-label","start drag handle for trimming video"),P(n,"class","handle left svelte-10c4beq"),x(n,"left",l[2]+"%"),P(i,"class","opaque-layer svelte-10c4beq"),x(i,"left",l[2]+"%"),x(i,"right",100-l[3]+"%"),P(g,"aria-label","end drag handle for trimming video"),P(g,"class","handle right svelte-10c4beq"),x(g,"left",l[3]+"%"),P(e,"id","timeline"),P(e,"class","thumbnail-wrapper svelte-10c4beq")},m(w,v){Ae(w,e,v),ae(e,n),ae(e,t),ae(e,i),ae(e,o);for(let k=0;k<r.length;k+=1)r[k]&&r[k].m(e,null);ae(e,c),ae(e,g),d||(u=[ue(n,"mousedown",l[10]),ue(n,"blur",l[5]),ue(n,"keydown",l[11]),ue(g,"mousedown",l[12]),ue(g,"blur",l[5]),ue(g,"keydown",l[13])],d=!0)},p(w,v){v&4&&x(n,"left",w[2]+"%"),v&4&&x(i,"left",w[2]+"%"),v&8&&x(i,"right",100-w[3]+"%"),v&2&&(_=nt(w[1]),r=en(r,v,f,1,w,_,s,e,Qt,rt,c,ot)),v&8&&x(g,"left",w[3]+"%")},d(w){w&&Ie(e);for(let v=0;v<r.length;v+=1)r[v].d();d=!1,Zt(u)}}}function ln(l){let e;return{c(){e=se("div"),e.innerHTML='<span aria-label="loading timeline" class="loader svelte-10c4beq"></span>',P(e,"class","load-wrap svelte-10c4beq")},m(n,t){Ae(n,e,t)},p:Qe,d(n){n&&Ie(e)}}}function rt(l,e){let n,t,i;return{key:l,first:null,c(){n=se("img"),lt(n.src,t=e[20])||P(n,"src",t),P(n,"alt",i=`frame-${e[22]}`),P(n,"draggable","false"),P(n,"class","svelte-10c4beq"),this.first=n},m(o,r){Ae(o,n,r)},p(o,r){e=o,r&2&&!lt(n.src,t=e[20])&&P(n,"src",t),r&2&&i!==(i=`frame-${e[22]}`)&&P(n,"alt",i)},d(o){o&&Ie(n)}}}function on(l){let e;function n(o,r){return o[0]?ln:nn}let t=n(l),i=t(l);return{c(){e=se("div"),i.c(),P(e,"class","container svelte-10c4beq")},m(o,r){Ae(o,e,r),i.m(e,null)},p(o,[r]){t===(t=n(o))&&i?i.p(o,r):(i.d(1),i=t(o),i&&(i.c(),i.m(e,null)))},i:Qe,o:Qe,d(o){o&&Ie(e),i.d()}}}let Je=10;function rn(l,e,n){let{videoElement:t}=e,{trimmedDuration:i}=e,{dragStart:o}=e,{dragEnd:r}=e,{loadingTimeline:s}=e,c=[],g,d=0,u=100,_=null;const f=h=>{_=h},w=()=>{_=null},v=(h,y)=>{if(_){const m=document.getElementById("timeline");if(!m)return;const E=m.getBoundingClientRect();let D=(h.clientX-E.left)/E.width*100;if(y?D=_==="left"?d+y:u+y:D=(h.clientX-E.left)/E.width*100,D=Math.max(0,Math.min(D,100)),_==="left"){n(2,d=Math.min(D,u));const C=d/100*g;n(6,t.currentTime=C,t),n(8,o=C)}else if(_==="right"){n(3,u=Math.max(D,d));const C=u/100*g;n(6,t.currentTime=C,t),n(9,r=C)}const J=d/100*g,p=u/100*g;n(7,i=p-J),n(2,d),n(3,u)}},k=h=>{if(_){const y=1/g*100;h.key==="ArrowLeft"?v({clientX:0},-y):h.key==="ArrowRight"&&v({clientX:0},y)}},R=()=>{const h=document.createElement("canvas"),y=h.getContext("2d");if(!y)return;h.width=t.videoWidth,h.height=t.videoHeight,y.drawImage(t,0,0,h.width,h.height);const m=h.toDataURL("image/jpeg",.7);n(1,c=[...c,m])};it(()=>{const h=()=>{g=t.duration;const y=g/Je;let m=0;const E=()=>{R(),m++,m<Je?n(6,t.currentTime+=y,t):t.removeEventListener("seeked",E)};t.addEventListener("seeked",E),n(6,t.currentTime=0,t)};t.readyState>=1?h():t.addEventListener("loadedmetadata",h)}),tn(()=>{window.removeEventListener("mousemove",v),window.removeEventListener("mouseup",w),window.removeEventListener("keydown",k)}),it(()=>{window.addEventListener("mousemove",v),window.addEventListener("mouseup",w),window.addEventListener("keydown",k)});const B=()=>f("left"),L=h=>{(h.key==="ArrowLeft"||h.key=="ArrowRight")&&f("left")},N=()=>f("right"),I=h=>{(h.key==="ArrowLeft"||h.key=="ArrowRight")&&f("right")};return l.$$set=h=>{"videoElement"in h&&n(6,t=h.videoElement),"trimmedDuration"in h&&n(7,i=h.trimmedDuration),"dragStart"in h&&n(8,o=h.dragStart),"dragEnd"in h&&n(9,r=h.dragEnd),"loadingTimeline"in h&&n(0,s=h.loadingTimeline)},l.$$.update=()=>{l.$$.dirty&2&&n(0,s=c.length!==Je)},[s,c,d,u,f,w,t,i,o,r,B,L,N,I]}class sn extends Kt{constructor(e){super(),Yt(this,e,rn,on,xt,{videoElement:6,trimmedDuration:7,dragStart:8,dragEnd:9,loadingTimeline:0})}get videoElement(){return this.$$.ctx[6]}set videoElement(e){this.$$set({videoElement:e}),ve()}get trimmedDuration(){return this.$$.ctx[7]}set trimmedDuration(e){this.$$set({trimmedDuration:e}),ve()}get dragStart(){return this.$$.ctx[8]}set dragStart(e){this.$$set({dragStart:e}),ve()}get dragEnd(){return this.$$.ctx[9]}set dragEnd(e){this.$$set({dragEnd:e}),ve()}get loadingTimeline(){return this.$$.ctx[0]}set loadingTimeline(e){this.$$set({loadingTimeline:e}),ve()}}const{SvelteComponent:an,add_flush_callback:$e,append:fe,attr:W,bind:Ce,binding_callbacks:Me,check_outros:Ye,create_component:He,destroy_component:Ne,detach:Y,element:te,empty:un,flush:X,group_outros:Ze,init:dn,insert:Z,listen:st,mount_component:Xe,noop:_n,run_all:fn,safe_not_equal:cn,set_data:mn,space:Se,text:hn,toggle_class:ee,transition_in:H,transition_out:G}=window.__gradio__svelte__internal,{onMount:gn}=window.__gradio__svelte__internal;function at(l){let e,n,t,i,o,r,s;function c(f){l[18](f)}function g(f){l[19](f)}function d(f){l[20](f)}function u(f){l[21](f)}let _={videoElement:l[2]};return l[14]!==void 0&&(_.dragStart=l[14]),l[15]!==void 0&&(_.dragEnd=l[15]),l[12]!==void 0&&(_.trimmedDuration=l[12]),l[16]!==void 0&&(_.loadingTimeline=l[16]),n=new sn({props:_}),Me.push(()=>Ce(n,"dragStart",c)),Me.push(()=>Ce(n,"dragEnd",g)),Me.push(()=>Ce(n,"trimmedDuration",d)),Me.push(()=>Ce(n,"loadingTimeline",u)),{c(){e=te("div"),He(n.$$.fragment),W(e,"class","timeline-wrapper svelte-7yrr5f")},m(f,w){Z(f,e,w),Xe(n,e,null),s=!0},p(f,w){const v={};w&4&&(v.videoElement=f[2]),!t&&w&16384&&(t=!0,v.dragStart=f[14],$e(()=>t=!1)),!i&&w&32768&&(i=!0,v.dragEnd=f[15],$e(()=>i=!1)),!o&&w&4096&&(o=!0,v.trimmedDuration=f[12],$e(()=>o=!1)),!r&&w&65536&&(r=!0,v.loadingTimeline=f[16],$e(()=>r=!1)),n.$set(v)},i(f){s||(H(n.$$.fragment,f),s=!0)},o(f){G(n.$$.fragment,f),s=!1},d(f){f&&Y(e),Ne(n)}}}function bn(l){let e;return{c(){e=te("div"),W(e,"class","svelte-7yrr5f")},m(n,t){Z(n,e,t)},p:_n,d(n){n&&Y(e)}}}function wn(l){let e,n=_e(l[12])+"",t,i,o,r,s,c,g,d;return{c(){e=te("time"),t=hn(n),i=Se(),o=te("div"),r=te("button"),r.textContent="Trim",s=Se(),c=te("button"),c.textContent="Cancel",W(e,"aria-label","duration of selected region in seconds"),W(e,"class","svelte-7yrr5f"),ee(e,"hidden",l[16]),W(r,"class","text-button svelte-7yrr5f"),ee(r,"hidden",l[16]),W(c,"class","text-button svelte-7yrr5f"),ee(c,"hidden",l[16]),W(o,"class","edit-buttons svelte-7yrr5f")},m(u,_){Z(u,e,_),fe(e,t),Z(u,i,_),Z(u,o,_),fe(o,r),fe(o,s),fe(o,c),g||(d=[st(r,"click",l[22]),st(c,"click",l[17])],g=!0)},p(u,_){_&4096&&n!==(n=_e(u[12])+"")&&mn(t,n),_&65536&&ee(e,"hidden",u[16]),_&65536&&ee(r,"hidden",u[16]),_&65536&&ee(c,"hidden",u[16])},d(u){u&&(Y(e),Y(i),Y(o)),g=!1,fn(d)}}}function ut(l){let e,n;return e=new xe({props:{Icon:yt,label:"Reset video to initial value",disabled:l[1]||!l[11]}}),e.$on("click",l[23]),{c(){He(e.$$.fragment)},m(t,i){Xe(e,t,i),n=!0},p(t,i){const o={};i&2050&&(o.disabled=t[1]||!t[11]),e.$set(o)},i(t){n||(H(e.$$.fragment,t),n=!0)},o(t){G(e.$$.fragment,t),n=!1},d(t){Ne(e,t)}}}function dt(l){let e,n;return e=new xe({props:{Icon:Lt,label:"Trim video to selection",disabled:l[1]}}),e.$on("click",l[17]),{c(){He(e.$$.fragment)},m(t,i){Xe(e,t,i),n=!0},p(t,i){const o={};i&2&&(o.disabled=t[1]),e.$set(o)},i(t){n||(H(e.$$.fragment,t),n=!0)},o(t){G(e.$$.fragment,t),n=!1},d(t){Ne(e,t)}}}function vn(l){let e,n,t,i=l[3]&&l[0]===""&&ut(l),o=l[4]&&l[0]===""&&dt(l);return{c(){i&&i.c(),e=Se(),o&&o.c(),n=un()},m(r,s){i&&i.m(r,s),Z(r,e,s),o&&o.m(r,s),Z(r,n,s),t=!0},p(r,s){r[3]&&r[0]===""?i?(i.p(r,s),s&9&&H(i,1)):(i=ut(r),i.c(),H(i,1),i.m(e.parentNode,e)):i&&(Ze(),G(i,1,1,()=>{i=null}),Ye()),r[4]&&r[0]===""?o?(o.p(r,s),s&17&&H(o,1)):(o=dt(r),o.c(),H(o,1),o.m(n.parentNode,n)):o&&(Ze(),G(o,1,1,()=>{o=null}),Ye())},i(r){t||(H(i),H(o),t=!0)},o(r){G(i),G(o),t=!1},d(r){r&&(Y(e),Y(n)),i&&i.d(r),o&&o.d(r)}}}function pn(l){let e,n,t,i,o,r,s=l[0]==="edit"&&at(l);function c(u,_){return u[0]==="edit"&&u[12]!==null?wn:bn}let g=c(l),d=g(l);return o=new Xt({props:{i18n:l[7],download:l[9]?l[8]?.url:null,$$slots:{default:[vn]},$$scope:{ctx:l}}}),o.$on("clear",l[24]),{c(){e=te("div"),s&&s.c(),n=Se(),t=te("div"),d.c(),i=Se(),He(o.$$.fragment),W(t,"class","controls svelte-7yrr5f"),W(t,"data-testid","waveform-controls"),W(e,"class","container svelte-7yrr5f"),ee(e,"hidden",l[0]!=="edit")},m(u,_){Z(u,e,_),s&&s.m(e,null),fe(e,n),fe(e,t),d.m(t,null),Z(u,i,_),Xe(o,u,_),r=!0},p(u,[_]){u[0]==="edit"?s?(s.p(u,_),_&1&&H(s,1)):(s=at(u),s.c(),H(s,1),s.m(e,n)):s&&(Ze(),G(s,1,1,()=>{s=null}),Ye()),g===(g=c(u))&&d?d.p(u,_):(d.d(1),d=g(u),d&&(d.c(),d.m(t,null))),(!r||_&1)&&ee(e,"hidden",u[0]!=="edit");const f={};_&128&&(f.i18n=u[7]),_&768&&(f.download=u[9]?u[8]?.url:null),_&33556539&&(f.$$scope={dirty:_,ctx:u}),o.$set(f)},i(u){r||(H(s),H(o.$$.fragment,u),r=!0)},o(u){G(s),G(o.$$.fragment,u),r=!1},d(u){u&&(Y(e),Y(i)),s&&s.d(),d.d(),Ne(o,u)}}}function kn(l,e,n){let{videoElement:t}=e,{showRedo:i=!1}=e,{interactive:o=!0}=e,{mode:r=""}=e,{handle_reset_value:s}=e,{handle_trim_video:c}=e,{processingVideo:g=!1}=e,{i18n:d}=e,{value:u=null}=e,{show_download_button:_=!1}=e,{handle_clear:f=()=>{}}=e,{has_change_history:w=!1}=e,v;gn(async()=>{n(13,v=await It())});let k=null,R=0,B=0,L=!1;const N=()=>{r==="edit"?(n(0,r=""),n(12,k=t.duration)):n(0,r="edit")};function I(p){R=p,n(14,R)}function h(p){B=p,n(15,B)}function y(p){k=p,n(12,k),n(0,r),n(2,t)}function m(p){L=p,n(16,L)}const E=()=>{n(0,r=""),n(1,g=!0),At(v,R,B,t).then(p=>{c(p)}).then(()=>{n(1,g=!1)})},D=()=>{s(),n(0,r="")},J=()=>f();return l.$$set=p=>{"videoElement"in p&&n(2,t=p.videoElement),"showRedo"in p&&n(3,i=p.showRedo),"interactive"in p&&n(4,o=p.interactive),"mode"in p&&n(0,r=p.mode),"handle_reset_value"in p&&n(5,s=p.handle_reset_value),"handle_trim_video"in p&&n(6,c=p.handle_trim_video),"processingVideo"in p&&n(1,g=p.processingVideo),"i18n"in p&&n(7,d=p.i18n),"value"in p&&n(8,u=p.value),"show_download_button"in p&&n(9,_=p.show_download_button),"handle_clear"in p&&n(10,f=p.handle_clear),"has_change_history"in p&&n(11,w=p.has_change_history)},l.$$.update=()=>{l.$$.dirty&4101&&r==="edit"&&k===null&&t&&n(12,k=t.duration)},[r,g,t,i,o,s,c,d,u,_,f,w,k,v,R,B,L,N,I,h,y,m,E,D,J]}class yn extends an{constructor(e){super(),dn(this,e,kn,pn,cn,{videoElement:2,showRedo:3,interactive:4,mode:0,handle_reset_value:5,handle_trim_video:6,processingVideo:1,i18n:7,value:8,show_download_button:9,handle_clear:10,has_change_history:11})}get videoElement(){return this.$$.ctx[2]}set videoElement(e){this.$$set({videoElement:e}),X()}get showRedo(){return this.$$.ctx[3]}set showRedo(e){this.$$set({showRedo:e}),X()}get interactive(){return this.$$.ctx[4]}set interactive(e){this.$$set({interactive:e}),X()}get mode(){return this.$$.ctx[0]}set mode(e){this.$$set({mode:e}),X()}get handle_reset_value(){return this.$$.ctx[5]}set handle_reset_value(e){this.$$set({handle_reset_value:e}),X()}get handle_trim_video(){return this.$$.ctx[6]}set handle_trim_video(e){this.$$set({handle_trim_video:e}),X()}get processingVideo(){return this.$$.ctx[1]}set processingVideo(e){this.$$set({processingVideo:e}),X()}get i18n(){return this.$$.ctx[7]}set i18n(e){this.$$set({i18n:e}),X()}get value(){return this.$$.ctx[8]}set value(e){this.$$set({value:e}),X()}get show_download_button(){return this.$$.ctx[9]}set show_download_button(e){this.$$set({show_download_button:e}),X()}get handle_clear(){return this.$$.ctx[10]}set handle_clear(e){this.$$set({handle_clear:e}),X()}get has_change_history(){return this.$$.ctx[11]}set has_change_history(e){this.$$set({has_change_history:e}),X()}}const{SvelteComponent:En,add_flush_callback:ye,append:A,attr:$,bind:Ee,binding_callbacks:Te,bubble:pe,check_outros:_t,create_component:ce,destroy_component:me,detach:Re,element:Q,empty:Tn,flush:q,group_outros:ft,init:Sn,insert:Le,listen:re,mount_component:he,prevent_default:ct,run_all:Vn,safe_not_equal:Dn,set_data:mt,space:ke,src_url_equal:ht,stop_propagation:qn,text:Ke,toggle_class:gt,transition_in:U,transition_out:z}=window.__gradio__svelte__internal,{createEventDispatcher:$n}=window.__gradio__svelte__internal;function Cn(l){let e,n;return{c(){e=Q("track"),$(e,"kind","captions"),ht(e.src,n=l[1])||$(e,"src",n),e.default=!0},m(t,i){Le(t,e,i)},p(t,i){i[0]&2&&!ht(e.src,n=t[1])&&$(e,"src",n)},d(t){t&&Re(e)}}}function Mn(l){let e,n;return e=new Pt({}),{c(){ce(e.$$.fragment)},m(t,i){he(e,t,i),n=!0},i(t){n||(U(e.$$.fragment,t),n=!0)},o(t){z(e.$$.fragment,t),n=!1},d(t){me(e,t)}}}function Rn(l){let e,n;return e=new Bt({}),{c(){ce(e.$$.fragment)},m(t,i){he(e,t,i),n=!0},i(t){n||(U(e.$$.fragment,t),n=!0)},o(t){z(e.$$.fragment,t),n=!1},d(t){me(e,t)}}}function Ln(l){let e,n;return e=new yt({}),{c(){ce(e.$$.fragment)},m(t,i){he(e,t,i),n=!0},i(t){n||(U(e.$$.fragment,t),n=!0)},o(t){z(e.$$.fragment,t),n=!1},d(t){me(e,t)}}}function bt(l){let e,n,t;function i(r){l[37](r)}let o={videoElement:l[17],showRedo:!0,handle_trim_video:l[23],handle_reset_value:l[7],value:l[11],i18n:l[9],show_download_button:l[10],handle_clear:l[12],has_change_history:l[13]};return l[18]!==void 0&&(o.processingVideo=l[18]),e=new yn({props:o}),Te.push(()=>Ee(e,"processingVideo",i)),{c(){ce(e.$$.fragment)},m(r,s){he(e,r,s),t=!0},p(r,s){const c={};s[0]&131072&&(c.videoElement=r[17]),s[0]&128&&(c.handle_reset_value=r[7]),s[0]&2048&&(c.value=r[11]),s[0]&512&&(c.i18n=r[9]),s[0]&1024&&(c.show_download_button=r[10]),s[0]&4096&&(c.handle_clear=r[12]),s[0]&8192&&(c.has_change_history=r[13]),!n&&s[0]&262144&&(n=!0,c.processingVideo=r[18],ye(()=>n=!1)),e.$set(c)},i(r){t||(U(e.$$.fragment,r),t=!0)},o(r){z(e.$$.fragment,r),t=!1},d(r){me(e,r)}}}function Pn(l){let e,n,t,i,o,r,s,c,g,d,u,_,f,w,v,k=_e(l[14])+"",R,B,L=_e(l[15])+"",N,I,h,y,m,E,D,J,p,C,we,Ve;function Ue(b){l[28](b)}function je(b){l[29](b)}function Fe(b){l[30](b)}function Oe(b){l[31](b)}let oe={src:l[0],preload:"auto",autoplay:l[3],loop:l[4],is_stream:l[8],"data-testid":`${l[5]}-player`,processingVideo:l[18],$$slots:{default:[Cn]},$$scope:{ctx:l}};l[14]!==void 0&&(oe.currentTime=l[14]),l[15]!==void 0&&(oe.duration=l[15]),l[16]!==void 0&&(oe.paused=l[16]),l[17]!==void 0&&(oe.node=l[17]),t=new Ht({props:oe}),Te.push(()=>Ee(t,"currentTime",Ue)),Te.push(()=>Ee(t,"duration",je)),Te.push(()=>Ee(t,"paused",Fe)),Te.push(()=>Ee(t,"node",Oe)),t.$on("click",l[20]),t.$on("play",l[32]),t.$on("pause",l[33]),t.$on("ended",l[22]),t.$on("loadstart",l[34]),t.$on("loadeddata",l[35]),t.$on("loadedmetadata",l[36]);const De=[Ln,Rn,Mn],K=[];function qe(b,a){return b[14]===b[15]?0:b[16]?1:2}_=qe(l),f=K[_]=De[_](l),D=new Jt({});let T=l[6]&&bt(l);return{c(){e=Q("div"),n=Q("div"),ce(t.$$.fragment),c=ke(),g=Q("div"),d=Q("div"),u=Q("span"),f.c(),w=ke(),v=Q("span"),R=Ke(k),B=Ke(" / "),N=Ke(L),I=ke(),h=Q("progress"),m=ke(),E=Q("div"),ce(D.$$.fragment),J=ke(),T&&T.c(),p=Tn(),$(n,"class","mirror-wrap svelte-euo1cw"),gt(n,"mirror",l[2]),$(u,"role","button"),$(u,"tabindex","0"),$(u,"class","icon svelte-euo1cw"),$(u,"aria-label","play-pause-replay-button"),$(v,"class","time svelte-euo1cw"),h.value=y=l[14]/l[15]||0,$(h,"class","svelte-euo1cw"),$(E,"role","button"),$(E,"tabindex","0"),$(E,"class","icon svelte-euo1cw"),$(E,"aria-label","full-screen"),$(d,"class","inner svelte-euo1cw"),$(g,"class","controls svelte-euo1cw"),$(e,"class","wrap svelte-euo1cw")},m(b,a){Le(b,e,a),A(e,n),he(t,n,null),A(e,c),A(e,g),A(g,d),A(d,u),K[_].m(u,null),A(d,w),A(d,v),A(v,R),A(v,B),A(v,N),A(d,I),A(d,h),A(d,m),A(d,E),he(D,E,null),Le(b,J,a),T&&T.m(b,a),Le(b,p,a),C=!0,we||(Ve=[re(u,"click",l[20]),re(u,"keydown",l[20]),re(h,"mousemove",l[19]),re(h,"touchmove",ct(l[19])),re(h,"click",qn(ct(l[21]))),re(E,"click",l[24]),re(E,"keypress",l[24])],we=!0)},p(b,a){const S={};a[0]&1&&(S.src=b[0]),a[0]&8&&(S.autoplay=b[3]),a[0]&16&&(S.loop=b[4]),a[0]&256&&(S.is_stream=b[8]),a[0]&32&&(S["data-testid"]=`${b[5]}-player`),a[0]&262144&&(S.processingVideo=b[18]),a[0]&2|a[1]&256&&(S.$$scope={dirty:a,ctx:b}),!i&&a[0]&16384&&(i=!0,S.currentTime=b[14],ye(()=>i=!1)),!o&&a[0]&32768&&(o=!0,S.duration=b[15],ye(()=>o=!1)),!r&&a[0]&65536&&(r=!0,S.paused=b[16],ye(()=>r=!1)),!s&&a[0]&131072&&(s=!0,S.node=b[17],ye(()=>s=!1)),t.$set(S),(!C||a[0]&4)&&gt(n,"mirror",b[2]);let j=_;_=qe(b),_!==j&&(ft(),z(K[j],1,1,()=>{K[j]=null}),_t(),f=K[_],f||(f=K[_]=De[_](b),f.c()),U(f,1),f.m(u,null)),(!C||a[0]&16384)&&k!==(k=_e(b[14])+"")&&mt(R,k),(!C||a[0]&32768)&&L!==(L=_e(b[15])+"")&&mt(N,L),(!C||a[0]&49152&&y!==(y=b[14]/b[15]||0))&&(h.value=y),b[6]?T?(T.p(b,a),a[0]&64&&U(T,1)):(T=bt(b),T.c(),U(T,1),T.m(p.parentNode,p)):T&&(ft(),z(T,1,1,()=>{T=null}),_t())},i(b){C||(U(t.$$.fragment,b),U(f),U(D.$$.fragment,b),U(T),C=!0)},o(b){z(t.$$.fragment,b),z(f),z(D.$$.fragment,b),z(T),C=!1},d(b){b&&(Re(e),Re(J),Re(p)),me(t),K[_].d(),me(D),T&&T.d(b),we=!1,Vn(Ve)}}}function Bn(l,e,n){let{root:t=""}=e,{src:i}=e,{subtitle:o=null}=e,{mirror:r}=e,{autoplay:s}=e,{loop:c}=e,{label:g="test"}=e,{interactive:d=!1}=e,{handle_change:u=()=>{}}=e,{handle_reset_value:_=()=>{}}=e,{upload:f}=e,{is_stream:w}=e,{i18n:v}=e,{show_download_button:k=!1}=e,{value:R=null}=e,{handle_clear:B=()=>{}}=e,{has_change_history:L=!1}=e;const N=$n();let I=0,h,y=!0,m,E=!1;function D(a){if(!h)return;if(a.type==="click"){p(a);return}if(a.type!=="touchmove"&&!(a.buttons&1))return;const S=a.type==="touchmove"?a.touches[0].clientX:a.clientX,{left:j,right:ze}=a.currentTarget.getBoundingClientRect();n(14,I=h*(S-j)/(ze-j))}async function J(){document.fullscreenElement!=m&&(m.currentTime>0&&!m.paused&&!m.ended&&m.readyState>m.HAVE_CURRENT_DATA?m.pause():await m.play())}function p(a){const{left:S,right:j}=a.currentTarget.getBoundingClientRect();n(14,I=h*(a.clientX-S)/(j-S))}function C(){N("stop"),N("end")}const we=async a=>{let S=new File([a],"video.mp4");const j=await Nt([S]);let ze=(await f(j,t))?.filter(Boolean)[0];u(ze)};function Ve(){m.requestFullscreen()}function Ue(a){I=a,n(14,I)}function je(a){h=a,n(15,h)}function Fe(a){y=a,n(16,y)}function Oe(a){m=a,n(17,m)}function oe(a){pe.call(this,l,a)}function De(a){pe.call(this,l,a)}function K(a){pe.call(this,l,a)}function qe(a){pe.call(this,l,a)}function T(a){pe.call(this,l,a)}function b(a){E=a,n(18,E)}return l.$$set=a=>{"root"in a&&n(25,t=a.root),"src"in a&&n(0,i=a.src),"subtitle"in a&&n(1,o=a.subtitle),"mirror"in a&&n(2,r=a.mirror),"autoplay"in a&&n(3,s=a.autoplay),"loop"in a&&n(4,c=a.loop),"label"in a&&n(5,g=a.label),"interactive"in a&&n(6,d=a.interactive),"handle_change"in a&&n(26,u=a.handle_change),"handle_reset_value"in a&&n(7,_=a.handle_reset_value),"upload"in a&&n(27,f=a.upload),"is_stream"in a&&n(8,w=a.is_stream),"i18n"in a&&n(9,v=a.i18n),"show_download_button"in a&&n(10,k=a.show_download_button),"value"in a&&n(11,R=a.value),"handle_clear"in a&&n(12,B=a.handle_clear),"has_change_history"in a&&n(13,L=a.has_change_history)},[i,o,r,s,c,g,d,_,w,v,k,R,B,L,I,h,y,m,E,D,J,p,C,we,Ve,t,u,f,Ue,je,Fe,Oe,oe,De,K,qe,T,b]}class In extends En{constructor(e){super(),Sn(this,e,Bn,Pn,Dn,{root:25,src:0,subtitle:1,mirror:2,autoplay:3,loop:4,label:5,interactive:6,handle_change:26,handle_reset_value:7,upload:27,is_stream:8,i18n:9,show_download_button:10,value:11,handle_clear:12,has_change_history:13},null,[-1,-1])}get root(){return this.$$.ctx[25]}set root(e){this.$$set({root:e}),q()}get src(){return this.$$.ctx[0]}set src(e){this.$$set({src:e}),q()}get subtitle(){return this.$$.ctx[1]}set subtitle(e){this.$$set({subtitle:e}),q()}get mirror(){return this.$$.ctx[2]}set mirror(e){this.$$set({mirror:e}),q()}get autoplay(){return this.$$.ctx[3]}set autoplay(e){this.$$set({autoplay:e}),q()}get loop(){return this.$$.ctx[4]}set loop(e){this.$$set({loop:e}),q()}get label(){return this.$$.ctx[5]}set label(e){this.$$set({label:e}),q()}get interactive(){return this.$$.ctx[6]}set interactive(e){this.$$set({interactive:e}),q()}get handle_change(){return this.$$.ctx[26]}set handle_change(e){this.$$set({handle_change:e}),q()}get handle_reset_value(){return this.$$.ctx[7]}set handle_reset_value(e){this.$$set({handle_reset_value:e}),q()}get upload(){return this.$$.ctx[27]}set upload(e){this.$$set({upload:e}),q()}get is_stream(){return this.$$.ctx[8]}set is_stream(e){this.$$set({is_stream:e}),q()}get i18n(){return this.$$.ctx[9]}set i18n(e){this.$$set({i18n:e}),q()}get show_download_button(){return this.$$.ctx[10]}set show_download_button(e){this.$$set({show_download_button:e}),q()}get value(){return this.$$.ctx[11]}set value(e){this.$$set({value:e}),q()}get handle_clear(){return this.$$.ctx[12]}set handle_clear(e){this.$$set({handle_clear:e}),q()}get has_change_history(){return this.$$.ctx[13]}set has_change_history(e){this.$$set({has_change_history:e}),q()}}const An=In,{SvelteComponent:Hn,attr:Nn,bubble:de,check_outros:Pe,create_component:ne,destroy_component:le,detach:ge,element:Xn,empty:Et,flush:O,group_outros:Be,init:Un,insert:be,mount_component:ie,noop:Tt,safe_not_equal:St,space:et,transition_in:V,transition_out:M}=window.__gradio__svelte__internal,{createEventDispatcher:jn,afterUpdate:Fn,tick:On}=window.__gradio__svelte__internal;function zn(l){let e=l[0].url,n,t,i,o,r=wt(l);return i=new Ct({props:{$$slots:{default:[Jn]},$$scope:{ctx:l}}}),{c(){r.c(),n=et(),t=Xn("div"),ne(i.$$.fragment),Nn(t,"data-testid","download-div")},m(s,c){r.m(s,c),be(s,n,c),be(s,t,c),ie(i,t,null),o=!0},p(s,c){c&1&&St(e,e=s[0].url)?(Be(),M(r,1,1,Tt),Pe(),r=wt(s),r.c(),V(r,1),r.m(n.parentNode,n)):r.p(s,c);const g={};c&2097505&&(g.$$scope={dirty:c,ctx:s}),i.$set(g)},i(s){o||(V(r),V(i.$$.fragment,s),o=!0)},o(s){M(r),M(i.$$.fragment,s),o=!1},d(s){s&&(ge(n),ge(t)),r.d(s),le(i)}}}function Wn(l){let e,n;return e=new Dt({props:{unpadded_box:!0,size:"large",$$slots:{default:[Kn]},$$scope:{ctx:l}}}),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},p(t,i){const o={};i&2097152&&(o.$$scope={dirty:i,ctx:t}),e.$set(o)},i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function wt(l){let e,n;return e=new An({props:{src:l[0].url,subtitle:l[1]?.url,is_stream:l[0].is_stream,autoplay:l[4],mirror:!1,label:l[2],loop:l[7],interactive:!1,upload:l[9],i18n:l[8]}}),e.$on("play",l[11]),e.$on("pause",l[12]),e.$on("stop",l[13]),e.$on("end",l[14]),e.$on("loadedmetadata",l[15]),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},p(t,i){const o={};i&1&&(o.src=t[0].url),i&2&&(o.subtitle=t[1]?.url),i&1&&(o.is_stream=t[0].is_stream),i&16&&(o.autoplay=t[4]),i&4&&(o.label=t[2]),i&128&&(o.loop=t[7]),i&512&&(o.upload=t[9]),i&256&&(o.i18n=t[8]),e.$set(o)},i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function vt(l){let e,n;return e=new Rt({props:{href:l[0].is_stream?l[0].url?.replace("playlist.m3u8","playlist-file"):l[0].url,download:l[0].orig_name||l[0].path,$$slots:{default:[Gn]},$$scope:{ctx:l}}}),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},p(t,i){const o={};i&1&&(o.href=t[0].is_stream?t[0].url?.replace("playlist.m3u8","playlist-file"):t[0].url),i&1&&(o.download=t[0].orig_name||t[0].path),i&2097152&&(o.$$scope={dirty:i,ctx:t}),e.$set(o)},i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function Gn(l){let e,n;return e=new xe({props:{Icon:$t,label:"Download"}}),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},p:Tt,i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function pt(l){let e,n;return e=new qt({props:{i18n:l[8],value:l[0],formatter:l[16]}}),e.$on("error",l[17]),e.$on("share",l[18]),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},p(t,i){const o={};i&256&&(o.i18n=t[8]),i&1&&(o.value=t[0]),e.$set(o)},i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function Jn(l){let e,n,t,i=l[6]&&vt(l),o=l[5]&&pt(l);return{c(){i&&i.c(),e=et(),o&&o.c(),n=Et()},m(r,s){i&&i.m(r,s),be(r,e,s),o&&o.m(r,s),be(r,n,s),t=!0},p(r,s){r[6]?i?(i.p(r,s),s&64&&V(i,1)):(i=vt(r),i.c(),V(i,1),i.m(e.parentNode,e)):i&&(Be(),M(i,1,1,()=>{i=null}),Pe()),r[5]?o?(o.p(r,s),s&32&&V(o,1)):(o=pt(r),o.c(),V(o,1),o.m(n.parentNode,n)):o&&(Be(),M(o,1,1,()=>{o=null}),Pe())},i(r){t||(V(i),V(o),t=!0)},o(r){M(i),M(o),t=!1},d(r){r&&(ge(e),ge(n)),i&&i.d(r),o&&o.d(r)}}}function Kn(l){let e,n;return e=new kt({}),{c(){ne(e.$$.fragment)},m(t,i){ie(e,t,i),n=!0},i(t){n||(V(e.$$.fragment,t),n=!0)},o(t){M(e.$$.fragment,t),n=!1},d(t){le(e,t)}}}function Qn(l){let e,n,t,i,o,r;e=new Vt({props:{show_label:l[3],Icon:kt,label:l[2]||"Video"}});const s=[Wn,zn],c=[];function g(d,u){return!d[0]||d[0].url===void 0?0:1}return t=g(l),i=c[t]=s[t](l),{c(){ne(e.$$.fragment),n=et(),i.c(),o=Et()},m(d,u){ie(e,d,u),be(d,n,u),c[t].m(d,u),be(d,o,u),r=!0},p(d,[u]){const _={};u&8&&(_.show_label=d[3]),u&4&&(_.label=d[2]||"Video"),e.$set(_);let f=t;t=g(d),t===f?c[t].p(d,u):(Be(),M(c[f],1,1,()=>{c[f]=null}),Pe(),i=c[t],i?i.p(d,u):(i=c[t]=s[t](d),i.c()),V(i,1),i.m(o.parentNode,o))},i(d){r||(V(e.$$.fragment,d),V(i),r=!0)},o(d){M(e.$$.fragment,d),M(i),r=!1},d(d){d&&(ge(n),ge(o)),le(e,d),c[t].d(d)}}}function Yn(l,e,n){let{value:t=null}=e,{subtitle:i=null}=e,{label:o=void 0}=e,{show_label:r=!0}=e,{autoplay:s}=e,{show_share_button:c=!0}=e,{show_download_button:g=!0}=e,{loop:d}=e,{i18n:u}=e,{upload:_}=e,f=null,w=null;const v=jn();Fn(async()=>{t!==f&&i!==w&&w!==null&&(f=t,n(0,t=null),await On(),n(0,t=f)),f=t,w=i});function k(m){de.call(this,l,m)}function R(m){de.call(this,l,m)}function B(m){de.call(this,l,m)}function L(m){de.call(this,l,m)}const N=()=>{v("load")},I=async m=>m?await Mt(m.data):"";function h(m){de.call(this,l,m)}function y(m){de.call(this,l,m)}return l.$$set=m=>{"value"in m&&n(0,t=m.value),"subtitle"in m&&n(1,i=m.subtitle),"label"in m&&n(2,o=m.label),"show_label"in m&&n(3,r=m.show_label),"autoplay"in m&&n(4,s=m.autoplay),"show_share_button"in m&&n(5,c=m.show_share_button),"show_download_button"in m&&n(6,g=m.show_download_button),"loop"in m&&n(7,d=m.loop),"i18n"in m&&n(8,u=m.i18n),"upload"in m&&n(9,_=m.upload)},l.$$.update=()=>{l.$$.dirty&1&&t&&v("change",t)},[t,i,o,r,s,c,g,d,u,_,v,k,R,B,L,N,I,h,y]}class Zn extends Hn{constructor(e){super(),Un(this,e,Yn,Qn,St,{value:0,subtitle:1,label:2,show_label:3,autoplay:4,show_share_button:5,show_download_button:6,loop:7,i18n:8,upload:9})}get value(){return this.$$.ctx[0]}set value(e){this.$$set({value:e}),O()}get subtitle(){return this.$$.ctx[1]}set subtitle(e){this.$$set({subtitle:e}),O()}get label(){return this.$$.ctx[2]}set label(e){this.$$set({label:e}),O()}get show_label(){return this.$$.ctx[3]}set show_label(e){this.$$set({show_label:e}),O()}get autoplay(){return this.$$.ctx[4]}set autoplay(e){this.$$set({autoplay:e}),O()}get show_share_button(){return this.$$.ctx[5]}set show_share_button(e){this.$$set({show_share_button:e}),O()}get show_download_button(){return this.$$.ctx[6]}set show_download_button(e){this.$$set({show_download_button:e}),O()}get loop(){return this.$$.ctx[7]}set loop(e){this.$$set({loop:e}),O()}get i18n(){return this.$$.ctx[8]}set i18n(e){this.$$set({i18n:e}),O()}get upload(){return this.$$.ctx[9]}set upload(e){this.$$set({upload:e}),O()}}const bl=Object.freeze(Object.defineProperty({__proto__:null,default:Zn},Symbol.toStringTag,{value:"Module"}));export{An as P,Zn as V,bl as a};
//# sourceMappingURL=VideoPreview-CLizT0-g.js.map