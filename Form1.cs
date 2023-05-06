/*
 * 製作時遇到無法突破的困難
 * 1.若有元件與picture相疊，則會出現參數無效的問題。見1/2的範例檔案 => 目前作法只顯示場景或人物
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PET_II
{

    public partial class petScreen : Form
    {
        /*Common variables*/
        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();//Music variable
        SqlConnection cn;
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;" + @"AttachDbFilename=|DataDirectory|\databases\PET.mdf;" + "Integrated Security=True;";
        DataSet ds = new DataSet();
        int saveId;
        string petName = "";
        int currentStoryProcess = -1;
        int positiveValue = 0;
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string filename);

        public petScreen()
        {
            InitializeComponent();
        }

        /*Element control functions*/
        private void petScreen_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void petScreen_Load(object sender, EventArgs e)
        {
            Title_Panel.BringToFront();
            wplayer.settings.setMode("loop", true);
            wplayer.settings.volume = 50;
            wplayer.URL = @".\music\TalesofBerseriaMainMenuTheme.mp3";
            wplayer.controls.play();
            ShowRecord();
            changeCur(1);
            foreach (Control ctrl in Controls)
            {
                ctrl.MouseDown += new System.Windows.Forms.MouseEventHandler(petScreen_MouseDown);
                ctrl.MouseUp += new System.Windows.Forms.MouseEventHandler(petScreen_MouseUp);
                foreach (Control ctrl2 in ctrl.Controls)
                {
                    if(ctrl2 is Button)
                    {
                        if (ctrl2.Name.ToString() != "Title_NewGameBtn")
                        {
                            ctrl2.MouseMove += new System.Windows.Forms.MouseEventHandler(NewGameBtn_MouseMove);
                            ctrl2.MouseLeave += new System.EventHandler(NewGameBtn_MouseLeave);
                        }
                        if (ctrl2.Name.ToString() != "Title_NewGameBtn" && ctrl2.Name.ToString() != "story_nextDialog" && ctrl2.Name.ToString() != "pet_maxPos")
                        {
                            ctrl2.MouseClick += new System.Windows.Forms.MouseEventHandler(Title_NewGameBtn_MouseClick);
                        }
                        
                    }
                    if(ctrl2.Name.ToString() != "pet_catPic" && ctrl2.Name.ToString() != "pet_food" && ctrl2.Name.ToString() != "pet_can" && ctrl2.Name.ToString() != "pet_foodfollow")
                    {
                        ctrl2.MouseDown += new System.Windows.Forms.MouseEventHandler(petScreen_MouseDown);
                        ctrl2.MouseUp += new System.Windows.Forms.MouseEventHandler(petScreen_MouseUp);
                    }
                    foreach (Control ctrl3 in ctrl2.Controls)
                    {
                        if (ctrl3 is Button)
                        {
                            ctrl3.MouseMove += new System.Windows.Forms.MouseEventHandler(NewGameBtn_MouseMove);
                            ctrl3.MouseLeave += new System.EventHandler(NewGameBtn_MouseLeave);
                            ctrl3.MouseClick += new System.Windows.Forms.MouseEventHandler(Title_NewGameBtn_MouseClick);
                        }
                        ctrl3.MouseDown += new System.Windows.Forms.MouseEventHandler(petScreen_MouseDown);
                        ctrl3.MouseUp += new System.Windows.Forms.MouseEventHandler(petScreen_MouseUp);
                    }
                }
            }
            pet_talk.Parent = pet_Panel;
            pet_communication.Parent = pet_Panel;
        }

        private void changeCur(int v)
        {
            Cursor mycursor = new Cursor(Cursor.Current.Handle);
            IntPtr colorcursorhandle = LoadCursorFromFile(@".\cur\Finger"+v.ToString()+".cur");
            mycursor.GetType().InvokeMember("handle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField, null, mycursor, new object[] { colorcursorhandle });
            this.Cursor = mycursor;
        }

        private void ShowRecord()
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                ds.Clear();
                SqlDataAdapter da_story = new SqlDataAdapter("select * from dialog_CAT", cn);
                da_story.Fill(ds, "story_cat");
                SqlDataAdapter da_playerSave = new SqlDataAdapter("select * from playerSave", cn);
                da_playerSave.Fill(ds, "playerSave");
                DataTable dt = ds.Tables["playerSave"];
                if (dt.Rows[0][1].ToString().CompareTo("") != 0)
                {
                    //loadGame_goHouse1.Visible = true;
                    loadGame_loadSlotBtn01.Enabled = true;
                    loadGame_loadSlotBtn01.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt.Rows[0][0], dt.Rows[0][1], dt.Rows[0][3]);
                }
                else
                {
                    //loadGame_goHouse1.Visible = false;
                    loadGame_loadSlotBtn01.Enabled = false;
                    loadGame_loadSlotBtn01.Text = "紀錄欄位：空";
                }
                if (dt.Rows[1][1].ToString().CompareTo("") != 0)
                {
                    //loadGame_goHouse2.Visible = true;
                    loadGame_loadSlotBtn02.Enabled = true;
                    loadGame_loadSlotBtn02.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt.Rows[1][0], dt.Rows[1][1], dt.Rows[1][3]);
                }
                else
                {
                    //loadGame_goHouse2.Visible = false;
                    loadGame_loadSlotBtn02.Enabled = false;
                    loadGame_loadSlotBtn02.Text = "紀錄欄位：空";
                }
                if (dt.Rows[2][1].ToString().CompareTo("") != 0)
                {
                    //loadGame_goHouse3.Visible = true;
                    loadGame_loadSlotBtn03.Enabled = true;
                    loadGame_loadSlotBtn03.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt.Rows[2][0], dt.Rows[2][1], dt.Rows[2][3]);
                }
                else
                {
                    //loadGame_goHouse3.Visible = false;
                    loadGame_loadSlotBtn03.Enabled = false;
                    loadGame_loadSlotBtn03.Text = "紀錄欄位：空";
                }
            }
        }


        //NewGameBtn
        private void NewGameBtn_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse_btnMoveInSound();
        }

        private void NewGameBtn_MouseLeave(object sender, EventArgs e)
        {
            Mouse_btnMoveOutSound();
        }

        private void NewGameBtn_Click(object sender, EventArgs e)
        {
            DateTime delay = DateTime.Now;
            black_Panel.BringToFront();
            wplayer.controls.stop();
            timer_nowloading.Enabled = true;
            //StoryPanel.BringToFront();
            //while (delay.AddSeconds(10) > DateTime.Now) ;
            //getStory(currentStoryProcess);
        }

        //LoadBtn

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            //Mouse_btnClickSound();
            loadGame_Panel.BringToFront();
            ShowRecord();
        }

        //EndBtn

        private void EndBtn_Click(object sender, EventArgs e)
        {
            //Mouse_btnClickSound();
            Application.Exit();
        }

        /*Functions and variable about mouse*/
        Boolean MouseMoveHandle = false;

        private void Mouse_btnMoveInSound()
        {
            if (MouseMoveHandle == false)
            {
                MouseMoveHandle = true;
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                sp.SoundLocation = @".\se\Cursor1.wav";
                sp.Play();
                sp.Dispose();
            }
        }

        private void Mouse_btnMoveOutSound()
        {
            MouseMoveHandle = false;
        }

        private void Mouse_btnClickSound()
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
            sp.SoundLocation = @".\se\se_maoudamashii_system40.wav";
            sp.Play();
            sp.Dispose();
        }

        /*Loading to record btn and timer*/
        //---------------------------

        private void loadSlotBtn01_Click(object sender, EventArgs e)
        {
            int load = 0;
            loadSlot(load);
            
        }

        private void loadSlot(int load)
        {
            //Mouse_btnClickSound();
            wplayer.controls.stop();
            black_Panel.BringToFront();
            timer_LoadToGameLoading.Enabled = true;
            DataTable dt = ds.Tables["playerSave"];
            saveId = int.Parse(dt.Rows[load][0].ToString());
            petName = dt.Rows[load][1].ToString();
            currentStoryProcess = int.Parse(dt.Rows[load][2].ToString());
            positiveValue = int.Parse(dt.Rows[load][3].ToString());
        }

        //---------------------------

        private void loadSlotBtn02_Click(object sender, EventArgs e)
        {
            int load = 1;
            loadSlot(load);
        }

        //---------------------------

        private void loadSlotBtn03_Click(object sender, EventArgs e)
        {
            int load = 2;
            loadSlot(load);
        }

        //---------------------------

        private void LoadToGameLoading_Tick(object sender, EventArgs e)
        {
            timer_LoadToGameLoading.Enabled = false;
            goHouse();
        }

        //Music effect cutoff
        //private void timerMusicEffectCutOff_Tick(object sender, EventArgs e)
        //{
        //    wplayer.settings.volume -= 1;
        //    if (wplayer.settings.volume == 0)
        //    {
        //        wplayer.URL = "";
        //        wplayer.controls.stop();
        //        timerMusicEffectCutOff.Enabled = false;
        //    }
        //}

        //private void timerMusicEffectCutIn_Tick(object sender, EventArgs e)
        //{
        //    wplayer.settings.volume += 1;
        //    if (wplayer.settings.volume == 50)
        //    {
        //        timerMusicEffectCutIn.Enabled = false;
        //    }
        //}

        /*Functions and variable about story dialog*/
        private void getStory(int Process)
        {
            DataTable dt = ds.Tables["story_cat"];
            /*linq*/
            var result = from line in dt.AsEnumerable()
                         where line.Field<int>("Process") == Process
                         select new {
                             Process = line.Field<int>("Process"),
                             Function = line.Field<int>("Function"),
                             Article = line.Field<string>("Article")
                         };
            int process = result.First().Process;
            int function = result.First().Function;
            string article = result.First().Article;

            switch (function)
            {
                case -1: // background change
                    string[] line = article.Split(',');
                    line[1] = line[1].Replace("%n", "\r\n");
                    story_storyDialog.Text = line[1].Replace("!name", petName);
                    story_storyScencePicture.Image = Image.FromFile(@".\pic\bk\" + line[0]);
                    if (line.Length > 2)
                    {
                        if (line[2].CompareTo("1") == 0)
                        {                
                            currentStoryProcess += 1;
                            getStory(currentStoryProcess);
                        }
                    }
                    break;
                case 0: //normal article
                    article = article.Replace("%n", "\r\n");
                    line = article.Split(',');
                    story_storyDialog.Text = line[0].Replace("!name", petName);
                    
                    if (line[1].CompareTo("") != 0)
                    {
                        if (line[1].CompareTo("clean") == 0)
                        {
                            story_storyScencePicture.Image = null;
                            story_cat.Image = null;
                        }
                        else
                        {
                            story_storyScencePicture.Image = Image.FromFile(@".\pic\bk\home.png");
                            story_cat.Image = Image.FromFile(@".\pic\cat\" + line[1]);
                            story_catPanel.Parent = story_storyScencePicture;

                        }
                    }
                    if(line[1].CompareTo("half-cat1.png") == 0)
                    {
                        
                    }
                    break;
                case 1: //sound effect article
                    line = article.Split(',');
                    line[1] = line[1].Replace("!name", petName);
                    story_storyDialog.Text = line[1].Replace("%n", "\r\n");
                    if (line[2].CompareTo("") != 0)
                    {
                        if (line[2].CompareTo("clean") == 0)
                        {
                            story_storyScencePicture.Image = null;
                            story_cat.Image = null;
                        }
                        else
                        {
                            story_storyScencePicture.Image = Image.FromFile(@".\pic\cat\" + line[2]);
                        }
                    }
                    System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                    sp.SoundLocation = @".\se\"+line[0];
                    sp.Play();
                    sp.Dispose();
                    break;
                case 2: //inputbox
                    while (petName == "" && Process == 7) petName = Interaction.InputBox("取名字(不可不幫取貓取名字，\n本遊戲會逼你取名字，直到你取為止！)");
                    currentStoryProcess += 1;
                    getStory(currentStoryProcess);
                    break;
                case 3: //select saveslot
                    story_chooseBox.Visible = true;
                    line = article.Split(',');
                    story_nextDialog.Enabled = false;
                    line[1] = line[1].Replace("!name", petName);
                    story_storyDialog.Text = line[1].Replace("%n", "\r\n");
                    if (Process == 8)
                    {
                        using (cn = new SqlConnection())
                        {
                            cn.ConnectionString = connectionString;
                            cn.Open();
                            SqlDataAdapter da_playerSave = new SqlDataAdapter("select * from playerSave", cn);
                            da_playerSave.Fill(ds, "playerSave");
                            DataTable dt2 = ds.Tables["playerSave"];
                            story_choosebtn01.Enabled = true;
                            story_choosebtn02.Enabled = true;
                            story_choosebtn03.Enabled = true;
                            if (dt2.Rows[0][1].ToString().CompareTo("") != 0)
                            {
                                story_choosebtn01.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt2.Rows[0][0], dt2.Rows[0][1], dt2.Rows[0][3]);
                            }
                            else
                            {
                                story_choosebtn01.Text = "紀錄欄位1:空";
                            }
                            if (dt2.Rows[1][1].ToString().CompareTo("") != 0)
                            {
                                story_choosebtn02.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt2.Rows[1][0], dt2.Rows[1][1], dt2.Rows[1][3]);
                            }
                            else
                            {
                                story_choosebtn02.Text = "紀錄欄位2:空";
                            }
                            if (dt2.Rows[2][1].ToString().CompareTo("") != 0)
                            {
                                story_choosebtn03.Text = string.Format("紀錄欄位:{0} 寵物的名字:{1} 好感度:{2}", dt2.Rows[2][0], dt2.Rows[2][1], dt2.Rows[2][3]);
                            }
                            else
                            {
                                story_choosebtn03.Text = "紀錄欄位3:空";
                            }
                        }
                    }
                    break;
                case 4: //music change
                    line = article.Split(',');
                    story_storyDialog.Text = line[1].Replace("%n", "\r\n");
                    if (line[2].CompareTo("") != 0)
                    {
                        if (line[2].CompareTo("clean") == 0)
                        {
                            story_storyScencePicture.Image = null;
                            story_cat.Image = null;
                        }
                        else
                        {
                            story_storyScencePicture.Image = Image.FromFile(@".\pic\cat\" + line[2]);
                        }
                    }
                    if (line[0].CompareTo("clean") == 0)
                    {
                        wplayer.controls.stop();
                    }
                    else
                    {
                        wplayer.URL = @".\music\" + line[0];
                        wplayer.controls.play();
                    }
                    break;
                case 5:
                    black_Panel.BringToFront();
                    using (cn = new SqlConnection())
                    {
                        cn.ConnectionString = connectionString;
                        cn.Open();
                        SqlCommand cmd = new SqlCommand("SaveMode", cn); //預存程序
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                        cmd.Parameters.Add(new SqlParameter("@Process", SqlDbType.Int));
                        cmd.Parameters.Add(new SqlParameter("@PositiveValue", SqlDbType.Int));
                        cmd.Parameters["@Id"].Value = saveId;
                        cmd.Parameters["@Name"].Value = petName;
                        cmd.Parameters["@Process"].Value = currentStoryProcess;
                        cmd.Parameters["@PositiveValue"].Value = positiveValue;
                        cmd.ExecuteNonQuery();
                    }
                    timer_newgameloading.Enabled = true;
                    break;
            }
        }

        private void nextDialog_Click(object sender, EventArgs e)
        {
            if (story_storyScencePicture.Image != null) story_storyScencePicture.Image.Dispose(); // release memory from picturebox
            currentStoryProcess += 1;
            getStory(currentStoryProcess);
        }

        private void timerNowLoading(object sender, EventArgs e)
        {
            timer_nowloading.Enabled = false;
            getStory(currentStoryProcess);
            story_Panel.BringToFront();
        }

        private void newgameloading_Tick(object sender, EventArgs e)
        {
            timer_newgameloading.Enabled = false;
            goHouse();
        }

        private void choosebtn01_Click(object sender, EventArgs e)
        {
            saveId = 1;
            overWriteRecord(saveId);
        }

        private void overWriteRecord(int v)
        {
            //Mouse_btnClickSound();
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select Name from playerSave where Id = " + v, cn);
                string name = cmd.ExecuteScalar().ToString();
                if(name != "")
                {
                    DialogResult result = MessageBox.Show("你確定要覆蓋這筆紀錄嗎", "覆蓋確認", MessageBoxButtons.YesNo);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        cmd = new SqlCommand("DeleteMode", cn); //預存程序
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                        cmd.Parameters["@Id"].Value = v;
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("紀錄已覆蓋！");
                        choose(saveId);
                    }
                }
                else
                {
                    choose(v);
                }
            }
            ShowRecord();
        }

        private void choose(int saveId)
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("SaveMode", cn); //預存程序
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                cmd.Parameters.Add(new SqlParameter("@Process", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@PositiveValue", SqlDbType.Int));
                cmd.Parameters["@Id"].Value = saveId;
                cmd.Parameters["@Name"].Value = petName;
                cmd.Parameters["@Process"].Value = currentStoryProcess;
                cmd.Parameters["@PositiveValue"].Value = positiveValue;
                cmd.ExecuteNonQuery();
            }
            story_choosebtn01.Enabled = false;
            story_choosebtn02.Enabled = false;
            story_choosebtn03.Enabled = false;
            //story_choosebtn01.Text = "";
            //story_choosebtn02.Text = "";
            //story_choosebtn03.Text = "";
            currentStoryProcess += 1;
            story_nextDialog.Enabled = true;
            story_chooseBox.Visible = false;
            getStory(currentStoryProcess);
        }

        private void choosebtn02_Click(object sender, EventArgs e)
        {
            saveId = 2;
            overWriteRecord(saveId);
        }

        private void choosebtn03_Click(object sender, EventArgs e)
        {
            saveId = 3;
            overWriteRecord(saveId);
        }


        //gamePanelControl
        private void gameSaveBtn_Click(object sender, EventArgs e)
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("SaveMode", cn); //預存程序
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar));
                cmd.Parameters.Add(new SqlParameter("@Process", SqlDbType.Int));
                cmd.Parameters.Add(new SqlParameter("@PositiveValue", SqlDbType.Int));
                cmd.Parameters["@Id"].Value = saveId;
                cmd.Parameters["@Name"].Value = petName;
                cmd.Parameters["@Process"].Value = currentStoryProcess;
                cmd.Parameters["@PositiveValue"].Value = positiveValue;
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("紀錄已保存！");
        }

        private void gameEndBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadGame_DeleteRecordBtn01_Click(object sender, EventArgs e)
        {
            int deletevalue = 1;
            deleterecord(deletevalue);
        }

        private void deleterecord(int deletevalue)
        {
            //Mouse_btnClickSound();
            DialogResult result = MessageBox.Show("你確定要刪除這筆紀錄嗎", "刪除確認", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                using (cn = new SqlConnection())
                {
                    cn.ConnectionString = connectionString;
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("DeleteMode", cn); //預存程序
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    cmd.Parameters["@Id"].Value = deletevalue;
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("紀錄已刪除！");
                ShowRecord();
            }
        }

        private void loadGame_DeleteRecordBtn02_Click(object sender, EventArgs e)
        {
            int deletevalue = 2;
            deleterecord(deletevalue);
        }

        private void loadGame_DeleteRecordBtn03_Click(object sender, EventArgs e)
        {
            int deletevalue = 3;
            deleterecord(deletevalue);
        }

        private void game_returnToTitle_Click(object sender, EventArgs e)
        {
            //Mouse_btnClickSound();
            Title_Panel.BringToFront();
            cleanCurrent();
        }

        private void updatepos()
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select PositiveValue from playerSave where Id = " + saveId.ToString(), cn);
                int currentpos = (int)cmd.ExecuteScalar();
                pet_pos.Width = currentpos * 2;
                if (currentpos == 100)
                {
                    pet_maxPos.Visible = true;
                }
                else
                {
                    pet_maxPos.Visible = false;
                }
            }
        }

        int x, y;
        Boolean left, down;
        Boolean l, d;
        private void timer_petMove_Tick(object sender, EventArgs e)
        {
            int mx = 2,my = 1;

            if (left == true && pet_catPic.Location.X < x || left == false && pet_catPic.Location.X > x)
            {
                l = true;
                mx = 0;
            }
            if (down == true && pet_catPic.Location.Y > y || down == false && pet_catPic.Location.Y < y)
            {
                d = true;
                my = 0;
            }
            if (l == true && d == true) movestop();
            if (left == true) pet_catPic.Left -= mx;
            else pet_catPic.Left += mx;
            if (down == true) pet_catPic.Top += my;
            else pet_catPic.Top -= my;

        }

        private void pet_catPic_MouseMove(object sender, MouseEventArgs e)
        {
            if(timer_petRun.Enabled == false)
            {
                pet_catPic.Image = Image.FromFile(@".\pic\cat\stand1.png");
                timer_petMove.Enabled = false;
                timer_petStand.Enabled = false;
                changeCur(3);
            }
        }

        private void pet_catPic_MouseLeave(object sender, EventArgs e)
        {
            if (timer_petRun.Enabled == false)
            {
                timer_petStand.Enabled = true;
                changeCur(1);
            }
        }

        public void petScreen_MouseDown(object sender, MouseEventArgs e)
        {
            changeCur(2);
        }

        public void petScreen_MouseUp(object sender, MouseEventArgs e)
        {
            changeCur(1);
        }

        private void pet_catPic_MouseUp(object sender, MouseEventArgs e)
        {
            if (timer_petRun.Enabled == false)
            {
                changeCur(3);
            }
        }

        private void pet_catPic_MouseDown(object sender, MouseEventArgs e)
        {
            if (timer_petRun.Enabled == false)
            {
                changeCur(4);
            }
        }

        private void pet_catPic_Click(object sender, EventArgs e)
        {
            if(timer_petRun.Enabled == false)
            {
                showTalk(0);
                pluspositive(1,"摸了!name一下，親密度增加!pos");
            }
            
            //pet_communication.Text = "你為什麼要碰我肩膀";
            //pet_communication.Visible = true;
            //pet_communication.BringToFront();
        }

        private void showTalk(int v)
        {
            string txt;
            Random r = new Random();
            int rcount;
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from PetTalk where Id like '" + v.ToString() + "_' + '%'", cn);
                rcount = (int)cmd.ExecuteScalar();
                cmd = new SqlCommand("SELECT Talk from PetTalk where Id = @id", cn);
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.NVarChar));
                cmd.Parameters["@Id"].Value = v.ToString() + "_" + r.Next(1, rcount + 1).ToString();
                txt = cmd.ExecuteScalar().ToString();
            }
            pet_communication.Text = txt;
            pet_talk.Width = pet_communication.Width + 15;
            pet_talk.Top = pet_catPic.Location.Y - 60;
            pet_talk.Left = pet_catPic.Location.X + pet_catPic.Size.Width / 2 - pet_talk.Size.Width / 2;
            pet_communication.Top = pet_talk.Location.Y + 15;
            pet_communication.Left = pet_talk.Location.X + (pet_talk.Size.Width - pet_communication.Size.Width) / 2;
            pet_talk.Visible = true;
            pet_communication.Visible = true;
        }

        private void pet_communication_Click(object sender, EventArgs e)
        {
            //pet_communication.Visible = false;
            //MessageBox.Show(pet_communication.Text.Length.ToString());
        }

        private void loadGame_loadReturnToTitleBtn_Click(object sender, EventArgs e)
        {
            //Mouse_btnClickSound();
            Title_Panel.BringToFront();
            cleanCurrent();
        }

        private void cleanCurrent()
        {
            petName = "";
            currentStoryProcess = -1;
            positiveValue = 0;
            story_catPanel.Parent = null;
            story_cat.Image = null;
        }

        private void pet_can_MouseMove(object sender, MouseEventArgs e)
        {
            pet_can.Width = 80;
            pet_can.Height = 80;
            changeCur(3);
        }

        private void pet_can_MouseLeave(object sender, EventArgs e)
        {
            pet_can.Width = 70;
            pet_can.Height = 70;
            changeCur(1);
        }

        private void pet_food_MouseMove(object sender, MouseEventArgs e)
        {
            pet_food.Width = 80;
            pet_food.Height = 80;
            changeCur(3);
        }

        private void pet_food_MouseLeave(object sender, EventArgs e)
        {
            pet_food.Width = 70;
            pet_food.Height = 70;
            changeCur(1);
        }

        int nowfood;
        private void pet_food_Click(object sender, EventArgs e)
        {
            nowfood = 2;
            foodfollow("./pic/cat/food.png");
            changeCur(4);
        }

        private void foodfollow(string v)
        {
            pet_foodfollow.Image = Image.FromFile(@""+v);
            pet_food.Visible = false;
            pet_can.Visible = false;
            timer_foodfollow.Enabled = true;
            pet_foodfollow.Visible = true;
            pet_foodfollow.BringToFront();
        }

        private void pet_can_Click(object sender, EventArgs e)
        {
            nowfood = 4;
            foodfollow("./pic/cat/can.png");
            changeCur(4);
        }

        private void timer_foodfollow_Tick(object sender, EventArgs e)
        {
            pet_foodfollow.Left = mousex-5;
            pet_foodfollow.Top = mousey-5;
            changeCur(4);
        }

        int mousex, mousey;

        private void pet_Panel_MouseMove(object sender, MouseEventArgs e)
        {
            mousex = e.Location.X;
            mousey = e.Location.Y;
        }

        private void pet_foodfollow_Click(object sender, EventArgs e)
        {
            MouseEventArgs ee = (MouseEventArgs)e;
            changeCur(1);
            if (ee.Button == System.Windows.Forms.MouseButtons.Right)
            {
                timer_foodfollow.Enabled = false;
                pet_catPic.BringToFront();
                pet_foodfollow.Visible = false;
                pet_can.Visible = true;
                pet_food.Visible = true;
                return;
            }
            if (pet_foodfollow.Location.X > 280 && pet_foodfollow.Location.X < 628 && pet_foodfollow.Location.Y>364 && pet_foodfollow.Location.Y < 541)
            {
                timer_foodfollow.Enabled = false;
                pet_catPic.BringToFront();
                timer_petStand.Enabled = false;
                timer_petMove.Enabled = false;
                x = pet_foodfollow.Location.X;
                y = pet_foodfollow.Location.Y - 30;
                if (pet_catPic.Location.X > x)
                {
                    left = true;
                    pet_catPic.Image = Image.FromFile(@".\pic\cat\movingl.gif");
                }
                else
                {
                    left = false;
                    pet_catPic.Image = Image.FromFile(@".\pic\cat\movingr.gif");
                }
                if (pet_catPic.Location.Y < y)
                {
                    down = true;
                }
                else
                {
                    down = false;
                }
                l = false; d = false;
                timer_petRun.Enabled = true;
                showTalk(nowfood);
            }
            else
            {
                showTalk(1);
                timer_petMove.Enabled = false;
                timer_petStand.Enabled = true;
                pet_catPic.Image = Image.FromFile(@".\pic\cat\stand1.png");
            }
            
        }

        private void timer_petRun_Tick(object sender, EventArgs e)
        {
            int mx = 8, my = 8;

            if (left == true && pet_catPic.Location.X < x || left == false && pet_catPic.Location.X > x)
            {
                l = true;
                mx = 0;
            }
            if (down == true && pet_catPic.Location.Y > y || down == false && pet_catPic.Location.Y < y)
            {
                d = true;
                my = 0;
            }
            if (l == true && d == true)
            {
                pet_foodfollow.Visible = false;
                pet_food.Visible = true;
                pet_can.Visible = true;
                movestop();
                if (nowfood == 2)
                    pluspositive(2, "餵食了!name貓飼料，親密度增加!pos");
                else
                    pluspositive(2, "餵食了!name貓罐頭，親密度增加!pos");
            }
            if (left == true) pet_catPic.Left -= mx;
            else pet_catPic.Left += mx;
            if (down == true) pet_catPic.Top += my;
            else pet_catPic.Top -= my;
            pet_talk.Top = pet_catPic.Location.Y - 60;
            pet_talk.Left = pet_catPic.Location.X + pet_catPic.Size.Width / 2 - pet_talk.Size.Width / 2;
            pet_communication.Top = pet_talk.Location.Y + 15;
            pet_communication.Left = pet_talk.Location.X + (pet_talk.Size.Width - pet_communication.Size.Width) / 2;
        }

        private void pluspositive(int v ,string record)
        {
            Random r = new Random();
            int change;
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select PositiveValue from playerSave where Id = " + saveId.ToString(), cn);
                int currentpos = (int)cmd.ExecuteScalar();
                change = currentpos;
                currentpos += r.Next(5, 11) * v;
                if (currentpos > 100) currentpos = 100;
                if (currentpos < 0) currentpos = 0;
                change = currentpos - change;
                cmd = new SqlCommand("update playerSave set PositiveValue = " + currentpos + " where Id = " + saveId.ToString(), cn);
                cmd.ExecuteNonQuery();
            }
            updatepos();
            if (change < 0) change = 0 - change;
            recordPlayRecord(record, change);
        }

        private void recordPlayRecord(string record, int r)
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select Name from playerSave where Id = " + saveId.ToString(), cn);
                string name = cmd.ExecuteScalar().ToString();
                cmd = new SqlCommand("insert into playRecord values(N'" + record.Replace("!name",name).Replace("!pos",r.ToString()) + "','" + saveId + "')", cn);
                cmd.ExecuteNonQuery();
            }
            updatePlayRecord();
        }

        private void updatePlayRecord()
        {
            pet_playRecord.Text = "操作紀錄：\r\n";
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select Record from playRecord where playerId = " + saveId.ToString(), cn);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    pet_playRecord.AppendText(sdr[0].ToString() + "\r\n");
                }
            }
        }

        private void pet_punch_Click(object sender, EventArgs e)
        {
            if (timer_petRun.Enabled == false)
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                sp.SoundLocation = @".\se\se_maoudamashii_battle15.wav";
                sp.Play();
                sp.Dispose();
                pet_catPic.Image = Image.FromFile(@".\pic\cat\stand1.png");
                showTalk(3);
                pluspositive(-2,"打了!name一下，親密度減少!pos");
                punch = 0;
                pet_punch.Visible = false;
                timer_petStand.Enabled = false;
                timer_petMove.Enabled = false;
                timer_petPunch.Enabled = true;
            }
        }

        private void pet_punch_MouseMove(object sender, MouseEventArgs e)
        {
            pet_punch.Width = 80;
            pet_punch.Height = 80;
        }

        private void pet_punch_MouseLeave(object sender, EventArgs e)
        {
            pet_punch.Width = 70;
            pet_punch.Height = 70;
        }


        int punch;
        private void timer_petPunch_Tick(object sender, EventArgs e)
        {
            punch++;
            if (punch <= 3)
            {
                pet_catPic.Top -= 6;
                pet_talk.Top -= 6;
                pet_communication.Top -= 6;
            }
            else if (punch <= 6)
            {
                pet_catPic.Top += 6;
                pet_talk.Top += 6;
                pet_communication.Top += 6;
            }
            else
            {
                timer_petPunch.Enabled = false;
                timer_petStand.Enabled = true;
                pet_punch.Visible = true;
            }
        }

        private void pet_posborder_Click(object sender, EventArgs e)
        {
            showpos();
        }

        private void pet_pos_Click(object sender, EventArgs e)
        {
            showpos();
        }

        private void petScreen_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void Title_NewGameBtn_MouseClick(object sender, MouseEventArgs e)
        {
            Mouse_btnClickSound();
        }

        private void pet_returnToTitle_Click(object sender, EventArgs e)
        {
            Title_Panel.BringToFront();
            petReturnOrMaxPos();
            wplayer.URL = @".\music\TalesofBerseriaMainMenuTheme.mp3";
            wplayer.controls.play();
        }

        private void petReturnOrMaxPos()
        {
            cleanCurrent();
            timer_petMove.Enabled = false;
            timer_petStand.Enabled = false;
            timer_foodfollow.Enabled = false;
            timer_petRun.Enabled = false;
            timer_petPunch.Enabled = false;
            timer_nope.Enabled = false;
            pet_catPic.BringToFront();
            pet_foodfollow.Visible = false;
            pet_can.Visible = true;
            pet_food.Visible = true;
            pet_talk.Visible = false;
            pet_communication.Visible = false;
            nope_word.Visible = false;
            nope_countdown.Text = "3";
            nope_countdown.Visible = true;
            nope_pic.Visible = false;
            wplayer.controls.stop();
        }

        private void pet_maxPos_Click(object sender, EventArgs e)
        {
            petReturnOrMaxPos();
            nope_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            nope_pic.Top = -977;
            nope_pic.Left = -634;
            nope_Panel.BringToFront();
            nope = 0;
            timer_countdown.Enabled = true;
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
            sp.SoundLocation = @".\se\321.wav";
            sp.Play();
            sp.Dispose();
        }

        int nope;
        private void timer_nope_Tick(object sender, EventArgs e)
        {
            nope++;
            if (nope <= 70)
            {

            }
            else if (nope == 270)
            {
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                sp.SoundLocation = @".\se\Wow.wav";
                sp.Play();
                sp.Dispose();
            }
            else if (nope <= 290)
            {
                nope_pic.Top -= 4;
            }
            else if(nope <= 440)
            {
                
            }
            else
            {
                nope_pic.Top = -367;
                nope_pic.Left = -411;
                nope_word.Visible = true;
                timer_nope.Enabled = false;
            }
        }

        private void Title_mute_Click(object sender, EventArgs e)
        {
            wplayer.settings.volume = 0;
        }

        private void Title_voldown_Click(object sender, EventArgs e)
        {
            wplayer.settings.volume -= 10;
            Mouse_btnClickSound();
        }

        private void Title_volup_Click(object sender, EventArgs e)
        {
            wplayer.settings.volume += 10;
            Mouse_btnClickSound();
        }

        private void timer_countdown_Tick(object sender, EventArgs e)
        {
            if (nope_countdown.Text == "1")
            {
                nope_countdown.Visible = false;
                timer_countdown.Enabled = false;
                nope_pic.Visible = true;
                timer_nope.Enabled = true;
                System.Media.SoundPlayer sp = new System.Media.SoundPlayer();
                sp.SoundLocation = @".\se\Ah.wav";
                sp.Play();
                sp.Dispose();
            }
            if (nope_countdown.Text == "2") nope_countdown.Text = "1";
            if (nope_countdown.Text == "3") nope_countdown.Text = "2";
        }

        private void loadGame_goHouse1_Click(object sender, EventArgs e)
        {
            saveId = 0;
            goHouse();
        }

        private void goHouse()
        {
            pet_Panel.BringToFront();
            wplayer.URL = @".\music\bgm_maoudamashii_piano30.mp3";
            wplayer.controls.play();
            pet_catPic.Image = Image.FromFile(@".\pic\cat\stand1.png");
            timer_petStand.Enabled = true;
            updatepos();
            updatePlayRecord();
        }

        private void loadGame_goHouse2_Click(object sender, EventArgs e)
        {
            saveId = 1;
            goHouse();
        }

        private void loadGame_goHouse3_Click(object sender, EventArgs e)
        {
            saveId = 2;
            goHouse();
        }

        private void showpos()
        {
            using (cn = new SqlConnection())
            {
                cn.ConnectionString = connectionString;
                cn.Open();
                SqlCommand cmd = new SqlCommand("select PositiveValue from playerSave where Id = " + saveId.ToString(), cn);
                int currentpos = (int)cmd.ExecuteScalar();
                MessageBox.Show("目前的親密度："+currentpos+"/100","親密度");
            }
        }

        private void movestop()
        {
            if (left == true) pet_catPic.Image = Image.FromFile(@".\pic\cat\standl.png");
            else pet_catPic.Image = Image.FromFile(@".\pic\cat\standr.png");
            timer_petRun.Enabled = false;
            timer_petMove.Enabled = false;
            timer_petStand.Enabled = true;
        }

        private void timer_petStand_Tick(object sender, EventArgs e)
        {
            randommove();
            pet_communication.Visible = false;
            pet_talk.Visible = false;
            timer_petMove.Enabled = true;
            timer_petStand.Enabled = false;
        }

        private void randommove()
        {
            Random r = new Random();
            x = r.Next(280, 628);
            y = r.Next(364, 541);
            if (pet_catPic.Location.X > x)
            {
                left = true;
                pet_catPic.Image = Image.FromFile(@".\pic\cat\movingl.gif");
            }
            else
            {
                left = false;
                pet_catPic.Image = Image.FromFile(@".\pic\cat\movingr.gif");
            }
            if (pet_catPic.Location.Y < y)
            {
                down = true;
            }
            else
            {
                down = false;
            }
            l = false; d = false;
            //MessageBox.Show(x.ToString()+" "+y.ToString());
        }
    }

    
}
