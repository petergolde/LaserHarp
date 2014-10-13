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

namespace LaserHarp
{
    public class LaserHarpGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SerialPort serialPort;
        bool connected = false;

        List<SoundEffect> noteSoundEffects = new List<SoundEffect>();
        KeyboardState oldKeyboardState = Keyboard.GetState();
        string[] notes = { "Sounds/piano-0g", 
                            "Sounds/piano-a",
                            "Sounds/piano-b",
                            "Sounds/piano-c1",
                            "Sounds/piano-d",
                            "Sounds/piano-e",
                            "Sounds/piano-f1",
                            "Sounds/piano-g2",
                             };

        public LaserHarpGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 200;
            graphics.PreferredBackBufferHeight = 100;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            string serialPortName = SerialPort.GetPortNames().FirstOrDefault();
            if (!string.IsNullOrEmpty(serialPortName))
            { 
                try
                {
                    serialPort = new SerialPort(serialPortName, 57600, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                    serialPort.DataReceived += serialPort_DataReceived;
                    serialPort.ErrorReceived += serialPort_ErrorReceived;
                    connected = true;
                }
                catch (Exception)
                {
                    
                }
            }

            base.Initialize();
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            this.connected = false;
        }

        

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
 	        string buffer = serialPort.ReadExisting();
            foreach (char inputChar in buffer.ToCharArray())
            {
                int note = inputChar - '0';
                // if we receive a number that is in the range of our sounds... play it
                if (note >= 0 && note < this.noteSoundEffects.Count)
                {
                    this.noteSoundEffects[note].Play();
                }
            }
        } 

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        
            foreach (var note in notes)
            {
                SoundEffect noteSoundEffect = Content.Load<SoundEffect>(note);
                this.noteSoundEffects.Add(noteSoundEffect);
            }
        }

        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState newKeyboardState = Keyboard.GetState();
            for (Keys key = Keys.NumPad0; key <= Keys.NumPad7; key++ )
            {
                if (newKeyboardState.IsKeyDown(key)  && !oldKeyboardState.IsKeyDown(key))
                    this.noteSoundEffects[key - Keys.NumPad0].Play();
            }
            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this.connected ? Color.Black : Color.Red);
            base.Draw(gameTime);
        }
    }
}
