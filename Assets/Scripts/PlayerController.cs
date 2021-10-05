using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;
    private int currentHP;
    private bool isDead = false;
    public bool IsDead => isDead;
    [SerializeField] private int damage = 2;
    [SerializeField] private float speed = 3;

    private Vector2 originMousePos;
    private Color originColor;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        currentHP = maxHP;

        meshRenderer = GetComponent<MeshRenderer>();

        originColor = meshRenderer.material.color;
    }

    private void Update()
    {
        Move();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP = Mathf.Max(currentHP - damage, 0);

        StopCoroutine("HitFlash");
        StartCoroutine("HitFlash");

        if (currentHP == 0)
        {
            isDead = true;
        }
    }

    private IEnumerator HitFlash()
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / 0.2f;

            meshRenderer.material.color = Color.Lerp(Color.red, originColor, percent);

            yield return null;
        }

        meshRenderer.material.color = originColor;
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            originMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mouseDir = (Vector2)Input.mousePosition - originMousePos;
            Vector3 dir = new Vector3(mouseDir.x, 0, mouseDir.y);

            transform.position += dir.normalized * speed * Time.deltaTime;
        }
    }
}
