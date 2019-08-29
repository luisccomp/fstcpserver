open System
open System.Net
open System.Net.Sockets
open System.Text

[<EntryPoint>]
let main args =
    (* Criando uma variável mutável para poder realizar algumas operações no objeto    *)
    (* TcpListener. Por motivos de boas práticas, no paradigma funcional o uso de      *)
    (* variáveis mutávels é desencorajado.                                             *)
    let mutable server: TcpListener = null
    
    try
        try
            (* Configurando o servidor no endereço 127.0.0.1 e na porta 23456. O       *)
            (* deve ser a primeira aplicação a rodar antes de instanciar os clientes   *)
            (* ou eles não vão funcionar corretamente.                                 *)
            let addr = IPAddress.Parse("127.0.0.1") in
            let port = 23456 in
            server <- TcpListener(addr, port)

            (* Começa a escutar por conexões dos clientes. *)
            server.Start()

            (* Loop principal da aplicação servidor. Este loop é aonde as requisições  *)
            (* dos clientes serão processadas e as respostas produzidas.               *)
            while true do
                Console.WriteLine("Aguardando conexão...")
                (* Realiza uma chamada bloqueante para aceitar requisições. Você       *)
                (* também pode chamar server.AcceptSocket() aqui.                      *)
                let client = server.AcceptTcpClient()
                Console.WriteLine("Conectado...")

                (* Abrindo o canal de comunicação com o cliente. *)
                let stream = client.GetStream() in
                let req = Array.create 1024 0uy in
                let i = ref 0 in
                let readBytes () =
                    i := stream.Read(req, 0, req.Length)
                    !i

                while readBytes() <> 0 do
                    (* Traduz a sequência de bytes lidos em uma string UTF-8. *)
                    let request = Encoding.UTF8.GetString(req, 0, !i) in
                    Console.WriteLine("Recebido: {0}", request)

                    (* Processando os dados enviados pelo cliente. *)
                    let response = request.ToUpper() in
                    let res = Encoding.UTF8.GetBytes(response) in

                    (* Enviando de volta uma resposta. *)
                    stream.Write(res, 0, res.Length)
                    Console.WriteLine("Enviado: {0}", response)

        with :? SocketException as e ->
            Console.WriteLine("Socket Exception: {0}", e)
    finally
        (* Encerrando o servidor. *)
        server.Stop()

    Console.WriteLine("Pressione qualquer tecla para continuar...")
    Console.ReadKey() |>
    ignore

    0
