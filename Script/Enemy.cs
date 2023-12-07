using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator animator;
    WaitForFixedUpdate wait;
    SortingGroup sortingGroup;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sortingGroup = GetComponent<SortingGroup>();
        animator = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;

        animator.SetFloat("Speed", nextVec.magnitude);
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (target.position.x > rigid.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        sortingGroup.sortingOrder = 4;
        animator.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        speed = data.speed;
        maxHealth = data.health;
        health= data.health;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return; 
    
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());       

        if(health > 0)
        {
            animator.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.sfx.Hit);
        }
        else
        {
            isLive= false;
            coll.enabled= false;
            rigid.simulated = false;
            sortingGroup.sortingOrder = 5;
            animator.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if(GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.sfx.Dead);
        }

    }

    IEnumerator KnockBack()
    {        
        Vector3 PlayerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - PlayerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        yield return wait;
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
