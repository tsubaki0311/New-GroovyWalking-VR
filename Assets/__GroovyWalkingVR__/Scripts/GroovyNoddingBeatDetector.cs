using System;
using UnityEngine;

/// <summary>
/// 首振りに拍を検知するクラス
/// </summary>
public class GroovyNoddingBeatDetector : BaseBeatDetector
{
    [Header("Select Algorithm")]
    public BeatDetectionAlgorithm _detectionAlgorithm;

    [Header("GroovyNoddingBeatDetector")]
    [SerializeField] Transform _head;
    [Range(-1f, 2f)] public float _detectionPosFromBottomToTop = 1f;
    [Range(0f, 0.5f)] public float _detectionPosFromTop = 0f;
    public float _timeIntervalNoDetection = 0.3f;
    public bool _useFilteredVelocity;
    Vector3[] _lastHeadPositions = new Vector3[10];
    Vector3[] _lastHeadMaxPositions = new Vector3[3];
    Vector3[] _lastHeadMinPositions = new Vector3[3];
    Vector3 _midHeadPos;
    Vector3 _postHeadVelocity;
    float _lastTimeDetectGroove = 0f;

    public static GroovyNoddingBeatDetector Instance { get; protected set; }
    void Initialize()
    {
        _beatDetectionType = BeatDetectionType.GroovyNodding;
        
        // _headが未割り振りな場合
        if (_head == null)
        {
            _head = Camera.main.transform;
        }

        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if(_detectionAlgorithm == BeatDetectionAlgorithm.AsIsPrototype)
        {
            DetectBeatAsIsPrototype();
        }
        else if(_detectionAlgorithm == BeatDetectionAlgorithm.PeakDetection)
        {
            DetectBeatWhenTouchedPeak();
        }
        else if(_detectionAlgorithm == BeatDetectionAlgorithm.HalfMedianDetection)
        {
            DetectBeatUsingMedianAndBottom();
        }
        else if(_detectionAlgorithm == BeatDetectionAlgorithm.CustomPositionDetection)
        {
            DetectBeat();
        }
        else if(_detectionAlgorithm == BeatDetectionAlgorithm.FromTopDetection)
        {
            DetectBeatUsingFromTop();
        }
    }

    void FixedUpdate()
    {
        if(_detectionAlgorithm == BeatDetectionAlgorithm.AsIsPrototype)
        {
            SetHeadPosAsIsPrototype();
        }
    }

    void DetectBeatAsIsPrototype()
    {
        var posDiff = _lastHeadPositions[0] - _lastHeadPositions[1];
        var velocity = posDiff / Time.fixedDeltaTime;
        var lastHalfOfMidPoint = (_midHeadPos + _lastHeadMinPositions[0]) / 2f;
        var passedTime = Time.time - _lastTimeDetectGroove;

        if(Mathf.Abs(velocity.y) > 0.03f)
        {
            if(velocity.y > 0)
            {
                Debug.Log("<color=yellow>up</color>");
                if(velocity.y * _postHeadVelocity.y < 0)
                {
                    for(var i = _lastHeadMinPositions.Length - 1; i > 0; i--)
                    {
                        _lastHeadMinPositions[i] = _lastHeadMinPositions[i - 1];
                    }
                    _lastHeadMinPositions[0] = _head.position;
                    Debug.Log("<color=cyan><b>BOTTOM</b></color>");
                }
            }
            else if (velocity.y < 0)
            {
                Debug.Log("<color=cyan>down</color>");
                if (velocity.y * _postHeadVelocity.y < 0)
                {
                    Debug.Log($"(headY, lastHalfOfMidPointY) = ({_head.position.y}, {lastHalfOfMidPoint.y})");
                    if (_head.position.y <= lastHalfOfMidPoint.y && passedTime > _timeIntervalNoDetection)
                    {
                        _lastTimeDetectGroove = Time.time;
                        NotifyBeatDetectionForcibly(true);
                    }

                    for (var i = _lastHeadMaxPositions.Length - 1; i > 0; i--)
                    {
                        _lastHeadMaxPositions[i] = _lastHeadMaxPositions[i - 1];
                    }
                    _lastHeadMaxPositions[0] = _head.position;
                    Debug.Log("<color=yellow><b>TOP</b></color>");
                }
            }
            _postHeadVelocity = velocity;

            // update mid point
            for (var i = 0; i < _lastHeadMaxPositions.Length; i++)
            {
                _midHeadPos += _lastHeadMaxPositions[i];
            }
            for (var i = 0; i < _lastHeadMinPositions.Length; i++)
            {
                _midHeadPos += _lastHeadMinPositions[i];
            }
            _midHeadPos /= _lastHeadMaxPositions.Length + _lastHeadMinPositions.Length;
        }
    }

