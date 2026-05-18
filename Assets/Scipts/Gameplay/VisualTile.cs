using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualTile : MonoBehaviour
{

    private int row, col;
    public int Row => row;
    public int Col => col;

    public int index;
    private SpriteRenderer spriteRenderer;

    private Tween hintTween;
    private void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Tile thiếu spriterender", gameObject);
        }
    }

    // Start is called before the first frame update


    public void SetSkin(TileData skin)
    {
        if (skin == null && index < 0 && index >= skin.tileSprites.Length) return;

        spriteRenderer.sprite = skin.tileSprites[index];
        transform.localScale = Vector2.one * skin.scaleMultiplier;
    }
    public void SetPostionGrid(int x, int Y)
    {
        this.col = x;
        this.row = Y;
    }

    public void PlayHintAnimation()
    {
        if (spriteRenderer == null) return;

        // Dừng các hiệu ứng nhấp nháy cũ đang chạy (nếu có) để tránh bị chồng chéo
        StopHintAnimation();

        // Đảm bảo màu gốc ban đầu có Alpha là 1 (Đậm nhất)
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        // Kích hoạt nhấp nháy: 
        hintTween = spriteRenderer.DOFade(0.3f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    // --- HÀM TẮT HIỆU ỨNG (Khi người chơi click chọn hoặc bàn chơi đổi trạng thái) ---
    public void StopHintAnimation()
    {
        if (hintTween != null && hintTween.IsActive())
        {
            hintTween.Kill(); // Xóa bỏ hoàn toàn hiệu ứng nhấp nháy ngầm
        }

        // Trả lại độ mờ 100% nguyên bản cho ô cờ
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }
    }

}
