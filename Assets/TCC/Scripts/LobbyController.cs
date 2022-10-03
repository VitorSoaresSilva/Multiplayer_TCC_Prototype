using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TextMeshProUGUI LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    
    //other data
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new();
    public PlayerObjectController LocalplayerController;
    
    //Ready
    public Button StartGameButton;
    public TextMeshProUGUI ReadyButtonText;
    
    //Manager
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        ReadyButtonText.text = LocalplayerController.Ready ? "Unready" : "Ready";
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;
        foreach (PlayerObjectController playerObjectController in Manager.GamePlayers)
        {
            if (playerObjectController.Ready)
            {
                AllReady = true;
            }
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (LocalplayerController.PlayerIdNumber == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
        
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) { CreateHostPlayerItem(); }
        if (PlayerListItems.Count < Manager.GamePlayers.Count){ CreateClientPlayerItem();}
        if (PlayerListItems.Count > Manager.GamePlayers.Count) { RemovePlayerItem();}
        if (PlayerListItems.Count == Manager.GamePlayers.Count) { UpdatePlayerItem();}
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();
            
            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;
            
            PlayerListItems.Add(NewPlayerItemScript);
        }

        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();
            
                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;
            
                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListScript in PlayerListItems)
            {
                if (PlayerListScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListScript.PlayerName = player.PlayerName;
                    PlayerListScript.Ready = player.Ready;
                    PlayerListScript.SetPlayerValues();
                    if (player == LocalplayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemsToRemove = new();

        foreach (PlayerListItem playerListItem in PlayerListItems)
        {
            if (Manager.GamePlayers.All(b => b.ConnectionID != playerListItem.ConnectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }

        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                if (playerListItemToRemove != null)
                {
                    GameObject ObjectToRemove = playerListItemToRemove.gameObject;
                    PlayerListItems.Remove(playerListItemToRemove);
                    Destroy(ObjectToRemove);
                    ObjectToRemove = null;
                }
            }
        }
    }

    public void StartGame(string SceneName)
    {
        LocalplayerController.CanStartGame(SceneName);
    }
}