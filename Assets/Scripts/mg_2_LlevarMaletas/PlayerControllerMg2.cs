using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerMg2 : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadRotacion = 150f;
    public float anguloMaximo = 60f;

    [Range(1f, 20f)]
    public float factorSuavizado = 10f; // Slider de suavizado (Más bajo = más lento/pesado)

    [Header("Configuración del Arco (Radio)")]
    [Range(5f, 50f)]
    public float radioArco = 3f; // Slider de la curva

    private float alturaFijaDeMano = 0f;

    [Header("Referencias")]
    public Transform handPlatform; // Arrastra el objeto hijo HandPlatform
    public Rigidbody2D rb;         // Arrastra el Rigidbody (normalmente en HandPlatform o Pivot)

    // Variables internas
    private float anguloObjetivo = 0f;
    private float anguloSuavizado = 0f;
    private Vector2 inputActual;

    void OnValidate()
    {
        // Esto permite ver los cambios en el Editor sin dar Play
        ActualizarGeometriaBrazo();
    }

    void Start()
    {
        // Inicializamos los ángulos
        anguloObjetivo = transform.rotation.eulerAngles.z;
        // Ajustamos los ángulos para que vayan de -180 a 180
        if (anguloObjetivo > 180) anguloObjetivo -= 360;

        anguloSuavizado = anguloObjetivo;

        ActualizarGeometriaBrazo();
    }

    public void OnMove(InputValue value)
    {
        inputActual = value.Get<Vector2>();
    }

    void Update()
    {
        // Calculamos la geometría en Update para que si mueves el slider jugando, 
        // se ajuste en tiempo real sin tirar las maletas.
        ActualizarGeometriaBrazo();
    }

    void FixedUpdate()
    {
        // 1. Calcular el input
        float inputX = inputActual.x;

        // 2. Acumular ángulo (CONTROLES INVERTIDOS: Usamos '+=' en vez de '-=')
        // Antes: Derecha restaba ángulo. Ahora: Derecha suma ángulo.
        anguloObjetivo += inputX * velocidadRotacion * Time.fixedDeltaTime;

        // 3. Limitar (Clamp) el ángulo objetivo
        anguloObjetivo = Mathf.Clamp(anguloObjetivo, -anguloMaximo, anguloMaximo);

        // 4. Suavizado (Interpolación Lineal)
        // Lerp acerca el ángulo actual al objetivo basándose en el factor de suavizado
        anguloSuavizado = Mathf.Lerp(anguloSuavizado, anguloObjetivo, Time.fixedDeltaTime * factorSuavizado);

        // 5. Aplicar la rotación física
        if (rb != null)
        {
            rb.MoveRotation(anguloSuavizado);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, anguloSuavizado);
        }
    }

    // Esta es la función mágica que ajusta Padre e Hijo a la vez
    void ActualizarGeometriaBrazo()
    {
        if (handPlatform == null) return;

        // A. Colocar el Pivote (Padre) más arriba según el radio crece
        // La posición del padre será la altura deseada de la mano + el radio
        // Mantenemos la X y Z originales del padre.
        Vector3 nuevaPosPadre = transform.position;
        nuevaPosPadre.y = alturaFijaDeMano + radioArco;
        transform.position = nuevaPosPadre;

        // B. Colocar la Mano (Hijo) más abajo en local
        // Al ser posición local, es relativa al padre.
        handPlatform.localPosition = new Vector3(0, -radioArco, 0);
    }
}