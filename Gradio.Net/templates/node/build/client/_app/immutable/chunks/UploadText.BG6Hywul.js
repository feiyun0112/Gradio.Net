import { SvelteComponent, init, safe_not_equal, element, space, claim_element, children, detach, claim_space, attr, toggle_class, insert_hydration, append_hydration, group_outros, transition_out, check_outros, transition_in, create_component, claim_component, mount_component, destroy_component, text, empty, claim_text, set_data } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import "./2.BqWhUxOo.js";
import { U as Upload, I as ImagePaste } from "./Upload.v2Thvwuk.js";
const RE_HEADING = /^(#\s*)(.+)$/m;
function inject(text2) {
  const trimmed_text = text2.trim();
  const heading_match = trimmed_text.match(RE_HEADING);
  if (!heading_match) {
    return [false, trimmed_text || false];
  }
  const [full_match, , heading_content] = heading_match;
  const _heading = heading_content.trim();
  if (trimmed_text === full_match) {
    return [_heading, false];
  }
  const heading_end_index = heading_match.index !== void 0 ? heading_match.index + full_match.length : 0;
  const remaining_text = trimmed_text.substring(heading_end_index).trim();
  const _paragraph = remaining_text || false;
  return [_heading, _paragraph];
}
function create_else_block_1(ctx) {
  let uploadicon;
  let current;
  uploadicon = new Upload({});
  return {
    c() {
      create_component(uploadicon.$$.fragment);
    },
    l(nodes) {
      claim_component(uploadicon.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(uploadicon, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(uploadicon.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(uploadicon.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(uploadicon, detaching);
    }
  };
}
function create_if_block_4(ctx) {
  let imagepaste;
  let current;
  imagepaste = new ImagePaste({});
  return {
    c() {
      create_component(imagepaste.$$.fragment);
    },
    l(nodes) {
      claim_component(imagepaste.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(imagepaste, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(imagepaste.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(imagepaste.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(imagepaste, detaching);
    }
  };
}
function create_else_block(ctx) {
  let t0_value = (
    /*i18n*/
    ctx[1](
      /*defs*/
      ctx[7][
        /*type*/
        ctx[0]
      ] || /*defs*/
      ctx[7].file
    ) + ""
  );
  let t0;
  let t1;
  let if_block_anchor;
  let if_block = (
    /*mode*/
    ctx[3] !== "short" && create_if_block_3(ctx)
  );
  return {
    c() {
      t0 = text(t0_value);
      t1 = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      t0 = claim_text(nodes, t0_value);
      t1 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      insert_hydration(target, t0, anchor);
      insert_hydration(target, t1, anchor);
      if (if_block)
        if_block.m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*i18n, type*/
      3 && t0_value !== (t0_value = /*i18n*/
      ctx2[1](
        /*defs*/
        ctx2[7][
          /*type*/
          ctx2[0]
        ] || /*defs*/
        ctx2[7].file
      ) + ""))
        set_data(t0, t0_value);
      if (
        /*mode*/
        ctx2[3] !== "short"
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_3(ctx2);
          if_block.c();
          if_block.m(if_block_anchor.parentNode, if_block_anchor);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(t1);
        detach(if_block_anchor);
      }
      if (if_block)
        if_block.d(detaching);
    }
  };
}
function create_if_block(ctx) {
  let t;
  let if_block1_anchor;
  let if_block0 = (
    /*heading*/
    ctx[6] && create_if_block_2(ctx)
  );
  let if_block1 = (
    /*paragraph*/
    ctx[5] && create_if_block_1(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (
        /*heading*/
        ctx2[6]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_2(ctx2);
          if_block0.c();
          if_block0.m(t.parentNode, t);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (
        /*paragraph*/
        ctx2[5]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
        } else {
          if_block1 = create_if_block_1(ctx2);
          if_block1.c();
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        if_block1.d(1);
        if_block1 = null;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(t);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_if_block_3(ctx) {
  let span;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[1]("common.or") + ""
  );
  let t1;
  let t2;
  let t3;
  let t4_value = (
    /*message*/
    (ctx[2] || /*i18n*/
    ctx[1]("upload_text.click_to_upload")) + ""
  );
  let t4;
  return {
    c() {
      span = element("span");
      t0 = text("- ");
      t1 = text(t1_value);
      t2 = text(" -");
      t3 = space();
      t4 = text(t4_value);
      this.h();
    },
    l(nodes) {
      span = claim_element(nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      t0 = claim_text(span_nodes, "- ");
      t1 = claim_text(span_nodes, t1_value);
      t2 = claim_text(span_nodes, " -");
      span_nodes.forEach(detach);
      t3 = claim_space(nodes);
      t4 = claim_text(nodes, t4_value);
      this.h();
    },
    h() {
      attr(span, "class", "or svelte-12ioyct");
    },
    m(target, anchor) {
      insert_hydration(target, span, anchor);
      append_hydration(span, t0);
      append_hydration(span, t1);
      append_hydration(span, t2);
      insert_hydration(target, t3, anchor);
      insert_hydration(target, t4, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*i18n*/
      2 && t1_value !== (t1_value = /*i18n*/
      ctx2[1]("common.or") + ""))
        set_data(t1, t1_value);
      if (dirty & /*message, i18n*/
      6 && t4_value !== (t4_value = /*message*/
      (ctx2[2] || /*i18n*/
      ctx2[1]("upload_text.click_to_upload")) + ""))
        set_data(t4, t4_value);
    },
    d(detaching) {
      if (detaching) {
        detach(span);
        detach(t3);
        detach(t4);
      }
    }
  };
}
function create_if_block_2(ctx) {
  let h2;
  let t;
  return {
    c() {
      h2 = element("h2");
      t = text(
        /*heading*/
        ctx[6]
      );
      this.h();
    },
    l(nodes) {
      h2 = claim_element(nodes, "H2", { class: true });
      var h2_nodes = children(h2);
      t = claim_text(
        h2_nodes,
        /*heading*/
        ctx[6]
      );
      h2_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(h2, "class", "svelte-12ioyct");
    },
    m(target, anchor) {
      insert_hydration(target, h2, anchor);
      append_hydration(h2, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*heading*/
      64)
        set_data(
          t,
          /*heading*/
          ctx2[6]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(h2);
      }
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
        /*paragraph*/
        ctx[5]
      );
      this.h();
    },
    l(nodes) {
      p = claim_element(nodes, "P", { class: true });
      var p_nodes = children(p);
      t = claim_text(
        p_nodes,
        /*paragraph*/
        ctx[5]
      );
      p_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(p, "class", "svelte-12ioyct");
    },
    m(target, anchor) {
      insert_hydration(target, p, anchor);
      append_hydration(p, t);
    },
    p(ctx2, dirty) {
      if (dirty & /*paragraph*/
      32)
        set_data(
          t,
          /*paragraph*/
          ctx2[5]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(p);
      }
    }
  };
}
function create_fragment(ctx) {
  let div;
  let span;
  let current_block_type_index;
  let if_block0;
  let t;
  let current;
  const if_block_creators = [create_if_block_4, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*type*/
      ctx2[0] === "clipboard"
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  function select_block_type_1(ctx2, dirty) {
    if (
      /*heading*/
      ctx2[6] || /*paragraph*/
      ctx2[5]
    )
      return create_if_block;
    return create_else_block;
  }
  let current_block_type = select_block_type_1(ctx);
  let if_block1 = current_block_type(ctx);
  return {
    c() {
      div = element("div");
      span = element("span");
      if_block0.c();
      t = space();
      if_block1.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      span = claim_element(div_nodes, "SPAN", { class: true });
      var span_nodes = children(span);
      if_block0.l(span_nodes);
      span_nodes.forEach(detach);
      t = claim_space(div_nodes);
      if_block1.l(div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span, "class", "icon-wrap svelte-12ioyct");
      toggle_class(
        span,
        "hovered",
        /*hovered*/
        ctx[4]
      );
      attr(div, "class", "wrap svelte-12ioyct");
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      append_hydration(div, span);
      if_blocks[current_block_type_index].m(span, null);
      append_hydration(div, t);
      if_block1.m(div, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index !== previous_block_index) {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block0 = if_blocks[current_block_type_index];
        if (!if_block0) {
          if_block0 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block0.c();
        }
        transition_in(if_block0, 1);
        if_block0.m(span, null);
      }
      if (!current || dirty & /*hovered*/
      16) {
        toggle_class(
          span,
          "hovered",
          /*hovered*/
          ctx2[4]
        );
      }
      if (current_block_type === (current_block_type = select_block_type_1(ctx2)) && if_block1) {
        if_block1.p(ctx2, dirty);
      } else {
        if_block1.d(1);
        if_block1 = current_block_type(ctx2);
        if (if_block1) {
          if_block1.c();
          if_block1.m(div, null);
        }
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
        detach(div);
      }
      if_blocks[current_block_type_index].d();
      if_block1.d();
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let heading;
  let paragraph;
  let { type = "file" } = $$props;
  let { i18n } = $$props;
  let { message = void 0 } = $$props;
  let { mode = "full" } = $$props;
  let { hovered = false } = $$props;
  let { placeholder = void 0 } = $$props;
  const defs = {
    image: "upload_text.drop_image",
    video: "upload_text.drop_video",
    audio: "upload_text.drop_audio",
    file: "upload_text.drop_file",
    csv: "upload_text.drop_csv",
    gallery: "upload_text.drop_gallery",
    clipboard: "upload_text.paste_clipboard"
  };
  $$self.$$set = ($$props2) => {
    if ("type" in $$props2)
      $$invalidate(0, type = $$props2.type);
    if ("i18n" in $$props2)
      $$invalidate(1, i18n = $$props2.i18n);
    if ("message" in $$props2)
      $$invalidate(2, message = $$props2.message);
    if ("mode" in $$props2)
      $$invalidate(3, mode = $$props2.mode);
    if ("hovered" in $$props2)
      $$invalidate(4, hovered = $$props2.hovered);
    if ("placeholder" in $$props2)
      $$invalidate(8, placeholder = $$props2.placeholder);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*placeholder*/
    256) {
      $$invalidate(6, [heading, paragraph] = placeholder ? inject(placeholder) : [false, false], heading, ($$invalidate(5, paragraph), $$invalidate(8, placeholder)));
    }
  };
  return [type, i18n, message, mode, hovered, paragraph, heading, defs, placeholder];
}
class UploadText extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      type: 0,
      i18n: 1,
      message: 2,
      mode: 3,
      hovered: 4,
      placeholder: 8
    });
  }
}
export {
  UploadText as U
};
