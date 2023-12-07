using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        //basic set
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;
        //ȸ�� ����
        transform.localScale = Vector3.zero;

        //property set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch(type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach(Weapon weapon in weapons) 
        {
            switch (weapon.id)
            {
                case 0:
                    float speed = 250 * Character.WeaponSpeed;
                    weapon.speed = speed + (speed * rate); 
                    break;
                default:
                    speed = 1f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;
            }
            
        }
    }

    void SpeedUp()
    {
        float speed = 4 * Character.Speed;
        GameManager.instance.player.speed = speed + (speed * rate);
    }
}
