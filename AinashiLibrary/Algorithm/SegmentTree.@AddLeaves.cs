using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// セグメント木の葉を追加します。
        /// </summary>
        /// <param name="collection">セグメント木に追加する葉を含む列挙子。</param>
        public void AddLeaves(IEnumerable<ImputType> collection)
        {

            while (TreeArray[0].Count < Count + collection.Count())
                ExpandSegment();

            foreach (var item in collection)
                SolveWithImput(Count++, item);

        }
    }
}
