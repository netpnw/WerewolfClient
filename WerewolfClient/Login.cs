using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandEnum = WerewolfClient.WerewolfCommand.CommandEnum;

namespace WerewolfClient
{
    public partial class Login : Form, View
    {
		bool drag = false;
		Point start_point = new Point(0, 0);
        private WerewolfController controller;
        private Form _mainForm;
		System.Media.SoundPlayer soundplay = new System.Media.SoundPlayer();
		public Login(Form MainForm)
        {
            InitializeComponent();
            _mainForm = MainForm;
			soundplay.SoundLocation = "Halloween-Music-Night-of-the-Werewolf-192-kbps (online-audio-converter.com).wav";
			soundloginplay.Visible = false;
		}

        public void Notify(Model m)
        {
            if (m is WerewolfModel)
            {
                WerewolfModel wm = (WerewolfModel)m;
                switch (wm.Event)
                {
                    case WerewolfModel.EventEnum.SignIn:
                        if (wm.EventPayloads["Success"] == "True")
                        {
                            _mainForm.Visible = true;
                            this.Visible = false;
							
                           // WerewolfCommand wcmd = new WerewolfCommand();
                           // wcmd.Action = CommandEnum.JoinGame;
                           // controller.ActionPerformed(wcmd);
                        }
                        else
                        {
                            MessageBox.Show("Login or password incorrect, please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case WerewolfModel.EventEnum.SignUp:
                        if (wm.EventPayloads["Success"] == "True")
                        {
                            MessageBox.Show("Sign up successfuly, please login", "Success", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            MessageBox.Show("Login or password incorrect, please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                }
            }
        }

        public void setController(Controller c)
        {
            controller = (WerewolfController)c;
        }

        private void BtnSignIn_Click(object sender, EventArgs e)
        {
			if(TbLogin.Text == ""||TbPassword.Text == "")
			{
				MessageBox.Show("Login or password incorrect, please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
            WerewolfCommand wcmd = new WerewolfCommand();
            wcmd.Action = WerewolfCommand.CommandEnum.SignIn;
            wcmd.Payloads = new Dictionary<string, string>() { { "Login", TbLogin.Text }, { "Password", TbPassword.Text }, { "Server", TBServer.Text } };
            controller.ActionPerformed(wcmd);
        }

        private void BtnSignUp_Click(object sender, EventArgs e)
        {
			if (TbLogin.Text == "" || TbPassword.Text == "")
			{
				MessageBox.Show("Login or password incorrect, please try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			WerewolfCommand wcmd = new WerewolfCommand();
            wcmd.Action = WerewolfCommand.CommandEnum.SignUp;
            wcmd.Payloads = new Dictionary<string, string>() { { "Login", TbLogin.Text}, { "Password",TbPassword.Text}, { "Server", TBServer.Text } };
            controller.ActionPerformed(wcmd);
			MessageBox.Show("Sign up success,please logout to enter the game.");
			
        }

		public string GetServer()
        {
            return TBServer.Text;
        }
        
		
		private void BtnExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            
            CBServer.BackColor = Color.FromArgb(34, 36, 49);

            if (CBServer.SelectedItem.ToString() == "2 Player")
            {
				TBServer.Text = "http://project-ile.net:2342/werewolf/";
				//TBServer.Text = "http://localhost:2343/werewolf/";
				

			}

           else if (CBServer.SelectedItem.ToString() == "4 Player")
            {
                TBServer.Text = "http://project-ile.net:2344/werewolf/";
				//TBServer.Text = "http://localhost:2343/werewolf/";
			}

            else  if (CBServer.SelectedItem.ToString() == "16 Player")
            {
				 TBServer.Text = "http://project-ile.net:23416/werewolf/";
				//TBServer.Text = "http://localhost:2343/werewolf/";
			}
        }

		private void Login_Load(object sender, EventArgs e)
		{

		}
		


		private void Login_MouseDown(object sender, MouseEventArgs e)
		{
			drag = true;
			start_point = new Point(e.X, e.Y);
		}

		private void Login_MouseMove(object sender, MouseEventArgs e)
		{
			if (drag)
			{
				Point p = PointToScreen(e.Location);
				this.Location = new Point(p.X - start_point.X, p.Y - start_point.Y);
			}

		}

		private void Login_MouseUp(object sender, MouseEventArgs e)
		{
			drag = false;
		}

		private void soundloginplay_Click(object sender, EventArgs e)
		{
			soundplay.PlayLooping();
			soundloginplay.Visible = false;
			soundloginstop.Visible = true;

		}

		private void soundloginstop_Click(object sender, EventArgs e)
		{
			soundplay.Stop();
			soundloginplay.Visible = true;
			soundloginstop.Visible = false;
		}
	}
}
