using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        private readonly string[] Info = new string[61];
        private readonly int[,] Map = new int[10,10];
        //0无子 1黑色 2白色 
        private int MyColor;
        private int x1;
        //落子位置 
        private int y1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Show_Can_Position()
        {
            //用图片形式显示可以落子的位置 
            int i;
            int j;
            Graphics g = pictureBox1.CreateGraphics();
            Bitmap bitmap = new Bitmap("Info2.png");
            //提示图片 
            int n = 0;
            for (i = 1; i <= 8; i++)
            {
                for (j = 1; j <= 8; j++)
                {
                    if (Map[i, j] == 0 & Can_go(i, j))
                    {
                        Info[n] = i + "|" + j;
                        n = n + 1;
                        g.DrawImage(bitmap, (i - 1)*45 + 26, (j - 1)*45 + 26, 30, 30);
                    }
                }
            }
        }

        //统计可以落子的位置数 
        private int Show_Can_Num()
        {
            int i, j;
            int n = 0;
            for (i = 1; i <= 8; i++)
            {
                for (j = 1; j <= 8; j++)
                {
                    if (Can_go(i, j))
                    {
                        Info[n] = i + "|" + j;
                        n = n + 1;
                    }
                }
            }
            return n;
            //可以落子的位置个数 
        }

        private void Cls_Can_Position()
        {
            int n;
            string a;
            string b;
            int x;
            int y;
            string s;
            Graphics g = pictureBox1.CreateGraphics();
            Bitmap bitmap = new Bitmap("BackColor.png");
            //背景图片 
            for (n = 0; n <= 60; n++)
            {
                s = Info[n];
                if (string.IsNullOrEmpty(s)) break;

                a = s.Substring(0, 1);
                //b = s.Substring(Strings.InStr(s, "|"), 1);
                b = s.Substring(s.IndexOf('|', 1) + 1);
                x = Convert.ToInt16(a);
                y = Convert.ToInt16(b);
                if (Map[x, y] == 0)
                {
                    g.DrawImage(bitmap, (x - 1)*45 + 26, (y - 1)*45 + 26, 30, 30);
                }
                //Me.Text = CInt(x) & y 
            }
        }

        private bool CheckDirect(int x1, int y1, int dx, int dy)
        {
            int x, y;
            bool flag;
            x = x1 + dx;
            y = y1 + dy;
            flag = false;
            while (InBoard(x, y) & !Ismychess(x, y) & Map[x, y] != 0)
            {
                x += dx;
                y += dy;
                flag = true; //构成夹击之势 
            }
            if (InBoard(x, y) & Ismychess(x, y) & flag)
            {
                return true; //该方向落子有效 
            }
            return false;
        }

        private void DirectReverse(int x1, int y1, int dx, int dy)
        {
            int x, y;
            bool flag;
            x = x1 + dx;
            y = y1 + dy;
            flag = false;
            while (InBoard(x, y) & !Ismychess(x, y) & Map[x, y] != 0)
            {
                x += dx;
                y += dy;
                flag = true; //构成夹击之势 
            }
            if (InBoard(x, y) & Ismychess(x, y) & flag)
            {
                do
                {
                    x -= dx;
                    y -= dy;
                    if ((x != x1 || y != y1)) FanQi(x, y);
                } while ((x != x1 || y != y1));
            }
        }

        private bool InBoard(int x, int y)
        {
            if (x >= 1 & x <= 8 & y >= 1 & y <= 8)
                return true;
            else
                return false;
        }

        private bool Can_go(int x1, int y1)
        {
            //从左，左上，上，右上，右，右下，下，左下八个方向判断

            if (CheckDirect(x1, y1, -1, 0))
                return true;
            if (CheckDirect(x1, y1, -1, -1))
                return true;
            if (CheckDirect(x1, y1, 0, -1))
                return true;
            if (CheckDirect(x1, y1, 1, -1))
                return true;
            if (CheckDirect(x1, y1, 1, 0))
                return true;
            if (CheckDirect(x1, y1, 1, 1))
                return true;
            if (CheckDirect(x1, y1, 0, 1))
                return true;
            if (CheckDirect(x1, y1, -1, 1))
                return true;
            return false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int x, y;
            int n = 0;
            x1 = (e.X - 22)/45 + 1;
            y1 = (e.Y - 22)/45 + 1;
            if (!Can_go(x1, y1))
            {
                toolStripStatusLabel1.Text = "此处不能走棋子";
                return;
            }

            if (MyColor == 1)
            {
                listBox1.Items.Add("黑色落在(" + Convert.ToChar(x1 + 64).ToString() + "，" + y1 + ")");
            }
            else
            {
                listBox1.Items.Add("白色落在(" + Convert.ToChar(x1 + 64).ToString() + "，" + y1 + ")");
            }

            //(x1,y1)处原色处理 
            Graphics g = pictureBox1.CreateGraphics();
            Bitmap bitmap = new Bitmap("WhiteStone.png");
            if (MyColor == 2)
            {
                Map[x1, y1] = 2;
                g.DrawImage(bitmap, (x1 - 1)*45 + 22, (y1 - 1)*45 + 22, 45, 45);
            }
            if (MyColor == 1)
            {
                Map[x1, y1] = 1;
                bitmap = new Bitmap("BlackStone.png");
                g.DrawImage(bitmap, (x1 - 1)*45 + 22, (y1 - 1)*45 + 22, 45, 45);
            }

            //从左，左上，上，右上，右，右下，下，左下八个方向翻转 
            if (CheckDirect(x1, y1, -1, 0)) //向左方向形成夹击之势
                DirectReverse(x1, y1, -1, 0);
            if (CheckDirect(x1, y1, -1, -1)) //向左上方向形成夹击之势
                DirectReverse(x1, y1, -1, -1);
            if (CheckDirect(x1, y1, 0, -1)) //向上方向形成夹击之势
                DirectReverse(x1, y1, 0, -1);
            if (CheckDirect(x1, y1, 1, -1)) //向右上方向形成夹击之势
                DirectReverse(x1, y1, 1, -1);

            if (CheckDirect(x1, y1, 1, 0))
                DirectReverse(x1, y1, 1, 0);
            if (CheckDirect(x1, y1, 1, 1))
                DirectReverse(x1, y1, 1, 1);
            if (CheckDirect(x1, y1, 0, 1))
                DirectReverse(x1, y1, 0, 1);
            if (CheckDirect(x1, y1, -1, 1))
                DirectReverse(x1, y1, -1, 1);
            Cls_Can_Position(); //清除提示             
            if (MyColor == 1)
            {
                //状态行提示该对方走棋 
                MyColor = 2;
                toolStripStatusLabel1.Text = "白色棋子走";
            }
            else
            {
                MyColor = 1;
                toolStripStatusLabel1.Text = "黑色棋子走";
            }
            Show_Can_Position(); //显示提示             
            if (Show_Can_Num() == 0)
            {
                MessageBox.Show("提示", "对方无可走位置,请继续走棋");
                if (MyColor == 1)
                {
                    MyColor = 2;
                    toolStripStatusLabel1.Text = "白色棋子继续走";
                }
                else
                {
                    MyColor = 1;
                    toolStripStatusLabel1.Text = "黑色棋子继续走";
                }
                Show_Can_Position(); //显示提示 
            }
            //判断游戏是否结束 
            int whitenum = 0;
            int blacknum = 0;
            for (x = 1; x <= 8; x++)
            {
                for (y = 1; y <= 8; y++)
                {
                    if (Map[x, y] != 0)
                    {
                        n = n + 1;
                        if (Map[x, y] == 2)
                            whitenum += 1;
                        if (Map[x, y] == 1)
                            blacknum += 1;
                    }
                }
            }
            if (n == 64) //在棋盘下满时， 
            {
                if (blacknum > whitenum)
                {
                    MessageBox.Show("游戏结束，恭喜黑方爸爸胜利，白方儿子你的父亲还有刘洋 李文全 宫矫健 姜建鹏 王宏宇。", "黑方:" + blacknum + "白方:" + whitenum);
                }
                else
                {
                    MessageBox.Show("游戏结束，恭喜白方爸爸胜利，黑方儿子你的父亲还有刘洋 李文全 宫矫健 姜建鹏 王宏宇。", "黑方:" + blacknum + "白方:" + whitenum);
                }
                Application.Exit();
                pictureBox1.Enabled = false; //游戏结束，不能走棋 
                button1.Enabled = true; //"开始游戏"按钮有效 
                return;
            }
            //在棋盘还没有下满时 
            if (whitenum == 0)
            {
                MessageBox.Show("游戏结束，恭喜黑方爸爸胜利，白方儿子你的父亲还有刘洋 李文全 宫矫健 姜建鹏 王宏宇。", "黑方:" + blacknum + "白方:" + whitenum);
                pictureBox1.Enabled = false; //游戏结束，不能走棋 
                button1.Enabled = true; //"开始游戏"按钮有效 
                Application.Exit();
            }
            if (blacknum == 0)
            {
                MessageBox.Show("游戏结束，恭喜白方爸爸胜利，黑方儿子你的父亲还有刘洋 李文全 宫矫健 姜建鹏 王宏宇。", "黑方:" + blacknum + "白方:" + whitenum);
                pictureBox1.Enabled = false; //游戏结束，不能走棋 
                button1.Enabled = true; //"开始游戏"按钮有效 
                Application.Exit();
            }
        }


        private void FanQi(int x, int y)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Bitmap bitmap = new Bitmap("WhiteStone.png");

            //1黑色 2白色 
            if (Map[x, y] == 1)
            {
                Map[x, y] = 2;
                g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            }
            else
            {
                Map[x, y] = 1;
                bitmap = new Bitmap("BlackStone.png");
                g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            }
            Thread.Sleep(200); //延时0.2秒
            listBox1.Items.Add(" (" + Convert.ToChar(x + 64).ToString() + "，" + y + ")处被反色");
        }

        private bool Ismychess(int x, int y)
        {
            if (Map[x, y] == MyColor)
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //开始游戏按钮事件过程 
            int x, y;
            Graphics g = pictureBox1.CreateGraphics();
            Bitmap bitmap = new Bitmap("WhiteStone.png");
            pictureBox1.Enabled = true;
            pictureBox1.Refresh();
            for (x = 1; x <= 8; x++)
            {
                for (y = 1; y <= 8; y++)
                {
                    Map[x, y] = 0;
                }
            }
            listBox1.Items.Clear();
            //棋盘上画初始４个棋子 
            x = 4;
            y = 4;
            g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            x = 5;
            y = 5;
            g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            bitmap = new Bitmap("BlackStone.png");
            x = 5;
            y = 4;
            g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            x = 4;
            y = 5;
            g.DrawImage(bitmap, (x - 1)*45 + 22, (y - 1)*45 + 22, 45, 45);
            Map[4, 4] = 2;
            //0无子 1黑色 2白色 
            Map[5, 5] = 2;
            Map[4, 5] = 1;
            Map[5, 4] = 1;
            MyColor = 1;
            //自己棋子颜色为黑色 
            toolStripStatusLabel1.Text = "黑色棋子先走";
            Text = "黑白棋游戏";
            Show_Can_Position();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Show_Can_Position();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Cls_Can_Position();
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button5.Visible = true;
            button4.Visible = false;
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.Visible = false;
            button4.Visible = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
{
    //自身窗口上的关闭按钮
    case CloseReason.FormOwnerClosing:
            e.Cancel = true;//拦截，不响应操作
            break;
    //MDI窗体关闭事件
    case CloseReason.MdiFormClosing:
            e.Cancel = true;//拦截，不响应操作
            break;
    //不明原因的关闭
    case CloseReason.None:
            break;
    //用户通过UI关闭窗口或者通过Alt+F4关闭窗口
    case CloseReason.UserClosing:
            e.Cancel = true;//拦截，不响应操作
            break;
    //操作系统准备关机
    case CloseReason.WindowsShutDown:
            e.Cancel = false;//不拦截，响应操作
            break;
    default:
            break;
}
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.ReadOnly = true;
        }



    }
}