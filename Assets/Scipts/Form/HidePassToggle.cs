using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HidePassToggle : MonoBehaviour
{
    Toggle toggle;
   [SerializeField]private InputBase[] input;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = false;


        // Đăng ký sự kiện
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }



    private void OnToggleChanged(bool isOn)
    {
        foreach (InputBase input in input) 
        {
            if (input == null) return;
            input.SetPassVisibility(isOn); // thực hiện chuyển input sâng pass hay text
        }
      
    }
}
