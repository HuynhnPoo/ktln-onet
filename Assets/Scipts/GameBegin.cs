using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBegin : MonoBehaviour
{

    public UIManager.SceneType sceneType;

    [SerializeField] private RectTransform[] transformUI;
    [SerializeField] private RectTransform canvasRect;
    Vector2 startPos;
    float time = 2.5f;

    private void OnEnable()
    {
        float screenWidth = canvasRect.rect.width;


        switch (sceneType)
        {
            case UIManager.SceneType.MAINMENU:

                // Tạo vị trí xuất phát ở rìa phải màn hình
                startPos = new Vector2(screenWidth, 0);

                transformUI[0].ShowPopup(time);
                transformUI[1].MoveUI(startPos, time);
                break;
            case UIManager.SceneType.FORM:
            case UIManager.SceneType.ONLINEMAINMENU:
            case UIManager.SceneType.LEVELONLINEMANAGER:
            case UIManager.SceneType.LEVELMANAGER:
            case UIManager.SceneType.GAMEOFFLINE:

                startPos = new Vector2(-screenWidth, 0);

                transformUI[0].ShowPopup(time);
                transformUI[1].MoveUI(startPos, time);
                transformUI[2].MoveUI(-startPos, time);
                break;
            case UIManager.SceneType.GAMEONLINE:
                float screenHeigh = canvasRect.rect.height;
                startPos = new Vector2(-screenHeigh, 0);
               Vector2 startPosY = new Vector2(-screenHeigh, 0);

                transformUI[0].ShowPopup(time);
                transformUI[1].MoveUI(startPos, time);
                transformUI[2].MoveUI(-startPos, time);
                transformUI[3].MoveUI(startPosY, time);
                break;
        }



    }
}
