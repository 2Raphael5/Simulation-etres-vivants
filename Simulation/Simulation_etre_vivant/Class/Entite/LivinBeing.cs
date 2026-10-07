using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Class.Entite
{
    public class LivinBeing
    {
        protected string Name;
        protected float Life;
        protected PixelGeneric Pixel;
        protected float _timer;

        public LivinBeing(string name, float life, PixelGeneric pixel)
        {
            Name = name;
            Life = life;
            Pixel = pixel;
        }

        public virtual void Update(float deltaTime)
        {
            if (Life<=0)
            {
                Die();
            }
        }
        public void Spawn(SpriteBatch _spriteBach)
        {
            Pixel.DrawRectangle(_spriteBach);
        }

        public void TakeDamage()
        {
            if (Life > 0)
            {
                Life -= 10;
            }
        }

        protected void Die() 
        { 
            Pixel = null;
        }
    }
}
