using UnityEngine;
using DG.Tweening; 

public class PauseMenuAnim : MonoBehaviour
{
    
    void OnEnable()
    {
     
        transform.localScale = Vector3.zero;

        
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}