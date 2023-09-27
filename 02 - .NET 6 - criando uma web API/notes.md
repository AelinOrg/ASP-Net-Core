## Criando um projeto .NET 6
Com os recursos necessários instalados, podemos começar a criação da API. Para isso, crie um novo projeto do tipo API Web ASP.NET Core. Nesse template, temos diversas estruturas e configurações já prontas para o desenvolvimento de uma API.

### launchSettings.json
O arquivo `launchSettings.json` é responsável por definir as configurações externas de inicialização da aplicação. Nele, podemos definir as portas que serão utilizadas para a execução da aplicação, o browser que será aberto, o ambiente de execução, entre outras configurações. Podemos remover o "IIS Express".

### appsettings.json
O arquivo appsettings.json é responsável por definir as configurações da aplicação. Nele, podemos definir as configurações de conexão com o banco de dados, configurações de autenticação, configurações de cache, entre outras configurações.

### Program.cs
O arquivo `Program.cs` é responsável por definir as configurações gerais de inicialização da aplicação. Nele, podemos definir as configurações de inicialização do host, configurações de logging, configurações de ambiente, entre outras configurações.

### Controllers
Olhando a classe `WeatherForecastController`, podemos ver que ela herda da classe `ControllerBase`. Essa classe é responsável por prover diversos recursos para a criação de uma API. Nela, temos recursos para a criação de rotas, validações, entre outros recursos.

O decorator `[Route]` é responsável por definir a rota que será utilizada para acessar o controller.

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        // ...
    }
}
```
O decorator `[ApiController]` é responsável por definir que o controller é uma API. Com isso, temos diversos recursos disponíveis para a criação de uma API.

O `[Route("[controller]")]` define que a rota do controller será o nome do controller. No caso, a rota será `/WeatherForecast`. Devido a isso, é importante que o nome do controller termine com a palavra "Controller".

### Swagger
O Swagger é uma ferramenta que permite a criação de uma documentação para a API. Com ele, podemos definir as rotas, os parâmetros, os retornos, entre outras informações. Como utilizamos o template de API Web ASP.NET Core, o Swagger já está configurado para ser utilizado.

## Recebendo os dados de um filme
Antes de recebermos os dados de um filme, precisamos criar uma entidade e um controller. Para isso, crie uma pasta chamada `Models` e crie uma classe chamada `Movie.cs`. Essa classe será responsável por definir a entidade de um filme.

```csharp
namespace MovieAPI.Models;

public class Movie
{
	public string Title { get; set; }
	public string Genre { get; set; }
	public int Duration { get; set; }
}
```

Agora, usaremos a pasta `Controllers` e criaremos uma classe chamada `MovieController.cs`.

```csharp
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;

namespace MovieAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
	private static List<Movie> movies = new List<Movie>();

	[HttpPost]
	public void Create([FromBody] Movie movie)
	{
		movies.Add(movie);
	}
}
```

Além da anotação `Route` com o uso da notação `[controller]`, e da anotação `HttpPost`, temos o `[FromBody]`. Esse decorator é responsável por definir que o parâmetro `movie` será recebido no corpo da requisição.

## Validando parâmetros recebidos
Para validar os parâmetros recebidos, podemos utilizar o conceito de Data Annotations. Com ele, podemos definir regras de validação para os parâmetros recebidos e também mensagens de erro. Para isso, vamos alterar a classe `Movie.cs`.

```csharp
using System.ComponentModel.DataAnnotations;
namespace MovieAPI.Models;

public class Movie
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    [MaxLength(20, ErrorMessage = "Genre cannot be longer than 20 characters")]
    public string Genre { get; set; }

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
    public int Duration { get; set; }
}
```

Agora, se tentarmos enviar um filme sem o título, receberemos uma mensagem de erro.

```json
{
  "errors": {
	"Title": [
	  "Title is required"
	]
  },
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-0e0a0a0a0a0a0a0a0a0a0a0a0a0a0a0a"
}
```

## Retornando filmes da API
Para retornar os filmes da API, vamos criar um método `Get` no controller `MovieController.cs`.

```csharp
[HttpGet]
public IEnumerable<Movie> Get()
{
	return movies;
}
```

Por que usamos o `IEnumerable` e não o `List`? Em algum momento, podemos querer alterar a implementação da lista (`movies`) para outro tipo, que implementará o `IEnumerable`. Com isso, não precisaremos alterar o retorno do método. Além disso, se não iremos alterar a lista, não precisamos expor a implementação dela.

Ainda sobre `IEnumerable`, ele faz com que uma query seja executada no momento que o método for chamado. Por exemplo, se tivermos uma query que retorna uma lista de 10 filmes, e chamarmos o método `Count`, toda a query será executada novamente. Se convertermos o resultado da query para uma lista, e chamarmos o método `Count`, a query não será executada novamente.

```csharp
var things = 
    from item in BigDatabaseCall()
    where ....
    select item;

