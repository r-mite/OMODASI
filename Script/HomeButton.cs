using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private static string[] spriteName =
    {
        "Image/homebutton",
        "Image/homebutton2",
    };
    private static GameObject realy;
    private static bool realyMode;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        SwitchRealy(!realyMode);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Music.PlaySE(Music.Clip.Over);
        ChangeColor(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeColor(0);
    }

    // Use this for initialization
    void Start () {

        Init();

	}
	
	// Update is called once per frame
	void Update () {

        if (realyMode && Input.GetMouseButtonDown(1))
        {
            SwitchRealy(false);
        }

    }

    public void Init()
    {
        realy = GameObject.Find("Realy");
        realy.transform.FindChild("Win").FindChild("Y").GetComponent<RealyButton>().Init(0);
        realy.transform.FindChild("Win").FindChild("N").GetComponent<RealyButton>().Init(1);
        SwitchRealy(false);
        ChangeColor(0);
    }

    //パネル色変え
    private void ChangeColor(int num)
    {
        transform.GetComponent<Image>().sprite = Resources.Load<Sprite>(spriteName[num]);
    }

    public static void SwitchRealy(bool swi)
    {
        realy.SetActive(swi);
        realyMode = swi;
        if (swi)
        {
            realy.transform.FindChild("Win").FindChild("Y").GetComponent<RealyButton>().ResetColor();
            realy.transform.FindChild("Win").FindChild("N").GetComponent<RealyButton>().ResetColor();
        }
    }

}
