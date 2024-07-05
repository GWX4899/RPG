using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="SoundData",menuName = "Assets/Resources")]
public class SoundAsset:ScriptableObject
{
    public List<SoundAssets> assets = new List<SoundAssets>();
    private Dictionary<string, AudioClip[]> assetsDictionary = new Dictionary<string, AudioClip[]>();

    public void InitAssets()
    {
        for (int i = 0; i < assets.Count; i++)
        {
            if (!assetsDictionary.ContainsKey(assets[i].assetaName))
            {
                assetsDictionary.Add(assets[i].assetaName, assets[i].assetsClip);
                //Debug.Log(assetsDictionary[assets[i].assetaName].Length);
            }
        }
    }

    public AudioClip GetAudioClip(SoundType sound)
    {
        switch (sound) 
        {
            case SoundType.Hurt:
                return assetsDictionary["Hurt"][Random.Range(0, assetsDictionary["Hurt"].Length)];
            case SoundType.SwordWave:
                //Debug.Log(1);
                return assetsDictionary["SwordWave"][Random.Range(0, assetsDictionary["SwordWave"].Length)];
            case SoundType.GSwordWave:
                //Debug.Log(2);
                return assetsDictionary["GSwordWave"][Random.Range(0, assetsDictionary["GSwordWave"].Length)];
            case SoundType.SwordDefend:
                return assetsDictionary["SwordDefend"][Random.Range(0, assetsDictionary["SwordDefend"].Length)];
            case SoundType.GSwordDefend:
                return assetsDictionary["GSwordDefend"][Random.Range(0, assetsDictionary["GSwordDefend"].Length)];
            default:
                //Debug.Log("没有");
                return null;
        }

    }
    public void PlaySoundEffect(AudioSource audiosource, SoundType soundType)
    {
        audiosource.clip = GetAudioClip(soundType);
        audiosource.Play();
    }


    [System.Serializable]
    public class SoundAssets
    {
        public string assetaName;
        public AudioClip[] assetsClip;
    }
}

