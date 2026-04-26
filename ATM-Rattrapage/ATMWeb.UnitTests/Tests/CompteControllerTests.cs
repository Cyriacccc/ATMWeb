// Import du contrôleur que l’on veut tester
using ATMWeb.Controllers;

// Import des DTO utilisés dans les réponses
using ATMWeb.Dtos;

// Import des exceptions métier simulées dans les tests
using ATMWeb.Exceptions;

// Import de l’interface du service que l’on va mocker
using ATMWeb.Services;

// Bibliothèque d’assertions plus lisibles
using FluentAssertions;

// Permet de créer un faux contexte HTTP pour le controller
using Microsoft.AspNetCore.Http;

// Permet de manipuler les résultats HTTP comme OkObjectResult, BadRequestObjectResult, etc.
using Microsoft.AspNetCore.Mvc;

// Permet de créer des mocks, ici pour simuler IAtmService
using Moq;

namespace ATMWeb.UnitTests.Tests;

// Classe de tests du CompteController
[TestClass]
public class CompteControllerTests
{
    // Méthode utilitaire pour créer un controller avec un faux contexte HTTP
    private static CompteController CreerController(Mock<IAtmService> atmServiceMock)
    {
        // On injecte le faux service dans le controller
        var controller = new CompteController(atmServiceMock.Object);

        // On crée un contexte HTTP fictif pour pouvoir ajouter des headers
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
        };

        return controller;
    }

    // Test : consultation du solde avec headers valides
    [TestMethod]
    public void ConsulterSolde_AvecHeadersValides_RetourneOk()
    {
        // Création d’un mock du service
        var atmServiceMock = new Mock<IAtmService>();

        // On définit le comportement attendu :
        // si le controller appelle ConsulterSolde avec ces valeurs, le service retourne 100
        atmServiceMock.Setup(s => s.ConsulterSolde("123456", "0000")).Returns(100.0m);

        // Création du controller avec le mock
        var controller = CreerController(atmServiceMock);

        // Ajout des headers nécessaires à la requête
        controller.Request.Headers["X-Card"] = "123456";
        controller.Request.Headers["X-Pin"] = "0000";

        // Appel de la méthode du controller
        var result = controller.ConsulterSolde();

        // Vérifie que le résultat HTTP est 200 OK
        result.Result.Should().BeOfType<OkObjectResult>();

        var ok = result.Result as OkObjectResult;

        // Vérifie que le contenu retourné est un SoldeDto
        ok!.Value.Should().BeOfType<SoldeDto>();

        var dto = ok.Value as SoldeDto;

        // Vérifie que le solde retourné est bien 100
        dto!.Solde.Should().Be(100.0m);
    }

    // Test : consultation du solde sans headers
    [TestMethod]
    public void ConsulterSolde_SansHeaders_RetourneBadRequest()
    {
        var atmServiceMock = new Mock<IAtmService>();
        var controller = CreerController(atmServiceMock);

        // On n’ajoute pas les headers volontairement
        var result = controller.ConsulterSolde();

        // Le controller doit retourner 400 Bad Request
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // Test : consultation avec une carte bloquée
    [TestMethod]
    public void ConsulterSolde_AvecCarteBloquee_Retourne403()
    {
        var atmServiceMock = new Mock<IAtmService>();

        // On simule le comportement du service :
        // il lève une ForbiddenException quand la carte est bloquée
        atmServiceMock
            .Setup(s => s.ConsulterSolde("123456", "0000"))
            .Throws(new ForbiddenException("Action impossible : carte bloquée"));

        var controller = CreerController(atmServiceMock);

        controller.Request.Headers["X-Card"] = "123456";
        controller.Request.Headers["X-Pin"] = "0000";

        var result = controller.ConsulterSolde();

        // Le controller doit transformer l’exception en réponse HTTP 403
        result.Result.Should().BeOfType<ObjectResult>();

        var objectResult = result.Result as ObjectResult;

        objectResult!.StatusCode.Should().Be(403);
    }

    // Test : versement valide
    [TestMethod]
    public void EffectuerVersement_AvecMontantValide_RetourneOk()
    {
        var atmServiceMock = new Mock<IAtmService>();

        // Si le service reçoit un versement de 50, il retourne un nouveau solde de 150
        atmServiceMock.Setup(s => s.EffectuerVersement("123456", "0000", 50.0m)).Returns(150.0m);

        var controller = CreerController(atmServiceMock);

        controller.Request.Headers["X-Card"] = "123456";
        controller.Request.Headers["X-Pin"] = "0000";

        // Appel de la méthode avec un MontantDto
        var result = controller.EffectuerVersement(new MontantDto { Montant = 50.0m });

        // Vérifie que le controller répond 200 OK
        result.Result.Should().BeOfType<OkObjectResult>();

        var ok = result.Result as OkObjectResult;

        // Vérifie que le retour est un OperationResultDto
        ok!.Value.Should().BeOfType<OperationResultDto>();

        var dto = ok.Value as OperationResultDto;

        // Vérifie le solde et le message retournés
        dto!.Solde.Should().Be(150.0m);
        dto.Message.Should().Be("Versement effectué");
    }

    // Test : retrait refusé pour solde insuffisant
    [TestMethod]
    public void EffectuerRetrait_AvecSoldeInsuffisant_RetourneBadRequest()
    {
        var atmServiceMock = new Mock<IAtmService>();

        // On simule une erreur métier levée par le service
        atmServiceMock
            .Setup(s => s.EffectuerRetrait("123456", "0000", 150.0m))
            .Throws(new DomainValidationException("Solde insuffisant"));

        var controller = CreerController(atmServiceMock);

        controller.Request.Headers["X-Card"] = "123456";
        controller.Request.Headers["X-Pin"] = "0000";

        var result = controller.EffectuerRetrait(new MontantDto { Montant = 150.0m });

        // Le controller doit traduire l’exception métier en 400 Bad Request
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
