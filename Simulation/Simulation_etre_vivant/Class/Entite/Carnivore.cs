using Class;
using Class.Entite;
using Microsoft.Xna.Framework;

namespace Simulation_etre_vivant.Class.Entite
{
    public class Carnivore : Animal
    {
        public Carnivore(string name, float life, PixelGeneric pixel, float speed, float time, int nbChild, int nbMaxChild) : base(name, life, pixel, speed, time, nbChild, nbMaxChild)
        {
        }

        public override LivingBeing Clone(Vector2 position)
        {
            Carnivore herbivore = new Carnivore(Name, Life, new PixelGeneric(position, Pixel.Color), Speed, TimeBeforeAction, 0, NbMaxChild);
            herbivore.Timer = this.Timer;
            return herbivore;
        }
    }
}
