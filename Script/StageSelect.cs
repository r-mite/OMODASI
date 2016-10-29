using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int buttonNum;
    private static Color[] col = new Color[2];
    private static string[] spriteName =
    {
        "Image/bg1",
        "Image/bg2",
        "Image/bg3",
        "Image/bg4",
    };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StartMenu.CheckMoving()) return;
        if (StartMenu.CheckAfterEnter()) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        StartCoroutine("CallAdvent");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (StartMenu.CheckAfterEnter()) return;
        Music.PlaySE(Music.Clip.Dun);
        ChangeColor(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (StartMenu.CheckAfterEnter()) return;
        ChangeColor(0);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        

    }

    public void Init(int num)
    {
        buttonNum = num;
        col[0] = Color.gray;
        col[1] = Color.white;
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteName[num]);
        ChangeColor(0);
    }

    //パネル色変え
    private void ChangeColor(int num)
    {
        GetComponent<Image>().color = col[num];
        //transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteName[num]);
    }

    private IEnumerator CallAdvent()
    {
        Music.PlaySE(Music.Clip.Enter);
        StartMenu.OnAfterEnter();
        yield return new WaitForSeconds(3);
        switch (buttonNum)
        {
            case 0:
                Advent.StartAdventure(-1, 1);
                break;
            case 1:
                Advent.StartAdventure(0, 1);
                break;
            case 2:
                Advent.StartAdventure(1, 1);
                break;
            case 3:
                Advent.StartAdventure(2, 1);
                break;
        }
    }

}
