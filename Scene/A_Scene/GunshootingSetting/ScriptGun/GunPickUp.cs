using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public Transform controller;             // 控制器 Transform
    public Transform gun;                    // 枪的 Transform
    public float pickupDistance = 0.5f;      // 捡枪的最大距离
    private bool isPickedUp = false;         // 枪是否被捡起

    private Rigidbody gunRigidbody;          // 枪的 Rigidbody

    void Start()
    {
        gunRigidbody = gun.GetComponent<Rigidbody>();
        gunRigidbody.isKinematic = false; // 开启物理模拟
    }

    void Update()
    {
        float distance = Vector3.Distance(controller.position, gun.position);

        // 检测捡枪逻辑
        if (distance <= pickupDistance && Input.GetAxis("Grip") > 0.5f)
        {
            if (!isPickedUp)
            {
                isPickedUp = true;
                PickupGun();
            }
        }
        else if (Input.GetAxis("Grip") <= 0.5f)
        {
            if (isPickedUp)
            {
                isPickedUp = false;
                DropGun();
            }
        }
    }

    private void PickupGun()
    {
        gunRigidbody.isKinematic = true; // 停止物理模拟
        gun.SetParent(controller);      // 将枪绑定到控制器
    }

    private void DropGun()
    {
        gunRigidbody.isKinematic = false; // 恢复物理模拟
        gun.SetParent(null);              // 解除绑定
    }

    public bool IsGunPickedUp()
    {
        return isPickedUp;
    }
}
