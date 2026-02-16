using UnityEngine;
using UnityEngine.InputSystem; // Obligatorio para el nuevo sistema

public class PlayerMovement : MonoBehaviour
{
    private Vector2 inputMovimiento;
    public float velocidad = 5f;

    public void OnMove(InputValue value)
    {
        inputMovimiento = value.Get<Vector2>();
    }

    void Update()
    {
        // Aplicamos el movimiento
        Vector3 movimiento = new Vector3(inputMovimiento.x, inputMovimiento.y, 0);
        transform.Translate(movimiento * velocidad * Time.deltaTime, Space.World);
    }
}