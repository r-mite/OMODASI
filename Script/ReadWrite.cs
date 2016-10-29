using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class ReadWrite : MonoBehaviour {

    public class CommandList : IEquatable<CommandList>
    {
        public int type;
        public int variety;
        public string name;
        public string text;
        public int[] effect;

        public CommandList(string[] str, int num)
        {
            if (str.Length < 3) return;
            type = int.Parse(str[0]);
            variety = num;
            name = str[1];
            text = str[2];
            effect = new int[3];
            int len = str.Length - 3;
            for(int i=0; i<len; i++)
            {
                effect[i] = int.Parse(str[i + 3]);
            }
        }

        public bool Equals(CommandList other)
        {
            if (other == null) return false;
            return (this.type == other.type && this.variety == other.variety);
        }
    }

    private static string path = Application.dataPath + "/Resources/Text/";
    private static string[][] fileName =
    {
        new string[]{ "command1.txt", "field1.txt", "enemypara.txt" ,"playerpara.txt"},
        new string[]{ "command2.txt", "field2.txt", "enemypara.txt", "playerpara.txt"},
        new string[]{ "command3.txt", "field3.txt", "enemypara.txt", "playerpara.txt"},
        new string[]{ "command4.txt", "field4.txt", "enemypara.txt", "playerpara.txt"},
        new string[]{ "command5.txt", "field5.txt", "enemypara_end.txt", "playerpara_end.txt"},
    };
    public enum TextName { CommandText, Field, Enemy, Player};
    private static List<CommandList> command;
    public static int[] commandNum;
    public static int[] probability;
    private static int[,] field;
    private static List<int[]> enemyParameter;
    private static List<int[]> playerParameter;
    public const int paraNum = 10;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //読み込みシーン番号指定で一括
    public static void SceneReader(int id)
    {
        for(int i=0; i<Enum.GetNames(typeof(TextName)).Length; i++)
        {
            ReadFile(i, fileName[id][i]);
        }
    }

    //指定ファイルの読み込み
    public static void ReadFile(int tn, string file)
    {
        string text = "";
        FileInfo fi = new FileInfo(path + file);
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            text = "すみませんです。\n読み込み失敗してしまったです。";
            return;
        }
        switch (tn)
        {
            case (int)TextName.CommandText:
                ParseCommandText(text);
                break;
            case (int)TextName.Field:
                ParseField(text);
                break;
            case (int)TextName.Enemy:
                ParseParameter(text, out enemyParameter);
                break;
            case (int)TextName.Player:
                ParseParameter(text, out playerParameter);
                break;
        }
    }

    //空行とコメントは飛ばしましょう
    private static bool ContainEmptyOrComment(string str)
    {
        /*
         * len=1: 空行
         * len=0: EOF
         */
        return str.Length <= 1 || str.IndexOf("//") != -1;
    }

    //コマンド用パース
    /*
     * 1:種類
     * 2:名前
     * 3:コマンドの説明
     * 4以降:効果量
     */
    private static void ParseCommandText(string text)
    {
        command = new List<CommandList>();
        commandNum = new int[]{ 0, 0, 0, 0, 0, 0 };

        string[] commandText;
        commandText = text.Split('\n');
        for(int i=0; i<commandText.Length; i++)
        {
            if (ContainEmptyOrComment(commandText[i])) continue;
            string[] cmd = commandText[i].Split(' ');
            if (cmd[0] == "probability")
            {
                probability = new int[5];
                for(int j=0; j<probability.Length; j++)
                {
                    probability[j] = int.Parse(cmd[j + 1]);
                }
            }
            else
            {
                int num = commandNum[int.Parse(cmd[0]) - 1]++;
                command.Add(new CommandList(cmd, num));
            }
        }
    }

    //コマンド名呼び出し
    public static string CallCommandName(int _type, int _var)
    {
        return command.Find(x => x.type.Equals(_type) && x.variety.Equals(_var)).name;
    }

    //コマンドテキスト呼び出し
    public static string CallCommandText(int _type, int _var)
    {
        //ラムダ式っていうらしいぞ
        //読み:goes to
        return command.Find(x => x.type.Equals(_type) && x.variety.Equals(_var)).text;
    }

    //コマンド効果量呼び出し
    public static int[] CallCommandEffect(int _type, int _var)
    {
        return command.Find(x => x.type.Equals(_type) && x.variety.Equals(_var)).effect;
    }

    //フィールド用パース
    private static void ParseField(string str)
    {
        string[] fieldLine;
        fieldLine = str.Split('\n');
        int column = 0;
        for(int i=0; i<fieldLine.Length; i++)
        {
            if (ContainEmptyOrComment(fieldLine[i])) continue;
            string[] cell = fieldLine[i].Split(' ');
            if(cell[0] == "max")
            {
                field = new int[int.Parse(cell[1]), int.Parse(cell[2])];
            }else
            {
                for(int row=0; row<cell.Length; row++)
                {
                    field[column, row] = int.Parse(cell[row]);
                }
                column++;
            }
        }
    }

    //フィールドサイズを測る
    public static int[] CalcField()
    {
        return new int[2]{field.GetLength(0), field.GetLength(1)};
    }

    //フィールド情報を返す
    public static int PointPanel(int x, int y)
    {
        return field[x, y];
    }

    //パラメータ用パース
    private static void ParseParameter(string str, out List<int[]> paraList)
    {
        string[] parameters;
        parameters = str.Split('\n');
        paraList = new List<int[]>();
        for (int i = 0; i < parameters.Length; i++)
        {
            if (ContainEmptyOrComment(parameters[i])) continue;
            string[] para = parameters[i].Split(' ');
            int[] one = new int[paraNum];
            string a = "";
            for(int j=0; j<para.Length; j++)
            {
                one[j] = int.Parse(para[j]);
                a += para[j];
            }
            paraList.Add(one);
        }
    }

    //該当エネミーのパラメータを返す
    public static int[] GetEnemyParameter(int ver)
    {
        return enemyParameter[ver];
    }

    //該当プレイヤーのパラメータを返す
    public static int[] GetPlayerParameter(int ver)
    {
        //エンドレスモードの場合は0番目を返す
        if(ver == 4)
        {
            ver = 0;
        }
        return playerParameter[ver];
    }



    //設定書き換え用
    public static void ReadSetting(out int spd, out int vol1)
    {
        spd = 0;
        vol1 = 50;
        string text = "";
        FileInfo fi = new FileInfo(path + "setting.txt");
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            text = "すみませんです。\n読み込み失敗してしまったです。";
            return;
        }
        string[] para = text.Split(',');
        if (para.Length != 2) return;
        spd = int.Parse(para[0]);
        vol1 = int.Parse(para[1]);
    }

    public static void RewriteSetting(int spd, int vol1)
    {
        if(spd == -1)
        {
            int __spd, __vol1;
            ReadSetting(out __spd, out __vol1);
            spd = __spd;
        }
        if (vol1 == -1)
        {
            int __spd, __vol1;
            ReadSetting(out __spd, out __vol1);
            vol1 = __vol1;
        }
        string text = spd + "," + vol1;
        FileInfo fi = new FileInfo(path + "setting.txt");
        try
        {
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(text);
            }
        }
        catch (Exception e)
        {
            text = "すみませんです。\n読み込み失敗してしまったです。";
            return;
        }
    }

    //記録専用読み込み
    public static string ReadRecord()
    {
        string text = "";
        FileInfo fi = new FileInfo(path + "record.txt");
        try
        {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            text = "すみませんです。\n読み込み失敗してしまったです。";
            return text;
        }
        return text;
    }

}
