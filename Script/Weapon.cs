using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            case 1:
                timer += Time.deltaTime;
                if(timer > speed) 
                {
                    timer = 0f;
                    Fire();
                }
                break;
            case 2:
                break;
            case 6:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Throw();
                }
                break;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage += damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        if (id == 2)
            Summon();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        //기본 셋팅
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount;

        for (int index= 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if(data.progectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 250 * Character.WeaponSpeed;
                Batch(); // 방패를 하나 더 생성
                break;
            case 1:
                speed = 1f * Character.WeaponRate; 
                break;
            case 2:
                Summon(); // 검을 하나 더 생성
                break;
            case 6:
                speed = 2f * Character.WeaponRate;
                Throw();
                break;

        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch()
    {
        for (int index = 0; index < count; index++) 
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
            }            
            bullet.parent = transform;

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 roVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(roVec);
            bullet.Translate(bullet.up * 1.1f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // 무한히 관통
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.sfx.Range);
    }

    void Summon()
    {
        Vector2 dir = new Vector2(0, 1);
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(damage, 150, dir);
    }

    void Throw()
    {
        Vector2 dir = new Vector2(0, 1);
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
