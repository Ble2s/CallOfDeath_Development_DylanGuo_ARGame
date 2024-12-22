using UnityEngine;

public class SmokeInteraction_3 : MonoBehaviour
{
    public Transform leftControllerMesh;   // 手柄的 mesh
    public GameObject smokeEffect;          // 烟雾特效
    public GameObject blowDryer;            // 吹风机物体
    public float influenceRadius = 1.0f;    // 控制器与烟雾交互的半径
    public float disperseSpeed = 5.0f;      // 普通模式下烟雾挥散的速度
    public float blowDryerPower = 10.0f;    // 吹风机的强风驱散速度
    public AudioSource blowDryerSound;      // 吹风机音效

    private bool isHoldingBlowDryer = false; // 是否捡起了吹风机
    private Vector3 lastControllerPosition; // 上一帧控制器的位置

    void Start()
    {
        // 初始化位置
        lastControllerPosition = leftControllerMesh.position;

        // 检查必要的绑定
        if (smokeEffect == null || leftControllerMesh == null)
        {
            Debug.LogError("烟雾特效或控制器未绑定！");
            enabled = false;
            return;
        }

        if (blowDryer == null)
        {
            Debug.LogWarning("吹风机物体未绑定，将无法使用吹风机模式。");
        }
    }

    void Update()
    {
        // 检测是否靠近吹风机并尝试捡起
        if (!isHoldingBlowDryer && blowDryer != null)
        {
            TryPickUpBlowDryer();
        }

        // 当前控制器位置
        Vector3 currentPosition = leftControllerMesh.position;

        // 计算控制器与烟雾的距离
        float distance = Vector3.Distance(currentPosition, smokeEffect.transform.position);

        if (distance < influenceRadius)
        {
            // 计算挥手方向（当前位置 - 上一帧位置）
            Vector3 swipeDirection = (currentPosition - lastControllerPosition).normalized;

            if (isHoldingBlowDryer)
            {
                // 吹风机模式
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) // 监听手柄 Trigger 按键
                {
                    BlowSmoke(swipeDirection, blowDryerPower);

                    // 播放吹风机音效
                    if (blowDryerSound != null && !blowDryerSound.isPlaying)
                    {
                        blowDryerSound.Play();
                    }
                }
                else if (blowDryerSound != null && blowDryerSound.isPlaying)
                {
                    blowDryerSound.Stop();
                }
            }
            else
            {
                // 普通模式
                BlowSmoke(swipeDirection, disperseSpeed);
            }
        }

        // 更新上一帧位置
        lastControllerPosition = currentPosition;
    }

    private void TryPickUpBlowDryer()
    {
        // 检测控制器与吹风机的距离
        float blowDryerDistance = Vector3.Distance(leftControllerMesh.position, blowDryer.transform.position);

        if (blowDryerDistance < 0.5f && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)) // 监听手柄 Grip 按键
        {
            isHoldingBlowDryer = true;

            // 将吹风机附加到控制器
            blowDryer.transform.SetParent(leftControllerMesh);
            blowDryer.transform.localPosition = new Vector3(0, 0, 0.2f); // 调整吹风机在控制器上的位置
            blowDryer.transform.localRotation = Quaternion.identity;

            Debug.Log("捡起了吹风机！");
        }
    }

    private void BlowSmoke(Vector3 direction, float speed)
    {
        // 获取烟雾粒子系统
        ParticleSystem particleSystem = smokeEffect.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("烟雾特效缺少粒子系统！");
            return;
        }

        // 启用粒子速度模块
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;

        // 设置粒子向挥动方向移动
        velocityOverLifetime.x = direction.x * speed;
        velocityOverLifetime.y = direction.y * speed;
        velocityOverLifetime.z = direction.z * speed;
    }
}
