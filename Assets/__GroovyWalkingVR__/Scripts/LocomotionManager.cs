using R3;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class LocomotionManager : MonoBehaviour
{
    [SerializeField] BeatDetectorManager _detectorManager;
    [SerializeField] VisualLocomotionController _visualLocomotionController;
    [SerializeField] AcousticLocomotionController _acousticLocomotionController;
    [SerializeField] Transform _moveProvider;
    [SerializeField] Transform _turnProvider;
    [SerializeField] InputProvider _inputProvider;

    [Header("GroovyWalking or StickInputWalking")]
    public WalkingType _walkingType = WalkingType.GroovyWalking;
    ReactiveProperty<bool> _onSetUseGroovyWalking;
    public bool _canRun = true;
    bool _useGroovyWalking;

    [Header("Groovy Walking Settings")]
    public bool _useVisuallyWalking = true;
    public bool _useAcousticWalking = true;
    public float _inputThresholdForward = 0.3f;
    public float _inputThresholdSide = 0.3f;
    public float _groovyWalkingSpeed = 1.0f;
    ReactiveProperty<bool> _onPressedLeftStick;
    bool _shouldRun;
    float _lastTimeShouldRun = 0f;

    [Header("Stick Input Walking Settings")]
    public float _stickInputWalkingSpeed = 1f;
    ReactiveProperty<float> _onSetStickInputWalkingSpeed;
    float _lastTimeStepSoundPlayed = 0f;

    public BeatDetectionType CurrentDetectionType => _detectorManager._shouldDetectOn;


    public static LocomotionManager Instance { get; private set; }

    private void Initialize()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _onSetStickInputWalkingSpeed = new ReactiveProperty<float>(_stickInputWalkingSpeed);
        _onSetUseGroovyWalking = new ReactiveProperty<bool>(_useGroovyWalking);
        _onPressedLeftStick = new ReactiveProperty<bool>(false);
    }

    private void Awake()
    {
        Initialize();

        
        _detectorManager.OnBeatDetected.Where(value => value).Subscribe(_ =>
        {
            if(_useGroovyWalking)
            {
                if (_useVisuallyWalking)
                {
                    if (Mathf.Abs(_inputProvider.LeftStickValue.y) > _inputThresholdForward)
                    {
                        var direction = _visualLocomotionController.XrOrigin.forward;
                        var speed = _inputProvider.LeftStickValue.y * _groovyWalkingSpeed;
                        if(_canRun && _shouldRun)
                        {
                            speed *= 2f;
                        }
                        var duration = 0.5f;
                        var easingType = DG.Tweening.Ease.InOutSine;

                        _visualLocomotionController.WalkOneStepOnBeatDetected(direction, speed, duration, easingType);
                    }
                }

                if(_useAcousticWalking)
                {
                    _acousticLocomotionController.PlayStepSoundOnBeatDetected();
                }

            }
        }).AddTo(this);

        _onSetUseGroovyWalking.DistinctUntilChanged().Subscribe(value =>
        {
            // 実験では使わないのでコメントアウト
            // _moveProvider.gameObject.SetActive(!value);
            // _turnProvider.gameObject.SetActive(!value);
        }).AddTo(this);

        _onSetStickInputWalkingSpeed.DistinctUntilChanged().Subscribe(speed =>
        {
            var moveProviderObj = _moveProvider.gameObject;
            if (!_useGroovyWalking && moveProviderObj.activeSelf)
            {
                moveProviderObj.GetComponent<DynamicMoveProvider>().moveSpeed = speed;
            }
        }).AddTo(this);

        _onPressedLeftStick.DistinctUntilChanged().Where(value => value).Subscribe(_ =>
        {
            if(_canRun)
            {
                _shouldRun = !_shouldRun;
            }
            else
            {
                _shouldRun = false;
            }
            
        }).AddTo(this);
    }

    private void Update()
    {
        _onSetStickInputWalkingSpeed.Value = _stickInputWalkingSpeed;

        if(_walkingType == WalkingType.GroovyWalking)
        {
            _useGroovyWalking = true;
        }
        else if(_walkingType == WalkingType.StickInputWalking)
        {
            _useGroovyWalking = false;
        }

        _onSetUseGroovyWalking.Value = _useGroovyWalking;
        _onPressedLeftStick.Value = _inputProvider.IsLeftStickPressed;


        if(Time.time - _lastTimeShouldRun > 0.3f && _inputProvider.LeftStickValue == Vector2.zero)
        {
            _shouldRun = false;
        }

        if(_shouldRun)
        {
            _lastTimeShouldRun = Time.time;
        }

        if (_walkingType == WalkingType.StickInputWalking)
        {
            // 左スティックが前に倒されている時には前に、後ろに倒され散るときは後ろに移動
            // 左右は動かない
            _visualLocomotionController.XrOrigin.position += 
                _visualLocomotionController.XrOrigin.forward * _inputProvider.LeftStickValue.y * _stickInputWalkingSpeed * Time.deltaTime;
            if (Mathf.Abs(_inputProvider.LeftStickValue.y) > _inputThresholdForward)
            {
                if (Time.time - _lastTimeStepSoundPlayed > 0.45f)
                {
                    _acousticLocomotionController.PlayStepSound();
                    _lastTimeStepSoundPlayed = Time.time;
                }
            }

        }
    }

    public enum WalkingType
    {
        GroovyWalking,
        StickInputWalking
    }
}
