using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KTZ
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int[] need = new int[4];
        int[] stock = new int[3];
        int[,] cost = new int[3, 4];
        int[,] x = new int[3, 4];

        int iPositive = 0;
        int jPositive = 0;

        int iTop = 0;
        int jTop = 0;
        bool foundTop = false;

        TextBox[,] textBoxes = new TextBox[3, 4];
        TextBlock[,] textBlocks = new TextBlock[3, 4];
        StackPanel[,] stackPanels = new StackPanel[3, 4];
        TextBlock[] VTextBlocks = new TextBlock[4];
        TextBlock[] UTextBlocks = new TextBlock[3];

        int[] U = new int[3];
        bool[] UValueGiven = new bool[3] { false, false, false };
        int[] V = new int[4];
        bool[] VValueGiven = new bool[4] { false, false, false, false };

        int[,] grades = new int[3, 4];

        bool planCreated = false;
        bool potential = false;
        bool grade = false;
        bool foundOptimal = true;

        public MainWindow()
        {
            InitializeComponent();

            textBoxes[0, 0] = A1B1;
            textBoxes[0, 1] = A1B2;
            textBoxes[0, 2] = A1B3;
            textBoxes[0, 3] = A1B4;
            textBoxes[1, 0] = A2B1;
            textBoxes[1, 1] = A2B2;
            textBoxes[1, 2] = A2B3;
            textBoxes[1, 3] = A2B4;
            textBoxes[2, 0] = A3B1;
            textBoxes[2, 1] = A3B2;
            textBoxes[2, 2] = A3B3;
            textBoxes[2, 3] = A3B4;

            textBlocks[0, 0] = XA1B1;
            textBlocks[0, 1] = XA1B2;
            textBlocks[0, 2] = XA1B3;
            textBlocks[0, 3] = XA1B4;
            textBlocks[1, 0] = XA2B1;
            textBlocks[1, 1] = XA2B2;
            textBlocks[1, 2] = XA2B3;
            textBlocks[1, 3] = XA2B4;
            textBlocks[2, 0] = XA3B1;
            textBlocks[2, 1] = XA3B2;
            textBlocks[2, 2] = XA3B3;
            textBlocks[2, 3] = XA3B4;

            stackPanels[0, 0] = StackA1B1;
            stackPanels[0, 1] = StackA1B2;
            stackPanels[0, 2] = StackA1B3;
            stackPanels[0, 3] = StackA1B4;
            stackPanels[1, 0] = StackA2B1;
            stackPanels[1, 1] = StackA2B2;
            stackPanels[1, 2] = StackA2B3;
            stackPanels[1, 3] = StackA2B4;
            stackPanels[2, 0] = StackA3B1;
            stackPanels[2, 1] = StackA3B2;
            stackPanels[2, 2] = StackA3B3;
            stackPanels[2, 3] = StackA3B4;

            VTextBlocks[0] = V1;
            VTextBlocks[1] = V2;
            VTextBlocks[2] = V3;
            VTextBlocks[3] = V4;

            UTextBlocks[0] = U1;
            UTextBlocks[1] = U2;
            UTextBlocks[2] = U3;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cost[0, 0] = Convert.ToInt32(A1B1.Text);
                cost[0, 1] = Convert.ToInt32(A1B2.Text);
                cost[0, 2] = Convert.ToInt32(A1B3.Text);
                cost[0, 3] = Convert.ToInt32(A1B4.Text);
                cost[1, 0] = Convert.ToInt32(A2B1.Text);
                cost[1, 1] = Convert.ToInt32(A2B2.Text);
                cost[1, 2] = Convert.ToInt32(A2B3.Text);
                cost[1, 3] = Convert.ToInt32(A2B4.Text);
                cost[2, 0] = Convert.ToInt32(A3B1.Text);
                cost[2, 1] = Convert.ToInt32(A3B2.Text);
                cost[2, 2] = Convert.ToInt32(A3B3.Text);
                cost[2, 3] = Convert.ToInt32(A3B4.Text);

                stock[0] = Convert.ToInt32(A1a.Text);
                stock[1] = Convert.ToInt32(A2a.Text);
                stock[2] = Convert.ToInt32(A3a.Text);

                need[0] = Convert.ToInt32(B1b.Text);
                need[1] = Convert.ToInt32(B2b.Text);
                need[2] = Convert.ToInt32(B3b.Text);
                need[3] = Convert.ToInt32(B4b.Text);
            }
            catch
            {
                MessageBox.Show("Введите числовые значения");
            }

            foreach (var textBox in textBoxes)
            {
                textBox.IsEnabled = false;
            }

            A1a.IsEnabled = false;
            A2a.IsEnabled = false;
            A3a.IsEnabled = false;

            B1b.IsEnabled = false;
            B2b.IsEnabled = false;
            B3b.IsEnabled = false;
            B4b.IsEnabled = false;

            Solve.IsEnabled = false;
            NextStep.IsEnabled = true;
        }

        void Solving()
        {
            if (!planCreated)
            {
                int minI = 0;
                int minJ = 0;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (cost[i, j] < cost[minI, minJ] && stock[i] != 0 && need[j] != 0 || stock[minI] == 0 || need[minJ] == 0)
                        {
                            minI = i;
                            minJ = j;
                        }
                    }
                }


                if (stock[minI] >= need[minJ] && stock[minI] != 0 && need[minJ] != 0)
                {
                    x[minI, minJ] = need[minJ];
                    textBlocks[minI, minJ].Text = "X" + (minI + 1) + "" + (minJ + 1) + " = " + need[minJ];
                    stock[minI] -= need[minJ];
                    need[minJ] = 0;
                    SetNeed(minJ);
                    SetStock(minI);
                    RemoveColumn(minJ);
                }
                else if (stock[minI] < need[minJ])
                {
                    x[minI, minJ] = stock[minI];
                    textBlocks[minI, minJ].Text = "X" + (minI + 1) + "" + (minJ + 1) + " = " + stock[minI];
                    need[minJ] -= stock[minI];
                    stock[minI] = 0;
                    SetNeed(minJ);
                    SetStock(minI);
                    RemoveRow(minI);
                }
                else
                {
                    //надо всё на белый вернуть
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            stackPanels[i, j].Background = new SolidColorBrush(Colors.White);
                        }
                    }
                    planCreated = true;
                }
            }
            else if (!potential)
            {
                bool first = true;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        textBoxes[i, j].Text = cost[i, j].ToString();
                        stackPanels[i, j].Background = new SolidColorBrush(Colors.White);
                    }
                }

                while (UValueGiven.Contains(false) || VValueGiven.Contains(false))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (x[i, j] != 0)
                            {
                                if (first)
                                {
                                    U[i] = 0;
                                    UValueGiven[i] = true;
                                    UTextBlocks[i].Text = U[i].ToString();
                                    V[j] = cost[i, j] + U[i];
                                    VValueGiven[j] = true;
                                    VTextBlocks[j].Text = V[j].ToString();
                                    first = false;
                                }
                                else if (!UValueGiven[i] && VValueGiven[j])
                                {
                                    U[i] = V[j] - cost[i, j];
                                    UValueGiven[i] = true;
                                    UTextBlocks[i].Text = U[i].ToString();
                                }
                                else if (!VValueGiven[j] && UValueGiven[i])
                                {
                                    V[j] = cost[i, j] + U[i];
                                    VValueGiven[j] = true;
                                    VTextBlocks[j].Text = V[j].ToString();
                                }
                            }
                        }
                    }
                }
                potential = true;
                grade = false;
            }
            else if (!grade)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        grades[i, j] = V[j] - U[i] - cost[i, j];
                        textBoxes[i, j].Text = "Δ = " + grades[i, j].ToString();
                        if (grades[i, j] > 0)
                        {
                            foundOptimal = false;
                            iPositive = i;
                            jPositive = j;
                        }
                    }
                }
                grade = true;
            }
            else if (!foundOptimal)
            {
                if (!foundTop)
                {
                    U1.Text = "";
                    U2.Text = "";
                    U3.Text = "";

                    V1.Text = "";
                    V2.Text = "";
                    V3.Text = "";
                    V4.Text = "";

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (i == iPositive || j == jPositive)
                            {
                                continue;
                            }

                            if (x[i, j] != 0 && x[i, jPositive] != 0 && x[iPositive, j] != 0)
                            {
                                iTop = i;
                                jTop = j;
                            }
                        }
                    }
                    stackPanels[iTop, jTop].Background = new SolidColorBrush(Colors.Green);
                    stackPanels[iTop, jPositive].Background = new SolidColorBrush(Colors.Green);
                    stackPanels[iPositive, jTop].Background = new SolidColorBrush(Colors.Green);
                    stackPanels[iPositive, jPositive].Background = new SolidColorBrush(Colors.Green);

                    foundTop = true;

                    textBoxes[iTop, jTop].Text = textBoxes[iTop, jTop].Text + " +";
                    textBoxes[iTop, jPositive].Text = textBoxes[iTop, jPositive].Text + " -";
                    textBoxes[iPositive, jTop].Text = textBoxes[iPositive, jTop].Text + " -";
                    textBoxes[iPositive, jPositive].Text = textBoxes[iPositive, jPositive].Text + " +";

                }
                else
                {
                    int min = Math.Min(x[iTop, jPositive], x[iPositive, jTop]);
                    x[iTop, jTop] = x[iTop, jTop] + min;
                    x[iPositive, jPositive] = x[iPositive, jPositive] + min;
                    x[iPositive, jTop] = x[iPositive, jTop] - min;
                    x[iTop, jPositive] = x[iTop, jPositive] - min;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            textBlocks[i, j].Text = "";
                            if (x[i, j] != 0)
                            {
                                textBlocks[i, j].Text = "X" + (i + 1) + "" + (j + 1) + " = " + x[i, j];
                            }
                        }
                    }

                    foundOptimal = true;
                    foundTop = false;
                    potential = false;

                    UValueGiven = new bool[3] { false, false, false };
                    VValueGiven = new bool[4] { false, false, false, false };
                }

                
            }
            else
            {
                MessageBox.Show("Найдено оптимальное решенеие");
                Solve.IsEnabled = true;
                NextStep.IsEnabled = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Solving();
        }

        void SetNeed(int j)
        {
            switch (j)
            {
                case 0:
                    B1b.Text = need[j].ToString();
                    break;
                case 1:
                    B2b.Text = need[j].ToString();
                    break;
                case 2:
                    B3b.Text = need[j].ToString();
                    break;
                case 3:
                    B4b.Text = need[j].ToString();
                    break;
                default:
                    break;
            }
        }

        void SetStock(int i)
        {
            switch (i)
            {
                case 0:
                    A1a.Text = stock[i].ToString();
                    break;
                case 1:
                    A2a.Text = stock[i].ToString();
                    break;
                case 2:
                    A3a.Text = stock[i].ToString();
                    break;
                default:
                    break;
            }
        }

        void RemoveColumn(int j)
        {
            for (int i = 0; i < 3; i++)
            {
                stackPanels[i, j].Background = new SolidColorBrush(Colors.Red);
            }
        }

        void RemoveRow(int i)
        {
            for (int j = 0; j < 4; j++)
            {
                stackPanels[i, j].Background = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
