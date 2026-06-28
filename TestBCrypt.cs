// Script temporário para testar BCrypt e gerar hash correto
// Execute com: dotnet script TestBCrypt.cs
// Ou adicione como endpoint temporário na API

using System;

class TestBCrypt
{
    static void Main()
    {
        Console.WriteLine("=== Teste de BCrypt ===\n");

        string senhaTexto = "admin123";
        string hashDoBanco = "$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi";

        // Testar o hash existente no banco
        Console.WriteLine($"Senha em texto: {senhaTexto}");
        Console.WriteLine($"Hash no banco: {hashDoBanco}");
        
        bool hashValido = BCrypt.Net.BCrypt.Verify(senhaTexto, hashDoBanco);
        Console.WriteLine($"\nO hash no banco está correto? {(hashValido ? "SIM ✓" : "NÃO ✗")}\n");

        if (!hashValido)
        {
            Console.WriteLine("GERANDO NOVO HASH CORRETO:");
            string novoHash = BCrypt.Net.BCrypt.HashPassword(senhaTexto);
            Console.WriteLine($"Novo hash: {novoHash}");
            
            // Verificar o novo hash
            bool novoHashValido = BCrypt.Net.BCrypt.Verify(senhaTexto, novoHash);
            Console.WriteLine($"Novo hash válido? {(novoHashValido ? "SIM ✓" : "NÃO ✗")}");
            
            Console.WriteLine("\n=== EXECUTE ESTE UPDATE NO BANCO ===");
            Console.WriteLine($"UPDATE usuario SET \"SenhaHash\" = '{novoHash}' WHERE email = 'admin@finansee.com';");
        }
        else
        {
            Console.WriteLine("✓ Hash está correto! O problema deve estar em outro lugar.");
        }

        Console.WriteLine("\n=== Testando outras variações ===");
        
        // Testar com espaços
        string senhaComEspaco = " admin123";
        Console.WriteLine($"\nTentando com espaço antes: '{senhaComEspaco}'");
        Console.WriteLine($"Resultado: {BCrypt.Net.BCrypt.Verify(senhaComEspaco, hashDoBanco)}");
        
        string senhaComEspacoDepois = "admin123 ";
        Console.WriteLine($"\nTentando com espaço depois: '{senhaComEspacoDepois}'");
        Console.WriteLine($"Resultado: {BCrypt.Net.BCrypt.Verify(senhaComEspacoDepois, hashDoBanco)}");
    }
}
