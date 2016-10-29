using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

    private static Vector3 relative = new Vector3(0, 4, -3);
    private static Vector3 relativeStairs = new Vector3(0, 1, -1);
    private static int stairsNum;
    private static int lowerLimit = -2;
    private static int upperLimit = 7;
    private static GameObject player;
    private static int moveMode;
    private enum MoveMode { Stop, Player, Mouse, Enemy, Dig};
    private const float playerSpeed = 4.0f;
    private const float mouseSpeed = 0.8f;
    public static float slope;
    private static Vector3 stopPosition;
    private static GameObject backGround;

	// Use this for initialization
	void Start () {

        stairsNum = 0;
        moveMode = (int)MoveMode.Stop;
        slope = transform.rotation.x;
        backGround = GameObject.Find("BackGround");

	}
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;
        Vector3 v;
        switch (moveMode)
        {
            case (int)MoveMode.Stop:
                if(stopPosition == new Vector3())
                {
                    stopPosition = transform.position - relative - relativeStairs * stairsNum;
                }
                v = stopPosition + relative + relativeStairs * stairsNum - transform.position;
                v = Parameter.ConvertToCamera(v);
                transform.Translate(v * delta * playerSpeed);
                break;
            case (int)MoveMode.Player:
                v = player.transform.position - transform.position + relative + relativeStairs * stairsNum;
                v = Parameter.ConvertToCamera(v);
                transform.Translate(v * delta * playerSpeed);
                break;
            case (int)MoveMode.Mouse:
                v = Parameter.ConvertToCamera(Mouse.transPosition);
                transform.Translate(v * delta * mouseSpeed);
                break;
            case (int)MoveMode.Enemy:
                v = Unit.GetActionEnemyVec3() - transform.position + relative + relativeStairs * stairsNum;
                v = Parameter.ConvertToCamera(v);
                transform.Translate(v * delta * playerSpeed);
                break;
            case (int)MoveMode.Dig:
                Vector3 vec = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                v = vec - transform.position + relative + relativeStairs * stairsNum;
                v = Parameter.ConvertToCamera(v);
                transform.Translate(v * delta * playerSpeed);
                break;
        }
        
        backGround.transform.position = new Vector3(transform.position.x, -1, transform.position.z);

	}

    //初期設定
    public static void SetCamera()
    {
        player = GameObject.Find("Player");
        moveMode = (int)MoveMode.Player;
    }

    //追従停止
    public static void TrackStop()
    {
        moveMode = (int)MoveMode.Stop;
        stopPosition = new Vector3();
    }

    //プレイヤー追従
    public static void TrackPlayer()
    {
        moveMode = (int)MoveMode.Player;
    }

    //マウス追従
    public static void TrackMouse()
    {
        moveMode = (int)MoveMode.Mouse;
    }

    //敵追従
    public static void TrackEnemy()
    {
        moveMode = (int)MoveMode.Enemy;
    }

    //瞬間移動追跡
    public static void TrackDig()
    {
        moveMode = (int)MoveMode.Dig;
    }

    //カメラ動作確認
    public static bool DoTrackPlayer()
    {
        if(moveMode == (int)MoveMode.Player)
        {
            return true;
        }
        return false;
    }

    //ホイールで前後調整
    public static void FrontBehind(float scroll)
    {
        if(scroll == 0)
        {
            return;
        }else if(scroll > 0)
        {
            stairsNum--;
            stairsNum = Mathf.Max(stairsNum, lowerLimit);
        }else
        {
            stairsNum++;
            stairsNum = Mathf.Min(stairsNum, upperLimit);
        }
    }
}
