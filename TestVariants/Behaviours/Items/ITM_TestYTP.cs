using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_TestYTP : Item
{
    private readonly Fog myFog = new();

    private EnvironmentController ec;

    public override bool Use(PlayerManager pm)
    {
        GameObject blindLoopObject = new("TestYTP Blind Loop");
        AudioSource audioSource = blindLoopObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = TestPlugin.Instance.assetMan.Get<SoundObject>("TestYTPLoop").soundClip;
        audioSource.Play();
        Destroy(blindLoopObject, 30f);

        Fog fog = TestPlugin.FindResourceOfName<NPC>("LookAt", null).gameObject.GetComponent<LookAtGuy>().fog;
        myFog.strength = fog.strength;
        myFog.maxDist = fog.maxDist;
        myFog.startDist = fog.startDist;
        myFog.priority = fog.priority;
        myFog.color = fog.color;

        pm.ec.AddFog(myFog);
        ec = pm.ec;
        Invoke(nameof(NoFog), 30f);
        Singleton<CoreGameManager>.Instance.AddPoints(300, pm.playerNumber, true);

        return true;
    }

    private void NoFog()
    {
        ec.RemoveFog(myFog);
        Destroy(gameObject);
    }
}
