using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class KCG_AutoTextureAssigner : EditorWindow
{
    private string folderPath = "Assets/???";

    public string MainTexSuffix = "";
    public string NormalMapSuffix = "_NM";
    public string EmissiveSuffix = "_EM";
    public string AmbientOcclusionSuffix = "_AO";
    public string SpecularSuffix = "_SG";
    public string MetallicSuffix = "_MT";
    
    private bool searchAllFolders = false;

    [MenuItem("Tools/KCG_AutoTextureAssigner")]
    public static void ShowWindow()
    {
        GetWindow<KCG_AutoTextureAssigner>("KCG_AutoTextureAssigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Textures to Materials from Folder", EditorStyles.boldLabel);
        
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        
        searchAllFolders = EditorGUILayout.Toggle("Search All Project Folders", searchAllFolders);
        
        EditorGUILayout.Space();
        GUILayout.Label("Texture Suffix Settings", EditorStyles.boldLabel);
        
        MainTexSuffix = EditorGUILayout.TextField("Main Texture Suffix", MainTexSuffix);
        NormalMapSuffix = EditorGUILayout.TextField("Normal Map Suffix", NormalMapSuffix);
        EmissiveSuffix = EditorGUILayout.TextField("Emissive Suffix", EmissiveSuffix);
        AmbientOcclusionSuffix = EditorGUILayout.TextField("Ambient Occlusion Suffix", AmbientOcclusionSuffix);
        SpecularSuffix = EditorGUILayout.TextField("Specular Suffix", SpecularSuffix);
        MetallicSuffix = EditorGUILayout.TextField("Metallic Suffix", MetallicSuffix);

        if (GUILayout.Button("Assign Textures"))
        {
            AssignTexturesToMaterials();
        }

        GUILayout.Space(15);
        EditorGUILayout.HelpBox("If you have duplicate materials, textures will only be assigned to one of them.", MessageType.Warning);
        
        if (searchAllFolders)
        {
            EditorGUILayout.HelpBox("'Search All Project Folders' is enabled. This will search for matching textures across the entire project, which might take longer for large projects.", MessageType.Info);
        }
        
        EditorGUILayout.HelpBox("File naming example: \n Blue_car.mat ........... (material) \n Blue_car.tga ............ (main texture) \n Blue_car_NM.tga ..... (normal map) \n Blue_car_AO.tga ...... (ambient occlusion)", MessageType.Info);
    }

    private void AssignTexturesToMaterials()
    {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                materials[material.name] = material;
            }
        }

        string[] searchFolders;
        if (searchAllFolders)
        {
            searchFolders = new[] { "Assets" };
        }
        else
        {
            searchFolders = new[] { folderPath };
        }

        Dictionary<string, Texture> allTextures = new Dictionary<string, Texture>();

        string[] textureGuids = AssetDatabase.FindAssets("t:Texture", searchFolders);
        foreach (string guid in textureGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            if (texture != null)
            {
                if (!allTextures.ContainsKey(texture.name))
                {
                    allTextures[texture.name] = texture;
                }
            }
        }

        int assignedTexturesCount = 0;
        
        foreach (var materialPair in materials)
        {
            Material material = materialPair.Value;
            string materialName = materialPair.Key;

            // Base Map
            int assigned = AssignTexture(material, materialName, MainTexSuffix, new List<string> { "_MainTex", "_BaseColorMap", "_BaseMap" }, allTextures);
            assignedTexturesCount += assigned;
            
            // Normal Map
            assigned = AssignTexture(material, materialName, NormalMapSuffix, new List<string> { "_NormalMap", "_BumpMap" }, allTextures);
            assignedTexturesCount += assigned;
            
            // Emissive
            assigned = AssignTexture(material, materialName, EmissiveSuffix, new List<string> { "_EmissiveColorMap", "_EmissionMap" }, allTextures);
            assignedTexturesCount += assigned;
            
            // Ambient Occlusion
            assigned = AssignTexture(material, materialName, AmbientOcclusionSuffix, new List<string> { "_OcclusionMap", "_MaskMap" }, allTextures);
            assignedTexturesCount += assigned;
            
            // Specular
            assigned = AssignTexture(material, materialName, SpecularSuffix, new List<string> { "_SpecularColorMap", "_SpecGlossMap", "_Glossiness", "_Smoothness" }, allTextures);
            assignedTexturesCount += assigned;
            
            // Metallic
            assigned = AssignTexture(material, materialName, MetallicSuffix, new List<string> { "_MetallicGlossMap", "_Metallic" }, allTextures);
            assignedTexturesCount += assigned;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Texture assignment completed. Assigned {assignedTexturesCount} textures to {materials.Count} materials.");
    }

    private int AssignTexture(Material material, string materialName, string suffix, List<string> slotNames, Dictionary<string, Texture> textures)
    {
        string textureName = materialName + suffix;
        int assignedCount = 0;
        
        if (textures.TryGetValue(textureName, out Texture texture))
        {
            foreach (string slotName in slotNames)
            {
                if (material.HasProperty(slotName) && material.GetTexture(slotName) == null)
                {
                    material.SetTexture(slotName, texture);
                    Debug.Log($"Assigned texture '{textureName}' to '{slotName}' in material '{materialName}'.");
                    EditorUtility.SetDirty(material);
                    assignedCount++;
                }
            }
        }
        
        return assignedCount;
    }
}