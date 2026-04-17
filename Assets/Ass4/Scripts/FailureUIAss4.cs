using UnityEngine;
using UnityEngine.SceneManagement;

public class FailureUIAss4 : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool resetTimeScale = true;

    public void RestartLevel()
    {
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

    public void GoToMainMenu()
    {
        if (resetTimeScale)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

