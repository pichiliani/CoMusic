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
		
        private Button[] aButtonWhite;
        private Button[] aButtonBlack;
        private WSounds ws;
 
        public frmKeyboard()
		{
			InitializeComponent();
		}
		
		[DllImport("KERNEL32.DLL")]
        public static  extern void Beep(int freq, int dur);


        #region Play_KeyDown
        private void Play_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{	
			this.Focus();
			
			switch (e.KeyData.ToString())
			{
				case "A":
					
					break;
				case "S":
					
					break;
				case "D":
					
					break;
				case "F":
					
					break;
				case "G":
					
					break;
				case "H":
					
					break;
				case "J":
					
					break;
				case "K":
					
					break;
				case "L":
					
					break;
				case "Z":
					
					break;
				case "X":
					
					break;
				case "C":
					
					break;
			}

        }
        #endregion


        #region frmKeyboard_Load
        private void frmKeyboard_Load(object sender, System.EventArgs e)
		{

        }
        #endregion

        #region btnMaster_Click
        private void btnMaster_Click(System.Object sender, System.EventArgs e)
        {
            // Start a new Thread to play the sound
            Button b = (Button)sender;

            // this.ws = 

            WSounds ws = new WSounds();
            

            switch (b.Name)
            {
                case "btn0":
                    ws.Play("C1.WAV", 0x0001 | 0x00002000);
                    break;
                case "btn1":
                    ws.Play("D1.WAV", 0x0001 | 0x00002000);
                    break;
                case "btn2":
                    ws.Play("E1.WAV", 0x0001);
                    break;
                case "btn3":
                    ws.Play("F1.WAV", 0x0001 );
                    break;
                case "btn4":
                    ws.Play("G1.WAV", 0x0001);
                    break;
                case "btn5":
                    ws.Play("A1.WAV", 0x0001);
                    break;
                case "btn6":
                    ws.Play("B1.WAV", 0x0001);
                    break;
                case "btn7":
                    ws.Play("C2.WAV", 0x0001);
                    break;
                case "btn8":
                    ws.Play("D2.WAV", 0x0001);
                    break;
                case "btn9":
                    ws.Play("E2.WAV", 0x0001);
                    break;
                case "btn10":
                    ws.Play("F2.WAV", 0x0001);
                    break;
                case "btn11":
                    ws.Play("G2.WAV", 0x0001);
                    break;
                case "btn12":
                    ws.Play("A2.WAV", 0x0001);
                    break;
                case "btn13":
                    ws.Play("B2.WAV", 0x0001);
                    break;
                case "btn14":
                    ws.Play("C3.WAV", 0x0001);
                    break;



                case "btnSus0":
                    ws.Play("CS1.WAV", 0x0001);
                    break;
                case "btnSus1":
                    ws.Play("DS1.WAV", 0x0001);
                    break;
                case "btnSus2":
                    ws.Play("FS1.WAV", 0x0001);
                    break;
                case "btnSus3":
                    ws.Play("GS1.WAV", 0x0001);
                    break;
                case "btnSus4":
                    ws.Play("AS1.WAV", 0x0001);
                    break;
                case "btnSus5":
                    ws.Play("CS2.WAV", 0x0001);
                    break;
                case "btnSus6":
                    ws.Play("DS2.WAV", 0x0001);
                    break;
                case "btnSus7":
                    ws.Play("FS2.WAV", 0x0001);
                    break;
                case "btnSus8":
                    ws.Play("GS2.WAV", 0x0001);
                    break;
                case "btnSus9":
                    ws.Play("A2.WAV", 0x0001);
                    break;




            }

            

            /* 
             */
             


           /* if(b.Name.Equals("btn0"))
            {
                
                // Beep(261, 150);
            } */


        }
        #endregion



        private void frmKeyboard_Load_1(object sender, EventArgs e)
        {

        }


    }



    #region WSounds
    public class WSounds
    {

        [DllImport("WinMM.dll")]

        public static extern bool PlaySound(string fname, int Mod, int flag);


        // these are the SoundFlags we are using here, check mmsystem.h for more

        public int SND_ASYNC = 0x0001; // play asynchronously

        public int SND_FILENAME = 0x00020000; // use file name

        public int SND_PURGE = 0x0040; // purge non-static events



        public void Play(string fname, int SoundFlags)
        {

            fname = "\\MP3\\PIANO\\" + fname;
            fname = Application.StartupPath + fname;

            //PlaySound(fname, 0, SoundFlags);


            System.Media.SoundPlayer sound = new System.Media.SoundPlayer(fname);
            sound.LoadAsync();
            sound.Play();
            

        }

        public void StopPlay()
        {

            PlaySound(null, 0, SND_PURGE);

        }

    }
    #endregion
}
