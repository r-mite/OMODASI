using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel : MonoBehaviour {

    //gameポジション
    private Vector2 grid;
    private Material[] mat = new Material[3];
    private Player player;
    public enum MatStr { Select, NoSelect, Selecting };
    private bool mode;
    private List<Vector2> root;

	// Use this for initialization
	void Start () {

        mat[0] = (Material)Resources.Load("Material/block_sibaS");
        mat[1] = (Material)Resources.Load("Material/block_siba");
        mat[2] = (Material)Resources.Load("Material/block_sibaSG");
        player = GameObject.Find("Player").GetComponent<Player>();
        mode = false;
        root = new List<Vector2>();

        ChangeColor(1);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //マウス関連
    void OnMouseEnter()
    {
        ChangeColor(0);
    }

    void OnMouseExit()
    {
        if (mode)
        {
            ChangeColor(2);
        }else
        {
            ChangeColor(1);
        }
    }

    void OnMouseDown()
    {
        if (TurnAndPhase.OnAction()) return;
        if (!Mouse.CheckMouseOutMenu()) return;
        if(mode)
        {
            switch (TurnAndPhase.type)
            {
                case (int)TurnAndPhase.Type.Move:
                    if (Unit.playerPosition == grid) break;
                    Unit.playerPosition = grid;
                    GameCamera.TrackPlayer();
                    player.SendMessage("MovePlayer", root);
                    break;
                case (int)TurnAndPhase.Type.Attack:
                    if (Unit.GetEnemyNum(grid) == -1) break;
                    player.SendMessage("AttackPlayer", grid);
                    break;
                case (int)TurnAndPhase.Type.Buff:
                    player.SendMessage("BuffPlayer");
                    break;
                case (int)TurnAndPhase.Type.Debuff:
                    if (Unit.GetEnemyNum(grid) == -1) break;
                    player.SendMessage("DebuffPlayer", grid);
                    break;
                case (int)TurnAndPhase.Type.Extra:
                    Unit.playerPosition = grid;
                    GameCamera.TrackDig();
                    CutIn.StartCut(player);
                    //player.SendMessage("ExtraPlayer");
                    break;
            }
        }
    }

    //パネル位置
    public void SetGrid(int x, int y)
    {
        grid = new Vector2(x, y);
    }

    //移動可能範囲圏内
    public void ChangeColor(int num)
    {
        if(num == (int)MatStr.Selecting)
        {
            mode = true;
        }
        GetComponent<Renderer>().material = mat[num];
    }

    //移動範囲フラグ解除
    public void ResetMode()
    {
        mode = false;
        MakeRoot();
    }

    //ルート作成
    public void MakeRoot()
    {
        if (root == null) return;
        root.Clear();
        root.Add(grid);
    }

    //ルート出力
    public List<Vector2> GetRoot()
    {
        return root;
    }

    //ルート追加
    public void AddRoot(List<Vector2> list)
    {
        if (root == null) return;
        root.AddRange(list);
    }

    //外部から選択可能か調べる
    public bool CheckMode()
    {
        return mode;
    }

}
