using System;
using System.Collections.Generic;
using System.Text;

namespace AinashiLibraryCSharp.Exception
{

    /// <summary>
    /// 想定外の例外が発生した場合に代理で出力する例外。
    /// </summary>
    public class UnknownException : System.Exception
    {

        /// <summary>
        /// 発生した例外を格納します。メッセージには想定外の例外が発生した旨が記載されます。
        /// </summary>
        /// <param name="exception">発生した想定外の例外。</param>
        public UnknownException(System.Exception exception):
            base("想定外の例外が生成されました。", exception)
        { }

    }
}
