import{b as Le}from"./index-Ya43XIXp.js";/* empty css                                             */const{SvelteComponent:Oe,append:A,attr:T,detach:ae,element:U,flush:M,init:qe,insert:ue,noop:ie,safe_not_equal:Ie,set_data:j,set_style:Y,space:Z,text:q,toggle_class:le}=window.__gradio__svelte__internal,{onMount:Ne,createEventDispatcher:Me,onDestroy:je}=window.__gradio__svelte__internal;function re(t){let e,i,n,r,d=I(t[2])+"",a,u,s,o,b=t[2].orig_name+"",_;return{c(){e=U("div"),i=U("span"),n=U("div"),r=U("progress"),a=q(d),s=Z(),o=U("span"),_=q(b),Y(r,"visibility","hidden"),Y(r,"height","0"),Y(r,"width","0"),r.value=u=I(t[2]),T(r,"max","100"),T(r,"class","svelte-1vsfomn"),T(n,"class","progress-bar svelte-1vsfomn"),T(o,"class","file-name svelte-1vsfomn"),T(e,"class","file svelte-1vsfomn")},m(h,c){ue(h,e,c),A(e,i),A(i,n),A(n,r),A(r,a),A(e,s),A(e,o),A(o,_)},p(h,c){c&4&&d!==(d=I(h[2])+"")&&j(a,d),c&4&&u!==(u=I(h[2]))&&(r.value=u),c&4&&b!==(b=h[2].orig_name+"")&&j(_,b)},d(h){h&&ae(e)}}}function Je(t){let e,i,n,r=t[0].length+"",d,a,u=t[0].length>1?"files":"file",s,o,b,_=t[2]&&re(t);return{c(){e=U("div"),i=U("span"),n=q("Uploading "),d=q(r),a=Z(),s=q(u),o=q("..."),b=Z(),_&&_.c(),T(i,"class","uploading svelte-1vsfomn"),T(e,"class","wrap svelte-1vsfomn"),le(e,"progress",t[1])},m(h,c){ue(h,e,c),A(e,i),A(i,n),A(i,d),A(i,a),A(i,s),A(i,o),A(e,b),_&&_.m(e,null)},p(h,[c]){c&1&&r!==(r=h[0].length+"")&&j(d,r),c&1&&u!==(u=h[0].length>1?"files":"file")&&j(s,u),h[2]?_?_.p(h,c):(_=re(h),_.c(),_.m(e,null)):_&&(_.d(1),_=null),c&2&&le(e,"progress",h[1])},i:ie,o:ie,d(h){h&&ae(e),_&&_.d()}}}function I(t){return t.progress*100/(t.size||0)||0}function Re(t){let e=0;return t.forEach(i=>{e+=I(i)}),document.documentElement.style.setProperty("--upload-progress-width",(e/t.length).toFixed(2)+"%"),e/t.length}function Be(t,e,i){let{upload_id:n}=e,{root:r}=e,{files:d}=e,{stream_handler:a}=e,u,s=!1,o,b,_=d.map(f=>({...f,progress:0}));const h=Me();function c(f,g){i(0,_=_.map(W=>(W.orig_name===f&&(W.progress+=g),W)))}return Ne(async()=>{if(u=await a(new URL(`${r}/gradio_api/upload_progress?upload_id=${n}`)),u==null)throw new Error("Event source is not defined");u.onmessage=async function(f){const g=JSON.parse(f.data);s||i(1,s=!0),g.msg==="done"?(u?.close(),h("done")):(i(7,o=g),c(g.orig_name,g.chunk_size))}}),je(()=>{(u!=null||u!=null)&&u.close()}),t.$$set=f=>{"upload_id"in f&&i(3,n=f.upload_id),"root"in f&&i(4,r=f.root),"files"in f&&i(5,d=f.files),"stream_handler"in f&&i(6,a=f.stream_handler)},t.$$.update=()=>{t.$$.dirty&1&&Re(_),t.$$.dirty&129&&i(2,b=o||_[0])},[_,s,b,n,r,d,a,o]}class Ge extends Oe{constructor(e){super(),qe(this,e,Be,Je,Ie,{upload_id:3,root:4,files:5,stream_handler:6})}get upload_id(){return this.$$.ctx[3]}set upload_id(e){this.$$set({upload_id:e}),M()}get root(){return this.$$.ctx[4]}set root(e){this.$$set({root:e}),M()}get files(){return this.$$.ctx[5]}set files(e){this.$$set({files:e}),M()}get stream_handler(){return this.$$.ctx[6]}set stream_handler(e){this.$$set({stream_handler:e}),M()}}const{SvelteComponent:He,append:se,attr:v,binding_callbacks:Ke,bubble:D,check_outros:fe,create_component:Qe,create_slot:de,destroy_component:Ve,detach:R,element:x,empty:_e,flush:w,get_all_dirty_from_scope:ce,get_slot_changes:he,group_outros:ge,init:Xe,insert:B,listen:z,mount_component:Ye,prevent_default:P,run_all:Ze,safe_not_equal:xe,set_style:J,space:$e,stop_propagation:S,toggle_class:p,transition_in:F,transition_out:L,update_slot_base:me}=window.__gradio__svelte__internal,{createEventDispatcher:et,tick:tt,getContext:ft}=window.__gradio__svelte__internal;function nt(t){let e,i,n,r,d,a,u,s,o,b,_;const h=t[28].default,c=de(h,t,t[27],null);return{c(){e=x("button"),c&&c.c(),i=$e(),n=x("input"),v(n,"aria-label","file upload"),v(n,"data-testid","file-upload"),v(n,"type","file"),v(n,"accept",r=t[17]||void 0),n.multiple=d=t[6]==="multiple"||void 0,v(n,"webkitdirectory",a=t[6]==="directory"||void 0),v(n,"mozdirectory",u=t[6]==="directory"||void 0),v(n,"class","svelte-1b742ao"),v(e,"tabindex",s=t[9]?-1:0),v(e,"class","svelte-1b742ao"),p(e,"hidden",t[9]),p(e,"center",t[4]),p(e,"boundedheight",t[3]),p(e,"flex",t[5]),p(e,"disable_click",t[7]),p(e,"icon-mode",t[12]),J(e,"height",t[12]?"":"100%")},m(f,g){B(f,e,g),c&&c.m(e,null),se(e,i),se(e,n),t[36](n),o=!0,b||(_=[z(n,"change",t[19]),z(e,"drag",S(P(t[29]))),z(e,"dragstart",S(P(t[30]))),z(e,"dragend",S(P(t[31]))),z(e,"dragover",S(P(t[32]))),z(e,"dragenter",S(P(t[33]))),z(e,"dragleave",S(P(t[34]))),z(e,"drop",S(P(t[35]))),z(e,"click",t[14]),z(e,"drop",t[20]),z(e,"dragenter",t[18]),z(e,"dragleave",t[18])],b=!0)},p(f,g){c&&c.p&&(!o||g[0]&134217728)&&me(c,h,f,f[27],o?he(h,f[27],g,null):ce(f[27]),null),(!o||g[0]&131072&&r!==(r=f[17]||void 0))&&v(n,"accept",r),(!o||g[0]&64&&d!==(d=f[6]==="multiple"||void 0))&&(n.multiple=d),(!o||g[0]&64&&a!==(a=f[6]==="directory"||void 0))&&v(n,"webkitdirectory",a),(!o||g[0]&64&&u!==(u=f[6]==="directory"||void 0))&&v(n,"mozdirectory",u),(!o||g[0]&512&&s!==(s=f[9]?-1:0))&&v(e,"tabindex",s),(!o||g[0]&512)&&p(e,"hidden",f[9]),(!o||g[0]&16)&&p(e,"center",f[4]),(!o||g[0]&8)&&p(e,"boundedheight",f[3]),(!o||g[0]&32)&&p(e,"flex",f[5]),(!o||g[0]&128)&&p(e,"disable_click",f[7]),(!o||g[0]&4096)&&p(e,"icon-mode",f[12]),g[0]&4096&&J(e,"height",f[12]?"":"100%")},i(f){o||(F(c,f),o=!0)},o(f){L(c,f),o=!1},d(f){f&&R(e),c&&c.d(f),t[36](null),b=!1,Ze(_)}}}function it(t){let e,i,n=!t[9]&&oe(t);return{c(){n&&n.c(),e=_e()},m(r,d){n&&n.m(r,d),B(r,e,d),i=!0},p(r,d){r[9]?n&&(ge(),L(n,1,1,()=>{n=null}),fe()):n?(n.p(r,d),d[0]&512&&F(n,1)):(n=oe(r),n.c(),F(n,1),n.m(e.parentNode,e))},i(r){i||(F(n),i=!0)},o(r){L(n),i=!1},d(r){r&&R(e),n&&n.d(r)}}}function lt(t){let e,i,n,r,d;const a=t[28].default,u=de(a,t,t[27],null);return{c(){e=x("button"),u&&u.c(),v(e,"tabindex",i=t[9]?-1:0),v(e,"class","svelte-1b742ao"),p(e,"hidden",t[9]),p(e,"center",t[4]),p(e,"boundedheight",t[3]),p(e,"flex",t[5]),p(e,"icon-mode",t[12]),J(e,"height",t[12]?"":"100%")},m(s,o){B(s,e,o),u&&u.m(e,null),n=!0,r||(d=z(e,"click",t[13]),r=!0)},p(s,o){u&&u.p&&(!n||o[0]&134217728)&&me(u,a,s,s[27],n?he(a,s[27],o,null):ce(s[27]),null),(!n||o[0]&512&&i!==(i=s[9]?-1:0))&&v(e,"tabindex",i),(!n||o[0]&512)&&p(e,"hidden",s[9]),(!n||o[0]&16)&&p(e,"center",s[4]),(!n||o[0]&8)&&p(e,"boundedheight",s[3]),(!n||o[0]&32)&&p(e,"flex",s[5]),(!n||o[0]&4096)&&p(e,"icon-mode",s[12]),o[0]&4096&&J(e,"height",s[12]?"":"100%")},i(s){n||(F(u,s),n=!0)},o(s){L(u,s),n=!1},d(s){s&&R(e),u&&u.d(s),r=!1,d()}}}function oe(t){let e,i;return e=new Ge({props:{root:t[8],upload_id:t[15],files:t[16],stream_handler:t[11]}}),{c(){Qe(e.$$.fragment)},m(n,r){Ye(e,n,r),i=!0},p(n,r){const d={};r[0]&256&&(d.root=n[8]),r[0]&32768&&(d.upload_id=n[15]),r[0]&65536&&(d.files=n[16]),r[0]&2048&&(d.stream_handler=n[11]),e.$set(d)},i(n){i||(F(e.$$.fragment,n),i=!0)},o(n){L(e.$$.fragment,n),i=!1},d(n){Ve(e,n)}}}function rt(t){let e,i,n,r;const d=[lt,it,nt],a=[];function u(s,o){return s[0]==="clipboard"?0:s[1]&&s[10]?1:2}return e=u(t),i=a[e]=d[e](t),{c(){i.c(),n=_e()},m(s,o){a[e].m(s,o),B(s,n,o),r=!0},p(s,o){let b=e;e=u(s),e===b?a[e].p(s,o):(ge(),L(a[b],1,1,()=>{a[b]=null}),fe(),i=a[e],i?i.p(s,o):(i=a[e]=d[e](s),i.c()),F(i,1),i.m(n.parentNode,n))},i(s){r||(F(i),r=!0)},o(s){L(i),r=!1},d(s){s&&R(n),a[e].d(s)}}}function st(t,e,i){if(!t||t==="*"||t==="file/*"||Array.isArray(t)&&t.some(r=>r==="*"||r==="file/*"))return!0;let n;if(typeof t=="string")n=t.split(",").map(r=>r.trim());else if(Array.isArray(t))n=t;else return!1;return n.includes(e)||n.some(r=>{const[d]=r.split("/").map(a=>a.trim());return r.endsWith("/*")&&i.startsWith(d+"/")})}function ot(t,e,i){let n,{$$slots:r={},$$scope:d}=e,{filetype:a=null}=e,{dragging:u=!1}=e,{boundedheight:s=!0}=e,{center:o=!0}=e,{flex:b=!0}=e,{file_count:_="single"}=e,{disable_click:h=!1}=e,{root:c}=e,{hidden:f=!1}=e,{format:g="file"}=e,{uploading:W=!1}=e,{hidden_upload:C=null}=e,{show_progress:$=!0}=e,{max_file_size:G=null}=e,{upload:H}=e,{stream_handler:ee}=e,{icon_upload:te=!1}=e,K,Q,O,ne=null;const pe=()=>{if(typeof navigator<"u"){const l=navigator.userAgent.toLowerCase();return l.indexOf("iphone")>-1||l.indexOf("ipad")>-1}return!1},E=et(),be=["image","video","audio","text","file"],V=l=>n&&l.startsWith(".")?(ne=!0,l):n&&l.includes("file/*")?"*":l.startsWith(".")||l.endsWith("/*")?l:be.includes(l)?l+"/*":"."+l;function we(){i(21,u=!u)}function ye(){navigator.clipboard.read().then(async l=>{for(let m=0;m<l.length;m++){const y=l[m].types.find(k=>k.startsWith("image/"));if(y){l[m].getType(y).then(async k=>{const X=new File([k],`clipboard.${y.replace("image/","")}`);await N([X])});break}}})}function ke(){h||C&&(i(2,C.value="",C),C.click())}async function ve(l){await tt(),i(15,K=Math.random().toString(36).substring(2,15)),i(1,W=!0);try{const m=await H(l,c,K,G??1/0);return E("load",_==="single"?m?.[0]:m),i(1,W=!1),m||[]}catch(m){return E("error",m.message),i(1,W=!1),[]}}async function N(l){if(!l.length)return;let m=l.map(y=>new File([y],y instanceof File?y.name:"file",{type:y.type}));return n&&ne&&(m=m.filter(y=>Ae(y)?!0:(E("error",`Invalid file type: ${y.name}. Only ${a} allowed.`),!1)),m.length===0)?[]:(i(16,Q=await Le(m)),await ve(Q))}function Ae(l){return a?(Array.isArray(a)?a:[a]).some(y=>{const k=V(y);if(k.startsWith("."))return l.name.toLowerCase().endsWith(k.toLowerCase());if(k==="*")return!0;if(k.endsWith("/*")){const[X]=k.split("/");return l.type.startsWith(X+"/")}return l.type===k}):!0}async function ze(l){const m=l.target;if(m.files)if(g!="blob")await N(Array.from(m.files));else{if(_==="single"){E("load",m.files[0]);return}E("load",m.files)}}async function We(l){if(i(21,u=!1),!l.dataTransfer?.files)return;const m=Array.from(l.dataTransfer.files).filter(y=>{const k="."+y.name.split(".").pop();return k&&st(O,k,y.type)||(k&&Array.isArray(a)?a.includes(k):k===a)?!0:(E("error",`Invalid file type only ${a} allowed.`),!1)});if(g!="blob")await N(m);else{if(_==="single"){E("load",m[0]);return}E("load",m)}}function Ce(l){D.call(this,t,l)}function Ee(l){D.call(this,t,l)}function Fe(l){D.call(this,t,l)}function De(l){D.call(this,t,l)}function Pe(l){D.call(this,t,l)}function Se(l){D.call(this,t,l)}function Te(l){D.call(this,t,l)}function Ue(l){Ke[l?"unshift":"push"](()=>{C=l,i(2,C)})}return t.$$set=l=>{"filetype"in l&&i(0,a=l.filetype),"dragging"in l&&i(21,u=l.dragging),"boundedheight"in l&&i(3,s=l.boundedheight),"center"in l&&i(4,o=l.center),"flex"in l&&i(5,b=l.flex),"file_count"in l&&i(6,_=l.file_count),"disable_click"in l&&i(7,h=l.disable_click),"root"in l&&i(8,c=l.root),"hidden"in l&&i(9,f=l.hidden),"format"in l&&i(22,g=l.format),"uploading"in l&&i(1,W=l.uploading),"hidden_upload"in l&&i(2,C=l.hidden_upload),"show_progress"in l&&i(10,$=l.show_progress),"max_file_size"in l&&i(23,G=l.max_file_size),"upload"in l&&i(24,H=l.upload),"stream_handler"in l&&i(11,ee=l.stream_handler),"icon_upload"in l&&i(12,te=l.icon_upload),"$$scope"in l&&i(27,d=l.$$scope)},t.$$.update=()=>{t.$$.dirty[0]&67108865&&(a==null?i(17,O=null):typeof a=="string"?i(17,O=V(a)):n&&a.includes("file/*")?i(17,O="*"):(i(0,a=a.map(V)),i(17,O=a.join(", "))))},i(26,n=pe()),[a,W,C,s,o,b,_,h,c,f,$,ee,te,ye,ke,K,Q,O,we,ze,We,u,g,G,H,N,n,d,r,Ce,Ee,Fe,De,Pe,Se,Te,Ue]}class dt extends He{constructor(e){super(),Xe(this,e,ot,rt,xe,{filetype:0,dragging:21,boundedheight:3,center:4,flex:5,file_count:6,disable_click:7,root:8,hidden:9,format:22,uploading:1,hidden_upload:2,show_progress:10,max_file_size:23,upload:24,stream_handler:11,icon_upload:12,paste_clipboard:13,open_file_upload:14,load_files:25},null,[-1,-1])}get filetype(){return this.$$.ctx[0]}set filetype(e){this.$$set({filetype:e}),w()}get dragging(){return this.$$.ctx[21]}set dragging(e){this.$$set({dragging:e}),w()}get boundedheight(){return this.$$.ctx[3]}set boundedheight(e){this.$$set({boundedheight:e}),w()}get center(){return this.$$.ctx[4]}set center(e){this.$$set({center:e}),w()}get flex(){return this.$$.ctx[5]}set flex(e){this.$$set({flex:e}),w()}get file_count(){return this.$$.ctx[6]}set file_count(e){this.$$set({file_count:e}),w()}get disable_click(){return this.$$.ctx[7]}set disable_click(e){this.$$set({disable_click:e}),w()}get root(){return this.$$.ctx[8]}set root(e){this.$$set({root:e}),w()}get hidden(){return this.$$.ctx[9]}set hidden(e){this.$$set({hidden:e}),w()}get format(){return this.$$.ctx[22]}set format(e){this.$$set({format:e}),w()}get uploading(){return this.$$.ctx[1]}set uploading(e){this.$$set({uploading:e}),w()}get hidden_upload(){return this.$$.ctx[2]}set hidden_upload(e){this.$$set({hidden_upload:e}),w()}get show_progress(){return this.$$.ctx[10]}set show_progress(e){this.$$set({show_progress:e}),w()}get max_file_size(){return this.$$.ctx[23]}set max_file_size(e){this.$$set({max_file_size:e}),w()}get upload(){return this.$$.ctx[24]}set upload(e){this.$$set({upload:e}),w()}get stream_handler(){return this.$$.ctx[11]}set stream_handler(e){this.$$set({stream_handler:e}),w()}get icon_upload(){return this.$$.ctx[12]}set icon_upload(e){this.$$set({icon_upload:e}),w()}get paste_clipboard(){return this.$$.ctx[13]}get open_file_upload(){return this.$$.ctx[14]}get load_files(){return this.$$.ctx[25]}}export{dt as U};
//# sourceMappingURL=Upload-SRQN7pqf.js.map