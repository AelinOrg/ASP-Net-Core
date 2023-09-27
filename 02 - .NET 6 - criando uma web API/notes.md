## Criando um projeto .NET 6
Com os recursos necess�rios instalados, podemos come�ar a cria��o da API. Para isso, crie um novo projeto do tipo API Web ASP.NET Core. Nesse template, temos diversas estruturas e configura��es j� prontas para o desenvolvimento de uma API.

### launchSettings.json
O arquivo `launchSettings.json` � respons�vel por definir as configura��es externas de inicializa��o da aplica��o. Nele, podemos definir as portas que ser�o utilizadas para a execu��o da aplica��o, o browser que ser� aberto, o ambiente de execu��o, entre outras configura��es. Podemos remover o "IIS Express".

### appsettings.json
O arquivo appsettings.json � respons�vel por definir as configura��es da aplica��o. Nele, podemos definir as configura��es de conex�o com o banco de dados, configura��es de autentica��o, configura��es de cache, entre outras configura��es.

### Program.cs
O arquivo `Program.cs` � respons�vel por definir as configura��es gerais de inicializa��o da aplica��o. Nele, podemos definir as configura��es de inicializa��o do host, configura��es de logging, configura��es de ambiente, entre outras configura��es.

### Controllers
Olhando a classe `WeatherForecastController`, podemos ver que ela herda da classe `ControllerBase`. Essa classe � respons�vel por prover diversos recursos para a cria��o de uma API. Nela, temos recursos para a cria��o de rotas, valida��es, entre outros recursos.

O decorator `[Route]` � respons�vel por definir a rota que ser� utilizada para acessar o controller.

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
O decorator `[ApiController]` � respons�vel por definir que o controller � uma API. Com isso, temos diversos recursos dispon�veis para a cria��o de uma API.

O `[Route("[controller]")]` define que a rota do controller ser� o nome do controller. No caso, a rota ser� `/WeatherForecast`. Devido a isso, � importante que o nome do controller termine com a palavra "Controller".

### Swagger
O Swagger � uma ferramenta que permite a cria��o de uma documenta��o para a API. Com ele, podemos definir as rotas, os par�metros, os retornos, entre outras informa��es. Como utilizamos o template de API Web ASP.NET Core, o Swagger j� est� configurado para ser utilizado.

## Recebendo os dados de um filme
Antes de recebermos os dados de um filme, precisamos criar uma entidade e um controller. Para isso, crie uma pasta chamada `Models` e crie uma classe chamada `Movie.cs`. Essa classe ser� respons�vel por definir a entidade de um filme.

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

Al�m da anota��o `Route` com o uso da nota��o `[controller]`, e da anota��o `HttpPost`, temos o `[FromBody]`. Esse decorator � respons�vel por definir que o par�metro `movie` ser� recebido no corpo da requisi��o.

## Validando par�metros recebidos
Para validar os par�metros recebidos, podemos utilizar o conceito de Data Annotations. Com ele, podemos definir regras de valida��o para os par�metros recebidos e tamb�m mensagens de erro. Para isso, vamos alterar a classe `Movie.cs`.

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

Agora, se tentarmos enviar um filme sem o t�tulo, receberemos uma mensagem de erro.

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
Para retornar os filmes da API, vamos criar um m�todo `Get` no controller `MovieController.cs`.

```csharp
[HttpGet]
public IEnumerable<Movie> Get()
{
	return movies;
}
```

Por que usamos o `IEnumerable` e n�o o `List`? Em algum momento, podemos querer alterar a implementa��o da lista (`movies`) para outro tipo, que implementar� o `IEnumerable`. Com isso, n�o precisaremos alterar o retorno do m�todo. Al�m disso, se n�o iremos alterar a lista, n�o precisamos expor a implementa��o dela.

Ainda sobre `IEnumerable`, ele faz com que uma query seja executada no momento que o m�todo for chamado. Por exemplo, se tivermos uma query que retorna uma lista de 10 filmes, e chamarmos o m�todo `Count`, toda a query ser� executada novamente. Se convertermos o resultado da query para uma lista, e chamarmos o m�todo `Count`, a query n�o ser� executada novamente.

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

