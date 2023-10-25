using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour {

   public AudioClip deathClip; // 사망시 재생할 오디오 클립
   public float jumpForce = 700f; // 점프 힘

   //플레이어 캐릭터 상태를 나타내는 변수
   private int jumpCount = 0; // 누적 점프 횟수
   private bool isGrounded = false; // 바닥에 닿았는지 나타냄
   private bool isDead = false; // 사망 상태

   //게임 오브젝트의 컴포넌트를 할당할 변수
   private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
   private Animator animator; // 사용할 애니메이터 컴포넌트
   private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

   private void Start() {
        // 게임 오브젝트로부터 사용할 컴포넌트들을 가져와서 변수에 할당
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

   }

   private void Update() {
       // 사용자 입력을 감지하고 점프하는 처리
       if(isDead)
       {
           //사망시 처리를 더이상 진행하지 않고 종료
           return;
       }

       //마우스 왼쪽 버튼을 눌렀으며 && 최대 점프 횟수(2)에 도달하지 않았다면
       if(Input.GetMouseButtonDown(0) && jumpCount < 2)
       {
            //점프 횟수 증사
            jumpCount++;
            //점프 직전에 속도를 순간적으로 제로(0, 0)로 변경
            playerRigidbody.velocity = Vector2.zero;
            //리지드바디에 위쪽으로 힘주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            //오디오 소스 재생
            playerAudio.Play();
       }
       else if(Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
       {
            //마우스 왼쪽 버튼에서 손을 떼는 순가 && 속도의 y 값이 양수라면 (위로 상승중 상태)
            //현재 속도를 절반으로 변경
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
       }

        animator.SetBool("Grounded", isGrounded);
   }

   private void Die() {
        // 애니메이터의 Die 트리거를 파라미터 셋팅
        animator.SetTrigger("Die");
        //오디오 소스에 할당된 오디오 클립을 deathClip으로 변경
        playerAudio.clip = deathClip;
        //사망 효과음 재생
        playerAudio.Play();
        //속도를 제로(0,0)로 변경
        playerRigidbody.velocity = Vector2.zero;
        //사망 상태를 true 변경
        isDead = true;
   }

   private void OnTriggerEnter2D(Collider2D other) {
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
       //충돌한 상대방의 태그가 Dead이며 아직 사망하지 않았다면 Die() 실행
       if (other.tag == "Dead" && !isDead)
       {
            Die();
       }
   }

   private void OnCollisionEnter2D(Collision2D collision) {
       // 바닥에 닿았음을 감지하는 처리
       if(collision.contacts[0].normal.y > 0.7f)
       {
            isGrounded = true;
            jumpCount = 0;
       }
   }

   private void OnCollisionExit2D(Collision2D collision) {
        // 바닥에서 벗어났음을 감지하는 처리
        isGrounded = false;
   }
}