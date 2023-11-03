using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EnemyState
{
    Idle,
    Move,
    Attack,
    Hurt,
    Die,
}

public class Enemy : MonoBehaviour
{
    public EnemyState enemyState;

    public Animator anim;

    private float speed;
    public float moveSpeed;
    public float attackSpeed;

    public float findRange;
    public float damage;
    public Transform player;

    private AudioSource audioSrc;
    public Transform fxPoint;
    public GameObject hitFx;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public GameObject guiPivot;
    public Slider lifebar;
    public float maxHp;
    public float hp;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (enemyState == EnemyState.Idle)
            DistanceCheck();
        else if (enemyState == EnemyState.Move)
        {
            MoveUpdate();
            AttackRangeCheck();
        }
    }

    private void AttackRangeCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < 1.5f && enemyState != EnemyState.Attack)
        {
            speed = 0;
            enemyState = EnemyState.Attack;
            anim.SetTrigger("attack");
        }
    }
    private void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) >= findRange)
        {
            if (enemyState != EnemyState.Idle)
            {
                enemyState = EnemyState.Idle;
                anim.SetBool("run", false);
                speed = 0;
            }
        }
        else
        {
            if (enemyState != EnemyState.Move)
            {
                enemyState = EnemyState.Move;
                anim.SetBool("run", true);
                speed = moveSpeed;
            }
        }
    }

    private void MoveUpdate()
    {
        Vector3 dir = new Vector3(player.position.x, transform.position.y, player.position.z) - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void AttackOn()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.Hurt(damage);
    }

    public void Hurt(float damage)
    {
        if (hp > 0)
        {
            enemyState = EnemyState.Hurt;
            speed = 0;
            anim.SetTrigger("hurt");

            Instantiate(hitFx, fxPoint.position, Quaternion.LookRotation(fxPoint.forward));

            hp -= damage;
            lifebar.value = hp / maxHp;

            audioSrc.clip = hitSound;
            audioSrc.Play();
        }

        if (hp <= 0)
            Death();
    }

    public void Death()
    {
        enemyState = EnemyState.Die;
        anim.SetTrigger("die");
        speed = 0;

        guiPivot.SetActive(false);
        audioSrc.clip = deathSound;
        audioSrc.Play();
    }
}
