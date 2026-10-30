import { e as e$1, n as n$2 } from './environment-BbsaTwvI.js';
import { H as HttpError, S as SvelteKitError } from './index-DBqjc0Yf.js';
import { y as hydration_mismatch, H as HYDRATION_ERROR, C as COMMENT_NODE, z as HYDRATION_END, A as HYDRATION_START, B as HYDRATION_START_ELSE, D as get_next_sibling, E as effect_tracking, F as get, G as render_effect, I as source, J as untrack, K as increment, L as queue_micro_task, M as active_effect, N as BOUNDARY_EFFECT, O as block, P as branch, Q as create_text, R as pause_effect, S as current_batch, T as move_effect, U as defer_effect, V as set_active_effect, W as set_active_reaction, X as set_component_context, Y as Batch, Z as handle_error, _ as active_reaction, $ as component_context, a0 as internal_set, a1 as destroy_effect, a2 as invoke_error_boundary, a3 as svelte_boundary_reset_noop, a4 as svelte_boundary_reset_onerror, a5 as HYDRATION_START_FAILED, a6 as EFFECT_TRANSPARENT, a7 as EFFECT_PRESERVED, a8 as define_property, a9 as init_operations, aa as get_first_child, ab as hydration_failed, ac as clear_text_content, ad as component_root, ae as array_from, af as is_passive_event, ag as push, ah as pop, ai as set, aj as LEGACY_PROPS, ak as flushSync, al as mutable_source, am as render, q as setContext, d as derived } from './renderer-RGQlaTg4.js';
import { a as async_mode_flag } from './async-Cv1-GZGV.js';

/** @import { TemplateNode } from '#client' */


/**
 * Use this variable to guard everything related to hydration code so it can be treeshaken out
 * if the user doesn't use the `hydrate` method and these code paths are therefore not needed.
 */
let hydrating = false;

/** @param {boolean} value */
function set_hydrating(value) {
	hydrating = value;
}

/**
 * The node that is currently being hydrated. This starts out as the first node inside the opening
 * <!--[--> comment, and updates each time a component calls `$.child(...)` or `$.sibling(...)`.
 * When entering a block (e.g. `{#if ...}`), `hydrate_node` is the block opening comment; by the
 * time we leave the block it is the closing comment, which serves as the block's anchor.
 * @type {TemplateNode}
 */
let hydrate_node;

/** @param {TemplateNode | null} node */
function set_hydrate_node(node) {
	if (node === null) {
		hydration_mismatch();
		throw HYDRATION_ERROR;
	}

	return (hydrate_node = node);
}

function hydrate_next() {
	return set_hydrate_node(get_next_sibling(hydrate_node));
}

function next(count = 1) {
	if (hydrating) {
		var i = count;
		var node = hydrate_node;

		while (i--) {
			node = /** @type {TemplateNode} */ (get_next_sibling(node));
		}

		hydrate_node = node;
	}
}

/**
 * Skips or removes (depending on {@link remove}) all nodes starting at `hydrate_node` up until the next hydration end comment
 * @param {boolean} remove
 */
function skip_nodes(remove = true) {
	var depth = 0;
	var node = hydrate_node;

	while (true) {
		if (node.nodeType === COMMENT_NODE) {
			var data = /** @type {Comment} */ (node).data;

			if (data === HYDRATION_END) {
				if (depth === 0) return node;
				depth -= 1;
			} else if (
				data === HYDRATION_START ||
				data === HYDRATION_START_ELSE ||
				// "[1", "[2", etc. for if blocks
				(data[0] === '[' && !isNaN(Number(data.slice(1))))
			) {
				depth += 1;
			}
		}

		var next = /** @type {TemplateNode} */ (get_next_sibling(node));
		if (remove) node.remove();
		node = next;
	}
}

/**
 * Returns a `subscribe` function that integrates external event-based systems with Svelte's reactivity.
 * It's particularly useful for integrating with web APIs like `MediaQuery`, `IntersectionObserver`, or `WebSocket`.
 *
 * If `subscribe` is called inside an effect (including indirectly, for example inside a getter),
 * the `start` callback will be called with an `update` function. Whenever `update` is called, the effect re-runs.
 *
 * If `start` returns a cleanup function, it will be called when the effect is destroyed.
 *
 * If `subscribe` is called in multiple effects, `start` will only be called once as long as the effects
 * are active, and the returned teardown function will only be called when all effects are destroyed.
 *
 * It's best understood with an example. Here's an implementation of [`MediaQuery`](https://svelte.dev/docs/svelte/svelte-reactivity#MediaQuery):
 *
 * ```js
 * import { createSubscriber } from 'svelte/reactivity';
 * import { on } from 'svelte/events';
 *
 * export class MediaQuery {
 * 	#query;
 * 	#subscribe;
 *
 * 	constructor(query) {
 * 		this.#query = window.matchMedia(`(${query})`);
 *
 * 		this.#subscribe = createSubscriber((update) => {
 * 			// when the `change` event occurs, re-run any effects that read `this.current`
 * 			const off = on(this.#query, 'change', update);
 *
 * 			// stop listening when all the effects are destroyed
 * 			return () => off();
 * 		});
 * 	}
 *
 * 	get current() {
 * 		// This makes the getter reactive, if read in an effect
 * 		this.#subscribe();
 *
 * 		// Return the current state of the query, whether or not we're in an effect
 * 		return this.#query.matches;
 * 	}
 * }
 * ```
 * @param {(update: () => void) => (() => void) | void} start
 * @since 5.7.0
 */
function createSubscriber(start) {
	let subscribers = 0;
	let version = source(0);
	/** @type {(() => void) | void} */
	let stop;

	return () => {
		if (effect_tracking()) {
			get(version);

			render_effect(() => {
				if (subscribers === 0) {
					stop = untrack(() => start(() => increment(version)));
				}

				subscribers += 1;

				return () => {
					queue_micro_task(() => {
						// Only count down after a microtask, else we would reach 0 before our own render effect reruns,
						// but reach 1 again when the tick callback of the prior teardown runs. That would mean we
						// re-subcribe unnecessarily and create a memory leak because the old subscription is never cleaned up.
						subscribers -= 1;

						if (subscribers === 0) {
							stop?.();
							stop = undefined;
							// Increment the version to ensure any dependent deriveds are marked dirty when the subscription is picked up again later.
							// If we didn't do this then the comparison of write versions would determine that the derived has a later version than
							// the subscriber, and it would not be re-run.
							increment(version);
						}
					});
				};
			});
		}
	};
}

/** @import { Effect, Source, TemplateNode, } from '#client' */

/**
 * @typedef {{
 * 	 onerror?: ((error: unknown, reset: () => void) => void) | null;
 *   failed?: ((anchor: Node, error: () => unknown, reset: () => () => void) => void) | null;
 *   pending?: ((anchor: Node) => void) | null;
 * }} BoundaryProps
 */

var flags = EFFECT_TRANSPARENT | EFFECT_PRESERVED;

/**
 * @param {TemplateNode} node
 * @param {BoundaryProps} props
 * @param {((anchor: Node) => void)} children
 * @param {((error: unknown) => unknown) | undefined} [transform_error]
 * @returns {void}
 */
function boundary(node, props, children, transform_error) {
	new Boundary(node, props, children, transform_error);
}

class Boundary {
	/** @type {Boundary | null} */
	parent;

	is_pending = false;

	/**
	 * API-level transformError transform function. Transforms errors before they reach the `failed` snippet.
	 * Inherited from parent boundary, or defaults to identity.
	 * @type {(error: unknown) => unknown}
	 */
	transform_error;

	/** @type {TemplateNode} */
	#anchor;

	/** @type {TemplateNode | null} */
	#hydrate_open = hydrating ? hydrate_node : null;

	/** @type {BoundaryProps} */
	#props;

	/** @type {((anchor: Node) => void)} */
	#children;

	/** @type {Effect} */
	#effect;

	/** @type {Effect | null} */
	#main_effect = null;

	/** @type {Effect | null} */
	#pending_effect = null;

	/** @type {Effect | null} */
	#failed_effect = null;

	/** @type {DocumentFragment | null} */
	#offscreen_fragment = null;

	#local_pending_count = 0;
	#pending_count = 0;
	#pending_count_update_queued = false;

	/** @type {Set<Effect>} */
	#dirty_effects = new Set();

	/** @type {Set<Effect>} */
	#maybe_dirty_effects = new Set();

	/**
	 * A source containing the number of pending async deriveds/expressions.
	 * Only created if `$effect.pending()` is used inside the boundary,
	 * otherwise updating the source results in needless `Batch.ensure()`
	 * calls followed by no-op flushes
	 * @type {Source<number> | null}
	 */
	#effect_pending = null;

