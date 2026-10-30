import { _ as _$1, a4 as Oe, a5 as B, a6 as V, a7 as I, a8 as O, m as ee } from './src3-C1zEyqUA.js';
import { u as ur } from './server-Ck07o0bn.js';
import { M as onDestroy } from './2-C9eYyMgE.js';
import './async-Cv1-GZGV.js';
import { n as noop, w as writable, e as escape_html, b as attr_style, i as stringify, o as store_get, h as ensure_array_like, u as unsubscribe_stores, a as attr_class, f as attr, d as derived } from './renderer-RGQlaTg4.js';
import { h as html } from './html-CfyvkLET.js';

/** @import { Raf } from '#client' */

const now = () => Date.now();

/** @type {Raf} */
const raf = {
	// don't access requestAnimationFrame eagerly outside method
	// this allows basic testing of user code without JSDOM
	// bunder will eval and remove ternary when the user's app is built
	tick: /** @param {any} _ */ (_) => (noop)(),
	now: () => now(),
	tasks: new Set()
};

/** @import { TaskCallback, Task, TaskEntry } from '#client' */

/**
 * Creates a new task that runs on each raf frame
 * until it returns a falsy value or is aborted
 * @param {TaskCallback} callback
 * @returns {Task}
 */
function loop(callback) {
	/** @type {TaskEntry} */
	let task;

	if (raf.tasks.size === 0) ;

	return {
		promise: new Promise((fulfill) => {
			raf.tasks.add((task = { c: callback, f: fulfill }));
		}),
		abort() {
			raf.tasks.delete(task);
		}
	};
}

/**
 * @param {any} obj
 * @returns {obj is Date}
 */
function is_date(obj) {
	return Object.prototype.toString.call(obj) === '[object Date]';
}

/** @import { Task } from '#client' */
/** @import { TickContext } from './private.js' */
/** @import { Spring as SpringStore, SpringOptions, SpringUpdateOptions } from './public.js' */

/**
 * @template T
 * @param {TickContext} ctx
 * @param {T} last_value
 * @param {T} current_value
 * @param {T} target_value
 * @returns {T}
 */
function tick_spring(ctx, last_value, current_value, target_value) {
	if (typeof current_value === 'number' || is_date(current_value)) {
		// @ts-ignore
		const delta = target_value - current_value;
		// @ts-ignore
		const velocity = (current_value - last_value) / (ctx.dt || 1 / 60); // guard div by 0
		const spring = ctx.opts.stiffness * delta;
		const damper = ctx.opts.damping * velocity;
		const acceleration = (spring - damper) * ctx.inv_mass;
		const d = (velocity + acceleration) * ctx.dt;
		if (Math.abs(d) < ctx.opts.precision && Math.abs(delta) < ctx.opts.precision) {
			return target_value; // settled
		} else {
			ctx.settled = false; // signal loop to keep ticking
			// @ts-ignore
			return is_date(current_value) ? new Date(current_value.getTime() + d) : current_value + d;
		}
	} else if (Array.isArray(current_value)) {
		// @ts-ignore
		return current_value.map((_, i) =>
			// @ts-ignore
			tick_spring(ctx, last_value[i], current_value[i], target_value[i])
		);
	} else if (typeof current_value === 'object') {
		const next_value = {};
		for (const k in current_value) {
			// @ts-ignore
			next_value[k] = tick_spring(ctx, last_value[k], current_value[k], target_value[k]);
		}
		// @ts-ignore
		return next_value;
	} else {
		throw new Error(`Cannot spring ${typeof current_value} values`);
	}
}

/**
 * The spring function in Svelte creates a store whose value is animated, with a motion that simulates the behavior of a spring. This means when the value changes, instead of transitioning at a steady rate, it "bounces" like a spring would, depending on the physics parameters provided. This adds a level of realism to the transitions and can enhance the user experience.
 *
 * @deprecated Use [`Spring`](https://svelte.dev/docs/svelte/svelte-motion#Spring) instead
 * @template [T=any]
 * @param {T} [value]
 * @param {SpringOptions} [opts]
 * @returns {SpringStore<T>}
 */
