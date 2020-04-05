namespace DW.ELA.Interfaces
{
    using DW.ELA.Interfaces.Settings;

    public class AbstractSettingsControl //: System.Windows.Forms.UserControl
    {
        public AbstractSettingsControl()
        {
        }

        /// <summary>
        /// Gets or sets reference to temporary instance of Settings existing in settings form
        /// </summary>
        public GlobalSettings GlobalSettings { get; set; }

        public virtual void SaveSettings()
        {
        }
    }
}
