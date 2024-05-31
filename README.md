# Free-Trial
Прототип установшика приложения с лимитом на количество использования или время использования

Программа представляет собой оконное приложение, запрашивающее ФИО пользователя и заносящее эту информацию в текстовый файл names.txt на Рабочем столе. 

Если такое ФИО имеется в файле, то об этом выдаётся сообщение. После успешного ввода приложение выводит информацию о лимитах её использования.

Приложение имеет 2 режима лимитов использования:
+ ограничение по времени;
+ ограничение на количество запусков.

По умолчанию используется ограничение на количество запусков, равное 5. В случае 
исчерпания лимита программа предлагает пользователю приобрести её полную версию и 
перестаёт выполнять свою функцию.

Программа поставляется с помощью инсталлятора TiMP2_VJDronov Setup.exe. После установки в папке программы также предоставляется деинсталлятор unins000.exe.

При повторной установке программы приложение сверяется с прошлыми лимитами пользования, т.е. переустановка не позволяет суммарно их превысить.

Программа также имеет секретное меню администратора, в котором можно выбрать режим лимита использования, а также количественные параметры этого лимита (количество секунд для ограничения по времени и количество успешных вводов ФИО для ограничения на количество запусков).
