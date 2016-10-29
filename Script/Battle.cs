using UnityEngine;
using System.Collections;

public class Battle : MonoBehaviour {

    //ゲーム操作関連
    private static Object preCursor;
    private static GameObject cursor;

    //フィールド関連
    private static Object preFloor;
    private static GameObject floor;

    //シーンキー
    private const int KEY = 0;

    private static float timer;
    private const float firstWait = 1.0f;
    private static bool firstAnim;
    private const float lastWait = 4.0f;
    private static bool lastAnim;

    private static int win;

	// Use this for initialization
	void Start () {

        ReadWrite.SceneReader(KEY);
        Log.InitGameID(KEY);
        Unit.Init(KEY);

        SummonCursor();
        SummonFloor();

        TurnAndPhase.InitVariable();
        GameCamera.SetCamera();
        Music.Init(0);

        timer = 0;
        firstAnim = false;
        lastAnim = false;

    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if(!firstAnim && timer > firstWait)
        {
            firstAnim = true;
            Unit.StartAnim();
        }
        if (lastAnim && timer > lastWait)
        {
            lastAnim = false;
            //事後処理
            Advent.StartAdventure(KEY, win);
        }

    }

    //カーソル設置
    private static void SummonCursor()
    {
        /*
        preCursor = Resources.Load("Prefub/CursorF");
        cursor = (GameObject)Instantiate(preCursor);
        cursor.name = "CursorF";
        */    
        Menu.SetMenu();
    }

    //フロアー設置
    private static void SummonFloor()
    {
        preFloor = Resources.Load("Prefub/Floor");
        floor = (GameObject)Instantiate(preFloor, new Vector3(), Quaternion.identity);
        floor.name = "Floor";
    }

    //ゲーム終了
    public static void EndGame(int _win)
    {
        lastAnim = true;
        win = _win;
        timer = 0;
    }

}
