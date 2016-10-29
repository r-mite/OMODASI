using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RealyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int buttonNum;
    private static Color[] col = new Color[2];

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        switch (buttonNum)
        {
            case 0:
                SceneManager.LoadScene("Start");
                break;
            case 1:
                HomeButton.SwitchRealy(false);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Music.PlaySE(Music.Clip.Dun);
        ChangeColor(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
        col[0] = Color.white;
        col[1] = Color.gray;
        ChangeColor(0);
    }

    //パネル色変え
    private void ChangeColor(int num)
    {
        GetComponent<Image>().color = col[num];
    }

    public void ResetColor()
    {
        ChangeColor(0);
    }
    
}
