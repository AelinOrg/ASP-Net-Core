# Conhecendo o protocolo HTTP
O HTTP (HyperText Transfer Protocol) � um protocolo de comunica��o que permite que aplica��es web se comuniquem. Ele funciona como um protocolo de requisi��o-resposta no modelo cliente-servidor. O cliente faz uma requisi��o para o servidor e o servidor responde essa requisi��o. O HTTP � um protocolo *stateless*, ou seja, n�o guarda estado entre requisi��es. Isso significa que cada requisi��o que o cliente faz para o servidor, � independente das outras requisi��es. O servidor n�o sabe se o cliente fez uma requisi��o antes ou n�o.

## Conhecendo a arquitetura do HTTP
O HTTP � um protocolo que funciona em camadas. A camada mais baixa � a camada de transporte, que � respons�vel por transportar os dados entre o cliente e o servidor. O HTTP est� na camada de aplica��o, ou seja, roda sobre o protocolo de transporte TCP/IP. O HTTP � um protocolo de texto, ou seja, as mensagens s�o leg�veis por humanos. Isso facilita o entendimento e a depura��o de problemas.

**O que seria um protocolo?**
Para responder a essa pergunta, usaremos uma analogia: um protocolo � como se fosse uma conversa. Se o nosso navegador pudesse enviar mensagens de WhatsApp para o servidor, a conversa entre eles seria como abaixo:

```
Navegador: Ol�, servidor! poderia me enviar a p�gina inicial do seu site?
Servidor: Aqui est�! (<html>...</html>)
Navegador: Obrigado! Agora, poderia me enviar o javascript da p�gina?
Servidor: Aqui est�! (<script>...</script>)
```

Como vemos, um servidor n�o � necessariamente um backend. Um frontend pode ser interpretado como um servidor, pois ele recebe requisi��es e envia respostas, disponibilizando recursos para o cliente, como o HTML, CSS, JavaScript, imagens, etc.

### Regras do protocolo HTTP
A primeira regra � que sempre deve haver duas entidades dialogando, sempre um cliente e um servidor.

Essa conversa � sempre iniciada pelo cliente. � o cliente quem pedir� o HTML, por exemplo, e n�o o contr�rio. O servidor n�o decidir� enviar informa��es ao cliente por conta pr�pria.

Ap�s a requisi��o do cliente, o servidor envia uma resposta com o que foi solicitado. O protocolo HTTP � baseado em mensagens de texto que seguem uma estrutura espec�fica. Esse texto pode ser lido tanto por pessoas quanto por m�quinas.

Por fim, temos uma camada sobre a qual o HTTP roda, ou seja, o Transmission Control Protocol - TCP. Essa � a camada de transporte, relevante para garantirmos que as mensagens HTTP chegar�o ao seu destino com sucesso. Com isso, nenhuma das duas entidades ficar� sem resposta.

Dentro desse contexto de comunica��o, o cliente pode ser qualquer aplica��o que fa�a requisi��es HTTP, como um navegador, um aplicativo mobile, um software de linha de comando, etc. O servidor pode ser qualquer aplica��o que responda a essas requisi��es, como um servidor web, um servidor de arquivos, um servidor de imagens, etc.

## Peer-To-Peer
Voc� j� usou torrent para baixar algum arquivo na internet? Caso sim, aproveitou um outro modelo de comunica��o, o P2P ou Peer-To-Peer!

O modelo Cliente-Servidor n�o � o �nico modelo de comunica��o na rede, nem sempre o mais adequado. Por exemplo, imagine que precisemos contar as letras de 20 palavras. No caso do modelo Cliente-Servidor, quem far� esse trabalho � o servidor, certo? E se precisar contar as letras de 1 milh�o de palavras? Muito trabalhoso para o servidor, n�o?

O modelo Cliente-Servidor tenta centralizar o trabalho no servidor, mas isso tamb�m pode gerar gargalos. Se cada Cliente pudesse ajudar no trabalho, ou seja, assumir um pouco da responsabilidade do servidor, seria muito mais r�pido. Essa � a ideia do P2P! N�o h� mais uma clara divis�o entre Cliente-Servidor, cada cliente tamb�m � servidor e vice-versa!

