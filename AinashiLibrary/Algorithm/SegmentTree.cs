using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace AinashiLibraryCSharp.Algorithm
{

    /// <summary>
    /// セグメント木を生成するオブジェクトです。
    /// </summary>
    /// <typeparam name="ImputType">入力時に用いる変数型です。</typeparam>
    /// <typeparam name="EditType">セグメント記述時に用いる変数型です。</typeparam>
    /// <typeparam name="OutputType">出力時に用いる変数型です。</typeparam>
    partial class SegmentTree<ImputType, EditType, OutputType>
    {

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Fields and Simple Propaties
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Fields and Simple Propaties

        /// <summary>
        /// セグメント木に格納する際に使用する変換メソッド
        /// </summary>
        readonly private Func<ImputType, EditType> ImputConverter;

        /// <summary>
        /// 各セグメント木動詞を比較して上位に出力するメソッド
        /// </summary>
        readonly private Func<EditType, EditType, EditType> Selector;

        /// <summary>
        /// セグメント木から値を取得する際に使用する変換メソッド
        /// </summary>
        readonly private Func<EditType, OutputType> OutputConverter;

        /// <summary>
        /// セグメント木から入力変数型として出力する際に使用する変換メソッド
        /// </summary>
        readonly private Func<EditType, ImputType>? ReverseImputConverter;

        /// <summary>
        /// 出力変数型から入力変数型として使用する際に使用する変換メソッド
        /// </summary>
        readonly private Func<OutputType, EditType>? ReverseOutputConverter;

        /// <summary>
        /// セグメント木が実際に格納されている配列。
        /// </summary>
        private List<List<(EditType value, bool isValid)>> TreeArray;

        /// <summary>
        /// セグメント木上で値が含まれていない最下位の葉に対して割り当てる規定値。この値にはセグメント比較時に無視する、または無視できる値とすることが好ましいです。
        /// </summary>
        readonly public EditType DefaultValue;

        /// <summary>
        /// セグメント木の要素数を取得します。この値は規定値をその数から除外し、Ο(1)での取得を行います。
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// セグメント木の次元数を取得します。
        /// </summary>
        private int Dims;

        #endregion


    }
}
