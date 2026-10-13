using Class;
using Class.DebugTools;
using Class.Entite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using Simulation_etre_vivant.Class.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
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
        List<LivinBeing> beingToExist;

        private const int CellSize = 10;
        Dictionary<Point, LivinBeing> grid = new Dictionary<Point, LivinBeing>();

        LivinBeing currentLivingBeing;
        PixelGeneric normalPlantPixel;
        Plant normalPlant;

        private bool _wasMouseDown;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);/*
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;*/
            Content.RootDirectory = "Content";
            IsMouseVisible = true; 
            normalPlantPixel = new PixelGeneric(new Vector2(0, 0), Color.DarkGreen); 
            normalPlant = new Plant("Plant", 10, normalPlantPixel, 0.01f);
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
            beingToExist = new List<LivinBeing>();
            currentLivingBeing = normalPlant;
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

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalNanoseconds;
            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    UpdateMainMenu();
                    break;
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
            }


            if (_input.IsJustPressed(InputAction.Exit))
            {
                GameStateManager.ChangeState(GameState.Menu);
            }
            base.Update(gameTime);
        }

        private void UpdateMainMenu()
        {
            if (_input.IsJustPressed(InputAction.Exit))
            {
                Application.Exit();
            }
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
            foreach (Plant plant in allLivingBeing)
            {
                plant.Timer -= deltatime;
                if (plant.Timer <= 0f)
                {
                    plant.Timer = plant.TimeToGrow;
                    Random rnd = new Random();

                    Point newCell = new Point((int)((plant.Pixel.Position.X + (10 * rnd.Next(-1, 2))) / CellSize), (int)((plant.Pixel.Position.Y + (10 * rnd.Next(-1, 2))) / CellSize));

                    if (!grid.ContainsKey(newCell) && newCell.X < this.Window.ClientBounds.Width && newCell.Y < this.Window.ClientBounds.Height)
                    {
                        Vector2 newSnapped = new Vector2(newCell.X * CellSize, newCell.Y * CellSize);
                        PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                        grid[newCell] = new Plant("Plant", 10, pixel, 1); ;
                        beingToExist.Add(grid[newCell]);
                    }
                }
            }
            bool isDown = _mouseInputManager.LeftMouseDown();

            Vector2 mousePos = _mouseInputManager.GetMousePosition();
            Point cell = new Point((int)mousePos.X / CellSize, (int)mousePos.Y / CellSize);
            Vector2 snapped = new Vector2(cell.X * CellSize, cell.Y * CellSize);

            currentLivingBeing.Pixel.Position = snapped;

            if (_wasMouseDown && !isDown && !grid.ContainsKey(cell))
            {
                LivinBeing newBeing = currentLivingBeing.Clone(snapped);
                grid[cell] = newBeing;
                beingToExist.Add(newBeing);
            }
            _wasMouseDown = isDown;
            foreach (LivinBeing being in beingToExist)
            {
                allLivingBeing.Add(being);
            }          
            beingToExist.Clear();
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
            foreach (LivinBeing being in allLivingBeing)
            {
                being.Spawn(_spriteBatch);
            }

            if (_mouseInputManager.LeftMouseDown())
            {
                currentLivingBeing.Spawn(_spriteBatch);
            }
        }

        #endregion
        private void ResetSimulation()
        {
            allLivingBeing.Clear();
            beingToExist.Clear();
            grid.Clear();
            GameStateManager.ChangeState(GameState.Playing);
        }
        private Vector2 SnapToGrid(Vector2 position)
        {
            int cellX = (int)(position.X / CellSize);
            int cellY = (int)(position.Y / CellSize);
            return new Vector2(cellX * CellSize, cellY * CellSize);
        }
    }
}
