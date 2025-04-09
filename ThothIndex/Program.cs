using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThothIndex.App;
using ThothIndex.Domain;
using ThothIndex.Infra;

using var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<InvertedIndex>();
                    services.AddSingleton<ITextProcessor, TextProcessor>();
                    services.AddSingleton<IIndexPersistence, IndexPersistence>();
                })
                .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var index = host.Services.GetRequiredService<InvertedIndex>();
var processor = host.Services.GetRequiredService<ITextProcessor>();
var persistence = host.Services.GetRequiredService<IIndexPersistence>();

string directoryPath = configuration["directoryPath"] ?? @"D:\ThothIndex\textos";
string outputIndexPath = configuration["outputIndexPath"] ?? @"D:\ThothIndex\indice_invertido.json";

if (!Directory.Exists(directoryPath))
    Directory.CreateDirectory(directoryPath);

if (!Directory.GetFiles(directoryPath, "*.txt").Any())
{
    string[] frasesExemplo = new[]
    {
        "Chevrolet Opala preparado para arrancada noturna",
        "Drag race entre Mustang e Camaro V8 turbinado",
        "Mitsubishi Eclipse com motor forjado na pista",
        "Fusca turbo venceu a etapa regional de arrancada",
        "Chevette 2JZ impressiona no tempo de reação",
        "Dodge Charger alinhado para o burnout inicial",
        "Civic SI fez 201 metros em tempo recorde",
        "Gol turbo com nitro domina o grid de largada",
        "Fiat Uno preparado com chip de performance",
        "Saveiro rebaixada com escape direto no evento",
        "Lancer Evolution acelera forte na reta final",
        "Subaru WRX STI mantém tração integral na pista",
        "BMW M3 faz tempo de pista impecável em arrancada",
        "Porsche 911 GT3 mostra equilíbrio e torque",
        "Ford Maverick V8 finaliza a prova com estilo"
    };

    for (int i = 0; i < 15; i++)
    {
        var path = Path.Combine(directoryPath, $"arquivo_{i + 1}.txt");
        File.WriteAllText(path, frasesExemplo[i]);
    }

    Console.WriteLine("Arquivos de teste criados com sucesso.");
}

var files = Directory.GetFiles(directoryPath, "*.txt");

Console.WriteLine("Processando arquivos...");
processor.ProcessFiles(files);

Console.WriteLine("Salvando índice em disco...");
persistence.SaveToDisk(index, outputIndexPath);

Console.WriteLine("Índice salvo em: " + outputIndexPath);

while (true)
{
    Console.Write("\nDigite um termo para buscar (ou 'exit' para sair): ");
    string term = Console.ReadLine();

    if (string.Equals(term, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    var results = index.Search(term);

    if (!results.Any())
    {
        Console.WriteLine("Nenhum arquivo encontrado.");
        continue;
    }

    Console.WriteLine("Arquivos encontrados:");
    foreach (var file in results)
    {
        Console.WriteLine("- " + file);
    }
}