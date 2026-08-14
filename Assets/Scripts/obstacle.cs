using UnityEngine;

public class obstacle : MonoBehaviour
{
    private Rigidbody2D rb;
    private float rotationVelocity;

    void Start()
    {
        float tamanhometeoro = Random.Range(0.5f, 2f);
        transform.localScale = new Vector3(tamanhometeoro, tamanhometeoro, 1);
        rotationVelocity = Random.Range(-50f, 50f);
        rb = GetComponent<Rigidbody2D>();
        Vector2 direcaoAleatoria = Random.insideUnitCircle;
        rb.AddForce(direcaoAleatoria * 5f, ForceMode2D.Impulse);

        float torqueAleatorio = Random.Range(-10f, 10f);
        rb.AddTorque(torqueAleatorio);
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationVelocity * Time.deltaTime);
    }
}
