using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform player;
    public float parallaxMultiplier = 0.5f;

    private Transform leftPart;
    private Transform rightPart;

    private float spriteWidth;

    private Vector3 lastPlayerPos;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        // Los dos hijos
        leftPart = transform.GetChild(0);
        rightPart = transform.GetChild(1);

        // Ancho del sprite
        SpriteRenderer sr = leftPart.GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;

        lastPlayerPos = player.position;
    }

    void LateUpdate()
    {
        Vector3 delta = player.position - lastPlayerPos;

        // Movimiento parallax
        transform.position += new Vector3(delta.x * parallaxMultiplier, 0, 0);

        // Loop infinito
        if (player.position.x - leftPart.position.x > spriteWidth)
            MoveLeftToRight();

        if (rightPart.position.x - player.position.x > spriteWidth)
            MoveRightToLeft();

        lastPlayerPos = player.position;
    }

    void MoveLeftToRight()
    {
        leftPart.position = new Vector3(
            rightPart.position.x + spriteWidth,
            leftPart.position.y,
            leftPart.position.z
        );

        // Intercambiar referencias
        var temp = leftPart;
        leftPart = rightPart;
        rightPart = temp;
    }

    void MoveRightToLeft()
    {
        rightPart.position = new Vector3(
            leftPart.position.x - spriteWidth,
            rightPart.position.y,
            rightPart.position.z
        );

        // Intercambiar referencias
        var temp = leftPart;
        leftPart = rightPart;
        rightPart = temp;
    }
}
