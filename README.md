# 🚗 Avtomobil yuvish — Boshqaruv tizimi

Ushbu loyiha avtomobil yuvish shoxobchasi faoliyatini boshqarish uchun ishlab chiqilgan dastur hisoblanadi.

Dastur **C# Windows Forms** texnologiyasida, **.NET 10** platformasi asosida yaratilgan.

Bu loyiha IDEF0 diagrammalarini ko‘rsatish dasturi emas, balki avtomobil yuvish jarayonlarini boshqarishga mo‘ljallangan amaliy dasturdir.

## 💻 Dasturni ishga tushirish

Dastur Windows operatsion tizimida ishlash uchun mo‘ljallangan.

1. Loyiha papkasini **Visual Studio Code** dasturida oching.
2. **Terminal → New Terminal** bo‘limiga o‘ting.
3. Quyidagi buyruqlarni ketma-ket bajaring:

```powershell
dotnet restore
dotnet run
```

**Talab qilinadigan dasturiy ta’minot:**
- .NET 10 SDK
- .NET 10 Windows Desktop Runtime

O‘rnatilgan versiyalarni tekshirish uchun:

```powershell
dotnet --list-sdks
dotnet --list-runtimes
```

## ⚙️ Dasturning asosiy imkoniyatlari

### A1 — Buyurtma yaratish
- Mijozning ismi va telefon raqamini kiritish.
- Avtomobilning davlat raqami va modelini kiritish.
- Yuvish xizmatini tanlash.
- Xizmat narxini avtomatik hisoblash.

### A2 — Avtomobilni tayyor deb belgilash
- Avtomobilning tayyorlik holatini belgilash.
- Buyurtma holatini yangilash.

### A3 — Yuvish jarayonini yakunlash
- Avtomobil yuvish ishlari tugaganini qayd etish.
- Buyurtma holatini yangilash.

### A4 — Tekshirish, to‘lov va topshirish
- Yakuniy tekshiruvni tasdiqlash.
- To‘lovni qabul qilingan deb qayd etish.
- Avtomobilni mijozga topshirish.

## 📋 Buyurtmalarni boshqarish

Dastur quyidagi imkoniyatlarni taqdim etadi:

- Buyurtmalar ro‘yxatini ko‘rish.
- Buyurtmalarni qidirish.
- Faol va yakunlangan buyurtmalarni kuzatish.
- Umumiy ko‘rsatkichlarni ko‘rish.

## 💾 Ma’lumotlarni saqlash

Buyurtmalar kompyuterning mahalliy xotirasida **JSON** formatida avtomatik saqlanadi.

Ma’lumotlar joylashuvi:

```text
%LOCALAPPDATA%\AvtoYuvishBoshqaruvApp\orders.json
```

## 🧾 Chek yaratish

Yakunlangan buyurtmalar uchun oddiy matnli chekni eksport qilish imkoniyati mavjud.

**Eslatma:** Dasturdagi to‘lovlar va cheklar namoyish uchun mo‘ljallangan. Ular haqiqiy to‘lov tizimiga ulanmagan va qonuniy fiskal chek hisoblanmaydi.

## 🛠️ Texnologiyalar

- Dasturlash tili: C#
- Grafik interfeys: Windows Forms
- Platforma: .NET 10
- Ma’lumotlarni saqlash: JSON
- Operatsion tizim: Windows

## 📌 Loyiha holati

Loyihaning dastur kodi tayyorlangan, ammo dastur yaratilgan muhitda .NET SDK mavjud bo‘lmagani sababli kompilyatsiya orqali tekshirilmagan.
