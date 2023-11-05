# Introdu��o
Implementaremos um fluxo completo de usu�rio com o aux�lio do **Identity**, uma ferramenta da Microsoft que nos permite gerenciar autentica��o e autoriza��o de usu�rios em nossas aplica��es.

## O que � o Identity?
O Identity � um framework que nos permite gerenciar usu�rios, senhas, perfis e permiss�es, tokens, confirma��o de email, e muito mais.

## Iniciando...
Com o projeto de API configurado, e com a base de usu�rios criada (banco, pacotes, modelo, mapeamento, DTOs, etc), vamos come�ar a implementar o Identity. Antes, vale ressaltar alguns decorators:

- `[Compare]`: Usado na valida��o, � �til para, por exemplo, comparar a senha e a senha de confirma��o.
- `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`: Usado para indicar que o campo � gerado pelo banco de dados.
- `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]`: Usado para indicar que o campo � gerado pelo banco de dados, mas n�o � um campo de chave prim�ria.
- `[DataType(DataType.Password)]`: Usado para indicar que o campo � uma senha.


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

Agora, vamos criar um m�todo para efetuar o login no `AuthService`:

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

Como vemos, o Identity nos fornece o `SignInManager<User>`, que � respons�vel por gerenciar o login do usu�rio. O m�todo `PasswordSignInAsync` recebe o usu�rio, a senha, e um booleano que indica se o cookie deve ser persistido ("remember me") e, por fim, um booleano que indica se o usu�rio deve ser bloqueado caso o login falhe. O m�todo retorna um `SignInResult`, que indica se o login foi bem sucedido ou n�o. Caso exista alguma falha, lan�amos uma exce��o 401 por meio do `UnauthorizedAccessException`.

Al�m do `PasswordSignInAsync`, temos o `CheckPasswordSignInAsync`, que, diferente do m�todo usado, n�o possui a op��o relacionada aos cookies e, mais importante, n�o realiza o login. O `PasswordSignInAsync` oferece mais recursos, como a verifica��o de dois fatores, confirma��o de email, etc, por isso, atente-se ao m�todo escolhido e as configura��es. Ele possui uma sobrecarga, que recebe o username e faz a mesma checagem feita com o `FindByEmailAsync`. No nosso caso, estamos usando o email.

## Gerando o token
Come�aremos instalando o pacote `System.IdentityModel.Tokens.Jwt`. Com ele, podemos gerar tokens JWT, que � uma string de acesso que cont�m informa��es do usu�rio, como o username, o email, o id, etc. N�o � recomendado armazenar informa��es sens�veis, como a senha.

Agora, vamos criar um m�todo para gerar o token no `AuthService`:

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

N�o � seguro armazenar a chave de seguran�a diretamente no c�digo, como fizemos. O ideal � armazenar em um arquivo de configura��o, como o `appsettings.json`. No entanto, para fins did�ticos, deixaremos a chave, fraca, diretamente no c�digo.

Sobre as funcionalidades:
- `JwtSecurityTokenHandler`: � respons�vel por gerar o token.
- `SecurityTokenDescriptor`: � respons�vel por configurar o token. Nele, definimos o `Subject`, que � o usu�rio, a data de expira��o, e as credenciais de assinatura, que � a chave de seguran�a.
- `SigningCredentials`: � respons�vel por assinar o token. Nele, definimos o algoritmo de assinatura e a chave de seguran�a.
- `SecurityToken`: � o token em si no formato de objeto.
- `WriteToken`: � respons�vel por escrever o token.
- `Claim`: � uma informa��o do usu�rio que ser� armazenada no token. O `ClaimTypes` � uma classe que possui diversos tipos de claims, como o `Name`, `Email`, `NameIdentifier`, etc.

O m�todo `GenerateToken` ser� usado no m�todo `Login` do servi�o:

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

No controller, apenas corrige o retorno do m�todo:

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

Alternativamente, a gera��o do token poderia ser realizada da seguinte forma:

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

## Criando a pol�tica de acesso
Antes de prosseguirmos, instale o pacote `Microsoft.AspNetCore.Authentication.JwtBearer`. Ele � respons�vel por validar o token JWT.

Para fins de estudo, o nosso objetivo � criar uma pol�tica de acesso que exija que o usu�rio esteja autenticado para acessar determinadas rotas. Para isso, vamos criar uma classe `AuthPolicy` em `API/Authorization`. Note que, assim como para verifica��o de role, � poss�vel usar uma pol�tica pr�-existente, atrav�s do `AddPolicy`:

