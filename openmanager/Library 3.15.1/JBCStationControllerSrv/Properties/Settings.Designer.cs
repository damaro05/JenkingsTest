﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace JBCStationControllerSrv
{
    namespace My
    {


        [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
        internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
        {

            private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

            public static Settings Default
            {
                get
                {
                    return defaultInstance;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("True")]
            public bool UpgradeSettings
            {
                get
                {
                    return ((bool)(this["UpgradeSettings"]));
                }
                set
                {
                    this["UpgradeSettings"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("True")]
            public bool SearchUSB
            {
                get
                {
                    return ((bool)(this["SearchUSB"]));
                }
                set
                {
                    this["SearchUSB"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("False")]
            public bool SearchETH
            {
                get
                {
                    return ((bool)(this["SearchETH"]));
                }
                set
                {
                    this["SearchETH"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("")]
            public string TraceManagerUri
            {
                get
                {
                    return ((string)(this["TraceManagerUri"]));
                }
                set
                {
                    this["TraceManagerUri"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("")]
            public string TraceManagerServerCode
            {
                get
                {
                    return ((string)(this["TraceManagerServerCode"]));
                }
                set
                {
                    this["TraceManagerServerCode"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("1970-01-01")]
            public global::System.DateTime EventLogLastDataCollection
            {
                get
                {
                    return ((global::System.DateTime)(this["EventLogLastDataCollection"]));
                }
                set
                {
                    this["EventLogLastDataCollection"] = value;
                }
            }

            [global::System.Configuration.UserScopedSettingAttribute()]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            [global::System.Configuration.DefaultSettingValueAttribute("True")]
            public bool EnableStationWorkingEvent
            {
                get
                {
                    return ((bool)(this["EnableStationWorkingEvent"]));
                }
                set
                {
                    this["EnableStationWorkingEvent"] = value;
                }
            }
        }
    }
}
