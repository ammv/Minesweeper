using System;
using System.Collections.Generic;

namespace Minesweeper.ClassFolder
{
    internal struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool IsBetweenTwoPoints(Point p1, Point p2, int x, int y)
        {
            return p1.X <= x
                && p2.X >= x
                && p1.Y <= y
                && p2.Y >= y;
        }
    }

    public enum CellState
    {
        Closed,
        Open,
        Flagged
    }

    internal class GameField
    {
        public int[,] Field { get; set; }
        public CellState[,] CellsStates { get; set; } // 0 - Close, 1 - Open, 2 - Flag
        public int Mines { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public GameField(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Field = new int[rows, columns];
            CellsStates = new CellState[rows, columns];
            FillField();
        }

        // Заполняет поле значением -2 - Отсутсвие значения
        private void FillField()
        {
            Mines = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Field[i, j] = -2;
                    CellsStates[i, j] = CellState.Closed;
                }
            }
        }

        // Очищает поле путем заполнения(заменяет старые значения новыми)
        public void Clear()
        {
            FillField();
        }

        // Меняет размер поля и пересоздает его
        public void Reset(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            FillField();
        }

        // Добавляет мины на поле
        public void AddMines(int mines, int safeRow, int safeColumn)
        {
            Point[] minesPoses = GenerateMinesPositions(mines, safeRow, safeColumn);
            Mines += minesPoses.Length;
            for (int i = 0; i < minesPoses.Length; i++)
            {
                Field[minesPoses[i].X, minesPoses[i].Y] = -1;
            }
        }

        // Добавляет остальные ячейки на поле, значения которых означают количество мин вокруг них в радиусе 1
        public void AddBlanks()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (Field[i, j] != -1)
                        Field[i, j] = CountNearestMines(i, j);
                }
            }
        }

        // Проверяет является ли ячейка миной
        public bool IsMine(int row, int column)
        {
            return Field[row, column] == -1;
        }

        // Проверяет является ли ячейка пустой или значением 0(отсутсиве мин вокруг)
        public bool IsEmpty(int row, int column)
        {
            return Field[row, column] == 0;
        }

        // Генерирует позиции для мин
        // Генерирует список позиций, случайным образом выбирает позиции в которых будут мины
        private Point[] GenerateMinesPositions(int mines, int safeRow, int safeColumn)
        {
            // Защита от переполнения поля минами
            if (mines + Mines > Rows * Columns)
                mines = Rows * Columns - Mines;

            Point[] poses = new Point[mines];
            if (mines == 0)
                return poses;

            int rndIndex;
            List<Point> allPoses = new List<Point>();
            Point[] safePoints = GetSafePositions(safeRow, safeColumn);

            Random rnd = new Random();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!Point.IsBetweenTwoPoints(safePoints[0], safePoints[1], i, j) && Field[i, j] != -1)
                        allPoses.Add(new Point(i, j));
                }
            }

            // Мешаем все позиции
            for (int i = allPoses.Count - 1; i >= 1; i--)
            {
                int j = rnd.Next(i + 1);
                // обменять значения data[j] и data[i]
                var temp = allPoses[j];
                allPoses[j] = allPoses[i];
                allPoses[i] = temp;
            }
            int length = allPoses.Count - 1;
            for (int i = 0; i < poses.GetLength(0); i++)
            {
                rndIndex = rnd.Next(length - i);
                poses[i].X = allPoses[rndIndex].X;
                poses[i].Y = allPoses[rndIndex].Y;
                allPoses.RemoveAt(rndIndex);
            }
            return poses;
        }

        private Point[] GetSafePositions(int safeRow, int safeColumn)
        {
            Random rnd = new Random();
            Point[] safePoints = new Point[2];
            int highMin = 1, highMax = 3,
                lowMin = 1, lowMax = 3;
            safePoints[0] = new Point(safeRow - rnd.Next(highMin, highMax), safeColumn - rnd.Next(highMin, highMax));
            safePoints[1] = new Point(safeRow + rnd.Next(lowMin, lowMax), safeColumn + rnd.Next(lowMin, lowMax));
            return safePoints;
        }

        /*
        private List<Point> generateSafeStartPositions(int row, int column)
        {
            List<Point> generateSafeStartPositions = new List<Point>();
            int temp_x, temp_y;

            for(int offset_x = -2; offset_x < 3; offset_x++)
                for (int offset_y = 0; offset_y < 3; offset_y++)
                {
                    temp_x = offset_x + row;
                    temp_y = offset_y + column;
                    if ((temp_x >= 0 && temp_x < Rows) &&
                        (temp_y >= 0 && temp_y < Columns))
                        generateSafeStartPositions.Add(new Point(temp_x, temp_y));
                }

            return generateSafeStartPositions;
        }*/

        // Генерирует позиции которые будут открыты в случае нажатия на пустую ячейку(0)
        public List<Point> GetToOpenPositions(int row, int column)
        {
            List<Point> toOpenPoses = new List<Point>();
            List<Point> usedEmptyPoses = new List<Point>();
            List<Point> emptyPoses = new List<Point>();
            List<Point> tempCellsValues = new List<Point>();

            int x, y;
            Point point;

            emptyPoses.Add(new Point(row, column));
            toOpenPoses.Add(emptyPoses[0]);

            while (emptyPoses.Count != 0)
            {
                x = emptyPoses[0].X;
                y = emptyPoses[0].Y;

                usedEmptyPoses.Add(emptyPoses[0]);
                emptyPoses.RemoveAt(0);

                // Собирает позиции вокруг ячейки, учитывая, что она может быть в углах
                // if (y > 0)
                int shiftX;
                for (int i = -1; i < 2; i++)
                    if ((shiftX = x + i) >= 0 && shiftX < Rows)
                    {
                        if (y > 0)
                        {
                            if (!IsMine(shiftX, y - 1))
                                tempCellsValues.Add(new Point(shiftX, y - 1));
                        }
                        if (y < Columns - 1)
                        {
                            if (!IsMine(shiftX, y + 1))
                                tempCellsValues.Add(new Point(shiftX, y + 1));
                        }
                        if (!IsMine(shiftX, y))
                            tempCellsValues.Add(new Point(shiftX, y));
                    }

                //if (y < Columns - 1)
                //    for (int i = -1; i < 2; i++)
                //        if (x + i >= 0 && x + i < Rows)
                //            if (!IsMine(x + i, y + 1))
                //                tempCellsValues.Add(new Point(x + i, y + 1));

                //for (int i = -1; i < 2; i++)
                //    if (x + i >= 0 && x + i < Rows)
                //        if (!IsMine(x + i, y))
                //            tempCellsValues.Add(new Point(x + i, y));

                for (int i = 0; i < tempCellsValues.Count; i++)
                {
                    point = tempCellsValues[i];

                    if (CellsStates[point.X, point.Y] == CellState.Closed)
                    {
                        if (IsEmpty(point.X, point.Y))
                            if (!usedEmptyPoses.Contains(point) && !emptyPoses.Contains(point))
                                emptyPoses.Add(point);

                        if (!toOpenPoses.Contains(point))
                            toOpenPoses.Add(point);
                    }
                }
                tempCellsValues.Clear();
            }
            //toOpenPoses.AddRange(usedEmptyPoses);
            return toOpenPoses;
        }

        // Считает мины вокруг ячейки (сначала слева, потом справа и по центру)
        private int CountNearestMines(int x, int y)
        {
            int nearestMines = 0;

            for (int i = -1; i < 2; i++)

                if (x + i >= 0 && x + i < Rows)
                {
                    if (y > 0)
                    {
                        if (Field[x + i, y - 1] == -1)
                            nearestMines++;
                    }
                    if (y < Columns - 1)
                    {
                        if (Field[x + i, y + 1] == -1)
                            nearestMines++;
                    }
                    if (Field[x + i, y] == -1)
                        nearestMines++;
                }

            return nearestMines;
        }
    }
}