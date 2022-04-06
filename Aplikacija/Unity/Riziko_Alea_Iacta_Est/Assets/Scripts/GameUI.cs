using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Classes;
using Assets.Classes.DataTransfer;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class GameUI : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject bonusCanvas;
    public GameObject uiCanvas;
    public GameObject explosion;
    public Sprite[] avatars;
    public Sprite emptySprite;
    public Sprite tank;
    public Sprite plane;
    public Sprite solider;
    public GameObject loaderPrefab, blackLoaderPrefab;
    public GameObject arrowPrefab;
    public GameObject arrowContainer;

    private string ongoingFrom, ongoingTo;
    private string myUsername;
    private GameObject loader;
    private GameComm commManager;
    public UserGameState gameState;
    private string firstClick, secondClick;
    public delegate void Delegate();
    public delegate void DelegateInt(int br);
    public DelegateInt IntDelegate;
    public Delegate trueDelegate, falseDelegate;
    private float currCountdownValue;
    private List<GameObject> arrows = new List<GameObject>();

    public void Awake()
    {
        StartLoadBlack("Joining game", null);
        commManager = new GameComm(this);
        myUsername = PlayerPrefs.GetString("username");
        Camera.main.transform.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SettingsSound") / 100;
        Camera.main.transform.GetComponent<AudioSource>().Play();
        explosion.transform.GetComponent<AudioSource>().volume= PlayerPrefs.GetFloat("SettingsSound") / 100;
    }
    private void Update()
    {
        if (currCountdownValue >= 0)
        {
            int showVal = (int)currCountdownValue;
            uiCanvas.transform.Find("Timer").Find("TimeTxt").GetComponent<Text>().text = showVal.ToString();
            currCountdownValue -= Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            trueDelegate = Application.Quit;
            falseDelegate = DoNothing;
            ShowDialog(trueDelegate, "Yes", falseDelegate, "No", "Are you sure you want to quit?");
        }
    }
    public void InitGameState(UserGameState ugs)
    {
        string[] colors = { "#cc0000", "#33cc33", "#0066ff", "#ffff00", "#cc00cc", "#ffffff" };
        int j = 0;
        gameState = ugs;
        //map
        foreach (ActiveContinent c in ugs.gameMap.continents)
        {
            Color continentColor;
            ColorUtility.TryParseHtmlString(colors[j], out continentColor);
            foreach (Territory t in c.territories)
            {
                bonusCanvas.transform.Find(c.continentName).Find(t.name).GetComponent<Image>().color = continentColor;

                GameObject ter = mapCanvas.transform.Find(c.continentName).Find(t.name).gameObject;
                Color boja;
                ColorUtility.TryParseHtmlString(t.currentHolder.color, out boja);
                ter.transform.GetComponent<Image>().color = boja;
                ter.transform.Find("Tanks").Find("Back").GetComponent<Image>().color = boja;
                ter.transform.Find("Tanks").Find("Number").GetComponent<Text>().text = t.tanks.ToString();
            }
            bonusCanvas.transform.Find(c.continentName).Find("Bonus").GetComponent<Text>().text = c.continentName+"\n+" + c.bonusTanks.ToString();
            j++;
            if (j == 6)
                j = 0;
        }
        //players
        for (int i = 0; i < ugs.players.Count; i++)
        {
            GameObject userObj = uiCanvas.transform.Find("Users").Find("User" + i.ToString()).gameObject;
            userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[ugs.players[i].avatar];
            userObj.transform.Find("Name").GetComponent<Text>().text = ugs.players[i].username;
            Color boja;
            ColorUtility.TryParseHtmlString(ugs.players[i].color, out boja);
            userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
        }
        for (int i = ugs.players.Count; i < 6; i++)
        {
            GameObject userObj = uiCanvas.transform.Find("Users").Find("User" + i.ToString()).gameObject;
            userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = emptySprite;
            Color boja;
            ColorUtility.TryParseHtmlString("#909090", out boja);
            boja.a = 0.75f;
            userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
            userObj.transform.Find("Name").GetComponent<Text>().text = "";
        }
        //onTurnUser
        GameObject onTurn = uiCanvas.transform.Find("Turn").gameObject;
        onTurn.transform.Find("User").Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[ugs.onTurn.avatar];
        Color turnBoja;
        ColorUtility.TryParseHtmlString(ugs.onTurn.color, out turnBoja);
        onTurn.transform.Find("User").Find("Frame").GetComponent<Image>().color = turnBoja;
        onTurn.transform.Find("PhaseTxt").GetComponent<Text>().text = ugs.phase;
        if (ugs.phase == "Draft")
        {
            onTurn.transform.Find("Draft").gameObject.SetActive(true);
            onTurn.transform.Find("Draft").Find("DraftNum").GetComponent<Text>().text = ugs.draftTanks.ToString();
        }
        else
        {
            onTurn.transform.Find("Draft").gameObject.SetActive(false);
        }

        //timer
        StartTimer(ugs.turnDuration);
        for (int i = 0; i < ugs.Cards.Count; i++)
        {
            Transform card = uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i.ToString());
            card.Find("Name").GetComponent<Text>().text = ugs.Cards[i].territoryName;
            if (FindTerritoryByName(ugs.Cards[i].territoryName) != null)
                card.Find("Territory").GetComponent<Image>().sprite = FindTerritoryByName(ugs.Cards[i].territoryName).GetComponent<Image>().sprite;
            if (ugs.Cards[i].type == "Tank")
                card.Find("Army").GetComponent<Image>().sprite = tank;
            if (ugs.Cards[i].type == "Plane")
                card.Find("Army").GetComponent<Image>().sprite = plane;
            if (ugs.Cards[i].type == "Solider")
                card.Find("Army").GetComponent<Image>().sprite = solider;
            if (ugs.onTurn.username == myUsername)
                card.GetComponent<Button>().interactable = true;
            else
            {
                card.GetComponent<Button>().interactable = true;
                ClearSelectedCards();
            }
        }
        for (int i = ugs.Cards.Count; i < 5; i++)
        {
            Transform card = uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i.ToString());
            card.Find("Name").GetComponent<Text>().text = "";
            card.Find("Territory").GetComponent<Image>().sprite = emptySprite;
            card.Find("Army").GetComponent<Image>().sprite = emptySprite;
            card.GetComponent<Button>().interactable=false;
        }
        //task
        uiCanvas.transform.Find("Task").Find("TaskPanel").Find("TaskTxt").GetComponent<Text>().text = ugs.winCond;

        if (gameState.onTurn.username != myUsername)
            NotMyTurn();
        else
            MyTurn();
        EndLoad();
    }
    public void AttackUnsuccesful()
    {
        uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("AttackBtn").GetComponent<Button>().interactable=false;
        InitPhase();
    }
    public void AttackSuccesful(string from, string to)
    {
        commManager.EndCurrentAttack();
        uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("AttackBtn").GetComponent<Button>().interactable = false;
        Territory fromTer = gameState.FindTerritory(from);
        uiCanvas.transform.Find("Attack").gameObject.SetActive(false);
        if (fromTer.tanks > 1)
        {
            uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Msg").GetComponent<Text>().text = "Transfer";
            Dropdown count = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>();
            count.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            for (int i = 0; i < fromTer.tanks; i++)
                options.Add(new Dropdown.OptionData((i).ToString()));
            count.AddOptions(options);
            uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").gameObject.SetActive(false);
            uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").gameObject.SetActive(true);
            Button okBtn = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("OkBtn").GetComponent<Button>();
            okBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(() => { uiCanvas.transform.Find("HowMany").gameObject.SetActive(false); });
            okBtn.onClick.AddListener(() => { commManager.Transfer(from, to, uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>().value); });
            uiCanvas.transform.Find("HowMany").gameObject.SetActive(true);
        }
        InitPhase();
    }
    public void UpdateGameState(UserGameState ugs)
    {
        #region Timer
        if (ugs.onTurn.username != gameState.onTurn.username)
            StartTimer(ugs.turnDuration);
        #endregion
        #region Map
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            foreach (Territory t in c.territories)
            {
                Territory newTer = ugs.FindTerritory(t.name);
                if (t.currentHolder != newTer.currentHolder)
                {
                    GameObject ter = mapCanvas.transform.Find(c.continentName).Find(t.name).gameObject;
                    Color boja;
                    ColorUtility.TryParseHtmlString(newTer.currentHolder.color, out boja);
                    ter.transform.GetComponent<Image>().color = boja;
                    ter.transform.Find("Tanks").Find("Back").GetComponent<Image>().color = boja;
                    if (ugs.onTurn.username == myUsername && newTer.currentHolder.username == myUsername && firstClick == null)
                        ter.GetComponent<Button>().interactable = true;
                }
                if (t.tanks != newTer.tanks)
                {
                    GameObject ter = mapCanvas.transform.Find(c.continentName).Find(t.name).gameObject;
                    ter.transform.Find("Tanks").Find("Number").GetComponent<Text>().text = newTer.tanks.ToString();
                    if (t.tanks < newTer.tanks)
                    {
                        ter.transform.Find("Tanks").GetComponent<Animator>().Play("TanksAnimation");
                    }

                }

            }
        }
        #endregion
        #region Cards Added
        if (ugs.Cards.Count > gameState.Cards.Count)
        {
            Notify(true, "Card added!");
        }
        #endregion
        #region On Turn
        GameObject Turn = uiCanvas.transform.Find("Turn").gameObject;
        if (gameState.onTurn.username != ugs.onTurn.username)
        {
            Turn.transform.Find("User").Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[ugs.onTurn.avatar];
            Color turnColor;
            ColorUtility.TryParseHtmlString(ugs.onTurn.color, out turnColor);
            Turn.transform.Find("User").Find("Frame").GetComponent<Image>().color = turnColor;
            StartTimer(ugs.turnDuration);
        }
        Turn.transform.Find("PhaseTxt").GetComponent<Text>().text = ugs.phase;
        if (ugs.phase == "Draft")
        {
            ClearArrows();
            GameObject onTurn = uiCanvas.transform.Find("Turn").gameObject;
            onTurn.transform.Find("Draft").gameObject.SetActive(true);
            onTurn.transform.Find("Draft").Find("DraftNum").GetComponent<Text>().text = ugs.draftTanks.ToString();
        }
        else
        {
            GameObject onTurn = uiCanvas.transform.Find("Turn").gameObject;
            onTurn.transform.Find("Draft").gameObject.SetActive(false);
        }
        if (ugs.phase == "Fortify")
            ClearArrows();
        #endregion
        #region Players
        if (ugs.players.Count!=gameState.players.Count)
        {
            for (int i = 0; i < ugs.players.Count; i++)
            {
                GameObject userObj = uiCanvas.transform.Find("Users").Find("User" + i.ToString()).gameObject;
                userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[ugs.players[i].avatar];
                userObj.transform.Find("Name").GetComponent<Text>().text = ugs.players[i].username;
                Color boja;
                ColorUtility.TryParseHtmlString(ugs.players[i].color, out boja);
                userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
            }
            for (int i = ugs.players.Count; i < 6; i++)
            {
                GameObject userObj = uiCanvas.transform.Find("Users").Find("User" + i.ToString()).gameObject;
                userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = emptySprite;
                Color boja;
                ColorUtility.TryParseHtmlString("#909090", out boja);
                boja.a = 0.75f;
                userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
                userObj.transform.Find("Name").GetComponent<Text>().text = "";
            }
        }
        #endregion
        gameState = ugs;
        #region Cards
        for (int i = 0; i < ugs.Cards.Count; i++)
        {
            Transform card = uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i.ToString());

            card.Find("Name").GetComponent<Text>().text = ugs.Cards[i].territoryName;
            if (FindTerritoryByName(ugs.Cards[i].territoryName) != null)
                card.Find("Territory").GetComponent<Image>().sprite = FindTerritoryByName(ugs.Cards[i].territoryName).GetComponent<Image>().sprite;
            if (ugs.Cards[i].type == "Tank")
                card.Find("Army").GetComponent<Image>().sprite = tank;
            if (ugs.Cards[i].type == "Plane")
                card.Find("Army").GetComponent<Image>().sprite = plane;
            if (ugs.Cards[i].type == "Solider")
                card.Find("Army").GetComponent<Image>().sprite = solider;
            if (ugs.onTurn.username == myUsername && ugs.phase == "Draft")
                card.GetComponent<Button>().interactable = true;
            else
            {
                card.GetComponent<Button>().interactable = true;
                ClearSelectedCards();
            }
        }
        for (int i = ugs.Cards.Count; i < 5; i++)
        {
            Transform card = uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i.ToString());
            card.Find("Name").GetComponent<Text>().text = "";
            card.Find("Territory").GetComponent<Image>().sprite = emptySprite;
            card.Find("Army").GetComponent<Image>().sprite = emptySprite;
            card.GetComponent<Button>().interactable = false;
        }
        #endregion
        if (ugs.onTurn.username == myUsername)
            MyTurn();
        else
            NotMyTurn();
    }
    private void StartTimer(float countdownValue)
    {
        currCountdownValue = countdownValue;
    }
    public void TerritoryClick()
    {
        string terName = EventSystem.current.currentSelectedGameObject.name;
        if (gameState.onTurn.username == myUsername)
        {
            if (gameState.phase == "Draft")
            {
                DraftTo(terName);
            }
            if (gameState.phase == "Attack")
            {
                if (firstClick == null && gameState.FindTerritory(terName).currentHolder.username == myUsername)
                {
                    firstClick = terName;
                    AttackFrom(terName);
                }
                else if (terName == firstClick)
                {
                    firstClick = null;
                    InitPhase();
                }
                else
                {
                    secondClick = terName;
                    AttackDialog(firstClick, secondClick);
                    firstClick = null;
                    secondClick = null;
                    InitPhase();
                }
            }
            if (gameState.phase == "Fortify")
            {
                if (firstClick == null && gameState.FindTerritory(terName).currentHolder.username == myUsername)
                {
                    firstClick = terName;
                    FortifyFrom(terName);
                }
                else if (terName == firstClick)
                {
                    firstClick = null;
                    InitPhase();
                }
                else
                {
                    secondClick = terName;
                    FortifyDialog(firstClick, secondClick);
                    firstClick = null;
                    secondClick = null;
                    InitPhase();
                }
            }
        }
    }
    private void FortifyDialog(string firstClick, string terName)
    {
        Territory from = gameState.FindTerritory(firstClick);
        uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Msg").GetComponent<Text>().text = "Fortify";
        Dropdown count = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>();
        count.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < from.tanks-1; i++)
            options.Add(new Dropdown.OptionData((i + 1).ToString()));
        count.AddOptions(options);
        Button okBtn = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("OkBtn").GetComponent<Button>();
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(() => { uiCanvas.transform.Find("HowMany").gameObject.SetActive(false); });
        okBtn.onClick.AddListener(() => { commManager.Fortify(firstClick, terName, uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>().value + 1); });
        uiCanvas.transform.Find("HowMany").gameObject.SetActive(true);
    }
    private void FortifyFrom(string terName)
    {
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            foreach (Territory t in c.territories)
            {
                FindTerritoryByName(t.name).GetComponent<Button>().interactable = false;
            }
        }
        Territory from = gameState.FindTerritory(terName);
        FindTerritoryByName(terName).GetComponent<Button>().interactable = true;
        foreach (Territory t in from.neighbors)
        {
            Territory ter = gameState.FindTerritory(t.name);
            if (ter.currentHolder.username == myUsername)
                FindTerritoryByName(t.name).GetComponent<Button>().interactable = true;
        }
    }
    private void AttackDialog(string firstClick, string terName)
    {
        Transform dice = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("Dice");
        for (int i = 0; i < 6; i++)
        {
            dice.Find("Dice" + i.ToString()).GetComponent<Animator>().Play("DiceAnimation");
            dice.Find("Dice" + i.ToString()).gameObject.SetActive(false);
        }
        uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("AttackBtn").GetComponent<Button>().interactable = true;
        ongoingFrom = firstClick;
        ongoingTo = terName;
        Territory from = gameState.FindTerritory(firstClick);
        Territory to = gameState.FindTerritory(terName);
        if (from.currentHolder.username == myUsername && to.currentHolder.username != myUsername)
        {
            commManager.WantToAttack(firstClick, terName);
            Dropdown count = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("With").GetComponent<Dropdown>();
            count.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("ALL!"));
            for (int i = 1; i < from.tanks; i++)
            {
                if (i <= 3)
                    options.Add(new Dropdown.OptionData(i.ToString()));
            } 
            count.AddOptions(options);
            GameObject attacker = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("Attacker").gameObject;
            GameObject defender = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("Defender").gameObject;

            Color attackerColor;
            ColorUtility.TryParseHtmlString(from.currentHolder.color, out attackerColor);
            attacker.transform.Find("Frame").GetComponent<Image>().color = attackerColor;
            attacker.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[from.currentHolder.avatar];
            attacker.transform.Find("Name").GetComponent<Text>().text = from.currentHolder.username;
            attacker.transform.Find("Name").GetComponent<Outline>().effectColor = attackerColor;

            Color defenderColor;
            ColorUtility.TryParseHtmlString(to.currentHolder.color, out defenderColor);
            defender.transform.Find("Frame").GetComponent<Image>().color = defenderColor;
            defender.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[to.currentHolder.avatar];
            defender.transform.Find("Name").GetComponent<Text>().text = to.currentHolder.username;
            defender.transform.Find("Name").GetComponent<Outline>().effectColor = defenderColor;

            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryFrom").GetComponent<Image>().sprite = FindTerritoryByName(this.firstClick).GetComponent<Image>().sprite;
            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryFrom").Find("Count").GetComponent<Text>().text = gameState.FindTerritory(firstClick).tanks.ToString();
            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryTo").GetComponent<Image>().sprite = FindTerritoryByName(terName).GetComponent<Image>().sprite;
            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryTo").Find("Count").GetComponent<Text>().text = gameState.FindTerritory(terName).tanks.ToString();
            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryFrom").GetComponent<Image>().color = attackerColor;
            uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryTo").GetComponent<Image>().color =defenderColor;

            Button attackBtn = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("AttackBtn").GetComponent<Button>();
            attackBtn.onClick.RemoveAllListeners();
            attackBtn.onClick.AddListener(() => { commManager.Attack(firstClick, terName, uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("With").GetComponent<Dropdown>().value); });
            uiCanvas.transform.Find("Attack").gameObject.SetActive(true);
        }
    }
    private void InitPhase()
    {
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            foreach (Territory t in c.territories)
            {
                if (t.currentHolder.username == myUsername)
                {
                    FindTerritoryByName(t.name).GetComponent<Button>().interactable = true;
                    if (gameState.phase != "Draft" && gameState.FindTerritory(t.name).tanks < 2)
                        FindTerritoryByName(t.name).GetComponent<Button>().interactable = false;
                }
                else
                    FindTerritoryByName(t.name).GetComponent<Button>().interactable = false;
            }
        }
    }
    private void AttackFrom(string terName)
    {
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            foreach (Territory t in c.territories)
            {
                FindTerritoryByName(t.name).GetComponent<Button>().interactable = false;
            }
        }
        Territory from = gameState.FindTerritory(terName);
        FindTerritoryByName(terName).GetComponent<Button>().interactable = true;
        foreach (Territory t in from.neighbors)
        {
            Territory ter = gameState.FindTerritory(t.name);
            if (ter.currentHolder.username != myUsername)
                FindTerritoryByName(t.name).GetComponent<Button>().interactable = true;
        }
    }
    private void DraftTo(string terName)
    {
        uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Msg").GetComponent<Text>().text = "Draft";
        Dropdown count = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>();
        count.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < gameState.draftTanks; i++)
            options.Add(new Dropdown.OptionData((i + 1).ToString()));
        count.AddOptions(options);
        Button okBtn = uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("OkBtn").GetComponent<Button>();
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(() => { uiCanvas.transform.Find("HowMany").gameObject.SetActive(false); });
        okBtn.onClick.AddListener(() => { commManager.Draft(terName, uiCanvas.transform.Find("HowMany").Find("HowManyPanel").Find("Count").GetComponent<Dropdown>().value + 1); });
        uiCanvas.transform.Find("HowMany").gameObject.SetActive(true);


    }
    public void EndAttack()
    {
        commManager.EndCurrentAttack();
    }
    public void CardClick()
    {
        string clickedCard = EventSystem.current.currentSelectedGameObject.name;
        int num = 0;
        Transform cardCanvas = uiCanvas.transform.Find("Cards").Find("CardsPanel");
        for (int i = 0; i < 5; i++)
        {
            if (cardCanvas.Find("Card" + i).GetComponent<Outline>().enabled)
                num++;
        }
        if (!cardCanvas.Find(clickedCard).GetComponent<Outline>().enabled && num < 3)
        {
            cardCanvas.Find(clickedCard).GetComponent<Outline>().enabled = true;
        }
        else if (cardCanvas.Find(clickedCard).GetComponent<Outline>().enabled)
        {
            cardCanvas.Find(clickedCard).GetComponent<Outline>().enabled = false;
        }
        CheckSwap();

    }
    private void CheckSwap()
    {
        List<int> clickedCards =new List<int>() ;
        Transform cardCanvas = uiCanvas.transform.Find("Cards").Find("CardsPanel");
        for (int i = 0; i < 5; i++)
        {
            if (cardCanvas.Find("Card" + i).GetComponent<Outline>().enabled)
                clickedCards.Add(i);
        }
        if (clickedCards.Count == 3)
        {
            if (gameState.Cards[clickedCards[0]].type == gameState.Cards[clickedCards[1]].type && gameState.Cards[clickedCards[0]].type == gameState.Cards[clickedCards[2]].type)
            {
                //3 iste
                if (gameState.Cards[clickedCards[0]].type == "Solider")
                    cardCanvas.Find("Bonus").GetComponent<Text>().text = "+4";
                else if (gameState.Cards[clickedCards[0]].type == "Tank")
                    cardCanvas.Find("Bonus").GetComponent<Text>().text = "+6";
                else if (gameState.Cards[clickedCards[0]].type == "Plane")
                    cardCanvas.Find("Bonus").GetComponent<Text>().text = "+8";
                cardCanvas.Find("SwapBtn").gameObject.GetComponent<Button>().interactable = true;
            }
            else if (gameState.Cards[clickedCards[0]].type != gameState.Cards[clickedCards[1]].type && gameState.Cards[clickedCards[0]].type != gameState.Cards[clickedCards[2]].type && gameState.Cards[clickedCards[1]].type != gameState.Cards[clickedCards[2]].type)
            {
                cardCanvas.Find("Bonus").GetComponent<Text>().text = "+10";
                cardCanvas.Find("SwapBtn").gameObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                cardCanvas.Find("Bonus").GetComponent<Text>().text = "";
                cardCanvas.Find("SwapBtn").gameObject.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            cardCanvas.Find("Bonus").GetComponent<Text>().text = "";
            cardCanvas.Find("SwapBtn").gameObject.GetComponent<Button>().interactable = false;
        }
    }
    public void ClearSelectedCards()
    {
        for (int i = 0; i < 5; i++)
            uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i).GetComponent<Outline>().enabled = false;
        CheckSwap();
        
    }
    public void CardSwap()
    {
        List<Card> clickedCards = new List<Card>();
        Transform cardCanvas = uiCanvas.transform.Find("Cards").Find("CardsPanel");
        for (int i = 0; i < 5; i++)
        {
            if (cardCanvas.Find("Card" + i).GetComponent<Outline>().enabled)
                clickedCards.Add(gameState.Cards[i]);
        }
        commManager.SwapCards(clickedCards);
    }
    public void MyTurn()
    {
        
        InitPhase();
        if(uiCanvas.transform.Find("Attack").gameObject.active==true)
        {
            Territory from = gameState.FindTerritory(ongoingFrom);
            Dropdown count = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("With").GetComponent<Dropdown>();
            count.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("ALL!"));
            for (int i = 1; i < from.tanks; i++)
            {
                if (i <= 3)
                    options.Add(new Dropdown.OptionData(i.ToString()));
            }
            count.AddOptions(options);
        }
        uiCanvas.transform.Find("Turn").Find("NextPhaseBtn").GetComponent<Button>().interactable = true;
    }
    public void NotMyTurn()
    {
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            foreach (Territory t in c.territories)
            {
                FindTerritoryByName(t.name).GetComponent<Button>().interactable = false;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("Card" + i.ToString()).GetComponent<Button>().interactable = false;
        }
        uiCanvas.transform.Find("Cards").Find("CardsPanel").Find("SwapBtn").GetComponent<Button>().interactable = false;
        uiCanvas.transform.Find("Turn").Find("NextPhaseBtn").GetComponent<Button>().interactable = false;
    }
    public void Explode(string ter, int loss)
    {
        Transform terr = FindTerritoryByName(ter);
        explosion.transform.Find("Number").GetComponent<Text>().text = "-" + loss.ToString();
        Instantiate(explosion, terr.Find("Tanks").position,Quaternion.identity,mapCanvas.transform.Find("Explosions"));

        if(ter==ongoingFrom && uiCanvas.transform.Find("Attack").gameObject.active==true)
        {
            Transform count = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryFrom").Find("Count");
            count.GetComponent<Text>().text = (Convert.ToInt32(count.GetComponent<Text>().text) - loss).ToString();
            Instantiate(explosion, count.position, Quaternion.identity, count);
        }
        if (ter == ongoingTo && uiCanvas.transform.Find("Attack").gameObject.active == true)
        {
            Transform count = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("TerritoryTo").Find("Count");
            count.GetComponent<Text>().text = (Convert.ToInt32(count.GetComponent<Text>().text)-loss).ToString();
            Instantiate(explosion, count.position, Quaternion.identity, count);
        }
    }
    public void NextPhasePress()
    {
        if (PlayerPrefs.GetInt("SettingsActionConfirm") == 1)
        {
            trueDelegate = NextPhase;
            falseDelegate = DoNothing;
            if (gameState.phase == "Draft")
                ShowDialog(trueDelegate, "yes", falseDelegate, "no", "Continue to Attack phase?");
            if (gameState.phase == "Attack")
                ShowDialog(trueDelegate, "yes", falseDelegate, "no", "Continue to Fortify phase?");
            if (gameState.phase == "Fortify")
                ShowDialog(trueDelegate, "yes", falseDelegate, "no", "End turn?");
        }
        else
        {
            NextPhase();
        }
    }
    public void NextPhase()
    {
        commManager.NextPhase();
    }
    public void VictoryDialog(Player p)
    {
        GameObject userObj = uiCanvas.transform.Find("Endgame").Find("EndgamePanel").Find("User").gameObject;
        userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[p.avatar];
        userObj.transform.Find("Name").GetComponent<Text>().text = p.username;
        Color boja;
        ColorUtility.TryParseHtmlString(p.color, out boja);
        userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
        uiCanvas.transform.Find("Endgame").Find("EndgamePanel").Find("StatusTxt").GetComponent<Text>().text = "WINNER!";
        uiCanvas.transform.Find("Endgame").gameObject.SetActive(true);
    }
    public void LoseDialog(Player p)
    {
        GameObject userObj = uiCanvas.transform.Find("Endgame").Find("EndgamePanel").Find("User").gameObject;
        userObj.transform.Find("Frame").Find("Avatar").GetComponent<Image>().sprite = avatars[p.avatar];
        userObj.transform.Find("Name").GetComponent<Text>().text = p.username;
        Color boja;
        ColorUtility.TryParseHtmlString(p.color, out boja);
        userObj.transform.Find("Frame").GetComponent<Image>().color = boja;
        uiCanvas.transform.Find("Endgame").Find("EndgamePanel").Find("StatusTxt").GetComponent<Text>().text = "You lost";
        uiCanvas.transform.Find("Endgame").gameObject.SetActive(true);
    }
    public void DiceRoll(List<int> attacker, List<int> defender)
    {
        Transform dice = uiCanvas.transform.Find("Attack").Find("AttackPanel").Find("Dice");
        for (int i = 0; i < 6; i++)
        {
            dice.Find("Dice" + i.ToString()).GetComponent<Animator>().Play("DiceAnimation");
            dice.Find("Dice" + i.ToString()).gameObject.SetActive(false);
           // dice.Find("Dice" + i.ToString()).GetComponent<Animator>().SetInteger("Result", 0);
        }
        for (int i = 0; i < attacker.Count; i++)
        {
            //dice.Find("Dice" + i.ToString()).GetComponent<Animator>().Play("DiceAnimation");
            dice.Find("Dice" + i.ToString()).gameObject.SetActive(true);
            dice.Find("Dice" + i.ToString()).GetComponent<Animator>().SetInteger("Result", attacker[i]);
            
        }
        for (int i = 3; i < defender.Count + 3; i++)
        {
            //dice.Find("Dice" + i.ToString()).GetComponent<Animator>().Play("DiceAnimation");
            dice.Find("Dice" + i.ToString()).gameObject.SetActive(true);
            dice.Find("Dice" + i.ToString()).GetComponent<Animator>().SetInteger("Result", defender[i-3]);
            
        }
    }
    public void BonusToggle()
    {
        if (bonusCanvas.activeInHierarchy)
            bonusCanvas.SetActive(false);
        else
            bonusCanvas.SetActive(true);
    }

    public void ShowArrow(string from, string to)
    {
        Vector3 pointA = FindTerritoryByName(from).Find("Tanks").position;
        Vector3 pointB = FindTerritoryByName(to).Find("Tanks").position;
        Vector3 dest = Vector3.Lerp(pointA, pointB, 0.5f);
        float angle= Mathf.Rad2Deg * Mathf.Atan((pointB.y-pointA.y)/(pointB.x-pointA.x));
        if (pointA.x > pointB.x)
            angle = angle + 180;
        arrows.Add(Instantiate(arrowPrefab, dest, Quaternion.Euler(0, 0, angle), arrowContainer.transform));

    }
    public void ClearArrows()
    {
        foreach (GameObject a in arrows)
        {
            GameObject temp = a;
            Destroy(temp);
        }
        arrows.Clear();
    }
    public void DoNothing()
    { 
        return; 
    }
    public void OpenMe(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void CloseMe(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void ShowDialog(Delegate fTrue, string opTrue, Delegate fFalse, string opFalse, string msg)
    {
        uiCanvas.transform.Find("Dialog").gameObject.SetActive(true);
        GameObject dialog = uiCanvas.transform.Find("Dialog").Find("DialogPanel").gameObject;
        dialog.transform.Find("True").Find("Text").GetComponent<Text>().text = opTrue;
        dialog.transform.Find("False").Find("Text").GetComponent<Text>().text = opFalse;
        dialog.transform.Find("Msg").GetComponent<Text>().text = msg;
        dialog.transform.Find("True").GetComponent<Button>().onClick.RemoveAllListeners();
        dialog.transform.Find("False").GetComponent<Button>().onClick.RemoveAllListeners();
        dialog.transform.Find("True").GetComponent<Button>().onClick.AddListener(() => { fTrue(); });
        dialog.transform.Find("False").GetComponent<Button>().onClick.AddListener(() => { fFalse(); });
    }
    public void StartLoadBlack(string mess, Delegate exit)
    {
        GameObject loadContainer = uiCanvas.transform.Find("Loading").gameObject;
        loadContainer.SetActive(true);
        loader = Instantiate(blackLoaderPrefab, loadContainer.transform);
        loader.transform.Find("LoadingTxt").GetComponent<Text>().text = mess;
        loader.transform.Find("LoadingPanel").Find("ExitBtn").gameObject.SetActive(false);
    }
    public void StartLoad(string mess)
    {
        GameObject loadContainer = uiCanvas.transform.Find("Loading").gameObject;
        loadContainer.SetActive(true);
        loader = Instantiate(loaderPrefab, loadContainer.transform);
        loader.transform.Find("LoadingTxt").GetComponent<Text>().text = mess;
    }
    public void EndLoad()
    {
        Destroy(loader);
        GameObject loadContainer = uiCanvas.transform.Find("Loading").gameObject;
        loadContainer.SetActive(false);
    }
    public void Notify(bool isGood, string mess)
    {
        GameObject notification = uiCanvas.transform.Find("NotifyContainer").Find("Notification").gameObject;
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
    public Transform FindTerritoryByName(string name)
    {
        foreach (ActiveContinent c in gameState.gameMap.continents)
        {
            Transform ter = mapCanvas.transform.Find(c.continentName).Find(name);
            if (ter != null)
                return ter;
        }
        return null;
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }

}
