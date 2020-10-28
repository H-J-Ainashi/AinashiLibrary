using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// 閉区間[<paramref name="minIndex"/>, <paramref name="maxIndex"/>]における計算結果を取得します。
        /// </summary>
        /// <param name="minIndex">計算結果を出力するために使用する配列の開始インデックス。</param>
        /// <param name="maxIndex">計算結果を出力するために使用する配列の終了インデックス。</param>
        /// <returns></returns>
        public virtual OutputType Output(int minIndex, int maxIndex)
            => (minIndex == maxIndex ? OutputConverter(TreeArray[0][minIndex].value) : OutputConverter(SolveWithOutput(minIndex, maxIndex, Dims - 1, 0)));

        /// <summary>
        /// 指定した枝と閉区間[<paramref name="minIndex"/>, <paramref name="maxIndex"/>]の共通部分における計算結果を取得します。
        /// </summary>
        /// <param name="minIndex">計算結果を出力するために使用する配列の開始インデックス。</param>
        /// <param name="maxIndex">計算結果を出力するために使用する配列の終了インデックス。</param>
        /// <param name="dimension">セグメント木の次元。</param>
        /// <param name="index">セグメント木のインデックス。</param>
        /// <returns></returns>
        private EditType SolveWithOutput(int minIndex, int maxIndex, int dimension, int index)
        {

            var span = GetSpan(dimension, index);
            if (span.max < minIndex || maxIndex < span.min)
                return DefaultValue;
            else if (minIndex <= span.min && span.max <= maxIndex)
                return ComputeBranch(dimension, index);
            else
                return Selector(
                    SolveWithOutput(minIndex, maxIndex, dimension - 1, index << 1),
                    SolveWithOutput(minIndex, maxIndex, dimension - 1, (index << 1) + 1));
        }

        /// <summary>
        /// 指定した枝が含んでいる計算結果の範囲を取得します。
        /// </summary>
        /// <param name="dimension">セグメント木の次元。</param>
        /// <param name="index">セグメント木のインデックス。</param>
        /// <returns></returns>
        private (int min, int max) GetSpan(int dimension, int index)
            => (index << dimension, ((index + 1) << dimension) - 1);

    }
}
