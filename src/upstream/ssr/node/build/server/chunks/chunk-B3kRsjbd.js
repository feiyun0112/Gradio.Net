import { createRequire } from 'node:module';

var t=Object.create,n=Object.defineProperty,r=Object.getOwnPropertyDescriptor,i=Object.getOwnPropertyNames,a=Object.getPrototypeOf,o=Object.prototype.hasOwnProperty,s=(e,t)=>()=>(e&&(t=e(e=0)),t),c=(e,t)=>()=>(t||(e((t={exports:{}}).exports,t),e=null),t.exports),l=(e,t)=>{let r={};for(var i in e)n(r,i,{get:e[i],enumerable:true});return n(r,Symbol.toStringTag,{value:`Module`}),r},u=(e,t,a,s)=>{if(t&&typeof t==`object`||typeof t==`function`)for(var c=i(t),l=0,u=c.length,d;l<u;l++)d=c[l],!o.call(e,d)&&d!==a&&n(e,d,{get:(e=>t[e]).bind(null,d),enumerable:!(s=r(t,d))||s.enumerable});return e},d=(e,r,i)=>(i=e==null?{}:t(a(e)),u(n(i,`default`,{value:e,enumerable:true}),e)),f=e=>o.call(e,`module.exports`)?e[`module.exports`]:u(n({},`__esModule`,{value:true}),e),p=createRequire(import.meta.url);

export { c, d, f, l, p, s };
//# sourceMappingURL=chunk-B3kRsjbd.js.map
