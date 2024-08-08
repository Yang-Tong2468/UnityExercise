      using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private Rigidbody2D rb;

    private PhysicsCheck physicsCheck;

    private PlayerController playerController;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        physicsCheck = GetComponent<PhysicsCheck>();

        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        //取绝对值使他无论左右都可跑动
        anim.SetFloat("velocityX",Mathf.Abs(rb.velocity.x));

        anim.SetFloat("velocityY",rb.velocity.y);

        anim.SetBool("isGround",physicsCheck.isGround);

        anim.SetBool("isCrouch",playerController.isCrouch);

        anim.SetBool("isDead",playerController.isDead);

        anim.SetBool("isAttack",playerController.isAttack);

    }

    public void PlayHurt(){
        anim.SetTrigger("hurt");
    }

    public void PlayerAttack(){
        anim.SetTrigger("attack");
    }
}
