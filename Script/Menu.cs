using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    private static int commandNum;
    public static int handMax;
    private static Object command;
    private static List<GameObject> commandList;
    private static GameObject hand;
    private static int choiceNum;
    public static string[] imageName =
    {
        "Image/end",
        "Image/atk",
        "Image/move",
        "Image/buff",
        "Image/debuff",
        "Image/extra"
    };
    private static int sortNum;

    // Use this for initialization
    void Start () {

        InitParameter();
        command = Resources.Load("Prefub/Command");
        commandList = new List<GameObject>();
        hand = GameObject.Find("Hand");
        choiceNum = 0;
        sortNum = 0;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //試合ごとに変数をリセットだ
    private static void InitParameter()
    {
        commandNum = 0;
        handMax = 0;
    }

    //ターンごとにコマンドを増やすよ
    public static void AddCommand(int _type, int _var)
    {
        GameObject obj = (GameObject)Instantiate(command, new Vector3(0, 0, 0), Quaternion.identity);
        obj.transform.SetParent(hand.transform, false);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.transform.FindChild("Text").gameObject.GetComponent<Text>().text = ReadWrite.CallCommandName(_type, _var);
        obj.transform.GetComponent<Command>().SetNum(commandNum, _type, _var);
        if(_type == (int)TurnAndPhase.Type.TurnEnd)
        {
            obj.transform.SetSiblingIndex(0);
        }
        commandList.Add(obj);
        commandNum++;
        handMax++;
    }

    //選択しているコマンドに着色
    public static void ChooseCommand(int num)
    {
        commandList[choiceNum].transform.GetComponent<Image>().color = Color.blue;
        commandList[num].transform.GetComponent<Image>().color = Color.red;
        choiceNum = num;
    }

    //選べる範囲でコマンドを選択
    public static void ChangeHand(out int cursor, int now, int pm)
    {
        cursor = now + pm;
        if(cursor >= handMax)
        {
            cursor = handMax - 1;
        }
        if(cursor < 0)
        {
            cursor = 0;
        }
    }

    //試合の最初にコマンドをセット
    public static void SetMenu()
    {
        for (int i = 0; i < Unit.playerParameter[(int)Unit.StatusName.KL]; i++)
        {
            int rnd = UnityEngine.Random.Range(0, ReadWrite.commandNum[i]);
            AddCommand(i + 1, rnd);
        }
    }

    //試合の途中にコマンドを追加
    public static void AddMenu()
    {
        //TurnEnd
        AddCommand(1, 0);
        //アクションポイント分のコマンド
        for (int i = 0; i < Unit.SumParameter(-1, Unit.StatusName.ACT); i++)
        {
            //全体を100としたときの確率出現
            int rnd1 = UnityEngine.Random.Range(0, 100);
            for(int j=0; j<ReadWrite.probability.Length; j++)
            {
                rnd1 -= ReadWrite.probability[j];
                if(rnd1 < 0)
                {
                    rnd1 = j;
                    break;
                }
            }
            int rnd2 = UnityEngine.Random.Range(0, ReadWrite.commandNum[rnd1 + 1]);
            AddCommand(rnd1 + 2, rnd2);
        }
        sortNum = 0;
    }

    //コマンドを削除
    public static void DeleteCommand(int num)
    {
        int listNum = -1;
        for(int i=0; i<commandList.Count; i++)
        {
            if(commandList[i].transform.GetComponent<Command>().GetNum() == num)
            {
                listNum = i;
                break;
            }
        }
        commandList[listNum].transform.GetComponent<Command>().DeletePanel();
        commandList.RemoveAt(listNum);
        handMax--;
    }

    //ソートの切り替え
    public static void SortMenu()
    {
        if(sortNum == 0)
        {
            commandList.Sort((a, b) => b.GetComponent<Command>().CalcTypeVar() - a.GetComponent<Command>().CalcTypeVar());
            commandList.ForEach(x => x.transform.SetSiblingIndex(0));
            sortNum++;
        }
        else
        {
            commandList.Sort((a, b) => b.GetComponent<Command>().commandNum - a.GetComponent<Command>().commandNum);
            commandList.ForEach(x => x.transform.SetSiblingIndex(0));
            commandList.ForEach(x => x.GetComponent<Command>().SortEndUp());
            sortNum--;
        }
        
    }

}
