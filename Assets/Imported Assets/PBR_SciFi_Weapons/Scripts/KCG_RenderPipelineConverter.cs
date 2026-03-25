using UnityEngine;
using UnityEditor;
using System.IO;

public class KCG_RenderPipelineConverter : EditorWindow
{
    private string targetFolder = "Assets/???";
    private bool convertExistingMaterials = false;
    private float defaultSmoothness = 0.5f;

    [MenuItem("Tools/KCG_RenderPipelineConverter")]
    public static void ShowWindow()
    {
        GetWindow<KCG_RenderPipelineConverter>("KCG_RenderPipelineConverter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Convert Materials", EditorStyles.boldLabel);

        targetFolder = EditorGUILayout.TextField("Target Folder", targetFolder);
        convertExistingMaterials = EditorGUILayout.Toggle("Convert Existing Materials", convertExistingMaterials);
        
        GUILayout.Label("Material Settings", EditorStyles.boldLabel);
        defaultSmoothness = EditorGUILayout.Slider("Default Smoothness", defaultSmoothness, 0f, 1f);

        if (GUILayout.Button("Convert to HDRP"))
        {
            ConvertMaterials("HDRP/Lit", "HDRP_mats");
        }

        if (GUILayout.Button("Convert to URP"))
        {
            ConvertMaterials("Universal Render Pipeline/Lit", "URP_mats");
        }

        if (GUILayout.Button("Convert to Built-In"))
        {
            ConvertMaterials("Standard", null);
        }

        GUILayout.Space(15);
        EditorGUILayout.HelpBox("This tool preserves transparency settings and allows you to set a default smoothness value to control material shininess.", MessageType.Info);
    }

    private void ConvertMaterials(string shaderName, string newFolderName)
    {
        string[] materialPaths = Directory.GetFiles(targetFolder, "*.mat", SearchOption.AllDirectories);
        
        foreach (string materialPath in materialPaths)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null) continue;

