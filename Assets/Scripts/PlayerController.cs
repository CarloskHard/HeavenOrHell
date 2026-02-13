using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float xRange = 3.5f;

    private Vector2 moveInput;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // 1. Mover al personaje
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        // 2. NUEVO: Limitar la posición X
        // Mathf.Clamp "encierra" el valor entre el mínimo (-xRange) y el máximo (xRange)
        float clampedX = Mathf.Clamp(transform.position.x, -xRange, xRange);

        // Aplicamos la posición limitada (manteniendo la Y y Z actuales)
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}