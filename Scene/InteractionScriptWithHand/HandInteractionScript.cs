using UnityEngine;
using Oculus.Interaction;

public class HandSmokeInteraction : MonoBehaviour
{
    [SerializeField] private ParticleSystem smokeEffect; // 目标烟雾粒子系统
    [SerializeField] private Transform leftHand; // 左手Transform
    [SerializeField] private Transform rightHand; // 右手Transform
    [SerializeField] private float interactionRadius = 0.5f; // 手势影响的半径

    private ParticleSystem.Particle[] particles; // 粒子缓存
    private int maxParticles = 1000; // 最大粒子数

    void Update()
    {
        // 检测手的位置是否在烟雾附近
        CheckHandInteraction(leftHand);
        CheckHandInteraction(rightHand);
    }

    private void CheckHandInteraction(Transform hand)
    {
        if (smokeEffect == null || hand == null) return;

        // 获取当前手的位置
        Vector3 handPosition = hand.position;

        // 获取烟雾粒子
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        int numParticles = smokeEffect.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            float distance = Vector3.Distance(particles[i].position, handPosition);
            if (distance <= interactionRadius)
            {
                // 推动烟雾粒子远离手的位置
                Vector3 direction = (particles[i].position - handPosition).normalized;
                particles[i].position += direction * Time.deltaTime * 2f; // 速度可以调整
            }
        }

        // 更新粒子
        smokeEffect.SetParticles(particles, numParticles);
    }
}
