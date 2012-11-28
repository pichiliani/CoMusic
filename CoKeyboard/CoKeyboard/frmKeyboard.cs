using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using IrrKlang;
using LibSom;
using TUIO;
using TUIO_DEMO;
using CoKeyboard.Collab;

namespace SmallKeyboard
{
	public partial class frmKeyboard
	{

        public static ClienteConecta clienteEnvia;
        public static Thread tClienteEnvia; // Precisa 

        private Button[] aButtonWhite;   // Array com as teclas brancas
        private Button[] aButtonBlack;   // Array com as teclas pretas
        private PictureBox[] aPictures;  // Array com os blobs
        public PictureBox[] aTelepointers;  // Array com os Telepointers
        // public CoKeyboard.Collab.TransPanel[] aTelepointers;  

        public ArrayList aTeclados = new ArrayList(); // Teclados remotos
        public ArrayList aBotoes = new ArrayList(); // Botões remotos
        public ArrayList aJanelas = new ArrayList(); // Janelas remotas
        public ArrayList aUsers = new ArrayList(); // Todos os usuários e suas cores


        public PictureBox[] aTelefingers;  // Array com os Telefingers

        // Variável que armazenará os blobs e as teclas que se encontram pressionadas
        ArrayList aTeclaBlobs = new ArrayList();


        // Necessário para regular a freqüência de envio do telepointer!
        private int ContaMouseMove = 0;

        private int ContaTelefingerMove = 0;
        public int ContaTelefingerRemovido = 0;


        // Deletegate Genérico (Telepointer e Telefingers)
        public delegate void DelegateGenerico(ArrayList l);


        private string Instrument = "PIANO";
        ISoundEngine engine = new ISoundEngine();
        ISoundSource source;

        // As variáveis abaixo são para o uso do protocolo Tuio
        private TuioClient client;
        private Dictionary<long, TuioDemoObject> objectList;
        private Dictionary<long, TuioCursor> cursorList;

        public static int width, height;
        
        // private int window_width = 640;
        // private int window_height = 480;

        // Verificar também a propriedade ClientSize!

        private int window_width = 955;
        private int window_height = 555;

        private int window_left = 0;
        private int window_top = 0;
        private int screen_width = Screen.PrimaryScreen.Bounds.Width;
        private int screen_height = Screen.PrimaryScreen.Bounds.Height;

        private bool fullscreen;
        private bool verbose;

        SolidBrush blackBrush = new SolidBrush(Color.Black);
        SolidBrush whiteBrush = new SolidBrush(Color.White);

        SolidBrush grayBrush = new SolidBrush(Color.Gray);
        SolidBrush lightgrayBrush = new SolidBrush(Color.LightGray);

        // Variáveis utilizadas para o resize e move do groupbox
        public bool modoResise = false;
        public bool modoMove = false;
        public bool modoMoveBlob = false;
        public Point PontoMaior; // Útimo para deixar o groupbox "colado" nos controles

        private DateTime horaAnterior;


        #region frmKeyboard
        public frmKeyboard()
		{
            // Cria a variável de armazenará as informações de conexão com o servidor
            clienteEnvia = new ClienteConecta();
            clienteEnvia.Form = this;

            InitializeComponent();
            
            // Aqui carrego as variáveis com o áudio
            this.CarregaAudio();


        }
        #endregion

        #region FormLoad
        private void frmKeyboard_Load_1(object sender, EventArgs e)
        {
            // Aqui inicio a Thread que vai Capturar os eventos!
            this.StartTuio(3333); // Recebe e trata mensagens na Porta UDP 3333 por padrão
            this.Focus();

            // Posiciona os botões
            // Posicionando o botão +
            this.bntMais.Left = this.groupTeclado.Left + this.groupTeclado.Width + 1;
            this.bntMais.Top = this.groupTeclado.Top + this.groupTeclado.Height + 1;


            // Posicionando o botão bntMove
            this.bntMove.Left = this.groupTeclado.Left + 15;
            this.bntMove.Top = this.groupTeclado.Top - 30;
            this.bntMove.Width = this.groupTeclado.Width - 38;

            // Inicializando o contator para acordes
            horaAnterior = DateTime.Now;

        }
        #endregion

        #region frmKeyboard_MouseMove
        private void frmKeyboard_MouseMove(object sender, MouseEventArgs e)
        {
            
            // this.label1.Text = e.X.ToString() + " , " + e.Y.ToString();
            CheckMousePosition(e, null);

            /* 
            label1.Text = "x: " + e.X + ", y:" + e.Y;
            label2.Text = "Width:" + (e.X - this.groupBox1.Left ).ToString();
            label3.Text = "Height:" + (e.Y - this.groupBox1.Top).ToString();
            */

            // Se estiver no modo resize foi mover um transpanel a partir do 
            // left e top do groupbox1 até o ponteiro do mouse
            if (modoResise)
            {
                if (((e.X - this.groupTeclado.Left) <= 10) || ((e.Y - this.groupTeclado.Top) <= 10))
                    return;


                this.groupTeclado.SuspendLayout();

                this.groupTeclado.Tag = this.groupTeclado.Size;
                this.groupTeclado.Size = new Size(e.X - this.groupTeclado.Left, e.Y - this.groupTeclado.Top);
                this.groupTeclado.ResumeLayout();
            }

            // Se estiver no modo Move preciso colocar o quadro de acordo com 
            // a posição do ponteiro do mouse
            if (modoMove && (!modoMoveBlob)) 
            {
                this.groupTeclado.Left = e.X - (this.groupTeclado.Width / 2);
                this.groupTeclado.Top = e.Y;
            }
        }
        #endregion

        #region frmKeyboard_MouseClick
        private void frmKeyboard_MouseClick(object sender, MouseEventArgs e)
        {
            if (modoResise)
            {
                // this.BackColor = System.Drawing.SystemColors.Control;
                modoResise = false;
                
                this.bntMais.Visible = true;
                this.groupTeclado.Width = PontoMaior.X;
                this.groupTeclado.Height = PontoMaior.Y;
                this.groupTeclado.Refresh();

                // Aqui preciso mandar o novo tamanho do teclado para os outros clientes

                ArrayList list = new ArrayList();

                list.Add(clienteEnvia.getLogin()); // O login que está mandando esta mensagem

                // Mandando o novo tamanho do teclado
                list.Add(this.groupTeclado.Size);

                clienteEnvia.EnviaEvento((Object)list, "keyboardSize");
            }
            if (modoMove)
            {
                // Saindo do modo de movimentação do teclado 
                // this.BackColor = System.Drawing.SystemColors.Control;
                modoMove = false;
                modoMoveBlob = false;

                this.groupTeclado.Enabled = true;

                foreach (Button b in aButtonBlack)
                    b.Enabled = true;

                foreach (Button b in aButtonWhite)
                    b.Enabled = true;


                this.bntMove.Visible = true;
                this.bntMove.BackColor = System.Drawing.Color.Tan;
                this.groupTeclado.Refresh();

                // Acertando a posição do botão +
                this.bntMais.Visible = true;
                this.bntMais.Left = this.groupTeclado.Left + this.groupTeclado.Width + 1;
                this.bntMais.Top = this.groupTeclado.Top + this.groupTeclado.Height + 1;

                // Aqui preciso mandar a nova posição do teclado para os outros clientes

                ArrayList list = new ArrayList();

                list.Add(clienteEnvia.getLogin()); // O login que está mandando esta mensagem

                // Mandando a nova posição do teclado
                list.Add(this.groupTeclado.Location);

                clienteEnvia.EnviaEvento((Object)list, "keyboardLocation");

            }

        }
        #endregion

        #region frmKeyboard_Move
        private void frmKeyboard_Move(object sender, EventArgs e)
        {
            foreach (Form f in aJanelas)
            {
                f.Top = this.Top + int.Parse(f.AccessibleDefaultActionDescription);
                f.Left = this.Left + int.Parse(f.AccessibleDescription);
            }
        }
        #endregion

        #region ff_MouseMove
        private void ff_MouseMove(object sender, MouseEventArgs e)
        {
            // Necessário por causa das outras janelas!
            this.Focus();

            int ajusteX = int.Parse(((Form)sender).AccessibleDefaultActionDescription);
            int ajusteY = int.Parse(((Form)sender).AccessibleDescription);

            int left = e.X + ajusteX;
            int top = e.Y + ajusteY;

            this.EnviaPonteiroMouse(left, top);

            // if (toggleMouse) 
            // bntCollab.Text = "x: " + left + ", y:" + top;


            // Se estiver no modo resize foi mover um transpanel a partir do 
            // left e top do groupbox1 até o ponteiro do mouse
            if (modoResise)
            {
                if (((left - this.groupTeclado.Left) <= 10) || ((left - this.groupTeclado.Top) <= 10))
                    return;


                this.groupTeclado.SuspendLayout();

                this.groupTeclado.Tag = this.groupTeclado.Size;
                this.groupTeclado.Size = new Size(left - this.groupTeclado.Left, top - this.groupTeclado.Top);
                this.groupTeclado.ResumeLayout();
            }

            // Se estiver no modo Move preciso colocar o quadro de acordo com 
            // a posição do ponteiro do mouse
            if (modoMove)
            {
                this.groupTeclado.Left = left - (this.groupTeclado.Width / 2);
                this.groupTeclado.Top = top;
            }
        }
        #endregion]

        #region keyboardLocationImpl
        public void keyboardLocationImpl(ArrayList l)
        {
                string u = (string)l[0];
                Point p = (Point)l[1];

                if (ChecaAwarenessRemoto(u))
                {
                    // Encontrando o teclado certo para mudar a sua posição
                    foreach (GroupBox gp in aTeclados)
                    {
                        if (gp.Tag.Equals(u))
                        {
                            gp.Enabled = true;
                            gp.Location = p;
                            gp.Enabled = false;
                        }
                    }
                }
        }
        #endregion

        #region keyboardSizeImpl
        public void keyboardSizeImpl(ArrayList l)
        {
            string u = (string)l[0];
            Size s = (Size)l[1];

            if (ChecaAwarenessRemoto(u))
            {
                // Encontrando o teclado certo para mudar a sua posição
                foreach (GroupBox gp in aTeclados)
                {
                    if (gp.Tag.Equals(u))
                    {
                        gp.Enabled = true;
                        gp.Size = s;
                        gp.Enabled = false;
                    }
                }
            }
        }
        #endregion

