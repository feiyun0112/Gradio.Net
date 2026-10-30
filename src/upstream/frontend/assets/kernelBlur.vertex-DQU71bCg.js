import{t as e}from"./shaderStore-D-XQlhUT.js";import"./kernelBlurVaryingDeclaration-DSCV0eGX.js";var t=`kernelBlurVertex`,n=`sampleCoord{X}=sampleCenter+delta*KERNEL_OFFSET{X};`;e.IncludesShadersStore[t]||(e.IncludesShadersStore[t]=n);var r=`kernelBlurVertexShader`,i=`attribute vec2 position;uniform vec2 delta;varying vec2 sampleCenter;
#include<kernelBlurVaryingDeclaration>[0..varyingCount]
const vec2 madd=vec2(0.5,0.5);
#define CUSTOM_VERTEX_DEFINITIONS
void main(void) {
#define CUSTOM_VERTEX_MAIN_BEGIN
sampleCenter=(position*madd+madd);
#include<kernelBlurVertex>[0..varyingCount]
gl_Position=vec4(position,0.0,1.0);
#define CUSTOM_VERTEX_MAIN_END
}`;e.ShadersStore[r]||(e.ShadersStore[r]=i);var a={name:r,shader:i};export{a as kernelBlurVertexShader};