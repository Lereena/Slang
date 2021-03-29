Slang
=====

Модельный язык с поддержкой фукнкциональных параметров.

.NET Core 3.1

[Пример программы на Slang](/a.txt).

Генерация лексера и парсера
---------------------------

Лексер и парсер генерируются из SimpleLex.lex и SimpleYacc.y. 

Linux (необходим установленный пакет Wine): 
``` sh
./genLex.sh
./genYacc.sh
```

Windows:
```
gplex.exe /unicode SimpleLex.lex
gppg.exe /no-lines /gplex SimpleYacc.y
```
