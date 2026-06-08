using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    private float duration = 0.25f;
    [SerializeField] private PlayerInput playerInput;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup pauseMenuCG;
    [SerializeField] private CanvasGroup settingsMenuCG;

    [Header("Animators")]
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Animator settingsMenuAnimator;

    private void Awake()
    {
        GameIsPaused = false;

        SetCGState(pauseMenuCG, false);
        pauseMenuCG.alpha = 0f;

        SetCGState(settingsMenuCG, false);
        settingsMenuCG.alpha = 0f;
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
        GameIsPaused = true;
        Time.timeScale = 0f;
        playerInput.SwitchCurrentActionMap("UI");

        SetCGState(pauseMenuCG, true);
        StartCoroutine(FadeCG(pauseMenuCG, 0f, 1f));

        if (pauseMenuAnimator != null)
        {
            pauseMenuAnimator.SetTrigger("Open");
        }
    }
    public void Resume() => StartCoroutine(CloseMenuRoutine());

    public void Settings() => SwitchMenu(settingsMenuCG, pauseMenuCG, settingsMenuAnimator, "Open");

    public void Back() => SwitchMenu(pauseMenuCG, settingsMenuCG, settingsMenuAnimator, "Close");

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void SwitchMenu(CanvasGroup toShow, CanvasGroup toHide, Animator anim, string trigger)
    {
        StopAllCoroutines();

        SetCGState(toHide, false);
        StartCoroutine(FadeCG(toHide, toHide.alpha, 0f));

        SetCGState(toShow, true);
        StartCoroutine(FadeCG(toShow, toShow.alpha, 1f));

        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
    }

    private IEnumerator CloseMenuRoutine()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        playerInput.SwitchCurrentActionMap("Player");

        SetCGState(pauseMenuCG, false);
        SetCGState(settingsMenuCG, false);

        if (pauseMenuAnimator != null && pauseMenuCG.alpha > 0.1f)
        {
            pauseMenuAnimator.SetTrigger("Close");
        }
        if (settingsMenuAnimator != null && settingsMenuCG.alpha > 0.1f)
        {
            settingsMenuAnimator.SetTrigger("Close");
        }

        StartCoroutine(FadeCG(pauseMenuCG, pauseMenuCG.alpha, 0f));
        StartCoroutine(FadeCG(settingsMenuCG, settingsMenuCG.alpha, 0f));

        yield return new WaitForSecondsRealtime(duration);
    }

    private IEnumerator FadeCG(CanvasGroup cg ,float from, float to)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            cg.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = to;
    }
    private void SetCGState(CanvasGroup cg, bool active)
    {
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }
}