using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{

    [SerializeField] private ActiveSectionButton[] button;
    [SerializeField] private GameObject[] panelShop;
    // Start is called before the first frame update

    public void ShowUI(int index)
    {
           
        for (int i = 0;i< panelShop.Length ; i++)
        {

            bool isAcitve = (i == index);
            panelShop[i].SetActive(isAcitve);
        }
        for (int j = 0;j< button.Length ; j++) 
        {
            bool isAcitve = (j == index);
            button[j].SetInteractable (!isAcitve);
        }

    }
}
