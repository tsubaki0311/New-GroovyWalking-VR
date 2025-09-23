using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 指によるボタンタップに拍を検知するクラス
/// </summary>
public class ControllerTappingBeatDetector : BaseBeatDetector
{
    public HandType _tappingHand = HandType.Right;
    public WhenButton _detectionTiming = WhenButton.IsReleased;
    [SerializeField] InputProvider _inputProvider;
    bool _isPressedLastTime = false;
    public static ControllerTappingBeatDetector Instance { get; protected set; }

    void Initialize()
    {
        _beatDetectionType = BeatDetectionType.ControllerTapping;
        
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

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        DetectBeat(_tappingHand);
    }

    void DetectBeat(HandType handType)
    {
        bool isPressed = false;
        switch(handType)
        {
            case HandType.Left:
                isPressed = _inputProvider.IsLeftButtonPressed;
                break;
            case HandType.Right:
                isPressed = _inputProvider.IsRightButtonPressed;
                break;
            default:
                Debug.LogError("右か左、どちらか一方を選択してください。");
                break;
        }

        // left と right の共通部分
        if (_detectionTiming == WhenButton.IsPressed)
        {
            if (isPressed && !_isPressedLastTime)
            {
                // ボタンを押した瞬間に拍
                NotifyBeatDetectionForcibly(true);
            }
        }
        else if (_detectionTiming == WhenButton.IsReleased)
        {
            // ボタンから指を話した瞬間に拍
            if (!isPressed && _isPressedLastTime)
            {
                NotifyBeatDetectionForcibly(true);
            }
        }

        switch (handType)
        {
            case HandType.Left:
                _isPressedLastTime = _inputProvider.IsLeftButtonPressed;
                break;
            case HandType.Right:
                _isPressedLastTime = _inputProvider.IsRightButtonPressed;
                break;
        }
    }

    public enum HandType
    {
        Right,
        Left
    }

    public enum WhenButton
    {
       IsPressed,
       IsReleased
    }
}
