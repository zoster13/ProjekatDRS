﻿using System.Runtime.CompilerServices;

namespace Server.Logger
{
    public class LogHelper
    {
        public static log4net.ILog GetLogger([CallerFilePath]string fileName = "")
        {
            return log4net.LogManager.GetLogger(fileName);
        }
    }
}
