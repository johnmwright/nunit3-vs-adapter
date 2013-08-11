﻿// ****************************************************************
// Copyright (c) 2011 NUnit Software. All rights reserved.
// ****************************************************************

using System;
using System.Runtime.Remoting.Channels;
using NUnit.Util;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NUnit.VisualStudio.TestAdapter
{
    using System.Reflection;

    /// <summary>
    /// NUnitTestAdapter is the common base for the
    /// NUnit discoverer and executor classes.
    /// </summary>
    public abstract class NUnitTestAdapter
    {
        #region Properties

        // The logger currently in use
        protected static IMessageLogger Logger { get; set; }

        // The adapter version
        private static string Version { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The common constructor initializes NUnit services 
        /// needed to load and run tests and sets some properties.
        /// </summary>
        protected NUnitTestAdapter()
        {
            ServiceManager.Services.AddService(new DomainManager());
            ServiceManager.Services.AddService(new ProjectService());

            ServiceManager.Services.InitializeServices();

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #endregion

        #region Protected Helper Methods

        protected void Info(string method, string function)
        {
            string msg = string.Format("NUnit {0} {1} is {2}", Version, method, function);
            SendInformationalMessage(msg);
        }

        protected static void CleanUpRegisteredChannels()
        {
            foreach (IChannel chan in ChannelServices.RegisteredChannels)
                ChannelServices.UnregisterChannel(chan);
        }

        protected void AssemblyNotSupportedWarning(string sourceAssembly)
        {
            SendWarningMessage("Assembly not supported: " + sourceAssembly);
        }

        protected void DependentAssemblyNotFoundWarning(string dependentAssembly, string sourceAssembly)
        {
            SendWarningMessage("Dependent Assembly "+dependentAssembly+" of " + sourceAssembly + " not found. Can be ignored if not a NUnit project.");
        }

        protected void NUnitLoadError(string sourceAssembly)
        {
            SendErrorMessage("NUnit failed to load " + sourceAssembly);
        }

        #endregion

        #region Public Static Methods

        public static void SendErrorMessage(string message)
        {
            Logger.SendMessage(TestMessageLevel.Error, message);
        }

        public static void SendErrorMessage(string message, Exception ex)
        {
            Logger.SendMessage(TestMessageLevel.Error, message);
            Logger.SendMessage(TestMessageLevel.Error, ex.ToString());
        }

        public static void SendWarningMessage(string message)
        {
            Logger.SendMessage(TestMessageLevel.Warning, message);
        }

        public static void SendInformationalMessage(string message)
        {
            Logger.SendMessage(TestMessageLevel.Informational, message);
        }

        public static void SendDebugMessage(string message)
        {
#if DEBUG
            Logger.SendMessage(TestMessageLevel.Informational, message);
#endif
        }

        #endregion
    }

    
}
