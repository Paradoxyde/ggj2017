using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseComponent : WaveComponent
{
    public Texture grayMainTex;
    public Texture grayDetail;
    public Texture blueMainTex;
    public Texture blueDetail;
    public Texture redMainTex;
    public Texture redDetail;

    public Renderer mainRenderer1;
    public Renderer mainRenderer2;
    public Renderer detailRenderer;

    void Awake()
    {
        Helpers.MakeGrey(gameObject);
    }

    void Update()
    {

    }

    public override void OnPhaseChanged(Phase phase)
    {
        switch(phase.phase_type)
        {
            case PhaseType.Blue:
                //Helpers.MakeBlue(gameObject);
                SetTextures(blueMainTex, blueDetail);
                break;
            case PhaseType.Red:
                //Helpers.MakeRed(gameObject);
                SetTextures(redMainTex, redDetail);
                break;
            case PhaseType.Neutral:
                //Helpers.MakeGrey(gameObject);
                SetTextures(grayMainTex, grayDetail);
                break;
        }
    }

    private void SetTextures(Texture mainTex, Texture detailTex)
    {
        mainRenderer1.material.SetTexture("_MainTex", mainTex);
        mainRenderer2.material.SetTexture("_MainTex", mainTex);
        detailRenderer.material.SetTexture("_MainTex", detailTex);
    }
}
