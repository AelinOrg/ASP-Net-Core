# Conhecendo o protocolo HTTP
O HTTP (HyperText Transfer Protocol) é um protocolo de comunicação que permite que aplicações web se comuniquem. Ele funciona como um protocolo de requisição-resposta no modelo cliente-servidor. O cliente faz uma requisição para o servidor e o servidor responde essa requisição. O HTTP é um protocolo *stateless*, ou seja, não guarda estado entre requisições. Isso significa que cada requisição que o cliente faz para o servidor, é independente das outras requisições. O servidor não sabe se o cliente fez uma requisição antes ou não.

## Conhecendo a arquitetura do HTTP
O HTTP é um protocolo que funciona em camadas. A camada mais baixa é a camada de transporte, que é responsável por transportar os dados entre o cliente e o servidor. O HTTP está na camada de aplicação, ou seja, roda sobre o protocolo de transporte TCP/IP. O HTTP é um protocolo de texto, ou seja, as mensagens são legíveis por humanos. Isso facilita o entendimento e a depuração de problemas.

**O que seria um protocolo?**
Para responder a essa pergunta, usaremos uma analogia: um protocolo é como se fosse uma conversa. Se o nosso navegador pudesse enviar mensagens de WhatsApp para o servidor, a conversa entre eles seria como abaixo:

```
Navegador: Olá, servidor! poderia me enviar a página inicial do seu site?
Servidor: Aqui está! (<html>...</html>)
Navegador: Obrigado! Agora, poderia me enviar o javascript da página?
Servidor: Aqui está! (<script>...</script>)
```

Como vemos, um servidor não é necessariamente um backend. Um frontend pode ser interpretado como um servidor, pois ele recebe requisições e envia respostas, disponibilizando recursos para o cliente, como o HTML, CSS, JavaScript, imagens, etc.

### Regras do protocolo HTTP
A primeira regra é que sempre deve haver duas entidades dialogando, sempre um cliente e um servidor.

Essa conversa é sempre iniciada pelo cliente. É o cliente quem pedirá o HTML, por exemplo, e não o contrário. O servidor não decidirá enviar informações ao cliente por conta própria.

Após a requisição do cliente, o servidor envia uma resposta com o que foi solicitado. O protocolo HTTP é baseado em mensagens de texto que seguem uma estrutura específica. Esse texto pode ser lido tanto por pessoas quanto por máquinas.

Por fim, temos uma camada sobre a qual o HTTP roda, ou seja, o Transmission Control Protocol - TCP. Essa é a camada de transporte, relevante para garantirmos que as mensagens HTTP chegarão ao seu destino com sucesso. Com isso, nenhuma das duas entidades ficará sem resposta.

Dentro desse contexto de comunicação, o cliente pode ser qualquer aplicação que faça requisições HTTP, como um navegador, um aplicativo mobile, um software de linha de comando, etc. O servidor pode ser qualquer aplicação que responda a essas requisições, como um servidor web, um servidor de arquivos, um servidor de imagens, etc.

## Peer-To-Peer
Você já usou torrent para baixar algum arquivo na internet? Caso sim, aproveitou um outro modelo de comunicação, o P2P ou Peer-To-Peer!

O modelo Cliente-Servidor não é o único modelo de comunicação na rede, nem sempre o mais adequado. Por exemplo, imagine que precisemos contar as letras de 20 palavras. No caso do modelo Cliente-Servidor, quem fará esse trabalho é o servidor, certo? E se precisar contar as letras de 1 milhão de palavras? Muito trabalhoso para o servidor, não?

O modelo Cliente-Servidor tenta centralizar o trabalho no servidor, mas isso também pode gerar gargalos. Se cada Cliente pudesse ajudar no trabalho, ou seja, assumir um pouco da responsabilidade do servidor, seria muito mais rápido. Essa é a ideia do P2P! Não há mais uma clara divisão entre Cliente-Servidor, cada cliente também é servidor e vice-versa!

Isto é útil quando você precisa distribuir um trabalho ou necessita baixar algo de vários lugares diferentes. Faz sentido?

Usando algum aplicativo de Torrent, o protocolo utilizado não é o HTTP, e sim o protocolo P2P, como BitTorrent ou Gnutella.

## Outros protocolos
O HTTP é o protocolo mais utilizado na internet, mas não é o único. Existem outros protocolos que também são muito utilizados, como o P2P, FTP, SMTP, POP3, IMAP, etc. Cada um desses protocolos é utilizado para um propósito diferente. O HTTP é utilizado para transferir arquivos, o FTP é utilizado para transferir arquivos também, mas com mais recursos, como autenticação, por exemplo. O SMTP é utilizado para enviar e-mails, o POP3 e o IMAP são utilizados para receber e-mails.

# Aprendendo sobre URLs
A seguir, veremos como funciona uma URL, que é a forma como acessamos um recurso na web. Por exemplo, quando acessamos o site da Alura, digitamos a URL https://www.alura.com.br. Essa URL é composta por vários elementos, que veremos a seguir.

## Entendendo a estrutura de uma URL
Uma URL (Uniform Resource Locator), por exemplo, http://localhost:3000/ é composta por vários elementos:
- `http://` é o protocolo utilizado para acessar o recurso. Nesse caso, o protocolo é o HTTP, mas poderia ser o HTTPS, FTP, etc.
- `localhost` é o domínio do recurso. Nesse caso, o domínio é `localhost`, mas poderia ser www.alura.com.br, www.google.com, etc.
- `:3000` é a porta utilizada para acessar o recurso. Nesse caso, a porta é 3000.
- `/` é o caminho do recurso. Nesse caso, o caminho é a raiz do site.

## URI ou URL?
Muitas vezes, desenvolvedores usam a sigla URI (Uniform Resource Identifier) quando falam de endereços na web. Alguns preferem URL (Uniform Resource Locator), e alguns misturam as duas siglas à vontade. Há uma certa confusão no mercado a respeito e mesmo desenvolvedores experientes não sabem explicar a diferença. Então, qual é a diferença?

- **Resposta 1 (fácil)**: Uma URL é uma URI. No contexto do desenvolvimento web, ambas as siglas são válidas para falar de endereços na web. As siglas são praticamente sinônimos e são utilizadas dessa forma.

- **Resposta 2 (mais elaborada)**: Uma URL é uma URI, mas nem todas as URI's são URL's! Existem URI's que identificam um recurso sem definir o endereço, nem o protocolo. Em outras palavras, uma URL representa uma identificação de um recurso (URI) através do endereço, mas nem todas as identificações são URL's.

Existe um outro padrão que se chama URN (Uniform Resource Name). Os URN's também são URI's! Um URN segue também uma sintaxe bem definida, algo assim **urn:cursos:alura:course:introducao-html-css**. Repare que criamos uma outra identificação do curso Introdução ao HTML e CSS da Alura, mas essa identificação não é um endereço. 
[Imagem](/assets/uri_url.png)