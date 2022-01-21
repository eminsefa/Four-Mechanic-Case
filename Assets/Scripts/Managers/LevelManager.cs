using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager 
{
    /// <summary>
    /// This script sets and saves the level index.
    /// Level loop is set due to build index order.
    /// </summary>
    private static AsyncOperation _scene;
    public static int GetLevelIndex() =>SceneManager.GetActiveScene().buildIndex;

    public static int GetPrefIndex() => PlayerPrefs.GetInt("Level", 1);

    private static void SetPrefIndex(int pref)
    {
        PlayerPrefs.SetInt("Level",pref);
    }
    public static void LevelCompleted()
    {
        var levelIndex = GetLevelIndex();
        var prefIndex = GetPrefIndex();
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        
        if ( levelIndex+ 1 == sceneCount)
        {
            _scene=SceneManager.LoadSceneAsync(1);
        }
        else
        {
            _scene=SceneManager.LoadSceneAsync(levelIndex+ 1);
        }
        _scene.completed += OnSceneLoaded;
        _scene.allowSceneActivation = false;
        SetPrefIndex(prefIndex+1);
        var nextLevelIndex = levelIndex + 1;
        if (nextLevelIndex % 5 == 0) nextLevelIndex = 1;
        PlayerPrefs.SetInt("Last Scene",nextLevelIndex);
    }

    public static void LevelFailed()
    {
        var levelIndex = GetLevelIndex();
        _scene = SceneManager.LoadSceneAsync(levelIndex);
        _scene.allowSceneActivation = false;
        _scene.completed += OnSceneLoaded;
        PlayerPrefs.SetInt("Last Scene",levelIndex);
    }

    private static void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        _scene = null;
    }

    public static void LoadLastScene()
    {
        var lastSceneIndex = PlayerPrefs.GetInt("Last Scene", 1);
        SceneManager.LoadSceneAsync(lastSceneIndex % 5);
    }

    public static void RestartLevel()
    {
        SceneManager.LoadSceneAsync(GetLevelIndex());
    } 

    public static void ActivateScene()
    {
        _scene.allowSceneActivation = true;
    }
}
