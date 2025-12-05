using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1f; // İlerleme hızı
    private Transform targetHome;
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f);
    public bool doesRotate = false;
    void Start()
    {
        // Sahnedeki "Home" etiketli objeyi bul
        GameObject home = GameObject.FindGameObjectWithTag("Home");
        if (home != null)
            targetHome = home.transform;
    }

    void Update()
    {
        if (doesRotate)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
        if (targetHome != null)
        {
            // Eve doğru sabit hızla ilerle
            float step = speed * Time.deltaTime;
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

            // Alternatif: Sadece aşağı inmesini istiyorsan (Klasik Space Invaders gibi)
            // transform.Translate(Vector3.down * step);
        }
    }
}