using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
 
[InitializeOnLoad]
public class Autosave
{

    const string MENU_ITEM = "Tools/AutoSave Scenes";
    const string PREF_KEY = "AutoSaveScenes";
        
    static bool IsEnabled
    {
        get => EditorPrefs.GetBool(PREF_KEY, true);
        set => EditorPrefs.SetBool(PREF_KEY, value);
    }
    
    static Autosave()
    {
        EditorApplication.playmodeStateChanged += () =>
        {
            if (IsEnabled && EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("Auto-saving all open scenes...");
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        };
    }

    [MenuItem(MENU_ITEM)]
    static void ToggleAutoSave()
    {
        IsEnabled = !IsEnabled;
    }

    [MenuItem(MENU_ITEM, true)]
    static bool RefreshAutoSaveMenuItem()
    {
        Menu.SetChecked(MENU_ITEM, IsEnabled);
        return true;
    }
    
}
