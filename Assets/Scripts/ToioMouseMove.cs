using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using static toio.MathUtils.Utils;

public class ToioMouseMove : MonoBehaviour
{
    public ConnectType connectType;
    public int maxSpd = 50;
    public int rotateTime = 250;
    public int tolerance = 8;
    CubeManager cubeManager;

    Vector3 targetPos, startMapPos, endMapPos;
    int matWidth, matHeight;
    OSCSender oscSender;
    public int OscClientNum = 1;
    private float _repeatSpan;    //繰り返す間隔
    private float _timeElapsed;   //経過時間
    int sendData = 1;
    int phase = 0;

    async void Start()
    {
        startMapPos = new Vector3(100, 150);      //startMapPos = new Vector3(34, 35);
        endMapPos = new Vector3(400, 350);      //endMapPos = new Vector3(339, 250);
        matWidth = ((int)endMapPos.x) - ((int)startMapPos.x);
        matHeight = ((int)endMapPos.y) - ((int)startMapPos.y);

        cubeManager = new CubeManager(connectType);
        await cubeManager.MultiConnect(1);
        targetPos = new Vector3(startMapPos.x + matWidth / 2, startMapPos.y + matHeight / 2);
        oscSender = GetComponent<OSCSender>();
        oscSender.Send(OscClientNum, sendData);
         _repeatSpan = 0.5f;    //実行間隔を５に設定
        _timeElapsed = 0;
    }

    void Update()
    {


        if(Input.GetKeyDown(KeyCode.S))
        {
            phase++;
        }

        if(phase == 0)
        {
            Move(startMapPos, endMapPos);
        }
        else if(phase == 1)
        {
            Raund();
            ServoPiston();
        }
        //showCubeInfo();        
    }

    void ServoPiston()
    {
         _timeElapsed += Time.deltaTime;     //時間をカウントする
        //経過時間が繰り返す間隔を経過したら
        if (_timeElapsed >= _repeatSpan)
        {
            if(sendData == 0) sendData = 1;
            else if(sendData == 1) sendData = 0;

            oscSender.Send(OscClientNum, sendData);
            _timeElapsed = 0;   //経過時間をリセットする
        }
    }

    void Move(Vector3 sp, Vector3 ep)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Input.mousePosition;
            targetPos.x = Map(position.x, 0, Screen.width, sp.x, ep.x);
            targetPos.y = Map(position.y, 0, Screen.height, ep.y, sp.y);
            Debug.Log(targetPos.x + ", " + targetPos.y);
        }
         foreach (var handle in cubeManager.handles)
        {
            if (cubeManager.IsControllable(handle))
            {
                handle.Update();
                Movement mv = handle.Move2Target((int)targetPos.x, (int)targetPos.y, maxSpd, rotateTime, tolerance).Exec(); 
            }
        }
    }
    void Raund()
    {
        foreach (var handle in cubeManager.handles)
        {
            if (cubeManager.IsControllable(handle))
            {
                //handle.Update();
                handle.MoveRaw(20, -20, 1000);
            }
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
