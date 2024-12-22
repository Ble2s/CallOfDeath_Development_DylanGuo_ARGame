using UnityEngine;

public class BlowSmoke : MonoBehaviour
{
    public ParticleSystem blowerParticle; // 粒子系统
    public float repelRadius = 5f; // 鼠标驱散范围
    public float repelForce = 50f; // 驱散力强度
    public Camera mainCamera; // 主摄像机

    private void Start()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // 获取鼠标位置
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10f)); // 假定粒子系统在 z = 10 附近

        // 获取粒子系统中的粒子
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[blowerParticle.particleCount];
        int particleCount = blowerParticle.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 toMouse = particles[i].position - mouseWorldPosition;

            // 只对驱散范围内的粒子施加力
            if (toMouse.magnitude < repelRadius)
            {
                // 施加力的方向从鼠标位置向粒子反向
                Vector3 forceDirection = toMouse.normalized;

                // 增加粒子的速度，模拟驱散
                particles[i].velocity += forceDirection * repelForce * Time.deltaTime;
            }
        }

        // 更新粒子
        blowerParticle.SetParticles(particles, particleCount);
    }
}

