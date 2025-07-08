using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_GoodSoup : Item
{
    private PlayerManager pme;

    private bool used = false;

    private float timer = 0f;

    public override bool Use(PlayerManager pm)
    {
        pme = pm;
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("GoodSoupEat"));
        used = true;
        timer = 10f;
        return true;
    }

    public void Update()
    {
        if (timer >= 0f & used)
        {
            timer -= Time.deltaTime;
            pme.plm.stamina += pme.plm.staminaMax * Time.deltaTime / 10f;
            if (timer <= 0f)
            {
                used = false;
                Destroy(gameObject);
            }
        }
    }
}
