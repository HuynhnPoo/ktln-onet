
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonBase : MonoBehaviour
{

    protected Button button;

    public void LoadCompoment()
    {
        if (this.button == null)
            this.button = GetComponent<Button>();
    }

    protected virtual void Awake()
    {
    }
    protected virtual void OnEnable()
    {
        this.LoadCompoment();
    }

    protected virtual void Start()
    {

        this.AddEventListener();

    }
    public virtual void AddEventListener()
    {

        this.button.onClick.AddListener(this.OnClick);
    }
    public abstract void OnClick();

    /* protected virtual void CloseKeyboard()
     {
 #if UNITY_WEBGL && !UNITY_EDITOR
         HideInputField();
 #endif
     }*/

}
