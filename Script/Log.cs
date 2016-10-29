using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Log : MonoBehaviour {

    private static Text textComponent;
    private static Image imageComponent;
    private static Text clearComponent;
    private static Image lastComponent;
    private static Text turnComponent;
    private static Image turnBackComponent;
    private static Image homeComponent;
    private static string[] logText =
    {
        "プレイヤーのターン",
        "エネミーのターン",
        "プレイヤーの攻撃",
        "エネミーの攻撃",
        "プレイヤーはバフをかけた",
        "エネミーにデバフをかけた",
        "プレイヤーは気絶した",
        "エネミーは全滅した",
        "エネミーが現れた"
    };
    private static string[] clearText =
    {
        "ガメオベア",
        "ゲームクリアー",
    };
    private static Color[] backCol =
    {
        new Color(0, 0, 0, 147/255f),
        new Color(1, 248/255f, 0, 147/255f),
    };
    private static Color[] frontCol =
    {
        new Color(1, 0 ,0),
        new Color(0, 0, 0),
    };
    private static float timer;
    private const float showTime = 3.0f;
    private static bool showFlag;
    private static float endTimer;
    private const float endTime = 1f;
    private static bool ending;
    private static int endPattern;

    private static int gameID;

	// Use this for initialization
	void Start () {

        textComponent = transform.FindChild("Text").GetComponent<Text>();
        imageComponent = transform.GetComponent<Image>();
        GameObject last = GameObject.Find("Last");
        clearComponent = last.transform.FindChild("Text").GetComponent<Text>();
        lastComponent = last.GetComponent<Image>();
        showFlag = false;

        turnBackComponent = transform.root.gameObject.transform.FindChild("Turn").FindChild("TurnWin").GetComponent<Image>();
        turnComponent = transform.root.gameObject.transform.FindChild("Turn").FindChild("TurnWin").FindChild("Text").GetComponent<Text>();
        homeComponent = transform.root.gameObject.transform.FindChild("Turn").FindChild("Home").GetComponent<Image>();
        NonDisplayTurn();

    }
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;
        if (showFlag)
        {
            timer += delta;
            if (timer > showTime)
            {
                VisibleObject(false);
            }
        }
        if (ending)
        {
            endTimer += delta;
            if (endTimer > endTime)
            {
                VisibleLast(endPattern);
                switch (gameID)
                {
                    case 0:
                        Battle1.EndGame(endPattern);
                        break;
                    case 1:
                        Battle2.EndGame(endPattern);
                        break;
                    case 2:
                        Battle3.EndGame(endPattern);
                        break;
                    case 3:
                        Battle4.EndGame(endPattern);
                        break;
                    case 4:
                        Battle5.EndGame(endPattern);
                        break;
                }
            }
        }
	
	}

    //ゲーム番号登録
    public static void InitGameID(int key)
    {
        gameID = key;
    }

    //テキストの表示
    public static void ShowLog(int num)
    {
        textComponent.text = logText[num];
        timer = 0;
        VisibleObject(true);

        //文字表示だけは更新しておく
        turnComponent.text = TurnAndPhase.CheckEndlessTurn() + "ターン";
    }

    //オブジェクトの可視化
    public static void VisibleObject(bool flag)
    {
        showFlag = flag;
        imageComponent.enabled = flag;
        textComponent.enabled = flag;
    }

    //ゲーム終了表示
    private static void VisibleLast(int win)
    {
        ending = false;
        clearComponent.text = clearText[win];
        lastComponent.color = backCol[win];
        lastComponent.enabled = true;
        clearComponent.color = frontCol[win];
        clearComponent.enabled = true;
        Music.StopBGM();
        if (win == 0)
        {
            Music.PlaySE(Music.Clip.Lose);
        }
        else
        {
            Music.PlaySE(Music.Clip.Win);
        }
    }

    //ゲーム終了宣言
    public static void EndDeclaration(int win)
    {
        ending = true;
        endPattern = win;
        endTimer = 0;
    }


    //エンドレスモード時のターン表示
    public static void DisplayTurn()
    {
        turnComponent.text = TurnAndPhase.CheckEndlessTurn() + "ターン";
        turnComponent.enabled = true;
        turnBackComponent.enabled = true;
        homeComponent.enabled = true;
    }

    public static void NonDisplayTurn()
    {
        turnComponent.enabled = false;
        turnBackComponent.enabled = false;
        homeComponent.enabled = false;
    }

}
