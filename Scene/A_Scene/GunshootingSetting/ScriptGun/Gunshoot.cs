using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GunShoot : MonoBehaviour
{
    public Transform laserOrigin;        // 激光起点（枪口的空物体）
    public float gunRange = 50f;         // 射击距离
    public float fireRate = 0.2f;        // 射击间隔
    public float laserDuration = 0.05f; // 激光持续时间

    private LineRenderer laserLine;
    private float fireTimer;
    private GunPickup gunPickupScript;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    void Start()
    {
        // 获取捡枪脚本
        gunPickupScript = FindObjectOfType<GunPickup>();
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        // 检测是否捡起枪，并且按下 Trigger 键
        if (gunPickupScript.IsGunPickedUp() && Input.GetAxis("Trigger") > 0.5f && fireTimer > fireRate)
        {
            fireTimer = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        // 激光起点设置
        laserLine.SetPosition(0, laserOrigin.position);

        // 从枪口发射射线
        RaycastHit hit;
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point); // 激光终点
            Destroy(hit.transform.gameObject);  // 销毁目标
        }
        else
        {
            laserLine.SetPosition(1, laserOrigin.position + (laserOrigin.forward * gunRange)); // 激光未命中时延伸
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
