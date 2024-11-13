using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 3f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxLifetime = 5f;
    
    private float currentSpeed;
    private Vector3 direction;
    private float lifetime = 0f;

    private void Start()
    {
        currentSpeed = initialSpeed;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void FixedUpdate()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += direction * currentSpeed * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

        Collider[] targets = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var item in targets)
        {
            if (item.CompareTag("Enemy"))
            {
                PlayerController player = Object.FindAnyObjectByType<PlayerController>();
                if (player != null)
                {
                    player.DestroyEnemy(item.gameObject);
                }
                Destroy(gameObject);
                return;
            }
        }
    }
}