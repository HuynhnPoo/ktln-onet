using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGlobalSld : SliderBase
{
    protected override void OnChange(float amount)
    {
        
    }


    protected override void Start()
    {
        base.Start();

        slider.minValue = 0;
        slider.maxValue = 120;
        slider.value = 120;
     
    }


    // Update is called once per frame
    void Update()
    {

        // chỉ kich haotj khi game bắt đầu

        Debug.Log(slider.value + " " + PhotonManager.Instance.OnlineMatchManager.TimeGlobal);
        slider.value = PhotonManager.Instance.OnlineMatchManager.TimeGlobal;

    }
}
