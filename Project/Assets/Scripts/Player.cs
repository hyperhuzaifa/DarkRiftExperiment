using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Server;
using DarkRift.Client;
using DarkRift.Client.Unity;
using TMPro;

public class Player : MonoBehaviour
{
    const byte MOVEMENT_TAG = 1;

    [SerializeField]
    [Tooltip("The distance we can move before we send a position update.")]
    float moveDistance = 0.05f;

    public UnityClient Client { get; set; }
    [SerializeField]
    public string pname;

    bool nameSet = false;

    Vector3 lastPosition;


    void Update()
    {
        //Set name locally
        if(pname != "" && nameSet == false)
        {
            Debug.Log("setname");
            this.GetComponentInChildren<TextMeshPro>().text = pname;
            nameSet = true;

            //Update server with new name
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(pname);

                using (Message message = Message.Create(Tags.ClientNameRequestTag, writer))
                    Client.SendMessage(message, SendMode.Reliable);
            }

        }
        //  Camera follow
        if (nameSet)
        {
            if (Camera.main.GetComponent<CameraFollow>().Target == null)
                Camera.main.GetComponent<CameraFollow>().Target = this.gameObject.transform;
        }


        if (Vector3.Distance(lastPosition, transform.position) > moveDistance)
        {
            /* Send position to server here */
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);
                using (Message message = Message.Create(Tags.MovePlayerTag, writer))
                    Client.SendMessage(message, SendMode.Unreliable);
            }

            lastPosition = transform.position;
        }
    }
}