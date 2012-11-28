using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;

namespace SmallKeyboard
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class frmKeyboard : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKeyboard));
            this.btnLC = new System.Windows.Forms.Button();
            this.btnLD = new System.Windows.Forms.Button();
            this.btnLE = new System.Windows.Forms.Button();
            this.btnLF = new System.Windows.Forms.Button();
            this.btnLG = new System.Windows.Forms.Button();
            this.btnLA = new System.Windows.Forms.Button();
            this.btnLB = new System.Windows.Forms.Button();
            this.btnLCs = new System.Windows.Forms.Button();
            this.btnLDs = new System.Windows.Forms.Button();
            this.btnLFs = new System.Windows.Forms.Button();
            this.btnLGs = new System.Windows.Forms.Button();
            this.btnLAs = new System.Windows.Forms.Button();
            this.btnMAs = new System.Windows.Forms.Button();
            this.btnMGs = new System.Windows.Forms.Button();
            this.btnMFs = new System.Windows.Forms.Button();
            this.btnMDs = new System.Windows.Forms.Button();
            this.btnMCs = new System.Windows.Forms.Button();
            this.btnMB = new System.Windows.Forms.Button();
            this.btnMA = new System.Windows.Forms.Button();
            this.btnMG = new System.Windows.Forms.Button();
            this.btnMF = new System.Windows.Forms.Button();
            this.btnME = new System.Windows.Forms.Button();
            this.btnMD = new System.Windows.Forms.Button();
            this.btnMC = new System.Windows.Forms.Button();
            this.btnHAs = new System.Windows.Forms.Button();
            this.btnHGs = new System.Windows.Forms.Button();
            this.btnHFs = new System.Windows.Forms.Button();
            this.btnHDs = new System.Windows.Forms.Button();
            this.btnHCs = new System.Windows.Forms.Button();
            this.btnHB = new System.Windows.Forms.Button();
            this.btnHA = new System.Windows.Forms.Button();
            this.btnHG = new System.Windows.Forms.Button();
            this.btnHF = new System.Windows.Forms.Button();
            this.btnHE = new System.Windows.Forms.Button();
            this.btnHD = new System.Windows.Forms.Button();
            this.btnHC = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLC
            // 
            this.btnLC.BackColor = System.Drawing.Color.White;
            this.btnLC.Location = new System.Drawing.Point(8, 11);
            this.btnLC.Name = "btnLC";
            this.btnLC.Size = new System.Drawing.Size(39, 167);
            this.btnLC.TabIndex = 0;
            this.btnLC.UseVisualStyleBackColor = false;
            this.btnLC.Click += new System.EventHandler(this.btnLC_Click);
            this.btnLC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLD
            // 
            this.btnLD.BackColor = System.Drawing.Color.White;
            this.btnLD.Location = new System.Drawing.Point(45, 11);
            this.btnLD.Name = "btnLD";
            this.btnLD.Size = new System.Drawing.Size(39, 167);
            this.btnLD.TabIndex = 1;
            this.btnLD.UseVisualStyleBackColor = false;
            this.btnLD.Click += new System.EventHandler(this.btnLD_Click);
            this.btnLD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLE
            // 
            this.btnLE.BackColor = System.Drawing.Color.White;
            this.btnLE.Location = new System.Drawing.Point(81, 11);
            this.btnLE.Name = "btnLE";
            this.btnLE.Size = new System.Drawing.Size(39, 167);
            this.btnLE.TabIndex = 2;
            this.btnLE.UseVisualStyleBackColor = false;
            this.btnLE.Click += new System.EventHandler(this.btnLE_Click);
            this.btnLE.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLF
            // 
            this.btnLF.BackColor = System.Drawing.Color.White;
            this.btnLF.Location = new System.Drawing.Point(117, 11);
            this.btnLF.Name = "btnLF";
            this.btnLF.Size = new System.Drawing.Size(39, 167);
            this.btnLF.TabIndex = 3;
            this.btnLF.UseVisualStyleBackColor = false;
            this.btnLF.Click += new System.EventHandler(this.btnLF_Click);
            this.btnLF.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLG
            // 
            this.btnLG.BackColor = System.Drawing.Color.White;
            this.btnLG.Location = new System.Drawing.Point(154, 11);
            this.btnLG.Name = "btnLG";
            this.btnLG.Size = new System.Drawing.Size(39, 167);
            this.btnLG.TabIndex = 4;
            this.btnLG.UseVisualStyleBackColor = false;
            this.btnLG.Click += new System.EventHandler(this.btnLG_Click);
            this.btnLG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLA
            // 
            this.btnLA.BackColor = System.Drawing.Color.White;
            this.btnLA.Location = new System.Drawing.Point(191, 11);
            this.btnLA.Name = "btnLA";
            this.btnLA.Size = new System.Drawing.Size(39, 167);
            this.btnLA.TabIndex = 5;
            this.btnLA.UseVisualStyleBackColor = false;
            this.btnLA.Click += new System.EventHandler(this.btnLA_Click);
            this.btnLA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLB
            // 
            this.btnLB.BackColor = System.Drawing.Color.White;
            this.btnLB.Location = new System.Drawing.Point(228, 11);
            this.btnLB.Name = "btnLB";
            this.btnLB.Size = new System.Drawing.Size(39, 167);
            this.btnLB.TabIndex = 6;
            this.btnLB.UseVisualStyleBackColor = false;
            this.btnLB.Click += new System.EventHandler(this.btnLB_Click);
            this.btnLB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLCs
            // 
            this.btnLCs.BackColor = System.Drawing.Color.Black;
            this.btnLCs.Location = new System.Drawing.Point(32, 11);
            this.btnLCs.Name = "btnLCs";
            this.btnLCs.Size = new System.Drawing.Size(29, 101);
            this.btnLCs.TabIndex = 7;
            this.btnLCs.UseVisualStyleBackColor = false;
            this.btnLCs.Click += new System.EventHandler(this.btnLCs_Click);
            this.btnLCs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLDs
            // 
            this.btnLDs.BackColor = System.Drawing.Color.Black;
            this.btnLDs.Location = new System.Drawing.Point(67, 11);
            this.btnLDs.Name = "btnLDs";
            this.btnLDs.Size = new System.Drawing.Size(29, 101);
            this.btnLDs.TabIndex = 8;
            this.btnLDs.UseVisualStyleBackColor = false;
            this.btnLDs.Click += new System.EventHandler(this.btnLDs_Click);
            this.btnLDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLFs
            // 
            this.btnLFs.BackColor = System.Drawing.Color.Black;
            this.btnLFs.Location = new System.Drawing.Point(141, 11);
            this.btnLFs.Name = "btnLFs";
            this.btnLFs.Size = new System.Drawing.Size(29, 101);
            this.btnLFs.TabIndex = 9;
            this.btnLFs.UseVisualStyleBackColor = false;
            this.btnLFs.Click += new System.EventHandler(this.btnLFs_Click);
            this.btnLFs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLGs
            // 
            this.btnLGs.BackColor = System.Drawing.Color.Black;
            this.btnLGs.Location = new System.Drawing.Point(176, 11);
            this.btnLGs.Name = "btnLGs";
            this.btnLGs.Size = new System.Drawing.Size(29, 101);
            this.btnLGs.TabIndex = 10;
            this.btnLGs.UseVisualStyleBackColor = false;
            this.btnLGs.Click += new System.EventHandler(this.btnLGs_Click);
            this.btnLGs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnLAs
            // 
            this.btnLAs.BackColor = System.Drawing.Color.Black;
            this.btnLAs.Location = new System.Drawing.Point(214, 11);
            this.btnLAs.Name = "btnLAs";
            this.btnLAs.Size = new System.Drawing.Size(29, 101);
            this.btnLAs.TabIndex = 11;
            this.btnLAs.UseVisualStyleBackColor = false;
            this.btnLAs.Click += new System.EventHandler(this.btnLAs_Click);
            this.btnLAs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMAs
            // 
            this.btnMAs.BackColor = System.Drawing.Color.Black;
            this.btnMAs.Location = new System.Drawing.Point(470, 11);
            this.btnMAs.Name = "btnMAs";
            this.btnMAs.Size = new System.Drawing.Size(29, 101);
            this.btnMAs.TabIndex = 23;
            this.btnMAs.UseVisualStyleBackColor = false;
            this.btnMAs.Click += new System.EventHandler(this.btnMAs_Click);
            this.btnMAs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMGs
            // 
            this.btnMGs.BackColor = System.Drawing.Color.Black;
            this.btnMGs.Location = new System.Drawing.Point(432, 11);
            this.btnMGs.Name = "btnMGs";
            this.btnMGs.Size = new System.Drawing.Size(29, 101);
            this.btnMGs.TabIndex = 22;
            this.btnMGs.UseVisualStyleBackColor = false;
            this.btnMGs.Click += new System.EventHandler(this.btnMGs_Click);
            this.btnMGs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMFs
            // 
            this.btnMFs.BackColor = System.Drawing.Color.Black;
            this.btnMFs.Location = new System.Drawing.Point(397, 11);
            this.btnMFs.Name = "btnMFs";
            this.btnMFs.Size = new System.Drawing.Size(29, 101);
            this.btnMFs.TabIndex = 21;
            this.btnMFs.UseVisualStyleBackColor = false;
            this.btnMFs.Click += new System.EventHandler(this.btnMFs_Click);
            this.btnMFs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMDs
            // 
            this.btnMDs.BackColor = System.Drawing.Color.Black;
            this.btnMDs.Location = new System.Drawing.Point(323, 11);
            this.btnMDs.Name = "btnMDs";
            this.btnMDs.Size = new System.Drawing.Size(29, 101);
            this.btnMDs.TabIndex = 20;
            this.btnMDs.UseVisualStyleBackColor = false;
            this.btnMDs.Click += new System.EventHandler(this.btnMDs_Click);
            this.btnMDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMCs
            // 
            this.btnMCs.BackColor = System.Drawing.Color.Black;
            this.btnMCs.Location = new System.Drawing.Point(288, 11);
            this.btnMCs.Name = "btnMCs";
            this.btnMCs.Size = new System.Drawing.Size(29, 101);
            this.btnMCs.TabIndex = 19;
            this.btnMCs.UseVisualStyleBackColor = false;
            this.btnMCs.Click += new System.EventHandler(this.btnMCs_Click);
            this.btnMCs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMB
            // 
            this.btnMB.BackColor = System.Drawing.Color.White;
            this.btnMB.Location = new System.Drawing.Point(484, 11);
            this.btnMB.Name = "btnMB";
            this.btnMB.Size = new System.Drawing.Size(39, 167);
            this.btnMB.TabIndex = 18;
            this.btnMB.UseVisualStyleBackColor = false;
            this.btnMB.Click += new System.EventHandler(this.btnMB_Click);
            this.btnMB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMA
            // 
            this.btnMA.BackColor = System.Drawing.Color.White;
            this.btnMA.Location = new System.Drawing.Point(447, 11);
            this.btnMA.Name = "btnMA";
            this.btnMA.Size = new System.Drawing.Size(39, 167);
            this.btnMA.TabIndex = 17;
            this.btnMA.UseVisualStyleBackColor = false;
            this.btnMA.Click += new System.EventHandler(this.btnMA_Click);
            this.btnMA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMG
            // 
            this.btnMG.BackColor = System.Drawing.Color.White;
            this.btnMG.Location = new System.Drawing.Point(410, 11);
            this.btnMG.Name = "btnMG";
            this.btnMG.Size = new System.Drawing.Size(39, 167);
            this.btnMG.TabIndex = 16;
            this.btnMG.UseVisualStyleBackColor = false;
            this.btnMG.Click += new System.EventHandler(this.btnMG_Click);
            this.btnMG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMF
            // 
            this.btnMF.BackColor = System.Drawing.Color.White;
            this.btnMF.Location = new System.Drawing.Point(373, 11);
            this.btnMF.Name = "btnMF";
            this.btnMF.Size = new System.Drawing.Size(39, 167);
            this.btnMF.TabIndex = 15;
            this.btnMF.UseVisualStyleBackColor = false;
            this.btnMF.Click += new System.EventHandler(this.btnMF_Click);
            this.btnMF.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnME
            // 
            this.btnME.BackColor = System.Drawing.Color.White;
            this.btnME.Location = new System.Drawing.Point(337, 11);
            this.btnME.Name = "btnME";
            this.btnME.Size = new System.Drawing.Size(39, 167);
            this.btnME.TabIndex = 14;
            this.btnME.UseVisualStyleBackColor = false;
            this.btnME.Click += new System.EventHandler(this.btnME_Click);
            this.btnME.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMD
            // 
            this.btnMD.BackColor = System.Drawing.Color.White;
            this.btnMD.Location = new System.Drawing.Point(301, 11);
            this.btnMD.Name = "btnMD";
            this.btnMD.Size = new System.Drawing.Size(39, 167);
            this.btnMD.TabIndex = 13;
            this.btnMD.UseVisualStyleBackColor = false;
            this.btnMD.Click += new System.EventHandler(this.btnMD_Click);
            this.btnMD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnMC
            // 
            this.btnMC.BackColor = System.Drawing.Color.White;
            this.btnMC.Location = new System.Drawing.Point(264, 11);
            this.btnMC.Name = "btnMC";
            this.btnMC.Size = new System.Drawing.Size(39, 167);
            this.btnMC.TabIndex = 12;
            this.btnMC.UseVisualStyleBackColor = false;
            this.btnMC.Click += new System.EventHandler(this.btnMC_Click);
            this.btnMC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHAs
            // 
            this.btnHAs.BackColor = System.Drawing.Color.Black;
            this.btnHAs.Location = new System.Drawing.Point(726, 11);
            this.btnHAs.Name = "btnHAs";
            this.btnHAs.Size = new System.Drawing.Size(29, 101);
            this.btnHAs.TabIndex = 35;
            this.btnHAs.UseVisualStyleBackColor = false;
            this.btnHAs.Click += new System.EventHandler(this.btnHAs_Click);
            this.btnHAs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHGs
            // 
            this.btnHGs.BackColor = System.Drawing.Color.Black;
            this.btnHGs.Location = new System.Drawing.Point(688, 11);
            this.btnHGs.Name = "btnHGs";
            this.btnHGs.Size = new System.Drawing.Size(29, 101);
            this.btnHGs.TabIndex = 34;
            this.btnHGs.UseVisualStyleBackColor = false;
            this.btnHGs.Click += new System.EventHandler(this.btnHGs_Click);
            this.btnHGs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHFs
            // 
            this.btnHFs.BackColor = System.Drawing.Color.Black;
            this.btnHFs.Location = new System.Drawing.Point(653, 11);
            this.btnHFs.Name = "btnHFs";
            this.btnHFs.Size = new System.Drawing.Size(29, 101);
            this.btnHFs.TabIndex = 33;
            this.btnHFs.UseVisualStyleBackColor = false;
            this.btnHFs.Click += new System.EventHandler(this.btnHFs_Click);
            this.btnHFs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHDs
            // 
            this.btnHDs.BackColor = System.Drawing.Color.Black;
            this.btnHDs.Location = new System.Drawing.Point(579, 11);
            this.btnHDs.Name = "btnHDs";
            this.btnHDs.Size = new System.Drawing.Size(29, 101);
            this.btnHDs.TabIndex = 32;
            this.btnHDs.UseVisualStyleBackColor = false;
            this.btnHDs.Click += new System.EventHandler(this.btnHDs_Click);
            this.btnHDs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHCs
            // 
            this.btnHCs.BackColor = System.Drawing.Color.Black;
            this.btnHCs.Location = new System.Drawing.Point(544, 11);
            this.btnHCs.Name = "btnHCs";
            this.btnHCs.Size = new System.Drawing.Size(29, 101);
            this.btnHCs.TabIndex = 31;
            this.btnHCs.UseVisualStyleBackColor = false;
            this.btnHCs.Click += new System.EventHandler(this.btnHCs_Click);
            this.btnHCs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHB
            // 
            this.btnHB.BackColor = System.Drawing.Color.White;
            this.btnHB.Location = new System.Drawing.Point(740, 11);
            this.btnHB.Name = "btnHB";
            this.btnHB.Size = new System.Drawing.Size(39, 167);
            this.btnHB.TabIndex = 30;
            this.btnHB.UseVisualStyleBackColor = false;
            this.btnHB.Click += new System.EventHandler(this.btnHB_Click);
            this.btnHB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHA
            // 
            this.btnHA.BackColor = System.Drawing.Color.White;
            this.btnHA.Location = new System.Drawing.Point(703, 11);
            this.btnHA.Name = "btnHA";
            this.btnHA.Size = new System.Drawing.Size(39, 167);
            this.btnHA.TabIndex = 29;
            this.btnHA.UseVisualStyleBackColor = false;
            this.btnHA.Click += new System.EventHandler(this.btnHA_Click);
            this.btnHA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHG
            // 
            this.btnHG.BackColor = System.Drawing.Color.White;
            this.btnHG.Location = new System.Drawing.Point(666, 11);
            this.btnHG.Name = "btnHG";
            this.btnHG.Size = new System.Drawing.Size(39, 167);
            this.btnHG.TabIndex = 28;
            this.btnHG.UseVisualStyleBackColor = false;
            this.btnHG.Click += new System.EventHandler(this.btnHG_Click);
            this.btnHG.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHF
            // 
            this.btnHF.BackColor = System.Drawing.Color.White;
            this.btnHF.Location = new System.Drawing.Point(629, 11);
            this.btnHF.Name = "btnHF";
            this.btnHF.Size = new System.Drawing.Size(39, 167);
            this.btnHF.TabIndex = 27;
            this.btnHF.UseVisualStyleBackColor = false;
            this.btnHF.Click += new System.EventHandler(this.btnHF_Click);
            this.btnHF.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHE
            // 
            this.btnHE.BackColor = System.Drawing.Color.White;
            this.btnHE.Location = new System.Drawing.Point(593, 11);
            this.btnHE.Name = "btnHE";
            this.btnHE.Size = new System.Drawing.Size(39, 167);
            this.btnHE.TabIndex = 26;
            this.btnHE.UseVisualStyleBackColor = false;
            this.btnHE.Click += new System.EventHandler(this.btnHE_Click);
            this.btnHE.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHD
            // 
            this.btnHD.BackColor = System.Drawing.Color.White;
            this.btnHD.Location = new System.Drawing.Point(557, 11);
            this.btnHD.Name = "btnHD";
            this.btnHD.Size = new System.Drawing.Size(39, 167);
            this.btnHD.TabIndex = 25;
            this.btnHD.UseVisualStyleBackColor = false;
            this.btnHD.Click += new System.EventHandler(this.btnHD_Click);
            this.btnHD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // btnHC
            // 
            this.btnHC.BackColor = System.Drawing.Color.White;
            this.btnHC.Location = new System.Drawing.Point(520, 11);
            this.btnHC.Name = "btnHC";
            this.btnHC.Size = new System.Drawing.Size(39, 167);
            this.btnHC.TabIndex = 24;
            this.btnHC.UseVisualStyleBackColor = false;
            this.btnHC.Click += new System.EventHandler(this.btnHC_Click);
            this.btnHC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
            // 
            // frmKeyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 186);
            this.Controls.Add(this.btnHAs);
            this.Controls.Add(this.btnHGs);
            this.Controls.Add(this.btnHFs);
            this.Controls.Add(this.btnHDs);
            this.Controls.Add(this.btnHCs);
            this.Controls.Add(this.btnHB);
            this.Controls.Add(this.btnHA);
            this.Controls.Add(this.btnHG);
            this.Controls.Add(this.btnHF);
            this.Controls.Add(this.btnHE);
            this.Controls.Add(this.btnHD);
            this.Controls.Add(this.btnHC);
            this.Controls.Add(this.btnMAs);
            this.Controls.Add(this.btnMGs);
            this.Controls.Add(this.btnMFs);
            this.Controls.Add(this.btnMDs);
            this.Controls.Add(this.btnMCs);
            this.Controls.Add(this.btnMB);
            this.Controls.Add(this.btnMA);
            this.Controls.Add(this.btnMG);
            this.Controls.Add(this.btnMF);
            this.Controls.Add(this.btnME);
            this.Controls.Add(this.btnMD);
            this.Controls.Add(this.btnMC);
            this.Controls.Add(this.btnLAs);
            this.Controls.Add(this.btnLGs);
            this.Controls.Add(this.btnLFs);
            this.Controls.Add(this.btnLDs);
            this.Controls.Add(this.btnLCs);
            this.Controls.Add(this.btnLB);
            this.Controls.Add(this.btnLA);
            this.Controls.Add(this.btnLG);
            this.Controls.Add(this.btnLF);
            this.Controls.Add(this.btnLE);
            this.Controls.Add(this.btnLD);
            this.Controls.Add(this.btnLC);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmKeyboard";
            this.Text = "Small Keyboard";
            this.Load += new System.EventHandler(this.frmKeyboard_Load_1);
            this.ResumeLayout(false);

		}
		internal System.Windows.Forms.Button btnLC;
		internal System.Windows.Forms.Button btnLD;
		internal System.Windows.Forms.Button btnLE;
		internal System.Windows.Forms.Button btnLF;
		internal System.Windows.Forms.Button btnLG;
		internal System.Windows.Forms.Button btnLA;
		internal System.Windows.Forms.Button btnLB;
		internal System.Windows.Forms.Button btnLCs;
		internal System.Windows.Forms.Button btnLDs;
		internal System.Windows.Forms.Button btnLFs;
		internal System.Windows.Forms.Button btnLGs;
		internal System.Windows.Forms.Button btnLAs;
		internal System.Windows.Forms.Button btnMAs;
		internal System.Windows.Forms.Button btnMGs;
		internal System.Windows.Forms.Button btnMFs;
		internal System.Windows.Forms.Button btnMDs;
		internal System.Windows.Forms.Button btnMCs;
		internal System.Windows.Forms.Button btnMB;
		internal System.Windows.Forms.Button btnMA;
		internal System.Windows.Forms.Button btnMG;
		internal System.Windows.Forms.Button btnMF;
		internal System.Windows.Forms.Button btnME;
		internal System.Windows.Forms.Button btnMD;
		internal System.Windows.Forms.Button btnMC;
		internal System.Windows.Forms.Button btnHAs;
		internal System.Windows.Forms.Button btnHGs;
		internal System.Windows.Forms.Button btnHFs;
		internal System.Windows.Forms.Button btnHDs;
		internal System.Windows.Forms.Button btnHCs;
		internal System.Windows.Forms.Button btnHB;
		internal System.Windows.Forms.Button btnHA;
		internal System.Windows.Forms.Button btnHG;
		internal System.Windows.Forms.Button btnHF;
		internal System.Windows.Forms.Button btnHE;
		internal System.Windows.Forms.Button btnHD;
		internal System.Windows.Forms.Button btnHC;
		
	}
	
}
