# Avto Yuvish — Boshqaruv tizimi

This is a **working car wash management app**, not an IDEF0 diagram viewer. Built in C# Windows Forms for .NET 10.

## Run on Windows

1. Open this folder in VS Code.
2. Open **Terminal → New Terminal**.
3. Run:

```powershell
dotnet restore
dotnet run
```

The project requires the **.NET 10 SDK** and **.NET 10 Windows Desktop Runtime**. Check `dotnet --list-sdks` and `dotnet --list-runtimes`.

## Features

- A1 — Create an order (customer, phone, plate, model, service, automatically calculated price).
- A2 — Mark vehicle ready.
- A3 — Mark washing complete.
- A4 — Approve inspection, collect payment, hand over the car.
- Search and list orders, totals, finished and active jobs.
- Automatic local order storage as JSON under `%LOCALAPPDATA%\AvtoYuvishBoshqaruvApp\orders.json`.
- Export simple text receipt for completed orders.

Note: payments and receipts are **demo records**, not a real payment gateway or legally valid fiscal receipt.

This project has been authored but **not compiler-tested** in the creation environment, which has no .NET SDK.
