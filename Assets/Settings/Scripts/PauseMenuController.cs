using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;

    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Animator settingsMenuAnimator;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetUIState(false);
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
        SetUIState(true);
        pauseMenuAnimator.SetTrigger("Open");

        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    public void Resume() => StartCoroutine(CloseMenuRoutine());

    public void Settings() => SwitchMenu(settingsMenuUI, pauseMenuUI, settingsMenuAnimator, "Open");

    public void Back() => SwitchMenu(pauseMenuUI, settingsMenuUI, settingsMenuAnimator, "Close");

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }

    private void SwitchMenu(GameObject toShow, GameObject toHide, Animator anim, string trigger)
    {
        toShow.SetActive(true);
        toHide.SetActive(false);
        anim.SetTrigger(trigger);
    }
    private void SetUIState(bool active)
    {
        GameIsPaused = active;
        Time.timeScale = active ? 0f : 1f;
        playerInput.SwitchCurrentActionMap(active ? "UI" : "Player");

        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;

        StopAllCoroutines();
        StartCoroutine(FadeFromTo(canvasGroup.alpha, active ? 1f : 0f));
    }
    private IEnumerator CloseMenuRoutine()
    {
        pauseMenuAnimator.SetTrigger("Close");
        settingsMenuAnimator.SetTrigger("Close");
        SetUIState(false);

        yield return new WaitForSecondsRealtime(0.25f);
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