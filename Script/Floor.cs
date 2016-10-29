using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : MonoBehaviour {

    private Object floor;
    private Object tree;
    private static Vector3 panelSize = new Vector3(0.1f, 1, 0.1f);
    private static Vector3 treeSize = new Vector3(0.08f, 0.08f, 0.08f);
    private static Panel[,] panels = new Panel[20,20];
    private static int[,] searchPanel = new int[20,20];
    private static int[] fieldSize = new int[2];

	// Use this for initialization
	void Start () {

        floor = Resources.Load("Prefub/Panel");
        tree = Resources.Load("Prefub/Tree");
        fieldSize = ReadWrite.CalcField();
        SetPanel();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //パネルを並べるのら
    private void SetPanel()
    {
        for(int i=0; i<fieldSize[0]; i++)
        {
            for(int j=0; j<fieldSize[1]; j++)
            {
                GameObject obj = (GameObject)Instantiate(floor, new Vector3(i - 2, 0, j - 2), Quaternion.identity);
                obj.transform.SetParent(transform, false);
                obj.transform.localScale = panelSize;
                obj.transform.GetComponent<Panel>().SetGrid(i, j);
                panels[i,j] = obj.transform.GetComponent<Panel>();
                //障害物があれば設置
                if (ReadWrite.PointPanel(i, j) == 1)
                {
                    obj = (GameObject)Instantiate(tree, new Vector3(i - 2, 0, j - 2), Quaternion.identity);
                    obj.transform.SetParent(transform, false);
                    obj.transform.localScale = treeSize;
                }
            }
        }
    }

    //障害物設置
    private static void ResetObstacle()
    {
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                if (ReadWrite.PointPanel(i, j) == 1)
                {
                    searchPanel[i, j] = -9;
                }
            }
        }
    }

    //瞬間移動時の移動範囲
    public static void ShowTeleport()
    {
        ResetPanel();
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                if(searchPanel[i, j] == -1)
                {
                    searchPanel[i, j] = 0;
                }
            }
        }
        ReShow();
    }

    //ボスエネミーの貫通攻撃範囲
    public static void ShowBossEnemyATK(Vector2 vecGame, int dis)
    {
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                searchPanel[i, j] = -1;
                panels[i, j].ChangeColor((int)Panel.MatStr.NoSelect);
                panels[i, j].ResetMode();
            }
        }
        SearchPosition(vecGame, dis);
        ReShow();
    }

    //移動範囲を表示す
    public static void ShowSelecting(Vector2 vecGame, int dis)
    {
        ResetPanel();
        SearchPosition(vecGame, dis);
        ReShow();
    }

    //パネルの表示をリセット
    public static void ResetPanel()
    {
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                searchPanel[i, j] = -1;
                panels[i, j].ChangeColor((int)Panel.MatStr.NoSelect);
                panels[i, j].ResetMode();
            }
        }
        if (TurnAndPhase.type == (int)TurnAndPhase.Type.Attack || TurnAndPhase.type == (int)TurnAndPhase.Type.Move)
        {
            Vector2 vec = Unit.playerPosition;
            searchPanel[(int)vec.x, (int)vec.y] = -9;
        }
        if (TurnAndPhase.type != (int)TurnAndPhase.Type.Attack && TurnAndPhase.type != (int)TurnAndPhase.Type.Debuff)
        {
            int len = Unit.GetPositionListLength();
            for (int i = 0; i < len; i++)
            {
                if (Unit.DeadEnemy(i)) continue;
                Vector2 vec = Unit.GetEnemyGamePosition(i);
                searchPanel[(int)vec.x, (int)vec.y] = -9;
            }
        }
        ResetObstacle();
    }

    //パネルを再表示
    public static void ReShow()
    {
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                if (searchPanel[i, j] >= 0)
                {
                    panels[i, j].ChangeColor((int)Panel.MatStr.Selecting);
                }
            }
        }
    }

    //繰り返しで移動経路を調べる
    //*再帰だとおーばふろーしてしまったぞ
    //*繰り返しだとさすがに早いな
    private static void SearchPosition(Vector2 vec, int point)
    {
        bool change = false;

        searchPanel[(int)vec.x, (int)vec.y] = point;

        while (true)
        {
            for (int i = 0; i < fieldSize[0]; i++)
            {
                for (int j = 0; j < fieldSize[1]; j++)
                {
                    if(searchPanel[i, j] > 0)
                    {
                        UpdatePanelPoint(ref change, i, j);
                    }
                }
            }
            if (change)
            {
                change = false;
            }else
            {
                break;
            }
        }
    }

    //上下左右のパネルを更新
    private static void UpdatePanelPoint(ref bool flag, int x, int y)
    {
        for(int i=0; i<4; i++)
        {
            int dx = x + (int)Parameter.direcrion[i].x;
            int dy = y + (int)Parameter.direcrion[i].y;

            if (dx < 0 || fieldSize[0] <= dx || dy < 0 || fieldSize[1] <= dy) continue;
            if (searchPanel[dx, dy] < -1) continue;

            //初回更新
            if (searchPanel[dx, dy] == -1)
            {
                searchPanel[dx, dy] = searchPanel[x, y] - 1;
                panels[dx, dy].AddRoot(panels[x, y].GetRoot());
                flag = true;
            }
            //２回目以降
            else if (searchPanel[dx, dy] < searchPanel[x, y] - 1)
            {
                searchPanel[dx, dy] = searchPanel[x, y] - 1;
                panels[dx, dy].MakeRoot();
                panels[dx, dy].AddRoot(panels[x, y].GetRoot());
                flag = true;
            }
        }
    }

    //プレイヤーに近接しているか調べる
    public static void NearPlayer(ref List<Vector2> list)
    {
        float dis = 10000f;
        int own = 0;
        Vector2 vec = new Vector2();
        for (int i = 0; i < fieldSize[0]; i++)
        {
            for (int j = 0; j < fieldSize[1]; j++)
            {
                if (searchPanel[i, j] >= 0)
                {
                    Vector2 newVec = new Vector2(i, j);
                    float newDis = Vector2.Distance(Unit.playerPosition, newVec);
                    if (dis > newDis && newDis != 0)
                    {
                        dis = newDis;
                        vec = newVec;
                    }
                    own = Mathf.Max(own, searchPanel[i, j]);
                }
            }
        }
        if(searchPanel[(int)vec.x, (int)vec.y] == own)
        {
            dis = 10000f;
            for (int i = 0; i < fieldSize[0]; i++)
            {
                for (int j = 0; j < fieldSize[1]; j++)
                {
                    if (searchPanel[i, j] == 0)
                    {
                        Vector2 newVec = new Vector2(i, j);
                        float newDis = Vector2.Distance(Unit.playerPosition, newVec);
                        if (dis > newDis && newDis != 0)
                        {
                            dis = newDis;
                            vec = newVec;
                        }
                    }
                }
            }
        }
        list.AddRange(panels[(int)vec.x, (int)vec.y].GetRoot());
    }

    //その位置のパネルが選択可能かどうか
    public static bool CheckSelectingGrid(Vector2 GameVec)
    {
        return panels[(int)GameVec.x, (int)GameVec.y].CheckMode();
    }

    //コンソールに出力
    private static void DebugPanel()
    {
        for(int j=fieldSize[0]-1; j>=0; j--)
        {
            string line = "";
            for(int i=0; i<fieldSize[1]; i++)
            {
                line += searchPanel[i, j] + ", ";
            }
            Debug.Log(line);
        }
    }
}
