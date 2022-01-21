using UnityEngine;

public class SplashController : MonoBehaviour
{
    /// <summary>
    /// This script loads the last scene before loading any scene.
    /// </summary>
    private void Awake()
    {
        Application.targetFrameRate = 60;
        LevelManager.LoadLastScene();
    }
}
