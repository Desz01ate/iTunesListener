﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iTunesListener.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("139971873511766")]
        public string AppID {
            get {
                return ((string)(this["AppID"]));
            }
            set {
                this["AppID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7caf7e0f75c3f1a61208a916b824dd27")]
        public string AppSecret {
            get {
                return ((string)(this["AppSecret"]));
            }
            set {
                this["AppSecret"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("abcde")]
        public string AccessToken {
            get {
                return ((string)(this["AccessToken"]));
            }
            set {
                this["AccessToken"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoShare {
            get {
                return ((bool)(this["AutoShare"]));
            }
            set {
                this["AutoShare"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%track - %artist")]
        public string DiscordPlayDetail {
            get {
                return ((string)(this["DiscordPlayDetail"]));
            }
            set {
                this["DiscordPlayDetail"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%playlist_type: %playlist_name")]
        public string DiscordPlayState {
            get {
                return ((string)(this["DiscordPlayState"]));
            }
            set {
                this["DiscordPlayState"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%track - %artist")]
        public string DiscordPauseDetail {
            get {
                return ((string)(this["DiscordPauseDetail"]));
            }
            set {
                this["DiscordPauseDetail"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Paused")]
        public string DiscordPauseState {
            get {
                return ((string)(this["DiscordPauseState"]));
            }
            set {
                this["DiscordPauseState"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Listening to %track - %album by %artist on Apple Music!")]
        public string FacebookFormat {
            get {
                return ((string)(this["FacebookFormat"]));
            }
            set {
                this["FacebookFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ChromaSDKEnable {
            get {
                return ((bool)(this["ChromaSDKEnable"]));
            }
            set {
                this["ChromaSDKEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color Background_Playing {
            get {
                return ((global::System.Drawing.Color)(this["Background_Playing"]));
            }
            set {
                this["Background_Playing"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color Background_Pause {
            get {
                return ((global::System.Drawing.Color)(this["Background_Pause"]));
            }
            set {
                this["Background_Pause"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Red")]
        public global::System.Drawing.Color Position_Foreground {
            get {
                return ((global::System.Drawing.Color)(this["Position_Foreground"]));
            }
            set {
                this["Position_Foreground"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("White")]
        public global::System.Drawing.Color Position_Background {
            get {
                return ((global::System.Drawing.Color)(this["Position_Background"]));
            }
            set {
                this["Position_Background"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("255, 40, 0")]
        public global::System.Drawing.Color Volume {
            get {
                return ((global::System.Drawing.Color)(this["Volume"]));
            }
            set {
                this["Volume"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool WebServiceListening {
            get {
                return ((bool)(this["WebServiceListening"]));
            }
            set {
                this["WebServiceListening"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DiscordRichPresenceEnable {
            get {
                return ((bool)(this["DiscordRichPresenceEnable"]));
            }
            set {
                this["DiscordRichPresenceEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public byte HistoryStackLimit {
            get {
                return ((byte)(this["HistoryStackLimit"]));
            }
            set {
                this["HistoryStackLimit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DynamicColorEnable {
            get {
                return ((bool)(this["DynamicColorEnable"]));
            }
            set {
                this["DynamicColorEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ReverseLEDRender {
            get {
                return ((bool)(this["ReverseLEDRender"]));
            }
            set {
                this["ReverseLEDRender"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AlbumCoverRenderEnable {
            get {
                return ((bool)(this["AlbumCoverRenderEnable"]));
            }
            set {
                this["AlbumCoverRenderEnable"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int RefreshRate {
            get {
                return ((int)(this["RefreshRate"]));
            }
            set {
                this["RefreshRate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AdaptiveDensity {
            get {
                return ((bool)(this["AdaptiveDensity"]));
            }
            set {
                this["AdaptiveDensity"] = value;
            }
        }
    }
}
