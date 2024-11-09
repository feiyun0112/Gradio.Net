const __vite__fileDeps=["./vega-embed.module.zbmVXw6j.js","./2.BqWhUxOo.js","./preload-helper.DpQnamwV.js","./stores.DcWgXC6T.js","./client.DB6ownDU.js","../assets/2.DoS7Rli5.css","./dsv.CKxpOsHB.js"],__vite__mapDeps=i=>i.map(i=>__vite__fileDeps[i]);
import { _ as __vitePreload } from "./preload-helper.DpQnamwV.js";
import { SvelteComponent, init, safe_not_equal, create_component, claim_component, mount_component, transition_in, transition_out, destroy_component, onMount, space, empty, claim_space, insert_hydration, group_outros, check_outros, detach, binding_callbacks, assign, get_spread_update, get_spread_object, text, claim_text, set_data, element, claim_element, children, attr, noop, append_hydration } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { B as Block, f as BlockTitle, S as Static } from "./2.BqWhUxOo.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { L as LineChart } from "./LineChart.D-OPS8mj.js";
function create_if_block_2(ctx) {
  let statustracker;
  let current;
  const statustracker_spread_levels = [
    { autoscroll: (
      /*gradio*/
      ctx[3].autoscroll
    ) },
    { i18n: (
      /*gradio*/
      ctx[3].i18n
    ) },
    /*loading_status*/
    ctx[11]
  ];
  let statustracker_props = {};
  for (let i = 0; i < statustracker_spread_levels.length; i += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[40]
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
      2056 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        8 && { autoscroll: (
          /*gradio*/
          ctx2[3].autoscroll
        ) },
        dirty[0] & /*gradio*/
        8 && { i18n: (
          /*gradio*/
          ctx2[3].i18n
        ) },
        dirty[0] & /*loading_status*/
        2048 && get_spread_object(
          /*loading_status*/
          ctx2[11]
        )
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
function create_default_slot_2(ctx) {
  let t;
  return {
    c() {
      t = text(
        /*label*/
        ctx[4]
      );
    },
    l(nodes) {
      t = claim_text(
        nodes,
        /*label*/
        ctx[4]
      );
    },
    m(target, anchor) {
      insert_hydration(target, t, anchor);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*label*/
      16)
        set_data(
          t,
          /*label*/
          ctx2[4]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(t);
      }
    }
  };
}
function create_else_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      $$slots: { default: [create_default_slot_1] },
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
      if (dirty[1] & /*$$scope*/
      33554432) {
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
function create_if_block(ctx) {
  let div;
  let t;
  let if_block_anchor;
  let if_block = (
    /*caption*/
    ctx[2] && create_if_block_1(ctx)
  );
  return {
    c() {
      div = element("div");
      t = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      children(div).forEach(detach);
      t = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(div, "class", "svelte-10k9m4v");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      ctx[41](div);
      insert_hydration(target, t, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (
        /*caption*/
        ctx2[2]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_1(ctx2);
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(div);
        detach(t);
        detach(if_block_anchor);
      }
      ctx[41](null);
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let labelicon;
  let current;
  labelicon = new LineChart({});
  return {
    c() {
      create_component(labelicon.$$.fragment);
    },
    l(nodes) {
      claim_component(labelicon.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(labelicon, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(labelicon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(labelicon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(labelicon, detaching);
    }
  };
}
function create_if_block_1(ctx) {
  let p;
  let t;
  return {
    c() {
      p = element("p");
      t = text(
        /*caption*/
        ctx[2]
      );
      this.h();
    },
    l(nodes) {
      p = claim_element(nodes, "P", { class: true });
      var p_nodes = children(p);
      t = claim_text(
        p_nodes,
        /*caption*/
        ctx[2]
      );
      p_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(p, "class", "caption svelte-10k9m4v");
    },
    m(target, anchor) {
      insert_hydration(target, p, anchor);
      append_hydration(p, t);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*caption*/
      4)
        set_data(
          t,
          /*caption*/
          ctx2[2]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(p);
      }
    }
  };
}
function create_default_slot(ctx) {
  let t0;
  let blocktitle;
  let t1;
  let current_block_type_index;
  let if_block1;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*loading_status*/
    ctx[11] && create_if_block_2(ctx)
  );
  blocktitle = new BlockTitle({
    props: {
      root: (
        /*root*/
        ctx[1]
      ),
      show_label: (
        /*show_label*/
        ctx[8]
      ),
      info: void 0,
      $$slots: { default: [create_default_slot_2] },
      $$scope: { ctx }
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] && /*is_browser*/
      ctx2[14]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t0 = space();
      create_component(blocktitle.$$.fragment);
      t1 = space();
      if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t0 = claim_space(nodes);
      claim_component(blocktitle.$$.fragment, nodes);
      t1 = claim_space(nodes);
      if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t0, anchor);
      mount_component(blocktitle, target, anchor);
      insert_hydration(target, t1, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*loading_status*/
        ctx2[11]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*loading_status*/
          2048) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2(ctx2);
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
      const blocktitle_changes = {};
      if (dirty[0] & /*root*/
      2)
        blocktitle_changes.root = /*root*/
        ctx2[1];
      if (dirty[0] & /*show_label*/
      256)
        blocktitle_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*label*/
      16 | dirty[1] & /*$$scope*/
      33554432) {
        blocktitle_changes.$$scope = { dirty, ctx: ctx2 };
      }
      blocktitle.$set(blocktitle_changes);
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
        if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(blocktitle.$$.fragment, local);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(blocktitle.$$.fragment, local);
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
      destroy_component(blocktitle, detaching);
      if_blocks[current_block_type_index].d(detaching);
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
        ctx[7]
      ),
      elem_id: (
        /*elem_id*/
        ctx[5]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[6]
      ),
      scale: (
        /*scale*/
        ctx[9]
      ),
      min_width: (
        /*min_width*/
        ctx[10]
      ),
      allow_overflow: false,
      padding: true,
      height: (
        /*height*/
        ctx[12]
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
    p(ctx2, dirty) {
      const block_changes = {};
      if (dirty[0] & /*visible*/
      128)
        block_changes.visible = /*visible*/
        ctx2[7];
      if (dirty[0] & /*elem_id*/
      32)
        block_changes.elem_id = /*elem_id*/
        ctx2[5];
      if (dirty[0] & /*elem_classes*/
      64)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[6];
      if (dirty[0] & /*scale*/
      512)
        block_changes.scale = /*scale*/
        ctx2[9];
      if (dirty[0] & /*min_width*/
      1024)
        block_changes.min_width = /*min_width*/
        ctx2[10];
      if (dirty[0] & /*height*/
      4096)
        block_changes.height = /*height*/
        ctx2[12];
      if (dirty[0] & /*caption, chart_element, value, root, show_label, label, gradio, loading_status*/
      10527 | dirty[1] & /*$$scope*/
      33554432) {
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
  let unique_colors;
  let _sort;
  let x_temporal;
  let _x_lim;
  let computed_style;
  let { value } = $$props;
  let { x } = $$props;
  let { y } = $$props;
  let { color = null } = $$props;
  let { root } = $$props;
  let { title = null } = $$props;
  let { x_title = null } = $$props;
  let { y_title = null } = $$props;
  let { color_title = null } = $$props;
  let { x_bin = null } = $$props;
  let { y_aggregate = void 0 } = $$props;
  let { color_map = null } = $$props;
  let { x_lim = null } = $$props;
  let { y_lim = null } = $$props;
  let { x_label_angle = null } = $$props;
  let { y_label_angle = null } = $$props;
  let { x_axis_labels_visible = true } = $$props;
  let { caption = null } = $$props;
  let { sort = null } = $$props;
  let { tooltip = "axis" } = $$props;
  function reformat_sort(_sort2) {
    if (_sort2 === "x") {
      return "ascending";
    } else if (_sort2 === "-x") {
      return "descending";
    } else if (_sort2 === "y") {
      return { field: y, order: "ascending" };
    } else if (_sort2 === "-y") {
      return { field: y, order: "descending" };
    } else if (_sort2 === null) {
      return void 0;
    } else if (Array.isArray(_sort2)) {
      return _sort2;
    }
  }
  let { _selectable = false } = $$props;
  let _data;
  let { gradio } = $$props;
  let _x_bin;
  let mouse_down_on_chart = false;
  const SUFFIX_DURATION = { s: 1, m: 60, h: 60 * 60, d: 24 * 60 * 60 };
  let _y_aggregate;
  let aggregating;
  function reformat_data(data) {
    if (tooltip == "all" || Array.isArray(tooltip)) {
      return data.data.map((row) => {
        const obj = {};
        data.columns.forEach((col, i) => {
          obj[col] = row[i];
        });
        return obj;
      });
    }
    let x_index = data.columns.indexOf(x);
    let y_index = data.columns.indexOf(y);
    let color_index = color ? data.columns.indexOf(color) : null;
    return data.data.map((row) => {
      const obj = { [x]: row[x_index], [y]: row[y_index] };
      if (color && color_index !== null) {
        obj[color] = row[color_index];
      }
      return obj;
    });
  }
  const is_browser = typeof window !== "undefined";
  let chart_element;
  let view;
  let mounted = false;
  let old_width;
  let resizeObserver;
  let vegaEmbed;
  async function load_chart() {
    if (view) {
      view.finalize();
    }
    if (!value || !chart_element)
      return;
    old_width = chart_element.offsetWidth;
    const spec = create_vega_lite_spec();
    if (!spec)
      return;
    resizeObserver = new ResizeObserver((el) => {
      if (!el[0].target || !(el[0].target instanceof HTMLElement))
        return;
      if (old_width === 0 && chart_element.offsetWidth !== 0 && value.datatypes[x] === "nominal") {
        load_chart();
      } else {
        view.signal("width", el[0].target.offsetWidth).run();
      }
    });
    if (!vegaEmbed) {
      vegaEmbed = (await __vitePreload(() => import("./vega-embed.module.zbmVXw6j.js"), true ? __vite__mapDeps([0,1,2,3,4,5,6]) : void 0, import.meta.url)).default;
    }
    vegaEmbed(chart_element, spec, { actions: false }).then(function(result) {
      view = result.view;
      resizeObserver.observe(chart_element);
      var debounceTimeout;
      view.addEventListener("dblclick", () => {
        gradio.dispatch("double_click");
      });
      chart_element.addEventListener(
        "mousedown",
        function(e) {
          if (e.detail > 1) {
            e.preventDefault();
          }
        },
        false
      );
      if (_selectable) {
        view.addSignalListener("brush", function(_, value2) {
          if (Object.keys(value2).length === 0)
            return;
          clearTimeout(debounceTimeout);
          let range = value2[Object.keys(value2)[0]];
          if (x_temporal) {
            range = [range[0] / 1e3, range[1] / 1e3];
          }
          let callback = () => {
            gradio.dispatch("select", {
              value: range,
              index: range,
              selected: true
            });
          };
          if (mouse_down_on_chart) {
            release_callback = callback;
          } else {
            debounceTimeout = setTimeout(
              function() {
                gradio.dispatch("select", {
                  value: range,
                  index: range,
                  selected: true
                });
              },
              250
            );
          }
        });
      }
    });
  }
  let release_callback = null;
  onMount(() => {
    $$invalidate(37, mounted = true);
    chart_element.addEventListener("mousedown", () => {
      mouse_down_on_chart = true;
    });
    chart_element.addEventListener("mouseup", () => {
      mouse_down_on_chart = false;
      if (release_callback) {
        release_callback();
        release_callback = null;
      }
    });
    return () => {
      $$invalidate(37, mounted = false);
      if (view) {
        view.finalize();
      }
      if (resizeObserver) {
        resizeObserver.disconnect();
      }
    };
  });
  function create_vega_lite_spec() {
    if (!value || !computed_style)
      return null;
    let accent_color = computed_style.getPropertyValue("--color-accent");
    let body_text_color = computed_style.getPropertyValue("--body-text-color");
    let borderColorPrimary = computed_style.getPropertyValue("--border-color-primary");
    let font_family = computed_style.fontFamily;
    let title_weight = computed_style.getPropertyValue("--block-title-text-weight");
    const font_to_px_val = (font) => {
      return font.endsWith("px") ? parseFloat(font.slice(0, -2)) : 12;
    };
    let text_size_md = font_to_px_val(computed_style.getPropertyValue("--text-md"));
    let text_size_sm = font_to_px_val(computed_style.getPropertyValue("--text-sm"));
    return {
      $schema: "https://vega.github.io/schema/vega-lite/v5.17.0.json",
      background: "transparent",
      config: {
        autosize: { type: "fit", contains: "padding" },
        axis: {
          labelFont: font_family,
          labelColor: body_text_color,
          titleFont: font_family,
          titleColor: body_text_color,
          titlePadding: 8,
          tickColor: borderColorPrimary,
          labelFontSize: text_size_sm,
          gridColor: borderColorPrimary,
          titleFontWeight: "normal",
          titleFontSize: text_size_sm,
          labelFontWeight: "normal",
          domain: false,
          labelAngle: 0
        },
        legend: {
          labelColor: body_text_color,
          labelFont: font_family,
          titleColor: body_text_color,
          titleFont: font_family,
          titleFontWeight: "normal",
          titleFontSize: text_size_sm,
          labelFontWeight: "normal",
          offset: 2
        },
        title: {
          color: body_text_color,
          font: font_family,
          fontSize: text_size_md,
          fontWeight: title_weight,
          anchor: "middle"
        },
        view: { stroke: borderColorPrimary },
        mark: {
          stroke: value.mark !== "bar" ? accent_color : void 0,
          fill: value.mark === "bar" ? accent_color : void 0,
          cursor: "crosshair"
        }
      },
      data: { name: "data" },
      datasets: { data: _data },
      layer: ["plot", ...value.mark === "line" ? ["hover"] : []].map((mode) => {
        return {
          encoding: {
            size: value.mark === "line" ? mode == "plot" ? {
              condition: {
                empty: false,
                param: "hoverPlot",
                value: 3
              },
              value: 2
            } : {
              condition: { empty: false, param: "hover", value: 100 },
              value: 0
            } : void 0,
            opacity: mode === "plot" ? void 0 : {
              condition: { empty: false, param: "hover", value: 1 },
              value: 0
            },
            x: {
              axis: {
                ...x_label_angle !== null && { labelAngle: x_label_angle },
                labels: x_axis_labels_visible,
                ticks: x_axis_labels_visible
              },
              field: x,
              title: x_title || x,
              type: value.datatypes[x],
              scale: _x_lim ? { domain: _x_lim } : void 0,
              bin: _x_bin ? { step: _x_bin } : void 0,
              sort: _sort
            },
            y: {
              axis: y_label_angle ? { labelAngle: y_label_angle } : {},
              field: y,
              title: y_title || y,
              type: value.datatypes[y],
              scale: y_lim ? { domain: y_lim } : void 0,
              aggregate: aggregating ? _y_aggregate : void 0
            },
            color: color ? {
              field: color,
              legend: { orient: "bottom", title: color_title },
              scale: value.datatypes[color] === "nominal" ? {
                domain: unique_colors,
                range: color_map ? unique_colors.map((c) => color_map[c]) : void 0
              } : {
                range: [100, 200, 300, 400, 500, 600, 700, 800, 900].map((n) => computed_style.getPropertyValue("--primary-" + n)),
                interpolate: "hsl"
              },
              type: value.datatypes[color]
            } : void 0,
            tooltip: tooltip == "none" ? void 0 : [
              {
                field: y,
                type: value.datatypes[y],
                aggregate: aggregating ? _y_aggregate : void 0,
                title: y_title || y
              },
              {
                field: x,
                type: value.datatypes[x],
                title: x_title || x,
                format: x_temporal ? "%Y-%m-%d %H:%M:%S" : void 0,
                bin: _x_bin ? { step: _x_bin } : void 0
              },
              ...color ? [
                {
                  field: color,
                  type: value.datatypes[color]
                }
              ] : [],
              ...tooltip === "axis" ? [] : value == null ? void 0 : value.columns.filter((col) => col !== x && col !== y && col !== color && (tooltip === "all" || tooltip.includes(col))).map((column) => ({
                field: column,
                type: value.datatypes[column]
              }))
            ]
          },
          strokeDash: {},
          mark: {
            clip: true,
            type: mode === "hover" ? "point" : value.mark
          },
          name: mode
        };
      }),
      // @ts-ignore
      params: [
        ...value.mark === "line" ? [
          {
            name: "hoverPlot",
            select: {
              clear: "mouseout",
              fields: color ? [color] : [],
              nearest: true,
              on: "mouseover",
              type: "point"
            },
            views: ["hover"]
          },
          {
            name: "hover",
            select: {
              clear: "mouseout",
              nearest: true,
              on: "mouseover",
              type: "point"
            },
            views: ["hover"]
          }
        ] : [],
        ..._selectable ? [
          {
            name: "brush",
            select: {
              encodings: ["x"],
              mark: {
                fill: "gray",
                fillOpacity: 0.3,
                stroke: "none"
              },
              type: "interval"
            },
            views: ["plot"]
          }
        ] : []
      ],
      width: chart_element.offsetWidth,
      title: title || void 0
    };
  }
  let { label = "Textbox" } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { show_label } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status = void 0 } = $$props;
  let { height = void 0 } = $$props;
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      chart_element = $$value;
      $$invalidate(13, chart_element);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("x" in $$props2)
      $$invalidate(15, x = $$props2.x);
    if ("y" in $$props2)
      $$invalidate(16, y = $$props2.y);
    if ("color" in $$props2)
      $$invalidate(17, color = $$props2.color);
    if ("root" in $$props2)
      $$invalidate(1, root = $$props2.root);
    if ("title" in $$props2)
      $$invalidate(18, title = $$props2.title);
    if ("x_title" in $$props2)
      $$invalidate(19, x_title = $$props2.x_title);
    if ("y_title" in $$props2)
      $$invalidate(20, y_title = $$props2.y_title);
    if ("color_title" in $$props2)
      $$invalidate(21, color_title = $$props2.color_title);
    if ("x_bin" in $$props2)
      $$invalidate(22, x_bin = $$props2.x_bin);
    if ("y_aggregate" in $$props2)
      $$invalidate(23, y_aggregate = $$props2.y_aggregate);
    if ("color_map" in $$props2)
      $$invalidate(24, color_map = $$props2.color_map);
    if ("x_lim" in $$props2)
      $$invalidate(25, x_lim = $$props2.x_lim);
    if ("y_lim" in $$props2)
      $$invalidate(26, y_lim = $$props2.y_lim);
    if ("x_label_angle" in $$props2)
      $$invalidate(27, x_label_angle = $$props2.x_label_angle);
    if ("y_label_angle" in $$props2)
      $$invalidate(28, y_label_angle = $$props2.y_label_angle);
    if ("x_axis_labels_visible" in $$props2)
      $$invalidate(29, x_axis_labels_visible = $$props2.x_axis_labels_visible);
    if ("caption" in $$props2)
      $$invalidate(2, caption = $$props2.caption);
    if ("sort" in $$props2)
      $$invalidate(30, sort = $$props2.sort);
    if ("tooltip" in $$props2)
      $$invalidate(31, tooltip = $$props2.tooltip);
    if ("_selectable" in $$props2)
      $$invalidate(32, _selectable = $$props2._selectable);
    if ("gradio" in $$props2)
      $$invalidate(3, gradio = $$props2.gradio);
    if ("label" in $$props2)
      $$invalidate(4, label = $$props2.label);
    if ("elem_id" in $$props2)
      $$invalidate(5, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(6, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(7, visible = $$props2.visible);
    if ("show_label" in $$props2)
      $$invalidate(8, show_label = $$props2.show_label);
    if ("scale" in $$props2)
      $$invalidate(9, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(10, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(11, loading_status = $$props2.loading_status);
    if ("height" in $$props2)
      $$invalidate(12, height = $$props2.height);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value*/
    1) {
      $$invalidate(33, _data = value ? reformat_data(value) : []);
    }
    if ($$self.$$.dirty[0] & /*color, value*/
    131073 | $$self.$$.dirty[1] & /*_data*/
    4) {
      unique_colors = color && value && value.datatypes[color] === "nominal" ? Array.from(new Set(_data.map((d) => d[color]))) : [];
    }
    if ($$self.$$.dirty[0] & /*sort*/
    1073741824) {
      _sort = reformat_sort(sort);
    }
    if ($$self.$$.dirty[0] & /*value, x*/
    32769) {
      $$invalidate(38, x_temporal = value && value.datatypes[x] === "temporal");
    }
    if ($$self.$$.dirty[0] & /*x_lim*/
    33554432 | $$self.$$.dirty[1] & /*x_temporal*/
    128) {
      _x_lim = x_lim && x_temporal ? [x_lim[0] * 1e3, x_lim[1] * 1e3] : x_lim;
    }
    if ($$self.$$.dirty[0] & /*x_bin*/
    4194304) {
      $$invalidate(34, _x_bin = x_bin ? typeof x_bin === "string" ? 1e3 * parseInt(x_bin.substring(0, x_bin.length - 1)) * SUFFIX_DURATION[x_bin[x_bin.length - 1]] : x_bin : void 0);
    }
    if ($$self.$$.dirty[0] & /*value, y_aggregate, x*/
    8421377 | $$self.$$.dirty[1] & /*_x_bin, aggregating*/
    40) {
      {
        if (value) {
          if (value.mark === "point") {
            $$invalidate(36, aggregating = _x_bin !== void 0);
            $$invalidate(35, _y_aggregate = y_aggregate || aggregating ? "sum" : void 0);
          } else {
            $$invalidate(36, aggregating = _x_bin !== void 0 || value.datatypes[x] === "nominal");
            $$invalidate(35, _y_aggregate = y_aggregate ? y_aggregate : "sum");
          }
        }
      }
    }
    if ($$self.$$.dirty[0] & /*chart_element*/
    8192) {
      $$invalidate(39, computed_style = chart_element ? window.getComputedStyle(chart_element) : null);
    }
    if ($$self.$$.dirty[0] & /*title, x_title, y_title, color_title, x, y, color, x_bin, color_map, x_lim, y_lim, caption, sort, value, chart_element*/
    1199546373 | $$self.$$.dirty[1] & /*_y_aggregate, mounted, computed_style*/
    336) {
      computed_style && requestAnimationFrame(load_chart);
    }
  };
  return [
    value,
    root,
    caption,
    gradio,
    label,
    elem_id,
    elem_classes,
    visible,
    show_label,
    scale,
    min_width,
    loading_status,
    height,
    chart_element,
    is_browser,
    x,
    y,
    color,
    title,
    x_title,
    y_title,
    color_title,
    x_bin,
    y_aggregate,
    color_map,
    x_lim,
    y_lim,
    x_label_angle,
    y_label_angle,
    x_axis_labels_visible,
    sort,
    tooltip,
    _selectable,
    _data,
    _x_bin,
    _y_aggregate,
    aggregating,
    mounted,
    x_temporal,
    computed_style,
    clear_status_handler,
    div_binding
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
        value: 0,
        x: 15,
        y: 16,
        color: 17,
        root: 1,
        title: 18,
        x_title: 19,
        y_title: 20,
        color_title: 21,
        x_bin: 22,
        y_aggregate: 23,
        color_map: 24,
        x_lim: 25,
        y_lim: 26,
        x_label_angle: 27,
        y_label_angle: 28,
        x_axis_labels_visible: 29,
        caption: 2,
        sort: 30,
        tooltip: 31,
        _selectable: 32,
        gradio: 3,
        label: 4,
        elem_id: 5,
        elem_classes: 6,
        visible: 7,
        show_label: 8,
        scale: 9,
        min_width: 10,
        loading_status: 11,
        height: 12
      },
      null,
      [-1, -1]
    );
  }
}
export {
  Index as default
};
