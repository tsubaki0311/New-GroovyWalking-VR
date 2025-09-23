using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField] float _rotationSpeed = 0.2f;
    [SerializeField] bool _shouldRotate = true;

    private void Update()
    {
        if (_shouldRotate)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.realtimeSinceStartup * _rotationSpeed);
        }
    }
}
