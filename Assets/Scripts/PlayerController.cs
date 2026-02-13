using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 moveInput;
    private bool isDead = false;

    [Header("Límites de Movimiento")]
    public float minY = -1f;
    public float maxY = 1000f;
    private float minX, maxX;

    void Start()
    {
        CalculateBoundaries();
    }

    public void OnMove(InputValue value)
    {
        // Si estamos muertos, no guardamos el movimiento
        if (isDead)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (isDead) return;

        // 1. Movimiento
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // 2. Aplicar límites horizontales (Dinámicos)
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);

        // 3. Aplicar límites verticales (Manuales)
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        // 4. Aplicar posición final
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si chocamos con un coche y aún no estamos muertos...
        if (other.CompareTag("Car") && !isDead)
        {
            Die(other.gameObject);
        }
    }

    void Die(GameObject carThatHitMe)
    {
        isDead = true;
        moveInput = Vector2.zero; // Frenamos cualquier movimiento pendiente
        Debug.Log("¡GAME OVER!");

        // 1. Le decimos al coche que frene
        Car carScript = carThatHitMe.GetComponent<Car>();
        if (carScript != null)
        {
            carScript.speed = 0; // Frenazo instantáneo
        }

        // 2. Opcional: Cambiar color al jugador para que se note la muerte
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    void CalculateBoundaries()
    {
        Camera cam = Camera.main;
        float distance = transform.position.z - cam.transform.position.z;
        Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, distance));
        float playerHalfWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        minX = leftEdge.x + playerHalfWidth;
        maxX = rightEdge.x - playerHalfWidth;
    }
}