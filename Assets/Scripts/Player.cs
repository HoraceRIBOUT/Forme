using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Input")]
    public List<KeyCode> upCode;
    public List<KeyCode> downCode;
    public List<KeyCode> leftCode;
    public List<KeyCode> rightCode;

    [Header("Movement")]
    public float speedNormal = 1;
    private Vector2 lastSpeed = Vector2.zero;
    public float friction = 0.9f;

    public Transform camFocus;
    public float ecart = 0.5f;
    public float camFocusDelay = 0.5f;
    private Vector2 lastDirection = Vector2.zero;
    private Vector2 targetDirection = Vector2.zero;

    [Header("Influenceur")]
    public List<Influenceur> influenceurList = new List<Influenceur>();
    public float maximalInfluence = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        influenceurList = new List<Influenceur>();
        foreach (Influenceur influenceur in FindObjectsOfType<Influenceur>())
        {
            influenceurList.Add(influenceur);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovementManagement();

        Debug();
    }

    public void MovementManagement()
    {
        Vector2 direction = Vector2.zero;
        if (GetKey(upCode))
        {
            direction.y = 1;
        }
        if (GetKey(downCode))
        {
            direction.y = -1;
        }
        if (GetKey(leftCode))
        {
            direction.x = -1;
        }
        if (GetKey(rightCode))
        {
            direction.x = 1;
        }

        direction = InfluenceurZone(direction);


        Vector2 finalSpeed = direction.normalized * speedNormal;

        //compute speed !! 
        lastSpeed += finalSpeed * Time.deltaTime;
        lastSpeed *= friction;

        Vector3 res = new Vector3(lastSpeed.x, lastSpeed.y, this.transform.position.z);
        this.transform.position += res * Time.deltaTime;
        if (direction != Vector2.zero)
            targetDirection = direction.normalized;

        lastDirection = Vector2.Lerp(lastDirection, targetDirection, Time.deltaTime * camFocusDelay);
        camFocus.position = this.transform.position + (Vector3)lastDirection * ecart;
        //        camFocus.position = Vector3.Lerp(camFocus.position, this.transform.position + (Vector3)lastDirection * ecart, Time.deltaTime * camFocusDelay);
    }


    public Vector2 InfluenceurZone(Vector2 direction)
    {
        foreach (Influenceur infl in influenceurList)
        {
            if (infl.attractivenessForYou == 0)
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

            //Debug.DrawRay(this.transform.position, directionToHim, Color.green);
            //Debug.DrawRay(this.transform.position, directionToHim * valAttract01, Color.blue);


            if (valRepuls01 > 0 && valRepuls01 < 1)
            {
                //Debug.DrawRay(this.transform.position, directionToHim * valRepuls01, Color.black);
                //Debug.Log("ValRepuls :" + valRepuls01);
                direction = Vector2.Lerp(direction, -directionToHim, valRepuls01);
            }
            else
            {
                direction = Vector2.Lerp(direction, directionToHim * infl.attractivenessForYou, valAttract01 * maximalInfluence);
            }
        }
        return direction;
    }

    public bool GetKey(List<KeyCode> codes)
    {
        foreach (KeyCode code in codes)
        {
            if (Input.GetKey(code))
                return true;
        }
        return false;
    }



    public void Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
