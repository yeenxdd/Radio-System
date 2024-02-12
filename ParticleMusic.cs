using System.Collections;

public class ParticleMusic : AParticle
{
    protected override void OnEnableAction()
    {
        //base.OnEnableAction();
    }

    protected override IEnumerator ReturnToPool()
    {
        return base.ReturnToPool();
    }
}
