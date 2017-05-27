using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 賓果Ai
namespace AiBingoBoard
{

    public class AiStrategy : BingoBoard
    {
        public AiStrategy(int b) : base(b)
        {
        }

        public int GetNextNumber(AiLevelinterface AiContext)
        {
            return AiContext.GetNextNumber(m_Board);
        }

    }

    public interface AiLevelinterface
    {

        int GetNextNumber(int[,] m_Board);

    }

    public class AiLevel1 : AiLevelinterface
    {
        protected int[] m_LinePoint = new int[12];


        public int GetNextNumber(int[,] m_Board) //老師的AI
        {

            int lineindex = 0;
            int point = 0;

            //計算列值
            for (int i = 0; i < 5; i++)
            {
                point = 0;
                for (int j = 0; j < 5; j++)
                    if (m_Board[i, j] > 0)
                        point++;
                m_LinePoint[lineindex++] = point;
            }

            //計算行值
            for (int j = 0; j < 5; j++)
            {
                point = 0;
                for (int i = 0; i < 5; i++)
                    if (m_Board[i, j] > 0)
                        point++;
                m_LinePoint[lineindex++] = point;
            }

            // TODO:左斜,右斜
            for (int i = 0; i < 5; i++)
            {
                point = 0;
                if (m_Board[i, i] > 0)
                    point++;
            }
            m_LinePoint[lineindex++] = point;


            for (int i = 4; i >= 0; i--)
            {
                point = 0;
                if (m_Board[i, 4 - i] > 0)
                    point++;
            }
            m_LinePoint[lineindex++] = point;

            // 取得最少分數者
            int MinNum = 6;
            int MinIndex = -1;
            for (lineindex = 0; lineindex < 12; ++lineindex)
                if (m_LinePoint[lineindex] != 0 && m_LinePoint[lineindex] < MinNum)
                {
                    MinNum = m_LinePoint[lineindex];
                    MinIndex = lineindex;
                }

            // 決定出牌,列方向
            int NextNumber = 0;
            if (MinIndex < 5)
            {
                // 第一個號碼
                for (int j = 0; j < 5; j++)
                    if (m_Board[MinIndex, j] != 0)
                    {
                        NextNumber = m_Board[MinIndex, j];
                        break;
                    }
            }
            // 決定出牌,行方向
            else if (MinIndex < 10)
            {
                // 第一個號碼
                for (int i = 0; i < 5; i++)
                    if (m_Board[i, MinIndex - 5] != 0)
                    {
                        NextNumber = m_Board[i, MinIndex - 5];
                        break;
                    }
            }
            // TODO,左斜(左上->右下)
            else if (MinIndex == 10)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (m_Board[i, i] != 0)
                    {
                        NextNumber = m_Board[i, i];
                        //Debug.Log("電腦選擇左斜");
                        break;
                    }
                }
            }
            // TODO,右斜(右上->左下)
            else
            {
                for (int i = 4; i >= 0; i--)
                    if (m_Board[i, 4 - i] != 0)
                    {
                        NextNumber = m_Board[i, 4 - i];
                        //Debug.Log("電腦選擇右斜");
                        break;
                    }
            }

