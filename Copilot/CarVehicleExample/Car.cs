namespace CarVehicleExample;

public class Car : Vehicle
{
    public Car(int numberOfWheels = 4) : base(numberOfWheels, true)
    {
    }

    public void Drive()
    {
        if (!IsDriveable)
            throw new InvalidOperationException("Car is not driveable.");
        Console.WriteLine("The car is driving.");
    }
}