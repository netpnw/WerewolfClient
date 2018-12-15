using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventEnum = WerewolfClient.WerewolfModel.EventEnum;
using CommandEnum = WerewolfClient.WerewolfCommand.CommandEnum;
using WerewolfAPI.Model;
using Role = WerewolfAPI.Model.Role;

namespace WerewolfClient
{
    public partial class MainForm : Form, View
    {
		System.Media.SoundPlayer soundplay = new System.Media.SoundPlayer();
		bool drag = false;
		Point start_point = new Point(0, 0);
		private Timer _updateTimer;
        private WerewolfController controller;
        private Game.PeriodEnum _currentPeriod;
        private int _currentDay;
        private int _currentTime;
        private bool _voteActivated;
        private bool _actionActivated;
        private string _myRole;
        private bool _isDead;
        private List<Player> players = null;
		private Form constuctlogin;
        public MainForm()
        {
            InitializeComponent();
			soundplay.SoundLocation = "Halloween-Music-Night-of-the-Werewolf-192-kbps (online-audio-converter.com).wav";
			soundplay.PlayLooping();

            foreach (int i in Enumerable.Range(0, 16))
            {

                this.Controls["GBPlayers"].Controls["BtnPlayer" + i].Click += new System.EventHandler(this.BtnPlayerX_Click);
                
                this.Controls["GBPlayers"].Controls["BtnPlayer" + i].Tag = i;

				
				//}else if(i >= 15)
				//{
				//	BtnPlayer0.Visible = true;
				//	BtnPlayer1.Visible = true;
				//	BtnPlayer2.Visible = true;
				//	BtnPlayer3.Visible = true;
				//	BtnPlayer4.Visible = true;
				//	BtnPlayer5.Visible = true;
				//	BtnPlayer6.Visible = true;
				//	BtnPlayer7.Visible = true;
				//	BtnPlayer8.Visible = true;
				//	BtnPlayer9.Visible = true;
				//	BtnPlayer10.Visible = true;
				//	BtnPlayer11.Visible = true;
				//	BtnPlayer12.Visible = true;
				//	BtnPlayer13.Visible = true;
				//	BtnPlayer14.Visible = true;
				//	BtnPlayer15.Visible = true;
				//}
            }

            _updateTimer = new Timer();
            _voteActivated = false;
            _actionActivated = false;
            EnableButton(BtnJoin, true);
				
            EnableButton(BtnAction, false);
            EnableButton(BtnVote, false);
            _myRole = null;
            _isDead = false;
			LogoutPanel.Visible = false;
			backtomain.Visible = false;
			GameFinish.Visible = false;
			
			soundstop.Visible = true;
			soundstop2.Visible = false;
			soundplayloop2.Visible = false;

		}

		public void addSignIn(Form login)
        {
            constuctlogin = login;
        }

        private void OnTimerEvent(object sender, EventArgs e)
        {
            WerewolfCommand wcmd = new WerewolfCommand();
            wcmd.Action = CommandEnum.RequestUpdate;
            controller.ActionPerformed(wcmd);
        }

        public void AddChatMessage(string str)
        {
            TbChatBox.AppendText(str + Environment.NewLine);
        }

        public void EnableButton(Button btn, bool state)
        {
            btn.Visible = btn.Enabled = state;
        }

