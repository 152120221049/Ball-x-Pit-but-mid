using UnityEngine;

public class HomeBase : MonoBehaviour
{
    public PlayerHealth pluh;
    void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            if (pluh != null)
            {
                pluh.TakeDamage(1f);
            }

           
            Destroy(collision.gameObject);
        }
    }
}