using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_report
{
    class Program
    {
        static char[,] Panel;//格局
        static int[] Pos;//玩家座標
        static char Look;//玩家方向
        static int Step;//玩家步數
        static int Change = -1;//敵人移動軸向
        static int Status;//遊戲狀態

        static void Main(string[] args)
        {
            New_Game();
            ConsoleKey Key;
            while (true)
            {
                Key = Console.ReadKey(true).Key;
                while (!Console.KeyAvailable)
                {
                    switch (Key)
                    {
                        case ConsoleKey.R:
                            New_Game();
                            break;
                        case ConsoleKey.Escape:
                            Environment.Exit(0);
                            break;
                    }
                    if (Status == 0)
                        switch (Key)
                        {
                            case ConsoleKey.UpArrow:
                                Move('︿');
                                break;
                            case ConsoleKey.DownArrow:
                                Move('﹀');
                                break;
                            case ConsoleKey.LeftArrow:
                                Move('＜');
                                break;
                            case ConsoleKey.RightArrow:
                                Move('＞');
                                break;
                        }
                    break;
                }
            }
        }

        #region 開啟新局
        static void New_Game()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Status = 0;
            Console.Clear();
            Console.WriteLine("＝遊戲操作＝\n\n方向鍵移動\nR 鍵新遊戲\nEsc 鍵離開\n\n請局格大小 ( 5 ~ 45 )");
            string RL = Console.ReadLine();
            while (int.TryParse(RL, out int n) == false || n < 5 || n > 45)
            {
                Console.WriteLine("請重新輸入數字！");
                RL = Console.ReadLine();
            }
            Panel = new char[int.Parse(RL), int.Parse(RL)];
            Step = 0;
            for (int i = 0; i < Panel.GetLength(0); i++)
            {
                for (int j = 0; j < Panel.GetLength(1); j++)
                {
                    Panel[i, j] = '口';
                }
            }
            Pos = new[] { Panel.GetLength(0) / 2, Panel.GetLength(1) / 2 };
            Look = '︿';
            Panel[Panel.GetLength(0) / 2, Panel.GetLength(1) / 2] = Look;
            Update();
        }
        #endregion

        #region 更新畫面
        static void Update()
        {
            Console.Clear();
            for (int i = 0; i < Panel.GetLength(0); i++)
            {
                for (int j = 0; j < Panel.GetLength(1); j++)
                {
                    switch (Panel[i, j])
                    {
                        case '︿':
                        case '﹀':
                        case '＜':
                        case '＞':
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case '＊':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }
                    Console.Write(Panel[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"\n步數：{Step}");
            if (Status == 1)//遊戲結束
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n＝遊戲結束＝\n\n按下 R 鍵開啟新局\n或按下 Esc 鍵離開");
            }
        }
        #endregion

        #region 玩家移動
        static void Move(char L)
        {
            Look = L;
            int[] newPos = new[] { Pos[0], Pos[1] };
            switch (Look)
            {
                case '︿':
                    if (newPos[0] > 0)
                        newPos[0]--;
                    break;
                case '﹀':
                    if (newPos[0] < Panel.GetLength(0) - 1)
                        newPos[0]++;
                    break;
                case '＜':
                    if (newPos[1] > 0)
                        newPos[1]--;
                    break;
                case '＞':
                    if (newPos[1] < Panel.GetLength(1) - 1)
                        newPos[1]++;
                    break;
            }
            Collision(0, Pos, newPos);
            Enemy();
            Update();
        }
        #endregion

        #region 敵人行為
        static void Enemy()
        {
            int[] ePos = new int[2];
            int[] newePos = new int[2];
            Random R = new Random();
            for (int i = 0; i < Panel.GetLength(0); i++)
            {
                for (int j = 0; j < Panel.GetLength(1); j++)
                {
                    if (Panel[i, j] == '＊')
                    {
                        ePos = new[] { i, j };
                        newePos = new[] { ePos[0], ePos[1] };
                        if (Change == -1)
                        {
                            if (newePos[0] > Pos[0])
                                newePos[0]--;
                        }
                        else
                        {
                            if (newePos[1] > Pos[1])
                                newePos[1]--;
                        }
                        Collision(1, ePos, newePos);
                    }
                }
            }
            for (int i = Panel.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = Panel.GetLength(1) - 1; j >= 0; j--)
                {
                    if (Panel[i, j] == '＊')
                    {
                        ePos = new[] { i, j };
                        newePos = new[] { ePos[0], ePos[1] };
                        if (Change == -1)
                        {
                            if (newePos[0] < Pos[0])
                                newePos[0]++;
                        }
                        else
                        {
                            if (newePos[1] < Pos[1])
                                newePos[1]++;
                        }
                        Collision(1, ePos, newePos);
                    }
                }
            }
            Change *= -1;
            do
            {
                switch (R.Next(0, 4))//進攻方向
                {
                    case 0://上
                        ePos = new[] { 0, R.Next(0, Panel.GetLength(1)) };
                        break;
                    case 1://下
                        ePos = new[] { Panel.GetLength(0) - 1, R.Next(0, Panel.GetLength(1)) };
                        break;
                    case 2://左
                        ePos = new[] { R.Next(0, Panel.GetLength(0)), 0 };
                        break;
                    case 3://右
                        ePos = new[] { R.Next(0, Panel.GetLength(0)), Panel.GetLength(1) - 1 };
                        break;
                }
            } while (Panel[ePos[0], ePos[1]] != '口');
            Panel[ePos[0], ePos[1]] = '＊';
        }
        #endregion

        #region 碰撞偵測
        static void Collision(int P, int[] A, int[] C)
        {
            if (P == 0)//玩家
                switch (Panel[C[0], C[1]])
                {
                    case '口':
                        Panel[C[0], C[1]] = Look;
                        Panel[A[0], A[1]] = '口';
                        Step++;
                        Pos = C;
                        break;
                    case '＊':
                        Status = 1;
                        Panel[A[0], A[1]] = '口';
                        break;
                }
            if (P == 1)//敵人
                switch (Panel[C[0], C[1]])
                {
                    case '口':
                        Panel[C[0], C[1]] = '＊';
                        Panel[A[0], A[1]] = '口';
                        break;
                    case '︿':
                    case '﹀':
                    case '＜':
                    case '＞':
                        Status = 1;
                        goto case '口';
                }
        }
        #endregion
    }
}
