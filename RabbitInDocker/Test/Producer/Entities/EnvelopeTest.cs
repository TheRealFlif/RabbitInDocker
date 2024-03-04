using NUnit.Framework.Internal;
using Producer.Entities;

namespace Test.Producer.Entities;

[TestFixture]
internal class EnvelopeTest
{
    [Test]
    public void To_AEnvelopeWithAString_ReturnsAString()
    {
        //Arrange
        var sut = new Envelope<string>("test");

        //Act
        var actual = sut.To();

        //Assert
        Assert.That(actual, Is.EqualTo("{\"MetaData\":{},\"Data\":\"test\"}"));
    }

    [Test]
    public void From_AString_ReturnsAnEnvelopeWithAString()
    {
        //Arrange
        var sut = Envelope<string>.From("{\"MetaData\":{},\"Data\":\"test\"}");

        //Act
        var actual = sut!.Data;

        //Assert
        Assert.That(actual, Is.EqualTo("test"));
    }

    [Test]
    public void To_EnvelopeOfEnvelope_ReturnsAString()
    {
        //Arrange
        var envelope = new Envelope<string>("test");
        var sut = new Envelope<Envelope<string>>(envelope);

        //Act
        var actual = sut.To();

        //Assert
        Assert.That(actual, Is.EqualTo("{\"MetaData\":{},\"Data\":{\"MetaData\":{},\"Data\":\"test\"}}"));
    }
}