            Shader shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Shader '{shaderName}' not found!");
                continue;
            }

            bool isTransparent = IsTransparentMaterial(material);
            bool hasAlphaClipping = HasAlphaClipping(material);
            
            Material newMaterial;
            if (convertExistingMaterials)
            {
                newMaterial = material;
                newMaterial.shader = shader;
            }
            else
            {
                newMaterial = new Material(shader);
                CopyMaterialProperties(material, newMaterial);

                string newFolderPath = Path.Combine(Path.GetDirectoryName(materialPath), newFolderName);
                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(materialPath), newFolderName);
                }

                string newMaterialPath = Path.Combine(newFolderPath, material.name + ".mat");
                AssetDatabase.DeleteAsset(newMaterialPath);
                AssetDatabase.CreateAsset(newMaterial, newMaterialPath);
            }

            ApplyTransparencySettings(newMaterial, isTransparent, hasAlphaClipping, shaderName);
            
            ApplySmoothness(newMaterial, shaderName);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Materials converted to " + shaderName + " with smoothness set to " + defaultSmoothness);
    }

    private void ApplySmoothness(Material material, string shaderName)
    {
        if (shaderName.Contains("HDRP"))
        {
            // For HDRP materials
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", defaultSmoothness);
            }
        }
        else if (shaderName.Contains("Universal Render Pipeline") || shaderName.Contains("URP"))
        {
            // For URP materials
            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", defaultSmoothness);
            }
        }
        else
        {
            // For standard materials
            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", defaultSmoothness);
            }
            if (material.HasProperty("_GlossMapScale"))
            {
                material.SetFloat("_GlossMapScale", defaultSmoothness);
            }
        }
    }

    private bool IsTransparentMaterial(Material material)
    {
        if (material.shader.name.Contains("HDRP"))
        {
            // For HDRP materials
            if (material.HasProperty("_SurfaceType"))
                return material.GetFloat("_SurfaceType") > 0.5f;
        }
        else if (material.shader.name.Contains("Universal Render Pipeline") || material.shader.name.Contains("URP"))
        {
            // For URP materials
            if (material.HasProperty("_Surface"))
                return material.GetFloat("_Surface") > 0.5f;
        }
        else
        {
            // For standard materials
            if (material.HasProperty("_Mode"))
                return material.GetFloat("_Mode") > 0.5f;
            
            // If the material has a transparent texture and its alpha channel is used
            if (material.HasProperty("_MainTex"))
            {
                var mainTex = material.GetTexture("_MainTex") as Texture2D;
                return material.HasProperty("_Color") && 
                       material.color.a < 1.0f || 
                       (mainTex != null && 
                       material.HasProperty("_AlphaClip") && 
                       material.GetFloat("_AlphaClip") > 0);
            }
        }
        
        return false;
    }

    private bool HasAlphaClipping(Material material)
    {
        if (material.shader.name.Contains("HDRP"))
        {
            // For HDRP materials
            if (material.HasProperty("_AlphaCutoffEnable"))
                return material.GetFloat("_AlphaCutoffEnable") > 0.5f;
        }
        else if (material.shader.name.Contains("Universal Render Pipeline") || material.shader.name.Contains("URP"))
        {
            // For URP materials
            if (material.HasProperty("_AlphaClip"))
                return material.GetFloat("_AlphaClip") > 0.5f;
        }
        else
        {
            // For standard materials
            if (material.HasProperty("_Mode"))
                return material.GetFloat("_Mode") == 1.0f; // Cutout mode
        }
        
        return false;
    }

    private void ApplyTransparencySettings(Material material, bool isTransparent, bool hasAlphaClipping, string targetShaderName)
    {
        if (targetShaderName.Contains("HDRP"))
        {
            // Settings for HDRP
            if (material.HasProperty("_SurfaceType"))
            {
                material.SetFloat("_SurfaceType", isTransparent ? 1 : 0); // 0 = Opaque, 1 = Transparent
            }
            
            if (material.HasProperty("_AlphaCutoffEnable"))
            {
                material.SetFloat("_AlphaCutoffEnable", hasAlphaClipping ? 1 : 0);
            }
            
            if (isTransparent && material.HasProperty("_BlendMode"))
            {
                material.SetFloat("_BlendMode", 0); // Alpha blend
                
                // Additional settings for transparency
                if (material.HasProperty("_EnableBlendModePreserveSpecularLighting"))
                    material.SetFloat("_EnableBlendModePreserveSpecularLighting", 1);
                if (material.HasProperty("_ZTestDepthEqualForOpaque") && isTransparent)
                    material.SetFloat("_ZTestDepthEqualForOpaque", 4); // Less Equal
            }
        }
        else if (targetShaderName.Contains("Universal Render Pipeline") || targetShaderName.Contains("URP"))
        {
            // Settings for URP
            if (material.HasProperty("_Surface"))
            {
                material.SetFloat("_Surface", isTransparent ? 1 : 0); // 0 = Opaque, 1 = Transparent
            }
            
            if (material.HasProperty("_AlphaClip"))
            {
                material.SetFloat("_AlphaClip", hasAlphaClipping ? 1 : 0);
            }
            
            if (isTransparent && material.HasProperty("_Blend"))
            {
                material.SetFloat("_Blend", 0); // SrcAlpha, OneMinusSrcAlpha
            }
            
            // Setting the rendering mode for URP
            if (material.HasProperty("_SrcBlend") && material.HasProperty("_DstBlend"))
            {
                if (isTransparent)
                {
                    material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_ZWrite", 0);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetOverrideTag("RenderType", "Transparent");
                }
                else
                {
                    material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                    material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetFloat("_ZWrite", 1);
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    material.SetOverrideTag("RenderType", "Opaque");
                }
            }
            
            if (hasAlphaClipping)
            {
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            }
        }
        else // Standard shader (Built-in RP)
        {
            // Settings for Standard shader
            if (material.HasProperty("_Mode"))
            {
                float mode = 0; // Opaque
                if (hasAlphaClipping)
                    mode = 1; // Cutout
                else if (isTransparent)
                    mode = 3; // Transparent
                
                material.SetFloat("_Mode", mode);
                
                // We apply the appropriate settings depending on the mode
                switch ((int)mode)
                {
                    case 0: // Opaque
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;
                        break;
                    case 1: // Cutout
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 2450;
                        break;
                    case 2: // Fade
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    case 3: // Transparent
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                }
            }
        }
    }

    private void CopyMaterialProperties(Material sourceMaterial, Material targetMaterial)
    {
        // Copying the general properties of the material
        targetMaterial.CopyPropertiesFromMaterial(sourceMaterial);
        
        // Copying textures
        CopyTexture(sourceMaterial, targetMaterial, "_MainTex", "_BaseMap");
        CopyTexture(sourceMaterial, targetMaterial, "_BaseMap", "_MainTex");
        CopyTexture(sourceMaterial, targetMaterial, "_BumpMap", "_BumpMap");
        CopyTexture(sourceMaterial, targetMaterial, "_MetallicGlossMap", "_MetallicGlossMap");
        CopyTexture(sourceMaterial, targetMaterial, "_OcclusionMap", "_OcclusionMap");
        CopyTexture(sourceMaterial, targetMaterial, "_EmissionMap", "_EmissionMap");
        
        // Copying colors
        CopyColor(sourceMaterial, targetMaterial, "_Color", "_BaseColor");
        CopyColor(sourceMaterial, targetMaterial, "_BaseColor", "_Color");
        CopyColor(sourceMaterial, targetMaterial, "_EmissionColor", "_EmissionColor");
        
        // Copy metallicity/smoothness settings
        CopyFloat(sourceMaterial, targetMaterial, "_Metallic", "_Metallic");
        CopyFloat(sourceMaterial, targetMaterial, "_Glossiness", "_Smoothness");
        CopyFloat(sourceMaterial, targetMaterial, "_Smoothness", "_Glossiness");
        CopyFloat(sourceMaterial, targetMaterial, "_GlossMapScale", "_Smoothness");
        
        // Copy the Cutoff setting for Alpha Clipping
        CopyFloat(sourceMaterial, targetMaterial, "_Cutoff", "_Cutoff");
    }

    private void CopyTexture(Material source, Material target, string sourceProperty, string targetProperty)
    {
        if (source.HasProperty(sourceProperty) && target.HasProperty(targetProperty))
        {
            Texture tex = source.GetTexture(sourceProperty);
            if (tex != null)
            {
                target.SetTexture(targetProperty, tex);
                
                if (source.HasProperty(sourceProperty + "_ST") && target.HasProperty(targetProperty + "_ST"))
                {
                    target.SetVector(targetProperty + "_ST", source.GetVector(sourceProperty + "_ST"));
                }
            }
        }
    }

    private void CopyColor(Material source, Material target, string sourceProperty, string targetProperty)
    {
        if (source.HasProperty(sourceProperty) && target.HasProperty(targetProperty))
        {
            target.SetColor(targetProperty, source.GetColor(sourceProperty));
        }
    }

    private void CopyFloat(Material source, Material target, string sourceProperty, string targetProperty)
    {
        if (source.HasProperty(sourceProperty) && target.HasProperty(targetProperty))
        {
            target.SetFloat(targetProperty, source.GetFloat(sourceProperty));
        }
    }
}