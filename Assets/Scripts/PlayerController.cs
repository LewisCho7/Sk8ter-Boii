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
    [SerializeField] private Rigidbody2D boardRb;
    [SerializeField] private float tumbleSpeedX;    // 부딪혔을때 앞으로 튀어나가는 정도
    [SerializeField] private float tumbleSpeedY;    // 부딪혔을때 위로 튀어나가는 정도
    [SerializeField] private float tumbleDamping;   // 착지 후 미끄러지는 정도
    [SerializeField] private float tumbleDropY;     // 부딪혔을때 아래로 떨어지는 정도
    [SerializeField] private float tumbleTargetAngle;// 회전 목표 각도
    [SerializeField] private float tumbleRotateSpeed; // 초당 각도
    [SerializeField] private float crashDelaySeconds; // 충돌 후 딜레이 시간
    [SerializeField] private float tiltSpeed = 200f; // tilt 속도
    [SerializeField] private float landingAngleLimit = 30f; // 착지 각도 범위
    [SerializeField] private float holeSpeedKeep = 0.5f; // 구멍에 빠졌을 때 수평 속도 유지 비율
    [SerializeField] private float tiltRecoverSpeed = 360f; // 기울기 복구 속도
    [SerializeField] private float boardSpin = 360f; 
    [SerializeField] private int flipScore = 200; // flip 점수
    [SerializeField] private float groundRayLength = 0.6f; // 땅 체크 레이 길이
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private ParticleSystem sparkFx;
    [SerializeField] private ParticleSystem flipFx;


    private Rigidbody2D rb;
    private bool isTumbling; // 충돌 후 튀어나가는 상태인지 여부
    private WaitForSeconds crashDelay;
    private float tiltAngle; // 기울기 각도
    private bool wasGrounded; // 이전 프레임의 땅에 닿아있는지 여부
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int VelocityYHash = Animator.StringToHash("VelocityY");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int CrashHash = Animator.StringToHash("Crash");

    public bool IsGrounded { get; private set; } // 땅에 닿아있는지 여부
    public float SpeedMultiplier { get; set; } = 1f; // 속도 배율
    public bool IsDead { get; private set; } // 플레이어가 죽었는지 여부
    public bool IsFinished { get; private set; } = false; // 플레이어가 게임을 클리어했는지 여부
    public bool IsFalling { get; private set; } // 플레이어가 떨어지고 있는지 여부

    public CapColor CurrentCap { get; private set; } // 현재 모자 색깔 enum

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boardRb = board.GetComponent<Rigidbody2D>();
        boardRb.simulated = false; // 보드 Rigidbody2D 시뮬레이션 비활성화
        CurrentCap = CapColor.Blue; // 초기 모자 색깔 설정
        capSprite.color = CapType.GetColor(CurrentCap);
        crashDelay = new WaitForSeconds(crashDelaySeconds);
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); // layerMask를 이용하여 groundLayer에 닿아있는지 확인. collider2D 반환
        IsGrounded = groundHit != null && !(rb.linearVelocity.y > 0.1f && groundHit.usedByEffector); // 올라가는중에 레일에 닿으면 아직 지면이 아님. 내려가는 중에 닿아야 지면.
        
        animator.SetBool(GroundedHash, IsGrounded); // 땅에 닿아있는지 여부를 animator에 전달
        animator.SetFloat(VelocityYHash, rb.linearVelocity.y); // 수직 속도를 전달

        bool alive = !IsDead && !IsFinished;
        bool onRail = IsGrounded && groundHit.usedByEffector;
        SetEmit(dustFx, alive && IsGrounded && !onRail); // 땅에 닿아있고 레일이 아니면 dustFx 켜기
        SetEmit(sparkFx, alive && onRail); // 레일이면 sparkFx 켜기

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

        if(!IsGrounded)
        {
            float input = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                input += 1f; 
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                input -= 1f; 
            }
            tiltAngle += input * tiltSpeed * Time.deltaTime; // 기울기 각도 연산
        }
        else
        {
            float groundAngle = GetGroundAngle(); // 땅의 기울기 각도 계산

            if (!wasGrounded) // 이전 프레임에는 땅에 닿아있지 않았는데 이번 프레임에는 땅에 닿아있다면 착지한 경우
            {
                float diff = Mathf.Abs(Mathf.DeltaAngle(tiltAngle, groundAngle)); // 착지 시 기울기 각도 차이 계산
                if (diff > landingAngleLimit) // 착지 시 기울기 각도가 제한 범위를 초과하면 충돌 처리
                {
                    Crash(CrashReason.BadLanding); // 충돌 처리
                    return; // 충돌 처리 후 return
                }

                int flips = Mathf.RoundToInt(Mathf.Abs(tiltAngle) / 360f); // 360도 회전한 횟수 계산

                if(flips > 0)
                {
                    GameManager.Instance.AddScore(flips * flipScore); // 점수 추가
                    flipFx.Emit(15 * flips); // flipFx 파티클 발생
                    Debug.Log("Flips: " + flips + ", Score added: " + (flips * flipScore)); // 점수 추가 로그 출력
                }

                tiltAngle = Mathf.DeltaAngle(0f, tiltAngle); // 착지 시 기울기 각도 초기화
            }

            tiltAngle = Mathf.MoveTowardsAngle(tiltAngle, groundAngle, tiltRecoverSpeed * Time.deltaTime); // 착지 시 기울기 각도 복구
        }
        wasGrounded = IsGrounded; // 이전 프레임의 땅에 닿아있는지 여부 저장
        visual.localRotation = Quaternion.Euler(0, 0, tiltAngle); // 기울기 각도 적용
        
        float velocityY = rb.linearVelocity.y; 

        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            velocityY = jumpPower;  // 점프
            animator.SetTrigger(JumpHash); // 점프 애니메이션 트리거
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
            boardRb.simulated = true; // 보드 Rigidbody2D 시뮬레이션 활성화
            boardRb.linearVelocity = Vector2.zero;
            rb.linearVelocity = new Vector2(tumbleSpeedX, tumbleSpeedY); // 충돌 후 튀어나가는 속도 설정
            rb.linearDamping = tumbleDamping; // 충돌 후 미끄러지는 정도 설정
            visual.localPosition += new Vector3(0f, -tumbleDropY, 0f); // 충돌 후 visual을 아래로 떨어뜨림
            isTumbling = true; // 충돌 후 튀어나가는 상태로 설정
            animator.SetTrigger(CrashHash); // 충돌 애니메이션 트리거
        }
        else
        {
            IsFalling = true; // 플레이어가 구멍에 빠졌음을 표시
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * holeSpeedKeep, rb.linearVelocity.y);
            DetachBoard(rb.linearVelocity, boardSpin); // 구멍에 떨어졌을 때 보드를 분리하고 속도와 회전 적용
        }

        StartCoroutine(GameOverAfterDelay());
    }

    public void Launch(float power)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, power); // 점프력에 따라 수직 속도 설정
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

    private float GetGroundAngle()
    {
        Vector2 origin = (Vector2)groundCheck.position + Vector2.up * 0.1f; 
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayLength, groundLayer); // groundCheck 위치에서 아래로 레이캐스트 발사
    
        if(hit.collider == null)
        {
            return 0f;
        }
        return Vector2.SignedAngle(Vector2.up, hit.normal); // 땅의 법선 벡터와 위쪽 벡터 사이의 각도 반환
    }

    private void DetachBoard(Vector2 velocity, float spin) // 구멍에 떨어졌을 때 속도, 회전 줘서 보드를 분리
    {
        board.SetParent(null); // 보드를 분리
        boardRb.simulated = true; // 보드 Rigidbody2D 시뮬레이션 활성화
        boardRb.linearVelocity = velocity; 
        boardRb.angularVelocity = spin; 
    }

    private void SetEmit(ParticleSystem ps, bool on)
    {
        var emission = ps.emission;
        if(emission.enabled != on)
        {
            emission.enabled = on;
        }
    }
}
