using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class LobbyDataEntry : MonoBehaviour
{
    //Data
    public CSteamID lobbyId;
    public string lobbyName;
    public TextMeshProUGUI lobbyNameText;

    public void SetLobbyData()
    {
        lobbyNameText.text = lobbyName != string.Empty ? lobbyName : "Empty";
    }

    public void JoinLobby()
    {   
        SteamLobby.Instance.JoinLobby(lobbyId);
    }
}
