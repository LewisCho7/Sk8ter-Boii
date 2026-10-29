using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f; // 기본 속도
    [SerializeField] private float jumpPower = 6f; // 점프 power
    [SerializeField] private Transform groundCheck; // 땅 체크 위치
    [SerializeField] private LayerMask groundLayer; // 땅 레이어
    [SerializeField] private float groundCheckRadius = 0.15f; // 땅 체크 반경
    [SerializeField] private SpriteRenderer capSprite; // 모자 스프라이트
    [SerializeField] private Transform visual;
    [SerializeField] private Transform board;
    [SerializeField] private float tumbleSpeedX;    // 부딪혔을때 앞으로 튀어나가는 정도
    [SerializeField] private float tumbleSpeedY;    // 부딪혔을때 위로 튀어나가는 정도
    [SerializeField] private float tumbleDamping;   // 착지 후 미끄러지는 정도
    [SerializeField] private float tumbleDropY;     // 부딪혔을때 아래로 떨어지는 정도
    [SerializeField] private float tumbleTargetAngle;// 회전 목표 각도
    [SerializeField] private float tumbleRotateSpeed; // 초당 각도
    [SerializeField] private float crashDelaySeconds; // 충돌 후 딜레이 시간

    private Rigidbody2D rb;
    private bool isTumbling; // 충돌 후 튀어나가는 상태인지 여부
    private WaitForSeconds crashDelay;

    public bool IsGrounded { get; private set; } // 땅에 닿아있는지 여부
    public float SpeedMultiplier { get; set; } = 1f; // 속도 배율
    public bool IsDead { get; private set; } // 플레이어가 죽었는지 여부
    public bool IsFinished { get; private set; } = false; // 플레이어가 게임을 클리어했는지 여부

    public CapColor CurrentCap { get; private set; } // 현재 모자 색깔 enum

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentCap = CapColor.Blue; // 초기 모자 색깔 설정
        capSprite.color = CapType.GetColor(CurrentCap);
        crashDelay = new WaitForSeconds(crashDelaySeconds);
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); // 땅에 닿아있는지 여부 확인(layerMask를 이용하여 groundLayer에 닿아있는지 확인)

        if (IsDead) 
        {
            if(isTumbling)
            {
                float z = Mathf.MoveTowardsAngle(visual.eulerAngles.z, tumbleTargetAngle, tumbleRotateSpeed * Time.deltaTime); // 회전 목표 각도까지 회전
                visual.localRotation = Quaternion.Euler(0, 0, z); // 회전 적용. 부모와 별개로 visual만 회전하도록 localRotation 사용
            }
            return; // 이미 죽었으면 return
        }

        if (IsFinished) 
        {
            rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, 5f * Time.deltaTime), rb.linearVelocity.y); // 게임 클리어 시 수평 속도를 0으로 설정
            return; // 이미 클리어했으면 return
        }

        
        float velocityY = rb.linearVelocity.y; 

        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            velocityY = jumpPower;  // 점프
        }

        rb.linearVelocity = new Vector2(speed * SpeedMultiplier, velocityY); // 키보드로 이동하는게 아니라 speed에 따라 자동 이동

        if (Input.GetKeyDown(KeyCode.Q)) // Q는 파란색
        {
            CurrentCap = CapColor.Blue;
        }
        else if (Input.GetKeyDown(KeyCode.W)) // W는 빨간색
        {
            CurrentCap = CapColor.Red;
        }
        else if (Input.GetKeyDown(KeyCode.E)) // E는 초록색
        {
            CurrentCap = CapColor.Green;
        }
        else if (Input.GetKeyDown(KeyCode.R)) // R은 보라색
        {
            CurrentCap = CapColor.Purple;
        }

        capSprite.color = CapType.GetColor(CurrentCap); // 모자 색깔 변경

    }

    public void Crash(CrashReason reason)
    {
        if (IsDead) return; // 이미 죽었으면 return
        IsDead = true; // 플레이어가 죽었음을 표시

        if (reason != CrashReason.Hole)
        {
            board.SetParent(null); // 보드를 분리
            rb.linearVelocity = new Vector2(tumbleSpeedX, tumbleSpeedY); // 충돌 후 튀어나가는 속도 설정
            rb.linearDamping = tumbleDamping; // 충돌 후 미끄러지는 정도 설정
            visual.localPosition += new Vector3(0f, -tumbleDropY, 0f); // 충돌 후 visual을 아래로 떨어뜨림
            isTumbling = true; // 충돌 후 튀어나가는 상태로 설정
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // 구멍에 빠졌을 때는 수평 속도를 0으로 설정
        }

        StartCoroutine(GameOverAfterDelay());
    }

    public void Finish()
    {
        if (IsFinished) return; // 이미 클리어했으면 return
        IsFinished = true; // 플레이어가 게임을 클리어했음을 표시
    }

    private IEnumerator GameOverAfterDelay()
    {
        yield return crashDelay; // 충돌 후 딜레이 시간만큼 대기
        GameManager.Instance.GameOver(); // 게임 오버 처리
    }
}
