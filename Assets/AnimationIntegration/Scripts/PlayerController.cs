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
    [SerializeField] float distanceToFinish = 5;

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
        CheckDistanceToEnemy();
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
        animator.SetTrigger("Finish");
        StartCoroutine(WaitForFinishing());
    }

    IEnumerator WaitForFinishing()
    {
        automatic.SetActive(false);
        sword.SetActive(true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
            yield return null;

        // while (AnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
            yield return null;

        sword.SetActive(false);
        automatic.SetActive(true);

        inAction = false;
    }

    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    void CheckDistanceToEnemy()
    {
        if (enemy == null)
            return;

        enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
        readyForAction = !inAction && enemyDistance <= distanceToFinish;
    }

    private void OnGUI()
    {
        instructionsText.gameObject.SetActive(readyForAction);
    }

    private void FixedUpdate()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Finishing"))
        {
            animator.SetFloat("Speed", verticalInput);

            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.fixedDeltaTime);

            Vector3 moveVector = new Vector3(0, 0, verticalInput);
            transform.Translate(moveVector * speed * Time.fixedDeltaTime, Space.Self);

            Camera.main.transform.position = transform.position - cameraDelta;
        }
    }
}
