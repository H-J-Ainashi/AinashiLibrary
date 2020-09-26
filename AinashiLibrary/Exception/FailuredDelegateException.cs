using System;
using System.Collections.Generic;
using System.Text;

namespace AinashiLibraryCSharp.Exception
{
    public class FailuredDelegateException : System.Exception
    {

        /// <summary>
        /// デリゲート内部で発生した例外を格納します。メッセージにはデリゲート内部で例外が発生した旨が記載されます。
        /// </summary>
        /// <param name="exception">デリゲート内部で発生した例外。</param>
        public FailuredDelegateException(System.Exception exception):
            base("デリゲート内部で例外が発生しました。", exception)
        {}

        /// <summary>
        /// デリゲート内部で発生した例外を格納します。
        /// </summary>
        /// <param name="message">メッセージ。考えられる発生した原因等を記載してください。</param>
        /// <param name="exception">デリゲート内部で発生した例外。</param>
        public FailuredDelegateException(string message, System.Exception exception):
            base("デリゲート内部で例外が発生しました：" + message, exception)
        {}

    }
}