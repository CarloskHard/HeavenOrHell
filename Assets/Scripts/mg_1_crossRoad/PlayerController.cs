using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 moveInput;
    private bool isDead = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim; // <--- REFERENCIA AL ANIMATOR

    // Límites
    private float minX, maxX;
    public float minY = -1f;

    [Header("Configuración de Muerte")]
    public float knockbackForce = 10f;
    public float spinForce = 500f;
    public float deathLinearDamping = 3f;
    public float deathAngularDamping = 3f;

    [Header("Efectos")]
    public ParticleSystem bloodPrefab;

    [Header("Sonidos")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // <--- OBTENER EL ANIMATOR

        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        CalculateBoundaries();
    }

    public void OnMove(InputValue value)
    {
        if (isDead) return;
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (isDead) return;

        // Movimiento normal
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // --- LÓGICA DE ANIMACIÓN ---
        // 1. Caminar: Si el valor absoluto de moveInput es mayor a casi cero, está caminando
        bool isWalking = moveInput.sqrMagnitude > 0.01f;
        anim.SetBool("isWalking", isWalking);

        // 2. Voltear Sprite: Si va a la izquierda, mirror del sprite
        if (moveInput.x > 0) sr.flipX = false;
        else if (moveInput.x < 0) sr.flipX = true;
        // ---------------------------

        // Limitar posición
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Max(transform.position.y, minY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
            {
                ProcessHit(car);
            }
        }
    }

    void ProcessHit(Car car)
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (bloodPrefab != null)
        {
            ParticleSystem particles = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            float angle = car.speed > 0 ? 0 : 180;
            float yRotation = car.speed > 0 ? 90f : -90f;
            particles.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        float carSpeedAtImpact = car.speed;
        car.processHit();

        if (!isDead)
        {
            isDead = true;

            // --- ACTIVAR ANIMACIÓN DE MUERTE ---
            anim.SetBool("dead", true);
            // -----------------------------------

            rb.linearDamping = deathLinearDamping;
            rb.angularDamping = deathAngularDamping;
            rb.freezeRotation = false;
        }

        Vector2 impactDir = new Vector2(carSpeedAtImpact, 0);
        Vector2 force = (impactDir + moveInput) * knockbackForce;

        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-spinForce, spinForce));

        Debug.Log("¡Impacto detectado!");
    }

    // El método Die original parece ser una versión alternativa del ProcessHit. 
    // Si decides usar Die en lugar de ProcessHit, asegúrate de añadir anim.SetTrigger("die") ahí también.

    void CalculateBoundaries()
    {
        Camera cam = Camera.main;
        float distance = transform.position.z - cam.transform.position.z;
        Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, distance));

        float playerHalfWidth = 0.5f;
        if (sr != null) playerHalfWidth = sr.bounds.extents.x;

        minX = leftEdge.x + playerHalfWidth;
        maxX = rightEdge.x - playerHalfWidth;
    }
}