// This will execute the query again
var count = things.Count();

// This will not execute the query again
var list = things.ToList();
var count = list.Count();
```

## Retornando um filme específico
Para retornar um filme específico, vamos atualizar nossa modelo `Movie.cs` e adicionar um identificador único.

```csharp
using System.ComponentModel.DataAnnotations;    
namespace MovieAPI.Models;

public class Movie
{
	public Guid Id { get; set; }

	//...
}
```

Agora, vamos atualizar o controller `MovieController.cs`.

```csharp
//...
public class MovieController : ControllerBase
{
    private static readonly List<Movie> movies = new();
    private static int id = 0;

    [HttpGet("{id}")]
    public void Create([FromBody] Movie movie)
	{
        movie.Id = id++;
		movies.Add(movie);
	}

    [HttpGet("{id}")]
    public Movie? GetOne(int id)
	{
		return movies.FirstOrDefault(movie => movie.Id == id);
	}
}
```

Vale ressaltar que, para fins didáticos, estamos utilizando uma lista estática para armazenar os filmes e também um identificador. Em uma aplicação real, não podemos utilizar esses recursos.

Para incluir um parâmetro na rota, utilizamos o `{}`. O `FirstOrDefaul` retorna o primeiro elemento de uma lista, ou o valor padrão, muitas vezes `null`, caso não encontre nenhum elemento.

## Paginando os resultados
Atualmente, estamos retornando todos os filmes da API. Porém, em uma aplicação real, não podemos retornar todos os filmes de uma vez, pois isso pode causar problemas de performance. Para isso, vamos atualizar o método `Get` do controller `MovieController.cs`.

```csharp
//...
public class MovieController : ControllerBase
{
    //...
    private readonly int itemsPerPage = 10;

    //...

    [HttpGet]
    public IEnumerable<Movie> Get([FromQuery] int page = 1) 
    {

        return movies.Skip(--page * itemsPerPage).Take(itemsPerPage);
    }

    //...
}
```

Para obter os parâmetros da query string, utilizamos o decorator `[FromQuery]`. O método `Skip` é responsável por pular uma quantidade de elementos da lista, já o método `Take` retorna uma quantidade de elementos da lista.

## Padronizando o retorno
Na busca por um filme específico, podemos retornar um filme ou `null`. Porém, idealmente, devemos retornar um status code 404. Para isso, vamos atualizar o método `GetOne` do controller `MovieController.cs`.

```csharp
//...

