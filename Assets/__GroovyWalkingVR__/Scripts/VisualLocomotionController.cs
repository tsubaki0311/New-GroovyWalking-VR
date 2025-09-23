using UnityEngine;
using DG.Tweening;
using Unity.XR.CoreUtils;

public class VisualLocomotionController : MonoBehaviour
{
    [SerializeField] Camera _head;
    float _speed = 1.0f;
    float _duration = 0.5f;
    Ease _easingType = Ease.InOutSine;
    Transform _xrOrigin;
    public Transform XrOrigin => _xrOrigin;

    public static VisualLocomotionController Instance { get; private set; }


    void Initialize()
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

        // 処理重いらしいけど、初期化時に一回なら良いだろう
        _xrOrigin = GameObject.FindObjectOfType<XROrigin>().Origin.transform;
        if(_xrOrigin == null )
        {
            Debug.LogError("シーンにXROriginが存在しません。");
        }
    }

    private void Awake()
    {
        Initialize();
    }

    // オーバーロード
    public void WalkOneStepOnBeatDetected(Vector3 direction ,float speed, float duration, Ease easingType)
    {
        _xrOrigin.DOMove(_xrOrigin.position + direction * speed, duration).SetEase(easingType);
    }

}
