using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation_etre_vivant.Class.Entite
{
    internal class Herbivore : Animal
    {
        public Point Goal;
        public Herbivore(string name, float life, PixelGeneric pixel, float speed, float visionRange, float time, int nbChild, int nbMaxChild) : base(name, life, pixel, speed, visionRange, time, nbChild, nbMaxChild)
        {
        }
        public void DefineGoal(Point goal)
        {
            Goal = goal;
        }
        public override LivingBeing Clone(Vector2 position)
        {
            Herbivore herbivore = new Herbivore(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, VisionRange, TimeBeforeAction, 0, NbMaxChild);
            herbivore.DefineGoal(Goal);
            herbivore.Timer = this.Timer;
            return herbivore;
        }
    }
}