        #region frmKeyboard_FormClosing
        private void frmKeyboard_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (tClienteEnvia != null)
                {
                    if (tClienteEnvia.IsAlive)
                        tClienteEnvia.Abort();
                }

                clienteEnvia.getClienteGT().client.Stop();
                clienteEnvia.getClienteGT().client.Dispose();

            }
            catch { }

            client.removeTuioListener(this);
            client.disconnect();

            Application.Exit();
            Application.ExitThread();
        }
        #endregion

        // Restaura a posição inicial e o tamanho do teclado
        #region btnRestore_Click
        private void btnRestore_Click(System.Object sender, System.EventArgs e)
        {
            this.groupTeclado.Location = new System.Drawing.Point(100, 100);
            this.groupTeclado.Size = new System.Drawing.Size(565, 176);

            int row = 5;
            int distance = 37;
            int start = 3;

            Size white_size = new System.Drawing.Size(39, 167);
            Size black_size = new System.Drawing.Size(29, 101);

            aButtonBlack[0].Location = new System.Drawing.Point((start + distance) - 13, row);
            aButtonBlack[1].Location = new System.Drawing.Point((start + distance * 2) - 13, row);

            aButtonBlack[2].Location = new System.Drawing.Point((start + distance * 4) - 13, row);
            aButtonBlack[3].Location = new System.Drawing.Point((start + distance * 5) - 13, row);
            aButtonBlack[4].Location = new System.Drawing.Point((start + distance * 6) - 13, row);

            aButtonBlack[5].Location = new System.Drawing.Point((start + distance * 8) - 13, row);
            aButtonBlack[6].Location = new System.Drawing.Point((start + distance * 9) - 13, row);

            aButtonBlack[7].Location = new System.Drawing.Point((start + distance * 11) - 13, row);
            aButtonBlack[8].Location = new System.Drawing.Point((start + distance * 12) - 13, row);
            aButtonBlack[9].Location = new System.Drawing.Point((start + distance * 13) - 13, row);

            for (int i = 0; i < 10; i++)
            {
                aButtonBlack[i].Size = black_size;
            }


            int pos = 0;
            for (int i = 0; i < 15; i++)
            {
                aButtonWhite[i].Location = new System.Drawing.Point(start + pos, row);
                aButtonWhite[i].Size = new System.Drawing.Size(39, 167);
                pos = pos + distance;
            }


            this.bntMove.Size = new System.Drawing.Size(170, 30);
            this.bntMais.Size = new System.Drawing.Size(35, 28);

            // Posiciona os botões
            // Posicionando o botão +
            this.bntMais.Left = this.groupTeclado.Left + this.groupTeclado.Width + 1;
            this.bntMais.Top = this.groupTeclado.Top + this.groupTeclado.Height + 1;

            // Posicionando o botão bntMove
            this.bntMove.Left = this.groupTeclado.Left + 15;
            this.bntMove.Top = this.groupTeclado.Top - 30;
            this.bntMove.Width = this.groupTeclado.Width - 38;


            // Testes com a janela transparente
            /*
            Form ff = new Form();

            ff.Opacity = 0.4;
            ff.ControlBox = false;


            ff.TopMost = true;
            ff.ShowInTaskbar = false;

            ff.Owner = this;
            ff.Show();

            ff.Enabled = true;
            ff.Visible = true;
            ff.FormBorderStyle = FormBorderStyle.None;

            Point p = new Point(this.Location.X + 100, Location.Y + 100);

            ff.Location = p;

            // Testando a inserção de controles!

            System.Windows.Forms.Button b1 = new Button();

            b1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            b1.Enabled = false;
            b1.Location = new System.Drawing.Point(50, 50);
            b1.Name = "button5";
            b1.Size = new System.Drawing.Size(75, 81);
            b1.UseVisualStyleBackColor = true;
            b1.Visible = true;
            b1.Text = "aaa";
            ff.Controls.Add(b1);

            ff.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmKeyboard_MouseClick); // Acertar
            ff.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ff_MouseMove); // Acertar

            // Acertar de acordo com a posiação da janela na tela!
            // O valor a ser colocado é a diferença de posicionamento entre as janelas (top e left)

            ff.AccessibleDefaultActionDescription = Convert.ToString(ff.Top - this.Top);
            ff.AccessibleDescription = Convert.ToString(ff.Left - this.Left);

            aJanelas.Add(ff);

            this.Focus();*/


        }
        #endregion

        // Maximiza/restaura o tamanho da tela
        #region btnMaximiza_Click
        private void btnMaximiza_Click(System.Object sender, System.EventArgs e)
        {
            if (fullscreen == false)
            {

                width = screen_width;
                height = screen_height;

                window_left = this.Left;
                window_top = this.Top;

                this.FormBorderStyle = FormBorderStyle.None;
                this.Left = 0;
                this.Top = 0;
                this.Width = screen_width;
                this.Height = screen_height;

                fullscreen = true;
                this.btnMaximiza.Text = "Restore";

                this.groupBox1.Left = this.Width - 200;
                this.trackBar1.Left = this.Width - 220;
                this.bntCollab.Left = this.Width - 172;
                this.btnExit.Left = this.Width - 172;
                this.btnMaximiza.Left = this.Width - 172;
                this.btnRestore.Left = this.Width - 172;
                this.listView1.Left = this.Width - 150;

                this.label1.Left = this.Width - 140;

            }
            else
            {
                width = window_width;
                height = window_height;

                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Left = window_left;
                this.Top = window_top;
                this.Width = window_width;
                this.Height = window_height;

                fullscreen = false;
                this.btnMaximiza.Text = "Maximize";

                this.groupBox1.Left = this.Width - 200;
                this.trackBar1.Left = this.Width - 220;
                this.bntCollab.Left = this.Width - 172;
                this.btnExit.Left = this.Width - 172;
                this.btnMaximiza.Left = this.Width - 172;
                this.btnRestore.Left = this.Width - 172;
                this.listView1.Left = this.Width - 150;

                this.label1.Left = this.Width - 140;
            }

            // Testes de inclusão de um novo teclado remoto!
        }

        #endregion

        // Criar um teclado "fantasma" para representar o teclaro remoto dentre outras funções
        #region AddRemoteKeyboard
        public void AddRemoteKeyboard(ArrayList a)
        {

            string u = (string)a[0];
            string user_cor = (string)a[1];

            // Nao cria um teclado para si mesmo!
            if (u.Equals(clienteEnvia.getLogin()))
                return;

            // Aqui adiciono o usuário e sua cor no ArrayList de usuários!
            // O armazenamento será feito em um ArrayList que contém arrays de strings (string[])
            string[] info = new string[2];
            info[0] = u;
            info[1] = user_cor;

            aUsers.Add(info);

            // Adicionando o usuário no listview
            System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(new string[] {
            u.Trim()}, -1, System.Drawing.SystemColors.WindowText, Color.FromName(user_cor), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            listViewItem.StateImageIndex = 0;
            listViewItem.Checked = true;
            listViewItem.Tag = u.Trim();
            this.listView1.Items.Add(listViewItem);
            this.listView1.Refresh();

            Button[] aButtonWhiteRemote = new Button[15];   // Array com as teclas brancas
            Button[] aButtonBlackRemote = new Button[10];   // Array com as teclas pretas

            aBotoes.Add(aButtonWhiteRemote);
            aBotoes.Add(aButtonBlackRemote);

            int row = 15;
            int distance = 37;
            int start = 3;
            Size white_size = new System.Drawing.Size(39, 167);
            Size black_size = new System.Drawing.Size(29, 101);


            // Primeiro o GroupBox
            System.Windows.Forms.GroupBox gTecladoRemoto  = new GroupBox();
            gTecladoRemoto.SuspendLayout();


            // Teclas Pretas
            #region Black keys
            // Now the black keys
            for (int i = 0; i < 10; i++)
            {
                aButtonBlackRemote[i] = new System.Windows.Forms.Button();

                aButtonBlackRemote[i].BackColor = Color.FromArgb(30, System.Drawing.Color.Black);

                aButtonBlackRemote[i].Name = "btnSus" + i.ToString();
                aButtonBlackRemote[i].Size = black_size;
                aButtonBlackRemote[i].TabStop = true;
                aButtonBlackRemote[i].UseVisualStyleBackColor = false;

                aButtonBlackRemote[i].Enabled = false;
                aButtonBlackRemote[i].Visible = true;
                aButtonBlackRemote[i].SendToBack() ;

                gTecladoRemoto.Controls.Add(aButtonBlackRemote[i]);

                //  this.Controls.Add(aButtonBlack[i]);
            }

            // A propriedade AccessibleDescription vai contar a tecla do teclado 
            // utilizada para tocar a nota

            aButtonBlackRemote[0].Location = new System.Drawing.Point((start + distance) - 13, row);
            aButtonBlackRemote[0].AccessibleDescription = "W";
            aButtonBlackRemote[0].Tag = "CS1.WAV";

            aButtonBlackRemote[1].Location = new System.Drawing.Point((start + distance * 2) - 13, row);
            aButtonBlackRemote[1].AccessibleDescription = "E";
            aButtonBlackRemote[1].Tag = "DS1.WAV";
            
            aButtonBlackRemote[2].Location = new System.Drawing.Point((start + distance * 4) - 13, row);
            aButtonBlackRemote[2].AccessibleDescription = "T";
            aButtonBlackRemote[2].Tag = "FS1.WAV";

            aButtonBlackRemote[3].Location = new System.Drawing.Point((start + distance * 5) - 13, row);
            aButtonBlackRemote[3].AccessibleDescription = "Y";
            aButtonBlackRemote[3].Tag = "GS1.WAV";

            aButtonBlackRemote[4].Location = new System.Drawing.Point((start + distance * 6) - 13, row);
            aButtonBlackRemote[4].AccessibleDescription = "U";
            aButtonBlackRemote[4].Tag = "AS1.WAV";

            aButtonBlackRemote[5].Location = new System.Drawing.Point((start + distance * 8) - 13, row);
            aButtonBlackRemote[5].AccessibleDescription = "O";
            aButtonBlackRemote[5].Tag = "CS2.WAV";

            aButtonBlackRemote[6].Location = new System.Drawing.Point((start + distance * 9) - 13, row);
            aButtonBlackRemote[6].AccessibleDescription = "P";
            aButtonBlackRemote[6].Tag = "DS2.WAV";
            
            aButtonBlackRemote[7].Location = new System.Drawing.Point((start + distance * 11) - 13, row);
            aButtonBlackRemote[7].AccessibleDescription = "Q";
            aButtonBlackRemote[7].Tag = "FS2.WAV";

            aButtonBlackRemote[8].Location = new System.Drawing.Point((start + distance * 12) - 13, row);
            aButtonBlackRemote[8].AccessibleDescription = "R";
            aButtonBlackRemote[8].Tag = "GS2.WAV";

            aButtonBlackRemote[9].Location = new System.Drawing.Point((start + distance * 13) - 13, row);
            aButtonBlackRemote[9].AccessibleDescription = "I";
            aButtonBlackRemote[9].Tag = "AS2.WAV";

            #endregion

            #region White Keys
            // Then the white keys
            int pos = 0;

            for (int i = 0; i < 15; i++)
            {
                aButtonWhiteRemote[i] = new System.Windows.Forms.Button();

                aButtonWhiteRemote[i].BackColor = Color.FromArgb(5, Color.White);

                aButtonWhiteRemote[i].FlatAppearance.BorderColor = System.Drawing.Color.Black;
                aButtonWhiteRemote[i].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                aButtonWhiteRemote[i].Location = new System.Drawing.Point(start + pos, row);
                aButtonWhiteRemote[i].Name = "btn" + i.ToString();
                aButtonWhiteRemote[i].Size = white_size;
                aButtonWhiteRemote[i].UseVisualStyleBackColor = false;
                aButtonWhiteRemote[i].Enabled = false;

                aButtonWhiteRemote[i].TabStop = false;

                aButtonWhiteRemote[i].SendToBack();

                gTecladoRemoto.Controls.Add(aButtonWhiteRemote[i]);
                //  this.Controls.Add(aButtonWhite[i]);
                pos = pos + distance;

            }

            aButtonWhiteRemote[0].Tag = "C1.WAV";
            aButtonWhiteRemote[1].Tag = "D1.WAV";
            aButtonWhiteRemote[2].Tag = "E1.WAV";
            aButtonWhiteRemote[3].Tag = "F1.WAV";
            aButtonWhiteRemote[4].Tag = "G1.WAV";
            aButtonWhiteRemote[5].Tag = "A1.WAV";
            aButtonWhiteRemote[6].Tag = "B1.WAV";
            aButtonWhiteRemote[7].Tag = "C2.WAV";
            aButtonWhiteRemote[8].Tag = "D2.WAV";
            aButtonWhiteRemote[9].Tag = "E2.WAV";
            aButtonWhiteRemote[10].Tag = "F2.WAV";
            aButtonWhiteRemote[11].Tag = "G2.WAV";
            aButtonWhiteRemote[12].Tag = "A2.WAV";
            aButtonWhiteRemote[13].Tag = "B2.WAV";
            aButtonWhiteRemote[14].Tag = "C3.WAV";
            
            #endregion

            #region Atribui tecla à nota
            aButtonWhiteRemote[0].AccessibleDescription = "A";
            aButtonWhiteRemote[1].AccessibleDescription = "S";
            aButtonWhiteRemote[2].AccessibleDescription = "D";
            aButtonWhiteRemote[3].AccessibleDescription = "F";
            aButtonWhiteRemote[4].AccessibleDescription = "G";
            aButtonWhiteRemote[5].AccessibleDescription = "H";
            aButtonWhiteRemote[6].AccessibleDescription = "J";
            aButtonWhiteRemote[7].AccessibleDescription = "K";
            aButtonWhiteRemote[8].AccessibleDescription = "L";
            aButtonWhiteRemote[9].AccessibleDescription = "Z";
            aButtonWhiteRemote[10].AccessibleDescription = "X";
            aButtonWhiteRemote[11].AccessibleDescription = "C";
            aButtonWhiteRemote[12].AccessibleDescription = "V";
            aButtonWhiteRemote[13].AccessibleDescription = "B";
            aButtonWhiteRemote[14].AccessibleDescription = "N";
            #endregion

            // 
            // groupTeclado
            // 
            #region groupTeclado
            gTecladoRemoto.ForeColor = System.Drawing.SystemColors.ControlText;
            gTecladoRemoto.Location = new System.Drawing.Point(100, 100); // Coloca na posição inicial igual ao do teclado original
            gTecladoRemoto.Name = "gTecladoRemoto" + u;
            gTecladoRemoto.Tag = u;
            gTecladoRemoto.Size = new System.Drawing.Size(565, 186);
            gTecladoRemoto.TabStop = false;
            gTecladoRemoto.Enabled = false;
            gTecladoRemoto.MouseMove += new System.Windows.Forms.MouseEventHandler(this.groupTeclado_MouseMove);
            gTecladoRemoto.Resize += new System.EventHandler(this.groupTecladoRemoto_Resize);
            gTecladoRemoto.Paint += new System.Windows.Forms.PaintEventHandler(this.groupTecladoRemoto_Paint);

            gTecladoRemoto.Text = "User " + u + " keyboard";

            // Utilizando estas duas propriedades para armazenar o tamanho do controle para o evento resize
            gTecladoRemoto.AccessibleDefaultActionDescription = gTecladoRemoto.Width.ToString();
            gTecladoRemoto.AccessibleDescription = gTecladoRemoto.Height.ToString();


            this.Controls.Add(gTecladoRemoto);
            gTecladoRemoto.ResumeLayout(false);
            #endregion

            // Agora adiciona no array de teclados

            aTeclados.Add(gTecladoRemoto);
        }

        #endregion

        #region ChecaAwarenessRemoto
        private bool ChecaAwarenessRemoto(string u)
        {
            bool ret = true;

            foreach(ListViewItem li in listView1.Items)
            {
                if(li.Tag.Equals(u.Trim()))
                {
                    if (li.Checked)
                        return true;
                    else
                        return false;

                }

            }
            return ret;

        }
        #endregion

        #region CarregaAudio
        private void CarregaAudio()
        {

            // Piano
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.A1, "PIANO_A1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.A2, "PIANO_A2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.AS1, "PIANO_AS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.AS2, "PIANO_AS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.B1, "PIANO_B1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.B2, "PIANO_B2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.C1, "PIANO_C1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.C2, "PIANO_C2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.C3, "PIANO_C3.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.CS1, "PIANO_CS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.CS2, "PIANO_CS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.D1, "PIANO_D1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.D2, "PIANO_D2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.DS1, "PIANO_DS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.DS2, "PIANO_DS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.E1, "PIANO_E1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.E2, "PIANO_E2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.F1, "PIANO_F1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.F2, "PIANO_F2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.FS1, "PIANO_FS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.FS2, "PIANO_FS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.G1, "PIANO_G1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.G2, "PIANO_G2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.GS1, "PIANO_GS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPiano.GS2, "PIANO_GS2.WAV");

           // Guitar
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.A1, "GUITAR_A1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.A2, "GUITAR_A2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.AS1, "GUITAR_AS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.AS2, "GUITAR_AS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.B1, "GUITAR_B1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.B2, "GUITAR_B2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.C1, "GUITAR_C1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.C2, "GUITAR_C2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.C3, "GUITAR_C3.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.CS1, "GUITAR_CS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.CS2, "GUITAR_CS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.D1, "GUITAR_D1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.D2, "GUITAR_D2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.DS1, "GUITAR_DS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.DS2, "GUITAR_DS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.E1, "GUITAR_E1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.E2, "GUITAR_E2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.F1, "GUITAR_F1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.F2, "GUITAR_F2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.FS1, "GUITAR_FS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.FS2, "GUITAR_FS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.G1, "GUITAR_G1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.G2, "GUITAR_G2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.GS1, "GUITAR_GS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioGuitar.GS2, "GUITAR_GS2.WAV");


           // Percussion
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.A1, "PERCUSSION_A1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.A2, "PERCUSSION_A2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.AS1, "PERCUSSION_AS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.AS2, "PERCUSSION_AS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.B1, "PERCUSSION_B1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.B2, "PERCUSSION_B2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.C1, "PERCUSSION_C1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.C2, "PERCUSSION_C2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.C3, "PERCUSSION_C3.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.CS1, "PERCUSSION_CS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.CS2, "PERCUSSION_CS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.D1, "PERCUSSION_D1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.D2, "PERCUSSION_D2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.DS1, "PERCUSSION_DS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.DS2, "PERCUSSION_DS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.E1, "PERCUSSION_E1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.E2, "PERCUSSION_E2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.F1, "PERCUSSION_F1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.F2, "PERCUSSION_F2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.FS1, "PERCUSSION_FS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.FS2, "PERCUSSION_FS2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.G1, "PERCUSSION_G1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.G2, "PERCUSSION_G2.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.GS1, "PERCUSSION_GS1.WAV");
           source = engine.AddSoundSourceFromMemory(LibSom.AudioPercussion.GS2, "PERCUSSION_GS2.WAV"); 


            // Vioin

