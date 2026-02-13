using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 moveInput;
    private float minX, maxX;

    void Start()
    {
        CalculateBoundaries();
    }

    void CalculateBoundaries()
    {
        // 1. Obtenemos la cámara principal
        Camera cam = Camera.main;

        // 2. Calculamos el borde izquierdo (0) y derecho (1) en el mundo
        // Usamos la posición Z del jugador respecto a la cámara para mayor precisión
        float distance = transform.position.z - cam.transform.position.z;

        Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, distance));

        // 3. Obtenemos el ancho del jugador
        float playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;

        // 4. Guardamos los límites finales
        minX = leftEdge.x + playerHalfWidth;
        maxX = rightEdge.x - playerHalfWidth;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Movimiento normal
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // APLICAR LÍMITES DINÁMICOS
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}