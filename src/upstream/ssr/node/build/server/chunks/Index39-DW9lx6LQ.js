import { K as h, P as tick } from './2-C9eYyMgE.js';
import { l as loop, r as raf, i as is_date, p } from './statustracker-Bh1XNgjP.js';
import { n as n$1, h as h$1, H as H$1, v, V as Ve$1, _, S as Se$1, a as He$1, g, i as ie$1, m as ee$1, z as ze$1 } from './src3-C1zEyqUA.js';
import { v as v$1 } from './src4-DEpL1Uf9.js';
import './async-Cv1-GZGV.js';
import { w as writable, s as spread_props, c as bind_props, d as derived, o as store_get, a as attr_class, b as attr_style, i as stringify, m as attributes, u as unsubscribe_stores } from './renderer-RGQlaTg4.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './server-Ck07o0bn.js';
import './html-CfyvkLET.js';

/*
Adapted from https://github.com/mattdesl
Distributed under MIT License https://github.com/mattdesl/eases/blob/master/LICENSE.md
*/

/**
 * @param {number} t
 * @returns {number}
 */
function linear(t) {
	return t;
}

/** @import { Task } from '../internal/client/types' */
/** @import { Tweened, TweenOptions } from './public' */

/**
 * @template T
 * @param {T} a
 * @param {T} b
 * @returns {(t: number) => T}
 */
function get_interpolator(a, b) {
	if (a === b || a !== a) return () => a;

	const type = typeof a;
	if (type !== typeof b || Array.isArray(a) !== Array.isArray(b)) {
		throw new Error('Cannot interpolate values of different type');
	}

	if (Array.isArray(a)) {
		const arr = /** @type {Array<any>} */ (b).map((bi, i) => {
			return get_interpolator(/** @type {Array<any>} */ (a)[i], bi);
		});

		// @ts-ignore
		return (t) => arr.map((fn) => fn(t));
	}

	if (type === 'object') {
		if (!a || !b) {
			throw new Error('Object cannot be null');
		}

		if (is_date(a) && is_date(b)) {
			const an = a.getTime();
			const bn = b.getTime();
			const delta = bn - an;

			// @ts-ignore
			return (t) => new Date(an + t * delta);
		}

		const keys = Object.keys(b);

		/** @type {Record<string, (t: number) => T>} */
		const interpolators = {};
		keys.forEach((key) => {
			// @ts-ignore
			interpolators[key] = get_interpolator(a[key], b[key]);
		});

		// @ts-ignore
		return (t) => {
			/** @type {Record<string, any>} */
			const result = {};
			keys.forEach((key) => {
				result[key] = interpolators[key](t);
			});
			return result;
		};
	}

	if (type === 'number') {
		const delta = /** @type {number} */ (b) - /** @type {number} */ (a);
		// @ts-ignore
		return (t) => a + t * delta;
	}

	// for non-numeric values, snap to the final value immediately
	return () => b;
}

/**
 * A tweened store in Svelte is a special type of store that provides smooth transitions between state values over time.
 *
 * @deprecated Use [`Tween`](https://svelte.dev/docs/svelte/svelte-motion#Tween) instead
 * @template T
 * @param {T} [value]
 * @param {TweenOptions<T>} [defaults]
 * @returns {Tweened<T>}
 */
function tweened(value, defaults = {}) {
	const store = writable(value);
	/** @type {Task} */
	let task;
	let target_value = value;
	/**
	 * @param {T} new_value
	 * @param {TweenOptions<T>} [opts]
	 */
	function set(new_value, opts) {
		target_value = new_value;

		if (value == null) {
			store.set((value = new_value));
			return Promise.resolve();
		}

		/** @type {Task | null} */
		let previous_task = task;

		let started = false;
		let {
			delay = 0,
			duration = 400,
			easing = linear,
			interpolate = get_interpolator
		} = { ...defaults, ...opts };

		if (duration === 0) {
			if (previous_task) {
				previous_task.abort();
				previous_task = null;
			}
			store.set((value = target_value));
			return Promise.resolve();
		}

		const start = raf.now() + delay;

		/** @type {(t: number) => T} */
		let fn;
		task = loop((now) => {
			if (now < start) return true;
			if (!started) {
				fn = interpolate(/** @type {any} */ (value), new_value);
				if (typeof duration === 'function')
					duration = duration(/** @type {any} */ (value), new_value);
				started = true;
			}
			if (previous_task) {
				previous_task.abort();
				previous_task = null;
			}
			const elapsed = now - start;
			if (elapsed > /** @type {number} */ (duration)) {
				store.set((value = new_value));
				return false;
			}
			// @ts-ignore
			store.set((value = fn(easing(elapsed / duration))));
			return true;
		});
		return task.promise;
	}
	return {
		set,
		update: (fn, opts) =>
			set(fn(/** @type {any} */ (target_value), /** @type {any} */ (value)), opts),
		subscribe: store.subscribe
	};
}

