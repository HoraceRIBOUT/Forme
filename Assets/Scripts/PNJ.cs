using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJ : InterestPoint
{
    public List<Influenceur> influenceurAutour;
    //Pour l'instant : s'en foutent de si y a des PNJ

    public float speedNormal = 1;
    private Vector2 lastSpeed = Vector2.zero;
    public float friction = 0.9f;

    private Vector2 seed = new Vector2(1,1);
    public float randomSeedSpeed = 0.1f;

    private void Start()
    {
        seed = new Vector2(Random.Range(0f, 200f), Random.Range(0f, 200f));
    }

    // Update is called once per frame
    void Update()
    {

        SpeedManagement();

    }

    void SpeedManagement()
    {
        Vector2 direction = Vector2.zero;
        direction.x = 0.5f - Mathf.PerlinNoise(seed.x + Time.timeSinceLevelLoad * randomSeedSpeed, seed.y);
        direction.y = 0.5f - Mathf.PerlinNoise(seed.y + Time.timeSinceLevelLoad * randomSeedSpeed, seed.x);
        Debug.DrawRay(this.transform.position, direction.normalized, Color.red);




        //We only touch that direction !!
        if(influenceurAutour.Count > 0)
        {
            //try to reach them or go away from them !
            foreach (Influenceur infl in influenceurAutour)
            {
                if (infl.attractivenessForPNJ == 0)
                    continue;
                Vector2 directionToHim = infl.transform.position - this.transform.position;
                float distDist = directionToHim.sqrMagnitude;
                if (infl.radiusMinMax.y * infl.radiusMinMax.y < distDist)
                    continue;
                float dist = Mathf.Sqrt(distDist);
                float valAttract01 = (dist - infl.radiusMinMax.x) / (infl.radiusMinMax.y - infl.radiusMinMax.x);
                valAttract01 = 1 - valAttract01;
                float valRepuls01 = dist / infl.radiusMinMax.x;
                valRepuls01 = 1 - valRepuls01;
                Debug.DrawRay(this.transform.position, directionToHim, Color.green);
                Debug.DrawRay(this.transform.position, directionToHim * valAttract01, Color.blue);


                if (valRepuls01 > 0 && valRepuls01 < 1)
                {
                    Debug.DrawRay(this.transform.position, directionToHim * valRepuls01, Color.black);
                    //Debug.Log("ValRepuls :" + valRepuls01);
                    direction = Vector2.Lerp(direction, -directionToHim, valRepuls01);
                }
                else
                {
                    direction = Vector2.Lerp(direction, directionToHim * infl.attractivenessForPNJ, valAttract01 * 0.5f);
                }
            }

        }


        Debug.DrawRay(this.transform.position, direction, Color.blue + Color.red);
        Vector2 finalSpeed = direction.normalized * speedNormal;

        //compute speed !! 
        lastSpeed += finalSpeed * Time.deltaTime;
        lastSpeed *= friction;


        Vector3 res = new Vector3(lastSpeed.x, lastSpeed.y, this.transform.position.z);
        this.transform.position += res * Time.deltaTime;
    }
}
