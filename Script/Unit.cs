using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    //バフ管理クラス
    public class BuffManager
    {
        private int para;
        private int quantity;
        private int turn;

        public BuffManager(int[] eff)
        {
            para = eff[0];
            quantity = eff[1];
            turn = eff[2];
        }

        //該当パラメータだった場合に効果量を返す
        public int CalcBuff(int _para)
        {
            return para == _para ? quantity : 0;
        }

        //バフをプリント
        public string PrintBuff()
        {
            string str;
            if (quantity > 0)
            {
                str = "バフ: ";
            } else
            {
                str = "デバフ: ";
            }
            return Status.statusText[para] + str + quantity + "{" + turn + "T}\n";
        }

        //毎ターンのチェック
        public bool TurnZero()
        {
            turn--;
            if (turn < 1)
            {
                return true;
            }
            return false;
        }
    }
    /*
     * パラメータ各種
     * name:名前
     * position:位置
     * hitpoint:体力
     * attack:攻撃
     * defense:防御
     * dexterity:命中率(%)
     * evasion:回避率(%)
     * criticalDEX:クリティカル命中(%)
     * criticalPOW:クリティカル倍率(%)
     * knowledge:初期行動値
     * act:行動値
     * distance:移動距離
     * 
     */
    //プレイヤーパラメータ
    public static string playerName = "ユニティちゃん";
    private static Object prePlayer;
    private static GameObject player;
    public static Vector2 playerPosition;
    public static int[] playerParameter;
    public static int[] playerStatus;
    public static List<BuffManager> playerBuff = new List<BuffManager>();
    public enum StatusName { HP, ATK, DEF, DEX, EVA, CDEX, CPOW, DIS, KL, ACT };

    //エネミーパラメータ
    public static string enemyname = "ユニティSD";
    private static Object preEnemy;
    private static GameObject enemy;
    private static List<GameObject> enemyList;
    //public static int enemyDistance = 2;
    private static List<Vector2> enemyPositionList;
    public static List<int[]> enemyParameterList;
    public static List<int[]> enemyStatusList;
    private static int actionEnemyNum;

    private static int gameKey;
    private static Vector2[] endlessPosition = { new Vector2(14, 7), new Vector2(7, 14), new Vector2(0, 7), new Vector2(7, 0) };
    private static int endlessCounter;
    private static bool newEnemy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //加算バフとパラメータの合計値
    public static int SumParameter(int chara, StatusName sn)
    {
        if(chara < 0)
        {
            //0:プレイヤー
            return playerParameter[(int)sn] + playerStatus[(int)sn];
        }else
        {
            //1以上:エネミー
            return enemyParameterList[chara][(int)sn] + enemyStatusList[chara][(int)sn];
        }
    }

    //初期設定
    public static void Init(int key)
    {
        prePlayer = Resources.Load("Prefub/Player");
        SummonPlayer(key);

        preEnemy = Resources.Load("Prefub/Enemy");
        enemyList = new List<GameObject>();
        SetEnemyParameters();

        gameKey = key;
        endlessCounter = 0;
    }

    //エネミー初期セット
    private static void SetEnemyParameters()
    {
        enemyPositionList = new List<Vector2>();
        enemyParameterList = new List<int[]>();
        enemyStatusList = new List<int[]>();
        int[] size = ReadWrite.CalcField();
        for (int i = 0; i < size[0]; i++)
        {
            for (int j = 0; j < size[1]; j++)
            {
                int point = ReadWrite.PointPanel(i, j);
                if (point >= 5)
                {
                    AddEnemy(point - 5, i, j, false);
                }
            }
        }
    }

    //エネミー追加
    private static void AddEnemy(int ver, int x, int y, bool dig)
    {
        int id = enemyParameterList.Count;
        Vector2 vec = new Vector2(x, y);
        int[] param = ReadWrite.GetEnemyParameter(ver);
        //enemyParameterList.Add(ReadWrite.GetEnemyParameter(ver));
        if (gameKey == 5 - 1)
        {
            int turn = TurnAndPhase.CheckEndlessTurn() + 1;
            if(turn % 10 == 0)
            {
                param[(int)StatusName.HP] += 50 * (turn / 10 - 1);
                param[(int)StatusName.ATK] += 5 * (turn / 10 - 1);
                param[(int)StatusName.DEF] += 10 * (turn / 10 - 1);
                param[(int)StatusName.CDEX] += Mathf.Min(95, 2 * (turn / 10 - 1));
                param[(int)StatusName.CPOW] += 10 * (turn / 10 - 1);
                /*
                enemyParameterList[id][(int)StatusName.HP] += 50 * (turn / 10 - 1);
                enemyParameterList[id][(int)StatusName.ATK] += 5 * (turn / 10 - 1);
                enemyParameterList[id][(int)StatusName.DEF] += 10 * (turn / 10 - 1);
                enemyParameterList[id][(int)StatusName.CDEX] += Mathf.Min(95, 2 * (turn / 10 - 1));
                enemyParameterList[id][(int)StatusName.CPOW] += 10 * (turn / 10 - 1);
                */
            }
            else if(turn % 3 == 0)
            {
                param[(int)StatusName.HP] += 10 * (turn / 3 - 1);
                param[(int)StatusName.ATK] += 2 * (turn / 3 - 1);
                param[(int)StatusName.DEF] += 2 * (turn / 5 - 1);
                param[(int)StatusName.CPOW] += 5 * (turn / 10 - 1);
                /*
                enemyParameterList[id][(int)StatusName.HP] += 10 * (turn / 3 - 1);
                enemyParameterList[id][(int)StatusName.ATK] += 2 * (turn / 3 - 1);
                enemyParameterList[id][(int)StatusName.DEF] += 2 * (turn / 5 - 1);
                enemyParameterList[id][(int)StatusName.CPOW] += 5 * (turn / 10 - 1);
                */
            }
        }
        enemyParameterList.Add(param);
        enemyStatusList.Add(MakeStatus(enemyParameterList[id]));
        enemyPositionList.Add(vec);
        SummonEnemy(id, ver, vec, dig);
    }

    //プレイヤー設置
    private static void SummonPlayer(int key)
    {
        playerPosition = new Vector2(2, 1);
        int[] size = ReadWrite.CalcField();
        int id = 0;
        for (int i = 0; i < size[0]; i++)
        {
            for (int j = 0; j < size[1]; j++)
            {
                int point = ReadWrite.PointPanel(i, j);
                if (point == 4)
                {
                    playerPosition = new Vector2(i, j);
                    break;
                }
            }
        }
        Vector3 vec = Parameter.Calc3DFloorToVec3(playerPosition);
        player = (GameObject)Instantiate(prePlayer, vec, Quaternion.identity);
        player.transform.localScale = Parameter.localMagnification;
        player.name = "Player";
        playerParameter = ReadWrite.GetPlayerParameter(key);
        playerStatus = MakeStatus(playerParameter);
    }

    //エネミー設置
    private static void SummonEnemy(int id, int version, Vector2 vec, bool dig)
    {
        if (dig)
        {
            enemy = (GameObject)Instantiate(preEnemy, Parameter.Calc3DFloorToVec3(vec) - new Vector3(0, 2f, 0), Quaternion.identity);
        }
        else
        {
            enemy = (GameObject)Instantiate(preEnemy, Parameter.Calc3DFloorToVec3(vec), Quaternion.identity);
        }
        //enemy.transform.SetParent(transform, false);
        enemy.transform.localScale = Parameter.localMagnification;
        enemy.transform.Rotate(new Vector3(0, 180));
        enemy.GetComponent<Enemy>().SetParameter(id, version);
        SetEnemy(enemy);
    }

    //体力のみステータスに反映
    private static int[] MakeStatus(int[] para)
    {
        int[] sta = new int[ReadWrite.paraNum];
        sta[0] = para[0];
        for (int i = 1; i < para.Length; i++){
            sta[i] = 0;
        }
        return sta;
    }

    //エネミーの数
    public static int GetPositionListLength()
    {
        return enemyPositionList.Count;
    }

    //番号エネミーの位置
    public static Vector3 GetEnemyGamePosition(int id)
    {
        return enemyPositionList[id];
    }

    public static Vector3 GetEnemy3DPosition(int id)
    {
        return Parameter.Calc3DFloorToVec3(enemyPositionList[id]);
    }

    //エネミー保管
    public static void SetEnemy(GameObject obj)
    {
        enemyList.Add(obj);
    }

    //エネミー行動順制御
    public static void TurnEnemy(bool first)
    {
        if (DeadPlayer())
        {
            Log.EndDeclaration(0);
            return;
        }
        if (first)
        {
            actionEnemyNum = 0;
            newEnemy = false;
        }else
        {
            actionEnemyNum++;
            if (actionEnemyNum >= GetPositionListLength())
            {
                if (newEnemy)
                {
                    TurnAndPhase.EndEnemy();
                    return;
                }
                newEnemy = AddEndlessEnemy();
                if (newEnemy)
                {
                    enemyList[actionEnemyNum].GetComponent<Enemy>().AppearEnemy();
                }else
                {
                    TurnAndPhase.EndEnemy();
                }
                return;
            }
        }
        Status.VisibleDamage(false);
        enemyList[actionEnemyNum].GetComponent<Enemy>().FightEnemy();
    }

    //エンドレスエネミー
    private static bool AddEndlessEnemy()
    {
        if (gameKey != 5 - 1) return false;
        int turn = TurnAndPhase.CheckEndlessTurn() + 1;
        if(turn != 0 && (turn % 3 == 0 || turn % 10 == 0))
        {
            //出現位置上下左右パターン
            for(int i=0; i<4; i++)
            {
                endlessCounter++;
                if (endlessCounter > 3) endlessCounter = 0;
                if (CheckEndlessPosition())
                {
                    int version;
                    if(turn % 10 == 0)
                    {
                        version = 4;
                    }else
                    {
                        version = Random.Range(0, 3);
                    }
                    AddEnemy(version, (int)endlessPosition[endlessCounter].x, (int)endlessPosition[endlessCounter].y, true);
                    return true;
                }
            }
        }
        return false;
    }

    //エンドレス位置出現空き領域確認
    private static bool CheckEndlessPosition()
    {
        Vector2 vec2 = endlessPosition[endlessCounter];
        Vector3 vec3 = Parameter.ConvertToVec3(vec2);
        for(int i=0; i<enemyPositionList.Count; i++)
        {
            Vector3 v = GetEnemyGamePosition(i);
            if(vec3 == v)
            {
                return false;
            }
        }
        if(vec2 == playerPosition)
        {
            return false;
        }
        return true;
    }

    //エネミーの位置移動
    public static void ChangePosition(int id, Vector2 vecGame)
    {
        enemyPositionList[id] = vecGame;
    }

    //エネミーに被ダメージアクションをさせる
    public static void DamagedEnemy(Vector2 vecGame)
    {
        int num = GetEnemyNum(vecGame);
        bool knock, back;
        Status.BreakEnemy(out knock, out back, num, Parameter.Calc3DFloorToVec3(vecGame));
        enemyList[num].GetComponent<Enemy>().DamageEnemy(knock, back);
    }

    //エネミーにデバフをかける
    public static void DebuffedEnemy(Vector2 vecGame)
    {
        int num = GetEnemyNum(vecGame);
        Status.PowerDownEnemy(vecGame);
        enemyList[num].GetComponent<Enemy>().DebuffEnemy();
    }

    //エネミーの方向を変える
    public static void ChangeDirectionEnemy(Vector2 vecGame)
    {
        int num = GetEnemyNum(vecGame);
        enemyList[num].GetComponent<Enemy>().FollowPlayer();
    }

    //プレイヤーに被ダメージアクションをさせる
    public static void DamagedPlayer(int num)
    {
        bool knock, back;
        Status.BreakPlayer(out knock, out back, num, Parameter.Calc3DFloorToVec3(playerPosition));
        player.GetComponent<Player>().DamagePlayer(back);
    }

    //プレイヤーの方向を変える
    public static void ChangeDirectionPlayer(int num)
    {
        Vector2 vec = GetEnemyGamePosition(num);
        player.GetComponent<Player>().FollowEnemy(vec);
    }

    //エネミーの位置から番号を算出
    public static int GetEnemyNum(Vector2 vecGame)
    {
        int listNum = -1;
        for (int i = 0; i < enemyPositionList.Count; i++)
        {
            if (enemyPositionList[i] == vecGame)
            {
                if (DeadEnemy(i)) continue;
                listNum = i;
                break;
            }
        }
        return listNum;
    }

    //エネミーの番号から位置取得
    public static Vector3 GetActionEnemyVec3()
    {
        return GetEnemy3DPosition(actionEnemyNum);
    }

    //エネミーが死んでるかどうか
    public static bool DeadEnemy(int id)
    {
        return enemyList[id].GetComponent<Enemy>().AreYouDead();
    }

    public static bool AllDeadEnemy()
    {
        bool d = true;
        for(int i=0; i<enemyList.Count; i++)
        {
            if (!enemyList[i].GetComponent<Enemy>().AreYouDead())
            {
                d = false;
            }
        }
        if(d) Log.ShowLog(7);
        return d;
    }

    //プレイヤーが死んでるかどうか
    public static bool DeadPlayer()
    {
        if (playerStatus[(int)StatusName.HP] <= 0)
        {
            Log.ShowLog(6);
            return true;
        }
        return false;
    }

    //アニメーション開始
    public static void StartAnim()
    {
        player.GetComponent<Player>().StartAnim();
    }

}
