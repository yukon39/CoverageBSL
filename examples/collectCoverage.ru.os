#Использовать coveragebsl

Процедура ЗапуститьРаннер(АдресСервераОтладки)

	ПараметрыОтладки = Новый Массив();
	ПараметрыОтладки.Добавить("/Debug");
	ПараметрыОтладки.Добавить("-http");
	ПараметрыОтладки.Добавить("-attach");
	ПараметрыОтладки.Добавить("/DEBUGGERURL");
	ПараметрыОтладки.Добавить(АдресСервераОтладки);
	
	ПараметрыОтладкиСтрокой = СтрСоединить(ПараметрыОтладки, " ");
	
	ЧастиКоманднойСтроки = Новый Массив();
	ЧастиКоманднойСтроки.Добавить("vrunner.bat");
	ЧастиКоманднойСтроки.Добавить("run");
	ЧастиКоманднойСтроки.Добавить("--additional");
	ЧастиКоманднойСтроки.Добавить(СтрШаблон("""%1""", ПараметрыОтладкиСтрокой));
	
	КоманднаяСтрока = СтрСоединить(ЧастиКоманднойСтроки, " ");
	
	Сообщить("Командная строка: " + КоманднаяСтрока);
	Процесс = СоздатьПроцесс(КоманднаяСтрока);
	Процесс.Запустить();
	Процесс.ОжидатьЗавершения();

КонецПроцедуры

АдресСервераОтладки = "http://localhost:1550";
ИмяИнформационнойБазы = "DefAlias"; // Значение по умолчанию
ПарольОтладчика = "";

МенеджерПокрытия = Новый МенеджерПокрытия(АдресСервераОтладки);

МенеджерПокрытия.ПроверитьСоединение();

ВерсияAPI = МенеджерПокрытия.ВерсияAPI();
Сообщить("Версия API: " + ВерсияAPI);

Сессия = МенеджерПокрытия.НоваяСессияПокрытия(ИмяИнформационнойБазы);
Сессия.Подключить(ПарольОтладчика);

ИдентификаторСбора = Сессия.НачатьСборПокрытия();
Сообщить("Идентификатор сбора: " + ИдентификаторСбора);

ЗапуститьРаннер(АдресСервераОтладки);

ДанныеПокрытия = Сессия.ЗавершитьСборПокрытия();

Сессия.Отключить();

Сообщить("Общая продолжительность: " + ДанныеПокрытия.ОбщаяПродолжительность);
Сообщить("Количество данных: " + ДанныеПокрытия.Данные.Количество());

ЗаписьJSON = Новый ЗаписьJSON();
ЗаписьJSON.ОткрытьФайл("platformCoverage.json");
ДанныеПокрытия.СериализоватьJSON(ЗаписьJSON);
ЗаписьJSON.Закрыть();