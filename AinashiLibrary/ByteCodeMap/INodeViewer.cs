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
    ///     バイトコードマップ上のノードを取得するためのインターフェースです。現在、ノード系インターフェースの中で最上位の位置を占めています。<para/>
    ///     
    ///         TODO（for Implementer）:デバイスレベルでのアクセス問題が発生した場合は<see cref="IOException"/>をその理由をメッセージに残したうえで返してください。（全メソッド共通）
    /// </summary>
    public interface INodeViewer
    {

        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         *
         * View Infomation
         *
         \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region View Infomation

        /// <summary>
        ///     ノードに割り当てられたマーカー値を取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        int                             Marker                  { get; }

        /// <summary>
        ///     ノードに割り当てられたバイトコードを取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        ReadOnlyCollection<byte>        ByteCodeArray           { get; }

        /// <summary>
        ///     バイトコードの長さを取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        long                            ByteCodeLength => ByteCodeArray.Count();

        /// <summary>
        ///     ノード内部の子要素数を取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        int                             CountChildren           { get; }

        /// <summary>
        ///     このノードとその下位にあるノードすべての全体長を取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        long                            ComputeFullByteLength   { get; }

        /// <summary>
        ///     ノード内部の子要素の一覧を取得します。
        /// </summary>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        ReadOnlyCollection<INodeViewer> CollectChildren                { get; }

        /// <summary>
        ///     ノード内部の指定した番号の子要素を取得します。<para></para>
        ///     
        ///         TODO (for Implementer) : 引数に予想外の値が含まれる場合、<see cref="IndexOutOfRangeException"/>を返却してください。<para/>
        ///         例外とならない<paramref name="num"/>値について、取得では0以上<see cref="INodeViewer.CountChildren"/>未満としてください。
        /// </summary>
        /// 
        /// <param name="num">
        ///     子要素に割り当てられた0から始まるインデックス番号。</param>
        ///     
        /// <returns>
        ///     指定したインデックス番号と紐づけされている子要素を読み取り専用のノードとして示します。</returns>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="IndexOutOfRangeException">
        ///     想定されていないインデックス番号が引数として渡されました。</exception>
        INodeViewer                     this[in int num]        { get; }

        /// <summary>
        ///		ノードを引数で指定したストリームに出力します。子孫要素も出力されます。
        /// </summary>
        /// 
        /// <param name="destination">
        ///		出力先のストリームです。現在の位置から書き込みます。</param>
        ///	
        /// <exception cref="IOException">
        ///		アクセスエラーが発生しました。</exception>
        /// <exception cref="NotSupportedException">
        ///		<paramref name="destination"/>の書き込み許可がありません。</exception>
        void							WriteToStream	(System.IO.Stream destination)
        {

            if (!destination.CanWrite)
                throw new NotSupportedException("引数として渡されたストリームは書き込みを行うことが出来ません。");

            var children = CollectChildren;

            var marker = BitConverter.GetBytes(Marker);
            var length = BitConverter.GetBytes(ByteCodeLength);
            var count = BitConverter.GetBytes(children.Count);

            if (!BitConverter.IsLittleEndian)
            {

                Array.Reverse(marker);
                Array.Reverse(length);
                Array.Reverse(count);

            }

            destination.Write(marker);
            destination.Write(length);
            destination.Write(count);

            for (int i = 0, l = children.Count; i < l; ++i)
                children[i].WriteToStream(destination);

            destination.Write(marker);

        }

        #endregion


        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         *
         * Selector
         *
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Selector

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
        ReadOnlyCollection<INodeViewer> PickChildren(out ReadOnlyCollection<int> resultIdx, Func<INodeViewer, int, bool> predicate)
        {

            var ans         = new List<INodeViewer>();
            var ansIdx      = new List<int>();
            var children    = CollectChildren;

            try
            {

                for (int i = 0, l = children.Count; i < l; i++)
                    if (predicate(children[i], i))
                    {

                        ans     .Add(children[i]);
                        ansIdx  .Add(i);

                    }

                resultIdx = ansIdx.AsReadOnly();
                return ans.AsReadOnly();

            }
            catch (IOException) { throw; }
            catch (FailuredDelegateException) { throw; }

        }

        /// <summary>
        ///     指定したマーカー値を持つ子要素を配列として取得します。<para/>
        ///     
        ///         TIPS (for Implementer) :このメソッドは継承するべきではありません。
        ///         本メソッド内部で呼び出している<see cref="PickChildren(out ReadOnlyCollection{int}, Func{INodeViewer, int, bool})"/>を継承するべきです。
        /// </summary>
        /// 
        /// <param name="resultIdx">
        ///     戻り値として得られる配列のこのノードにおけるインデックスの出力パラメータ。</param>
        /// <param name="marker">
        ///     検索するマーカー情報。</param>
        /// 
        /// <returns>
        ///     指定した条件の子要素を読み取り専用のノード配列として示します。</returns>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="UnknownException">
        ///     原因不明のエラーが発生しました。</exception>
        ReadOnlyCollection<INodeViewer> PickChildren(out ReadOnlyCollection<int> resultIdx, int marker)
        {

            try
            {

                return PickChildren(out resultIdx, (node, idx) => node.Marker == marker);

            }
            catch(IOException e)
            {

                throw e;

            }
            catch(FailuredDelegateException e)
            {

                if (e.InnerException is IOException)
                    throw e.InnerException;

                else
                    throw new UnknownException(e);

            }

        }

        /// <summary>
        ///     指定した複数のマーカー値のいずれかを持つ子要素を配列として取得します。<para/>
        ///     
        ///         TIPS (for Implementer) :このメソッドは継承するべきではありません。
        ///         本メソッド内部で呼び出している<see cref="PickChildren(out ReadOnlyCollection{int}, Func{INodeViewer, int, bool})"/>を継承するべきです。
        /// </summary>
        ///
        /// <param name="resultIdx">
        ///     戻り値として得られる配列のこのノードにおけるインデックスの出力パラメータ。</param>
        /// <param name="markers">
        ///     検索するマーカー情報。</param>
        ///
        /// <returns>
        ///     指定した条件の子要素を読み取り専用のノード配列として示します。</returns>
        /// 
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="UnknownException">
        ///     原因不明のエラーが発生しました。</exception>
        ReadOnlyCollection<INodeViewer> PickChildren(out ReadOnlyCollection<int> resultIdx, IEnumerable<int> markers)
        {

            try
            {

                return PickChildren(out resultIdx, (node, idx) => markers.Any(x => node.Marker == x));

            }
            catch(IOException e)
            {

                throw e;

            }
            catch (FailuredDelegateException e)
            {

                if (e.InnerException is IOException)
                    throw e.InnerException;

                else
                    throw new UnknownException(e);

            }
        }

        /// <summary>
        ///     指定したマーカー値とデシリアライザを用いた<paramref name="predicate"/>に沿った条件を満たす子要素を<typeparamref name="DeserializeResult"/>の配列として取得します。<para/>
        ///     
        ///         TODO (for Implementer) :<paramref name="deserializer"/>で返される例外は
        ///         <see cref="IOException"/>又は<see cref="FormatException"/>, <see cref="FailuredDelegateException"/>とし、
        ///         <paramref name="predicate"/>で返される例外は<see cref="IOException"/>又は<see cref="FailuredDelegateException"/>としてください。
        /// </summary>
        /// 
        /// <typeparam name="DeserializeResult">
        ///     戻り値として示すデシリアライズ結果。</typeparam>
        /// 
        /// <param name="resultIdx">
        ///     戻り値として得られる配列のこのノードにおけるインデックスの出力パラメータ。</param>
        /// <param name="markers">
        ///     検索するマーカー情報。長さ0の配列であった場合は全ての子要素について検査を行います。</param>
        /// <param name="deserializer">
        ///     <see cref="INodeViewer"/>から<typeparamref name="DeserializeResult"/>へ変換するためのデシリアライザ。
        ///     第一引数に子要素、第二引数に子要素に割り当てられている0から始まるインデックス番号が渡されます。</param>
        /// <param name="predicate">
        ///     取得するかどうかを指定する条件式。</param>
        /// 
        /// <returns>
        ///     <typeparamref name="DeserializeResult"/>として変換を行った子ノードが示されます。
        ///     <typeparamref name="DeserializeResult"/>内のメンバーを変更したとしても、子ノードが影響を受けることはありません。</returns>
        /// 
        /// <exception cref="FormatException">
        ///     フォーマット上のエラーが発生しました。</exception>
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="FailuredDelegateException">
        ///     デリゲート内部でエラーが発生しました。</exception>
        List<DeserializeResult> ConvertPickChildren<DeserializeResult>(
            out List<int>                                   resultIdx,
            int[]                                           markers,
            Func<
                INodeViewer         /* node */,                     
                int                 /* index */,                    
                DeserializeResult   /* returns(parsed data) */>
                                                            deserializer,
            Predicate<
                DeserializeResult   /* parsed data */>
                                                            predicate)
        {

            var ans         = new List<DeserializeResult>();
                resultIdx   = new List<int>();
            var children    = CollectChildren;

            try
            {

                // マーカー値絞り込みを考慮しない場合
                if (markers.Length == 0)
                    for (int i = 0, l = children.Count; i < l; ++i)
                    {

                        var deserialized = deserializer(
                            /* node = */    children[i],
                            /* index = */   i);
                        if (predicate(deserialized))
                        {
                            ans         .Add(deserialized);
                            resultIdx   .Add(i);
                        }

                    }

                // マーカー情報絞り込みを考慮する場合
                else
                    for (int node_i = 0, l = children.Count; node_i < l; ++node_i)
                        if (markers.Contains(children[node_i].Marker))
                        {

                            var deserialized = deserializer(
                                /* node = */    children[node_i],
                                /* index = */   node_i);
                            if (predicate(deserialized))
                            {
                                ans         .Add(deserialized);
                                resultIdx   .Add(node_i);
                            }

                        }

                return ans;

            }
            catch (IOException) { throw; }
            catch (FormatException) { throw; }
            catch (FailuredDelegateException) { throw; }

        }

        /// <summary>
        ///     指定したマーカー値と条件確認用デシリアライザを用いて<paramref name="predicate"/>に沿った条件を満たす子要素を<paramref name="deserializerForResult"/>の配列として取得します。<para/>
        ///     
        ///         TODO (for Implementer) :<paramref name="deserializerForPredicate"/>及び<paramref name="deserializerForResult"/>で返される例外は
        ///         <see cref="IOException"/>又は<see cref="FormatException"/>, <see cref="FailuredDelegateException"/>とし、
        ///         <paramref name="predicate"/>で返される例外は<see cref="IOException"/>又は<see cref="FailuredDelegateException"/>としてください。
        /// </summary>
        /// 
        /// <typeparam name="PredicateDeserializeType">
        ///     <paramref name="predicate"/>で条件を満たすかどうかを確認するために用いる変数型。最小限に抑えることで計算速度の改善が見込めます。</typeparam>
        /// <typeparam name="ResultDeserializeType">
        ///     戻り値として示すデシリアライズ結果。</typeparam>
        ///     
        /// <param name="resultIdx">
        ///     戻り値として得られる配列のこのノードにおけるインデックスの出力パラメータ。</param>
        /// <param name="markers">
        ///     検索するマーカー情報。長さ0の配列であった場合は全ての子要素について検査を行います。</param>
        /// <param name="deserializerForPredicate">
        ///     <paramref name="predicate"/>で使用する変数型に変換するためのデシリアライザ。
        ///     第一引数に子要素、第二引数に子要素に割り当てられている0から始まるインデックス番号が渡されます。
        ///     このメソッド内部の処理を必要最小限に抑えることで計算速度の改善が見込めます。</param>
        /// <param name="predicate">
        ///     取得するかどうかを指定する条件式。</param>
        /// <param name="deserializerForResult">
        ///     戻り値として取得するために使用するデシリアライザ。
        ///     第一引数に子要素、第二引数に子要素から得られた<paramref name="deserializerForPredicate"/>、第三引数に子要素に割り当てられている0から始まるインデックス番号が渡されます。</param>
        ///     
        /// <returns>
        ///     <typeparamref name="ResultDeserializeType"/>として変換を行った子ノードが示されます。
        ///     <typeparamref name="ResultDeserializeType"/>内のメンバーを変更したとしても、子ノードが影響を受けることはありません。</returns>
        ///     
        /// <exception cref="FormatException">
        ///     フォーマット上のエラーが発生しました。</exception>
        /// <exception cref="IOException">
        ///     アクセスエラーが発生しました。</exception>
        /// <exception cref="FailuredDelegateException">
        ///     デリゲート内部でエラーが発生しました。</exception>
        List<ResultDeserializeType> ConvertPickChildren<PredicateDeserializeType, ResultDeserializeType>(
            out List<int>                                                       resultIdx,
            int[]                                                               markers,
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
        {

            var ans         = new List<ResultDeserializeType>();
                resultIdx   = new List<int>();
            var children    = CollectChildren;

            try
            {

                if (markers.Length == 0)
                    for (int node_i = 0, node_l = children.Count; node_i < node_l; ++node_i)
                    {

                        var partialAnalyzed = deserializerForPredicate(
                            /* node = */    children[node_i],
                            /* index = */   node_i);

                        if (predicate(/* parsed data = */ partialAnalyzed))
                        {

                            ans.Add(
                                deserializerForResult(
                                    /* node = */    children[node_i],
                                    /* part = */    partialAnalyzed, 
                                    /* index = */   node_i));
                            resultIdx.Add(node_i);

                        }

                    }
                else
                    for (int node_i = 0, node_l = children.Count; node_i < node_l; ++node_i)
                        if (markers.Contains(/* value = */ children[node_i].Marker))
                        {

                            var partialAnalyzed = deserializerForPredicate(
                                /* node = */    children[node_i],
                                /* index = */   node_i);

                            if (predicate(/* parsed data = */ partialAnalyzed))
                            {

                                ans.Add(
                                    deserializerForResult(
                                        /* node = */    children[node_i],
                                        /* part = */    partialAnalyzed,
                                        /* index = */   node_i));
                                resultIdx   .Add(node_i);

                            }

                        }

                return ans;

            }
            catch (IOException) { throw; }
            catch (FormatException) { throw; }
            catch (FailuredDelegateException) { throw; }


        }

        #endregion

    }
}
