using UnityEditor;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// あらゆるクラスの設定を一律で変更可能なクラス
/// </summary>
// 次はLocomotionManagerを追記
public class God : MonoBehaviour
{
    [Header("Mirror or Override")]
    [SerializeField] WhatGodDoes _whatGodDoesIs;
    [SerializeField] WhatGodDoes _whatGodDoesIsOnStart;

    [Header("LocomotionManager")]
    [SerializeField] LocomotionManager.WalkingType _walkingType;
    [SerializeField] bool _useVisuallyWalking;
    [SerializeField] bool _useAcousticWalking;
    [SerializeField] bool _canRun;
    [SerializeField] float _inputThresholdForward;
    [SerializeField] float _inputThresholdSide;
    [SerializeField] float _groovyWalkingSpeed;
    [SerializeField] float _stickInputWalkingSpeed;

    [Header("BeatDetectorManager")]
    [SerializeField] BeatDetectionType _detectorShouldDetectOn;

    [Header("ControllerTapping")]
    [SerializeField] ControllerTappingBeatDetector.HandType _tappingHand;
    [SerializeField] ControllerTappingBeatDetector.WhenButton _detectionTiming;

    [Header("GroovyNodding")]
    [SerializeField] BeatDetectionAlgorithm _detectionAlgorithm;
    [SerializeField, Range(-1f, 2f)] float _detectionPosFromBottomToTop;
    [SerializeField, Range(0f, 0.5f)] float _detectionPosFromTop;
    [SerializeField] float _timeIntervalNoDetection;
    [SerializeField] bool _useFilteredVelocity;
     
    [Header("AcousticLocomotionController")]
    [SerializeField] float _soundDelay;
    [SerializeField] bool _shouldPlayRandomly = true;
    [SerializeField, Range(0f, 1f)] float _volume = 1f;
    [SerializeField, Range(0f, 1f)] float _pitch = 1f;
    [SerializeField, Range(-1f, 1f)] float _panStereo = 0f;
    [SerializeField, Range(0f, 1f)] float _spatialBlend = 0f;
    [SerializeField, Range(0f, 1.1f)] float _reverbZoneMix = 1f;

    [Header("以下メモ")]
    [SerializeField, TextArea(10, 10)] string _memo;


