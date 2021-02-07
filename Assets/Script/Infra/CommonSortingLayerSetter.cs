using UnityEngine;

public class CommonSortingLayerSetter : MonoBehaviour {
    [SerializeField] private string sortingLayer;

    void Awake () {
        GetComponent<MeshRenderer> ().sortingLayerName = sortingLayer;
    }
}