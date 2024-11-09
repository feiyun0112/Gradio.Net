import { c as create_ssr_component, v as validate_component, a as createEventDispatcher, e as escape, n as null_to_empty, b as add_attribute, f as each, d as add_styles, o as onDestroy, m as missing_component } from './ssr-RaXq3SJh.js';
import { B as Block, S as Static, e as BlockLabel, C as Chat, h as IconButtonWrapper, i as IconButton, z as Community, A as Trash, G as MarkdownCode, H as Copy, J as ScrollDownArrow, R as Retry, v as Undo } from './2-B6LMYTAg.js';
import { I as Image$1 } from './Image-DFqHtuJN.js';
import { D as DownloadLink } from './DownloadLink--4obEanq.js';
import { d as dequal } from './index6-sfNUnwRZ.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

const redirect_src_url = (src, root) => src.replace('src="/file', `src="${root}file`);
function get_component_for_mime_type(mime_type) {
  if (!mime_type)
    return "file";
  if (mime_type.includes("audio"))
    return "audio";
  if (mime_type.includes("video"))
    return "video";
  if (mime_type.includes("image"))
    return "image";
  return "file";
}
function convert_file_message_to_component_message(message) {
  const _file = Array.isArray(message.file) ? message.file[0] : message.file;
  return {
    component: get_component_for_mime_type(_file?.mime_type),
    value: message.file,
    alt_text: message.alt_text,
    constructor_args: {},
    props: {}
  };
}
function normalise_messages(messages, root) {
  if (messages === null)
    return messages;
  return messages.map((message, i) => {
    if (typeof message.content === "string") {
      return {
        role: message.role,
        metadata: message.metadata,
        content: redirect_src_url(message.content, root),
        type: "text",
        index: i
      };
    } else if ("file" in message.content) {
      return {
        content: convert_file_message_to_component_message(message.content),
        metadata: message.metadata,
        role: message.role,
        type: "component",
        index: i
      };
    }
    return { type: "component", ...message };
  });
}
function normalise_tuples(messages, root) {
  if (messages === null)
    return messages;
  const msg = messages.flatMap((message_pair, i) => {
    return message_pair.map((message, index) => {
      if (message == null)
        return null;
      const role = index == 0 ? "user" : "assistant";
      if (typeof message === "string") {
        return {
          role,
          type: "text",
          content: redirect_src_url(message, root),
          metadata: { title: null },
          index: [i, index]
        };
      }
      if ("file" in message) {
        return {
          content: convert_file_message_to_component_message(message),
          role,
          type: "component",
          index: [i, index]
        };
      }
      return {
        role,
        content: message,
        type: "component",
        index: [i, index]
      };
    });
  });
  return msg.filter((message) => message != null);
}
function is_component_message(message) {
  return message.type === "component";
}
function is_last_bot_message(messages, all_messages) {
  const is_bot = messages[messages.length - 1].role === "assistant";
  const last_index = messages[messages.length - 1].index;
  const is_last = JSON.stringify(last_index) === JSON.stringify(all_messages[all_messages.length - 1].index);
  return is_last && is_bot;
}
function group_messages(messages, msg_format) {
  const groupedMessages = [];
  let currentGroup = [];
  let currentRole = null;
  for (const message of messages) {
    if (msg_format === "tuples") {
      currentRole = null;
    }
    if (!(message.role === "assistant" || message.role === "user")) {
      continue;
    }
    if (message.role === currentRole) {
      currentGroup.push(message);
    } else {
      if (currentGroup.length > 0) {
        groupedMessages.push(currentGroup);
      }
      currentGroup = [message];
      currentRole = message.role;
    }
  }
  if (currentGroup.length > 0) {
    groupedMessages.push(currentGroup);
  }
  return groupedMessages;
}
async function load_components(component_names, _components, load_component) {
  let names = [];
  let components = [];
  component_names.forEach((component_name) => {
    if (_components[component_name] || component_name === "file") {
      return;
    }
    const { name, component } = load_component(component_name, "base");
    names.push(name);
    components.push(component);
  });
  const loaded_components = await Promise.all(components);
  loaded_components.forEach((component, i) => {
    _components[names[i]] = component.default;
  });
  return _components;
}
function get_components_from_messages(messages) {
  if (!messages)
    return [];
  let components = /* @__PURE__ */ new Set();
  messages.forEach((message) => {
    if (message.type === "component") {
      components.add(message.content.component);
    }
  });
  return Array.from(components);
}
const Component = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { type } = $$props;
  let { components } = $$props;
  let { value } = $$props;
  let { target } = $$props;
  let { theme_mode } = $$props;
  let { props } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  let { _fetch } = $$props;
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.components === void 0 && $$bindings.components && components !== void 0)
    $$bindings.components(components);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.target === void 0 && $$bindings.target && target !== void 0)
    $$bindings.target(target);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.props === void 0 && $$bindings.props && props !== void 0)
    $$bindings.props(props);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props._fetch === void 0 && $$bindings._fetch && _fetch !== void 0)
    $$bindings._fetch(_fetch);
  return `${type === "gallery" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      value,
      show_label: false,
      i18n,
      label: "",
      _fetch,
      allow_preview: false,
      interactive: false,
      mode: "minimal",
      fixed_height: 1
    },
    {},
    {}
  )}` : `${type === "plot" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      value,
      target,
      theme_mode,
      bokeh_version: props.bokeh_version,
      caption: "",
      show_actions_button: true
    },
    {},
    {}
  )}` : `${type === "audio" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      value,
      show_label: false,
      show_share_button: true,
      i18n,
      label: "",
      waveform_settings: {},
      waveform_options: {},
      show_download_button: false
    },
    {},
    {}
  )}` : `${type === "video" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      autoplay: true,
      value: value.video || value,
      show_label: false,
      show_share_button: true,
      i18n,
      upload,
      show_download_button: false
    },
    {},
    {
      default: () => {
        return `<track kind="captions">`;
      }
    }
  )}` : `${type === "image" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      value,
      show_label: false,
      label: "chatbot-image",
      show_download_button: false,
      i18n
    },
    {},
    {}
  )}` : `${type === "html" ? `${validate_component(components[type] || missing_component, "svelte:component").$$render(
    $$result,
    {
      value,
      show_label: false,
      label: "chatbot-image",
      show_share_button: true,
      i18n,
      gradio: {
        dispatch: () => {
        }
      }
    },
    {},
    {}
  )}` : ``}`}`}`}`}`}`;
});
const css$5 = {
  code: ".box.svelte-1e60bn1{border-radius:4px;cursor:pointer;max-width:max-content;background:var(--color-accent-soft);border:1px solid var(--border-color-accent-subdued);font-size:0.8em}.title.svelte-1e60bn1{display:flex;align-items:center;padding:3px 6px;color:var(--body-text-color);opacity:0.8}.content.svelte-1e60bn1{padding:4px 8px}.content.svelte-1e60bn1 *{font-size:0.8em}.title-text.svelte-1e60bn1{padding-right:var(--spacing-lg)}.arrow.svelte-1e60bn1{margin-left:auto;opacity:0.8}",
  map: '{"version":3,"file":"MessageBox.svelte","sources":["MessageBox.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let expanded = false;\\nexport let title;\\nfunction toggleExpanded() {\\n    expanded = !expanded;\\n}\\n<\/script>\\n\\n<button class=\\"box\\" on:click={toggleExpanded}>\\n\\t<div class=\\"title\\">\\n\\t\\t<span class=\\"title-text\\">{title}</span>\\n\\t\\t<span\\n\\t\\t\\tstyle:transform={expanded ? \\"rotate(0)\\" : \\"rotate(90deg)\\"}\\n\\t\\t\\tclass=\\"arrow\\"\\n\\t\\t>\\n\\t\\t\\t▼\\n\\t\\t</span>\\n\\t</div>\\n\\t{#if expanded}\\n\\t\\t<div class=\\"content\\">\\n\\t\\t\\t<slot></slot>\\n\\t\\t</div>\\n\\t{/if}\\n</button>\\n\\n<style>\\n\\t.box {\\n\\t\\tborder-radius: 4px;\\n\\t\\tcursor: pointer;\\n\\t\\tmax-width: max-content;\\n\\t\\tbackground: var(--color-accent-soft);\\n\\t\\tborder: 1px solid var(--border-color-accent-subdued);\\n\\t\\tfont-size: 0.8em;\\n\\t}\\n\\n\\t.title {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tpadding: 3px 6px;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\topacity: 0.8;\\n\\t}\\n\\n\\t.content {\\n\\t\\tpadding: 4px 8px;\\n\\t}\\n\\n\\t.content :global(*) {\\n\\t\\tfont-size: 0.8em;\\n\\t}\\n\\n\\t.title-text {\\n\\t\\tpadding-right: var(--spacing-lg);\\n\\t}\\n\\n\\t.arrow {\\n\\t\\tmargin-left: auto;\\n\\t\\topacity: 0.8;\\n\\t}</style>\\n"],"names":[],"mappings":"AAyBC,mBAAK,CACJ,aAAa,CAAE,GAAG,CAClB,MAAM,CAAE,OAAO,CACf,SAAS,CAAE,WAAW,CACtB,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,6BAA6B,CAAC,CACpD,SAAS,CAAE,KACZ,CAEA,qBAAO,CACN,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,OAAO,CAAE,GAAG,CAAC,GAAG,CAChB,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,OAAO,CAAE,GACV,CAEA,uBAAS,CACR,OAAO,CAAE,GAAG,CAAC,GACd,CAEA,uBAAQ,CAAS,CAAG,CACnB,SAAS,CAAE,KACZ,CAEA,0BAAY,CACX,aAAa,CAAE,IAAI,YAAY,CAChC,CAEA,qBAAO,CACN,WAAW,CAAE,IAAI,CACjB,OAAO,CAAE,GACV"}'
};
const MessageBox = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { expanded = false } = $$props;
  let { title } = $$props;
  if ($$props.expanded === void 0 && $$bindings.expanded && expanded !== void 0)
    $$bindings.expanded(expanded);
  if ($$props.title === void 0 && $$bindings.title && title !== void 0)
    $$bindings.title(title);
  $$result.css.add(css$5);
  return `<button class="box svelte-1e60bn1"><div class="title svelte-1e60bn1"><span class="title-text svelte-1e60bn1">${escape(title)}</span> <span class="arrow svelte-1e60bn1"${add_styles({
    "transform": expanded ? "rotate(0)" : "rotate(90deg)"
  })} data-svelte-h="svelte-15ydlzc">▼</span></div> ${expanded ? `<div class="content svelte-1e60bn1">${slots.default ? slots.default({}) : ``}</div>` : ``} </button>`;
});
const ThumbDownDefault = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  return `<svg width="100%" height="100%" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M2.25 8.11523H4.5V10.3652C4.5003 10.6635 4.61892 10.9495 4.82983 11.1604C5.04075 11.3713 5.32672 11.4899 5.625 11.4902H6.42488C6.60519 11.4895 6.77926 11.4241 6.91549 11.3059C7.05172 11.1878 7.14109 11.0248 7.16737 10.8464L7.48425 8.62748L8.82562 6.61523H11.25V1.36523H3.375C2.67911 1.36623 2.01201 1.64311 1.51994 2.13517C1.02787 2.62724 0.750992 3.29435 0.75 3.99023V6.61523C0.750496 7.01291 0.908691 7.39415 1.18989 7.67535C1.47109 7.95654 1.85233 8.11474 2.25 8.11523ZM9 2.11523H10.5V5.86523H9V2.11523ZM1.5 3.99023C1.5006 3.49314 1.69833 3.01657 2.04983 2.66507C2.40133 2.31356 2.8779 2.11583 3.375 2.11523H8.25V6.12661L6.76575 8.35298L6.4245 10.7402H5.625C5.52554 10.7402 5.43016 10.7007 5.35983 10.6304C5.28951 10.5601 5.25 10.4647 5.25 10.3652V7.36523H2.25C2.05118 7.36494 1.86059 7.28582 1.72 7.14524C1.57941 7.00465 1.5003 6.81406 1.5 6.61523V3.99023Z" fill="currentColor"></path></svg>`;
});
const ThumbUpDefault = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  return `<svg width="100%" height="100%" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M9.75 4.74023H7.5V2.49023C7.4997 2.19196 7.38108 1.90598 7.17017 1.69507C6.95925 1.48415 6.67328 1.36553 6.375 1.36523H5.57512C5.39481 1.366 5.22074 1.43138 5.08451 1.54952C4.94828 1.66766 4.85891 1.83072 4.83262 2.00911L4.51575 4.22798L3.17438 6.24023H0.75V11.4902H8.625C9.32089 11.4892 9.98799 11.2124 10.4801 10.7203C10.9721 10.2282 11.249 9.56112 11.25 8.86523V6.24023C11.2495 5.84256 11.0913 5.46132 10.8101 5.18012C10.5289 4.89893 10.1477 4.74073 9.75 4.74023ZM3 10.7402H1.5V6.99023H3V10.7402ZM10.5 8.86523C10.4994 9.36233 10.3017 9.8389 9.95017 10.1904C9.59867 10.5419 9.1221 10.7396 8.625 10.7402H3.75V6.72886L5.23425 4.50248L5.5755 2.11523H6.375C6.47446 2.11523 6.56984 2.15474 6.64017 2.22507C6.71049 2.2954 6.75 2.39078 6.75 2.49023V5.49023H9.75C9.94882 5.49053 10.1394 5.56965 10.28 5.71023C10.4206 5.85082 10.4997 6.04141 10.5 6.24023V8.86523Z" fill="currentColor"></path></svg>`;
});
const LikeDislike = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { handle_action } = $$props;
  if ($$props.handle_action === void 0 && $$bindings.handle_action && handle_action !== void 0)
    $$bindings.handle_action(handle_action);
  return `${validate_component(IconButton, "IconButton").$$render(
    $$result,
    {
      Icon: ThumbDownDefault,
      label: "dislike",
      color: "var(--block-label-text-color)"
    },
    {},
    {}
  )} ${validate_component(IconButton, "IconButton").$$render(
    $$result,
    {
      Icon: ThumbUpDefault,
      label: "like",
      color: "var(--block-label-text-color)"
    },
    {},
    {}
  )}`;
});
const Copy_1 = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  onDestroy(() => {
  });
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  return `${validate_component(IconButton, "IconButton").$$render(
    $$result,
    {
      label: "Copy message",
      Icon: Copy
    },
    {},
    {}
  )}`;
});
const Download = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  return `<svg width="16" height="16" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6.27701 8.253C6.24187 8.29143 6.19912 8.32212 6.15147 8.34311C6.10383 8.36411 6.05233 8.37495 6.00026 8.37495C5.94819 8.37495 5.89669 8.36411 5.84905 8.34311C5.8014 8.32212 5.75865 8.29143 5.72351 8.253L3.72351 6.0655C3.65798 5.99185 3.62408 5.89536 3.62916 5.79691C3.63424 5.69846 3.67788 5.60596 3.75064 5.53945C3.8234 5.47293 3.91943 5.43774 4.01794 5.44149C4.11645 5.44525 4.20952 5.48764 4.27701 5.5595L5.62501 7.0345V1.5C5.62501 1.40054 5.66452 1.30516 5.73485 1.23483C5.80517 1.16451 5.90055 1.125 6.00001 1.125C6.09947 1.125 6.19485 1.16451 6.26517 1.23483C6.3355 1.30516 6.37501 1.40054 6.37501 1.5V7.034L7.72351 5.559C7.79068 5.4856 7.88425 5.44189 7.98364 5.43748C8.08304 5.43308 8.18011 5.46833 8.25351 5.5355C8.32691 5.60267 8.37062 5.69624 8.37503 5.79563C8.37943 5.89503 8.34418 5.9921 8.27701 6.0655L6.27701 8.253Z" fill="currentColor"></path><path d="M1.875 7.39258C1.875 7.29312 1.83549 7.19774 1.76517 7.12741C1.69484 7.05709 1.59946 7.01758 1.5 7.01758C1.40054 7.01758 1.30516 7.05709 1.23483 7.12741C1.16451 7.19774 1.125 7.29312 1.125 7.39258V7.42008C1.125 8.10358 1.125 8.65508 1.1835 9.08858C1.2435 9.53858 1.3735 9.91758 1.674 10.2186C1.975 10.5196 2.354 10.6486 2.804 10.7096C3.2375 10.7676 3.789 10.7676 4.4725 10.7676H7.5275C8.211 10.7676 8.7625 10.7676 9.196 10.7096C9.646 10.6486 10.025 10.5196 10.326 10.2186C10.627 9.91758 10.756 9.53858 10.817 9.08858C10.875 8.65508 10.875 8.10358 10.875 7.42008V7.39258C10.875 7.29312 10.8355 7.19774 10.7652 7.12741C10.6948 7.05709 10.5995 7.01758 10.5 7.01758C10.4005 7.01758 10.3052 7.05709 10.2348 7.12741C10.1645 7.19774 10.125 7.29312 10.125 7.39258C10.125 8.11008 10.124 8.61058 10.0735 8.98858C10.024 9.35558 9.9335 9.54958 9.7955 9.68808C9.657 9.82658 9.463 9.91658 9.0955 9.96608C8.718 10.0166 8.2175 10.0176 7.5 10.0176H4.5C3.7825 10.0176 3.2815 10.0166 2.904 9.96608C2.537 9.91658 2.343 9.82608 2.2045 9.68808C2.066 9.54958 1.976 9.35558 1.9265 8.98808C1.876 8.61058 1.875 8.11008 1.875 7.39258Z" fill="currentColor"></path></svg>`;
});
const css$4 = {
  code: ".bubble.svelte-1ibfe7l .icon-button-wrapper{margin:0px calc(var(--spacing-xl) * 2)}.message-buttons-left.svelte-1ibfe7l{align-self:flex-start}.bubble.message-buttons-right.svelte-1ibfe7l{align-self:flex-end}.message-buttons-right.svelte-1ibfe7l .icon-button-wrapper{margin-left:auto}.bubble.with-avatar.svelte-1ibfe7l{margin-left:calc(var(--spacing-xl) * 5);margin-right:calc(var(--spacing-xl) * 5)}.panel.svelte-1ibfe7l{display:flex;align-self:flex-start;padding:0 var(--spacing-xl);z-index:var(--layer-1)}",
  map: `{"version":3,"file":"ButtonPanel.svelte","sources":["ButtonPanel.svelte"],"sourcesContent":["<script lang=\\"ts\\">import LikeDislike from \\"./LikeDislike.svelte\\";\\nimport Copy from \\"./Copy.svelte\\";\\nimport DownloadIcon from \\"./Download.svelte\\";\\nimport { DownloadLink } from \\"@gradio/wasm/svelte\\";\\nimport { is_component_message } from \\"./utils\\";\\nimport { Retry, Undo } from \\"@gradio/icons\\";\\nimport { IconButtonWrapper, IconButton } from \\"@gradio/atoms\\";\\nexport let likeable;\\nexport let show_retry;\\nexport let show_undo;\\nexport let show_copy_button;\\nexport let show;\\nexport let message;\\nexport let position;\\nexport let avatar;\\nexport let generating;\\nexport let handle_action;\\nexport let layout;\\nfunction is_all_text(message2) {\\n    return Array.isArray(message2) && message2.every((m) => typeof m.content === \\"string\\") || !Array.isArray(message2) && typeof message2.content === \\"string\\";\\n}\\nfunction all_text(message2) {\\n    if (Array.isArray(message2)) {\\n        return message2.map((m) => m.content).join(\\"\\\\n\\");\\n    }\\n    return message2.content;\\n}\\n$: message_text = is_all_text(message) ? all_text(message) : \\"\\";\\n$: show_copy = show_copy_button && message && is_all_text(message);\\n$: show_download = !Array.isArray(message) && is_component_message(message) && message.content.value?.url;\\n<\/script>\\n\\n{#if show}\\n\\t<div\\n\\t\\tclass=\\"message-buttons-{position} {layout} message-buttons {avatar !==\\n\\t\\t\\tnull && 'with-avatar'}\\"\\n\\t>\\n\\t\\t<IconButtonWrapper top_panel={false}>\\n\\t\\t\\t{#if show_copy}\\n\\t\\t\\t\\t<Copy value={message_text} />\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if show_download && !Array.isArray(message) && is_component_message(message)}\\n\\t\\t\\t\\t<DownloadLink\\n\\t\\t\\t\\t\\thref={message?.content?.value.url}\\n\\t\\t\\t\\t\\tdownload={message.content.value.orig_name || \\"image\\"}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<IconButton Icon={DownloadIcon} />\\n\\t\\t\\t\\t</DownloadLink>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if show_retry}\\n\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\tIcon={Retry}\\n\\t\\t\\t\\t\\tlabel=\\"Retry\\"\\n\\t\\t\\t\\t\\ton:click={() => handle_action(\\"retry\\")}\\n\\t\\t\\t\\t\\tdisabled={generating}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if show_undo}\\n\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\tlabel=\\"Undo\\"\\n\\t\\t\\t\\t\\tIcon={Undo}\\n\\t\\t\\t\\t\\ton:click={() => handle_action(\\"undo\\")}\\n\\t\\t\\t\\t\\tdisabled={generating}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if likeable}\\n\\t\\t\\t\\t<LikeDislike {handle_action} />\\n\\t\\t\\t{/if}\\n\\t\\t</IconButtonWrapper>\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.bubble :global(.icon-button-wrapper) {\\n\\t\\tmargin: 0px calc(var(--spacing-xl) * 2);\\n\\t}\\n\\n\\t.message-buttons-left {\\n\\t\\talign-self: flex-start;\\n\\t}\\n\\n\\t.bubble.message-buttons-right {\\n\\t\\talign-self: flex-end;\\n\\t}\\n\\n\\t.message-buttons-right :global(.icon-button-wrapper) {\\n\\t\\tmargin-left: auto;\\n\\t}\\n\\n\\t.bubble.with-avatar {\\n\\t\\tmargin-left: calc(var(--spacing-xl) * 5);\\n\\t\\tmargin-right: calc(var(--spacing-xl) * 5);\\n\\t}\\n\\n\\t.panel {\\n\\t\\tdisplay: flex;\\n\\t\\talign-self: flex-start;\\n\\t\\tpadding: 0 var(--spacing-xl);\\n\\t\\tz-index: var(--layer-1);\\n\\t}</style>\\n"],"names":[],"mappings":"AAyEC,sBAAO,CAAS,oBAAsB,CACrC,MAAM,CAAE,GAAG,CAAC,KAAK,IAAI,YAAY,CAAC,CAAC,CAAC,CAAC,CAAC,CACvC,CAEA,oCAAsB,CACrB,UAAU,CAAE,UACb,CAEA,OAAO,qCAAuB,CAC7B,UAAU,CAAE,QACb,CAEA,qCAAsB,CAAS,oBAAsB,CACpD,WAAW,CAAE,IACd,CAEA,OAAO,2BAAa,CACnB,WAAW,CAAE,KAAK,IAAI,YAAY,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CACxC,YAAY,CAAE,KAAK,IAAI,YAAY,CAAC,CAAC,CAAC,CAAC,CAAC,CACzC,CAEA,qBAAO,CACN,OAAO,CAAE,IAAI,CACb,UAAU,CAAE,UAAU,CACtB,OAAO,CAAE,CAAC,CAAC,IAAI,YAAY,CAAC,CAC5B,OAAO,CAAE,IAAI,SAAS,CACvB"}`
};
function is_all_text(message2) {
  return Array.isArray(message2) && message2.every((m) => typeof m.content === "string") || !Array.isArray(message2) && typeof message2.content === "string";
}
function all_text(message2) {
  if (Array.isArray(message2)) {
    return message2.map((m) => m.content).join("\n");
  }
  return message2.content;
}
const ButtonPanel = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let message_text;
  let show_copy;
  let show_download;
  let { likeable } = $$props;
  let { show_retry } = $$props;
  let { show_undo } = $$props;
  let { show_copy_button } = $$props;
  let { show } = $$props;
  let { message } = $$props;
  let { position } = $$props;
  let { avatar } = $$props;
  let { generating } = $$props;
  let { handle_action } = $$props;
  let { layout } = $$props;
  if ($$props.likeable === void 0 && $$bindings.likeable && likeable !== void 0)
    $$bindings.likeable(likeable);
  if ($$props.show_retry === void 0 && $$bindings.show_retry && show_retry !== void 0)
    $$bindings.show_retry(show_retry);
  if ($$props.show_undo === void 0 && $$bindings.show_undo && show_undo !== void 0)
    $$bindings.show_undo(show_undo);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.show === void 0 && $$bindings.show && show !== void 0)
    $$bindings.show(show);
  if ($$props.message === void 0 && $$bindings.message && message !== void 0)
    $$bindings.message(message);
  if ($$props.position === void 0 && $$bindings.position && position !== void 0)
    $$bindings.position(position);
  if ($$props.avatar === void 0 && $$bindings.avatar && avatar !== void 0)
    $$bindings.avatar(avatar);
  if ($$props.generating === void 0 && $$bindings.generating && generating !== void 0)
    $$bindings.generating(generating);
  if ($$props.handle_action === void 0 && $$bindings.handle_action && handle_action !== void 0)
    $$bindings.handle_action(handle_action);
  if ($$props.layout === void 0 && $$bindings.layout && layout !== void 0)
    $$bindings.layout(layout);
  $$result.css.add(css$4);
  message_text = is_all_text(message) ? all_text(message) : "";
  show_copy = show_copy_button && message && is_all_text(message);
  show_download = !Array.isArray(message) && is_component_message(message) && message.content.value?.url;
  return `${show ? `<div class="${"message-buttons-" + escape(position, true) + " " + escape(layout, true) + " message-buttons " + escape(avatar !== null && "with-avatar", true) + " svelte-1ibfe7l"}">${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, { top_panel: false }, {}, {
    default: () => {
      return `${show_copy ? `${validate_component(Copy_1, "Copy").$$render($$result, { value: message_text }, {}, {})}` : ``} ${show_download && !Array.isArray(message) && is_component_message(message) ? `${validate_component(DownloadLink, "DownloadLink").$$render(
        $$result,
        {
          href: message?.content?.value.url,
          download: message.content.value.orig_name || "image"
        },
        {},
        {
          default: () => {
            return `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Download }, {}, {})}`;
          }
        }
      )}` : ``} ${show_retry ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          Icon: Retry,
          label: "Retry",
          disabled: generating
        },
        {},
        {}
      )}` : ``} ${show_undo ? `${validate_component(IconButton, "IconButton").$$render(
        $$result,
        {
          label: "Undo",
          Icon: Undo,
          disabled: generating
        },
        {},
        {}
      )}` : ``} ${likeable ? `${validate_component(LikeDislike, "LikeDislike").$$render($$result, { handle_action }, {}, {})}` : ``}`;
    }
  })}</div>` : ``}`;
});
const css$3 = {
  code: ".message.svelte-5ng3n.svelte-5ng3n{position:relative;width:100%}.avatar-container.svelte-5ng3n.svelte-5ng3n{flex-shrink:0;border-radius:50%;border:1px solid var(--border-color-primary);overflow:hidden}.avatar-container.svelte-5ng3n img{object-fit:cover}.flex-wrap.svelte-5ng3n.svelte-5ng3n{display:flex;flex-direction:column;width:calc(100% - var(--spacing-xxl));max-width:100%;color:var(--body-text-color);font-size:var(--chatbot-text-size);overflow-wrap:break-word;width:100%;height:100%}.component.svelte-5ng3n.svelte-5ng3n{padding:0;border-radius:var(--radius-md);width:fit-content;overflow:hidden}.component.gallery.svelte-5ng3n.svelte-5ng3n{border:none}.message-row.svelte-5ng3n .svelte-5ng3n:not(.avatar-container) img{margin:var(--size-2);max-height:300px}.file-pil.svelte-5ng3n.svelte-5ng3n{display:block;width:fit-content;padding:var(--spacing-sm) var(--spacing-lg);border-radius:var(--radius-md);background:var(--background-fill-secondary);color:var(--body-text-color);text-decoration:none;margin:0;font-family:var(--font-mono);font-size:var(--text-sm)}.file.svelte-5ng3n.svelte-5ng3n{width:auto !important;max-width:fit-content !important}@media(max-width: 600px) or (max-width: 480px){.component.svelte-5ng3n.svelte-5ng3n{width:100%}}.message.svelte-5ng3n .prose{font-size:var(--chatbot-text-size)}.message-bubble-border.svelte-5ng3n.svelte-5ng3n{border-width:1px;border-radius:var(--radius-md)}.message-fit.svelte-5ng3n.svelte-5ng3n{width:fit-content !important}.panel-full-width.svelte-5ng3n.svelte-5ng3n{width:100%}.message-markdown-disabled.svelte-5ng3n.svelte-5ng3n{white-space:pre-line}.user.svelte-5ng3n.svelte-5ng3n{border-width:1px;border-radius:var(--radius-md);align-self:flex-start;border-bottom-right-radius:0;box-shadow:var(--shadow-drop);align-self:flex-start;text-align:right;padding:var(--spacing-sm) var(--spacing-xl);border-color:var(--border-color-accent-subdued);background-color:var(--color-accent-soft)}.bot.svelte-5ng3n.svelte-5ng3n{border-width:1px;border-radius:var(--radius-lg);border-bottom-left-radius:0;border-color:var(--border-color-primary);background-color:var(--background-fill-secondary);box-shadow:var(--shadow-drop);align-self:flex-start;text-align:right;padding:var(--spacing-sm) var(--spacing-xl)}.panel.svelte-5ng3n .user.svelte-5ng3n *{text-align:right}.bubble.svelte-5ng3n .bot.svelte-5ng3n{border-color:var(--border-color-primary)}.message-row.svelte-5ng3n.svelte-5ng3n{display:flex;position:relative}.bubble.svelte-5ng3n.svelte-5ng3n{margin:calc(var(--spacing-xl) * 2);margin-bottom:var(--spacing-xl)}.bubble.user-row.svelte-5ng3n.svelte-5ng3n{align-self:flex-end;max-width:calc(100% - var(--spacing-xl) * 6)}.bubble.bot-row.svelte-5ng3n.svelte-5ng3n{align-self:flex-start;max-width:calc(100% - var(--spacing-xl) * 6)}.bubble.svelte-5ng3n .user-row.svelte-5ng3n{flex-direction:row;justify-content:flex-end}.bubble.svelte-5ng3n .with_avatar.user-row.svelte-5ng3n{margin-right:calc(var(--spacing-xl) * 2) !important}.bubble.svelte-5ng3n .with_avatar.bot-row.svelte-5ng3n{margin-left:calc(var(--spacing-xl) * 2) !important}.bubble.svelte-5ng3n .with_opposite_avatar.user-row.svelte-5ng3n{margin-left:calc(var(--spacing-xxl) + 35px + var(--spacing-xxl))}.bubble.svelte-5ng3n .message-fit.svelte-5ng3n{width:fit-content !important}.panel.svelte-5ng3n.svelte-5ng3n{margin:0;padding:calc(var(--spacing-lg) * 2) calc(var(--spacing-lg) * 2)}.panel.bot-row.svelte-5ng3n.svelte-5ng3n{background:var(--background-fill-secondary)}.panel.svelte-5ng3n .with_avatar.svelte-5ng3n{padding-left:calc(var(--spacing-xl) * 2) !important;padding-right:calc(var(--spacing-xl) * 2) !important}.panel.svelte-5ng3n .panel-full-width.svelte-5ng3n{width:100%}.panel.svelte-5ng3n .user.svelte-5ng3n *{text-align:right}.flex-wrap.svelte-5ng3n.svelte-5ng3n{display:flex;flex-direction:column;max-width:100%;color:var(--body-text-color);font-size:var(--chatbot-text-size);overflow-wrap:break-word}.user.svelte-5ng3n.svelte-5ng3n{border-width:1px;border-radius:var(--radius-md);align-self:flex-start;border-bottom-right-radius:0;box-shadow:var(--shadow-drop);text-align:right;padding:var(--spacing-sm) var(--spacing-xl);border-color:var(--border-color-accent-subdued);background-color:var(--color-accent-soft)}@media(max-width: 480px){.user-row.bubble.svelte-5ng3n.svelte-5ng3n{align-self:flex-end}.bot-row.bubble.svelte-5ng3n.svelte-5ng3n{align-self:flex-start}.message.svelte-5ng3n.svelte-5ng3n{width:100%}}.avatar-container.svelte-5ng3n.svelte-5ng3n{align-self:flex-start;position:relative;display:flex;justify-content:flex-start;align-items:flex-start;width:35px;height:35px;flex-shrink:0;bottom:0;border-radius:50%;border:1px solid var(--border-color-primary)}.user-row.svelte-5ng3n>.avatar-container.svelte-5ng3n{order:2}.user-row.bubble.svelte-5ng3n>.avatar-container.svelte-5ng3n{margin-left:var(--spacing-xxl)}.bot-row.bubble.svelte-5ng3n>.avatar-container.svelte-5ng3n{margin-left:var(--spacing-xxl)}.panel.user-row.svelte-5ng3n>.avatar-container.svelte-5ng3n{order:0}.bot-row.bubble.svelte-5ng3n>.avatar-container.svelte-5ng3n{margin-right:var(--spacing-xxl);margin-left:0}.avatar-container.svelte-5ng3n:not(.thumbnail-item) img{width:100%;height:100%;object-fit:cover;border-radius:50%;padding:6px}.selectable.svelte-5ng3n.svelte-5ng3n{cursor:pointer}@keyframes svelte-5ng3n-dot-flashing{0%{opacity:0.8}50%{opacity:0.5}100%{opacity:0.8}}.message.svelte-5ng3n .preview{object-fit:contain;width:95%;max-height:93%}.image-preview.svelte-5ng3n.svelte-5ng3n{position:absolute;z-index:999;left:0;top:0;width:100%;height:100%;overflow:auto;background-color:rgba(0, 0, 0, 0.9);display:flex;justify-content:center;align-items:center}.image-preview.svelte-5ng3n svg{stroke:white}.image-preview-close-button.svelte-5ng3n.svelte-5ng3n{position:absolute;top:10px;right:10px;background:none;border:none;font-size:1.5em;cursor:pointer;height:30px;width:30px;padding:3px;background:var(--bg-color);box-shadow:var(--shadow-drop);border:1px solid var(--button-secondary-border-color);border-radius:var(--radius-lg)}.message.svelte-5ng3n>button.svelte-5ng3n{width:100%}.html.svelte-5ng3n.svelte-5ng3n{padding:0;border:none;background:none}.thought.svelte-5ng3n.svelte-5ng3n{margin-top:var(--spacing-xxl)}.panel.svelte-5ng3n .bot.svelte-5ng3n,.panel.svelte-5ng3n .user.svelte-5ng3n{border:none;box-shadow:none;background-color:var(--background-fill-secondary)}.panel.user-row.svelte-5ng3n.svelte-5ng3n{background-color:var(--color-accent-soft)}.panel.svelte-5ng3n .user-row.svelte-5ng3n,.panel.svelte-5ng3n .bot-row.svelte-5ng3n{align-self:flex-start}.panel.svelte-5ng3n .user.svelte-5ng3n *,.panel.svelte-5ng3n .bot.svelte-5ng3n *{text-align:left}.panel.svelte-5ng3n .user.svelte-5ng3n{background-color:var(--color-accent-soft)}.panel.svelte-5ng3n .user-row.svelte-5ng3n{background-color:var(--color-accent-soft);align-self:flex-start}.panel.svelte-5ng3n .message.svelte-5ng3n{margin-bottom:var(--spacing-md)}",
  map: '{"version":3,"file":"Message.svelte","sources":["Message.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { is_component_message, is_last_bot_message } from \\"../shared/utils\\";\\nimport { Image } from \\"@gradio/image/shared\\";\\nimport Component from \\"./Component.svelte\\";\\nimport MessageBox from \\"./MessageBox.svelte\\";\\nimport { MarkdownCode as Markdown } from \\"@gradio/markdown-code\\";\\nimport ButtonPanel from \\"./ButtonPanel.svelte\\";\\nexport let value;\\nexport let avatar_img;\\nexport let opposite_avatar_img = null;\\nexport let role = \\"user\\";\\nexport let messages = [];\\nexport let layout;\\nexport let bubble_full_width;\\nexport let render_markdown;\\nexport let latex_delimiters;\\nexport let sanitize_html;\\nexport let selectable;\\nexport let _fetch;\\nexport let rtl;\\nexport let dispatch;\\nexport let i18n;\\nexport let line_breaks;\\nexport let upload;\\nexport let target;\\nexport let root;\\nexport let theme_mode;\\nexport let _components;\\nexport let i;\\nexport let show_copy_button;\\nexport let generating;\\nexport let show_like;\\nexport let show_retry;\\nexport let show_undo;\\nexport let msg_format;\\nexport let handle_action;\\nexport let scroll;\\nfunction handle_select(i2, message) {\\n    dispatch(\\"select\\", {\\n        index: message.index,\\n        value: message.content\\n    });\\n}\\nfunction get_message_label_data(message) {\\n    if (message.type === \\"text\\") {\\n        return message.content;\\n    }\\n    else if (message.type === \\"component\\" && message.content.component === \\"file\\") {\\n        if (Array.isArray(message.content.value)) {\\n            return `file of extension type: ${message.content.value[0].orig_name?.split(\\".\\").pop()}`;\\n        }\\n        return `file of extension type: ${message.content.value?.orig_name?.split(\\".\\").pop()}` + (message.content.value?.orig_name ?? \\"\\");\\n    }\\n    return `a component of type ${message.content.component ?? \\"unknown\\"}`;\\n}\\nlet button_panel_props;\\n$: button_panel_props = {\\n    show: show_like || show_retry || show_undo || show_copy_button,\\n    handle_action,\\n    likeable: show_like,\\n    show_retry,\\n    show_undo,\\n    generating,\\n    show_copy_button,\\n    message: msg_format === \\"tuples\\" ? messages[0] : messages,\\n    position: role === \\"user\\" ? \\"right\\" : \\"left\\",\\n    avatar: avatar_img,\\n    layout\\n};\\n<\/script>\\n\\n<div\\n\\tclass=\\"message-row {layout} {role}-row\\"\\n\\tclass:with_avatar={avatar_img !== null}\\n\\tclass:with_opposite_avatar={opposite_avatar_img !== null}\\n>\\n\\t{#if avatar_img !== null}\\n\\t\\t<div class=\\"avatar-container\\">\\n\\t\\t\\t<Image class=\\"avatar-image\\" src={avatar_img?.url} alt=\\"{role} avatar\\" />\\n\\t\\t</div>\\n\\t{/if}\\n\\t<div\\n\\t\\tclass:role\\n\\t\\tclass=\\"flex-wrap\\"\\n\\t\\tclass:component-wrap={messages[0].type === \\"component\\"}\\n\\t>\\n\\t\\t{#each messages as message, thought_index}\\n\\t\\t\\t<div\\n\\t\\t\\t\\tclass=\\"message {role} {is_component_message(message)\\n\\t\\t\\t\\t\\t? message?.content.component\\n\\t\\t\\t\\t\\t: \'\'}\\"\\n\\t\\t\\t\\tclass:message-fit={layout === \\"bubble\\" && !bubble_full_width}\\n\\t\\t\\t\\tclass:panel-full-width={true}\\n\\t\\t\\t\\tclass:message-markdown-disabled={!render_markdown}\\n\\t\\t\\t\\tstyle:text-align={rtl && role === \\"user\\" ? \\"left\\" : \\"right\\"}\\n\\t\\t\\t\\tclass:component={message.type === \\"component\\"}\\n\\t\\t\\t\\tclass:html={is_component_message(message) &&\\n\\t\\t\\t\\t\\tmessage.content.component === \\"html\\"}\\n\\t\\t\\t\\tclass:thought={thought_index > 0}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tdata-testid={role}\\n\\t\\t\\t\\t\\tclass:latest={i === value.length - 1}\\n\\t\\t\\t\\t\\tclass:message-markdown-disabled={!render_markdown}\\n\\t\\t\\t\\t\\tstyle:user-select=\\"text\\"\\n\\t\\t\\t\\t\\tclass:selectable\\n\\t\\t\\t\\t\\tstyle:cursor={selectable ? \\"pointer\\" : \\"default\\"}\\n\\t\\t\\t\\t\\tstyle:text-align={rtl ? \\"right\\" : \\"left\\"}\\n\\t\\t\\t\\t\\ton:click={() => handle_select(i, message)}\\n\\t\\t\\t\\t\\ton:keydown={(e) => {\\n\\t\\t\\t\\t\\t\\tif (e.key === \\"Enter\\") {\\n\\t\\t\\t\\t\\t\\t\\thandle_select(i, message);\\n\\t\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t\\tdir={rtl ? \\"rtl\\" : \\"ltr\\"}\\n\\t\\t\\t\\t\\taria-label={role + \\"\'s message: \\" + get_message_label_data(message)}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t{#if message.type === \\"text\\"}\\n\\t\\t\\t\\t\\t\\t{#if message.metadata.title}\\n\\t\\t\\t\\t\\t\\t\\t<MessageBox\\n\\t\\t\\t\\t\\t\\t\\t\\ttitle={message.metadata.title}\\n\\t\\t\\t\\t\\t\\t\\t\\texpanded={is_last_bot_message([message], value)}\\n\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t\\t<Markdown\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tmessage={message.content}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{latex_delimiters}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{sanitize_html}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{render_markdown}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{line_breaks}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\ton:load={scroll}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t</MessageBox>\\n\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t<Markdown\\n\\t\\t\\t\\t\\t\\t\\t\\tmessage={message.content}\\n\\t\\t\\t\\t\\t\\t\\t\\t{latex_delimiters}\\n\\t\\t\\t\\t\\t\\t\\t\\t{sanitize_html}\\n\\t\\t\\t\\t\\t\\t\\t\\t{render_markdown}\\n\\t\\t\\t\\t\\t\\t\\t\\t{line_breaks}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:load={scroll}\\n\\t\\t\\t\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t{:else if message.type === \\"component\\" && message.content.component in _components}\\n\\t\\t\\t\\t\\t\\t<Component\\n\\t\\t\\t\\t\\t\\t\\t{target}\\n\\t\\t\\t\\t\\t\\t\\t{theme_mode}\\n\\t\\t\\t\\t\\t\\t\\tprops={message.content.props}\\n\\t\\t\\t\\t\\t\\t\\ttype={message.content.component}\\n\\t\\t\\t\\t\\t\\t\\tcomponents={_components}\\n\\t\\t\\t\\t\\t\\t\\tvalue={message.content.value}\\n\\t\\t\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t\\t\\t\\t{_fetch}\\n\\t\\t\\t\\t\\t\\t\\ton:load={() => scroll()}\\n\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t{:else if message.type === \\"component\\" && message.content.component === \\"file\\"}\\n\\t\\t\\t\\t\\t\\t<a\\n\\t\\t\\t\\t\\t\\t\\tdata-testid=\\"chatbot-file\\"\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"file-pil\\"\\n\\t\\t\\t\\t\\t\\t\\thref={message.content.value.url}\\n\\t\\t\\t\\t\\t\\t\\ttarget=\\"_blank\\"\\n\\t\\t\\t\\t\\t\\t\\tdownload={window.__is_colab__\\n\\t\\t\\t\\t\\t\\t\\t\\t? null\\n\\t\\t\\t\\t\\t\\t\\t\\t: message.content.value?.orig_name ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\tmessage.content.value?.path.split(\\"/\\").pop() ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\"file\\"}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{message.content.value?.orig_name ||\\n\\t\\t\\t\\t\\t\\t\\t\\tmessage.content.value?.path.split(\\"/\\").pop() ||\\n\\t\\t\\t\\t\\t\\t\\t\\t\\"file\\"}\\n\\t\\t\\t\\t\\t\\t</a>\\n\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t</div>\\n\\n\\t\\t\\t{#if layout === \\"panel\\"}\\n\\t\\t\\t\\t<ButtonPanel {...button_panel_props} />\\n\\t\\t\\t{/if}\\n\\t\\t{/each}\\n\\t</div>\\n</div>\\n\\n{#if layout === \\"bubble\\"}\\n\\t<ButtonPanel {...button_panel_props} />\\n{/if}\\n\\n<style>\\n\\t.message {\\n\\t\\tposition: relative;\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t/* avatar styles */\\n\\t.avatar-container {\\n\\t\\tflex-shrink: 0;\\n\\t\\tborder-radius: 50%;\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.avatar-container :global(img) {\\n\\t\\tobject-fit: cover;\\n\\t}\\n\\n\\t/* message wrapper */\\n\\t.flex-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\twidth: calc(100% - var(--spacing-xxl));\\n\\t\\tmax-width: 100%;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-size: var(--chatbot-text-size);\\n\\t\\toverflow-wrap: break-word;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.component {\\n\\t\\tpadding: 0;\\n\\t\\tborder-radius: var(--radius-md);\\n\\t\\twidth: fit-content;\\n\\t\\toverflow: hidden;\\n\\t}\\n\\n\\t.component.gallery {\\n\\t\\tborder: none;\\n\\t}\\n\\n\\t.message-row :not(.avatar-container) :global(img) {\\n\\t\\tmargin: var(--size-2);\\n\\t\\tmax-height: 300px;\\n\\t}\\n\\n\\t.file-pil {\\n\\t\\tdisplay: block;\\n\\t\\twidth: fit-content;\\n\\t\\tpadding: var(--spacing-sm) var(--spacing-lg);\\n\\t\\tborder-radius: var(--radius-md);\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\ttext-decoration: none;\\n\\t\\tmargin: 0;\\n\\t\\tfont-family: var(--font-mono);\\n\\t\\tfont-size: var(--text-sm);\\n\\t}\\n\\n\\t.file {\\n\\t\\twidth: auto !important;\\n\\t\\tmax-width: fit-content !important;\\n\\t}\\n\\n\\t@media (max-width: 600px) or (max-width: 480px) {\\n\\t\\t.component {\\n\\t\\t\\twidth: 100%;\\n\\t\\t}\\n\\t}\\n\\n\\t.message :global(.prose) {\\n\\t\\tfont-size: var(--chatbot-text-size);\\n\\t}\\n\\n\\t.message-bubble-border {\\n\\t\\tborder-width: 1px;\\n\\t\\tborder-radius: var(--radius-md);\\n\\t}\\n\\n\\t.message-fit {\\n\\t\\twidth: fit-content !important;\\n\\t}\\n\\n\\t.panel-full-width {\\n\\t\\twidth: 100%;\\n\\t}\\n\\t.message-markdown-disabled {\\n\\t\\twhite-space: pre-line;\\n\\t}\\n\\n\\t.user {\\n\\t\\tborder-width: 1px;\\n\\t\\tborder-radius: var(--radius-md);\\n\\t\\talign-self: flex-start;\\n\\t\\tborder-bottom-right-radius: 0;\\n\\t\\tbox-shadow: var(--shadow-drop);\\n\\t\\talign-self: flex-start;\\n\\t\\ttext-align: right;\\n\\t\\tpadding: var(--spacing-sm) var(--spacing-xl);\\n\\t\\tborder-color: var(--border-color-accent-subdued);\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t}\\n\\n\\t.bot {\\n\\t\\tborder-width: 1px;\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t\\tborder-bottom-left-radius: 0;\\n\\t\\tborder-color: var(--border-color-primary);\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tbox-shadow: var(--shadow-drop);\\n\\t\\talign-self: flex-start;\\n\\t\\ttext-align: right;\\n\\t\\tpadding: var(--spacing-sm) var(--spacing-xl);\\n\\t}\\n\\n\\t.panel .user :global(*) {\\n\\t\\ttext-align: right;\\n\\t}\\n\\n\\t/* Colors */\\n\\t.bubble .bot {\\n\\t\\tborder-color: var(--border-color-primary);\\n\\t}\\n\\n\\t.message-row {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t/* bubble mode styles */\\n\\t.bubble {\\n\\t\\tmargin: calc(var(--spacing-xl) * 2);\\n\\t\\tmargin-bottom: var(--spacing-xl);\\n\\t}\\n\\n\\t.bubble.user-row {\\n\\t\\talign-self: flex-end;\\n\\t\\tmax-width: calc(100% - var(--spacing-xl) * 6);\\n\\t}\\n\\n\\t.bubble.bot-row {\\n\\t\\talign-self: flex-start;\\n\\t\\tmax-width: calc(100% - var(--spacing-xl) * 6);\\n\\t}\\n\\n\\t.bubble .user-row {\\n\\t\\tflex-direction: row;\\n\\t\\tjustify-content: flex-end;\\n\\t}\\n\\n\\t.bubble .with_avatar.user-row {\\n\\t\\tmargin-right: calc(var(--spacing-xl) * 2) !important;\\n\\t}\\n\\n\\t.bubble .with_avatar.bot-row {\\n\\t\\tmargin-left: calc(var(--spacing-xl) * 2) !important;\\n\\t}\\n\\n\\t.bubble .with_opposite_avatar.user-row {\\n\\t\\tmargin-left: calc(var(--spacing-xxl) + 35px + var(--spacing-xxl));\\n\\t}\\n\\n\\t.bubble .message-fit {\\n\\t\\twidth: fit-content !important;\\n\\t}\\n\\n\\t/* panel mode styles */\\n\\t.panel {\\n\\t\\tmargin: 0;\\n\\t\\tpadding: calc(var(--spacing-lg) * 2) calc(var(--spacing-lg) * 2);\\n\\t}\\n\\n\\t.panel.bot-row {\\n\\t\\tbackground: var(--background-fill-secondary);\\n\\t}\\n\\n\\t.panel .with_avatar {\\n\\t\\tpadding-left: calc(var(--spacing-xl) * 2) !important;\\n\\t\\tpadding-right: calc(var(--spacing-xl) * 2) !important;\\n\\t}\\n\\n\\t.panel .panel-full-width {\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t.panel .user :global(*) {\\n\\t\\ttext-align: right;\\n\\t}\\n\\n\\t/* message content */\\n\\t.flex-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tmax-width: 100%;\\n\\t\\tcolor: var(--body-text-color);\\n\\t\\tfont-size: var(--chatbot-text-size);\\n\\t\\toverflow-wrap: break-word;\\n\\t}\\n\\n\\t.user {\\n\\t\\tborder-width: 1px;\\n\\t\\tborder-radius: var(--radius-md);\\n\\t\\talign-self: flex-start;\\n\\t\\tborder-bottom-right-radius: 0;\\n\\t\\tbox-shadow: var(--shadow-drop);\\n\\t\\ttext-align: right;\\n\\t\\tpadding: var(--spacing-sm) var(--spacing-xl);\\n\\t\\tborder-color: var(--border-color-accent-subdued);\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t}\\n\\t@media (max-width: 480px) {\\n\\t\\t.user-row.bubble {\\n\\t\\t\\talign-self: flex-end;\\n\\t\\t}\\n\\n\\t\\t.bot-row.bubble {\\n\\t\\t\\talign-self: flex-start;\\n\\t\\t}\\n\\t\\t.message {\\n\\t\\t\\twidth: 100%;\\n\\t\\t}\\n\\t}\\n\\n\\t.avatar-container {\\n\\t\\talign-self: flex-start;\\n\\t\\tposition: relative;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: flex-start;\\n\\t\\talign-items: flex-start;\\n\\t\\twidth: 35px;\\n\\t\\theight: 35px;\\n\\t\\tflex-shrink: 0;\\n\\t\\tbottom: 0;\\n\\t\\tborder-radius: 50%;\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t}\\n\\t.user-row > .avatar-container {\\n\\t\\torder: 2;\\n\\t}\\n\\n\\t.user-row.bubble > .avatar-container {\\n\\t\\tmargin-left: var(--spacing-xxl);\\n\\t}\\n\\n\\t.bot-row.bubble > .avatar-container {\\n\\t\\tmargin-left: var(--spacing-xxl);\\n\\t}\\n\\n\\t.panel.user-row > .avatar-container {\\n\\t\\torder: 0;\\n\\t}\\n\\n\\t.bot-row.bubble > .avatar-container {\\n\\t\\tmargin-right: var(--spacing-xxl);\\n\\t\\tmargin-left: 0;\\n\\t}\\n\\n\\t.avatar-container:not(.thumbnail-item) :global(img) {\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\tobject-fit: cover;\\n\\t\\tborder-radius: 50%;\\n\\t\\tpadding: 6px;\\n\\t}\\n\\n\\t.selectable {\\n\\t\\tcursor: pointer;\\n\\t}\\n\\n\\t@keyframes dot-flashing {\\n\\t\\t0% {\\n\\t\\t\\topacity: 0.8;\\n\\t\\t}\\n\\t\\t50% {\\n\\t\\t\\topacity: 0.5;\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\topacity: 0.8;\\n\\t\\t}\\n\\t}\\n\\n\\t/* Image preview */\\n\\t.message :global(.preview) {\\n\\t\\tobject-fit: contain;\\n\\t\\twidth: 95%;\\n\\t\\tmax-height: 93%;\\n\\t}\\n\\t.image-preview {\\n\\t\\tposition: absolute;\\n\\t\\tz-index: 999;\\n\\t\\tleft: 0;\\n\\t\\ttop: 0;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\toverflow: auto;\\n\\t\\tbackground-color: rgba(0, 0, 0, 0.9);\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t}\\n\\t.image-preview :global(svg) {\\n\\t\\tstroke: white;\\n\\t}\\n\\t.image-preview-close-button {\\n\\t\\tposition: absolute;\\n\\t\\ttop: 10px;\\n\\t\\tright: 10px;\\n\\t\\tbackground: none;\\n\\t\\tborder: none;\\n\\t\\tfont-size: 1.5em;\\n\\t\\tcursor: pointer;\\n\\t\\theight: 30px;\\n\\t\\twidth: 30px;\\n\\t\\tpadding: 3px;\\n\\t\\tbackground: var(--bg-color);\\n\\t\\tbox-shadow: var(--shadow-drop);\\n\\t\\tborder: 1px solid var(--button-secondary-border-color);\\n\\t\\tborder-radius: var(--radius-lg);\\n\\t}\\n\\n\\t.message > button {\\n\\t\\twidth: 100%;\\n\\t}\\n\\t.html {\\n\\t\\tpadding: 0;\\n\\t\\tborder: none;\\n\\t\\tbackground: none;\\n\\t}\\n\\n\\t.thought {\\n\\t\\tmargin-top: var(--spacing-xxl);\\n\\t}\\n\\n\\t.panel .bot,\\n\\t.panel .user {\\n\\t\\tborder: none;\\n\\t\\tbox-shadow: none;\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t}\\n\\n\\t.panel.user-row {\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t}\\n\\n\\t.panel .user-row,\\n\\t.panel .bot-row {\\n\\t\\talign-self: flex-start;\\n\\t}\\n\\n\\t.panel .user :global(*),\\n\\t.panel .bot :global(*) {\\n\\t\\ttext-align: left;\\n\\t}\\n\\n\\t.panel .user {\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t}\\n\\n\\t.panel .user-row {\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t\\talign-self: flex-start;\\n\\t}\\n\\n\\t.panel .message {\\n\\t\\tmargin-bottom: var(--spacing-md);\\n\\t}</style>\\n"],"names":[],"mappings":"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"}'
};
function get_message_label_data(message) {
  if (message.type === "text") {
    return message.content;
  } else if (message.type === "component" && message.content.component === "file") {
    if (Array.isArray(message.content.value)) {
      return `file of extension type: ${message.content.value[0].orig_name?.split(".").pop()}`;
    }
    return `file of extension type: ${message.content.value?.orig_name?.split(".").pop()}` + (message.content.value?.orig_name ?? "");
  }
  return `a component of type ${message.content.component ?? "unknown"}`;
}
const Message = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { avatar_img } = $$props;
  let { opposite_avatar_img = null } = $$props;
  let { role = "user" } = $$props;
  let { messages = [] } = $$props;
  let { layout } = $$props;
  let { bubble_full_width } = $$props;
  let { render_markdown } = $$props;
  let { latex_delimiters } = $$props;
  let { sanitize_html } = $$props;
  let { selectable } = $$props;
  let { _fetch } = $$props;
  let { rtl } = $$props;
  let { dispatch } = $$props;
  let { i18n } = $$props;
  let { line_breaks } = $$props;
  let { upload } = $$props;
  let { target } = $$props;
  let { root } = $$props;
  let { theme_mode } = $$props;
  let { _components } = $$props;
  let { i } = $$props;
  let { show_copy_button } = $$props;
  let { generating } = $$props;
  let { show_like } = $$props;
  let { show_retry } = $$props;
  let { show_undo } = $$props;
  let { msg_format } = $$props;
  let { handle_action } = $$props;
  let { scroll: scroll2 } = $$props;
  let button_panel_props;
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.avatar_img === void 0 && $$bindings.avatar_img && avatar_img !== void 0)
    $$bindings.avatar_img(avatar_img);
  if ($$props.opposite_avatar_img === void 0 && $$bindings.opposite_avatar_img && opposite_avatar_img !== void 0)
    $$bindings.opposite_avatar_img(opposite_avatar_img);
  if ($$props.role === void 0 && $$bindings.role && role !== void 0)
    $$bindings.role(role);
  if ($$props.messages === void 0 && $$bindings.messages && messages !== void 0)
    $$bindings.messages(messages);
  if ($$props.layout === void 0 && $$bindings.layout && layout !== void 0)
    $$bindings.layout(layout);
  if ($$props.bubble_full_width === void 0 && $$bindings.bubble_full_width && bubble_full_width !== void 0)
    $$bindings.bubble_full_width(bubble_full_width);
  if ($$props.render_markdown === void 0 && $$bindings.render_markdown && render_markdown !== void 0)
    $$bindings.render_markdown(render_markdown);
  if ($$props.latex_delimiters === void 0 && $$bindings.latex_delimiters && latex_delimiters !== void 0)
    $$bindings.latex_delimiters(latex_delimiters);
  if ($$props.sanitize_html === void 0 && $$bindings.sanitize_html && sanitize_html !== void 0)
    $$bindings.sanitize_html(sanitize_html);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  if ($$props._fetch === void 0 && $$bindings._fetch && _fetch !== void 0)
    $$bindings._fetch(_fetch);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.dispatch === void 0 && $$bindings.dispatch && dispatch !== void 0)
    $$bindings.dispatch(dispatch);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.line_breaks === void 0 && $$bindings.line_breaks && line_breaks !== void 0)
    $$bindings.line_breaks(line_breaks);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.target === void 0 && $$bindings.target && target !== void 0)
    $$bindings.target(target);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props._components === void 0 && $$bindings._components && _components !== void 0)
    $$bindings._components(_components);
  if ($$props.i === void 0 && $$bindings.i && i !== void 0)
    $$bindings.i(i);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.generating === void 0 && $$bindings.generating && generating !== void 0)
    $$bindings.generating(generating);
  if ($$props.show_like === void 0 && $$bindings.show_like && show_like !== void 0)
    $$bindings.show_like(show_like);
  if ($$props.show_retry === void 0 && $$bindings.show_retry && show_retry !== void 0)
    $$bindings.show_retry(show_retry);
  if ($$props.show_undo === void 0 && $$bindings.show_undo && show_undo !== void 0)
    $$bindings.show_undo(show_undo);
  if ($$props.msg_format === void 0 && $$bindings.msg_format && msg_format !== void 0)
    $$bindings.msg_format(msg_format);
  if ($$props.handle_action === void 0 && $$bindings.handle_action && handle_action !== void 0)
    $$bindings.handle_action(handle_action);
  if ($$props.scroll === void 0 && $$bindings.scroll && scroll2 !== void 0)
    $$bindings.scroll(scroll2);
  $$result.css.add(css$3);
  button_panel_props = {
    show: show_like || show_retry || show_undo || show_copy_button,
    handle_action,
    likeable: show_like,
    show_retry,
    show_undo,
    generating,
    show_copy_button,
    message: msg_format === "tuples" ? messages[0] : messages,
    position: role === "user" ? "right" : "left",
    avatar: avatar_img,
    layout
  };
  return `<div class="${[
    "message-row " + escape(layout, true) + " " + escape(role, true) + "-row svelte-5ng3n",
    (avatar_img !== null ? "with_avatar" : "") + " " + (opposite_avatar_img !== null ? "with_opposite_avatar" : "")
  ].join(" ").trim()}">${avatar_img !== null ? `<div class="avatar-container svelte-5ng3n">${validate_component(Image$1, "Image").$$render(
    $$result,
    {
      class: "avatar-image",
      src: avatar_img?.url,
      alt: role + " avatar"
    },
    {},
    {}
  )}</div>` : ``} <div class="${[
    "flex-wrap svelte-5ng3n",
    (role ? "role" : "") + " " + (messages[0].type === "component" ? "component-wrap" : "")
  ].join(" ").trim()}">${each(messages, (message, thought_index) => {
    return `<div class="${[
      "message " + escape(role, true) + " " + escape(
        is_component_message(message) ? message?.content.component : "",
        true
      ) + " svelte-5ng3n",
      (layout === "bubble" && !bubble_full_width ? "message-fit" : "") + " panel-full-width " + (!render_markdown ? "message-markdown-disabled" : "") + " " + (message.type === "component" ? "component" : "") + " " + (is_component_message(message) && message.content.component === "html" ? "html" : "") + " " + (thought_index > 0 ? "thought" : "")
    ].join(" ").trim()}"${add_styles({
      "text-align": rtl && role === "user" ? "left" : "right"
    })}><button${add_attribute("data-testid", role, 0)}${add_attribute("dir", rtl ? "rtl" : "ltr", 0)}${add_attribute("aria-label", role + "'s message: " + get_message_label_data(message), 0)} class="${[
      "svelte-5ng3n",
      (i === value.length - 1 ? "latest" : "") + " " + (!render_markdown ? "message-markdown-disabled" : "") + " " + (selectable ? "selectable" : "")
    ].join(" ").trim()}"${add_styles({
      "user-select": `text`,
      "cursor": selectable ? "pointer" : "default",
      "text-align": rtl ? "right" : "left"
    })}>${message.type === "text" ? `${message.metadata.title ? `${validate_component(MessageBox, "MessageBox").$$render(
      $$result,
      {
        title: message.metadata.title,
        expanded: is_last_bot_message([message], value)
      },
      {},
      {
        default: () => {
          return `${validate_component(MarkdownCode, "Markdown").$$render(
            $$result,
            {
              message: message.content,
              latex_delimiters,
              sanitize_html,
              render_markdown,
              line_breaks,
              root
            },
            {},
            {}
          )} `;
        }
      }
    )}` : `${validate_component(MarkdownCode, "Markdown").$$render(
      $$result,
      {
        message: message.content,
        latex_delimiters,
        sanitize_html,
        render_markdown,
        line_breaks,
        root
      },
      {},
      {}
    )}`}` : `${message.type === "component" && message.content.component in _components ? `${validate_component(Component, "Component").$$render(
      $$result,
      {
        target,
        theme_mode,
        props: message.content.props,
        type: message.content.component,
        components: _components,
        value: message.content.value,
        i18n,
        upload,
        _fetch
      },
      {},
      {}
    )}` : `${message.type === "component" && message.content.component === "file" ? `<a data-testid="chatbot-file" class="file-pil svelte-5ng3n"${add_attribute("href", message.content.value.url, 0)} target="_blank"${add_attribute(
      "download",
      window.__is_colab__ ? null : message.content.value?.orig_name || message.content.value?.path.split("/").pop() || "file",
      0
    )}>${escape(message.content.value?.orig_name || message.content.value?.path.split("/").pop() || "file")} </a>` : ``}`}`} </button></div> ${layout === "panel" ? `${validate_component(ButtonPanel, "ButtonPanel").$$render($$result, Object.assign({}, button_panel_props), {}, {})}` : ``}`;
  })}</div></div> ${layout === "bubble" ? `${validate_component(ButtonPanel, "ButtonPanel").$$render($$result, Object.assign({}, button_panel_props), {}, {})}` : ``}`;
});
const css$2 = {
  code: ".pending.svelte-1gpwetz{background:var(--color-accent-soft);display:flex;flex-direction:row;justify-content:center;align-items:center;align-self:center;gap:2px;width:100%;height:var(--size-16)}.dot-flashing.svelte-1gpwetz{animation:svelte-1gpwetz-flash 1s infinite ease-in-out;border-radius:5px;background-color:var(--body-text-color);width:7px;height:7px;color:var(--body-text-color)}@keyframes svelte-1gpwetz-flash{0%,100%{opacity:0}50%{opacity:1}}.dot-flashing.svelte-1gpwetz:nth-child(1){animation-delay:0s}.dot-flashing.svelte-1gpwetz:nth-child(2){animation-delay:0.33s}.dot-flashing.svelte-1gpwetz:nth-child(3){animation-delay:0.66s}",
  map: '{"version":3,"file":"Pending.svelte","sources":["Pending.svelte"],"sourcesContent":["<script lang=\\"ts\\">export let layout = \\"bubble\\";\\n<\/script>\\n\\n<div\\n\\tclass=\\"message pending\\"\\n\\trole=\\"status\\"\\n\\taria-label=\\"Loading response\\"\\n\\taria-live=\\"polite\\"\\n\\tstyle:border-radius={layout === \\"bubble\\" ? \\"var(--radius-xxl)\\" : \\"none\\"}\\n>\\n\\t<span class=\\"sr-only\\">Loading content</span>\\n\\t<div class=\\"dot-flashing\\" />\\n\\t&nbsp;\\n\\t<div class=\\"dot-flashing\\" />\\n\\t&nbsp;\\n\\t<div class=\\"dot-flashing\\" />\\n</div>\\n\\n<style>\\n\\t.pending {\\n\\t\\tbackground: var(--color-accent-soft);\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: row;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\talign-self: center;\\n\\t\\tgap: 2px;\\n\\t\\twidth: 100%;\\n\\t\\theight: var(--size-16);\\n\\t}\\n\\t.dot-flashing {\\n\\t\\tanimation: flash 1s infinite ease-in-out;\\n\\t\\tborder-radius: 5px;\\n\\t\\tbackground-color: var(--body-text-color);\\n\\t\\twidth: 7px;\\n\\t\\theight: 7px;\\n\\t\\tcolor: var(--body-text-color);\\n\\t}\\n\\t@keyframes flash {\\n\\t\\t0%,\\n\\t\\t100% {\\n\\t\\t\\topacity: 0;\\n\\t\\t}\\n\\t\\t50% {\\n\\t\\t\\topacity: 1;\\n\\t\\t}\\n\\t}\\n\\n\\t.dot-flashing:nth-child(1) {\\n\\t\\tanimation-delay: 0s;\\n\\t}\\n\\n\\t.dot-flashing:nth-child(2) {\\n\\t\\tanimation-delay: 0.33s;\\n\\t}\\n\\t.dot-flashing:nth-child(3) {\\n\\t\\tanimation-delay: 0.66s;\\n\\t}</style>\\n"],"names":[],"mappings":"AAmBC,uBAAS,CACR,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,GAAG,CACnB,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,MAAM,CAClB,GAAG,CAAE,GAAG,CACR,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,SAAS,CACtB,CACA,4BAAc,CACb,SAAS,CAAE,oBAAK,CAAC,EAAE,CAAC,QAAQ,CAAC,WAAW,CACxC,aAAa,CAAE,GAAG,CAClB,gBAAgB,CAAE,IAAI,iBAAiB,CAAC,CACxC,KAAK,CAAE,GAAG,CACV,MAAM,CAAE,GAAG,CACX,KAAK,CAAE,IAAI,iBAAiB,CAC7B,CACA,WAAW,oBAAM,CAChB,EAAE,CACF,IAAK,CACJ,OAAO,CAAE,CACV,CACA,GAAI,CACH,OAAO,CAAE,CACV,CACD,CAEA,4BAAa,WAAW,CAAC,CAAE,CAC1B,eAAe,CAAE,EAClB,CAEA,4BAAa,WAAW,CAAC,CAAE,CAC1B,eAAe,CAAE,KAClB,CACA,4BAAa,WAAW,CAAC,CAAE,CAC1B,eAAe,CAAE,KAClB"}'
};
const Pending = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { layout = "bubble" } = $$props;
  if ($$props.layout === void 0 && $$bindings.layout && layout !== void 0)
    $$bindings.layout(layout);
  $$result.css.add(css$2);
  return `<div class="message pending svelte-1gpwetz" role="status" aria-label="Loading response" aria-live="polite"${add_styles({
    "border-radius": layout === "bubble" ? "var(--radius-xxl)" : "none"
  })} data-svelte-h="svelte-exuub1"><span class="sr-only">Loading content</span> <div class="dot-flashing svelte-1gpwetz"></div>
	 
	<div class="dot-flashing svelte-1gpwetz"></div>
	 
	<div class="dot-flashing svelte-1gpwetz"></div> </div>`;
});
const CopyAll = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  onDestroy(() => {
  });
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  return `${validate_component(IconButton, "IconButton").$$render(
    $$result,
    {
      Icon: Copy,
      label: "Copy conversation"
    },
    {},
    {}
  )}`;
});
const css$1 = {
  code: ".placeholder-content.svelte-vxn3uw.svelte-vxn3uw{display:flex;flex-direction:column;height:100%}.placeholder.svelte-vxn3uw.svelte-vxn3uw{align-items:center;display:flex;justify-content:center;height:100%;flex-grow:1}.examples.svelte-vxn3uw img{pointer-events:none}.examples.svelte-vxn3uw.svelte-vxn3uw{margin:auto;padding:var(--spacing-xxl);display:grid;grid-template-columns:repeat(auto-fit, minmax(200px, 1fr));gap:var(--spacing-xxl);max-width:calc(min(4 * 200px + 5 * var(--spacing-xxl), 100%))}.example.svelte-vxn3uw.svelte-vxn3uw{display:flex;flex-direction:column;align-items:center;padding:var(--spacing-xl);border:0.05px solid var(--border-color-primary);border-radius:var(--radius-xl);background-color:var(--background-fill-secondary);cursor:pointer;transition:var(--button-transition);max-width:var(--size-56);width:100%;justify-content:center}.example.svelte-vxn3uw.svelte-vxn3uw:hover{background-color:var(--color-accent-soft);border-color:var(--border-color-accent)}.example-icon-container.svelte-vxn3uw.svelte-vxn3uw{display:flex;align-self:flex-start;margin-left:var(--spacing-md);width:var(--size-6);height:var(--size-6)}.example-display-text.svelte-vxn3uw.svelte-vxn3uw,.example-text.svelte-vxn3uw.svelte-vxn3uw,.example-file.svelte-vxn3uw.svelte-vxn3uw{font-size:var(--text-md);width:100%;text-align:center;overflow:hidden;text-overflow:ellipsis}.example-display-text.svelte-vxn3uw.svelte-vxn3uw,.example-file.svelte-vxn3uw.svelte-vxn3uw{margin-top:var(--spacing-md)}.example-image-container.svelte-vxn3uw.svelte-vxn3uw{flex-grow:1;display:flex;justify-content:center;align-items:center;margin-top:var(--spacing-xl)}.example-image-container.svelte-vxn3uw img{max-height:100%;max-width:100%;height:var(--size-32);width:100%;object-fit:cover;border-radius:var(--radius-xl)}.panel-wrap.svelte-vxn3uw.svelte-vxn3uw{width:100%;overflow-y:auto}.bubble-wrap.svelte-vxn3uw.svelte-vxn3uw{width:100%;overflow-y:auto;height:100%;padding-top:var(--spacing-xxl)}@media(prefers-color-scheme: dark){.bubble-wrap.svelte-vxn3uw.svelte-vxn3uw{background:var(--background-fill-secondary)}}.message-wrap.svelte-vxn3uw.svelte-vxn3uw{display:flex;flex-direction:column;justify-content:space-between;margin-bottom:var(--spacing-xxl)}.message-wrap.svelte-vxn3uw .prose.chatbot.md{opacity:0.8;overflow-wrap:break-word}.message-wrap.svelte-vxn3uw .message-row .md img{border-radius:var(--radius-xl);margin:var(--size-2);width:400px;max-width:30vw;max-height:30vw}.message-wrap.svelte-vxn3uw .message a{color:var(--color-text-link);text-decoration:underline}.message-wrap.svelte-vxn3uw .bot table,.message-wrap.svelte-vxn3uw .bot tr,.message-wrap.svelte-vxn3uw .bot td,.message-wrap.svelte-vxn3uw .bot th{border:1px solid var(--border-color-primary)}.message-wrap.svelte-vxn3uw .user table,.message-wrap.svelte-vxn3uw .user tr,.message-wrap.svelte-vxn3uw .user td,.message-wrap.svelte-vxn3uw .user th{border:1px solid var(--border-color-accent)}.message-wrap.svelte-vxn3uw span.katex{font-size:var(--text-lg);direction:ltr}.message-wrap.svelte-vxn3uw span.katex-display{margin-top:0}.message-wrap.svelte-vxn3uw pre{position:relative}.message-wrap.svelte-vxn3uw .grid-wrap{max-height:80% !important;max-width:600px;object-fit:contain}.message-wrap.svelte-vxn3uw>div.svelte-vxn3uw p:not(:first-child){margin-top:var(--spacing-xxl)}.message-wrap.svelte-vxn3uw.svelte-vxn3uw{display:flex;flex-direction:column;justify-content:space-between;margin-bottom:var(--spacing-xxl)}.panel-wrap.svelte-vxn3uw .message-row:first-child{padding-top:calc(var(--spacing-xxl) * 2)}.scroll-down-button-container.svelte-vxn3uw.svelte-vxn3uw{position:absolute;bottom:10px;left:50%;transform:translateX(-50%);z-index:var(--layer-top)}.scroll-down-button-container.svelte-vxn3uw button{border-radius:50%;box-shadow:var(--shadow-drop);transition:box-shadow 0.2s ease-in-out,\n			transform 0.2s ease-in-out}.scroll-down-button-container.svelte-vxn3uw button:hover{box-shadow:var(--shadow-drop),\n			0 2px 2px rgba(0, 0, 0, 0.05);transform:translateY(-2px)}.image-preview.svelte-vxn3uw.svelte-vxn3uw{position:absolute;z-index:999;left:0;top:0;width:100%;height:100%;overflow:auto;background-color:var(--background-fill-secondary);display:flex;justify-content:center;align-items:center}",
  map: '{"version":3,"file":"ChatBot.svelte","sources":["ChatBot.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { format_chat_for_sharing, is_last_bot_message, group_messages, load_components, get_components_from_messages } from \\"./utils\\";\\nimport { copy } from \\"@gradio/utils\\";\\nimport Message from \\"./Message.svelte\\";\\nimport { dequal } from \\"dequal/lite\\";\\nimport { afterUpdate, createEventDispatcher, tick, onMount } from \\"svelte\\";\\nimport { Image } from \\"@gradio/image/shared\\";\\nimport { Clear, Trash, Community, ScrollDownArrow } from \\"@gradio/icons\\";\\nimport { IconButtonWrapper, IconButton } from \\"@gradio/atoms\\";\\nimport { MarkdownCode as Markdown } from \\"@gradio/markdown-code\\";\\nimport Pending from \\"./Pending.svelte\\";\\nimport { ShareError } from \\"@gradio/utils\\";\\nimport { Gradio } from \\"@gradio/utils\\";\\nexport let value = [];\\nlet old_value = null;\\nimport CopyAll from \\"./CopyAll.svelte\\";\\nexport let _fetch;\\nexport let load_component;\\nlet _components = {};\\nconst is_browser = typeof window !== \\"undefined\\";\\nasync function update_components() {\\n    _components = await load_components(get_components_from_messages(value), _components, load_component);\\n}\\n$: value, update_components();\\nexport let latex_delimiters;\\nexport let pending_message = false;\\nexport let generating = false;\\nexport let selectable = false;\\nexport let likeable = false;\\nexport let show_share_button = false;\\nexport let show_copy_all_button = false;\\nexport let rtl = false;\\nexport let show_copy_button = false;\\nexport let avatar_images = [null, null];\\nexport let sanitize_html = true;\\nexport let bubble_full_width = true;\\nexport let render_markdown = true;\\nexport let line_breaks = true;\\nexport let autoscroll = true;\\nexport let theme_mode;\\nexport let i18n;\\nexport let layout = \\"bubble\\";\\nexport let placeholder = null;\\nexport let upload;\\nexport let msg_format = \\"tuples\\";\\nexport let examples = null;\\nexport let _retryable = false;\\nexport let _undoable = false;\\nexport let like_user_message = false;\\nexport let root;\\nlet target = null;\\nonMount(() => {\\n    target = document.querySelector(\\"div.gradio-container\\");\\n});\\nlet div;\\nlet show_scroll_button = false;\\nconst dispatch = createEventDispatcher();\\nfunction is_at_bottom() {\\n    return div && div.offsetHeight + div.scrollTop > div.scrollHeight - 100;\\n}\\nfunction scroll_to_bottom() {\\n    if (!div)\\n        return;\\n    div.scrollTo(0, div.scrollHeight);\\n    show_scroll_button = false;\\n}\\nlet scroll_after_component_load = false;\\nfunction on_child_component_load() {\\n    if (scroll_after_component_load) {\\n        scroll_to_bottom();\\n        scroll_after_component_load = false;\\n    }\\n}\\nasync function scroll_on_value_update() {\\n    if (!autoscroll)\\n        return;\\n    if (is_at_bottom()) {\\n        scroll_after_component_load = true;\\n        await tick();\\n        scroll_to_bottom();\\n    }\\n    else {\\n        show_scroll_button = true;\\n    }\\n}\\nonMount(() => {\\n    scroll_on_value_update();\\n});\\n$: if (value || pending_message || _components) {\\n    scroll_on_value_update();\\n}\\nonMount(() => {\\n    function handle_scroll() {\\n        if (is_at_bottom()) {\\n            show_scroll_button = false;\\n        }\\n        else {\\n            scroll_after_component_load = false;\\n        }\\n    }\\n    div?.addEventListener(\\"scroll\\", handle_scroll);\\n    return () => {\\n        div?.removeEventListener(\\"scroll\\", handle_scroll);\\n    };\\n});\\nlet image_preview_source;\\nlet image_preview_source_alt;\\nlet is_image_preview_open = false;\\nafterUpdate(() => {\\n    if (!div)\\n        return;\\n    div.querySelectorAll(\\"img\\").forEach((n) => {\\n        n.addEventListener(\\"click\\", (e) => {\\n            const target2 = e.target;\\n            if (target2) {\\n                image_preview_source = target2.src;\\n                image_preview_source_alt = target2.alt;\\n                is_image_preview_open = true;\\n            }\\n        });\\n    });\\n});\\n$: {\\n    if (!dequal(value, old_value)) {\\n        old_value = value;\\n        dispatch(\\"change\\");\\n    }\\n}\\n$: groupedMessages = value && group_messages(value, msg_format);\\nfunction handle_example_select(i, example) {\\n    dispatch(\\"example_select\\", {\\n        index: i,\\n        value: { text: example.text, files: example.files }\\n    });\\n}\\nfunction handle_like(i, message, selected) {\\n    if (selected === \\"undo\\" || selected === \\"retry\\") {\\n        const val_ = value;\\n        let last_index = val_.length - 1;\\n        while (val_[last_index].role === \\"assistant\\") {\\n            last_index--;\\n        }\\n        dispatch(selected, {\\n            index: val_[last_index].index,\\n            value: val_[last_index].content\\n        });\\n        return;\\n    }\\n    if (msg_format === \\"tuples\\") {\\n        dispatch(\\"like\\", {\\n            index: message.index,\\n            value: message.content,\\n            liked: selected === \\"like\\"\\n        });\\n    }\\n    else {\\n        if (!groupedMessages)\\n            return;\\n        const message_group = groupedMessages[i];\\n        const [first, last] = [\\n            message_group[0],\\n            message_group[message_group.length - 1]\\n        ];\\n        dispatch(\\"like\\", {\\n            index: [first.index, last.index],\\n            value: message_group.map((m) => m.content),\\n            liked: selected === \\"like\\"\\n        });\\n    }\\n}\\n<\/script>\\n\\n{#if value !== null && value.length > 0}\\n\\t<IconButtonWrapper>\\n\\t\\t{#if show_share_button}\\n\\t\\t\\t<IconButton\\n\\t\\t\\t\\tIcon={Community}\\n\\t\\t\\t\\ton:click={async () => {\\n\\t\\t\\t\\t\\ttry {\\n\\t\\t\\t\\t\\t\\t// @ts-ignore\\n\\t\\t\\t\\t\\t\\tconst formatted = await format_chat_for_sharing(value);\\n\\t\\t\\t\\t\\t\\tdispatch(\\"share\\", {\\n\\t\\t\\t\\t\\t\\t\\tdescription: formatted\\n\\t\\t\\t\\t\\t\\t});\\n\\t\\t\\t\\t\\t} catch (e) {\\n\\t\\t\\t\\t\\t\\tconsole.error(e);\\n\\t\\t\\t\\t\\t\\tlet message = e instanceof ShareError ? e.message : \\"Share failed.\\";\\n\\t\\t\\t\\t\\t\\tdispatch(\\"error\\", message);\\n\\t\\t\\t\\t\\t}\\n\\t\\t\\t\\t}}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<Community />\\n\\t\\t\\t</IconButton>\\n\\t\\t{/if}\\n\\t\\t<IconButton Icon={Trash} on:click={() => dispatch(\\"clear\\")} label={\\"Clear\\"}\\n\\t\\t></IconButton>\\n\\t\\t{#if show_copy_all_button}\\n\\t\\t\\t<CopyAll {value} />\\n\\t\\t{/if}\\n\\t</IconButtonWrapper>\\n{/if}\\n\\n<div\\n\\tclass={layout === \\"bubble\\" ? \\"bubble-wrap\\" : \\"panel-wrap\\"}\\n\\tbind:this={div}\\n\\trole=\\"log\\"\\n\\taria-label=\\"chatbot conversation\\"\\n\\taria-live=\\"polite\\"\\n>\\n\\t{#if value !== null && value.length > 0 && groupedMessages !== null}\\n\\t\\t<div class=\\"message-wrap\\" use:copy>\\n\\t\\t\\t{#each groupedMessages as messages, i}\\n\\t\\t\\t\\t{@const role = messages[0].role === \\"user\\" ? \\"user\\" : \\"bot\\"}\\n\\t\\t\\t\\t{@const avatar_img = avatar_images[role === \\"user\\" ? 0 : 1]}\\n\\t\\t\\t\\t{@const opposite_avatar_img = avatar_images[role === \\"user\\" ? 0 : 1]}\\n\\t\\t\\t\\t{#if is_image_preview_open}\\n\\t\\t\\t\\t\\t<div class=\\"image-preview\\">\\n\\t\\t\\t\\t\\t\\t<img src={image_preview_source} alt={image_preview_source_alt} />\\n\\t\\t\\t\\t\\t\\t<IconButtonWrapper>\\n\\t\\t\\t\\t\\t\\t\\t<IconButton\\n\\t\\t\\t\\t\\t\\t\\t\\tIcon={Clear}\\n\\t\\t\\t\\t\\t\\t\\t\\ton:click={() => (is_image_preview_open = false)}\\n\\t\\t\\t\\t\\t\\t\\t\\tlabel={\\"Clear\\"}\\n\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t</IconButtonWrapper>\\n\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t<Message\\n\\t\\t\\t\\t\\t{messages}\\n\\t\\t\\t\\t\\t{opposite_avatar_img}\\n\\t\\t\\t\\t\\t{avatar_img}\\n\\t\\t\\t\\t\\t{role}\\n\\t\\t\\t\\t\\t{layout}\\n\\t\\t\\t\\t\\t{dispatch}\\n\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t{_fetch}\\n\\t\\t\\t\\t\\t{line_breaks}\\n\\t\\t\\t\\t\\t{theme_mode}\\n\\t\\t\\t\\t\\t{target}\\n\\t\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t\\t{selectable}\\n\\t\\t\\t\\t\\t{sanitize_html}\\n\\t\\t\\t\\t\\t{bubble_full_width}\\n\\t\\t\\t\\t\\t{render_markdown}\\n\\t\\t\\t\\t\\t{rtl}\\n\\t\\t\\t\\t\\t{i}\\n\\t\\t\\t\\t\\t{value}\\n\\t\\t\\t\\t\\t{latex_delimiters}\\n\\t\\t\\t\\t\\t{_components}\\n\\t\\t\\t\\t\\t{generating}\\n\\t\\t\\t\\t\\t{msg_format}\\n\\t\\t\\t\\t\\tshow_like={role === \\"user\\" ? likeable && like_user_message : likeable}\\n\\t\\t\\t\\t\\tshow_retry={_retryable && is_last_bot_message(messages, value)}\\n\\t\\t\\t\\t\\tshow_undo={_undoable && is_last_bot_message(messages, value)}\\n\\t\\t\\t\\t\\t{show_copy_button}\\n\\t\\t\\t\\t\\thandle_action={(selected) => handle_like(i, messages[0], selected)}\\n\\t\\t\\t\\t\\tscroll={is_browser ? scroll : () => {}}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/each}\\n\\t\\t\\t{#if pending_message}\\n\\t\\t\\t\\t<Pending {layout} />\\n\\t\\t\\t{/if}\\n\\t\\t</div>\\n\\t{:else}\\n\\t\\t<div class=\\"placeholder-content\\">\\n\\t\\t\\t{#if placeholder !== null}\\n\\t\\t\\t\\t<div class=\\"placeholder\\">\\n\\t\\t\\t\\t\\t<Markdown message={placeholder} {latex_delimiters} {root} />\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t{/if}\\n\\t\\t\\t{#if examples !== null}\\n\\t\\t\\t\\t<div class=\\"examples\\">\\n\\t\\t\\t\\t\\t{#each examples as example, i}\\n\\t\\t\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\t\\t\\tclass=\\"example\\"\\n\\t\\t\\t\\t\\t\\t\\ton:click={() => handle_example_select(i, example)}\\n\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{#if example.icon !== undefined}\\n\\t\\t\\t\\t\\t\\t\\t\\t<div class=\\"example-icon-container\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"example-icon\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tsrc={example.icon.url}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\talt=\\"example-icon\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t\\t{#if example.display_text !== undefined}\\n\\t\\t\\t\\t\\t\\t\\t\\t<span class=\\"example-display-text\\">{example.display_text}</span>\\n\\t\\t\\t\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t\\t\\t\\t<span class=\\"example-text\\">{example.text}</span>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t\\t{#if example.files !== undefined && example.files.length > 1}\\n\\t\\t\\t\\t\\t\\t\\t\\t<span class=\\"example-file\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t><em>{example.files.length} Files</em></span\\n\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{:else if example.files !== undefined && example.files[0] !== undefined && example.files[0].mime_type?.includes(\\"image\\")}\\n\\t\\t\\t\\t\\t\\t\\t\\t<div class=\\"example-image-container\\">\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t<Image\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tclass=\\"example-image\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\tsrc={example.files[0].url}\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t\\talt=\\"example-image\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t/>\\n\\t\\t\\t\\t\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t\\t\\t\\t{:else if example.files !== undefined && example.files[0] !== undefined}\\n\\t\\t\\t\\t\\t\\t\\t\\t<span class=\\"example-file\\"\\n\\t\\t\\t\\t\\t\\t\\t\\t\\t><em>{example.files[0].orig_name}</em></span\\n\\t\\t\\t\\t\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t\\t\\t</button>\\n\\t\\t\\t\\t\\t{/each}\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t{/if}\\n\\t\\t</div>\\n\\t{/if}\\n</div>\\n\\n{#if show_scroll_button}\\n\\t<div class=\\"scroll-down-button-container\\">\\n\\t\\t<IconButton\\n\\t\\t\\tIcon={ScrollDownArrow}\\n\\t\\t\\tlabel=\\"Scroll down\\"\\n\\t\\t\\tsize=\\"large\\"\\n\\t\\t\\ton:click={scroll_to_bottom}\\n\\t\\t/>\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.placeholder-content {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\theight: 100%;\\n\\t}\\n\\n\\t.placeholder {\\n\\t\\talign-items: center;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\theight: 100%;\\n\\t\\tflex-grow: 1;\\n\\t}\\n\\n\\t.examples :global(img) {\\n\\t\\tpointer-events: none;\\n\\t}\\n\\n\\t.examples {\\n\\t\\tmargin: auto;\\n\\t\\tpadding: var(--spacing-xxl);\\n\\t\\tdisplay: grid;\\n\\t\\tgrid-template-columns: repeat(auto-fit, minmax(200px, 1fr));\\n\\t\\tgap: var(--spacing-xxl);\\n\\t\\tmax-width: calc(min(4 * 200px + 5 * var(--spacing-xxl), 100%));\\n\\t}\\n\\n\\t.example {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\talign-items: center;\\n\\t\\tpadding: var(--spacing-xl);\\n\\t\\tborder: 0.05px solid var(--border-color-primary);\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tcursor: pointer;\\n\\t\\ttransition: var(--button-transition);\\n\\t\\tmax-width: var(--size-56);\\n\\t\\twidth: 100%;\\n\\t\\tjustify-content: center;\\n\\t}\\n\\n\\t.example:hover {\\n\\t\\tbackground-color: var(--color-accent-soft);\\n\\t\\tborder-color: var(--border-color-accent);\\n\\t}\\n\\n\\t.example-icon-container {\\n\\t\\tdisplay: flex;\\n\\t\\talign-self: flex-start;\\n\\t\\tmargin-left: var(--spacing-md);\\n\\t\\twidth: var(--size-6);\\n\\t\\theight: var(--size-6);\\n\\t}\\n\\n\\t.example-display-text,\\n\\t.example-text,\\n\\t.example-file {\\n\\t\\tfont-size: var(--text-md);\\n\\t\\twidth: 100%;\\n\\t\\ttext-align: center;\\n\\t\\toverflow: hidden;\\n\\t\\ttext-overflow: ellipsis;\\n\\t}\\n\\n\\t.example-display-text,\\n\\t.example-file {\\n\\t\\tmargin-top: var(--spacing-md);\\n\\t}\\n\\n\\t.example-image-container {\\n\\t\\tflex-grow: 1;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t\\tmargin-top: var(--spacing-xl);\\n\\t}\\n\\n\\t.example-image-container :global(img) {\\n\\t\\tmax-height: 100%;\\n\\t\\tmax-width: 100%;\\n\\t\\theight: var(--size-32);\\n\\t\\twidth: 100%;\\n\\t\\tobject-fit: cover;\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t}\\n\\n\\t.panel-wrap {\\n\\t\\twidth: 100%;\\n\\t\\toverflow-y: auto;\\n\\t}\\n\\n\\t.bubble-wrap {\\n\\t\\twidth: 100%;\\n\\t\\toverflow-y: auto;\\n\\t\\theight: 100%;\\n\\t\\tpadding-top: var(--spacing-xxl);\\n\\t}\\n\\n\\t@media (prefers-color-scheme: dark) {\\n\\t\\t.bubble-wrap {\\n\\t\\t\\tbackground: var(--background-fill-secondary);\\n\\t\\t}\\n\\t}\\n\\n\\t.message-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: space-between;\\n\\t\\tmargin-bottom: var(--spacing-xxl);\\n\\t}\\n\\n\\t.message-wrap :global(.prose.chatbot.md) {\\n\\t\\topacity: 0.8;\\n\\t\\toverflow-wrap: break-word;\\n\\t}\\n\\n\\t.message-wrap :global(.message-row .md img) {\\n\\t\\tborder-radius: var(--radius-xl);\\n\\t\\tmargin: var(--size-2);\\n\\t\\twidth: 400px;\\n\\t\\tmax-width: 30vw;\\n\\t\\tmax-height: 30vw;\\n\\t}\\n\\n\\t/* link styles */\\n\\t.message-wrap :global(.message a) {\\n\\t\\tcolor: var(--color-text-link);\\n\\t\\ttext-decoration: underline;\\n\\t}\\n\\n\\t/* table styles */\\n\\t.message-wrap :global(.bot table),\\n\\t.message-wrap :global(.bot tr),\\n\\t.message-wrap :global(.bot td),\\n\\t.message-wrap :global(.bot th) {\\n\\t\\tborder: 1px solid var(--border-color-primary);\\n\\t}\\n\\n\\t.message-wrap :global(.user table),\\n\\t.message-wrap :global(.user tr),\\n\\t.message-wrap :global(.user td),\\n\\t.message-wrap :global(.user th) {\\n\\t\\tborder: 1px solid var(--border-color-accent);\\n\\t}\\n\\n\\t/* KaTeX */\\n\\t.message-wrap :global(span.katex) {\\n\\t\\tfont-size: var(--text-lg);\\n\\t\\tdirection: ltr;\\n\\t}\\n\\n\\t.message-wrap :global(span.katex-display) {\\n\\t\\tmargin-top: 0;\\n\\t}\\n\\n\\t.message-wrap :global(pre) {\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t.message-wrap :global(.grid-wrap) {\\n\\t\\tmax-height: 80% !important;\\n\\t\\tmax-width: 600px;\\n\\t\\tobject-fit: contain;\\n\\t}\\n\\n\\t.message-wrap > div :global(p:not(:first-child)) {\\n\\t\\tmargin-top: var(--spacing-xxl);\\n\\t}\\n\\n\\t.message-wrap {\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: space-between;\\n\\t\\tmargin-bottom: var(--spacing-xxl);\\n\\t}\\n\\n\\t.panel-wrap :global(.message-row:first-child) {\\n\\t\\tpadding-top: calc(var(--spacing-xxl) * 2);\\n\\t}\\n\\n\\t.scroll-down-button-container {\\n\\t\\tposition: absolute;\\n\\t\\tbottom: 10px;\\n\\t\\tleft: 50%;\\n\\t\\ttransform: translateX(-50%);\\n\\t\\tz-index: var(--layer-top);\\n\\t}\\n\\t.scroll-down-button-container :global(button) {\\n\\t\\tborder-radius: 50%;\\n\\t\\tbox-shadow: var(--shadow-drop);\\n\\t\\ttransition:\\n\\t\\t\\tbox-shadow 0.2s ease-in-out,\\n\\t\\t\\ttransform 0.2s ease-in-out;\\n\\t}\\n\\t.scroll-down-button-container :global(button:hover) {\\n\\t\\tbox-shadow:\\n\\t\\t\\tvar(--shadow-drop),\\n\\t\\t\\t0 2px 2px rgba(0, 0, 0, 0.05);\\n\\t\\ttransform: translateY(-2px);\\n\\t}\\n\\n\\t.image-preview {\\n\\t\\tposition: absolute;\\n\\t\\tz-index: 999;\\n\\t\\tleft: 0;\\n\\t\\ttop: 0;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\toverflow: auto;\\n\\t\\tbackground-color: var(--background-fill-secondary);\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\talign-items: center;\\n\\t}</style>\\n"],"names":[],"mappings":"AAwUC,gDAAqB,CACpB,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,MAAM,CAAE,IACT,CAEA,wCAAa,CACZ,WAAW,CAAE,MAAM,CACnB,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,MAAM,CAAE,IAAI,CACZ,SAAS,CAAE,CACZ,CAEA,uBAAS,CAAS,GAAK,CACtB,cAAc,CAAE,IACjB,CAEA,qCAAU,CACT,MAAM,CAAE,IAAI,CACZ,OAAO,CAAE,IAAI,aAAa,CAAC,CAC3B,OAAO,CAAE,IAAI,CACb,qBAAqB,CAAE,OAAO,QAAQ,CAAC,CAAC,OAAO,KAAK,CAAC,CAAC,GAAG,CAAC,CAAC,CAC3D,GAAG,CAAE,IAAI,aAAa,CAAC,CACvB,SAAS,CAAE,KAAK,IAAI,CAAC,CAAC,CAAC,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,IAAI,aAAa,CAAC,CAAC,CAAC,IAAI,CAAC,CAC9D,CAEA,oCAAS,CACR,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,WAAW,CAAE,MAAM,CACnB,OAAO,CAAE,IAAI,YAAY,CAAC,CAC1B,MAAM,CAAE,MAAM,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAAC,CAChD,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,MAAM,CAAE,OAAO,CACf,UAAU,CAAE,IAAI,mBAAmB,CAAC,CACpC,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,KAAK,CAAE,IAAI,CACX,eAAe,CAAE,MAClB,CAEA,oCAAQ,MAAO,CACd,gBAAgB,CAAE,IAAI,mBAAmB,CAAC,CAC1C,YAAY,CAAE,IAAI,qBAAqB,CACxC,CAEA,mDAAwB,CACvB,OAAO,CAAE,IAAI,CACb,UAAU,CAAE,UAAU,CACtB,WAAW,CAAE,IAAI,YAAY,CAAC,CAC9B,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,MAAM,CAAE,IAAI,QAAQ,CACrB,CAEA,iDAAqB,CACrB,yCAAa,CACb,yCAAc,CACb,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,MAAM,CAClB,QAAQ,CAAE,MAAM,CAChB,aAAa,CAAE,QAChB,CAEA,iDAAqB,CACrB,yCAAc,CACb,UAAU,CAAE,IAAI,YAAY,CAC7B,CAEA,oDAAyB,CACxB,SAAS,CAAE,CAAC,CACZ,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MAAM,CACnB,UAAU,CAAE,IAAI,YAAY,CAC7B,CAEA,sCAAwB,CAAS,GAAK,CACrC,UAAU,CAAE,IAAI,CAChB,SAAS,CAAE,IAAI,CACf,MAAM,CAAE,IAAI,SAAS,CAAC,CACtB,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,KAAK,CACjB,aAAa,CAAE,IAAI,WAAW,CAC/B,CAEA,uCAAY,CACX,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,IACb,CAEA,wCAAa,CACZ,KAAK,CAAE,IAAI,CACX,UAAU,CAAE,IAAI,CAChB,MAAM,CAAE,IAAI,CACZ,WAAW,CAAE,IAAI,aAAa,CAC/B,CAEA,MAAO,uBAAuB,IAAI,CAAE,CACnC,wCAAa,CACZ,UAAU,CAAE,IAAI,2BAA2B,CAC5C,CACD,CAEA,yCAAc,CACb,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,aAAa,CAC9B,aAAa,CAAE,IAAI,aAAa,CACjC,CAEA,2BAAa,CAAS,iBAAmB,CACxC,OAAO,CAAE,GAAG,CACZ,aAAa,CAAE,UAChB,CAEA,2BAAa,CAAS,oBAAsB,CAC3C,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,KAAK,CACZ,SAAS,CAAE,IAAI,CACf,UAAU,CAAE,IACb,CAGA,2BAAa,CAAS,UAAY,CACjC,KAAK,CAAE,IAAI,iBAAiB,CAAC,CAC7B,eAAe,CAAE,SAClB,CAGA,2BAAa,CAAS,UAAW,CACjC,2BAAa,CAAS,OAAQ,CAC9B,2BAAa,CAAS,OAAQ,CAC9B,2BAAa,CAAS,OAAS,CAC9B,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,sBAAsB,CAC7C,CAEA,2BAAa,CAAS,WAAY,CAClC,2BAAa,CAAS,QAAS,CAC/B,2BAAa,CAAS,QAAS,CAC/B,2BAAa,CAAS,QAAU,CAC/B,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,qBAAqB,CAC5C,CAGA,2BAAa,CAAS,UAAY,CACjC,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,SAAS,CAAE,GACZ,CAEA,2BAAa,CAAS,kBAAoB,CACzC,UAAU,CAAE,CACb,CAEA,2BAAa,CAAS,GAAK,CAC1B,QAAQ,CAAE,QACX,CAEA,2BAAa,CAAS,UAAY,CACjC,UAAU,CAAE,GAAG,CAAC,UAAU,CAC1B,SAAS,CAAE,KAAK,CAChB,UAAU,CAAE,OACb,CAEA,2BAAa,CAAG,iBAAG,CAAS,mBAAqB,CAChD,UAAU,CAAE,IAAI,aAAa,CAC9B,CAEA,yCAAc,CACb,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,aAAa,CAC9B,aAAa,CAAE,IAAI,aAAa,CACjC,CAEA,yBAAW,CAAS,wBAA0B,CAC7C,WAAW,CAAE,KAAK,IAAI,aAAa,CAAC,CAAC,CAAC,CAAC,CAAC,CACzC,CAEA,yDAA8B,CAC7B,QAAQ,CAAE,QAAQ,CAClB,MAAM,CAAE,IAAI,CACZ,IAAI,CAAE,GAAG,CACT,SAAS,CAAE,WAAW,IAAI,CAAC,CAC3B,OAAO,CAAE,IAAI,WAAW,CACzB,CACA,2CAA6B,CAAS,MAAQ,CAC7C,aAAa,CAAE,GAAG,CAClB,UAAU,CAAE,IAAI,aAAa,CAAC,CAC9B,UAAU,CACT,UAAU,CAAC,IAAI,CAAC,WAAW,CAAC;AAC/B,GAAG,SAAS,CAAC,IAAI,CAAC,WACjB,CACA,2CAA6B,CAAS,YAAc,CACnD,UAAU,CACT,IAAI,aAAa,CAAC,CAAC;AACtB,GAAG,CAAC,CAAC,GAAG,CAAC,GAAG,CAAC,KAAK,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,CAAC,IAAI,CAAC,CAC9B,SAAS,CAAE,WAAW,IAAI,CAC3B,CAEA,0CAAe,CACd,QAAQ,CAAE,QAAQ,CAClB,OAAO,CAAE,GAAG,CACZ,IAAI,CAAE,CAAC,CACP,GAAG,CAAE,CAAC,CACN,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,QAAQ,CAAE,IAAI,CACd,gBAAgB,CAAE,IAAI,2BAA2B,CAAC,CAClD,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,WAAW,CAAE,MACd"}'
};
const ChatBot = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let groupedMessages;
  let { value = [] } = $$props;
  let old_value = null;
  let { _fetch } = $$props;
  let { load_component } = $$props;
  let _components = {};
  const is_browser = typeof window !== "undefined";
  async function update_components() {
    _components = await load_components(get_components_from_messages(value), _components, load_component);
  }
  let { latex_delimiters } = $$props;
  let { pending_message = false } = $$props;
  let { generating = false } = $$props;
  let { selectable = false } = $$props;
  let { likeable = false } = $$props;
  let { show_share_button = false } = $$props;
  let { show_copy_all_button = false } = $$props;
  let { rtl = false } = $$props;
  let { show_copy_button = false } = $$props;
  let { avatar_images = [null, null] } = $$props;
  let { sanitize_html = true } = $$props;
  let { bubble_full_width = true } = $$props;
  let { render_markdown = true } = $$props;
  let { line_breaks = true } = $$props;
  let { autoscroll = true } = $$props;
  let { theme_mode } = $$props;
  let { i18n } = $$props;
  let { layout = "bubble" } = $$props;
  let { placeholder = null } = $$props;
  let { upload } = $$props;
  let { msg_format = "tuples" } = $$props;
  let { examples = null } = $$props;
  let { _retryable = false } = $$props;
  let { _undoable = false } = $$props;
  let { like_user_message = false } = $$props;
  let { root } = $$props;
  let target = null;
  let div;
  let show_scroll_button = false;
  const dispatch = createEventDispatcher();
  async function scroll_on_value_update() {
    if (!autoscroll)
      return;
    {
      show_scroll_button = true;
    }
  }
  function handle_like(i, message, selected) {
    if (selected === "undo" || selected === "retry") {
      const val_ = value;
      let last_index = val_.length - 1;
      while (val_[last_index].role === "assistant") {
        last_index--;
      }
      dispatch(selected, {
        index: val_[last_index].index,
        value: val_[last_index].content
      });
      return;
    }
    if (msg_format === "tuples") {
      dispatch("like", {
        index: message.index,
        value: message.content,
        liked: selected === "like"
      });
    } else {
      if (!groupedMessages)
        return;
      const message_group = groupedMessages[i];
      const [first, last] = [message_group[0], message_group[message_group.length - 1]];
      dispatch("like", {
        index: [first.index, last.index],
        value: message_group.map((m) => m.content),
        liked: selected === "like"
      });
    }
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props._fetch === void 0 && $$bindings._fetch && _fetch !== void 0)
    $$bindings._fetch(_fetch);
  if ($$props.load_component === void 0 && $$bindings.load_component && load_component !== void 0)
    $$bindings.load_component(load_component);
  if ($$props.latex_delimiters === void 0 && $$bindings.latex_delimiters && latex_delimiters !== void 0)
    $$bindings.latex_delimiters(latex_delimiters);
  if ($$props.pending_message === void 0 && $$bindings.pending_message && pending_message !== void 0)
    $$bindings.pending_message(pending_message);
  if ($$props.generating === void 0 && $$bindings.generating && generating !== void 0)
    $$bindings.generating(generating);
  if ($$props.selectable === void 0 && $$bindings.selectable && selectable !== void 0)
    $$bindings.selectable(selectable);
  if ($$props.likeable === void 0 && $$bindings.likeable && likeable !== void 0)
    $$bindings.likeable(likeable);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.show_copy_all_button === void 0 && $$bindings.show_copy_all_button && show_copy_all_button !== void 0)
    $$bindings.show_copy_all_button(show_copy_all_button);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.avatar_images === void 0 && $$bindings.avatar_images && avatar_images !== void 0)
    $$bindings.avatar_images(avatar_images);
  if ($$props.sanitize_html === void 0 && $$bindings.sanitize_html && sanitize_html !== void 0)
    $$bindings.sanitize_html(sanitize_html);
  if ($$props.bubble_full_width === void 0 && $$bindings.bubble_full_width && bubble_full_width !== void 0)
    $$bindings.bubble_full_width(bubble_full_width);
  if ($$props.render_markdown === void 0 && $$bindings.render_markdown && render_markdown !== void 0)
    $$bindings.render_markdown(render_markdown);
  if ($$props.line_breaks === void 0 && $$bindings.line_breaks && line_breaks !== void 0)
    $$bindings.line_breaks(line_breaks);
  if ($$props.autoscroll === void 0 && $$bindings.autoscroll && autoscroll !== void 0)
    $$bindings.autoscroll(autoscroll);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.layout === void 0 && $$bindings.layout && layout !== void 0)
    $$bindings.layout(layout);
  if ($$props.placeholder === void 0 && $$bindings.placeholder && placeholder !== void 0)
    $$bindings.placeholder(placeholder);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.msg_format === void 0 && $$bindings.msg_format && msg_format !== void 0)
    $$bindings.msg_format(msg_format);
  if ($$props.examples === void 0 && $$bindings.examples && examples !== void 0)
    $$bindings.examples(examples);
  if ($$props._retryable === void 0 && $$bindings._retryable && _retryable !== void 0)
    $$bindings._retryable(_retryable);
  if ($$props._undoable === void 0 && $$bindings._undoable && _undoable !== void 0)
    $$bindings._undoable(_undoable);
  if ($$props.like_user_message === void 0 && $$bindings.like_user_message && like_user_message !== void 0)
    $$bindings.like_user_message(like_user_message);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  $$result.css.add(css$1);
  {
    update_components();
  }
  {
    if (value || pending_message || _components) {
      scroll_on_value_update();
    }
  }
  {
    {
      if (!dequal(value, old_value)) {
        old_value = value;
        dispatch("change");
      }
    }
  }
  groupedMessages = value && group_messages(value, msg_format);
  return `${value !== null && value.length > 0 ? `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${show_share_button ? `${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Community }, {}, {
        default: () => {
          return `${validate_component(Community, "Community").$$render($$result, {}, {}, {})}`;
        }
      })}` : ``} ${validate_component(IconButton, "IconButton").$$render($$result, { Icon: Trash, label: "Clear" }, {}, {})} ${show_copy_all_button ? `${validate_component(CopyAll, "CopyAll").$$render($$result, { value }, {}, {})}` : ``}`;
    }
  })}` : ``} <div class="${escape(null_to_empty(layout === "bubble" ? "bubble-wrap" : "panel-wrap"), true) + " svelte-vxn3uw"}" role="log" aria-label="chatbot conversation" aria-live="polite"${add_attribute("this", div, 0)}>${value !== null && value.length > 0 && groupedMessages !== null ? `<div class="message-wrap svelte-vxn3uw">${each(groupedMessages, (messages, i) => {
    let role = messages[0].role === "user" ? "user" : "bot", avatar_img = avatar_images[role === "user" ? 0 : 1], opposite_avatar_img = avatar_images[role === "user" ? 0 : 1];
    return `   ${``} ${validate_component(Message, "Message").$$render(
      $$result,
      {
        messages,
        opposite_avatar_img,
        avatar_img,
        role,
        layout,
        dispatch,
        i18n,
        _fetch,
        line_breaks,
        theme_mode,
        target,
        root,
        upload,
        selectable,
        sanitize_html,
        bubble_full_width,
        render_markdown,
        rtl,
        i,
        value,
        latex_delimiters,
        _components,
        generating,
        msg_format,
        show_like: role === "user" ? likeable && like_user_message : likeable,
        show_retry: _retryable && is_last_bot_message(messages, value),
        show_undo: _undoable && is_last_bot_message(messages, value),
        show_copy_button,
        handle_action: (selected) => handle_like(i, messages[0], selected),
        scroll: is_browser ? scroll : () => {
        }
      },
      {},
      {}
    )}`;
  })} ${pending_message ? `${validate_component(Pending, "Pending").$$render($$result, { layout }, {}, {})}` : ``}</div>` : `<div class="placeholder-content svelte-vxn3uw">${placeholder !== null ? `<div class="placeholder svelte-vxn3uw">${validate_component(MarkdownCode, "Markdown").$$render(
    $$result,
    {
      message: placeholder,
      latex_delimiters,
      root
    },
    {},
    {}
  )}</div>` : ``} ${examples !== null ? `<div class="examples svelte-vxn3uw">${each(examples, (example, i) => {
    return `<button class="example svelte-vxn3uw">${example.icon !== void 0 ? `<div class="example-icon-container svelte-vxn3uw">${validate_component(Image$1, "Image").$$render(
      $$result,
      {
        class: "example-icon",
        src: example.icon.url,
        alt: "example-icon"
      },
      {},
      {}
    )} </div>` : ``} ${example.display_text !== void 0 ? `<span class="example-display-text svelte-vxn3uw">${escape(example.display_text)}</span>` : `<span class="example-text svelte-vxn3uw">${escape(example.text)}</span>`} ${example.files !== void 0 && example.files.length > 1 ? `<span class="example-file svelte-vxn3uw"><em>${escape(example.files.length)} Files</em></span>` : `${example.files !== void 0 && example.files[0] !== void 0 && example.files[0].mime_type?.includes("image") ? `<div class="example-image-container svelte-vxn3uw">${validate_component(Image$1, "Image").$$render(
      $$result,
      {
        class: "example-image",
        src: example.files[0].url,
        alt: "example-image"
      },
      {},
      {}
    )} </div>` : `${example.files !== void 0 && example.files[0] !== void 0 ? `<span class="example-file svelte-vxn3uw"><em>${escape(example.files[0].orig_name)}</em></span>` : ``}`}`} </button>`;
  })}</div>` : ``}</div>`}</div> ${show_scroll_button ? `<div class="scroll-down-button-container svelte-vxn3uw">${validate_component(IconButton, "IconButton").$$render(
    $$result,
    {
      Icon: ScrollDownArrow,
      label: "Scroll down",
      size: "large"
    },
    {},
    {}
  )}</div>` : ``}`;
});
const ChatBot$1 = ChatBot;
const css = {
  code: ".wrapper.svelte-g3p8na{display:flex;position:relative;flex-direction:column;align-items:start;width:100%;height:100%;flex-grow:1}.progress-text{right:auto}",
  map: '{"version":3,"file":"Index.svelte","sources":["Index.svelte"],"sourcesContent":["<script context=\\"module\\" lang=\\"ts\\">export { default as BaseChatBot } from \\"./shared/ChatBot.svelte\\";\\n<\/script>\\n\\n<script lang=\\"ts\\">import ChatBot from \\"./shared/ChatBot.svelte\\";\\nimport { Block, BlockLabel } from \\"@gradio/atoms\\";\\nimport { Chat } from \\"@gradio/icons\\";\\nimport { StatusTracker } from \\"@gradio/statustracker\\";\\nimport { normalise_tuples, normalise_messages } from \\"./shared/utils\\";\\nexport let elem_id = \\"\\";\\nexport let elem_classes = [];\\nexport let visible = true;\\nexport let value = [];\\nexport let scale = null;\\nexport let min_width = void 0;\\nexport let label;\\nexport let show_label = true;\\nexport let root;\\nexport let _selectable = false;\\nexport let likeable = false;\\nexport let show_share_button = false;\\nexport let rtl = false;\\nexport let show_copy_button = true;\\nexport let show_copy_all_button = false;\\nexport let sanitize_html = true;\\nexport let bubble_full_width = true;\\nexport let layout = \\"bubble\\";\\nexport let type = \\"tuples\\";\\nexport let render_markdown = true;\\nexport let line_breaks = true;\\nexport let autoscroll = true;\\nexport let _retryable = false;\\nexport let _undoable = false;\\nexport let latex_delimiters;\\nexport let gradio;\\nlet _value = [];\\n$: _value = type === \\"tuples\\" ? normalise_tuples(value, root) : normalise_messages(value, root);\\nexport let avatar_images = [null, null];\\nexport let like_user_message = false;\\nexport let loading_status = void 0;\\nexport let height;\\nexport let min_height;\\nexport let max_height;\\nexport let placeholder = null;\\nexport let examples = null;\\nexport let theme_mode;\\n<\/script>\\n\\n<Block\\n\\t{elem_id}\\n\\t{elem_classes}\\n\\t{visible}\\n\\tpadding={false}\\n\\t{scale}\\n\\t{min_width}\\n\\t{height}\\n\\t{min_height}\\n\\t{max_height}\\n\\tallow_overflow={true}\\n\\tflex={true}\\n\\toverflow_behavior=\\"auto\\"\\n>\\n\\t{#if loading_status}\\n\\t\\t<StatusTracker\\n\\t\\t\\tautoscroll={gradio.autoscroll}\\n\\t\\t\\ti18n={gradio.i18n}\\n\\t\\t\\t{...loading_status}\\n\\t\\t\\tshow_progress={loading_status.show_progress === \\"hidden\\"\\n\\t\\t\\t\\t? \\"hidden\\"\\n\\t\\t\\t\\t: \\"minimal\\"}\\n\\t\\t\\ton:clear_status={() => gradio.dispatch(\\"clear_status\\", loading_status)}\\n\\t\\t/>\\n\\t{/if}\\n\\t<div class=\\"wrapper\\">\\n\\t\\t{#if show_label}\\n\\t\\t\\t<BlockLabel\\n\\t\\t\\t\\t{show_label}\\n\\t\\t\\t\\tIcon={Chat}\\n\\t\\t\\t\\tfloat={true}\\n\\t\\t\\t\\tlabel={label || \\"Chatbot\\"}\\n\\t\\t\\t/>\\n\\t\\t{/if}\\n\\t\\t<ChatBot\\n\\t\\t\\ti18n={gradio.i18n}\\n\\t\\t\\tselectable={_selectable}\\n\\t\\t\\t{likeable}\\n\\t\\t\\t{show_share_button}\\n\\t\\t\\t{show_copy_all_button}\\n\\t\\t\\tvalue={_value}\\n\\t\\t\\t{latex_delimiters}\\n\\t\\t\\t{render_markdown}\\n\\t\\t\\t{theme_mode}\\n\\t\\t\\tpending_message={loading_status?.status === \\"pending\\"}\\n\\t\\t\\tgenerating={loading_status?.status === \\"generating\\"}\\n\\t\\t\\t{rtl}\\n\\t\\t\\t{show_copy_button}\\n\\t\\t\\t{like_user_message}\\n\\t\\t\\ton:change={() => gradio.dispatch(\\"change\\", value)}\\n\\t\\t\\ton:select={(e) => gradio.dispatch(\\"select\\", e.detail)}\\n\\t\\t\\ton:like={(e) => gradio.dispatch(\\"like\\", e.detail)}\\n\\t\\t\\ton:share={(e) => gradio.dispatch(\\"share\\", e.detail)}\\n\\t\\t\\ton:error={(e) => gradio.dispatch(\\"error\\", e.detail)}\\n\\t\\t\\ton:example_select={(e) => gradio.dispatch(\\"example_select\\", e.detail)}\\n\\t\\t\\ton:retry={(e) => gradio.dispatch(\\"retry\\", e.detail)}\\n\\t\\t\\ton:undo={(e) => gradio.dispatch(\\"undo\\", e.detail)}\\n\\t\\t\\ton:clear={() => {\\n\\t\\t\\t\\tvalue = [];\\n\\t\\t\\t\\tgradio.dispatch(\\"clear\\");\\n\\t\\t\\t}}\\n\\t\\t\\t{avatar_images}\\n\\t\\t\\t{sanitize_html}\\n\\t\\t\\t{bubble_full_width}\\n\\t\\t\\t{line_breaks}\\n\\t\\t\\t{autoscroll}\\n\\t\\t\\t{layout}\\n\\t\\t\\t{placeholder}\\n\\t\\t\\t{examples}\\n\\t\\t\\t{_retryable}\\n\\t\\t\\t{_undoable}\\n\\t\\t\\tupload={(...args) => gradio.client.upload(...args)}\\n\\t\\t\\t_fetch={(...args) => gradio.client.fetch(...args)}\\n\\t\\t\\tload_component={gradio.load_component}\\n\\t\\t\\tmsg_format={type}\\n\\t\\t\\troot={gradio.root}\\n\\t\\t/>\\n\\t</div>\\n</Block>\\n\\n<style>\\n\\t.wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\tposition: relative;\\n\\t\\tflex-direction: column;\\n\\t\\talign-items: start;\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\tflex-grow: 1;\\n\\t}\\n\\n\\t:global(.progress-text) {\\n\\t\\tright: auto;\\n\\t}</style>\\n"],"names":[],"mappings":"AAgIC,sBAAS,CACR,OAAO,CAAE,IAAI,CACb,QAAQ,CAAE,QAAQ,CAClB,cAAc,CAAE,MAAM,CACtB,WAAW,CAAE,KAAK,CAClB,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,SAAS,CAAE,CACZ,CAEQ,cAAgB,CACvB,KAAK,CAAE,IACR"}'
};
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = [] } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { label } = $$props;
  let { show_label = true } = $$props;
  let { root } = $$props;
  let { _selectable = false } = $$props;
  let { likeable = false } = $$props;
  let { show_share_button = false } = $$props;
  let { rtl = false } = $$props;
  let { show_copy_button = true } = $$props;
  let { show_copy_all_button = false } = $$props;
  let { sanitize_html = true } = $$props;
  let { bubble_full_width = true } = $$props;
  let { layout = "bubble" } = $$props;
  let { type = "tuples" } = $$props;
  let { render_markdown = true } = $$props;
  let { line_breaks = true } = $$props;
  let { autoscroll = true } = $$props;
  let { _retryable = false } = $$props;
  let { _undoable = false } = $$props;
  let { latex_delimiters } = $$props;
  let { gradio } = $$props;
  let _value = [];
  let { avatar_images = [null, null] } = $$props;
  let { like_user_message = false } = $$props;
  let { loading_status = void 0 } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let { placeholder = null } = $$props;
  let { examples = null } = $$props;
  let { theme_mode } = $$props;
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props._selectable === void 0 && $$bindings._selectable && _selectable !== void 0)
    $$bindings._selectable(_selectable);
  if ($$props.likeable === void 0 && $$bindings.likeable && likeable !== void 0)
    $$bindings.likeable(likeable);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.rtl === void 0 && $$bindings.rtl && rtl !== void 0)
    $$bindings.rtl(rtl);
  if ($$props.show_copy_button === void 0 && $$bindings.show_copy_button && show_copy_button !== void 0)
    $$bindings.show_copy_button(show_copy_button);
  if ($$props.show_copy_all_button === void 0 && $$bindings.show_copy_all_button && show_copy_all_button !== void 0)
    $$bindings.show_copy_all_button(show_copy_all_button);
  if ($$props.sanitize_html === void 0 && $$bindings.sanitize_html && sanitize_html !== void 0)
    $$bindings.sanitize_html(sanitize_html);
  if ($$props.bubble_full_width === void 0 && $$bindings.bubble_full_width && bubble_full_width !== void 0)
    $$bindings.bubble_full_width(bubble_full_width);
  if ($$props.layout === void 0 && $$bindings.layout && layout !== void 0)
    $$bindings.layout(layout);
  if ($$props.type === void 0 && $$bindings.type && type !== void 0)
    $$bindings.type(type);
  if ($$props.render_markdown === void 0 && $$bindings.render_markdown && render_markdown !== void 0)
    $$bindings.render_markdown(render_markdown);
  if ($$props.line_breaks === void 0 && $$bindings.line_breaks && line_breaks !== void 0)
    $$bindings.line_breaks(line_breaks);
  if ($$props.autoscroll === void 0 && $$bindings.autoscroll && autoscroll !== void 0)
    $$bindings.autoscroll(autoscroll);
  if ($$props._retryable === void 0 && $$bindings._retryable && _retryable !== void 0)
    $$bindings._retryable(_retryable);
  if ($$props._undoable === void 0 && $$bindings._undoable && _undoable !== void 0)
    $$bindings._undoable(_undoable);
  if ($$props.latex_delimiters === void 0 && $$bindings.latex_delimiters && latex_delimiters !== void 0)
    $$bindings.latex_delimiters(latex_delimiters);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  if ($$props.avatar_images === void 0 && $$bindings.avatar_images && avatar_images !== void 0)
    $$bindings.avatar_images(avatar_images);
  if ($$props.like_user_message === void 0 && $$bindings.like_user_message && like_user_message !== void 0)
    $$bindings.like_user_message(like_user_message);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.height === void 0 && $$bindings.height && height !== void 0)
    $$bindings.height(height);
  if ($$props.min_height === void 0 && $$bindings.min_height && min_height !== void 0)
    $$bindings.min_height(min_height);
  if ($$props.max_height === void 0 && $$bindings.max_height && max_height !== void 0)
    $$bindings.max_height(max_height);
  if ($$props.placeholder === void 0 && $$bindings.placeholder && placeholder !== void 0)
    $$bindings.placeholder(placeholder);
  if ($$props.examples === void 0 && $$bindings.examples && examples !== void 0)
    $$bindings.examples(examples);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  $$result.css.add(css);
  _value = type === "tuples" ? normalise_tuples(value, root) : normalise_messages(value, root);
  return `${validate_component(Block, "Block").$$render(
    $$result,
    {
      elem_id,
      elem_classes,
      visible,
      padding: false,
      scale,
      min_width,
      height,
      min_height,
      max_height,
      allow_overflow: true,
      flex: true,
      overflow_behavior: "auto"
    },
    {},
    {
      default: () => {
        return `${loading_status ? `${validate_component(Static, "StatusTracker").$$render(
          $$result,
          Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status, {
            show_progress: loading_status.show_progress === "hidden" ? "hidden" : "minimal"
          }),
          {},
          {}
        )}` : ``} <div class="wrapper svelte-g3p8na">${show_label ? `${validate_component(BlockLabel, "BlockLabel").$$render(
          $$result,
          {
            show_label,
            Icon: Chat,
            float: true,
            label: label || "Chatbot"
          },
          {},
          {}
        )}` : ``} ${validate_component(ChatBot$1, "ChatBot").$$render(
          $$result,
          {
            i18n: gradio.i18n,
            selectable: _selectable,
            likeable,
            show_share_button,
            show_copy_all_button,
            value: _value,
            latex_delimiters,
            render_markdown,
            theme_mode,
            pending_message: loading_status?.status === "pending",
            generating: loading_status?.status === "generating",
            rtl,
            show_copy_button,
            like_user_message,
            avatar_images,
            sanitize_html,
            bubble_full_width,
            line_breaks,
            autoscroll,
            layout,
            placeholder,
            examples,
            _retryable,
            _undoable,
            upload: (...args) => gradio.client.upload(...args),
            _fetch: (...args) => gradio.client.fetch(...args),
            load_component: gradio.load_component,
            msg_format: type,
            root: gradio.root
          },
          {},
          {}
        )}</div>`;
      }
    }
  )}`;
});

export { ChatBot$1 as BaseChatBot, Index as default };
//# sourceMappingURL=Index56-B8Q3Lwup.js.map
