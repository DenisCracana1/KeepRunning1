using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerController player;

    [Header("Ajustes generales")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);
    [Range(0.01f, 1f)] public float smoothX = 0.12f;
    [Range(0.01f, 1f)] public float smoothY = 0.12f;

    [Header("Deadzone")]
    public Vector2 deadzoneSize = new Vector2(2.5f, 1.4f);

    [Header("Look Ahead")]
    public float lookAheadDistance = 1.2f;
    public float lookAheadReturnSpeed = 0.1f;

    [Header("Límites del mapa")]
    public float minX = -Mathf.Infinity;
    public float maxX = Mathf.Infinity;
    public float minY = -Mathf.Infinity;
    public float maxY = Mathf.Infinity;

    private Camera cam;
    private float velX, velY;
    private float currentLookAhead;
    private float lookAheadVel;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (player == null)
            player = Object.FindFirstObjectByType<PlayerController>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 camPos = transform.position;
        Vector3 playerPos = player.transform.position;

        // --- LOOK AHEAD ---
        float xInput = Input.GetAxisRaw("Horizontal");
        float targetLookAhead = xInput * lookAheadDistance;

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVel,
            lookAheadReturnSpeed
        );

        // --- DEADZONE ---
        Vector2 halfDead = deadzoneSize * 0.5f;
        Rect deadzone = new Rect(
            camPos.x - halfDead.x,
            camPos.y - halfDead.y,
            deadzoneSize.x,
            deadzoneSize.y
        );

        float targetX = camPos.x;
        float targetY = camPos.y;

        if (playerPos.x < deadzone.xMin)
            targetX = playerPos.x + halfDead.x;
        else if (playerPos.x > deadzone.xMax)
            targetX = playerPos.x - halfDead.x;

        if (playerPos.y < deadzone.yMin)
            targetY = playerPos.y + halfDead.y;
        else if (playerPos.y > deadzone.yMax)
            targetY = playerPos.y - halfDead.y;

        // --- APLICAR LOOK AHEAD ---
        targetX += currentLookAhead;

        // --- SUAVIZADO ---
        float newX = Mathf.SmoothDamp(camPos.x, targetX, ref velX, smoothX);
        float newY = Mathf.SmoothDamp(camPos.y, targetY, ref velY, smoothY);

        Vector3 finalPos = new Vector3(newX, newY, offset.z);

        // --- LÍMITES ---
        finalPos.x = Mathf.Clamp(finalPos.x, minX, maxX);
        finalPos.y = Mathf.Clamp(finalPos.y, minY, maxY);

        transform.position = finalPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 pos = transform.position;
        Gizmos.DrawWireCube(pos, new Vector3(deadzoneSize.x, deadzoneSize.y, 0.1f));
    }
}
