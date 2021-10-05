using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;
    private int currentHP;
    private bool isDead = false;
    [SerializeField] private int damage = 2;
    [SerializeField] private float speed = 3;

    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float chaseRange = 5f;

    [SerializeField] private float patrolCooltime = 1;
    private float lastPatrolTime = 0;
    private Vector3 patrolDir = Vector3.zero;

    [SerializeField] private float attackCooltime = 1;
    private float lastAttackTime = 0;

    private PlayerController player;

    public void Init()
    {
        currentHP = maxHP;

        player = FindObjectOfType<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP = Mathf.Max(currentHP - damage, 0);

        if (currentHP == 0)
        {
            isDead = true;
        }
    }

    public bool IsPlayerDead()
    {
        return player.IsDead;
    }

    public bool IsHerePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        return distance <= attackRange ? true : false;
    }

    public bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        return distance <= chaseRange ? true : false;
    }

    public void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooltime)
        {
            lastAttackTime = Time.time;

            player.TakeDamage(damage);
        }

    }

    public void Chase()
    {
        Vector3 dir = player.transform.position - transform.position;

        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    public void Patrol()
    {
        if (Time.time - lastPatrolTime >= patrolCooltime)
        {
            lastPatrolTime = Time.time;

            if (patrolDir != Vector3.zero)
            {
                patrolDir = Vector3.zero;
            }
            else
            {
                patrolDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }
        }
        transform.position += patrolDir.normalized * speed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