[HttpGet("{id}")]
public ActionResult<Movie> GetOne(int id)
{
	var movie = movies.FirstOrDefault(movie => movie.Id == id);

	if (movie == null)
	{
		return NotFound();
	}

	return Ok(movie);
}
```

Repare que, ao invés de retornarmos um `Movie`, estamos retornando um `ActionResult<Movie>`. Esse tipo de retorno é responsável por definir o status code da requisição. No caso, estamos retornando um status code 200 (Ok) ou 404 (NotFound).

Na criação, podemos retornar o objeto criado e também o caminho (`location`) em que o filme que acaba de ser cadastrado.

```csharp
//...
[HttpPost]
public ActionResult<Movie> Create([FromBody] Movie movie)
{
	movie.Id = id++;
	movies.Add(movie);

	return CreatedAtAction(nameof(GetOne), new { id = movie.Id }, movie);
}
```

O `CreatedAtAction` espera o nome do método que será chamado, os parâmetros que serão passados e o objeto que será retornado. Com isso, o status code 201 (Created) será retornado, pois o filme foi **criado** com sucesso.

## Conectando ao banco de dados
Para conectar ao banco de dados, vamos utilizar o Entity Framework Core. Para isso, vamos instalar os pacotes:
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Tools
- Pomelo.EntityFrameworkCore.MySql

Agora criamos uma pasta chamada `Data` e criamos uma classe chamada `MovieContext.cs`.

```csharp
using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class MovieContext : DbContext
{
	public MovieContext(DbContextOptions<MovieContext> options) : base(options)
	{
	}

	public DbSet<Movie> Movies { get; set; }
}
```

Essa classe é responsável por definir o contexto do banco de dados. Nela, definimos as tabelas que serão criadas no banco de dados. Para isso, utilizamos a propriedade `DbSet`. Através do contexto, podemos nos comunicar com o banco de dados e realizar operações de CRUD.

Podemos pensar no `DbContext` como a conexão do banco de dados e um conjunto de tabelas, e em `DbSet` como uma representação das próprias tabelas. O `DbContext` permite vincular as propriedades do seu modelo (presumivelmente usando o Entity Framework) ao banco de dados com uma string de conexão.

No construtor, recebemos as configurações do banco de dados através do `DbContextOptions` como o tipo do banco de dados, a string de conexão, entre outras configurações.

Agora, no `appSettings.json`, definimos as configurações da conexão:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "server=localhost;user=root;password=;database=movieapi"
  }
}
```

No `Program.cs`, definimos a conexão com o banco de dados:

```csharp
//...
builder.Services.AddDbContext<MovieContext>(options =>
{
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//...
```

## Gerando a primeira migration
Antes de qualquer coisa, precisamos anotar o `Id` da classe `Movie` com o decorator `[Key]`. Esse decorator é responsável por definir que a propriedade é uma chave primária. Por padrão, usa-se a estratégia de auto-incremento, o que resulta em um campo do tipo `int`. Porém, podemos facilmente alterar isso se desejarmos outro comportamento.


```csharp
using System.ComponentModel.DataAnnotations;
namespace MovieAPI.Models;

public class Movie
{
	[Key]
	public Guid Id { get; set; }

	//...
}
```

Agora, vamos gerar a primeira migration usando a interface do Visual Studio. Para isso, acesse "Ferramentas > Gerenciador de Pacotes do NuGet > Console do Gerenciador de Pacotes" e execute o comando `Add-Migration InitialCreate`. para gerar uma migration com o nome `InitialCreate`. Concluída essa etapa, vamos executar o comando `Update-Database` para rodar a migration.

### Migrations
No contexto do ASP.NET, as migrações são uma forma de gerenciar as alterações no esquema do banco de dados de forma controlada e automatizada. Elas permitem que você descreva as alterações no modelo de dados do seu aplicativo e aplique essas alterações ao banco de dados de maneira consistente.

As migrações são implementadas usando o Entity Framework Core, uma biblioteca popular para mapeamento objeto-relacional (ORM) no ASP.NET. O Entity Framework Core fornece uma CLI (Command-Line Interface) que permite gerar migrações e aplicá-las ao banco de dados.

Aqui estão algumas recomendações e boas práticas ao trabalhar com migrações no ASP.NET:

1. **Use migrações para todas as alterações no esquema do banco de dados**: É uma prática recomendada usar migrações para todas as alterações no esquema do banco de dados, como criação de tabelas, adição de colunas, alteração de tipos de dados, etc. Isso garante que as alterações sejam aplicadas de forma consistente e controlada.

2. **Nomeie as migrações de forma descritiva**: Dê nomes significativos às migrações para que seja fácil entender quais alterações elas representam. Use nomes descritivos que indiquem claramente o propósito da migração, como "CreateUsersTable" ou "AddEmailColumnToUsers".

3. **Divida as migrações em arquivos menores**: Se uma migração se tornar muito grande e complexa, considere dividi-la em várias migrações menores. Isso facilita a compreensão das alterações e a manutenção do código. Cada migração deve representar uma única alteração lógica no esquema do banco de dados.

4. **Mantenha um histórico completo de migrações**: É importante manter um histórico completo de todas as migrações aplicadas ao banco de dados. Isso permite que você rastreie as alterações feitas ao longo do tempo e reverta para estados anteriores, se necessário. Não remova migrações antigas, a menos que você tenha certeza de que não são mais relevantes.