    private void Start()
    {
        switch(_whatGodDoesIsOnStart)
        {
            case WhatGodDoes.Mirroring:
                MirrorEverything();
                break;
            case WhatGodDoes.Overriding:
                OverrideEverything();
                break;
            default:
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(_whatGodDoesIs == WhatGodDoes.Overriding && EditorApplication.isPlaying)
        {
            OverrideEverything();
        }
        else if(_whatGodDoesIs == WhatGodDoes.Mirroring && EditorApplication.isPlaying)
        {
            MirrorEverything();
        }
    }
#endif

    [ContextMenu("Mirror Everything")]
    void MirrorEverything()
    {
        if(BeatDetectorManager.Instance != null)
        {
            _detectorShouldDetectOn = BeatDetectorManager.Instance._shouldDetectOn;
        }

        if(LocomotionManager.Instance != null)
        {
            _walkingType = LocomotionManager.Instance._walkingType;
            _useVisuallyWalking = LocomotionManager.Instance._useVisuallyWalking;
            _useAcousticWalking = LocomotionManager.Instance._useAcousticWalking;
            _canRun = LocomotionManager.Instance._canRun;
            _inputThresholdForward = LocomotionManager.Instance._inputThresholdForward;
            _inputThresholdSide = LocomotionManager.Instance._inputThresholdSide;
            _groovyWalkingSpeed = LocomotionManager.Instance._groovyWalkingSpeed;
            _stickInputWalkingSpeed = LocomotionManager.Instance._stickInputWalkingSpeed;
        }

        if(ControllerTappingBeatDetector.Instance != null)
        {
            _tappingHand = ControllerTappingBeatDetector.Instance._tappingHand;
            _detectionTiming = ControllerTappingBeatDetector.Instance._detectionTiming;
        }

        if(GroovyNoddingBeatDetector.Instance != null)
        {
            _detectionAlgorithm = GroovyNoddingBeatDetector.Instance._detectionAlgorithm;
            _detectionPosFromBottomToTop = GroovyNoddingBeatDetector.Instance._detectionPosFromBottomToTop;
            _detectionPosFromTop = GroovyNoddingBeatDetector.Instance._detectionPosFromTop;
            _timeIntervalNoDetection = GroovyNoddingBeatDetector.Instance._timeIntervalNoDetection;
            _useFilteredVelocity = GroovyNoddingBeatDetector.Instance._useFilteredVelocity;
        }

        if(AcousticLocomotionController.Instance != null)
        {
            _soundDelay = AcousticLocomotionController.Instance._soundDelay;
            _shouldPlayRandomly = AcousticLocomotionController.Instance._shouldPlayRandomly;

            _volume = AcousticLocomotionController.Instance._volume;
            _pitch = AcousticLocomotionController.Instance._pitch;
            _panStereo = AcousticLocomotionController.Instance._panStereo;
            _spatialBlend = AcousticLocomotionController.Instance._spatialBlend;
            _reverbZoneMix = AcousticLocomotionController.Instance._reverbZoneMix;
        }
    }

    [ContextMenu("Override Everything")]
    void OverrideEverything()
    {
        if (BeatDetectorManager.Instance != null)
        {
            BeatDetectorManager.Instance._shouldDetectOn = _detectorShouldDetectOn;
        }

        if (LocomotionManager.Instance != null)
        {
            LocomotionManager.Instance._walkingType = _walkingType;
            LocomotionManager.Instance._useVisuallyWalking = _useVisuallyWalking;
            LocomotionManager.Instance._useAcousticWalking = _useAcousticWalking;
            LocomotionManager.Instance._canRun = _canRun;
            LocomotionManager.Instance._inputThresholdForward = _inputThresholdForward;
            LocomotionManager.Instance._inputThresholdSide = _inputThresholdSide;
            LocomotionManager.Instance._groovyWalkingSpeed = _groovyWalkingSpeed;
            LocomotionManager.Instance._stickInputWalkingSpeed = _stickInputWalkingSpeed;
        }   

        if (ControllerTappingBeatDetector.Instance != null)
        {
            ControllerTappingBeatDetector.Instance._tappingHand = _tappingHand;
            ControllerTappingBeatDetector.Instance._detectionTiming = _detectionTiming;
        }

        if (GroovyNoddingBeatDetector.Instance != null)
        {
            GroovyNoddingBeatDetector.Instance._detectionAlgorithm = _detectionAlgorithm;
            GroovyNoddingBeatDetector.Instance._detectionPosFromBottomToTop = _detectionPosFromBottomToTop;
            GroovyNoddingBeatDetector.Instance._detectionPosFromTop = _detectionPosFromTop;
            GroovyNoddingBeatDetector.Instance._timeIntervalNoDetection = _timeIntervalNoDetection;
            GroovyNoddingBeatDetector.Instance._useFilteredVelocity = _useFilteredVelocity;
        }

        if(AcousticLocomotionController.Instance != null)
        {
            AcousticLocomotionController.Instance._soundDelay = _soundDelay;
            AcousticLocomotionController.Instance._shouldPlayRandomly = _shouldPlayRandomly;

            AcousticLocomotionController.Instance._volume = _volume;
            AcousticLocomotionController.Instance._pitch = _pitch;
            AcousticLocomotionController.Instance._panStereo = _panStereo;
            AcousticLocomotionController.Instance._spatialBlend = _spatialBlend;
            AcousticLocomotionController.Instance._reverbZoneMix = _reverbZoneMix;
        }
    }

    enum WhatGodDoes
    {
        Nothing,
        Mirroring,
        Overriding
    }
}
