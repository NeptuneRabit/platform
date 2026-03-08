using UnityEngine;

public class SwordEffect : MonoBehaviour
{
    void Start() {
        // 0.3초 뒤에 이펙트 자동 삭제 (애니메이션 길이에 맞춰 조절)
        Destroy(gameObject, 0.3f); 
    }

    

}