5. **Teste as migrações em ambientes de desenvolvimento e staging**: Antes de aplicar as migrações em um ambiente de produção, é recomendado testá-las em ambientes de desenvolvimento e staging. Isso ajuda a identificar possíveis problemas ou conflitos com o esquema existente.

6. **Documente as migrações**: Ao criar migrações, é útil documentar as alterações que estão sendo feitas. Isso pode ser feito adicionando comentários no código da migração ou mantendo uma documentação separada. Isso facilita a compreensão das alterações no futuro.

7. **Faça backup do banco de dados antes de aplicar migrações**: Antes de aplicar migrações em um ambiente de produção, é recomendado fazer um backup completo do banco de dados. Isso garante que você possa restaurar o banco de dados em caso de problemas durante a aplicação das migrações.

Seguir essas recomendações e boas práticas ajudará a garantir que as alterações no esquema do banco de dados sejam gerenciadas de forma consistente e controlada, facilitando a manutenção e evolução do seu aplicativo.

## Realizando operações no banco
Agora que já temos o banco de dados configurado, podemos realizar operações de CRUD. Para isso, vamos atualizar o controller `MovieController.cs` para injetar o contexto do banco de dados.

```csharp
//...

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
	private readonly MovieContext _context;

	public MovieController(MovieContext context)
	{
		_context = context;
	}

	[HttpPost]
    public ActionResult<Movie> Create([FromBody] Movie movie)
    {
		_context.Movies.Add(movie);
		_context.SaveChanges();

		return CreatedAtAction(nameof(GetOne), new { id = movie.Id }, movie);
	}
}
```

O `SaveChanges` é responsável por acionar o processo de persistência no banco de dados.

### Injeção de dependência
A injeção de dependência é um padrão de projeto que permite que as classes definam suas dependências externas em vez de criá-las. Isso permite que as dependências sejam substituídas por implementações diferentes, como por exemplo, em testes unitários.

No construtor do controller, recebemos o contexto do banco de dados. Com isso, podemos realizar operações de CRUD.

### Contexto do banco de dados
O contexto do banco de dados é responsável por realizar operações de CRUD. Para isso, ele possui diversos métodos, como `Add`, `Remove`, `Update`, `Find`, `SaveChanges`, entre outros. Para cada tabela, temos um `DbSet` no contexto, o que nos permite realizar operações específicas para cada tabela.

## Utilizando DTOs
Em vez de deixar a validação no modelo, podemos criar um DTO (Data Transfer Object) para realizar a validação. Para isso, vamos criar uma pasta chamada `Data/DTOs` e criar uma classe chamada `CreateMovieDto.cs`.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class CreateMovieDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Genre is required")]
    [StringLength(20, ErrorMessage = "Genre cannot be longer than 20 characters")]
    public string Genre { get; set; }

    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
    public int Duration { get; set; }
}
```

Como não é responsabilidade do usuário definir o `Id`, não precisamos incluir essa propriedade no DTO. 

Agora, vamos remover as anotações de validação da classe `Movie.cs`.

```csharp
//...

