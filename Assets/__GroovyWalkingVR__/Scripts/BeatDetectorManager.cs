using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

/// <summary>
/// 他のBeatDetectorが拍を検知したタイミングに拍を検知するクラス
/// 拍検知の方法によらず、拍検知のタイミングとその方法を取得できる
/// </summary>
public class BeatDetectorManager : MonoBehaviour
{
    [SerializeField] GroovyNoddingBeatDetector _noddingBeatDetector;
    [SerializeField] ControllerTappingBeatDetector _tappingBeatDetector;
    public BeatDetectionType _shouldDetectOn;
    public ReactiveProperty<bool> OnBeatDetected { get; } = new ReactiveProperty<bool>();
    public static BeatDetectorManager Instance { get; protected set; }

    private void Initialize()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (_shouldDetectOn == BeatDetectionType.None)
        {
            Debug.LogError("拍検知の手法を指定してください");
        }
    }

    // 何が最適か分からないので、指し当たり二種類の実装をしておく
    #region 実装A : Managerから拍検知の手法を指定し、その仕方でのみタイミングを取得可能な実装
    private void Awake()
    {
        Initialize();

        _tappingBeatDetector.OnBeatDetected.Where(value => value).Subscribe(_ =>
        {
            if (_shouldDetectOn == BeatDetectionType.ControllerTapping)
            {
                NotifyBeatDetectionForcibly(true);
            }
        }).AddTo(this);

        _noddingBeatDetector.OnBeatDetected.Where(value => value).Subscribe(_ =>
        {
            if(_shouldDetectOn == BeatDetectionType.GroovyNodding)
            {
                this.NotifyBeatDetectionForcibly(true);
            }
        }).AddTo(this);
    }

    #endregion

    #region 実装B : 拍検知の方法によらず、タイミングとその仕方が取得できる実装
    /*
    [SerializeField] BaseBeatDetector[] _beatDetectors;

    private void Awake()
    {
        Initialize();
        foreach (var beatDetector in _beatDetectors)
        {
            beatDetector.OnBeatDetected.Where(value => value).Subscribe(value =>
            {
                // 送られてきた信号のタイプを取得
                // BeatDetectorManagerを参照するだけで、拍検知のタイミングと方法を取得できる
                this._beatDetectionType = beatDetector.BeatDetectionType;
                NotifyBeatDetectionForcibly(true);
            }).AddTo(this);
        }
    }
    */
    #endregion

    
    protected void NotifyBeatDetectionForcibly(bool value)
    {
        if (OnBeatDetected.CurrentValue != value)
        {
            OnBeatDetected.Value = value;
        }
        else
        {
            OnBeatDetected.ForceNotify();
        }
    }

}