function spring(value, opts = {}) {
	const store = writable(value);
	const { stiffness = 0.15, damping = 0.8, precision = 0.01 } = opts;
	/** @type {number} */
	let last_time;
	/** @type {Task | null} */
	let task;
	/** @type {object} */
	let current_token;

	let last_value = /** @type {T} */ (value);
	let target_value = /** @type {T | undefined} */ (value);

	let inv_mass = 1;
	let inv_mass_recovery_rate = 0;
	let cancel_task = false;
	/**
	 * @param {T} new_value
	 * @param {SpringUpdateOptions} opts
	 * @returns {Promise<void>}
	 */
	function set(new_value, opts = {}) {
		target_value = new_value;
		const token = (current_token = {});
		if (value == null || opts.hard || (spring.stiffness >= 1 && spring.damping >= 1)) {
			cancel_task = true; // cancel any running animation
			last_time = raf.now();
			last_value = new_value;
			store.set((value = target_value));
			return Promise.resolve();
		} else if (opts.soft) {
			const rate = opts.soft === true ? 0.5 : +opts.soft;
			inv_mass_recovery_rate = 1 / (rate * 60);
			inv_mass = 0; // infinite mass, unaffected by spring forces
		}
		if (!task) {
			last_time = raf.now();
			cancel_task = false;
			task = loop((now) => {
				if (cancel_task) {
					cancel_task = false;
					task = null;
					return false;
				}
				inv_mass = Math.min(inv_mass + inv_mass_recovery_rate, 1);

				// clamp elapsed time to 1/30th of a second, so that longer pauses
				// (blocked thread or inactive tab) don't cause the spring to go haywire
				const elapsed = Math.min(now - last_time, 1000 / 30);

				/** @type {TickContext} */
				const ctx = {
					inv_mass,
					opts: spring,
					settled: true,
					dt: (elapsed * 60) / 1000
				};
				// @ts-ignore
				const next_value = tick_spring(ctx, last_value, value, target_value);
				last_time = now;
				last_value = /** @type {T} */ (value);
				store.set((value = /** @type {T} */ (next_value)));
				if (ctx.settled) {
					task = null;
				}
				return !ctx.settled;
			});
		}
		return new Promise((fulfil) => {
			/** @type {Task} */ (task).promise.then(() => {
				if (token === current_token) fulfil();
			});
		});
	}
	/** @type {SpringStore<T>} */
	// @ts-expect-error - class-only properties are missing
	const spring = {
		set,
		update: (fn, opts) => set(fn(/** @type {T} */ (target_value), /** @type {T} */ (value)), opts),
		subscribe: store.subscribe,
		stiffness,
		damping,
		precision
	};
	return spring;
}

