using ToyStore.ApiGateway.Entities;

namespace ToyStore.ApiGateway.Persistence
{
    /// <summary>
    /// Catálogo estático de produtos, mantido em memória.
    /// Não há CRUD: a lista é fixa, definida no código.
    /// </summary>
    public static class ProductCatalog
    {
        public static readonly IReadOnlyList<Product> Products = new List<Product>
    {
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000001"),
            Name = "Robô Explorador X1",
            Description = "Robô de brinquedo com luzes e sons, ideal para crianças curiosas por tecnologia.",
            Category = "Eletrônicos",
            Price = 189.90m,
            ImageUrl = "https://a-static.mlcdn.com.br/%7Bw%7Dx%7Bh%7D/brinquedo-infantil-robo-c-sensor-de-movimento-luz-e-som-etitoys/petlamp/15946629363/6235273af73b4c9283ad708104036589.jpeg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000002"),
            Name = "Pista de Carrinhos Turbo Loop",
            Description = "Pista com loop duplo e dois carrinhos inclusos. Diversão garantida.",
            Category = "Veículos",
            Price = 149.50m,
            ImageUrl = "https://http2.mlstatic.com/D_NQ_NP_2X_626427-MLA113695175287_062026-F.webp"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000003"),
            Name = "Boneca Sofia Aventureira",
            Description = "Boneca articulada com acessórios de viagem e roupas intercambiáveis.",
            Category = "Bonecas",
            Price = 99.90m,
            ImageUrl = "https://rihappy.vtexassets.com/arquivos/ids/5416217/Conjunto-Boneca-E-Acessorios---Barbie---Viajante---Mattel-1.jpg?v=638104361503870000"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000004"),
            Name = "Quebra-Cabeça Mapa Mundi 500 peças",
            Description = "Quebra-cabeça educativo com mapa-múndi ilustrado, 500 peças.",
            Category = "Educativos",
            Price = 59.90m,
            ImageUrl = "https://cdn.awsli.com.br/600x450/2437/2437567/produto/165004182/779d225232.jpg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000005"),
            Name = "Blocos de Montar Cidade Mágica",
            Description = "Conjunto com 350 blocos para construir uma cidade completa.",
            Category = "Construção",
            Price = 219.00m,
            ImageUrl = "https://http2.mlstatic.com/D_NQ_NP_800173-MLB108867112802_032026-O.webp"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000006"),
            Name = "Pelúcia Urso Soneca",
            Description = "Pelúcia macia e antialérgica, perfeita para a hora de dormir.",
            Category = "Pelúcias",
            Price = 79.90m,
            ImageUrl = "https://m.media-amazon.com/images/I/51e76g8q1nL._AC_UF894,1000_QL80_.jpg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000007"),
            Name = "Jogo de Tabuleiro Desafio Total",
            Description = "Jogo de tabuleiro para até 6 jogadores, com perguntas e desafios.",
            Category = "Jogos",
            Price = 89.90m,
            ImageUrl = "https://m.media-amazon.com/images/I/71lI3TW5GlL._AC_UF894,1000_QL80_.jpg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000008"),
            Name = "Kit Massinha Criativa 12 Cores",
            Description = "Massinhas atóxicas em 12 cores vibrantes com moldes inclusos.",
            Category = "Criatividade",
            Price = 39.90m,
            ImageUrl = "https://cdn.awsli.com.br/600x700/2640/2640978/produto/303836313/imagem7899550902697_1-6w3uw1e3t7.jpg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000009"),
            Name = "Drone Mini Voo Fácil",
            Description = "Mini drone leve e resistente, fácil de pilotar para iniciantes.",
            Category = "Eletrônicos",
            Price = 249.90m,
            ImageUrl = "https://m.media-amazon.com/images/I/61GZuh5VFIL._AC_UL960_FMwebp_QL65_.jpg"
        },
        new()
        {
            Id = Guid.Parse("8f14e45f-ceea-467e-bd96-25541d000010"),
            Name = "Cozinha Infantil Chef Mirim",
            Description = "Cozinha de brinquedo completa com fogão, pia e acessórios de cozinha.",
            Category = "Casinha",
            Price = 329.00m,
            ImageUrl = "https://m.magazineluiza.com.br/a-static/420x420/cozinha-infantil-completo-fogao-forno-pia-com-agua-acessorios-top-deluxe-lugo-brinquedos/joekidsbrinquedos/ctdll068lug/0e230c8fcf01d83be290f0ae94ccac51.jpeg"
        }
    };
    }
}
