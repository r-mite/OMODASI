using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Status : MonoBehaviour {

    public static string[] statusText =
    {
        "体力",
        "攻撃力",
        "防御力",
        "命中率",
        "回避率",
        "クリティカル命中率",
        "クリティカル倍率",
        "足の速さ",
        "知識",
        "頭の回転"
    };
    private static string newLine;
    private static Text nameComponent;
    private static Text textComponent;
    private static Image imageComponent;
    private static GameObject damageObject;
    private static bool showFlag;
    private static float timer;
    private const float showStart = 0.8f;
    private const float showTime = 0.8f;
    private static Vector3 damagePosition = new Vector3(0.2f, 1.8f, 0);
    private static bool comStatus;
    private static string[] commandType =
    {
        "皆無",
        "終了",
        "攻撃",
        "移動",
        "強化",
        "弱体",
        "特殊"
    };
    private static string[] commandText =
    {
        "威力",
        "範囲",
        "パラメータ",
        "効果時間"
    };

    // Use this for initialization
    void Start () {

        newLine = System.Environment.NewLine;
        nameComponent = transform.FindChild("Name").GetComponent<Text>();
        textComponent = transform.FindChild("Text").GetComponent<Text>();
        imageComponent = transform.GetComponent<Image>();
        damageObject = GameObject.Find("Damage");

        showFlag = false;
        comStatus = false;

        VisibleStatus(false);
        VisibleDamage(false);

    }
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;
        if (showFlag)
        {
            timer += delta;
            if(timer > showStart + showTime)
            {
                VisibleDamage(false);
                showFlag = false;
            }
            else if (timer > showStart)
            {
                VisibleDamage(true);
            }
        }

    }

    //プレイヤーの表示
    public static void PlayerStatus(bool vis)
    {
        if (comStatus) return;
        if (vis)
        {
            nameComponent.text = Unit.playerName;
            string str = "";
            for(int i=0; i < Unit.playerParameter.Length; i++)
            {
                switch (i)
                {
                    case (int)Unit.StatusName.HP:
                        str += statusText[i] + ": " + Unit.playerStatus[i] + "/" + Unit.playerParameter[i] + newLine;
                        break;
                    case (int)Unit.StatusName.DEX:
                    case (int)Unit.StatusName.EVA:
                    case (int)Unit.StatusName.CDEX:
                        DrawText(ref str, statusText[i], Unit.playerParameter[i], Unit.playerStatus[i], "%");
                        break;
                    case (int)Unit.StatusName.CPOW:
                        DrawText(ref str, statusText[i],
                            Math.Round((double)Unit.playerParameter[i] / 100, 1, MidpointRounding.AwayFromZero),
                            Math.Round((double)Unit.playerStatus[i] / 100, 1, MidpointRounding.AwayFromZero), "");
                        break;
                    default:
                        DrawText(ref str, statusText[i], Unit.playerParameter[i], Unit.playerStatus[i], "");
                        break;
                }
            }
            str += newLine;
            Unit.playerBuff.ForEach(x => str += x.PrintBuff());
            textComponent.text = str;
        }
        VisibleStatus(vis);
    }

    //パラメータと加算バフ表示
    private static void DrawText(ref string str, string text1, double param, double plus, string text2)
    {
        str += text1 + ": " + param;
        if (plus > 0)
        {
            str += " (+" + plus + ")";
        }
        else if (plus < 0)
        {
            str += " (" + plus + ")";
        }
        str += text2 + newLine;
    }

    //エネミーの表示
    public static void EnemyStatus(bool vis, int num)
    {
        if (comStatus) return;
        if (vis)
        {
            nameComponent.text = Unit.enemyname;
            string str = "";
            for (int i = 0; i < Unit.enemyParameterList[num].Length; i++)
            {
                switch (i)
                {
                    case (int)Unit.StatusName.HP:
                        str += statusText[i] + ": " + Unit.enemyStatusList[num][i] + "/" + Unit.enemyParameterList[num][i] + newLine;
                        break;
                    case (int)Unit.StatusName.DEX:
                    case (int)Unit.StatusName.EVA:
                    case (int)Unit.StatusName.CDEX:
                        DrawText(ref str, statusText[i], Unit.enemyParameterList[num][i], Unit.enemyStatusList[num][i], "%");
                        break;
                    case (int)Unit.StatusName.CPOW:
                        DrawText(ref str, statusText[i],
                            Math.Round((double)Unit.enemyParameterList[num][i] / 100, 1, MidpointRounding.AwayFromZero),
                            Math.Round((double)Unit.enemyStatusList[num][i] / 100, 1, MidpointRounding.AwayFromZero), "");
                        break;
                    default:
                        DrawText(ref str, statusText[i], Unit.enemyParameterList[num][i], Unit.enemyStatusList[num][i], "");
                        break;
                }
            }
            textComponent.text = str;
        }
        VisibleStatus(vis);
    }

    //コマンド詳細表示
    public static void CommandStatus(bool vis, int _type, int _var)
    {
        comStatus = vis;
        if (vis)
        {
            nameComponent.text = ReadWrite.CallCommandName(_type, _var);
            string str = "";
            int[] eff;
            str += "                              [" + commandType[_type] + "]" + newLine;
            switch (_type)
            {
                case (int)TurnAndPhase.Type.Attack:
                    eff = ReadWrite.CallCommandEffect(_type, _var);
                    str += commandText[0] + ": " + eff[0] + newLine;
                    str += commandText[1] + ": " + eff[1] + newLine;
                    break;
                case (int)TurnAndPhase.Type.Move:
                    eff = ReadWrite.CallCommandEffect(_type, _var);
                    str += commandText[1] + ": " + eff[0] + newLine;
                    break;
                case (int)TurnAndPhase.Type.Buff:
                    eff = ReadWrite.CallCommandEffect(_type, _var);
                    str += commandText[2] + ": " + statusText[eff[0]] + newLine;
                    str += commandText[0] + ": " + eff[1] + newLine;
                    str += commandText[3] + ": ";
                    if(eff[2] > 0)
                    {
                        str += eff[2] + "T";
                    }else
                    {
                        str += "永続";
                    }
                    str += newLine;
                    break;
                case (int)TurnAndPhase.Type.Debuff:
                    eff = ReadWrite.CallCommandEffect(_type, _var);
                    str += commandText[2] + ": " + statusText[eff[0]] + "\n";
                    str += commandText[0] + ": " + -eff[1] + newLine;
                    str += commandText[1] + ": " + eff[2] + newLine;
                    break;
            }
            str += "\n";
            str += ReadWrite.CallCommandText(_type, _var);
            textComponent.text = str;
        }
        VisibleStatus(vis);
    }

    //プレイヤー攻撃判定
    public static void BreakEnemy(out bool knock, out bool back, int id, Vector3 vec3D)
    {
        knock = false;
        back = true;
        //命中ダイス1d200
        int rnd = UnityEngine.Random.Range(0, 200);
        double dmg = 0;

        int dex = Unit.SumParameter(-1, Unit.StatusName.DEX);
        int eva = Unit.SumParameter(id, Unit.StatusName.EVA);
        if (rnd >= dex + 100 - eva)
        {
            //miss判定
            WriteDamage(vec3D, dmg);
            back = false;
            return;
        }

        //ダメージ誤差ダイス1d10
        rnd = UnityEngine.Random.Range(0, 10);
        int atk = Unit.SumParameter(-1, Unit.StatusName.ATK);
        int def = Unit.SumParameter(id, Unit.StatusName.DEF);
        int cdex = Unit.SumParameter(-1, Unit.StatusName.CDEX);
        int cpow = Unit.SumParameter(-1, Unit.StatusName.CPOW);
        atk += ReadWrite.CallCommandEffect(TurnAndPhase.type, TurnAndPhase.variety)[0];
        int pow = 0;
        Unit.playerBuff.ForEach(x => pow += x.CalcBuff((int)Unit.StatusName.ATK));
        dmg = Math.Round((double)atk * (95 - def + pow + rnd) / 100);
        dmg = Math.Max(dmg, 1);
        //クリティカルダイス1d100
        rnd = UnityEngine.Random.Range(0, 100);
        if(rnd < cdex)
        {
            dmg *= Math.Round((double)cpow / 100);
        }

        //atk判定
        WriteDamage(vec3D, dmg);
        int rest = Unit.enemyStatusList[id][(int)Unit.StatusName.HP] - (int)dmg;
        if(rest <= 0)
        {
            rest = 0;
            knock = true;
        }
        Unit.enemyStatusList[id][(int)Unit.StatusName.HP] = rest;
    }

    //エネミー攻撃判定
    public static void BreakPlayer(out bool knock, out bool back, int id, Vector3 vec3D)
    {
        knock = false;
        back = true;
        //命中ダイス1d200
        int rnd = UnityEngine.Random.Range(0, 200);
        double dmg = 0;

        int dex = Unit.SumParameter(id, Unit.StatusName.DEX);
        int eva = Unit.SumParameter(-1, Unit.StatusName.EVA);
        if (rnd >= dex + 100 - eva)
        {
            //miss判定
            WriteDamage(vec3D, dmg);
            back = false;
            return;
        }

        //ダメージ誤差ダイス1d10
        rnd = UnityEngine.Random.Range(0, 10);
        int atk = Unit.SumParameter(id, Unit.StatusName.ATK);
        int def = Unit.SumParameter(-1, Unit.StatusName.DEF);
        int cdex = Unit.SumParameter(id, Unit.StatusName.CDEX);
        int cpow = Unit.SumParameter(id, Unit.StatusName.CPOW);
        double pow = 0;
        Unit.playerBuff.ForEach(x => pow += x.CalcBuff((int)Unit.StatusName.DEF));
        dmg = Math.Round((double)atk * (95 - def - pow + rnd) / 100);
        dmg = Math.Max(dmg, 1);
        //クリティカルダイス1d100
        rnd = UnityEngine.Random.Range(0, 100);
        if (rnd < cdex)
        {
            dmg *= Math.Round((double)cpow / 100);
        }

        //atk判定
        WriteDamage(vec3D, dmg);
        int rest = Unit.playerStatus[(int)Unit.StatusName.HP] - (int)dmg;
        if (rest <= 0)
        {
            rest = 0;
            knock = true;
        }
        Unit.playerStatus[(int)Unit.StatusName.HP] = rest;
    }

    //オブジェクトの可視化
    public static void VisibleStatus(bool flag)
    {
        imageComponent.enabled = flag;
        textComponent.enabled = flag;
        nameComponent.enabled = flag;

        if (flag)
        {
            Log.NonDisplayTurn();
        }else
        {
            Log.DisplayTurn();
        }
    }

    //ダメージ表記
    private static void WriteDamage(Vector3 vec3D, double damage)
    {
        string str = damage + "";
        if(damage <= 0)
        {
            str = "miss";
        }
        damageObject.GetComponent<TextMesh>().text = str;
        damageObject.GetComponent<TextMesh>().color = Color.red;
        damageObject.transform.position = vec3D + damagePosition;
        timer = 0;
        showFlag = true;
    }

    //ダメージの可視化
    public static void VisibleDamage(bool flag)
    {
        damageObject.GetComponent<MeshRenderer>().enabled = flag;
    }

    //プレイヤーバフを識別
    public static void PowerUpPlayer()
    {
        int[] eff = ReadWrite.CallCommandEffect(TurnAndPhase.type, TurnAndPhase.variety);
        if(TurnAndPhase.type == (int)TurnAndPhase.Type.Extra && eff[0] == 0)
        {
            Unit.playerStatus[(int)Unit.StatusName.HP] /= 2;
            Unit.playerBuff.Add(new Unit.BuffManager(new int[] { (int)Unit.StatusName.ATK, 100, 2}));
            Unit.playerStatus[(int)Unit.StatusName.DEF] -= 10;
            Unit.playerStatus[(int)Unit.StatusName.DEX] -= 5;
            Unit.playerStatus[(int)Unit.StatusName.EVA] -= 5;
            Unit.playerStatus[(int)Unit.StatusName.CDEX] += 30;
            Unit.playerStatus[(int)Unit.StatusName.CPOW] += 300;
        }
        if(eff[2] > 0)
        {
            //ターン乗算バフ
            Unit.playerBuff.Add(new Unit.BuffManager(eff));
        }else
        {
            //永続加算バフ
            Unit.playerStatus[eff[0]] += eff[1];
        }
    }

    //エネミーデバフを識別
    public static void PowerDownEnemy(Vector2 vec)
    {
        int id = Unit.GetEnemyNum(vec);
        int[] eff = ReadWrite.CallCommandEffect(TurnAndPhase.type, TurnAndPhase.variety);
        //永続減算デバフ
        Unit.enemyStatusList[id][eff[0]] += eff[1];
    }

}
