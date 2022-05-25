using UnityEngine;

public class Melee_Enemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    private HealthController playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        cooldownTimer += Time.deltaTime;
        if (PlayerVisual())
        {   
            if (cooldownTimer >= attackCooldown)
            {
            //anim (animator) goes here
            cooldownTimer = 0;

             Debug.Log("Attack");
            }
        }
    }

    private bool PlayerVisual()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z) 
        , 0,Vector2.left ,0 ,playerLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<HealthController>();

        return hit.collider != null;
    }

    private void DamagePlayer()
    {
        if (PlayerVisual())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

}
