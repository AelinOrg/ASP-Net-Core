# Introdução
Implementaremos um fluxo completo de usuário com o auxílio do **Identity**, uma ferramenta da Microsoft que nos permite gerenciar autenticação e autorização de usuários em nossas aplicações.

## O que é o Identity?
O Identity é um framework que nos permite gerenciar usuários, senhas, perfis e permissões, tokens, confirmação de email, e muito mais.

## Iniciando...
Com o projeto de API configurado, e com a base de usuários criada (banco, pacotes, modelo, mapeamento, DTOs, etc), vamos começar a implementar o Identity. Antes, vale ressaltar alguns decorators:

- `[Compare]`: Usado na validação, é útil para, por exemplo, comparar a senha e a senha de confirmação.
- `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`: Usado para indicar que o campo é gerado pelo banco de dados.
- `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]`: Usado para indicar que o campo é gerado pelo banco de dados, mas não é um campo de chave primária.
- `[DataType(DataType.Password)]`: Usado para indicar que o campo é uma senha.


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

## Efetuando o login
Para efetuar o login, precisamos de um `LoginDto`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace API.Data.Dtos;

public class LoginDto
{
	[Required]
	public string Username { get; set; } = null!;

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; } = null!;
}
```

Agora, vamos criar um método para efetuar o login no `AuthService`:

```csharp
//...

namespace API.Services;

public class AuthService
{
    //...
    private readonly SignInManager<User> _signInManager;

    public AuthService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        //...
        _signInManager = signInManager;
    }

    //...

    public async Task<User> Login(LoginDto dto)
    {
        User? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        return user;
    }
}
```

Como vemos, o Identity nos fornece o `SignInManager<User>`, que é responsável por gerenciar o login do usuário. O método `PasswordSignInAsync` recebe o usuário, a senha, e um booleano que indica se o cookie deve ser persistido ("remember me") e, por fim, um booleano que indica se o usuário deve ser bloqueado caso o login falhe. O método retorna um `SignInResult`, que indica se o login foi bem sucedido ou não. Caso exista alguma falha, lançamos uma exceção 401 por meio do `UnauthorizedAccessException`.

Além do `PasswordSignInAsync`, temos o `CheckPasswordSignInAsync`, que, diferente do método usado, não possui a opção relacionada aos cookies e, mais importante, não realiza o login. O `PasswordSignInAsync` oferece mais recursos, como a verificação de dois fatores, confirmação de email, etc, por isso, atente-se ao método escolhido e as configurações. Ele possui uma sobrecarga, que recebe o username e faz a mesma checagem feita com o `FindByEmailAsync`. No nosso caso, estamos usando o email.

## Gerando o token
Começaremos instalando o pacote `System.IdentityModel.Tokens.Jwt`. Com ele, podemos gerar tokens JWT, que é uma string de acesso que contém informações do usuário, como o username, o email, o id, etc. Não é recomendado armazenar informações sensíveis, como a senha.

Agora, vamos criar um método para gerar o token no `AuthService`:

```csharp
//...
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class AuthService
{
	//...

	private static string GenerateToken(User user)
	{
		JwtSecurityTokenHandler tokenHandler = new();
		byte[] key = Encoding.ASCII.GetBytes("hash");

		SecurityTokenDescriptor tokenDescriptor = new()
		{
			Subject = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id)
			}),
			Expires = DateTime.UtcNow.AddDays(7),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

		return tokenHandler.WriteToken(token);
	}
}
```

Não é seguro armazenar a chave de segurança diretamente no código, como fizemos. O ideal é armazenar em um arquivo de configuração, como o `appsettings.json`. No entanto, para fins didáticos, deixaremos a chave, fraca, diretamente no código.

Sobre as funcionalidades:
- `JwtSecurityTokenHandler`: É responsável por gerar o token.
- `SecurityTokenDescriptor`: É responsável por configurar o token. Nele, definimos o `Subject`, que é o usuário, a data de expiração, e as credenciais de assinatura, que é a chave de segurança.
- `SigningCredentials`: É responsável por assinar o token. Nele, definimos o algoritmo de assinatura e a chave de segurança.
- `SecurityToken`: É o token em si no formato de objeto.
- `WriteToken`: É responsável por escrever o token.
- `Claim`: É uma informação do usuário que será armazenada no token. O `ClaimTypes` é uma classe que possui diversos tipos de claims, como o `Name`, `Email`, `NameIdentifier`, etc.

O método `GenerateToken` será usado no método `Login` do serviço:

```csharp
//...
public class AuthService
{
	//...
	public async Task<string> Login(LoginDto dto)
	{
		//...
		return GenerateToken(user);
	}
}
```

No controller, apenas corrige o retorno do método:

```csharp
//...
public class UserController : ControllerBase
{
	//...
	[HttpPost("login")]
	public async Task<ActionResult<string>> Login(LoginDto dto)
	{
		return Ok(await _authService.Login(dto));
	}
}
```

Alternativamente, a geração do token poderia ser realizada da seguinte forma:

```csharp
//...
private static string GenerateToken(User user)
{
    byte[] key = Encoding.ASCII.GetBytes("hash");

    var token = new JwtSecurityToken
    (
        claims: new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Name!),
            new Claim(ClaimTypes.NameIdentifier, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        },
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
//...
```
# Controle de acesso

## Criando a política de acesso
Antes de prosseguirmos, instale o pacote `Microsoft.AspNetCore.Authentication.JwtBearer`. Ele é responsável por validar o token JWT.

Para fins de estudo, o nosso objetivo é criar uma política de acesso que exija que o usuário esteja autenticado para acessar determinadas rotas. Para isso, vamos criar uma classe `AuthPolicy` em `API/Authorization`. Note que, assim como para verificação de role, é possível usar uma política pré-existente, através do `AddPolicy`:

```csharp
//...
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
});
//...
```

Porém, para fins de estudo, vamos criar uma política personalizada:

```csharp
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class AuthPolicy: IAuthorizationRequirement
{
}
```

Agora, vamos criar um handler para a política:

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class AuthPolicyHandler: AuthorizationHandler<AuthPolicy>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthPolicy requirement)
	{
		if (context.User.Identity?.IsAuthenticated == true)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}
```

