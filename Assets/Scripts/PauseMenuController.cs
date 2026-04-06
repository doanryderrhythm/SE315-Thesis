using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    private CanvasGroup canvasGroup;

    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Animator settingsMenuAnimator;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
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
        StartCoroutine(FadeFromTo(0f, 1f));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        pauseMenuAnimator.SetTrigger("Open");

        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Resume()
    {
        StartCoroutine(FadeFromTo(1f, 0f));
        StartCoroutine(CloseMenuRoutine());

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator CloseMenuRoutine()
    {
        pauseMenuAnimator.SetTrigger("Close");
        settingsMenuAnimator.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(0.25f);

        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        pauseMenuAnimator.SetTrigger("Close");

        settingsMenuUI.SetActive(true);
        settingsMenuAnimator.SetTrigger("Open");
    }

    public void Back()
    {
        settingsMenuUI.SetActive(false);
        settingsMenuAnimator.SetTrigger("Close");

        pauseMenuUI.SetActive(true);
        pauseMenuAnimator.SetTrigger("Open");
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator FadeFromTo(float from, float to)
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
