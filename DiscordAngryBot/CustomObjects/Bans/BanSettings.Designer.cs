﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiscordAngryBot.CustomObjects.Bans {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class BanSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static BanSettings defaultInstance = ((BanSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BanSettings())));
        
        public static BanSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1800000")]
        public int AutoBanLength {
            get {
                return ((int)(this["AutoBanLength"]));
            }
            set {
                this["AutoBanLength"] = value;
            }
        }
    }
}
