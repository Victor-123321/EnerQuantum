# EnerQuantum

**Optimizing Rural Mexico's Energy Future**

EnerQuantum is an open-source web application designed to enhance energy reliability in Mexico by reducing grid losses (15-20%) and optimizing renewable integration. Developed for the **D3CODE 2025 Hackathon** by UST, hosted by Tecnológico de Monterrey, it leverages a data ecosystem (PostgreSQL), AI forecasting (PyTorch), and quantum optimization (Qiskit). The C# Blazor frontend, styled with Tailwind CSS, features an interactive OpenStreetMap and date picker for accessible, low-bandwidth UX, targeting rural areas like Oaxaca and Jalisco.

## Table of Contents
- [Features](#features)
- [Architecture](#architecture)
- [Setup](#setup)
- [Usage](#usage)
- [Data Sources](#data-sources)
- [Contributing](#contributing)
- [License](#license)
- [Team](#team)

## Features
- **Interactive Map**: Select rural areas (e.g., Oaxaca) using OpenStreetMap to view energy metrics.
- **Date Range Picker**: Choose historical periods (e.g., Jan 1-7, 2025) or "Current" for real-time data via Flatpickr.
- **Energy Status**: Displays usage (kWh), renewable output (solar/wind), and losses (%) from PostgreSQL.
- **AI Predictions**: PyTorch LSTM models forecast demand and detect anomalies (e.g., blackout risks from heat waves).
- **Quantum Optimization**: Qiskit’s QAOA optimizes energy redistribution, targeting 10-15% loss reduction.
- **Accessible UX**: Light palette (whites, grays, blues), WCAG 2.1 AA compliant, optimized for low-bandwidth rural users.
- **Open-Source**: Hosted on GitHub for transparency and collaboration.

## Architecture
- **Frontend**: C# Blazor (Razor) with Tailwind CSS, Leaflet.js for OpenStreetMap, Flatpickr for date selection.
- **Backend**: Flask API (`/status`, `/predict`) for data retrieval and AI/quantum processing.
- **Database**: PostgreSQL with TimescaleDB for time-series storage of energy data.
- **Data Pipeline**: Python script (`data_loader.py`) loads simulated IoT data (CFE, NASA POWER) into PostgreSQL.
- **AI**: PyTorch LSTM for demand forecasting and anomaly detection.
- **Quantum**: Qiskit QAOA for grid optimization, minimizing losses.
- **Orchestration**: Airflow for workflows; Docker for containerization (optional).

![Architecture Diagram](assets/architecture.png) *(Placeholder: Add diagram to repo)*

## Setup
### Prerequisites
- **Environment**: .NET 8.0 (Blazor), Python 3.12+, PostgreSQL 16, Docker (optional for DB).
- **Dependencies**:
  - **Frontend**: Tailwind CSS (CDN), Leaflet.js (CDN), Flatpickr (CDN).
  - **Backend**: `pip install flask psycopg2-binary torch scikit-learn qiskit qiskit-aer pennylane apache-airflow numpy pandas`.
  - **Database**: PostgreSQL with TimescaleDB (`docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=password timescale/timescaledb:latest-pg16`).
- **Data**: CSVs from:
  - CFE/SENER: [sie.energia.gob.mx](https://sie.energia.gob.mx) (National Energy Balance, PRODESEN).
  - NASA POWER: [power.larc.nasa.gov](https://power.larc.nasa.gov) (hourly solar/wind for Mexico).

### Installation
1. **Clone Repository**:
   ```bash
   git clone https://github.com/enerquantum.git
   cd enerquantum
   ```
2. **Backend Setup**:
   - Install Python dependencies:
     ```bash
     pip install -r backend/requirements.txt
     ```
   - Setup PostgreSQL:
     - Create database: `createdb enerquantum`.
     - Initialize tables: `psql -d enerquantum -f backend/schema.sql`.
   - Load data: `cd backend && python data_loader.py`.
   - Run Flask: `python app.py`.
3. **Frontend Setup**:
   - Open Blazor project in Visual Studio or VS Code.
   - Add CDNs to `wwwroot/index.html` (or `Pages/_Host.cshtml` for Blazor Server):
     ```html
     <link rel="stylesheet" href="https://cdn.tailwindcss.com">
     <script src="https://cdn.tailwindcss.com"></script>
     <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
     <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
     <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
     <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
     <script src="js/site.js"></script>
     ```
   - Run Blazor: `dotnet run`.
4. **Data Loading**:
   - Download CSVs from CFE/SENER (e.g., National Energy Balance 2024) and NASA POWER (hourly solar/wind for Oaxaca).
   - Run `backend/data_loader.py` to insert data into PostgreSQL, simulating IoT inputs.

## Usage
1. **Access Web App**: Open `http://localhost:5000` (Blazor app).
2. **Select Area**: Click on the OpenStreetMap (centered on Mexico, lat: 23.63, lon: -102.55) to choose a rural area (e.g., Oaxaca).
3. **Choose Date Range**: Use Flatpickr to select a historical range (e.g., Jan 1-7, 2025) or toggle “Current” for real-time data.
4. **View Data**: Click “View Data” to fetch metrics (usage, renewables, losses) via `/status` API.
5. **Predict Problems**: Click “Predict Problems” to get AI/quantum insights (e.g., blackout risks) via `/predict` API.
6. **Results**: View metrics and predictions in the results card (e.g., “Usage: 300 kWh, Current Problems: Overload risk”).

## Data Sources
- **CFE/SENER (SIE)**: Electricity generation, demand, losses (15-20%) from [sie.energia.gob.mx](https://sie.energia.gob.mx).
- **NASA POWER**: Hourly solar/wind data for renewables from [power.larc.nasa.gov](https://power.larc.nasa.gov).
- **IEA/Ember**: Global/Mexico energy trends for validation from [iea.org](https://www.iea.org/data-and-statistics) and [ember-climate.org](https://ember-climate.org).
- **Simulated IoT**: Mock sensor data from CFE/NASA CSVs, loaded via `data_loader.py`.

## Contributing
We welcome contributions! Follow these steps:
1. Fork the repository.
2. Create a branch: `git checkout -b feature/your-feature`.
3. Commit changes: `git commit -m "Add your feature"`.
4. Push to branch: `git push origin feature/your-feature`.
5. Open a pull request with a clear description.

**Guidelines**:
- Follow C# coding standards for Blazor (PascalCase, XML comments).
- Use PEP 8 for Python backend code.
- Ensure accessibility (WCAG 2.1 AA).
- Test with `dotnet test` (Blazor) and `pytest` (backend).
- Document new features in `docs/`.

## License
Licensed under the [MIT License](LICENSE).

## Team
- **Víctor André Velázquez Salcido** (Tecnológico de Monterrey) – Project Lead, UI/UX Developer.
- **Diego Martín Lizárraga Sánchez** (Tecnológico de Monterrey) – Lead AI Developer.
- **Diana Carolina Rodríguez Pelayo** (Tecnológico de Monterrey) – Data Analyst.
- **Isaac Esaú Vega Reynaga** (Tecnológico de Monterrey) – Quantum Developer.

---

*Built with 💡 for D3CODE 2025, hosted by UST, to empower Mexico with sustainable energy solutions.*