        private void UpdateAvatar(WerewolfModel wm)
        {
            int i = 0;
            foreach (Player player in wm.Players)
            {
                Controls["GBPlayers"].Controls["BtnPlayer" + i].Text = player.Name;
                if (player.Name == wm.Player.Name || player.Status != Player.StatusEnum.Alive)
                {
                    // FIXME, need to optimize this
                    
                    Image img = Properties.Resources.Icon_villager;
                    string role;
                    if (player.Name == wm.Player.Name)
                    {
                        role = _myRole;
                    }
                    else if (player.Role != null)
                    {
                        role = player.Role.Name;
                    }
                    else
                    {
                        continue;
                    }
                    switch (role)
                    {
                        case WerewolfModel.ROLE_SEER:
                            img = Properties.Resources.Icon_seer;
                            break;
                        case WerewolfModel.ROLE_AURA_SEER:
                            img = Properties.Resources.Icon_aura_seer;
                            break;
                        case WerewolfModel.ROLE_PRIEST:
                            img = Properties.Resources.Icon_priest;
                            break;
                        case WerewolfModel.ROLE_DOCTOR:
                            img = Properties.Resources.Icon_doctor;
                            break;
                        case WerewolfModel.ROLE_WEREWOLF:
                            img = Properties.Resources.Icon_werewolf;
                            break;
                        case WerewolfModel.ROLE_WEREWOLF_SEER:
                            img = Properties.Resources.Icon_wolf_seer;
                            break;
                        case WerewolfModel.ROLE_ALPHA_WEREWOLF:
                            img = Properties.Resources.Icon_alpha_werewolf;
                            break;
                        case WerewolfModel.ROLE_WEREWOLF_SHAMAN:
                            img = Properties.Resources.Icon_wolf_shaman;
                            break;
                        case WerewolfModel.ROLE_MEDIUM:
                            img = Properties.Resources.Icon_medium;
                            break;
                        case WerewolfModel.ROLE_BODYGUARD:
                            img = Properties.Resources.Icon_bodyguard;
                            break;
                        case WerewolfModel.ROLE_JAILER:
                            img = Properties.Resources.Icon_jailer;
                            break;
                        case WerewolfModel.ROLE_FOOL:
                            img = Properties.Resources.Icon_fool;
                            break;
                        case WerewolfModel.ROLE_HEAD_HUNTER:
                            img = Properties.Resources.Icon_head_hunter;
                            break;
                        case WerewolfModel.ROLE_SERIAL_KILLER:
                            img = Properties.Resources.Icon_serial_killer;
                            break;
                        case WerewolfModel.ROLE_GUNNER:
                            img = Properties.Resources.Icon_gunner;
                            break;
                    }
                    ((Button)Controls["GBPlayers"].Controls["BtnPlayer" + i]).Image = img;
                }
                i++;    
            }
        }
        public void Notify(Model m)
        {
            if (m is WerewolfModel)
            {
                WerewolfModel wm = (WerewolfModel)m;
                switch (wm.Event)
                {
                    case EventEnum.JoinGame:
                        if (wm.EventPayloads["Success"] == WerewolfModel.TRUE)
                        {
							afterjoin.Visible = false;
                            BtnJoin.Visible = false;
                            AddChatMessage("You're joing the game #" + wm.EventPayloads["Game.Id"] + ", please wait for game start.");
                            _updateTimer.Interval = 1000;
                            _updateTimer.Tick += new EventHandler(OnTimerEvent);
                            _updateTimer.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show("You can't join the game, please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case EventEnum.GameStopped:
                        AddChatMessage("Game is finished, outcome is " + wm.EventPayloads["Game.Outcome"]);
                        _updateTimer.Enabled = false;
                        LablePicWereWolf.Visible = true;
						GameFinish.Visible = true;
						this.BackColor = Color.FromArgb(34, 36, 49);
						break;
                    case EventEnum.GameStarted:
                        players = wm.Players;
                        _myRole = wm.EventPayloads["Player.Role.Name"];
                        AddChatMessage("Your role is " + _myRole + ".");
                        _currentPeriod = Game.PeriodEnum.Night;
						this.BackColor = Color.FromArgb(34, 36, 49);
						if (soundstop.Visible == true)
						{
							soundstop2.Visible = true;
							soundplayloop2.Visible = false;
						}
						else
						{
							soundplayloop2.Visible = true;
							soundstop2.Visible = false;
						}
						soundstop.Visible = false;
						soundplayloop.Visible = false;
						GBPlayers.ForeColor = Color.White;
						GBChat.ForeColor = Color.White;
						GBAction.ForeColor = Color.White;
						GBStatus.ForeColor = Color.White;
						BtnManuLoout.ForeColor = Color.White;
						EnableButton(BtnAction, true);
                        switch (_myRole)
                        {
                            case WerewolfModel.ROLE_PRIEST:
                                BtnAction.Text = WerewolfModel.ACTION_HOLYWATER;
                                break;
                            case WerewolfModel.ROLE_GUNNER:
                                BtnAction.Text = WerewolfModel.ACTION_SHOOT;
                                break;
                            case WerewolfModel.ROLE_JAILER:
                                BtnAction.Text = WerewolfModel.ACTION_JAIL;
                                break;
                            case WerewolfModel.ROLE_WEREWOLF_SHAMAN:
                                BtnAction.Text = WerewolfModel.ACTION_ENCHANT;
                                break;
                            case WerewolfModel.ROLE_BODYGUARD:
                                BtnAction.Text = WerewolfModel.ACTION_GUARD;
                                break;
                            case WerewolfModel.ROLE_DOCTOR:
                                BtnAction.Text = WerewolfModel.ACTION_HEAL;
                                break;
                            case WerewolfModel.ROLE_SERIAL_KILLER:
                                BtnAction.Text = WerewolfModel.ACTION_KILL;
                                break;
                            case WerewolfModel.ROLE_SEER:
                            case WerewolfModel.ROLE_WEREWOLF_SEER:
                                BtnAction.Text = WerewolfModel.ACTION_REVEAL;
                                break;
                            case WerewolfModel.ROLE_AURA_SEER:
                                BtnAction.Text = WerewolfModel.ACTION_AURA;
                                break;
                            case WerewolfModel.ROLE_MEDIUM:
                                BtnAction.Text = WerewolfModel.ACTION_REVIVE;
                                break;
                            default:
                                EnableButton(BtnAction, false);
                                break;
                        }
                        EnableButton(BtnVote, true);
                        EnableButton(BtnJoin, false);
                        UpdateAvatar(wm);
                        break;
                    case EventEnum.SwitchToDayTime:
                        AddChatMessage("Switch to day time of day #" + wm.EventPayloads["Game.Current.Day"] + ".");
                        _currentPeriod = Game.PeriodEnum.Day;
                        LBPeriod.Text = "Day time of";
						this.BackColor = Color.Gainsboro;
						GBPlayers.ForeColor = Color.Black;
						GBChat.ForeColor = Color.Black;
						GBAction.ForeColor = Color.Black;
						GBStatus.ForeColor = Color.Black;
						BtnManuLoout.ForeColor = Color.Black;
						if (soundstop2.Visible == true)
						{
							soundstop.Visible = true;
							soundplayloop.Visible = false;
						}
						else
						{
							soundplayloop.Visible = true;
							soundstop.Visible = false;
						}
						soundstop2.Visible = false;
						soundplayloop2.Visible = false;
						

						break;
                    case EventEnum.SwitchToNightTime:
                        AddChatMessage("Switch to night time of day #" + wm.EventPayloads["Game.Current.Day"] + ".");
                        _currentPeriod = Game.PeriodEnum.Night;
                        LBPeriod.Text = "Night time of";
						if(soundstop.Visible == true)
						{
							soundstop2.Visible = true;
							soundplayloop2.Visible = false;
						}
						else
						{
							soundplayloop2.Visible = true;
							soundstop2.Visible = false;
						}
						soundstop.Visible = false;
						soundplayloop.Visible = false;
						this.BackColor = Color.FromArgb(34, 36, 49);
						GBPlayers.ForeColor = Color.White;
						GBChat.ForeColor = Color.White;
						GBAction.ForeColor = Color.White;
						GBStatus.ForeColor = Color.White;
						BtnManuLoout.ForeColor = Color.White;
						

						break;
                    case EventEnum.UpdateDay:
                        // TODO  catch parse exception here
                        string tempDay = wm.EventPayloads["Game.Current.Day"];
                        _currentDay = int.Parse(tempDay);
                        LBDay.Text = tempDay;
						//this.BackColor = Color.Gainsboro;
                        break;
                    case EventEnum.UpdateTime:
                        string tempTime = wm.EventPayloads["Game.Current.Time"];
                        _currentTime = int.Parse(tempTime);
                        LBTime.Text = tempTime;
                        UpdateAvatar(wm);
                        break;
                    case EventEnum.Vote:
                        if (wm.EventPayloads["Success"] == WerewolfModel.TRUE)
                        {
                            AddChatMessage("Your vote is registered.");
                        }
                        else
                        {
                            AddChatMessage("You can't vote now.");
                        }
                        break;
                    case EventEnum.Action:
                        if (wm.EventPayloads["Success"] == WerewolfModel.TRUE)
                        {
                            AddChatMessage("Your action is registered.");
                        }
                        else
                        {
                            AddChatMessage("You can't perform action now.");
                        }
                        break;
                    case EventEnum.YouShotDead:
                        AddChatMessage("You're shot dead by gunner.");
                        _isDead = true;
                        break;
                    case EventEnum.OtherShotDead:
                        AddChatMessage(wm.EventPayloads["Game.Target.Name"] + " was shot dead by gunner.");
                        break;
                    case EventEnum.Alive:
                        AddChatMessage(wm.EventPayloads["Game.Target.Name"] + " has been revived by medium.");
                        if (wm.EventPayloads["Game.Target.Id"] == null)
                        {
                            _isDead = false;
                        }
                        break;
                    case EventEnum.ChatMessage:
                        if (wm.EventPayloads["Success"] == WerewolfModel.TRUE)
                        {
                            AddChatMessage(wm.EventPayloads["Game.Chatter"] + ":" + wm.EventPayloads["Game.ChatMessage"]);
                        }
                        break;
                    case EventEnum.Chat:
                        if (wm.EventPayloads["Success"] == WerewolfModel.FALSE)
                        {
                            switch (wm.EventPayloads["Error"])
                            {
                                case "403":
                                    AddChatMessage("You're not alive, can't talk now.");
                                    break;
                                case "404":
                                    AddChatMessage("You're not existed, can't talk now.");
                                    break;
                                case "405":
                                    AddChatMessage("You're not in a game, can't talk now.");
                                    break;
                                case "406":
                                    AddChatMessage("You're not allow to talk now, go to sleep.");
                                    break;
                            }
                        }
                        break;
					case EventEnum.SignOut:
						if (wm.EventPayloads["Success"] == WerewolfModel.TRUE)
						{
							constuctlogin.Visible = true;
							this.Visible = false;
						}
						break;
                }
                // need to reset event
                wm.Event = EventEnum.NOP;
            }
        }

        public void setController(Controller c)
        {
            controller = (WerewolfController)c;
        }

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            WerewolfCommand wcmd = new WerewolfCommand();
            wcmd.Action = CommandEnum.JoinGame;
            controller.ActionPerformed(wcmd);
        }

        private void BtnVote_Click(object sender, EventArgs e)
        {
            if (_voteActivated)
            {
                BtnVote.BackColor = Button.DefaultBackColor;
            }
            else
            {
                BtnVote.BackColor = Color.Red;
            }
            _voteActivated = !_voteActivated;
            if (_actionActivated)
            {
                BtnAction.BackColor = Button.DefaultBackColor;
                _actionActivated = false;
            }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (_isDead)
            {
                AddChatMessage("You're dead!!");
                return;
            }
            if (_actionActivated)
            {
                BtnAction.BackColor = Button.DefaultBackColor;
            }
            else
            {
                BtnAction.BackColor = Color.Red;
            }
            _actionActivated = !_actionActivated;
            if (_voteActivated)
            {
                BtnVote.BackColor = Button.DefaultBackColor;
                _voteActivated = false;
            }
        }

        private void BtnPlayerX_Click(object sender, EventArgs e)
        {
            Button btnPlayer = (Button)sender;
            int index = (int) btnPlayer.Tag;

            if (players == null)
            {
                // Nothing to do here;
                return;
            }
            if (_actionActivated)
            {
                _actionActivated = false;
                BtnAction.BackColor = Button.DefaultBackColor;
                AddChatMessage("You perform [" + BtnAction.Text + "] on " + players[index].Name);
                WerewolfCommand wcmd = new WerewolfCommand();
                wcmd.Action = CommandEnum.Action;
                wcmd.Payloads = new Dictionary<string, string>() { { "Target", players[index].Id.ToString() } };
                controller.ActionPerformed(wcmd);
            }
            if (_voteActivated)
            {
                _voteActivated = false;
                BtnVote.BackColor = Button.DefaultBackColor;
                AddChatMessage("You vote on " + players[index].Name);
               
                WerewolfCommand wcmd = new WerewolfCommand();
                wcmd.Action = CommandEnum.Vote;
                wcmd.Payloads = new Dictionary<string, string>() { { "Target", players[index].Id.ToString() } };
                controller.ActionPerformed(wcmd);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void TbChatInput_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && TbChatInput.Text != "")
            {
                WerewolfCommand wcmd = new WerewolfCommand();
                wcmd.Action = CommandEnum.Chat;
                wcmd.Payloads = new Dictionary<string, string>() { { "Message", TbChatInput.Text } };
                TbChatInput.Text = "";
                controller.ActionPerformed(wcmd);
            }
        }

		

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{
			this.BackColor = Color.FromArgb(34, 36, 49);
			BtnManuLoout.ForeColor = Color.White;
			LogoutPanel.Visible = true;
			BtnManuLoout.Visible = false;
			backtomain.Visible = true;
			if (soundstop.Visible == true)
			{
				soundstop2.Visible = true;
			}
			else
			{
				soundplayloop2.Visible = true;
				soundstop2.Visible = false;
			}
			soundstop.Visible = false;
			soundplayloop.Visible = false;
		}

		private void button1_Click(object sender, EventArgs e)
		{

			Login login = (Login)constuctlogin;
			WerewolfCommand wcmd = new WerewolfCommand();
			wcmd.Action = CommandEnum.SignOut;
			wcmd.Payloads = new Dictionary<string, string>() { { "Server", login.GetServer() } };
			controller.ActionPerformed(wcmd);
			
			login.Show();
			this.Hide();
			BtnJoin.Show();
			LogoutPanel.Visible = false;
			this.BackColor = Color.Gainsboro;
			GBPlayers.ForeColor = Color.Black;
			GBChat.ForeColor = Color.Black;
			GBAction.ForeColor = Color.Black;
			GBStatus.ForeColor = Color.Black;
			BtnManuLoout.ForeColor = Color.Black;
			if (soundstop2.Visible == true)
			{
				soundstop.Visible = true;
			}
			else
			{
				soundplayloop.Visible = true;
				soundstop.Visible = false;
			}
			soundstop2.Visible = false;
			soundplayloop2.Visible = false;
		}

	

		private void label4_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Close();

        }

