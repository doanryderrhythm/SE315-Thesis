using Photon.Pun;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public static bool isGamePaused = false;
    private float duration = 0.25f;
    private PlayerInput playerInput;

    [SerializeField] private Button pauseBtn;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup pauseMenuCG;
    [SerializeField] private CanvasGroup settingsMenuCG;

    [Header("Animators")]
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Animator settingsMenuAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        isGamePaused = false;

        SetCGState(pauseMenuCG, false);
        pauseMenuCG.alpha = 0f;

        SetCGState(settingsMenuCG, false);
        settingsMenuCG.alpha = 0f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void Pause()
    {
        isGamePaused = true;
        if (pauseBtn)
        {
            pauseBtn.interactable = false;
        }

        if (SceneManager.GetActiveScene().name == "Practice Room")
        {
            Time.timeScale = 0f;
        }

        if(playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
            Debug.Log("Switching to UI Input");
        }

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

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        if (GameManager.Instance)
            Destroy(GameManager.Instance.gameObject);

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
        isGamePaused = false;
        Time.timeScale = 1f;

        if (pauseBtn)
        {
            pauseBtn.interactable = true;
        }

        if(playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("Player");
            Debug.Log("Switching to Player Input");
        }

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

    public void RegisterPlayerInput(PlayerInput input)
    {
        playerInput = input;
    }
}