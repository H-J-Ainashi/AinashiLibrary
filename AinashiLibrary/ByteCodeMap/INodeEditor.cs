using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using AinashiLibraryCSharp.Exception;

#nullable enable

namespace AinashiLibraryCSharp.ByteCodeMap
{

    /// <summary>
    ///     バイトコードマップ上のノードを編集するためのインターフェースです。<para/>
    ///         TODO（for Implementer）:デバイスレベルでのアクセス問題が発生した場合は<see cref="IOException"/>をその理由をメッセージに残したうえで返してください。（全メソッド共通）
    /// </summary>
    public interface INodeEditor : INodeViewer
    {

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         *
         * Inherit from INodeViewer
         *
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Inherit from INodeViewer

        int                             INodeViewer.Marker              => Marker;
        ReadOnlyCollection<byte>        INodeViewer.ByteCodeArray       => Array.AsReadOnly(ByteCodeArray);
        ReadOnlyCollection<INodeViewer> INodeViewer.CollectChildren     => Array.AsReadOnly((INodeViewer[])CollectChildren);
        INodeViewer                     INodeViewer.this[in int index]  => this[index];
        ReadOnlyCollection<INodeViewer> INodeViewer.PickChildren(out ReadOnlyCollection<int> resultIdx, Func<INodeViewer, int, bool> predicate)
        {

            ReadOnlyCollection<INodeViewer> ans;

            try
            {

                ans         = ConvertPickChildren(
                    /* resultIdx = */			out var Idx,
                    /* marker = */			new int[0],
                    /* annotation maker = */	(node, idx) => new { node = (INodeViewer)node, idx },
                    /* predicate = */			x => predicate(x.node, x.idx),
                    /* converter = */			(node, annotation, idx) => annotation.node)
                    .AsReadOnly();

                resultIdx   = Idx.AsReadOnly();

            }
            catch (IOException) { throw; }
            catch (FormatException) { throw; }
            catch (FailuredDelegateException) { throw; }

            return ans;

        }
        List<DeserializeResult> INodeViewer.ConvertPickChildren<DeserializeResult>(out List<int> resultIdx,
            int[] markers,
            Func<
                INodeViewer         /* node */,
                int                 /* index */,
                DeserializeResult   /* returns(parsed data) */>
                                                            deserializer,
            Predicate<
                DeserializeResult   /* parsed data */>
                                                            predicate)
            => ConvertPickChildren(out resultIdx, markers, deserializer, predicate);
        List<ResultDeserializeType> INodeViewer.ConvertPickChildren<PredicateDeserializeType, ResultDeserializeType>(
            out List<int> resultIdx,
            int[] markers,
            Func<
                INodeViewer                 /* node */,
                int                         /* index */,
                PredicateDeserializeType    /* returns(temporary parsed data) */>
                                                                                deserializerForPredicate,

            Predicate<
                PredicateDeserializeType    /* parsed data */>
                                                                                predicate,

            Func<
                INodeViewer                 /* node */,
                PredicateDeserializeType    /* part */,
                int                         /* index */,
                ResultDeserializeType       /* returns(finally parsed data) */>
                                                                                deserializerForResult)
            => ConvertPickChildren(out resultIdx, markers, deserializerForPredicate, predicate, deserializerForResult);

        #endregion

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         *
         * View And Edit Infomation (Overwriting)
         *
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region View And Edit Infomation (Overwriting)

        /// <summary>
        ///     ノードに割り当てられたマーカー値を取得・設定します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        new int             Marker          { get; set; }

        /// <summary>
        ///		ノードに割り当てられたバイトコードを取得・設定します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        new byte[]          ByteCodeArray   { get; set; }

        /// <summary>
        /// ノード内部の子要素の一覧を取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        new INodeEditor[]   CollectChildren { get; }

        /// <summary>
        ///		ノード内部の指定した番号の子要素を取得・設定します。<para/>
        ///		
        ///			TODO（for Implementer）: 引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="index"/>値について、取得では0以上<see cref="INodeViewer.CountChildren"/>未満であるのに対し、
        ///			設定では0以上<see cref="INodeViewer.CountChildren"/>以下となるように設定し、
        ///			<paramref name="index"/> == <see cref="INodeViewer.CountChildren"/>である場合には<see cref="AddAsChildClone(INodeViewer)"/>と同一の動作を行ってください。
        /// </summary>
        /// 
        /// <param name="index">
        ///		子要素に割り当てられた0から始まるインデックス番号。</param>
        /// 
        /// <returns>
        ///		指定したインデックスの子要素が編集機能付きで示されます。</returns>
        /// 
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        new INodeEditor     this[in int index] { get; set; }

