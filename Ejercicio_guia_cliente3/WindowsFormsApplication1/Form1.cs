using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           
        }
        private void AtenderServidor()
        {
            while (true)
            {
                //recibimos la respuesta
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                //la troceamos para quedarnos con el codigo que nos interesa
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                //trozo[0] = codigo int
                // trozo [1] = respuesta char
                string mensaje = trozos[1].Split('\0')[0];

                //clasificamos la respuesta segun el codigo
                switch (codigo)
                {
                    case 1:  // respuesta a longitud

                        MessageBox.Show("La longitud de tu nombre es: " + mensaje);
                        break;
                    case 2:      //respuesta a si mi nombre es bonito

                        if (mensaje == "SI")
                            MessageBox.Show("Tu nombre ES bonito.");
                        else
                            MessageBox.Show("Tu nombre NO bonito. Lo siento.");
                        break;
                    case 3:       //Recibimos la respuesta de si soy alto

                        MessageBox.Show(mensaje);
                        break;
                    case 4:     //Recibimos notificacion

                        label4.Text = mensaje;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//conectar
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.103");
            IPEndPoint ipep = new IPEndPoint(direc, 9033);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Pink;
                MessageBox.Show("Conectado");



            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            
                if (Longitud.Checked)
                {
                    string mensaje = "1/" + nombre.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                    //Ya no recibimos aqui la respuesta del servidor
                   
                }
                else if (Bonito.Checked)
                {
                    string mensaje = "2/" + nombre.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);

                }
                else
                {
                     string mensaje = "3/" + nombre.Text + "/" + alturaBox.Text;
                    // Enviamos al servidor el nombre y altura tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);        
                    //MessageBox.Show(mensaje);
                }

          }

        private void button3_Click(object sender, EventArgs e)      //desconectar
        {
            // Se terminó el servicio. 
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            // Nos desconectamos
            atender.Abort();
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();

        }

        

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    //numero de servicios 
        //    string mensaje = "4/";

        //    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
        //    server.Send(msg);

        //    //Recibimos la respuesta del servidor
        //    byte[] msg2 = new byte[80];
        //    server.Receive(msg2);
        //    mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
        //    label1.Text = mensaje;
        //}

   

     
    }
}
