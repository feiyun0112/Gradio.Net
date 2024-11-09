import { SvelteComponent, init, safe_not_equal, element, space, claim_element, children, detach, claim_space, attr, insert_hydration, append_hydration, noop, onMount, onDestroy, text, claim_text, set_data, binding_callbacks } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { k as colors } from "./2.BqWhUxOo.js";
import { g as get_next_color } from "./color.DhNifA4j.js";
import embed from "./vega-embed.module.zbmVXw6j.js";
function set_config(spec, computed_style, chart_type, colors2) {
  let accentColor = computed_style.getPropertyValue("--color-accent");
  let bodyTextColor = computed_style.getPropertyValue("--body-text-color");
  let borderColorPrimary = computed_style.getPropertyValue(
    "--border-color-primary"
  );
  let fontFamily = computed_style.fontFamily;
  let titleWeight = computed_style.getPropertyValue(
    "--block-title-text-weight"
  );
  const fontToPxVal = (font) => {
    return font.endsWith("px") ? parseFloat(font.slice(0, -2)) : 12;
  };
  let textSizeMd = fontToPxVal(computed_style.getPropertyValue("--text-md"));
  let textSizeSm = fontToPxVal(computed_style.getPropertyValue("--text-sm"));
  let config = {
    autosize: { type: "fit", contains: "padding" },
    axis: {
      labelFont: fontFamily,
      labelColor: bodyTextColor,
      titleFont: fontFamily,
      titleColor: bodyTextColor,
      tickColor: borderColorPrimary,
      labelFontSize: textSizeSm,
      gridColor: borderColorPrimary,
      titleFontWeight: "normal",
      titleFontSize: textSizeSm,
      labelFontWeight: "normal",
      domain: false,
      labelAngle: 0
    },
    legend: {
      labelColor: bodyTextColor,
      labelFont: fontFamily,
      titleColor: bodyTextColor,
      titleFont: fontFamily,
      titleFontWeight: "normal",
      titleFontSize: textSizeSm,
      labelFontWeight: "normal",
      offset: 2
    },
    title: {
      color: bodyTextColor,
      font: fontFamily,
      fontSize: textSizeMd,
      fontWeight: titleWeight,
      anchor: "middle"
    },
    view: {
      stroke: borderColorPrimary
    }
  };
  spec.config = config;
  let encoding = spec.encoding;
  let layer = spec.layer;
  switch (chart_type) {
    case "scatter":
      spec.config.mark = { stroke: accentColor };
      if (encoding.color && encoding.color.type == "nominal") {
        encoding.color.scale.range = encoding.color.scale.range.map(
          (_, i) => get_color(colors2, i)
        );
      } else if (encoding.color && encoding.color.type == "quantitative") {
        encoding.color.scale.range = ["#eff6ff", "#1e3a8a"];
        encoding.color.scale.range.interpolate = "hsl";
      }
      break;
    case "line":
      spec.config.mark = { stroke: accentColor, cursor: "crosshair" };
      layer.forEach((d) => {
        if (d.encoding.color) {
          d.encoding.color.scale.range = d.encoding.color.scale.range.map(
            (_, i) => get_color(colors2, i)
          );
        }
      });
      break;
    case "bar":
      spec.config.mark = { opacity: 0.8, fill: accentColor };
      if (encoding.color) {
        encoding.color.scale.range = encoding.color.scale.range.map(
          (_, i) => get_color(colors2, i)
        );
      }
      break;
  }
  return spec;
}
function get_color(colors$1, index) {
  var _a;
  let current_color = colors$1[index % colors$1.length];
  if (current_color && current_color in colors) {
    return (_a = colors[current_color]) == null ? void 0 : _a.primary;
  } else if (!current_color) {
    return colors[get_next_color(index)].primary;
  }
  return current_color;
}
function create_if_block(ctx) {
  let div;
  let t;
  return {
    c() {
      div = element("div");
      t = text(
        /*caption*/
        ctx[0]
      );
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      t = claim_text(
        div_nodes,
        /*caption*/
        ctx[0]
      );
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "caption layout svelte-1qhqpn7");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*caption*/
      1)
        set_data(
          t,
          /*caption*/
          ctx2[0]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
    }
  };
}
function create_fragment(ctx) {
  let div1;
  let div0;
  let t;
  let if_block = (
    /*caption*/
    ctx[0] && create_if_block(ctx)
  );
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      t = space();
      if (if_block)
        if_block.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { "data-testid": true, class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", {});
      children(div0).forEach(detach);
      t = claim_space(div1_nodes);
      if (if_block)
        if_block.l(div1_nodes);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div1, "data-testid", "altair");
      attr(div1, "class", "altair layout svelte-1qhqpn7");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      ctx[11](div0);
      append_hydration(div1, t);
      if (if_block)
        if_block.m(div1, null);
      ctx[12](div1);
    },
    p(ctx2, [dirty]) {
      if (
        /*caption*/
        ctx2[0]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block(ctx2);
          if_block.c();
          if_block.m(div1, null);
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
        detach(div1);
      }
      ctx[11](null);
      if (if_block)
        if_block.d();
      ctx[12](null);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let plot;
  let spec;
  let fit_width_to_parent;
  let { value } = $$props;
  let { colors: colors2 = [] } = $$props;
  let { caption } = $$props;
  let { show_actions_button } = $$props;
  let { gradio } = $$props;
  let element2;
  let parent_element;
  let view;
  let { _selectable } = $$props;
  let computed_style = window.getComputedStyle(document.body);
  let old_spec;
  let spec_width;
  const get_width = () => {
    return Math.min(parent_element.offsetWidth, spec_width || parent_element.offsetWidth);
  };
  let resize_callback = () => {
  };
  const renderPlot = () => {
    if (fit_width_to_parent) {
      $$invalidate(9, spec.width = get_width(), spec);
    }
    embed(element2, spec, { actions: show_actions_button }).then(function(result) {
      view = result.view;
      resize_callback = () => {
        view.signal("width", get_width()).run();
      };
      if (!_selectable)
        return;
      const callback = (event, item) => {
        const brushValue = view.signal("brush");
        if (brushValue) {
          if (Object.keys(brushValue).length === 0) {
            gradio.dispatch("select", {
              value: null,
              index: null,
              selected: false
            });
          } else {
            const key = Object.keys(brushValue)[0];
            let range = brushValue[key].map((x) => x / 1e3);
            gradio.dispatch("select", {
              value: brushValue,
              index: range,
              selected: true
            });
          }
        }
      };
      view.addEventListener("mouseup", callback);
      view.addEventListener("touchup", callback);
    });
  };
  let resizeObserver = new ResizeObserver(() => {
    if (fit_width_to_parent && spec.width !== parent_element.offsetWidth) {
      resize_callback();
    }
  });
  onMount(() => {
    renderPlot();
    resizeObserver.observe(parent_element);
  });
  onDestroy(() => {
    resizeObserver.disconnect();
  });
  function div0_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      element2 = $$value;
      $$invalidate(1, element2);
    });
  }
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      parent_element = $$value;
      $$invalidate(2, parent_element);
    });
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(3, value = $$props2.value);
    if ("colors" in $$props2)
      $$invalidate(4, colors2 = $$props2.colors);
    if ("caption" in $$props2)
      $$invalidate(0, caption = $$props2.caption);
    if ("show_actions_button" in $$props2)
      $$invalidate(5, show_actions_button = $$props2.show_actions_button);
    if ("gradio" in $$props2)
      $$invalidate(6, gradio = $$props2.gradio);
    if ("_selectable" in $$props2)
      $$invalidate(7, _selectable = $$props2._selectable);
  };
  $$self.$$.update = () => {
    var _a, _b, _c, _d;
    if ($$self.$$.dirty & /*value*/
    8) {
      $$invalidate(10, plot = value == null ? void 0 : value.plot);
    }
    if ($$self.$$.dirty & /*plot*/
    1024) {
      $$invalidate(9, spec = JSON.parse(plot));
    }
    if ($$self.$$.dirty & /*spec, _selectable*/
    640) {
      if (spec && spec.params && !_selectable) {
        $$invalidate(9, spec.params = spec.params.filter((param) => param.name !== "brush"), spec);
      }
    }
    if ($$self.$$.dirty & /*value, spec, colors*/
    536) {
      if (value.chart) {
        $$invalidate(9, spec = set_config(spec, computed_style, value.chart, colors2));
      }
    }
    if ($$self.$$.dirty & /*old_spec, spec*/
    768) {
      if (old_spec !== spec) {
        $$invalidate(8, old_spec = spec);
        spec_width = spec.width;
      }
    }
    if ($$self.$$.dirty & /*spec, value*/
    520) {
      fit_width_to_parent = ((_b = (_a = spec.encoding) == null ? void 0 : _a.column) == null ? void 0 : _b.field) || ((_d = (_c = spec.encoding) == null ? void 0 : _c.row) == null ? void 0 : _d.field) || value.chart === void 0 ? false : true;
    }
  };
  return [
    caption,
    element2,
    parent_element,
    value,
    colors2,
    show_actions_button,
    gradio,
    _selectable,
    old_spec,
    spec,
    plot,
    div0_binding,
    div1_binding
  ];
}
class AltairPlot extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 3,
      colors: 4,
      caption: 0,
      show_actions_button: 5,
      gradio: 6,
      _selectable: 7
    });
  }
}
export {
  AltairPlot as default
};
