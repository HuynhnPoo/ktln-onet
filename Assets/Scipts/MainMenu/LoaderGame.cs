using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderGame : MonoBehaviour
{
 
    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.GetInt(StringManager.hasPlayed) ==1)
        {
            GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(0).GetChild(1).gameObject;
            // Debug.Log(objBtn);
            objBtn.SetActive(true);

            Debug.Log("thuc hien vao menu");
        }
       
    }
}
