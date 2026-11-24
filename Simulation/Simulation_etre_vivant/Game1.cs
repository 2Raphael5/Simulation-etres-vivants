using System;
using System.Collections.Generic;
using System.Linq;
using Class;
using Class.Entite;
using Class.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation_etre_vivant.Class.Entite;
using Simulation_etre_vivant.Class.Input;

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

        /// <summary>
        /// Distance (en cases) à partir de laquelle un herbivore détecte un carnivore et fuit
        /// </summary>
        private const int FleeRadius = 6;

        Dictionary<Point, LivingBeing> grid = new Dictionary<Point, LivingBeing>();

        private readonly Random _rnd = new Random();

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
            normalPlant = new Plant("Plant", 500, normalPlantPixel, 1000f);

            PixelGeneric normalHerbivorePixel = new PixelGeneric(new Vector2(0, 0), Color.White);
            normalHerbivore = new Herbivore("Lapin", 500, normalHerbivorePixel, 20, 100f, 0, 2);

            PixelGeneric normalCarnivorePixel = new PixelGeneric(new Vector2(0, 0), Color.Orange);
            normalCarnivore = new Carnivore("Renard", 750, normalCarnivorePixel, 10, 100f, 0, 1);

            allSeletableBeing = new List<LivingBeing>
            {
                normalPlant,
                normalHerbivore,
                normalCarnivore
            };

            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Initialise les instances des objets
        /// </summary>
        protected override void Initialize()
        {
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
        }

        #region Update

        /// <summary>
        /// Choisit une méthode à update en permanence en fonction de l'état du jeu
        /// </summary>
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
                Exit();
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
        /// Boucle principale de la simulation
        /// </summary>
        private void UpdatePlaying(float deltatime)
        {
            // Copies faites au début de la frame : on peut modifier allLivingBeing pendant la boucle
            var plants = allLivingBeing.OfType<Plant>().ToList();
            var herbivores = allLivingBeing.OfType<Herbivore>().ToList();
            var carnivores = allLivingBeing.OfType<Carnivore>().ToList();

            UpdatePlants(plants, deltatime);

            // Herbivores : cherchent les plantes et fuient les carnivores
            foreach (Herbivore herbivore in herbivores)
            {
                UpdateAnimal(herbivore, normalHerbivore, b => b is Plant, deltatime, b => b is Carnivore);
            }

            // Carnivores : chassent les herbivores
            foreach (Carnivore carnivore in carnivores)
            {
                UpdateAnimal(carnivore, normalCarnivore, b => b is Herbivore, deltatime);
            }

            HandleMousePlacement();

            foreach (LivingBeing being in beingToExist)
            {
                allLivingBeing.Add(being);
            }
            beingToExist.Clear();

            foreach (LivingBeing being in allLivingBeing)
            {
                if (being is Animal animal)
                {
                    animal.Update();
                }
            }

            foreach (LivingBeing dead in allLivingBeing.Where(b => b.Life <= 0).ToList())
            {
                RemoveBeing(dead);
            }

            if (_input.IsJustPressed(InputAction.MoveLeft))
            {
                indexBeing--;
                if (indexBeing < 0)
                {
                    indexBeing = allSeletableBeing.Count -1;
                }
            }
            else if (_input.IsJustPressed(InputAction.MoveRight))
            {
                indexBeing++;
                if (indexBeing >= allSeletableBeing.Count)
                {
                    indexBeing = 0;
                }
            }
        }

        /// <summary>
        /// Les plantes se propagent sur une case voisine libre
        /// </summary>
        private void UpdatePlants(List<Plant> plants, float deltatime)
        {
            foreach (Plant plant in plants)
            {
                plant.Timer -= deltatime;
                if (plant.Timer > 0f) continue;
                plant.Timer = plant.TimeBeforeAction;

                Point from = ToCell(plant.Pixel.Position);
                Point newCell = new Point(from.X + _rnd.Next(-1, 2), from.Y + _rnd.Next(-1, 2));

                if (newCell != from && InBounds(newCell) && IsFree(newCell))
                {
                    Vector2 newSnapped = CellToPosition(newCell);
                    LivingBeing child = plant.Clone(newSnapped);
                    grid[newCell] = child;
                    beingToExist.Add(child);
                }
            }
        }

        /// <summary>
        /// Comportement générique d'un animal : fuit les menaces proches, sinon cherche la proie
        /// la plus proche, avance d'une case vers elle, la mange, ou se reproduit / contourne si bloqué.
        /// </summary>
        /// <param name="animal">L'animal à mettre à jour</param>
        /// <param name="template">Modèle servant à créer les petits (stats de départ)</param>
        /// <param name="isPrey">Définit ce que l'animal mange</param>
        /// <param name="isThreat">Définit ce que l'animal fuit (null = ne fuit rien)</param>
        private void UpdateAnimal(Animal animal, Animal template, Func<LivingBeing, bool> isPrey, float deltatime, Func<LivingBeing, bool> isThreat = null)
        {
            if (animal.Life <= 0)
            {
                return;
            }

            animal.Timer -= deltatime;
            if (animal.Timer > 0f)
            {
                return;
            }
            animal.Timer = animal.TimeBeforeAction;

            Point from = ToCell(animal.Pixel.Position);

            if (isThreat != null)
            {
                LivingBeing threat = FindNearest(animal, isThreat);
                if (threat != null && Distance(from, ToCell(threat.Pixel.Position)) <= FleeRadius)
                {
                    if (TryFleeStep(from, ToCell(threat.Pixel.Position), out Point fleeCell))
                    {
                        MoveAnimal(animal, from, fleeCell);
                        return;
                    }
                }
            }

            LivingBeing prey = FindNearest(animal, isPrey);

            Point next;
            if (prey == null)
            {
                next = new Point(from.X + _rnd.Next(-1, 2), from.Y + _rnd.Next(-1, 2));
            }
            else
            {
                Point target = ToCell(prey.Pixel.Position);
                next = new Point(from.X + Math.Sign(target.X - from.X),
                                 from.Y + Math.Sign(target.Y - from.Y));
            }

            if (next == from || !InBounds(next))
            {
                return;
            }

            if (grid.TryGetValue(next, out LivingBeing occupant) && occupant != null)
            {
                if (isPrey(occupant))
                {
                    animal.Life += occupant.Life;
                    RemoveBeing(occupant);
                    MoveAnimal(animal, from, next);
                }
                else if (occupant.GetType() == animal.GetType() && animal.NbChild < animal.NbMaxChild)
                {
                    if (TryFindFreeNeighbor(from, out Point childCell))
                    {
                        animal.NbChild++;
                        LivingBeing child = template.Clone(CellToPosition(childCell));
                        grid[childCell] = child;
                        beingToExist.Add(child);
                    }
                }
                else if (TryFindFreeNeighbor(from, out Point sidestep))
                {
                    MoveAnimal(animal, from, sidestep);
                }
            }
            else
            {
                MoveAnimal(animal, from, next);
            }
        }

        /// <summary>
        /// Choisit la case voisine libre qui éloigne le plus de la menace.
        /// Retourne false si aucune case ne permet de s'éloigner (animal acculé).
        /// </summary>
        private bool TryFleeStep(Point from, Point threat, out Point result)
        {
            int bestScore = DistanceSquared(from, threat);
            var best = new List<Point>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    Point p = new Point(from.X + dx, from.Y + dy);
                    if (!InBounds(p) || !IsFree(p))
                    {
                        continue;
                    }

                    int score = DistanceSquared(p, threat);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best.Clear();
                        best.Add(p);
                    }
                    else if (score == bestScore && best.Count > 0)
                    {
                        best.Add(p);
                    }
                }
            }

            if (best.Count == 0)
            {
                result = from;
                return false;
            }
            result = best[_rnd.Next(best.Count)];
            return true;
        }

        /// <summary>
        /// Gère le placement d'un être à la souris
        /// </summary>
        private void HandleMousePlacement()
        {
            bool isDown = _mouseInputManager.LeftMouseDown();

            Vector2 mousePos = _mouseInputManager.GetMousePosition();
            Point cell = ToCell(mousePos);
            Vector2 snapped = CellToPosition(cell);

            allSeletableBeing[indexBeing].Pixel.Position = snapped;

            if (_wasMouseDown && !isDown && IsFree(cell))
            {
                LivingBeing newBeing = allSeletableBeing[indexBeing].Clone(snapped);
                grid[cell] = newBeing;
                beingToExist.Add(newBeing);
            }
            _wasMouseDown = isDown;
        }

        #endregion

        #region Helpers

        private Point ToCell(Vector2 pos)
        {
            return new Point((int)pos.X / CellSize, (int)pos.Y / CellSize);
        }

        private Vector2 CellToPosition(Point cell)
        {
            return new Vector2(cell.X * CellSize, cell.Y * CellSize);
        }

        private bool InBounds(Point cell)
        {
            return cell.X >= 0 && cell.Y >= 0 && cell.X * CellSize < Window.ClientBounds.Width && cell.Y * CellSize < Window.ClientBounds.Height;
        }

        private bool IsFree(Point cell)
        {
            return !grid.TryGetValue(cell, out LivingBeing b) || b == null;
        }

        /// <summary>
        /// Distance en cases (diagonales comprises)
        /// </summary>
        private int Distance(Point a, Point b)
        {
            return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        private int DistanceSquared(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        /// <summary>
        /// Trouve l'être le plus proche (distance en cases) qui correspond au filtre
        /// </summary>
        private LivingBeing FindNearest(Animal hunter, Func<LivingBeing, bool> filter)
        {
            Point hc = ToCell(hunter.Pixel.Position);
            LivingBeing best = null;
            int bestDist = int.MaxValue;

            foreach (LivingBeing being in allLivingBeing)
            {
                if (being == hunter || being.Life <= 0 || !filter(being)) continue;

                int dist = Distance(ToCell(being.Pixel.Position), hc);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = being;
                }
            }
            return best;
        }

        /// <summary>
        /// Cherche une case voisine libre (ordre aléatoire)
        /// </summary>
        private bool TryFindFreeNeighbor(Point origin, out Point result)
        {
            var candidates = new List<Point>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }
                    Point p = new Point(origin.X + dx, origin.Y + dy);
                    if (InBounds(p) && IsFree(p))
                    {
                        candidates.Add(p);
                    }
                }
            }

            if (candidates.Count == 0)
            {
                result = origin;
                return false;
            }
            result = candidates[_rnd.Next(candidates.Count)];
            return true;
        }

        /// <summary>
        /// Déplace un animal en mettant à jour la grille et sa position (sans clone)
        /// </summary>
        private void MoveAnimal(Animal animal, Point from, Point to)
        {
            grid.Remove(from);
            grid[to] = animal;
            animal.Pixel.Position = CellToPosition(to);
        }

        /// <summary>
        /// Retire définitivement un être de la simulation
        /// </summary>
        private void RemoveBeing(LivingBeing being)
        {
            being.Life = 0;
            Point cell = ToCell(being.Pixel.Position);
            if (grid.TryGetValue(cell, out LivingBeing inCell) && inCell == being)
            {
                grid.Remove(cell);
            }
            allLivingBeing.Remove(being);
            beingToExist.Remove(being);
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