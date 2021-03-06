﻿//-----------------------------------------------------------------------
// <summary>RelayCommandクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-04-06 ‏‎23:09:37  +9:00 $</date>
// <copyright file="$Name: RelayCommand.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace MoonGate.Component
{
    /// <summary>
    /// 自作RelayCommandクラス
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        /// <summary>
        /// 実行用メソッド
        /// </summary>
        private Action<object> _execute = null;

        /// <summary>
        /// 実行可否判定用メソッド
        /// </summary>
        private Predicate<object> _canExecute = null;

        /// <summary>
        /// 実行可否状態変更検知用イベントハンドラ
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        /// <summary>
        /// コンストラクタ（可否判定不使用時）
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }


        /// <summary>
        /// コンストラクタ（可否判定使用時）
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }


        /// <summary>
        /// コマンドの実行可否状態が変更された事を知らせるイベント
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
            return;
        }


        /// <summary>
        /// コマンド実行可否判定
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }


        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
            return;
        }


        /// <summary>
        /// 後始末用メソッド
        /// </summary>
        public void Clean()
        {
            _execute = null;
            _canExecute = null;
        }
    }
}
