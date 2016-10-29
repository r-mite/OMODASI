using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CutIn : MonoBehaviour {

    private static Transform cut;
    private static Transform anim;
    private static Sprite[] sp;
    private static string spName = "anime/A_";
    private const int animMax = 16;
    private static bool cutFlag;
    private static int spNum;
    private static float timer;
    private const float flame = 0.08f;

    private static Player player;

	// Use this for initialization
	void Start () {

        cut = transform;
        anim = transform.FindChild("Image");
        sp = new Sprite[animMax];
        for(int i=0; i<animMax; i++)
        {
            sp[i] = Resources.Load<Sprite>(spName + String.Format("{0:00000}", i));
        }
        cutFlag = false;

	}
	
	// Update is called once per frame
	void Update () {

        if (cutFlag)
        {
            timer += Time.deltaTime;
            if(timer > flame)
            {
                timer = 0;
                spNum++;
                if(spNum > animMax - 1)
                {
                    cutFlag = false;
                    cut.GetComponent<Image>().enabled = false;
                    anim.GetComponent<Image>().enabled = false;
                    player.SendMessage("ExtraPlayer");
                    return;
                }
                anim.GetComponent<Image>().sprite = sp[spNum];
            }
        }

	}

    public static void StartCut(Player p)
    {
        player = p;
        cutFlag = true;
        spNum = 0;
        anim.GetComponent<Image>().sprite = sp[spNum];
        cut.GetComponent<Image>().enabled = true;
        anim.GetComponent<Image>().enabled = true;
        timer = 0;
    }
}
