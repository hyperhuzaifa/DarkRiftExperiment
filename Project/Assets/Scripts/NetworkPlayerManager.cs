using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Server;
using DarkRift.Client;
using DarkRift.Client.Unity;
using TMPro;

public class NetworkPlayerManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    [SerializeField]
    [Tooltip("Spawner reference.")]
    PlayerSpawner spawner;

    [SerializeField]
    [Tooltip("food spawner reference.")]
    FoodManager foodSpawner;

    public TextMeshProUGUI nameInput;
    public GameObject mainMenuPanel;

    Dictionary<ushort, AgarObject> networkPlayers = new Dictionary<ushort, AgarObject>();

    public void Add(ushort id, AgarObject player)
    {
        networkPlayers.Add(id, player);
    }

    void SendName()
    {
        Debug.Log("sending name...");
        using (DarkRiftWriter writer = DarkRiftWriter.Create())
        {
            writer.Write(nameInput.text);
            using (Message message = Message.Create(Tags.ClientNameRequestTag, writer))
                client.SendMessage(message, SendMode.Reliable);
        }
        Debug.Log("name sent sucessfully");
        mainMenuPanel.SetActive(false);
    }

    public void RequestName()
    {
        SendName();
    }

    public void MessageReceived(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage() as Message)
        {
            if(message.Tag == Tags.PlayerDeathTag)
            {
                Debug.Log("called death sucessfully");
                using (DarkRiftReader reader = message.GetReader())
                {
                    ushort id = reader.ReadUInt16();
                    Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

                    if (networkPlayers.ContainsKey(id))
                    {
                        networkPlayers[id].gameObject.transform.position = newPosition;
                        networkPlayers[id].SetRadius(1);
                    }
                }
            }
            if (message.Tag == Tags.ClientNameUpdateTag)
            {
                using (DarkRiftReader reader = message.GetReader())
                {
                    Debug.Log("called name refresh");
                    string n = reader.ReadString();
                    ushort ID = reader.ReadUInt16();

                    GameObject g = networkPlayers[ID].gameObject;
                    if (g.GetComponent<Player>())
                        g.GetComponent<Player>().pname = n;
                    else
                        g.GetComponentInChildren<TextMeshPro>().text = n;

                    g.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            if (message.Tag == Tags.SpawnPlayerTag)
            {
                Debug.Log("recieved player creation request");
                spawner.SpawnPlayer(sender, e);
            }
            if (message.Tag == Tags.MovePlayerTag)
            {
                using (DarkRiftReader reader = message.GetReader())
                {
                    ushort id = reader.ReadUInt16();
                    Vector3 newPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), 0);

                    if (networkPlayers.ContainsKey(id))
                        networkPlayers[id].SetMovePosition(newPosition);
                }
            }
            if (message.Tag == Tags.SetRadiusTag)
            {
                Debug.Log("setting radius");
                using (DarkRiftReader reader = message.GetReader())
                {
                    ushort id = reader.ReadUInt16();
                    double r = reader.ReadDouble();

                    networkPlayers[id].SetRadius((float)r);
                }
                if (message.Tag == Tags.DespawnPlayerTag)
                {
                    spawner.DespawnPlayer(sender, e);
                }
            }
        }
    }

    public void DestroyPlayer(ushort id)
    {
        AgarObject o = networkPlayers[id];

        Destroy(o.gameObject);

        networkPlayers.Remove(id);
    }
}