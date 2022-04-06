using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Classes;
using UnityEngine.SceneManagement;
using System;

public class LobbyUI : MonoBehaviour
{
    public Player user;
    //1 rome 0 world;
    public Sprite[] maps;
    public string[] mapNames;
    public GameObject mapImg;
    public LobbyComm lobbyCommManager;
    public Sprite[] avatars;

    public GameObject loaderPrefab, loader,blackLoader;

    public delegate void Delegate();
    public Delegate trueDelegate, falseDelegate;

    private int selMap = 0;
    private string gameType;
    private bool isHost;
    private string code;

    LobbyControl lobby;
    public void Awake()
    {
        lobbyCommManager = new LobbyComm(this);
        lobbyCommManager.TOKEN = PlayerPrefs.GetString("token");
        string username = PlayerPrefs.GetString("username");
        gameType = PlayerPrefs.GetString("gameType");
        int host = PlayerPrefs.GetInt("isHost");
        string prefColor = PlayerPrefs.GetString("SettingsPrefColor");
        int elo;
        string e = PlayerPrefs.GetString("elo");
        if (e == "???")
            elo = 0;
        else
            elo = Convert.ToInt32(PlayerPrefs.GetString("elo"));
        int avatar = PlayerPrefs.GetInt("SettingAvatar");
        user = new Player(username,prefColor,elo,avatar);
        if (host == 0)
            isHost = false;
        else
            isHost = true;

        trueDelegate = BackToMenu;

        if (gameType == "Friendly" && !isHost) 
            ShowCodePrompt();
        else
        {
            lobbyCommManager.RequestLobby(user, gameType, isHost);
            if (isHost)
                StartLoadBlack("Creating lobby...", trueDelegate);
            else
                StartLoadBlack("Searching for lobby...", trueDelegate);
        }
        Camera.main.transform.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SettingsSound") / 100;
        Camera.main.transform.GetComponent<AudioSource>().Play();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            trueDelegate = BackToMenu;
            falseDelegate = DoNothing;
            ShowDialog(trueDelegate, "Yes", falseDelegate, "No", "Are you sure you want to return to menu?");
        }
    }
    public void DoNothing()
    {
        return;
    }
    public void NextMap()
    {
        selMap = (selMap + 1) % maps.Length;
        UpdateMap();
    }
    public void PreviousMap()
    {
        selMap = (selMap - 1);
        if (selMap == -1)
            selMap = maps.Length - 1;
        UpdateMap();
    }
    private void UpdateMap()
    {
        mapImg.GetComponent<Image>().sprite = maps[selMap];
        transform.Find("Panel").transform.Find("MapTxt").GetComponent<Text>().text = mapNames[selMap];
        UpdateLobby();
    }
    public void GetLobby(LobbyControl l)
    {
        EndLoad();
        GameObject panel = transform.Find("Panel").gameObject;
        lobby = l;
        for (int i = 0; i < lobby.players.Count; i++)
        {
            GameObject userObj = panel.transform.Find("User" + i.ToString()).gameObject;
            userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[l.players[i].avatar];
            userObj.transform.Find("Name").GetComponent<Text>().text = l.players[i].username;
            if (l.players[i].elo != 0) 
                userObj.transform.Find("Elo").GetComponent<Text>().text = "Elo: " + l.players[i].elo;
            else
                userObj.transform.Find("Elo").GetComponent<Text>().text = "Elo: ???";
            Color boja;
            ColorUtility.TryParseHtmlString(l.players[i].color, out boja);
            userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
        }
        for (int i = lobby.players.Count; i < 6; i++)
        {
            GameObject userObj = panel.transform.Find("User" + i.ToString()).gameObject;
            userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[0];
            Color boja;
            ColorUtility.TryParseHtmlString("#f0f0f0", out boja);
            userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
            userObj.transform.Find("Name").GetComponent<Text>().text = "Free spot";
            userObj.transform.Find("Elo").GetComponent<Text>().text = "";
        }

        panel.transform.Find("GameTypeTxt").GetComponent<Text>().text = "Game type: " + l.gameType;
        if (l.gameType == "Friendly")
            panel.transform.Find("CodeTxt").GetComponent<Text>().text = "Code: " + l.joinCode;
        else
            panel.transform.Find("CodeTxt").GetComponent<Text>().text = "";

        panel.transform.Find("MapTxt").GetComponent<Text>().text = l.mapName;
        if (l.mapName == "World")
            panel.transform.Find("MapImg").GetComponent<Image>().sprite = maps[0];
        else
            panel.transform.Find("MapImg").GetComponent<Image>().sprite = maps[1];

        if (l.turnDuration == 60)
            panel.transform.Find("Dropdown").GetComponent<Dropdown>().value = 0;
        if (l.turnDuration == 90)
            panel.transform.Find("Dropdown").GetComponent<Dropdown>().value = 1;
        if (l.turnDuration == 120)
            panel.transform.Find("Dropdown").GetComponent<Dropdown>().value = 2;


        if (l.host != user.username)
        {
            panel.transform.Find("Dropdown").GetComponent<Dropdown>().interactable = false;
            panel.transform.Find("StartBtn").GetComponent<Button>().interactable = false;
            panel.transform.Find("NextMapBtn").GetComponent<Button>().interactable = false;
            panel.transform.Find("PrevMapBtn").GetComponent<Button>().interactable = false;
        }

    }
    public void StartGame()
    {
        if(lobby.players.Count>1)
        {
            transform.Find("Panel").Find("StartBtn").GetComponent<Button>().interactable = false;
            lobbyCommManager.StartGame(lobby);
        }
        else
        {
            Notify(false, "Not enough players!");
        }
        
    }
    public void GameStarted(string map)
    {
        if (map == "Rome")
        {
            SceneManager.LoadScene("RomeGame");
        }
        if (map == "World")
        {
            SceneManager.LoadScene("WorldGame");
        }
    }
    public void StartLoad(string mess)
    {
        GameObject loadContainer = transform.Find("Panel").Find("Loading").gameObject;
        loadContainer.SetActive(true);
        loader = Instantiate(loaderPrefab, loadContainer.transform);
        loader.transform.Find("LoadingTxt").GetComponent<Text>().text = mess;
    }
    public void StartLoadBlack(string mess, Delegate exit)
    {
        GameObject loadContainer = transform.Find("Panel").Find("Loading").gameObject;
        loadContainer.SetActive(true);
        loader = Instantiate(blackLoader, loadContainer.transform);
        loader.transform.Find("LoadingTxt").GetComponent<Text>().text = mess;
        loader.transform.Find("LoadingPanel").Find("ExitBtn").GetComponent<Button>().onClick.AddListener(() => { exit(); });
    }
    public void EndLoad()
    {
        Destroy(loader);
        GameObject loadContainer = transform.Find("Panel").Find("Loading").gameObject;
        loadContainer.SetActive(false);
    }
    public void ShowDialog(Delegate fTrue, string opTrue, Delegate fFalse, string opFalse, string msg)
    {
        transform.Find("Start").Find("Dialog").gameObject.SetActive(true);
        GameObject dialog = transform.Find("Start").Find("Dialog").Find("DialogPanel").gameObject;
        dialog.transform.Find("True").Find("Text").GetComponent<Text>().text = opTrue;
        dialog.transform.Find("False").Find("Text").GetComponent<Text>().text = opFalse;
        dialog.transform.Find("Msg").GetComponent<Text>().text = msg;
        dialog.transform.Find("True").GetComponent<Button>().onClick.RemoveAllListeners();
        dialog.transform.Find("False").GetComponent<Button>().onClick.RemoveAllListeners();
        dialog.transform.Find("True").GetComponent<Button>().onClick.AddListener(() => { fTrue(); });
        dialog.transform.Find("False").GetComponent<Button>().onClick.AddListener(() => { fFalse(); });
    }
    public void Notify(bool isGood, string mess)
    {
        GameObject notification = transform.Find("Panel").Find("NotifyContainer").Find("Notification").gameObject;
        notification.GetComponent<Animator>().Play("NotificationAnimation");
        if (isGood)
        {
            notification.transform.Find("Tick").gameObject.SetActive(true);
            notification.transform.Find("Cross").gameObject.SetActive(false);
        }
        else
        {
            notification.transform.Find("Cross").gameObject.SetActive(true);
            notification.transform.Find("Tick").gameObject.SetActive(false);
        }
        notification.transform.Find("Mess").GetComponent<Text>().text = mess;
        notification.GetComponent<Animator>().Play("NotificationAnimation");

    }
    public void BackToMenu()
    {
        lobbyCommManager.LeaveLobby(user);
        SceneManager.LoadScene("Menu");
    }
    public void Reatempt(int sec)
    {
        StartCoroutine(ReatemptRequest(sec));
    }
    public void ShowCodePrompt()
    {
        transform.Find("Panel").Find("CodePrompt").Find("PromptPanel").Find("CodeInput")
            .GetComponent<InputField>().characterLimit = 4;
        transform.Find("Panel").Find("CodePrompt").gameObject.SetActive(true);
    }
    public void JoinFriendly(InputField code)
    {
        transform.Find("Panel").Find("CodePrompt").gameObject.SetActive(false);
        StartLoadBlack("Searching for game...", trueDelegate);
        lobbyCommManager.RequestLobbyByCode(user,code.text.ToUpper());
    }
    public void WrongCode(string msg)
    {
        Notify(false, msg);
        EndLoad();
        ShowCodePrompt();
    }
    public void HostLeft()
    {
        transform.Find("Panel").Find("HostLeft").gameObject.SetActive(true);
    }
    public void UpdateLobby()
    {
        if(isHost)
        {
            int turnDuration = transform.Find("Panel").Find("Dropdown").GetComponent<Dropdown>().value * 30 + 60;
            LobbyControl newLobby = new LobbyControl(lobby.lobbyId, lobby.host, lobby.players, mapNames[selMap], turnDuration, lobby.gameType, lobby.joinCode);
            lobbyCommManager.UpdateLobby(newLobby);
        }   
    }
    IEnumerator ReatemptRequest(int sec)
    {
        yield return new WaitForSeconds(sec);
        if (gameType == "Friendly")
            lobbyCommManager.RequestLobbyByCode(user,code);
        else
            lobbyCommManager.RequestLobby(user, gameType, isHost);
    }

}
