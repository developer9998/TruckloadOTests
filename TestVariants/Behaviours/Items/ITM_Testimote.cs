using System.Collections.Generic;
using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_Testimote : Item
{
    public override bool Use(PlayerManager pm)
    {
        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(TestPlugin.Instance.assetMan.Get<SoundObject>("Testiport"));
        Invoke(nameof(Spawn), 0.511f);
        return true;
    }

    private void Spawn()
    {
        EnvironmentController environmentController = FindObjectOfType<EnvironmentController>();

        List<NPC> list =
        [
            // Base game characters
            TestPlugin.FindResourceOfName<NPC>("LookAt", null), // The Test
            TestPlugin.Instance.assetMan.Get<NPC>("Bluxam"),
            // Modded characters, aka. "Test Variants"
            TestPlugin.Instance.assetMan.Get<NPC>("Orlan"),
            TestPlugin.Instance.assetMan.Get<NPC>("Shelfman"),
            TestPlugin.Instance.assetMan.Get<NPC>("Plit"),
            TestPlugin.Instance.assetMan.Get<NPC>("Yestholomew"),
            TestPlugin.Instance.assetMan.Get<NPC>("Testholder"),
            TestPlugin.Instance.assetMan.Get<NPC>("Cren"),
            TestPlugin.Instance.assetMan.Get<NPC>("TheBestOne"),
            TestPlugin.Instance.assetMan.Get<NPC>("AJOPI9"),
            TestPlugin.Instance.assetMan.Get<NPC>("MeMest"),
            TestPlugin.Instance.assetMan.Get<NPC>("PalbyFan"),
            TestPlugin.Instance.assetMan.Get<NPC>("Canny"),
            TestPlugin.Instance.assetMan.Get<NPC>("Qrid"),
            TestPlugin.Instance.assetMan.Get<NPC>("Gummin"),
            TestPlugin.Instance.assetMan.Get<NPC>("ThroneTest"),
            TestPlugin.Instance.assetMan.Get<NPC>("Testerly")
        ];

        environmentController.SpawnNPC(list[Random.Range(0, list.Count)], new IntVector2(0, 0));
        NPC randomNPC = environmentController.Npcs[^1];
        randomNPC.transform.position = environmentController.Players[0].transform.position + Singleton<CoreGameManager>.Instance.GetCamera(environmentController.Players[0].playerNumber).camCom.transform.forward * 5f;
        randomNPC.gameObject.GetComponent<Entity>().AddForce(new Force(Singleton<CoreGameManager>.Instance.GetCamera(environmentController.Players[0].playerNumber).camCom.transform.forward, 70f, -50f));
        randomNPC.gameObject.AddComponent<TestThrow>().ec = environmentController;
    }
}
