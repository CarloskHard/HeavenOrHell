using UnityEngine;

public class CalculadoraDeInercia : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 ultimaPosicion;

    // "Saltamos" los primeros frames para evitar que las maletas
    // salgan volando al iniciar el juego por el teletransporte inicial.
    private int framesDeEspera = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Inicializamos la posición para que no empiece en (0,0)
        ultimaPosicion = rb.position;
    }

    void FixedUpdate()
    {
        if (framesDeEspera > 0)
        {
            framesDeEspera--;
            ultimaPosicion = rb.position; // Sincronizamos sin calcular
            return;
        }

        Vector2 nuevaPosicion = rb.position;

        // Calculamos velocidad: Distancia / Tiempo
        // Usamos Lerp (suavizado) para evitar picos bruscos que lancen las maletas
        Vector2 velocidadCalculada = (nuevaPosicion - ultimaPosicion) / Time.fixedDeltaTime;

        // IMPORTANTE: Solo aplicamos velocidad lineal (movimiento), NO rotación.
        // Esto arregla el problema de que la mano gire sobre sí misma.
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, velocidadCalculada, 0.5f);

        ultimaPosicion = nuevaPosicion;
    }
}