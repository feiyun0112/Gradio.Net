import { c as create_ssr_component, a as createEventDispatcher, b as add_attribute } from './ssr-RaXq3SJh.js';

var plotly_min = {exports: {}};

/**
* plotly.js v2.32.0
* Copyright 2012-2024, Plotly, Inc.
* All rights reserved.
* Licensed under the MIT license
*/

var hasRequiredPlotly_min;

function requirePlotly_min () {
	if (hasRequiredPlotly_min) return plotly_min.exports;
	hasRequiredPlotly_min = 1;
	(function (module, exports) {
		/*! For license information please see plotly.min.js.LICENSE.txt */
	} (plotly_min));
	return plotly_min.exports;
}

requirePlotly_min();

const PlotlyPlot = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { value } = $$props;
  let { show_label } = $$props;
  let plot_div;
  createEventDispatcher();
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  value?.plot;
  return `<div${add_attribute("data-testid", "plotly", 0)}${add_attribute("this", plot_div, 0)}></div>`;
});

export { PlotlyPlot as default };
//# sourceMappingURL=PlotlyPlot-P3y0I-J2.js.map