Isto � �til quando voc� precisa distribuir um trabalho ou necessita baixar algo de v�rios lugares diferentes. Faz sentido?

Usando algum aplicativo de Torrent, o protocolo utilizado n�o � o HTTP, e sim o protocolo P2P, como BitTorrent ou Gnutella.

## Outros protocolos
O HTTP � o protocolo mais utilizado na internet, mas n�o � o �nico. Existem outros protocolos que tamb�m s�o muito utilizados, como o P2P, FTP, SMTP, POP3, IMAP, etc. Cada um desses protocolos � utilizado para um prop�sito diferente. O HTTP � utilizado para transferir arquivos, o FTP � utilizado para transferir arquivos tamb�m, mas com mais recursos, como autentica��o, por exemplo. O SMTP � utilizado para enviar e-mails, o POP3 e o IMAP s�o utilizados para receber e-mails.

# Aprendendo sobre URLs
A seguir, veremos como funciona uma URL, que � a forma como acessamos um recurso na web. Por exemplo, quando acessamos o site da Alura, digitamos a URL https://www.alura.com.br. Essa URL � composta por v�rios elementos, que veremos a seguir.

## Entendendo a estrutura de uma URL
Uma URL (Uniform Resource Locator), por exemplo, http://localhost:3000/ � composta por v�rios elementos:
- `http://` � o protocolo utilizado para acessar o recurso. Nesse caso, o protocolo � o HTTP, mas poderia ser o HTTPS, FTP, etc.
- `localhost` � o dom�nio do recurso. Nesse caso, o dom�nio � `localhost`, mas poderia ser www.alura.com.br, www.google.com, etc.
- `:3000` � a porta utilizada para acessar o recurso. Nesse caso, a porta � 3000.
- `/` � o caminho do recurso. Nesse caso, o caminho � a raiz do site.

## URI ou URL?
Muitas vezes, desenvolvedores usam a sigla URI (Uniform Resource Identifier) quando falam de endere�os na web. Alguns preferem URL (Uniform Resource Locator), e alguns misturam as duas siglas � vontade. H� uma certa confus�o no mercado a respeito e mesmo desenvolvedores experientes n�o sabem explicar a diferen�a. Ent�o, qual � a diferen�a?

- **Resposta 1 (f�cil)**: Uma URL � uma URI. No contexto do desenvolvimento web, ambas as siglas s�o v�lidas para falar de endere�os na web. As siglas s�o praticamente sin�nimos e s�o utilizadas dessa forma.

- **Resposta 2 (mais elaborada)**: Uma URL � uma URI, mas nem todas as URI's s�o URL's! Existem URI's que identificam um recurso sem definir o endere�o, nem o protocolo. Em outras palavras, uma URL representa uma identifica��o de um recurso (URI) atrav�s do endere�o, mas nem todas as identifica��es s�o URL's.

Existe um outro padr�o que se chama URN (Uniform Resource Name). Os URN's tamb�m s�o URI's! Um URN segue tamb�m uma sintaxe bem definida, algo assim **urn:cursos:alura:course:introducao-html-css**. Repare que criamos uma outra identifica��o do curso Introdu��o ao HTML e CSS da Alura, mas essa identifica��o n�o � um endere�o. 
![Imagem](./assets/uri_url.png)

## Acessando diferentes portas
Quando acessamos um site, geralmente n�o precisamos informar a porta utilizada. Por exemplo, quando acessamos o site da Alura, digitamos a URL https://www.alura.com.br. Nesse caso, n�o precisamos informar a porta utilizada, pois o navegador j� sabe que a porta padr�o para o protocolo HTTP � a porta 80. Poder�amos acessar o site da Alura informando a porta, como http://www.alura.com.br:80, mas n�o � necess�rio.

Dentre as portas padr�es, podemos citar:
- HTTP: 80
- HTTPS: 443
- Livres para uso: de 1023 at� 65535

Portas entre 0 e 1023 s�o reservadas para servi�os padronizados.