        /// <summary>
        ///     指定した条件の子要素を配列として取得します。<para/>
        ///     
        ///         TODO (for Implementer) :<paramref name="predicate"/>内で例外が発生した場合、<see cref="FailuredDelegateException"/>を返却してください。
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     取得するノードであるかどうかを確かめる条件式。
        ///     第一引数にノード、第二引数に0から始まるインデックス番号が割り当てられるため、<see cref="bool"/>値で削除するかどうかを示してください。</param>
        /// <param name="resultIdx">
        ///     戻り値として得られる配列のこのノードにおけるインデックスの出力パラメータ。</param>
        /// 
        /// <returns>
        ///     指定した条件の子要素を読み取り専用のノード配列として示します。</returns>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="FailuredDelegateException">
        ///     <paramref name="predicate"/>内部で例外が発生しました。</exception>
        new ReadOnlyCollection<INodeEditor> PickChildren(out ReadOnlyCollection<int> resultIdx, Func<INodeViewer, int, bool> predicate)
        {

            var ans = new List<INodeEditor>();
            var ansIdx = new List<int>();
            var children = CollectChildren;

            try
            {

                for (int i = 0, l = children.Length; i < l; i++)
                    if (predicate(children[i], i))
                    {

                        ans.Add(children[i]);
                        ansIdx.Add(i);

                    }

                resultIdx = ansIdx.AsReadOnly();
                return ans.AsReadOnly();

            }
            catch (IOException) { throw; }
            catch (FailuredDelegateException) { throw; }

        }


        #endregion


        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         *
         * Edit Infomation (Original)
         *
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Edit Infomation (Original)

        /// <summary>
        ///		<paramref name="node"/>をクローンコピーし、子要素の最後尾に追加します。
        /// </summary>
        /// 
        /// <param name="node">
        ///		追加するノード。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        void AddAsChildClone(INodeViewer node);

        /// <summary>
        ///		<paramref name="nodes"/>をクローンコピーし、子要素の最後尾に追加します。<paramref name="nodes"/>で指定した順に追加されます。
        /// </summary>
        /// 
        /// <param name="nodes">
        ///		追加するノードの配列。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        void            AddAsChildrenClone(INodeViewer[] nodes);

        /// <summary>
        ///		<paramref name="nodes"/>をクローンコピーし、子要素の最後尾に追加します。
        /// </summary>
        /// 
        /// <param name="nodes">
        ///		追加するノードのコレクション。</param>
        /// 
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        void            AddAsChildrenClone(IEnumerable<INodeViewer> nodes);

        /// <summary>
        ///		<paramref name="node"/>をクローンコピーし、子要素の<paramref name="assignIndex"/>で指定した位置に追加します。<para/>
        ///		
        ///			TODO（for Implementer）:引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="assignIndex"/>値について、0以上<see cref="INodeViewer.CountChildren"/>以下となるように設定し、
        ///			<paramref name="assignIndex"/> == <see cref="INodeViewer.CountChildren"/>である場合には<see cref="AddAsChildClone(INodeViewer)"/>と同一の動作を行ってください。
        /// </summary>
        /// 
        /// <param name="node">
        ///		追加するノード。</param>
        /// <param name="assignIndex">
        ///		追加する位置の0から始まるインデックス番号。元々<paramref name="assignIndex"/>とそれ以降の番号が割り当てられていたノードは追加分だけ後方に移動します。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///		想定されていないインデックス番号が引数として渡されました。</exception>
        void            InsertAsChildClone(INodeViewer node, int assignIndex);