    void SetHeadPosAsIsPrototype()
    {
        for (var i = _lastHeadPositions.Length - 1; i > 0; i--)
        {
            _lastHeadPositions[i] = _lastHeadPositions[i - 1];
        }
        _lastHeadPositions[0] = _head.position;
    }

    void DetectBeatWhenTouchedPeak()
    {
        var velocity = GetVelocity(_useFilteredVelocity);
        var passedTime = Time.time - _lastTimeDetectGroove;

        if (Mathf.Abs(velocity.y) > 0.03f)
        {
            if (velocity.y < 0)
            {
                Debug.Log($"<color=cyan>down</color>");
                if (velocity.y * _postHeadVelocity.y < 0 && passedTime > _timeIntervalNoDetection)
                {
                    NotifyBeatDetectionForcibly(true);
                    _lastTimeDetectGroove = Time.time;
                    Debug.Log("<color=yellow><b>TOP</b></color>");
                    Debug.Log("<color=orange><b>Groove Detected</b></color>");
                    Debug.Log($"(velocityY, postVelocityY) = ({velocity.y}, {_postHeadVelocity.y})");
                }
            }

            _postHeadVelocity = velocity;
        }
        // _postHeadVelocity = velocity;

        for (var i = _lastHeadPositions.Length - 1; i > 0; i--)
        {
            _lastHeadPositions[i] = _lastHeadPositions[i - 1];
        }
        _lastHeadPositions[0] = _head.position;
    }

    void DetectBeat()
    {
        var velocity = GetVelocity(_useFilteredVelocity);

        if (Mathf.Abs(velocity.y) > 0.03f)
        {
            // 首振りの最高点・最低点に達した場合、中点を更新
            if (velocity.y * _postHeadVelocity.y < 0)
            {
                _midHeadPos = CalcMidPos(velocity);
            }

            if (velocity.y < 0)
            {
                Debug.Log("<color=cyan>down</color>");

                // 直前の最低点と最高点にかけての任意位置を拍検出点とする
                var beatDetectionPoint = Mathf.LerpUnclamped(_lastHeadMinPositions[0].y, _lastHeadMaxPositions[0].y, _detectionPosFromBottomToTop);
                var passedTime = Time.time - _lastTimeDetectGroove;

                if (_head.position.y <= beatDetectionPoint && passedTime > _timeIntervalNoDetection)
                {
                    // 拍を検知
                    NotifyBeatDetectionForcibly(true);
                    _lastTimeDetectGroove = Time.time;
                    Debug.Log($"<color=yellow><b>Groove Detected</b></color>");
                }
            }

            _postHeadVelocity = velocity;
        }

        // 座標の更新
        for (var i = _lastHeadPositions.Length - 1; i > 0; i--)
        {
            _lastHeadPositions[i] = _lastHeadPositions[i - 1];
        }
        _lastHeadPositions[0] = _head.position;
    }

