import{t as e}from"./shaderStore-D-XQlhUT.js";import"./helperFunctions-CM1SYQ_G.js";var t=`rgbdDecodePixelShader`,n=`varying vec2 vUV;uniform sampler2D textureSampler;
#include<helperFunctions>
#define CUSTOM_FRAGMENT_DEFINITIONS
void main(void) 
{gl_FragColor=vec4(fromRGBD(texture2D(textureSampler,vUV)),1.0);}`;e.ShadersStore[t]||(e.ShadersStore[t]=n);var r={name:t,shader:n};export{r as rgbdDecodePixelShader};