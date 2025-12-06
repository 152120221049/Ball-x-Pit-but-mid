using UnityEngine;
using TMPro;
using DG.Tweening; 

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount, bool isCritical)
    {
 
        textMesh.text = Mathf.CeilToInt(damageAmount).ToString();

      
        if (isCritical)
        {
            textMesh.fontSize = 6;
            textMesh.color = new Color(1f, 0.5f, 0f, 1f);
        }
        else
        {
            textMesh.fontSize = 3;
            textMesh.color = new Color(1f, 0.92f, 0.016f, 1f); // Sarı
        }

      
        transform.DOMoveY(transform.position.y + 2f, 1f).SetEase(Ease.OutQuad);

   
        textMesh.DOFade(0f, 1f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => Destroy(gameObject));

      
        transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);

        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
    }
}