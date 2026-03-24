
using UnityEngine;
using UnityEngine.UI;

public abstract class SliderBase : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        this.AddChangedEvent();
    }

    protected virtual void AddChangedEvent()
    {
        this.slider.onValueChanged.AddListener(this.OnChange);// thực hien các logic khi thay đổi sliders
    }

    protected abstract void OnChange(float amount);
}
