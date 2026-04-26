// Import du contrôleur à tester
using ATMWeb.Controllers;

// Import des DTO utilisés dans les réponses
using ATMWeb.Dtos;

// Import des exceptions métier simulées
using ATMWeb.Exceptions;

// Import de l’interface du service (qui sera mocké)
using ATMWeb.Services;

// Librairie pour des assertions lisibles
using FluentAssertions;

// Permet de manipuler les résultats HTTP
using Microsoft.AspNetCore.Mvc;

// Librairie pour créer des mocks
using Moq;

namespace ATMWeb.UnitTests.Tests;

// Classe de tests pour AtmController
[TestClass]
public class AtmControllerTests
{
    // Test : le reset fonctionne normalement
    [TestMethod]
    public void Reset_RetourneOk()
    {
        // Création d’un mock du service ATM
        var atmServiceMock = new Mock<IAtmService>();

        // Injection du mock dans le controller
        var controller = new AtmController(atmServiceMock.Object);

        // Appel de la méthode Reset
        var result = controller.Reset();

        // Vérifie que la réponse HTTP est 200 OK
        result.Result.Should().BeOfType<OkObjectResult>();

        var ok = result.Result as OkObjectResult;

        // Vérifie que la réponse contient un MessageDto
        ok!.Value.Should().BeOfType<MessageDto>();

        var dto = ok.Value as MessageDto;

        // Vérifie que le message est correct
        dto!.Message.Should().Be("ATM réinitialisé");
    }

    // Test : cas où la carte de test n’existe pas
    [TestMethod]
    public void Reset_CarteIntrouvable_RetourneNotFound()
    {
        // Création du mock
        var atmServiceMock = new Mock<IAtmService>();

        // On simule une exception levée par le service
        atmServiceMock
            .Setup(s => s.Reset())
            .Throws(new NotFoundException("Carte de test introuvable"));

        var controller = new AtmController(atmServiceMock.Object);

        // Appel du reset
        var result = controller.Reset();

        // Vérifie que le controller retourne 404 NotFound
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}