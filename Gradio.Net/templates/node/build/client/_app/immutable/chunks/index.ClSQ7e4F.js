const __vite__fileDeps=["./module.8J93j-Wi.js","./module.CwcSxJsq.js","./2.BqWhUxOo.js","./preload-helper.DpQnamwV.js","./stores.DcWgXC6T.js","./client.DB6ownDU.js","../assets/2.DoS7Rli5.css","./module.BBKnj-Vx.js"],__vite__mapDeps=i=>i.map(i=>__vite__fileDeps[i]);
import { SvelteComponent, init, safe_not_equal, element, claim_element, children, detach, attr, insert_hydration, noop, createEventDispatcher, ensure_array_like, empty, destroy_each, text, claim_text, set_input_value, append_hydration, set_data, binding_callbacks, bind, space, create_component, claim_space, claim_component, mount_component, listen, add_flush_callback, transition_in, transition_out, destroy_component, run_all, group_outros, check_outros, onMount, get_svelte_dataset, set_style, null_to_empty, onDestroy, bubble, create_slot, update_slot_base, get_all_dirty_from_scope, get_slot_changes, flush, afterUpdate, assign, get_spread_update, get_spread_object } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { s as skip_audio, W as WaveformControls, p as process_audio, a as WaveSurfer, A as AudioPlayer, S as StaticAudio } from "./StaticAudio.a9A5cXcJ.js";
import { _ as __vitePreload } from "./preload-helper.DpQnamwV.js";
import { U as Upload } from "./Upload.CpXh2Xm5.js";
import { M as ModifyUpload } from "./ModifyUpload.Bygvg79x.js";
import { x as format_time, h as prepare_files, B as Block, S as Static } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { M as Music } from "./Music.BKn1BNLT.js";
import { a as Spinner, S as SelectSource } from "./SelectSource.j1x9BYNI.js";
import { S as StreamingBar } from "./StreamingBar.CkIfdne5.js";
import { P as Pause } from "./Trim.CQ15_So8.js";
import { U as UploadText } from "./UploadText.BG6Hywul.js";
import { default as default2 } from "./Example.CNa918u4.js";
function e(e2, t2, i2, s2) {
  return new (i2 || (i2 = Promise))(function(r2, n) {
    function o(e3) {
      try {
        d(s2.next(e3));
      } catch (e4) {
        n(e4);
      }
    }
    function a(e3) {
      try {
        d(s2.throw(e3));
      } catch (e4) {
        n(e4);
      }
    }
    function d(e3) {
      var t3;
      e3.done ? r2(e3.value) : (t3 = e3.value, t3 instanceof i2 ? t3 : new i2(function(e4) {
        e4(t3);
      })).then(o, a);
    }
    d((s2 = s2.apply(e2, [])).next());
  });
}
"function" == typeof SuppressedError && SuppressedError;
class t {
  constructor() {
    this.listeners = {}, this.on = this.addEventListener, this.un = this.removeEventListener;
  }
  addEventListener(e2, t2, i2) {
    if (this.listeners[e2] || (this.listeners[e2] = /* @__PURE__ */ new Set()), this.listeners[e2].add(t2), null == i2 ? void 0 : i2.once) {
      const i3 = () => {
        this.removeEventListener(e2, i3), this.removeEventListener(e2, t2);
      };
      return this.addEventListener(e2, i3), i3;
    }
    return () => this.removeEventListener(e2, t2);
  }
  removeEventListener(e2, t2) {
    var i2;
    null === (i2 = this.listeners[e2]) || void 0 === i2 || i2.delete(t2);
  }
  once(e2, t2) {
    return this.on(e2, t2, { once: true });
  }
  unAll() {
    this.listeners = {};
  }
  emit(e2, ...t2) {
    this.listeners[e2] && this.listeners[e2].forEach((e3) => e3(...t2));
  }
}
class i extends t {
  constructor(e2) {
    super(), this.subscriptions = [], this.options = e2;
  }
  onInit() {
  }
  init(e2) {
    this.wavesurfer = e2, this.onInit();
  }
  destroy() {
    this.emit("destroy"), this.subscriptions.forEach((e2) => e2());
  }
}
const s = ["audio/webm", "audio/wav", "audio/mpeg", "audio/mp4", "audio/mp3"];
class r extends i {
  constructor(e2) {
    var t2;
    super(Object.assign(Object.assign({}, e2), { audioBitsPerSecond: null !== (t2 = e2.audioBitsPerSecond) && void 0 !== t2 ? t2 : 128e3 })), this.stream = null, this.mediaRecorder = null;
  }
  static create(e2) {
    return new r(e2 || {});
  }
  renderMicStream(e2) {
    const t2 = new AudioContext(), i2 = t2.createMediaStreamSource(e2), s2 = t2.createAnalyser();
    i2.connect(s2);
    const r2 = s2.frequencyBinCount, n = new Float32Array(r2), o = r2 / t2.sampleRate;
    let a;
    const d = () => {
      s2.getFloatTimeDomainData(n), this.wavesurfer && (this.wavesurfer.options.cursorWidth = 0, this.wavesurfer.options.interact = false, this.wavesurfer.load("", [n], o)), a = requestAnimationFrame(d);
    };
    return d(), () => {
      cancelAnimationFrame(a), null == i2 || i2.disconnect(), null == t2 || t2.close();
    };
  }
  startMic(t2) {
    return e(this, void 0, void 0, function* () {
      let e2;
      try {
        e2 = yield navigator.mediaDevices.getUserMedia({ audio: !(null == t2 ? void 0 : t2.deviceId) || { deviceId: t2.deviceId } });
      } catch (e3) {
        throw new Error("Error accessing the microphone: " + e3.message);
      }
      const i2 = this.renderMicStream(e2);
      return this.subscriptions.push(this.once("destroy", i2)), this.stream = e2, e2;
    });
  }
  stopMic() {
    this.stream && (this.stream.getTracks().forEach((e2) => e2.stop()), this.stream = null, this.mediaRecorder = null);
  }
  startRecording(t2) {
    return e(this, void 0, void 0, function* () {
      const e2 = this.stream || (yield this.startMic(t2)), i2 = this.mediaRecorder || new MediaRecorder(e2, { mimeType: this.options.mimeType || s.find((e3) => MediaRecorder.isTypeSupported(e3)), audioBitsPerSecond: this.options.audioBitsPerSecond });
      this.mediaRecorder = i2, this.stopRecording();
      const r2 = [];
      i2.ondataavailable = (e3) => {
        e3.data.size > 0 && r2.push(e3.data);
      }, i2.onstop = () => {
        var e3;
        const t3 = new Blob(r2, { type: i2.mimeType });
        this.emit("record-end", t3), false !== this.options.renderRecordedAudio && (null === (e3 = this.wavesurfer) || void 0 === e3 || e3.load(URL.createObjectURL(t3)));
      }, i2.start(), this.emit("record-start");
    });
  }
  isRecording() {
    var e2;
    return "recording" === (null === (e2 = this.mediaRecorder) || void 0 === e2 ? void 0 : e2.state);
  }
  isPaused() {
    var e2;
    return "paused" === (null === (e2 = this.mediaRecorder) || void 0 === e2 ? void 0 : e2.state);
  }
  stopRecording() {
    var e2;
    this.isRecording() && (null === (e2 = this.mediaRecorder) || void 0 === e2 || e2.stop());
  }
  pauseRecording() {
    var e2;
    this.isRecording() && (null === (e2 = this.mediaRecorder) || void 0 === e2 || e2.pause(), this.emit("record-pause"));
  }
  resumeRecording() {
    var e2;
    this.isPaused() && (null === (e2 = this.mediaRecorder) || void 0 === e2 || e2.resume(), this.emit("record-resume"));
  }
  static getAvailableAudioDevices() {
    return e(this, void 0, void 0, function* () {
      return navigator.mediaDevices.enumerateDevices().then((e2) => e2.filter((e3) => "audioinput" === e3.kind));
    });
  }
  destroy() {
    super.destroy(), this.stopRecording(), this.stopMic();
  }
}
function get_each_context(ctx, list, i2) {
  const child_ctx = ctx.slice();
  child_ctx[3] = list[i2];
  return child_ctx;
}
function create_else_block$4(ctx) {
  let each_1_anchor;
  let each_value = ensure_array_like(
    /*micDevices*/
    ctx[0]
  );
  let each_blocks = [];
  for (let i2 = 0; i2 < each_value.length; i2 += 1) {
    each_blocks[i2] = create_each_block(get_each_context(ctx, each_value, i2));
  }
  return {
    c() {
      for (let i2 = 0; i2 < each_blocks.length; i2 += 1) {
        each_blocks[i2].c();
      }
      each_1_anchor = empty();
    },
    l(nodes) {
      for (let i2 = 0; i2 < each_blocks.length; i2 += 1) {
        each_blocks[i2].l(nodes);
      }
      each_1_anchor = empty();
    },
    m(target, anchor) {
      for (let i2 = 0; i2 < each_blocks.length; i2 += 1) {
        if (each_blocks[i2]) {
          each_blocks[i2].m(target, anchor);
        }
      }
      insert_hydration(target, each_1_anchor, anchor);
    },
    p(ctx2, dirty) {
      if (dirty & /*micDevices*/
      1) {
        each_value = ensure_array_like(
          /*micDevices*/
          ctx2[0]
        );
        let i2;
        for (i2 = 0; i2 < each_value.length; i2 += 1) {
          const child_ctx = get_each_context(ctx2, each_value, i2);
          if (each_blocks[i2]) {
            each_blocks[i2].p(child_ctx, dirty);
          } else {
            each_blocks[i2] = create_each_block(child_ctx);
            each_blocks[i2].c();
            each_blocks[i2].m(each_1_anchor.parentNode, each_1_anchor);
          }
        }
        for (; i2 < each_blocks.length; i2 += 1) {
          each_blocks[i2].d(1);
        }
        each_blocks.length = each_value.length;
      }
    },
    d(detaching) {
      if (detaching) {
        detach(each_1_anchor);
      }
      destroy_each(each_blocks, detaching);
    }
  };
}
function create_if_block$5(ctx) {
  let option;
  let t_value = (
    /*i18n*/
    ctx[1]("audio.no_microphone") + ""
  );
  let t2;
  return {
    c() {
      option = element("option");
      t2 = text(t_value);
      this.h();
    },
    l(nodes) {
      option = claim_element(nodes, "OPTION", {});
      var option_nodes = children(option);
      t2 = claim_text(option_nodes, t_value);
      option_nodes.forEach(detach);
      this.h();
    },
    h() {
      option.__value = "";
      set_input_value(option, option.__value);
    },
    m(target, anchor) {
      insert_hydration(target, option, anchor);
      append_hydration(option, t2);
    },
    p(ctx2, dirty) {
      if (dirty & /*i18n*/
      2 && t_value !== (t_value = /*i18n*/
      ctx2[1]("audio.no_microphone") + ""))
        set_data(t2, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(option);
      }
    }
  };
}
function create_each_block(ctx) {
  let option;
  let t_value = (
    /*micDevice*/
    ctx[3].label + ""
  );
  let t2;
  let option_value_value;
  return {
    c() {
      option = element("option");
      t2 = text(t_value);
      this.h();
    },
    l(nodes) {
      option = claim_element(nodes, "OPTION", {});
      var option_nodes = children(option);
      t2 = claim_text(option_nodes, t_value);
      option_nodes.forEach(detach);
      this.h();
    },
    h() {
      option.__value = option_value_value = /*micDevice*/
      ctx[3].deviceId;
      set_input_value(option, option.__value);
    },
    m(target, anchor) {
      insert_hydration(target, option, anchor);
      append_hydration(option, t2);
    },
    p(ctx2, dirty) {
      if (dirty & /*micDevices*/
      1 && t_value !== (t_value = /*micDevice*/
      ctx2[3].label + ""))
        set_data(t2, t_value);
      if (dirty & /*micDevices*/
      1 && option_value_value !== (option_value_value = /*micDevice*/
      ctx2[3].deviceId)) {
        option.__value = option_value_value;
        set_input_value(option, option.__value);
      }
    },
    d(detaching) {
      if (detaching) {
        detach(option);
      }
    }
  };
}
function create_fragment$5(ctx) {
  let select;
  let select_disabled_value;
  function select_block_type(ctx2, dirty) {
    if (
      /*micDevices*/
      ctx2[0].length === 0
    )
      return create_if_block$5;
    return create_else_block$4;
  }
  let current_block_type = select_block_type(ctx);
  let if_block = current_block_type(ctx);
  return {
    c() {
      select = element("select");
      if_block.c();
      this.h();
    },
    l(nodes) {
      select = claim_element(nodes, "SELECT", { class: true, "aria-label": true });
      var select_nodes = children(select);
      if_block.l(select_nodes);
      select_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(select, "class", "mic-select svelte-1ya9x7a");
      attr(select, "aria-label", "Select input device");
      select.disabled = select_disabled_value = /*micDevices*/
      ctx[0].length === 0;
    },
    m(target, anchor) {
      insert_hydration(target, select, anchor);
      if_block.m(select, null);
    },
    p(ctx2, [dirty]) {
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block) {
        if_block.p(ctx2, dirty);
      } else {
        if_block.d(1);
        if_block = current_block_type(ctx2);
        if (if_block) {
          if_block.c();
          if_block.m(select, null);
        }
      }
      if (dirty & /*micDevices*/
      1 && select_disabled_value !== (select_disabled_value = /*micDevices*/
      ctx2[0].length === 0)) {
        select.disabled = select_disabled_value;
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(select);
      }
      if_block.d();
    }
  };
}
function instance$5($$self, $$props, $$invalidate) {
  let { i18n } = $$props;
  let { micDevices = [] } = $$props;
  const dispatch = createEventDispatcher();
  $$self.$$set = ($$props2) => {
    if ("i18n" in $$props2)
      $$invalidate(1, i18n = $$props2.i18n);
    if ("micDevices" in $$props2)
      $$invalidate(0, micDevices = $$props2.micDevices);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*i18n*/
    2) {
      if (typeof window !== "undefined") {
        try {
          let tempDevices = [];
          r.getAvailableAudioDevices().then((devices) => {
            $$invalidate(0, micDevices = devices);
            devices.forEach((device) => {
              if (device.deviceId) {
                tempDevices.push(device);
              }
            });
            $$invalidate(0, micDevices = tempDevices);
          });
        } catch (err) {
          if (err instanceof DOMException && err.name == "NotAllowedError") {
            dispatch("error", i18n("audio.allow_recording_access"));
          }
          throw err;
        }
      }
    }
  };
  return [micDevices, i18n];
}
class DeviceSelect extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$5, create_fragment$5, safe_not_equal, { i18n: 1, micDevices: 0 });
  }
}
function create_if_block$4(ctx) {
  let time;
  let t2;
  return {
    c() {
      time = element("time");
      t2 = text(
        /*record_time*/
        ctx[2]
      );
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { class: true });
      var time_nodes = children(time);
      t2 = claim_text(
        time_nodes,
        /*record_time*/
        ctx[2]
      );
      time_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "class", "duration-button duration svelte-11fum4b");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      append_hydration(time, t2);
    },
    p(ctx2, dirty) {
      if (dirty & /*record_time*/
      4)
        set_data(
          t2,
          /*record_time*/
          ctx2[2]
        );
    },
    d(detaching) {
      if (detaching) {
        detach(time);
      }
    }
  };
}
function create_fragment$4(ctx) {
  let div1;
  let div0;
  let button0;
  let t0_value = (
    /*i18n*/
    ctx[1]("audio.record") + ""
  );
  let t0;
  let t1;
  let button1;
  let t2_value = (
    /*i18n*/
    ctx[1]("audio.stop") + ""
  );
  let t2;
  let button1_class_value;
  let t3;
  let button2;
  let t4_value = (
    /*i18n*/
    ctx[1]("audio.stop") + ""
  );
  let t4;
  let t5;
  let button3;
  let pause;
  let t6;
  let button4;
  let t7_value = (
    /*i18n*/
    ctx[1]("audio.resume") + ""
  );
  let t7;
  let t8;
  let t9;
  let deviceselect;
  let updating_micDevices;
  let current;
  let mounted;
  let dispose;
  pause = new Pause({});
  let if_block = (
    /*timing*/
    ctx[4] && !/*show_recording_waveform*/
    ctx[3] && create_if_block$4(ctx)
  );
  function deviceselect_micDevices_binding(value) {
    ctx[23](value);
  }
  let deviceselect_props = { i18n: (
    /*i18n*/
    ctx[1]
  ) };
  if (
    /*micDevices*/
    ctx[5] !== void 0
  ) {
    deviceselect_props.micDevices = /*micDevices*/
    ctx[5];
  }
  deviceselect = new DeviceSelect({ props: deviceselect_props });
  binding_callbacks.push(() => bind(deviceselect, "micDevices", deviceselect_micDevices_binding));
  return {
    c() {
      div1 = element("div");
      div0 = element("div");
      button0 = element("button");
      t0 = text(t0_value);
      t1 = space();
      button1 = element("button");
      t2 = text(t2_value);
      t3 = space();
      button2 = element("button");
      t4 = text(t4_value);
      t5 = space();
      button3 = element("button");
      create_component(pause.$$.fragment);
      t6 = space();
      button4 = element("button");
      t7 = text(t7_value);
      t8 = space();
      if (if_block)
        if_block.c();
      t9 = space();
      create_component(deviceselect.$$.fragment);
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      button0 = claim_element(div0_nodes, "BUTTON", { class: true });
      var button0_nodes = children(button0);
      t0 = claim_text(button0_nodes, t0_value);
      button0_nodes.forEach(detach);
      t1 = claim_space(div0_nodes);
      button1 = claim_element(div0_nodes, "BUTTON", { class: true });
      var button1_nodes = children(button1);
      t2 = claim_text(button1_nodes, t2_value);
      button1_nodes.forEach(detach);
      t3 = claim_space(div0_nodes);
      button2 = claim_element(div0_nodes, "BUTTON", { id: true, class: true });
      var button2_nodes = children(button2);
      t4 = claim_text(button2_nodes, t4_value);
      button2_nodes.forEach(detach);
      t5 = claim_space(div0_nodes);
      button3 = claim_element(div0_nodes, "BUTTON", { "aria-label": true, class: true });
      var button3_nodes = children(button3);
      claim_component(pause.$$.fragment, button3_nodes);
      button3_nodes.forEach(detach);
      t6 = claim_space(div0_nodes);
      button4 = claim_element(div0_nodes, "BUTTON", { class: true });
      var button4_nodes = children(button4);
      t7 = claim_text(button4_nodes, t7_value);
      button4_nodes.forEach(detach);
      t8 = claim_space(div0_nodes);
      if (if_block)
        if_block.l(div0_nodes);
      div0_nodes.forEach(detach);
      t9 = claim_space(div1_nodes);
      claim_component(deviceselect.$$.fragment, div1_nodes);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button0, "class", "record record-button svelte-11fum4b");
      attr(button1, "class", button1_class_value = "stop-button " + /*record*/
      (ctx[0].isPaused() ? "stop-button-paused" : "") + " svelte-11fum4b");
      attr(button2, "id", "stop-paused");
      attr(button2, "class", "stop-button-paused svelte-11fum4b");
      attr(button3, "aria-label", "pause");
      attr(button3, "class", "pause-button svelte-11fum4b");
      attr(button4, "class", "resume-button svelte-11fum4b");
      attr(div0, "class", "wrapper svelte-11fum4b");
      attr(div1, "class", "controls svelte-11fum4b");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, div0);
      append_hydration(div0, button0);
      append_hydration(button0, t0);
      ctx[13](button0);
      append_hydration(div0, t1);
      append_hydration(div0, button1);
      append_hydration(button1, t2);
      ctx[15](button1);
      append_hydration(div0, t3);
      append_hydration(div0, button2);
      append_hydration(button2, t4);
      ctx[17](button2);
      append_hydration(div0, t5);
      append_hydration(div0, button3);
      mount_component(pause, button3, null);
      ctx[19](button3);
      append_hydration(div0, t6);
      append_hydration(div0, button4);
      append_hydration(button4, t7);
      ctx[21](button4);
      append_hydration(div0, t8);
      if (if_block)
        if_block.m(div0, null);
      append_hydration(div1, t9);
      mount_component(deviceselect, div1, null);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            button0,
            "click",
            /*click_handler*/
            ctx[14]
          ),
          listen(
            button1,
            "click",
            /*click_handler_1*/
            ctx[16]
          ),
          listen(
            button2,
            "click",
            /*click_handler_2*/
            ctx[18]
          ),
          listen(
            button3,
            "click",
            /*click_handler_3*/
            ctx[20]
          ),
          listen(
            button4,
            "click",
            /*click_handler_4*/
            ctx[22]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if ((!current || dirty & /*i18n*/
      2) && t0_value !== (t0_value = /*i18n*/
      ctx2[1]("audio.record") + ""))
        set_data(t0, t0_value);
      if ((!current || dirty & /*i18n*/
      2) && t2_value !== (t2_value = /*i18n*/
      ctx2[1]("audio.stop") + ""))
        set_data(t2, t2_value);
      if (!current || dirty & /*record*/
      1 && button1_class_value !== (button1_class_value = "stop-button " + /*record*/
      (ctx2[0].isPaused() ? "stop-button-paused" : "") + " svelte-11fum4b")) {
        attr(button1, "class", button1_class_value);
      }
      if ((!current || dirty & /*i18n*/
      2) && t4_value !== (t4_value = /*i18n*/
      ctx2[1]("audio.stop") + ""))
        set_data(t4, t4_value);
      if ((!current || dirty & /*i18n*/
      2) && t7_value !== (t7_value = /*i18n*/
      ctx2[1]("audio.resume") + ""))
        set_data(t7, t7_value);
      if (
        /*timing*/
        ctx2[4] && !/*show_recording_waveform*/
        ctx2[3]
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block$4(ctx2);
          if_block.c();
          if_block.m(div0, null);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      const deviceselect_changes = {};
      if (dirty & /*i18n*/
      2)
        deviceselect_changes.i18n = /*i18n*/
        ctx2[1];
      if (!updating_micDevices && dirty & /*micDevices*/
      32) {
        updating_micDevices = true;
        deviceselect_changes.micDevices = /*micDevices*/
        ctx2[5];
        add_flush_callback(() => updating_micDevices = false);
      }
      deviceselect.$set(deviceselect_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(pause.$$.fragment, local);
      transition_in(deviceselect.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(pause.$$.fragment, local);
      transition_out(deviceselect.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      ctx[13](null);
      ctx[15](null);
      ctx[17](null);
      destroy_component(pause);
      ctx[19](null);
      ctx[21](null);
      if (if_block)
        if_block.d();
      destroy_component(deviceselect);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$4($$self, $$props, $$invalidate) {
  let { record } = $$props;
  let { i18n } = $$props;
  let { recording = false } = $$props;
  let micDevices = [];
  let recordButton;
  let pauseButton;
  let resumeButton;
  let stopButton;
  let stopButtonPaused;
  let recording_ongoing = false;
  let { record_time } = $$props;
  let { show_recording_waveform } = $$props;
  let { timing = false } = $$props;
  function button0_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      recordButton = $$value;
      $$invalidate(6, recordButton), $$invalidate(0, record);
    });
  }
  const click_handler = () => record.startRecording();
  function button1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      stopButton = $$value;
      $$invalidate(9, stopButton), $$invalidate(0, record);
    });
  }
  const click_handler_1 = () => {
    if (record.isPaused()) {
      record.resumeRecording();
      record.stopRecording();
    }
    record.stopRecording();
  };
  function button2_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      stopButtonPaused = $$value;
      $$invalidate(10, stopButtonPaused), $$invalidate(0, record);
    });
  }
  const click_handler_2 = () => {
    if (record.isPaused()) {
      record.resumeRecording();
      record.stopRecording();
    }
    record.stopRecording();
  };
  function button3_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      pauseButton = $$value;
      $$invalidate(7, pauseButton), $$invalidate(0, record);
    });
  }
  const click_handler_3 = () => record.pauseRecording();
  function button4_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      resumeButton = $$value;
      $$invalidate(8, resumeButton), $$invalidate(0, record);
    });
  }
  const click_handler_4 = () => record.resumeRecording();
  function deviceselect_micDevices_binding(value) {
    micDevices = value;
    $$invalidate(5, micDevices);
  }
  $$self.$$set = ($$props2) => {
    if ("record" in $$props2)
      $$invalidate(0, record = $$props2.record);
    if ("i18n" in $$props2)
      $$invalidate(1, i18n = $$props2.i18n);
    if ("recording" in $$props2)
      $$invalidate(11, recording = $$props2.recording);
    if ("record_time" in $$props2)
      $$invalidate(2, record_time = $$props2.record_time);
    if ("show_recording_waveform" in $$props2)
      $$invalidate(3, show_recording_waveform = $$props2.show_recording_waveform);
    if ("timing" in $$props2)
      $$invalidate(4, timing = $$props2.timing);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*record*/
    1) {
      record.on("record-start", () => {
        record.startMic();
        $$invalidate(6, recordButton.style.display = "none", recordButton);
        $$invalidate(9, stopButton.style.display = "flex", stopButton);
        $$invalidate(7, pauseButton.style.display = "block", pauseButton);
      });
    }
    if ($$self.$$.dirty & /*record*/
    1) {
      record.on("record-end", () => {
        if (record.isPaused()) {
          record.resumeRecording();
          record.stopRecording();
        }
        record.stopMic();
        $$invalidate(6, recordButton.style.display = "flex", recordButton);
        $$invalidate(9, stopButton.style.display = "none", stopButton);
        $$invalidate(7, pauseButton.style.display = "none", pauseButton);
        $$invalidate(6, recordButton.disabled = false, recordButton);
      });
    }
    if ($$self.$$.dirty & /*record*/
    1) {
      record.on("record-pause", () => {
        $$invalidate(7, pauseButton.style.display = "none", pauseButton);
        $$invalidate(8, resumeButton.style.display = "block", resumeButton);
        $$invalidate(9, stopButton.style.display = "none", stopButton);
        $$invalidate(10, stopButtonPaused.style.display = "flex", stopButtonPaused);
      });
    }
    if ($$self.$$.dirty & /*record*/
    1) {
      record.on("record-resume", () => {
        $$invalidate(7, pauseButton.style.display = "block", pauseButton);
        $$invalidate(8, resumeButton.style.display = "none", resumeButton);
        $$invalidate(6, recordButton.style.display = "none", recordButton);
        $$invalidate(9, stopButton.style.display = "flex", stopButton);
        $$invalidate(10, stopButtonPaused.style.display = "none", stopButtonPaused);
      });
    }
    if ($$self.$$.dirty & /*recording, recording_ongoing, record*/
    6145) {
      if (recording && !recording_ongoing) {
        record.startRecording();
        $$invalidate(12, recording_ongoing = true);
      } else {
        record.stopRecording();
        $$invalidate(12, recording_ongoing = false);
      }
    }
  };
  return [
    record,
    i18n,
    record_time,
    show_recording_waveform,
    timing,
    micDevices,
    recordButton,
    pauseButton,
    resumeButton,
    stopButton,
    stopButtonPaused,
    recording,
    recording_ongoing,
    button0_binding,
    click_handler,
    button1_binding,
    click_handler_1,
    button2_binding,
    click_handler_2,
    button3_binding,
    click_handler_3,
    button4_binding,
    click_handler_4,
    deviceselect_micDevices_binding
  ];
}
class WaveformRecordControls extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$4, create_fragment$4, safe_not_equal, {
      record: 0,
      i18n: 1,
      recording: 11,
      record_time: 2,
      show_recording_waveform: 3,
      timing: 4
    });
  }
}
function create_if_block_2$2(ctx) {
  let div1;
  let time;
  let textContent = "0:00";
  let t1;
  let div0;
  let t2;
  let if_block0 = (
    /*mode*/
    ctx[0] === "edit" && /*trimDuration*/
    ctx[17] > 0 && create_if_block_4(ctx)
  );
  function select_block_type(ctx2, dirty) {
    if (
      /*timing*/
      ctx2[16]
    )
      return create_if_block_3$1;
    return create_else_block$3;
  }
  let current_block_type = select_block_type(ctx);
  let if_block1 = current_block_type(ctx);
  return {
    c() {
      div1 = element("div");
      time = element("time");
      time.textContent = textContent;
      t1 = space();
      div0 = element("div");
      if (if_block0)
        if_block0.c();
      t2 = space();
      if_block1.c();
      this.h();
    },
    l(nodes) {
      div1 = claim_element(nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      time = claim_element(div1_nodes, "TIME", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(time) !== "svelte-1rda9am")
        time.textContent = textContent;
      t1 = claim_space(div1_nodes);
      div0 = claim_element(div1_nodes, "DIV", {});
      var div0_nodes = children(div0);
      if (if_block0)
        if_block0.l(div0_nodes);
      t2 = claim_space(div0_nodes);
      if_block1.l(div0_nodes);
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "class", "time svelte-9n45fh");
      attr(div1, "class", "timestamps svelte-9n45fh");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      append_hydration(div1, time);
      ctx[23](time);
      append_hydration(div1, t1);
      append_hydration(div1, div0);
      if (if_block0)
        if_block0.m(div0, null);
      append_hydration(div0, t2);
      if_block1.m(div0, null);
    },
    p(ctx2, dirty) {
      if (
        /*mode*/
        ctx2[0] === "edit" && /*trimDuration*/
        ctx2[17] > 0
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          if_block0.m(div0, t2);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (current_block_type === (current_block_type = select_block_type(ctx2)) && if_block1) {
        if_block1.p(ctx2, dirty);
      } else {
        if_block1.d(1);
        if_block1 = current_block_type(ctx2);
        if (if_block1) {
          if_block1.c();
          if_block1.m(div0, null);
        }
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      ctx[23](null);
      if (if_block0)
        if_block0.d();
      if_block1.d();
    }
  };
}
function create_if_block_4(ctx) {
  let time;
  let t_value = format_time(
    /*trimDuration*/
    ctx[17]
  ) + "";
  let t2;
  return {
    c() {
      time = element("time");
      t2 = text(t_value);
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { class: true });
      var time_nodes = children(time);
      t2 = claim_text(time_nodes, t_value);
      time_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "class", "trim-duration svelte-9n45fh");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      append_hydration(time, t2);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*trimDuration*/
      131072 && t_value !== (t_value = format_time(
        /*trimDuration*/
        ctx2[17]
      ) + ""))
        set_data(t2, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(time);
      }
    }
  };
}
function create_else_block$3(ctx) {
  let time;
  let textContent = "0:00";
  return {
    c() {
      time = element("time");
      time.textContent = textContent;
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(time) !== "svelte-1llsiqq")
        time.textContent = textContent;
      this.h();
    },
    h() {
      attr(time, "class", "duration svelte-9n45fh");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      ctx[24](time);
    },
    p: noop,
    d(detaching) {
      if (detaching) {
        detach(time);
      }
      ctx[24](null);
    }
  };
}
function create_if_block_3$1(ctx) {
  let time;
  let t_value = format_time(
    /*seconds*/
    ctx[15]
  ) + "";
  let t2;
  return {
    c() {
      time = element("time");
      t2 = text(t_value);
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { class: true });
      var time_nodes = children(time);
      t2 = claim_text(time_nodes, t_value);
      time_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "class", "duration svelte-9n45fh");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      append_hydration(time, t2);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*seconds*/
      32768 && t_value !== (t_value = format_time(
        /*seconds*/
        ctx2[15]
      ) + ""))
        set_data(t2, t_value);
    },
    d(detaching) {
      if (detaching) {
        detach(time);
      }
    }
  };
}
function create_if_block_1$2(ctx) {
  let waveformrecordcontrols;
  let updating_record;
  let current;
  function waveformrecordcontrols_record_binding(value) {
    ctx[25](value);
  }
  let waveformrecordcontrols_props = {
    i18n: (
      /*i18n*/
      ctx[1]
    ),
    timing: (
      /*timing*/
      ctx[16]
    ),
    recording: (
      /*recording*/
      ctx[5]
    ),
    show_recording_waveform: (
      /*waveform_options*/
      ctx[2].show_recording_waveform
    ),
    record_time: format_time(
      /*seconds*/
      ctx[15]
    )
  };
  if (
    /*record*/
    ctx[7] !== void 0
  ) {
    waveformrecordcontrols_props.record = /*record*/
    ctx[7];
  }
  waveformrecordcontrols = new WaveformRecordControls({ props: waveformrecordcontrols_props });
  binding_callbacks.push(() => bind(waveformrecordcontrols, "record", waveformrecordcontrols_record_binding));
  return {
    c() {
      create_component(waveformrecordcontrols.$$.fragment);
    },
    l(nodes) {
      claim_component(waveformrecordcontrols.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(waveformrecordcontrols, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const waveformrecordcontrols_changes = {};
      if (dirty[0] & /*i18n*/
      2)
        waveformrecordcontrols_changes.i18n = /*i18n*/
        ctx2[1];
      if (dirty[0] & /*timing*/
      65536)
        waveformrecordcontrols_changes.timing = /*timing*/
        ctx2[16];
      if (dirty[0] & /*recording*/
      32)
        waveformrecordcontrols_changes.recording = /*recording*/
        ctx2[5];
      if (dirty[0] & /*waveform_options*/
      4)
        waveformrecordcontrols_changes.show_recording_waveform = /*waveform_options*/
        ctx2[2].show_recording_waveform;
      if (dirty[0] & /*seconds*/
      32768)
        waveformrecordcontrols_changes.record_time = format_time(
          /*seconds*/
          ctx2[15]
        );
      if (!updating_record && dirty[0] & /*record*/
      128) {
        updating_record = true;
        waveformrecordcontrols_changes.record = /*record*/
        ctx2[7];
        add_flush_callback(() => updating_record = false);
      }
      waveformrecordcontrols.$set(waveformrecordcontrols_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(waveformrecordcontrols.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(waveformrecordcontrols.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(waveformrecordcontrols, detaching);
    }
  };
}
function create_if_block$3(ctx) {
  let waveformcontrols;
  let updating_waveform;
  let updating_trimDuration;
  let updating_mode;
  let current;
  function waveformcontrols_waveform_binding(value) {
    ctx[26](value);
  }
  function waveformcontrols_trimDuration_binding(value) {
    ctx[27](value);
  }
  function waveformcontrols_mode_binding(value) {
    ctx[28](value);
  }
  let waveformcontrols_props = {
    container: (
      /*recordingContainer*/
      ctx[11]
    ),
    playing: (
      /*playing*/
      ctx[10]
    ),
    audio_duration: (
      /*audio_duration*/
      ctx[14]
    ),
    i18n: (
      /*i18n*/
      ctx[1]
    ),
    editable: (
      /*editable*/
      ctx[4]
    ),
    interactive: true,
    handle_trim_audio: (
      /*handle_trim_audio*/
      ctx[18]
    ),
    show_redo: true,
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[3]
    ),
    waveform_options: (
      /*waveform_options*/
      ctx[2]
    )
  };
  if (
    /*recordingWaveform*/
    ctx[6] !== void 0
  ) {
    waveformcontrols_props.waveform = /*recordingWaveform*/
    ctx[6];
  }
  if (
    /*trimDuration*/
    ctx[17] !== void 0
  ) {
    waveformcontrols_props.trimDuration = /*trimDuration*/
    ctx[17];
  }
  if (
    /*mode*/
    ctx[0] !== void 0
  ) {
    waveformcontrols_props.mode = /*mode*/
    ctx[0];
  }
  waveformcontrols = new WaveformControls({ props: waveformcontrols_props });
  binding_callbacks.push(() => bind(waveformcontrols, "waveform", waveformcontrols_waveform_binding));
  binding_callbacks.push(() => bind(waveformcontrols, "trimDuration", waveformcontrols_trimDuration_binding));
  binding_callbacks.push(() => bind(waveformcontrols, "mode", waveformcontrols_mode_binding));
  return {
    c() {
      create_component(waveformcontrols.$$.fragment);
    },
    l(nodes) {
      claim_component(waveformcontrols.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(waveformcontrols, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const waveformcontrols_changes = {};
      if (dirty[0] & /*recordingContainer*/
      2048)
        waveformcontrols_changes.container = /*recordingContainer*/
        ctx2[11];
      if (dirty[0] & /*playing*/
      1024)
        waveformcontrols_changes.playing = /*playing*/
        ctx2[10];
      if (dirty[0] & /*audio_duration*/
      16384)
        waveformcontrols_changes.audio_duration = /*audio_duration*/
        ctx2[14];
      if (dirty[0] & /*i18n*/
      2)
        waveformcontrols_changes.i18n = /*i18n*/
        ctx2[1];
      if (dirty[0] & /*editable*/
      16)
        waveformcontrols_changes.editable = /*editable*/
        ctx2[4];
      if (dirty[0] & /*handle_reset_value*/
      8)
        waveformcontrols_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[3];
      if (dirty[0] & /*waveform_options*/
      4)
        waveformcontrols_changes.waveform_options = /*waveform_options*/
        ctx2[2];
      if (!updating_waveform && dirty[0] & /*recordingWaveform*/
      64) {
        updating_waveform = true;
        waveformcontrols_changes.waveform = /*recordingWaveform*/
        ctx2[6];
        add_flush_callback(() => updating_waveform = false);
      }
      if (!updating_trimDuration && dirty[0] & /*trimDuration*/
      131072) {
        updating_trimDuration = true;
        waveformcontrols_changes.trimDuration = /*trimDuration*/
        ctx2[17];
        add_flush_callback(() => updating_trimDuration = false);
      }
      if (!updating_mode && dirty[0] & /*mode*/
      1) {
        updating_mode = true;
        waveformcontrols_changes.mode = /*mode*/
        ctx2[0];
        add_flush_callback(() => updating_mode = false);
      }
      waveformcontrols.$set(waveformcontrols_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(waveformcontrols.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(waveformcontrols.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(waveformcontrols, detaching);
    }
  };
}
function create_fragment$3(ctx) {
  let div2;
  let div0;
  let t0;
  let div1;
  let t1;
  let t2;
  let t3;
  let current;
  let if_block0 = (
    /*timing*/
    (ctx[16] || /*recordedAudio*/
    ctx[13]) && /*waveform_options*/
    ctx[2].show_recording_waveform && create_if_block_2$2(ctx)
  );
  let if_block1 = (
    /*microphoneContainer*/
    ctx[12] && !/*recordedAudio*/
    ctx[13] && create_if_block_1$2(ctx)
  );
  let if_block2 = (
    /*recordingWaveform*/
    ctx[6] && /*recordedAudio*/
    ctx[13] && create_if_block$3(ctx)
  );
  return {
    c() {
      div2 = element("div");
      div0 = element("div");
      t0 = space();
      div1 = element("div");
      t1 = space();
      if (if_block0)
        if_block0.c();
      t2 = space();
      if (if_block1)
        if_block1.c();
      t3 = space();
      if (if_block2)
        if_block2.c();
      this.h();
    },
    l(nodes) {
      div2 = claim_element(nodes, "DIV", { class: true });
      var div2_nodes = children(div2);
      div0 = claim_element(div2_nodes, "DIV", { class: true, "data-testid": true });
      children(div0).forEach(detach);
      t0 = claim_space(div2_nodes);
      div1 = claim_element(div2_nodes, "DIV", { "data-testid": true });
      children(div1).forEach(detach);
      t1 = claim_space(div2_nodes);
      if (if_block0)
        if_block0.l(div2_nodes);
      t2 = claim_space(div2_nodes);
      if (if_block1)
        if_block1.l(div2_nodes);
      t3 = claim_space(div2_nodes);
      if (if_block2)
        if_block2.l(div2_nodes);
      div2_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "microphone svelte-9n45fh");
      attr(div0, "data-testid", "microphone-waveform");
      attr(div1, "data-testid", "recording-waveform");
      attr(div2, "class", "component-wrapper svelte-9n45fh");
    },
    m(target, anchor) {
      insert_hydration(target, div2, anchor);
      append_hydration(div2, div0);
      ctx[21](div0);
      append_hydration(div2, t0);
      append_hydration(div2, div1);
      ctx[22](div1);
      append_hydration(div2, t1);
      if (if_block0)
        if_block0.m(div2, null);
      append_hydration(div2, t2);
      if (if_block1)
        if_block1.m(div2, null);
      append_hydration(div2, t3);
      if (if_block2)
        if_block2.m(div2, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*timing*/
        (ctx2[16] || /*recordedAudio*/
        ctx2[13]) && /*waveform_options*/
        ctx2[2].show_recording_waveform
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_2$2(ctx2);
          if_block0.c();
          if_block0.m(div2, t2);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
      }
      if (
        /*microphoneContainer*/
        ctx2[12] && !/*recordedAudio*/
        ctx2[13]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty[0] & /*microphoneContainer, recordedAudio*/
          12288) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_1$2(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(div2, t3);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
      if (
        /*recordingWaveform*/
        ctx2[6] && /*recordedAudio*/
        ctx2[13]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*recordingWaveform, recordedAudio*/
          8256) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block$3(ctx2);
          if_block2.c();
          transition_in(if_block2, 1);
          if_block2.m(div2, null);
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
      transition_in(if_block1);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(if_block1);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div2);
      }
      ctx[21](null);
      ctx[22](null);
      if (if_block0)
        if_block0.d();
      if (if_block1)
        if_block1.d();
      if (if_block2)
        if_block2.d();
    }
  };
}
function instance$3($$self, $$props, $$invalidate) {
  let { mode } = $$props;
  let { i18n } = $$props;
  let { dispatch_blob } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options = { show_recording_waveform: true } } = $$props;
  let { handle_reset_value } = $$props;
  let { editable = true } = $$props;
  let { recording = false } = $$props;
  let micWaveform;
  let recordingWaveform;
  let playing = false;
  let recordingContainer;
  let microphoneContainer;
  let record;
  let recordedAudio = null;
  let timeRef;
  let durationRef;
  let audio_duration;
  let seconds = 0;
  let interval;
  let timing = false;
  let trimDuration = 0;
  const start_interval = () => {
    clearInterval(interval);
    interval = setInterval(
      () => {
        $$invalidate(15, seconds++, seconds);
      },
      1e3
    );
  };
  const dispatch = createEventDispatcher();
  function record_start_callback() {
    start_interval();
    $$invalidate(16, timing = true);
    dispatch("start_recording");
    if (waveform_options.show_recording_waveform) {
      let waveformCanvas = microphoneContainer;
      if (waveformCanvas)
        waveformCanvas.style.display = "block";
    }
  }
  async function record_end_callback(blob) {
    $$invalidate(15, seconds = 0);
    $$invalidate(16, timing = false);
    clearInterval(interval);
    try {
      const array_buffer = await blob.arrayBuffer();
      const context = new AudioContext({ sampleRate: waveform_settings.sampleRate });
      const audio_buffer = await context.decodeAudioData(array_buffer);
      if (audio_buffer)
        await process_audio(audio_buffer).then(async (audio) => {
          await dispatch_blob([audio], "change");
          await dispatch_blob([audio], "stop_recording");
        });
    } catch (e2) {
      console.error(e2);
    }
  }
  const create_mic_waveform = () => {
    if (microphoneContainer)
      $$invalidate(12, microphoneContainer.innerHTML = "", microphoneContainer);
    if (micWaveform !== void 0)
      micWaveform.destroy();
    if (!microphoneContainer)
      return;
    micWaveform = WaveSurfer.create({
      ...waveform_settings,
      normalize: false,
      container: microphoneContainer
    });
    $$invalidate(7, record = micWaveform.registerPlugin(r.create()));
    record.startMic();
    record == null ? void 0 : record.on("record-end", record_end_callback);
    record == null ? void 0 : record.on("record-start", record_start_callback);
    record == null ? void 0 : record.on("record-pause", () => {
      dispatch("pause_recording");
      clearInterval(interval);
    });
    record == null ? void 0 : record.on("record-end", (blob) => {
      $$invalidate(13, recordedAudio = URL.createObjectURL(blob));
      const microphone = microphoneContainer;
      const recording2 = recordingContainer;
      if (microphone)
        microphone.style.display = "none";
      if (recording2 && recordedAudio) {
        recording2.innerHTML = "";
        create_recording_waveform();
      }
    });
  };
  const create_recording_waveform = () => {
    let recording2 = recordingContainer;
    if (!recordedAudio || !recording2)
      return;
    $$invalidate(6, recordingWaveform = WaveSurfer.create({
      container: recording2,
      url: recordedAudio,
      ...waveform_settings
    }));
  };
  const handle_trim_audio = async (start, end) => {
    $$invalidate(0, mode = "edit");
    const decodedData = recordingWaveform.getDecodedData();
    if (decodedData)
      await process_audio(decodedData, start, end).then(async (trimmedAudio) => {
        await dispatch_blob([trimmedAudio], "change");
        await dispatch_blob([trimmedAudio], "stop_recording");
        recordingWaveform.destroy();
        create_recording_waveform();
      });
    dispatch("edit");
  };
  onMount(() => {
    create_mic_waveform();
    window.addEventListener("keydown", (e2) => {
      if (e2.key === "ArrowRight") {
        skip_audio(recordingWaveform, 0.1);
      } else if (e2.key === "ArrowLeft") {
        skip_audio(recordingWaveform, -0.1);
      }
    });
  });
  function div0_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      microphoneContainer = $$value;
      $$invalidate(12, microphoneContainer);
    });
  }
  function div1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      recordingContainer = $$value;
      $$invalidate(11, recordingContainer);
    });
  }
  function time_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      timeRef = $$value;
      $$invalidate(8, timeRef), $$invalidate(6, recordingWaveform);
    });
  }
  function time_binding_1($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      durationRef = $$value;
      $$invalidate(9, durationRef), $$invalidate(6, recordingWaveform);
    });
  }
  function waveformrecordcontrols_record_binding(value) {
    record = value;
    $$invalidate(7, record);
  }
  function waveformcontrols_waveform_binding(value) {
    recordingWaveform = value;
    $$invalidate(6, recordingWaveform);
  }
  function waveformcontrols_trimDuration_binding(value) {
    trimDuration = value;
    $$invalidate(17, trimDuration);
  }
  function waveformcontrols_mode_binding(value) {
    mode = value;
    $$invalidate(0, mode);
  }
  $$self.$$set = ($$props2) => {
    if ("mode" in $$props2)
      $$invalidate(0, mode = $$props2.mode);
    if ("i18n" in $$props2)
      $$invalidate(1, i18n = $$props2.i18n);
    if ("dispatch_blob" in $$props2)
      $$invalidate(19, dispatch_blob = $$props2.dispatch_blob);
    if ("waveform_settings" in $$props2)
      $$invalidate(20, waveform_settings = $$props2.waveform_settings);
    if ("waveform_options" in $$props2)
      $$invalidate(2, waveform_options = $$props2.waveform_options);
    if ("handle_reset_value" in $$props2)
      $$invalidate(3, handle_reset_value = $$props2.handle_reset_value);
    if ("editable" in $$props2)
      $$invalidate(4, editable = $$props2.editable);
    if ("recording" in $$props2)
      $$invalidate(5, recording = $$props2.recording);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*record*/
    128) {
      record == null ? void 0 : record.on("record-resume", () => {
        start_interval();
      });
    }
    if ($$self.$$.dirty[0] & /*recordingWaveform, durationRef*/
    576) {
      recordingWaveform == null ? void 0 : recordingWaveform.on("decode", (duration) => {
        $$invalidate(14, audio_duration = duration);
        durationRef && $$invalidate(9, durationRef.textContent = format_time(duration), durationRef);
      });
    }
    if ($$self.$$.dirty[0] & /*recordingWaveform, timeRef*/
    320) {
      recordingWaveform == null ? void 0 : recordingWaveform.on("timeupdate", (currentTime) => timeRef && $$invalidate(8, timeRef.textContent = format_time(currentTime), timeRef));
    }
    if ($$self.$$.dirty[0] & /*recordingWaveform*/
    64) {
      recordingWaveform == null ? void 0 : recordingWaveform.on("pause", () => {
        dispatch("pause");
        $$invalidate(10, playing = false);
      });
    }
    if ($$self.$$.dirty[0] & /*recordingWaveform*/
    64) {
      recordingWaveform == null ? void 0 : recordingWaveform.on("play", () => {
        dispatch("play");
        $$invalidate(10, playing = true);
      });
    }
    if ($$self.$$.dirty[0] & /*recordingWaveform*/
    64) {
      recordingWaveform == null ? void 0 : recordingWaveform.on("finish", () => {
        dispatch("stop");
        $$invalidate(10, playing = false);
      });
    }
  };
  return [
    mode,
    i18n,
    waveform_options,
    handle_reset_value,
    editable,
    recording,
    recordingWaveform,
    record,
    timeRef,
    durationRef,
    playing,
    recordingContainer,
    microphoneContainer,
    recordedAudio,
    audio_duration,
    seconds,
    timing,
    trimDuration,
    handle_trim_audio,
    dispatch_blob,
    waveform_settings,
    div0_binding,
    div1_binding,
    time_binding,
    time_binding_1,
    waveformrecordcontrols_record_binding,
    waveformcontrols_waveform_binding,
    waveformcontrols_trimDuration_binding,
    waveformcontrols_mode_binding
  ];
}
class AudioRecorder extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$3,
      create_fragment$3,
      safe_not_equal,
      {
        mode: 0,
        i18n: 1,
        dispatch_blob: 19,
        waveform_settings: 20,
        waveform_options: 2,
        handle_reset_value: 3,
        editable: 4,
        recording: 5
      },
      null,
      [-1, -1]
    );
  }
}
function create_if_block_2$1(ctx) {
  let div;
  return {
    c() {
      div = element("div");
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", {});
      children(div).forEach(detach);
      this.h();
    },
    h() {
      set_style(
        div,
        "display",
        /*recording*/
        ctx[0] ? "block" : "none"
      );
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      ctx[11](div);
    },
    p(ctx2, dirty) {
      if (dirty & /*recording*/
      1) {
        set_style(
          div,
          "display",
          /*recording*/
          ctx2[0] ? "block" : "none"
        );
      }
    },
    d(detaching) {
      if (detaching) {
        detach(div);
      }
      ctx[11](null);
    }
  };
}
function create_else_block$2(ctx) {
  let button;
  let span1;
  let textContent = `<span class="dot"></span>`;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[4]("audio.record") + ""
  );
  let t1;
  let mounted;
  let dispose;
  return {
    c() {
      button = element("button");
      span1 = element("span");
      span1.innerHTML = textContent;
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      span1 = claim_element(button_nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span1) !== "svelte-1dwz2xe")
        span1.innerHTML = textContent;
      t0 = claim_space(button_nodes);
      t1 = claim_text(button_nodes, t1_value);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span1, "class", "record-icon");
      attr(button, "class", "record-button svelte-1fz19cj");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, span1);
      append_hydration(button, t0);
      append_hydration(button, t1);
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_2*/
          ctx[14]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*i18n*/
      16 && t1_value !== (t1_value = /*i18n*/
      ctx2[4]("audio.record") + ""))
        set_data(t1, t1_value);
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_if_block_1$1(ctx) {
  let button;
  let div;
  let spinner;
  let t0;
  let t1_value = (
    /*i18n*/
    ctx[4]("audio.waiting") + ""
  );
  let t1;
  let current;
  let mounted;
  let dispose;
  spinner = new Spinner({});
  return {
    c() {
      button = element("button");
      div = element("div");
      create_component(spinner.$$.fragment);
      t0 = space();
      t1 = text(t1_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      div = claim_element(button_nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(spinner.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      t0 = claim_space(button_nodes);
      t1 = claim_text(button_nodes, t1_value);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "icon svelte-1fz19cj");
      attr(button, "class", "spinner-button svelte-1fz19cj");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, div);
      mount_component(spinner, div, null);
      append_hydration(button, t0);
      append_hydration(button, t1);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_1*/
          ctx[13]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if ((!current || dirty & /*i18n*/
      16) && t1_value !== (t1_value = /*i18n*/
      ctx2[4]("audio.waiting") + ""))
        set_data(t1, t1_value);
    },
    i(local) {
      if (current)
        return;
      transition_in(spinner.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(spinner.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(spinner);
      mounted = false;
      dispose();
    }
  };
}
function create_if_block$2(ctx) {
  let button;
  let span2;
  let textContent = `<span class="pinger"></span> <span class="dot"></span>`;
  let t1;
  let t2_value = (
    /*paused_recording*/
    (ctx[1] ? (
      /*i18n*/
      ctx[4]("audio.pause")
    ) : (
      /*i18n*/
      ctx[4]("audio.stop")
    )) + ""
  );
  let t2;
  let button_class_value;
  let mounted;
  let dispose;
  return {
    c() {
      button = element("button");
      span2 = element("span");
      span2.innerHTML = textContent;
      t1 = space();
      t2 = text(t2_value);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true });
      var button_nodes = children(button);
      span2 = claim_element(button_nodes, "SPAN", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(span2) !== "svelte-bla7qm")
        span2.innerHTML = textContent;
      t1 = claim_space(button_nodes);
      t2 = claim_text(button_nodes, t2_value);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(span2, "class", "record-icon");
      attr(button, "class", button_class_value = null_to_empty(
        /*paused_recording*/
        ctx[1] ? "stop-button-paused" : "stop-button"
      ) + " svelte-1fz19cj");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      append_hydration(button, span2);
      append_hydration(button, t1);
      append_hydration(button, t2);
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler*/
          ctx[12]
        );
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (dirty & /*paused_recording, i18n*/
      18 && t2_value !== (t2_value = /*paused_recording*/
      (ctx2[1] ? (
        /*i18n*/
        ctx2[4]("audio.pause")
      ) : (
        /*i18n*/
        ctx2[4]("audio.stop")
      )) + ""))
        set_data(t2, t2_value);
      if (dirty & /*paused_recording*/
      2 && button_class_value !== (button_class_value = null_to_empty(
        /*paused_recording*/
        ctx2[1] ? "stop-button-paused" : "stop-button"
      ) + " svelte-1fz19cj")) {
        attr(button, "class", button_class_value);
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      mounted = false;
      dispose();
    }
  };
}
function create_fragment$2(ctx) {
  let div1;
  let t0;
  let div0;
  let current_block_type_index;
  let if_block1;
  let t1;
  let deviceselect;
  let updating_micDevices;
  let current;
  let if_block0 = (
    /*waveform_options*/
    ctx[5].show_recording_waveform && create_if_block_2$1(ctx)
  );
  const if_block_creators = [create_if_block$2, create_if_block_1$1, create_else_block$2];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*recording*/
      ctx2[0] && !/*waiting*/
      ctx2[6]
    )
      return 0;
    if (
      /*recording*/
      ctx2[0] && /*waiting*/
      ctx2[6]
    )
      return 1;
    return 2;
  }
  current_block_type_index = select_block_type(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  function deviceselect_micDevices_binding(value) {
    ctx[15](value);
  }
  let deviceselect_props = { i18n: (
    /*i18n*/
    ctx[4]
  ) };
  if (
    /*micDevices*/
    ctx[9] !== void 0
  ) {
    deviceselect_props.micDevices = /*micDevices*/
    ctx[9];
  }
  deviceselect = new DeviceSelect({ props: deviceselect_props });
  binding_callbacks.push(() => bind(deviceselect, "micDevices", deviceselect_micDevices_binding));
  return {
    c() {
      div1 = element("div");
      if (if_block0)
        if_block0.c();
      t0 = space();
      div0 = element("div");
      if_block1.c();
      t1 = space();
      create_component(deviceselect.$$.fragment);
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
      if_block1.l(div0_nodes);
      t1 = claim_space(div0_nodes);
      claim_component(deviceselect.$$.fragment, div0_nodes);
      div0_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "class", "controls svelte-1fz19cj");
      attr(div1, "class", "mic-wrap svelte-1fz19cj");
    },
    m(target, anchor) {
      insert_hydration(target, div1, anchor);
      if (if_block0)
        if_block0.m(div1, null);
      append_hydration(div1, t0);
      append_hydration(div1, div0);
      if_blocks[current_block_type_index].m(div0, null);
      append_hydration(div0, t1);
      mount_component(deviceselect, div0, null);
      current = true;
    },
    p(ctx2, [dirty]) {
      if (
        /*waveform_options*/
        ctx2[5].show_recording_waveform
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
        } else {
          if_block0 = create_if_block_2$1(ctx2);
          if_block0.c();
          if_block0.m(div1, t0);
        }
      } else if (if_block0) {
        if_block0.d(1);
        if_block0 = null;
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
        if_block1.m(div0, t1);
      }
      const deviceselect_changes = {};
      if (dirty & /*i18n*/
      16)
        deviceselect_changes.i18n = /*i18n*/
        ctx2[4];
      if (!updating_micDevices && dirty & /*micDevices*/
      512) {
        updating_micDevices = true;
        deviceselect_changes.micDevices = /*micDevices*/
        ctx2[9];
        add_flush_callback(() => updating_micDevices = false);
      }
      deviceselect.$set(deviceselect_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block1);
      transition_in(deviceselect.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(if_block1);
      transition_out(deviceselect.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div1);
      }
      if (if_block0)
        if_block0.d();
      if_blocks[current_block_type_index].d();
      destroy_component(deviceselect);
    }
  };
}
function instance$2($$self, $$props, $$invalidate) {
  let { recording = false } = $$props;
  let { paused_recording = false } = $$props;
  let { stop } = $$props;
  let { record } = $$props;
  let { i18n } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options = { show_recording_waveform: true } } = $$props;
  let { waiting = false } = $$props;
  let micWaveform;
  let waveformRecord;
  let microphoneContainer;
  let micDevices = [];
  onMount(() => {
    create_mic_waveform();
  });
  const create_mic_waveform = () => {
    if (micWaveform !== void 0)
      micWaveform.destroy();
    if (!microphoneContainer)
      return;
    micWaveform = WaveSurfer.create({
      ...waveform_settings,
      height: 100,
      container: microphoneContainer
    });
    $$invalidate(7, waveformRecord = micWaveform.registerPlugin(r.create()));
  };
  function div_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      microphoneContainer = $$value;
      $$invalidate(8, microphoneContainer);
    });
  }
  const click_handler = () => {
    waveformRecord == null ? void 0 : waveformRecord.stopMic();
    stop();
  };
  const click_handler_1 = () => {
    stop();
  };
  const click_handler_2 = () => {
    waveformRecord == null ? void 0 : waveformRecord.startMic();
    record();
  };
  function deviceselect_micDevices_binding(value) {
    micDevices = value;
    $$invalidate(9, micDevices);
  }
  $$self.$$set = ($$props2) => {
    if ("recording" in $$props2)
      $$invalidate(0, recording = $$props2.recording);
    if ("paused_recording" in $$props2)
      $$invalidate(1, paused_recording = $$props2.paused_recording);
    if ("stop" in $$props2)
      $$invalidate(2, stop = $$props2.stop);
    if ("record" in $$props2)
      $$invalidate(3, record = $$props2.record);
    if ("i18n" in $$props2)
      $$invalidate(4, i18n = $$props2.i18n);
    if ("waveform_settings" in $$props2)
      $$invalidate(10, waveform_settings = $$props2.waveform_settings);
    if ("waveform_options" in $$props2)
      $$invalidate(5, waveform_options = $$props2.waveform_options);
    if ("waiting" in $$props2)
      $$invalidate(6, waiting = $$props2.waiting);
  };
  return [
    recording,
    paused_recording,
    stop,
    record,
    i18n,
    waveform_options,
    waiting,
    waveformRecord,
    microphoneContainer,
    micDevices,
    waveform_settings,
    div_binding,
    click_handler,
    click_handler_1,
    click_handler_2,
    deviceselect_micDevices_binding
  ];
}
class StreamAudio extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$2, create_fragment$2, safe_not_equal, {
      recording: 0,
      paused_recording: 1,
      stop: 2,
      record: 3,
      i18n: 4,
      waveform_settings: 10,
      waveform_options: 5,
      waiting: 6
    });
  }
}
function create_else_block_1(ctx) {
  let modifyupload;
  let t2;
  let audioplayer;
  let updating_mode;
  let current;
  modifyupload = new ModifyUpload({
    props: {
      i18n: (
        /*i18n*/
        ctx[12]
      ),
      download: (
        /*show_download_button*/
        ctx[9] ? (
          /*value*/
          ctx[2].url
        ) : null
      )
    }
  });
  modifyupload.$on(
    "clear",
    /*clear*/
    ctx[27]
  );
  modifyupload.$on(
    "edit",
    /*edit_handler_1*/
    ctx[46]
  );
  function audioplayer_mode_binding(value) {
    ctx[47](value);
  }
  let audioplayer_props = {
    value: (
      /*value*/
      ctx[2]
    ),
    label: (
      /*label*/
      ctx[5]
    ),
    i18n: (
      /*i18n*/
      ctx[12]
    ),
    dispatch_blob: (
      /*dispatch_blob*/
      ctx[25]
    ),
    waveform_settings: (
      /*waveform_settings*/
      ctx[13]
    ),
    waveform_options: (
      /*waveform_options*/
      ctx[15]
    ),
    trim_region_settings: (
      /*trim_region_settings*/
      ctx[14]
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[16]
    ),
    editable: (
      /*editable*/
      ctx[17]
    ),
    loop: (
      /*loop*/
      ctx[7]
    ),
    interactive: true
  };
  if (
    /*mode*/
    ctx[23] !== void 0
  ) {
    audioplayer_props.mode = /*mode*/
    ctx[23];
  }
  audioplayer = new AudioPlayer({ props: audioplayer_props });
  binding_callbacks.push(() => bind(audioplayer, "mode", audioplayer_mode_binding));
  audioplayer.$on(
    "stop",
    /*stop_handler*/
    ctx[48]
  );
  audioplayer.$on(
    "play",
    /*play_handler*/
    ctx[49]
  );
  audioplayer.$on(
    "pause",
    /*pause_handler*/
    ctx[50]
  );
  audioplayer.$on(
    "edit",
    /*edit_handler*/
    ctx[51]
  );
  return {
    c() {
      create_component(modifyupload.$$.fragment);
      t2 = space();
      create_component(audioplayer.$$.fragment);
    },
    l(nodes) {
      claim_component(modifyupload.$$.fragment, nodes);
      t2 = claim_space(nodes);
      claim_component(audioplayer.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(modifyupload, target, anchor);
      insert_hydration(target, t2, anchor);
      mount_component(audioplayer, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const modifyupload_changes = {};
      if (dirty[0] & /*i18n*/
      4096)
        modifyupload_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*show_download_button, value*/
      516)
        modifyupload_changes.download = /*show_download_button*/
        ctx2[9] ? (
          /*value*/
          ctx2[2].url
        ) : null;
      modifyupload.$set(modifyupload_changes);
      const audioplayer_changes = {};
      if (dirty[0] & /*value*/
      4)
        audioplayer_changes.value = /*value*/
        ctx2[2];
      if (dirty[0] & /*label*/
      32)
        audioplayer_changes.label = /*label*/
        ctx2[5];
      if (dirty[0] & /*i18n*/
      4096)
        audioplayer_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*waveform_settings*/
      8192)
        audioplayer_changes.waveform_settings = /*waveform_settings*/
        ctx2[13];
      if (dirty[0] & /*waveform_options*/
      32768)
        audioplayer_changes.waveform_options = /*waveform_options*/
        ctx2[15];
      if (dirty[0] & /*trim_region_settings*/
      16384)
        audioplayer_changes.trim_region_settings = /*trim_region_settings*/
        ctx2[14];
      if (dirty[0] & /*handle_reset_value*/
      65536)
        audioplayer_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[16];
      if (dirty[0] & /*editable*/
      131072)
        audioplayer_changes.editable = /*editable*/
        ctx2[17];
      if (dirty[0] & /*loop*/
      128)
        audioplayer_changes.loop = /*loop*/
        ctx2[7];
      if (!updating_mode && dirty[0] & /*mode*/
      8388608) {
        updating_mode = true;
        audioplayer_changes.mode = /*mode*/
        ctx2[23];
        add_flush_callback(() => updating_mode = false);
      }
      audioplayer.$set(audioplayer_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(modifyupload.$$.fragment, local);
      transition_in(audioplayer.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(modifyupload.$$.fragment, local);
      transition_out(audioplayer.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
      }
      destroy_component(modifyupload, detaching);
      destroy_component(audioplayer, detaching);
    }
  };
}
function create_if_block$1(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block_1, create_if_block_3];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*active_source*/
      ctx2[3] === "microphone"
    )
      return 0;
    if (
      /*active_source*/
      ctx2[3] === "upload"
    )
      return 1;
    return -1;
  }
  if (~(current_block_type_index = select_block_type_1(ctx))) {
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
    p(ctx2, dirty) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2);
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
function create_if_block_3(ctx) {
  let upload_1;
  let updating_dragging;
  let updating_uploading;
  let current;
  function upload_1_dragging_binding(value) {
    ctx[43](value);
  }
  function upload_1_uploading_binding(value) {
    ctx[44](value);
  }
  let upload_1_props = {
    filetype: "audio/aac,audio/midi,audio/mpeg,audio/ogg,audio/wav,audio/x-wav,audio/opus,audio/webm,audio/flac,audio/vnd.rn-realaudio,audio/x-ms-wma,audio/x-aiff,audio/amr,audio/*",
    root: (
      /*root*/
      ctx[6]
    ),
    max_file_size: (
      /*max_file_size*/
      ctx[18]
    ),
    upload: (
      /*upload*/
      ctx[19]
    ),
    stream_handler: (
      /*stream_handler*/
      ctx[20]
    ),
    $$slots: { default: [create_default_slot$1] },
    $$scope: { ctx }
  };
  if (
    /*dragging*/
    ctx[0] !== void 0
  ) {
    upload_1_props.dragging = /*dragging*/
    ctx[0];
  }
  if (
    /*uploading*/
    ctx[4] !== void 0
  ) {
    upload_1_props.uploading = /*uploading*/
    ctx[4];
  }
  upload_1 = new Upload({ props: upload_1_props });
  binding_callbacks.push(() => bind(upload_1, "dragging", upload_1_dragging_binding));
  binding_callbacks.push(() => bind(upload_1, "uploading", upload_1_uploading_binding));
  upload_1.$on(
    "load",
    /*handle_load*/
    ctx[28]
  );
  upload_1.$on(
    "error",
    /*error_handler*/
    ctx[45]
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
      if (dirty[0] & /*root*/
      64)
        upload_1_changes.root = /*root*/
        ctx2[6];
      if (dirty[0] & /*max_file_size*/
      262144)
        upload_1_changes.max_file_size = /*max_file_size*/
        ctx2[18];
      if (dirty[0] & /*upload*/
      524288)
        upload_1_changes.upload = /*upload*/
        ctx2[19];
      if (dirty[0] & /*stream_handler*/
      1048576)
        upload_1_changes.stream_handler = /*stream_handler*/
        ctx2[20];
      if (dirty[1] & /*$$scope*/
      4194304) {
        upload_1_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      1) {
        updating_dragging = true;
        upload_1_changes.dragging = /*dragging*/
        ctx2[0];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      16) {
        updating_uploading = true;
        upload_1_changes.uploading = /*uploading*/
        ctx2[4];
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
function create_if_block_1(ctx) {
  let modifyupload;
  let t2;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  modifyupload = new ModifyUpload({ props: { i18n: (
    /*i18n*/
    ctx[12]
  ) } });
  modifyupload.$on(
    "clear",
    /*clear*/
    ctx[27]
  );
  const if_block_creators = [create_if_block_2, create_else_block$1];
  const if_blocks = [];
  function select_block_type_2(ctx2, dirty) {
    if (
      /*streaming*/
      ctx2[11]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_2(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(modifyupload.$$.fragment);
      t2 = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(modifyupload.$$.fragment, nodes);
      t2 = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(modifyupload, target, anchor);
      insert_hydration(target, t2, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const modifyupload_changes = {};
      if (dirty[0] & /*i18n*/
      4096)
        modifyupload_changes.i18n = /*i18n*/
        ctx2[12];
      modifyupload.$set(modifyupload_changes);
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_2(ctx2);
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
      transition_in(modifyupload.$$.fragment, local);
      transition_in(if_block);
      current = true;
    },
    o(local) {
      transition_out(modifyupload.$$.fragment, local);
      transition_out(if_block);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
        detach(if_block_anchor);
      }
      destroy_component(modifyupload, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function create_default_slot$1(ctx) {
  let current;
  const default_slot_template = (
    /*#slots*/
    ctx[38].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[53],
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
        if (default_slot.p && (!current || dirty[1] & /*$$scope*/
        4194304)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[53],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[53]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[53],
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
function create_else_block$1(ctx) {
  let audiorecorder;
  let updating_mode;
  let current;
  function audiorecorder_mode_binding(value) {
    ctx[39](value);
  }
  let audiorecorder_props = {
    i18n: (
      /*i18n*/
      ctx[12]
    ),
    editable: (
      /*editable*/
      ctx[17]
    ),
    recording: (
      /*recording*/
      ctx[1]
    ),
    dispatch_blob: (
      /*dispatch_blob*/
      ctx[25]
    ),
    waveform_settings: (
      /*waveform_settings*/
      ctx[13]
    ),
    waveform_options: (
      /*waveform_options*/
      ctx[15]
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[16]
    )
  };
  if (
    /*mode*/
    ctx[23] !== void 0
  ) {
    audiorecorder_props.mode = /*mode*/
    ctx[23];
  }
  audiorecorder = new AudioRecorder({ props: audiorecorder_props });
  binding_callbacks.push(() => bind(audiorecorder, "mode", audiorecorder_mode_binding));
  audiorecorder.$on(
    "start_recording",
    /*start_recording_handler*/
    ctx[40]
  );
  audiorecorder.$on(
    "pause_recording",
    /*pause_recording_handler*/
    ctx[41]
  );
  audiorecorder.$on(
    "stop_recording",
    /*stop_recording_handler*/
    ctx[42]
  );
  return {
    c() {
      create_component(audiorecorder.$$.fragment);
    },
    l(nodes) {
      claim_component(audiorecorder.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(audiorecorder, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const audiorecorder_changes = {};
      if (dirty[0] & /*i18n*/
      4096)
        audiorecorder_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*editable*/
      131072)
        audiorecorder_changes.editable = /*editable*/
        ctx2[17];
      if (dirty[0] & /*recording*/
      2)
        audiorecorder_changes.recording = /*recording*/
        ctx2[1];
      if (dirty[0] & /*waveform_settings*/
      8192)
        audiorecorder_changes.waveform_settings = /*waveform_settings*/
        ctx2[13];
      if (dirty[0] & /*waveform_options*/
      32768)
        audiorecorder_changes.waveform_options = /*waveform_options*/
        ctx2[15];
      if (dirty[0] & /*handle_reset_value*/
      65536)
        audiorecorder_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[16];
      if (!updating_mode && dirty[0] & /*mode*/
      8388608) {
        updating_mode = true;
        audiorecorder_changes.mode = /*mode*/
        ctx2[23];
        add_flush_callback(() => updating_mode = false);
      }
      audiorecorder.$set(audiorecorder_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(audiorecorder.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(audiorecorder.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(audiorecorder, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  let streamaudio;
  let current;
  streamaudio = new StreamAudio({
    props: {
      record: (
        /*record*/
        ctx[26]
      ),
      recording: (
        /*recording*/
        ctx[1]
      ),
      stop: (
        /*stop*/
        ctx[29]
      ),
      i18n: (
        /*i18n*/
        ctx[12]
      ),
      waveform_settings: (
        /*waveform_settings*/
        ctx[13]
      ),
      waveform_options: (
        /*waveform_options*/
        ctx[15]
      ),
      waiting: (
        /*stream_state*/
        ctx[22] === "waiting"
      )
    }
  });
  return {
    c() {
      create_component(streamaudio.$$.fragment);
    },
    l(nodes) {
      claim_component(streamaudio.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(streamaudio, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const streamaudio_changes = {};
      if (dirty[0] & /*recording*/
      2)
        streamaudio_changes.recording = /*recording*/
        ctx2[1];
      if (dirty[0] & /*i18n*/
      4096)
        streamaudio_changes.i18n = /*i18n*/
        ctx2[12];
      if (dirty[0] & /*waveform_settings*/
      8192)
        streamaudio_changes.waveform_settings = /*waveform_settings*/
        ctx2[13];
      if (dirty[0] & /*waveform_options*/
      32768)
        streamaudio_changes.waveform_options = /*waveform_options*/
        ctx2[15];
      if (dirty[0] & /*stream_state*/
      4194304)
        streamaudio_changes.waiting = /*stream_state*/
        ctx2[22] === "waiting";
      streamaudio.$set(streamaudio_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(streamaudio.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(streamaudio.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(streamaudio, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let blocklabel;
  let t0;
  let div;
  let streamingbar;
  let t1;
  let current_block_type_index;
  let if_block;
  let t2;
  let selectsource;
  let updating_active_source;
  let current;
  blocklabel = new BlockLabel({
    props: {
      show_label: (
        /*show_label*/
        ctx[8]
      ),
      Icon: Music,
      float: (
        /*active_source*/
        ctx[3] === "upload" && /*value*/
        ctx[2] === null
      ),
      label: (
        /*label*/
        ctx[5] || /*i18n*/
        ctx[12]("audio.audio")
      )
    }
  });
  streamingbar = new StreamingBar({
    props: { time_limit: (
      /*time_limit*/
      ctx[21]
    ) }
  });
  const if_block_creators = [create_if_block$1, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[2] === null || /*streaming*/
      ctx2[11]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  function selectsource_active_source_binding(value) {
    ctx[52](value);
  }
  let selectsource_props = {
    sources: (
      /*sources*/
      ctx[10]
    ),
    handle_clear: (
      /*clear*/
      ctx[27]
    )
  };
  if (
    /*active_source*/
    ctx[3] !== void 0
  ) {
    selectsource_props.active_source = /*active_source*/
    ctx[3];
  }
  selectsource = new SelectSource({ props: selectsource_props });
  binding_callbacks.push(() => bind(selectsource, "active_source", selectsource_active_source_binding));
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t0 = space();
      div = element("div");
      create_component(streamingbar.$$.fragment);
      t1 = space();
      if_block.c();
      t2 = space();
      create_component(selectsource.$$.fragment);
      this.h();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t0 = claim_space(nodes);
      div = claim_element(nodes, "DIV", { class: true });
      var div_nodes = children(div);
      claim_component(streamingbar.$$.fragment, div_nodes);
      t1 = claim_space(div_nodes);
      if_block.l(div_nodes);
      t2 = claim_space(div_nodes);
      claim_component(selectsource.$$.fragment, div_nodes);
      div_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "audio-container svelte-cbyffp");
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t0, anchor);
      insert_hydration(target, div, anchor);
      mount_component(streamingbar, div, null);
      append_hydration(div, t1);
      if_blocks[current_block_type_index].m(div, null);
      append_hydration(div, t2);
      mount_component(selectsource, div, null);
      current = true;
    },
    p(ctx2, dirty) {
      const blocklabel_changes = {};
      if (dirty[0] & /*show_label*/
      256)
        blocklabel_changes.show_label = /*show_label*/
        ctx2[8];
      if (dirty[0] & /*active_source, value*/
      12)
        blocklabel_changes.float = /*active_source*/
        ctx2[3] === "upload" && /*value*/
        ctx2[2] === null;
      if (dirty[0] & /*label, i18n*/
      4128)
        blocklabel_changes.label = /*label*/
        ctx2[5] || /*i18n*/
        ctx2[12]("audio.audio");
      blocklabel.$set(blocklabel_changes);
      const streamingbar_changes = {};
      if (dirty[0] & /*time_limit*/
      2097152)
        streamingbar_changes.time_limit = /*time_limit*/
        ctx2[21];
      streamingbar.$set(streamingbar_changes);
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
        if_block.m(div, t2);
      }
      const selectsource_changes = {};
      if (dirty[0] & /*sources*/
      1024)
        selectsource_changes.sources = /*sources*/
        ctx2[10];
      if (!updating_active_source && dirty[0] & /*active_source*/
      8) {
        updating_active_source = true;
        selectsource_changes.active_source = /*active_source*/
        ctx2[3];
        add_flush_callback(() => updating_active_source = false);
      }
      selectsource.$set(selectsource_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(blocklabel.$$.fragment, local);
      transition_in(streamingbar.$$.fragment, local);
      transition_in(if_block);
      transition_in(selectsource.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(blocklabel.$$.fragment, local);
      transition_out(streamingbar.$$.fragment, local);
      transition_out(if_block);
      transition_out(selectsource.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t0);
        detach(div);
      }
      destroy_component(blocklabel, detaching);
      destroy_component(streamingbar);
      if_blocks[current_block_type_index].d();
      destroy_component(selectsource);
    }
  };
}
const NUM_HEADER_BYTES = 44;
function instance$1($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { value = null } = $$props;
  let { label } = $$props;
  let { root } = $$props;
  let { loop } = $$props;
  let { show_label = true } = $$props;
  let { show_download_button = false } = $$props;
  let { sources = ["microphone", "upload"] } = $$props;
  let { pending = false } = $$props;
  let { streaming = false } = $$props;
  let { i18n } = $$props;
  let { waveform_settings } = $$props;
  let { trim_region_settings = {} } = $$props;
  let { waveform_options = {} } = $$props;
  let { dragging } = $$props;
  let { active_source } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let { editable = true } = $$props;
  let { max_file_size = null } = $$props;
  let { upload } = $$props;
  let { stream_handler } = $$props;
  let { stream_every } = $$props;
  let { uploading = false } = $$props;
  let { recording = false } = $$props;
  let time_limit = null;
  let stream_state = "closed";
  const modify_stream = (state) => {
    if (state === "closed") {
      $$invalidate(21, time_limit = null);
      $$invalidate(22, stream_state = "closed");
    } else if (state === "waiting") {
      $$invalidate(22, stream_state = "waiting");
    } else {
      $$invalidate(22, stream_state = "open");
    }
  };
  const set_time_limit = (time) => {
    if (recording)
      $$invalidate(21, time_limit = time);
  };
  let recorder;
  let mode = "";
  let header = void 0;
  let pending_stream = [];
  let submit_pending_stream_on_pending_end = false;
  let inited = false;
  let audio_chunks = [];
  let module_promises;
  function get_modules() {
    module_promises = [
      __vitePreload(() => import("./module.8J93j-Wi.js"), true ? __vite__mapDeps([0,1,2,3,4,5,6]) : void 0, import.meta.url),
      __vitePreload(() => import("./module.BBKnj-Vx.js"), true ? __vite__mapDeps([7,1]) : void 0, import.meta.url)
    ];
  }
  const is_browser = typeof window !== "undefined";
  if (is_browser && streaming) {
    get_modules();
  }
  const dispatch = createEventDispatcher();
  const dispatch_blob = async (blobs, event) => {
    var _a;
    let _audio_blob = new File(blobs, "audio.wav");
    const val = await prepare_files([_audio_blob], event === "stream");
    $$invalidate(2, value = (_a = await upload(val, root, void 0, max_file_size || void 0)) == null ? void 0 : _a.filter(Boolean)[0]);
    dispatch(event, value);
  };
  onDestroy(() => {
    if (streaming && recorder && recorder.state !== "inactive") {
      recorder.stop();
    }
  });
  async function prepare_audio() {
    let stream;
    try {
      stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    } catch (err) {
      if (!navigator.mediaDevices) {
        dispatch("error", i18n("audio.no_device_support"));
        return;
      }
      if (err instanceof DOMException && err.name == "NotAllowedError") {
        dispatch("error", i18n("audio.allow_recording_access"));
        return;
      }
      throw err;
    }
    if (stream == null)
      return;
    if (streaming) {
      const [{ MediaRecorder: MediaRecorder2, register }, { connect }] = await Promise.all(module_promises);
      await register(await connect());
      $$invalidate(34, recorder = new MediaRecorder2(stream, { mimeType: "audio/wav" }));
      recorder.addEventListener("dataavailable", handle_chunk);
    } else {
      $$invalidate(34, recorder = new MediaRecorder(stream));
      recorder.addEventListener("dataavailable", (event) => {
        audio_chunks.push(event.data);
      });
    }
    recorder.addEventListener("stop", async () => {
      $$invalidate(1, recording = false);
      await dispatch_blob(audio_chunks, "change");
      await dispatch_blob(audio_chunks, "stop_recording");
      audio_chunks = [];
    });
    inited = true;
  }
  async function handle_chunk(event) {
    let buffer = await event.data.arrayBuffer();
    let payload = new Uint8Array(buffer);
    if (!header) {
      $$invalidate(35, header = new Uint8Array(buffer.slice(0, NUM_HEADER_BYTES)));
      payload = new Uint8Array(buffer.slice(NUM_HEADER_BYTES));
    }
    if (pending) {
      pending_stream.push(payload);
    } else {
      let blobParts = [header].concat(pending_stream, [payload]);
      if (!recording || stream_state === "waiting")
        return;
      dispatch_blob(blobParts, "stream");
      $$invalidate(36, pending_stream = []);
    }
  }
  async function record() {
    $$invalidate(1, recording = true);
    dispatch("start_recording");
    if (!inited)
      await prepare_audio();
    $$invalidate(35, header = void 0);
    if (streaming && recorder.state != "recording") {
      recorder.start(stream_every * 1e3);
    }
  }
  function clear() {
    dispatch("change", null);
    dispatch("clear");
    $$invalidate(23, mode = "");
    $$invalidate(2, value = null);
  }
  function handle_load({ detail }) {
    $$invalidate(2, value = detail);
    dispatch("change", detail);
    dispatch("upload", detail);
  }
  async function stop() {
    $$invalidate(1, recording = false);
    if (streaming) {
      dispatch("close_stream");
      dispatch("stop_recording");
      recorder.stop();
      if (pending) {
        $$invalidate(37, submit_pending_stream_on_pending_end = true);
      }
      dispatch_blob(audio_chunks, "stop_recording");
      dispatch("clear");
      $$invalidate(23, mode = "");
    }
  }
  function audiorecorder_mode_binding(value2) {
    mode = value2;
    $$invalidate(23, mode);
  }
  function start_recording_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_recording_handler(event) {
    bubble.call(this, $$self, event);
  }
  function stop_recording_handler(event) {
    bubble.call(this, $$self, event);
  }
  function upload_1_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(0, dragging);
  }
  function upload_1_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(4, uploading);
  }
  const error_handler = ({ detail }) => dispatch("error", detail);
  const edit_handler_1 = () => $$invalidate(23, mode = "edit");
  function audioplayer_mode_binding(value2) {
    mode = value2;
    $$invalidate(23, mode);
  }
  function stop_handler(event) {
    bubble.call(this, $$self, event);
  }
  function play_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_handler(event) {
    bubble.call(this, $$self, event);
  }
  function edit_handler(event) {
    bubble.call(this, $$self, event);
  }
  function selectsource_active_source_binding(value2) {
    active_source = value2;
    $$invalidate(3, active_source);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(2, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(5, label = $$props2.label);
    if ("root" in $$props2)
      $$invalidate(6, root = $$props2.root);
    if ("loop" in $$props2)
      $$invalidate(7, loop = $$props2.loop);
    if ("show_label" in $$props2)
      $$invalidate(8, show_label = $$props2.show_label);
    if ("show_download_button" in $$props2)
      $$invalidate(9, show_download_button = $$props2.show_download_button);
    if ("sources" in $$props2)
      $$invalidate(10, sources = $$props2.sources);
    if ("pending" in $$props2)
      $$invalidate(30, pending = $$props2.pending);
    if ("streaming" in $$props2)
      $$invalidate(11, streaming = $$props2.streaming);
    if ("i18n" in $$props2)
      $$invalidate(12, i18n = $$props2.i18n);
    if ("waveform_settings" in $$props2)
      $$invalidate(13, waveform_settings = $$props2.waveform_settings);
    if ("trim_region_settings" in $$props2)
      $$invalidate(14, trim_region_settings = $$props2.trim_region_settings);
    if ("waveform_options" in $$props2)
      $$invalidate(15, waveform_options = $$props2.waveform_options);
    if ("dragging" in $$props2)
      $$invalidate(0, dragging = $$props2.dragging);
    if ("active_source" in $$props2)
      $$invalidate(3, active_source = $$props2.active_source);
    if ("handle_reset_value" in $$props2)
      $$invalidate(16, handle_reset_value = $$props2.handle_reset_value);
    if ("editable" in $$props2)
      $$invalidate(17, editable = $$props2.editable);
    if ("max_file_size" in $$props2)
      $$invalidate(18, max_file_size = $$props2.max_file_size);
    if ("upload" in $$props2)
      $$invalidate(19, upload = $$props2.upload);
    if ("stream_handler" in $$props2)
      $$invalidate(20, stream_handler = $$props2.stream_handler);
    if ("stream_every" in $$props2)
      $$invalidate(31, stream_every = $$props2.stream_every);
    if ("uploading" in $$props2)
      $$invalidate(4, uploading = $$props2.uploading);
    if ("recording" in $$props2)
      $$invalidate(1, recording = $$props2.recording);
    if ("$$scope" in $$props2)
      $$invalidate(53, $$scope = $$props2.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*dragging*/
    1) {
      dispatch("drag", dragging);
    }
    if ($$self.$$.dirty[0] & /*pending*/
    1073741824 | $$self.$$.dirty[1] & /*submit_pending_stream_on_pending_end, header, pending_stream*/
    112) {
      if (submit_pending_stream_on_pending_end && pending === false) {
        $$invalidate(37, submit_pending_stream_on_pending_end = false);
        if (header && pending_stream) {
          let blobParts = [header].concat(pending_stream);
          $$invalidate(36, pending_stream = []);
          dispatch_blob(blobParts, "stream");
        }
      }
    }
    if ($$self.$$.dirty[0] & /*recording*/
    2 | $$self.$$.dirty[1] & /*recorder*/
    8) {
      if (!recording && recorder)
        stop();
    }
    if ($$self.$$.dirty[0] & /*recording*/
    2 | $$self.$$.dirty[1] & /*recorder*/
    8) {
      if (recording && recorder)
        record();
    }
  };
  return [
    dragging,
    recording,
    value,
    active_source,
    uploading,
    label,
    root,
    loop,
    show_label,
    show_download_button,
    sources,
    streaming,
    i18n,
    waveform_settings,
    trim_region_settings,
    waveform_options,
    handle_reset_value,
    editable,
    max_file_size,
    upload,
    stream_handler,
    time_limit,
    stream_state,
    mode,
    dispatch,
    dispatch_blob,
    record,
    clear,
    handle_load,
    stop,
    pending,
    stream_every,
    modify_stream,
    set_time_limit,
    recorder,
    header,
    pending_stream,
    submit_pending_stream_on_pending_end,
    slots,
    audiorecorder_mode_binding,
    start_recording_handler,
    pause_recording_handler,
    stop_recording_handler,
    upload_1_dragging_binding,
    upload_1_uploading_binding,
    error_handler,
    edit_handler_1,
    audioplayer_mode_binding,
    stop_handler,
    play_handler,
    pause_handler,
    edit_handler,
    selectsource_active_source_binding,
    $$scope
  ];
}
class InteractiveAudio extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 2,
        label: 5,
        root: 6,
        loop: 7,
        show_label: 8,
        show_download_button: 9,
        sources: 10,
        pending: 30,
        streaming: 11,
        i18n: 12,
        waveform_settings: 13,
        trim_region_settings: 14,
        waveform_options: 15,
        dragging: 0,
        active_source: 3,
        handle_reset_value: 16,
        editable: 17,
        max_file_size: 18,
        upload: 19,
        stream_handler: 20,
        stream_every: 31,
        uploading: 4,
        recording: 1,
        modify_stream: 32,
        set_time_limit: 33
      },
      null,
      [-1, -1]
    );
  }
  get modify_stream() {
    return this.$$.ctx[32];
  }
  get set_time_limit() {
    return this.$$.ctx[33];
  }
}
const InteractiveAudio$1 = InteractiveAudio;
function create_else_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      variant: (
        /*value*/
        ctx[0] === null && /*active_source*/
        ctx[25] === "upload" ? "dashed" : "solid"
      ),
      border_mode: (
        /*dragging*/
        ctx[27] ? "focus" : "base"
      ),
      padding: false,
      allow_overflow: false,
      elem_id: (
        /*elem_id*/
        ctx[4]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[5]
      ),
      visible: (
        /*visible*/
        ctx[6]
      ),
      container: (
        /*container*/
        ctx[12]
      ),
      scale: (
        /*scale*/
        ctx[13]
      ),
      min_width: (
        /*min_width*/
        ctx[14]
      ),
      $$slots: { default: [create_default_slot_1] },
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
      if (dirty[0] & /*value, active_source*/
      33554433)
        block_changes.variant = /*value*/
        ctx2[0] === null && /*active_source*/
        ctx2[25] === "upload" ? "dashed" : "solid";
      if (dirty[0] & /*dragging*/
      134217728)
        block_changes.border_mode = /*dragging*/
        ctx2[27] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      16)
        block_changes.elem_id = /*elem_id*/
        ctx2[4];
      if (dirty[0] & /*elem_classes*/
      32)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[5];
      if (dirty[0] & /*visible*/
      64)
        block_changes.visible = /*visible*/
        ctx2[6];
      if (dirty[0] & /*container*/
      4096)
        block_changes.container = /*container*/
        ctx2[12];
      if (dirty[0] & /*scale*/
      8192)
        block_changes.scale = /*scale*/
        ctx2[13];
      if (dirty[0] & /*min_width*/
      16384)
        block_changes.min_width = /*min_width*/
        ctx2[14];
      if (dirty[0] & /*label, show_label, show_download_button, value, root, sources, active_source, pending, streaming, loop, gradio, editable, waveform_settings, waveform_options, stream_every, recording, dragging, uploading, _modify_stream, set_time_limit, loading_status*/
      536710927 | dirty[2] & /*$$scope*/
      128) {
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
function create_if_block(ctx) {
  let block;
  let current;
  block = new Block({
    props: {
      variant: "solid",
      border_mode: (
        /*dragging*/
        ctx[27] ? "focus" : "base"
      ),
      padding: false,
      allow_overflow: false,
      elem_id: (
        /*elem_id*/
        ctx[4]
      ),
      elem_classes: (
        /*elem_classes*/
        ctx[5]
      ),
      visible: (
        /*visible*/
        ctx[6]
      ),
      container: (
        /*container*/
        ctx[12]
      ),
      scale: (
        /*scale*/
        ctx[13]
      ),
      min_width: (
        /*min_width*/
        ctx[14]
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
      if (dirty[0] & /*dragging*/
      134217728)
        block_changes.border_mode = /*dragging*/
        ctx2[27] ? "focus" : "base";
      if (dirty[0] & /*elem_id*/
      16)
        block_changes.elem_id = /*elem_id*/
        ctx2[4];
      if (dirty[0] & /*elem_classes*/
      32)
        block_changes.elem_classes = /*elem_classes*/
        ctx2[5];
      if (dirty[0] & /*visible*/
      64)
        block_changes.visible = /*visible*/
        ctx2[6];
      if (dirty[0] & /*container*/
      4096)
        block_changes.container = /*container*/
        ctx2[12];
      if (dirty[0] & /*scale*/
      8192)
        block_changes.scale = /*scale*/
        ctx2[13];
      if (dirty[0] & /*min_width*/
      16384)
        block_changes.min_width = /*min_width*/
        ctx2[14];
      if (dirty[0] & /*gradio, show_label, show_download_button, show_share_button, value, label, loop, waveform_settings, waveform_options, editable, loading_status*/
      277842435 | dirty[2] & /*$$scope*/
      128) {
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
function create_default_slot_2(ctx) {
  let uploadtext;
  let current;
  uploadtext = new UploadText({
    props: {
      i18n: (
        /*gradio*/
        ctx[23].i18n
      ),
      type: "audio"
    }
  });
  return {
    c() {
      create_component(uploadtext.$$.fragment);
    },
    l(nodes) {
      claim_component(uploadtext.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(uploadtext, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const uploadtext_changes = {};
      if (dirty[0] & /*gradio*/
      8388608)
        uploadtext_changes.i18n = /*gradio*/
        ctx2[23].i18n;
      uploadtext.$set(uploadtext_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(uploadtext.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(uploadtext.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(uploadtext, detaching);
    }
  };
}
function create_default_slot_1(ctx) {
  let statustracker;
  let t2;
  let interactiveaudio;
  let updating_recording;
  let updating_dragging;
  let updating_uploading;
  let updating_modify_stream;
  let updating_set_time_limit;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[23].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[23].i18n
    ) },
    /*loading_status*/
    ctx[1]
  ];
  let statustracker_props = {};
  for (let i2 = 0; i2 < statustracker_spread_levels.length; i2 += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i2]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler_1*/
    ctx[45]
  );
  function interactiveaudio_recording_binding(value) {
    ctx[48](value);
  }
  function interactiveaudio_dragging_binding(value) {
    ctx[49](value);
  }
  function interactiveaudio_uploading_binding(value) {
    ctx[50](value);
  }
  function interactiveaudio_modify_stream_binding(value) {
    ctx[51](value);
  }
  function interactiveaudio_set_time_limit_binding(value) {
    ctx[52](value);
  }
  let interactiveaudio_props = {
    label: (
      /*label*/
      ctx[9]
    ),
    show_label: (
      /*show_label*/
      ctx[11]
    ),
    show_download_button: (
      /*show_download_button*/
      ctx[16]
    ),
    value: (
      /*value*/
      ctx[0]
    ),
    root: (
      /*root*/
      ctx[10]
    ),
    sources: (
      /*sources*/
      ctx[8]
    ),
    active_source: (
      /*active_source*/
      ctx[25]
    ),
    pending: (
      /*pending*/
      ctx[20]
    ),
    streaming: (
      /*streaming*/
      ctx[21]
    ),
    loop: (
      /*loop*/
      ctx[15]
    ),
    max_file_size: (
      /*gradio*/
      ctx[23].max_file_size
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[29]
    ),
    editable: (
      /*editable*/
      ctx[18]
    ),
    i18n: (
      /*gradio*/
      ctx[23].i18n
    ),
    waveform_settings: (
      /*waveform_settings*/
      ctx[28]
    ),
    waveform_options: (
      /*waveform_options*/
      ctx[19]
    ),
    trim_region_settings: (
      /*trim_region_settings*/
      ctx[30]
    ),
    stream_every: (
      /*stream_every*/
      ctx[22]
    ),
    upload: (
      /*func*/
      ctx[46]
    ),
    stream_handler: (
      /*func_1*/
      ctx[47]
    ),
    $$slots: { default: [create_default_slot_2] },
    $$scope: { ctx }
  };
  if (
    /*recording*/
    ctx[2] !== void 0
  ) {
    interactiveaudio_props.recording = /*recording*/
    ctx[2];
  }
  if (
    /*dragging*/
    ctx[27] !== void 0
  ) {
    interactiveaudio_props.dragging = /*dragging*/
    ctx[27];
  }
  if (
    /*uploading*/
    ctx[24] !== void 0
  ) {
    interactiveaudio_props.uploading = /*uploading*/
    ctx[24];
  }
  if (
    /*_modify_stream*/
    ctx[26] !== void 0
  ) {
    interactiveaudio_props.modify_stream = /*_modify_stream*/
    ctx[26];
  }
  if (
    /*set_time_limit*/
    ctx[3] !== void 0
  ) {
    interactiveaudio_props.set_time_limit = /*set_time_limit*/
    ctx[3];
  }
  interactiveaudio = new InteractiveAudio$1({ props: interactiveaudio_props });
  binding_callbacks.push(() => bind(interactiveaudio, "recording", interactiveaudio_recording_binding));
  binding_callbacks.push(() => bind(interactiveaudio, "dragging", interactiveaudio_dragging_binding));
  binding_callbacks.push(() => bind(interactiveaudio, "uploading", interactiveaudio_uploading_binding));
  binding_callbacks.push(() => bind(interactiveaudio, "modify_stream", interactiveaudio_modify_stream_binding));
  binding_callbacks.push(() => bind(interactiveaudio, "set_time_limit", interactiveaudio_set_time_limit_binding));
  interactiveaudio.$on(
    "change",
    /*change_handler*/
    ctx[53]
  );
  interactiveaudio.$on(
    "stream",
    /*stream_handler*/
    ctx[54]
  );
  interactiveaudio.$on(
    "drag",
    /*drag_handler*/
    ctx[55]
  );
  interactiveaudio.$on(
    "edit",
    /*edit_handler*/
    ctx[56]
  );
  interactiveaudio.$on(
    "play",
    /*play_handler_1*/
    ctx[57]
  );
  interactiveaudio.$on(
    "pause",
    /*pause_handler_1*/
    ctx[58]
  );
  interactiveaudio.$on(
    "stop",
    /*stop_handler_1*/
    ctx[59]
  );
  interactiveaudio.$on(
    "start_recording",
    /*start_recording_handler*/
    ctx[60]
  );
  interactiveaudio.$on(
    "pause_recording",
    /*pause_recording_handler*/
    ctx[61]
  );
  interactiveaudio.$on(
    "stop_recording",
    /*stop_recording_handler*/
    ctx[62]
  );
  interactiveaudio.$on(
    "upload",
    /*upload_handler*/
    ctx[63]
  );
  interactiveaudio.$on(
    "clear",
    /*clear_handler*/
    ctx[64]
  );
  interactiveaudio.$on(
    "error",
    /*handle_error*/
    ctx[31]
  );
  interactiveaudio.$on(
    "close_stream",
    /*close_stream_handler*/
    ctx[65]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t2 = space();
      create_component(interactiveaudio.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t2 = claim_space(nodes);
      claim_component(interactiveaudio.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t2, anchor);
      mount_component(interactiveaudio, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      8388610 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        8388608 && {
          autoscroll: (
            /*gradio*/
            ctx2[23].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        8388608 && { i18n: (
          /*gradio*/
          ctx2[23].i18n
        ) },
        dirty[0] & /*loading_status*/
        2 && get_spread_object(
          /*loading_status*/
          ctx2[1]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const interactiveaudio_changes = {};
      if (dirty[0] & /*label*/
      512)
        interactiveaudio_changes.label = /*label*/
        ctx2[9];
      if (dirty[0] & /*show_label*/
      2048)
        interactiveaudio_changes.show_label = /*show_label*/
        ctx2[11];
      if (dirty[0] & /*show_download_button*/
      65536)
        interactiveaudio_changes.show_download_button = /*show_download_button*/
        ctx2[16];
      if (dirty[0] & /*value*/
      1)
        interactiveaudio_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*root*/
      1024)
        interactiveaudio_changes.root = /*root*/
        ctx2[10];
      if (dirty[0] & /*sources*/
      256)
        interactiveaudio_changes.sources = /*sources*/
        ctx2[8];
      if (dirty[0] & /*active_source*/
      33554432)
        interactiveaudio_changes.active_source = /*active_source*/
        ctx2[25];
      if (dirty[0] & /*pending*/
      1048576)
        interactiveaudio_changes.pending = /*pending*/
        ctx2[20];
      if (dirty[0] & /*streaming*/
      2097152)
        interactiveaudio_changes.streaming = /*streaming*/
        ctx2[21];
      if (dirty[0] & /*loop*/
      32768)
        interactiveaudio_changes.loop = /*loop*/
        ctx2[15];
      if (dirty[0] & /*gradio*/
      8388608)
        interactiveaudio_changes.max_file_size = /*gradio*/
        ctx2[23].max_file_size;
      if (dirty[0] & /*editable*/
      262144)
        interactiveaudio_changes.editable = /*editable*/
        ctx2[18];
      if (dirty[0] & /*gradio*/
      8388608)
        interactiveaudio_changes.i18n = /*gradio*/
        ctx2[23].i18n;
      if (dirty[0] & /*waveform_settings*/
      268435456)
        interactiveaudio_changes.waveform_settings = /*waveform_settings*/
        ctx2[28];
      if (dirty[0] & /*waveform_options*/
      524288)
        interactiveaudio_changes.waveform_options = /*waveform_options*/
        ctx2[19];
      if (dirty[0] & /*stream_every*/
      4194304)
        interactiveaudio_changes.stream_every = /*stream_every*/
        ctx2[22];
      if (dirty[0] & /*gradio*/
      8388608)
        interactiveaudio_changes.upload = /*func*/
        ctx2[46];
      if (dirty[0] & /*gradio*/
      8388608)
        interactiveaudio_changes.stream_handler = /*func_1*/
        ctx2[47];
      if (dirty[0] & /*gradio*/
      8388608 | dirty[2] & /*$$scope*/
      128) {
        interactiveaudio_changes.$$scope = { dirty, ctx: ctx2 };
      }
      if (!updating_recording && dirty[0] & /*recording*/
      4) {
        updating_recording = true;
        interactiveaudio_changes.recording = /*recording*/
        ctx2[2];
        add_flush_callback(() => updating_recording = false);
      }
      if (!updating_dragging && dirty[0] & /*dragging*/
      134217728) {
        updating_dragging = true;
        interactiveaudio_changes.dragging = /*dragging*/
        ctx2[27];
        add_flush_callback(() => updating_dragging = false);
      }
      if (!updating_uploading && dirty[0] & /*uploading*/
      16777216) {
        updating_uploading = true;
        interactiveaudio_changes.uploading = /*uploading*/
        ctx2[24];
        add_flush_callback(() => updating_uploading = false);
      }
      if (!updating_modify_stream && dirty[0] & /*_modify_stream*/
      67108864) {
        updating_modify_stream = true;
        interactiveaudio_changes.modify_stream = /*_modify_stream*/
        ctx2[26];
        add_flush_callback(() => updating_modify_stream = false);
      }
      if (!updating_set_time_limit && dirty[0] & /*set_time_limit*/
      8) {
        updating_set_time_limit = true;
        interactiveaudio_changes.set_time_limit = /*set_time_limit*/
        ctx2[3];
        add_flush_callback(() => updating_set_time_limit = false);
      }
      interactiveaudio.$set(interactiveaudio_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(interactiveaudio.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(interactiveaudio.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
      }
      destroy_component(statustracker, detaching);
      destroy_component(interactiveaudio, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let statustracker;
  let t2;
  let staticaudio;
  let current;
  const statustracker_spread_levels = [
    {
      autoscroll: (
        /*gradio*/
        ctx[23].autoscroll
      )
    },
    { i18n: (
      /*gradio*/
      ctx[23].i18n
    ) },
    /*loading_status*/
    ctx[1]
  ];
  let statustracker_props = {};
  for (let i2 = 0; i2 < statustracker_spread_levels.length; i2 += 1) {
    statustracker_props = assign(statustracker_props, statustracker_spread_levels[i2]);
  }
  statustracker = new Static({ props: statustracker_props });
  statustracker.$on(
    "clear_status",
    /*clear_status_handler*/
    ctx[39]
  );
  staticaudio = new StaticAudio({
    props: {
      i18n: (
        /*gradio*/
        ctx[23].i18n
      ),
      show_label: (
        /*show_label*/
        ctx[11]
      ),
      show_download_button: (
        /*show_download_button*/
        ctx[16]
      ),
      show_share_button: (
        /*show_share_button*/
        ctx[17]
      ),
      value: (
        /*value*/
        ctx[0]
      ),
      label: (
        /*label*/
        ctx[9]
      ),
      loop: (
        /*loop*/
        ctx[15]
      ),
      waveform_settings: (
        /*waveform_settings*/
        ctx[28]
      ),
      waveform_options: (
        /*waveform_options*/
        ctx[19]
      ),
      editable: (
        /*editable*/
        ctx[18]
      )
    }
  });
  staticaudio.$on(
    "share",
    /*share_handler*/
    ctx[40]
  );
  staticaudio.$on(
    "error",
    /*error_handler*/
    ctx[41]
  );
  staticaudio.$on(
    "play",
    /*play_handler*/
    ctx[42]
  );
  staticaudio.$on(
    "pause",
    /*pause_handler*/
    ctx[43]
  );
  staticaudio.$on(
    "stop",
    /*stop_handler*/
    ctx[44]
  );
  return {
    c() {
      create_component(statustracker.$$.fragment);
      t2 = space();
      create_component(staticaudio.$$.fragment);
    },
    l(nodes) {
      claim_component(statustracker.$$.fragment, nodes);
      t2 = claim_space(nodes);
      claim_component(staticaudio.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(statustracker, target, anchor);
      insert_hydration(target, t2, anchor);
      mount_component(staticaudio, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const statustracker_changes = dirty[0] & /*gradio, loading_status*/
      8388610 ? get_spread_update(statustracker_spread_levels, [
        dirty[0] & /*gradio*/
        8388608 && {
          autoscroll: (
            /*gradio*/
            ctx2[23].autoscroll
          )
        },
        dirty[0] & /*gradio*/
        8388608 && { i18n: (
          /*gradio*/
          ctx2[23].i18n
        ) },
        dirty[0] & /*loading_status*/
        2 && get_spread_object(
          /*loading_status*/
          ctx2[1]
        )
      ]) : {};
      statustracker.$set(statustracker_changes);
      const staticaudio_changes = {};
      if (dirty[0] & /*gradio*/
      8388608)
        staticaudio_changes.i18n = /*gradio*/
        ctx2[23].i18n;
      if (dirty[0] & /*show_label*/
      2048)
        staticaudio_changes.show_label = /*show_label*/
        ctx2[11];
      if (dirty[0] & /*show_download_button*/
      65536)
        staticaudio_changes.show_download_button = /*show_download_button*/
        ctx2[16];
      if (dirty[0] & /*show_share_button*/
      131072)
        staticaudio_changes.show_share_button = /*show_share_button*/
        ctx2[17];
      if (dirty[0] & /*value*/
      1)
        staticaudio_changes.value = /*value*/
        ctx2[0];
      if (dirty[0] & /*label*/
      512)
        staticaudio_changes.label = /*label*/
        ctx2[9];
      if (dirty[0] & /*loop*/
      32768)
        staticaudio_changes.loop = /*loop*/
        ctx2[15];
      if (dirty[0] & /*waveform_settings*/
      268435456)
        staticaudio_changes.waveform_settings = /*waveform_settings*/
        ctx2[28];
      if (dirty[0] & /*waveform_options*/
      524288)
        staticaudio_changes.waveform_options = /*waveform_options*/
        ctx2[19];
      if (dirty[0] & /*editable*/
      262144)
        staticaudio_changes.editable = /*editable*/
        ctx2[18];
      staticaudio.$set(staticaudio_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(statustracker.$$.fragment, local);
      transition_in(staticaudio.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(statustracker.$$.fragment, local);
      transition_out(staticaudio.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
      }
      destroy_component(statustracker, detaching);
      destroy_component(staticaudio, detaching);
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
    if (!/*interactive*/
    ctx2[7])
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
  let { value_is_output = false } = $$props;
  let { elem_id = "" } = $$props;
  let { elem_classes = [] } = $$props;
  let { visible = true } = $$props;
  let { interactive } = $$props;
  let { value = null } = $$props;
  let { sources } = $$props;
  let { label } = $$props;
  let { root } = $$props;
  let { show_label } = $$props;
  let { container = true } = $$props;
  let { scale = null } = $$props;
  let { min_width = void 0 } = $$props;
  let { loading_status } = $$props;
  let { autoplay = false } = $$props;
  let { loop = false } = $$props;
  let { show_download_button } = $$props;
  let { show_share_button = false } = $$props;
  let { editable = true } = $$props;
  let { waveform_options = {} } = $$props;
  let { pending } = $$props;
  let { streaming } = $$props;
  let { stream_every } = $$props;
  let { input_ready } = $$props;
  let { recording = false } = $$props;
  let uploading = false;
  let stream_state = "closed";
  let _modify_stream;
  function modify_stream_state(state) {
    stream_state = state;
    _modify_stream(state);
  }
  const get_stream_state = () => stream_state;
  let { set_time_limit } = $$props;
  let { gradio } = $$props;
  let old_value = null;
  let active_source;
  let initial_value = value;
  const handle_reset_value = () => {
    if (initial_value === null || value === initial_value) {
      return;
    }
    $$invalidate(0, value = initial_value);
  };
  let dragging;
  let waveform_settings;
  let color_accent = "darkorange";
  onMount(() => {
    color_accent = getComputedStyle(document == null ? void 0 : document.documentElement).getPropertyValue("--color-accent");
    set_trim_region_colour();
    $$invalidate(28, waveform_settings.waveColor = waveform_options.waveform_color || "#9ca3af", waveform_settings);
    $$invalidate(28, waveform_settings.progressColor = waveform_options.waveform_progress_color || color_accent, waveform_settings);
    $$invalidate(28, waveform_settings.mediaControls = waveform_options.show_controls, waveform_settings);
    $$invalidate(28, waveform_settings.sampleRate = waveform_options.sample_rate || 44100, waveform_settings);
  });
  const trim_region_settings = {
    color: waveform_options.trim_region_color,
    drag: true,
    resize: true
  };
  function set_trim_region_colour() {
    document.documentElement.style.setProperty("--trim-region-color", trim_region_settings.color || color_accent);
  }
  function handle_error({ detail }) {
    const [level, status] = detail.includes("Invalid file type") ? ["warning", "complete"] : ["error", "error"];
    $$invalidate(1, loading_status = loading_status || {});
    $$invalidate(1, loading_status.status = status, loading_status);
    $$invalidate(1, loading_status.message = detail, loading_status);
    gradio.dispatch(level, detail);
  }
  afterUpdate(() => {
    $$invalidate(32, value_is_output = false);
  });
  const clear_status_handler = () => gradio.dispatch("clear_status", loading_status);
  const share_handler = (e2) => gradio.dispatch("share", e2.detail);
  const error_handler = (e2) => gradio.dispatch("error", e2.detail);
  const play_handler = () => gradio.dispatch("play");
  const pause_handler = () => gradio.dispatch("pause");
  const stop_handler = () => gradio.dispatch("stop");
  const clear_status_handler_1 = () => gradio.dispatch("clear_status", loading_status);
  const func = (...args) => gradio.client.upload(...args);
  const func_1 = (...args) => gradio.client.stream(...args);
  function interactiveaudio_recording_binding(value2) {
    recording = value2;
    $$invalidate(2, recording);
  }
  function interactiveaudio_dragging_binding(value2) {
    dragging = value2;
    $$invalidate(27, dragging);
  }
  function interactiveaudio_uploading_binding(value2) {
    uploading = value2;
    $$invalidate(24, uploading);
  }
  function interactiveaudio_modify_stream_binding(value2) {
    _modify_stream = value2;
    $$invalidate(26, _modify_stream);
  }
  function interactiveaudio_set_time_limit_binding(value2) {
    set_time_limit = value2;
    $$invalidate(3, set_time_limit);
  }
  const change_handler = ({ detail }) => $$invalidate(0, value = detail);
  const stream_handler = ({ detail }) => {
    $$invalidate(0, value = detail);
    gradio.dispatch("stream", value);
  };
  const drag_handler = ({ detail }) => $$invalidate(27, dragging = detail);
  const edit_handler = () => gradio.dispatch("edit");
  const play_handler_1 = () => gradio.dispatch("play");
  const pause_handler_1 = () => gradio.dispatch("pause");
  const stop_handler_1 = () => gradio.dispatch("stop");
  const start_recording_handler = () => gradio.dispatch("start_recording");
  const pause_recording_handler = () => gradio.dispatch("pause_recording");
  const stop_recording_handler = (e2) => gradio.dispatch("stop_recording");
  const upload_handler = () => gradio.dispatch("upload");
  const clear_handler = () => gradio.dispatch("clear");
  const close_stream_handler = () => gradio.dispatch("close_stream", "stream");
  $$self.$$set = ($$props2) => {
    if ("value_is_output" in $$props2)
      $$invalidate(32, value_is_output = $$props2.value_is_output);
    if ("elem_id" in $$props2)
      $$invalidate(4, elem_id = $$props2.elem_id);
    if ("elem_classes" in $$props2)
      $$invalidate(5, elem_classes = $$props2.elem_classes);
    if ("visible" in $$props2)
      $$invalidate(6, visible = $$props2.visible);
    if ("interactive" in $$props2)
      $$invalidate(7, interactive = $$props2.interactive);
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("sources" in $$props2)
      $$invalidate(8, sources = $$props2.sources);
    if ("label" in $$props2)
      $$invalidate(9, label = $$props2.label);
    if ("root" in $$props2)
      $$invalidate(10, root = $$props2.root);
    if ("show_label" in $$props2)
      $$invalidate(11, show_label = $$props2.show_label);
    if ("container" in $$props2)
      $$invalidate(12, container = $$props2.container);
    if ("scale" in $$props2)
      $$invalidate(13, scale = $$props2.scale);
    if ("min_width" in $$props2)
      $$invalidate(14, min_width = $$props2.min_width);
    if ("loading_status" in $$props2)
      $$invalidate(1, loading_status = $$props2.loading_status);
    if ("autoplay" in $$props2)
      $$invalidate(34, autoplay = $$props2.autoplay);
    if ("loop" in $$props2)
      $$invalidate(15, loop = $$props2.loop);
    if ("show_download_button" in $$props2)
      $$invalidate(16, show_download_button = $$props2.show_download_button);
    if ("show_share_button" in $$props2)
      $$invalidate(17, show_share_button = $$props2.show_share_button);
    if ("editable" in $$props2)
      $$invalidate(18, editable = $$props2.editable);
    if ("waveform_options" in $$props2)
      $$invalidate(19, waveform_options = $$props2.waveform_options);
    if ("pending" in $$props2)
      $$invalidate(20, pending = $$props2.pending);
    if ("streaming" in $$props2)
      $$invalidate(21, streaming = $$props2.streaming);
    if ("stream_every" in $$props2)
      $$invalidate(22, stream_every = $$props2.stream_every);
    if ("input_ready" in $$props2)
      $$invalidate(33, input_ready = $$props2.input_ready);
    if ("recording" in $$props2)
      $$invalidate(2, recording = $$props2.recording);
    if ("set_time_limit" in $$props2)
      $$invalidate(3, set_time_limit = $$props2.set_time_limit);
    if ("gradio" in $$props2)
      $$invalidate(23, gradio = $$props2.gradio);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*uploading*/
    16777216) {
      $$invalidate(33, input_ready = !uploading);
    }
    if ($$self.$$.dirty[0] & /*value*/
    1 | $$self.$$.dirty[1] & /*initial_value*/
    128) {
      if (value && initial_value === null) {
        $$invalidate(38, initial_value = value);
      }
    }
    if ($$self.$$.dirty[0] & /*value, gradio*/
    8388609 | $$self.$$.dirty[1] & /*old_value, value_is_output*/
    66) {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          $$invalidate(37, old_value = value);
          gradio.dispatch("change");
          if (!value_is_output) {
            gradio.dispatch("input");
          }
        }
      }
    }
    if ($$self.$$.dirty[0] & /*active_source, sources*/
    33554688) {
      if (!active_source && sources) {
        $$invalidate(25, active_source = sources[0]);
      }
    }
    if ($$self.$$.dirty[1] & /*autoplay*/
    8) {
      $$invalidate(28, waveform_settings = {
        height: 50,
        barWidth: 2,
        barGap: 3,
        cursorWidth: 2,
        cursorColor: "#ddd5e9",
        autoplay,
        barRadius: 10,
        dragToSeek: true,
        normalize: true,
        minPxPerSec: 20
      });
    }
  };
  return [
    value,
    loading_status,
    recording,
    set_time_limit,
    elem_id,
    elem_classes,
    visible,
    interactive,
    sources,
    label,
    root,
    show_label,
    container,
    scale,
    min_width,
    loop,
    show_download_button,
    show_share_button,
    editable,
    waveform_options,
    pending,
    streaming,
    stream_every,
    gradio,
    uploading,
    active_source,
    _modify_stream,
    dragging,
    waveform_settings,
    handle_reset_value,
    trim_region_settings,
    handle_error,
    value_is_output,
    input_ready,
    autoplay,
    modify_stream_state,
    get_stream_state,
    old_value,
    initial_value,
    clear_status_handler,
    share_handler,
    error_handler,
    play_handler,
    pause_handler,
    stop_handler,
    clear_status_handler_1,
    func,
    func_1,
    interactiveaudio_recording_binding,
    interactiveaudio_dragging_binding,
    interactiveaudio_uploading_binding,
    interactiveaudio_modify_stream_binding,
    interactiveaudio_set_time_limit_binding,
    change_handler,
    stream_handler,
    drag_handler,
    edit_handler,
    play_handler_1,
    pause_handler_1,
    stop_handler_1,
    start_recording_handler,
    pause_recording_handler,
    stop_recording_handler,
    upload_handler,
    clear_handler,
    close_stream_handler
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
        value_is_output: 32,
        elem_id: 4,
        elem_classes: 5,
        visible: 6,
        interactive: 7,
        value: 0,
        sources: 8,
        label: 9,
        root: 10,
        show_label: 11,
        container: 12,
        scale: 13,
        min_width: 14,
        loading_status: 1,
        autoplay: 34,
        loop: 15,
        show_download_button: 16,
        show_share_button: 17,
        editable: 18,
        waveform_options: 19,
        pending: 20,
        streaming: 21,
        stream_every: 22,
        input_ready: 33,
        recording: 2,
        modify_stream_state: 35,
        get_stream_state: 36,
        set_time_limit: 3,
        gradio: 23
      },
      null,
      [-1, -1, -1]
    );
  }
  get value_is_output() {
    return this.$$.ctx[32];
  }
  set value_is_output(value_is_output) {
    this.$$set({ value_is_output });
    flush();
  }
  get elem_id() {
    return this.$$.ctx[4];
  }
  set elem_id(elem_id) {
    this.$$set({ elem_id });
    flush();
  }
  get elem_classes() {
    return this.$$.ctx[5];
  }
  set elem_classes(elem_classes) {
    this.$$set({ elem_classes });
    flush();
  }
  get visible() {
    return this.$$.ctx[6];
  }
  set visible(visible) {
    this.$$set({ visible });
    flush();
  }
  get interactive() {
    return this.$$.ctx[7];
  }
  set interactive(interactive) {
    this.$$set({ interactive });
    flush();
  }
  get value() {
    return this.$$.ctx[0];
  }
  set value(value) {
    this.$$set({ value });
    flush();
  }
  get sources() {
    return this.$$.ctx[8];
  }
  set sources(sources) {
    this.$$set({ sources });
    flush();
  }
  get label() {
    return this.$$.ctx[9];
  }
  set label(label) {
    this.$$set({ label });
    flush();
  }
  get root() {
    return this.$$.ctx[10];
  }
  set root(root) {
    this.$$set({ root });
    flush();
  }
  get show_label() {
    return this.$$.ctx[11];
  }
  set show_label(show_label) {
    this.$$set({ show_label });
    flush();
  }
  get container() {
    return this.$$.ctx[12];
  }
  set container(container) {
    this.$$set({ container });
    flush();
  }
  get scale() {
    return this.$$.ctx[13];
  }
  set scale(scale) {
    this.$$set({ scale });
    flush();
  }
  get min_width() {
    return this.$$.ctx[14];
  }
  set min_width(min_width) {
    this.$$set({ min_width });
    flush();
  }
  get loading_status() {
    return this.$$.ctx[1];
  }
  set loading_status(loading_status) {
    this.$$set({ loading_status });
    flush();
  }
  get autoplay() {
    return this.$$.ctx[34];
  }
  set autoplay(autoplay) {
    this.$$set({ autoplay });
    flush();
  }
  get loop() {
    return this.$$.ctx[15];
  }
  set loop(loop) {
    this.$$set({ loop });
    flush();
  }
  get show_download_button() {
    return this.$$.ctx[16];
  }
  set show_download_button(show_download_button) {
    this.$$set({ show_download_button });
    flush();
  }
  get show_share_button() {
    return this.$$.ctx[17];
  }
  set show_share_button(show_share_button) {
    this.$$set({ show_share_button });
    flush();
  }
  get editable() {
    return this.$$.ctx[18];
  }
  set editable(editable) {
    this.$$set({ editable });
    flush();
  }
  get waveform_options() {
    return this.$$.ctx[19];
  }
  set waveform_options(waveform_options) {
    this.$$set({ waveform_options });
    flush();
  }
  get pending() {
    return this.$$.ctx[20];
  }
  set pending(pending) {
    this.$$set({ pending });
    flush();
  }
  get streaming() {
    return this.$$.ctx[21];
  }
  set streaming(streaming) {
    this.$$set({ streaming });
    flush();
  }
  get stream_every() {
    return this.$$.ctx[22];
  }
  set stream_every(stream_every) {
    this.$$set({ stream_every });
    flush();
  }
  get input_ready() {
    return this.$$.ctx[33];
  }
  set input_ready(input_ready) {
    this.$$set({ input_ready });
    flush();
  }
  get recording() {
    return this.$$.ctx[2];
  }
  set recording(recording) {
    this.$$set({ recording });
    flush();
  }
  get modify_stream_state() {
    return this.$$.ctx[35];
  }
  get get_stream_state() {
    return this.$$.ctx[36];
  }
  get set_time_limit() {
    return this.$$.ctx[3];
  }
  set set_time_limit(set_time_limit) {
    this.$$set({ set_time_limit });
    flush();
  }
  get gradio() {
    return this.$$.ctx[23];
  }
  set gradio(gradio) {
    this.$$set({ gradio });
    flush();
  }
}
const Index$1 = Index;
export {
  default2 as BaseExample,
  InteractiveAudio$1 as BaseInteractiveAudio,
  AudioPlayer as BasePlayer,
  StaticAudio as BaseStaticAudio,
  Index$1 as default
};
