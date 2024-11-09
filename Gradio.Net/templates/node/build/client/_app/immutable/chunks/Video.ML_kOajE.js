var __defProp = Object.defineProperty;
var __defNormalProp = (obj, key, value) => key in obj ? __defProp(obj, key, { enumerable: true, configurable: true, writable: true, value }) : obj[key] = value;
var __publicField = (obj, key, value) => {
  __defNormalProp(obj, typeof key !== "symbol" ? key + "" : key, value);
  return value;
};
var __accessCheck = (obj, member, msg) => {
  if (!member.has(obj))
    throw TypeError("Cannot " + msg);
};
var __privateGet = (obj, member, getter) => {
  __accessCheck(obj, member, "read from private field");
  return getter ? getter.call(obj) : member.get(obj);
};
var __privateAdd = (obj, member, value) => {
  if (member.has(obj))
    throw TypeError("Cannot add the same private member more than once");
  member instanceof WeakSet ? member.add(obj) : member.set(obj, value);
};
var __privateSet = (obj, member, value, setter) => {
  __accessCheck(obj, member, "write to private field");
  setter ? setter.call(obj, value) : member.set(obj, value);
  return value;
};
var _worker, _resolves, _rejects, _logEventCallbacks, _progressEventCallbacks, _registerHandlers, _send;
import { SvelteComponent, init, safe_not_equal, create_slot, element, space, claim_element, get_svelte_dataset, claim_space, children, detach, attr, toggle_class, src_url_equal, add_render_callback, insert_hydration, listen, action_destroyer, update_slot_base, get_all_dirty_from_scope, get_slot_changes, is_function, transition_in, transition_out, run_all, createEventDispatcher, assign, exclude_internal_props, raf, bubble, binding_callbacks } from "../../../svelte/svelte.js";
import "../../../svelte/svelte-submodules.js";
import { r as resolve_wasm_src } from "./file-url.Bs-FMz4v.js";
/* empty css                                                    */
import { H as Hls } from "./hls.CFPBCiRi.js";
const ERROR_RESPONSE_BODY_READER = new Error("failed to get response body reader");
const ERROR_INCOMPLETED_DOWNLOAD = new Error("failed to complete download");
const HeaderContentLength = "Content-Length";
const downloadWithProgress = async (url, cb) => {
  var _a;
  const resp = await fetch(url);
  let buf;
  try {
    const total = parseInt(resp.headers.get(HeaderContentLength) || "-1");
    const reader = (_a = resp.body) == null ? void 0 : _a.getReader();
    if (!reader)
      throw ERROR_RESPONSE_BODY_READER;
    const chunks = [];
    let received = 0;
    for (; ; ) {
      const { done, value } = await reader.read();
      const delta = value ? value.length : 0;
      if (done) {
        if (total != -1 && total !== received)
          throw ERROR_INCOMPLETED_DOWNLOAD;
        cb && cb({ url, total, received, delta, done });
        break;
      }
      chunks.push(value);
      received += delta;
      cb && cb({ url, total, received, delta, done });
    }
    const data = new Uint8Array(received);
    let position = 0;
    for (const chunk of chunks) {
      data.set(chunk, position);
      position += chunk.length;
    }
    buf = data.buffer;
  } catch (e) {
    console.log(`failed to send download progress event: `, e);
    buf = await resp.arrayBuffer();
  }
  return buf;
};
const toBlobURL = async (url, mimeType, progress = false, cb) => {
  const buf = progress ? await downloadWithProgress(url, cb) : await (await fetch(url)).arrayBuffer();
  const blob = new Blob([buf], { type: mimeType });
  return URL.createObjectURL(blob);
};
var FFMessageType;
(function(FFMessageType2) {
  FFMessageType2["LOAD"] = "LOAD";
  FFMessageType2["EXEC"] = "EXEC";
  FFMessageType2["WRITE_FILE"] = "WRITE_FILE";
  FFMessageType2["READ_FILE"] = "READ_FILE";
  FFMessageType2["DELETE_FILE"] = "DELETE_FILE";
  FFMessageType2["RENAME"] = "RENAME";
  FFMessageType2["CREATE_DIR"] = "CREATE_DIR";
  FFMessageType2["LIST_DIR"] = "LIST_DIR";
  FFMessageType2["DELETE_DIR"] = "DELETE_DIR";
  FFMessageType2["ERROR"] = "ERROR";
  FFMessageType2["DOWNLOAD"] = "DOWNLOAD";
  FFMessageType2["PROGRESS"] = "PROGRESS";
  FFMessageType2["LOG"] = "LOG";
  FFMessageType2["MOUNT"] = "MOUNT";
  FFMessageType2["UNMOUNT"] = "UNMOUNT";
})(FFMessageType || (FFMessageType = {}));
const getMessageID = /* @__PURE__ */ (() => {
  let messageID = 0;
  return () => messageID++;
})();
const ERROR_NOT_LOADED = new Error("ffmpeg is not loaded, call `await ffmpeg.load()` first");
const ERROR_TERMINATED = new Error("called FFmpeg.terminate()");
class FFmpeg {
  constructor() {
    __privateAdd(this, _worker, null);
    /**
     * #resolves and #rejects tracks Promise resolves and rejects to
     * be called when we receive message from web worker.
     */
    __privateAdd(this, _resolves, {});
    __privateAdd(this, _rejects, {});
    __privateAdd(this, _logEventCallbacks, []);
    __privateAdd(this, _progressEventCallbacks, []);
    __publicField(this, "loaded", false);
    /**
     * register worker message event handlers.
     */
    __privateAdd(this, _registerHandlers, () => {
      if (__privateGet(this, _worker)) {
        __privateGet(this, _worker).onmessage = ({ data: { id, type, data } }) => {
          switch (type) {
            case FFMessageType.LOAD:
              this.loaded = true;
              __privateGet(this, _resolves)[id](data);
              break;
            case FFMessageType.MOUNT:
            case FFMessageType.UNMOUNT:
            case FFMessageType.EXEC:
            case FFMessageType.WRITE_FILE:
            case FFMessageType.READ_FILE:
            case FFMessageType.DELETE_FILE:
            case FFMessageType.RENAME:
            case FFMessageType.CREATE_DIR:
            case FFMessageType.LIST_DIR:
            case FFMessageType.DELETE_DIR:
              __privateGet(this, _resolves)[id](data);
              break;
            case FFMessageType.LOG:
              __privateGet(this, _logEventCallbacks).forEach((f) => f(data));
              break;
            case FFMessageType.PROGRESS:
              __privateGet(this, _progressEventCallbacks).forEach((f) => f(data));
              break;
            case FFMessageType.ERROR:
              __privateGet(this, _rejects)[id](data);
              break;
          }
          delete __privateGet(this, _resolves)[id];
          delete __privateGet(this, _rejects)[id];
        };
      }
    });
    /**
     * Generic function to send messages to web worker.
     */
    __privateAdd(this, _send, ({ type, data }, trans = [], signal) => {
      if (!__privateGet(this, _worker)) {
        return Promise.reject(ERROR_NOT_LOADED);
      }
      return new Promise((resolve, reject) => {
        const id = getMessageID();
        __privateGet(this, _worker) && __privateGet(this, _worker).postMessage({ id, type, data }, trans);
        __privateGet(this, _resolves)[id] = resolve;
        __privateGet(this, _rejects)[id] = reject;
        signal == null ? void 0 : signal.addEventListener("abort", () => {
          reject(new DOMException(`Message # ${id} was aborted`, "AbortError"));
        }, { once: true });
      });
    });
    /**
     * Loads ffmpeg-core inside web worker. It is required to call this method first
     * as it initializes WebAssembly and other essential variables.
     *
     * @category FFmpeg
     * @returns `true` if ffmpeg core is loaded for the first time.
     */
    __publicField(this, "load", (config = {}, { signal } = {}) => {
      if (!__privateGet(this, _worker)) {
        __privateSet(this, _worker, new Worker(new URL("" + new URL("../workers/worker-wLcBAjZt.js", import.meta.url).href, import.meta.url), {
          type: "module"
        }));
        __privateGet(this, _registerHandlers).call(this);
      }
      return __privateGet(this, _send).call(this, {
        type: FFMessageType.LOAD,
        data: config
      }, void 0, signal);
    });
    /**
     * Execute ffmpeg command.
     *
     * @remarks
     * To avoid common I/O issues, ["-nostdin", "-y"] are prepended to the args
     * by default.
     *
     * @example
     * ```ts
     * const ffmpeg = new FFmpeg();
     * await ffmpeg.load();
     * await ffmpeg.writeFile("video.avi", ...);
     * // ffmpeg -i video.avi video.mp4
     * await ffmpeg.exec(["-i", "video.avi", "video.mp4"]);
     * const data = ffmpeg.readFile("video.mp4");
     * ```
     *
     * @returns `0` if no error, `!= 0` if timeout (1) or error.
     * @category FFmpeg
     */
    __publicField(this, "exec", (args, timeout = -1, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.EXEC,
      data: { args, timeout }
    }, void 0, signal));
    /**
     * Terminate all ongoing API calls and terminate web worker.
     * `FFmpeg.load()` must be called again before calling any other APIs.
     *
     * @category FFmpeg
     */
    __publicField(this, "terminate", () => {
      const ids = Object.keys(__privateGet(this, _rejects));
      for (const id of ids) {
        __privateGet(this, _rejects)[id](ERROR_TERMINATED);
        delete __privateGet(this, _rejects)[id];
        delete __privateGet(this, _resolves)[id];
      }
      if (__privateGet(this, _worker)) {
        __privateGet(this, _worker).terminate();
        __privateSet(this, _worker, null);
        this.loaded = false;
      }
    });
    /**
     * Write data to ffmpeg.wasm.
     *
     * @example
     * ```ts
     * const ffmpeg = new FFmpeg();
     * await ffmpeg.load();
     * await ffmpeg.writeFile("video.avi", await fetchFile("../video.avi"));
     * await ffmpeg.writeFile("text.txt", "hello world");
     * ```
     *
     * @category File System
     */
    __publicField(this, "writeFile", (path, data, { signal } = {}) => {
      const trans = [];
      if (data instanceof Uint8Array) {
        trans.push(data.buffer);
      }
      return __privateGet(this, _send).call(this, {
        type: FFMessageType.WRITE_FILE,
        data: { path, data }
      }, trans, signal);
    });
    __publicField(this, "mount", (fsType, options, mountPoint) => {
      const trans = [];
      return __privateGet(this, _send).call(this, {
        type: FFMessageType.MOUNT,
        data: { fsType, options, mountPoint }
      }, trans);
    });
    __publicField(this, "unmount", (mountPoint) => {
      const trans = [];
      return __privateGet(this, _send).call(this, {
        type: FFMessageType.UNMOUNT,
        data: { mountPoint }
      }, trans);
    });
    /**
     * Read data from ffmpeg.wasm.
     *
     * @example
     * ```ts
     * const ffmpeg = new FFmpeg();
     * await ffmpeg.load();
     * const data = await ffmpeg.readFile("video.mp4");
     * ```
     *
     * @category File System
     */
    __publicField(this, "readFile", (path, encoding = "binary", { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.READ_FILE,
      data: { path, encoding }
    }, void 0, signal));
    /**
     * Delete a file.
     *
     * @category File System
     */
    __publicField(this, "deleteFile", (path, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.DELETE_FILE,
      data: { path }
    }, void 0, signal));
    /**
     * Rename a file or directory.
     *
     * @category File System
     */
    __publicField(this, "rename", (oldPath, newPath, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.RENAME,
      data: { oldPath, newPath }
    }, void 0, signal));
    /**
     * Create a directory.
     *
     * @category File System
     */
    __publicField(this, "createDir", (path, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.CREATE_DIR,
      data: { path }
    }, void 0, signal));
    /**
     * List directory contents.
     *
     * @category File System
     */
    __publicField(this, "listDir", (path, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.LIST_DIR,
      data: { path }
    }, void 0, signal));
    /**
     * Delete an empty directory.
     *
     * @category File System
     */
    __publicField(this, "deleteDir", (path, { signal } = {}) => __privateGet(this, _send).call(this, {
      type: FFMessageType.DELETE_DIR,
      data: { path }
    }, void 0, signal));
  }
  on(event, callback) {
    if (event === "log") {
      __privateGet(this, _logEventCallbacks).push(callback);
    } else if (event === "progress") {
      __privateGet(this, _progressEventCallbacks).push(callback);
    }
  }
  off(event, callback) {
    if (event === "log") {
      __privateSet(this, _logEventCallbacks, __privateGet(this, _logEventCallbacks).filter((f) => f !== callback));
    } else if (event === "progress") {
      __privateSet(this, _progressEventCallbacks, __privateGet(this, _progressEventCallbacks).filter((f) => f !== callback));
    }
  }
}
_worker = new WeakMap();
_resolves = new WeakMap();
_rejects = new WeakMap();
_logEventCallbacks = new WeakMap();
_progressEventCallbacks = new WeakMap();
_registerHandlers = new WeakMap();
_send = new WeakMap();
const mimes = {
  "3g2": "video/3gpp2",
  "3gp": "video/3gpp",
  "3gpp": "video/3gpp",
  "3mf": "model/3mf",
  "aac": "audio/aac",
  "ac": "application/pkix-attr-cert",
  "adp": "audio/adpcm",
  "adts": "audio/aac",
  "ai": "application/postscript",
  "aml": "application/automationml-aml+xml",
  "amlx": "application/automationml-amlx+zip",
  "amr": "audio/amr",
  "apng": "image/apng",
  "appcache": "text/cache-manifest",
  "appinstaller": "application/appinstaller",
  "appx": "application/appx",
  "appxbundle": "application/appxbundle",
  "asc": "application/pgp-keys",
  "atom": "application/atom+xml",
  "atomcat": "application/atomcat+xml",
  "atomdeleted": "application/atomdeleted+xml",
  "atomsvc": "application/atomsvc+xml",
  "au": "audio/basic",
  "avci": "image/avci",
  "avcs": "image/avcs",
  "avif": "image/avif",
  "aw": "application/applixware",
  "bdoc": "application/bdoc",
  "bin": "application/octet-stream",
  "bmp": "image/bmp",
  "bpk": "application/octet-stream",
  "btf": "image/prs.btif",
  "btif": "image/prs.btif",
  "buffer": "application/octet-stream",
  "ccxml": "application/ccxml+xml",
  "cdfx": "application/cdfx+xml",
  "cdmia": "application/cdmi-capability",
  "cdmic": "application/cdmi-container",
  "cdmid": "application/cdmi-domain",
  "cdmio": "application/cdmi-object",
  "cdmiq": "application/cdmi-queue",
  "cer": "application/pkix-cert",
  "cgm": "image/cgm",
  "cjs": "application/node",
  "class": "application/java-vm",
  "coffee": "text/coffeescript",
  "conf": "text/plain",
  "cpl": "application/cpl+xml",
  "cpt": "application/mac-compactpro",
  "crl": "application/pkix-crl",
  "css": "text/css",
  "csv": "text/csv",
  "cu": "application/cu-seeme",
  "cwl": "application/cwl",
  "cww": "application/prs.cww",
  "davmount": "application/davmount+xml",
  "dbk": "application/docbook+xml",
  "deb": "application/octet-stream",
  "def": "text/plain",
  "deploy": "application/octet-stream",
  "dib": "image/bmp",
  "disposition-notification": "message/disposition-notification",
  "dist": "application/octet-stream",
  "distz": "application/octet-stream",
  "dll": "application/octet-stream",
  "dmg": "application/octet-stream",
  "dms": "application/octet-stream",
  "doc": "application/msword",
  "dot": "application/msword",
  "dpx": "image/dpx",
  "drle": "image/dicom-rle",
  "dsc": "text/prs.lines.tag",
  "dssc": "application/dssc+der",
  "dtd": "application/xml-dtd",
  "dump": "application/octet-stream",
  "dwd": "application/atsc-dwd+xml",
  "ear": "application/java-archive",
  "ecma": "application/ecmascript",
  "elc": "application/octet-stream",
  "emf": "image/emf",
  "eml": "message/rfc822",
  "emma": "application/emma+xml",
  "emotionml": "application/emotionml+xml",
  "eps": "application/postscript",
  "epub": "application/epub+zip",
  "exe": "application/octet-stream",
  "exi": "application/exi",
  "exp": "application/express",
  "exr": "image/aces",
  "ez": "application/andrew-inset",
  "fdf": "application/fdf",
  "fdt": "application/fdt+xml",
  "fits": "image/fits",
  "g3": "image/g3fax",
  "gbr": "application/rpki-ghostbusters",
  "geojson": "application/geo+json",
  "gif": "image/gif",
  "glb": "model/gltf-binary",
  "gltf": "model/gltf+json",
  "gml": "application/gml+xml",
  "gpx": "application/gpx+xml",
  "gram": "application/srgs",
  "grxml": "application/srgs+xml",
  "gxf": "application/gxf",
  "gz": "application/gzip",
  "h261": "video/h261",
  "h263": "video/h263",
  "h264": "video/h264",
  "heic": "image/heic",
  "heics": "image/heic-sequence",
  "heif": "image/heif",
  "heifs": "image/heif-sequence",
  "hej2": "image/hej2k",
  "held": "application/atsc-held+xml",
  "hjson": "application/hjson",
  "hlp": "application/winhlp",
  "hqx": "application/mac-binhex40",
  "hsj2": "image/hsj2",
  "htm": "text/html",
  "html": "text/html",
  "ics": "text/calendar",
  "ief": "image/ief",
  "ifb": "text/calendar",
  "iges": "model/iges",
  "igs": "model/iges",
  "img": "application/octet-stream",
  "in": "text/plain",
  "ini": "text/plain",
  "ink": "application/inkml+xml",
  "inkml": "application/inkml+xml",
  "ipfix": "application/ipfix",
  "iso": "application/octet-stream",
  "its": "application/its+xml",
  "jade": "text/jade",
  "jar": "application/java-archive",
  "jhc": "image/jphc",
  "jls": "image/jls",
  "jp2": "image/jp2",
  "jpe": "image/jpeg",
  "jpeg": "image/jpeg",
  "jpf": "image/jpx",
  "jpg": "image/jpeg",
  "jpg2": "image/jp2",
  "jpgm": "image/jpm",
  "jpgv": "video/jpeg",
  "jph": "image/jph",
  "jpm": "image/jpm",
  "jpx": "image/jpx",
  "js": "text/javascript",
  "json": "application/json",
  "json5": "application/json5",
  "jsonld": "application/ld+json",
  "jsonml": "application/jsonml+json",
  "jsx": "text/jsx",
  "jt": "model/jt",
  "jxr": "image/jxr",
  "jxra": "image/jxra",
  "jxrs": "image/jxrs",
  "jxs": "image/jxs",
  "jxsc": "image/jxsc",
  "jxsi": "image/jxsi",
  "jxss": "image/jxss",
  "kar": "audio/midi",
  "ktx": "image/ktx",
  "ktx2": "image/ktx2",
  "less": "text/less",
  "lgr": "application/lgr+xml",
  "list": "text/plain",
  "litcoffee": "text/coffeescript",
  "log": "text/plain",
  "lostxml": "application/lost+xml",
  "lrf": "application/octet-stream",
  "m1v": "video/mpeg",
  "m21": "application/mp21",
  "m2a": "audio/mpeg",
  "m2v": "video/mpeg",
  "m3a": "audio/mpeg",
  "m4a": "audio/mp4",
  "m4p": "application/mp4",
  "m4s": "video/iso.segment",
  "ma": "application/mathematica",
  "mads": "application/mads+xml",
  "maei": "application/mmt-aei+xml",
  "man": "text/troff",
  "manifest": "text/cache-manifest",
  "map": "application/json",
  "mar": "application/octet-stream",
  "markdown": "text/markdown",
  "mathml": "application/mathml+xml",
  "mb": "application/mathematica",
  "mbox": "application/mbox",
  "md": "text/markdown",
  "mdx": "text/mdx",
  "me": "text/troff",
  "mesh": "model/mesh",
  "meta4": "application/metalink4+xml",
  "metalink": "application/metalink+xml",
  "mets": "application/mets+xml",
  "mft": "application/rpki-manifest",
  "mid": "audio/midi",
  "midi": "audio/midi",
  "mime": "message/rfc822",
  "mj2": "video/mj2",
  "mjp2": "video/mj2",
  "mjs": "text/javascript",
  "mml": "text/mathml",
  "mods": "application/mods+xml",
  "mov": "video/quicktime",
  "mp2": "audio/mpeg",
  "mp21": "application/mp21",
  "mp2a": "audio/mpeg",
  "mp3": "audio/mpeg",
  "mp4": "video/mp4",
  "mp4a": "audio/mp4",
  "mp4s": "application/mp4",
  "mp4v": "video/mp4",
  "mpd": "application/dash+xml",
  "mpe": "video/mpeg",
  "mpeg": "video/mpeg",
  "mpf": "application/media-policy-dataset+xml",
  "mpg": "video/mpeg",
  "mpg4": "video/mp4",
  "mpga": "audio/mpeg",
  "mpp": "application/dash-patch+xml",
  "mrc": "application/marc",
  "mrcx": "application/marcxml+xml",
  "ms": "text/troff",
  "mscml": "application/mediaservercontrol+xml",
  "msh": "model/mesh",
  "msi": "application/octet-stream",
  "msix": "application/msix",
  "msixbundle": "application/msixbundle",
  "msm": "application/octet-stream",
  "msp": "application/octet-stream",
  "mtl": "model/mtl",
  "musd": "application/mmt-usd+xml",
  "mxf": "application/mxf",
  "mxmf": "audio/mobile-xmf",
  "mxml": "application/xv+xml",
  "n3": "text/n3",
  "nb": "application/mathematica",
  "nq": "application/n-quads",
  "nt": "application/n-triples",
  "obj": "model/obj",
  "oda": "application/oda",
  "oga": "audio/ogg",
  "ogg": "audio/ogg",
  "ogv": "video/ogg",
  "ogx": "application/ogg",
  "omdoc": "application/omdoc+xml",
  "onepkg": "application/onenote",
  "onetmp": "application/onenote",
  "onetoc": "application/onenote",
  "onetoc2": "application/onenote",
  "opf": "application/oebps-package+xml",
  "opus": "audio/ogg",
  "otf": "font/otf",
  "owl": "application/rdf+xml",
  "oxps": "application/oxps",
  "p10": "application/pkcs10",
  "p7c": "application/pkcs7-mime",
  "p7m": "application/pkcs7-mime",
  "p7s": "application/pkcs7-signature",
  "p8": "application/pkcs8",
  "pdf": "application/pdf",
  "pfr": "application/font-tdpfr",
  "pgp": "application/pgp-encrypted",
  "pkg": "application/octet-stream",
  "pki": "application/pkixcmp",
  "pkipath": "application/pkix-pkipath",
  "pls": "application/pls+xml",
  "png": "image/png",
  "prc": "model/prc",
  "prf": "application/pics-rules",
  "provx": "application/provenance+xml",
  "ps": "application/postscript",
  "pskcxml": "application/pskc+xml",
  "pti": "image/prs.pti",
  "qt": "video/quicktime",
  "raml": "application/raml+yaml",
  "rapd": "application/route-apd+xml",
  "rdf": "application/rdf+xml",
  "relo": "application/p2p-overlay+xml",
  "rif": "application/reginfo+xml",
  "rl": "application/resource-lists+xml",
  "rld": "application/resource-lists-diff+xml",
  "rmi": "audio/midi",
  "rnc": "application/relax-ng-compact-syntax",
  "rng": "application/xml",
  "roa": "application/rpki-roa",
  "roff": "text/troff",
  "rq": "application/sparql-query",
  "rs": "application/rls-services+xml",
  "rsat": "application/atsc-rsat+xml",
  "rsd": "application/rsd+xml",
  "rsheet": "application/urc-ressheet+xml",
  "rss": "application/rss+xml",
  "rtf": "text/rtf",
  "rtx": "text/richtext",
  "rusd": "application/route-usd+xml",
  "s3m": "audio/s3m",
  "sbml": "application/sbml+xml",
  "scq": "application/scvp-cv-request",
  "scs": "application/scvp-cv-response",
  "sdp": "application/sdp",
  "senmlx": "application/senml+xml",
  "sensmlx": "application/sensml+xml",
  "ser": "application/java-serialized-object",
  "setpay": "application/set-payment-initiation",
  "setreg": "application/set-registration-initiation",
  "sgi": "image/sgi",
  "sgm": "text/sgml",
  "sgml": "text/sgml",
  "shex": "text/shex",
  "shf": "application/shf+xml",
  "shtml": "text/html",
  "sieve": "application/sieve",
  "sig": "application/pgp-signature",
  "sil": "audio/silk",
  "silo": "model/mesh",
  "siv": "application/sieve",
  "slim": "text/slim",
  "slm": "text/slim",
  "sls": "application/route-s-tsid+xml",
  "smi": "application/smil+xml",
  "smil": "application/smil+xml",
  "snd": "audio/basic",
  "so": "application/octet-stream",
  "spdx": "text/spdx",
  "spp": "application/scvp-vp-response",
  "spq": "application/scvp-vp-request",
  "spx": "audio/ogg",
  "sql": "application/sql",
  "sru": "application/sru+xml",
  "srx": "application/sparql-results+xml",
  "ssdl": "application/ssdl+xml",
  "ssml": "application/ssml+xml",
  "stk": "application/hyperstudio",
  "stl": "model/stl",
  "stpx": "model/step+xml",
  "stpxz": "model/step-xml+zip",
  "stpz": "model/step+zip",
  "styl": "text/stylus",
  "stylus": "text/stylus",
  "svg": "image/svg+xml",
  "svgz": "image/svg+xml",
  "swidtag": "application/swid+xml",
  "t": "text/troff",
  "t38": "image/t38",
  "td": "application/urc-targetdesc+xml",
  "tei": "application/tei+xml",
  "teicorpus": "application/tei+xml",
  "text": "text/plain",
  "tfi": "application/thraud+xml",
  "tfx": "image/tiff-fx",
  "tif": "image/tiff",
  "tiff": "image/tiff",
  "toml": "application/toml",
  "tr": "text/troff",
  "trig": "application/trig",
  "ts": "video/mp2t",
  "tsd": "application/timestamped-data",
  "tsv": "text/tab-separated-values",
  "ttc": "font/collection",
  "ttf": "font/ttf",
  "ttl": "text/turtle",
  "ttml": "application/ttml+xml",
  "txt": "text/plain",
  "u3d": "model/u3d",
  "u8dsn": "message/global-delivery-status",
  "u8hdr": "message/global-headers",
  "u8mdn": "message/global-disposition-notification",
  "u8msg": "message/global",
  "ubj": "application/ubjson",
  "uri": "text/uri-list",
  "uris": "text/uri-list",
  "urls": "text/uri-list",
  "vcard": "text/vcard",
  "vrml": "model/vrml",
  "vtt": "text/vtt",
  "vxml": "application/voicexml+xml",
  "war": "application/java-archive",
  "wasm": "application/wasm",
  "wav": "audio/wav",
  "weba": "audio/webm",
  "webm": "video/webm",
  "webmanifest": "application/manifest+json",
  "webp": "image/webp",
  "wgsl": "text/wgsl",
  "wgt": "application/widget",
  "wif": "application/watcherinfo+xml",
  "wmf": "image/wmf",
  "woff": "font/woff",
  "woff2": "font/woff2",
  "wrl": "model/vrml",
  "wsdl": "application/wsdl+xml",
  "wspolicy": "application/wspolicy+xml",
  "x3d": "model/x3d+xml",
  "x3db": "model/x3d+fastinfoset",
  "x3dbz": "model/x3d+binary",
  "x3dv": "model/x3d-vrml",
  "x3dvz": "model/x3d+vrml",
  "x3dz": "model/x3d+xml",
  "xaml": "application/xaml+xml",
  "xav": "application/xcap-att+xml",
  "xca": "application/xcap-caps+xml",
  "xcs": "application/calendar+xml",
  "xdf": "application/xcap-diff+xml",
  "xdssc": "application/dssc+xml",
  "xel": "application/xcap-el+xml",
  "xenc": "application/xenc+xml",
  "xer": "application/patch-ops-error+xml",
  "xfdf": "application/xfdf",
  "xht": "application/xhtml+xml",
  "xhtml": "application/xhtml+xml",
  "xhvml": "application/xv+xml",
  "xlf": "application/xliff+xml",
  "xm": "audio/xm",
  "xml": "text/xml",
  "xns": "application/xcap-ns+xml",
  "xop": "application/xop+xml",
  "xpl": "application/xproc+xml",
  "xsd": "application/xml",
  "xsf": "application/prs.xsf+xml",
  "xsl": "application/xml",
  "xslt": "application/xml",
  "xspf": "application/xspf+xml",
  "xvm": "application/xv+xml",
  "xvml": "application/xv+xml",
  "yaml": "text/yaml",
  "yang": "application/yang",
  "yin": "application/yin+xml",
  "yml": "text/yaml",
  "zip": "application/zip"
};
function lookup(extn) {
  let tmp = ("" + extn).trim().toLowerCase();
  let idx = tmp.lastIndexOf(".");
  return mimes[!~idx ? tmp : tmp.substring(++idx)];
}
const prettyBytes = (bytes) => {
  let units = ["B", "KB", "MB", "GB", "PB"];
  let i = 0;
  while (bytes > 1024) {
    bytes /= 1024;
    i++;
  }
  let unit = units[i];
  return bytes.toFixed(1) + " " + unit;
};
const playable = () => {
  return true;
};
function loaded(node, { autoplay }) {
  async function handle_playback() {
    if (!autoplay)
      return;
    await node.play();
  }
  node.addEventListener("loadeddata", handle_playback);
  return {
    destroy() {
      node.removeEventListener("loadeddata", handle_playback);
    }
  };
}
async function loadFfmpeg() {
  const ffmpeg = new FFmpeg();
  const baseURL = "https://unpkg.com/@ffmpeg/core@0.12.4/dist/esm";
  await ffmpeg.load({
    coreURL: await toBlobURL(`${baseURL}/ffmpeg-core.js`, "text/javascript"),
    wasmURL: await toBlobURL(`${baseURL}/ffmpeg-core.wasm`, "application/wasm")
  });
  return ffmpeg;
}
async function trimVideo(ffmpeg, startTime, endTime, videoElement) {
  const videoUrl = videoElement.src;
  const mimeType = lookup(videoElement.src) || "video/mp4";
  const blobUrl = await toBlobURL(videoUrl, mimeType);
  const response = await fetch(blobUrl);
  const vidBlob = await response.blob();
  const type = getVideoExtensionFromMimeType(mimeType) || "mp4";
  const inputName = `input.${type}`;
  const outputName = `output.${type}`;
  try {
    if (startTime === 0 && endTime === 0) {
      return vidBlob;
    }
    await ffmpeg.writeFile(
      inputName,
      new Uint8Array(await vidBlob.arrayBuffer())
    );
    let command = [
      "-i",
      inputName,
      ...startTime !== 0 ? ["-ss", startTime.toString()] : [],
      ...endTime !== 0 ? ["-to", endTime.toString()] : [],
      "-c:a",
      "copy",
      outputName
    ];
    await ffmpeg.exec(command);
    const outputData = await ffmpeg.readFile(outputName);
    const outputBlob = new Blob([outputData], {
      type: `video/${type}`
    });
    return outputBlob;
  } catch (error) {
    console.error("Error initializing FFmpeg:", error);
    return vidBlob;
  }
}
const getVideoExtensionFromMimeType = (mimeType) => {
  const videoMimeToExtensionMap = {
    "video/mp4": "mp4",
    "video/webm": "webm",
    "video/ogg": "ogv",
    "video/quicktime": "mov",
    "video/x-msvideo": "avi",
    "video/x-matroska": "mkv",
    "video/mpeg": "mpeg",
    "video/3gpp": "3gp",
    "video/3gpp2": "3g2",
    "video/h261": "h261",
    "video/h263": "h263",
    "video/h264": "h264",
    "video/jpeg": "jpgv",
    "video/jpm": "jpm",
    "video/mj2": "mj2",
    "video/mpv": "mpv",
    "video/vnd.ms-playready.media.pyv": "pyv",
    "video/vnd.uvvu.mp4": "uvu",
    "video/vnd.vivo": "viv",
    "video/x-f4v": "f4v",
    "video/x-fli": "fli",
    "video/x-flv": "flv",
    "video/x-m4v": "m4v",
    "video/x-ms-asf": "asf",
    "video/x-ms-wm": "wm",
    "video/x-ms-wmv": "wmv",
    "video/x-ms-wmx": "wmx",
    "video/x-ms-wvx": "wvx",
    "video/x-sgi-movie": "movie",
    "video/x-smv": "smv"
  };
  return videoMimeToExtensionMap[mimeType] || null;
};
function create_fragment(ctx) {
  let div;
  let textContent = `<span class="load-wrap svelte-1y0s5gv"><span class="loader svelte-1y0s5gv"></span></span>`;
  let t;
  let video;
  let video_src_value;
  let video_data_testid_value;
  let video_updating = false;
  let video_animationframe;
  let video_is_paused = true;
  let loaded_action;
  let current;
  let mounted;
  let dispose;
  const default_slot_template = (
    /*#slots*/
    ctx[18].default
  );
  const default_slot = create_slot(
    default_slot_template,
    ctx,
    /*$$scope*/
    ctx[17],
    null
  );
  function video_timeupdate_handler() {
    cancelAnimationFrame(video_animationframe);
    if (!video.paused) {
      video_animationframe = raf(video_timeupdate_handler);
      video_updating = true;
    }
    ctx[22].call(video);
  }
  return {
    c() {
      div = element("div");
      div.innerHTML = textContent;
      t = space();
      video = element("video");
      if (default_slot)
        default_slot.c();
      this.h();
    },
    l(nodes) {
      div = claim_element(nodes, "DIV", { class: true, ["data-svelte-h"]: true });
      if (get_svelte_dataset(div) !== "svelte-mez4j5")
        div.innerHTML = textContent;
      t = claim_space(nodes);
      video = claim_element(nodes, "VIDEO", {
        src: true,
        preload: true,
        "data-testid": true,
        crossorigin: true
      });
      var video_nodes = children(video);
      if (default_slot)
        default_slot.l(video_nodes);
      video_nodes.forEach(detach);
      this.h();
    },
    h() {
      attr(div, "class", "overlay svelte-1y0s5gv");
      toggle_class(div, "hidden", !/*processingVideo*/
      ctx[10]);
      if (!src_url_equal(video.src, video_src_value = /*resolved_src*/
      ctx[11]))
        attr(video, "src", video_src_value);
      video.muted = /*muted*/
      ctx[4];
      video.playsInline = /*playsinline*/
      ctx[5];
      attr(
        video,
        "preload",
        /*preload*/
        ctx[6]
      );
      video.autoplay = /*autoplay*/
      ctx[7];
      video.controls = /*controls*/
      ctx[8];
      video.loop = /*loop*/
      ctx[9];
      attr(video, "data-testid", video_data_testid_value = /*$$props*/
      ctx[13]["data-testid"]);
      attr(video, "crossorigin", "anonymous");
      if (
        /*duration*/
        ctx[2] === void 0
      )
        add_render_callback(() => (
          /*video_durationchange_handler*/
          ctx[23].call(video)
        ));
    },
    m(target, anchor) {
      insert_hydration(target, div, anchor);
      insert_hydration(target, t, anchor);
      insert_hydration(target, video, anchor);
      if (default_slot) {
        default_slot.m(video, null);
      }
      ctx[25](video);
      current = true;
      if (!mounted) {
        dispose = [
          listen(
            video,
            "loadeddata",
            /*dispatch*/
            ctx[12].bind(null, "loadeddata")
          ),
          listen(
            video,
            "click",
            /*dispatch*/
            ctx[12].bind(null, "click")
          ),
          listen(
            video,
            "play",
            /*dispatch*/
            ctx[12].bind(null, "play")
          ),
          listen(
            video,
            "pause",
            /*dispatch*/
            ctx[12].bind(null, "pause")
          ),
          listen(
            video,
            "ended",
            /*dispatch*/
            ctx[12].bind(null, "ended")
          ),
          listen(
            video,
            "mouseover",
            /*dispatch*/
            ctx[12].bind(null, "mouseover")
          ),
          listen(
            video,
            "mouseout",
            /*dispatch*/
            ctx[12].bind(null, "mouseout")
          ),
          listen(
            video,
            "focus",
            /*dispatch*/
            ctx[12].bind(null, "focus")
          ),
          listen(
            video,
            "blur",
            /*dispatch*/
            ctx[12].bind(null, "blur")
          ),
          listen(
            video,
            "loadstart",
            /*loadstart_handler*/
            ctx[19]
          ),
          listen(
            video,
            "loadeddata",
            /*loadeddata_handler*/
            ctx[20]
          ),
          listen(
            video,
            "loadedmetadata",
            /*loadedmetadata_handler*/
            ctx[21]
          ),
          listen(video, "timeupdate", video_timeupdate_handler),
          listen(
            video,
            "durationchange",
            /*video_durationchange_handler*/
            ctx[23]
          ),
          listen(
            video,
            "play",
            /*video_play_pause_handler*/
            ctx[24]
          ),
          listen(
            video,
            "pause",
            /*video_play_pause_handler*/
            ctx[24]
          ),
          action_destroyer(loaded_action = loaded.call(null, video, { autoplay: (
            /*autoplay*/
            ctx[7] ?? false
          ) }))
        ];
        mounted = true;
      }
    },
    p(ctx2, [dirty]) {
      if (!current || dirty & /*processingVideo*/
      1024) {
        toggle_class(div, "hidden", !/*processingVideo*/
        ctx2[10]);
      }
      if (default_slot) {
        if (default_slot.p && (!current || dirty & /*$$scope*/
        131072)) {
          update_slot_base(
            default_slot,
            default_slot_template,
            ctx2,
            /*$$scope*/
            ctx2[17],
            !current ? get_all_dirty_from_scope(
              /*$$scope*/
              ctx2[17]
            ) : get_slot_changes(
              default_slot_template,
              /*$$scope*/
              ctx2[17],
              dirty,
              null
            ),
            null
          );
        }
      }
      if (!current || dirty & /*resolved_src*/
      2048 && !src_url_equal(video.src, video_src_value = /*resolved_src*/
      ctx2[11])) {
        attr(video, "src", video_src_value);
      }
      if (!current || dirty & /*muted*/
      16) {
        video.muted = /*muted*/
        ctx2[4];
      }
      if (!current || dirty & /*playsinline*/
      32) {
        video.playsInline = /*playsinline*/
        ctx2[5];
      }
      if (!current || dirty & /*preload*/
      64) {
        attr(
          video,
          "preload",
          /*preload*/
          ctx2[6]
        );
      }
      if (!current || dirty & /*autoplay*/
      128) {
        video.autoplay = /*autoplay*/
        ctx2[7];
      }
      if (!current || dirty & /*controls*/
      256) {
        video.controls = /*controls*/
        ctx2[8];
      }
      if (!current || dirty & /*loop*/
      512) {
        video.loop = /*loop*/
        ctx2[9];
      }
      if (!current || dirty & /*$$props*/
      8192 && video_data_testid_value !== (video_data_testid_value = /*$$props*/
      ctx2[13]["data-testid"])) {
        attr(video, "data-testid", video_data_testid_value);
      }
      if (!video_updating && dirty & /*currentTime*/
      2 && !isNaN(
        /*currentTime*/
        ctx2[1]
      )) {
        video.currentTime = /*currentTime*/
        ctx2[1];
      }
      video_updating = false;
      if (dirty & /*paused*/
      8 && video_is_paused !== (video_is_paused = /*paused*/
      ctx2[3])) {
        video[video_is_paused ? "pause" : "play"]();
      }
      if (loaded_action && is_function(loaded_action.update) && dirty & /*autoplay*/
      128)
        loaded_action.update.call(null, { autoplay: (
          /*autoplay*/
          ctx2[7] ?? false
        ) });
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
      if (detaching) {
        detach(div);
        detach(t);
        detach(video);
      }
      if (default_slot)
        default_slot.d(detaching);
      ctx[25](null);
      mounted = false;
      run_all(dispose);
    }
  };
}
function instance($$self, $$props, $$invalidate) {
  let { $$slots: slots = {}, $$scope } = $$props;
  let { src = void 0 } = $$props;
  let { muted = void 0 } = $$props;
  let { playsinline = void 0 } = $$props;
  let { preload = void 0 } = $$props;
  let { autoplay = void 0 } = $$props;
  let { controls = void 0 } = $$props;
  let { currentTime = void 0 } = $$props;
  let { duration = void 0 } = $$props;
  let { paused = void 0 } = $$props;
  let { node = void 0 } = $$props;
  let { loop } = $$props;
  let { is_stream } = $$props;
  let { processingVideo = false } = $$props;
  let resolved_src;
  let stream_active = false;
  let latest_src;
  const dispatch = createEventDispatcher();
  function load_stream(src2, is_stream2, node2) {
    if (!src2 || !is_stream2)
      return;
    if (!node2)
      return;
    if (Hls.isSupported() && !stream_active) {
      const hls = new Hls({
        maxBufferLength: 1,
        // 0.5 seconds (500 ms)
        maxMaxBufferLength: 1,
        // Maximum max buffer length in seconds
        lowLatencyMode: true
      });
      hls.loadSource(src2);
      hls.attachMedia(node2);
      hls.on(Hls.Events.MANIFEST_PARSED, function() {
        node2.play();
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
    }
  }
  function loadstart_handler(event) {
    bubble.call(this, $$self, event);
  }
  function loadeddata_handler(event) {
    bubble.call(this, $$self, event);
  }
  function loadedmetadata_handler(event) {
    bubble.call(this, $$self, event);
  }
  function video_timeupdate_handler() {
    currentTime = this.currentTime;
    $$invalidate(1, currentTime);
  }
  function video_durationchange_handler() {
    duration = this.duration;
    $$invalidate(2, duration);
  }
  function video_play_pause_handler() {
    paused = this.paused;
    $$invalidate(3, paused);
  }
  function video_binding($$value) {
    binding_callbacks[$$value ? "unshift" : "push"](() => {
      node = $$value;
      $$invalidate(0, node);
    });
  }
  $$self.$$set = ($$new_props) => {
    $$invalidate(13, $$props = assign(assign({}, $$props), exclude_internal_props($$new_props)));
    if ("src" in $$new_props)
      $$invalidate(14, src = $$new_props.src);
    if ("muted" in $$new_props)
      $$invalidate(4, muted = $$new_props.muted);
    if ("playsinline" in $$new_props)
      $$invalidate(5, playsinline = $$new_props.playsinline);
    if ("preload" in $$new_props)
      $$invalidate(6, preload = $$new_props.preload);
    if ("autoplay" in $$new_props)
      $$invalidate(7, autoplay = $$new_props.autoplay);
    if ("controls" in $$new_props)
      $$invalidate(8, controls = $$new_props.controls);
    if ("currentTime" in $$new_props)
      $$invalidate(1, currentTime = $$new_props.currentTime);
    if ("duration" in $$new_props)
      $$invalidate(2, duration = $$new_props.duration);
    if ("paused" in $$new_props)
      $$invalidate(3, paused = $$new_props.paused);
    if ("node" in $$new_props)
      $$invalidate(0, node = $$new_props.node);
    if ("loop" in $$new_props)
      $$invalidate(9, loop = $$new_props.loop);
    if ("is_stream" in $$new_props)
      $$invalidate(15, is_stream = $$new_props.is_stream);
    if ("processingVideo" in $$new_props)
      $$invalidate(10, processingVideo = $$new_props.processingVideo);
    if ("$$scope" in $$new_props)
      $$invalidate(17, $$scope = $$new_props.$$scope);
  };
  $$self.$$.update = () => {
    if ($$self.$$.dirty & /*src, latest_src*/
    81920) {
      {
        $$invalidate(11, resolved_src = src);
        $$invalidate(16, latest_src = src);
        const resolving_src = src;
        resolve_wasm_src(resolving_src).then((s) => {
          if (latest_src === resolving_src) {
            $$invalidate(11, resolved_src = s);
          }
        });
      }
    }
    if ($$self.$$.dirty & /*src*/
    16384) {
      stream_active = false;
    }
    if ($$self.$$.dirty & /*src, is_stream, node*/
    49153) {
      load_stream(src, is_stream, node);
    }
  };
  $$props = exclude_internal_props($$props);
  return [
    node,
    currentTime,
    duration,
    paused,
    muted,
    playsinline,
    preload,
    autoplay,
    controls,
    loop,
    processingVideo,
    resolved_src,
    dispatch,
    $$props,
    src,
    is_stream,
    latest_src,
    $$scope,
    slots,
    loadstart_handler,
    loadeddata_handler,
    loadedmetadata_handler,
    video_timeupdate_handler,
    video_durationchange_handler,
    video_play_pause_handler,
    video_binding
  ];
}
class Video extends SvelteComponent {
  constructor(options) {
    super();
    init(this, options, instance, create_fragment, safe_not_equal, {
      src: 14,
      muted: 4,
      playsinline: 5,
      preload: 6,
      autoplay: 7,
      controls: 8,
      currentTime: 1,
      duration: 2,
      paused: 3,
      node: 0,
      loop: 9,
      is_stream: 15,
      processingVideo: 10
    });
  }
}
export {
  Video as V,
  playable as a,
  loadFfmpeg as b,
  loaded as l,
  prettyBytes as p,
  trimVideo as t
};
