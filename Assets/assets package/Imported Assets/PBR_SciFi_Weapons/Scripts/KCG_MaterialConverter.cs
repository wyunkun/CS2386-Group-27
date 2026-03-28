using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class KCG_MaterialConverter : EditorWindow
{
    private string folderPath = "Assets/???"; // Specify the starting folder
    private Dictionary<string, List<Material>> materials = new Dictionary<string, List<Material>>();
    private Dictionary<string, bool> selectedMaterials = new Dictionary<string, bool>();
    
    // Variables for the shader dropdown list
    private List<string> availableShaders = new List<string>();
    private int selectedShaderIndex = 0;
    private Vector2 scrollPosition;

    [MenuItem("Tools/KCG_MaterialConverter")]
    public static void ShowWindow()
    {
        GetWindow<KCG_MaterialConverter>("KCG_MaterialConverter");
    }

    private void OnEnable()
    {
        // Load a list of all available shaders when the window opens
        LoadAvailableShaders();
    }

    private void LoadAvailableShaders()
    {
        availableShaders.Clear();
        
        // We get all shaders in the project
        var shaders = ShaderUtil.GetAllShaderInfo()
            .Where(s => !s.name.StartsWith("Hidden/"))
            .Select(s => s.name)
            .OrderBy(s => s)
            .ToList();
        
        availableShaders.AddRange(shaders);
        
        // If the list is not empty, set the first shader by default
        if (availableShaders.Count > 0)
            selectedShaderIndex = 0;
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Converter", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Find Materials"))
        {
            FindMaterials();
        }

        // Dropdown list of shaders
        GUILayout.Space(10);
        GUILayout.Label("Target Shader:", EditorStyles.boldLabel);
        
        if (availableShaders.Count > 0)
        {
            selectedShaderIndex = EditorGUILayout.Popup("Select Shader", selectedShaderIndex, availableShaders.ToArray());
        }
        else
        {
            EditorGUILayout.HelpBox("No shaders found in the project.", MessageType.Warning);
        }

        GUILayout.Space(10);
        GUILayout.Label("Found Materials:", EditorStyles.boldLabel);

        // Use ScrollView to display a list of materials if there are a lot of them
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        
        foreach (var mat in materials)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Use `TryGetValue` to check the selection and display a checkmark
            bool isSelected = selectedMaterials.TryGetValue(mat.Key, out bool value) && value;

            // Create a radio button and update the selection state
            isSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));
            selectedMaterials[mat.Key] = isSelected;

            // Show shader name and material count
            EditorGUILayout.LabelField($"{mat.Key} ({mat.Value.Count} materials)");
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        GUI.enabled = availableShaders.Count > 0 && materials.Count > 0;
        if (GUILayout.Button("Convert Selected Materials"))
        {
            ConvertMaterials();
        }
        GUI.enabled = true;

        // Button to select all materials
        if (GUILayout.Button("Select All Materials"))
        {
            foreach (var key in materials.Keys)
            {
                selectedMaterials[key] = true;
            }
        }

        // Button to deselect all materials
        if (GUILayout.Button("Deselect All Materials"))
        {
            foreach (var key in materials.Keys)
            {
                selectedMaterials[key] = false;
            }
        }
    }

    private void FindMaterials()
    {
        materials.Clear();
        selectedMaterials.Clear();
        
        var guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                string shaderName = mat.shader.name;
                if (!materials.ContainsKey(shaderName))
                {
                    materials[shaderName] = new List<Material>();
                }
                materials[shaderName].Add(mat);
                selectedMaterials[shaderName] = false; // Initially, all shaders are not selected
            }
        }
    }

    private void ConvertMaterials()
    {
        if (availableShaders.Count == 0 || selectedShaderIndex >= availableShaders.Count)
        {
            Debug.LogWarning("No target shader selected.");
            return;
        }

        string targetShaderName = availableShaders[selectedShaderIndex];
        Shader newShader = Shader.Find(targetShaderName);
        
        if (newShader == null)
        {
            Debug.LogWarning($"Shader '{targetShaderName}' not found.");
            return;
        }

        int convertedCount = 0;
        foreach (var matPair in materials)
        {
            if (selectedMaterials.TryGetValue(matPair.Key, out bool isSelected) && isSelected)
            {
                foreach (var mat in matPair.Value)
                {
                    mat.shader = newShader; // Installing a new shader
                    EditorUtility.SetDirty(mat); // We designate the material as modified
                    convertedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Materials converted: {convertedCount} materials changed to '{targetShaderName}' shader.");
    }
}