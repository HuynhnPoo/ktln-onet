using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSlider : SliderBase
{
    float currentSFX = 0.7f;
    protected override void OnChange(float amount)
    {
        currentSFX = SoundManager.Instance.SetSFXGame(amount);
        slider.value = currentSFX;
    }


    protected override void Start()
    {
        base.Start();

        slider.value = SoundManager.Instance.SetSFXGame(currentSFX);
    }

}
