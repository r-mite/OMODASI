using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private static Image backPanel;
    private static Text result;

	// Use this for initialization
	void Start () {

        backPanel = transform.GetComponent<Image>();
        /*
        result = GameObject.Find("Result").transform.FindChild("Text").GetComponent<Text>();
        result.text = ReadWrite.ReadRecord();
	*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StartMenu.CheckAfterEnter()) return;
        StartMenu.BackMenu();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public static void VisBackPanel(bool vis)
    {
        backPanel.enabled = vis;
    }

}