var e={value:()=>{}};function n(e){this._=e;}function r(e,t){return e.trim().split(/^|\s+/).map(function(e){var n=``,r=e.indexOf(`.`);if(r>=0&&(n=e.slice(r+1),e=e.slice(0,r)),e&&!t.hasOwnProperty(e))throw Error(`unknown type: `+e);return {type:e,name:n}})}n.prototype={constructor:n,on:function(e,t){var n=this._,o=r(e+``,n),s,c=-1,l=o.length;if(arguments.length<2){for(;++c<l;)if((s=(e=o[c]).type)&&(s=i(n[s],e.name)))return s;return}if(t!=null&&typeof t!=`function`)throw Error(`invalid callback: `+t);for(;++c<l;)if(s=(e=o[c]).type)n[s]=a(n[s],e.name,t);else if(t==null)for(s in n)n[s]=a(n[s],e.name,null);return this},copy:function(){var e={},t=this._;for(var r in t)e[r]=t[r].slice();return new n(e)},call:function(e,t){if((i=arguments.length-2)>0)for(var n=Array(i),r=0,i,a;r<i;++r)n[r]=arguments[r+2];if(!this._.hasOwnProperty(e))throw Error(`unknown type: `+e);for(a=this._[e],r=0,i=a.length;r<i;++r)a[r].value.apply(t,n);},apply:function(e,t,n){if(!this._.hasOwnProperty(e))throw Error(`unknown type: `+e);for(var r=this._[e],i=0,a=r.length;i<a;++i)r[i].value.apply(t,n);}};function i(e,t){for(var n=0,r=e.length,i;n<r;++n)if((i=e[n]).name===t)return i.value}function a(t,n,r){for(var i=0,a=t.length;i<a;++i)if(t[i].name===n){t[i]=e,t=t.slice(0,i).concat(t.slice(i+1));break}return r!=null&&t.push({name:n,value:r}),t}

