using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); //Lataa pelin
        Time.timeScale = 1f; //varmistaa etta peli jatkuu normaalisti
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");     //näkyy editorissa
        Application.Quit();     //sulkeepi pelin
    }
}
