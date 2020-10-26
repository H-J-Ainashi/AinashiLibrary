using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace AinashiLibraryCSharp.Algorithm
{

    /// <summary>
    /// 非同期入力受付を一元的に管理するクラス。
    /// </summary>
    /// <typeparam name="SignalData"></typeparam>
    public class AsyncFreeSignal<SignalData>
        where SignalData : class
    {


        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Sync Fields
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Sync Fields

        /// <summary>
        /// プッシュされたシグナル。
        /// </summary>
        private SignalData? pushedSignal;

        /// <summary>
        /// <see cref="pushedSignal"/>の編集権限を管理する排他ロック。
        /// </summary>
        private readonly object lockWritingSignal = new object();

        /// <summary>
        /// プッシュを許可状態にするための可変メソッド。
        /// </summary>
        private readonly Action? AllowingSignal;

        /// <summary>
        /// プッシュを禁止状態にするための可変メソッド。
        /// </summary>
        private readonly Action? BanPushSignal;

        /// <summary>
        /// プッシュの更新を監視する間隔。
        /// </summary>
        private readonly int intervalMilliSeconds;

        /// <summary>
        /// プッシュが許可状態かどうかを示す値。
        /// </summary>
        private bool isAllowPushSignal;

        #endregion


        /* *'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'*'* *\
         * 
         * Constractors
         * 
        \* *,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,*,* */

        #region Constractors

        /// <summary>
        /// 非同期入力受付を管理するインスタンスを作成します。
        /// </summary>
        /// <param name="initializeAllowPushSignal">初期化時にシグナルのプッシュを有効化するかどうかを指定する<see cref="bool"/>値です。
        /// <paramref name="defaultAllowPushSignal"/>が<see cref="true"/>である場合はこの値が無視されます。</param>
        /// <param name="intervalMilliSeconds"></param>
        /// <param name="defaultAllowPushSignal"></param>
        private AsyncFreeSignal(bool initializeAllowPushSignal, int intervalMilliSeconds = 50, bool defaultAllowPushSignal = false)
        {

            if (defaultAllowPushSignal)
            {

                this.AllowingSignal = () => { };
                this.BanPushSignal = () => { };

            }
            else
            {

                this.AllowingSignal = () => isAllowPushSignal = true;
                this.BanPushSignal = () => isAllowPushSignal = false;


            }

            this.intervalMilliSeconds = intervalMilliSeconds;
            this.isAllowPushSignal = initializeAllowPushSignal;
            

        }

        #endregion


    }
}
