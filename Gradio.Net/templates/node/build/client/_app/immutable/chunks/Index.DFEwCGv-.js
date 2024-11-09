import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, set_style, insert_hydration, append_hydration, noop, element, claim_element, toggle_class, listen, run_all, createEventDispatcher, ensure_array_like, transition_in, group_outros, check_outros, transition_out, destroy_each, space, text, claim_space, claim_text, set_data, bubble, create_component, claim_component, mount_component, destroy_component, src_url_equal, stop_propagation, flush, assign, empty, get_spread_update, get_spread_object, binding_callbacks, bind, add_flush_callback } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, S as Static } from "./2.BqWhUxOo.js";
import { F as File } from "./File.DqOJDDoa.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
function create_fragment$4(ctx) {
  let svg;
  let g;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      g = svg_element("g");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        version: true,
        style: true
      });
      var svg_nodes = children(svg);
      g = claim_svg_element(svg_nodes, "g", { transform: true });
      var g_nodes = children(g);
      path = claim_svg_element(g_nodes, "path", { d: true, style: true });
      children(path).forEach(detach);
      g_nodes.forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "d", "M12.7,24.033C12.256,24.322 11.806,24.339 11.351,24.084C10.896,23.829 10.668,23.434 10.667,22.9L10.667,9.1C10.667,8.567 10.895,8.172 11.351,7.916C11.807,7.66 12.256,7.677 12.7,7.967L23.567,14.867C23.967,15.133 24.167,15.511 24.167,16C24.167,16.489 23.967,16.867 23.567,17.133L12.7,24.033Z");
      set_style(path, "fill", "currentColor");
      set_style(path, "fill-rule", "nonzero");
      attr(g, "transform", "matrix(1,0,0,1,-10.6667,-7.73588)");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 14 17");
      attr(svg, "version", "1.1");
      set_style(svg, "fill-rule", "evenodd");
      set_style(svg, "clip-rule", "evenodd");
      set_style(svg, "stroke-linejoin", "round");
      set_style(svg, "stroke-miterlimit", "2");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, g);
      append_hydration(g, path);
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
class ArrowIcon extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$4, safe_not_equal, {});
  }
}
function create_fragment$3(ctx) {
  let input;
  let mounted;
  let dispose;
  return {
    c() {
      input = element("input");
      this.h();
    },
    l(nodes) {
      input = claim_element(nodes, "INPUT", { type: true, class: true });
      this.h();
    },
    h() {
      attr(input, "type", "checkbox");
      input.disabled = /*disabled*/
      ctx[1];
      attr(input, "class", "svelte-1j130g3");
      toggle_class(
        input,
        "disabled",
        /*disabled*/
        ctx[1] && !/*value*/
        ctx[0]
      );
    },
    m(target, anchor) {
      insert_hydration(target, input, anchor);
      input.checked = /*value*/
      ctx[0];
      if (!mounted) {
        dispose = [
          listen(
            input,
            "change",
            /*input_change_handler*/
            ctx[3]
          ),
          listen(
            input,
            "input",
            /*input_handler*/
            ctx[4]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*disabled*/
      2) {
        input.disabled = /*disabled*/
        ctx2[1];
      }
      if (dirty & /*value*/
      1) {
        input.checked = /*value*/
        ctx2[0];
      }
      if (dirty & /*disabled, value*/
      3) {
        toggle_class(
          input,
          "disabled",
          /*disabled*/
          ctx2[1] && !/*value*/
          ctx2[0]
        );
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(input);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$3($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let { disabled } = $$props;
  const dispatch = createEventDispatcher();
  function input_change_handler() {
    value = this.checked;
    $$invalidate(0, value);
  }
  const input_handler = () => dispatch("change", !value);
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("disabled" in $$props2)
      $$invalidate(1, disabled = $$props2.disabled);
  };
  return [value, disabled, dispatch, input_change_handler, input_handler];
}
class Checkbox extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, { value: 0, disabled: 1 });
  }
}
const FileIcon = "data:image/svg+xml,%3csvg%20xmlns='http://www.w3.org/2000/svg'%20width='32'%20height='32'%20viewBox='0%200%2024%2024'%3e%3cpath%20fill='%23888888'%20d='M6%202c-1.1%200-1.99.9-1.99%202L4%2020c0%201.1.89%202%201.99%202H18c1.1%200%202-.9%202-2V8l-6-6H6zm7%207V3.5L18.5%209H13z'/%3e%3c/svg%3e";
const FolderIcon = "data:image/svg+xml,%3c?xml%20version='1.0'%20encoding='UTF-8'%20standalone='no'?%3e%3csvg%20viewBox='0%200%2032%2032'%20version='1.1'%20id='svg7'%20sodipodi:docname='light-folder-new.svg'%20inkscape:version='1.3.2%20(091e20e,%202023-11-25)'%20xmlns:inkscape='http://www.inkscape.org/namespaces/inkscape'%20xmlns:sodipodi='http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd'%20xmlns='http://www.w3.org/2000/svg'%20xmlns:svg='http://www.w3.org/2000/svg'%3e%3csodipodi:namedview%20id='namedview7'%20pagecolor='%23ffffff'%20bordercolor='%23000000'%20borderopacity='0.25'%20inkscape:showpageshadow='2'%20inkscape:pageopacity='0.0'%20inkscape:pagecheckerboard='0'%20inkscape:deskcolor='%23d1d1d1'%20inkscape:zoom='7.375'%20inkscape:cx='15.932203'%20inkscape:cy='16'%20inkscape:window-width='1312'%20inkscape:window-height='529'%20inkscape:window-x='0'%20inkscape:window-y='38'%20inkscape:window-maximized='0'%20inkscape:current-layer='svg7'%20/%3e%3cdefs%20id='defs6'%3e%3cclipPath%20id='clipPath1'%3e%3cpath%20d='m69.63%2012.145h-.052c-22.727-.292-46.47%204.077-46.709%204.122-2.424.451-4.946%202.974-5.397%205.397-.044.237-4.414%2023.983-4.122%2046.71-.292%2022.777%204.078%2046.523%204.122%2046.761.451%202.423%202.974%204.945%205.398%205.398.237.044%2023.982%204.413%2046.709%204.121%2022.779.292%2046.524-4.077%2046.761-4.121%202.423-.452%204.946-2.976%205.398-5.399.044-.236%204.413-23.981%204.121-46.709.292-22.777-4.077-46.523-4.121-46.761-.453-2.423-2.976-4.946-5.398-5.397-.238-.045-23.984-4.414-46.71-4.122'%20id='path1'%20/%3e%3c/clipPath%3e%3clinearGradient%20gradientUnits='userSpaceOnUse'%20y2='352.98'%20x2='-601.15'%20y1='663.95'%20x1='-591.02'%20id='2'%3e%3cstop%20stop-color='%23a0a0a0'%20id='stop1'%20/%3e%3cstop%20offset='1'%20stop-color='%23aaa'%20id='stop2'%20/%3e%3c/linearGradient%3e%3clinearGradient%20gradientUnits='userSpaceOnUse'%20y2='354.29'%20x2='-704.05'%20y1='647.77'%20x1='-701.19'%20id='1'%3e%3cstop%20stop-color='%23acabab'%20id='stop3'%20/%3e%3cstop%20offset='1'%20stop-color='%23d4d4d4'%20id='stop4'%20/%3e%3c/linearGradient%3e%3clinearGradient%20id='0'%20x1='59.12'%20y1='-19.888'%20x2='59.15'%20y2='-37.783'%20gradientUnits='userSpaceOnUse'%20gradientTransform='matrix(4.17478%200%200%204.16765-1069.7%20447.73)'%3e%3cstop%20stop-color='%23a0a0a0'%20id='stop5'%20/%3e%3cstop%20offset='1'%20stop-color='%23bdbdbd'%20id='stop6'%20/%3e%3c/linearGradient%3e%3c/defs%3e%3cg%20transform='matrix(.07089%200%200%20.07017%2023.295-40.67)'%20fill='%2360aae5'%20id='g7'%20style='fill:%23888888;fill-opacity:1'%3e%3cpath%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20d='m-884.1%20294.78c-4.626%200-8.349%203.718-8.349%208.335v161.41l468.19%201v-121.2c0-4.618-3.724-8.335-8.35-8.335h-272.65c-8.51.751-9.607-.377-13.812-5.981-5.964-7.968-14.969-21.443-20.84-29.21-4.712-6.805-5.477-6.02-13.292-6.02z'%20fill='url(%230)'%20color='%23000'%20id='path6'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3crect%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20y='356.85'%20x='-890.28'%20height='295.13'%20width='463.85'%20fill='url(%231)'%20stroke='url(%231)'%20stroke-width='2.378'%20rx='9.63'%20id='rect6'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3crect%20width='463.85'%20height='295.13'%20x='-890.28'%20y='356.85'%20transform='matrix(.7872%200%200%20.79524%20415.34%20430.11)'%20fill='none'%20stroke='url(%232)'%20stroke-linejoin='round'%20stroke-linecap='round'%20stroke-width='5.376'%20rx='9.63'%20id='rect7'%20style='fill:%23888888;fill-opacity:1'%20/%3e%3c/g%3e%3c/svg%3e";
function get_each_context(ctx, list, i) {
  const child_ctx = ctx.slice();
  child_ctx[21] = list[i].type;
  child_ctx[22] = list[i].name;
  child_ctx[23] = list[i].valid;
  child_ctx[25] = i;
  return child_ctx;
}
function create_else_block_1(ctx) {
  let checkbox;
  let current;
  function func(...args) {
    return (
      /*func*/
      ctx[13](
        /*name*/
        ctx[22],
        ...args
      )
    );
  }
  function change_handler(...args) {
    return (
      /*change_handler*/
      ctx[14](
        /*name*/
        ctx[22],
        /*type*/
        ctx[21],
        /*i*/
        ctx[25],
        ...args
      )
    );
  }
  checkbox = new Checkbox({
    props: {
      disabled: !/*interactive*/
      ctx[3],
      value: (
        /*type*/
        (ctx[21] === "file" ? (
          /*selected_files*/
          ctx[1]
        ) : (
          /*selected_folders*/
          ctx[2]
        )).some(func)
      )
    }
  });
  checkbox.$on("change", change_handler);
  return {
    c() {
      create_component(checkbox.$$.fragment);
    },
    l(nodes) {
      claim_component(checkbox.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(checkbox, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const checkbox_changes = {};
      if (dirty & /*interactive*/
      8)
        checkbox_changes.disabled = !/*interactive*/
        ctx[3];
      if (dirty & /*content, selected_files, selected_folders*/
      70)
        checkbox_changes.value = /*type*/
        (ctx[21] === "file" ? (
          /*selected_files*/
          ctx[1]
        ) : (
          /*selected_folders*/
          ctx[2]
        )).some(func);
      checkbox.$set(checkbox_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(checkbox.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(checkbox.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(checkbox, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let span;
  return {
    c() {
      span = element("span");
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true, "aria-hidden": true });
      children(span).forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "no-checkbox svelte-p1d4ff");
      attr(span, "aria-hidden", "true");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(span);
      }
    }
  };
}
function create_else_block(ctx) {
  let span;
  let img;
  let img_src_value;
  return {
    c() {
      span = element("span");
      img = element("img");
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      img = claim_element(span_nodes, "IMG", { src: true, alt: true, class: true });
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      if (!src_url_equal(img.src, img_src_value = /*name*/
      ctx[22] === "." ? FolderIcon : FileIcon))
        attr(img, "src", img_src_value);
      attr(img, "alt", "file icon");
      attr(img, "class", "svelte-p1d4ff");
      attr(span, "class", "file-icon svelte-p1d4ff");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, img);
    },
    p(ctx2, dirty) {
      if (dirty & /*content*/
      64 && !src_url_equal(img.src, img_src_value = /*name*/
      ctx2[22] === "." ? FolderIcon : FileIcon)) {
        attr(img, "src", img_src_value);
      }
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
function create_if_block_1(ctx) {
  let span;
  let arrow;
  let current;
  let mounted;
  let dispose;
  arrow = new ArrowIcon({});
  function click_handler() {
    return (
      /*click_handler*/
      ctx[15](
        /*i*/
        ctx[25]
      )
    );
  }
  function keydown_handler(...args) {
    return (
      /*keydown_handler*/
      ctx[16](
        /*i*/
        ctx[25],
        ...args
      )
    );
  }
  return {
    c() {
      span = element("span");
      create_component(arrow.$$.fragment);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", {
        class: true,
        role: true,
        "aria-label": true,
        tabindex: true
      });
      var span_nodes = children(span);
      claim_component(arrow.$$.fragment, span_nodes);
      span_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "icon svelte-p1d4ff");
      attr(span, "role", "button");
      attr(span, "aria-label", "expand directory");
      attr(span, "tabindex", "0");
      toggle_class(span, "hidden", !/*opened_folders*/
      ctx[7].includes(
        /*i*/
        ctx[25]
      ));
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      mount_component(arrow, span, null);
      current = true;
      if (!mounted) {
        dispose = [
          listen(span, "click", stop_propagation(click_handler)),
          listen(span, "keydown", keydown_handler)
        ];
        mounted = true;
      }
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      if (!current || dirty & /*opened_folders*/
      128) {
        toggle_class(span, "hidden", !/*opened_folders*/
        ctx[7].includes(
          /*i*/
          ctx[25]
        ));
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(arrow.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(arrow.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(span);
      }
      destroy_component(arrow);
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block(ctx) {
  let filetree;
  let current;
  function func_1(...args) {
    return (
      /*func_1*/
      ctx[17](
        /*name*/
        ctx[22],
        ...args
      )
    );
  }
  function func_3(...args) {
    return (
      /*func_3*/
      ctx[18](
        /*name*/
        ctx[22],
        ...args
      )
    );
  }
  function func_5(...args) {
    return (
      /*func_5*/
      ctx[19](
        /*name*/
        ctx[22],
        ...args
      )
    );
  }
  filetree = new FileTree({
    props: {
      path: [
        .../*path*/
        ctx[0],
        /*name*/
        ctx[22]
      ],
      selected_files: (
        /*selected_files*/
        ctx[1].filter(func_1).map(func_2)
      ),
      selected_folders: (
        /*selected_folders*/
        ctx[2].filter(func_3).map(func_4)
      ),
      is_selected_entirely: (
        /*selected_folders*/
        ctx[2].some(func_5)
      ),
      interactive: (
        /*interactive*/
        ctx[3]
      ),
      ls_fn: (
        /*ls_fn*/
        ctx[4]
      ),
      file_count: (
        /*file_count*/
        ctx[5]
      ),
      valid_for_selection: (
        /*valid*/
        ctx[23]
      )
    }
  });
  filetree.$on(
    "check",
    /*check_handler*/
    ctx[20]
  );
  return {
    c() {
      create_component(filetree.$$.fragment);
    },
    l(nodes) {
      claim_component(filetree.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(filetree, target, anchor);
      current = true;
    },
    p(new_ctx, dirty) {
      ctx = new_ctx;
      const filetree_changes = {};
      if (dirty & /*path, content*/
      65)
        filetree_changes.path = [
          .../*path*/
          ctx[0],
          /*name*/
          ctx[22]
        ];
      if (dirty & /*selected_files, content*/
      66)
        filetree_changes.selected_files = /*selected_files*/
        ctx[1].filter(func_1).map(func_2);
      if (dirty & /*selected_folders, content*/
      68)
        filetree_changes.selected_folders = /*selected_folders*/
        ctx[2].filter(func_3).map(func_4);
      if (dirty & /*selected_folders, content*/
      68)
        filetree_changes.is_selected_entirely = /*selected_folders*/
        ctx[2].some(func_5);
      if (dirty & /*interactive*/
      8)
        filetree_changes.interactive = /*interactive*/
        ctx[3];
      if (dirty & /*ls_fn*/
      16)
        filetree_changes.ls_fn = /*ls_fn*/
        ctx[4];
      if (dirty & /*file_count*/
      32)
        filetree_changes.file_count = /*file_count*/
        ctx[5];
      if (dirty & /*content*/
      64)
        filetree_changes.valid_for_selection = /*valid*/
        ctx[23];
      filetree.$set(filetree_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(filetree.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(filetree.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(filetree, detaching);
    }
  };
}
function create_each_block(ctx) {
  let li;
  let span;
  let current_block_type_index;
  let if_block0;
  let t0;
  let current_block_type_index_1;
  let if_block1;
  let t1;
  let t2_value = (
    /*name*/
    ctx[22] + ""
  );
  let t2;
  let t3;
  let show_if = (
    /*type*/
    ctx[21] === "folder" && /*opened_folders*/
    ctx[7].includes(
      /*i*/
      ctx[25]
    )
  );
  let t4;
  let current;
  const if_block_creators = [create_if_block_2, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*type*/
      ctx2[21] === "folder" && /*file_count*/
      ctx2[5] === "single"
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  const if_block_creators_1 = [create_if_block_1, create_else_block];
  const if_blocks_1 = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*type*/
      ctx2[21] === "folder"
    )
      return 0;
    return 1;
  }
  current_block_type_index_1 = select_block_type_1(ctx);
  if_block1 = if_blocks_1[current_block_type_index_1] = if_block_creators_1[current_block_type_index_1](ctx);
  let if_block2 = show_if && create_if_block(ctx);
  return {
    c() {
      li = element("li");
      span = element("span");
      if_block0.c();
      t0 = space();
      if_block1.c();
      t1 = space();
      t2 = text(t2_value);
      t3 = space();
      if (if_block2)
        if_block2.c();
      t4 = space();
      this.h();
    },
    l(nodes) {
      li = claim_element(nodes, "LI", { class: true });
      var li_nodes = children(li);
      span = claim_element(li_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      if_block0.l(span_nodes);
      t0 = claim_space(span_nodes);
      if_block1.l(span_nodes);
      t1 = claim_space(span_nodes);
      t2 = claim_text(span_nodes, t2_value);
      span_nodes.forEach(detach);
      t3 = claim_space(li_nodes);
      if (if_block2)
        if_block2.l(li_nodes);
      t4 = claim_space(li_nodes);
      li_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "wrap svelte-p1d4ff");
      attr(li, "class", "svelte-p1d4ff");
    },
    m(target, anchor) {
      insert_hydration(target, li, anchor);
      append_hydration(li, span);
      if_blocks[current_block_type_index].m(span, null);
      append_hydration(span, t0);
      if_blocks_1[current_block_type_index_1].m(span, null);
      append_hydration(span, t1);
      append_hydration(span, t2);
      append_hydration(li, t3);
      if (if_block2)
        if_block2.m(li, null);
      append_hydration(li, t4);
      current = true;
    },
    p(ctx2, dirty) {
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
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block0.c();
        } else {
          if_block0.p(ctx2, dirty);
        }
        transition_in(if_block0, 1);
        if_block0.m(span, t0);
      }
      let previous_block_index_1 = current_block_type_index_1;
      current_block_type_index_1 = select_block_type_1(ctx2);
      if (current_block_type_index_1 === previous_block_index_1) {
        if_blocks_1[current_block_type_index_1].p(ctx2, dirty);
      } else {
        group_outros();
        transition_out(if_blocks_1[previous_block_index_1], 1, 1, () => {
          if_blocks_1[previous_block_index_1] = null;
        });
        check_outros();
        if_block1 = if_blocks_1[current_block_type_index_1];
        if (!if_block1) {
          if_block1 = if_blocks_1[current_block_type_index_1] = if_block_creators_1[current_block_type_index_1](ctx2);
          if_block1.c();
        } else {
          if_block1.p(ctx2, dirty);
        }
        transition_in(if_block1, 1);
        if_block1.m(span, t1);
      }
      if ((!current || dirty & /*content*/
      64) && t2_value !== (t2_value = /*name*/
      ctx2[22] + ""))
        set_data(t2, t2_value);
      if (dirty & /*content, opened_folders*/
      192)
        show_if = /*type*/
        ctx2[21] === "folder" && /*opened_folders*/
        ctx2[7].includes(
          /*i*/
          ctx2[25]
        );
      if (show_if) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty & /*content, opened_folders*/
          192) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(li, t4);
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
        detach(li);
      }
      if_blocks[current_block_type_index].d();
      if_blocks_1[current_block_type_index_1].d();
      if (if_block2)
        if_block2.d();
    }
  };
}
function create_fragment$2(ctx) {
  let ul;
  let current;
  let each_value = ensure_array_like(
    /*content*/
    ctx[6]
  );
  let each_blocks = [];
  for (let i = 0; i < each_value.length; i += 1) {
    each_blocks[i] = create_each_block(get_each_context(ctx, each_value, i));
  }
  const out = (i) => transition_out(each_blocks[i], 1, 1, () => {
    each_blocks[i] = null;
  });
  return {
    c() {
      ul = element("ul");
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].c();
      }
      this.h();
    },
    l(nodes) {
      ul = claim_element(nodes, "UL", { class: true });
      var ul_nodes = children(ul);
      for (let i = 0; i < each_blocks.length; i += 1) {
        each_blocks[i].l(ul_nodes);
      }
      ul_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(ul, "class", "svelte-p1d4ff");
    },
    m(target, anchor) {
      insert_hydration(target, ul, anchor);
      for (let i = 0; i < each_blocks.length; i += 1) {
        if (each_blocks[i]) {
          each_blocks[i].m(ul, null);
        }
      }
      current = true;
    },
    p(ctx2, [dirty]) {
      if (dirty & /*path, content, selected_files, selected_folders, interactive, ls_fn, file_count, opened_folders, toggle_open_folder, dispatch, open_folder*/
      2047) {
        each_value = ensure_array_like(
          /*content*/
          ctx2[6]
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
            each_blocks[i].m(ul, null);
          }
        }
        group_outros();
        for (i = each_value.length; i < each_blocks.length; i += 1) {
          out(i);
        }
        check_outros();
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
      each_blocks = each_blocks.filter(Boolean);
      for (let i = 0; i < each_blocks.length; i += 1) {
        transition_out(each_blocks[i]);
      }
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(ul);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
const func_2 = (x) => x.slice(1);
const func_4 = (x) => x.slice(1);
function instance$2($$self, $$props, $$invalidate) {
  let { path = [] } = $$props;
  let { selected_files = [] } = $$props;
  let { selected_folders = [] } = $$props;
  let { is_selected_entirely = false } = $$props;
  let { interactive } = $$props;
  let { ls_fn } = $$props;
  let { file_count = "multiple" } = $$props;
  let { valid_for_selection } = $$props;
  let content = [];
  let opened_folders = [];
  const toggle_open_folder = (i) => {
    if (opened_folders.includes(i)) {
      $$invalidate(7, opened_folders = opened_folders.filter((x) => x !== i));
    } else {
      $$invalidate(7, opened_folders = [...opened_folders, i]);
    }
  };
  const open_folder = (i) => {
    if (!opened_folders.includes(i)) {
      $$invalidate(7, opened_folders = [...opened_folders, i]);
    }
  };
  (async () => {
    $$invalidate(6, content = await ls_fn(path));
    if (valid_for_selection) {
      $$invalidate(6, content = [{ name: ".", type: "file" }, ...content]);
    }
    $$invalidate(7, opened_folders = content.map((x, i) => x.type === "folder" && (is_selected_entirely || selected_files.some((y) => y[0] === x.name)) ? i : null).filter((x) => x !== null));
  })();
  const dispatch = createEventDispatcher();
  const func = (name, x) => x[0] === name && x.length === 1;
  const change_handler = (name, type, i, e) => {
    let checked = e.detail;
    dispatch("check", { path: [...path, name], checked, type });
    if (type === "folder" && checked) {
      open_folder(i);
    }
  };
  const click_handler = (i) => toggle_open_folder(i);
  const keydown_handler = (i, { key }) => {
    if (key === " " || key === "Enter") {
      toggle_open_folder(i);
    }
  };
  const func_1 = (name, x) => x[0] === name;
  const func_3 = (name, x) => x[0] === name;
  const func_5 = (name, x) => x[0] === name && x.length === 1;
  function check_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("path" in $$props2)
      $$invalidate(0, path = $$props2.path);
    if ("selected_files" in $$props2)
      $$invalidate(1, selected_files = $$props2.selected_files);
    if ("selected_folders" in $$props2)
      $$invalidate(2, selected_folders = $$props2.selected_folders);
    if ("is_selected_entirely" in $$props2)
      $$invalidate(11, is_selected_entirely = $$props2.is_selected_entirely);
    if ("interactive" in $$props2)
      $$invalidate(3, interactive = $$props2.interactive);
    if ("ls_fn" in $$props2)
      $$invalidate(4, ls_fn = $$props2.ls_fn);
    if ("file_count" in $$props2)
      $$invalidate(5, file_count = $$props2.file_count);
    if ("valid_for_selection" in $$props2)
      $$invalidate(12, valid_for_selection = $$props2.valid_for_selection);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*is_selected_entirely, content, path*/
    2113) {
      if (is_selected_entirely) {
        content.forEach((x) => {
          dispatch("check", {
            path: [...path, x.name],
            checked: true,
            type: x.type
          });
        });
      }
    }
  };
  return [
    path,
    selected_files,
    selected_folders,
    interactive,
    ls_fn,
    file_count,
    content,
    opened_folders,
    toggle_open_folder,
    open_folder,
    dispatch,
    is_selected_entirely,
    valid_for_selection,
    func,
    change_handler,
    click_handler,
    keydown_handler,
    func_1,
    func_3,
    func_5,
    check_handler
  ];
}
class FileTree extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      path: 0,
      selected_files: 1,
      selected_folders: 2,
      is_selected_entirely: 11,
      interactive: 3,
      ls_fn: 4,
      file_count: 5,
      valid_for_selection: 12
    });
  }
}
function create_fragment$1(ctx) {
  let div;
  let filetree;
  let current;
  filetree = new FileTree({
    props: {
      path: [],
      selected_files: (
        /*value*/
        ctx[0]
      ),
      selected_folders: (
        /*selected_folders*/
        ctx[4]
      ),
      interactive: (
        /*interactive*/
        ctx[1]
      ),
      ls_fn: (
        /*ls_fn*/
        ctx[3]
      ),
      file_count: (
        /*file_count*/
        ctx[2]
      ),
      valid_for_selection: false
    }
  });
  filetree.$on(
    "check",
    /*check_handler*/
    ctx[8]
  );
  return {
    c() {
      div = element("div");
      create_component(filetree.$$.fragment);
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(filetree.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "file-wrap svelte-dicskc");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      mount_component(filetree, div, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      const filetree_changes = {};
      if (dirty & /*value*/
      1)
        filetree_changes.selected_files = /*value*/
        ctx2[0];
      if (dirty & /*selected_folders*/
      16)
        filetree_changes.selected_folders = /*selected_folders*/
        ctx2[4];
      if (dirty & /*interactive*/
      2)
        filetree_changes.interactive = /*interactive*/
        ctx2[1];
      if (dirty & /*ls_fn*/
      8)
        filetree_changes.ls_fn = /*ls_fn*/
        ctx2[3];
      if (dirty & /*file_count*/
      4)
        filetree_changes.file_count = /*file_count*/
        ctx2[2];
      filetree.$set(filetree_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(filetree.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(filetree.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      destroy_component(filetree);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let { interactive } = $$props;
  let { file_count = "multiple" } = $$props;
  let { value = [] } = $$props;
  let { ls_fn } = $$props;
  let selected_folders = [];
  const paths_equal = (path, path_2) => {
    return path.join("/") === path_2.join("/");
  };
  const path_in_set = (path, set) => {
    return set.some((x) => paths_equal(x, path));
  };
  const path_inside = (path, path_2) => {
    return path.join("/").startsWith(path_2.join("/"));
  };
  const check_handler = (e) => {
    const { path, checked, type } = e.detail;
    if (checked) {
      if (file_count === "single") {
        $$invalidate(0, value = [path]);
      } else if (type === "folder") {
        if (!path_in_set(path, selected_folders)) {
          $$invalidate(4, selected_folders = [...selected_folders, path]);
        }
      } else {
        if (!path_in_set(path, value)) {
          $$invalidate(0, value = [...value, path]);
        }
      }
    } else {
      $$invalidate(4, selected_folders = selected_folders.filter((folder) => !path_inside(path, folder)));
      if (type === "folder") {
        $$invalidate(4, selected_folders = selected_folders.filter((folder) => !path_inside(folder, path)));
        $$invalidate(0, value = value.filter((file) => !path_inside(file, path)));
      } else {
        $$invalidate(0, value = value.filter((x) => !paths_equal(x, path)));
      }
    }
  };
  $$self.$$set = ($$props2) => {
    if ("interactive" in $$props2)
      $$invalidate(1, interactive = $$props2.interactive);
    if ("file_count" in $$props2)
      $$invalidate(2, file_count = $$props2.file_count);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("ls_fn" in $$props2)
      $$invalidate(3, ls_fn = $$props2.ls_fn);
  };
  return [
    value,
    interactive,
    file_count,
    ls_fn,
    selected_folders,
    paths_equal,
    path_in_set,
    path_inside,
    check_handler
  ];
}
class DirectoryExplorer extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$1, create_fragment$1, safe_not_equal, {
      interactive: 1,
      file_count: 2,
      value: 0,
      ls_fn: 3
    });
  }
}
function create_key_block(ctx) {
  let directoryexplorer;
  let updating_value;
  let current;
  function directoryexplorer_value_binding(value) {
    ctx[23](value);
  }
  let directoryexplorer_props = {
    file_count: (
      /*file_count*/
      ctx[9]
    ),
    interactive: (
      /*interactive*/
      ctx[16]
    ),
    ls_fn: (
      /*server*/
      ctx[15].ls
    )
  };
  if (
    /*value*/
    ctx[0] !== void 0
  ) {
    directoryexplorer_props.value = /*value*/
    ctx[0];
  }
  directoryexplorer = new DirectoryExplorer({ props: directoryexplorer_props });
  binding_callbacks.push(() => bind(directoryexplorer, "value", directoryexplorer_value_binding));
  return {
    c() {
      create_component(directoryexplorer.$$.fragment);
    },
    l(nodes) {
      claim_component(directoryexplorer.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(directoryexplorer, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const directoryexplorer_changes = {};
      if (dirty & /*file_count*/
      512)
        directoryexplorer_changes.file_count = /*file_count*/
        ctx2[9];
      if (dirty & /*interactive*/
      65536)
        directoryexplorer_changes.interactive = /*interactive*/
        ctx2[16];
      if (dirty & /*server*/
      32768)
        directoryexplorer_changes.ls_fn = /*server*/
        ctx2[15].ls;
      if (!updating_value && dirty & /*value*/
      1) {
        updating_value = true;
        directoryexplorer_changes.value = /*value*/
        ctx2[0];
        add_flush_callback(() => updating_value = false);
      }
      directoryexplorer.$set(directoryexplorer_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(directoryexplorer.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(directoryexplorer.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(directoryexplorer, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t0;
  let blocklabel;
  let t1;
  let previous_key = (
    /*rerender_key*/
    ctx[17]
  );
  let key_block_anchor;
  let current;
  const statustracker_spread_levels = [
    /*loading_status*/
    ctx[10],
    {
      autoscroll: (
        /*gradio*/
        ctx[14].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[14].i18n
    ) }
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[22]
  );
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[5]
      ),
      Icon: File,
      label: (
        /*label*/
        ctx[4] || "FileExplorer"
      ),
      float: false
    }
  });
  let key_block = create_key_block(ctx);
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t0 = space();
      create_component(blocklabel.$$.fragment);
      t1 = space();
      key_block.c();
      key_block_anchor = empty();
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t0 = claim_space(nodes);
      claim_component(blocklabel.$$.fragment, nodes);
      t1 = claim_space(nodes);
      key_block.l(nodes);
      key_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t0, anchor);
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t1, anchor);
      key_block.m(target, anchor);
      insert_hydration(target, key_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty & /*loading_status, gradio*/
      17408 ? get_spread_update(statustracker_spread_levels, [
        dirty & /*loading_status*/
        1024 && get_spread_object(
          /*loading_status*/
          ctx2[10]
        ),
        dirty & /*gradio*/
        16384 && {
          autoscroll: (
            /*gradio*/
            ctx2[14].autoscroll
          )
        },
        dirty & /*gradio*/
        16384 && { i18n: (
          /*gradio*/
          ctx2[14].i18n
        ) }
      ]) : {};
      statustracker.$set(statustracker_changes);
      const blocklabel_changes = {};
      if (dirty & /*show_label*/
      32)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[5];
      if (dirty & /*label*/
      16)
        blocklabel_changes.label = /*label*/
        ctx2[4] || "FileExplorer";
      blocklabel.$set(blocklabel_changes);
      if (dirty & /*rerender_key*/
      131072 && safe_not_equal(previous_key, previous_key = /*rerender_key*/
      ctx2[17])) {
        group_outros();
        transition_out(key_block, 1, 1, noop);
        check_outros();
        key_block = create_key_block(ctx2);
        key_block.c();
        transition_in(key_block, 1);
        key_block.m(key_block_anchor.parentNode, key_block_anchor);
      } else {
        key_block.p(ctx2, dirty);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(blocklabel.$$.fragment, local);
      transition_in(key_block);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(blocklabel.$$.fragment, local);
      transition_out(key_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(key_block_anchor);
      }
      destroy_component(statustracker, detaching);
      destroy_component(blocklabel, detaching);
      key_block.d(detaching);
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
        ctx[3]
      ),
      variant: (
        /*value*/
        ctx[0] === null ? "dashed" : "solid"
      ),
      border_mode: "base",
      padding: false,
      elem_id: (
        /*elem_id*/
        ctx[1]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[2]
      ),
      container: (
        /*container*/
        ctx[11]
      ),
      scale: (
        /*scale*/
        ctx[12]
      ),
      min_width: (
        /*min_width*/
        ctx[13]
      ),
      allow_overflow: true,
      overflow_behavior: "auto",
      height: (
        /*height*/
        ctx[6]
      ),
      max_height: (
        /*max_height*/
        ctx[8]
      ),
      min_height: (
        /*min_height*/
        ctx[7]
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
      8)
        block_changes.visible = /*visible*/
        ctx2[3];
      if (dirty & /*value*/
      1)
        block_changes.variant = /*value*/
        ctx2[0] === null ? "dashed" : "solid";
      if (dirty & /*elem_id*/
      2)
        block_changes.elem_id = /*elem_id*/
        ctx2[1];
      if (dirty & /*elem_classes*/
      4)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[2];
      if (dirty & /*container*/
      2048)
        block_changes.container = /*container*/
        ctx2[11];
      if (dirty & /*scale*/
      4096)
        block_changes.scale = /*scale*/
        ctx2[12];
      if (dirty & /*min_width*/
      8192)
        block_changes.min_width = /*min_width*/
        ctx2[13];
      if (dirty & /*height*/
      64)
        block_changes.height = /*height*/
        ctx2[6];
      if (dirty & /*max_height*/
      256)
        block_changes.max_height = /*max_height*/
        ctx2[8];
      if (dirty & /*min_height*/
      128)
        block_changes.min_height = /*min_height*/
        ctx2[7];
      if (dirty & /*$$scope, rerender_key, file_count, interactive, server, value, show_label, label, loading_status, gradio*/
      17024561) {
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
  let rerender_key;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { value } = $$props;
  let old_value;
  let { label } = $$props;
  let { show_label } = $$props;
  let { height } = $$props;
  let { min_height } = $$props;
  let { max_height } = $$props;
  let { file_count = "multiple" } = $$props;
  let { root_dir } = $$props;
  let { glob } = $$props;
  let { ignore_glob } = $$props;
  let { loading_status } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { gradio } = $$props;
  let { server } = $$props;
  let { interactive } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function directoryexplorer_value_binding(value$1) {
    value = value$1;
    $$invalidate(0, value);
  }
  $$self.$$set = ($$props2) => {
    if ("elem_id" in $$props2)
      $$invalidate(1, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(2, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(3, visible = $$props2.visible);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(5, show_label = $$props2.show_label);
    if ("height" in $$props2)
      $$invalidate(6, height = $$props2.height);
    if ("min_height" in $$props2)
      $$invalidate(7, min_height = $$props2.min_height);
    if ("max_height" in $$props2)
      $$invalidate(8, max_height = $$props2.max_height);
    if ("file_count" in $$props2)
      $$invalidate(9, file_count = $$props2.file_count);
    if ("root_dir" in $$props2)
      $$invalidate(18, root_dir = $$props2.root_dir);
    if ("glob" in $$props2)
      $$invalidate(19, glob = $$props2.glob);
    if ("ignore_glob" in $$props2)
      $$invalidate(20, ignore_glob = $$props2.ignore_glob);
    if ("loading_status" in $$props2)
      $$invalidate(10, loading_status = $$props2.loading_status);
    if ("container" in $$props2)
      $$invalidate(11, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(12, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(13, min_width = $$props2.min_width);
    if ("gradio" in $$props2)
      $$invalidate(14, gradio = $$props2.gradio);
    if ("server" in $$props2)
      $$invalidate(15, server = $$props2.server);
    if ("interactive" in $$props2)
      $$invalidate(16, interactive = $$props2.interactive);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*root_dir, glob, ignore_glob*/
    1835008) {
      $$invalidate(17, rerender_key = [root_dir, glob, ignore_glob]);
    }
    if ($$self.$$.dirty & /*value, old_value, gradio*/
    2113537) {
      if (JSON.stringify(value) !== JSON.stringify(old_value)) {
        $$invalidate(21, old_value = value);
        gradio.dispatch("change");
      }
    }
  };
  return [
    value,
    elem_id,
    elem_classes,
    visible,
    label,
    show_label,
    height,
    min_height,
    max_height,
    file_count,
    loading_status,
    container,
    scale,
    min_width,
    gradio,
    server,
    interactive,
    rerender_key,
    root_dir,
    glob,
    ignore_glob,
    old_value,
    clear_status_handler,
    directoryexplorer_value_binding
  ];
}
class Index extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      elem_id: 1,
      elem_classes: 2,
      visible: 3,
      value: 0,
      label: 4,
      show_label: 5,
      height: 6,
      min_height: 7,
      max_height: 8,
      file_count: 9,
      root_dir: 18,
      glob: 19,
      ignore_glob: 20,
      loading_status: 10,
      container: 11,
      scale: 12,
      min_width: 13,
      gradio: 14,
      server: 15,
      interactive: 16
    });
  }
  get elem_id() {
    return this.$$.ctx[1];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[2];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[3];
  }
  set visible(visible) {
    this.$$set({ visible });
    flush();
  }
  get value() {
    return this.$$.ctx[0];
  }
  set value(value) {
    this.$$set({ value });
    flush();
  }
  get label() {
    return this.$$.ctx[4];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get show_label() {
    return this.$$.ctx[5];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get height() {
    return this.$$.ctx[6];
  }
  set height(height) {
    this.$$set({ height });
    flush();
  }
  get min_height() {
    return this.$$.ctx[7];
  }
  set min_height(min_height) {
    this.$$set({ min_height });
    flush();
  }
  get max_height() {
    return this.$$.ctx[8];
  }
  set max_height(max_height) {
    this.$$set({ max_height });
    flush();
  }
  get file_count() {
    return this.$$.ctx[9];
  }
  set file_count(file_count) {
    this.$$set({ file_count });
    flush();
  }
  get root_dir() {
    return this.$$.ctx[18];
  }
  set root_dir(root_dir) {
    this.$$set({ root_dir });
    flush();
  }
  get glob() {
    return this.$$.ctx[19];
  }
  set glob(glob) {
    this.$$set({ glob });
    flush();
  }
  get ignore_glob() {
    return this.$$.ctx[20];
  }
  set ignore_glob(ignore_glob) {
    this.$$set({ ignore_glob });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[10];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get container() {
    return this.$$.ctx[11];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[12];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[13];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get gradio() {
    return this.$$.ctx[14];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
  get server() {
    return this.$$.ctx[15];
  }
  set server(server) {
    this.$$set({ server });
    flush();
  }
  get interactive() {
    return this.$$.ctx[16];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
}
export {
  Index as default
};
