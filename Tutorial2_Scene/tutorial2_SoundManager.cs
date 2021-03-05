using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundNamd;
    public AudioClip Clip;

}


public class tutorial2_soundManager : MonoBehaviour
{
    [Header("사운드 등록")]
    [SerializeField] Sound[] bgmSounds;

    [Header("브금 플레이어")]
    [SerializeField] AudioSource bgmPlayer;

    public int bgmCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        PlayFirstBGM();
    }

    public void PlayRandomBGM()//랜덤으로 브금 실행
    {
        int random = Random.Range(0,1);//0과 1중에 브금 랜덤으로 설정
        bgmPlayer.clip = bgmSounds[random].Clip;//클립 불러옴
        bgmPlayer.Play();

    }

    public void PlayFirstBGM()
    {
        bgmPlayer.clip = bgmSounds[bgmCount].Clip;
        bgmPlayer.Play();
    }

    public void PlayNextBGM()
    {
        bgmCount++;
        bgmPlayer.clip = bgmSounds[bgmCount].Clip;
        bgmPlayer.Play();
    }


}
