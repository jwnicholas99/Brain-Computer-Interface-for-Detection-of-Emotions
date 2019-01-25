using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.IO;
using BCILib.Util;

namespace BCILib.App
{
    public partial class ExternalBCIApp : UserControl
    {
        public ExternalBCIApp()
        {
            InitializeComponent();
        }

        public void LoadGameList()
        {
            comboGameList.Items.Clear();
            comboGameList.Items.Add("New Game ...");
            comboGameList.SelectedIndex = 0;
            string[] glist = BCIApplication.GetAppGames();
            if (glist != null) {
                foreach (string gname in glist) {
                    comboGameList.Items.Add(gname);
                }

                if (comboGameList.Items.Count > 1) {
                    comboGameList.SelectedIndex = 1;
                }
            }
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (comboGameList.SelectedIndex == 0) {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.RestoreDirectory = true;
                fdlg.InitialDirectory = Directory.GetCurrentDirectory();
                fdlg.Filter = "Exe files|*.exe|All Files|*.*";
                if (fdlg.ShowDialog() != DialogResult.OK) {
                    return;
                }

                string game_path = fdlg.FileName;
                string game_name = Path.GetFileNameWithoutExtension(game_path);
                game_name = ConfirmNewGame.ShowDialog(game_name);
                if (game_name == null) return;

                if (comboGameList.Items.IndexOf(game_name) >= 0) {
                    MessageBox.Show("Game name already exists!");
                    return;
                }

                ResManager rm = BCIApplication.AppResource;
                string line = rm.GetConfigValue(BCIApplication.AppName, "AppGames");
                if (string.IsNullOrEmpty(line)) line = game_name;
                else if (comboGameList.Items.IndexOf(game_name) >= 0) {
                    MessageBox.Show("Game ID already exists!");
                        return;
                }
                else line = line + "," + game_name;
                rm.SetConfigValue(BCIApplication.AppName, "AppGames", line);

                rm.SetConfigValue(game_name, "Game_Path", game_path);

                int gn = comboGameList.Items.Add(game_name);
                comboGameList.SelectedIndex = gn;
                rm.SaveFile();
            }
            else if (comboGameList.SelectedIndex > 0) {
                //string game_path = 
                BCIApplication.GetGamePath(comboGameList.Text, true);
            }
        }

        WMCopyData _wmclient = null;

        public WMCopyData LaunchGame()
        {
            if (comboGameList.SelectedIndex <= 0) {
                return null;
            }

			// start game specified in the list
			string gname = comboGameList.Text;
			if (_wmclient != null) {
				if (_wmclient.Property != gname) {
					if (_wmclient.GetAllGUIWindows() > 0) {
						_wmclient.SendClient(GameCommand.CloseGame);
					}
					_wmclient = null;
				}
			}

			if (_wmclient == null) _wmclient = new WMCopyData(gname, this.Handle);

			//ResManager rm = new ResManager(NIApplication.MAIN_CFG);
			if (_wmclient.GetAllGUIWindows() == 0) {
				string game_path = BCIApplication.GetGamePath(gname);
				if (gname == null || !File.Exists(game_path)) return null;

				// start program
				System.Diagnostics.ProcessStartInfo pinf = new System.Diagnostics.ProcessStartInfo();
				pinf.FileName = game_path;
				pinf.WorkingDirectory = Path.GetDirectoryName(game_path);
				Console.WriteLine("Start {0} in {1}", pinf.FileName, pinf.WorkingDirectory);
				System.Diagnostics.Process proc = System.Diagnostics.Process.Start(pinf);
				proc.Close();

				System.Threading.Thread.Sleep(1000);

			}

			_wmclient.GetAllGUIWindows();
            return _wmclient;
        }

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            if (comboGameList.SelectedIndex <= 0) {
                MessageBox.Show("Please select a valid game to start.");
            }
            else {
                LaunchGame();
            }
        }
    }
}
