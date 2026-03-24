using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
public abstract class InputBase : MonoBehaviour, ICompoment, IPointerClickHandler
{
    [SerializeField] protected TMP_InputField input;
   

    public void LoadCompoment()
    {
        if (input == null) input = GetComponent<TMP_InputField>();
    }

    private void Awake()
    {
        this.LoadCompoment();
        
    }

    private void Start()
    {
        this.AddEventListener();
    }

    protected virtual void AddEventListener()
    {
        this.input.onEndEdit.AddListener(this.OnEndEdit);
    }

    // Abstract: class con phải implement
    protected abstract void OnEndEdit(string text);

    // ========== Các hàm Unity nhận từ JS ==========

    // Nhận khi user nhập text
    public void OnKeyboardValueChanged(string val)
    {
        if (input == null) return;

        input.text = val;
        input.caretPosition = input.text.Length;
        input.ForceLabelUpdate();
    }

    // Nhận khi user nhấn Enter trên bàn phím ảo
    public void OnEndEditFromJs(string text)
    {
        input.text = text;
        this.OnEndEdit(text);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (input != null && !input.isFocused) input.Select();
    }
}