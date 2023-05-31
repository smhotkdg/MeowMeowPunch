using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField]
    ParticleSystem movementParticle;
    [Range(0, 10f)]
    [SerializeField]
    int occurAfterVelocity;
    [Range(0, 0.2f)]
    [SerializeField]
    float dustFormationPeriod;
    [SerializeField]
    FloatingJoystick Joystick;
    [SerializeField]
    SpriteRenderer Player;
    float counter;

    private void Update()
    {
        counter += Time.deltaTime;
        if(Player.flipX)
        {
            movementParticle.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            movementParticle.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        if(Mathf.Abs(Joystick.Horizontal) + Mathf.Abs(Joystick.Vertical) > occurAfterVelocity)
        {
            if(counter > dustFormationPeriod)
            {
                movementParticle.gameObject.SetActive(true);
                movementParticle.Play();
                counter = 0;
            }
        }
    }
}
