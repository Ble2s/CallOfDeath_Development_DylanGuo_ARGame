using System.Collections.Generic;
using UnityEngine;

public class SmokeDisperseController : MonoBehaviour
{
    public ParticleSystem smokeParticle;    // 烟雾粒子系统
    public GameObject blowerObject;         // 吹风机道具

    public float mouseRepelForce = 5f;      // 鼠标驱散力度
    public float blowerRepelForce = 10f;    // 吹风机驱散力度
    public float repelRadius = 0.2f;        // 驱散半径

    private Transform blowerTransform;      // 吹风机的 Transform
    private Vector3 lastMousePosition;      // 上一次鼠标位置
    private float currentRepelForce;        // 当前驱散力度

    // 定义驱散模式
    private enum DisperseMode { Mouse, Blower }
    private DisperseMode currentMode = DisperseMode.Mouse; // 默认使用鼠标模式

    void Start()
    {
        // 初始化吹风机
        if (blowerObject != null)
        {
            blowerTransform = blowerObject.transform;
        }
        currentRepelForce = mouseRepelForce;
    }

    void Update()
    {
        // 切换驱散模式
        if (Input.GetKeyDown(KeyCode.Tab)) // 按下 Tab 键切换模式
        {
            ToggleDisperseMode();
        }

        // 根据模式执行不同的驱散行为
        if (currentMode == DisperseMode.Mouse)
        {
            UpdateBlowerPositionWithMouse();
            currentRepelForce = mouseRepelForce;
        }
        else if (currentMode == DisperseMode.Blower)
        {
            UpdateBlowerPositionWithBlower();
            currentRepelForce = blowerRepelForce;
        }

        // 应用驱散效果
        ApplyBlowerForceToParticles();
    }

    // 切换驱散模式
    private void ToggleDisperseMode()
    {
        if (currentMode == DisperseMode.Mouse)
        {
            currentMode = DisperseMode.Blower;
            Debug.Log("Switched to Blower Mode");
        }
        else
        {
            currentMode = DisperseMode.Mouse;
            Debug.Log("Switched to Mouse Mode");
        }
    }

    // 用鼠标控制吹风机的位置
    private void UpdateBlowerPositionWithMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // 鼠标 Z 值，确保位于相机前方
        blowerTransform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    // 用吹风机控制（AR设备位置或手动模拟）
    private void UpdateBlowerPositionWithBlower()
    {
        // 示例：吹风机的物理位置可以通过手动设置或外部控制器更新
        // 在 AR 模式下，这里可以使用 ARFoundation 提供的设备位置。
        blowerTransform.position += Vector3.right * Time.deltaTime; // 示例：吹风机移动
    }

    // 驱散烟雾
    private void ApplyBlowerForceToParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[smokeParticle.particleCount];
        int count = smokeParticle.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 toBlower = particles[i].position - blowerTransform.position;
            if (toBlower.magnitude < repelRadius)
            {
                particles[i].velocity += toBlower.normalized * currentRepelForce;
            }
        }

        smokeParticle.SetParticles(particles, count);
    }
}
