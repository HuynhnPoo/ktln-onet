using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // 🛠️ THÊM VÀO ĐÂY: Tự động đổi màu ngay khi LineController được kích hoạt (vào màn chơi)
    private void OnEnable()
    {
        ApplyCurrentSkin();
    }

    // ⭐ ĐẶT HÀM ĐỔI MÀU CỦA BẠN VÀO ĐÂY ⭐
    public void ApplyCurrentSkin()
    {
        if (PlayFabDataManager.Instance != null && PlayFabDataManager.Instance.playerData != null)
        {
            string currentLineId = PlayFabDataManager.Instance.playerData.currentLineId;
            Debug.Log("hien thi mau sac đã chọn"+currentLineId);

            switch (currentLineId)
            {
                case "line_default":
                    ChangeLineColor(Color.white);
                    break;
                case "red_line":
                    ChangeLineColor(Color.red);
                    break;
                case "blue_line":
                    ChangeLineColor(Color.cyan);
                    break;
                case "yellow_line":
                    ChangeLineColor(Color.yellow);
                    break;
                case "green_line":
                    ChangeLineColor(Color.green);
                    break;
                default:
                    ChangeLineColor(Color.white);
                    break;
            }

            Debug.Log($"[LineController] Đã đổi màu theo ID: {currentLineId}");
        }
    }

    public void DrawPath(Vector3[] points)
    {
        // 🛠️ THÊM VÀO ĐÂY: Trước khi vẽ, ép nó kiểm tra màu lại lần nữa cho chắc chắn
        ApplyCurrentSkin();

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        CancelInvoke(nameof(ClearLine));
        Invoke(nameof(ClearLine), 0.5f);
    }

    public void ClearLine()
    {
        lineRenderer.positionCount = 0;
    }

    public void ChangeLineColor(Color newColor)
    {
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
    }
}