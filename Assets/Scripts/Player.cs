using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Player : MonoBehaviour
{
    [Header("instance")]
    public static Player _pl;
    [Header("Input")]
    public List<KeyCode> upCode;
    public List<KeyCode> downCode;
    public List<KeyCode> leftCode;
    public List<KeyCode> rightCode;

    [Header("Movement")]
    public float speedNormal = 1;
    public float currentSpeedMax = 1;
    private Vector2 lastSpeed = Vector2.zero;
    [Range(0,1)]
    public float friction = 0.9f;

    public Transform camFocus;
    public Transform leftFoot;
    public Transform rightFoot;
    public float ecart = 0.5f;
    public float footEcart = 0.5f;
    public float camFocusDelay = 0.5f;
    private Vector2 lastDirection = Vector2.zero;
    private Vector2 targetDirection = Vector2.zero;

    [Header("Influenceur")]
    public List<Influenceur> influenceurList = new List<Influenceur>();
    public float maximalInfluence = 0.9f;
    public float howLongToLooseAllBuilt = 20f;

    [Header("Mur")]
    public List<Mur> murList = new List<Mur>();

    [Header("Step")]
    public GameObject step;
    public float distanceBetweenStep = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _pl = this;

        influenceurList = new List<Influenceur>();
        foreach (Influenceur influenceur in FindObjectsOfType<Influenceur>())
        {
            influenceurList.Add(influenceur);
        }

        currentSpeedMax = speedNormal;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        MovementManagement();

        VisualManagement();


#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
            _Debug();
    }

    [Header("Visual")]
    public SpriteRenderer _sR;
    public SpriteRenderer _sR_Zone;
    public SpriteRenderer _sR_GoHome;
    public Gradient gradientSpeed;
    public float maxSpeed = 30f;
    public AnimationCurve roundWay;

    public void VisualManagement()
    {
        _sR.color = gradientSpeed.Evaluate(currentSpeedMax / maxSpeed);
        Color col = _sR.color; ;
        col.a = _sR_Zone.color.a;
        _sR_Zone.color = col;

        _sR_GoHome.transform.localScale = Vector3.one * goHome / 3f;


        //_sR.transform.localEulerAngles = Vector3.forward * roundWay.Evaluate(lastDirection.y) * 180f;// * Mathf.Sign(lastDirection.x);


        //_sR.transform.rotation = Quaternion.Euler(0, 0, );
    }

    public float goHome = 0;
    public Vector3 lastStepPos = Vector3.zero;
    private bool leftLeftLeft = false;
    public void MovementManagement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            goHome += Time.deltaTime;
            if (goHome > 3f)
            {
                goHome = 0;
                this.transform.position = Vector3.zero;
            }
        }
        else
            goHome = 0;


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
        direction = InfluenceurMur(direction);


        Vector2 finalSpeed = direction.normalized * currentSpeedMax;

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

        leftFoot.position = this.transform.position + Vector3.Cross(lastDirection, Vector3.forward) * footEcart;
        rightFoot.position = this.transform.position + -Vector3.Cross(lastDirection, Vector3.forward) * footEcart;

        Vector3 dirdir = (lastStepPos - (leftLeftLeft ? leftFoot.position : rightFoot.position));
        if (dirdir.magnitude > distanceBetweenStep)
        {
            lastStepPos = leftFoot.position;
            Instantiate(step, leftLeftLeft ? leftFoot.position : rightFoot.position, Quaternion.identity, null);
            leftLeftLeft = !leftLeftLeft;
        }
    }


    public Vector2 InfluenceurZone(Vector2 direction)
    {
        Debug.DrawRay(this.transform.position, direction, Color.red);
        bool atLeastOne = false;
        foreach (Influenceur infl in influenceurList)
        {
            if (infl.attractivenessForYou == 0)
                continue;
            Vector2 directionToHim = infl.transform.position - this.transform.position;
            float distDist = directionToHim.sqrMagnitude;
            if (infl.radiusMinMax.y * infl.radiusMinMax.y < distDist)
                continue;
            atLeastOne = true;
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
                Debug.DrawRay(this.transform.position, -directionToHim, Color.black);
            }
            else
            {
                direction = Vector2.Lerp(direction, directionToHim * infl.attractivenessForYou, valAttract01 * maximalInfluence);
                Debug.DrawRay(this.transform.position, directionToHim * infl.attractivenessForYou, Color.green);
            }


            if (infl.changeSpeed != 0)
            {
                currentSpeedMax = Mathf.Lerp(currentSpeedMax, infl.changeSpeed, Time.deltaTime / infl.howQuickDidItBuildUp);

            }

        }
        Debug.DrawRay(this.transform.position, direction, Color.red+Color.blue);

        if (influenceurList.Count == 0 || !atLeastOne)
            currentSpeedMax = Mathf.Lerp(currentSpeedMax, speedNormal, Time.deltaTime / howLongToLooseAllBuilt);

        return direction;
    }

    public float murDist = 4;
    public Vector2 InfluenceurMur(Vector2 direction)
    {
        foreach(Mur mur in murList)
        {
            Vector3 murDirection = mur.transform.right;
            Vector3 towardWall = (this.transform.position - mur.transform.position);
            if (Vector3.Dot(mur.transform.right, towardWall) < 0)
            {
                murDirection = -mur.transform.right;
            }
            Debug.DrawRay(this.transform.position, murDirection, Color.red + Color.green);
            float value01 = towardWall.magnitude / murDist;
            value01 = 1 - Mathf.Clamp01(value01);
            direction = Vector2.Lerp(direction, murDirection, value01);
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



    public void _Debug()
    {
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
