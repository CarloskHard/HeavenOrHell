using UnityEngine;
using UnityEngine.SceneManagement;

public class gm_3_manager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public CameraFollow cameraScript;

    [Header("Reglas de Desplazamiento")]
    public float bufferCamara = 5f; // Distancia que el jugador puede bajar antes de que la cámara se clave
    private float alturaMaximaAlcanzada = 0f;
    private float sueloActualInfranqueable = -1000f;

    [Header("Reglas de Muerte")]
    public float margenMuerte = 2f; // Metros por debajo del borde de pantalla para morir
    private bool haMuerto = false;
    private Camera cam;

    void Start()
    {
        cam = cameraScript.GetComponent<Camera>();

        // Inicializamos el suelo en la posición inicial del jugador menos un poco de margen
        if (player != null)
        {
            alturaMaximaAlcanzada = player.position.y;
            sueloActualInfranqueable = alturaMaximaAlcanzada - bufferCamara;
            cameraScript.minY = sueloActualInfranqueable;
        }
    }

    void Update()
    {
        if (haMuerto || player == null || cameraScript == null) return;

        ControlarSueloYCamara();
        ComprobarMuerte();
    }

    void ControlarSueloYCamara()
    {
        // 1. Detectamos si el jugador ha subido a un nuevo récord personal
        if (player.position.y > alturaMaximaAlcanzada)
        {
            alturaMaximaAlcanzada = player.position.y;
        }

        // 2. El "Suelo Infranqueable" es el récord actual menos el buffer
        float posibleSuelo = alturaMaximaAlcanzada - bufferCamara;

        // 3. LÓGICA DE TRINQUETE: El suelo solo puede subir, nunca bajar
        if (posibleSuelo > sueloActualInfranqueable)
        {
            sueloActualInfranqueable = posibleSuelo;

            // Le enviamos a la cámara su nuevo límite mínimo de Y
            cameraScript.minY = sueloActualInfranqueable;
        }
    }

    void ComprobarMuerte()
    {
        // Calculamos dónde está el borde inferior de la cámara en este momento
        float bordeInferior = cam.transform.position.y - cam.orthographicSize;

        // Si el jugador cae por debajo del borde visible + el margen de muerte
        if (player.position.y < (bordeInferior - margenMuerte))
        {
            Morir();
        }
    }

    void Morir()
    {
        haMuerto = true;
        Debug.Log("Game Over");
        Invoke("ReiniciarNivel", 1.5f);
    }

    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}