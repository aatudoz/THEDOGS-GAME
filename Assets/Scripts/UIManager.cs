using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    //sydamet inspectorissa
    [Header("Hearts")]
    public List<Image> heartImages;

    //score inspectorissa
    [Header("Score")]
    public int score = 0;
    public TMP_Text scoreText;
    public TMP_Text deathScoreText;
    public TMP_Text victoryScoreText;

    public GameObject scoreUI;

    public GameObject pauseMenu;
    public GameObject deathScreen;
    private bool isPaused = false;

    public TMP_Text floatingScoreText;

    //resume nappula
    public void OnGameResumePress()
    {
        ResumeGame();
    }

    //quit nappula
    public void OnGameExitPress()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //try again nappula
    public void OnGameTryAgainPress()
    {
        TryAgain();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {   
        scoreUI.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; //stoppaa pelin
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; //jatkaa pelin
        isPaused = false;
        scoreUI.SetActive(true);
    }
    public void ShowDeathScreen()
    {   
        scoreUI.SetActive(false);
        deathScreen.SetActive(true);
        Time.timeScale = 0f; //stoppaa pelin
        scoreUI.SetActive(false);
        if (deathScoreText != null)
        deathScoreText.text = score.ToString();
    }


    public void TryAgain()
    {   
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; //jatkaa pelin
        isPaused = false;
        deathScreen.SetActive(false);

        SceneManager.LoadScene("SampleScene");
    }

    //paivitasydamet
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].gameObject.SetActive(i < currentHealth);
        }
    }

    //lisaa score
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    //paivita scoreUI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }


    public void ShowFloatingScore(Vector3 worldPosition, int amount)
    {
        if (floatingScoreText == null) return;

        StartCoroutine(FloatingScoreRoutine(worldPosition, amount));
    }

    private IEnumerator FloatingScoreRoutine(Vector3 worldPosition, int amount)
    {
        floatingScoreText.gameObject.SetActive(true);
        floatingScoreText.text = "+" + amount;

        // muunna worldPosition -> screenPosition
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        floatingScoreText.transform.position = screenPos;

        float duration = 1f;
        float moveSpeed = 50f; // pikseliä sekunnissa
        float elapsed = 0f;

        Color originalColor = floatingScoreText.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            floatingScoreText.transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            floatingScoreText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (elapsed / duration));
            yield return null;
        }

        floatingScoreText.color = originalColor;
        floatingScoreText.gameObject.SetActive(false);
    }
    
    

}
