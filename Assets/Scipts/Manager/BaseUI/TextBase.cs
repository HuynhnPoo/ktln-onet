
using TMPro;
using UnityEngine;

public abstract class TextBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;

    public void LoadCompoment()
    {
        if (text == null)
            this.text = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Awake()
    {
        this.LoadCompoment();

    }
    protected virtual void Update()
    {
        this.PrintText();
    }
    protected abstract void PrintText();

}
