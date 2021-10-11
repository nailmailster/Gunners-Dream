using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator animator;
    float verticalInput, horizontalInput;
    [SerializeField] float speed = 15;
    [SerializeField] float rotationSpeed = 90;

    [SerializeField] GameObject enemy;
    float enemyDistance;
    [SerializeField] float enemyDetectionDistance = 5;
    [SerializeField] float finishDistance = 1.5f;

    [SerializeField] Text instructionsText;
    bool readyForAction, inAction;

    Vector3 cameraDelta;

    [SerializeField] GameObject sword;
    [SerializeField] GameObject automatic;

    void Start()
    {
        cameraDelta = transform.position - Camera.main.transform.position;

        enemy = GameObject.Find("Enemy");
    }

    void Update()
    {
        GetInput();
        DetectEnemy();
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && readyForAction)
            FinishEnemy();
    }

    void FinishEnemy()
    {
        inAction = true;
        StartCoroutine(SmoothRotateToEnemy());
    }

    IEnumerator SmoothRotateToEnemy()
    {
        Vector3 targetDirection = (enemy.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 10 * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(SmoothRunToEnemy());
    }

    IEnumerator SmoothRunToEnemy()
    {
        animator.SetFloat("Speed", 1);
        while (Vector3.Distance(transform.position, enemy.transform.position) > finishDistance)
        {
            var deltaMove = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, deltaMove);
            CorrectCameraPosition();
            yield return null;
        }
        animator.SetTrigger("Finish");
        animator.SetFloat("Speed", 0);
        StartCoroutine(WaitForFinishing());
    }

    IEnumerator WaitForFinishing()
    {
        automatic.SetActive(false);
        sword.SetActive(true);

        //  ждем когда состояние перейдет в Finishing
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
            yield return null;

        //  ждем когда Finishing отыграет
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
            yield return null;

        sword.SetActive(false);
        automatic.SetActive(true);

        inAction = false;
    }

    void DetectEnemy()
    {
        if (enemy == null)
            return;

        enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
        readyForAction = !inAction && enemyDistance <= enemyDetectionDistance;
    }

    private void OnGUI()
    {
        instructionsText.gameObject.SetActive(readyForAction);
    }

    private void FixedUpdate()
    {
        if (!inAction)
        {
            WASDPlayer();
            CorrectCameraPosition();
        }
    }

    void WASDPlayer()
    {
        animator.SetFloat("Speed", verticalInput);

        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.fixedDeltaTime);

        Vector3 moveVector = new Vector3(0, 0, verticalInput);
        transform.Translate(moveVector * speed * Time.fixedDeltaTime, Space.Self);
    }

    void CorrectCameraPosition()
    {
        Camera.main.transform.position = transform.position - cameraDelta;
    }
}