## Retornando um filme espec�fico
Para retornar um filme espec�fico, vamos atualizar nossa modelo `Movie.cs` e adicionar um identificador �nico.

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

Vale ressaltar que, para fins did�ticos, estamos utilizando uma lista est�tica para armazenar os filmes e tamb�m um identificador. Em uma aplica��o real, n�o podemos utilizar esses recursos.

Para incluir um par�metro na rota, utilizamos o `{}`. O `FirstOrDefaul` retorna o primeiro elemento de uma lista, ou o valor padr�o, muitas vezes `null`, caso n�o encontre nenhum elemento.

## Paginando os resultados
Atualmente, estamos retornando todos os filmes da API. Por�m, em uma aplica��o real, n�o podemos retornar todos os filmes de uma vez, pois isso pode causar problemas de performance. Para isso, vamos atualizar o m�todo `Get` do controller `MovieController.cs`.

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

Para obter os par�metros da query string, utilizamos o decorator `[FromQuery]`. O m�todo `Skip` � respons�vel por pular uma quantidade de elementos da lista, j� o m�todo `Take` retorna uma quantidade de elementos da lista.

## Padronizando o retorno
Na busca por um filme espec�fico, podemos retornar um filme ou `null`. Por�m, idealmente, devemos retornar um status code 404. Para isso, vamos atualizar o m�todo `GetOne` do controller `MovieController.cs`.

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

Repare que, ao inv�s de retornarmos um `Movie`, estamos retornando um `ActionResult<Movie>`. Esse tipo de retorno � respons�vel por definir o status code da requisi��o. No caso, estamos retornando um status code 200 (Ok) ou 404 (NotFound).

Na cria��o, podemos retornar o objeto criado e tamb�m o caminho (`location`) em que o filme que acaba de ser cadastrado.

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

O `CreatedAtAction` espera o nome do m�todo que ser� chamado, os par�metros que ser�o passados e o objeto que ser� retornado. Com isso, o status code 201 (Created) ser� retornado, pois o filme foi **criado** com sucesso.

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

Essa classe � respons�vel por definir o contexto do banco de dados. Nela, definimos as tabelas que ser�o criadas no banco de dados. Para isso, utilizamos a propriedade `DbSet`. Atrav�s do contexto, podemos nos comunicar com o banco de dados e realizar opera��es de CRUD.

Podemos pensar no `DbContext` como a conex�o do banco de dados e um conjunto de tabelas, e em `DbSet` como uma representa��o das pr�prias tabelas. O `DbContext` permite vincular as propriedades do seu modelo (presumivelmente usando o Entity Framework) ao banco de dados com uma string de conex�o.

No construtor, recebemos as configura��es do banco de dados atrav�s do `DbContextOptions` como o tipo do banco de dados, a string de conex�o, entre outras configura��es.

