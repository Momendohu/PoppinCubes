using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
    private CubeEntity cubeEntity;
    private Animator animator;

    [SerializeField] private GameObject powerView;

    [SerializeField] private GameObject explode;

    private GameManager gameManager = null;

    private bool waitChangeState = false;

    void Awake () {
        cubeEntity = GetComponent<CubeEntity> ();
        animator = GetComponent<Animator> ();

        gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
    }

    void Start () {
        transform.position = cubeEntity.Position;
    }

    public void ApplyState () {
        switch (cubeEntity.State) {
            case CubeState.CLOSE:
                transform.Find ("Sprite").gameObject.SetActive (false);
                transform.Find ("Core").gameObject.SetActive (true);
                animator.SetBool ("Open", false);
                animator.SetBool ("Close", true);
                transform.Find ("Core").GetComponent<MeshRenderer> ().material.color = new Color (0, 0, 0, 100f / 255);
                explode.SetActive (false);
                waitChangeState = false;
                break;

            case CubeState.OPENR:
                AudioManager.Instance.PlaySE ("pop");
                transform.Find ("Sprite").gameObject.SetActive (true);
                transform.Find ("Core").gameObject.SetActive (false);
                animator.SetBool ("Open", true);
                animator.SetBool ("Close", false);
                transform.Find ("Sprite").GetComponent<MeshRenderer> ().material.color = new Color (1, 0.1f, 0);
                break;

            case CubeState.OPENG:
                AudioManager.Instance.PlaySE ("pop");
                transform.Find ("Sprite").gameObject.SetActive (true);
                transform.Find ("Core").gameObject.SetActive (false);
                animator.SetBool ("Open", true);
                animator.SetBool ("Close", false);
                transform.Find ("Sprite").GetComponent<MeshRenderer> ().material.color = new Color (0, 1, 0.1f);
                break;

            case CubeState.OPENB:
                AudioManager.Instance.PlaySE ("pop");
                transform.Find ("Sprite").gameObject.SetActive (true);
                transform.Find ("Core").gameObject.SetActive (false);
                animator.SetBool ("Open", true);
                animator.SetBool ("Close", false);
                transform.Find ("Sprite").GetComponent<MeshRenderer> ().material.color = new Color (0.1f, 0, 1);
                break;

            case CubeState.EXPLODE:
                waitChangeState = false;
                transform.Find ("Sprite").gameObject.SetActive (false);
                transform.Find ("Core").gameObject.SetActive (true);
                transform.Find ("Core").GetComponent<MeshRenderer> ().material.color = new Color (1, 0, 0, 120f / 255);
                explode.SetActive (true);
                break;

            default:
                Debug.LogAssertion ("ステートがおかしい");
                break;
        }
    }

    void Update () {
        if (gameManager.IsPause && gameManager.IsGamestart && !gameManager.IsGameover) {
            return;
        }

        if (cubeEntity.State == CubeState.CLOSE && !waitChangeState) {
            waitChangeState = true;
            StartCoroutine (ChangeState ());
        }

        if (cubeEntity.Power < GameManager.EXPLODE_POWER_LOWER_LIMIT) {
            animator.SetTrigger ("Normal");
        } else {
            animator.SetTrigger ("Standby");
        }
    }

    public IEnumerator ChangeState () {
        float time = Random.Range (1f, 10f);

        float partOfTime = 0;
        while (partOfTime < 10) {
            yield return new WaitForSeconds (time / 10);
            if (!gameManager.IsPause && gameManager.IsGamestart && !gameManager.IsGameover) {
                partOfTime++;
            }
        }

        if (cubeEntity.State == CubeState.CLOSE) {
            int state = Random.Range (1, 4);
            cubeEntity.State = (CubeState) state;
            ApplyState ();
        }
    }

    public void Close () {
        cubeEntity.State = CubeState.CLOSE;
        initialize ();
    }

    private void initialize () {
        cubeEntity.Power = 0;
        applyPowerText (0);
        powerView.SetActive (false);
        ApplyState ();
    }

    public IEnumerator Explode () {
        cubeEntity.State = CubeState.EXPLODE;
        initialize ();
        yield return new WaitForSeconds (0.5f);
        Close ();
    }

    public void Charge (int num) {
        animator.SetTrigger ("Repop");

        if (cubeEntity.Power == 0) {
            powerView.SetActive (true);
        }
        cubeEntity.Power += num;

        applyPowerText (cubeEntity.Power);
    }

    private void applyPowerText (int num) {
        powerView.GetComponent<TextMesh> ().text = string.Format ("{0}", num);
    }
}