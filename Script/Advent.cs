using UnityEngine;
using System.Collections;
using Novel;

public class Advent : MonoBehaviour {

    private static string wide = "wide/";
    private static string[] scene =
    {
        "scene00",
        "scene01",
        "scene02",
        "scene03",
        "scene04",
    };
    private static string[] losePattern =
    {
        "lose01",
        "lose02",
        "lose03",
        "lose04",
    };

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void StartAdventure(int key, int win)
    {
        string str1 = wide;
        string str2 = "";
        if(win == 1)
        {
            str1 += scene[key+1];
        }else
        {
            str1 += scene[4];
            str2 += losePattern[key];
        }
        NovelSingleton.StatusManager.callJoker(str1, str2);
    }

}
