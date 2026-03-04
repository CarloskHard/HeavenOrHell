using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    private ClimberController controller;

    void Start()
    {
        controller = GetComponentInParent<ClimberController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Si tocamos un agarre y el jugador está listo para agarrarse (automático)
        if (col.CompareTag("Agarre") && controller.CanGrab())
        {
            controller.AttachToLedge(col, transform.position);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Agarre") && controller.CanGrab())
        {
            controller.AttachToLedge(col, transform.position);
        }
    }
}