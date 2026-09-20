using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Level1_Village");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}