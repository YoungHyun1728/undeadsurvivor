using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawndata;
    public float levelTime;

    int spwanLevel;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawndata.Length;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime;

        spwanLevel = Mathf.Min(Mathf.FloorToInt( GameManager.instance.gameTime / levelTime), spawndata.Length-1);

        if (timer > spawndata[spwanLevel].spawnTime) 
        {
            timer = 0;
            Spawn();
        }

        void Spawn() 
        {
            GameObject enemy = GameManager.instance.pool.Get(spwanLevel);
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().Init(spawndata[spwanLevel]);
        }
    }
}

[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
