using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoBoard
{
    static int Bound = 5;
    public int[,] m_Board = new int[5, 5];
    int[] m_LineValue = new int[12];

    public BingoBoard(int b)
    {
        Bound = b;
    }

    // 初始賓果盤
    public void InitBoard()
    {
        // 填值
        int NowNum = 1;
        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
                m_Board[i, j] = NowNum++;
        // 打亂
        for (int i = 0; i < 5; ++i)
            for (int j = 0; j < 5; ++j)
            {
                int RandPoxi = UnityEngine.Random.Range(0, 5);
                int RandPoxj = UnityEngine.Random.Range(0, 5);
                int tmpVal = m_Board[RandPoxi, RandPoxj];
                m_Board[RandPoxi, RandPoxj] = m_Board[i, j];
                m_Board[i, j] = tmpVal;
            }
    }

    // 計算
    public int CountLine()
    {
        int valueindex = 0;
        int tempvalue = 0;

        //計算列值
        for (int i = 0; i < 5; i++)
        {
            tempvalue = 0;
            for (int j = 0; j < 5; j++)
                tempvalue += m_Board[i, j];//
            m_LineValue[valueindex++] = tempvalue;
        }

        //計算行值
        for (int j = 0; j < 5; j++)
        {
            tempvalue = 0;
            for (int i = 0; i < 5; i++)
                tempvalue += m_Board[i, j];
            m_LineValue[valueindex++] = tempvalue;
        }

        // TODO:左斜,右斜
        tempvalue = 0;
        for (int i = 0; i < 5; i++)
        {
            tempvalue += m_Board[i, i];
        }
        m_LineValue[valueindex++] = tempvalue;
        tempvalue = 0;
        for (int i = 4; i >= 0; i--)
        {
            tempvalue += m_Board[i, 4 - i];
        }

        m_LineValue[valueindex++] = tempvalue;


        //計算連線行數
        int lines = 0;
        for (int i = 0; i < 12; i++)
            if (m_LineValue[i] == 0)
                lines++;
        return lines;
    }

    // 設定要消除的號碼
    public void SetNumber(int Value)
    {
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                if (m_Board[i, j] == Value)
                {
                    m_Board[i, j] = 0;
                    return;
                }
    }



}
