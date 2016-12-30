﻿using ClientCommon;
using System;
using ClientCommon.Data;
using System.ServiceModel;

namespace Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class CallbackMethods : ICallbackMethods
    {
        MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;

        public void LogInCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
             {
                 mainWindow.LogInCallbackResult(employee);
             });
        }

        public void LogOutCallback(Employee employee)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                mainWindow.LogOutCallbackResult(employee);
            });
        }
    }
}