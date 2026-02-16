using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioEventChannel canalMusica;
    public AudioMixer mixer;

    // PENDIENTE: Cargar estos datos desde memoria (PlayerPrefs) para que se mantengan entre sesiones la preferencias del jugador
    public float volumenMusica = 0.1f;
    public float volumenSfx = 1f;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private static AudioManager instance;

    private void Awake()
    {
        // --- Lógica Singleton (Para que no se destruya) ---
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

    private void ConfigurarFuentes()
    {
        // 1. Configurar la fuente de Música
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = volumenMusica;

        // 2. Crear y configurar la fuente de SFX
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = false;
        _sfxSource.playOnAwake = false;
        _musicSource.volume = volumenSfx;

        // 3. Conectar cada fuente a su grupo del Mixer
        if (mixer != null)
        {
            // Buscamos los grupos en el Mixer. Asegúrate de que se llamen "Musica" y "SFX"
            AudioMixerGroup[] gruposMusica = mixer.FindMatchingGroups("Musica");
            AudioMixerGroup[] gruposSFX = mixer.FindMatchingGroups("SFX");

            if (gruposMusica.Length > 0) _musicSource.outputAudioMixerGroup = gruposMusica[0];
            if (gruposSFX.Length > 0) _sfxSource.outputAudioMixerGroup = gruposSFX[0];
        }
        else
        {
            Debug.LogWarning("¡Ojo! No has asignado el Mixer al AudioManager en el Inspector.");
        }
    }

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

        // Si es la misma canción que ya suena, no hacemos nada
        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // Usamos la fuente de SFX para que los efectos no corten la música
        // y para que respondan al Slider de SFX
        _sfxSource.PlayOneShot(clip);
    }
}