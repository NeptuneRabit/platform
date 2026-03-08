using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; 
    SpriteRenderer sprite; 
    private Animator anim;

    [Header("상태 설정")]
    [SerializeField] int health = 3; 
    private bool isDead = false; 

    [Header("추적 설정")]
    public Transform target;          // 추적 대상 (Player)
    public float detectionRange = 5f; // 플레이어를 감지할 범위
    public float chaseSpeed = 1.5f;   // 추적 시 이동 속도 (기본 이동보다 약간 빠르게 추천)
    private bool isChasing = false;   // 현재 추적 중인지 확인

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 시작 시 플레이어를 자동으로 찾아 타겟으로 설정 (태그가 "Player"여야 함)
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) target = player.transform;

        Think();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // 1. 플레이어와의 거리 체크
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= detectionRange)
        {
            // 감지 범위 안이면 추적 시작
            isChasing = true;
            StopWaitThink(); // 랜덤 배회 중단
            ChasePlayer();
        }
        else
        {
            // 범위를 벗어나면 다시 배회 (추적 중이었다면 다시 배회 시작)
            if (isChasing)
            {
                isChasing = false;
                Think();
            }
            rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
        }

        // 애니메이션 및 방향 전환
        if (anim != null) anim.SetBool("isRun", nextMove != 0);
        if (nextMove != 0) sprite.flipX = (nextMove == 1);

        // 2. 낭떠러지 감지 (추적 중에는 낭떠러지 앞에서 멈추거나 되돌아가는 로직이 필요할 수 있음)
        CheckCliff();
    }

    void ChasePlayer()
    {
        // 플레이어의 위치에 따라 방향 결정 (-1 또는 1)
        float direction = target.position.x - transform.position.x;
        nextMove = direction > 0 ? 1 : -1;

        // 추적 속도 적용
        rigid.linearVelocity = new Vector2(nextMove * chaseSpeed, rigid.linearVelocity.y);
    }

    void StopWaitThink()
    {
        CancelInvoke("Think");
    }

    void CheckCliff()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null)
        {
            if (isChasing) 
            {
                // 추적 중 낭떠러지를 만나면 일단 멈춤 (자살 방지)
                rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
                nextMove = 0;
            }
            else
            {
                TurnAround();
            }
        }
    }

    // --- 기존 TakeDamage, Die, TurnAround, Think 함수는 그대로 유지 ---
    // (이전 코드와 동일하므로 지면상 생략합니다.)
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        StopCoroutine("HitFlash");
        StartCoroutine("HitFlash");
        if (health <= 0) Die();
    }

    IEnumerator HitFlash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("isRun", false);
        anim.SetTrigger("Die");
        Destroy(gameObject, 1.5f);
    }

    void TurnAround()
    {
        nextMove *= -1; 
        CancelInvoke("Think");
        Invoke("Think", 5);
    }

    void Think()
    {
        if (isDead || isChasing) return;
        nextMove = Random.Range(-1, 2);
        Invoke("Think", 5);
    }
}