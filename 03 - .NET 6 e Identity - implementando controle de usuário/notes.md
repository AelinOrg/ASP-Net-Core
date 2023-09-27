# Introdu��o
Implementaremos um fluxo completo de usu�rio com o aux�lio do **Identity**, uma ferramenta da Microsoft que nos permite gerenciar autentica��o e autoriza��o de usu�rios em nossas aplica��es.

## O que � o Identity?
O Identity � um framework que nos permite gerenciar usu�rios, senhas, perfis e permiss�es, tokens, confirma��o de email, e muito mais.

## Iniciando...
Com o projeto de API configurado, e com a base de usu�rios criada (banco, pacotes, modelo, mapeamento, DTOs, etc), vamos come�ar a implementar o Identity. Antes, vale ressaltar alguns decorators:

`[Compare]`: Usado na valida��o, � �til para, por exemplo, comparar a senha e a senha de confirma��o.
`[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`: Usado para indicar que o campo � gerado pelo banco de dados.
`[DatabaseGenerated(DatabaseGeneratedOption.Computed)]`: Usado para indicar que o campo � gerado pelo banco de dados, mas n�o � um campo de chave prim�ria.

## Configurando o banco
Antes de tudo, precisamos dos seguintes pacotes:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Identity.Stores`
- `Pomelo.EntityFrameworkCore.MySql`
- `AutoMapper.Extensions.Microsoft.DependencyInject`

Agora, a connection string:

```json
//...
"ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=aelin;password=aquela;database=api_identify"
}
//...
```

De forma habitual, configuramos o contexto. Como estamos usando o Identity, precisamos herdar de `IdentityDbContext`:

```csharp
using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApiDbContext: IdentityDbContext<User>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }
}
```

O `IdentityDbContext` j� possui as tabelas necess�rias para o Identity. Come ela, toda a responsabilidade de gerenciar os usu�rios fica por conta do Identity.

Um ponto interessante, poder�amos usar diretamente a classe `IdentityUser`, mas como queremos adicionar campos personalizados, criamos uma classe `User` que herda de `IdentityUser`:

```csharp
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User: IdentityUser
{
    public DateTime BirthDate { get; set; }
}
```

A classe `IdentityUser` j� possui alguns campos, como `Id`, `UserName`, `Email`, `PasswordHash`, `SecurityStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, etc. Al�m disso, podemos adicionar campos personalizados, como `BirthDate`.

Agora, precisamos configurar o `Program.cs`:

```csharp
//...
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
}); 

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>()
    .AddDefaultTokenProviders();

//...
```

Sobre:
- `AddIdentity<User, IdentityRole>()`: Diz ao Identity aplicar o conceito de identidade para a classe `User` e que a parte de autoriza��o ser� gerenciada tamb�m por ele.
- `AddEntityFrameworkStores<ApiDbContext>()`: Diz ao Identity usamos o contexto `ApiDbContext` para nos comunicarmos com o banco de dados.
- `AddDefaultTokenProviders()`: Diz ao Identity a gest�o de tokens ser� responsabilidade dele.

Nesse ponto, j� podemos gerar a migration com `Add-Migration InitialCreate` e atualizar o banco com `Update-Database`.

## Cadastrando um usu�rio
Nosso `SignUpDTO` est� assim:

```csharp
using System.ComponentModel.DataAnnotations;

namespace API.Data.Dtos;

public class SignUpDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
}
```

Agora, vamos criar um m�todo para cadastrar um usu�rio:

```csharp
//...
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public UserController(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<User>> SignUp(SignUpDto dto)
    {
        User user = _mapper.Map<User>(dto);
        IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            return Ok(user);
        }
        
        throw new ApplicationException("Unknown error occurred");
    }
}
//...
```

Al�m do `_mapper`, injetamos o `UserManager<User>`. Ele � respons�vel por gerenciar os usu�rios, como criar, atualizar, deletar, etc. 

Para criar um usu�rio, basta chamar o m�todo `CreateAsync`, que recebe o usu�rio e a senha, e retorna um `IdentityResult`. Se o resultado for positivo, retornamos o usu�rio, caso contr�rio, lan�amos uma exce��o.

Dentre os dados automaticamente gerados pelo Identity, destacamos o `PasswordHash` e o `SecurityStamp`. O `PasswordHash` � o hash da senha do usu�rio, e o `SecurityStamp` � um valor aleat�rio que � gerado toda vez que o usu�rio � criado ou atualizado. O `SecurityStamp` � usado para invalidar os tokens de acesso, caso o usu�rio tenha alterado a senha, por exemplo. **N�o � correto nem seguro armazenar diretamete a senha do usu�rio no banco de dados**.

O m�todo `CreateAsync` modifica o objeto `user`, adicionando diversas propriedades. Por isso o `user` retornado pelo m�todo.

O Identity em si, por padr�o, possui fatores de seguran�a que exigem um crit�rio m�nimo para a senha, como tamanho m�nimo, caracteres especiais, etc. No entanto, podemos alterar esses crit�rios se necess�rio, como explica a [documenta��o](https://learn.microsoft.com/pt-br/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-7.0#password).

O `ApplicationException` � uma exce��o gen�rica que usamos para indicar que ocorreu um erro inesperado, o famoso erro 500.

## Movendo para um servi�o
N�o � uma boa pr�tica deixar a l�gica de neg�cio no controller. Por isso, vamos criar um servi�o para gerenciar os usu�rios:

```csharp
using API.Data.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Services;

public class AuthService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public AuthService(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<User> SignUp(SignUpDto dto)
    {
        User user = _mapper.Map<User>(dto);
        IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            throw new ApplicationException("Unknown error occurred");
        }

        return user;
        
    }
}
```


Agora, vamos alterar o controller:

```csharp
//...
public class UserController : ControllerBase
{
	private readonly AuthService _authService;

	public UserController(AuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("signup")]
	public async Task<ActionResult<User>> SignUp(SignUpDto dto)
	{
        return Ok(await _authService.SignUp(dto));
	}
}
//...
```

Tanto o `_mapper` quanto o `_userManager` s�o injetados automaticamente pelo ASP.NET Core por debaixo dos panos, ao configurarmos os pacotes. Para inje��es customizadas, precisamos fazer esse processo manualmente.

No `Program.cs`, adicionamos o servi�o:

```csharp
//...
var assembly = Assembly.GetExecutingAssembly();

var serviceTypes = assembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Service"))
    .ToList();

foreach (var serviceType in serviceTypes)
{
	builder.Services.AddScoped(serviceType);
}
//...
```

Por meio do Reflection, pegamos todos os tipos que terminam com `Service` e adicionamos ao container de servi�os. O Reflection � uma API que nos permite inspecionar e manipular tipos em tempo de execu��o, modificar e criar metadados (atributos, anota��es, etc), e muito mais.

Sobre a adi��o de depend�ncias, temos tr�s op��es:

- `AddTransient`: Uma inst�ncia � criada toda vez que o servi�o � requisitado.
- `AddScoped`: Uma inst�ncia � criada por requisi��o.
- `AddSingleton`: Uma inst�ncia � criada por aplica��o.

