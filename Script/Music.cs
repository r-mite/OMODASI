using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Music : MonoBehaviour {

    private static GameObject scriptObject;
    private static AudioSource bgmSource;
    private static string[] bgmName = {
        "battleBGM",
        "menuBGM",
    };
    private static float bgmVolume = 0.05f;
    private static AudioSource seSource;
    private static List<AudioClip> clip;
    private static string[] seName =
    {
        "win",
        "lose",
        "dash",//主人公移動-草
        "walk",//敵移動-草
        "hit",//主人公攻撃
        "swing",//主人公攻撃あたり
        "swing2",//敵攻撃
        "hit2",//敵攻撃あたり
        "powerup",//バフをつける
        "powerdown",//デバフをかける
        "menumekuri",//ステージセレクトと設定をひらく
        "select",//ボタンオーバー
        "dun",//重めのオーバー
        "enter",//ステージインとか
    };
    public enum Clip { Win, Lose, Dash, Walk, Hit, Swing, Swing2, Hit2, Buff, Debuff , Mekuri, Over, Dun, Enter};
    private static string musicStr = "Music/";
    private static float seVolume = 0.3f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //オーディオの初期設定
    public static void Init(int bgm)
    {
        SetVolume();

        scriptObject = GameObject.Find("Script");
        bgmSource = scriptObject.AddComponent<AudioSource>();
        bgmSource.clip = (AudioClip)Resources.Load(musicStr + bgmName[bgm]);
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;

        seSource = scriptObject.AddComponent<AudioSource>();
        seSource.volume = seVolume;
        clip = new List<AudioClip>();
        for (int i=0; i<seName.Length; i++)
        {
            clip.Add((AudioClip)Resources.Load(musicStr + seName[i]));
        }

        PlayBGM();
    }

    //BGM再生
    public static void PlayBGM()
    {
        bgmSource.Play();
    }

    public static void StopBGM()
    {
        bgmSource.Stop();
    }

    //SE再生
    public static void PlaySE(Clip c)
    {
        seSource.loop = false;
        seSource.PlayOneShot(clip[(int)c]);
    }

    public static void PlaySELoop(Clip c)
    {
        seSource.loop = true;
        seSource.clip = clip[(int)c];
        seSource.Play();
    }

    public static void StopSE()
    {
        seSource.Stop();
    }

    private static void SetVolume()
    {
        int spd, vol;
        ReadWrite.ReadSetting(out spd, out vol);
        bgmVolume = 0.1f * vol / 100;
        seVolume = 0.6f * vol / 100;
    }
}
