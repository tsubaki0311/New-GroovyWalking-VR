using UnityEngine;

public class ActivationAreaMover : MonoBehaviour
{
    [SerializeField] Transform _activationArea;
    [SerializeField] Vector3 _moveDirection;
    public float _speed = 1.0f;
    [SerializeField] bool _shouldMove;

    public static ActivationAreaMover Instance;
    public Transform ActivationArea => _activationArea;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(_shouldMove)
        {
            _activationArea.position += _moveDirection * _speed * Time.deltaTime;
        }
    }
}
