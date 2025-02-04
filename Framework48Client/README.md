# Solution Framework48Client

The solution consists of the following projects:

 - **CodeFirstService**: Web-based gRPC code-first service running on .NET 8, supporting HTTP/1 and HTTP/2.

 - **CodeFirstClient**: .NET 8 gRPC client using HTTP/2.

 - **Framework48Client**: gRPC client running on .NET Framework 4.8 using HTTP/1.

 - **CodeFirstContracts**: Shared .netstandard2.0 project containing contracts between client and server.

**To support both HTTP/1 and HTTP/2 simultaneously, the web server must use TLS security.**