            //Debug.Log("電腦出牌[" + NextNumber + "]" + "價值 " + MaxPrice);
            return NextNumber;
        }
        // 決定出牌
    }
    public class AiLevel2 : AiLevelinterface
    {
        int bound = 5; //正方形邊長
        public AiLevel2(int b)
        {
            bound = b;
        }
        public int GetNextNumber(int[,] m_Board)// Ai lebel 2
        {

            int[,] point = new int[5, 5];
            int NextNumber = -1;
            int col = 5;
            int row = 5;
            
                           //計算每個位置價值(point)
            for (int c = 0; c < bound; c++)
            {
                for (int r = 0; r < bound; r++)
                {
                    point[c, r] = 0;
                    if (m_Board[c, r] == 0)//如果以選過則不用判斷
                        continue;
                    for (int k = 0; k < bound; k++)
                    {
                        if (m_Board[c, k] == 0) point[c, r] += 1;//判斷row方向之已選過的點 
                        if (m_Board[k, r] == 0) point[c, r] += 1;//判斷col方向之已選過的點
                        if (c == r || (c + r) == (bound - 1))//如果為斜線上的點
                        {
                            if (c == r)//左上向右下的斜線
                                if (m_Board[k, k] == 0) point[c, r] += 2;
                            if ((c + r) == (bound - 1))//左下向 右上的斜線
                                if (m_Board[k, (bound - 1) - k] == 0) point[c, r] += 2;

                        }
                    }
                    if (c == r || (c + r) == (bound - 1)) point[c, r] += 8;//斜線上的點加權8
                    if (c == r && (c + r) == (bound - 1)) point[c, r] += 4;//中心交點 額外加權 4

                }
            }
            //找出最有價值的選擇
            int MaxPrice = -1;
            int MaxPriceNumber = -1;
            for (int c = 0; c < 5; c++)
            {
                for (int r = 0; r < 5; r++)
                {
                    if (point[c, r] > MaxPrice)
                    {
                        MaxPrice = point[c, r];
                        MaxPriceNumber = m_Board[c, r];
                    }
                }
            }
            NextNumber = MaxPriceNumber;
            //Debug.Log("電腦出牌[" + NextNumber + "]" + "價值 " + MaxPrice);
            return NextNumber;
        }
    }
    public class AiLevel3 : AiLevelinterface
    {
        public int GetNextNumber(int[,] m_Board) //Ai level 3
        {
            int[,] point = new int[5, 5];
            int NextNumber = -1;
            int[,] colPrice = new int[5, 5];
            int[,] rowPrice = new int[5, 5];
            int bound = 5; //正方形邊長
                           //計算每個位置價值(point)
            for (int c = 0; c < bound; c++)
            {
                for (int r = 0; r < bound; r++)
                {
                    point[c, r] = 0;
                    colPrice[c, r] = 0;
                    rowPrice[c, r] = 0;
                    if (m_Board[c, r] == 0)//如果以選過則不用判斷
                        continue;
                    for (int k = 0; k < bound; k++)
                    {
                        if (m_Board[c, k] == 0) { point[c, r] += 1; rowPrice[c, r] += 1; }//判斷row方向之已選過的點 
                        if (m_Board[k, r] == 0) { point[c, r] += 1; colPrice[c, r] += 1; }//判斷col方向之已選過的點
                        if (c == r || (c + r) == (bound - 1))//如果為斜線上的點
                        {
                            if (c == r)//左上向右下的斜線
                                if (m_Board[k, k] == 0) point[c, r] += 2;
                            if ((c + r) == (bound - 1))//左下向 右上的斜線
                                if (m_Board[k, (bound - 1) - k] == 0) point[c, r] += 2;

                        }
                    }
                    if (c == r || (c + r) == (bound - 1)) point[c, r] += 8;//斜線上的點加權8
                    if (c == r && (c + r) == (bound - 1)) point[c, r] += 4;//中心交點 額外加權 4
                }
            }
            //找出最有價值的選擇
            int MaxPrice = -1;
            int MaxPriceNumber = -1;
            int MaxPry = -1;//優先值 => (rowPrice - colPrice )^2
            for (int c = 0; c < 5; c++)
            {
                for (int r = 0; r < 5; r++)
                {
                    if (point[c, r] == MaxPrice)//if 兩點價值相等 比較優先度
                    {
                        int thepry = (colPrice[c, r] - rowPrice[c, r]) * (colPrice[c, r] - rowPrice[c, r]);//平方取正
                        if (thepry > MaxPry)
                        {
                            MaxPry = thepry;
                            MaxPrice = point[c, r];
                            MaxPriceNumber = m_Board[c, r];
                        }

                    }
                    else if (point[c, r] > MaxPrice)//比較價值 
                    {
                        MaxPrice = point[c, r];
                        MaxPry = (colPrice[c, r] - rowPrice[c, r]) * (colPrice[c, r] - rowPrice[c, r]);
                        MaxPriceNumber = m_Board[c, r];
                    }
                }
            }
            NextNumber = MaxPriceNumber;
            //Debug.Log("電腦出牌[" + NextNumber + "]" + "價值 " + MaxPrice + "優先值 " + MaxPry);
            return NextNumber;
        }
    }
}

