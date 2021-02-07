using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeFactory {
    public static GameObject CreateCube (Vector2Int id, Vector3 position) {
        GameObject obj = GameObject.Instantiate (Resources.Load ("Prefabs/Cube")) as GameObject;
        obj.transform.SetParent (GameObject.Find ("Field").transform, true);

        CubeEntity entity = obj.GetComponent<CubeEntity> ();
        entity.Position = position;
        entity.Id = id;

        return obj;
    }
}