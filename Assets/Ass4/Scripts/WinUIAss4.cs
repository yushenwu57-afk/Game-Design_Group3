using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUIAss4 : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string level3SceneName = "Level3";
    [SerializeField] private string level4SceneName = "Level4";
    [SerializeField] private bool resetTimeScale = true;

    // Choose these in Button OnClick:
    // WinUI.LoadLevel1 / LoadLevel2 / LoadLevel3 / LoadLevel4
    public void LoadLevel1()
    {
        LoadByName(level1SceneName);
    }

    public void LoadLevel2()
    {
        LoadByName(level2SceneName);
    }

    public void LoadLevel3()
    {
        LoadByName(level3SceneName);
    }

    public void LoadLevel4()
    {
        LoadByName(level4SceneName);
    }

    // Choose this in Button OnClick to restart current level.
    public void RestartLevel()
    {
        // Reset player/health if they exist, then reload current scene.
        var player = FindAnyObjectByType<PlayerAss4>();
        if (player != null)
        {
            player.ResetForRestart();
        }
        else
        {
            var health = FindAnyObjectByType<Health>();
            if (health != null)
            {
                health.ResetToFull();
            }
        }

        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void LoadByName(string sceneName)
    {
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"{name}: Scene name is empty.");
        }
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
