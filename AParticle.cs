using System.Collections;
using UnityEngine;

public class AParticle : MonoBehaviour
{
    [SerializeField] private float m_aliveDuration = 1.0f;

    private Coroutine m_returnToPoolCO = null;

    //====================================================

    private void OnEnable()
    {
        this.OnEnableAction();
    }

    //====================================================

    public void ForceReturnToPool()
    {
        ObjectPooling.GetInstance().ReturnToPool(this.gameObject);
    }

    protected virtual void OnEnableAction()
    {
        if (this.m_returnToPoolCO != null)
        {
            this.StopCoroutine(this.m_returnToPoolCO);
            this.m_returnToPoolCO = null;
        }

        this.m_returnToPoolCO = this.StartCoroutine(this.ReturnToPool());
    }

    protected virtual IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(this.m_aliveDuration);

        ObjectPooling.GetInstance().ReturnToPool(this.gameObject);
    }

    //====================================================
}
