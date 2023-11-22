using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        int rows = 0;
        int columns = 0;

        bool right = true;

        int[] need;
        int[] stock;
        int[,] cost;
        int[,] x;

        int iPositive = 0;
        int jPositive = 0;

        TextBox[,] textBoxes;
        TextBlock[,] textBlocks;
        StackPanel[,] stackPanels;
        TextBlock[] VTextBlocks;
        TextBlock[] UTextBlocks;
        TextBox[] aTextBoxes;
        TextBox[] bTextBoxes;

        int[] U;
        bool[] UValueGiven;
        int[] V;
        bool[] VValueGiven;

        int[,] grades;

        bool[,] usableCells;
        string[,] signs;

        bool planCreated = false;
        bool potential = false;
        bool grade = false;
        bool foundOptimal = true;
        bool solved = false;
        bool foundPath = false;

        public MainWindow(int rows, int columns)
        {
            InitializeComponent();

            this.rows = rows;
            this.columns = columns;

            aTextBoxes = new TextBox[rows];
            bTextBoxes = new TextBox[columns];

            need = new int[columns];
            stock = new int[rows];
            cost = new int[rows, columns];
            x = new int[rows, columns];

            textBoxes = new TextBox[rows, columns];
            textBlocks = new TextBlock[rows, columns];
            stackPanels = new StackPanel[rows, columns];
            VTextBlocks = new TextBlock[columns];
            UTextBlocks = new TextBlock[rows];

            U = new int[rows];
            UValueGiven = new bool[rows];
            V = new int[columns];
            VValueGiven = new bool[columns];

            grades = new int[rows, columns];

            usableCells = new bool[rows, columns];
            signs = new string[rows, columns];

            for (int i = 0; i <= rows; i++)
            {
                MainPlace.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i <= columns; i++)
            {
                MainPlace.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < rows; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "B" + (i + 1);
                MainPlace.Children.Add(textBlock);
                Grid.SetColumn(textBlock, 1);
                Grid.SetRow(textBlock, i + 2);
            }

            for (int i = 0; i < columns; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = "A" + (i + 1);
                MainPlace.Children.Add(textBlock);
                Grid.SetColumn(textBlock, i + 2);
                Grid.SetRow(textBlock, 1);
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    StackPanel stackPanel = new StackPanel();
                    TextBox textBox = new TextBox();
                    TextBlock textBlock = new TextBlock();

                    MainPlace.Children.Add(stackPanel);

                    Grid.SetColumn(stackPanel, j + 2);
                    Grid.SetRow(stackPanel, i + 2);

                    stackPanel.Children.Add(textBox);
                    stackPanel.Children.Add(textBlock);

                    stackPanels[i, j] = stackPanel;
                    textBoxes[i, j] = textBox;
                    textBlocks[i, j] = textBlock;
                }
            }

            for (int i = 0; i < columns; i++)
            {
                TextBlock textBlock = new TextBlock();
                MainPlace.Children.Add(textBlock);
                Grid.SetColumn(textBlock, i + 2);
                VTextBlocks[i] = textBlock;
            }

            for (int i = 0; i < rows; i++)
            {
                TextBlock textBlock = new TextBlock();
                MainPlace.Children.Add(textBlock);
                Grid.SetRow(textBlock, i + 2);
                UTextBlocks[i] = textBlock;
            }

            for (int i = 0; i < columns; i++)
            {
                TextBox textBox = new TextBox();

                MainPlace.Children.Add(textBox);
                Grid.SetRow(textBox, rows + 2);
                Grid.SetColumn(textBox, i + 2);

                bTextBoxes[i] = textBox;
            }

            for (int i = 0; i < rows; i++)
            {
                TextBox textBox = new TextBox();

                MainPlace.Children.Add(textBox);
                Grid.SetColumn(textBox, columns + 2);
                Grid.SetRow(textBox, i + 2);

                aTextBoxes[i] = textBox;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            right = true;

            need = new int[columns];
            stock = new int[rows];
            cost = new int[rows, columns];
            x = new int[rows, columns];

            U = new int[rows];
            UValueGiven = new bool[rows];
            V = new int[columns];
            VValueGiven = new bool[columns];

            grades = new int[rows, columns];

            planCreated = false;
            potential = false;
            grade = false;
            foundOptimal = true;
            solved = false;
            foundPath = false;

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        cost[i, j] = Convert.ToInt32(textBoxes[i, j].Text);
                    }
                }

                for (int i = 0; i < rows; i++)
                {
                    stock[i] = Convert.ToInt32(aTextBoxes[i].Text);
                }

                for (int i = 0; i < columns; i++)
                {
                    need[i] = Convert.ToInt32(bTextBoxes[i].Text);
                }

                foreach (var num in cost)
                {
                    if (num <= 0)
                    {
                        MessageBox.Show("Значение стоимостей должны быть больше 0");
                        right = false;
                    }
                }

                int sumStock = stock.Sum(x => x);
                int sumNeed = need.Sum(x => x);

                if (sumNeed != sumStock)
                {
                    MessageBox.Show("Суммы a должна быть равна b");
                    right = false;
                }

                if (right)
                {
                    foreach (var textBox in textBoxes)
                    {
                        textBox.IsEnabled = false;
                    }

                    foreach (var a in aTextBoxes)
                    {
                        a.IsEnabled = false;
                    }

                    foreach (var b in bTextBoxes)
                    {
                        b.IsEnabled = false;
                    }

                    Solve.IsEnabled = false;
                    NextStep.IsEnabled = true;
                }
            }
            catch
            {
                MessageBox.Show("Введите числовые значения");
            }

        }

        void Solving()
        {
            if (!planCreated)
            {
                int minI = 0;
                int minJ = 0;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
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
                    usableCells[minI, minJ] = true;
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
                    usableCells[minI, minJ] = true;
                    SetNeed(minJ);
                    SetStock(minI);
                    RemoveRow(minI);
                }
                else
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
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

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        textBoxes[i, j].Text = cost[i, j].ToString();
                        stackPanels[i, j].Background = new SolidColorBrush(Colors.White);
                    }
                }

                while (UValueGiven.Contains(false) || VValueGiven.Contains(false))
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
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
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
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
                if (!foundPath)
                {
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (x[i, j] > 0)
                            {
                                usableCells[i, j] = true;
                            }
                            else
                            {
                                usableCells[i, j] = false;
                            }
                        }
                    }
                    FindPath();
                    foundPath = true;
                }
                else
                {
                    List<int> negative = new List<int>();

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (signs[i, j] == "-")
                            {
                                negative.Add(x[i, j]);
                            }
                        }
                    }

                    int min = negative.Min(x => x);

                    x[iPositive, jPositive] = min;
                    textBlocks[iPositive, jPositive].Text = "X" + (iPositive + 1) + "" + (jPositive + 1) + " = " + x[iPositive, jPositive];

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {

                            if (x[i, j] != 0 && (i != iPositive || j != jPositive))
                            {
                                if (signs[i, j] == "+")
                                {
                                    x[i, j] += min;
                                }
                                else if (signs[i, j] == "-")
                                {
                                    x[i, j] -= min;
                                }

                                if (x[i, j] != 0)
                                {
                                    textBlocks[i, j].Text = "X" + (i + 1) + "" + (j + 1) + " = " + x[i, j];
                                }
                                else
                                {
                                    textBlocks[i, j].Text = "";
                                }
                            }
                        }
                    }
                    foundOptimal = true;
                    potential = false;

                    UValueGiven = new bool[rows];
                    VValueGiven = new bool[columns];

                    signs = new string[rows, columns];
                    foundPath = false;
                }
                
                
            }
            else if (solved)
            {
                foreach (var textBox in textBoxes)
                {
                    textBox.Text = "";
                    textBox.IsEnabled = true;
                }
                foreach (var textBlock in UTextBlocks)
                {
                    textBlock.Text = "";
                }
                foreach (var textBlock in VTextBlocks)
                {
                    textBlock.Text = "";
                }
                foreach (var textBlock in textBlocks)
                {
                    textBlock.Text = "";
                }
                foreach (var textBox in aTextBoxes)
                {
                    textBox.IsEnabled = true;
                }
                foreach (var textBox in bTextBoxes)
                {
                    textBox.IsEnabled = true;
                }
                Solve.IsEnabled = true;
                NextStep.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Найдено оптимальное решенеие");
                solved = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Solving();
        }

        void SetNeed(int j)
        {
            bTextBoxes[j].Text = need[j].ToString();
        }

        void SetStock(int i)
        {
            aTextBoxes[i].Text = stock[i].ToString();
        }

        void RemoveColumn(int j)
        {
            for (int i = 0; i < rows; i++)
            {
                stackPanels[i, j].Background = new SolidColorBrush(Colors.Red);
            }
        }

        void RemoveRow(int i)
        {
            for (int j = 0; j < columns; j++)
            {
                stackPanels[i, j].Background = new SolidColorBrush(Colors.Red);
            }
        }

        void FindPath()
        {
            bool[,] usableCellsCopy = (bool[,])usableCells.Clone();
            bool allCellsDeleted = false;
            usableCellsCopy[iPositive, jPositive] = true;
            
            while (!allCellsDeleted)
            {
                allCellsDeleted = true;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (usableCellsCopy[i, j])
                        {
                            bool hasVerticalNeighbours = false;
                            bool hasHorizontalNeighbours = false;

                            for (int k = 0; k < rows; k++)
                            {
                                if (k == i)
                                {
                                    continue;
                                }

                                if (usableCellsCopy[k, j])
                                {
                                    hasVerticalNeighbours = true;
                                }
                            }

                            if (hasVerticalNeighbours)
                            {
                                for (int k = 0; k < columns; k++)
                                {
                                    if (k == j)
                                    {
                                        continue;
                                    }

                                    if (usableCellsCopy[i, k])
                                    {
                                        hasHorizontalNeighbours = true;
                                    }
                                }
                            }

                            if (!hasVerticalNeighbours || !hasHorizontalNeighbours)
                            {
                                allCellsDeleted = false;
                                usableCellsCopy[i, j] = false;
                            }
                        }
                    }
                }
            }

            signs[iPositive, jPositive] = "+";
            textBoxes[iPositive, jPositive].Text = textBoxes[iPositive, jPositive].Text + " " + signs[iPositive, jPositive];
            stackPanels[iPositive, jPositive].Background = new SolidColorBrush(Colors.Green);

            bool allSignsGiven = false;

            while (!allSignsGiven)
            {
                allSignsGiven = true;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (usableCellsCopy[i, j] && signs[i, j] == null)
                        {
                            for (int k = 0; k < rows; k++)
                            {
                                if (k == i)
                                {
                                    continue;
                                }

                                if (usableCellsCopy[k, j] && signs[k, j] != null)
                                {
                                    if (signs[k, j] == "+")
                                    {
                                        signs[i, j] = "-";
                                    }
                                    else if (signs[k, j] == "-")
                                    {
                                        signs[i, j] = "+";
                                    }

                                    textBoxes[i, j].Text = textBoxes[i, j].Text + " " + signs[i, j];
                                    stackPanels[i, j].Background = new SolidColorBrush(Colors.Green);
                                    allSignsGiven = false;
                                }
                            }

                            if (signs[i, j] == null)
                            {
                                for (int k = 0; k < columns; k++)
                                {
                                    if (k == j)
                                    {
                                        continue;
                                    }

                                    if (usableCellsCopy[i, k] && signs[i, k] != null)
                                    {
                                        if (signs[i, k] == "+")
                                        {
                                            signs[i, j] = "-";
                                        }
                                        else if (signs[i, k] == "-")
                                        {
                                            signs[i, j] = "+";
                                        }

                                        textBoxes[i, j].Text = textBoxes[i, j].Text + " " + signs[i, j];
                                        stackPanels[i, j].Background = new SolidColorBrush(Colors.Green);
                                        allSignsGiven = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
