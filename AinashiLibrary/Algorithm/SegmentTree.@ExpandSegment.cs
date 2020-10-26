using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using AinashiLibraryCSharp.Extensions;

namespace AinashiLibraryCSharp.Algorithm
{
    public partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /// <summary>
        /// セグメント木の葉の格納量を2倍にします。
        /// </summary>
        private void ExpandSegment()
        {

            Dims++;
            TreeArray.Add(new List<(EditType value, bool isValid)> { (DefaultValue, false) });

            for (int i = 0, l = TreeArray[0].Count; i < l; ++i)
                TreeArray[0].Add((DefaultValue, true));

            for (int i = 1; i < Dims - 1; i++)
                for (int j = 0, l = TreeArray[i].Count; i < l; ++j)
                    TreeArray[i].Add((DefaultValue, false));

        }

    }
}
