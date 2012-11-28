using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;
using TUIO;

namespace SmallKeyboard
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class frmKeyboard : System.Windows.Forms.Form, TuioListener
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            // this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.bntCollab = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();

            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            
            this.groupTeclado = new System.Windows.Forms.GroupBox();

            this.bntMais = new System.Windows.Forms.Button();
            this.bntMove = new System.Windows.Forms.Button();
            this.btnMaximiza = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();

            this.groupTeclado.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();


            // int start = 8;
            // int row = 11;

            int row = 5;
            int distance = 37;
            int start = 3;


            // int size = 37;
            // Size white_size = new System.Drawing.Size(39, 167);
            // Size black_size = new System.Drawing.Size(29, 101);

            
            Size white_size = new System.Drawing.Size(39, 167);
            Size black_size = new System.Drawing.Size(29, 101);


            aButtonWhite = new Button[15];
            aButtonBlack = new Button[10];
            aPictures = new  PictureBox[20];
            aTelepointers = new PictureBox[20];
            // aTelepointers = new CoKeyboard.Collab.TransPanel[20];
            aTelefingers = new PictureBox[20];
 

            #region Blobs
            // The picture boxes
            for (int i = 0; i < 20; i++)
            {

                aPictures[i] = new System.Windows.Forms.PictureBox();

                ((System.ComponentModel.ISupportInitialize)(aPictures[i])).BeginInit();

                // Esta Cor indica que o usuário não está conectado!
                aPictures[i].BackColor = clienteEnvia.getCorTelepointer();

                aPictures[i].Location = new System.Drawing.Point(200, 200);
                aPictures[i].Name = (i + 1).ToString();

                aPictures[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                aPictures[i].Size = new System.Drawing.Size(30, 30);
                aPictures[i].TabStop = false;
                aPictures[i].Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
                aPictures[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

                aPictures[i].Enabled = true;
                aPictures[i].Visible = false;
                aPictures[i].Tag = null; // Usado para armazenar um botão que estão sob o blob

                aPictures[i].BringToFront();

                this.Controls.Add(aPictures[i]);

                ((System.ComponentModel.ISupportInitialize)(aPictures[i])).EndInit();
            }
            #endregion

            #region Telepointers
            // The telepointers
            for (int i = 0; i < 20; i++)
            {

                aTelepointers[i] = new System.Windows.Forms.PictureBox();
                // aTelepointers[i] = new CoKeyboard.Collab.TransPanel();

                // ((System.ComponentModel.ISupportInitialize)(aTelepointers[i])).BeginInit();

                aTelepointers[i].BackColor = System.Drawing.Color.Transparent;
                aTelepointers[i].BackColor = System.Drawing.Color.Blue; // Mudar a cor dependendo do que veio pelo protocolo

                aTelepointers[i].Location = new System.Drawing.Point(200, 200);
                aTelepointers[i].Name = (i + 1).ToString();

                aTelepointers[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                aTelepointers[i].Size = new System.Drawing.Size(15, 15);
                aTelepointers[i].TabStop = false;
                aTelepointers[i].Paint += new System.Windows.Forms.PaintEventHandler(this.telepointer_Paint);
                aTelepointers[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
                aTelepointers[i].Enabled = true;
                aTelepointers[i].Visible = false;
                aTelepointers[i].Tag = 0; // Por enquanto sem uso

                aTelepointers[i].BringToFront();

                this.Controls.Add(aTelepointers[i]);

               // ((System.ComponentModel.ISupportInitialize)(aTelepointers[i])).EndInit();
            }

            #endregion

            #region Telefingers
            // The Telefingers
            for (int i = 0; i < 20; i++)
            {

                aTelefingers[i] = new System.Windows.Forms.PictureBox();

                ((System.ComponentModel.ISupportInitialize)(aTelefingers[i])).BeginInit();

                aTelefingers[i].BackColor = System.Drawing.Color.Transparent;
                // aTelepointers[i].BackColor = System.Drawing.Color.Blue; // Mudar a cor dependendo do que veio pelo protocolo

                aTelefingers[i].Location = new System.Drawing.Point(200, 200);
                aTelefingers[i].Name = (i + 1).ToString();

                aTelefingers[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                aTelefingers[i].Size = new System.Drawing.Size(15, 15);
                aTelefingers[i].TabStop = false;
                aTelefingers[i].Paint += new System.Windows.Forms.PaintEventHandler(this.telefinger_Paint);
                aTelefingers[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
                aTelefingers[i].Enabled = true;
                aTelefingers[i].Visible = false;
                aTelefingers[i].Tag = 0; // Por enquanto sem uso

                aTelefingers[i].BringToFront();

                this.Controls.Add(aTelefingers[i]);

                ((System.ComponentModel.ISupportInitialize)(aTelefingers[i])).EndInit();
            }

            #endregion

            #region Black keys
            // Now the black keys
            for (int i = 0; i < 10; i++)
            {
                aButtonBlack[i] = new System.Windows.Forms.Button();

                aButtonBlack[i].BackColor = System.Drawing.Color.Black;
                aButtonBlack[i].Location = new System.Drawing.Point(0, row);
                aButtonBlack[i].Name = "btnSus" + i.ToString();
                aButtonBlack[i].Size = black_size;
                aButtonBlack[i].TabStop = true;
                aButtonBlack[i].UseVisualStyleBackColor = false;
                aButtonBlack[i].Click += new System.EventHandler(this.btnMaster_Click);

                aButtonBlack[i].MouseDown += new System.Windows.Forms.MouseEventHandler(this.Master_MouseDown);
                aButtonBlack[i].MouseUp += new System.Windows.Forms.MouseEventHandler(this.Master_MouseUp);
                aButtonBlack[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMoveGroupBox);

                aButtonBlack[i].KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
                aButtonBlack[i].KeyUp += new System.Windows.Forms.KeyEventHandler(this.Play_KeyUp);

                aButtonBlack[i].Visible = true;
                aButtonBlack[i].BringToFront();
                aButtonBlack[i].Tag = 0;


                this.groupTeclado.Controls.Add(aButtonBlack[i]);

               //  this.Controls.Add(aButtonBlack[i]);
            }

            // A propriedade AccessibleDescription vai contar a tecla do teclado 
            // utilizada para tocar a nota

            aButtonBlack[0].Location = new System.Drawing.Point((start + distance) - 13, row);
            aButtonBlack[0].AccessibleDescription = "W";
            aButtonBlack[1].Location = new System.Drawing.Point((start + distance * 2) - 13, row);
            aButtonBlack[1].AccessibleDescription = "E";

            aButtonBlack[2].Location = new System.Drawing.Point((start + distance * 4) - 13, row);
            aButtonBlack[2].AccessibleDescription = "T";
            aButtonBlack[3].Location = new System.Drawing.Point((start + distance * 5) - 13, row);
            aButtonBlack[3].AccessibleDescription = "Y";
            aButtonBlack[4].Location = new System.Drawing.Point((start + distance * 6) - 13, row);
            aButtonBlack[4].AccessibleDescription = "U";

            aButtonBlack[5].Location = new System.Drawing.Point((start + distance * 8) - 13, row);
            aButtonBlack[5].AccessibleDescription = "O";
            aButtonBlack[6].Location = new System.Drawing.Point((start + distance * 9) - 13, row);
            aButtonBlack[6].AccessibleDescription = "P";

            aButtonBlack[7].Location = new System.Drawing.Point((start + distance * 11) - 13, row);
            aButtonBlack[7].AccessibleDescription = "Q";
            aButtonBlack[8].Location = new System.Drawing.Point((start + distance * 12) - 13, row);
            aButtonBlack[8].AccessibleDescription = "R";
            aButtonBlack[9].Location = new System.Drawing.Point((start + distance * 13) - 13, row);
            aButtonBlack[9].AccessibleDescription = "I";

            #endregion

            #region White Keys
            // Then the white keys
            int pos = 0;

            for (int i = 0; i < 15; i++)
            {
                aButtonWhite[i] = new System.Windows.Forms.Button();

                aButtonWhite[i].BackColor = System.Drawing.Color.White;
                aButtonWhite[i].FlatAppearance.BorderColor = System.Drawing.Color.Black;
                aButtonWhite[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                aButtonWhite[i].Location = new System.Drawing.Point(start + pos, row);
                aButtonWhite[i].Name = "btn" + i.ToString();
                aButtonWhite[i].Size = white_size;
                aButtonWhite[i].UseVisualStyleBackColor = false;
                aButtonWhite[i].Click += new System.EventHandler(this.btnMaster_Click);
                aButtonWhite[i].KeyDown += new System.Windows.Forms.KeyEventHandler(this.Play_KeyDown);
                aButtonWhite[i].KeyUp += new System.Windows.Forms.KeyEventHandler(this.Play_KeyUp);

                aButtonWhite[i].MouseDown += new System.Windows.Forms.MouseEventHandler(this.Master_MouseDown);
                aButtonWhite[i].MouseUp += new System.Windows.Forms.MouseEventHandler(this.Master_MouseUp);
                aButtonWhite[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMoveGroupBox);
                aButtonWhite[i].TabStop = false;
                aButtonWhite[i].Tag = 0;

                aButtonWhite[i].SendToBack();

                this.groupTeclado.Controls.Add(aButtonWhite[i]);
               //  this.Controls.Add(aButtonWhite[i]);
                pos = pos + distance;

            }

            #endregion

            #region Atribui tecla à nota
            aButtonWhite[0].AccessibleDescription = "A";
            aButtonWhite[1].AccessibleDescription = "S";
            aButtonWhite[2].AccessibleDescription = "D";
            aButtonWhite[3].AccessibleDescription = "F";
            aButtonWhite[4].AccessibleDescription = "G";
            aButtonWhite[5].AccessibleDescription = "H";
            aButtonWhite[6].AccessibleDescription = "J";
            aButtonWhite[7].AccessibleDescription = "K";
            aButtonWhite[8].AccessibleDescription = "L";
            aButtonWhite[9].AccessibleDescription = "Z";
            aButtonWhite[10].AccessibleDescription = "X";
            aButtonWhite[11].AccessibleDescription = "C";
            aButtonWhite[12].AccessibleDescription = "V";
            aButtonWhite[13].AccessibleDescription = "B";
            aButtonWhite[14].AccessibleDescription = "N";
            #endregion

            // 
            // groupTeclado
            // 

            this.groupTeclado.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupTeclado.Location = new System.Drawing.Point(100, 100);
            this.groupTeclado.Name = "groupTeclado";
            this.groupTeclado.Size = new System.Drawing.Size(565, 176);
            this.groupTeclado.TabIndex = 7;
            this.groupTeclado.TabStop = false;
            this.groupTeclado.Paint += new System.Windows.Forms.PaintEventHandler(this.groupTeclado_Paint);
            this.groupTeclado.MouseMove += new System.Windows.Forms.MouseEventHandler(this.groupTeclado_MouseMove);
            this.groupTeclado.MouseClick += new System.Windows.Forms.MouseEventHandler(this.groupTeclado_MouseClick);
            this.groupTeclado.Resize += new System.EventHandler(this.groupTeclado_Resize);
           

            // this.button1
            // groupBox1
            // 
            //this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(765, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(184, 200);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instruments";
            groupBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(23, 176);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(77, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.Text = "Percussion";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            radioButton3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMoveGroupBox);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(23, 103);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(53, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Guitar";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            radioButton2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMoveGroupBox);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(23, 30);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(52, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Piano";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            radioButton1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMoveGroupBox);

            // 
            // btnMaximiza
            // 
            this.btnMaximiza.Location = new System.Drawing.Point(788, 400);
            this.btnMaximiza.Name = "bntMaximiza";
            this.btnMaximiza.Size = new System.Drawing.Size(144, 25);
            this.btnMaximiza.TabIndex = 1;
            this.btnMaximiza.Text = "Maximize";
            this.btnMaximiza.UseVisualStyleBackColor = true;
            this.btnMaximiza.Click += new System.EventHandler(this.btnMaximiza_Click);
            this.btnMaximiza.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(788, 435);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(144, 25);
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "Restore defaults";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            this.btnRestore.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            
            // 
            // bntCollab
            // 
            this.bntCollab.Location = new System.Drawing.Point(788, 465);
            this.bntCollab.Name = "bntCollab";
            this.bntCollab.Size = new System.Drawing.Size(144, 25);
            this.bntCollab.TabIndex = 1;
            this.bntCollab.Text = "Start Collaboration";
            this.bntCollab.UseVisualStyleBackColor = true;
            this.bntCollab.Click += new System.EventHandler(this.bntCollab_Click);
            this.bntCollab.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(788, 495);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(144, 25);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            this.btnExit.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            // 
            // Trackbar1
            // 
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.trackBar1.Location = new System.Drawing.Point(750, 250);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(204, 42);
            this.trackBar1.TabIndex = 5;
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 5; // Valor inicial = 5


            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(830, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Sensibility:";
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);


            // 
            // bntMais
            // 
            this.bntMais.Location = new System.Drawing.Point(485, 305);
            this.bntMais.Name = "bntMais";
            this.bntMais.Size = new System.Drawing.Size(35, 28);
            this.bntMais.TabIndex = 9;
            this.bntMais.Text = "+";
            this.bntMais.UseVisualStyleBackColor = true;
            this.bntMais.Click += new System.EventHandler(this.bntMais_Click);
            this.bntMais.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            // 
            // bntMove
            // 
            this.bntMove.BackColor = System.Drawing.Color.Tan;
            this.bntMove.Location = new System.Drawing.Point(254, 133);
            this.bntMove.Name = "bntMove";
            this.bntMove.Size = new System.Drawing.Size(170, 30);
            this.bntMove.TabIndex = 15;
            this.bntMove.UseVisualStyleBackColor = false;
            this.bntMove.Click += new System.EventHandler(this.bntMove_Click);
            this.bntMove.FlatStyle = FlatStyle.Flat;
            this.bntMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Control_MouseMove);

            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.columnHeader1});
            this.listView1.LabelWrap = false;
            this.listView1.Location = new System.Drawing.Point(815, 295);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(65, 97);
            this.listView1.TabIndex = 19;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView1_ItemCheck);

            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Users";
            // 
            // Form1
            // 



            // 
            // frmKeyboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 555);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.bntCollab);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupTeclado);
            this.Controls.Add(this.bntMais);
            this.Controls.Add(this.bntMove);
            this.Controls.Add(this.btnMaximiza);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.listView1);

            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmKeyboard";
            this.Text = "CoKeyboard";
            this.Load += new System.EventHandler(this.frmKeyboard_Load_1);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKeyboard_FormClosing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmKeyboard_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmKeyboard_MouseClick);
            this.Move += new System.EventHandler(this.frmKeyboard_Move);


            this.groupTeclado.ResumeLayout(false);
            this.groupTeclado.PerformLayout();
     
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}

        private GroupBox groupBox1;
        //private RadioButton radioButton4;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Button bntCollab;
        private Button btnExit;
        private Button btnMaximiza;
        private Button btnRestore;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupTeclado;
        private System.Windows.Forms.Button bntMais;
        private System.Windows.Forms.Button bntMove;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
		
	}
	
}
