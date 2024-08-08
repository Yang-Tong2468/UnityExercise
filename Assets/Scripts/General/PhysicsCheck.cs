using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    [Header("检测参数")]
    public bool manual;
    public float checkRadius;

    public Vector2 bottomOffset;

    public Vector2 leftOffset;

    public Vector2 rightOffset;

    public LayerMask groundLayer;

    [Header("状态")]
    public bool isGround;

    public bool touchLeftWall;

    public bool touchRightWall;

    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if(!manual){
            rightOffset =  new Vector2((coll.bounds.size.x + coll.offset.x) / 2,coll.bounds.size.y / 2);

            leftOffset =  new Vector2(-rightOffset.x,rightOffset.y);
        }
    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);

        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);

        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}