```csharp
//...
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
});
//...
```

Por�m, para fins de estudo, vamos criar uma pol�tica personalizada:

```csharp
using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class AuthPolicy: IAuthorizationRequirement
{
}
```

Agora, vamos criar um handler para a pol�tica:

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

O m�todo `HandleRequirementAsync` � respons�vel por verificar se o usu�rio est� autenticado. Caso esteja, a pol�tica � satisfeita, caso contr�rio, falha. O m�todo `Succeed` � respons�vel por indicar que a pol�tica foi satisfeita.

Agora, vamos registrar a pol�tica no `Program.cs`:

```csharp
//...
builder.Services.AddSingleton<IAuthorizationHandler, AuthPolicyHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuth", policy => policy.AddRequirements(new AuthPolicy()));
});
//...
```

Agora, vamos aplicar a pol�tica em uma rota:

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
Al�m da nossa pr�pria pol�tica, podemos utilizar outros recursos a partir do token JWT. Por exemplo, podemos usar o `Authorize` para verificar se o usu�rio est� autenticado e, se estiver, podemos usar o `Authorize(Roles = "Admin")` para verificar se o usu�rio possui a role de administrador. Para isso, precisamos configurar o `JwtBearerDefaults.AuthenticationScheme`:

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

Expliquemos os par�metros:
- `ValidateIssuer`: Indica se o emissor do token deve ser validado.
- `ValidateAudience`: Indica se o destinat�rio do token deve ser validado.
- `ValidateLifetime`: Indica se a validade do token deve ser validada.
- `ValidateIssuerSigningKey`: Indica se a chave de assinatura deve ser validada.
- `IssuerSigningKey`: Indica a chave de assinatura (a mesma usada para assinar o token).
- `ClockSkew`: Indica o tempo de toler�ncia para a validade do token. No nosso caso, estamos dizendo que o token deve ser v�lido at� o momento em que expira, sem toler�ncia.

Inicialmente, indicamos que o padr�o de autentica��o � o `JwtBearerDefaults.AuthenticationScheme`. Ao fim, usamos o `UseAuthentication` para indicar que a autentica��o ser� feita por meio do token JWT.

Essa configura��o � importante para outros cen�rios. Digamos que queremos obter a claim de data de nascimento do token. Para isso, podemos usar, dentre as op��es, o `HttpContext.User.Claims`, por exemplo, que retorna uma lista de claims. No entanto, se n�o configurarmos o `JwtBearerDefaults.AuthenticationScheme`, o `HttpContext.User` ser� nulo.

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
Quando trabalhamos com dados sens�veis, como chaves de seguran�a, senhas, etc, n�o � recomendado armazenar diretamente no c�digo. O ideal � armazenar em um arquivo de configura��o. A Microsoft oferece o `Secret Manager`, que � um gerenciador de segredos, e tamb�m por meio de vari�veis de ambiente. Veja a [documenta��o](https://learn.microsoft.com/pt-br/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows) para mais informa��es.

O gerenciador de segredos, usado por meio da linha de comando, � �til para desenvolvimento, mas n�o � recomendado para produ��o. Para produ��o, o ideal � usar vari�veis de ambiente.

Na nossa aplica��o atual, estamos, incorretamente, armazenando a chave de seguran�a e a connection string diretamente no c�digo. Vamos corrigir isso.

## Aplicando secrets ao projeto
A principio, iniciamos o Secret no nosso projeto, obtendo um secret user id (� inserido no arquivo `.csproj`):

```bash
dotnet user-secrets init
```

Em seguida, adicionamos a chave de seguran�a e a connection string:


```bash
dotnet user-secrets set "SigningKey" <chave>
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=<host>;user=<user>;password=<senha>;database=<db>"
```

Ser� criado um arquivo `secrets.json` na pasta `C:\Users\<user>\AppData\Roaming\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`.

Note que, para a connection string, usamos o formato `key:value`. Isso � necess�rio para que o Secret Manager consiga ler a connection string. Lembre-se de remove-la do `appsettings.json`.

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

E tamb�m no `GenerateToken` do `AuthService`:

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

Repare que o m�todo `GenerateToken` deixou de ser est�tico. Isso � necess�rio porque estamos acessando um membro da inst�ncia, o `_configuration`.

