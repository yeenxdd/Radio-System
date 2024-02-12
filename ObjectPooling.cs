using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonobehaviourSingleton<ObjectPooling>
{
    private Transform m_alivePoolTransform = null;
    private Transform m_deadPoolTransform = null;
    private Transform m_pooltransform = null;

    private Dictionary<Transform, List<GameObject>> m_alivePool = new Dictionary<Transform, List<GameObject>>();
    private Dictionary<Transform, List<GameObject>> m_deadPool = new Dictionary<Transform, List<GameObject>>();

    //================================================

    #region MONOBEHAVIOUR

    protected override void Awake()
    {
        base.Awake();
        this.GeneratePoolParentsAndCache();
    }

    #endregion MONOBEHAVIOUR

    //================================================

    #region FUNCTIONS

    //Public function below

    #region PUBLIC

    public GameObject GetFromPool(GameObject objectReference)
    {
        GameObject poolObject = null;

        foreach (var key in this.m_deadPool.Keys)
        {
            if (key.gameObject.name == objectReference.name.ToString()) //Means found key
            {
                if (this.m_deadPool[key].Count > 0)
                {
                    poolObject = this.m_deadPool[key][0];
                    this.m_deadPool[key].Remove(poolObject);
                    foreach (var keyAlivePool in this.m_alivePool.Keys)
                    {
                        if (keyAlivePool.gameObject.name == objectReference.name.ToString())
                        {
                            //found key
                            poolObject.transform.parent = keyAlivePool;
                            this.m_alivePool[keyAlivePool].Add(poolObject);
                            return poolObject;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        //If until this stage still no return, means no key from dead pool now check for alive pool
        foreach (var keyAlivePool in this.m_alivePool.Keys)
        {
            if (keyAlivePool.gameObject.name == objectReference.name.ToString())
            {
                //found key
                poolObject = Instantiate(objectReference, keyAlivePool);
                poolObject.name = objectReference.name.ToString();
                this.m_alivePool[keyAlivePool].Add(poolObject);
                return poolObject;
            }
        }

        //If still no, means alive pool also no key
        //Generate a transform first
        Transform newKey = new GameObject().GetComponent<Transform>();
        newKey.gameObject.name = objectReference.name.ToString();
        newKey.parent = this.m_alivePoolTransform;
        poolObject = Instantiate(objectReference, newKey);
        poolObject.name = objectReference.name.ToString();

        List<GameObject> childrenList = new List<GameObject>();
        childrenList.Add(poolObject);

        this.m_alivePool.Add(newKey, childrenList);

        return poolObject;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        foreach (var keyAlivePool in this.m_alivePool.Keys)
        {
            if (keyAlivePool.gameObject.name == objectToReturn.name.ToString())
            {
                //found key
                if (this.m_alivePool[keyAlivePool].Contains(objectToReturn))
                {
                    foreach (var keyDeadPool in this.m_deadPool.Keys)
                    {
                        if (keyDeadPool.gameObject.name == objectToReturn.name.ToString()) //Means found key
                        {
                            objectToReturn.transform.parent = keyDeadPool;
                            this.m_alivePool[keyAlivePool].Remove(objectToReturn);
                            this.m_deadPool[keyDeadPool].Add(objectToReturn);
                            return;
                        }
                    }

                    //It means no found key on dead pool

                    //Generate a transform first
                    Transform newKeyInner = new GameObject().GetComponent<Transform>();
                    newKeyInner.gameObject.name = objectToReturn.name.ToString();
                    newKeyInner.parent = this.m_deadPoolTransform;
                    objectToReturn.transform.parent = newKeyInner;
                    List<GameObject> childrenListInner = new List<GameObject>();
                    childrenListInner.Add(objectToReturn);

                    this.m_deadPool.Add(newKeyInner, childrenListInner);
                    return;
                }
            }
        }

        Debug.LogWarning("Not found key, something went wrong");

        Transform newKey = new GameObject().GetComponent<Transform>();
        newKey.gameObject.name = objectToReturn.name.ToString();
        newKey.parent = this.m_deadPoolTransform;
        objectToReturn.transform.parent = newKey;
        List<GameObject> childrenList = new List<GameObject>();
        childrenList.Add(objectToReturn);

        this.m_deadPool.Add(newKey, childrenList);
        return;
    }

    public void RemoveAllObjectToDeadPool()
    {
        foreach (var keyAlivePool in this.m_alivePool.Keys)
        {
            foreach (GameObject alivepoolgo in this.m_alivePool[keyAlivePool])
            {
                Destroy(alivepoolgo);
            }
            this.m_alivePool[keyAlivePool].Clear();
        }
    }

    #endregion PUBLIC

    //Private function below

    #region PRIVATE

    private void GeneratePoolParentsAndCache()
    {
        this.m_pooltransform = this.transform;

        //Generate the child pool
        this.m_alivePoolTransform = new GameObject().GetComponent<Transform>();// Instantiate(new GameObject(), this.m_pooltransform).GetComponent<Transform>();
        this.m_alivePoolTransform.parent = this.m_pooltransform;
        this.m_alivePoolTransform.gameObject.name = "Alive_Pool";
        this.m_deadPoolTransform = new GameObject().GetComponent<Transform>();//Instantiate(new GameObject(), this.m_pooltransform).GetComponent<Transform>();
        this.m_deadPoolTransform.parent = this.m_pooltransform;
        this.m_deadPoolTransform.gameObject.name = "Dead_Pool";
        this.m_deadPoolTransform.gameObject.SetActive(false);
    }

    #endregion PRIVATE

    #endregion FUNCTIONS

    //================================================
}
