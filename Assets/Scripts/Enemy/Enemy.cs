using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rg;

    //只能由内部或子类访问，外部无法访问
    public Animator anim;

    public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;

    public float chaseSpeed;

    public float currentSpeed;

    public Vector3 faceDir;

    public float hurtForce;

    public Transform attacker;

    public Vector3 spwanPoint;

    [Header("检测")]
    public Vector2 centerOffset;

    public Vector2 checkSize;

    public float checkDistance;

    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;

    public float waitTimeCounter;

    public bool wait;

    public float lostTime;

    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;

    public bool isDead;

    protected BaseState currentState;

    protected BaseState patrolState;

    protected BaseState chaseState;

    protected BaseState skillState;

    protected virtual void Awake()
    {
        rg = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;

        //初始化
        waitTimeCounter = waitTime;

        spwanPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;

        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        //currentState.LogicUpdate();

        TimeCounter();
    }

    private void FixedUpdate()
    {
        //currentState.PhysicsUpdate();
        if (!isHurt && !isDead && !wait)
            Move();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PreMove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("snailRecover"));
        rg.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, rg.velocity.y);
    }

    //计时器
    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        // else
        // {
        //     lostTimeCounter = lostTime;
        // }
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();

        currentState = newState;

        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPoint(){
        return transform.position ;
    }

    #region  事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;

        //反转
        //player在野猪右边
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //player在野猪左边
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //受伤被击退
        isHurt = true;

        anim.SetTrigger("hurt");

        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;

        //启动协程
        StartCoroutine(OnHurt(dir));

    }

    //迭代器,协同程序返回值
    private IEnumerator OnHurt(Vector2 dir)
    {
        rg.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;

        anim.SetBool("dead", true);

        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion

    public virtual void OnDrawGizmosSelect()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x ,faceDir.x, 0), 0.2f);
    }
}
