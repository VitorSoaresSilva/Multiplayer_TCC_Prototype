using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;
    // Lobbies List Variables
    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject lobbiesButton, hostButton;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void DestroyLobbies()
    {
        foreach (GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab, lobbyListContent.transform, true);
                LobbyDataEntry dataEntry = createdItem.GetComponent<LobbyDataEntry>();
                dataEntry.lobbyId = (CSteamID)lobbyIDs[i].m_SteamID;
                dataEntry.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID,"name");
                dataEntry.SetLobbyData();
                createdItem.transform.localScale = Vector3.one;
                
                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void GetListOfLobbies()
    {
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);
        lobbiesMenu.SetActive(true);
        
        SteamLobby.Instance.GetLobbiesList();
    }

    public void CloseListOfLobbies()
    {
        lobbiesButton.SetActive(true);
        hostButton.SetActive(true);
        lobbiesMenu.SetActive(false);
        DestroyLobbies();
    }
}
