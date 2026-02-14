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
        // Si estamos muertos, no aplicamos movimiento manual ni Clamping
        // así permitimos que el jugador salga volando fuera de los bordes
        if (isDead) return;

        // Movimiento normal
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // Limitar posición (solo mientras vive)
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
                // Ejecutamos el impacto SIEMPRE (vivo o muerto)
                ProcessHit(car);
            }
        }
    }

    void ProcessHit(Car car)
    {
        // 1. Sonido de impacto
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (bloodPrefab != null)
        {
            // 1. Instanciamos las partículas en la posición del jugador
            ParticleSystem particles = Instantiate(bloodPrefab, transform.position, Quaternion.identity);

            // 2. Orientarlas según la dirección del coche
            // Si el coche va a la derecha (velocidad positiva), el ángulo es 0
            // Si va a la izquierda, el ángulo es 180
            float angle = car.speed > 0 ? 0 : 180;
            float yRotation = car.speed > 0 ? 90f : -90f;
            particles.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        // 2. Guardamos la velocidad del coche antes de frenarlo
        float carSpeedAtImpact = car.speed;
        car.processHit();

        // 3. Si es el primer golpe, activamos el estado de muerte
        if (!isDead)
        {
            isDead = true;
            sr.color = Color.red;
            rb.linearDamping = deathLinearDamping;
            rb.angularDamping = deathAngularDamping;
            rb.freezeRotation = false; // Permitimos que gire
        }

        // 4. Aplicar el empujón (Knockback)
        // Calculamos la dirección del golpe basada en la velocidad del coche
        Vector2 impactDir = new Vector2(carSpeedAtImpact, 0);

        // Si el jugador estaba moviéndose, sumamos su inercia solo en el primer golpe
        // (En los siguientes golpes el moveInput será cero)
        Vector2 force = (impactDir + moveInput) * knockbackForce;

        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-spinForce, spinForce));

        Debug.Log("¡Impacto detectado!");
    }

    void Die(GameObject carThatHitMe)
    {
        isDead = true;

        rb.linearDamping = deathLinearDamping;
        rb.angularDamping = deathAngularDamping;

        sr.color = Color.gray;

        // 1. Obtener datos del coche
        Car carScript = carThatHitMe.GetComponent<Car>();
        float carVelocityX = carScript.speed;

        // El coche da un "frenazo" pero guardamos su fuerza para el golpe
        carScript.speed = 0;

        // 2. CALCULAR EL LANZAMIENTO
        // Vector del coche (Horizontal) + Vector del jugador (WASD que llevaba)
        Vector2 carForce = new Vector2(carVelocityX, 0) * knockbackForce;
        Vector2 playerForce = moveInput * speed;
        Vector2 finalLaunchVector = carForce + playerForce;

        // 3. APLICAR FÍSICA
        // Permitimos que el objeto rote para que el atropello sea realista
        rb.freezeRotation = false;

        // Aplicamos el vector como velocidad instantánea
        rb.linearVelocity = finalLaunchVector;

        // Añadimos un poco de rotación aleatoria para que "ruede" por el suelo
        rb.AddTorque(Random.Range(-500f, 500f));

        Debug.Log("¡Atropellado! Fuerza aplicada: " + finalLaunchVector);
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