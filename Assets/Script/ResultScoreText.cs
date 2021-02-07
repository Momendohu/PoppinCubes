using UnityEngine;
using UnityEngine.UI;

public class ResultScoreText : MonoBehaviour {
    [SerializeField] private Text text;
    void Update () {
        text.text = string.Format ("{0}pop", Model.Instance.Score);
    }
}