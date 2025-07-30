float2 SampleSceneNormalBuffer(float2 uv, float3x3 viewMatrix)
{
    // Optimized normal sampling and transformation
    float3 normalWorldSpace = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv);
    
    // Manual matrix multiplication for better performance
    float3 normalViewSpace;
    normalViewSpace.x = dot(viewMatrix[0], normalWorldSpace);
    normalViewSpace.y = dot(viewMatrix[1], normalWorldSpace);
    normalViewSpace.z = dot(viewMatrix[2], normalWorldSpace);
    return normalViewSpace.xy;
}

float CalculateCurvature(
    float2 left, float2 right, float2 down, float2 up, float exponent, float multiplier)
{
    // Optimized curvature calculation
    float totalResult = (left.x - right.x) + (up.y - down.y);
    
    // Branchless power calculation
    float absResult = abs(totalResult * multiplier);
    float poweredResult = exp2(exponent * log2(absResult));
    float curvature = 0.5 + sign(totalResult) * poweredResult;
    
    // Clamp without branching
    return saturate(curvature);
}

float GetCurvatureAtPoint(float2 uv, float exponent, float multiplier, float3x3 viewMatrix)
{
    // Precompute offsets once
    static const float2 leftRight = float2(1.0 / _ScreenParams.x, 0);
    static const float2 upDown = float2(0, 1.0 / _ScreenParams.y);
    
    // Sample all four directions in one go (better for GPU parallelism)
    float2 left = SampleSceneNormalBuffer(uv + leftRight, viewMatrix);
    float2 right = SampleSceneNormalBuffer(uv - leftRight, viewMatrix);
    float2 down = SampleSceneNormalBuffer(uv - upDown, viewMatrix);
    float2 up = SampleSceneNormalBuffer(uv + upDown, viewMatrix);
    
    return CalculateCurvature(left, right, down, up, exponent, multiplier);
}

void GetAverageCurvature_float(
    float2 screenPosition, int radius, float exponent, 
    float multiplier, float sharpness, out float curvature)
{
    float3x3 viewMatrix = (float3x3)UNITY_MATRIX_V;
    float totalWeight = 0.0;
    curvature = 0.0;
    
    // Precompute sharpness and inverse screen params
    sharpness = max(1.0 - sharpness, 0.0001);
    float2 invScreenParams = 1.0 / _ScreenParams.xy;
    
    for (int i = -radius; i <= radius; i++)
    {
        for (int j = -radius; j <= radius; j++)
        {
            float2 pixelOffset = float2(i, j);
            float2 uvOffset = pixelOffset * invScreenParams;
            
            // Optimized weight calculation
            float pixelDistSq = dot(pixelOffset, pixelOffset);
            float weight = rcp(pixelDistSq + sharpness);
            
            totalWeight += weight;
            curvature += weight * GetCurvatureAtPoint(screenPosition + uvOffset, exponent, multiplier, viewMatrix);
        }
    }
    
    curvature *= rcp(totalWeight);
}