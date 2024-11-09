import { c as create_ssr_component, a as createEventDispatcher, v as validate_component, b as add_attribute, d as add_styles, e as escape } from './ssr-RaXq3SJh.js';
import { e as BlockLabel, j as Music, h as IconButtonWrapper, i as IconButton, D as Download, o as ShareButton, u as uploadToHuggingFace, g as Empty, q as format_time, r as Backward, P as Pause, t as Play, F as Forward, v as Undo, w as Trim, V as VolumeMuted, x as VolumeLow, y as VolumeHigh } from './2-B6LMYTAg.js';
import { D as DownloadLink, r as resolve_wasm_src } from './DownloadLink--4obEanq.js';
import './hls-CrxM9YLy.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';

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
    step((generator = generator.apply(thisArg, [])).next());
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
    step((generator = generator.apply(thisArg, [])).next());
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
  setMediaElement(element) {
    this.media = element;
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
function makeDraggable(element, onDrag, onStart, onEnd, threshold = 5) {
  let unsub = () => {
    return;
  };
  if (!element)
    return unsub;
  const down = (e2) => {
    if (e2.button === 2)
      return;
    e2.preventDefault();
    e2.stopPropagation();
    element.style.touchAction = "none";
    let startX = e2.clientX;
    let startY = e2.clientY;
    let isDragging = false;
    const move = (e3) => {
      e3.preventDefault();
      e3.stopPropagation();
      const x = e3.clientX;
      const y = e3.clientY;
      if (isDragging || Math.abs(x - startX) >= threshold || Math.abs(y - startY) >= threshold) {
        const { left, top } = element.getBoundingClientRect();
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
      element.style.touchAction = "";
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
  element.addEventListener("pointerdown", down);
  return () => {
    unsub();
    element.removeEventListener("pointerdown", down);
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
    step((generator = generator.apply(thisArg, [])).next());
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
    step((generator = generator.apply(thisArg, [])).next());
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
  setMediaElement(element) {
    this.unsubscribePlayerEvents();
    super.setMediaElement(element);
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
const VolumeLevels = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { currentVolume } = $$props;
  if ($$props.currentVolume === void 0 && $$bindings.currentVolume && currentVolume !== void 0)
    $$bindings.currentVolume(currentVolume);
  return `${currentVolume == 0 ? `${validate_component(VolumeMuted, "VolumeMuted").$$render($$result, {}, {}, {})}` : `${currentVolume < 0.5 ? `${validate_component(VolumeLow, "VolumeLow").$$render($$result, {}, {}, {})}` : `${currentVolume >= 0.5 ? `${validate_component(VolumeHigh, "VolumeHigh").$$render($$result, {}, {}, {})}` : ``}`}`}`;
});
const css$2 = {
  code: '.volume-slider.svelte-wuo8j5{-webkit-appearance:none;appearance:none;width:var(--size-20);accent-color:var(--color-accent);height:4px;cursor:pointer;outline:none;border-radius:15px;background-color:var(--neutral-400)}input[type="range"].svelte-wuo8j5::-webkit-slider-thumb{-webkit-appearance:none;appearance:none;height:15px;width:15px;background-color:var(--color-accent);border-radius:50%;border:none;transition:0.2s ease-in-out}input[type="range"].svelte-wuo8j5::-moz-range-thumb{height:15px;width:15px;background-color:var(--color-accent);border-radius:50%;border:none;transition:0.2s ease-in-out}',
  map: '{"version":3,"file":"VolumeControl.svelte","sources":["VolumeControl.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport WaveSurfer from \\"wavesurfer.js\\";\\nexport let currentVolume = 1;\\nexport let show_volume_slider = false;\\nexport let waveform;\\nlet volumeElement;\\nonMount(() => {\\n    adjustSlider();\\n});\\nconst adjustSlider = () => {\\n    let slider = volumeElement;\\n    if (!slider)\\n        return;\\n    slider.style.background = `linear-gradient(to right, var(--color-accent) ${currentVolume * 100}%, var(--neutral-400) ${currentVolume * 100}%)`;\\n};\\n$: currentVolume, adjustSlider();\\n<\/script>\\n\\n<input\\n\\tbind:this={volumeElement}\\n\\tid=\\"volume\\"\\n\\tclass=\\"volume-slider\\"\\n\\ttype=\\"range\\"\\n\\tmin=\\"0\\"\\n\\tmax=\\"1\\"\\n\\tstep=\\"0.01\\"\\n\\tvalue={currentVolume}\\n\\ton:focusout={() => (show_volume_slider = false)}\\n\\ton:input={(e) => {\\n\\t\\tif (e.target instanceof HTMLInputElement) {\\n\\t\\t\\tcurrentVolume = parseFloat(e.target.value);\\n\\t\\t\\twaveform?.setVolume(currentVolume);\\n\\t\\t}\\n\\t}}\\n/>\\n\\n<style>\\n\\t.volume-slider {\\n\\t\\t-webkit-appearance: none;\\n\\t\\tappearance: none;\\n\\t\\twidth: var(--size-20);\\n\\t\\taccent-color: var(--color-accent);\\n\\t\\theight: 4px;\\n\\t\\tcursor: pointer;\\n\\t\\toutline: none;\\n\\t\\tborder-radius: 15px;\\n\\t\\tbackground-color: var(--neutral-400);\\n\\t}\\n\\n\\tinput[type=\\"range\\"]::-webkit-slider-thumb {\\n\\t\\t-webkit-appearance: none;\\n\\t\\tappearance: none;\\n\\t\\theight: 15px;\\n\\t\\twidth: 15px;\\n\\t\\tbackground-color: var(--color-accent);\\n\\t\\tborder-radius: 50%;\\n\\t\\tborder: none;\\n\\t\\ttransition: 0.2s ease-in-out;\\n\\t}\\n\\n\\tinput[type=\\"range\\"]::-moz-range-thumb {\\n\\t\\theight: 15px;\\n\\t\\twidth: 15px;\\n\\t\\tbackground-color: var(--color-accent);\\n\\t\\tborder-radius: 50%;\\n\\t\\tborder: none;\\n\\t\\ttransition: 0.2s ease-in-out;\\n\\t}</style>\\n"],"names":[],"mappings":"AAqCC,4BAAe,CACd,kBAAkB,CAAE,IAAI,CACxB,UAAU,CAAE,IAAI,CAChB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,YAAY,CAAE,IAAI,cAAc,CAAC,CACjC,MAAM,CAAE,GAAG,CACX,MAAM,CAAE,OAAO,CACf,OAAO,CAAE,IAAI,CACb,aAAa,CAAE,IAAI,CACnB,gBAAgB,CAAE,IAAI,aAAa,CACpC,CAEA,KAAK,CAAC,IAAI,CAAC,OAAO,eAAC,sBAAuB,CACzC,kBAAkB,CAAE,IAAI,CACxB,UAAU,CAAE,IAAI,CAChB,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,IAAI,CACX,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,aAAa,CAAE,GAAG,CAClB,MAAM,CAAE,IAAI,CACZ,UAAU,CAAE,IAAI,CAAC,WAClB,CAEA,KAAK,CAAC,IAAI,CAAC,OAAO,eAAC,kBAAmB,CACrC,MAAM,CAAE,IAAI,CACZ,KAAK,CAAE,IAAI,CACX,gBAAgB,CAAE,IAAI,cAAc,CAAC,CACrC,aAAa,CAAE,GAAG,CAClB,MAAM,CAAE,IAAI,CACZ,UAAU,CAAE,IAAI,CAAC,WAClB"}'
};
const VolumeControl = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { currentVolume = 1 } = $$props;
  let { show_volume_slider = false } = $$props;
  let { waveform } = $$props;
  let volumeElement;
  if ($$props.currentVolume === void 0 && $$bindings.currentVolume && currentVolume !== void 0)
    $$bindings.currentVolume(currentVolume);
  if ($$props.show_volume_slider === void 0 && $$bindings.show_volume_slider && show_volume_slider !== void 0)
    $$bindings.show_volume_slider(show_volume_slider);
  if ($$props.waveform === void 0 && $$bindings.waveform && waveform !== void 0)
    $$bindings.waveform(waveform);
  $$result.css.add(css$2);
  return `<input id="volume" class="volume-slider svelte-wuo8j5" type="range" min="0" max="1" step="0.01"${add_attribute("value", currentVolume, 0)}${add_attribute("this", volumeElement, 0)}>`;
});
const css$1 = {
  code: '.settings-wrapper.svelte-ije4bl.svelte-ije4bl{display:flex;justify-self:self-end;align-items:center;grid-area:editing}.text-button.svelte-ije4bl.svelte-ije4bl{border:1px solid var(--neutral-400);border-radius:var(--radius-sm);font-weight:300;font-size:var(--size-3);text-align:center;color:var(--neutral-400);height:var(--size-5);font-weight:bold;padding:0 5px;margin-left:5px}.text-button.svelte-ije4bl.svelte-ije4bl:hover,.text-button.svelte-ije4bl.svelte-ije4bl:focus{color:var(--color-accent);border-color:var(--color-accent)}.controls.svelte-ije4bl.svelte-ije4bl{display:grid;grid-template-columns:1fr 1fr 1fr;grid-template-areas:"controls playback editing";margin-top:5px;align-items:center;position:relative;flex-wrap:wrap;justify-content:space-between}.controls.svelte-ije4bl div.svelte-ije4bl{margin:var(--size-1) 0}@media(max-width: 600px){.controls.svelte-ije4bl.svelte-ije4bl{grid-template-columns:1fr 1fr;grid-template-rows:auto auto;grid-template-areas:"playback playback"\n				"controls editing"}}@media(max-width: 319px){.controls.svelte-ije4bl.svelte-ije4bl{overflow-x:scroll}}.hidden.svelte-ije4bl.svelte-ije4bl{display:none}.control-wrapper.svelte-ije4bl.svelte-ije4bl{display:flex;justify-self:self-start;align-items:center;justify-content:space-between;grid-area:controls}.action.svelte-ije4bl.svelte-ije4bl{width:var(--size-5);color:var(--neutral-400);margin-left:var(--spacing-md)}.icon.svelte-ije4bl.svelte-ije4bl:hover,.icon.svelte-ije4bl.svelte-ije4bl:focus{color:var(--color-accent)}.play-pause-wrapper.svelte-ije4bl.svelte-ije4bl{display:flex;justify-self:center;grid-area:playback}@media(max-width: 600px){.play-pause-wrapper.svelte-ije4bl.svelte-ije4bl{margin:var(--spacing-md)}}.playback.svelte-ije4bl.svelte-ije4bl{border:1px solid var(--neutral-400);border-radius:var(--radius-sm);width:5.5ch;font-weight:300;font-size:var(--size-3);text-align:center;color:var(--neutral-400);height:var(--size-5);font-weight:bold}.playback.svelte-ije4bl.svelte-ije4bl:hover,.playback.svelte-ije4bl.svelte-ije4bl:focus{color:var(--color-accent);border-color:var(--color-accent)}.rewind.svelte-ije4bl.svelte-ije4bl,.skip.svelte-ije4bl.svelte-ije4bl{margin:0 10px;color:var(--neutral-400)}.play-pause-button.svelte-ije4bl.svelte-ije4bl{width:var(--size-8);display:flex;align-items:center;justify-content:center;color:var(--neutral-400);fill:var(--neutral-400)}.volume.svelte-ije4bl.svelte-ije4bl{position:relative;display:flex;justify-content:center;margin-right:var(--spacing-xl);width:var(--size-5)}',
  map: '{"version":3,"file":"WaveformControls.svelte","sources":["WaveformControls.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { Play, Pause, Forward, Backward, Undo, Trim } from \\"@gradio/icons\\";\\nimport { get_skip_rewind_amount } from \\"../shared/utils\\";\\nimport WaveSurfer from \\"wavesurfer.js\\";\\nimport RegionsPlugin, {} from \\"wavesurfer.js/dist/plugins/regions.js\\";\\nimport VolumeLevels from \\"./VolumeLevels.svelte\\";\\nimport VolumeControl from \\"./VolumeControl.svelte\\";\\nexport let waveform;\\nexport let audio_duration;\\nexport let i18n;\\nexport let playing;\\nexport let show_redo = false;\\nexport let interactive = false;\\nexport let handle_trim_audio;\\nexport let mode = \\"\\";\\nexport let container;\\nexport let handle_reset_value;\\nexport let waveform_options = {};\\nexport let trim_region_settings = {};\\nexport let show_volume_slider = false;\\nexport let editable = true;\\nexport let trimDuration = 0;\\nlet playbackSpeeds = [0.5, 1, 1.5, 2];\\nlet playbackSpeed = playbackSpeeds[1];\\nlet trimRegion = null;\\nlet activeRegion = null;\\nlet leftRegionHandle;\\nlet rightRegionHandle;\\nlet activeHandle = \\"\\";\\nlet currentVolume = 1;\\n$: trimRegion = container && waveform ? waveform.registerPlugin(RegionsPlugin.create()) : null;\\n$: trimRegion?.on(\\"region-out\\", (region) => {\\n    region.play();\\n});\\n$: trimRegion?.on(\\"region-updated\\", (region) => {\\n    trimDuration = region.end - region.start;\\n});\\n$: trimRegion?.on(\\"region-clicked\\", (region, e) => {\\n    e.stopPropagation();\\n    activeRegion = region;\\n    region.play();\\n});\\nconst addTrimRegion = () => {\\n    if (!trimRegion)\\n        return;\\n    activeRegion = trimRegion?.addRegion({\\n        start: audio_duration / 4,\\n        end: audio_duration / 2,\\n        ...trim_region_settings\\n    });\\n    trimDuration = activeRegion.end - activeRegion.start;\\n};\\n$: if (activeRegion) {\\n    const shadowRoot = container.children[0].shadowRoot;\\n    rightRegionHandle = shadowRoot.querySelector(\'[data-resize=\\"right\\"]\');\\n    leftRegionHandle = shadowRoot.querySelector(\'[data-resize=\\"left\\"]\');\\n    if (leftRegionHandle && rightRegionHandle) {\\n        leftRegionHandle.setAttribute(\\"role\\", \\"button\\");\\n        rightRegionHandle.setAttribute(\\"role\\", \\"button\\");\\n        leftRegionHandle?.setAttribute(\\"aria-label\\", \\"Drag to adjust start time\\");\\n        rightRegionHandle?.setAttribute(\\"aria-label\\", \\"Drag to adjust end time\\");\\n        leftRegionHandle?.setAttribute(\\"tabindex\\", \\"0\\");\\n        rightRegionHandle?.setAttribute(\\"tabindex\\", \\"0\\");\\n        leftRegionHandle.addEventListener(\\"focus\\", () => {\\n            if (trimRegion)\\n                activeHandle = \\"left\\";\\n        });\\n        rightRegionHandle.addEventListener(\\"focus\\", () => {\\n            if (trimRegion)\\n                activeHandle = \\"right\\";\\n        });\\n    }\\n}\\nconst trimAudio = () => {\\n    if (waveform && trimRegion) {\\n        if (activeRegion) {\\n            const start = activeRegion.start;\\n            const end = activeRegion.end;\\n            handle_trim_audio(start, end);\\n            mode = \\"\\";\\n            activeRegion = null;\\n        }\\n    }\\n};\\nconst clearRegions = () => {\\n    trimRegion?.getRegions().forEach((region) => {\\n        region.remove();\\n    });\\n    trimRegion?.clearRegions();\\n};\\nconst toggleTrimmingMode = () => {\\n    clearRegions();\\n    if (mode === \\"edit\\") {\\n        mode = \\"\\";\\n    }\\n    else {\\n        mode = \\"edit\\";\\n        addTrimRegion();\\n    }\\n};\\nconst adjustRegionHandles = (handle, key) => {\\n    let newStart;\\n    let newEnd;\\n    if (!activeRegion)\\n        return;\\n    if (handle === \\"left\\") {\\n        if (key === \\"ArrowLeft\\") {\\n            newStart = activeRegion.start - 0.05;\\n            newEnd = activeRegion.end;\\n        }\\n        else {\\n            newStart = activeRegion.start + 0.05;\\n            newEnd = activeRegion.end;\\n        }\\n    }\\n    else {\\n        if (key === \\"ArrowLeft\\") {\\n            newStart = activeRegion.start;\\n            newEnd = activeRegion.end - 0.05;\\n        }\\n        else {\\n            newStart = activeRegion.start;\\n            newEnd = activeRegion.end + 0.05;\\n        }\\n    }\\n    activeRegion.setOptions({\\n        start: newStart,\\n        end: newEnd\\n    });\\n    trimDuration = activeRegion.end - activeRegion.start;\\n};\\n$: trimRegion && window.addEventListener(\\"keydown\\", (e) => {\\n    if (e.key === \\"ArrowLeft\\") {\\n        adjustRegionHandles(activeHandle, \\"ArrowLeft\\");\\n    }\\n    else if (e.key === \\"ArrowRight\\") {\\n        adjustRegionHandles(activeHandle, \\"ArrowRight\\");\\n    }\\n});\\n<\/script>\\n\\n<div class=\\"controls\\" data-testid=\\"waveform-controls\\">\\n\\t<div class=\\"control-wrapper\\">\\n\\t\\t<button\\n\\t\\t\\tclass=\\"action icon volume\\"\\n\\t\\t\\tstyle:color={show_volume_slider\\n\\t\\t\\t\\t? \\"var(--color-accent)\\"\\n\\t\\t\\t\\t: \\"var(--neutral-400)\\"}\\n\\t\\t\\taria-label=\\"Adjust volume\\"\\n\\t\\t\\ton:click={() => (show_volume_slider = !show_volume_slider)}\\n\\t\\t>\\n\\t\\t\\t<VolumeLevels {currentVolume} />\\n\\t\\t</button>\\n\\n\\t\\t{#if show_volume_slider}\\n\\t\\t\\t<VolumeControl bind:currentVolume bind:show_volume_slider {waveform} />\\n\\t\\t{/if}\\n\\n\\t\\t<button\\n\\t\\t\\tclass:hidden={show_volume_slider}\\n\\t\\t\\tclass=\\"playback icon\\"\\n\\t\\t\\taria-label={`Adjust playback speed to ${\\n\\t\\t\\t\\tplaybackSpeeds[\\n\\t\\t\\t\\t\\t(playbackSpeeds.indexOf(playbackSpeed) + 1) % playbackSpeeds.length\\n\\t\\t\\t\\t]\\n\\t\\t\\t}x`}\\n\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\tplaybackSpeed =\\n\\t\\t\\t\\t\\tplaybackSpeeds[\\n\\t\\t\\t\\t\\t\\t(playbackSpeeds.indexOf(playbackSpeed) + 1) % playbackSpeeds.length\\n\\t\\t\\t\\t\\t];\\n\\n\\t\\t\\t\\twaveform?.setPlaybackRate(playbackSpeed);\\n\\t\\t\\t}}\\n\\t\\t>\\n\\t\\t\\t<span>{playbackSpeed}x</span>\\n\\t\\t</button>\\n\\t</div>\\n\\n\\t<div class=\\"play-pause-wrapper\\">\\n\\t\\t<button\\n\\t\\t\\tclass=\\"rewind icon\\"\\n\\t\\t\\taria-label={`Skip backwards by ${get_skip_rewind_amount(\\n\\t\\t\\t\\taudio_duration,\\n\\t\\t\\t\\twaveform_options.skip_length\\n\\t\\t\\t)} seconds`}\\n\\t\\t\\ton:click={() =>\\n\\t\\t\\t\\twaveform?.skip(\\n\\t\\t\\t\\t\\tget_skip_rewind_amount(audio_duration, waveform_options.skip_length) *\\n\\t\\t\\t\\t\\t\\t-1\\n\\t\\t\\t\\t)}\\n\\t\\t>\\n\\t\\t\\t<Backward />\\n\\t\\t</button>\\n\\t\\t<button\\n\\t\\t\\tclass=\\"play-pause-button icon\\"\\n\\t\\t\\ton:click={() => waveform?.playPause()}\\n\\t\\t\\taria-label={playing ? i18n(\\"audio.pause\\") : i18n(\\"audio.play\\")}\\n\\t\\t>\\n\\t\\t\\t{#if playing}\\n\\t\\t\\t\\t<Pause />\\n\\t\\t\\t{:else}\\n\\t\\t\\t\\t<Play />\\n\\t\\t\\t{/if}\\n\\t\\t</button>\\n\\t\\t<button\\n\\t\\t\\tclass=\\"skip icon\\"\\n\\t\\t\\taria-label=\\"Skip forward by {get_skip_rewind_amount(\\n\\t\\t\\t\\taudio_duration,\\n\\t\\t\\t\\twaveform_options.skip_length\\n\\t\\t\\t)} seconds\\"\\n\\t\\t\\ton:click={() =>\\n\\t\\t\\t\\twaveform?.skip(\\n\\t\\t\\t\\t\\tget_skip_rewind_amount(audio_duration, waveform_options.skip_length)\\n\\t\\t\\t\\t)}\\n\\t\\t>\\n\\t\\t\\t<Forward />\\n\\t\\t</button>\\n\\t</div>\\n\\n\\t<div class=\\"settings-wrapper\\">\\n\\t\\t{#if editable && interactive}\\n\\t\\t\\t{#if show_redo && mode === \\"\\"}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"action icon\\"\\n\\t\\t\\t\\t\\taria-label=\\"Reset audio\\"\\n\\t\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\t\\thandle_reset_value();\\n\\t\\t\\t\\t\\t\\tclearRegions();\\n\\t\\t\\t\\t\\t\\tmode = \\"\\";\\n\\t\\t\\t\\t\\t}}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<Undo />\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t{/if}\\n\\n\\t\\t\\t{#if mode === \\"\\"}\\n\\t\\t\\t\\t<button\\n\\t\\t\\t\\t\\tclass=\\"action icon\\"\\n\\t\\t\\t\\t\\taria-label=\\"Trim audio to selection\\"\\n\\t\\t\\t\\t\\ton:click={toggleTrimmingMode}\\n\\t\\t\\t\\t>\\n\\t\\t\\t\\t\\t<Trim />\\n\\t\\t\\t\\t</button>\\n\\t\\t\\t{:else}\\n\\t\\t\\t\\t<button class=\\"text-button\\" on:click={trimAudio}>Trim</button>\\n\\t\\t\\t\\t<button class=\\"text-button\\" on:click={toggleTrimmingMode}>Cancel</button\\n\\t\\t\\t\\t>\\n\\t\\t\\t{/if}\\n\\t\\t{/if}\\n\\t</div>\\n</div>\\n\\n<style>\\n\\t.settings-wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-self: self-end;\\n\\t\\talign-items: center;\\n\\t\\tgrid-area: editing;\\n\\t}\\n\\t.text-button {\\n\\t\\tborder: 1px solid var(--neutral-400);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\tfont-weight: 300;\\n\\t\\tfont-size: var(--size-3);\\n\\t\\ttext-align: center;\\n\\t\\tcolor: var(--neutral-400);\\n\\t\\theight: var(--size-5);\\n\\t\\tfont-weight: bold;\\n\\t\\tpadding: 0 5px;\\n\\t\\tmargin-left: 5px;\\n\\t}\\n\\n\\t.text-button:hover,\\n\\t.text-button:focus {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tborder-color: var(--color-accent);\\n\\t}\\n\\n\\t.controls {\\n\\t\\tdisplay: grid;\\n\\t\\tgrid-template-columns: 1fr 1fr 1fr;\\n\\t\\tgrid-template-areas: \\"controls playback editing\\";\\n\\t\\tmargin-top: 5px;\\n\\t\\talign-items: center;\\n\\t\\tposition: relative;\\n\\t\\tflex-wrap: wrap;\\n\\t\\tjustify-content: space-between;\\n\\t}\\n\\t.controls div {\\n\\t\\tmargin: var(--size-1) 0;\\n\\t}\\n\\n\\t@media (max-width: 600px) {\\n\\t\\t.controls {\\n\\t\\t\\tgrid-template-columns: 1fr 1fr;\\n\\t\\t\\tgrid-template-rows: auto auto;\\n\\t\\t\\tgrid-template-areas:\\n\\t\\t\\t\\t\\"playback playback\\"\\n\\t\\t\\t\\t\\"controls editing\\";\\n\\t\\t}\\n\\t}\\n\\n\\t@media (max-width: 319px) {\\n\\t\\t.controls {\\n\\t\\t\\toverflow-x: scroll;\\n\\t\\t}\\n\\t}\\n\\n\\t.hidden {\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.control-wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-self: self-start;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: space-between;\\n\\t\\tgrid-area: controls;\\n\\t}\\n\\n\\t.action {\\n\\t\\twidth: var(--size-5);\\n\\t\\tcolor: var(--neutral-400);\\n\\t\\tmargin-left: var(--spacing-md);\\n\\t}\\n\\t.icon:hover,\\n\\t.icon:focus {\\n\\t\\tcolor: var(--color-accent);\\n\\t}\\n\\t.play-pause-wrapper {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-self: center;\\n\\t\\tgrid-area: playback;\\n\\t}\\n\\n\\t@media (max-width: 600px) {\\n\\t\\t.play-pause-wrapper {\\n\\t\\t\\tmargin: var(--spacing-md);\\n\\t\\t}\\n\\t}\\n\\t.playback {\\n\\t\\tborder: 1px solid var(--neutral-400);\\n\\t\\tborder-radius: var(--radius-sm);\\n\\t\\twidth: 5.5ch;\\n\\t\\tfont-weight: 300;\\n\\t\\tfont-size: var(--size-3);\\n\\t\\ttext-align: center;\\n\\t\\tcolor: var(--neutral-400);\\n\\t\\theight: var(--size-5);\\n\\t\\tfont-weight: bold;\\n\\t}\\n\\n\\t.playback:hover,\\n\\t.playback:focus {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tborder-color: var(--color-accent);\\n\\t}\\n\\n\\t.rewind,\\n\\t.skip {\\n\\t\\tmargin: 0 10px;\\n\\t\\tcolor: var(--neutral-400);\\n\\t}\\n\\n\\t.play-pause-button {\\n\\t\\twidth: var(--size-8);\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\tcolor: var(--neutral-400);\\n\\t\\tfill: var(--neutral-400);\\n\\t}\\n\\n\\t.volume {\\n\\t\\tposition: relative;\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: center;\\n\\t\\tmargin-right: var(--spacing-xl);\\n\\t\\twidth: var(--size-5);\\n\\t}</style>\\n"],"names":[],"mappings":"AA6PC,6CAAkB,CACjB,OAAO,CAAE,IAAI,CACb,YAAY,CAAE,QAAQ,CACtB,WAAW,CAAE,MAAM,CACnB,SAAS,CAAE,OACZ,CACA,wCAAa,CACZ,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,aAAa,CAAC,CACpC,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,WAAW,CAAE,GAAG,CAChB,SAAS,CAAE,IAAI,QAAQ,CAAC,CACxB,UAAU,CAAE,MAAM,CAClB,KAAK,CAAE,IAAI,aAAa,CAAC,CACzB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,WAAW,CAAE,IAAI,CACjB,OAAO,CAAE,CAAC,CAAC,GAAG,CACd,WAAW,CAAE,GACd,CAEA,wCAAY,MAAM,CAClB,wCAAY,MAAO,CAClB,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,YAAY,CAAE,IAAI,cAAc,CACjC,CAEA,qCAAU,CACT,OAAO,CAAE,IAAI,CACb,qBAAqB,CAAE,GAAG,CAAC,GAAG,CAAC,GAAG,CAClC,mBAAmB,CAAE,2BAA2B,CAChD,UAAU,CAAE,GAAG,CACf,WAAW,CAAE,MAAM,CACnB,QAAQ,CAAE,QAAQ,CAClB,SAAS,CAAE,IAAI,CACf,eAAe,CAAE,aAClB,CACA,uBAAS,CAAC,iBAAI,CACb,MAAM,CAAE,IAAI,QAAQ,CAAC,CAAC,CACvB,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,qCAAU,CACT,qBAAqB,CAAE,GAAG,CAAC,GAAG,CAC9B,kBAAkB,CAAE,IAAI,CAAC,IAAI,CAC7B,mBAAmB,CAClB,mBAAmB;AACvB,IAAI,kBACF,CACD,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,qCAAU,CACT,UAAU,CAAE,MACb,CACD,CAEA,mCAAQ,CACP,OAAO,CAAE,IACV,CAEA,4CAAiB,CAChB,OAAO,CAAE,IAAI,CACb,YAAY,CAAE,UAAU,CACxB,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,aAAa,CAC9B,SAAS,CAAE,QACZ,CAEA,mCAAQ,CACP,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,KAAK,CAAE,IAAI,aAAa,CAAC,CACzB,WAAW,CAAE,IAAI,YAAY,CAC9B,CACA,iCAAK,MAAM,CACX,iCAAK,MAAO,CACX,KAAK,CAAE,IAAI,cAAc,CAC1B,CACA,+CAAoB,CACnB,OAAO,CAAE,IAAI,CACb,YAAY,CAAE,MAAM,CACpB,SAAS,CAAE,QACZ,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,+CAAoB,CACnB,MAAM,CAAE,IAAI,YAAY,CACzB,CACD,CACA,qCAAU,CACT,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,aAAa,CAAC,CACpC,aAAa,CAAE,IAAI,WAAW,CAAC,CAC/B,KAAK,CAAE,KAAK,CACZ,WAAW,CAAE,GAAG,CAChB,SAAS,CAAE,IAAI,QAAQ,CAAC,CACxB,UAAU,CAAE,MAAM,CAClB,KAAK,CAAE,IAAI,aAAa,CAAC,CACzB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,WAAW,CAAE,IACd,CAEA,qCAAS,MAAM,CACf,qCAAS,MAAO,CACf,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,YAAY,CAAE,IAAI,cAAc,CACjC,CAEA,mCAAO,CACP,iCAAM,CACL,MAAM,CAAE,CAAC,CAAC,IAAI,CACd,KAAK,CAAE,IAAI,aAAa,CACzB,CAEA,8CAAmB,CAClB,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CACvB,KAAK,CAAE,IAAI,aAAa,CAAC,CACzB,IAAI,CAAE,IAAI,aAAa,CACxB,CAEA,mCAAQ,CACP,QAAQ,CAAE,QAAQ,CAClB,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,MAAM,CACvB,YAAY,CAAE,IAAI,YAAY,CAAC,CAC/B,KAAK,CAAE,IAAI,QAAQ,CACpB"}'
};
const WaveformControls = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
    trimDuration = activeRegion.end - activeRegion.start;
  };
  if ($$props.waveform === void 0 && $$bindings.waveform && waveform !== void 0)
    $$bindings.waveform(waveform);
  if ($$props.audio_duration === void 0 && $$bindings.audio_duration && audio_duration !== void 0)
    $$bindings.audio_duration(audio_duration);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.playing === void 0 && $$bindings.playing && playing !== void 0)
    $$bindings.playing(playing);
  if ($$props.show_redo === void 0 && $$bindings.show_redo && show_redo !== void 0)
    $$bindings.show_redo(show_redo);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.handle_trim_audio === void 0 && $$bindings.handle_trim_audio && handle_trim_audio !== void 0)
    $$bindings.handle_trim_audio(handle_trim_audio);
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.trim_region_settings === void 0 && $$bindings.trim_region_settings && trim_region_settings !== void 0)
    $$bindings.trim_region_settings(trim_region_settings);
  if ($$props.show_volume_slider === void 0 && $$bindings.show_volume_slider && show_volume_slider !== void 0)
    $$bindings.show_volume_slider(show_volume_slider);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.trimDuration === void 0 && $$bindings.trimDuration && trimDuration !== void 0)
    $$bindings.trimDuration(trimDuration);
  $$result.css.add(css$1);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    trimRegion = container && waveform ? waveform.registerPlugin(s.create()) : null;
    {
      trimRegion?.on("region-out", (region) => {
        region.play();
      });
    }
    {
      trimRegion?.on("region-updated", (region) => {
        trimDuration = region.end - region.start;
      });
    }
    {
      trimRegion?.on("region-clicked", (region, e2) => {
        e2.stopPropagation();
        activeRegion = region;
        region.play();
      });
    }
    {
      if (activeRegion) {
        const shadowRoot = container.children[0].shadowRoot;
        rightRegionHandle = shadowRoot.querySelector('[data-resize="right"]');
        leftRegionHandle = shadowRoot.querySelector('[data-resize="left"]');
        if (leftRegionHandle && rightRegionHandle) {
          leftRegionHandle.setAttribute("role", "button");
          rightRegionHandle.setAttribute("role", "button");
          leftRegionHandle?.setAttribute("aria-label", "Drag to adjust start time");
          rightRegionHandle?.setAttribute("aria-label", "Drag to adjust end time");
          leftRegionHandle?.setAttribute("tabindex", "0");
          rightRegionHandle?.setAttribute("tabindex", "0");
          leftRegionHandle.addEventListener("focus", () => {
            if (trimRegion)
              activeHandle = "left";
          });
          rightRegionHandle.addEventListener("focus", () => {
            if (trimRegion)
              activeHandle = "right";
          });
        }
      }
    }
    trimRegion && window.addEventListener("keydown", (e2) => {
      if (e2.key === "ArrowLeft") {
        adjustRegionHandles(activeHandle, "ArrowLeft");
      } else if (e2.key === "ArrowRight") {
        adjustRegionHandles(activeHandle, "ArrowRight");
      }
    });
    $$rendered = `<div class="controls svelte-ije4bl" data-testid="waveform-controls"><div class="control-wrapper svelte-ije4bl"><button class="action icon volume svelte-ije4bl" aria-label="Adjust volume"${add_styles({
      "color": show_volume_slider ? "var(--color-accent)" : "var(--neutral-400)"
    })}>${validate_component(VolumeLevels, "VolumeLevels").$$render($$result, { currentVolume }, {}, {})}</button> ${show_volume_slider ? `${validate_component(VolumeControl, "VolumeControl").$$render(
      $$result,
      {
        waveform,
        currentVolume,
        show_volume_slider
      },
      {
        currentVolume: ($$value) => {
          currentVolume = $$value;
          $$settled = false;
        },
        show_volume_slider: ($$value) => {
          show_volume_slider = $$value;
          $$settled = false;
        }
      },
      {}
    )}` : ``} <button class="${["playback icon svelte-ije4bl", show_volume_slider ? "hidden" : ""].join(" ").trim()}"${add_attribute("aria-label", `Adjust playback speed to ${playbackSpeeds[(playbackSpeeds.indexOf(playbackSpeed) + 1) % playbackSpeeds.length]}x`, 0)}><span>${escape(playbackSpeed)}x</span></button></div> <div class="play-pause-wrapper svelte-ije4bl"><button class="rewind icon svelte-ije4bl"${add_attribute("aria-label", `Skip backwards by ${get_skip_rewind_amount(audio_duration, waveform_options.skip_length)} seconds`, 0)}>${validate_component(Backward, "Backward").$$render($$result, {}, {}, {})}</button> <button class="play-pause-button icon svelte-ije4bl"${add_attribute("aria-label", playing ? i18n("audio.pause") : i18n("audio.play"), 0)}>${playing ? `${validate_component(Pause, "Pause").$$render($$result, {}, {}, {})}` : `${validate_component(Play, "Play").$$render($$result, {}, {}, {})}`}</button> <button class="skip icon svelte-ije4bl" aria-label="${"Skip forward by " + escape(get_skip_rewind_amount(audio_duration, waveform_options.skip_length), true) + " seconds"}">${validate_component(Forward, "Forward").$$render($$result, {}, {}, {})}</button></div> <div class="settings-wrapper svelte-ije4bl">${editable && interactive ? `${show_redo && mode === "" ? `<button class="action icon svelte-ije4bl" aria-label="Reset audio">${validate_component(Undo, "Undo").$$render($$result, {}, {}, {})}</button>` : ``} ${mode === "" ? `<button class="action icon svelte-ije4bl" aria-label="Trim audio to selection">${validate_component(Trim, "Trim").$$render($$result, {}, {}, {})}</button>` : `<button class="text-button svelte-ije4bl" data-svelte-h="svelte-1brf00d">Trim</button> <button class="text-button svelte-ije4bl" data-svelte-h="svelte-1r0ma01">Cancel</button>`}` : ``}</div> </div>`;
  } while (!$$settled);
  return $$rendered;
});
const css = {
  code: ".component-wrapper.svelte-19usgod{padding:var(--size-3);width:100%}::part(wrapper){margin-bottom:var(--size-2)}.timestamps.svelte-19usgod{display:flex;justify-content:space-between;align-items:center;width:100%;padding:var(--size-1) 0}#time.svelte-19usgod{color:var(--neutral-400)}#duration.svelte-19usgod{color:var(--neutral-400)}#trim-duration.svelte-19usgod{color:var(--color-accent);margin-right:var(--spacing-sm)}.waveform-container.svelte-19usgod{display:flex;align-items:center;justify-content:center;width:var(--size-full)}#waveform.svelte-19usgod{width:100%;height:100%;position:relative}.standard-player.svelte-19usgod{width:100%;padding:var(--size-2)}.hidden.svelte-19usgod{display:none}",
  map: '{"version":3,"file":"AudioPlayer.svelte","sources":["AudioPlayer.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport { Music } from \\"@gradio/icons\\";\\nimport { format_time } from \\"@gradio/utils\\";\\nimport WaveSurfer from \\"wavesurfer.js\\";\\nimport { skip_audio, process_audio } from \\"../shared/utils\\";\\nimport WaveformControls from \\"../shared/WaveformControls.svelte\\";\\nimport { Empty } from \\"@gradio/atoms\\";\\nimport { resolve_wasm_src } from \\"@gradio/wasm/svelte\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nimport Hls from \\"hls.js\\";\\nexport let value = null;\\n$: url = value?.url;\\nexport let label;\\nexport let i18n;\\nexport let dispatch_blob = () => Promise.resolve();\\nexport let interactive = false;\\nexport let editable = true;\\nexport let trim_region_settings = {};\\nexport let waveform_settings;\\nexport let waveform_options;\\nexport let mode = \\"\\";\\nexport let loop;\\nexport let handle_reset_value = () => {\\n};\\nlet container;\\nlet waveform;\\nlet playing = false;\\nlet timeRef;\\nlet durationRef;\\nlet audio_duration;\\nlet trimDuration = 0;\\nlet show_volume_slider = false;\\nlet audio_player;\\nlet stream_active = false;\\nconst dispatch = createEventDispatcher();\\nconst create_waveform = () => {\\n    waveform = WaveSurfer.create({\\n        container,\\n        ...waveform_settings\\n    });\\n    resolve_wasm_src(value?.url).then((resolved_src) => {\\n        if (resolved_src && waveform) {\\n            return waveform.load(resolved_src);\\n        }\\n    });\\n};\\n$: if (!value?.is_stream && container !== void 0 && container !== null) {\\n    if (waveform !== void 0)\\n        waveform.destroy();\\n    container.innerHTML = \\"\\";\\n    create_waveform();\\n    playing = false;\\n}\\n$: waveform?.on(\\"decode\\", (duration) => {\\n    audio_duration = duration;\\n    durationRef && (durationRef.textContent = format_time(duration));\\n});\\n$: waveform?.on(\\"timeupdate\\", (currentTime) => timeRef && (timeRef.textContent = format_time(currentTime)));\\n$: waveform?.on(\\"ready\\", () => {\\n    if (!waveform_settings.autoplay) {\\n        waveform?.stop();\\n    }\\n    else {\\n        waveform?.play();\\n    }\\n});\\n$: waveform?.on(\\"finish\\", () => {\\n    if (loop) {\\n        waveform?.play();\\n    }\\n    else {\\n        playing = false;\\n        dispatch(\\"stop\\");\\n    }\\n});\\n$: waveform?.on(\\"pause\\", () => {\\n    playing = false;\\n    dispatch(\\"pause\\");\\n});\\n$: waveform?.on(\\"play\\", () => {\\n    playing = true;\\n    dispatch(\\"play\\");\\n});\\n$: waveform?.on(\\"load\\", () => {\\n    dispatch(\\"load\\");\\n});\\nconst handle_trim_audio = async (start, end) => {\\n    mode = \\"\\";\\n    const decodedData = waveform?.getDecodedData();\\n    if (decodedData)\\n        await process_audio(decodedData, start, end, waveform_settings.sampleRate).then(async (trimmedBlob) => {\\n            await dispatch_blob([trimmedBlob], \\"change\\");\\n            waveform?.destroy();\\n            container.innerHTML = \\"\\";\\n        });\\n    dispatch(\\"edit\\");\\n};\\nasync function load_audio(data) {\\n    stream_active = false;\\n    await resolve_wasm_src(data).then((resolved_src) => {\\n        if (!resolved_src || value?.is_stream)\\n            return;\\n        return waveform?.load(resolved_src);\\n    });\\n}\\n$: url && load_audio(url);\\nfunction load_stream(value2) {\\n    if (!value2 || !value2.is_stream || !value2.url)\\n        return;\\n    if (!audio_player)\\n        return;\\n    if (Hls.isSupported() && !stream_active) {\\n        const hls = new Hls({\\n            maxBufferLength: 1,\\n            maxMaxBufferLength: 1,\\n            lowLatencyMode: true\\n        });\\n        hls.loadSource(value2.url);\\n        hls.attachMedia(audio_player);\\n        hls.on(Hls.Events.MANIFEST_PARSED, function () {\\n            if (waveform_settings.autoplay)\\n                audio_player.play();\\n        });\\n        hls.on(Hls.Events.ERROR, function (event, data) {\\n            console.error(\\"HLS error:\\", event, data);\\n            if (data.fatal) {\\n                switch (data.type) {\\n                    case Hls.ErrorTypes.NETWORK_ERROR:\\n                        console.error(\\"Fatal network error encountered, trying to recover\\");\\n                        hls.startLoad();\\n                        break;\\n                    case Hls.ErrorTypes.MEDIA_ERROR:\\n                        console.error(\\"Fatal media error encountered, trying to recover\\");\\n                        hls.recoverMediaError();\\n                        break;\\n                    default:\\n                        console.error(\\"Fatal error, cannot recover\\");\\n                        hls.destroy();\\n                        break;\\n                }\\n            }\\n        });\\n        stream_active = true;\\n    }\\n    else if (!stream_active) {\\n        audio_player.src = value2.url;\\n        if (waveform_settings.autoplay)\\n            audio_player.play();\\n        stream_active = true;\\n    }\\n}\\n$: load_stream(value);\\nonMount(() => {\\n    window.addEventListener(\\"keydown\\", (e) => {\\n        if (!waveform || show_volume_slider)\\n            return;\\n        if (e.key === \\"ArrowRight\\" && mode !== \\"edit\\") {\\n            skip_audio(waveform, 0.1);\\n        }\\n        else if (e.key === \\"ArrowLeft\\" && mode !== \\"edit\\") {\\n            skip_audio(waveform, -0.1);\\n        }\\n    });\\n});\\n<\/script>\\n\\n<audio\\n\\tclass=\\"standard-player\\"\\n\\tclass:hidden={!(value && value.is_stream)}\\n\\tcontrols\\n\\tautoplay={waveform_settings.autoplay}\\n\\ton:load\\n\\tbind:this={audio_player}\\n\\ton:ended={() => dispatch(\\"stop\\")}\\n\\ton:play={() => dispatch(\\"play\\")}\\n/>\\n{#if value === null}\\n\\t<Empty size=\\"small\\">\\n\\t\\t<Music />\\n\\t</Empty>\\n{:else if !value.is_stream}\\n\\t<div\\n\\t\\tclass=\\"component-wrapper\\"\\n\\t\\tdata-testid={label ? \\"waveform-\\" + label : \\"unlabelled-audio\\"}\\n\\t>\\n\\t\\t<div class=\\"waveform-container\\">\\n\\t\\t\\t<div\\n\\t\\t\\t\\tid=\\"waveform\\"\\n\\t\\t\\t\\tbind:this={container}\\n\\t\\t\\t\\tstyle:height={container ? null : \\"58px\\"}\\n\\t\\t\\t/>\\n\\t\\t</div>\\n\\n\\t\\t<div class=\\"timestamps\\">\\n\\t\\t\\t<time bind:this={timeRef} id=\\"time\\">0:00</time>\\n\\t\\t\\t<div>\\n\\t\\t\\t\\t{#if mode === \\"edit\\" && trimDuration > 0}\\n\\t\\t\\t\\t\\t<time id=\\"trim-duration\\">{format_time(trimDuration)}</time>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t<time bind:this={durationRef} id=\\"duration\\">0:00</time>\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\n\\t\\t<!-- {#if waveform} -->\\n\\t\\t<WaveformControls\\n\\t\\t\\t{container}\\n\\t\\t\\t{waveform}\\n\\t\\t\\t{playing}\\n\\t\\t\\t{audio_duration}\\n\\t\\t\\t{i18n}\\n\\t\\t\\t{interactive}\\n\\t\\t\\t{handle_trim_audio}\\n\\t\\t\\tbind:mode\\n\\t\\t\\tbind:trimDuration\\n\\t\\t\\tbind:show_volume_slider\\n\\t\\t\\tshow_redo={interactive}\\n\\t\\t\\t{handle_reset_value}\\n\\t\\t\\t{waveform_options}\\n\\t\\t\\t{trim_region_settings}\\n\\t\\t\\t{editable}\\n\\t\\t/>\\n\\t\\t<!-- {/if} -->\\n\\t</div>\\n{/if}\\n\\n<style>\\n\\t.component-wrapper {\\n\\t\\tpadding: var(--size-3);\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t:global(::part(wrapper)) {\\n\\t\\tmargin-bottom: var(--size-2);\\n\\t}\\n\\n\\t.timestamps {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: center;\\n\\t\\twidth: 100%;\\n\\t\\tpadding: var(--size-1) 0;\\n\\t}\\n\\n\\t#time {\\n\\t\\tcolor: var(--neutral-400);\\n\\t}\\n\\n\\t#duration {\\n\\t\\tcolor: var(--neutral-400);\\n\\t}\\n\\n\\t#trim-duration {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tmargin-right: var(--spacing-sm);\\n\\t}\\n\\t.waveform-container {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: center;\\n\\t\\twidth: var(--size-full);\\n\\t}\\n\\n\\t#waveform {\\n\\t\\twidth: 100%;\\n\\t\\theight: 100%;\\n\\t\\tposition: relative;\\n\\t}\\n\\n\\t.standard-player {\\n\\t\\twidth: 100%;\\n\\t\\tpadding: var(--size-2);\\n\\t}\\n\\n\\t.hidden {\\n\\t\\tdisplay: none;\\n\\t}</style>\\n"],"names":[],"mappings":"AAkOC,iCAAmB,CAClB,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,KAAK,CAAE,IACR,CAEQ,eAAiB,CACxB,aAAa,CAAE,IAAI,QAAQ,CAC5B,CAEA,0BAAY,CACX,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,CACX,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,CACxB,CAEA,oBAAM,CACL,KAAK,CAAE,IAAI,aAAa,CACzB,CAEA,wBAAU,CACT,KAAK,CAAE,IAAI,aAAa,CACzB,CAEA,6BAAe,CACd,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,YAAY,CAAE,IAAI,YAAY,CAC/B,CACA,kCAAoB,CACnB,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,MAAM,CACvB,KAAK,CAAE,IAAI,WAAW,CACvB,CAEA,wBAAU,CACT,KAAK,CAAE,IAAI,CACX,MAAM,CAAE,IAAI,CACZ,QAAQ,CAAE,QACX,CAEA,+BAAiB,CAChB,KAAK,CAAE,IAAI,CACX,OAAO,CAAE,IAAI,QAAQ,CACtB,CAEA,sBAAQ,CACP,OAAO,CAAE,IACV"}'
};
const AudioPlayer = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
  const dispatch = createEventDispatcher();
  const create_waveform = () => {
    waveform = WaveSurfer.create({ container, ...waveform_settings });
    resolve_wasm_src(value?.url).then((resolved_src) => {
      if (resolved_src && waveform) {
        return waveform.load(resolved_src);
      }
    });
  };
  const handle_trim_audio = async (start, end) => {
    mode = "";
    const decodedData = waveform?.getDecodedData();
    if (decodedData)
      await process_audio(decodedData, start, end, waveform_settings.sampleRate).then(async (trimmedBlob) => {
        await dispatch_blob([trimmedBlob], "change");
        waveform?.destroy();
        container.innerHTML = "";
      });
    dispatch("edit");
  };
  async function load_audio(data) {
    await resolve_wasm_src(data).then((resolved_src) => {
      if (!resolved_src || value?.is_stream)
        return;
      return waveform?.load(resolved_src);
    });
  }
  function load_stream(value2) {
    if (!value2 || !value2.is_stream || !value2.url)
      return;
    return;
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.dispatch_blob === void 0 && $$bindings.dispatch_blob && dispatch_blob !== void 0)
    $$bindings.dispatch_blob(dispatch_blob);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.trim_region_settings === void 0 && $$bindings.trim_region_settings && trim_region_settings !== void 0)
    $$bindings.trim_region_settings(trim_region_settings);
  if ($$props.waveform_settings === void 0 && $$bindings.waveform_settings && waveform_settings !== void 0)
    $$bindings.waveform_settings(waveform_settings);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    url = value?.url;
    {
      if (!value?.is_stream && container !== void 0 && container !== null) {
        if (waveform !== void 0)
          waveform.destroy();
        container.innerHTML = "";
        create_waveform();
        playing = false;
      }
    }
    {
      waveform?.on("decode", (duration) => {
        audio_duration = duration;
      });
    }
    {
      waveform?.on("timeupdate", (currentTime) => timeRef);
    }
    {
      waveform?.on("ready", () => {
        if (!waveform_settings.autoplay) {
          waveform?.stop();
        } else {
          waveform?.play();
        }
      });
    }
    {
      waveform?.on("finish", () => {
        if (loop) {
          waveform?.play();
        } else {
          playing = false;
          dispatch("stop");
        }
      });
    }
    {
      waveform?.on("pause", () => {
        playing = false;
        dispatch("pause");
      });
    }
    {
      waveform?.on("play", () => {
        playing = true;
        dispatch("play");
      });
    }
    {
      waveform?.on("load", () => {
        dispatch("load");
      });
    }
    url && load_audio(url);
    {
      load_stream(value);
    }
    $$rendered = `<audio class="${[
      "standard-player svelte-19usgod",
      !(value && value.is_stream) ? "hidden" : ""
    ].join(" ").trim()}" controls ${waveform_settings.autoplay ? "autoplay" : ""}${add_attribute("this", audio_player, 0)}></audio> ${value === null ? `${validate_component(Empty, "Empty").$$render($$result, { size: "small" }, {}, {
      default: () => {
        return `${validate_component(Music, "Music").$$render($$result, {}, {}, {})}`;
      }
    })}` : `${!value.is_stream ? `<div class="component-wrapper svelte-19usgod"${add_attribute("data-testid", label ? "waveform-" + label : "unlabelled-audio", 0)}><div class="waveform-container svelte-19usgod"><div id="waveform" class="svelte-19usgod"${add_styles({ "height": "58px" })}${add_attribute("this", container, 0)}></div></div> <div class="timestamps svelte-19usgod"><time id="time" class="svelte-19usgod"${add_attribute("this", timeRef, 0)} data-svelte-h="svelte-lp3mlp">0:00</time> <div>${mode === "edit" && trimDuration > 0 ? `<time id="trim-duration" class="svelte-19usgod">${escape(format_time(trimDuration))}</time>` : ``} <time id="duration" class="svelte-19usgod"${add_attribute("this", durationRef, 0)} data-svelte-h="svelte-1jd0owv">0:00</time></div></div>  ${validate_component(WaveformControls, "WaveformControls").$$render(
      $$result,
      {
        container,
        waveform,
        playing,
        audio_duration,
        i18n,
        interactive,
        handle_trim_audio,
        show_redo: interactive,
        handle_reset_value,
        waveform_options,
        trim_region_settings,
        editable,
        mode,
        trimDuration,
        show_volume_slider
      },
      {
        mode: ($$value) => {
          mode = $$value;
          $$settled = false;
        },
        trimDuration: ($$value) => {
          trimDuration = $$value;
          $$settled = false;
        },
        show_volume_slider: ($$value) => {
          show_volume_slider = $$value;
          $$settled = false;
        }
      },
      {}
    )} </div>` : ``}`}`;
  } while (!$$settled);
  return $$rendered;
});
const AudioPlayer$1 = AudioPlayer;
const StaticAudio = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.waveform_settings === void 0 && $$bindings.waveform_settings && waveform_settings !== void 0)
    $$bindings.waveform_settings(waveform_settings);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  value && dispatch("change", value);
  return `${validate_component(BlockLabel, "BlockLabel").$$render(
    $$result,
    {
      show_label,
      Icon: Music,
      float: false,
      label: label || i18n("audio.audio")
    },
    {},
    {}
  )} ${value !== null ? `${validate_component(IconButtonWrapper, "IconButtonWrapper").$$render($$result, {}, {}, {
    default: () => {
      return `${show_download_button ? `${validate_component(DownloadLink, "DownloadLink").$$render(
        $$result,
        {
          href: value.is_stream ? value.url?.replace("playlist.m3u8", "playlist-file") : value.url,
          download: value.orig_name || value.path
        },
        {},
        {
          default: () => {
            return `${validate_component(IconButton, "IconButton").$$render(
              $$result,
              {
                Icon: Download,
                label: i18n("common.download")
              },
              {},
              {}
            )}`;
          }
        }
      )}` : ``} ${show_share_button ? `${validate_component(ShareButton, "ShareButton").$$render(
        $$result,
        {
          i18n,
          formatter: async (value2) => {
            if (!value2)
              return "";
            let url = await uploadToHuggingFace(value2.url);
            return `<audio controls src="${url}"></audio>`;
          },
          value
        },
        {},
        {}
      )}` : ``}`;
    }
  })} ${validate_component(AudioPlayer$1, "AudioPlayer").$$render(
    $$result,
    {
      value,
      label,
      i18n,
      waveform_settings,
      waveform_options,
      editable,
      loop
    },
    {},
    {}
  )}` : `${validate_component(Empty, "Empty").$$render($$result, { size: "small" }, {}, {
    default: () => {
      return `${validate_component(Music, "Music").$$render($$result, {}, {}, {})}`;
    }
  })}`}`;
});
const StaticAudio$1 = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  default: StaticAudio
}, Symbol.toStringTag, { value: "Module" }));

export { AudioPlayer$1 as A, StaticAudio as S, StaticAudio$1 as a, skip_audio as s };
//# sourceMappingURL=StaticAudio-BZzN-JME.js.map
