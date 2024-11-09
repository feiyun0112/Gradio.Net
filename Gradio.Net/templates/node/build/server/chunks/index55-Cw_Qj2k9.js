import { c as create_ssr_component, a as createEventDispatcher, o as onDestroy, v as validate_component, b as add_attribute, d as add_styles, e as escape, n as null_to_empty, f as each } from './ssr-RaXq3SJh.js';
import { A as AudioPlayer$1, S as StaticAudio } from './StaticAudio-BZzN-JME.js';
import { M as ModifyUpload, U as Upload } from './ModifyUpload-CsV9IzIz.js';
import { e as BlockLabel, j as Music, b as StreamingBar, k as SelectSource, B as Block, S as Static, U as UploadText, l as Spinner, n as prepare_files } from './2-B6LMYTAg.js';
export { default as BaseExample } from './Example3-CcyZAx20.js';
import './DownloadLink--4obEanq.js';
import './hls-CrxM9YLy.js';
import './Component-Dv7eSVA_.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';

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
const css$4 = {
  code: ".mic-select.svelte-1ya9x7a{height:var(--size-8);background:var(--block-background-fill);padding:0px var(--spacing-xxl);border-radius:var(--button-large-radius);font-size:var(--text-md);border:1px solid var(--block-border-color);gap:var(--size-1)}select.svelte-1ya9x7a{text-overflow:ellipsis;max-width:var(--size-40)}@media(max-width: 375px){select.svelte-1ya9x7a{width:100%}}",
  map: '{"version":3,"file":"DeviceSelect.svelte","sources":["DeviceSelect.svelte"],"sourcesContent":["<script lang=\\"ts\\">import RecordPlugin from \\"wavesurfer.js/dist/plugins/record.js\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nexport let i18n;\\nexport let micDevices = [];\\nconst dispatch = createEventDispatcher();\\n$: if (typeof window !== \\"undefined\\") {\\n    try {\\n        let tempDevices = [];\\n        RecordPlugin.getAvailableAudioDevices().then((devices) => {\\n            micDevices = devices;\\n            devices.forEach((device) => {\\n                if (device.deviceId) {\\n                    tempDevices.push(device);\\n                }\\n            });\\n            micDevices = tempDevices;\\n        });\\n    }\\n    catch (err) {\\n        if (err instanceof DOMException && err.name == \\"NotAllowedError\\") {\\n            dispatch(\\"error\\", i18n(\\"audio.allow_recording_access\\"));\\n        }\\n        throw err;\\n    }\\n}\\n<\/script>\\n\\n<select\\n\\tclass=\\"mic-select\\"\\n\\taria-label=\\"Select input device\\"\\n\\tdisabled={micDevices.length === 0}\\n>\\n\\t{#if micDevices.length === 0}\\n\\t\\t<option value=\\"\\">{i18n(\\"audio.no_microphone\\")}</option>\\n\\t{:else}\\n\\t\\t{#each micDevices as micDevice}\\n\\t\\t\\t<option value={micDevice.deviceId}>{micDevice.label}</option>\\n\\t\\t{/each}\\n\\t{/if}\\n</select>\\n\\n<style>\\n\\t.mic-select {\\n\\t\\theight: var(--size-8);\\n\\t\\tbackground: var(--block-background-fill);\\n\\t\\tpadding: 0px var(--spacing-xxl);\\n\\t\\tborder-radius: var(--button-large-radius);\\n\\t\\tfont-size: var(--text-md);\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tgap: var(--size-1);\\n\\t}\\n\\n\\tselect {\\n\\t\\ttext-overflow: ellipsis;\\n\\t\\tmax-width: var(--size-40);\\n\\t}\\n\\n\\t@media (max-width: 375px) {\\n\\t\\tselect {\\n\\t\\t\\twidth: 100%;\\n\\t\\t}\\n\\t}</style>\\n"],"names":[],"mappings":"AA0CC,0BAAY,CACX,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,UAAU,CAAE,IAAI,uBAAuB,CAAC,CACxC,OAAO,CAAE,GAAG,CAAC,IAAI,aAAa,CAAC,CAC/B,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,SAAS,CAAE,IAAI,SAAS,CAAC,CACzB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CAC3C,GAAG,CAAE,IAAI,QAAQ,CAClB,CAEA,qBAAO,CACN,aAAa,CAAE,QAAQ,CACvB,SAAS,CAAE,IAAI,SAAS,CACzB,CAEA,MAAO,YAAY,KAAK,CAAE,CACzB,qBAAO,CACN,KAAK,CAAE,IACR,CACD"}'
};
const DeviceSelect = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { i18n } = $$props;
  let { micDevices = [] } = $$props;
  const dispatch = createEventDispatcher();
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.micDevices === void 0 && $$bindings.micDevices && micDevices !== void 0)
    $$bindings.micDevices(micDevices);
  $$result.css.add(css$4);
  {
    if (typeof window !== "undefined") {
      try {
        let tempDevices = [];
        r.getAvailableAudioDevices().then((devices) => {
          micDevices = devices;
          devices.forEach((device) => {
            if (device.deviceId) {
              tempDevices.push(device);
            }
          });
          micDevices = tempDevices;
        });
      } catch (err) {
        if (err instanceof DOMException && err.name == "NotAllowedError") {
          dispatch("error", i18n("audio.allow_recording_access"));
        }
        throw err;
      }
    }
  }
  return `<select class="mic-select svelte-1ya9x7a" aria-label="Select input device" ${micDevices.length === 0 ? "disabled" : ""}>${micDevices.length === 0 ? `<option value="">${escape(i18n("audio.no_microphone"))}</option>` : `${each(micDevices, (micDevice) => {
    return `<option${add_attribute("value", micDevice.deviceId, 0)}>${escape(micDevice.label)}</option>`;
  })}`}</select>`;
});
const css$2 = {
  code: ".microphone.svelte-9n45fh{width:100%;display:none}.component-wrapper.svelte-9n45fh{padding:var(--size-3);width:100%}.timestamps.svelte-9n45fh{display:flex;justify-content:space-between;align-items:center;width:100%;padding:var(--size-1) 0;margin:var(--spacing-md) 0}.time.svelte-9n45fh{color:var(--neutral-400)}.duration.svelte-9n45fh{color:var(--neutral-400)}.trim-duration.svelte-9n45fh{color:var(--color-accent);margin-right:var(--spacing-sm)}",
  map: '{"version":3,"file":"AudioRecorder.svelte","sources":["AudioRecorder.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport { createEventDispatcher } from \\"svelte\\";\\nimport WaveSurfer from \\"wavesurfer.js\\";\\nimport { skip_audio, process_audio } from \\"../shared/utils\\";\\nimport WSRecord from \\"wavesurfer.js/dist/plugins/record.js\\";\\nimport WaveformControls from \\"../shared/WaveformControls.svelte\\";\\nimport WaveformRecordControls from \\"../shared/WaveformRecordControls.svelte\\";\\nimport RecordPlugin from \\"wavesurfer.js/dist/plugins/record.js\\";\\nimport { format_time } from \\"@gradio/utils\\";\\nexport let mode;\\nexport let i18n;\\nexport let dispatch_blob;\\nexport let waveform_settings;\\nexport let waveform_options = {\\n    show_recording_waveform: true\\n};\\nexport let handle_reset_value;\\nexport let editable = true;\\nexport let recording = false;\\nlet micWaveform;\\nlet recordingWaveform;\\nlet playing = false;\\nlet recordingContainer;\\nlet microphoneContainer;\\nlet record;\\nlet recordedAudio = null;\\nlet timeRef;\\nlet durationRef;\\nlet audio_duration;\\nlet seconds = 0;\\nlet interval;\\nlet timing = false;\\nlet trimDuration = 0;\\nconst start_interval = () => {\\n    clearInterval(interval);\\n    interval = setInterval(() => {\\n        seconds++;\\n    }, 1e3);\\n};\\nconst dispatch = createEventDispatcher();\\nfunction record_start_callback() {\\n    start_interval();\\n    timing = true;\\n    dispatch(\\"start_recording\\");\\n    if (waveform_options.show_recording_waveform) {\\n        let waveformCanvas = microphoneContainer;\\n        if (waveformCanvas)\\n            waveformCanvas.style.display = \\"block\\";\\n    }\\n}\\nasync function record_end_callback(blob) {\\n    seconds = 0;\\n    timing = false;\\n    clearInterval(interval);\\n    try {\\n        const array_buffer = await blob.arrayBuffer();\\n        const context = new AudioContext({\\n            sampleRate: waveform_settings.sampleRate\\n        });\\n        const audio_buffer = await context.decodeAudioData(array_buffer);\\n        if (audio_buffer)\\n            await process_audio(audio_buffer).then(async (audio) => {\\n                await dispatch_blob([audio], \\"change\\");\\n                await dispatch_blob([audio], \\"stop_recording\\");\\n            });\\n    }\\n    catch (e) {\\n        console.error(e);\\n    }\\n}\\n$: record?.on(\\"record-resume\\", () => {\\n    start_interval();\\n});\\n$: recordingWaveform?.on(\\"decode\\", (duration) => {\\n    audio_duration = duration;\\n    durationRef && (durationRef.textContent = format_time(duration));\\n});\\n$: recordingWaveform?.on(\\"timeupdate\\", (currentTime) => timeRef && (timeRef.textContent = format_time(currentTime)));\\n$: recordingWaveform?.on(\\"pause\\", () => {\\n    dispatch(\\"pause\\");\\n    playing = false;\\n});\\n$: recordingWaveform?.on(\\"play\\", () => {\\n    dispatch(\\"play\\");\\n    playing = true;\\n});\\n$: recordingWaveform?.on(\\"finish\\", () => {\\n    dispatch(\\"stop\\");\\n    playing = false;\\n});\\nconst create_mic_waveform = () => {\\n    if (microphoneContainer)\\n        microphoneContainer.innerHTML = \\"\\";\\n    if (micWaveform !== void 0)\\n        micWaveform.destroy();\\n    if (!microphoneContainer)\\n        return;\\n    micWaveform = WaveSurfer.create({\\n        ...waveform_settings,\\n        normalize: false,\\n        container: microphoneContainer\\n    });\\n    record = micWaveform.registerPlugin(RecordPlugin.create());\\n    record.startMic();\\n    record?.on(\\"record-end\\", record_end_callback);\\n    record?.on(\\"record-start\\", record_start_callback);\\n    record?.on(\\"record-pause\\", () => {\\n        dispatch(\\"pause_recording\\");\\n        clearInterval(interval);\\n    });\\n    record?.on(\\"record-end\\", (blob) => {\\n        recordedAudio = URL.createObjectURL(blob);\\n        const microphone = microphoneContainer;\\n        const recording2 = recordingContainer;\\n        if (microphone)\\n            microphone.style.display = \\"none\\";\\n        if (recording2 && recordedAudio) {\\n            recording2.innerHTML = \\"\\";\\n            create_recording_waveform();\\n        }\\n    });\\n};\\nconst create_recording_waveform = () => {\\n    let recording2 = recordingContainer;\\n    if (!recordedAudio || !recording2)\\n        return;\\n    recordingWaveform = WaveSurfer.create({\\n        container: recording2,\\n        url: recordedAudio,\\n        ...waveform_settings\\n    });\\n};\\nconst handle_trim_audio = async (start, end) => {\\n    mode = \\"edit\\";\\n    const decodedData = recordingWaveform.getDecodedData();\\n    if (decodedData)\\n        await process_audio(decodedData, start, end).then(async (trimmedAudio) => {\\n            await dispatch_blob([trimmedAudio], \\"change\\");\\n            await dispatch_blob([trimmedAudio], \\"stop_recording\\");\\n            recordingWaveform.destroy();\\n            create_recording_waveform();\\n        });\\n    dispatch(\\"edit\\");\\n};\\nonMount(() => {\\n    create_mic_waveform();\\n    window.addEventListener(\\"keydown\\", (e) => {\\n        if (e.key === \\"ArrowRight\\") {\\n            skip_audio(recordingWaveform, 0.1);\\n        }\\n        else if (e.key === \\"ArrowLeft\\") {\\n            skip_audio(recordingWaveform, -0.1);\\n        }\\n    });\\n});\\n<\/script>\\n\\n<div class=\\"component-wrapper\\">\\n\\t<div\\n\\t\\tclass=\\"microphone\\"\\n\\t\\tbind:this={microphoneContainer}\\n\\t\\tdata-testid=\\"microphone-waveform\\"\\n\\t/>\\n\\t<div bind:this={recordingContainer} data-testid=\\"recording-waveform\\" />\\n\\n\\t{#if (timing || recordedAudio) && waveform_options.show_recording_waveform}\\n\\t\\t<div class=\\"timestamps\\">\\n\\t\\t\\t<time bind:this={timeRef} class=\\"time\\">0:00</time>\\n\\t\\t\\t<div>\\n\\t\\t\\t\\t{#if mode === \\"edit\\" && trimDuration > 0}\\n\\t\\t\\t\\t\\t<time class=\\"trim-duration\\">{format_time(trimDuration)}</time>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t\\t{#if timing}\\n\\t\\t\\t\\t\\t<time class=\\"duration\\">{format_time(seconds)}</time>\\n\\t\\t\\t\\t{:else}\\n\\t\\t\\t\\t\\t<time bind:this={durationRef} class=\\"duration\\">0:00</time>\\n\\t\\t\\t\\t{/if}\\n\\t\\t\\t</div>\\n\\t\\t</div>\\n\\t{/if}\\n\\n\\t{#if microphoneContainer && !recordedAudio}\\n\\t\\t<WaveformRecordControls\\n\\t\\t\\tbind:record\\n\\t\\t\\t{i18n}\\n\\t\\t\\t{timing}\\n\\t\\t\\t{recording}\\n\\t\\t\\tshow_recording_waveform={waveform_options.show_recording_waveform}\\n\\t\\t\\trecord_time={format_time(seconds)}\\n\\t\\t/>\\n\\t{/if}\\n\\n\\t{#if recordingWaveform && recordedAudio}\\n\\t\\t<WaveformControls\\n\\t\\t\\tbind:waveform={recordingWaveform}\\n\\t\\t\\tcontainer={recordingContainer}\\n\\t\\t\\t{playing}\\n\\t\\t\\t{audio_duration}\\n\\t\\t\\t{i18n}\\n\\t\\t\\t{editable}\\n\\t\\t\\tinteractive={true}\\n\\t\\t\\t{handle_trim_audio}\\n\\t\\t\\tbind:trimDuration\\n\\t\\t\\tbind:mode\\n\\t\\t\\tshow_redo\\n\\t\\t\\t{handle_reset_value}\\n\\t\\t\\t{waveform_options}\\n\\t\\t/>\\n\\t{/if}\\n</div>\\n\\n<style>\\n\\t.microphone {\\n\\t\\twidth: 100%;\\n\\t\\tdisplay: none;\\n\\t}\\n\\n\\t.component-wrapper {\\n\\t\\tpadding: var(--size-3);\\n\\t\\twidth: 100%;\\n\\t}\\n\\n\\t.timestamps {\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-between;\\n\\t\\talign-items: center;\\n\\t\\twidth: 100%;\\n\\t\\tpadding: var(--size-1) 0;\\n\\t\\tmargin: var(--spacing-md) 0;\\n\\t}\\n\\n\\t.time {\\n\\t\\tcolor: var(--neutral-400);\\n\\t}\\n\\n\\t.duration {\\n\\t\\tcolor: var(--neutral-400);\\n\\t}\\n\\n\\t.trim-duration {\\n\\t\\tcolor: var(--color-accent);\\n\\t\\tmargin-right: var(--spacing-sm);\\n\\t}</style>\\n"],"names":[],"mappings":"AAoNC,yBAAY,CACX,KAAK,CAAE,IAAI,CACX,OAAO,CAAE,IACV,CAEA,gCAAmB,CAClB,OAAO,CAAE,IAAI,QAAQ,CAAC,CACtB,KAAK,CAAE,IACR,CAEA,yBAAY,CACX,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,aAAa,CAC9B,WAAW,CAAE,MAAM,CACnB,KAAK,CAAE,IAAI,CACX,OAAO,CAAE,IAAI,QAAQ,CAAC,CAAC,CAAC,CACxB,MAAM,CAAE,IAAI,YAAY,CAAC,CAAC,CAC3B,CAEA,mBAAM,CACL,KAAK,CAAE,IAAI,aAAa,CACzB,CAEA,uBAAU,CACT,KAAK,CAAE,IAAI,aAAa,CACzB,CAEA,4BAAe,CACd,KAAK,CAAE,IAAI,cAAc,CAAC,CAC1B,YAAY,CAAE,IAAI,YAAY,CAC/B"}'
};
const AudioRecorder = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { mode } = $$props;
  let { i18n } = $$props;
  let { dispatch_blob } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options = { show_recording_waveform: true } } = $$props;
  let { handle_reset_value } = $$props;
  let { editable = true } = $$props;
  let { recording = false } = $$props;
  let recordingContainer;
  let microphoneContainer;
  createEventDispatcher();
  if ($$props.mode === void 0 && $$bindings.mode && mode !== void 0)
    $$bindings.mode(mode);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.dispatch_blob === void 0 && $$bindings.dispatch_blob && dispatch_blob !== void 0)
    $$bindings.dispatch_blob(dispatch_blob);
  if ($$props.waveform_settings === void 0 && $$bindings.waveform_settings && waveform_settings !== void 0)
    $$bindings.waveform_settings(waveform_settings);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.recording === void 0 && $$bindings.recording && recording !== void 0)
    $$bindings.recording(recording);
  $$result.css.add(css$2);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `<div class="component-wrapper svelte-9n45fh"><div class="microphone svelte-9n45fh" data-testid="microphone-waveform"${add_attribute("this", microphoneContainer, 0)}></div> <div data-testid="recording-waveform"${add_attribute("this", recordingContainer, 0)}></div> ${``} ${``} ${``} </div>`;
  } while (!$$settled);
  return $$rendered;
});
const css$1 = {
  code: '.controls.svelte-1fz19cj{display:flex;align-items:center;justify-content:space-between;flex-wrap:wrap}.mic-wrap.svelte-1fz19cj{display:block;align-items:center;margin:var(--spacing-xl)}.icon.svelte-1fz19cj{width:var(--size-4);height:var(--size-4);fill:var(--primary-600);stroke:var(--primary-600)}.stop-button-paused.svelte-1fz19cj{display:none;height:var(--size-8);width:var(--size-20);background-color:var(--block-background-fill);border-radius:var(--button-large-radius);align-items:center;border:1px solid var(--block-border-color);margin-right:5px}.stop-button-paused.svelte-1fz19cj::before{content:"";height:var(--size-4);width:var(--size-4);border-radius:var(--radius-full);background:var(--primary-600);margin:0 var(--spacing-xl)}.stop-button.svelte-1fz19cj::before{content:"";height:var(--size-4);width:var(--size-4);border-radius:var(--radius-full);background:var(--primary-600);margin:0 var(--spacing-xl);animation:svelte-1fz19cj-scaling 1800ms infinite}.stop-button.svelte-1fz19cj{height:var(--size-8);width:var(--size-20);background-color:var(--block-background-fill);border-radius:var(--button-large-radius);align-items:center;border:1px solid var(--primary-600);margin-right:5px;display:flex}.spinner-button.svelte-1fz19cj{height:var(--size-8);width:var(--size-24);background-color:var(--block-background-fill);border-radius:var(--radius-3xl);align-items:center;border:1px solid var(--primary-600);margin:0 var(--spacing-xl);display:flex;justify-content:space-evenly}.record-button.svelte-1fz19cj::before{content:"";height:var(--size-4);width:var(--size-4);border-radius:var(--radius-full);background:var(--primary-600);margin:0 var(--spacing-xl)}.record-button.svelte-1fz19cj{height:var(--size-8);width:var(--size-24);background-color:var(--block-background-fill);border-radius:var(--button-large-radius);display:flex;align-items:center;border:1px solid var(--block-border-color)}@keyframes svelte-1fz19cj-scaling{0%{background-color:var(--primary-600);scale:1}50%{background-color:var(--primary-600);scale:1.2}100%{background-color:var(--primary-600);scale:1}}',
  map: '{"version":3,"file":"StreamAudio.svelte","sources":["StreamAudio.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onMount } from \\"svelte\\";\\nimport { Spinner } from \\"@gradio/icons\\";\\nimport WaveSurfer from \\"wavesurfer.js\\";\\nimport RecordPlugin from \\"wavesurfer.js/dist/plugins/record.js\\";\\nimport DeviceSelect from \\"../shared/DeviceSelect.svelte\\";\\nexport let recording = false;\\nexport let paused_recording = false;\\nexport let stop;\\nexport let record;\\nexport let i18n;\\nexport let waveform_settings;\\nexport let waveform_options = {\\n    show_recording_waveform: true\\n};\\nexport let waiting = false;\\nlet micWaveform;\\nlet waveformRecord;\\nlet microphoneContainer;\\nlet micDevices = [];\\nonMount(() => {\\n    create_mic_waveform();\\n});\\nconst create_mic_waveform = () => {\\n    if (micWaveform !== void 0)\\n        micWaveform.destroy();\\n    if (!microphoneContainer)\\n        return;\\n    micWaveform = WaveSurfer.create({\\n        ...waveform_settings,\\n        height: 100,\\n        container: microphoneContainer\\n    });\\n    waveformRecord = micWaveform.registerPlugin(RecordPlugin.create());\\n};\\n<\/script>\\n\\n<div class=\\"mic-wrap\\">\\n\\t{#if waveform_options.show_recording_waveform}\\n\\t\\t<div\\n\\t\\t\\tbind:this={microphoneContainer}\\n\\t\\t\\tstyle:display={recording ? \\"block\\" : \\"none\\"}\\n\\t\\t/>\\n\\t{/if}\\n\\t<div class=\\"controls\\">\\n\\t\\t{#if recording && !waiting}\\n\\t\\t\\t<button\\n\\t\\t\\t\\tclass={paused_recording ? \\"stop-button-paused\\" : \\"stop-button\\"}\\n\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\twaveformRecord?.stopMic();\\n\\t\\t\\t\\t\\tstop();\\n\\t\\t\\t\\t}}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<span class=\\"record-icon\\">\\n\\t\\t\\t\\t\\t<span class=\\"pinger\\" />\\n\\t\\t\\t\\t\\t<span class=\\"dot\\" />\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t{paused_recording ? i18n(\\"audio.pause\\") : i18n(\\"audio.stop\\")}\\n\\t\\t\\t</button>\\n\\t\\t{:else if recording && waiting}\\n\\t\\t\\t<button\\n\\t\\t\\t\\tclass=\\"spinner-button\\"\\n\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\tstop();\\n\\t\\t\\t\\t}}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<div class=\\"icon\\">\\n\\t\\t\\t\\t\\t<Spinner />\\n\\t\\t\\t\\t</div>\\n\\t\\t\\t\\t{i18n(\\"audio.waiting\\")}\\n\\t\\t\\t</button>\\n\\t\\t{:else}\\n\\t\\t\\t<button\\n\\t\\t\\t\\tclass=\\"record-button\\"\\n\\t\\t\\t\\ton:click={() => {\\n\\t\\t\\t\\t\\twaveformRecord?.startMic();\\n\\t\\t\\t\\t\\trecord();\\n\\t\\t\\t\\t}}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<span class=\\"record-icon\\">\\n\\t\\t\\t\\t\\t<span class=\\"dot\\" />\\n\\t\\t\\t\\t</span>\\n\\t\\t\\t\\t{i18n(\\"audio.record\\")}\\n\\t\\t\\t</button>\\n\\t\\t{/if}\\n\\n\\t\\t<DeviceSelect bind:micDevices {i18n} />\\n\\t</div>\\n</div>\\n\\n<style>\\n\\t.controls {\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tjustify-content: space-between;\\n\\t\\tflex-wrap: wrap;\\n\\t}\\n\\n\\t.mic-wrap {\\n\\t\\tdisplay: block;\\n\\t\\talign-items: center;\\n\\t\\tmargin: var(--spacing-xl);\\n\\t}\\n\\n\\t.icon {\\n\\t\\twidth: var(--size-4);\\n\\t\\theight: var(--size-4);\\n\\t\\tfill: var(--primary-600);\\n\\t\\tstroke: var(--primary-600);\\n\\t}\\n\\n\\t.stop-button-paused {\\n\\t\\tdisplay: none;\\n\\t\\theight: var(--size-8);\\n\\t\\twidth: var(--size-20);\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tborder-radius: var(--button-large-radius);\\n\\t\\talign-items: center;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t\\tmargin-right: 5px;\\n\\t}\\n\\n\\t.stop-button-paused::before {\\n\\t\\tcontent: \\"\\";\\n\\t\\theight: var(--size-4);\\n\\t\\twidth: var(--size-4);\\n\\t\\tborder-radius: var(--radius-full);\\n\\t\\tbackground: var(--primary-600);\\n\\t\\tmargin: 0 var(--spacing-xl);\\n\\t}\\n\\n\\t.stop-button::before {\\n\\t\\tcontent: \\"\\";\\n\\t\\theight: var(--size-4);\\n\\t\\twidth: var(--size-4);\\n\\t\\tborder-radius: var(--radius-full);\\n\\t\\tbackground: var(--primary-600);\\n\\t\\tmargin: 0 var(--spacing-xl);\\n\\t\\tanimation: scaling 1800ms infinite;\\n\\t}\\n\\n\\t.stop-button {\\n\\t\\theight: var(--size-8);\\n\\t\\twidth: var(--size-20);\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tborder-radius: var(--button-large-radius);\\n\\t\\talign-items: center;\\n\\t\\tborder: 1px solid var(--primary-600);\\n\\t\\tmargin-right: 5px;\\n\\t\\tdisplay: flex;\\n\\t}\\n\\n\\t.spinner-button {\\n\\t\\theight: var(--size-8);\\n\\t\\twidth: var(--size-24);\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tborder-radius: var(--radius-3xl);\\n\\t\\talign-items: center;\\n\\t\\tborder: 1px solid var(--primary-600);\\n\\t\\tmargin: 0 var(--spacing-xl);\\n\\t\\tdisplay: flex;\\n\\t\\tjustify-content: space-evenly;\\n\\t}\\n\\n\\t.record-button::before {\\n\\t\\tcontent: \\"\\";\\n\\t\\theight: var(--size-4);\\n\\t\\twidth: var(--size-4);\\n\\t\\tborder-radius: var(--radius-full);\\n\\t\\tbackground: var(--primary-600);\\n\\t\\tmargin: 0 var(--spacing-xl);\\n\\t}\\n\\n\\t.record-button {\\n\\t\\theight: var(--size-8);\\n\\t\\twidth: var(--size-24);\\n\\t\\tbackground-color: var(--block-background-fill);\\n\\t\\tborder-radius: var(--button-large-radius);\\n\\t\\tdisplay: flex;\\n\\t\\talign-items: center;\\n\\t\\tborder: 1px solid var(--block-border-color);\\n\\t}\\n\\n\\t@keyframes scaling {\\n\\t\\t0% {\\n\\t\\t\\tbackground-color: var(--primary-600);\\n\\t\\t\\tscale: 1;\\n\\t\\t}\\n\\t\\t50% {\\n\\t\\t\\tbackground-color: var(--primary-600);\\n\\t\\t\\tscale: 1.2;\\n\\t\\t}\\n\\t\\t100% {\\n\\t\\t\\tbackground-color: var(--primary-600);\\n\\t\\t\\tscale: 1;\\n\\t\\t}\\n\\t}</style>\\n"],"names":[],"mappings":"AA0FC,wBAAU,CACT,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,eAAe,CAAE,aAAa,CAC9B,SAAS,CAAE,IACZ,CAEA,wBAAU,CACT,OAAO,CAAE,KAAK,CACd,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,IAAI,YAAY,CACzB,CAEA,oBAAM,CACL,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,IAAI,CAAE,IAAI,aAAa,CAAC,CACxB,MAAM,CAAE,IAAI,aAAa,CAC1B,CAEA,kCAAoB,CACnB,OAAO,CAAE,IAAI,CACb,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAAC,CAC3C,YAAY,CAAE,GACf,CAEA,kCAAmB,QAAS,CAC3B,OAAO,CAAE,EAAE,CACX,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,aAAa,CAAE,IAAI,aAAa,CAAC,CACjC,UAAU,CAAE,IAAI,aAAa,CAAC,CAC9B,MAAM,CAAE,CAAC,CAAC,IAAI,YAAY,CAC3B,CAEA,2BAAY,QAAS,CACpB,OAAO,CAAE,EAAE,CACX,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,aAAa,CAAE,IAAI,aAAa,CAAC,CACjC,UAAU,CAAE,IAAI,aAAa,CAAC,CAC9B,MAAM,CAAE,CAAC,CAAC,IAAI,YAAY,CAAC,CAC3B,SAAS,CAAE,sBAAO,CAAC,MAAM,CAAC,QAC3B,CAEA,2BAAa,CACZ,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,aAAa,CAAC,CACpC,YAAY,CAAE,GAAG,CACjB,OAAO,CAAE,IACV,CAEA,8BAAgB,CACf,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,YAAY,CAAC,CAChC,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,aAAa,CAAC,CACpC,MAAM,CAAE,CAAC,CAAC,IAAI,YAAY,CAAC,CAC3B,OAAO,CAAE,IAAI,CACb,eAAe,CAAE,YAClB,CAEA,6BAAc,QAAS,CACtB,OAAO,CAAE,EAAE,CACX,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,QAAQ,CAAC,CACpB,aAAa,CAAE,IAAI,aAAa,CAAC,CACjC,UAAU,CAAE,IAAI,aAAa,CAAC,CAC9B,MAAM,CAAE,CAAC,CAAC,IAAI,YAAY,CAC3B,CAEA,6BAAe,CACd,MAAM,CAAE,IAAI,QAAQ,CAAC,CACrB,KAAK,CAAE,IAAI,SAAS,CAAC,CACrB,gBAAgB,CAAE,IAAI,uBAAuB,CAAC,CAC9C,aAAa,CAAE,IAAI,qBAAqB,CAAC,CACzC,OAAO,CAAE,IAAI,CACb,WAAW,CAAE,MAAM,CACnB,MAAM,CAAE,GAAG,CAAC,KAAK,CAAC,IAAI,oBAAoB,CAC3C,CAEA,WAAW,sBAAQ,CAClB,EAAG,CACF,gBAAgB,CAAE,IAAI,aAAa,CAAC,CACpC,KAAK,CAAE,CACR,CACA,GAAI,CACH,gBAAgB,CAAE,IAAI,aAAa,CAAC,CACpC,KAAK,CAAE,GACR,CACA,IAAK,CACJ,gBAAgB,CAAE,IAAI,aAAa,CAAC,CACpC,KAAK,CAAE,CACR,CACD"}'
};
const StreamAudio = create_ssr_component(($$result, $$props, $$bindings, slots) => {
  let { recording = false } = $$props;
  let { paused_recording = false } = $$props;
  let { stop } = $$props;
  let { record } = $$props;
  let { i18n } = $$props;
  let { waveform_settings } = $$props;
  let { waveform_options = { show_recording_waveform: true } } = $$props;
  let { waiting = false } = $$props;
  let microphoneContainer;
  let micDevices = [];
  if ($$props.recording === void 0 && $$bindings.recording && recording !== void 0)
    $$bindings.recording(recording);
  if ($$props.paused_recording === void 0 && $$bindings.paused_recording && paused_recording !== void 0)
    $$bindings.paused_recording(paused_recording);
  if ($$props.stop === void 0 && $$bindings.stop && stop !== void 0)
    $$bindings.stop(stop);
  if ($$props.record === void 0 && $$bindings.record && record !== void 0)
    $$bindings.record(record);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.waveform_settings === void 0 && $$bindings.waveform_settings && waveform_settings !== void 0)
    $$bindings.waveform_settings(waveform_settings);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.waiting === void 0 && $$bindings.waiting && waiting !== void 0)
    $$bindings.waiting(waiting);
  $$result.css.add(css$1);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    $$rendered = `<div class="mic-wrap svelte-1fz19cj">${waveform_options.show_recording_waveform ? `<div${add_styles({ "display": recording ? "block" : "none" })}${add_attribute("this", microphoneContainer, 0)}></div>` : ``} <div class="controls svelte-1fz19cj">${recording && !waiting ? `<button class="${escape(null_to_empty(paused_recording ? "stop-button-paused" : "stop-button"), true) + " svelte-1fz19cj"}"><span class="record-icon" data-svelte-h="svelte-bla7qm"><span class="pinger"></span> <span class="dot"></span></span> ${escape(paused_recording ? i18n("audio.pause") : i18n("audio.stop"))}</button>` : `${recording && waiting ? `<button class="spinner-button svelte-1fz19cj"><div class="icon svelte-1fz19cj">${validate_component(Spinner, "Spinner").$$render($$result, {}, {}, {})}</div> ${escape(i18n("audio.waiting"))}</button>` : `<button class="record-button svelte-1fz19cj"><span class="record-icon" data-svelte-h="svelte-1dwz2xe"><span class="dot"></span></span> ${escape(i18n("audio.record"))}</button>`}`} ${validate_component(DeviceSelect, "DeviceSelect").$$render(
      $$result,
      { i18n, micDevices },
      {
        micDevices: ($$value) => {
          micDevices = $$value;
          $$settled = false;
        }
      },
      {}
    )}</div> </div>`;
  } while (!$$settled);
  return $$rendered;
});
const css = {
  code: ".audio-container.svelte-cbyffp{height:calc(var(--size-full) - var(--size-6));display:flex;flex-direction:column;justify-content:space-between}",
  map: '{"version":3,"file":"InteractiveAudio.svelte","sources":["InteractiveAudio.svelte"],"sourcesContent":["<script lang=\\"ts\\">import { onDestroy, createEventDispatcher, tick } from \\"svelte\\";\\nimport { Upload, ModifyUpload } from \\"@gradio/upload\\";\\nimport { prepare_files } from \\"@gradio/client\\";\\nimport { BlockLabel } from \\"@gradio/atoms\\";\\nimport { Music } from \\"@gradio/icons\\";\\nimport { StreamingBar } from \\"@gradio/statustracker\\";\\nimport AudioPlayer from \\"../player/AudioPlayer.svelte\\";\\nimport AudioRecorder from \\"../recorder/AudioRecorder.svelte\\";\\nimport StreamAudio from \\"../streaming/StreamAudio.svelte\\";\\nimport { SelectSource } from \\"@gradio/atoms\\";\\nexport let value = null;\\nexport let label;\\nexport let root;\\nexport let loop;\\nexport let show_label = true;\\nexport let show_download_button = false;\\nexport let sources = [\\"microphone\\", \\"upload\\"];\\nexport let pending = false;\\nexport let streaming = false;\\nexport let i18n;\\nexport let waveform_settings;\\nexport let trim_region_settings = {};\\nexport let waveform_options = {};\\nexport let dragging;\\nexport let active_source;\\nexport let handle_reset_value = () => {\\n};\\nexport let editable = true;\\nexport let max_file_size = null;\\nexport let upload;\\nexport let stream_handler;\\nexport let stream_every;\\nexport let uploading = false;\\nexport let recording = false;\\nlet time_limit = null;\\nlet stream_state = \\"closed\\";\\nexport const modify_stream = (state) => {\\n    if (state === \\"closed\\") {\\n        time_limit = null;\\n        stream_state = \\"closed\\";\\n    }\\n    else if (state === \\"waiting\\") {\\n        stream_state = \\"waiting\\";\\n    }\\n    else {\\n        stream_state = \\"open\\";\\n    }\\n};\\nexport const set_time_limit = (time) => {\\n    if (recording)\\n        time_limit = time;\\n};\\n$: dispatch(\\"drag\\", dragging);\\nlet recorder;\\nlet mode = \\"\\";\\nlet header = void 0;\\nlet pending_stream = [];\\nlet submit_pending_stream_on_pending_end = false;\\nlet inited = false;\\nconst NUM_HEADER_BYTES = 44;\\nlet audio_chunks = [];\\nlet module_promises;\\nfunction get_modules() {\\n    module_promises = [\\n        import(\\"extendable-media-recorder\\"),\\n        import(\\"extendable-media-recorder-wav-encoder\\")\\n    ];\\n}\\nconst is_browser = typeof window !== \\"undefined\\";\\nif (is_browser && streaming) {\\n    get_modules();\\n}\\nconst dispatch = createEventDispatcher();\\nconst dispatch_blob = async (blobs, event) => {\\n    let _audio_blob = new File(blobs, \\"audio.wav\\");\\n    const val = await prepare_files([_audio_blob], event === \\"stream\\");\\n    value = ((await upload(val, root, void 0, max_file_size || void 0))?.filter(Boolean))[0];\\n    dispatch(event, value);\\n};\\nonDestroy(() => {\\n    if (streaming && recorder && recorder.state !== \\"inactive\\") {\\n        recorder.stop();\\n    }\\n});\\nasync function prepare_audio() {\\n    let stream;\\n    try {\\n        stream = await navigator.mediaDevices.getUserMedia({ audio: true });\\n    }\\n    catch (err) {\\n        if (!navigator.mediaDevices) {\\n            dispatch(\\"error\\", i18n(\\"audio.no_device_support\\"));\\n            return;\\n        }\\n        if (err instanceof DOMException && err.name == \\"NotAllowedError\\") {\\n            dispatch(\\"error\\", i18n(\\"audio.allow_recording_access\\"));\\n            return;\\n        }\\n        throw err;\\n    }\\n    if (stream == null)\\n        return;\\n    if (streaming) {\\n        const [{ MediaRecorder: MediaRecorder2, register }, { connect }] = await Promise.all(module_promises);\\n        await register(await connect());\\n        recorder = new MediaRecorder2(stream, { mimeType: \\"audio/wav\\" });\\n        recorder.addEventListener(\\"dataavailable\\", handle_chunk);\\n    }\\n    else {\\n        recorder = new MediaRecorder(stream);\\n        recorder.addEventListener(\\"dataavailable\\", (event) => {\\n            audio_chunks.push(event.data);\\n        });\\n    }\\n    recorder.addEventListener(\\"stop\\", async () => {\\n        recording = false;\\n        await dispatch_blob(audio_chunks, \\"change\\");\\n        await dispatch_blob(audio_chunks, \\"stop_recording\\");\\n        audio_chunks = [];\\n    });\\n    inited = true;\\n}\\nasync function handle_chunk(event) {\\n    let buffer = await event.data.arrayBuffer();\\n    let payload = new Uint8Array(buffer);\\n    if (!header) {\\n        header = new Uint8Array(buffer.slice(0, NUM_HEADER_BYTES));\\n        payload = new Uint8Array(buffer.slice(NUM_HEADER_BYTES));\\n    }\\n    if (pending) {\\n        pending_stream.push(payload);\\n    }\\n    else {\\n        let blobParts = [header].concat(pending_stream, [payload]);\\n        if (!recording || stream_state === \\"waiting\\")\\n            return;\\n        dispatch_blob(blobParts, \\"stream\\");\\n        pending_stream = [];\\n    }\\n}\\n$: if (submit_pending_stream_on_pending_end && pending === false) {\\n    submit_pending_stream_on_pending_end = false;\\n    if (header && pending_stream) {\\n        let blobParts = [header].concat(pending_stream);\\n        pending_stream = [];\\n        dispatch_blob(blobParts, \\"stream\\");\\n    }\\n}\\nasync function record() {\\n    recording = true;\\n    dispatch(\\"start_recording\\");\\n    if (!inited)\\n        await prepare_audio();\\n    header = void 0;\\n    if (streaming && recorder.state != \\"recording\\") {\\n        recorder.start(stream_every * 1e3);\\n    }\\n}\\nfunction clear() {\\n    dispatch(\\"change\\", null);\\n    dispatch(\\"clear\\");\\n    mode = \\"\\";\\n    value = null;\\n}\\nfunction handle_load({ detail }) {\\n    value = detail;\\n    dispatch(\\"change\\", detail);\\n    dispatch(\\"upload\\", detail);\\n}\\nasync function stop() {\\n    recording = false;\\n    if (streaming) {\\n        dispatch(\\"close_stream\\");\\n        dispatch(\\"stop_recording\\");\\n        recorder.stop();\\n        if (pending) {\\n            submit_pending_stream_on_pending_end = true;\\n        }\\n        dispatch_blob(audio_chunks, \\"stop_recording\\");\\n        dispatch(\\"clear\\");\\n        mode = \\"\\";\\n    }\\n}\\n$: if (!recording && recorder)\\n    stop();\\n$: if (recording && recorder)\\n    record();\\n<\/script>\\n\\n<BlockLabel\\n\\t{show_label}\\n\\tIcon={Music}\\n\\tfloat={active_source === \\"upload\\" && value === null}\\n\\tlabel={label || i18n(\\"audio.audio\\")}\\n/>\\n<div class=\\"audio-container\\">\\n\\t<StreamingBar {time_limit} />\\n\\t{#if value === null || streaming}\\n\\t\\t{#if active_source === \\"microphone\\"}\\n\\t\\t\\t<ModifyUpload {i18n} on:clear={clear} />\\n\\t\\t\\t{#if streaming}\\n\\t\\t\\t\\t<StreamAudio\\n\\t\\t\\t\\t\\t{record}\\n\\t\\t\\t\\t\\t{recording}\\n\\t\\t\\t\\t\\t{stop}\\n\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t{waveform_settings}\\n\\t\\t\\t\\t\\t{waveform_options}\\n\\t\\t\\t\\t\\twaiting={stream_state === \\"waiting\\"}\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{:else}\\n\\t\\t\\t\\t<AudioRecorder\\n\\t\\t\\t\\t\\tbind:mode\\n\\t\\t\\t\\t\\t{i18n}\\n\\t\\t\\t\\t\\t{editable}\\n\\t\\t\\t\\t\\t{recording}\\n\\t\\t\\t\\t\\t{dispatch_blob}\\n\\t\\t\\t\\t\\t{waveform_settings}\\n\\t\\t\\t\\t\\t{waveform_options}\\n\\t\\t\\t\\t\\t{handle_reset_value}\\n\\t\\t\\t\\t\\ton:start_recording\\n\\t\\t\\t\\t\\ton:pause_recording\\n\\t\\t\\t\\t\\ton:stop_recording\\n\\t\\t\\t\\t/>\\n\\t\\t\\t{/if}\\n\\t\\t{:else if active_source === \\"upload\\"}\\n\\t\\t\\t<!-- explicitly listed out audio mimetypes due to iOS bug not recognizing audio/* -->\\n\\t\\t\\t<Upload\\n\\t\\t\\t\\tfiletype=\\"audio/aac,audio/midi,audio/mpeg,audio/ogg,audio/wav,audio/x-wav,audio/opus,audio/webm,audio/flac,audio/vnd.rn-realaudio,audio/x-ms-wma,audio/x-aiff,audio/amr,audio/*\\"\\n\\t\\t\\t\\ton:load={handle_load}\\n\\t\\t\\t\\tbind:dragging\\n\\t\\t\\t\\tbind:uploading\\n\\t\\t\\t\\ton:error={({ detail }) => dispatch(\\"error\\", detail)}\\n\\t\\t\\t\\t{root}\\n\\t\\t\\t\\t{max_file_size}\\n\\t\\t\\t\\t{upload}\\n\\t\\t\\t\\t{stream_handler}\\n\\t\\t\\t>\\n\\t\\t\\t\\t<slot />\\n\\t\\t\\t</Upload>\\n\\t\\t{/if}\\n\\t{:else}\\n\\t\\t<ModifyUpload\\n\\t\\t\\t{i18n}\\n\\t\\t\\ton:clear={clear}\\n\\t\\t\\ton:edit={() => (mode = \\"edit\\")}\\n\\t\\t\\tdownload={show_download_button ? value.url : null}\\n\\t\\t/>\\n\\n\\t\\t<AudioPlayer\\n\\t\\t\\tbind:mode\\n\\t\\t\\t{value}\\n\\t\\t\\t{label}\\n\\t\\t\\t{i18n}\\n\\t\\t\\t{dispatch_blob}\\n\\t\\t\\t{waveform_settings}\\n\\t\\t\\t{waveform_options}\\n\\t\\t\\t{trim_region_settings}\\n\\t\\t\\t{handle_reset_value}\\n\\t\\t\\t{editable}\\n\\t\\t\\t{loop}\\n\\t\\t\\tinteractive\\n\\t\\t\\ton:stop\\n\\t\\t\\ton:play\\n\\t\\t\\ton:pause\\n\\t\\t\\ton:edit\\n\\t\\t/>\\n\\t{/if}\\n\\t<SelectSource {sources} bind:active_source handle_clear={clear} />\\n</div>\\n\\n<style>\\n\\t.audio-container {\\n\\t\\theight: calc(var(--size-full) - var(--size-6));\\n\\t\\tdisplay: flex;\\n\\t\\tflex-direction: column;\\n\\t\\tjustify-content: space-between;\\n\\t}</style>\\n"],"names":[],"mappings":"AAgRC,8BAAiB,CAChB,MAAM,CAAE,KAAK,IAAI,WAAW,CAAC,CAAC,CAAC,CAAC,IAAI,QAAQ,CAAC,CAAC,CAC9C,OAAO,CAAE,IAAI,CACb,cAAc,CAAE,MAAM,CACtB,eAAe,CAAE,aAClB"}'
};
const NUM_HEADER_BYTES = 44;
const InteractiveAudio = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
      time_limit = null;
      stream_state = "closed";
    } else if (state === "waiting") {
      stream_state = "waiting";
    } else {
      stream_state = "open";
    }
  };
  const set_time_limit = (time) => {
    if (recording)
      time_limit = time;
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
      import('./module3-DvZwOF3D.js'),
      import('./module-ooWiuM7-.js')
    ];
  }
  const is_browser = typeof window !== "undefined";
  if (is_browser && streaming) {
    get_modules();
  }
  const dispatch = createEventDispatcher();
  const dispatch_blob = async (blobs, event) => {
    let _audio_blob = new File(blobs, "audio.wav");
    const val = await prepare_files([_audio_blob], event === "stream");
    value = (await upload(val, root, void 0, max_file_size || void 0))?.filter(Boolean)[0];
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
      recorder = new MediaRecorder2(stream, { mimeType: "audio/wav" });
      recorder.addEventListener("dataavailable", handle_chunk);
    } else {
      recorder = new MediaRecorder(stream);
      recorder.addEventListener("dataavailable", (event) => {
        audio_chunks.push(event.data);
      });
    }
    recorder.addEventListener("stop", async () => {
      recording = false;
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
      header = new Uint8Array(buffer.slice(0, NUM_HEADER_BYTES));
      payload = new Uint8Array(buffer.slice(NUM_HEADER_BYTES));
    }
    if (pending) {
      pending_stream.push(payload);
    } else {
      let blobParts = [header].concat(pending_stream, [payload]);
      if (!recording || stream_state === "waiting")
        return;
      dispatch_blob(blobParts, "stream");
      pending_stream = [];
    }
  }
  async function record() {
    recording = true;
    dispatch("start_recording");
    if (!inited)
      await prepare_audio();
    header = void 0;
    if (streaming && recorder.state != "recording") {
      recorder.start(stream_every * 1e3);
    }
  }
  function clear() {
    dispatch("change", null);
    dispatch("clear");
    mode = "";
    value = null;
  }
  async function stop() {
    recording = false;
    if (streaming) {
      dispatch("close_stream");
      dispatch("stop_recording");
      recorder.stop();
      if (pending) {
        submit_pending_stream_on_pending_end = true;
      }
      dispatch_blob(audio_chunks, "stop_recording");
      dispatch("clear");
      mode = "";
    }
  }
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.pending === void 0 && $$bindings.pending && pending !== void 0)
    $$bindings.pending(pending);
  if ($$props.streaming === void 0 && $$bindings.streaming && streaming !== void 0)
    $$bindings.streaming(streaming);
  if ($$props.i18n === void 0 && $$bindings.i18n && i18n !== void 0)
    $$bindings.i18n(i18n);
  if ($$props.waveform_settings === void 0 && $$bindings.waveform_settings && waveform_settings !== void 0)
    $$bindings.waveform_settings(waveform_settings);
  if ($$props.trim_region_settings === void 0 && $$bindings.trim_region_settings && trim_region_settings !== void 0)
    $$bindings.trim_region_settings(trim_region_settings);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.dragging === void 0 && $$bindings.dragging && dragging !== void 0)
    $$bindings.dragging(dragging);
  if ($$props.active_source === void 0 && $$bindings.active_source && active_source !== void 0)
    $$bindings.active_source(active_source);
  if ($$props.handle_reset_value === void 0 && $$bindings.handle_reset_value && handle_reset_value !== void 0)
    $$bindings.handle_reset_value(handle_reset_value);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.max_file_size === void 0 && $$bindings.max_file_size && max_file_size !== void 0)
    $$bindings.max_file_size(max_file_size);
  if ($$props.upload === void 0 && $$bindings.upload && upload !== void 0)
    $$bindings.upload(upload);
  if ($$props.stream_handler === void 0 && $$bindings.stream_handler && stream_handler !== void 0)
    $$bindings.stream_handler(stream_handler);
  if ($$props.stream_every === void 0 && $$bindings.stream_every && stream_every !== void 0)
    $$bindings.stream_every(stream_every);
  if ($$props.uploading === void 0 && $$bindings.uploading && uploading !== void 0)
    $$bindings.uploading(uploading);
  if ($$props.recording === void 0 && $$bindings.recording && recording !== void 0)
    $$bindings.recording(recording);
  if ($$props.modify_stream === void 0 && $$bindings.modify_stream && modify_stream !== void 0)
    $$bindings.modify_stream(modify_stream);
  if ($$props.set_time_limit === void 0 && $$bindings.set_time_limit && set_time_limit !== void 0)
    $$bindings.set_time_limit(set_time_limit);
  $$result.css.add(css);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    {
      dispatch("drag", dragging);
    }
    {
      if (submit_pending_stream_on_pending_end && pending === false) {
        submit_pending_stream_on_pending_end = false;
        if (header && pending_stream) {
          let blobParts = [header].concat(pending_stream);
          pending_stream = [];
          dispatch_blob(blobParts, "stream");
        }
      }
    }
    {
      if (!recording && recorder)
        stop();
    }
    {
      if (recording && recorder)
        record();
    }
    $$rendered = `${validate_component(BlockLabel, "BlockLabel").$$render(
      $$result,
      {
        show_label,
        Icon: Music,
        float: active_source === "upload" && value === null,
        label: label || i18n("audio.audio")
      },
      {},
      {}
    )} <div class="audio-container svelte-cbyffp">${validate_component(StreamingBar, "StreamingBar").$$render($$result, { time_limit }, {}, {})} ${value === null || streaming ? `${active_source === "microphone" ? `${validate_component(ModifyUpload, "ModifyUpload").$$render($$result, { i18n }, {}, {})} ${streaming ? `${validate_component(StreamAudio, "StreamAudio").$$render(
      $$result,
      {
        record,
        recording,
        stop,
        i18n,
        waveform_settings,
        waveform_options,
        waiting: stream_state === "waiting"
      },
      {},
      {}
    )}` : `${validate_component(AudioRecorder, "AudioRecorder").$$render(
      $$result,
      {
        i18n,
        editable,
        recording,
        dispatch_blob,
        waveform_settings,
        waveform_options,
        handle_reset_value,
        mode
      },
      {
        mode: ($$value) => {
          mode = $$value;
          $$settled = false;
        }
      },
      {}
    )}`}` : `${active_source === "upload" ? ` ${validate_component(Upload, "Upload").$$render(
      $$result,
      {
        filetype: "audio/aac,audio/midi,audio/mpeg,audio/ogg,audio/wav,audio/x-wav,audio/opus,audio/webm,audio/flac,audio/vnd.rn-realaudio,audio/x-ms-wma,audio/x-aiff,audio/amr,audio/*",
        root,
        max_file_size,
        upload,
        stream_handler,
        dragging,
        uploading
      },
      {
        dragging: ($$value) => {
          dragging = $$value;
          $$settled = false;
        },
        uploading: ($$value) => {
          uploading = $$value;
          $$settled = false;
        }
      },
      {
        default: () => {
          return `${slots.default ? slots.default({}) : ``}`;
        }
      }
    )}` : ``}`}` : `${validate_component(ModifyUpload, "ModifyUpload").$$render(
      $$result,
      {
        i18n,
        download: show_download_button ? value.url : null
      },
      {},
      {}
    )} ${validate_component(AudioPlayer$1, "AudioPlayer").$$render(
      $$result,
      {
        value,
        label,
        i18n,
        dispatch_blob,
        waveform_settings,
        waveform_options,
        trim_region_settings,
        handle_reset_value,
        editable,
        loop,
        interactive: true,
        mode
      },
      {
        mode: ($$value) => {
          mode = $$value;
          $$settled = false;
        }
      },
      {}
    )}`} ${validate_component(SelectSource, "SelectSource").$$render(
      $$result,
      {
        sources,
        handle_clear: clear,
        active_source
      },
      {
        active_source: ($$value) => {
          active_source = $$value;
          $$settled = false;
        }
      },
      {}
    )} </div>`;
  } while (!$$settled);
  return $$rendered;
});
const InteractiveAudio$1 = InteractiveAudio;
const Index = create_ssr_component(($$result, $$props, $$bindings, slots) => {
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
    value = initial_value;
  };
  let dragging;
  let waveform_settings;
  const trim_region_settings = {
    color: waveform_options.trim_region_color,
    drag: true,
    resize: true
  };
  if ($$props.value_is_output === void 0 && $$bindings.value_is_output && value_is_output !== void 0)
    $$bindings.value_is_output(value_is_output);
  if ($$props.elem_id === void 0 && $$bindings.elem_id && elem_id !== void 0)
    $$bindings.elem_id(elem_id);
  if ($$props.elem_classes === void 0 && $$bindings.elem_classes && elem_classes !== void 0)
    $$bindings.elem_classes(elem_classes);
  if ($$props.visible === void 0 && $$bindings.visible && visible !== void 0)
    $$bindings.visible(visible);
  if ($$props.interactive === void 0 && $$bindings.interactive && interactive !== void 0)
    $$bindings.interactive(interactive);
  if ($$props.value === void 0 && $$bindings.value && value !== void 0)
    $$bindings.value(value);
  if ($$props.sources === void 0 && $$bindings.sources && sources !== void 0)
    $$bindings.sources(sources);
  if ($$props.label === void 0 && $$bindings.label && label !== void 0)
    $$bindings.label(label);
  if ($$props.root === void 0 && $$bindings.root && root !== void 0)
    $$bindings.root(root);
  if ($$props.show_label === void 0 && $$bindings.show_label && show_label !== void 0)
    $$bindings.show_label(show_label);
  if ($$props.container === void 0 && $$bindings.container && container !== void 0)
    $$bindings.container(container);
  if ($$props.scale === void 0 && $$bindings.scale && scale !== void 0)
    $$bindings.scale(scale);
  if ($$props.min_width === void 0 && $$bindings.min_width && min_width !== void 0)
    $$bindings.min_width(min_width);
  if ($$props.loading_status === void 0 && $$bindings.loading_status && loading_status !== void 0)
    $$bindings.loading_status(loading_status);
  if ($$props.autoplay === void 0 && $$bindings.autoplay && autoplay !== void 0)
    $$bindings.autoplay(autoplay);
  if ($$props.loop === void 0 && $$bindings.loop && loop !== void 0)
    $$bindings.loop(loop);
  if ($$props.show_download_button === void 0 && $$bindings.show_download_button && show_download_button !== void 0)
    $$bindings.show_download_button(show_download_button);
  if ($$props.show_share_button === void 0 && $$bindings.show_share_button && show_share_button !== void 0)
    $$bindings.show_share_button(show_share_button);
  if ($$props.editable === void 0 && $$bindings.editable && editable !== void 0)
    $$bindings.editable(editable);
  if ($$props.waveform_options === void 0 && $$bindings.waveform_options && waveform_options !== void 0)
    $$bindings.waveform_options(waveform_options);
  if ($$props.pending === void 0 && $$bindings.pending && pending !== void 0)
    $$bindings.pending(pending);
  if ($$props.streaming === void 0 && $$bindings.streaming && streaming !== void 0)
    $$bindings.streaming(streaming);
  if ($$props.stream_every === void 0 && $$bindings.stream_every && stream_every !== void 0)
    $$bindings.stream_every(stream_every);
  if ($$props.input_ready === void 0 && $$bindings.input_ready && input_ready !== void 0)
    $$bindings.input_ready(input_ready);
  if ($$props.recording === void 0 && $$bindings.recording && recording !== void 0)
    $$bindings.recording(recording);
  if ($$props.modify_stream_state === void 0 && $$bindings.modify_stream_state && modify_stream_state !== void 0)
    $$bindings.modify_stream_state(modify_stream_state);
  if ($$props.get_stream_state === void 0 && $$bindings.get_stream_state && get_stream_state !== void 0)
    $$bindings.get_stream_state(get_stream_state);
  if ($$props.set_time_limit === void 0 && $$bindings.set_time_limit && set_time_limit !== void 0)
    $$bindings.set_time_limit(set_time_limit);
  if ($$props.gradio === void 0 && $$bindings.gradio && gradio !== void 0)
    $$bindings.gradio(gradio);
  let $$settled;
  let $$rendered;
  let previous_head = $$result.head;
  do {
    $$settled = true;
    $$result.head = previous_head;
    input_ready = !uploading;
    {
      if (value && initial_value === null) {
        initial_value = value;
      }
    }
    {
      {
        if (JSON.stringify(value) !== JSON.stringify(old_value)) {
          old_value = value;
          gradio.dispatch("change");
          if (!value_is_output) {
            gradio.dispatch("input");
          }
        }
      }
    }
    {
      if (!active_source && sources) {
        active_source = sources[0];
      }
    }
    waveform_settings = {
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
    };
    $$rendered = `  ${!interactive ? `${validate_component(Block, "Block").$$render(
      $$result,
      {
        variant: "solid",
        border_mode: dragging ? "focus" : "base",
        padding: false,
        allow_overflow: false,
        elem_id,
        elem_classes,
        visible,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(StaticAudio, "StaticAudio").$$render(
            $$result,
            {
              i18n: gradio.i18n,
              show_label,
              show_download_button,
              show_share_button,
              value,
              label,
              loop,
              waveform_settings,
              waveform_options,
              editable
            },
            {},
            {}
          )}`;
        }
      }
    )}` : `${validate_component(Block, "Block").$$render(
      $$result,
      {
        variant: value === null && active_source === "upload" ? "dashed" : "solid",
        border_mode: dragging ? "focus" : "base",
        padding: false,
        allow_overflow: false,
        elem_id,
        elem_classes,
        visible,
        container,
        scale,
        min_width
      },
      {},
      {
        default: () => {
          return `${validate_component(Static, "StatusTracker").$$render($$result, Object.assign({}, { autoscroll: gradio.autoscroll }, { i18n: gradio.i18n }, loading_status), {}, {})} ${validate_component(InteractiveAudio$1, "InteractiveAudio").$$render(
            $$result,
            {
              label,
              show_label,
              show_download_button,
              value,
              root,
              sources,
              active_source,
              pending,
              streaming,
              loop,
              max_file_size: gradio.max_file_size,
              handle_reset_value,
              editable,
              i18n: gradio.i18n,
              waveform_settings,
              waveform_options,
              trim_region_settings,
              stream_every,
              upload: (...args) => gradio.client.upload(...args),
              stream_handler: (...args) => gradio.client.stream(...args),
              recording,
              dragging,
              uploading,
              modify_stream: _modify_stream,
              set_time_limit
            },
            {
              recording: ($$value) => {
                recording = $$value;
                $$settled = false;
              },
              dragging: ($$value) => {
                dragging = $$value;
                $$settled = false;
              },
              uploading: ($$value) => {
                uploading = $$value;
                $$settled = false;
              },
              modify_stream: ($$value) => {
                _modify_stream = $$value;
                $$settled = false;
              },
              set_time_limit: ($$value) => {
                set_time_limit = $$value;
                $$settled = false;
              }
            },
            {
              default: () => {
                return `${validate_component(UploadText, "UploadText").$$render($$result, { i18n: gradio.i18n, type: "audio" }, {}, {})}`;
              }
            }
          )}`;
        }
      }
    )}`}`;
  } while (!$$settled);
  return $$rendered;
});
const Index$1 = Index;

export { InteractiveAudio$1 as BaseInteractiveAudio, AudioPlayer$1 as BasePlayer, StaticAudio as BaseStaticAudio, Index$1 as default };
//# sourceMappingURL=index55-Cw_Qj2k9.js.map