    void DetectBeatUsingMedianAndBottom()
    {
        var velocity = GetVelocity(_useFilteredVelocity);

        if (Mathf.Abs(velocity.y) > 0.03f)
        {
            // 首振りの最高点・最低点に達した場合、中点を更新
            if (velocity.y * _postHeadVelocity.y < 0)
            {
                _midHeadPos = CalcMidPos(velocity);
            }

            if (velocity.y < 0)
            {
                Debug.Log("<color=cyan>down</color>");

                // 中点から直前の最低点にかけての半分の地点を拍検知点とする
                var beatDetectionPoint = (_midHeadPos + _lastHeadMinPositions[0]) / 2f;
                var passedTime = Time.time - _lastTimeDetectGroove;

                if (_head.position.y <= beatDetectionPoint.y && passedTime > _timeIntervalNoDetection)
                {
                    // 拍を検知
                    NotifyBeatDetectionForcibly(true);
                    _lastTimeDetectGroove = Time.time;
                    Debug.Log($"<color=yellow><b>Groove Detected</b></color>");
                }
            }

        }
        _postHeadVelocity = velocity;

        // 座標の更新
        for (var i = _lastHeadPositions.Length - 1; i > 0; i--)
        {
            _lastHeadPositions[i] = _lastHeadPositions[i - 1];
        }
        _lastHeadPositions[0] = _head.position;
    }

    bool _canDetect;
    void DetectBeatUsingFromTop()
    {
        var velocity = GetVelocity(_useFilteredVelocity);
        var passedTime = Time.time - _lastTimeDetectGroove;

        if(Mathf.Abs(velocity.y) > 0.03f)
        {
            if (velocity.y < 0)
            {
                Debug.Log($"<color=cyan>down</color>");
                if (velocity.y * _postHeadVelocity.y < 0)
                {
                    Debug.Log("<color=yellow><b>TOP</b></color>");
                    _canDetect = true;
                    _lastHeadMaxPositions[0] = _head.position;
                }

                if(_canDetect)
                {
                    var detectionPoint = _lastHeadMaxPositions[0] + Vector3.down * _detectionPosFromTop;
                    if(_head.position.y <= detectionPoint.y && passedTime > _timeIntervalNoDetection)
                    {
                        NotifyBeatDetectionForcibly(true);
                        _lastTimeDetectGroove = Time.time;
                        Debug.Log("<color=orange><b>Groove Detected</b></color>");
                    }
                }
            }
            else
            {
                _canDetect = false;
            }

            _postHeadVelocity = velocity;
        }

        // 座標の更新
        for (var i = _lastHeadPositions.Length - 1; i > 0; i--)
        {
            _lastHeadPositions[i] = _lastHeadPositions[i - 1];
        }
        _lastHeadPositions[0] = _head.position;

    }

    Vector3 GetVelocity(bool useFilteredValue)
    {
        var filterSize = 3;
        var posDiff = _lastHeadPositions[0] - _lastHeadPositions[1];
        var velocity = posDiff / Time.deltaTime;

        if (useFilteredValue)
        {
            velocity = Vector3.zero; // 一旦リセット
            if(filterSize > _lastHeadPositions.Length)
            {
                filterSize = _lastHeadPositions.Length;
            }

            for (var i = 0; i < filterSize; i++)
            {
                velocity += (_lastHeadPositions[i] - _lastHeadPositions[i + 1]);
            }
            velocity /= filterSize * Time.deltaTime;
        }

        return velocity;
    }

    Vector3 CalcMidPos(Vector3 velocity)
    {
        // 首の動きが上昇中
        if (velocity.y > 0)
        {
            for (var i = _lastHeadMinPositions.Length - 1; i > 0; i--)
            {
                _lastHeadMinPositions[i] = _lastHeadMinPositions[i - 1];
            }
            _lastHeadMinPositions[0] = _head.position;
        }
        else if (velocity.y < 0)
        {
            for (var i = _lastHeadMaxPositions.Length - 1; i > 0; i--)
            {
                _lastHeadMaxPositions[i] = _lastHeadMaxPositions[i - 1];
            }
            _lastHeadMaxPositions[0] = _head.position;
        }

        Vector3 sum = Vector3.zero;

        // 中点の位置を計算
        for (var i = 0; i < _lastHeadMaxPositions.Length; i++)
        {
            sum += _lastHeadMaxPositions[i];
        }
        for (var i = 0; i < _lastHeadMinPositions.Length; i++)
        {
            sum += _lastHeadMinPositions[i];
        }
        return sum /= _lastHeadMaxPositions.Length + _lastHeadMinPositions.Length;
    }
}

public enum BeatDetectionAlgorithm
{
    AsIsPrototype,
    PeakDetection,
    CustomPositionDetection,
    HalfMedianDetection,
    FromTopDetection
}