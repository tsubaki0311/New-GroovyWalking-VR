using UnityEngine;

public class SineMoveController : MonoBehaviour
{
    [SerializeField] float _amplitude = 1f;
    [SerializeField] float _frequency = 1f;
    [SerializeField] Vector3 _wavingOrigin = Vector3.zero;
    [SerializeField] Transform _target;
    void Update()
    {
        var pos = _target.position;
        pos.y = _amplitude * Mathf.Sin(Time.time * _frequency);
        _target.position = pos + _wavingOrigin;
    }
}
