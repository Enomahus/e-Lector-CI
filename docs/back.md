![logo](images/logo.png)

# Documentation technique sur le backend du projet E-Lector-CI

## Technologies

Le backend du projet est en **ASP.NET Core** en **.NET 10**.

Pour interagir avec la base de données, nous utilisons **Entity Framework 10**.
Nous sommes en **Code First**, nous utilisons donc des migrations pour faire évoluer la base de données.

Nous utilisons [MediatR](https://github.com/jbogard/MediatR) pour gérer la partie **CQRS (Command and Query Responsibility Segregation)**.

Un [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) est mis à disposition pour documenter les points de terminaison de l'API.

## Architecture

Le backend est architecturé autour de la [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/).
Le but de cette architecture est de mettre à disposition des points de terminaison
qui traduisent des **cas d'utilisation (use case)** regroupés sous une **fonctionnalité (feature)**.

![logo](images/back/usecase.jpg)

### Cas d'utilisation

Pour ajouter un nouveau use case, vous devez d'abord créer le dossier de la feature correspondante
s'il n'existe pas au sein du projet **Application**.

Ensuite vous devez créer un dossier pour votre use case, ainsi que les fichiers Controller, Command ou Query et Readme au minimum.

Voici un exemple:

![logo](images/back/usecase-example.jpg)

- **Authenticate.md**: Fichier qui permet de documenter votre cas d'utilisation et qui sera visible dans Swagger.
- **AuthenticateCommand.md**: Contient la logique de cas d'utilisation.
- **AuthenticateController.cs**: Décris le point de terminaison de l'API
- **AuthenticateResponse.cs**: Fichier qui décrit la structure du corps de la réponse http. il est optionnel selon le type de retour.

### Point de terminaison (endpoint) de l'API

Vous devez créer votre point de terminaison dans le fichier **Controller**.

![logo](images/back/endpoint-example.jpg)

Votre controller doit avoir les trois attributs suivants:

- ExcludeFromCodeCoverage: Permet de dire à Sonarqube de ne pas compter ce fichier dans le calcul de la couverture de code.
- ApiController: Permet de dire que votre Controller déclare un endpoint d'API.
- Route: Spécifie l'url de base de votre use case, qui doit être commune à tous les use cases de votre feature.
  Dans Swagger, tous les endpoints partageant la même url de base seront regroupés dans un même ensemble.

Ensuite pour le endpoint en lui-même, vous devez déclarer les attributs suivants:

- HttpGet, HttpPost,...: Spécifie la méthode pour utiliser le endpoint avec l'url associée.
- SwaggerOperation: Décrit le endpoint pour Swagger avec un identifiant et une brève description.
- Produces("application/json"): Spécifie que votre endpoint va retourner du json.
- ProducesResponseType: Décrit tous les types de réponses possibles de votre endpoint, en spécifiant le code http et la structure du corps de la réponse.

Petite spécificité pour les paramètres d'une méthode **GET**, il n'est en effet pas possible de recevoir un objet directement depuis l'url et il n'y a pas de corps dans la requête. Il est donc nécessaire de déclarer chaque paramètre individuellement dans le endpoint et d'ensuite recréer la **Query** avec tous les paramètres.

Si vos paramètres ne sont pas déclarés dans la route (ex: `[HttpGet("study/{id}")]`), il sera nécessaire d'ajouter l'attribut `[FromQuery]` devant ces derniers.

Voici un exemple:
![logo](images/back/endpoint2.jpg)

### Command ou Query

Une **Command** ou une **Query** possède la même structure de classes.
Une Command permet d'effectuer une modification de donnée alors qu'une Query ne fait que de la lecture.

Ici nous allons continuer avec l'exemple de la Command Authenticate.

#### Command

Nous avons tout d'abord le modèle qui nous sera envoyé depuis le frontend.
![logo](images/back/command1.jpg)

Il décrit les différents champs qui nous serons nécessaire et le type de réponse.

#### Validator

Ensuite il faut créer un Validator, qui permet comme son nom l'indique de valider les champs de la classe précédente.
![logo](images/back/command2.jpg)

Ici nous utilisons la syntaxe de [FluentValidation](https://docs.fluentvalidation.net/en/latest/) pour vérifier chaque champ.

Normalement avec la méthode **WithMessage** ont est censé retourner un message pour l'utilisateur, mais le problème c'est que ça ne gère pas la traduction. Pour palier à ce problème nous avons décider de retourner des codes d'erreurs qui pourront être facilement traduit dans le frontend.

Si une exception est levée par le **Validator**, le corps de la réponse suivra la structure suivante:

```{
  "data": {
    "code": "Validation",
    "description": "One or more validation failures have occurred.",
    "kind": "Validation",
    "additionalData": {
      "Password": [
        "PasswordTooShort",
        "PasswordRequiresDigit",
        "PasswordRequiresUpper"
      ]
    },
    "values": {}
  },
  "duration": 0
}
```

Ainsi dans les **additionalData**, vous aurez la liste des champs avec les codes d'erreurs associés.

#### Handler

Enfin, voici la classe qui sera appelée lorsque la Command est envoyée à MediatR que le validator n'a pas remonté de problèmes.
![logo](images/back/command3.jpg)

Dans le constructeur, vous devez spécifier les services qui seront nécessaire au use case.
Petite spécificité pour le contexte de base de données,
si vous faites une Command vous devez utiliser le WritableDbContext, sinon pour une Query ça sera ReadOnlyDbContext.

Viens ensuite la méthode Handle.

La première ligne `using var activity = ActivitySourceLog.CQRS.Start().AddParameters(request);` permet de tracer l'appel de cette méthode.

Le bloc de code suivant est lié au use case Authenticate et donc pas nécessaire pour les autres use case.

### Tests

Ici nous allons voir seulement les tests unitaires car les tests d'intégrations seront inclus dans les tests end-to-end du frontend.

L'ensemble des tests seront réunis au sein du projet **Application.UnitTests**, répartis dans des dossiers par feature.

En plus de tester le cas nominal de votre Command ou Query, il est nécessaire de tester tous les paramètres individuellement pour vérifier les que les erreurs retournées sont correctes.

Nous allons reprendre l'exemple de la commande **AuthenticateCommand** pour décrire la structure d'un test.

![logo](images/back/test1.jpg)

Commençons par l'attribut **Fact** qui signifie qu'on va décrire un simple test. Il est aussi possible de faire des **Theory** si vous voulez tester plusieurs paramètres sans dupliquer le test. Ces attributs font partie de la librairie de test [xUnit](https://xunit.net/docs/getting-started/v2/netcore/visual-studio).

Ensuite le nom du test doit respecter la syntaxe suivante: `{nom de la command}Test_Should{resultat attendu}_When{condition d'exécution}`.

Au sein d'un test nous allons retrouver à chaque fois les trois même blocs: **Arrange**, **Act** et **Assert**.

#### Arrange

Dans ce bloc de code, nous allons préparer l'exécution de notre test, en créant l'injection de dépendances et en 'mockant' les méthodes de service nécessaire.

Voici à quoi ressemble cette méthode pour information:
![logo](images/back/test2.jpg)

Globalement, on ajoute MediatR, une base de données en mémoire, la gestion des utilisateurs et les différents services 'mockés'.

Revenons à notre test, nous pouvons donc changer le comportement d'une méthode de service pour adapter son comportement au besoin du test.

Dans ce test, nous fixons tout d'abord la date du jour grâce à la ligne `t.Now().Returns(new DateTime(2024, 6, 1, 12, 0, 0));`.

Et ensuite, nous configurons la création du token JWT pour retourner ceux que l'on a créer préceddement afin de pouvoir tester que c'est bien ceux-là que l'on va recevoir à la fin du test.
`t.CreateTokensAsync(Arg.Any<string>(),Arg.Any<User>(),Arg.Any<WritableDbContext>(),Arg.Any<IDateService>()).Returns(tokens);`.

Ensuite, on créé un utilisateur pour le besoin du test et finalement on créé la commande qui sera exécutée.

#### Act

Ce bloc de code est le moins fourni car il ne ser qu'à exécuter la Command ou Query.

Dans l'exemple, nous ne nous attendons à ce que l'appel réussise donc nous faisons un simple `var result = await serviceProvider.SendAsync(command);`.

Dans le cas où vous vous attendez à recevoir une exception, il est nécessaire d'entourer votre appel comme ceci: `var result = await FluentActions.Invoking(() => serviceProvider.SendAsync(command)).Should().ThrowAsync<UserAuthenticationException>();`.

#### Assert

Ici vous allez vérifier le résultat de l'exécution de votre test. Nous utilisons pour cela la librairie [FluentValidation](https://docs.fluentvalidation.net/en/latest/index.html).

Elle permet par exemple la non nullité d'un objet grâce à la syntaxe `result.Should().NotBeNull();`.

Il est aussi possible de vérifier si une méthode d'un mock a bien été appelée ou au contraire qu'elle n'a pas été appelée.
Pour cela, il faut récupérer le mock dans une variable lors de l'étape **Arrange**. Puis faire un appel comme suit: `{mock}.Received().{méthode}`.

## Analyse de performance

En lançant le projet en local avec le Docker, vous aurez aussi accès à l'application Jaeger sur l'url <http://localhost:51000>.

![logo](images/back/jaeger1.jpg)

Cette application permet de tracer tous les appels qui sont fait sur le backend.

Si on reprend l'exemple du use case Authenticate, voici ce que donne une trace de cet appel.

![logo](images/back/jaeger2.jpg)

Vous pouvez observer les différentes méthodes qui sont traversées par l'appel http.
Le trait noir permet de suivre le chemin de l'appel dans les différentes méthodes.

La première ligne **App** est enregistré automatiquement par l'API lors de la réception de l'appel.

Les quatre lignes suivantes correspondent à la couche **CQRS**
dont les trois premières sont des comportements que l'ont à ajouter à MediatR
pour faire des traitements sur toutes les Command et Query.

Par exemple, nous avons le **PerformanceBehaviour** qui mesure le temps d'exécution du **Handle**.

Ici, nous allons nous intéresser aux quatre dernières lignes, car elles concernent l'exécution du **Handle**.

En cliquant, sur la ligne `[CQRS] AuthenticateCommand.Handle` vous aurez accès aux paramètres qui ont été envoyés à la méthode.

![logo](images/back/jaeger3.jpg)

Les lignes suivantes sont automatiquement enregistrées par **Entity Framework**.

En cliquant sur l'une d'elles, vous aurez accès à la requête **SQL** générée et ses paramètres.

![logo](images/back/jaeger4.jpg)

Pour tracer un appel dans Jaeger, il est nécessaire de faire appel à la ligne `using var activity = ActivitySourceLog.<couche>.Start();` en début de méthode.
