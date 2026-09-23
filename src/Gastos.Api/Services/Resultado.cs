using System.Diagnostics.CodeAnalysis;

namespace Gastos.Api.Services;

public sealed class Resultado
{
    public static readonly Resultado Ok = new(null);

    private Resultado(Falha? falha)
    {
        Falha = falha;
    }

    public Falha? Falha { get; }

    [MemberNotNullWhen(false, nameof(Falha))]
    public bool Sucesso => Falha is null;

    public static implicit operator Resultado(Falha falha)
    {
        return new Resultado(falha);
    }
}

public sealed class Resultado<T>
{
    private Resultado(T? valor, Falha? falha)
    {
        Valor = valor;
        Falha = falha;
    }

    public T? Valor { get; }
    public Falha? Falha { get; }

    [MemberNotNullWhen(true, nameof(Valor))]
    [MemberNotNullWhen(false, nameof(Falha))]
    public bool Sucesso => Falha is null;

    public static implicit operator Resultado<T>(T valor)
    {
        return new Resultado<T>(valor, null);
    }

    public static implicit operator Resultado<T>(Falha falha)
    {
        return new Resultado<T>(default, falha);
    }
}
