using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
namespace Test
{
    [TestClass]
    public sealed class PlantTest
    {
        [TestMethod]
        public void Clone_PixelPosition_NewPlantAddedToList()
        {
            //ARRANGE
            PixelGeneric pixel = new PixelGeneric(new Vector2(0, 0), Color.Blue);

            List<LivingBeing> being = new List<LivingBeing>
            {
                new Plant("animal", 10, pixel, 2)
            };
            //ACT
            being.Add(being[0].Clone(being[0].Pixel.Position));

            //ASSERT
            Assert.AreEqual(2, being.Count);
            Assert.IsTrue(being[being.Count - 1] is Plant);
        }
    }
}
