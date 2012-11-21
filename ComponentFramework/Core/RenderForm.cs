using System.Windows.Forms;

namespace ComponentFramework.Core
{
    public partial class RenderForm : Form
    {
        public RenderForm()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            /* Do nothing to prevent flicker during resize */
        }
    }
}
