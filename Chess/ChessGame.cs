﻿using System;
using System.Windows.Forms;
using Cselian.Chess.Game;

namespace Cselian.Chess
{
	public partial class ChessGame : Form
	{
		private UIScreen Screen;
		private UIScreen OtherScreen;

		public ChessGame(UIScreen mode = null)
		{
			InitializeComponent();

			AboutMnu.Click += AboutMnu_Click;

			All.SplitterDistance = Square.Size + 16;
			Boards.Panel1.AutoScroll = true;
			Boards.Panel1.AutoScrollMinSize = Board.Size;
			Boards.Panel2.AutoScroll = true;
			Boards.Panel2.AutoScrollMinSize = OppBoard.Size;
			All.FixedPanel = FixedPanel.Panel1;

			Screen = mode ?? new UIScreen(this, UIScreen.UIState.Dual_Screens, true);
		}

		public void SetOtherVisible(bool visible)
		{
			Boards.Panel2Collapsed = !visible;
		}

		public string GetOtherIP()
		{
			return ModeOtherIP.Text.Length > 0 ? ModeOtherIP.Text : "not set";
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (Screen.State == UIScreen.UIState.Dual_Screens && !Screen.IsHost)
			{
				UIModeMnu.Visible = false;
			}
			else
			{
				WinHelper.CycleStates<UIScreen.UIState>(UIModeMnu, UIModeMnu_StateChanged);
				var ix = WinHelper.GetState<int>(UIModeMnu);
				(UIModeMnu.DropDownItems[ix] as ToolStripMenuItem).Checked = true;
			}

			SetGameState();
		}

		private void SetGameState()
		{
			var txt = Screen.State.ToString().Replace("_", " ");
			UIModeMnu.Text = "UI: " + txt;

			if (Screen.Clear)
			{
				Screen.ClearBoards(Board, OppBoard, MyKilled, OppKilled);
				if (OtherScreen != null)
				{
					OtherScreen.Game.Hide();
					OtherScreen = null;
				}

				Screen.Clear = false;
				return;
			}

			if (Screen.Board == null)
			{
				Screen.CreateBoards(Board, OppBoard, MyKilled, OppKilled);
			}

			Screen.SetState();
			if (Screen.State == UIScreen.UIState.Dual_Screens && Screen.IsHost)
			{
				if (OtherScreen == null)
				{
					OtherScreen = new UIScreen(Screen.State, false);
					OtherScreen.CreateGame();
				}

				OtherScreen.SetState();
				OtherScreen.Game.Show();
			}
			else if (OtherScreen != null)
			{
				OtherScreen.Game.Hide();
			}
		}

		private void UIModeMnu_StateChanged(object sender, EventArgs e)
		{
			Screen.State = WinHelper.GetState<UIScreen.UIState>(UIModeMnu);
			SetGameState();
			ModeMyIP.Visible = ModeOtherIP.Visible = Screen.State == UIScreen.UIState.Remote;
		}

		private void AboutMnu_Click(object sender, EventArgs e)
		{
			new About().ShowDialog();
		}
	}
}
