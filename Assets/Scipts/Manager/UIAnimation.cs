using DG.Tweening;
using UnityEngine;

public static class UIAnimation
{
    // ================= SCALE =================

    // Hiển thị Popup
    public static Tween ShowPopup(this RectTransform rect, float time = 0.5f, Ease ease = Ease.OutBack)
    {
        // Dừng mọi Tween đang chạy trên object này để tránh lỗi
        
        rect.localScale = Vector3.zero;

        // Thêm SetDelay nhẹ để tránh xung đột với Layout Group của Unity
        return rect.DOScale(Vector3.one, time)
            .SetEase(ease)
            .SetUpdate(true)
            .SetDelay(0.5f);
    }

    // Ẩn Popup
    public static Tween HidePopup(this RectTransform rect, float time = 0.3f, Ease ease = Ease.InBounce)
    {
        rect.DOKill();

        return rect.DOScale(Vector3.zero, time)
            .SetEase(ease)
            .SetUpdate(true)
            .SetDelay(0.5f) ;
    }

    // ================= MOVE =================

    // Di chuyển UI đến vị trí đích
    public static Tween MoveUI(this RectTransform rect, Vector2 targetPos, float duration = 0.3f, Ease ease = Ease.OutCubic)
    {
        rect.DOKill();

        return rect.DOAnchorPos(rect.anchoredPosition, duration)
            .From(targetPos)
            .SetEase(ease)
            .SetUpdate(true);
    }

    // Di chuyển từ điểm A đến điểm B
    public static Tween MoveFromUI(this RectTransform rect, Vector2 startPos, Vector2 endPos, float duration = 0.3f, Ease ease = Ease.OutCubic)
    {
        rect.DOKill();
        rect.anchoredPosition = startPos;

        return rect.DOAnchorPos(endPos, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetDelay(0.01f);
    }
}