using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class CameraMover : MonoBehaviour {
    private float easingSpeed = 0.2f; //カメラ追従速度
    //private Vector3 fix = new Vector3(0,1.65f,-0.8f); //位置補正
    //private Vector3 rot = new Vector3(20,0,0); //角度補正

    private Vector3 fix = new Vector3 (0, 2.4f, -1.4f); //位置補正
    private Vector3 rot = new Vector3 (30, 0, 0); //角度補正

    [SerializeField]
    private GameObject attention; //注目オブジェクト

    private Camera cam; //カメラ

    private void Init () {
        cam = this.GetComponent<Camera> ();
    }

    private void Awake () {
        Init ();
    }

    private void Update () {
        if (attention == null) return;

        var goal = attention.transform.position + fix;
        transform.position = Vector3.Lerp (transform.position, goal, easingSpeed);

        var goalRotate = rot;
        transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, goalRotate, easingSpeed);
    }
}