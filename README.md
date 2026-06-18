# e-Lector-CI


![logo](/docs/images/logo.png)

# Documentation technique sur le projet e-Lector-CI

## Liens utiles


## Développement

### Local

Le projet est composé d'un frontend **Angular 21** et d'un backend en **ASP.NET Core** en **.NET 10**.
L'utilisation de **Docker** est obligatoire afin de pouvoir héberger la base de données **SQL Server 2022**.

#### Backend

Pour démarrer le backend depuis une console: `docker compose -f src/back/docker-compose.yml --project-directory ./src/back up --build`
depuis la racine du projet.

Depuis **Visual Studio Code**, la tâche `Start services (Debug)` permet de lancer le projet.

Si vous utilisez **Visual Studio 2026**, ouvrez la solution **E-Lector-CI.slnx** (src/back/E-Lector-CI.slnx)
et mettez le projet **docker-compose** en projet de démarrage, puis lancer-le.
La compilation du projet web sera faite dans Visual Studio puis les fichiers seront remplacés dans le container directement.

Une fois le projet lancé, vérifiez bien que tous les containeurs sont bien démarrés.

Pour plus de détails, voir le Readme: [Lien](/docs/back.md)

#### Frontend

Nous conseillons l'utilisation de **Visual Studio Code** pour lancer le frontend.
Vous aurez accès aux tâches suivantes:

- ng serve (`npm start`): Lance le frontend
- ng lint (`npm lint`): Execute ESLint sur le frontend
- ng lint-fix (`npm lint-fix`): Execute ESLint sur le frontend en tentant de corriger les erreurs de façon sécurisée
- Generate nswag api client (`npm generate-api-client`): Génère les services pour interagir avec le backend

Pour plus de détails, voir le Readme: [Lien](/docs/front.md)

#### Base de données

Pour accéder à la base de données, nous conseillons l'utilisation des outils **SQL Server Management Studio** ou **Azure Data Studio**.

- **Host**: `localhost:43100`
- **User**: `sa`
- **Password**: `#Elector@2026`
- **Database**: `ElectorDb`

#### Urls

- **Front** : <https://localhost:44082/>
- **Swagger** : <http://localhost:44210/swagger/index.html>
- **ReDoc** : <http://localhost:44210/docs/index.html>
- **Jeager** : <http://localhost:41100/search>


### Git

Pour commencer un dévelopement, vous devez d'abord faire un **fetch** du repository pour récupérer l'état actuel de ce dernier.

Ensuite, vous devez créer une branche à partir d'**origin/develop**,
dont le nom aura le format suivant **(trigramme)\/(n° du ticket)-(description du développement en 2-3 mots)**.

Pour les messages de commit, veillez à suivre la convention décrite sur ce [Lien](https://www.conventionalcommits.org/en/v1.0.0/).

A la fin de votre développement, pensez bien à refaire un fetch pour récupérer les développements qu'il y a pu avoir entre temps.
Il faut ensuite faire un **rebase** d'**origin/develop** sur votre branche, résoudre les conflits s'il y en a, puis pousser vos commits.

Sur **GitHub**, vous pourrez créer une Pull Request de votre branche vers **develop**,
des pipelines se lanceront automatiquement pour valider vos développements.

Une validation d'un autre développeur est nécessaire pour pouvoir compléter la Pull Request.
Si lors de vos développements, vous avez créer plusieurs commits,
nous vous encourageons lors de la complétion de la Pull Request de faire un **Squash Commit**
afin des les fusionner en un seul pour garder un historique propre.

#### Environnements

- Dev:
- Qa:
- Prod:

#### Création d'un environnement

La création d'un environnement se fait automatiquement grâce aux fichiers Biceps à la racine du projet.

#### Déploiement

Le déploiement sur l'environnement de **Dev** se fait automatiquement à la fusion des Pull Request sur la branche **develop**.

Le déploiement sur l'environnement de **Qa** se fait automatiquement à la fusion des Pull Request sur la branche **qa**.