using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetWork_Manager : MonoBehaviourPunCallbacks
{

    public void ConnectServer() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        // PhotonNetwork.LocalPlayer.NickName;
    }
}
