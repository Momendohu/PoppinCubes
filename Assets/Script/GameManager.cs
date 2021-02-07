using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static readonly float CUBE_INTERVAL = 1.1f;

    public static readonly int EXPLODE_POWER_LOWER_LIMIT = 4;

    public static readonly float TIME_LIMIT = 75;

    private bool _isGamestart = false;

    private bool _isGameover = false;

    private bool _isPause = false;

    public bool IsGamestart {
        get => this._isGamestart;
        set => this._isGamestart = value;
    }

    public bool IsGameover {
        get => this._isGameover;
        set => this._isGameover = value;
    }

    public bool IsPause {
        get => this._isPause;
        set => this._isPause = value;
    }

    private GameObject[, ] cubes = new GameObject[10, 10];

    [SerializeField] private GameObject shadowPrefab;

    [SerializeField] private CanvasManager canvasManager;

    [SerializeField] private GameObject pauseLayer;

    [SerializeField] private GameObject resultLayer;

    [SerializeField] private GameObject titleLayer;

    [SerializeField] private GameObject gameLayer;

    void Awake () {

    }

    void Start () {
        AudioManager.Instance.PlayBGM ("Normal", true);
        for (int i = 0; i < cubes.GetLength (0); i++) {
            for (int j = 0; j < cubes.GetLength (1); j++) {
                cubes[i, j] = CubeFactory.CreateCube (new Vector2Int (i, j), new Vector3 (i * GameManager.CUBE_INTERVAL, 0, j * GameManager.CUBE_INTERVAL));
            }
        }
    }

    void Update () {
        if (IsPause || !IsGamestart || IsGameover) {
            return;
        }

        CalcurateTime ();
        PickCube ();
    }

    private void CalcurateTime () {
        Model.Instance.Time += Time.deltaTime;

        if (Model.Instance.Time >= GameManager.TIME_LIMIT) {
            Model.Instance.Time = GameManager.TIME_LIMIT;
            IsGameover = true;

            gameLayer.SetActive (false);
            resultLayer.SetActive (true);

            AudioManager.Instance.PlaySE ("clear");

            naichilab.RankingLoader.Instance.SendScoreAndShowRanking (Model.Instance.Score);
        }
    }

    private void PickCube () {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast (ray, out hit, 50.0f)) {
            return;
        }

        if (InputUtil.GetTouch () == (InputUtil.TouchInfo.Began)) {
            Transform cube = hit.collider.transform.parent;
            CubeEntity entity = cube.GetComponent<CubeEntity> ();

            int openNum = 0;
            if (entity.State != CubeState.CLOSE) {
                for (int i = -1; i < 2; i++) {
                    for (int j = -1; j < 2; j++) {
                        if (!((Mathf.Abs (i) + Mathf.Abs (j)) == 1)) {
                            continue;
                        }

                        int _x = i + entity.GetIdentify ().x;
                        int _y = j + entity.GetIdentify ().y;

                        if (0 > _x || _x > cubes.GetLength (0) - 1) {
                            continue;
                        }

                        if (0 > _y || _y > cubes.GetLength (1) - 1) {
                            continue;
                        }

                        if (cubes[_x, _y].GetComponent<CubeEntity> ().State == entity.State) {
                            openNum += (cubes[_x, _y].GetComponent<CubeEntity> ().Power + 1);
                            cubes[_x, _y].GetComponent<Cube> ().Close ();
                        }

                    }
                }

                int power = cube.GetComponent<CubeEntity> ().Power;

                canvasManager.AppearAdditionalScore (power + openNum + 1);

                if (power <= 0) {
                    if (openNum == 0) {
                        cube.GetComponent<Cube> ().Close ();
                        AudioManager.Instance.PlaySE ("pop2");
                        return;
                    }

                    cube.GetComponent<Cube> ().Charge (openNum);
                    AudioManager.Instance.PlaySE ("puyon");
                    return;
                }

                if (0 < power && power < GameManager.EXPLODE_POWER_LOWER_LIMIT) {
                    cube.GetComponent<Cube> ().Close ();
                    AudioManager.Instance.PlaySE ("pop2");
                    return;
                } else {
                    AudioManager.Instance.PlaySE ("bomb");
                    StartCoroutine (ExplodeAround (entity));
                    StartCoroutine (cube.GetComponent<Cube> ().Explode ());
                }
            }
        }
    }

    private IEnumerator ExplodeAround (CubeEntity entity) {
        // NOTE:初期化処理がかかるため先にキャッシュしておく
        int tmpPower = entity.Power;
        for (int i = -2; i < 3; i++) {
            for (int j = -2; j < 3; j++) {
                int _x = i + entity.GetIdentify ().x;
                int _y = j + entity.GetIdentify ().y;

                if (0 > _x || _x > cubes.GetLength (0) - 1) {
                    continue;
                }

                if (0 > _y || _y > cubes.GetLength (1) - 1) {
                    continue;
                }

                if (i == 0 && j == 0) {
                    continue;
                }

                canvasManager.AppearAdditionalScore (cubes[_x, _y].GetComponent<CubeEntity> ().Power * tmpPower + 1);
                StartCoroutine (cubes[_x, _y].GetComponent<Cube> ().Explode ());
                AudioManager.Instance.PlaySE ("bomb");

                yield return new WaitForSeconds (0.03f);
            }
        }
    }

    public void PushStartButton () {
        AudioManager.Instance.PlaySE ("button");
        AudioManager.Instance.PlaySE ("start");
        IsGamestart = true;
        titleLayer.SetActive (false);
        gameLayer.SetActive (true);
    }

    public void PushPauseButton () {
        AudioManager.Instance.PlaySE ("cat");
        IsPause = true;
        pauseLayer.SetActive (true);
        gameLayer.SetActive (false);
    }

    public void PushResumeButton () {
        AudioManager.Instance.PlaySE ("button");
        IsPause = false;
        pauseLayer.SetActive (false);
        gameLayer.SetActive (true);
    }

    public void PushRestartButton () {
        AudioManager.Instance.PlaySE ("bomb");
        AudioManager.Instance.PlaySE ("start");
        for (int i = 0; i < cubes.GetLength (0); i++) {
            for (int j = 0; j < cubes.GetLength (1); j++) {
                StartCoroutine (cubes[i, j].GetComponent<Cube> ().Explode ());
            }
        }

        IsGamestart = true;
        IsGameover = false;
        IsPause = false;

        Model.Instance.Score = 0;
        Model.Instance.Time = 0;

        gameLayer.SetActive (true);
        pauseLayer.SetActive (false);
        resultLayer.SetActive (false);
    }

    public void PushTweetButton () {
        AudioManager.Instance.PlaySE ("button");
        naichilab.UnityRoomTweet.Tweet (
            "poppincubes",
            string.Format ("ポッピンキューブス で {0}pop したよ☆", Model.Instance.Score),
            "unity1week", "POPPINCUBES"
        );
    }

    public void PushTitleButton () {
        AudioManager.Instance.PlaySE ("bomb");
        for (int i = 0; i < cubes.GetLength (0); i++) {
            for (int j = 0; j < cubes.GetLength (1); j++) {
                StartCoroutine (cubes[i, j].GetComponent<Cube> ().Explode ());
            }
        }

        IsGamestart = false;
        IsGameover = false;
        IsPause = false;

        Model.Instance.Score = 0;
        Model.Instance.Time = 0;

        pauseLayer.SetActive (false);
        titleLayer.SetActive (true);
        resultLayer.SetActive (false);
    }

    public void PushRankingButton () {
        AudioManager.Instance.PlaySE ("button");
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking (Model.Instance.Score);
    }
}