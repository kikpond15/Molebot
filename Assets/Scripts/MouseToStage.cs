using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseToStage : MonoBehaviour
{
    private Vector3 position;
    private Vector3 screenToWorldPointPosition;

    public Vector3 Get(Vector3 mouse)
    {
        position = mouse;
        // Z軸修正
        position.z = 10f;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);
        // ワールド座標に変換されたマウス座標を代入
        gameObject.transform.position = screenToWorldPointPosition;
        Vector3 p = gameObject.transform.position;

        float mapx = Map(position.x, 0, Screen.width, 0, 1);
        float mapy = Map(position.y, 0, Screen.height, 0, 1);
        //mapx = System.Math.Min(mapx, 1);
        //mapx = System.Math.Max(mapx, 0);
        //mapy = System.Math.Min(mapy, 1);
        //mapy = System.Math.Max(mapy, 0);
        //Debug.Log(mapx + ", " + mapy);

        return new Vector3(mapx, mapy);
    }

    public void DebugPrint()
    {
        Debug.Log("Screen Width : " + Screen.width);
        Debug.Log("Screen  height: " + Screen.height);
    }

    float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
}
