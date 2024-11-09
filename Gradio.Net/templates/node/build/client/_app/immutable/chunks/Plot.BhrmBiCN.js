const __vite__fileDeps=["./PlotlyPlot.DB9IIckf.js","./2.BqWhUxOo.js","./preload-helper.DpQnamwV.js","./stores.DcWgXC6T.js","./client.DB6ownDU.js","../assets/2.DoS7Rli5.css","./BokehPlot.Dd2N6NIe.js","../assets/BokehPlot.DX04T7W4.css","./AltairPlot.DmdWsCOe.js","./color.DhNifA4j.js","./vega-embed.module.zbmVXw6j.js","./dsv.CKxpOsHB.js","../assets/AltairPlot.O7q90H0e.css","./MatplotlibPlot.m3Jijthl.js","../assets/MatplotlibPlot.BHPTlvkw.css"],__vite__mapDeps=i=>i.map(i=>__vite__fileDeps[i]);
import { _ as __vitePreload } from "./preload-helper.DpQnamwV.js";
import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, empty, group_outros, transition_out, check_outros, transition_in, bubble, create_component, claim_component, mount_component, destroy_component, construct_svelte_component } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
function create_fragment$1(ctx) {
  let svg;
  let circle0;
  let circle1;
  let circle2;
  let circle3;
  let circle4;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      circle0 = svg_element("circle");
      circle1 = svg_element("circle");
      circle2 = svg_element("circle");
      circle3 = svg_element("circle");
      circle4 = svg_element("circle");
      path = svg_element("path");
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
      circle0 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle0).forEach(detach);
      circle1 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle1).forEach(detach);
      circle2 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle2).forEach(detach);
      circle3 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle3).forEach(detach);
      circle4 = claim_svg_element(svg_nodes, "circle", { cx: true, cy: true, r: true, fill: true });
      children(circle4).forEach(detach);
      path = claim_svg_element(svg_nodes, "path", { fill: true, d: true });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(circle0, "cx", "20");
      attr(circle0, "cy", "4");
      attr(circle0, "r", "2");
      attr(circle0, "fill", "currentColor");
      attr(circle1, "cx", "8");
      attr(circle1, "cy", "16");
      attr(circle1, "r", "2");
      attr(circle1, "fill", "currentColor");
      attr(circle2, "cx", "28");
      attr(circle2, "cy", "12");
      attr(circle2, "r", "2");
      attr(circle2, "fill", "currentColor");
      attr(circle3, "cx", "11");
      attr(circle3, "cy", "7");
      attr(circle3, "r", "2");
      attr(circle3, "fill", "currentColor");
      attr(circle4, "cx", "16");
      attr(circle4, "cy", "24");
      attr(circle4, "r", "2");
      attr(circle4, "fill", "currentColor");
      attr(path, "fill", "currentColor");
      attr(path, "d", "M30 3.413L28.586 2L4 26.585V2H2v26a2 2 0 0 0 2 2h26v-2H5.413Z");
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
      append_hydration(svg, circle0);
      append_hydration(svg, circle1);
      append_hydration(svg, circle2);
      append_hydration(svg, circle3);
      append_hydration(svg, circle4);
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
let Plot$2 = class Plot extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$1, safe_not_equal, {});
  }
};
function create_else_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      unpadded_box: true,
      size: "large",
      $$slots: { default: [create_default_slot] },
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
      524288) {
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
  let previous_key = (
    /*key*/
    ctx[11]
  );
  let key_block_anchor;
  let current;
  let key_block = create_key_block(ctx);
  return {
    c() {
      key_block.c();
      key_block_anchor = empty();
    },
    l(nodes) {
      key_block.l(nodes);
      key_block_anchor = empty();
    },
    m(target, anchor) {
      key_block.m(target, anchor);
      insert_hydration(target, key_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty & /*key*/
      2048 && safe_not_equal(previous_key, previous_key = /*key*/
      ctx2[11])) {
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
      transition_in(key_block);
      current = true;
    },
    o(local) {
      transition_out(key_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(key_block_anchor);
      }
      key_block.d(detaching);
    }
  };
}
function create_default_slot(ctx) {
  let ploticon;
  let current;
  ploticon = new Plot$2({});
  return {
    c() {
      create_component(ploticon.$$.fragment);
    },
    l(nodes) {
      claim_component(ploticon.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(ploticon, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(ploticon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(ploticon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(ploticon, detaching);
    }
  };
}
function create_key_block(ctx) {
  let switch_instance;
  let switch_instance_anchor;
  let current;
  var switch_value = (
    /*PlotComponent*/
    ctx[10]
  );
  function switch_props(ctx2, dirty) {
    return {
      props: {
        value: (
          /*value*/
          ctx2[0]
        ),
        colors: (
          /*colors*/
          ctx2[1]
        ),
        theme_mode: (
          /*theme_mode*/
          ctx2[3]
        ),
        show_label: (
          /*show_label*/
          ctx2[2]
        ),
        caption: (
          /*caption*/
          ctx2[4]
        ),
        bokeh_version: (
          /*bokeh_version*/
          ctx2[5]
        ),
        show_actions_button: (
          /*show_actions_button*/
          ctx2[6]
        ),
        gradio: (
          /*gradio*/
          ctx2[7]
        ),
        _selectable: (
          /*_selectable*/
          ctx2[9]
        ),
        x_lim: (
          /*x_lim*/
          ctx2[8]
        )
      }
    };
  }
  if (switch_value) {
    switch_instance = construct_svelte_component(switch_value, switch_props(ctx));
    switch_instance.$on(
      "load",
      /*load_handler*/
      ctx[15]
    );
    switch_instance.$on(
      "select",
      /*select_handler*/
      ctx[16]
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
      if (dirty & /*PlotComponent*/
      1024 && switch_value !== (switch_value = /*PlotComponent*/
      ctx2[10])) {
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
            ctx2[15]
          );
          switch_instance.$on(
            "select",
            /*select_handler*/
            ctx2[16]
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
        1)
          switch_instance_changes.value = /*value*/
          ctx2[0];
        if (dirty & /*colors*/
        2)
          switch_instance_changes.colors = /*colors*/
          ctx2[1];
        if (dirty & /*theme_mode*/
        8)
          switch_instance_changes.theme_mode = /*theme_mode*/
          ctx2[3];
        if (dirty & /*show_label*/
        4)
          switch_instance_changes.show_label = /*show_label*/
          ctx2[2];
        if (dirty & /*caption*/
        16)
          switch_instance_changes.caption = /*caption*/
          ctx2[4];
        if (dirty & /*bokeh_version*/
        32)
          switch_instance_changes.bokeh_version = /*bokeh_version*/
          ctx2[5];
        if (dirty & /*show_actions_button*/
        64)
          switch_instance_changes.show_actions_button = /*show_actions_button*/
          ctx2[6];
        if (dirty & /*gradio*/
        128)
          switch_instance_changes.gradio = /*gradio*/
          ctx2[7];
        if (dirty & /*_selectable*/
        512)
          switch_instance_changes._selectable = /*_selectable*/
          ctx2[9];
        if (dirty & /*x_lim*/
        256)
          switch_instance_changes.x_lim = /*x_lim*/
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
function create_fragment(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] && /*PlotComponent*/
      ctx2[10]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
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
    p(ctx2, [dirty]) {
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
function instance($$self, $$props, $$invalidate) {
  let { value } = $$props;
  let _value;
  let { colors = [] } = $$props;
  let { show_label } = $$props;
  let { theme_mode } = $$props;
  let { caption } = $$props;
  let { bokeh_version } = $$props;
  let { show_actions_button } = $$props;
  let { gradio } = $$props;
  let { x_lim = null } = $$props;
  let { _selectable } = $$props;
  let PlotComponent = null;
  let _type = value == null ? void 0 : value.type;
  const plotTypeMapping = {
    plotly: () => __vitePreload(() => import("./PlotlyPlot.DB9IIckf.js"), true ? __vite__mapDeps([0,1,2,3,4,5]) : void 0, import.meta.url),
    bokeh: () => __vitePreload(() => import("./BokehPlot.Dd2N6NIe.js"), true ? __vite__mapDeps([6,7]) : void 0, import.meta.url),
    altair: () => __vitePreload(() => import("./AltairPlot.DmdWsCOe.js"), true ? __vite__mapDeps([8,1,2,3,4,5,9,10,11,12]) : void 0, import.meta.url),
    matplotlib: () => __vitePreload(() => import("./MatplotlibPlot.m3Jijthl.js"), true ? __vite__mapDeps([13,14]) : void 0, import.meta.url)
  };
  let loadedPlotTypeMapping = {};
  const is_browser = typeof window !== "undefined";
  let key = 0;
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  function select_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("colors" in $$props2)
      $$invalidate(1, colors = $$props2.colors);
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("theme_mode" in $$props2)
      $$invalidate(3, theme_mode = $$props2.theme_mode);
    if ("caption" in $$props2)
      $$invalidate(4, caption = $$props2.caption);
    if ("bokeh_version" in $$props2)
      $$invalidate(5, bokeh_version = $$props2.bokeh_version);
    if ("show_actions_button" in $$props2)
      $$invalidate(6, show_actions_button = $$props2.show_actions_button);
    if ("gradio" in $$props2)
      $$invalidate(7, gradio = $$props2.gradio);
    if ("x_lim" in $$props2)
      $$invalidate(8, x_lim = $$props2.x_lim);
    if ("_selectable" in $$props2)
      $$invalidate(9, _selectable = $$props2._selectable);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value, _value, key, _type, loadedPlotTypeMapping, PlotComponent*/
    31745) {
      if (value !== _value) {
        $$invalidate(11, key += 1);
        let type = value == null ? void 0 : value.type;
        if (type !== _type) {
          $$invalidate(10, PlotComponent = null);
        }
        if (type && type in plotTypeMapping && is_browser) {
          if (loadedPlotTypeMapping[type]) {
            $$invalidate(10, PlotComponent = loadedPlotTypeMapping[type]);
          } else {
            plotTypeMapping[type]().then((module) => {
              $$invalidate(10, PlotComponent = module.default);
              $$invalidate(14, loadedPlotTypeMapping[type] = PlotComponent, loadedPlotTypeMapping);
            });
          }
        }
        $$invalidate(12, _value = value);
        $$invalidate(13, _type = type);
      }
    }
  };
  return [
    value,
    colors,
    show_label,
    theme_mode,
    caption,
    bokeh_version,
    show_actions_button,
    gradio,
    x_lim,
    _selectable,
    PlotComponent,
    key,
    _value,
    _type,
    loadedPlotTypeMapping,
    load_handler,
    select_handler
  ];
}
class Plot2 extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      colors: 1,
      show_label: 2,
      theme_mode: 3,
      caption: 4,
      bokeh_version: 5,
      show_actions_button: 6,
      gradio: 7,
      x_lim: 8,
      _selectable: 9
    });
  }
}
const Plot$1 = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: Plot2
}, Symbol.toStringTag, { value: "Module" }));
export {
  Plot$2 as P,
  Plot2 as a,
  Plot$1 as b
};
