using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public float waitTimer = 2f;
    public GameObject[] prefab;

    float timeElapsed;
    bool allowSpawn = true;
    GameObject spawned;

    void Start()
    {
        Spawn();
        allowSpawn = false;
    }

    void Update()
    {
        if (spawned == null)
            allowSpawn = true;

        if (!allowSpawn)
            return;
        
        if (CheckWaitTimer())
        {
            Spawn();
        }
    }

    bool CheckWaitTimer()
    {
        timeElapsed += Time.deltaTime;
        return (timeElapsed >= waitTimer);
    }

    void Spawn()
    {
        spawned = Instantiate(PickRandomPrefab(), transform.position, transform.rotation);
        timeElapsed = 0;
        allowSpawn = false;
    }

    void OnTriggerStay(Collider collider)
    {
        if (spawned != null && collider.transform.root == spawned.transform.root)
        {
            allowSpawn = false;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (spawned != null && collider.transform.root == spawned.transform.root)
        {
            allowSpawn = true;
        }
    }

    public GameObject PickRandomPrefab()
    {
        if (prefab != null)
        {
            return prefab[Mathf.RoundToInt(Random.Range(0, prefab.Length))].gameObject;
        }
        else
        {
            Debug.LogWarning("No prefabs defined in " + transform.name);
            return null;
        }
    }

}
