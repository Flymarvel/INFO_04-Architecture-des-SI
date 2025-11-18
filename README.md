### 🔨 Build & Test

| Raccourci                | Action                         |
| ------------------------ | ------------------------------ |
| `Ctrl+Shift+B`           | **Build** la solution          |
| `Ctrl+Shift+P` → "test"  | **Lancer les tests unitaires** |
| `Ctrl+Shift+P` → "clean" | Nettoyer (bin/obj)             |

### Build & Test (Terminal)

```powershell
# Build la solution
dotnet build

# Lancer les tests
dotnet test

# Build + tests avec rapport détaillé
dotnet test --verbosity detailed

# Nettoyer les artifacts
dotnet clean
```

### Gestion des projets (via VS Code)

```
Ctrl+Shift+P → Tasks: Run Task → Choisir :
```

1. **New Project: Class Library** → Crée une librairie
2. **New Project: Console App** → Crée une app console
3. **New Project: ASP.NET Web API** → Crée une API
4. **Add Project to Solution** → Ajoute à la solution
5. **Add Project Reference** → Ajoute une dépendance

---

## ➕ Ajouter des projets

### Via VS Code (Recommandé - Facile)

1. `Ctrl+Shift+P` → "New Project: Class Library"
2. Entrez le nom (ex: `UniversiteApplication`)
3. `Ctrl+Shift+P` → "Add Project to Solution"
4. Entrez le chemin (ex: `UniversiteApplication/UniversiteApplication.csproj`)

### Via Terminal (Manuel)

```powershell
# Créer un nouveau projet
dotnet new classlib -n UniversiteApplication -o UniversiteApplication

# Ajouter à la solution
dotnet sln add UniversiteApplication/UniversiteApplication.csproj

# Ajouter une référence (Application dépend de Domain)
dotnet add UniversiteApplication/UniversiteApplication.csproj reference UniversiteDomain/UniversiteDomain.csproj
```

---

## 🔗 Gestion des dépendances

### Ajouter une référence entre projets

```powershell
# ProjectA dépend de ProjectB
dotnet add ProjectA/ProjectA.csproj reference ProjectB/ProjectB.csproj
```

### Ordre des dépendances (Clean Architecture)

```
API → Application, Infrastructure
Application → Domain
Infrastructure → Domain, Application
Domain → (aucune dépendance)
```

### Ajouter un package NuGet

```powershell
# Via CLI
dotnet add UniversiteDomain/UniversiteDomain.csproj package Newtonsoft.Json

# Ou éditer le .csproj directement
```

---

## 🧪 Tests

Fichiers de test :

- `UniversiteDomainUnitTest/EtudiantUnitTest.cs`
- `UniversiteDomainUnitTest/ParcoursUnitTest.cs`
- `UniversiteDomainUnitTest/UeUnitTest.cs`

### Exécuter les tests

```
Ctrl+Shift+P → Tasks: Run Task → test
```

Ou en terminal :

```powershell
dotnet test
```

---

## 📝 Notes

- **OmniSharp** : IntelliSense de C# peut prendre 30sec-2min au premier chargement
- **Debug** : N'oubliez pas de lancer en `Debug` pour que les breakpoints fonctionnent
- **Format** : Les fichiers sont formatés automatiquement au save (`.editorconfig`)

---

## 🔧 Configuration VS Code

Les fichiers de configuration se trouvent dans `.vscode/` :

- `settings.json` : Paramètres de l'IDE
- `launch.json` : Configuration du débogueur
- `tasks.json` : Tâches personnalisées
