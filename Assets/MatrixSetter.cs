using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class MatrixSetter : MonoBehaviour
{
    [SerializeField] private Texture2D matrixTexture;
    public void SetLevelMatrixByTexture()
    {
        var matrixList = new List<int>();
        var width = matrixTexture.width;
        var height = matrixTexture.height;
        for (int i = 0; i < height+0.01f; i+=height/5)
        {
            for (int j = 0; j < width+0.01f; j+=width/4)
            {
                var p=matrixTexture.GetPixel(j, i);
                if (p.r > 0.5f)
                {
                    if (p.g > 0.47f) matrixList.Add(1); // Yellow g is given 242
                    else matrixList.Add(0);
                }
                else matrixList.Add(2);
            }
        }
        var matrix = new Matrix() { matrix = matrixList };
        var m=JsonUtility.ToJson(matrix);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/MatrixTexture.json",m);
        AssetDatabase.SaveAssets();
    }
    private class Matrix
    {
        public List<int> matrix = new List<int>();
    } 
}
