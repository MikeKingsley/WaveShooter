using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SpawnEnemy_Astar : MonoBehaviour
{
    public Transform Waypoint;
    public Transform Container;
    public GameObject[] EnemiesToSpawn;
    public GameObject[] EquipmentToSpawn;
    public Transform[] Spawnpoints;
    public int count = 10;
    public int waves = 5;
    public float spawnTimer = 1f;
    public float waveTimer = 25f;
    public bool updateScoreboard;

    Scoreboard score;
    int currentWave = 0;

    private void Start()
    {
        if (updateScoreboard)
        {
            score = FindObjectOfType<Scoreboard>();
        }
        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        if (score != null)
        {
            score.UpdateWaveInfo(currentWave, EnemiesRemaining());
        }
    }

    IEnumerator SpawnWave()
    {
        currentWave++;

        while (EnemiesRemaining() < count)
        {
            Spawn();
            yield return new WaitForSeconds(spawnTimer);
        }

        yield return new WaitUntil(() => WaveCleared());
        if (currentWave < waves)
        {
            StartCoroutine(SpawnWave());
        }
    }

    void Spawn()
    {
        GameObject agent = Instantiate(PickRandomEnemy(), Container);
        Transform startPos = PickSpawnPoint();

        if (agent != null)
        {
            agent.transform.position = startPos.position;
            agent.transform.rotation = startPos.rotation;

            if (agent.GetComponent<StateController>() != null)
            {
                StateController sc = agent.GetComponent<StateController>();
                sc.weaponToUse = PickRandomEquipment();
                sc.SpawnWeapons();
                sc.StartDestination = Waypoint;
                sc.EnemySpawnContainer = Container;
                sc.SetStartPosition(startPos);
            }

            if (score != null)
            {
                score.UpdateWaveInfo(currentWave, EnemiesRemaining());
            }
        }
    }

    public int EnemiesRemaining()
    {
        return Container.childCount;
    }

    public bool WaveCleared()
    {
        if (EnemiesRemaining() == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public Transform PickSpawnPoint()
    {
        if (Spawnpoints != null)
        {
            return Spawnpoints[Mathf.RoundToInt(Random.Range(0, Spawnpoints.Length))].transform;
        }
        else
        {
            Debug.LogWarning("No spawnpoint defined in " + transform.name);
            return null;
        }
    }

    public GameObject PickRandomEnemy()
    {
        if (EnemiesToSpawn != null)
        {
            return EnemiesToSpawn[Mathf.RoundToInt(Random.Range(0, EnemiesToSpawn.Length))].gameObject;
        }
        else
        {
            Debug.LogWarning("No enemies defined in " + transform.name);
            return null;
        }
    }

    public GameObject PickRandomEquipment()
    {
        if (EquipmentToSpawn != null)
        {
            return EquipmentToSpawn[Mathf.RoundToInt(Random.Range(0, EquipmentToSpawn.Length))].gameObject;
        }
        else
        {
            Debug.LogWarning("No equipment defined in " + transform.name);
            return null;
        }
    }

}
