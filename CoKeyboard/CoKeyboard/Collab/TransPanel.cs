using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CoKeyboard.Collab
{
    public class TransPanel : Panel
    {

        Timer Wriggler = new Timer();

        #region TransPanel
        public TransPanel()
        {
            //
            // TODO: Add constructor logic here
            //

            Wriggler.Tick += new EventHandler(TickHandler);

            this.Wriggler.Interval = 50;

            this.Wriggler.Enabled = true;

        }
        #endregion

        #region TickHandler
        protected void TickHandler(object sender, EventArgs e)
        {
            this.InvalidateEx();
        }
        #endregion

        #region CreateParams
        protected override CreateParams CreateParams
        {

            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }
        #endregion

        #region InvalidateEx
        protected void InvalidateEx()
        {

            if (Parent == null)
                return;

            Rectangle rc = new Rectangle(this.Location, this.Size);

            Parent.Invalidate(rc, true);

        }
        #endregion

        #region OnPaintBackground
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

            //do not allow the background to be painted 
        }
        #endregion


    }
}
