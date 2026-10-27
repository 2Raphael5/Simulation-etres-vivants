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
using System.Linq;
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
        Herbivore normalHerbivore;
        Carnivore normalCarnivore;

        List<LivingBeing> allSeletableBeing;
        int indexBeing;

        private bool _wasMouseDown;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            normalPlantPixel = new PixelGeneric(new Vector2(0, 0), Color.DarkGreen);
            normalPlant = new Plant("Plant", 500, normalPlantPixel, 10000f);

            PixelGeneric normalHerbivorePixel = new PixelGeneric(new Vector2(0, 0), Color.White);
            normalHerbivore = new Herbivore("Lapin", 500, normalHerbivorePixel, 10, 20, 100f, 2);


            PixelGeneric normalCarnivorePixel = new PixelGeneric(new Vector2(0, 0), Color.OrangeRed);
            normalCarnivore = new Carnivore("Renard", 500, normalCarnivorePixel, 10, 20, 100f, 2);

            allSeletableBeing = new List<LivingBeing>
            {
                normalPlant,
                normalHerbivore,
                normalCarnivore
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
            if (_input.IsDown(InputAction.Accelerate))
            {
                deltaTime = (float)gameTime.ElapsedGameTime.TotalMicroseconds;
            }
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
            var plants = allLivingBeing.OfType<Plant>().ToList();
            var animals = allLivingBeing.OfType<Animal>().ToList();
            var herbivores = animals.OfType<Herbivore>().ToList();
            var carnivores = animals.OfType<Carnivore>().ToList();

            if (plants.Count != 0)
            {
                foreach (Plant plant in plants)
                {
                    plant.Timer -= deltatime;
                    if (plant.Timer <= 0f)
                    {
                        plant.Timer = plant.TimeBeforeAction;
                        Random rnd = new Random();

                        Point newCell = new Point((int)((plant.Pixel.Position.X + (CellSize * rnd.Next(-1, 2))) / CellSize), (int)((plant.Pixel.Position.Y + (CellSize * rnd.Next(-1, 2))) / CellSize));

                        if ((!grid.ContainsKey(newCell) || grid[newCell] == null) && newCell.X < this.Window.ClientBounds.Width && newCell.Y < this.Window.ClientBounds.Height)
                        {
                            Vector2 newSnapped = new Vector2(newCell.X * CellSize, newCell.Y * CellSize);
                            PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                            grid[newCell] = plant.Clone(newSnapped);
                            beingToExist.Add(grid[newCell]);
                        }
                    }


                }

            }
            try
            {
                foreach (Animal animal in animals)
                {
                    Random rnd = new Random();
                    if (animal.Pixel.Position == new Vector2(animal.Goal.X, animal.Goal.Y) || animal.Goal == new Point(0, 0))
                    {
                        Point goal = new Point((int)((rnd.Next(0, this.Window.ClientBounds.Width)) / 10 * CellSize), (int)((rnd.Next(0, this.Window.ClientBounds.Height)) / 10 * CellSize));
                        if (!grid.ContainsKey(goal))
                        {
                            animal.DefineGoal(goal);
                        }                        
                    }
                    else
                    {
                        Point newCell = new Point(0, 0);
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

                            if (animal.Pixel.Position.Y > animal.Goal.Y)
                            {
                                newCell = new Point((int)animal.Pixel.Position.X, (int)animal.Pixel.Position.Y - CellSize);
                            }
                            else if (animal.Pixel.Position.Y < animal.Goal.Y)
                            {
                                newCell = new Point((int)animal.Pixel.Position.X, (int)animal.Pixel.Position.Y + CellSize);
                            }

                            if (!grid.ContainsKey(newCell / CellSize))
                            {
                                Vector2 newSnapped = new Vector2(newCell.X, newCell.Y);
                                PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                                grid[newCell] = animal.Clone(newSnapped);
                                beingToExist.Add(grid[newCell]);
                                grid.Remove(new Point((int)animal.Pixel.Position.X / CellSize, (int)animal.Pixel.Position.Y / CellSize));
                                allLivingBeing.Remove(animal);
                            }
                            else if ((grid[newCell / CellSize] is Plant && animal is Herbivore)|| (grid[newCell / CellSize] is Herbivore && animal is Carnivore))
                            {
                                animal.Life += grid[newCell / CellSize].Life;
                                allLivingBeing.Remove(grid[newCell / CellSize]);
                                grid.Remove(newCell / CellSize);
                            }
                            else if (animal.NbMaxChild > 0 && grid[newCell / CellSize].GetType() == animal.GetType())
                            {
                                animal.NbMaxChild--;
                                Vector2 newSnapped = new Vector2(newCell.X + 10, newCell.Y + 10);
                                PixelGeneric pixel = new PixelGeneric(newSnapped, normalPlantPixel.Color);
                                grid[newCell] = animal.Clone(newSnapped);
                                grid[newCell].Life = normalHerbivore.Life;
                                beingToExist.Add(grid[newCell]);
                            }
                            else
                            {
                                animal.Goal = new Point(0, 0);
                            }
                        }
                    }
                    if (animal.Life <= 0)
                    {
                        grid.Remove(new Point((int)animal.Pixel.Position.X, (int)animal.Pixel.Position.Y));
                        allLivingBeing.Remove(animal);
                        animals.Remove(animal);
                    }
                }
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }
            animals.RemoveAll(a => a.Life == 0);
            allLivingBeing.RemoveAll(a => a.Life == 0);
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
            foreach (LivingBeing being in allLivingBeing)
            {
                being.Update(deltatime);
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
