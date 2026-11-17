using Class.Entite;
using Class;
using Microsoft.Xna.Framework;

namespace Test;

[TestClass]
public class LivingBeingTest
{
    [TestMethod]
    public void Clone_PixelGenericPosition_NewLivingBeing()
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
    public void Die_PixelGenericIsNull()
    {
        //ARRANGE
        LivingBeing livingBeing = new LivingBeing("test", 10, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10);
        //ACT
        livingBeing.Die();
        //ASSERT
        Assert.IsNull(livingBeing.Pixel);
    }

    [TestMethod]
    public void Die_PixelGenericIsNotNull() 
    {
        //ARRANGE
        LivingBeing livingBeing = new LivingBeing("test", 10, new PixelGeneric(new Vector2(0, 0), Color.Blue), 10);
        //ACT
        
        //ASSERT
    }
}
