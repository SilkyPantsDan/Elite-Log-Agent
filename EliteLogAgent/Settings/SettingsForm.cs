﻿using EliteLogAgent.Settings;
using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Utility;

namespace EliteLogAgent
{
    public partial class SettingsForm : Form
    {
        // These fields have to be properties because Form designer does not allow arguments in constructor
        internal ISettingsProvider Provider { get; set; }
        internal IMessageBroker MessageBroker { get; set; }
        internal List<IPlugin> Plugins { get; set; }

        private IDictionary<string, AbstractSettingsControl> SettingsControls = new Dictionary<string, AbstractSettingsControl>();

        public SettingsForm()
        {
            InitializeComponent();

            var versionLabel = "Version: " + AppInfo.Version;
            Text += ". " + versionLabel;
            Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            var generalSettingsControl = new GeneralSettingsControl() { MessageBroker = MessageBroker };
            generalSettingsControl.Dock = DockStyle.Fill;
            generalSettingsControl.PerformLayout();
            SettingsControls.Add("General", generalSettingsControl );
            //PlaceControlGroup("General", generalSettingsControl);
            flowLayoutPanel1.Controls.Add(generalSettingsControl);

            foreach (var plugin in Plugins)
            {
                var control = plugin.GetPluginSettingsControl();
                if (control == null)
                    continue;

                control.PerformLayout();
                control.Dock = DockStyle.Fill;
                SettingsControls.Add(plugin.PluginId, control);
                //PlaceControlGroup(plugin.SettingsLabel, control);
                flowLayoutPanel1.Controls.Add(control);
            }
            PerformLayout();
            Settings = Provider.Settings;
        }

        private void PlaceControlGroup(string name, Control control)
        {
            var groupBox = new GroupBox() { Text = name };
            groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            groupBox.Controls.Add(control);
            // groupBox.AutoSize = true;
            groupBox.Size = control.MinimumSize;
            flowLayoutPanel1.Controls.Add(groupBox);
            groupBox.PerformLayout();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e) => ApplySettings();

        private void ApplySettings() => Provider.Settings = Settings;

        private void CancelButton_Click(object sender, EventArgs e) => Close();

        GlobalSettings Settings
        {
            get
            {
                var newSettings = new GlobalSettings
                {
                    PluginSettings = SettingsControls.ToDictionary(c => c.Key, c => c.Value.Settings)
                };
                return newSettings;
            }
            set
            {
                var newSettings = value;
                foreach (var category in SettingsControls)
                {
                    if (newSettings.PluginSettings.TryGetValue(category.Key, out JObject settings))
                        category.Value.Settings = settings;
                }
            }
        }
    }
}