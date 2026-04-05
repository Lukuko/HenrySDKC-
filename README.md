# 🧩 Henry SDK C#

Implementação em C# de uma classe para comunicação com catracas que utilizam o SDK da HenryAgros, baseada na implementação em PHP feita pelo [juliolvfilho](https://github.com/juliolvfilho/henry-php) :

Este projeto serve como um guia prático para integração entre hardware (catracas Henry) e aplicações .NET, facilitando o uso do protocolo de comunicação.

## 📌 Sobre o Projeto

O Henry SDK C# fornece uma base simples e reutilizável para desenvolvedores que precisam:

- Integrar catracas Henry em sistemas próprios
- Controlar acesso de usuários
- Realizar comunicação com dispositivos físicos via socket TCP
- Entender a estrutura do protocolo da Henry

## ⚙️ Tecnologias Utilizadas
C# <br>
.NET <br>

## 🚀 Como Usar

### 1. Clone o repositório
git clone https://github.com/Lukuko/HenrySDKC-.git

### 2. Exemplo básico de uso

```C#
var sdk = new HenrySdk();

sdk.OnEventoRecebido += (index, comando) =>
{
    Console.WriteLine($"Evento recebido: {index} - {comando}");
};

sdk.Connect("192.168.0.100", 3000);

// Liberar acesso
sdk.Liberar("01", 5, "LIBERADO");

// Bloquear acesso
sdk.Bloquear("01", 5);

// Enviar comando bruto
sdk.SendRaw("01", "ALGUM_COMANDO");

// Encerrar conexão
sdk.Disconnect();
``` 
## 🔌 Funcionamento

A comunicação com a catraca ocorre via TCP Socket, utilizando um protocolo proprietário com a seguinte estrutura:

[STX][SIZE][PAYLOAD][CHECKSUM][ETX]

## 📦 Estrutura da mensagem

| Byte | Descrição |
| - | - |
| `STX (0x02)` | Início da mensagem |
| `SIZE (2 bytes)` | Tamanho do payload |
| `PAYLOAD `| Dados no formato index + comando |
| `CHECKSUM `| XOR dos bytes do payload |
| `ETX (0x03) `| Fim da mensagem |

# 📡 Eventos
OnEventoRecebido

Evento disparado quando uma mensagem válida é recebida da catraca:

```C#
sdk.OnEventoRecebido += (index, comando) =>
{
    // index = identificador do equipamento
    // comando = payload completo recebido
};
```

# 🧠 Métodos Disponíveis
| Método | Descrição | Argumentos do método |
| - | - | - |
| `Connect()` | Inicia uma nova conexão com o dispositivo via Socket Tcp. | `String Ip, int Port` |
| ` Disconnect() ` | Encerra a conexão atual com o dispositivo | Não recebe nenhum Argumento |
| `ProcessarMensagem()` | Recebe os bytes enviados pelo dispositivo e converte em texto usando a decodificação em ASCII | `byte[] msg` |
| `Enviar() `| Realiza a conversão do texto da mensagem para bytes <br> seguindo o padrão do protocolo usando a codificação em ASCII e envia a mesma para o dispositivo | `string index, string comando` |
| ` Liberar() `| Método possuindo o comando de liberação pré estabelecido <br> sendo possível informar o tempo que a catraca irá aguardar até ser realizado o movimento de liberação e a mensagem a aparecer no visor  | `string index,int release_time, string mensagem` |
| `Bloquear()`| Método possuindo o comando de bloqueio pré estabelecido <br> sendo possível informar o tempo que a catraca irá exibir a mensagem de bloqueio e a mensagem a aparecer no visor(que no momento está fixo como negado)  | `string index, int release_time, string mensagem = "NEGADO"` |
| `SendRaw() `| Envio "Cru" da mensagem, para realizar envio de alguma mensagem customizada | `string index, string comando` |

### ⚠️ Observações Importantes!

1. A catraca deve estar acessível via rede (IP e porta corretos)
2. O protocolo exige estrutura rígida (STX/ETX + checksum)
3. O SDK não possui reconexão automática na versão atual
4. Testes completos exigem hardware real


## 🧠 Objetivo

Este projeto não é uma biblioteca oficial, mas sim uma forma de auxiliar quaisquer interessados que precisem integrar com esse dispositivo, assim como a implementação em php me auxiliou quando precisei, espero que esse código possa lhe auxiliar durante a sua integração.
Existem planos de melhorar essa biblioteca mais para frente, então sempre consultem as releases da mesma para verificar novas modificações

## 👨‍💻 Autor

Desenvolvido por:

Davi Menezes

## 🗣️Contato:
Caso necessite entrar em contato sinta-se a vontade para abrir uma nova issue ou me contatar através da seguintes redes:

[LinkedIn](https://www.linkedin.com/in/davi-menezes-42a883239/)

## 🤝 Contribuições

Contribuições sempre serão bem-vindas!

Abra uma issue
Sugira melhorias
Envie um pull request

### 🗒️Licensa:
Este Software/Repositório está disponível sobre a licensa Licença Pública Geral GNU (GNU GPL) de 29 de Junho de 2007
