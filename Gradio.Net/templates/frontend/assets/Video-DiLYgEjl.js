import{r as Y}from"./file-url-DgijyRSD.js";/* empty css                                                   */import{H as y}from"./hls-CnVhpNcu.js";const Q=new Error("failed to get response body reader"),Z=new Error("failed to complete download"),$="Content-Length",ii=async(t,i)=>{const a=await fetch(t);let e;try{const p=parseInt(a.headers.get($)||"-1"),m=a.body?.getReader();if(!m)throw Q;const g=[];let d=0;for(;;){const{done:s,value:x}=await m.read(),f=x?x.length:0;if(s){if(p!=-1&&p!==d)throw Z;i&&i({url:t,total:p,received:d,delta:f,done:s});break}g.push(x),d+=f,i&&i({url:t,total:p,received:d,delta:f,done:s})}const v=new Uint8Array(d);let h=0;for(const s of g)v.set(s,h),h+=s.length;e=v.buffer}catch(p){console.log("failed to send download progress event: ",p),e=await a.arrayBuffer()}return e},U=async(t,i,a=!1,e)=>{const p=a?await ii(t,e):await(await fetch(t)).arrayBuffer(),m=new Blob([p],{type:i});return URL.createObjectURL(m)};var n;(function(t){t.LOAD="LOAD",t.EXEC="EXEC",t.WRITE_FILE="WRITE_FILE",t.READ_FILE="READ_FILE",t.DELETE_FILE="DELETE_FILE",t.RENAME="RENAME",t.CREATE_DIR="CREATE_DIR",t.LIST_DIR="LIST_DIR",t.DELETE_DIR="DELETE_DIR",t.ERROR="ERROR",t.DOWNLOAD="DOWNLOAD",t.PROGRESS="PROGRESS",t.LOG="LOG",t.MOUNT="MOUNT",t.UNMOUNT="UNMOUNT"})(n||(n={}));const ti=(()=>{let t=0;return()=>t++})(),ai=new Error("ffmpeg is not loaded, call `await ffmpeg.load()` first"),ei=new Error("called FFmpeg.terminate()");class oi{#t=null;#e={};#a={};#o=[];#l=[];loaded=!1;#p=()=>{this.#t&&(this.#t.onmessage=({data:{id:i,type:a,data:e}})=>{switch(a){case n.LOAD:this.loaded=!0,this.#e[i](e);break;case n.MOUNT:case n.UNMOUNT:case n.EXEC:case n.WRITE_FILE:case n.READ_FILE:case n.DELETE_FILE:case n.RENAME:case n.CREATE_DIR:case n.LIST_DIR:case n.DELETE_DIR:this.#e[i](e);break;case n.LOG:this.#o.forEach(p=>p(e));break;case n.PROGRESS:this.#l.forEach(p=>p(e));break;case n.ERROR:this.#a[i](e);break}delete this.#e[i],delete this.#a[i]})};#i=({type:i,data:a},e=[],p)=>this.#t?new Promise((m,g)=>{const d=ti();this.#t&&this.#t.postMessage({id:d,type:i,data:a},e),this.#e[d]=m,this.#a[d]=g,p?.addEventListener("abort",()=>{g(new DOMException(`Message # ${d} was aborted`,"AbortError"))},{once:!0})}):Promise.reject(ai);on(i,a){i==="log"?this.#o.push(a):i==="progress"&&this.#l.push(a)}off(i,a){i==="log"?this.#o=this.#o.filter(e=>e!==a):i==="progress"&&(this.#l=this.#l.filter(e=>e!==a))}load=(i={},{signal:a}={})=>(this.#t||(this.#t=new Worker(new URL(""+new URL("worker-DJ3jufjD.js",import.meta.url).href,import.meta.url),{type:"module"}),this.#p()),this.#i({type:n.LOAD,data:i},void 0,a));exec=(i,a=-1,{signal:e}={})=>this.#i({type:n.EXEC,data:{args:i,timeout:a}},void 0,e);terminate=()=>{const i=Object.keys(this.#a);for(const a of i)this.#a[a](ei),delete this.#a[a],delete this.#e[a];this.#t&&(this.#t.terminate(),this.#t=null,this.loaded=!1)};writeFile=(i,a,{signal:e}={})=>{const p=[];return a instanceof Uint8Array&&p.push(a.buffer),this.#i({type:n.WRITE_FILE,data:{path:i,data:a}},p,e)};mount=(i,a,e)=>{const p=[];return this.#i({type:n.MOUNT,data:{fsType:i,options:a,mountPoint:e}},p)};unmount=i=>{const a=[];return this.#i({type:n.UNMOUNT,data:{mountPoint:i}},a)};readFile=(i,a="binary",{signal:e}={})=>this.#i({type:n.READ_FILE,data:{path:i,encoding:a}},void 0,e);deleteFile=(i,{signal:a}={})=>this.#i({type:n.DELETE_FILE,data:{path:i}},void 0,a);rename=(i,a,{signal:e}={})=>this.#i({type:n.RENAME,data:{oldPath:i,newPath:a}},void 0,e);createDir=(i,{signal:a}={})=>this.#i({type:n.CREATE_DIR,data:{path:i}},void 0,a);listDir=(i,{signal:a}={})=>this.#i({type:n.LIST_DIR,data:{path:i}},void 0,a);deleteDir=(i,{signal:a}={})=>this.#i({type:n.DELETE_DIR,data:{path:i}},void 0,a)}const li={"3g2":"video/3gpp2","3gp":"video/3gpp","3gpp":"video/3gpp","3mf":"model/3mf",aac:"audio/aac",ac:"application/pkix-attr-cert",adp:"audio/adpcm",adts:"audio/aac",ai:"application/postscript",aml:"application/automationml-aml+xml",amlx:"application/automationml-amlx+zip",amr:"audio/amr",apng:"image/apng",appcache:"text/cache-manifest",appinstaller:"application/appinstaller",appx:"application/appx",appxbundle:"application/appxbundle",asc:"application/pgp-keys",atom:"application/atom+xml",atomcat:"application/atomcat+xml",atomdeleted:"application/atomdeleted+xml",atomsvc:"application/atomsvc+xml",au:"audio/basic",avci:"image/avci",avcs:"image/avcs",avif:"image/avif",aw:"application/applixware",bdoc:"application/bdoc",bin:"application/octet-stream",bmp:"image/bmp",bpk:"application/octet-stream",btf:"image/prs.btif",btif:"image/prs.btif",buffer:"application/octet-stream",ccxml:"application/ccxml+xml",cdfx:"application/cdfx+xml",cdmia:"application/cdmi-capability",cdmic:"application/cdmi-container",cdmid:"application/cdmi-domain",cdmio:"application/cdmi-object",cdmiq:"application/cdmi-queue",cer:"application/pkix-cert",cgm:"image/cgm",cjs:"application/node",class:"application/java-vm",coffee:"text/coffeescript",conf:"text/plain",cpl:"application/cpl+xml",cpt:"application/mac-compactpro",crl:"application/pkix-crl",css:"text/css",csv:"text/csv",cu:"application/cu-seeme",cwl:"application/cwl",cww:"application/prs.cww",davmount:"application/davmount+xml",dbk:"application/docbook+xml",deb:"application/octet-stream",def:"text/plain",deploy:"application/octet-stream",dib:"image/bmp","disposition-notification":"message/disposition-notification",dist:"application/octet-stream",distz:"application/octet-stream",dll:"application/octet-stream",dmg:"application/octet-stream",dms:"application/octet-stream",doc:"application/msword",dot:"application/msword",dpx:"image/dpx",drle:"image/dicom-rle",dsc:"text/prs.lines.tag",dssc:"application/dssc+der",dtd:"application/xml-dtd",dump:"application/octet-stream",dwd:"application/atsc-dwd+xml",ear:"application/java-archive",ecma:"application/ecmascript",elc:"application/octet-stream",emf:"image/emf",eml:"message/rfc822",emma:"application/emma+xml",emotionml:"application/emotionml+xml",eps:"application/postscript",epub:"application/epub+zip",exe:"application/octet-stream",exi:"application/exi",exp:"application/express",exr:"image/aces",ez:"application/andrew-inset",fdf:"application/fdf",fdt:"application/fdt+xml",fits:"image/fits",g3:"image/g3fax",gbr:"application/rpki-ghostbusters",geojson:"application/geo+json",gif:"image/gif",glb:"model/gltf-binary",gltf:"model/gltf+json",gml:"application/gml+xml",gpx:"application/gpx+xml",gram:"application/srgs",grxml:"application/srgs+xml",gxf:"application/gxf",gz:"application/gzip",h261:"video/h261",h263:"video/h263",h264:"video/h264",heic:"image/heic",heics:"image/heic-sequence",heif:"image/heif",heifs:"image/heif-sequence",hej2:"image/hej2k",held:"application/atsc-held+xml",hjson:"application/hjson",hlp:"application/winhlp",hqx:"application/mac-binhex40",hsj2:"image/hsj2",htm:"text/html",html:"text/html",ics:"text/calendar",ief:"image/ief",ifb:"text/calendar",iges:"model/iges",igs:"model/iges",img:"application/octet-stream",in:"text/plain",ini:"text/plain",ink:"application/inkml+xml",inkml:"application/inkml+xml",ipfix:"application/ipfix",iso:"application/octet-stream",its:"application/its+xml",jade:"text/jade",jar:"application/java-archive",jhc:"image/jphc",jls:"image/jls",jp2:"image/jp2",jpe:"image/jpeg",jpeg:"image/jpeg",jpf:"image/jpx",jpg:"image/jpeg",jpg2:"image/jp2",jpgm:"image/jpm",jpgv:"video/jpeg",jph:"image/jph",jpm:"image/jpm",jpx:"image/jpx",js:"text/javascript",json:"application/json",json5:"application/json5",jsonld:"application/ld+json",jsonml:"application/jsonml+json",jsx:"text/jsx",jt:"model/jt",jxr:"image/jxr",jxra:"image/jxra",jxrs:"image/jxrs",jxs:"image/jxs",jxsc:"image/jxsc",jxsi:"image/jxsi",jxss:"image/jxss",kar:"audio/midi",ktx:"image/ktx",ktx2:"image/ktx2",less:"text/less",lgr:"application/lgr+xml",list:"text/plain",litcoffee:"text/coffeescript",log:"text/plain",lostxml:"application/lost+xml",lrf:"application/octet-stream",m1v:"video/mpeg",m21:"application/mp21",m2a:"audio/mpeg",m2v:"video/mpeg",m3a:"audio/mpeg",m4a:"audio/mp4",m4p:"application/mp4",m4s:"video/iso.segment",ma:"application/mathematica",mads:"application/mads+xml",maei:"application/mmt-aei+xml",man:"text/troff",manifest:"text/cache-manifest",map:"application/json",mar:"application/octet-stream",markdown:"text/markdown",mathml:"application/mathml+xml",mb:"application/mathematica",mbox:"application/mbox",md:"text/markdown",mdx:"text/mdx",me:"text/troff",mesh:"model/mesh",meta4:"application/metalink4+xml",metalink:"application/metalink+xml",mets:"application/mets+xml",mft:"application/rpki-manifest",mid:"audio/midi",midi:"audio/midi",mime:"message/rfc822",mj2:"video/mj2",mjp2:"video/mj2",mjs:"text/javascript",mml:"text/mathml",mods:"application/mods+xml",mov:"video/quicktime",mp2:"audio/mpeg",mp21:"application/mp21",mp2a:"audio/mpeg",mp3:"audio/mpeg",mp4:"video/mp4",mp4a:"audio/mp4",mp4s:"application/mp4",mp4v:"video/mp4",mpd:"application/dash+xml",mpe:"video/mpeg",mpeg:"video/mpeg",mpf:"application/media-policy-dataset+xml",mpg:"video/mpeg",mpg4:"video/mp4",mpga:"audio/mpeg",mpp:"application/dash-patch+xml",mrc:"application/marc",mrcx:"application/marcxml+xml",ms:"text/troff",mscml:"application/mediaservercontrol+xml",msh:"model/mesh",msi:"application/octet-stream",msix:"application/msix",msixbundle:"application/msixbundle",msm:"application/octet-stream",msp:"application/octet-stream",mtl:"model/mtl",musd:"application/mmt-usd+xml",mxf:"application/mxf",mxmf:"audio/mobile-xmf",mxml:"application/xv+xml",n3:"text/n3",nb:"application/mathematica",nq:"application/n-quads",nt:"application/n-triples",obj:"model/obj",oda:"application/oda",oga:"audio/ogg",ogg:"audio/ogg",ogv:"video/ogg",ogx:"application/ogg",omdoc:"application/omdoc+xml",onepkg:"application/onenote",onetmp:"application/onenote",onetoc:"application/onenote",onetoc2:"application/onenote",opf:"application/oebps-package+xml",opus:"audio/ogg",otf:"font/otf",owl:"application/rdf+xml",oxps:"application/oxps",p10:"application/pkcs10",p7c:"application/pkcs7-mime",p7m:"application/pkcs7-mime",p7s:"application/pkcs7-signature",p8:"application/pkcs8",pdf:"application/pdf",pfr:"application/font-tdpfr",pgp:"application/pgp-encrypted",pkg:"application/octet-stream",pki:"application/pkixcmp",pkipath:"application/pkix-pkipath",pls:"application/pls+xml",png:"image/png",prc:"model/prc",prf:"application/pics-rules",provx:"application/provenance+xml",ps:"application/postscript",pskcxml:"application/pskc+xml",pti:"image/prs.pti",qt:"video/quicktime",raml:"application/raml+yaml",rapd:"application/route-apd+xml",rdf:"application/rdf+xml",relo:"application/p2p-overlay+xml",rif:"application/reginfo+xml",rl:"application/resource-lists+xml",rld:"application/resource-lists-diff+xml",rmi:"audio/midi",rnc:"application/relax-ng-compact-syntax",rng:"application/xml",roa:"application/rpki-roa",roff:"text/troff",rq:"application/sparql-query",rs:"application/rls-services+xml",rsat:"application/atsc-rsat+xml",rsd:"application/rsd+xml",rsheet:"application/urc-ressheet+xml",rss:"application/rss+xml",rtf:"text/rtf",rtx:"text/richtext",rusd:"application/route-usd+xml",s3m:"audio/s3m",sbml:"application/sbml+xml",scq:"application/scvp-cv-request",scs:"application/scvp-cv-response",sdp:"application/sdp",senmlx:"application/senml+xml",sensmlx:"application/sensml+xml",ser:"application/java-serialized-object",setpay:"application/set-payment-initiation",setreg:"application/set-registration-initiation",sgi:"image/sgi",sgm:"text/sgml",sgml:"text/sgml",shex:"text/shex",shf:"application/shf+xml",shtml:"text/html",sieve:"application/sieve",sig:"application/pgp-signature",sil:"audio/silk",silo:"model/mesh",siv:"application/sieve",slim:"text/slim",slm:"text/slim",sls:"application/route-s-tsid+xml",smi:"application/smil+xml",smil:"application/smil+xml",snd:"audio/basic",so:"application/octet-stream",spdx:"text/spdx",spp:"application/scvp-vp-response",spq:"application/scvp-vp-request",spx:"audio/ogg",sql:"application/sql",sru:"application/sru+xml",srx:"application/sparql-results+xml",ssdl:"application/ssdl+xml",ssml:"application/ssml+xml",stk:"application/hyperstudio",stl:"model/stl",stpx:"model/step+xml",stpxz:"model/step-xml+zip",stpz:"model/step+zip",styl:"text/stylus",stylus:"text/stylus",svg:"image/svg+xml",svgz:"image/svg+xml",swidtag:"application/swid+xml",t:"text/troff",t38:"image/t38",td:"application/urc-targetdesc+xml",tei:"application/tei+xml",teicorpus:"application/tei+xml",text:"text/plain",tfi:"application/thraud+xml",tfx:"image/tiff-fx",tif:"image/tiff",tiff:"image/tiff",toml:"application/toml",tr:"text/troff",trig:"application/trig",ts:"video/mp2t",tsd:"application/timestamped-data",tsv:"text/tab-separated-values",ttc:"font/collection",ttf:"font/ttf",ttl:"text/turtle",ttml:"application/ttml+xml",txt:"text/plain",u3d:"model/u3d",u8dsn:"message/global-delivery-status",u8hdr:"message/global-headers",u8mdn:"message/global-disposition-notification",u8msg:"message/global",ubj:"application/ubjson",uri:"text/uri-list",uris:"text/uri-list",urls:"text/uri-list",vcard:"text/vcard",vrml:"model/vrml",vtt:"text/vtt",vxml:"application/voicexml+xml",war:"application/java-archive",wasm:"application/wasm",wav:"audio/wav",weba:"audio/webm",webm:"video/webm",webmanifest:"application/manifest+json",webp:"image/webp",wgsl:"text/wgsl",wgt:"application/widget",wif:"application/watcherinfo+xml",wmf:"image/wmf",woff:"font/woff",woff2:"font/woff2",wrl:"model/vrml",wsdl:"application/wsdl+xml",wspolicy:"application/wspolicy+xml",x3d:"model/x3d+xml",x3db:"model/x3d+fastinfoset",x3dbz:"model/x3d+binary",x3dv:"model/x3d-vrml",x3dvz:"model/x3d+vrml",x3dz:"model/x3d+xml",xaml:"application/xaml+xml",xav:"application/xcap-att+xml",xca:"application/xcap-caps+xml",xcs:"application/calendar+xml",xdf:"application/xcap-diff+xml",xdssc:"application/dssc+xml",xel:"application/xcap-el+xml",xenc:"application/xenc+xml",xer:"application/patch-ops-error+xml",xfdf:"application/xfdf",xht:"application/xhtml+xml",xhtml:"application/xhtml+xml",xhvml:"application/xv+xml",xlf:"application/xliff+xml",xm:"audio/xm",xml:"text/xml",xns:"application/xcap-ns+xml",xop:"application/xop+xml",xpl:"application/xproc+xml",xsd:"application/xml",xsf:"application/prs.xsf+xml",xsl:"application/xml",xslt:"application/xml",xspf:"application/xspf+xml",xvm:"application/xv+xml",xvml:"application/xv+xml",yaml:"text/yaml",yang:"application/yang",yin:"application/yin+xml",yml:"text/yaml",zip:"application/zip"};function pi(t){let i=(""+t).trim().toLowerCase(),a=i.lastIndexOf(".");return li[~a?i.substring(++a):i]}const Ti=t=>{let i=["B","KB","MB","GB","PB"],a=0;for(;t>1024;)t/=1024,a++;let e=i[a];return t.toFixed(1)+" "+e},Ai=()=>!0;function si(t,{autoplay:i}){async function a(){i&&await t.play()}return t.addEventListener("loadeddata",a),{destroy(){t.removeEventListener("loadeddata",a)}}}async function Ni(){const t=new oi,i="https://unpkg.com/@ffmpeg/core@0.12.4/dist/esm";return await t.load({coreURL:await U(`${i}/ffmpeg-core.js`,"text/javascript"),wasmURL:await U(`${i}/ffmpeg-core.wasm`,"application/wasm")}),t}async function Ui(t,i,a,e){const p=e.src,m=pi(e.src)||"video/mp4",g=await U(p,m),v=await(await fetch(g)).blob(),h=ni(m)||"mp4",s=`input.${h}`,x=`output.${h}`;try{if(i===0&&a===0)return v;await t.writeFile(s,new Uint8Array(await v.arrayBuffer()));let f=["-i",s,...i!==0?["-ss",i.toString()]:[],...a!==0?["-to",a.toString()]:[],"-c:a","copy",x];await t.exec(f);const b=await t.readFile(x);return new Blob([b],{type:`video/${h}`})}catch(f){return console.error("Error initializing FFmpeg:",f),v}}const ni=t=>({"video/mp4":"mp4","video/webm":"webm","video/ogg":"ogv","video/quicktime":"mov","video/x-msvideo":"avi","video/x-matroska":"mkv","video/mpeg":"mpeg","video/3gpp":"3gp","video/3gpp2":"3g2","video/h261":"h261","video/h263":"h263","video/h264":"h264","video/jpeg":"jpgv","video/jpm":"jpm","video/mj2":"mj2","video/mpv":"mpv","video/vnd.ms-playready.media.pyv":"pyv","video/vnd.uvvu.mp4":"uvu","video/vnd.vivo":"viv","video/x-f4v":"f4v","video/x-fli":"fli","video/x-flv":"flv","video/x-m4v":"m4v","video/x-ms-asf":"asf","video/x-ms-wm":"wm","video/x-ms-wmv":"wmv","video/x-ms-wmx":"wmx","video/x-ms-wvx":"wvx","video/x-sgi-movie":"movie","video/x-smv":"smv"})[t]||null,{SvelteComponent:mi,action_destroyer:ci,add_render_callback:ri,assign:q,attr:j,binding_callbacks:di,bubble:T,create_slot:ui,detach:A,element:S,exclude_internal_props:z,flush:E,get_all_dirty_from_scope:xi,get_slot_changes:fi,init:gi,insert:N,is_function:hi,listen:u,raf:vi,run_all:Ei,safe_not_equal:bi,space:ji,src_url_equal:B,toggle_class:M,transition_in:wi,transition_out:yi,update_slot_base:_i}=window.__gradio__svelte__internal,{createEventDispatcher:Ri}=window.__gradio__svelte__internal;function ki(t){let i,a,e,p,m,g=!1,d,v=!0,h,s,x,f;const b=t[18].default,r=ui(b,t,t[17],null);function _(){cancelAnimationFrame(d),e.paused||(d=vi(_),g=!0),t[22].call(e)}return{c(){i=S("div"),i.innerHTML='<span class="load-wrap svelte-1y0s5gv"><span class="loader svelte-1y0s5gv"></span></span>',a=ji(),e=S("video"),r&&r.c(),j(i,"class","overlay svelte-1y0s5gv"),M(i,"hidden",!t[10]),B(e.src,p=t[11])||j(e,"src",p),e.muted=t[4],e.playsInline=t[5],j(e,"preload",t[6]),e.autoplay=t[7],e.controls=t[8],e.loop=t[9],j(e,"data-testid",m=t[13]["data-testid"]),j(e,"crossorigin","anonymous"),t[2]===void 0&&ri(()=>t[23].call(e))},m(l,c){N(l,i,c),N(l,a,c),N(l,e,c),r&&r.m(e,null),t[25](e),s=!0,x||(f=[u(e,"loadeddata",t[12].bind(null,"loadeddata")),u(e,"click",t[12].bind(null,"click")),u(e,"play",t[12].bind(null,"play")),u(e,"pause",t[12].bind(null,"pause")),u(e,"ended",t[12].bind(null,"ended")),u(e,"mouseover",t[12].bind(null,"mouseover")),u(e,"mouseout",t[12].bind(null,"mouseout")),u(e,"focus",t[12].bind(null,"focus")),u(e,"blur",t[12].bind(null,"blur")),u(e,"loadstart",t[19]),u(e,"loadeddata",t[20]),u(e,"loadedmetadata",t[21]),u(e,"timeupdate",_),u(e,"durationchange",t[23]),u(e,"play",t[24]),u(e,"pause",t[24]),ci(h=si.call(null,e,{autoplay:t[7]??!1}))],x=!0)},p(l,[c]){(!s||c&1024)&&M(i,"hidden",!l[10]),r&&r.p&&(!s||c&131072)&&_i(r,b,l,l[17],s?fi(b,l[17],c,null):xi(l[17]),null),(!s||c&2048&&!B(e.src,p=l[11]))&&j(e,"src",p),(!s||c&16)&&(e.muted=l[4]),(!s||c&32)&&(e.playsInline=l[5]),(!s||c&64)&&j(e,"preload",l[6]),(!s||c&128)&&(e.autoplay=l[7]),(!s||c&256)&&(e.controls=l[8]),(!s||c&512)&&(e.loop=l[9]),(!s||c&8192&&m!==(m=l[13]["data-testid"]))&&j(e,"data-testid",m),!g&&c&2&&!isNaN(l[1])&&(e.currentTime=l[1]),g=!1,c&8&&v!==(v=l[3])&&e[v?"pause":"play"](),h&&hi(h.update)&&c&128&&h.update.call(null,{autoplay:l[7]??!1})},i(l){s||(wi(r,l),s=!0)},o(l){yi(r,l),s=!1},d(l){l&&(A(i),A(a),A(e)),r&&r.d(l),t[25](null),x=!1,Ei(f)}}}function Li(t,i,a){let{$$slots:e={},$$scope:p}=i,{src:m=void 0}=i,{muted:g=void 0}=i,{playsinline:d=void 0}=i,{preload:v=void 0}=i,{autoplay:h=void 0}=i,{controls:s=void 0}=i,{currentTime:x=void 0}=i,{duration:f=void 0}=i,{paused:b=void 0}=i,{node:r=void 0}=i,{loop:_}=i,{is_stream:l}=i,{processingVideo:c=!1}=i,R,k=!1,L;const C=Ri();function P(o,D,O){if(!(!o||!D)&&O&&y.isSupported()&&!k){const w=new y({maxBufferLength:1,maxMaxBufferLength:1,lowLatencyMode:!0});w.loadSource(o),w.attachMedia(O),w.on(y.Events.MANIFEST_PARSED,function(){O.play()}),w.on(y.Events.ERROR,function(J,I){if(console.error("HLS error:",J,I),I.fatal)switch(I.type){case y.ErrorTypes.NETWORK_ERROR:console.error("Fatal network error encountered, trying to recover"),w.startLoad();break;case y.ErrorTypes.MEDIA_ERROR:console.error("Fatal media error encountered, trying to recover"),w.recoverMediaError();break;default:console.error("Fatal error, cannot recover"),w.destroy();break}}),k=!0}}function V(o){T.call(this,t,o)}function W(o){T.call(this,t,o)}function G(o){T.call(this,t,o)}function H(){x=this.currentTime,a(1,x)}function F(){f=this.duration,a(2,f)}function X(){b=this.paused,a(3,b)}function K(o){di[o?"unshift":"push"](()=>{r=o,a(0,r)})}return t.$$set=o=>{a(13,i=q(q({},i),z(o))),"src"in o&&a(14,m=o.src),"muted"in o&&a(4,g=o.muted),"playsinline"in o&&a(5,d=o.playsinline),"preload"in o&&a(6,v=o.preload),"autoplay"in o&&a(7,h=o.autoplay),"controls"in o&&a(8,s=o.controls),"currentTime"in o&&a(1,x=o.currentTime),"duration"in o&&a(2,f=o.duration),"paused"in o&&a(3,b=o.paused),"node"in o&&a(0,r=o.node),"loop"in o&&a(9,_=o.loop),"is_stream"in o&&a(15,l=o.is_stream),"processingVideo"in o&&a(10,c=o.processingVideo),"$$scope"in o&&a(17,p=o.$$scope)},t.$$.update=()=>{if(t.$$.dirty&81920){a(11,R=m),a(16,L=m);const o=m;Y(o).then(D=>{L===o&&a(11,R=D)})}t.$$.dirty&16384&&(k=!1),t.$$.dirty&49153&&P(m,l,r)},i=z(i),[r,x,f,b,g,d,v,h,s,_,c,R,C,i,m,l,L,p,e,V,W,G,H,F,X,K]}class qi extends mi{constructor(i){super(),gi(this,i,Li,ki,bi,{src:14,muted:4,playsinline:5,preload:6,autoplay:7,controls:8,currentTime:1,duration:2,paused:3,node:0,loop:9,is_stream:15,processingVideo:10})}get src(){return this.$$.ctx[14]}set src(i){this.$$set({src:i}),E()}get muted(){return this.$$.ctx[4]}set muted(i){this.$$set({muted:i}),E()}get playsinline(){return this.$$.ctx[5]}set playsinline(i){this.$$set({playsinline:i}),E()}get preload(){return this.$$.ctx[6]}set preload(i){this.$$set({preload:i}),E()}get autoplay(){return this.$$.ctx[7]}set autoplay(i){this.$$set({autoplay:i}),E()}get controls(){return this.$$.ctx[8]}set controls(i){this.$$set({controls:i}),E()}get currentTime(){return this.$$.ctx[1]}set currentTime(i){this.$$set({currentTime:i}),E()}get duration(){return this.$$.ctx[2]}set duration(i){this.$$set({duration:i}),E()}get paused(){return this.$$.ctx[3]}set paused(i){this.$$set({paused:i}),E()}get node(){return this.$$.ctx[0]}set node(i){this.$$set({node:i}),E()}get loop(){return this.$$.ctx[9]}set loop(i){this.$$set({loop:i}),E()}get is_stream(){return this.$$.ctx[15]}set is_stream(i){this.$$set({is_stream:i}),E()}get processingVideo(){return this.$$.ctx[10]}set processingVideo(i){this.$$set({processingVideo:i}),E()}}export{qi as V,Ai as a,Ni as b,si as l,Ti as p,Ui as t};
//# sourceMappingURL=Video-DiLYgEjl.js.map