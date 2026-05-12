using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum SceneNames
    {
        WelcomeMenu,
        RoomScene
        ,
        
        
    }

    private static SceneNames targetScene;
    public static void Load(SceneNames targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(targetScene.ToString());
        
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}

