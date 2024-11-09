import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, empty, group_outros, transition_out, check_outros, transition_in, bubble, construct_svelte_component, create_component, claim_component, mount_component, destroy_component, element, claim_element, text, space, claim_text, claim_space, get_svelte_dataset, set_style, listen, set_data, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, onDestroy, ensure_array_like, toggle_class, destroy_each, run_all, assign, get_spread_update, get_spread_object, is_function, null_to_empty, onMount, createEventDispatcher, afterUpdate, binding_callbacks, action_destroyer, tick, src_url_equal } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { u as uploadToHuggingFace, I as IconButton, q as Check, r as Copy, M as MarkdownCode, d as ShareError, n as copy, C as Clear, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { I as Image } from "./Image.eJ_qOnkr.js";
/* empty css                                                    */
/* empty css                                                    */
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
import { U as Undo } from "./Undo.BEjgqHJW.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { d as dequal } from "./index.DTU9giFV.js";
import { C as Community } from "./Community.4FBzcHeP.js";
import { T as Trash } from "./Trash.phFEWuFK.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
function create_fragment$h(ctx) {
  let svg;
  let path0;
  let path1;
  return {
    c() {
      svg = svg_element("svg");
      path0 = svg_element("path");
      path1 = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        "xmlns:xlink": true,
        "aria-hidden": true,
        role: true,
        class: true,
        width: true,
        height: true,
        preserveAspectRatio: true,
        viewBox: true
      });
      var svg_nodes = children(svg);
      path0 = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "fill", "currentColor");
      attr(path0, "d", "M17.74 30L16 29l4-7h6a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h9v2H6a4 4 0 0 1-4-4V8a4 4 0 0 1 4-4h20a4 4 0 0 1 4 4v12a4 4 0 0 1-4 4h-4.84Z");
      attr(path1, "fill", "currentColor");
      attr(path1, "d", "M8 10h16v2H8zm0 6h10v2H8z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "xmlns:xlink", "http://www.w3.org/1999/xlink");
      attr(svg, "aria-hidden", "true");
      attr(svg, "role", "img");
      attr(svg, "class", "iconify iconify--carbon");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "preserveAspectRatio", "xMidYMid meet");
      attr(svg, "viewBox", "0 0 32 32");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class Chat extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$h, safe_not_equal, {});
  }
}
function create_fragment$g(ctx) {
  let svg;
  let path0;
  let path1;
  let path2;
  let path3;
  return {
    c() {
      svg = svg_element("svg");
      path0 = svg_element("path");
      path1 = svg_element("path");
      path2 = svg_element("path");
      path3 = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        "stroke-width": true,
        viewBox: true,
        fill: true,
        xmlns: true,
        color: true
      });
      var svg_nodes = children(svg);
      path0 = claim_svg_element(svg_nodes, "path", {
        d: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", {
        d: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path1).forEach(detach);
      path2 = claim_svg_element(svg_nodes, "path", {
        d: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path2).forEach(detach);
      path3 = claim_svg_element(svg_nodes, "path", {
        d: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path3).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M19.1679 9C18.0247 6.46819 15.3006 4.5 11.9999 4.5C8.31459 4.5 5.05104 7.44668 4.54932 11");
      attr(path0, "stroke", "currentColor");
      attr(path0, "stroke-width", "1.5");
      attr(path0, "stroke-linecap", "round");
      attr(path0, "stroke-linejoin", "round");
      attr(path1, "d", "M16 9H19.4C19.7314 9 20 8.73137 20 8.4V5");
      attr(path1, "stroke", "currentColor");
      attr(path1, "stroke-width", "1.5");
      attr(path1, "stroke-linecap", "round");
      attr(path1, "stroke-linejoin", "round");
      attr(path2, "d", "M4.88146 15C5.92458 17.5318 8.64874 19.5 12.0494 19.5C15.7347 19.5 18.9983 16.5533 19.5 13");
      attr(path2, "stroke", "currentColor");
      attr(path2, "stroke-width", "1.5");
      attr(path2, "stroke-linecap", "round");
      attr(path2, "stroke-linejoin", "round");
      attr(path3, "d", "M8.04932 15H4.64932C4.31795 15 4.04932 15.2686 4.04932 15.6V19");
      attr(path3, "stroke", "currentColor");
      attr(path3, "stroke-width", "1.5");
      attr(path3, "stroke-linecap", "round");
      attr(path3, "stroke-linejoin", "round");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
      append_hydration(svg, path2);
      append_hydration(svg, path3);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class Retry extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$g, safe_not_equal, {});
  }
}
function create_fragment$f(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", {
        d: true,
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M12 20L12 4M12 20L7 15M12 20L17 15");
      attr(path, "stroke", "currentColor");
      attr(path, "stroke-width", "2");
      attr(path, "stroke-linecap", "round");
      attr(path, "stroke-linejoin", "round");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class ScrollDownArrow extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$f, safe_not_equal, {});
  }
}
const format_chat_for_sharing = async (chat) => {
  let messages = await Promise.all(
    chat.map(async (message_pair) => {
      return await Promise.all(
        message_pair.map(async (message, i) => {
          var _a, _b, _c;
          if (message === null)
            return "";
          let speaker_emoji = i === 0 ? "😃" : "🤖";
          let html_content = "";
          if (typeof message === "string") {
            const regexPatterns = {
              audio: /<audio.*?src="(\/file=.*?)"/g,
              video: /<video.*?src="(\/file=.*?)"/g,
              image: /<img.*?src="(\/file=.*?)".*?\/>|!\[.*?\]\((\/file=.*?)\)/g
            };
            html_content = message;
            for (let [_, regex] of Object.entries(regexPatterns)) {
              let match;
              while ((match = regex.exec(message)) !== null) {
                const fileUrl = match[1] || match[2];
                const newUrl = await uploadToHuggingFace(fileUrl);
                html_content = html_content.replace(fileUrl, newUrl);
              }
            }
          } else {
            if (!(message == null ? void 0 : message.url))
              return "";
            const file_url = await uploadToHuggingFace(message.url);
            if ((_a = message.mime_type) == null ? void 0 : _a.includes("audio")) {
              html_content = `<audio controls src="${file_url}"></audio>`;
            } else if ((_b = message.mime_type) == null ? void 0 : _b.includes("video")) {
              html_content = file_url;
            } else if ((_c = message.mime_type) == null ? void 0 : _c.includes("image")) {
              html_content = `<img src="${file_url}" />`;
            }
          }
          return `${speaker_emoji}: ${html_content}`;
        })
      );
    })
  );
  return messages.map(
    (message_pair) => message_pair.join(
      message_pair[0] !== "" && message_pair[1] !== "" ? "\n" : ""
    )
  ).join("\n");
};
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
    component: get_component_for_mime_type(_file == null ? void 0 : _file.mime_type),
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
function create_if_block_5$3(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[2]
        ),
        show_label: false,
        label: "chatbot-image",
        show_share_button: true,
        i18n: (
          /*i18n*/
          ctx2[6]
        ),
        gradio: { dispatch: func }
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler_5*/
      ctx[14]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler_5*/
            ctx2[14]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2];
        if (dirty & /*i18n*/
        64)
          switch_instance_changes.i18n = /*i18n*/
          ctx2[6];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_4$3(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[2]
        ),
        show_label: false,
        label: "chatbot-image",
        show_download_button: false,
        i18n: (
          /*i18n*/
          ctx2[6]
        )
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler_4*/
      ctx[13]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler_4*/
            ctx2[13]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2];
        if (dirty & /*i18n*/
        64)
          switch_instance_changes.i18n = /*i18n*/
          ctx2[6];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_3$3(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        autoplay: true,
        value: (
          /*value*/
          ctx2[2].video || /*value*/
          ctx2[2]
        ),
        show_label: false,
        show_share_button: true,
        i18n: (
          /*i18n*/
          ctx2[6]
        ),
        upload: (
          /*upload*/
          ctx2[7]
        ),
        show_download_button: false,
        $$slots: { default: [create_default_slot$4] },
        $$scope: { ctx: ctx2 }
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler_3*/
      ctx[12]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler_3*/
            ctx2[12]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2].video || /*value*/
          ctx2[2];
        if (dirty & /*i18n*/
        64)
          switch_instance_changes.i18n = /*i18n*/
          ctx2[6];
        if (dirty & /*upload*/
        128)
          switch_instance_changes.upload = /*upload*/
          ctx2[7];
        if (dirty & /*$$scope*/
        32768) {
          switch_instance_changes.$$scope = { dirty, ctx: ctx2 };
        }
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_2$3(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[2]
        ),
        show_label: false,
        show_share_button: true,
        i18n: (
          /*i18n*/
          ctx2[6]
        ),
        label: "",
        waveform_settings: {},
        waveform_options: {},
        show_download_button: false
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler_2*/
      ctx[11]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler_2*/
            ctx2[11]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2];
        if (dirty & /*i18n*/
        64)
          switch_instance_changes.i18n = /*i18n*/
          ctx2[6];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block_1$4(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[2]
        ),
        target: (
          /*target*/
          ctx2[3]
        ),
        theme_mode: (
          /*theme_mode*/
          ctx2[4]
        ),
        bokeh_version: (
          /*props*/
          ctx2[5].bokeh_version
        ),
        caption: "",
        show_actions_button: true
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler_1*/
      ctx[10]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler_1*/
            ctx2[10]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2];
        if (dirty & /*target*/
        8)
          switch_instance_changes.target = /*target*/
          ctx2[3];
        if (dirty & /*theme_mode*/
        16)
          switch_instance_changes.theme_mode = /*theme_mode*/
          ctx2[4];
        if (dirty & /*props*/
        32)
          switch_instance_changes.bokeh_version = /*props*/
          ctx2[5].bokeh_version;
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_if_block$5(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*components*/
    ctx[1][
      /*type*/
      ctx[0]
    ]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[2]
        ),
        show_label: false,
        i18n: (
          /*i18n*/
          ctx2[6]
        ),
        label: "",
        _fetch: (
          /*_fetch*/
          ctx2[8]
        ),
        allow_preview: false,
        interactive: false,
        mode: "minimal",
        fixed_height: 1
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler*/
      ctx[9]
    );
  }
  return {
    c() {
      if (switch_instance)
        create_component(switch_instance.$$.fragment);
      switch_instance_anchor = empty();
    },
    l(nodes) {
      if (switch_instance)
        claim_component(switch_instance.$$.fragment, nodes);
      switch_instance_anchor = empty();
    },
    m(target, anchor) {
      if (switch_instance)
        mount_component(switch_instance, target, anchor);
      insert_hydration(target, switch_instance_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*components, type*/
      3 && switch_value !== (switch_value = /*components*/
      ctx2[1][
        /*type*/
        ctx2[0]
      ])) {
        if (switch_instance) {
          group_outros();
          const old_component = switch_instance;
          transition_out(old_component.$$.fragment, 1, 0, () => {
            destroy_component(old_component, 1);
          });
          check_outros();
        }
        if (switch_value) {
          switch_instance = construct_svelte_component(switch_value, switch_props(ctx2));
          switch_instance.$on(
            "load",
            /*load_handler*/
            ctx2[9]
          );
          create_component(switch_instance.$$.fragment);
          transition_in(switch_instance.$$.fragment, 1);
          mount_component(switch_instance, switch_instance_anchor.parentNode, switch_instance_anchor);
        } else {
          switch_instance = null;
        }
      } else if (switch_value) {
        const switch_instance_changes = {};
        if (dirty & /*value*/
        4)
          switch_instance_changes.value = /*value*/
          ctx2[2];
        if (dirty & /*i18n*/
        64)
          switch_instance_changes.i18n = /*i18n*/
          ctx2[6];
        if (dirty & /*_fetch*/
        256)
          switch_instance_changes._fetch = /*_fetch*/
          ctx2[8];
        switch_instance.$set(switch_instance_changes);
      }
    },
    i(local) {
      if (current)
        return;
      if (switch_instance)
        transition_in(switch_instance.$$.fragment, local);
      current = true;
    },
    o(local) {
      if (switch_instance)
        transition_out(switch_instance.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(switch_instance_anchor);
      }
      if (switch_instance)
        destroy_component(switch_instance, detaching);
    }
  };
}
function create_default_slot$4(ctx) {
  let track;
  return {
    c() {
      track = element("track");
      this.h();
    },
    l(nodes) {
      track = claim_element(nodes, "TRACK", { kind: true });
      this.h();
    },
    h() {
      attr(track, "kind", "captions");
    },
    m(target, anchor) {
      insert_hydration(target, track, anchor);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(track);
      }
    }
  };
}
function create_fragment$e(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [
    create_if_block$5,
    create_if_block_1$4,
    create_if_block_2$3,
    create_if_block_3$3,
    create_if_block_4$3,
    create_if_block_5$3
  ];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*type*/
      ctx2[0] === "gallery"
    )
      return 0;
    if (
      /*type*/
      ctx2[0] === "plot"
    )
      return 1;
    if (
      /*type*/
      ctx2[0] === "audio"
    )
      return 2;
    if (
      /*type*/
      ctx2[0] === "video"
    )
      return 3;
    if (
      /*type*/
      ctx2[0] === "image"
    )
      return 4;
    if (
      /*type*/
      ctx2[0] === "html"
    )
      return 5;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
    if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  return {
    c() {
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(target, anchor);
      }
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx2, dirty);
        }
      } else {
        if (if_block) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block = if_blocks[current_block_type_index];
          if (!if_block) {
            if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
            if_block.c();
          } else {
            if_block.p(ctx2, dirty);
          }
          transition_in(if_block, 1);
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        } else {
          if_block = null;
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d(detaching);
      }
    }
  };
}
const func = () => {
};
function instance$9($$self, $$props, $$invalidate) {
  let { type } = $$props;
  let { components } = $$props;
  let { value } = $$props;
  let { target } = $$props;
  let { theme_mode } = $$props;
  let { props } = $$props;
  let { i18n } = $$props;
  let { upload } = $$props;
  let { _fetch } = $$props;
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler_1(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler_2(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler_3(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler_4(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler_5(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("type" in $$props2)
      $$invalidate(0, type = $$props2.type);
    if ("components" in $$props2)
      $$invalidate(1, components = $$props2.components);
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
    if ("target" in $$props2)
      $$invalidate(3, target = $$props2.target);
    if ("theme_mode" in $$props2)
      $$invalidate(4, theme_mode = $$props2.theme_mode);
    if ("props" in $$props2)
      $$invalidate(5, props = $$props2.props);
    if ("i18n" in $$props2)
      $$invalidate(6, i18n = $$props2.i18n);
    if ("upload" in $$props2)
      $$invalidate(7, upload = $$props2.upload);
    if ("_fetch" in $$props2)
      $$invalidate(8, _fetch = $$props2._fetch);
  };
  return [
    type,
    components,
    value,
    target,
    theme_mode,
    props,
    i18n,
    upload,
    _fetch,
    load_handler,
    load_handler_1,
    load_handler_2,
    load_handler_3,
    load_handler_4,
    load_handler_5
  ];
}
class Component extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$9, create_fragment$e, safe_not_equal, {
      type: 0,
      components: 1,
      value: 2,
      target: 3,
      theme_mode: 4,
      props: 5,
      i18n: 6,
      upload: 7,
      _fetch: 8
    });
  }
}
function create_if_block$4(ctx) {
  let div;
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[4].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[3],
    null
  );
  return {
    c() {
      div = element("div");
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if (default_slot)
        default_slot.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "content svelte-1e60bn1");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (default_slot) {
        default_slot.m(div, null);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        8)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[3],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[3]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[3],
              dirty,
              null
            ),
            null
          );
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(default_slot, local);
      current = true;
    },
    o(local) {
      transition_out(default_slot, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_fragment$d(ctx) {
  let button;
  let div;
  let span0;
  let t0;
  let t1;
  let span1;
  let textContent = "▼";
  let t3;
  let current;
  let mounted;
  let dispose;
  let if_block = (
    /*expanded*/
    ctx[0] && create_if_block$4(ctx)
  );
  return {
    c() {
      button = element("button");
      div = element("div");
      span0 = element("span");
      t0 = text(
        /*title*/
        ctx[1]
      );
      t1 = space();
      span1 = element("span");
      span1.textContent = textContent;
      t3 = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      div = claim_element(button_nodes, "DIV", { class: true });
      var div_nodes = children(div);
      span0 = claim_element(div_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(
        span0_nodes,
        /*title*/
        ctx[1]
      );
      span0_nodes.forEach(detach);
      t1 = claim_space(div_nodes);
      span1 = claim_element(div_nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span1) !== "svelte-15ydlzc")
        span1.textContent = textContent;
      div_nodes.forEach(detach);
      t3 = claim_space(button_nodes);
      if (if_block)
        if_block.l(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "title-text svelte-1e60bn1");
      attr(span1, "class", "arrow svelte-1e60bn1");
      set_style(
        span1,
        "transform",
        /*expanded*/
        ctx[0] ? "rotate(0)" : "rotate(90deg)"
      );
      attr(div, "class", "title svelte-1e60bn1");
      attr(button, "class", "box svelte-1e60bn1");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, div);
      append_hydration(div, span0);
      append_hydration(span0, t0);
      append_hydration(div, t1);
      append_hydration(div, span1);
      append_hydration(button, t3);
      if (if_block)
        if_block.m(button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*toggleExpanded*/
          ctx[2]
        );
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (!current || dirty & /*title*/
      2)
        set_data(
          t0,
          /*title*/
          ctx2[1]
        );
      if (dirty & /*expanded*/
      1) {
        set_style(
          span1,
          "transform",
          /*expanded*/
          ctx2[0] ? "rotate(0)" : "rotate(90deg)"
        );
      }
      if (
        /*expanded*/
        ctx2[0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*expanded*/
          1) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block$4(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(button, null);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      if (if_block)
        if_block.d();
      mounted = false;
      dispose();
    }
  };
}
function instance$8($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { expanded = false } = $$props;
  let { title } = $$props;
  function toggleExpanded() {
    $$invalidate(0, expanded = !expanded);
  }
  $$self.$$set = ($$props2) => {
    if ("expanded" in $$props2)
      $$invalidate(0, expanded = $$props2.expanded);
    if ("title" in $$props2)
      $$invalidate(1, title = $$props2.title);
    if ("$$scope" in $$props2)
      $$invalidate(3, $$scope = $$props2.$$scope);
  };
  return [expanded, title, toggleExpanded, $$scope, slots];
}
class MessageBox extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$8, create_fragment$d, safe_not_equal, { expanded: 0, title: 1 });
  }
}
function create_fragment$c(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M11.25 6.61523H9.375V1.36523H11.25V6.61523ZM3.375 1.36523H8.625V6.91636L7.48425 8.62748L7.16737 10.8464C7.14108 11.0248 7.05166 11.1879 6.91535 11.3061C6.77904 11.4242 6.60488 11.4896 6.4245 11.4902H6.375C6.07672 11.4899 5.79075 11.3713 5.57983 11.1604C5.36892 10.9495 5.2503 10.6635 5.25 10.3652V8.11523H2.25C1.85233 8.11474 1.47109 7.95654 1.18989 7.67535C0.908691 7.39415 0.750496 7.01291 0.75 6.61523V3.99023C0.750992 3.29435 1.02787 2.62724 1.51994 2.13517C2.01201 1.64311 2.67911 1.36623 3.375 1.36523Z");
      attr(path, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 12 12");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class ThumbDownActive extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$c, safe_not_equal, {});
  }
}
function create_fragment$b(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M2.25 8.11523H4.5V10.3652C4.5003 10.6635 4.61892 10.9495 4.82983 11.1604C5.04075 11.3713 5.32672 11.4899 5.625 11.4902H6.42488C6.60519 11.4895 6.77926 11.4241 6.91549 11.3059C7.05172 11.1878 7.14109 11.0248 7.16737 10.8464L7.48425 8.62748L8.82562 6.61523H11.25V1.36523H3.375C2.67911 1.36623 2.01201 1.64311 1.51994 2.13517C1.02787 2.62724 0.750992 3.29435 0.75 3.99023V6.61523C0.750496 7.01291 0.908691 7.39415 1.18989 7.67535C1.47109 7.95654 1.85233 8.11474 2.25 8.11523ZM9 2.11523H10.5V5.86523H9V2.11523ZM1.5 3.99023C1.5006 3.49314 1.69833 3.01657 2.04983 2.66507C2.40133 2.31356 2.8779 2.11583 3.375 2.11523H8.25V6.12661L6.76575 8.35298L6.4245 10.7402H5.625C5.52554 10.7402 5.43016 10.7007 5.35983 10.6304C5.28951 10.5601 5.25 10.4647 5.25 10.3652V7.36523H2.25C2.05118 7.36494 1.86059 7.28582 1.72 7.14524C1.57941 7.00465 1.5003 6.81406 1.5 6.61523V3.99023Z");
      attr(path, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 12 12");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class ThumbDownDefault extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$b, safe_not_equal, {});
  }
}
function create_fragment$a(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M0.75 6.24023H2.625V11.4902H0.75V6.24023ZM8.625 11.4902H3.375V5.93911L4.51575 4.22798L4.83263 2.00911C4.85892 1.83065 4.94834 1.66754 5.08465 1.5494C5.22096 1.43125 5.39512 1.36591 5.5755 1.36523H5.625C5.92328 1.36553 6.20925 1.48415 6.42017 1.69507C6.63108 1.90598 6.7497 2.19196 6.75 2.49023V4.74023H9.75C10.1477 4.74073 10.5289 4.89893 10.8101 5.18012C11.0913 5.46132 11.2495 5.84256 11.25 6.24023V8.86523C11.249 9.56112 10.9721 10.2282 10.4801 10.7203C9.98799 11.2124 9.32089 11.4892 8.625 11.4902Z");
      attr(path, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 12 12");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class ThumbUpActive extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$a, safe_not_equal, {});
  }
}
function create_fragment$9(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M9.75 4.74023H7.5V2.49023C7.4997 2.19196 7.38108 1.90598 7.17017 1.69507C6.95925 1.48415 6.67328 1.36553 6.375 1.36523H5.57512C5.39481 1.366 5.22074 1.43138 5.08451 1.54952C4.94828 1.66766 4.85891 1.83072 4.83262 2.00911L4.51575 4.22798L3.17438 6.24023H0.75V11.4902H8.625C9.32089 11.4892 9.98799 11.2124 10.4801 10.7203C10.9721 10.2282 11.249 9.56112 11.25 8.86523V6.24023C11.2495 5.84256 11.0913 5.46132 10.8101 5.18012C10.5289 4.89893 10.1477 4.74073 9.75 4.74023ZM3 10.7402H1.5V6.99023H3V10.7402ZM10.5 8.86523C10.4994 9.36233 10.3017 9.8389 9.95017 10.1904C9.59867 10.5419 9.1221 10.7396 8.625 10.7402H3.75V6.72886L5.23425 4.50248L5.5755 2.11523H6.375C6.47446 2.11523 6.56984 2.15474 6.64017 2.22507C6.71049 2.2954 6.75 2.39078 6.75 2.49023V5.49023H9.75C9.94882 5.49053 10.1394 5.56965 10.28 5.71023C10.4206 5.85082 10.4997 6.04141 10.5 6.24023V8.86523Z");
      attr(path, "fill", "currentColor");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 12 12");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class ThumbUpDefault extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$9, safe_not_equal, {});
  }
}
function create_fragment$8(ctx) {
  let iconbutton0;
  let t;
  let iconbutton1;
  let current;
  iconbutton0 = new IconButton({
    props: {
      Icon: (
        /*selected*/
        ctx[1] === "dislike" ? ThumbDownActive : ThumbDownDefault
      ),
      label: (
        /*selected*/
        ctx[1] === "dislike" ? "clicked dislike" : "dislike"
      ),
      color: (
        /*selected*/
        ctx[1] === "dislike" ? "var(--color-accent)" : "var(--block-label-text-color)"
      )
    }
  });
  iconbutton0.$on(
    "click",
    /*click_handler*/
    ctx[2]
  );
  iconbutton1 = new IconButton({
    props: {
      Icon: (
        /*selected*/
        ctx[1] === "like" ? ThumbUpActive : ThumbUpDefault
      ),
      label: (
        /*selected*/
        ctx[1] === "like" ? "clicked like" : "like"
      ),
      color: (
        /*selected*/
        ctx[1] === "like" ? "var(--color-accent)" : "var(--block-label-text-color)"
      )
    }
  });
  iconbutton1.$on(
    "click",
    /*click_handler_1*/
    ctx[3]
  );
  return {
    c() {
      create_component(iconbutton0.$$.fragment);
      t = space();
      create_component(iconbutton1.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton0.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(iconbutton1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton0, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(iconbutton1, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const iconbutton0_changes = {};
      if (dirty & /*selected*/
      2)
        iconbutton0_changes.Icon = /*selected*/
        ctx2[1] === "dislike" ? ThumbDownActive : ThumbDownDefault;
      if (dirty & /*selected*/
      2)
        iconbutton0_changes.label = /*selected*/
        ctx2[1] === "dislike" ? "clicked dislike" : "dislike";
      if (dirty & /*selected*/
      2)
        iconbutton0_changes.color = /*selected*/
        ctx2[1] === "dislike" ? "var(--color-accent)" : "var(--block-label-text-color)";
      iconbutton0.$set(iconbutton0_changes);
      const iconbutton1_changes = {};
      if (dirty & /*selected*/
      2)
        iconbutton1_changes.Icon = /*selected*/
        ctx2[1] === "like" ? ThumbUpActive : ThumbUpDefault;
      if (dirty & /*selected*/
      2)
        iconbutton1_changes.label = /*selected*/
        ctx2[1] === "like" ? "clicked like" : "like";
      if (dirty & /*selected*/
      2)
        iconbutton1_changes.color = /*selected*/
        ctx2[1] === "like" ? "var(--color-accent)" : "var(--block-label-text-color)";
      iconbutton1.$set(iconbutton1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton0.$$.fragment, local);
      transition_in(iconbutton1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton0.$$.fragment, local);
      transition_out(iconbutton1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(iconbutton0, detaching);
      destroy_component(iconbutton1, detaching);
    }
  };
}
function instance$7($$self, $$props, $$invalidate) {
  let { handle_action } = $$props;
  let selected = null;
  const click_handler = () => {
    $$invalidate(1, selected = "dislike");
    handle_action(selected);
  };
  const click_handler_1 = () => {
    $$invalidate(1, selected = "like");
    handle_action(selected);
  };
  $$self.$$set = ($$props2) => {
    if ("handle_action" in $$props2)
      $$invalidate(0, handle_action = $$props2.handle_action);
  };
  return [handle_action, selected, click_handler, click_handler_1];
}
class LikeDislike extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$7, create_fragment$8, safe_not_equal, { handle_action: 0 });
  }
}
function create_fragment$7(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      label: (
        /*copied*/
        ctx[0] ? "Copied message" : "Copy message"
      ),
      Icon: (
        /*copied*/
        ctx[0] ? Check : Copy
      )
    }
  });
  iconbutton.$on(
    "click",
    /*handle_copy*/
    ctx[1]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const iconbutton_changes = {};
      if (dirty & /*copied*/
      1)
        iconbutton_changes.label = /*copied*/
        ctx2[0] ? "Copied message" : "Copy message";
      if (dirty & /*copied*/
      1)
        iconbutton_changes.Icon = /*copied*/
        ctx2[0] ? Check : Copy;
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function instance$6($$self, $$props, $$invalidate) {
  let copied = false;
  let { value } = $$props;
  let timer;
  function copy_feedback() {
    $$invalidate(0, copied = true);
    if (timer)
      clearTimeout(timer);
    timer = setTimeout(
      () => {
        $$invalidate(0, copied = false);
      },
      2e3
    );
  }
  async function handle_copy() {
    if ("clipboard" in navigator) {
      await navigator.clipboard.writeText(value);
      copy_feedback();
    } else {
      const textArea = document.createElement("textarea");
      textArea.value = value;
      textArea.style.position = "absolute";
      textArea.style.left = "-999999px";
      document.body.prepend(textArea);
      textArea.select();
      try {
        document.execCommand("copy");
        copy_feedback();
      } catch (error) {
        console.error(error);
      } finally {
        textArea.remove();
      }
    }
  }
  onDestroy(() => {
    if (timer)
      clearTimeout(timer);
  });
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
  };
  return [copied, handle_copy, value];
}
class Copy_1 extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$6, create_fragment$7, safe_not_equal, { value: 2 });
  }
}
function create_fragment$6(ctx) {
  let svg;
  let path0;
  let path1;
  return {
    c() {
      svg = svg_element("svg");
      path0 = svg_element("path");
      path1 = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        fill: true,
        xmlns: true
      });
      var svg_nodes = children(svg);
      path0 = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", { d: true, fill: true });
      children(path1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M6.27701 8.253C6.24187 8.29143 6.19912 8.32212 6.15147 8.34311C6.10383 8.36411 6.05233 8.37495 6.00026 8.37495C5.94819 8.37495 5.89669 8.36411 5.84905 8.34311C5.8014 8.32212 5.75865 8.29143 5.72351 8.253L3.72351 6.0655C3.65798 5.99185 3.62408 5.89536 3.62916 5.79691C3.63424 5.69846 3.67788 5.60596 3.75064 5.53945C3.8234 5.47293 3.91943 5.43774 4.01794 5.44149C4.11645 5.44525 4.20952 5.48764 4.27701 5.5595L5.62501 7.0345V1.5C5.62501 1.40054 5.66452 1.30516 5.73485 1.23483C5.80517 1.16451 5.90055 1.125 6.00001 1.125C6.09947 1.125 6.19485 1.16451 6.26517 1.23483C6.3355 1.30516 6.37501 1.40054 6.37501 1.5V7.034L7.72351 5.559C7.79068 5.4856 7.88425 5.44189 7.98364 5.43748C8.08304 5.43308 8.18011 5.46833 8.25351 5.5355C8.32691 5.60267 8.37062 5.69624 8.37503 5.79563C8.37943 5.89503 8.34418 5.9921 8.27701 6.0655L6.27701 8.253Z");
      attr(path0, "fill", "currentColor");
      attr(path1, "d", "M1.875 7.39258C1.875 7.29312 1.83549 7.19774 1.76517 7.12741C1.69484 7.05709 1.59946 7.01758 1.5 7.01758C1.40054 7.01758 1.30516 7.05709 1.23483 7.12741C1.16451 7.19774 1.125 7.29312 1.125 7.39258V7.42008C1.125 8.10358 1.125 8.65508 1.1835 9.08858C1.2435 9.53858 1.3735 9.91758 1.674 10.2186C1.975 10.5196 2.354 10.6486 2.804 10.7096C3.2375 10.7676 3.789 10.7676 4.4725 10.7676H7.5275C8.211 10.7676 8.7625 10.7676 9.196 10.7096C9.646 10.6486 10.025 10.5196 10.326 10.2186C10.627 9.91758 10.756 9.53858 10.817 9.08858C10.875 8.65508 10.875 8.10358 10.875 7.42008V7.39258C10.875 7.29312 10.8355 7.19774 10.7652 7.12741C10.6948 7.05709 10.5995 7.01758 10.5 7.01758C10.4005 7.01758 10.3052 7.05709 10.2348 7.12741C10.1645 7.19774 10.125 7.29312 10.125 7.39258C10.125 8.11008 10.124 8.61058 10.0735 8.98858C10.024 9.35558 9.9335 9.54958 9.7955 9.68808C9.657 9.82658 9.463 9.91658 9.0955 9.96608C8.718 10.0166 8.2175 10.0176 7.5 10.0176H4.5C3.7825 10.0176 3.2815 10.0166 2.904 9.96608C2.537 9.91658 2.343 9.82608 2.2045 9.68808C2.066 9.54958 1.976 9.35558 1.9265 8.98808C1.876 8.61058 1.875 8.11008 1.875 7.39258Z");
      attr(path1, "fill", "currentColor");
      attr(svg, "width", "16");
      attr(svg, "height", "16");
      attr(svg, "viewBox", "0 0 12 12");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(svg);
      }
    }
  };
}
class Download extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$6, safe_not_equal, {});
  }
}
function create_if_block$3(ctx) {
  let div;
  let iconbuttonwrapper;
  let div_class_value;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      top_panel: false,
      $$slots: { default: [create_default_slot$3] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      div = element("div");
      create_component(iconbuttonwrapper.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(iconbuttonwrapper.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", div_class_value = "message-buttons-" + /*position*/
      ctx[5] + " " + /*layout*/
      ctx[9] + " message-buttons " + /*avatar*/
      (ctx[6] !== null && "with-avatar") + " svelte-1ibfe7l");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(iconbuttonwrapper, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, handle_action, likeable, generating, show_undo, show_retry, message, show_download, message_text, show_copy*/
      73111) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      if (!current || dirty & /*position, layout, avatar*/
      608 && div_class_value !== (div_class_value = "message-buttons-" + /*position*/
      ctx2[5] + " " + /*layout*/
      ctx2[9] + " message-buttons " + /*avatar*/
      (ctx2[6] !== null && "with-avatar") + " svelte-1ibfe7l")) {
        attr(div, "class", div_class_value);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(iconbuttonwrapper);
    }
  };
}
function create_if_block_5$2(ctx) {
  let copy2;
  let current;
  copy2 = new Copy_1({
    props: { value: (
      /*message_text*/
      ctx[12]
    ) }
  });
  return {
    c() {
      create_component(copy2.$$.fragment);
    },
    l(nodes) {
      claim_component(copy2.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(copy2, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const copy_changes = {};
      if (dirty & /*message_text*/
      4096)
        copy_changes.value = /*message_text*/
        ctx2[12];
      copy2.$set(copy_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(copy2.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(copy2.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(copy2, detaching);
    }
  };
}
function create_if_block_4$2(ctx) {
  var _a, _b;
  let downloadlink;
  let current;
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*message*/
        (_b = (_a = ctx[4]) == null ? void 0 : _a.content) == null ? void 0 : _b.value.url
      ),
      download: (
        /*message*/
        ctx[4].content.value.orig_name || "image"
      ),
      $$slots: { default: [create_default_slot_1$1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(downloadlink.$$.fragment);
    },
    l(nodes) {
      claim_component(downloadlink.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(downloadlink, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2, _b2;
      const downloadlink_changes = {};
      if (dirty & /*message*/
      16)
        downloadlink_changes.href = /*message*/
        (_b2 = (_a2 = ctx2[4]) == null ? void 0 : _a2.content) == null ? void 0 : _b2.value.url;
      if (dirty & /*message*/
      16)
        downloadlink_changes.download = /*message*/
        ctx2[4].content.value.orig_name || "image";
      if (dirty & /*$$scope*/
      65536) {
        downloadlink_changes.$$scope = { dirty, ctx: ctx2 };
      }
      downloadlink.$set(downloadlink_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(downloadlink.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(downloadlink.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(downloadlink, detaching);
    }
  };
}
function create_default_slot_1$1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({ props: { Icon: Download } });
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_if_block_3$2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Retry,
      label: "Retry",
      disabled: (
        /*generating*/
        ctx[7]
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[14]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*generating*/
      128)
        iconbutton_changes.disabled = /*generating*/
        ctx2[7];
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_if_block_2$2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      label: "Undo",
      Icon: Undo,
      disabled: (
        /*generating*/
        ctx[7]
      )
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler_1*/
    ctx[15]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty & /*generating*/
      128)
        iconbutton_changes.disabled = /*generating*/
        ctx2[7];
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_if_block_1$3(ctx) {
  let likedislike;
  let current;
  likedislike = new LikeDislike({
    props: { handle_action: (
      /*handle_action*/
      ctx[8]
    ) }
  });
  return {
    c() {
      create_component(likedislike.$$.fragment);
    },
    l(nodes) {
      claim_component(likedislike.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(likedislike, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const likedislike_changes = {};
      if (dirty & /*handle_action*/
      256)
        likedislike_changes.handle_action = /*handle_action*/
        ctx2[8];
      likedislike.$set(likedislike_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(likedislike.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(likedislike.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(likedislike, detaching);
    }
  };
}
function create_default_slot$3(ctx) {
  let t0;
  let show_if = (
    /*show_download*/
    ctx[10] && !Array.isArray(
      /*message*/
      ctx[4]
    ) && is_component_message(
      /*message*/
      ctx[4]
    )
  );
  let t1;
  let t2;
  let t3;
  let if_block4_anchor;
  let current;
  let if_block0 = (
    /*show_copy*/
    ctx[11] && create_if_block_5$2(ctx)
  );
  let if_block1 = show_if && create_if_block_4$2(ctx);
  let if_block2 = (
    /*show_retry*/
    ctx[1] && create_if_block_3$2(ctx)
  );
  let if_block3 = (
    /*show_undo*/
    ctx[2] && create_if_block_2$2(ctx)
  );
  let if_block4 = (
    /*likeable*/
    ctx[0] && create_if_block_1$3(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      if (if_block1)
        if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      t2 = space();
      if (if_block3)
        if_block3.c();
      t3 = space();
      if (if_block4)
        if_block4.c();
      if_block4_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      t1 = claim_space(nodes);
      if (if_block2)
        if_block2.l(nodes);
      t2 = claim_space(nodes);
      if (if_block3)
        if_block3.l(nodes);
      t3 = claim_space(nodes);
      if (if_block4)
        if_block4.l(nodes);
      if_block4_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, t1, anchor);
      if (if_block2)
        if_block2.m(target, anchor);
      insert_hydration(target, t2, anchor);
      if (if_block3)
        if_block3.m(target, anchor);
      insert_hydration(target, t3, anchor);
      if (if_block4)
        if_block4.m(target, anchor);
      insert_hydration(target, if_block4_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_copy*/
        ctx2[11]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*show_copy*/
          2048) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_5$2(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t0.parentNode, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (dirty & /*show_download, message*/
      1040)
        show_if = /*show_download*/
        ctx2[10] && !Array.isArray(
          /*message*/
          ctx2[4]
        ) && is_component_message(
          /*message*/
          ctx2[4]
        );
      if (show_if) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*show_download, message*/
          1040) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_4$2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(t1.parentNode, t1);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (
        /*show_retry*/
        ctx2[1]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty & /*show_retry*/
          2) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block_3$2(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(t2.parentNode, t2);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
      if (
        /*show_undo*/
        ctx2[2]
      ) {
        if (if_block3) {
          if_block3.p(ctx2, dirty);
          if (dirty & /*show_undo*/
          4) {
            transition_in(if_block3, 1);
          }
        } else {
          if_block3 = create_if_block_2$2(ctx2);
          if_block3.c();
          transition_in(if_block3, 1);
          if_block3.m(t3.parentNode, t3);
        }
      } else if (if_block3) {
        group_outros();
        transition_out(if_block3, 1, 1, () => {
          if_block3 = null;
        });
        check_outros();
      }
      if (
        /*likeable*/
        ctx2[0]
      ) {
        if (if_block4) {
          if_block4.p(ctx2, dirty);
          if (dirty & /*likeable*/
          1) {
            transition_in(if_block4, 1);
          }
        } else {
          if_block4 = create_if_block_1$3(ctx2);
          if_block4.c();
          transition_in(if_block4, 1);
          if_block4.m(if_block4_anchor.parentNode, if_block4_anchor);
        }
      } else if (if_block4) {
        group_outros();
        transition_out(if_block4, 1, 1, () => {
          if_block4 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(if_block2);
      transition_in(if_block3);
      transition_in(if_block4);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      transition_out(if_block3);
      transition_out(if_block4);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(t2);
        detach(t3);
        detach(if_block4_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
      if (if_block2)
        if_block2.d(detaching);
      if (if_block3)
        if_block3.d(detaching);
      if (if_block4)
        if_block4.d(detaching);
    }
  };
}
function create_fragment$5(ctx) {
  let if_block_anchor;
  let current;
  let if_block = (
    /*show*/
    ctx[3] && create_if_block$3(ctx)
  );
  return {
    c() {
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      if (
        /*show*/
        ctx2[3]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*show*/
          8) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block$3(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function is_all_text(message2) {
  return Array.isArray(message2) && message2.every((m) => typeof m.content === "string") || !Array.isArray(message2) && typeof message2.content === "string";
}
function all_text(message2) {
  if (Array.isArray(message2)) {
    return message2.map((m) => m.content).join("\n");
  }
  return message2.content;
}
function instance$5($$self, $$props, $$invalidate) {
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
  const click_handler = () => handle_action("retry");
  const click_handler_1 = () => handle_action("undo");
  $$self.$$set = ($$props2) => {
    if ("likeable" in $$props2)
      $$invalidate(0, likeable = $$props2.likeable);
    if ("show_retry" in $$props2)
      $$invalidate(1, show_retry = $$props2.show_retry);
    if ("show_undo" in $$props2)
      $$invalidate(2, show_undo = $$props2.show_undo);
    if ("show_copy_button" in $$props2)
      $$invalidate(13, show_copy_button = $$props2.show_copy_button);
    if ("show" in $$props2)
      $$invalidate(3, show = $$props2.show);
    if ("message" in $$props2)
      $$invalidate(4, message = $$props2.message);
    if ("position" in $$props2)
      $$invalidate(5, position = $$props2.position);
    if ("avatar" in $$props2)
      $$invalidate(6, avatar = $$props2.avatar);
    if ("generating" in $$props2)
      $$invalidate(7, generating = $$props2.generating);
    if ("handle_action" in $$props2)
      $$invalidate(8, handle_action = $$props2.handle_action);
    if ("layout" in $$props2)
      $$invalidate(9, layout = $$props2.layout);
  };
  $$self.$$.update = () => {
    var _a;
    if ($$self.$$.dirty & /*message*/
    16) {
      $$invalidate(12, message_text = is_all_text(message) ? all_text(message) : "");
    }
    if ($$self.$$.dirty & /*show_copy_button, message*/
    8208) {
      $$invalidate(11, show_copy = show_copy_button && message && is_all_text(message));
    }
    if ($$self.$$.dirty & /*message*/
    16) {
      $$invalidate(10, show_download = !Array.isArray(message) && is_component_message(message) && ((_a = message.content.value) == null ? void 0 : _a.url));
    }
  };
  return [
    likeable,
    show_retry,
    show_undo,
    show,
    message,
    position,
    avatar,
    generating,
    handle_action,
    layout,
    show_download,
    show_copy,
    message_text,
    show_copy_button,
    click_handler,
    click_handler_1
  ];
}
class ButtonPanel extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$5, create_fragment$5, safe_not_equal, {
      likeable: 0,
      show_retry: 1,
      show_undo: 2,
      show_copy_button: 13,
      show: 3,
      message: 4,
      position: 5,
      avatar: 6,
      generating: 7,
      handle_action: 8,
      layout: 9
    });
  }
}
function get_each_context$1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[35] = list[i];
  child_ctx[37] = i;
  return child_ctx;
}
function create_if_block_6$1(ctx) {
  var _a;
  let div;
  let image;
  let current;
  image = new Image({
    props: {
      class: "avatar-image",
      src: (
        /*avatar_img*/
        (_a = ctx[1]) == null ? void 0 : _a.url
      ),
      alt: (
        /*role*/
        ctx[3] + " avatar"
      )
    }
  });
  return {
    c() {
      div = element("div");
      create_component(image.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(image.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "avatar-container svelte-5ng3n");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(image, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2;
      const image_changes = {};
      if (dirty[0] & /*avatar_img*/
      2)
        image_changes.src = /*avatar_img*/
        (_a2 = ctx2[1]) == null ? void 0 : _a2.url;
      if (dirty[0] & /*role*/
      8)
        image_changes.alt = /*role*/
        ctx2[3] + " avatar";
      image.$set(image_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(image);
    }
  };
}
function create_if_block_5$1(ctx) {
  var _a, _b;
  let a;
  let t_value = (
    /*message*/
    (((_a = ctx[35].content.value) == null ? void 0 : _a.orig_name) || /*message*/
    ((_b = ctx[35].content.value) == null ? void 0 : _b.path.split("/").pop()) || "file") + ""
  );
  let t;
  let a_href_value;
  let a_download_value;
  return {
    c() {
      a = element("a");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      a = claim_element(nodes, "A", {
        "data-testid": true,
        class: true,
        href: true,
        target: true,
        download: true
      });
      var a_nodes = children(a);
      t = claim_text(a_nodes, t_value);
      a_nodes.forEach(detach);
      this.h();
    },
    h() {
      var _a2, _b2;
      attr(a, "data-testid", "chatbot-file");
      attr(a, "class", "file-pil svelte-5ng3n");
      attr(a, "href", a_href_value = /*message*/
      ctx[35].content.value.url);
      attr(a, "target", "_blank");
      attr(a, "download", a_download_value = window.__is_colab__ ? null : (
        /*message*/
        ((_a2 = ctx[35].content.value) == null ? void 0 : _a2.orig_name) || /*message*/
        ((_b2 = ctx[35].content.value) == null ? void 0 : _b2.path.split("/").pop()) || "file"
      ));
    },
    m(target, anchor) {
      insert_hydration(target, a, anchor);
      append_hydration(a, t);
    },
    p(ctx2, dirty) {
      var _a2, _b2, _c, _d;
      if (dirty[0] & /*messages*/
      16 && t_value !== (t_value = /*message*/
      (((_a2 = ctx2[35].content.value) == null ? void 0 : _a2.orig_name) || /*message*/
      ((_b2 = ctx2[35].content.value) == null ? void 0 : _b2.path.split("/").pop()) || "file") + ""))
        set_data(t, t_value);
      if (dirty[0] & /*messages*/
      16 && a_href_value !== (a_href_value = /*message*/
      ctx2[35].content.value.url)) {
        attr(a, "href", a_href_value);
      }
      if (dirty[0] & /*messages*/
      16 && a_download_value !== (a_download_value = window.__is_colab__ ? null : (
        /*message*/
        ((_c = ctx2[35].content.value) == null ? void 0 : _c.orig_name) || /*message*/
        ((_d = ctx2[35].content.value) == null ? void 0 : _d.path.split("/").pop()) || "file"
      ))) {
        attr(a, "download", a_download_value);
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(a);
      }
    }
  };
}
function create_if_block_4$1(ctx) {
  let component;
  let current;
  component = new Component({
    props: {
      target: (
        /*target*/
        ctx[16]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[18]
      ),
      props: (
        /*message*/
        ctx[35].content.props
      ),
      type: (
        /*message*/
        ctx[35].content.component
      ),
      components: (
        /*_components*/
        ctx[19]
      ),
      value: (
        /*message*/
        ctx[35].content.value
      ),
      i18n: (
        /*i18n*/
        ctx[13]
      ),
      upload: (
        /*upload*/
        ctx[15]
      ),
      _fetch: (
        /*_fetch*/
        ctx[11]
      )
    }
  });
  component.$on(
    "load",
    /*load_handler*/
    ctx[32]
  );
  return {
    c() {
      create_component(component.$$.fragment);
    },
    l(nodes) {
      claim_component(component.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(component, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const component_changes = {};
      if (dirty[0] & /*target*/
      65536)
        component_changes.target = /*target*/
        ctx2[16];
      if (dirty[0] & /*theme_mode*/
      262144)
        component_changes.theme_mode = /*theme_mode*/
        ctx2[18];
      if (dirty[0] & /*messages*/
      16)
        component_changes.props = /*message*/
        ctx2[35].content.props;
      if (dirty[0] & /*messages*/
      16)
        component_changes.type = /*message*/
        ctx2[35].content.component;
      if (dirty[0] & /*_components*/
      524288)
        component_changes.components = /*_components*/
        ctx2[19];
      if (dirty[0] & /*messages*/
      16)
        component_changes.value = /*message*/
        ctx2[35].content.value;
      if (dirty[0] & /*i18n*/
      8192)
        component_changes.i18n = /*i18n*/
        ctx2[13];
      if (dirty[0] & /*upload*/
      32768)
        component_changes.upload = /*upload*/
        ctx2[15];
      if (dirty[0] & /*_fetch*/
      2048)
        component_changes._fetch = /*_fetch*/
        ctx2[11];
      component.$set(component_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(component.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(component.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(component, detaching);
    }
  };
}
function create_if_block_2$1(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block_3$1, create_else_block$1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*message*/
      ctx2[35].metadata.title
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block = if_blocks[current_block_type_index];
        if (!if_block) {
          if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block.c();
        } else {
          if_block.p(ctx2, dirty);
        }
        transition_in(if_block, 1);
        if_block.m(if_block_anchor.parentNode, if_block_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(if_block_anchor);
      }
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function create_else_block$1(ctx) {
  let markdown;
  let current;
  markdown = new MarkdownCode({
    props: {
      message: (
        /*message*/
        ctx[35].content
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[8]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[9]
      ),
      render_markdown: (
        /*render_markdown*/
        ctx[7]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[14]
      ),
      root: (
        /*root*/
        ctx[17]
      )
    }
  });
  markdown.$on("load", function() {
    if (is_function(
      /*scroll*/
      ctx[21]
    ))
      ctx[21].apply(this, arguments);
  });
  return {
    c() {
      create_component(markdown.$$.fragment);
    },
    l(nodes) {
      claim_component(markdown.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(markdown, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const markdown_changes = {};
      if (dirty[0] & /*messages*/
      16)
        markdown_changes.message = /*message*/
        ctx[35].content;
      if (dirty[0] & /*latex_delimiters*/
      256)
        markdown_changes.latex_delimiters = /*latex_delimiters*/
        ctx[8];
      if (dirty[0] & /*sanitize_html*/
      512)
        markdown_changes.sanitize_html = /*sanitize_html*/
        ctx[9];
      if (dirty[0] & /*render_markdown*/
      128)
        markdown_changes.render_markdown = /*render_markdown*/
        ctx[7];
      if (dirty[0] & /*line_breaks*/
      16384)
        markdown_changes.line_breaks = /*line_breaks*/
        ctx[14];
      if (dirty[0] & /*root*/
      131072)
        markdown_changes.root = /*root*/
        ctx[17];
      markdown.$set(markdown_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(markdown.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(markdown.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(markdown, detaching);
    }
  };
}
function create_if_block_3$1(ctx) {
  let messagebox;
  let current;
  messagebox = new MessageBox({
    props: {
      title: (
        /*message*/
        ctx[35].metadata.title
      ),
      expanded: is_last_bot_message(
        [
          /*message*/
          ctx[35]
        ],
        /*value*/
        ctx[0]
      ),
      $$slots: { default: [create_default_slot$2] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(messagebox.$$.fragment);
    },
    l(nodes) {
      claim_component(messagebox.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(messagebox, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const messagebox_changes = {};
      if (dirty[0] & /*messages*/
      16)
        messagebox_changes.title = /*message*/
        ctx2[35].metadata.title;
      if (dirty[0] & /*messages, value*/
      17)
        messagebox_changes.expanded = is_last_bot_message(
          [
            /*message*/
            ctx2[35]
          ],
          /*value*/
          ctx2[0]
        );
      if (dirty[0] & /*messages, latex_delimiters, sanitize_html, render_markdown, line_breaks, root, scroll*/
      2245520 | dirty[1] & /*$$scope*/
      128) {
        messagebox_changes.$$scope = { dirty, ctx: ctx2 };
      }
      messagebox.$set(messagebox_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(messagebox.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(messagebox.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(messagebox, detaching);
    }
  };
}
function create_default_slot$2(ctx) {
  let markdown;
  let current;
  markdown = new MarkdownCode({
    props: {
      message: (
        /*message*/
        ctx[35].content
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[8]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[9]
      ),
      render_markdown: (
        /*render_markdown*/
        ctx[7]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[14]
      ),
      root: (
        /*root*/
        ctx[17]
      )
    }
  });
  markdown.$on("load", function() {
    if (is_function(
      /*scroll*/
      ctx[21]
    ))
      ctx[21].apply(this, arguments);
  });
  return {
    c() {
      create_component(markdown.$$.fragment);
    },
    l(nodes) {
      claim_component(markdown.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(markdown, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const markdown_changes = {};
      if (dirty[0] & /*messages*/
      16)
        markdown_changes.message = /*message*/
        ctx[35].content;
      if (dirty[0] & /*latex_delimiters*/
      256)
        markdown_changes.latex_delimiters = /*latex_delimiters*/
        ctx[8];
      if (dirty[0] & /*sanitize_html*/
      512)
        markdown_changes.sanitize_html = /*sanitize_html*/
        ctx[9];
      if (dirty[0] & /*render_markdown*/
      128)
        markdown_changes.render_markdown = /*render_markdown*/
        ctx[7];
      if (dirty[0] & /*line_breaks*/
      16384)
        markdown_changes.line_breaks = /*line_breaks*/
        ctx[14];
      if (dirty[0] & /*root*/
      131072)
        markdown_changes.root = /*root*/
        ctx[17];
      markdown.$set(markdown_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(markdown.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(markdown.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(markdown, detaching);
    }
  };
}
function create_if_block_1$2(ctx) {
  let buttonpanel;
  let current;
  const buttonpanel_spread_levels = [
    /*button_panel_props*/
    ctx[22]
  ];
  let buttonpanel_props = {};
  for (let i = 0; i < buttonpanel_spread_levels.length; i += 1) {
    buttonpanel_props = assign(buttonpanel_props, buttonpanel_spread_levels[i]);
  }
  buttonpanel = new ButtonPanel({ props: buttonpanel_props });
  return {
    c() {
      create_component(buttonpanel.$$.fragment);
    },
    l(nodes) {
      claim_component(buttonpanel.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(buttonpanel, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const buttonpanel_changes = dirty[0] & /*button_panel_props*/
      4194304 ? get_spread_update(buttonpanel_spread_levels, [get_spread_object(
        /*button_panel_props*/
        ctx2[22]
      )]) : {};
      buttonpanel.$set(buttonpanel_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(buttonpanel.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(buttonpanel.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(buttonpanel, detaching);
    }
  };
}
function create_each_block$1(ctx) {
  let div;
  let button;
  let current_block_type_index;
  let if_block0;
  let button_dir_value;
  let button_aria_label_value;
  let div_class_value;
  let t;
  let if_block1_anchor;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_2$1, create_if_block_4$1, create_if_block_5$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*message*/
      ctx2[35].type === "text"
    )
      return 0;
    if (
      /*message*/
      ctx2[35].type === "component" && /*message*/
      ctx2[35].content.component in /*_components*/
      ctx2[19]
    )
      return 1;
    if (
      /*message*/
      ctx2[35].type === "component" && /*message*/
      ctx2[35].content.component === "file"
    )
      return 2;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
    if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  function click_handler() {
    return (
      /*click_handler*/
      ctx[33](
        /*message*/
        ctx[35]
      )
    );
  }
  function keydown_handler(...args) {
    return (
      /*keydown_handler*/
      ctx[34](
        /*message*/
        ctx[35],
        ...args
      )
    );
  }
  let if_block1 = (
    /*layout*/
    ctx[5] === "panel" && create_if_block_1$2(ctx)
  );
  return {
    c() {
      div = element("div");
      button = element("button");
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      button = claim_element(div_nodes, "BUTTON", {
        "data-testid": true,
        dir: true,
        "aria-label": true,
        class: true
      });
      var button_nodes = children(button);
      if (if_block0)
        if_block0.l(button_nodes);
      button_nodes.forEach(detach);
      div_nodes.forEach(detach);
      t = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
      this.h();
    },
    h() {
      var _a;
      attr(
        button,
        "data-testid",
        /*role*/
        ctx[3]
      );
      attr(button, "dir", button_dir_value = /*rtl*/
      ctx[12] ? "rtl" : "ltr");
      attr(button, "aria-label", button_aria_label_value = /*role*/
      ctx[3] + "'s message: " + get_message_label_data(
        /*message*/
        ctx[35]
      ));
      attr(button, "class", "svelte-5ng3n");
      toggle_class(
        button,
        "latest",
        /*i*/
        ctx[20] === /*value*/
        ctx[0].length - 1
      );
      toggle_class(button, "message-markdown-disabled", !/*render_markdown*/
      ctx[7]);
      toggle_class(
        button,
        "selectable",
        /*selectable*/
        ctx[10]
      );
      set_style(button, "user-select", `text`);
      set_style(
        button,
        "cursor",
        /*selectable*/
        ctx[10] ? "pointer" : "default"
      );
      set_style(
        button,
        "text-align",
        /*rtl*/
        ctx[12] ? "right" : "left"
      );
      attr(div, "class", div_class_value = "message " + /*role*/
      ctx[3] + " " + (is_component_message(
        /*message*/
        ctx[35]
      ) ? (
        /*message*/
        (_a = ctx[35]) == null ? void 0 : _a.content.component
      ) : "") + " svelte-5ng3n");
      toggle_class(
        div,
        "message-fit",
        /*layout*/
        ctx[5] === "bubble" && !/*bubble_full_width*/
        ctx[6]
      );
      toggle_class(div, "panel-full-width", true);
      toggle_class(div, "message-markdown-disabled", !/*render_markdown*/
      ctx[7]);
      toggle_class(
        div,
        "component",
        /*message*/
        ctx[35].type === "component"
      );
      toggle_class(div, "html", is_component_message(
        /*message*/
        ctx[35]
      ) && /*message*/
      ctx[35].content.component === "html");
      toggle_class(
        div,
        "thought",
        /*thought_index*/
        ctx[37] > 0
      );
      set_style(
        div,
        "text-align",
        /*rtl*/
        ctx[12] && /*role*/
        ctx[3] === "user" ? "left" : "right"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, button);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(button, null);
      }
      insert_hydration(target, t, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(button, "click", click_handler),
          listen(button, "keydown", keydown_handler)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      var _a;
      ctx = new_ctx;
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx, dirty);
        }
      } else {
        if (if_block0) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block0 = if_blocks[current_block_type_index];
          if (!if_block0) {
            if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
            if_block0.c();
          } else {
            if_block0.p(ctx, dirty);
          }
          transition_in(if_block0, 1);
          if_block0.m(button, null);
        } else {
          if_block0 = null;
        }
      }
      if (!current || dirty[0] & /*role*/
      8) {
        attr(
          button,
          "data-testid",
          /*role*/
          ctx[3]
        );
      }
      if (!current || dirty[0] & /*rtl*/
      4096 && button_dir_value !== (button_dir_value = /*rtl*/
      ctx[12] ? "rtl" : "ltr")) {
        attr(button, "dir", button_dir_value);
      }
      if (!current || dirty[0] & /*role, messages*/
      24 && button_aria_label_value !== (button_aria_label_value = /*role*/
      ctx[3] + "'s message: " + get_message_label_data(
        /*message*/
        ctx[35]
      ))) {
        attr(button, "aria-label", button_aria_label_value);
      }
      if (!current || dirty[0] & /*i, value*/
      1048577) {
        toggle_class(
          button,
          "latest",
          /*i*/
          ctx[20] === /*value*/
          ctx[0].length - 1
        );
      }
      if (!current || dirty[0] & /*render_markdown*/
      128) {
        toggle_class(button, "message-markdown-disabled", !/*render_markdown*/
        ctx[7]);
      }
      if (!current || dirty[0] & /*selectable*/
      1024) {
        toggle_class(
          button,
          "selectable",
          /*selectable*/
          ctx[10]
        );
      }
      if (dirty[0] & /*selectable*/
      1024) {
        set_style(
          button,
          "cursor",
          /*selectable*/
          ctx[10] ? "pointer" : "default"
        );
      }
      if (dirty[0] & /*rtl*/
      4096) {
        set_style(
          button,
          "text-align",
          /*rtl*/
          ctx[12] ? "right" : "left"
        );
      }
      if (!current || dirty[0] & /*role, messages*/
      24 && div_class_value !== (div_class_value = "message " + /*role*/
      ctx[3] + " " + (is_component_message(
        /*message*/
        ctx[35]
      ) ? (
        /*message*/
        (_a = ctx[35]) == null ? void 0 : _a.content.component
      ) : "") + " svelte-5ng3n")) {
        attr(div, "class", div_class_value);
      }
      if (!current || dirty[0] & /*role, messages, layout, bubble_full_width*/
      120) {
        toggle_class(
          div,
          "message-fit",
          /*layout*/
          ctx[5] === "bubble" && !/*bubble_full_width*/
          ctx[6]
        );
      }
      if (!current || dirty[0] & /*role, messages*/
      24) {
        toggle_class(div, "panel-full-width", true);
      }
      if (!current || dirty[0] & /*role, messages, render_markdown*/
      152) {
        toggle_class(div, "message-markdown-disabled", !/*render_markdown*/
        ctx[7]);
      }
      if (!current || dirty[0] & /*role, messages, messages*/
      24) {
        toggle_class(
          div,
          "component",
          /*message*/
          ctx[35].type === "component"
        );
      }
      if (!current || dirty[0] & /*role, messages, messages*/
      24) {
        toggle_class(div, "html", is_component_message(
          /*message*/
          ctx[35]
        ) && /*message*/
        ctx[35].content.component === "html");
      }
      if (!current || dirty[0] & /*role, messages*/
      24) {
        toggle_class(
          div,
          "thought",
          /*thought_index*/
          ctx[37] > 0
        );
      }
      if (dirty[0] & /*rtl, role*/
      4104) {
        set_style(
          div,
          "text-align",
          /*rtl*/
          ctx[12] && /*role*/
          ctx[3] === "user" ? "left" : "right"
        );
      }
      if (
        /*layout*/
        ctx[5] === "panel"
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
          if (dirty[0] & /*layout*/
          32) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_1$2(ctx);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t);
        detach(if_block1_anchor);
      }
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
      if (if_block1)
        if_block1.d(detaching);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block$2(ctx) {
  let buttonpanel;
  let current;
  const buttonpanel_spread_levels = [
    /*button_panel_props*/
    ctx[22]
  ];
  let buttonpanel_props = {};
  for (let i = 0; i < buttonpanel_spread_levels.length; i += 1) {
    buttonpanel_props = assign(buttonpanel_props, buttonpanel_spread_levels[i]);
  }
  buttonpanel = new ButtonPanel({ props: buttonpanel_props });
  return {
    c() {
      create_component(buttonpanel.$$.fragment);
    },
    l(nodes) {
      claim_component(buttonpanel.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(buttonpanel, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const buttonpanel_changes = dirty[0] & /*button_panel_props*/
      4194304 ? get_spread_update(buttonpanel_spread_levels, [get_spread_object(
        /*button_panel_props*/
        ctx2[22]
      )]) : {};
      buttonpanel.$set(buttonpanel_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(buttonpanel.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(buttonpanel.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(buttonpanel, detaching);
    }
  };
}
function create_fragment$4(ctx) {
  let div1;
  let t0;
  let div0;
  let div1_class_value;
  let t1;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*avatar_img*/
    ctx[1] !== null && create_if_block_6$1(ctx)
  );
  let each_value = ensure_array_like(
    /*messages*/
    ctx[4]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block$1(get_each_context$1(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  let if_block1 = (
    /*layout*/
    ctx[5] === "bubble" && create_if_block$2(ctx)
  );
  return {
    c() {
      div1 = element("div");
      if (if_block0)
        if_block0.c();
      t0 = space();
      div0 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t1 = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      if (if_block0)
        if_block0.l(div1_nodes);
      t0 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div0_nodes);
      }
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      t1 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
      this.h();
    },
    h() {
      attr(div0, "class", "flex-wrap svelte-5ng3n");
      toggle_class(
        div0,
        "role",
        /*role*/
        ctx[3]
      );
      toggle_class(
        div0,
        "component-wrap",
        /*messages*/
        ctx[4][0].type === "component"
      );
      attr(div1, "class", div1_class_value = "message-row " + /*layout*/
      ctx[5] + " " + /*role*/
      ctx[3] + "-row svelte-5ng3n");
      toggle_class(
        div1,
        "with_avatar",
        /*avatar_img*/
        ctx[1] !== null
      );
      toggle_class(
        div1,
        "with_opposite_avatar",
        /*opposite_avatar_img*/
        ctx[2] !== null
      );
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      if (if_block0)
        if_block0.m(div1, null);
      append_hydration(div1, t0);
      append_hydration(div1, div0);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div0, null);
        }
      }
      insert_hydration(target, t1, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*avatar_img*/
        ctx2[1] !== null
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*avatar_img*/
          2) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_6$1(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div1, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (dirty[0] & /*button_panel_props, layout, role, messages, bubble_full_width, render_markdown, rtl, i, value, selectable, handle_select, latex_delimiters, sanitize_html, line_breaks, root, scroll, target, theme_mode, _components, i18n, upload, _fetch*/
      16777209) {
        each_value = ensure_array_like(
          /*messages*/
          ctx2[4]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context$1(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block$1(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div0, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if (!current || dirty[0] & /*role*/
      8) {
        toggle_class(
          div0,
          "role",
          /*role*/
          ctx2[3]
        );
      }
      if (!current || dirty[0] & /*messages*/
      16) {
        toggle_class(
          div0,
          "component-wrap",
          /*messages*/
          ctx2[4][0].type === "component"
        );
      }
      if (!current || dirty[0] & /*layout, role*/
      40 && div1_class_value !== (div1_class_value = "message-row " + /*layout*/
      ctx2[5] + " " + /*role*/
      ctx2[3] + "-row svelte-5ng3n")) {
        attr(div1, "class", div1_class_value);
      }
      if (!current || dirty[0] & /*layout, role, avatar_img*/
      42) {
        toggle_class(
          div1,
          "with_avatar",
          /*avatar_img*/
          ctx2[1] !== null
        );
      }
      if (!current || dirty[0] & /*layout, role, opposite_avatar_img*/
      44) {
        toggle_class(
          div1,
          "with_opposite_avatar",
          /*opposite_avatar_img*/
          ctx2[2] !== null
        );
      }
      if (
        /*layout*/
        ctx2[5] === "bubble"
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*layout*/
          32) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block$2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
        detach(t1);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d();
      destroy_each(each_blocks, detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function get_message_label_data(message) {
  var _a, _b, _c, _d;
  if (message.type === "text") {
    return message.content;
  } else if (message.type === "component" && message.content.component === "file") {
    if (Array.isArray(message.content.value)) {
      return `file of extension type: ${(_a = message.content.value[0].orig_name) == null ? void 0 : _a.split(".").pop()}`;
    }
    return `file of extension type: ${(_c = (_b = message.content.value) == null ? void 0 : _b.orig_name) == null ? void 0 : _c.split(".").pop()}` + (((_d = message.content.value) == null ? void 0 : _d.orig_name) ?? "");
  }
  return `a component of type ${message.content.component ?? "unknown"}`;
}
function instance$4($$self, $$props, $$invalidate) {
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
  function handle_select(i2, message) {
    dispatch("select", {
      index: message.index,
      value: message.content
    });
  }
  let button_panel_props;
  const load_handler = () => scroll2();
  const click_handler = (message) => handle_select(i, message);
  const keydown_handler = (message, e) => {
    if (e.key === "Enter") {
      handle_select(i, message);
    }
  };
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("avatar_img" in $$props2)
      $$invalidate(1, avatar_img = $$props2.avatar_img);
    if ("opposite_avatar_img" in $$props2)
      $$invalidate(2, opposite_avatar_img = $$props2.opposite_avatar_img);
    if ("role" in $$props2)
      $$invalidate(3, role = $$props2.role);
    if ("messages" in $$props2)
      $$invalidate(4, messages = $$props2.messages);
    if ("layout" in $$props2)
      $$invalidate(5, layout = $$props2.layout);
    if ("bubble_full_width" in $$props2)
      $$invalidate(6, bubble_full_width = $$props2.bubble_full_width);
    if ("render_markdown" in $$props2)
      $$invalidate(7, render_markdown = $$props2.render_markdown);
    if ("latex_delimiters" in $$props2)
      $$invalidate(8, latex_delimiters = $$props2.latex_delimiters);
    if ("sanitize_html" in $$props2)
      $$invalidate(9, sanitize_html = $$props2.sanitize_html);
    if ("selectable" in $$props2)
      $$invalidate(10, selectable = $$props2.selectable);
    if ("_fetch" in $$props2)
      $$invalidate(11, _fetch = $$props2._fetch);
    if ("rtl" in $$props2)
      $$invalidate(12, rtl = $$props2.rtl);
    if ("dispatch" in $$props2)
      $$invalidate(24, dispatch = $$props2.dispatch);
    if ("i18n" in $$props2)
      $$invalidate(13, i18n = $$props2.i18n);
    if ("line_breaks" in $$props2)
      $$invalidate(14, line_breaks = $$props2.line_breaks);
    if ("upload" in $$props2)
      $$invalidate(15, upload = $$props2.upload);
    if ("target" in $$props2)
      $$invalidate(16, target = $$props2.target);
    if ("root" in $$props2)
      $$invalidate(17, root = $$props2.root);
    if ("theme_mode" in $$props2)
      $$invalidate(18, theme_mode = $$props2.theme_mode);
    if ("_components" in $$props2)
      $$invalidate(19, _components = $$props2._components);
    if ("i" in $$props2)
      $$invalidate(20, i = $$props2.i);
    if ("show_copy_button" in $$props2)
      $$invalidate(25, show_copy_button = $$props2.show_copy_button);
    if ("generating" in $$props2)
      $$invalidate(26, generating = $$props2.generating);
    if ("show_like" in $$props2)
      $$invalidate(27, show_like = $$props2.show_like);
    if ("show_retry" in $$props2)
      $$invalidate(28, show_retry = $$props2.show_retry);
    if ("show_undo" in $$props2)
      $$invalidate(29, show_undo = $$props2.show_undo);
    if ("msg_format" in $$props2)
      $$invalidate(30, msg_format = $$props2.msg_format);
    if ("handle_action" in $$props2)
      $$invalidate(31, handle_action = $$props2.handle_action);
    if ("scroll" in $$props2)
      $$invalidate(21, scroll2 = $$props2.scroll);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*show_like, show_retry, show_undo, show_copy_button, generating, msg_format, messages, role, avatar_img, layout*/
    2113929274 | $$self.$$.dirty[1] & /*handle_action*/
    1) {
      $$invalidate(22, button_panel_props = {
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
      });
    }
  };
  return [
    value,
    avatar_img,
    opposite_avatar_img,
    role,
    messages,
    layout,
    bubble_full_width,
    render_markdown,
    latex_delimiters,
    sanitize_html,
    selectable,
    _fetch,
    rtl,
    i18n,
    line_breaks,
    upload,
    target,
    root,
    theme_mode,
    _components,
    i,
    scroll2,
    button_panel_props,
    handle_select,
    dispatch,
    show_copy_button,
    generating,
    show_like,
    show_retry,
    show_undo,
    msg_format,
    handle_action,
    load_handler,
    click_handler,
    keydown_handler
  ];
}
class Message extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$4,
      create_fragment$4,
      safe_not_equal,
      {
        value: 0,
        avatar_img: 1,
        opposite_avatar_img: 2,
        role: 3,
        messages: 4,
        layout: 5,
        bubble_full_width: 6,
        render_markdown: 7,
        latex_delimiters: 8,
        sanitize_html: 9,
        selectable: 10,
        _fetch: 11,
        rtl: 12,
        dispatch: 24,
        i18n: 13,
        line_breaks: 14,
        upload: 15,
        target: 16,
        root: 17,
        theme_mode: 18,
        _components: 19,
        i: 20,
        show_copy_button: 25,
        generating: 26,
        show_like: 27,
        show_retry: 28,
        show_undo: 29,
        msg_format: 30,
        handle_action: 31,
        scroll: 21
      },
      null,
      [-1, -1]
    );
  }
}
function create_fragment$3(ctx) {
  let div3;
  let textContent = `<span class="sr-only">Loading content</span> <div class="dot-flashing svelte-1gpwetz"></div>
	 
	<div class="dot-flashing svelte-1gpwetz"></div>
	 
	<div class="dot-flashing svelte-1gpwetz"></div>`;
  return {
    c() {
      div3 = element("div");
      div3.innerHTML = textContent;
      this.h();
    },
    l(nodes) {
      div3 = claim_element(nodes, "DIV", {
        class: true,
        role: true,
        "aria-label": true,
        "aria-live": true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(div3) !== "svelte-exuub1")
        div3.innerHTML = textContent;
      this.h();
    },
    h() {
      attr(div3, "class", "message pending svelte-1gpwetz");
      attr(div3, "role", "status");
      attr(div3, "aria-label", "Loading response");
      attr(div3, "aria-live", "polite");
      set_style(
        div3,
        "border-radius",
        /*layout*/
        ctx[0] === "bubble" ? "var(--radius-xxl)" : "none"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div3, anchor);
    },
    p(ctx2, [dirty]) {
      if (dirty & /*layout*/
      1) {
        set_style(
          div3,
          "border-radius",
          /*layout*/
          ctx2[0] === "bubble" ? "var(--radius-xxl)" : "none"
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div3);
      }
    }
  };
}
function instance$3($$self, $$props, $$invalidate) {
  let { layout = "bubble" } = $$props;
  $$self.$$set = ($$props2) => {
    if ("layout" in $$props2)
      $$invalidate(0, layout = $$props2.layout);
  };
  return [layout];
}
class Pending extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, { layout: 0 });
  }
}
function create_fragment$2(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: (
        /*copied*/
        ctx[0] ? Check : Copy
      ),
      label: (
        /*copied*/
        ctx[0] ? "Copied conversation" : "Copy conversation"
      )
    }
  });
  iconbutton.$on(
    "click",
    /*handle_copy*/
    ctx[1]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const iconbutton_changes = {};
      if (dirty & /*copied*/
      1)
        iconbutton_changes.Icon = /*copied*/
        ctx2[0] ? Check : Copy;
      if (dirty & /*copied*/
      1)
        iconbutton_changes.label = /*copied*/
        ctx2[0] ? "Copied conversation" : "Copy conversation";
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function instance$2($$self, $$props, $$invalidate) {
  let copied = false;
  let { value } = $$props;
  let timer;
  function copy_feedback() {
    $$invalidate(0, copied = true);
    if (timer)
      clearTimeout(timer);
    timer = setTimeout(
      () => {
        $$invalidate(0, copied = false);
      },
      1e3
    );
  }
  const copy_conversation = () => {
    if (value) {
      const conversation_value = value.map((message) => {
        if (message.type === "text") {
          return `${message.role}: ${message.content}`;
        }
        return `${message.role}: ${message.content.value.url}`;
      }).join("\n\n");
      navigator.clipboard.writeText(conversation_value).catch((err) => {
        console.error("Failed to copy conversation: ", err);
      });
    }
  };
  async function handle_copy() {
    if ("clipboard" in navigator) {
      copy_conversation();
      copy_feedback();
    }
  }
  onDestroy(() => {
    if (timer)
      clearTimeout(timer);
  });
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
  };
  return [copied, handle_copy, value];
}
class CopyAll extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, { value: 2 });
  }
}
function get_each_context_1(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[60] = list[i];
  child_ctx[59] = i;
  return child_ctx;
}
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[54] = list[i];
  child_ctx[59] = i;
  const constants_0 = (
    /*messages*/
    child_ctx[54][0].role === "user" ? "user" : "bot"
  );
  child_ctx[55] = constants_0;
  const constants_1 = (
    /*avatar_images*/
    child_ctx[11][
      /*role*/
      child_ctx[55] === "user" ? 0 : 1
    ]
  );
  child_ctx[56] = constants_1;
  const constants_2 = (
    /*avatar_images*/
    child_ctx[11][
      /*role*/
      child_ctx[55] === "user" ? 0 : 1
    ]
  );
  child_ctx[57] = constants_2;
  return child_ctx;
}
function create_if_block_11(ctx) {
  let iconbuttonwrapper;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty[0] & /*value, show_copy_all_button, show_share_button*/
      385 | dirty[2] & /*$$scope*/
      1) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbuttonwrapper, detaching);
    }
  };
}
function create_if_block_13(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Community,
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  iconbutton.$on(
    "click",
    /*click_handler*/
    ctx[43]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbutton_changes = {};
      if (dirty[2] & /*$$scope*/
      1) {
        iconbutton_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbutton.$set(iconbutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let community;
  let current;
  community = new Community({});
  return {
    c() {
      create_component(community.$$.fragment);
    },
    l(nodes) {
      claim_component(community.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(community, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(community.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(community.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(community, detaching);
    }
  };
}
function create_if_block_12(ctx) {
  let copyall;
  let current;
  copyall = new CopyAll({ props: { value: (
    /*value*/
    ctx[0]
  ) } });
  return {
    c() {
      create_component(copyall.$$.fragment);
    },
    l(nodes) {
      claim_component(copyall.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(copyall, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const copyall_changes = {};
      if (dirty[0] & /*value*/
      1)
        copyall_changes.value = /*value*/
        ctx2[0];
      copyall.$set(copyall_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(copyall.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(copyall.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(copyall, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let t0;
  let iconbutton;
  let t1;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*show_share_button*/
    ctx[7] && create_if_block_13(ctx)
  );
  iconbutton = new IconButton({ props: { Icon: Trash, label: "Clear" } });
  iconbutton.$on(
    "click",
    /*click_handler_1*/
    ctx[44]
  );
  let if_block1 = (
    /*show_copy_all_button*/
    ctx[8] && create_if_block_12(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      create_component(iconbutton.$$.fragment);
      t1 = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      claim_component(iconbutton.$$.fragment, nodes);
      t1 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      mount_component(iconbutton, target, anchor);
      insert_hydration(target, t1, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_share_button*/
        ctx2[7]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*show_share_button*/
          128) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_13(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t0.parentNode, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*show_copy_all_button*/
        ctx2[8]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*show_copy_all_button*/
          256) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_12(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(iconbutton.$$.fragment, local);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(iconbutton.$$.fragment, local);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      destroy_component(iconbutton, detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_else_block(ctx) {
  let div_1;
  let t;
  let current;
  let if_block0 = (
    /*placeholder*/
    ctx[19] !== null && create_if_block_10(ctx)
  );
  let if_block1 = (
    /*examples*/
    ctx[22] !== null && create_if_block_4(ctx)
  );
  return {
    c() {
      div_1 = element("div");
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      if (if_block0)
        if_block0.l(div_1_nodes);
      t = claim_space(div_1_nodes);
      if (if_block1)
        if_block1.l(div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "placeholder-content svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      if (if_block0)
        if_block0.m(div_1, null);
      append_hydration(div_1, t);
      if (if_block1)
        if_block1.m(div_1, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*placeholder*/
        ctx2[19] !== null
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*placeholder*/
          524288) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_10(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div_1, t);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*examples*/
        ctx2[22] !== null
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*examples*/
          4194304) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_4(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div_1, null);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
    }
  };
}
function create_if_block_1$1(ctx) {
  let div_1;
  let t;
  let current;
  let mounted;
  let dispose;
  let each_value = ensure_array_like(
    /*groupedMessages*/
    ctx[34]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  let if_block = (
    /*pending_message*/
    ctx[3] && create_if_block_2(ctx)
  );
  return {
    c() {
      div_1 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      t = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_1_nodes);
      }
      t = claim_space(div_1_nodes);
      if (if_block)
        if_block.l(div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "message-wrap svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div_1, null);
        }
      }
      append_hydration(div_1, t);
      if (if_block)
        if_block.m(div_1, null);
      current = true;
      if (!mounted) {
        dispose = action_destroyer(copy.call(null, div_1));
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*avatar_images, layout, i18n, _fetch, line_breaks, theme_mode, target, root, upload, selectable, sanitize_html, bubble_full_width, render_markdown, rtl, value, latex_delimiters, _components, generating, msg_format, likeable, like_user_message, _retryable, _undoable, show_copy_button*/
      532151927 | dirty[1] & /*groupedMessages, dispatch, handle_like, is_browser, is_image_preview_open, image_preview_source, image_preview_source_alt*/
      319) {
        each_value = ensure_array_like(
          /*groupedMessages*/
          ctx2[34]
        );
        let i;
        for (i = 0; i < each_value.length; i += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div_1, t);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
      if (
        /*pending_message*/
        ctx2[3]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty[0] & /*pending_message*/
          8) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_2(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(div_1, null);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      transition_in(if_block);
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_each(each_blocks, detaching);
      if (if_block)
        if_block.d();
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_10(ctx) {
  let div_1;
  let markdown;
  let current;
  markdown = new MarkdownCode({
    props: {
      message: (
        /*placeholder*/
        ctx[19]
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[2]
      ),
      root: (
        /*root*/
        ctx[26]
      )
    }
  });
  return {
    c() {
      div_1 = element("div");
      create_component(markdown.$$.fragment);
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      claim_component(markdown.$$.fragment, div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "placeholder svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      mount_component(markdown, div_1, null);
      current = true;
    },
    p(ctx2, dirty) {
      const markdown_changes = {};
      if (dirty[0] & /*placeholder*/
      524288)
        markdown_changes.message = /*placeholder*/
        ctx2[19];
      if (dirty[0] & /*latex_delimiters*/
      4)
        markdown_changes.latex_delimiters = /*latex_delimiters*/
        ctx2[2];
      if (dirty[0] & /*root*/
      67108864)
        markdown_changes.root = /*root*/
        ctx2[26];
      markdown.$set(markdown_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(markdown.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(markdown.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_component(markdown);
    }
  };
}
function create_if_block_4(ctx) {
  let div_1;
  let current;
  let each_value_1 = ensure_array_like(
    /*examples*/
    ctx[22]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value_1.length; i += 1) {
    each_blocks[i] = create_each_block_1(get_each_context_1(ctx, each_value_1, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      div_1 = element("div");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(div_1_nodes);
      }
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "examples svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(div_1, null);
        }
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*examples*/
      4194304 | dirty[1] & /*handle_example_select*/
      128) {
        each_value_1 = ensure_array_like(
          /*examples*/
          ctx2[22]
        );
        let i;
        for (i = 0; i < each_value_1.length; i += 1) {
          const child_ctx = get_each_context_1(ctx2, each_value_1, i);
          if (each_blocks[i]) {
            each_blocks[i].p(child_ctx, dirty);
            transition_in(each_blocks[i], 1);
          } else {
            each_blocks[i] = create_each_block_1(child_ctx);
            each_blocks[i].c();
            transition_in(each_blocks[i], 1);
            each_blocks[i].m(div_1, null);
          }
        }
        group_outros();
        for (i = each_value_1.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value_1.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block_9(ctx) {
  let div_1;
  let image;
  let current;
  image = new Image({
    props: {
      class: "example-icon",
      src: (
        /*example*/
        ctx[60].icon.url
      ),
      alt: "example-icon"
    }
  });
  return {
    c() {
      div_1 = element("div");
      create_component(image.$$.fragment);
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      claim_component(image.$$.fragment, div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "example-icon-container svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      mount_component(image, div_1, null);
      current = true;
    },
    p(ctx2, dirty) {
      const image_changes = {};
      if (dirty[0] & /*examples*/
      4194304)
        image_changes.src = /*example*/
        ctx2[60].icon.url;
      image.$set(image_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_component(image);
    }
  };
}
function create_else_block_1(ctx) {
  let span;
  let t_value = (
    /*example*/
    ctx[60].text + ""
  );
  let t;
  return {
    c() {
      span = element("span");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t = claim_text(span_nodes, t_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "example-text svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*examples*/
      4194304 && t_value !== (t_value = /*example*/
      ctx2[60].text + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_8(ctx) {
  let span;
  let t_value = (
    /*example*/
    ctx[60].display_text + ""
  );
  let t;
  return {
    c() {
      span = element("span");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t = claim_text(span_nodes, t_value);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "example-display-text svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*examples*/
      4194304 && t_value !== (t_value = /*example*/
      ctx2[60].display_text + ""))
        set_data(t, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_7(ctx) {
  let span;
  let em;
  let t_value = (
    /*example*/
    ctx[60].files[0].orig_name + ""
  );
  let t;
  return {
    c() {
      span = element("span");
      em = element("em");
      t = text(t_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      em = claim_element(span_nodes, "EM", {});
      var em_nodes = children(em);
      t = claim_text(em_nodes, t_value);
      em_nodes.forEach(detach);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "example-file svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, em);
      append_hydration(em, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*examples*/
      4194304 && t_value !== (t_value = /*example*/
      ctx2[60].files[0].orig_name + ""))
        set_data(t, t_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_if_block_6(ctx) {
  let div_1;
  let image;
  let current;
  image = new Image({
    props: {
      class: "example-image",
      src: (
        /*example*/
        ctx[60].files[0].url
      ),
      alt: "example-image"
    }
  });
  return {
    c() {
      div_1 = element("div");
      create_component(image.$$.fragment);
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      claim_component(image.$$.fragment, div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "example-image-container svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      mount_component(image, div_1, null);
      current = true;
    },
    p(ctx2, dirty) {
      const image_changes = {};
      if (dirty[0] & /*examples*/
      4194304)
        image_changes.src = /*example*/
        ctx2[60].files[0].url;
      image.$set(image_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(image.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(image.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_component(image);
    }
  };
}
function create_if_block_5(ctx) {
  let span;
  let em;
  let t0_value = (
    /*example*/
    ctx[60].files.length + ""
  );
  let t0;
  let t1;
  return {
    c() {
      span = element("span");
      em = element("em");
      t0 = text(t0_value);
      t1 = text(" Files");
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      em = claim_element(span_nodes, "EM", {});
      var em_nodes = children(em);
      t0 = claim_text(em_nodes, t0_value);
      t1 = claim_text(em_nodes, " Files");
      em_nodes.forEach(detach);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "example-file svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, em);
      append_hydration(em, t0);
      append_hydration(em, t1);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*examples*/
      4194304 && t0_value !== (t0_value = /*example*/
      ctx2[60].files.length + ""))
        set_data(t0, t0_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_each_block_1(ctx) {
  let button;
  let t0;
  let t1;
  let show_if;
  let current_block_type_index;
  let if_block2;
  let t2;
  let current;
  let mounted;
  let dispose;
  let if_block0 = (
    /*example*/
    ctx[60].icon !== void 0 && create_if_block_9(ctx)
  );
  function select_block_type_1(ctx2, dirty) {
    if (
      /*example*/
      ctx2[60].display_text !== void 0
    )
      return create_if_block_8;
    return create_else_block_1;
  }
  let current_block_type = select_block_type_1(ctx);
  let if_block1 = current_block_type(ctx);
  const if_block_creators = [create_if_block_5, create_if_block_6, create_if_block_7];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    var _a;
    if (dirty[0] & /*examples*/
    4194304)
      show_if = null;
    if (
      /*example*/
      ctx2[60].files !== void 0 && /*example*/
      ctx2[60].files.length > 1
    )
      return 0;
    if (show_if == null)
      show_if = !!/*example*/
      (ctx2[60].files !== void 0 && /*example*/
      ctx2[60].files[0] !== void 0 && /*example*/
      ((_a = ctx2[60].files[0].mime_type) == null ? void 0 : _a.includes("image")));
    if (show_if)
      return 1;
    if (
      /*example*/
      ctx2[60].files !== void 0 && /*example*/
      ctx2[60].files[0] !== void 0
    )
      return 2;
    return -1;
  }
  if (~(current_block_type_index = select_block_type_2(ctx, [-1, -1, -1]))) {
    if_block2 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  function click_handler_3() {
    return (
      /*click_handler_3*/
      ctx[47](
        /*i*/
        ctx[59],
        /*example*/
        ctx[60]
      )
    );
  }
  return {
    c() {
      button = element("button");
      if (if_block0)
        if_block0.c();
      t0 = space();
      if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      t2 = space();
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      if (if_block0)
        if_block0.l(button_nodes);
      t0 = claim_space(button_nodes);
      if_block1.l(button_nodes);
      t1 = claim_space(button_nodes);
      if (if_block2)
        if_block2.l(button_nodes);
      t2 = claim_space(button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "example svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      if (if_block0)
        if_block0.m(button, null);
      append_hydration(button, t0);
      if_block1.m(button, null);
      append_hydration(button, t1);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(button, null);
      }
      append_hydration(button, t2);
      current = true;
      if (!mounted) {
        dispose = listen(button, "click", click_handler_3);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (
        /*example*/
        ctx[60].icon !== void 0
      ) {
        if (if_block0) {
          if_block0.p(ctx, dirty);
          if (dirty[0] & /*examples*/
          4194304) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_9(ctx);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(button, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (current_block_type === (current_block_type = select_block_type_1(ctx)) && if_block1) {
        if_block1.p(ctx, dirty);
      } else {
        if_block1.d(1);
        if_block1 = current_block_type(ctx);
        if (if_block1) {
          if_block1.c();
          if_block1.m(button, t1);
        }
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_2(ctx, dirty);
      if (current_block_type_index === previous_block_index) {
        if (~current_block_type_index) {
          if_blocks[current_block_type_index].p(ctx, dirty);
        }
      } else {
        if (if_block2) {
          group_outros();
          transition_out(if_blocks[previous_block_index], 1, 1, () => {
            if_blocks[previous_block_index] = null;
          });
          check_outros();
        }
        if (~current_block_type_index) {
          if_block2 = if_blocks[current_block_type_index];
          if (!if_block2) {
            if_block2 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
            if_block2.c();
          } else {
            if_block2.p(ctx, dirty);
          }
          transition_in(if_block2, 1);
          if_block2.m(button, t2);
        } else {
          if_block2 = null;
        }
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      if (if_block0)
        if_block0.d();
      if_block1.d();
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d();
      }
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_3(ctx) {
  let div_1;
  let img;
  let img_src_value;
  let t;
  let iconbuttonwrapper;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      div_1 = element("div");
      img = element("img");
      t = space();
      create_component(iconbuttonwrapper.$$.fragment);
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      img = claim_element(div_1_nodes, "IMG", { src: true, alt: true });
      t = claim_space(div_1_nodes);
      claim_component(iconbuttonwrapper.$$.fragment, div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      if (!src_url_equal(img.src, img_src_value = /*image_preview_source*/
      ctx[31]))
        attr(img, "src", img_src_value);
      attr(
        img,
        "alt",
        /*image_preview_source_alt*/
        ctx[32]
      );
      attr(div_1, "class", "image-preview svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      append_hydration(div_1, img);
      append_hydration(div_1, t);
      mount_component(iconbuttonwrapper, div_1, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (!current || dirty[1] & /*image_preview_source*/
      1 && !src_url_equal(img.src, img_src_value = /*image_preview_source*/
      ctx2[31])) {
        attr(img, "src", img_src_value);
      }
      if (!current || dirty[1] & /*image_preview_source_alt*/
      2) {
        attr(
          img,
          "alt",
          /*image_preview_source_alt*/
          ctx2[32]
        );
      }
      const iconbuttonwrapper_changes = {};
      if (dirty[1] & /*is_image_preview_open*/
      4 | dirty[2] & /*$$scope*/
      1) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_component(iconbuttonwrapper);
    }
  };
}
function create_default_slot$1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({ props: { Icon: Clear, label: "Clear" } });
  iconbutton.$on(
    "click",
    /*click_handler_2*/
    ctx[45]
  );
  return {
    c() {
      create_component(iconbutton.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbutton, target, anchor);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(iconbutton, detaching);
    }
  };
}
function create_each_block(ctx) {
  let t;
  let message;
  let current;
  let if_block = (
    /*is_image_preview_open*/
    ctx[33] && create_if_block_3(ctx)
  );
  function func2(...args) {
    return (
      /*func*/
      ctx[46](
        /*i*/
        ctx[59],
        /*messages*/
        ctx[54],
        ...args
      )
    );
  }
  message = new Message({
    props: {
      messages: (
        /*messages*/
        ctx[54]
      ),
      opposite_avatar_img: (
        /*opposite_avatar_img*/
        ctx[57]
      ),
      avatar_img: (
        /*avatar_img*/
        ctx[56]
      ),
      role: (
        /*role*/
        ctx[55]
      ),
      layout: (
        /*layout*/
        ctx[18]
      ),
      dispatch: (
        /*dispatch*/
        ctx[36]
      ),
      i18n: (
        /*i18n*/
        ctx[17]
      ),
      _fetch: (
        /*_fetch*/
        ctx[1]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[15]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[16]
      ),
      target: (
        /*target*/
        ctx[28]
      ),
      root: (
        /*root*/
        ctx[26]
      ),
      upload: (
        /*upload*/
        ctx[20]
      ),
      selectable: (
        /*selectable*/
        ctx[5]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[12]
      ),
      bubble_full_width: (
        /*bubble_full_width*/
        ctx[13]
      ),
      render_markdown: (
        /*render_markdown*/
        ctx[14]
      ),
      rtl: (
        /*rtl*/
        ctx[9]
      ),
      i: (
        /*i*/
        ctx[59]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[2]
      ),
      _components: (
        /*_components*/
        ctx[27]
      ),
      generating: (
        /*generating*/
        ctx[4]
      ),
      msg_format: (
        /*msg_format*/
        ctx[21]
      ),
      show_like: (
        /*role*/
        ctx[55] === "user" ? (
          /*likeable*/
          ctx[6] && /*like_user_message*/
          ctx[25]
        ) : (
          /*likeable*/
          ctx[6]
        )
      ),
      show_retry: (
        /*_retryable*/
        ctx[23] && is_last_bot_message(
          /*messages*/
          ctx[54],
          /*value*/
          ctx[0]
        )
      ),
      show_undo: (
        /*_undoable*/
        ctx[24] && is_last_bot_message(
          /*messages*/
          ctx[54],
          /*value*/
          ctx[0]
        )
      ),
      show_copy_button: (
        /*show_copy_button*/
        ctx[10]
      ),
      handle_action: func2,
      scroll: (
        /*is_browser*/
        ctx[35] ? scroll : func_1
      )
    }
  });
  return {
    c() {
      if (if_block)
        if_block.c();
      t = space();
      create_component(message.$$.fragment);
    },
    l(nodes) {
      if (if_block)
        if_block.l(nodes);
      t = claim_space(nodes);
      claim_component(message.$$.fragment, nodes);
    },
    m(target, anchor) {
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(message, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (
        /*is_image_preview_open*/
        ctx[33]
      ) {
        if (if_block) {
          if_block.p(ctx, dirty);
          if (dirty[1] & /*is_image_preview_open*/
          4) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block_3(ctx);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(t.parentNode, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const message_changes = {};
      if (dirty[1] & /*groupedMessages*/
      8)
        message_changes.messages = /*messages*/
        ctx[54];
      if (dirty[0] & /*avatar_images*/
      2048 | dirty[1] & /*groupedMessages*/
      8)
        message_changes.opposite_avatar_img = /*opposite_avatar_img*/
        ctx[57];
      if (dirty[0] & /*avatar_images*/
      2048 | dirty[1] & /*groupedMessages*/
      8)
        message_changes.avatar_img = /*avatar_img*/
        ctx[56];
      if (dirty[1] & /*groupedMessages*/
      8)
        message_changes.role = /*role*/
        ctx[55];
      if (dirty[0] & /*layout*/
      262144)
        message_changes.layout = /*layout*/
        ctx[18];
      if (dirty[0] & /*i18n*/
      131072)
        message_changes.i18n = /*i18n*/
        ctx[17];
      if (dirty[0] & /*_fetch*/
      2)
        message_changes._fetch = /*_fetch*/
        ctx[1];
      if (dirty[0] & /*line_breaks*/
      32768)
        message_changes.line_breaks = /*line_breaks*/
        ctx[15];
      if (dirty[0] & /*theme_mode*/
      65536)
        message_changes.theme_mode = /*theme_mode*/
        ctx[16];
      if (dirty[0] & /*target*/
      268435456)
        message_changes.target = /*target*/
        ctx[28];
      if (dirty[0] & /*root*/
      67108864)
        message_changes.root = /*root*/
        ctx[26];
      if (dirty[0] & /*upload*/
      1048576)
        message_changes.upload = /*upload*/
        ctx[20];
      if (dirty[0] & /*selectable*/
      32)
        message_changes.selectable = /*selectable*/
        ctx[5];
      if (dirty[0] & /*sanitize_html*/
      4096)
        message_changes.sanitize_html = /*sanitize_html*/
        ctx[12];
      if (dirty[0] & /*bubble_full_width*/
      8192)
        message_changes.bubble_full_width = /*bubble_full_width*/
        ctx[13];
      if (dirty[0] & /*render_markdown*/
      16384)
        message_changes.render_markdown = /*render_markdown*/
        ctx[14];
      if (dirty[0] & /*rtl*/
      512)
        message_changes.rtl = /*rtl*/
        ctx[9];
      if (dirty[0] & /*value*/
      1)
        message_changes.value = /*value*/
        ctx[0];
      if (dirty[0] & /*latex_delimiters*/
      4)
        message_changes.latex_delimiters = /*latex_delimiters*/
        ctx[2];
      if (dirty[0] & /*_components*/
      134217728)
        message_changes._components = /*_components*/
        ctx[27];
      if (dirty[0] & /*generating*/
      16)
        message_changes.generating = /*generating*/
        ctx[4];
      if (dirty[0] & /*msg_format*/
      2097152)
        message_changes.msg_format = /*msg_format*/
        ctx[21];
      if (dirty[0] & /*likeable, like_user_message*/
      33554496 | dirty[1] & /*groupedMessages*/
      8)
        message_changes.show_like = /*role*/
        ctx[55] === "user" ? (
          /*likeable*/
          ctx[6] && /*like_user_message*/
          ctx[25]
        ) : (
          /*likeable*/
          ctx[6]
        );
      if (dirty[0] & /*_retryable, value*/
      8388609 | dirty[1] & /*groupedMessages*/
      8)
        message_changes.show_retry = /*_retryable*/
        ctx[23] && is_last_bot_message(
          /*messages*/
          ctx[54],
          /*value*/
          ctx[0]
        );
      if (dirty[0] & /*_undoable, value*/
      16777217 | dirty[1] & /*groupedMessages*/
      8)
        message_changes.show_undo = /*_undoable*/
        ctx[24] && is_last_bot_message(
          /*messages*/
          ctx[54],
          /*value*/
          ctx[0]
        );
      if (dirty[0] & /*show_copy_button*/
      1024)
        message_changes.show_copy_button = /*show_copy_button*/
        ctx[10];
      if (dirty[1] & /*groupedMessages*/
      8)
        message_changes.handle_action = func2;
      message.$set(message_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(message.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(message.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      if (if_block)
        if_block.d(detaching);
      destroy_component(message, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let pending;
  let current;
  pending = new Pending({ props: { layout: (
    /*layout*/
    ctx[18]
  ) } });
  return {
    c() {
      create_component(pending.$$.fragment);
    },
    l(nodes) {
      claim_component(pending.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(pending, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const pending_changes = {};
      if (dirty[0] & /*layout*/
      262144)
        pending_changes.layout = /*layout*/
        ctx2[18];
      pending.$set(pending_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(pending.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(pending.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(pending, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let div_1;
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: ScrollDownArrow,
      label: "Scroll down",
      size: "large"
    }
  });
  iconbutton.$on(
    "click",
    /*scroll_to_bottom*/
    ctx[37]
  );
  return {
    c() {
      div_1 = element("div");
      create_component(iconbutton.$$.fragment);
      this.h();
    },
    l(nodes) {
      div_1 = claim_element(nodes, "DIV", { class: true });
      var div_1_nodes = children(div_1);
      claim_component(iconbutton.$$.fragment, div_1_nodes);
      div_1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div_1, "class", "scroll-down-button-container svelte-vxn3uw");
    },
    m(target, anchor) {
      insert_hydration(target, div_1, anchor);
      mount_component(iconbutton, div_1, null);
      current = true;
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(iconbutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div_1);
      }
      destroy_component(iconbutton);
    }
  };
}
function create_fragment$1(ctx) {
  let t0;
  let div_1;
  let current_block_type_index;
  let if_block1;
  let div_1_class_value;
  let t1;
  let if_block2_anchor;
  let current;
  let if_block0 = (
    /*value*/
    ctx[0] !== null && /*value*/
    ctx[0].length > 0 && create_if_block_11(ctx)
  );
  const if_block_creators = [create_if_block_1$1, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] !== null && /*value*/
      ctx2[0].length > 0 && /*groupedMessages*/
      ctx2[34] !== null
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block2 = (
    /*show_scroll_button*/
    ctx[30] && create_if_block$1(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      div_1 = element("div");
      if_block1.c();
      t1 = space();
      if (if_block2)
        if_block2.c();
      if_block2_anchor = empty();
      this.h();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      div_1 = claim_element(nodes, "DIV", {
        class: true,
        role: true,
        "aria-label": true,
        "aria-live": true
      });
      var div_1_nodes = children(div_1);
      if_block1.l(div_1_nodes);
      div_1_nodes.forEach(detach);
      t1 = claim_space(nodes);
      if (if_block2)
        if_block2.l(nodes);
      if_block2_anchor = empty();
      this.h();
    },
    h() {
      attr(div_1, "class", div_1_class_value = null_to_empty(
        /*layout*/
        ctx[18] === "bubble" ? "bubble-wrap" : "panel-wrap"
      ) + " svelte-vxn3uw");
      attr(div_1, "role", "log");
      attr(div_1, "aria-label", "chatbot conversation");
      attr(div_1, "aria-live", "polite");
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, div_1, anchor);
      if_blocks[current_block_type_index].m(div_1, null);
      ctx[48](div_1);
      insert_hydration(target, t1, anchor);
      if (if_block2)
        if_block2.m(target, anchor);
      insert_hydration(target, if_block2_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*value*/
        ctx2[0] !== null && /*value*/
        ctx2[0].length > 0
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*value*/
          1) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_11(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t0.parentNode, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(div_1, null);
      }
      if (!current || dirty[0] & /*layout*/
      262144 && div_1_class_value !== (div_1_class_value = null_to_empty(
        /*layout*/
        ctx2[18] === "bubble" ? "bubble-wrap" : "panel-wrap"
      ) + " svelte-vxn3uw")) {
        attr(div_1, "class", div_1_class_value);
      }
      if (
        /*show_scroll_button*/
        ctx2[30]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*show_scroll_button*/
          1073741824) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block$1(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(if_block2_anchor.parentNode, if_block2_anchor);
        }
      } else if (if_block2) {
        group_outros();
        transition_out(if_block2, 1, 1, () => {
          if_block2 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(div_1);
        detach(t1);
        detach(if_block2_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if_blocks[current_block_type_index].d();
      ctx[48](null);
      if (if_block2)
        if_block2.d(detaching);
    }
  };
}
const func_1 = () => {
};
function instance$1($$self, $$props, $$invalidate) {
  let groupedMessages;
  let { value = [] } = $$props;
  let old_value = null;
  let { _fetch } = $$props;
  let { load_component } = $$props;
  let _components = {};
  const is_browser = typeof window !== "undefined";
  async function update_components() {
    $$invalidate(27, _components = await load_components(get_components_from_messages(value), _components, load_component));
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
  onMount(() => {
    $$invalidate(28, target = document.querySelector("div.gradio-container"));
  });
  let div;
  let show_scroll_button = false;
  const dispatch = createEventDispatcher();
  function is_at_bottom() {
    return div && div.offsetHeight + div.scrollTop > div.scrollHeight - 100;
  }
  function scroll_to_bottom() {
    if (!div)
      return;
    div.scrollTo(0, div.scrollHeight);
    $$invalidate(30, show_scroll_button = false);
  }
  async function scroll_on_value_update() {
    if (!autoscroll)
      return;
    if (is_at_bottom()) {
      await tick();
      scroll_to_bottom();
    } else {
      $$invalidate(30, show_scroll_button = true);
    }
  }
  onMount(() => {
    scroll_on_value_update();
  });
  onMount(() => {
    function handle_scroll() {
      if (is_at_bottom()) {
        $$invalidate(30, show_scroll_button = false);
      }
    }
    div == null ? void 0 : div.addEventListener("scroll", handle_scroll);
    return () => {
      div == null ? void 0 : div.removeEventListener("scroll", handle_scroll);
    };
  });
  let image_preview_source;
  let image_preview_source_alt;
  let is_image_preview_open = false;
  afterUpdate(() => {
    if (!div)
      return;
    div.querySelectorAll("img").forEach((n) => {
      n.addEventListener("click", (e) => {
        const target2 = e.target;
        if (target2) {
          $$invalidate(31, image_preview_source = target2.src);
          $$invalidate(32, image_preview_source_alt = target2.alt);
          $$invalidate(33, is_image_preview_open = true);
        }
      });
    });
  });
  function handle_example_select(i, example) {
    dispatch("example_select", {
      index: i,
      value: { text: example.text, files: example.files }
    });
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
  const click_handler = async () => {
    try {
      const formatted = await format_chat_for_sharing(value);
      dispatch("share", { description: formatted });
    } catch (e) {
      console.error(e);
      let message = e instanceof ShareError ? e.message : "Share failed.";
      dispatch("error", message);
    }
  };
  const click_handler_1 = () => dispatch("clear");
  const click_handler_2 = () => $$invalidate(33, is_image_preview_open = false);
  const func2 = (i, messages, selected) => handle_like(i, messages[0], selected);
  const click_handler_3 = (i, example) => handle_example_select(i, example);
  function div_1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      div = $$value;
      $$invalidate(29, div);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("_fetch" in $$props2)
      $$invalidate(1, _fetch = $$props2._fetch);
    if ("load_component" in $$props2)
      $$invalidate(40, load_component = $$props2.load_component);
    if ("latex_delimiters" in $$props2)
      $$invalidate(2, latex_delimiters = $$props2.latex_delimiters);
    if ("pending_message" in $$props2)
      $$invalidate(3, pending_message = $$props2.pending_message);
    if ("generating" in $$props2)
      $$invalidate(4, generating = $$props2.generating);
    if ("selectable" in $$props2)
      $$invalidate(5, selectable = $$props2.selectable);
    if ("likeable" in $$props2)
      $$invalidate(6, likeable = $$props2.likeable);
    if ("show_share_button" in $$props2)
      $$invalidate(7, show_share_button = $$props2.show_share_button);
    if ("show_copy_all_button" in $$props2)
      $$invalidate(8, show_copy_all_button = $$props2.show_copy_all_button);
    if ("rtl" in $$props2)
      $$invalidate(9, rtl = $$props2.rtl);
    if ("show_copy_button" in $$props2)
      $$invalidate(10, show_copy_button = $$props2.show_copy_button);
    if ("avatar_images" in $$props2)
      $$invalidate(11, avatar_images = $$props2.avatar_images);
    if ("sanitize_html" in $$props2)
      $$invalidate(12, sanitize_html = $$props2.sanitize_html);
    if ("bubble_full_width" in $$props2)
      $$invalidate(13, bubble_full_width = $$props2.bubble_full_width);
    if ("render_markdown" in $$props2)
      $$invalidate(14, render_markdown = $$props2.render_markdown);
    if ("line_breaks" in $$props2)
      $$invalidate(15, line_breaks = $$props2.line_breaks);
    if ("autoscroll" in $$props2)
      $$invalidate(41, autoscroll = $$props2.autoscroll);
    if ("theme_mode" in $$props2)
      $$invalidate(16, theme_mode = $$props2.theme_mode);
    if ("i18n" in $$props2)
      $$invalidate(17, i18n = $$props2.i18n);
    if ("layout" in $$props2)
      $$invalidate(18, layout = $$props2.layout);
    if ("placeholder" in $$props2)
      $$invalidate(19, placeholder = $$props2.placeholder);
    if ("upload" in $$props2)
      $$invalidate(20, upload = $$props2.upload);
    if ("msg_format" in $$props2)
      $$invalidate(21, msg_format = $$props2.msg_format);
    if ("examples" in $$props2)
      $$invalidate(22, examples = $$props2.examples);
    if ("_retryable" in $$props2)
      $$invalidate(23, _retryable = $$props2._retryable);
    if ("_undoable" in $$props2)
      $$invalidate(24, _undoable = $$props2._undoable);
    if ("like_user_message" in $$props2)
      $$invalidate(25, like_user_message = $$props2.like_user_message);
    if ("root" in $$props2)
      $$invalidate(26, root = $$props2.root);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value*/
    1) {
      update_components();
    }
    if ($$self.$$.dirty[0] & /*value, pending_message, _components*/
    134217737) {
      if (value || pending_message || _components) {
        scroll_on_value_update();
      }
    }
    if ($$self.$$.dirty[0] & /*value*/
    1 | $$self.$$.dirty[1] & /*old_value*/
    2048) {
      {
        if (!dequal(value, old_value)) {
          $$invalidate(42, old_value = value);
          dispatch("change");
        }
      }
    }
    if ($$self.$$.dirty[0] & /*value, msg_format*/
    2097153) {
      $$invalidate(34, groupedMessages = value && group_messages(value, msg_format));
    }
  };
  return [
    value,
    _fetch,
    latex_delimiters,
    pending_message,
    generating,
    selectable,
    likeable,
    show_share_button,
    show_copy_all_button,
    rtl,
    show_copy_button,
    avatar_images,
    sanitize_html,
    bubble_full_width,
    render_markdown,
    line_breaks,
    theme_mode,
    i18n,
    layout,
    placeholder,
    upload,
    msg_format,
    examples,
    _retryable,
    _undoable,
    like_user_message,
    root,
    _components,
    target,
    div,
    show_scroll_button,
    image_preview_source,
    image_preview_source_alt,
    is_image_preview_open,
    groupedMessages,
    is_browser,
    dispatch,
    scroll_to_bottom,
    handle_example_select,
    handle_like,
    load_component,
    autoscroll,
    old_value,
    click_handler,
    click_handler_1,
    click_handler_2,
    func2,
    click_handler_3,
    div_1_binding
  ];
}
class ChatBot extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 0,
        _fetch: 1,
        load_component: 40,
        latex_delimiters: 2,
        pending_message: 3,
        generating: 4,
        selectable: 5,
        likeable: 6,
        show_share_button: 7,
        show_copy_all_button: 8,
        rtl: 9,
        show_copy_button: 10,
        avatar_images: 11,
        sanitize_html: 12,
        bubble_full_width: 13,
        render_markdown: 14,
        line_breaks: 15,
        autoscroll: 41,
        theme_mode: 16,
        i18n: 17,
        layout: 18,
        placeholder: 19,
        upload: 20,
        msg_format: 21,
        examples: 22,
        _retryable: 23,
        _undoable: 24,
        like_user_message: 25,
        root: 26
      },
      null,
      [-1, -1, -1]
    );
  }
}
const ChatBot$1 = ChatBot;
function create_if_block_1(ctx) {
  let statustracker;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[24].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[24].i18n
    ) },
    /*loading_status*/
    ctx[27],
    {
      show_progress: (
        /*loading_status*/
        ctx[27].show_progress === "hidden" ? "hidden" : "minimal"
      )
    }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[36]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      150994944 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        16777216 && {
          autoscroll: (
            /*gradio*/
            ctx2[24].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        16777216 && { i18n: (
          /*gradio*/
          ctx2[24].i18n
        ) },
        dirty[0] & /*loading_status*/
        134217728 && get_spread_object(
          /*loading_status*/
          ctx2[27]
        ),
        dirty[0] & /*loading_status*/
        134217728 && {
          show_progress: (
            /*loading_status*/
            ctx2[27].show_progress === "hidden" ? "hidden" : "minimal"
          )
        }
      ]) : {};
      statustracker.$set(statustracker_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(statustracker, detaching);
    }
  };
}
function create_if_block(ctx) {
  let blocklabel;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[7]
      ),
      Icon: Chat,
      float: true,
      label: (
        /*label*/
        ctx[6] || "Chatbot"
      )
    }
  });
  return {
    c() {
      create_component(blocklabel.$$.fragment);
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      128)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[7];
      if (dirty[0] & /*label*/
      64)
        blocklabel_changes.label = /*label*/
        ctx2[6] || "Chatbot";
      blocklabel.$set(blocklabel_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(blocklabel, detaching);
    }
  };
}
function create_default_slot(ctx) {
  var _a, _b;
  let t0;
  let div;
  let t1;
  let chatbot;
  let current;
  let if_block0 = (
    /*loading_status*/
    ctx[27] && create_if_block_1(ctx)
  );
  let if_block1 = (
    /*show_label*/
    ctx[7] && create_if_block(ctx)
  );
  chatbot = new ChatBot$1({
    props: {
      i18n: (
        /*gradio*/
        ctx[24].i18n
      ),
      selectable: (
        /*_selectable*/
        ctx[8]
      ),
      likeable: (
        /*likeable*/
        ctx[9]
      ),
      show_share_button: (
        /*show_share_button*/
        ctx[10]
      ),
      show_copy_all_button: (
        /*show_copy_all_button*/
        ctx[13]
      ),
      value: (
        /*_value*/
        ctx[34]
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[23]
      ),
      render_markdown: (
        /*render_markdown*/
        ctx[18]
      ),
      theme_mode: (
        /*theme_mode*/
        ctx[33]
      ),
      pending_message: (
        /*loading_status*/
        ((_a = ctx[27]) == null ? void 0 : _a.status) === "pending"
      ),
      generating: (
        /*loading_status*/
        ((_b = ctx[27]) == null ? void 0 : _b.status) === "generating"
      ),
      rtl: (
        /*rtl*/
        ctx[11]
      ),
      show_copy_button: (
        /*show_copy_button*/
        ctx[12]
      ),
      like_user_message: (
        /*like_user_message*/
        ctx[26]
      ),
      avatar_images: (
        /*avatar_images*/
        ctx[25]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[14]
      ),
      bubble_full_width: (
        /*bubble_full_width*/
        ctx[15]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[19]
      ),
      autoscroll: (
        /*autoscroll*/
        ctx[20]
      ),
      layout: (
        /*layout*/
        ctx[16]
      ),
      placeholder: (
        /*placeholder*/
        ctx[31]
      ),
      examples: (
        /*examples*/
        ctx[32]
      ),
      _retryable: (
        /*_retryable*/
        ctx[21]
      ),
      _undoable: (
        /*_undoable*/
        ctx[22]
      ),
      upload: (
        /*func*/
        ctx[37]
      ),
      _fetch: (
        /*func_1*/
        ctx[38]
      ),
      load_component: (
        /*gradio*/
        ctx[24].load_component
      ),
      msg_format: (
        /*type*/
        ctx[17]
      ),
      root: (
        /*gradio*/
        ctx[24].root
      )
    }
  });
  chatbot.$on(
    "change",
    /*change_handler*/
    ctx[39]
  );
  chatbot.$on(
    "select",
    /*select_handler*/
    ctx[40]
  );
  chatbot.$on(
    "like",
    /*like_handler*/
    ctx[41]
  );
  chatbot.$on(
    "share",
    /*share_handler*/
    ctx[42]
  );
  chatbot.$on(
    "error",
    /*error_handler*/
    ctx[43]
  );
  chatbot.$on(
    "example_select",
    /*example_select_handler*/
    ctx[44]
  );
  chatbot.$on(
    "retry",
    /*retry_handler*/
    ctx[45]
  );
  chatbot.$on(
    "undo",
    /*undo_handler*/
    ctx[46]
  );
  chatbot.$on(
    "clear",
    /*clear_handler*/
    ctx[47]
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      div = element("div");
      if (if_block1)
        if_block1.c();
      t1 = space();
      create_component(chatbot.$$.fragment);
      this.h();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      if (if_block1)
        if_block1.l(div_nodes);
      t1 = claim_space(div_nodes);
      claim_component(chatbot.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "wrapper svelte-g3p8na");
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, div, anchor);
      if (if_block1)
        if_block1.m(div, null);
      append_hydration(div, t1);
      mount_component(chatbot, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      var _a2, _b2;
      if (
        /*loading_status*/
        ctx2[27]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*loading_status*/
          134217728) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_1(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t0.parentNode, t0);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*show_label*/
        ctx2[7]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*show_label*/
          128) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div, t1);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      const chatbot_changes = {};
      if (dirty[0] & /*gradio*/
      16777216)
        chatbot_changes.i18n = /*gradio*/
        ctx2[24].i18n;
      if (dirty[0] & /*_selectable*/
      256)
        chatbot_changes.selectable = /*_selectable*/
        ctx2[8];
      if (dirty[0] & /*likeable*/
      512)
        chatbot_changes.likeable = /*likeable*/
        ctx2[9];
      if (dirty[0] & /*show_share_button*/
      1024)
        chatbot_changes.show_share_button = /*show_share_button*/
        ctx2[10];
      if (dirty[0] & /*show_copy_all_button*/
      8192)
        chatbot_changes.show_copy_all_button = /*show_copy_all_button*/
        ctx2[13];
      if (dirty[1] & /*_value*/
      8)
        chatbot_changes.value = /*_value*/
        ctx2[34];
      if (dirty[0] & /*latex_delimiters*/
      8388608)
        chatbot_changes.latex_delimiters = /*latex_delimiters*/
        ctx2[23];
      if (dirty[0] & /*render_markdown*/
      262144)
        chatbot_changes.render_markdown = /*render_markdown*/
        ctx2[18];
      if (dirty[1] & /*theme_mode*/
      4)
        chatbot_changes.theme_mode = /*theme_mode*/
        ctx2[33];
      if (dirty[0] & /*loading_status*/
      134217728)
        chatbot_changes.pending_message = /*loading_status*/
        ((_a2 = ctx2[27]) == null ? void 0 : _a2.status) === "pending";
      if (dirty[0] & /*loading_status*/
      134217728)
        chatbot_changes.generating = /*loading_status*/
        ((_b2 = ctx2[27]) == null ? void 0 : _b2.status) === "generating";
      if (dirty[0] & /*rtl*/
      2048)
        chatbot_changes.rtl = /*rtl*/
        ctx2[11];
      if (dirty[0] & /*show_copy_button*/
      4096)
        chatbot_changes.show_copy_button = /*show_copy_button*/
        ctx2[12];
      if (dirty[0] & /*like_user_message*/
      67108864)
        chatbot_changes.like_user_message = /*like_user_message*/
        ctx2[26];
      if (dirty[0] & /*avatar_images*/
      33554432)
        chatbot_changes.avatar_images = /*avatar_images*/
        ctx2[25];
      if (dirty[0] & /*sanitize_html*/
      16384)
        chatbot_changes.sanitize_html = /*sanitize_html*/
        ctx2[14];
      if (dirty[0] & /*bubble_full_width*/
      32768)
        chatbot_changes.bubble_full_width = /*bubble_full_width*/
        ctx2[15];
      if (dirty[0] & /*line_breaks*/
      524288)
        chatbot_changes.line_breaks = /*line_breaks*/
        ctx2[19];
      if (dirty[0] & /*autoscroll*/
      1048576)
        chatbot_changes.autoscroll = /*autoscroll*/
        ctx2[20];
      if (dirty[0] & /*layout*/
      65536)
        chatbot_changes.layout = /*layout*/
        ctx2[16];
      if (dirty[1] & /*placeholder*/
      1)
        chatbot_changes.placeholder = /*placeholder*/
        ctx2[31];
      if (dirty[1] & /*examples*/
      2)
        chatbot_changes.examples = /*examples*/
        ctx2[32];
      if (dirty[0] & /*_retryable*/
      2097152)
        chatbot_changes._retryable = /*_retryable*/
        ctx2[21];
      if (dirty[0] & /*_undoable*/
      4194304)
        chatbot_changes._undoable = /*_undoable*/
        ctx2[22];
      if (dirty[0] & /*gradio*/
      16777216)
        chatbot_changes.upload = /*func*/
        ctx2[37];
      if (dirty[0] & /*gradio*/
      16777216)
        chatbot_changes._fetch = /*func_1*/
        ctx2[38];
      if (dirty[0] & /*gradio*/
      16777216)
        chatbot_changes.load_component = /*gradio*/
        ctx2[24].load_component;
      if (dirty[0] & /*type*/
      131072)
        chatbot_changes.msg_format = /*type*/
        ctx2[17];
      if (dirty[0] & /*gradio*/
      16777216)
        chatbot_changes.root = /*gradio*/
        ctx2[24].root;
      chatbot.$set(chatbot_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      transition_in(chatbot.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      transition_out(chatbot.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(div);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d();
      destroy_component(chatbot);
    }
  };
}
function create_fragment(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      elem_id: (
        /*elem_id*/
        ctx[1]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[2]
      ),
      visible: (
        /*visible*/
        ctx[3]
      ),
      padding: false,
      scale: (
        /*scale*/
        ctx[4]
      ),
      min_width: (
        /*min_width*/
        ctx[5]
      ),
      height: (
        /*height*/
        ctx[28]
      ),
      min_height: (
        /*min_height*/
        ctx[29]
      ),
      max_height: (
        /*max_height*/
        ctx[30]
      ),
      allow_overflow: true,
      flex: true,
      overflow_behavior: "auto",
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(block.$$.fragment);
    },
    l(nodes) {
      claim_component(block.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(block, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const block_changes = {};
      if (dirty[0] & /*elem_id*/
      2)
        block_changes.elem_id = /*elem_id*/
        ctx2[1];
      if (dirty[0] & /*elem_classes*/
      4)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[2];
      if (dirty[0] & /*visible*/
      8)
        block_changes.visible = /*visible*/
        ctx2[3];
      if (dirty[0] & /*scale*/
      16)
        block_changes.scale = /*scale*/
        ctx2[4];
      if (dirty[0] & /*min_width*/
      32)
        block_changes.min_width = /*min_width*/
        ctx2[5];
      if (dirty[0] & /*height*/
      268435456)
        block_changes.height = /*height*/
        ctx2[28];
      if (dirty[0] & /*min_height*/
      536870912)
        block_changes.min_height = /*min_height*/
        ctx2[29];
      if (dirty[0] & /*max_height*/
      1073741824)
        block_changes.max_height = /*max_height*/
        ctx2[30];
      if (dirty[0] & /*gradio, _selectable, likeable, show_share_button, show_copy_all_button, latex_delimiters, render_markdown, loading_status, rtl, show_copy_button, like_user_message, avatar_images, sanitize_html, bubble_full_width, line_breaks, autoscroll, layout, _retryable, _undoable, type, value, show_label, label*/
      268435393 | dirty[1] & /*$$scope, _value, theme_mode, placeholder, examples*/
      131087) {
        block_changes.$$scope = { dirty, ctx: ctx2 };
      }
      block.$set(block_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(block.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(block.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(block, detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
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
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const func2 = (...args) => gradio.client.upload(...args);
  const func_12 = (...args) => gradio.client.fetch(...args);
  const change_handler = () => gradio.dispatch("change", value);
  const select_handler = (e) => gradio.dispatch("select", e.detail);
  const like_handler = (e) => gradio.dispatch("like", e.detail);
  const share_handler = (e) => gradio.dispatch("share", e.detail);
  const error_handler = (e) => gradio.dispatch("error", e.detail);
  const example_select_handler = (e) => gradio.dispatch("example_select", e.detail);
  const retry_handler = (e) => gradio.dispatch("retry", e.detail);
  const undo_handler = (e) => gradio.dispatch("undo", e.detail);
  const clear_handler = () => {
    $$invalidate(0, value = []);
    gradio.dispatch("clear");
  };
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("scale" in $$props2)
      $$invalidate(4, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(5, min_width = $$props2.min_width);
    if ("label" in $$props2)
      $$invalidate(6, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(7, show_label = $$props2.show_label);
    if ("root" in $$props2)
      $$invalidate(35, root = $$props2.root);
    if ("_selectable" in $$props2)
      $$invalidate(8, _selectable = $$props2._selectable);
    if ("likeable" in $$props2)
      $$invalidate(9, likeable = $$props2.likeable);
    if ("show_share_button" in $$props2)
      $$invalidate(10, show_share_button = $$props2.show_share_button);
    if ("rtl" in $$props2)
      $$invalidate(11, rtl = $$props2.rtl);
    if ("show_copy_button" in $$props2)
      $$invalidate(12, show_copy_button = $$props2.show_copy_button);
    if ("show_copy_all_button" in $$props2)
      $$invalidate(13, show_copy_all_button = $$props2.show_copy_all_button);
    if ("sanitize_html" in $$props2)
      $$invalidate(14, sanitize_html = $$props2.sanitize_html);
    if ("bubble_full_width" in $$props2)
      $$invalidate(15, bubble_full_width = $$props2.bubble_full_width);
    if ("layout" in $$props2)
      $$invalidate(16, layout = $$props2.layout);
    if ("type" in $$props2)
      $$invalidate(17, type = $$props2.type);
    if ("render_markdown" in $$props2)
      $$invalidate(18, render_markdown = $$props2.render_markdown);
    if ("line_breaks" in $$props2)
      $$invalidate(19, line_breaks = $$props2.line_breaks);
    if ("autoscroll" in $$props2)
      $$invalidate(20, autoscroll = $$props2.autoscroll);
    if ("_retryable" in $$props2)
      $$invalidate(21, _retryable = $$props2._retryable);
    if ("_undoable" in $$props2)
      $$invalidate(22, _undoable = $$props2._undoable);
    if ("latex_delimiters" in $$props2)
      $$invalidate(23, latex_delimiters = $$props2.latex_delimiters);
    if ("gradio" in $$props2)
      $$invalidate(24, gradio = $$props2.gradio);
    if ("avatar_images" in $$props2)
      $$invalidate(25, avatar_images = $$props2.avatar_images);
    if ("like_user_message" in $$props2)
      $$invalidate(26, like_user_message = $$props2.like_user_message);
    if ("loading_status" in $$props2)
      $$invalidate(27, loading_status = $$props2.loading_status);
    if ("height" in $$props2)
      $$invalidate(28, height = $$props2.height);
    if ("min_height" in $$props2)
      $$invalidate(29, min_height = $$props2.min_height);
    if ("max_height" in $$props2)
      $$invalidate(30, max_height = $$props2.max_height);
    if ("placeholder" in $$props2)
      $$invalidate(31, placeholder = $$props2.placeholder);
    if ("examples" in $$props2)
      $$invalidate(32, examples = $$props2.examples);
    if ("theme_mode" in $$props2)
      $$invalidate(33, theme_mode = $$props2.theme_mode);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*type, value*/
    131073 | $$self.$$.dirty[1] & /*root*/
    16) {
      $$invalidate(34, _value = type === "tuples" ? normalise_tuples(value, root) : normalise_messages(value, root));
    }
  };
  return [
    value,
    elem_id,
    elem_classes,
    visible,
    scale,
    min_width,
    label,
    show_label,
    _selectable,
    likeable,
    show_share_button,
    rtl,
    show_copy_button,
    show_copy_all_button,
    sanitize_html,
    bubble_full_width,
    layout,
    type,
    render_markdown,
    line_breaks,
    autoscroll,
    _retryable,
    _undoable,
    latex_delimiters,
    gradio,
    avatar_images,
    like_user_message,
    loading_status,
    height,
    min_height,
    max_height,
    placeholder,
    examples,
    theme_mode,
    _value,
    root,
    clear_status_handler,
    func2,
    func_12,
    change_handler,
    select_handler,
    like_handler,
    share_handler,
    error_handler,
    example_select_handler,
    retry_handler,
    undo_handler,
    clear_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance,
      create_fragment,
      safe_not_equal,
      {
        elem_id: 1,
        elem_classes: 2,
        visible: 3,
        value: 0,
        scale: 4,
        min_width: 5,
        label: 6,
        show_label: 7,
        root: 35,
        _selectable: 8,
        likeable: 9,
        show_share_button: 10,
        rtl: 11,
        show_copy_button: 12,
        show_copy_all_button: 13,
        sanitize_html: 14,
        bubble_full_width: 15,
        layout: 16,
        type: 17,
        render_markdown: 18,
        line_breaks: 19,
        autoscroll: 20,
        _retryable: 21,
        _undoable: 22,
        latex_delimiters: 23,
        gradio: 24,
        avatar_images: 25,
        like_user_message: 26,
        loading_status: 27,
        height: 28,
        min_height: 29,
        max_height: 30,
        placeholder: 31,
        examples: 32,
        theme_mode: 33
      },
      null,
      [-1, -1]
    );
  }
}
export {
  ChatBot$1 as BaseChatBot,
  Index as default
};
