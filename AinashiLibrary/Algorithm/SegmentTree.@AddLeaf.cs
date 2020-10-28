using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// セグメント木の葉を一枚追加します。
        /// </summary>
        /// <param name="value">セグメント木に追加する葉。</param>
        public void AddLeaf(ImputType value)
        {

            if (TreeArray[0].Count == Count)
                ExpandSegment();

            SolveWithImput(Count++, value);

        }

    }
}
