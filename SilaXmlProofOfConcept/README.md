# HelloWorld Solution (.NET 8)

This solution demonstrates a simple gRPC-based Hello World application using .NET 8. It consists of the following projects:

---

## 1. HelloWorldContracts

**Purpose:**  
Defines the gRPC service contract and message types shared between the server and client.

**Contents:**
- `Protos/hello.proto`:  
  The Protocol Buffers file describing the `HelloWorld` gRPC service, including the `SayHello` and `JustSayHello` RPC methods, and the message types `HelloRequest` and `HelloReply`.
- `Messages/HelloReply.cs`:  
  C# class representing the `HelloReply` message.

---

## 2. HelloWorldServer

**Purpose:**  
Implements the gRPC server hosting the Hello World service and an optional background worker.

**Contents:**
- `GrpcServices/HelloWorldService.cs`:  
  The gRPC service implementation, inheriting from the generated base class and providing logic for the `SayHello` method.
- `HelloWorldBackgroundService.cs`:  
  An example of a background service (using `BackgroundService`) that can run additional tasks alongside the gRPC server.
- `Program.cs`:  
  Configures and starts the gRPC server, registers the gRPC service, and sets up Kestrel for HTTP/2.

---

## 3. Net8Client

**Purpose:**  
A .NET 8 client application that connects to the gRPC server and calls the Hello World service.

**Contents:**
- `Program.cs`:  
  Contains the client logic to create a gRPC channel, send requests to the server, and display responses.

---

## How to Run

1. **Build the solution** using Visual Studio 2022 or the .NET CLI.
2. **Start the server** (`HelloWorldServer` project).
3. **Run the client** (`Net8Client` project) to send requests to the server.

---

## Requirements

- .NET 8 SDK
- Visual Studio 2022 (recommended)
- gRPC tools and dependencies (handled via NuGet)

---

## Notes

- The gRPC server is configured to use HTTP/2.
- The contracts project (`HelloWorldContracts`) should be referenced by both the server and client projects.

