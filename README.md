## Clean Architecture

Solution `Renmei.Backend.slnx` tách biệt hoàn toàn Backend với ứng dụng Mobile (.NET MAUI) để đảm bảo CI độc lập và nhanh gọn:

```text
Renmei.WebApi (Presentation: Controllers, Middleware, Program.cs)
     ├──► Renmei.Application: Use Cases, Interfaces, DTOs ──► Renmei.Domain: Entities, Pure Rules
     └──► Renmei.Infrastructure: EF Core DbContext, JWT, SystemClock ──┘