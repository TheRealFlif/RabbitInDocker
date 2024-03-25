using Producer.Producers;

namespace Test.Producer;

[TestFixture]
internal class LatinWordCreatorTest
{
    [Test]
    public void CreateCombination_Four_ReturnsThreeSeparators()
    {
        //Arrange
        var sut = new LatinWordCreator();
        var separator = '_';

        //Act
        var actual = sut.CreateCombination(4, separator);

        //Assert
        Assert.That(actual.Count(c => c==separator), Is.EqualTo(3));
    }

    [Test]
    public void CreateCombination_One_ReturnsNoSeparators()
    {
        //Arrange
        var sut = new LatinWordCreator();
        var separator = '_';

        //Act
        var actual = sut.CreateCombination(1, separator);

        //Assert
        Assert.That(actual.Count(c => c == separator), Is.EqualTo(0));
    }
}
