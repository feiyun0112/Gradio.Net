import './2-C9eYyMgE.js';
import './Index17-Dg3Uj9lt.js';
import { d } from './Index42-Czg-D_x9.js';
import { b as attr_style, i as stringify, a as attr_class, h as ensure_array_like, e as escape_html, f as attr } from './renderer-RGQlaTg4.js';
import './async-Cv1-GZGV.js';
import './environment-BbsaTwvI.js';
import './chunk-B3kRsjbd.js';
import 'node:module';
import './statustracker-Bh1XNgjP.js';
import './src3-C1zEyqUA.js';
import './html-CfyvkLET.js';
import './server-Ck07o0bn.js';
import './markdown-code-DbI7wxXO.js';

function r(r,i){r.component(r=>{let {app:a,root:o}=i,s=350,l=[],u=[];(async()=>{a.post_data(`${o}/gradio_api/vibe-starter-queries/`,{}).then(async([e,t])=>{if(t!==200)throw Error(`Error: ${t}`);u=e.starter_queries;}).catch(async e=>{console.error(`Failed to fetch starter queries:`,e);});})();let p=true,m;function h(t){t.push(`<div class="vibe-editor svelte-1s2fnws"${attr_style(`width: ${stringify(s)}px;`)}><button class="resize-handle svelte-1s2fnws" aria-label="Resize sidebar"></button> <div class="tab-header svelte-1s2fnws"><button${attr_class(`tab-button svelte-1s2fnws`,void 0,{active:true})}>Chat</button> <button${attr_class(`tab-button svelte-1s2fnws`,void 0,{active:false})}>Code `),t.push(`<!--[-1-->`),t.push(`<!--]--></button></div> <div class="tab-content svelte-1s2fnws">`);{t.push(`<!--[0-->`),t.push(`<div class="message-history svelte-1s2fnws"><!--[-->`);let r=ensure_array_like(l);for(let i=0,a=r.length;i<a;i++){let a=r[i];t.push(`<div${attr_class(`message-item svelte-1s2fnws`,void 0,{"bot-message":a.isBot,"user-message":!a.isBot})}><div class="message-content svelte-1s2fnws"><span class="message-text svelte-1s2fnws">`),d(t,{value:a.text,latex_delimiters:[],theme_mode:`system`}),t.push(`<!----></span> `),!a.isBot&&a.hash&&!a.isPending?(t.push(`<!--[0-->`),t.push(`<button class="undo-button svelte-1s2fnws" title="Undo this change">Undo</button>`)):t.push(`<!--[-1-->`),t.push(`<!--]--></div></div>`);}if(t.push(`<!--]--> `),l.length===0?(t.push(`<!--[0-->`),t.push(`<div class="no-messages svelte-1s2fnws">No messages yet</div>`)):t.push(`<!--[-1-->`),t.push(`<!--]--> `),l.length===0){t.push(`<!--[0-->`),t.push(`<div class="starter-queries-container svelte-1s2fnws"><div class="starter-queries svelte-1s2fnws"><!--[-->`);let e=ensure_array_like(u);for(let r=0,i=e.length;r<i;r++){let i=e[r];t.push(`<button class="starter-query-button svelte-1s2fnws">${escape_html(i)}</button>`);}t.push(`<!--]--></div></div>`);}else t.push(`<!--[-1-->`);t.push(`<!--]--></div>`);}t.push(`<!--]--></div> <div class="input-section svelte-1s2fnws"><div class="powered-by svelte-1s2fnws">Powered by: <code>gpt-oss</code></div> <textarea placeholder="What can I add or change?" class="prompt-input svelte-1s2fnws">`);let r=escape_html(``);r&&t.push(`${r}`),t.push(`</textarea> <button class="submit-button svelte-1s2fnws"${attr(`disabled`,true,true)}>Send</button></div></div>`);}do p=true,m=r.copy(),h(m);while(!p);r.subsume(m);});}

export { r as default };
//# sourceMappingURL=Index61-DMdWV2qR.js.map
