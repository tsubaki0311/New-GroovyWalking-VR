using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;

public class TwinklingCrossManager : MonoBehaviour
{
    [SerializeField] float _basicVisibleTime = 10.0f;
    [SerializeField] float _invisibleTime = 15.0f;
    [SerializeField] float _activationAreaScale = 100.0f;
    Transform _thisCross;
    TextMeshProUGUI _thisCrossUGUI;
    Transform _activationArea;
    bool _isVisible;
    ReactiveProperty<bool> _canVisible = new ReactiveProperty<bool>(false);

    private async void Awake()
    {
        // ActivationAreaMover ÇÃ Awake Ç…Ç®Ç¢ÇƒÅAÉCÉìÉXÉ^ÉìÉXÇ™äiî[Ç≥ÇÍÇÈÇÃÇë“Ç¬
        await UniTask.Yield();
        _activationArea = ActivationAreaMover.Instance.ActivationArea;

        // é©ï™é©êgÇäiî[
        _thisCross = this.transform;
        _thisCrossUGUI = this.GetComponent<TextMeshProUGUI>();
        _thisCrossUGUI.DOFade(0f, 0f);
    }

    private void Start()
    {
        _canVisible.Where(value => value).Subscribe(async value =>
        {
            while (value)
            {
                _thisCrossUGUI.DOFade(1f, 0.2f);
                _isVisible = true;
                var thisVisibleTime = _basicVisibleTime + UnityEngine.Random.Range(0f, 3f);
                await UniTask.Delay(Mathf.RoundToInt(thisVisibleTime * 1000f));

                _thisCrossUGUI.DOFade(0f, 0.2f);
                _isVisible = false;
                await UniTask.Delay(Mathf.RoundToInt(_invisibleTime * 1000f));

                if (_canVisible.Value == false) break;
            }
        });

        _canVisible.Where(value => !value).Subscribe(async _ =>
        {
            if (_isVisible) 
            {
                await UniTask.Delay(3000); 
                _thisCrossUGUI.DOFade(0f, 0.2f);
                _isVisible = false;
            }
        });
    }

    private void Update()
    {
        var distance = Mathf.Abs(_thisCross.position.x - _activationArea.position.x);
        if (distance < _activationAreaScale / 2.0f)
        {
            _canVisible.Value = true;
        }
        else
        {
            _canVisible.Value = false;
        }
    }
}
