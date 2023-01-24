using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
public class RunGame : Editor
{
    const string CHEATS_ENABLED = "CheatsEnabled";

    [MenuItem("Game/Run Game")]
    static void StartGame()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/InitialMenu.unity");
            EditorApplication.isPlaying = true;
        }
    }
}
#endif