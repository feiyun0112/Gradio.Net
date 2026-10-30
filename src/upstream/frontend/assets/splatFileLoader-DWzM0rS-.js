import{gn as e}from"./index-D88pgWqG.js";import{n as t}from"./typeStore-Cu_sj2zM.js";import{t as n}from"./engineStore-VHTr1-03.js";import{r,s as i,u as a}from"./math.scalar.functions-Dhej7fET.js";import{a as o,i as s,n as c,o as l,r as u,t as d}from"./math.vector-BxfzvQ1g.js";import{n as f,t as p}from"./math.color-B-P5_lZD.js";import{t as m}from"./decorators.serialization-4F1gbVII.js";import{t as h}from"./logger-CuBYPcDK.js";import{n as g}from"./tools-DAjdSQwQ.js";import{d as _}from"./tools.functions-VA-Es30k.js";import{t as v}from"./shaderStore-D-XQlhUT.js";import{t as y}from"./guid-BXdVC_j_.js";import{n as b}from"./buffer-RYS_o-MH.js";import{t as x}from"./camera-DKN4yECi.js";import{n as S}from"./ray-BB5FY8YQ.js";import"./engine-D-MoFWbP.js";import"./helperFunctions-DquCMwzt.js";import"./meshUboDeclaration-CmIQ_8hI.js";import"./sceneUboDeclaration-TbZ5RU0g.js";import{t as C}from"./baseTexture-CDiana7Y.js";import{t as w}from"./drawWrapper-oCi5T3b9.js";import{r as T}from"./textureTools-DHmwIys2.js";import{a as E,i as D,o as ee,r as O}from"./abstractMesh-ZOvRffef.js";import{t as te}from"./subMesh-IhIGZKiz.js";import{A as ne,E as re,O as ie,b as ae,f as oe,g as se,h as ce,j as le,n as ue}from"./materialHelper.functions-DLwUvPy9.js";import"./clipPlaneFragment-CSA2ud-B.js";import"./clipPlaneVertex-CGSrwNQT.js";import{o as de}from"./imageProcessing-DUXc4oco.js";import{t as fe}from"./pushMaterial-D9rKEe0I.js";import{t as k}from"./rawTexture-YTwKCvql.js";import{t as pe}from"./shaderMaterial-G7uWMkQP.js";import{t as A}from"./mesh-Crxa-pY7.js";import"./helperFunctions-CM1SYQ_G.js";import"./sceneUboDeclaration-CR5dQRQU.js";import{t as me}from"./standardMaterial-DQwewOnl.js";import"./logDepthDeclaration--laj1kHT.js";import"./fogFragment-D_7zXuQT.js";import"./logDepthVertex-Dj05gqZS.js";import"./meshUboDeclaration-D9VmL7M6.js";import"./logDepthDeclaration-B8O1rFmO.js";import"./clipPlaneFragment-DGqZJNeS.js";import"./fogFragment-DT_vR29_.js";import"./clipPlaneVertex-gMyNlLVT.js";import"./logDepthVertex-B7eOEYaq.js";import{r as he,t as ge}from"./sceneLoader-hzLrTDct.js";import{t as _e}from"./assetContainer-DerrFhf_.js";import{t as j}from"./splatFileLoader.metadata-C_pWpMDJ.js";import"./thinInstanceMesh--x-cCgPc.js";var ve=`gaussianSplattingFragmentDeclaration`,ye=`vec4 gaussianColor(vec4 inColor)
{float A=-dot(vPosition,vPosition);if (A<-4.0) discard;float B=exp(A)*inColor.a;
#include<logDepthFragment>
vec3 color=inColor.rgb;
#ifdef FOG
#include<fogFragment>
#endif
return vec4(color,B);}
`;v.IncludesShadersStore[ve]||(v.IncludesShadersStore[ve]=ye);var M=`gaussianSplattingPixelShader`,N=`#include<clipPlaneFragmentDeclaration>
#include<logDepthDeclaration>
#include<fogFragmentDeclaration>
varying vec4 vColor;varying vec2 vPosition;
#include<gaussianSplattingFragmentDeclaration>
void main () { 
#include<clipPlaneFragment>
gl_FragColor=gaussianColor(vColor);}
`;v.ShadersStore[M]||(v.ShadersStore[M]=N);var be={name:M,shader:N},P=`gaussianSplattingVertexDeclaration`,xe=`attribute vec3 position;attribute vec4 splatIndex0;attribute vec4 splatIndex1;attribute vec4 splatIndex2;attribute vec4 splatIndex3;uniform mat4 view;uniform mat4 projection;uniform mat4 world;uniform vec4 vEyePosition;`;v.IncludesShadersStore[P]||(v.IncludesShadersStore[P]=xe);var Se=`gaussianSplattingUboDeclaration`,Ce=`#include<sceneUboDeclaration>
#include<meshUboDeclaration>
attribute vec3 position;attribute vec4 splatIndex0;attribute vec4 splatIndex1;attribute vec4 splatIndex2;attribute vec4 splatIndex3;
`;v.IncludesShadersStore[Se]||(v.IncludesShadersStore[Se]=Ce);var F=`gaussianSplatting`,we=`#if !defined(WEBGL2) && !defined(WEBGPU) && !defined(NATIVE)
mat3 transpose(mat3 matrix) {return mat3(matrix[0][0],matrix[1][0],matrix[2][0],
matrix[0][1],matrix[1][1],matrix[2][1],
matrix[0][2],matrix[1][2],matrix[2][2]);}
#endif
vec2 getDataUV(float index,vec2 textureSize) {float y=floor(index/textureSize.x);float x=index-y*textureSize.x;return vec2((x+0.5)/textureSize.x,(y+0.5)/textureSize.y);}
#if SH_DEGREE>0
ivec2 getDataUVint(float index,vec2 textureSize) {float y=floor(index/textureSize.x);float x=index-y*textureSize.x;return ivec2(uint(x+0.5),uint(y+0.5));}
#endif
struct Splat {vec4 center;vec4 color;vec4 covA;vec4 covB;
#if SH_DEGREE>0
uvec4 sh0; 
#endif
#if SH_DEGREE>1
uvec4 sh1;
#endif
#if SH_DEGREE>2
uvec4 sh2;
#endif
};float getSplatIndex(int localIndex)
{float splatIndex;switch (localIndex)
{case 0: splatIndex=splatIndex0.x; break;case 1: splatIndex=splatIndex0.y; break;case 2: splatIndex=splatIndex0.z; break;case 3: splatIndex=splatIndex0.w; break;case 4: splatIndex=splatIndex1.x; break;case 5: splatIndex=splatIndex1.y; break;case 6: splatIndex=splatIndex1.z; break;case 7: splatIndex=splatIndex1.w; break;case 8: splatIndex=splatIndex2.x; break;case 9: splatIndex=splatIndex2.y; break;case 10: splatIndex=splatIndex2.z; break;case 11: splatIndex=splatIndex2.w; break;case 12: splatIndex=splatIndex3.x; break;case 13: splatIndex=splatIndex3.y; break;case 14: splatIndex=splatIndex3.z; break;case 15: splatIndex=splatIndex3.w; break;}
return splatIndex;}
Splat readSplat(float splatIndex)
{Splat splat;vec2 splatUV=getDataUV(splatIndex,dataTextureSize);splat.center=texture2D(centersTexture,splatUV);splat.color=texture2D(colorsTexture,splatUV);splat.covA=texture2D(covariancesATexture,splatUV)*splat.center.w;splat.covB=texture2D(covariancesBTexture,splatUV)*splat.center.w;
#if SH_DEGREE>0
ivec2 splatUVint=getDataUVint(splatIndex,dataTextureSize);splat.sh0=texelFetch(shTexture0,splatUVint,0);
#endif
#if SH_DEGREE>1
splat.sh1=texelFetch(shTexture1,splatUVint,0);
#endif
#if SH_DEGREE>2
splat.sh2=texelFetch(shTexture2,splatUVint,0);
#endif
return splat;}
#if defined(WEBGL2) || defined(WEBGPU) || defined(NATIVE)
vec3 computeColorFromSHDegree(vec3 dir,const vec3 sh[16])
{const float SH_C0=0.28209479;const float SH_C1=0.48860251;float SH_C2[5];SH_C2[0]=1.092548430;SH_C2[1]=-1.09254843;SH_C2[2]=0.315391565;SH_C2[3]=-1.09254843;SH_C2[4]=0.546274215;float SH_C3[7];SH_C3[0]=-0.59004358;SH_C3[1]=2.890611442;SH_C3[2]=-0.45704579;SH_C3[3]=0.373176332;SH_C3[4]=-0.45704579;SH_C3[5]=1.445305721;SH_C3[6]=-0.59004358;vec3 result=/*SH_C0**/sh[0];
#if SH_DEGREE>0
float x=dir.x;float y=dir.y;float z=dir.z;result+=- SH_C1*y*sh[1]+SH_C1*z*sh[2]-SH_C1*x*sh[3];
#if SH_DEGREE>1
float xx=x*x,yy=y*y,zz=z*z;float xy=x*y,yz=y*z,xz=x*z;result+=
SH_C2[0]*xy*sh[4] +
SH_C2[1]*yz*sh[5] +
SH_C2[2]*(2.0*zz-xx-yy)*sh[6] +
SH_C2[3]*xz*sh[7] +
SH_C2[4]*(xx-yy)*sh[8];
#if SH_DEGREE>2
result+=
SH_C3[0]*y*(3.0*xx-yy)*sh[9] +
SH_C3[1]*xy*z*sh[10] +
SH_C3[2]*y*(4.0*zz-xx-yy)*sh[11] +
SH_C3[3]*z*(2.0*zz-3.0*xx-3.0*yy)*sh[12] +
SH_C3[4]*x*(4.0*zz-xx-yy)*sh[13] +
SH_C3[5]*z*(xx-yy)*sh[14] +
SH_C3[6]*x*(xx-3.0*yy)*sh[15];
#endif
#endif
#endif
return result;}
vec4 decompose(uint value)
{vec4 components=vec4(
float((value ) & 255u),
float((value>>uint( 8)) & 255u),
float((value>>uint(16)) & 255u),
float((value>>uint(24)) & 255u));return components*vec4(2./255.)-vec4(1.);}
vec3 computeSH(Splat splat,vec3 dir)
{vec3 sh[16];sh[0]=vec3(0.,0.,0.);
#if SH_DEGREE>0
vec4 sh00=decompose(splat.sh0.x);vec4 sh01=decompose(splat.sh0.y);vec4 sh02=decompose(splat.sh0.z);sh[1]=vec3(sh00.x,sh00.y,sh00.z);sh[2]=vec3(sh00.w,sh01.x,sh01.y);sh[3]=vec3(sh01.z,sh01.w,sh02.x);
#endif
#if SH_DEGREE>1
vec4 sh03=decompose(splat.sh0.w);vec4 sh04=decompose(splat.sh1.x);vec4 sh05=decompose(splat.sh1.y);sh[4]=vec3(sh02.y,sh02.z,sh02.w);sh[5]=vec3(sh03.x,sh03.y,sh03.z);sh[6]=vec3(sh03.w,sh04.x,sh04.y);sh[7]=vec3(sh04.z,sh04.w,sh05.x);sh[8]=vec3(sh05.y,sh05.z,sh05.w);
#endif
#if SH_DEGREE>2
vec4 sh06=decompose(splat.sh1.z);vec4 sh07=decompose(splat.sh1.w);vec4 sh08=decompose(splat.sh2.x);vec4 sh09=decompose(splat.sh2.y);vec4 sh10=decompose(splat.sh2.z);vec4 sh11=decompose(splat.sh2.w);sh[9]=vec3(sh06.x,sh06.y,sh06.z);sh[10]=vec3(sh06.w,sh07.x,sh07.y);sh[11]=vec3(sh07.z,sh07.w,sh08.x);sh[12]=vec3(sh08.y,sh08.z,sh08.w);sh[13]=vec3(sh09.x,sh09.y,sh09.z);sh[14]=vec3(sh09.w,sh10.x,sh10.y);sh[15]=vec3(sh10.z,sh10.w,sh11.x); 
#endif
return computeColorFromSHDegree(dir,sh);}
#else
vec3 computeSH(Splat splat,vec3 dir)
{return vec3(0.,0.,0.);}
#endif
vec4 gaussianSplatting(vec2 meshPos,vec3 worldPos,vec2 scale,vec3 covA,vec3 covB,mat4 worldMatrix,mat4 viewMatrix,mat4 projectionMatrix)
{mat4 modelView=viewMatrix*worldMatrix;vec4 camspace=viewMatrix*vec4(worldPos,1.);vec4 pos2d=projectionMatrix*camspace;float bounds=1.2*pos2d.w;if (pos2d.z<-pos2d.w || pos2d.x<-bounds || pos2d.x>bounds
|| pos2d.y<-bounds || pos2d.y>bounds) {return vec4(0.0,0.0,2.0,1.0);}
mat3 Vrk=mat3(
covA.x,covA.y,covA.z,
covA.y,covB.x,covB.y,
covA.z,covB.y,covB.z
);bool isOrtho=abs(projectionMatrix[3][3]-1.0)<0.001;mat3 J;if (isOrtho) {J=mat3(
focal.x,0.,0.,
0.,focal.y,0.,
0.,0.,0.
);} else {J=mat3(
focal.x/camspace.z,0.,-(focal.x*camspace.x)/(camspace.z*camspace.z),
0.,focal.y/camspace.z,-(focal.y*camspace.y)/(camspace.z*camspace.z),
0.,0.,0.
);}
mat3 T=transpose(mat3(modelView))*J;mat3 cov2d=transpose(T)*Vrk*T;
#if COMPENSATION
float c00=cov2d[0][0];float c11=cov2d[1][1];float c01=cov2d[0][1];float detOrig=c00*c11-c01*c01;
#endif
cov2d[0][0]+=kernelSize;cov2d[1][1]+=kernelSize;
#if COMPENSATION
vec3 c2d=vec3(cov2d[0][0],c01,cov2d[1][1]);float detBlur=c2d.x*c2d.z-c2d.y*c2d.y;float compensation=sqrt(max(0.,detOrig/detBlur));vColor.w*=compensation;
#endif
float mid=(cov2d[0][0]+cov2d[1][1])/2.0;float radius=length(vec2((cov2d[0][0]-cov2d[1][1])/2.0,cov2d[0][1]));float epsilon=0.0001;float lambda1=mid+radius+epsilon,lambda2=mid-radius+epsilon;if (lambda2<0.0)
{return vec4(0.0,0.0,2.0,1.0);}
vec2 diagonalVector=normalize(vec2(cov2d[0][1],lambda1-cov2d[0][0]));vec2 majorAxis=min(sqrt(2.0*lambda1),1024.0)*diagonalVector;vec2 minorAxis=min(sqrt(2.0*lambda2),1024.0)*vec2(diagonalVector.y,-diagonalVector.x);vec2 vCenter=vec2(pos2d);float scaleFactor=isOrtho ? 1.0 : pos2d.w;return vec4(
vCenter 
+ ((meshPos.x*majorAxis
+ meshPos.y*minorAxis)*invViewport*scaleFactor)*scale,pos2d.zw);}`;v.IncludesShadersStore[F]||(v.IncludesShadersStore[F]=we);var I=`gaussianSplattingVertexShader`,Te=`#include<__decl__gaussianSplattingVertex>
#ifdef LOGARITHMICDEPTH
#extension GL_EXT_frag_depth : enable
#endif
#include<clipPlaneVertexDeclaration>
#include<fogVertexDeclaration>
#include<logDepthDeclaration>
#include<helperFunctions>
uniform vec2 invViewport;uniform vec2 dataTextureSize;uniform vec2 focal;uniform float kernelSize;uniform vec3 eyePosition;uniform float alpha;uniform sampler2D covariancesATexture;uniform sampler2D covariancesBTexture;uniform sampler2D centersTexture;uniform sampler2D colorsTexture;
#if SH_DEGREE>0
uniform highp usampler2D shTexture0;
#endif
#if SH_DEGREE>1
uniform highp usampler2D shTexture1;
#endif
#if SH_DEGREE>2
uniform highp usampler2D shTexture2;
#endif
varying vec4 vColor;varying vec2 vPosition;
#include<gaussianSplatting>
void main () {float splatIndex=getSplatIndex(int(position.z+0.5));Splat splat=readSplat(splatIndex);vec3 covA=splat.covA.xyz;vec3 covB=vec3(splat.covA.w,splat.covB.xy);vec4 worldPos=world*vec4(splat.center.xyz,1.0);vColor=splat.color;vPosition=position.xy;
#if SH_DEGREE>0
mat3 worldRot=mat3(world);mat3 normWorldRot=inverseMat3(worldRot);vec3 eyeToSplatLocalSpace=normalize(normWorldRot*(worldPos.xyz-eyePosition));vColor.xyz=splat.color.xyz+computeSH(splat,eyeToSplatLocalSpace);
#endif
vColor.w*=alpha;gl_Position=gaussianSplatting(position.xy,worldPos.xyz,vec2(1.,1.),covA,covB,world,view,projection);
#include<clipPlaneVertex>
#include<fogVertex>
#include<logDepthVertex>
}
`;v.ShadersStore[I]||(v.ShadersStore[I]=Te);var Ee={name:I,shader:Te},L=`gaussianSplattingFragmentDeclaration`,De=`fn gaussianColor(inColor: vec4f,inPosition: vec2f)->vec4f
{var A : f32=-dot(inPosition,inPosition);if (A>-4.0)
{var B: f32=exp(A)*inColor.a;
#include<logDepthFragment>
var color: vec3f=inColor.rgb;
#ifdef FOG
#include<fogFragment>
#endif
return vec4f(color,B);} else {return vec4f(0.0);}}
`;v.IncludesShadersStoreWGSL[L]||(v.IncludesShadersStoreWGSL[L]=De);var R=`gaussianSplattingPixelShader`,Oe=`#include<clipPlaneFragmentDeclaration>
#include<logDepthDeclaration>
#include<fogFragmentDeclaration>
varying vColor: vec4f;varying vPosition: vec2f;
#include<gaussianSplattingFragmentDeclaration>
@fragment
fn main(input: FragmentInputs)->FragmentOutputs {
#include<clipPlaneFragment>
fragmentOutputs.color=gaussianColor(input.vColor,input.vPosition);}
`;v.ShadersStoreWGSL[R]||(v.ShadersStoreWGSL[R]=Oe);var ke={name:R,shader:Oe},z=`gaussianSplatting`,B=`fn getDataUV(index: f32,dataTextureSize: vec2f)->vec2<f32> {let y: f32=floor(index/dataTextureSize.x);let x: f32=index-y*dataTextureSize.x;return vec2f((x+0.5),(y+0.5));}
struct Splat {center: vec4f,
color: vec4f,
covA: vec4f,
covB: vec4f,
#if SH_DEGREE>0
sh0: vec4<u32>,
#endif
#if SH_DEGREE>1
sh1: vec4<u32>,
#endif
#if SH_DEGREE>2
sh2: vec4<u32>,
#endif
};fn getSplatIndex(localIndex: i32,splatIndex0: vec4f,splatIndex1: vec4f,splatIndex2: vec4f,splatIndex3: vec4f)->f32 {var splatIndex: f32;switch (localIndex)
{case 0:
{splatIndex=splatIndex0.x;break;}
case 1:
{splatIndex=splatIndex0.y;break;}
case 2:
{splatIndex=splatIndex0.z;break;}
case 3:
{splatIndex=splatIndex0.w;break;}
case 4:
{splatIndex=splatIndex1.x;break;}
case 5:
{splatIndex=splatIndex1.y;break;}
case 6:
{splatIndex=splatIndex1.z;break;}
case 7:
{splatIndex=splatIndex1.w;break;}
case 8:
{splatIndex=splatIndex2.x;break;}
case 9:
{splatIndex=splatIndex2.y;break;}
case 10:
{splatIndex=splatIndex2.z;break;}
case 11:
{splatIndex=splatIndex2.w;break;}
case 12:
{splatIndex=splatIndex3.x;break;}
case 13:
{splatIndex=splatIndex3.y;break;}
case 14:
{splatIndex=splatIndex3.z;break;}
default:
{splatIndex=splatIndex3.w;break;}}
return splatIndex;}
fn readSplat(splatIndex: f32,dataTextureSize: vec2f)->Splat {var splat: Splat;let splatUV=getDataUV(splatIndex,dataTextureSize);let splatUVi32=vec2<i32>(i32(splatUV.x),i32(splatUV.y));splat.center=textureLoad(centersTexture,splatUVi32,0);splat.color=textureLoad(colorsTexture,splatUVi32,0);splat.covA=textureLoad(covariancesATexture,splatUVi32,0)*splat.center.w;splat.covB=textureLoad(covariancesBTexture,splatUVi32,0)*splat.center.w;
#if SH_DEGREE>0
splat.sh0=textureLoad(shTexture0,splatUVi32,0);
#endif
#if SH_DEGREE>1
splat.sh1=textureLoad(shTexture1,splatUVi32,0);
#endif
#if SH_DEGREE>2
splat.sh2=textureLoad(shTexture2,splatUVi32,0);
#endif
return splat;}
fn computeColorFromSHDegree(dir: vec3f,sh: array<vec3<f32>,16>)->vec3f
{let SH_C0: f32=0.28209479;let SH_C1: f32=0.48860251;var SH_C2: array<f32,5>=array<f32,5>(
1.092548430,
-1.09254843,
0.315391565,
-1.09254843,
0.546274215
);var SH_C3: array<f32,7>=array<f32,7>(
-0.59004358,
2.890611442,
-0.45704579,
0.373176332,
-0.45704579,
1.445305721,
-0.59004358
);var result: vec3f=/*SH_C0**/sh[0];
#if SH_DEGREE>0
let x: f32=dir.x;let y: f32=dir.y;let z: f32=dir.z;result+=-SH_C1*y*sh[1]+SH_C1*z*sh[2]-SH_C1*x*sh[3];
#if SH_DEGREE>1
let xx: f32=x*x;let yy: f32=y*y;let zz: f32=z*z;let xy: f32=x*y;let yz: f32=y*z;let xz: f32=x*z;result+=
SH_C2[0]*xy*sh[4] +
SH_C2[1]*yz*sh[5] +
SH_C2[2]*(2.0f*zz-xx-yy)*sh[6] +
SH_C2[3]*xz*sh[7] +
SH_C2[4]*(xx-yy)*sh[8];
#if SH_DEGREE>2
result+=
SH_C3[0]*y*(3.0f*xx-yy)*sh[9] +
SH_C3[1]*xy*z*sh[10] +
SH_C3[2]*y*(4.0f*zz-xx-yy)*sh[11] +
SH_C3[3]*z*(2.0f*zz-3.0f*xx-3.0f*yy)*sh[12] +
SH_C3[4]*x*(4.0f*zz-xx-yy)*sh[13] +
SH_C3[5]*z*(xx-yy)*sh[14] +
SH_C3[6]*x*(xx-3.0f*yy)*sh[15];
#endif
#endif
#endif
return result;}
fn decompose(value: u32)->vec4f
{let components : vec4f=vec4f(
f32((value ) & 255u),
f32((value>>u32( 8)) & 255u),
f32((value>>u32(16)) & 255u),
f32((value>>u32(24)) & 255u));return components*vec4f(2./255.)-vec4f(1.);}
fn computeSH(splat: Splat,dir: vec3f)->vec3f
{var sh: array<vec3<f32>,16>;sh[0]=vec3f(0.,0.,0.);
#if SH_DEGREE>0
let sh00: vec4f=decompose(splat.sh0.x);let sh01: vec4f=decompose(splat.sh0.y);let sh02: vec4f=decompose(splat.sh0.z);sh[1]=vec3f(sh00.x,sh00.y,sh00.z);sh[2]=vec3f(sh00.w,sh01.x,sh01.y);sh[3]=vec3f(sh01.z,sh01.w,sh02.x);
#endif
#if SH_DEGREE>1
let sh03: vec4f=decompose(splat.sh0.w);let sh04: vec4f=decompose(splat.sh1.x);let sh05: vec4f=decompose(splat.sh1.y);sh[4]=vec3f(sh02.y,sh02.z,sh02.w);sh[5]=vec3f(sh03.x,sh03.y,sh03.z);sh[6]=vec3f(sh03.w,sh04.x,sh04.y);sh[7]=vec3f(sh04.z,sh04.w,sh05.x);sh[8]=vec3f(sh05.y,sh05.z,sh05.w);
#endif
#if SH_DEGREE>2
let sh06: vec4f=decompose(splat.sh1.z);let sh07: vec4f=decompose(splat.sh1.w);let sh08: vec4f=decompose(splat.sh2.x);let sh09: vec4f=decompose(splat.sh2.y);let sh10: vec4f=decompose(splat.sh2.z);let sh11: vec4f=decompose(splat.sh2.w);sh[9]=vec3f(sh06.x,sh06.y,sh06.z);sh[10]=vec3f(sh06.w,sh07.x,sh07.y);sh[11]=vec3f(sh07.z,sh07.w,sh08.x);sh[12]=vec3f(sh08.y,sh08.z,sh08.w);sh[13]=vec3f(sh09.x,sh09.y,sh09.z);sh[14]=vec3f(sh09.w,sh10.x,sh10.y);sh[15]=vec3f(sh10.z,sh10.w,sh11.x); 
#endif
return computeColorFromSHDegree(dir,sh);}
fn gaussianSplatting(
meshPos: vec2<f32>,
worldPos: vec3<f32>,
scale: vec2<f32>,
covA: vec3<f32>,
covB: vec3<f32>,
worldMatrix: mat4x4<f32>,
viewMatrix: mat4x4<f32>,
projectionMatrix: mat4x4<f32>,
focal: vec2f,
invViewport: vec2f,
kernelSize: f32
)->vec4f {let modelView=viewMatrix*worldMatrix;let camspace=viewMatrix*vec4f(worldPos,1.0);let pos2d=projectionMatrix*camspace;let bounds=1.2*pos2d.w;if (pos2d.z<0. || pos2d.x<-bounds || pos2d.x>bounds || pos2d.y<-bounds || pos2d.y>bounds) {return vec4f(0.0,0.0,2.0,1.0);}
let Vrk=mat3x3<f32>(
covA.x,covA.y,covA.z,
covA.y,covB.x,covB.y,
covA.z,covB.y,covB.z
);let isOrtho=abs(projectionMatrix[3][3]-1.0)<0.001;var J: mat3x3<f32>;if (isOrtho) {J=mat3x3<f32>(
focal.x,0.0,0.0,
0.0,focal.y,0.0,
0.0,0.0,0.0
);} else {J=mat3x3<f32>(
focal.x/camspace.z,0.0,-(focal.x*camspace.x)/(camspace.z*camspace.z),
0.0,focal.y/camspace.z,-(focal.y*camspace.y)/(camspace.z*camspace.z),
0.0,0.0,0.0
);}
let T=transpose(mat3x3<f32>(
modelView[0].xyz,
modelView[1].xyz,
modelView[2].xyz))*J;var cov2d=transpose(T)*Vrk*T;
#if COMPENSATION
let c00: f32=cov2d[0][0];let c11: f32=cov2d[1][1];let c01: f32=cov2d[0][1];let detOrig: f32=c00*c11-c01*c01;
#endif
cov2d[0][0]+=kernelSize;cov2d[1][1]+=kernelSize;
#if COMPENSATION
let c2d: vec3f=vec3f(cov2d[0][0],c01,cov2d[1][1]);let detBlur: f32=c2d.x*c2d.z-c2d.y*c2d.y;let compensation: f32=sqrt(max(0.,detOrig/detBlur));vertexOutputs.vColor.w*=compensation;
#endif
let mid=(cov2d[0][0]+cov2d[1][1])/2.0;let radius=length(vec2<f32>((cov2d[0][0]-cov2d[1][1])/2.0,cov2d[0][1]));let lambda1=mid+radius;let lambda2=mid-radius;if (lambda2<0.0) {return vec4f(0.0,0.0,2.0,1.0);}
let diagonalVector=normalize(vec2<f32>(cov2d[0][1],lambda1-cov2d[0][0]));let majorAxis=min(sqrt(2.0*lambda1),1024.0)*diagonalVector;let minorAxis=min(sqrt(2.0*lambda2),1024.0)*vec2<f32>(diagonalVector.y,-diagonalVector.x);let vCenter=vec2<f32>(pos2d.x,pos2d.y);let scaleFactor=select(pos2d.w,1.0,isOrtho);return vec4f(
vCenter+((meshPos.x*majorAxis+meshPos.y*minorAxis)*invViewport*scaleFactor)*scale,
pos2d.z,
pos2d.w
);}`;v.IncludesShadersStoreWGSL[z]||(v.IncludesShadersStoreWGSL[z]=B);var V=`gaussianSplattingVertexShader`,H=`#include<sceneUboDeclaration>
#include<meshUboDeclaration>
#include<helperFunctions>
#include<clipPlaneVertexDeclaration>
#include<fogVertexDeclaration>
#include<logDepthDeclaration>
attribute splatIndex0: vec4f;attribute splatIndex1: vec4f;attribute splatIndex2: vec4f;attribute splatIndex3: vec4f;attribute position: vec3f;uniform invViewport: vec2f;uniform dataTextureSize: vec2f;uniform focal: vec2f;uniform kernelSize: f32;uniform eyePosition: vec3f;uniform alpha: f32;var covariancesATexture: texture_2d<f32>;var covariancesBTexture: texture_2d<f32>;var centersTexture: texture_2d<f32>;var colorsTexture: texture_2d<f32>;
#if SH_DEGREE>0
var shTexture0: texture_2d<u32>;
#endif
#if SH_DEGREE>1
var shTexture1: texture_2d<u32>;
#endif
#if SH_DEGREE>2
var shTexture2: texture_2d<u32>;
#endif
varying vColor: vec4f;varying vPosition: vec2f;
#include<gaussianSplatting>
@vertex
fn main(input : VertexInputs)->FragmentInputs {let splatIndex: f32=getSplatIndex(i32(input.position.z+0.5),input.splatIndex0,input.splatIndex1,input.splatIndex2,input.splatIndex3);var splat: Splat=readSplat(splatIndex,uniforms.dataTextureSize);var covA: vec3f=splat.covA.xyz;var covB: vec3f=vec3f(splat.covA.w,splat.covB.xy);let worldPos: vec4f=mesh.world*vec4f(splat.center.xyz,1.0);vertexOutputs.vPosition=input.position.xy;
#if SH_DEGREE>0
let worldRot: mat3x3f= mat3x3f(mesh.world[0].xyz,mesh.world[1].xyz,mesh.world[2].xyz);let normWorldRot: mat3x3f=inverseMat3(worldRot);var eyeToSplatLocalSpace: vec3f=normalize(normWorldRot*(worldPos.xyz-uniforms.eyePosition.xyz));vertexOutputs.vColor=vec4f(splat.color.xyz+computeSH(splat,eyeToSplatLocalSpace),splat.color.w*uniforms.alpha);
#else
vertexOutputs.vColor=vec4f(splat.color.xyz,splat.color.w*uniforms.alpha);
#endif
vertexOutputs.position=gaussianSplatting(input.position.xy,worldPos.xyz,vec2f(1.0,1.0),covA,covB,mesh.world,scene.view,scene.projection,uniforms.focal,uniforms.invViewport,uniforms.kernelSize);
#include<clipPlaneVertex>
#include<fogVertex>
#include<logDepthVertex>
}
`;v.ShadersStoreWGSL[V]||(v.ShadersStoreWGSL[V]=H);var U={name:V,shader:H},Ae=class{constructor(){this.mm=new Map}get(e,t){let n=this.mm.get(e);if(n!==void 0)return n.get(t)}set(e,t,n){let r=this.mm.get(e);r===void 0&&this.mm.set(e,r=new Map),r.set(t,n)}},je=class{get standalone(){return this._options?.standalone??!1}get baseMaterial(){return this._baseMaterial}get doNotInjectCode(){return this._options?.doNotInjectCode??!1}constructor(e,t,r){this._baseMaterial=e,this._scene=t??n.LastCreatedScene,this._options=r,this._subMeshToEffect=new Map,this._subMeshToDepthWrapper=new Ae,this._meshes=new Map,this._onEffectCreatedObserver=this._baseMaterial.onEffectCreatedObservable.add(e=>{let t=e.subMesh?.getMesh();t&&!this._meshes.has(t)&&this._meshes.set(t,t.onDisposeObservable.add(e=>{let t=this._subMeshToEffect.keys();for(let n=t.next();n.done!==!0;n=t.next()){let t=n.value;t?.getMesh()===e&&(this._subMeshToEffect.delete(t),this._deleteDepthWrapperEffect(t))}})),this._subMeshToEffect.get(e.subMesh)?.[0]!==e.effect&&(this._subMeshToEffect.set(e.subMesh,[e.effect,this._scene.getEngine().currentRenderPassId]),this._deleteDepthWrapperEffect(e.subMesh))})}_deleteDepthWrapperEffect(e){let t=this._subMeshToDepthWrapper.mm.get(e);t&&(t.forEach(e=>{e.mainDrawWrapper.effect?.dispose()}),this._subMeshToDepthWrapper.mm.delete(e))}getEffect(e,t,n){let r=this._subMeshToDepthWrapper.mm.get(e)?.get(t);if(!r)return null;let i=r.drawWrapper[n];return i||(i=r.drawWrapper[n]=new w(this._scene.getEngine()),i.setEffect(r.mainDrawWrapper.effect,r.mainDrawWrapper.defines)),i}isReadyForSubMesh(e,t,n,r,i){return this.standalone&&!this._baseMaterial.isReadyForSubMesh(e.getMesh(),e,r)?!1:this._makeEffect(e,t,n,i)?.isReady()??!1}dispose(){this._baseMaterial.onEffectCreatedObservable.remove(this._onEffectCreatedObserver),this._onEffectCreatedObserver=null;let e=this._meshes.entries();for(let t=e.next();t.done!==!0;t=e.next()){let[e,n]=t.value;e.onDisposeObservable.remove(n)}}_makeEffect(e,t,n,r){let i=this._scene.getEngine(),a=this._subMeshToEffect.get(e);if(!a)return null;let[o,s]=a;if(!o.isReady())return null;let c=this._subMeshToDepthWrapper.get(e,n);if(!c){let t=new w(i);t.defines=e._getDrawWrapper(s)?.defines??null,c={drawWrapper:[],mainDrawWrapper:t,depthDefines:``,token:y()},c.drawWrapper[r]=t,this._subMeshToDepthWrapper.set(e,n,c)}let l=t.join(`
`);if(c.mainDrawWrapper.effect&&l===c.depthDefines)return c.mainDrawWrapper.effect;c.depthDefines=l;let u=o.getUniformNames().slice(),d=o.vertexSourceCodeBeforeMigration,f=o.fragmentSourceCodeBeforeMigration;if(!d&&!f)return null;if(!this.doNotInjectCode){let e=this._options&&this._options.remappedVariables?`#include<shadowMapVertexNormalBias>(${this._options.remappedVariables.join(`,`)})`:`#include<shadowMapVertexNormalBias>`,t=this._options&&this._options.remappedVariables?`#include<shadowMapVertexMetric>(${this._options.remappedVariables.join(`,`)})`:`#include<shadowMapVertexMetric>`,n=this._options&&this._options.remappedVariables?`#include<shadowMapFragmentSoftTransparentShadow>(${this._options.remappedVariables.join(`,`)})`:`#include<shadowMapFragmentSoftTransparentShadow>`,r=`#include<shadowMapVertexExtraDeclaration>`;d=o.shaderLanguage===0?d.replace(/void\s+?main/g,`\n${r}\nvoid main`):d.replace(/@vertex/g,`\n${r}\n@vertex`),d=d.replace(/#define SHADOWDEPTH_NORMALBIAS|#define CUSTOM_VERTEX_UPDATE_WORLDPOS/g,e),d=d.indexOf(`#define SHADOWDEPTH_METRIC`)===-1?d.replace(/}\s*$/g,t+`
}`):d.replace(/#define SHADOWDEPTH_METRIC/g,t),d=d.replace(/#define SHADER_NAME.*?\n|out vec4 glFragColor;\n/g,``);let i=f.indexOf(`#define SHADOWDEPTH_SOFTTRANSPARENTSHADOW`)>=0||f.indexOf(`#define CUSTOM_FRAGMENT_BEFORE_FOG`)>=0,a=f.indexOf(`#define SHADOWDEPTH_FRAGMENT`)!==-1,s=``;i?f=f.replace(/#define SHADOWDEPTH_SOFTTRANSPARENTSHADOW|#define CUSTOM_FRAGMENT_BEFORE_FOG/g,n):s=n+`
`,f=f.replace(/void\s+?main/g,_.IncludesShadersStore.shadowMapFragmentExtraDeclaration+`
void main`),a?f=f.replace(/#define SHADOWDEPTH_FRAGMENT/g,`#include<shadowMapFragment>`):s+=`#include<shadowMapFragment>
`,s&&(f=f.replace(/}\s*$/g,s+`}`)),u.push(`biasAndScaleSM`,`depthValuesSM`,`lightDataSM`,`softTransparentShadowSM`)}c.mainDrawWrapper.effect=i.createEffect({vertexSource:d,fragmentSource:f,vertexToken:c.token,fragmentToken:c.token},{attributes:o.getAttributesNames(),uniformsNames:u,uniformBuffersNames:o.getUniformBuffersNames(),samplers:o.getSamplers(),defines:l+`
`+o.defines.replace(`#define SHADOWS`,``).replace(/#define SHADOW\d/g,``),indexParameters:o.getIndexParameters(),shaderLanguage:o.shaderLanguage},i);for(let e=0;e<c.drawWrapper.length;++e)e!==r&&c.drawWrapper[e]?.setEffect(c.mainDrawWrapper.effect,c.mainDrawWrapper.defines);return c.mainDrawWrapper.effect}},Me=`gaussianSplattingDepthPixelShader`,W=`precision highp float;varying vec2 vPosition;varying vec4 vColor;
#ifdef DEPTH_RENDER
varying float vDepthMetric;
#endif
void main(void) {float A=-dot(vPosition,vPosition);
#if defined(SM_SOFTTRANSPARENTSHADOW) && SM_SOFTTRANSPARENTSHADOW==1
float alpha=exp(A)*vColor.a;if (A<-4.) discard;
#else
if (A<-vColor.a) discard;
#endif
#ifdef DEPTH_RENDER
gl_FragColor=vec4(vDepthMetric,0.0,0.0,1.0);
#endif
}`;v.ShadersStore[Me]||(v.ShadersStore[Me]=W);var Ne=`gaussianSplattingDepthVertexShader`,Pe=`#include<__decl__gaussianSplattingVertex>
uniform vec2 invViewport;uniform vec2 dataTextureSize;uniform vec2 focal;uniform float kernelSize;uniform float alpha;uniform sampler2D covariancesATexture;uniform sampler2D covariancesBTexture;uniform sampler2D centersTexture;uniform sampler2D colorsTexture;varying vec2 vPosition;varying vec4 vColor;
#include<gaussianSplatting>
#ifdef DEPTH_RENDER
uniform vec2 depthValues;varying float vDepthMetric;
#endif
void main(void) {float splatIndex=getSplatIndex(int(position.z+0.5));Splat splat=readSplat(splatIndex);vec3 covA=splat.covA.xyz;vec3 covB=vec3(splat.covA.w,splat.covB.xy);vec4 worldPosGS=world*vec4(splat.center.xyz,1.0);vPosition=position.xy;vColor=splat.color;vColor.w*=alpha;gl_Position=gaussianSplatting(position.xy,worldPosGS.xyz,vec2(1.,1.),covA,covB,world,view,projection);
#ifdef DEPTH_RENDER
#ifdef USE_REVERSE_DEPTHBUFFER
vDepthMetric=((-gl_Position.z+depthValues.x)/(depthValues.y));
#else
vDepthMetric=((gl_Position.z+depthValues.x)/(depthValues.y));
#endif
#endif
}`;v.ShadersStore[Ne]||(v.ShadersStore[Ne]=Pe);var Fe=`gaussianSplattingDepthPixelShader`,G=`#include<gaussianSplattingFragmentDeclaration>
varying vPosition: vec2f;varying vColor: vec4f;
#ifdef DEPTH_RENDER
varying vDepthMetric: f32;
#endif
fn checkDiscard(inPosition: vec2f,inColor: vec4f)->vec4f {var A : f32=-dot(inPosition,inPosition);var alpha : f32=exp(A)*inColor.a;
#if defined(SM_SOFTTRANSPARENTSHADOW) && SM_SOFTTRANSPARENTSHADOW==1
if (A<-4.) {discard;}
#else
if (A<-inColor.a) {discard;}
#endif
#ifdef DEPTH_RENDER
return vec4f(fragmentInputs.vDepthMetric,0.0,0.0,1.0);
#else
return vec4f(inColor.rgb,alpha);
#endif
}
#define CUSTOM_FRAGMENT_DEFINITIONS
@fragment
fn main(input: FragmentInputs)->FragmentOutputs {fragmentOutputs.color=checkDiscard(fragmentInputs.vPosition,fragmentInputs.vColor);
#if defined(SM_SOFTTRANSPARENTSHADOW) && SM_SOFTTRANSPARENTSHADOW==1
var alpha : f32=fragmentOutputs.color.a;
#endif
}
`;v.ShadersStoreWGSL[Fe]||(v.ShadersStoreWGSL[Fe]=G);var K=`gaussianSplattingDepthVertexShader`,q=`#include<sceneUboDeclaration>
#include<meshUboDeclaration>
attribute splatIndex0: vec4f;attribute splatIndex1: vec4f;attribute splatIndex2: vec4f;attribute splatIndex3: vec4f;attribute position: vec3f;uniform invViewport: vec2f;uniform dataTextureSize: vec2f;uniform focal: vec2f;uniform kernelSize: f32;uniform alpha: f32;var covariancesATexture: texture_2d<f32>;var covariancesBTexture: texture_2d<f32>;var centersTexture: texture_2d<f32>;var colorsTexture: texture_2d<f32>;varying vPosition: vec2f;varying vColor: vec4f;
#ifdef DEPTH_RENDER
uniform depthValues: vec2f;varying vDepthMetric: f32;
#endif
#include<gaussianSplatting>
@vertex
fn main(input : VertexInputs)->FragmentInputs {let splatIndex: f32=getSplatIndex(i32(input.position.z+0.5),input.splatIndex0,input.splatIndex1,input.splatIndex2,input.splatIndex3);var splat: Splat=readSplat(splatIndex,uniforms.dataTextureSize);var covA: vec3f=splat.covA.xyz;var covB: vec3f=vec3f(splat.covA.w,splat.covB.xy);let worldPos: vec4f=mesh.world*vec4f(splat.center.xyz,1.0);vertexOutputs.vPosition=input.position.xy;vertexOutputs.vColor=splat.color;vertexOutputs.vColor.w*=uniforms.alpha;vertexOutputs.position=gaussianSplatting(input.position.xy,worldPos.xyz,vec2f(1.0,1.0),covA,covB,mesh.world,scene.view,scene.projection,uniforms.focal,uniforms.invViewport,uniforms.kernelSize);
#ifdef DEPTH_RENDER
#ifdef USE_REVERSE_DEPTHBUFFER
vertexOutputs.vDepthMetric=((-vertexOutputs.position.z+uniforms.depthValues.x)/(uniforms.depthValues.y));
#else
vertexOutputs.vDepthMetric=((vertexOutputs.position.z+uniforms.depthValues.x)/(uniforms.depthValues.y));
#endif
#endif
}`;v.ShadersStoreWGSL[K]||(v.ShadersStoreWGSL[K]=q);var J=class extends de{constructor(){super(),this.FOG=!1,this.THIN_INSTANCES=!0,this.LOGARITHMICDEPTH=!1,this.CLIPPLANE=!1,this.CLIPPLANE2=!1,this.CLIPPLANE3=!1,this.CLIPPLANE4=!1,this.CLIPPLANE5=!1,this.CLIPPLANE6=!1,this.SH_DEGREE=0,this.COMPENSATION=!1,this.rebuild()}},Y=class t extends fe{constructor(e,n){super(e,n),this.kernelSize=t.KernelSize,this._compensation=t.Compensation,this._isDirty=!1,this._sourceMesh=null,this.backFaceCulling=!1,this.shadowDepthWrapper=t._MakeGaussianSplattingShadowDepthWrapper(n,this.shaderLanguage)}set compensation(e){this._isDirty=this._isDirty!=e,this._compensation=e}get compensation(){return this._compensation}get hasRenderTargetTextures(){return!1}needAlphaTesting(){return!1}needAlphaBlending(){return!0}isReadyForSubMesh(n,r){let i=r._drawWrapper,a=r.materialDefines;if(a&&this._isDirty&&a.markAsUnprocessed(),i.effect&&this.isFrozen&&i._wasPreviouslyReady&&i._wasPreviouslyUsingInstances===!0)return!0;r.materialDefines||(a=r.materialDefines=new J);let o=this.getScene();if(this._isReadyForSubMesh(r))return!0;if(!this._sourceMesh)return!1;let s=o.getEngine(),c=this._sourceMesh;ae(n,o,this._useLogarithmicDepth,this.pointsCloud,this.fogEnabled,!1,a,void 0,void 0,void 0,this._isVertexOutputInvariant),se(o,s,this,a,!0,null,!0),ce(n,a,!1,!1),(s.version>1||s.isWebGPU)&&(a.SH_DEGREE=c.shDegree);let l=c.material;if(a.COMPENSATION=l&&l.compensation?l.compensation:t.Compensation,a.isDirty){a.markAsProcessed(),o.resetCachedMaterial(),oe(t._Attribs,a),re({uniformsNames:t._Uniforms,uniformBuffersNames:t._UniformBuffers,samplers:t._Samplers,defines:a}),ne(t._Uniforms);let n=a.toString(),i=o.getEngine().createEffect(`gaussianSplatting`,{attributes:t._Attribs,uniformsNames:t._Uniforms,uniformBuffersNames:t._UniformBuffers,samplers:t._Samplers,defines:n,onCompiled:this.onCompiled,onError:this.onError,indexParameters:{},shaderLanguage:this._shaderLanguage,extraInitializationsAsync:async()=>{this._shaderLanguage===1?await Promise.all([e(()=>import(`./gaussianSplatting.fragment-CEfpieXo.js`),[],import.meta.url),e(()=>import(`./gaussianSplatting.vertex-BHGZresk.js`),[],import.meta.url)]):await Promise.all([e(()=>import(`./gaussianSplatting.fragment-CfbrFqXo.js`),[],import.meta.url),e(()=>import(`./gaussianSplatting.vertex-DbO8xI7T.js`),[],import.meta.url)])}},s);r.setEffect(i,a,this._materialContext)}return!r.effect||!r.effect.isReady()?!1:(a._renderId=o.getRenderId(),i._wasPreviouslyReady=!0,i._wasPreviouslyUsingInstances=!0,this._isDirty=!1,!0)}setSourceMesh(e){this._sourceMesh=e}static BindEffect(e,n,r){let i=r.getEngine(),a=r.activeCamera,o=i.getRenderWidth()*a.viewport.width,s=i.getRenderHeight()*a.viewport.height,c=e.material;if(!c._sourceMesh)return;let l=c._sourceMesh,u=a?.rigParent?.rigCameras.length||1;n.setFloat2(`invViewport`,1/(o/u),1/s);let d=1e3;if(a){let e=a.getProjectionMatrix().m[5];d=a.fovMode==x.FOVMODE_VERTICAL_FIXED?s*e/2:o*e/2}if(n.setFloat2(`focal`,d,d),n.setFloat(`kernelSize`,c&&c.kernelSize?c.kernelSize:t.KernelSize),n.setFloat(`alpha`,c.alpha),r.bindEyePosition(n,`eyePosition`,!0),l.covariancesATexture){let e=l.covariancesATexture.getSize();if(n.setFloat2(`dataTextureSize`,e.width,e.height),n.setTexture(`covariancesATexture`,l.covariancesATexture),n.setTexture(`covariancesBTexture`,l.covariancesBTexture),n.setTexture(`centersTexture`,l.centersTexture),n.setTexture(`colorsTexture`,l.colorsTexture),l.shTextures)for(let e=0;e<l.shTextures?.length;e++)n.setTexture(`shTexture${e}`,l.shTextures[e])}}bindForSubMesh(e,n,r){let i=this.getScene(),a=r.materialDefines;if(!a)return;let o=r.effect;o&&(this._activeEffect=o,n.getMeshUniformBuffer().bindToEffect(o,`Mesh`),n.transferToEffect(e),this._mustRebind(i,o,r,n.visibility)?(this.bindView(o),this.bindViewProjection(o),t.BindEffect(n,this._activeEffect,i),le(o,this,i)):i.getEngine()._features.needToAlwaysBindUniformBuffers&&(this._needToBindSceneUbo=!0),ue(i,n,o),this.useLogarithmicDepth&&ie(a,o,i),this._afterBind(n,this._activeEffect,r))}static _BindEffectUniforms(e,n,r,i){let a=i.getEngine(),o=r.getEffect();e.getMeshUniformBuffer().bindToEffect(o,`Mesh`),r.bindView(o),r.bindViewProjection(o);let s=a.getRenderWidth(),c=a.getRenderHeight();o.setFloat2(`invViewport`,1/s,1/c);let l=s*i.getProjectionMatrix().m[5]/2;o.setFloat2(`focal`,l,l),o.setFloat(`kernelSize`,n&&n.kernelSize?n.kernelSize:t.KernelSize),o.setFloat(`alpha`,n.alpha);let u,d,f=i.activeCamera;if(f&&(f.mode===x.ORTHOGRAPHIC_CAMERA?(u=!a.useReverseDepthBuffer&&a.isNDCHalfZRange?0:1,d=a.useReverseDepthBuffer&&a.isNDCHalfZRange?0:1):(u=a.useReverseDepthBuffer&&a.isNDCHalfZRange?f.minZ:a.isNDCHalfZRange?0:f.minZ,d=a.useReverseDepthBuffer&&a.isNDCHalfZRange?0:f.maxZ),o.setFloat2(`depthValues`,u,u+d),e.covariancesATexture)){let t=e.covariancesATexture.getSize();o.setFloat2(`dataTextureSize`,t.width,t.height),o.setTexture(`covariancesATexture`,e.covariancesATexture),o.setTexture(`covariancesBTexture`,e.covariancesBTexture),o.setTexture(`centersTexture`,e.centersTexture),o.setTexture(`colorsTexture`,e.colorsTexture)}}makeDepthRenderingMaterial(e,n){let r=new pe(`gaussianSplattingDepthRender`,e,{vertex:`gaussianSplattingDepth`,fragment:`gaussianSplattingDepth`},{attributes:t._Attribs,uniforms:t._Uniforms,samplers:t._Samplers,uniformBuffers:t._UniformBuffers,shaderLanguage:n,defines:[`#define DEPTH_RENDER`]});return r.onBindObservable.add(n=>{let i=n.material,a=n;t._BindEffectUniforms(a,i,r,e)}),r}static _MakeGaussianSplattingShadowDepthWrapper(e,n){let r=new pe(`gaussianSplattingDepth`,e,{vertex:`gaussianSplattingDepth`,fragment:`gaussianSplattingDepth`},{attributes:t._Attribs,uniforms:t._Uniforms,samplers:t._Samplers,uniformBuffers:t._UniformBuffers,shaderLanguage:n}),i=new je(r,e,{standalone:!0});return r.onBindObservable.add(n=>{let i=n.material,a=n;t._BindEffectUniforms(a,i,r,e)}),i}clone(e){return m.Clone(()=>new t(e,this.getScene()),this)}serialize(){let e=super.serialize();return e.customType=`BABYLON.GaussianSplattingMaterial`,e}getClassName(){return`GaussianSplattingMaterial`}static Parse(e,n,r){return m.Parse(()=>new t(e.name,n),e,n,r)}};Y.KernelSize=.3,Y.Compensation=!1,Y._Attribs=[b.PositionKind,`splatIndex0`,`splatIndex1`,`splatIndex2`,`splatIndex3`],Y._Samplers=[`covariancesATexture`,`covariancesBTexture`,`centersTexture`,`colorsTexture`,`shTexture0`,`shTexture1`,`shTexture2`],Y._UniformBuffers=[`Scene`,`Mesh`],Y._Uniforms=[`world`,`view`,`projection`,`vFogInfos`,`vFogColor`,`logarithmicDepthConstant`,`invViewport`,`dataTextureSize`,`focal`,`eyePosition`,`kernelSize`,`alpha`,`depthValues`],t(`BABYLON.GaussianSplattingMaterial`,Y);var Ie=r,X={...a,TwoPi:Math.PI*2,Sign:Math.sign,Log2:Math.log2,HCF:Ie},Z=(e,t)=>{let n=(1<<t)-1;return(e&n)/n},Q=(e,t)=>{t.x=Z(e>>>21,11),t.y=Z(e>>>11,10),t.z=Z(e,11)},Le=(e,t)=>{t[0]=Z(e>>>24,8)*255,t[1]=Z(e>>>16,8)*255,t[2]=Z(e>>>8,8)*255,t[3]=Z(e,8)*255},Re=(e,t)=>{let n=1/(Math.sqrt(2)*.5),r=(Z(e>>>20,10)-.5)*n,i=(Z(e>>>10,10)-.5)*n,a=(Z(e,10)-.5)*n,o=Math.sqrt(1-(r*r+i*i+a*a));switch(e>>>30){case 0:t.set(o,r,i,a);break;case 1:t.set(r,o,i,a);break;case 2:t.set(r,i,o,a);break;case 3:t.set(r,i,a,o);break}},ze;(function(e){e[e.FLOAT=0]=`FLOAT`,e[e.INT=1]=`INT`,e[e.UINT=2]=`UINT`,e[e.DOUBLE=3]=`DOUBLE`,e[e.UCHAR=4]=`UCHAR`,e[e.UNDEFINED=5]=`UNDEFINED`})(ze||={});var Be;(function(e){e[e.MIN_X=0]=`MIN_X`,e[e.MIN_Y=1]=`MIN_Y`,e[e.MIN_Z=2]=`MIN_Z`,e[e.MAX_X=3]=`MAX_X`,e[e.MAX_Y=4]=`MAX_Y`,e[e.MAX_Z=5]=`MAX_Z`,e[e.MIN_SCALE_X=6]=`MIN_SCALE_X`,e[e.MIN_SCALE_Y=7]=`MIN_SCALE_Y`,e[e.MIN_SCALE_Z=8]=`MIN_SCALE_Z`,e[e.MAX_SCALE_X=9]=`MAX_SCALE_X`,e[e.MAX_SCALE_Y=10]=`MAX_SCALE_Y`,e[e.MAX_SCALE_Z=11]=`MAX_SCALE_Z`,e[e.PACKED_POSITION=12]=`PACKED_POSITION`,e[e.PACKED_ROTATION=13]=`PACKED_ROTATION`,e[e.PACKED_SCALE=14]=`PACKED_SCALE`,e[e.PACKED_COLOR=15]=`PACKED_COLOR`,e[e.X=16]=`X`,e[e.Y=17]=`Y`,e[e.Z=18]=`Z`,e[e.SCALE_0=19]=`SCALE_0`,e[e.SCALE_1=20]=`SCALE_1`,e[e.SCALE_2=21]=`SCALE_2`,e[e.DIFFUSE_RED=22]=`DIFFUSE_RED`,e[e.DIFFUSE_GREEN=23]=`DIFFUSE_GREEN`,e[e.DIFFUSE_BLUE=24]=`DIFFUSE_BLUE`,e[e.OPACITY=25]=`OPACITY`,e[e.F_DC_0=26]=`F_DC_0`,e[e.F_DC_1=27]=`F_DC_1`,e[e.F_DC_2=28]=`F_DC_2`,e[e.F_DC_3=29]=`F_DC_3`,e[e.ROT_0=30]=`ROT_0`,e[e.ROT_1=31]=`ROT_1`,e[e.ROT_2=32]=`ROT_2`,e[e.ROT_3=33]=`ROT_3`,e[e.MIN_COLOR_R=34]=`MIN_COLOR_R`,e[e.MIN_COLOR_G=35]=`MIN_COLOR_G`,e[e.MIN_COLOR_B=36]=`MIN_COLOR_B`,e[e.MAX_COLOR_R=37]=`MAX_COLOR_R`,e[e.MAX_COLOR_G=38]=`MAX_COLOR_G`,e[e.MAX_COLOR_B=39]=`MAX_COLOR_B`,e[e.SH_0=40]=`SH_0`,e[e.SH_1=41]=`SH_1`,e[e.SH_2=42]=`SH_2`,e[e.SH_3=43]=`SH_3`,e[e.SH_4=44]=`SH_4`,e[e.SH_5=45]=`SH_5`,e[e.SH_6=46]=`SH_6`,e[e.SH_7=47]=`SH_7`,e[e.SH_8=48]=`SH_8`,e[e.SH_9=49]=`SH_9`,e[e.SH_10=50]=`SH_10`,e[e.SH_11=51]=`SH_11`,e[e.SH_12=52]=`SH_12`,e[e.SH_13=53]=`SH_13`,e[e.SH_14=54]=`SH_14`,e[e.SH_15=55]=`SH_15`,e[e.SH_16=56]=`SH_16`,e[e.SH_17=57]=`SH_17`,e[e.SH_18=58]=`SH_18`,e[e.SH_19=59]=`SH_19`,e[e.SH_20=60]=`SH_20`,e[e.SH_21=61]=`SH_21`,e[e.SH_22=62]=`SH_22`,e[e.SH_23=63]=`SH_23`,e[e.SH_24=64]=`SH_24`,e[e.SH_25=65]=`SH_25`,e[e.SH_26=66]=`SH_26`,e[e.SH_27=67]=`SH_27`,e[e.SH_28=68]=`SH_28`,e[e.SH_29=69]=`SH_29`,e[e.SH_30=70]=`SH_30`,e[e.SH_31=71]=`SH_31`,e[e.SH_32=72]=`SH_32`,e[e.SH_33=73]=`SH_33`,e[e.SH_34=74]=`SH_34`,e[e.SH_35=75]=`SH_35`,e[e.SH_36=76]=`SH_36`,e[e.SH_37=77]=`SH_37`,e[e.SH_38=78]=`SH_38`,e[e.SH_39=79]=`SH_39`,e[e.SH_40=80]=`SH_40`,e[e.SH_41=81]=`SH_41`,e[e.SH_42=82]=`SH_42`,e[e.SH_43=83]=`SH_43`,e[e.SH_44=84]=`SH_44`,e[e.UNDEFINED=85]=`UNDEFINED`})(Be||={});var $=class e extends A{get disableDepthSort(){return this._disableDepthSort}set disableDepthSort(e){!this._disableDepthSort&&e?(this._worker?.terminate(),this._worker=null,this._disableDepthSort=!0):this._disableDepthSort&&!e&&(this._disableDepthSort=!1,this._sortIsDirty=!0,this._instanciateWorker())}get viewDirectionFactor(){return o.OneReadOnly}get shDegree(){return this._shDegree}get splatCount(){return this._splatIndex?.length}get splatsData(){return this._splatsData}get covariancesATexture(){return this._covariancesATexture}get covariancesBTexture(){return this._covariancesBTexture}get centersTexture(){return this._centersTexture}get colorsTexture(){return this._colorsTexture}get shTextures(){return this._shTextures}get kernelSize(){return this._material instanceof Y?this._material.kernelSize:0}get compensation(){return this._material instanceof Y?this._material.compensation:!1}set material(e){this._material=e,this._material.backFaceCulling=!1,this._material.cullBackFaces=!1,e.resetDrawCache()}get material(){return this._material}static _MakeSplatGeometryForMesh(t){let n=new O,r=[-2,-2,0,2,-2,0,2,2,0,-2,2,0],i=[0,1,2,0,2,3],a=[],o=[];for(let t=0;t<e._BatchSize;t++){for(let e=0;e<12;e++)e==2||e==5||e==8||e==11?a.push(t):a.push(r[e]);o.push(i.map(e=>e+t*4))}n.positions=a,n.indices=o.flat(),n.applyToMesh(t)}constructor(t,n=null,r=null,i=!1){super(t,r),this._vertexCount=0,this._worker=null,this._modelViewMatrix=d.Identity(),this._canPostToWorker=!0,this._readyToDisplay=!1,this._covariancesATexture=null,this._covariancesBTexture=null,this._centersTexture=null,this._colorsTexture=null,this._splatPositions=null,this._splatIndex=null,this._shTextures=null,this._splatsData=null,this._keepInRam=!1,this._delayedTextureUpdate=null,this._useRGBACovariants=!1,this._material=null,this._tmpCovariances=[0,0,0,0,0,0],this._sortIsDirty=!1,this._shDegree=0,this._cameraViewInfos=new Map,this._disableDepthSort=!1,this._loadingPromise=null,this.subMeshes=[],new te(0,0,4*e._BatchSize,0,6*e._BatchSize,this),this.setEnabled(!1),this._useRGBACovariants=!this.getEngine().isWebGPU&&this.getEngine().version===1,this._keepInRam=i,n&&(this._loadingPromise=this.loadFileAsync(n));let a=new Y(this.name+`_material`,this._scene);a.setSourceMesh(this),this._material=a,this._scene.onCameraRemovedObservable.add(e=>{let t=e.uniqueId;this._cameraViewInfos.has(t)&&(this._cameraViewInfos.get(t)?.mesh.dispose(),this._cameraViewInfos.delete(t))})}getLoadingPromise(){return this._loadingPromise}getClassName(){return`GaussianSplattingMesh`}getTotalVertices(){return this._vertexCount}isReady(e=!1){return super.isReady(e,!0)?this._readyToDisplay?!0:(this._postToWorker(!0),!1):!1}_getCameraDirection(e){let t=e.getViewMatrix();this.getWorldMatrix().multiplyToRef(t,this._modelViewMatrix);let n=u.Vector3[1];return n.set(this._modelViewMatrix.m[2],this._modelViewMatrix.m[6],this._modelViewMatrix.m[10]),n.normalize(),n}_postToWorker(t=!1){let n=this._scene.getFrameId(),r=!1;this._cameraViewInfos.forEach(e=>{e.frameIdLastUpdate!==n&&(r=!0)});let i=this._scene.activeCameras?.length?this._scene.activeCameras:[this._scene.activeCamera],a=[];i.forEach(t=>{if(!t)return;let r=t.uniqueId,i=this._cameraViewInfos.get(r);if(i)a.push(i);else{let i=new A(this.name+`_cameraMesh_`+r,this._scene);i.reservedDataStore={hidden:!0},i.setEnabled(!1),i.material=this.material,e._MakeSplatGeometryForMesh(i);let s={camera:t,cameraDirection:new o(0,0,0),mesh:i,frameIdLastUpdate:n,splatIndexBufferSet:!1};a.push(s),this._cameraViewInfos.set(r,s)}}),a.sort((e,t)=>e.frameIdLastUpdate-t.frameIdLastUpdate);let s=this._worker||_native&&_native.sortSplats||this._disableDepthSort;(t||r)&&s&&(this._scene.activeCameras?.length||this._scene.activeCamera)&&this._canPostToWorker?a.forEach(e=>{let r=e.camera,i=this._getCameraDirection(r),a=e.cameraDirection,s=o.Dot(i,a);(t||Math.abs(s-1)>=.01)&&this._canPostToWorker&&(e.cameraDirection.copyFrom(i),e.frameIdLastUpdate=n,this._canPostToWorker=!1,this._worker?this._worker.postMessage({view:this._modelViewMatrix.m,depthMix:this._depthMix,useRightHandedSystem:this._scene.useRightHandedSystem,cameraId:r.uniqueId},[this._depthMix.buffer]):_native&&_native.sortSplats&&(_native.sortSplats(this._modelViewMatrix,this._splatPositions,this._splatIndex,this._scene.useRightHandedSystem),e.splatIndexBufferSet?e.mesh.thinInstanceBufferUpdated(`splatIndex`):(e.mesh.thinInstanceSetBuffer(`splatIndex`,this._splatIndex,16,!1),e.splatIndexBufferSet=!0),this._canPostToWorker=!0,this._readyToDisplay=!0))}):this._disableDepthSort&&(a.forEach(e=>{e.splatIndexBufferSet||=(e.mesh.thinInstanceSetBuffer(`splatIndex`,this._splatIndex,16,!1),!0)}),this._canPostToWorker=!0,this._readyToDisplay=!0)}render(e,t,n){this._postToWorker(),!this._geometry&&this._cameraViewInfos.size&&(this._geometry=this._cameraViewInfos.values().next().value.mesh.geometry);let r=this._scene.activeCamera.uniqueId,i=this._cameraViewInfos.get(r);if(!i||!i.splatIndexBufferSet)return this;let a=i.mesh;return a.getWorldMatrix().copyFrom(this.getWorldMatrix()),a.render(e,t,n)}static _TypeNameToEnum(e){switch(e){case`float`:return 0;case`int`:return 1;case`uint`:return 2;case`double`:return 3;case`uchar`:return 4}return 5}static _ValueNameToEnum(e){switch(e){case`min_x`:return 0;case`min_y`:return 1;case`min_z`:return 2;case`max_x`:return 3;case`max_y`:return 4;case`max_z`:return 5;case`min_scale_x`:return 6;case`min_scale_y`:return 7;case`min_scale_z`:return 8;case`max_scale_x`:return 9;case`max_scale_y`:return 10;case`max_scale_z`:return 11;case`packed_position`:return 12;case`packed_rotation`:return 13;case`packed_scale`:return 14;case`packed_color`:return 15;case`x`:return 16;case`y`:return 17;case`z`:return 18;case`scale_0`:return 19;case`scale_1`:return 20;case`scale_2`:return 21;case`diffuse_red`:case`red`:return 22;case`diffuse_green`:case`green`:return 23;case`diffuse_blue`:case`blue`:return 24;case`f_dc_0`:return 26;case`f_dc_1`:return 27;case`f_dc_2`:return 28;case`f_dc_3`:return 29;case`opacity`:return 25;case`rot_0`:return 30;case`rot_1`:return 31;case`rot_2`:return 32;case`rot_3`:return 33;case`min_r`:return 34;case`min_g`:return 35;case`min_b`:return 36;case`max_r`:return 37;case`max_g`:return 38;case`max_b`:return 39;case`f_rest_0`:return 40;case`f_rest_1`:return 41;case`f_rest_2`:return 42;case`f_rest_3`:return 43;case`f_rest_4`:return 44;case`f_rest_5`:return 45;case`f_rest_6`:return 46;case`f_rest_7`:return 47;case`f_rest_8`:return 48;case`f_rest_9`:return 49;case`f_rest_10`:return 50;case`f_rest_11`:return 51;case`f_rest_12`:return 52;case`f_rest_13`:return 53;case`f_rest_14`:return 54;case`f_rest_15`:return 55;case`f_rest_16`:return 56;case`f_rest_17`:return 57;case`f_rest_18`:return 58;case`f_rest_19`:return 59;case`f_rest_20`:return 60;case`f_rest_21`:return 61;case`f_rest_22`:return 62;case`f_rest_23`:return 63;case`f_rest_24`:return 64;case`f_rest_25`:return 65;case`f_rest_26`:return 66;case`f_rest_27`:return 67;case`f_rest_28`:return 68;case`f_rest_29`:return 69;case`f_rest_30`:return 70;case`f_rest_31`:return 71;case`f_rest_32`:return 72;case`f_rest_33`:return 73;case`f_rest_34`:return 74;case`f_rest_35`:return 75;case`f_rest_36`:return 76;case`f_rest_37`:return 77;case`f_rest_38`:return 78;case`f_rest_39`:return 79;case`f_rest_40`:return 80;case`f_rest_41`:return 81;case`f_rest_42`:return 82;case`f_rest_43`:return 83;case`f_rest_44`:return 84}return 85}static ParseHeader(t){let n=new Uint8Array(t),r=new TextDecoder().decode(n.slice(0,1024*10)),i=r.indexOf(`end_header
`);if(i<0||!r)return null;let a=parseInt(/element vertex (\d+)\n/.exec(r)[1]),o=/element chunk (\d+)\n/.exec(r),s=0;o&&(s=parseInt(o[1]));let c=0,l=0,u={double:8,int:4,uint:4,float:4,short:2,ushort:2,uchar:1,list:0},d;(function(e){e[e.Vertex=0]=`Vertex`,e[e.Chunk=1]=`Chunk`,e[e.SH=2]=`SH`,e[e.Unused=3]=`Unused`})(d||={});let f=1,p=[],m=[],g=r.slice(0,i).split(`
`),_=0;for(let t of g)if(t.startsWith(`property `)){let[,n,r]=t.split(` `),i=e._ValueNameToEnum(r);i!=85&&(i>=84?_=3:i>=64?_=Math.max(_,2):i>=48&&(_=Math.max(_,1)));let a=e._TypeNameToEnum(n);f==1?(m.push({value:i,type:a,offset:l}),l+=u[n]):f==0?(p.push({value:i,type:a,offset:c}),c+=u[n]):f==2&&p.push({value:i,type:a,offset:c}),u[n]||h.Warn(`Unsupported property type: ${n}.`)}else if(t.startsWith(`element `)){let[,e]=t.split(` `);f=e==`chunk`?1:e==`vertex`?0:e==`sh`?2:3}let v=new DataView(t,i+11),y=new ArrayBuffer(e._RowOutputLength*a),b=null,x=0;return _&&(x=((_+1)*(_+1)-1)*3,b=new ArrayBuffer(x*a)),{vertexCount:a,chunkCount:s,rowVertexLength:c,rowChunkLength:l,vertexProperties:p,chunkProperties:m,dataView:v,buffer:y,shDegree:_,shCoefficientCount:x,shBuffer:b}}static _GetCompressedChunks(e,t){if(!e.chunkCount)return null;let n=e.dataView,r=Array(e.chunkCount);for(let i=0;i<e.chunkCount;i++){let a={min:new o,max:new o,minScale:new o,maxScale:new o,minColor:new o(0,0,0),maxColor:new o(1,1,1)};r[i]=a;for(let r=0;r<e.chunkProperties.length;r++){let i=e.chunkProperties[r],o;switch(i.type){case 0:o=n.getFloat32(i.offset+t.value,!0);break;default:continue}switch(i.value){case 0:a.min.x=o;break;case 1:a.min.y=o;break;case 2:a.min.z=o;break;case 3:a.max.x=o;break;case 4:a.max.y=o;break;case 5:a.max.z=o;break;case 6:a.minScale.x=o;break;case 7:a.minScale.y=o;break;case 8:a.minScale.z=o;break;case 9:a.maxScale.x=o;break;case 10:a.maxScale.y=o;break;case 11:a.maxScale.z=o;break;case 34:a.minColor.x=o;break;case 35:a.minColor.y=o;break;case 36:a.minColor.z=o;break;case 37:a.maxColor.x=o;break;case 38:a.maxColor.y=o;break;case 39:a.maxColor.z=o;break}}t.value+=e.rowChunkLength}return r}static _GetSplat(t,n,r,i){let a=u.Quaternion[0],o=u.Vector3[0],s=e._RowOutputLength,c=t.buffer,l=t.dataView,d=new Float32Array(c,n*s,3),f=new Float32Array(c,n*s+12,3),p=new Uint8ClampedArray(c,n*s+24,4),m=new Uint8ClampedArray(c,n*s+28,4),h=null;t.shBuffer&&(h=new Uint8ClampedArray(t.shBuffer,n*t.shCoefficientCount,t.shCoefficientCount));let g=n>>8,_=255,v=0,y=0,b=0,x=[];for(let s=0;s<t.vertexProperties.length;s++){let c=t.vertexProperties[s],u;switch(c.type){case 0:u=l.getFloat32(i.value+c.offset,!0);break;case 1:u=l.getInt32(i.value+c.offset,!0);break;case 2:u=l.getUint32(i.value+c.offset,!0);break;case 3:u=l.getFloat64(i.value+c.offset,!0);break;case 4:u=l.getUint8(i.value+c.offset);break;default:continue}switch(c.value){case 12:{let e=r[g];Q(u,o),d[0]=X.Lerp(e.min.x,e.max.x,o.x),d[1]=X.Lerp(e.min.y,e.max.y,o.y),d[2]=X.Lerp(e.min.z,e.max.z,o.z)}break;case 13:Re(u,a),_=a.x,v=a.y,y=a.z,b=a.w;break;case 14:{let e=r[g];Q(u,o),f[0]=Math.exp(X.Lerp(e.minScale.x,e.maxScale.x,o.x)),f[1]=Math.exp(X.Lerp(e.minScale.y,e.maxScale.y,o.y)),f[2]=Math.exp(X.Lerp(e.minScale.z,e.maxScale.z,o.z))}break;case 15:{let e=r[g];Le(u,p),p[0]=X.Lerp(e.minColor.x,e.maxColor.x,p[0]/255)*255,p[1]=X.Lerp(e.minColor.y,e.maxColor.y,p[1]/255)*255,p[2]=X.Lerp(e.minColor.z,e.maxColor.z,p[2]/255)*255}break;case 16:d[0]=u;break;case 17:d[1]=u;break;case 18:d[2]=u;break;case 19:f[0]=Math.exp(u);break;case 20:f[1]=Math.exp(u);break;case 21:f[2]=Math.exp(u);break;case 22:p[0]=u;break;case 23:p[1]=u;break;case 24:p[2]=u;break;case 26:p[0]=(.5+e._SH_C0*u)*255;break;case 27:p[1]=(.5+e._SH_C0*u)*255;break;case 28:p[2]=(.5+e._SH_C0*u)*255;break;case 29:p[3]=(.5+e._SH_C0*u)*255;break;case 25:p[3]=1/(1+Math.exp(-u))*255;break;case 30:_=u;break;case 31:v=u;break;case 32:y=u;break;case 33:b=u;break}if(h&&c.value>=40&&c.value<=84){let e=c.value-40;c.type==4&&t.chunkCount?x[e]=(l.getUint8(t.rowChunkLength*t.chunkCount+t.vertexCount*t.rowVertexLength+n*t.shCoefficientCount+e)*(8/255)-4)*127.5+127.5:x[e]=X.Clamp(u*127.5+127.5,0,255)}}if(h){let e=t.shDegree==1?3:t.shDegree==2?8:15;for(let t=0;t<e;t++)h[t*3+0]=x[t],h[t*3+1]=x[t+e],h[t*3+2]=x[t+e*2]}a.set(v,y,b,_),a.normalize(),m[0]=a.w*127.5+127.5,m[1]=a.x*127.5+127.5,m[2]=a.y*127.5+127.5,m[3]=a.z*127.5+127.5,i.value+=t.rowVertexLength}static*ConvertPLYWithSHToSplat(t,r=!1){let i=e.ParseHeader(t);if(!i)return{buffer:t};let a={value:0},o=e._GetCompressedChunks(i,a);for(let t=0;t<i.vertexCount;t++)e._GetSplat(i,t,o,a),t%e._PlyConversionBatchSize===0&&r&&(yield);let s=null;if(i.shDegree&&i.shBuffer){let e=Math.ceil(i.shCoefficientCount/16),t=0,r=new Uint8Array(i.shBuffer);s=[];let a=i.vertexCount,o=n.LastCreatedEngine;if(o){let n=o.getCaps().maxTextureSize,c=Math.ceil(a/n);for(let t=0;t<e;t++){let e=new Uint8Array(c*n*4*4);s.push(e)}for(let e=0;e<a;e++)for(let n=0;n<i.shCoefficientCount;n++){let i=r[t++],a=Math.floor(n/16),o=s[a],c=n%16,l=e*16;o[c+l]=i}}}return{buffer:i.buffer,sh:s}}static*ConvertPLYToSplat(t,n=!1){let r=e.ParseHeader(t);if(!r)return t;let i={value:0},a=e._GetCompressedChunks(r,i);for(let t=0;t<r.vertexCount;t++)e._GetSplat(r,t,a,i),t%e._PlyConversionBatchSize===0&&n&&(yield);return r.buffer}static async ConvertPLYToSplatAsync(t){return await E(e.ConvertPLYToSplat(t,!0),D())}static async ConvertPLYWithSHToSplatAsync(t){return await E(e.ConvertPLYWithSHToSplat(t,!0),D())}async loadDataAsync(e){return await this.updateDataAsync(e)}async loadFileAsync(e,t){await ge(e,t||n.LastCreatedScene,{pluginOptions:{splat:{gaussianSplattingMesh:this}}})}dispose(e){if(this._covariancesATexture?.dispose(),this._covariancesBTexture?.dispose(),this._centersTexture?.dispose(),this._colorsTexture?.dispose(),this._shTextures)for(let e of this._shTextures)e.dispose();this._covariancesATexture=null,this._covariancesBTexture=null,this._centersTexture=null,this._colorsTexture=null,this._shTextures=null,this._worker?.terminate(),this._worker=null,this._cameraViewInfos.forEach(e=>{e.mesh.dispose()}),super.dispose(e,!0)}_copyTextures(e){if(this._covariancesATexture=e.covariancesATexture?.clone(),this._covariancesBTexture=e.covariancesBTexture?.clone(),this._centersTexture=e.centersTexture?.clone(),this._colorsTexture=e.colorsTexture?.clone(),e._shTextures){this._shTextures=[];for(let e of this._shTextures)this._shTextures?.push(e.clone())}}clone(t=``){let n=new e(t,void 0,this.getScene());n._copySource(this),n.makeGeometryUnique(),n._vertexCount=this._vertexCount,n._copyTextures(this),n._modelViewMatrix=d.Identity(),n._splatPositions=this._splatPositions,n._readyToDisplay=!1,n._disableDepthSort=this._disableDepthSort,n._instanciateWorker();let r=this.getBoundingInfo();return n.getBoundingInfo().reConstruct(r.minimum,r.maximum,this.getWorldMatrix()),n.forcedInstanceCount=this.forcedInstanceCount,n.setEnabled(!0),n}_makeEmptySplat(e,t,n,r){let i=this._useRGBACovariants?4:2;this._splatPositions[4*e+0]=0,this._splatPositions[4*e+1]=0,this._splatPositions[4*e+2]=0,t[e*4+0]=T(0),t[e*4+1]=T(0),t[e*4+2]=T(0),t[e*4+3]=T(0),n[e*i+0]=T(0),n[e*i+1]=T(0),r[e*4+3]=0}_makeSplat(e,t,n,r,i,a,o,s,c){let l=u.Matrix[0],f=u.Matrix[1],p=u.Quaternion[0],m=this._useRGBACovariants?4:2,h=t[8*e+0],g=t[8*e+1]*(c.flipY?-1:1),_=t[8*e+2];this._splatPositions[4*e+0]=h,this._splatPositions[4*e+1]=g,this._splatPositions[4*e+2]=_,o.minimizeInPlaceFromFloats(h,g,_),s.maximizeInPlaceFromFloats(h,g,_),p.set((n[32*e+28+1]-127.5)/127.5,(n[32*e+28+2]-127.5)/127.5,(n[32*e+28+3]-127.5)/127.5,-(n[32*e+28+0]-127.5)/127.5),p.normalize(),p.toRotationMatrix(l),d.ScalingToRef(t[8*e+3+0]*2,t[8*e+3+1]*2,t[8*e+3+2]*2,f);let v=l.multiplyToRef(f,u.Matrix[0]).m,y=this._tmpCovariances;y[0]=v[0]*v[0]+v[1]*v[1]+v[2]*v[2],y[1]=v[0]*v[4]+v[1]*v[5]+v[2]*v[6],y[2]=v[0]*v[8]+v[1]*v[9]+v[2]*v[10],y[3]=v[4]*v[4]+v[5]*v[5]+v[6]*v[6],y[4]=v[4]*v[8]+v[5]*v[9]+v[6]*v[10],y[5]=v[8]*v[8]+v[9]*v[9]+v[10]*v[10];let b=-1e4;for(let e=0;e<6;e++)b=Math.max(b,Math.abs(y[e]));this._splatPositions[4*e+3]=b;let x=b;r[e*4+0]=T(y[0]/x),r[e*4+1]=T(y[1]/x),r[e*4+2]=T(y[2]/x),r[e*4+3]=T(y[3]/x),i[e*m+0]=T(y[4]/x),i[e*m+1]=T(y[5]/x),a[e*4+0]=n[32*e+24+0],a[e*4+1]=n[32*e+24+1],a[e*4+2]=n[32*e+24+2],a[e*4+3]=n[32*e+24+3]}_updateTextures(e,t,n,r){let i=this._getTextureSize(this._vertexCount),a=(e,t,n,r)=>new k(e,t,n,r,this._scene,!1,!1,2,1),o=(e,t,n,r)=>new k(e,t,n,r,this._scene,!1,!1,2,0),s=(e,t,n,r)=>new k(e,t,n,r,this._scene,!1,!1,1,7),c=(e,t,n,r)=>new k(e,t,n,r,this._scene,!1,!1,2,2);if(this._covariancesATexture){this._delayedTextureUpdate={covA:e,covB:t,colors:n,centers:this._splatPositions,sh:r};let i=Float32Array.from(this._splatPositions),a=this._vertexCount;this._worker&&this._worker.postMessage({positions:i,vertexCount:a},[i.buffer]),this._postToWorker(!0)}else{if(this._covariancesATexture=c(e,i.x,i.y,5),this._covariancesBTexture=c(t,i.x,i.y,this._useRGBACovariants?5:7),this._centersTexture=a(this._splatPositions,i.x,i.y,5),this._colorsTexture=o(n,i.x,i.y,5),r){this._shTextures=[];for(let e of r){let t=s(new Uint32Array(e.buffer),i.x,i.y,11);t.wrapU=0,t.wrapV=0,this._shTextures.push(t)}}this._instanciateWorker()}}*_updateData(t,n,r,i={flipY:!1}){this._covariancesATexture||(this._readyToDisplay=!1);let a=new Uint8Array(t),s=new Float32Array(a.buffer);this._keepInRam&&(this._splatsData=t);let c=a.length/e._RowOutputLength;c!=this._vertexCount&&this._updateSplatIndexBuffer(c),this._vertexCount=c,this._shDegree=r?r.length:0;let l=this._getTextureSize(c),u=l.x*l.y,d=e.ProgressiveUpdateAmount??l.y,f=l.x*d;this._splatPositions=new Float32Array(4*u);let p=new Uint16Array(u*4),m=new Uint16Array((this._useRGBACovariants?4:2)*u),h=new Uint8Array(u*4),g=new o(Number.MAX_VALUE,Number.MAX_VALUE,Number.MAX_VALUE),_=new o(-Number.MAX_VALUE,-Number.MAX_VALUE,-Number.MAX_VALUE);if(e.ProgressiveUpdateAmount){this._updateTextures(p,m,h,r),this.setEnabled(!0);let e=Math.ceil(l.y/d);for(let t=0;t<e;t++){let e=t*d,r=e*l.x;for(let e=0;e<f;e++)this._makeSplat(r+e,s,a,p,m,h,g,_,i);this._updateSubTextures(this._splatPositions,p,m,h,e,Math.min(d,l.y-e)),this.getBoundingInfo().reConstruct(g,_,this.getWorldMatrix()),n&&(yield)}let t=Float32Array.from(this._splatPositions),o=this._vertexCount;this._worker&&this._worker.postMessage({positions:t,vertexCount:o},[t.buffer]),this._sortIsDirty=!0}else{let t=c+15&-16;for(let t=0;t<c;t++)this._makeSplat(t,s,a,p,m,h,g,_,i),n&&t%e._SplatBatchSize===0&&(yield);for(let e=c;e<t;e++)this._makeEmptySplat(e,p,m,h);this._updateTextures(p,m,h,r),this.getBoundingInfo().reConstruct(g,_,this.getWorldMatrix()),this.setEnabled(!0),this._sortIsDirty=!0}this._postToWorker(!0)}async updateDataAsync(e,t){return await E(this._updateData(e,!0,t),D())}updateData(e,t,n={flipY:!0}){ee(this._updateData(e,!1,t,n))}refreshBoundingInfo(){return this.thinInstanceRefreshBoundingInfo(!1),this}_updateSplatIndexBuffer(e){let t=e+15&-16;if(!this._splatIndex||e>this._splatIndex.length){this._splatIndex=new Float32Array(t);for(let e=0;e<t;e++)this._splatIndex[e]=e;this._cameraViewInfos.forEach(e=>{e.mesh.thinInstanceSetBuffer(`splatIndex`,this._splatIndex,16,!1)})}this.forcedInstanceCount=t>>4}_updateSubTextures(e,t,n,r,i,a,o){let s=(e,t,n,r,i)=>{this.getEngine().updateTextureData(e.getInternalTexture(),t,0,r,n,i,0,0,!1)},c=this._getTextureSize(this._vertexCount),l=this._useRGBACovariants?4:2,u=i*c.x,d=a*c.x,f=new Uint16Array(t.buffer,u*4*Uint16Array.BYTES_PER_ELEMENT,d*4),p=new Uint16Array(n.buffer,u*l*Uint16Array.BYTES_PER_ELEMENT,d*l),m=new Uint8Array(r.buffer,u*4,d*4),h=new Float32Array(e.buffer,u*4*Float32Array.BYTES_PER_ELEMENT,d*4);if(s(this._covariancesATexture,f,c.x,i,a),s(this._covariancesBTexture,p,c.x,i,a),s(this._centersTexture,h,c.x,i,a),s(this._colorsTexture,m,c.x,i,a),o)for(let e=0;e<o.length;e++){let t=new Uint32Array(o[e].buffer,u*4*4,d*4);s(this._shTextures[e],t,c.x,i,a)}}_instanciateWorker(){if(!this._vertexCount||this._disableDepthSort||(this._updateSplatIndexBuffer(this._vertexCount),_native))return;this._worker?.terminate(),this._worker=new Worker(URL.createObjectURL(new Blob([`(`,e._CreateWorker.toString(),`)(self)`],{type:`application/javascript`})));let t=this._vertexCount+15&-16;this._depthMix=new BigInt64Array(t);let n=Float32Array.from(this._splatPositions);this._worker.postMessage({positions:n},[n.buffer]),this._worker.onmessage=e=>{this._depthMix=e.data.depthMix;let n=e.data.cameraId,r=new Uint32Array(e.data.depthMix.buffer);if(this._splatIndex)for(let e=0;e<t;e++)this._splatIndex[e]=r[2*e];if(this._delayedTextureUpdate){let e=this._getTextureSize(t);this._updateSubTextures(this._delayedTextureUpdate.centers,this._delayedTextureUpdate.covA,this._delayedTextureUpdate.covB,this._delayedTextureUpdate.colors,0,e.y,this._delayedTextureUpdate.sh),this._delayedTextureUpdate=null}let i=this._cameraViewInfos.get(n);i&&(i.splatIndexBufferSet?i.mesh.thinInstanceBufferUpdated(`splatIndex`):(i.mesh.thinInstanceSetBuffer(`splatIndex`,this._splatIndex,16,!1),i.splatIndexBufferSet=!0)),this._canPostToWorker=!0,this._readyToDisplay=!0,this._sortIsDirty&&=(this._postToWorker(!0),!1)}}_getTextureSize(e){let t=this._scene.getEngine(),n=t.getCaps().maxTextureSize,r=1;if(t.version===1&&!t.isWebGPU)for(;n*r<e;)r*=2;else r=Math.ceil(e/n);return r>n&&(h.Error(`GaussianSplatting texture size: (`+n+`, `+r+`), maxTextureSize: `+n),r=n),new s(n,r)}};$._RowOutputLength=32,$._SH_C0=.28209479177387814,$._SplatBatchSize=327680,$._PlyConversionBatchSize=32768,$._BatchSize=16,$.ProgressiveUpdateAmount=0,$._CreateWorker=function(e){let t,n,r,i;e.onmessage=a=>{if(a.data.positions)t=a.data.positions;else{let o=a.data.cameraId,s=a.data.view,c=t.length/4+15&-16;if(!t||!s)throw Error(`positions or view is not defined!`);n=a.data.depthMix,r=new Uint32Array(n.buffer),i=new Float32Array(n.buffer);for(let e=0;e<c;e++)r[2*e]=e;let l=-1;a.data.useRightHandedSystem&&(l=1);for(let e=0;e<c;e++)i[2*e+1]=1e4+(s[2]*t[4*e+0]+s[6]*t[4*e+1]+s[10]*t[4*e+2])*l;n.sort(),e.postMessage({depthMix:n,cameraId:o},[n.buffer])}}};var Ve=class{constructor(e,t,n,r,i){this.idx=0,this.color=new f(1,1,1,1),this.position=o.Zero(),this.rotation=o.Zero(),this.uv=new s(0,0),this.velocity=o.Zero(),this.pivot=o.Zero(),this.translateFromPivot=!1,this._pos=0,this._ind=0,this.groupId=0,this.idxInGroup=0,this._stillInvisible=!1,this._rotationMatrix=[1,0,0,0,1,0,0,0,1],this.parentId=null,this._globalPosition=o.Zero(),this.idx=e,this._group=t,this.groupId=n,this.idxInGroup=r,this._pcs=i}get size(){return this.size}set size(e){this.size=e}get quaternion(){return this.rotationQuaternion}set quaternion(e){this.rotationQuaternion=e}intersectsMesh(e,t){if(!e.hasBoundingInfo)return!1;if(!this._pcs.mesh)throw Error(`Point Cloud System doesnt contain the Mesh`);if(t)return e.getBoundingInfo().boundingSphere.intersectsPoint(this.position.add(this._pcs.mesh.position));let n=e.getBoundingInfo().boundingBox,r=n.maximumWorld.x,i=n.minimumWorld.x,a=n.maximumWorld.y,o=n.minimumWorld.y,s=n.maximumWorld.z,c=n.minimumWorld.z,l=this.position.x+this._pcs.mesh.position.x,u=this.position.y+this._pcs.mesh.position.y,d=this.position.z+this._pcs.mesh.position.z;return i<=l&&l<=r&&o<=u&&u<=a&&c<=d&&d<=s}getRotationMatrix(e){let t;if(this.rotationQuaternion)t=this.rotationQuaternion;else{t=u.Quaternion[0];let e=this.rotation;c.RotationYawPitchRollToRef(e.y,e.x,e.z,t)}t.toRotationMatrix(e)}},He=class{get groupID(){return this.groupId}set groupID(e){this.groupId=e}constructor(e,t){this.groupId=e,this._positionFunction=t}},Ue;(function(e){e[e.Color=2]=`Color`,e[e.UV=1]=`UV`,e[e.Random=0]=`Random`,e[e.Stated=3]=`Stated`})(Ue||={});var We=class{get positions(){return this._positions32}get colors(){return this._colors32}get uvs(){return this._uvs32}constructor(e,t,r,i){this.particles=[],this.nbParticles=0,this.counter=0,this.vars={},this._promises=[],this._positions=[],this._indices=[],this._normals=[],this._colors=[],this._uvs=[],this._updatable=!0,this._isVisibilityBoxLocked=!1,this._alwaysVisible=!1,this._groups=[],this._groupCounter=0,this._computeParticleColor=!0,this._computeParticleTexture=!0,this._computeParticleRotation=!0,this._computeBoundingBox=!1,this._isReady=!1,this.name=e,this._size=t,this._scene=r||n.LastCreatedScene,i&&i.updatable!==void 0?this._updatable=i.updatable:this._updatable=!0}async buildMeshAsync(e){return await Promise.all(this._promises),this._isReady=!0,await this._buildMeshAsync(e)}async _buildMeshAsync(e){this.nbParticles===0&&this.addPoints(1),this._positions32=new Float32Array(this._positions),this._uvs32=new Float32Array(this._uvs),this._colors32=new Float32Array(this._colors);let t=new O;t.set(this._positions32,b.PositionKind),this._uvs32.length>0&&t.set(this._uvs32,b.UVKind);let n=0;this._colors32.length>0&&(n=1,t.set(this._colors32,b.ColorKind));let r=new A(this.name,this._scene);t.applyToMesh(r,this._updatable),this.mesh=r,this._positions=null,this._uvs=null,this._colors=null,this._updatable||(this.particles.length=0);let i=e;return i||(i=new me(`point cloud material`,this._scene),i.emissiveColor=new p(n,n,n),i.disableLighting=!0,i.pointsCloud=!0,i.pointSize=this._size),r.material=i,r}_addParticle(e,t,n,r){let i=new Ve(e,t,n,r,this);return this.particles.push(i),i}_randomUnitVector(e){e.position=new o(Math.random(),Math.random(),Math.random()),e.color=new f(1,1,1,1)}_getColorIndicesForCoord(e,t,n,r){let i=e._groupImageData,a=r*4*n+t*4,o=[a,a+1,a+2,a+3],s=o[0],c=o[1],l=o[2],u=o[3],d=i[s],p=i[c],m=i[l],h=i[u];return new f(d/255,p/255,m/255,h)}_setPointsColorOrUV(e,t,n,r,a,c,u,d){d??=0,n&&e.updateFacetData();let m=2*e.getBoundingInfo().boundingSphere.radius,h=e.getVerticesData(b.PositionKind),g=e.getIndices(),_=e.getVerticesData(b.UVKind+(d?d+1:``)),v=e.getVerticesData(b.ColorKind),y=o.Zero();e.computeWorldMatrix();let x=e.getWorldMatrix();if(!x.isIdentity()){h=h.slice(0);for(let e=0;e<h.length/3;e++)o.TransformCoordinatesFromFloatsToRef(h[3*e],h[3*e+1],h[3*e+2],x,y),h[3*e]=y.x,h[3*e+1]=y.y,h[3*e+2]=y.z}let C=0,w=0,T=0,E=0,D=0,ee=0,O=0,te=0,ne=0,re=0,ie=0,ae=0,oe=0,se=o.Zero(),ce=o.Zero(),le=o.Zero(),ue=o.Zero(),de=o.Zero(),fe=0,k=0,pe=0,A=0,me=0,he=0,ge=s.Zero(),_e=s.Zero(),j=s.Zero(),ve=s.Zero(),ye=s.Zero(),M=0,N=0,be=0,P=0,xe=0,Se=0,Ce=0,F=0,we=0,I=0,Te=0,Ee=0,L=l.Zero(),De=l.Zero(),R=l.Zero(),Oe=l.Zero(),ke=l.Zero(),z=0,B=0;u||=0;let V,H,U=new l(0,0,0,0),Ae=o.Zero(),je=o.Zero(),Me=o.Zero(),W=0,Ne=o.Zero(),Pe=0,Fe=0,G=new S(o.Zero(),new o(1,0,0)),K,q=o.Zero();for(let s=0;s<g.length/3;s++){w=g[3*s],T=g[3*s+1],E=g[3*s+2],D=h[3*w],ee=h[3*w+1],O=h[3*w+2],te=h[3*T],ne=h[3*T+1],re=h[3*T+2],ie=h[3*E],ae=h[3*E+1],oe=h[3*E+2],se.set(D,ee,O),ce.set(te,ne,re),le.set(ie,ae,oe),ce.subtractToRef(se,ue),le.subtractToRef(ce,de),_&&(fe=_[2*w],k=_[2*w+1],pe=_[2*T],A=_[2*T+1],me=_[2*E],he=_[2*E+1],ge.set(fe,k),_e.set(pe,A),j.set(me,he),_e.subtractToRef(ge,ve),j.subtractToRef(_e,ye)),v&&r&&(M=v[4*w],N=v[4*w+1],be=v[4*w+2],P=v[4*w+3],xe=v[4*T],Se=v[4*T+1],Ce=v[4*T+2],F=v[4*T+3],we=v[4*E],I=v[4*E+1],Te=v[4*E+2],Ee=v[4*E+3],L.set(M,N,be,P),De.set(xe,Se,Ce,F),R.set(we,I,Te,Ee),De.subtractToRef(L,Oe),R.subtractToRef(De,ke));let l,d,y,b,x,S,J,Y,Ie=new p(0,0,0),X=new p(0,0,0),Z,Q;for(let h=0;h<t._groupDensity[s];h++)C=this.particles.length,this._addParticle(C,t,this._groupCounter,s+h),Q=this.particles[C],z=Math.sqrt(i(0,1)),B=i(0,1),V=se.add(ue.scale(z)).add(de.scale(z*B)),n&&(Ae=e.getFacetNormal(s).normalize().scale(-1),je=ue.clone().normalize(),Me=o.Cross(Ae,je),W=i(0,2*Math.PI),Ne=je.scale(Math.cos(W)).add(Me.scale(Math.sin(W))),W=i(.1,Math.PI/2),q=Ne.scale(Math.cos(W)).add(Ae.scale(Math.sin(W))),G.origin=V.add(q.scale(1e-5)),G.direction=q,G.length=m,K=G.intersectsMesh(e),K.hit&&(Fe=K.pickedPoint.subtract(V).length(),Pe=i(0,1)*Fe,V.addInPlace(q.scale(Pe)))),Q.position=V.clone(),this._positions.push(Q.position.x,Q.position.y,Q.position.z),r===void 0?(c?(Ie.set(c.r,c.g,c.b),y=i(-u,u),b=i(-u,u),Y=Ie.toHSV(),x=Y.r,S=Y.g+y,J=Y.b+b,S<0&&(S=0),S>1&&(S=1),J<0&&(J=0),J>1&&(J=1),p.HSVtoRGBToRef(x,S,J,X),U.set(X.r,X.g,X.b,1)):U=L.set(Math.random(),Math.random(),Math.random(),1),Q.color=new f(U.x,U.y,U.z,U.w),this._colors.push(U.x,U.y,U.z,U.w)):_&&(H=ge.add(ve.scale(z)).add(ye.scale(z*B)),r?a&&t._groupImageData!==null?(l=t._groupImgWidth,d=t._groupImgHeight,Z=this._getColorIndicesForCoord(t,Math.round(H.x*l),Math.round(H.y*d),l),Q.color=Z,this._colors.push(Z.r,Z.g,Z.b,Z.a)):v?(U=L.add(Oe.scale(z)).add(ke.scale(z*B)),Q.color=new f(U.x,U.y,U.z,U.w),this._colors.push(U.x,U.y,U.z,U.w)):(U=L.set(Math.random(),Math.random(),Math.random(),1),Q.color=new f(U.x,U.y,U.z,U.w),this._colors.push(U.x,U.y,U.z,U.w)):(Q.uv=H.clone(),this._uvs.push(Q.uv.x,Q.uv.y)))}}_colorFromTexture(e,t,n){if(e.material===null){h.Warn(e.name+`has no material.`),t._groupImageData=null,this._setPointsColorOrUV(e,t,n,!0,!1);return}let r=e.material.getActiveTextures();if(r.length===0){h.Warn(e.name+`has no usable texture.`),t._groupImageData=null,this._setPointsColorOrUV(e,t,n,!0,!1);return}let i=e.clone();i.setEnabled(!1),this._promises.push(new Promise(e=>{C.WhenAllReady(r,()=>{let a=t._textureNb;a<0&&(a=0),a>r.length-1&&(a=r.length-1);let o=()=>{t._groupImgWidth=r[a].getSize().width,t._groupImgHeight=r[a].getSize().height,this._setPointsColorOrUV(i,t,n,!0,!0,void 0,void 0,r[a].coordinatesIndex),i.dispose(),e()};t._groupImageData=null;let s=r[a].readPixels();s?s.then(e=>{t._groupImageData=e,o()}):o()})}))}_calculateDensity(e,t,n){let r,i,a,s,c,l,u,d,f,p,m,h,g=o.Zero(),_=o.Zero(),v=o.Zero(),y=o.Zero(),b=o.Zero(),x=o.Zero(),S,C=[],w=0,T=n.length/3;for(let e=0;e<T;e++)r=n[3*e],i=n[3*e+1],a=n[3*e+2],s=t[3*r],c=t[3*r+1],l=t[3*r+2],u=t[3*i],d=t[3*i+1],f=t[3*i+2],p=t[3*a],m=t[3*a+1],h=t[3*a+2],g.set(s,c,l),_.set(u,d,f),v.set(p,m,h),_.subtractToRef(g,y),v.subtractToRef(_,b),o.CrossToRef(y,b,x),S=.5*x.length(),w+=S,C[e]=w;let E=Array(T),D=e;for(let e=T-1;e>0;e--){let t=C[e];if(t===0)E[e]=0;else{let n=(t-C[e-1])/t*D,r=Math.floor(n),i=n-r,a=r+Number(Math.random()<i);E[e]=a,D-=a}}return E[0]=D,E}addPoints(e,t=this._randomUnitVector){let n=new He(this._groupCounter,t),r,i=this.nbParticles;for(let t=0;t<e;t++)r=this._addParticle(i,n,this._groupCounter,t),n&&n._positionFunction&&n._positionFunction(r,i,t),this._positions.push(r.position.x,r.position.y,r.position.z),r.color&&this._colors.push(r.color.r,r.color.g,r.color.b,r.color.a),r.uv&&this._uvs.push(r.uv.x,r.uv.y),i++;return this.nbParticles+=e,this._groupCounter++,this._groupCounter}addSurfacePoints(e,t,n,r,i){let a=n||0;(isNaN(a)||a<0||a>3)&&(a=0);let o=e.getVerticesData(b.PositionKind),s=e.getIndices();this._groups.push(this._groupCounter);let c=new He(this._groupCounter,null);switch(c._groupDensity=this._calculateDensity(t,o,s),a===2?c._textureNb=r||0:r||=new f(1,1,1,1),a){case 2:this._colorFromTexture(e,c,!1);break;case 1:this._setPointsColorOrUV(e,c,!1,!1,!1);break;case 0:this._setPointsColorOrUV(e,c,!1);break;case 3:this._setPointsColorOrUV(e,c,!1,void 0,void 0,r,i);break}return this.nbParticles+=t,this._groupCounter++,this._groupCounter-1}addVolumePoints(e,t,n,r,i){let a=n||0;(isNaN(a)||a<0||a>3)&&(a=0);let o=e.getVerticesData(b.PositionKind),s=e.getIndices();this._groups.push(this._groupCounter);let c=new He(this._groupCounter,null);switch(c._groupDensity=this._calculateDensity(t,o,s),a===2?c._textureNb=r||0:r||=new f(1,1,1,1),a){case 2:this._colorFromTexture(e,c,!0);break;case 1:this._setPointsColorOrUV(e,c,!0,!1,!1);break;case 0:this._setPointsColorOrUV(e,c,!0);break;case 3:this._setPointsColorOrUV(e,c,!0,void 0,void 0,r,i);break}return this.nbParticles+=t,this._groupCounter++,this._groupCounter-1}setParticles(e=0,t=this.nbParticles-1,n=!0){if(!this._updatable||!this._isReady)return this;this.beforeUpdateParticles(e,t,n);let r=u.Matrix[0],i=this.mesh,a=this._colors32,o=this._positions32,s=this._uvs32,c=u.Vector3,l=c[5].copyFromFloats(1,0,0),f=c[6].copyFromFloats(0,1,0),p=c[7].copyFromFloats(0,0,1),m=c[8].setAll(Number.MAX_VALUE),h=c[9].setAll(-Number.MAX_VALUE);d.IdentityToRef(r);let g=0;if(this.mesh?.isFacetDataEnabled&&(this._computeBoundingBox=!0),t=t>=this.nbParticles?this.nbParticles-1:t,this._computeBoundingBox&&(e!=0||t!=this.nbParticles-1)){let e=this.mesh?.getBoundingInfo();e&&(m.copyFrom(e.minimum),h.copyFrom(e.maximum))}g=0;let _=0,v=0,y=0;for(let n=e;n<=t;n++){let e=this.particles[n];g=e.idx,_=3*g,v=4*g,y=2*g,this.updateParticle(e);let t=e._rotationMatrix,i=e.position,a=e._globalPosition;if(this._computeParticleRotation&&e.getRotationMatrix(r),e.parentId!==null){let n=this.particles[e.parentId],o=n._rotationMatrix,s=n._globalPosition,c=i.x*o[1]+i.y*o[4]+i.z*o[7],l=i.x*o[0]+i.y*o[3]+i.z*o[6],u=i.x*o[2]+i.y*o[5]+i.z*o[8];if(a.x=s.x+l,a.y=s.y+c,a.z=s.z+u,this._computeParticleRotation){let e=r.m;t[0]=e[0]*o[0]+e[1]*o[3]+e[2]*o[6],t[1]=e[0]*o[1]+e[1]*o[4]+e[2]*o[7],t[2]=e[0]*o[2]+e[1]*o[5]+e[2]*o[8],t[3]=e[4]*o[0]+e[5]*o[3]+e[6]*o[6],t[4]=e[4]*o[1]+e[5]*o[4]+e[6]*o[7],t[5]=e[4]*o[2]+e[5]*o[5]+e[6]*o[8],t[6]=e[8]*o[0]+e[9]*o[3]+e[10]*o[6],t[7]=e[8]*o[1]+e[9]*o[4]+e[10]*o[7],t[8]=e[8]*o[2]+e[9]*o[5]+e[10]*o[8]}}else if(a.x=0,a.y=0,a.z=0,this._computeParticleRotation){let e=r.m;t[0]=e[0],t[1]=e[1],t[2]=e[2],t[3]=e[4],t[4]=e[5],t[5]=e[6],t[6]=e[8],t[7]=e[9],t[8]=e[10]}let s=c[11];e.translateFromPivot?s.setAll(0):s.copyFrom(e.pivot);let u=c[0];u.copyFrom(e.position);let d=u.x-e.pivot.x,b=u.y-e.pivot.y,x=u.z-e.pivot.z,S=d*t[0]+b*t[3]+x*t[6],C=d*t[1]+b*t[4]+x*t[7],w=d*t[2]+b*t[5]+x*t[8];S+=s.x,C+=s.y,w+=s.z;let T=o[_]=a.x+l.x*S+f.x*C+p.x*w,E=o[_+1]=a.y+l.y*S+f.y*C+p.y*w,D=o[_+2]=a.z+l.z*S+f.z*C+p.z*w;if(this._computeBoundingBox&&(m.minimizeInPlaceFromFloats(T,E,D),h.maximizeInPlaceFromFloats(T,E,D)),this._computeParticleColor&&e.color){let t=e.color,n=this._colors32;n[v]=t.r,n[v+1]=t.g,n[v+2]=t.b,n[v+3]=t.a}if(this._computeParticleTexture&&e.uv){let t=e.uv,n=this._uvs32;n[y]=t.x,n[y+1]=t.y}}return i&&(n&&(this._computeParticleColor&&i.updateVerticesData(b.ColorKind,a,!1,!1),this._computeParticleTexture&&i.updateVerticesData(b.UVKind,s,!1,!1),i.updateVerticesData(b.PositionKind,o,!1,!1)),this._computeBoundingBox&&(i.hasBoundingInfo?i.getBoundingInfo().reConstruct(m,h,i._worldMatrix):i.buildBoundingInfo(m,h,i._worldMatrix))),this.afterUpdateParticles(e,t,n),this}dispose(){this.mesh?.dispose(),this.vars=null,this._positions=null,this._indices=null,this._normals=null,this._uvs=null,this._colors=null,this._indices32=null,this._positions32=null,this._uvs32=null,this._colors32=null}refreshVisibleSize(){return this._isVisibilityBoxLocked||this.mesh?.refreshBoundingInfo(),this}setVisibilityBox(e){if(!this.mesh)return;let t=e/2;this.mesh.buildBoundingInfo(new o(-t,-t,-t),new o(t,t,t))}get isAlwaysVisible(){return this._alwaysVisible}set isAlwaysVisible(e){this.mesh&&(this._alwaysVisible=e,this.mesh.alwaysSelectAsActiveMesh=e)}set computeParticleRotation(e){this._computeParticleRotation=e}set computeParticleColor(e){this._computeParticleColor=e}set computeParticleTexture(e){this._computeParticleTexture=e}get computeParticleColor(){return this._computeParticleColor}get computeParticleTexture(){return this._computeParticleTexture}set computeBoundingBox(e){this._computeBoundingBox=e}get computeBoundingBox(){return this._computeBoundingBox}initParticles(){}recycleParticle(e){return e}updateParticle(e){return e}beforeUpdateParticles(e,t,n){}afterUpdateParticles(e,t,n){}};function Ge(e,t,n){let r=new Uint8Array(e),i=new Uint32Array(e.slice(0,12)),a=i[2],o=r[12],s=r[13],c=r[14],l=r[15],u=i[1];if(l||i[0]!=1347635022||u!=2&&u!=3)return new Promise(e=>{e({mode:3,data:d,hasVertexColors:!1})});let d=new ArrayBuffer(32*a),f=1/(1<<s),p=new Int32Array(1),m=new Uint8Array(p.buffer),h=function(e,t){return m[0]=e[t+0],m[1]=e[t+1],m[2]=e[t+2],m[3]=e[t+2]&128?255:0,p[0]*f},g=16,_=new Float32Array(d),v=new Float32Array(d),y=new Uint8ClampedArray(d),b=new Uint8ClampedArray(d);for(let e=0;e<a;e++)_[e*8+0]=h(r,g+0),_[e*8+1]=h(r,g+3),_[e*8+2]=h(r,g+6),g+=9;for(let e=0;e<a;e++){for(let t=0;t<3;t++){let n=(r[g+a+e*3+t]-127.5)/(.15*255);y[e*32+24+t]=X.Clamp((.5+.282*n)*255,0,255)}y[e*32+24+3]=r[g+e]}g+=a*4;for(let e=0;e<a;e++)v[e*8+3+0]=Math.exp(r[g+0]/16-10),v[e*8+3+1]=Math.exp(r[g+1]/16-10),v[e*8+3+2]=Math.exp(r[g+2]/16-10),g+=3;if(u>=3){let e=Math.SQRT1_2;for(let t=0;t<a;t++){let n=[r[g+0],r[g+1],r[g+2],r[g+3]],i=n[0]+(n[1]<<8)+(n[2]<<16)+(n[3]<<24),a=[],o=i>>>30,s=i,c=0;for(let t=3;t>=0;--t)if(t!==o){let n=s&511,r=s>>>9&1;s>>>=10,a[t]=n/511*e,r===1&&(a[t]=-a[t]),c+=a[t]*a[t]}let l=1-c;a[o]=Math.sqrt(Math.max(l,0));let u=[3,0,1,2];for(let e=0;e<4;e++)b[t*32+28+e]=Math.round(127.5+a[u[e]]*127.5);g+=4}}else for(let e=0;e<a;e++){let t=r[g+0],n=r[g+1],i=r[g+2],a=t/127.5-1,o=n/127.5-1,s=i/127.5-1;b[e*32+28+1]=t,b[e*32+28+2]=n,b[e*32+28+3]=i;let c=1-(a*a+o*o+s*s);b[e*32+28+0]=127.5+Math.sqrt(c<0?0:c)*127.5,g+=3}if(o){let e=((o+1)*(o+1)-1)*3,n=Math.ceil(e/16),i=g,s=[],l=t.getEngine().getCaps().maxTextureSize,u=Math.ceil(a/l);for(let e=0;e<n;e++){let e=new Uint8Array(u*l*4*4);s.push(e)}for(let t=0;t<a;t++)for(let n=0;n<e;n++){let e=r[i++],a=s[Math.floor(n/16)],o=n%16,c=t*16;a[o+c]=e}return new Promise(e=>{e({mode:0,data:d,hasVertexColors:!1,sh:s,trainedWithAntialiasing:!!c})})}return new Promise(e=>{e({mode:0,data:d,hasVertexColors:!1,trainedWithAntialiasing:!!c})})}var Ke=.28209479177387814;async function qe(e,t,n){return await new Promise((r,i)=>{let a=n.createCanvasImage();if(!a)throw Error(`Failed to create ImageBitmap`);a.onload=()=>{try{let e=n.createCanvas(a.width,a.height);if(!e)throw Error(`Failed to create canvas`);let t=e.getContext(`2d`);if(!t)throw Error(`Failed to get 2D context`);t.drawImage(a,0,0);let i=t.getImageData(0,0,e.width,e.height);r({bits:new Uint8Array(i.data.buffer),width:i.width})}catch(e){i(`Error loading image ${a.src} with exception: ${e}`)}},a.onerror=e=>{i(`Error loading image ${a.src} with exception: ${e}`)},a.crossOrigin=`anonymous`;let o;if(typeof e==`string`){if(!t)throw Error(`filename is required when using a URL`);a.src=e+t}else{let t=new Blob([e],{type:`image/webp`});o=URL.createObjectURL(t),a.src=o}})}async function Je(e,t,n){let r=e.count?e.count:e.means.shape[0],i=new ArrayBuffer(32*r),a=new Float32Array(i),o=new Float32Array(i),s=new Uint8ClampedArray(i),c=new Uint8ClampedArray(i),l=e=>Math.sign(e)*(Math.exp(Math.abs(e))-1),u=t[0].bits,d=t[1].bits;if(!Array.isArray(e.means.mins)||!Array.isArray(e.means.maxs))throw Error(`Missing arrays in SOG data.`);for(let t=0;t<r;t++){let n=t*4;for(let r=0;r<3;r++){let i=e.means.mins[r],o=e.means.maxs[r],s=d[n+r],c=u[n+r],f=s<<8|c,p=X.Lerp(i,o,f/65535);a[t*8+r]=l(p)}}let f=t[2].bits;if(e.version===2){if(!e.scales.codebook)throw Error(`Missing codebook in SOG version 2 scales data.`);for(let t=0;t<r;t++){let n=t*4;for(let r=0;r<3;r++){let i=e.scales.codebook[f[n+r]],a=Math.exp(i);o[t*8+3+r]=a}}}else{if(!Array.isArray(e.scales.mins)||!Array.isArray(e.scales.maxs))throw Error(`Missing arrays in SOG scales data.`);for(let t=0;t<r;t++){let n=t*4;for(let r=0;r<3;r++){let i=f[n+r],a=X.Lerp(e.scales.mins[r],e.scales.maxs[r],i/255),s=Math.exp(a);o[t*8+3+r]=s}}}let p=t[4].bits;if(e.version===2){if(!e.sh0.codebook)throw Error(`Missing codebook in SOG version 2 sh0 data.`);for(let t=0;t<r;t++){let n=t*4;for(let r=0;r<3;r++){let i=.5+e.sh0.codebook[p[n+r]]*Ke;s[t*32+24+r]=Math.max(0,Math.min(255,Math.round(255*i)))}s[t*32+24+3]=p[n+3]}}else{if(!Array.isArray(e.sh0.mins)||!Array.isArray(e.sh0.maxs))throw Error(`Missing arrays in SOG sh0 data.`);for(let t=0;t<r;t++){let n=t*4;for(let r=0;r<4;r++){let i=e.sh0.mins[r],a=e.sh0.maxs[r],o=p[n+r],c=X.Lerp(i,a,o/255),l;l=r<3?.5+c*Ke:1/(1+Math.exp(-c)),s[t*32+24+r]=Math.max(0,Math.min(255,Math.round(255*l)))}}}let m=e=>(e/255-.5)*2/Math.SQRT2,h=t[3].bits;for(let e=0;e<r;e++){let t=h[e*4+0],n=h[e*4+1],r=h[e*4+2],i=h[e*4+3],a=m(t),o=m(n),s=m(r),l=i-252,u=a*a+o*o+s*s,d=Math.sqrt(Math.max(0,1-u)),f;switch(l){case 0:f=[d,a,o,s];break;case 1:f=[a,d,o,s];break;case 2:f=[a,o,d,s];break;case 3:f=[a,o,s,d];break;default:throw Error(`Invalid quaternion mode`)}c[e*32+28+0]=f[0]*127.5+127.5,c[e*32+28+1]=f[1]*127.5+127.5,c[e*32+28+2]=f[2]*127.5+127.5,c[e*32+28+3]=f[3]*127.5+127.5}if(e.shN){let a=e.shN.bands?[0,3,8,15][e.shN.bands]:e.shN.shape[1]/3,o=t[5].bits,s=t[6].bits,c=t[5].width,l=a*3,u=Math.ceil(l/16),d=[],f=n.getEngine().getCaps().maxTextureSize,p=Math.ceil(r/f);for(let e=0;e<u;e++){let e=new Uint8Array(p*f*4*4);d.push(e)}if(e.version===2){if(!e.shN.codebook)throw Error(`Missing codebook in SOG version 2 shN data.`);for(let t=0;t<r;t++){let n=s[t*4+0]+(s[t*4+1]<<8),r=n%64*a,i=Math.floor(n/64);for(let n=0;n<a;n++)for(let a=0;a<3;a++){let s=n*3+a,l=d[Math.floor(s/16)],u=s%16,f=t*16,p=e.shN.codebook[o[(r+n)*4+a+i*c*4]]*127.5+127.5;l[u+f]=Math.max(0,Math.min(255,p))}}}else for(let t=0;t<r;t++){let n=s[t*4+0]+(s[t*4+1]<<8),r=n%64*a,i=Math.floor(n/64),l=e.shN.mins,u=e.shN.maxs;for(let e=0;e<3;e++)for(let n=0;n<a/3;n++){let a=n*3+e,s=d[Math.floor(a/16)],f=a%16,p=t*16,m=X.Lerp(l,u,o[(r+n)*4+e+i*c*4]/255)*127.5+127.5;s[f+p]=Math.max(0,Math.min(255,m))}}return await new Promise(e=>{e({mode:0,data:i,hasVertexColors:!1,sh:d})})}return await new Promise(e=>{e({mode:0,data:i,hasVertexColors:!1})})}async function Ye(e,t,n){let r,i;if(e instanceof Map){i=e;let t=i.get(`meta.json`);if(!t)throw Error(`meta.json not found in files Map`);r=JSON.parse(new TextDecoder().decode(t))}else r=e;let a=[...r.means.files,...r.scales.files,...r.quats.files,...r.sh0.files];r.shN&&a.push(...r.shN.files);let o=await Promise.all(a.map(async e=>i&&i.has(e)?await qe(i.get(e),e,n.getEngine()):await qe(t,e,n.getEngine())));return await Je(r,o,n)}var Xe=class e{constructor(t=e._DefaultLoadingOptions){this.name=j.name,this._assetContainer=null,this.extensions=j.extensions,this._loadingOptions=t}createPlugin(t){return new e(t[j.name])}async importMeshAsync(e,t,n,r,i,a){return await this._parseAsync(e,t,n,r).then(e=>({meshes:e,particleSystems:[],skeletons:[],animationGroups:[],transformNodes:[],geometries:[],lights:[],spriteManagers:[]}))}static _BuildPointCloud(e,t){if(!t.byteLength)return!1;let n=new Uint8Array(t),r=new Float32Array(t),i=n.length/32;return e.addPoints(i,function(e,t){let i=r[8*t+0],a=r[8*t+1],s=r[8*t+2];e.position=new o(i,a,s),e.color=new f(n[32*t+24+0]/255,n[32*t+24+1]/255,n[32*t+24+2]/255,1)}),!0}static _BuildMesh(e,t){let n=new A(`PLYMesh`,e),r=new Uint8Array(t.data),i=new Float32Array(t.data),a=r.length/32,o=[],s=new O;for(let e=0;e<a;e++){let t=i[8*e+0],n=i[8*e+1],r=i[8*e+2];o.push(t,n,r)}if(t.hasVertexColors){let e=new Float32Array(a*4);for(let t=0;t<a;t++){let n=r[32*t+24+0]/255,i=r[32*t+24+1]/255,a=r[32*t+24+2]/255;e[t*4+0]=n,e[t*4+1]=i,e[t*4+2]=a,e[t*4+3]=1}s.colors=e}return s.positions=o,s.indices=t.faces,s.applyToMesh(n),n}async _unzipWithFFlateAsync(e){let t=this._loadingOptions.fflate;t||=(window.fflate===void 0&&await g.LoadScriptAsync(this._loadingOptions.deflateURL??`https://unpkg.com/fflate/umd/index.js`),window.fflate);let{unzipSync:n}=t,r=n(e),i=new Map;for(let[e,t]of Object.entries(r))i.set(e,t);return i}_parseAsync(t,n,r,i){let a=[],s=e=>{n._blockEntityCollection=!!this._assetContainer;let t=this._loadingOptions.gaussianSplattingMesh??new $(`GaussianSplatting`,null,n,this._loadingOptions.keepInRam);t._parentContainer=this._assetContainer,a.push(t),t.updateData(e.data,e.sh,{flipY:!1}),t.scaling.y*=-1,t.computeWorldMatrix(!0),n._blockEntityCollection=!1};if(typeof r==`string`){let e=JSON.parse(r);if(e&&e.means&&e.scales&&e.quats&&e.sh0)return new Promise(t=>{Ye(e,i,n).then(e=>{s(e),t(a)}).catch(()=>{throw Error(`Failed to parse SOG data.`)})})}let c=r instanceof ArrayBuffer?new Uint8Array(r):r;if(c[0]===80&&c[1]===75)return new Promise(e=>{this._unzipWithFFlateAsync(c).then(t=>{Ye(t,i,n).then(t=>{s(t),e(a)}).catch(()=>{throw Error(`Failed to parse SOG zip data.`)})})});let l=new ReadableStream({start(e){e.enqueue(new Uint8Array(r)),e.close()}}),u=new DecompressionStream(`gzip`),d=l.pipeThrough(u);return new Promise(t=>{new Response(d).arrayBuffer().then(e=>{Ge(e,n,this._loadingOptions).then(e=>{n._blockEntityCollection=!!this._assetContainer;let r=this._loadingOptions.gaussianSplattingMesh??new $(`GaussianSplatting`,null,n,this._loadingOptions.keepInRam);if(e.trainedWithAntialiasing){let e=r.material;e.kernelSize=.1,e.compensation=!0}r._parentContainer=this._assetContainer,a.push(r),r.updateData(e.data,e.sh,{flipY:!1}),this._loadingOptions.flipY||(r.scaling.y*=-1,r.computeWorldMatrix(!0)),n._blockEntityCollection=!1,this.applyAutoCameraLimits(e,n),t(a)})}).catch(()=>{e._ConvertPLYToSplat(r).then(async r=>{switch(n._blockEntityCollection=!!this._assetContainer,r.mode){case 0:{let e=this._loadingOptions.gaussianSplattingMesh??new $(`GaussianSplatting`,null,n,this._loadingOptions.keepInRam);switch(e._parentContainer=this._assetContainer,a.push(e),e.updateData(r.data,r.sh,{flipY:!1}),e.scaling.y*=-1,r.chirality===`RightHanded`&&(e.scaling.y*=-1),r.upAxis){case`X`:e.rotation=new o(0,0,Math.PI/2);break;case`Y`:e.rotation=new o(0,0,Math.PI);break;case`Z`:e.rotation=new o(-Math.PI/2,Math.PI,0);break}e.computeWorldMatrix(!0)}break;case 1:{let t=new We(`PointCloud`,1,n);e._BuildPointCloud(t,r.data)?await t.buildMeshAsync().then(e=>{a.push(e)}):t.dispose()}break;case 2:if(r.faces)a.push(e._BuildMesh(n,r));else throw Error(`PLY mesh doesn't contain face informations.`);break;default:throw Error(`Unsupported Splat mode`)}n._blockEntityCollection=!1,this.applyAutoCameraLimits(r,n),t(a)})})})}applyAutoCameraLimits(e,t){if(!this._loadingOptions.disableAutoCameraLimits&&(e.safeOrbitCameraRadiusMin!==void 0||e.safeOrbitCameraElevationMinMax!==void 0)&&t.activeCamera?.getClassName()===`ArcRotateCamera`){let n=t.activeCamera;e.safeOrbitCameraElevationMinMax&&(n.lowerBetaLimit=Math.PI*.5-e.safeOrbitCameraElevationMinMax[1],n.upperBetaLimit=Math.PI*.5-e.safeOrbitCameraElevationMinMax[0]),e.safeOrbitCameraRadiusMin&&(n.lowerRadiusLimit=e.safeOrbitCameraRadiusMin)}}loadAssetContainerAsync(e,t,n){let r=new _e(e);return this._assetContainer=r,this.importMeshAsync(null,e,t,n).then(e=>{for(let t of e.meshes)r.meshes.push(t);return this._assetContainer=null,r}).catch(e=>{throw this._assetContainer=null,e})}loadAsync(e,t,n){return this.importMeshAsync(null,e,t,n).then(()=>{})}static _ConvertPLYToSplat(e){let t=new Uint8Array(e),n=new TextDecoder().decode(t.slice(0,1024*10)),r=n.indexOf(`end_header
`);if(r<0||!n)return new Promise(t=>{t({mode:0,data:e,rawSplat:!0})});let i=parseInt(/element vertex (\d+)\n/.exec(n)[1]),a=/element face (\d+)\n/.exec(n),o=0;a&&(o=parseInt(a[1]));let s=/element chunk (\d+)\n/.exec(n),c=0;s&&(c=parseInt(s[1]));let l=0,u=0,d={double:8,int:4,uint:4,float:4,short:2,ushort:2,uchar:1,list:0},f={Vertex:0,Chunk:1,SH:2,Float_Tuple:3,Float:4,Uchar:5},p=f.Chunk,m=[],g=[],_=n.slice(0,r).split(`
`),v={};for(let t of _)if(t.startsWith(`property `)){let[,n,r]=t.split(` `);if(p==f.Chunk)g.push({name:r,type:n,offset:u}),u+=d[n];else if(p==f.Vertex)m.push({name:r,type:n,offset:l}),l+=d[n];else if(p==f.SH)m.push({name:r,type:n,offset:l});else if(p==f.Float_Tuple){let t=new DataView(e,u,d.float*2);v.safeOrbitCameraElevationMinMax=[t.getFloat32(0,!0),t.getFloat32(4,!0)]}else if(p==f.Float)v.safeOrbitCameraRadiusMin=new DataView(e,u,d.float).getFloat32(0,!0);else if(p==f.Uchar){let t=new DataView(e,u,d.uchar);r==`up_axis`?v.upAxis=t.getUint8(0)==0?`X`:t.getUint8(0)==1?`Y`:`Z`:r==`chirality`&&(v.chirality=t.getUint8(0)==0?`LeftHanded`:`RightHanded`)}d[n]||h.Warn(`Unsupported property type: ${n}.`)}else if(t.startsWith(`element `)){let[,e]=t.split(` `);e==`chunk`?p=f.Chunk:e==`vertex`?p=f.Vertex:e==`sh`?p=f.SH:e==`safe_orbit_camera_elevation_min_max_radians`?p=f.Float_Tuple:e==`safe_orbit_camera_radius_min`?p=f.Float:(e==`up_axis`||e==`chirality`)&&(p=f.Uchar)}let y=l,b=u;return $.ConvertPLYWithSHToSplatAsync(e).then(async t=>{let n=new DataView(e,r+11),a=b*c+y*i,s=[];if(o)for(let e=0;e<o;e++){let e=n.getUint8(a);if(e==3){a+=1;for(let t=0;t<e;t++){let e=n.getUint32(a+(2-t)*4,!0);s.push(e)}a+=12}}if(c)return await new Promise(e=>{e({mode:0,data:t.buffer,sh:t.sh,faces:s,hasVertexColors:!1,compressed:!0,rawSplat:!1})});let l=0,u=0,d=[`x`,`y`,`z`,`scale_0`,`scale_1`,`scale_2`,`opacity`,`rot_0`,`rot_1`,`rot_2`,`rot_3`],f=[`red`,`green`,`blue`,`f_dc_0`,`f_dc_1`,`f_dc_2`];for(let e=0;e<m.length;e++){let t=m[e];d.includes(t.name)&&l++,f.includes(t.name)&&u++}let p=l==d.length&&u==3,h=o?2:+!p;return await new Promise(e=>{e({...v,mode:h,data:t.buffer,sh:t.sh,faces:s,hasVertexColors:!!u,compressed:!1,rawSplat:!1})})})}};Xe._DefaultLoadingOptions={keepInRam:!1,flipY:!1},he(new Xe);export{Xe as SPLATFileLoader,be as i,ke as n,Ee as r,U as t};