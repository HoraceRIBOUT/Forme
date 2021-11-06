using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float ZVal;


    public Transform player;
    public List<InterestPoint> interestList = new List<InterestPoint>();

    [Header("Zone")]
    public float minDist = 0;
    public float maxDist = 10;

    public Vector3 currentTarget = new Vector3(0,0,-10);

    public float speed = 2f;

    public void Start()
    {
        interestList = new List<InterestPoint>();
        foreach(InterestPoint point in FindObjectsOfType<InterestPoint>())
        {
            interestList.Add(point);
        }
    }

    public void Update()
    {
        ComputeDist();


        this.transform.position = Vector3.Lerp(this.transform.position, currentTarget, Time.deltaTime * speed);
    }

    public void ComputeDist()
    {
        float sumInterest = 0;
        Vector3 sumPos = Vector3.zero;
        foreach (InterestPoint point in interestList)
        {
            float distDist = (point.transform.position - player.position).sqrMagnitude;
            if (distDist < maxDist * maxDist)
            {
                float dist = Mathf.Sqrt(distDist);
                float value01 = (dist - minDist) / (maxDist - minDist);
                value01 = 1 - value01; 
                if (value01 < 0 || value01 > 1) Debug.Log("Value01 wrong " + value01);
                //else Debug.Log("Value01 good but... " + value01);
                sumInterest += point.interest * value01;
                sumPos += point.transform.position * point.interest * value01;
            }
        }
        currentTarget = sumPos / sumInterest;

        currentTarget.z = -10;
    }
}