function d(e,t){e.component(e=>{var n;let{margin:r=true}=t,i=spring([0,0]),a=spring([0,0]);e.push(`<div${attr_class(`svelte-1vhirvf`,void 0,{margin:r})}><svg viewBox="-1200 -1200 3000 3000" fill="none" xmlns="http://www.w3.org/2000/svg" class="svelte-1vhirvf"><g${attr_style(`transform: translate(${stringify(store_get(n??={},`$top`,i)[0])}px, ${stringify(store_get(n??={},`$top`,i)[1])}px);`)}><path d="M255.926 0.754768L509.702 139.936V221.027L255.926 81.8465V0.754768Z" fill="#FF7C00" fill-opacity="0.4" class="svelte-1vhirvf"></path><path d="M509.69 139.936L254.981 279.641V361.255L509.69 221.55V139.936Z" fill="#FF7C00" class="svelte-1vhirvf"></path><path d="M0.250138 139.937L254.981 279.641V361.255L0.250138 221.55V139.937Z" fill="#FF7C00" fill-opacity="0.4" class="svelte-1vhirvf"></path><path d="M255.923 0.232622L0.236328 139.936V221.55L255.923 81.8469V0.232622Z" fill="#FF7C00" class="svelte-1vhirvf"></path></g><g${attr_style(`transform: translate(${stringify(store_get(n??={},`$bottom`,a)[0])}px, ${stringify(store_get(n??={},`$bottom`,a)[1])}px);`)}><path d="M255.926 141.5L509.702 280.681V361.773L255.926 222.592V141.5Z" fill="#FF7C00" fill-opacity="0.4" class="svelte-1vhirvf"></path><path d="M509.69 280.679L254.981 420.384V501.998L509.69 362.293V280.679Z" fill="#FF7C00" class="svelte-1vhirvf"></path><path d="M0.250138 280.681L254.981 420.386V502L0.250138 362.295V280.681Z" fill="#FF7C00" fill-opacity="0.4" class="svelte-1vhirvf"></path><path d="M255.923 140.977L0.236328 280.68V362.294L255.923 222.591V140.977Z" fill="#FF7C00" class="svelte-1vhirvf"></path></g></svg></div>`),n&&unsubscribe_stores(n);});}function f(e){let t=[``,`k`,`M`,`G`,`T`,`P`,`E`,`Z`],n=0;for(;e>1e3&&n<t.length-1;)e/=1e3,n++;let r=t[n];return (Number.isInteger(e)?e:e.toFixed(1))+r}function p(e,t){e.component(e=>{let{i18n:n,eta:i=null,queue_position:a,queue_size:s,component_id:c=null,fn_index:u=null,status:p,scroll_to_output:m=false,timer:h=true,show_progress:g=`full`,message:_=null,progress:v=null,time_start:y=null,eta_total:b=null,variant:x=`default`,loading_text:S=`Loading...`,absolute:C=true,translucent:w=false,border:T=false,autoscroll:E,validation_error:D=null,show_validation_error:O=true,type:k=null,on_clear_status:A,used_cache:j=null,cache_duration:M=null,avg_time:N=null,cache_event_id:P=null,additional_loading_text:F,error_details:I}=t,L=derived(()=>!(O&&D)&&(k===`input`||!p||p===`complete`||g===`hidden`||p==`streaming`)),R=derived(()=>0),z=derived(()=>0 .toFixed(1)),B=derived(()=>v==null);let V=derived(()=>{let e=null;e=v==null?null:v.map(e=>{if(e.index!=null&&e.length!=null)return e.index/e.length;if(e.progress!=null)return e.progress});let t,n=``;return e?(t=e[e.length-1],n=t===0?`0`:`150ms`):t=void 0,{progress_level:e,last_progress_level:t,progress_bar_transition:n}}),H=derived(()=>{if(V().last_progress_level==null)return null;let e=Math.round(Math.min(Math.max(V().last_progress_level,0),1)*100),t=`${e}%`,n=v?.[v.length-1];return n?.index!=null&&n?.length!=null&&(t=`${f(n.index)} / ${f(n.length)}`,n.unit&&(t+=` ${n.unit}`)),n?.desc&&(t=`${n.desc}: ${t}`),{valuenow:e,valuetext:t}});if(e.push(`<div${attr_class(`wrap ${stringify(x)} ${stringify(g)}`,`svelte-1uj8rng`,{"no-click":D&&O,hide:L(),translucent:x===`center`&&(p===`pending`||p===`error`)||w||g===`minimal`||D,generating:p===`generating`&&g===`full`,border:T})} data-testid="status-tracker"${attr_style(``,{position:C?`absolute`:`static`,padding:C?`0`:`var(--size-8) 0`})}>`),D&&O?(e.push(`<!--[0-->`),e.push(`<div class="validation-error svelte-1uj8rng">${escape_html(D)} <button class="svelte-1uj8rng">`),_$1(e,{Icon:ee,label:n?n(`common.clear`):`Clear`,disabled:false,size:`x-small`,background:`var(--background-fill-primary)`,color:`var(--error-background-text)`,border:`var(--border-color-primary)`,onclick:()=>D=null}),e.push(`<!----></button></div>`)):e.push(`<!--[-1-->`),e.push(`<!--]--> `),p===`pending`){if(e.push(`<!--[0-->`),x==="default"&&B()&&g===`full`?(e.push(`<!--[0-->`),e.push(`<div class="eta-bar svelte-1uj8rng"${attr_style(``,{transform:`translateX(${stringify((R()||0)*100-100)}%)`})}></div>`)):e.push(`<!--[-1-->`),e.push(`<!--]--> <div${attr_class(`progress-text svelte-1uj8rng`,void 0,{"meta-text-center":x===`center`,"meta-text":x==="default"})}>`),v){e.push(`<!--[0-->`),e.push(`<!--[-->`);let t=ensure_array_like(v);for(let n=0,r=t.length;n<r;n++){let r=t[n];r.index==null?e.push(`<!--[-1-->`):(e.push(`<!--[0-->`),r.length==null?(e.push(`<!--[-1-->`),e.push(`${escape_html(f(r.index||0))}`)):(e.push(`<!--[0-->`),e.push(`${escape_html(f(r.index||0))}/${escape_html(f(r.length))}`)),e.push(`<!--]--> ${escape_html(r.unit)} |  `)),e.push(`<!--]-->`);}e.push(`<!--]-->`);}else a!==null&&s!==void 0&&a>=0?(e.push(`<!--[1-->`),e.push(`queue: ${escape_html(a+1)}/${escape_html(s)} |`)):a===0?(e.push(`<!--[2-->`),e.push(`processing |`)):e.push(`<!--[-1-->`);if(e.push(`<!--]--> `),h?(e.push(`<!--[0-->`),e.push(`${escape_html(z())}${escape_html(i?`/null`:``)}s`)):e.push(`<!--[-1-->`),e.push(`<!--]--></div> `),V().last_progress_level!=null){if(e.push(`<!--[0-->`),e.push(`<div class="progress-level svelte-1uj8rng"><div class="progress-level-inner svelte-1uj8rng">`),v!=null){e.push(`<!--[0-->`),e.push(`<!--[-->`);let t=ensure_array_like(v);for(let n=0,r=t.length;n<r;n++){let r=t[n];r.desc!=null||V().progress_level&&V().progress_level[n]!=null?(e.push(`<!--[0-->`),n===0?e.push(`<!--[-1-->`):(e.push(`<!--[0-->`),e.push(`\xA0/`)),e.push(`<!--]--> `),r.desc==null?e.push(`<!--[-1-->`):(e.push(`<!--[0-->`),e.push(`${escape_html(r.desc)}`)),e.push(`<!--]--> `),r.desc!=null&&V().progress_level&&V().progress_level[n]!=null?(e.push(`<!--[0-->`),e.push(`-`)):e.push(`<!--[-1-->`),e.push(`<!--]--> `),V().progress_level==null?e.push(`<!--[-1-->`):(e.push(`<!--[0-->`),e.push(`${escape_html((100*(V().progress_level[n]||0)).toFixed(1))}%`)),e.push(`<!--]-->`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);}e.push(`<!--]-->`);}else e.push(`<!--[-1-->`);e.push(`<!--]--></div> <div class="progress-bar-wrap svelte-1uj8rng" role="progressbar"${attr(`aria-label`,v?.[v.length-1]?.desc||n(`common.progress`))} aria-valuemin="0" aria-valuemax="100"${attr(`aria-valuenow`,H()?.valuenow)}${attr(`aria-valuetext`,H()?.valuetext)}><div class="progress-bar svelte-1uj8rng"${attr_style(``,{width:`${stringify(V().last_progress_level*100)}%`,transition:V().progress_bar_transition})}></div></div></div>`);}else g===`full`?(e.push(`<!--[1-->`),d(e,{margin:x==="default"})):e.push(`<!--[-1-->`);e.push(`<!--]--> `),h?e.push(`<!--[-1-->`):(e.push(`<!--[0-->`),e.push(`<p class="loading svelte-1uj8rng">${escape_html(S)}</p> `),F?.(e),e.push(`<!---->`)),e.push(`<!--]-->`);}else p===`error`?(e.push(`<!--[1-->`),e.push(`<div class="clear-status svelte-1uj8rng">`),_$1(e,{Icon:ee,label:n(`common.clear`),disabled:false,onclick:()=>{A?.();}}),e.push(`<!----></div> <span class="error svelte-1uj8rng">${escape_html(n(`common.error`))}</span> `),I?.(e),e.push(`<!---->`)):e.push(`<!--[-1-->`);e.push(`<!--]--></div> `),e.push(`<!--[-1-->`),e.push(`<!--]-->`);});}function m(e,t){e.component(e=>{let{status:n,i18n:r}=t;onDestroy(()=>{}),e.push(`<div class="sr-only" role="status" aria-live="polite" aria-atomic="true">${escape_html(``)}</div>`);});}function h(r,o){r.component(r=>{let{type:c,messages:u=[],expanded:d=true}=o,m={error:`An error occurred.`,warning:`Warning.`,success:`Success.`,info:`Info.`},h=derived(()=>u.length),g=derived(()=>u[0]),_=derived(()=>c.charAt(0).toUpperCase()+c.slice(1)),v=derived(()=>g()?.duration!==null),y=derived(()=>v()?`${g().duration}s`:`0s`);if(r.push(`<div${attr_class(`toast-body ${stringify(c)}`,`svelte-irmu64`)} role="status" aria-live="polite" data-testid="toast-body"${attr_style(`transform: translateX(${stringify(0)}px); opacity: ${stringify(1-0/300)};`)}><div class="toast-header svelte-irmu64" role="button" tabindex="0"><div${attr_class(`toast-icon ${stringify(c)}`,`svelte-irmu64`)}>`),c===`warning`?(r.push(`<!--[0-->`),Oe(r)):c===`info`?(r.push(`<!--[1-->`),B(r)):c===`success`?(r.push(`<!--[2-->`),V(r)):c===`error`?(r.push(`<!--[3-->`),I(r)):r.push(`<!--[-1-->`),r.push(`<!--]--></div> <div class="toast-title-row svelte-irmu64"><span${attr_class(`toast-title ${stringify(c)}`,`svelte-irmu64`)}>${escape_html(_())} `),h()>1?(r.push(`<!--[0-->`),r.push(`<span class="toast-count svelte-irmu64">(${escape_html(h())})</span>`)):r.push(`<!--[-1-->`),r.push(`<!--]--></span> <div${attr_class(`chevron svelte-irmu64`,void 0,{expanded:d,visible:h()>0})}>`),O(r),r.push(`<!----></div></div> <button${attr_class(`toast-close ${stringify(c)}`,`svelte-irmu64`)} type="button" aria-label="Close" data-testid="toast-close"><span aria-hidden="true">×</span></button></div> `),d){r.push(`<!--[0-->`),r.push(`<div class="toast-messages svelte-irmu64"><!--[-->`);let e=ensure_array_like(u);for(let t=0,n=e.length;t<n;t++){let n=e[t];r.push(`<div${attr_class(`toast-message-item ${stringify(c)}`,`svelte-irmu64`)}><div${attr_class(`toast-message-text ${stringify(c)}`,`svelte-irmu64`)} data-testid="toast-text">${html(ur(n.message||m[c]))}</div></div> `),t<u.length-1?(r.push(`<!--[0-->`),r.push(`<div class="toast-separator svelte-irmu64"></div>`)):r.push(`<!--[-1-->`),r.push(`<!--]-->`);}r.push(`<!--]--></div>`);}else r.push(`<!--[-1-->`);r.push(`<!--]--> `),v()?(r.push(`<!--[0-->`),r.push(`<div${attr_class(`timer ${stringify(c)}`,`svelte-irmu64`)}${attr_style(`animation-duration: ${y()}`)}></div>`)):r.push(`<!--[-1-->`),r.push(`<!--]--></div>`);});}function g(e,t){e.component(e=>{var n;let{messages:r=[],on_close:i}=t,a=spring(0,{stiffness:.4,damping:.5}),o=[];e.push(`<div class="toast-wrap svelte-1qhecvt"${attr_style(`--toast-top: ${stringify(store_get(n??={},`$top`,a))}px;`)}><!--[-->`);let c=ensure_array_like(o);for(let t=0,n=c.length;t<n;t++){let n=c[t];e.push(`<div class="toast-item svelte-1qhecvt">`),h(e,{type:n.type,messages:n.messages,expanded:n.expanded}),e.push(`<!----></div>`);}e.push(`<!--]--></div>`),n&&unsubscribe_stores(n);});}function _(e,t){let{time_limit:n}=t;n?(e.push(`<!--[0-->`),e.push(`<div class="streaming-bar svelte-1au5sp1"${attr_style(``,{"animation-duration":`${stringify(n)}s`})}></div>`)):e.push(`<!--[-1-->`),e.push(`<!--]-->`);}var v=class{current={};fn_outputs={};fn_inputs={};pending_outputs=new Map;fn_status={};show_progress={};cache_event_id=0;register(e,t,n,r){this.fn_outputs[e]=t,this.fn_inputs[e]=n,this.show_progress[e]=r;}remap_ids(e,t,n,r,i){let a=(e,t)=>{let n=Math.min(e.length,t.length);for(let r=0;r<n;r++){let n=e[r],i=t[r];n!==i&&(n in this.current&&(this.current[i]={...this.current[n],component_id:i},delete this.current[n]),this.pending_outputs.has(n)&&(this.pending_outputs.set(i,this.pending_outputs.get(n)),this.pending_outputs.delete(n)));}for(let n=t.length;n<e.length;n++)delete this.current[e[n]],this.pending_outputs.delete(e[n]);};a(t,n),a(r,i),this.fn_outputs[e]=n,this.fn_inputs[e]=i;}clear(e){e in this.current&&(this.current[e]={});}update(e){for(let[t,n]of Object.entries(this.current))n.fn_index!==e.fn_index&&(this.current[t]={...n,used_cache:null,cache_duration:null,avg_time:null,cache_event_id:null});let t=e.used_cache?++this.cache_event_id:null;this.resolve_args(e).forEach(({id:n,queue_position:r,queue_size:i,eta:a,status:o,message:s,progress:c,stream_state:l,time_limit:u,type:d,used_cache:f,cache_duration:p,avg_time:m})=>{let h=this.current[n],g=o===`pending`?h?.time_start??performance.now():null,_=null;o===`pending`&&g!=null&&(_=a!=null&&(h?.eta_total==null||h.eta!==a)?(performance.now()-g)/1e3+a:h?.eta_total??null),this.current[n]={queue:e.queue||false,queue_size:i,queue_position:r,eta:a,component_id:Number(n),stream_state:l,message:s,progress:c||void 0,status:o,fn_index:e.fn_index,time_limit:u,time_start:g,eta_total:_,type:d,show_progress:this.show_progress[e.fn_index],used_cache:f,cache_duration:p,avg_time:m,cache_event_id:t};});}set_status(e,t){this.current[e].status=t;}resolve_args(e){let{fn_index:t,status:n,size:r=void 0,position:i=null,eta:a=null,message:o=null,stream_state:s=null,time_limit:c=null,progress_data:l=null,used_cache:u=null,cache_duration:d=null,avg_time:f=null}=e,p=this.fn_outputs[t],m=this.fn_status[t],h=this.fn_inputs[t];return p.concat(h).map(e=>{let t,g=this.pending_outputs.get(e)||0;if(m===`pending`&&n!==`pending`){let r=g-1;this.pending_outputs.set(e,r<0?0:r),t=r>0?`pending`:n;}else m===`pending`&&n===`pending`?t=`pending`:m!==`pending`&&n===`pending`?(t=`pending`,this.pending_outputs.set(e,g+1)):t=n;let _=h.includes(e)&&s?`input`:p.includes(e)?`output`:`skip`;return {id:e,queue_position:i,queue_size:r,eta:a,status:t,message:o,progress:l,stream_state:s,time_limit:c,type:_,used_cache:u,cache_duration:d,avg_time:f}}).filter(e=>e.type!==`skip`)}};

export { _, d, g, is_date as i, loop as l, m, p, raf as r, v };
//# sourceMappingURL=statustracker-Bh1XNgjP.js.map