Agora, no `appSettings.json`, definimos as configura��es da conex�o:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "server=localhost;user=root;password=;database=movieapi"
  }
}
```

No `Program.cs`, definimos a conex�o com o banco de dados:

```csharp
//...
builder.Services.AddDbContext<MovieContext>(options =>
{
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//...
```

## Gerando a primeira migration
Antes de qualquer coisa, precisamos anotar o `Id` da classe `Movie` com o decorator `[Key]`. Esse decorator � respons�vel por definir que a propriedade � uma chave prim�ria. Por padr�o, usa-se a estrat�gia de auto-incremento, o que resulta em um campo do tipo `int`. Por�m, podemos facilmente alterar isso se desejarmos outro comportamento.


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

Agora, vamos gerar a primeira migration usando a interface do Visual Studio. Para isso, acesse "Ferramentas > Gerenciador de Pacotes do NuGet > Console do Gerenciador de Pacotes" e execute o comando `Add-Migration InitialCreate`. para gerar uma migration com o nome `InitialCreate`. Conclu�da essa etapa, vamos executar o comando `Update-Database` para rodar a migration.

### Migrations
No contexto do ASP.NET, as migra��es s�o uma forma de gerenciar as altera��es no esquema do banco de dados de forma controlada e automatizada. Elas permitem que voc� descreva as altera��es no modelo de dados do seu aplicativo e aplique essas altera��es ao banco de dados de maneira consistente.

As migra��es s�o implementadas usando o Entity Framework Core, uma biblioteca popular para mapeamento objeto-relacional (ORM) no ASP.NET. O Entity Framework Core fornece uma CLI (Command-Line Interface) que permite gerar migra��es e aplic�-las ao banco de dados.

Aqui est�o algumas recomenda��es e boas pr�ticas ao trabalhar com migra��es no ASP.NET:

1. **Use migra��es para todas as altera��es no esquema do banco de dados**: � uma pr�tica recomendada usar migra��es para todas as altera��es no esquema do banco de dados, como cria��o de tabelas, adi��o de colunas, altera��o de tipos de dados, etc. Isso garante que as altera��es sejam aplicadas de forma consistente e controlada.

2. **Nomeie as migra��es de forma descritiva**: D� nomes significativos �s migra��es para que seja f�cil entender quais altera��es elas representam. Use nomes descritivos que indiquem claramente o prop�sito da migra��o, como "CreateUsersTable" ou "AddEmailColumnToUsers".

3. **Divida as migra��es em arquivos menores**: Se uma migra��o se tornar muito grande e complexa, considere dividi-la em v�rias migra��es menores. Isso facilita a compreens�o das altera��es e a manuten��o do c�digo. Cada migra��o deve representar uma �nica altera��o l�gica no esquema do banco de dados.

4. **Mantenha um hist�rico completo de migra��es**: � importante manter um hist�rico completo de todas as migra��es aplicadas ao banco de dados. Isso permite que voc� rastreie as altera��es feitas ao longo do tempo e reverta para estados anteriores, se necess�rio. N�o remova migra��es antigas, a menos que voc� tenha certeza de que n�o s�o mais relevantes.

5. **Teste as migra��es em ambientes de desenvolvimento e staging**: Antes de aplicar as migra��es em um ambiente de produ��o, � recomendado test�-las em ambientes de desenvolvimento e staging. Isso ajuda a identificar poss�veis problemas ou conflitos com o esquema existente.

6. **Documente as migra��es**: Ao criar migra��es, � �til documentar as altera��es que est�o sendo feitas. Isso pode ser feito adicionando coment�rios no c�digo da migra��o ou mantendo uma documenta��o separada. Isso facilita a compreens�o das altera��es no futuro.

7. **Fa�a backup do banco de dados antes de aplicar migra��es**: Antes de aplicar migra��es em um ambiente de produ��o, � recomendado fazer um backup completo do banco de dados. Isso garante que voc� possa restaurar o banco de dados em caso de problemas durante a aplica��o das migra��es.

Seguir essas recomenda��es e boas pr�ticas ajudar� a garantir que as altera��es no esquema do banco de dados sejam gerenciadas de forma consistente e controlada, facilitando a manuten��o e evolu��o do seu aplicativo.

## Realizando opera��es no banco
Agora que j� temos o banco de dados configurado, podemos realizar opera��es de CRUD. Para isso, vamos atualizar o controller `MovieController.cs` para injetar o contexto do banco de dados.

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

O `SaveChanges` � respons�vel por acionar o processo de persist�ncia no banco de dados.

### Inje��o de depend�ncia
A inje��o de depend�ncia � um padr�o de projeto que permite que as classes definam suas depend�ncias externas em vez de cri�-las. Isso permite que as depend�ncias sejam substitu�das por implementa��es diferentes, como por exemplo, em testes unit�rios.

No construtor do controller, recebemos o contexto do banco de dados. Com isso, podemos realizar opera��es de CRUD.

### Contexto do banco de dados
O contexto do banco de dados � respons�vel por realizar opera��es de CRUD. Para isso, ele possui diversos m�todos, como `Add`, `Remove`, `Update`, `Find`, `SaveChanges`, entre outros. Para cada tabela, temos um `DbSet` no contexto, o que nos permite realizar opera��es espec�ficas para cada tabela.

## Utilizando DTOs
Em vez de deixar a valida��o no modelo, podemos criar um DTO (Data Transfer Object) para realizar a valida��o. Para isso, vamos criar uma pasta chamada `Data/DTOs` e criar uma classe chamada `CreateMovieDto.cs`.

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

Como n�o � responsabilidade do usu�rio definir o `Id`, n�o precisamos incluir essa propriedade no DTO. 

Agora, vamos remover as anota��es de valida��o da classe `Movie.cs`.

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

Note que mantivemos o `MaxLength` na propriedade `Genre`, pois o Entity Framework usa essa anota��o para definir o tamanho m�ximo do campo no banco de dados (`varchar(20)`). Ela � dierente da anota��o `StringLength`, que � usada para valida��o.

Com o DTO em m�os, podemos usar um pacote para mapear o DTO para o modelo. Para isso, vamos instalar os pacotes `AutoMapper` e `AutoMapper.Extensions.Microsoft.DependencyInjection`. Feito isso, precisamos configurar o AutoMapper no `Program.cs`.

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

A configura��o � super simples e intuitiva, basta herdar da classe `Profile` e chamar o m�todo `CreateMap` para mapear o DTO para o modelo.

Agora, vamos atualizar o controller `MovieController.cs` injetando o `IMapper` e alterando o m�todo `Create`.

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
Antes de atualizarmos um filme, precisamos criar um DTO para atualiza��o. Para isso, vamos criar uma classe chamada `UpdateMovieDto.cs`.

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

Atualizamos tamb�m o `MovieProfile.cs` para mapear o DTO de atualiza��o.

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

Agora, vamos atualizar o controller `MovieController.cs` para receber o DTO de atualiza��o.

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

Note que, ao inv�s de retornarmos o filme atualizado, retornamos um status code 204 (NoContent). Isso porque, ao atualizarmos um filme, n�o precisamos retornar o filme atualizado, pois o cliente j� possui os dados do filme.

## Atualizando dados com PATCH
O m�todo `PATCH` � respons�vel por atualizar parcialmente um recurso. Para isso, antes instale `Microsoft.AspNetCore.Mvc.NewtonsoftJson` e adicione o seguinte c�digo no `Program.cs`.

```csharp
//...
builder.Services.AddControllers().AddNewtonsoftJson();
//...
```

Agora, atualizamos o `MovieProfile.cs` para incluir a convers�o do filme para o DTO de atualiza��o.

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

O `JsonPatchDocument` � respons�vel por armazenar as altera��es que ser�o realizadas no filme. Para aplicar as altera��es, utilizamos o m�todo `ApplyTo`. Para validar o DTO, utilizamos o m�todo `TryValidateModel`. Caso a valida��o falhe, retornamos um status code 400 (BadRequest). O `ModelState` � respons�vel por armazenar os valores e as mensagens de erro da valida��o.

Ap�s executar a valida��o, mapeamos o DTO para o modelo e salvamos as altera��es no banco de dados.

O payload para atualiza��o parcial � o seguinte:

```json
[
	{
		"op": "replace",
		"path": "/title",
		"value": "New title"
	}
]
```

O `op` � respons�vel por definir a opera��o que ser� realizada. No caso, estamos substituindo o valor da propriedade `title`, o `path` por definir o caminho da propriedade que ser� alterada, j� o `value`, defini o valor que ser� atribu�do � propriedade. Note que � um array, pois podemos realizar diversas opera��es em um �nico payload.

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
Para documentar a API, vamos utilizar o Swagger. Como j� o temos configurado, basta adicionarmos coment�rios nas classes e m�todos. Antes, no `Program.cs`, vamos adicionar o seguinte c�digo:

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

E ent�o, ativar a op��o de gera��o de XML. Para isso, clique com o bot�o direito no projeto e acesse "Propriedades > Criar > Sa�da > Arquivo de documenta��o". Ou ent�o, manualmente, clique duplo no projeto e adicione o seguinte c�digo no arquivo `.csproj`.

```xml
//...
<PropertyGroup>
	//...
	<GenerateDocumentationFile>true</GenerateDocumentationFile>	
	//...
</PropertyGroup>
//...
```
Agora, vamos adicionar coment�rios, por exemplo, no m�todo `Create` do controller `MovieController.cs`.

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

Al�m de coment�rios, podemos utilizar o decorator `[ProducesResponseType]` para definir os status codes que ser�o retornados pelo m�todo.

## Relacionando Endere�o e Cinema
Para relacionar o endere�o e o cinema, vamos incluir algumas propriedades na classe `Cine.cs`.

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

O Entity Framework � respons�vel por criar a chave estrangeira e o relacionamento entre as tabelas. Para isso, basta que n�s especifiquemos a entidade relacionada (propriedade de navega��o) e a chave estrangeira.

Agora, vamos atualizar tamb�m a classe `Address.cs`.

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

O que fizemos foi criar uma rela��o de cardinalidade 1 para 1. Ou seja, um endere�o pertence a um cinema e um cinema possui um endere�o. Ambos s�o obrigat�rios. No caso do modelo de endere�o, n�o h� chave estrangeira pois apenas o lado dito owner possui uma.

Incluiremos o `AddressId` no DTO de cria��o de cinema.

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

Por fim, rodamos o comando �Add-Migration AddAddressToCine� e �Update-Database�.

## Lazy Properties
Por padr�o, o Entity Framework carrega as propriedades de navega��o (`virtual`) de forma lazy. Ou seja, ele s� carrega as rela��es quando elas s�o acessadas. Por exemplo, se acessarmos a propriedade `Address` do `Cine`, o Entity Framework ir� carregar o endere�o do cinema, do contr�rio, n�o.

Em programa��o, particularmente no contexto de banco de dados e ORM, o termo lazy � usado para descrever uma opera��o que � executada somente quando necess�rio. Por exemplo, se voc� tem uma propriedade que � carregada sob demanda, ela � considerada lazy. O contr�rio disso s�o as opera��es eager, que s�o executadas imediatamente, isto �, no nosso caso, trazendo todos os dados de uma vez.

Digamos que o endere�o est� contido em DTO aninhado:

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

Se n�o mapearmos corretamento o `ReadCineDto`, o endere�o n�o ser� carregado. Para isso, vamos atualizar o `CineProfile.cs`.

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

O `ForMember` � respons�vel por mapear uma propriedade espec�fica. No caso, estamos mapeando a propriedade `Address` do `Cine` para a propriedade `Address` do `ReadCineDto`, ou seja, "ensinamos" o AutoMapper a mapear uma propriedade de navega��o em um DTO.

Ao usar o `ReadCineDto`, o endere�o ser� carregado, pois acessamos a propriedade `Address`. Outro caso curioso de acesso � atrav�s do debugger. Como ele descreve o objeto, ele acaba acessando as propriedades de navega��o, o que faz com que elas sejam carregadas. Em um cen�rio mais comum, quando chamamos o m�todo `ToList`, por exemplo, as propriedades de navega��o n�o ser�o carregadas. Ainda assim, � importante usar o `ToList` apenas ao final da query, pois ele aciona a execu��o da query.

Agora partindo para a implementa��o, vamos instalar e configurar o pacote `Microsoft.EntityFrameworkCore.Proxies`, o respons�vel por carregar as propriedades de navega��o de forma lazy. Feito isso, vamos atualizar o `Program.cs`.

```csharp
//...
builder.Services.AddDbContext<MovieContext>(options =>
{
	options.UseLazyLoadingProxies.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//...
```

Apenas inclu�mos o m�todo `UseLazyLoadingProxies` na configura��o do banco de dados. Pronto, temos o lazy loading funcionando.

## Relacionando Filme e Sess�o
Para relacionar o filme e a sess�o, vamos incluir algumas propriedades na classe `Session.cs`.

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

Estamos configurando uma cardinalidade 1 para N. Ou seja, um filme possui v�rias sess�es e uma sess�o pertence a um filme. A propriedade `MovieId` � a chave estrangeira e indica que exatamente isso, que a sess�o pertence a um filme (um-para-um).

Agora, vamos atualizar a classe `Movie.cs`.

```csharp
//...

public class Movie
{
	//...

	public virtual ICollection<Session> Sessions { get; set; } = null!;
}
```

A propriedade `Sessions` � respons�vel por armazenar as sess�es do filme. Note que ela � do tipo `ICollection`, ou seja, uma cole��o de sess�es. Usamos uma interface para que possamos usar qualquer implementa��o de cole��o, como `List`, `HashSet`, `LinkedList`, entre outras. Seria poss�vel usar uma lista, por exemplo, mas n�o � uma boa pr�tica expor a implementa��o da cole��o.

O Entity Framework � respons�vel por criar a chave estrangeira e o relacionamento entre as tabelas.

Com o modelo, DTOs, mapeamentos, controller e contexto prontos, vamos rodar os comandos `Add-Migration AddSessionToMovie` e `Update-Database`.

## Relacionando Cinema e Sess�o
Para relacionar o cinema e a sess�o, vamos incluir algumas propriedades na classe `Session.cs`.

```csharp
//...

public class Session
{
	//...

	public int? CineId { get; set; }
	public virtual Cine Cine { get; set; }
}
```

Como existem criados sess�es criadas no banco de dados, precisamos definir a propriedade `CineId` como opcional. Caso contr�rio, o Entity Framework ir� tentar criar uma coluna no banco de dados com a restri��o `NOT NULL`, o que resultar� em um erro.

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
Para relacionar o filme e cinema, precisamos de uma terceira tabela, que chamamos de tabela de relacionamento. No caso, a `Session` � essa tabela. Podemos remover sua propriedade `Id`, visto que n�o precisamos dela, mantenhamos apenas as propriedades de relacionamento (`MovieId` e `CineId`). Com isso, � necess�rio ajustar o controller de sess�o.

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
1. Definimos a chave prim�ria da tabela de relacionamento como uma tupla de `MovieId` e `CineId`, ou seja, uma chave composta.
2. Definimos o relacionamento entre `Session` e `Movie`. Ou seja, uma sess�o possui um filme e um filme possui v�rias sess�es. Atrav�s do `HasForeignKey`, definimos a chave estrangeira da tabela de relacionamento.									
3. Semelhante ao filme, definimos o relacionamento entre `Session` e `Cine`. 

Agora, vamos rodar os comandos `Add-Migration AddSessionToMovieAndCine` e `Update-Database`.

## Desabilitando o cascade delete
Por padr�o, o Entity Framework habilita o cascade delete. Ou seja, ao deletarmos um endere�o, por exemplo, o cinema tamb�m ser� deletado, e por consequ�ncia, as sess�es. Para desabilitar o cascade delete, vamos atualizar o `MovieContext.cs`.

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

O `DeleteBehavior.Restrict` � respons�vel por desabilitar rejeitar a dele��o caso existam registros relacionados. Por exemplo, se tentarmos deletar um endere�o que possui um cinema, o Entity Framework ir� rejeitar a dele��o.

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

Para evitar SQL Injection, utilizamos o placeholder `{0}` para definir o valor do par�metro.

## Consultas com LINQ
Para realizar consultas com LINQ, vamos atualizar o `MovieController.cs` para aceitar, opcionalmente, um par�metro de busca por nome do cinema.

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

O `AsQueryable` � respons�vel por transformar o `DbSet` em um `IQueryable`, o que nos permite compor consultas com LINQ, como no exemplo acima. A execu��o ocorrer� quando chamarmos um m�todo como `ToList`, que materializa os resultados em uma lista.

Fazendo um paralelo, o `AsQueryable` � semelhante ao `getQuery` do TypeORM.

Um ponto importante: estamos retornando o modelo diretamente. Isso n�o � uma boa pr�tica, pois estamos expondo o modelo. Para resolver isso, devemos mape�-lo para um DTO. Dentre as vantagens, podemos destacar a seguran�a, separa��o de responsabilidades e a possibilidade de incluir novas propriedades no DTO sem afetar o modelo. Normalmente, a nomenclatura utilizada � `ReadModel`. Lembre-se de atualizar o profile do AutoMapper.