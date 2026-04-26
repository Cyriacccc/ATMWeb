using ATMWeb.Exceptions;
using ATMWeb.Model;
using ATMWeb.Repositories;
using ATMWeb.Services;
using FluentAssertions;
using Moq;

namespace ATMWeb.UnitTests.Tests;

[TestClass]
public class AtmServiceTests
{
    private static (Compte compte, CarteBancaire carte) CreerJeuDeDonnees()
    {
        var compte = new Compte
        {
            Id = 1,
            Solde = 100.0m,
            Operations = [],
        };

        var carte = new CarteBancaire
        {
            Id = 1,
            NumeroCarte = "123456",
            Pin = "0000",
            EstBloquee = false,
            NombreEssaisRestants = 3,
            CompteId = 1,
            Compte = compte,
        };

        compte.CarteBancaire = carte;

        return (compte, carte);
    }

    [TestMethod]
    public void ConsulterSolde_AvecBonPin_RetourneLeSolde()
    {
        var (compte, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        var resultat = service.ConsulterSolde("123456", "0000");

        resultat.Should().Be(100.0m);
    }

    [TestMethod]
    public void EffectuerVersement_AvecMontantPositif_AugmenteLeSolde()
    {
        var (compte, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        var resultat = service.EffectuerVersement("123456", "0000", 50.0m);

        resultat.Should().Be(150.0m);
        compte.Solde.Should().Be(150.0m);

        compteRepositoryMock.Verify(r => r.Update(compte), Times.Once);
        compteRepositoryMock.Verify(r => r.AddOperation(It.IsAny<Operation>()), Times.Once);
        compteRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [TestMethod]
    public void EffectuerVersement_AvecMontantNegatif_LeveUneException()
    {
        var (_, _) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        Action action = () => service.EffectuerVersement("123456", "0000", -10.0m);

        action
            .Should()
            .Throw<DomainValidationException>()
            .WithMessage("Le montant doit être positif");
    }

    [TestMethod]
    public void EffectuerRetrait_AvecMontantValide_DiminueLeSolde()
    {
        var (compte, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        var resultat = service.EffectuerRetrait("123456", "0000", 20.0m);

        resultat.Should().Be(80.0m);
        compte.Solde.Should().Be(80.0m);

        compteRepositoryMock.Verify(r => r.Update(compte), Times.Once);
        compteRepositoryMock.Verify(r => r.AddOperation(It.IsAny<Operation>()), Times.Once);
        compteRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [TestMethod]
    public void EffectuerRetrait_AvecSoldeInsuffisant_LeveUneException()
    {
        var (compte, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        Action action = () => service.EffectuerRetrait("123456", "0000", 150.0m);

        action.Should().Throw<DomainValidationException>().WithMessage("Solde insuffisant");
    }

    [TestMethod]
    public void ConsulterSolde_AvecPremierMauvaisPin_RetourneErreurEtDecrementeLesEssais()
    {
        var (_, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        Action action = () => service.ConsulterSolde("123456", "9999");

        action
            .Should()
            .Throw<UnauthorizedException>()
            .WithMessage("PIN incorrect. 2 essais restants");

        carte.NombreEssaisRestants.Should().Be(2);
        carte.EstBloquee.Should().BeFalse();
    }

    [TestMethod]
    public void ConsulterSolde_ApresTroisMauvaisPin_BloqueLaCarte()
    {
        var (_, carte) = CreerJeuDeDonnees();

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        Action a1 = () => service.ConsulterSolde("123456", "9999");
        Action a2 = () => service.ConsulterSolde("123456", "8888");
        Action a3 = () => service.ConsulterSolde("123456", "7777");

        a1.Should().Throw<UnauthorizedException>();
        a2.Should().Throw<UnauthorizedException>();
        a3.Should().Throw<ForbiddenException>().WithMessage("Carte bloquée");

        carte.NombreEssaisRestants.Should().Be(0);
        carte.EstBloquee.Should().BeTrue();
    }

    [TestMethod]
    public void ConsulterSolde_AvecCarteDejaBloquee_LeveForbiddenException()
    {
        var (compte, carte) = CreerJeuDeDonnees();
        carte.EstBloquee = true;

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        Action action = () => service.ConsulterSolde("123456", "0000");

        action
            .Should()
            .Throw<ForbiddenException>()
            .WithMessage("Action impossible : carte bloquée");
    }

    [TestMethod]
    public void Reset_RemetLEtatInitial()
    {
        var (compte, carte) = CreerJeuDeDonnees();

        compte.Solde = 250.0m;
        carte.Pin = "1111";
        carte.EstBloquee = true;
        carte.NombreEssaisRestants = 0;

        compte.Operations.Add(
            new Operation
            {
                Id = 1,
                Type = "Versement",
                Montant = 50.0m,
                DateOperation = DateTime.UtcNow,
                CompteId = compte.Id,
                Compte = compte,
            }
        );

        var carteRepositoryMock = new Mock<ICarteRepository>();
        var compteRepositoryMock = new Mock<ICompteRepository>();

        carteRepositoryMock.Setup(r => r.GetByNumeroCarte("123456")).Returns(carte);

        var service = new AtmService(carteRepositoryMock.Object, compteRepositoryMock.Object);

        service.Reset();

        compte.Solde.Should().Be(100.0m);
        carte.Pin.Should().Be("0000");
        carte.EstBloquee.Should().BeFalse();
        carte.NombreEssaisRestants.Should().Be(3);

        compteRepositoryMock.Verify(r => r.Update(compte), Times.Once);
        carteRepositoryMock.Verify(r => r.Update(carte), Times.Once);
    }
}
