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

    }

    

    private void OnToggleChanged(bool isOn)
    {
        foreach (InputBase input in input) 
        {
            if (input == null) return;
            input.SetPassVisibility(isOn);
        }
      
    }
    // Start is called before the first frame update
    void Start()
    {
     toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
