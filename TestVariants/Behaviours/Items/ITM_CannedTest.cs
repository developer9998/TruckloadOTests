namespace TestVariants.Behaviours.Items;

public class ITM_CannedTest : Item
{
    public override bool Use(PlayerManager pm)
    {
        EnvironmentController ec = FindObjectOfType<EnvironmentController>();
        if (ec.Npcs == null || ec.Npcs.Count == 0) return false;

        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("CannedTestDispense"));
        ec.SpawnNPC(TestPlugin.Instance.assetMan.Get<NPC>("Canny"), new IntVector2(0, 0));
        NPC canny = ec.Npcs[^1];
        canny.transform.position = pm.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).camCom.transform.forward * 5f;
        return true;
    }
}