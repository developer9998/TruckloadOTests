namespace TestVariants.Behaviours.Items;

public class ITM_Potion : Item
{
    public override bool Use(PlayerManager pm)
    {
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Drink"));
        pm.ec.fogs.Clear();
        pm.ec.UpdateFog();
        pm.gameObject.AddComponent<Dizziness>().StartDizziness();
        return true;
    }
}
