using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_TestyDar : Item
{
    private TimeScaleModifier tsm;

    private PlayerManager pme;

    private bool used = false;

    private float timer = 0f;

    public override bool Use(PlayerManager pm)
    {
        pme = pm;
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("TestyDarEat"));
        tsm = new TimeScaleModifier(0f, 0f, 1f);
        pme.ec.AddTimeScale(tsm);
        used = true;
        timer = 1f;
        return true;
    }

    public void Update()
    {
        if (timer >= 0f & used)
        {
            timer -= Time.deltaTime / 18f;
            pme.plm.stamina = timer * pme.plm.staminaMax;
            if (timer <= 0f)
            {
                pme.ec.RemoveTimeScale(tsm);
                used = false;
                Destroy((Object)(object)gameObject);
            }
        }
    }
}
