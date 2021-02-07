using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {
    [SerializeField] private Text timeText;

    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject additionalScorePrefab;

    [SerializeField] private GameObject pauseLayer;

    private int prevIntTime;

    public void AppearAdditionalScore (int num) {
        Model.Instance.Score += num;

        GameObject obj = Instantiate (additionalScorePrefab) as GameObject;
        obj.transform.SetParent (this.transform, false);
        obj.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (Random.Range (10f, 50f), Random.Range (10f, 30f));
        obj.GetComponent<Text> ().text = string.Format ("+{0}", num);

        Destroy (obj, 2);
    }

    void Awake () {

    }

    void Start () {
        prevIntTime = (int) GameManager.TIME_LIMIT - Mathf.FloorToInt (Model.Instance.Time);
    }

    void Update () {
        if (prevIntTime != (int) GameManager.TIME_LIMIT - Mathf.FloorToInt (Model.Instance.Time)) {
            if (1 <= (int) GameManager.TIME_LIMIT - Mathf.FloorToInt (Model.Instance.Time) && (int) GameManager.TIME_LIMIT - Mathf.FloorToInt (Model.Instance.Time) <= 5) {
                AudioManager.Instance.PlaySE ("alart");
            }
        }

        prevIntTime = (int) GameManager.TIME_LIMIT - Mathf.FloorToInt (Model.Instance.Time);
        timeText.text = string.Format ("Time\n{0}", prevIntTime);
        scoreText.text = string.Format ("{0}pop", Model.Instance.Score);
    }
}