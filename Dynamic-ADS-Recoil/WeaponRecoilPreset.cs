using UnityEngine;
public class WeaponRecoilPreset : ScriptableObject {
    public float zRecoil = .6f;
    public float recoilSpeed = 4;
    public AnimationCurve recoilCurve;
    public AnimationCurve yCurve;
    public AnimationCurve xRotCurve;
    public float yRecoil = .2f;
    public float randomStepInterval = .1f;
    [Range(0, 20)]
    public float randXYAdjustmentSpeed = 15;
    public float randXYAmount = .4f;
    public float randAmountStep = .1f;
    public float randTimeMax = 1f;
    public float climbMod = .2f;
    public Vector3 recoilRotation = new Vector3(10,0,0);
}
