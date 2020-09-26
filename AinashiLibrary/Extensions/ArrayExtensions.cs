using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace AinashiLibraryCSharp.Extensions
{
    public static class ArrayExtensions
    {

        /// <summary>
        /// <paramref name="srcArray"/>から<paramref name="dstArray"/>へデータをコピーします。
        /// </summary>
        /// <typeparam name="SrcType">コピー元配列の変数型。<typeparamref name="DstType"/>との互換性がある必要があります。</typeparam>
        /// <typeparam name="DstType">コピー先配列の変数型。</typeparam>
        /// <param name="srcArray">データを格納しているコピー元の配列。インデックスによるランダムな読み取りアクセスが条件となります。</param>
        /// <param name="dstArray">コピー先の配列。</param>
        /// <param name="length">コピーする要素の数。</param>
        public static void CopyTo<SrcType, DstType>(this IList<SrcType> srcArray, DstType[] dstArray, int length)
            where SrcType : DstType
            => CopyTo(srcArray, 0, dstArray, 0, length);

        /// <summary>
        /// <paramref name="srcArray"/>から<paramref name="dstArray"/>へデータをコピーします。
        /// </summary>
        /// <typeparam name="SrcType">コピー元配列の変数型。<typeparamref name="DstType"/>との互換性がある必要があります。</typeparam>
        /// <typeparam name="DstType">コピー先配列の変数型。</typeparam>
        /// <param name="srcArray">データを格納しているコピー元の配列。インデックスによるランダムな読み取りアクセスが条件となります。</param>
        /// <param name="dstArray">コピー先の配列。</param>
        /// <param name="length">コピーする要素の数。</param>
        public static void CopyTo<SrcType, DstType>(this SrcType[] srcArray, DstType[] dstArray, int length)
            where SrcType : DstType
            => CopyTo(srcArray, 0, dstArray, 0, length);

        /// <summary>
        /// 指定した<paramref name="srcIdx"/>を開始位置として<paramref name="srcArray"/>から指定した区間<paramref name="length"/>をコピーし、
        /// 指定した<paramref name="dstIdx"/>を開始位置として他の<paramref name="dstArray"/>にそれらの要素を貼り付けます。
        /// </summary>
        /// <typeparam name="SrcType">コピー元配列の変数型。<typeparamref name="DstType"/>との互換性がある必要があります。</typeparam>
        /// <typeparam name="DstType">コピー先配列の変数型。</typeparam>
        /// <param name="srcArray">データを格納しているコピー元の配列。インデックスによるランダムな読み取りアクセスが条件となります。</param>
        /// <param name="srcIdx">コピー操作の開始位置となる<paramref name="srcArray"/>内のインデックス番号。</param>
        /// <param name="dstArray">コピー先の配列。</param>
        /// <param name="dstIdx">コピー操作の開始位置となる<paramref name="dstArray"/>内のインデックス番号。</param>
        /// <param name="length">コピーする要素の数。</param>
        public static void CopyTo<SrcType, DstType>(this IList<SrcType> srcArray, int srcIdx, DstType[] dstArray, int dstIdx, int length)
            where SrcType: DstType
        {

            for (int src_i = srcIdx, dst_i = dstIdx, count_i = 0; count_i < length; src_i++, dst_i++, count_i++)
                dstArray[dst_i] = srcArray[src_i];

        }

        /// <summary>
        /// 指定した<paramref name="srcIdx"/>を開始位置として<paramref name="srcArray"/>から指定した区間<paramref name="length"/>をコピーし、
        /// 指定した<paramref name="dstIdx"/>を開始位置として他の<paramref name="dstArray"/>にそれらの要素を貼り付けます。
        /// </summary>
        /// <typeparam name="SrcType">コピー元配列の変数型。<typeparamref name="DstType"/>との互換性がある必要があります。</typeparam>
        /// <typeparam name="DstType">コピー先配列の変数型。</typeparam>
        /// <param name="srcArray">データを格納しているコピー元の配列。インデックスによるランダムな読み取りアクセスが条件となります。</param>
        /// <param name="srcIdx">コピー操作の開始位置となる<paramref name="srcArray"/>内のインデックス番号。</param>
        /// <param name="dstArray">コピー先の配列。</param>
        /// <param name="dstIdx">コピー操作の開始位置となる<paramref name="dstArray"/>内のインデックス番号。</param>
        /// <param name="length">コピーする要素の数。</param>
        public static void CopyTo<SrcType, DstType>(this SrcType[] srcArray, int srcIdx, DstType[] dstArray, int dstIdx, int length)
            where SrcType : DstType
        {

            for (int src_i = srcIdx, dst_i = dstIdx, count_i = 0; count_i < length; src_i++, dst_i++, count_i++)
                dstArray[dst_i] = srcArray[src_i];

        }
    }
}
