using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation_etre_vivant.Class.Entite
{
    public class Carnivore : Animal
    {
        public Carnivore(string name, float life, PixelGeneric pixel, float speed, float visionRange, float time, int nbMaxChild) : base(name, life, pixel, speed, visionRange, time, nbMaxChild)
        {
        }

        public override LivingBeing Clone(Vector2 position)
        {
            Carnivore herbivore = new Carnivore(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, VisionRange, TimeBeforeAction, NbMaxChild);
            herbivore.DefineGoal(Goal);
            herbivore.Timer = this.Timer;
            return herbivore;
        }
    }
}
