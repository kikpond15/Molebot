using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using static toio.MathUtils.Utils;
using System.Threading.Tasks;
using System.Collections.Generic;
using TMPro;

public class ToioMouseDraggeMove : MonoBehaviour
{
    public ConnectType connectType;
    public int maxSpd = 100;
    public int rotateTime = 250;
    public int tolerance = 1;
    public int distThreshold = 10;
    public int OscClientNum = 1;
    CubeManager cubeManager;
    OSCSender oscSender;
    Vector3 targetPos, startMapPos, endMapPos, saveTargetPos;
    int matWidth, matHeight, listCtr, tarCtr;
    bool isDrawStarted, isDrawing, isDrawEnded, isReDraw;
    List<Vector2> targets = new List<Vector2>();
    List<List<Vector2>> targetsList = new List<List<Vector2>>();

    async void Start()
    {
        startMapPos = new Vector3(100, 150);      //startMapPos = new Vector3(34, 35);
        endMapPos = new Vector3(400, 350);      //endMapPos = new Vector3(339, 250);
        matWidth = ((int)endMapPos.x) - ((int)startMapPos.x);
        matHeight = ((int)endMapPos.y) - ((int)startMapPos.y);
        cubeManager = new CubeManager(connectType);
        await cubeManager.MultiConnect(1);
        targetPos = new Vector3(startMapPos.x + matWidth / 3, startMapPos.y + matHeight / 2);
        saveTargetPos = targetPos;
        oscSender = GetComponent<OSCSender>();
        Init();
    }
    void Init()
    {
        Debug.Log("Init");
        targetsList.Clear();
        targets.Clear();
        isDrawStarted = isDrawing = isDrawEnded = isReDraw = false;
        listCtr = tarCtr = 0;
        oscSender.Send(OscClientNum, 0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            isDrawStarted = true;
            targets.Clear();
            targetsList.Clear();
            Debug.Log("Start Save Dragge");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isDrawStarted = false; isDrawEnded = true;
            Debug.Log("End Save Dragge");
        }
        if (isDrawStarted) SaveDragge(startMapPos, endMapPos);
        else MoveAlongDragge();
        //showCubeInfo();

        if (Input.GetKeyDown(KeyCode.Alpha0)) oscSender.Send(OscClientNum, 0); //servo down
        else if (Input.GetKeyDown(KeyCode.Alpha1)) oscSender.Send(OscClientNum, 1);  //servo up
    }

    void SaveDragge(Vector3 sp, Vector3 ep)
    {
        if (Input.GetMouseButton(0))
        {
            isDrawing = true;
            Vector2 position = Input.mousePosition;
            Vector2 p = new Vector2(Map(position.x, 0, Screen.width, sp.x, ep.x),
                                    Map(position.y, 0, Screen.height, ep.y, sp.y));
            if (Vector3.Distance(saveTargetPos, p) > distThreshold)
            {
                targets.Add(p);
                saveTargetPos = p;
            }
            //Debug.Log("points : " + targets.Count);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing)
            {
                isDrawing = false;
                targetsList.Add(new List<Vector2>(targets));
                targets.Clear();
            }
        }
    }

    void MoveAlongDragge()
    {
        if (isDrawEnded && !isReDraw)
        {
            tarCtr = listCtr = 0;
            targetPos = targetsList[0][0];
            isReDraw = true;
            //Debug.Log("targetsList : " + targetsList.Count);
        }
        foreach (var handle in cubeManager.handles)
        {
            if (cubeManager.IsControllable(handle))
            {
                handle.Update();
                Movement mv = handle.Move2Target((int)targetPos.x, (int)targetPos.y, maxSpd, rotateTime, tolerance).Exec();
                if (mv.reached)
                {
                    if (isReDraw && listCtr < targetsList.Count)
                    {
                        SetNextTarget();
                    }
                    else if (isReDraw && listCtr == targetsList.Count)
                    {
                        oscSender.Send(1, 0);
                        isReDraw = false;
                        Debug.Log("EndReDraw");
                        Init();
                    }
                }
            }
        }
    }

    void SetNextTarget()
    {
        tarCtr += 1;
        //Debug.Log("listCtr:" + listCtr + ", tarCtr:" + tarCtr + ", targetsList:" +targetsList[listCtr].Count);
        if (targetsList[listCtr].Count > tarCtr)
        {
            targetPos = targetsList[listCtr][tarCtr];
            if (tarCtr == 2) oscSender.Send(OscClientNum, 1);
            if (tarCtr == targetsList[listCtr].Count-1) oscSender.Send(OscClientNum, 0);
        }
        else
        {
            tarCtr = 0;
            listCtr += 1;
            // if (tarCtr == 0) oscSender.Send(OscClientNum, 0);
        }
    }

    float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
    void showCubeInfo()
    {
        foreach (var handle in cubeManager.syncHandles)
        {
            Debug.Log("id:" + handle.cube.id +
                       ", pos:" + handle.cube.pos);
        }
    }
}
