using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void MapSelect()
    {
        SceneManager.LoadScene("Polishing Test");
    }
}
