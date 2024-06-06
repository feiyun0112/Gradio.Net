const __vite__fileDeps=["./eventsource-CfQQVHjX.js","./_commonjsHelpers-BosuxZz1.js","./url-BybKzMWq.js","./__vite-browser-external-DFe-p4yY.js","./wrapper-CviSselG-DCvi549i.js","./Index-C_XES7MU.js","./Index-DQmn0l2I.css"],__vite__mapDeps=i=>i.map(i=>__vite__fileDeps[i]);
import*as ee from"./svelte/svelte.js";(function(){const s=document.createElement("link").relList;if(s&&s.supports&&s.supports("modulepreload"))return;for(const n of document.querySelectorAll('link[rel="modulepreload"]'))o(n);new MutationObserver(n=>{for(const i of n)if(i.type==="childList")for(const r of i.addedNodes)r.tagName==="LINK"&&r.rel==="modulepreload"&&o(r)}).observe(document,{childList:!0,subtree:!0});function t(n){const i={};return n.integrity&&(i.integrity=n.integrity),n.referrerPolicy&&(i.referrerPolicy=n.referrerPolicy),n.crossOrigin==="use-credentials"?i.credentials="include":n.crossOrigin==="anonymous"?i.credentials="omit":i.credentials="same-origin",i}function o(n){if(n.ep)return;n.ep=!0;const i=t(n);fetch(n.href,i)}})();const je="modulepreload",Ie=function(e,s){return new URL(e,s).href},de={},W=function(s,t,o){let n=Promise.resolve();if(t&&t.length>0){const i=document.getElementsByTagName("link"),r=document.querySelector("meta[property=csp-nonce]"),a=r?.nonce||r?.getAttribute("nonce");n=Promise.all(t.map(u=>{if(u=Ie(u,o),u in de)return;de[u]=!0;const l=u.endsWith(".css"),d=l?'[rel="stylesheet"]':"";if(!!o)for(let v=i.length-1;v>=0;v--){const A=i[v];if(A.href===u&&(!l||A.rel==="stylesheet"))return}else if(document.querySelector(`link[href="${u}"]${d}`))return;const p=document.createElement("link");if(p.rel=l?"stylesheet":je,l||(p.as="script",p.crossOrigin=""),p.href=u,a&&p.setAttribute("nonce",a),document.head.appendChild(p),l)return new Promise((v,A)=>{p.addEventListener("load",v),p.addEventListener("error",()=>A(new Error(`Unable to preload CSS for ${u}`)))})}))}return n.then(()=>s()).catch(i=>{const r=new Event("vite:preloadError",{cancelable:!0});if(r.payload=i,window.dispatchEvent(r),!r.defaultPrevented)throw i})};var qe=Object.defineProperty,Ue=(e,s,t)=>s in e?qe(e,s,{enumerable:!0,configurable:!0,writable:!0,value:t}):e[s]=t,m=(e,s,t)=>(Ue(e,typeof s!="symbol"?s+"":s,t),t),te=new Intl.Collator(0,{numeric:1}).compare;function ye(e,s,t){return e=e.split("."),s=s.split("."),te(e[0],s[0])||te(e[1],s[1])||(s[2]=s.slice(2).join("."),t=/[.-]/.test(e[2]=e.slice(2).join(".")),t==/[.-]/.test(s[2])?te(e[2],s[2]):t?-1:1)}const xe="host",ze="upload",Be="login",Fe="config",Me="info",Ge="runtime",Je="sleeptime",We="https://gradio-space-api-fetcher-v2.hf.space/api",ve="This application is currently busy. Please try again. ",F="Connection errored out. ",G="Could not resolve app config. ",He="Could not get space status. ",Ve="Could not get API info. ",ce="Space metadata could not be loaded. ",Ke="Invalid URL. A full URL path is required.",Ze="Not authorized to access this space. ",be="Invalid credentials. Could not login. ",Ye="Login credentials are required to access this space.";function Se(e,s,t){return s.startsWith("http://")||s.startsWith("https://")?t?e:s:e+s}async function fe(e,s,t){try{return(await(await fetch(`https://huggingface.co/api/spaces/${e}/jwt`,{headers:{Authorization:`Bearer ${s}`,...t?{Cookie:t}:{}}})).json()).token||!1}catch{return!1}}function Qe(e){let s={};return e.forEach(({api_name:t,id:o})=>{t&&(s[t]=o)}),s}async function Xe(e){var s;const t=this.options.hf_token?{Authorization:`Bearer ${this.options.hf_token}`}:{};if(t["Content-Type"]="application/json",typeof window<"u"&&window.gradio_config&&location.origin!=="http://localhost:9876"&&!window.gradio_config.dev_mode){const o=window.gradio_config.root,n=window.gradio_config;let i=Se(e,n.root,!1);return n.root=i,{...n,path:o}}else if(e){const o=Ae(e,Fe),n=await this.fetch(o,{headers:t,credentials:"include"});if(n?.status===401&&!this.options.auth)throw new Error(Ye);if(n?.status===401&&this.options.auth)throw new Error(be);if(n?.status===200){let i=await n.json();return i.path=i.path??"",i.root=e,(s=i.dependencies)==null||s.forEach((r,a)=>{r.id===void 0&&(r.id=a)}),i}else if(n?.status===401)throw new Error(Ze);throw new Error(G)}throw new Error(G)}async function et(){const{http_protocol:e,host:s}=await K(this.app_reference,this.options.hf_token);try{if(this.options.auth){const t=await Ee(e,s,this.options.auth,this.fetch,this.options.hf_token);t&&this.set_cookies(t)}}catch(t){throw Error(t.message)}}async function Ee(e,s,t,o,n){const i=new FormData;i.append("username",t?.[0]),i.append("password",t?.[1]);let r={};n&&(r.Authorization=`Bearer ${n}`);const a=await o(`${e}//${s}/${Be}`,{headers:r,method:"POST",body:i,credentials:"include"});if(a.status===200)return a.headers.get("set-cookie");throw a.status===401?new Error(be):new Error(ce)}function se(e){if(e.startsWith("http")){const{protocol:s,host:t,pathname:o}=new URL(e);return t.endsWith("hf.space")?{ws_protocol:"wss",host:t,http_protocol:s}:{ws_protocol:s==="https:"?"wss":"ws",http_protocol:s,host:t+(o!=="/"?o:"")}}else if(e.startsWith("file:"))return{ws_protocol:"ws",http_protocol:"http:",host:"lite.local"};return{ws_protocol:"wss",http_protocol:"https:",host:e}}const $e=e=>{let s=[];return e.split(/,(?=\s*[^\s=;]+=[^\s=;]+)/).forEach(o=>{const[n,i]=o.split(";")[0].split("=");n&&i&&s.push(`${n.trim()}=${i.trim()}`)}),s},ke=/^[a-zA-Z0-9_\-\.]+\/[a-zA-Z0-9_\-\.]+$/,tt=/.*hf\.space\/{0,1}$/;async function K(e,s){const t={};s&&(t.Authorization=`Bearer ${s}`);const o=e.trim().replace(/\/$/,"");if(ke.test(o))try{const i=(await(await fetch(`https://huggingface.co/api/spaces/${o}/${xe}`,{headers:t})).json()).host;return{space_id:e,...se(i)}}catch{throw new Error(ce)}if(tt.test(o)){const{ws_protocol:n,http_protocol:i,host:r}=se(o);return{space_id:r.replace(".hf.space",""),ws_protocol:n,http_protocol:i,host:r}}return{space_id:!1,...se(o)}}const Ae=(...e)=>{try{return e.reduce((s,t)=>(s=s.replace(/\/+$/,""),t=t.replace(/^\/+/,""),new URL(t,s+"/").toString()))}catch{throw new Error(Ke)}};function st(e,s,t){const o={named_endpoints:{},unnamed_endpoints:{}};return Object.keys(e).forEach(n=>{(n==="named_endpoints"||n==="unnamed_endpoints")&&(o[n]={},Object.entries(e[n]).forEach(([i,{parameters:r,returns:a}])=>{var u,l,d,c;const p=((u=s.dependencies.find(f=>f.api_name===i||f.api_name===i.replace("/","")))==null?void 0:u.id)||t[i.replace("/","")]||-1,v=p!==-1?(l=s.dependencies.find(f=>f.id==p))==null?void 0:l.types:{continuous:!1,generator:!1};if(p!==-1&&((c=(d=s.dependencies.find(f=>f.id==p))==null?void 0:d.inputs)==null?void 0:c.length)!==r.length){const f=s.dependencies.find(w=>w.id==p).inputs.map(w=>{var $;return($=s.components.find(O=>O.id===w))==null?void 0:$.type});try{f.forEach((w,$)=>{if(w==="state"){const O={component:"state",example:null,parameter_default:null,parameter_has_default:!0,parameter_name:null,hidden:!0};r.splice($,0,O)}})}catch(w){console.error(w)}}const A=(f,w,$,O)=>({...f,description:it(f?.type,$),type:nt(f?.type,w,$,O)||""});o[n][i]={parameters:r.map(f=>A(f,f?.component,f?.serializer,"parameter")),returns:a.map(f=>A(f,f?.component,f?.serializer,"return")),type:v}}))}),o}function nt(e,s,t,o){switch(e?.type){case"string":return"string";case"boolean":return"boolean";case"number":return"number"}if(t==="JSONSerializable"||t==="StringSerializable")return"any";if(t==="ListStringSerializable")return"string[]";if(s==="Image")return o==="parameter"?"Blob | File | Buffer":"string";if(t==="FileSerializable")return e?.type==="array"?o==="parameter"?"(Blob | File | Buffer)[]":"{ name: string; data: string; size?: number; is_file?: boolean; orig_name?: string}[]":o==="parameter"?"Blob | File | Buffer":"{ name: string; data: string; size?: number; is_file?: boolean; orig_name?: string}";if(t==="GallerySerializable")return o==="parameter"?"[(Blob | File | Buffer), (string | null)][]":"[{ name: string; data: string; size?: number; is_file?: boolean; orig_name?: string}, (string | null))][]"}function it(e,s){return s==="GallerySerializable"?"array of [file, label] tuples":s==="ListStringSerializable"?"array of strings":s==="FileSerializable"?"array of files or single file":e?.description}function ne(e,s){switch(e.msg){case"send_data":return{type:"data"};case"send_hash":return{type:"hash"};case"queue_full":return{type:"update",status:{queue:!0,message:ve,stage:"error",code:e.code,success:e.success}};case"heartbeat":return{type:"heartbeat"};case"unexpected_error":return{type:"unexpected_error",status:{queue:!0,message:e.message,stage:"error",success:!1}};case"estimation":return{type:"update",status:{queue:!0,stage:s||"pending",code:e.code,size:e.queue_size,position:e.rank,eta:e.rank_eta,success:e.success}};case"progress":return{type:"update",status:{queue:!0,stage:"pending",code:e.code,progress_data:e.progress_data,success:e.success}};case"log":return{type:"log",data:e};case"process_generating":return{type:"generating",status:{queue:!0,message:e.success?null:e.output.error,stage:e.success?"generating":"error",code:e.code,progress_data:e.progress_data,eta:e.average_duration},data:e.success?e.output:null};case"process_completed":return"error"in e.output?{type:"update",status:{queue:!0,message:e.output.error,stage:"error",code:e.code,success:e.success}}:{type:"complete",status:{queue:!0,message:e.success?void 0:e.output.error,stage:e.success?"complete":"error",code:e.code,progress_data:e.progress_data,changed_state_ids:e.success?e.output.changed_state_ids:void 0},data:e.success?e.output:null};case"process_starts":return{type:"update",status:{queue:!0,stage:"pending",code:e.code,size:e.rank,position:0,success:e.success,eta:e.eta}}}return{type:"none",status:{stage:"error",queue:!0}}}const ot=(e,s)=>{const t=Object.values(s.named_endpoints).flatMap(i=>i.parameters);if(Array.isArray(e))return e.length>t.length&&console.warn("Too many arguments provided for the endpoint."),e;const o=[],n=Object.keys(e);return t.forEach((i,r)=>{if(e.hasOwnProperty(i.parameter_name))o[r]=e[i.parameter_name];else if(i.parameter_has_default)o[r]=i.parameter_default;else throw new Error(`No value provided for required parameter: ${i.parameter_name}`)}),n.forEach(i=>{if(!t.some(r=>r.parameter_name===i))throw new Error(`Parameter \`${i}\` is not a valid keyword argument. Please refer to the API for usage.`)}),o.forEach((i,r)=>{if(i===void 0&&!t[r].parameter_has_default)throw new Error(`No value provided for required parameter: ${t[r].parameter_name}`)}),o};async function rt(){if(this.api_info)return this.api_info;const{hf_token:e}=this.options,{config:s}=this,t={"Content-Type":"application/json"};if(e&&(t.Authorization=`Bearer ${e}`),!!s)try{let o;if(ye(s?.version||"2.0.0","3.30")<0)o=await this.fetch(We,{method:"POST",body:JSON.stringify({serialize:!1,config:JSON.stringify(s)}),headers:t,credentials:"include"});else{const i=Ae(s.root,Me);o=await this.fetch(i,{headers:t,credentials:"include"})}if(!o.ok)throw new Error(F);let n=await o.json();return"api"in n&&(n=n.api),n.named_endpoints["/predict"]&&!n.unnamed_endpoints[0]&&(n.unnamed_endpoints[0]=n.named_endpoints["/predict"]),st(n,s,this.api_map)}catch(o){""+o.message}}async function at(e,s,t){var o;const n={};(o=this==null?void 0:this.options)!=null&&o.hf_token&&(n.Authorization=`Bearer ${this.options.hf_token}`);const i=1e3,r=[];let a;for(let u=0;u<s.length;u+=i){const l=s.slice(u,u+i),d=new FormData;l.forEach(p=>{d.append("files",p)});try{const p=t?`${e}/upload?upload_id=${t}`:`${e}/${ze}`;a=await this.fetch(p,{method:"POST",body:d,headers:n,credentials:"include"})}catch(p){throw new Error(F+p.message)}if(!a.ok){const p=await a.text();return{error:`HTTP ${a.status}: ${p}`}}const c=await a.json();c&&r.push(...c)}return{files:r}}async function ct(e,s,t,o){let n=(Array.isArray(e)?e:[e]).map(r=>r.blob);const i=n.filter(r=>r.size>(o??1/0));if(i.length)throw new Error(`File size exceeds the maximum allowed size of ${o} bytes: ${i.map(r=>r.name).join(", ")}`);return await Promise.all(await this.upload_files(s,n,t).then(async r=>{if(r.error)throw new Error(r.error);return r.files?r.files.map((a,u)=>new le({...e[u],path:a,url:s+"/file="+a})):[]}))}async function Rt(e,s){return e.map(t=>new le({path:t.name,orig_name:t.name,blob:t,size:t.size,mime_type:t.type,is_stream:s}))}class le{constructor({path:s,url:t,orig_name:o,size:n,blob:i,is_stream:r,mime_type:a,alt_text:u}){m(this,"path"),m(this,"url"),m(this,"orig_name"),m(this,"size"),m(this,"blob"),m(this,"is_stream"),m(this,"mime_type"),m(this,"alt_text"),m(this,"meta",{_type:"gradio.FileData"}),this.path=s,this.url=t,this.orig_name=o,this.size=n,this.blob=t?void 0:i,this.is_stream=r,this.mime_type=a,this.alt_text=u}}function _e(e,s,t){for(;t.length>1;){const n=t.shift();if(typeof n=="string"||typeof n=="number")e=e[n];else throw new Error("Invalid key type")}const o=t.shift();if(typeof o=="string"||typeof o=="number")e[o]=s;else throw new Error("Invalid key type")}async function ie(e,s=void 0,t=[],o=!1,n=void 0){if(Array.isArray(e)){let i=[];return await Promise.all(e.map(async(r,a)=>{var u;let l=t.slice();l.push(String(a));const d=await ie(e[a],o?((u=n?.parameters[a])==null?void 0:u.component)||void 0:s,l,!1,n);i=i.concat(d)})),i}else{if(globalThis.Buffer&&e instanceof globalThis.Buffer||e instanceof Blob)return[{path:t,blob:s==="Image"?!1:new Ce([e]),type:s}];if(typeof e=="object"&&e!==null){let i=[];for(const r of Object.keys(e)){const a=[...t,r],u=e[r];i=i.concat(await ie(u,void 0,a,!1,n))}return i}}return[]}function lt(e,s){var t,o;let n=(o=(t=s?.dependencies)==null?void 0:t.find(i=>i.id==e))==null?void 0:o.queue;return n!=null?!n:!s.enable_queue}function ut(e,s){return new Promise((t,o)=>{const n=new MessageChannel;n.port1.onmessage=({data:i})=>{n.port1.close(),t(i)},window.parent.postMessage(e,s,[n.port2])})}async function pt(e,s,t){const o=this,n=await ie(s,void 0,[],!0,t);return(await Promise.all(n.map(async({path:r,blob:a,type:u})=>{if(!a)return{path:r,type:u};const l=await o.upload_files(e,[a]),d=l.files&&l.files[0];return{path:r,file_url:d,type:u,name:a instanceof File?a?.name:void 0}}))).forEach(({path:r,file_url:a,type:u,name:l})=>{if(u==="Gallery")_e(s,a,r);else if(a){const d=new le({path:a,orig_name:l});_e(s,d,r)}}),s}async function ht(e,s,t){const o={"Content-Type":"application/json"};this.options.hf_token&&(o.Authorization=`Bearer ${this.options.hf_token}`);try{var n=await this.fetch(e,{method:"POST",body:JSON.stringify(s),headers:{...o,...t},credentials:"include"})}catch{return[{error:F},500]}let i,r;try{i=await n.json(),r=n.status}catch(a){i={error:`Could not parse server response: ${a}`},r=500}return[i,r]}async function dt(e,s){let t=!1,o=!1,n;if(!this.config)throw new Error("Could not resolve app config");if(typeof e=="number")n=this.config.dependencies.find(i=>i.id==e);else{const i=e.replace(/^\//,"");n=this.config.dependencies.find(r=>r.id==this.api_map[i])}if(n?.types.continuous)throw new Error("Cannot call predict on this function as it may run forever. Use submit instead");return new Promise(async(i,r)=>{const a=this.submit(e,s);let u;a.on("data",l=>{o&&(a.destroy(),i(l)),t=!0,u=l}).on("status",l=>{l.stage==="error"&&r(l),l.stage==="complete"&&(o=!0,t&&(a.destroy(),i(u)))})})}async function oe(e,s,t){let o=s==="subdomain"?`https://huggingface.co/api/spaces/by-subdomain/${e}`:`https://huggingface.co/api/spaces/${e}`,n,i;try{if(n=await fetch(o),i=n.status,i!==200)throw new Error;n=await n.json()}catch{t({status:"error",load_status:"error",message:He,detail:"NOT_FOUND"});return}if(!n||i!==200)return;const{runtime:{stage:r},id:a}=n;switch(r){case"STOPPED":case"SLEEPING":t({status:"sleeping",load_status:"pending",message:"Space is asleep. Waking it up...",detail:r}),setTimeout(()=>{oe(e,s,t)},1e3);break;case"PAUSED":t({status:"paused",load_status:"error",message:"This space has been paused by the author. If you would like to try this demo, consider duplicating the space.",detail:r,discussions_enabled:await me(a)});break;case"RUNNING":case"RUNNING_BUILDING":t({status:"running",load_status:"complete",message:"",detail:r});break;case"BUILDING":t({status:"building",load_status:"pending",message:"Space is building...",detail:r}),setTimeout(()=>{oe(e,s,t)},1e3);break;default:t({status:"space_error",load_status:"error",message:"This space is experiencing an issue.",detail:r,discussions_enabled:await me(a)});break}}const ft=/^(?=[^]*\b[dD]iscussions{0,1}\b)(?=[^]*\b[dD]isabled\b)[^]*$/;async function me(e){try{const t=(await fetch(`https://huggingface.co/api/spaces/${e}/discussions`,{method:"HEAD"})).headers.get("x-error-message");return!(t&&ft.test(t))}catch{return!1}}async function _t(e,s){const t={};s&&(t.Authorization=`Bearer ${s}`);try{const o=await fetch(`https://huggingface.co/api/spaces/${e}/${Ge}`,{headers:t});if(o.status!==200)throw new Error("Space hardware could not be obtained.");const{hardware:n}=await o.json();return n.current}catch(o){throw new Error(o.message)}}async function mt(e,s,t){const o={};t&&(o.Authorization=`Bearer ${t}`);const n={seconds:s};try{const i=await fetch(`https://huggingface.co/api/spaces/${e}/${Je}`,{method:"POST",headers:{"Content-Type":"application/json",...o},body:JSON.stringify(n)});if(i.status!==200)throw new Error("Could not set sleep timeout on duplicated Space. Please visit *ADD HF LINK TO SETTINGS* to set a timeout manually to reduce billing charges.");return await i.json()}catch(i){throw new Error(i.message)}}const ge=["cpu-basic","cpu-upgrade","cpu-xl","t4-small","t4-medium","a10g-small","a10g-large","a10g-largex2","a10g-largex4","a100-large","zero-a10g","h100","h100x8"];async function gt(e,s){const{hf_token:t,private:o,hardware:n,timeout:i,auth:r}=s;if(n&&!ge.includes(n))throw new Error(`Invalid hardware type provided. Valid types are: ${ge.map(w=>`"${w}"`).join(",")}.`);const{http_protocol:a,host:u}=await K(e,t);let l=null;if(r){const w=await Ee(a,u,r,fetch);w&&(l=$e(w))}const d={Authorization:`Bearer ${t}`,"Content-Type":"application/json",...l?{Cookie:l.join("; ")}:{}},c=(await(await fetch("https://huggingface.co/api/whoami-v2",{headers:d})).json()).name,p=e.split("/")[1],v={repository:`${c}/${p}`};o&&(v.private=!0);let A;try{n||(A=await _t(e,t))}catch(w){throw Error(ce+w.message)}const f=n||A||"cpu-basic";v.hardware=f;try{const w=await fetch(`https://huggingface.co/api/spaces/${e}/duplicate`,{method:"POST",headers:d,body:JSON.stringify(v)});if(w.status===409)try{return await V.connect(`${c}/${p}`,s)}catch(O){throw console.error("Failed to connect Client instance:",O),O}else if(w.status!==200)throw new Error(w.statusText);const $=await w.json();return await mt(`${c}/${p}`,i||300,t),await V.connect(wt($.url),s)}catch(w){throw new Error(w)}}function wt(e){const s=/https:\/\/huggingface.co\/spaces\/([^/]+\/[^/]+)/,t=e.match(s);if(t)return t[1]}async function yt(){let{event_callbacks:e,unclosed_events:s,pending_stream_messages:t,stream_status:o,config:n,jwt:i}=this;if(!n)throw new Error("Could not resolve app config");o.open=!0;let r=null,a=new URLSearchParams({session_hash:this.session_hash}).toString(),u=new URL(`${n.root}/queue/data?${a}`);if(i&&u.searchParams.set("__sign",i),r=await this.stream(u),!r){console.warn("Cannot connect to SSE endpoint: "+u.toString());return}r.onmessage=async function(l){let d=JSON.parse(l.data);if(d.msg==="close_stream"){H(o,r);return}const c=d.event_id;if(!c)await Promise.all(Object.keys(e).map(p=>e[p](d)));else if(e[c]&&n){d.msg==="process_completed"&&["sse","sse_v1","sse_v2","sse_v2.1"].includes(n.protocol)&&(s.delete(c),s.size===0&&H(o,r));let p=e[c];typeof window<"u"?window.setTimeout(p,0,d):setImmediate(p,d)}else t[c]||(t[c]=[]),t[c].push(d)},r.onerror=async function(){await Promise.all(Object.keys(e).map(l=>e[l]({msg:"unexpected_error",message:F}))),H(o,r)}}function H(e,s){e&&s&&(e.open=!1,s?.close())}function vt(e,s,t){!e[s]?(e[s]=[],t.data.forEach((n,i)=>{e[s][i]=n})):t.data.forEach((n,i)=>{let r=bt(e[s][i],n);e[s][i]=r,t.data[i]=r})}function bt(e,s){return s.forEach(([t,o,n])=>{e=St(e,o,t,n)}),e}function St(e,s,t,o){if(s.length===0){if(t==="replace")return o;if(t==="append")return e+o;throw new Error(`Unsupported action: ${t}`)}let n=e;for(let r=0;r<s.length-1;r++)n=n[s[r]];const i=s[s.length-1];switch(t){case"replace":n[i]=o;break;case"append":n[i]+=o;break;case"add":Array.isArray(n)?n.splice(Number(i),0,o):n[i]=o;break;case"delete":Array.isArray(n)?n.splice(Number(i),1):delete n[i];break;default:throw new Error(`Unknown action: ${t}`)}return e}function Et(e,s,t,o){try{let n=function(y){const k=M[y.type]||[];k?.forEach(g=>g(y))},i=function(y,b){const k=M,g=k[y]||[];return k[y]=g,g?.push(b),{on:i,off:r,cancel:Q,destroy:a}},r=function(y,b){const k=M;let g=k[y]||[];return g=g?.filter(S=>S!==b),k[y]=g,{on:i,off:r,cancel:Q,destroy:a}},a=function(){var y;for(const b in M)M&&((y=M[b])==null||y.forEach(k=>{r(b,k)}))};const{hf_token:u}=this.options,{fetch:l,app_reference:d,config:c,session_hash:p,api_info:v,api_map:A,stream_status:f,pending_stream_messages:w,pending_diff_streams:$,event_callbacks:O,unclosed_events:Te,post_data:Z}=this;if(!v)throw new Error("No API found");if(!c)throw new Error("Could not resolve app config");let{fn_index:h,endpoint_info:Ne,dependency:Pe}=$t(v,e,A,c),De=ot(s,v),R,P,U=c.protocol??"ws";const _=typeof e=="number"?"/predict":e;let J,C=null,T=!1;const M={};let Y={},B=typeof window<"u"?new URLSearchParams(window.location.search).toString():"";async function Q(){const y={stage:"complete",queue:!1,time:new Date};T=y,n({...y,type:"status",endpoint:_,fn_index:h});let b={};U==="ws"?(R&&R.readyState===0?R.addEventListener("open",()=>{R.close()}):R.close(),b={fn_index:h,session_hash:p}):(P?.close(),b={event_id:C});try{if(!c)throw new Error("Could not resolve app config");await l(`${c.root}/reset`,{headers:{"Content-Type":"application/json"},method:"POST",body:JSON.stringify(b)})}catch{console.warn("The `/reset` endpoint could not be called. Subsequent endpoint results may be unreliable.")}}const Le=async y=>{await this._resolve_hearbeat(y)};async function ue(y){if(!c)return;let b=y.render_id;c.components=[...c.components.filter(S=>S.props.rendered_in!==b),...y.components],c.dependencies=[...c.dependencies.filter(S=>S.rendered_in!==b),...y.dependencies];const k=c.components.some(S=>S.type==="state"),g=c.dependencies.some(S=>S.targets.some(L=>L[1]==="unload"));c.connect_heartbeat=k||g,await Le(c),n({type:"render",data:y,endpoint:_,fn_index:h})}return this.handle_blob(c.root,De,Ne).then(async y=>{var b;if(J={data:y||[],event_data:t,fn_index:h,trigger_id:o},lt(h,c))n({type:"status",endpoint:_,stage:"pending",queue:!1,fn_index:h,time:new Date}),Z(`${c.root}/run${_.startsWith("/")?_:`/${_}`}${B?"?"+B:""}`,{...J,session_hash:p}).then(([g,S])=>{const L=g.data;S==200?(n({type:"data",endpoint:_,fn_index:h,data:L,time:new Date,event_data:t,trigger_id:o}),g.render_config&&ue(g.render_config),n({type:"status",endpoint:_,fn_index:h,stage:"complete",eta:g.average_duration,queue:!1,time:new Date})):n({type:"status",stage:"error",endpoint:_,fn_index:h,message:g.error,queue:!1,time:new Date})}).catch(g=>{n({type:"status",stage:"error",message:g.message,endpoint:_,fn_index:h,queue:!1,time:new Date})});else if(U=="ws"){const{ws_protocol:g,host:S}=await K(d,u);n({type:"status",stage:"pending",queue:!0,endpoint:_,fn_index:h,time:new Date});let L=new URL(`${g}://${Se(S,c.path,!0)}/queue/join${B?"?"+B:""}`);this.jwt&&L.searchParams.set("__sign",this.jwt),R=new WebSocket(L),R.onclose=j=>{j.wasClean||n({type:"status",stage:"error",broken:!0,message:F,queue:!0,endpoint:_,fn_index:h,time:new Date})},R.onmessage=function(j){const N=JSON.parse(j.data),{type:D,status:I,data:q}=ne(N,Y[h]);if(D==="update"&&I&&!T)n({type:"status",endpoint:_,fn_index:h,time:new Date,...I}),I.stage==="error"&&R.close();else if(D==="hash"){R.send(JSON.stringify({fn_index:h,session_hash:p}));return}else D==="data"?R.send(JSON.stringify({...J,session_hash:p})):D==="complete"?T=I:D==="log"?n({type:"log",log:q.log,level:q.level,endpoint:_,fn_index:h}):D==="generating"&&n({type:"status",time:new Date,...I,stage:I?.stage,queue:!0,endpoint:_,fn_index:h});q&&(n({type:"data",time:new Date,data:q.data,endpoint:_,fn_index:h,event_data:t,trigger_id:o}),T&&(n({type:"status",time:new Date,...T,stage:I?.stage,queue:!0,endpoint:_,fn_index:h}),R.close()))},ye(c.version||"2.0.0","3.6")<0&&addEventListener("open",()=>R.send(JSON.stringify({hash:p})))}else if(U=="sse"){n({type:"status",stage:"pending",queue:!0,endpoint:_,fn_index:h,time:new Date});var k=new URLSearchParams({fn_index:h.toString(),session_hash:p}).toString();let g=new URL(`${c.root}/queue/join?${B?B+"&":""}${k}`);if(this.jwt&&g.searchParams.set("__sign",this.jwt),P=await this.stream(g),!P)return Promise.reject(new Error("Cannot connect to SSE endpoint: "+g.toString()));P.onmessage=async function(S){const L=JSON.parse(S.data),{type:j,status:N,data:D}=ne(L,Y[h]);if(j==="update"&&N&&!T)n({type:"status",endpoint:_,fn_index:h,time:new Date,...N}),N.stage==="error"&&P?.close();else if(j==="data"){C=L.event_id;let[I,q]=await Z(`${c.root}/queue/data`,{...J,session_hash:p,event_id:C});q!==200&&(n({type:"status",stage:"error",message:F,queue:!0,endpoint:_,fn_index:h,time:new Date}),P?.close())}else j==="complete"?T=N:j==="log"?n({type:"log",log:D.log,level:D.level,endpoint:_,fn_index:h}):j==="generating"&&n({type:"status",time:new Date,...N,stage:N?.stage,queue:!0,endpoint:_,fn_index:h});D&&(n({type:"data",time:new Date,data:D.data,endpoint:_,fn_index:h,event_data:t,trigger_id:o}),T&&(n({type:"status",time:new Date,...T,stage:N?.stage,queue:!0,endpoint:_,fn_index:h}),P?.close()))}}else if(U=="sse_v1"||U=="sse_v2"||U=="sse_v2.1"||U=="sse_v3"){n({type:"status",stage:"pending",queue:!0,endpoint:_,fn_index:h,time:new Date});let g="";typeof window<"u"&&(g=(b=window?.location)==null?void 0:b.hostname);const L=g.includes(".dev.")?`https://moon-${g.split(".")[1]}.dev.spaces.huggingface.tech`:"https://huggingface.co",j=typeof window<"u"&&window.parent!=window,N=Pe.zerogpu&&c.space_id;(j&&N?ut("zerogpu-headers",L):Promise.resolve(null)).then(q=>Z(`${c.root}/queue/join?${B}`,{...J,session_hash:p},q)).then(async([q,pe])=>{if(pe===503)n({type:"status",stage:"error",message:ve,queue:!0,endpoint:_,fn_index:h,time:new Date});else if(pe!==200)n({type:"status",stage:"error",message:F,queue:!0,endpoint:_,fn_index:h,time:new Date});else{C=q.event_id;let he=async function(X){try{const{type:x,status:E,data:z}=ne(X,Y[h]);if(x=="heartbeat")return;if(x==="update"&&E&&!T)n({type:"status",endpoint:_,fn_index:h,time:new Date,...E});else if(x==="complete")T=E;else if(x=="unexpected_error")console.error("Unexpected error",E?.message),n({type:"status",stage:"error",message:E?.message||"An Unexpected Error Occurred!",queue:!0,endpoint:_,fn_index:h,time:new Date});else if(x==="log"){n({type:"log",log:z.log,level:z.level,endpoint:_,fn_index:h});return}else x==="generating"&&(n({type:"status",time:new Date,...E,stage:E?.stage,queue:!0,endpoint:_,fn_index:h}),z&&["sse_v2","sse_v2.1","sse_v3"].includes(U)&&vt($,C,z));z&&(n({type:"data",time:new Date,data:z.data,endpoint:_,fn_index:h}),z.render_config&&await ue(z.render_config),T&&n({type:"status",time:new Date,...T,stage:E?.stage,queue:!0,endpoint:_,fn_index:h})),(E?.stage==="complete"||E?.stage==="error")&&(O[C]&&delete O[C],C in $&&delete $[C])}catch(x){console.error("Unexpected client exception",x),n({type:"status",stage:"error",message:"An Unexpected Error Occurred!",queue:!0,endpoint:_,fn_index:h,time:new Date}),["sse_v2","sse_v2.1"].includes(U)&&(H(f,P),f.open=!1)}};C in w&&(w[C].forEach(X=>he(X)),delete w[C]),O[C]=he,Te.add(C),f.open||await this.open_stream()}})}}),{on:i,off:r,cancel:Q,destroy:a}}catch(n){throw console.error("Submit function encountered an error:",n),n}}function $t(e,s,t,o){let n,i,r;if(typeof s=="number")n=s,i=e.unnamed_endpoints[n],r=o.dependencies.find(a=>a.id==s);else{const a=s.replace(/^\//,"");n=t[a],i=e.named_endpoints[s.trim()],r=o.dependencies.find(u=>u.id==t[a])}if(typeof n!="number")throw new Error("There is no endpoint matching that name of fn_index matching that number.");return{fn_index:n,endpoint_info:i,dependency:r}}class Ce extends Blob{constructor(s,t){super(s,t)}}class V{constructor(s,t={}){m(this,"app_reference"),m(this,"options"),m(this,"config"),m(this,"api_info"),m(this,"api_map",{}),m(this,"session_hash",Math.random().toString(36).substring(2)),m(this,"jwt",!1),m(this,"last_status",{}),m(this,"cookies",null),m(this,"stream_status",{open:!1}),m(this,"pending_stream_messages",{}),m(this,"pending_diff_streams",{}),m(this,"event_callbacks",{}),m(this,"unclosed_events",new Set),m(this,"heartbeat_event",null),m(this,"view_api"),m(this,"upload_files"),m(this,"upload"),m(this,"handle_blob"),m(this,"post_data"),m(this,"submit"),m(this,"predict"),m(this,"open_stream"),m(this,"resolve_config"),m(this,"resolve_cookies"),this.app_reference=s,this.options=t,this.view_api=rt.bind(this),this.upload_files=at.bind(this),this.handle_blob=pt.bind(this),this.post_data=ht.bind(this),this.submit=Et.bind(this),this.predict=dt.bind(this),this.open_stream=yt.bind(this),this.resolve_config=Xe.bind(this),this.resolve_cookies=et.bind(this),this.upload=ct.bind(this)}fetch(s,t){const o=new Headers(t?.headers||{});return this&&this.cookies&&o.append("Cookie",this.cookies),fetch(s,{...t,headers:o})}async stream(s){if(typeof window>"u"||typeof EventSource>"u")try{const t=await W(()=>import("./eventsource-CfQQVHjX.js").then(o=>o.e),__vite__mapDeps([0,1,2,3]),import.meta.url);return new t.default(s.toString())}catch(t){throw console.error("Failed to load EventSource module:",t),t}else return new EventSource(s.toString())}async init(){var s;if((typeof window>"u"||!("WebSocket"in window))&&!global.WebSocket){const t=await W(()=>import("./wrapper-CviSselG-DCvi549i.js"),__vite__mapDeps([4,3]),import.meta.url);Ce=(await W(()=>import("./__vite-browser-external-DFe-p4yY.js").then(o=>o._),[],import.meta.url)).Blob,global.WebSocket=t.WebSocket}try{this.options.auth&&await this.resolve_cookies(),await this._resolve_config().then(({config:t})=>this._resolve_hearbeat(t))}catch(t){throw Error(t)}this.api_info=await this.view_api(),this.api_map=Qe(((s=this.config)==null?void 0:s.dependencies)||[])}async _resolve_hearbeat(s){var t;if(s&&(this.config=s,this.config&&this.config.connect_heartbeat&&this.config.space_id&&this.options.hf_token&&(this.jwt=await fe(this.config.space_id,this.options.hf_token,this.cookies))),s.space_id&&this.options.hf_token&&(this.jwt=await fe(s.space_id,this.options.hf_token)),this.config&&this.config.connect_heartbeat){const o=new URL(`${this.config.root}/heartbeat/${this.session_hash}`);this.jwt&&o.searchParams.set("__sign",this.jwt),this.heartbeat_event||(this.heartbeat_event=await this.stream(o))}else(t=this.heartbeat_event)==null||t.close()}static async connect(s,t={}){const o=new this(s,t);return await o.init(),o}close(){var s;(s=this.heartbeat_event)==null||s.close()}static async duplicate(s,t={}){return gt(s,t)}async _resolve_config(){const{http_protocol:s,host:t,space_id:o}=await K(this.app_reference,this.options.hf_token),{status_callback:n}=this.options;let i;try{if(i=await this.resolve_config(`${s}//${t}`),!i)throw new Error(G);return this.config_success(i)}catch(r){if(o&&n)oe(o,ke.test(o)?"space_name":"subdomain",this.handle_space_success);else throw n&&n({status:"error",message:"Could not load this space.",load_status:"error",detail:"NOT_FOUND"}),Error(r)}}async config_success(s){if(this.config=s,typeof window<"u"&&window.location.protocol==="https:"&&(this.config.root=this.config.root.replace("http://","https://")),this.config.auth_required)return this.prepare_return_obj();try{this.api_info=await this.view_api()}catch(t){console.error(Ve+t.message)}return this.prepare_return_obj()}async handle_space_success(s){if(!this)throw new Error(G);const{status_callback:t}=this.options;if(t&&t(s),s.status==="running")try{if(this.config=await this._resolve_config(),!this.config)throw new Error(G);return await this.config_success(this.config)}catch(o){throw t&&t({status:"error",message:"Could not load this space.",load_status:"error",detail:"NOT_FOUND"}),o}}async component_server(s,t,o){var n;if(!this.config)throw new Error(G);const i={},{hf_token:r}=this.options,{session_hash:a}=this;r&&(i.Authorization=`Bearer ${this.options.hf_token}`);let u,l=this.config.components.find(c=>c.id===s);(n=l?.props)!=null&&n.root_url?u=l.props.root_url:u=this.config.root;let d;if("binary"in o){d=new FormData;for(const c in o.data)c!=="binary"&&d.append(c,o.data[c]);d.set("component_id",s.toString()),d.set("fn_name",t),d.set("session_hash",a)}else d=JSON.stringify({data:o,component_id:s,fn_name:t,session_hash:a}),i["Content-Type"]="application/json";r&&(i.Authorization=`Bearer ${r}`);try{const c=await this.fetch(`${u}/component_server/`,{method:"POST",body:d,headers:i,credentials:"include"});if(!c.ok)throw new Error("Could not connect to component server: "+c.statusText);return await c.json()}catch(c){console.warn(c)}}set_cookies(s){this.cookies=$e(s).join("; ")}prepare_return_obj(){return{config:this.config,predict:this.predict,submit:this.submit,view_api:this.view_api,component_server:this.component_server}}}let Oe=!1;"attachShadow"in Element.prototype&&"adoptedStyleSheets"in Document.prototype&&(Oe="adoptedStyleSheets"in document.createElement("div").attachShadow({mode:"open"}));function we(e,s){const t=new URL(import.meta.url).origin,o=new URL(e,t).href;if(document.querySelector(`link[href='${o}']`))return Promise.resolve();const i=document.createElement("link");return i.rel="stylesheet",i.href=o,new Promise((r,a)=>{i.addEventListener("load",()=>r()),i.addEventListener("error",()=>{console.error(`Unable to preload CSS for ${o}`),r()}),s.appendChild(i)})}function Tt(e,s,t=document.createElement("style")){if(!Oe)return null;t.remove();const o=new CSSStyleSheet;o.replaceSync(e);let n="";e=e.replace(/@import\s+url\((.*?)\);\s*/g,(u,l)=>(n+=`@import url(${l});
`,""));const i=o.cssRules;let r="",a=`gradio-app .gradio-container.gradio-container-${s} .contain `;for(let u=0;u<i.length;u++){const l=i[u];let d=l.cssText.includes(".dark");if(l instanceof CSSStyleRule){const c=l.selectorText;if(c){const p=c.replace(".dark","").split(",").map(v=>`${d?".dark":""} ${a} ${v.trim()} `).join(",");r+=l.cssText,r+=l.cssText.replace(c,p)}}else if(l instanceof CSSMediaRule){let c=`@media ${l.media.mediaText} {`;for(let p=0;p<l.cssRules.length;p++){const v=l.cssRules[p];if(v instanceof CSSStyleRule){let A=v.cssText.includes(".dark ");const f=v.selectorText,w=f.replace(".dark","").split(",").map($=>`${A?".dark":""} ${a} ${$.trim()} `).join(",");c+=v.cssText.replace(f,w)}}c+="}",r+=c}else if(l instanceof CSSKeyframesRule){r+=`@keyframes ${l.name} {`;for(let c=0;c<l.cssRules.length;c++){const p=l.cssRules[c];p instanceof CSSKeyframeRule&&(r+=`${p.keyText} { ${p.style.cssText} }`)}r+="}"}else l instanceof CSSFontFaceRule&&(r+=`@font-face { ${l.style.cssText} }`)}return r=n+r,t.textContent=r,document.head.appendChild(t),t}const kt="./assets/index-DeRUbA-x.css";let re;re=[];let ae,Re,At=new Promise(e=>{Re=e});async function Ct(){ae=(await W(()=>import("./Index-C_XES7MU.js").then(e=>e.f),__vite__mapDeps([5,1,6]),import.meta.url)).default,Re()}function Ot(){const e={SvelteComponent:ee.SvelteComponent};for(const t in ee)t!=="SvelteComponent"&&(t==="SvelteComponentDev"?e[t]=e.SvelteComponent:e[t]=ee[t]);window.__gradio__svelte__internal=e;class s extends HTMLElement{constructor(){super(),this.host=this.getAttribute("host"),this.space=this.getAttribute("space"),this.src=this.getAttribute("src"),this.control_page_title=this.getAttribute("control_page_title"),this.initial_height=this.getAttribute("initial_height")??"300px",this.is_embed=this.getAttribute("embed")??"true",this.container=this.getAttribute("container")??"true",this.info=this.getAttribute("info")??!0,this.autoscroll=this.getAttribute("autoscroll"),this.eager=this.getAttribute("eager"),this.theme_mode=this.getAttribute("theme_mode"),this.updating=!1,this.loading=!1}async connectedCallback(){await Ct(),this.loading=!0,this.app&&this.app.$destroy(),typeof re!="string"&&re.forEach(i=>we(i,document.head)),await we(kt,document.head);const o=new CustomEvent("domchange",{bubbles:!0,cancelable:!1,composed:!0});new MutationObserver(i=>{this.dispatchEvent(o)}).observe(this,{childList:!0}),this.app=new ae({target:this,props:{space:this.space?this.space.trim():this.space,src:this.src?this.src.trim():this.src,host:this.host?this.host.trim():this.host,info:this.info!=="false",container:this.container!=="false",is_embed:this.is_embed!=="false",initial_height:this.initial_height,eager:this.eager==="true",version:"4-32-2",theme_mode:this.theme_mode,autoscroll:this.autoscroll==="true",control_page_title:this.control_page_title==="true",Client:V,app_mode:window.__gradio_mode__==="app"}}),this.updating&&this.setAttribute(this.updating.name,this.updating.value),this.loading=!1}static get observedAttributes(){return["src","space","host"]}async attributeChangedCallback(o,n,i){if(await At,(o==="host"||o==="space"||o==="src")&&i!==n){if(this.updating={name:o,value:i},this.loading)return;this.app&&this.app.$destroy(),this.space=null,this.host=null,this.src=null,o==="host"?this.host=i:o==="space"?this.space=i:o==="src"&&(this.src=i),this.app=new ae({target:this,props:{space:this.space?this.space.trim():this.space,src:this.src?this.src.trim():this.src,host:this.host?this.host.trim():this.host,info:this.info!=="false",container:this.container!=="false",is_embed:this.is_embed!=="false",initial_height:this.initial_height,eager:this.eager==="true",version:"4-32-2",theme_mode:this.theme_mode,autoscroll:this.autoscroll==="true",control_page_title:this.control_page_title==="true",Client:V,app_mode:window.__gradio_mode__==="app"}}),this.updating=!1}}}customElements.get("gradio-app")||customElements.define("gradio-app",s)}Ot();export{W as _,Tt as a,we as m,Rt as p};
//# sourceMappingURL=index-CmhFtYKU.js.map