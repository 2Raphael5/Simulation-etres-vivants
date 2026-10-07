using Class;
using Class.DebugTools;
using Class.Entite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation_etre_vivant.Class.Entite;
using Simulation_etre_vivant.Class.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
public enum GameState
{
    Playing,
    Pause,
    Menu
}
namespace Simulation_etre_vivant
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private InputManager _input;

        private MainMenu _mainMenu;

        SpriteFont _font;

        MouseInputManager _mouseInputManager;
        MouseState currentMouseState;

        List<LivinBeing> allLivingBeing;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _input = new InputManager();
            _mainMenu = new MainMenu();
            GameStateManager.ChangeState(GameState.Menu);
            _font = Content.Load<SpriteFont>("Font/Font");          
            PrimitiveRenderer.Initialize(GraphicsDevice);
            _mouseInputManager = new MouseInputManager();
            allLivingBeing = new List<LivinBeing>();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }
        #region Update
        protected override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    UpdateMainMenu();
                    break;
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
            }


            base.Update(gameTime);
        }

        private void UpdateMainMenu()
        {
            _mainMenu.Update(_input);
            if (_mainMenu.IsConfirmed(_input))
            {
                if (_mainMenu.SelectedIndex == 0)
                {
                    ResetSimulation();
                }
                else
                {
                    Exit();
                }
            }
        }

        private void UpdatePlaying(float deltatime)
        {
        }

        #endregion
        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            _spriteBatch.Begin();
            currentMouseState = Mouse.GetState();
            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Playing:
                    DrawPlaying(_spriteBatch);
                    break;
            }
            _mouseInputManager.Update(gameTime, currentMouseState);
            base.Draw(gameTime);

            _spriteBatch.End();
        }
        private void DrawMenu()
        {
            Vector2 textSize = _font.MeasureString("Play");
            _mainMenu.Draw(_spriteBatch, _font, new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2f, (_graphics.PreferredBackBufferHeight - textSize.Y) / 2f));
        }
        private void DrawPlaying(SpriteBatch _spriteBatch)
        {
            if (_mouseInputManager.LeftMouseDown())
            {
                PixelGeneric pixel = new PixelGeneric(_mouseInputManager.GetMousePosition(), Color.DarkGreen);
                Plant plant = new Plant("Plant", 10, pixel, 2);
                allLivingBeing.Add(plant);
            }
            foreach (LivinBeing being in allLivingBeing)
            {

                being.Spawn(_spriteBatch);
            }
        }

        #endregion
        private void ResetSimulation()
        {
            GameStateManager.ChangeState(GameState.Playing);
        }
    }
}
