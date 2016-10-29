using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cursor : MonoBehaviour {

    private const float allow = 0.1f;
    private Vector2 keydown;
    private float timer;
    private const float step = 0.18f;
    public static int phase;
    public enum Phase { Field , Menu };
    private int menuCursor;
    private static int actCounter;

    GameObject cursorM;

    // Use this for initialization
    void Start () {

        InitParameter();
        phase = (int)Phase.Menu;
        menuCursor = 0;

        cursorM = GameObject.Find("CursorM");

    }
	
	// Update is called once per frame
	void Update () {

        float delta = Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (phase)
            {
                case (int)Phase.Field:
                    //MovePlayer(transform.position);
                    break;
                case (int)Phase.Menu:
                    switch (menuCursor)
                    {
                        case 0:
                            //AttackPlayer();
                            break;
                        case 1:
                            //FocusField();
                            break;
                    }
                    break;
            }
            InitParameter();
            return;
        }

        Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        int newx,newy;

        CalcPosition(out newx, inputDir.x);
        CalcPosition(out newy, inputDir.y);
        MovePosition(delta, newx, newy);
        keydown.Set(newx, newy);

    }

    private void InitParameter()
    {
        keydown = new Vector2();
        timer = 0f;
    }

    private void CalcPosition(out int pos, float vec)
    {
        pos = 0;
        if (vec > allow)
        {
            pos = 1;
        }
        else if (vec < -allow)
        {
            pos = -1;
        }
    }

    private void MovePosition(float time, int x, int y)
    {
        if (x == 0 && y == 0) return;
        if ((keydown.x == 0 && keydown.y == 0) || timer > step)
        {
            switch (phase)
            {
                case (int)Phase.Field:
                    transform.Translate(x, 0, y);
                    break;
                case (int)Phase.Menu:
                    Menu.ChangeHand(out menuCursor, menuCursor, -y);
                    Menu.ChooseCommand(menuCursor);
                    break;
            }
            timer = 0;
        }
        else
        {
            timer += time;
        }
    }

    /*
    public void MovePlayer(List<Vector2> list)
    {
        player.MovePlayer(list);
        phase = (int)Phase.Menu;
    }

    public void AttackPlayer()
    {
        player.SendMessage("AttackPlayer");
        Menu.AddCommand();
    }

    public static void FocusField()
    {
        phase = (int)Phase.Field;
    }

    private static void turnPhase(int num)
    {
        actCounter--;
        if(actCounter < 1)
        {
            Battle.turn = 1;
            Log.ShowLog(num);
        }
    }

    public static void SetActCounter()
    {
        actCounter = Parameter.playerAct;
    }
    */
}
