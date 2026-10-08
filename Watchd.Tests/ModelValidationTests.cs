using System.ComponentModel.DataAnnotations;
using Watchd.Models;

namespace Watchd.Tests;

[TestClass]
public sealed class ModelValidationTests
{
    [TestMethod]
    public void Pelicula_RequiresTitle()
    {
        var pelicula = new Pelicula { Titulo = null };

        var validationResults = Validate(pelicula);

        Assert.IsTrue(validationResults.Any(result =>
            result.MemberNames.Contains(nameof(Pelicula.Titulo))));
    }

    [TestMethod]
    public void Pelicula_RejectsTitleLongerThan150Characters()
    {
        var pelicula = new Pelicula { Titulo = new string('a', 151) };

        var validationResults = Validate(pelicula);

        Assert.IsTrue(validationResults.Any(result =>
            result.MemberNames.Contains(nameof(Pelicula.Titulo))));
    }

    [TestMethod]
    public void Visualizacion_RejectsRatingOutsideOneToFive()
    {
        var visualizacion = new Visualizacion
        {
            TituloPelicula = "Película de prueba",
            Calificacion = 6
        };

        var validationResults = Validate(visualizacion);

        Assert.IsTrue(validationResults.Any(result =>
            result.MemberNames.Contains(nameof(Visualizacion.Calificacion))));
    }

    [TestMethod]
    public void Visualizacion_AcceptsRatingWithinOneToFive()
    {
        var visualizacion = new Visualizacion
        {
            TituloPelicula = "Película de prueba",
            Calificacion = 5
        };

        var validationResults = Validate(visualizacion);

        Assert.IsEmpty(validationResults);
    }

    private static List<ValidationResult> Validate(object model)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            validationResults,
            validateAllProperties: true);

        return validationResults;
    }
}