public class Movie
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    [MaxLength(20)]
    public string Genre { get; set; }

    [Required]
    public int Duration { get; set; }
}
```

Note que mantivemos o `MaxLength` na propriedade `Genre`, pois o Entity Framework usa essa anotação para definir o tamanho máximo do campo no banco de dados (`varchar(20)`). Ela é dierente da anotação `StringLength`, que é usada para validação.

Com o DTO em mãos, podemos usar um pacote para mapear o DTO para o modelo. Para isso, vamos instalar os pacotes `AutoMapper` e `AutoMapper.Extensions.Microsoft.DependencyInjection`. Feito isso, precisamos configurar o AutoMapper no `Program.cs`.

```csharp
//...
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//...
```

Agora vamos criar uma pasta chamada `Profiles` e criar uma classe chamada `MovieProfile.cs`.

```csharp
using AutoMapper;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<CreateMovieDto, Movie>();
    }
}
```

A configuração é super simples e intuitiva, basta herdar da classe `Profile` e chamar o método `CreateMap` para mapear o DTO para o modelo.

Agora, vamos atualizar o controller `MovieController.cs` injetando o `IMapper` e alterando o método `Create`.

```csharp
//...
[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
	private readonly MovieContext _context;
	private readonly IMapper _mapper;

	public MovieController(MovieContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	[HttpPost]
	public ActionResult<Movie> Create([FromBody] CreateMovieDto movieDto)
	{
		var movie = _mapper.Map<Movie>(movieDto);

		_context.Movies.Add(movie);
		_context.SaveChanges();

		return CreatedAtAction(nameof(GetOne), new { id = movie.Id }, movie);
	}
}
```

## Atualizando dados com PUT
Antes de atualizarmos um filme, precisamos criar um DTO para atualização. Para isso, vamos criar uma classe chamada `UpdateMovieDto.cs`.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class UpdateMovieDto
{
	[Required(ErrorMessage = "Title is required")]
	public string Title { get; set; }

	[Required(ErrorMessage = "Genre is required")]
	[StringLength(20, ErrorMessage = "Genre cannot be longer than 20 characters")]
	public string Genre { get; set; }

	[Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
	public int Duration { get; set; }
}
```

Atualizamos também o `MovieProfile.cs` para mapear o DTO de atualização.

```csharp
//...
public class MovieProfile : Profile
{
	public MovieProfile()
	{
		CreateMap<CreateMovieDto, Movie>();
		CreateMap<UpdateMovieDto, Movie>();
	}
}
//...
```

Agora, vamos atualizar o controller `MovieController.cs` para receber o DTO de atualização.

```csharp
//...
[HttpPut("{id}")]
public ActionResult<Movie> Update(int id, [FromBody] UpdateMovieDto movieDto)
{
	var movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);

	if (movie == null)
	{
		return NotFound();
	}

	_mapper.Map(movieDto, movie);

	_context.SaveChanges();

	return NoContent();
}
//...
```

Note que, ao invés de retornarmos o filme atualizado, retornamos um status code 204 (NoContent). Isso porque, ao atualizarmos um filme, não precisamos retornar o filme atualizado, pois o cliente já possui os dados do filme.

## Atualizando dados com PATCH
O método `PATCH` é responsável por atualizar parcialmente um recurso. Para isso, antes instale `Microsoft.AspNetCore.Mvc.NewtonsoftJson` e adicione o seguinte código no `Program.cs`.

```csharp
//...
builder.Services.AddControllers().AddNewtonsoftJson();
//...
```

Agora, atualizamos o `MovieProfile.cs` para incluir a conversão do filme para o DTO de atualização.

```csharp
//...
public class MovieProfile : Profile
{
	public MovieProfile()
	{
		CreateMap<CreateMovieDto, Movie>();
		CreateMap<UpdateMovieDto, Movie>();
		CreateMap<Movie, UpdateMovieDto>();
	}
}
//...
```

Agora, vamos atualizar o controller `MovieController.cs` para receber um `JsonPatchDocument`.

```csharp
//...
[HttpPatch("{id}")]
public ActionResult<Movie> UpdatePatch(int id, [FromBody] JsonPatchDocument<UpdateMovieDto> patchDocument)
{
	var movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);

	if (movie == null)
	{
		return NotFound();
	}

	var movieDto = _mapper.Map<UpdateMovieDto>(movie);

	patchDocument.ApplyTo(movieDto, ModelState);

	if (!TryValidateModel(movieDto))
	{
		return ValidationProblem(ModelState);
	}

	_mapper.Map(movieDto, movie);

	_context.SaveChanges();

	return NoContent();
}
//...
```

O `JsonPatchDocument` é responsável por armazenar as alterações que serão realizadas no filme. Para aplicar as alterações, utilizamos o método `ApplyTo`. Para validar o DTO, utilizamos o método `TryValidateModel`. Caso a validação falhe, retornamos um status code 400 (BadRequest). O `ModelState` é responsável por armazenar os valores e as mensagens de erro da validação.

Após executar a validação, mapeamos o DTO para o modelo e salvamos as alterações no banco de dados.

O payload para atualização parcial é o seguinte:

```json
[
	{
		"op": "replace",
		"path": "/title",
		"value": "New title"
	}
]
```

O `op` é responsável por definir a operação que será realizada. No caso, estamos substituindo o valor da propriedade `title`, o `path` por definir o caminho da propriedade que será alterada, já o `value`, defini o valor que será atribuído à propriedade. Note que é um array, pois podemos realizar diversas operações em um único payload.

## Deletando dados
Para deletar um filme, vamos atualizar o controller `MovieController.cs`.

```csharp
//...
[HttpDelete("{id}")]
public ActionResult<Movie> Delete(int id)
{
	var movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);

	if (movie == null)
	{
		return NotFound();
	}

	_context.Movies.Remove(movie);
	_context.SaveChanges();

	return NoContent();
}
//...
```

## Documentando a API
Para documentar a API, vamos utilizar o Swagger. Como já o temos configurado, basta adicionarmos comentários nas classes e métodos. Antes, no `Program.cs`, vamos adicionar o seguinte código:

```csharp
//...
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "MovieAPI", Version = "v1" });
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
//...
```

E então, ativar a opção de geração de XML. Para isso, clique com o botão direito no projeto e acesse "Propriedades > Criar > Saída > Arquivo de documentação". Ou então, manualmente, clique duplo no projeto e adicione o seguinte código no arquivo `.csproj`.

```xml
//...
<PropertyGroup>
	//...
	<GenerateDocumentationFile>true</GenerateDocumentationFile>	
	//...
</PropertyGroup>
//...
```
Agora, vamos adicionar comentários, por exemplo, no método `Create` do controller `MovieController.cs`.

```csharp
//...
/// <summary>
/// Creates a movie
/// </summary>
/// <remarks>
/// Sample request:
///
///     POST /Movie
///     {
///        "title": "Movie title",
///        "genre": "Movie genre",
///        "duration": 120
///     }
///
/// </remarks>
/// <param name="movieDto"></param>
/// <returns>A newly created movie</returns>
/// <response code="201">Returns the newly created movie</response>
/// <response code="400">If the item is null</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
public ActionResult<Movie> Create([FromBody] CreateMovieDto movieDto)
{
	//...
}
//...
```

Além de comentários, podemos utilizar o decorator `[ProducesResponseType]` para definir os status codes que serão retornados pelo método.

## Relacionando Endereço e Cinema
Para relacionar o endereço e o cinema, vamos incluir algumas propriedades na classe `Cine.cs`.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Cine
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

	public int AddressId { get; set; }
	public virtual Address Address { get; set; } = null!;
}
```

O Entity Framework é responsável por criar a chave estrangeira e o relacionamento entre as tabelas. Para isso, basta que nós especifiquemos a entidade relacionada (propriedade de navegação) e a chave estrangeira.

Agora, vamos atualizar também a classe `Address.cs`.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models;

public class Address
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Street { get; set; } = null!;

    [Required]
    public string Number { get; set; } = null!;

	public virtual Cine Cine { get; set; } = null!;
}
```

