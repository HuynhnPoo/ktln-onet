using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeUpBoost", menuName = "Boosts/TimeUp")]
public class TimeUpBoost : BoostBase
{
    protected override bool CanExecute(GridManager grid)
    {
        // Điều kiện riêng của Bomb: Bàn chơi phải còn vật phẩm để nổ
        return grid != null; // Bạn có thể thêm logic grid.GetTotalItems() > 0 ở đây
    }

    protected override void Execute(GridManager grid)
    {
        Debug.Log($"[PlayFab Synced] Đã trừ 1 {itemId} thành công! Time UP!");
        // Viết code xử lý nổ các ô trên Grid của bạn tại đây
      //  if (GameMechanics.CountDown() > (GameMechanics.GetMaxTime() / 2))
            GameMechanics.AddTime(10); // 10 laf gia trị thêm của time 
    }
}
