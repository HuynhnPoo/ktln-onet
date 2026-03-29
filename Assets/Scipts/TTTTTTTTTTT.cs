using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTTTTTTTTT : MonoBehaviour
{
   public Button button;
   
    // Start is called before the first frame update
     void Start()
    {
     button=GetComponent<Button>();

      //  button.interactable=false;
      //  button.enabled=false;

        button.onClick.AddListener(() => Debug.Log("hhhhh") );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
