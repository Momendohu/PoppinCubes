using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder (-100)]
public class AudioManager : SingletonMonoBehaviour<AudioManager> {
    private const int BGM_NUM = 5;
    private const int SE_NUM = 1;

    private List<AudioSource> bgmSourceList = new List<AudioSource> ();
    private List<AudioSource> seSourceList = new List<AudioSource> ();

    private Dictionary<string, AudioClip> bgmDic = new Dictionary<string, AudioClip> ();
    private Dictionary<string, AudioClip> seDic = new Dictionary<string, AudioClip> ();

    public Dictionary<string, float> bgmBPM = new Dictionary<string, float> ();

    //bpmデータ追加用
    private int addbmpNum;
    private float[] addbpmData = { 136, 1, 1, 1, 1, 1 };
    private float AddBPM () {
        addbmpNum++;
        return addbpmData[addbmpNum - 1];
    }

    private void Init () {
        //SE、BGMの数分だけAudioSourceを追加
        for (int i = 0; i < BGM_NUM + SE_NUM; i++) {
            gameObject.AddComponent<AudioSource> ();
        }

        AudioSource[] audioSourceArray = GetComponents<AudioSource> ();

        for (int i = 0; i < audioSourceArray.Length; i++) {
            audioSourceArray[i].playOnAwake = false;

            //BGM、SE設定
            if (i < BGM_NUM) {
                audioSourceArray[i].loop = true;
                bgmSourceList.Add (audioSourceArray[i]);
            } else {
                seSourceList.Add (audioSourceArray[i]);
            }
        }

        object[] bgmData = Resources.LoadAll ("Audio/BGM");
        object[] seData = Resources.LoadAll ("Audio/SE");

        foreach (AudioClip bgm in bgmData) {
            bgmDic[bgm.name] = bgm;
            bgmBPM[bgm.name] = AddBPM ();
        }

        foreach (AudioClip se in seData) {
            seDic[se.name] = se;
        }
    }

    private void Awake () {
        //Instance化をすでにしてるなら
        if (this != Instance) {
            Destroy (this);
            return;
        }

        DontDestroyOnLoad (this.gameObject);
        Init ();
    }

    /// <summary>
    /// seをならす
    /// </summary>
    public void PlaySE (string name) {
        if (!seDic.ContainsKey (name)) {
            return;
        }

        foreach (AudioSource se in seSourceList) {
            se.PlayOneShot (seDic[name] as AudioClip);
            return;
        }
    }

    /// <summary>
    /// bgmをならす
    /// </summary>
    public void PlayBGM (string name, bool isLoop) {
        if (!bgmDic.ContainsKey (name)) {
            return;
        }

        for (int i = 0; i < bgmSourceList.Count; i++) {
            if (!bgmSourceList[i].isPlaying) {
                bgmSourceList[i].clip = bgmDic[name] as AudioClip;
                bgmSourceList[i].loop = isLoop;
                bgmSourceList[i].Play ();
                return;
            }
        }
    }

    /// <summary>
    /// bgmをならす
    /// </summary>
    public void PlayBGM (string name, bool isLoop, float volume) {
        if (!bgmDic.ContainsKey (name)) {
            return;
        }

        for (int i = 0; i < bgmSourceList.Count; i++) {
            if (!bgmSourceList[i].isPlaying) {
                bgmSourceList[i].clip = bgmDic[name] as AudioClip;
                bgmSourceList[i].loop = isLoop;
                bgmSourceList[i].volume = volume;
                bgmSourceList[i].Play ();
                return;
            }
        }
    }

    /// <summary>
    /// bgmを止める
    /// </summary>
    public void StopBGM (string name) {
        if (!bgmDic.ContainsKey (name)) {
            return;
        }

        for (int i = 0; i < bgmSourceList.Count; i++) {
            if (bgmSourceList[i].isPlaying) {
                bgmSourceList[i].clip = bgmDic[name] as AudioClip;
                bgmSourceList[i].Stop ();
                return;
            }
        }
    }

    /// <summary>
    /// bgmのボリュームを変える
    /// </summary>
    public void SetBGMVolume (string name, float volume) {
        if (!bgmDic.ContainsKey (name)) {
            return;
        }

        int containIndex = -1;
        for (int i = 0; i < bgmSourceList.Count; i++) {
            if (bgmSourceList[i].clip) {
                if (bgmSourceList[i].clip.name == name) {
                    containIndex = i;
                }
            }
        }

        if (containIndex == -1) {
            return;
        }

        bgmSourceList[containIndex].volume = volume;
    }

    /// <summary>
    /// bgmの現在時間を取得
    /// </summary>
    public float GetBGMTime (int index) {
        return bgmSourceList[index].time;
    }

    /// <summary>
    /// bgmの時間の長さを取得
    /// </summary>
    public float GetBGMTimeLength (string name) {
        if (!bgmDic.ContainsKey (name)) {
            return -1;
        }

        return bgmDic[name].length;
    }

    /// <summary>
    /// bpmから逆算してタイミングを取得する
    /// </summary>
    public float GetBGMTimingFromBPM (string name, int bpmIndex) {
        if (!bgmDic.ContainsKey (name)) {
            return -1;
        }

        return GetBGMTime (bpmIndex) / (60f / bgmBPM[name]);
    }

    public float GetBGMBeat (string name, int bpmIndex, int interval) {
        if (!bgmDic.ContainsKey (name)) {
            return -1;
        }

        float applyInterval = GetBGMTimingFromBPM (name, bpmIndex) / (float) interval;
        int applyIntervalInt = (int) (GetBGMTimingFromBPM (name, bpmIndex) / (float) interval);

        return applyInterval - applyIntervalInt;
    }

    public void AddPitch (int index, float num) {
        float prev = bgmSourceList[index].pitch;
        bgmSourceList[index].pitch = prev + num;
    }

    public void SetPitch (int index, float num) {
        bgmSourceList[index].pitch = num;
    }
}