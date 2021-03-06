﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public static class TabularConnection
    {
        private const string ApplicationNameKey = "Application Name";
        private const string DataSourceKey = "Data Source";
        private const string ProviderKey = "Provider";
        private const string UsernameKey = "User ID";
        private const string PasswordKey = "Password";

        private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString)
        {
            DbConnectionStringBuilder csb = new DbConnectionStringBuilder();
            try
            {
                csb.ConnectionString = serverNameOrConnectionString;
            }
            catch (ArgumentException ex)
            {
            }

            if (!csb.ContainsKey("Provider")) csb.Add(ProviderKey, "MSOLAP");
            if (!csb.ContainsAny("Data Source", "DataSource")) csb.Add(DataSourceKey, serverNameOrConnectionString);

            return csb;
        }
        private static DbConnectionStringBuilder GetBuilder(string serverNameOrConnectionString, string applicationName)
        {
            var csb = GetBuilder(serverNameOrConnectionString);
            csb[ApplicationNameKey] = applicationName;
            return csb;
        }

        public static string GetConnectionString(string serverNameOrConnectionString, string applicationName)
        {
            return GetBuilder(serverNameOrConnectionString, applicationName).ToString();
        }

        public static string StripApplicationName(string connectionString)
        {
            var csb = GetBuilder(connectionString);
            if (csb.ContainsKey(ApplicationNameKey)) csb.Remove(ApplicationNameKey);
            return csb.ToString();
        }

        private static bool ContainsAny(this DbConnectionStringBuilder csb, params string[] keys)
        {
            foreach (var key in keys)
                if (csb.ContainsKey(key)) return true;
            return false;
        }

        public static string GetConnectionString(string serverNameOrConnectionString, string userName, string password, string applicationName)
        {
            var csb = GetBuilder(serverNameOrConnectionString, applicationName);

            if (!csb.ContainsAny("User ID", "UID", "UserName"))
                csb.Add(UsernameKey, userName);
            if (!csb.ContainsAny("Password", "PWD"))
                csb.Add(PasswordKey, userName);

            return csb.ToString();
        }

        public static bool IsSensitive(string connectionString)
        {
            var csb = new DbConnectionStringBuilder();
            csb.ConnectionString = connectionString;
            return csb.ContainsAny("Password", "PWD");
        }

        public static string StripSensitive(string connectionString)
        {
            var csb = GetBuilder(connectionString);
            if (csb.ContainsKey("Password")) csb.Remove("Password");
            if (csb.ContainsKey("PWD")) csb.Remove("PWD");
            return csb.ToString();
        }

        public static string GetConnectionString(string serverName)
        {
            return string.Format("Provider=MSOLAP;DataSource={0}", serverName);
        }

        public static string GetConnectionString(string serverName, string userName, string password)
        {
            return string.Format("Provider=MSOLAP;DataSource={0};User ID={1};Password=\"{2}\";Persist Security Info=True;Impersonation Level=Impersonate;",
                serverName,
                userName,
                password.Replace("\"", "\"\""));
        }
    }
}
