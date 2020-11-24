#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <pthread.h>
#include <stdio.h>
int cont;//contador de servicios
//acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
//hacemos accesibles los sockets
int i;
int sockets[100];


void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	int terminar=0; //variable de conexion
	//bucle de atencion de varias peticiones
	while (terminar ==0)
	{
		//recibir peticion
		ret= read(sock_conn,peticion,sizeof(peticion));
		printf("Recibido\n");
		
		peticion[ret]='\0'; //fin de string
		printf ("Peticion: %s\n",peticion);
		
		//recogemos el codigo de la peticion
		char *p=strtok(peticion,"/");
		int cod = atoi(p);
		
		char nom[20];//lo q esta despues del codigo
		
		if((cod!= 0) && (cod!=4))
		{
			p =strtok(NULL,"/");//recogemos la info despues del codigo
			strcpy(nom,p);
			printf("codigo: %d, Nombre:%s\n", cod,nom);
		
		}
		if (cod==0)// desconexion
					terminar=1;
/*			else if (cod==4) *///cuantos servicios
/*					sprintf(respuesta, "%d",cont);*/
		else if(cod==1)//logitud del nombre
				sprintf(respuesta,"1/%d",strlen(nom));
		else if(cod==2)//mi nombre es bonito
				if((nom[0]=='M') || (nom[0]=='S'))
						strcpy(respuesta,"2/SI");
				else
						strcpy(respuesta,"2/NO");
				else
		    	{
	         		p= strtok (NULL,"/");
					float altura = atof(p);
					if (altura > 1.70)
						sprintf(respuesta, "3/%s: eres alto", nom);
					else 
						sprintf(respuesta, "3/%s: eres bajo",nom);
				}
		        if (cod != 0)
				{
					printf("Respuesta:%s\n",respuesta);
					//para enviar respuesta
					write (sock_conn,respuesta, strlen(respuesta));
				}
			
				if ((cod==1)||(cod==2)||(cod==3))
				{
					pthread_mutex_lock( &mutex ); //No me interrumpas ahora
					cont= cont +1;
					pthread_mutex_unlock( &mutex); //ya puedes interrumpirme
					//notificar
					char notificacion[20];
					sprintf (notificacion, "4/%d",cont);
					int j;
					for (j=0; j< i; j++)
						write (sockets[j],notificacion, strlen(notificacion));
				}
	}			
	close(sock_conn);
}


int main(int argc, char *argv[])
{
	
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	

	// INICIALITZACIONS
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
		
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// establecemos el puerto de escucha
	serv_adr.sin_port = htons(9036);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
    cont =0;
	pthread_t thread;
	int i=0;
	for (;;){
		printf ("Escuchando\n");
		
        //sock_conn es el socket que usaremos para este cliente
		sock_conn=accept(sock_listen,NULL,NULL);
		printf("He recibido la conexion\n");
		
		sockets[i] = sock_conn;
		
		pthread_create(&thread,NULL,AtenderCliente,&sockets[i]);
		i++;
		}
	
	//for (i=0; i<5; i++)
	//pthread_join (thread[i], NULL);
}
