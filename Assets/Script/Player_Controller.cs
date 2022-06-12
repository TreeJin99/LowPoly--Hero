using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Animator _animator;
    private Camera _camera;
    private CharacterController _characterController;

    // 플레이어 기본 스탯
    public float playerHP = 100f;
    public float playerDamage = 20f;
    public float walkSpeed = 2f;
    public float defendSpeed = 1f;
    public float runSpeed = 4f;
    public float jumpHeight = 3f;

    // 플레이어 무기 & 방어구
    public GameObject grenadeObj;
    public Collider weaponCollider;
    public Collider shieldCollider;
    public TrailRenderer trailEffect;
    public float attackSpeed = 0.4f;

    // 플레이어 움직임 시스템 설정 
    public float smoothness = 10f;
    public float gravity = 10f;

    private float currentHP;
    private float currentSpeed;
    private float attackDelay;
    private int grenadeNum = 10;

    private Vector3 moveDir;

    private float hAxis;
    private float vAxis;
    private float xMouse;
    private float yMouse;


    private bool wDown;
    private bool gDown;
    private bool altDown;

    private bool isJump;
    private bool isRun;
    private bool isAttack;
    private bool isDefend;
    private bool isCoolTime;

    private void Awake()
    {
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        moveDir = Vector3.zero;
        currentHP = playerHP;
    }

    private void Update()
    {
        GetInput();
        Movement();
        Action();
    }

    private void LateUpdate()
    {
        toggleCamera();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;


        if (hit.gameObject.tag == "Enemy")
            Debug.Log("적팀!");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            switch (other.gameObject.name)
            {
                case "Health":
                    Debug.Log("치료!");
                    currentHP += 50;
                    break;

                case "Grenade Item":
                    Debug.Log("수류탄 획득!");
                    grenadeNum++;
                    break;
            }
        }
    }

    private void GetInput()
    {

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        altDown = Input.GetKey(KeyCode.LeftAlt);
        wDown = Input.GetKey(KeyCode.W);
        gDown = Input.GetKeyDown(KeyCode.G);

        isJump = Input.GetKey(KeyCode.Space);
        isRun = Input.GetKey(KeyCode.LeftShift);

        isAttack = Input.GetMouseButtonDown(0);
        isDefend = Input.GetMouseButton(1);
    }

    private void toggleCamera()
    {
        if (!altDown)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    private void Movement()
    {
        currentSpeed = isDefend ? defendSpeed : isRun ? runSpeed : walkSpeed;
        if (_characterController.isGrounded)
        {
            moveDir = new Vector3(hAxis, 0, vAxis);
            moveDir = transform.TransformDirection(moveDir);
            moveDir *= currentSpeed;

            if (isJump)
                moveDir.y = jumpHeight;
        }


        moveDir.y -= gravity * Time.deltaTime;
        _characterController.Move(moveDir * Time.deltaTime);

        float isMove = isRun ? 1 : wDown ? 0.5f : 0;
        _animator.SetFloat("Blend", isMove, 0.1f, Time.deltaTime);

    }

    private void Action()
    {
        attackDelay += Time.deltaTime;
        isCoolTime = attackDelay + attackSpeed < 1f;

        if (isAttack && !isCoolTime && !isDefend)
        {
            StopCoroutine("Attack");
            StartCoroutine("Attack");
            _animator.SetTrigger("isAttack");
            attackDelay = 0f;
        }
        else if (!isAttack && isDefend)
        {
            shieldCollider.enabled = true;
            _animator.SetTrigger("isDefend");
        }

        if (gDown)
        {
            if (grenadeNum > 0 && !isAttack && !isDefend)
            {
                Vector3 grenadePos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                GameObject grenadeThrow = Instantiate(grenadeObj, grenadePos, transform.rotation);
                Rigidbody grenadeRigid = grenadeThrow.GetComponent<Rigidbody>();

                grenadeRigid.AddForce(new Vector3(moveDir.x, moveDir.y + 2, moveDir.z) * 2, ForceMode.Impulse);
                grenadeRigid.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                grenadeNum--;
            }
        }

    }


    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(0.3f);
        weaponCollider.enabled = false;
    }



}
