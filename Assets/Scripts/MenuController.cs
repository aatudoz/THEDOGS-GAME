using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); //Lataa pelin
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");     //näkyy editorissa
        Application.Quit();     //sulkeepi pelin
    }
}
