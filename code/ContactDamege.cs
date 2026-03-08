using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    public int damage = 10; // 플레이어에게 줄 데미지

    // 무언가와 부딪혔을 때 실행됨 (몸통 박치기)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 대상이 'Player'라는 태그를 가지고 있다면?
        if (collision.gameObject.CompareTag("Player"))
        {
            // 그 대상에게서 PlayerHealth 스크립트를 찾아라!
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // 찾았다면 데미지를 줘라!
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}