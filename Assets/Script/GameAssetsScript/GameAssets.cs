using UnityEngine;
public class GameAssets:SingletonBase<GameAssets>
{
    [SerializeField, Header("资源")] private SoundAsset soundAssets;

    private void Awake()
    {
        soundAssets = Resources.Load<SoundAsset>("SoundData");
        soundAssets.InitAssets();
    }
    public void PlaySoundEffect(AudioSource audioSource, SoundType soundAssetsType)
    {
        //if (audioSource == null) Debug.Log("AudioSource是空的");
        //if (audioSource == null) Debug.Log("SoundType是空的");
        //Debug.Log("AudioSource:" + audioSource + "SoundType:" + soundAssetsType);
        audioSource.clip = soundAssets.GetAudioClip(soundAssetsType);
        audioSource.Play();
    }
}
