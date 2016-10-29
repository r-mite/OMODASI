using UnityEngine;
using System.Collections;

public class Parameter : MonoBehaviour
{

    //game中心ポジション
    public static Vector2 centorPosition = new Vector2(2, 2);

    //一般
    public static Vector3 localMagnification = new Vector3(1, 1, 1);
    public static Vector2[] direcrion = new Vector2[] {
        new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0)
    };

    // Use this for initialization
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {

    }

    //3D空間からゲーム内2D空間への変換
    /*

        [3D]        [ゲーム]
           2        4
        -2 + 2      |
          -2        0 - 4

    */
    /*public static Vector2 CalcFloorVec2(Vector2 vec)
    {
        return vec - centorPosition;
    }*/

    public static Vector2 CalcGameFloorToVec2(Vector2 vec)
    {
        return vec + centorPosition;
    }

    public static Vector2 CalcGameFloorToVec2(Vector3 vec)
    {
        return CalcGameFloorToVec2(ConvertToVec2(vec));
    }

    public static Vector3 Calc3DFloorToVec3(Vector2 vec)
    {
        return ConvertToVec3(vec - centorPosition);
    }

    public static Vector3 Calc3DFloorToVec3(Vector3 vec)
    {
        return Calc3DFloorToVec3(ConvertToVec2(vec));
    }

    public static Vector2 ConvertToVec2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector3 ConvertToVec3(Vector2 vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }

    //マウス移動(x,y)をカメラ移動(x, y, z)に変換
    public static Vector3 ConvertToCamera(Vector2 vec)
    {
        vec = -vec;
        return new Vector3(vec.x, vec.y * Mathf.Cos(GameCamera.slope), vec.y * Mathf.Sign(GameCamera.slope));
    }

    public static Vector3 ConvertToCamera(Vector3 vec)
    {
        return new Vector3(vec.x, vec.z * Mathf.Cos(GameCamera.slope) + vec.y * Mathf.Sign(GameCamera.slope), vec.z * Mathf.Cos(GameCamera.slope) - vec.y * Mathf.Sign(GameCamera.slope));
    }
}
