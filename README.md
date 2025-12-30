# 🏋️ MicroMeter Pro

**MicroMeter Pro** to zaawansowana aplikacja desktopowa (Windows Forms) do obliczania zapotrzebowania kalorycznego i makroskładników odżywczych. Idealne narzędzie dla osób dbających o dietę, sportowców i trenerów personalnych.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![C#](https://img.shields.io/badge/C%23-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Spis treści

- [Funkcje](#-funkcje)
- [Zrzuty ekranu](#-zrzuty-ekranu)
- [Wymagania](#-wymagania)
- [Instalacja](#-instalacja)
- [Jak używać](#-jak-używać)
- [Wzory i obliczenia](#-wzory-i-obliczenia)
- [Technologie](#-technologie)
- [Struktura projektu](#-struktura-projektu)
- [Roadmap](#-roadmap)
- [Licencja](#-licencja)

---

## ✨ Funkcje

### 📊 Obliczenia metaboliczne
- **BMI (Body Mass Index)** - wskaźnik masy ciała z kolorową interpretacją
- **PPM (BMR)** - podstawowa przemiana materii z wyborem wzoru:
  - Mifflin–St Jeor *(domyślny, najbardziej aktualny)*
  - Harris–Benedict *(klasyczny)*
  - Katch–McArdle *(wymaga % tkanki tłuszczowej)*
- **CPM (TDEE)** - całkowita przemiana materii z uwzględnieniem aktywności fizycznej

### 🎯 Personalizacja celów
- **Utrzymanie masy** - bez korekty kalorycznej
- **Redukcja** - deficyt 300 kcal
- **Przyrost masy** - nadwyżka 300 kcal
- **Budowa masy mięśniowej** - nadwyżka 500 kcal

### 🥗 Makroskładniki
- Automatyczne obliczanie podziału makroskładników
- Konfigurowalne proporcje: **Białko**, **Tłuszcze**, **Węglowodany**
- Wyświetlanie w gramach i procentach
- Automatyczne przeliczanie węglowodanów (100% - białko - tłuszcze)

### 🎨 Interfejs użytkownika
- Intuicyjny, nowoczesny interfejs (Guna UI2)
- Kolorowa interpretacja wyników
- Wizualizacja sylwetki (męska/żeńska)
- Walidacja danych wejściowych
- Ostrzeżenia o ekstremalnych wartościach

---

## 🖼️ Zrzuty ekranu

*Dodaj tutaj screenshoty aplikacji*

```
[Screenshot 1: Główny widok aplikacji]
[Screenshot 2: Wyniki obliczeń]
[Screenshot 3: Makroskładniki]
```

---

## 💻 Wymagania

### Wymagania systemowe
- **System operacyjny:** Windows 7 / 8 / 10 / 11
- **.NET Framework:** 4.7.2 lub nowszy
- **RAM:** minimum 512 MB
- **Miejsce na dysku:** ~5 MB

### Wymagania developerskie
- **Visual Studio:** 2019 lub nowszy
- **Guna.UI2.WinForms:** NuGet package
- **.NET Framework SDK:** 4.7.2+

---

## 🚀 Instalacja

### Dla użytkowników

1. Pobierz najnowszą wersję z [Releases](https://github.com/twoj-username/MicroMeter-Pro/releases)
2. Rozpakuj archiwum ZIP
3. Uruchom `MicroMeterPro.exe`
4. Jeśli potrzebne, zainstaluj .NET Framework 4.7.2

### Dla deweloperów

```bash
# Sklonuj repozytorium
git clone https://github.com/twoj-username/MicroMeter-Pro.git

# Przejdź do folderu projektu
cd MicroMeter-Pro

# Otwórz solution w Visual Studio
start MicroMeter_Pro.sln

# Zainstaluj zależności NuGet
# (Visual Studio zrobi to automatycznie)

# Zbuduj projekt (Ctrl + Shift + B)
# Uruchom (F5)
```

---

## 📖 Jak używać

### Krok 1: Wprowadź dane podstawowe
1. **Waga** - w kilogramach (np. 75)
2. **Wzrost** - w centymetrach (np. 180)
3. **Wiek** - w latach (np. 25)
4. **Płeć** - wybierz mężczyzna/kobieta

### Krok 2: Wybierz parametry
1. **Wzór PPM** - wybierz preferowany wzór metaboliczny
   - Dla Katch-McArdle podaj % tkanki tłuszczowej
2. **Aktywność fizyczna (PAL)**:
   - 1.2 - brak aktywności (praca siedząca)
   - 1.4 - lekka aktywność (1-2x trening/tydzień)
   - 1.6 - umiarkowana (3-4x trening, fizyczna praca)
   - 1.8 - duża (5-6x trening/tydzień)
   - 2.0+ - bardzo wysoka (zawodowi sportowcy)

### Krok 3: Określ cel
- **Utrzymanie masy** - dla rekompozykcji ciała
- **Redukcja** - dla utraty wagi (-300 kcal)
- **Przytycie** - dla zyskania masy (+300 kcal)
- **Masa mięśniowa** - dla budowy mięśni (+500 kcal)

### Krok 4: Dostosuj makroskładniki
- Ustaw % białka (domyślnie 25%)
- Ustaw % tłuszczów (domyślnie 26%)
- Węglowodany obliczą się automatycznie (49%)

### Krok 5: Odczytaj wyniki
- **BMI** - z interpretacją stanu masy ciała
- **PPM** - Twoje podstawowe spalanie
- **CPM** - całkowite dzienne zapotrzebowanie
- **Makroskładniki** - w gramach i procentach

---

## 🧮 Wzory i obliczenia

### BMI (Body Mass Index)
```
BMI = waga [kg] / (wzrost [m])²
```

**Interpretacja:**
- < 18.5 - Niedowaga
- 18.5 - 24.9 - Norma
- 25 - 29.9 - Nadwaga
- 30 - 34.9 - Otyłość I stopnia
- 35 - 39.9 - Otyłość II stopnia
- ≥ 40 - Otyłość III stopnia

### PPM (Podstawowa Przemiana Materii)

**Mifflin–St Jeor** *(rekomendowany)*
```
Mężczyźni: PPM = 10×W + 6.25×H - 5×A + 5
Kobiety:    PPM = 10×W + 6.25×H - 5×A - 161
```

**Harris–Benedict** *(klasyczny)*
```
Mężczyźni: PPM = 88.362 + 13.397×W + 4.799×H - 5.677×A
Kobiety:    PPM = 447.593 + 9.247×W + 3.098×H - 4.330×A
```

**Katch–McArdle** *(wymaga % tkanki tłuszczowej)*
```
PPM = 370 + 21.6 × FFM
gdzie FFM = W × (1 - BF/100)
```

*W - waga [kg], H - wzrost [cm], A - wiek [lat], BF - % tkanki tłuszczowej, FFM - beztłuszczowa masa ciała*

### CPM (Całkowita Przemiana Materii)
```
CPM = PPM × PAL + Korekta
```

**Korekty:**
- Utrzymanie: 0 kcal
- Redukcja: -300 kcal
- Przyrost: +300 kcal
- Masa mięśniowa: +500 kcal

### Makroskładniki

**Wartości kaloryczne:**
- Białko: 4 kcal/g
- Tłuszcze: 9 kcal/g
- Węglowodany: 4 kcal/g

**Obliczenia:**
```
Kalorie_makro = CPM × (procent / 100)
Gramy = Kalorie_makro / wartość_kaloryczna
```

---

## 🛠️ Technologie

- **Język:** C# 8.0
- **Framework:** .NET Framework 4.7.2
- **UI Library:** Guna.UI2.WinForms
- **IDE:** Visual Studio 2019+
- **Architektura:** Windows Forms

### Struktura kodu
- Walidacja danych wejściowych w czasie rzeczywistym
- System blokad przed cyklicznymi wywołaniami
- Obsługa wyjątków i błędów
- Kulturowa obsługa separatorów dziesiętnych
- Event-driven architecture

---

## 📁 Struktura projektu

```
MicroMeter-Pro/
├── Form1.cs              # Główna logika aplikacji
├── Form1.Designer.cs     # Wygenerowany kod UI
├── Form1.resx            # Zasoby formularza
├── Program.cs            # Entry point aplikacji
├── Properties/
│   └── AssemblyInfo.cs   # Metadata projektu
├── Resources/            # Grafiki i zasoby
│   ├── male_silhouette.png
│   └── female_silhouette.png
└── README.md             # Ten plik
```

---

## 🗺️ Roadmap

### Planowane funkcje (v2.0)
- [ ] Export wyników do PDF/Excel
- [ ] Historia pomiarów z wykresami
- [ ] Profile użytkowników
- [ ] Kalkulator procentu tkanki tłuszczowej
- [ ] Baza produktów spożywczych
- [ ] Planer posiłków
- [ ] Tryb ciemny (Dark Mode)
- [ ] Wsparcie dla wielu języków
- [ ] Integracja z API fitness trackerów

### Ulepszenia techniczne
- [ ] Migracja do .NET 6/8
- [ ] Unit testy
- [ ] Lokalna baza danych (SQLite)
- [ ] Auto-update system
- [ ] Logging system

---

## 🤝 Współpraca

Chętnie przyjmę pull requesty! Jeśli chcesz pomóc w rozwoju:

1. Fork projektu
2. Stwórz branch z funkcjonalnością (`git checkout -b feature/NowaFunkcja`)
3. Commit zmian (`git commit -m 'Dodano nową funkcję'`)
4. Push do brancha (`git push origin feature/NowaFunkcja`)
5. Otwórz Pull Request

### Zgłaszanie błędów
Używaj [Issues](https://github.com/twoj-username/MicroMeter-Pro/issues) do zgłaszania bugów i propozycji funkcji.

---

## 📄 Licencja

Ten projekt jest licencjonowany na warunkach licencji MIT - zobacz plik [LICENSE](LICENSE) po szczegóły.

```
MIT License

Copyright (c) 2024 [Twoje Imię]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files...
```

---

## 👨‍💻 Autor

**[Twoje Imię]**
- GitHub: [@twoj-username](https://github.com/twoj-username)
- Email: twoj.email@example.com

---

## 🙏 Podziękowania

- Guna.UI2 - za świetną bibliotekę UI
- Społeczność fitness - za feedback i sugestie
- Wszystkim kontrybutorowi

---

## 📚 Bibliografia

1. Mifflin MD, St Jeor ST, et al. (1990). "A new predictive equation for resting energy expenditure in healthy individuals"
2. Harris JA, Benedict FG (1918). "A Biometric Study of Human Basal Metabolism"
3. Katch FI, McArdle WD (1973). "Prediction of body density from simple anthropometric measurements"

---

<div align="center">

**Zbudowano z ❤️ przy użyciu C# i Guna.UI2**

⭐ Jeśli projekt Ci się podoba, zostaw gwiazdkę!

[Zgłoś błąd](https://github.com/twoj-username/MicroMeter-Pro/issues) · [Poproś o funkcję](https://github.com/twoj-username/MicroMeter-Pro/issues) · [Dokumentacja](https://github.com/twoj-username/MicroMeter-Pro/wiki)

</div>