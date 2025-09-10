using CarVehicleExample;

namespace CarVehicleExampleUnitTests;

[TestClass]
public class VehicleTests
{
    [TestMethod]
    public void Vehicle_WithEngine_IsDriveable()
    {
        var vehicle = new Vehicle(2, true);
        Assert.IsTrue(vehicle.IsDriveable);
    }

    [TestMethod]
    public void Vehicle_WithoutEngine_IsNotDriveable()
    {
        var vehicle = new Vehicle(2, false);
        Assert.IsFalse(vehicle.IsDriveable);
    }

    [TestMethod]
    public void Car_IsDriveable()
    {
        var car = new Car();
        Assert.IsTrue(car.IsDriveable);
    }

    [TestMethod]
    public void Car_HasEngine_And_FourWheels()
    {
        var car = new Car();
        Assert.IsTrue(car.HasEngine);
        Assert.AreEqual(4, car.NumberOfWheels);
    }
}