        private void TbChatBox_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void TbChatBox_TextChanged_1(object sender, EventArgs e)
        {
            

        }


		private void MainForm_MouseDown(object sender, MouseEventArgs e)
		{
			drag = true;
			start_point = new Point(e.X, e.Y);
		}

		private void MainForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (drag)
			{
				Point p = PointToScreen(e.Location);
				this.Location = new Point(p.X - start_point.X, p.Y - start_point.Y);
			}
		}

		private void MainForm_MouseUp(object sender, MouseEventArgs e)
		{
			drag = false;
		}

		private void backtomain_Click(object sender, EventArgs e)
		{
			LogoutPanel.Visible = false;
			BtnManuLoout.Visible = true;
			backtomain.Visible = false;
			//if (_currentPeriod == Game.PeriodEnum.Day || )
			//{
			this.BackColor = Color.Gainsboro;
			GBPlayers.ForeColor = Color.Black;
			GBChat.ForeColor = Color.Black;
			GBAction.ForeColor = Color.Black;
			GBStatus.ForeColor = Color.Black;
			BtnManuLoout.ForeColor = Color.Black;
			//}
			//else
			//{
			//	this.BackColor = Color.FromArgb(34, 36, 49);
			//	GBPlayers.ForeColor = Color.White;
			//	GBChat.ForeColor = Color.White;
			//	GBAction.ForeColor = Color.White;
			//	GBStatus.ForeColor = Color.White;
			//	BtnManuLoout.ForeColor = Color.White;
			//}
			
			if (soundstop2.Visible == true)
			{
				soundstop.Visible = true;
			}
			else
			{
				soundplayloop.Visible = true;
				soundstop.Visible = false;
			}
			soundstop2.Visible = false;
			soundplayloop2.Visible = false;
		}
		int chatsetup = 1;
		private void TbChatBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (chatsetup == 1)
			{
				chatsetup = 2;
			}
			else
			{
				chatsetup = 1;
			}
		}

		private void TbChatBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (chatsetup == 2)
			{
				GBChat.Size = new Size(325, 370);
				GBChat.Location = new Point(20, 30);
				TbChatBox.Location = new Point(19, 23);
				TbChatBox.Size = new Size(284, 299);
				TbChatInput.Location = new Point(19, 325);
			}
			if (chatsetup == 1)
			{
				GBChat.Size = new Size(325, 113);
				GBChat.Location = new Point(20, 287);
				TbChatBox.Location = new Point(19, 23);
				TbChatBox.Size = new Size(284, 41);
				TbChatInput.Location = new Point(19, 70);
			}
		}

		private void BtnExitinGamefinish_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void soundstop_Click(object sender, EventArgs e)
		{
			
			soundplay.Stop();
			soundplayloop.Visible = true;
			soundstop.Visible = false;
			
		}

		private void soundplayloop_Click(object sender, EventArgs e)
		{
			soundplay.PlayLooping();
			soundplayloop.Visible = false;
			soundstop.Visible = true;
			
			
			
		}

		private void soundstop2_Click(object sender, EventArgs e)
		{
			
				soundplay.Stop();
				soundplayloop2.Visible = true;
				soundstop2.Visible = false;
			
			
			

		}

		private void soundplayloop2_Click(object sender, EventArgs e)
		{
				soundplay.PlayLooping();
				soundplayloop2.Visible = false;
				soundstop2.Visible = true;
			
		}
	}

}
