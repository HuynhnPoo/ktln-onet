using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSectionButton : ButtonBase
{
    public int indexSection;
    [SerializeField] private ShopUIManager shopUI;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (shopUI == null)
        {
            shopUI=GetComponentInParent<ShopUIManager>();
        }
    }

    protected override void Start()
    {
        base.Start();
        if (indexSection == 0)
        {
            shopUI.ShowUI(0);
        }
    }


    public override void OnClick()
    {
        shopUI.ShowUI(indexSection);

    }
    public void SetInteractable(bool value)
    {
        //if(button==null)
        //button = GetComponent<Button>();
        //else
        button.interactable = value;
    }
}
