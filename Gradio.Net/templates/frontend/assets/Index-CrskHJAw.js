import{T as y}from"./Textbox-CbeUAC4I.js";import{B as p}from"./Block-rEXcgPfT.js";import"./MarkdownCode.svelte_svelte_type_style_lang-CaEo_I2i.js";import"./IconButtonWrapper.svelte_svelte_type_style_lang-DAP8_Zsr.js";import{S as $}from"./index-DbBaXY2J.js";import"./StreamingBar.svelte_svelte_type_style_lang-CxOfZBE-.js";import{default as Kt}from"./Example-DN4wtGrM.js";import"./BlockTitle-DifTNC0o.js";import"./Info-CCva9LDo.js";import"./MarkdownCode-CzoKYvsr.js";import"./Check-BiRlaMNo.js";import"./Copy-CxQ9EyK2.js";import"./Send-DyoOovnk.js";import"./Square-oAGqOwsh.js";import"./index-Ymvr2fNH.js";import"./index-Ya43XIXp.js";import"./svelte/svelte.js";/* empty css                                              */import"./prism-python-DMUsXzi0.js";import"./IconButton-DtUbToT-.js";import"./Clear-By3xiIwg.js";/* empty css                                              */const{SvelteComponent:tt,add_flush_callback:J,assign:et,bind:K,binding_callbacks:L,check_outros:it,create_component:v,destroy_component:k,detach:st,flush:_,get_spread_object:lt,get_spread_update:nt,group_outros:ut,init:ot,insert:at,mount_component:B,safe_not_equal:_t,space:ft,transition_in:m,transition_out:b}=window.__gradio__svelte__internal;function M(s){let t,i;const n=[{autoscroll:s[2].autoscroll},{i18n:s[2].i18n},s[19]];let h={};for(let u=0;u<n.length;u+=1)h=et(h,n[u]);return t=new $({props:h}),t.$on("clear_status",s[27]),{c(){v(t.$$.fragment)},m(u,a){B(t,u,a),i=!0},p(u,a){const c=a[0]&524292?nt(n,[a[0]&4&&{autoscroll:u[2].autoscroll},a[0]&4&&{i18n:u[2].i18n},a[0]&524288&&lt(u[19])]):{};t.$set(c)},i(u){i||(m(t.$$.fragment,u),i=!0)},o(u){b(t.$$.fragment,u),i=!1},d(u){k(t,u)}}}function ht(s){let t,i,n,h,u,a=s[19]&&M(s);function c(l){s[28](l)}function g(l){s[29](l)}let r={label:s[3],info:s[4],root:s[25],show_label:s[10],lines:s[8],type:s[12],rtl:s[20],text_align:s[21],max_lines:s[11]?s[11]:s[8]+1,placeholder:s[9],submit_btn:s[16],stop_btn:s[17],show_copy_button:s[18],autofocus:s[22],container:s[13],autoscroll:s[23],max_length:s[26],disabled:!s[24]};return s[0]!==void 0&&(r.value=s[0]),s[1]!==void 0&&(r.value_is_output=s[1]),i=new y({props:r}),L.push(()=>K(i,"value",c)),L.push(()=>K(i,"value_is_output",g)),i.$on("change",s[30]),i.$on("input",s[31]),i.$on("submit",s[32]),i.$on("blur",s[33]),i.$on("select",s[34]),i.$on("focus",s[35]),i.$on("stop",s[36]),{c(){a&&a.c(),t=ft(),v(i.$$.fragment)},m(l,o){a&&a.m(l,o),at(l,t,o),B(i,l,o),u=!0},p(l,o){l[19]?a?(a.p(l,o),o[0]&524288&&m(a,1)):(a=M(l),a.c(),m(a,1),a.m(t.parentNode,t)):a&&(ut(),b(a,1,1,()=>{a=null}),it());const f={};o[0]&8&&(f.label=l[3]),o[0]&16&&(f.info=l[4]),o[0]&33554432&&(f.root=l[25]),o[0]&1024&&(f.show_label=l[10]),o[0]&256&&(f.lines=l[8]),o[0]&4096&&(f.type=l[12]),o[0]&1048576&&(f.rtl=l[20]),o[0]&2097152&&(f.text_align=l[21]),o[0]&2304&&(f.max_lines=l[11]?l[11]:l[8]+1),o[0]&512&&(f.placeholder=l[9]),o[0]&65536&&(f.submit_btn=l[16]),o[0]&131072&&(f.stop_btn=l[17]),o[0]&262144&&(f.show_copy_button=l[18]),o[0]&4194304&&(f.autofocus=l[22]),o[0]&8192&&(f.container=l[13]),o[0]&8388608&&(f.autoscroll=l[23]),o[0]&67108864&&(f.max_length=l[26]),o[0]&16777216&&(f.disabled=!l[24]),!n&&o[0]&1&&(n=!0,f.value=l[0],J(()=>n=!1)),!h&&o[0]&2&&(h=!0,f.value_is_output=l[1],J(()=>h=!1)),i.$set(f)},i(l){u||(m(a),m(i.$$.fragment,l),u=!0)},o(l){b(a),b(i.$$.fragment,l),u=!1},d(l){l&&st(t),a&&a.d(l),k(i,l)}}}function rt(s){let t,i;return t=new p({props:{visible:s[7],elem_id:s[5],elem_classes:s[6],scale:s[14],min_width:s[15],allow_overflow:!1,padding:s[13],$$slots:{default:[ht]},$$scope:{ctx:s}}}),{c(){v(t.$$.fragment)},m(n,h){B(t,n,h),i=!0},p(n,h){const u={};h[0]&128&&(u.visible=n[7]),h[0]&32&&(u.elem_id=n[5]),h[0]&64&&(u.elem_classes=n[6]),h[0]&16384&&(u.scale=n[14]),h[0]&32768&&(u.min_width=n[15]),h[0]&8192&&(u.padding=n[13]),h[0]&134168351|h[1]&64&&(u.$$scope={dirty:h,ctx:n}),t.$set(u)},i(n){i||(m(t.$$.fragment,n),i=!0)},o(n){b(t.$$.fragment,n),i=!1},d(n){k(t,n)}}}function ct(s,t,i){let{gradio:n}=t,{label:h="Textbox"}=t,{info:u=void 0}=t,{elem_id:a=""}=t,{elem_classes:c=[]}=t,{visible:g=!0}=t,{value:r=""}=t,{lines:l}=t,{placeholder:o=""}=t,{show_label:f}=t,{max_lines:T}=t,{type:S="text"}=t,{container:x=!0}=t,{scale:j=null}=t,{min_width:q=void 0}=t,{submit_btn:C=null}=t,{stop_btn:E=null}=t,{show_copy_button:I=!1}=t,{loading_status:w=void 0}=t,{value_is_output:d=!1}=t,{rtl:N=!1}=t,{text_align:z=void 0}=t,{autofocus:A=!1}=t,{autoscroll:D=!0}=t,{interactive:F}=t,{root:G}=t,{max_length:H=void 0}=t;const O=()=>n.dispatch("clear_status",w);function P(e){r=e,i(0,r)}function Q(e){d=e,i(1,d)}const R=()=>n.dispatch("change",r),U=()=>n.dispatch("input"),V=()=>n.dispatch("submit"),W=()=>n.dispatch("blur"),X=e=>n.dispatch("select",e.detail),Y=()=>n.dispatch("focus"),Z=()=>n.dispatch("stop");return s.$$set=e=>{"gradio"in e&&i(2,n=e.gradio),"label"in e&&i(3,h=e.label),"info"in e&&i(4,u=e.info),"elem_id"in e&&i(5,a=e.elem_id),"elem_classes"in e&&i(6,c=e.elem_classes),"visible"in e&&i(7,g=e.visible),"value"in e&&i(0,r=e.value),"lines"in e&&i(8,l=e.lines),"placeholder"in e&&i(9,o=e.placeholder),"show_label"in e&&i(10,f=e.show_label),"max_lines"in e&&i(11,T=e.max_lines),"type"in e&&i(12,S=e.type),"container"in e&&i(13,x=e.container),"scale"in e&&i(14,j=e.scale),"min_width"in e&&i(15,q=e.min_width),"submit_btn"in e&&i(16,C=e.submit_btn),"stop_btn"in e&&i(17,E=e.stop_btn),"show_copy_button"in e&&i(18,I=e.show_copy_button),"loading_status"in e&&i(19,w=e.loading_status),"value_is_output"in e&&i(1,d=e.value_is_output),"rtl"in e&&i(20,N=e.rtl),"text_align"in e&&i(21,z=e.text_align),"autofocus"in e&&i(22,A=e.autofocus),"autoscroll"in e&&i(23,D=e.autoscroll),"interactive"in e&&i(24,F=e.interactive),"root"in e&&i(25,G=e.root),"max_length"in e&&i(26,H=e.max_length)},[r,d,n,h,u,a,c,g,l,o,f,T,S,x,j,q,C,E,I,w,N,z,A,D,F,G,H,O,P,Q,R,U,V,W,X,Y,Z]}class Gt extends tt{constructor(t){super(),ot(this,t,ct,rt,_t,{gradio:2,label:3,info:4,elem_id:5,elem_classes:6,visible:7,value:0,lines:8,placeholder:9,show_label:10,max_lines:11,type:12,container:13,scale:14,min_width:15,submit_btn:16,stop_btn:17,show_copy_button:18,loading_status:19,value_is_output:1,rtl:20,text_align:21,autofocus:22,autoscroll:23,interactive:24,root:25,max_length:26},null,[-1,-1])}get gradio(){return this.$$.ctx[2]}set gradio(t){this.$$set({gradio:t}),_()}get label(){return this.$$.ctx[3]}set label(t){this.$$set({label:t}),_()}get info(){return this.$$.ctx[4]}set info(t){this.$$set({info:t}),_()}get elem_id(){return this.$$.ctx[5]}set elem_id(t){this.$$set({elem_id:t}),_()}get elem_classes(){return this.$$.ctx[6]}set elem_classes(t){this.$$set({elem_classes:t}),_()}get visible(){return this.$$.ctx[7]}set visible(t){this.$$set({visible:t}),_()}get value(){return this.$$.ctx[0]}set value(t){this.$$set({value:t}),_()}get lines(){return this.$$.ctx[8]}set lines(t){this.$$set({lines:t}),_()}get placeholder(){return this.$$.ctx[9]}set placeholder(t){this.$$set({placeholder:t}),_()}get show_label(){return this.$$.ctx[10]}set show_label(t){this.$$set({show_label:t}),_()}get max_lines(){return this.$$.ctx[11]}set max_lines(t){this.$$set({max_lines:t}),_()}get type(){return this.$$.ctx[12]}set type(t){this.$$set({type:t}),_()}get container(){return this.$$.ctx[13]}set container(t){this.$$set({container:t}),_()}get scale(){return this.$$.ctx[14]}set scale(t){this.$$set({scale:t}),_()}get min_width(){return this.$$.ctx[15]}set min_width(t){this.$$set({min_width:t}),_()}get submit_btn(){return this.$$.ctx[16]}set submit_btn(t){this.$$set({submit_btn:t}),_()}get stop_btn(){return this.$$.ctx[17]}set stop_btn(t){this.$$set({stop_btn:t}),_()}get show_copy_button(){return this.$$.ctx[18]}set show_copy_button(t){this.$$set({show_copy_button:t}),_()}get loading_status(){return this.$$.ctx[19]}set loading_status(t){this.$$set({loading_status:t}),_()}get value_is_output(){return this.$$.ctx[1]}set value_is_output(t){this.$$set({value_is_output:t}),_()}get rtl(){return this.$$.ctx[20]}set rtl(t){this.$$set({rtl:t}),_()}get text_align(){return this.$$.ctx[21]}set text_align(t){this.$$set({text_align:t}),_()}get autofocus(){return this.$$.ctx[22]}set autofocus(t){this.$$set({autofocus:t}),_()}get autoscroll(){return this.$$.ctx[23]}set autoscroll(t){this.$$set({autoscroll:t}),_()}get interactive(){return this.$$.ctx[24]}set interactive(t){this.$$set({interactive:t}),_()}get root(){return this.$$.ctx[25]}set root(t){this.$$set({root:t}),_()}get max_length(){return this.$$.ctx[26]}set max_length(t){this.$$set({max_length:t}),_()}}export{Kt as BaseExample,y as BaseTextbox,Gt as default};
//# sourceMappingURL=Index-CrskHJAw.js.map