/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  Service Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2010 - 2018 Simon Carter.  All Rights Reserved.
 *
 *  Product:  Service Manager
 *  
 *  File: ServerSettingControl.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  02/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ServiceManager.Core;

using SharedControls;
using SharedControls.Controls;
using SharedControls.Forms;

namespace ServiceAdminConsole.Controls
{
    public partial class ServiceSettingControl : UserControl
    {
        #region Private Members

        private ServiceSetting _serverSetting;

        #endregion Private Members

        #region Constructors

        public ServiceSettingControl()
        {
            InitializeComponent();
        }

        public ServiceSettingControl(ServiceSetting setting)
            : this ()
        {
            _serverSetting = setting ?? throw new ArgumentNullException(nameof(setting));
            lblName.Text = setting.Name;
            CreatePropertyControl();
        }

        #endregion Constructors

        #region Properties

        public string SettingName
        {
            get
            {
                return (_serverSetting.Name);
            }
        }

        public string SettingValue
        {
            get
            {
                return (_serverSetting.Value);
            }
        }

        #endregion Properties

        #region Private Methods

        private void CreatePropertyControl()
        {
            switch (_serverSetting.SettingType)
            {
                case SettingType.Bool:
                    CreateInputBool(_serverSetting.GetValue<bool>());
                    break;
                case SettingType.Decimal:
                    CreateInputDecimal(_serverSetting.GetValue<decimal>());
                    break;
                case SettingType.Integer:
                    CreateInputInteger(_serverSetting.GetValue<int>());
                    break;
                case SettingType.List:
                    CreateInputList(_serverSetting.Items.Split(_serverSetting.ItemSeperator), _serverSetting.Value);
                    break;
                case SettingType.String:
                    CreateInputString(_serverSetting.Value, false);
                    break;
                case SettingType.Password:
                    CreateInputString(_serverSetting.Value, true);
                    break;

                default:
                    throw new Exception("Unknown Setting Type");
            }
        }

        private void CreateInputString(string value, bool isPassword)
        {
            TextBoxEx textBox = new TextBoxEx();
            textBox.Parent = this;
            textBox.Left = 200;
            textBox.Top = 1;
            textBox.Height = 20;
            textBox.Width = this.Width = textBox.Left;
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            textBox.Text = value;
            textBox.MaxLength = _serverSetting.MaximumLength;
            textBox.AllowedCharacters = _serverSetting.AllowedCharacters;
            textBox.TextChanged += TextBox_TextChanged;

            if (isPassword)
                textBox.PasswordChar = '*';
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            _serverSetting.Value = ((TextBox)sender).Text;
        }

        private void CreateInputList(string[] values, string selected)
        {
            if (values.Length == 0)
                throw new ArgumentException(nameof(values));

            ComboBox comboBox = new ComboBox();
            comboBox.Parent = this;
            comboBox.Left = 200;
            comboBox.Top = 1;
            comboBox.Height = 21;
            comboBox.Width = this.Width = comboBox.Left;
            comboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            foreach (string item in values)
            {
                int index = comboBox.Items.Add(values);

                if (item == selected)
                    comboBox.SelectedIndex = index;
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _serverSetting.Value = ((ComboBox)sender).SelectedItem.ToString();
        }

        private void CreateInputInteger(int value)
        {
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Parent = this;
            numericUpDown.Left = 200;
            numericUpDown.Top = 1;
            numericUpDown.Height = 21;
            numericUpDown.Width = 90;
            numericUpDown.Minimum = _serverSetting.MinimumValue;
            numericUpDown.Maximum = _serverSetting.MaximumValue;
            numericUpDown.DecimalPlaces = _serverSetting.DecimalPlaces;
            numericUpDown.ValueChanged += NumericUpDown_ValueChangedInteger;
            numericUpDown.Value = _serverSetting.GetValue<int>();
        }

        private void NumericUpDown_ValueChangedInteger(object sender, EventArgs e)
        {
            _serverSetting.Value = ((NumericUpDown)sender).Value.ToString();
        }

        private void CreateInputDecimal(decimal value)
        {
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Parent = this;
            numericUpDown.Left = 200;
            numericUpDown.Top = 1;
            numericUpDown.Height = 21;
            numericUpDown.Width = 90;
            numericUpDown.Minimum = _serverSetting.MinimumValue;
            numericUpDown.Maximum = _serverSetting.MaximumValue;
            numericUpDown.DecimalPlaces = _serverSetting.DecimalPlaces;
            numericUpDown.ValueChanged += NumericUpDown_ValueChangedDecimal;

            numericUpDown.Value = _serverSetting.GetValue<decimal>();
        }

        private void NumericUpDown_ValueChangedDecimal(object sender, EventArgs e)
        {
            _serverSetting.Value = ((NumericUpDown)sender).Value.ToString();
        }

        private void CreateInputBool(bool value)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Parent = this;
            checkBox.Left = 200;
            checkBox.Top = 1;
            checkBox.Height = 20;
            checkBox.Width = this.Width = checkBox.Left;
            checkBox.Checked = value;
            checkBox.CheckedChanged += CheckBox_CheckedChanged;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _serverSetting.Value = ((CheckBox)sender).Checked.ToString();
        }

        #endregion Private Methods
    }
}
