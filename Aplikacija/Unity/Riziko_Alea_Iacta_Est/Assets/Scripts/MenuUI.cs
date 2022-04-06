using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Classes;
using Microsoft.AspNetCore.SignalR.Client;
using Assets.Classes.DataTransfer;
using System;
using UnityEngine.SceneManagement;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class MenuUI : MonoBehaviour
{
    public User user;
    public MenuComm commManager;
    public GameObject menuFrame, menuAvatar, usernameTxt, eloTxt;
    public GameObject userColor, userAvatar, volTog, volSlide, actionConfirm;
    public GameObject loaderPrefab, loader;
    public InputField loginUser, loginPass, regUser, regPass, regPassConf, regEmail;

    public Sprite[] avatars;
    private string[] colors = { "#cc0000", "#33cc33", "#0066ff", "#ffff00", "#cc00cc", "#ffffff" };
    private int selectedColor = 0, selectedAvatar = 0;
    private string mapPom;

    public delegate void Delegate();
    public Delegate trueDelegate, falseDelegate;

    private void Awake()
    {
        Screen.fullScreen = true;
        loginPass.characterLimit = 64;
        loginUser.characterLimit = 18;
        regUser.characterLimit = 18;
        regPass.characterLimit = 64;
        regPassConf.characterLimit = 64;
        regEmail.characterLimit = 64;
        volSlide.GetComponent<Slider>().onValueChanged.AddListener(delegate { VolumeAdjust(); });
        volTog.GetComponent<Toggle>().onValueChanged.AddListener(delegate { Mute(); });
        commManager = new MenuComm(this);
        CheckAccount();

    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            trueDelegate = Application.Quit;
            falseDelegate = DoNothing;
            ShowDialog(trueDelegate, "Yes", falseDelegate, "No", "Are you sure you want to quit?");
        }
    }
    public void DoNothing()
    {
        return;
    }
    public void SignIn()
    {
        commManager.Login(loginUser.text, loginPass.text);
    }
    public void Register()
    {
        if (regPass.text == regPassConf.text)
            if (regPass.text.Length >= 6)
                if (!regUser.text.ToLower().Contains("guest"))
                    if (regUser.text.Length > 4 && regUser.text.Length < 18)
                    {
                        commManager.Register(regUser.text, regPass.text, regEmail.text);
                        PlayerPrefs.SetString("username", regUser.text);
                        PlayerPrefs.SetString("password", regPass.text);
                    }
                    else
                        Notify(false, "Pick a username that is between 4 and 18 characters in lenght!");
                else
                    Notify(false, "Username cannot contain \"GUEST\"");
            else
                Notify(false, "Password too short! It has to be at least 6 characters!");
        else
            Notify(false, "Passwords don't match!");

    }
    public void Quit()
    {
        trueDelegate = Application.Quit;
        falseDelegate = ClosePannels;
        ShowDialog(trueDelegate,"Yes",falseDelegate,"No","Are you sure you want to quit?");
    }
    public void SettingsUpdated()
    {
        Color boja;
        ColorUtility.TryParseHtmlString(user.mySettings.PrefColor, out boja);
        menuFrame.GetComponent<Image>().color = boja;
        for (int i = 0; i < colors.Length; i++)
        {
            if (user.mySettings.PrefColor == colors[i])
                selectedColor = i;
        }
        menuAvatar.GetComponent<Image>().sprite = avatars[user.mySettings.Avatar];
        selectedAvatar = user.mySettings.Avatar;
        usernameTxt.GetComponent<Text>().text = user.username;
    }
    public void SettingsAdjust()
    {
        Color boja;
        ColorUtility.TryParseHtmlString(user.mySettings.PrefColor, out boja);
        userColor.GetComponent<Image>().color = boja;
        userAvatar.GetComponent<Image>().sprite = avatars[user.mySettings.Avatar];
        if (user.mySettings.Sound == 0)
            volTog.GetComponent<Toggle>().isOn = false;
        else
            volTog.GetComponent<Toggle>().isOn = true;
        volSlide.GetComponent<Slider>().value = user.mySettings.Sound / 100;
        actionConfirm.GetComponent<Toggle>().isOn = user.mySettings.ActionConfirm;
    }
    public void ColorChange()
    {
        selectedColor = (selectedColor + 1) % colors.Length;
        Color boja;
        ColorUtility.TryParseHtmlString(colors[selectedColor], out boja);
        userColor.GetComponent<Image>().color = boja;
    }
    internal void ShowLeaderboard(List<LeaderboardTransfer> leaderboard)
    {
        
        GameObject list = transform.Find("Start").Find("Leaderboard").Find("LeaderboardPanel").Find("List").gameObject;
        for(int i=0;i<5;i++)
        {
            list.transform.Find((i + 1).ToString()).GetComponent<Text>().text = leaderboard[i].position.ToString() + ". \t\t" + leaderboard[i].username + " \t\t" + leaderboard[i].elo.ToString();
        }
        if (leaderboard.Count == 6)
        {
            list.transform.Find("6").GetComponent<Text>().text = "You: " + leaderboard[5].position.ToString() + ". \t\t" + leaderboard[5].username + " \t\t" + leaderboard[5].elo.ToString();
        }
        else
            list.transform.Find("6").GetComponent<Text>().text = "???";
        transform.Find("Start").Find("Leaderboard").gameObject.SetActive(true);
    }
    public void AvatarChange()
    {
        selectedAvatar = (selectedAvatar + 1) % avatars.Length;
        userAvatar.GetComponent<Image>().sprite = avatars[selectedAvatar];
    }
    public void SaveSettings()
    {
        float soundVal;
        if (volTog.GetComponent<Toggle>().isOn == false)
            soundVal = 0;
        else
            soundVal = volSlide.GetComponent<Slider>().value * 100;
        user.UpdateSettings(selectedAvatar, soundVal, colors[selectedColor], actionConfirm.GetComponent<Toggle>().isOn);
        SettingsUpdated();
        ClosePannels();
        commManager.UpdateSettings(user.mySettings, user.username,user.elo);
        
    }
    public void OpenMe(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void CloseMe(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void StartLoad(string mess)
    {
        GameObject loadContainer = transform.Find("Start").Find("Loading").gameObject;
        loadContainer.SetActive(true);
        loader = Instantiate(loaderPrefab, loadContainer.transform);
        loader.transform.Find("LoadingTxt").GetComponent<Text>().text = mess;
    }
    public void EndLoad()
    {
        Destroy(loader);
        GameObject loadContainer = transform.Find("Start").Find("Loading").gameObject;
        loadContainer.SetActive(false);
    }
    public void UserInfo(string username, Settings sett, string elo)
    {
        commManager.CheckReconnect(username);
        if (!username.Contains("Guest"))
        {
            user = new Standard(username, sett,this);
            SettingsUpdated();
            transform.Find("Start").Find("LogOutBtn").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Start").Find("LogOutBtn").gameObject.SetActive(false);
            user = new Guest(username, sett,this);
            SettingsUpdated();
        }
        Camera.main.transform.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SettingsSound") / 100;
        if (!Camera.main.transform.GetComponent<AudioSource>().isPlaying)
            Camera.main.transform.GetComponent<AudioSource>().Play();
        transform.Find("Start").Find("UserImg").Find("Elo").GetComponent<Text>().text = "ELO: " + elo;
        PlayerPrefs.SetString("elo", elo);
        user.elo = elo;
        ClosePannels();
    }
    public void ClosePannels()
    {
        transform.Find("Start").Find("SignIn").gameObject.SetActive(false);
        transform.Find("Start").Find("Dialog").gameObject.SetActive(false);
        transform.Find("Start").Find("SignUp").gameObject.SetActive(false);
        transform.Find("Start").Find("Settings").gameObject.SetActive(false);
    }
    public void Notify(bool isGood, string mess)
    {
        GameObject notification = transform.Find("Start").Find("NotifyContainer").Find("Notification").gameObject;
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
    public void CheckAccount()
    {
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            string username = PlayerPrefs.GetString("username");
            string password = PlayerPrefs.GetString("password");
            commManager.Login(username, password);
        }
        else if (PlayerPrefs.HasKey("guestUsername"))
        {
            string username = PlayerPrefs.GetString("guestUsername");
            commManager.Login(username, "GuestPass");
        }
        else
        {
            commManager.CreateGuest();
        }
    }
    public void GuestLogin()
    {
        if (PlayerPrefs.HasKey("guestUsername"))
        {
            string username = PlayerPrefs.GetString("guestUsername");
            commManager.Login(username, "GuestPass");
        }
        else
        {
            commManager.CreateGuest();
        }
    }
    public void LogOut()
    {
        if (PlayerPrefs.HasKey("guestUsername"))
            PlayerPrefs.SetString("username", PlayerPrefs.GetString("guestUsername"));
        else
            PlayerPrefs.DeleteKey("username");
        PlayerPrefs.SetString("password", null);
        commManager.LogOut();
        CheckAccount();
    }
    public void GetLeaderboard()
    {
        commManager.GetLeaderboard(user.username);
    }
    public void ShowDialog(Delegate fTrue, string opTrue ,Delegate fFalse, string opFalse,string msg)
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
    public void CasualClick()
    {

        trueDelegate = user.JoinCasual;
        falseDelegate = user.HostCasual;
        ShowDialog(trueDelegate, "Join", falseDelegate, "Host", "Join or Host a game?");
    }
    public void FriendlyClick()
    {

        trueDelegate = user.JoinFriendly;
        falseDelegate = user.HostFriendly;
        ShowDialog(trueDelegate, "Join", falseDelegate, "Host", "Join or Host a game?");
    }
    public void RankedClick()
    {

        trueDelegate = user.JoinRanked;
        falseDelegate = user.HostRanked;
        ShowDialog(trueDelegate, "Join", falseDelegate, "Host", "Join or Host a game?");
    }
    public void SaveUser(string username, string token, SettingsTransfer settings,string elo)
    {
        if(username.Contains("Guest"))
        {
            PlayerPrefs.SetString("guestUsername", username);
        }
        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("token", token);
        PlayerPrefs.SetString("elo", elo);
        PlayerPrefs.SetInt("SettingsId", settings.Id);
        PlayerPrefs.SetInt("SettingAvatar", settings.Avatar);
        PlayerPrefs.SetFloat("SettingsSound", settings.Sound);
        PlayerPrefs.SetString("SettingsPrefColor", settings.PrefColor);
        if (settings.ActionConfirm)
            PlayerPrefs.SetInt("SettingsActionConfirm", 1);
        else
            PlayerPrefs.SetInt("SettingsActionConfirm", 0);
        
        Camera.main.transform.GetComponent<AudioSource>().volume = settings.Sound / 100;
    }
    public void VolumeAdjust()
    {
        Camera.main.transform.GetComponent<AudioSource>().volume = volSlide.GetComponent<Slider>().value;
    }
    public void Mute()
    {
        if (!volTog.GetComponent<Toggle>().isOn)
            Camera.main.transform.GetComponent<AudioSource>().mute = true;
        else
            Camera.main.transform.GetComponent<AudioSource>().mute = false;
    }
    public void LoadGame()
    {
        if (mapPom == "Rome")
            SceneManager.LoadScene("RomeGame");
        else if (mapPom == "World")
            SceneManager.LoadScene("WorldGame");
    }
    public void Reconnect(string map)
    {
        mapPom = map;
        trueDelegate = LoadGame;
        falseDelegate = Application.Quit;
        transform.Find("Start").Find("Dialog").Find("DialogPanel").Find("ExitBtn").gameObject.SetActive(false);
        ShowDialog(trueDelegate, "Reconnect", falseDelegate, "Quit", "You have an ongoing game!");
    }
    public void Report()
    {
        transform.Find("Start").Find("Settings").gameObject.SetActive(false);
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("riziko.reports@gmail.com");
        mail.To.Add("riziko.degaa@gmail.com");
        mail.Subject = "Report form "+user.username+": " +transform.Find("Start").Find("Settings").Find("SettingsPanel").Find("Help").Find("SubjectInput").GetComponent<InputField>().text;
        mail.Body = transform.Find("Start").Find("Settings").Find("SettingsPanel").Find("Help").Find("ContentInput").GetComponent<InputField>().text;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("riziko.reports@gmail.com", "degaa999") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);

        Notify(true, "Report sent. Thank you for your feedback!");
    }
    public void OpenKeyboard()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }
    public void OpenKeyboardMail()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.EmailAddress);
    }
}