## Entendendo dom�nios
Acessando a nossa aplica��o pelo navegador no endere�o `localhost:3000`, temos o nosso servidor como sendo o `localhost` (nome de dom�nio de onde estamos acessando o servidor) e a porta `3000`.

O site `google.com` � outro nome de dom�nio, que conseguimos acessar de forma direta sem a necessidade de informar a porta (por ser padr�o), no entanto, podemos utilizar o IP:
```
http://142.251.128.14
```

Que nos leva a `google.com`.

**DNS**
O DNS (Domain Name System) � um sistema de gerenciamento de nomes de dom�nios. Ele � respons�vel por traduzir um nome de dom�nio para um endere�o IP. Por exemplo, quando acessamos o site da Alura, digitamos a URL https://www.alura.com.br. O DNS � respons�vel por traduzir o nome de dom�nio www.alura.com.br para o endere�o IP.

Na web, cada site, ou sistema, ou empresa possui seu pr�prio dom�nio, sendo onde tudo � controlado. Por exemplo, todos os sistemas da Alura est�o sobre o dom�nio de alura.com.br. H� dom�nios globais e dom�nios locais (localhost).

Um servidor DNS � uma arvore hier�rquica de nomes de dom�nio, pois este tipo de estrutura permite que os nomes de dom�nio sejam organizados de forma a facilitar a sua administra��o. Por exemplo, o dom�nio:
- `www.alura.com.br` � um subdom�nio do dom�nio `alura.com.br`	
	- `alura.com.br` � um subdom�nio do dom�nio `com.br`
		- `com.br` � um subdom�nio do dom�nio `br`

Quando acessamos algo `.com.br` ou `.org`, significa que s�o o primeiro n�vel dos nomes dos sites. Inicia-se da raiz (n�vel mais abstrato) que � para ter um ponto de onde come�ar, e a partir disso vamos descendo os n�veis. Chamamos esse n�vel de top-level domains (TLD) (em portugu�s, "dom�nios de n�vel superior").
![Imagem](./assets/dns.png)

# Inspecionando o protocolo HTTP
A seguir, veremos como inspecionar o protocolo HTTP, para entendermos melhor como ele funciona.

## Modo hacker
Para entender melhor a estrutura do HTTP, usaremos a ferramenta `telnet` para realizar requisi��es em baixo n�vel, conectando via TCP com servidores (de sistema ou servidor). Para isso, com a ferramenta instalada e front/back rodando, abra o terminal e digite:

```
telnet localhost 3000
```

Onde `localhost` � o dom�nio e `3000` � a porta. Ap�s isso, digite:

```
GET / HTTP/1.1 
(enter 2x)
```

Onde `GET` � o m�todo, `/` � o caminho e `HTTP/1.1` � a vers�o do protocolo. Ap�s isso, o servidor ir� responder com o HTML da p�gina inicial.

Para um exemplo de POST, faremos o login no sistema. Para isso, digite:

```
POST /public/login HTTP/1.1
Content-Type: application/json
Content-length: 45

{"email": "geo@alura.com.br", "senha": "123"}
```

Obtendo a resposta:

```
HTTP/1.1 200 OK
X-Powered-By: Express
Vary: Origin, Accept-Encoding
(...)
Content-Type: application/json
Content-Length: 364

{
    "access_token: "eyJhiJ...WCbZof2rf",
    (...)
}
```

Noque que as mensagens HTTP tem esse formato de **texto** espec�fico com detalhes da requisi��o, cabe�alhos e corpo, tanto para requisi��es quanto para respostas.

## Depurando m�todos HTTP
Resumidamente, temos os seguintes m�todos HTTP, popularmente conhecidos como verbos CRUD:
- **GET**: obter um recurso
- **POST**: criar um recurso
- **PUT**: atualizar um recurso
- **DELETE**: remover um recurso
- **PATCH**: atualizar parcialmente um recurso

### PUT e PATCH
A diferen�a entre PUT e PATCH � que o PUT � utilizado para atualizar um recurso por completo, enquanto o PATCH � utilizado para atualizar parcialmente um recurso. Por exemplo, se temos um recurso com os campos `nome` e `email`, e queremos atualizar apenas o campo `nome`, devemos utilizar o PATCH. Se quisermos atualizar os campos `nome` e `email`, devemos utilizar o PUT.

