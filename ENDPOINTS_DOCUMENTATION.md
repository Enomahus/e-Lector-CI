# Endpoints de Gestion des Bureaux de Vote - Documentation

## 📋 Vue d'ensemble

Cette implémentation fournit les endpoints REST et les tests unitaires pour gérer les bureaux de vote (polling stations).

## 🔌 Endpoints Créés

### 1. **GET - Récupérer un Bureau de Vote par ID**
```
GET /api/polling-station/{id}
```
- **Description** : Récupère un bureau de vote spécifique
- **Paramètres** : 
  - `id` (path) : ID du bureau de vote
- **Réponses** :
  - `200 OK` : Bureau de vote trouvé
  - `404 Not Found` : Bureau de vote non trouvé
  - `401 Unauthorized` : Non authentifié
  - `403 Forbidden` : Permission insuffisante

### 2. **GET - Lister les Bureaux de Vote d'une Circonscription**
```
GET /api/polling-station/constituency/{constituencyId}
```
- **Description** : Récupère tous les bureaux de vote d'une circonscription
- **Paramètres** :
  - `constituencyId` (path) : ID de la circonscription
- **Réponses** :
  - `200 OK` : Liste des bureaux de vote (peut être vide)
  - `400 Bad Request` : Paramètres invalides
  - `401 Unauthorized` : Non authentifié
  - `403 Forbidden` : Permission insuffisante

### 3. **PUT - Mettre à Jour un Bureau de Vote**
```
PUT /api/polling-station/{id}
```
- **Description** : Met à jour un bureau de vote existant
- **Paramètres** :
  - `id` (path) : ID du bureau de vote à mettre à jour
  - Body (JSON) :
    ```json
    {
      "stationNumber": "05",
      "wording": "MARSEILLE",
      "constituencyId": 1,
      "isActive": true
    }
    ```
