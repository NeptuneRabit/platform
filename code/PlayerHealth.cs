using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;    
    public int currentHealth;      

    [Header("UI 연결")]
    public Slider healthSlider;    

    [Header("★ 중요: 죽으면 꺼버릴 스크립트들")]
    // 여기에 Player(이동) 스크립트랑 PlayerAttack(공격) 스크립트를 드래그해서 넣으세요!
    public MonoBehaviour moveScript;   // 이동 스크립트 (이름 몰라도 됨)
    public MonoBehaviour attackScript; // 공격 스크립트

    private bool isDead = false; // 죽었는지 확인하는 도장

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        // 이미 죽었으면 데미지 받지 마! (부관참시 방지)
        if (isDead) return;

        currentHealth -= damage;
        // Debug.Log("으악! 남은 체력: " + currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        if (isDead) return; // 두 번 죽는 거 방지

        isDead = true; // "나 죽었다" 도장 쾅!
        Debug.Log("💀 플레이어 사망!");

        // 1. 애니메이션: 강제로 DEATH 재생
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Play("DEATH"); 
        }
        else
        {
            Debug.LogError("애니메이터를 못 찾았습니다!");
        }

        // 2. ★ 핵심: 이동 & 공격 스크립트 끄기 (드래그로 연결한 것들)
        if (moveScript != null) moveScript.enabled = false;
        if (attackScript != null) attackScript.enabled = false;

        // 3. 물리 엔진 끄기 (밀리지 않게)
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        if (rigid != null)
        {
            rigid.linearVelocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }

        // 4. 충돌체 끄기 (몬스터가 밟지 않게)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}