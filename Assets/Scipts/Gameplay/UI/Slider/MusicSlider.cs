using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSlider : SliderBase
{
    float currentVolume = 0.7f;

    // Start is called before the first frame update
    protected override void OnChange(float amount)
    {
        //throw new System.NotImplementedException();
        currentVolume = SoundManager.Instance.SetMusicGame(amount);
        slider.value = currentVolume;

       // PlayerPrefs.SetFloat(StringManager.musicSave, currentVolume); // lưu lại giấ trị sửa đổi music
    }

    protected override void Start()
    {
        base.Start();

      //  currentVolume = PlayerPrefs.GetFloat(StringManager.musicSave, 0.7f);
        slider.value = SoundManager.Instance.SetMusicGame(currentVolume);
    }


    // Update is called once per frame
    void Update()
    {
        if (SoundManager.Instance.IsResseted)
        {
            slider.value = SoundManager.Instance.SetMusicGame(0.7f); //set laij gias tri cho slider và am thanh
            //PlayerPrefs.SetFloat(StringManager.musicSave, currentVolume);

            //SoundManager.Instance.IsResseted = false;    
        }
    }
}
