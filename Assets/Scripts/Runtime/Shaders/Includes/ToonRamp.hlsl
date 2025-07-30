void ToonShading_float(in float3 Normal, in float ToonRampSmoothness, in float4 ClipSpacePos, 
                      in float3 WorldPos, in float3 ToonRampTinting, in float ToonRampOffset, 
                      in float ToonRampOffsetPoint, in float Ambient, 
                      out float3 ToonRampOutput, out float3 Direction)
{
    #ifdef SHADERGRAPH_PREVIEW
        ToonRampOutput = float3(0.5,0.5,0);
        Direction = float3(0.5,0.5,0);
        return;
    #else
        // Shadow coordinates
        #if SHADOWS_SCREEN
            half4 shadowCoord = ComputeScreenPos(ClipSpacePos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif
        
        // Main light with conditional shadows
        #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
            Light light = GetMainLight(shadowCoord);
        #else
            Light light = GetMainLight();
        #endif

        // Main light calculation
        half d = saturate(dot(Normal, light.direction) * 0.5 + 0.5);
        half toonRamp = smoothstep(ToonRampOffset, ToonRampOffset + ToonRampSmoothness, d) * light.shadowAttenuation;
        
        // Additional lights
        half3 extraLights = float3(0,0,0); 

        // Create inputdata struct to use in LIGHT_LOOP
   		InputData inputData = (InputData)0;
        inputData.positionWS = WorldPos;
        inputData.normalWS = Normal;
        inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(WorldPos);
        //light disappearing outside of range fix thanks to  rsofia on github https://github.com/rsofia/CustomLightingForwardPlus
        float4 screenPos = float4(ClipSpacePos.x, (_ScaledScreenParams.y - ClipSpacePos.y), 0, 0);
        inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(screenPos);

        // forward and forward+ lights loop
        uint lightsCount = GetAdditionalLightsCount();
        LIGHT_LOOP_BEGIN(lightsCount)
            Light aLight = GetAdditionalLight(lightIndex, WorldPos, half4(1,1,1,1));
            half d = saturate(dot(Normal, aLight.direction) * 0.5 + 0.5);
            half3 attenuatedLightColor = aLight.color * (aLight.distanceAttenuation * aLight.shadowAttenuation);
            extraLights += smoothstep(ToonRampOffsetPoint, ToonRampOffsetPoint + ToonRampSmoothness, d) * attenuatedLightColor;
        LIGHT_LOOP_END

        // Final composition
        half3 tintedRamp = toonRamp + half3(ToonRampTinting);
        ToonRampOutput = light.color * tintedRamp + Ambient + extraLights;
        Direction = normalize(light.direction);
    #endif
}