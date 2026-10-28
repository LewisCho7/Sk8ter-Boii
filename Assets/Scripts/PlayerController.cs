using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 6f; // 기본 속도
    [SerializeField] private float jumpPower = 6f; // 점프 power
    [SerializeField] private Transform groundCheck; // 땅 체크 위치
    [SerializeField] private LayerMask groundLayer; // 땅 레이어
    [SerializeField] private float groundCheckRadius = 0.15f; // 땅 체크 반경
    [SerializeField] private SpriteRenderer capSprite; // 모자 스프라이트

    private Rigidbody2D rb;

    public bool IsGrounded { get; private set; } // 땅에 닿아있는지 여부
    public float SpeedMultiplier { get; set; } = 1f; // 속도 배율

    public CapColor CurrentCap { get; private set; } // 현재 모자 색깔 enum


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentCap = CapColor.Blue; // 초기 모자 색깔 설정
        capSprite.color = CapType.GetColor(CurrentCap);
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); // 땅에 닿아있는지 여부 확인(layerMask를 이용하여 groundLayer에 닿아있는지 확인)
        
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
}
