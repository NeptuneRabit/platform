using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public int damage;
    public void SetDamage(int playerDamage)
    {
        damage = playerDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 일단 태그가 "Enemy"인지는 먼저 확인 (성능 최적화)
        if (collision.CompareTag("Enemy"))
        {
            // [방식 A] 슬라임(Enemy) 스크립트가 있는지 확인
            Enemy slime = collision.GetComponent<Enemy>();
            if (slime != null)
            {
                slime.TakeDamage(damage);
                return; // 데미지를 줬다면 여기서 중단
            }

            // [방식 B] 벌이나 공격형 몹(Enemy_Attack) 스크립트가 있는지 확인
            Enemy_Attack attacker = collision.GetComponent<Enemy_Attack>();
            if (attacker != null)
            {
                attacker.TakeDamage(damage);
                return;
            }
            
            // 앞으로 새로운 적 대본(예: Boss)이 생기면 여기에 추가하면 됩니다!
        }
    }
}