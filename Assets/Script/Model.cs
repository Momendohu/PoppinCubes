using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : SingletonMonoBehaviour<Model> {
    private int _score = 0;

    private float _time = 0;

    public int Score {
        get => this._score;
        set => this._score = value;
    }

    public float Time {
        get => this._time;
        set => this._time = value;
    }

    void Awake () {
        //Instance化をすでにしてるなら
        if (this != Instance) {
            Destroy (this);
            return;
        }

        DontDestroyOnLoad (this.gameObject);
    }
}