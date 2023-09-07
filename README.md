# SolucionWebAppGestorPrestamos

- Instrucciones para DataBase First
  Add-Migration InitialCreate -Project WebApp
  update-database -Project WebApp

- Intrucciones para Comenzar de BD existente
  Scaffold-DbContext “server=192.168.1.21; port=3306; database=gestion_creditos ;user=root; password=root;”
  Pomelo.EntityFrameworkCore.MySql -o Models

PASOS a SEGUIR en caso de eliminar la APP
-Crear nueva ASP NET Core Web App (MVC) con autenticacion individual
-Modificar appsettings.json, cambiar DefaultConnection
-Modificar Program.cs, cambiar forma conectar MySql
-Instalar Pomelo.EntityFrameworkCore.Mysql 6 y Microsoft.EntityFramewore.Tools 6
-Cuidado con tablas de usuarios de la BD - Desde la migracion no se puede, tomar la resguardada
"Ejecutar la migracion desde Nuget-CMD update-database"
-Ejecutar Scaffold-DbContext
-Eliminar/Renombrar clases en Models
-Agregar anotaciones [ValidateNever], para los parametros de tipo objeto
-Copiar DbSet del nuevo DBContext al que existe
-Renombrar DBSet por los valores de las tablas (estan en ingles y el plural agrega S)
-Hacer build de la aplicacion
-Posicionarse sobre Controllers y generar nuevos con Entityframework

PASO habilitar ambiente AWS
-Crear maquina EC2 Ubuntu 20.04, 1GB RAM, 10GB Disco, 1CPU
--Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker
docker --version
--Instalar Docker-Compose
sudo curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
docker-compose --version

#refrescar docker
nano docker-compose.yaml

#Imagenes docker utilizadas
docker pull jwilder/nginx-proxy:1.3.1-alpine
docker pull linuxserver/duckdns:version-1c126a78
docker pull jrcs/letsencrypt-nginx-proxy-companion:2.2.8
docker pull mysql:8.1.0

#ejecutar contenedores
docker-compose up -d

#ver log de contenedores
docker ps -a
docker logs <<Nombre instancia>>

#bajar contenedores
docker-compose down

#actualizar las imagenes de docker-compose para traer lo ultimo del docker-hub
docker-compose pull
#modificar el docker-componse para dejar las versiones fijas de las imagenes que se usan en el resto de componente
#sino refresca con la version latest

#Comando conectar EC2 AWS
ssh -i "archivo.pem" ubuntu@ip-instacia-aws

#ver recursos de la instacia EC2 AWS
htop

#para usar JMETER, entrar a CMD con perfil administrador

#publicar desde visual studio hacia dockerhub

#guardar codigo en github desde studio code
