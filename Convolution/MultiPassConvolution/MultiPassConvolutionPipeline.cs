using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
namespace Shaders.Convolution.MultiPassConvolution
{
    [CreateAssetMenu(fileName = "MultiPassConv", menuName = "Rendering/MultiPass Convolution Pipeline", order = 5)]
    public class MultiPassConvolutionPipeline : ScriptableObject
    {

        [Serializable]
        public class ConvolutionPass
        {
            public Material mat;
            public CustomRenderTexture texture;
        }
        public RenderTexture inputTexture;
        public CustomRenderTextureUpdateMode initMode = CustomRenderTextureUpdateMode.OnLoad;
        public CustomRenderTextureUpdateMode updateMode = CustomRenderTextureUpdateMode.Realtime;
        public List<Material> pipeline = new();
        [SerializeField] [HideInInspector] private List<ConvolutionPass> passes = new();

        [Button]
        private void Purge()
        {
            foreach (var pass in passes) {
                AssetDatabase.RemoveObjectFromAsset(pass.texture);
                AssetDatabase.RemoveObjectFromAsset(pass.mat);
            }
            passes.Clear();
            AssetDatabase.SaveAssets();
        }
        
        [Button]
        public void Configure()
        {
            Purge();
            var inputWidth = inputTexture.width;
            var inputHeight = inputTexture.height;
            var prev = inputTexture;
            var passCount = 1;
            foreach (var mat in pipeline) {
                var intermediate = new CustomRenderTexture(inputWidth, inputHeight, inputTexture.format);
                intermediate.initializationTexture = prev;
                intermediate.name = $"Pass[{passCount}]_" + mat.name;
                intermediate.updateMode = updateMode;
                intermediate.initializationMode = initMode;
                var matCopy = new Material(mat);
                AssetDatabase.AddObjectToAsset(matCopy, this);
                intermediate.material = matCopy;
                matCopy.SetTexture("_MainTex", prev);
                prev = intermediate;
                AssetDatabase.AddObjectToAsset(intermediate, this);
                passes.Add(new ConvolutionPass() {
                    mat = matCopy,
                    texture = intermediate
                });
                passCount++;
            }
            AssetDatabase.SaveAssets();
        }
    }
}