using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("基本参数")]
    public int damage;

    public float attackrange;

    public float attackrate;


    private void OnTriggerStay2D(Collider2D other){
        //访问被攻击的那个人
        //用问号来判断
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}
