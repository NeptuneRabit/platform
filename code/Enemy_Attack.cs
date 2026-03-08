using UnityEngine;
using System.Collections;

public class Enemy_Attack : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; 
    SpriteRenderer sprite; 
    private Animator anim;

    [Header("상태 설정")]
    [SerializeField] int health = 3; 
    private bool isDead = false; 

    [Header("추적 설정")]
    public Transform target;          
    public float detectionRange = 5f; // 발견 범위
    public float chaseSpeed = 1.5f;   

    [Header("공격 설정 (핵심)")]
    public float attackRange = 1.0f;  // 공격 사정거리 (이 안에 들어오면 공격 시도)
    public int damage = 10;           // 공격력
    public float attackCooldown = 2f; // 공격 쿨타임
    public float hitDelay = 0.5f;     // ★ 공격 모션 시작 후 데미지가 들어갈 때까지의 시간 (박자 맞추기)
    
    private bool isChasing = false;   
    private float lastAttackTime;     

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        // 타겟 자동 찾기
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        Think();
    }

    void FixedUpdate()
    {
        if (isDead || target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);

        // 1. 공격 사정거리 안 -> 멈춰서 때리기
        if (dist <= attackRange)
        {
            StopMoving();
            TryAttack(); 
        }
        // 2. 발견 범위 안 -> 쫓아가기
        else if (dist <= detectionRange)
        {
            isChasing = true;
            StopWaitThink();
            ChasePlayer();
        }
        // 3. 평화 상태 -> 배회
        else
        {
            if (isChasing)
            {
                isChasing = false;
                Think();
            }
            rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
            CheckCliff();
        }

        // 애니메이션
        if (anim != null) anim.SetBool("isRun", nextMove != 0);
        if (nextMove != 0) sprite.flipX = (nextMove == -1);
    }

    // ★ 공격 시도 (모션 재생 -> 예약 걸기)
    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // 1. 공격 애니메이션 재생
            if (anim != null) anim.SetTrigger("Attack");

            // 2. 데미지는 바로 주지 않고, 'hitDelay'초 뒤에 줍니다. (박자 맞추기)
            Invoke("DeliverDamage", hitDelay);
        }
    }

    // ★ 실제로 데미지를 주는 함수 (Invoke에 의해 호출됨)
    void DeliverDamage()
    {
        // 죽었거나 타겟이 사라졌으면 취소
        if (isDead || target == null) return;

        // 공격 타이밍이 됐을 때, 아직도 플레이어가 사정거리(혹은 조금 더 넓게) 안에 있는지 확인
        float dist = Vector2.Distance(transform.position, target.position);
        
        // "공격 모션이 끝날 때쯤에도 플레이어가 근처(attackRange + 0.5f)에 있다면 데미지!"
        if (dist <= attackRange + 0.5f)
        {
            PlayerHealth pHealth = target.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                pHealth.TakeDamage(damage);
                Debug.Log("⚔️ 몬스터의 공격 적중!");
            }
        }
    }

    void StopMoving()
    {
        rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
        nextMove = 0;
    }

    void ChasePlayer()
    {
        float direction = target.position.x - transform.position.x;
        nextMove = direction > 0 ? 1 : -1;
        rigid.linearVelocity = new Vector2(nextMove * chaseSpeed, rigid.linearVelocity.y);
    }

    void StopWaitThink() { CancelInvoke("Think"); }

    void CheckCliff()
    {
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1.5f, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null) TurnAround();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        StartCoroutine(HitFlash());
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
    StopMoving();
    
    // 죽는 애니메이션 재생
    if(anim != null) anim.SetTrigger("Die");
    
    // 1. 플레이어가 밟지 않고 지나가게 하기 (충돌체 끄기)
    Collider2D col = GetComponent<Collider2D>();
    if(col != null) col.enabled = false;

    // 2. ★ 중요: 땅으로 꺼지지 않게 하기 (물리 엔진 끄기)
    if(rigid != null)
    {
        rigid.linearVelocity = Vector2.zero;        // 움직임 즉시 정지
        rigid.bodyType = RigidbodyType2D.Kinematic; // 물리 영향 안 받게 설정 (중력 무시)
    }

    // 1.5초 뒤에 사라짐
    Destroy(gameObject, 1.5f);
}

    void TurnAround()
    {
        nextMove *= -1; 
        CancelInvoke("Think");
        Invoke("Think", 2);
    }

    void Think()
    {
        if (isDead || isChasing) return;
        nextMove = Random.Range(-1, 2);
        Invoke("Think", Random.Range(2f, 5f));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}