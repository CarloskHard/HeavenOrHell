using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerMg2 : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidadRotacion = 150f;
    float anguloMaximoDiseño = 60f;

    [Range(1f, 20f)]
    public float factorSuavizado = 10f;

    [Header("Configuración del Arco")]
    [Range(3f, 30f)]
    public float radioArco = 3f;
    float alturaFijaDeMano = 0f;

    [Header("Límites de Pantalla")]
    [Tooltip("Espacio que dejamos en los bordes para que la mano no se corte (aprox mitad del ancho de la mano)")]
    [Range(0f, 1f)]
    public float margenLateral = 1.0f;

    [Header("Referencias")]
    public Transform handPlatform;

    // Variables internas
    private float anguloObjetivo = 0f;
    private float anguloSuavizado = 0f;
    private float anguloMaximoCalculado = 0f; // El límite real actual
    private Vector2 inputActual;
    private Camera cam;

    void OnValidate()
    {
        ActualizarGeometriaBrazo();
    }

    void Start()
    {
        cam = Camera.main; // Obtenemos la cámara para medir la pantalla

        // Inicializar ángulos
        anguloObjetivo = transform.rotation.eulerAngles.z;
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
        // 1. Calcular el límite dinámico basado en la pantalla
        CalcularLimitePantalla();

        // 2. Actualizar geometría (por si cambias el radio en tiempo real)
        ActualizarGeometriaBrazo();
    }

    void FixedUpdate()
    {
        float inputX = inputActual.x;

        // 3. Acumular ángulo
        anguloObjetivo += inputX * velocidadRotacion * Time.fixedDeltaTime;

        // 4. CLAMP INTELIGENTE
        // Usamos el MENOR valor entre: tu límite de diseño (ej. 60) Y el límite de pantalla calculado
        float limiteFinal = Mathf.Min(anguloMaximoDiseño, anguloMaximoCalculado);

        anguloObjetivo = Mathf.Clamp(anguloObjetivo, -limiteFinal, limiteFinal);

        // 5. Suavizado
        anguloSuavizado = Mathf.Lerp(anguloSuavizado, anguloObjetivo, Time.fixedDeltaTime * factorSuavizado);

        // 6. Aplicar rotación
        transform.rotation = Quaternion.Euler(0, 0, anguloSuavizado);
    }

    void CalcularLimitePantalla()
    {
        if (cam == null) return;

        // A. Calculamos el ancho visible del mundo (Mitad del ancho total)
        float alturaCamara = cam.orthographicSize;
        float anchoPantallaMundo = alturaCamara * cam.aspect;

        // B. Definimos el X máximo al que puede llegar el centro de la mano
        float xMaximoPermitido = anchoPantallaMundo - margenLateral;

        // C. Matemáticas: Despejamos el ángulo de la fórmula "X = Radio * Sin(Angulo)"
        // Angulo = Asin(X / Radio)
        // Clamp es necesario por si el radio es muy pequeño y X/Radio da > 1 (error matemático)
        float ratio = Mathf.Clamp(xMaximoPermitido / radioArco, -1f, 1f);

        // Convertimos de radianes a grados
        anguloMaximoCalculado = Mathf.Asin(ratio) * Mathf.Rad2Deg;
    }

    void ActualizarGeometriaBrazo()
    {
        if (handPlatform == null) return;

        // Mover pivote y mano para mantener la mano a altura fija visualmente
        Vector3 nuevaPosPadre = transform.position;
        nuevaPosPadre.y = alturaFijaDeMano + radioArco;
        transform.position = nuevaPosPadre;

        handPlatform.localPosition = new Vector3(0, -radioArco, 0);
    }

    // Debug visual para ver hasta dónde permite llegar la pantalla
    void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        // Dibujar líneas verticales donde está el límite de la pantalla (con margen)
        float alto = cam.orthographicSize;
        float ancho = alto * cam.aspect;
        float limiteX = ancho - margenLateral;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(limiteX, -10, 0), new Vector3(limiteX, 10, 0));
        Gizmos.DrawLine(new Vector3(-limiteX, -10, 0), new Vector3(-limiteX, 10, 0));
    }
}