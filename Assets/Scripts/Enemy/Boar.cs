using Unity.VisualScripting;
using UnityEngine;

//继承Enemy类
public class Boar : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        patrolState = new BoarPatrolState();

        chaseState = new BoarChaseState();

    }
}
