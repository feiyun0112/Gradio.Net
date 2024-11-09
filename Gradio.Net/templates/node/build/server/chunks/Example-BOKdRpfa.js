import { c as create_ssr_component, e as escape } from './ssr-RaXq3SJh.js';

const Example = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  return `${escape(value || "")}`;
});

export { Example as default };
//# sourceMappingURL=Example-BOKdRpfa.js.map
