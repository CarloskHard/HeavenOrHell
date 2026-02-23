using UnityEngine;
using UnityEngine.InputSystem;

public class ClimberController : MonoBehaviour
{
    [Header("Configuración de Salto")]
    public float launchPower = 10f;
    public float maxDragDistance = 3f;

    [Header("Visuales y Brazos")]
    public Transform handsTransform;
    public Sprite handsOpen;
    public Sprite handsClosed;
    public LineRenderer leftArmLine;
    public LineRenderer rightArmLine;
    public float shoulderWidth = 0.4f;
    public float maxElbowBend = 0.3f;
    [Range(0f, 1f)]
    public float escalaManosVuelo = 0.2f; // 0 = en la cara, 1 = posición normal arriba

    [Header("Físicas de Colgado (Unity 6)")]
    public float reboteBrazos = 3.5f;
    public float frenoInerciaMuelle = 0.6f;
    public float resistenciaGiro = 40f;
    public float frenoBalanceo = 3f;

    private Rigidbody2D rb;
    private SpringJoint2D springJoint;
    private SpriteRenderer handsSR;

    private bool isAttached = false;
    private bool isDragging = false;
    private Vector2 dragStartPos;
    private float grabCooldownTimer = 0f;
    private Vector2 anchorPoint;
    private Vector3 defaultHandsLocalPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.angularDamping = 0.05f;
        rb.linearDamping = 0f;

        if (handsTransform != null)
        {
            handsSR = handsTransform.GetComponent<SpriteRenderer>();
            defaultHandsLocalPos = handsTransform.localPosition;
        }

        if (leftArmLine != null) { leftArmLine.positionCount = 3; leftArmLine.enabled = true; }
        if (rightArmLine != null) { rightArmLine.positionCount = 3; rightArmLine.enabled = true; }

        springJoint = gameObject.AddComponent<SpringJoint2D>();
        springJoint.autoConfigureDistance = false;
        springJoint.enabled = false;
    }

    void Update()
    {
        if (grabCooldownTimer > 0) grabCooldownTimer -= Time.deltaTime;

        springJoint.frequency = reboteBrazos;
        springJoint.dampingRatio = frenoInerciaMuelle;

        HandleInput();
        HandleBodyRotation();
        HandleHandsVisuals();
    }

    void LateUpdate()
    {
        UpdateArmsVisuals();
    }

    void HandleInput()
    {
        if (Pointer.current == null) return;

        bool isPressingDown = Pointer.current.press.wasPressedThisFrame;
        bool isPressing = Pointer.current.press.isPressed;
        bool isReleasing = Pointer.current.press.wasReleasedThisFrame;
        Vector2 pointerPos = Pointer.current.position.ReadValue();

        if (isAttached)
        {
            if (isPressingDown)
            {
                isDragging = true;
                dragStartPos = Camera.main.ScreenToWorldPoint(pointerPos);

                springJoint.enabled = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            if (isDragging && isPressing)
            {
                Vector2 currentPos = Camera.main.ScreenToWorldPoint(pointerPos);
                Vector2 pullVector = currentPos - dragStartPos;
                if (pullVector.magnitude > maxDragDistance)
                    pullVector = pullVector.normalized * maxDragDistance;

                Vector3 baseBodyPos = (Vector3)anchorPoint - defaultHandsLocalPos;
                transform.position = baseBodyPos + (Vector3)pullVector;
            }

            if (isDragging && isReleasing)
            {
                isDragging = false;
                Vector2 currentPos = Camera.main.ScreenToWorldPoint(pointerPos);
                Vector2 pullVector = currentPos - dragStartPos;
                if (pullVector.magnitude > maxDragDistance)
                    pullVector = pullVector.normalized * maxDragDistance;

                Launch(-pullVector);
            }
        }
    }

    void Launch(Vector2 forceVector)
    {
        isAttached = false;
        grabCooldownTimer = 0.2f;
        springJoint.enabled = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.angularDamping = 0.05f;
        rb.linearDamping = 0f;

        rb.AddForce(forceVector * launchPower, ForceMode2D.Impulse);
    }

    public void AttachToLedge(Collider2D ledgeCollider, Vector2 handPosition)
    {
        isAttached = true;
        isDragging = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.angularVelocity = 0f;
        rb.angularDamping = resistenciaGiro;
        rb.linearDamping = frenoBalanceo;

        anchorPoint = ledgeCollider.ClosestPoint(handPosition);
        springJoint.connectedAnchor = anchorPoint;
        springJoint.distance = defaultHandsLocalPos.magnitude;
        springJoint.enabled = true;
    }

    void HandleBodyRotation()
    {
        if (isAttached && !isDragging)
        {
            Vector2 dirToAnchor = (Vector2)anchorPoint - (Vector2)transform.position;
            float targetAngle = Mathf.Atan2(dirToAnchor.y, dirToAnchor.x) * Mathf.Rad2Deg - 90f;
            targetAngle = Mathf.Clamp(targetAngle, -20f, 20f);

            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        else if (!isAttached)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 2f);
        }
    }

    void HandleHandsVisuals()
    {
        if (handsSR == null) return;

        if (isAttached || isDragging)
        {
            handsTransform.position = anchorPoint;
            handsSR.sprite = handsClosed;
        }
        else
        {
            // VUELO: Las manos se encogen hacia la cara usando la escala configurada
            handsTransform.localPosition = defaultHandsLocalPos * escalaManosVuelo;
            handsSR.sprite = handsOpen;
        }
        handsTransform.rotation = Quaternion.identity;
    }

    void UpdateArmsVisuals()
    {
        if (leftArmLine == null || rightArmLine == null || handsTransform == null) return;

        Vector3 handsPos = handsTransform.position;
        Vector3 bodyPos = transform.position;
        Vector3 rightDir = transform.right;

        // LÓGICA CORREGIDA POR EL USUARIO:
        Vector3 leftShoulder = bodyPos + rightDir * (shoulderWidth / 2f);
        Vector3 rightShoulder = bodyPos - rightDir * (shoulderWidth / 2f);

        DrawArm(leftArmLine, leftShoulder, handsPos, true);
        DrawArm(rightArmLine, rightShoulder, handsPos, false);
    }

    void DrawArm(LineRenderer line, Vector3 shoulderPos, Vector3 handsPos, bool isLeftArm)
    {
        Vector3 dir = shoulderPos - handsPos;
        float dist = dir.magnitude;
        Vector3 midPoint = (handsPos + shoulderPos) / 2f;

        float restingDist = defaultHandsLocalPos.magnitude;
        float currentBend = 0f;

        // Si la distancia es corta (en vuelo o por rebote), los codos se doblan
        if (dist < restingDist)
        {
            float bendFactor = 1f - (dist / restingDist);
            currentBend = maxElbowBend * bendFactor;
        }

        Vector3 normal = Vector3.Cross(dir, Vector3.forward).normalized;

        // Ajuste de doblado de codos para la nueva orientación de hombros
        if (isLeftArm) normal = normal; // Cambiado para que sigan doblando hacia afuera
        else normal = -normal;

        Vector3 elbowPos = midPoint + normal * currentBend;

        line.SetPosition(0, handsPos);
        line.SetPosition(1, elbowPos);
        line.SetPosition(2, shoulderPos);
    }

    public bool CanGrab() => !isAttached && grabCooldownTimer <= 0f;
}