using UnityEngine;

namespace TestVariants.Behaviours;

public class TestFolderShopThing : MonoBehaviour
{
    public void OnItemCollected(Pickup pickup, int player)
    {
        TestPlugin.Instance.Logger.LogMessage("Test Folder collected");
    }

    public void OnItemDenied(Pickup pickup, int player)
    {
        StoreRoomFunction store = FindObjectOfType<StoreRoomFunction>();

        PropagatedAudioManagerAnimator propagatedAudioManagerAnimator = store.johnnyAudioManager;

        if (!Singleton<CoreGameManager>.Instance.johnnyHelped && pickup.price - Singleton<CoreGameManager>.Instance.GetPoints(player) <= 100)
        {
            TestPlugin.Instance.Logger.LogMessage("Test Folder bought (Johnny assisted)");
            Singleton<CoreGameManager>.Instance.johnnyHelped = true;
            Singleton<CoreGameManager>.Instance.AddPoints(-Singleton<CoreGameManager>.Instance.GetPoints(player), player, true);

            Transform transform = pickup.transform;
            transform.position += Vector3.right * 9999f;

            pickup.Collect(player);

            propagatedAudioManagerAnimator.FlushQueue(true);
            propagatedAudioManagerAnimator.QueueAudio(store.audHelp);

            store.itemPurchased = true;
            store.playerLeft = false;

            AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.clip = TestPlugin.Instance.assetMan.Get<SoundObject>("Lucky").soundClip;
            audioSource.Play();

            gameObject.AddComponent<Rigidbody>();
            return;
        }

        TestPlugin.Instance.Logger.LogMessage("Test Folder denied (not enough YTPs)");

        if (!propagatedAudioManagerAnimator.QueuedUp) propagatedAudioManagerAnimator.QueueRandomAudio(store.audUnafforable);
    }

    public void OnItemPurchased(Pickup pickup, int player)
    {
        TestPlugin.Instance.Logger.LogMessage("Test Folder bought");

        StoreRoomFunction store = FindObjectOfType<StoreRoomFunction>();

        PropagatedAudioManagerAnimator propagatedAudioManagerAnimator = store.johnnyAudioManager;

        Transform transform = pickup.transform;
        transform.position += Vector3.right * 9999f;

        if (!propagatedAudioManagerAnimator.QueuedUp && pickup.item.itemType != global::Items.Map) propagatedAudioManagerAnimator.QueueRandomAudio(store.audBuy);

        store.itemPurchased = true;
        store.playerLeft = false;

        AudioSource audioSource = new GameObject().AddComponent<AudioSource>();
        audioSource.clip = TestPlugin.Instance.assetMan.Get<SoundObject>("Lucky").soundClip;
        audioSource.Play();

        gameObject.AddComponent<Rigidbody>();
    }
}
