using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    private List<Vector2> root;
    private Vector3 cursorVec;
    private int action;
    private enum Action { None, Attack, Move, Damage, Buff, Debuff, Dig};
    private float timer;
    private bool swingFlag;
    private const float attackTime = 2f;
    private const float damageTime = 0.2f;
    private const float buffTime = 2f;
    private const float debuffTime = 0.5f;
    private float moveSpeed = 0.1f;
    private float allow = 0.001f;
    private int moveCounter;
    private int moveCounterMax = 10;
    private int rootCounter;
    private int enemyID;
    private int enemyVerstion;
    private int doubleAttack;
    private int bossMove = 0;
    private Player player;
    private bool dead;
    private float digTime = 1f;

    CharacterController characterController;
    Animator animator;

    // Use this for initialization
    void Start () {

        root = new List<Vector2>();
        cursorVec = new Vector3();
        action = (int)Action.None;
        player = GameObject.Find("Player").GetComponent<Player>();
        dead = false;

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        SetSpeed();

    }
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;

        switch (action)
        {
            case (int)Action.Attack:
                timer += delta;
                if (swingFlag && timer > 0.3f)
                {
                    swingFlag = false;
                    Unit.DamagedPlayer(enemyID);
                    Music.PlaySE(Music.Clip.Swing2);
                }
                if (timer > attackTime)
                {
                    action = (int)Action.None;
                    if (enemyVerstion == 4 && doubleAttack == 0)
                    {
                        doubleAttack++;
                        FightEnemy();
                        break;
                    }
                    doubleAttack--;
                    Unit.TurnEnemy(false);
                }
                break;
            case (int)Action.Move:
                Vector3 moveVec = cursorVec - transform.position;
                if (moveCounter <= 0)
                {
                    if (rootCounter > 1)
                    {
                        rootCounter--;
                        moveCounter = moveCounterMax;
                        SendMessage("FollowRoot", root[rootCounter - 1]);
                    }
                    else
                    {
                        action = (int)Action.None;
                        animator.SetBool("EnemyMove", false);
                        Music.StopSE();
                        AttackEnemy();
                    }
                }
                else
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
                    Music.PlaySE(Music.Clip.Hit);
                }
                break;
            case (int)Action.Debuff:
                timer += delta;
                if (timer > debuffTime)
                {
                    action = (int)Action.None;
                    Music.PlaySE(Music.Clip.Debuff);
                }
                break;
            case (int)Action.Dig:
                timer += delta;
                transform.Translate(new Vector3(0, 2f * delta / digTime, 0));
                if (timer > digTime)
                {
                    action = (int)Action.None;
                    Unit.TurnEnemy(false);
                }
                break;
        }

    }

    //初期位置
    public void SetParameter(int id, int ver)
    {
        enemyID = id;
        enemyVerstion = ver;
        doubleAttack = 0;
        bossMove = 0;
    }

    //マウスオーバ
    void OnMouseEnter()
    {
        Music.PlaySE(Music.Clip.Dun);
        Status.EnemyStatus(true, enemyID);
        if (!TurnAndPhase.OnField() && TurnAndPhase.OnPlayer())
        {
            Floor.ShowSelecting(Unit.GetEnemyGamePosition(enemyID), Unit.enemyParameterList[enemyID][(int)Unit.StatusName.DIS]);
        }
    }

    void OnMouseExit()
    {
        Status.EnemyStatus(false, enemyID);
        if (!TurnAndPhase.OnField() && TurnAndPhase.OnPlayer())
        {
            Floor.ResetPanel();
        }
    }

    //マウスクリック
    void OnMouseDown()
    {
        Vector3 vec = Unit.GetEnemyGamePosition(enemyID);
        switch (TurnAndPhase.type)
        {
            case (int)TurnAndPhase.Type.Attack:
                if (!Floor.CheckSelectingGrid(vec)) break;
                player.SendMessage("AttackPlayer", (Vector2)vec);
                break;
            case (int)TurnAndPhase.Type.Debuff:
                if (!Floor.CheckSelectingGrid(vec)) break;
                player.SendMessage("DebuffPlayer", (Vector2)vec);
                break;
        }
        Status.EnemyStatus(true, enemyID);
    }

    //敵のターン開始
    public void FightEnemy()
    {
        //死んでるか
        if (dead)
        {
            Unit.TurnEnemy(false);
            return;
        }
        //移動の必要性
        if(enemyVerstion == 4 && bossMove == 0)
        {
            int other = 0;
            for(int i=0; i<5; i++)
            {
                if (Unit.DeadEnemy(i))
                {
                    other++;
                }
            }
            if (other > 3)
            {
                bossMove++;
                FightEnemy();
                return;
            }
            if(Unit.playerPosition.x > 5 && Unit.playerPosition.x < 9 &&
                Unit.playerPosition.y > 0 && Unit.playerPosition.y < 7)
            {
                AttackEnemy();
                return;
            }
            bossMove++;
        }
        float dis = enemyVerstion == 4 ? 2f : 1f;
        if(Vector2.Distance(Unit.GetEnemyGamePosition(enemyID), Unit.playerPosition) > dis)
        {
            TurnAndPhase.SetEnemyCommand(TurnAndPhase.Type.Move);
            Floor.ShowSelecting(Unit.GetEnemyGamePosition(enemyID), Unit.enemyParameterList[enemyID][(int)Unit.StatusName.DIS]);
            root.Clear();
            Floor.NearPlayer(ref root);
            Unit.ChangePosition(enemyID, root[0]);
            moveCounter = 0;
            rootCounter = root.Count;
            FollowRoot(root[rootCounter - 1]);
            action = (int)Action.Move;
            animator.SetBool("EnemyMove", true);
            Music.PlaySELoop(Music.Clip.Walk);
        }else
        {
            AttackEnemy();
        }
    }

    //攻撃開始
    public void AttackEnemy()
    {
        //攻撃範囲内にいるか
        float dis = enemyVerstion == 4 ? 2f : 1f;
        if (Vector2.Distance(Unit.GetEnemyGamePosition(enemyID), Unit.playerPosition) <= dis)
        {
            TurnAndPhase.SetEnemyCommand(TurnAndPhase.Type.None);
            if(enemyVerstion == 4)
            {
                Floor.ShowBossEnemyATK(Unit.GetEnemyGamePosition(enemyID), (int)dis);
            }else
            {
                Floor.ShowSelecting(Unit.GetEnemyGamePosition(enemyID), (int)dis);
            }
            action = (int)Action.Attack;
            Log.ShowLog(3);
            timer = 0;
            FollowRoot(Unit.playerPosition);
            Unit.ChangeDirectionPlayer(enemyID);
            animator.SetTrigger("AttackTrigger");
            swingFlag = true;
        }else
        {
            if (enemyVerstion == 4 && doubleAttack == 0)
            {
                doubleAttack++;
                FightEnemy();
                return;
            }
            doubleAttack--;
            Unit.TurnEnemy(false);
        }
    }

    //目的地を定める
    private void FollowRoot(Vector2 vec)
    {
        cursorVec = Parameter.Calc3DFloorToVec3(vec);
        transform.LookAt(cursorVec);
    }

    //被ダメージ動作
    public void DamageEnemy(bool knock, bool back)
    {
        if (knock)
        {
            dead = true;
            transform.GetComponent<CharacterController>().enabled = false;
            animator.SetBool("KnockBool", true);
        }
        if (back)
        {
            animator.SetTrigger("DamageTrigger");
            action = (int)Action.Damage;
            timer = 0;
        }
    }

    //バフかけ
    public void BuffEnemy()
    {
        action = (int)Action.Buff;
        timer = 0;
    }

    //デバフかけられ
    public void DebuffEnemy()
    {
        animator.SetTrigger("DebuffTrigger");
        action = (int)Action.Debuff;
        timer = 0;
    }

    //プレイヤーのほうを向く
    public void FollowPlayer()
    {
        FollowRoot(Unit.playerPosition);
    }

    //ダイニングメッセージ
    public bool AreYouDead()
    {
        return dead;
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
                digTime = 1f;
                break;
            case 1:
                moveSpeed = 0.05f;
                moveCounterMax = 20;
                digTime = 0.8f;
                break;
            case 2:
                moveSpeed = 0.1f;
                moveCounterMax = 10;
                digTime = 0.6f;
                break;
            case 3:
                moveSpeed = 1f;
                moveCounterMax = 1;
                digTime = 0.4f;
                break;
        }
    }

    public void AppearEnemy()
    {
        
        Log.ShowLog(8);
        timer = 0;
        StartCoroutine("sleep");
        //Unit.TurnEnemy(false);
    }

    IEnumerator sleep()
    {
        yield return new WaitForSeconds(1);  //10秒待つ
        action = (int)Action.Dig;
    }

}
