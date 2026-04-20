using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResultScreenController : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject finishIntroUI;
    [SerializeField] private GameObject resultPanelUI;

    [SerializeField] private Animator finishIntroAnimator;
    [SerializeField] private Animator resultPanelAnimator;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetUIState(false);

        finishIntroUI.SetActive(false);
        resultPanelUI.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowResultScreen();
        }
    }

    public void ShowResultScreen()
    {
        StartCoroutine(ResultSequenceRoutine());
    }

    public void Leave()
    {
        SceneManager.LoadScene(0);
    }
    private IEnumerator ResultSequenceRoutine()
    {
        SetUIState(true);

        finishIntroUI.SetActive(true);
        finishIntroAnimator.SetTrigger("Play");

        yield return new WaitForSecondsRealtime(1f);
        finishIntroUI.SetActive(false);

        resultPanelUI.SetActive(true);
        resultPanelAnimator.SetTrigger("Play");
    }
    private void SetUIState(bool active)
    {
        GameIsPaused = active;
        Time.timeScale = active ? 0f : 1f;
        playerInput.SwitchCurrentActionMap(active ? "UI" : "Player");

        canvasGroup.alpha = active ? 1f : 0f;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}