- **Réponses** :
  - `200 OK` : Mise à jour réussie (retourne l'ID)
  - `400 Bad Request` : Données invalides
  - `401 Unauthorized` : Non authentifié
  - `403 Forbidden` : Permission insuffisante
  - `404 Not Found` : Bureau de vote non trouvé

## 📁 Structure des Fichiers Créés

### Backend - Application (CQRS Pattern)

#### Queries (Lecture)
```
📦 11-Application.Features/PollingStation/GetPollingStation/
├── GetPollingStationQuery.cs
│   ├── GetPollingStationByIdQuery
│   ├── GetPollingStationByIdQueryHandler
│   ├── GetPollingStationsByConstituencyIdQuery
│   └── GetPollingStationsByConstituencyIdQueryHandler
└── GetPollingStationController.cs
```

#### Commands (Modification)
```
📦 11-Application.Features/PollingStation/UpdatePollingStation/
├── UpdatePollingStationCommand.cs
│   ├── UpdatePollingStationCommand
│   ├── UpdatePollingStationCommandValidator
│   └── UpdatePollingStationCommandHandler
└── UpdatePollingStationController.cs
```

### Tests Unitaires

```
📦 tests/Application.UnitTest/Features/PollingStation/
├── GetPollingStationTest.cs
│   ├── GetPollingStationByIdQuery_ShouldReturnNull_When_PollingStation_Does_Not_Exist
│   ├── GetPollingStationByIdQuery_ShouldReturnPollingStation_When_It_Exists
│   ├── GetPollingStationsByConstituencyIdQuery_ShouldReturnEmptyList_When_No_PollingStations_Exist
│   ├── GetPollingStationsByConstituencyIdQuery_ShouldReturnAllPollingStations_When_They_Exist
│   └── GetPollingStationsByConstituencyIdQuery_ShouldNotReturnPollingStations_From_Other_Constituencies
└── UpdatePollingStationTest.cs
    ├── UpdatePollingStationCommand_ShouldFail_When_StationNumber_Is_Empty
    ├── UpdatePollingStationCommand_ShouldFail_When_Wording_Is_Empty
    ├── UpdatePollingStationCommand_ShouldFail_When_ConstituencyId_Is_Empty
    ├── UpdatePollingStationCommand_ShouldFail_When_Constituency_Does_Not_Exist
    ├── UpdatePollingStationCommand_ShouldFail_When_Constituency_Is_Not_VotingLocation
    ├── UpdatePollingStationCommand_ShouldSucceed_When_All_Parameters_Are_Valid
    ├── UpdatePollingStationCommand_ShouldReturnZero_When_PollingStation_Does_Not_Exist
    ├── UpdatePollingStationCommand_ShouldSucceed_When_Only_StationNumber_Changes
    └── UpdatePollingStationCommand_ShouldSucceed_When_Only_Wording_Changes
```

## 🔐 Permissions Requises

- **GetPollingStation** : Pour accéder aux endpoints GET
- **UpdatePollingStation** : Pour accéder à l'endpoint PUT

Ces permissions sont déjà définies dans `AppPermission` enum.

## 🧪 Couverture des Tests

### GetPollingStationTest (5 tests)
- ✅ Retour null pour ID inexistant
- ✅ Retour des données pour ID existant
- ✅ Liste vide pour circonscription sans bureaux
- ✅ Retour tous les bureaux pour une circonscription
- ✅ Filtrage correct entre circonscriptions

### UpdatePollingStationTest (9 tests)
- ✅ Validation du numéro de station
- ✅ Validation de la dénomination
- ✅ Validation de l'ID de circonscription
- ✅ Vérification de l'existence de la circonscription
- ✅ Vérification du type VotingLocation
- ✅ Mise à jour réussie
- ✅ Gestion des ID inexistants
- ✅ Mise à jour partielle (numéro seul)
- ✅ Mise à jour partielle (dénomination seule)

## 🏗️ Architecture

### Pattern CQRS avec MediatR
- **Queries** : Lectures (GetPollingStation)
- **Commands** : Modifications (UpdatePollingStation)
- **Handlers** : Logique métier
- **Validators** : Validations (FluentValidation)

### Modèles
- **PollingStationDao** : Entity Framework (persistance)
- **PollingStationModel** : DTO (transfert de données)
- Conversion bidirectionnelle via `FromDao()` et `ToDao()`

### Contrôleurs
- Héritage de `ApiControllerBase` (middleware authorization)
- Route `/api/polling-station`
- Attributs OpenAPI pour documentation
- Gestion d'erreurs avec `Result<T>`

## 🚀 Utilisation

### Exemple 1 : Récupérer un bureau de vote
```bash
curl -X GET "https://api.example.com/api/polling-station/1" \
  -H "Authorization: Bearer {token}"
```

### Exemple 2 : Lister les bureaux d'une circonscription
```bash
curl -X GET "https://api.example.com/api/polling-station/constituency/5" \
  -H "Authorization: Bearer {token}"
```

### Exemple 3 : Mettre à jour un bureau de vote
```bash
curl -X PUT "https://api.example.com/api/polling-station/1" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "stationNumber": "05",
    "wording": "MARSEILLE",
    "constituencyId": 1,
    "isActive": true
  }'
```

## ✅ Validation des Données

### Règles de Validation
1. **StationNumber** (Numéro de Station)
   - Obligatoire
   
2. **Wording** (Dénomination)
   - Obligatoire
   - Longueur maximum : 100 caractères

3. **ConstituencyId** (ID Circonscription)
   - Obligatoire
   - La circonscription doit exister
   - La circonscription doit être de type VotingLocation

## 📊 Statuts de Réponse

### Succès
- `200 OK` : Opération réussie
- `201 Created` : Ressource créée (Create endpoint)

### Erreurs Client
- `400 Bad Request` : Données invalides
- `401 Unauthorized` : Authentification manquante/invalide
- `403 Forbidden` : Permission insuffisante
- `404 Not Found` : Ressource non trouvée

### Erreurs Serveur
- `500 Internal Server Error` : Erreur serveur

## 📝 Notes d'Implémentation

1. Les timestamps `CreatedAt` et `ModifiedAt` sont gérés automatiquement par l'ORM
2. Le statut `IsActive` est calculé en fonction de `DisabledDate`
3. Les permissions sont vérifiées via l'attribut `[WithPermission()]`
4. La journalisation CQRS est active via `ActivitySourceLog.CQRS`
5. Tous les tests utilisent une base de données en mémoire pour l'isolation
