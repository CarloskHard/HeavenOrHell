using UnityEngine;

public class CameraControllerMg2 : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform manosPivot; // Arrastra tu "PlayerPivot"

    [Header("Márgenes")]
    public float margenInferior = 2f; // Espacio debajo de las manos
    public float margenSuperior = 3f; // Espacio arriba de la maleta más alta

    [Header("Ajustes")]
    public float tamanoMinimoSeguridad = 5f; // Para que no haga zoom excesivo al inicio
    public float suavizado = 3f; // Velocidad de la cámara (Lerp)

    private Camera cam;
    private float alturaManos = 0f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // 1. Definir el Límite Inferior (EL SUELO VISUAL)
        // Queremos ver desde las manos hacia abajo un poco
        float limiteInferior = alturaManos - margenInferior;

        // 2. Definir el Límite Superior (EL TECHO VISUAL)
        float alturaMaxima = ObtenerAlturaMaletaMasAlta();
        float limiteSuperior = alturaMaxima + margenSuperior;

        // 3. Calcular la altura total que necesitamos ver
        float alturaNecesaria = limiteSuperior - limiteInferior;

        // 4. Calcular el Size (Zoom)
        // En Unity, OrthographicSize es la MITAD de la altura visible.
        float targetSize = alturaNecesaria / 2f;

        // Evitamos que el zoom sea demasiado pequeño al principio (seguridad)
        targetSize = Mathf.Max(targetSize, tamanoMinimoSeguridad);

        // 5. Calcular la Posición Y de la cámara
        // La cámara debe estar en el punto medio entre el límite inferior y el superior
        // PERO ajustado al nuevo tamaño calculado.

        // Fórmula: El centro es el Límite Inferior + la mitad de la altura que estamos viendo
        float targetY = limiteInferior + targetSize;

        // --- APLICAR CAMBIOS SUAVEMENTE ---

        // Aplicar Zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * suavizado);

        // Aplicar Posición
        Vector3 nuevaPos = transform.position;
        nuevaPos.y = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * suavizado);
        transform.position = nuevaPos;
    }

    float ObtenerAlturaMaletaMasAlta()
    {
        GameObject[] maletas = GameObject.FindGameObjectsWithTag("Maleta");

        // Si no hay maletas, la altura máxima son tus propias manos
        if (maletas.Length == 0) return manosPivot.position.y;

        float maxY = -Mathf.Infinity;

        foreach (var maleta in maletas)
        {
            // Solo miramos maletas que estén por encima de las manos (por si alguna cae al vacío)
            if (maleta != null && maleta.transform.position.y > maxY)
            {
                maxY = maleta.transform.position.y;
            }
        }

        // Si todas las maletas se cayeron al vacío (están por debajo de las manos), 
        // devolvemos la altura de las manos para resetear la cámara.
        if (maxY == -Mathf.Infinity) return manosPivot.position.y;

        return maxY;
    }

    // DIBUJAR LÍNEAS EN EL EDITOR PARA QUE VEAS LO QUE HACE (Opcional)
    void OnDrawGizmos()
    {
        if (manosPivot == null) return;

        Gizmos.color = Color.green;
        // Línea del margen inferior
        Gizmos.DrawLine(new Vector3(-10, manosPivot.position.y - margenInferior, 0), new Vector3(10, manosPivot.position.y - margenInferior, 0));

        Gizmos.color = Color.red;
        // Línea del margen superior (aproximado en editor)
        float alto = ObtenerAlturaMaletaMasAlta() + margenSuperior;
        Gizmos.DrawLine(new Vector3(-10, alto, 0), new Vector3(10, alto, 0));
    }
}