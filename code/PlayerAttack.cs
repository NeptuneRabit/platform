using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("감지 설정 (새로 추가됨)")]
    public float detectionRange = 3.0f; // 적이 이 범위 안에 들어오면 공격 시작

    [Header("공격 설정")]
    public Transform attackPoint;       // 공격 이펙트가 나가는 위치
    public float attackRange = 1.0f;    // 실제로 데미지가 들어가는 범위
    public LayerMask enemyLayers;       // 적(Enemy) 레이어
    public int damage = 20;             // 공격력
    public float attackCooldown = 1.5f; // 공격 속도

    [Header("이펙트 설정")]
    public GameObject attackEffectPrefab;
    public float effectSize = 2.0f; 

    private Animator anim;
    private float timer; 

    void Awake()
    {
        // 자식 오브젝트에 있는 애니메이터를 가져옵니다.
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. 시간 흐름 체크
        timer += Time.deltaTime;

        // 2. 쿨타임이 찼고 + 적이 감지 범위 안에 있을 때만 공격!
        if (timer >= attackCooldown && IsEnemyNearby())
        {
            Attack();
            timer = 0f; // 공격했으니 타이머 초기화
        }
    }

    // 내 주변에 적이 있는지 확인하는 탐지기 함수
    bool IsEnemyNearby()
    {
        // 내 위치(transform.position)를 기준으로 감지 범위(detectionRange) 안에
        // enemyLayers에 해당하는 물체가 하나라도 있으면 true를 반환
        return Physics2D.OverlapCircle(transform.position, detectionRange, enemyLayers) != null;
    }

    void Attack()
    {
        if (attackPoint == null || attackEffectPrefab == null) return;

        // 1. 애니메이션 재생
        if(anim != null) anim.SetTrigger("2_Attack");

        // 2. 이펙트 생성
        GameObject effect = Instantiate(attackEffectPrefab, attackPoint.position, Quaternion.identity, transform);
        effect.transform.localScale = new Vector3(-effectSize, effectSize, effectSize);
        Destroy(effect, 0.5f);

        // 3. 실제 타격 판정 (데미지 주기)
        // 감지 범위보다 짧은 '타격 범위(attackRange)' 안에 있는 적만 맞습니다.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider2D hit in hitEnemies)
        {
            // Enemy 스크립트 찾아서 데미지 주기
            Enemy enemyScript = hit.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }
    }

    // 에디터에서 범위를 눈으로 확인하기 위한 코드
    private void OnDrawGizmosSelected()
    {
        // 1. 감지 범위 (노란색) - 이 안에 들어오면 공격 준비
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 2. 타격 범위 (빨간색) - 이 안에 있어야 데미지를 입음
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}