using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace EncryptedCard
{
    public class Card
    {
        public Guid CardId { get; private set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int Rows { get; private set; }
        /// <summary>
        /// 列数
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// 单元格集合，按行展开，即：
        ///row1: 1 2 3
        ///row2: 4 5 6
        /// 展开后为{1,2,3,4,5,6}
        /// </summary>
        [JsonIgnore]
        public List<CardCell> Cells { get; private set; }
        /// <summary>
        /// 序列化值
        /// </summary>
        public string CellData
        {
            get
            {
                return string.Join(',', Cells.Select(i => i.Value));
            }
        }
        public int this[int row, int col]
        {
            get
            {
                return Cells[Columns * row + col].Value;
            }
        }
        public Card(int rows = 6, int columns = 5)
        {
            Rows = rows;
            Columns = columns;
            Cells = new List<CardCell>(columns * rows);
        }
        public Card GenerateData()
        {
            var arr = GenerateRandomMatrix(Rows, Columns);
            FillCellData(arr);
            return this;
        }
        /// <summary>
        /// 使用指定的数组填充单元格
        /// </summary>
        /// <param name="array"></param>
        public void FillCellData(int[,] array)
        {
            Cells.Clear();
            if (array.GetLength(0) < Rows || array.GetLength(1) < Columns)
            {
                throw new ArgumentException($"{nameof(array)}长度小于密保卡要求行数或列数");
            }
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = new CardCell(row, col, array[row, col]);
                    Cells.Add(cell);
                }
            }
        }
        public Card LoadCellData(string strMatrix)
        {
            var arrstr = strMatrix.Split(',');
            if (arrstr.Length < Columns * Rows)
            {
                throw new ArgumentException($"{strMatrix}长度小于卡片需要行列数据长度");
            }
            var arr = new int[Rows, Columns];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (int.TryParse(arrstr[Columns * row + col], out int value))
                    {
                        arr[row, col] = value;
                    }
                    else
                    {
                        throw new ArgumentException($"{ strMatrix }内容必须为\",\"分割数字");
                    }
                }
            }
            this.FillCellData(arr);
            return this;
        }
        /// <summary>
        /// 随机选择单元格
        /// </summary>
        /// <param name="howMany"></param>
        /// <returns></returns>
        public IEnumerable<CardCell> PickRandomCells(int howMany)
        {
            var r = new Random();
            for (int i = 0; i < howMany; i++)
            {
                var randomCol = r.Next(0, Columns);
                var randomRow = r.Next(0, Rows);
                var c = new CardCell(randomRow, randomCol);
                yield return c;
            }
        }
        public bool Validate(IEnumerable<CardCell> cellsToValidate)
        {
            return (from cell in cellsToValidate
                    let thisCell = Cells.Find(p => p.ColIndex == cell.ColIndex && p.RowIndex == cell.RowIndex)
                    select thisCell.Value == cell.Value
                    ).All(matches => matches);
        }
        /// <summary>
        /// 生成数字数组
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="colums"></param>
        /// <returns></returns>
        private static int[,] GenerateRandomMatrix(int rows, int colums)
        {
            var random = new Random();
            var arr = new int[rows, colums];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    arr[i, j] = random.Next(0, 1000);
                }
            }
            return arr;
        }

    }
}
