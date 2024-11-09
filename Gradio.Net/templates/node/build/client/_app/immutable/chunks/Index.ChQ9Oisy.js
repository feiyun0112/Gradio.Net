import { SvelteComponent, init, safe_not_equal, element, space, create_component, claim_element, children, claim_space, claim_component, detach, attr, toggle_class, set_style, insert_hydration, append_hydration, mount_component, action_destroyer, transition_in, group_outros, transition_out, check_outros, destroy_component, createEventDispatcher, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { M as MarkdownCode, l as css_units, n as copy, I as IconButton, q as Check, r as Copy, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { default as default2 } from "./Example.BX_ByPoX.js";
function create_if_block(ctx) {
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
      if (dirty & /*$$scope, copied*/
      270336) {
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
function create_default_slot$1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: (
        /*copied*/
        ctx[13] ? Check : Copy
      ),
      label: (
        /*copied*/
        ctx[13] ? "Copied conversation" : "Copy conversation"
      )
    }
  });
  iconbutton.$on(
    "click",
    /*handle_copy*/
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
      if (dirty & /*copied*/
      8192)
        iconbutton_changes.Icon = /*copied*/
        ctx2[13] ? Check : Copy;
      if (dirty & /*copied*/
      8192)
        iconbutton_changes.label = /*copied*/
        ctx2[13] ? "Copied conversation" : "Copy conversation";
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
function create_fragment$1(ctx) {
  let div;
  let t;
  let markdowncode;
  let div_class_value;
  let div_dir_value;
  let div_style_value;
  let current;
  let mounted;
  let dispose;
  let if_block = (
    /*show_copy_button*/
    ctx[10] && create_if_block(ctx)
  );
  markdowncode = new MarkdownCode({
    props: {
      message: (
        /*value*/
        ctx[2]
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[7]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[5]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[6]
      ),
      chatbot: false,
      header_links: (
        /*header_links*/
        ctx[8]
      ),
      root: (
        /*root*/
        ctx[11]
      )
    }
  });
  return {
    c() {
      div = element("div");
      if (if_block)
        if_block.c();
      t = space();
      create_component(markdowncode.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {
        class: true,
        "data-testid": true,
        dir: true,
        style: true
      });
      var div_nodes = children(div);
      if (if_block)
        if_block.l(div_nodes);
      t = claim_space(div_nodes);
      claim_component(markdowncode.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      var _a;
      attr(div, "class", div_class_value = "prose " + /*elem_classes*/
      ctx[0].join(" ") + " svelte-lag733");
      attr(div, "data-testid", "markdown");
      attr(div, "dir", div_dir_value = /*rtl*/
      ctx[4] ? "rtl" : "ltr");
      attr(div, "style", div_style_value = /*height*/
      ctx[9] ? `max-height: ${css_units(
        /*height*/
        ctx[9]
      )}; overflow-y: auto;` : "");
      toggle_class(div, "hide", !/*visible*/
      ctx[1]);
      set_style(
        div,
        "min-height",
        /*min_height*/
        ctx[3] && /*loading_status*/
        ((_a = ctx[12]) == null ? void 0 : _a.status) !== "pending" ? css_units(
          /*min_height*/
          ctx[3]
        ) : void 0
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      if (if_block)
        if_block.m(div, null);
      append_hydration(div, t);
      mount_component(markdowncode, div, null);
      current = true;
      if (!mounted) {
        dispose = action_destroyer(copy.call(null, div));
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      var _a;
      if (
        /*show_copy_button*/
        ctx2[10]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
          if (dirty & /*show_copy_button*/
          1024) {
            transition_in(if_block, 1);
          }
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          transition_in(if_block, 1);
          if_block.m(div, t);
        }
      } else if (if_block) {
        group_outros();
        transition_out(if_block, 1, 1, () => {
          if_block = null;
        });
        check_outros();
      }
      const markdowncode_changes = {};
      if (dirty & /*value*/
      4)
        markdowncode_changes.message = /*value*/
        ctx2[2];
      if (dirty & /*latex_delimiters*/
      128)
        markdowncode_changes.latex_delimiters = /*latex_delimiters*/
        ctx2[7];
      if (dirty & /*sanitize_html*/
      32)
        markdowncode_changes.sanitize_html = /*sanitize_html*/
        ctx2[5];
      if (dirty & /*line_breaks*/
      64)
        markdowncode_changes.line_breaks = /*line_breaks*/
        ctx2[6];
      if (dirty & /*header_links*/
      256)
        markdowncode_changes.header_links = /*header_links*/
        ctx2[8];
      if (dirty & /*root*/
      2048)
        markdowncode_changes.root = /*root*/
        ctx2[11];
      markdowncode.$set(markdowncode_changes);
      if (!current || dirty & /*elem_classes*/
      1 && div_class_value !== (div_class_value = "prose " + /*elem_classes*/
      ctx2[0].join(" ") + " svelte-lag733")) {
        attr(div, "class", div_class_value);
      }
      if (!current || dirty & /*rtl*/
      16 && div_dir_value !== (div_dir_value = /*rtl*/
      ctx2[4] ? "rtl" : "ltr")) {
        attr(div, "dir", div_dir_value);
      }
      if (!current || dirty & /*height*/
      512 && div_style_value !== (div_style_value = /*height*/
      ctx2[9] ? `max-height: ${css_units(
        /*height*/
        ctx2[9]
      )}; overflow-y: auto;` : "")) {
        attr(div, "style", div_style_value);
      }
      if (!current || dirty & /*elem_classes, visible*/
      3) {
        toggle_class(div, "hide", !/*visible*/
        ctx2[1]);
      }
      const style_changed = dirty & /*height*/
      512;
      if (dirty & /*min_height, loading_status, height*/
      4616 || style_changed) {
        set_style(
          div,
          "min-height",
          /*min_height*/
          ctx2[3] && /*loading_status*/
          ((_a = ctx2[12]) == null ? void 0 : _a.status) !== "pending" ? css_units(
            /*min_height*/
            ctx2[3]
          ) : void 0
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block);
      transition_in(markdowncode.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block);
      transition_out(markdowncode.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      if (if_block)
        if_block.d();
      destroy_component(markdowncode);
      mounted = false;
      dispose();
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let { min_height = void 0 } = $$props;
  let { rtl = false } = $$props;
  let { sanitize_html = true } = $$props;
  let { line_breaks = false } = $$props;
  let { latex_delimiters } = $$props;
  let { header_links = false } = $$props;
  let { height = void 0 } = $$props;
  let { show_copy_button = false } = $$props;
  let { root } = $$props;
  let { loading_status = void 0 } = $$props;
  let copied = false;
  let timer;
  const dispatch = createEventDispatcher();
  async function handle_copy() {
    if ("clipboard" in navigator) {
      await navigator.clipboard.writeText(value);
      copy_feedback();
    }
  }
  function copy_feedback() {
    $$invalidate(13, copied = true);
    if (timer)
      clearTimeout(timer);
    timer = setTimeout(
      () => {
        $$invalidate(13, copied = false);
      },
      1e3
    );
  }
  $$self.$$set = ($$props2) => {
    if ("elem_classes" in $$props2)
      $$invalidate(0, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(1, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
    if ("min_height" in $$props2)
      $$invalidate(3, min_height = $$props2.min_height);
    if ("rtl" in $$props2)
      $$invalidate(4, rtl = $$props2.rtl);
    if ("sanitize_html" in $$props2)
      $$invalidate(5, sanitize_html = $$props2.sanitize_html);
    if ("line_breaks" in $$props2)
      $$invalidate(6, line_breaks = $$props2.line_breaks);
    if ("latex_delimiters" in $$props2)
      $$invalidate(7, latex_delimiters = $$props2.latex_delimiters);
    if ("header_links" in $$props2)
      $$invalidate(8, header_links = $$props2.header_links);
    if ("height" in $$props2)
      $$invalidate(9, height = $$props2.height);
    if ("show_copy_button" in $$props2)
      $$invalidate(10, show_copy_button = $$props2.show_copy_button);
    if ("root" in $$props2)
      $$invalidate(11, root = $$props2.root);
    if ("loading_status" in $$props2)
      $$invalidate(12, loading_status = $$props2.loading_status);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    4) {
      dispatch("change");
    }
  };
  return [
    elem_classes,
    visible,
    value,
    min_height,
    rtl,
    sanitize_html,
    line_breaks,
    latex_delimiters,
    header_links,
    height,
    show_copy_button,
    root,
    loading_status,
    copied,
    handle_copy
  ];
}
class Markdown extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      elem_classes: 0,
      visible: 1,
      value: 2,
      min_height: 3,
      rtl: 4,
      sanitize_html: 5,
      line_breaks: 6,
      latex_delimiters: 7,
      header_links: 8,
      height: 9,
      show_copy_button: 10,
      root: 11,
      loading_status: 12
    });
  }
}
const Markdown$1 = Markdown;
function create_default_slot(ctx) {
  let statustracker;
  let t;
  let div;
  let markdown;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[8].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[8].i18n
    ) },
    /*loading_status*/
    ctx[4],
    { variant: "center" }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[17]
  );
  markdown = new Markdown$1({
    props: {
      value: (
        /*value*/
        ctx[3]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      visible: (
        /*visible*/
        ctx[2]
      ),
      rtl: (
        /*rtl*/
        ctx[5]
      ),
      latex_delimiters: (
        /*latex_delimiters*/
        ctx[9]
      ),
      sanitize_html: (
        /*sanitize_html*/
        ctx[6]
      ),
      line_breaks: (
        /*line_breaks*/
        ctx[7]
      ),
      header_links: (
        /*header_links*/
        ctx[10]
      ),
      show_copy_button: (
        /*show_copy_button*/
        ctx[14]
      ),
      root: (
        /*gradio*/
        ctx[8].root
      ),
      loading_status: (
        /*loading_status*/
        ctx[4]
      )
    }
  });
  markdown.$on(
    "change",
    /*change_handler*/
    ctx[18]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t = space();
      div = element("div");
      create_component(markdown.$$.fragment);
      this.h();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(markdown.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      var _a;
      attr(div, "class", "svelte-1ed2p3z");
      toggle_class(
        div,
        "pending",
        /*loading_status*/
        ((_a = ctx[4]) == null ? void 0 : _a.status) === "pending"
      );
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, div, anchor);
      mount_component(markdown, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      var _a;
      const statustracker_changes = dirty & /*gradio, loading_status*/
      272 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*gradio*/
        256 && { autoscroll: (
          /*gradio*/
          ctx2[8].autoscroll
        ) },
        dirty & /*gradio*/
        256 && { i18n: (
          /*gradio*/
          ctx2[8].i18n
        ) },
        dirty & /*loading_status*/
        16 && get_spread_object(
          /*loading_status*/
          ctx2[4]
        ),
        statustracker_spread_levels[3]
      ]) : {};
      statustracker.$set(statustracker_changes);
      const markdown_changes = {};
      if (dirty & /*value*/
      8)
        markdown_changes.value = /*value*/
        ctx2[3];
      if (dirty & /*elem_classes*/
      2)
        markdown_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*visible*/
      4)
        markdown_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*rtl*/
      32)
        markdown_changes.rtl = /*rtl*/
        ctx2[5];
      if (dirty & /*latex_delimiters*/
      512)
        markdown_changes.latex_delimiters = /*latex_delimiters*/
        ctx2[9];
      if (dirty & /*sanitize_html*/
      64)
        markdown_changes.sanitize_html = /*sanitize_html*/
        ctx2[6];
      if (dirty & /*line_breaks*/
      128)
        markdown_changes.line_breaks = /*line_breaks*/
        ctx2[7];
      if (dirty & /*header_links*/
      1024)
        markdown_changes.header_links = /*header_links*/
        ctx2[10];
      if (dirty & /*show_copy_button*/
      16384)
        markdown_changes.show_copy_button = /*show_copy_button*/
        ctx2[14];
      if (dirty & /*gradio*/
      256)
        markdown_changes.root = /*gradio*/
        ctx2[8].root;
      if (dirty & /*loading_status*/
      16)
        markdown_changes.loading_status = /*loading_status*/
        ctx2[4];
      markdown.$set(markdown_changes);
      if (!current || dirty & /*loading_status*/
      16) {
        toggle_class(
          div,
          "pending",
          /*loading_status*/
          ((_a = ctx2[4]) == null ? void 0 : _a.status) === "pending"
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(markdown.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(markdown.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(div);
      }
      destroy_component(statustracker, detaching);
      destroy_component(markdown);
    }
  };
}
function create_fragment(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      visible: (
        /*visible*/
        ctx[2]
      ),
      elem_id: (
        /*elem_id*/
        ctx[0]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[1]
      ),
      container: (
        /*container*/
        ctx[15]
      ),
      allow_overflow: true,
      overflow_behavior: "auto",
      height: (
        /*height*/
        ctx[11]
      ),
      min_height: (
        /*min_height*/
        ctx[12]
      ),
      max_height: (
        /*max_height*/
        ctx[13]
      ),
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
    p(ctx2, [dirty]) {
      const block_changes = {};
      if (dirty & /*visible*/
      4)
        block_changes.visible = /*visible*/
        ctx2[2];
      if (dirty & /*elem_id*/
      1)
        block_changes.elem_id = /*elem_id*/
        ctx2[0];
      if (dirty & /*elem_classes*/
      2)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[1];
      if (dirty & /*container*/
      32768)
        block_changes.container = /*container*/
        ctx2[15];
      if (dirty & /*height*/
      2048)
        block_changes.height = /*height*/
        ctx2[11];
      if (dirty & /*min_height*/
      4096)
        block_changes.min_height = /*min_height*/
        ctx2[12];
      if (dirty & /*max_height*/
      8192)
        block_changes.max_height = /*max_height*/
        ctx2[13];
      if (dirty & /*$$scope, loading_status, value, elem_classes, visible, rtl, latex_delimiters, sanitize_html, line_breaks, header_links, show_copy_button, gradio*/
      542718) {
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
  let { label } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value = "" } = $$props;
  let { loading_status } = $$props;
  let { rtl = false } = $$props;
  let { sanitize_html = true } = $$props;
  let { line_breaks = false } = $$props;
  let { gradio } = $$props;
  let { latex_delimiters } = $$props;
  let { header_links = false } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let { show_copy_button = false } = $$props;
  let { container = false } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const change_handler = () => gradio.dispatch("change");
  $$self.$$set = ($$props2) => {
    if ("label" in $$props2)
      $$invalidate(16, label = $$props2.label);
    if ("elem_id" in $$props2)
      $$invalidate(0, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(1, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(2, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("loading_status" in $$props2)
      $$invalidate(4, loading_status = $$props2.loading_status);
    if ("rtl" in $$props2)
      $$invalidate(5, rtl = $$props2.rtl);
    if ("sanitize_html" in $$props2)
      $$invalidate(6, sanitize_html = $$props2.sanitize_html);
    if ("line_breaks" in $$props2)
      $$invalidate(7, line_breaks = $$props2.line_breaks);
    if ("gradio" in $$props2)
      $$invalidate(8, gradio = $$props2.gradio);
    if ("latex_delimiters" in $$props2)
      $$invalidate(9, latex_delimiters = $$props2.latex_delimiters);
    if ("header_links" in $$props2)
      $$invalidate(10, header_links = $$props2.header_links);
    if ("height" in $$props2)
      $$invalidate(11, height = $$props2.height);
    if ("min_height" in $$props2)
      $$invalidate(12, min_height = $$props2.min_height);
    if ("max_height" in $$props2)
      $$invalidate(13, max_height = $$props2.max_height);
    if ("show_copy_button" in $$props2)
      $$invalidate(14, show_copy_button = $$props2.show_copy_button);
    if ("container" in $$props2)
      $$invalidate(15, container = $$props2.container);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*label, gradio*/
    65792) {
      gradio.dispatch("change");
    }
  };
  return [
    elem_id,
    elem_classes,
    visible,
    value,
    loading_status,
    rtl,
    sanitize_html,
    line_breaks,
    gradio,
    latex_delimiters,
    header_links,
    height,
    min_height,
    max_height,
    show_copy_button,
    container,
    label,
    clear_status_handler,
    change_handler
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      label: 16,
      elem_id: 0,
      elem_classes: 1,
      visible: 2,
      value: 3,
      loading_status: 4,
      rtl: 5,
      sanitize_html: 6,
      line_breaks: 7,
      gradio: 8,
      latex_delimiters: 9,
      header_links: 10,
      height: 11,
      min_height: 12,
      max_height: 13,
      show_copy_button: 14,
      container: 15
    });
  }
}
export {
  default2 as BaseExample,
  Markdown$1 as BaseMarkdown,
  Index as default
};
