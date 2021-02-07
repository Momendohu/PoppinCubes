using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntity : MonoBehaviour, IEntity {
    private Vector2Int _id = Vector2Int.zero;

    private Vector3 _position = Vector3.zero;

    private CubeState _state = CubeState.CLOSE;

    private int _power = 0;

    public Vector2Int Id {
        get => this._id;
        set => this._id = value;
    }

    public Vector3 Position {
        get => this._position;
        set => this._position = value;
    }

    public CubeState State {
        get => this._state;
        set => this._state = value;
    }

    public int Power {
        get => this._power;
        set => this._power = value;
    }

    public Vector2Int GetIdentify () {
        return Id;
    }
}