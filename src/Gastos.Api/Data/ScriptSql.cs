using System.Reflection;

namespace Gastos.Api.Data;

public static class ScriptSql
{
    private const string PrefixoRecurso = "Gastos.Api.Sql.";

    public static string LerComoExec(string nomeArquivo)
    {
        string script = Ler(nomeArquivo);

        return "EXEC(N'" + script.Replace("'", "''") + "');";
    }

    private static string Ler(string nomeArquivo)
    {
        string nomeRecurso = PrefixoRecurso + nomeArquivo;
        Assembly assembly = typeof(ScriptSql).Assembly;

        using Stream stream = assembly.GetManifestResourceStream(nomeRecurso)
            ?? throw new InvalidOperationException($"Script SQL '{nomeRecurso}' não encontrado como recurso embutido.");
        using StreamReader leitor = new(stream);

        return leitor.ReadToEnd();
    }
}
