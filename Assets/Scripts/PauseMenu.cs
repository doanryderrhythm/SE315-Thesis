using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject pauseMenuButtons;
    [SerializeField] private Animator menuAnimator;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        menuAnimator.SetTrigger("Open");
    }
    public void Resume()
    {
        StartCoroutine(CloseMenuRoutine());
    }

    private IEnumerator CloseMenuRoutine()
    {
        menuAnimator.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(0.25f);

        settingsMenu.SetActive(false);
        pauseMenuButtons.SetActive(true);

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Settings()
    {
        Debug.Log("Setting");
    }

    public void ReturnToTitle()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(0);
    }
}