        /// <summary>
        ///		<paramref name="nodes"/>をクローンコピーし、子要素の<paramref name="assignIndex"/>で指定した位置に追加します。<paramref name="nodes"/>で指定した順に追加されます。<para/>
        ///			TODO (for Implementer) :引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="assignIndex"/>値について、0以上<see cref="INodeViewer.CountChildren"/>以下となるように設定し、
        ///			<paramref name="assignIndex"/> == <see cref="INodeViewer.CountChildren"/>である場合には<see cref="AddAsChildrenClone(INodeViewer[])"/>と同一の動作を行ってください。
        /// </summary>
        /// 
        /// <param name="nodes">
        ///		追加するノードの配列。</param>
        /// <param name="assignIndex">
        ///		追加する位置の0から始まるインデックス番号。
        ///		元々<paramref name="assignIndex"/>とそれ以降の番号が割り当てられていたノードは追加分だけ後方に移動します。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///		想定されていないインデックス番号が引数として渡されました。</exception>
        void            InsertAsChildrenClone(INodeViewer[] nodes, int assignIndex);

        /// <summary>
        ///		<paramref name="nodes"/>をクローンコピーし、子要素の<paramref name="assignIndex"/>で指定した位置に追加します。<para/>
        ///			TODO (for Implementer) :引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="assignIndex"/>値について、0以上<see cref="INodeViewer.CountChildren"/>以下となるように設定し、
        ///			<paramref name="assignIndex"/> == <see cref="INodeViewer.CountChildren"/>である場合には<see cref="AddAsChildrenClone(IEnumerable{INodeViewer})"/>と同一の動作を行ってください。		
        /// </summary>
        /// 
        /// <param name="nodes">
        ///		追加するノードのコレクション。</param>
        /// <param name="assignIndex">
        ///		追加する位置の0から始まるインデックス番号。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///		想定されていないインデックス番号が引数として渡されました。</exception>
        void            InsertAsChildrenClone(IEnumerable<INodeViewer> nodes, int assignIndex);

        /// <summary>
        ///		<paramref name="removeIndex"/>に割り当てられている子要素を削除します。<para/>
        ///			TODO (for Implementer) :引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="removeMinIdx"/>及び<paramref name="removeMaxIdx"/>値について、0以上<see cref="INodeViewer.CountChildren"/>未満となるように設定してください。
        /// </summary>
        /// 
        /// <param name="removeIndex">
        ///		削除するノードに割り当てられている0から始まるインデックス番号。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///		想定されていないインデックス番号が引数として渡されました。</exception>
        void            RemoveChildAt(int removeIndex);

        /// <summary>
        ///		<paramref name="removeMinIdx"/>から<paramref name="removeMaxIdx"/>までのインデックスが割り当てられている子要素を削除します。<para/>
        ///			TODO (for Implementer) :引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///			例外とならない<paramref name="removeMinIdx"/>及び<paramref name="removeMaxIdx"/>値について、0以上<see cref="INodeViewer.CountChildren"/>未満となるように設定してください。
        /// </summary>
        /// 
        /// <param name="removeMinIdx">
        ///		削除するノードの0から始まる開始インデックス。</param>
        /// <param name="removeMaxIdx">
        ///		削除するノードの0から始まる終了インデックス。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///		想定されていないインデックス番号が引数として渡されました。</exception>
        void            RemoveChildrenRange(int removeMinIdx, int removeMaxIdx);

        /// <summary>
        ///		指定した条件の子要素を削除します。
        ///			TODO (for Implementer) :<paramref name="predicate"/>内で例外が発生した場合、<see cref="FailuredDelegateException"/>を返却してください。
        /// </summary>
        /// 
        /// <param name="predicate">
        ///		削除するノードであるかどうかを確かめる条件式。
        ///		第一引数にノード、第二引数に0から始まるインデックス番号が割り当てられるため、<see cref="bool"/>値で削除するかどうかを示してください。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="FailuredDelegateException">
        ///		<paramref name="predicate"/>内部で例外が発生しました。</exception>
        void            RemoveChildrenConditional(Func<INodeViewer, int, bool> predicate);

        /// <summary>
        ///		全ての子要素を削除します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        void            RemoveAll();

        /// <summary>
        ///		現在持っている子要素をすべて破棄し、<paramref name="nodes"/>のクローンを子要素として保持します。
        /// </summary>
        /// <param name="nodes">
        ///		新しく保持するノード。</param>
        ///		
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        void            ReplaceChildrenCloneArray(IEnumerable<INodeViewer> nodes);

        #endregion

    }
}
