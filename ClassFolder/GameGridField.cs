using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.String;

namespace Minesweeper.ClassFolder
{
    public class GameGridField
    {
        private Button[,] cells;
        private GameField gameField;
        private readonly Grid grid;
        private readonly ResourceDictionary resourceDictionary;
        private readonly Label minesLbl;
        private readonly Label timeLbl;
        private readonly Button restartBtn;
        private DispatcherTimer timer;
        private bool firstClick;
        private int secondsTimer;

        public bool Initialized;
        public int Rows;
        public int Columns;
        public int CurrentMines;
        public int OpenCells;
        public int DefaultMines;
        public bool IsFlagStyle;

        // Ицициализация
        public GameGridField(Grid grid, Label minesLbl, Label timeLbl, Button restartBtn)
        {
            this.grid = grid;
            this.minesLbl = minesLbl;
            this.timeLbl = timeLbl;
            this.restartBtn = restartBtn;

            Initialized = false;
            resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("SourceFolder/Dictionaries/GameDictionary.xaml", UriKind.Relative)
            };

            CreateTimer();
        }

        public void Reset()
        {
            timer.Stop();
            firstClick = false;
            OpenCells = 0;
            secondsTimer = 0;
            IsFlagStyle = true;
            CurrentMines = DefaultMines != 0 ? DefaultMines : GenerateCountMines();
            SetLabels();

            Style closeCellStyle = resourceDictionary["Close"] as Style;
            restartBtn.Style = resourceDictionary["RestartSmile"] as Style;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    cells[i, j].Style = closeCellStyle;
                    cells[i, j].Content = Empty;
                }
            }

            grid.IsEnabled = true;
            gameField.Clear();
        }

        public void ClearGrid()
        {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }

        public void SetLabels()
        {
            minesLbl.Content = String.Format("{0}", CurrentMines);
            timeLbl.Content = String.Format("{0}", new TimeSpan().ToString());
        }

        public void Init(int rows, int columns, int mines)
        {
            Initialized = true;
            firstClick = false;
            OpenCells = 0;
            IsFlagStyle = true;

            Rows = rows;
            Columns = columns;

            DefaultMines = mines != 0 ? mines : 0;
            CurrentMines = DefaultMines != 0 ? DefaultMines : GenerateCountMines();

            gameField = new GameField(rows, columns);
            cells = new Button[Rows, Columns];

            int gridCellSie = GetGridCellSize();

            for (int i = 0; i < Rows; i++)
            {
                RowDefinition rowDefinition = new RowDefinition
                {
                    Height = new GridLength(gridCellSie)
                };
                grid.RowDefinitions.Add(rowDefinition);
            }

            for (int i = 0; i < Columns; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition
                {
                    Width = new GridLength(gridCellSie)
                };
                grid.ColumnDefinitions.Add(columnDefinition);
            }
            SetLabels();
            timer.Start();
        }

        private void FillCells(int clickRow, int clickColumn)
        {
            gameField.AddMines(CurrentMines, clickRow, clickColumn);
            gameField.AddBlanks();
        }

        public void CreateCells()
        {
            var closeCellStyle = resourceDictionary["Close"] as Style;
            gameField = new GameField(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                for (int k = 0; k < Columns; k++)
                {
                    Button btn = new Button
                    {
                        Style = closeCellStyle
                    };

                    btn.Click += OpenCell_Click;
                    btn.MouseRightButtonUp += SetFlag_Click;

                    grid.Children.Add(btn);

                    btn.SetValue(Grid.RowProperty, i);
                    btn.SetValue(Grid.ColumnProperty, k);

                    cells[i, k] = btn;
                    gameField.CellsStates[i, k] = CellState.Closed;
                }
            }

            // MessageBox.Show($"Button size = {cells[0, 0].Width}, {cells[0, 0].Height}");
            // MessageBox.Show($"Grid size = {grid.RowDefinitions[0].Height.Value}, {grid.ColumnDefinitions[0].Width.Value}");
        }

        private int GetGridCellSize()
        {
            if (Rows * 32 > SystemParameters.PrimaryScreenHeight * 0.8)
                return (int)(SystemParameters.PrimaryScreenHeight * 0.8) / Rows;
            if (Columns * 32 > SystemParameters.PrimaryScreenWidth * 0.8)
                return (int)(SystemParameters.PrimaryScreenWidth * 0.8) / Columns;
            return 32;
        }

        private int GenerateCountMines()
        {
            return (int)(Rows * Columns * 0.25);
        }

        private void MarkMinesWithFlags()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (gameField.CellsStates[row, column] == CellState.Closed)
                        SetFlag(row, column);
                }
            }
        }

        private void EndGame(bool isWin, bool isFlagStyle)
        {
            timer.Stop();

            if (isFlagStyle)
                MarkMinesWithFlags();

            GameInfo gameInfo = new GameInfo(Rows * Columns, secondsTimer, isWin, isFlagStyle);
            AccountGameInfoManager.Update(gameInfo);

            secondsTimer = 0;
            grid.IsEnabled = false;

            if (isWin)
            {
                restartBtn.Style = resourceDictionary["WinRestartSmile"] as Style;
                //MessageBox.Show("You win!");
            }
            else
            {
                restartBtn.Style = resourceDictionary["LoseRestartSmile"] as Style;
                //MessageBox.Show("You lose!");
            }

            if (AccountGameInfoManager.userAccount.Status == AccountStatus.Complete)
            {
                new WindowFolder.WinNotifyWindow(gameInfo).ShowDialog();
            }
        }

        private void ShowMines(int rowActiveMine, int columnActiveMine)
        {
            Style mineStyle = resourceDictionary["Mine"] as Style;
            Style wrongFlagStyle = resourceDictionary["WrongFlag"] as Style;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (gameField.IsMine(i, j))
                        cells[i, j].Style = mineStyle;
                    else if (gameField.CellsStates[i, j] == CellState.Flagged)
                        cells[i, j].Style = wrongFlagStyle;
                }
            }
            cells[rowActiveMine, columnActiveMine].Style = resourceDictionary["MineActive"] as Style;
        }

        public bool IsWin()
        {
            return CurrentMines == 0 && OpenCells == (Rows * Columns) - gameField.Mines;
        }

        public bool IsFlagStyleWin()
        {
            return IsFlagStyle && OpenCells == (Rows * Columns - gameField.Mines);
        }

        private void OpenCell(int row, int column)
        {
            if (!gameField.IsEmpty(row, column))
            {
                cells[row, column].Style = resourceDictionary[gameField.Field[row, column].ToString()] as Style;
                cells[row, column].Content = gameField.Field[row, column].ToString();
                gameField.CellsStates[row, column] = CellState.Open;
                OpenCells++;
            }
            else
            {
                List<Point> toOpenPoses = gameField.GetToOpenPositions(row, column);
                OpenCells += toOpenPoses.Count;
                foreach (var point in toOpenPoses)
                {
                    cells[point.X, point.Y].Style =
                        resourceDictionary[gameField.Field[point.X, point.Y].ToString()] as Style;

                    if (!gameField.IsEmpty(point.X, point.Y))
                        cells[point.X, point.Y].Content = gameField.Field[point.X, point.Y].ToString();

                    gameField.CellsStates[point.X, point.Y] = CellState.Open;
                }
            }
        }

        private void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            secondsTimer++;
            timeLbl.Content = $"{secondsTimer / 3600:d2}:{secondsTimer / 60 % 60:d2}:{secondsTimer % 60:d2}";
        }

        private void SetFlag(int row, int column)
        {
            if (gameField.CellsStates[row, column] == CellState.Closed)
            {
                cells[row, column].Style = resourceDictionary["Flag"] as Style;
                gameField.CellsStates[row, column] = CellState.Flagged;
                minesLbl.Content = Format("{0}", --CurrentMines);
            }
            else if (gameField.CellsStates[row, column] == CellState.Flagged)
            {
                cells[row, column].Style = resourceDictionary["Close"] as Style;
                gameField.CellsStates[row, column] = CellState.Closed;
                minesLbl.Content = Format("{0}", ++CurrentMines);
            }
        }

        public void OpenCell_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            int row = (int)btn.GetValue(Grid.RowProperty);
            int column = (int)btn.GetValue(Grid.ColumnProperty);

            if (firstClick == false)
            {
                timer.Start();
                FillCells(row, column);
                firstClick = true;
            }

            if (gameField.CellsStates[row, column] == CellState.Closed)
            {
                if (gameField.IsMine(row, column))
                {
                    EndGame(false, false);
                    ShowMines(row, column);
                }
                else
                    OpenCell(row, column);
            }
            if (IsWin() || IsFlagStyleWin())
                EndGame(true, IsFlagStyleWin());

            //GameInfo gameInfo = new GameInfo(Rows * Columns, 30, false);
            //AccountGameInfoManager.Update(gameInfo);
        }

        public void SetFlag_Click(object sender, RoutedEventArgs e)
        {
            if (firstClick)
            {
                IsFlagStyle = false;
                Button btn = (Button)sender;
                int row = (int)btn.GetValue(Grid.RowProperty);
                int column = (int)btn.GetValue(Grid.ColumnProperty);

                SetFlag(row, column);
                if (IsWin() || IsFlagStyleWin())
                    EndGame(true, IsFlagStyleWin());
            }
        }
    }
}