using Class;
using Class.DebugTools;
using Class.Entite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX;
using Simulation_etre_vivant.Class.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Vector2 = Microsoft.Xna.Framework.Vector2;
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

        List<LivingBeing> allLivingBeing;
        List<LivingBeing> beingToExist;

        private const int CellSize = 10;
        Dictionary<Point, LivingBeing> grid = new Dictionary<Point, LivingBeing>();

        PixelGeneric normalPlantPixel;
        Plant normalPlant;
        Animal testAnimal;

        List<LivingBeing> allSeletableBeing;
        int indexBeing;

        private bool _wasMouseDown;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            normalPlantPixel = new PixelGeneric(new Vector2(0, 0), Color.DarkGreen);
            normalPlant = new Plant("Plant", 10, normalPlantPixel, 1000f);

            PixelGeneric normalAnimalPixel = new PixelGeneric(new Vector2(0, 0), Color.White);
            testAnimal = new Animal("Test", 10, normalAnimalPixel, 10, 20, 250f);

            allSeletableBeing = new List<LivingBeing>
            {
                normalPlant,
                testAnimal
            };
        }

        /// <summary>
        /// Initialise les instances des objets
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _input = new InputManager();
            _mainMenu = new MainMenu();
            GameStateManager.ChangeState(GameState.Menu);
            PrimitiveRenderer.Initialize(GraphicsDevice);
            _mouseInputManager = new MouseInputManager();
            allLivingBeing = new List<LivingBeing>();
            beingToExist = new List<LivingBeing>();
        }

        /// <summary>
        /// Load le spritebatch et la font
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font/Font");
            // TODO: use this.Content to load your game content here
        }
        #region Update

        /// <summary>
        /// Choisi une méthode a update en permance en fonction de l'état du jeu
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            _input.Update(gameTime);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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

        /// <summary>
        /// Sélection du menu
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltatime"></param>
        private void UpdatePlaying(float deltatime)
        {
            var plants = allLivingBeing.Where(e => e is Plant).ToList();
            var animals = allLivingBeing.Where(e => e is Animal).ToList();
            foreach (Plant plant in plants)
            {
                plant.Timer -= deltatime;
                if (plant.Timer <= 0f)
                {
                    plant.Timer = plant.TimeBeforeAction;
                    Random rnd = new Random();

                    Point newCell = new Point((int)((plant.Pixel.Position.X + (10 * rnd.Next(-1, 2))) / CellSize), (int)((plant.Pixel.Position.Y + (10 * rnd.Next(-1, 2))) / CellSize));

                    if ((!grid.ContainsKey(newCell) || grid[newCell] == null) && newCell.X < this.Window.ClientBounds.Width && newCell.Y < this.Window.ClientBounds.Height)
                    {
                        Vector2 newSnapped = new Vector2(newCell.X * CellSize, newCell.Y * CellSize);
                        PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                        grid[newCell] = plant.Clone(newSnapped);
                        beingToExist.Add(grid[newCell]);
                    }
                }


            }
            foreach (Animal animal in animals)
            {
                Random rnd = new Random();
                if (animal.Pixel.Position == new Vector2(animal.Goal.X, animal.Goal.Y) || animal.Goal == new Point(0,0))
                {                    
                    Point goal = new Point((int)((rnd.NextFloat(0, this.Window.ClientBounds.Width)) / CellSize * 10), (int)((rnd.NextFloat(0, this.Window.ClientBounds.Height)) / CellSize * 10));
                    animal.DefineGoal(goal);
                    Debug.WriteLine(animal.Goal);
                }
                else
                {
                    Point newCell = new Point(0,0);
                    animal.Timer -= deltatime;

                    if (animal.Timer <= 0f)
                    {
                        animal.Timer = animal.TimeBeforeAction;
                        if (animal.Pixel.Position.X > animal.Goal.X)
                        {
                            newCell = new Point((int)animal.Pixel.Position.X - CellSize, (int)animal.Pixel.Position.Y);

                        }
                        else if (animal.Pixel.Position.X < animal.Goal.X)
                        {
                            newCell = new Point((int)animal.Pixel.Position.X + CellSize, (int)animal.Pixel.Position.Y);
                        }

                        Vector2 newSnapped = new Vector2(newCell.X, newCell.Y);
                        PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                        grid[newCell] = animal.Clone(newSnapped);
                        beingToExist.Add(grid[newCell]);
                        grid.Remove(new Point((int)animal.Pixel.Position.X, (int)animal.Pixel.Position.Y));
                        allLivingBeing.Remove(animal);
                    }
                }
            }

            this.Window.Title = this.Window.ClientBounds.Height.ToString();
            bool isDown = _mouseInputManager.LeftMouseDown();

            Vector2 mousePos = _mouseInputManager.GetMousePosition();
            Point cell = new Point((int)mousePos.X / CellSize, (int)mousePos.Y / CellSize);
            Vector2 snapped = new Vector2(cell.X * CellSize, cell.Y * CellSize);

            allSeletableBeing[indexBeing].Pixel.Position = snapped;

            if (_wasMouseDown && !isDown && !grid.ContainsKey(cell))
            {
                LivingBeing newBeing = allSeletableBeing[indexBeing].Clone(snapped);
                grid[cell] = newBeing;
                beingToExist.Add(newBeing);
            }
            _wasMouseDown = isDown;
            foreach (LivingBeing being in beingToExist)
            {
                allLivingBeing.Add(being);
            }
            beingToExist.Clear();          

            if (_input.IsJustPressed(InputAction.Pause))
            {
                indexBeing++;
                if (indexBeing >= allSeletableBeing.Count)
                {
                    indexBeing = 0;
                }
            }
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
            foreach (LivingBeing being in allLivingBeing)
            {
                being.Spawn(_spriteBatch);
            }

            if (_mouseInputManager.LeftMouseDown())
            {
                allSeletableBeing[indexBeing].Spawn(_spriteBatch);
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
    }
}
