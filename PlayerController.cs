using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject bulletPrefab; // Cambiar el nombre para mayor claridad
    [SerializeField] private Transform rifleStart;
    [SerializeField] private Text HpText;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private GameObject Victory;
    
    [Header("Configuración")]
    [SerializeField] private float moveSpeed = 5f;
    
    private CharacterController controller;
    private int score = 0;
    private bool isGameOver = false;
    private int enemiesDestroyed = 0;
    private GameObject bulletReference; // Nueva variable para mantener una copia de seguridad
    
    void Start()
    {
        // Verificar componentes necesarios
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("No se encontró CharacterController en " + gameObject.name);
            enabled = false;
            return;
        }

        // Crear una copia de seguridad del prefab
        if (bulletPrefab != null)
        {
            bulletReference = bulletPrefab;
            Debug.Log("Bullet reference initialized successfully");
        }
        else
        {
            Debug.LogError("¡Falta la referencia al prefab bullet! Asígnalo en el Inspector.");
            enabled = false;
            return;
        }
        
        if (rifleStart == null || HpText == null || GameOver == null || Victory == null)
        {
            Debug.LogError("Faltan referencias requeridas en PlayerController");
            enabled = false;
            return;
        }

        // Inicializar UI
        UpdateScore(0);
        GameOver.SetActive(false);
        Victory.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver && controller != null && controller.enabled)
        {
            HandleMovement();
            HandleShooting();
            CheckCollisions();
        }
    }

    void HandleMovement()
    {
        if (controller == null || !controller.enabled) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = transform.right * horizontalInput + transform.forward * verticalInput;
        controller.Move(movement * moveSpeed * Time.deltaTime);
        
        // Aplicar gravedad básica
        controller.Move(Physics.gravity * Time.deltaTime);
    }

    void HandleShooting()
    {
        // Usar la referencia de respaldo si la principal se pierde
        GameObject currentBullet = bulletPrefab != null ? bulletPrefab : bulletReference;

        if (currentBullet == null)
        {
            Debug.LogError("No se puede encontrar la referencia al bullet. Verifica el prefab en el Inspector.");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            try
            {
                GameObject buf = Instantiate(currentBullet, rifleStart.position, transform.rotation);
                
                if (buf == null)
                {
                    Debug.LogError("Error al instanciar el bullet");
                    return;
                }
                
                Bullet bulletComponent = buf.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.SetDirection(transform.forward);
                }
                else
                {
                    Debug.LogError("El prefab bullet no tiene el componente Bullet");
                    Destroy(buf);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error al disparar: {e.Message}");
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Collider[] tar = Physics.OverlapSphere(transform.position, 2);
            foreach (var item in tar)
            {
                if (item != null && item.CompareTag("Enemy"))
                {
                    DestroyEnemy(item.gameObject);
                }
            }
        }
    }

    void CheckCollisions()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, 3);
        foreach (var item in targets)
        {
            if (item == null) continue;

            try
            {
                if (item.CompareTag("Heal"))
                {
                    UpdateScore(50);
                    Destroy(item.gameObject);
                }
                else if (item.CompareTag("Finish"))
                {
                    Win();
                }
                else if (item.CompareTag("Enemy"))
                {
                    Lost();
                }
            }
            catch (UnityException e)
            {
                Debug.LogWarning($"Error al verificar tag en {item.name}: {e.Message}");
            }
        }
    }

    public void DestroyEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        enemiesDestroyed++;
        UpdateScore(10); // Aumentamos 10 puntos por cada enemigo
        Destroy(enemy);
        
        if (enemiesDestroyed >= 3)
        {
            Win();
        }
    }

    private void UpdateScore(int points)
    {
        score += points;
        if (HpText != null)
        {
            HpText.text = score.ToString();
        }
    }

    public void Win()
    {
        if (!isGameOver && Victory != null)
        {
            isGameOver = true;
            Victory.SetActive(true);
            DisableControls();
        }
    }

    public void Lost()
    {
        if (!isGameOver && GameOver != null)
        {
            isGameOver = true;
            GameOver.SetActive(true);
            DisableControls();
        }
    }

    private void DisableControls()
    {
        PlayerLook playerLook = GetComponent<PlayerLook>();
        if (playerLook != null)
        {
            Destroy(playerLook);
        }
        Cursor.lockState = CursorLockMode.None;
    }
}