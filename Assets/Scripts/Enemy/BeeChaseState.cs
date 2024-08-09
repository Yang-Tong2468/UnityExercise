using UnityEngine;

public class BeeChaseState : BaseState
{
    private Vector3 target;

    private Vector3 moveDir;

    private Attack attack;

    private bool isAttack;

    private float attackRateCounter = 0;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;

        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;

        attack = enemy.GetComponent<Attack>();

        currentEnemy.lostTimeCounter = currentEnemy.lostTime;

        currentEnemy.anim.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);


        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackrange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackrange)
        {
            isAttack = true;

            currentEnemy.rg.velocity = Vector2.zero;

            attackRateCounter-= Time.deltaTime;
            if(attackRateCounter<=0){
                attackRateCounter = attack.attackrate;
                currentEnemy.anim.SetTrigger("attack");
            }
        }else{
            isAttack = false;
        }

        moveDir = (target - currentEnemy.transform.position).normalized;

        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rg.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }

    }

    public override void OnExit()
    {

        Debug.Log("Exit");
        currentEnemy.anim.SetBool("chase", true);
    }
}
