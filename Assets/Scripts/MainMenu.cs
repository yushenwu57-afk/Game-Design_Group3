using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names (must be added to Build Settings)")]
    [SerializeField] private string introductionScene = "Introduction";
    [SerializeField] private string tutorialScene = "Tutorial";
    [SerializeField] private string level1Scene = "Level1";
    [SerializeField] private string level2Scene = "Level2";
    [SerializeField] private string level3Scene = "Level3";
    [SerializeField] private string level4Scene = "Level4";

    public void LoadIntroduction()
    {
        LoadSceneByName(introductionScene);
    }

    public void LoadTutorial()
    {
        LoadSceneByName(tutorialScene);
    }

    public void LoadLevel1()
    {
        LoadSceneByName(level1Scene);
    }

    public void LoadLevel2()
    {
        LoadSceneByName(level2Scene);
    }

    public void LoadLevel3()
    {
        LoadSceneByName(level3Scene);
    }

    public void LoadLevel4()
    {
        LoadSceneByName(level4Scene);
    }

    private void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("MainMenu: Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
