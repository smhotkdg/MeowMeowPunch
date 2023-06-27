using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterParticleController : MonoBehaviour
{
    public SpriteRenderer ImageSprite;
    public SpriteRenderer ShadowSprite;

    public List<Sprite> MechanicList;
    public List<Sprite> CreatureList;

    public void SetMonster(Monster.MonsterParticleType particleType)
    {
        switch(particleType)
        {
            case Monster.MonsterParticleType.Creature:
                ImageSprite.sprite = CreatureList[Random.Range(0, CreatureList.Count)];
                ShadowSprite.sprite = CreatureList[Random.Range(0, CreatureList.Count)];
                break;
            case Monster.MonsterParticleType.Mechanic:
                ImageSprite.sprite = MechanicList[Random.Range(0, CreatureList.Count)];
                ShadowSprite.sprite = MechanicList[Random.Range(0, CreatureList.Count)];
                break;
        }
    }
}
