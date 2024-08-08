using System;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine;
//调用
using UnityEngine.InputSystem;
//调用这个库，调用这个命名空间

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;

    public Vector2 inputDirection;

    //可通过拖拽获得物体的刚体 public Rigidbody2D rb;
    //也可手动获取刚体组件 在Awake函数中
    private Rigidbody2D rg;

    private CapsuleCollider2D coll;

    private PhysicsCheck physicsCheck;

    private PlayerAnimation playerAnimation;

    [Header("基本参数")]
    public float speed;

    private float runspeed;

    private float walkspeed => speed / 2.5f;

    public float jumpForce;

    public float crouchForce;

    public float hurtForce;



    private Vector2 originOffset;

    private Vector2 originSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;

    public PhysicsMaterial2D wall;

    [Header("状态")]

    public bool isCrouch;

    public bool isHurt;

    public bool isDead;

    public bool isAttack;

    private void Awake()
    {
        rg = GetComponent<Rigidbody2D>();

        coll = GetComponent<CapsuleCollider2D>();

        originOffset = coll.offset;
        originSize = coll.size;

        //调用类需要创建一个实例
        inputControl = new PlayerInputControl();

        //跳跃
        inputControl.Gameplay.Jump.started += Jump;

        physicsCheck = GetComponent<PhysicsCheck>();

        #region 左shift键强制走路
        runspeed = speed;

        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkspeed;
        };

        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runspeed;
        };
        #endregion

        //攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;

        playerAnimation = GetComponent<PlayerAnimation>();
    }


    private void OnEnable()
    {
        //启动
        inputControl.Enable();
    }

    private void OnDisable()
    {
        //关闭
        inputControl.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //代码周期性函数
    // void Start()
    // {

    // }

    //Update is called once per frame
    //代码周期性函数
    void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt)
            Move();
    }

    public void Move()
    {
        //在 FixedUpdate() 中执行
        //打点访问其中所有函数方法和变量参数
        //velocity是二维向量，x轴与y轴皆有速度
        //人物移动 
        if (!isCrouch && !isAttack)
            rg.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rg.velocity.y);

        //将float类型的变量强制转换成int
        int faceDir = (int)transform.localScale.x;
        //人物翻转,localscale是三维向量
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            //修改碰撞体大小及位移
            coll.offset = new Vector2(-0.05f, 0.85f);
            coll.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            //还原
            coll.offset = originOffset;
            coll.size = originSize;
        }
    }
    
    private void Jump(InputAction.CallbackContext context)
    {
        //验证空格键和A键已可控制人物跳跃 Debug.Log("jump");
        if (physicsCheck.isGround)
            rg.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        //在空中无法攻击
        if(physicsCheck.isGround){
        playerAnimation.PlayerAttack();
        isAttack = true;
        }
        // if(!physicsCheck.isGround)
        // return;
        // playerAnimation.PlayerAttack();
        // isAttack = true;
    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        //受伤反弹效果
        isHurt = true;
        //速度皆为0
        rg.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        rg.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion

    private void CheckState(){
        //三元运算符，如果isGround 就选择normal  否则选择wall
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
}