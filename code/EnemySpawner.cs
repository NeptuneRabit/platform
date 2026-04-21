using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject[] enemyPrefabs; // 소환할 적들 (슬라임, 벌)
    public float spawnInterval = 2.0f; // 몇 초마다 나올지

    [Header("소환 구역")]
    public BoxCollider2D spawnArea; // 이 네모난 구역 안에서 나옵니다

    private void Start()
    {
        // 게임 시작하면 바로 소환 루틴 시작!
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true) // 무한 반복
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval); // 대기
        }
    }

    void SpawnEnemy()
    {
        // 1. 소환할 적을 랜덤으로 고릅니다 (슬라임 vs 벌)
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedEnemy = enemyPrefabs[randomIndex];

        // 2. 박스 콜라이더의 범위 안에서 랜덤한 좌표를 계산합니다.
        Bounds bounds = spawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        // 3. 적 소환!
        Instantiate(selectedEnemy, spawnPos, Quaternion.identity);
    }
}