using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

    private enum Click { Left, Right, Wheel}
    private static Vector2 position = new Vector2();
    public static Vector2 transPosition = new Vector2();
    private static int screenWidth;

	// Use this for initialization
	void Start () {

        screenWidth = Screen.width;

	}
	
	// Update is called once per frame
	void Update () {

        Vector2 vec = Input.mousePosition;

        //ホイール押下中のみカメラ移動
        if (Input.GetMouseButton((int)Click.Wheel))
        {
            transPosition = vec - position;
            GameCamera.TrackMouse();
        }
        if(Input.GetMouseButtonUp((int)Click.Wheel))
        {
            GameCamera.TrackStop();
        }

        //右クリック
        if (Input.GetMouseButtonDown((int)Click.Right))
        {
            if (!CheckMouseOutMenu())
            {
                //メニュー上ならコマンドをソート
                Menu.SortMenu();
            }else
            {
                if (TurnAndPhase.OnField())
                {
                    if (GameCamera.DoTrackPlayer())
                    {
                        //コマンド中ならキャンセル
                        TurnAndPhase.CancelAction();
                    }
                    else
                    {
                        GameCamera.TrackPlayer();
                    }
                }
                else
                {
                    //それ以外ならカメラをプレイヤーに返す
                    GameCamera.TrackPlayer();
                }
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(CheckMouseOutMenu())
        {
            GameCamera.FrontBehind(scroll);
        }
            

        position = vec;

    }

    //マウスがコマンドメニュー上にいないか
    public static bool CheckMouseOutMenu()
    {
        if (Input.mousePosition.x < screenWidth * 3 / 4)
        {
            return true;
        }
        return false;
    }

}
