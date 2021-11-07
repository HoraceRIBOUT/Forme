using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Influenceur : PNJ
{
    public float attractivenessForPNJ = 1;
    public float attractivenessForYou = 1;

    public float changeSpeed = 15f;
    public float howQuickDidItBuildUp = 3f;

    public Vector2 radiusMinMax = new Vector2(0, 2.2f);

    public SpriteRenderer _sR_Zone;
    public List<SpriteRenderer> _sR_Onde;
    public Transform _tr;

    //Pour l'instant : s'en foutent de si y a des PNJ

        public Animator _ani;

    public void Start()
    {
        base.Start();

        AnimatorUpdate();
    }

    public void Update()
    {
#if UNITY_EDITOR
        _tr.localScale = Vector3.one * radiusMinMax.y / 2.2f;
        if (Player._pl == null)
            Player._pl = FindObjectOfType<Player>();
        _sR.color = Player._pl.gradientSpeed.Evaluate(changeSpeed / Player._pl.maxSpeed);
        Color col = _sR.color;
        col.a = _sR_Zone.color.a;
        _sR_Zone.color = col;

        if (!Application.isPlaying)
            return;
#endif


        base.Update();

        OndeUpdate();
    }

    public void AnimatorUpdate()
    {
        _ani.speed = attractivenessForYou * Mathf.Sign(attractivenessForYou);
        if (attractivenessForYou > 0)
            _ani.SetBool("Rev", true);
    }

    public float amplitue = 1f;
    public void OndeUpdate()
    {
        float dist = (Player._pl.transform.position - this.transform.position).sqrMagnitude;
        float val01 = (dist - (radiusMinMax.y * radiusMinMax.y)) / (radiusMinMax.y * radiusMinMax.y * amplitue);
        val01 = 1 - Mathf.Clamp01(val01);

        Color col = Color.Lerp(Color.white, Color.black, val01 * 0.3f);
        col.a = _sR_Onde[0].color.a;
        foreach (SpriteRenderer ondeR in _sR_Onde)
        {
            ondeR.color = col;
        }
    }


}
