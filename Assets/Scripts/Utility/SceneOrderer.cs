using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneOrderer : EditorWindow
{
    private List<SceneAsset> m_SceneAssets = new List<SceneAsset>();

    [MenuItem("Alictus/Scene Order Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SceneOrderer));
    }
    void OnGUI()
    {
        GUILayout.Label("Scenes to include in build:", EditorStyles.boldLabel);
        GUILayout.Label("First scene must be splash scene!", EditorStyles.boldLabel);
        for (int i = 0; i < m_SceneAssets.Count; ++i)
        {
            m_SceneAssets[i] = (SceneAsset)EditorGUILayout.ObjectField(m_SceneAssets[i], typeof(SceneAsset), false);
        }
        if (GUILayout.Button("Add Scene"))
        {
            m_SceneAssets.Add(null);
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Apply"))
        {
            SetEditorBuildSettingsScenes();
        }
    }
    
    private void SetEditorBuildSettingsScenes()
    {
        var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach (var sceneAsset in m_SceneAssets)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }
}
public class Level3Editor : EditorWindow
{
    [MenuItem("Alictus/Level 3 Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Level3Editor));
    }
    void OnGUI()
    {
        GUILayout.Label("Didn't have enough time to learn window editor", EditorStyles.boldLabel);
        GUILayout.Label("Level 3 matrix can be set with texture on Matrix Setter in level 3 scene", EditorStyles.boldLabel);
    }
}