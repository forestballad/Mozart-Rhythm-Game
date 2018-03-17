using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
    public float updateInterval = 0.5F;

    // Update() fps
    public Text updateFpsText;
    // FixUpdate() fps
    public Text fixedUpdateFpsText;
    // Input update fps maybe upper than 60, if we need high-precision judgement
    public Text inputUpdateFpsText;
    // Logic update fps maybe lower than inputUpdate fps, if we need average performance with high-precision judgement
    public Text logicUpdateFpsText;

    private enum FpsType
    {
        Update = 0,
        FixedUpdate,
        InputUpdate,
        LogicUpdate,
        Num
    }

    private double[] _lastInterval = new double[(int)FpsType.Num];
    private int[] _frames = new int[(int)FpsType.Num];
    private float[] _fps = new float[(int)FpsType.Num];
    private Text[] _fpsTexts = new Text[(int)FpsType.Num];
    private string[] _updateTypeTexts = new []
    {
        "UpdateFPS : ",
        "FixedUpdateFPS : ",
        "InputUpdateFPS : ",
        "LogicUpdateFPS : ",
    };
    void Start()
    {
        for (int i = 0; i < (int)FpsType.Num; i++)
        {
            _lastInterval[i] = Time.realtimeSinceStartup;
        }
        _fpsTexts[(int)FpsType.Update] = updateFpsText;
        _fpsTexts[(int)FpsType.FixedUpdate] = fixedUpdateFpsText;
        _fpsTexts[(int)FpsType.InputUpdate] = inputUpdateFpsText;
        _fpsTexts[(int)FpsType.LogicUpdate] = logicUpdateFpsText;
    }

    void Update()
    {
        UpdateFps(FpsType.Update);
        UpdateFps(FpsType.InputUpdate);
        UpdateFps(FpsType.LogicUpdate);
    }

    void FixedUpdate()
    {
        UpdateFps(FpsType.FixedUpdate);
    }

    private void UpdateFps(FpsType fpsType)
    {
        var type = (int) fpsType;
        float timeNow = Time.realtimeSinceStartup;
        ++_frames[type];
        if (timeNow > _lastInterval[type] + updateInterval)
        {
            _fps[type] = (float)(_frames[type] / (timeNow - _lastInterval[type]));
            _frames[type] = 0;
            _lastInterval[type] = timeNow;

            if (_fpsTexts[type])
                _fpsTexts[type].text = _updateTypeTexts[type] + _fps[0].ToString("f2");
        }
    }
}