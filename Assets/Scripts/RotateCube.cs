using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;

// キューブの回転
public class RotateCube : MonoBehaviour
{
    public ConnectType connectType;
    public int connectNum = 1;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;
    CubeManager cm;
    OSCSender oscSender;
    public int OscClientNum = 1;
    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(connectType);
        await cm.MultiConnect(connectNum);

        foreach (var navi in cm.navigators)
        {
            navi.usePred = true;
            navi.mode = naviMode;
        }
        oscSender = GetComponent<OSCSender>();
        oscSender.Send(OscClientNum, 0);
    }


    void Update()
    {
        //foreach (var cube in cm.syncCubes)
        //{
        //    cube.Move(50, -50, 100);
        //}

        var tar = Vector.fromRadMag(Time.time / 1, 60) + new Vector(250, 250);
        if (cm.synced)
        {
            for (int i = 0; i < cm.navigators.Count; i++)
            {
                var navi = cm.navigators[i];
                navi.mode = naviMode;
                var mv = navi.Navi2Target(tar, maxSpd: 30).Exec();
                oscSender.Send(OscClientNum, 1);
            }
        }
    }

}