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
        /// 指定した要素からセグメント木を作成します。
        /// </summary>
        /// <param name="defaultValue">データが無効な場合に用いる値を指定します。</param>
        /// <param name="imputConv"><paramref name="imputs"/>などから実際にセグメント木の葉として使用する値を取得するための変換メソッドを指定します。</param>
        /// <param name="selector">下位のセグメント木の葉か上位の葉を生成するために使用する変換メソッドを指定します。</param>
        /// <param name="outputConv">セグメント木から値を取得する際に使用する変換メソッドを指定します。</param>
        /// <param name="imputs">セグメント木の初期化に使用する列挙子。<see cref="null"/>が指定された場合は、空の配列を要素として格納します。</param>
        public SegmentTree(
            EditType defaultValue,
            Func<ImputType, EditType> imputConv,
            Func<EditType, EditType, EditType> selector,
            Func<EditType, OutputType> outputConv,
            IEnumerable<ImputType>? imputs = null)
        {

            this.ImputConverter = imputConv;
            this.Selector = selector;
            this.OutputConverter = outputConv;
            this.ReverseImputConverter = null;
            this.ReverseOutputConverter = null;
            this.DefaultValue = defaultValue;

            this.TreeArray = new List<List<(EditType value, bool isValid)>>{ };

            // セグメント木の初期化
            Initializer(imputs is null ? new ImputType[] { } : imputs);

        }

        /// <summary>
        /// 指定した要素からセグメント木を作成します。
        /// </summary>
        /// <param name="defaultValue">データが無効な場合に用いる値を指定します。</param>
        /// <param name="imputConv"><paramref name="imputConv"/>などから実際にセグメント木の葉として使用する値を取得するための変換メソッドを指定します。</param>
        /// <param name="selector">下位のセグメント木の葉・枝から上位の枝を生成するために使用する変換メソッドを指定します。</param>
        /// <param name="outputConv">セグメント木から値を取得する際に使用する変換メソッドを指定します。</param>
        /// <param name="imputs">セグメント木の初期化に使用する列挙子。<see cref="null"/>が指定された場合は、空の配列を要素として格納します。</param>
        public SegmentTree(
            ImputType defaultValue,
            Func<ImputType, EditType> imputConv,
            Func<EditType, EditType, EditType> selector,
            Func<EditType, OutputType> outputConv,
            IEnumerable<ImputType>? imputs = null)
        {

            this.ImputConverter = imputConv;
            this.Selector = selector;
            this.OutputConverter = outputConv;
            this.ReverseImputConverter = null;
            this.ReverseOutputConverter = null;
            this.DefaultValue = imputConv(defaultValue);

            this.TreeArray = new List<List<(EditType value, bool isValid)>> { };

            // セグメント木の初期化
            Initializer(imputs is null ? new ImputType[] { } : imputs);

        }

    }
}
