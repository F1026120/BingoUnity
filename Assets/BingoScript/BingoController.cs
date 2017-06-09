using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class BingoController : MonoBehaviour
{
    static readonly int Bound = 5;
    static int Wins_Player = 0;
    static int Wins_Computer = 0;
    static int PlayerLine = 0;
    static int ComLine = 0;
    static int usingAi = 1;
    // 資料結構
	BingoBoard m_PlayerBoard = BoardFactory.CreateBoard(BoardFactory.board_type.manual);
	BingoBoard m_ComBoard = BoardFactory.CreateBoard(BoardFactory.board_type.ai);

    // 換誰出手
    enum WhichOne
    {
        Player = 0,
        Ai,
        GameOver,
    }

    WhichOne m_WhichOnePlay = WhichOne.Player;
    bool m_bNeedFlush = false;
    // 顯示相關
    GameObject[,] m_ComGrid;    // 電腦使用的Bingo盤
    GameObject[,] m_PlayerGrid;    // 玩家使用的Bingo盤
    Text m_PlayerLine;
    Text m_ComLine;
    // 開始時
    void Start()
    {
        InitComGrid();
        InitPlayGrid();
        CreatNextGameButton();
        GameObject tmpObj = null;
        tmpObj = GameObject.Find("PlayerLineTxt");
        m_PlayerLine = tmpObj.GetComponent<Text>();
        tmpObj = GameObject.Find("ComLineTxt");
        m_ComLine = tmpObj.GetComponent<Text>();
        m_PlayerBoard.InitBoard();
        m_ComBoard.InitBoard();
        m_bNeedFlush = true;
    }

    // GameLoop
    void Update()
    {
        // 換AI出手
        if (m_WhichOnePlay == WhichOne.Ai)
        {
            int NextNumber;
            switch (usingAi)
            {
                case 1:
                    NextNumber = m_ComBoard.GetNextNumber(new AiLevel1(Bound));
                    break;
                case 2:
                    NextNumber = m_ComBoard.GetNextNumber(new AiLevel2(Bound));
                    break;
                case 3:
                    NextNumber = m_ComBoard.GetNextNumber(new AiLevel3(Bound));
                    break;
                default:
                    NextNumber = m_ComBoard.GetNextNumber(new AiLevel1(Bound));
                    break;

            }

            //AI難度選擇
            ChangeButtonColor(NextNumber);
            m_ComBoard.SetNumber(NextNumber);
            m_PlayerBoard.SetNumber(NextNumber);
            m_bNeedFlush = true;
            m_WhichOnePlay = WhichOne.Player;
        }

        // 顯示雙方賓果盤
        if (m_bNeedFlush)
        {
            // 計算雙方分數及顯示
            CalculationDisplay();
            // 判斷勝利
            if (PlayerLine >= 5 && ComLine < 5)
            {
                Wins_Player += 1;
                CalculationDisplay();
                m_PlayerLine.text += "你勝了!!!";
                m_WhichOnePlay = WhichOne.GameOver;
            }

            if (ComLine >= 5 && PlayerLine < 5)
            {
                Wins_Computer += 1;
                CalculationDisplay();
                m_ComLine.text += "電腦勝了!!!";
                m_WhichOnePlay = WhichOne.GameOver;
            }

            if (PlayerLine >= 5 && ComLine >= 5)
            {
                CalculationDisplay();
                m_PlayerLine.text += "平手!!!";
                m_ComLine.text += "平手了!!!";
                m_WhichOnePlay = WhichOne.GameOver;
            }

            // 顯示Board內容
            ShowPlayerBingoBoard();
            if (m_WhichOnePlay == WhichOne.GameOver)
                ShowComBingoBoard(false);
            else
                ShowComBingoBoard(true);
            m_bNeedFlush = false;
        }

    }

    // 顯示玩家的賓果盤
    void ShowPlayerBingoBoard()
    {
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
            {
                Text theText = m_PlayerGrid[i, j].GetComponentInChildren<Text>();
                theText.text = string.Format("{0}", m_PlayerBoard.m_Board[i, j]);
            }

    }

    // 顯示電腦的賓果盤
    void ShowComBingoBoard(bool bStartMode)
    {
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
            {
                Text theText = m_ComGrid[i, j].GetComponentInChildren<Text>();
                if (!(bStartMode))
                {
                    theText.text = "";
                    if (m_ComBoard.m_Board[i, j] > 0)
                        theText.text = "*";
                }

                else
                    theText.text = string.Format("{0}", m_ComBoard.m_Board[i, j]);
            }

    }

    // 產生電腦使用的Bingo盤
    void InitComGrid()
    {
        m_ComGrid = new GameObject[Bound, Bound];
        GameObject Obj = GameObject.Find("ComBtn"); // 參考的按鈕
        // 取得按鈕的長寬 
        RectTransform RectInfo = Obj.GetComponent<RectTransform>();
        RectInfo.sizeDelta = new Vector2(350 / Bound, 350 / Bound);
        float BtnWidth = RectInfo.rect.width;
        float BtnHeight = RectInfo.rect.height;
        // 取得位置
        Vector3 PosInfo = Obj.transform.position;
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
            {
                GameObject NewObj = null;
                if (i == 0 && j == 0)
                    NewObj = Obj;
                else
                    NewObj = GameObject.Instantiate(Obj);
                // 設定位置                
                m_ComGrid[i, j] = NewObj;
                NewObj.name = String.Format("{0}{1}", i, j);
                NewObj.transform.SetParent(Obj.transform.parent);
                // 設定位置
                float Posx = PosInfo.x + (BtnWidth * j);
                float Posy = PosInfo.y + -(BtnHeight * i);
                NewObj.transform.position = new Vector3(Posx, Posy, 0);
            }

    }

    // 產生玩家使用的Bingo盤
    void InitPlayGrid()
    {
        m_PlayerGrid = new GameObject[Bound, Bound];
        GameObject Obj = GameObject.Find("PlayerBtn"); // 參考的按鈕
        // 取得按鈕的長寬 
        RectTransform RectInfo = Obj.GetComponent<RectTransform>();
        RectInfo.sizeDelta = new Vector2(350 / Bound, 350 / Bound);
        float BtnWidth = RectInfo.rect.width;
        float BtnHeight = RectInfo.rect.height;
        // 取得位置
        Vector3 PosInfo = Obj.transform.position;
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
            {
                GameObject NewObj = null;
                if (i == 0 && j == 0)
                    NewObj = Obj;
                else
                    NewObj = GameObject.Instantiate(Obj);
                // 設定Text                
                m_PlayerGrid[i, j] = NewObj;
                NewObj.name = String.Format("{0}{1}", i, j);
                NewObj.transform.SetParent(Obj.transform.parent);
                // 設定位置
                float Posx = PosInfo.x + (BtnWidth * j);
                float Posy = PosInfo.y + -(BtnHeight * i);
                NewObj.transform.position = new Vector3(Posx, Posy, 0);
                // 設定Button事件
                Button NewButton = NewObj.GetComponent<Button>();
                NewButton.onClick.AddListener(() => OnPlayerBtnClick(NewButton));
            }

    }

    // 玩家按下Btn
    public void OnPlayerBtnClick(Button theButton)
    {
        if (m_WhichOnePlay != WhichOne.Player)
            return;
        //Debug.Log("OnPlayerBtnClick:" + theButton.gameObject.name);
        // 取得按鈕上的值
        Text theText = theButton.GetComponentInChildren<Text>();
        // 轉換成數字
        int Number = Int32.Parse(theText.text);
        if (Number > 0)
        {
            ColorBlock thecolors;
            thecolors = theButton.colors;
            thecolors.highlightedColor = Color.green;
            theButton.colors = thecolors;
            ChangeButtonColor(Number);
            m_PlayerBoard.SetNumber(Number); // 設定為0
            m_ComBoard.SetNumber(Number);
            m_bNeedFlush = true;
            m_WhichOnePlay = WhichOne.Ai;
        }

    }

    public void ChangeButtonColor(int Number)
    {
        ColorBlock thecolors;
        GameObject BtnObj;
        Button theButton;
        Image theImage;
        for (int c = 0; c < Bound; c++)
            for (int r = 0; r < Bound; r++)
            {
                if (m_PlayerBoard.m_Board[c, r] == Number)
                {
                    BtnObj = m_PlayerGrid[c, r];
                    theButton = BtnObj.GetComponent<Button>();

                    theImage = BtnObj.GetComponent<Image>();
                    theImage.sprite = Resources.Load<Sprite>("Buttonslect");
                    /*
                    thecolors = BtnObj.GetComponent<Button>().colors;
                    thecolors.normalColor = Color.red;
                    theButton.colors = thecolors;
                    */

                }

                if (m_ComBoard.m_Board[c, r] == Number)
                {
                    BtnObj = m_ComGrid[c, r];
                    theButton = BtnObj.GetComponent<Button>();
                    theImage = BtnObj.GetComponent<Image>();
                    theImage.sprite = Resources.Load<Sprite>("Buttonslect");
                    /*
                    thecolors = BtnObj.GetComponent<Button>().colors;
                    thecolors.normalColor = Color.red;
                    theButton.colors = thecolors;
                    */
                }

            }

    }

    public void ResetButtonColor()
    {
        ColorBlock thecolors;
        GameObject BtnObj;
        Button theButton;
        Image theImage;
        for (int c = 0; c < Bound; c++)
            for (int r = 0; r < Bound; r++)
            {
                BtnObj = m_PlayerGrid[c, r];
                theButton = BtnObj.GetComponent<Button>();
                theImage = BtnObj.GetComponent<Image>();
                theImage.sprite = Resources.Load<Sprite>("Buttondefult");
                /*
                thecolors = BtnObj.GetComponent<Button>().colors;
                thecolors.normalColor = Color.white;
                theButton.colors = thecolors;
                */
                BtnObj = m_ComGrid[c, r];
                theButton = BtnObj.GetComponent<Button>();
                theImage = BtnObj.GetComponent<Image>();
                theImage.sprite = Resources.Load<Sprite>("Buttondefult");
                /*
                thecolors = BtnObj.GetComponent<Button>().colors;
                thecolors.normalColor = Color.white;
                theButton.colors = thecolors;
                */
            }

    }

    public void NextGame()//下一局
    {
        GameObject obj = GameObject.Find("BtnNextGame");
        Image theImage = obj.GetComponent<Image>();
        theImage.sprite = Resources.Load<Sprite>("NEXT");
        ResetButtonColor();
        m_PlayerBoard.InitBoard();
        m_ComBoard.InitBoard();
        m_bNeedFlush = true;
        m_WhichOnePlay = WhichOne.Player;
    }

    public void CreatNextGameButton()
    {
        /*
        GameObject Btn_Restart;
        GameObject _text;
        Btn_Restart = new GameObject();
        _text = new GameObject();
        // 取得按鈕的長寬 
        RectTransform RectInfo = Btn_Restart.AddComponent<RectTransform>();
        float BtnWidth = 160f;
        float BtnHeight = 30f;
        // 設定Text                
        Btn_Restart.name = "BtnRestart";
        Text Text = _text.AddComponent<Text>();
        _text.transform.parent = Btn_Restart.transform;
        Text.text = "開始下一局";
        //NewObj.transform.SetParent(Obj.transform.parent);
        Btn_Restart.AddComponent<Image>();
        // 設定位置
        float Posx = 290f;
        float Posy = 166f;
        Btn_Restart.transform.position = new Vector3(Posx, Posy, 0);
        // 設定Button事件
        Button NewButton = Btn_Restart.AddComponent<Button>();
        NewButton.onClick.AddListener(() => NextGame());
        */
        GameObject Btn_NextGame = GameObject.Find("BtnNextGame");
        Button theButton = Btn_NextGame.GetComponent<Button>();
        theButton.onClick.AddListener(() => NextGame());
    }

    public void CalculationDisplay()
    {
        // 計算雙方分數及顯示
        PlayerLine = m_PlayerBoard.CountLine();
        m_PlayerLine.text = string.Format("目前連線數:{0}\n勝場數:{1}", PlayerLine, Wins_Player);
        ComLine = m_ComBoard.CountLine();
        m_ComLine.text = string.Format("目前連線數:{0}\n勝場數:{1}", ComLine, Wins_Computer);
    }

    public void ChangeFinishButtonImage()
    {

    }

    public void UsingAiClick(Button theButton)
    {
        usingAi = Int32.Parse(theButton.name);
        GameObject Obj = GameObject.Find("AIState");
        Text AiText = Obj.GetComponent<Text>();
        AiText.text = "目前Ai狀態: Level " + usingAi;
    }

}