/*           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin1.A1 , "VIOLIN_A1.WAV"); 
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin1.A2 , "VIOLIN_A2.WAV"); 
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin1.AS1 , "VIOLIN_AS1.WAV"); 
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin1.AS2 , "VIOLIN_AS2.WAV"); 
           
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin2.B1 , "VIOLIN_B1.WAV"); 
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin2.B2 , "VIOLIN_B2.WAV");  
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin2.C1 , "VIOLIN_C1.WAV");  
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin2.C2 , "VIOLIN_C2.WAV");  

           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin3.C3 , "VIOLIN_C3.WAV");  
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin3.CS1 , "VIOLIN_CS1.WAV");  
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin3.CS2 , "VIOLIN_CS2.WAV");  
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin3.D1 , "VIOLIN_D1.WAV");  

           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin4.D2 , "VIOLIN_D2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin4.DS1 , "VIOLIN_DS1.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin4.DS2 , "VIOLIN_DS2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin4.E1 , "VIOLIN_E1.WAV");   

           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin5.E2  , "VIOLIN_E2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin5.F1  , "VIOLIN_F1.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin5.F2  , "VIOLIN_F2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin5.FS1  , "VIOLIN_FS1.WAV");   
           
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin6.FS2 , "VIOLIN_FS2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin6.G1 , "VIOLIN_F1.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin6.G2 , "VIOLIN_F2.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin6.GS1 , "VIOLIN_GS1.WAV");   
           source = engine.AddSoundSourceFromMemory(LibSom.AudioViolin6.GS2 , "VIOLIN_GS2.WAV");   
            */

        }
        #endregion 

        #region Play_KeyUp
        private void Play_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Refactoring: colocando a lógica de qual tecla tocar em outra função 
            // para modularizar a execução
            
            TocarTeclaNota(e.KeyData.ToString(), 1);  // Um significa KeyUP

        }
        #endregion

        #region Play_KeyDown
        private void Play_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{	

            // Refactoring: colocando a lógica de qual tecla tocar em outra função 
            // para modularizar a execução

            TocarTeclaNota(e.KeyData.ToString(),0);  // Zero significa KeySown

            // Sair da aplicação com a tecla ESC
            if (e.KeyData == Keys.Escape)
            {
                this.Close();

            } 

        }
        #endregion

        #region TocarTeclaNota
        private void TocarTeclaNota(string tecla,int tipo) // 0=keydown, 1=keyup
        {
            switch (tecla)
            {

                #region Teclas Brancas
                case "A":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[0].Tag.Equals(0))
                            aButtonWhite[0].Tag = 1;
                        else
                            return;

                        aButtonWhite[0].BackColor = Color.RoyalBlue;
                        this.Play("C1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[0].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[0].BackColor = Color.White;
                            aButtonWhite[0].Tag = 0;
                        }

                    }
                    break;
                case "S":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[1].Tag.Equals(0))
                            aButtonWhite[1].Tag = 1;
                        else
                            return;

                        aButtonWhite[1].BackColor = Color.RoyalBlue;
                        this.Play("D1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[1].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[1].BackColor = Color.White;
                            aButtonWhite[1].Tag = 0;
                        }

                    }
                    break;
                case "D":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[2].Tag.Equals(0))
                            aButtonWhite[2].Tag = 1;
                        else
                            return;

                        aButtonWhite[2].BackColor = Color.RoyalBlue;
                        this.Play("E1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[2].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[2].BackColor = Color.White;
                            aButtonWhite[2].Tag = 0;
                        }
                    }
                    break;
                case "F":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[3].Tag.Equals(0))
                            aButtonWhite[3].Tag = 1;
                        else
                            return;

                        aButtonWhite[3].BackColor = Color.RoyalBlue;
                        this.Play("F1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[3].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[3].BackColor = Color.White;
                            aButtonWhite[3].Tag = 0;
                        }

                    }
                    break;
                case "G":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[4].Tag.Equals(0))
                            aButtonWhite[4].Tag = 1;
                        else
                            return;

                        aButtonWhite[4].BackColor = Color.RoyalBlue;
                        this.Play("G1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[4].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[4].BackColor = Color.White;
                            aButtonWhite[4].Tag = 0;
                        }
                    }
                    break;
                case "H":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[5].Tag.Equals(0))
                            aButtonWhite[5].Tag = 1;
                        else
                            return;

                        aButtonWhite[5].BackColor = Color.RoyalBlue;
                        this.Play("A1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[5].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[5].BackColor = Color.White;
                            aButtonWhite[5].Tag = 0;
                        }
                    }
                    break;
                case "J":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[6].Tag.Equals(0))
                            aButtonWhite[6].Tag = 1;
                        else
                            return;

                        aButtonWhite[6].BackColor = Color.RoyalBlue;
                        this.Play("B1.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[6].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[6].BackColor = Color.White;
                            aButtonWhite[6].Tag = 0;
                        }

                    }
                    break;
                case "K":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[7].Tag.Equals(0))
                            aButtonWhite[7].Tag = 1;
                        else
                            return;

                        aButtonWhite[7].BackColor = Color.RoyalBlue;
                        this.Play("C2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[7].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[7].BackColor = Color.White;
                            aButtonWhite[7].Tag = 0;
                        }
                    }
                    break;
                case "L":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[8].Tag.Equals(0))
                            aButtonWhite[8].Tag = 1;
                        else
                            return;

                        aButtonWhite[8].BackColor = Color.RoyalBlue;
                        this.Play("D2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[8].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[8].BackColor = Color.White;
                            aButtonWhite[8].Tag = 0;
                        }


                    }
                    break;
                case "Z":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[9].Tag.Equals(0))
                            aButtonWhite[9].Tag = 1;
                        else
                            return;

                        aButtonWhite[9].BackColor = Color.RoyalBlue;
                        this.Play("E2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[9].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[9].BackColor = Color.White;
                            aButtonWhite[9].Tag = 0;
                        }

                    }
                    break;
                case "X":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[10].Tag.Equals(0))
                            aButtonWhite[10].Tag = 1;
                        else
                            return;

                        aButtonWhite[10].BackColor = Color.RoyalBlue;
                        this.Play("F2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[10].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[10].BackColor = Color.White;
                            aButtonWhite[10].Tag = 0;
                        }

                    }
                    break;
                case "C":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[11].Tag.Equals(0))
                            aButtonWhite[11].Tag = 1;
                        else
                            return;

                        aButtonWhite[11].BackColor = Color.RoyalBlue;
                        this.Play("G2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[11].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[11].BackColor = Color.White;
                            aButtonWhite[11].Tag = 0;
                        }

                    }
                    break;

                case "V":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[12].Tag.Equals(0))
                            aButtonWhite[12].Tag = 1;
                        else
                            return;

                        aButtonWhite[12].BackColor = Color.RoyalBlue;
                        this.Play("A2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[12].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[12].BackColor = Color.White;
                            aButtonWhite[12].Tag = 0;
                        }
                    }
                    break;

                case "B":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[13].Tag.Equals(0))
                            aButtonWhite[13].Tag = 1;
                        else
                            return;

                        aButtonWhite[13].BackColor = Color.RoyalBlue;
                        this.Play("B2.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[13].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[13].BackColor = Color.White;
                            aButtonWhite[13].Tag = 0;
                        }
                    }
                    break;

                case "N":
                    if (tipo.Equals(0))
                    {
                        if (aButtonWhite[14].Tag.Equals(0))
                            aButtonWhite[14].Tag = 1;
                        else
                            return;

                        aButtonWhite[14].BackColor = Color.RoyalBlue;
                        this.Play("C3.WAV");
                    }
                    else
                    {
                        if (aButtonWhite[14].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonWhite[14].BackColor = Color.White;
                            aButtonWhite[14].Tag = 0;
                        }

                    }
                    break;
                #endregion

                #region Teclas Pretas

                case "W":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[0].Tag.Equals(0))
                            aButtonBlack[0].Tag = 1;
                        else
                            return;

                        aButtonBlack[0].BackColor = Color.RoyalBlue;
                        this.Play("CS1.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[0].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[0].BackColor = Color.Black;
                            aButtonBlack[0].Tag = 0;
                        }

                    }
                    break;
                case "E":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[1].Tag.Equals(0))
                            aButtonBlack[1].Tag = 1;
                        else
                            return;

                        aButtonBlack[1].BackColor = Color.RoyalBlue;
                        this.Play("DS1.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[1].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[1].BackColor = Color.Black;
                            aButtonBlack[1].Tag = 0;
                        }

                    }
                    break;

                case "T":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[2].Tag.Equals(0))
                            aButtonBlack[2].Tag = 1;
                        else
                            return;

                        aButtonBlack[2].BackColor = Color.RoyalBlue;
                        this.Play("FS1.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[2].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[2].BackColor = Color.Black;
                            aButtonBlack[2].Tag = 0;
                        }

                    }
                    break;

                case "Y":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[3].Tag.Equals(0))
                            aButtonBlack[3].Tag = 1;
                        else
                            return;

                        aButtonBlack[3].BackColor = Color.RoyalBlue;
                        this.Play("GS1.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[3].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[3].BackColor = Color.Black;
                            aButtonBlack[3].Tag = 0;
                        }

                    }
                    break;

                case "U":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[4].Tag.Equals(0))
                            aButtonBlack[4].Tag = 1;
                        else
                            return;

                        aButtonBlack[4].BackColor = Color.RoyalBlue;
                        this.Play("AS1.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[4].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[4].BackColor = Color.Black;
                            aButtonBlack[4].Tag = 0;
                        }

                    }
                    break;

                case "O":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[5].Tag.Equals(0))
                            aButtonBlack[5].Tag = 1;
                        else
                            return;

                        aButtonBlack[5].BackColor = Color.RoyalBlue;
                        this.Play("CS2.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[5].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[5].BackColor = Color.Black;
                            aButtonBlack[5].Tag = 0;
                        }

                    }
                    break;

                case "P":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[6].Tag.Equals(0))
                            aButtonBlack[6].Tag = 1;
                        else
                            return;

                        aButtonBlack[6].BackColor = Color.RoyalBlue;
                        this.Play("DS2.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[6].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[6].BackColor = Color.Black;
                            aButtonBlack[6].Tag = 0;
                        }

                    }
                    break;
                
                case "Q":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[7].Tag.Equals(0))
                            aButtonBlack[7].Tag = 1;
                        else
                            return;

                        aButtonBlack[7].BackColor = Color.RoyalBlue;
                        this.Play("FS2.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[7].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[7].BackColor = Color.Black;
                            aButtonBlack[7].Tag = 0;
                        }

                    }
                    break;

                case "R":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[8].Tag.Equals(0))
                            aButtonBlack[8].Tag = 1;
                        else
                            return;

                        aButtonBlack[8].BackColor = Color.RoyalBlue;
                        this.Play("GS2.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[8].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[8].BackColor = Color.Black;
                            aButtonBlack[8].Tag = 0;
                        }
                    }
                    break;

                case "I":
                    if (tipo.Equals(0))
                    {
                        if (aButtonBlack[9].Tag.Equals(0))
                            aButtonBlack[9].Tag = 1;
                        else
                            return;

                        aButtonBlack[9].BackColor = Color.RoyalBlue;
                        this.Play("AS2.WAV");
                    }
                    else
                    {
                        if (aButtonBlack[9].Tag.Equals(0))
                            return;
                        else
                        {
                            aButtonBlack[9].BackColor = Color.Black;
                            aButtonBlack[9].Tag = 0;
                        }
                    }
                    break;

                #endregion
            }

        }
        #endregion
        
        #region Master_MouseDown
        private void Master_MouseDown(System.Object sender, System.EventArgs e)
        {
          
            Button b = (Button)sender;

            if (b.Tag.Equals(0))
                b.Tag = 1;
            else
                return;

            b.BackColor = Color.RoyalBlue;

        }
        #endregion
        
        #region Master_MouseUp
        private void Master_MouseUp(System.Object sender, System.EventArgs e)
        {
            Button b = (Button)sender;

            if( b.Name.ToString().IndexOf("s") > 0 )
                b.BackColor = Color.Black;
            else
                b.BackColor = Color.White;
        }
        #endregion

        // O evento onMouseMove() será utilizado para caputar a posição do mouse no form!    
        #region onMouseMove()
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // dbg.WriteLine("----- OnMouseMove -----" + e.X.ToString() + " " + e.Y.ToString());   
            base.OnMouseMove(e);

            CheckMousePosition(e, null);
        }

        #endregion

        // Este método será utilizado para capturar o mouse move de todos os controles menos do formulátio!
        #region Control_MouseMove
        protected void Control_MouseMove(object sender, MouseEventArgs e)
        {
            CheckMousePosition(e, sender);
        }
        #endregion 
        
        // Este método é diferente por quê preciso somar a posição dos elementos d groupbox com a posição do form
        #region Control_MouseMoveGroupBox
        protected void Control_MouseMoveGroupBox(object sender, MouseEventArgs e)
        {

            Control c =  new Control();
            Control o = (Control)sender;

            if(groupBox1.Contains(o))
            {
                c.Left = this.groupBox1.Left;
                c.Top = this.groupBox1.Top;
            }

            if(groupTeclado.Contains(o))
            {
                c.Left = this.groupTeclado.Left;
                c.Top = this.groupTeclado.Top;
            }

            MouseEventArgs me = new MouseEventArgs(MouseButtons.Left, 1, o.Left + e.X , o.Top + e.Y, 0);

            // Chamada para verificar o move ou o resize
            groupTeclado_MouseMove(sender, e);
        

            CheckMousePosition(me, c);
        }
        #endregion 
        
        // Este método envia a posição do ponteiro do mouse para o servidor de colaboração
        #region CheckMousePosition
        private void CheckMousePosition(MouseEventArgs e, object control)
        {
            int left;
            int top;

            if (control == null)
            {
                left = e.X;
                top = e.Y;
            }
            else
            {
                left = e.X + ((Control)control).Left;
                top = e.Y + ((Control)control).Top;
            }

            this.EnviaPonteiroMouse(left, top);

           // this.bntCollab.Text = "X:" + left.ToString() + " Y:" + top.ToString();

           

        }
        #endregion

        #region EnviaPonteiroMouse
        public void EnviaPonteiroMouse(int left, int top)
        {

            // Enviando a posição do cursor do mouse!
            ContaMouseMove++;

            if (ContaMouseMove.Equals(4))
            {
                ArrayList list = new ArrayList();

                list.Add(new Point(left, top)); // objeto 'genérico' (pode ser um mouse event ou outro)

                // Mandando a cor atual do telepointer
                list.Add(clienteEnvia.getCorTelepointer());

                // 	Mandando id do telepointer
                list.Add(clienteEnvia.getIdTelepointer());

                // Mandando o nome do proprietario do telepointer
                list.Add(clienteEnvia.getLogin());

                // this.label1.Text = "Enviado:" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond.ToString() ;

                // Este evento eh para reproduzir o telepointer
                // clienteEnvia.EnviaEvento((Object)list, "mouseMovedPointer");

                clienteEnvia.EnviaEventoGT((Object)list, "mouseMovedPointer");

                ContaMouseMove = 0;

                if (clienteEnvia.getEnvioOK())
                {
                    // ContaTelefingerMove++;
                    // this.label1.Text = "Remoções enviadas:" + ContaTelefingerMove.ToString();
                }
            }


        }
        #endregion

        #region OnMouseMoveImpl()
        public void OnMouseMoveImpl(ArrayList l)
        {
            // this.bntCollab.Text = "Recebido:" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond.ToString();

            /*
            dados_rec[0] = Point
            dados_rec[1] = Color
            dados_rec[2] = Id
            dados_rec[3] = User_name */
            string u = (String) l[3];


            if (ChecaAwarenessRemoto(u))
            {
                int pos = int.Parse(l[2].ToString());
                Point p = (Point)l[0];

                this.aTelepointers[pos].Visible = true;
                this.aTelepointers[pos].Location = p;
                this.aTelepointers[pos].BackColor = (Color)l[1];
                this.aTelepointers[pos].Name = (String)l[3];
            }

            // this.bntCollab.Text = "Recebidas:" + ContaTelefingerRemovido.ToString();

        }
        #endregion 

        #region btnMaster_Click
        private void btnMaster_Click(System.Object sender, System.EventArgs e)
        {

            // Start a new Thread to play the sound
            Button b = (Button)sender;

            if (b.Tag.Equals(0))
                return;
            else
            {

                switch (b.Name)
                {
                    // Teclas normais
                    case "btn0":
                        this.Play("C1.WAV");
                        break;
                    case "btn1":
                        this.Play("D1.WAV");
                        break;
                    case "btn2":
                        this.Play("E1.WAV");
                        break;
                    case "btn3":
                        this.Play("F1.WAV");
                        break;
                    case "btn4":
                        this.Play("G1.WAV");
                        break;
                    case "btn5":
                        this.Play("A1.WAV");
                        break;
                    case "btn6":
                        this.Play("B1.WAV");
                        break;
                    case "btn7":
                        this.Play("C2.WAV");
                        break;
                    case "btn8":
                        this.Play("D2.WAV");
                        break;
                    case "btn9":
                        this.Play("E2.WAV");
                        break;
                    case "btn10":
                        this.Play("F2.WAV");
                        break;
                    case "btn11":
                        this.Play("G2.WAV");
                        break;
                    case "btn12":
                        this.Play("A2.WAV");
                        break;
                    case "btn13":
                        this.Play("B2.WAV");
                        break;
                    case "btn14":
                        this.Play("C3.WAV");
                        break;


                    // Aqui são os sustenidos
                    case "btnSus0":
                        this.Play("CS1.WAV");
                        break;
                    case "btnSus1":
                        this.Play("DS1.WAV");
                        break;
                    case "btnSus2":
                        this.Play("FS1.WAV");
                        break;
                    case "btnSus3":
                        this.Play("GS1.WAV");
                        break;
                    case "btnSus4":
                        this.Play("AS1.WAV");
                        break;
                    case "btnSus5":
                        this.Play("CS2.WAV");
                        break;
                    case "btnSus6":
                        this.Play("DS2.WAV");
                        break;
                    case "btnSus7":
                        this.Play("FS2.WAV");
                        break;
                    case "btnSus8":
                        this.Play("GS2.WAV");
                        break;
                    case "btnSus9":
                        this.Play("AS2.WAV");
                        break;
                }

                b.Tag = 0;
            }         

        }
        #endregion

        #region Play()
        private void Play(string file)
        {


            // TODO: Verificação do acorde: se a diferença entre o tempo das notas
            // foi de mais de 200 milisegundos coloca no buffer. Caso contrário
            // manda na hora e não coloca no buffer

            // É preciso adicionar um timer de 100 milisengundos que verifique
            // a diferença entre os tempos, mandar o buffer e limpá-lo
             /* this.label1.Text = DateTime.Now.Subtract(horaAnterior).ToString();

            // Se for mais de um segundo
             if (DateTime.Now.Subtract(horaAnterior) > TimeSpan.FromMilliseconds(200))
             {
                 // Não coloca no buffer e manda 
                this.bntCollab.Text = ">1 seg.";
             }
             else
             {
                // Coloca no buffer, pois será mandado pelo timer
                this.bntCollab.Text = "<1 seg.";
             } */
            
            // Atualizando 
            horaAnterior = DateTime.Now;

            // Mandando a nota 'solo'

            clienteEnvia.EnviaEvento(file + ";" + this.Instrument + ";" + clienteEnvia.getLogin(), "PlayNote");

            string fname = "\\MP3\\" + Instrument+ "\\" + file;
            fname = Application.StartupPath + fname;

            engine.SoundVolume = 0.4f;
            ISound music = engine.Play2D(Instrument.ToString() + "_" + file.ToUpper()); 
            

            // engine.Play2D(fname);
        }
        #endregion

        #region PlayImpl
        public void PlayImpl(ArrayList dados)
        {
            string file = (string) dados[0];
            string i = (string)dados[1];
            string user = (string)dados[2];

            if (ChecaAwarenessRemoto(user))
            {
                // Muda a cor da tecla
                foreach (String[] s in aUsers)
                {
                    // Este loop é utilizado para saber qual cor deve ser
                    // colocada na tecla
                    if (user.Equals(s[0]))
                    {
                        Color c = Color.FromName(s[1]);
                        this.MudaCorTecla(file, c, user);
                        break;
                    }
                }

                // Tocando o som
                string fname = "\\MP3\\" + i + "\\" + file;
                fname = Application.StartupPath + fname;

                engine.SoundVolume = 0.5f;
                // engine.Play2D(fname);

                engine.SoundVolume = 0.4f;
                // engine.

                engine.Play2D(i.ToString() + "_" + file.ToUpper());

                Thread.Sleep(300);

                // Voltando a cor original da Tecla
                if (file.IndexOf("S") > 0)
                    this.MudaCorTecla(file, Color.FromArgb(30, System.Drawing.Color.Black), user);
                else
                    this.MudaCorTecla(file, Color.FromArgb(5, Color.White), user);
            }

        }

        #endregion

        // É chamado apenas pela PlayImpl
        #region MudaCorTela
        private void MudaCorTecla(string f, Color c, string user)
        {

            // Encontrando o teclado certo para mudar a sua posição
            foreach (GroupBox gp in aTeclados)
            {
                // Encontrando o teclado
                if (gp.Tag.Equals(user))
                {
                    gp.Enabled = true;

                    // Encontrando a tecla
                    foreach (Button b in gp.Controls)
                    {
                            b.Enabled = true;

                            if (b.Tag.Equals(f))
                            {
                                b.BackColor = c;

                                b.Enabled = false;
                                gp.Enabled = false;
                                return;
                            }

                            b.Enabled = false;
                    }
                    
                }
            }
        }
        #endregion

        #region OptionButtons

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.Instrument = "PIANO";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.Instrument = "GUITAR";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.Instrument = "PERCUSSION";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.Instrument = "VIOLIN";
        }

        #endregion

        #region bntCollab_Click
        private void bntCollab_Click(object sender, EventArgs e)
        {
            // Chama a janela de colaboração
             
            CollabDialog dlg = new CollabDialog();
            dlg.ShowDialog(this);
        }
        #endregion
      
        #region btnExit_Click
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                if (tClienteEnvia != null)
                {
                    if (tClienteEnvia.IsAlive)
                        tClienteEnvia.Abort();
                }

            }
            catch { }

            Application.Exit();
            Application.ExitThread();

        }
        #endregion

        #region groupTeclado_Paint
        private void groupTeclado_Paint(object sender, PaintEventArgs e)
        {

            if (modoMove)
            {
                return;
            }


            Graphics gfx = e.Graphics;

            Pen p;
            if (modoResise)
                p = new Pen(Color.Orange, 3);
            else
                p = new Pen(System.Drawing.SystemColors.Control, 3);


            GroupBox gb = (GroupBox)sender;
            Rectangle r = new Rectangle(0, 0, gb.Width, gb.Height);

            gfx.DrawLine(p, 0, 5, 0, r.Height - 2);
            gfx.DrawLine(p, 0, 5, 10, 5);
            gfx.DrawLine(p, 0, 5, r.Width - 2, 5);
            // gfx.DrawLine(p, 62, 5, r.Width - 2, 5);  // A linha acima controla a cor da barra superior do GroupBox

            gfx.DrawLine(p, r.Width - 2, 5, r.Width - 2, r.Height - 2);
            gfx.DrawLine(p, r.Width - 2, r.Height - 2, 0, r.Height - 2);
             
            
        }
        #endregion

        #region groupTecladoRemoto_Paint
        private void groupTecladoRemoto_Paint(object sender, PaintEventArgs e)
        {

            Graphics gfx = e.Graphics;

            Pen p  = new Pen(System.Drawing.SystemColors.Control, 3);

            GroupBox gb = (GroupBox)sender;
            Rectangle r = new Rectangle(0, 0, gb.Width, gb.Height);

            gfx.DrawLine(p, 0, 5, 0, r.Height - 2);
            gfx.DrawLine(p, 0, 5, 10, 5);
            gfx.DrawLine(p, 90, 5, r.Width - 2, 5);
            gfx.DrawLine(p, 90, 5, r.Width - 2, 5);  // A linha acima controla a cor da barra superior do GroupBox

            gfx.DrawLine(p, r.Width - 2, 5, r.Width - 2, r.Height - 2);
            gfx.DrawLine(p, r.Width - 2, r.Height - 2, 0, r.Height - 2);


        }
        #endregion

        #region groupTeclado_MouseMove
        private void groupTeclado_MouseMove(object sender, MouseEventArgs e)
        {

            int x = e.X + this.groupTeclado.Left;
            int y = e.Y + this.groupTeclado.Top;

            if (modoResise)
            {
                if (((x - this.groupTeclado.Left) <= 10) || ((y - this.groupTeclado.Top) <= 10))
                    return;


                this.groupTeclado.SuspendLayout();


                this.groupTeclado.Tag = this.groupTeclado.Size;
                this.groupTeclado.Size = new Size(x - this.groupTeclado.Left, y - this.groupTeclado.Top);

                this.groupTeclado.ResumeLayout();


            }

            // Se estiver no modo Move preciso colocar o quadro de acordo com 
            // a posição do ponteiro do mouse
            if (modoMove)
            {
                // this.groupTeclado.Left = x - (this.groupTeclado.Left / 2);
                // this.groupTeclado.Top = y;
            }

        }

        #endregion

        #region groupTeclado_MouseClick
        private void groupTeclado_MouseClick(object sender, MouseEventArgs e)
        {
            if (modoMove)
            {
                // this.BackColor = System.Drawing.SystemColors.Control;
                modoMove = false;
                this.groupTeclado.Enabled = true;
                this.bntMove.Visible = true;
                this.groupTeclado.Refresh();
            }

        }
        #endregion

        #region groupTeclado_Resize
        private void groupTeclado_Resize(object sender, EventArgs e)
        {

            if (modoResise)
            {
                GroupBox gb = (GroupBox)sender;
                gb.SuspendLayout();

                Size original = (Size)gb.Tag;

                Double ajusteW = Convert.ToDouble(gb.Width) / Convert.ToDouble(original.Width);
                Double ajusteH = Convert.ToDouble(gb.Height) / Convert.ToDouble(original.Height);

                Control anterior = null;

                // Redimencionando tudo que estiver dentro do groupbox
                // preciso garantir que este loop vai ler os controles do mais à esquerda até o mais à direita
                // nesta ordem
                foreach (Control c in  this.groupTeclado.Controls)
                {

                    Size s = c.Size;

                    int crescimentoPixelsW = c.Width;
                    int crescimentoPixelsH = c.Height;

                    c.Width = Convert.ToInt32(Convert.ToDouble(c.Width) * ajusteW);
                    crescimentoPixelsW = c.Width - crescimentoPixelsW;

                    c.Height = Convert.ToInt32(Convert.ToDouble(c.Height) * ajusteH);
                    crescimentoPixelsH = c.Height - crescimentoPixelsH;

                    if (anterior != null)
                    {
                        c.Left = c.Left + crescimentoPixelsW;
                        // c.Top = c.Top  + crescimentoPixelsH;
                    }

                    anterior = c;
                    PontoMaior = new Point(c.Left + c.Width, c.Top + c.Height);

                    // TODO: Pensar em como fazer o ajuste do left e do top
                    // Solução: pegar o quanto o anterior cresceu e somar este tamanho no atual 
                    // (apenas no X - Width) para manter a distância entre os elementos
                    // vai ser preciso separar as teclas pretas das brancas, pois elas tem tamanho diferentes...

                    // c.Left = Convert.ToInt32(Convert.ToDouble(c.Left) * ajusteW);
                    // c.Top  = Convert.ToInt32(Convert.ToDouble(c.Top ) * ajusteH);


                    gb.Refresh();
                }

                PontoMaior.X = PontoMaior.X + 10;
                PontoMaior.Y = PontoMaior.Y + 10;

                gb.Tag = gb.Size;
            }


        }

        #endregion

        #region groupTecladoRemoto_Resize
        private void groupTecladoRemoto_Resize(object sender, EventArgs e)
        {

            GroupBox gb = (GroupBox)sender;
            gb.SuspendLayout();

            // gb.AccessibleDefaultActionDescription // Width
            // gb.AccessibleDescription // Heigh

            Size original = new Size(int.Parse(gb.AccessibleDefaultActionDescription),int.Parse(gb.AccessibleDescription));

            Double ajusteW = Convert.ToDouble(gb.Width) / Convert.ToDouble(original.Width);
            Double ajusteH = Convert.ToDouble(gb.Height) / Convert.ToDouble(original.Height);

            Control anterior = null;

            // Redimencionando tudo que estiver dentro do groupbox
            // preciso garantir que este loop vai ler os controles do mais à esquerda até o mais à direita
            // nesta ordem
            foreach (Control c in gb.Controls)
            {

                Size s = c.Size;

                int crescimentoPixelsW = c.Width;
                int crescimentoPixelsH = c.Height;

                c.Width = Convert.ToInt32(Convert.ToDouble(c.Width) * ajusteW);
                crescimentoPixelsW = c.Width - crescimentoPixelsW;

                c.Height = Convert.ToInt32(Convert.ToDouble(c.Height) * ajusteH);
                crescimentoPixelsH = c.Height - crescimentoPixelsH;

                if (anterior != null)
                {
                    c.Left = c.Left + crescimentoPixelsW;
                }

                anterior = c;

                gb.Refresh();
            }

            gb.AccessibleDefaultActionDescription = gb.Width.ToString() ;
            gb.AccessibleDescription = gb.Height.ToString();

        }

        #endregion

        #region bntMais_Click
        private void bntMais_Click(object sender, EventArgs e)
        {

            // entra no modo de aumento do GroupBox
            this.modoResise = true;

            // this.BackColor = Color.Gray;
            this.bntMais.Visible = false;

        }

        #endregion

        #region bntMove_Click
        private void bntMove_Click(object sender, EventArgs e)
        {
            // entra no modo de aumento do GroupBox
            this.modoMove = true;
            this.groupTeclado.Enabled = false;

            // this.BackColor = Color.Gray;

            this.bntMais.Visible = false;
            this.bntMove.Visible = false;

        }
        #endregion

        // Aqui inicio o recebimento dos blobs via porta 3333 UDP
        #region StartTuio
        public void StartTuio(int port) 
        {
		
			verbose = false;
			fullscreen = false;
			width = window_width;
			height = window_height;
            
			// this.ClientSize = new System.Drawing.Size(width, height);
			// this.Name = "TuioDemo";
			// this.Text = "TuioDemo";

            this.SetStyle(  ControlStyles.AllPaintingInWmPaint |
                            ControlStyles.UserPaint |
                            ControlStyles.DoubleBuffer, true);


			objectList = new Dictionary<long,TuioDemoObject>(128);	
			cursorList = new Dictionary<long,TuioCursor>(128);	
			
			client = new TuioClient(port);
			client.addTuioListener(this);
			client.connect();
        }
        #endregion

        // No evento OnPaintBackground eu faço todo o tratamento dos blobs
        #region OnPaintBackground
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            
            // Fazer uma cópia do cursorList.Values antes de percorrê-lo!
            // Isso evita o erro "Collection was modified; enumeration operation may not execute."
            List<TuioCursor> meusBlogs = new List<TuioCursor>();
            foreach (TuioCursor tcur in cursorList.Values)
            {
                meusBlogs.Add(tcur);
            }
            

            // Variavel para guardar os elementos existentes
            ArrayList aBlobs = new ArrayList();
            
            
            // Getting the graphics object
            Graphics g = pevent.Graphics;
            g.FillRectangle(lightgrayBrush, new Rectangle(0, 0, width, height));

            // draw the cursor path (movimentar os pictureboxes)
            if (cursorList.Count > 0)
            {
                // foreach (TuioCursor tcur in cursorList.Values)
                foreach (TuioCursor tcur in meusBlogs)
                {
                    aBlobs.Add(tcur.getFingerID());
                    
                    List<TuioPoint> path = tcur.getPath();
                    TuioPoint current_point = path[0];

                    for (int i = 0; i < path.Count; i++)
                    {
                        TuioPoint next_point = path[i];
                        // g.DrawLine(fingerPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
                        current_point = next_point;
                    }
                    
                    // Aqui entra o cálculo para verificar par aonde vai mover o blob
                    // int x = (int) tcur.getScreenX(width);
                    // int y = (int)tcur.getScreenY(height);]

                    // 5.0 é o maximo
                    // Inicial: 2.0 para x e 1.5 para y

                   int x = Convert.ToInt32(tcur.getScreenX(width)* (0.5*trackBar1.Value) );
                   int y = Convert.ToInt32(tcur.getScreenY(height)*(0.5*trackBar1.Value) );

                    Point p = new Point(x,y);

                    // Posicionando o picturebox que indica qual é blob
                    aPictures[tcur.getFingerID()].Visible = true;
                    aPictures[tcur.getFingerID()].Location = p;
                    aPictures[tcur.getFingerID()].BackColor = clienteEnvia.getCorTelepointer();

                    // TODO: Arrumar posiconamento dos telefingers
                    // this.btnExit.Text = "X:" + p.X.ToString() + " Y:" + p.Y.ToString();

                    // Na função abaixo verifico se o blob está sobre alguma tecla 
                    // e toco a nota caso necessário
                    VerificaBlobTeclasSom(p, tcur.getFingerID());

                    // tcur.pctCursor.Location = new Point(x,y);

                    // g.FillEllipse(grayBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
                    // Font font = new Font("Arial", 10.0f);
                    // g.DrawString(tcur.getFingerID() + "", font, blackBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
                }
            }


            // A função abaixo colocar visible = false nos pictures boxes que saíram
            // e também remove o elementos de aTeclaBlobs, que faz a relação entre Blobs e Teclas pressionadas
            RemoveBlobs(aBlobs);


            if (modoResise)
            {
                // Posiciona os botões
                // Posicionando o botão +
                this.bntMais.Left = this.groupTeclado.Left + this.groupTeclado.Width + 1;
                this.bntMais.Top = this.groupTeclado.Top + this.groupTeclado.Height + 1;
            }

            if (modoMove)
            {
                // Posicionando o botão bntMove
                this.bntMove.Left = this.groupTeclado.Left + 15;
                this.bntMove.Top = this.groupTeclado.Top - 30;
                this.bntMove.Width = this.groupTeclado.Width - 38;

                if (this.modoMoveBlob)
                {
                    ContaTelefingerMove++;

                    if (ContaTelefingerMove > 15)
                    {
                        PictureBox p = (PictureBox)this.bntMove.Tag;

                        this.groupTeclado.Left = p.Left - (this.groupTeclado.Width / 2);
                        this.groupTeclado.Top = p.Top;

                        ContaTelefingerMove = 0;
                    }
                }

            }
            

        }
        #endregion

        // Neste método verifico o que acontece dependendo de onde o blob está
        #region VerificaTeclasSom
        private void VerificaBlobTeclasSom(Point p, int fID)
        {
            // Agora verifico os blobs áreas que saíram para tocar o KeyUP
            // Se a marca existe na lista aTeclaBlobs e não está mais sobre o retângulo da nota, faz o keydown_do botão
            // Se a marca não existe mais faz o keydown do botão -> Verificação colocada em OnPaintBackground()

            ArrayList remover = new ArrayList();

            foreach (ArrayList d in aTeclaBlobs)
            {
                int fingerID = (int) d[0];
                Button b_guardado = (Button)d[1];

                if(fingerID.Equals(fID))
                {
                    // Rectangle r_guardado = new Rectangle(b_guardado.Location.X, b_guardado.Location.Y, b_guardado.Width, b_guardado.Height);

                    Rectangle r_guardado = new Rectangle(this.groupTeclado.Left +  b_guardado.Location.X, this.groupTeclado.Top +  b_guardado.Location.Y, b_guardado.Width, b_guardado.Height);
                    
                    // Compara com o ponto atual
                    if (! (r_guardado.Contains(p)))
                    {
                        if(b_guardado.Enabled) 
                            this.TocarTeclaNota(b_guardado.AccessibleDescription, 1);

                        // Removendo o elemento do array aTeclaBlobs
                        remover.Add(d);

                    }
                }
            }

            // Removento a associação entre blob e tecla
            foreach (ArrayList r in remover)
            {
                aTeclaBlobs.Remove(r);
            }


            // Verifica se ponto passado está sobre alguma tecla Branca ou Preta
            #region VerificaBlobTecla

            // Se já tiver um blob sobre uma tecla preta não coloca o blob na tecla braca e vice versa
            // A prioridade são as teclas pretas, pois elas estão em cima das teclas brancas
            // aButtonBlack -> 10 teclas (0 a 9)
            // aButtonWhite -> 15 teclas (0 a 14)
            Button[] aTodasTeclas = new Button[25];

            aButtonBlack.CopyTo(aTodasTeclas, 0);
            aButtonWhite.CopyTo(aTodasTeclas, 10);
            
            foreach( Button b in aTodasTeclas)
            //for (int i = 0; i < 15; i++)
            {
                // Rectangle r = new Rectangle(b.Location.X,b.Location.Y,b.Width, b.Height);

                Rectangle r = new Rectangle(this.groupTeclado.Left +  b.Location.X, this.groupTeclado.Top + b.Location.Y, b.Width, b.Height);

                if (r.Contains(p))
                {
                    
                    // Armazena em uma variável global o ID do blob e a tecla que está sendo tocada! -> lista aTeclaBlobs
                    // somente armazena na lista se já não existir
                    // Armazena apenas uma tecla por vez, seja ela branca ou preta
                    ArrayList Dados = new ArrayList();
                    bool existe = false;

                    Dados.Add(fID);
                    Dados.Add(b);

                    foreach (ArrayList d in aTeclaBlobs)
                    {
                        int fingerID = (int)d[0];

                        if (fingerID.Equals(fID))
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (!existe)
                    {
                        aTeclaBlobs.Add(Dados);

                        if(b.Enabled) 
                            // Executa o evento Play_keydown (por meio de TocarTeclaNota) e depois o evento Play_keyup
                            this.TocarTeclaNota(b.AccessibleDescription, 0);
                    }
                }
            }

            #endregion

            // Aqui verifico se algum blob está ou passou sobre um radio button
            #region Verifica sobre radio button
            foreach (RadioButton ra in groupBox1.Controls)
            {
                Rectangle r = new Rectangle(groupBox1.Left+ra.Location.X, groupBox1.Top +ra.Location.Y, ra.Width, ra.Height);

                if (r.Contains(p))
                {
                    ra.Checked = true;    
                }

            }
            #endregion

            // Aqui verifico se algum blob está ou passou sobre algum botão (bntExit, bntCollab,btnRestore, btnMaximiza )
            #region Verifica sobre botão 
            // bntExit
            VerificaBlobBotao(btnExit,p,fID);
            VerificaBlobBotao(bntCollab,p,fID);
            VerificaBlobBotao(btnRestore,p,fID);
            VerificaBlobBotao(btnMaximiza,p,fID);

            VerificaBlobBotao(bntMove, p, fID);

            #endregion



            #region TODO: Verifica sobre slider (trackBar1)

            /*
            // TODO: Aqui verifico se o blob passou por cima do slider de sensibilidade
            // Somente mudo o valor do slider se o blob estiver sobre o controle do slider

            Rectangle rect = new Rectangle(trackBar1.Location.X, trackBar1.Location.Y, trackBar1.Width, trackBar1.Height);
            if (rect.Contains(p))
            {
                // Aqui verifico onde colocar o slider!
                int pos = 0;

                for(int i = 1;i<=10;i++)
                {
                    rect = new Rectangle(this.trackBar1.Left + 8 + pos, this.trackBar1.Top + 10, 11, 20);

                    if (rect.Contains(p)) 
                    {
                        this.trackBar1.Value = i;
                        break;
                    }


                    pos = (this.trackBar1.Width / 10) - i;
                }

                pos = trackBar1.Width / 10;

            } */
            #endregion

        }
        #endregion

        #region RemoveBlobs
        private void RemoveBlobs(ArrayList aBlobs)
        {

            // Aqui preciso colovar visible = false nos pictures boxes que saíram!
            // Também preciso fazer o key_up se algum dos blobs estavam em algumas das teclas

            ArrayList remover = new ArrayList();

            for (int i = 0; i < 20; i++)
            {
                // Se não está na lista de blobs visíveis...
                if (!(aBlobs.IndexOf(i) >= 0) && aPictures[i].Visible)
                {
                    // Retirando a visibilidade do blob
                    aPictures[i].Visible = false;

                    // Além de colocar o blob invisível preciso remover ele da lista de teclas
                    foreach (ArrayList d in aTeclaBlobs)
                    {
                        int fingerID = (int)d[0];
                        Button b_guardado = (Button)d[1];

                        if (fingerID.Equals(i))
                        {
                            // Rectangle r_guardado = new Rectangle(b_guardado.Location.X, b_guardado.Location.Y, b_guardado.Width, b_guardado.Height);
                            Rectangle r_guardado = new Rectangle(this.groupTeclado.Left + b_guardado.Location.X, this.groupTeclado.Top + b_guardado.Location.Y, b_guardado.Width, b_guardado.Height);

                            // Compara com o ponto que está sendo removido
                            if (r_guardado.Contains(aPictures[i].Location))
                            {
                                this.TocarTeclaNota(b_guardado.AccessibleDescription, 1);

                                // Preciso remover o elemento do array aTeclaBlobs em outro loop
                                // para evitar problemas de concorrência de acesso no arrylist
                                remover.Add(d);

                            }
                        }
                    }

                    // Aqui chamo o evento do botão, caso haja algum na propriedade tag do blob
                    #region ChecaBotão
                    if (aPictures[i].Tag != null)
                    {
                        Button b = (Button)aPictures[i].Tag;

                        // Disparar o click do botão
                        // E colocar a cor de fundo normal
                        b.BackColor = System.Drawing.SystemColors.Control;
                        b.PerformClick();

                        aPictures[i].Tag = null;

                        // Se estiver no modo de movimentação libera o teclado
                        if (  b.Equals(bntMove)  && (this.modoMoveBlob)  )
                            this.frmKeyboard_MouseClick(null, null);
                    }
                    #endregion

                }
            }

            // Removento a associação entre blob e tecla
            foreach (ArrayList r in remover)
            {
                aTeclaBlobs.Remove(r);
            }
        
        }
        #endregion

        #region VerificaBlobBotao
        public void VerificaBlobBotao(Button b, Point p, int fID)
        {

            Rectangle re = new Rectangle(b.Left, b.Top, b.Width, b.Height);

            if (re.Contains(p))
            {

                // Verificaçao especial para o botão bntMove-> Colocar no movo de movimentação do 
                //  do teclado
                if (b.Equals(bntMove))
                {
                    modoMoveBlob = true;
                    modoMove = true;
                    b.Tag = aPictures[fID];

                    aPictures[fID].Tag = b;
                    b.BackColor = Color.Gray;
                    // Desabilitando o teclado e as suas notas!
                    this.groupTeclado.Enabled = false;
                    
                    foreach (Button bot in aButtonBlack)
                        bot.Enabled = true;

                    foreach (Button bot in aButtonWhite )
                        bot.Enabled = true;


                }

                
                aPictures[fID].Tag = b;
                b.BackColor = Color.Gray;
            }
            else
            {
                if (aPictures[fID].Tag != null)
                {
                    if (aPictures[fID].Tag.Equals(b))
                    {
                        if (!b.Equals(bntMove))
                        {
                            aPictures[fID].Tag = null;
                            b.BackColor = System.Drawing.SystemColors.Control;
                        }
                    }
                }

                
            }


        }
        #endregion

        // Os eventos abaixo são para tratar o que vem do protocolo TUIO e fazem parte da interface Tuiolistener
        #region addTuioCursor
        public void addTuioCursor(TuioCursor c)
        {
            // Adicionando na lista de objetos a serem varidos no método OnPaintBackground
            cursorList.Add(c.getSessionID(), c);

            if (clienteEnvia.connected)
            {
                // TODO: Resolver problema de posicionamento do TeleFinger
                // Enviando os dados para a crição do TeleFinger
                // int x = (int) c.getScreenX(width);
                // int y = (int) c.getScreenY(height);
                int x = Convert.ToInt32(c.getScreenX(width) * (0.5 * 5));
                int y = Convert.ToInt32(c.getScreenY(height) * (0.5 * 5));

                this.EnviaMsgBlob(new Point(x, y), clienteEnvia.getCorTelepointer(), c.getFingerID(), clienteEnvia.getLogin(), "fingerCreatePointer");
            }
        }
        #endregion

        #region updateTuioCursor
        public void updateTuioCursor(TuioCursor c)
        {
            // Enviando a posição do cursor do mouse!
            // ContaTelefingerMove++;


            if (clienteEnvia.connected)
            {
                // if (ContaTelefingerMove.Equals(4))
                // {
                // TODO: Resolver problema de posicionamento do TeleFinger
                // Enviando os dados para a movimentação do TeleFinger
                // int x = (int)c.getScreenX(width);
                // int y = (int)c.getScreenY(height);
                int x = Convert.ToInt32(c.getScreenX(width) * (0.5 * 5));
                int y = Convert.ToInt32(c.getScreenY(height) * (0.5 * 5));


                this.EnviaMsgBlob(new Point(x, y), clienteEnvia.getCorTelepointer(), c.getFingerID(), clienteEnvia.getLogin(), "fingerMovePointer");
                // ContaTelefingerMove = 0;
                //}
            }

        }
        #endregion

        #region removeTuioCursor
        public void removeTuioCursor(TuioCursor c)
        {
  
            cursorList.Remove(c.getSessionID());

            // this.label1.Text = "Eemoções enviadas:" + ContaTelefingerMove.ToString();
            // ContaTelefingerMove++;

            // this.label1.Text = "Enviado:" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond.ToString() ;

            if (clienteEnvia.connected)
            {

                int x = Convert.ToInt32(c.getScreenX(width) * (0.5 * 5));
                int y = Convert.ToInt32(c.getScreenY(height) * (0.5 * 5));

                this.EnviaMsgBlob(new Point(x, y), clienteEnvia.getCorTelepointer(), c.getFingerID(), clienteEnvia.getLogin(), "fingerRemovePointer");
            }

        }
        #endregion

        // Os objetos não são utilizados neste protótipo
        #region addTuioObject
        public void addTuioObject(TuioObject o)
        {
        }
        #endregion

        #region updateTuioObject
        public void updateTuioObject(TuioObject o)
        {
        }
        #endregion

        #region removeTuioObject
        public void removeTuioObject(TuioObject o)
        {
        }
        #endregion

        #region refresh
        public void refresh(long timestamp)
        {
            Invalidate();
        }
        #endregion

        #region EnviaMsgBlob
        private void EnviaMsgBlob(Point p, Color c, int id, string login, string msg)
        {
            ArrayList list = new ArrayList();

            list.Add(p); // objeto 'genérico' (pode ser um mouse event ou outro)

            // Mandando a cor atual do telefinger
            list.Add(c);

            // 	Mandando id do telefinger
            list.Add(id);

            // Mandando o nome do proprietario do telepointer
            list.Add(login);

            // Este evento eh para reproduzir o telepointer
            // clienteEnvia.EnviaEvento((Object)list, msg);
            clienteEnvia.EnviaEventoGT((Object)list, msg);

        }
        #endregion 

        #region listView1_ItemCheck
        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string user = (string) listView1.Items[e.Index].Tag;
            bool visivel = true;
            
            if(e.NewValue.Equals(CheckState.Checked))
                visivel = true;
            else
                visivel = false;

            // Visualizando ou não o teclado remoto!
            foreach (GroupBox gp in aTeclados)
            {
                if (gp.Tag.Equals(user))
                {
                    gp.Enabled = true;
                    gp.Visible = visivel;
                    gp.Enabled = false;
                }
            }
            // Agora preciso habilitar/desabilitar os telepointers do cara e os seus telefingers!
            foreach (PictureBox p in aTelepointers)
            {
                if (p.Name.Equals(user))
                    p.Visible = visivel;
            }
            
            // Agora preciso habilitar/desabilitar os telefingers do cara e os seus telefingers!
            foreach (PictureBox p in aTelefingers)
            {
                if (p.Name.Equals(user))
                    p.Visible = visivel;
            }


        }
        #endregion 

        #region OnTelefingerImpl()
        public void OnTelefingerImpl(ArrayList l)
        {

                // Recebendo os valores
                Point p = (Point)l[0];
                Color c = (Color)l[1];
                int id = (int)l[2];
                String login = (String)l[3];
                String msg = (String)l[4];

                if(ChecaAwarenessRemoto(login))
                {

                    // TODO: Arrumar posiconamento dos telefingers
                    // this.btnExit.Text = "X:" + p.X.ToString() + " Y:" + p.Y.ToString();

                    switch (msg)
                    {
                        case "fingerCreatePointer":

                            if (this.aTelefingers[id].Visible)
                                this.aTelefingers[id].Visible = false;

                            this.aTelefingers[id].Visible = true;
                            this.aTelefingers[id].Location = p;
                            this.aTelefingers[id].BackColor = c;
                            this.aTelefingers[id].Name = login;

                            break;
                        case "fingerMovePointer":
                            this.aTelefingers[id].Location = p;
                            this.aTelefingers[id].Visible = true;
                            this.aTelefingers[id].BackColor = c;
                            this.aTelefingers[id].Name = login;
                            break;
                        case "fingerRemovePointer":
                            this.aTelefingers[id].Visible = false;
                            // ContaTelefingerRemovido++;
                            // this.bntCollab.Text = "Recebidas:" + ContaTelefingerRemovido.ToString();
                            break;
                    }
             }
        }
        #endregion 

        // O evento Paint de Picture_box monta a representação gráfica do Blobs
        #region pictureBox_Paint
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {

            Font font = new Font("Arial", 9.0f);

            PictureBox p = (PictureBox)sender;

            Rectangle r = new Rectangle(0, 0, p.Width, p.Height);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(r.X, r.Y, 15, 15);

            p.Region = new Region(gp);
            p.BackColor = clienteEnvia.getCorTelepointer(); 
            // pictureBox1.BackColor = Color.Blue;

            e.Graphics.DrawString(p.Name, font, new SolidBrush(Color.Black), new PointF(2, 0));

        }
        #endregion

        // O evento Paint de Picture_box monta a representação gráfica dos telepointers
        #region telepointer_Paint
        private void telepointer_Paint(object sender, PaintEventArgs e)
        {
            // TODO: Pensar em colocar Panels modificados no lugar de PictureBoxes
            // para poder utilizar a transparência
            PictureBox p = (PictureBox)sender;

            // CoKeyboard.Collab.TransPanel p = (CoKeyboard.Collab.TransPanel)sender;
            Color c;

            c = p.BackColor;

            // p.BackColor = System.Drawing.Color.Transparent;

            Point pp = new Point(p.Width / 2, p.Height / 2);

           // e.Graphics.DrawRectangle(new Pen(new SolidBrush(c)), pp.X - 10, pp.Y - 2, 20, 4);
           // e.Graphics.DrawRectangle(new Pen(new SolidBrush(c)), pp.X - 2, pp.Y - 10, 4, 20);

            // e.Graphics.DrawString(p.Name, new Font("Arial", 10), new SolidBrush(c), pp.X + 4, pp.Y + 4);

            e.Graphics.DrawString(p.Name, new Font("Arial", 9), new SolidBrush(Color.Black), 0, 0);
            
            /* 
            Font font = new Font("Arial", 10.0f);
            
            e.Graphics.DrawString(p.Name, font, new SolidBrush(Color.Black), new PointF(10, 10)); */

        }
        #endregion

        // O evento Paint de Picture_box monta a representação gráfica dos telefingers
        #region telefinger_Paint
        private void telefinger_Paint(object sender, PaintEventArgs e)
        {
            
            // TODO: Pensar em colocar Panels modificados no lugar de PictureBoxes
            // para poder utilizar a transparência
            PictureBox p = (PictureBox)sender;
            Color c;

            c = p.BackColor;

            // p.BackColor = System.Drawing.Color.Transparent;

            Point pp = new Point(p.Width / 2, p.Height / 2);
            Font font = new Font("Arial", 9.0f);

            Rectangle r = new Rectangle(0, 0, p.Width, p.Height);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(r.X, r.Y, 15, 15);

            p.Region = new Region(gp);
            // pictureBox1.BackColor = Color.Blue;

            e.Graphics.DrawString(p.Name, font, new SolidBrush(Color.Black), new PointF(2, 0));


        }
        #endregion

      
    }

}
