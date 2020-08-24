using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Server;
using DarkRift.Client;
using DarkRift.Client.Unity;

public class FoodManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The network player manager.")]
    NetworkPlayerManager networkPlayerManager;

    [SerializeField]
    [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    Dictionary<ushort, AgarObject> networkFood = new Dictionary<ushort, AgarObject>();

    [SerializeField]
    [Tooltip("Food item prefab.")]
    GameObject foodPrefab;

    private void Awake()
    {
       client.MessageReceived += MessageReceived;
    }

    void MessageReceived(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
     //   Debug.Log("message received");
        using (Message message = e.GetMessage())
        {
            if (message.Tag == Tags.FoodSpawnTag)
            {
                SpawnFood(sender, e);
            }
            if (message.Tag == Tags.MoveFoodTag)
            {
                Debug.Log("food moving");
                RespawnFood(sender, e);

            }
        }
    }

    void RespawnFood(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            if (reader.Length % 10 != 0)
            {
                Debug.LogWarning("Received malformed move packet.");
                return;
            }

            ushort id = reader.ReadUInt16();
            Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle());

            if (networkFood.ContainsKey(id))
                networkFood[id].transform.position = position;
        }
    }

    public void SpawnFood(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        Debug.Log("calling thisa ");

        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            if (message.Tag == Tags.FoodSpawnTag)
            {
                if (reader.Length % 13 != 0)
                {
                    Debug.LogWarning("Received malformed spawn packet.");
                    return;
                }

                while (reader.Position < reader.Length)
                {
                    ushort id = reader.ReadUInt16();
                    Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle());
                    Color32 color = new Color32(
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        255
                    );

                    GameObject obj;
                    obj = Instantiate(foodPrefab, position, Quaternion.identity) as GameObject;

                   // Debug.Log("Spawning food with ID = " + id + ".");

                    AgarObject agarObj = obj.GetComponent<AgarObject>();

                    agarObj.SetRadius(1);
                    agarObj.SetColor(color);

                    networkFood.Add(id, agarObj);
                }
            }
        }
    }
}
