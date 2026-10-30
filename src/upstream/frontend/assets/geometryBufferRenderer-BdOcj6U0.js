const __vite__mapDeps=(i,m=__vite__mapDeps,d=(m.f||(m.f=["./geometry.vertex-Bs20sk8Q.js","./shaderStore-D-XQlhUT.js","./bakedVertexAnimation-DS-FWsHx.js","./instancesVertex-BN7clrit.js","./bonesDeclaration-B7wjhknY.js","./bonesVertex-BIfu6COM.js","./bumpVertex-ZynzIDrr.js","./clipPlaneVertex-gMyNlLVT.js","./instancesDeclaration-DWorEgDR.js","./morphTargetsVertex-BRUNyA6N.js","./morphTargetsVertexDeclaration-C19K6Bin.js","./sceneUboDeclaration-TbZ5RU0g.js","./geometry.fragment-BbR0RHzc.js","./bumpFragment-CAlQ_ztm.js","./samplerFragmentDeclaration-I1t02tES.js","./clipPlaneFragment-DGqZJNeS.js","./helperFunctions-DquCMwzt.js"])))=>i.map(i=>d[i]);
import{bn as e,gn as t}from"./index-D88pgWqG.js";import{t as n}from"./math.vector-BxfzvQ1g.js";import{n as r}from"./math.color-B-P5_lZD.js";import{t as i}from"./devTools-DmD3AOL3.js";import{t as a}from"./shaderStore-D-XQlhUT.js";import{n as o}from"./buffer-RYS_o-MH.js";import"./engine.multiRender-BoCLqmSa.js";import{t as s}from"./texture-2OuxwQhP.js";import{A as c,D as l,M as u,j as d,k as f,m as p,o as m,s as h}from"./materialHelper.functions-DLwUvPy9.js";import"./clipPlaneFragment-CSA2ud-B.js";import"./bakedVertexAnimation-ByrDP9UX.js";import"./morphTargetsVertex--kSZKp4V.js";import"./clipPlaneVertex-CGSrwNQT.js";import"./instancesDeclaration-B_vMdLHG.js";import{t as g}from"./material-BA6wJUX2.js";import{t as _}from"./multiRenderTarget-BUBQC9s0.js";import"./bumpFragment-CXPA1fUd.js";import"./helperFunctions-CM1SYQ_G.js";import"./sceneUboDeclaration-CR5dQRQU.js";import"./bumpVertex-Djx3Dn9n.js";var v=`mrtFragmentDeclaration`,y=`#if defined(WEBGL2) || defined(WEBGPU) || defined(NATIVE)
layout(location=0) out vec4 glFragData[{X}];
#endif
`;a.IncludesShadersStore[v]||(a.IncludesShadersStore[v]=y);var b=e({geometryPixelShader:()=>C}),x=`geometryPixelShader`,S=`#extension GL_EXT_draw_buffers : require
#if defined(BUMP) || !defined(NORMAL)
#extension GL_OES_standard_derivatives : enable
#endif
precision highp float;
#ifdef BUMP
varying mat4 vWorldView;varying vec3 vNormalW;
#else
varying vec3 vNormalV;
#endif
varying vec4 vViewPos;
#if defined(POSITION) || defined(BUMP)
varying vec3 vPositionW;
#endif
#if defined(VELOCITY) || defined(VELOCITY_LINEAR)
varying vec4 vCurrentPosition;varying vec4 vPreviousPosition;
#endif
#ifdef NEED_UV
varying vec2 vUV;
#endif
#ifdef BUMP
uniform vec3 vBumpInfos;uniform vec2 vTangentSpaceParams;
#endif
#if defined(REFLECTIVITY)
#if defined(ORMTEXTURE) || defined(SPECULARGLOSSINESSTEXTURE) || defined(REFLECTIVITYTEXTURE)
uniform sampler2D reflectivitySampler;varying vec2 vReflectivityUV;
#else
#ifdef METALLIC_TEXTURE
uniform sampler2D metallicSampler;varying vec2 vMetallicUV;
#endif
#ifdef ROUGHNESS_TEXTURE
uniform sampler2D roughnessSampler;varying vec2 vRoughnessUV;
#endif
#endif
#ifdef ALBEDOTEXTURE
varying vec2 vAlbedoUV;uniform sampler2D albedoSampler;
#endif
#ifdef REFLECTIVITYCOLOR
uniform vec3 reflectivityColor;
#endif
#ifdef ALBEDOCOLOR
uniform vec3 albedoColor;
#endif
#ifdef METALLIC
uniform float metallic;
#endif
#if defined(ROUGHNESS) || defined(GLOSSINESS)
uniform float glossiness;
#endif
#endif
#if defined(ALPHATEST) && defined(NEED_UV)
uniform sampler2D diffuseSampler;
#endif
#include<clipPlaneFragmentDeclaration>
#include<mrtFragmentDeclaration>[SCENE_MRT_COUNT]
#include<bumpFragmentMainFunctions>
#include<bumpFragmentFunctions>
#include<helperFunctions>
void main() {
#include<clipPlaneFragment>
#ifdef ALPHATEST
if (texture2D(diffuseSampler,vUV).a<0.4)
discard;
#endif
vec3 normalOutput;
#ifdef BUMP
vec3 normalW=normalize(vNormalW);
#include<bumpFragment>
#ifdef NORMAL_WORLDSPACE
normalOutput=normalW;
#else
normalOutput=normalize(vec3(vWorldView*vec4(normalW,0.0)));
#endif
#elif defined(HAS_NORMAL_ATTRIBUTE)
normalOutput=normalize(vNormalV);
#elif defined(POSITION)
normalOutput=normalize(-cross(dFdx(vPositionW),dFdy(vPositionW)));
#endif
#ifdef ENCODE_NORMAL
normalOutput=normalOutput*0.5+0.5;
#endif
#ifdef DEPTH
gl_FragData[DEPTH_INDEX]=vec4(vViewPos.z/vViewPos.w,0.0,0.0,1.0);
#endif
#ifdef NORMAL
gl_FragData[NORMAL_INDEX]=vec4(normalOutput,1.0);
#endif
#ifdef SCREENSPACE_DEPTH
gl_FragData[SCREENSPACE_DEPTH_INDEX]=vec4(gl_FragCoord.z,0.0,0.0,1.0);
#endif
#ifdef POSITION
gl_FragData[POSITION_INDEX]=vec4(vPositionW,1.0);
#endif
#ifdef VELOCITY
vec2 a=(vCurrentPosition.xy/vCurrentPosition.w)*0.5+0.5;vec2 b=(vPreviousPosition.xy/vPreviousPosition.w)*0.5+0.5;vec2 velocity=abs(a-b);velocity=vec2(pow(velocity.x,1.0/3.0),pow(velocity.y,1.0/3.0))*sign(a-b)*0.5+0.5;gl_FragData[VELOCITY_INDEX]=vec4(velocity,0.0,1.0);
#endif
#ifdef VELOCITY_LINEAR
vec2 velocity=vec2(0.5)*((vPreviousPosition.xy/vPreviousPosition.w) -
(vCurrentPosition.xy/vCurrentPosition.w));gl_FragData[VELOCITY_LINEAR_INDEX]=vec4(velocity,0.0,1.0);
#endif
#ifdef REFLECTIVITY
vec4 reflectivity=vec4(0.0,0.0,0.0,1.0);
#ifdef METALLICWORKFLOW
float metal=1.0;float roughness=1.0;
#ifdef ORMTEXTURE
metal*=texture2D(reflectivitySampler,vReflectivityUV).b;roughness*=texture2D(reflectivitySampler,vReflectivityUV).g;
#else
#ifdef METALLIC_TEXTURE
metal*=texture2D(metallicSampler,vMetallicUV).r;
#endif
#ifdef ROUGHNESS_TEXTURE
roughness*=texture2D(roughnessSampler,vRoughnessUV).r;
#endif
#endif
#ifdef METALLIC
metal*=metallic;
#endif
#ifdef ROUGHNESS
roughness*=(1.0-glossiness); 
#endif
reflectivity.a-=roughness;vec3 color=vec3(1.0);
#ifdef ALBEDOTEXTURE
color=texture2D(albedoSampler,vAlbedoUV).rgb;
#ifdef GAMMAALBEDO
color=toLinearSpace(color);
#endif
#endif
#ifdef ALBEDOCOLOR
color*=albedoColor.xyz;
#endif
reflectivity.rgb=mix(vec3(0.04),color,metal);
#else
#if defined(SPECULARGLOSSINESSTEXTURE) || defined(REFLECTIVITYTEXTURE)
reflectivity=texture2D(reflectivitySampler,vReflectivityUV);
#ifdef GAMMAREFLECTIVITYTEXTURE
reflectivity.rgb=toLinearSpace(reflectivity.rgb);
#endif
#else 
#ifdef REFLECTIVITYCOLOR
reflectivity.rgb=toLinearSpace(reflectivityColor.xyz);reflectivity.a=1.0;
#endif
#endif
#ifdef GLOSSINESSS
reflectivity.a*=glossiness; 
#endif
#endif
gl_FragData[REFLECTIVITY_INDEX]=reflectivity;
#endif
}
`;a.ShadersStore[x]||(a.ShadersStore[x]=S);var C={name:x,shader:S},w=`geometryVertexDeclaration`,T=`uniform mat4 viewProjection;uniform mat4 view;`;a.IncludesShadersStore[w]||(a.IncludesShadersStore[w]=T);var E=`geometryUboDeclaration`,D=`#include<sceneUboDeclaration>
`;a.IncludesShadersStore[E]||(a.IncludesShadersStore[E]=D);var O=e({geometryVertexShader:()=>j}),k=`geometryVertexShader`,A=`precision highp float;
#include<bonesDeclaration>
#include<bakedVertexAnimationDeclaration>
#include<morphTargetsVertexGlobalDeclaration>
#include<morphTargetsVertexDeclaration>[0..maxSimultaneousMorphTargets]
#include<instancesDeclaration>
#include<__decl__geometryVertex>
#include<clipPlaneVertexDeclaration>
attribute vec3 position;
#ifdef HAS_NORMAL_ATTRIBUTE
attribute vec3 normal;
#endif
#ifdef NEED_UV
varying vec2 vUV;
#ifdef ALPHATEST
uniform mat4 diffuseMatrix;
#endif
#ifdef BUMP
uniform mat4 bumpMatrix;varying vec2 vBumpUV;
#endif
#ifdef REFLECTIVITY
uniform mat4 reflectivityMatrix;uniform mat4 albedoMatrix;varying vec2 vReflectivityUV;varying vec2 vAlbedoUV;
#endif
#ifdef METALLIC_TEXTURE
varying vec2 vMetallicUV;uniform mat4 metallicMatrix;
#endif
#ifdef ROUGHNESS_TEXTURE
varying vec2 vRoughnessUV;uniform mat4 roughnessMatrix;
#endif
#ifdef UV1
attribute vec2 uv;
#endif
#ifdef UV2
attribute vec2 uv2;
#endif
#endif
#ifdef BUMP
varying mat4 vWorldView;
#endif
#ifdef BUMP
varying vec3 vNormalW;
#else
varying vec3 vNormalV;
#endif
varying vec4 vViewPos;
#if defined(POSITION) || defined(BUMP)
varying vec3 vPositionW;
#endif
#if defined(VELOCITY) || defined(VELOCITY_LINEAR)
uniform mat4 previousViewProjection;varying vec4 vCurrentPosition;varying vec4 vPreviousPosition;
#endif
#define CUSTOM_VERTEX_DEFINITIONS
void main(void)
{vec3 positionUpdated=position;
#ifdef HAS_NORMAL_ATTRIBUTE
vec3 normalUpdated=normal;
#else
vec3 normalUpdated=vec3(0.0,0.0,0.0);
#endif
#ifdef UV1
vec2 uvUpdated=uv;
#endif
#ifdef UV2
vec2 uv2Updated=uv2;
#endif
#include<morphTargetsVertexGlobal>
#include<morphTargetsVertex>[0..maxSimultaneousMorphTargets]
#include<instancesVertex>
#if (defined(VELOCITY) || defined(VELOCITY_LINEAR)) && !defined(BONES_VELOCITY_ENABLED)
vCurrentPosition=viewProjection*finalWorld*vec4(positionUpdated,1.0);vPreviousPosition=previousViewProjection*finalPreviousWorld*vec4(positionUpdated,1.0);
#endif
#include<bonesVertex>
#include<bakedVertexAnimation>
vec4 worldPos=vec4(finalWorld*vec4(positionUpdated,1.0));
#ifdef BUMP
vWorldView=view*finalWorld;mat3 normalWorld=mat3(finalWorld);vNormalW=normalize(normalWorld*normalUpdated);
#else
#ifdef NORMAL_WORLDSPACE
vNormalV=normalize(vec3(finalWorld*vec4(normalUpdated,0.0)));
#else
vNormalV=normalize(vec3((view*finalWorld)*vec4(normalUpdated,0.0)));
#endif
#endif
vViewPos=view*worldPos;
#if (defined(VELOCITY) || defined(VELOCITY_LINEAR)) && defined(BONES_VELOCITY_ENABLED)
vCurrentPosition=viewProjection*finalWorld*vec4(positionUpdated,1.0);
#if NUM_BONE_INFLUENCERS>0
mat4 previousInfluence;previousInfluence=mPreviousBones[int(matricesIndices[0])]*matricesWeights[0];
#if NUM_BONE_INFLUENCERS>1
previousInfluence+=mPreviousBones[int(matricesIndices[1])]*matricesWeights[1];
#endif
#if NUM_BONE_INFLUENCERS>2
previousInfluence+=mPreviousBones[int(matricesIndices[2])]*matricesWeights[2];
#endif
#if NUM_BONE_INFLUENCERS>3
previousInfluence+=mPreviousBones[int(matricesIndices[3])]*matricesWeights[3];
#endif
#if NUM_BONE_INFLUENCERS>4
previousInfluence+=mPreviousBones[int(matricesIndicesExtra[0])]*matricesWeightsExtra[0];
#endif
#if NUM_BONE_INFLUENCERS>5
previousInfluence+=mPreviousBones[int(matricesIndicesExtra[1])]*matricesWeightsExtra[1];
#endif
#if NUM_BONE_INFLUENCERS>6
previousInfluence+=mPreviousBones[int(matricesIndicesExtra[2])]*matricesWeightsExtra[2];
#endif
#if NUM_BONE_INFLUENCERS>7
previousInfluence+=mPreviousBones[int(matricesIndicesExtra[3])]*matricesWeightsExtra[3];
#endif
vPreviousPosition=previousViewProjection*finalPreviousWorld*previousInfluence*vec4(positionUpdated,1.0);
#else
vPreviousPosition=previousViewProjection*finalPreviousWorld*vec4(positionUpdated,1.0);
#endif
#endif
#if defined(POSITION) || defined(BUMP)
vPositionW=worldPos.xyz/worldPos.w;
#endif
gl_Position=viewProjection*finalWorld*vec4(positionUpdated,1.0);
#include<clipPlaneVertex>
#ifdef NEED_UV
#ifdef UV1
#if defined(ALPHATEST) && defined(ALPHATEST_UV1)
vUV=vec2(diffuseMatrix*vec4(uvUpdated,1.0,0.0));
#else
vUV=uvUpdated;
#endif
#ifdef BUMP_UV1
vBumpUV=vec2(bumpMatrix*vec4(uvUpdated,1.0,0.0));
#endif
#ifdef REFLECTIVITY_UV1
vReflectivityUV=vec2(reflectivityMatrix*vec4(uvUpdated,1.0,0.0));
#else
#ifdef METALLIC_UV1
vMetallicUV=vec2(metallicMatrix*vec4(uvUpdated,1.0,0.0));
#endif
#ifdef ROUGHNESS_UV1
vRoughnessUV=vec2(roughnessMatrix*vec4(uvUpdated,1.0,0.0));
#endif
#endif
#ifdef ALBEDO_UV1
vAlbedoUV=vec2(albedoMatrix*vec4(uvUpdated,1.0,0.0));
#endif
#endif
#ifdef UV2
#if defined(ALPHATEST) && defined(ALPHATEST_UV2)
vUV=vec2(diffuseMatrix*vec4(uv2Updated,1.0,0.0));
#else
vUV=uv2Updated;
#endif
#ifdef BUMP_UV2
vBumpUV=vec2(bumpMatrix*vec4(uv2Updated,1.0,0.0));
#endif
#ifdef REFLECTIVITY_UV2
vReflectivityUV=vec2(reflectivityMatrix*vec4(uv2Updated,1.0,0.0));
#else
#ifdef METALLIC_UV2
vMetallicUV=vec2(metallicMatrix*vec4(uv2Updated,1.0,0.0));
#endif
#ifdef ROUGHNESS_UV2
vRoughnessUV=vec2(roughnessMatrix*vec4(uv2Updated,1.0,0.0));
#endif
#endif
#ifdef ALBEDO_UV2
vAlbedoUV=vec2(albedoMatrix*vec4(uv2Updated,1.0,0.0));
#endif
#endif
#endif
#include<bumpVertex>
}
`;a.ShadersStore[k]||(a.ShadersStore[k]=A);var j={name:k,shader:A},M=[`world`,`mBones`,`viewProjection`,`diffuseMatrix`,`view`,`previousWorld`,`previousViewProjection`,`mPreviousBones`,`bumpMatrix`,`reflectivityMatrix`,`albedoMatrix`,`reflectivityColor`,`albedoColor`,`metallic`,`glossiness`,`vTangentSpaceParams`,`vBumpInfos`,`morphTargetInfluences`,`morphTargetCount`,`morphTargetTextureInfo`,`morphTargetTextureIndices`,`boneTextureWidth`];c(M);var N=class e{get normalsAreUnsigned(){return this._normalsAreUnsigned}_linkPrePassRenderer(e){this._linkedWithPrePass=!0,this._prePassRenderer=e,this._multiRenderTarget&&(this._multiRenderTarget.onClearObservable.clear(),this._multiRenderTarget.onClearObservable.add(()=>{}))}_unlinkPrePassRenderer(){this._linkedWithPrePass=!1,this._createRenderTargets()}_resetLayout(){this._enableDepth=!0,this._enableNormal=!0,this._enablePosition=!1,this._enableReflectivity=!1,this._enableVelocity=!1,this._enableVelocityLinear=!1,this._enableScreenspaceDepth=!1,this._attachmentsFromPrePass=[]}_forceTextureType(t,n){t===e.POSITION_TEXTURE_TYPE?(this._positionIndex=n,this._enablePosition=!0):t===e.VELOCITY_TEXTURE_TYPE?(this._velocityIndex=n,this._enableVelocity=!0):t===e.VELOCITY_LINEAR_TEXTURE_TYPE?(this._velocityLinearIndex=n,this._enableVelocityLinear=!0):t===e.REFLECTIVITY_TEXTURE_TYPE?(this._reflectivityIndex=n,this._enableReflectivity=!0):t===e.DEPTH_TEXTURE_TYPE?(this._depthIndex=n,this._enableDepth=!0):t===e.NORMAL_TEXTURE_TYPE?(this._normalIndex=n,this._enableNormal=!0):t===e.SCREENSPACE_DEPTH_TEXTURE_TYPE&&(this._screenspaceDepthIndex=n,this._enableScreenspaceDepth=!0)}_setAttachments(e){this._attachmentsFromPrePass=e}_linkInternalTexture(e){this._multiRenderTarget.setInternalTexture(e,0,!1)}get renderList(){return this._multiRenderTarget.renderList}set renderList(e){this._multiRenderTarget.renderList=e}get isSupported(){return this._multiRenderTarget.isSupported}getTextureIndex(t){switch(t){case e.POSITION_TEXTURE_TYPE:return this._positionIndex;case e.VELOCITY_TEXTURE_TYPE:return this._velocityIndex;case e.VELOCITY_LINEAR_TEXTURE_TYPE:return this._velocityLinearIndex;case e.REFLECTIVITY_TEXTURE_TYPE:return this._reflectivityIndex;case e.DEPTH_TEXTURE_TYPE:return this._depthIndex;case e.NORMAL_TEXTURE_TYPE:return this._normalIndex;case e.SCREENSPACE_DEPTH_TEXTURE_TYPE:return this._screenspaceDepthIndex;default:return-1}}get enableDepth(){return this._enableDepth}set enableDepth(e){this._enableDepth=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get enableNormal(){return this._enableNormal}set enableNormal(e){this._enableNormal=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get enablePosition(){return this._enablePosition}set enablePosition(e){this._enablePosition=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get enableVelocity(){return this._enableVelocity}set enableVelocity(e){this._enableVelocity=e,e||(this._previousTransformationMatrices={}),this._linkedWithPrePass||(this.dispose(),this._createRenderTargets()),this._scene.needsPreviousWorldMatrices=e}get enableVelocityLinear(){return this._enableVelocityLinear}set enableVelocityLinear(e){this._enableVelocityLinear=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get enableReflectivity(){return this._enableReflectivity}set enableReflectivity(e){this._enableReflectivity=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get enableScreenspaceDepth(){return this._enableScreenspaceDepth}set enableScreenspaceDepth(e){this._enableScreenspaceDepth=e,this._linkedWithPrePass||(this.dispose(),this._createRenderTargets())}get scene(){return this._scene}get ratio(){return typeof this._ratioOrDimensions==`object`?1:this._ratioOrDimensions}get shaderLanguage(){return this._shaderLanguage}constructor(t,n=1,i=15,a){this._previousTransformationMatrices={},this._previousBonesTransformationMatrices={},this.excludedSkinnedMeshesFromVelocity=[],this.renderTransparentMeshes=!0,this.generateNormalsInWorldSpace=!1,this._normalsAreUnsigned=!1,this._resizeObserver=null,this._enableDepth=!0,this._enableNormal=!0,this._enablePosition=!1,this._enableVelocity=!1,this._enableVelocityLinear=!1,this._enableReflectivity=!1,this._enableScreenspaceDepth=!1,this._clearColor=new r(0,0,0,0),this._clearDepthColor=new r(0,0,0,1),this._positionIndex=-1,this._velocityIndex=-1,this._velocityLinearIndex=-1,this._reflectivityIndex=-1,this._depthIndex=-1,this._normalIndex=-1,this._screenspaceDepthIndex=-1,this._linkedWithPrePass=!1,this.useSpecificClearForDepthTexture=!1,this._shaderLanguage=0,this._shadersLoaded=!1,this._scene=t,this._ratioOrDimensions=n,this._useUbo=t.getEngine().supportsUniformBuffers,this._depthFormat=i,this._textureTypesAndFormats=a||{},this._initShaderSourceAsync(),e._SceneComponentInitialization(this._scene),this._createRenderTargets()}async _initShaderSourceAsync(){this._scene.getEngine().isWebGPU&&!e.ForceGLSL?(this._shaderLanguage=1,await Promise.all([t(()=>import(`./geometry.vertex-Bs20sk8Q.js`),__vite__mapDeps([0,1,2,3,4,5,6,7,8,9,10,11]),import.meta.url),t(()=>import(`./geometry.fragment-BbR0RHzc.js`),__vite__mapDeps([12,1,13,14,15,16]),import.meta.url)])):await Promise.all([t(()=>Promise.resolve().then(()=>O),void 0,import.meta.url),t(()=>Promise.resolve().then(()=>b),void 0,import.meta.url)]),this._shadersLoaded=!0}isReady(e,t){if(!this._shadersLoaded)return!1;let n=e.getMaterial();if(n&&n.disableDepthWrite)return!1;let r=[],i=[o.PositionKind],a=e.getMesh();a.isVerticesDataPresent(o.NormalKind)&&(r.push(`#define HAS_NORMAL_ATTRIBUTE`),i.push(o.NormalKind));let s=!1,c=!1;if(n){let e=!1;if(n.needAlphaTestingForMesh(a)&&n.getAlphaTestTexture()&&(r.push(`#define ALPHATEST`),r.push(`#define ALPHATEST_UV${n.getAlphaTestTexture().coordinatesIndex+1}`),e=!0),(n.bumpTexture||n.normalTexture||n.geometryNormalTexture)&&f.BumpTextureEnabled){let t=n.bumpTexture||n.normalTexture||n.geometryNormalTexture;r.push(`#define BUMP`),r.push(`#define BUMP_UV${t.coordinatesIndex+1}`),e=!0}if(this._enableReflectivity){let t=!1;if(n.getClassName()===`PBRMetallicRoughnessMaterial`)n.metallicRoughnessTexture&&(r.push(`#define ORMTEXTURE`),r.push(`#define REFLECTIVITY_UV${n.metallicRoughnessTexture.coordinatesIndex+1}`),r.push(`#define METALLICWORKFLOW`),e=!0,t=!0),n.metallic!=null&&(r.push(`#define METALLIC`),r.push(`#define METALLICWORKFLOW`),t=!0),n.roughness!=null&&(r.push(`#define ROUGHNESS`),r.push(`#define METALLICWORKFLOW`),t=!0),t&&(n.baseTexture&&(r.push(`#define ALBEDOTEXTURE`),r.push(`#define ALBEDO_UV${n.baseTexture.coordinatesIndex+1}`),n.baseTexture.gammaSpace&&r.push(`#define GAMMAALBEDO`),e=!0),n.baseColor&&r.push(`#define ALBEDOCOLOR`));else if(n.getClassName()===`PBRSpecularGlossinessMaterial`)n.specularGlossinessTexture?(r.push(`#define SPECULARGLOSSINESSTEXTURE`),r.push(`#define REFLECTIVITY_UV${n.specularGlossinessTexture.coordinatesIndex+1}`),e=!0,n.specularGlossinessTexture.gammaSpace&&r.push(`#define GAMMAREFLECTIVITYTEXTURE`)):n.specularColor&&r.push(`#define REFLECTIVITYCOLOR`),n.glossiness!=null&&r.push(`#define GLOSSINESS`);else if(n.getClassName()===`PBRMaterial`)n.metallicTexture&&(r.push(`#define ORMTEXTURE`),r.push(`#define REFLECTIVITY_UV${n.metallicTexture.coordinatesIndex+1}`),r.push(`#define METALLICWORKFLOW`),e=!0,t=!0),n.metallic!=null&&(r.push(`#define METALLIC`),r.push(`#define METALLICWORKFLOW`),t=!0),n.roughness!=null&&(r.push(`#define ROUGHNESS`),r.push(`#define METALLICWORKFLOW`),t=!0),t?(n.albedoTexture&&(r.push(`#define ALBEDOTEXTURE`),r.push(`#define ALBEDO_UV${n.albedoTexture.coordinatesIndex+1}`),n.albedoTexture.gammaSpace&&r.push(`#define GAMMAALBEDO`),e=!0),n.albedoColor&&r.push(`#define ALBEDOCOLOR`)):(n.reflectivityTexture?(r.push(`#define SPECULARGLOSSINESSTEXTURE`),r.push(`#define REFLECTIVITY_UV${n.reflectivityTexture.coordinatesIndex+1}`),n.reflectivityTexture.gammaSpace&&r.push(`#define GAMMAREFLECTIVITYTEXTURE`),e=!0):n.reflectivityColor&&r.push(`#define REFLECTIVITYCOLOR`),n.microSurface!=null&&r.push(`#define GLOSSINESS`));else if(n.getClassName()===`StandardMaterial`)n.specularTexture&&(r.push(`#define REFLECTIVITYTEXTURE`),r.push(`#define REFLECTIVITY_UV${n.specularTexture.coordinatesIndex+1}`),n.specularTexture.gammaSpace&&r.push(`#define GAMMAREFLECTIVITYTEXTURE`),e=!0),n.specularColor&&r.push(`#define REFLECTIVITYCOLOR`);else if(n.getClassName()===`OpenPBRMaterial`){let i=n;r.push(`#define METALLICWORKFLOW`),t=!0,r.push(`#define METALLIC`),r.push(`#define ROUGHNESS`),i._useRoughnessFromMetallicTextureGreen&&i.baseMetalnessTexture?(r.push(`#define ORMTEXTURE`),r.push(`#define REFLECTIVITY_UV${i.baseMetalnessTexture.coordinatesIndex+1}`),e=!0):i.baseMetalnessTexture?(r.push(`#define METALLIC_TEXTURE`),r.push(`#define METALLIC_UV${i.baseMetalnessTexture.coordinatesIndex+1}`),e=!0):i.specularRoughnessTexture&&(r.push(`#define ROUGHNESS_TEXTURE`),r.push(`#define ROUGHNESS_UV${i.specularRoughnessTexture.coordinatesIndex+1}`),e=!0),i.baseColorTexture&&(r.push(`#define ALBEDOTEXTURE`),r.push(`#define ALBEDO_UV${i.baseColorTexture.coordinatesIndex+1}`),i.baseColorTexture.gammaSpace&&r.push(`#define GAMMAALBEDO`),e=!0),i.baseColor&&r.push(`#define ALBEDOCOLOR`)}}e&&(r.push(`#define NEED_UV`),a.isVerticesDataPresent(o.UVKind)&&(i.push(o.UVKind),r.push(`#define UV1`),s=!0),a.isVerticesDataPresent(o.UV2Kind)&&(i.push(o.UV2Kind),r.push(`#define UV2`),c=!0))}this._enableDepth&&(r.push(`#define DEPTH`),r.push(`#define DEPTH_INDEX `+this._depthIndex)),this._enableNormal&&(r.push(`#define NORMAL`),r.push(`#define NORMAL_INDEX `+this._normalIndex)),this._enablePosition&&(r.push(`#define POSITION`),r.push(`#define POSITION_INDEX `+this._positionIndex)),this._enableVelocity&&(r.push(`#define VELOCITY`),r.push(`#define VELOCITY_INDEX `+this._velocityIndex),this.excludedSkinnedMeshesFromVelocity.indexOf(a)===-1&&r.push(`#define BONES_VELOCITY_ENABLED`)),this._enableVelocityLinear&&(r.push(`#define VELOCITY_LINEAR`),r.push(`#define VELOCITY_LINEAR_INDEX `+this._velocityLinearIndex),this.excludedSkinnedMeshesFromVelocity.indexOf(a)===-1&&r.push(`#define BONES_VELOCITY_ENABLED`)),this._enableReflectivity&&(r.push(`#define REFLECTIVITY`),r.push(`#define REFLECTIVITY_INDEX `+this._reflectivityIndex)),this._enableScreenspaceDepth&&this._screenspaceDepthIndex!==-1&&(r.push(`#define SCREENSPACE_DEPTH_INDEX `+this._screenspaceDepthIndex),r.push(`#define SCREENSPACE_DEPTH`)),this.generateNormalsInWorldSpace&&r.push(`#define NORMAL_WORLDSPACE`),this._normalsAreUnsigned&&r.push(`#define ENCODE_NORMAL`),a.useBones&&a.computeBonesUsingShaders&&a.skeleton?(i.push(o.MatricesIndicesKind),i.push(o.MatricesWeightsKind),a.numBoneInfluencers>4&&(i.push(o.MatricesIndicesExtraKind),i.push(o.MatricesWeightsExtraKind)),r.push(`#define NUM_BONE_INFLUENCERS `+a.numBoneInfluencers),r.push(`#define BONETEXTURE `+a.skeleton.isUsingTextureForMatrices),r.push(`#define BonesPerMesh `+(a.skeleton.bones.length+1))):(r.push(`#define NUM_BONE_INFLUENCERS 0`),r.push(`#define BONETEXTURE false`),r.push(`#define BonesPerMesh 0`));let d=a.morphTargetManager?p(a.morphTargetManager,r,i,a,!0,!0,!1,s,c,!1):0;t&&(r.push(`#define INSTANCES`),l(i,this._enableVelocity||this._enableVelocityLinear),e.getRenderingMesh().hasThinInstances&&r.push(`#define THIN_INSTANCES`)),this._linkedWithPrePass?r.push(`#define SCENE_MRT_COUNT `+this._attachmentsFromPrePass.length):r.push(`#define SCENE_MRT_COUNT `+this._multiRenderTarget.textures.length),u(n,this._scene,r);let m=this._scene.getEngine(),h=e._getDrawWrapper(void 0,!0),g=h.defines,_=r.join(`
`);return g!==_&&h.setEffect(m.createEffect(`geometry`,{attributes:i,uniformsNames:M,samplers:[`diffuseSampler`,`bumpSampler`,`reflectivitySampler`,`albedoSampler`,`morphTargets`,`boneSampler`],defines:_,onCompiled:null,fallbacks:null,onError:null,uniformBuffersNames:[`Scene`],indexParameters:{buffersCount:this._multiRenderTarget.textures.length-1,maxSimultaneousMorphTargets:d},shaderLanguage:this.shaderLanguage},m),_),h.effect.isReady()}getGBuffer(){return this._multiRenderTarget}get samples(){return this._multiRenderTarget.samples}set samples(e){this._multiRenderTarget.samples=e}dispose(){this._resizeObserver&&=(this._scene.getEngine().onResizeObservable.remove(this._resizeObserver),null),this.getGBuffer().dispose()}_assignRenderTargetIndices(){let t=[],n=[],r=0;return this._enableDepth&&(this._depthIndex=r,r++,t.push(`gBuffer_Depth`),n.push(this._textureTypesAndFormats[e.DEPTH_TEXTURE_TYPE])),this._enableNormal&&(this._normalIndex=r,r++,t.push(`gBuffer_Normal`),n.push(this._textureTypesAndFormats[e.NORMAL_TEXTURE_TYPE])),this._enablePosition&&(this._positionIndex=r,r++,t.push(`gBuffer_Position`),n.push(this._textureTypesAndFormats[e.POSITION_TEXTURE_TYPE])),this._enableVelocity&&(this._velocityIndex=r,r++,t.push(`gBuffer_Velocity`),n.push(this._textureTypesAndFormats[e.VELOCITY_TEXTURE_TYPE])),this._enableVelocityLinear&&(this._velocityLinearIndex=r,r++,t.push(`gBuffer_VelocityLinear`),n.push(this._textureTypesAndFormats[e.VELOCITY_LINEAR_TEXTURE_TYPE])),this._enableReflectivity&&(this._reflectivityIndex=r,r++,t.push(`gBuffer_Reflectivity`),n.push(this._textureTypesAndFormats[e.REFLECTIVITY_TEXTURE_TYPE])),this._enableScreenspaceDepth&&(this._screenspaceDepthIndex=r,r++,t.push(`gBuffer_ScreenspaceDepth`),n.push(this._textureTypesAndFormats[e.SCREENSPACE_DEPTH_TEXTURE_TYPE])),[r,t,n]}_createRenderTargets(){let t=this._scene.getEngine(),[r,i,a]=this._assignRenderTargetIndices(),o=0;t._caps.textureFloat&&t._caps.textureFloatLinearFiltering?o=1:t._caps.textureHalfFloat&&t._caps.textureHalfFloatLinearFiltering&&(o=2);let c=this._ratioOrDimensions.width===void 0?{width:t.getRenderWidth()*this._ratioOrDimensions,height:t.getRenderHeight()*this._ratioOrDimensions}:this._ratioOrDimensions,l=[],u=[];for(let e of a)e?(l.push(e.textureType),u.push(e.textureFormat)):(l.push(o),u.push(5));if(this._normalsAreUnsigned=l[e.NORMAL_TEXTURE_TYPE]===11||l[e.NORMAL_TEXTURE_TYPE]===13,this._multiRenderTarget=new _(`gBuffer`,c,r,this._scene,{generateMipMaps:!1,generateDepthTexture:!0,types:l,formats:u,depthTextureFormat:this._depthFormat},i.concat(`gBuffer_DepthBuffer`)),!this.isSupported)return;this._multiRenderTarget.wrapU=s.CLAMP_ADDRESSMODE,this._multiRenderTarget.wrapV=s.CLAMP_ADDRESSMODE,this._multiRenderTarget.refreshRate=1,this._multiRenderTarget.renderParticles=!1,this._multiRenderTarget.renderList=null;let p=[!0],v=[!1],y=[!0];for(let e=1;e<r;++e)p.push(!0),y.push(!1),v.push(!0);let b=t.buildTextureLayout(p),x=t.buildTextureLayout(v),S=t.buildTextureLayout(y);this._multiRenderTarget.onClearObservable.add(e=>{e.bindAttachments(this.useSpecificClearForDepthTexture?x:b),e.clear(this._clearColor,!0,!0,!0),this.useSpecificClearForDepthTexture&&(e.bindAttachments(S),e.clear(this._clearDepthColor,!0,!0,!0)),e.bindAttachments(b)}),this._resizeObserver=t.onResizeObservable.add(()=>{if(this._multiRenderTarget){let e=this._ratioOrDimensions.width===void 0?{width:t.getRenderWidth()*this._ratioOrDimensions,height:t.getRenderHeight()*this._ratioOrDimensions}:this._ratioOrDimensions;this._multiRenderTarget.resize(e)}});let C=e=>{let t=e.getRenderingMesh(),r=e.getEffectiveMesh(),i=this._scene,a=i.getEngine(),o=e.getMaterial();if(!o)return;if(r._internalAbstractMeshDataInfo._isActiveIntermediate=!1,(this._enableVelocity||this._enableVelocityLinear)&&!this._previousTransformationMatrices[r.uniqueId]&&(this._previousTransformationMatrices[r.uniqueId]={world:n.Identity(),viewProjection:i.getTransformMatrix()},t.skeleton)){let e=t.skeleton.getTransformMatrices(t);this._previousBonesTransformationMatrices[t.uniqueId]=this._copyBonesTransformationMatrices(e,new Float32Array(e.length))}let s=t._getInstancesRenderList(e._id,!!e.getReplacementMesh());if(s.mustReturn)return;let c=a.getCaps().instancedArrays&&(s.visibleInstances[e._id]!==null||t.hasThinInstances),l=r.getWorldMatrix();if(this.isReady(e,c)){let n=e._getDrawWrapper();if(!n)return;let u=n.effect;a.enableEffect(n),c||t._bind(e,u,o.fillMode),this._useUbo?(h(u,this._scene.getSceneUniformBuffer()),this._scene.finalizeSceneUbo()):(u.setMatrix(`viewProjection`,i.getTransformMatrix()),u.setMatrix(`view`,i.getViewMatrix()));let p;if(!t._instanceDataStorage.isFrozen&&(o.backFaceCulling||o.sideOrientation!==null)){let e=r._getWorldMatrixDeterminant();p=o._getEffectiveOrientation(t),e<0&&(p=p===g.ClockWiseSideOrientation?g.CounterClockWiseSideOrientation:g.ClockWiseSideOrientation)}else p=t._effectiveSideOrientation;if(o._preBind(n,p),o.needAlphaTestingForMesh(r)){let e=o.getAlphaTestTexture();e&&(u.setTexture(`diffuseSampler`,e),u.setMatrix(`diffuseMatrix`,e.getTextureMatrix()))}if((o.bumpTexture||o.normalTexture||o.geometryNormalTexture)&&i.getEngine().getCaps().standardDerivatives&&f.BumpTextureEnabled){let e=o.bumpTexture||o.normalTexture||o.geometryNormalTexture;u.setFloat3(`vBumpInfos`,e.coordinatesIndex,1/e.level,o.parallaxScaleBias),u.setMatrix(`bumpMatrix`,e.getTextureMatrix()),u.setTexture(`bumpSampler`,e),u.setFloat2(`vTangentSpaceParams`,o.invertNormalMapX?-1:1,o.invertNormalMapY?-1:1)}if(this._enableReflectivity){if(o.getClassName()===`PBRMetallicRoughnessMaterial`)o.metallicRoughnessTexture!==null&&(u.setTexture(`reflectivitySampler`,o.metallicRoughnessTexture),u.setMatrix(`reflectivityMatrix`,o.metallicRoughnessTexture.getTextureMatrix())),o.metallic!==null&&u.setFloat(`metallic`,o.metallic),o.roughness!==null&&u.setFloat(`glossiness`,1-o.roughness),o.baseTexture!==null&&(u.setTexture(`albedoSampler`,o.baseTexture),u.setMatrix(`albedoMatrix`,o.baseTexture.getTextureMatrix())),o.baseColor!==null&&u.setColor3(`albedoColor`,o.baseColor);else if(o.getClassName()===`PBRSpecularGlossinessMaterial`)o.specularGlossinessTexture===null?o.specularColor!==null&&u.setColor3(`reflectivityColor`,o.specularColor):(u.setTexture(`reflectivitySampler`,o.specularGlossinessTexture),u.setMatrix(`reflectivityMatrix`,o.specularGlossinessTexture.getTextureMatrix())),o.glossiness!==null&&u.setFloat(`glossiness`,o.glossiness);else if(o.getClassName()===`PBRMaterial`)o.metallicTexture!==null&&(u.setTexture(`reflectivitySampler`,o.metallicTexture),u.setMatrix(`reflectivityMatrix`,o.metallicTexture.getTextureMatrix())),o.metallic!==null&&u.setFloat(`metallic`,o.metallic),o.roughness!==null&&u.setFloat(`glossiness`,1-o.roughness),o.roughness!==null||o.metallic!==null||o.metallicTexture!==null?(o.albedoTexture!==null&&(u.setTexture(`albedoSampler`,o.albedoTexture),u.setMatrix(`albedoMatrix`,o.albedoTexture.getTextureMatrix())),o.albedoColor!==null&&u.setColor3(`albedoColor`,o.albedoColor)):(o.reflectivityTexture===null?o.reflectivityColor!==null&&u.setColor3(`reflectivityColor`,o.reflectivityColor):(u.setTexture(`reflectivitySampler`,o.reflectivityTexture),u.setMatrix(`reflectivityMatrix`,o.reflectivityTexture.getTextureMatrix())),o.microSurface!==null&&u.setFloat(`glossiness`,o.microSurface));else if(o.getClassName()===`StandardMaterial`)o.specularTexture!==null&&(u.setTexture(`reflectivitySampler`,o.specularTexture),u.setMatrix(`reflectivityMatrix`,o.specularTexture.getTextureMatrix())),o.specularColor!==null&&u.setColor3(`reflectivityColor`,o.specularColor);else if(o.getClassName()===`OpenPBRMaterial`){let e=o;e._useRoughnessFromMetallicTextureGreen&&e.baseMetalnessTexture?(u.setTexture(`reflectivitySampler`,e.baseMetalnessTexture),u.setMatrix(`reflectivityMatrix`,e.baseMetalnessTexture.getTextureMatrix())):e.baseMetalnessTexture?(u.setTexture(`metallicSampler`,e.baseMetalnessTexture),u.setMatrix(`metallicMatrix`,e.baseMetalnessTexture.getTextureMatrix())):e.specularRoughnessTexture&&(u.setTexture(`roughnessSampler`,e.specularRoughnessTexture),u.setMatrix(`roughnessMatrix`,e.specularRoughnessTexture.getTextureMatrix())),u.setFloat(`metallic`,e.baseMetalness),u.setFloat(`glossiness`,1-e.specularRoughness),e.baseColorTexture!==null&&(u.setTexture(`albedoSampler`,e.baseColorTexture),u.setMatrix(`albedoMatrix`,e.baseColorTexture.getTextureMatrix())),e.baseColor!==null&&u.setColor3(`albedoColor`,e.baseColor)}}if(d(u,o,this._scene),t.useBones&&t.computeBonesUsingShaders&&t.skeleton){let e=t.skeleton;if(e.isUsingTextureForMatrices&&u.getUniformIndex(`boneTextureWidth`)>-1){let n=e.getTransformMatrixTexture(t);u.setTexture(`boneSampler`,n),u.setFloat(`boneTextureWidth`,4*(e.bones.length+1))}else u.setMatrices(`mBones`,t.skeleton.getTransformMatrices(t));(this._enableVelocity||this._enableVelocityLinear)&&u.setMatrices(`mPreviousBones`,this._previousBonesTransformationMatrices[t.uniqueId])}m(t,u),t.morphTargetManager&&t.morphTargetManager.isUsingTextureForTargets&&t.morphTargetManager._bind(u),(this._enableVelocity||this._enableVelocityLinear)&&(u.setMatrix(`previousWorld`,this._previousTransformationMatrices[r.uniqueId].world),u.setMatrix(`previousViewProjection`,this._previousTransformationMatrices[r.uniqueId].viewProjection)),c&&t.hasThinInstances&&u.setMatrix(`world`,l),t._processRendering(r,e,u,o.fillMode,s,c,(e,t)=>{e||u.setMatrix(`world`,t)})}(this._enableVelocity||this._enableVelocityLinear)&&(this._previousTransformationMatrices[r.uniqueId].world=l.clone(),this._previousTransformationMatrices[r.uniqueId].viewProjection=this._scene.getTransformMatrix().clone(),t.skeleton&&this._copyBonesTransformationMatrices(t.skeleton.getTransformMatrices(t),this._previousBonesTransformationMatrices[r.uniqueId]))};this._multiRenderTarget.customIsReadyFunction=(e,n,r)=>{if((r||n===0)&&e.subMeshes)for(let n=0;n<e.subMeshes.length;++n){let r=e.subMeshes[n],i=r.getMaterial(),a=r.getRenderingMesh();if(!i)continue;let o=a._getInstancesRenderList(r._id,!!r.getReplacementMesh()),s=t.getCaps().instancedArrays&&(o.visibleInstances[r._id]!==null||a.hasThinInstances);if(!this.isReady(r,s))return!1}return!0},this._multiRenderTarget.customRenderFunction=(e,n,r,i)=>{let a;if(this._linkedWithPrePass){if(!this._prePassRenderer.enabled)return;this._scene.getEngine().bindAttachments(this._attachmentsFromPrePass)}if(i.length){for(t.setColorWrite(!1),a=0;a<i.length;a++)C(i.data[a]);t.setColorWrite(!0)}for(a=0;a<e.length;a++)C(e.data[a]);for(t.setDepthWrite(!1),a=0;a<n.length;a++)C(n.data[a]);if(this.renderTransparentMeshes)for(a=0;a<r.length;a++)C(r.data[a]);t.setDepthWrite(!0)}}_copyBonesTransformationMatrices(e,t){for(let n=0;n<e.length;n++)t[n]=e[n];return t}};N.ForceGLSL=!1,N.DEPTH_TEXTURE_TYPE=0,N.NORMAL_TEXTURE_TYPE=1,N.POSITION_TEXTURE_TYPE=2,N.VELOCITY_TEXTURE_TYPE=3,N.REFLECTIVITY_TEXTURE_TYPE=4,N.SCREENSPACE_DEPTH_TEXTURE_TYPE=5,N.VELOCITY_LINEAR_TEXTURE_TYPE=6,N._SceneComponentInitialization=e=>{throw i(`GeometryBufferRendererSceneComponent`)};export{N as t};