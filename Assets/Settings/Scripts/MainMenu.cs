using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        PhotonNetwork.Disconnect();

        if(SceneManager.GetActiveScene().name == "Scene_0")
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Selection Screen");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}