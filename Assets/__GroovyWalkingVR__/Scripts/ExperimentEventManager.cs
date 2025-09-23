using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ExperimentEventManager : MonoBehaviour
{
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] int _bgmIteration = 2;
    [SerializeField] AudioClip _bgmStartSound;
    [SerializeField] ActivationAreaMover _activationAreaMover;
    [SerializeField, TextArea(10, 10)] string memo;
    async void Start()
    {
        Debug.Log($"Experiment Start");

        await UniTask.Delay(3000);

        for (var i = 0; i < _bgmIteration; i++)
        {

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                _bgmSource.PlayOneShot(_bgmStartSound);
                _bgmSource.Play();
            }
#else
{
    _bgmSource.PlayOneShot(_bgmStartSound);
    _bgmSource.Play();
}
#endif


            await UniTask.WaitUntil(() => !_bgmSource.isPlaying);
            _bgmSource.Stop();
            _activationAreaMover._speed *= -1;
        }

        await UniTask.Delay(3000);

        Debug.Log($"Experiment End");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
