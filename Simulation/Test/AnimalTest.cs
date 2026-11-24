using Class;
using Class.Entite;
using Microsoft.Xna.Framework;
namespace Test
{
    [TestClass]
    public sealed class AnimalTest
    {
        [TestMethod]
        public void Clone_PixelPosition_NewAnimalAddedToList()
        {
            //ARRANGE
            PixelGeneric pixel = new PixelGeneric(new Vector2(0,0), Color.Blue);

            List<LivingBeing> animals = new List<LivingBeing>
            {
                new Animal("animal", 10, pixel, 10, 10, 0, 2)
            };
            //ACT
            animals.Add(animals[0].Clone(animals[0].Pixel.Position));

            //ASSERT
            Assert.AreEqual(2, animals.Count);
        }

        [TestMethod]
        public void Update_LifeIsReducedBy1()
        {
            //ARRANGE
            Animal animal = new Animal("test", 10, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10, 10, 0, 2);
            
            //ACT
            animal.Update();

            //ASSERT
            Assert.AreEqual(9, animal.Life);
        }
    }
}
