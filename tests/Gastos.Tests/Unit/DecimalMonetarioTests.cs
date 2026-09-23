namespace Gastos.Tests.Unit;

public class DecimalMonetarioTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void DecimalSomaCentavosSemPerda()
    {
        decimal soma = 0.1m + 0.2m;

        Assert.Equal(0.3m, soma);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DoubleSomaCentavosComPerda()
    {
        double soma = 0.1 + 0.2;

        Assert.NotEqual(0.3, soma);
    }
}
