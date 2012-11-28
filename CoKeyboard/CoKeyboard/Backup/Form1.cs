using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace SmallKeyboard
{
	public partial class frmKeyboard
	{
		public frmKeyboard()
		{
			InitializeComponent();
		}
		
		[DllImport("KERNEL32.DLL")]
        public static  extern void Beep(int freq, int dur);
		
		
		private void Play_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{	
			this.Focus();
			
			switch (e.KeyData.ToString())
			{
				case "A":
					
					this.btnMC_Click(sender, e);
					break;
				case "S":
					
					this.btnMD_Click(sender, e);
					break;
				case "D":
					
					this.btnME_Click(sender, e);
					break;
				case "F":
					
					this.btnMF_Click(sender, e);
					break;
				case "G":
					
					this.btnMG_Click(sender, e);
					break;
				case "H":
					
					this.btnMA_Click(sender, e);
					break;
				case "J":
					
					this.btnHC_Click(sender, e);
					break;
				case "K":
					
					this.btnHD_Click(sender, e);
					break;
				case "L":
					
					this.btnHE_Click(sender, e);
					break;
				case "Z":
					
					this.btnHF_Click(sender, e);
					break;
				case "X":
					
					this.btnHG_Click(sender, e);
					break;
				case "C":
					
					this.btnHA_Click(sender, e);
					break;
			}
			
		}
		
		
		private void frmKeyboard_Load(object sender, System.EventArgs e)
		{
			
		}
		
		
		private void btnMC_Click(System.Object sender, System.EventArgs e)
		{
			// middle C
			Beep(261, 150);
		}
		
		private void btnMD_Click(System.Object sender, System.EventArgs e)
		{
			Beep(293, 150);
		}
		
		private void btnME_Click(System.Object sender, System.EventArgs e)
		{
			Beep(329, 150);
		}
		
		private void btnMF_Click(System.Object sender, System.EventArgs e)
		{
			Beep(349, 150);
		}
		
		private void btnMCs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(277, 150);
		}
		
		private void btnMDs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(311, 150);
		}
		
		private void btnMFs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(369, 150);
		}
		
		private void btnMG_Click(System.Object sender, System.EventArgs e)
		{
			Beep(391, 150);
		}
		
		private void btnMGs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(415, 150);
		}
		
		private void btnMA_Click(System.Object sender, System.EventArgs e)
		{
			Beep(440, 150);
		}
		
		private void btnMAs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(466, 150);
		}
		
		private void btnMB_Click(System.Object sender, System.EventArgs e)
		{
			Beep(493, 150);
		}
		
		private void btnHC_Click(System.Object sender, System.EventArgs e)
		{
			Beep(523, 150);
		}
		
		private void btnHCs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(554, 150);
		}
		
		private void btnHD_Click(System.Object sender, System.EventArgs e)
		{
			Beep(587, 150);
		}
		
		private void btnHDs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(622, 150);
		}
		
		private void btnHE_Click(System.Object sender, System.EventArgs e)
		{
			Beep(659, 150);
		}
		
		private void btnHF_Click(System.Object sender, System.EventArgs e)
		{
			Beep(698, 150);
		}
		
		private void btnHFs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(739, 150);
		}
		
		private void btnHG_Click(System.Object sender, System.EventArgs e)
		{
			Beep(783, 150);
		}
		
		private void btnHGs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(830, 150);
		}
		
		private void btnHA_Click(System.Object sender, System.EventArgs e)
		{
			Beep(880, 150);
		}
		
		private void btnHAs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(932, 150);
		}
		
		private void btnHB_Click(System.Object sender, System.EventArgs e)
		{
			Beep(987, 150);
		}
		
		private void btnLB_Click(System.Object sender, System.EventArgs e)
		{
			Beep(246, 150);
		}
		
		private void btnLAs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(233, 150);
		}
		
		private void btnLA_Click(System.Object sender, System.EventArgs e)
		{
			Beep(220, 150);
		}
		
		private void btnLGs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(207, 150);
		}
		
		private void btnLG_Click(System.Object sender, System.EventArgs e)
		{
			Beep(195, 150);
		}
		
		private void btnLFs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(184, 150);
		}
		
		private void btnLF_Click(System.Object sender, System.EventArgs e)
		{
			Beep(174, 150);
		}
		
		private void btnLE_Click(System.Object sender, System.EventArgs e)
		{
			Beep(164, 150);
		}
		
		private void btnLDs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(155, 150);
		}
		
		private void btnLD_Click(System.Object sender, System.EventArgs e)
		{
			Beep(146, 150);
		}
		
		private void btnLCs_Click(System.Object sender, System.EventArgs e)
		{
			Beep(138, 150);
		}
		
		private void btnLC_Click(System.Object sender, System.EventArgs e)
		{
			Beep(130, 150);
		}

        private void frmKeyboard_Load_1(object sender, EventArgs e)
        {

        }
		
	}
	
}
