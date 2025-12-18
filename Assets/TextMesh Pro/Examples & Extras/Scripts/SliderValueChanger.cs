using UnityEngine;
using TMPro; // or using UnityEngine.UI; for legacy Text
using UnityEngine.UI;
public class SliderValueDisplay : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI valueText; 

    void Start()
    {
       
        volumeSlider.onValueChanged.AddListener(UpdateText);

        
        UpdateText(volumeSlider.value);
    }

    void UpdateText(float value)
    {
        
        valueText.text = (Mathf.RoundToInt(value * 100)).ToString() ;

        
    }
}