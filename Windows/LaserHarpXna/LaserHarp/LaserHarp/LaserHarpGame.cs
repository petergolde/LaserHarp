using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO.Ports;
using SysWinForms = System.Windows.Forms;
using System.IO;

namespace LaserHarp
{
    public class LaserHarpGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SerialPort serialPort;
        bool connected = false;
        bool heartbeatDetected = false;

        MainForm mainForm;

        const int numberOfNotes = 8;
        SoundToPlay[] sounds;
        List<SoundEffect> noteSoundEffects = new List<SoundEffect>();
        List<SoundEffectInstance> noteSoundEffectInstances = new List<SoundEffectInstance>();
        KeyboardState oldKeyboardState = Keyboard.GetState();

        public LaserHarpGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 200;
            graphics.PreferredBackBufferHeight = 100;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            SysWinForms.Form gameWindowForm = (SysWinForms.Form)SysWinForms.Form.FromHandle(this.Window.Handle);
            gameWindowForm.Shown += new EventHandler(gameWindowForm_Shown);

            mainForm = new MainForm();
            mainForm.HandleDestroyed += new EventHandler(mainForm_HandleDestroyed);
            mainForm.StateChanged += new EventHandler(mainForm_StateChanged);
            mainForm.Show();

            base.Initialize();
        }

        void OpenSerialPort()
        {
            if (connected)
                CloseSerialPort();

            mainForm.Started = true;

            string serialPortName = SerialPort.GetPortNames().FirstOrDefault();
            if (!string.IsNullOrEmpty(serialPortName)) {
                try {
                    serialPort = new SerialPort(serialPortName, 57600, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                    serialPort.DataReceived += serialPort_DataReceived;
                    serialPort.ErrorReceived += serialPort_ErrorReceived;
                    heartbeatDetected = false;
                }
                catch (Exception e) {
                    mainForm.ErrorMessage(e.Message);
                }
            }
            else {
                mainForm.ErrorMessage("No serial ports detected.");
            }

            connected = true;
        }

        void CloseSerialPort()
        {
            if (connected) {
                if (serialPort != null) {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    serialPort = null;
                }
                connected = false;
                mainForm.Started = false;
            }
        }

        private void mainForm_StateChanged(object sender, EventArgs e)
        {
            if (mainForm.Started) {
                LoadSounds();
                OpenSerialPort();
            }
            else {
                CloseSerialPort();
                UnloadSounds();
            }
        }

        void mainForm_HandleDestroyed(object sender, EventArgs e)
        {
            this.Exit();
        }

        void gameWindowForm_Shown(object sender, EventArgs e)
        {
            ((SysWinForms.Form)sender).Hide();
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            mainForm.ErrorMessage("Error: " + e.EventType.ToString());
            CloseSerialPort();
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
 	        string buffer = serialPort.ReadExisting();
            foreach (char inputChar in buffer.ToCharArray())
            {
                int note;
                if (inputChar == '~') {
                    // begin monitoring for real data
                    heartbeatDetected = true;
                    mainForm.DataReceived();
                }
                else if (inputChar == '@') {
                    // debug input being printed, do not interpret as data
                    heartbeatDetected = false;
                }
                else if (heartbeatDetected && inputChar >= 'A' && inputChar <= 'H') {
                    note = inputChar - 'A';
                    UpdateSound(note, true);
                }
                else if (heartbeatDetected && inputChar >= 'a' && inputChar <= 'h') {
                    note = inputChar - 'a';
                    UpdateSound(note, false);
                }
            }
        } 

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        void LoadSounds()
        {
            sounds = mainForm.Sounds;
            noteSoundEffects.Clear();
            noteSoundEffectInstances.Clear();
            for (int i = 0; i < numberOfNotes; ++i) {
                if (sounds[i] != null) {
                    SoundEffect effect;
                    using (Stream stm = new FileStream(sounds[i].soundFile.fileName, FileMode.Open)) {
                        effect = SoundEffect.FromStream(stm);
                    }
                    this.noteSoundEffects.Add(effect);
                    this.noteSoundEffectInstances.Add(CreateSoundEffectInstance(i));
                }
                else {
                    this.noteSoundEffects.Add(null);
                    this.noteSoundEffectInstances.Add(null);
                }
            }

            // Start all the continuous looping notes so they are syncronized.
            for (int i = 0; i < numberOfNotes; ++i) {
                if (sounds[i] != null && sounds[i].playMode == PlayMode.ContinuousLoop) {
                    noteSoundEffectInstances[i].Volume = 0;
                    noteSoundEffectInstances[i].IsLooped = true;
                    noteSoundEffectInstances[i].Play();
                }
                if (sounds[i] != null && sounds[i].playMode == PlayMode.Looping) {
                    noteSoundEffectInstances[i].IsLooped = true;
                }
            }
        }

        SoundEffectInstance CreateSoundEffectInstance(int note)
        {
            SoundEffectInstance noteInstance = noteSoundEffects[note].CreateInstance();
            noteInstance.Pitch = sounds[note].pitchShiftInSemitones * (1.0F / 12.0F);
            noteInstance.Volume = sounds[note].volume;
            return noteInstance;
        }

        void UnloadSounds()
        {
            for (int i = 0; i < noteSoundEffects.Count; ++i) {
                if (noteSoundEffectInstances[i] != null) {
                    noteSoundEffectInstances[i].Stop();
                    noteSoundEffectInstances[i].Dispose();
                }
                if (noteSoundEffects[i] != null) {
                    noteSoundEffects[i].Dispose();
                }
            }

            noteSoundEffects.Clear();
            noteSoundEffectInstances.Clear();
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState newKeyboardState = Keyboard.GetState();
            for (Keys key = Keys.NumPad1; key <= Keys.NumPad8; key++)
            {
                if (newKeyboardState.IsKeyDown(key)  && !oldKeyboardState.IsKeyDown(key))
                    UpdateSound(key - Keys.NumPad1, true);
                else if (!newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyDown(key))
                    UpdateSound(key - Keys.NumPad1, false);
            }
            for (Keys key = Keys.D1; key <= Keys.D8; key++) {
                if (newKeyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key))
                    UpdateSound(key - Keys.D1, true);
                else if (!newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyDown(key))
                    UpdateSound(key - Keys.D1, false);
            }
            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        private void UpdateSound(int note, bool on)
        {
            if (connected && note >= 0 && note < noteSoundEffects.Count && this.noteSoundEffects[note] != null) {
                PlayMode mode = sounds[note].playMode;
                SoundEffect soundEffect = this.noteSoundEffects[note];
                SoundEffectInstance soundEffectInstance = noteSoundEffectInstances[note];
                switch (mode) {
                    case PlayMode.Once:
                        if (on) {
                            // Create a new sound effect instance each time, so that multiple can play at once.
                            soundEffectInstance = CreateSoundEffectInstance(note);
                            soundEffectInstance.Play();
                        }
                        break;

                    case PlayMode.OnOff:
                        if (on)
                            soundEffectInstance.Play();
                        else
                            soundEffectInstance.Stop();
                        break;

                    case PlayMode.Looping:
                        if (on)
                            soundEffectInstance.Play();
                        else
                            soundEffectInstance.Stop();
                        break;

                    case PlayMode.ContinuousLoop:
                        if (on)
                            soundEffectInstance.Volume = sounds[note].volume;
                        else
                            soundEffectInstance.Volume = 0;
                        break;

                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this.connected ? Color.Black : Color.Red);
            base.Draw(gameTime);
        }
    }
}
