import { SvelteComponent, init, safe_not_equal, ensure_array_like, element, claim_element, children, detach, attr, set_style, insert_hydration, append_hydration, group_outros, update_keyed_each, outro_and_destroy_block, check_outros, transition_in, transition_out, createEventDispatcher, text, space, claim_text, claim_space, toggle_class, listen, set_data, get_svelte_dataset, run_all, noop, create_component, claim_component, mount_component, destroy_component, HtmlTagHydration, claim_html_tag, empty, bubble, tick, binding_callbacks, bind, add_flush_callback, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { I as IconButton, C as Clear } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { F as File$1 } from "./File.DqOJDDoa.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
import { U as Upload$1 } from "./Upload.v2Thvwuk.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
/* empty css                                                    */
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
const prettyBytes = (bytes) => {
  let units = ["B", "KB", "MB", "GB", "PB"];
  let i = 0;
  while (bytes > 1024) {
    bytes /= 1024;
    i++;
  }
  let unit = units[i];
  return bytes.toFixed(1) + "&nbsp;" + unit;
};
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[14] = list[i];
  child_ctx[16] = i;
  return child_ctx;
}
function create_else_block$2(ctx) {
  let t_value = (
    /*i18n*/
    ctx[2]("file.uploading") + ""
  );
  let t;
  return {
    c() {
      t = text(t_value);
    },
    l(nodes) {
      t = claim_text(nodes, t_value);
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*i18n*/
      4 && t_value !== (t_value = /*i18n*/
      ctx2[2]("file.uploading") + ""))
        set_data(t, t_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_if_block_1(ctx) {
  let downloadlink;
  let current;
  function click_handler() {
    return (
      /*click_handler*/
      ctx[9](
        /*file*/
        ctx[14]
      )
    );
  }
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*file*/
        ctx[14].url
      ),
      download: (
        /*is_browser*/
        ctx[7] && window.__is_colab__ ? null : (
          /*file*/
          ctx[14].orig_name
        )
      ),
      $$slots: { default: [create_default_slot$2] },
      $$scope: { ctx }
    }
  });
  downloadlink.$on("click", click_handler);
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
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const downloadlink_changes = {};
      if (dirty & /*normalized_files*/
      8)
        downloadlink_changes.href = /*file*/
        ctx[14].url;
      if (dirty & /*normalized_files*/
      8)
        downloadlink_changes.download = /*is_browser*/
        ctx[7] && window.__is_colab__ ? null : (
          /*file*/
          ctx[14].orig_name
        );
      if (dirty & /*$$scope, normalized_files*/
      131080) {
        downloadlink_changes.$$scope = { dirty, ctx };
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
function create_default_slot$2(ctx) {
  let html_tag;
  let raw_value = (
    /*file*/
    (ctx[14].size != null ? prettyBytes(
      /*file*/
      ctx[14].size
    ) : "(size unknown)") + ""
  );
  let t;
  return {
    c() {
      html_tag = new HtmlTagHydration(false);
      t = text(" ⇣");
      this.h();
    },
    l(nodes) {
      html_tag = claim_html_tag(nodes, false);
      t = claim_text(nodes, " ⇣");
      this.h();
    },
    h() {
      html_tag.a = t;
    },
    m(target, anchor) {
      html_tag.m(raw_value, target, anchor);
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*normalized_files*/
      8 && raw_value !== (raw_value = /*file*/
      (ctx2[14].size != null ? prettyBytes(
        /*file*/
        ctx2[14].size
      ) : "(size unknown)") + ""))
        html_tag.p(raw_value);
    },
    d(detaching) {
      if (detaching) {
        html_tag.d();
        detach(t);
      }
    }
  };
}
function create_if_block$2(ctx) {
  let td;
  let button;
  let textContent = "×";
  let mounted;
  let dispose;
  function click_handler_1() {
    return (
      /*click_handler_1*/
      ctx[10](
        /*i*/
        ctx[16]
      )
    );
  }
  function keydown_handler(...args) {
    return (
      /*keydown_handler*/
      ctx[11](
        /*i*/
        ctx[16],
        ...args
      )
    );
  }
  return {
    c() {
      td = element("td");
      button = element("button");
      button.textContent = textContent;
      this.h();
    },
    l(nodes) {
      td = claim_element(nodes, "TD", { class: true });
      var td_nodes = children(td);
      button = claim_element(td_nodes, "BUTTON", {
        class: true,
        "aria-label": true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(button) !== "svelte-nhtord")
        button.textContent = textContent;
      td_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "label-clear-button svelte-18wv37q");
      attr(button, "aria-label", "Remove this file");
      attr(td, "class", "svelte-18wv37q");
    },
    m(target, anchor) {
      insert_hydration(target, td, anchor);
      append_hydration(td, button);
      if (!mounted) {
        dispose = [
          listen(button, "click", click_handler_1),
          listen(button, "keydown", keydown_handler)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
    },
    d(detaching) {
      if (detaching) {
        detach(td);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_each_block(key_1, ctx) {
  let tr;
  let td0;
  let span0;
  let t0_value = (
    /*file*/
    ctx[14].filename_stem + ""
  );
  let t0;
  let t1;
  let span1;
  let t2_value = (
    /*file*/
    ctx[14].filename_ext + ""
  );
  let t2;
  let td0_aria_label_value;
  let t3;
  let td1;
  let current_block_type_index;
  let if_block0;
  let t4;
  let t5;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block_1, create_else_block$2];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*file*/
      ctx2[14].url
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  let if_block1 = (
    /*normalized_files*/
    ctx[3].length > 1 && create_if_block$2(ctx)
  );
  function click_handler_2(...args) {
    return (
      /*click_handler_2*/
      ctx[12](
        /*i*/
        ctx[16],
        ...args
      )
    );
  }
  return {
    key: key_1,
    first: null,
    c() {
      tr = element("tr");
      td0 = element("td");
      span0 = element("span");
      t0 = text(t0_value);
      t1 = space();
      span1 = element("span");
      t2 = text(t2_value);
      t3 = space();
      td1 = element("td");
      if_block0.c();
      t4 = space();
      if (if_block1)
        if_block1.c();
      t5 = space();
      this.h();
    },
    l(nodes) {
      tr = claim_element(nodes, "TR", { class: true });
      var tr_nodes = children(tr);
      td0 = claim_element(tr_nodes, "TD", { class: true, "aria-label": true });
      var td0_nodes = children(td0);
      span0 = claim_element(td0_nodes, "SPAN", { class: true });
      var span0_nodes = children(span0);
      t0 = claim_text(span0_nodes, t0_value);
      span0_nodes.forEach(detach);
      t1 = claim_space(td0_nodes);
      span1 = claim_element(td0_nodes, "SPAN", { class: true });
      var span1_nodes = children(span1);
      t2 = claim_text(span1_nodes, t2_value);
      span1_nodes.forEach(detach);
      td0_nodes.forEach(detach);
      t3 = claim_space(tr_nodes);
      td1 = claim_element(tr_nodes, "TD", { class: true });
      var td1_nodes = children(td1);
      if_block0.l(td1_nodes);
      td1_nodes.forEach(detach);
      t4 = claim_space(tr_nodes);
      if (if_block1)
        if_block1.l(tr_nodes);
      t5 = claim_space(tr_nodes);
      tr_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span0, "class", "stem svelte-18wv37q");
      attr(span1, "class", "ext svelte-18wv37q");
      attr(td0, "class", "filename svelte-18wv37q");
      attr(td0, "aria-label", td0_aria_label_value = /*file*/
      ctx[14].orig_name);
      attr(td1, "class", "download svelte-18wv37q");
      attr(tr, "class", "file svelte-18wv37q");
      toggle_class(
        tr,
        "selectable",
        /*selectable*/
        ctx[0]
      );
      this.first = tr;
    },
    m(target, anchor) {
      insert_hydration(target, tr, anchor);
      append_hydration(tr, td0);
      append_hydration(td0, span0);
      append_hydration(span0, t0);
      append_hydration(td0, t1);
      append_hydration(td0, span1);
      append_hydration(span1, t2);
      append_hydration(tr, t3);
      append_hydration(tr, td1);
      if_blocks[current_block_type_index].m(td1, null);
      append_hydration(tr, t4);
      if (if_block1)
        if_block1.m(tr, null);
      append_hydration(tr, t5);
      current = true;
      if (!mounted) {
        dispose = listen(tr, "click", click_handler_2);
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if ((!current || dirty & /*normalized_files*/
      8) && t0_value !== (t0_value = /*file*/
      ctx[14].filename_stem + ""))
        set_data(t0, t0_value);
      if ((!current || dirty & /*normalized_files*/
      8) && t2_value !== (t2_value = /*file*/
      ctx[14].filename_ext + ""))
        set_data(t2, t2_value);
      if (!current || dirty & /*normalized_files*/
      8 && td0_aria_label_value !== (td0_aria_label_value = /*file*/
      ctx[14].orig_name)) {
        attr(td0, "aria-label", td0_aria_label_value);
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx);
      if (current_block_type_index === previous_block_index) {
        if_blocks[current_block_type_index].p(ctx, dirty);
      } else {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
          if_block0.c();
        } else {
          if_block0.p(ctx, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(td1, null);
      }
      if (
        /*normalized_files*/
        ctx[3].length > 1
      ) {
        if (if_block1) {
          if_block1.p(ctx, dirty);
        } else {
          if_block1 = create_if_block$2(ctx);
          if_block1.c();
          if_block1.m(tr, t5);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
      if (!current || dirty & /*selectable*/
      1) {
        toggle_class(
          tr,
          "selectable",
          /*selectable*/
          ctx[0]
        );
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(tr);
      }
      if_blocks[current_block_type_index].d();
      if (if_block1)
        if_block1.d();
      mounted = false;
      dispose();
    }
  };
}
function create_fragment$2(ctx) {
  let div;
  let table;
  let tbody;
  let each_blocks = [];
  let each_1_lookup = /* @__PURE__ */ new Map();
  let current;
  let each_value = ensure_array_like(
    /*normalized_files*/
    ctx[3]
  );
  const get_key = (ctx2) => (
    /*file*/
    ctx2[14]
  );
  for (let i = 0; i < each_value.length; i += 1) {
    let child_ctx = get_each_context(ctx, each_value, i);
    let key = get_key(child_ctx);
    each_1_lookup.set(key, each_blocks[i] = create_each_block(key, child_ctx));
  }
  return {
    c() {
      div = element("div");
      table = element("table");
      tbody = element("tbody");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, style: true });
      var div_nodes = children(div);
      table = claim_element(div_nodes, "TABLE", { class: true });
      var table_nodes = children(table);
      tbody = claim_element(table_nodes, "TBODY", { class: true });
      var tbody_nodes = children(tbody);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(tbody_nodes);
      }
      tbody_nodes.forEach(detach);
      table_nodes.forEach(detach);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(tbody, "class", "svelte-18wv37q");
      attr(table, "class", "file-preview svelte-18wv37q");
      attr(div, "class", "file-preview-holder svelte-18wv37q");
      set_style(div, "max-height", typeof /*height*/
      ctx[1] === void 0 ? "auto" : (
        /*height*/
        ctx[1] + "px"
      ));
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, table);
      append_hydration(table, tbody);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(tbody, null);
        }
      }
      current = true;
    },
    p(ctx2, [dirty]) {
      if (dirty & /*selectable, handle_row_click, normalized_files, remove_file, is_browser, window, handle_download, i18n*/
      253) {
        each_value = ensure_array_like(
          /*normalized_files*/
          ctx2[3]
        );
        group_outros();
        each_blocks = update_keyed_each(each_blocks, dirty, get_key, 1, ctx2, each_value, each_1_lookup, tbody, outro_and_destroy_block, create_each_block, null, get_each_context);
        check_outros();
      }
      if (!current || dirty & /*height*/
      2) {
        set_style(div, "max-height", typeof /*height*/
        ctx2[1] === void 0 ? "auto" : (
          /*height*/
          ctx2[1] + "px"
        ));
      }
    },
    i(local) {
      if (current)
        return;
      for (let i = 0; i < each_value.length; i += 1) {
        transition_in(each_blocks[i]);
      }
      current = true;
    },
    o(local) {
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].d();
      }
    }
  };
}
function split_filename(filename) {
  const last_dot = filename.lastIndexOf(".");
  if (last_dot === -1) {
    return [filename, ""];
  }
  return [filename.slice(0, last_dot), filename.slice(last_dot)];
}
function instance$2($$self, $$props, $$invalidate) {
  let normalized_files;
  const dispatch = createEventDispatcher();
  let { value } = $$props;
  let { selectable = false } = $$props;
  let { height = void 0 } = $$props;
  let { i18n } = $$props;
  function handle_row_click(event, index) {
    const tr = event.currentTarget;
    const should_select = event.target === tr || // Only select if the click is on the row itself
    tr && tr.firstElementChild && event.composedPath().includes(tr.firstElementChild);
    if (should_select) {
      dispatch("select", {
        value: normalized_files[index].orig_name,
        index
      });
    }
  }
  function remove_file(index) {
    const removed = normalized_files.splice(index, 1);
    $$invalidate(3, normalized_files = [...normalized_files]);
    $$invalidate(8, value = normalized_files);
    dispatch("delete", removed[0]);
    dispatch("change", normalized_files);
  }
  function handle_download(file) {
    dispatch("download", file);
  }
  const is_browser = typeof window !== "undefined";
  const click_handler = (file) => handle_download(file);
  const click_handler_1 = (i) => {
    remove_file(i);
  };
  const keydown_handler = (i, event) => {
    if (event.key === "Enter") {
      remove_file(i);
    }
  };
  const click_handler_2 = (i, event) => {
    handle_row_click(event, i);
  };
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(8, value = $$props2.value);
    if ("selectable" in $$props2)
      $$invalidate(0, selectable = $$props2.selectable);
    if ("height" in $$props2)
      $$invalidate(1, height = $$props2.height);
    if ("i18n" in $$props2)
      $$invalidate(2, i18n = $$props2.i18n);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    256) {
      $$invalidate(3, normalized_files = (Array.isArray(value) ? value : [value]).map((file) => {
        const [filename_stem, filename_ext] = split_filename(file.orig_name ?? "");
        return { ...file, filename_stem, filename_ext };
      }));
    }
  };
  return [
    selectable,
    height,
    i18n,
    normalized_files,
    handle_row_click,
    remove_file,
    handle_download,
    is_browser,
    value,
    click_handler,
    click_handler_1,
    keydown_handler,
    click_handler_2
  ];
}
class FilePreview extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      value: 8,
      selectable: 0,
      height: 1,
      i18n: 2
    });
  }
}
const FilePreview$1 = FilePreview;
function create_else_block$1(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot$1] },
      $$scope: { ctx }
    }
  });
  return {
    c() {
      create_component(empty_1.$$.fragment);
    },
    l(nodes) {
      claim_component(empty_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(empty_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const empty_1_changes = {};
      if (dirty & /*$$scope*/
      256) {
        empty_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      empty_1.$set(empty_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(empty_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(empty_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(empty_1, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let filepreview;
  let current;
  filepreview = new FilePreview$1({
    props: {
      i18n: (
        /*i18n*/
        ctx[5]
      ),
      selectable: (
        /*selectable*/
        ctx[3]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      height: (
        /*height*/
        ctx[4]
      )
    }
  });
  filepreview.$on(
    "select",
    /*select_handler*/
    ctx[6]
  );
  filepreview.$on(
    "download",
    /*download_handler*/
    ctx[7]
  );
  return {
    c() {
      create_component(filepreview.$$.fragment);
    },
    l(nodes) {
      claim_component(filepreview.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(filepreview, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const filepreview_changes = {};
      if (dirty & /*i18n*/
      32)
        filepreview_changes.i18n = /*i18n*/
        ctx2[5];
      if (dirty & /*selectable*/
      8)
        filepreview_changes.selectable = /*selectable*/
        ctx2[3];
      if (dirty & /*value*/
      1)
        filepreview_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*height*/
      16)
        filepreview_changes.height = /*height*/
        ctx2[4];
      filepreview.$set(filepreview_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(filepreview.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(filepreview.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(filepreview, detaching);
    }
  };
}
function create_default_slot$1(ctx) {
  let file;
  let current;
  file = new File$1({});
  return {
    c() {
      create_component(file.$$.fragment);
    },
    l(nodes) {
      claim_component(file.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(file, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(file.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(file.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(file, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let blocklabel;
  let t;
  let show_if;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[2]
      ),
      float: (
        /*value*/
        ctx[0] === null
      ),
      Icon: File$1,
      label: (
        /*label*/
        ctx[1] || "File"
      )
    }
  });
  const if_block_creators = [create_if_block$1, create_else_block$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (dirty & /*value*/
    1)
      show_if = null;
    if (show_if == null)
      show_if = !!/*value*/
      (ctx2[0] && (Array.isArray(
        /*value*/
        ctx2[0]
      ) ? (
        /*value*/
        ctx2[0].length > 0
      ) : true));
    if (show_if)
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx, -1);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      4)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[2];
      if (dirty & /*value*/
      1)
        blocklabel_changes.float = /*value*/
        ctx2[0] === null;
      if (dirty & /*label*/
      2)
        blocklabel_changes.label = /*label*/
        ctx2[1] || "File";
      blocklabel.$set(blocklabel_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2, dirty);
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
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { value = null } = $$props;
  let { label } = $$props;
  let { show_label = true } = $$props;
  let { selectable = false } = $$props;
  let { height = void 0 } = $$props;
  let { i18n } = $$props;
  function select_handler(event) {
    bubble.call(this, $$self, event);
  }
  function download_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("selectable" in $$props2)
      $$invalidate(3, selectable = $$props2.selectable);
    if ("height" in $$props2)
      $$invalidate(4, height = $$props2.height);
    if ("i18n" in $$props2)
      $$invalidate(5, i18n = $$props2.i18n);
  };
  return [
    value,
    label,
    show_label,
    selectable,
    height,
    i18n,
    select_handler,
    download_handler
  ];
}
class File_1 extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      value: 0,
      label: 1,
      show_label: 2,
      selectable: 3,
      height: 4,
      i18n: 5
    });
  }
}
const File = File_1;
function create_else_block(ctx) {
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let current;
  function upload_1_dragging_binding_1(value) {
    ctx[25](value);
  }
  function upload_1_uploading_binding_1(value) {
    ctx[26](value);
  }
  let upload_1_props = {
    filetype: (
      /*file_types*/
      ctx[5]
    ),
    file_count: (
      /*file_count*/
      ctx[4]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[10]
    ),
    root: (
      /*root*/
      ctx[7]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[12]
    ),
    upload: (
      /*upload*/
      ctx[11]
    ),
    $$slots: { default: [create_default_slot_2] },
    $$scope: { ctx }
  };
  if (
    /*dragging*/
    ctx[13] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[13];
  }
  if (
    /*uploading*/
    ctx[1] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[1];
  }
  upload_1 = new Upload({ props: upload_1_props });
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding_1));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding_1));
  upload_1.$on(
    "load",
    /*handle_upload*/
    ctx[14]
  );
  upload_1.$on(
    "error",
    /*error_handler_1*/
    ctx[27]
  );
  return {
    c() {
      create_component(upload_1.$$.fragment);
    },
    l(nodes) {
      claim_component(upload_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(upload_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const upload_1_changes = {};
      if (dirty & /*file_types*/
      32)
        upload_1_changes.filetype = /*file_types*/
        ctx2[5];
      if (dirty & /*file_count*/
      16)
        upload_1_changes.file_count = /*file_count*/
        ctx2[4];
      if (dirty & /*max_file_size*/
      1024)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[10];
      if (dirty & /*root*/
      128)
        upload_1_changes.root = /*root*/
        ctx2[7];
      if (dirty & /*stream_handler*/
      4096)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[12];
      if (dirty & /*upload*/
      2048)
        upload_1_changes.upload = /*upload*/
        ctx2[11];
      if (dirty & /*$$scope*/
      268435456) {
        upload_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_dragging && dirty & /*dragging*/
      8192) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[13];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty & /*uploading*/
      2) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[1];
        add_flush_callback(() => updating_uploading = false);
      }
      upload_1.$set(upload_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(upload_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(upload_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(upload_1, detaching);
    }
  };
}
function create_if_block(ctx) {
  let iconbuttonwrapper;
  let t;
  let filepreview;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  filepreview = new FilePreview$1({
    props: {
      i18n: (
        /*i18n*/
        ctx[9]
      ),
      selectable: (
        /*selectable*/
        ctx[6]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      height: (
        /*height*/
        ctx[8]
      )
    }
  });
  filepreview.$on(
    "select",
    /*select_handler*/
    ctx[22]
  );
  filepreview.$on(
    "change",
    /*change_handler*/
    ctx[23]
  );
  filepreview.$on(
    "delete",
    /*delete_handler*/
    ctx[24]
  );
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
      t = space();
      create_component(filepreview.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
      t = claim_space(nodes);
      claim_component(filepreview.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      insert_hydration(target, t, anchor);
      mount_component(filepreview, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, i18n, file_types, file_count, max_file_size, root, stream_handler, upload, dragging, uploading*/
      268451506) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      const filepreview_changes = {};
      if (dirty & /*i18n*/
      512)
        filepreview_changes.i18n = /*i18n*/
        ctx2[9];
      if (dirty & /*selectable*/
      64)
        filepreview_changes.selectable = /*selectable*/
        ctx2[6];
      if (dirty & /*value*/
      1)
        filepreview_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*height*/
      256)
        filepreview_changes.height = /*height*/
        ctx2[8];
      filepreview.$set(filepreview_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(filepreview.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(filepreview.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
      destroy_component(iconbuttonwrapper, detaching);
      destroy_component(filepreview, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[17].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[28],
    null
  );
  return {
    c() {
      if (default_slot)
        default_slot.c();
    },
    l(nodes) {
      if (default_slot)
        default_slot.l(nodes);
    },
    m(target, anchor) {
      if (default_slot) {
        default_slot.m(target, anchor);
      }
      current = true;
    },
    p(ctx2, dirty) {
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        268435456)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[28],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[28]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[28],
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
      if (default_slot)
        default_slot.d(detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let current;
  function upload_1_dragging_binding(value) {
    ctx[18](value);
  }
  function upload_1_uploading_binding(value) {
    ctx[19](value);
  }
  let upload_1_props = {
    icon_upload: true,
    filetype: (
      /*file_types*/
      ctx[5]
    ),
    file_count: (
      /*file_count*/
      ctx[4]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[10]
    ),
    root: (
      /*root*/
      ctx[7]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[12]
    ),
    upload: (
      /*upload*/
      ctx[11]
    )
  };
  if (
    /*dragging*/
    ctx[13] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[13];
  }
  if (
    /*uploading*/
    ctx[1] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[1];
  }
  upload_1 = new Upload({ props: upload_1_props });
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  upload_1.$on(
    "load",
    /*handle_upload*/
    ctx[14]
  );
  upload_1.$on(
    "error",
    /*error_handler*/
    ctx[20]
  );
  return {
    c() {
      create_component(upload_1.$$.fragment);
    },
    l(nodes) {
      claim_component(upload_1.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(upload_1, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const upload_1_changes = {};
      if (dirty & /*file_types*/
      32)
        upload_1_changes.filetype = /*file_types*/
        ctx2[5];
      if (dirty & /*file_count*/
      16)
        upload_1_changes.file_count = /*file_count*/
        ctx2[4];
      if (dirty & /*max_file_size*/
      1024)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[10];
      if (dirty & /*root*/
      128)
        upload_1_changes.root = /*root*/
        ctx2[7];
      if (dirty & /*stream_handler*/
      4096)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[12];
      if (dirty & /*upload*/
      2048)
        upload_1_changes.upload = /*upload*/
        ctx2[11];
      if (!updating_dragging && dirty & /*dragging*/
      8192) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[13];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty & /*uploading*/
      2) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[1];
        add_flush_callback(() => updating_uploading = false);
      }
      upload_1.$set(upload_1_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(upload_1.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(upload_1.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(upload_1, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let iconbutton0;
  let t;
  let iconbutton1;
  let current;
  iconbutton0 = new IconButton({
    props: {
      Icon: Upload$1,
      label: (
        /*i18n*/
        ctx[9]("common.upload")
      ),
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
  iconbutton1 = new IconButton({
    props: {
      Icon: Clear,
      label: (
        /*i18n*/
        ctx[9]("common.clear")
      )
    }
  });
  iconbutton1.$on(
    "click",
    /*click_handler*/
    ctx[21]
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
    p(ctx2, dirty) {
      const iconbutton0_changes = {};
      if (dirty & /*i18n*/
      512)
        iconbutton0_changes.label = /*i18n*/
        ctx2[9]("common.upload");
      if (dirty & /*$$scope, file_types, file_count, max_file_size, root, stream_handler, upload, dragging, uploading*/
      268450994) {
        iconbutton0_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbutton0.$set(iconbutton0_changes);
      const iconbutton1_changes = {};
      if (dirty & /*i18n*/
      512)
        iconbutton1_changes.label = /*i18n*/
        ctx2[9]("common.clear");
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
function create_fragment(ctx) {
  let blocklabel;
  let t;
  let show_if;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[3]
      ),
      Icon: File$1,
      float: !/*value*/
      ctx[0],
      label: (
        /*label*/
        ctx[2] || "File"
      )
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (dirty & /*value*/
    1)
      show_if = null;
    if (show_if == null)
      show_if = !!/*value*/
      (ctx2[0] && (Array.isArray(
        /*value*/
        ctx2[0]
      ) ? (
        /*value*/
        ctx2[0].length > 0
      ) : true));
    if (show_if)
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx, -1);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, [dirty]) {
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      8)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[3];
      if (dirty & /*value*/
      1)
        blocklabel_changes.float = !/*value*/
        ctx2[0];
      if (dirty & /*label*/
      4)
        blocklabel_changes.label = /*label*/
        ctx2[2] || "File";
      blocklabel.$set(blocklabel_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2, dirty);
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
      transition_in(blocklabel.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block_anchor);
      }
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { value } = $$props;
  let { label } = $$props;
  let { show_label = true } = $$props;
  let { file_count = "single" } = $$props;
  let { file_types = null } = $$props;
  let { selectable = false } = $$props;
  let { root } = $$props;
  let { height = void 0 } = $$props;
  let { i18n } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { uploading = false } = $$props;
  async function handle_upload({ detail }) {
    if (Array.isArray(value)) {
      $$invalidate(0, value = [...value, ...Array.isArray(detail) ? detail : [detail]]);
    } else if (value) {
      $$invalidate(0, value = [value, ...Array.isArray(detail) ? detail : [detail]]);
    } else {
      $$invalidate(0, value = detail);
    }
    await tick();
    dispatch("change", value);
    dispatch("upload", detail);
  }
  function handle_clear() {
    $$invalidate(0, value = null);
    dispatch("change", null);
    dispatch("clear");
  }
  const dispatch = createEventDispatcher();
  let dragging = false;
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(13, dragging);
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(1, uploading);
  }
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  const click_handler = (event) => {
    dispatch("clear");
    event.stopPropagation();
    handle_clear();
  };
  function select_handler(event) {
    bubble.call(this, $$self, event);
  }
  function change_handler(event) {
    bubble.call(this, $$self, event);
  }
  function delete_handler(event) {
    bubble.call(this, $$self, event);
  }
  function upload_1_dragging_binding_1(value2) {
    dragging = value2;
    $$invalidate(13, dragging);
  }
  function upload_1_uploading_binding_1(value2) {
    uploading = value2;
    $$invalidate(1, uploading);
  }
  function error_handler_1(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(3, show_label = $$props2.show_label);
    if ("file_count" in $$props2)
      $$invalidate(4, file_count = $$props2.file_count);
    if ("file_types" in $$props2)
      $$invalidate(5, file_types = $$props2.file_types);
    if ("selectable" in $$props2)
      $$invalidate(6, selectable = $$props2.selectable);
    if ("root" in $$props2)
      $$invalidate(7, root = $$props2.root);
    if ("height" in $$props2)
      $$invalidate(8, height = $$props2.height);
    if ("i18n" in $$props2)
      $$invalidate(9, i18n = $$props2.i18n);
    if ("max_file_size" in $$props2)
      $$invalidate(10, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(11, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(12, stream_handler = $$props2.stream_handler);
    if ("uploading" in $$props2)
      $$invalidate(1, uploading = $$props2.uploading);
    if ("$$scope" in $$props2)
      $$invalidate(28, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*dragging*/
    8192) {
      dispatch("drag", dragging);
    }
  };
  return [
    value,
    uploading,
    label,
    show_label,
    file_count,
    file_types,
    selectable,
    root,
    height,
    i18n,
    max_file_size,
    upload,
    stream_handler,
    dragging,
    handle_upload,
    handle_clear,
    dispatch,
    slots,
    upload_1_dragging_binding,
    upload_1_uploading_binding,
    error_handler,
    click_handler,
    select_handler,
    change_handler,
    delete_handler,
    upload_1_dragging_binding_1,
    upload_1_uploading_binding_1,
    error_handler_1,
    $$scope
  ];
}
class FileUpload extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      label: 2,
      show_label: 3,
      file_count: 4,
      file_types: 5,
      selectable: 6,
      root: 7,
      height: 8,
      i18n: 9,
      max_file_size: 10,
      upload: 11,
      stream_handler: 12,
      uploading: 1
    });
  }
}
const BaseFileUpload = FileUpload;
export {
  BaseFileUpload as B,
  File as F,
  FilePreview$1 as a
};