	#effect_pending_subscriber = createSubscriber(() => {
		this.#effect_pending = source(this.#local_pending_count);

		return () => {
			this.#effect_pending = null;
		};
	});

	/**
	 * @param {TemplateNode} node
	 * @param {BoundaryProps} props
	 * @param {((anchor: Node) => void)} children
	 * @param {((error: unknown) => unknown) | undefined} [transform_error]
	 */
	constructor(node, props, children, transform_error) {
		this.#anchor = node;
		this.#props = props;

		this.#children = (anchor) => {
			var effect = /** @type {Effect} */ (active_effect);

			effect.b = this;
			effect.f |= BOUNDARY_EFFECT;

			children(anchor);
		};

		this.parent = /** @type {Effect} */ (active_effect).b;

		// Inherit transform_error from parent boundary, or use the provided one, or default to identity
		this.transform_error = transform_error ?? this.parent?.transform_error ?? ((e) => e);

		this.#effect = block(() => {
			if (hydrating) {
				const comment = /** @type {Comment} */ (this.#hydrate_open);
				hydrate_next();

				const server_rendered_pending = comment.data === HYDRATION_START_ELSE;
				const server_rendered_failed = comment.data.startsWith(HYDRATION_START_FAILED);

				if (server_rendered_failed) {
					// Server rendered the failed snippet - hydrate it.
					// The serialized error is embedded in the comment: <!--[?<json>-->
					const serialized_error = JSON.parse(comment.data.slice(HYDRATION_START_FAILED.length));
					this.#hydrate_failed_content(serialized_error);
				} else if (server_rendered_pending) {
					this.#hydrate_pending_content();
				} else {
					this.#hydrate_resolved_content();
				}
			} else {
				this.#render();
			}
		}, flags);

		if (hydrating) {
			this.#anchor = hydrate_node;
		}
	}

	#hydrate_resolved_content() {
		try {
			this.#main_effect = branch(() => this.#children(this.#anchor));
		} catch (error) {
			this.error(error);
		}
	}

	/**
	 * @param {unknown} error The deserialized error from the server's hydration comment
	 */
	#hydrate_failed_content(error) {
		const failed = this.#props.failed;
		const { reset, invoke_onerror } = this.#create_reset(error);

		// `onerror` may mutate state, which is disallowed while hydrating
		queue_micro_task(invoke_onerror);

		if (!failed) return;

		this.#failed_effect = branch(() => {
			failed(
				this.#anchor,
				() => error,
				() => reset
			);
		});
	}

	/**
	 * Creates the `reset` function for a failed boundary, along with a function
	 * that invokes `onerror` with it (if provided)
	 * @param {unknown} error
	 * @returns {{ reset: () => void, invoke_onerror: () => void }}
	 */
	#create_reset(error) {
		var did_reset = false;
		var calling_on_error = false;

		const reset = () => {
			if (did_reset) {
				svelte_boundary_reset_noop();
				return;
			}

			did_reset = true;

			if (calling_on_error) {
				svelte_boundary_reset_onerror();
			}

			if (this.#failed_effect !== null) {
				pause_effect(this.#failed_effect, () => {
					this.#failed_effect = null;
				});
			}

			this.#run(() => {
				this.#render();
			});
		};

		const invoke_onerror = () => {
			try {
				calling_on_error = true;
				this.#props.onerror?.(error, reset);
				calling_on_error = false;
			} catch (err) {
				invoke_error_boundary(err, this.#effect && this.#effect.parent);
			}
		};

		return { reset, invoke_onerror };
	}

	#hydrate_pending_content() {
		const pending = this.#props.pending;
		if (!pending) return;

		this.is_pending = true;
		this.#pending_effect = branch(() => pending(this.#anchor));

		queue_micro_task(() => {
			var fragment = (this.#offscreen_fragment = document.createDocumentFragment());
			var anchor = create_text();

			fragment.append(anchor);

			this.#main_effect = this.#run(() => {
				return branch(() => this.#children(anchor));
			});

			if (this.#pending_count === 0) {
				this.#anchor.before(fragment);
				this.#offscreen_fragment = null;

				pause_effect(/** @type {Effect} */ (this.#pending_effect), () => {
					this.#pending_effect = null;
				});

				this.#resolve(/** @type {Batch} */ (current_batch));
			}
		});
	}

	#render() {
		try {
			this.is_pending = this.has_pending_snippet();
			this.#pending_count = 0;
			this.#local_pending_count = 0;

			this.#main_effect = branch(() => {
				this.#children(this.#anchor);
			});

			if (this.#pending_count > 0) {
				var fragment = (this.#offscreen_fragment = document.createDocumentFragment());
				move_effect(this.#main_effect, fragment);

				const pending = /** @type {(anchor: Node) => void} */ (this.#props.pending);
				this.#pending_effect = branch(() => pending(this.#anchor));
			} else {
				this.#resolve(/** @type {Batch} */ (current_batch));
			}
		} catch (error) {
			this.error(error);
		}
	}

	/**
	 * @param {Batch} batch
	 */
	#resolve(batch) {
		this.is_pending = false;

		// any effects that were previously deferred should be transferred
		// to the batch, which will flush in the next microtask
		batch.transfer_effects(this.#dirty_effects, this.#maybe_dirty_effects);
	}

	/**
	 * Defer an effect inside a pending boundary until the boundary resolves
	 * @param {Effect} effect
	 */
	defer_effect(effect) {
		defer_effect(effect, this.#dirty_effects, this.#maybe_dirty_effects);
	}

	/**
	 * Returns `false` if the effect exists inside a boundary whose pending snippet is shown
	 * @returns {boolean}
	 */
	is_rendered() {
		return !this.is_pending && (!this.parent || this.parent.is_rendered());
	}

	has_pending_snippet() {
		return !!this.#props.pending;
	}

	/**
	 * @template T
	 * @param {() => T} fn
	 */
	#run(fn) {
		var previous_effect = active_effect;
		var previous_reaction = active_reaction;
		var previous_ctx = component_context;

		set_active_effect(this.#effect);
		set_active_reaction(this.#effect);
		set_component_context(this.#effect.ctx);

		try {
			Batch.ensure();
			return fn();
		} catch (e) {
			handle_error(e);
			return null;
		} finally {
			set_active_effect(previous_effect);
			set_active_reaction(previous_reaction);
			set_component_context(previous_ctx);
		}
	}

	/**
	 * Updates the pending count associated with the currently visible pending snippet,
	 * if any, such that we can replace the snippet with content once work is done
	 * @param {1 | -1} d
	 * @param {Batch} batch
	 */
	#update_pending_count(d, batch) {
		if (!this.has_pending_snippet()) {
			if (this.parent) {
				this.parent.#update_pending_count(d, batch);
			}

			// if there's no parent, we're in a scope with no pending snippet
			return;
		}

		this.#pending_count += d;

		if (this.#pending_count === 0) {
			this.#resolve(batch);

			if (this.#pending_effect) {
				pause_effect(this.#pending_effect, () => {
					this.#pending_effect = null;
				});
			}

			if (this.#offscreen_fragment) {
				this.#anchor.before(this.#offscreen_fragment);
				this.#offscreen_fragment = null;
			}
		}
	}

	/**
	 * Update the source that powers `$effect.pending()` inside this boundary,
	 * and controls when the current `pending` snippet (if any) is removed.
	 * Do not call from inside the class
	 * @param {1 | -1} d
	 * @param {Batch} batch
	 */
	update_pending_count(d, batch) {
		this.#update_pending_count(d, batch);

		this.#local_pending_count += d;

		if (!this.#effect_pending || this.#pending_count_update_queued) return;
		this.#pending_count_update_queued = true;

		queue_micro_task(() => {
			this.#pending_count_update_queued = false;
			if (this.#effect_pending) {
				internal_set(this.#effect_pending, this.#local_pending_count);
			}
		});
	}

	get_effect_pending() {
		this.#effect_pending_subscriber();
		return get(/** @type {Source<number>} */ (this.#effect_pending));
	}

	/** @param {unknown} error */
	error(error) {
		// If we have nothing to capture the error, or if we hit an error while
		// rendering the fallback, re-throw for another boundary to handle
		if (!this.#props.onerror && !this.#props.failed) {
			throw error;
		}

		if (current_batch?.is_fork) {
			if (this.#main_effect) current_batch.skip_effect(this.#main_effect);
			if (this.#pending_effect) current_batch.skip_effect(this.#pending_effect);
			if (this.#failed_effect) current_batch.skip_effect(this.#failed_effect);

			current_batch.oncommit(() => {
				this.#handle_error(error);
			});
		} else {
			this.#handle_error(error);
		}
	}

	/**
	 * @param {unknown} error
	 */
	#handle_error(error) {
		if (this.#main_effect) {
			destroy_effect(this.#main_effect);
			this.#main_effect = null;
		}

		if (this.#pending_effect) {
			destroy_effect(this.#pending_effect);
			this.#pending_effect = null;
		}

		if (this.#failed_effect) {
			destroy_effect(this.#failed_effect);
			this.#failed_effect = null;
		}

		if (hydrating) {
			set_hydrate_node(/** @type {TemplateNode} */ (this.#hydrate_open));
			next();
			set_hydrate_node(skip_nodes());
		}

		let failed = this.#props.failed;

		/** @param {unknown} transformed_error */
		const handle_error_result = (transformed_error) => {
			const { reset, invoke_onerror } = this.#create_reset(transformed_error);

			invoke_onerror();

			if (failed) {
				this.#failed_effect = this.#run(() => {
					try {
						return branch(() => {
							// errors in `failed` snippets cause the boundary to error again
							// TODO Svelte 6: revisit this decision, most likely better to go to parent boundary instead
							var effect = /** @type {Effect} */ (active_effect);

							effect.b = this;
							effect.f |= BOUNDARY_EFFECT;

							failed(
								this.#anchor,
								() => transformed_error,
								() => reset
							);
						});
					} catch (error) {
						invoke_error_boundary(error, /** @type {Effect} */ (this.#effect.parent));
						return null;
					}
				});
			}
		};

		queue_micro_task(() => {
			// Run the error through the API-level transformError transform (e.g. SvelteKit's handleError)
			/** @type {unknown} */
			var result;
			try {
				result = this.transform_error(error);
			} catch (e) {
				invoke_error_boundary(e, this.#effect && this.#effect.parent);
				return;
			}

			if (
				result !== null &&
				typeof result === 'object' &&
				typeof (/** @type {any} */ (result).then) === 'function'
			) {
				// transformError returned a Promise — wait for it
				/** @type {any} */ (result).then(
					handle_error_result,
					/** @param {unknown} e */
					(e) => invoke_error_boundary(e, this.#effect && this.#effect.parent)
				);
			} else {
				// Synchronous result — handle immediately
				handle_error_result(result);
			}
		});
	}
}

function i$2(){}function a$2(e){let t=false,n;return ()=>t?n:(t=true,n=e())}var o$2=2**32-1,s$3=o$2-1,c$2={"<":`\\u003C`,"\\":`\\\\`,"\b":`\\b`,"\f":`\\f`,"\n":`\\n`,"\r":`\\r`,"	":`\\t`,"\u2028":`\\u2028`,"\u2029":`\\u2029`},l$3=class l extends Error{constructor(e,t,n,r){super(e),this.name=`DevalueError`,this.path=t.join(``),this.value=n,this.root=r;}};function u$2(e){return e===null||typeof e!=`object`&&typeof e!=`function`}var d$2=Object.getOwnPropertyNames(Object.prototype).sort().join(`\0`);function f$1(e){let t=Object.getPrototypeOf(e);return t===Object.prototype||t===null||Object.getPrototypeOf(t)===null||Object.getOwnPropertyNames(t).sort().join(`\0`)===d$2}function p$1(e){return Object.prototype.toString.call(e).slice(8,-1)}function m$1(e){switch(e){case `"`:return `\\"`;case `<`:return `\\u003C`;case `\\`:return `\\\\`;case `
`:return `\\n`;case `\r`:return `\\r`;case `	`:return `\\t`;case `\b`:return `\\b`;case `\f`:return `\\f`;case `\u2028`:return `\\u2028`;case `\u2029`:return `\\u2029`;default:return e<` `?`\\u${e.charCodeAt(0).toString(16).padStart(4,`0`)}`:``}}function h$1(e){let t=``,n=0,r=e.length;for(let i=0;i<r;i+=1){let r=e[i],a=m$1(r);a&&(t+=e.slice(n,i)+a,n=i+1);}return `"${n===0?e:t+e.slice(n)}"`}function g$1(e){return Object.getOwnPropertySymbols(e).filter(t=>Object.getOwnPropertyDescriptor(e,t).enumerable)}var _$1=/^[a-zA-Z_$][a-zA-Z_$0-9]*$/;function v$1(e){return _$1.test(e)?`.`+e:`[`+JSON.stringify(e)+`]`}function y$1(e){return !(!Number.isInteger(e)||e<0||e>s$3)}function ee(e){return !(!Number.isInteger(e)||e<0||e>o$2)}function te(e){if(e.length===0||e.length>1&&e.charCodeAt(0)===48)return  false;for(let t=0;t<e.length;t++){let n=e.charCodeAt(t);if(n<48||n>57)return  false}return y$1(+e)}function b$1(e){let t=Object.keys(e);for(var n=t.length-1;n>=0&&!te(t[n]);n--);return t.length=n+1,t}function x$1(e){return new Uint8Array(e).toBase64()}function S$1(e){return Uint8Array.fromBase64(e).buffer}function C$1(e){return Buffer.from(e).toString(`base64`)}function w$1(e){return Uint8Array.from(Buffer.from(e,`base64`)).buffer}function T$1(e){let t=new Uint8Array(e),n=``,r=32768;for(let e=0;e<t.length;e+=r){let i=t.subarray(e,e+r);n+=String.fromCharCode.apply(null,i);}return btoa(n)}function ne(e){let t=atob(e),n=t.length,r=new Uint8Array(n);for(let e=0;e<n;e++)r[e]=t.charCodeAt(e);return r.buffer}var E$1=typeof Uint8Array.fromBase64==`function`,D$1=typeof process==`object`&&process.versions?.node!==void 0,re=E$1?x$1:D$1?C$1:T$1,O$1=E$1?S$1:D$1?w$1:ne;function k$1(e,t){return A$1(JSON.parse(e),t)}function A$1(e,t){if(typeof e==`number`)return a(e,true);if(!Array.isArray(e)||e.length===0)throw Error(`Invalid input`);let n=e,r=Array(n.length),i=null;function a(e,o=false){if(e===-1)return;if(e===-3)return NaN;if(e===-4)return 1/0;if(e===-5)return  -1/0;if(e===-6)return  -0;if(o||typeof e!=`number`)throw Error(`Invalid input`);if(e in r)return r[e];let c=n[e];if(!c||typeof c!=`object`)r[e]=c;else if(Array.isArray(c))if(typeof c[0]==`string`){let o=c[0],s=t&&Object.hasOwn(t,o)?t[o]:void 0;if(s){let t=c[1];if(typeof t!=`number`&&(t=n.push(c[1])-1),i??=new Set,i.has(t))throw Error(`Invalid circular reference`);return i.add(t),r[e]=s(a(t)),i.delete(t),r[e]}switch(o){case `Date`:r[e]=new Date(c[1]);break;case `Set`:let t=new Set;r[e]=t;for(let e=1;e<c.length;e+=1)t.add(a(c[e]));break;case `Map`:let i=new Map;r[e]=i;for(let e=1;e<c.length;e+=2)i.set(a(c[e]),a(c[e+1]));break;case `RegExp`:r[e]=new RegExp(c[1],c[2]);break;case `Object`:{let t=c[1];if(typeof n[t]==`object`&&n[t][0]!==`BigInt`)throw Error(`Invalid input`);r[e]=Object(a(t));break}case `BigInt`:r[e]=BigInt(c[1]);break;case `null`:let s=Object.create(null);r[e]=s;for(let e=1;e<c.length;e+=2){if(c[e]===`__proto__`)throw Error("Cannot parse an object with a `__proto__` property");s[c[e]]=a(c[e+1]);}break;case `Int8Array`:case `Uint8Array`:case `Uint8ClampedArray`:case `Int16Array`:case `Uint16Array`:case `Float16Array`:case `Int32Array`:case `Uint32Array`:case `Float32Array`:case `Float64Array`:case `BigInt64Array`:case `BigUint64Array`:case `DataView`:{if(n[c[1]][0]!==`ArrayBuffer`)throw Error(`Invalid data`);let t=globalThis[o],i=a(c[1]);r[e]=c[2]===void 0?new t(i):new t(i,c[2],c[3]);break}case `ArrayBuffer`:{let t=c[1];if(typeof t!=`string`)throw Error(`Invalid ArrayBuffer encoding`);r[e]=O$1(t);break}case `Temporal.Duration`:case `Temporal.Instant`:case `Temporal.PlainDate`:case `Temporal.PlainTime`:case `Temporal.PlainDateTime`:case `Temporal.PlainMonthDay`:case `Temporal.PlainYearMonth`:case `Temporal.ZonedDateTime`:{let t=o.slice(9);r[e]=Temporal[t].from(c[1]);break}case `URL`:r[e]=new URL(c[1]);break;case `URLSearchParams`:r[e]=new URLSearchParams(c[1]);break;default:throw Error(`Unknown type ${o}`)}}else if(c[0]===-7){let t=c[1];if(!ee(t))throw Error(`Invalid input`);let n=[];r[e]=n,n[s$3]=void 0,delete n[s$3];for(let e=2;e<c.length;e+=2){let r=c[e];if(!y$1(r)||r>=t)throw Error(`Invalid input`);n[r]=a(c[e+1]);}n.length=t;}else {let t=Array(c.length);r[e]=t;for(let e=0;e<c.length;e+=1){let n=c[e];n!==-2&&(t[e]=a(n));}}else {let t={};r[e]=t;for(let e of Object.keys(c)){if(e===`__proto__`)throw Error("Cannot parse an object with a `__proto__` property");let n=c[e];t[e]=a(n);}}return r[e]}return a(0)}function j$1(e,t){let n=M$1(false,e,t);return typeof n==`string`?n:`[${n.join(`,`)}]`}function M$1(e,t,n){let r=[],i=new Map,a=[];if(n)for(let e of Object.getOwnPropertyNames(n))a.push({key:e,fn:n[e]});let o=[],s=0;function c(n,d){if(n===void 0)return  -1;if(Number.isNaN(n))return  -3;if(n===1/0)return  -4;if(n===-1/0)return  -5;if(n===0&&1/n<0)return  -6;if(i.has(n))return i.get(n);d??=s++,i.set(n,d);for(let{key:e,fn:t}of a){let i=t(n);if(i)return r[d]=`["${e}",${c(i)}]`,d}if(typeof n==`function`)throw new l$3(`Cannot stringify a function`,o,n,t);if(typeof n==`symbol`)throw new l$3(`Cannot stringify a Symbol primitive`,o,n,t);let m=``;if(u$2(n))m=N$1(n);else if(typeof n.then==`function`){throw new l$3(`Cannot stringify a Promise or thenable — use stringifyAsync instead`,o,n,t);}else {let e=p$1(n);switch(e){case `Number`:case `String`:case `Boolean`:case `BigInt`:m=`["Object",${c(n.valueOf())}]`;break;case `Date`:m=`["Date","${isNaN(n.getDate())?``:n.toISOString()}"]`;break;case `URL`:m=`["URL",${h$1(n.toString())}]`;break;case `URLSearchParams`:m=`["URLSearchParams",${h$1(n.toString())}]`;break;case `RegExp`:let{source:r,flags:i}=n;m=i?`["RegExp",${h$1(r)},"${i}"]`:`["RegExp",${h$1(r)}]`;break;case `Array`:{let e=false;m=`[`;for(let t=0;t<n.length;t+=1)if(t>0&&(m+=`,`),Object.hasOwn(n,t))o.push(`[${t}]`),m+=c(n[t]),o.pop();else if(e)m+=-2;else {let t=b$1(n),r=t.length,i=String(n.length).length;if((n.length-r)*3>4+i+r*(i+1)){m=`[-7,`+n.length;for(let e=0;e<t.length;e++){let r=t[e];o.push(`[${r}]`),m+=`,`+r+`,`+c(n[r]),o.pop();}break}else e=true,m+=-2;}m+=`]`;break}case `Set`:m=`["Set"`;for(let e of n)m+=`,${c(e)}`;m+=`]`;break;case `Map`:m=`["Map"`;for(let[e,t]of n)o.push(`.get(${u$2(e)?N$1(e):`...`})`),m+=`,${c(e)},${c(t)}`,o.pop();m+=`]`;break;case `Int8Array`:case `Uint8Array`:case `Uint8ClampedArray`:case `Int16Array`:case `Uint16Array`:case `Float16Array`:case `Int32Array`:case `Uint32Array`:case `Float32Array`:case `Float64Array`:case `BigInt64Array`:case `BigUint64Array`:case `DataView`:{let t=n;m=`["`+e+`",`+c(t.buffer),t.byteLength!==t.buffer.byteLength&&(m+=`,${t.byteOffset},${t.length}`),m+=`]`;break}case `ArrayBuffer`:m=`["ArrayBuffer","${re(n)}"]`;break;case `Temporal.Duration`:case `Temporal.Instant`:case `Temporal.PlainDate`:case `Temporal.PlainTime`:case `Temporal.PlainDateTime`:case `Temporal.PlainMonthDay`:case `Temporal.PlainYearMonth`:case `Temporal.ZonedDateTime`:m=`["${e}",${h$1(n.toString())}]`;break;default:if(!f$1(n))throw new l$3(`Cannot stringify arbitrary non-POJOs`,o,n,t);if(g$1(n).length>0)throw new l$3(`Cannot stringify POJOs with symbolic keys`,o,n,t);if(Object.getPrototypeOf(n)===null){m=`["null"`;for(let e of Object.keys(n)){if(e===`__proto__`)throw new l$3(`Cannot stringify objects with __proto__ keys`,o,n,t);o.push(v$1(e)),m+=`,${h$1(e)},${c(n[e])}`,o.pop();}m+=`]`;}else {m=`{`;let e=false;for(let r of Object.keys(n)){if(r===`__proto__`)throw new l$3(`Cannot stringify objects with __proto__ keys`,o,n,t);e&&(m+=`,`),e=true,o.push(v$1(r)),m+=`${h$1(r)}:${c(n[r])}`,o.pop();}m+=`}`;}}}return r[d]=m,d}let d=c(t);return d<0?`${d}`:r}function N$1(e){let t=typeof e;return t===`string`?h$1(e):e===void 0?`-1`:e===0&&1/e<0?`-6`:t===`bigint`?`["BigInt","${e}"]`:String(e)}var P$1=new TextEncoder;function F$1(e,t){let n=e.split(/[/\\]/),r=t.split(/[/\\]/);for(n.pop();n[0]===r[0];)n.shift(),r.shift();let i=n.length;for(;i--;)n[i]=`..`;return n.concat(r).join(`/`)}function I$1(t){if(!e$1&&globalThis.Buffer)return globalThis.Buffer.from(t).toString(`base64`);let n=``;for(let e=0;e<t.length;e++)n+=String.fromCharCode(t[e]);return btoa(n)}function L$1(t){if(!e$1&&globalThis.Buffer){let e=globalThis.Buffer.from(t,`base64`);return new Uint8Array(e)}let n=atob(t),r=new Uint8Array(n.length);for(let e=0;e<n.length;e++)r[e]=n.charCodeAt(e);return r}function R$1(e){return e instanceof Error||e&&e.name&&e.message?e:Error(JSON.stringify(e))}function z(e){return e}function B(e){return e instanceof HttpError||e instanceof SvelteKitError?e.status:500}function V(e){return e instanceof SvelteKitError?e.text:`Internal Error`}function H(e,t){let n=/^(moz-icon|view-source|jar):/.exec(t);n&&console.warn(`${e}: Calling \`depends('${t}')\` will throw an error in Firefox because \`${n[1]}\` is a special URI scheme`);}var U=`x-sveltekit-invalidated`,ie=`x-sveltekit-trailing-slash`;function W(e,t){if(e!=null&&Object.getPrototypeOf(e)!==Object.prototype)throw Error(`a load function ${t} returned ${typeof e==`object`?e instanceof Response?`a Response object`:Array.isArray(e)?`an array`:`a non-plain object`:`a ${typeof e}`}, but must return a plain object at the top level (i.e. \`return {...}\`)`)}function G(e,t){return j$1(e,Object.fromEntries(Object.entries(t).map(([e,t])=>[e,t.encode])))}Object.getOwnPropertyNames(Object.prototype).sort().join(`\0`);var Y=`__skrao`,X=`__skram`,Z=`__skras`;function se(e){let t={[Y]:e=>e,[X]:e=>{if(!Array.isArray(e))throw Error(`Invalid data for Map reviver`);let t=new Map;for(let n of e){if(!Array.isArray(n)||n.length!==2||typeof n[0]!=`string`||typeof n[1]!=`string`)throw Error(`Invalid data for Map reviver`);let[e,i]=n;t.set(r(e),r(i));}return t},[Z]:e=>{if(!Array.isArray(e))throw Error(`Invalid data for Set reviver`);let t=new Set;for(let n of e){if(typeof n!=`string`)throw Error(`Invalid data for Set reviver`);t.add(r(n));}return t}},n={...Object.fromEntries(Object.entries(e).map(([e,t])=>[e,t.decode])),...t},r=e=>k$1(e,n);return n}function le(e,t){return e?k$1(new TextDecoder().decode(L$1(e.replaceAll(`-`,`+`).replaceAll(`_`,`/`))),se(t)):void 0}function $(e,t){return e+`/`+t}function ue(e){let t=e.lastIndexOf(`/`);if(t===-1)throw Error(`Invalid remote key: ${e}`);return {id:e.slice(0,t),payload:e.slice(t+1)}}

var e=``,t=e,n$1=`_app`,r$1={base:e,assets:t};function i$1(n){e=n.base,t=n.assets;}function a$1(){e=r$1.base,t=r$1.assets;}var s$2={};function c$1(e){}function l$2(e){s$2=e;}

// eslint-disable-next-line n/prefer-global/process
const IN_WEBCONTAINER = !!globalThis.process?.versions?.webcontainer;

/** @import { RequestEvent } from '@sveltejs/kit' */
/** @import { RequestStore } from 'types' */
/** @import { AsyncLocalStorage } from 'node:async_hooks' */


/** @type {RequestStore | null} */
let sync_store = null;

/** @type {AsyncLocalStorage<RequestStore | null> | null} */
let als;

import('node:async_hooks')
	.then((hooks) => (als = new hooks.AsyncLocalStorage()))
	.catch(() => {
		// can't use AsyncLocalStorage, but can still call getRequestEvent synchronously.
		// this isn't behind `supports` because it's basically just StackBlitz (i.e.
		// in-browser usage) that doesn't support it AFAICT
	});

function try_get_request_store() {
	return sync_store ?? als?.getStore() ?? null;
}

/**
 * @template T
 * @param {RequestStore | null} store
 * @param {() => T} fn
 */
function with_request_store(store, fn) {
	try {
		sync_store = store;
		return als ? als.run(store, fn) : fn();
	} finally {
		// Since AsyncLocalStorage is not working in webcontainers, we don't reset `sync_store`
		// and handle only one request at a time in `src/runtime/server/index.js`.
		if (!IN_WEBCONTAINER) {
			sync_store = null;
		}
	}
}

function n(e){return e.filter(e=>e!=null)}var r=`/__data.json`,i=`.html__data.json`;function a(e){return e.endsWith(r)||e.endsWith(i)}function o$1(e){return e.endsWith(`.html`)?e.replace(/\.html$/,i):e.replace(/\/$/,``)+r}function s$1(e){return e.endsWith(i)?e.slice(0,-16)+`.html`:e.slice(0,-12)}var c=`/__route.js`;function l$1(e){return e.endsWith(c)}function u$1(e){return e.replace(/\/$/,``)+c}function d$1(e){return e.slice(0,-11)}var f={spanContext(){return p},setAttribute(){return this},setAttributes(){return this},addEvent(){return this},setStatus(){return this},updateName(){return this},end(){return this},isRecording(){return  false},recordException(){return this},addLink(){return this},addLinks(){return this}},p={traceId:``,spanId:``,traceFlags:0},m=/^[a-z][a-z\d+\-.]+:/i,h=new URL(`sveltekit-internal://`);function g(e,t){if(t[0]===`/`&&t[1]===`/`)return t;let n=new URL(e,h);return n=new URL(t,n),n.protocol===h.protocol?n.pathname+n.search+n.hash:n.href}function _(e,t){return e===`/`||t===`ignore`?e:t===`never`?e.endsWith(`/`)?e.slice(0,-1):e:t===`always`&&!e.endsWith(`/`)?e+`/`:e}function v(e){return e.split(`%25`).map(decodeURI).join(`%25`)}function y(e){for(let t in e)e[t]=decodeURIComponent(e[t]);return e}function b(n,r,i,a=false){let o=new URL(n);Object.defineProperty(o,"searchParams",{value:new Proxy(o.searchParams,{get(e,t){if(t===`get`||t===`getAll`||t===`has`)return (n,...r)=>(i(n),e[t](n,...r));r();let n=Reflect.get(e,t);return typeof n==`function`?n.bind(e):n}}),enumerable:true,configurable:true});let s=[`href`,`pathname`,`search`,`toString`,`toJSON`];a&&s.push(`hash`);for(let e of s)Object.defineProperty(o,e,{get(){return r(),n[e]},enumerable:true,configurable:true});return e$1||(o[Symbol.for(`nodejs.util.inspect.custom`)]=(e,t,r)=>r(n,t),o.searchParams[Symbol.for(`nodejs.util.inspect.custom`)]=(e,t,r)=>r(n.searchParams,t)),(n$2||!e$1)&&!a&&x(o),o}function x(e){C(e),Object.defineProperty(e,"hash",{get(){throw Error("Cannot access event.url.hash. Consider using `page.url.hash` inside a component instead")}});}function S(e){C(e);for(let t of [`search`,`searchParams`])Object.defineProperty(e,t,{get(){throw Error(`Cannot access url.${t} on a page with prerendering enabled`)}});}function C(e){e$1||(e[Symbol.for(`nodejs.util.inspect.custom`)]=(t,n,r)=>r(new URL(e),n));}function w(...e){let t=5381;for(let n of e)if(typeof n==`string`){let e=n.length;for(;e;)t=t*33^n.charCodeAt(--e);}else if(ArrayBuffer.isView(n)){let e=new Uint8Array(n.buffer,n.byteOffset,n.byteLength),r=e.length;for(;r;)t=t*33^e[--r];}else throw TypeError(`value must be a string or TypedArray`);return (t>>>0).toString(36)}function T(e,t,n){let r={},i=e.slice(1),a=i.filter(e=>e!==void 0),o=0;for(let e=0;e<t.length;e+=1){let s=t[e],c=i[e-o];if(s.chained&&s.rest&&o&&(c=i.slice(e-o,e+1).filter(e=>e).join(`/`),o=0),c===void 0)if(s.rest)c=``;else continue;if(!s.matcher||n[s.matcher](c)){r[s.name]=c;let n=t[e+1],l=i[e+1];n&&!n.rest&&n.optional&&l&&s.chained&&(o=0),!n&&!l&&Object.keys(r).length===a.length&&(o=0);continue}if(s.optional&&s.chained){o++;continue}return}if(!o)return r}function E(e,t,n){for(let r of t){let t=r.pattern.exec(e);if(!t)continue;let i=T(t,r.params,n);if(i)return {route:r,params:y(i)}}return null}function D(e){function t(t,n){if(t)for(let r in t){if(r[0]===`_`||e.has(r))continue;let t=[...e.values()],i=O(r,n?.slice(n.lastIndexOf(`.`)))??`valid exports are ${t.join(`, `)}, or anything with a '_' prefix`;throw Error(`Invalid export '${r}'${n?` in ${n}`:``} (${i})`)}}return t}function O(e,t=`.js`){let n=[];if(k.has(e)&&n.push(`+layout${t}`),A.has(e)&&n.push(`+page${t}`),j.has(e)&&n.push(`+layout.server${t}`),M.has(e)&&n.push(`+page.server${t}`),N.has(e)&&n.push(`+server${t}`),n.length>0)return `'${e}' is a valid export in ${n.slice(0,-1).join(`, `)}${n.length>1?` or `:``}${n.at(-1)}`}var k=new Set([`load`,`prerender`,`csr`,`ssr`,`trailingSlash`,`config`]),A=new Set([...k,`entries`]),j=new Set([...k]),M=new Set([...j,`actions`,`entries`]),N=new Set([`GET`,`POST`,`PATCH`,`PUT`,`DELETE`,`OPTIONS`,`HEAD`,`fallback`,`prerender`,`trailingSlash`,`config`,`entries`]),P=D(k),F=D(A),I=D(j),L=D(M),R=D(N);

/**
 * Used on elements, as a map of event type -> event handler,
 * and on events themselves to track which element handled an event
 */
const event_symbol = Symbol('events');

/** @type {Set<string>} */
const all_registered_events = new Set();

/** @type {Set<(events: Array<string>) => void>} */
const root_event_handles = new Set();

// used to store the reference to the currently propagated event
// to prevent garbage collection between microtasks in Firefox
// If the event object is GCed too early, the expando __root property
// set on the event object is lost, causing the event delegation
// to process the event twice
let last_propagated_event = null;

/**
 * @this {EventTarget}
 * @param {Event} event
 * @returns {void}
 */
function handle_event_propagation(event) {
	var handler_element = this;
	var owner_document = /** @type {Node} */ (handler_element).ownerDocument;
	var event_name = event.type;
	var path = event.composedPath?.() || [];
	var current_target = /** @type {null | Element} */ (path[0] || event.target);

	last_propagated_event = event;

	// composedPath contains list of nodes the event has propagated through.
	// We check `event_symbol` to skip all nodes below it in case this is a
	// parent of the `event_symbol` node, which indicates that there's nested
	// mounted apps. In this case we don't want to trigger events multiple times.
	var path_idx = 0;

	// the `last_propagated_event === event` check is redundant, but
	// without it the variable will be DCE'd and things will
	// fail mysteriously in Firefox
	// @ts-expect-error is added below
	var handled_at = last_propagated_event === event && event[event_symbol];

	if (handled_at) {
		var at_idx = path.indexOf(handled_at);
		if (
			at_idx !== -1 &&
			(handler_element === document || handler_element === /** @type {any} */ (window))
		) {
			// This is the fallback document listener or a window listener, but the event was already handled
			// -> ignore, but set handle_at to document/window so that we're resetting the event
			// chain in case someone manually dispatches the same event object again.
			// @ts-expect-error
			event[event_symbol] = handler_element;
			return;
		}

		// We're deliberately not skipping if the index is higher, because
		// someone could create an event programmatically and emit it multiple times,
		// in which case we want to handle the whole propagation chain properly each time.
		// (this will only be a false negative if the event is dispatched multiple times and
		// the fallback document listener isn't reached in between, but that's super rare)
		var handler_idx = path.indexOf(handler_element);
		if (handler_idx === -1) {
			// handle_idx can theoretically be -1 (happened in some JSDOM testing scenarios with an event listener on the window object)
			// so guard against that, too, and assume that everything was handled at this point.
			return;
		}

		if (at_idx <= handler_idx) {
			path_idx = at_idx;
		}
	}

	current_target = /** @type {Element} */ (path[path_idx] || event.target);
	// there can only be one delegated event per element, and we either already handled the current target,
	// or this is the very first target in the chain which has a non-delegated listener, in which case it's safe
	// to handle a possible delegated event on it later (through the root delegation listener for example).
	if (current_target === handler_element) return;

	// Proxy currentTarget to correct target
	define_property(event, 'currentTarget', {
		configurable: true,
		get() {
			return current_target || owner_document;
		}
	});

	// This started because of Chromium issue https://chromestatus.com/feature/5128696823545856,
	// where removal or moving of the DOM can cause sync `blur` events to fire, which can cause logic
	// to run inside the current `active_reaction`, which isn't what we want at all. However, on reflection,
	// it's probably best that all events handled by Svelte have this behaviour, as we don't really want
	// an event handler to run in the context of another reaction or effect.
	var previous_reaction = active_reaction;
	var previous_effect = active_effect;
	set_active_reaction(null);
	set_active_effect(null);

	try {
		/**
		 * @type {unknown}
		 */
		var throw_error;
		/**
		 * @type {unknown[]}
		 */
		var other_errors = [];

		while (current_target !== null) {
			if (current_target === handler_element) break;

			try {
				// @ts-expect-error
				var delegated = current_target[event_symbol]?.[event_name];

				if (
					delegated != null &&
					(!(/** @type {any} */ (current_target).disabled) ||
						// DOM could've been updated already by the time this is reached, so we check this as well
						// -> the target could not have been disabled because it emits the event in the first place
						event.target === current_target)
				) {
					delegated.call(current_target, event);
				}
			} catch (error) {
				if (throw_error) {
					other_errors.push(error);
				} else {
					throw_error = error;
				}
			}
			if (event.cancelBubble) break;

			path_idx++;
			current_target = path_idx < path.length ? /** @type {Element} */ (path[path_idx]) : null;
		}

		if (throw_error) {
			for (let error of other_errors) {
				// Throw the rest of the errors, one-by-one on a microtask
				queueMicrotask(() => {
					throw error;
				});
			}
			throw throw_error;
		}
	} finally {
		// @ts-expect-error is used above
		event[event_symbol] = handler_element;
		// @ts-ignore remove proxy on currentTarget
		delete event.currentTarget;
		set_active_reaction(previous_reaction);
		set_active_effect(previous_effect);
	}
}

/** @import { Effect, EffectNodes, TemplateNode } from '#client' */
/** @import { TemplateStructure } from './types' */

/**
 * @param {TemplateNode} start
 * @param {TemplateNode | null} end
 */
function assign_nodes(start, end) {
	var effect = /** @type {Effect} */ (active_effect);
	if (effect.nodes === null) {
		effect.nodes = { start, end, a: null, t: null };
	}
}

/** @import { ComponentContext, Effect, EffectNodes, TemplateNode } from '#client' */
/** @import { Component, ComponentType, SvelteComponent, MountOptions } from '../../index.js' */

/**
 * Mounts a component to the given target and returns the exports and potentially the props (if compiled with `accessors: true`) of the component.
 * Transitions will play during the initial render unless the `intro` option is set to `false`.
 *
 * @template {Record<string, any>} Props
 * @template {Record<string, any>} Exports
 * @param {ComponentType<SvelteComponent<Props>> | Component<Props, Exports, any>} component
 * @param {MountOptions<Props>} options
 * @returns {Exports}
 */
function mount(component, options) {
	return _mount(component, options);
}

/**
 * Hydrates a component on the given target and returns the exports and potentially the props (if compiled with `accessors: true`) of the component
 *
 * @template {Record<string, any>} Props
 * @template {Record<string, any>} Exports
 * @param {ComponentType<SvelteComponent<Props>> | Component<Props, Exports, any>} component
 * @param {{} extends Props ? {
 * 		target: Document | Element | ShadowRoot;
 * 		props?: Props;
 * 		events?: Record<string, (e: any) => any>;
 *  	context?: Map<any, any>;
 * 		intro?: boolean;
 * 		recover?: boolean;
 *		transformError?: (error: unknown) => unknown;
 * 	} : {
 * 		target: Document | Element | ShadowRoot;
 * 		props: Props;
 * 		events?: Record<string, (e: any) => any>;
 *  	context?: Map<any, any>;
 * 		intro?: boolean;
 * 		recover?: boolean;
 *		transformError?: (error: unknown) => unknown;
 * 	}} options
 * @returns {Exports}
 */
function hydrate(component, options) {
	init_operations();
	options.intro = options.intro ?? false;
	const target = options.target;
	const was_hydrating = hydrating;
	const previous_hydrate_node = hydrate_node;

	try {
		var anchor = get_first_child(target);

		while (
			anchor &&
			(anchor.nodeType !== COMMENT_NODE || /** @type {Comment} */ (anchor).data !== HYDRATION_START)
		) {
			anchor = get_next_sibling(anchor);
		}

		if (!anchor) {
			throw HYDRATION_ERROR;
		}

		set_hydrating(true);
		set_hydrate_node(/** @type {Comment} */ (anchor));

		const instance = _mount(component, { ...options, anchor });

		set_hydrating(false);

		return /**  @type {Exports} */ (instance);
	} catch (error) {
		// re-throw Svelte errors - they are certainly not related to hydration
		if (
			error instanceof Error &&
			error.message.split('\n').some((line) => line.startsWith('https://svelte.dev/e/'))
		) {
			throw error;
		}
		if (error !== HYDRATION_ERROR) {
			// eslint-disable-next-line no-console
			console.warn('Failed to hydrate: ', error);
		}

		if (options.recover === false) {
			hydration_failed();
		}

		// If an error occurred above, the operations might not yet have been initialised.
		init_operations();
		clear_text_content(target);

		set_hydrating(false);
		return mount(component, options);
	} finally {
		set_hydrating(was_hydrating);
		set_hydrate_node(previous_hydrate_node);
	}
}

/** @type {Map<EventTarget, Map<string, number>>} */
const listeners = new Map();

/**
 * @template {Record<string, any>} Exports
 * @param {ComponentType<SvelteComponent<any>> | Component<any>} Component
 * @param {MountOptions} options
 * @returns {Exports}
 */
function _mount(
	Component,
	{ target, anchor, props = {}, events, context, intro = true, transformError }
) {
	init_operations();

	/** @type {Exports} */
	// @ts-expect-error will be defined because the render effect runs synchronously
	var component = undefined;

	var unmount = component_root(() => {
		var anchor_node = anchor ?? target.appendChild(create_text());

		boundary(
			/** @type {TemplateNode} */ (anchor_node),
			{
				pending: () => {}
			},
			(anchor_node) => {
				push({});
				var ctx = /** @type {ComponentContext} */ (component_context);
				if (context) ctx.c = context;

				if (events) {
					// We can't spread the object or else we'd lose the state proxy stuff, if it is one
					/** @type {any} */ (props).$$events = events;
				}

				if (hydrating) {
					assign_nodes(/** @type {TemplateNode} */ (anchor_node), null);
				}
				// @ts-expect-error the public typings are not what the actual function looks like
				component = Component(anchor_node, props) || {};

				if (hydrating) {
					/** @type {Effect & { nodes: EffectNodes }} */ (active_effect).nodes.end = hydrate_node;

					if (
						hydrate_node === null ||
						hydrate_node.nodeType !== COMMENT_NODE ||
						/** @type {Comment} */ (hydrate_node).data !== HYDRATION_END
					) {
						hydration_mismatch();
						throw HYDRATION_ERROR;
					}
				}

				pop();
			},
			transformError
		);

		// Setup event delegation _after_ component is mounted - if an error would happen during mount, it would otherwise not be cleaned up
		/** @type {Set<string>} */
		var registered_events = new Set();

		/** @param {Array<string>} events */
		var event_handle = (events) => {
			for (var i = 0; i < events.length; i++) {
				var event_name = events[i];

				if (registered_events.has(event_name)) continue;
				registered_events.add(event_name);

				var passive = is_passive_event(event_name);

				// Add the event listener to both the container and the document.
				// The container listener ensures we catch events from within in case
				// the outer content stops propagation of the event.
				//
				// The document listener ensures we catch events that originate from elements that were
				// manually moved outside of the container (e.g. via manual portals).
				for (const node of [target, document]) {
					var counts = listeners.get(node);

					if (counts === undefined) {
						counts = new Map();
						listeners.set(node, counts);
					}

					var count = counts.get(event_name);

					if (count === undefined) {
						node.addEventListener(event_name, handle_event_propagation, { passive });
						counts.set(event_name, 1);
					} else {
						counts.set(event_name, count + 1);
					}
				}
			}
		};

		event_handle(array_from(all_registered_events));
		root_event_handles.add(event_handle);

		return () => {
			for (var event_name of registered_events) {
				for (const node of [target, document]) {
					var counts = /** @type {Map<string, number>} */ (listeners.get(node));
					var count = /** @type {number} */ (counts.get(event_name));

					if (--count == 0) {
						node.removeEventListener(event_name, handle_event_propagation);
						counts.delete(event_name);

						if (counts.size === 0) {
							listeners.delete(node);
						}
					} else {
						counts.set(event_name, count);
					}
				}
			}

			root_event_handles.delete(event_handle);

			if (anchor_node !== anchor) {
				anchor_node.parentNode?.removeChild(anchor_node);
			}
		};
	});

	mounted_components.set(component, unmount);
	return component;
}

/**
 * References of the components that were mounted or hydrated.
 * Uses a `WeakMap` to avoid memory leaks.
 */
let mounted_components = new WeakMap();

/**
 * Unmounts a component that was previously mounted using `mount` or `hydrate`.
 *
 * Since 5.13.0, if `options.outro` is `true`, [transitions](https://svelte.dev/docs/svelte/transition) will play before the component is removed from the DOM.
 *
 * Returns a `Promise` that resolves after transitions have completed if `options.outro` is true, or immediately otherwise (prior to 5.13.0, returns `void`).
 *
 * ```js
 * import { mount, unmount } from 'svelte';
 * import App from './App.svelte';
 *
 * const app = mount(App, { target: document.body });
 *
 * // later...
 * unmount(app, { outro: true });
 * ```
 * @param {Record<string, any>} component
 * @param {{ outro?: boolean }} [options]
 * @returns {Promise<void>}
 */
function unmount(component, options) {
	const fn = mounted_components.get(component);

	if (fn) {
		mounted_components.delete(component);
		return fn(options);
	}

	return Promise.resolve();
}

/** @import { ComponentConstructorOptions, ComponentType, SvelteComponent, Component } from 'svelte' */

/**
 * Takes the component function and returns a Svelte 4 compatible component constructor.
 *
 * @deprecated Use this only as a temporary solution to migrate your imperative component code to Svelte 5.
 *
 * @template {Record<string, any>} Props
 * @template {Record<string, any>} Exports
 * @template {Record<string, any>} Events
 * @template {Record<string, any>} Slots
 *
 * @param {SvelteComponent<Props, Events, Slots> | Component<Props>} component
 * @returns {ComponentType<SvelteComponent<Props, Events, Slots> & Exports>}
 */
function asClassComponent$1(component) {
	// @ts-expect-error $$prop_def etc are not actually defined
	return class extends Svelte4Component {
		/** @param {any} options */
		constructor(options) {
			super({
				component,
				...options
			});
		}
	};
}

/**
 * Support using the component as both a class and function during the transition period
 * @typedef  {{new (o: ComponentConstructorOptions): SvelteComponent;(...args: Parameters<Component<Record<string, any>>>): ReturnType<Component<Record<string, any>, Record<string, any>>>;}} LegacyComponentType
 */

class Svelte4Component {
	/** @type {any} */
	#events;

	/** @type {Record<string, any>} */
	#instance;

	/**
	 * @param {ComponentConstructorOptions & {
	 *  component: any;
	 * }} options
	 */
	constructor(options) {
		var sources = new Map();

		/**
		 * @param {string | symbol} key
		 * @param {unknown} value
		 */
		var add_source = (key, value) => {
			var s = mutable_source(value, false, false);
			sources.set(key, s);
			return s;
		};

		// Replicate coarse-grained props through a proxy that has a version source for
		// each property, which is incremented on updates to the property itself. Do not
		// use our $state proxy because that one has fine-grained reactivity.
		const props = new Proxy(
			{ ...(options.props || {}), $$events: {} },
			{
				get(target, prop) {
					return get(sources.get(prop) ?? add_source(prop, Reflect.get(target, prop)));
				},
				has(target, prop) {
					// Necessary to not throw "invalid binding" validation errors on the component side
					if (prop === LEGACY_PROPS) return true;

					get(sources.get(prop) ?? add_source(prop, Reflect.get(target, prop)));
					return Reflect.has(target, prop);
				},
				set(target, prop, value) {
					set(sources.get(prop) ?? add_source(prop, value), value);
					return Reflect.set(target, prop, value);
				}
			}
		);

		this.#instance = (options.hydrate ? hydrate : mount)(options.component, {
			target: options.target,
			anchor: options.anchor,
			props,
			context: options.context,
			intro: options.intro ?? false,
			recover: options.recover,
			transformError: options.transformError
		});

		// We don't flushSync for custom element wrappers or if the user doesn't want it,
		// or if we're in async mode since `flushSync()` will fail
		if (!async_mode_flag && (!options?.props?.$$host || options.sync === false)) {
			flushSync();
		}

		this.#events = props.$$events;

		for (const key of Object.keys(this.#instance)) {
			if (key === '$set' || key === '$destroy' || key === '$on') continue;
			define_property(this, key, {
				get() {
					return this.#instance[key];
				},
				/** @param {any} value */
				set(value) {
					this.#instance[key] = value;
				},
				enumerable: true
			});
		}

		this.#instance.$set = /** @param {Record<string, any>} next */ (next) => {
			Object.assign(props, next);
		};

		this.#instance.$destroy = () => {
			unmount(this.#instance);
		};
	}

	/** @param {Record<string, any>} props */
	$set(props) {
		this.#instance.$set(props);
	}

	/**
	 * @param {string} event
	 * @param {(...args: any[]) => any} callback
	 * @returns {any}
	 */
	$on(event, callback) {
		this.#events[event] = this.#events[event] || [];

		/** @param {any[]} args */
		const cb = (...args) => callback.call(this, ...args);
		this.#events[event].push(cb);
		return () => {
			this.#events[event] = this.#events[event].filter(/** @param {any} fn */ (fn) => fn !== cb);
		};
	}

	$destroy() {
		this.#instance.$destroy();
	}
}

/** @import { SvelteComponent } from '../index.js' */
/** @import { Csp } from '#server' */

/** @typedef {{ head: string, html: string, css: { code: string, map: null }; hashes?: { script: `sha256-${string}`[] } }} LegacyRenderResult */

/**
 * Takes a Svelte 5 component and returns a Svelte 4 compatible component constructor.
 *
 * @deprecated Use this only as a temporary solution to migrate your imperative component code to Svelte 5.
 *
 * @template {Record<string, any>} Props
 * @template {Record<string, any>} Exports
 * @template {Record<string, any>} Events
 * @template {Record<string, any>} Slots
 *
 * @param {SvelteComponent<Props, Events, Slots>} component
 * @returns {typeof SvelteComponent<Props, Events, Slots> & Exports}
 */
function asClassComponent(component) {
	const component_constructor = asClassComponent$1(component);
	/** @type {(props?: {}, opts?: { $$slots?: {}; context?: Map<any, any>; csp?: Csp; transformError?: (error: unknown) => unknown }) => LegacyRenderResult & PromiseLike<LegacyRenderResult> } */
	const _render = (props, { context, csp, transformError } = {}) => {
		// @ts-expect-error the typings are off, but this will work if the component is compiled in SSR mode
		const result = render(component, { props, context, csp, transformError });

		const munged = Object.defineProperties(
			/** @type {LegacyRenderResult & PromiseLike<LegacyRenderResult>} */ ({}),
			{
				css: {
					value: { code: '', map: null }
				},
				head: {
					get: () => result.head
				},
				html: {
					get: () => result.body
				},
				then: {
					/**
					 * this is not type-safe, but honestly it's the best I can do right now, and it's a straightforward function.
					 *
					 * @template TResult1
					 * @template [TResult2=never]
					 * @param { (value: LegacyRenderResult) => TResult1 } onfulfilled
					 * @param { (reason: unknown) => TResult2 } onrejected
					 */
					value: (onfulfilled, onrejected) => {
						if (!async_mode_flag) {
							const user_result = onfulfilled({
								css: munged.css,
								head: munged.head,
								html: munged.html
							});
							return Promise.resolve(user_result);
						}

						return result.then((result) => {
							return onfulfilled({
								css: munged.css,
								head: result.head,
								html: result.body,
								hashes: result.hashes
							});
						}, onrejected);
					}
				}
			}
		);

		return munged;
	};

	// @ts-expect-error this is present for SSR
	component_constructor.render = _render;

	// @ts-ignore
	return component_constructor;
}

/**
 * Runs the given function once immediately on the server, and works like `$effect.pre` on the client.
 *
 * @deprecated Use this only as a temporary solution to migrate your component code to Svelte 5.
 * @param {() => void | (() => void)} fn
 * @returns {void}
 */
function run(fn) {
	fn();
}

var o=null;function s(e){o=e;}function l(i,o){i.component(i=>{let{stores:s,page:c,constructors:l,components:u=[],form:d,data_0:f=null,data_1:p=null}=o;e$1||setContext(`__svelte__`,s),e$1||s.page.set(c);let _=derived(()=>l[1]);if(l[1]){i.push(`<!--[0-->`);let e=l[0];e?(i.push(`<!--[-->`),e(i,{data:f,form:d,params:c.params,children:e=>{_()?(e.push(`<!--[-->`),_()(e,{data:p,form:d,params:c.params}),e.push(`<!--]-->`)):(e.push(`<!--[!-->`),e.push(`<!--]-->`));},$$slots:{default:true}}),i.push(`<!--]-->`)):(i.push(`<!--[!-->`),i.push(`<!--]-->`));}else {i.push(`<!--[-1-->`);let e=l[0];e?(i.push(`<!--[-->`),e(i,{data:f,form:d,params:c.params}),i.push(`<!--]-->`)):(i.push(`<!--[!-->`),i.push(`<!--]-->`));}i.push(`<!--]--> `),i.push(`<!--[-1-->`),i.push(`<!--]-->`);});}var u={app_template_contains_nonce:false,async:true,csp:{mode:`auto`,directives:{"upgrade-insecure-requests":false,"block-all-mixed-content":false},reportOnly:{"upgrade-insecure-requests":false,"block-all-mixed-content":false}},csrf_check_origin:true,csrf_trusted_origins:[],embedded:false,env_public_prefix:`PUBLIC_`,env_private_prefix:``,hash_routing:false,hooks:null,preload_strategy:`modulepreload`,root:asClassComponent(l),service_worker:false,service_worker_options:void 0,server_error_boundaries:false,templates:{app:({head:e,body:t,assets:n,nonce:r,env:i})=>`<!doctype html>
<html
	lang="en"
	style="
		margin: 0;
		padding: 0;
		min-height: 100%;
		display: flex;
		flex-direction: column;
	"
>
	<head>
		<meta charset="utf-8" />
		<link rel="icon" href="/favicon.ico" />
		<meta name="viewport" content="width=device-width, initial-scale=1" />
		<meta property="og:title" content="Gradio" />
		<meta property="og:type" content="website" />
		<meta property="og:url" content="{url}" />
		<meta property="og:description" content="Click to try out the app!" />
		<meta
			property="og:image"
			content="https://raw.githubusercontent.com/gradio-app/gradio/main/js/_website/src/lib/assets/img/header-image.jpg"
		/>
		<meta name="twitter:card" content="summary_large_image" />
		<meta name="twitter:creator" content="@Gradio" />
		<meta name="twitter:title" content="Gradio" />
		<meta name="twitter:description" content="Click to try out the app!" />
		<meta
			name="twitter:image"
			content="https://raw.githubusercontent.com/gradio-app/gradio/main/js/_website/src/lib/assets/img/header-image.jpg"
		/>
		<script data-gradio-mode>
			window.__gradio_mode__ = "app";
			window.iFrameResizer = {
				autoResize: false,
				sizeWidth: false,
				onReady: () =>
					window.dispatchEvent(new Event("gradio:iframe-resizer-ready"))
			};
			window.parent?.postMessage(
				{ type: "SET_SCROLLING", enabled: false },
				"*"
			);
		<\/script>
		<script src="static/js/iframeResizer.contentWindow.min.js" async><\/script>

		`+e+`
	</head>
	<body
		data-sveltekit-preload-data="hover"
		style="
			width: 100%;
			margin: 0;
			padding: 0;
			display: flex;
			flex-direction: column;
			flex-grow: 1;
		"
	>
		<div style="display: contents">`+t+`</div>
	</body>
</html>
`,error:({status:e,message:t})=>`<!doctype html>
<html lang="en">
	<head>
		<meta charset="utf-8" />
		<title>`+t+`</title>

		<style>
			body {
				--bg: white;
				--fg: #222;
				--divider: #ccc;
				background: var(--bg);
				color: var(--fg);
				font-family:
					system-ui,
					-apple-system,
					BlinkMacSystemFont,
					'Segoe UI',
					Roboto,
					Oxygen,
					Ubuntu,
					Cantarell,
					'Open Sans',
					'Helvetica Neue',
					sans-serif;
				display: flex;
				align-items: center;
				justify-content: center;
				height: 100vh;
				margin: 0;
			}

			.error {
				display: flex;
				align-items: center;
				max-width: 32rem;
				margin: 0 1rem;
			}

			.status {
				font-weight: 200;
				font-size: 3rem;
				line-height: 1;
				position: relative;
				top: -0.05rem;
			}

			.message {
				border-left: 1px solid var(--divider);
				padding: 0 0 0 1rem;
				margin: 0 0 0 1rem;
				min-height: 2.5rem;
				display: flex;
				align-items: center;
			}

			.message h1 {
				font-weight: 400;
				font-size: 1em;
				margin: 0;
			}

			@media (prefers-color-scheme: dark) {
				body {
					--bg: #222;
					--fg: #ddd;
					--divider: #666;
				}
			}
		</style>
	</head>
	<body>
		<div class="error">
			<span class="status">`+e+`</span>
			<div class="message">
				<h1>`+t+`</h1>
			</div>
		</div>
	</body>
</html>
`},version_hash:`ow8a2e`};async function d(){return {handle:void 0,handleFetch:void 0,handleError:void 0,handleValidationError:void 0,init:void 0,reroute:void 0,transport:void 0}}

export { $, R as A, B, s$2 as C, f as D, E, F, g as G, P$1 as H, I, o as J, F$1 as K, L, s as M, m as N, try_get_request_store as O, P, i$1 as Q, R$1 as R, S, a$1 as T, U, V, n$1 as W, w as X, n as Y, a$2 as Z, _, u as a, z as a0, b as a1, H as a2, W as a3, le as a4, G as a5, j$1 as a6, ue as a7, I$1 as a8, run as a9, b$1 as b, c$2 as c, c$1 as d, l$2 as e, f$1 as f, g$1 as g, h$1 as h, i$2 as i, d as j, k$1 as k, l$3 as l, l$1 as m, a as n, e as o, p$1 as p, d$1 as q, ie as r, s$1 as s, t, u$2 as u, v$1 as v, with_request_store as w, v as x, o$1 as y, u$1 as z };
//# sourceMappingURL=internal-DQeF4eWE.js.map
