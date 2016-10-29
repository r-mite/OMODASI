using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    private static GameObject[] startButtonList = new GameObject[5];
    private static string[] buttonName =
    {
        "はじめから",
        "途中から",
        "エンドレス",
        "設定",
        "終了"
    };
    private static bool moveFirstFlag;
    private static bool visualFirst;
    private static int moveCounter;

    private static GameObject stageSelect;
    //private static GameObject[] stageSelectList = new GameObject[4];
    private static string[] selectList =
    {
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
    };
    private const int upCount = 20;
    private const int upPosition = 1980;
    
    private static bool moveStageFlag;
    private static bool visualStage;

    private static GameObject setting;
    private static bool moveSettingFlag;
    private static bool visualSetting;

    private static bool afterEnter;

    // Use this for initialization
    void Start () {

        Music.Init(1);

        GameObject obj = Resources.Load<GameObject>("Prefub/StartButton");
        for(int i=0; i<5; i++)
        {
            GameObject go = Instantiate<GameObject>(obj);
            go.transform.SetParent(transform, false);
            go.transform.Translate(new Vector3(0, -100 * i, 0));
            go.transform.FindChild("Text").GetComponent<Text>().text = buttonName[i];
            go.GetComponent<StartButton>().Init(i);
            startButtonList[i] = go;
        }

        obj = Resources.Load<GameObject>("Prefub/StageSelect");
        stageSelect = Instantiate<GameObject>(obj);
        stageSelect.transform.SetParent(transform, false);
        stageSelect.transform.Translate(new Vector3(0, upPosition, 0));
        for (int i=0; i<4; i++)
        {
            stageSelect.transform.FindChild(selectList[i]).GetComponent<StageSelect>().Init(i);
        }
        
        obj = Resources.Load<GameObject>("Prefub/Setting");
        setting = Instantiate<GameObject>(obj);
        setting.transform.SetParent(transform, false);
        setting.transform.Translate(new Vector3(0, upPosition, 0));
        int spd, vol;
        ReadWrite.ReadSetting(out spd, out vol);
        setting.transform.FindChild("BV").FindChild("BVS").GetComponent<Slider>().minValue = 0;
        setting.transform.FindChild("BV").FindChild("BVS").GetComponent<Slider>().maxValue = 3;
        setting.transform.FindChild("BV").FindChild("BVS").GetComponent<Slider>().value = spd;
        //コールバック用
        //setting.transform.FindChild("BS").FindChild("BSS").GetComponent<Slider>().onValueChanged.AddListener(x => ReadWrite.RewriteSetting((int)x, -1));
        //setting.transform.FindChild("BV").FindChild("BVS").GetComponent<Slider>().onValueChanged.AddListener(x => ReadWrite.RewriteSetting(-1, (int)x));

        afterEnter = false;

    }
	
	// Update is called once per frame
	void Update () {

        if (moveStageFlag)
        {
            Vector3 vec = visualStage ? new Vector3(0, -upPosition / upCount, 0) : new Vector3(0, upPosition / upCount, 0);
            stageSelect.transform.Translate(vec);
            /*
            for (int i = 0; i < 4; i++)
            {
                stageSelectList[i].transform.Translate(vec);
            }
            */
        }

        if (moveSettingFlag)
        {
            Vector3 vec = visualSetting ? new Vector3(0, -upPosition / upCount, 0) : new Vector3(0, upPosition / upCount, 0);
            setting.transform.Translate(vec);
        }

        if (moveFirstFlag)
        {
            Vector3 vec = visualFirst ? new Vector3(-250/upCount, 0, 0) : new Vector3(250/upCount, 0, 0);
            for (int i = 0; i < 5; i++)
            {
                startButtonList[i].transform.Translate(vec);
            }
            moveCounter++;
            if (moveCounter >= upCount)
            {
                moveStageFlag = false;
                moveFirstFlag = false;
                moveSettingFlag = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (CheckAfterEnter()) return;
            BackMenu();
        }

    }

    private static void MoveFirstMenu(bool vis)
    {
        moveFirstFlag = true;
        visualFirst = vis;
        for (int i = 0; i < 5; i++)
        {
            startButtonList[i].GetComponent<StartButton>().ChangeVisual(vis);
        }
        moveCounter = 0;
        BackPanel.VisBackPanel(!vis);
    }

    public static void MoveStageMenu(bool vis)
    {
        moveStageFlag = true;
        visualStage = vis;
        MoveFirstMenu(!vis);
    }

    public static void MoveSettingMenu(bool vis)
    {
        moveSettingFlag = true;
        visualSetting = vis;
        MoveFirstMenu(!vis);
        if (!vis)
        {
            //int spd = (int)setting.transform.FindChild("BS").FindChild("BSS").GetComponent<Slider>().value;
            int spd = (int)setting.transform.FindChild("BV").FindChild("BVS").GetComponent<Slider>().value;
            int vol = 50;
            ReadWrite.RewriteSetting(spd, vol);
        }
    }

    //スタートメニューに戻す
    public static void BackMenu()
    {
        //メニューが動いてるときは操作無効
        if (moveSettingFlag) return;
        if (visualSetting)
        {
            Music.PlaySE(Music.Clip.Mekuri);
            MoveSettingMenu(false);
        }
        if (visualStage)
        {
            Music.PlaySE(Music.Clip.Mekuri);
            MoveStageMenu(false);
        }
    }

    //動いてる途中か否か
    public static bool CheckMoving()
    {
        return moveSettingFlag;
    }

    //セレクトした後の動作停止
    public static void OnAfterEnter()
    {
        afterEnter = true;
    }

    public static bool CheckAfterEnter()
    {
        return afterEnter;
    }

}
