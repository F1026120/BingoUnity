using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingoBoard
{
    static int Bound = 5;
    public int[,] m_Board = new int[Bound, Bound];
    int[] m_LineValue = new int[(Bound*2)+2];

    public BingoBoard(int b)
    {
        Bound = b;
    }

    // 初始賓果盤
    public void InitBoard()
    {
        // 填值
        int NowNum = 1;
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
                m_Board[i, j] = NowNum++;
        // 打亂
        for (int i = 0; i < Bound; ++i)
            for (int j = 0; j < Bound; ++j)
            {
                int RandPoxi = UnityEngine.Random.Range(0, Bound);
                int RandPoxj = UnityEngine.Random.Range(0, Bound);
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
        for (int i = 0; i < Bound; i++)
        {
            tempvalue = 0;
            for (int j = 0; j < Bound; j++)
                tempvalue += m_Board[i, j];//
            m_LineValue[valueindex++] = tempvalue;
        }

        //計算行值
        for (int j = 0; j < Bound; j++)
        {
            tempvalue = 0;
            for (int i = 0; i < Bound; i++)
                tempvalue += m_Board[i, j];
            m_LineValue[valueindex++] = tempvalue;
        }

        // TODO:左斜,右斜
        tempvalue = 0;
        for (int i = 0; i < Bound; i++)
        {
            tempvalue += m_Board[i, i];
        }
        m_LineValue[valueindex++] = tempvalue;
        tempvalue = 0;
        for (int i = (Bound-1); i >= 0; i--)
        {
            tempvalue += m_Board[i, (Bound-1) - i];
        }

        m_LineValue[valueindex++] = tempvalue;


        //計算連線行數
        int lines = 0;
        for (int i = 0; i < ((Bound*2)+2); i++)
            if (m_LineValue[i] == 0)
                lines++;
        return lines;
    }

    // 設定要消除的號碼
    public void SetNumber(int Value)
    {
        for (int i = 0; i < Bound; i++)
            for (int j = 0; j < Bound; j++)
                if (m_Board[i, j] == Value)
                {
                    m_Board[i, j] = 0;
                    return;
                }
    }



}
