using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    public Transform cam;           // 메인 카메라
    public float parallaxEffect = 0.1f;    // 0은 고정, 1은 카메라와 동일 속도 (보통 0.1~0.8 사이 설정)

    private float startPos;

    void Start()
    {
        startPos = transform.position.x;
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 카메라가 이동한 거리만큼 배경을 미세하게 이동시킴
        float distance = (cam.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}