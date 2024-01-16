#ifndef SONAR
#define SONAR

void Sonar_float(
    UnityTexture2D waveData,
    UnitySamplerState samplerState,
    float3 position,
    float waveWidth, float fresnelWidth,
    float3 waveColor, float3 fresnelColor,
    out float3 outColor)
{
    outColor = float3(0,0,0);
    
    uint width;
    uint height;
    waveData.tex.GetDimensions(width, height);
    
    // for (int i = 0; i < arraySize; i++)
    // {
    //     float x = waveData.tex.Load(i * 3 + 0,0).x;
    //     float y = waveData.tex.Load(i * 3 +  1,0).x;
    //     float z = waveData.tex.Load(i * 3 + 2,0).x;
    //     float3 origin = float3(x, y, z);
    //
    //     float range = waveData.tex.Load(i * 0,1).x;
    //     float strength = waveData.tex.Load(i * 1,1).x;
    //     
    //     float distance1 = length(origin - position) - range;
    //     outColor = distance1 <= 0 ? float3(0,0,0) : strength * waveColor;
    //     
    //     float distance2 = range + waveWidth - length(origin - position);
    //     outColor = distance2 <= 0 ? float3(0,0,0) : strength * waveColor;
    // }
    
    int i = 0;
    for (; i < height; i++)
    {
        float3 tempColor = float3(0,0,0);
        
        float x = SAMPLE_TEXTURE2D(waveData, waveData.samplerstate, float2(1.0 * 1.0 / width / 2.0, (float)i /height + 1.0/height/2.0)).r;
        float y = SAMPLE_TEXTURE2D(waveData, waveData.samplerstate, float2(3.0 * 1.0 / width / 2.0, (float)i /height + 1.0/height/2.0)).r;
        float z = SAMPLE_TEXTURE2D(waveData, waveData.samplerstate, float2(5.0 * 1.0 / width / 2.0, (float)i /height + 1.0/height/2.0)).r;
        float3 origin = float3(x, y, z);
    
        float range = SAMPLE_TEXTURE2D(waveData, waveData.samplerstate, float2(7.0 * 1.0 / width / 2.0, (float)i/height + 1.0/height/2.0)).r;
        float strength = SAMPLE_TEXTURE2D(waveData, waveData.samplerstate, float2(9.0 * 1.0 / width / 2.0, (float)i/height + 1.0/height/2.0)).r;

        float distance1 = length(origin - position) - range + waveWidth;

        tempColor = distance1 <= 0 ? float3(0,0,0) : lerp(
                //float3(0,0,0),
                ///lerp(
                    float3(0,0,0),
                    waveColor,
                    clamp(strength, 0, 1)
                //),
               // clamp(distance1 / waveWidth, 0, 1)
            );
         
        float distance2 = range - length(origin - position);
        tempColor = distance2 <= 0 ? float3(0,0,0) : tempColor;
        
        tempColor *= 0.5;
        outColor = float3(max(tempColor.x, outColor.x), max(tempColor.y, outColor.y),max(tempColor.z, outColor.z));
    }
    //outColor = float3((float)i / 20, 0, 0);
    
    //outColor = float3(distance1,0,0);
}

#endif