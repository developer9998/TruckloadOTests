using UnityEngine;

namespace TestVariants.Behaviours.Items;

public class ITM_TestYTPEVIL : Item
{
    private Fog myFog = new();

    private EnvironmentController ec;

    public override bool Use(PlayerManager pm)
    {
        GameObject blindLoopObject = new("TestYTPEVIL Blind Loop");
        AudioSource audioSource = blindLoopObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = TestPlugin.Instance.assetMan.Get<SoundObject>("TestYTPEVILLoop").soundClip;
        audioSource.Play();
        Destroy(blindLoopObject, 60f);

        Fog fog = TestPlugin.FindResourceOfName<NPC>("LookAt", null).gameObject.GetComponent<LookAtGuy>().fog;
        myFog.strength = fog.strength;
        myFog.maxDist = fog.maxDist;
        myFog.startDist = fog.startDist;
        myFog.priority = fog.priority;
        myFog.color = new Color(0.48f, 0f, 0f);

        pm.ec.AddFog(myFog);
        ec = pm.ec;
        Invoke(nameof(NoFog), 60f);
        Singleton<CoreGameManager>.Instance.AddPoints(600, pm.playerNumber, true);

        return true;
    }

    private void NoFog()
    {
        ec.RemoveFog(myFog);
        Destroy(gameObject);
    }
}
