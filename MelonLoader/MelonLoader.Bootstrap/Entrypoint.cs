﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MelonLoader.Shared.NativeUtils;
using MelonLoader.Shared.Utils;

namespace MelonLoader.Bootstrap
{
    public static class Entrypoint
    {
        public static unsafe void Entry()
        {
            Shared.Core.Startup();
            Shared.Fixes.UnhandledException.Install(AppDomain.CurrentDomain);
            Shared.Fixes.DotnetLoadFromManagedFolderFix.Install();
            
            MelonDebug.Msg("Starting Up...");
            MelonDebug.Msg(MelonEnvironment.MelonLoaderDirectory);
            var module = ModuleManager.FindBootstrapModule();
            
            if (module == null)
            {
                Assertion.ThrowInternalFailure("Failed to find a valid Bootstrap module for this Game Engine!");
                return;
            }
            
            MelonDebug.Msg("Found Bootstrap Module: " + module.GetType().FullName);
            MelonLogger.Msg($"Running Engine: {module.EngineName}");

            module.Startup();
        }
    }   
}