using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public PlayerController playerController;
   
    public void Shoot()
    {
        playerController.Shoot();
    }
}
