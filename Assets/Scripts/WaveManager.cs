using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject enemy3;
    [SerializeField] GameObject boss;

    [SerializeField] float timeBetweenWaves = 5f;
    [SerializeField] float spawnRate = 2f;
    [SerializeField] int groupAmount = 3;

    [SerializeField] float spawnDist = 15f;
    [SerializeField] Transform player;

    [SerializeField] private UIManager uiManager;

    [SerializeField] Transform[] spawnPoints;


    int wave = 0;
    int spawned = 0;
    int toSpawn = 0;
    int alive = 0;

    bool waveGoing = false;
    bool bossDone = false;
    bool gameStarted = false;

    int[] waveCounts = new int[] { 15, 30, 25 };

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Show the start message
        if (uiManager != null)
            uiManager.ShowStartMessage("Press G to start!");
    }

    void Update()
    {
        // Check if player presses G to start the game
        if (!gameStarted && Input.GetKeyDown(KeyCode.G))
        {
            gameStarted = true;
            if (uiManager != null)
                uiManager.HideStartMessage();
            StartCoroutine(StartWave());
        }

        if (waveGoing && alive <= 0 && spawned >= toSpawn)
        {
            //if 3rd wave, spawn boss
            if (wave == 3 && !bossDone)
            {
                SpawnBoss();
                return;
            }

            EndWave();
        }
    }

    IEnumerator StartWave()
    {
        wave++;

        //show wave number ui
        if (uiManager != null)
            uiManager.ShowWave(wave);

        if (wave > 3)
        {
            Debug.Log("Game finished!");
            yield break;
        }

        if (wave > 1)
            yield return new WaitForSeconds(timeBetweenWaves);

        spawned = 0;
        toSpawn = waveCounts[wave - 1];
        alive = 0;
        waveGoing = true;
        bossDone = false;

        if (uiManager != null)
            uiManager.ShowWaveMessage("WAVE " + wave + " START!", 2f);

        Debug.Log("Wave " + wave + " started! Spawning " + toSpawn);

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (spawned < toSpawn)
        {
            for (int i = 0; i < groupAmount && spawned < toSpawn; i++)
            {
                SpawnOne();
                spawned++;
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    // void SpawnOne()
    // {
    //     if (player == null) return;

    //     Vector2 dir = Random.insideUnitCircle.normalized;
    //     Vector3 pos = player.position + (Vector3)(dir * spawnDist);

    //     GameObject pick = PickEnemy();
    //     if (pick == null) return;

    //     GameObject e = Instantiate(pick, pos, Quaternion.identity);
    //     alive++;

    //     var ec = e.GetComponent<EnemyController>();
    //     if (ec != null)
    //     {
    //         ec.OnEnemyDeath += OnEnemyKilled;
    //     }
    // }

    void SpawnOne()
    {
        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject e = Instantiate(PickEnemy(), p.position, Quaternion.identity);

        alive++;
        e.GetComponent<EnemyController>().OnEnemyDeath += OnEnemyKilled;
    }

    void SpawnBoss()
    {
        if (boss == null || player == null) return;

        bossDone = true;

        Vector3 pos = player.position + new Vector3(10f, 0f, 0f);

        // b = boss
        GameObject b = Instantiate(boss, pos, Quaternion.identity);
        alive++;

        var bc = b.GetComponent<EnemyController>();
        if (bc != null)
        {
            bc.OnEnemyDeath += OnEnemyKilled;
        }

        if (uiManager != null)
            uiManager.ShowWaveMessage("BOSS INCOMING!", 3f);
        Debug.Log("Boss spawned!");
    }

    GameObject PickEnemy()
    {
        if (wave == 1)
            return enemy1;

        if (wave == 2)
            return Random.value > 0.5f ? enemy1 : enemy2;

        if (wave == 3)
        {
            float r = Random.value;
            if (r < 0.33f) return enemy1;
            if (r < 0.66f) return enemy2;
            return enemy3;
        }

        return enemy1;
    }

    void OnEnemyKilled()
    {
        alive--;
    }

    void EndWave()
    {
        waveGoing = false;
        Debug.Log("Wave " + wave + " done!");

        if (uiManager != null)
            uiManager.ShowWaveMessage("WAVE " + wave + " COMPLETE!", 2f);

        StartCoroutine(StartWave());
    }

    public int GetCurrentWave()
    {
        return wave;
    }

    public int GetEnemiesLeft()
    {
        return alive + (toSpawn - spawned);
    }
}