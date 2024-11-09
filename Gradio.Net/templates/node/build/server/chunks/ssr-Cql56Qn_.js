import { S as SvelteComponent, t as tick } from './Component-Dv7eSVA_.js';
import { a as createEventDispatcher, y as getAllContexts, g as getContext, z as hasContext, o as onDestroy, p as setContext } from './ssr-RaXq3SJh.js';

/**
 * Base class for Svelte components with some minor dev-enhancements. Used when dev=true.
 *
 * Can be used to create strongly typed Svelte components.
 *
 * #### Example:
 *
 * You have component library on npm called `component-library`, from which
 * you export a component called `MyComponent`. For Svelte+TypeScript users,
 * you want to provide typings. Therefore you create a `index.d.ts`:
 * ```ts
 * import { SvelteComponent } from "svelte";
 * export class MyComponent extends SvelteComponent<{foo: string}> {}
 * ```
 * Typing this makes it possible for IDEs like VS Code with the Svelte extension
 * to provide intellisense and to use the component like this in a Svelte file
 * with TypeScript:
 * ```svelte
 * <script lang="ts">
 * 	import { MyComponent } from "component-library";
 * </script>
 * <MyComponent foo={'bar'} />
 * ```
 * @template {Record<string, any>} [Props=any]
 * @template {Record<string, any>} [Events=any]
 * @template {Record<string, any>} [Slots=any]
 * @extends {SvelteComponent<Props, Events>}
 */
class SvelteComponentDev extends SvelteComponent {
	/**
	 * For type checking capabilities only.
	 * Does not exist at runtime.
	 * ### DO NOT USE!
	 *
	 * @type {Props}
	 */
	$$prop_def;
	/**
	 * For type checking capabilities only.
	 * Does not exist at runtime.
	 * ### DO NOT USE!
	 *
	 * @type {Events}
	 */
	$$events_def;
	/**
	 * For type checking capabilities only.
	 * Does not exist at runtime.
	 * ### DO NOT USE!
	 *
	 * @type {Slots}
	 */
	$$slot_def;

	/** @param {import('./public.js').ComponentConstructorOptions<Props>} options */
	constructor(options) {
		if (!options || (!options.target && !options.$$inline)) {
			throw new Error("'target' is a required option");
		}
		super();
	}

	/** @returns {void} */
	$destroy() {
		super.$destroy();
		this.$destroy = () => {
			console.warn('Component was already destroyed'); // eslint-disable-line no-console
		};
	}

	/** @returns {void} */
	$capture_state() {}

	/** @returns {void} */
	$inject_state() {}
}
/**
 * @template {Record<string, any>} [Props=any]
 * @template {Record<string, any>} [Events=any]
 * @template {Record<string, any>} [Slots=any]
 * @deprecated Use `SvelteComponent` instead. See PR for more information: https://github.com/sveltejs/svelte/pull/8512
 * @extends {SvelteComponentDev<Props, Events, Slots>}
 */
class SvelteComponentTyped extends SvelteComponentDev {}

/** @returns {void} */
function onMount() {}

/** @returns {void} */
function beforeUpdate() {}

/** @returns {void} */
function afterUpdate() {}

var svelte = /*#__PURE__*/Object.freeze({
	__proto__: null,
	SvelteComponent: SvelteComponentDev,
	SvelteComponentTyped: SvelteComponentTyped,
	afterUpdate: afterUpdate,
	beforeUpdate: beforeUpdate,
	createEventDispatcher: createEventDispatcher,
	getAllContexts: getAllContexts,
	getContext: getContext,
	hasContext: hasContext,
	onDestroy: onDestroy,
	onMount: onMount,
	setContext: setContext,
	tick: tick
});

export { SvelteComponentDev as S, afterUpdate as a, svelte as s };
//# sourceMappingURL=ssr-Cql56Qn_.js.map
