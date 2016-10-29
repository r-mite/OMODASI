using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Command : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public int commandNum;
    private int type;
    private int variety;
    private static Color[] col = new Color[2];

    // Use this for initialization
    void Start () {

        //選択色
        col[0] = new Color(235/255f, 121/255f, 136/255f);
        //非選択色
        col[1] = new Color(116/255f, 169/255f, 214/255f);
        ChangeColor(1);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //コマンド識別
    public void SetNum(int num, int _type, int _var)
    {
        commandNum = num;
        type = _type;
        variety = _var;
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(Menu.imageName[type-1]);
    }

    //識別ごとの行動
    public void ActionCommand()
    {
        int[] eff;
        TurnAndPhase.SetCommand(commandNum, type, variety);
        switch (type)
        {
            case (int)TurnAndPhase.Type.TurnEnd:
                TurnAndPhase.EndAction();
                break;
            case (int)TurnAndPhase.Type.Attack:
                eff = ReadWrite.CallCommandEffect(type, variety);
                Floor.ShowSelecting(Unit.playerPosition, eff[1]);
                break;
            case (int)TurnAndPhase.Type.Move:
                eff = ReadWrite.CallCommandEffect(type, variety);
                Floor.ShowSelecting(Unit.playerPosition, Unit.SumParameter(-1, Unit.StatusName.DIS) + eff[0]);
                break;
            case (int)TurnAndPhase.Type.Buff:
                Floor.ShowSelecting(Unit.playerPosition, 0);
                GameCamera.TrackPlayer();
                break;
            case (int)TurnAndPhase.Type.Debuff:
                eff = ReadWrite.CallCommandEffect(type, variety);
                Floor.ShowSelecting(Unit.playerPosition, eff[2]);
                break;
            case (int)TurnAndPhase.Type.Extra:
                eff = ReadWrite.CallCommandEffect(type, variety);
                switch (eff[0])
                {
                    case 0:
                        Floor.ShowSelecting(Unit.playerPosition, 0);
                        GameCamera.TrackPlayer();
                        break;
                    case 1:
                        Floor.ShowTeleport();
                        break;
                }
                break;
        }
    }

    //パネル色変え
    private void ChangeColor(int num)
    {
        GetComponent<Image>().color = col[num];
    }

    //UI用マウス操作
    //*他のとこでも流用できるかもかも
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!TurnAndPhase.OnAction())
        {
            Music.PlaySE(Music.Clip.Over);
            ChangeColor(0);
            Status.CommandStatus(true, type, variety);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeColor(1);
        Status.CommandStatus(false, type, variety);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!TurnAndPhase.OnAction())
        {
            SendMessage("ActionCommand");
        }
    }

    public void DeletePanel()
    {
        Destroy(this.gameObject);
    }

    public int GetNum()
    {
        return commandNum;
    }

    //ソート用計算
    public int CalcTypeVar()
    {
        return type * 1000 + variety;
    }

    //TurnEnd用ソート
    public void SortEndUp()
    {
        if(type == (int)TurnAndPhase.Type.TurnEnd)
        {
            transform.SetSiblingIndex(0);
        }
    }

}
