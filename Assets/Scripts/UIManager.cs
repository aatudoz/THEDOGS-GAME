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

    [Header("Wave Messages")]
    public TMP_Text waveMessageText;

    [Header("Start Message")]
    public TMP_Text startMessageText;

    public GameObject scoreUI;

    //ammo ui
    public GameObject AmmoTextCount;

    //stargame ui thing
    public GameObject StartGameUI;

    public GameObject pauseMenu;
    public GameObject deathScreen;
    private bool isPaused = false;

    public TMP_Text floatingScoreText;

    [Header("Wave UI")]
    public TMP_Text waveText;

    [Header("Ammo UI")]
    public TMP_Text ammoText;
    public Image reloadImage; // The circular image for reload animation

    private Gun playerGun;
    private float reloadStartTime;

    //powerup text
    public TextMeshProUGUI powerupText;

    //wave number
    public void ShowWave(int waveNumber)
    {
        waveText.gameObject.SetActive(true);
        waveText.text = waveNumber.ToString();
    }

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
        playerGun = FindFirstObjectByType<Gun>();

        //Hide reload circle
        if (reloadImage != null)
        {
            reloadImage.gameObject.SetActive(false);
            reloadImage.fillAmount = 0f;
        }
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
        if (playerGun != null)
        {
            UpdateAmmoUI(playerGun.currentMagAmmo, playerGun.MaxMagAmmo, playerGun.isReloading);
        }
    }

    //From Gun.cs
    private void UpdateAmmoUI(int currentAmmo, int maxAmmo, bool isReloading)
    {
        //Update Ammo Text
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }

        //Reload image visibility/animation
        if (reloadImage != null)
        {
            if (isReloading)
            {
                //Reload logic, thank internet
                if (!reloadImage.gameObject.activeSelf)
                {
                    reloadImage.gameObject.SetActive(true);
                    reloadStartTime = Time.time;
                }

                float timeSinceReload = Time.time - reloadStartTime;
                float progress = timeSinceReload / playerGun.ReloadTime;

                reloadImage.fillAmount = progress;
            }
            else
            {
                //Reload finished or not reloading
                if (reloadImage.gameObject.activeSelf)
                {
                    reloadImage.gameObject.SetActive(false);
                    reloadImage.fillAmount = 0f;
                }
            }
        }
    }

    public void PauseGame()
    {
        scoreUI.SetActive(false);
        pauseMenu.SetActive(true);
        AmmoTextCount.SetActive(false);
        Time.timeScale = 0f; //stoppaa pelin
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        AmmoTextCount.SetActive(true);
        Time.timeScale = 1f; //jatkaa pelin
        isPaused = false;
        scoreUI.SetActive(true);
    }
    public void ShowDeathScreen()
    {
        scoreUI.SetActive(false);
        deathScreen.SetActive(true);
        AmmoTextCount.SetActive(false);
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
        AmmoTextCount.SetActive(true);

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

    //Wave texts
    public void ShowWaveMessage(string message, float duration = 2f)
    {
        StartCoroutine(ShowWaveMessageRoutine(message, duration));
    }

    private IEnumerator ShowWaveMessageRoutine(string message, float duration)
    {
        if (waveMessageText == null) yield break;

        waveMessageText.text = message;
        waveMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        waveMessageText.gameObject.SetActive(false);
    }

    // Start message methods
    public void ShowStartMessage(string message)
    {
        if (startMessageText != null)
        {
            startMessageText.text = message;
            startMessageText.gameObject.SetActive(true);
        }
    }

    public void HideStartMessage()
    {
        if (startMessageText != null)
        {
            startMessageText.gameObject.SetActive(false);
        }
    }

    //po
    public void ShowPowerupText(string msg, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(PowerupRoutine(msg, duration));
    }

    private IEnumerator PowerupRoutine(string msg, float duration)
    {
        powerupText.text = msg;
        powerupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        powerupText.gameObject.SetActive(false);
    }



}