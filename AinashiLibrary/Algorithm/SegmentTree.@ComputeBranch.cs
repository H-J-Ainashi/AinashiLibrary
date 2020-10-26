using System;
using System.Collections.Generic;
using System.Text;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// セグメント木の枝の計算値を取得します。最大Ο(log<see cref="Dims"/>)の計算量を使用します。
        /// </summary>
        /// <param name="dimension"><see cref="TreeArray"/>の次元数。第一添え字として使用する。</param>
        /// <param name="index"><see cref="TreeArray"/>のインデックス。第二添え字として使用する。</param>
        /// <returns></returns>
        private EditType ComputeBranch(int dimension, int index)
        {

            if (!TreeArray[dimension][index].isValid)
            {

                TreeArray[dimension][index] = 
                    (
                    Selector(
                        ComputeBranch(dimension - 1, (index << 1)), 
                        ComputeBranch(dimension - 1, (index << 1) + 1)), 
                    true);

            }
            return TreeArray[dimension][index].value;

        }

    }
}