O que fizemos foi criar uma relação de cardinalidade 1 para 1. Ou seja, um endereço pertence a um cinema e um cinema possui um endereço. Ambos são obrigatórios. No caso do modelo de endereço, não há chave estrangeira pois apenas o lado dito owner possui uma.

Incluiremos o `AddressId` no DTO de criação de cinema.

```csharp
using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Data.Dtos;

public class CreateCineDto
{
	//...

	[Required(ErrorMessage = "AddressId is required")]
	public int AddressId { get; set; }
}
```

Por fim, rodamos o comando ´Add-Migration AddAddressToCine´ e ´Update-Database´.

## Lazy Properties
Por padrão, o Entity Framework carrega as propriedades de navegação (`virtual`) de forma lazy. Ou seja, ele só carrega as relações quando elas são acessadas. Por exemplo, se acessarmos a propriedade `Address` do `Cine`, o Entity Framework irá carregar o endereço do cinema, do contrário, não.

Em programação, particularmente no contexto de banco de dados e ORM, o termo lazy é usado para descrever uma operação que é executada somente quando necessário. Por exemplo, se você tem uma propriedade que é carregada sob demanda, ela é considerada lazy. O contrário disso são as operações eager, que são executadas imediatamente, isto é, no nosso caso, trazendo todos os dados de uma vez.

Digamos que o endereço está contido em DTO aninhado:

```csharp
//...

public class ReadCineDto
{
	//...

	public ReadAddressDto Address { get; set; };
}

public class ReadAddressDto
{
	//...
}
```

