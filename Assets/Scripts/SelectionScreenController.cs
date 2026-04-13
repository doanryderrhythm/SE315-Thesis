using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene(0);
    }
    public void MapSelect()
    {
        SceneManager.LoadScene(2);
    }
}
