using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LaserHarp
{
    public partial class MainForm : Form
    {
        List<SoundFile> soundFiles;
        ComboBox[] soundFileChoosers;
        NumericUpDown[] pitchBendChoosers;
        NumericUpDown[] volumeChoosers;

        bool started;  // start button pressed
        bool connected; // a heartbeat heard in the last 2 seconds

        public EventHandler StateChanged;

        public MainForm()
        {
            InitializeComponent();
            playModeCombo.SelectedIndex = 0;

            soundFileChoosers = new ComboBox[] { soundFile1, soundFile2, soundFile3, soundFile4, soundFile5, soundFile6, soundFile7, soundFile8 };
            pitchBendChoosers = new NumericUpDown[] { pitchBend1, pitchBend2, pitchBend3, pitchBend4, pitchBend5, pitchBend6, pitchBend7, pitchBend8 };
            volumeChoosers = new NumericUpDown[] { volume1, volume2, volume3, volume4, volume5, volume6, volume7, volume8 };

            RefreshSoundFiles();
        }

        public SoundToPlay[] Sounds
        {
            get
            {
                SoundToPlay[] result = new SoundToPlay[soundFileChoosers.Length];
                PlayMode playMode = (PlayMode)(playModeCombo.SelectedIndex);
                for (int i = 0; i < soundFileChoosers.Length; ++i) {
                    if (soundFileChoosers[i].SelectedItem != null)
                        result[i] = new SoundToPlay(((SoundFile)(soundFileChoosers[i].SelectedItem)).fileName, playMode, (int)pitchBendChoosers[i].Value, (float) volumeChoosers[i].Value / 100);
                }
                return result;
            }
        }

        public bool Started
        {
            get
            {
                return started;
            }
            set
            {
                if (value == started)
                    return;

                if (value) {
                    startStopButton.Text = "Stop";
                    foreach (Control c in soundFileChoosers) { c.Enabled = false; }
                    foreach (Control c in pitchBendChoosers) { c.Enabled = false; }
                    foreach (Control c in volumeChoosers) { c.Enabled = false; }
                    buttonRefreshSounds.Enabled = false;
                    playModeCombo.Enabled = false;

                    statusLabel.Text = "Waiting For Data";
                    statusLabel.ForeColor = Color.Orange;
                    connected = false;
                }
                else {
                    startStopButton.Text = "Start";
                    foreach (Control c in soundFileChoosers) { c.Enabled = true; }
                    foreach (Control c in pitchBendChoosers) { c.Enabled = true; }
                    foreach (Control c in volumeChoosers) { c.Enabled = true; }
                    buttonRefreshSounds.Enabled = true;
                    playModeCombo.Enabled = true;

                    statusLabel.Text = "";
                    statusLabel.ForeColor = Color.Black;
                    connected = false;
                }

                started = value;

                if (StateChanged != null)
                    StateChanged(this, EventArgs.Empty);
            }
        }

        public void ErrorMessage(string s)
        {
            if (this.IsHandleCreated)
                this.Invoke(new Action<string>(UpdateStatusForErrorMessage), s);
        }

        void UpdateStatusForErrorMessage(string s)
        {
            statusLabel.Text = s;
            statusLabel.ForeColor = Color.Red;
        }

        public void DataReceived()
        {
            // This can be called on another thread.
            if (this.IsHandleCreated)
                this.Invoke(new Action(UpdateStatusForDataReceived));
        }


        void UpdateStatusForDataReceived()
        {
            if (started) {
                if (!connected) {
                    connected = true;
                    statusLabel.Text = "Connected";
                    statusLabel.ForeColor = Color.Green;
                    connectionTimer.Stop();
                    connectionTimer.Enabled = true;
                    connectionTimer.Start();
                }
                else {
                    connectionTimer.Stop();
                    connectionTimer.Enabled = true;
                    connectionTimer.Start();
                }
            }
        }
        void TimerExpired()
        {
            if (started) {
                if (connected) {
                    connected = false;
                    statusLabel.Text = "Waiting For Data";
                    statusLabel.ForeColor = Color.Orange;
                    connectionTimer.Stop();
                    connectionTimer.Enabled = false;
                }
            }
        }

        void RefreshSoundFiles()
        {
            soundFiles = new List<SoundFile>();

            DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(typeof(MainForm).Assembly.Location));
            foreach (FileInfo file in directory.EnumerateFiles("*.wav", SearchOption.AllDirectories)) {
                soundFiles.Add(new SoundFile(file.FullName));
            }

            foreach (ComboBox combo in soundFileChoosers) {
                object value = combo.SelectedItem;
                combo.Items.Clear();
                combo.Items.AddRange(soundFiles.ToArray());
                if (value != null && combo.Items.Contains(value))
                    combo.SelectedItem = value;
            }
        }

        private void buttonRefreshSounds_Click(object sender, EventArgs e)
        {
            RefreshSoundFiles();
        }

        private void connectionTimer_Tick(object sender, EventArgs e)
        {
            TimerExpired();
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            Started = !Started;
        }
    }

    public class SoundFile
    {
        public string fileName;
        public string soundName;

        public SoundFile(string filename)
        {
            this.fileName = filename;
            this.soundName = Path.GetFileNameWithoutExtension(filename);
        }
        public override string ToString()
        {
            return soundName;
        }

        public override bool Equals(object obj)
        {
            SoundFile other = obj as SoundFile;
            if (other == null)
                return false;
            else
                return other.fileName == this.fileName;
        }

        public override int GetHashCode()
        {
            return fileName.GetHashCode();
        }
    }

    public enum PlayMode { Once, OnOff, Looping, ContinuousLoop }

    public class SoundToPlay
    {
        public SoundFile soundFile;
        public float pitchShiftInSemitones;
        public float volume;
        public PlayMode playMode;

        public SoundToPlay(string fileName, PlayMode playMode, int pitchShift, float volume = 1.0F)
        {
            this.soundFile = new SoundFile(fileName);
            this.playMode = playMode;
            this.pitchShiftInSemitones = pitchShift;
            this.volume = volume;
        }
    }
}
