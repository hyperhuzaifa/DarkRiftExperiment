using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Server;
using DarkRift.Client.Unity;
using DarkRift.Client;
using System;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    const byte SPAWN_TAG = 0;

    [SerializeField]
    [Tooltip("The network player manager.")]
    NetworkPlayerManager networkPlayerManager;

    [SerializeField]
    [Tooltip("The DarkRift client to communicate on.")]
    UnityClient client;

    [SerializeField]
    [Tooltip("The controllable player prefab.")]
    GameObject controllablePrefab;

    [SerializeField]
    [Tooltip("The network controllable player prefab.")]
    GameObject networkPrefab;

    void Awake()
    {
        if (client == null)
        {
            Debug.LogError("Client unassigned in PlayerSpawner.");
            Application.Quit();
        }

        if (controllablePrefab == null)
        {
            Debug.LogError("Controllable Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

        if (networkPrefab == null)
        {
            Debug.LogError("Network Prefab unassigned in PlayerSpawner.");
            Application.Quit();
        }

        client.MessageReceived += networkPlayerManager.MessageReceived;
    }

    public void DespawnPlayer(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
            networkPlayerManager.DestroyPlayer(reader.ReadUInt16());
    }

    public void SpawnPlayer(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        Debug.Log("calling this ");

        using (Message message = e.GetMessage())
        using (DarkRiftReader reader = message.GetReader())
        {
            if (message.Tag == Tags.SpawnPlayerTag)
            {
                //if (reader.Length % 22 != 0)
                //{
                //  Debug.LogWarning("Received malformed spawn packet.");
                //   return;
                //}

                while (reader.Position < reader.Length)
                {
                    ushort id = reader.ReadUInt16();
                    string name = reader.ReadString();
                    Vector3 position = new Vector3(reader.ReadSingle(), reader.ReadSingle());
                    float radius = reader.ReadSingle();
                    Color32 color = new Color32(
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        255
                    );
                    Debug.Log(id);
                    GameObject obj;
                    if (id == client.ID)
                    {
                        obj = Instantiate(controllablePrefab, position, Quaternion.identity) as GameObject;

                        Player player = obj.GetComponent<Player>();
                        player.Client = client;
                        player.name = name;
                        if (obj.GetComponentInChildren<TextMeshPro>())
                        {
                            Debug.Log("found it");
                            obj.GetComponentInChildren<TextMeshPro>().text = name;
                        }
                        else
                            Debug.Log("didnt find it");
                    }
                    else
                    {
                        obj = Instantiate(networkPrefab, position, Quaternion.identity) as GameObject;
                        obj.GetComponentInChildren<TextMeshPro>().text = name;
                    }


                    AgarObject agarObj = obj.GetComponent<AgarObject>();

                    agarObj.SetRadius(radius);
                    agarObj.SetColor(color);

                    networkPlayerManager.Add(id, agarObj);
                }
            }
        }
    }
}

