using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace AinashiLibraryCSharp.Algorithm
{
    partial class SegmentTree<ImputType, EditType, OutputType>
    {
        
        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="imputs">初期化に用いる列挙子。</param>
        private void Initializer(IEnumerable<ImputType> imputs)
        {

            // 要素数の設定
            Count = imputs.Count();
            Dims = 1;
            
            // 次元数の設定
            for (int i = 1; i < Count; i <<= 1) 
                ++Dims;

            int lstDim = Dims - 1;

            // 葉と枝の初期化
            TreeArray = new (EditType value, bool isValid)[Dims][];
            for (int i = 0, l = 1 << (lstDim - i); i < Dims; ++i, l >>= 1)
            {

                TreeArray[i] = new (EditType value, bool isValid)[l];

            }

            // 葉の初期化（有効値）
            {
                var array_i = 0;

                foreach (var imput in imputs)
                {

                    TreeArray[0][array_i] = (ImputConverter(imput), true);
                    ++array_i;

                }
            }

            // 葉の初期化（無効値）

            for (int i = Count, l = 1 << (Dims - 1); i < l; ++i)
                TreeArray[0][i] = (DefaultValue, true);

            // 枝の初期化
            for (int i = 1; i < Dims; ++i)
                for (int j = 0, l = 1 << (lstDim - i); j < l; j++)
                    TreeArray[i][j] = (DefaultValue, false);

        }

    }
}
