using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using AinashiLibraryCSharp.Extensions;

namespace AinashiLibraryCSharp.ByteCodeMap
{

    /// <summary>
    /// バイトコードマップの各ノードを包括的に管理するクラス用のインターフェース。
    /// </summary>
    public interface ICodeMapModule<NodeType> : ICodeMapModule
        where NodeType: INodeViewer
    {

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Inherit from ICodeMapModule(non-generics)
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Inherit from ICodeMapModule(non-generics)

        ReadOnlyCollection<INodeViewer> ICodeMapModule.RootNodes
        {
            get
            {

                var src = RootNodes;
                var ans = new INodeViewer[src.Count];

                src.CopyTo(ans, src.Count);

                return Array.AsReadOnly(ans);

            }
        }

        #endregion

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Infomation Property
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Infomation Property

        /// <summary>
        /// このモジュールが管理している最上位ノードの一覧を取得します。編集可能かどうかは<typeparams cref="NodeType"/>によって変化します。
        /// </summary>
        new ReadOnlyCollection<NodeType> RootNodes { get; }

        #endregion

    }

    /// <summary>
    /// <see cref="ICodeMapModule{NodeType}"/>の共通実装。
    /// </summary>
    public interface ICodeMapModule
    {


        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Infomation Property
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Infomation Property

        /// <summary>
        /// 読み取り専用かどうかを取得します。
        /// </summary>
        bool isReadOnly { get; }

        /// <summary>
        /// このモジュールが管理している最上位ノードの一覧を取得します。編集することはできません。
        /// </summary>
        ReadOnlyCollection<INodeViewer> RootNodes { get; }

        #endregion


    }
    
    /// <summary>
    /// <see cref="ICodeMapModule{NodeType}"/>及び<see cref="ICodeMapModule"/>の拡張メソッド群。
    /// </summary>
    public static class ICodeMapModuleExtensions
    {
        
        /// <summary>
        /// <see cref="ICodeMapModule"/>からデータをバイトデータとして取得します。
        /// </summary>
        /// <param name="module">データの取得元である<see cref="ICodeMapModule"/>。</param>
        /// <returns></returns>
        public static byte[] OutputByteData(this ICodeMapModule module)
        {
            
            using MemoryStream memory = new MemoryStream();

            module.WriteTo(memory);

            return memory.ToArray();
            
        }
        
        /// <summary>
        /// ストリームに<paramref name="module"/>から取得したバイトデータを現在位置から書き込み、書き込んだ分だけポジションを移動させます。
        /// </summary>
        /// <param name="module">データの取得元である<see cref="ICodeMapModule"/>。</param>
        /// <param name="stream">データの書き込み先であるストリーム。</param>
        public static void WriteTo(this ICodeMapModule module, Stream stream)
        {

            var rootnodes = module.RootNodes;

            foreach (var node in rootnodes)
                node.WriteToStream(stream);

        }
        
    }

}
