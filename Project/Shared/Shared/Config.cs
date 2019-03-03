﻿using log4net;
using System;
using System.Configuration;

namespace Shared
{
    public static class Config
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string
            DEFAULT_TIMEOUT = "defaultTimeout",
            CLIENT_PROCESS_NUM = "clientProcessNum",
            BROKER_IP = "brokerIp",
            BROKER_PORT = "brokerPort",
            BROKER_PROCESS_NUM = "brokerProcessNum",
            STOCK_SERVER_IP = "stockServerIp",
            STOCK_SERVER_PORT = "stockServerPort",
            STOCK_SERVER_PROCESS_NUM = "stockServerProcesNum";

        public static string GetString(string key)
        {
            string val = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                val = appSettings[key];
                if (string.IsNullOrEmpty(val))
                    Log.Warn($"Empty value for {key} configuration.");
            }
            catch (ConfigurationErrorsException e)
            {
                Log.Error($"Error reading {key} from app settings.\n{e.BareMessage}\n{e.StackTrace}");
            }
            return val;
        }

        public static int GetInt(string key)
        {
            int val;
            if(int.TryParse(GetString(key),out val))
            {
                return val;
            }
            else
            {
                throw new Exception($"Error parsing configuration {key} to int.");
            }
        }
    }
}