O método `HandleRequirementAsync` é responsável por verificar se o usuário está autenticado. Caso esteja, a política é satisfeita, caso contrário, falha. O método `Succeed` é responsável por indicar que a política foi satisfeita.

Agora, vamos registrar a política no `Program.cs`:

```csharp
//...
builder.Services.AddSingleton<IAuthorizationHandler, AuthPolicyHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuth", policy => policy.AddRequirements(new AuthPolicy()));
});
//...
```

Agora, vamos aplicar a política em uma rota:

```csharp
//...
[HttPost("test")]
[Autorize("RequireAuth")]
public ActionResult Test()
{
	return Ok("Test");
}
//...
```

## Autenticando via token
Além da nossa própria política, podemos utilizar outros recursos a partir do token JWT. Por exemplo, podemos usar o `Authorize` para verificar se o usuário está autenticado e, se estiver, podemos usar o `Authorize(Roles = "Admin")` para verificar se o usuário possui a role de administrador. Para isso, precisamos configurar o `JwtBearerDefaults.AuthenticationScheme`:

```csharp
//...
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("hash")),
            ClockSkew = TimeSpan.Zero
		};
	});
//...

app.UseAuthentication();
```

Expliquemos os parâmetros:
- `ValidateIssuer`: Indica se o emissor do token deve ser validado.
- `ValidateAudience`: Indica se o destinatário do token deve ser validado.
- `ValidateLifetime`: Indica se a validade do token deve ser validada.
- `ValidateIssuerSigningKey`: Indica se a chave de assinatura deve ser validada.
- `IssuerSigningKey`: Indica a chave de assinatura (a mesma usada para assinar o token).
- `ClockSkew`: Indica o tempo de tolerância para a validade do token. No nosso caso, estamos dizendo que o token deve ser válido até o momento em que expira, sem tolerância.

Inicialmente, indicamos que o padrão de autenticação é o `JwtBearerDefaults.AuthenticationScheme`. Ao fim, usamos o `UseAuthentication` para indicar que a autenticação será feita por meio do token JWT.

Essa configuração é importante para outros cenários. Digamos que queremos obter a claim de data de nascimento do token. Para isso, podemos usar, dentre as opções, o `HttpContext.User.Claims`, por exemplo, que retorna uma lista de claims. No entanto, se não configurarmos o `JwtBearerDefaults.AuthenticationScheme`, o `HttpContext.User` será nulo.

Agora, vamos aplicar o `Authorize` em uma rota:

```csharp
//...
[HttpGet("test")]
[Authorize]
public ActionResult Test()
{
	return Ok("Test");
}
//...
```

# Utilizando secrets

## O problema de dados expostos
Quando trabalhamos com dados sensíveis, como chaves de segurança, senhas, etc, não é recomendado armazenar diretamente no código. O ideal é armazenar em um arquivo de configuração. A Microsoft oferece o `Secret Manager`, que é um gerenciador de segredos, e também por meio de variáveis de ambiente. Veja a [documentação](https://learn.microsoft.com/pt-br/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows) para mais informações.

O gerenciador de segredos, usado por meio da linha de comando, é útil para desenvolvimento, mas não é recomendado para produção. Para produção, o ideal é usar variáveis de ambiente.

Na nossa aplicação atual, estamos, incorretamente, armazenando a chave de segurança e a connection string diretamente no código. Vamos corrigir isso.

## Aplicando secrets ao projeto
A principio, iniciamos o Secret no nosso projeto, obtendo um secret user id (é inserido no arquivo `.csproj`):

```bash
dotnet user-secrets init
```

Em seguida, adicionamos a chave de segurança e a connection string:


```bash
dotnet user-secrets set "SigningKey" <chave>
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=<host>;user=<user>;password=<senha>;database=<db>"
```

Será criado um arquivo `secrets.json` na pasta `C:\Users\<user>\AppData\Roaming\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`.

Note que, para a connection string, usamos o formato `key:value`. Isso é necessário para que o Secret Manager consiga ler a connection string. Lembre-se de remove-la do `appsettings.json`.

Agora, vamos alterar o `Program.cs`:

```csharp
//...
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

//...
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //...
            IssuerSigningKey = new SymmetricSecurityKey
            (
                Encoding.ASCII.GetBytes(builder.Configuration["SigningKey"])
            ),
        };
    });
//...
```

E também no `GenerateToken` do `AuthService`:

```csharp
//...

namespace API.Services;

public class AuthService
{
    //...
    private readonly IConfiguration _configuration;


    public AuthService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        //...
        _configuration = configuration;
    }

    //...

    private string GenerateToken(User user)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.ASCII.GetBytes(_configuration["SigningKey"]);

        //...
    }
}
```

Repare que o método `GenerateToken` deixou de ser estático. Isso é necessário porque estamos acessando um membro da instância, o `_configuration`.

