using Dalamud.Configuration;

namespace DrahsidLib;

public class ConfigurationBase : IPluginConfiguration {
    int IPluginConfiguration.Version { get; set; }

    #region Saved configuration values
    public bool HideTooltips { get; set; } = false;
    #endregion

    public void Initialize() {
        Save();
    }

    public void Save() {
        Service.Interface.SavePluginConfig(this);
    }
}
