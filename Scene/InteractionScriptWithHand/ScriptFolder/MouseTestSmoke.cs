using UnityEngine;
using UnityEngine.InputSystem; // 新输入系统命名空间

public class SmokeDisperse : MonoBehaviour
{
    public ParticleSystem smokeParticle;
    public float repelRadius = 5f; // 控制器的驱散半径
    public float repelForce = 5f; // 驱散力度

    private Transform controllerTransform;

    void Update()
    {
        // 模拟控制器的位置
        if (controllerTransform == null)
        {
            controllerTransform = new GameObject("Controller").transform;
        }

        // 使用新输入系统获取鼠标位置
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10f);
        controllerTransform.position = Camera.main.ScreenToWorldPoint(mousePosition);

        // 获取粒子系统的粒子数据
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[smokeParticle.particleCount];
        int count = smokeParticle.GetParticles(particles);

        // 驱散粒子
        for (int i = 0; i < count; i++)
        {
            Vector3 toController = particles[i].position - controllerTransform.position;
            if (toController.magnitude < repelRadius)
            {
                particles[i].velocity += toController.normalized * repelForce;
            }
        }

        smokeParticle.SetParticles(particles, count);
    }
}
