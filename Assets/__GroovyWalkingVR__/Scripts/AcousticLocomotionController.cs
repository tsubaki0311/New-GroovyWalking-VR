using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AcousticLocomotionController : MonoBehaviour
{
    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip[] _stepSounds;
    public float _soundDelay = 0.0f;
    public bool _shouldPlayRandomly = true;
    int _postRandomValue = -1; // do-whileのwhileで被らない初期値

    [Header("Audio Settings")] // 多分いじりたくなるAudioSourceの変数
    [Range(0f, 1f)] public float _volume = 1f;
    [Range(0f, 1f)] public float _pitch = 1f;
    [Range(-1f, 1f)] public float _panStereo = 0f;
    [Range(0f, 1f)] public float _spatialBlend = 1f;
    [Range(0f, 1.1f)] public float _reverbZoneMix = 1f; 

    public static AcousticLocomotionController Instance { get; private set; }

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (_audio == null)
        {
            // 再生機能を追加
            var camera = Camera.main.gameObject;
            if (camera.TryGetComponent<AudioSource>(out var audioSource))
            {
                _audio = audioSource;
            }
            else
            {
                _audio = camera.AddComponent<AudioSource>();
            }
            
            _audio.playOnAwake = false;
            _audio.volume = _volume;
            _audio.pitch = _pitch;
            _audio.panStereo = _panStereo;
            _audio.spatialBlend = _spatialBlend;
            _audio.reverbZoneMix = _reverbZoneMix;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    public async void PlayStepSoundOnBeatDetected()
    {
        await UniTask.WaitForSeconds(_soundDelay);
        _audio.PlayOneShot(GetSound(_stepSounds));
    }

    public async void PlayStepSound()
    {
        await UniTask.WaitForSeconds(_soundDelay);
        _audio.PlayOneShot(GetSound(_stepSounds));
    }

    AudioClip GetSound(AudioClip[] sounds, int value = 0)
    {
        if(_shouldPlayRandomly)
        {
            int randomValue = 0;
            do
            {
                randomValue = UnityEngine.Random.Range(0, sounds.Length);
            } while (randomValue == _postRandomValue);
            _postRandomValue = randomValue;
            
            return sounds[randomValue];
            
        }
        else
        {
            return sounds[value];
        }


    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(EditorApplication.isPlaying && _audio != null)
        {
            _audio.volume = _volume;
            _audio.pitch = _pitch;
            _audio.panStereo = _panStereo;
            _audio.spatialBlend = _spatialBlend;
            _audio.reverbZoneMix = _reverbZoneMix;
        }
    }
#endif
}
