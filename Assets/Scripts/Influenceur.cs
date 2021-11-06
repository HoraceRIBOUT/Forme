using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influenceur : PNJ
{
    public float attractivenessForPNJ = 1;
    public float attractivenessForYou = 1;

    public float changeSpeed = 15f;
    public float howQuickDidItBuildUp = 3f;

    public Vector2 radiusMinMax = new Vector2(0, 2.2f);

    //Pour l'instant : s'en foutent de si y a des PNJ

        public Animator _ani;

    public void Start()
    {
        base.Start();

        AnimatorUpdate();
    }

    public void AnimatorUpdate()
    {
        _ani.speed = attractivenessForYou * Mathf.Sign(attractivenessForYou);
        if (attractivenessForYou > 0)
            _ani.SetBool("Rev", true);
    }


}