## Cabe�alhos e autentica��o
Os servidores HTTP s�o stateless. Em outras palavras, os servidores HTTP n�o guardam estados, portanto n�o se lembram do aconteceu em requisi��es anteriores. No caso, ele n�o lembra que j� havia nos autenticado no sistema.

A implica��o � que precisamos continuamente comprovar ao servidor quem somos e que j� fomos autenticados.

Podemos fazer isso de duas formas, com o uso de **cookies** ou com o uso de **sess�es**. 

### Sess�es
Como o nome sugere, uma sess�o � um per�odo de tempo em que o cliente mant�m uma conex�o com o servidor, por meio de recursos como session storage ou local storage.

Ap�s signup ou login, o servidor nos envia um token, que � um c�digo que comprova que j� fomos autenticados. Esse token � enviado no cabe�alho da requisi��o, no campo `Authorization`. A estrutura do cabe�alho � de um objeto, isto �, chave e valor:

```
{
    "Authorization": "Bearer <token>"
}
```

Repare que o token � precedido pela palavra `Bearer`. Essa palavra � utilizada para indicar que o token � um token de acesso. Existem outros tipos de tokens, como o token de atualiza��o, por exemplo.

### Cookies
Um cookie � um pequeno arquivo de texto, normalmente criado pela aplica��o web, para guardar algumas informa��es sobre o usu�rio no navegador. Quais s�o essas informa��es depende um pouco da aplica��o. Pode ser que fique gravado alguma prefer�ncia do usu�rio, informa��es sobre as compras na loja virtual ou a identifica��o do usu�rio. Isso depende da utilidade para a aplica��o web.

