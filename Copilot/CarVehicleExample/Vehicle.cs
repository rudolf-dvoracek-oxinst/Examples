namespace CarVehicleExample;

public class Vehicle
{
    public int NumberOfWheels { get; }
    public bool HasEngine { get; }
    public virtual bool IsDriveable => HasEngine;

    public Vehicle(int numberOfWheels, bool hasEngine)
    {
        NumberOfWheels = numberOfWheels;
        HasEngine = hasEngine;
    }
}