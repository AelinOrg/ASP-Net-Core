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
[Imagem](/assets/uri_url.png)