Se não mapearmos corretamento o `ReadCineDto`, o endereço não será carregado. Para isso, vamos atualizar o `CineProfile.cs`.

```csharp
//...

public class CineProfile : Profile
{
	public CineProfile()
	{
		//...
		CreateMap<Cine, ReadCineDto>()
			.ForMember(cineDto => cineDto.Address, opt => opt.MapFrom(cine => cine.Address));
	}
}
```

O `ForMember` é responsável por mapear uma propriedade específica. No caso, estamos mapeando a propriedade `Address` do `Cine` para a propriedade `Address` do `ReadCineDto`, ou seja, "ensinamos" o AutoMapper a mapear uma propriedade de navegação em um DTO.

Ao usar o `ReadCineDto`, o endereço será carregado, pois acessamos a propriedade `Address`. Outro caso curioso de acesso é através do debugger. Como ele descreve o objeto, ele acaba acessando as propriedades de navegação, o que faz com que elas sejam carregadas. Em um cenário mais comum, quando chamamos o método `ToList`, por exemplo, as propriedades de navegação não serão carregadas. Ainda assim, é importante usar o `ToList` apenas ao final da query, pois ele aciona a execução da query.

Agora partindo para a implementação, vamos instalar e configurar o pacote `Microsoft.EntityFrameworkCore.Proxies`, o responsável por carregar as propriedades de navegação de forma lazy. Feito isso, vamos atualizar o `Program.cs`.

```csharp
//...
builder.Services.AddDbContext<MovieContext>(options =>
{
	options.UseLazyLoadingProxies.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//...
```

Apenas incluímos o método `UseLazyLoadingProxies` na configuração do banco de dados. Pronto, temos o lazy loading funcionando.

## Relacionando Filme e Sessão
Para relacionar o filme e a sessão, vamos incluir algumas propriedades na classe `Session.cs`.

```csharp
//...

public class Session
{
	//...

	[Required]
	public int MovieId { get; set; }
	public virtual Movie Movie { get; set; } = null!;
}
```

Estamos configurando uma cardinalidade 1 para N. Ou seja, um filme possui várias sessões e uma sessão pertence a um filme. A propriedade `MovieId` é a chave estrangeira e indica que exatamente isso, que a sessão pertence a um filme (um-para-um).

Agora, vamos atualizar a classe `Movie.cs`.

```csharp
//...

public class Movie
{
	//...

	public virtual ICollection<Session> Sessions { get; set; } = null!;
}
```

A propriedade `Sessions` é responsável por armazenar as sessões do filme. Note que ela é do tipo `ICollection`, ou seja, uma coleção de sessões. Usamos uma interface para que possamos usar qualquer implementação de coleção, como `List`, `HashSet`, `LinkedList`, entre outras. Seria possível usar uma lista, por exemplo, mas não é uma boa prática expor a implementação da coleção.

O Entity Framework é responsável por criar a chave estrangeira e o relacionamento entre as tabelas.

Com o modelo, DTOs, mapeamentos, controller e contexto prontos, vamos rodar os comandos `Add-Migration AddSessionToMovie` e `Update-Database`.

## Relacionando Cinema e Sessão
Para relacionar o cinema e a sessão, vamos incluir algumas propriedades na classe `Session.cs`.

```csharp
//...

public class Session
{
	//...

	public int? CineId { get; set; }
	public virtual Cine Cine { get; set; }
}
```

Como existem criados sessões criadas no banco de dados, precisamos definir a propriedade `CineId` como opcional. Caso contrário, o Entity Framework irá tentar criar uma coluna no banco de dados com a restrição `NOT NULL`, o que resultará em um erro.

Agora, vamos atualizar a classe `Cine.cs`.

```csharp
//...

public class Cine
{
	//...

	public virtual ICollection<Session> Sessions { get; set; } = null!;
}
```

Vamos rodar os comandos `Add-Migration AddSessionToCine` e `Update-Database`.

## Relacionando n:n com ModelBuilder
Para relacionar o filme e cinema, precisamos de uma terceira tabela, que chamamos de tabela de relacionamento. No caso, a `Session` é essa tabela. Podemos remover sua propriedade `Id`, visto que não precisamos dela, mantenhamos apenas as propriedades de relacionamento (`MovieId` e `CineId`). Com isso, é necessário ajustar o controller de sessão.

