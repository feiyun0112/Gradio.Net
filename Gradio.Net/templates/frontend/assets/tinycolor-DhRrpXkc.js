const{SvelteComponent:I,append:P,attr:v,detach:N,init:L,insert:B,noop:x,safe_not_equal:z,svg_element:S}=window.__gradio__svelte__internal;function G(t){let r,e;return{c(){r=S("svg"),e=S("path"),v(e,"fill","currentColor"),v(e,"d","M21.03 2.97a3.578 3.578 0 0 0-5.06 0L14 4.94l-.013-.013a1.75 1.75 0 0 0-2.475 0l-.585.586a1.75 1.75 0 0 0 0 2.475l.012.012l-6.78 6.78a2.25 2.25 0 0 0-.659 1.592v.687l-1.28 2.347c-.836 1.533.841 3.21 2.374 2.375l2.347-1.28h.688a2.25 2.25 0 0 0 1.59-.66L16 13.061l.012.012a1.75 1.75 0 0 0 2.475 0l.586-.585a1.75 1.75 0 0 0 0-2.475L19.061 10l1.97-1.97a3.578 3.578 0 0 0 0-5.06ZM12 9.061l2.94 2.94l-6.78 6.78a.75.75 0 0 1-.531.22H6.75a.75.75 0 0 0-.359.09l-2.515 1.373a.234.234 0 0 1-.159.032a.264.264 0 0 1-.138-.075a.264.264 0 0 1-.075-.138a.234.234 0 0 1 .033-.158l1.372-2.515A.75.75 0 0 0 5 17.25v-.878a.75.75 0 0 1 .22-.53L12 9.06Z"),v(r,"xmlns","http://www.w3.org/2000/svg"),v(r,"width","100%"),v(r,"height","100%"),v(r,"viewBox","0 0 24 24")},m(a,i){B(a,r,i),P(r,e)},p:x,i:x,o:x,d(a){a&&N(r)}}}class ot extends I{constructor(r){super(),L(this,r,null,G,z,{})}}function y(t){"@babel/helpers - typeof";return y=typeof Symbol=="function"&&typeof Symbol.iterator=="symbol"?function(r){return typeof r}:function(r){return r&&typeof Symbol=="function"&&r.constructor===Symbol&&r!==Symbol.prototype?"symbol":typeof r},y(t)}var O=/^\s+/,q=/\s+$/;function s(t,r){if(t=t||"",r=r||{},t instanceof s)return t;if(!(this instanceof s))return new s(t,r);var e=D(t);this._originalInput=t,this._r=e.r,this._g=e.g,this._b=e.b,this._a=e.a,this._roundA=Math.round(100*this._a)/100,this._format=r.format||e.format,this._gradientType=r.gradientType,this._r<1&&(this._r=Math.round(this._r)),this._g<1&&(this._g=Math.round(this._g)),this._b<1&&(this._b=Math.round(this._b)),this._ok=e.ok}s.prototype={isDark:function(){return this.getBrightness()<128},isLight:function(){return!this.isDark()},isValid:function(){return this._ok},getOriginalInput:function(){return this._originalInput},getFormat:function(){return this._format},getAlpha:function(){return this._a},getBrightness:function(){var r=this.toRgb();return(r.r*299+r.g*587+r.b*114)/1e3},getLuminance:function(){var r=this.toRgb(),e,a,i,n,h,f;return e=r.r/255,a=r.g/255,i=r.b/255,e<=.03928?n=e/12.92:n=Math.pow((e+.055)/1.055,2.4),a<=.03928?h=a/12.92:h=Math.pow((a+.055)/1.055,2.4),i<=.03928?f=i/12.92:f=Math.pow((i+.055)/1.055,2.4),.2126*n+.7152*h+.0722*f},setAlpha:function(r){return this._a=C(r),this._roundA=Math.round(100*this._a)/100,this},toHsv:function(){var r=w(this._r,this._g,this._b);return{h:r.h*360,s:r.s,v:r.v,a:this._a}},toHsvString:function(){var r=w(this._r,this._g,this._b),e=Math.round(r.h*360),a=Math.round(r.s*100),i=Math.round(r.v*100);return this._a==1?"hsv("+e+", "+a+"%, "+i+"%)":"hsva("+e+", "+a+"%, "+i+"%, "+this._roundA+")"},toHsl:function(){var r=k(this._r,this._g,this._b);return{h:r.h*360,s:r.s,l:r.l,a:this._a}},toHslString:function(){var r=k(this._r,this._g,this._b),e=Math.round(r.h*360),a=Math.round(r.s*100),i=Math.round(r.l*100);return this._a==1?"hsl("+e+", "+a+"%, "+i+"%)":"hsla("+e+", "+a+"%, "+i+"%, "+this._roundA+")"},toHex:function(r){return H(this._r,this._g,this._b,r)},toHexString:function(r){return"#"+this.toHex(r)},toHex8:function(r){return j(this._r,this._g,this._b,this._a,r)},toHex8String:function(r){return"#"+this.toHex8(r)},toRgb:function(){return{r:Math.round(this._r),g:Math.round(this._g),b:Math.round(this._b),a:this._a}},toRgbString:function(){return this._a==1?"rgb("+Math.round(this._r)+", "+Math.round(this._g)+", "+Math.round(this._b)+")":"rgba("+Math.round(this._r)+", "+Math.round(this._g)+", "+Math.round(this._b)+", "+this._roundA+")"},toPercentageRgb:function(){return{r:Math.round(u(this._r,255)*100)+"%",g:Math.round(u(this._g,255)*100)+"%",b:Math.round(u(this._b,255)*100)+"%",a:this._a}},toPercentageRgbString:function(){return this._a==1?"rgb("+Math.round(u(this._r,255)*100)+"%, "+Math.round(u(this._g,255)*100)+"%, "+Math.round(u(this._b,255)*100)+"%)":"rgba("+Math.round(u(this._r,255)*100)+"%, "+Math.round(u(this._g,255)*100)+"%, "+Math.round(u(this._b,255)*100)+"%, "+this._roundA+")"},toName:function(){return this._a===0?"transparent":this._a<1?!1:nt[H(this._r,this._g,this._b,!0)]||!1},toFilter:function(r){var e="#"+R(this._r,this._g,this._b,this._a),a=e,i=this._gradientType?"GradientType = 1, ":"";if(r){var n=s(r);a="#"+R(n._r,n._g,n._b,n._a)}return"progid:DXImageTransform.Microsoft.gradient("+i+"startColorstr="+e+",endColorstr="+a+")"},toString:function(r){var e=!!r;r=r||this._format;var a=!1,i=this._a<1&&this._a>=0,n=!e&&i&&(r==="hex"||r==="hex6"||r==="hex3"||r==="hex4"||r==="hex8"||r==="name");return n?r==="name"&&this._a===0?this.toName():this.toRgbString():(r==="rgb"&&(a=this.toRgbString()),r==="prgb"&&(a=this.toPercentageRgbString()),(r==="hex"||r==="hex6")&&(a=this.toHexString()),r==="hex3"&&(a=this.toHexString(!0)),r==="hex4"&&(a=this.toHex8String(!0)),r==="hex8"&&(a=this.toHex8String()),r==="name"&&(a=this.toName()),r==="hsl"&&(a=this.toHslString()),r==="hsv"&&(a=this.toHsvString()),a||this.toHexString())},clone:function(){return s(this.toString())},_applyModification:function(r,e){var a=r.apply(null,[this].concat([].slice.call(e)));return this._r=a._r,this._g=a._g,this._b=a._b,this.setAlpha(a._a),this},lighten:function(){return this._applyModification(J,arguments)},brighten:function(){return this._applyModification(K,arguments)},darken:function(){return this._applyModification(Q,arguments)},desaturate:function(){return this._applyModification(Z,arguments)},saturate:function(){return this._applyModification(W,arguments)},greyscale:function(){return this._applyModification(X,arguments)},spin:function(){return this._applyModification(Y,arguments)},_applyCombination:function(r,e){return r.apply(null,[this].concat([].slice.call(e)))},analogous:function(){return this._applyCombination(rt,arguments)},complement:function(){return this._applyCombination(tt,arguments)},monochromatic:function(){return this._applyCombination(at,arguments)},splitcomplement:function(){return this._applyCombination(et,arguments)},triad:function(){return this._applyCombination(F,[3])},tetrad:function(){return this._applyCombination(F,[4])}};s.fromRatio=function(t,r){if(y(t)=="object"){var e={};for(var a in t)t.hasOwnProperty(a)&&(a==="a"?e[a]=t[a]:e[a]=m(t[a]));t=e}return s(t,r)};function D(t){var r={r:0,g:0,b:0},e=1,a=null,i=null,n=null,h=!1,f=!1;return typeof t=="string"&&(t=ft(t)),y(t)=="object"&&(p(t.r)&&p(t.g)&&p(t.b)?(r=U(t.r,t.g,t.b),h=!0,f=String(t.r).substr(-1)==="%"?"prgb":"rgb"):p(t.h)&&p(t.s)&&p(t.v)?(a=m(t.s),i=m(t.v),r=$(t.h,a,i),h=!0,f="hsv"):p(t.h)&&p(t.s)&&p(t.l)&&(a=m(t.s),n=m(t.l),r=V(t.h,a,n),h=!0,f="hsl"),t.hasOwnProperty("a")&&(e=t.a)),e=C(e),{ok:h,format:t.format||f,r:Math.min(255,Math.max(r.r,0)),g:Math.min(255,Math.max(r.g,0)),b:Math.min(255,Math.max(r.b,0)),a:e}}function U(t,r,e){return{r:u(t,255)*255,g:u(r,255)*255,b:u(e,255)*255}}function k(t,r,e){t=u(t,255),r=u(r,255),e=u(e,255);var a=Math.max(t,r,e),i=Math.min(t,r,e),n,h,f=(a+i)/2;if(a==i)n=h=0;else{var o=a-i;switch(h=f>.5?o/(2-a-i):o/(a+i),a){case t:n=(r-e)/o+(r<e?6:0);break;case r:n=(e-t)/o+2;break;case e:n=(t-r)/o+4;break}n/=6}return{h:n,s:h,l:f}}function V(t,r,e){var a,i,n;t=u(t,360),r=u(r,100),e=u(e,100);function h(g,_,d){return d<0&&(d+=1),d>1&&(d-=1),d<1/6?g+(_-g)*6*d:d<1/2?_:d<2/3?g+(_-g)*(2/3-d)*6:g}if(r===0)a=i=n=e;else{var f=e<.5?e*(1+r):e+r-e*r,o=2*e-f;a=h(o,f,t+1/3),i=h(o,f,t),n=h(o,f,t-1/3)}return{r:a*255,g:i*255,b:n*255}}function w(t,r,e){t=u(t,255),r=u(r,255),e=u(e,255);var a=Math.max(t,r,e),i=Math.min(t,r,e),n,h,f=a,o=a-i;if(h=a===0?0:o/a,a==i)n=0;else{switch(a){case t:n=(r-e)/o+(r<e?6:0);break;case r:n=(e-t)/o+2;break;case e:n=(t-r)/o+4;break}n/=6}return{h:n,s:h,v:f}}function $(t,r,e){t=u(t,360)*6,r=u(r,100),e=u(e,100);var a=Math.floor(t),i=t-a,n=e*(1-r),h=e*(1-i*r),f=e*(1-(1-i)*r),o=a%6,g=[e,h,n,n,f,e][o],_=[f,e,e,h,n,n][o],d=[n,n,f,e,e,h][o];return{r:g*255,g:_*255,b:d*255}}function H(t,r,e,a){var i=[b(Math.round(t).toString(16)),b(Math.round(r).toString(16)),b(Math.round(e).toString(16))];return a&&i[0].charAt(0)==i[0].charAt(1)&&i[1].charAt(0)==i[1].charAt(1)&&i[2].charAt(0)==i[2].charAt(1)?i[0].charAt(0)+i[1].charAt(0)+i[2].charAt(0):i.join("")}function j(t,r,e,a,i){var n=[b(Math.round(t).toString(16)),b(Math.round(r).toString(16)),b(Math.round(e).toString(16)),b(E(a))];return i&&n[0].charAt(0)==n[0].charAt(1)&&n[1].charAt(0)==n[1].charAt(1)&&n[2].charAt(0)==n[2].charAt(1)&&n[3].charAt(0)==n[3].charAt(1)?n[0].charAt(0)+n[1].charAt(0)+n[2].charAt(0)+n[3].charAt(0):n.join("")}function R(t,r,e,a){var i=[b(E(a)),b(Math.round(t).toString(16)),b(Math.round(r).toString(16)),b(Math.round(e).toString(16))];return i.join("")}s.equals=function(t,r){return!t||!r?!1:s(t).toRgbString()==s(r).toRgbString()};s.random=function(){return s.fromRatio({r:Math.random(),g:Math.random(),b:Math.random()})};function Z(t,r){r=r===0?0:r||10;var e=s(t).toHsl();return e.s-=r/100,e.s=M(e.s),s(e)}function W(t,r){r=r===0?0:r||10;var e=s(t).toHsl();return e.s+=r/100,e.s=M(e.s),s(e)}function X(t){return s(t).desaturate(100)}function J(t,r){r=r===0?0:r||10;var e=s(t).toHsl();return e.l+=r/100,e.l=M(e.l),s(e)}function K(t,r){r=r===0?0:r||10;var e=s(t).toRgb();return e.r=Math.max(0,Math.min(255,e.r-Math.round(255*-(r/100)))),e.g=Math.max(0,Math.min(255,e.g-Math.round(255*-(r/100)))),e.b=Math.max(0,Math.min(255,e.b-Math.round(255*-(r/100)))),s(e)}function Q(t,r){r=r===0?0:r||10;var e=s(t).toHsl();return e.l-=r/100,e.l=M(e.l),s(e)}function Y(t,r){var e=s(t).toHsl(),a=(e.h+r)%360;return e.h=a<0?360+a:a,s(e)}function tt(t){var r=s(t).toHsl();return r.h=(r.h+180)%360,s(r)}function F(t,r){if(isNaN(r)||r<=0)throw new Error("Argument to polyad must be a positive number");for(var e=s(t).toHsl(),a=[s(t)],i=360/r,n=1;n<r;n++)a.push(s({h:(e.h+n*i)%360,s:e.s,l:e.l}));return a}function et(t){var r=s(t).toHsl(),e=r.h;return[s(t),s({h:(e+72)%360,s:r.s,l:r.l}),s({h:(e+216)%360,s:r.s,l:r.l})]}function rt(t,r,e){r=r||6,e=e||30;var a=s(t).toHsl(),i=360/e,n=[s(t)];for(a.h=(a.h-(i*r>>1)+720)%360;--r;)a.h=(a.h+i)%360,n.push(s(a));return n}function at(t,r){r=r||6;for(var e=s(t).toHsv(),a=e.h,i=e.s,n=e.v,h=[],f=1/r;r--;)h.push(s({h:a,s:i,v:n})),n=(n+f)%1;return h}s.mix=function(t,r,e){e=e===0?0:e||50;var a=s(t).toRgb(),i=s(r).toRgb(),n=e/100,h={r:(i.r-a.r)*n+a.r,g:(i.g-a.g)*n+a.g,b:(i.b-a.b)*n+a.b,a:(i.a-a.a)*n+a.a};return s(h)};s.readability=function(t,r){var e=s(t),a=s(r);return(Math.max(e.getLuminance(),a.getLuminance())+.05)/(Math.min(e.getLuminance(),a.getLuminance())+.05)};s.isReadable=function(t,r,e){var a=s.readability(t,r),i,n;switch(n=!1,i=ut(e),i.level+i.size){case"AAsmall":case"AAAlarge":n=a>=4.5;break;case"AAlarge":n=a>=3;break;case"AAAsmall":n=a>=7;break}return n};s.mostReadable=function(t,r,e){var a=null,i=0,n,h,f,o;e=e||{},h=e.includeFallbackColors,f=e.level,o=e.size;for(var g=0;g<r.length;g++)n=s.readability(t,r[g]),n>i&&(i=n,a=s(r[g]));return s.isReadable(t,a,{level:f,size:o})||!h?a:(e.includeFallbackColors=!1,s.mostReadable(t,["#fff","#000"],e))};var A=s.names={aliceblue:"f0f8ff",antiquewhite:"faebd7",aqua:"0ff",aquamarine:"7fffd4",azure:"f0ffff",beige:"f5f5dc",bisque:"ffe4c4",black:"000",blanchedalmond:"ffebcd",blue:"00f",blueviolet:"8a2be2",brown:"a52a2a",burlywood:"deb887",burntsienna:"ea7e5d",cadetblue:"5f9ea0",chartreuse:"7fff00",chocolate:"d2691e",coral:"ff7f50",cornflowerblue:"6495ed",cornsilk:"fff8dc",crimson:"dc143c",cyan:"0ff",darkblue:"00008b",darkcyan:"008b8b",darkgoldenrod:"b8860b",darkgray:"a9a9a9",darkgreen:"006400",darkgrey:"a9a9a9",darkkhaki:"bdb76b",darkmagenta:"8b008b",darkolivegreen:"556b2f",darkorange:"ff8c00",darkorchid:"9932cc",darkred:"8b0000",darksalmon:"e9967a",darkseagreen:"8fbc8f",darkslateblue:"483d8b",darkslategray:"2f4f4f",darkslategrey:"2f4f4f",darkturquoise:"00ced1",darkviolet:"9400d3",deeppink:"ff1493",deepskyblue:"00bfff",dimgray:"696969",dimgrey:"696969",dodgerblue:"1e90ff",firebrick:"b22222",floralwhite:"fffaf0",forestgreen:"228b22",fuchsia:"f0f",gainsboro:"dcdcdc",ghostwhite:"f8f8ff",gold:"ffd700",goldenrod:"daa520",gray:"808080",green:"008000",greenyellow:"adff2f",grey:"808080",honeydew:"f0fff0",hotpink:"ff69b4",indianred:"cd5c5c",indigo:"4b0082",ivory:"fffff0",khaki:"f0e68c",lavender:"e6e6fa",lavenderblush:"fff0f5",lawngreen:"7cfc00",lemonchiffon:"fffacd",lightblue:"add8e6",lightcoral:"f08080",lightcyan:"e0ffff",lightgoldenrodyellow:"fafad2",lightgray:"d3d3d3",lightgreen:"90ee90",lightgrey:"d3d3d3",lightpink:"ffb6c1",lightsalmon:"ffa07a",lightseagreen:"20b2aa",lightskyblue:"87cefa",lightslategray:"789",lightslategrey:"789",lightsteelblue:"b0c4de",lightyellow:"ffffe0",lime:"0f0",limegreen:"32cd32",linen:"faf0e6",magenta:"f0f",maroon:"800000",mediumaquamarine:"66cdaa",mediumblue:"0000cd",mediumorchid:"ba55d3",mediumpurple:"9370db",mediumseagreen:"3cb371",mediumslateblue:"7b68ee",mediumspringgreen:"00fa9a",mediumturquoise:"48d1cc",mediumvioletred:"c71585",midnightblue:"191970",mintcream:"f5fffa",mistyrose:"ffe4e1",moccasin:"ffe4b5",navajowhite:"ffdead",navy:"000080",oldlace:"fdf5e6",olive:"808000",olivedrab:"6b8e23",orange:"ffa500",orangered:"ff4500",orchid:"da70d6",palegoldenrod:"eee8aa",palegreen:"98fb98",paleturquoise:"afeeee",palevioletred:"db7093",papayawhip:"ffefd5",peachpuff:"ffdab9",peru:"cd853f",pink:"ffc0cb",plum:"dda0dd",powderblue:"b0e0e6",purple:"800080",rebeccapurple:"663399",red:"f00",rosybrown:"bc8f8f",royalblue:"4169e1",saddlebrown:"8b4513",salmon:"fa8072",sandybrown:"f4a460",seagreen:"2e8b57",seashell:"fff5ee",sienna:"a0522d",silver:"c0c0c0",skyblue:"87ceeb",slateblue:"6a5acd",slategray:"708090",slategrey:"708090",snow:"fffafa",springgreen:"00ff7f",steelblue:"4682b4",tan:"d2b48c",teal:"008080",thistle:"d8bfd8",tomato:"ff6347",turquoise:"40e0d0",violet:"ee82ee",wheat:"f5deb3",white:"fff",whitesmoke:"f5f5f5",yellow:"ff0",yellowgreen:"9acd32"},nt=s.hexNames=it(A);function it(t){var r={};for(var e in t)t.hasOwnProperty(e)&&(r[t[e]]=e);return r}function C(t){return t=parseFloat(t),(isNaN(t)||t<0||t>1)&&(t=1),t}function u(t,r){st(t)&&(t="100%");var e=ht(t);return t=Math.min(r,Math.max(0,parseFloat(t))),e&&(t=parseInt(t*r,10)/100),Math.abs(t-r)<1e-6?1:t%r/parseFloat(r)}function M(t){return Math.min(1,Math.max(0,t))}function l(t){return parseInt(t,16)}function st(t){return typeof t=="string"&&t.indexOf(".")!=-1&&parseFloat(t)===1}function ht(t){return typeof t=="string"&&t.indexOf("%")!=-1}function b(t){return t.length==1?"0"+t:""+t}function m(t){return t<=1&&(t=t*100+"%"),t}function E(t){return Math.round(parseFloat(t)*255).toString(16)}function T(t){return l(t)/255}var c=function(){var t="[-\\+]?\\d+%?",r="[-\\+]?\\d*\\.\\d+%?",e="(?:"+r+")|(?:"+t+")",a="[\\s|\\(]+("+e+")[,|\\s]+("+e+")[,|\\s]+("+e+")\\s*\\)?",i="[\\s|\\(]+("+e+")[,|\\s]+("+e+")[,|\\s]+("+e+")[,|\\s]+("+e+")\\s*\\)?";return{CSS_UNIT:new RegExp(e),rgb:new RegExp("rgb"+a),rgba:new RegExp("rgba"+i),hsl:new RegExp("hsl"+a),hsla:new RegExp("hsla"+i),hsv:new RegExp("hsv"+a),hsva:new RegExp("hsva"+i),hex3:/^#?([0-9a-fA-F]{1})([0-9a-fA-F]{1})([0-9a-fA-F]{1})$/,hex6:/^#?([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$/,hex4:/^#?([0-9a-fA-F]{1})([0-9a-fA-F]{1})([0-9a-fA-F]{1})([0-9a-fA-F]{1})$/,hex8:/^#?([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$/}}();function p(t){return!!c.CSS_UNIT.exec(t)}function ft(t){t=t.replace(O,"").replace(q,"").toLowerCase();var r=!1;if(A[t])t=A[t],r=!0;else if(t=="transparent")return{r:0,g:0,b:0,a:0,format:"name"};var e;return(e=c.rgb.exec(t))?{r:e[1],g:e[2],b:e[3]}:(e=c.rgba.exec(t))?{r:e[1],g:e[2],b:e[3],a:e[4]}:(e=c.hsl.exec(t))?{h:e[1],s:e[2],l:e[3]}:(e=c.hsla.exec(t))?{h:e[1],s:e[2],l:e[3],a:e[4]}:(e=c.hsv.exec(t))?{h:e[1],s:e[2],v:e[3]}:(e=c.hsva.exec(t))?{h:e[1],s:e[2],v:e[3],a:e[4]}:(e=c.hex8.exec(t))?{r:l(e[1]),g:l(e[2]),b:l(e[3]),a:T(e[4]),format:r?"name":"hex8"}:(e=c.hex6.exec(t))?{r:l(e[1]),g:l(e[2]),b:l(e[3]),format:r?"name":"hex"}:(e=c.hex4.exec(t))?{r:l(e[1]+""+e[1]),g:l(e[2]+""+e[2]),b:l(e[3]+""+e[3]),a:T(e[4]+""+e[4]),format:r?"name":"hex8"}:(e=c.hex3.exec(t))?{r:l(e[1]+""+e[1]),g:l(e[2]+""+e[2]),b:l(e[3]+""+e[3]),format:r?"name":"hex"}:!1}function ut(t){var r,e;return t=t||{level:"AA",size:"small"},r=(t.level||"AA").toUpperCase(),e=(t.size||"small").toLowerCase(),r!=="AA"&&r!=="AAA"&&(r="AA"),e!=="small"&&e!=="large"&&(e="small"),{level:r,size:e}}export{ot as E,s as t};
//# sourceMappingURL=tinycolor-DhRrpXkc.js.map