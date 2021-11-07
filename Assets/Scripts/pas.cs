using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pas : MonoBehaviour
{
    public SpriteRenderer pasSR;
    public AnimationCurve curve;
    public float duration = 1;
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > duration)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Color ol = pasSR.color;
            ol.a = curve.Evaluate(timer / duration);
            pasSR.color = ol;
        }
    }
}
