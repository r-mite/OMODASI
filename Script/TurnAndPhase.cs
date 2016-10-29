using UnityEngine;
using System.Collections;

public class TurnAndPhase : MonoBehaviour {

    private static int turn;
    private enum Turn { Player, Enemy};
    private static int endlessTurn = 1;
    private static int phase;
    private static int action;
    public enum Action { Field, Menu, Act };
    private static int command;
    public static int type;
    public enum Type { None, TurnEnd, Attack, Move, Buff, Debuff, Extra};
    public static int variety;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //変数の初期化
    public static void InitVariable()
    {
        turn = (int)Turn.Player;
        endlessTurn = 1;
        phase = 0;
        action = (int)Action.Menu;
        command = 0;
        type = (int)Type.None;
    }

    //行動中
    public static void StartAction()
    {
        action = (int)Action.Act;
    }

    //コマンド取り消し
    public static void CancelAction()
    {
        action = (int)Action.Menu;
        type = (int)Type.None;
        Floor.ResetPanel();
    }

    //行動終了
    public static void EndAction()
    {
        Menu.DeleteCommand(command);
        Floor.ResetPanel();
        Status.VisibleStatus(false);
        if (Unit.AllDeadEnemy())
        {
            Log.EndDeclaration(1);
            return;
        }
        action = (int)Action.Menu;
        CountPhase();
    }

    //フェーズ数カウント
    private static void CountPhase()
    {
        phase++;
        if (type == (int)Type.TurnEnd)
        {
            ExchangeTurn();
        }else if (Menu.handMax <= 0)
        {
            ExchangeTurn();
        }
        type = (int)Type.None;
    }

    public static void EndEnemy()
    {
        type = (int)Type.None;
        Floor.ResetPanel();
        ExchangeTurn();
    }

    //ターンの切り替え
    private static void ExchangeTurn()
    {
        if(turn == (int)Turn.Player)
        {
            turn = (int)Turn.Enemy;
            GameCamera.TrackEnemy();
            Unit.TurnEnemy(true);
            Log.ShowLog(1);
        }
        else
        {
            Unit.playerBuff.RemoveAll(x => x.TurnZero());
            turn = (int)Turn.Player;
            endlessTurn++;
            GameCamera.TrackPlayer();
            Menu.AddMenu();
            phase = 0;
            Log.ShowLog(0);
        }
    }

    //使用中のコマンドナンバータイプ
    public static void SetCommand(int num, int _type, int _var)
    {
        command = num;
        type = _type;
        variety = _var;
        action = (int)Action.Field;
    }

    //エネミーだってコマンドを使いたいっ
    public static void SetEnemyCommand(Type _type)
    {
        type = (int)_type;
    }

    //プレイヤーターンか
    public static bool OnPlayer()
    {
        if (turn == (int)Turn.Player)
        {
            return true;
        }
        return false;
    }

    //プレイヤーのターン数確認
    public static int CheckEndlessTurn()
    {
        return endlessTurn;
    }

    //コマンドによってフィールド選択画面であるかどうか
    public static bool OnField()
    {
        if(action == (int)Action.Field)
        {
            return true;
        }
        return false;
    }

    //コマンドの操作を実行中かどうか
    public static bool OnAction()
    {
        if (action == (int)Action.Act)
        {
            return true;
        }
        return false;
    }
}
