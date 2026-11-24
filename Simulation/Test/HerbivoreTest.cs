using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
using Simulation_etre_vivant.Class.Entite;
namespace Test
{
    [TestClass]
    public sealed class HerbivoreTest
    {
        [TestMethod]
        public void Clone_PixelPosition_NewHerbivoreAddedToList()
        {
            //ARRANGE
            PixelGeneric pixel = new PixelGeneric(new Vector2(0,0), Color.Blue);

            List<LivingBeing> being = new List<LivingBeing>
            {
                new Herbivore("animal", 10, pixel, 2, 1, 0, 2)
            };
            //ACT
            being.Add(being[0].Clone(being[0].Pixel.Position));

            //ASSERT
            Assert.AreEqual(2, being.Count);
            Assert.IsTrue(being[being.Count -1 ] is Herbivore);
        }
    }
}
