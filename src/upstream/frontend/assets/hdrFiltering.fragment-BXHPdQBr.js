import{t as e}from"./shaderStore-D-XQlhUT.js";import"./helperFunctions-CM1SYQ_G.js";import"./hdrFilteringFunctions-BK6JpkMG.js";import"./pbrBRDFFunctions-C40dSicq.js";var t=`hdrFilteringPixelShader`,n=`#include<helperFunctions>
#include<importanceSampling>
#include<pbrBRDFFunctions>
#include<hdrFilteringFunctions>
uniform float alphaG;uniform samplerCube inputTexture;uniform vec2 vFilteringInfo;uniform float hdrScale;varying vec3 direction;void main() {vec3 color=radiance(alphaG,inputTexture,direction,vFilteringInfo);gl_FragColor=vec4(color*hdrScale,1.0);}`;e.ShadersStore[t]||(e.ShadersStore[t]=n);var r={name:t,shader:n};export{r as hdrFilteringPixelShader};