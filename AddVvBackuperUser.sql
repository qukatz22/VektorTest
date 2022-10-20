Desni gumb misa u (MySQL Administrator.UserAccounts.SchemaPrivileges.Schemata)
pa 'Add Schema with Wildcards'
pa 'vs%'
#-------------------------------------------------
#Nakon sto dodas superuser-au Vektor-u:

GRANT ALL PRIVILEGES ON *.* TO 'superuser'@"%" WITH GRANT OPTION



GRANT SHOW DATABASES, SELECT, LOCK TABLES, RELOAD ON *.* to vvbackuper@localhost IDENTIFIED BY 'vvbackuper'; FLUSH PRIVILEGES;

#blat -serverSMTP mail.htnet.hr -f viper@zg.htnet.hr -to roberman007@gmail.com -subject Testis -body pero

MySQL Vatriables adjustment after install:

1. Startup variables - Advanced - Make table names: "2- Store as created, case insensitive"


# REPLICATION ============================================

GRANT REPLICATION SLAVE ON *.* TO 'vvreplicator'@'%' IDENTIFIED BY 'qweqwe';

Kada radis snapshot: osim vsurger i vs2009_000001_JORDAN i sl., iskopiraj i 
tablice 'user' i 'db' iz mysql databasea na slave-a

Rem: 'readonly' mod rada dbServera sljaka samo na userima koji nemaju 'SUPER' privileges (dakle root iil superuser moze svasta...)
