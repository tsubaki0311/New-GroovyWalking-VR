using R3;
using UnityEngine;

/// <summary>
/// 拍通知の機能を提供するクラス
/// </summary>
public abstract class BaseBeatDetector : MonoBehaviour
{
    protected BeatDetectionType _beatDetectionType = BeatDetectionType.None;
    public BeatDetectionType BeatDetectionType => _beatDetectionType;

    public ReactiveProperty<bool> OnBeatDetected { get; } = new ReactiveProperty<bool>();

    protected void NotifyBeatDetectionForcibly(bool value)
    {
        if(OnBeatDetected.CurrentValue != value)
        {
            OnBeatDetected.Value = value;
        }
        else
        {
            OnBeatDetected.ForceNotify();
        }
    }
}

public enum BeatDetectionType
{
    None,
    GroovyNodding,
    ControllerTapping
}
