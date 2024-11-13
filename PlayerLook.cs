using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float mouseSense = 2.0f;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera; // Referencia a la cámara

    private float verticalRotation = 0f;

    void Start()
    {
        if (player == null || playerCamera == null)
        {
            Debug.LogError("Faltan referencias en PlayerLook!");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSense;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSense;

        // Rotación vertical (mirando arriba/abajo)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        
        // Rotación horizontal (girando izquierda/derecha)
        player.Rotate(Vector3.up * mouseX);
    }
}