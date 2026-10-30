using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource; // 배경음 재생용 AudioSource
    [SerializeField] private AudioSource sfxSource; // 효과음 재생용 AudioSource
    [SerializeField] private AudioClip buttonClip; // 버튼 클릭 효과음
    [SerializeField] private AudioClip clearClip; // 게임 클리어 효과음
    [SerializeField] private float lowVolumeRatio = 0.3f; // 게임 오버 시 배경음 줄이는 비율
    private float bgmBaseVolume; // 배경음 기본 볼륨

    private void Awake()
    {
        Instance = this;
        bgmBaseVolume = bgmSource.volume; // 배경음 기본 볼륨 저장  
    }
    private void OnEnable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged; // 게임 상태 변경 이벤트 구독
        }
    }
    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged; // 게임 상태 변경 이벤트 구독 해제
        }
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return; // clip이 null이면 return
        sfxSource.PlayOneShot(clip, volume); // clip 재생
    }

    public void PlayButtonSound()
    {
        Play(buttonClip); // buttonClip 재생
    }

    private void HandleStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Clear)
        {
            bgmSource.Stop(); // 게임 클리어 시 배경음 정지
            Play(clearClip); // 게임 클리어 시 clearClip 재생
        }
        else if (newState == GameManager.GameState.GameOver)
        {
            bgmSource.volume = bgmBaseVolume * lowVolumeRatio; // 게임 오버 시 배경음 볼륨 줄이기
        }
    }

}
