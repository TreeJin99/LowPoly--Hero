using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller PLAYER_INSTANCE;
    public AudioSource attackSound;
    public AudioSource killSound;
    public AudioSource onHitSound;
    public AudioSource getItemSound;
    public AudioSource throwGrenade;


    private Animator _animator;
    private Camera _camera;
    private CharacterController _characterController;
    private Renderer _renderer;
    private Color originColor;

    private Text healthTxt;
    private Text damageTxt;
    private Text grenadeTxt;
    private Text killTxt;
    private Text timeTxt;
    private Text expTxt;
    public GameObject levelupPanel;
    public GameObject diePanel;

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
    private int killCount = 0;
    private float currentTime = 0.0f;
    private float currentExp = 0.0f;
    private float levelupExp = 100.0f;

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
    private bool isDamaged;
    private bool cursorLock = false;


    private void Awake()
    {
        PLAYER_INSTANCE = this;

        healthTxt = GameObject.Find("Health Txt").GetComponent<Text>();
        damageTxt = GameObject.Find("Damage Txt").GetComponent<Text>();
        grenadeTxt = GameObject.Find("Grenade Txt").GetComponent<Text>();
        killTxt = GameObject.Find("Kill Txt").GetComponent<Text>();
        timeTxt = GameObject.Find("Stage Txt").GetComponent<Text>();
        expTxt = GameObject.Find("Exp Txt").GetComponent<Text>();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _renderer = GetComponentInChildren<MeshRenderer>();

        moveDir = Vector3.zero;
        currentHP = playerHP;
        originColor = _renderer.material.color;
    }

    private void Update()
    {
        CursorLock();
        UpdateUI();
        GetInput();
        Movement();
        Action();
    }

    private void LateUpdate()
    {
        toggleCamera();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            switch (other.gameObject.name)
            {
                case "Health(Clone)":
                    currentHP += 30;
                    getItemSound.Play();
                    break;

                case "Grenade Item(Clone)":
                    grenadeNum++;
                    getItemSound.Play();
                    break;
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (!isDamaged)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                if (!isDefend)
                {
                    currentHP -= enemyBullet.damage;
                }
                else
                {
                    currentHP -= (enemyBullet.damage) / 3;
                }
                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage()
    {
        isDamaged = true;
        _renderer.material.color = Color.red;
        onHitSound.Play();

        yield return new WaitForSeconds(0.3f);
        if (currentHP > 0)
        {
            _renderer.material.color = originColor;
        }
        else if (currentHP <= 0)
        {
            _renderer.material.color = Color.gray;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            gameObject.SetActive(false);
            Time.timeScale = 0;
            diePanel.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        isDamaged = false;
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

    private void CursorLock()
    {
        if (cursorLock == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
                throwGrenade.Play();

                grenadeNum--;
            }
        }

    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        weaponCollider.enabled = true;
        attackSound.Play();

        yield return new WaitForSeconds(0.3f);
        weaponCollider.enabled = false;
        attackSound.Stop();
    }

    public void DamageUp()
    {
        playerDamage += 30;
        levelupPanel.SetActive(false);
        Time.timeScale = 1;
        cursorLock = false;
    }

    public void AttackSpeedUp()
    {
        attackSpeed += 0.1f;
        levelupPanel.SetActive(false);
        Time.timeScale = 1;
        cursorLock = false;
    }

    public void HealthUp()
    {
        currentHP += 50;
        levelupPanel.SetActive(false);
        Time.timeScale = 1;
        cursorLock = false;
    }

    public void KillEnemy(float exp)
    {
        killSound.Play();
        currentExp += exp;
        killCount++;
        LevelUp();
    }

    private void LevelUp()
    {
        if (currentExp >= levelupExp)
        {
            levelupExp *= 1.5f;
            Time.timeScale = 0;
            cursorLock = true;
            levelupPanel.SetActive(true);
        }
    }

    private void UpdateUI()
    {
        currentTime += Time.deltaTime;
        timeTxt.text = string.Format("{0:N1}", currentTime);

        healthTxt.text = currentHP.ToString();
        damageTxt.text = playerDamage.ToString();
        killTxt.text = killCount.ToString();
        grenadeTxt.text = grenadeNum.ToString();
        expTxt.text = currentExp.ToString();
    }

}