Agora, vamos atualizar o `MovieContext.cs` para definir o relacionamento muitos-para-muitos.

```csharp
//...
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.Entity<Session>()
		.HasKey(session => new { session.MovieId, session.CineId });

	modelBuilder.Entity<Session>()
		.HasOne(session => session.Movie)
		.WithMany(movie => movie.Sessions)
		.HasForeignKey(session => session.MovieId);

	modelBuilder.Entity<Session>()
		.HasOne(session => session.Cine)
		.WithMany(cine => cine.Sessions)
		.HasForeignKey(session => session.CineId);
}
//...
```

Explicando cada etapa:
1. Definimos a chave primária da tabela de relacionamento como uma tupla de `MovieId` e `CineId`, ou seja, uma chave composta.
2. Definimos o relacionamento entre `Session` e `Movie`. Ou seja, uma sessão possui um filme e um filme possui várias sessões. Através do `HasForeignKey`, definimos a chave estrangeira da tabela de relacionamento.									
3. Semelhante ao filme, definimos o relacionamento entre `Session` e `Cine`. 

Agora, vamos rodar os comandos `Add-Migration AddSessionToMovieAndCine` e `Update-Database`.

## Desabilitando o cascade delete
Por padrão, o Entity Framework habilita o cascade delete. Ou seja, ao deletarmos um endereço, por exemplo, o cinema também será deletado, e por consequência, as sessões. Para desabilitar o cascade delete, vamos atualizar o `MovieContext.cs`.

```csharp
//...
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	//...
	modelBuilder.Entity<Address>()
		.HasOne(address => address.Cine)
		.WithOne(cine => cine.Address)
		.OnDelete(DeleteBehavior.Restrict);
}
//...
```

O `DeleteBehavior.Restrict` é responsável por desabilitar rejeitar a deleção caso existam registros relacionados. Por exemplo, se tentarmos deletar um endereço que possui um cinema, o Entity Framework irá rejeitar a deleção.

# Consultas

## Consultas com RAW SQL
Para realizar consultas com RAW SQL, vamos atualizar o `CineController.cs`.

```csharp
//...
[HttpGet]
public ActionResult<IEnumerable<Cine>> Get([FromQuery] string? addressId = null)
{

	if (addressId != null)
	{
		return Ok(_context.Cines.FromSqlRaw("SELECT * FROM Cines WHERE AddressId = {0}", addressId));
	} else
	{
		return Ok(_context.Cines.FromSqlRaw("SELECT * FROM Cines"));
	}
}
//...
```

Para evitar SQL Injection, utilizamos o placeholder `{0}` para definir o valor do parâmetro.

## Consultas com LINQ
Para realizar consultas com LINQ, vamos atualizar o `MovieController.cs` para aceitar, opcionalmente, um parâmetro de busca por nome do cinema.

```csharp
//...
[HttpGet]
public ActionResult<IEnumerable<Movie>> Get([FromQuery] int? page = 1, [FromQuery] string? cineName = null)
{
	var movies = _context.Movies.AsQueryable();

	if (cineName != null)
	{
		movies = movies.Where(movie => movie.Sessions.Any(session => session.Cine.Name.Contains(cineName)));
	}

	return Ok(movies.Skip(--page * itemsPerPage).Take(itemsPerPage));
}
//...
```

O `AsQueryable` é responsável por transformar o `DbSet` em um `IQueryable`, o que nos permite compor consultas com LINQ, como no exemplo acima. A execução ocorrerá quando chamarmos um método como `ToList`, que materializa os resultados em uma lista.

Fazendo um paralelo, o `AsQueryable` é semelhante ao `getQuery` do TypeORM.

Um ponto importante: estamos retornando o modelo diretamente. Isso não é uma boa prática, pois estamos expondo o modelo. Para resolver isso, devemos mapeá-lo para um DTO. Dentre as vantagens, podemos destacar a segurança, separação de responsabilidades e a possibilidade de incluir novas propriedades no DTO sem afetar o modelo. Normalmente, a nomenclatura utilizada é `ReadModel`. Lembre-se de atualizar o profile do AutoMapper.