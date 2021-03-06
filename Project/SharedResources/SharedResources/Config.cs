﻿using System;
using System.Configuration;
using log4net;

namespace Shared
{
    public static class Config
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static long _processID;

        public const string

            CLEANUP_DEAD_CLIENTS = "cleanupDeadClients",
            MAX_STOCK_HISTORY = "MaxStockHistorySize",
            MAX_BTN_STOCK_HISTORY = "MaxStockHistorySizeForButtons",

            DEFAULT_UDP_CLIENT = "defaultUdpClientName",
            DEFAULT_TCP_CLIENT = "defaultTcpClientName",
            DEFAULT_TCP_LISTENER = "defaultTcpListenerName",
            DEFAULT_TIMEOUT = "defaultTimeout",
            DEFAULT_RETRY_COUNT = "defaultRetryCount",

            //CLIENT_PROCESS_NUM = "clientProcessNum",

            BROKER_IP = "brokerIp",
            BROKER_PORT = "brokerPort",
            BROKER_PROCESS_NUM = "brokerProcessNum",

            STOCK_SERVER_IP = "stockServerIp",
            STOCK_SERVER_PORT = "stockServerPort",
            STOCK_SERVER_TCP_PORT = "stockServerTcpPort",
            STOCK_SERVER_PROCESS_NUM = "stockServerProcesNum",

            STOCK_TICK_DELAY = "stockTickDelay";


        public static long GetClientProcessNumber()
        {
            if (default(long).Equals(_processID))
            {
                _processID = DateTime.Now.Ticks;
            }

            return _processID;
        }

        public static string GetString(string key)
        {
            string val = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                val = appSettings[key];
                if (String.IsNullOrEmpty(val))
                {
                    Log.Warn($"Empty value for {key} configuration.");
                }
            }
            catch (ConfigurationErrorsException e)
            {
                Log.Error($"Error reading {key} from app settings.\n{e.BareMessage}\n{e.StackTrace}");
            }
            return val;
        }

        public static int GetInt(string key)
        {
            var value = GetString(key);
            if (Int32.TryParse(value, out int val))
            {
                return val;
            }
            else
            {
                throw new Exception($"Error parsing configuration {key} to int.");
            }
        }

        public static bool GetBool(string key)
        {
            var value = GetString(key);
            if (value.Equals("true"))
            {
                return true;
            }
            return false;
        }
    }
}
