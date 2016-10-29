using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private static AsyncOperation async;
    private int buttonNum;
    private bool asyncFlag;
    private float timer;
    private bool visual;
    private static Color[] col = new Color[2];
    private static string[] spriteName =
    {
        "Image/startbutton",
        "Image/startbutton2",
    };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!visual || StartMenu.CheckMoving()) return;
        if (StartMenu.CheckAfterEnter()) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        switch (buttonNum)
        {
            case 0:
                StartCoroutine("CallAdvent");
                break;
            case 1:
                Music.PlaySE(Music.Clip.Mekuri);
                StartMenu.MoveStageMenu(true);
                break;
            case 2:
                StartCoroutine("CallAdvent");
                break;
            case 3:
                Music.PlaySE(Music.Clip.Mekuri);
                StartMenu.MoveSettingMenu(true);
                break;
            case 4:
                StartCoroutine("CallAdvent");
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!visual) return;
        if (StartMenu.CheckAfterEnter()) return;
        Music.PlaySE(Music.Clip.Over);
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
        asyncFlag = false;
        timer = 0;
        visual = true;
        ChangeColor(0);
    }

    //パネル色変え
    private void ChangeColor(int num)
    {
        //GetComponent<Image>().color = col[num];
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteName[num]);
    }

    public void ChangeVisual(bool vis)
    {
        visual = vis;
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
            case 2:
                SceneManager.LoadScene("Main5");
                break;
            case 4:
                Application.Quit();
                break;
        }
    }
    //非同期シーンローディング
    //コルーチン呼び出し
    //StartCoroutine(LoadScene());
    /*
    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("Main1");
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress * 100 + "%");
            yield return new WaitForSeconds(0);
        }
        asyncFlag = true;
        yield return async;
    }
    */
}
