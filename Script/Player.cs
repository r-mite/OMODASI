using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    private float moveSpeed = 0.1f;
    private float allow = 0.001f;
    private int moveCounter;
    private int moveCounterMax = 10;
    private int rootCounter;
    private static int action;
    private enum Action { None, Attack, Move, Damage, Buff, Debuff, Dig};
    private Vector3 cursorVec;
    private List<Vector2> root;
    private static float timer;
    private static bool swingFlag;
    private const float attackTime = 3f;
    private const float damageTime = 1f;
    private const float buffTime = 2f;
    private const float debuffTime = 2f;
    private const float digTime = 1f;
    private Vector2 enemyVec;

    CharacterController characterController;
    private static Animator animator;

	// Use this for initialization
	void Start () {

        action = (int)Action.None;
        cursorVec = new Vector3();
        root = new List<Vector2>();
        timer = 0;

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        SetSpeed();

	}
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;

        switch(action)
        {
            case (int)Action.Attack:
                timer += delta;
                if(swingFlag && timer > 1f)
                {
                    swingFlag = false;
                    Unit.DamagedEnemy(enemyVec);
                    Music.PlaySE(Music.Clip.Swing);
                }
                if(timer > attackTime)
                {
                    
                    FinishCommand();
                }
                break;
            case (int)Action.Move:
                Vector3 moveVec = cursorVec - transform.position;
                if(moveCounter <= 0)
                {
                    if (rootCounter > 1)
                    {
                        rootCounter--;
                        moveCounter = moveCounterMax;
                        SendMessage("FollowRoot", root[rootCounter-1]);
                    }
                    else
                    {
                        animator.SetBool("PlayerMove", false);
                        Music.StopSE();
                        FinishCommand();
                    }
                }else
                {
                    if (Mathf.Abs(moveVec.x) > allow || Mathf.Abs(moveVec.z) > allow)
                    {
                        moveCounter--;
                    }
                    characterController.Move(moveVec.normalized * moveSpeed);
                }
                break;
            case (int)Action.Damage:
                timer += delta;
                if (timer > damageTime)
                {
                    action = (int)Action.None;
                    animator.SetTrigger("DamageTrigger");
                    Music.PlaySE(Music.Clip.Hit2);
                }
                break;
            case (int)Action.Buff:
                timer += delta;
                if (swingFlag && timer > 1f)
                {
                    swingFlag = false;
                    Music.PlaySE(Music.Clip.Buff);
                }
                if (timer > buffTime)
                {
                    FinishCommand();
                }
                break;
            case (int)Action.Debuff:
                timer += delta;
                if (swingFlag && timer > 1f)
                {
                    swingFlag = false;
                    Unit.DebuffedEnemy(enemyVec);
                    Music.PlaySE(Music.Clip.Swing2);
                }
                if (timer > debuffTime)
                {
                    FinishCommand();
                }
                break;
            case (int)Action.Dig:
                timer += delta;
                if (swingFlag)
                {
                    transform.Translate(new Vector3(0, -2f * delta, 0));
                    if(timer > digTime)
                    {
                        timer = 0;
                        swingFlag = false;
                        Music.PlaySE(Music.Clip.Swing2);
                        transform.position =  Parameter.Calc3DFloorToVec3(Unit.playerPosition) + new Vector3(0, -2f, 0);
                    }
                }else
                {
                    transform.Translate(new Vector3(0, 2f * delta, 0));
                    if (timer > digTime)
                    {
                        FinishCommand();
                    }
                }
                break;
        }
	
	}

    //マウスオーバ
    void OnMouseEnter()
    {
        Music.PlaySE(Music.Clip.Dun);
        Status.PlayerStatus(true);
        if (!TurnAndPhase.OnField() && TurnAndPhase.OnPlayer())
        {
            Floor.ShowSelecting(Unit.playerPosition, Unit.SumParameter(-1, Unit.StatusName.DIS));
        }
    }

    void OnMouseExit()
    {
        Status.PlayerStatus(false);
        if (!TurnAndPhase.OnField() && TurnAndPhase.OnPlayer())
        {
            Floor.ResetPanel();
        }
    }

    //マウスクリック
    void OnMouseDown()
    {
        switch (TurnAndPhase.type)
        {
            case (int)TurnAndPhase.Type.Buff:
                SendMessage("BuffPlayer");
                Status.PlayerStatus(true);
                break;
            case (int)TurnAndPhase.Type.Extra:
                CutIn.StartCut(this);
                //SendMessage("ExtraPlayer");
                Status.PlayerStatus(true);
                break;
        }
    }

    //目的地を定める
    private void FollowRoot(Vector2 vec)
    {
        cursorVec = Parameter.Calc3DFloorToVec3(vec);
        transform.LookAt(cursorVec);
    }

    //移動開始
    public void MovePlayer(List<Vector2> list)
    {
        root = list;
        moveCounter = 0;
        rootCounter = root.Count;
        action = (int)Action.Move;
        TurnAndPhase.StartAction();
        FollowRoot(list[rootCounter-1]);
        animator.SetBool("PlayerMove", true);
        Music.PlaySELoop(Music.Clip.Dash);
    }

    //なぜかたまにlistが正常に処理されず、目的地の一歩手前で止まってしまうので
    public bool CheckPosition()
    {
        Vector2 vec = Parameter.CalcGameFloorToVec2(transform.position);
        vec = Unit.playerPosition - vec;
        if (Mathf.Abs(vec.x) < allow && Mathf.Abs(vec.y) < allow)
        {
            return true;
        }
        FollowRoot(Unit.playerPosition);
        return false;
    }

    //攻撃開始
    public void AttackPlayer(Vector2 vecGame)
    {
        action = (int)Action.Attack;
        TurnAndPhase.StartAction();
        Log.ShowLog(2);
        FollowRoot(vecGame);
        Unit.ChangeDirectionEnemy(vecGame);
        enemyVec = vecGame;
        animator.SetTrigger("AttackTrigger");
        swingFlag = true;
        timer = 0;
    }

    //バフをかける
    public void BuffPlayer()
    {
        action = (int)Action.Buff;
        TurnAndPhase.StartAction();
        Log.ShowLog(4);
        Status.PowerUpPlayer();
        animator.SetTrigger("BuffTrigger");
        swingFlag = true;
        timer = 0;
    }

    //デバフをかける
    public void DebuffPlayer(Vector2 vecGame)
    {
        action = (int)Action.Debuff;
        TurnAndPhase.StartAction();
        Log.ShowLog(5);
        FollowRoot(vecGame);
        Unit.ChangeDirectionEnemy(vecGame);
        enemyVec = vecGame;
        animator.SetTrigger("DebuffTrigger");
        swingFlag = true;
        timer = 0;
    }

    //エクストラ行動
    public void ExtraPlayer()
    {
        int[] eff = ReadWrite.CallCommandEffect(TurnAndPhase.type, TurnAndPhase.variety);
        switch (eff[0])
        {
            case 0:
                action = (int)Action.Buff;
                Log.ShowLog(4);
                Status.PowerUpPlayer();
                animator.SetTrigger("BuffTrigger");
                break;
            case 1:
                action = (int)Action.Dig;
                break;
        }
        TurnAndPhase.StartAction();
        swingFlag = true;
        timer = 0;
    }

    //コマンド終了時の処理
    private void FinishCommand()
    {
        action = (int)Action.None;
        TurnAndPhase.EndAction();
        Floor.ResetPanel();
    }

    //被ダメージ動作
    public void DamagePlayer(bool back)
    {
        if (!back) return;
        action = (int)Action.Damage;
        timer = 0;
    }

    //エネミーのほうを向く
    public void FollowEnemy(Vector2 gameVec)
    {
        FollowRoot(gameVec);
    }

    //プレイヤーアニメーション開始
    public void StartAnim()
    {
        animator.SetTrigger("StartTrigger");
    }

    //スピードの設定
    private void SetSpeed()
    {
        int spd, vol;
        ReadWrite.ReadSetting(out spd, out vol);
        switch (spd)
        {
            case 0:
                moveSpeed = 0.02f;
                moveCounterMax = 50;
                break;
            case 1:
                moveSpeed = 0.04f;
                moveCounterMax = 25;
                break;
            case 2:
                moveSpeed = 0.2f;
                moveCounterMax = 5;
                break;
            case 3:
                moveSpeed = 0.25f;
                moveCounterMax = 4;
                break;
        }
    }

}
