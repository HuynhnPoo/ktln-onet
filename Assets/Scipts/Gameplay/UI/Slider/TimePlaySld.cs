using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlaySld : SliderBase
{
   
  //  float currentValue = 1f;

    // Start is called before the first frame update
    protected override void OnChange(float amount)
    {
      
    }

    protected override void Start()
    {
        base.Start();

        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = GameMechanics.CountDown();
    }


    // Update is called once per frame
    void Update()
    {
        
       slider.value = GameMechanics.GetTimeRatio();

    }
}
