using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 5f;
    public Sprite[] skins; // Arrastra aquí todos los cochecitos de Cars_final

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // El Spawner llamará a esto al crear el coche
    public void Initialize(bool moveRight, int skinIndex)
    {
        // Aseguramos que sr no sea nulo antes de asignarle el sprite
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (skins != null && skins.Length > 0)
        {
            sr.sprite = skins[skinIndex];
        }
        else
        {
            Debug.LogWarning("¡Ojo! No hay sprites en la lista Skins del coche.");
        }

        sr.flipX = moveRight;

        // Si tu sprite mira a la IZQUIERDA por defecto:
        // Si moveRight es true, speed debe ser positiva.
        // Si moveRight es false, speed debe ser negativa.
        speed = moveRight ? Mathf.Abs(speed) : -Mathf.Abs(speed);
    }


    void Update()
    {
        // Si la velocidad es 0 (porque el jugador nos paró), el coche no se moverá
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Solo se destruye si se está moviendo (opcional)
        if (Mathf.Abs(speed) > 0 && Mathf.Abs(transform.position.x) > 15f)
        {
            Destroy(gameObject);
        }
    }
}