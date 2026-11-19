using Class.Entite;
using Class;
using Microsoft.Xna.Framework;

namespace Test;

[TestClass]
public class LivingBeingTest
{
    [TestMethod]
    public void Clone_PixelPosition_NewLivingBeing()
    {
        //ARRANGE
        List<LivingBeing> livingBeings = new List<LivingBeing>
        {
            new LivingBeing("test", 10, new PixelGeneric(new Vector2(0,0), Color.Blue), 10)
        };

        //ACT
        livingBeings.Add(livingBeings[0].Clone(livingBeings[0].Pixel.Position));

        //ASSERT
        Assert.AreEqual(2, livingBeings.Count);
    }

    [TestMethod]
    public void Die_PixelIsNull()
    {
        //ARRANGE
        LivingBeing livingBeing = new LivingBeing("test", 10, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10);

        //ACT
        livingBeing.Die();

        //ASSERT
        Assert.IsNull(livingBeing.Pixel);
    }

    [TestMethod]
    public void Update_LifeIs10_PixelIsNull()
    {
        //ARRANGE
        LivingBeing livingBeing = new LivingBeing("test", 10, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10);
        //ACT
        livingBeing.Update(0f);

        //ASSERT
        Assert.IsNotNull(livingBeing.Pixel);
    }

    [TestMethod]
    public void Update_LifeIs0_PixelIsNull() 
    {
        //ARRANGE
        LivingBeing livingBeing = new LivingBeing("test", 0, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10);
        //ACT
        livingBeing.Update(0f);

        //ASSERT
        Assert.IsNull(livingBeing.Pixel);
    }
}
