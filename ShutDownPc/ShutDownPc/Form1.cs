using System;
using System.Diagnostics;
using System.Windows.Forms;
using Bunifu.UI.WinForms;
using System.Speech.Synthesis;
using System.Globalization;
using System.IO;
using WMPLib;
using System.Media;
using NAudio;
using NAudio.Wave;
namespace ShutDownPc
{
    public partial class Form1 : Form
    {
        Stopwatch stopwatch;
        Stopwatch stopwatch2;
        ShutDownMethod shutdown;
        SpeechSynthesizer synthesizer;
        SoundPlayer player1;
        WindowsMediaPlayer windowsMedia;
        TimeSpan timespan;
        IWavePlayer waveOutDevice;
        AudioFileReader audioFileReader;
        private int hours; 
        private int minutes;
        private int timeForProgresBar;
        private int remainingSec;
        private int currentCond;
        private string filename;
        private int seconds;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            windowsMedia = new WindowsMediaPlayer();
            bunifuPages1.SetPage(1);
            stopwatch = new Stopwatch();
            stopwatch2 = new Stopwatch();
            shutdown = new ShutDownMethod();
            synthesizer = new SpeechSynthesizer();
            player1 = new SoundPlayer();
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child, 0, CultureInfo.GetCultureInfo("ru-RU"));
            remainingSec = bunifuProgressBar2.Maximum;
            waveOutDevice = new WaveOut();
            seconds = 60;
            bunifuLabel13.Text = timespan.ToString();
            timespan = new TimeSpan();
            LeftTime = timespan;
            
        }

        public TimeSpan LeftTime { get; set; }

        private void stopwatchButton_Click_1(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(0);
        }
        private void sleepTimer_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(1); 
        }

        int[] a = {60, 30, 10, 5, 1 };
        private void hmTimer_Tick(object sender, EventArgs e)
        {
            seconds = seconds - 1;
            if (hours > 0) {
                minutes = hours * 60;
                hours = 0;
            };
            if ((minutes - stopwatch2.Elapsed.Minutes).ToString() == "1") remainingtime.Text = (minutes - stopwatch2.Elapsed.Minutes).ToString() + " minute(s)";
            else remainingtime.Text = (minutes - stopwatch2.Elapsed.Minutes).ToString() + " minute(s)";
            bunifuLabel13.Text = timespan.Add(TimeSpan.FromHours(hours - stopwatch2.Elapsed.Hours)).Add(TimeSpan.FromMinutes((minutes - stopwatch.Elapsed.Minutes) - 1)).Add(TimeSpan.FromSeconds(seconds - stopwatch.Elapsed.Seconds)).ToString();
            if (stopwatch2.Elapsed.Hours == hours && stopwatch2.Elapsed.Minutes == minutes)
            {
                StopTimerAndStopwatch();
                a = new int[5] { 60, 30, 10, 5, 1 };
            }
            else
            {
                if (bunifuToggleSwitch2.Checked)
                {
                    if (minutes - stopwatch2.Elapsed.Minutes == a[0] || minutes - stopwatch2.Elapsed.Minutes == a[1] || minutes - stopwatch2.Elapsed.Minutes == a[2] || minutes - stopwatch2.Elapsed.Minutes == a[3] || minutes - stopwatch2.Elapsed.Minutes == a[4])
                    {
                        if (minutes - stopwatch2.Elapsed.Minutes == a[4]) synthesizer.SpeakAsync("Осталось одна минута");
                        else
                        {
                            synthesizer.SpeakAsync("Осталось " + ((minutes - stopwatch2.Elapsed.Minutes)).ToString() + "минут");
                        }

                        for (int i = 0; i < a.Length; i++) if (a[i] == minutes - stopwatch2.Elapsed.Minutes) a[i] = 0;
                    }
                }
               
            }
            try
            {
                bunifuProgressBar2.Value = timeForProgresBar++;
            }
            catch (Exception)
            {
                StopTimerAndStopwatch();
            }
         

            
        }

        private void StopTimerAndStopwatch()
        {
            Console.Beep();
            stopwatch2.Stop();
            timer4.Stop();
            hmTimer.Stop();
            timeForProgresBar = 0;
            bunifuButton4.Text = "START";
            bunifuLabel13.Text = timespan.ToString();
            this.Text = "Little Sleepy Head";
            bunifuProgressBar2.Value = 0;
            remainingtime.Text = "0 minute(s)";
            hours = 0;
            minutes = 0;
            seconds = 60;
            dropdownMinutes.Enabled = true;
            dropdownHours.Enabled = true;
            if (bunifuToggleSwitch1.Checked)
            {
                shutdown.Shutdown();
                CloseWindows();
            };

            if (playMusicYesno.Checked)
            {
                //player1.SoundLocation = filename;
                //player1.Play();
                if (filename.EndsWith("wav"))
                {
                    player1.SoundLocation = filename;
                    player1.Play();
                    bunifuButton7.Text = "Playing";
                }
                else if (filename.EndsWith("mp3"))
                {
                    audioFileReader = new AudioFileReader(filename);
                    waveOutDevice.Init(audioFileReader);
                    waveOutDevice.Play();
                    bunifuButton7.Text = "Playing";
                }
                //MessageBox.Show("Вы можете приостановить музыку нажав на название трека", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //windowsMedia.URL = filename;
                //windowsMedia.controls.play();
            }
        }
        private void StartTimerAndStopwatch()
        {
            stopwatch2.Start();
            hmTimer.Start();
            if (hours>0)
            {
                minutes = minutes + hours * 60;
                hours = 0;
            }
            timeForProgresBar = 0;
            this.Text = "Launched...";
            bunifuButton4.Text = "PAUSE";
        }
        private void CloseWindows()
        {
            foreach (var item in Process.GetProcesses())
            {
                item.CloseMainWindow();
                item.Close();
            }
        }
        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            minutes = Convert.ToInt32(dropdownMinutes.SelectedItem);
            hours = Convert.ToInt32(dropdownHours.SelectedItem);


            if (hours == 0 && minutes == 0)
            {
                bunifuSnackbar1.Show(this, "Please, enter the time in the Text Box or in Drop Down", BunifuSnackbar.MessageTypes.Error);
            }
            else
            {
                if (bunifuButton4.Text == "START")
                {

                    stopwatch2.Reset();
                    timer4.Start();
                    if (hours == 0) bunifuProgressBar2.Maximum = minutes * 60;
                    else if (minutes == 0) bunifuProgressBar2.Maximum = (hours * 60) * 60;
                    else bunifuProgressBar2.Maximum = (hours * 3600) + (minutes * 60);
                    StartTimerAndStopwatch();
                    dropdownMinutes.Enabled = false;
                    dropdownHours.Enabled = false;
                    waveOutDevice.Stop();
                    Console.Beep();
                }
                else if (bunifuButton4.Text == "CONTINUE")
                {
                    stopwatch2.Start();
                    hmTimer.Start();
                    //timer4.Start();
                   // minutes = minutes - 1;
                    this.Text = "Launched...";
                    bunifuButton4.Text = "PAUSE";
                }
                else
                {
                    hmTimer.Stop();
                    stopwatch2.Stop();
                   // timer4.Stop();
                    this.Text = "Pause...";
                    bunifuButton4.Text = "CONTINUE";
                }
            }
        }

        private void bunifuLabel8_Click(object sender, EventArgs e)
        {
            bunifuSnackbar1.Show(this, "Shut Down - компьютер автоматически выключается по завершении таймера\nVoice assistant - вам будет сообщать оставшееся время\nPlay music - программа автоматически запустит выбранную вами музыку, по окончании таймера");
        }
        private void dropdownHours_SelectedIndexChanged(object sender, EventArgs e)
        {
            hours = int.Parse(dropdownHours.SelectedItem.ToString());
           
        }

        private void dropdownMinutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            minutes = int.Parse(dropdownMinutes.SelectedItem.ToString());
        }
        //Not necessary

        private void timerButton_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage(2);
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open a Text File";
            ofd.Filter = "MP3 files |*.mp3|(*.wav)|*.wav";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo info = new FileInfo(ofd.FileName);
                filename = ofd.FileName;
                if (info.Length / 1048576 <= 50)
                {
                    musicName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                    playMusicYesno.Checked = true;
                }
                else
                {
                    MessageBox.Show($"Файл слишком большой! Минимальный размер 10мб\nРазмер файла: {info.Length / 1048576} мб", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void musicName_Click(object sender, EventArgs e)
        {
           

        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Console.Beep();
            waveOutDevice.Stop();
            bunifuButton7.Text = "PLAY";
            timespan = timespan.Negate();
            musicName.Text = "None";
            filename = "";
            seconds = 60;
            bunifuLabel12.Text = timespan.ToString();
            bunifuLabel13.Text = timespan.ToString();
            stopwatch2.Stop();
            stopwatch2.Reset();
            timer4.Stop();
            hmTimer.Stop();
            timeForProgresBar = 0;
            bunifuButton4.Text = "START";
            this.Text = "Little Sleepy Head";
            bunifuProgressBar2.Value = 0;
            remainingtime.Text = "0 minute(s)";
            hours = 0;
            minutes = 0;
            dropdownMinutes.Enabled = true;
            dropdownHours.Enabled = true;
            a = new int[5] { 60, 30, 10, 5, 1 };
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel3_Click(object sender, EventArgs e)
        {
           
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
           // stopwatchText.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", stopwatch.Elapsed);
            //  bunifuLabel6.Text = (stopwatch2.Elapsed.Seconds).ToString();
           // bunifuLabel6.Text= string.Format("{0:hh\\:mm\\:ss}", stopwatch2.Elapsed);
            bunifuLabel12.Text = string.Format("{0:hh\\:mm\\:ss}", stopwatch2.Elapsed);
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if (bunifuButton1.Text == "START")
            {
                timer1.Start();
                stopwatch.Start();
                bunifuButton1.Text = "PAUSE";
            }
            else
            {
                timer1.Stop();
                stopwatch.Stop();
                bunifuButton1.Text = "START";
            }
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            stopwatchText.Text = "00:00:00.00";
            stopwatch.Reset();
            bunifuButton1.Text = "START";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            stopwatchText.Text = string.Format("{0:hh\\:mm\\:ss\\:ff}", stopwatch.Elapsed);
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/o101xd");
        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            
            FullScreen fullScreen = new FullScreen();
            fullScreen.Show();
        }
        void ChangeWholeForm()
        {

        }

        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            try
            {
                player1.Stop();
                if (waveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    waveOutDevice.Pause();
                    bunifuButton7.Text = "PLAY";
                }
                else if(waveOutDevice.PlaybackState == PlaybackState.Paused)
                {
                    waveOutDevice.Play();
                    bunifuButton7.Text = "PAUSE";
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Дождитесь окончания таймера", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
