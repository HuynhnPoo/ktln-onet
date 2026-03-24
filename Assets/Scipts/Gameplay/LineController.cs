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

    public void DrawPath(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        // Tự động xóa line sau 0.5 giây
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