var b={svg:`http://www.w3.org/2000/svg`,xhtml:`http://www.w3.org/1999/xhtml`,xlink:`http://www.w3.org/1999/xlink`,xml:`http://www.w3.org/XML/1998/namespace`,xmlns:`http://www.w3.org/2000/xmlns/`};function x(e){var t=e+=``,n=t.indexOf(`:`);return n>=0&&(t=e.slice(0,n))!==`xmlns`&&(e=e.slice(n+1)),b.hasOwnProperty(t)?{space:b[t],local:e}:e}function S(e){return function(){var t=this.ownerDocument,n=this.namespaceURI;return n===`http://www.w3.org/1999/xhtml`&&t.documentElement.namespaceURI===`http://www.w3.org/1999/xhtml`?t.createElement(e):t.createElementNS(n,e)}}function C(e){return function(){return this.ownerDocument.createElementNS(e.space,e.local)}}function w(e){var t=x(e);return (t.local?C:S)(t)}function T(){}function E(e){return e==null?T:function(){return this.querySelector(e)}}function D(e){typeof e!=`function`&&(e=E(e));for(var t=this._groups,n=t.length,r=Array(n),i=0;i<n;++i)for(var a=t[i],o=a.length,s=r[i]=Array(o),c,l,u=0;u<o;++u)(c=a[u])&&(l=e.call(c,c.__data__,u,a))&&(`__data__`in c&&(l.__data__=c.__data__),s[u]=l);return new G(r,this._parents)}function ee(e){return e==null?[]:Array.isArray(e)?e:Array.from(e)}function O(){return []}function k(e){return e==null?O:function(){return this.querySelectorAll(e)}}function A(e){return function(){return ee(e.apply(this,arguments))}}function j(e){e=typeof e==`function`?A(e):k(e);for(var t=this._groups,n=t.length,r=[],i=[],a=0;a<n;++a)for(var o=t[a],s=o.length,c,l=0;l<s;++l)(c=o[l])&&(r.push(e.call(c,c.__data__,l,o)),i.push(c));return new G(r,i)}function M(e){return function(){return this.matches(e)}}function N(e){return function(t){return t.matches(e)}}var P=Array.prototype.find;function F(e){return function(){return P.call(this.children,e)}}function I(){return this.firstElementChild}function L(e){return this.select(e==null?I:F(typeof e==`function`?e:N(e)))}var R=Array.prototype.filter;function te(){return Array.from(this.children)}function ne(e){return function(){return R.call(this.children,e)}}function re(e){return this.selectAll(e==null?te:ne(typeof e==`function`?e:N(e)))}function z(e){typeof e!=`function`&&(e=M(e));for(var t=this._groups,n=t.length,r=Array(n),i=0;i<n;++i)for(var a=t[i],o=a.length,s=r[i]=[],c,l=0;l<o;++l)(c=a[l])&&e.call(c,c.__data__,l,a)&&s.push(c);return new G(r,this._parents)}function B(e){return Array(e.length)}function V(){return new G(this._enter||this._groups.map(B),this._parents)}function H(e,t){this.ownerDocument=e.ownerDocument,this.namespaceURI=e.namespaceURI,this._next=null,this._parent=e,this.__data__=t;}H.prototype={constructor:H,appendChild:function(e){return this._parent.insertBefore(e,this._next)},insertBefore:function(e,t){return this._parent.insertBefore(e,t)},querySelector:function(e){return this._parent.querySelector(e)},querySelectorAll:function(e){return this._parent.querySelectorAll(e)}};function ie(e){return function(){return e}}function ae(e,t,n,r,i,a){for(var o=0,s,c=t.length,l=a.length;o<l;++o)(s=t[o])?(s.__data__=a[o],r[o]=s):n[o]=new H(e,a[o]);for(;o<c;++o)(s=t[o])&&(i[o]=s);}function oe(e,t,n,r,i,a,o){var s,c,l=new Map,u=t.length,d=a.length,f=Array(u),p;for(s=0;s<u;++s)(c=t[s])&&(f[s]=p=o.call(c,c.__data__,s,t)+``,l.has(p)?i[s]=c:l.set(p,c));for(s=0;s<d;++s)p=o.call(e,a[s],s,a)+``,(c=l.get(p))?(r[s]=c,c.__data__=a[s],l.delete(p)):n[s]=new H(e,a[s]);for(s=0;s<u;++s)(c=t[s])&&l.get(f[s])===c&&(i[s]=c);}function se(e){return e.__data__}function ce(e,t){if(!arguments.length)return Array.from(this,se);var n=t?oe:ae,r=this._parents,i=this._groups;typeof e!=`function`&&(e=ie(e));for(var a=i.length,o=Array(a),s=Array(a),c=Array(a),l=0;l<a;++l){var u=r[l],d=i[l],f=d.length,p=le(e.call(u,u&&u.__data__,l,r)),m=p.length,h=s[l]=Array(m),g=o[l]=Array(m);n(u,d,h,g,c[l]=Array(f),p,t);for(var _=0,v=0,y,b;_<m;++_)if(y=h[_]){for(_>=v&&(v=_+1);!(b=g[v])&&++v<m;);y._next=b||null;}}return o=new G(o,r),o._enter=s,o._exit=c,o}function le(e){return typeof e==`object`&&`length`in e?e:Array.from(e)}function ue(){return new G(this._exit||this._groups.map(B),this._parents)}function de(e,t,n){var r=this.enter(),i=this,a=this.exit();return typeof e==`function`?(r=e(r),r&&=r.selection()):r=r.append(e+``),t!=null&&(i=t(i),i&&=i.selection()),n==null?a.remove():n(a),r&&i?r.merge(i).order():i}function fe(e){for(var t=e.selection?e.selection():e,n=this._groups,r=t._groups,i=n.length,a=r.length,o=Math.min(i,a),s=Array(i),c=0;c<o;++c)for(var l=n[c],u=r[c],d=l.length,f=s[c]=Array(d),p,m=0;m<d;++m)(p=l[m]||u[m])&&(f[m]=p);for(;c<i;++c)s[c]=n[c];return new G(s,this._parents)}function pe(){for(var e=this._groups,t=-1,n=e.length;++t<n;)for(var r=e[t],i=r.length-1,a=r[i],o;--i>=0;)(o=r[i])&&(a&&o.compareDocumentPosition(a)^4&&a.parentNode.insertBefore(o,a),a=o);return this}function me(e){e||=he;function t(t,n){return t&&n?e(t.__data__,n.__data__):!t-!n}for(var n=this._groups,r=n.length,i=Array(r),a=0;a<r;++a){for(var o=n[a],s=o.length,c=i[a]=Array(s),l,u=0;u<s;++u)(l=o[u])&&(c[u]=l);c.sort(t);}return new G(i,this._parents).order()}function he(e,t){return e<t?-1:e>t?1:e>=t?0:NaN}function ge(){var e=arguments[0];return arguments[0]=this,e.apply(null,arguments),this}function _e(){return Array.from(this)}function ve(){for(var e=this._groups,t=0,n=e.length;t<n;++t)for(var r=e[t],i=0,a=r.length;i<a;++i){var o=r[i];if(o)return o}return null}function ye(){let e=0;for(let t of this)++e;return e}function be(){return !this.node()}function xe(e){for(var t=this._groups,n=0,r=t.length;n<r;++n)for(var i=t[n],a=0,o=i.length,s;a<o;++a)(s=i[a])&&e.call(s,s.__data__,a,i);return this}function Se(e){return function(){this.removeAttribute(e);}}function Ce(e){return function(){this.removeAttributeNS(e.space,e.local);}}function we(e,t){return function(){this.setAttribute(e,t);}}function Te(e,t){return function(){this.setAttributeNS(e.space,e.local,t);}}function Ee(e,t){return function(){var n=t.apply(this,arguments);n==null?this.removeAttribute(e):this.setAttribute(e,n);}}function De(e,t){return function(){var n=t.apply(this,arguments);n==null?this.removeAttributeNS(e.space,e.local):this.setAttributeNS(e.space,e.local,n);}}function Oe(e,t){var n=x(e);if(arguments.length<2){var r=this.node();return n.local?r.getAttributeNS(n.space,n.local):r.getAttribute(n)}return this.each((t==null?n.local?Ce:Se:typeof t==`function`?n.local?De:Ee:n.local?Te:we)(n,t))}function U(e){return e.ownerDocument&&e.ownerDocument.defaultView||e.document&&e||e.defaultView}function ke(e){return function(){this.style.removeProperty(e);}}function Ae(e,t,n){return function(){this.style.setProperty(e,t,n);}}function je(e,t,n){return function(){var r=t.apply(this,arguments);r==null?this.style.removeProperty(e):this.style.setProperty(e,r,n);}}function Me(e,t,n){return arguments.length>1?this.each((t==null?ke:typeof t==`function`?je:Ae)(e,t,n??``)):Ne(this.node(),e)}function Ne(e,t){return e.style.getPropertyValue(t)||U(e).getComputedStyle(e,null).getPropertyValue(t)}function Pe(e){return function(){delete this[e];}}function Fe(e,t){return function(){this[e]=t;}}function Ie(e,t){return function(){var n=t.apply(this,arguments);n==null?delete this[e]:this[e]=n;}}function Le(e,t){return arguments.length>1?this.each((t==null?Pe:typeof t==`function`?Ie:Fe)(e,t)):this.node()[e]}function Re(e){return e.trim().split(/^|\s+/)}function W(e){return e.classList||new ze(e)}function ze(e){this._node=e,this._names=Re(e.getAttribute(`class`)||``);}ze.prototype={add:function(e){this._names.indexOf(e)<0&&(this._names.push(e),this._node.setAttribute(`class`,this._names.join(` `)));},remove:function(e){var t=this._names.indexOf(e);t>=0&&(this._names.splice(t,1),this._node.setAttribute(`class`,this._names.join(` `)));},contains:function(e){return this._names.indexOf(e)>=0}};function Be(e,t){for(var n=W(e),r=-1,i=t.length;++r<i;)n.add(t[r]);}function Ve(e,t){for(var n=W(e),r=-1,i=t.length;++r<i;)n.remove(t[r]);}function He(e){return function(){Be(this,e);}}function Ue(e){return function(){Ve(this,e);}}function We(e,t){return function(){(t.apply(this,arguments)?Be:Ve)(this,e);}}function Ge(e,t){var n=Re(e+``);if(arguments.length<2){for(var r=W(this.node()),i=-1,a=n.length;++i<a;)if(!r.contains(n[i]))return  false;return  true}return this.each((typeof t==`function`?We:t?He:Ue)(n,t))}function Ke(){this.textContent=``;}function qe(e){return function(){this.textContent=e;}}function Je(e){return function(){var t=e.apply(this,arguments);this.textContent=t??``;}}function Ye(e){return arguments.length?this.each(e==null?Ke:(typeof e==`function`?Je:qe)(e)):this.node().textContent}function Xe(){this.innerHTML=``;}function Ze(e){return function(){this.innerHTML=e;}}function Qe(e){return function(){var t=e.apply(this,arguments);this.innerHTML=t??``;}}function $e(e){return arguments.length?this.each(e==null?Xe:(typeof e==`function`?Qe:Ze)(e)):this.node().innerHTML}function et(){this.nextSibling&&this.parentNode.appendChild(this);}function tt(){return this.each(et)}function nt(){this.previousSibling&&this.parentNode.insertBefore(this,this.parentNode.firstChild);}function rt(){return this.each(nt)}function it(e){var t=typeof e==`function`?e:w(e);return this.select(function(){return this.appendChild(t.apply(this,arguments))})}function at(){return null}function ot(e,t){var n=typeof e==`function`?e:w(e),r=t==null?at:typeof t==`function`?t:E(t);return this.select(function(){return this.insertBefore(n.apply(this,arguments),r.apply(this,arguments)||null)})}function st(){var e=this.parentNode;e&&e.removeChild(this);}function ct(){return this.each(st)}function lt(){var e=this.cloneNode(false),t=this.parentNode;return t?t.insertBefore(e,this.nextSibling):e}function ut(){var e=this.cloneNode(true),t=this.parentNode;return t?t.insertBefore(e,this.nextSibling):e}function dt(e){return this.select(e?ut:lt)}function ft(e){return arguments.length?this.property(`__data__`,e):this.node().__data__}function pt(e){return function(t){e.call(this,t,this.__data__);}}function mt(e){return e.trim().split(/^|\s+/).map(function(e){var t=``,n=e.indexOf(`.`);return n>=0&&(t=e.slice(n+1),e=e.slice(0,n)),{type:e,name:t}})}function ht(e){return function(){var t=this.__on;if(t){for(var n=0,r=-1,i=t.length,a;n<i;++n)a=t[n],(!e.type||a.type===e.type)&&a.name===e.name?this.removeEventListener(a.type,a.listener,a.options):t[++r]=a;++r?t.length=r:delete this.__on;}}}function gt(e,t,n){return function(){var r=this.__on,i,a=pt(t);if(r){for(var o=0,s=r.length;o<s;++o)if((i=r[o]).type===e.type&&i.name===e.name){this.removeEventListener(i.type,i.listener,i.options),this.addEventListener(i.type,i.listener=a,i.options=n),i.value=t;return}}this.addEventListener(e.type,a,n),i={type:e.type,name:e.name,value:t,listener:a,options:n},r?r.push(i):this.__on=[i];}}function _t(e,t,n){var r=mt(e+``),i,a=r.length,o;if(arguments.length<2){var s=this.node().__on;if(s){for(var c=0,l=s.length,u;c<l;++c)for(i=0,u=s[c];i<a;++i)if((o=r[i]).type===u.type&&o.name===u.name)return u.value}return}for(s=t?gt:ht,i=0;i<a;++i)this.each(s(r[i],t,n));return this}function vt(e,t,n){var r=U(e),i=r.CustomEvent;typeof i==`function`?i=new i(t,n):(i=r.document.createEvent(`Event`),n?(i.initEvent(t,n.bubbles,n.cancelable),i.detail=n.detail):i.initEvent(t,false,false)),e.dispatchEvent(i);}function yt(e,t){return function(){return vt(this,e,t)}}function bt(e,t){return function(){return vt(this,e,t.apply(this,arguments))}}function xt(e,t){return this.each((typeof t==`function`?bt:yt)(e,t))}function*St(){for(var e=this._groups,t=0,n=e.length;t<n;++t)for(var r=e[t],i=0,a=r.length,o;i<a;++i)(o=r[i])&&(yield o);}function G(e,t){this._groups=e,this._parents=t;}function Tt(){return this}G.prototype={constructor:G,select:D,selectAll:j,selectChild:L,selectChildren:re,filter:z,data:ce,enter:V,exit:ue,join:de,merge:fe,selection:Tt,order:pe,sort:me,call:ge,nodes:_e,node:ve,size:ye,empty:be,each:xe,attr:Oe,style:Me,property:Le,classed:Ge,text:Ye,html:$e,raise:tt,lower:rt,append:it,insert:ot,remove:ct,clone:dt,datum:ft,on:_t,dispatch:xt,[Symbol.iterator]:St};function Q(e,t){e.component(e=>{let {position:r=.5,disabled:i=false,slider_color:a=`var(--border-color-primary)`,image_size:o={top:0,left:0,width:0,height:0},el:s=void 0,parent_el:c=void 0,onposition:l,children:u}=t,d=0,f=false;e.push(`<div class="wrap svelte-b2bl92" role="none"><div class="content svelte-b2bl92">`),u?(e.push(`<!--[0-->`),u(e),e.push(`<!---->`)):e.push(`<!--[-1-->`),e.push(`<!--]--></div> <div${attr_class(`outer svelte-b2bl92`,void 0,{disabled:i,grab:f})} data-testid="slider" role="none"${attr_style(`transform: translateX(${stringify(d)}px)`)}><span${attr_class(`icon-wrap svelte-b2bl92`,void 0,{active:f,disabled:i})}><span class="icon left svelte-b2bl92">◢</span><span class="icon center svelte-b2bl92"${attr_style(``,{"--color":a})}></span><span class="icon right svelte-b2bl92">◢</span></span> <div class="inner svelte-b2bl92"${attr_style(``,{"--color":a})}></div></div></div>`),bind_props(t,{position:r,el:s,parent_el:c});});}function $(e,t){e.component(e=>{let{src:n=void 0,fullscreen:r=false,fixed:i=false,transform:a=`translate(0px, 0px) scale(1)`,img_el:o=void 0,hidden:s=false,variant:c=`upload`,max_height:l=500,onload:u,$$slots:d,$$events:f,...p}=t;e.push(`<img${attributes({src:n,"data-testid":`imageslider-image`,...p},`svelte-j3ek2n`,{fixed:i,hidden:s,preview:c===`preview`,slider:c===`upload`,fullscreen:r,small:!r},{transform:a,"max-height":l&&!r?`${l}px`:null})} onload="this.__e=event" onerror="this.__e=event"/>`),bind_props(t,{img_el:o});});}function It(e,t){e.component(e=>{var l;let{value:d=[null,null],label:m=void 0,show_download_button:h=true,show_label:_$1,i18n:b,position:x=.5,onposition:S,layer_images:C=true,show_single:w=false,slider_color:T,show_fullscreen_button:E=true,fullscreen:D=false,buttons:ee=null,on_custom_button_click:O=null,el_width:k=0,max_height:A,interactive:j=true,onclear:M,onfullscreen:N}=t,P,F,I=tweened({x:0,y:0,z:1},{duration:75}),L,R={top:0,left:0,width:0,height:0},te=derived(()=>re(x,0,R.width,R.left,store_get(l??={},`$transform`,I).x,store_get(l??={},`$transform`,I).z)),ne=derived(()=>C?`clip-path: inset(0 0 0 ${te()*100}%)`:``);function re(e,t,n,r,i,a){return (e*n+r-i)/a/t}function z(e){R=e;}let B=true,V;function H(e){h$1(e,{show_label:_$1,Icon:H$1,label:m||b(`image.image`)}),e.push(`<!----> `),(d===null||d[0]===null||d[1]===null)&&!w?(e.push(`<!--[0-->`),v(e,{unpadded_box:true,size:`large`,children:e=>{H$1(e);}})):(e.push(`<!--[-1-->`),e.push(`<div class="image-container svelte-1880bc6">`),Ve$1(e,{buttons:ee,on_custom_button_click:O,children:e=>{_(e,{Icon:Se$1,label:b(`common.undo`),disabled:store_get(l??={},`$transform`,I).z===1,onclick:()=>void 0}),e.push(`<!----> `),E?(e.push(`<!--[0-->`),He$1(e,{fullscreen:D,onclick:e=>{D=e,N?.(e);}})):e.push(`<!--[-1-->`),e.push(`<!--]--> `),h?(e.push(`<!--[0-->`),g(e,{href:d[1]?.url,download:d[1]?.orig_name||`image`,children:e=>{_(e,{Icon:ie$1,label:b(`common.download`)});},$$slots:{default:true}})):e.push(`<!--[-1-->`),e.push(`<!--]--> `),j?(e.push(`<!--[0-->`),_(e,{Icon:ee$1,label:`Remove Image`,onclick:e=>{d=[null,null],M?.(),e.stopPropagation();}})):e.push(`<!--[-1-->`),e.push(`<!--]-->`);}}),e.push(`<!----> <div${attr_class(`slider-wrap svelte-1880bc6`,void 0,{limit_height:!D})}>`),Q(e,{onposition:S,slider_color:T,image_size:R,get position(){return x},set position(e){x=e,B=false;},get el(){return F},set el(e){F=e,B=false;},get parent_el(){return L},set parent_el(e){L=e,B=false;},children:e=>{$(e,{src:d?.[0]?.url,alt:``,loading:`lazy`,variant:`preview`,transform:`translate(${stringify(store_get(l??={},`$transform`,I).x)}px, ${stringify(store_get(l??={},`$transform`,I).y)}px) scale(${stringify(store_get(l??={},`$transform`,I).z)})`,fullscreen:D,max_height:A,onload:z,get img_el(){return P},set img_el(e){P=e,B=false;}}),e.push(`<!----> `),$(e,{variant:`preview`,fixed:C,hidden:!d?.[1]?.url,src:d?.[1]?.url,alt:``,loading:`lazy`,style:`${ne()}; background: var(--block-background-fill);`,transform:`translate(${stringify(store_get(l??={},`$transform`,I).x)}px, ${stringify(store_get(l??={},`$transform`,I).y)}px) scale(${stringify(store_get(l??={},`$transform`,I).z)})`,fullscreen:D,max_height:A,onload:z}),e.push(`<!---->`);},$$slots:{default:true}}),e.push(`<!----></div></div>`)),e.push(`<!--]-->`);}do B=true,V=e.copy(),H(V);while(!B);e.subsume(V),l&&unsubscribe_stores(l),bind_props(t,{value:d,position:x,fullscreen:D,el_width:k});});}function Lt(e,t){e.component(e=>{let{onremove_image:n}=t;e.push(`<div class="svelte-2ufkjh">`),_(e,{Icon:ee$1,label:`Remove Image`,onclick:e=>{n?.(),e.stopPropagation();}}),e.push(`<!----></div>`);});}function Rt(e,t){e.component(e=>{let{value:a=[null,null],label:s=void 0,show_label:l,root:u,position:d=.5,onposition:f,upload_count:h=2,show_download_button:g$1=true,slider_color:y,upload:b,stream_handler:x,max_file_size:S=null,i18n:C,max_height:w,upload_promise:T=void 0,dragging:E=false,onclear:D,ondrag:ee,onupload:O,children:k}=t,A=a||[null,null],j;async function M(e,t){let n=Array.isArray(e)?e:[e],r=[a[0],a[1]];n.length>1?r[t]=n[0]:r[t]=n[t],a=r,await tick(),O?.(r);}let N=true,P;function F(e){h$1(e,{show_label:l,Icon:H$1,label:s||C(`image.image`)}),e.push(`<!----> <div data-testid="image" class="image-container svelte-1c8zs50">`),a?.[0]?.url||a?.[1]?.url?(e.push(`<!--[0-->`),Lt(e,{onremove_image:()=>{d=.5,a=[null,null],D?.();}})):e.push(`<!--[-1-->`),e.push(`<!--]--> `),a?.[1]?.url?(e.push(`<!--[0-->`),e.push(`<div class="icon-buttons svelte-1c8zs50">`),g$1?(e.push(`<!--[0-->`),g(e,{href:a[1].url,download:a[1].orig_name||`image`,children:e=>{_(e,{Icon:ie$1});},$$slots:{default:true}})):e.push(`<!--[-1-->`),e.push(`<!--]--></div>`)):e.push(`<!--[-1-->`),e.push(`<!--]--> `),Q(e,{onposition:f,disabled:h==2||!a?.[0],slider_color:y,get position(){return d},set position(e){d=e,N=false;},children:e=>{e.push(`<div${attr_class(`upload-wrap svelte-1c8zs50`,void 0,{"side-by-side":h===2})}${attr_style(``,{display:h===2?`flex`:`block`})}>`),A?.[0]?(e.push(`<!--[-1-->`),$(e,{variant:`upload`,src:A[0]?.url,alt:``,max_height:w,get img_el(){return j},set img_el(e){j=e,N=false;}})):(e.push(`<!--[0-->`),e.push(`<div${attr_class(`wrap svelte-1c8zs50`,void 0,{"half-wrap":h===1})}>`),v$1(e,{filetype:`image/*`,onload:e=>M(e,0),disable_click:!!a?.[0],root:u,file_count:`multiple`,upload:b,stream_handler:x,max_file_size:S,get upload_promise(){return T},set upload_promise(e){T=e,N=false;},get dragging(){return E},set dragging(e){E=e,N=false;},children:e=>{k?(e.push(`<!--[0-->`),k(e),e.push(`<!---->`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);},$$slots:{default:true}}),e.push(`<!----></div>`)),e.push(`<!--]--> `),!A?.[1]&&h===2?(e.push(`<!--[0-->`),v$1(e,{filetype:`image/*`,onload:e=>M(e,1),disable_click:!!a?.[1],root:u,file_count:`multiple`,upload:b,stream_handler:x,max_file_size:S,get upload_promise(){return T},set upload_promise(e){T=e,N=false;},get dragging(){return E},set dragging(e){E=e,N=false;},children:e=>{k?(e.push(`<!--[0-->`),k(e),e.push(`<!---->`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);},$$slots:{default:true}})):!A?.[1]&&h===1?(e.push(`<!--[1-->`),e.push(`<div${attr_class(`empty-wrap fixed svelte-1c8zs50`,void 0,{"white-icon":!a?.[0]?.url})}${attr_style(``,{width:`${stringify(0*(1-d))}px`,transform:`translateX(${stringify(0*d)}px)`})}>`),v(e,{unpadded_box:true,size:`large`,children:e=>{H$1(e);}}),e.push(`<!----></div>`)):A?.[1]?(e.push(`<!--[2-->`),$(e,{variant:`upload`,src:A[1].url,alt:``,fixed:h===1,transform:`translate(0px, 0px) scale(1)`,max_height:w})):e.push(`<!--[-1-->`),e.push(`<!--]--></div>`);},$$slots:{default:true}}),e.push(`<!----></div>`);}do N=true,P=e.copy(),F(P);while(!N);e.subsume(P),bind_props(t,{value:a,position:d,upload_promise:T,dragging:E});});}function zt(e,t){e.component(e=>{let{value:n=[null,null],upload:r,stream_handler:i,label:a,show_label:o,i18n:s,root:c,upload_count:l=1,dragging:u=false,max_height:d,max_file_size:f=null,upload_promise:p=void 0,onclear:m,ondrag:h,onupload:g,onposition:_,position:y=.5,children:b}=t,x=true,S;function C(e){Rt(e,{slider_color:`var(--border-color-primary)`,position:y,onposition:_,root:c,onclear:m,ondrag:e=>{u=e,h?.(e);},onupload:g,label:a,show_label:o,upload_count:l,stream_handler:i,upload:r,max_file_size:f,max_height:d,i18n:s,get upload_promise(){return p},set upload_promise(e){p=e,x=false;},get value(){return n},set value(e){n=e,x=false;},get dragging(){return u},set dragging(e){u=e,x=false;},children:e=>{b?(e.push(`<!--[0-->`),b(e),e.push(`<!---->`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);},$$slots:{default:true}});}do x=true,S=e.copy(),C(S);while(!x);e.subsume(S),bind_props(t,{value:n,dragging:u,upload_promise:p});});}function Bt(n,r){n.component(n=>{let i;class a extends h{async get_data(){return i&&(await i,await tick()),await super.get_data()}}let{$$slots:o,$$events:s,...c}=r,u=new a(c),f=false,p$1=false,m=u.props.value??[null,null],h$1=derived(()=>Math.max(0,Math.min(100,u.props.slider_position))/100);function g(e){let t=Math.max(0,Math.min(100,Math.round(e*1e5)/1e3));u.props.slider_position!==t&&(u.props.slider_position=t);}u.watch_for_change();let y=true,b;function x(e){!u.shared.interactive||m?.[1]&&m?.[0]?(e.push(`<!--[0-->`),n$1(e,{visible:u.shared.visible,variant:`solid`,border_mode:p$1?`focus`:`base`,padding:false,elem_id:u.shared.elem_id,elem_classes:u.shared.elem_classes,height:u.props.height||void 0,width:u.props.width,allow_overflow:false,container:u.shared.container,scale:u.shared.scale,min_width:u.shared.min_width,get fullscreen(){return f},set fullscreen(e){f=e,y=false;},children:e=>{p(e,spread_props([{autoscroll:u.shared.autoscroll,i18n:u.i18n},u.shared.loading_status])),e.push(`<!----> `),It(e,{onclear:()=>{u.dispatch(`clear`),u.dispatch(`input`);},onfullscreen:e=>{f=e;},fullscreen:f,interactive:u.shared.interactive,label:u.shared.label,show_label:u.shared.show_label,show_download_button:u.props.buttons.some(e=>typeof e==`string`&&e===`download`),i18n:u.i18n,show_fullscreen_button:u.props.buttons.some(e=>typeof e==`string`&&e===`fullscreen`),buttons:u.props.buttons,on_custom_button_click:e=>{u.dispatch(`custom_button_click`,{id:e});},position:h$1(),onposition:g,slider_color:u.props.slider_color,max_height:u.props.max_height,get value(){return m},set value(e){m=e,y=false;}}),e.push(`<!---->`);},$$slots:{default:true}})):(e.push(`<!--[-1-->`),n$1(e,{visible:u.shared.visible,variant:m?.[0]||m?.[1]?`solid`:`dashed`,border_mode:p$1?`focus`:`base`,padding:false,elem_id:u.shared.elem_id,elem_classes:u.shared.elem_classes,height:u.props.height||void 0,width:u.props.width,allow_overflow:false,container:u.shared.container,scale:u.shared.scale,min_width:u.shared.min_width,children:e=>{p(e,spread_props([{autoscroll:u.shared.autoscroll,i18n:u.i18n},u.shared.loading_status,{on_clear_status:()=>u.dispatch(`clear_status`,u.shared.loading_status)}])),e.push(`<!----> `),zt(e,{root:u.shared.root,onclear:()=>{u.dispatch(`clear`),u.dispatch(`input`);},ondrag:e=>p$1=e,onupload:()=>{u.dispatch(`upload`),u.dispatch(`input`);},label:u.shared.label,show_label:u.shared.show_label,upload_count:u.props.upload_count,position:h$1(),onposition:g,max_file_size:u.shared.max_file_size,i18n:u.i18n,upload:(...e)=>u.shared.client.upload(...e),stream_handler:u.shared.client?.stream,max_height:u.props.max_height,get upload_promise(){return i},set upload_promise(e){i=e,y=false;},get value(){return m},set value(e){m=e,y=false;},get dragging(){return p$1},set dragging(e){p$1=e,y=false;},children:e=>{e.push(`<!--[0-->`),ze$1(e,{i18n:u.i18n,type:`image`,placeholder:u.props.placeholder}),e.push(`<!--]-->`);},$$slots:{default:true}}),e.push(`<!---->`);},$$slots:{default:true}})),e.push(`<!--]-->`);}do y=true,b=n.copy(),x(b);while(!y);n.subsume(b);});}

export { Bt as default };
//# sourceMappingURL=Index39-DW9lx6LQ.js.map
