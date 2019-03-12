using System.Drawing;
using System.Windows.Forms;

namespace NupackageDllExtractorLib.FormControls
{
    public class CustomTableLayoutPanel : TableLayoutPanel
    {
        public CustomTableLayoutPanel() { }

        public CustomTableLayoutPanel(int columnCount)
        {
            this.ColumnCount = columnCount;
        }

        public static Label StyleLabel(Label label)
        {
            label.Font = SystemFonts.MessageBoxFont;
            label.Width = 100;
            label.AutoSize = false;
            label.Anchor = AnchorStyles.Left;
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        public static TextBox StyleTextBox(TextBox textBox)
        {
            textBox.Font = new Font(FontFamily.GenericSansSerif, 10);
            textBox.Dock = DockStyle.Fill;
            textBox.Location = new Point(0, textBox.Location.Y);
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            return textBox;
        }

        public static Button StyleButton(Button button)
        {
            button.Font = new Font(FontFamily.GenericSansSerif, 10);
            button.Anchor = AnchorStyles.None;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Height = 34;
            button.Width = 120;
            return button;
        }

        public static ComboBox StyleComboBox(ComboBox comboBox)
        {
            comboBox.Font = new Font(FontFamily.GenericSansSerif, 11);
            comboBox.Anchor = AnchorStyles.Left;
            comboBox.Dock = DockStyle.Fill;
            comboBox.Location = new Point(0, comboBox.Location.Y);
            comboBox.Margin = new Padding(3, 10, 3, 3);
            return comboBox;
        }
    }
}
