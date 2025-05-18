# 🧱 Moongate – Architectural Decisions

| Component                | Technology / Choice                               | Motivation                                                               |
|--------------------------|---------------------------------------------------|--------------------------------------------------------------------------|
| **Language**             | C# 13 / .NET 9                                    | Modern, high-performance, cross-platform                                 |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection + DryIoc | Combines simplicity of built-in DI with advanced features of DryIoc      |
| **AOT Compilation**      | Native AOT (.NET 9)                               | Fast startup, low memory usage, single-file deployment                   |
| **Serialization**        | Custom Binary Serialization                       | Tailored for performance and control over data format                    |
| **Networking**           | TCP with SocketAsyncEventArgs                     | High-performance, low-overhead networking                                |
| **Persistence**          | Custom Binary Format                              | Consistent with serialization approach, optimized for performance        |
| **Scripting Engine**     | LuaJIT                                            | Exceptional performance, lightweight footprint, widely adopted in gaming |
| **Configuration**        | JSON                                              | Human-readable, widely supported, default in ASP.NET Core                |
| **Logging**              | Serilog                                           | Structured logging with rich ecosystem                                   |
| **Admin Tools**          | EmbedIO (Self-Hosted REST API)                    | Lightweight, modular, suitable for AOT and console applications          |
| **Testing**              | NUnit4 + Moq                                      | Popular, well-supported testing framework with mocking capabilities      |
