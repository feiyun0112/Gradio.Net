import"./IconButtonWrapper.svelte_svelte_type_style_lang-DAP8_Zsr.js";const{SvelteComponent:_,append:h,attr:c,create_component:d,destroy_component:g,detach:m,element:u,flush:b,init:w,insert:I,mount_component:k,safe_not_equal:v,set_data:B,space:q,text:C,toggle_class:o,transition_in:L,transition_out:S}=window.__gradio__svelte__internal;function j(s){let e,a,i,r,f,n;return i=new s[1]({}),{c(){e=u("label"),a=u("span"),d(i.$$.fragment),r=q(),f=C(s[0]),c(a,"class","svelte-i3tvor"),c(e,"for",""),c(e,"data-testid","block-label"),c(e,"class","svelte-i3tvor"),o(e,"hide",!s[2]),o(e,"sr-only",!s[2]),o(e,"float",s[4]),o(e,"hide-label",s[3])},m(l,t){I(l,e,t),h(e,a),k(i,a,null),h(e,r),h(e,f),n=!0},p(l,[t]){(!n||t&1)&&B(f,l[0]),(!n||t&4)&&o(e,"hide",!l[2]),(!n||t&4)&&o(e,"sr-only",!l[2]),(!n||t&16)&&o(e,"float",l[4]),(!n||t&8)&&o(e,"hide-label",l[3])},i(l){n||(L(i.$$.fragment,l),n=!0)},o(l){S(i.$$.fragment,l),n=!1},d(l){l&&m(e),g(i)}}}function z(s,e,a){let{label:i=null}=e,{Icon:r}=e,{show_label:f=!0}=e,{disable:n=!1}=e,{float:l=!0}=e;return s.$$set=t=>{"label"in t&&a(0,i=t.label),"Icon"in t&&a(1,r=t.Icon),"show_label"in t&&a(2,f=t.show_label),"disable"in t&&a(3,n=t.disable),"float"in t&&a(4,l=t.float)},[i,r,f,n,l]}class D extends _{constructor(e){super(),w(this,e,z,j,v,{label:0,Icon:1,show_label:2,disable:3,float:4})}get label(){return this.$$.ctx[0]}set label(e){this.$$set({label:e}),b()}get Icon(){return this.$$.ctx[1]}set Icon(e){this.$$set({Icon:e}),b()}get show_label(){return this.$$.ctx[2]}set show_label(e){this.$$set({show_label:e}),b()}get disable(){return this.$$.ctx[3]}set disable(e){this.$$set({disable:e}),b()}get float(){return this.$$.ctx[4]}set float(e){this.$$set({float:e}),b()}}export{D as B};
//# sourceMappingURL=BlockLabel-CnzaitFN.js.map