using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NupackageDllExtractor.FormUtils
{
    public static class FormConfigurator
    {
        public static void SetAsFixedForm(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.AutoScroll = true;
            form.AutoScrollMargin = new Size(1, 1);
            form.AutoScrollMinSize = new Size(1, 1);
        }
    }
}
