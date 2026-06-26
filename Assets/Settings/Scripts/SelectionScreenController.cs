using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionScreen : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInputField;

    private void Start()
    {
        usernameInputField.text = PlayerPrefs.GetString("Username", "Player");
    }

    public void Back()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void MapSelect()
    {
        SceneManager.LoadScene("Practice Room");
    }

    public void CreateMap()
    {
        SceneManager.LoadScene("MapSelectionScene");
    }

    public void SaveUsername()
    {
        PlayerPrefs.SetString("Username", usernameInputField.text);

        PhotonNetwork.NickName = usernameInputField.text;
    }
}
