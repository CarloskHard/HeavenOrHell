using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioEventChannel canalMusica; // Asumo que esto es un ScriptableObject tuyo
    public AudioMixer mixer;

    // Nombres de los parámetros expuestos en el Mixer
    private const string MIXER_MASTER_VOL = "MasterVol";
    private const string MIXER_MUSIC_VOL = "MusicVol";
    private const string MIXER_SFX_VOL = "SfxVol";

    [Range(0.0001f,1f)]
    public float volumenMaster = 1f;
    [Range(0.0001f, 1f)]
    public float volumenMusica = 0.5f;
    [Range(0.0001f, 1f)]
    public float volumenSfx = 1f;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ConfigurarFuentes();
    }

    private void Start()
    {
        // Aplicamos el volumen inicial al Mixer al arrancar
        // Se hace en Start para asegurar que el Mixer ya ha inicializado
        SetMusicVolume(volumenMusica);
        SetSFXVolume(volumenSfx);
    }

    private void ConfigurarFuentes()
    {
        // 1. Configurar la fuente de Música
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        // Dejamos el AudioSource al 100% y controlamos la mezcla en el Mixer
        _musicSource.volume = 1f;

        // 2. Configurar la fuente de SFX
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;     // CORREGIDO: ahora usa _sfxSource
        _sfxSource.playOnAwake = false;
        _sfxSource.volume = 1f;      // Dejamos el AudioSource al 100%

        // 3. Conectar al Mixer
        if (mixer != null)
        {
            //Debe tener mismo nombre que el canal. Ojo porque no hay verificación de que exista, así que mejor revisar el nombre en el Mixer
            AudioMixerGroup[] gruposMusica = mixer.FindMatchingGroups("Music");
            AudioMixerGroup[] gruposSFX = mixer.FindMatchingGroups("SFX");

            if (gruposMusica.Length > 0) _musicSource.outputAudioMixerGroup = gruposMusica[0];
            if (gruposSFX.Length > 0) _sfxSource.outputAudioMixerGroup = gruposSFX[0];
        }
        else
        {
            Debug.LogWarning("¡Ojo! No has asignado el Mixer al AudioManager.");
        }
    }

    // --- FUNCIONES PARA ACTUALIZAR VOLUMEN DINÁMICAMENTE ---
    public void SetMasterVolume(float volume)
    {
        volumenMaster = volume;
        ApplyVolumeToMixer(MIXER_MUSIC_VOL, volume);
    }

    public void SetMusicVolume(float volume)
    {
        volumenMusica = volume;
        ApplyVolumeToMixer(MIXER_MUSIC_VOL, volume);
    }

    public void SetSFXVolume(float volume)
    {
        volumenSfx = volume;
        ApplyVolumeToMixer(MIXER_SFX_VOL, volume);
    }

    private void ApplyVolumeToMixer(string parameterName, float linearVolume)
    {
        if (mixer == null) return;

        // Conversión mágica: De Lineal (0 a 1) a Decibelios (-80 a 0)
        // Usamos Log10. Si el volumen es 0, logaritmo da error, así que ponemos -80dB (silencio)
        float dbVolume = (linearVolume <= 0.0001f) ? -80f : Mathf.Log10(linearVolume) * 20;

        mixer.SetFloat(parameterName, dbVolume);
    }

    // --------------------------------------------------------

    private void OnEnable()
    {
        if (canalMusica != null)
        {
            canalMusica.OnMusicRequested += PlayMusic;
            canalMusica.OnSfxRequested += PlaySFX;
        }
    }

    private void OnDisable()
    {
        if (canalMusica != null)
        {
            canalMusica.OnMusicRequested -= PlayMusic;
            canalMusica.OnSfxRequested -= PlaySFX;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (_musicSource.clip == clip && _musicSource.isPlaying) return;

        _musicSource.clip = clip;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }
}