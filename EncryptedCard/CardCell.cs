using System;
using System.Collections.Generic;
using System.Text;

namespace EncryptedCard
{
    public struct CardCell
    {
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int RowIndex { get; }
        /// <summary>
        /// 列号
        /// </summary>
        public int ColIndex { get; }
        /// <summary>
        /// 列名
        /// </summary>
        public ColumnCode ColumnName => (ColumnCode)ColIndex;

        public CardCell(int rowIndex, int colIndex, int value = 0)
        {
            this.RowIndex = rowIndex;
            this.ColIndex = colIndex;
            this.Value = value;
        }

        /// <summary>
        /// 列明转换字母
        /// </summary>
        public enum ColumnCode
        {
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            E = 4,
            F = 5,
            G = 6,
            H = 7,
            I = 8,
            J = 9,
            K = 10,
            L = 11,
            M = 12,
            N = 13,
            O = 14,
            P = 15,
            Q = 16,
            R = 17,
            S = 18,
            T = 19,
            U = 20,
            V = 21,
            W = 22,
            X = 23,
            Y = 24,
            Z = 25,
        }
    }
}
