using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastGunNEw : MonoBehaviour
{
    public Camera playerCamera;           // 玩家相机
    public Transform laserOrigin;         // 激光起点
    public Transform gun;                 // 枪的 Transform
    public Transform controller;          // 控制器的 Transform
    public float pickupDistance = 0.5f;   // 捡起枪的距离
    public float gunRange = 50f;          // 射击距离
    public float fireRate = 0.2f;         // 射击速率
    public float laserDuration = 0.05f;   // 激光持续时间

    private LineRenderer laserLine;       // 激光的 LineRenderer
    private float fireTimer;              // 射击计时器
    private bool isPickedUp = false;      // 是否捡起了枪

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        // 检测控制器和枪的距离
        float distance = Vector3.Distance(controller.position, gun.position);
        if (distance <= pickupDistance && Input.GetAxis("Grip") > 0.5f)
        {
            isPickedUp = true; // 捡起枪
        }
        else if (Input.GetAxis("Grip") <= 0.5f)
        {
            isPickedUp = false; // 松开 Grip 键后放下枪
        }

        // 射击逻辑：必须在捡起枪后，按下 Trigger 键
        if (isPickedUp && Input.GetAxis("Trigger") > 0.5f && fireTimer > fireRate)
        {
            fireTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        // 激光起点设置
        laserLine.SetPosition(0, laserOrigin.position);

        // 获取 Raycast 起点和方向
        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 发射射线检测
        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point); // 激光终点
            Destroy(hit.transform.gameObject);  // 销毁目标
        }
        else
        {
            laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * gunRange)); // 无目标时延伸
        }

        StartCoroutine(ShootLaser());
    }

    private IEnumerator ShootLaser()
    {
        laserLine.enabled = true; // 启用激光
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false; // 禁用激光
    }
}
