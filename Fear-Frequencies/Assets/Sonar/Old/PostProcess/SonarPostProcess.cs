using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using TextureParameter = UnityEngine.Rendering.PostProcessing.TextureParameter;

namespace VertexFragment
{

    [PostProcess(typeof(SonarPostProcessRenderer), PostProcessEvent.BeforeStack, "SonarPostProcess")]
    public class SonarPostProcess : PostProcessEffectSettings
    {
        public TextureParameter waveData = new TextureParameter();
        public IntParameter width = new IntParameter { value = 5 };
        public IntParameter height = new IntParameter { value = 1 };
        public MatrixParameter inverseMatrix = new MatrixParameter(Matrix4x4.identity);
    }
    
    [Serializable]
    public sealed class MatrixParameter : ParameterOverride<Matrix4x4>
    {
        public MatrixParameter(Matrix4x4 _value)
        {
            value = _value;
        }
    }

    public sealed class SonarPostProcessRenderer : PostProcessEffectRenderer<SonarPostProcess>
    {
        public const string SobelShader = "VertexFragment/SonarCg";

        public override void Render(PostProcessRenderContext context)
        {
            var shader = Shader.Find(SobelShader);

            if (shader == null)
            {
                Debug.LogError($"Failed to get shader '{SobelShader}' for Sonar Post-Processing");
                return;
            }

            var sheet = context.propertySheets.Get(shader);

            if (sheet == null)
            {
                Debug.LogError($"Failed to get PropertySheet for Sonar Post-Processing effect.");
                return;
            }

            sheet.properties.SetTexture("_WaveData", settings.waveData.value);
            sheet.properties.SetInteger("_Width", settings.width.value);
            sheet.properties.SetInteger("_Height", settings.height.value);
            sheet.properties.SetMatrix("_InverseMatrix", settings.inverseMatrix.value);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}