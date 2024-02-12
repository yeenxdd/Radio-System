using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EPARTICLE
{
    SMOKE = 0,
    SMOKE_HEAT,
    SMOKE_BURNED,
    MUSIC,
    FIRE,
    BURNED_EFFECT,
    COOKING_EFFECT
}

[System.Serializable]
public class Particle
{
    public EPARTICLE particleType;
    public AParticle particlePrefab;
}

[CreateAssetMenu(fileName = "SO_particleBlueprint", menuName = "SO/SO_particleBlueprint", order = 1)]
public class SO_particleBlueprint : SingletonScriptableobject<SO_particleBlueprint>
{
    [SerializeField] private List<Particle> m_particleList = null;

    //===============================================


    public AParticle GetParticle(EPARTICLE particleType)
    {
        for(int i = 0; i < this.m_particleList.Count; ++i)
        {
            if(this.m_particleList[i].particleType == particleType)
            {
                return this.m_particleList[i].particlePrefab;
            }
        }

        return null;
    }

    //===============================================
}