## Depurando os c�digos resposta HTTP
Por ser um protocolo, o HTTP segue algumas regras, no caso, o [RFC 7231](https://www.rfc-editor.org/rfc/rfc7231). Neste documento, podemos encontrar os c�digos de resposta HTTP divididos em 5 classes:
- **1xx**: Informativo
- **2xx**: Sucesso
- **3xx**: Redirecionamento
- **4xx**: Erro cometido pelo cliente
- **5xx**: Erro cometido pelo servidor
- 
# Protegendo nossa aplica��o com HTTPS
Ao utilizar o HTTP, as informa��es trafegam entre o cliente e o servidor sem criptografia. Isso significa que qualquer pessoa mal intencionada pode interceptar essas informa��es e roubar dados sens�veis, como senhas, por exemplo. Para evitar esse tipo de problema, utilizamos o HTTPS.

Para migrar nossa aplica��o para HTTPS, precisamos de um certificado SSL e uma chave privada. O certificado SSL � um arquivo que cont�m informa��es sobre a nossa aplica��o, como o nome do dom�nio, por exemplo. A chave � utilizada para criptografar as informa��es que trafegam entre o cliente e o servidor.

Para fins de desenvolvimento, podemos utilizar o pacote `openssl` para gerar um certificado e uma chave privada. Para isso, basta executar o comando abaixo:

```
openssl req -x509 -sha256 -nodes -days 365 -newkey rsa:4096 -keyout key.key -out cert.crt
```

Esse comando ir� gerar um certificado e uma chave privada v�lidos por 365 dias. Ap�s executar esse comando, teremos dois arquivos: `cert.crt` e `key.key`.

Em sistemas servidores, importamos o m�dulo respons�vel por lidar com o HTTPS, e passamos o certificado e a chave privada como par�metro. Por exemplo, no Node.js, podemos fazer isso com o seguinte c�digo:

```js
const https = require('https');
const fs = require('fs');

//...

const cert = fs.readFileSync('cert.crt');
const key = fs.readFileSync('key.key');

https.createServer({ cert, key }, app).listen(3000, () => {
  console.log('Servidor HTTPS rodando na porta 3000');
});
```
## Entendendo certificados e chaves privadas
Fazendo uma analogia, podemos dizer que o certificado SSL � como se fosse o RG da nossa aplica��o, a identidade utilizada para entrar na empresa na qual trabalhamos, por exemplo. A chave privada � como se fosse a chave do nosso escrit�rio, a qual nunca compartilhamos ou fazemos c�pias.

Podemos analisar o conte�do do certificado e da chave privada com o comando abaixo:

```
openssl x509 -in cert.crt -text -noout
```

```
openssl rsa -in key.key -text 
```

Durante uma requisi��o HTTPS, o cliente envia uma mensagem para o servidor, solicitando o certificado SSL. O servidor envia o certificado SSL para o cliente. O cliente verifica se o certificado SSL � v�lido. Se for v�lido, o cliente gera uma **chave de sess�o** e criptografa essa chave com a chave p�blica contida no certificado, e envia para o servidor. O servidor descriptografa essa chave com a chave privada e envia a chave de sess�o para o cliente. A partir desse momento, o cliente e o servidor utilizam a chave de sess�o para criptografar e descriptografar as informa��es que trafegam entre eles.

Essas idas e vindas de mensagens entre o cliente e o servidor s�o chamadas de **handshake**. O handshake � um processo que ocorre sempre que o cliente e o servidor precisam estabelecer uma conex�o segura.

## Para saber mais: as chaves do HTTPS
O HTTPS usa uma chave p�blica e uma chave privada. As chaves est�o **ligadas matematicamente**, <u>o que foi cifrado pela chave p�blica s� pode ser decifrado pela chave privada</u>. Isso garante que os dados cifrados pelo navegador (chave p�blica) s� podem ser lidos pelo servidor (chave privada). Como temos duas chaves diferentes envolvidas, esse m�todo de criptografia � chamado de **criptografia assim�trica**. No entanto, a criptografia assim�trica tem um problema, ela � **lenta**.

Por outro lado, temos a **criptografia sim�trica**, que usa a mesma chave para cifrar e decifrar os dados, como na vida real, onde usamos a mesma chave para abrir e fechar a porta. A criptografia sim�trica � muito mais r�pida.

Agora, o interessante � que o HTTPS usa ambos os m�todos de criptografia, assim�trica e sim�trica.

No certificado, vem a chave p�blica para o cliente utilizar, certo? E o servidor continua na posse da chave privada, ok? Isso � seguro, mas lento e por isso o cliente gera uma chave sim�trica ao vivo (**chave de sess�o**). Uma chave s� para ele e o servidor com o qual est� se comunicando naquele momento. Essa chave exclusiva (e sim�trica) � ent�o enviada para o servidor utilizando a criptografia assim�trica (chave privada e p�blica) e ent�o � utilizada para o restante da comunica��o.

Ent�o, HTTPS come�a com criptografia assim�trica para depois mudar para criptografia sim�trica. Essa chave sim�trica ser� gerada no in�cio da comunica��o e ser� reaproveitada nas requisi��es seguintes.

# Controlando o HTTP
A seguir, veremos como controlar o HTTP, para que possamos utilizar o protocolo da melhor forma poss�vel.

## Para saber mais: enviando mais de um par�metro via GET
Podemos utilizar as query strings para enviar par�metros em uma URL, por exemplo fazendo da seguinte forma: `http://localhost/livros?categoria=3`. Mas e se al�m da categoria, tamb�m quis�ssemos filtrar pelo autor? Nesse caso, far�amos assim: `/livros?categoria=3&autor=1`. Ou seja, utilizamos o caractere `&` para separar os nomes dos par�metros que configuramos.

Pensando agora em outro exemplo, mais complexo, poder�amos ter a seguinte URL:

```
http://eletronicos.com/products?search=TV&maxPrice=1000&brand=ACME&model=XPTO&delivery=free&
```

O interessante de ter os par�metros na URL, � que al�m de ficar �bvio que o request � um GET, fica f�cil para o usu�rio compartilhar a busca que ele fez, diferente do POST, ou qualquer outro m�todo onde os par�metros v�o no corpo.

A especifica��o do HTTP n�o define um n�mero m�ximo de par�metros para uma URL, e nem mesmo um tamanho m�ximo para a URL (conforme a se��o 3.2.1 da [RFC 2616](https://www.ietf.org/rfc/rfc2616.txt)). Por isso, cada navegador e servidor pode implementar o seu pr�prio limite m�ximo, embora haja uma conven��o de suportar at� 2000 caracteres. Portanto, quando estiver criando seus requests, caso eles estejam muito grandes, considere convert�-los para um POST, ou talvez refatorar as URLs para simplificar o envio de par�metros.

## Para saber mais: par�metros na URL
Se desejarmos, por exemplo, exibir um produto espec�fico, podemos fazer isso utilizando um par�metro query `id`, por exemplo: 

```
http://localhost:3000/produtos?id=1
```

No entanto, podemos utilizar um par�metro de rota. Por exemplo:

```
http://localhost:3000/produtos/1
```

Para isso, precisamos definir a rota da seguinte forma:

```js
app.get('/produtos/:id', (req, res) => {
  const { id } = req.params;
  //...
});
```

Ou seja, definimos que ap�s a rota `/produtos/`, esperamos por uma informa��o que representa o `id` do produto.

## Configurando o formato dos dados
Ao receber a resposta do servidor, podemos identificar o formato dos dados atrav�s do cabe�alho `Content-Type`. Por exemplo, se o servidor enviar um JSON, o cabe�alho `Content-Type` ser� `application/json`. Se o servidor enviar um HTML, o cabe�alho `Content-Type` ser� `text/html`.

Para especificar um formato de dados, podemos utilizar o cabe�alho `Accept`. 

## Resolvendo algumas limita��es do HTTP
Tanto o HTTP/1.1 quanto o HTTP/2 trabalham em cima do TCP.

As maiores diferen�as entre esses dois protocolos � que o primeiro deixa a desejar as requisi��es sequenciais, ou seja, cada vez que fazemos um REQUEST, temos que esperar terminar para come�ar o outro no contexto de uma conex�o TCP, que � o canal por onde as mensagens passam.

Conseguiremos verificar isso pelo DevTools do Chrome, na aba Network, onde podemos ver o tempo de cada requisi��o.

O HTTP/2, por sua vez, permite que as requisi��es sejam feitas em paralelo, o que melhora muito a performance. Isso � poss�vel porque o HTTP/2 trabalha com o conceito de multiplexa��o, que � a capacidade de enviar v�rias requisi��es ao mesmo tempo e junt�-las em uma mesma conex�o TCP.

Para isso, devemos habilitar o HTTP/2 tanto no servidor quanto no cliente. 

Al�m disso, o HTTP/2 possui o mecanismo de **compacta��o de cabe�alho**, em que o que era leg�vel em formato de texto passa por um algoritmo de compress�o e se torna um cabe�alho bin�rio que pode ser enviado pela rede e economizar recursos. Podemos reparar isso observando o cabe�alho, que possui as chaves em min�sculo, por exemplo.

Outro recurso interessante do HTTP/2 � o **server push**, que permite que o servidor envie recursos para o cliente antes mesmo que ele solicite. Por exemplo, se o servidor enviar um HTML para o cliente, e esse HTML faz refer�ncia a um arquivo CSS, o servidor pode enviar esse arquivo CSS para o cliente antes mesmo que ele solicite.

## Conhecendo o HTTP3
O HTTP/3 � a nova vers�o do protocolo HTTP, que est� em desenvolvimento. O HTTP/3 trabalha em cima do QUIC, uma implementa��o do UDP, que � um protocolo de transporte mais leve que o TCP. O HTTP/3 tamb�m possui o mecanismo de multiplexa��o, compacta��o de cabe�alho e server push.

Para criar uma conex�o segura com o HTTP/1.1 ou HTTP/2, passamos pelas etapas de comunica��o TCP, TLS e, por fim, HTTP (requisi��es HTTPS). No HTTP/3, passamos pelas etapas de comunica��o UDP, TLS e, por fim, HTTP (requisi��es HTTPS). O HTTP/3 faz uso do QUIC, que tem embutido o TLS, o que torna a comunica��o mais r�pida, isto �, menos etapas.