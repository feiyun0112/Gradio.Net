import { c as create_ssr_component, s as subscribe, a as createEventDispatcher, o as onDestroy, b as add_attribute, v as validate_component, m as missing_component } from './ssr-RaXq3SJh.js';
import { $ as $format, s as setupi18n, d as browser, E as Embed } from './2-B6LMYTAg.js';
import { w as writable } from './index-hSrgoQUm.js';
import { p as page } from './stores-Bt2FUaXF.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './exports-DuWZopOC.js';

var inject_fonts = () => {
  const source_sans_pro = document.createElement("link");
  source_sans_pro.href = "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:ital,wght@0,200;0,300;0,400;0,600;0,700;0,900;1,200;1,300;1,400;1,600;1,700;1,900&display=swap";
  source_sans_pro.rel = "stylesheet";
  const ibm_mono = document.createElement("link");
  ibm_mono.href = "https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600;700&display=swap";
  ibm_mono.rel = "stylesheet";
  document.head.appendChild(source_sans_pro);
  document.head.appendChild(ibm_mono);
};
var Box = () => {
  const box = document.createElement("div");
  box.style.backgroundImage = "linear-gradient(to top, #f9fafb, white)";
  box.style.border = "1px solid #e5e7eb";
  box.style.borderRadius = "0.75rem";
  box.style.boxShadow = "0 0 10px rgba(0, 0, 0, 0.1)";
  box.style.color = "#374151";
  box.style.display = "flex";
  box.style.flexDirection = "row";
  box.style.alignItems = "center";
  box.style.height = "40px";
  box.style.justifyContent = "space-between";
  box.style.overflow = "hidden";
  box.style.position = "fixed";
  box.style.right = ".75rem";
  box.style.top = ".75rem";
  box.style.width = "auto";
  box.style.zIndex = "20";
  box.style.paddingLeft = "1rem";
  box.setAttribute("id", "huggingface-space-header");
  window.matchMedia("(max-width: 768px)").addEventListener("change", (e) => {
    if (e.matches) {
      box.style.display = "none";
    } else {
      box.style.display = "flex";
    }
  });
  return box;
};
var ArrowCollapse = () => {
  const arrow = document.createElementNS("http://www.w3.org/2000/svg", "svg");
  arrow.setAttribute("xmlns", "http://www.w3.org/2000/svg");
  arrow.setAttribute("xmlns:link", "http://www.w3.org/1999/xlink");
  arrow.setAttribute("aria-hidden", "true");
  arrow.setAttribute("focusable", "false");
  arrow.setAttribute("role", "img");
  arrow.setAttribute("width", "1em");
  arrow.setAttribute("height", "1em");
  arrow.setAttribute("preserveAspectRatio", "xMidYMid meet");
  arrow.setAttribute("viewBox", "0 0 12 12");
  arrow.setAttribute("fill", "currentColor");
  const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
  path.setAttribute(
    "d",
    "M0.375001 10.3828L0.375 1.61719C0.375 1.104 0.816001 0.687501 1.35938 0.687501L10.6406 0.6875C10.9017 0.6875 11.1521 0.785449 11.3367 0.959797C11.5213 1.13415 11.625 1.37062 11.625 1.61719V10.3828C11.625 10.6294 11.5213 10.8659 11.3367 11.0402C11.1521 11.2145 10.9017 11.3125 10.6406 11.3125H1.35938C0.816001 11.3125 0.375001 10.896 0.375001 10.3828ZM1.35938 10.5156H10.6406C10.7183 10.5156 10.7813 10.4561 10.7813 10.3828V4.40625H1.21875V10.3828C1.21875 10.418 1.23356 10.4518 1.25994 10.4767C1.28631 10.5017 1.32208 10.5156 1.35938 10.5156ZM4.61052 6.38251L5.9999 7.69472L7.38927 6.38251C7.44083 6.33007 7.50645 6.29173 7.57913 6.27153C7.6518 6.25134 7.72898 6.25003 7.8024 6.26776C7.87583 6.28549 7.9428 6.3216 7.99628 6.37227C8.04983 6.42295 8.08785 6.48631 8.10645 6.5557C8.12528 6.62497 8.12393 6.69773 8.10263 6.76635C8.0814 6.83497 8.0409 6.8969 7.98555 6.94564L6.29802 8.53936C6.21892 8.61399 6.11169 8.65592 5.9999 8.65592C5.8881 8.65592 5.78087 8.61399 5.70177 8.53936L4.01427 6.94564C3.95874 6.89694 3.91814 6.835 3.89676 6.76633C3.87538 6.69766 3.874 6.62483 3.89277 6.55549C3.91154 6.48615 3.94977 6.42287 4.00343 6.37233C4.05708 6.32179 4.12418 6.28585 4.19765 6.2683C4.27098 6.25054 4.34803 6.25178 4.42068 6.27188C4.49334 6.29198 4.55891 6.3302 4.61052 6.38251Z"
  );
  arrow.appendChild(path);
  return arrow;
};
var Collapse = (space, callback) => {
  const box = document.createElement("div");
  box.setAttribute("id", "space-header__collapse");
  box.style.display = "flex";
  box.style.flexDirection = "row";
  box.style.alignItems = "center";
  box.style.justifyContent = "center";
  box.style.fontSize = "16px";
  box.style.paddingLeft = "10px";
  box.style.paddingRight = "10px";
  box.style.height = "40px";
  box.style.cursor = "pointer";
  box.style.color = "#40546e";
  box.style.transitionDuration = "0.1s";
  box.style.transitionProperty = "all";
  box.style.transitionTimingFunction = "ease-in-out";
  box.appendChild(ArrowCollapse());
  box.addEventListener("click", (e) => {
    e.preventDefault();
    e.stopPropagation();
    callback();
  });
  box.addEventListener("mouseenter", () => {
    box.style.color = "#213551";
  });
  box.addEventListener("mouseleave", () => {
    box.style.color = "#40546e";
  });
  return box;
};
var Count = (count) => {
  const text = document.createElement("p");
  text.style.margin = "0";
  text.style.padding = "0";
  text.style.color = "#9ca3af";
  text.style.fontSize = "14px";
  text.style.fontFamily = "Source Sans Pro, sans-serif";
  text.style.padding = "0px 6px";
  text.style.borderLeft = "1px solid #e5e7eb";
  text.style.marginLeft = "4px";
  text.textContent = (count ?? 0).toString();
  return text;
};
var Heart = () => {
  const heart = document.createElementNS("http://www.w3.org/2000/svg", "svg");
  heart.setAttribute("xmlns", "http://www.w3.org/2000/svg");
  heart.setAttribute("xmlns:link", "http://www.w3.org/1999/xlink");
  heart.setAttribute("aria-hidden", "true");
  heart.setAttribute("focusable", "false");
  heart.setAttribute("role", "img");
  heart.setAttribute("width", "1em");
  heart.setAttribute("height", "1em");
  heart.setAttribute("preserveAspectRatio", "xMidYMid meet");
  heart.setAttribute("viewBox", "0 0 32 32");
  heart.setAttribute("fill", "#6b7280");
  const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
  path.setAttribute(
    "d",
    "M22.45,6a5.47,5.47,0,0,1,3.91,1.64,5.7,5.7,0,0,1,0,8L16,26.13,5.64,15.64a5.7,5.7,0,0,1,0-8,5.48,5.48,0,0,1,7.82,0L16,10.24l2.53-2.58A5.44,5.44,0,0,1,22.45,6m0-2a7.47,7.47,0,0,0-5.34,2.24L16,7.36,14.89,6.24a7.49,7.49,0,0,0-10.68,0,7.72,7.72,0,0,0,0,10.82L16,29,27.79,17.06a7.72,7.72,0,0,0,0-10.82A7.49,7.49,0,0,0,22.45,4Z"
  );
  heart.appendChild(path);
  return heart;
};
var Like = (space) => {
  const box = document.createElement("a");
  box.setAttribute("href", `https://huggingface.co/spaces/${space.id}`);
  box.setAttribute("rel", "noopener noreferrer");
  box.setAttribute("target", "_blank");
  box.style.border = "1px solid #e5e7eb";
  box.style.borderRadius = "6px";
  box.style.display = "flex";
  box.style.flexDirection = "row";
  box.style.alignItems = "center";
  box.style.margin = "0 0 0 12px";
  box.style.fontSize = "14px";
  box.style.paddingLeft = "4px";
  box.style.textDecoration = "none";
  box.appendChild(Heart());
  box.appendChild(Count(space.likes));
  return box;
};
var Avatar = (username) => {
  const element = document.createElement("img");
  element.src = `https://huggingface.co/api/users/${username}/avatar`;
  element.style.width = "0.875rem";
  element.style.height = "0.875rem";
  element.style.borderRadius = "50%";
  element.style.flex = "none";
  element.style.marginRight = "0.375rem";
  return element;
};
var Namespace = (id2) => {
  const [_, spaceName] = id2.split("/");
  const element = document.createElement("a");
  element.setAttribute("href", `https://huggingface.co/spaces/${id2}`);
  element.setAttribute("rel", "noopener noreferrer");
  element.setAttribute("target", "_blank");
  element.style.color = "#1f2937";
  element.style.textDecoration = "none";
  element.style.fontWeight = "600";
  element.style.fontSize = "15px";
  element.style.lineHeight = "24px";
  element.style.flex = "none";
  element.style.fontFamily = "IBM Plex Mono, sans-serif";
  element.addEventListener("mouseover", () => {
    element.style.color = "#2563eb";
  });
  element.addEventListener("mouseout", () => {
    element.style.color = "#1f2937";
  });
  element.textContent = spaceName;
  return element;
};
var Separation = () => {
  const separation = document.createElement("div");
  separation.style.marginLeft = ".125rem";
  separation.style.marginRight = ".125rem";
  separation.style.color = "#d1d5db";
  separation.textContent = "/";
  return separation;
};
var Username = (username) => {
  const element = document.createElement("a");
  element.setAttribute("href", `https://huggingface.co/${username}`);
  element.setAttribute("rel", "noopener noreferrer");
  element.setAttribute("target", "_blank");
  element.style.color = "rgb(107, 114, 128)";
  element.style.textDecoration = "none";
  element.style.fontWeight = "400";
  element.style.fontSize = "16px";
  element.style.lineHeight = "24px";
  element.style.flex = "none";
  element.style.fontFamily = "Source Sans Pro, sans-serif";
  element.addEventListener("mouseover", () => {
    element.style.color = "#2563eb";
  });
  element.addEventListener("mouseout", () => {
    element.style.color = "rgb(107, 114, 128)";
  });
  element.textContent = username;
  return element;
};
var Content = (space) => {
  const content = document.createElement("div");
  content.style.display = "flex";
  content.style.flexDirection = "row";
  content.style.alignItems = "center";
  content.style.justifyContent = "center";
  content.style.borderRight = "1px solid #e5e7eb";
  content.style.paddingRight = "12px";
  content.style.height = "40px";
  content.appendChild(Avatar(space.author));
  content.appendChild(Username(space.author));
  content.appendChild(Separation());
  content.appendChild(Namespace(space.id));
  content.appendChild(Like(space));
  return content;
};
var create = (space) => {
  const box = Box();
  const handleCollapse = () => box.style.display = "none";
  box.appendChild(Content(space));
  box.appendChild(Collapse(space, handleCollapse));
  return box;
};
var get_space = async (space_id) => {
  try {
    const response = await fetch(`https://huggingface.co/api/spaces/${space_id}`);
    const data = await response.json();
    return data;
  } catch (error) {
    return null;
  }
};
var inject = (element, options) => {
  if (document.body === null) {
    return console.error("document.body is null");
  }
  document.body.appendChild(element);
};
async function main(initialSpace, options) {
  if (window === void 0)
    return console.error("Please run this script in a browser environment");
  const has_huggingface_ancestor = Object.values(
    window.location?.ancestorOrigins ?? {
      0: window.document.referrer
    }
  ).some((origin) => new URL(origin)?.origin === "https://huggingface.co");
  if (has_huggingface_ancestor)
    return;
  inject_fonts();
  let space;
  if (typeof initialSpace === "string") {
    space = await get_space(initialSpace);
    if (space === null)
      return console.error("Space not found");
  } else {
    space = initialSpace;
  }
  const mini_header_element = create(space);
  inject(mini_header_element);
  return {
    element: mini_header_element
  };
}
var init = (space, options) => main(space);
let loader_status = "complete";
const Page = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let config;
  let $_, $$unsubscribe__;
  let $page, $$unsubscribe_page;
  $$unsubscribe__ = subscribe($format, (value) => $_ = value);
  $$unsubscribe_page = subscribe(page, (value) => $page = value);
  setupi18n();
  createEventDispatcher();
  let { data } = $$props;
  let { autoscroll = false } = $$props;
  let { version = "5-5-0" } = $$props;
  let { initial_height } = $$props;
  let { app_mode = true } = $$props;
  let { is_embed = false } = $$props;
  let { theme_mode = "system" } = $$props;
  let { control_page_title = true } = $$props;
  let { container } = $$props;
  let active_theme_mode;
  let { space } = $$props;
  let wrapper;
  let ready = false;
  let render_complete = false;
  $_("common.loading") + "...";
  ({
    register: () => {
    },
    subscribe: writable({}).subscribe
  });
  let app = data.app;
  let new_message_fn;
  let spaceheader;
  async function mount_space_header(space_id, is_embed2) {
    if (space_id && !is_embed2 && window.self === window.top) {
      if (spaceheader) {
        spaceheader.remove();
        spaceheader = void 0;
      }
      const header = await init(space_id);
      if (header)
        spaceheader = header.element;
    }
  }
  onDestroy(() => {
    spaceheader?.remove();
  });
  if ($$props.data === void 0 && $$bindings.data && data !== void 0)
    $$bindings.data(data);
  if ($$props.autoscroll === void 0 && $$bindings.autoscroll && autoscroll !== void 0)
    $$bindings.autoscroll(autoscroll);
  if ($$props.version === void 0 && $$bindings.version && version !== void 0)
    $$bindings.version(version);
  if ($$props.initial_height === void 0 && $$bindings.initial_height && initial_height !== void 0)
    $$bindings.initial_height(initial_height);
  if ($$props.app_mode === void 0 && $$bindings.app_mode && app_mode !== void 0)
    $$bindings.app_mode(app_mode);
  if ($$props.is_embed === void 0 && $$bindings.is_embed && is_embed !== void 0)
    $$bindings.is_embed(is_embed);
  if ($$props.theme_mode === void 0 && $$bindings.theme_mode && theme_mode !== void 0)
    $$bindings.theme_mode(theme_mode);
  if ($$props.control_page_title === void 0 && $$bindings.control_page_title && control_page_title !== void 0)
    $$bindings.control_page_title(control_page_title);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.space === void 0 && $$bindings.space && space !== void 0)
    $$bindings.space(space);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    config = data.config;
    {
      if (config?.app_id) {
        config.app_id;
      }
    }
    {
      if (render_complete) {
        wrapper.dispatchEvent(new CustomEvent(
          "render",
          {
            bubbles: true,
            cancelable: false,
            composed: true
          }
        ));
      }
    }
    app?.config && browser && mount_space_header(app?.config?.space_id, is_embed);
    $$rendered = `${$$result.head += `<!-- HEAD_svelte-1fnmhmq_START --><link rel="stylesheet"${add_attribute("href", "./theme.css?v=" + config?.theme_hash, 0)}>${config.head ? `<!-- HTML_TAG_START -->${config.head}<!-- HTML_TAG_END -->` : ``}<!-- HEAD_svelte-1fnmhmq_END -->`, ""} ${validate_component(Embed, "Embed").$$render(
      $$result,
      {
        display: container && is_embed,
        is_embed,
        info: false,
        version,
        initial_height,
        space,
        loaded: loader_status === "complete",
        fill_width: config?.fill_width || false,
        wrapper
      },
      {
        wrapper: ($$value) => {
          wrapper = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `${config?.auth_required ? `${validate_component(data.Render || missing_component, "svelte:component").$$render(
            $$result,
            {
              auth_message: config.auth_message,
              root: config.root,
              space_id: space,
              app_mode
            },
            {},
            {}
          )}` : `${config && app ? `${validate_component(data.Render || missing_component, "svelte:component").$$render(
            $$result,
            Object.assign(
              {},
              { app },
              config,
              {
                fill_height: !is_embed && config.fill_height
              },
              { theme_mode: active_theme_mode },
              { control_page_title },
              { target: wrapper },
              { autoscroll },
              { show_footer: !is_embed },
              { app_mode },
              { version },
              { search_params: $page.url.searchParams },
              { initial_layout: data.layout },
              { ready },
              { render_complete },
              { add_new_message: new_message_fn }
            ),
            {
              ready: ($$value) => {
                ready = $$value;
                $$settled = false;
              },
              render_complete: ($$value) => {
                render_complete = $$value;
                $$settled = false;
              },
              add_new_message: ($$value) => {
                new_message_fn = $$value;
                $$settled = false;
              }
            },
            {}
          )}` : ``}`}`;
        }
      }
    )} `;
  } while (!$$settled);
  $$unsubscribe__();
  $$unsubscribe_page();
  return $$rendered;
});

export { Page as default };
//# sourceMappingURL=_page.svelte-DtofCfl5.js.map
