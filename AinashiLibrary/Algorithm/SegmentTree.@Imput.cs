using System;
using System.Collections.Generic;
using System.Text;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// セグメント木にデータを配置します。
        /// </summary>
        /// <param name="index">代入先の0から始まるインデックス。</param>
        /// <param name="value">代入する値。</param>
        public virtual void Imput(int index, ImputType value)
        {

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("範囲外の添え字を指定されました。");

            else if (index == Count)
                AddLeaf(value);

            else
                SolveWithImput(index, value);
        }

        private void SolveWithImput(int index, ImputType value)
        {

            var dim_i = 0;

            // 葉へ代入
            TreeArray[dim_i][index] = (ImputConverter(value), true);

            // 枝へ代入
            for (++dim_i, index >>= 1; dim_i < Dims; ++dim_i, index >>= 1)
                if (TreeArray[dim_i][index].isValid) TreeArray[dim_i][index] = (TreeArray[dim_i][index].value, false);
                else return;


        }

    }
}
