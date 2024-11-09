import { SvelteComponent, init, safe_not_equal, svg_element, claim_svg_element, children, detach, attr, insert_hydration, append_hydration, noop, text, claim_text, empty, group_outros, transition_out, check_outros, transition_in, create_component, claim_component, mount_component, destroy_component, element, claim_element, listen, run_all, onMount, binding_callbacks, space, claim_space, set_style, toggle_class, set_data, bind, add_flush_callback, get_svelte_dataset, createEventDispatcher, bubble } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { x as format_time, u as uploadToHuggingFace, I as IconButton } from "./2.BqWhUxOo.js";
import { B as BlockLabel } from "./BlockLabel.BTMmUS9o.js";
import { E as Empty } from "./Empty.C5uBb2Fk.js";
import { S as ShareButton } from "./ShareButton.B3gcBIAK.js";
import { D as Download } from "./Download.BLM_J5wv.js";
import { M as Music } from "./Music.BKn1BNLT.js";
import { I as IconButtonWrapper } from "./IconButtonWrapper.IfQYleUI.js";
import { P as Pause, T as Trim } from "./Trim.CQ15_So8.js";
import { P as Play } from "./Play.wmWinRDD.js";
import { U as Undo } from "./Undo.BEjgqHJW.js";
import { r as resolve_wasm_src } from "./file-url.Bs-FMz4v.js";
/* empty css                                                    */
import { H as Hls } from "./hls.CFPBCiRi.js";
import { D as DownloadLink } from "./DownloadLink.CzZp0moC.js";
function create_fragment$9(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        fill: true,
        "stroke-width": true,
        viewBox: true,
        color: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", {
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true,
        d: true
      });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "stroke", "currentColor");
      attr(path, "stroke-width", "1.5");
      attr(path, "stroke-linecap", "round");
      attr(path, "stroke-linejoin", "round");
      attr(path, "d", "M21.044 5.704a.6.6 0 0 1 .956.483v11.626a.6.6 0 0 1-.956.483l-7.889-5.813a.6.6 0 0 1 0-.966l7.89-5.813ZM10.044 5.704a.6.6 0 0 1 .956.483v11.626a.6.6 0 0 1-.956.483l-7.888-5.813a.6.6 0 0 1 0-.966l7.888-5.813Z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "24px");
      attr(svg, "height", "24px");
      attr(svg, "fill", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
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
class Backward extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$9, safe_not_equal, {});
  }
}
function create_fragment$8(ctx) {
  let svg;
  let path;
  return {
    c() {
      svg = svg_element("svg");
      path = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        xmlns: true,
        width: true,
        height: true,
        fill: true,
        "stroke-width": true,
        viewBox: true,
        color: true
      });
      var svg_nodes = children(svg);
      path = claim_svg_element(svg_nodes, "path", {
        stroke: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true,
        d: true
      });
      children(path).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path, "stroke", "currentColor");
      attr(path, "stroke-width", "1.5");
      attr(path, "stroke-linecap", "round");
      attr(path, "stroke-linejoin", "round");
      attr(path, "d", "M2.956 5.704A.6.6 0 0 0 2 6.187v11.626a.6.6 0 0 0 .956.483l7.889-5.813a.6.6 0 0 0 0-.966l-7.89-5.813ZM13.956 5.704a.6.6 0 0 0-.956.483v11.626a.6.6 0 0 0 .956.483l7.889-5.813a.6.6 0 0 0 0-.966l-7.89-5.813Z");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "width", "24px");
      attr(svg, "height", "24px");
      attr(svg, "fill", "currentColor");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
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
class Forward extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$8, safe_not_equal, {});
  }
}
function create_fragment$7(ctx) {
  let svg;
  let title;
  let t2;
  let path0;
  let path1;
  return {
    c() {
      svg = svg_element("svg");
      title = svg_element("title");
      t2 = text("Low volume");
      path0 = svg_element("path");
      path1 = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        "stroke-width": true,
        fill: true,
        xmlns: true,
        stroke: true,
        color: true
      });
      var svg_nodes = children(svg);
      title = claim_svg_element(svg_nodes, "title", {});
      var title_nodes = children(title);
      t2 = claim_text(title_nodes, "Low volume");
      title_nodes.forEach(detach);
      path0 = claim_svg_element(svg_nodes, "path", {
        d: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", { d: true, "stroke-width": true });
      children(path1).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M19.5 7.5C19.5 7.5 21 9 21 11.5C21 14 19.5 15.5 19.5 15.5");
      attr(path0, "stroke-width", "1.5");
      attr(path0, "stroke-linecap", "round");
      attr(path0, "stroke-linejoin", "round");
      attr(path1, "d", "M2 13.8571V10.1429C2 9.03829 2.89543 8.14286 4 8.14286H6.9C7.09569 8.14286 7.28708 8.08544 7.45046 7.97772L13.4495 4.02228C14.1144 3.5839 15 4.06075 15 4.85714V19.1429C15 19.9392 14.1144 20.4161 13.4495 19.9777L7.45046 16.0223C7.28708 15.9146 7.09569 15.8571 6.9 15.8571H4C2.89543 15.8571 2 14.9617 2 13.8571Z");
      attr(path1, "stroke-width", "1.5");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "stroke", "currentColor");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, title);
      append_hydration(title, t2);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
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
class VolumeLow extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$7, safe_not_equal, {});
  }
}
function create_fragment$6(ctx) {
  let svg;
  let title;
  let t2;
  let path0;
  let path1;
  let path2;
  return {
    c() {
      svg = svg_element("svg");
      title = svg_element("title");
      t2 = text("High volume");
      path0 = svg_element("path");
      path1 = svg_element("path");
      path2 = svg_element("path");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        "stroke-width": true,
        fill: true,
        stroke: true,
        xmlns: true,
        color: true
      });
      var svg_nodes = children(svg);
      title = claim_svg_element(svg_nodes, "title", {});
      var title_nodes = children(title);
      t2 = claim_text(title_nodes, "High volume");
      title_nodes.forEach(detach);
      path0 = claim_svg_element(svg_nodes, "path", { d: true, "stroke-width": true });
      children(path0).forEach(detach);
      path1 = claim_svg_element(svg_nodes, "path", {
        d: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path1).forEach(detach);
      path2 = claim_svg_element(svg_nodes, "path", {
        d: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path2).forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M1 13.8571V10.1429C1 9.03829 1.89543 8.14286 3 8.14286H5.9C6.09569 8.14286 6.28708 8.08544 6.45046 7.97772L12.4495 4.02228C13.1144 3.5839 14 4.06075 14 4.85714V19.1429C14 19.9392 13.1144 20.4161 12.4495 19.9777L6.45046 16.0223C6.28708 15.9146 6.09569 15.8571 5.9 15.8571H3C1.89543 15.8571 1 14.9617 1 13.8571Z");
      attr(path0, "stroke-width", "1.5");
      attr(path1, "d", "M17.5 7.5C17.5 7.5 19 9 19 11.5C19 14 17.5 15.5 17.5 15.5");
      attr(path1, "stroke-width", "1.5");
      attr(path1, "stroke-linecap", "round");
      attr(path1, "stroke-linejoin", "round");
      attr(path2, "d", "M20.5 4.5C20.5 4.5 23 7 23 11.5C23 16 20.5 18.5 20.5 18.5");
      attr(path2, "stroke-width", "1.5");
      attr(path2, "stroke-linecap", "round");
      attr(path2, "stroke-linejoin", "round");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "fill", "none");
      attr(svg, "stroke", "currentColor");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, title);
      append_hydration(title, t2);
      append_hydration(svg, path0);
      append_hydration(svg, path1);
      append_hydration(svg, path2);
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
class VolumeHigh extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$6, safe_not_equal, {});
  }
}
function create_fragment$5(ctx) {
  let svg;
  let title;
  let t2;
  let g;
  let path0;
  let path1;
  let defs;
  let clipPath;
  let rect;
  return {
    c() {
      svg = svg_element("svg");
      title = svg_element("title");
      t2 = text("Muted volume");
      g = svg_element("g");
      path0 = svg_element("path");
      path1 = svg_element("path");
      defs = svg_element("defs");
      clipPath = svg_element("clipPath");
      rect = svg_element("rect");
      this.h();
    },
    l(nodes) {
      svg = claim_svg_element(nodes, "svg", {
        width: true,
        height: true,
        viewBox: true,
        "stroke-width": true,
        fill: true,
        xmlns: true,
        stroke: true,
        color: true
      });
      var svg_nodes = children(svg);
      title = claim_svg_element(svg_nodes, "title", {});
      var title_nodes = children(title);
      t2 = claim_text(title_nodes, "Muted volume");
      title_nodes.forEach(detach);
      g = claim_svg_element(svg_nodes, "g", { "clip-path": true });
      var g_nodes = children(g);
      path0 = claim_svg_element(g_nodes, "path", {
        d: true,
        "stroke-width": true,
        "stroke-linecap": true,
        "stroke-linejoin": true
      });
      children(path0).forEach(detach);
      path1 = claim_svg_element(g_nodes, "path", { d: true, "stroke-width": true });
      children(path1).forEach(detach);
      g_nodes.forEach(detach);
      defs = claim_svg_element(svg_nodes, "defs", {});
      var defs_nodes = children(defs);
      clipPath = claim_svg_element(defs_nodes, "clipPath", { id: true });
      var clipPath_nodes = children(clipPath);
      rect = claim_svg_element(clipPath_nodes, "rect", { width: true, height: true, fill: true });
      children(rect).forEach(detach);
      clipPath_nodes.forEach(detach);
      defs_nodes.forEach(detach);
      svg_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(path0, "d", "M18 14L20.0005 12M22 10L20.0005 12M20.0005 12L18 10M20.0005 12L22 14");
      attr(path0, "stroke-width", "1.5");
      attr(path0, "stroke-linecap", "round");
      attr(path0, "stroke-linejoin", "round");
      attr(path1, "d", "M2 13.8571V10.1429C2 9.03829 2.89543 8.14286 4 8.14286H6.9C7.09569 8.14286 7.28708 8.08544 7.45046 7.97772L13.4495 4.02228C14.1144 3.5839 15 4.06075 15 4.85714V19.1429C15 19.9392 14.1144 20.4161 13.4495 19.9777L7.45046 16.0223C7.28708 15.9146 7.09569 15.8571 6.9 15.8571H4C2.89543 15.8571 2 14.9617 2 13.8571Z");
      attr(path1, "stroke-width", "1.5");
      attr(g, "clip-path", "url(#clip0_3173_16686)");
      attr(rect, "width", "24");
      attr(rect, "height", "24");
      attr(rect, "fill", "white");
      attr(clipPath, "id", "clip0_3173_16686");
      attr(svg, "width", "100%");
      attr(svg, "height", "100%");
      attr(svg, "viewBox", "0 0 24 24");
      attr(svg, "stroke-width", "1.5");
      attr(svg, "fill", "none");
      attr(svg, "xmlns", "http://www.w3.org/2000/svg");
      attr(svg, "stroke", "currentColor");
      attr(svg, "color", "currentColor");
    },
    m(target, anchor) {
      insert_hydration(target, svg, anchor);
      append_hydration(svg, title);
      append_hydration(title, t2);
      append_hydration(svg, g);
      append_hydration(g, path0);
      append_hydration(g, path1);
      append_hydration(svg, defs);
      append_hydration(defs, clipPath);
      append_hydration(clipPath, rect);
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
class VolumeMuted extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, null, create_fragment$5, safe_not_equal, {});
  }
}
var __awaiter$3 = function(thisArg, _arguments, P, generator) {
  function adopt(value) {
    return value instanceof P ? value : new P(function(resolve) {
      resolve(value);
    });
  }
  return new (P || (P = Promise))(function(resolve, reject) {
    function fulfilled(value) {
      try {
        step(generator.next(value));
      } catch (e2) {
        reject(e2);
      }
    }
    function rejected(value) {
      try {
        step(generator["throw"](value));
      } catch (e2) {
        reject(e2);
      }
    }
    function step(result) {
      result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
    }
    step((generator = generator.apply(thisArg, _arguments || [])).next());
  });
};
function decode(audioData, sampleRate) {
  return __awaiter$3(this, void 0, void 0, function* () {
    const audioCtx = new AudioContext({ sampleRate });
    const decode2 = audioCtx.decodeAudioData(audioData);
    return decode2.finally(() => audioCtx.close());
  });
}
function normalize(channelData) {
  const firstChannel = channelData[0];
  if (firstChannel.some((n2) => n2 > 1 || n2 < -1)) {
    const length = firstChannel.length;
    let max = 0;
    for (let i2 = 0; i2 < length; i2++) {
      const absN = Math.abs(firstChannel[i2]);
      if (absN > max)
        max = absN;
    }
    for (const channel of channelData) {
      for (let i2 = 0; i2 < length; i2++) {
        channel[i2] /= max;
      }
    }
  }
  return channelData;
}
function createBuffer(channelData, duration) {
  if (typeof channelData[0] === "number")
    channelData = [channelData];
  normalize(channelData);
  return {
    duration,
    length: channelData[0].length,
    sampleRate: channelData[0].length / duration,
    numberOfChannels: channelData.length,
    getChannelData: (i2) => channelData === null || channelData === void 0 ? void 0 : channelData[i2],
    copyFromChannel: AudioBuffer.prototype.copyFromChannel,
    copyToChannel: AudioBuffer.prototype.copyToChannel
  };
}
const Decoder = {
  decode,
  createBuffer
};
var __awaiter$2 = function(thisArg, _arguments, P, generator) {
  function adopt(value) {
    return value instanceof P ? value : new P(function(resolve) {
      resolve(value);
    });
  }
  return new (P || (P = Promise))(function(resolve, reject) {
    function fulfilled(value) {
      try {
        step(generator.next(value));
      } catch (e2) {
        reject(e2);
      }
    }
    function rejected(value) {
      try {
        step(generator["throw"](value));
      } catch (e2) {
        reject(e2);
      }
    }
    function step(result) {
      result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
    }
    step((generator = generator.apply(thisArg, _arguments || [])).next());
  });
};
function fetchBlob(url, progressCallback, requestInit) {
  var _a, _b;
  return __awaiter$2(this, void 0, void 0, function* () {
    const response = yield fetch(url, requestInit);
    {
      const reader = (_a = response.clone().body) === null || _a === void 0 ? void 0 : _a.getReader();
      const contentLength = Number((_b = response.headers) === null || _b === void 0 ? void 0 : _b.get("Content-Length"));
      let receivedLength = 0;
      const processChunk = (done, value) => __awaiter$2(this, void 0, void 0, function* () {
        if (done)
          return;
        receivedLength += (value === null || value === void 0 ? void 0 : value.length) || 0;
        const percentage = Math.round(receivedLength / contentLength * 100);
        progressCallback(percentage);
        return reader === null || reader === void 0 ? void 0 : reader.read().then(({ done: done2, value: value2 }) => processChunk(done2, value2));
      });
      reader === null || reader === void 0 ? void 0 : reader.read().then(({ done, value }) => processChunk(done, value));
    }
    return response.blob();
  });
}
const Fetcher = {
  fetchBlob
};
class EventEmitter {
  constructor() {
    this.listeners = {};
    this.on = this.addEventListener;
    this.un = this.removeEventListener;
  }
  /** Add an event listener */
  addEventListener(event, listener, options) {
    if (!this.listeners[event]) {
      this.listeners[event] = /* @__PURE__ */ new Set();
    }
    this.listeners[event].add(listener);
    if (options === null || options === void 0 ? void 0 : options.once) {
      const unsubscribeOnce = () => {
        this.removeEventListener(event, unsubscribeOnce);
        this.removeEventListener(event, listener);
      };
      this.addEventListener(event, unsubscribeOnce);
      return unsubscribeOnce;
    }
    return () => this.removeEventListener(event, listener);
  }
  removeEventListener(event, listener) {
    var _a;
    (_a = this.listeners[event]) === null || _a === void 0 ? void 0 : _a.delete(listener);
  }
  /** Subscribe to an event only once */
  once(event, listener) {
    return this.on(event, listener, { once: true });
  }
  /** Clear all events */
  unAll() {
    this.listeners = {};
  }
  /** Emit an event */
  emit(eventName, ...args) {
    if (this.listeners[eventName]) {
      this.listeners[eventName].forEach((listener) => listener(...args));
    }
  }
}
class Player extends EventEmitter {
  constructor(options) {
    super();
    this.isExternalMedia = false;
    if (options.media) {
      this.media = options.media;
      this.isExternalMedia = true;
    } else {
      this.media = document.createElement("audio");
    }
    if (options.mediaControls) {
      this.media.controls = true;
    }
    if (options.autoplay) {
      this.media.autoplay = true;
    }
    if (options.playbackRate != null) {
      this.onceMediaEvent("canplay", () => {
        if (options.playbackRate != null) {
          this.media.playbackRate = options.playbackRate;
        }
      });
    }
  }
  onMediaEvent(event, callback, options) {
    this.media.addEventListener(event, callback, options);
    return () => this.media.removeEventListener(event, callback);
  }
  onceMediaEvent(event, callback) {
    return this.onMediaEvent(event, callback, { once: true });
  }
  getSrc() {
    return this.media.currentSrc || this.media.src || "";
  }
  revokeSrc() {
    const src = this.getSrc();
    if (src.startsWith("blob:")) {
      URL.revokeObjectURL(src);
    }
  }
  setSrc(url, blob) {
    const src = this.getSrc();
    if (src === url)
      return;
    this.revokeSrc();
    const newSrc = blob instanceof Blob ? URL.createObjectURL(blob) : url;
    this.media.src = newSrc;
    this.media.load();
  }
  destroy() {
    this.media.pause();
    if (this.isExternalMedia)
      return;
    this.media.remove();
    this.revokeSrc();
    this.media.src = "";
    this.media.load();
  }
  setMediaElement(element2) {
    this.media = element2;
  }
  /** Start playing the audio */
  play() {
    return this.media.play();
  }
  /** Pause the audio */
  pause() {
    this.media.pause();
  }
  /** Check if the audio is playing */
  isPlaying() {
    return !this.media.paused && !this.media.ended;
  }
  /** Jumpt to a specific time in the audio (in seconds) */
  setTime(time) {
    this.media.currentTime = time;
  }
  /** Get the duration of the audio in seconds */
  getDuration() {
    return this.media.duration;
  }
  /** Get the current audio position in seconds */
  getCurrentTime() {
    return this.media.currentTime;
  }
  /** Get the audio volume */
  getVolume() {
    return this.media.volume;
  }
  /** Set the audio volume */
  setVolume(volume) {
    this.media.volume = volume;
  }
  /** Get the audio muted state */
  getMuted() {
    return this.media.muted;
  }
  /** Mute or unmute the audio */
  setMuted(muted) {
    this.media.muted = muted;
  }
  /** Get the playback speed */
  getPlaybackRate() {
    return this.media.playbackRate;
  }
  /** Set the playback speed, pass an optional false to NOT preserve the pitch */
  setPlaybackRate(rate, preservePitch) {
    if (preservePitch != null) {
      this.media.preservesPitch = preservePitch;
    }
    this.media.playbackRate = rate;
  }
  /** Get the HTML media element */
  getMediaElement() {
    return this.media;
  }
  /** Set a sink id to change the audio output device */
  setSinkId(sinkId) {
    const media = this.media;
    return media.setSinkId(sinkId);
  }
}
function makeDraggable(element2, onDrag, onStart, onEnd, threshold = 5) {
  let unsub = () => {
    return;
  };
  if (!element2)
    return unsub;
  const down = (e2) => {
    if (e2.button === 2)
      return;
    e2.preventDefault();
    e2.stopPropagation();
    element2.style.touchAction = "none";
    let startX = e2.clientX;
    let startY = e2.clientY;
    let isDragging = false;
    const move = (e3) => {
      e3.preventDefault();
      e3.stopPropagation();
      const x = e3.clientX;
      const y = e3.clientY;
      if (isDragging || Math.abs(x - startX) >= threshold || Math.abs(y - startY) >= threshold) {
        const { left, top } = element2.getBoundingClientRect();
        if (!isDragging) {
          isDragging = true;
          onStart === null || onStart === void 0 ? void 0 : onStart(startX - left, startY - top);
        }
        onDrag(x - startX, y - startY, x - left, y - top);
        startX = x;
        startY = y;
      }
    };
    const click = (e3) => {
      if (isDragging) {
        e3.preventDefault();
        e3.stopPropagation();
      }
    };
    const up = () => {
      element2.style.touchAction = "";
      if (isDragging) {
        onEnd === null || onEnd === void 0 ? void 0 : onEnd();
      }
      unsub();
    };
    document.addEventListener("pointermove", move);
    document.addEventListener("pointerup", up);
    document.addEventListener("pointerleave", up);
    document.addEventListener("click", click, true);
    unsub = () => {
      document.removeEventListener("pointermove", move);
      document.removeEventListener("pointerup", up);
      document.removeEventListener("pointerleave", up);
      setTimeout(() => {
        document.removeEventListener("click", click, true);
      }, 10);
    };
  };
  element2.addEventListener("pointerdown", down);
  return () => {
    unsub();
    element2.removeEventListener("pointerdown", down);
  };
}
class Renderer extends EventEmitter {
  constructor(options, audioElement) {
    super();
    this.timeouts = [];
    this.isScrolling = false;
    this.audioData = null;
    this.resizeObserver = null;
    this.isDragging = false;
    this.options = options;
    const parent = this.parentFromOptionsContainer(options.container);
    this.parent = parent;
    const [div, shadow] = this.initHtml();
    parent.appendChild(div);
    this.container = div;
    this.scrollContainer = shadow.querySelector(".scroll");
    this.wrapper = shadow.querySelector(".wrapper");
    this.canvasWrapper = shadow.querySelector(".canvases");
    this.progressWrapper = shadow.querySelector(".progress");
    this.cursor = shadow.querySelector(".cursor");
    if (audioElement) {
      shadow.appendChild(audioElement);
    }
    this.initEvents();
  }
  parentFromOptionsContainer(container) {
    let parent;
    if (typeof container === "string") {
      parent = document.querySelector(container);
    } else if (container instanceof HTMLElement) {
      parent = container;
    }
    if (!parent) {
      throw new Error("Container not found");
    }
    return parent;
  }
  initEvents() {
    const getClickPosition = (e2) => {
      const rect = this.wrapper.getBoundingClientRect();
      const x = e2.clientX - rect.left;
      const y = e2.clientX - rect.left;
      const relativeX = x / rect.width;
      const relativeY = y / rect.height;
      return [relativeX, relativeY];
    };
    this.wrapper.addEventListener("click", (e2) => {
      const [x, y] = getClickPosition(e2);
      this.emit("click", x, y);
    });
    this.wrapper.addEventListener("dblclick", (e2) => {
      const [x, y] = getClickPosition(e2);
      this.emit("dblclick", x, y);
    });
    if (this.options.dragToSeek) {
      this.initDrag();
    }
    this.scrollContainer.addEventListener("scroll", () => {
      const { scrollLeft, scrollWidth, clientWidth } = this.scrollContainer;
      const startX = scrollLeft / scrollWidth;
      const endX = (scrollLeft + clientWidth) / scrollWidth;
      this.emit("scroll", startX, endX);
    });
    const delay = this.createDelay(100);
    this.resizeObserver = new ResizeObserver(() => {
      delay(() => this.reRender());
    });
    this.resizeObserver.observe(this.scrollContainer);
  }
  initDrag() {
    makeDraggable(
      this.wrapper,
      // On drag
      (_, __, x) => {
        this.emit("drag", Math.max(0, Math.min(1, x / this.wrapper.getBoundingClientRect().width)));
      },
      // On start drag
      () => this.isDragging = true,
      // On end drag
      () => this.isDragging = false
    );
  }
  getHeight() {
    const defaultHeight = 128;
    if (this.options.height == null)
      return defaultHeight;
    if (!isNaN(Number(this.options.height)))
      return Number(this.options.height);
    if (this.options.height === "auto")
      return this.parent.clientHeight || defaultHeight;
    return defaultHeight;
  }
  initHtml() {
    const div = document.createElement("div");
    const shadow = div.attachShadow({ mode: "open" });
    shadow.innerHTML = `
      <style>
        :host {
          user-select: none;
          min-width: 1px;
        }
        :host audio {
          display: block;
          width: 100%;
        }
        :host .scroll {
          overflow-x: auto;
          overflow-y: hidden;
          width: 100%;
          position: relative;
        }
        :host .noScrollbar {
          scrollbar-color: transparent;
          scrollbar-width: none;
        }
        :host .noScrollbar::-webkit-scrollbar {
          display: none;
          -webkit-appearance: none;
        }
        :host .wrapper {
          position: relative;
          overflow: visible;
          z-index: 2;
        }
        :host .canvases {
          min-height: ${this.getHeight()}px;
        }
        :host .canvases > div {
          position: relative;
        }
        :host canvas {
          display: block;
          position: absolute;
          top: 0;
          image-rendering: pixelated;
        }
        :host .progress {
          pointer-events: none;
          position: absolute;
          z-index: 2;
          top: 0;
          left: 0;
          width: 0;
          height: 100%;
          overflow: hidden;
        }
        :host .progress > div {
          position: relative;
        }
        :host .cursor {
          pointer-events: none;
          position: absolute;
          z-index: 5;
          top: 0;
          left: 0;
          height: 100%;
          border-radius: 2px;
        }
      </style>

      <div class="scroll" part="scroll">
        <div class="wrapper" part="wrapper">
          <div class="canvases"></div>
          <div class="progress" part="progress"></div>
          <div class="cursor" part="cursor"></div>
        </div>
      </div>
    `;
    return [div, shadow];
  }
  /** Wavesurfer itself calls this method. Do not call it manually. */
  setOptions(options) {
    if (this.options.container !== options.container) {
      const newParent = this.parentFromOptionsContainer(options.container);
      newParent.appendChild(this.container);
      this.parent = newParent;
    }
    if (options.dragToSeek && !this.options.dragToSeek) {
      this.initDrag();
    }
    this.options = options;
    this.reRender();
  }
  getWrapper() {
    return this.wrapper;
  }
  getScroll() {
    return this.scrollContainer.scrollLeft;
  }
  destroy() {
    var _a;
    this.container.remove();
    (_a = this.resizeObserver) === null || _a === void 0 ? void 0 : _a.disconnect();
  }
  createDelay(delayMs = 10) {
    const context = {};
    this.timeouts.push(context);
    return (callback) => {
      context.timeout && clearTimeout(context.timeout);
      context.timeout = setTimeout(callback, delayMs);
    };
  }
  // Convert array of color values to linear gradient
  convertColorValues(color) {
    if (!Array.isArray(color))
      return color || "";
    if (color.length < 2)
      return color[0] || "";
    const canvasElement = document.createElement("canvas");
    const ctx = canvasElement.getContext("2d");
    const gradient = ctx.createLinearGradient(0, 0, 0, canvasElement.height);
    const colorStopPercentage = 1 / (color.length - 1);
    color.forEach((color2, index) => {
      const offset = index * colorStopPercentage;
      gradient.addColorStop(offset, color2);
    });
    return gradient;
  }
  renderBarWaveform(channelData, options, ctx, vScale) {
    const topChannel = channelData[0];
    const bottomChannel = channelData[1] || channelData[0];
    const length = topChannel.length;
    const { width, height } = ctx.canvas;
    const halfHeight = height / 2;
    const pixelRatio = window.devicePixelRatio || 1;
    const barWidth = options.barWidth ? options.barWidth * pixelRatio : 1;
    const barGap = options.barGap ? options.barGap * pixelRatio : options.barWidth ? barWidth / 2 : 0;
    const barRadius = options.barRadius || 0;
    const barIndexScale = width / (barWidth + barGap) / length;
    const rectFn = barRadius && "roundRect" in ctx ? "roundRect" : "rect";
    ctx.beginPath();
    let prevX = 0;
    let maxTop = 0;
    let maxBottom = 0;
    for (let i2 = 0; i2 <= length; i2++) {
      const x = Math.round(i2 * barIndexScale);
      if (x > prevX) {
        const topBarHeight = Math.round(maxTop * halfHeight * vScale);
        const bottomBarHeight = Math.round(maxBottom * halfHeight * vScale);
        const barHeight = topBarHeight + bottomBarHeight || 1;
        let y = halfHeight - topBarHeight;
        if (options.barAlign === "top") {
          y = 0;
        } else if (options.barAlign === "bottom") {
          y = height - barHeight;
        }
        ctx[rectFn](prevX * (barWidth + barGap), y, barWidth, barHeight, barRadius);
        prevX = x;
        maxTop = 0;
        maxBottom = 0;
      }
      const magnitudeTop = Math.abs(topChannel[i2] || 0);
      const magnitudeBottom = Math.abs(bottomChannel[i2] || 0);
      if (magnitudeTop > maxTop)
        maxTop = magnitudeTop;
      if (magnitudeBottom > maxBottom)
        maxBottom = magnitudeBottom;
    }
    ctx.fill();
    ctx.closePath();
  }
  renderLineWaveform(channelData, _options, ctx, vScale) {
    const drawChannel = (index) => {
      const channel = channelData[index] || channelData[0];
      const length = channel.length;
      const { height } = ctx.canvas;
      const halfHeight = height / 2;
      const hScale = ctx.canvas.width / length;
      ctx.moveTo(0, halfHeight);
      let prevX = 0;
      let max = 0;
      for (let i2 = 0; i2 <= length; i2++) {
        const x = Math.round(i2 * hScale);
        if (x > prevX) {
          const h = Math.round(max * halfHeight * vScale) || 1;
          const y = halfHeight + h * (index === 0 ? -1 : 1);
          ctx.lineTo(prevX, y);
          prevX = x;
          max = 0;
        }
        const value = Math.abs(channel[i2] || 0);
        if (value > max)
          max = value;
      }
      ctx.lineTo(prevX, halfHeight);
    };
    ctx.beginPath();
    drawChannel(0);
    drawChannel(1);
    ctx.fill();
    ctx.closePath();
  }
  renderWaveform(channelData, options, ctx) {
    ctx.fillStyle = this.convertColorValues(options.waveColor);
    if (options.renderFunction) {
      options.renderFunction(channelData, ctx);
      return;
    }
    let vScale = options.barHeight || 1;
    if (options.normalize) {
      const max = Array.from(channelData[0]).reduce((max2, value) => Math.max(max2, Math.abs(value)), 0);
      vScale = max ? 1 / max : 1;
    }
    if (options.barWidth || options.barGap || options.barAlign) {
      this.renderBarWaveform(channelData, options, ctx, vScale);
      return;
    }
    this.renderLineWaveform(channelData, options, ctx, vScale);
  }
  renderSingleCanvas(channelData, options, width, height, start, end, canvasContainer, progressContainer) {
    const pixelRatio = window.devicePixelRatio || 1;
    const canvas = document.createElement("canvas");
    const length = channelData[0].length;
    canvas.width = Math.round(width * (end - start) / length);
    canvas.height = height * pixelRatio;
    canvas.style.width = `${Math.floor(canvas.width / pixelRatio)}px`;
    canvas.style.height = `${height}px`;
    canvas.style.left = `${Math.floor(start * width / pixelRatio / length)}px`;
    canvasContainer.appendChild(canvas);
    const ctx = canvas.getContext("2d");
    this.renderWaveform(channelData.map((channel) => channel.slice(start, end)), options, ctx);
    if (canvas.width > 0 && canvas.height > 0) {
      const progressCanvas = canvas.cloneNode();
      const progressCtx = progressCanvas.getContext("2d");
      progressCtx.drawImage(canvas, 0, 0);
      progressCtx.globalCompositeOperation = "source-in";
      progressCtx.fillStyle = this.convertColorValues(options.progressColor);
      progressCtx.fillRect(0, 0, canvas.width, canvas.height);
      progressContainer.appendChild(progressCanvas);
    }
  }
  renderChannel(channelData, options, width) {
    const canvasContainer = document.createElement("div");
    const height = this.getHeight();
    canvasContainer.style.height = `${height}px`;
    this.canvasWrapper.style.minHeight = `${height}px`;
    this.canvasWrapper.appendChild(canvasContainer);
    const progressContainer = canvasContainer.cloneNode();
    this.progressWrapper.appendChild(progressContainer);
    const { scrollLeft, scrollWidth, clientWidth } = this.scrollContainer;
    const len = channelData[0].length;
    const scale = len / scrollWidth;
    let viewportWidth = Math.min(Renderer.MAX_CANVAS_WIDTH, clientWidth);
    if (options.barWidth || options.barGap) {
      const barWidth = options.barWidth || 0.5;
      const barGap = options.barGap || barWidth / 2;
      const totalBarWidth = barWidth + barGap;
      if (viewportWidth % totalBarWidth !== 0) {
        viewportWidth = Math.floor(viewportWidth / totalBarWidth) * totalBarWidth;
      }
    }
    const start = Math.floor(Math.abs(scrollLeft) * scale);
    const end = Math.floor(start + viewportWidth * scale);
    const viewportLen = end - start;
    const draw = (start2, end2) => {
      this.renderSingleCanvas(channelData, options, width, height, Math.max(0, start2), Math.min(end2, len), canvasContainer, progressContainer);
    };
    const headDelay = this.createDelay();
    const tailDelay = this.createDelay();
    const renderHead = (fromIndex, toIndex) => {
      draw(fromIndex, toIndex);
      if (fromIndex > 0) {
        headDelay(() => {
          renderHead(fromIndex - viewportLen, toIndex - viewportLen);
        });
      }
    };
    const renderTail = (fromIndex, toIndex) => {
      draw(fromIndex, toIndex);
      if (toIndex < len) {
        tailDelay(() => {
          renderTail(fromIndex + viewportLen, toIndex + viewportLen);
        });
      }
    };
    renderHead(start, end);
    if (end < len) {
      renderTail(end, end + viewportLen);
    }
  }
  render(audioData) {
    this.timeouts.forEach((context) => context.timeout && clearTimeout(context.timeout));
    this.timeouts = [];
    this.canvasWrapper.innerHTML = "";
    this.progressWrapper.innerHTML = "";
    this.wrapper.style.width = "";
    if (this.options.width != null) {
      this.scrollContainer.style.width = typeof this.options.width === "number" ? `${this.options.width}px` : this.options.width;
    }
    const pixelRatio = window.devicePixelRatio || 1;
    const parentWidth = this.scrollContainer.clientWidth;
    const scrollWidth = Math.ceil(audioData.duration * (this.options.minPxPerSec || 0));
    this.isScrolling = scrollWidth > parentWidth;
    const useParentWidth = this.options.fillParent && !this.isScrolling;
    const width = (useParentWidth ? parentWidth : scrollWidth) * pixelRatio;
    this.wrapper.style.width = useParentWidth ? "100%" : `${scrollWidth}px`;
    this.scrollContainer.style.overflowX = this.isScrolling ? "auto" : "hidden";
    this.scrollContainer.classList.toggle("noScrollbar", !!this.options.hideScrollbar);
    this.cursor.style.backgroundColor = `${this.options.cursorColor || this.options.progressColor}`;
    this.cursor.style.width = `${this.options.cursorWidth}px`;
    if (this.options.splitChannels) {
      for (let i2 = 0; i2 < audioData.numberOfChannels; i2++) {
        const options = Object.assign(Object.assign({}, this.options), this.options.splitChannels[i2]);
        this.renderChannel([audioData.getChannelData(i2)], options, width);
      }
    } else {
      const channels = [audioData.getChannelData(0)];
      if (audioData.numberOfChannels > 1)
        channels.push(audioData.getChannelData(1));
      this.renderChannel(channels, this.options, width);
    }
    this.audioData = audioData;
    this.emit("render");
  }
  reRender() {
    if (!this.audioData)
      return;
    const oldCursorPosition = this.progressWrapper.clientWidth;
    this.render(this.audioData);
    const newCursortPosition = this.progressWrapper.clientWidth;
    this.scrollContainer.scrollLeft += newCursortPosition - oldCursorPosition;
  }
  zoom(minPxPerSec) {
    this.options.minPxPerSec = minPxPerSec;
    this.reRender();
  }
  scrollIntoView(progress, isPlaying = false) {
    const { clientWidth, scrollLeft, scrollWidth } = this.scrollContainer;
    const progressWidth = scrollWidth * progress;
    const center = clientWidth / 2;
    const minScroll = isPlaying && this.options.autoCenter && !this.isDragging ? center : clientWidth;
    if (progressWidth > scrollLeft + minScroll || progressWidth < scrollLeft) {
      if (this.options.autoCenter && !this.isDragging) {
        const minDiff = center / 20;
        if (progressWidth - (scrollLeft + center) >= minDiff && progressWidth < scrollLeft + clientWidth) {
          this.scrollContainer.scrollLeft += minDiff;
        } else {
          this.scrollContainer.scrollLeft = progressWidth - center;
        }
      } else if (this.isDragging) {
        const gap = 10;
        this.scrollContainer.scrollLeft = progressWidth < scrollLeft ? progressWidth - gap : progressWidth - clientWidth + gap;
      } else {
        this.scrollContainer.scrollLeft = progressWidth;
      }
    }
    {
      const { scrollLeft: scrollLeft2 } = this.scrollContainer;
      const startX = scrollLeft2 / scrollWidth;
      const endX = (scrollLeft2 + clientWidth) / scrollWidth;
      this.emit("scroll", startX, endX);
    }
  }
  renderProgress(progress, isPlaying) {
    if (isNaN(progress))
      return;
    const percents = progress * 100;
    this.canvasWrapper.style.clipPath = `polygon(${percents}% 0, 100% 0, 100% 100%, ${percents}% 100%)`;
    this.progressWrapper.style.width = `${percents}%`;
    this.cursor.style.left = `${percents}%`;
    this.cursor.style.marginLeft = Math.round(percents) === 100 ? `-${this.options.cursorWidth}px` : "";
    if (this.isScrolling && this.options.autoScroll) {
      this.scrollIntoView(progress, isPlaying);
    }
  }
}
Renderer.MAX_CANVAS_WIDTH = 4e3;
class Timer extends EventEmitter {
  constructor() {
    super(...arguments);
    this.unsubscribe = () => void 0;
  }
  start() {
    this.unsubscribe = this.on("tick", () => {
      requestAnimationFrame(() => {
        this.emit("tick");
      });
    });
    this.emit("tick");
  }
  stop() {
    this.unsubscribe();
  }
  destroy() {
    this.unsubscribe();
  }
}
var __awaiter$1 = function(thisArg, _arguments, P, generator) {
  function adopt(value) {
    return value instanceof P ? value : new P(function(resolve) {
      resolve(value);
    });
  }
  return new (P || (P = Promise))(function(resolve, reject) {
    function fulfilled(value) {
      try {
        step(generator.next(value));
      } catch (e2) {
        reject(e2);
      }
    }
    function rejected(value) {
      try {
        step(generator["throw"](value));
      } catch (e2) {
        reject(e2);
      }
    }
    function step(result) {
      result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
    }
    step((generator = generator.apply(thisArg, _arguments || [])).next());
  });
};
class WebAudioPlayer extends EventEmitter {
  constructor(audioContext = new AudioContext()) {
    super();
    this.bufferNode = null;
    this.autoplay = false;
    this.playStartTime = 0;
    this.playedDuration = 0;
    this._muted = false;
    this.buffer = null;
    this.currentSrc = "";
    this.paused = true;
    this.crossOrigin = null;
    this.audioContext = audioContext;
    this.gainNode = this.audioContext.createGain();
    this.gainNode.connect(this.audioContext.destination);
  }
  load() {
    return __awaiter$1(this, void 0, void 0, function* () {
      return;
    });
  }
  get src() {
    return this.currentSrc;
  }
  set src(value) {
    this.currentSrc = value;
    fetch(value).then((response) => response.arrayBuffer()).then((arrayBuffer) => this.audioContext.decodeAudioData(arrayBuffer)).then((audioBuffer) => {
      this.buffer = audioBuffer;
      this.emit("loadedmetadata");
      this.emit("canplay");
      if (this.autoplay)
        this.play();
    });
  }
  _play() {
    var _a;
    if (!this.paused)
      return;
    this.paused = false;
    (_a = this.bufferNode) === null || _a === void 0 ? void 0 : _a.disconnect();
    this.bufferNode = this.audioContext.createBufferSource();
    this.bufferNode.buffer = this.buffer;
    this.bufferNode.connect(this.gainNode);
    if (this.playedDuration >= this.duration) {
      this.playedDuration = 0;
    }
    this.bufferNode.start(this.audioContext.currentTime, this.playedDuration);
    this.playStartTime = this.audioContext.currentTime;
    this.bufferNode.onended = () => {
      if (this.currentTime >= this.duration) {
        this.pause();
        this.emit("ended");
      }
    };
  }
  _pause() {
    var _a;
    if (this.paused)
      return;
    this.paused = true;
    (_a = this.bufferNode) === null || _a === void 0 ? void 0 : _a.stop();
    this.playedDuration += this.audioContext.currentTime - this.playStartTime;
  }
  play() {
    return __awaiter$1(this, void 0, void 0, function* () {
      this._play();
      this.emit("play");
    });
  }
  pause() {
    this._pause();
    this.emit("pause");
  }
  setSinkId(deviceId) {
    return __awaiter$1(this, void 0, void 0, function* () {
      const ac = this.audioContext;
      return ac.setSinkId(deviceId);
    });
  }
  get playbackRate() {
    var _a, _b;
    return (_b = (_a = this.bufferNode) === null || _a === void 0 ? void 0 : _a.playbackRate.value) !== null && _b !== void 0 ? _b : 1;
  }
  set playbackRate(value) {
    if (this.bufferNode) {
      this.bufferNode.playbackRate.value = value;
    }
  }
  get currentTime() {
    return this.paused ? this.playedDuration : this.playedDuration + this.audioContext.currentTime - this.playStartTime;
  }
  set currentTime(value) {
    this.emit("seeking");
    if (this.paused) {
      this.playedDuration = value;
    } else {
      this._pause();
      this.playedDuration = value;
      this._play();
    }
    this.emit("timeupdate");
  }
  get duration() {
    var _a;
    return ((_a = this.buffer) === null || _a === void 0 ? void 0 : _a.duration) || 0;
  }
  get volume() {
    return this.gainNode.gain.value;
  }
  set volume(value) {
    this.gainNode.gain.value = value;
    this.emit("volumechange");
  }
  get muted() {
    return this._muted;
  }
  set muted(value) {
    if (this._muted === value)
      return;
    this._muted = value;
    if (this._muted) {
      this.gainNode.disconnect();
    } else {
      this.gainNode.connect(this.audioContext.destination);
    }
  }
  /** Get the GainNode used to play the audio. Can be used to attach filters. */
  getGainNode() {
    return this.gainNode;
  }
}
var __awaiter = function(thisArg, _arguments, P, generator) {
  function adopt(value) {
    return value instanceof P ? value : new P(function(resolve) {
      resolve(value);
    });
  }
  return new (P || (P = Promise))(function(resolve, reject) {
    function fulfilled(value) {
      try {
        step(generator.next(value));
      } catch (e2) {
        reject(e2);
      }
    }
    function rejected(value) {
      try {
        step(generator["throw"](value));
      } catch (e2) {
        reject(e2);
      }
    }
    function step(result) {
      result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
    }
    step((generator = generator.apply(thisArg, _arguments || [])).next());
  });
};
const defaultOptions = {
  waveColor: "#999",
  progressColor: "#555",
  cursorWidth: 1,
  minPxPerSec: 0,
  fillParent: true,
  interact: true,
  dragToSeek: false,
  autoScroll: true,
  autoCenter: true,
  sampleRate: 8e3
};
class WaveSurfer extends Player {
  /** Create a new WaveSurfer instance */
  static create(options) {
    return new WaveSurfer(options);
  }
  /** Create a new WaveSurfer instance */
  constructor(options) {
    const media = options.media || (options.backend === "WebAudio" ? new WebAudioPlayer() : void 0);
    super({
      media,
      mediaControls: options.mediaControls,
      autoplay: options.autoplay,
      playbackRate: options.audioRate
    });
    this.plugins = [];
    this.decodedData = null;
    this.subscriptions = [];
    this.mediaSubscriptions = [];
    this.options = Object.assign({}, defaultOptions, options);
    this.timer = new Timer();
    const audioElement = media ? void 0 : this.getMediaElement();
    this.renderer = new Renderer(this.options, audioElement);
    this.initPlayerEvents();
    this.initRendererEvents();
    this.initTimerEvents();
    this.initPlugins();
    const url = this.options.url || this.getSrc();
    if (url) {
      this.load(url, this.options.peaks, this.options.duration);
    } else if (this.options.peaks && this.options.duration) {
      this.loadPredecoded();
    }
  }
  initTimerEvents() {
    this.subscriptions.push(this.timer.on("tick", () => {
      const currentTime = this.getCurrentTime();
      this.renderer.renderProgress(currentTime / this.getDuration(), true);
      this.emit("timeupdate", currentTime);
      this.emit("audioprocess", currentTime);
    }));
  }
  initPlayerEvents() {
    this.mediaSubscriptions.push(this.onMediaEvent("timeupdate", () => {
      const currentTime = this.getCurrentTime();
      this.renderer.renderProgress(currentTime / this.getDuration(), this.isPlaying());
      this.emit("timeupdate", currentTime);
    }), this.onMediaEvent("play", () => {
      this.emit("play");
      this.timer.start();
    }), this.onMediaEvent("pause", () => {
      this.emit("pause");
      this.timer.stop();
    }), this.onMediaEvent("emptied", () => {
      this.timer.stop();
    }), this.onMediaEvent("ended", () => {
      this.emit("finish");
    }), this.onMediaEvent("seeking", () => {
      this.emit("seeking", this.getCurrentTime());
    }));
  }
  initRendererEvents() {
    this.subscriptions.push(
      // Seek on click
      this.renderer.on("click", (relativeX, relativeY) => {
        if (this.options.interact) {
          this.seekTo(relativeX);
          this.emit("interaction", relativeX * this.getDuration());
          this.emit("click", relativeX, relativeY);
        }
      }),
      // Double click
      this.renderer.on("dblclick", (relativeX, relativeY) => {
        this.emit("dblclick", relativeX, relativeY);
      }),
      // Scroll
      this.renderer.on("scroll", (startX, endX) => {
        const duration = this.getDuration();
        this.emit("scroll", startX * duration, endX * duration);
      }),
      // Redraw
      this.renderer.on("render", () => {
        this.emit("redraw");
      })
    );
    {
      let debounce;
      this.subscriptions.push(this.renderer.on("drag", (relativeX) => {
        if (!this.options.interact)
          return;
        this.renderer.renderProgress(relativeX);
        clearTimeout(debounce);
        debounce = setTimeout(() => {
          this.seekTo(relativeX);
        }, this.isPlaying() ? 0 : 200);
        this.emit("interaction", relativeX * this.getDuration());
        this.emit("drag", relativeX);
      }));
    }
  }
  initPlugins() {
    var _a;
    if (!((_a = this.options.plugins) === null || _a === void 0 ? void 0 : _a.length))
      return;
    this.options.plugins.forEach((plugin) => {
      this.registerPlugin(plugin);
    });
  }
  unsubscribePlayerEvents() {
    this.mediaSubscriptions.forEach((unsubscribe) => unsubscribe());
    this.mediaSubscriptions = [];
  }
  /** Set new wavesurfer options and re-render it */
  setOptions(options) {
    this.options = Object.assign({}, this.options, options);
    this.renderer.setOptions(this.options);
    if (options.audioRate) {
      this.setPlaybackRate(options.audioRate);
    }
    if (options.mediaControls != null) {
      this.getMediaElement().controls = options.mediaControls;
    }
  }
  /** Register a wavesurfer.js plugin */
  registerPlugin(plugin) {
    plugin.init(this);
    this.plugins.push(plugin);
    this.subscriptions.push(plugin.once("destroy", () => {
      this.plugins = this.plugins.filter((p) => p !== plugin);
    }));
    return plugin;
  }
  /** For plugins only: get the waveform wrapper div */
  getWrapper() {
    return this.renderer.getWrapper();
  }
  /** Get the current scroll position in pixels */
  getScroll() {
    return this.renderer.getScroll();
  }
  /** Get all registered plugins */
  getActivePlugins() {
    return this.plugins;
  }
  loadPredecoded() {
    return __awaiter(this, void 0, void 0, function* () {
      if (this.options.peaks && this.options.duration) {
        this.decodedData = Decoder.createBuffer(this.options.peaks, this.options.duration);
        yield Promise.resolve();
        this.renderDecoded();
      }
    });
  }
  renderDecoded() {
    return __awaiter(this, void 0, void 0, function* () {
      if (this.decodedData) {
        this.emit("decode", this.getDuration());
        this.renderer.render(this.decodedData);
      }
    });
  }
  loadAudio(url, blob, channelData, duration) {
    return __awaiter(this, void 0, void 0, function* () {
      this.emit("load", url);
      if (!this.options.media && this.isPlaying())
        this.pause();
      this.decodedData = null;
      if (!blob && !channelData) {
        const onProgress = (percentage) => this.emit("loading", percentage);
        blob = yield Fetcher.fetchBlob(url, onProgress, this.options.fetchParams);
      }
      this.setSrc(url, blob);
      duration = (yield Promise.resolve(duration || this.getDuration())) || (yield new Promise((resolve) => {
        this.onceMediaEvent("loadedmetadata", () => resolve(this.getDuration()));
      })) || (yield Promise.resolve(0));
      if (channelData) {
        this.decodedData = Decoder.createBuffer(channelData, duration);
      } else if (blob) {
        const arrayBuffer = yield blob.arrayBuffer();
        this.decodedData = yield Decoder.decode(arrayBuffer, this.options.sampleRate);
      }
      this.renderDecoded();
      this.emit("ready", this.getDuration());
    });
  }
  /** Load an audio file by URL, with optional pre-decoded audio data */
  load(url, channelData, duration) {
    return __awaiter(this, void 0, void 0, function* () {
      yield this.loadAudio(url, void 0, channelData, duration);
    });
  }
  /** Load an audio blob */
  loadBlob(blob, channelData, duration) {
    return __awaiter(this, void 0, void 0, function* () {
      yield this.loadAudio("blob", blob, channelData, duration);
    });
  }
  /** Zoom the waveform by a given pixels-per-second factor */
  zoom(minPxPerSec) {
    if (!this.decodedData) {
      throw new Error("No audio loaded");
    }
    this.renderer.zoom(minPxPerSec);
    this.emit("zoom", minPxPerSec);
  }
  /** Get the decoded audio data */
  getDecodedData() {
    return this.decodedData;
  }
  /** Get decoded peaks */
  exportPeaks({ channels = 2, maxLength = 8e3, precision = 1e4 } = {}) {
    if (!this.decodedData) {
      throw new Error("The audio has not been decoded yet");
    }
    const maxChannels = Math.min(channels, this.decodedData.numberOfChannels);
    const peaks = [];
    for (let i2 = 0; i2 < maxChannels; i2++) {
      const channel = this.decodedData.getChannelData(i2);
      const data = [];
      const sampleSize = Math.round(channel.length / maxLength);
      for (let i3 = 0; i3 < maxLength; i3++) {
        const sample = channel.slice(i3 * sampleSize, (i3 + 1) * sampleSize);
        const max = Math.max(...sample);
        data.push(Math.round(max * precision) / precision);
      }
      peaks.push(data);
    }
    return peaks;
  }
  /** Get the duration of the audio in seconds */
  getDuration() {
    let duration = super.getDuration() || 0;
    if ((duration === 0 || duration === Infinity) && this.decodedData) {
      duration = this.decodedData.duration;
    }
    return duration;
  }
  /** Toggle if the waveform should react to clicks */
  toggleInteraction(isInteractive) {
    this.options.interact = isInteractive;
  }
  /** Seek to a percentage of audio as [0..1] (0 = beginning, 1 = end) */
  seekTo(progress) {
    const time = this.getDuration() * progress;
    this.setTime(time);
  }
  /** Play or pause the audio */
  playPause() {
    return __awaiter(this, void 0, void 0, function* () {
      return this.isPlaying() ? this.pause() : this.play();
    });
  }
  /** Stop the audio and go to the beginning */
  stop() {
    this.pause();
    this.setTime(0);
  }
  /** Skip N or -N seconds from the current position */
  skip(seconds) {
    this.setTime(this.getCurrentTime() + seconds);
  }
  /** Empty the waveform by loading a tiny silent audio */
  empty() {
    this.load("", [[0]], 1e-3);
  }
  /** Set HTML media element */
  setMediaElement(element2) {
    this.unsubscribePlayerEvents();
    super.setMediaElement(element2);
    this.initPlayerEvents();
  }
  /** Unmount wavesurfer */
  destroy() {
    this.emit("destroy");
    this.plugins.forEach((plugin) => plugin.destroy());
    this.subscriptions.forEach((unsubscribe) => unsubscribe());
    this.unsubscribePlayerEvents();
    this.timer.destroy();
    this.renderer.destroy();
    super.destroy();
  }
}
function audioBufferToWav(audioBuffer) {
  const numOfChan = audioBuffer.numberOfChannels;
  const length = audioBuffer.length * numOfChan * 2 + 44;
  const buffer = new ArrayBuffer(length);
  const view = new DataView(buffer);
  let offset = 0;
  const writeString = function(view2, offset2, string) {
    for (let i2 = 0; i2 < string.length; i2++) {
      view2.setUint8(offset2 + i2, string.charCodeAt(i2));
    }
  };
  writeString(view, offset, "RIFF");
  offset += 4;
  view.setUint32(offset, length - 8, true);
  offset += 4;
  writeString(view, offset, "WAVE");
  offset += 4;
  writeString(view, offset, "fmt ");
  offset += 4;
  view.setUint32(offset, 16, true);
  offset += 4;
  view.setUint16(offset, 1, true);
  offset += 2;
  view.setUint16(offset, numOfChan, true);
  offset += 2;
  view.setUint32(offset, audioBuffer.sampleRate, true);
  offset += 4;
  view.setUint32(offset, audioBuffer.sampleRate * 2 * numOfChan, true);
  offset += 4;
  view.setUint16(offset, numOfChan * 2, true);
  offset += 2;
  view.setUint16(offset, 16, true);
  offset += 2;
  writeString(view, offset, "data");
  offset += 4;
  view.setUint32(offset, audioBuffer.length * numOfChan * 2, true);
  offset += 4;
  for (let i2 = 0; i2 < audioBuffer.length; i2++) {
    for (let channel = 0; channel < numOfChan; channel++) {
      const sample = Math.max(
        -1,
        Math.min(1, audioBuffer.getChannelData(channel)[i2])
      );
      view.setInt16(offset, sample * 32767, true);
      offset += 2;
    }
  }
  return new Uint8Array(buffer);
}
const process_audio = async (audioBuffer, start, end, waveform_sample_rate) => {
  const audioContext = new AudioContext({
    sampleRate: waveform_sample_rate || audioBuffer.sampleRate
  });
  const numberOfChannels = audioBuffer.numberOfChannels;
  const sampleRate = waveform_sample_rate || audioBuffer.sampleRate;
  let trimmedLength = audioBuffer.length;
  let startOffset = 0;
  if (start && end) {
    startOffset = Math.round(start * sampleRate);
    const endOffset = Math.round(end * sampleRate);
    trimmedLength = endOffset - startOffset;
  }
  const trimmedAudioBuffer = audioContext.createBuffer(
    numberOfChannels,
    trimmedLength,
    sampleRate
  );
  for (let channel = 0; channel < numberOfChannels; channel++) {
    const channelData = audioBuffer.getChannelData(channel);
    const trimmedData = trimmedAudioBuffer.getChannelData(channel);
    for (let i2 = 0; i2 < trimmedLength; i2++) {
      trimmedData[i2] = channelData[startOffset + i2];
    }
  }
  return audioBufferToWav(trimmedAudioBuffer);
};
const skip_audio = (waveform, amount) => {
  if (!waveform)
    return;
  waveform.skip(amount);
};
const get_skip_rewind_amount = (audio_duration, skip_length) => {
  if (!skip_length) {
    skip_length = 5;
  }
  return audio_duration / 100 * skip_length || 5;
};
class t {
  constructor() {
    this.listeners = {}, this.on = this.addEventListener, this.un = this.removeEventListener;
  }
  addEventListener(t2, e2, i2) {
    if (this.listeners[t2] || (this.listeners[t2] = /* @__PURE__ */ new Set()), this.listeners[t2].add(e2), null == i2 ? void 0 : i2.once) {
      const i3 = () => {
        this.removeEventListener(t2, i3), this.removeEventListener(t2, e2);
      };
      return this.addEventListener(t2, i3), i3;
    }
    return () => this.removeEventListener(t2, e2);
  }
  removeEventListener(t2, e2) {
    var i2;
    null === (i2 = this.listeners[t2]) || void 0 === i2 || i2.delete(e2);
  }
  once(t2, e2) {
    return this.on(t2, e2, { once: true });
  }
  unAll() {
    this.listeners = {};
  }
  emit(t2, ...e2) {
    this.listeners[t2] && this.listeners[t2].forEach((t3) => t3(...e2));
  }
}
class e extends t {
  constructor(t2) {
    super(), this.subscriptions = [], this.options = t2;
  }
  onInit() {
  }
  init(t2) {
    this.wavesurfer = t2, this.onInit();
  }
  destroy() {
    this.emit("destroy"), this.subscriptions.forEach((t2) => t2());
  }
}
function i(t2, e2, i2, n2, s2 = 5) {
  let r = () => {
  };
  if (!t2)
    return r;
  const o = (o2) => {
    if (2 === o2.button)
      return;
    o2.preventDefault(), o2.stopPropagation(), t2.style.touchAction = "none";
    let a = o2.clientX, h = o2.clientY, l = false;
    const d = (n3) => {
      n3.preventDefault(), n3.stopPropagation();
      const r2 = n3.clientX, o3 = n3.clientY;
      if (l || Math.abs(r2 - a) >= s2 || Math.abs(o3 - h) >= s2) {
        const { left: n4, top: s3 } = t2.getBoundingClientRect();
        l || (l = true, null == i2 || i2(a - n4, h - s3)), e2(r2 - a, o3 - h, r2 - n4, o3 - s3), a = r2, h = o3;
      }
    }, u = (t3) => {
      l && (t3.preventDefault(), t3.stopPropagation());
    }, c = () => {
      t2.style.touchAction = "", l && (null == n2 || n2()), r();
    };
    document.addEventListener("pointermove", d), document.addEventListener("pointerup", c), document.addEventListener("pointerleave", c), document.addEventListener("click", u, true), r = () => {
      document.removeEventListener("pointermove", d), document.removeEventListener("pointerup", c), document.removeEventListener("pointerleave", c), setTimeout(() => {
        document.removeEventListener("click", u, true);
      }, 10);
    };
  };
  return t2.addEventListener("pointerdown", o), () => {
    r(), t2.removeEventListener("pointerdown", o);
  };
}
class n extends t {
  constructor(t2, e2, i2 = 0) {
    var n2, s2, r, o, a, h, l;
    super(), this.totalDuration = e2, this.numberOfChannels = i2, this.minLength = 0, this.maxLength = 1 / 0, this.id = t2.id || `region-${Math.random().toString(32).slice(2)}`, this.start = this.clampPosition(t2.start), this.end = this.clampPosition(null !== (n2 = t2.end) && void 0 !== n2 ? n2 : t2.start), this.drag = null === (s2 = t2.drag) || void 0 === s2 || s2, this.resize = null === (r = t2.resize) || void 0 === r || r, this.color = null !== (o = t2.color) && void 0 !== o ? o : "rgba(0, 0, 0, 0.1)", this.minLength = null !== (a = t2.minLength) && void 0 !== a ? a : this.minLength, this.maxLength = null !== (h = t2.maxLength) && void 0 !== h ? h : this.maxLength, this.channelIdx = null !== (l = t2.channelIdx) && void 0 !== l ? l : -1, this.element = this.initElement(), this.setContent(t2.content), this.setPart(), this.renderPosition(), this.initMouseEvents();
  }
  clampPosition(t2) {
    return Math.max(0, Math.min(this.totalDuration, t2));
  }
  setPart() {
    const t2 = this.start === this.end;
    this.element.setAttribute("part", `${t2 ? "marker" : "region"} ${this.id}`);
  }
  addResizeHandles(t2) {
    const e2 = document.createElement("div");
    e2.setAttribute("data-resize", "left"), e2.setAttribute("style", "\n        position: absolute;\n        z-index: 2;\n        width: 6px;\n        height: 100%;\n        top: 0;\n        left: 0;\n        border-left: 2px solid rgba(0, 0, 0, 0.5);\n        border-radius: 2px 0 0 2px;\n        cursor: ew-resize;\n        word-break: keep-all;\n      "), e2.setAttribute("part", "region-handle region-handle-left");
    const n2 = e2.cloneNode();
    n2.setAttribute("data-resize", "right"), n2.style.left = "", n2.style.right = "0", n2.style.borderRight = n2.style.borderLeft, n2.style.borderLeft = "", n2.style.borderRadius = "0 2px 2px 0", n2.setAttribute("part", "region-handle region-handle-right"), t2.appendChild(e2), t2.appendChild(n2);
    i(e2, (t3) => this.onResize(t3, "start"), () => null, () => this.onEndResizing(), 1), i(n2, (t3) => this.onResize(t3, "end"), () => null, () => this.onEndResizing(), 1);
  }
  removeResizeHandles(t2) {
    const e2 = t2.querySelector('[data-resize="left"]'), i2 = t2.querySelector('[data-resize="right"]');
    e2 && t2.removeChild(e2), i2 && t2.removeChild(i2);
  }
  initElement() {
    const t2 = document.createElement("div"), e2 = this.start === this.end;
    let i2 = 0, n2 = 100;
    return this.channelIdx >= 0 && this.channelIdx < this.numberOfChannels && (n2 = 100 / this.numberOfChannels, i2 = n2 * this.channelIdx), t2.setAttribute("style", `
      position: absolute;
      top: ${i2}%;
      height: ${n2}%;
      background-color: ${e2 ? "none" : this.color};
      border-left: ${e2 ? "2px solid " + this.color : "none"};
      border-radius: 2px;
      box-sizing: border-box;
      transition: background-color 0.2s ease;
      cursor: ${this.drag ? "grab" : "default"};
      pointer-events: all;
    `), !e2 && this.resize && this.addResizeHandles(t2), t2;
  }
  renderPosition() {
    const t2 = this.start / this.totalDuration, e2 = (this.totalDuration - this.end) / this.totalDuration;
    this.element.style.left = 100 * t2 + "%", this.element.style.right = 100 * e2 + "%";
  }
  initMouseEvents() {
    const { element: t2 } = this;
    t2 && (t2.addEventListener("click", (t3) => this.emit("click", t3)), t2.addEventListener("mouseenter", (t3) => this.emit("over", t3)), t2.addEventListener("mouseleave", (t3) => this.emit("leave", t3)), t2.addEventListener("dblclick", (t3) => this.emit("dblclick", t3)), i(t2, (t3) => this.onMove(t3), () => this.onStartMoving(), () => this.onEndMoving()));
  }
  onStartMoving() {
    this.drag && (this.element.style.cursor = "grabbing");
  }
  onEndMoving() {
    this.drag && (this.element.style.cursor = "grab", this.emit("update-end"));
  }
  _onUpdate(t2, e2) {
    if (!this.element.parentElement)
      return;
    const i2 = t2 / this.element.parentElement.clientWidth * this.totalDuration, n2 = e2 && "start" !== e2 ? this.start : this.start + i2, s2 = e2 && "end" !== e2 ? this.end : this.end + i2, r = s2 - n2;
    n2 >= 0 && s2 <= this.totalDuration && n2 <= s2 && r >= this.minLength && r <= this.maxLength && (this.start = n2, this.end = s2, this.renderPosition(), this.emit("update"));
  }
  onMove(t2) {
    this.drag && this._onUpdate(t2);
  }
  onResize(t2, e2) {
    this.resize && this._onUpdate(t2, e2);
  }
  onEndResizing() {
    this.resize && this.emit("update-end");
  }
  _setTotalDuration(t2) {
    this.totalDuration = t2, this.renderPosition();
  }
  play() {
    this.emit("play");
  }
  setContent(t2) {
    var e2;
    if (null === (e2 = this.content) || void 0 === e2 || e2.remove(), t2) {
      if ("string" == typeof t2) {
        this.content = document.createElement("div");
        const e3 = this.start === this.end;
        this.content.style.padding = `0.2em ${e3 ? 0.2 : 0.4}em`, this.content.textContent = t2;
      } else
        this.content = t2;
      this.content.setAttribute("part", "region-content"), this.element.appendChild(this.content);
    } else
      this.content = void 0;
  }
  setOptions(t2) {
    var e2, i2;
    if (t2.color && (this.color = t2.color, this.element.style.backgroundColor = this.color), void 0 !== t2.drag && (this.drag = t2.drag, this.element.style.cursor = this.drag ? "grab" : "default"), void 0 !== t2.start || void 0 !== t2.end) {
      const n2 = this.start === this.end;
      this.start = this.clampPosition(null !== (e2 = t2.start) && void 0 !== e2 ? e2 : this.start), this.end = this.clampPosition(null !== (i2 = t2.end) && void 0 !== i2 ? i2 : n2 ? this.start : this.end), this.renderPosition(), this.setPart();
    }
    if (t2.content && this.setContent(t2.content), t2.id && (this.id = t2.id, this.setPart()), void 0 !== t2.resize && t2.resize !== this.resize) {
      const e3 = this.start === this.end;
      this.resize = t2.resize, this.resize && !e3 ? this.addResizeHandles(this.element) : this.removeResizeHandles(this.element);
    }
  }
  remove() {
    this.emit("remove"), this.element.remove(), this.element = null;
  }
}
class s extends e {
  constructor(t2) {
    super(t2), this.regions = [], this.regionsContainer = this.initRegionsContainer();
  }
  static create(t2) {
    return new s(t2);
  }
  onInit() {
    if (!this.wavesurfer)
      throw Error("WaveSurfer is not initialized");
    this.wavesurfer.getWrapper().appendChild(this.regionsContainer);
    let t2 = [];
    this.subscriptions.push(this.wavesurfer.on("timeupdate", (e2) => {
      const i2 = this.regions.filter((t3) => t3.start <= e2 && t3.end >= e2);
      i2.forEach((e3) => {
        t2.includes(e3) || this.emit("region-in", e3);
      }), t2.forEach((t3) => {
        i2.includes(t3) || this.emit("region-out", t3);
      }), t2 = i2;
    }));
  }
  initRegionsContainer() {
    const t2 = document.createElement("div");
    return t2.setAttribute("style", "\n      position: absolute;\n      top: 0;\n      left: 0;\n      width: 100%;\n      height: 100%;\n      z-index: 3;\n      pointer-events: none;\n    "), t2;
  }
  getRegions() {
    return this.regions;
  }
  avoidOverlapping(t2) {
    if (!t2.content)
      return;
    const e2 = t2.content, i2 = e2.getBoundingClientRect().left, n2 = t2.element.scrollWidth, s2 = this.regions.filter((e3) => {
      if (e3 === t2 || !e3.content)
        return false;
      const s3 = e3.content.getBoundingClientRect().left, r = e3.element.scrollWidth;
      return i2 < s3 + r && s3 < i2 + n2;
    }).map((t3) => {
      var e3;
      return (null === (e3 = t3.content) || void 0 === e3 ? void 0 : e3.getBoundingClientRect().height) || 0;
    }).reduce((t3, e3) => t3 + e3, 0);
    e2.style.marginTop = `${s2}px`;
  }
  saveRegion(t2) {
    this.regionsContainer.appendChild(t2.element), this.avoidOverlapping(t2), this.regions.push(t2);
    const e2 = [t2.on("update-end", () => {
      this.avoidOverlapping(t2), this.emit("region-updated", t2);
    }), t2.on("play", () => {
      var e3, i2;
      null === (e3 = this.wavesurfer) || void 0 === e3 || e3.play(), null === (i2 = this.wavesurfer) || void 0 === i2 || i2.setTime(t2.start);
    }), t2.on("click", (e3) => {
      this.emit("region-clicked", t2, e3);
    }), t2.on("dblclick", (e3) => {
      this.emit("region-double-clicked", t2, e3);
    }), t2.once("remove", () => {
      e2.forEach((t3) => t3()), this.regions = this.regions.filter((e3) => e3 !== t2);
    })];
    this.subscriptions.push(...e2), this.emit("region-created", t2);
  }
  addRegion(t2) {
    var e2, i2;
    if (!this.wavesurfer)
      throw Error("WaveSurfer is not initialized");
    const s2 = this.wavesurfer.getDuration(), r = null === (i2 = null === (e2 = this.wavesurfer) || void 0 === e2 ? void 0 : e2.getDecodedData()) || void 0 === i2 ? void 0 : i2.numberOfChannels, o = new n(t2, s2, r);
    return s2 ? this.saveRegion(o) : this.subscriptions.push(this.wavesurfer.once("ready", (t3) => {
      o._setTotalDuration(t3), this.saveRegion(o);
    })), o;
  }
  enableDragSelection(t2) {
    var e2, s2;
    const r = null === (s2 = null === (e2 = this.wavesurfer) || void 0 === e2 ? void 0 : e2.getWrapper()) || void 0 === s2 ? void 0 : s2.querySelector("div");
    if (!r)
      return () => {
      };
    let o = null, a = 0;
    return i(r, (t3, e3, i2) => {
      o && o._onUpdate(t3, i2 > a ? "end" : "start");
    }, (e3) => {
      var i2, s3;
      if (a = e3, !this.wavesurfer)
        return;
      const r2 = this.wavesurfer.getDuration(), h = null === (s3 = null === (i2 = this.wavesurfer) || void 0 === i2 ? void 0 : i2.getDecodedData()) || void 0 === s3 ? void 0 : s3.numberOfChannels, l = this.wavesurfer.getWrapper().clientWidth, d = e3 / l * r2, u = (e3 + 5) / l * r2;
      o = new n(Object.assign(Object.assign({}, t2), { start: d, end: u }), r2, h), this.regionsContainer.appendChild(o.element);
    }, () => {
      o && (this.saveRegion(o), o = null);
    });
  }
  clearRegions() {
    this.regions.forEach((t2) => t2.remove());
  }
  destroy() {
    this.clearRegions(), super.destroy();
  }
}
function create_if_block_2$3(ctx) {
  let volumehigh;
  let current;
  volumehigh = new VolumeHigh({});
  return {
    c() {
      create_component(volumehigh.$$.fragment);
    },
    l(nodes) {
      claim_component(volumehigh.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(volumehigh, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(volumehigh.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(volumehigh.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(volumehigh, detaching);
    }
  };
}
function create_if_block_1$3(ctx) {
  let volumelow;
  let current;
  volumelow = new VolumeLow({});
  return {
    c() {
      create_component(volumelow.$$.fragment);
    },
    l(nodes) {
      claim_component(volumelow.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(volumelow, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(volumelow.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(volumelow.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(volumelow, detaching);
    }
  };
}
function create_if_block$3(ctx) {
  let volumemuted;
  let current;
  volumemuted = new VolumeMuted({});
  return {
    c() {
      create_component(volumemuted.$$.fragment);
    },
    l(nodes) {
      claim_component(volumemuted.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(volumemuted, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(volumemuted.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(volumemuted.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(volumemuted, detaching);
    }
  };
}
function create_fragment$4(ctx) {
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  const if_block_creators = [create_if_block$3, create_if_block_1$3, create_if_block_2$3];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*currentVolume*/
      ctx2[0] == 0
    )
      return 0;
    if (
      /*currentVolume*/
      ctx2[0] < 0.5
    )
      return 1;
    if (
      /*currentVolume*/
      ctx2[0] >= 0.5
    )
      return 2;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
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
    p(ctx2, [dirty]) {
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index !== previous_block_index) {
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
function instance$4($$self, $$props, $$invalidate) {
  let { currentVolume } = $$props;
  $$self.$$set = ($$props2) => {
    if ("currentVolume" in $$props2)
      $$invalidate(0, currentVolume = $$props2.currentVolume);
  };
  return [currentVolume];
}
class VolumeLevels extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$4, create_fragment$4, safe_not_equal, { currentVolume: 0 });
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
      input = claim_element(nodes, "INPUT", {
        id: true,
        class: true,
        type: true,
        min: true,
        max: true,
        step: true
      });
      this.h();
    },
    h() {
      attr(input, "id", "volume");
      attr(input, "class", "volume-slider svelte-wuo8j5");
      attr(input, "type", "range");
      attr(input, "min", "0");
      attr(input, "max", "1");
      attr(input, "step", "0.01");
      input.value = /*currentVolume*/
      ctx[0];
    },
    m(target, anchor) {
      insert_hydration(target, input, anchor);
      ctx[4](input);
      if (!mounted) {
        dispose = [
          listen(
            input,
            "focusout",
            /*focusout_handler*/
            ctx[5]
          ),
          listen(
            input,
            "input",
            /*input_handler*/
            ctx[6]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (dirty & /*currentVolume*/
      1) {
        input.value = /*currentVolume*/
        ctx2[0];
      }
    },
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(input);
      }
      ctx[4](null);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$3($$self, $$props, $$invalidate) {
  let { currentVolume = 1 } = $$props;
  let { show_volume_slider = false } = $$props;
  let { waveform } = $$props;
  let volumeElement;
  onMount(() => {
    adjustSlider();
  });
  const adjustSlider = () => {
    let slider = volumeElement;
    if (!slider)
      return;
    slider.style.background = `linear-gradient(to right, var(--color-accent) ${currentVolume * 100}%, var(--neutral-400) ${currentVolume * 100}%)`;
  };
  function input_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      volumeElement = $$value;
      $$invalidate(3, volumeElement);
    });
  }
  const focusout_handler = () => $$invalidate(1, show_volume_slider = false);
  const input_handler = (e2) => {
    if (e2.target instanceof HTMLInputElement) {
      $$invalidate(0, currentVolume = parseFloat(e2.target.value));
      waveform == null ? void 0 : waveform.setVolume(currentVolume);
    }
  };
  $$self.$$set = ($$props2) => {
    if ("currentVolume" in $$props2)
      $$invalidate(0, currentVolume = $$props2.currentVolume);
    if ("show_volume_slider" in $$props2)
      $$invalidate(1, show_volume_slider = $$props2.show_volume_slider);
    if ("waveform" in $$props2)
      $$invalidate(2, waveform = $$props2.waveform);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*currentVolume*/
    1) {
      adjustSlider();
    }
  };
  return [
    currentVolume,
    show_volume_slider,
    waveform,
    volumeElement,
    input_binding,
    focusout_handler,
    input_handler
  ];
}
class VolumeControl extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance$3, create_fragment$3, safe_not_equal, {
      currentVolume: 0,
      show_volume_slider: 1,
      waveform: 2
    });
  }
}
function create_if_block_4(ctx) {
  let volumecontrol;
  let updating_currentVolume;
  let updating_show_volume_slider;
  let current;
  function volumecontrol_currentVolume_binding(value) {
    ctx[27](value);
  }
  function volumecontrol_show_volume_slider_binding(value) {
    ctx[28](value);
  }
  let volumecontrol_props = { waveform: (
    /*waveform*/
    ctx[2]
  ) };
  if (
    /*currentVolume*/
    ctx[12] !== void 0
  ) {
    volumecontrol_props.currentVolume = /*currentVolume*/
    ctx[12];
  }
  if (
    /*show_volume_slider*/
    ctx[1] !== void 0
  ) {
    volumecontrol_props.show_volume_slider = /*show_volume_slider*/
    ctx[1];
  }
  volumecontrol = new VolumeControl({ props: volumecontrol_props });
  binding_callbacks.push(() => bind(volumecontrol, "currentVolume", volumecontrol_currentVolume_binding));
  binding_callbacks.push(() => bind(volumecontrol, "show_volume_slider", volumecontrol_show_volume_slider_binding));
  return {
    c() {
      create_component(volumecontrol.$$.fragment);
    },
    l(nodes) {
      claim_component(volumecontrol.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(volumecontrol, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const volumecontrol_changes = {};
      if (dirty[0] & /*waveform*/
      4)
        volumecontrol_changes.waveform = /*waveform*/
        ctx2[2];
      if (!updating_currentVolume && dirty[0] & /*currentVolume*/
      4096) {
        updating_currentVolume = true;
        volumecontrol_changes.currentVolume = /*currentVolume*/
        ctx2[12];
        add_flush_callback(() => updating_currentVolume = false);
      }
      if (!updating_show_volume_slider && dirty[0] & /*show_volume_slider*/
      2) {
        updating_show_volume_slider = true;
        volumecontrol_changes.show_volume_slider = /*show_volume_slider*/
        ctx2[1];
        add_flush_callback(() => updating_show_volume_slider = false);
      }
      volumecontrol.$set(volumecontrol_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(volumecontrol.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(volumecontrol.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(volumecontrol, detaching);
    }
  };
}
function create_else_block_1(ctx) {
  let play;
  let current;
  play = new Play({});
  return {
    c() {
      create_component(play.$$.fragment);
    },
    l(nodes) {
      claim_component(play.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(play, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(play.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(play.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(play, detaching);
    }
  };
}
function create_if_block_3(ctx) {
  let pause;
  let current;
  pause = new Pause({});
  return {
    c() {
      create_component(pause.$$.fragment);
    },
    l(nodes) {
      claim_component(pause.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(pause, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(pause.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(pause.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(pause, detaching);
    }
  };
}
function create_if_block$2(ctx) {
  let t2;
  let current_block_type_index;
  let if_block1;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*show_redo*/
    ctx[6] && /*mode*/
    ctx[0] === "" && create_if_block_2$2(ctx)
  );
  const if_block_creators = [create_if_block_1$2, create_else_block$1];
  const if_blocks = [];
  function select_block_type_1(ctx2, dirty) {
    if (
      /*mode*/
      ctx2[0] === ""
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type_1(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t2 = space();
      if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t2 = claim_space(nodes);
      if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t2, anchor);
      if_blocks[current_block_type_index].m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_redo*/
        ctx2[6] && /*mode*/
        ctx2[0] === ""
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*show_redo, mode*/
          65) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2$2(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t2.parentNode, t2);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type_1(ctx2);
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
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function create_if_block_2$2(ctx) {
  let button;
  let undo;
  let current;
  let mounted;
  let dispose;
  undo = new Undo({});
  return {
    c() {
      button = element("button");
      create_component(undo.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(undo.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "action icon svelte-ije4bl");
      attr(button, "aria-label", "Reset audio");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(undo, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*click_handler_5*/
          ctx[33]
        );
        mounted = true;
      }
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(undo.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(undo.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(undo);
      mounted = false;
      dispose();
    }
  };
}
function create_else_block$1(ctx) {
  let button0;
  let textContent = "Trim";
  let t1;
  let button1;
  let textContent_1 = "Cancel";
  let mounted;
  let dispose;
  return {
    c() {
      button0 = element("button");
      button0.textContent = textContent;
      t1 = space();
      button1 = element("button");
      button1.textContent = textContent_1;
      this.h();
    },
    l(nodes) {
      button0 = claim_element(nodes, "BUTTON", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(button0) !== "svelte-1brf00d")
        button0.textContent = textContent;
      t1 = claim_space(nodes);
      button1 = claim_element(nodes, "BUTTON", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(button1) !== "svelte-1r0ma01")
        button1.textContent = textContent_1;
      this.h();
    },
    h() {
      attr(button0, "class", "text-button svelte-ije4bl");
      attr(button1, "class", "text-button svelte-ije4bl");
    },
    m(target, anchor) {
      insert_hydration(target, button0, anchor);
      insert_hydration(target, t1, anchor);
      insert_hydration(target, button1, anchor);
      if (!mounted) {
        dispose = [
          listen(
            button0,
            "click",
            /*trimAudio*/
            ctx[14]
          ),
          listen(
            button1,
            "click",
            /*toggleTrimmingMode*/
            ctx[16]
          )
        ];
        mounted = true;
      }
    },
    p: noop,
    i: noop,
    o: noop,
    d(detaching) {
      if (detaching) {
        detach(button0);
        detach(t1);
        detach(button1);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function create_if_block_1$2(ctx) {
  let button;
  let trim;
  let current;
  let mounted;
  let dispose;
  trim = new Trim({});
  return {
    c() {
      button = element("button");
      create_component(trim.$$.fragment);
      this.h();
    },
    l(nodes) {
      button = claim_element(nodes, "BUTTON", { class: true, "aria-label": true });
      var button_nodes = children(button);
      claim_component(trim.$$.fragment, button_nodes);
      button_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button, "class", "action icon svelte-ije4bl");
      attr(button, "aria-label", "Trim audio to selection");
    },
    m(target, anchor) {
      insert_hydration(target, button, anchor);
      mount_component(trim, button, null);
      current = true;
      if (!mounted) {
        dispose = listen(
          button,
          "click",
          /*toggleTrimmingMode*/
          ctx[16]
        );
        mounted = true;
      }
    },
    p: noop,
    i(local) {
      if (current)
        return;
      transition_in(trim.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(trim.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(button);
      }
      destroy_component(trim);
      mounted = false;
      dispose();
    }
  };
}
function create_fragment$2(ctx) {
  let div3;
  let div0;
  let button0;
  let volumelevels;
  let t0;
  let t1;
  let button1;
  let span;
  let t2;
  let t3;
  let button1_aria_label_value;
  let t4;
  let div1;
  let button2;
  let backward;
  let button2_aria_label_value;
  let t5;
  let button3;
  let current_block_type_index;
  let if_block1;
  let button3_aria_label_value;
  let t6;
  let button4;
  let forward;
  let button4_aria_label_value;
  let t7;
  let div2;
  let current;
  let mounted;
  let dispose;
  volumelevels = new VolumeLevels({
    props: { currentVolume: (
      /*currentVolume*/
      ctx[12]
    ) }
  });
  let if_block0 = (
    /*show_volume_slider*/
    ctx[1] && create_if_block_4(ctx)
  );
  backward = new Backward({});
  const if_block_creators = [create_if_block_3, create_else_block_1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*playing*/
      ctx2[5]
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  forward = new Forward({});
  let if_block2 = (
    /*editable*/
    ctx[10] && /*interactive*/
    ctx[7] && create_if_block$2(ctx)
  );
  return {
    c() {
      div3 = element("div");
      div0 = element("div");
      button0 = element("button");
      create_component(volumelevels.$$.fragment);
      t0 = space();
      if (if_block0)
        if_block0.c();
      t1 = space();
      button1 = element("button");
      span = element("span");
      t2 = text(
        /*playbackSpeed*/
        ctx[11]
      );
      t3 = text("x");
      t4 = space();
      div1 = element("div");
      button2 = element("button");
      create_component(backward.$$.fragment);
      t5 = space();
      button3 = element("button");
      if_block1.c();
      t6 = space();
      button4 = element("button");
      create_component(forward.$$.fragment);
      t7 = space();
      div2 = element("div");
      if (if_block2)
        if_block2.c();
      this.h();
    },
    l(nodes) {
      div3 = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div3_nodes = children(div3);
      div0 = claim_element(div3_nodes, "DIV", { class: true });
      var div0_nodes = children(div0);
      button0 = claim_element(div0_nodes, "BUTTON", { class: true, "aria-label": true });
      var button0_nodes = children(button0);
      claim_component(volumelevels.$$.fragment, button0_nodes);
      button0_nodes.forEach(detach);
      t0 = claim_space(div0_nodes);
      if (if_block0)
        if_block0.l(div0_nodes);
      t1 = claim_space(div0_nodes);
      button1 = claim_element(div0_nodes, "BUTTON", { class: true, "aria-label": true });
      var button1_nodes = children(button1);
      span = claim_element(button1_nodes, "SPAN", {});
      var span_nodes = children(span);
      t2 = claim_text(
        span_nodes,
        /*playbackSpeed*/
        ctx[11]
      );
      t3 = claim_text(span_nodes, "x");
      span_nodes.forEach(detach);
      button1_nodes.forEach(detach);
      div0_nodes.forEach(detach);
      t4 = claim_space(div3_nodes);
      div1 = claim_element(div3_nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      button2 = claim_element(div1_nodes, "BUTTON", { class: true, "aria-label": true });
      var button2_nodes = children(button2);
      claim_component(backward.$$.fragment, button2_nodes);
      button2_nodes.forEach(detach);
      t5 = claim_space(div1_nodes);
      button3 = claim_element(div1_nodes, "BUTTON", { class: true, "aria-label": true });
      var button3_nodes = children(button3);
      if_block1.l(button3_nodes);
      button3_nodes.forEach(detach);
      t6 = claim_space(div1_nodes);
      button4 = claim_element(div1_nodes, "BUTTON", { class: true, "aria-label": true });
      var button4_nodes = children(button4);
      claim_component(forward.$$.fragment, button4_nodes);
      button4_nodes.forEach(detach);
      div1_nodes.forEach(detach);
      t7 = claim_space(div3_nodes);
      div2 = claim_element(div3_nodes, "DIV", { class: true });
      var div2_nodes = children(div2);
      if (if_block2)
        if_block2.l(div2_nodes);
      div2_nodes.forEach(detach);
      div3_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(button0, "class", "action icon volume svelte-ije4bl");
      attr(button0, "aria-label", "Adjust volume");
      set_style(
        button0,
        "color",
        /*show_volume_slider*/
        ctx[1] ? "var(--color-accent)" : "var(--neutral-400)"
      );
      attr(button1, "class", "playback icon svelte-ije4bl");
      attr(button1, "aria-label", button1_aria_label_value = `Adjust playback speed to ${/*playbackSpeeds*/
      ctx[13][
        /*playbackSpeeds*/
        (ctx[13].indexOf(
          /*playbackSpeed*/
          ctx[11]
        ) + 1) % /*playbackSpeeds*/
        ctx[13].length
      ]}x`);
      toggle_class(
        button1,
        "hidden",
        /*show_volume_slider*/
        ctx[1]
      );
      attr(div0, "class", "control-wrapper svelte-ije4bl");
      attr(button2, "class", "rewind icon svelte-ije4bl");
      attr(button2, "aria-label", button2_aria_label_value = `Skip backwards by ${get_skip_rewind_amount(
        /*audio_duration*/
        ctx[3],
        /*waveform_options*/
        ctx[9].skip_length
      )} seconds`);
      attr(button3, "class", "play-pause-button icon svelte-ije4bl");
      attr(button3, "aria-label", button3_aria_label_value = /*playing*/
      ctx[5] ? (
        /*i18n*/
        ctx[4]("audio.pause")
      ) : (
        /*i18n*/
        ctx[4]("audio.play")
      ));
      attr(button4, "class", "skip icon svelte-ije4bl");
      attr(button4, "aria-label", button4_aria_label_value = "Skip forward by " + get_skip_rewind_amount(
        /*audio_duration*/
        ctx[3],
        /*waveform_options*/
        ctx[9].skip_length
      ) + " seconds");
      attr(div1, "class", "play-pause-wrapper svelte-ije4bl");
      attr(div2, "class", "settings-wrapper svelte-ije4bl");
      attr(div3, "class", "controls svelte-ije4bl");
      attr(div3, "data-testid", "waveform-controls");
    },
    m(target, anchor) {
      insert_hydration(target, div3, anchor);
      append_hydration(div3, div0);
      append_hydration(div0, button0);
      mount_component(volumelevels, button0, null);
      append_hydration(div0, t0);
      if (if_block0)
        if_block0.m(div0, null);
      append_hydration(div0, t1);
      append_hydration(div0, button1);
      append_hydration(button1, span);
      append_hydration(span, t2);
      append_hydration(span, t3);
      append_hydration(div3, t4);
      append_hydration(div3, div1);
      append_hydration(div1, button2);
      mount_component(backward, button2, null);
      append_hydration(div1, t5);
      append_hydration(div1, button3);
      if_blocks[current_block_type_index].m(button3, null);
      append_hydration(div1, t6);
      append_hydration(div1, button4);
      mount_component(forward, button4, null);
      append_hydration(div3, t7);
      append_hydration(div3, div2);
      if (if_block2)
        if_block2.m(div2, null);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            button0,
            "click",
            /*click_handler*/
            ctx[26]
          ),
          listen(
            button1,
            "click",
            /*click_handler_1*/
            ctx[29]
          ),
          listen(
            button2,
            "click",
            /*click_handler_2*/
            ctx[30]
          ),
          listen(
            button3,
            "click",
            /*click_handler_3*/
            ctx[31]
          ),
          listen(
            button4,
            "click",
            /*click_handler_4*/
            ctx[32]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      const volumelevels_changes = {};
      if (dirty[0] & /*currentVolume*/
      4096)
        volumelevels_changes.currentVolume = /*currentVolume*/
        ctx2[12];
      volumelevels.$set(volumelevels_changes);
      if (dirty[0] & /*show_volume_slider*/
      2) {
        set_style(
          button0,
          "color",
          /*show_volume_slider*/
          ctx2[1] ? "var(--color-accent)" : "var(--neutral-400)"
        );
      }
      if (
        /*show_volume_slider*/
        ctx2[1]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty[0] & /*show_volume_slider*/
          2) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_4(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(div0, t1);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (!current || dirty[0] & /*playbackSpeed*/
      2048)
        set_data(
          t2,
          /*playbackSpeed*/
          ctx2[11]
        );
      if (!current || dirty[0] & /*playbackSpeed*/
      2048 && button1_aria_label_value !== (button1_aria_label_value = `Adjust playback speed to ${/*playbackSpeeds*/
      ctx2[13][
        /*playbackSpeeds*/
        (ctx2[13].indexOf(
          /*playbackSpeed*/
          ctx2[11]
        ) + 1) % /*playbackSpeeds*/
        ctx2[13].length
      ]}x`)) {
        attr(button1, "aria-label", button1_aria_label_value);
      }
      if (!current || dirty[0] & /*show_volume_slider*/
      2) {
        toggle_class(
          button1,
          "hidden",
          /*show_volume_slider*/
          ctx2[1]
        );
      }
      if (!current || dirty[0] & /*audio_duration, waveform_options*/
      520 && button2_aria_label_value !== (button2_aria_label_value = `Skip backwards by ${get_skip_rewind_amount(
        /*audio_duration*/
        ctx2[3],
        /*waveform_options*/
        ctx2[9].skip_length
      )} seconds`)) {
        attr(button2, "aria-label", button2_aria_label_value);
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
      if (current_block_type_index !== previous_block_index) {
        group_outros();
        transition_out(if_blocks[previous_block_index], 1, 1, () => {
          if_blocks[previous_block_index] = null;
        });
        check_outros();
        if_block1 = if_blocks[current_block_type_index];
        if (!if_block1) {
          if_block1 = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx2);
          if_block1.c();
        }
        transition_in(if_block1, 1);
        if_block1.m(button3, null);
      }
      if (!current || dirty[0] & /*playing, i18n*/
      48 && button3_aria_label_value !== (button3_aria_label_value = /*playing*/
      ctx2[5] ? (
        /*i18n*/
        ctx2[4]("audio.pause")
      ) : (
        /*i18n*/
        ctx2[4]("audio.play")
      ))) {
        attr(button3, "aria-label", button3_aria_label_value);
      }
      if (!current || dirty[0] & /*audio_duration, waveform_options*/
      520 && button4_aria_label_value !== (button4_aria_label_value = "Skip forward by " + get_skip_rewind_amount(
        /*audio_duration*/
        ctx2[3],
        /*waveform_options*/
        ctx2[9].skip_length
      ) + " seconds")) {
        attr(button4, "aria-label", button4_aria_label_value);
      }
      if (
        /*editable*/
        ctx2[10] && /*interactive*/
        ctx2[7]
      ) {
        if (if_block2) {
          if_block2.p(ctx2, dirty);
          if (dirty[0] & /*editable, interactive*/
          1152) {
            transition_in(if_block2, 1);
          }
        } else {
          if_block2 = create_if_block$2(ctx2);
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
      transition_in(volumelevels.$$.fragment, local);
      transition_in(if_block0);
      transition_in(backward.$$.fragment, local);
      transition_in(if_block1);
      transition_in(forward.$$.fragment, local);
      transition_in(if_block2);
      current = true;
    },
    o(local) {
      transition_out(volumelevels.$$.fragment, local);
      transition_out(if_block0);
      transition_out(backward.$$.fragment, local);
      transition_out(if_block1);
      transition_out(forward.$$.fragment, local);
      transition_out(if_block2);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(div3);
      }
      destroy_component(volumelevels);
      if (if_block0)
        if_block0.d();
      destroy_component(backward);
      if_blocks[current_block_type_index].d();
      destroy_component(forward);
      if (if_block2)
        if_block2.d();
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$2($$self, $$props, $$invalidate) {
  let { waveform } = $$props;
  let { audio_duration } = $$props;
  let { i18n } = $$props;
  let { playing } = $$props;
  let { show_redo = false } = $$props;
  let { interactive = false } = $$props;
  let { handle_trim_audio } = $$props;
  let { mode = "" } = $$props;
  let { container } = $$props;
  let { handle_reset_value } = $$props;
  let { waveform_options = {} } = $$props;
  let { trim_region_settings = {} } = $$props;
  let { show_volume_slider = false } = $$props;
  let { editable = true } = $$props;
  let { trimDuration = 0 } = $$props;
  let playbackSpeeds = [0.5, 1, 1.5, 2];
  let playbackSpeed = playbackSpeeds[1];
  let trimRegion = null;
  let activeRegion = null;
  let leftRegionHandle;
  let rightRegionHandle;
  let activeHandle = "";
  let currentVolume = 1;
  const addTrimRegion = () => {
    if (!trimRegion)
      return;
    $$invalidate(22, activeRegion = trimRegion == null ? void 0 : trimRegion.addRegion({
      start: audio_duration / 4,
      end: audio_duration / 2,
      ...trim_region_settings
    }));
    $$invalidate(17, trimDuration = activeRegion.end - activeRegion.start);
  };
  const trimAudio = () => {
    if (waveform && trimRegion) {
      if (activeRegion) {
        const start = activeRegion.start;
        const end = activeRegion.end;
        handle_trim_audio(start, end);
        $$invalidate(0, mode = "");
        $$invalidate(22, activeRegion = null);
      }
    }
  };
  const clearRegions = () => {
    trimRegion == null ? void 0 : trimRegion.getRegions().forEach((region) => {
      region.remove();
    });
    trimRegion == null ? void 0 : trimRegion.clearRegions();
  };
  const toggleTrimmingMode = () => {
    clearRegions();
    if (mode === "edit") {
      $$invalidate(0, mode = "");
    } else {
      $$invalidate(0, mode = "edit");
      addTrimRegion();
    }
  };
  const adjustRegionHandles = (handle, key) => {
    let newStart;
    let newEnd;
    if (!activeRegion)
      return;
    if (handle === "left") {
      if (key === "ArrowLeft") {
        newStart = activeRegion.start - 0.05;
        newEnd = activeRegion.end;
      } else {
        newStart = activeRegion.start + 0.05;
        newEnd = activeRegion.end;
      }
    } else {
      if (key === "ArrowLeft") {
        newStart = activeRegion.start;
        newEnd = activeRegion.end - 0.05;
      } else {
        newStart = activeRegion.start;
        newEnd = activeRegion.end + 0.05;
      }
    }
    activeRegion.setOptions({ start: newStart, end: newEnd });
    $$invalidate(17, trimDuration = activeRegion.end - activeRegion.start);
  };
  const click_handler = () => $$invalidate(1, show_volume_slider = !show_volume_slider);
  function volumecontrol_currentVolume_binding(value) {
    currentVolume = value;
    $$invalidate(12, currentVolume);
  }
  function volumecontrol_show_volume_slider_binding(value) {
    show_volume_slider = value;
    $$invalidate(1, show_volume_slider);
  }
  const click_handler_1 = () => {
    $$invalidate(11, playbackSpeed = playbackSpeeds[(playbackSpeeds.indexOf(playbackSpeed) + 1) % playbackSpeeds.length]);
    waveform == null ? void 0 : waveform.setPlaybackRate(playbackSpeed);
  };
  const click_handler_2 = () => waveform == null ? void 0 : waveform.skip(get_skip_rewind_amount(audio_duration, waveform_options.skip_length) * -1);
  const click_handler_3 = () => waveform == null ? void 0 : waveform.playPause();
  const click_handler_4 = () => waveform == null ? void 0 : waveform.skip(get_skip_rewind_amount(audio_duration, waveform_options.skip_length));
  const click_handler_5 = () => {
    handle_reset_value();
    clearRegions();
    $$invalidate(0, mode = "");
  };
  $$self.$$set = ($$props2) => {
    if ("waveform" in $$props2)
      $$invalidate(2, waveform = $$props2.waveform);
    if ("audio_duration" in $$props2)
      $$invalidate(3, audio_duration = $$props2.audio_duration);
    if ("i18n" in $$props2)
      $$invalidate(4, i18n = $$props2.i18n);
    if ("playing" in $$props2)
      $$invalidate(5, playing = $$props2.playing);
    if ("show_redo" in $$props2)
      $$invalidate(6, show_redo = $$props2.show_redo);
    if ("interactive" in $$props2)
      $$invalidate(7, interactive = $$props2.interactive);
    if ("handle_trim_audio" in $$props2)
      $$invalidate(18, handle_trim_audio = $$props2.handle_trim_audio);
    if ("mode" in $$props2)
      $$invalidate(0, mode = $$props2.mode);
    if ("container" in $$props2)
      $$invalidate(19, container = $$props2.container);
    if ("handle_reset_value" in $$props2)
      $$invalidate(8, handle_reset_value = $$props2.handle_reset_value);
    if ("waveform_options" in $$props2)
      $$invalidate(9, waveform_options = $$props2.waveform_options);
    if ("trim_region_settings" in $$props2)
      $$invalidate(20, trim_region_settings = $$props2.trim_region_settings);
    if ("show_volume_slider" in $$props2)
      $$invalidate(1, show_volume_slider = $$props2.show_volume_slider);
    if ("editable" in $$props2)
      $$invalidate(10, editable = $$props2.editable);
    if ("trimDuration" in $$props2)
      $$invalidate(17, trimDuration = $$props2.trimDuration);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*container, waveform*/
    524292) {
      $$invalidate(21, trimRegion = container && waveform ? waveform.registerPlugin(s.create()) : null);
    }
    if ($$self.$$.dirty[0] & /*trimRegion*/
    2097152) {
      trimRegion == null ? void 0 : trimRegion.on("region-out", (region) => {
        region.play();
      });
    }
    if ($$self.$$.dirty[0] & /*trimRegion*/
    2097152) {
      trimRegion == null ? void 0 : trimRegion.on("region-updated", (region) => {
        $$invalidate(17, trimDuration = region.end - region.start);
      });
    }
    if ($$self.$$.dirty[0] & /*trimRegion*/
    2097152) {
      trimRegion == null ? void 0 : trimRegion.on("region-clicked", (region, e2) => {
        e2.stopPropagation();
        $$invalidate(22, activeRegion = region);
        region.play();
      });
    }
    if ($$self.$$.dirty[0] & /*activeRegion, container, leftRegionHandle, rightRegionHandle, trimRegion*/
    31981568) {
      if (activeRegion) {
        const shadowRoot = container.children[0].shadowRoot;
        $$invalidate(24, rightRegionHandle = shadowRoot.querySelector('[data-resize="right"]'));
        $$invalidate(23, leftRegionHandle = shadowRoot.querySelector('[data-resize="left"]'));
        if (leftRegionHandle && rightRegionHandle) {
          leftRegionHandle.setAttribute("role", "button");
          rightRegionHandle.setAttribute("role", "button");
          leftRegionHandle == null ? void 0 : leftRegionHandle.setAttribute("aria-label", "Drag to adjust start time");
          rightRegionHandle == null ? void 0 : rightRegionHandle.setAttribute("aria-label", "Drag to adjust end time");
          leftRegionHandle == null ? void 0 : leftRegionHandle.setAttribute("tabindex", "0");
          rightRegionHandle == null ? void 0 : rightRegionHandle.setAttribute("tabindex", "0");
          leftRegionHandle.addEventListener("focus", () => {
            if (trimRegion)
              $$invalidate(25, activeHandle = "left");
          });
          rightRegionHandle.addEventListener("focus", () => {
            if (trimRegion)
              $$invalidate(25, activeHandle = "right");
          });
        }
      }
    }
    if ($$self.$$.dirty[0] & /*trimRegion, activeHandle*/
    35651584) {
      trimRegion && window.addEventListener("keydown", (e2) => {
        if (e2.key === "ArrowLeft") {
          adjustRegionHandles(activeHandle, "ArrowLeft");
        } else if (e2.key === "ArrowRight") {
          adjustRegionHandles(activeHandle, "ArrowRight");
        }
      });
    }
  };
  return [
    mode,
    show_volume_slider,
    waveform,
    audio_duration,
    i18n,
    playing,
    show_redo,
    interactive,
    handle_reset_value,
    waveform_options,
    editable,
    playbackSpeed,
    currentVolume,
    playbackSpeeds,
    trimAudio,
    clearRegions,
    toggleTrimmingMode,
    trimDuration,
    handle_trim_audio,
    container,
    trim_region_settings,
    trimRegion,
    activeRegion,
    leftRegionHandle,
    rightRegionHandle,
    activeHandle,
    click_handler,
    volumecontrol_currentVolume_binding,
    volumecontrol_show_volume_slider_binding,
    click_handler_1,
    click_handler_2,
    click_handler_3,
    click_handler_4,
    click_handler_5
  ];
}
class WaveformControls extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$2,
      create_fragment$2,
      safe_not_equal,
      {
        waveform: 2,
        audio_duration: 3,
        i18n: 4,
        playing: 5,
        show_redo: 6,
        interactive: 7,
        handle_trim_audio: 18,
        mode: 0,
        container: 19,
        handle_reset_value: 8,
        waveform_options: 9,
        trim_region_settings: 20,
        show_volume_slider: 1,
        editable: 10,
        trimDuration: 17
      },
      null,
      [-1, -1]
    );
  }
}
function create_if_block_1$1(ctx) {
  let div4;
  let div1;
  let div0;
  let t0;
  let div3;
  let time0;
  let textContent = "0:00";
  let t2;
  let div2;
  let t3;
  let time1;
  let textContent_1 = "0:00";
  let t5;
  let waveformcontrols;
  let updating_mode;
  let updating_trimDuration;
  let updating_show_volume_slider;
  let div4_data_testid_value;
  let current;
  let if_block = (
    /*mode*/
    ctx[0] === "edit" && /*trimDuration*/
    ctx[16] > 0 && create_if_block_2$1(ctx)
  );
  function waveformcontrols_mode_binding(value) {
    ctx[31](value);
  }
  function waveformcontrols_trimDuration_binding(value) {
    ctx[32](value);
  }
  function waveformcontrols_show_volume_slider_binding(value) {
    ctx[33](value);
  }
  let waveformcontrols_props = {
    container: (
      /*container*/
      ctx[10]
    ),
    waveform: (
      /*waveform*/
      ctx[11]
    ),
    playing: (
      /*playing*/
      ctx[14]
    ),
    audio_duration: (
      /*audio_duration*/
      ctx[15]
    ),
    i18n: (
      /*i18n*/
      ctx[3]
    ),
    interactive: (
      /*interactive*/
      ctx[4]
    ),
    handle_trim_audio: (
      /*handle_trim_audio*/
      ctx[20]
    ),
    show_redo: (
      /*interactive*/
      ctx[4]
    ),
    handle_reset_value: (
      /*handle_reset_value*/
      ctx[9]
    ),
    waveform_options: (
      /*waveform_options*/
      ctx[8]
    ),
    trim_region_settings: (
      /*trim_region_settings*/
      ctx[6]
    ),
    editable: (
      /*editable*/
      ctx[5]
    )
  };
  if (
    /*mode*/
    ctx[0] !== void 0
  ) {
    waveformcontrols_props.mode = /*mode*/
    ctx[0];
  }
  if (
    /*trimDuration*/
    ctx[16] !== void 0
  ) {
    waveformcontrols_props.trimDuration = /*trimDuration*/
    ctx[16];
  }
  if (
    /*show_volume_slider*/
    ctx[17] !== void 0
  ) {
    waveformcontrols_props.show_volume_slider = /*show_volume_slider*/
    ctx[17];
  }
  waveformcontrols = new WaveformControls({ props: waveformcontrols_props });
  binding_callbacks.push(() => bind(waveformcontrols, "mode", waveformcontrols_mode_binding));
  binding_callbacks.push(() => bind(waveformcontrols, "trimDuration", waveformcontrols_trimDuration_binding));
  binding_callbacks.push(() => bind(waveformcontrols, "show_volume_slider", waveformcontrols_show_volume_slider_binding));
  return {
    c() {
      div4 = element("div");
      div1 = element("div");
      div0 = element("div");
      t0 = space();
      div3 = element("div");
      time0 = element("time");
      time0.textContent = textContent;
      t2 = space();
      div2 = element("div");
      if (if_block)
        if_block.c();
      t3 = space();
      time1 = element("time");
      time1.textContent = textContent_1;
      t5 = space();
      create_component(waveformcontrols.$$.fragment);
      this.h();
    },
    l(nodes) {
      div4 = claim_element(nodes, "DIV", { class: true, "data-testid": true });
      var div4_nodes = children(div4);
      div1 = claim_element(div4_nodes, "DIV", { class: true });
      var div1_nodes = children(div1);
      div0 = claim_element(div1_nodes, "DIV", { id: true, class: true });
      children(div0).forEach(detach);
      div1_nodes.forEach(detach);
      t0 = claim_space(div4_nodes);
      div3 = claim_element(div4_nodes, "DIV", { class: true });
      var div3_nodes = children(div3);
      time0 = claim_element(div3_nodes, "TIME", {
        id: true,
        class: true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(time0) !== "svelte-lp3mlp")
        time0.textContent = textContent;
      t2 = claim_space(div3_nodes);
      div2 = claim_element(div3_nodes, "DIV", {});
      var div2_nodes = children(div2);
      if (if_block)
        if_block.l(div2_nodes);
      t3 = claim_space(div2_nodes);
      time1 = claim_element(div2_nodes, "TIME", {
        id: true,
        class: true,
        ["data-svelte-h"]: true
      });
      if (get_svelte_dataset(time1) !== "svelte-1jd0owv")
        time1.textContent = textContent_1;
      div2_nodes.forEach(detach);
      div3_nodes.forEach(detach);
      t5 = claim_space(div4_nodes);
      claim_component(waveformcontrols.$$.fragment, div4_nodes);
      div4_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div0, "id", "waveform");
      attr(div0, "class", "svelte-19usgod");
      set_style(
        div0,
        "height",
        /*container*/
        ctx[10] ? null : "58px"
      );
      attr(div1, "class", "waveform-container svelte-19usgod");
      attr(time0, "id", "time");
      attr(time0, "class", "svelte-19usgod");
      attr(time1, "id", "duration");
      attr(time1, "class", "svelte-19usgod");
      attr(div3, "class", "timestamps svelte-19usgod");
      attr(div4, "class", "component-wrapper svelte-19usgod");
      attr(div4, "data-testid", div4_data_testid_value = /*label*/
      ctx[2] ? "waveform-" + /*label*/
      ctx[2] : "unlabelled-audio");
    },
    m(target, anchor) {
      insert_hydration(target, div4, anchor);
      append_hydration(div4, div1);
      append_hydration(div1, div0);
      ctx[28](div0);
      append_hydration(div4, t0);
      append_hydration(div4, div3);
      append_hydration(div3, time0);
      ctx[29](time0);
      append_hydration(div3, t2);
      append_hydration(div3, div2);
      if (if_block)
        if_block.m(div2, null);
      append_hydration(div2, t3);
      append_hydration(div2, time1);
      ctx[30](time1);
      append_hydration(div4, t5);
      mount_component(waveformcontrols, div4, null);
      current = true;
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*container*/
      1024) {
        set_style(
          div0,
          "height",
          /*container*/
          ctx2[10] ? null : "58px"
        );
      }
      if (
        /*mode*/
        ctx2[0] === "edit" && /*trimDuration*/
        ctx2[16] > 0
      ) {
        if (if_block) {
          if_block.p(ctx2, dirty);
        } else {
          if_block = create_if_block_2$1(ctx2);
          if_block.c();
          if_block.m(div2, t3);
        }
      } else if (if_block) {
        if_block.d(1);
        if_block = null;
      }
      const waveformcontrols_changes = {};
      if (dirty[0] & /*container*/
      1024)
        waveformcontrols_changes.container = /*container*/
        ctx2[10];
      if (dirty[0] & /*waveform*/
      2048)
        waveformcontrols_changes.waveform = /*waveform*/
        ctx2[11];
      if (dirty[0] & /*playing*/
      16384)
        waveformcontrols_changes.playing = /*playing*/
        ctx2[14];
      if (dirty[0] & /*audio_duration*/
      32768)
        waveformcontrols_changes.audio_duration = /*audio_duration*/
        ctx2[15];
      if (dirty[0] & /*i18n*/
      8)
        waveformcontrols_changes.i18n = /*i18n*/
        ctx2[3];
      if (dirty[0] & /*interactive*/
      16)
        waveformcontrols_changes.interactive = /*interactive*/
        ctx2[4];
      if (dirty[0] & /*interactive*/
      16)
        waveformcontrols_changes.show_redo = /*interactive*/
        ctx2[4];
      if (dirty[0] & /*handle_reset_value*/
      512)
        waveformcontrols_changes.handle_reset_value = /*handle_reset_value*/
        ctx2[9];
      if (dirty[0] & /*waveform_options*/
      256)
        waveformcontrols_changes.waveform_options = /*waveform_options*/
        ctx2[8];
      if (dirty[0] & /*trim_region_settings*/
      64)
        waveformcontrols_changes.trim_region_settings = /*trim_region_settings*/
        ctx2[6];
      if (dirty[0] & /*editable*/
      32)
        waveformcontrols_changes.editable = /*editable*/
        ctx2[5];
      if (!updating_mode && dirty[0] & /*mode*/
      1) {
        updating_mode = true;
        waveformcontrols_changes.mode = /*mode*/
        ctx2[0];
        add_flush_callback(() => updating_mode = false);
      }
      if (!updating_trimDuration && dirty[0] & /*trimDuration*/
      65536) {
        updating_trimDuration = true;
        waveformcontrols_changes.trimDuration = /*trimDuration*/
        ctx2[16];
        add_flush_callback(() => updating_trimDuration = false);
      }
      if (!updating_show_volume_slider && dirty[0] & /*show_volume_slider*/
      131072) {
        updating_show_volume_slider = true;
        waveformcontrols_changes.show_volume_slider = /*show_volume_slider*/
        ctx2[17];
        add_flush_callback(() => updating_show_volume_slider = false);
      }
      waveformcontrols.$set(waveformcontrols_changes);
      if (!current || dirty[0] & /*label*/
      4 && div4_data_testid_value !== (div4_data_testid_value = /*label*/
      ctx2[2] ? "waveform-" + /*label*/
      ctx2[2] : "unlabelled-audio")) {
        attr(div4, "data-testid", div4_data_testid_value);
      }
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
      if (detaching) {
        detach(div4);
      }
      ctx[28](null);
      ctx[29](null);
      if (if_block)
        if_block.d();
      ctx[30](null);
      destroy_component(waveformcontrols);
    }
  };
}
function create_if_block$1(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      size: "small",
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
      if (dirty[1] & /*$$scope*/
      128) {
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
function create_if_block_2$1(ctx) {
  let time;
  let t_value = format_time(
    /*trimDuration*/
    ctx[16]
  ) + "";
  let t2;
  return {
    c() {
      time = element("time");
      t2 = text(t_value);
      this.h();
    },
    l(nodes) {
      time = claim_element(nodes, "TIME", { id: true, class: true });
      var time_nodes = children(time);
      t2 = claim_text(time_nodes, t_value);
      time_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(time, "id", "trim-duration");
      attr(time, "class", "svelte-19usgod");
    },
    m(target, anchor) {
      insert_hydration(target, time, anchor);
      append_hydration(time, t2);
    },
    p(ctx2, dirty) {
      if (dirty[0] & /*trimDuration*/
      65536 && t_value !== (t_value = format_time(
        /*trimDuration*/
        ctx2[16]
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
function create_default_slot$1(ctx) {
  let music;
  let current;
  music = new Music({});
  return {
    c() {
      create_component(music.$$.fragment);
    },
    l(nodes) {
      claim_component(music.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(music, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(music.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(music.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(music, detaching);
    }
  };
}
function create_fragment$1(ctx) {
  let audio;
  let audio_autoplay_value;
  let t2;
  let current_block_type_index;
  let if_block;
  let if_block_anchor;
  let current;
  let mounted;
  let dispose;
  const if_block_creators = [create_if_block$1, create_if_block_1$1];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[1] === null
    )
      return 0;
    if (!/*value*/
    ctx2[1].is_stream)
      return 1;
    return -1;
  }
  if (~(current_block_type_index = select_block_type(ctx))) {
    if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  }
  return {
    c() {
      audio = element("audio");
      t2 = space();
      if (if_block)
        if_block.c();
      if_block_anchor = empty();
      this.h();
    },
    l(nodes) {
      audio = claim_element(nodes, "AUDIO", { class: true });
      children(audio).forEach(detach);
      t2 = claim_space(nodes);
      if (if_block)
        if_block.l(nodes);
      if_block_anchor = empty();
      this.h();
    },
    h() {
      attr(audio, "class", "standard-player svelte-19usgod");
      audio.controls = true;
      audio.autoplay = audio_autoplay_value = /*waveform_settings*/
      ctx[7].autoplay;
      toggle_class(audio, "hidden", !/*value*/
      (ctx[1] && /*value*/
      ctx[1].is_stream));
    },
    m(target, anchor) {
      insert_hydration(target, audio, anchor);
      ctx[25](audio);
      insert_hydration(target, t2, anchor);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].m(target, anchor);
      }
      insert_hydration(target, if_block_anchor, anchor);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            audio,
            "load",
            /*load_handler*/
            ctx[24]
          ),
          listen(
            audio,
            "ended",
            /*ended_handler*/
            ctx[26]
          ),
          listen(
            audio,
            "play",
            /*play_handler*/
            ctx[27]
          )
        ];
        mounted = true;
      }
    },
    p(ctx2, dirty) {
      if (!current || dirty[0] & /*waveform_settings*/
      128 && audio_autoplay_value !== (audio_autoplay_value = /*waveform_settings*/
      ctx2[7].autoplay)) {
        audio.autoplay = audio_autoplay_value;
      }
      if (!current || dirty[0] & /*value*/
      2) {
        toggle_class(audio, "hidden", !/*value*/
        (ctx2[1] && /*value*/
        ctx2[1].is_stream));
      }
      let previous_block_index = current_block_type_index;
      current_block_type_index = select_block_type(ctx2);
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
        detach(audio);
        detach(t2);
        detach(if_block_anchor);
      }
      ctx[25](null);
      if (~current_block_type_index) {
        if_blocks[current_block_type_index].d(detaching);
      }
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance$1($$self, $$props, $$invalidate) {
  let url;
  let { value = null } = $$props;
  let { label } = $$props;
  let { i18n } = $$props;
  let { dispatch_blob = () => Promise.resolve() } = $$props;
  let { interactive = false } = $$props;
  let { editable = true } = $$props;
  let { trim_region_settings = {} } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options } = $$props;
  let { mode = "" } = $$props;
  let { loop } = $$props;
  let { handle_reset_value = () => {
  } } = $$props;
  let container;
  let waveform;
  let playing = false;
  let timeRef;
  let durationRef;
  let audio_duration;
  let trimDuration = 0;
  let show_volume_slider = false;
  let audio_player;
  let stream_active = false;
  const dispatch = createEventDispatcher();
  const create_waveform = () => {
    $$invalidate(11, waveform = WaveSurfer.create({ container, ...waveform_settings }));
    resolve_wasm_src(value == null ? void 0 : value.url).then((resolved_src) => {
      if (resolved_src && waveform) {
        return waveform.load(resolved_src);
      }
    });
  };
  const handle_trim_audio = async (start, end) => {
    $$invalidate(0, mode = "");
    const decodedData = waveform == null ? void 0 : waveform.getDecodedData();
    if (decodedData)
      await process_audio(decodedData, start, end, waveform_settings.sampleRate).then(async (trimmedBlob) => {
        await dispatch_blob([trimmedBlob], "change");
        waveform == null ? void 0 : waveform.destroy();
        $$invalidate(10, container.innerHTML = "", container);
      });
    dispatch("edit");
  };
  async function load_audio(data) {
    stream_active = false;
    await resolve_wasm_src(data).then((resolved_src) => {
      if (!resolved_src || (value == null ? void 0 : value.is_stream))
        return;
      return waveform == null ? void 0 : waveform.load(resolved_src);
    });
  }
  function load_stream(value2) {
    if (!value2 || !value2.is_stream || !value2.url)
      return;
    if (!audio_player)
      return;
    if (Hls.isSupported() && !stream_active) {
      const hls = new Hls({
        maxBufferLength: 1,
        maxMaxBufferLength: 1,
        lowLatencyMode: true
      });
      hls.loadSource(value2.url);
      hls.attachMedia(audio_player);
      hls.on(Hls.Events.MANIFEST_PARSED, function() {
        if (waveform_settings.autoplay)
          audio_player.play();
      });
      hls.on(Hls.Events.ERROR, function(event, data) {
        console.error("HLS error:", event, data);
        if (data.fatal) {
          switch (data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              console.error("Fatal network error encountered, trying to recover");
              hls.startLoad();
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              console.error("Fatal media error encountered, trying to recover");
              hls.recoverMediaError();
              break;
            default:
              console.error("Fatal error, cannot recover");
              hls.destroy();
              break;
          }
        }
      });
      stream_active = true;
    } else if (!stream_active) {
      $$invalidate(18, audio_player.src = value2.url, audio_player);
      if (waveform_settings.autoplay)
        audio_player.play();
      stream_active = true;
    }
  }
  onMount(() => {
    window.addEventListener("keydown", (e2) => {
      if (!waveform || show_volume_slider)
        return;
      if (e2.key === "ArrowRight" && mode !== "edit") {
        skip_audio(waveform, 0.1);
      } else if (e2.key === "ArrowLeft" && mode !== "edit") {
        skip_audio(waveform, -0.1);
      }
    });
  });
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  function audio_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      audio_player = $$value;
      $$invalidate(18, audio_player);
    });
  }
  const ended_handler = () => dispatch("stop");
  const play_handler = () => dispatch("play");
  function div0_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      container = $$value;
      $$invalidate(10, container), $$invalidate(1, value), $$invalidate(11, waveform);
    });
  }
  function time0_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      timeRef = $$value;
      $$invalidate(12, timeRef), $$invalidate(11, waveform);
    });
  }
  function time1_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      durationRef = $$value;
      $$invalidate(13, durationRef), $$invalidate(11, waveform);
    });
  }
  function waveformcontrols_mode_binding(value2) {
    mode = value2;
    $$invalidate(0, mode);
  }
  function waveformcontrols_trimDuration_binding(value2) {
    trimDuration = value2;
    $$invalidate(16, trimDuration);
  }
  function waveformcontrols_show_volume_slider_binding(value2) {
    show_volume_slider = value2;
    $$invalidate(17, show_volume_slider);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(1, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(2, label = $$props2.label);
    if ("i18n" in $$props2)
      $$invalidate(3, i18n = $$props2.i18n);
    if ("dispatch_blob" in $$props2)
      $$invalidate(21, dispatch_blob = $$props2.dispatch_blob);
    if ("interactive" in $$props2)
      $$invalidate(4, interactive = $$props2.interactive);
    if ("editable" in $$props2)
      $$invalidate(5, editable = $$props2.editable);
    if ("trim_region_settings" in $$props2)
      $$invalidate(6, trim_region_settings = $$props2.trim_region_settings);
    if ("waveform_settings" in $$props2)
      $$invalidate(7, waveform_settings = $$props2.waveform_settings);
    if ("waveform_options" in $$props2)
      $$invalidate(8, waveform_options = $$props2.waveform_options);
    if ("mode" in $$props2)
      $$invalidate(0, mode = $$props2.mode);
    if ("loop" in $$props2)
      $$invalidate(22, loop = $$props2.loop);
    if ("handle_reset_value" in $$props2)
      $$invalidate(9, handle_reset_value = $$props2.handle_reset_value);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty[0] & /*value*/
    2) {
      $$invalidate(23, url = value == null ? void 0 : value.url);
    }
    if ($$self.$$.dirty[0] & /*value, container, waveform*/
    3074) {
      if (!(value == null ? void 0 : value.is_stream) && container !== void 0 && container !== null) {
        if (waveform !== void 0)
          waveform.destroy();
        $$invalidate(10, container.innerHTML = "", container);
        create_waveform();
        $$invalidate(14, playing = false);
      }
    }
    if ($$self.$$.dirty[0] & /*waveform, durationRef*/
    10240) {
      waveform == null ? void 0 : waveform.on("decode", (duration) => {
        $$invalidate(15, audio_duration = duration);
        durationRef && $$invalidate(13, durationRef.textContent = format_time(duration), durationRef);
      });
    }
    if ($$self.$$.dirty[0] & /*waveform, timeRef*/
    6144) {
      waveform == null ? void 0 : waveform.on("timeupdate", (currentTime) => timeRef && $$invalidate(12, timeRef.textContent = format_time(currentTime), timeRef));
    }
    if ($$self.$$.dirty[0] & /*waveform, waveform_settings*/
    2176) {
      waveform == null ? void 0 : waveform.on("ready", () => {
        if (!waveform_settings.autoplay) {
          waveform == null ? void 0 : waveform.stop();
        } else {
          waveform == null ? void 0 : waveform.play();
        }
      });
    }
    if ($$self.$$.dirty[0] & /*waveform, loop*/
    4196352) {
      waveform == null ? void 0 : waveform.on("finish", () => {
        if (loop) {
          waveform == null ? void 0 : waveform.play();
        } else {
          $$invalidate(14, playing = false);
          dispatch("stop");
        }
      });
    }
    if ($$self.$$.dirty[0] & /*waveform*/
    2048) {
      waveform == null ? void 0 : waveform.on("pause", () => {
        $$invalidate(14, playing = false);
        dispatch("pause");
      });
    }
    if ($$self.$$.dirty[0] & /*waveform*/
    2048) {
      waveform == null ? void 0 : waveform.on("play", () => {
        $$invalidate(14, playing = true);
        dispatch("play");
      });
    }
    if ($$self.$$.dirty[0] & /*waveform*/
    2048) {
      waveform == null ? void 0 : waveform.on("load", () => {
        dispatch("load");
      });
    }
    if ($$self.$$.dirty[0] & /*url*/
    8388608) {
      url && load_audio(url);
    }
    if ($$self.$$.dirty[0] & /*value*/
    2) {
      load_stream(value);
    }
  };
  return [
    mode,
    value,
    label,
    i18n,
    interactive,
    editable,
    trim_region_settings,
    waveform_settings,
    waveform_options,
    handle_reset_value,
    container,
    waveform,
    timeRef,
    durationRef,
    playing,
    audio_duration,
    trimDuration,
    show_volume_slider,
    audio_player,
    dispatch,
    handle_trim_audio,
    dispatch_blob,
    loop,
    url,
    load_handler,
    audio_binding,
    ended_handler,
    play_handler,
    div0_binding,
    time0_binding,
    time1_binding,
    waveformcontrols_mode_binding,
    waveformcontrols_trimDuration_binding,
    waveformcontrols_show_volume_slider_binding
  ];
}
class AudioPlayer extends SvelteComponent {
  constructor(options) {
    super();
    init(
      this,
      options,
      instance$1,
      create_fragment$1,
      safe_not_equal,
      {
        value: 1,
        label: 2,
        i18n: 3,
        dispatch_blob: 21,
        interactive: 4,
        editable: 5,
        trim_region_settings: 6,
        waveform_settings: 7,
        waveform_options: 8,
        mode: 0,
        loop: 22,
        handle_reset_value: 9
      },
      null,
      [-1, -1]
    );
  }
}
const AudioPlayer$1 = AudioPlayer;
function create_else_block(ctx) {
  let empty_1;
  let current;
  empty_1 = new Empty({
    props: {
      size: "small",
      $$slots: { default: [create_default_slot_2] },
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
      262144) {
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
  let iconbuttonwrapper;
  let t2;
  let audioplayer;
  let current;
  iconbuttonwrapper = new IconButtonWrapper({
    props: {
      $$slots: { default: [create_default_slot] },
      $$scope: { ctx }
    }
  });
  audioplayer = new AudioPlayer$1({
    props: {
      value: (
        /*value*/
        ctx[0]
      ),
      label: (
        /*label*/
        ctx[1]
      ),
      i18n: (
        /*i18n*/
        ctx[5]
      ),
      waveform_settings: (
        /*waveform_settings*/
        ctx[6]
      ),
      waveform_options: (
        /*waveform_options*/
        ctx[7]
      ),
      editable: (
        /*editable*/
        ctx[8]
      ),
      loop: (
        /*loop*/
        ctx[9]
      )
    }
  });
  audioplayer.$on(
    "pause",
    /*pause_handler*/
    ctx[13]
  );
  audioplayer.$on(
    "play",
    /*play_handler*/
    ctx[14]
  );
  audioplayer.$on(
    "stop",
    /*stop_handler*/
    ctx[15]
  );
  audioplayer.$on(
    "load",
    /*load_handler*/
    ctx[16]
  );
  return {
    c() {
      create_component(iconbuttonwrapper.$$.fragment);
      t2 = space();
      create_component(audioplayer.$$.fragment);
    },
    l(nodes) {
      claim_component(iconbuttonwrapper.$$.fragment, nodes);
      t2 = claim_space(nodes);
      claim_component(audioplayer.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(iconbuttonwrapper, target, anchor);
      insert_hydration(target, t2, anchor);
      mount_component(audioplayer, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const iconbuttonwrapper_changes = {};
      if (dirty & /*$$scope, i18n, value, show_share_button, show_download_button*/
      262201) {
        iconbuttonwrapper_changes.$$scope = { dirty, ctx: ctx2 };
      }
      iconbuttonwrapper.$set(iconbuttonwrapper_changes);
      const audioplayer_changes = {};
      if (dirty & /*value*/
      1)
        audioplayer_changes.value = /*value*/
        ctx2[0];
      if (dirty & /*label*/
      2)
        audioplayer_changes.label = /*label*/
        ctx2[1];
      if (dirty & /*i18n*/
      32)
        audioplayer_changes.i18n = /*i18n*/
        ctx2[5];
      if (dirty & /*waveform_settings*/
      64)
        audioplayer_changes.waveform_settings = /*waveform_settings*/
        ctx2[6];
      if (dirty & /*waveform_options*/
      128)
        audioplayer_changes.waveform_options = /*waveform_options*/
        ctx2[7];
      if (dirty & /*editable*/
      256)
        audioplayer_changes.editable = /*editable*/
        ctx2[8];
      if (dirty & /*loop*/
      512)
        audioplayer_changes.loop = /*loop*/
        ctx2[9];
      audioplayer.$set(audioplayer_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(iconbuttonwrapper.$$.fragment, local);
      transition_in(audioplayer.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(iconbuttonwrapper.$$.fragment, local);
      transition_out(audioplayer.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
      }
      destroy_component(iconbuttonwrapper, detaching);
      destroy_component(audioplayer, detaching);
    }
  };
}
function create_default_slot_2(ctx) {
  let music;
  let current;
  music = new Music({});
  return {
    c() {
      create_component(music.$$.fragment);
    },
    l(nodes) {
      claim_component(music.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(music, target, anchor);
      current = true;
    },
    i(local) {
      if (current)
        return;
      transition_in(music.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(music.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(music, detaching);
    }
  };
}
function create_if_block_2(ctx) {
  var _a;
  let downloadlink;
  let current;
  downloadlink = new DownloadLink({
    props: {
      href: (
        /*value*/
        ctx[0].is_stream ? (
          /*value*/
          (_a = ctx[0].url) == null ? void 0 : _a.replace("playlist.m3u8", "playlist-file")
        ) : (
          /*value*/
          ctx[0].url
        )
      ),
      download: (
        /*value*/
        ctx[0].orig_name || /*value*/
        ctx[0].path
      ),
      $$slots: { default: [create_default_slot_1] },
      $$scope: { ctx }
    }
  });
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
    p(ctx2, dirty) {
      var _a2;
      const downloadlink_changes = {};
      if (dirty & /*value*/
      1)
        downloadlink_changes.href = /*value*/
        ctx2[0].is_stream ? (
          /*value*/
          (_a2 = ctx2[0].url) == null ? void 0 : _a2.replace("playlist.m3u8", "playlist-file")
        ) : (
          /*value*/
          ctx2[0].url
        );
      if (dirty & /*value*/
      1)
        downloadlink_changes.download = /*value*/
        ctx2[0].orig_name || /*value*/
        ctx2[0].path;
      if (dirty & /*$$scope, i18n*/
      262176) {
        downloadlink_changes.$$scope = { dirty, ctx: ctx2 };
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
function create_default_slot_1(ctx) {
  let iconbutton;
  let current;
  iconbutton = new IconButton({
    props: {
      Icon: Download,
      label: (
        /*i18n*/
        ctx[5]("common.download")
      )
    }
  });
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
      if (dirty & /*i18n*/
      32)
        iconbutton_changes.label = /*i18n*/
        ctx2[5]("common.download");
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
function create_if_block_1(ctx) {
  let sharebutton;
  let current;
  sharebutton = new ShareButton({
    props: {
      i18n: (
        /*i18n*/
        ctx[5]
      ),
      formatter: (
        /*func*/
        ctx[10]
      ),
      value: (
        /*value*/
        ctx[0]
      )
    }
  });
  sharebutton.$on(
    "error",
    /*error_handler*/
    ctx[11]
  );
  sharebutton.$on(
    "share",
    /*share_handler*/
    ctx[12]
  );
  return {
    c() {
      create_component(sharebutton.$$.fragment);
    },
    l(nodes) {
      claim_component(sharebutton.$$.fragment, nodes);
    },
    m(target, anchor) {
      mount_component(sharebutton, target, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      const sharebutton_changes = {};
      if (dirty & /*i18n*/
      32)
        sharebutton_changes.i18n = /*i18n*/
        ctx2[5];
      if (dirty & /*value*/
      1)
        sharebutton_changes.value = /*value*/
        ctx2[0];
      sharebutton.$set(sharebutton_changes);
    },
    i(local) {
      if (current)
        return;
      transition_in(sharebutton.$$.fragment, local);
      current = true;
    },
    o(local) {
      transition_out(sharebutton.$$.fragment, local);
      current = false;
    },
    d(detaching) {
      destroy_component(sharebutton, detaching);
    }
  };
}
function create_default_slot(ctx) {
  let t2;
  let if_block1_anchor;
  let current;
  let if_block0 = (
    /*show_download_button*/
    ctx[3] && create_if_block_2(ctx)
  );
  let if_block1 = (
    /*show_share_button*/
    ctx[4] && create_if_block_1(ctx)
  );
  return {
    c() {
      if (if_block0)
        if_block0.c();
      t2 = space();
      if (if_block1)
        if_block1.c();
      if_block1_anchor = empty();
    },
    l(nodes) {
      if (if_block0)
        if_block0.l(nodes);
      t2 = claim_space(nodes);
      if (if_block1)
        if_block1.l(nodes);
      if_block1_anchor = empty();
    },
    m(target, anchor) {
      if (if_block0)
        if_block0.m(target, anchor);
      insert_hydration(target, t2, anchor);
      if (if_block1)
        if_block1.m(target, anchor);
      insert_hydration(target, if_block1_anchor, anchor);
      current = true;
    },
    p(ctx2, dirty) {
      if (
        /*show_download_button*/
        ctx2[3]
      ) {
        if (if_block0) {
          if_block0.p(ctx2, dirty);
          if (dirty & /*show_download_button*/
          8) {
            transition_in(if_block0, 1);
          }
        } else {
          if_block0 = create_if_block_2(ctx2);
          if_block0.c();
          transition_in(if_block0, 1);
          if_block0.m(t2.parentNode, t2);
        }
      } else if (if_block0) {
        group_outros();
        transition_out(if_block0, 1, 1, () => {
          if_block0 = null;
        });
        check_outros();
      }
      if (
        /*show_share_button*/
        ctx2[4]
      ) {
        if (if_block1) {
          if_block1.p(ctx2, dirty);
          if (dirty & /*show_share_button*/
          16) {
            transition_in(if_block1, 1);
          }
        } else {
          if_block1 = create_if_block_1(ctx2);
          if_block1.c();
          transition_in(if_block1, 1);
          if_block1.m(if_block1_anchor.parentNode, if_block1_anchor);
        }
      } else if (if_block1) {
        group_outros();
        transition_out(if_block1, 1, 1, () => {
          if_block1 = null;
        });
        check_outros();
      }
    },
    i(local) {
      if (current)
        return;
      transition_in(if_block0);
      transition_in(if_block1);
      current = true;
    },
    o(local) {
      transition_out(if_block0);
      transition_out(if_block1);
      current = false;
    },
    d(detaching) {
      if (detaching) {
        detach(t2);
        detach(if_block1_anchor);
      }
      if (if_block0)
        if_block0.d(detaching);
      if (if_block1)
        if_block1.d(detaching);
    }
  };
}
function create_fragment(ctx) {
  let blocklabel;
  let t2;
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
      Icon: Music,
      float: false,
      label: (
        /*label*/
        ctx[1] || /*i18n*/
        ctx[5]("audio.audio")
      )
    }
  });
  const if_block_creators = [create_if_block, create_else_block];
  const if_blocks = [];
  function select_block_type(ctx2, dirty) {
    if (
      /*value*/
      ctx2[0] !== null
    )
      return 0;
    return 1;
  }
  current_block_type_index = select_block_type(ctx);
  if_block = if_blocks[current_block_type_index] = if_block_creators[current_block_type_index](ctx);
  return {
    c() {
      create_component(blocklabel.$$.fragment);
      t2 = space();
      if_block.c();
      if_block_anchor = empty();
    },
    l(nodes) {
      claim_component(blocklabel.$$.fragment, nodes);
      t2 = claim_space(nodes);
      if_block.l(nodes);
      if_block_anchor = empty();
    },
    m(target, anchor) {
      mount_component(blocklabel, target, anchor);
      insert_hydration(target, t2, anchor);
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
      if (dirty & /*label, i18n*/
      34)
        blocklabel_changes.label = /*label*/
        ctx2[1] || /*i18n*/
        ctx2[5]("audio.audio");
      blocklabel.$set(blocklabel_changes);
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
        detach(t2);
        detach(if_block_anchor);
      }
      destroy_component(blocklabel, detaching);
      if_blocks[current_block_type_index].d(detaching);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { value = null } = $$props;
  let { label } = $$props;
  let { show_label = true } = $$props;
  let { show_download_button = true } = $$props;
  let { show_share_button = false } = $$props;
  let { i18n } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options } = $$props;
  let { editable = true } = $$props;
  let { loop } = $$props;
  const dispatch = createEventDispatcher();
  const func = async (value2) => {
    if (!value2)
      return "";
    let url = await uploadToHuggingFace(value2.url);
    return `<audio controls src="${url}"></audio>`;
  };
  function error_handler(event) {
    bubble.call(this, $$self, event);
  }
  function share_handler(event) {
    bubble.call(this, $$self, event);
  }
  function pause_handler(event) {
    bubble.call(this, $$self, event);
  }
  function play_handler(event) {
    bubble.call(this, $$self, event);
  }
  function stop_handler(event) {
    bubble.call(this, $$self, event);
  }
  function load_handler(event) {
    bubble.call(this, $$self, event);
  }
  $$self.$$set = ($$props2) => {
    if ("value" in $$props2)
      $$invalidate(0, value = $$props2.value);
    if ("label" in $$props2)
      $$invalidate(1, label = $$props2.label);
    if ("show_label" in $$props2)
      $$invalidate(2, show_label = $$props2.show_label);
    if ("show_download_button" in $$props2)
      $$invalidate(3, show_download_button = $$props2.show_download_button);
    if ("show_share_button" in $$props2)
      $$invalidate(4, show_share_button = $$props2.show_share_button);
    if ("i18n" in $$props2)
      $$invalidate(5, i18n = $$props2.i18n);
    if ("waveform_settings" in $$props2)
      $$invalidate(6, waveform_settings = $$props2.waveform_settings);
    if ("waveform_options" in $$props2)
      $$invalidate(7, waveform_options = $$props2.waveform_options);
    if ("editable" in $$props2)
      $$invalidate(8, editable = $$props2.editable);
    if ("loop" in $$props2)
      $$invalidate(9, loop = $$props2.loop);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*value*/
    1) {
      value && dispatch("change", value);
    }
  };
  return [
    value,
    label,
    show_label,
    show_download_button,
    show_share_button,
    i18n,
    waveform_settings,
    waveform_options,
    editable,
    loop,
    func,
    error_handler,
    share_handler,
    pause_handler,
    play_handler,
    stop_handler,
    load_handler
  ];
}
class StaticAudio extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      value: 0,
      label: 1,
      show_label: 2,
      show_download_button: 3,
      show_share_button: 4,
      i18n: 5,
      waveform_settings: 6,
      waveform_options: 7,
      editable: 8,
      loop: 9
    });
  }
}
const StaticAudio$1 = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: StaticAudio
}, Symbol.toStringTag, { value: "Module" }));
export {
  AudioPlayer$1 as A,
  StaticAudio as S,
  WaveformControls as W,
  WaveSurfer as a,
  StaticAudio$1 as b,
  process_audio as p,
  skip_audio as s
};
