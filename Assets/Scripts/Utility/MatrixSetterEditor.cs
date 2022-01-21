#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MatrixSetter))]
public class MatrixSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var generator = (MatrixSetter)target;
        if(GUILayout.Button("Set Level By Texture"))
        {
            generator.SetLevelMatrixByTexture();
        }
    }
}
#endif