# CarVehicleExample

This project demonstrates basic object-oriented programming concepts in C# using .NET 8 and C# 12.0. It features a simple class hierarchy for vehicles, focusing on inheritance, encapsulation, and unit testing.

**Purpose:**  
The purpose of this solution is to show how generative AI is generating the code, providing a practical example of AI-assisted software development.

## Features

- **Vehicle Base Class**: Represents a generic vehicle with properties for the number of wheels and whether it has an engine. The `IsDriveable` property indicates if the vehicle can be driven (i.e., it has an engine).
- **Car Class**: Inherits from `Vehicle`, always has an engine, and defaults to four wheels. Includes a `Drive` method that outputs to the console when the car is driving.
- **MSTest Unit Tests**: Comprehensive tests verify the behavior of both `Vehicle` and `Car` classes.

## Usage

1. **Instantiate a Car and Drive**
```csharp
	var car = new Car();
	car.Drive(); // Outputs: The car is driving.
```

2. **Vehicle Example**
```csharp
	var vehicle = new Vehicle(2, false);
	Console.WriteLine(vehicle.IsDriveable); // Outputs: False
```

## Running the Project

- Open the solution in Visual Studio 2022.
- Build and run the project to see the output in the console.

## Running Unit Tests

- Open the Test Explorer in Visual Studio.
- Run all tests to verify the functionality of the `Vehicle` and `Car` classes.

## Requirements

- .NET 8 SDK
- Visual Studio 2022 or later

## Project Structure

- `Vehicle.cs` - Base class for vehicles.
- `Car.cs` - Derived class representing a car.
- `Program.cs` - Example usage of the classes.
- `CarVehicleExample.Tests` - MSTest unit tests (if present).

---

## Prompts Used to Generate This Project

1. Please generate class car and vehicle with inheritance. It could hold information about number of wheels, engine and if it is driveable. Car is driveable because it has engine, vehicle not. Also generate mstest unit tests for those classes.
2. Put into the main program instantiation of car and call the drive method. Modify drive methot with the output to the console when car is driving
3. Generate README.MD at the top of the solution describing project
4. Please mention in the MD file all previous prompts used to generate this project

This project is intended for educational purposes to illustrate class inheritance and basic testing in C#.