using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace HenrySdk
{
    public class HenrySdk
    {
        private TcpClient client;
        private NetworkStream stream;
        private bool running;

        private const int BUFFER_SIZE = 1024;

        public event Action<string, string> OnEventoRecebido;

        public void Connect(string ip, int port)
        {
            client = new TcpClient();
            client.Connect(ip, port);

            stream = client.GetStream();
            running = true;

            Console.WriteLine(true);

            Task.Run(() => Escutar());
        }

        public void Disconnect()
        {
            running = false;

            try
            {
                stream?.Close();
                client?.Close();
                Console.WriteLine(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Escutar()
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            while (running)
            {
                try
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);

                    if (bytes <= 0)
                        continue;

                    byte[] mensagem = new byte[bytes];
                    Array.Copy(buffer, mensagem, bytes);

                    ProcessarMensagem(mensagem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao ler socket: " + ex.Message);
                    running = false;
                }
            }
        }

        public void ProcessarMensagem(byte[] msg)
        {
            if (msg.Length < 6)
                return;

            if (msg[0] != 0x02 || msg[msg.Length - 1] != 0x03)
                return;

            int size = msg[1];

            string payload = Encoding.ASCII.GetString(msg, 3, size);

            Console.WriteLine($"RAW: {payload}");

            string[] partes = payload.Split('+');

            if (partes.Length < 2)
                return;

            string index = partes[0];
            string comando = payload;

            OnEventoRecebido?.Invoke(index, comando);
        }

        public void Enviar(string index, string comando)
        {
            if (stream == null)
                return;

            string data = $"{index}+{comando}";

            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            byte[] size = new byte[2];
            size[0] = (byte)dataBytes.Length;
            size[1] = 0;

            byte[] payload = new byte[size.Length + dataBytes.Length];

            Buffer.BlockCopy(size, 0, payload, 0, size.Length);
            Buffer.BlockCopy(dataBytes, 0, payload, size.Length, dataBytes.Length);

            byte checksum = payload[0];

            for (int i = 1; i < payload.Length; i++)
                checksum ^= payload[i];

            byte[] full = new byte[payload.Length + 3];

            full[0] = 0x02;
            Buffer.BlockCopy(payload, 0, full, 1, payload.Length);
            full[payload.Length + 1] = checksum;
            full[payload.Length + 2] = 0x03;

            stream.Write(full, 0, full.Length);

            Console.WriteLine($"ENVIADO: {data}");
        }

        public void Liberar(string index,int release_time, string mensagem)
        {
            Enviar(index, $"REON+00+6]{release_time}]{mensagem}]2");
        }

        public void Bloquear(string index, int release_time, string mensagem = "NEGADO")
        {
            Enviar(index, $"REON+00+30]{release_time}]{mensagem}]1");
        }

        public void SendRaw(string index, string comando)
        {
            Enviar(index, comando);
        }
    }
}
