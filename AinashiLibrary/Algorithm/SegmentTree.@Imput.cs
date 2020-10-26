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
        public void Imput(int index, ImputType value)
        {

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("範囲外の添え字を指定されました。");

            else if (index == Count)
            {

                // TODO: Add Method

            }

            else 
            { 
                var dim_i = 0;

                // 葉へ代入
                TreeArray[dim_i][index].value = ImputConverter(value);

                // 枝へ代入
                for (++dim_i, index >>= 1; dim_i >= 0; ++dim_i, index >>= 1)
                    TreeArray[dim_i][index].isValid = false;

            }
        }

    }
}
