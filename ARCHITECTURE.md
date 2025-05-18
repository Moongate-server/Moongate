# 🧱 Moongate – Architectural Decisions

| Component                | Technology / Choice                               | Motivation                                                          |
|--------------------------|---------------------------------------------------|---------------------------------------------------------------------|
| **Language**             | C# 12 / .NET 8                                    | Modern, high-performance, cross-platform                            |
| **Dependency Injection** | Microsoft.Extensions.DependencyInjection + DryIoc | Combines simplicity of built-in DI with advanced features of DryIoc |
| **AOT Compilation**      | Native AOT (.NET 8)                               | Fast startup, low memory usage, single-file deployment              |
| **Serialization**        | Custom Binary Serialization                       | Tailored for performance and control over data format               |
| **Networking**           | TCP with SocketAsyncEventArgs                     | High-performance, low-overhead networking                           |
| **Persistence**          | Custom Binary Format                              | Consistent with serialization approach, optimized for performance   |
| **Scripting Engine**     | Roslyn Scripting (C#)                             | Full .NET integration, developer familiarity                        |
| **Configuration**        | JSON (appsettings.json)                           | Human-readable, widely supported, default in ASP.NET Core           |
| **Logging**              | Serilog                                           | Structured logging with rich ecosystem                              |
| **Admin Tools**          | ASP.NET Core REST API                             | Modern, interactive web-based administration                        |
