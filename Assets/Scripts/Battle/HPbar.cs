using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool IsUpdating { get; private set; }
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        IsUpdating = true;
        
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            SetHP(curHp);
            yield return null;
        }

        SetHP(newHp);
        IsUpdating = false;
    }
}
