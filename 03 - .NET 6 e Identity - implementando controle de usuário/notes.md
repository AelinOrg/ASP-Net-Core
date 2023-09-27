# Introdução
Implementaremos um fluxo completo de usuário com o auxílio do **Identity**, uma ferramenta da Microsoft que nos permite gerenciar autenticação e autorização de usuários em nossas aplicações.

## O que é o Identity?
O Identity é um framework que nos permite gerenciar usuários, senhas, perfis e permissões, tokens, confirmação de email, e muito mais.

## Iniciando...
Com o projeto de API configurado, e com a base de usuários criada (banco, pacotes, modelo, mapeamento, DTOs, etc), vamos começar a implementar o Identity. Antes, vale ressaltar alguns decorators:

`[Compare]`: Usado na validação, é útil para, por exemplo, comparar a senha e a senha de confirmação.
`[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`: Usado para indicar que o campo é gerado pelo banco de dados.
`[DatabaseGenerated(DatabaseGeneratedOption.Computed)]`: Usado para indicar que o campo é gerado pelo banco de dados, mas não é um campo de chave primária.

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

O `IdentityDbContext` já possui as tabelas necessárias para o Identity. Come ela, toda a responsabilidade de gerenciar os usuários fica por conta do Identity.

Um ponto interessante, poderíamos usar diretamente a classe `IdentityUser`, mas como queremos adicionar campos personalizados, criamos uma classe `User` que herda de `IdentityUser`:

```csharp
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User: IdentityUser
{
    public DateTime BirthDate { get; set; }
}
```

A classe `IdentityUser` já possui alguns campos, como `Id`, `UserName`, `Email`, `PasswordHash`, `SecurityStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, etc. Além disso, podemos adicionar campos personalizados, como `BirthDate`.

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
- `AddIdentity<User, IdentityRole>()`: Diz ao Identity aplicar o conceito de identidade para a classe `User` e que a parte de autorização será gerenciada também por ele.
- `AddEntityFrameworkStores<ApiDbContext>()`: Diz ao Identity usamos o contexto `ApiDbContext` para nos comunicarmos com o banco de dados.
- `AddDefaultTokenProviders()`: Diz ao Identity a gestão de tokens será responsabilidade dele.

Nesse ponto, já podemos gerar a migration com `Add-Migration InitialCreate` e atualizar o banco com `Update-Database`.

## Cadastrando um usuário
Nosso `SignUpDTO` está assim:

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

Agora, vamos criar um método para cadastrar um usuário:

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

Além do `_mapper`, injetamos o `UserManager<User>`. Ele é responsável por gerenciar os usuários, como criar, atualizar, deletar, etc. 

Para criar um usuário, basta chamar o método `CreateAsync`, que recebe o usuário e a senha, e retorna um `IdentityResult`. Se o resultado for positivo, retornamos o usuário, caso contrário, lançamos uma exceção.

Dentre os dados automaticamente gerados pelo Identity, destacamos o `PasswordHash` e o `SecurityStamp`. O `PasswordHash` é o hash da senha do usuário, e o `SecurityStamp` é um valor aleatório que é gerado toda vez que o usuário é criado ou atualizado. O `SecurityStamp` é usado para invalidar os tokens de acesso, caso o usuário tenha alterado a senha, por exemplo. **Não é correto nem seguro armazenar diretamete a senha do usuário no banco de dados**.

O método `CreateAsync` modifica o objeto `user`, adicionando diversas propriedades. Por isso o `user` retornado pelo método.

O Identity em si, por padrão, possui fatores de segurança que exigem um critério mínimo para a senha, como tamanho mínimo, caracteres especiais, etc. No entanto, podemos alterar esses critérios se necessário, como explica a [documentação](https://learn.microsoft.com/pt-br/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-7.0#password).

O `ApplicationException` é uma exceção genérica que usamos para indicar que ocorreu um erro inesperado, o famoso erro 500.

## Movendo para um serviço
Não é uma boa prática deixar a lógica de negócio no controller. Por isso, vamos criar um serviço para gerenciar os usuários:

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

Tanto o `_mapper` quanto o `_userManager` são injetados automaticamente pelo ASP.NET Core por debaixo dos panos, ao configurarmos os pacotes. Para injeções customizadas, precisamos fazer esse processo manualmente.

No `Program.cs`, adicionamos o serviço:

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

Por meio do Reflection, pegamos todos os tipos que terminam com `Service` e adicionamos ao container de serviços. O Reflection é uma API que nos permite inspecionar e manipular tipos em tempo de execução, modificar e criar metadados (atributos, anotações, etc), e muito mais.

Sobre a adição de dependências, temos três opções:

- `AddTransient`: Uma instância é criada toda vez que o serviço é requisitado.
- `AddScoped`: Uma instância é criada por requisição.
- `AddSingleton`: Uma instância é criada por aplicação.

